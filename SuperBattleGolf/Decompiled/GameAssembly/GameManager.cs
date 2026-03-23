using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using Cysharp.Threading.Tasks;
using Mirror;
using Steamworks;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.Diagnostics;
using UnityEngine.Pool;
using UnityEngine.ResourceManagement.AsyncOperations;
using UnityEngine.SceneManagement;

public class GameManager : SingletonBehaviour<GameManager>
{
	private const string gameManagerPrefabPath = "Assets/Prefabs/Managers/Game manager.prefab";

	[SerializeField]
	private LayerSettings layerSettings;

	[SerializeField]
	private GolfSettings golfSettings;

	[SerializeField]
	private GolfBallSettings golfBallSettings;

	[SerializeField]
	private ItemSettings itemSettings;

	[SerializeField]
	private MatchSettings matchSettings;

	[SerializeField]
	private PlayerInventorySettings playerInventorySettings;

	[SerializeField]
	private GolfCartSettings golfCartSettings;

	[SerializeField]
	private CheckpointSettings checkpointSettings;

	[SerializeField]
	private CameraGameplaySettings cameraGameplaySettings;

	[SerializeField]
	private UiSettings uiSettings;

	[SerializeField]
	private ItemCollection allItems;

	[SerializeField]
	private AllEmoteSettings emoteSettings;

	[SerializeField]
	private AchievementCollection achievements;

	[SerializeField]
	private AudioSettings audioSettings;

	[SerializeField]
	private HoleData drivingRangeHoleData;

	[SerializeField]
	private CourseCollection allCourses;

	[SerializeField]
	private GameObject networkManagerPrefab;

	[SerializeField]
	private GameObject steamManagerPrefab;

	[SerializeField]
	private GameObject persistentUiCanvasPrefab;

	[SerializeField]
	private GameObject gameplayCameraManagerPrefab;

	[SerializeField]
	private GameObject vfxPersistentDataPrefab;

	[SerializeField]
	private LocalSpectatorCameraFollower spectatorCameraPrefab;

	[SerializeField]
	private TextAsset[] badWordsTextAssets;

	private Camera camera;

	private LevelBoundsTracker cameraLevelBoundsTracker;

	private PlayerInfo localPlayerInfo;

	private PlayerId localPlayerId;

	private PlayerMovement localPlayerMovement;

	private PlayerInventory localPlayerInventory;

	private PlayerGolfer localPlayerAsGolfer;

	private PlayerInteractableTargeter localPlayerInteractableTargeter;

	private PlayerSpectator localPlayerAsSpectator;

	private readonly List<PlayerInfo> remotePlayers = new List<PlayerInfo>();

	private readonly Dictionary<int, PlayerInfo> remotePlayerPerconnectionId = new Dictionary<int, PlayerInfo>();

	private readonly Dictionary<PlayerInfo, int> remotePlayerIndices = new Dictionary<PlayerInfo, int>();

	private static bool isExitingToMainMenu;

	private static CourseData currentCourse;

	private static Regex badWordsRegex;

	[CVar("timeScale", "", "", false, true, callback = "TimeScaleSet")]
	private static float timeScaleCvar = 1f;

	private static readonly Regex richTextNoParseRegex = new Regex("<[^>]*\\b(noparse)\\b[^>]*>", RegexOptions.IgnoreCase | RegexOptions.Compiled);

	public static AchievementsManager AchievementsManager { get; private set; }

	public static bool IsSteamOverlayActive { get; private set; }

	public static UiHidingGroup HiddenUiGroups { get; private set; } = UiHidingGroup.None;

	public static bool IsApplicationQuitting { get; private set; }

	public static LayerSettings LayerSettings
	{
		get
		{
			if (!SingletonBehaviour<GameManager>.HasInstance)
			{
				return null;
			}
			return SingletonBehaviour<GameManager>.Instance.layerSettings;
		}
	}

	public static GolfSettings GolfSettings
	{
		get
		{
			if (!SingletonBehaviour<GameManager>.HasInstance)
			{
				return null;
			}
			return SingletonBehaviour<GameManager>.Instance.golfSettings;
		}
	}

	public static GolfBallSettings GolfBallSettings
	{
		get
		{
			if (!SingletonBehaviour<GameManager>.HasInstance)
			{
				return null;
			}
			return SingletonBehaviour<GameManager>.Instance.golfBallSettings;
		}
	}

	public static ItemSettings ItemSettings
	{
		get
		{
			if (!SingletonBehaviour<GameManager>.HasInstance)
			{
				return null;
			}
			return SingletonBehaviour<GameManager>.Instance.itemSettings;
		}
	}

	public static MatchSettings MatchSettings
	{
		get
		{
			if (!SingletonBehaviour<GameManager>.HasInstance)
			{
				return null;
			}
			return SingletonBehaviour<GameManager>.Instance.matchSettings;
		}
	}

	public static PlayerInventorySettings PlayerInventorySettings
	{
		get
		{
			if (!SingletonBehaviour<GameManager>.HasInstance)
			{
				return null;
			}
			return SingletonBehaviour<GameManager>.Instance.playerInventorySettings;
		}
	}

	public static GolfCartSettings GolfCartSettings
	{
		get
		{
			if (!SingletonBehaviour<GameManager>.HasInstance)
			{
				return null;
			}
			return SingletonBehaviour<GameManager>.Instance.golfCartSettings;
		}
	}

	public static CheckpointSettings CheckpointSettings
	{
		get
		{
			if (!SingletonBehaviour<GameManager>.HasInstance)
			{
				return null;
			}
			return SingletonBehaviour<GameManager>.Instance.checkpointSettings;
		}
	}

	public static CameraGameplaySettings CameraGameplaySettings
	{
		get
		{
			if (!SingletonBehaviour<GameManager>.HasInstance)
			{
				return null;
			}
			return SingletonBehaviour<GameManager>.Instance.cameraGameplaySettings;
		}
	}

	public static UiSettings UiSettings
	{
		get
		{
			if (!SingletonBehaviour<GameManager>.HasInstance)
			{
				return null;
			}
			return SingletonBehaviour<GameManager>.Instance.uiSettings;
		}
	}

	public static ItemCollection AllItems
	{
		get
		{
			if (!SingletonBehaviour<GameManager>.HasInstance)
			{
				return null;
			}
			return SingletonBehaviour<GameManager>.Instance.allItems;
		}
	}

	public static AllEmoteSettings EmoteSettings
	{
		get
		{
			if (!SingletonBehaviour<GameManager>.HasInstance)
			{
				return null;
			}
			return SingletonBehaviour<GameManager>.Instance.emoteSettings;
		}
	}

	public static AchievementCollection Achievements
	{
		get
		{
			if (!SingletonBehaviour<GameManager>.HasInstance)
			{
				return null;
			}
			return SingletonBehaviour<GameManager>.Instance.achievements;
		}
	}

	public static AudioSettings AudioSettings
	{
		get
		{
			if (!SingletonBehaviour<GameManager>.HasInstance)
			{
				return null;
			}
			return SingletonBehaviour<GameManager>.Instance.audioSettings;
		}
	}

	public static HoleData DrivingRangeHoleData
	{
		get
		{
			if (!SingletonBehaviour<GameManager>.HasInstance)
			{
				return null;
			}
			return SingletonBehaviour<GameManager>.Instance.drivingRangeHoleData;
		}
	}

	public static CourseCollection AllCourses
	{
		get
		{
			if (!SingletonBehaviour<GameManager>.HasInstance)
			{
				return null;
			}
			return SingletonBehaviour<GameManager>.Instance.allCourses;
		}
	}

	public static CourseData CurrentCourse => currentCourse;

	public static Camera Camera
	{
		get
		{
			if (!SingletonBehaviour<GameManager>.HasInstance)
			{
				return null;
			}
			return SingletonBehaviour<GameManager>.Instance.camera;
		}
	}

	public static LevelBoundsTracker CameraLevelBoundsTracker
	{
		get
		{
			if (!SingletonBehaviour<GameManager>.HasInstance)
			{
				return null;
			}
			return SingletonBehaviour<GameManager>.Instance.cameraLevelBoundsTracker;
		}
	}

	public static PlayerInfo LocalPlayerInfo
	{
		get
		{
			if (!SingletonBehaviour<GameManager>.HasInstance)
			{
				return null;
			}
			return SingletonBehaviour<GameManager>.Instance.localPlayerInfo;
		}
	}

	public static PlayerId LocalPlayerId
	{
		get
		{
			if (!SingletonBehaviour<GameManager>.HasInstance)
			{
				return null;
			}
			return SingletonBehaviour<GameManager>.Instance.localPlayerId;
		}
	}

	public static PlayerMovement LocalPlayerMovement
	{
		get
		{
			if (!SingletonBehaviour<GameManager>.HasInstance)
			{
				return null;
			}
			return SingletonBehaviour<GameManager>.Instance.localPlayerMovement;
		}
	}

	public static PlayerGolfer LocalPlayerAsGolfer
	{
		get
		{
			if (!SingletonBehaviour<GameManager>.HasInstance)
			{
				return null;
			}
			return SingletonBehaviour<GameManager>.Instance.localPlayerAsGolfer;
		}
	}

	public static PlayerInventory LocalPlayerInventory
	{
		get
		{
			if (!SingletonBehaviour<GameManager>.HasInstance)
			{
				return null;
			}
			return SingletonBehaviour<GameManager>.Instance.localPlayerInventory;
		}
	}

	public static PlayerInteractableTargeter LocalPlayerInteractableTargeter
	{
		get
		{
			if (!SingletonBehaviour<GameManager>.HasInstance)
			{
				return null;
			}
			return SingletonBehaviour<GameManager>.Instance.localPlayerInteractableTargeter;
		}
	}

	public static PlayerSpectator LocalPlayerAsSpectator
	{
		get
		{
			if (!SingletonBehaviour<GameManager>.HasInstance)
			{
				return null;
			}
			return SingletonBehaviour<GameManager>.Instance.localPlayerAsSpectator;
		}
	}

	public static List<PlayerInfo> RemotePlayers
	{
		get
		{
			if (!SingletonBehaviour<GameManager>.HasInstance)
			{
				return null;
			}
			return SingletonBehaviour<GameManager>.Instance.remotePlayers;
		}
	}

	public static Dictionary<int, PlayerInfo> RemotePlayerPerConnectionId
	{
		get
		{
			if (!SingletonBehaviour<GameManager>.HasInstance)
			{
				return null;
			}
			return SingletonBehaviour<GameManager>.Instance.remotePlayerPerconnectionId;
		}
	}

	public static event Action LocalPlayerRegistered;

	public static event Action LocalPlayerDeregistered;

	public static event Action<PlayerInfo> RemotePlayerRegistered;

	public static event Action<PlayerInfo> RemotePlayerDeregistered;

	public static event Action CurrentCourseSet;

	public static event Action UiHidingModeChanged;

	public static event Action ApplicationGainedFocus;

	public static event Action ApplicationLostFocus;

	public static event Action IsSteamOverlayActiveChanged;

	private static void TimeScaleSet()
	{
		Time.timeScale = timeScaleCvar;
	}

	[CCommand("isAchievementUnlocked", "", false, false)]
	private static void IsAchievementUnlocked(AchievementId id)
	{
		Debug.Log(string.Format("{0} is {1}", id, AchievementsManager.IsUnlocked(id) ? "UNLOCKED" : "locked"));
	}

	[CCommand("printAchievementProgress", "", false, false)]
	private static void PrintAchievementProgress(AchievementId id)
	{
		Debug.Log($"{id} progress: {AchievementsManager.GetProgress(id)}");
		AchievementsManager.GetProgress(id);
	}

	[CCommand("forceCrash", "", false, false)]
	private static void ForceCrash()
	{
		UnityEngine.Diagnostics.Utils.ForceCrash(ForcedCrashCategory.AccessViolation);
	}

	[CCommand("forceCrashOfType", "", false, false)]
	private static void ForceCrashOfType(ForcedCrashCategory type)
	{
		UnityEngine.Diagnostics.Utils.ForceCrash(type);
	}

	[RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
	private static void InitializeBeforeSceneLoad()
	{
		Debug.Log(GameVersionLabel.GetVersion());
		AsyncOperationHandle<GameObject> asyncOperationHandle = Addressables.InstantiateAsync("Assets/Prefabs/Managers/Game manager.prefab");
		asyncOperationHandle.WaitForCompletion();
		if (asyncOperationHandle.Status != AsyncOperationStatus.Succeeded)
		{
			Debug.LogError("Failed to load game manager");
			return;
		}
		UnityEngine.Object.DontDestroyOnLoad(asyncOperationHandle.Result);
		if (SteamEnabler.IsSteamEnabled)
		{
			UnityEngine.Object.DontDestroyOnLoad(UnityEngine.Object.Instantiate(SingletonBehaviour<GameManager>.Instance.steamManagerPrefab));
			AchievementsManager = new SteamAchievementManager();
		}
		else
		{
			AchievementsManager = new DummyAchievementManager();
		}
		AchievementsManager.Initialize();
		UnityEngine.Object.DontDestroyOnLoad(UnityEngine.Object.Instantiate(SingletonBehaviour<GameManager>.Instance.gameplayCameraManagerPrefab));
		UnityEngine.Object.DontDestroyOnLoad(UnityEngine.Object.Instantiate(SingletonBehaviour<GameManager>.Instance.vfxPersistentDataPrefab));
		UnityEngine.Object.DontDestroyOnLoad(UnityEngine.Object.Instantiate(SingletonBehaviour<GameManager>.Instance.persistentUiCanvasPrefab));
		ColorOverlay.SetColor(UiSettings.LoadingScreenBackgroundColor);
	}

	[RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
	private static void InitializeAfterSceneLoad()
	{
		UnityEngine.Object.Instantiate(SingletonBehaviour<GameManager>.Instance.networkManagerPrefab);
	}

	[Server]
	public static void ServerSetCourse(CourseData course)
	{
		if (!NetworkServer.active)
		{
			Debug.LogWarning("[Server] function 'System.Void GameManager::ServerSetCourse(CourseData)' called when server was not active");
		}
		else
		{
			SetCourseInternal(course);
		}
	}

	[Client]
	public static void ClientSetStandardCourse(int courseIndex)
	{
		if (!NetworkClient.active)
		{
			Debug.LogWarning("[Client] function 'System.Void GameManager::ClientSetStandardCourse(System.Int32)' called when client was not active");
		}
		else if (courseIndex < 0)
		{
			Debug.LogError($"Attempted to set course index to {courseIndex}, but it cannot be negative. To set a random or custom course, use the matching method");
		}
		else if (courseIndex >= AllCourses.Courses.Length)
		{
			Debug.LogError($"Attempted to set course index to {courseIndex}, but it is larger than the maximum allowed index of {AllCourses.Courses.Length - 1}");
		}
		else
		{
			SetCourseInternal(AllCourses.Courses[courseIndex]);
		}
	}

	[Client]
	public static void ClientSetNonStandardCourse(int[] globalHoleIndices, bool isRandom)
	{
		if (!NetworkClient.active)
		{
			Debug.LogWarning("[Client] function 'System.Void GameManager::ClientSetNonStandardCourse(System.Int32[],System.Boolean)' called when client was not active");
			return;
		}
		HoleData[] array = new HoleData[globalHoleIndices.Length];
		for (int i = 0; i < array.Length; i++)
		{
			array[i] = AllCourses.allHoles[globalHoleIndices[i]];
		}
		CourseData obj = (isRandom ? MatchSetupMenu.RandomCourseData : MatchSetupMenu.CustomCourseData);
		obj.OverrideHoles(array);
		SetCourseInternal(obj);
	}

	private static void SetCourseInternal(CourseData course)
	{
		currentCourse = course;
		GameManager.CurrentCourseSet?.Invoke();
	}

	protected override void Awake()
	{
		base.Awake();
		InitializeBadWords();
		InitializeSceneReferences();
		allCourses.RuntimeInitialize();
		audioSettings.Initialize();
		MatchSetupMenu.InitializeStatics();
		InputManager.Initialize();
		DevConsole.Initialize();
		PlayerGolfer.InitializeStatics();
		LocalizationManager.Initialize();
		PlayerVoiceChat.InitializeStatics();
		BNetworkManager.DestroyedDueToOfflineSceneTransition += OnNetworkManagerDestroyedDueToOfflineSceneTransition;
		SceneManager.sceneLoaded += OnSceneLoaded;
		SceneManager.sceneUnloaded += OnSceneUnloaded;
		if (SteamEnabler.IsSteamEnabled)
		{
			SteamFriends.OnGameOverlayActivated += OnSteamOverlayIsActivatedChanged;
		}
		Application.quitting += OnApplicationQuit;
		layerSettings = UnityEngine.Object.Instantiate(layerSettings);
	}

	private void Start()
	{
		GameSettings.Initialize();
	}

	protected override void OnDestroy()
	{
		base.OnDestroy();
		UnityEngine.Object.Destroy(layerSettings);
	}

	public static void RegisterPlayer(PlayerInfo playerInfo)
	{
		if (SingletonBehaviour<GameManager>.HasInstance)
		{
			SingletonBehaviour<GameManager>.Instance.RegisterPlayerInternal(playerInfo);
		}
	}

	public static void DeregisterPlayer(PlayerInfo playerInfo)
	{
		if (SingletonBehaviour<GameManager>.HasInstance)
		{
			SingletonBehaviour<GameManager>.Instance.DeregisterPlayerInternal(playerInfo);
		}
	}

	public static void HideUiGroup(UiHidingGroup mode)
	{
		UiHidingGroup hiddenUiGroups = HiddenUiGroups;
		HiddenUiGroups |= mode;
		if (HiddenUiGroups != hiddenUiGroups)
		{
			GameManager.UiHidingModeChanged?.Invoke();
		}
	}

	public static void UnhideUiGroup(UiHidingGroup mode)
	{
		UiHidingGroup hiddenUiGroups = HiddenUiGroups;
		HiddenUiGroups &= ~mode;
		if (HiddenUiGroups != hiddenUiGroups)
		{
			GameManager.UiHidingModeChanged?.Invoke();
		}
	}

	public static void ExitToMainMenu(bool showConfirmation, Action BeforeExit = null)
	{
		if (!SingletonBehaviour<MainMenu>.HasInstance && !isExitingToMainMenu)
		{
			if (!showConfirmation)
			{
				ExitGame(BeforeExit);
				return;
			}
			FullScreenMessage.Show(Localization.UI.PAUSE_ConfirmExitPrompt, new FullScreenMessage.ButtonEntry(Localization.UI.PAUSE_ConfirmExit, ExitWithoutSave), new FullScreenMessage.ButtonEntry(Localization.UI.MISC_Cancel, Cancel, cancel: true));
		}
		static void Cancel()
		{
			FullScreenMessage.Hide();
		}
		static async void ExitGame(Action action)
		{
			try
			{
				isExitingToMainMenu = true;
				InputManager.EnableMode(InputMode.ForceDisabled);
				FullScreenMessage.Hide();
				SingletonBehaviour<PauseMenu>.Instance.gameObject.SetActive(value: false);
				LoadingScreen.Show(Time.timeScale <= 0.25f);
				await UniTask.WaitWhile(() => LoadingScreen.IsFadingScreenIn);
				action?.Invoke();
			}
			catch (Exception exception)
			{
				Debug.LogError("Encountered exception while exiting to main menu. See the next log for details");
				Debug.LogException(exception);
			}
			finally
			{
				isExitingToMainMenu = false;
				InputManager.DisableMode(InputMode.ForceDisabled);
			}
			if (NetworkServer.activeHost)
			{
				BNetworkManager.StopHostWithDisconnectMessage();
			}
			else
			{
				BNetworkManager.singleton.StopClient();
			}
		}
		void ExitWithoutSave()
		{
			ExitGame(BeforeExit);
		}
	}

	public static bool TryFindPlayerByGuid(ulong guid, out PlayerInfo playerInfo)
	{
		playerInfo = null;
		if (!SingletonBehaviour<GameManager>.HasInstance)
		{
			return false;
		}
		if (SingletonBehaviour<GameManager>.Instance.localPlayerId != null && SingletonBehaviour<GameManager>.Instance.localPlayerId.Guid == guid)
		{
			playerInfo = SingletonBehaviour<GameManager>.Instance.localPlayerInfo;
			return true;
		}
		foreach (PlayerInfo remotePlayer in SingletonBehaviour<GameManager>.Instance.remotePlayers)
		{
			if (remotePlayer.PlayerId.Guid == guid)
			{
				playerInfo = remotePlayer;
				return true;
			}
		}
		return false;
	}

	public static bool TryGetPlayerIndex(PlayerInfo player, out int index)
	{
		if (!SingletonBehaviour<GameManager>.HasInstance)
		{
			index = 0;
			return false;
		}
		return SingletonBehaviour<GameManager>.Instance.TryGetPlayerIndexInternal(player, out index);
	}

	public static PlayerInfo GetViewedOrLocalPlayer()
	{
		if (LocalPlayerInfo == null)
		{
			return null;
		}
		if (LocalPlayerAsSpectator.IsSpectating && LocalPlayerAsSpectator.TargetPlayer != null)
		{
			return LocalPlayerAsSpectator.TargetPlayer;
		}
		return LocalPlayerInfo;
	}

	[Server]
	public static LocalSpectatorCameraFollower ServerInstantiateSpectatorCamera(GameObject playerOwner)
	{
		if (!NetworkServer.active)
		{
			Debug.LogWarning("[Server] function 'LocalSpectatorCameraFollower GameManager::ServerInstantiateSpectatorCamera(UnityEngine.GameObject)' called when server was not active");
			return null;
		}
		if (!SingletonBehaviour<GameManager>.HasInstance)
		{
			return null;
		}
		return SingletonBehaviour<GameManager>.Instance.InstantiateSpectatorCameraInternal(playerOwner);
	}

	private void RegisterPlayerInternal(PlayerInfo playerInfo)
	{
		if (playerInfo.isLocalPlayer)
		{
			RegisterLocalPlayer();
		}
		else
		{
			RegisterRemotePlayer();
		}
		void RegisterLocalPlayer()
		{
			localPlayerInfo = playerInfo;
			localPlayerId = localPlayerInfo.PlayerId;
			localPlayerMovement = playerInfo.Movement;
			localPlayerAsGolfer = playerInfo.AsGolfer;
			localPlayerInventory = playerInfo.Inventory;
			localPlayerInteractableTargeter = playerInfo.AsTargeter;
			localPlayerAsSpectator = playerInfo.AsSpectator;
			PhysicsManager.RegisterLocalPlayer(playerInfo);
			GameManager.LocalPlayerRegistered?.Invoke();
		}
		void RegisterRemotePlayer()
		{
			if (!remotePlayerIndices.TryAdd(playerInfo, remotePlayers.Count))
			{
				Debug.LogError(playerInfo.name + " attempted to register as a remote player, but they're already registered");
			}
			else
			{
				remotePlayers.Add(playerInfo);
				if (NetworkServer.active)
				{
					remotePlayerPerconnectionId.Add(playerInfo.connectionToClient.connectionId, playerInfo);
				}
				GameManager.RemotePlayerRegistered?.Invoke(playerInfo);
			}
		}
	}

	private void DeregisterPlayerInternal(PlayerInfo playerInfo)
	{
		if (playerInfo.isLocalPlayer)
		{
			DeregisterLocalPlayer();
		}
		else
		{
			DeregisterRemotePlayer();
		}
		void DeregisterLocalPlayer()
		{
			localPlayerInfo = null;
			localPlayerId = null;
			localPlayerMovement = null;
			localPlayerAsGolfer = null;
			localPlayerInventory = null;
			localPlayerInteractableTargeter = null;
			localPlayerAsSpectator = null;
			PhysicsManager.DeregisterLocalPlayer();
			GameManager.LocalPlayerDeregistered?.Invoke();
		}
		void DeregisterRemotePlayer()
		{
			if (remotePlayerIndices.TryGetValue(playerInfo, out var value))
			{
				Dictionary<PlayerInfo, int> dictionary = remotePlayerIndices;
				List<PlayerInfo> list = remotePlayers;
				dictionary[list[list.Count - 1]] = value;
				remotePlayerIndices.Remove(playerInfo);
				remotePlayers.RemoveAtSwapBack(value);
				if (NetworkServer.active || BNetworkManager.IsServerShuttingDown)
				{
					remotePlayerPerconnectionId.Remove(playerInfo.connectionToClient.connectionId);
				}
				GameManager.RemotePlayerDeregistered?.Invoke(playerInfo);
			}
		}
	}

	private bool TryGetPlayerIndexInternal(PlayerInfo player, out int index)
	{
		return remotePlayerIndices.TryGetValue(player, out index);
	}

	private void InitializeBadWords()
	{
		if (badWordsRegex != null)
		{
			return;
		}
		try
		{
			List<string> value;
			using (CollectionPool<List<string>, string>.Get(out value))
			{
				TextAsset[] array = badWordsTextAssets;
				foreach (TextAsset textAsset in array)
				{
					value.AddRange(from x in textAsset.text.ToLower().Split(new string[3] { "\r\n", "\n", "\r" }, StringSplitOptions.RemoveEmptyEntries)
						where !string.IsNullOrWhiteSpace(x)
						select x);
				}
				badWordsRegex = new Regex("\\b(" + string.Join("|", Enumerable.Select(value, Regex.Escape)) + ")\\b", RegexOptions.IgnoreCase | RegexOptions.Compiled);
			}
		}
		catch (Exception exception)
		{
			Debug.LogError("Failed to load bad words list!");
			Debug.LogException(exception);
		}
	}

	private void InitializeSceneReferences()
	{
		camera = Camera.main;
		if ((bool)camera)
		{
			cameraLevelBoundsTracker = camera.GetComponent<LevelBoundsTracker>();
		}
	}

	private static async void OnNetworkManagerDestroyedDueToOfflineSceneTransition()
	{
		await UniTask.Yield(PlayerLoopTiming.Initialization);
		UnityEngine.Object.Instantiate(SingletonBehaviour<GameManager>.Instance.networkManagerPrefab);
	}

	private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
	{
		InitializeSceneReferences();
	}

	private void OnSceneUnloaded(Scene scene)
	{
		InitializeSceneReferences();
	}

	private void OnSteamOverlayIsActivatedChanged(bool isActive)
	{
		IsSteamOverlayActive = isActive;
		GameManager.IsSteamOverlayActiveChanged?.Invoke();
	}

	[Server]
	private LocalSpectatorCameraFollower InstantiateSpectatorCameraInternal(GameObject playerOwner)
	{
		if (!NetworkServer.active)
		{
			Debug.LogWarning("[Server] function 'LocalSpectatorCameraFollower GameManager::InstantiateSpectatorCameraInternal(UnityEngine.GameObject)' called when server was not active");
			return null;
		}
		LocalSpectatorCameraFollower localSpectatorCameraFollower = UnityEngine.Object.Instantiate(spectatorCameraPrefab, playerOwner.transform.position, Quaternion.identity);
		NetworkServer.Spawn(localSpectatorCameraFollower.gameObject, playerOwner);
		return localSpectatorCameraFollower;
	}

	private void OnApplicationFocus(bool focus)
	{
		if (focus)
		{
			GameManager.ApplicationGainedFocus?.Invoke();
		}
		else
		{
			GameManager.ApplicationLostFocus?.Invoke();
		}
	}

	private void OnApplicationQuit()
	{
		IsApplicationQuitting = true;
		PlayerVoiceChat.OnApplicationShuttingDown();
		if (BNetworkManager.singleton != null)
		{
			BNetworkManager.singleton.OnApplicationShuttingDown();
		}
		if (SteamEnabler.IsSteamEnabled)
		{
			SteamManager.ShutDownSteam();
		}
	}

	public static bool FilterProfanity(string verifyString, out string filteredString)
	{
		try
		{
			filteredString = badWordsRegex.Replace(verifyString, (Match x) => GenerateCensorString(x.Length));
			return verifyString != filteredString;
		}
		catch (Exception exception)
		{
			Debug.LogError("Encountered exception when filtering string for profanity!");
			Debug.LogException(exception);
			filteredString = verifyString;
			return false;
		}
		static string GenerateCensorString(int len)
		{
			char[] collection = new char[6] { '!', '$', '@', '#', '%', '&' };
			List<char> value;
			using (CollectionPool<List<char>, char>.Get(out value))
			{
				string text = string.Empty;
				for (int i = 0; i < len; i++)
				{
					if (value.Count == 0)
					{
						value.AddRange(collection);
						value.Shuffle();
					}
					int index = UnityEngine.Random.Range(0, value.Count);
					char c = value[index];
					value.RemoveAt(index);
					text += c;
				}
				return text;
			}
		}
	}

	public static string RichTextNoParse(string text)
	{
		text = richTextNoParseRegex.Replace(text, string.Empty);
		return "<noparse>" + text + "</noparse>";
	}
}
