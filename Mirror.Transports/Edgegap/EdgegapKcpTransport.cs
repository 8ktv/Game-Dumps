using System;
using System.Net;
using System.Text.RegularExpressions;
using Mirror;
using UnityEngine;
using kcp2k;

namespace Edgegap;

[HelpURL("https://mirror-networking.gitbook.io/docs/manual/transports/edgegap-transports/edgegap-relay")]
public class EdgegapKcpTransport : KcpTransport
{
	[Header("Relay")]
	public string relayAddress = "127.0.0.1";

	public ushort relayGameServerPort = 8888;

	public ushort relayGameClientPort = 9999;

	public const int MaxPayload = 1187;

	[Header("Relay")]
	public bool relayGUI = true;

	public uint userId = 11111111u;

	public uint sessionId = 22222222u;

	internal static string ReParse(string cmd, string pattern, string defaultValue)
	{
		Match match = Regex.Match(cmd, pattern);
		if (!match.Success)
		{
			return defaultValue;
		}
		return match.Groups[1].Value;
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
		config = new KcpConfig(DualMode, RecvBufferSize, SendBufferSize, 1187, NoDelay, Interval, FastResend, CongestionWindow: false, SendWindowSize, ReceiveWindowSize, Timeout, MaxRetransmit);
		client = new EdgegapKcpClient(delegate
		{
			OnClientConnected();
		}, delegate(ArraySegment<byte> message, KcpChannel channel)
		{
			OnClientDataReceived(message, KcpTransport.FromKcpChannel(channel));
		}, delegate
		{
			OnClientDisconnected?.Invoke();
		}, delegate(ErrorCode error, string reason)
		{
			OnClientError(KcpTransport.ToTransportError(error), reason);
		}, config);
		server = new EdgegapKcpServer(delegate(int connectionId, IPEndPoint endPoint)
		{
			OnServerConnectedWithAddress(connectionId, endPoint.PrettyAddress());
		}, delegate(int connectionId, ArraySegment<byte> message, KcpChannel channel)
		{
			OnServerDataReceived(connectionId, message, KcpTransport.FromKcpChannel(channel));
		}, delegate(int connectionId)
		{
			OnServerDisconnected(connectionId);
		}, delegate(int connectionId, ErrorCode error, string reason)
		{
			OnServerError(connectionId, KcpTransport.ToTransportError(error), reason);
		}, config);
		if (statisticsLog)
		{
			InvokeRepeating("OnLogStatistics", 1f, 1f);
		}
		Debug.Log("EdgegapTransport initialized!");
	}

	protected override void OnValidate()
	{
		ReliableMaxMessageSize = KcpPeer.ReliableMaxMessageSize(1187, ReceiveWindowSize);
		UnreliableMaxMessageSize = KcpPeer.UnreliableMaxMessageSize(1187);
	}

	public override void ClientConnect(string address)
	{
		EdgegapKcpClient obj = (EdgegapKcpClient)client;
		obj.userId = userId;
		obj.sessionId = sessionId;
		obj.connectionState = ConnectionState.Checking;
		obj.Connect(relayAddress, relayGameClientPort);
	}

	public override void ClientConnect(Uri uri)
	{
		if (uri.Scheme != "kcp")
		{
			throw new ArgumentException(string.Format("Invalid url {0}, use {1}://host:port instead", uri, "kcp"), "uri");
		}
		((EdgegapKcpClient)client).Connect(relayAddress, relayGameClientPort, userId, sessionId);
	}

	public override void ServerStart()
	{
		((EdgegapKcpServer)server).Start(relayAddress, relayGameServerPort, userId, sessionId);
	}

	private void OnGUIRelay()
	{
		GUILayout.BeginArea(new Rect(300f, 30f, 200f, 100f));
		GUILayout.BeginHorizontal();
		GUILayout.Label("SessionId:");
		sessionId = Convert.ToUInt32(GUILayout.TextField(sessionId.ToString()));
		GUILayout.EndHorizontal();
		GUILayout.BeginHorizontal();
		GUILayout.Label("UserId:");
		userId = Convert.ToUInt32(GUILayout.TextField(userId.ToString()));
		GUILayout.EndHorizontal();
		if (NetworkServer.active)
		{
			EdgegapKcpServer obj = (EdgegapKcpServer)server;
			GUILayout.BeginHorizontal();
			GUILayout.Label("State:");
			GUILayout.Label(obj.state.ToString());
			GUILayout.EndHorizontal();
		}
		else if (NetworkClient.active)
		{
			EdgegapKcpClient obj2 = (EdgegapKcpClient)client;
			GUILayout.BeginHorizontal();
			GUILayout.Label("State:");
			GUILayout.Label(obj2.connectionState.ToString());
			GUILayout.EndHorizontal();
		}
		GUILayout.EndArea();
	}

	private void OnGUI()
	{
		if (relayGUI)
		{
			OnGUIRelay();
		}
	}

	public override string ToString()
	{
		return "Edgegap Kcp Transport";
	}
}
