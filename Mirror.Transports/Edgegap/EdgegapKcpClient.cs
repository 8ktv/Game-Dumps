using System;
using System.Net.Sockets;
using Mirror;
using UnityEngine;
using kcp2k;

namespace Edgegap;

public class EdgegapKcpClient : KcpClient
{
	private readonly byte[] relayReceiveBuffer;

	public uint userId;

	public uint sessionId;

	public ConnectionState connectionState;

	private double lastPingTime;

	public EdgegapKcpClient(Action OnConnected, Action<ArraySegment<byte>, KcpChannel> OnData, Action OnDisconnected, Action<ErrorCode, string> OnError, KcpConfig config)
		: base(OnConnected, OnData, OnDisconnected, OnError, config)
	{
		relayReceiveBuffer = new byte[config.Mtu + 13];
	}

	public void Connect(string relayAddress, ushort relayPort, uint userId, uint sessionId)
	{
		connectionState = ConnectionState.Checking;
		this.userId = userId;
		this.sessionId = sessionId;
		Connect(relayAddress, relayPort);
	}

	protected override bool RawReceive(out ArraySegment<byte> segment)
	{
		segment = default(ArraySegment<byte>);
		if (socket == null)
		{
			return false;
		}
		try
		{
			if (socket.ReceiveNonBlocking(relayReceiveBuffer, out var data))
			{
				using (NetworkReaderPooled networkReaderPooled = NetworkReaderPool.Get(data))
				{
					if (networkReaderPooled.Remaining != 0)
					{
						switch (networkReaderPooled.ReadByte())
						{
						case 1:
						{
							if (networkReaderPooled.Remaining < 1)
							{
								return false;
							}
							ConnectionState connectionState = this.connectionState;
							this.connectionState = (ConnectionState)networkReaderPooled.ReadByte();
							if (this.connectionState != connectionState)
							{
								Debug.Log($"EdgegapClient: state updated to: {this.connectionState}");
							}
							return true;
						}
						case 2:
							segment = networkReaderPooled.ReadBytesSegment(networkReaderPooled.Remaining);
							return true;
						default:
							return false;
						}
					}
					Debug.LogWarning($"EdgegapClient: message of {data.Count} is too small to parse.");
					return false;
				}
			}
		}
		catch (SocketException arg)
		{
			Log.Info($"EdgegapClient: looks like the other end has closed the connection. This is fine: {arg}");
			Disconnect();
		}
		return false;
	}

	protected override void RawSend(ArraySegment<byte> data)
	{
		using NetworkWriterPooled networkWriterPooled = NetworkWriterPool.Get();
		networkWriterPooled.WriteUInt(userId);
		networkWriterPooled.WriteUInt(sessionId);
		networkWriterPooled.WriteByte(2);
		networkWriterPooled.WriteBytes(data.Array, data.Offset, data.Count);
		base.RawSend((ArraySegment<byte>)networkWriterPooled);
	}

	private void SendPing()
	{
		using NetworkWriterPooled networkWriterPooled = NetworkWriterPool.Get();
		networkWriterPooled.WriteUInt(userId);
		networkWriterPooled.WriteUInt(sessionId);
		networkWriterPooled.WriteByte(1);
		base.RawSend((ArraySegment<byte>)networkWriterPooled);
	}

	public override void TickOutgoing()
	{
		if (connected && NetworkTime.localTime >= lastPingTime + 0.5)
		{
			SendPing();
			lastPingTime = NetworkTime.localTime;
		}
		base.TickOutgoing();
	}
}
