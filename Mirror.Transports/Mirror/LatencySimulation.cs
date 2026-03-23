using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

namespace Mirror;

[HelpURL("https://mirror-networking.gitbook.io/docs/transports/latency-simulaton-transport")]
[DisallowMultipleComponent]
public class LatencySimulation : Transport, PortTransport
{
	public Transport wrap;

	[Header("Common")]
	[Tooltip("Latency in milliseconds (1000 = 1 second). Always applied to both reliable and unreliable, otherwise unreliable NetworkTime may be behind reliable [SyncVars/Commands/Rpcs] or vice versa!")]
	[Range(0f, 10000f)]
	public float latency = 100f;

	[Tooltip("Jitter latency via perlin(Time * jitterSpeed) * jitter")]
	[FormerlySerializedAs("latencySpikeMultiplier")]
	[Range(0f, 1f)]
	public float jitter = 0.02f;

	[Tooltip("Jitter latency via perlin(Time * jitterSpeed) * jitter")]
	[FormerlySerializedAs("latencySpikeSpeedMultiplier")]
	public float jitterSpeed = 1f;

	[Header("Reliable Messages")]
	[Header("Unreliable Messages")]
	[Tooltip("Packet loss in %\n2% recommended for long term play testing, upto 5% for short bursts.\nAnything higher, or for a prolonged amount of time, suggests user has a connection fault.")]
	[Range(0f, 100f)]
	public float unreliableLoss = 2f;

	[Tooltip("Scramble % of unreliable messages, just like over the real network. Mirror unreliable is unordered.")]
	[Range(0f, 100f)]
	public float unreliableScramble = 2f;

	private readonly List<QueuedMessage> clientToServer = new List<QueuedMessage>();

	private readonly List<QueuedMessage> serverToClient = new List<QueuedMessage>();

	private readonly System.Random random = new System.Random();

	public ushort Port
	{
		get
		{
			if (wrap is PortTransport portTransport)
			{
				return portTransport.Port;
			}
			Debug.LogWarning($"LatencySimulation: attempted to get Port but {wrap} is not a PortTransport.");
			return 0;
		}
		set
		{
			if (wrap is PortTransport portTransport)
			{
				portTransport.Port = value;
			}
			else
			{
				Debug.LogWarning($"LatencySimulation: attempted to set Port but {wrap} is not a PortTransport.");
			}
		}
	}

	public void Awake()
	{
		if (wrap == null)
		{
			throw new Exception("LatencySimulationTransport requires an underlying transport to wrap around.");
		}
	}

	private void OnEnable()
	{
		wrap.enabled = true;
	}

	private void OnDisable()
	{
		wrap.enabled = false;
	}

	protected virtual float Noise(float time)
	{
		return Mathf.PerlinNoise(time, time);
	}

	private float SimulateLatency(int channeldId)
	{
		float num = Noise((float)Time.unscaledTimeAsDouble * jitterSpeed) * jitter;
		return channeldId switch
		{
			0 => latency / 1000f + num, 
			1 => latency / 1000f + num, 
			_ => 0f, 
		};
	}

	private void SimulateSend(int connectionId, ArraySegment<byte> segment, int channelId, float latency, List<QueuedMessage> messageQueue)
	{
		byte[] array = new byte[segment.Count];
		Buffer.BlockCopy(segment.Array, segment.Offset, array, 0, segment.Count);
		double time = Time.unscaledTimeAsDouble + (double)latency;
		QueuedMessage item = new QueuedMessage(connectionId, array, time, channelId);
		if (channelId == 1)
		{
			if (!(random.NextDouble() < (double)(unreliableLoss / 100f)))
			{
				bool num = random.NextDouble() < (double)(unreliableScramble / 100f);
				int count = messageQueue.Count;
				int index = (num ? random.Next(0, count + 1) : count);
				messageQueue.Insert(index, item);
			}
		}
		else
		{
			messageQueue.Add(item);
		}
	}

	public override bool Available()
	{
		return wrap.Available();
	}

	public override void ClientConnect(string address)
	{
		wrap.OnClientConnected = OnClientConnected;
		wrap.OnClientDataReceived = OnClientDataReceived;
		wrap.OnClientError = OnClientError;
		wrap.OnClientTransportException = OnClientTransportException;
		wrap.OnClientDisconnected = OnClientDisconnected;
		wrap.ClientConnect(address);
	}

	public override void ClientConnect(Uri uri)
	{
		wrap.OnClientConnected = OnClientConnected;
		wrap.OnClientDataReceived = OnClientDataReceived;
		wrap.OnClientError = OnClientError;
		wrap.OnClientTransportException = OnClientTransportException;
		wrap.OnClientDisconnected = OnClientDisconnected;
		wrap.ClientConnect(uri);
	}

	public override bool ClientConnected()
	{
		return wrap.ClientConnected();
	}

	public override void ClientDisconnect()
	{
		wrap.ClientDisconnect();
		clientToServer.Clear();
	}

	public override void ClientSend(ArraySegment<byte> segment, int channelId)
	{
		float num = SimulateLatency(channelId);
		SimulateSend(0, segment, channelId, num, clientToServer);
	}

	public override Uri ServerUri()
	{
		return wrap.ServerUri();
	}

	public override bool ServerActive()
	{
		return wrap.ServerActive();
	}

	public override string ServerGetClientAddress(int connectionId)
	{
		return wrap.ServerGetClientAddress(connectionId);
	}

	public override void ServerDisconnect(int connectionId)
	{
		wrap.ServerDisconnect(connectionId);
	}

	public override void ServerSend(int connectionId, ArraySegment<byte> segment, int channelId)
	{
		float num = SimulateLatency(channelId);
		SimulateSend(connectionId, segment, channelId, num, serverToClient);
	}

	public override void ServerStart()
	{
		wrap.OnServerConnected = OnServerConnected;
		wrap.OnServerConnectedWithAddress = OnServerConnectedWithAddress;
		wrap.OnServerDataReceived = OnServerDataReceived;
		wrap.OnServerError = OnServerError;
		wrap.OnServerTransportException = OnServerTransportException;
		wrap.OnServerDisconnected = OnServerDisconnected;
		wrap.ServerStart();
	}

	public override void ServerStop()
	{
		wrap.ServerStop();
		serverToClient.Clear();
	}

	public override void ClientEarlyUpdate()
	{
		wrap.ClientEarlyUpdate();
	}

	public override void ServerEarlyUpdate()
	{
		wrap.ServerEarlyUpdate();
	}

	public override void ClientLateUpdate()
	{
		for (int i = 0; i < clientToServer.Count; i++)
		{
			QueuedMessage queuedMessage = clientToServer[i];
			if (queuedMessage.time <= Time.unscaledTimeAsDouble)
			{
				wrap.ClientSend(new ArraySegment<byte>(queuedMessage.bytes), queuedMessage.channelId);
				clientToServer.RemoveAt(i);
				i--;
			}
		}
		wrap.ClientLateUpdate();
	}

	public override void ServerLateUpdate()
	{
		for (int i = 0; i < serverToClient.Count; i++)
		{
			QueuedMessage queuedMessage = serverToClient[i];
			if (queuedMessage.time <= Time.unscaledTimeAsDouble)
			{
				wrap.ServerSend(queuedMessage.connectionId, new ArraySegment<byte>(queuedMessage.bytes), queuedMessage.channelId);
				serverToClient.RemoveAt(i);
				i--;
			}
		}
		wrap.ServerLateUpdate();
	}

	public override int GetBatchThreshold(int channelId)
	{
		return wrap.GetBatchThreshold(channelId);
	}

	public override int GetMaxPacketSize(int channelId = 0)
	{
		return wrap.GetMaxPacketSize(channelId);
	}

	public override void Shutdown()
	{
		wrap.Shutdown();
	}

	public override string ToString()
	{
		return string.Format("{0} {1}", "LatencySimulation", wrap);
	}
}
