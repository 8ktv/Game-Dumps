using System;
using Mirror;
using Mirror.RemoteCalls;
using UnityEngine;

public class VfxManager : SingletonNetworkBehaviour<VfxManager>
{
	public struct GunShotHitVfxData
	{
		public Hittable hitHittable;

		public bool hitElectromagnetShield;

		public Vector3 localHitPoint;

		public Vector3 fallbackWorldPoint;

		public GunShotHitVfxData(Hittable hitHittable, bool hitElectromagnetShield, Vector3 localHitPoint, Vector3 fallbackWorldPoint)
		{
			this.hitHittable = hitHittable;
			this.hitElectromagnetShield = hitElectromagnetShield;
			this.localHitPoint = localHitPoint;
			this.fallbackWorldPoint = fallbackWorldPoint;
		}

		public readonly Vector3 GetHitPoint()
		{
			if (!(hitHittable != null))
			{
				return fallbackWorldPoint;
			}
			return hitHittable.transform.TransformPoint(localHitPoint);
		}
	}

	private readonly AntiCheatPerPlayerRateChecker serverPlayPooledCommandRateLimiter = new AntiCheatPerPlayerRateChecker("Play pooled VFX", 0.025f, 50, 150, 0.05f, 10);

	private readonly AntiCheatPerPlayerRateChecker serverPlayDuelingPistolHitCommandRateLimiter = new AntiCheatPerPlayerRateChecker("Play dueling pistol hit", 0.25f, 5, 10, 2f);

	private readonly AntiCheatPerPlayerRateChecker serverPlayDuelingPistolMissCommandRateLimiter = new AntiCheatPerPlayerRateChecker("Play dueling pistol miss", 0.25f, 5, 10, 2f);

	private readonly AntiCheatPerPlayerRateChecker serverPlayDuelingPistolElectromagnetShieldDeflectedHitCommandRateLimiter = new AntiCheatPerPlayerRateChecker("Play dueling pistol shield deflected hit", 0.25f, 5, 10, 2f);

	private readonly AntiCheatPerPlayerRateChecker serverPlayDuelingPistolElectromagnetShieldDeflectedMissCommandRateLimiter = new AntiCheatPerPlayerRateChecker("Play dueling pistol shield deflected miss", 0.25f, 5, 10, 2f);

	private readonly AntiCheatPerPlayerRateChecker serverPlayElephantGunHitCommandRateLimiter = new AntiCheatPerPlayerRateChecker("Play elephant gun hit", 0.25f, 5, 10, 2f);

	private readonly AntiCheatPerPlayerRateChecker serverPlayElephantGunMissCommandRateLimiter = new AntiCheatPerPlayerRateChecker("Play elephant gun miss", 0.25f, 5, 10, 2f);

	private readonly AntiCheatPerPlayerRateChecker serverPlayElephantGunElectromagnetShieldDeflectedHitCommandRateLimiter = new AntiCheatPerPlayerRateChecker("Play elephant gun shield deflected hit", 0.25f, 5, 10, 2f);

	private readonly AntiCheatPerPlayerRateChecker serverPlayElephantGunElectromagnetShieldDeflectedMissCommandRateLimiter = new AntiCheatPerPlayerRateChecker("Play elephant gun shield deflected miss", 0.25f, 5, 10, 2f);

	public static void ServerPlayPooledVfxForAllClients(VfxType vfxType, Vector3 position, Quaternion rotation, Vector3 localScale = default(Vector3), uint parentNetId = 0u, bool localSpace = false, float delay = 0f, NetworkConnectionToClient connectionToSkip = null)
	{
		if (SingletonNetworkBehaviour<VfxManager>.HasInstance)
		{
			SingletonNetworkBehaviour<VfxManager>.Instance.ServerPlayPooledVfxForAllClientsInternal(vfxType, position, rotation, localScale, parentNetId, localSpace, delay, connectionToSkip);
		}
	}

	public static void ClientPlayPooledVfxForAllClients(VfxType vfxType, Vector3 position, Quaternion rotation, Vector3 localScale = default(Vector3), uint parentNetId = 0u, bool localSpace = false, float delay = 0f)
	{
		if (SingletonNetworkBehaviour<VfxManager>.HasInstance)
		{
			SingletonNetworkBehaviour<VfxManager>.Instance.ClientPlayPooledVfxForAllClientsInternal(vfxType, position, rotation, localScale, parentNetId, localSpace, delay);
		}
	}

	public static PoolableParticleSystem PlayPooledVfxLocalOnly(VfxType vfxType, Vector3 position, Quaternion rotation, Vector3 localScale = default(Vector3), uint parentNetId = 0u, bool localSpace = false, float delay = 0f, Action<PoolableParticleSystem> additionalVfxCallback = null)
	{
		if (!SingletonNetworkBehaviour<VfxManager>.HasInstance)
		{
			return null;
		}
		return SingletonNetworkBehaviour<VfxManager>.Instance.PlayPooledVfxLocalOnlyInternal(vfxType, position, rotation, localScale, parentNetId, localSpace, delay, additionalVfxCallback);
	}

	public static PoolableParticleSystem PlayPooledVfxLocalOnly(VfxType vfxType, Vector3 position, Quaternion rotation, Transform parent, Vector3 localScale = default(Vector3), bool localSpace = false, float delay = 0f, Action<PoolableParticleSystem> additionalVfxCallback = null)
	{
		if (!SingletonNetworkBehaviour<VfxManager>.HasInstance)
		{
			return null;
		}
		return SingletonNetworkBehaviour<VfxManager>.Instance.PlayPooledVfxLocalOnlyInternal(vfxType, position, rotation, localScale, parent, localSpace, delay, additionalVfxCallback);
	}

	[Server]
	private void ServerPlayPooledVfxForAllClientsInternal(VfxType vfxType, Vector3 position, Quaternion rotation, Vector3 localScale, uint parentNetId, bool localSpace, float delay, NetworkConnectionToClient connectionToSkip)
	{
		if (!NetworkServer.active)
		{
			Debug.LogWarning("[Server] function 'System.Void VfxManager::ServerPlayPooledVfxForAllClientsInternal(VfxType,UnityEngine.Vector3,UnityEngine.Quaternion,UnityEngine.Vector3,System.UInt32,System.Boolean,System.Single,Mirror.NetworkConnectionToClient)' called when server was not active");
			return;
		}
		if (connectionToSkip == null)
		{
			RpcPlayPooledVfxForAllClients(vfxType, position, rotation, localScale, parentNetId, localSpace, delay);
			return;
		}
		foreach (NetworkConnectionToClient value in NetworkServer.connections.Values)
		{
			if (value != connectionToSkip)
			{
				RpcPlayPooledVfx(value, vfxType, position, rotation, localScale, parentNetId, localSpace, delay);
			}
		}
	}

	private void ClientPlayPooledVfxForAllClientsInternal(VfxType vfxType, Vector3 position, Quaternion rotation, Vector3 localScale, uint parentNetId, bool localSpace, float delay)
	{
		if (base.isServer)
		{
			Debug.LogError("On the server, ServerPlayPooledVfxForAllClients should be called instead");
			return;
		}
		PlayPooledVfxLocalOnlyInternal(vfxType, position, rotation, localScale, parentNetId, localSpace, delay);
		CmdPlayPooledVfxForAllOtherClients(vfxType, position, rotation, localScale, parentNetId, localSpace, delay);
	}

	[Command(requiresAuthority = false)]
	private void CmdPlayPooledVfxForAllOtherClients(VfxType vfxType, Vector3 position, Quaternion rotation, Vector3 localScale, uint parentNetId, bool localSpace, float delay, NetworkConnectionToClient sender = null)
	{
		if (base.isServer && base.isClient)
		{
			UserCode_CmdPlayPooledVfxForAllOtherClients__VfxType__Vector3__Quaternion__Vector3__UInt32__Boolean__Single__NetworkConnectionToClient(vfxType, position, rotation, localScale, parentNetId, localSpace, delay, sender);
			return;
		}
		NetworkWriterPooled writer = NetworkWriterPool.Get();
		GeneratedNetworkCode._Write_VfxType(writer, vfxType);
		writer.WriteVector3(position);
		writer.WriteQuaternion(rotation);
		writer.WriteVector3(localScale);
		writer.WriteVarUInt(parentNetId);
		writer.WriteBool(localSpace);
		writer.WriteFloat(delay);
		SendCommandInternal("System.Void VfxManager::CmdPlayPooledVfxForAllOtherClients(VfxType,UnityEngine.Vector3,UnityEngine.Quaternion,UnityEngine.Vector3,System.UInt32,System.Boolean,System.Single,Mirror.NetworkConnectionToClient)", -1689113395, writer, 0, requiresAuthority: false);
		NetworkWriterPool.Return(writer);
	}

	[ClientRpc]
	private void RpcPlayPooledVfxForAllClients(VfxType vfxType, Vector3 position, Quaternion rotation, Vector3 localScale, uint parentNetId, bool localSpace, float delay)
	{
		NetworkWriterPooled writer = NetworkWriterPool.Get();
		GeneratedNetworkCode._Write_VfxType(writer, vfxType);
		writer.WriteVector3(position);
		writer.WriteQuaternion(rotation);
		writer.WriteVector3(localScale);
		writer.WriteVarUInt(parentNetId);
		writer.WriteBool(localSpace);
		writer.WriteFloat(delay);
		SendRPCInternal("System.Void VfxManager::RpcPlayPooledVfxForAllClients(VfxType,UnityEngine.Vector3,UnityEngine.Quaternion,UnityEngine.Vector3,System.UInt32,System.Boolean,System.Single)", 917151967, writer, 0, includeOwner: true);
		NetworkWriterPool.Return(writer);
	}

	[TargetRpc]
	private void RpcPlayPooledVfx(NetworkConnectionToClient connection, VfxType vfxType, Vector3 position, Quaternion rotation, Vector3 localScale, uint parentNetId, bool localSpace, float delay)
	{
		NetworkWriterPooled writer = NetworkWriterPool.Get();
		GeneratedNetworkCode._Write_VfxType(writer, vfxType);
		writer.WriteVector3(position);
		writer.WriteQuaternion(rotation);
		writer.WriteVector3(localScale);
		writer.WriteVarUInt(parentNetId);
		writer.WriteBool(localSpace);
		writer.WriteFloat(delay);
		SendTargetRPCInternal(connection, "System.Void VfxManager::RpcPlayPooledVfx(Mirror.NetworkConnectionToClient,VfxType,UnityEngine.Vector3,UnityEngine.Quaternion,UnityEngine.Vector3,System.UInt32,System.Boolean,System.Single)", 1006815818, writer, 0);
		NetworkWriterPool.Return(writer);
	}

	private PoolableParticleSystem PlayPooledVfxLocalOnlyInternal(VfxType vfxType, Vector3 position, Quaternion rotation, Vector3 localScale = default(Vector3), uint parentNetId = 0u, bool localSpace = false, float delay = 0f, Action<PoolableParticleSystem> additionalVfxCallback = null)
	{
		if (!TryGetParent(out var parent))
		{
			parent = null;
		}
		return PlayPooledVfxLocalOnlyInternal(vfxType, position, rotation, localScale, parent, localSpace, delay, additionalVfxCallback);
		bool TryGetParent(out Transform reference)
		{
			reference = null;
			if (parentNetId == 0)
			{
				return false;
			}
			NetworkIdentity value;
			if (NetworkServer.active)
			{
				if (!NetworkServer.spawned.TryGetValue(parentNetId, out value))
				{
					return false;
				}
			}
			else
			{
				if (!NetworkClient.active)
				{
					return false;
				}
				if (!NetworkClient.spawned.TryGetValue(parentNetId, out value))
				{
					return false;
				}
			}
			Transform transform = value.transform;
			reference = transform;
			return true;
		}
	}

	private PoolableParticleSystem PlayPooledVfxLocalOnlyInternal(VfxType vfxType, Vector3 position, Quaternion rotation, Vector3 localScale = default(Vector3), Transform parent = null, bool localSpace = false, float delay = 0f, Action<PoolableParticleSystem> additionalVfxCallback = null)
	{
		try
		{
			if (!VfxPersistentData.TryGetPooledVfx(vfxType, out var particleSystem))
			{
				return null;
			}
			if (parent != null)
			{
				particleSystem.transform.parent = parent;
			}
			if (localSpace)
			{
				particleSystem.transform.SetLocalPositionAndRotation(position, rotation);
			}
			else
			{
				particleSystem.transform.SetPositionAndRotation(position, rotation);
			}
			if (localScale.x <= 0f)
			{
				particleSystem.transform.localScale = Vector3.one;
			}
			else
			{
				particleSystem.transform.localScale = localScale;
			}
			HandleDefaultCallback(vfxType, particleSystem, parent);
			additionalVfxCallback?.Invoke(particleSystem);
			particleSystem.Play(delay);
			return particleSystem;
		}
		catch (Exception exception)
		{
			Debug.LogError("Exception encountered while playing pooled VFX. See next log for details", base.gameObject);
			Debug.LogException(exception, base.gameObject);
		}
		return null;
		void HandleDefaultCallback(VfxType vfxType2, PoolableParticleSystem particles, Transform transform)
		{
			if (vfxType2 == VfxType.ItemPostHitSpin)
			{
				Rigidbody component = transform.GetComponent<Rigidbody>();
				particles.GetComponent<FollowRigidbody>().SetTarget(component);
				particles.transform.parent = base.transform;
			}
		}
	}

	public static void PlayDuelingPistolHitForAllClients(PlayerInventory shootingPlayer, GunShotHitVfxData hitData)
	{
		if (SingletonNetworkBehaviour<VfxManager>.HasInstance)
		{
			SingletonNetworkBehaviour<VfxManager>.Instance.PlayDuelingPistolHitForAllClientsInternal(shootingPlayer, hitData);
		}
	}

	private void PlayDuelingPistolHitForAllClientsInternal(PlayerInventory shootingPlayer, GunShotHitVfxData hitData)
	{
		PlayDuelingPistolHitInternal(shootingPlayer, hitData);
		CmdPlayDuelingPistolHitForAllClients(shootingPlayer, hitData);
	}

	[Command(requiresAuthority = false)]
	private void CmdPlayDuelingPistolHitForAllClients(PlayerInventory shootingPlayer, GunShotHitVfxData hitData, NetworkConnectionToClient sender = null)
	{
		if (base.isServer && base.isClient)
		{
			UserCode_CmdPlayDuelingPistolHitForAllClients__PlayerInventory__GunShotHitVfxData__NetworkConnectionToClient(shootingPlayer, hitData, sender);
			return;
		}
		NetworkWriterPooled writer = NetworkWriterPool.Get();
		writer.WriteNetworkBehaviour(shootingPlayer);
		GeneratedNetworkCode._Write_VfxManager_002FGunShotHitVfxData(writer, hitData);
		SendCommandInternal("System.Void VfxManager::CmdPlayDuelingPistolHitForAllClients(PlayerInventory,VfxManager/GunShotHitVfxData,Mirror.NetworkConnectionToClient)", -1641584201, writer, 0, requiresAuthority: false);
		NetworkWriterPool.Return(writer);
	}

	[TargetRpc]
	private void RpcPlayDuelingPistolHit(NetworkConnectionToClient connection, PlayerInventory shootingPlayer, GunShotHitVfxData hitData)
	{
		NetworkWriterPooled writer = NetworkWriterPool.Get();
		writer.WriteNetworkBehaviour(shootingPlayer);
		GeneratedNetworkCode._Write_VfxManager_002FGunShotHitVfxData(writer, hitData);
		SendTargetRPCInternal(connection, "System.Void VfxManager::RpcPlayDuelingPistolHit(Mirror.NetworkConnectionToClient,PlayerInventory,VfxManager/GunShotHitVfxData)", -1635538096, writer, 0);
		NetworkWriterPool.Return(writer);
	}

	private void PlayDuelingPistolHitInternal(PlayerInventory shootingPlayer, GunShotHitVfxData hitData)
	{
		if (!VfxPersistentData.TryGetPooledVfx(VfxType.DuelingPistolMuzzle, out var particleSystem))
		{
			return;
		}
		if (!particleSystem.TryGetComponent<DuelingPistolVfx>(out var component))
		{
			particleSystem.ReturnToPool();
			return;
		}
		if (!VfxPersistentData.TryGetPooledVfx(VfxType.DuelingPistolImpact, out var particleSystem2))
		{
			particleSystem.ReturnToPool();
			return;
		}
		Vector3 duelingPistolBarrelEndPosition = shootingPlayer.GetDuelingPistolBarrelEndPosition();
		Quaternion duelingPistolBarrelEndRotation = shootingPlayer.GetDuelingPistolBarrelEndRotation();
		Vector3 hitPoint = hitData.GetHitPoint();
		particleSystem.transform.SetPositionAndRotation(duelingPistolBarrelEndPosition, duelingPistolBarrelEndRotation);
		component.SetMuzzleEffectsEnabled(enabled: true);
		component.SetPoints(duelingPistolBarrelEndPosition, hitPoint);
		particleSystem.Play();
		if (!hitData.hitElectromagnetShield)
		{
			particleSystem2.transform.SetPositionAndRotation(hitPoint, Quaternion.LookRotation(duelingPistolBarrelEndPosition - hitPoint));
			particleSystem2.Play();
		}
	}

	public static void PlayDuelingPistolMissForAllClients(PlayerInventory shootingPlayer, Vector3 direction)
	{
		if (SingletonNetworkBehaviour<VfxManager>.HasInstance)
		{
			SingletonNetworkBehaviour<VfxManager>.Instance.PlayDuelingPistolMissForAllClientsInternal(shootingPlayer, direction);
		}
	}

	private void PlayDuelingPistolMissForAllClientsInternal(PlayerInventory shootingPlayer, Vector3 direction)
	{
		PlayDuelingPistolMissInternal(shootingPlayer, direction);
		CmdPlayDuelingPistolMissForAllClients(shootingPlayer, direction);
	}

	[Command(requiresAuthority = false)]
	private void CmdPlayDuelingPistolMissForAllClients(PlayerInventory shootingPlayer, Vector3 direction, NetworkConnectionToClient sender = null)
	{
		if (base.isServer && base.isClient)
		{
			UserCode_CmdPlayDuelingPistolMissForAllClients__PlayerInventory__Vector3__NetworkConnectionToClient(shootingPlayer, direction, sender);
			return;
		}
		NetworkWriterPooled writer = NetworkWriterPool.Get();
		writer.WriteNetworkBehaviour(shootingPlayer);
		writer.WriteVector3(direction);
		SendCommandInternal("System.Void VfxManager::CmdPlayDuelingPistolMissForAllClients(PlayerInventory,UnityEngine.Vector3,Mirror.NetworkConnectionToClient)", 2121582904, writer, 0, requiresAuthority: false);
		NetworkWriterPool.Return(writer);
	}

	[TargetRpc]
	private void RpcPlayDuelingPistolMiss(NetworkConnectionToClient connection, PlayerInventory shootingPlayer, Vector3 direction)
	{
		NetworkWriterPooled writer = NetworkWriterPool.Get();
		writer.WriteNetworkBehaviour(shootingPlayer);
		writer.WriteVector3(direction);
		SendTargetRPCInternal(connection, "System.Void VfxManager::RpcPlayDuelingPistolMiss(Mirror.NetworkConnectionToClient,PlayerInventory,UnityEngine.Vector3)", 153245979, writer, 0);
		NetworkWriterPool.Return(writer);
	}

	private void PlayDuelingPistolMissInternal(PlayerInventory shootingPlayer, Vector3 direction)
	{
		if (VfxPersistentData.TryGetPooledVfx(VfxType.DuelingPistolMuzzle, out var particleSystem))
		{
			if (!particleSystem.TryGetComponent<DuelingPistolVfx>(out var component))
			{
				particleSystem.ReturnToPool();
				return;
			}
			Vector3 duelingPistolBarrelEndPosition = shootingPlayer.GetDuelingPistolBarrelEndPosition();
			Quaternion duelingPistolBarrelEndRotation = shootingPlayer.GetDuelingPistolBarrelEndRotation();
			Vector3 endPoint = duelingPistolBarrelEndPosition + direction.normalized * GameManager.ItemSettings.DuelingPistolMaxShotDistance;
			particleSystem.transform.SetPositionAndRotation(duelingPistolBarrelEndPosition, duelingPistolBarrelEndRotation);
			component.SetMuzzleEffectsEnabled(enabled: true);
			component.SetPoints(duelingPistolBarrelEndPosition, endPoint);
			particleSystem.Play();
		}
	}

	public static void PlayDuelingPistolElectromagnetShieldDeflectedHitForAllClients(PlayerInfo shieldOwner, Vector3 shieldWorldNormal, GunShotHitVfxData hitData)
	{
		if (SingletonNetworkBehaviour<VfxManager>.HasInstance)
		{
			SingletonNetworkBehaviour<VfxManager>.Instance.PlayDuelingPistolElectromagnetShieldDeflectedHitForAllClientsInternal(shieldOwner, shieldWorldNormal, hitData);
		}
	}

	private void PlayDuelingPistolElectromagnetShieldDeflectedHitForAllClientsInternal(PlayerInfo shieldOwner, Vector3 shieldWorldNormal, GunShotHitVfxData hitData)
	{
		PlayDuelingPistolElectromagnetShieldDeflectedHitInternal(shieldOwner, shieldWorldNormal, hitData);
		CmdPlayDuelingPistolElectromagnetShieldDeflectedHitForAllClients(shieldOwner, shieldWorldNormal, hitData);
	}

	[Command(requiresAuthority = false)]
	private void CmdPlayDuelingPistolElectromagnetShieldDeflectedHitForAllClients(PlayerInfo shieldOwner, Vector3 shieldWorldNormal, GunShotHitVfxData hitData, NetworkConnectionToClient sender = null)
	{
		if (base.isServer && base.isClient)
		{
			UserCode_CmdPlayDuelingPistolElectromagnetShieldDeflectedHitForAllClients__PlayerInfo__Vector3__GunShotHitVfxData__NetworkConnectionToClient(shieldOwner, shieldWorldNormal, hitData, sender);
			return;
		}
		NetworkWriterPooled writer = NetworkWriterPool.Get();
		writer.WriteNetworkBehaviour(shieldOwner);
		writer.WriteVector3(shieldWorldNormal);
		GeneratedNetworkCode._Write_VfxManager_002FGunShotHitVfxData(writer, hitData);
		SendCommandInternal("System.Void VfxManager::CmdPlayDuelingPistolElectromagnetShieldDeflectedHitForAllClients(PlayerInfo,UnityEngine.Vector3,VfxManager/GunShotHitVfxData,Mirror.NetworkConnectionToClient)", 1055904195, writer, 0, requiresAuthority: false);
		NetworkWriterPool.Return(writer);
	}

	[TargetRpc]
	private void RpcPlayDuelingPistolElectromagnetShieldDeflectedHit(NetworkConnectionToClient connection, PlayerInfo shieldOwner, Vector3 shieldWorldNormal, GunShotHitVfxData hitData)
	{
		NetworkWriterPooled writer = NetworkWriterPool.Get();
		writer.WriteNetworkBehaviour(shieldOwner);
		writer.WriteVector3(shieldWorldNormal);
		GeneratedNetworkCode._Write_VfxManager_002FGunShotHitVfxData(writer, hitData);
		SendTargetRPCInternal(connection, "System.Void VfxManager::RpcPlayDuelingPistolElectromagnetShieldDeflectedHit(Mirror.NetworkConnectionToClient,PlayerInfo,UnityEngine.Vector3,VfxManager/GunShotHitVfxData)", -53315944, writer, 0);
		NetworkWriterPool.Return(writer);
	}

	private void PlayDuelingPistolElectromagnetShieldDeflectedHitInternal(PlayerInfo shieldOwner, Vector3 shieldWorldNormal, GunShotHitVfxData hitData)
	{
		if (!VfxPersistentData.TryGetPooledVfx(VfxType.DuelingPistolMuzzle, out var particleSystem))
		{
			return;
		}
		if (!particleSystem.TryGetComponent<DuelingPistolVfx>(out var component))
		{
			particleSystem.ReturnToPool();
			return;
		}
		PoolableParticleSystem particleSystem2 = null;
		if (!hitData.hitElectromagnetShield && !VfxPersistentData.TryGetPooledVfx(VfxType.DuelingPistolImpact, out particleSystem2))
		{
			particleSystem.ReturnToPool();
			return;
		}
		Vector3 vector = shieldOwner.ElectromagnetShieldCollider.transform.position + shieldWorldNormal * shieldOwner.ElectromagnetShieldCollider.radius;
		Vector3 hitPoint = hitData.GetHitPoint();
		particleSystem.transform.position = vector;
		component.SetMuzzleEffectsEnabled(enabled: false);
		component.SetPoints(vector, hitPoint);
		particleSystem.Play();
		if (!hitData.hitElectromagnetShield)
		{
			particleSystem2.transform.SetPositionAndRotation(hitPoint, Quaternion.LookRotation(vector - hitPoint));
			particleSystem2.Play();
		}
	}

	public static void PlayDuelingPistolElectromagnetShieldDeflectedMissForAllClients(PlayerInfo shieldOwner, Vector3 shieldWorldNormal, Vector3 deflectedDirection, float distance)
	{
		if (SingletonNetworkBehaviour<VfxManager>.HasInstance)
		{
			SingletonNetworkBehaviour<VfxManager>.Instance.PlayDuelingPistolElectromagnetShieldDeflectedMissForAllClientsInternal(shieldOwner, shieldWorldNormal, deflectedDirection, distance);
		}
	}

	private void PlayDuelingPistolElectromagnetShieldDeflectedMissForAllClientsInternal(PlayerInfo shieldOwner, Vector3 shieldWorldNormal, Vector3 deflectedDirection, float distance)
	{
		PlayDuelingPistolElectromagnetShieldDeflectedMissInternal(shieldOwner, shieldWorldNormal, deflectedDirection, distance);
		CmdPlayDuelingPistolElectromagnetShieldDeflectedMissForAllClients(shieldOwner, shieldWorldNormal, deflectedDirection, distance);
	}

	[Command(requiresAuthority = false)]
	private void CmdPlayDuelingPistolElectromagnetShieldDeflectedMissForAllClients(PlayerInfo shieldOwner, Vector3 shieldWorldNormal, Vector3 deflectedDirection, float distance, NetworkConnectionToClient sender = null)
	{
		if (base.isServer && base.isClient)
		{
			UserCode_CmdPlayDuelingPistolElectromagnetShieldDeflectedMissForAllClients__PlayerInfo__Vector3__Vector3__Single__NetworkConnectionToClient(shieldOwner, shieldWorldNormal, deflectedDirection, distance, sender);
			return;
		}
		NetworkWriterPooled writer = NetworkWriterPool.Get();
		writer.WriteNetworkBehaviour(shieldOwner);
		writer.WriteVector3(shieldWorldNormal);
		writer.WriteVector3(deflectedDirection);
		writer.WriteFloat(distance);
		SendCommandInternal("System.Void VfxManager::CmdPlayDuelingPistolElectromagnetShieldDeflectedMissForAllClients(PlayerInfo,UnityEngine.Vector3,UnityEngine.Vector3,System.Single,Mirror.NetworkConnectionToClient)", 412235169, writer, 0, requiresAuthority: false);
		NetworkWriterPool.Return(writer);
	}

	[TargetRpc]
	private void RpcPlayDuelingPistolElectromagnetShieldDeflectedMiss(NetworkConnectionToClient connection, PlayerInfo shieldOwner, Vector3 shieldWorldNormal, Vector3 deflectedDirection, float distance)
	{
		NetworkWriterPooled writer = NetworkWriterPool.Get();
		writer.WriteNetworkBehaviour(shieldOwner);
		writer.WriteVector3(shieldWorldNormal);
		writer.WriteVector3(deflectedDirection);
		writer.WriteFloat(distance);
		SendTargetRPCInternal(connection, "System.Void VfxManager::RpcPlayDuelingPistolElectromagnetShieldDeflectedMiss(Mirror.NetworkConnectionToClient,PlayerInfo,UnityEngine.Vector3,UnityEngine.Vector3,System.Single)", 572850092, writer, 0);
		NetworkWriterPool.Return(writer);
	}

	private void PlayDuelingPistolElectromagnetShieldDeflectedMissInternal(PlayerInfo shieldOwner, Vector3 shieldWorldNormal, Vector3 deflectedDirection, float distance)
	{
		if (!(shieldOwner == null) && VfxPersistentData.TryGetPooledVfx(VfxType.DuelingPistolMuzzle, out var particleSystem))
		{
			if (!particleSystem.TryGetComponent<DuelingPistolVfx>(out var component))
			{
				particleSystem.ReturnToPool();
				return;
			}
			Vector3 vector = shieldOwner.ElectromagnetShieldCollider.transform.position + shieldWorldNormal * shieldOwner.ElectromagnetShieldCollider.radius;
			Vector3 endPoint = vector + deflectedDirection.normalized * distance;
			particleSystem.transform.position = vector;
			component.SetMuzzleEffectsEnabled(enabled: false);
			component.SetPoints(vector, endPoint);
			particleSystem.Play();
		}
	}

	public static void PlayElephantGunHitForAllClients(PlayerInventory shootingPlayer, GunShotHitVfxData hitData)
	{
		if (SingletonNetworkBehaviour<VfxManager>.HasInstance)
		{
			SingletonNetworkBehaviour<VfxManager>.Instance.PlayElephantGunHitForAllClientsInternal(shootingPlayer, hitData);
		}
	}

	private void PlayElephantGunHitForAllClientsInternal(PlayerInventory shootingPlayer, GunShotHitVfxData hitData)
	{
		PlayElephantGunHitInternal(shootingPlayer, hitData);
		CmdPlayElephantGunHitForAllClients(shootingPlayer, hitData);
	}

	[Command(requiresAuthority = false)]
	private void CmdPlayElephantGunHitForAllClients(PlayerInventory shootingPlayer, GunShotHitVfxData hitData, NetworkConnectionToClient sender = null)
	{
		if (base.isServer && base.isClient)
		{
			UserCode_CmdPlayElephantGunHitForAllClients__PlayerInventory__GunShotHitVfxData__NetworkConnectionToClient(shootingPlayer, hitData, sender);
			return;
		}
		NetworkWriterPooled writer = NetworkWriterPool.Get();
		writer.WriteNetworkBehaviour(shootingPlayer);
		GeneratedNetworkCode._Write_VfxManager_002FGunShotHitVfxData(writer, hitData);
		SendCommandInternal("System.Void VfxManager::CmdPlayElephantGunHitForAllClients(PlayerInventory,VfxManager/GunShotHitVfxData,Mirror.NetworkConnectionToClient)", -1917558797, writer, 0, requiresAuthority: false);
		NetworkWriterPool.Return(writer);
	}

	[TargetRpc]
	private void RpcPlayElephantGunHit(NetworkConnectionToClient connection, PlayerInventory shootingPlayer, GunShotHitVfxData hitData)
	{
		NetworkWriterPooled writer = NetworkWriterPool.Get();
		writer.WriteNetworkBehaviour(shootingPlayer);
		GeneratedNetworkCode._Write_VfxManager_002FGunShotHitVfxData(writer, hitData);
		SendTargetRPCInternal(connection, "System.Void VfxManager::RpcPlayElephantGunHit(Mirror.NetworkConnectionToClient,PlayerInventory,VfxManager/GunShotHitVfxData)", -1450949024, writer, 0);
		NetworkWriterPool.Return(writer);
	}

	private void PlayElephantGunHitInternal(PlayerInventory shootingPlayer, GunShotHitVfxData hitData)
	{
		if (!VfxPersistentData.TryGetPooledVfx(VfxType.DuelingPistolMuzzle, out var particleSystem))
		{
			return;
		}
		if (!particleSystem.TryGetComponent<DuelingPistolVfx>(out var component))
		{
			particleSystem.ReturnToPool();
			return;
		}
		if (!VfxPersistentData.TryGetPooledVfx(VfxType.DuelingPistolImpact, out var particleSystem2))
		{
			particleSystem.ReturnToPool();
			return;
		}
		Vector3 elephantGunBarrelEndPosition = shootingPlayer.GetElephantGunBarrelEndPosition();
		Quaternion elephantGunBarrelEndRotation = shootingPlayer.GetElephantGunBarrelEndRotation();
		Vector3 hitPoint = hitData.GetHitPoint();
		particleSystem.transform.SetPositionAndRotation(elephantGunBarrelEndPosition, elephantGunBarrelEndRotation);
		component.SetMuzzleEffectsEnabled(enabled: true);
		component.SetPoints(elephantGunBarrelEndPosition, hitPoint);
		particleSystem.Play();
		if (!hitData.hitElectromagnetShield)
		{
			particleSystem2.transform.SetPositionAndRotation(hitPoint, Quaternion.LookRotation(elephantGunBarrelEndPosition - hitPoint));
			particleSystem2.Play();
		}
	}

	public static void PlayElephantGunMissForAllClients(PlayerInventory shootingPlayer, Vector3 direction)
	{
		if (SingletonNetworkBehaviour<VfxManager>.HasInstance)
		{
			SingletonNetworkBehaviour<VfxManager>.Instance.PlayElephantGunMissForAllClientsInternal(shootingPlayer, direction);
		}
	}

	private void PlayElephantGunMissForAllClientsInternal(PlayerInventory shootingPlayer, Vector3 direction)
	{
		PlayElephantGunMissInternal(shootingPlayer, direction);
		CmdPlayElephantGunMissForAllClients(shootingPlayer, direction);
	}

	[Command(requiresAuthority = false)]
	private void CmdPlayElephantGunMissForAllClients(PlayerInventory shootingPlayer, Vector3 direction, NetworkConnectionToClient sender = null)
	{
		if (base.isServer && base.isClient)
		{
			UserCode_CmdPlayElephantGunMissForAllClients__PlayerInventory__Vector3__NetworkConnectionToClient(shootingPlayer, direction, sender);
			return;
		}
		NetworkWriterPooled writer = NetworkWriterPool.Get();
		writer.WriteNetworkBehaviour(shootingPlayer);
		writer.WriteVector3(direction);
		SendCommandInternal("System.Void VfxManager::CmdPlayElephantGunMissForAllClients(PlayerInventory,UnityEngine.Vector3,Mirror.NetworkConnectionToClient)", 286496676, writer, 0, requiresAuthority: false);
		NetworkWriterPool.Return(writer);
	}

	[TargetRpc]
	private void RpcPlayElephantGunMiss(NetworkConnectionToClient connection, PlayerInventory shootingPlayer, Vector3 direction)
	{
		NetworkWriterPooled writer = NetworkWriterPool.Get();
		writer.WriteNetworkBehaviour(shootingPlayer);
		writer.WriteVector3(direction);
		SendTargetRPCInternal(connection, "System.Void VfxManager::RpcPlayElephantGunMiss(Mirror.NetworkConnectionToClient,PlayerInventory,UnityEngine.Vector3)", -1655858581, writer, 0);
		NetworkWriterPool.Return(writer);
	}

	private void PlayElephantGunMissInternal(PlayerInventory shootingPlayer, Vector3 direction)
	{
		if (VfxPersistentData.TryGetPooledVfx(VfxType.DuelingPistolMuzzle, out var particleSystem))
		{
			if (!particleSystem.TryGetComponent<DuelingPistolVfx>(out var component))
			{
				particleSystem.ReturnToPool();
				return;
			}
			Vector3 elephantGunBarrelEndPosition = shootingPlayer.GetElephantGunBarrelEndPosition();
			Quaternion elephantGunBarrelEndRotation = shootingPlayer.GetElephantGunBarrelEndRotation();
			Vector3 endPoint = elephantGunBarrelEndPosition + direction.normalized * GameManager.ItemSettings.ElephantGunMaxShotDistance;
			particleSystem.transform.SetPositionAndRotation(elephantGunBarrelEndPosition, elephantGunBarrelEndRotation);
			component.SetMuzzleEffectsEnabled(enabled: true);
			component.SetPoints(elephantGunBarrelEndPosition, endPoint);
			particleSystem.Play();
		}
	}

	public static void PlayElephantGunElectromagnetShieldDeflectedHitForAllClients(PlayerInfo shieldOwner, Vector3 shieldWorldNormal, GunShotHitVfxData hitData)
	{
		if (SingletonNetworkBehaviour<VfxManager>.HasInstance)
		{
			SingletonNetworkBehaviour<VfxManager>.Instance.PlayElephantGunElectromagnetShieldDeflectedHitForAllClientsInternal(shieldOwner, shieldWorldNormal, hitData);
		}
	}

	private void PlayElephantGunElectromagnetShieldDeflectedHitForAllClientsInternal(PlayerInfo shieldOwner, Vector3 shieldWorldNormal, GunShotHitVfxData hitData)
	{
		PlayElephantGunElectromagnetShieldDeflectedHitInternal(shieldOwner, shieldWorldNormal, hitData);
		CmdPlayElephantGunElectromagnetShieldDeflectedHitForAllClients(shieldOwner, shieldWorldNormal, hitData);
	}

	[Command(requiresAuthority = false)]
	private void CmdPlayElephantGunElectromagnetShieldDeflectedHitForAllClients(PlayerInfo shieldOwner, Vector3 shieldWorldNormal, GunShotHitVfxData hitData, NetworkConnectionToClient sender = null)
	{
		if (base.isServer && base.isClient)
		{
			UserCode_CmdPlayElephantGunElectromagnetShieldDeflectedHitForAllClients__PlayerInfo__Vector3__GunShotHitVfxData__NetworkConnectionToClient(shieldOwner, shieldWorldNormal, hitData, sender);
			return;
		}
		NetworkWriterPooled writer = NetworkWriterPool.Get();
		writer.WriteNetworkBehaviour(shieldOwner);
		writer.WriteVector3(shieldWorldNormal);
		GeneratedNetworkCode._Write_VfxManager_002FGunShotHitVfxData(writer, hitData);
		SendCommandInternal("System.Void VfxManager::CmdPlayElephantGunElectromagnetShieldDeflectedHitForAllClients(PlayerInfo,UnityEngine.Vector3,VfxManager/GunShotHitVfxData,Mirror.NetworkConnectionToClient)", 162430343, writer, 0, requiresAuthority: false);
		NetworkWriterPool.Return(writer);
	}

	[TargetRpc]
	private void RpcPlayElephantGunElectromagnetShieldDeflectedHit(NetworkConnectionToClient connection, PlayerInfo shieldOwner, Vector3 shieldWorldNormal, GunShotHitVfxData hitData)
	{
		NetworkWriterPooled writer = NetworkWriterPool.Get();
		writer.WriteNetworkBehaviour(shieldOwner);
		writer.WriteVector3(shieldWorldNormal);
		GeneratedNetworkCode._Write_VfxManager_002FGunShotHitVfxData(writer, hitData);
		SendTargetRPCInternal(connection, "System.Void VfxManager::RpcPlayElephantGunElectromagnetShieldDeflectedHit(Mirror.NetworkConnectionToClient,PlayerInfo,UnityEngine.Vector3,VfxManager/GunShotHitVfxData)", -714195064, writer, 0);
		NetworkWriterPool.Return(writer);
	}

	private void PlayElephantGunElectromagnetShieldDeflectedHitInternal(PlayerInfo shieldOwner, Vector3 shieldWorldNormal, GunShotHitVfxData hitData)
	{
		if (!VfxPersistentData.TryGetPooledVfx(VfxType.DuelingPistolMuzzle, out var particleSystem))
		{
			return;
		}
		if (!particleSystem.TryGetComponent<DuelingPistolVfx>(out var component))
		{
			particleSystem.ReturnToPool();
			return;
		}
		PoolableParticleSystem particleSystem2 = null;
		if (!hitData.hitElectromagnetShield && !VfxPersistentData.TryGetPooledVfx(VfxType.DuelingPistolImpact, out particleSystem2))
		{
			particleSystem.ReturnToPool();
			return;
		}
		Vector3 vector = shieldOwner.ElectromagnetShieldCollider.transform.position + shieldWorldNormal * shieldOwner.ElectromagnetShieldCollider.radius;
		Vector3 hitPoint = hitData.GetHitPoint();
		particleSystem.transform.position = vector;
		component.SetMuzzleEffectsEnabled(enabled: false);
		component.SetPoints(vector, hitPoint);
		particleSystem.Play();
		if (!hitData.hitElectromagnetShield)
		{
			particleSystem2.transform.SetPositionAndRotation(hitPoint, Quaternion.LookRotation(vector - hitPoint));
			particleSystem2.Play();
		}
	}

	public static void PlayElephantGunElectromagnetShieldDeflectedMissForAllClients(PlayerInfo shieldOwner, Vector3 shieldWorldNormal, Vector3 deflectedDirection, float distance)
	{
		if (SingletonNetworkBehaviour<VfxManager>.HasInstance)
		{
			SingletonNetworkBehaviour<VfxManager>.Instance.PlayElephantGunElectromagnetShieldDeflectedMissForAllClientsInternal(shieldOwner, shieldWorldNormal, deflectedDirection, distance);
		}
	}

	private void PlayElephantGunElectromagnetShieldDeflectedMissForAllClientsInternal(PlayerInfo shieldOwner, Vector3 shieldWorldNormal, Vector3 deflectedDirection, float distance)
	{
		PlayElephantGunElectromagnetShieldDeflectedMissInternal(shieldOwner, shieldWorldNormal, deflectedDirection, distance);
		CmdPlayElephantGunElectromagnetShieldDeflectedMissForAllClients(shieldOwner, shieldWorldNormal, deflectedDirection, distance);
	}

	[Command(requiresAuthority = false)]
	private void CmdPlayElephantGunElectromagnetShieldDeflectedMissForAllClients(PlayerInfo shieldOwner, Vector3 shieldWorldNormal, Vector3 deflectedDirection, float distance, NetworkConnectionToClient sender = null)
	{
		if (base.isServer && base.isClient)
		{
			UserCode_CmdPlayElephantGunElectromagnetShieldDeflectedMissForAllClients__PlayerInfo__Vector3__Vector3__Single__NetworkConnectionToClient(shieldOwner, shieldWorldNormal, deflectedDirection, distance, sender);
			return;
		}
		NetworkWriterPooled writer = NetworkWriterPool.Get();
		writer.WriteNetworkBehaviour(shieldOwner);
		writer.WriteVector3(shieldWorldNormal);
		writer.WriteVector3(deflectedDirection);
		writer.WriteFloat(distance);
		SendCommandInternal("System.Void VfxManager::CmdPlayElephantGunElectromagnetShieldDeflectedMissForAllClients(PlayerInfo,UnityEngine.Vector3,UnityEngine.Vector3,System.Single,Mirror.NetworkConnectionToClient)", -92307107, writer, 0, requiresAuthority: false);
		NetworkWriterPool.Return(writer);
	}

	[TargetRpc]
	private void RpcPlayElephantGunElectromagnetShieldDeflectedMiss(NetworkConnectionToClient connection, PlayerInfo shieldOwner, Vector3 shieldWorldNormal, Vector3 deflectedDirection, float distance)
	{
		NetworkWriterPooled writer = NetworkWriterPool.Get();
		writer.WriteNetworkBehaviour(shieldOwner);
		writer.WriteVector3(shieldWorldNormal);
		writer.WriteVector3(deflectedDirection);
		writer.WriteFloat(distance);
		SendTargetRPCInternal(connection, "System.Void VfxManager::RpcPlayElephantGunElectromagnetShieldDeflectedMiss(Mirror.NetworkConnectionToClient,PlayerInfo,UnityEngine.Vector3,UnityEngine.Vector3,System.Single)", 261217020, writer, 0);
		NetworkWriterPool.Return(writer);
	}

	private void PlayElephantGunElectromagnetShieldDeflectedMissInternal(PlayerInfo shieldOwner, Vector3 shieldWorldNormal, Vector3 deflectedDirection, float distance)
	{
		if (!(shieldOwner == null) && VfxPersistentData.TryGetPooledVfx(VfxType.DuelingPistolMuzzle, out var particleSystem))
		{
			if (!particleSystem.TryGetComponent<DuelingPistolVfx>(out var component))
			{
				particleSystem.ReturnToPool();
				return;
			}
			Vector3 vector = shieldOwner.ElectromagnetShieldCollider.transform.position + shieldWorldNormal * shieldOwner.ElectromagnetShieldCollider.radius;
			Vector3 endPoint = vector + deflectedDirection.normalized * distance;
			particleSystem.transform.position = vector;
			component.SetMuzzleEffectsEnabled(enabled: false);
			component.SetPoints(vector, endPoint);
			particleSystem.Play();
		}
	}

	public static void PlayRocketLaunchLocalOnly(PlayerInventory shootingPlayer)
	{
		Quaternion rocketLauncherRocketRotation = shootingPlayer.GetRocketLauncherRocketRotation();
		PlayRocketLaunchLocalOnlyInternal(shootingPlayer, rocketLauncherRocketRotation);
	}

	public static void PlayRocketLaunchLocalOnly(PlayerInventory shootingPlayer, Quaternion forcedMuzzleRotation)
	{
		PlayRocketLaunchLocalOnlyInternal(shootingPlayer, forcedMuzzleRotation);
	}

	private static void PlayRocketLaunchLocalOnlyInternal(PlayerInventory shootingPlayer, Quaternion muzzleRotation)
	{
		if (VfxPersistentData.TryGetPooledVfx(VfxType.RocketLauncherMuzzle, out var particleSystem))
		{
			if (!VfxPersistentData.TryGetPooledVfx(VfxType.RocketLauncherBackBlast, out var particleSystem2))
			{
				particleSystem.ReturnToPool();
				return;
			}
			Vector3 rocketLauncherBarrelFrontEndPosition = shootingPlayer.GetRocketLauncherBarrelFrontEndPosition();
			Vector3 rocketLauncherBarrelBackEndPosition = shootingPlayer.GetRocketLauncherBarrelBackEndPosition();
			Quaternion rotation = Quaternion.AngleAxis(180f, shootingPlayer.transform.up) * muzzleRotation;
			particleSystem.transform.SetPositionAndRotation(rocketLauncherBarrelFrontEndPosition, muzzleRotation);
			particleSystem2.transform.SetPositionAndRotation(rocketLauncherBarrelBackEndPosition, rotation);
			particleSystem.Play();
			particleSystem2.Play();
		}
	}

	public override bool Weaved()
	{
		return true;
	}

	protected void UserCode_CmdPlayPooledVfxForAllOtherClients__VfxType__Vector3__Quaternion__Vector3__UInt32__Boolean__Single__NetworkConnectionToClient(VfxType vfxType, Vector3 position, Quaternion rotation, Vector3 localScale, uint parentNetId, bool localSpace, float delay, NetworkConnectionToClient sender)
	{
		if (serverPlayPooledCommandRateLimiter.RegisterHit(sender))
		{
			SingletonNetworkBehaviour<VfxManager>.Instance.ServerPlayPooledVfxForAllClientsInternal(vfxType, position, rotation, localScale, parentNetId, localSpace, delay, sender);
		}
	}

	protected static void InvokeUserCode_CmdPlayPooledVfxForAllOtherClients__VfxType__Vector3__Quaternion__Vector3__UInt32__Boolean__Single__NetworkConnectionToClient(NetworkBehaviour obj, NetworkReader reader, NetworkConnectionToClient senderConnection)
	{
		if (!NetworkServer.active)
		{
			Debug.LogError("Command CmdPlayPooledVfxForAllOtherClients called on client.");
		}
		else
		{
			((VfxManager)obj).UserCode_CmdPlayPooledVfxForAllOtherClients__VfxType__Vector3__Quaternion__Vector3__UInt32__Boolean__Single__NetworkConnectionToClient(GeneratedNetworkCode._Read_VfxType(reader), reader.ReadVector3(), reader.ReadQuaternion(), reader.ReadVector3(), reader.ReadVarUInt(), reader.ReadBool(), reader.ReadFloat(), senderConnection);
		}
	}

	protected void UserCode_RpcPlayPooledVfxForAllClients__VfxType__Vector3__Quaternion__Vector3__UInt32__Boolean__Single(VfxType vfxType, Vector3 position, Quaternion rotation, Vector3 localScale, uint parentNetId, bool localSpace, float delay)
	{
		PlayPooledVfxLocalOnlyInternal(vfxType, position, rotation, localScale, parentNetId, localSpace, delay);
	}

	protected static void InvokeUserCode_RpcPlayPooledVfxForAllClients__VfxType__Vector3__Quaternion__Vector3__UInt32__Boolean__Single(NetworkBehaviour obj, NetworkReader reader, NetworkConnectionToClient senderConnection)
	{
		if (!NetworkClient.active)
		{
			Debug.LogError("RPC RpcPlayPooledVfxForAllClients called on server.");
		}
		else
		{
			((VfxManager)obj).UserCode_RpcPlayPooledVfxForAllClients__VfxType__Vector3__Quaternion__Vector3__UInt32__Boolean__Single(GeneratedNetworkCode._Read_VfxType(reader), reader.ReadVector3(), reader.ReadQuaternion(), reader.ReadVector3(), reader.ReadVarUInt(), reader.ReadBool(), reader.ReadFloat());
		}
	}

	protected void UserCode_RpcPlayPooledVfx__NetworkConnectionToClient__VfxType__Vector3__Quaternion__Vector3__UInt32__Boolean__Single(NetworkConnectionToClient connection, VfxType vfxType, Vector3 position, Quaternion rotation, Vector3 localScale, uint parentNetId, bool localSpace, float delay)
	{
		PlayPooledVfxLocalOnlyInternal(vfxType, position, rotation, localScale, parentNetId, localSpace, delay);
	}

	protected static void InvokeUserCode_RpcPlayPooledVfx__NetworkConnectionToClient__VfxType__Vector3__Quaternion__Vector3__UInt32__Boolean__Single(NetworkBehaviour obj, NetworkReader reader, NetworkConnectionToClient senderConnection)
	{
		if (!NetworkClient.active)
		{
			Debug.LogError("TargetRPC RpcPlayPooledVfx called on server.");
		}
		else
		{
			((VfxManager)obj).UserCode_RpcPlayPooledVfx__NetworkConnectionToClient__VfxType__Vector3__Quaternion__Vector3__UInt32__Boolean__Single(null, GeneratedNetworkCode._Read_VfxType(reader), reader.ReadVector3(), reader.ReadQuaternion(), reader.ReadVector3(), reader.ReadVarUInt(), reader.ReadBool(), reader.ReadFloat());
		}
	}

	protected void UserCode_CmdPlayDuelingPistolHitForAllClients__PlayerInventory__GunShotHitVfxData__NetworkConnectionToClient(PlayerInventory shootingPlayer, GunShotHitVfxData hitData, NetworkConnectionToClient sender)
	{
		if (!serverPlayDuelingPistolHitCommandRateLimiter.RegisterHit(sender))
		{
			return;
		}
		if (sender != null && sender != NetworkServer.localConnection)
		{
			PlayDuelingPistolHitInternal(shootingPlayer, hitData);
		}
		foreach (NetworkConnectionToClient value in NetworkServer.connections.Values)
		{
			if (value != NetworkServer.localConnection && value != sender)
			{
				RpcPlayDuelingPistolHit(value, shootingPlayer, hitData);
			}
		}
	}

	protected static void InvokeUserCode_CmdPlayDuelingPistolHitForAllClients__PlayerInventory__GunShotHitVfxData__NetworkConnectionToClient(NetworkBehaviour obj, NetworkReader reader, NetworkConnectionToClient senderConnection)
	{
		if (!NetworkServer.active)
		{
			Debug.LogError("Command CmdPlayDuelingPistolHitForAllClients called on client.");
		}
		else
		{
			((VfxManager)obj).UserCode_CmdPlayDuelingPistolHitForAllClients__PlayerInventory__GunShotHitVfxData__NetworkConnectionToClient(reader.ReadNetworkBehaviour<PlayerInventory>(), GeneratedNetworkCode._Read_VfxManager_002FGunShotHitVfxData(reader), senderConnection);
		}
	}

	protected void UserCode_RpcPlayDuelingPistolHit__NetworkConnectionToClient__PlayerInventory__GunShotHitVfxData(NetworkConnectionToClient connection, PlayerInventory shootingPlayer, GunShotHitVfxData hitData)
	{
		PlayDuelingPistolHitInternal(shootingPlayer, hitData);
	}

	protected static void InvokeUserCode_RpcPlayDuelingPistolHit__NetworkConnectionToClient__PlayerInventory__GunShotHitVfxData(NetworkBehaviour obj, NetworkReader reader, NetworkConnectionToClient senderConnection)
	{
		if (!NetworkClient.active)
		{
			Debug.LogError("TargetRPC RpcPlayDuelingPistolHit called on server.");
		}
		else
		{
			((VfxManager)obj).UserCode_RpcPlayDuelingPistolHit__NetworkConnectionToClient__PlayerInventory__GunShotHitVfxData(null, reader.ReadNetworkBehaviour<PlayerInventory>(), GeneratedNetworkCode._Read_VfxManager_002FGunShotHitVfxData(reader));
		}
	}

	protected void UserCode_CmdPlayDuelingPistolMissForAllClients__PlayerInventory__Vector3__NetworkConnectionToClient(PlayerInventory shootingPlayer, Vector3 direction, NetworkConnectionToClient sender)
	{
		if (!serverPlayDuelingPistolMissCommandRateLimiter.RegisterHit(sender))
		{
			return;
		}
		if (sender != null && sender != NetworkServer.localConnection)
		{
			PlayDuelingPistolMissInternal(shootingPlayer, direction);
		}
		foreach (NetworkConnectionToClient value in NetworkServer.connections.Values)
		{
			if (value != NetworkServer.localConnection && value != sender)
			{
				RpcPlayDuelingPistolMiss(value, shootingPlayer, direction);
			}
		}
	}

	protected static void InvokeUserCode_CmdPlayDuelingPistolMissForAllClients__PlayerInventory__Vector3__NetworkConnectionToClient(NetworkBehaviour obj, NetworkReader reader, NetworkConnectionToClient senderConnection)
	{
		if (!NetworkServer.active)
		{
			Debug.LogError("Command CmdPlayDuelingPistolMissForAllClients called on client.");
		}
		else
		{
			((VfxManager)obj).UserCode_CmdPlayDuelingPistolMissForAllClients__PlayerInventory__Vector3__NetworkConnectionToClient(reader.ReadNetworkBehaviour<PlayerInventory>(), reader.ReadVector3(), senderConnection);
		}
	}

	protected void UserCode_RpcPlayDuelingPistolMiss__NetworkConnectionToClient__PlayerInventory__Vector3(NetworkConnectionToClient connection, PlayerInventory shootingPlayer, Vector3 direction)
	{
		PlayDuelingPistolMissInternal(shootingPlayer, direction);
	}

	protected static void InvokeUserCode_RpcPlayDuelingPistolMiss__NetworkConnectionToClient__PlayerInventory__Vector3(NetworkBehaviour obj, NetworkReader reader, NetworkConnectionToClient senderConnection)
	{
		if (!NetworkClient.active)
		{
			Debug.LogError("TargetRPC RpcPlayDuelingPistolMiss called on server.");
		}
		else
		{
			((VfxManager)obj).UserCode_RpcPlayDuelingPistolMiss__NetworkConnectionToClient__PlayerInventory__Vector3(null, reader.ReadNetworkBehaviour<PlayerInventory>(), reader.ReadVector3());
		}
	}

	protected void UserCode_CmdPlayDuelingPistolElectromagnetShieldDeflectedHitForAllClients__PlayerInfo__Vector3__GunShotHitVfxData__NetworkConnectionToClient(PlayerInfo shieldOwner, Vector3 shieldWorldNormal, GunShotHitVfxData hitData, NetworkConnectionToClient sender)
	{
		if (!serverPlayDuelingPistolElectromagnetShieldDeflectedHitCommandRateLimiter.RegisterHit(sender))
		{
			return;
		}
		if (sender != null && sender != NetworkServer.localConnection)
		{
			PlayDuelingPistolElectromagnetShieldDeflectedHitInternal(shieldOwner, shieldWorldNormal, hitData);
		}
		foreach (NetworkConnectionToClient value in NetworkServer.connections.Values)
		{
			if (value != NetworkServer.localConnection && value != sender)
			{
				RpcPlayDuelingPistolElectromagnetShieldDeflectedHit(value, shieldOwner, shieldWorldNormal, hitData);
			}
		}
	}

	protected static void InvokeUserCode_CmdPlayDuelingPistolElectromagnetShieldDeflectedHitForAllClients__PlayerInfo__Vector3__GunShotHitVfxData__NetworkConnectionToClient(NetworkBehaviour obj, NetworkReader reader, NetworkConnectionToClient senderConnection)
	{
		if (!NetworkServer.active)
		{
			Debug.LogError("Command CmdPlayDuelingPistolElectromagnetShieldDeflectedHitForAllClients called on client.");
		}
		else
		{
			((VfxManager)obj).UserCode_CmdPlayDuelingPistolElectromagnetShieldDeflectedHitForAllClients__PlayerInfo__Vector3__GunShotHitVfxData__NetworkConnectionToClient(reader.ReadNetworkBehaviour<PlayerInfo>(), reader.ReadVector3(), GeneratedNetworkCode._Read_VfxManager_002FGunShotHitVfxData(reader), senderConnection);
		}
	}

	protected void UserCode_RpcPlayDuelingPistolElectromagnetShieldDeflectedHit__NetworkConnectionToClient__PlayerInfo__Vector3__GunShotHitVfxData(NetworkConnectionToClient connection, PlayerInfo shieldOwner, Vector3 shieldWorldNormal, GunShotHitVfxData hitData)
	{
		PlayDuelingPistolElectromagnetShieldDeflectedHitInternal(shieldOwner, shieldWorldNormal, hitData);
	}

	protected static void InvokeUserCode_RpcPlayDuelingPistolElectromagnetShieldDeflectedHit__NetworkConnectionToClient__PlayerInfo__Vector3__GunShotHitVfxData(NetworkBehaviour obj, NetworkReader reader, NetworkConnectionToClient senderConnection)
	{
		if (!NetworkClient.active)
		{
			Debug.LogError("TargetRPC RpcPlayDuelingPistolElectromagnetShieldDeflectedHit called on server.");
		}
		else
		{
			((VfxManager)obj).UserCode_RpcPlayDuelingPistolElectromagnetShieldDeflectedHit__NetworkConnectionToClient__PlayerInfo__Vector3__GunShotHitVfxData(null, reader.ReadNetworkBehaviour<PlayerInfo>(), reader.ReadVector3(), GeneratedNetworkCode._Read_VfxManager_002FGunShotHitVfxData(reader));
		}
	}

	protected void UserCode_CmdPlayDuelingPistolElectromagnetShieldDeflectedMissForAllClients__PlayerInfo__Vector3__Vector3__Single__NetworkConnectionToClient(PlayerInfo shieldOwner, Vector3 shieldWorldNormal, Vector3 deflectedDirection, float distance, NetworkConnectionToClient sender)
	{
		if (!serverPlayDuelingPistolElectromagnetShieldDeflectedMissCommandRateLimiter.RegisterHit(sender))
		{
			return;
		}
		if (sender != null && sender != NetworkServer.localConnection)
		{
			PlayDuelingPistolElectromagnetShieldDeflectedMissInternal(shieldOwner, shieldWorldNormal, deflectedDirection, distance);
		}
		foreach (NetworkConnectionToClient value in NetworkServer.connections.Values)
		{
			if (value != NetworkServer.localConnection && value != sender)
			{
				RpcPlayDuelingPistolElectromagnetShieldDeflectedMiss(value, shieldOwner, shieldWorldNormal, deflectedDirection, distance);
			}
		}
	}

	protected static void InvokeUserCode_CmdPlayDuelingPistolElectromagnetShieldDeflectedMissForAllClients__PlayerInfo__Vector3__Vector3__Single__NetworkConnectionToClient(NetworkBehaviour obj, NetworkReader reader, NetworkConnectionToClient senderConnection)
	{
		if (!NetworkServer.active)
		{
			Debug.LogError("Command CmdPlayDuelingPistolElectromagnetShieldDeflectedMissForAllClients called on client.");
		}
		else
		{
			((VfxManager)obj).UserCode_CmdPlayDuelingPistolElectromagnetShieldDeflectedMissForAllClients__PlayerInfo__Vector3__Vector3__Single__NetworkConnectionToClient(reader.ReadNetworkBehaviour<PlayerInfo>(), reader.ReadVector3(), reader.ReadVector3(), reader.ReadFloat(), senderConnection);
		}
	}

	protected void UserCode_RpcPlayDuelingPistolElectromagnetShieldDeflectedMiss__NetworkConnectionToClient__PlayerInfo__Vector3__Vector3__Single(NetworkConnectionToClient connection, PlayerInfo shieldOwner, Vector3 shieldWorldNormal, Vector3 deflectedDirection, float distance)
	{
		PlayDuelingPistolElectromagnetShieldDeflectedMissInternal(shieldOwner, shieldWorldNormal, deflectedDirection, distance);
	}

	protected static void InvokeUserCode_RpcPlayDuelingPistolElectromagnetShieldDeflectedMiss__NetworkConnectionToClient__PlayerInfo__Vector3__Vector3__Single(NetworkBehaviour obj, NetworkReader reader, NetworkConnectionToClient senderConnection)
	{
		if (!NetworkClient.active)
		{
			Debug.LogError("TargetRPC RpcPlayDuelingPistolElectromagnetShieldDeflectedMiss called on server.");
		}
		else
		{
			((VfxManager)obj).UserCode_RpcPlayDuelingPistolElectromagnetShieldDeflectedMiss__NetworkConnectionToClient__PlayerInfo__Vector3__Vector3__Single(null, reader.ReadNetworkBehaviour<PlayerInfo>(), reader.ReadVector3(), reader.ReadVector3(), reader.ReadFloat());
		}
	}

	protected void UserCode_CmdPlayElephantGunHitForAllClients__PlayerInventory__GunShotHitVfxData__NetworkConnectionToClient(PlayerInventory shootingPlayer, GunShotHitVfxData hitData, NetworkConnectionToClient sender)
	{
		if (!serverPlayElephantGunHitCommandRateLimiter.RegisterHit(sender))
		{
			return;
		}
		if (sender != null && sender != NetworkServer.localConnection)
		{
			PlayElephantGunHitInternal(shootingPlayer, hitData);
		}
		foreach (NetworkConnectionToClient value in NetworkServer.connections.Values)
		{
			if (value != NetworkServer.localConnection && value != sender)
			{
				RpcPlayElephantGunHit(value, shootingPlayer, hitData);
			}
		}
	}

	protected static void InvokeUserCode_CmdPlayElephantGunHitForAllClients__PlayerInventory__GunShotHitVfxData__NetworkConnectionToClient(NetworkBehaviour obj, NetworkReader reader, NetworkConnectionToClient senderConnection)
	{
		if (!NetworkServer.active)
		{
			Debug.LogError("Command CmdPlayElephantGunHitForAllClients called on client.");
		}
		else
		{
			((VfxManager)obj).UserCode_CmdPlayElephantGunHitForAllClients__PlayerInventory__GunShotHitVfxData__NetworkConnectionToClient(reader.ReadNetworkBehaviour<PlayerInventory>(), GeneratedNetworkCode._Read_VfxManager_002FGunShotHitVfxData(reader), senderConnection);
		}
	}

	protected void UserCode_RpcPlayElephantGunHit__NetworkConnectionToClient__PlayerInventory__GunShotHitVfxData(NetworkConnectionToClient connection, PlayerInventory shootingPlayer, GunShotHitVfxData hitData)
	{
		PlayElephantGunHitInternal(shootingPlayer, hitData);
	}

	protected static void InvokeUserCode_RpcPlayElephantGunHit__NetworkConnectionToClient__PlayerInventory__GunShotHitVfxData(NetworkBehaviour obj, NetworkReader reader, NetworkConnectionToClient senderConnection)
	{
		if (!NetworkClient.active)
		{
			Debug.LogError("TargetRPC RpcPlayElephantGunHit called on server.");
		}
		else
		{
			((VfxManager)obj).UserCode_RpcPlayElephantGunHit__NetworkConnectionToClient__PlayerInventory__GunShotHitVfxData(null, reader.ReadNetworkBehaviour<PlayerInventory>(), GeneratedNetworkCode._Read_VfxManager_002FGunShotHitVfxData(reader));
		}
	}

	protected void UserCode_CmdPlayElephantGunMissForAllClients__PlayerInventory__Vector3__NetworkConnectionToClient(PlayerInventory shootingPlayer, Vector3 direction, NetworkConnectionToClient sender)
	{
		if (!serverPlayElephantGunMissCommandRateLimiter.RegisterHit(sender))
		{
			return;
		}
		if (sender != null && sender != NetworkServer.localConnection)
		{
			PlayElephantGunMissInternal(shootingPlayer, direction);
		}
		foreach (NetworkConnectionToClient value in NetworkServer.connections.Values)
		{
			if (value != NetworkServer.localConnection && value != sender)
			{
				RpcPlayElephantGunMiss(value, shootingPlayer, direction);
			}
		}
	}

	protected static void InvokeUserCode_CmdPlayElephantGunMissForAllClients__PlayerInventory__Vector3__NetworkConnectionToClient(NetworkBehaviour obj, NetworkReader reader, NetworkConnectionToClient senderConnection)
	{
		if (!NetworkServer.active)
		{
			Debug.LogError("Command CmdPlayElephantGunMissForAllClients called on client.");
		}
		else
		{
			((VfxManager)obj).UserCode_CmdPlayElephantGunMissForAllClients__PlayerInventory__Vector3__NetworkConnectionToClient(reader.ReadNetworkBehaviour<PlayerInventory>(), reader.ReadVector3(), senderConnection);
		}
	}

	protected void UserCode_RpcPlayElephantGunMiss__NetworkConnectionToClient__PlayerInventory__Vector3(NetworkConnectionToClient connection, PlayerInventory shootingPlayer, Vector3 direction)
	{
		PlayElephantGunMissInternal(shootingPlayer, direction);
	}

	protected static void InvokeUserCode_RpcPlayElephantGunMiss__NetworkConnectionToClient__PlayerInventory__Vector3(NetworkBehaviour obj, NetworkReader reader, NetworkConnectionToClient senderConnection)
	{
		if (!NetworkClient.active)
		{
			Debug.LogError("TargetRPC RpcPlayElephantGunMiss called on server.");
		}
		else
		{
			((VfxManager)obj).UserCode_RpcPlayElephantGunMiss__NetworkConnectionToClient__PlayerInventory__Vector3(null, reader.ReadNetworkBehaviour<PlayerInventory>(), reader.ReadVector3());
		}
	}

	protected void UserCode_CmdPlayElephantGunElectromagnetShieldDeflectedHitForAllClients__PlayerInfo__Vector3__GunShotHitVfxData__NetworkConnectionToClient(PlayerInfo shieldOwner, Vector3 shieldWorldNormal, GunShotHitVfxData hitData, NetworkConnectionToClient sender)
	{
		if (!serverPlayElephantGunElectromagnetShieldDeflectedHitCommandRateLimiter.RegisterHit(sender))
		{
			return;
		}
		if (sender != null && sender != NetworkServer.localConnection)
		{
			PlayElephantGunElectromagnetShieldDeflectedHitInternal(shieldOwner, shieldWorldNormal, hitData);
		}
		foreach (NetworkConnectionToClient value in NetworkServer.connections.Values)
		{
			if (value != NetworkServer.localConnection && value != sender)
			{
				RpcPlayElephantGunElectromagnetShieldDeflectedHit(value, shieldOwner, shieldWorldNormal, hitData);
			}
		}
	}

	protected static void InvokeUserCode_CmdPlayElephantGunElectromagnetShieldDeflectedHitForAllClients__PlayerInfo__Vector3__GunShotHitVfxData__NetworkConnectionToClient(NetworkBehaviour obj, NetworkReader reader, NetworkConnectionToClient senderConnection)
	{
		if (!NetworkServer.active)
		{
			Debug.LogError("Command CmdPlayElephantGunElectromagnetShieldDeflectedHitForAllClients called on client.");
		}
		else
		{
			((VfxManager)obj).UserCode_CmdPlayElephantGunElectromagnetShieldDeflectedHitForAllClients__PlayerInfo__Vector3__GunShotHitVfxData__NetworkConnectionToClient(reader.ReadNetworkBehaviour<PlayerInfo>(), reader.ReadVector3(), GeneratedNetworkCode._Read_VfxManager_002FGunShotHitVfxData(reader), senderConnection);
		}
	}

	protected void UserCode_RpcPlayElephantGunElectromagnetShieldDeflectedHit__NetworkConnectionToClient__PlayerInfo__Vector3__GunShotHitVfxData(NetworkConnectionToClient connection, PlayerInfo shieldOwner, Vector3 shieldWorldNormal, GunShotHitVfxData hitData)
	{
		PlayElephantGunElectromagnetShieldDeflectedHitInternal(shieldOwner, shieldWorldNormal, hitData);
	}

	protected static void InvokeUserCode_RpcPlayElephantGunElectromagnetShieldDeflectedHit__NetworkConnectionToClient__PlayerInfo__Vector3__GunShotHitVfxData(NetworkBehaviour obj, NetworkReader reader, NetworkConnectionToClient senderConnection)
	{
		if (!NetworkClient.active)
		{
			Debug.LogError("TargetRPC RpcPlayElephantGunElectromagnetShieldDeflectedHit called on server.");
		}
		else
		{
			((VfxManager)obj).UserCode_RpcPlayElephantGunElectromagnetShieldDeflectedHit__NetworkConnectionToClient__PlayerInfo__Vector3__GunShotHitVfxData(null, reader.ReadNetworkBehaviour<PlayerInfo>(), reader.ReadVector3(), GeneratedNetworkCode._Read_VfxManager_002FGunShotHitVfxData(reader));
		}
	}

	protected void UserCode_CmdPlayElephantGunElectromagnetShieldDeflectedMissForAllClients__PlayerInfo__Vector3__Vector3__Single__NetworkConnectionToClient(PlayerInfo shieldOwner, Vector3 shieldWorldNormal, Vector3 deflectedDirection, float distance, NetworkConnectionToClient sender)
	{
		if (!serverPlayElephantGunElectromagnetShieldDeflectedMissCommandRateLimiter.RegisterHit(sender))
		{
			return;
		}
		if (sender != null && sender != NetworkServer.localConnection)
		{
			PlayElephantGunElectromagnetShieldDeflectedMissInternal(shieldOwner, shieldWorldNormal, deflectedDirection, distance);
		}
		foreach (NetworkConnectionToClient value in NetworkServer.connections.Values)
		{
			if (value != NetworkServer.localConnection && value != sender)
			{
				RpcPlayElephantGunElectromagnetShieldDeflectedMiss(value, shieldOwner, shieldWorldNormal, deflectedDirection, distance);
			}
		}
	}

	protected static void InvokeUserCode_CmdPlayElephantGunElectromagnetShieldDeflectedMissForAllClients__PlayerInfo__Vector3__Vector3__Single__NetworkConnectionToClient(NetworkBehaviour obj, NetworkReader reader, NetworkConnectionToClient senderConnection)
	{
		if (!NetworkServer.active)
		{
			Debug.LogError("Command CmdPlayElephantGunElectromagnetShieldDeflectedMissForAllClients called on client.");
		}
		else
		{
			((VfxManager)obj).UserCode_CmdPlayElephantGunElectromagnetShieldDeflectedMissForAllClients__PlayerInfo__Vector3__Vector3__Single__NetworkConnectionToClient(reader.ReadNetworkBehaviour<PlayerInfo>(), reader.ReadVector3(), reader.ReadVector3(), reader.ReadFloat(), senderConnection);
		}
	}

	protected void UserCode_RpcPlayElephantGunElectromagnetShieldDeflectedMiss__NetworkConnectionToClient__PlayerInfo__Vector3__Vector3__Single(NetworkConnectionToClient connection, PlayerInfo shieldOwner, Vector3 shieldWorldNormal, Vector3 deflectedDirection, float distance)
	{
		PlayElephantGunElectromagnetShieldDeflectedMissInternal(shieldOwner, shieldWorldNormal, deflectedDirection, distance);
	}

	protected static void InvokeUserCode_RpcPlayElephantGunElectromagnetShieldDeflectedMiss__NetworkConnectionToClient__PlayerInfo__Vector3__Vector3__Single(NetworkBehaviour obj, NetworkReader reader, NetworkConnectionToClient senderConnection)
	{
		if (!NetworkClient.active)
		{
			Debug.LogError("TargetRPC RpcPlayElephantGunElectromagnetShieldDeflectedMiss called on server.");
		}
		else
		{
			((VfxManager)obj).UserCode_RpcPlayElephantGunElectromagnetShieldDeflectedMiss__NetworkConnectionToClient__PlayerInfo__Vector3__Vector3__Single(null, reader.ReadNetworkBehaviour<PlayerInfo>(), reader.ReadVector3(), reader.ReadVector3(), reader.ReadFloat());
		}
	}

	static VfxManager()
	{
		RemoteProcedureCalls.RegisterCommand(typeof(VfxManager), "System.Void VfxManager::CmdPlayPooledVfxForAllOtherClients(VfxType,UnityEngine.Vector3,UnityEngine.Quaternion,UnityEngine.Vector3,System.UInt32,System.Boolean,System.Single,Mirror.NetworkConnectionToClient)", InvokeUserCode_CmdPlayPooledVfxForAllOtherClients__VfxType__Vector3__Quaternion__Vector3__UInt32__Boolean__Single__NetworkConnectionToClient, requiresAuthority: false);
		RemoteProcedureCalls.RegisterCommand(typeof(VfxManager), "System.Void VfxManager::CmdPlayDuelingPistolHitForAllClients(PlayerInventory,VfxManager/GunShotHitVfxData,Mirror.NetworkConnectionToClient)", InvokeUserCode_CmdPlayDuelingPistolHitForAllClients__PlayerInventory__GunShotHitVfxData__NetworkConnectionToClient, requiresAuthority: false);
		RemoteProcedureCalls.RegisterCommand(typeof(VfxManager), "System.Void VfxManager::CmdPlayDuelingPistolMissForAllClients(PlayerInventory,UnityEngine.Vector3,Mirror.NetworkConnectionToClient)", InvokeUserCode_CmdPlayDuelingPistolMissForAllClients__PlayerInventory__Vector3__NetworkConnectionToClient, requiresAuthority: false);
		RemoteProcedureCalls.RegisterCommand(typeof(VfxManager), "System.Void VfxManager::CmdPlayDuelingPistolElectromagnetShieldDeflectedHitForAllClients(PlayerInfo,UnityEngine.Vector3,VfxManager/GunShotHitVfxData,Mirror.NetworkConnectionToClient)", InvokeUserCode_CmdPlayDuelingPistolElectromagnetShieldDeflectedHitForAllClients__PlayerInfo__Vector3__GunShotHitVfxData__NetworkConnectionToClient, requiresAuthority: false);
		RemoteProcedureCalls.RegisterCommand(typeof(VfxManager), "System.Void VfxManager::CmdPlayDuelingPistolElectromagnetShieldDeflectedMissForAllClients(PlayerInfo,UnityEngine.Vector3,UnityEngine.Vector3,System.Single,Mirror.NetworkConnectionToClient)", InvokeUserCode_CmdPlayDuelingPistolElectromagnetShieldDeflectedMissForAllClients__PlayerInfo__Vector3__Vector3__Single__NetworkConnectionToClient, requiresAuthority: false);
		RemoteProcedureCalls.RegisterCommand(typeof(VfxManager), "System.Void VfxManager::CmdPlayElephantGunHitForAllClients(PlayerInventory,VfxManager/GunShotHitVfxData,Mirror.NetworkConnectionToClient)", InvokeUserCode_CmdPlayElephantGunHitForAllClients__PlayerInventory__GunShotHitVfxData__NetworkConnectionToClient, requiresAuthority: false);
		RemoteProcedureCalls.RegisterCommand(typeof(VfxManager), "System.Void VfxManager::CmdPlayElephantGunMissForAllClients(PlayerInventory,UnityEngine.Vector3,Mirror.NetworkConnectionToClient)", InvokeUserCode_CmdPlayElephantGunMissForAllClients__PlayerInventory__Vector3__NetworkConnectionToClient, requiresAuthority: false);
		RemoteProcedureCalls.RegisterCommand(typeof(VfxManager), "System.Void VfxManager::CmdPlayElephantGunElectromagnetShieldDeflectedHitForAllClients(PlayerInfo,UnityEngine.Vector3,VfxManager/GunShotHitVfxData,Mirror.NetworkConnectionToClient)", InvokeUserCode_CmdPlayElephantGunElectromagnetShieldDeflectedHitForAllClients__PlayerInfo__Vector3__GunShotHitVfxData__NetworkConnectionToClient, requiresAuthority: false);
		RemoteProcedureCalls.RegisterCommand(typeof(VfxManager), "System.Void VfxManager::CmdPlayElephantGunElectromagnetShieldDeflectedMissForAllClients(PlayerInfo,UnityEngine.Vector3,UnityEngine.Vector3,System.Single,Mirror.NetworkConnectionToClient)", InvokeUserCode_CmdPlayElephantGunElectromagnetShieldDeflectedMissForAllClients__PlayerInfo__Vector3__Vector3__Single__NetworkConnectionToClient, requiresAuthority: false);
		RemoteProcedureCalls.RegisterRpc(typeof(VfxManager), "System.Void VfxManager::RpcPlayPooledVfxForAllClients(VfxType,UnityEngine.Vector3,UnityEngine.Quaternion,UnityEngine.Vector3,System.UInt32,System.Boolean,System.Single)", InvokeUserCode_RpcPlayPooledVfxForAllClients__VfxType__Vector3__Quaternion__Vector3__UInt32__Boolean__Single);
		RemoteProcedureCalls.RegisterRpc(typeof(VfxManager), "System.Void VfxManager::RpcPlayPooledVfx(Mirror.NetworkConnectionToClient,VfxType,UnityEngine.Vector3,UnityEngine.Quaternion,UnityEngine.Vector3,System.UInt32,System.Boolean,System.Single)", InvokeUserCode_RpcPlayPooledVfx__NetworkConnectionToClient__VfxType__Vector3__Quaternion__Vector3__UInt32__Boolean__Single);
		RemoteProcedureCalls.RegisterRpc(typeof(VfxManager), "System.Void VfxManager::RpcPlayDuelingPistolHit(Mirror.NetworkConnectionToClient,PlayerInventory,VfxManager/GunShotHitVfxData)", InvokeUserCode_RpcPlayDuelingPistolHit__NetworkConnectionToClient__PlayerInventory__GunShotHitVfxData);
		RemoteProcedureCalls.RegisterRpc(typeof(VfxManager), "System.Void VfxManager::RpcPlayDuelingPistolMiss(Mirror.NetworkConnectionToClient,PlayerInventory,UnityEngine.Vector3)", InvokeUserCode_RpcPlayDuelingPistolMiss__NetworkConnectionToClient__PlayerInventory__Vector3);
		RemoteProcedureCalls.RegisterRpc(typeof(VfxManager), "System.Void VfxManager::RpcPlayDuelingPistolElectromagnetShieldDeflectedHit(Mirror.NetworkConnectionToClient,PlayerInfo,UnityEngine.Vector3,VfxManager/GunShotHitVfxData)", InvokeUserCode_RpcPlayDuelingPistolElectromagnetShieldDeflectedHit__NetworkConnectionToClient__PlayerInfo__Vector3__GunShotHitVfxData);
		RemoteProcedureCalls.RegisterRpc(typeof(VfxManager), "System.Void VfxManager::RpcPlayDuelingPistolElectromagnetShieldDeflectedMiss(Mirror.NetworkConnectionToClient,PlayerInfo,UnityEngine.Vector3,UnityEngine.Vector3,System.Single)", InvokeUserCode_RpcPlayDuelingPistolElectromagnetShieldDeflectedMiss__NetworkConnectionToClient__PlayerInfo__Vector3__Vector3__Single);
		RemoteProcedureCalls.RegisterRpc(typeof(VfxManager), "System.Void VfxManager::RpcPlayElephantGunHit(Mirror.NetworkConnectionToClient,PlayerInventory,VfxManager/GunShotHitVfxData)", InvokeUserCode_RpcPlayElephantGunHit__NetworkConnectionToClient__PlayerInventory__GunShotHitVfxData);
		RemoteProcedureCalls.RegisterRpc(typeof(VfxManager), "System.Void VfxManager::RpcPlayElephantGunMiss(Mirror.NetworkConnectionToClient,PlayerInventory,UnityEngine.Vector3)", InvokeUserCode_RpcPlayElephantGunMiss__NetworkConnectionToClient__PlayerInventory__Vector3);
		RemoteProcedureCalls.RegisterRpc(typeof(VfxManager), "System.Void VfxManager::RpcPlayElephantGunElectromagnetShieldDeflectedHit(Mirror.NetworkConnectionToClient,PlayerInfo,UnityEngine.Vector3,VfxManager/GunShotHitVfxData)", InvokeUserCode_RpcPlayElephantGunElectromagnetShieldDeflectedHit__NetworkConnectionToClient__PlayerInfo__Vector3__GunShotHitVfxData);
		RemoteProcedureCalls.RegisterRpc(typeof(VfxManager), "System.Void VfxManager::RpcPlayElephantGunElectromagnetShieldDeflectedMiss(Mirror.NetworkConnectionToClient,PlayerInfo,UnityEngine.Vector3,UnityEngine.Vector3,System.Single)", InvokeUserCode_RpcPlayElephantGunElectromagnetShieldDeflectedMiss__NetworkConnectionToClient__PlayerInfo__Vector3__Vector3__Single);
	}
}
