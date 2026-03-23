using System;
using Steamworks;
using Steamworks.Data;
using UnityEngine;

namespace Mirror.FizzySteam;

public class NextServer : NextCommon, IServer
{
	private BidirectionalDictionary<Connection, int> connToMirrorID;

	private BidirectionalDictionary<SteamId, int> steamIDToMirrorID;

	private int maxConnections;

	private int nextConnectionID;

	private FizzySocketManager listenSocket;

	private event Action<int> OnConnected;

	private event Action<int, byte[], int> OnReceivedData;

	private event Action<int> OnDisconnected;

	private event Action<int, Exception> OnReceivedError;

	private NextServer(int maxConnections)
	{
		this.maxConnections = maxConnections;
		connToMirrorID = new BidirectionalDictionary<Connection, int>();
		steamIDToMirrorID = new BidirectionalDictionary<SteamId, int>();
		nextConnectionID = 1;
		SteamNetworkingSockets.OnConnectionStatusChanged += OnConnectionStatusChanged;
	}

	public static NextServer CreateServer(FizzyFacepunch transport, int maxConnections)
	{
		NextServer nextServer = new NextServer(maxConnections);
		nextServer.OnConnected += delegate(int id)
		{
			transport.OnServerConnected(id);
		};
		nextServer.OnDisconnected += delegate(int id)
		{
			transport.OnServerDisconnected(id);
		};
		nextServer.OnReceivedData += delegate(int id, byte[] data, int ch)
		{
			transport.OnServerDataReceived(id, new ArraySegment<byte>(data), ch);
		};
		nextServer.OnReceivedError += delegate(int id, Exception exception)
		{
			transport.OnServerError(id, TransportError.Unexpected, exception.ToString());
		};
		if (!SteamClient.IsValid)
		{
			Debug.LogError("SteamWorks not initialized.");
		}
		nextServer.Host();
		return nextServer;
	}

	private void Host()
	{
		listenSocket = SteamNetworkingSockets.CreateRelaySocket<FizzySocketManager>();
		listenSocket.ForwardMessage = OnMessageReceived;
	}

	private void OnConnectionStatusChanged(Connection conn, ConnectionInfo info)
	{
		ulong num = info.Identity.SteamId;
		if (info.State == ConnectionState.Connecting)
		{
			Result result;
			if (connToMirrorID.Count >= maxConnections)
			{
				Debug.Log($"Incoming connection {num} would exceed max connection count. Rejecting.");
				conn.Close(linger: false, 0, "Max Connection Count");
			}
			else if ((result = conn.Accept()) == Result.OK)
			{
				Debug.Log($"Accepting connection {num}");
			}
			else
			{
				Debug.Log($"Connection {num} could not be accepted: {result.ToString()}");
			}
		}
		else if (info.State == ConnectionState.Connected)
		{
			int num2 = nextConnectionID++;
			connToMirrorID.Add(conn, num2);
			steamIDToMirrorID.Add(num, num2);
			this.OnConnected(num2);
			Debug.Log($"Client with SteamID {num} connected. Assigning connection id {num2}");
		}
		else if (info.State == ConnectionState.ClosedByPeer)
		{
			if (connToMirrorID.TryGetValue(conn, out var value))
			{
				InternalDisconnect(value, conn);
			}
		}
		else
		{
			Debug.Log($"Connection {num} state changed: {info.State.ToString()}");
		}
	}

	private void InternalDisconnect(int connId, Connection socket)
	{
		this.OnDisconnected(connId);
		socket.Close(linger: false, 0, "Graceful disconnect");
		connToMirrorID.Remove(connId);
		steamIDToMirrorID.Remove(connId);
		Debug.Log($"Client with SteamID {connId} disconnected.");
	}

	public void Disconnect(int connectionId)
	{
		if (connToMirrorID.TryGetValue(connectionId, out var value))
		{
			Debug.Log($"Connection id {connectionId} disconnected.");
			value.Close(linger: false, 0, "Disconnected by server");
			steamIDToMirrorID.Remove(connectionId);
			connToMirrorID.Remove(connectionId);
			this.OnDisconnected(connectionId);
		}
		else
		{
			Debug.LogWarning("Trying to disconnect unknown connection id: " + connectionId);
		}
	}

	public void FlushData()
	{
		foreach (Connection firstType in connToMirrorID.FirstTypes)
		{
			firstType.Flush();
		}
	}

	public void ReceiveData()
	{
		listenSocket.Receive(256);
	}

	private void OnMessageReceived(Connection conn, IntPtr dataPtr, int size)
	{
		var (arg, arg2) = ProcessMessage(dataPtr, size);
		this.OnReceivedData(connToMirrorID[conn], arg, arg2);
	}

	public void Send(int connectionId, byte[] data, int channelId)
	{
		if (connToMirrorID.TryGetValue(connectionId, out var value))
		{
			Result result = SendSocket(value, data, channelId);
			switch (result)
			{
			case Result.NoConnection:
			case Result.InvalidParam:
				Debug.Log($"Connection to {connectionId} was lost.");
				InternalDisconnect(connectionId, value);
				break;
			default:
				Debug.LogError("Could not send: " + result);
				break;
			case Result.OK:
				break;
			}
		}
		else
		{
			Debug.LogError("Trying to send on unknown connection: " + connectionId);
			this.OnReceivedError(connectionId, new Exception("ERROR Unknown Connection"));
		}
	}

	public string ServerGetClientAddress(int connectionId)
	{
		if (steamIDToMirrorID.TryGetValue(connectionId, out var value))
		{
			return value.ToString();
		}
		Debug.LogError("Trying to get info on unknown connection: " + connectionId);
		this.OnReceivedError(connectionId, new Exception("ERROR Unknown Connection"));
		return string.Empty;
	}

	public void Shutdown()
	{
		if (listenSocket != null)
		{
			SteamNetworkingSockets.OnConnectionStatusChanged -= OnConnectionStatusChanged;
			listenSocket.Close();
		}
	}
}
