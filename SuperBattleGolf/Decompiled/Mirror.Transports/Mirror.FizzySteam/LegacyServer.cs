using System;
using System.Collections.Generic;
using Steamworks;
using UnityEngine;

namespace Mirror.FizzySteam;

public class LegacyServer : LegacyCommon, IServer
{
	private BidirectionalDictionary<SteamId, int> steamToMirrorIds;

	private int maxConnections;

	private int nextConnectionID;

	private event Action<int> OnConnected;

	private event Action<int, byte[], int> OnReceivedData;

	private event Action<int> OnDisconnected;

	private event Action<int, Exception> OnReceivedError;

	public static LegacyServer CreateServer(FizzyFacepunch transport, int maxConnections)
	{
		LegacyServer legacyServer = new LegacyServer(transport, maxConnections);
		legacyServer.OnConnected += delegate(int id)
		{
			transport.OnServerConnected(id);
		};
		legacyServer.OnDisconnected += delegate(int id)
		{
			transport.OnServerDisconnected(id);
		};
		legacyServer.OnReceivedData += delegate(int id, byte[] data, int channel)
		{
			transport.OnServerDataReceived(id, new ArraySegment<byte>(data), channel);
		};
		legacyServer.OnReceivedError += delegate(int id, Exception exception)
		{
			transport.OnServerError(id, TransportError.Unexpected, exception.ToString());
		};
		SteamNetworking.OnP2PSessionRequest = delegate(SteamId steamid)
		{
			Debug.Log($"Incoming request from SteamId {steamid}.");
			SteamNetworking.AcceptP2PSessionWithUser(steamid);
		};
		if (!SteamClient.IsValid)
		{
			Debug.LogError("SteamWorks not initialized.");
		}
		return legacyServer;
	}

	private LegacyServer(FizzyFacepunch transport, int maxConnections)
		: base(transport)
	{
		this.maxConnections = maxConnections;
		steamToMirrorIds = new BidirectionalDictionary<SteamId, int>();
		nextConnectionID = 1;
	}

	protected override void OnNewConnection(SteamId id)
	{
		SteamNetworking.AcceptP2PSessionWithUser(id);
	}

	protected override void OnReceiveInternalData(InternalMessages type, SteamId clientSteamID)
	{
		switch (type)
		{
		case InternalMessages.CONNECT:
		{
			if (steamToMirrorIds.Count >= maxConnections)
			{
				SendInternal(clientSteamID, InternalMessages.DISCONNECT);
				break;
			}
			SendInternal(clientSteamID, InternalMessages.ACCEPT_CONNECT);
			int num = nextConnectionID++;
			steamToMirrorIds.Add(clientSteamID, num);
			this.OnConnected(num);
			Debug.Log($"Client with SteamID {clientSteamID} connected. Assigning connection id {num}");
			break;
		}
		case InternalMessages.DISCONNECT:
		{
			if (steamToMirrorIds.TryGetValue(clientSteamID, out var value))
			{
				this.OnDisconnected(value);
				CloseP2PSessionWithUser(clientSteamID);
				steamToMirrorIds.Remove(clientSteamID);
				Debug.Log($"Client with SteamID {clientSteamID} disconnected.");
			}
			else
			{
				this.OnReceivedError(-1, new Exception("ERROR Unknown SteamID while receiving disconnect message."));
			}
			break;
		}
		default:
			Debug.Log("Received unknown message type");
			break;
		}
	}

	protected override void OnReceiveData(byte[] data, SteamId clientSteamID, int channel)
	{
		if (steamToMirrorIds.TryGetValue(clientSteamID, out var value))
		{
			this.OnReceivedData(value, data, channel);
			return;
		}
		CloseP2PSessionWithUser(clientSteamID);
		SteamId steamId = clientSteamID;
		Debug.LogError("Data received from steam client thats not known " + steamId.ToString());
		this.OnReceivedError(-1, new Exception("ERROR Unknown SteamID"));
	}

	public void Disconnect(int connectionId)
	{
		if (steamToMirrorIds.TryGetValue(connectionId, out var value))
		{
			SendInternal(value, InternalMessages.DISCONNECT);
			steamToMirrorIds.Remove(connectionId);
		}
		else
		{
			Debug.LogWarning("Trying to disconnect unknown connection id: " + connectionId);
		}
	}

	public void Shutdown()
	{
		foreach (KeyValuePair<SteamId, int> steamToMirrorId in steamToMirrorIds)
		{
			Disconnect(steamToMirrorId.Value);
			WaitForClose(steamToMirrorId.Key);
		}
		SteamNetworking.OnP2PSessionRequest = null;
		Dispose();
	}

	public void Send(int connectionId, byte[] data, int channelId)
	{
		if (steamToMirrorIds.TryGetValue(connectionId, out var value))
		{
			Send(value, data, channelId);
			return;
		}
		Debug.LogError("Trying to send on unknown connection: " + connectionId);
		this.OnReceivedError(connectionId, new Exception("ERROR Unknown Connection"));
	}

	public string ServerGetClientAddress(int connectionId)
	{
		if (steamToMirrorIds.TryGetValue(connectionId, out var value))
		{
			return value.ToString();
		}
		Debug.LogError("Trying to get info on unknown connection: " + connectionId);
		this.OnReceivedError(connectionId, new Exception("ERROR Unknown Connection"));
		return string.Empty;
	}

	protected override void OnConnectionFailed(SteamId remoteId)
	{
		int value;
		int obj = (steamToMirrorIds.TryGetValue(remoteId, out value) ? value : nextConnectionID++);
		this.OnDisconnected(obj);
	}

	public void FlushData()
	{
	}
}
