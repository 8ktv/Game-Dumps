using System;
using Steamworks;
using UnityEngine;

namespace Mirror.FizzySteam;

[HelpURL("https://github.com/Chykary/FizzyFacepunch")]
public class FizzyFacepunch : Transport
{
	private const string STEAM_SCHEME = "steam";

	private static IClient client;

	private static IServer server;

	[SerializeField]
	public P2PSend[] Channels = new P2PSend[2]
	{
		P2PSend.Reliable,
		P2PSend.UnreliableNoDelay
	};

	[Tooltip("Timeout for connecting in seconds.")]
	public int Timeout = 25;

	[Tooltip("Allow or disallow P2P connections to fall back to being relayed through the Steam servers if a direct connection or NAT-traversal cannot be established.")]
	public bool AllowSteamRelay = true;

	[Tooltip("Use SteamSockets instead of the (deprecated) SteamNetworking. This will always use Relay.")]
	public bool UseNextGenSteamNetworking = true;

	[Tooltip("Check this if you want the transport to initialise Facepunch.")]
	public bool InitFacepunch = true;

	[Header("Info")]
	[Tooltip("This will display your Steam User ID when you start or connect to a server.")]
	public ulong SteamUserID;

	public void Initialize()
	{
		if (InitFacepunch && InitialiseSteamworks(SteamManager.appId))
		{
			Debug.Log("SteamWorks initialised");
			FetchSteamID();
		}
	}

	public override void ClientEarlyUpdate()
	{
		if (base.enabled)
		{
			client?.ReceiveData();
		}
	}

	public override void ServerEarlyUpdate()
	{
		if (base.enabled)
		{
			server?.ReceiveData();
		}
	}

	public override void ClientLateUpdate()
	{
		if (base.enabled)
		{
			client?.FlushData();
		}
	}

	public override void ServerLateUpdate()
	{
		if (base.enabled)
		{
			server?.FlushData();
		}
	}

	public override bool ClientConnected()
	{
		if (ClientActive())
		{
			return client.Connected;
		}
		return false;
	}

	public override void ClientConnect(string address)
	{
		if (!SteamClient.IsValid)
		{
			Debug.LogError("SteamWorks not initialized. Client could not be started.");
			OnClientDisconnected();
			return;
		}
		if (address == SteamUserID.ToString())
		{
			Debug.Log("You can't connect to yourself.");
			return;
		}
		FetchSteamID();
		if (ServerActive())
		{
			Debug.LogError("Transport already running as server!");
		}
		else if (!ClientActive() || client.Error)
		{
			if (UseNextGenSteamNetworking)
			{
				Debug.Log("Starting client [SteamSockets], target address " + address + ".");
				client = NextClient.CreateClient(this, address);
			}
			else
			{
				Debug.Log($"Starting client [DEPRECATED SteamNetworking], target address {address}. Relay enabled: {AllowSteamRelay}");
				SteamNetworking.AllowP2PPacketRelay(AllowSteamRelay);
				client = LegacyClient.CreateClient(this, address);
			}
		}
		else
		{
			Debug.LogError("Client already running!");
		}
	}

	public override void ClientConnect(Uri uri)
	{
		if (uri.Scheme != "steam")
		{
			throw new ArgumentException(string.Format("Invalid url {0}, use {1}://SteamID instead", uri, "steam"), "uri");
		}
		ClientConnect(uri.Host);
	}

	public override void ClientSend(ArraySegment<byte> segment, int channelId)
	{
		byte[] array = new byte[segment.Count];
		Array.Copy(segment.Array, segment.Offset, array, 0, segment.Count);
		client.Send(array, channelId);
	}

	public override void ClientDisconnect()
	{
		if (ClientActive())
		{
			Shutdown();
		}
	}

	public bool ClientActive()
	{
		return client != null;
	}

	public override bool ServerActive()
	{
		return server != null;
	}

	public override void ServerStart()
	{
		if (!SteamClient.IsValid)
		{
			Debug.LogError("SteamWorks not initialized. Server could not be started.");
			return;
		}
		FetchSteamID();
		if (ClientActive())
		{
			Debug.LogError("Transport already running as client!");
		}
		else if (!ServerActive())
		{
			if (UseNextGenSteamNetworking)
			{
				Debug.Log("Starting server [SteamSockets].");
				server = NextServer.CreateServer(this, NetworkManager.singleton.maxConnections);
			}
			else
			{
				Debug.Log($"Starting server [DEPRECATED SteamNetworking]. Relay enabled: {AllowSteamRelay}");
				SteamNetworking.AllowP2PPacketRelay(AllowSteamRelay);
				server = LegacyServer.CreateServer(this, NetworkManager.singleton.maxConnections);
			}
		}
		else
		{
			Debug.LogError("Server already started!");
		}
	}

	public override Uri ServerUri()
	{
		return new UriBuilder
		{
			Scheme = "steam",
			Host = SteamClient.SteamId.Value.ToString()
		}.Uri;
	}

	public override void ServerSend(int connectionId, ArraySegment<byte> segment, int channelId)
	{
		if (ServerActive())
		{
			byte[] array = new byte[segment.Count];
			Array.Copy(segment.Array, segment.Offset, array, 0, segment.Count);
			server.Send(connectionId, array, channelId);
		}
	}

	public override void ServerDisconnect(int connectionId)
	{
		if (ServerActive())
		{
			server.Disconnect(connectionId);
		}
	}

	public override string ServerGetClientAddress(int connectionId)
	{
		if (!ServerActive())
		{
			return string.Empty;
		}
		return server.ServerGetClientAddress(connectionId);
	}

	public override void ServerStop()
	{
		if (ServerActive())
		{
			Shutdown();
		}
	}

	public override void Shutdown()
	{
		if (server != null)
		{
			server.Shutdown();
			server = null;
			Debug.Log("Transport shut down - was server.");
		}
		if (client != null)
		{
			client.Disconnect();
			client = null;
			Debug.Log("Transport shut down - was client.");
		}
	}

	public override int GetMaxPacketSize(int channelId)
	{
		if (channelId >= Channels.Length)
		{
			Debug.LogError("Channel Id exceeded configured channels! Please configure more channels.");
			return 1200;
		}
		switch (Channels[channelId])
		{
		case P2PSend.Unreliable:
		case P2PSend.UnreliableNoDelay:
			return 1200;
		case P2PSend.Reliable:
		case P2PSend.ReliableWithBuffering:
			return 1048576;
		default:
			throw new NotSupportedException();
		}
	}

	public override bool Available()
	{
		try
		{
			return SteamClient.IsValid;
		}
		catch
		{
			return false;
		}
	}

	private void FetchSteamID()
	{
		if (SteamClient.IsValid)
		{
			if (UseNextGenSteamNetworking)
			{
				SteamNetworkingUtils.InitRelayNetworkAccess();
			}
			SteamUserID = SteamClient.SteamId;
		}
	}

	private bool InitialiseSteamworks(uint appid)
	{
		try
		{
			if (!SteamClient.IsValid)
			{
				SteamClient.Init(appid);
			}
		}
		catch (Exception ex)
		{
			Debug.LogError("Could be one of the following: Steam is closed, Can't find steam_api dlls or Don't have permission to open appid. Exception: " + ex.Message);
			return false;
		}
		return true;
	}

	private void OnDestroy()
	{
		Shutdown();
	}
}
