using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Security.Cryptography;
using System.Text;
using Cysharp.Threading.Tasks;
using Mirror;
using Mirror.FizzySteam;
using Steamworks;
using UnityEngine;

public class BClientAuthenticator : NetworkAuthenticator
{
	private enum AuthenticationResponse
	{
		Accept,
		WrongVersion,
		WrongPassword,
		LobbyFull,
		NotAcceptingConnections,
		StillKicked
	}

	[StructLayout(LayoutKind.Sequential, Size = 1)]
	private struct ClientCanceledAuthenticationMessage : NetworkMessage
	{
	}

	[StructLayout(LayoutKind.Sequential, Size = 1)]
	private struct IsPasswordRequiredRequestMessage : NetworkMessage
	{
	}

	private struct IsPasswordRequiredResponseMessage : NetworkMessage
	{
		public bool isPasswordRequired;
	}

	private struct AuthenticationRequestMessage : NetworkMessage
	{
		public string versionGuid;

		public byte[] passwordHash;
	}

	private struct AuthenticationResponseMessage : NetworkMessage
	{
		public AuthenticationResponse response;
	}

	public static string serverPassword = string.Empty;

	[SerializeField]
	private string versionGuid;

	private readonly HashSet<NetworkConnection> connectionsPendingDisconnect = new HashSet<NetworkConnection>();

	public readonly HashSet<ulong> playersKickedThisSession = new HashSet<ulong>();

	public readonly HashSet<ulong> invitedPlayersThisSession = new HashSet<ulong>();

	[CCommand("setNetworkVersionGuid", "Sets the version used for client authentication. Use for testing authentication only", false, false)]
	public static void SetVersionGuid(string guid)
	{
		if (BNetworkManager.singleton == null || BNetworkManager.singleton.ClientAuthenticator == null)
		{
			Debug.Log("Could not find a client authenticator");
		}
		else
		{
			BNetworkManager.singleton.ClientAuthenticator.versionGuid = guid;
		}
	}

	[CCommand("printNetworkVersionGuid", "", false, false)]
	private static void PrintGuid()
	{
		if (BNetworkManager.singleton == null || BNetworkManager.singleton.ClientAuthenticator == null)
		{
			Debug.Log("Could not find a client authenticator");
		}
		else
		{
			Debug.Log(BNetworkManager.singleton.ClientAuthenticator.versionGuid);
		}
	}

	[CCommand("copyNetworkVersionGuid", "", false, false)]
	private static void CopyGuid()
	{
		if (BNetworkManager.singleton == null || BNetworkManager.singleton.ClientAuthenticator == null)
		{
			Debug.Log("Could not find a client authenticator");
			return;
		}
		GUIUtility.systemCopyBuffer = BNetworkManager.singleton.ClientAuthenticator.versionGuid;
		Debug.Log("Copied network version GUID " + BNetworkManager.singleton.ClientAuthenticator.versionGuid + " to clipboard");
	}

	public override void OnStartServer()
	{
		NetworkServer.RegisterHandler<ClientCanceledAuthenticationMessage>(OnServerClientCanceledAuthenticationMessage, requireAuthentication: false);
		NetworkServer.RegisterHandler<AuthenticationRequestMessage>(OnServerAuthenticationRequestMessage, requireAuthentication: false);
		NetworkServer.RegisterHandler<IsPasswordRequiredRequestMessage>(OnServerIsPasswordRequiredRequestMessage, requireAuthentication: false);
		playersKickedThisSession.Clear();
		invitedPlayersThisSession.Clear();
	}

	public override void OnStopServer()
	{
		NetworkServer.UnregisterHandler<ClientCanceledAuthenticationMessage>();
		NetworkServer.UnregisterHandler<AuthenticationRequestMessage>();
		NetworkServer.UnregisterHandler<IsPasswordRequiredRequestMessage>();
		playersKickedThisSession.Clear();
		invitedPlayersThisSession.Clear();
	}

	public override void OnServerAuthenticate(NetworkConnectionToClient connection)
	{
		if (connection == NetworkServer.localConnection)
		{
			connection.Send(new AuthenticationResponseMessage
			{
				response = AuthenticationResponse.Accept
			});
			ServerAccept(connection);
		}
	}

	private void OnServerClientCanceledAuthenticationMessage(NetworkConnectionToClient connection, ClientCanceledAuthenticationMessage message)
	{
		connection.Disconnect();
	}

	private void OnServerIsPasswordRequiredRequestMessage(NetworkConnectionToClient connection, IsPasswordRequiredRequestMessage message)
	{
		connection.Send(new IsPasswordRequiredResponseMessage
		{
			isPasswordRequired = !string.IsNullOrEmpty(serverPassword)
		});
	}

	private void OnServerAuthenticationRequestMessage(NetworkConnectionToClient connection, AuthenticationRequestMessage message)
	{
		if (connectionsPendingDisconnect.Contains(connection))
		{
			return;
		}
		if (!BNetworkManager.IsAcceptingConnections)
		{
			DenyRequest(AuthenticationResponse.NotAcceptingConnections);
			return;
		}
		if (message.versionGuid != versionGuid)
		{
			DenyRequest(AuthenticationResponse.WrongVersion);
			return;
		}
		int num = 0;
		foreach (NetworkConnectionToClient value in NetworkServer.connections.Values)
		{
			if (value != connection)
			{
				num++;
			}
		}
		if (num >= BNetworkManager.MaxPlayers)
		{
			DenyRequest(AuthenticationResponse.LobbyFull);
			return;
		}
		if (!string.IsNullOrEmpty(serverPassword))
		{
			if (message.passwordHash == null || message.passwordHash.Length == 0)
			{
				DenyRequest(AuthenticationResponse.WrongPassword);
				return;
			}
			string text = serverPassword;
			if (NetworkManager.singleton.transport is FizzyFacepunch)
			{
				text += SteamClient.SteamId.ToString();
			}
			using SHA256 sHA = SHA256.Create();
			if (!sHA.ComputeHash(Encoding.UTF8.GetBytes(text)).SequenceEqual(message.passwordHash))
			{
				DenyRequest(AuthenticationResponse.WrongPassword);
				return;
			}
		}
		if (SteamEnabler.IsSteamEnabled && BNetworkManager.singleton.TryGetSteamPlayerGuid(connection, out var steamId) && steamId != (ulong)SteamClient.SteamId)
		{
			if (playersKickedThisSession.Contains(steamId))
			{
				DenyRequest(AuthenticationResponse.StillKicked);
				return;
			}
			if (BNetworkManager.TryGetPlayerInLobby(steamId, out var player))
			{
				if (PlayerInfo.IsBlockedOnSteam(player.Relationship))
				{
					DenyRequest(AuthenticationResponse.StillKicked);
					return;
				}
				if (BNetworkManager.LobbyMode != LobbyMode.Public && !player.IsFriend)
				{
					WaitForFriendshipConfirm();
					return;
				}
			}
		}
		AcceptRequest();
		void AcceptRequest()
		{
			connection.Send(new AuthenticationResponseMessage
			{
				response = AuthenticationResponse.Accept
			});
			ServerAccept(connection);
		}
		void DenyRequest(AuthenticationResponse response)
		{
			Debug.Log($"Sending connection {connection} authentication failed message with reason {response}");
			connection.Send(new AuthenticationResponseMessage
			{
				response = response
			});
			connection.isAuthenticated = false;
			StartCoroutine(DisconnectClient(connection));
		}
		IEnumerator DisconnectClient(NetworkConnectionToClient networkConnectionToClient)
		{
			connectionsPendingDisconnect.Add(networkConnectionToClient);
			yield return new WaitForSeconds(1f);
			ServerReject(networkConnectionToClient);
			yield return null;
			connectionsPendingDisconnect.Remove(networkConnectionToClient);
		}
		async void WaitForFriendshipConfirm()
		{
			bool isDone = false;
			BNetworkManager.ServerRequestFriendConfirmation(steamId);
			BNetworkManager.OnServerFriendshipConfirmed += OnFriendshipConfirm;
			try
			{
				await UniTask.WhenAny(UniTask.WaitForSeconds(2f), UniTask.WaitUntil(() => isDone));
				if (!(this == null) && NetworkServer.active && !connection.isAuthenticated)
				{
					if (isDone)
					{
						AcceptRequest();
					}
					else
					{
						DenyRequest(AuthenticationResponse.NotAcceptingConnections);
					}
				}
			}
			finally
			{
				BNetworkManager.OnServerFriendshipConfirmed -= OnFriendshipConfirm;
			}
			void OnFriendshipConfirm(ulong guid)
			{
				if (guid == steamId)
				{
					isDone = true;
				}
			}
		}
	}

	public override void OnStartClient()
	{
		NetworkClient.RegisterHandler<AuthenticationResponseMessage>(OnClientAuthenticationResponseMessage, requireAuthentication: false);
		NetworkClient.RegisterHandler<IsPasswordRequiredResponseMessage>(OnClientIsPasswordRequiredResponseMessage, requireAuthentication: false);
	}

	public override void OnStopClient()
	{
		NetworkClient.UnregisterHandler<AuthenticationResponseMessage>();
		NetworkClient.UnregisterHandler<IsPasswordRequiredResponseMessage>();
	}

	public override void OnClientAuthenticate()
	{
		if (!NetworkServer.active)
		{
			NetworkClient.Send(default(IsPasswordRequiredRequestMessage));
		}
	}

	private void OnClientIsPasswordRequiredResponseMessage(IsPasswordRequiredResponseMessage message)
	{
		AuthenticationRequestMessage request;
		if (NetworkClient.active)
		{
			request = default(AuthenticationRequestMessage);
			request.versionGuid = versionGuid;
			if (!message.isPasswordRequired)
			{
				NetworkClient.Send(request);
				return;
			}
			FullScreenMessage.Hide();
			FullScreenMessage.ShowTextField(string.Empty, Localization.UI.AUTHENTICATION_Password, string.Empty, Localization.UI.AUTHENTICATION_EnterPassword, true, new FullScreenMessage.ButtonEntry(Localization.UI.AUTHENTICATION_Connect, Connect), new FullScreenMessage.ButtonEntry(Localization.UI.MISC_Cancel, Cancel));
		}
		void Cancel()
		{
			StartCoroutine(ClientCancelAuthentication());
			FullScreenMessage.Hide();
		}
		void Connect()
		{
			string inputFieldText = FullScreenMessage.InputFieldText;
			if (inputFieldText != string.Empty)
			{
				string text = inputFieldText;
				if (NetworkManager.singleton.transport is FizzyFacepunch)
				{
					text += NetworkManager.singleton.networkAddress;
				}
				using SHA256 sHA = SHA256.Create();
				request.passwordHash = sHA.ComputeHash(Encoding.UTF8.GetBytes(text));
			}
			NetworkClient.Send(request);
			FullScreenMessage.Hide();
		}
	}

	private void OnClientAuthenticationResponseMessage(AuthenticationResponseMessage message)
	{
		if (NetworkClient.active)
		{
			if (message.response == AuthenticationResponse.Accept)
			{
				Debug.Log("Client authentication successful");
				FullScreenMessage.Hide();
				ClientAccept();
			}
			else
			{
				ClientReject();
				Debug.Log($"Client connection failed with reason: {message.response}");
				BNetworkManager.singleton.InformClientAuthenticationRejected();
				LoadingScreen.Hide();
				InputManager.DisableMode(InputMode.ForceDisabled);
				FullScreenMessage.Hide();
				FullScreenMessage.ShowErrorMessage(LocalizationManager.GetString(StringTable.UI, $"ERROR_ConnectionFailed_{message.response}"), "", Localization.UI.ERROR_ConnectionFailed, 0);
			}
			if (message.response != AuthenticationResponse.Accept)
			{
				NetworkClient.Disconnect();
			}
		}
	}

	private IEnumerator ClientCancelAuthentication()
	{
		NetworkClient.Send(default(ClientCanceledAuthenticationMessage));
		yield return new WaitForSeconds(0.1f);
		NetworkClient.Disconnect();
	}
}
