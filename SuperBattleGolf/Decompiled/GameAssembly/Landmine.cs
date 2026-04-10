#define DEBUG_DRAW
using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using Cysharp.Threading.Tasks;
using FMODUnity;
using Mirror;
using Mirror.RemoteCalls;
using Unity.Collections;
using UnityEngine;
using UnityEngine.Pool;

public class Landmine : NetworkBehaviour
{
	[SerializeField]
	private bool forceArmed;

	[SerializeField]
	private bool forcePlanted;

	[SerializeField]
	private MineVfx vfx;

	private ItemUseId itemUseId;

	[SyncVar(hook = "OnIsArmedChanged")]
	private bool isArmed;

	[SyncVar(hook = "OnIsPlantedChanged")]
	private bool isPlanted;

	private Coroutine armingRoutine;

	private bool isArming;

	private bool isArmingWhenStationary;

	private float remainingArmingTime;

	private bool isExploded;

	private Entity attachedEntity;

	[CVar("drawLandmineDebug", "", "", false, true)]
	private static bool drawLandmineDebug;

	public Action<bool, bool> _Mirror_SyncVarHookDelegate_isArmed;

	public Action<bool, bool> _Mirror_SyncVarHookDelegate_isPlanted;

	public Entity AsEntity { get; private set; }

	public PlayerInventory Owner { get; private set; }

	public bool IsArmed => isArmed;

	public bool IsPlanted => isPlanted;

	public bool NetworkisArmed
	{
		get
		{
			return isArmed;
		}
		[param: In]
		set
		{
			GeneratedSyncVarSetter(value, ref isArmed, 1uL, _Mirror_SyncVarHookDelegate_isArmed);
		}
	}

	public bool NetworkisPlanted
	{
		get
		{
			return isPlanted;
		}
		[param: In]
		set
		{
			GeneratedSyncVarSetter(value, ref isPlanted, 2uL, _Mirror_SyncVarHookDelegate_isPlanted);
		}
	}

	private void Awake()
	{
		AsEntity = GetComponent<Entity>();
	}

	private void Start()
	{
		AsEntity.Rigidbody.SetCenterOfMassAndInertiaTensor(GameManager.ItemSettings.LandmineLocalCenterOfMass);
	}

	public override void OnStartServer()
	{
		if (forceArmed)
		{
			NetworkisArmed = true;
		}
		if (forcePlanted)
		{
			NetworkisPlanted = true;
		}
		AsEntity.AsHittable.WasHitByGolfSwing += OnServerWasHitByGolfSwing;
		AsEntity.AsHittable.WasHitBySwingProjectile += OnServerWasHitBySwingProjectile;
		AsEntity.AsHittable.WasHitByDive += OnServerWasHitByDive;
		AsEntity.AsHittable.WasHitByItem += OnServerWasHitByItem;
		AsEntity.AsHittable.WasHitByRocketLauncherBackBlast += OnServerWasHitByRocketLauncherBackBlast;
		AsEntity.AsHittable.WasHitByRocketDriverSwingPostHitSpin += OnServerWasHitByRocketDriverSwingPostHitSpin;
		AsEntity.AsHittable.HitAsSwingProjectile += OnServerHitAsSwingProjectile;
	}

	public override void OnStopServer()
	{
		if (isArmed)
		{
			LandmineManager.DeregisterArmedLandmine(this);
		}
		if (!BNetworkManager.IsChangingSceneOrShuttingDown)
		{
			AsEntity.AsHittable.WasHitByGolfSwing -= OnServerWasHitByGolfSwing;
			AsEntity.AsHittable.WasHitBySwingProjectile -= OnServerWasHitBySwingProjectile;
			AsEntity.AsHittable.WasHitByDive -= OnServerWasHitByDive;
			AsEntity.AsHittable.WasHitByItem -= OnServerWasHitByItem;
			AsEntity.AsHittable.WasHitByRocketLauncherBackBlast -= OnServerWasHitByRocketLauncherBackBlast;
			AsEntity.AsHittable.WasHitByRocketDriverSwingPostHitSpin -= OnServerWasHitByRocketDriverSwingPostHitSpin;
			AsEntity.AsHittable.HitAsSwingProjectile -= OnServerHitAsSwingProjectile;
			if (attachedEntity != null)
			{
				attachedEntity.WillBeDestroyed -= OnAttachedEntityWillBeDestroyed;
			}
		}
	}

	public override void OnStartClient()
	{
		if (Owner != null)
		{
			AsEntity.TemporarilyIgnoreCollisionsWith(Owner.PlayerInfo.AsEntity, 0.5f);
		}
	}

	private void OnCollisionEnter(Collision collision)
	{
		if (NetworkServer.active && isArmed && !(collision.rigidbody == null) && !(collision.relativeVelocity.sqrMagnitude < 0.001f) && (!collision.collider.TryGetComponentInParent<Hittable>(out var foundComponent, includeInactive: true) || (!WillBeReflectedOnCollision(foundComponent.AsEntity) && CanExplodeOnCollisionWith(foundComponent))))
		{
			ServerExplode();
		}
	}

	public void ServerInitialize(LandmineArmType armType, PlayerInventory owner, ItemUseId itemUseId)
	{
		Owner = owner;
		this.itemUseId = itemUseId;
		if (armingRoutine != null)
		{
			StopCoroutine(armingRoutine);
		}
		switch (armType)
		{
		case LandmineArmType.Planted:
			NetworkisPlanted = true;
			ServerArmDelayed(GameManager.ItemSettings.LandminePlantingArmDelay);
			break;
		case LandmineArmType.Tossed:
			ServerArmWhenStationary(GameManager.ItemSettings.LandmineTossingMinArmDelay, GameManager.ItemSettings.LandmineTossingMaxArmDelay, GameManager.ItemSettings.LandmineTossingMaxArmSpeed);
			break;
		}
	}

	[Server]
	private void ServerArmDelayed(float delay)
	{
		if (!NetworkServer.active)
		{
			Debug.LogWarning("[Server] function 'System.Void Landmine::ServerArmDelayed(System.Single)' called when server was not active");
		}
		else if (ShouldArm())
		{
			ServerCancelArming();
			armingRoutine = StartCoroutine(ArmDelayedRoutine(delay));
		}
		IEnumerator ArmDelayedRoutine(float num)
		{
			isArming = true;
			for (remainingArmingTime = num; remainingArmingTime > 0f; remainingArmingTime -= Time.deltaTime)
			{
				yield return null;
			}
			isArming = false;
			NetworkisArmed = true;
		}
		bool ShouldArm()
		{
			if (isArmed)
			{
				return false;
			}
			if (isArmingWhenStationary)
			{
				return true;
			}
			if (!isArming)
			{
				return true;
			}
			if (remainingArmingTime > delay)
			{
				return true;
			}
			return false;
		}
	}

	[Server]
	private void ServerArmWhenStationary(float minDelay, float maxDelay, float maxSpeedSquared)
	{
		if (!NetworkServer.active)
		{
			Debug.LogWarning("[Server] function 'System.Void Landmine::ServerArmWhenStationary(System.Single,System.Single,System.Single)' called when server was not active");
		}
		else if (ShouldArm())
		{
			ServerCancelArming();
			armingRoutine = StartCoroutine(ArmWhenStationaryRoutine(minDelay, maxDelay, maxSpeedSquared));
		}
		IEnumerator ArmWhenStationaryRoutine(float num2, float num, float num3)
		{
			isArming = true;
			isArmingWhenStationary = true;
			for (float time = 0f; time < num && (!(time > num2) || !(AsEntity.Rigidbody.linearVelocity.sqrMagnitude <= num3)); time += Time.deltaTime)
			{
				yield return null;
			}
			isArming = false;
			isArmingWhenStationary = false;
			NetworkisArmed = true;
		}
		bool ShouldArm()
		{
			if (isArmed)
			{
				return false;
			}
			if (isArming)
			{
				return false;
			}
			return true;
		}
	}

	[Server]
	private void ServerCancelArming()
	{
		if (!NetworkServer.active)
		{
			Debug.LogWarning("[Server] function 'System.Void Landmine::ServerCancelArming()' called when server was not active");
		}
		else if (isArming)
		{
			isArming = false;
			isArmingWhenStationary = false;
			if (armingRoutine != null)
			{
				StopCoroutine(armingRoutine);
			}
		}
	}

	[Server]
	public void ServerProcessDetectedColliders(NativeSlice<ColliderHit> detectedColliderBuffer)
	{
		if (!NetworkServer.active)
		{
			Debug.LogWarning("[Server] function 'System.Void Landmine::ServerProcessDetectedColliders(Unity.Collections.NativeSlice`1<UnityEngine.ColliderHit>)' called when server was not active");
			return;
		}
		HashSet<Entity> value;
		using (CollectionPool<HashSet<Entity>, Entity>.Get(out value))
		{
			Vector3 worldPosition = Vector3.negativeInfinity;
			foreach (ColliderHit item in detectedColliderBuffer)
			{
				if (item.collider == null)
				{
					break;
				}
				Entity foundComponent;
				if (item.collider.attachedRigidbody != null)
				{
					if (item.collider.attachedRigidbody == AsEntity.Rigidbody || !item.collider.attachedRigidbody.TryGetComponentInParent<Entity>(out foundComponent, includeInactive: true))
					{
						continue;
					}
				}
				else if (!item.collider.TryGetComponentInParent<Entity>(out foundComponent, includeInactive: true))
				{
					continue;
				}
				if (!foundComponent.HasRigidbody || !value.Add(foundComponent))
				{
					continue;
				}
				if ((foundComponent.IsPlayer ? foundComponent.PlayerInfo.Movement.SyncedVelocity : foundComponent.Rigidbody.linearVelocity).sqrMagnitude >= GameManager.ItemSettings.LandmineDetectionMinSpeedSquared)
				{
					ServerExplode();
					break;
				}
				if (!WillBeReflectedOnCollision(foundComponent))
				{
					if (float.IsNegativeInfinity(worldPosition.x))
					{
						worldPosition = base.transform.TransformPoint(GameManager.ItemSettings.LandmineLocalCenter);
					}
					if (foundComponent.TryGetClosestPointOnAllActiveColliders(worldPosition, out var _, out var distanceSquared) && distanceSquared < GameManager.ItemSettings.LandmineCollisionRangeSquared)
					{
						ServerExplode();
						break;
					}
				}
			}
		}
	}

	private bool WillBeReflectedOnCollision(Entity entity)
	{
		if (entity == AsEntity)
		{
			return false;
		}
		if (!entity.IsPlayer)
		{
			return false;
		}
		if (AsEntity.AsHittable.SwingProjectileState == SwingProjectileState.None)
		{
			return false;
		}
		if (!entity.PlayerInfo.IsElectromagnetShieldActive)
		{
			return false;
		}
		return true;
	}

	[Server]
	private void ServerExplode()
	{
		if (!NetworkServer.active)
		{
			Debug.LogWarning("[Server] function 'System.Void Landmine::ServerExplode()' called when server was not active");
		}
		else
		{
			if (!CanExplode())
			{
				return;
			}
			isExploded = true;
			int num = Physics.OverlapSphereNonAlloc(base.transform.position, GameManager.ItemSettings.LandmineExplosionRange, layerMask: GameManager.LayerSettings.LandmineHittablesMask, results: PlayerGolfer.overlappingColliderBuffer, queryTriggerInteraction: QueryTriggerInteraction.Ignore);
			HashSet<Hittable> value;
			using (CollectionPool<HashSet<Hittable>, Hittable>.Get(out value))
			{
				for (int i = 0; i < num; i++)
				{
					if (PlayerGolfer.overlappingColliderBuffer[i].TryGetComponentInParent<Hittable>(out var foundComponent, includeInactive: true) && !(foundComponent == AsEntity.AsHittable) && value.Add(foundComponent))
					{
						if (!foundComponent.TryGetClosestPointOnAllActiveColliders(base.transform.position, out var closestPoint, out var distanceSquared))
						{
							distanceSquared = 0f;
							closestPoint = ((!foundComponent.AsEntity.HasRigidbody) ? foundComponent.transform.position : foundComponent.AsEntity.Rigidbody.worldCenterOfMass);
						}
						Vector3 direction = closestPoint - base.transform.position;
						foundComponent.HitWithItem(ItemType.Landmine, itemUseId, foundComponent.transform.InverseTransformPoint(closestPoint), direction, foundComponent.transform.InverseTransformPoint(base.transform.position), BMath.Sqrt(distanceSquared), Owner, isReflected: false, isPlanted, canHitWithNoUser: true);
						if (drawLandmineDebug)
						{
							BDebug.DrawLine(base.transform.position, closestPoint, Color.yellow, 5f);
						}
					}
				}
				OnExploded();
				foreach (NetworkConnectionToClient value2 in NetworkServer.connections.Values)
				{
					if (value2 != NetworkServer.localConnection)
					{
						RpcInformExploded(value2);
					}
				}
				if (drawLandmineDebug)
				{
					BDebug.DrawWireSphere(base.transform.position, GameManager.ItemSettings.LandmineExplosionRange, Color.red, 5f, drawInsideLines: true);
				}
				AsEntity.DestroyEntity();
			}
		}
		bool CanExplode()
		{
			if (isExploded)
			{
				return false;
			}
			if (AsEntity.IsDestroyed)
			{
				return false;
			}
			if (AsEntity.AsHittable.IsFrozen)
			{
				return false;
			}
			return true;
		}
	}

	[TargetRpc]
	private void RpcInformExploded(NetworkConnectionToClient connection)
	{
		NetworkWriterPooled writer = NetworkWriterPool.Get();
		SendTargetRPCInternal(connection, "System.Void Landmine::RpcInformExploded(Mirror.NetworkConnectionToClient)", 694097335, writer, 0);
		NetworkWriterPool.Return(writer);
	}

	private void OnExploded()
	{
		VfxManager.PlayPooledVfxLocalOnly(VfxType.MineExplosion, base.transform.position, Quaternion.identity);
		if (GameplayCameraManager.ShouldPlayImpactFrameForExplosion(base.transform.position, GameManager.ItemSettings.LandmineExplosionRange, GameManager.CameraGameplaySettings.LandmineImpactFrameDistanceSquared))
		{
			CameraModuleController.PlayImpactFrame(base.transform.position);
		}
		CameraModuleController.Shake(GameManager.CameraGameplaySettings.LandmineExplosionScreenshakeSettings, base.transform.position);
		RuntimeManager.PlayOneShot(GameManager.AudioSettings.LandmineExplosionEvent, base.transform.position);
	}

	private async void ServerExplodeDelayed(float delay)
	{
		await UniTask.WaitForSeconds(delay);
		if (!(this == null))
		{
			ServerExplode();
		}
	}

	private bool CanExplodeOnCollisionWith(Hittable hitHittable)
	{
		if (hitHittable.AsEntity.IsPlayer)
		{
			return true;
		}
		if (hitHittable.AsEntity.IsGolfCart)
		{
			return true;
		}
		if (hitHittable.TryGetComponent<TargetDummy>(out var _))
		{
			return true;
		}
		return false;
	}

	private void OnServerWasHitByGolfSwing(PlayerGolfer hitter, Vector3 worldDirection, float power, bool isRocketDriver)
	{
		if (isArmed)
		{
			ServerExplode();
		}
		else
		{
			ServerArmDelayed(GameManager.ItemSettings.LandmineHitArmDelay);
		}
	}

	private void OnServerWasHitBySwingProjectile(Hittable hitter, Vector3 worldHitVelocity)
	{
		if (isArmed)
		{
			ServerExplode();
		}
		else
		{
			ServerArmDelayed(GameManager.ItemSettings.LandmineHitArmDelay);
		}
	}

	private void OnServerWasHitByDive(PlayerMovement hitter)
	{
		if (isArmed)
		{
			ServerExplode();
		}
		else
		{
			ServerArmDelayed(GameManager.ItemSettings.LandmineHitArmDelay);
		}
	}

	private void OnServerWasHitByItem(PlayerInventory itemUser, ItemType itemType, ItemUseId itemUseId, Vector3 direction, float distance, bool isReflected)
	{
		if (itemType == ItemType.FreezeBomb)
		{
			return;
		}
		if (isArmed)
		{
			if (itemType == ItemType.RocketLauncher || itemType == ItemType.Landmine || itemType == ItemType.OrbitalLaser)
			{
				ServerExplodeDelayed(0.1f);
			}
			else
			{
				ServerExplode();
			}
		}
		else
		{
			ServerArmDelayed(GameManager.ItemSettings.LandmineHitArmDelay);
		}
	}

	private void OnServerWasHitByRocketLauncherBackBlast(PlayerInventory rocketLauncherUser, Vector3 direction)
	{
		if (isArmed)
		{
			ServerExplode();
		}
		else
		{
			ServerArmDelayed(GameManager.ItemSettings.LandmineHitArmDelay);
		}
	}

	private void OnServerWasHitByRocketDriverSwingPostHitSpin(PlayerGolfer hitter, Vector3 direction)
	{
		if (isArmed)
		{
			ServerExplode();
		}
		else
		{
			ServerArmDelayed(GameManager.ItemSettings.LandmineHitArmDelay);
		}
	}

	private void OnServerHitAsSwingProjectile(Hittable hitHittable)
	{
		if (!isArmed)
		{
			ServerArmDelayed(GameManager.ItemSettings.LandmineHitArmDelay);
		}
		else if (CanExplodeOnCollisionWith(hitHittable))
		{
			ServerExplode();
		}
	}

	private void OnIsArmedChanged(bool wasArmed, bool isArmed)
	{
		if (base.isServer)
		{
			if (isArmed)
			{
				LandmineManager.RegisterArmedLandmine(this);
				AsEntity.Rigidbody.includeLayers = 1 << GameManager.LayerSettings.HittablesLayer;
			}
			else
			{
				LandmineManager.DeregisterArmedLandmine(this);
				AsEntity.Rigidbody.includeLayers = 0;
			}
		}
		if (isArmed)
		{
			vfx.Arm(forceArmed);
		}
		else
		{
			vfx.Unarm();
		}
	}

	private void OnIsPlantedChanged(bool wasPlanted, bool isPlanted)
	{
		AsEntity.AsItem.SetIsPickupSuppressed(isPlanted);
		AsEntity.AsItem.AsEntity.Rigidbody.isKinematic = isPlanted;
		if (!(!wasPlanted && isPlanted))
		{
			return;
		}
		int num = Physics.OverlapSphereNonAlloc(base.transform.position + Vector3.up * GameManager.ItemSettings.LandminePlantingOffsetIntoGround, GameManager.ItemSettings.LandminePlantingCollisionCheckRange, layerMask: GameManager.LayerSettings.LandmineDynamicPlantablesMask, results: PlayerGolfer.overlappingColliderBuffer, queryTriggerInteraction: QueryTriggerInteraction.Ignore);
		HashSet<Entity> value;
		using (CollectionPool<HashSet<Entity>, Entity>.Get(out value))
		{
			float num2 = GameManager.ItemSettings.LandmineDetectionRange * GameManager.ItemSettings.LandmineDetectionRange;
			float num3 = float.MaxValue;
			Entity entity = null;
			for (int i = 0; i < num; i++)
			{
				if (PlayerGolfer.overlappingColliderBuffer[i].TryGetComponentInParent<Entity>(out var foundComponent, includeInactive: true) && !(foundComponent == AsEntity) && value.Add(foundComponent))
				{
					if (!foundComponent.TryGetClosestPointOnAllActiveColliders(base.transform.position, out var closestPoint, out var distanceSquared))
					{
						distanceSquared = 0f;
						closestPoint = ((!foundComponent.HasRigidbody) ? foundComponent.transform.position : foundComponent.Rigidbody.worldCenterOfMass);
					}
					if (!(distanceSquared > num2) && distanceSquared < num3)
					{
						num3 = distanceSquared;
						entity = foundComponent;
					}
				}
			}
			if (!(entity != null))
			{
				return;
			}
			float landminePlantingCollisionCheckRange = GameManager.ItemSettings.LandminePlantingCollisionCheckRange;
			if (entity.TryGetComponent<BreakableIce>(out var _))
			{
				Ray ray = new Ray(base.transform.position + Vector3.up * landminePlantingCollisionCheckRange, Vector3.down);
				bool flag = Physics.Raycast(ray, landminePlantingCollisionCheckRange, GameManager.LayerSettings.TerrainMask, QueryTriggerInteraction.Ignore);
				if (drawLandmineDebug)
				{
					BDebug.DrawRay(ray.origin, ray.direction * landminePlantingCollisionCheckRange, flag ? Color.green : Color.red, 5f);
				}
				if (!flag)
				{
					attachedEntity = entity;
					attachedEntity.WillBeDestroyed += OnAttachedEntityWillBeDestroyed;
				}
			}
		}
	}

	private void OnAttachedEntityWillBeDestroyed()
	{
		attachedEntity.WillBeDestroyed -= OnAttachedEntityWillBeDestroyed;
		attachedEntity = null;
		NetworkisPlanted = false;
	}

	public Landmine()
	{
		_Mirror_SyncVarHookDelegate_isArmed = OnIsArmedChanged;
		_Mirror_SyncVarHookDelegate_isPlanted = OnIsPlantedChanged;
	}

	public override bool Weaved()
	{
		return true;
	}

	protected void UserCode_RpcInformExploded__NetworkConnectionToClient(NetworkConnectionToClient connection)
	{
		OnExploded();
	}

	protected static void InvokeUserCode_RpcInformExploded__NetworkConnectionToClient(NetworkBehaviour obj, NetworkReader reader, NetworkConnectionToClient senderConnection)
	{
		if (!NetworkClient.active)
		{
			Debug.LogError("TargetRPC RpcInformExploded called on server.");
		}
		else
		{
			((Landmine)obj).UserCode_RpcInformExploded__NetworkConnectionToClient(null);
		}
	}

	static Landmine()
	{
		RemoteProcedureCalls.RegisterRpc(typeof(Landmine), "System.Void Landmine::RpcInformExploded(Mirror.NetworkConnectionToClient)", InvokeUserCode_RpcInformExploded__NetworkConnectionToClient);
	}

	public override void SerializeSyncVars(NetworkWriter writer, bool forceAll)
	{
		base.SerializeSyncVars(writer, forceAll);
		if (forceAll)
		{
			writer.WriteBool(isArmed);
			writer.WriteBool(isPlanted);
			return;
		}
		writer.WriteVarULong(syncVarDirtyBits);
		if ((syncVarDirtyBits & 1L) != 0L)
		{
			writer.WriteBool(isArmed);
		}
		if ((syncVarDirtyBits & 2L) != 0L)
		{
			writer.WriteBool(isPlanted);
		}
	}

	public override void DeserializeSyncVars(NetworkReader reader, bool initialState)
	{
		base.DeserializeSyncVars(reader, initialState);
		if (initialState)
		{
			GeneratedSyncVarDeserialize(ref isArmed, _Mirror_SyncVarHookDelegate_isArmed, reader.ReadBool());
			GeneratedSyncVarDeserialize(ref isPlanted, _Mirror_SyncVarHookDelegate_isPlanted, reader.ReadBool());
			return;
		}
		long num = (long)reader.ReadVarULong();
		if ((num & 1L) != 0L)
		{
			GeneratedSyncVarDeserialize(ref isArmed, _Mirror_SyncVarHookDelegate_isArmed, reader.ReadBool());
		}
		if ((num & 2L) != 0L)
		{
			GeneratedSyncVarDeserialize(ref isPlanted, _Mirror_SyncVarHookDelegate_isPlanted, reader.ReadBool());
		}
	}
}
