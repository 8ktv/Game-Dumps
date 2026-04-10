#define DEBUG_DRAW
using System.Runtime.InteropServices;
using FMOD.Studio;
using FMODUnity;
using Mirror;
using Mirror.RemoteCalls;
using UnityEngine;

public class FreezeBomb : NetworkBehaviour, IFixedBUpdateCallback, IAnyBUpdateCallback
{
	[SerializeField]
	private LevelBoundsTracker levelBoundsTracker;

	[SerializeField]
	private float collisionRadius;

	[SyncVar]
	private PlayerInfo shooter;

	private ItemUseId itemUseId;

	private bool isReflected;

	private Entity asEntity;

	private Vector3 previousPosition;

	private PoolableParticleSystem trailVfx;

	private EventInstance soundInstance;

	private double shotOrReflectionTimestamp;

	private float distanceTravelled;

	[CVar("drawFreezeBombDebug", "", "", false, true)]
	private static bool drawFreezeBombDebug;

	protected NetworkBehaviourSyncVar ___shooterNetId;

	public PlayerInfo Networkshooter
	{
		get
		{
			return GetSyncVarNetworkBehaviour(___shooterNetId, ref shooter);
		}
		[param: In]
		set
		{
			GeneratedSyncVarSetter_NetworkBehaviour(value, ref shooter, 1uL, null, ref ___shooterNetId);
		}
	}

	private void Awake()
	{
		asEntity = GetComponent<Entity>();
		shotOrReflectionTimestamp = Time.timeAsDouble;
		soundInstance = RuntimeManager.CreateInstance(GameManager.AudioSettings.FreezeBombProjectileLoopEvent);
		RuntimeManager.AttachInstanceToGameObject(soundInstance, base.gameObject);
		soundInstance.start();
		soundInstance.release();
		asEntity.Rigidbody.maxAngularVelocity = BMath.Max(GameManager.ItemSettings.FreezeBombShotAngularSpeed, asEntity.Rigidbody.maxAngularVelocity);
		if (VfxPersistentData.TryGetPooledVfx(VfxType.FreezeBombTrail, out trailVfx))
		{
			trailVfx.transform.SetParent(base.transform);
			trailVfx.transform.SetLocalPositionAndRotation(Vector3.zero, Quaternion.identity);
			trailVfx.Play();
		}
		asEntity.WillBeDestroyed += OnWillBeDestroyed;
	}

	private void Start()
	{
		previousPosition = asEntity.Rigidbody.position;
	}

	private void OnWillBeDestroyed()
	{
		if (trailVfx != null)
		{
			trailVfx.Stop(ParticleSystemStopBehavior.StopEmittingAndClear);
		}
		soundInstance.stop(FMOD.Studio.STOP_MODE.ALLOWFADEOUT);
	}

	public override void OnStartServer()
	{
		if (!base.isClient)
		{
			BUpdate.RegisterCallback(this);
		}
	}

	public override void OnStopServer()
	{
		if (!base.isClient)
		{
			BUpdate.DeregisterCallback(this);
		}
	}

	public override void OnStartClient()
	{
		BUpdate.RegisterCallback(this);
	}

	public override void OnStopClient()
	{
		BUpdate.DeregisterCallback(this);
	}

	[Server]
	public void ServerInitialize(PlayerInfo shooter, Vector3 velocity, Vector3 angularVelocity, ItemUseId itemUseId)
	{
		if (!NetworkServer.active)
		{
			Debug.LogWarning("[Server] function 'System.Void FreezeBomb::ServerInitialize(PlayerInfo,UnityEngine.Vector3,UnityEngine.Vector3,ItemUseId)' called when server was not active");
			return;
		}
		Networkshooter = shooter;
		this.itemUseId = itemUseId;
		asEntity.Rigidbody.linearVelocity = velocity;
		asEntity.Rigidbody.angularVelocity = angularVelocity;
	}

	public void OnFixedBUpdate()
	{
		if (base.isServer)
		{
			if (CheckCollision(out var explosionPosition))
			{
				ServerExplode(explosionPosition);
				return;
			}
			distanceTravelled += asEntity.Rigidbody.linearVelocity.magnitude * Time.fixedDeltaTime;
			if (distanceTravelled > GameManager.ItemSettings.FreezeBombMaxTravelDistance)
			{
				ServerExplode(base.transform.position);
				return;
			}
			if (levelBoundsTracker.IsInWaterLocalOnly(out var _, 0f))
			{
				ServerExplode(base.transform.position);
				return;
			}
		}
		previousPosition = asEntity.Rigidbody.position;
		bool CheckCollision(out Vector3 reference)
		{
			bool flag = Networkshooter != null && BMath.GetTimeSince(shotOrReflectionTimestamp) < 0.25f;
			int num = Physics.OverlapSphereNonAlloc(previousPosition, collisionRadius, layerMask: GameManager.LayerSettings.FreezeBombHittablesMask, results: PlayerGolfer.overlappingColliderBuffer, queryTriggerInteraction: QueryTriggerInteraction.Ignore);
			reference = previousPosition;
			bool flag2 = false;
			for (int i = 0; i < num; i++)
			{
				Collider collider = PlayerGolfer.overlappingColliderBuffer[i];
				if (!flag || !collider.TryGetComponentInParent<PlayerInfo>(out var foundComponent, includeInactive: true) || !(foundComponent == Networkshooter))
				{
					if (collider.gameObject.layer == GameManager.LayerSettings.ElectromagnetShieldLayer && collider.TryGetComponentInParent<PlayerInfo>(out var foundComponent2, includeInactive: true))
					{
						ReflectOffElectromagnetShield(previousPosition, foundComponent2.ElectromagnetShieldCollider, foundComponent2);
						return false;
					}
					flag2 = true;
				}
			}
			if (flag2)
			{
				return true;
			}
			Vector3 direction = asEntity.Rigidbody.position - previousPosition;
			Ray ray = new Ray(previousPosition, direction);
			float magnitude = direction.magnitude;
			int num2 = Physics.SphereCastNonAlloc(radius: collisionRadius, maxDistance: magnitude, layerMask: GameManager.LayerSettings.FreezeBombHittablesMask, ray: ray, results: PlayerGolfer.raycastHitBuffer, queryTriggerInteraction: QueryTriggerInteraction.Ignore);
			RaycastHit raycastHit = new RaycastHit
			{
				distance = float.MaxValue
			};
			PlayerInfo playerInfo = null;
			for (int j = 0; j < num2; j++)
			{
				RaycastHit raycastHit2 = PlayerGolfer.raycastHitBuffer[j];
				if ((!flag || !raycastHit2.collider.TryGetComponentInParent<PlayerInfo>(out var foundComponent3, includeInactive: true) || !(foundComponent3 == Networkshooter)) && !(raycastHit2.distance >= raycastHit.distance))
				{
					raycastHit = raycastHit2;
					playerInfo = ((raycastHit2.collider.gameObject.layer != GameManager.LayerSettings.ElectromagnetShieldLayer || !raycastHit2.collider.TryGetComponentInParent<PlayerInfo>(out var foundComponent4, includeInactive: true)) ? null : foundComponent4);
				}
			}
			bool flag3 = raycastHit.distance < float.MaxValue;
			if (flag3)
			{
				if (flag3 && playerInfo != null)
				{
					ReflectOffElectromagnetShield(raycastHit.point, playerInfo.ElectromagnetShieldCollider, playerInfo);
					return false;
				}
				reference = raycastHit.point + raycastHit.normal * 0.1f;
			}
			return flag3;
		}
		void ReflectOffElectromagnetShield(Vector3 hitWorldPosition, SphereCollider shieldCollider, PlayerInfo shieldOwner)
		{
			if (!(shieldOwner == Networkshooter))
			{
				Networkshooter = shieldOwner;
				isReflected = true;
				Vector3 normalized = (shieldCollider.transform.position - hitWorldPosition).normalized;
				float num = Vector3.Dot(asEntity.Rigidbody.linearVelocity, normalized);
				if (!(num <= 0f))
				{
					Vector3 position = shieldCollider.transform.position - normalized * (shieldCollider.radius + collisionRadius + 0.1f);
					base.transform.position = position;
					asEntity.Rigidbody.position = position;
					Vector3 linearVelocity = asEntity.Rigidbody.linearVelocity - 2f * num * normalized;
					asEntity.Rigidbody.linearVelocity = linearVelocity;
					shotOrReflectionTimestamp = Time.timeAsDouble;
					shieldOwner.PlayElectromagnetShieldHitForAllClients(hitWorldPosition - shieldOwner.ElectromagnetShieldCollider.transform.position);
				}
			}
		}
	}

	[Server]
	private void ServerExplode(Vector3 worldPosition)
	{
		if (!NetworkServer.active)
		{
			Debug.LogWarning("[Server] function 'System.Void FreezeBomb::ServerExplode(UnityEngine.Vector3)' called when server was not active");
		}
		else
		{
			if (asEntity.IsDestroyed)
			{
				return;
			}
			if (levelBoundsTracker.IsInWaterLocalOnly(out var heightAboveWater, 0f) || heightAboveWater <= GameManager.ItemSettings.FreezeBombMaxExplosionHeightAboveWaterToCreatePlatform)
			{
				CreatePlatform();
			}
			int num = Physics.OverlapSphereNonAlloc(worldPosition, GameManager.ItemSettings.FreezeBombExplosionRange, layerMask: GameManager.LayerSettings.FreezeBombHittablesMask, results: PlayerGolfer.overlappingColliderBuffer, queryTriggerInteraction: QueryTriggerInteraction.Ignore);
			PlayerGolfer.processedHittableBuffer.Clear();
			for (int i = 0; i < num; i++)
			{
				if (PlayerGolfer.overlappingColliderBuffer[i].TryGetComponentInParent<Hittable>(out var foundComponent, includeInactive: true) && PlayerGolfer.processedHittableBuffer.Add(foundComponent))
				{
					if (!foundComponent.TryGetClosestPointOnAllActiveColliders(worldPosition, out var closestPoint, out var distanceSquared))
					{
						distanceSquared = 0f;
						closestPoint = ((!foundComponent.AsEntity.HasRigidbody) ? foundComponent.transform.position : foundComponent.AsEntity.Rigidbody.worldCenterOfMass);
					}
					Vector3 vector = closestPoint - worldPosition;
					if (vector == Vector3.zero)
					{
						vector = asEntity.Rigidbody.linearVelocity;
					}
					foundComponent.HitWithItem(ItemType.FreezeBomb, itemUseId, foundComponent.transform.InverseTransformPoint(closestPoint), vector, foundComponent.transform.InverseTransformPoint(worldPosition), BMath.Sqrt(distanceSquared), (Networkshooter != null) ? Networkshooter.Inventory : null, isReflected, isInSpecialState: false, canHitWithNoUser: true);
					if (drawFreezeBombDebug)
					{
						BDebug.DrawLine(worldPosition, closestPoint, Color.yellow, 5f);
					}
				}
			}
			OnExploded(worldPosition);
			foreach (NetworkConnectionToClient value in NetworkServer.connections.Values)
			{
				if (value != NetworkServer.localConnection)
				{
					RpcInformExploded(value, worldPosition);
				}
			}
			if (drawFreezeBombDebug)
			{
				BDebug.DrawWireSphere(worldPosition, GameManager.ItemSettings.FreezeBombExplosionRange, Color.red, 5f, drawInsideLines: true);
			}
			asEntity.DestroyEntity();
		}
		void CreatePlatform()
		{
			bool num2 = Random.value > 0.5f;
			Vector3 position = worldPosition;
			position.y = levelBoundsTracker.CurrentOutOfBoundsHazardWorldHeightLocalOnly + Random.Range(0f - GameManager.ItemSettings.FreezeBombPlatformMaxHeightOffset, GameManager.ItemSettings.FreezeBombPlatformMaxHeightOffset);
			float num3 = asEntity.Rigidbody.linearVelocity.GetYawDeg();
			if (num2)
			{
				num3 = (num3 + 180f).WrapAngleDeg();
			}
			Quaternion rotation = Quaternion.Euler(0f, num3, 0f);
			FreezeBombPlatform freezeBombPlatform = Object.Instantiate(GameManager.ItemSettings.FreezeBombPlatformPrefab, position, rotation);
			if (freezeBombPlatform == null)
			{
				Debug.LogError("Freeze bomb platform did not instantiate properly", base.gameObject);
			}
			else
			{
				NetworkServer.Spawn(freezeBombPlatform.gameObject);
			}
		}
	}

	[TargetRpc]
	private void RpcInformExploded(NetworkConnectionToClient connection, Vector3 worldPosition)
	{
		NetworkWriterPooled writer = NetworkWriterPool.Get();
		writer.WriteVector3(worldPosition);
		SendTargetRPCInternal(connection, "System.Void FreezeBomb::RpcInformExploded(Mirror.NetworkConnectionToClient,UnityEngine.Vector3)", -2087155441, writer, 0);
		NetworkWriterPool.Return(writer);
	}

	private void OnExploded(Vector3 worldPosition)
	{
		if (trailVfx != null)
		{
			trailVfx.transform.SetParent(null, worldPositionStays: true);
			trailVfx.Stop();
			trailVfx = null;
		}
		VfxManager.PlayPooledVfxLocalOnly(VfxType.FreezeBombImpact, worldPosition, Quaternion.identity);
		CameraModuleController.Shake(GameManager.CameraGameplaySettings.FreezeBombExplosionScreenshakeSettings, worldPosition);
		RuntimeManager.PlayOneShot(GameManager.AudioSettings.FreezeBombExplosionEvent, worldPosition);
	}

	public override bool Weaved()
	{
		return true;
	}

	protected void UserCode_RpcInformExploded__NetworkConnectionToClient__Vector3(NetworkConnectionToClient connection, Vector3 worldPosition)
	{
		OnExploded(worldPosition);
	}

	protected static void InvokeUserCode_RpcInformExploded__NetworkConnectionToClient__Vector3(NetworkBehaviour obj, NetworkReader reader, NetworkConnectionToClient senderConnection)
	{
		if (!NetworkClient.active)
		{
			Debug.LogError("TargetRPC RpcInformExploded called on server.");
		}
		else
		{
			((FreezeBomb)obj).UserCode_RpcInformExploded__NetworkConnectionToClient__Vector3(null, reader.ReadVector3());
		}
	}

	static FreezeBomb()
	{
		RemoteProcedureCalls.RegisterRpc(typeof(FreezeBomb), "System.Void FreezeBomb::RpcInformExploded(Mirror.NetworkConnectionToClient,UnityEngine.Vector3)", InvokeUserCode_RpcInformExploded__NetworkConnectionToClient__Vector3);
	}

	public override void SerializeSyncVars(NetworkWriter writer, bool forceAll)
	{
		base.SerializeSyncVars(writer, forceAll);
		if (forceAll)
		{
			writer.WriteNetworkBehaviour(Networkshooter);
			return;
		}
		writer.WriteVarULong(syncVarDirtyBits);
		if ((syncVarDirtyBits & 1L) != 0L)
		{
			writer.WriteNetworkBehaviour(Networkshooter);
		}
	}

	public override void DeserializeSyncVars(NetworkReader reader, bool initialState)
	{
		base.DeserializeSyncVars(reader, initialState);
		if (initialState)
		{
			GeneratedSyncVarDeserialize_NetworkBehaviour(ref shooter, null, reader, ref ___shooterNetId);
			return;
		}
		long num = (long)reader.ReadVarULong();
		if ((num & 1L) != 0L)
		{
			GeneratedSyncVarDeserialize_NetworkBehaviour(ref shooter, null, reader, ref ___shooterNetId);
		}
	}
}
