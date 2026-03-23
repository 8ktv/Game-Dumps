using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;

namespace Mirror;

public class NetworkConnectionToClient : NetworkConnection
{
	private readonly NetworkWriter reliableRpcs = new NetworkWriter();

	private readonly NetworkWriter unreliableRpcs = new NetworkWriter();

	public readonly int connectionId;

	public readonly HashSet<NetworkIdentity> observing = new HashSet<NetworkIdentity>();

	public Unbatcher unbatcher = new Unbatcher();

	private ExponentialMovingAverage driftEma;

	private ExponentialMovingAverage deliveryTimeEma;

	public double remoteTimeline;

	public double remoteTimescale;

	private double bufferTimeMultiplier = 2.0;

	private readonly SortedList<double, TimeSnapshot> snapshots = new SortedList<double, TimeSnapshot>();

	public int snapshotBufferSizeLimit = 64;

	private double lastPingTime;

	internal ExponentialMovingAverage _rtt = new ExponentialMovingAverage(50);

	public virtual string address { get; private set; }

	private double bufferTime => (double)NetworkServer.sendInterval * bufferTimeMultiplier;

	public double rtt => _rtt.Value;

	internal NetworkConnectionToClient()
	{
	}

	public NetworkConnectionToClient(int networkConnectionId, string clientAddress = "localhost")
	{
		connectionId = networkConnectionId;
		address = clientAddress;
		driftEma = new ExponentialMovingAverage(NetworkServer.sendRate * NetworkClient.snapshotSettings.driftEmaDuration);
		deliveryTimeEma = new ExponentialMovingAverage(NetworkServer.sendRate * NetworkClient.snapshotSettings.deliveryTimeEmaDuration);
		snapshotBufferSizeLimit = Mathf.Max((int)NetworkClient.snapshotSettings.bufferTimeMultiplier, snapshotBufferSizeLimit);
	}

	public override string ToString()
	{
		return $"connection({connectionId})";
	}

	public void OnTimeSnapshot(TimeSnapshot snapshot)
	{
		if (snapshots.Count < snapshotBufferSizeLimit)
		{
			if (NetworkClient.snapshotSettings.dynamicAdjustment)
			{
				bufferTimeMultiplier = SnapshotInterpolation.DynamicAdjustment(NetworkServer.sendInterval, deliveryTimeEma.StandardDeviation, NetworkClient.snapshotSettings.dynamicAdjustmentTolerance);
			}
			SnapshotInterpolation.InsertAndAdjust(snapshots, NetworkClient.snapshotSettings.bufferLimit, snapshot, ref remoteTimeline, ref remoteTimescale, NetworkServer.sendInterval, bufferTime, NetworkClient.snapshotSettings.catchupSpeed, NetworkClient.snapshotSettings.slowdownSpeed, ref driftEma, NetworkClient.snapshotSettings.catchupNegativeThreshold, NetworkClient.snapshotSettings.catchupPositiveThreshold, ref deliveryTimeEma);
		}
	}

	public void UpdateTimeInterpolation()
	{
		if (snapshots.Count > 0)
		{
			SnapshotInterpolation.StepTime(Time.unscaledDeltaTime, ref remoteTimeline, remoteTimescale);
			SnapshotInterpolation.StepInterpolation(snapshots, remoteTimeline, out var _, out var _, out var _);
		}
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	protected override void SendToTransport(ArraySegment<byte> segment, int channelId = 0)
	{
		Transport.active.ServerSend(connectionId, segment, channelId);
	}

	protected virtual void UpdatePing()
	{
		if (NetworkTime.localTime >= lastPingTime + (double)NetworkTime.PingInterval)
		{
			NetworkPingMessage message = new NetworkPingMessage(NetworkTime.localTime, 0.0);
			Send(message, 1);
			lastPingTime = NetworkTime.localTime;
		}
	}

	internal override void Update()
	{
		UpdatePing();
		base.Update();
	}

	public override void Disconnect()
	{
		isReady = false;
		reliableRpcs.Position = 0;
		unreliableRpcs.Position = 0;
		Transport.active.ServerDisconnect(connectionId);
	}

	internal void AddToObserving(NetworkIdentity netIdentity)
	{
		observing.Add(netIdentity);
		NetworkServer.ShowForConnection(netIdentity, this);
	}

	internal void RemoveFromObserving(NetworkIdentity netIdentity, bool isDestroyed)
	{
		observing.Remove(netIdentity);
		if (!isDestroyed)
		{
			NetworkServer.HideForConnection(netIdentity, this);
		}
	}

	internal void RemoveFromObservingsObservers()
	{
		foreach (NetworkIdentity item in observing)
		{
			item.RemoveObserver(this);
		}
		observing.Clear();
	}

	internal void AddOwnedObject(NetworkIdentity obj)
	{
		owned.Add(obj);
	}

	internal void RemoveOwnedObject(NetworkIdentity obj)
	{
		owned.Remove(obj);
	}

	internal void DestroyOwnedObjects()
	{
		foreach (NetworkIdentity item in new HashSet<NetworkIdentity>(owned))
		{
			if (item != null)
			{
				if (item.sceneId != 0L)
				{
					NetworkServer.RemovePlayerForConnection(this);
				}
				else
				{
					NetworkServer.Destroy(item.gameObject);
				}
			}
		}
		owned.Clear();
	}
}
