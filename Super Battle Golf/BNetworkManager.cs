using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Cysharp.Threading.Tasks;
using Mirror;
using Mirror.FizzySteam;
using Steamworks;
using Steamworks.Data;
using UnityEngine;
using UnityEngine.Localization;
using UnityEngine.Localization.Settings;
using UnityEngine.Pool;
using UnityEngine.SceneManagement;
using kcp2k;

public class BNetworkManager : NetworkManager, IBUpdateCallback, IAnyBUpdateCallback
{
	public const string steamInviteConnectionCommand = "connect_lobby";

	public const string steamCustomConnectionCommandLineArgument = "+connectSteamLobby";

	public const string steamRichPresenceConnectKey = "connect";

	public const string steamRichPresenceIsHostingKey = "sbg_isHosting";

	public const string steamRichPresenceTrueValue = "true";

	public const string steamRichPresenceFalseValue = "false";

	public const string steamLobbyLobbyNameKey = "lobbyName";

	public const string steamLobbyMaxPlayersKey = "maxPlayers";

	public const string steamLobbyCurrentPlayerCountKey = "currentPlayerCount";

	public const string steamLobbyHostLocationKey = "hostLocation";

	public const string steamLobbyCourseKey = "currentCourse";

	public const string steamLobbyCourseProgressKey = "currentCourseProgress";

	public const string steamLobbyCourseLengthKey = "currentCourseLength";

	public const string steamLobbyPasswordRequired = "sbg_passwordRequired";

	public const string steamLobbyModeKey = "sbg_lobbyMode";

	public const int defaultMaxPlayers = 16;

	private const float clientTimeout = 10f;

	public static readonly Vector3 playerInitialTempPosition = Vector3.one * 100000f;

	private bool isAcceptingConnectionsInternal;

	private static Lobby? steamLobby;

	private static CancellationTokenSource setLobbyPingLocationCancellationTokenSource;

	private Transport transportInternal;

	private bool isSimulatingLatency;

	public readonly Dictionary<int, ulong> playerGuidPerConnectionId = new Dictionary<int, ulong>();

	public readonly Dictionary<ulong, int> connectionIdPerPlayerGuid = new Dictionary<ulong, int>();

	private readonly List<int> serverConnectionIds = new List<int>();

	private readonly Dictionary<int, int> serverConnectionIdIndices = new Dictionary<int, int>();

	private readonly HashSet<ulong> showedConnectionMessage = new HashSet<ulong>();

	private bool isChangingScene;

	private static LobbyMode lobbyMode = LobbyMode.Friends;

	private static string lobbyName = string.Empty;

	private Dictionary<ulong, Relationship> steamPlayerRelationships = new Dictionary<ulong, Relationship>();

	private bool pollingSteamPlayerRelationships;

	public BClientAuthenticator ClientAuthenticator { get; private set; }

	public static bool IsChangingScene
	{
		get
		{
			if (singleton != null)
			{
				return singleton.isChangingScene;
			}
			return false;
		}
	}

	public static bool IsShuttingDown { get; private set; }

	public static bool IsServerShuttingDown { get; private set; }

	public static bool IsChangingSceneOrShuttingDown
	{
		get
		{
			if (!IsShuttingDown)
			{
				return IsChangingScene;
			}
			return true;
		}
	}

	public static bool IsOrWasServer
	{
		get
		{
			if (!IsServerShuttingDown)
			{
				return NetworkServer.active;
			}
			return true;
		}
	}

	public static int MaxPlayers { get; private set; } = 16;

	public static ulong LocalPlayerGuidOnServer { get; private set; }

	public static List<int> ServerConnectionIds
	{
		get
		{
			if (!(singleton != null))
			{
				return null;
			}
			return singleton.serverConnectionIds;
		}
	}

	public static bool IsAcceptingConnections
	{
		get
		{
			if (singleton != null && singleton.isAcceptingConnectionsInternal)
			{
				return !IsOffline;
			}
			return false;
		}
	}

	public static bool IsOffline
	{
		get
		{
			if (SteamEnabler.IsSteamEnabled)
			{
				if (SteamClient.IsValid)
				{
					return !SteamClient.IsLoggedOn;
				}
				return true;
			}
			return false;
		}
	}

	public new static BNetworkManager singleton => (BNetworkManager)NetworkManager.singleton;

	public static LobbyMode LobbyMode => lobbyMode;

	public static string LobbyName
	{
		get
		{
			if (!string.IsNullOrWhiteSpace(lobbyName))
			{
				return lobbyName;
			}
			return DefaultLobbyName;
		}
		set
		{
			GameManager.FilterProfanity(value, out var filteredString);
			lobbyName = filteredString;
			if (steamLobby.HasValue)
			{
				steamLobby.Value.SetData("lobbyName", LobbyName);
			}
		}
	}

	public static string DefaultLobbyName => string.Format(Localization.UI.LOBBY_BROWSER_DefaultLobbyName, SteamClient.Name);

	public static string SteamLobbyIdToConnectToFromMainMenu { get; private set; }

	public static event Action ConnectedToInternet;

	public static event Action DisconnectedFromInternet;

	public static event Action LobbyOpened;

	public static event Action LobbyClosed;

	public static event Action LobbyJoined;

	public static event Action LobbyLeft;

	public static event Action LobbyStateChanged;

	public static event Action DestroyedDueToOfflineSceneTransition;

	public static event Action WillChangeScene;

	public static event Action<ulong> OnServerFriendshipConfirmed;

	public static event Action<ulong, Relationship> SteamPlayerRelationshipChanged;

	[CCommand("simulateLatency", "", false, false)]
	public static void SimulateLatency(bool simulate)
	{
		if (singleton != null)
		{
			singleton.SimulateLatencyInternal(simulate);
		}
	}

	[CCommand("connect_lobby", "", false, false, hidden = true)]
	public static void ConnectToSteamLobbyFromInvite(string lobbyId)
	{
		SteamLobbyIdToConnectToFromMainMenu = lobbyId;
	}

	[CCommand("triggerNetworkClientException", "Causes the server to send a message to this client that will throw an exception", false, false)]
	private static void TriggerNetworkClientException()
	{
		NetworkClient.Send(default(RequestNetworkServerToClientExceptionMessage));
	}

	[CCommand("triggerNetworkClientToServerException", "Sends the server a message that will throw an exception", false, false)]
	private static void TriggerNetworkClientToServerException()
	{
		NetworkClient.Send(default(TriggerNetworkClientToServerExceptionMessage));
	}

	public override void Awake()
	{
		base.Awake();
		BUpdate.RegisterCallback(this);
		ClientAuthenticator = GetComponent<BClientAuthenticator>();
		offlineSceneLoadDelay = 0.5f;
		AntiCheatRateChecker.PlayerSuspiciousActivityDetected += OnPlayerSuspiciousActivityDetected;
		AntiCheatRateChecker.PlayerConfirmedCheatingDetected += OnPlayerConfirmedCheatingDetected;
		if (!SteamEnabler.IsSteamEnabled)
		{
			SetTransport(TransportType.Kcp);
			return;
		}
		SetTransport(TransportType.Steam);
		SteamUser.OnSteamServersConnected += OnSteamConnectedToInternet;
		SteamUser.OnSteamServersDisconnected += OnSteamDisconnectedFromInternet;
		SteamFriends.OnGameRichPresenceJoinRequested += OnSteamGameRichPresenceJoinRequested;
		SteamFriends.OnGameLobbyJoinRequested += OnSteamGameLobbyJoinRequested;
		SteamMatchmaking.OnLobbyDataChanged += OnSteamLobbyDataChanged;
	}

	public override void OnDestroy()
	{
		base.OnDestroy();
		BUpdate.DeregisterCallback(this);
		bool num = IsDueToOfflineSceneTransition();
		if (num)
		{
			Debug.Log("BNetworkManager was destroyed due to transition to offline scene");
		}
		else if (!GameManager.IsApplicationQuitting)
		{
			Debug.LogError("BNetworkmanager was destroyed for an unknown reason. It should only ever be destroyed when transitioning to offline scene");
		}
		AntiCheatRateChecker.PlayerSuspiciousActivityDetected -= OnPlayerSuspiciousActivityDetected;
		AntiCheatRateChecker.PlayerConfirmedCheatingDetected -= OnPlayerConfirmedCheatingDetected;
		SteamUser.OnSteamServersConnected -= OnSteamConnectedToInternet;
		SteamUser.OnSteamServersDisconnected -= OnSteamDisconnectedFromInternet;
		SteamFriends.OnGameRichPresenceJoinRequested -= OnSteamGameRichPresenceJoinRequested;
		SteamFriends.OnGameLobbyJoinRequested -= OnSteamGameLobbyJoinRequested;
		SteamMatchmaking.OnLobbyDataChanged -= OnSteamLobbyDataChanged;
		if (num)
		{
			BNetworkManager.DestroyedDueToOfflineSceneTransition?.Invoke();
		}
		bool IsDueToOfflineSceneTransition()
		{
			if (GameManager.IsApplicationQuitting)
			{
				return false;
			}
			if (!dontDestroyOnLoad)
			{
				return false;
			}
			if (string.IsNullOrWhiteSpace(offlineScene))
			{
				return false;
			}
			if (base.gameObject.scene.name == "DontDestroyOnLoad")
			{
				return false;
			}
			for (int i = 0; i < SceneManager.sceneCount; i++)
			{
				Scene sceneAt = SceneManager.GetSceneAt(i);
				if (sceneAt.path == offlineScene && !sceneAt.isLoaded)
				{
					return true;
				}
			}
			return false;
		}
	}

	public void OnBUpdate()
	{
		if (!NetworkServer.active && NetworkClient.active && NetworkClient.isConnected && BMath.GetTimeSince(NetworkTime.ClientLastPongsTime) > 10f)
		{
			ClientDisconnectWithMessage(DisconnectReason.TimedOut);
		}
		if (!pollingSteamPlayerRelationships)
		{
			PollSteamPlayerRelationships();
		}
	}

	private async void PollSteamPlayerRelationships()
	{
		pollingSteamPlayerRelationships = true;
		try
		{
			if (TryGetSteamLobby(out var lobby))
			{
				await UniTask.Yield();
				if (this == null)
				{
					return;
				}
				List<(ulong, Relationship)> value;
				using (CollectionPool<List<(ulong, Relationship)>, (ulong, Relationship)>.Get(out value))
				{
					foreach (Friend player in lobby.Members)
					{
						await UniTask.Yield();
						if (this == null)
						{
							return;
						}
						ulong num = player.Id;
						Relationship relationship = player.Relationship;
						Relationship value2;
						bool num2 = !steamPlayerRelationships.TryGetValue(num, out value2) || value2 != relationship;
						steamPlayerRelationships[num] = relationship;
						if (num2)
						{
							BNetworkManager.SteamPlayerRelationshipChanged?.Invoke(num, relationship);
							if (NetworkServer.active && PlayerInfo.IsBlockedOnSteam(relationship) && ServerTryGetConnectionFromPlayerGuid(num, out var connection))
							{
								Debug.Log($"Player {num} was blocked, kicking!");
								ServerKickConnection(connection);
							}
						}
					}
				}
			}
			await UniTask.WaitForSeconds(1);
		}
		catch (Exception exception)
		{
			Debug.LogError("Caught exception while polling steam player relationships, see next log for details");
			Debug.LogException(exception);
		}
		finally
		{
			pollingSteamPlayerRelationships = false;
		}
	}

	public static bool TryGetPlayerRelationship(ulong guid, out Relationship relationship)
	{
		if (singleton == null)
		{
			relationship = Relationship.None;
			return false;
		}
		return singleton.steamPlayerRelationships.TryGetValue(guid, out relationship);
	}

	public static async UniTask StopHostWithDisconnectMessage()
	{
		if (!NetworkServer.activeHost)
		{
			Debug.LogError("Attempted to stop host without it being active");
			return;
		}
		bool flag = false;
		foreach (NetworkConnectionToClient value in NetworkServer.connections.Values)
		{
			if (value != NetworkServer.localConnection)
			{
				value.Send(new DisconnectReasonMessage
				{
					reason = DisconnectReason.LobbyClosed
				});
				flag = true;
			}
		}
		if (flag)
		{
			await UniTask.WaitForSeconds(0.1f, ignoreTimeScale: true);
		}
		singleton.StopHost();
	}

	public override void ConfigureHeadlessFrameRate()
	{
		base.ConfigureHeadlessFrameRate();
	}

	public override void OnApplicationQuit()
	{
	}

	public void OnApplicationShuttingDown()
	{
		IsShuttingDown = true;
		if (NetworkServer.active)
		{
			IsServerShuttingDown = true;
		}
		base.OnApplicationQuit();
	}

	public override void ServerChangeScene(string newSceneName)
	{
		base.ServerChangeScene(newSceneName);
	}

	public override void OnServerChangeScene(string newSceneName)
	{
		isChangingScene = true;
		BNetworkManager.WillChangeScene?.Invoke();
	}

	public override void OnServerSceneChanged(string sceneName)
	{
		isChangingScene = false;
	}

	public override void OnClientChangeScene(string newSceneName, SceneOperation sceneOperation, bool customHandling)
	{
		if (NetworkServer.active)
		{
			return;
		}
		isChangingScene = true;
		BNetworkManager.WillChangeScene?.Invoke();
		switch (sceneOperation)
		{
		case SceneOperation.Normal:
		{
			foreach (NetworkIdentity value in NetworkClient.spawned.Values)
			{
				Inform(value);
			}
			break;
		}
		case SceneOperation.UnloadAdditive:
		{
			Scene sceneByName = SceneManager.GetSceneByName(newSceneName);
			if (!sceneByName.IsValid())
			{
				break;
			}
			{
				foreach (NetworkIdentity value2 in NetworkClient.spawned.Values)
				{
					if (value2.gameObject.scene == sceneByName)
					{
						Inform(value2);
					}
				}
				break;
			}
		}
		}
		static void Inform(NetworkIdentity networkIdentity)
		{
			if (!(networkIdentity == null))
			{
				networkIdentity.ForceOnStopClient();
				if (networkIdentity.isLocalPlayer)
				{
					networkIdentity.ForceOnStopLocalPlayer();
				}
				if (networkIdentity.isOwned)
				{
					networkIdentity.ForceOnStopAuthority();
				}
				if (networkIdentity.TryGetComponent<Entity>(out var component))
				{
					component.InformClientSceneChange();
				}
			}
		}
	}

	public override void OnClientSceneChanged()
	{
		base.OnClientSceneChanged();
		if (!NetworkServer.active)
		{
			isChangingScene = false;
		}
	}

	public override void OnServerConnect(NetworkConnectionToClient connection)
	{
		connection.Send(new MaxPlayersUpdatedMessage
		{
			maxPlayers = MaxPlayers
		});
		ServerUpdateCurrentPlayerCount();
		ulong num = ServerGetPlayerGuid(connection);
		connection.Send(new SetPlayerGuidMessage
		{
			playerGuidOnServer = num
		});
		serverConnectionIdIndices.Add(connection.connectionId, serverConnectionIds.Count);
		serverConnectionIds.Add(connection.connectionId);
		playerGuidPerConnectionId.Add(connection.connectionId, num);
		connectionIdPerPlayerGuid.Add(num, connection.connectionId);
		CourseManager.ServerRegisterPlayer(connection);
		GolfTeeManager.UpdateActivePlatforms();
	}

	private ulong ServerGetPlayerGuid(NetworkConnectionToClient connection)
	{
		if (!NetworkServer.active)
		{
			return 0uL;
		}
		if (transportInternal is FizzyFacepunch)
		{
			if (!TryGetSteamPlayerGuid(connection, out var steamId))
			{
				Debug.LogError($"No Steam player GUID could be found for connection {connection.connectionId}");
				return 0uL;
			}
			return steamId;
		}
		if (transportInternal is KcpTransport)
		{
			return (ulong)(connection.connectionId + 1);
		}
		Debug.LogError($"No player GUID could be found for connection {connection.connectionId} using transport of type {transportInternal.GetType()}");
		return 0uL;
	}

	public bool TryGetSteamPlayerGuid(NetworkConnectionToClient connection, out ulong steamId)
	{
		if (!SteamEnabler.IsSteamEnabled || !(transportInternal is FizzyFacepunch fizzyFacepunch))
		{
			steamId = 0uL;
			return false;
		}
		if (connection == NetworkServer.localConnection)
		{
			steamId = SteamClient.SteamId.Value;
			return true;
		}
		return ulong.TryParse(fizzyFacepunch.ServerGetClientAddress(connection.connectionId), out steamId);
	}

	public override void OnServerReady(NetworkConnectionToClient connection)
	{
		base.OnServerReady(connection);
	}

	public override async void OnServerAddPlayer(NetworkConnectionToClient connection)
	{
		GameObject gameObject = UnityEngine.Object.Instantiate(playerPrefab, playerInitialTempPosition, Quaternion.identity);
		gameObject.name = $"{playerPrefab.name} [connId={connection.connectionId}]";
		PlayerInfo player = gameObject.GetComponent<PlayerInfo>();
		player.PlayerId.ServerSetGuid(ServerGetPlayerGuid(connection));
		NetworkServer.AddPlayerForConnection(connection, gameObject);
		player.RpcAwaitSpawning();
		if (SingletonBehaviour<DrivingRangeManager>.HasInstance)
		{
			AddPlayerToDrivingRange();
			return;
		}
		bool flag = await AddPlayerToHole();
		if (!(this == null) && !flag)
		{
			Debug.LogError($"Failed to spawn add player for connection {connection}");
		}
		void AddPlayerToDrivingRange()
		{
			DrivingRangeSpawnArea spawnArea = DrivingRangeManager.SpawnArea;
			Vector3 randomSpawnPosition = spawnArea.GetRandomSpawnPosition();
			Quaternion rotation = spawnArea.transform.rotation;
			player.ServerInitializeAsParticipant(randomSpawnPosition, rotation, null);
		}
		async UniTask<bool> AddPlayerToHole()
		{
			bool isInitializingHole = CourseManager.MatchState == MatchState.Initializing;
			TeeingSpot teeingSpot = await GetPlayerSpawnData();
			if (this == null)
			{
				return false;
			}
			if (connection == null || !NetworkServer.connections.ContainsKey(connection.connectionId))
			{
				return false;
			}
			Vector3 position = Vector3.zero;
			Quaternion rotation = Quaternion.identity;
			if (teeingSpot != null)
			{
				position = teeingSpot.playerWorldPosition;
				rotation = teeingSpot.playerWorldRotation;
			}
			if (teeingSpot == null)
			{
				player.ServerInitializeAsSpectator(isInitializingHole);
				Debug.Log($"Initialized {player} as spectator");
			}
			else
			{
				player.ServerInitializeAsParticipant(position, rotation, teeingSpot);
				Debug.Log($"Initialized {player} as participant");
			}
			return true;
			async UniTask<TeeingSpot> GetPlayerSpawnData()
			{
				if (!SingletonBehaviour<DrivingRangeManager>.HasInstance && CourseManager.IsPlayerSpectator(player.AsGolfer))
				{
					return null;
				}
				if (CourseManager.MatchState > MatchState.Initializing && CourseManager.MatchState <= MatchState.Ongoing)
				{
					return GolfTeeManager.GetAvailableTeeingSpot();
				}
				if (CourseManager.MatchState == MatchState.Initializing)
				{
					CourseManager.ReportPlayerAwaitingSpawning(connection.connectionId);
					double awaitingTeeingSpotTimestamp = Time.timeAsDouble;
					while (!CourseManager.DoesPlayerHaveReservedTeeingSpot(connection.connectionId, out teeingSpot))
					{
						await UniTask.Yield();
						if (this == null)
						{
							return null;
						}
						if (BMath.GetTimeSince(awaitingTeeingSpotTimestamp) > 20f)
						{
							CourseManager.ReportPlayerNoLongerAwaitingSpawning(connection.connectionId);
							return null;
						}
						if (connection == null || !NetworkServer.connections.ContainsKey(connection.connectionId))
						{
							CourseManager.ReportPlayerNoLongerAwaitingSpawning(connection.connectionId);
							return null;
						}
					}
					CourseManager.ReportPlayerNoLongerAwaitingSpawning(connection.connectionId);
					return teeingSpot;
				}
				return null;
			}
		}
	}

	public override void OnServerDisconnect(NetworkConnectionToClient connection)
	{
		base.OnServerDisconnect(connection);
		if (connection != NetworkServer.localConnection)
		{
			ServerUpdateCurrentPlayerCount();
			if (serverConnectionIdIndices.TryGetValue(connection.connectionId, out var value))
			{
				Dictionary<int, int> dictionary = serverConnectionIdIndices;
				List<int> list = serverConnectionIds;
				dictionary[list[list.Count - 1]] = value;
				serverConnectionIdIndices.Remove(connection.connectionId);
				serverConnectionIds.RemoveAtSwapBack(value);
			}
			CourseManager.DeregisterPlayer(connection);
			if (playerGuidPerConnectionId.TryGetValue(connection.connectionId, out var value2))
			{
				playerGuidPerConnectionId.Remove(connection.connectionId);
				connectionIdPerPlayerGuid.Remove(value2);
				NetworkServer.SendToAll(new RemotePlayerDisconnectMessage
				{
					playerGuidOnServer = value2
				});
			}
			GolfTeeManager.UpdateActivePlatforms();
		}
	}

	public override void OnServerError(NetworkConnectionToClient connection, TransportError transportError, string message)
	{
	}

	public override void OnServerTransportException(NetworkConnectionToClient connection, Exception exception)
	{
	}

	public override void OnClientConnect()
	{
		base.OnClientConnect();
		UpdateConnectRichPresence();
	}

	public override void OnClientDisconnect()
	{
	}

	public override void OnClientNotReady()
	{
	}

	public override void OnClientError(TransportError transportError, string message)
	{
	}

	public override void OnClientTransportException(Exception exception)
	{
	}

	public override void OnStartHost()
	{
	}

	public override void OnStartServer()
	{
		IsServerShuttingDown = false;
		IsShuttingDown = false;
		NetworkServer.RegisterHandler<StartMatchMessage>(OnServerStartMatchMessage);
		NetworkServer.RegisterHandler<TriggerNetworkClientToServerExceptionMessage>(OnServerTriggerNetworkClientToServerExceptionMessage);
		NetworkServer.RegisterHandler<RequestNetworkServerToClientExceptionMessage>(OnServerRequestNetworkServerToClientExceptionMessage);
		NetworkServer.RegisterHandler<ClientFriendCheckConfirmationMessage>(OnServerClientConfirmFriend);
		if (transportInternal is KcpTransport)
		{
			OpenKcpLobby();
		}
		else if (transportInternal is FizzyFacepunch)
		{
			OpenSteamLobby();
		}
	}

	public override void OnStartClient()
	{
		IsServerShuttingDown = false;
		IsShuttingDown = false;
		NetworkClient.RegisterFallbackUnspawnHandler(NetworkDestroy);
		NetworkClient.RegisterHandler<MaxPlayersUpdatedMessage>(OnClientMaxPlayersUpdatedMessage);
		NetworkClient.RegisterHandler<SetPlayerGuidMessage>(OnClientSetPlayerGuidMessage);
		NetworkClient.RegisterHandler<SetStandardCourseMessage>(OnClientSetStandardCourseMessage, requireAuthentication: false);
		NetworkClient.RegisterHandler<SetNonStandardCourseMessage>(OnClientSetNonStandardCourseMessage, requireAuthentication: false);
		NetworkClient.RegisterHandler<DisconnectReasonMessage>(OnClientDisconnectReasonMessage);
		NetworkClient.RegisterHandler<RemotePlayerDisconnectMessage>(OnClientRemotePlayerDisconnectMessage);
		NetworkClient.RegisterHandler<TriggerNetworkServerToClientExceptionMessage>(OnClientTriggerNetworkServerToClientExceptionMessage);
		NetworkClient.RegisterHandler<ServerRequestFriendCheckMessage>(OnClientServerFriendCheckRequest);
	}

	public override void OnStopHost()
	{
		IsShuttingDown = true;
		IsServerShuttingDown = true;
	}

	public override void OnStopServer()
	{
		IsShuttingDown = true;
		IsServerShuttingDown = true;
		isAcceptingConnectionsInternal = false;
		if (transportInternal is FizzyFacepunch)
		{
			CloseLobby(DisconnectReason.LobbyClosed);
		}
	}

	public override void OnStopClient()
	{
		MatchSetupRules.CheatsWarningShowed = false;
		IsShuttingDown = true;
		if (transportInternal is FizzyFacepunch)
		{
			SteamFriends.ClearRichPresence();
			LeaveSteamLobby();
		}
		if (!SingletonBehaviour<MainMenu>.HasInstance)
		{
			LoadingScreen.Show();
		}
		if (base.mode == NetworkManagerMode.ServerOnly)
		{
			StopServer();
		}
	}

	public static void ConnectToSteamFriend(Friend friend)
	{
		if (TryGetSteamLobbyId(friend, out var id))
		{
			ConnectToSteamLobby(id.ToString(), canExitCurrentLobby: true);
		}
	}

	public static void ConnectToSteamLobby(string lobbyId, bool canExitCurrentLobby)
	{
		bool num = NetworkServer.active || NetworkClient.active;
		Debug.Log("Attempting to join Steam lobby " + lobbyId);
		if (num)
		{
			if (!canExitCurrentLobby)
			{
				if (NetworkServer.active)
				{
					Debug.LogError("Cannot connect to a Steam lobby while a server is already active; shut it down first");
					return;
				}
				if (NetworkClient.active)
				{
					Debug.LogError("Cannot connect to a Steam lobby while a client is already active; disconnect from the server first");
					return;
				}
			}
			SteamLobbyIdToConnectToFromMainMenu = lobbyId;
			if (NetworkServer.activeHost)
			{
				singleton.StopHost();
			}
			else if (NetworkServer.active)
			{
				singleton.StopServer();
			}
			else if (NetworkClient.active)
			{
				singleton.StopClient();
			}
		}
		else
		{
			SteamLobbyIdToConnectToFromMainMenu = null;
			singleton.SetTransport(TransportType.Steam);
			ConnectToServer(lobbyId);
		}
	}

	public static void ConnectKcp(string address)
	{
		if (NetworkServer.active)
		{
			Debug.LogError("Cannot connect to a server while a server is already active; shut it down first");
			return;
		}
		if (NetworkClient.active)
		{
			Debug.LogError("Cannot connect to a server while a client is already active; disconnect from the server first");
			return;
		}
		singleton.SetTransport(TransportType.Kcp);
		ConnectToServer(address);
	}

	private static void NetworkDestroy(GameObject gameObject)
	{
		if (!gameObject.TryGetComponent<Entity>(out var component))
		{
			UnityEngine.Object.Destroy(gameObject);
		}
		else
		{
			component.DestroyEntity();
		}
	}

	private void SetTransport(TransportType transportType)
	{
		if (NetworkServer.active || NetworkClient.active)
		{
			throw new Exception("Can't change network transport layer while server/client is active!");
		}
		transportInternal = transportType switch
		{
			TransportType.Kcp => GetComponent<KcpTransport>(), 
			TransportType.Steam => GetComponent<FizzyFacepunch>(), 
			_ => null, 
		};
		if (isSimulatingLatency)
		{
			GetComponent<LatencySimulation>().wrap = transportInternal;
		}
		else
		{
			transport.enabled = false;
			transport = transportInternal;
			Transport.active = transport;
		}
		if (transportType == TransportType.Steam)
		{
			(transportInternal as FizzyFacepunch).Initialize();
		}
		transportInternal.enabled = true;
		Debug.Log($"Set transport to {transportType}");
	}

	private void SimulateLatencyInternal(bool simulate)
	{
		if (isSimulatingLatency != simulate)
		{
			if (simulate)
			{
				base.gameObject.SetActive(value: false);
				LatencySimulation latencySimulation = base.gameObject.AddComponent<LatencySimulation>();
				latencySimulation.wrap = transportInternal;
				base.gameObject.SetActive(value: true);
				Transport.active = latencySimulation;
			}
			else
			{
				UnityEngine.Object.Destroy(GetComponent<LatencySimulation>());
				transport = transportInternal;
				transport.enabled = true;
				Transport.active = transport;
			}
			isSimulatingLatency = simulate;
		}
	}

	private void OpenKcpLobby()
	{
		if (NetworkServer.active)
		{
			isAcceptingConnectionsInternal = true;
			Debug.Log("Server is now accepting connections");
		}
	}

	private async UniTask OpenSteamLobby()
	{
		if (steamLobby.HasValue)
		{
			Debug.LogError("Attempted to create a steam lobby while already in one");
		}
		else
		{
			if (!NetworkServer.active)
			{
				return;
			}
			try
			{
				await CreateLobby();
			}
			catch (Exception exception)
			{
				Debug.LogError("Encountered an exception while opening a Steam lobby. See next log for details");
				Debug.LogException(exception);
				return;
			}
			if (!steamLobby.HasValue)
			{
				Debug.LogError("Failed to create Steam lobby in order to accept connections");
				isAcceptingConnectionsInternal = false;
				SteamFriends.ClearRichPresence();
				return;
			}
			ServerUpdateCurrentPlayerCount();
			ServerUpdatePasswordRequired();
			ServerUpdateCourseProgress();
			steamLobby.Value.SetData("lobbyName", LobbyName);
			steamLobby.Value.SetData("maxPlayers", MaxPlayers.ToString());
			if (setLobbyPingLocationCancellationTokenSource != null)
			{
				setLobbyPingLocationCancellationTokenSource.Cancel();
				setLobbyPingLocationCancellationTokenSource = null;
			}
			setLobbyPingLocationCancellationTokenSource = new CancellationTokenSource();
			SetLobbyPingLocation(setLobbyPingLocationCancellationTokenSource.Token);
			isAcceptingConnectionsInternal = true;
			Debug.Log("Server is now accepting connections");
			UpdateConnectRichPresence();
			SetSteamRichPresence("sbg_isHosting", "true");
		}
		static async UniTask<Lobby?> CreateLobby()
		{
			Task<Lobby?> lobbyCreationTask = SteamMatchmaking.CreateLobbyAsync();
			await lobbyCreationTask;
			steamLobby = lobbyCreationTask.Result;
			if (!steamLobby.HasValue)
			{
				Debug.LogError("Failed to create Steam lobby; setting to singleplayer");
				singleton.CloseLobby(DisconnectReason.KickedFromLobby);
			}
			else
			{
				steamLobby.Value.SetJoinable(b: true);
				SetLobbyMode(lobbyMode);
				BNetworkManager.LobbyOpened?.Invoke();
				BNetworkManager.LobbyStateChanged?.Invoke();
				Debug.Log($"Created Steam lobby {steamLobby.Value.Id}");
			}
			return steamLobby;
		}
		async void SetLobbyPingLocation(CancellationToken cancellationToken)
		{
			do
			{
				NetPingLocation? localPingLocation = SteamNetworkingUtils.LocalPingLocation;
				if (localPingLocation.HasValue)
				{
					string value = localPingLocation.ToString();
					if (!string.IsNullOrEmpty(value))
					{
						steamLobby.Value.SetData("hostLocation", value);
						break;
					}
				}
				await UniTask.WaitForSeconds(3f);
			}
			while (!(this == null) && !cancellationToken.IsCancellationRequested);
		}
	}

	private void CloseLobby(DisconnectReason clientDisconnectReason, bool fromLeaveLobby = false)
	{
		if (clientDisconnectReason != DisconnectReason.None)
		{
			foreach (NetworkConnectionToClient item in new List<NetworkConnectionToClient>(NetworkServer.connections.Values))
			{
				if (item != NetworkServer.localConnection)
				{
					ServerDisconnectClientWithMessage(item, clientDisconnectReason);
				}
			}
		}
		if (transportInternal is FizzyFacepunch)
		{
			SteamFriends.ClearRichPresence();
			if (!fromLeaveLobby)
			{
				LeaveSteamLobby(fromCloseLobby: true);
			}
		}
		Debug.Log("Server is no longer accepting connections");
		if (!IsShuttingDown)
		{
			isAcceptingConnectionsInternal = false;
			if (setLobbyPingLocationCancellationTokenSource != null)
			{
				setLobbyPingLocationCancellationTokenSource.Cancel();
				setLobbyPingLocationCancellationTokenSource = null;
			}
			BNetworkManager.LobbyClosed?.Invoke();
			BNetworkManager.LobbyStateChanged?.Invoke();
		}
	}

	public static void SetLobbyMode(LobbyMode newLobbyMode)
	{
		lobbyMode = newLobbyMode;
		if (steamLobby.HasValue)
		{
			switch (newLobbyMode)
			{
			case LobbyMode.Public:
				steamLobby.Value.SetPublic();
				steamLobby.Value.SetData("sbg_lobbyMode", "public");
				break;
			case LobbyMode.Friends:
				steamLobby.Value.SetFriendsOnly();
				steamLobby.Value.SetData("sbg_lobbyMode", "friends");
				break;
			case LobbyMode.InviteOnly:
				steamLobby.Value.SetPrivate();
				steamLobby.Value.SetData("sbg_lobbyMode", "private");
				break;
			}
		}
	}

	public static bool TryGetSteamLobby(out Lobby lobby)
	{
		lobby = default(Lobby);
		if (!steamLobby.HasValue)
		{
			return false;
		}
		if (!steamLobby.Value.Id.IsValid)
		{
			return false;
		}
		lobby = steamLobby.Value;
		return true;
	}

	public static bool TryGetPlayerInLobby(ulong steamId, out Friend player)
	{
		player = default(Friend);
		if (!TryGetSteamLobby(out var lobby))
		{
			return false;
		}
		foreach (Friend member in lobby.Members)
		{
			if ((ulong)member.Id == steamId)
			{
				player = member;
				return true;
			}
		}
		return false;
	}

	private async UniTask<RoomEnter> JoinSteamLobby(Lobby lobby)
	{
		steamLobby = lobby;
		Task<RoomEnter> joinTask = lobby.Join();
		await joinTask;
		if (joinTask.Result == RoomEnter.Success)
		{
			Debug.Log($"Joined Steam lobby {lobby.Id}");
		}
		else
		{
			Debug.LogError($"Failed to join Steam lobby {lobby.Id} with reason {joinTask.Result}");
		}
		UpdateConnectRichPresence();
		BNetworkManager.LobbyJoined?.Invoke();
		BNetworkManager.LobbyStateChanged?.Invoke();
		return joinTask.Result;
	}

	private void LeaveSteamLobby(bool fromCloseLobby = false)
	{
		if (!steamLobby.HasValue)
		{
			return;
		}
		if (steamLobby.Value.IsOwnedBy(SteamClient.SteamId))
		{
			if (!fromCloseLobby)
			{
				CloseLobby(DisconnectReason.LobbyClosed, fromLeaveLobby: true);
			}
			steamLobby.Value.SetInvisible();
		}
		Debug.Log($"Left Steam lobby {steamLobby.Value.Id}");
		steamLobby.Value.Leave();
		steamLobby = null;
		UpdateConnectRichPresence();
		BNetworkManager.LobbyLeft?.Invoke();
		BNetworkManager.LobbyStateChanged?.Invoke();
	}

	private void SetSteamRichPresence(string key, string value)
	{
		if (!SteamFriends.SetRichPresence(key, value))
		{
			Debug.LogError("Failed to set Steam rich presence key " + key + " with value " + value);
		}
	}

	private void ServerUpdateCurrentPlayerCount()
	{
		if (NetworkServer.active && steamLobby.HasValue && steamLobby.Value.Id.IsValid)
		{
			steamLobby.Value.SetData("currentPlayerCount", NetworkServer.connections.Count.ToString());
		}
	}

	private void UpdateConnectRichPresence()
	{
		if (transport is FizzyFacepunch && TryGetSteamLobby(out var lobby))
		{
			if (!steamLobby.HasValue || lobby.GetData("sbg_lobbyMode") == "private")
			{
				SetSteamRichPresence("connect", null);
			}
			else
			{
				SetSteamRichPresence("connect", string.Format("{0} {1}", "+connectSteamLobby", steamLobby.Value.Id));
			}
		}
	}

	public static bool TryGetSteamLobbyId(Friend friend, out SteamId id)
	{
		id = default(SteamId);
		Friend.FriendGameInfo? gameInfo = friend.GameInfo;
		if (!gameInfo.HasValue)
		{
			return false;
		}
		Lobby? lobby = gameInfo.Value.Lobby;
		if (!lobby.HasValue)
		{
			return false;
		}
		id = lobby.Value.Id;
		return true;
	}

	public void ServerSetMaxPlayers(int newMaxPlayers)
	{
		MaxPlayers = newMaxPlayers;
		NetworkServer.SendToAll(new MaxPlayersUpdatedMessage
		{
			maxPlayers = newMaxPlayers
		});
		if (CanSetLobbyValue())
		{
			steamLobby.Value.SetData("maxPlayers", newMaxPlayers.ToString());
		}
	}

	public void ServerUpdatePasswordRequired()
	{
		if (CanSetLobbyValue())
		{
			steamLobby.Value.SetData("sbg_passwordRequired", string.IsNullOrEmpty(BClientAuthenticator.serverPassword) ? "false" : "true");
		}
	}

	public void ServerUpdateCourseProgress()
	{
		if (CanSetLobbyValue())
		{
			string text;
			if (SingletonBehaviour<DrivingRangeManager>.HasInstance)
			{
				text = "DrivingRange";
			}
			else
			{
				Locale locale = LocalizationSettings.AvailableLocales.GetLocale("keys");
				LocalizedString currentCourseLocalizedName = CourseManager.GetCurrentCourseLocalizedName();
				currentCourseLocalizedName = new LocalizedString(currentCourseLocalizedName.TableReference, currentCourseLocalizedName.TableEntryReference);
				currentCourseLocalizedName.LocaleOverride = locale;
				text = currentCourseLocalizedName.GetLocalizedString();
			}
			steamLobby.Value.SetData("currentCourse", text);
			if (SingletonBehaviour<DrivingRangeManager>.HasInstance)
			{
				steamLobby.Value.DeleteData("currentCourseProgress");
				steamLobby.Value.DeleteData("currentCourseLength");
			}
			else
			{
				steamLobby.Value.SetData("currentCourseProgress", (CourseManager.CurrentHoleCourseIndex + 1).ToString());
				steamLobby.Value.SetData("currentCourseLength", GameManager.CurrentCourse.Holes.Length.ToString());
			}
			Debug.Log("UPDATE COURSE " + text);
		}
	}

	private bool CanSetLobbyValue()
	{
		if (!steamLobby.HasValue)
		{
			return false;
		}
		return true;
	}

	private async void ServerDisconnectClientWithMessage(NetworkConnectionToClient client, DisconnectReason reason)
	{
		client.Send(new DisconnectReasonMessage
		{
			reason = reason
		});
		await UniTask.WaitForSeconds(0.1f, ignoreTimeScale: true);
		client?.Disconnect();
	}

	public void ServerKickConnection(NetworkConnectionToClient connection)
	{
		if (authenticator is BClientAuthenticator bClientAuthenticator && ServerTryGetPlayerGuidFromConnection(connection, out var playerGuid))
		{
			bClientAuthenticator.playersKickedThisSession.Add(playerGuid);
		}
		ServerDisconnectClientWithMessage(connection, DisconnectReason.KickedFromLobby);
	}

	private void ClientDisconnectWithMessage(DisconnectReason reason)
	{
		DisplayDisconnectReasonMessage(reason);
		StopClient();
	}

	private static async void ConnectToServer(string address)
	{
		if (NetworkServer.active)
		{
			Debug.LogError("Attempted to connect to a server at address " + address + " while a server is already active");
			return;
		}
		if (NetworkClient.active)
		{
			Debug.LogError("Attempted to connect to a server at address " + address + " while a client is already active");
			return;
		}
		bool connectionCanceledByPlayer = false;
		FullScreenMessage.Show(Localization.UI.AUTHENTICATION_Connecting, new FullScreenMessage.ButtonEntry(Localization.UI.MISC_Cancel, CancelConnection));
		if (singleton.transportInternal is FizzyFacepunch)
		{
			if (!ulong.TryParse(address, out var result))
			{
				FullScreenMessage.Hide();
				FullScreenMessage.ShowErrorMessage(Localization.UI.ERROR_InvalidLobbyId, "", Localization.UI.ERROR_ConnectionFailed, 0);
				return;
			}
			RoomEnter roomEnter = await singleton.JoinSteamLobby(new Lobby(result));
			if (connectionCanceledByPlayer)
			{
				singleton.LeaveSteamLobby();
				return;
			}
			if (roomEnter != RoomEnter.Success)
			{
				singleton.LeaveSteamLobby();
				FullScreenMessage.Hide();
				FullScreenMessage.ShowErrorMessage(Localization.UI.ERROR_JoinLobbyFailed, "", Localization.UI.ERROR_ConnectionFailed, 0);
				return;
			}
			address = steamLobby.Value.Owner.Id.ToString();
		}
		FullScreenMessage.Hide();
		if (connectionCanceledByPlayer)
		{
			return;
		}
		try
		{
			InputManager.EnableMode(InputMode.ForceDisabled);
			LoadingScreen.Show(Time.timeScale <= 0.25f);
			await UniTask.WaitWhile(() => LoadingScreen.IsFadingScreenIn);
		}
		catch (Exception exception)
		{
			Debug.LogError("Encountered exception while fading to host start loading screen. See the next log for details");
			Debug.LogException(exception);
		}
		finally
		{
			InputManager.DisableMode(InputMode.ForceDisabled);
		}
		singleton.networkAddress = address;
		singleton.StartClient();
		SceneManager.sceneLoaded += OnSceneLoaded;
		Transport obj = singleton.transport;
		obj.OnClientDisconnected = (Action)Delegate.Combine(obj.OnClientDisconnected, new Action(OnClientDisconnectDuringConnection));
		void CancelConnection()
		{
			connectionCanceledByPlayer = true;
			RemoveTransportCallbacksAndHidePrompt();
			singleton.StopClient();
		}
		static void OnClientDisconnectDuringConnection()
		{
			InputManager.DisableMode(InputMode.ForceDisabled);
			LoadingScreen.Hide();
			RemoveTransportCallbacksAndHidePrompt();
			FullScreenMessage.ShowErrorMessage(Localization.UI.ERROR_ConnectionFailed);
		}
		static void OnSceneLoaded(Scene scene, LoadSceneMode mode)
		{
			RemoveTransportCallbacksAndHidePrompt();
			SceneManager.sceneLoaded -= OnSceneLoaded;
		}
		static void RemoveTransportCallbacksAndHidePrompt()
		{
			FullScreenMessage.Hide();
			Transport obj2 = singleton.transport;
			obj2.OnClientDisconnected = (Action)Delegate.Remove(obj2.OnClientDisconnected, new Action(OnClientDisconnectDuringConnection));
		}
	}

	public void InformClientAuthenticationRejected()
	{
		if (transportInternal is FizzyFacepunch)
		{
			LeaveSteamLobby();
		}
	}

	[Server]
	public bool ServerTryGetPlayerGuidFromConnection(NetworkConnectionToClient connection, out ulong playerGuid)
	{
		if (!NetworkServer.active)
		{
			Debug.LogWarning("[Server] function 'System.Boolean BNetworkManager::ServerTryGetPlayerGuidFromConnection(Mirror.NetworkConnectionToClient,System.UInt64&)' called when server was not active");
			playerGuid = default(ulong);
			return default(bool);
		}
		return playerGuidPerConnectionId.TryGetValue(connection.connectionId, out playerGuid);
	}

	[Server]
	public bool ServerTryGetConnectionFromPlayerGuid(ulong playerGuid, out NetworkConnectionToClient connection)
	{
		if (!NetworkServer.active)
		{
			Debug.LogWarning("[Server] function 'System.Boolean BNetworkManager::ServerTryGetConnectionFromPlayerGuid(System.UInt64,Mirror.NetworkConnectionToClient&)' called when server was not active");
			connection = null;
			return default(bool);
		}
		if (!connectionIdPerPlayerGuid.TryGetValue(playerGuid, out var value))
		{
			connection = null;
			return false;
		}
		return NetworkServer.connections.TryGetValue(value, out connection);
	}

	private void DisplayDisconnectReasonMessage(DisconnectReason reason)
	{
		Debug.Log($"Client is about to be disconnected due to reason: {reason}");
		string message = LocalizationManager.GetString(StringTable.UI, $"DISCONNECT_{reason}");
		FullScreenMessage.Hide();
		FullScreenMessage.Show(message, new FullScreenMessage.ButtonEntry(Localization.UI.MISC_Ok, FullScreenMessage.Hide));
	}

	private void OnServerStartMatchMessage(NetworkConnectionToClient sender, StartMatchMessage message)
	{
		CourseManager.StartCourse();
	}

	private void OnServerTriggerNetworkClientToServerExceptionMessage(NetworkConnectionToClient sender, TriggerNetworkClientToServerExceptionMessage message)
	{
		throw new Exception("CLIENT -> SERVER NETWORK ERROR TEST");
	}

	private void OnServerRequestNetworkServerToClientExceptionMessage(NetworkConnectionToClient sender, RequestNetworkServerToClientExceptionMessage message)
	{
		sender.Send(default(TriggerNetworkServerToClientExceptionMessage));
	}

	public static void ServerRequestFriendConfirmation(ulong guid)
	{
		NetworkServer.SendToReady(new ServerRequestFriendCheckMessage
		{
			playerGuidOnServer = guid
		});
	}

	private void OnServerClientConfirmFriend(NetworkConnectionToClient sender, ClientFriendCheckConfirmationMessage message)
	{
		if (sender.isAuthenticated && sender.isReady)
		{
			BNetworkManager.OnServerFriendshipConfirmed?.Invoke(message.friendPlayerGuid);
		}
	}

	private void OnClientMaxPlayersUpdatedMessage(MaxPlayersUpdatedMessage message)
	{
		MaxPlayers = message.maxPlayers;
	}

	private void OnClientSetPlayerGuidMessage(SetPlayerGuidMessage message)
	{
		LocalPlayerGuidOnServer = message.playerGuidOnServer;
	}

	private void OnClientSetStandardCourseMessage(SetStandardCourseMessage message)
	{
		GameManager.ClientSetStandardCourse(message.courseIndex);
	}

	private void OnClientSetNonStandardCourseMessage(SetNonStandardCourseMessage message)
	{
		GameManager.ClientSetNonStandardCourse(message.globalHoleIndices, message.isRandom);
	}

	private void OnClientDisconnectReasonMessage(DisconnectReasonMessage message)
	{
		DisplayDisconnectReasonMessage(message.reason);
	}

	private void OnClientRemotePlayerDisconnectMessage(RemotePlayerDisconnectMessage message)
	{
		showedConnectionMessage.Remove(message.playerGuidOnServer);
	}

	private void OnClientTriggerNetworkServerToClientExceptionMessage(TriggerNetworkServerToClientExceptionMessage message)
	{
		throw new Exception("SERVER -> CLIENT NETWORK EXCEPTION TEST");
	}

	private void OnClientServerFriendCheckRequest(ServerRequestFriendCheckMessage message)
	{
		if (!SteamEnabler.IsSteamEnabled || NetworkServer.active)
		{
			return;
		}
		foreach (Friend friend in SteamFriends.GetFriends())
		{
			if ((ulong)friend.Id == message.playerGuidOnServer)
			{
				NetworkClient.Send(new ClientFriendCheckConfirmationMessage
				{
					friendPlayerGuid = friend.Id
				});
				break;
			}
		}
	}

	private void OnSteamConnectedToInternet()
	{
		BNetworkManager.ConnectedToInternet?.Invoke();
	}

	private void OnSteamDisconnectedFromInternet()
	{
		BNetworkManager.DisconnectedFromInternet?.Invoke();
	}

	private void OnSteamGameRichPresenceJoinRequested(Friend friend, string arguments)
	{
		string[] array = arguments.Split(' ');
		if (array.Length < 2)
		{
			Debug.LogError("Too few rich presence arguments");
		}
		else if (array[0] != "+connectSteamLobby")
		{
			Debug.LogWarning("No valid connection argument in rich presence");
		}
		else
		{
			ConnectToSteamLobby(array[1], canExitCurrentLobby: true);
		}
	}

	private void OnSteamGameLobbyJoinRequested(Lobby lobby, SteamId friend)
	{
		ConnectToSteamLobby(lobby.Id.ToString(), canExitCurrentLobby: true);
	}

	private void OnSteamLobbyDataChanged(Lobby lobby)
	{
		UpdateConnectRichPresence();
	}

	private void OnPlayerSuspiciousActivityDetected(int playerConnectionId)
	{
	}

	private void OnPlayerConfirmedCheatingDetected(int playerConnectionId)
	{
		if (!NetworkServer.active)
		{
			Debug.LogError("Player was confirmed cheating, but there is no server running");
			return;
		}
		if (!NetworkServer.connections.TryGetValue(playerConnectionId, out var value))
		{
			Debug.LogError($"Player with connection ID {playerConnectionId} was confirmed cheating, but their connection could not be found");
			return;
		}
		if (value == NetworkServer.localConnection)
		{
			Debug.LogError($"Player with connection ID {playerConnectionId} was confirmed cheating, but they are the host");
			return;
		}
		ulong value2;
		bool flag = playerGuidPerConnectionId.TryGetValue(playerConnectionId, out value2);
		Debug.Log(string.Format("Player with connection ID {0} (GUID {1}) was confirmed cheating; kicking them", playerConnectionId, flag ? ((object)value2) : "not found"));
		ServerKickConnection(value);
	}

	public bool TryShowConnectedMessage(PlayerId player)
	{
		if (player.Guid == 0L || player.PlayerName == null || player.name.Length == 0)
		{
			return false;
		}
		int num;
		if (!player.isLocalPlayer && GameManager.LocalPlayerInfo != null)
		{
			num = ((!showedConnectionMessage.Contains(player.Guid)) ? 1 : 0);
			if (num != 0)
			{
				TextChatUi.ShowMessage(string.Format(Localization.UI.TEXTCHAT_Info_PlayerConnected, GameManager.UiSettings.ApplyColorTag(GameManager.RichTextNoParse(player.name), TextHighlight.Regular)));
			}
		}
		else
		{
			num = 0;
		}
		showedConnectionMessage.Add(player.Guid);
		return (byte)num != 0;
	}
}
