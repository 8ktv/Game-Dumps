using System;
using System.Collections;
using System.Threading;
using Mirror;
using UnityEngine;

namespace Edgegap;

[HelpURL("https://mirror-networking.gitbook.io/docs/manual/transports/edgegap-transports/edgegap-relay")]
public class EdgegapLobbyKcpTransport : EdgegapKcpTransport
{
	public enum TransportStatus
	{
		Offline,
		CreatingLobby,
		StartingLobby,
		JoiningLobby,
		WaitingRelay,
		Connecting,
		Connected,
		Error
	}

	[Header("Lobby Settings")]
	[Tooltip("URL to the Edgegap lobby service, automatically filled in after completing the creation process via button below (or enter manually)")]
	public string lobbyUrl;

	[Tooltip("How long to wait for the relay to be assigned after starting a lobby")]
	public float lobbyWaitTimeout = 60f;

	public LobbyApi Api;

	private LobbyCreateRequest? _request;

	private string _lobbyId;

	private string _playerId;

	private TransportStatus _status;

	public TransportStatus Status
	{
		get
		{
			if (!NetworkClient.active && !NetworkServer.active)
			{
				return TransportStatus.Offline;
			}
			if (_status == TransportStatus.Connecting)
			{
				if (NetworkServer.active)
				{
					switch (((EdgegapKcpServer)server).state)
					{
					case ConnectionState.Valid:
						return TransportStatus.Connected;
					case ConnectionState.Invalid:
					case ConnectionState.SessionTimeout:
					case ConnectionState.Error:
						return TransportStatus.Error;
					}
				}
				else if (NetworkClient.active)
				{
					switch (((EdgegapKcpClient)client).connectionState)
					{
					case ConnectionState.Valid:
						return TransportStatus.Connected;
					case ConnectionState.Invalid:
					case ConnectionState.SessionTimeout:
					case ConnectionState.Error:
						return TransportStatus.Error;
					}
				}
			}
			return _status;
		}
	}

	protected override void Awake()
	{
		base.Awake();
		Api = new LobbyApi(lobbyUrl);
	}

	private void Reset()
	{
		relayGUI = false;
	}

	public override void ServerStart()
	{
		if (!_request.HasValue)
		{
			throw new Exception("No lobby request set. Call SetServerLobbyParams");
		}
		_status = TransportStatus.CreatingLobby;
		Api.CreateLobby(_request.Value, delegate(Lobby lobby)
		{
			_lobbyId = lobby.lobby_id;
			_status = TransportStatus.StartingLobby;
			Api.StartLobby(new LobbyIdRequest(_lobbyId), delegate
			{
				StartCoroutine(WaitForLobbyRelay(_lobbyId, forServer: true));
			}, delegate(string error)
			{
				_status = TransportStatus.Error;
				string text = "Could not start lobby: " + error;
				Debug.LogError(text);
				OnServerError?.Invoke(0, TransportError.Unexpected, text);
				ServerStop();
			});
		}, delegate(string error)
		{
			_status = TransportStatus.Error;
			string text = "Couldn't create lobby: " + error;
			Debug.LogError(text);
			OnServerError?.Invoke(0, TransportError.Unexpected, text);
		});
	}

	public override void ServerStop()
	{
		base.ServerStop();
		Api.DeleteLobby(_lobbyId, delegate
		{
		}, delegate(string error)
		{
			OnServerError?.Invoke(0, TransportError.Unexpected, "Failed to delete lobby: " + error);
		});
	}

	public override void ClientDisconnect()
	{
		base.ClientDisconnect();
		if (!NetworkServer.active)
		{
			Api.LeaveLobby(new LobbyJoinOrLeaveRequest
			{
				player = new LobbyJoinOrLeaveRequest.Player
				{
					id = _playerId
				},
				lobby_id = _lobbyId
			}, delegate
			{
			}, delegate(string error)
			{
				string text = "Failed to leave lobby: " + error;
				OnClientError?.Invoke(TransportError.Unexpected, text);
				Debug.LogError(text);
			});
		}
	}

	public override void ClientConnect(string address)
	{
		_lobbyId = address;
		_playerId = RandomPlayerId();
		_status = TransportStatus.JoiningLobby;
		Api.JoinLobby(new LobbyJoinOrLeaveRequest
		{
			player = new LobbyJoinOrLeaveRequest.Player
			{
				id = _playerId
			},
			lobby_id = address
		}, delegate
		{
			StartCoroutine(WaitForLobbyRelay(_lobbyId, forServer: false));
		}, delegate(string error)
		{
			_status = TransportStatus.Offline;
			string text = "Failed to join lobby: " + error;
			OnClientError?.Invoke(TransportError.Unexpected, text);
			Debug.LogError(text);
			OnClientDisconnected?.Invoke();
		});
	}

	private IEnumerator WaitForLobbyRelay(string lobbyId, bool forServer)
	{
		_status = TransportStatus.WaitingRelay;
		double startTime = NetworkTime.localTime;
		bool running = true;
		while (running)
		{
			if (NetworkTime.localTime - startTime >= (double)lobbyWaitTimeout)
			{
				_status = TransportStatus.Error;
				string text = "Timed out waiting for lobby.";
				Debug.LogError(text);
				if (forServer)
				{
					_status = TransportStatus.Error;
					OnServerError?.Invoke(0, TransportError.Unexpected, text);
					ServerStop();
				}
				else
				{
					_status = TransportStatus.Error;
					OnClientError?.Invoke(TransportError.Unexpected, text);
					ClientDisconnect();
				}
				break;
			}
			bool waitingForResponse = true;
			Api.GetLobby(lobbyId, delegate(Lobby lobby)
			{
				waitingForResponse = false;
				if (!string.IsNullOrEmpty(lobby.assignment.ip))
				{
					relayAddress = lobby.assignment.ip;
					Lobby.Port[] ports = lobby.assignment.ports;
					for (int i = 0; i < ports.Length; i++)
					{
						Lobby.Port port = ports[i];
						if (port.protocol == "UDP")
						{
							if (port.name == "server")
							{
								relayGameServerPort = (ushort)port.port;
							}
							else if (port.name == "client")
							{
								relayGameClientPort = (ushort)port.port;
							}
						}
					}
					bool flag = false;
					Lobby.Player[] players = lobby.players;
					for (int i = 0; i < players.Length; i++)
					{
						Lobby.Player player = players[i];
						if (player.id == _playerId)
						{
							userId = player.authorization_token;
							sessionId = lobby.assignment.authorization_token;
							flag = true;
							break;
						}
					}
					running = false;
					if (!flag)
					{
						string text2 = "Couldn't find my player (" + _playerId + ")";
						Debug.LogError(text2);
						if (forServer)
						{
							_status = TransportStatus.Error;
							OnServerError?.Invoke(0, TransportError.Unexpected, text2);
							ServerStop();
						}
						else
						{
							_status = TransportStatus.Error;
							OnClientError?.Invoke(TransportError.Unexpected, text2);
							ClientDisconnect();
						}
					}
					else
					{
						_status = TransportStatus.Connecting;
						if (forServer)
						{
							base.ServerStart();
						}
						else
						{
							base.ClientConnect("");
						}
					}
				}
			}, delegate(string error)
			{
				running = false;
				waitingForResponse = false;
				_status = TransportStatus.Error;
				string text2 = "Failed to get lobby info: " + error;
				Debug.LogError(text2);
				if (forServer)
				{
					OnServerError?.Invoke(0, TransportError.Unexpected, text2);
					ServerStop();
				}
				else
				{
					OnClientError?.Invoke(TransportError.Unexpected, text2);
					ClientDisconnect();
				}
			});
			while (waitingForResponse)
			{
				yield return null;
			}
			yield return new WaitForSeconds(0.2f);
		}
	}

	private static string RandomPlayerId()
	{
		return $"mirror-player-{UnityEngine.Random.Range(1, int.MaxValue)}";
	}

	public void SetServerLobbyParams(string lobbyName, int capacity)
	{
		SetServerLobbyParams(new LobbyCreateRequest
		{
			player = new LobbyCreateRequest.Player
			{
				id = RandomPlayerId()
			},
			annotations = new LobbyCreateRequest.Annotation[0],
			capacity = capacity,
			is_joinable = true,
			name = lobbyName,
			tags = new string[0]
		});
	}

	public void SetServerLobbyParams(LobbyCreateRequest request)
	{
		_playerId = request.player.id;
		_request = request;
	}

	private void OnDestroy()
	{
		if (NetworkServer.active)
		{
			ServerStop();
			Thread.Sleep(300);
		}
		else if (NetworkClient.active)
		{
			ClientDisconnect();
			Thread.Sleep(300);
		}
	}
}
