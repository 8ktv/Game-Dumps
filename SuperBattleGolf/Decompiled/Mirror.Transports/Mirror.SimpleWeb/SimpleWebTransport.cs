using System;
using System.Net;
using System.Security.Authentication;
using UnityEngine;
using UnityEngine.Serialization;

namespace Mirror.SimpleWeb;

[DisallowMultipleComponent]
[HelpURL("https://mirror-networking.gitbook.io/docs/manual/transports/websockets-transport")]
public class SimpleWebTransport : Transport, PortTransport
{
	public const string NormalScheme = "ws";

	public const string SecureScheme = "wss";

	[Tooltip("Protect against allocation attacks by keeping the max message size small. Otherwise an attacker might send multiple fake packets with 2GB headers, causing the server to run out of memory after allocating multiple large packets.")]
	public int maxMessageSize = 16384;

	[FormerlySerializedAs("handshakeMaxSize")]
	[Tooltip("Max size for http header send as handshake for websockets")]
	public int maxHandshakeSize = 16384;

	[FormerlySerializedAs("serverMaxMessagesPerTick")]
	[Tooltip("Caps the number of messages the server will process per tick. Allows LateUpdate to finish to let the reset of unity continue in case more messages arrive before they are processed")]
	public int serverMaxMsgsPerTick = 10000;

	[FormerlySerializedAs("clientMaxMessagesPerTick")]
	[Tooltip("Caps the number of messages the client will process per tick. Allows LateUpdate to finish to let the reset of unity continue in case more messages arrive before they are processed")]
	public int clientMaxMsgsPerTick = 1000;

	[Tooltip("Send would stall forever if the network is cut off during a send, so we need a timeout (in milliseconds)")]
	public int sendTimeout = 5000;

	[Tooltip("How long without a message before disconnecting (in milliseconds)")]
	public int receiveTimeout = 20000;

	[Tooltip("disables nagle algorithm. lowers CPU% and latency but increases bandwidth")]
	public bool noDelay = true;

	[Header("Obsolete SSL settings")]
	[Tooltip("Requires wss connections on server, only to be used with SSL cert.json, never with reverse proxy.\nNOTE: if sslEnabled is true clientUseWss is forced true, even if not checked.")]
	public bool sslEnabled;

	[Tooltip("Protocols that SSL certificate is created to support.")]
	public SslProtocols sslProtocols = SslProtocols.Tls12;

	[Tooltip("Path to json file that contains path to cert and its password\nUse Json file so that cert password is not included in client builds\nSee Assets/Mirror/Transports/.cert.example.Json")]
	public string sslCertJson = "./cert.json";

	[Header("Server settings")]
	[Tooltip("Port to use for server")]
	public ushort port = 27777;

	[Tooltip("Groups messages in queue before calling Stream.Send")]
	public bool batchSend = true;

	[Tooltip("Waits for 1ms before grouping and sending messages.\nThis gives time for mirror to finish adding message to queue so that less groups need to be made.\nIf WaitBeforeSend is true then BatchSend Will also be set to true")]
	public bool waitBeforeSend = true;

	[Header("Client settings")]
	[Tooltip("Sets connect scheme to wss. Useful when client needs to connect using wss when TLS is outside of transport.\nNOTE: if sslEnabled is true clientUseWss is also true")]
	public bool clientUseWss;

	public ClientWebsocketSettings clientWebsocketSettings = new ClientWebsocketSettings
	{
		ClientPortOption = WebsocketPortOption.DefaultSameAsServer,
		CustomClientPort = 7777
	};

	[Header("Logging")]
	[Tooltip("Choose minimum severity level for logging\nFlood level requires Debug build")]
	[SerializeField]
	private Log.Levels minimumLogLevel = Log.Levels.Warn;

	private SimpleWebClient client;

	private SimpleWebServer server;

	public ushort Port
	{
		get
		{
			return port;
		}
		set
		{
			port = value;
		}
	}

	public Log.Levels LogLevels
	{
		get
		{
			return minimumLogLevel;
		}
		set
		{
			minimumLogLevel = value;
			Log.minLogLevel = minimumLogLevel;
		}
	}

	private TcpConfig TcpConfig => new TcpConfig(noDelay, sendTimeout, receiveTimeout);

	public override bool IsEncrypted
	{
		get
		{
			if (!ClientConnected() || (!clientUseWss && !sslEnabled))
			{
				if (ServerActive())
				{
					return sslEnabled;
				}
				return false;
			}
			return true;
		}
	}

	public override string EncryptionCipher => "TLS";

	private void Awake()
	{
		Log.minLogLevel = minimumLogLevel;
	}

	public override string ToString()
	{
		return $"SWT [{Port}]";
	}

	private void OnValidate()
	{
		Log.minLogLevel = minimumLogLevel;
	}

	public override bool Available()
	{
		return true;
	}

	public override int GetMaxPacketSize(int channelId = 0)
	{
		return maxMessageSize;
	}

	public override void Shutdown()
	{
		client?.Disconnect();
		client = null;
		server?.Stop();
		server = null;
	}

	private string GetClientScheme()
	{
		if (!sslEnabled && !clientUseWss)
		{
			return "ws";
		}
		return "wss";
	}

	public override bool ClientConnected()
	{
		if (client != null)
		{
			return client.ConnectionState != ClientState.NotConnected;
		}
		return false;
	}

	public override void ClientConnect(string hostname)
	{
		UriBuilder uriBuilder = new UriBuilder
		{
			Scheme = GetClientScheme(),
			Host = hostname
		};
		switch (clientWebsocketSettings.ClientPortOption)
		{
		case WebsocketPortOption.SpecifyPort:
			uriBuilder.Port = clientWebsocketSettings.CustomClientPort;
			break;
		default:
			uriBuilder.Port = port;
			break;
		case WebsocketPortOption.MatchWebpageProtocol:
			break;
		}
		ClientConnect(uriBuilder.Uri);
	}

	public override void ClientConnect(Uri uri)
	{
		if (ClientConnected())
		{
			Log.Warn("[SWT-ClientConnect]: Already Connected");
			return;
		}
		client = SimpleWebClient.Create(maxMessageSize, clientMaxMsgsPerTick, TcpConfig);
		if (client == null)
		{
			return;
		}
		client.onConnect += OnClientConnected.Invoke;
		client.onDisconnect += delegate
		{
			OnClientDisconnected();
			client = null;
		};
		client.onData += delegate(ArraySegment<byte> data)
		{
			OnClientDataReceived(data, 0);
		};
		switch (Log.minLogLevel)
		{
		case Log.Levels.Flood:
		case Log.Levels.Verbose:
			client.onError += delegate(Exception e)
			{
				OnClientError(TransportError.Unexpected, e.ToString());
				ClientDisconnect();
			};
			break;
		case Log.Levels.Info:
		case Log.Levels.Warn:
		case Log.Levels.Error:
			client.onError += delegate(Exception e)
			{
				OnClientError(TransportError.Unexpected, e.Message);
				ClientDisconnect();
			};
			break;
		}
		client.Connect(uri);
	}

	public override void ClientDisconnect()
	{
		client?.Disconnect();
	}

	public override void ClientSend(ArraySegment<byte> segment, int channelId)
	{
		if (!ClientConnected())
		{
			Log.Error("[SWT-ClientSend]: Not Connected");
			return;
		}
		if (segment.Count > maxMessageSize)
		{
			Log.Error("[SWT-ClientSend]: Message greater than max size");
			return;
		}
		if (segment.Count == 0)
		{
			Log.Error("[SWT-ClientSend]: Message count was zero");
			return;
		}
		client.Send(segment);
		OnClientDataSent?.Invoke(segment, 0);
	}

	public override void ClientEarlyUpdate()
	{
		client?.ProcessMessageQueue(this);
	}

	private string GetServerScheme()
	{
		if (!sslEnabled)
		{
			return "ws";
		}
		return "wss";
	}

	public override Uri ServerUri()
	{
		return new UriBuilder
		{
			Scheme = GetServerScheme(),
			Host = Dns.GetHostName(),
			Port = port
		}.Uri;
	}

	public override bool ServerActive()
	{
		if (server != null)
		{
			return server.Active;
		}
		return false;
	}

	public override void ServerStart()
	{
		if (ServerActive())
		{
			Log.Warn("[SWT-ServerStart]: Server Already Started");
		}
		SslConfig sslConfig = SslConfigLoader.Load(sslEnabled, sslCertJson, sslProtocols);
		server = new SimpleWebServer(serverMaxMsgsPerTick, TcpConfig, maxMessageSize, maxHandshakeSize, sslConfig);
		server.onConnect += OnServerConnectedWithAddress.Invoke;
		server.onDisconnect += OnServerDisconnected.Invoke;
		server.onData += delegate(int connId, ArraySegment<byte> data)
		{
			OnServerDataReceived(connId, data, 0);
		};
		switch (Log.minLogLevel)
		{
		case Log.Levels.Flood:
		case Log.Levels.Verbose:
			server.onError += delegate(int connId, Exception exception)
			{
				OnServerError(connId, TransportError.Unexpected, exception.ToString());
				ServerDisconnect(connId);
			};
			break;
		case Log.Levels.Info:
		case Log.Levels.Warn:
		case Log.Levels.Error:
			server.onError += delegate(int connId, Exception exception)
			{
				OnServerError(connId, TransportError.Unexpected, exception.Message);
				ServerDisconnect(connId);
			};
			break;
		}
		SendLoopConfig.batchSend = batchSend || waitBeforeSend;
		SendLoopConfig.sleepBeforeSend = waitBeforeSend;
		server.Start(port);
	}

	public override void ServerStop()
	{
		if (ServerActive())
		{
			server.Stop();
			server = null;
		}
	}

	public override void ServerDisconnect(int connectionId)
	{
		if (ServerActive())
		{
			server.KickClient(connectionId);
		}
	}

	public override void ServerSend(int connectionId, ArraySegment<byte> segment, int channelId)
	{
		if (!ServerActive())
		{
			Log.Error("[SWT-ServerSend]: Server Not Active");
			return;
		}
		if (segment.Count > maxMessageSize)
		{
			Log.Error("[SWT-ServerSend]: Message greater than max size");
			return;
		}
		if (segment.Count == 0)
		{
			Log.Error("[SWT-ServerSend]: Message count was zero");
			return;
		}
		server.SendOne(connectionId, segment);
		OnServerDataSent?.Invoke(connectionId, segment, 0);
	}

	public override string ServerGetClientAddress(int connectionId)
	{
		return server.GetClientAddress(connectionId);
	}

	public Request ServerGetClientRequest(int connectionId)
	{
		return server.GetClientRequest(connectionId);
	}

	public override void ServerEarlyUpdate()
	{
		server?.ProcessMessageQueue(this);
	}
}
