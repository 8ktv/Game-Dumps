using System.Runtime.InteropServices;
using Mirror;
using Mirror.RemoteCalls;
using UnityEngine;

public class GolfTee : NetworkBehaviour
{
	private Hittable asHittable;

	[SyncVar]
	private PlayerGolfer owner;

	protected NetworkBehaviourSyncVar ___ownerNetId;

	public PlayerGolfer Owner => Networkowner;

	public PlayerGolfer Networkowner
	{
		get
		{
			return GetSyncVarNetworkBehaviour(___ownerNetId, ref owner);
		}
		[param: In]
		set
		{
			GeneratedSyncVarSetter_NetworkBehaviour(value, ref owner, 1uL, null, ref ___ownerNetId);
		}
	}

	public override void OnStartServer()
	{
		asHittable = GetComponent<Hittable>();
		asHittable.WasHitByGolfSwing += OnServerWasHitByGolfSwing;
	}

	public override void OnStopServer()
	{
		if (!BNetworkManager.IsChangingSceneOrShuttingDown)
		{
			asHittable.WasHitByGolfSwing -= OnServerWasHitByGolfSwing;
		}
	}

	public void SetOwner(PlayerGolfer owner)
	{
		if (Networkowner != null)
		{
			Debug.LogError($"Tee attempting to set an owner ({owner}), but it already has one ({Networkowner})", base.gameObject);
		}
		else
		{
			Networkowner = owner;
		}
	}

	public Vector3 GetPlayerWorldSpawnPosition(Matrix4x4 effectiveLocalToWorld)
	{
		return effectiveLocalToWorld.MultiplyPoint((0f - GameManager.GolfSettings.SwingHitBoxLocalCenter.x) * Vector3.right);
	}

	public Vector3 GetBallWorldSpawnPosition()
	{
		return base.transform.TransformPoint(GameManager.GolfSettings.TeeBallLocalSpawnHeight * Vector3.up);
	}

	private void OnServerWasHitByGolfSwing(PlayerGolfer hitter, Vector3 worldDirection, float power, bool isRocketDriver)
	{
		SpawnHitTeeInternal(worldDirection, power);
		foreach (NetworkConnectionToClient value in NetworkServer.connections.Values)
		{
			if (value != NetworkServer.localConnection)
			{
				RpcSpawnHitTee(value, worldDirection, power);
			}
		}
		NetworkServer.Destroy(base.gameObject);
	}

	[TargetRpc]
	private void RpcSpawnHitTee(NetworkConnectionToClient connection, Vector3 worldHitDirection, float hitPower)
	{
		NetworkWriterPooled writer = NetworkWriterPool.Get();
		writer.WriteVector3(worldHitDirection);
		writer.WriteFloat(hitPower);
		SendTargetRPCInternal(connection, "System.Void GolfTee::RpcSpawnHitTee(Mirror.NetworkConnectionToClient,UnityEngine.Vector3,System.Single)", -216514983, writer, 0);
		NetworkWriterPool.Return(writer);
	}

	private void SpawnHitTeeInternal(Vector3 worldHitDirection, float hitPower)
	{
		GolfTeeManager.SpawnHitTee(base.transform.position, base.transform.rotation, worldHitDirection, hitPower);
	}

	public override bool Weaved()
	{
		return true;
	}

	protected void UserCode_RpcSpawnHitTee__NetworkConnectionToClient__Vector3__Single(NetworkConnectionToClient connection, Vector3 worldHitDirection, float hitPower)
	{
		SpawnHitTeeInternal(worldHitDirection, hitPower);
	}

	protected static void InvokeUserCode_RpcSpawnHitTee__NetworkConnectionToClient__Vector3__Single(NetworkBehaviour obj, NetworkReader reader, NetworkConnectionToClient senderConnection)
	{
		if (!NetworkClient.active)
		{
			Debug.LogError("TargetRPC RpcSpawnHitTee called on server.");
		}
		else
		{
			((GolfTee)obj).UserCode_RpcSpawnHitTee__NetworkConnectionToClient__Vector3__Single(null, reader.ReadVector3(), reader.ReadFloat());
		}
	}

	static GolfTee()
	{
		RemoteProcedureCalls.RegisterRpc(typeof(GolfTee), "System.Void GolfTee::RpcSpawnHitTee(Mirror.NetworkConnectionToClient,UnityEngine.Vector3,System.Single)", InvokeUserCode_RpcSpawnHitTee__NetworkConnectionToClient__Vector3__Single);
	}

	public override void SerializeSyncVars(NetworkWriter writer, bool forceAll)
	{
		base.SerializeSyncVars(writer, forceAll);
		if (forceAll)
		{
			writer.WriteNetworkBehaviour(Networkowner);
			return;
		}
		writer.WriteVarULong(syncVarDirtyBits);
		if ((syncVarDirtyBits & 1L) != 0L)
		{
			writer.WriteNetworkBehaviour(Networkowner);
		}
	}

	public override void DeserializeSyncVars(NetworkReader reader, bool initialState)
	{
		base.DeserializeSyncVars(reader, initialState);
		if (initialState)
		{
			GeneratedSyncVarDeserialize_NetworkBehaviour(ref owner, null, reader, ref ___ownerNetId);
			return;
		}
		long num = (long)reader.ReadVarULong();
		if ((num & 1L) != 0L)
		{
			GeneratedSyncVarDeserialize_NetworkBehaviour(ref owner, null, reader, ref ___ownerNetId);
		}
	}
}
