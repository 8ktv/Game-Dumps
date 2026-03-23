using System;
using System.Collections;
using Steamworks;
using Steamworks.Data;
using UnityEngine;

namespace Mirror.FizzySteam;

public abstract class LegacyCommon
{
	protected enum InternalMessages : byte
	{
		CONNECT,
		ACCEPT_CONNECT,
		DISCONNECT
	}

	private P2PSend[] channels;

	protected readonly FizzyFacepunch transport;

	private int internal_ch => channels.Length;

	protected LegacyCommon(FizzyFacepunch transport)
	{
		channels = transport.Channels;
		SteamNetworking.OnP2PSessionRequest = (Action<SteamId>)Delegate.Combine(SteamNetworking.OnP2PSessionRequest, new Action<SteamId>(OnNewConnection));
		SteamNetworking.OnP2PConnectionFailed = (Action<SteamId, P2PSessionError>)Delegate.Combine(SteamNetworking.OnP2PConnectionFailed, new Action<SteamId, P2PSessionError>(OnConnectFail));
		this.transport = transport;
	}

	protected void WaitForClose(SteamId cSteamID)
	{
		transport.StartCoroutine(DelayedClose(cSteamID));
	}

	private IEnumerator DelayedClose(SteamId cSteamID)
	{
		yield return null;
		CloseP2PSessionWithUser(cSteamID);
	}

	protected void Dispose()
	{
		SteamNetworking.OnP2PSessionRequest = (Action<SteamId>)Delegate.Remove(SteamNetworking.OnP2PSessionRequest, new Action<SteamId>(OnNewConnection));
		SteamNetworking.OnP2PConnectionFailed = (Action<SteamId, P2PSessionError>)Delegate.Remove(SteamNetworking.OnP2PConnectionFailed, new Action<SteamId, P2PSessionError>(OnConnectFail));
	}

	protected abstract void OnNewConnection(SteamId steamID);

	private void OnConnectFail(SteamId id, P2PSessionError err)
	{
		OnConnectionFailed(id);
		CloseP2PSessionWithUser(id);
		switch (err)
		{
		case P2PSessionError.NotRunningApp_DELETED:
			throw new Exception("Connection failed: The target user is not running the same game.");
		case P2PSessionError.NoRightsToApp:
			throw new Exception("Connection failed: The local user doesn't own the app that is running.");
		case P2PSessionError.DestinationNotLoggedIn_DELETED:
			throw new Exception("Connection failed: Target user isn't connected to Steam.");
		case P2PSessionError.Timeout:
			throw new Exception("Connection failed: The connection timed out because the target user didn't respond.");
		default:
			throw new Exception("Connection failed: Unknown error.");
		}
	}

	protected bool SendInternal(SteamId target, InternalMessages type)
	{
		return SteamNetworking.SendP2PPacket(target, new byte[1] { (byte)type }, 1, internal_ch);
	}

	protected void Send(SteamId host, byte[] msgBuffer, int channel)
	{
		SteamNetworking.SendP2PPacket(host, msgBuffer, msgBuffer.Length, channel, channels[Mathf.Min(channel, channels.Length - 1)]);
	}

	private bool Receive(out SteamId clientSteamID, out byte[] receiveBuffer, int channel)
	{
		if (SteamNetworking.IsP2PPacketAvailable(channel))
		{
			P2Packet? p2Packet = SteamNetworking.ReadP2PPacket(channel);
			if (p2Packet.HasValue)
			{
				receiveBuffer = p2Packet.Value.Data;
				clientSteamID = p2Packet.Value.SteamId;
				return true;
			}
		}
		receiveBuffer = null;
		clientSteamID = 0uL;
		return false;
	}

	protected void CloseP2PSessionWithUser(SteamId clientSteamID)
	{
		SteamNetworking.CloseP2PSessionWithUser(clientSteamID);
	}

	public void ReceiveData()
	{
		try
		{
			SteamId clientSteamID;
			byte[] receiveBuffer;
			while (transport.enabled && Receive(out clientSteamID, out receiveBuffer, internal_ch))
			{
				if (receiveBuffer.Length == 1)
				{
					OnReceiveInternalData((InternalMessages)receiveBuffer[0], clientSteamID);
					return;
				}
				Debug.Log("Incorrect package length on internal channel.");
			}
			for (int i = 0; i < channels.Length; i++)
			{
				SteamId clientSteamID2;
				byte[] receiveBuffer2;
				while (transport.enabled && Receive(out clientSteamID2, out receiveBuffer2, i))
				{
					OnReceiveData(receiveBuffer2, clientSteamID2, i);
				}
			}
		}
		catch (Exception exception)
		{
			Debug.LogException(exception);
		}
	}

	protected abstract void OnReceiveInternalData(InternalMessages type, SteamId clientSteamID);

	protected abstract void OnReceiveData(byte[] data, SteamId clientSteamID, int channel);

	protected abstract void OnConnectionFailed(SteamId remoteId);
}
