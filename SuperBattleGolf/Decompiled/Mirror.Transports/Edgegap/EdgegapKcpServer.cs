using System;
using System.Net;
using System.Net.Sockets;
using Mirror;
using UnityEngine;
using kcp2k;

namespace Edgegap;

public class EdgegapKcpServer : KcpServer
{
	private readonly byte[] relayReceiveBuffer;

	public uint userId;

	public uint sessionId;

	public ConnectionState state;

	protected Socket relaySocket;

	public EndPoint remoteEndPoint;

	private double lastPingTime;

	private bool relayActive;

	public EdgegapKcpServer(Action<int, IPEndPoint> OnConnected, Action<int, ArraySegment<byte>, KcpChannel> OnData, Action<int> OnDisconnected, Action<int, ErrorCode, string> OnError, KcpConfig config)
		: base(OnConnected, OnData, OnDisconnected, OnError, config)
	{
		relayReceiveBuffer = new byte[config.Mtu + 13];
	}

	public override bool IsActive()
	{
		return relayActive;
	}

	public void Start(string relayAddress, ushort relayPort, uint userId, uint sessionId)
	{
		state = ConnectionState.Checking;
		this.userId = userId;
		this.sessionId = sessionId;
		if (!Common.ResolveHostname(relayAddress, out var addresses))
		{
			OnError(0, ErrorCode.DnsResolve, "Failed to resolve host: " + relayAddress);
			return;
		}
		remoteEndPoint = new IPEndPoint(addresses[0], relayPort);
		relaySocket = new Socket(remoteEndPoint.AddressFamily, SocketType.Dgram, ProtocolType.Udp);
		relaySocket.Blocking = false;
		Common.ConfigureSocketBuffers(relaySocket, config.RecvBufferSize, config.SendBufferSize);
		relaySocket.Connect(remoteEndPoint);
		relayActive = true;
	}

	public override void Stop()
	{
		relayActive = false;
	}

	protected override bool RawReceiveFrom(out ArraySegment<byte> segment, out int connectionId)
	{
		segment = default(ArraySegment<byte>);
		connectionId = 0;
		if (relaySocket == null)
		{
			return false;
		}
		try
		{
			if (relaySocket.ReceiveNonBlocking(relayReceiveBuffer, out var data))
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
							ConnectionState connectionState = state;
							state = (ConnectionState)networkReaderPooled.ReadByte();
							if (state != connectionState)
							{
								Debug.Log($"EdgegapServer: state updated to: {state}");
							}
							return true;
						}
						case 2:
							if (networkReaderPooled.Remaining <= 4)
							{
								Debug.LogWarning($"EdgegapServer: message of {data.Count} is too small to parse connId.");
								return false;
							}
							connectionId = networkReaderPooled.ReadInt();
							segment = networkReaderPooled.ReadBytesSegment(networkReaderPooled.Remaining);
							return true;
						default:
							return false;
						}
					}
					Debug.LogWarning($"EdgegapServer: message of {data.Count} is too small to parse header.");
					return false;
				}
			}
		}
		catch (SocketException arg)
		{
			Log.Info($"EdgegapServer: looks like the other end has closed the connection. This is fine: {arg}");
		}
		return false;
	}

	protected override void RawSend(int connectionId, ArraySegment<byte> data)
	{
		using NetworkWriterPooled networkWriterPooled = NetworkWriterPool.Get();
		networkWriterPooled.WriteUInt(userId);
		networkWriterPooled.WriteUInt(sessionId);
		networkWriterPooled.WriteByte(2);
		networkWriterPooled.WriteInt(connectionId);
		networkWriterPooled.WriteBytes(data.Array, data.Offset, data.Count);
		ArraySegment<byte> data2 = networkWriterPooled;
		try
		{
			relaySocket.SendNonBlocking(data2);
		}
		catch (SocketException arg)
		{
			Log.Error($"KcpRleayServer: RawSend failed: {arg}");
		}
	}

	private void SendPing()
	{
		using NetworkWriterPooled networkWriterPooled = NetworkWriterPool.Get();
		networkWriterPooled.WriteUInt(userId);
		networkWriterPooled.WriteUInt(sessionId);
		networkWriterPooled.WriteByte(1);
		ArraySegment<byte> data = networkWriterPooled;
		try
		{
			relaySocket.SendNonBlocking(data);
		}
		catch (SocketException arg)
		{
			Debug.LogWarning($"EdgegapServer: failed to ping. perhaps the relay isn't running? {arg}");
		}
	}

	public override void TickOutgoing()
	{
		if (relayActive && NetworkTime.localTime >= lastPingTime + 0.5)
		{
			SendPing();
			lastPingTime = NetworkTime.localTime;
		}
		base.TickOutgoing();
	}
}
