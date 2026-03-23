using System;
using System.Net;
using Mirror;
using UnityEngine;
using UnityEngine.Serialization;

namespace kcp2k;

[HelpURL("https://mirror-networking.gitbook.io/docs/transports/kcp-transport")]
[DisallowMultipleComponent]
public class ThreadedKcpTransport : ThreadedTransport, PortTransport
{
	public const string Scheme = "kcp";

	[Header("Transport Configuration")]
	[FormerlySerializedAs("Port")]
	public ushort port = 7777;

	[Tooltip("DualMode listens to IPv6 and IPv4 simultaneously. Disable if the platform only supports IPv4.")]
	public bool DualMode = true;

	[Tooltip("NoDelay is recommended to reduce latency. This also scales better without buffers getting full.")]
	public bool NoDelay = true;

	[Tooltip("KCP internal update interval. 100ms is KCP default, but a lower interval is recommended to minimize latency and to scale to more networked entities.")]
	public uint Interval = 10u;

	[Tooltip("KCP timeout in milliseconds. Note that KCP sends a ping automatically.")]
	public int Timeout = 10000;

	[Tooltip("Socket receive buffer size. Large buffer helps support more connections. Increase operating system socket buffer size limits if needed.")]
	public int RecvBufferSize = 7361536;

	[Tooltip("Socket send buffer size. Large buffer helps support more connections. Increase operating system socket buffer size limits if needed.")]
	public int SendBufferSize = 7361536;

	[Header("Advanced")]
	[Tooltip("KCP fastresend parameter. Faster resend for the cost of higher bandwidth. 0 in normal mode, 2 in turbo mode.")]
	public int FastResend = 2;

	[Tooltip("KCP congestion window. Restricts window size to reduce congestion. Results in only 2-3 MTU messages per Flush even on loopback. Best to keept his disabled.")]
	private bool CongestionWindow;

	[Tooltip("KCP window size can be modified to support higher loads. This also increases max message size.")]
	public uint ReceiveWindowSize = 4096u;

	[Tooltip("KCP window size can be modified to support higher loads.")]
	public uint SendWindowSize = 4096u;

	[Tooltip("KCP will try to retransmit lost messages up to MaxRetransmit (aka dead_link) before disconnecting.")]
	public uint MaxRetransmit = 40u;

	[Tooltip("Enable to automatically set client & server send/recv buffers to OS limit. Avoids issues with too small buffers under heavy load, potentially dropping connections. Increase the OS limit if this is still too small.")]
	[FormerlySerializedAs("MaximizeSendReceiveBuffersToOSLimit")]
	public bool MaximizeSocketBuffers = true;

	[Header("Allowed Max Message Sizes\nBased on Receive Window Size")]
	[Tooltip("KCP reliable max message size shown for convenience. Can be changed via ReceiveWindowSize.")]
	[ReadOnly]
	public int ReliableMaxMessageSize;

	[Tooltip("KCP unreliable channel max message size for convenience. Not changeable.")]
	[ReadOnly]
	public int UnreliableMaxMessageSize;

	protected KcpConfig config;

	private const int MTU = 1200;

	protected KcpServer server;

	protected KcpClient client;

	private volatile bool enabledCopy = true;

	[Header("Debug")]
	public bool debugLog;

	public bool statisticsGUI;

	public bool statisticsLog;

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

	protected override void Awake()
	{
		if (debugLog)
		{
			Log.Info = Debug.Log;
		}
		else
		{
			Log.Info = delegate
			{
			};
		}
		Log.Warning = Debug.LogWarning;
		Log.Error = Debug.LogError;
		config = new KcpConfig(DualMode, RecvBufferSize, SendBufferSize, 1200, NoDelay, Interval, FastResend, CongestionWindow, SendWindowSize, ReceiveWindowSize, Timeout, MaxRetransmit);
		client = new KcpClient(base.OnThreadedClientConnected, delegate(ArraySegment<byte> message, KcpChannel channel)
		{
			OnThreadedClientReceive(message, KcpTransport.FromKcpChannel(channel));
		}, base.OnThreadedClientDisconnected, delegate(ErrorCode error, string reason)
		{
			OnThreadedClientError(KcpTransport.ToTransportError(error), reason);
		}, config);
		server = new KcpServer(base.OnThreadedServerConnected, delegate(int connectionId, ArraySegment<byte> message, KcpChannel channel)
		{
			OnThreadedServerReceive(connectionId, message, KcpTransport.FromKcpChannel(channel));
		}, base.OnThreadedServerDisconnected, delegate(int connectionId, ErrorCode error, string reason)
		{
			OnThreadedServerError(connectionId, KcpTransport.ToTransportError(error), reason);
		}, config);
		if (statisticsLog)
		{
			InvokeRepeating("OnLogStatistics", 1f, 1f);
		}
		base.Awake();
		Log.Info("ThreadedKcpTransport initialized!");
	}

	protected virtual void OnValidate()
	{
		ReliableMaxMessageSize = KcpPeer.ReliableMaxMessageSize(1200, ReceiveWindowSize);
		UnreliableMaxMessageSize = KcpPeer.UnreliableMaxMessageSize(1200);
	}

	private void OnEnable()
	{
		enabledCopy = true;
	}

	private void OnDisable()
	{
		enabledCopy = true;
	}

	public override bool Available()
	{
		return true;
	}

	protected override void ThreadedClientConnect(string address)
	{
		client.Connect(address, Port);
	}

	protected override void ThreadedClientConnect(Uri uri)
	{
		if (uri.Scheme != "kcp")
		{
			throw new ArgumentException(string.Format("Invalid url {0}, use {1}://host:port instead", uri, "kcp"), "uri");
		}
		int num = (uri.IsDefaultPort ? Port : uri.Port);
		client.Connect(uri.Host, (ushort)num);
	}

	protected override void ThreadedClientSend(ArraySegment<byte> segment, int channelId)
	{
		client.Send(segment, KcpTransport.ToKcpChannel(channelId));
		OnThreadedClientSend(segment, channelId);
	}

	protected override void ThreadedClientDisconnect()
	{
		client.Disconnect();
	}

	protected override void ThreadedClientEarlyUpdate()
	{
		if (enabledCopy)
		{
			client.TickIncoming();
		}
	}

	protected override void ThreadedClientLateUpdate()
	{
		client.TickOutgoing();
	}

	public override Uri ServerUri()
	{
		return new UriBuilder
		{
			Scheme = "kcp",
			Host = Dns.GetHostName(),
			Port = Port
		}.Uri;
	}

	protected override void ThreadedServerStart()
	{
		server.Start(Port);
	}

	protected override void ThreadedServerSend(int connectionId, ArraySegment<byte> segment, int channelId)
	{
		server.Send(connectionId, segment, KcpTransport.ToKcpChannel(channelId));
		OnThreadedServerSend(connectionId, segment, channelId);
	}

	protected override void ThreadedServerDisconnect(int connectionId)
	{
		server.Disconnect(connectionId);
	}

	protected override void ThreadedServerStop()
	{
		server.Stop();
	}

	protected override void ThreadedServerEarlyUpdate()
	{
		if (enabledCopy)
		{
			server.TickIncoming();
		}
	}

	protected override void ThreadedServerLateUpdate()
	{
		server.TickOutgoing();
	}

	protected override void ThreadedShutdown()
	{
	}

	public override int GetMaxPacketSize(int channelId = 0)
	{
		if (channelId == 1)
		{
			return KcpPeer.UnreliableMaxMessageSize(config.Mtu);
		}
		return KcpPeer.ReliableMaxMessageSize(config.Mtu, ReceiveWindowSize);
	}

	public override int GetBatchThreshold(int channelId)
	{
		return KcpPeer.UnreliableMaxMessageSize(config.Mtu);
	}

	protected virtual void OnGUIStatistics()
	{
	}

	protected virtual void OnLogStatistics()
	{
	}

	public override string ToString()
	{
		return $"ThreadedKCP {port}";
	}
}
