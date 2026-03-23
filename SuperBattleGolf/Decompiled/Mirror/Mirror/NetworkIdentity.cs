using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Mirror.RemoteCalls;
using UnityEngine;
using UnityEngine.Serialization;

namespace Mirror;

[DisallowMultipleComponent]
[DefaultExecutionOrder(-1)]
[AddComponentMenu("Network/Network Identity")]
[HelpURL("https://mirror-networking.gitbook.io/docs/components/network-identity")]
public sealed class NetworkIdentity : MonoBehaviour
{
	public delegate void ClientAuthorityCallback(NetworkConnectionToClient conn, NetworkIdentity identity, bool authorityState);

	internal bool clientStarted;

	public readonly Dictionary<int, NetworkConnectionToClient> observers = new Dictionary<int, NetworkConnectionToClient>();

	[FormerlySerializedAs("m_SceneId")]
	[HideInInspector]
	public ulong sceneId;

	[SerializeField]
	[HideInInspector]
	private uint _assetId;

	[FormerlySerializedAs("m_ServerOnly")]
	[Tooltip("Prevents this object from being spawned / enabled on clients")]
	public bool serverOnly;

	internal bool destroyCalled;

	private NetworkConnectionToClient _connectionToClient;

	private const int MaxNetworkBehaviours = 64;

	[Tooltip("Visibility can overwrite interest management. ForceHidden can be useful to hide monsters while they respawn. ForceShown can be useful for score NetworkIdentities that should always broadcast to everyone in the world.")]
	[FormerlySerializedAs("visible")]
	public Visibility visibility;

	private NetworkIdentitySerialization lastSerialization = new NetworkIdentitySerialization
	{
		ownerWriterReliable = new NetworkWriter(),
		observersWriterReliable = new NetworkWriter(),
		ownerWriterUnreliableBaseline = new NetworkWriter(),
		observersWriterUnreliableBaseline = new NetworkWriter(),
		ownerWriterUnreliableDelta = new NetworkWriter(),
		observersWriterUnreliableDelta = new NetworkWriter()
	};

	internal double lastUnreliableStateTime;

	internal byte lastUnreliableBaselineSent;

	internal byte lastUnreliableBaselineReceived;

	private static readonly Dictionary<ulong, NetworkIdentity> sceneIds = new Dictionary<ulong, NetworkIdentity>();

	private static uint nextNetworkId = 1u;

	[SerializeField]
	[HideInInspector]
	private bool hasSpawned;

	internal static NetworkIdentity previousLocalPlayer = null;

	private bool hadAuthority;

	public bool isClient { get; internal set; }

	public bool isServer { get; internal set; }

	public bool isHost
	{
		get
		{
			if (isServer)
			{
				return isClient;
			}
			return false;
		}
	}

	public bool isLocalPlayer { get; internal set; }

	public bool isServerOnly
	{
		get
		{
			if (isServer)
			{
				return !isClient;
			}
			return false;
		}
	}

	public bool isClientOnly
	{
		get
		{
			if (isClient)
			{
				return !isServer;
			}
			return false;
		}
	}

	public bool isOwned { get; internal set; }

	public uint netId { get; internal set; }

	public uint assetId
	{
		get
		{
			return _assetId;
		}
		internal set
		{
			if (value == 0)
			{
				Debug.LogError($"Can not set AssetId to empty guid on NetworkIdentity '{base.name}', old assetId '{_assetId}'");
			}
			else
			{
				_assetId = value;
			}
		}
	}

	public NetworkConnection connectionToServer { get; internal set; }

	public NetworkConnectionToClient connectionToClient
	{
		get
		{
			return _connectionToClient;
		}
		internal set
		{
			_connectionToClient?.RemoveOwnedObject(this);
			_connectionToClient = value;
			_connectionToClient?.AddOwnedObject(this);
		}
	}

	public NetworkBehaviour[] NetworkBehaviours { get; private set; }

	public bool SpawnedFromInstantiate { get; private set; }

	public static event ClientAuthorityCallback clientAuthorityCallback;

	internal void HandleRemoteCall(byte componentIndex, ushort functionHash, RemoteCallType remoteCallType, NetworkReader reader, NetworkConnectionToClient senderConnection = null)
	{
		if (this == null)
		{
			Debug.LogWarning($"{remoteCallType} [{functionHash}] received for deleted object [netId={netId}]");
			return;
		}
		if (componentIndex >= NetworkBehaviours.Length)
		{
			Debug.LogWarning($"Component [{componentIndex}] not found for [netId={netId}]");
			return;
		}
		NetworkBehaviour component = NetworkBehaviours[componentIndex];
		if (!RemoteProcedureCalls.Invoke(functionHash, remoteCallType, reader, component, senderConnection))
		{
			Debug.LogError($"Found no receiver for incoming {remoteCallType} [{functionHash}] on {base.gameObject.name}, the server and client should have the same NetworkBehaviour instances [netId={netId}].");
		}
	}

	[RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
	internal static void ResetStatics()
	{
		ResetClientStatics();
		ResetServerStatics();
	}

	internal static void ResetClientStatics()
	{
		previousLocalPlayer = null;
		NetworkIdentity.clientAuthorityCallback = null;
	}

	internal static void ResetServerStatics()
	{
		nextNetworkId = 1u;
	}

	public static NetworkIdentity GetSceneIdentity(ulong id)
	{
		return sceneIds[id];
	}

	internal static uint GetNextNetworkId()
	{
		return nextNetworkId++;
	}

	public static void ResetNextNetworkId()
	{
		nextNetworkId = 1u;
	}

	internal void InitializeNetworkBehaviours()
	{
		NetworkBehaviours = GetComponentsInChildren<NetworkBehaviour>(includeInactive: true);
		ValidateComponents();
		for (int i = 0; i < NetworkBehaviours.Length; i++)
		{
			NetworkBehaviour obj = NetworkBehaviours[i];
			obj.netIdentity = this;
			obj.ComponentIndex = (byte)i;
		}
	}

	private void ValidateComponents()
	{
		if (NetworkBehaviours == null)
		{
			Debug.LogError("NetworkBehaviours array is null on " + base.gameObject.name + "!\nTypically this can happen when a networked object is a child of a non-networked parent that's disabled, preventing Awake on the networked object from being invoked, where the NetworkBehaviours array is initialized.", base.gameObject);
		}
		else if (NetworkBehaviours.Length > 64)
		{
			Debug.LogError($"NetworkIdentity {base.name} has too many NetworkBehaviour components: only {64} NetworkBehaviour components are allowed in order to save bandwidth.", this);
		}
	}

	internal void Awake()
	{
		InitializeNetworkBehaviours();
		if (hasSpawned)
		{
			Debug.LogError(base.name + " has already spawned. Don't call Instantiate for NetworkIdentities that were in the scene since the beginning (aka scene objects).  Otherwise the client won't know which object to use for a SpawnSceneObject message.");
			SpawnedFromInstantiate = true;
			UnityEngine.Object.Destroy(base.gameObject);
		}
		hasSpawned = true;
	}

	private void OnValidate()
	{
		hasSpawned = false;
	}

	public static uint AssetGuidToUint(Guid guid)
	{
		return (uint)guid.GetHashCode();
	}

	private void OnDestroy()
	{
		if (SpawnedFromInstantiate)
		{
			return;
		}
		if (isServer && !destroyCalled)
		{
			NetworkServer.Destroy(base.gameObject);
		}
		if (isLocalPlayer && NetworkClient.localPlayer == this)
		{
			NetworkClient.localPlayer = null;
		}
		if (isClient)
		{
			if (NetworkClient.connection != null)
			{
				NetworkClient.connection.owned.Remove(this);
			}
			if (NetworkClient.spawned.TryGetValue(netId, out var value) && (value == this || value == null))
			{
				NetworkClient.spawned.Remove(netId);
			}
		}
		NetworkBehaviours = null;
	}

	internal void OnStartServer()
	{
		NetworkBehaviour[] networkBehaviours = NetworkBehaviours;
		foreach (NetworkBehaviour networkBehaviour in networkBehaviours)
		{
			try
			{
				networkBehaviour.OnStartServer();
			}
			catch (Exception exception)
			{
				Debug.LogException(exception, networkBehaviour);
			}
		}
	}

	internal void OnStopServer()
	{
		NetworkBehaviour[] networkBehaviours = NetworkBehaviours;
		foreach (NetworkBehaviour networkBehaviour in networkBehaviours)
		{
			try
			{
				networkBehaviour.OnStopServer();
			}
			catch (Exception exception)
			{
				Debug.LogException(exception, networkBehaviour);
			}
		}
	}

	internal void OnStartClient()
	{
		if (clientStarted)
		{
			return;
		}
		clientStarted = true;
		NetworkBehaviour[] networkBehaviours = NetworkBehaviours;
		foreach (NetworkBehaviour networkBehaviour in networkBehaviours)
		{
			try
			{
				networkBehaviour.OnStartClient();
			}
			catch (Exception exception)
			{
				Debug.LogException(exception, networkBehaviour);
			}
		}
	}

	public void ForceOnStopClient()
	{
		OnStopClient();
	}

	internal void OnStopClient()
	{
		if (!clientStarted)
		{
			return;
		}
		NetworkBehaviour[] networkBehaviours = NetworkBehaviours;
		foreach (NetworkBehaviour networkBehaviour in networkBehaviours)
		{
			try
			{
				networkBehaviour.OnStopClient();
			}
			catch (Exception exception)
			{
				Debug.LogException(exception, networkBehaviour);
			}
		}
	}

	internal void OnStartLocalPlayer()
	{
		if (previousLocalPlayer == this)
		{
			return;
		}
		previousLocalPlayer = this;
		NetworkBehaviour[] networkBehaviours = NetworkBehaviours;
		foreach (NetworkBehaviour networkBehaviour in networkBehaviours)
		{
			try
			{
				networkBehaviour.OnStartLocalPlayer();
			}
			catch (Exception exception)
			{
				Debug.LogException(exception, networkBehaviour);
			}
		}
	}

	public void ForceOnStopLocalPlayer()
	{
		OnStopLocalPlayer();
	}

	internal void OnStopLocalPlayer()
	{
		NetworkBehaviour[] networkBehaviours = NetworkBehaviours;
		foreach (NetworkBehaviour networkBehaviour in networkBehaviours)
		{
			try
			{
				networkBehaviour.OnStopLocalPlayer();
			}
			catch (Exception exception)
			{
				Debug.LogException(exception, networkBehaviour);
			}
		}
	}

	private (ulong, ulong) ServerDirtyMasks_Spawn()
	{
		ulong num = 0uL;
		ulong num2 = 0uL;
		NetworkBehaviour[] networkBehaviours = NetworkBehaviours;
		for (int i = 0; i < networkBehaviours.Length; i++)
		{
			NetworkBehaviour obj = networkBehaviours[i];
			ulong num3 = (ulong)(1L << i);
			num |= num3;
			if (obj.syncMode == SyncMode.Observers)
			{
				num2 |= num3;
			}
		}
		return (num, num2);
	}

	private void ServerDirtyMasks_Broadcast(out ulong ownerMaskReliable, out ulong observerMaskReliable, out ulong ownerMaskUnreliableBaseline, out ulong observerMaskUnreliableBaseline, out ulong ownerMaskUnreliableDelta, out ulong observerMaskUnreliableDelta)
	{
		ownerMaskReliable = 0uL;
		observerMaskReliable = 0uL;
		ownerMaskUnreliableBaseline = 0uL;
		observerMaskUnreliableBaseline = 0uL;
		ownerMaskUnreliableDelta = 0uL;
		observerMaskUnreliableDelta = 0uL;
		NetworkBehaviour[] networkBehaviours = NetworkBehaviours;
		for (int i = 0; i < networkBehaviours.Length; i++)
		{
			NetworkBehaviour networkBehaviour = networkBehaviours[i];
			ulong num = (ulong)(1L << i);
			if (networkBehaviour.syncMethod == SyncMethod.Reliable)
			{
				bool flag = networkBehaviour.IsDirty();
				if (networkBehaviour.syncDirection == SyncDirection.ServerToClient && flag)
				{
					ownerMaskReliable |= num;
				}
				if (networkBehaviour.syncMode == SyncMode.Observers && flag)
				{
					observerMaskReliable |= num;
				}
			}
			else if (networkBehaviour.syncMethod == SyncMethod.Hybrid)
			{
				bool flag2 = networkBehaviour.IsDirty();
				if (networkBehaviour.syncDirection == SyncDirection.ServerToClient && flag2)
				{
					ownerMaskUnreliableDelta |= num;
				}
				if (networkBehaviour.syncMode == SyncMode.Observers && flag2)
				{
					observerMaskUnreliableDelta |= num;
				}
				bool flag3 = networkBehaviour.IsDirty_BitsOnly();
				if (networkBehaviour.syncDirection == SyncDirection.ServerToClient && flag3)
				{
					ownerMaskUnreliableBaseline |= num;
				}
				if (networkBehaviour.syncMode == SyncMode.Observers && flag3)
				{
					observerMaskUnreliableBaseline |= num;
				}
			}
		}
	}

	private void ClientDirtyMasks(out ulong dirtyMaskReliable, out ulong dirtyMaskUnreliableBaseline, out ulong dirtyMaskUnreliableDelta)
	{
		dirtyMaskReliable = 0uL;
		dirtyMaskUnreliableBaseline = 0uL;
		dirtyMaskUnreliableDelta = 0uL;
		NetworkBehaviour[] networkBehaviours = NetworkBehaviours;
		for (int i = 0; i < networkBehaviours.Length; i++)
		{
			NetworkBehaviour networkBehaviour = networkBehaviours[i];
			ulong num = (ulong)(1L << i);
			if (!isOwned || networkBehaviour.syncDirection != SyncDirection.ClientToServer)
			{
				continue;
			}
			if (networkBehaviour.syncMethod == SyncMethod.Reliable)
			{
				if (networkBehaviour.IsDirty())
				{
					dirtyMaskReliable |= num;
				}
			}
			else if (networkBehaviour.syncMethod == SyncMethod.Hybrid)
			{
				if (networkBehaviour.IsDirty_BitsOnly())
				{
					dirtyMaskUnreliableBaseline |= num;
				}
				if (networkBehaviour.IsDirty())
				{
					dirtyMaskUnreliableDelta |= num;
				}
			}
		}
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	internal static bool IsDirty(ulong mask, int index)
	{
		ulong num = (ulong)(1L << index);
		return (mask & num) != 0;
	}

	internal void SerializeServer_Spawn(NetworkWriter ownerWriter, NetworkWriter observersWriter)
	{
		ValidateComponents();
		NetworkBehaviour[] networkBehaviours = NetworkBehaviours;
		var (num, num2) = ServerDirtyMasks_Spawn();
		if (num != 0L)
		{
			Compression.CompressVarUInt(ownerWriter, num);
		}
		if (num2 != 0L)
		{
			Compression.CompressVarUInt(observersWriter, num2);
		}
		if ((num | num2) == 0L)
		{
			return;
		}
		for (int i = 0; i < networkBehaviours.Length; i++)
		{
			NetworkBehaviour networkBehaviour = networkBehaviours[i];
			bool flag = IsDirty(num, i);
			bool flag2 = IsDirty(num2, i);
			if (!(flag || flag2))
			{
				continue;
			}
			using NetworkWriterPooled networkWriterPooled = NetworkWriterPool.Get();
			networkBehaviour.Serialize(networkWriterPooled, initialState: true);
			ArraySegment<byte> arraySegment = networkWriterPooled.ToArraySegment();
			if (flag)
			{
				ownerWriter.WriteBytes(arraySegment.Array, arraySegment.Offset, arraySegment.Count);
			}
			if (flag2)
			{
				observersWriter.WriteBytes(arraySegment.Array, arraySegment.Offset, arraySegment.Count);
			}
		}
	}

	internal void SerializeServer_Broadcast(NetworkWriter ownerWriterReliable, NetworkWriter observersWriterReliable, NetworkWriter ownerWriterUnreliableBaseline, NetworkWriter observersWriterUnreliableBaseline, NetworkWriter ownerWriterUnreliableDelta, NetworkWriter observersWriterUnreliableDelta, bool unreliableBaseline)
	{
		ValidateComponents();
		NetworkBehaviour[] networkBehaviours = NetworkBehaviours;
		ServerDirtyMasks_Broadcast(out var ownerMaskReliable, out var observerMaskReliable, out var ownerMaskUnreliableBaseline, out var observerMaskUnreliableBaseline, out var ownerMaskUnreliableDelta, out var observerMaskUnreliableDelta);
		if (ownerMaskReliable != 0L)
		{
			Compression.CompressVarUInt(ownerWriterReliable, ownerMaskReliable);
		}
		if (observerMaskReliable != 0L)
		{
			Compression.CompressVarUInt(observersWriterReliable, observerMaskReliable);
		}
		if (ownerMaskUnreliableDelta != 0L)
		{
			Compression.CompressVarUInt(ownerWriterUnreliableDelta, ownerMaskUnreliableDelta);
		}
		if (observerMaskUnreliableDelta != 0L)
		{
			Compression.CompressVarUInt(observersWriterUnreliableDelta, observerMaskUnreliableDelta);
		}
		if (ownerMaskUnreliableBaseline != 0L)
		{
			Compression.CompressVarUInt(ownerWriterUnreliableBaseline, ownerMaskUnreliableBaseline);
		}
		if (observerMaskUnreliableBaseline != 0L)
		{
			Compression.CompressVarUInt(observersWriterUnreliableBaseline, observerMaskUnreliableBaseline);
		}
		if ((ownerMaskReliable | observerMaskReliable | ownerMaskUnreliableBaseline | observerMaskUnreliableBaseline | ownerMaskUnreliableDelta | observerMaskUnreliableDelta) == 0L)
		{
			return;
		}
		for (int i = 0; i < networkBehaviours.Length; i++)
		{
			NetworkBehaviour networkBehaviour = networkBehaviours[i];
			bool flag = IsDirty(ownerMaskReliable, i);
			bool flag2 = IsDirty(observerMaskReliable, i);
			bool flag3 = IsDirty(ownerMaskUnreliableBaseline, i);
			bool flag4 = IsDirty(observerMaskUnreliableBaseline, i);
			bool flag5 = IsDirty(ownerMaskUnreliableDelta, i);
			bool flag6 = IsDirty(observerMaskUnreliableDelta, i);
			if (flag || flag2)
			{
				using (NetworkWriterPooled networkWriterPooled = NetworkWriterPool.Get())
				{
					networkBehaviour.Serialize(networkWriterPooled, initialState: false);
					ArraySegment<byte> arraySegment = networkWriterPooled.ToArraySegment();
					if (flag)
					{
						ownerWriterReliable.WriteBytes(arraySegment.Array, arraySegment.Offset, arraySegment.Count);
					}
					if (flag2)
					{
						observersWriterReliable.WriteBytes(arraySegment.Array, arraySegment.Offset, arraySegment.Count);
					}
				}
				networkBehaviour.ClearAllDirtyBits();
			}
			if (flag5 || flag6)
			{
				using NetworkWriterPooled networkWriterPooled2 = NetworkWriterPool.Get();
				networkBehaviour.Serialize(networkWriterPooled2, initialState: false);
				ArraySegment<byte> arraySegment2 = networkWriterPooled2.ToArraySegment();
				if (flag5)
				{
					ownerWriterUnreliableDelta.WriteBytes(arraySegment2.Array, arraySegment2.Offset, arraySegment2.Count);
				}
				if (flag6)
				{
					observersWriterUnreliableDelta.WriteBytes(arraySegment2.Array, arraySegment2.Offset, arraySegment2.Count);
				}
				networkBehaviour.lastSyncTime = NetworkTime.localTime;
			}
			if (!unreliableBaseline || !(flag3 || flag4))
			{
				continue;
			}
			using (NetworkWriterPooled networkWriterPooled3 = NetworkWriterPool.Get())
			{
				networkBehaviour.Serialize(networkWriterPooled3, initialState: true);
				ArraySegment<byte> arraySegment3 = networkWriterPooled3.ToArraySegment();
				if (flag3)
				{
					ownerWriterUnreliableBaseline.WriteBytes(arraySegment3.Array, arraySegment3.Offset, arraySegment3.Count);
				}
				if (flag4)
				{
					observersWriterUnreliableBaseline.WriteBytes(arraySegment3.Array, arraySegment3.Offset, arraySegment3.Count);
				}
			}
			networkBehaviour.ClearAllDirtyBits(clearSyncTime: false);
		}
	}

	internal void SerializeClient(NetworkWriter writerReliable, NetworkWriter writerUnreliableBaseline, NetworkWriter writerUnreliableDelta, bool unreliableBaseline)
	{
		ValidateComponents();
		NetworkBehaviour[] networkBehaviours = NetworkBehaviours;
		ClientDirtyMasks(out var dirtyMaskReliable, out var dirtyMaskUnreliableBaseline, out var dirtyMaskUnreliableDelta);
		if (dirtyMaskReliable != 0L)
		{
			Compression.CompressVarUInt(writerReliable, dirtyMaskReliable);
		}
		if (dirtyMaskUnreliableDelta != 0L)
		{
			Compression.CompressVarUInt(writerUnreliableDelta, dirtyMaskUnreliableDelta);
		}
		if (dirtyMaskUnreliableBaseline != 0L)
		{
			Compression.CompressVarUInt(writerUnreliableBaseline, dirtyMaskUnreliableBaseline);
		}
		if (dirtyMaskReliable == 0L && dirtyMaskUnreliableDelta == 0L && dirtyMaskUnreliableBaseline == 0L)
		{
			return;
		}
		for (int i = 0; i < networkBehaviours.Length; i++)
		{
			NetworkBehaviour networkBehaviour = networkBehaviours[i];
			if (IsDirty(dirtyMaskReliable, i))
			{
				networkBehaviour.Serialize(writerReliable, initialState: false);
				networkBehaviour.ClearAllDirtyBits();
			}
			if (IsDirty(dirtyMaskUnreliableDelta, i))
			{
				networkBehaviour.Serialize(writerUnreliableDelta, initialState: false);
				networkBehaviour.lastSyncTime = NetworkTime.localTime;
			}
			if (unreliableBaseline && IsDirty(dirtyMaskUnreliableBaseline, i))
			{
				networkBehaviour.Serialize(writerUnreliableBaseline, initialState: true);
				networkBehaviour.ClearAllDirtyBits(clearSyncTime: false);
			}
		}
	}

	internal bool DeserializeServer(NetworkReader reader, bool initialState)
	{
		ValidateComponents();
		NetworkBehaviour[] networkBehaviours = NetworkBehaviours;
		ulong mask = Compression.DecompressVarUInt(reader);
		for (int i = 0; i < networkBehaviours.Length; i++)
		{
			if (!IsDirty(mask, i))
			{
				continue;
			}
			NetworkBehaviour networkBehaviour = networkBehaviours[i];
			if (networkBehaviour.syncDirection == SyncDirection.ClientToServer)
			{
				if (!networkBehaviour.Deserialize(reader, initialState))
				{
					return false;
				}
				networkBehaviour.SetDirty();
			}
		}
		return true;
	}

	internal void DeserializeClient(NetworkReader reader, bool initialState)
	{
		ValidateComponents();
		NetworkBehaviour[] networkBehaviours = NetworkBehaviours;
		ulong mask = Compression.DecompressVarUInt(reader);
		for (int i = 0; i < networkBehaviours.Length; i++)
		{
			if (IsDirty(mask, i))
			{
				networkBehaviours[i].Deserialize(reader, initialState);
			}
		}
	}

	internal NetworkIdentitySerialization GetServerSerializationAtTick(int tick, bool unreliableBaselineElapsed)
	{
		if (lastSerialization.tick != tick)
		{
			lastSerialization.ResetWriters();
			SerializeServer_Broadcast(lastSerialization.ownerWriterReliable, lastSerialization.observersWriterReliable, lastSerialization.ownerWriterUnreliableBaseline, lastSerialization.observersWriterUnreliableBaseline, lastSerialization.ownerWriterUnreliableDelta, lastSerialization.observersWriterUnreliableDelta, unreliableBaselineElapsed);
			lastSerialization.tick = tick;
		}
		return lastSerialization;
	}

	internal void AddObserver(NetworkConnectionToClient conn)
	{
		if (!observers.ContainsKey(conn.connectionId))
		{
			if (observers.Count == 0)
			{
				ClearAllComponentsDirtyBits();
			}
			observers[conn.connectionId] = conn;
			conn.AddToObserving(this);
		}
	}

	internal void ClearAllComponentsDirtyBits()
	{
		NetworkBehaviour[] networkBehaviours = NetworkBehaviours;
		for (int i = 0; i < networkBehaviours.Length; i++)
		{
			networkBehaviours[i].ClearAllDirtyBits();
		}
	}

	internal void RemoveObserver(NetworkConnectionToClient conn)
	{
		observers.Remove(conn.connectionId);
	}

	public bool AssignClientAuthority(NetworkConnectionToClient conn)
	{
		if (!isServer)
		{
			Debug.LogError("AssignClientAuthority can only be called on the server for spawned objects.");
			return false;
		}
		if (conn == null)
		{
			Debug.LogError($"AssignClientAuthority for {base.gameObject} owner cannot be null. Use RemoveClientAuthority() instead.");
			return false;
		}
		if (connectionToClient != null && conn != connectionToClient)
		{
			Debug.LogError($"AssignClientAuthority for {base.gameObject} already has an owner. Use RemoveClientAuthority() first.");
			return false;
		}
		SetClientOwner(conn);
		NetworkServer.SendChangeOwnerMessage(this, conn);
		NetworkIdentity.clientAuthorityCallback?.Invoke(conn, this, authorityState: true);
		if (conn.observing.Contains(this))
		{
			NetworkBehaviour[] networkBehaviours = NetworkBehaviours;
			foreach (NetworkBehaviour networkBehaviour in networkBehaviours)
			{
				if (networkBehaviour.syncMode == SyncMode.Owner)
				{
					networkBehaviour.SetDirty();
				}
			}
		}
		else
		{
			NetworkServer.RebuildObservers(this, initialize: false);
		}
		return true;
	}

	internal void SetClientOwner(NetworkConnectionToClient conn)
	{
		if (connectionToClient != null && conn != connectionToClient)
		{
			Debug.LogError($"Object {this} netId={netId} already has an owner. Use RemoveClientAuthority() first", this);
		}
		else
		{
			connectionToClient = conn;
		}
	}

	public void RemoveClientAuthority()
	{
		if (!isServer)
		{
			Debug.LogError("RemoveClientAuthority can only be called on the server for spawned objects.");
		}
		else if (connectionToClient?.identity == this)
		{
			Debug.LogError("RemoveClientAuthority cannot remove authority for a player object");
		}
		else if (connectionToClient != null)
		{
			NetworkIdentity.clientAuthorityCallback?.Invoke(connectionToClient, this, authorityState: false);
			NetworkConnectionToClient conn = connectionToClient;
			connectionToClient = null;
			NetworkServer.SendChangeOwnerMessage(this, conn);
		}
	}

	internal void ResetState()
	{
		hasSpawned = false;
		clientStarted = false;
		isClient = false;
		isServer = false;
		isOwned = false;
		NotifyAuthority();
		netId = 0u;
		connectionToServer = null;
		connectionToClient = null;
		ClearObservers();
		if (isLocalPlayer && NetworkClient.localPlayer == this)
		{
			NetworkClient.localPlayer = null;
		}
		previousLocalPlayer = null;
		isLocalPlayer = false;
	}

	internal void NotifyAuthority()
	{
		if (!hadAuthority && isOwned)
		{
			OnStartAuthority();
		}
		if (hadAuthority && !isOwned)
		{
			OnStopAuthority();
		}
		hadAuthority = isOwned;
	}

	internal void OnStartAuthority()
	{
		NetworkBehaviour[] networkBehaviours = NetworkBehaviours;
		foreach (NetworkBehaviour networkBehaviour in networkBehaviours)
		{
			try
			{
				networkBehaviour.OnStartAuthority();
			}
			catch (Exception exception)
			{
				Debug.LogException(exception, networkBehaviour);
			}
		}
	}

	public void ForceOnStopAuthority()
	{
		OnStopAuthority();
	}

	internal void OnStopAuthority()
	{
		NetworkBehaviour[] networkBehaviours = NetworkBehaviours;
		foreach (NetworkBehaviour networkBehaviour in networkBehaviours)
		{
			try
			{
				networkBehaviour.OnStopAuthority();
			}
			catch (Exception exception)
			{
				Debug.LogException(exception, networkBehaviour);
			}
		}
	}

	internal void ClearObservers()
	{
		foreach (NetworkConnectionToClient value in observers.Values)
		{
			value.RemoveFromObserving(this, isDestroyed: true);
		}
		observers.Clear();
	}
}
