#define DEBUG_DRAW
using System;
using System.Collections;
using System.Runtime.InteropServices;
using FMOD.Studio;
using FMODUnity;
using Mirror;
using Mirror.RemoteCalls;
using UnityEngine;

public class Rocket : NetworkBehaviour, IFixedBUpdateCallback, IAnyBUpdateCallback, ILateBUpdateCallback
{
	[SerializeField]
	private float collisionRadius;

	[SyncVar]
	private PlayerInfo launcher;

	[SyncVar(hook = "OnHomingTargetHittableChanged")]
	private Hittable homingTargetHittable;

	private ItemUseId itemUseId;

	private bool isReflected;

	private Entity asEntity;

	private double launchOrReflectionTimestamp;

	private float distanceTravelled;

	private Vector3 previousPosition;

	private PoolableParticleSystem trailVfx;

	private EventInstance rocketEngineSound;

	private WorldspaceIconUi homingWarningWorldspaceIcon;

	private EventInstance homingWarningSoundInstance;

	private Coroutine homingWarningUpdateRoutine;

	[CVar("drawRocketDebug", "", "", false, true)]
	private static bool drawRocketDebug;

	protected NetworkBehaviourSyncVar ___launcherNetId;

	protected NetworkBehaviourSyncVar ___homingTargetHittableNetId;

	public Action<Hittable, Hittable> _Mirror_SyncVarHookDelegate_homingTargetHittable;

	public bool IsPlayingHomingWarning { get; private set; }

	public PlayerInfo Networklauncher
	{
		get
		{
			return GetSyncVarNetworkBehaviour(___launcherNetId, ref launcher);
		}
		[param: In]
		set
		{
			GeneratedSyncVarSetter_NetworkBehaviour(value, ref launcher, 1uL, null, ref ___launcherNetId);
		}
	}

	public Hittable NetworkhomingTargetHittable
	{
		get
		{
			return GetSyncVarNetworkBehaviour(___homingTargetHittableNetId, ref homingTargetHittable);
		}
		[param: In]
		set
		{
			GeneratedSyncVarSetter_NetworkBehaviour(value, ref homingTargetHittable, 2uL, _Mirror_SyncVarHookDelegate_homingTargetHittable, ref ___homingTargetHittableNetId);
		}
	}

	private void Awake()
	{
		asEntity = GetComponent<Entity>();
		launchOrReflectionTimestamp = Time.timeAsDouble;
		rocketEngineSound = RuntimeManager.CreateInstance(GameManager.AudioSettings.RocketEngineEvent);
		RuntimeManager.AttachInstanceToGameObject(rocketEngineSound, base.gameObject);
		rocketEngineSound.start();
		rocketEngineSound.release();
		if (VfxPersistentData.TryGetPooledVfx(VfxType.RocketLauncherRocketTrail, out trailVfx))
		{
			trailVfx.transform.SetParent(base.transform);
			trailVfx.transform.SetLocalPositionAndRotation(Vector3.zero, Quaternion.identity);
			trailVfx.Play();
		}
		asEntity.WillBeDestroyed += OnWillBeDestroyed;
		PlayerInfo.LocalPlayerIsInGolfCartChanged += OnLocalPlayerIsInGolfCartChanged;
		PlayerSpectator.LocalPlayerIsSpectatingChanged += OnLocalPlayerIsSpectatingChanged;
		PlayerSpectator.LocalPlayerSetSpectatingTarget += OnLocalPlayerSetSpectatingTarget;
		PlayerSpectator.LocalPlayerSpectatingTargetIsInGolfCartChanged += OnLocalPlayerSpectatingTargetActiveGolfCartSeatChanged;
	}

	private void Start()
	{
		asEntity.Rigidbody.linearVelocity = base.transform.forward * GameManager.ItemSettings.RocketVelocity;
		previousPosition = asEntity.Rigidbody.position;
	}

	private void OnWillBeDestroyed()
	{
		if (trailVfx != null)
		{
			trailVfx.Stop(ParticleSystemStopBehavior.StopEmittingAndClear);
		}
		rocketEngineSound.stop(FMOD.Studio.STOP_MODE.ALLOWFADEOUT);
		RemoveWorldspaceIcon();
		if (homingWarningSoundInstance.isValid())
		{
			homingWarningSoundInstance.stop(FMOD.Studio.STOP_MODE.ALLOWFADEOUT);
		}
		PlayerInfo.LocalPlayerIsInGolfCartChanged -= OnLocalPlayerIsInGolfCartChanged;
		PlayerSpectator.LocalPlayerIsSpectatingChanged -= OnLocalPlayerIsSpectatingChanged;
		PlayerSpectator.LocalPlayerSetSpectatingTarget -= OnLocalPlayerSetSpectatingTarget;
		PlayerSpectator.LocalPlayerSpectatingTargetIsInGolfCartChanged -= OnLocalPlayerSpectatingTargetActiveGolfCartSeatChanged;
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
	public void ServerInitialize(PlayerInfo launcher, Hittable homingTargetHittable, ItemUseId itemUseId)
	{
		if (!NetworkServer.active)
		{
			Debug.LogWarning("[Server] function 'System.Void Rocket::ServerInitialize(PlayerInfo,Hittable,ItemUseId)' called when server was not active");
			return;
		}
		Networklauncher = launcher;
		NetworkhomingTargetHittable = homingTargetHittable;
		this.itemUseId = itemUseId;
	}

	public void OnFixedBUpdate()
	{
		if (base.isServer && CheckCollision(out var explosionPosition))
		{
			ServerExplode(explosionPosition);
			return;
		}
		ApplyHoming();
		if (base.isServer)
		{
			distanceTravelled += asEntity.Rigidbody.linearVelocity.magnitude * Time.fixedDeltaTime;
			if (distanceTravelled > GameManager.ItemSettings.RocketMaxTravelDistance)
			{
				ServerExplode(base.transform.position);
			}
		}
		previousPosition = asEntity.Rigidbody.position;
		void ApplyHoming()
		{
			if (!(NetworkhomingTargetHittable == null))
			{
				Vector3 target = NetworkhomingTargetHittable.AsEntity.AsLockOnTarget.GetLockOnPosition() - asEntity.Rigidbody.worldCenterOfMass;
				float timeSince = BMath.GetTimeSince(launchOrReflectionTimestamp);
				float maxRadiansDelta = BMath.Min(1f, timeSince / GameManager.ItemSettings.RocketPostLaunchFullHomingTime) * GameManager.ItemSettings.RocketMaxVelocityRotationPerSecond * Time.fixedDeltaTime * (MathF.PI / 180f);
				Vector3 linearVelocity = asEntity.Rigidbody.linearVelocity;
				linearVelocity = Vector3.RotateTowards(linearVelocity, target, maxRadiansDelta, 0f);
				asEntity.Rigidbody.linearVelocity = linearVelocity;
				if (drawRocketDebug)
				{
					BDebug.DrawLine(base.transform.position, NetworkhomingTargetHittable.AsEntity.AsLockOnTarget.GetLockOnPosition(), Color.red);
				}
			}
		}
		bool CheckCollision(out Vector3 reference)
		{
			bool flag = Networklauncher != null && BMath.GetTimeSince(launchOrReflectionTimestamp) < 0.25f;
			int num = Physics.OverlapSphereNonAlloc(previousPosition, collisionRadius, layerMask: GameManager.LayerSettings.RocketHittablesMask, results: PlayerGolfer.overlappingColliderBuffer, queryTriggerInteraction: QueryTriggerInteraction.Ignore);
			reference = previousPosition;
			bool flag2 = false;
			for (int i = 0; i < num; i++)
			{
				Collider collider = PlayerGolfer.overlappingColliderBuffer[i];
				if (!flag || (!collider.TryGetComponentInParent<PlayerInfo>(out var foundComponent, includeInactive: true) && !(foundComponent != Networklauncher)))
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
			int num2 = Physics.SphereCastNonAlloc(radius: collisionRadius, maxDistance: magnitude, layerMask: GameManager.LayerSettings.RocketHittablesMask, ray: ray, results: PlayerGolfer.raycastHitBuffer, queryTriggerInteraction: QueryTriggerInteraction.Ignore);
			RaycastHit raycastHit = new RaycastHit
			{
				distance = float.MaxValue
			};
			PlayerInfo playerInfo = null;
			for (int j = 0; j < num2; j++)
			{
				RaycastHit raycastHit2 = PlayerGolfer.raycastHitBuffer[j];
				if ((!flag || !raycastHit2.collider.TryGetComponentInParent<PlayerInfo>(out var foundComponent3, includeInactive: true) || !(foundComponent3 == Networklauncher)) && !(raycastHit2.distance >= raycastHit.distance))
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
			if (!(shieldOwner == Networklauncher))
			{
				NetworkhomingTargetHittable = ((Networklauncher != null) ? Networklauncher.AsHittable : null);
				Networklauncher = shieldOwner;
				isReflected = true;
				Vector3 normalized = (shieldCollider.transform.position - hitWorldPosition).normalized;
				float num = Vector3.Dot(asEntity.Rigidbody.linearVelocity, normalized);
				if (!(num <= 0f))
				{
					Vector3 position = shieldCollider.transform.position - normalized * (shieldCollider.radius + collisionRadius + 0.1f);
					base.transform.position = position;
					asEntity.Rigidbody.position = position;
					Vector3 vector = asEntity.Rigidbody.linearVelocity - 2f * num * normalized;
					if (NetworkhomingTargetHittable != null)
					{
						vector = Vector3.RotateTowards(vector, Vector3.up, GameManager.ItemSettings.ElectromagnetShieldRocketReflectTiltUpMaxAngle * (MathF.PI / 180f), 0f);
					}
					asEntity.Rigidbody.linearVelocity = vector;
					launchOrReflectionTimestamp = Time.timeAsDouble;
					shieldOwner.PlayElectromagnetShieldHitForAllClients(hitWorldPosition - shieldOwner.ElectromagnetShieldCollider.transform.position);
				}
			}
		}
	}

	public void OnLateBUpdate()
	{
		Quaternion rotation = Quaternion.LookRotation(asEntity.Rigidbody.linearVelocity, base.transform.up);
		asEntity.Rigidbody.rotation = rotation;
		base.transform.rotation = rotation;
	}

	[Server]
	private void ServerExplode(Vector3 worldPosition)
	{
		if (!NetworkServer.active)
		{
			Debug.LogWarning("[Server] function 'System.Void Rocket::ServerExplode(UnityEngine.Vector3)' called when server was not active");
		}
		else
		{
			if (asEntity.IsDestroyed)
			{
				return;
			}
			int num = Physics.OverlapSphereNonAlloc(worldPosition, GameManager.ItemSettings.RocketExplosionRange, layerMask: GameManager.LayerSettings.RocketHittablesMask, results: PlayerGolfer.overlappingColliderBuffer, queryTriggerInteraction: QueryTriggerInteraction.Ignore);
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
					foundComponent.HitWithItem(ItemType.RocketLauncher, itemUseId, foundComponent.transform.InverseTransformPoint(closestPoint), vector, foundComponent.transform.InverseTransformPoint(worldPosition), BMath.Sqrt(distanceSquared), (Networklauncher != null) ? Networklauncher.Inventory : null, isReflected, isInSpecialState: false, canHitWithNoUser: true);
					if (drawRocketDebug)
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
			if (drawRocketDebug)
			{
				BDebug.DrawWireSphere(worldPosition, GameManager.ItemSettings.RocketExplosionRange, Color.red, 5f, drawInsideLines: true);
			}
			asEntity.DestroyEntity();
		}
	}

	[TargetRpc]
	private void RpcInformExploded(NetworkConnectionToClient connection, Vector3 worldPosition)
	{
		NetworkWriterPooled writer = NetworkWriterPool.Get();
		writer.WriteVector3(worldPosition);
		SendTargetRPCInternal(connection, "System.Void Rocket::RpcInformExploded(Mirror.NetworkConnectionToClient,UnityEngine.Vector3)", 888721830, writer, 0);
		NetworkWriterPool.Return(writer);
	}

	private void UpdateHomingWarning()
	{
		bool isPlayingHomingWarning = IsPlayingHomingWarning;
		IsPlayingHomingWarning = ShouldPlayWarning();
		if (IsPlayingHomingWarning && homingWarningWorldspaceIcon != null)
		{
			homingWarningWorldspaceIcon.SetDistanceReference(GetWorldspaceIconDistanceReference());
		}
		if (IsPlayingHomingWarning != isPlayingHomingWarning)
		{
			if (homingWarningSoundInstance.isValid())
			{
				homingWarningSoundInstance.stop(FMOD.Studio.STOP_MODE.ALLOWFADEOUT);
			}
			if (homingWarningUpdateRoutine != null)
			{
				StopCoroutine(homingWarningUpdateRoutine);
			}
			if (IsPlayingHomingWarning)
			{
				homingWarningWorldspaceIcon = WorldspaceIconManager.GetUnusedIcon();
				homingWarningWorldspaceIcon.Initialize(WorldspaceIconManager.HomingWarningIconSettings, asEntity.HasTargetReticlePosition ? asEntity.TargetReticlePosition.transform : base.transform, GetWorldspaceIconDistanceReference(), WorldspaceIconManager.HomingWarningIcon);
				homingWarningSoundInstance = RuntimeManager.CreateInstance(GameManager.AudioSettings.HomingWarningEvent);
				UpdateHomingWarningSound();
				homingWarningSoundInstance.start();
				homingWarningSoundInstance.release();
				homingWarningUpdateRoutine = StartCoroutine(HomingWarningUpdateRoutine());
			}
			else
			{
				RemoveWorldspaceIcon();
			}
		}
		static Transform GetWorldspaceIconDistanceReference()
		{
			if (!GameManager.LocalPlayerAsSpectator.IsSpectating)
			{
				return GameManager.LocalPlayerAsGolfer.transform;
			}
			return GameManager.LocalPlayerAsSpectator.Target;
		}
		IEnumerator HomingWarningUpdateRoutine()
		{
			while (homingWarningSoundInstance.isValid())
			{
				yield return null;
				UpdateHomingWarningSound();
			}
		}
		bool ShouldPlayWarning()
		{
			if (NetworkhomingTargetHittable == null)
			{
				return false;
			}
			PlayerInfo viewedOrLocalPlayer = GameManager.GetViewedOrLocalPlayer();
			if (viewedOrLocalPlayer == null)
			{
				return false;
			}
			if (NetworkhomingTargetHittable == viewedOrLocalPlayer.AsHittable)
			{
				return true;
			}
			if (viewedOrLocalPlayer.ActiveGolfCartSeat.IsValid() && NetworkhomingTargetHittable == viewedOrLocalPlayer.ActiveGolfCartSeat.golfCart.AsEntity.AsHittable)
			{
				return true;
			}
			return false;
		}
		void UpdateHomingWarningSound()
		{
			Vector3 vector = base.transform.position - NetworkhomingTargetHittable.transform.position;
			float magnitude = vector.magnitude;
			float value = BMath.InverseLerpClamped(GameManager.AudioSettings.HomingProjectileWarningMinDistance, GameManager.AudioSettings.HomingProjectileWarningMaxDistance, magnitude);
			homingWarningSoundInstance.setParameterByID(AudioSettings.DistanceId, value);
			float value2 = Vector3.Dot(vector / magnitude, GameManager.Camera.transform.right);
			homingWarningSoundInstance.setParameterByID(AudioSettings.PanningId, value2);
		}
	}

	private void RemoveWorldspaceIcon()
	{
		if (!(homingWarningWorldspaceIcon == null))
		{
			WorldspaceIconManager.ReturnIcon(homingWarningWorldspaceIcon);
			homingWarningWorldspaceIcon = null;
		}
	}

	private void OnExploded(Vector3 worldPosition)
	{
		if (trailVfx != null)
		{
			trailVfx.transform.SetParent(null, worldPositionStays: true);
			trailVfx.Stop();
			trailVfx = null;
		}
		VfxManager.PlayPooledVfxLocalOnly(VfxType.RocketLauncherRocketExplosion, worldPosition, Quaternion.identity);
		if (GameplayCameraManager.ShouldPlayImpactFrameForExplosion(worldPosition, GameManager.ItemSettings.RocketExplosionRange, GameManager.CameraGameplaySettings.RocketExplosionImpactFrameDistanceSquared))
		{
			CameraModuleController.PlayImpactFrame(worldPosition);
		}
		CameraModuleController.Shake(GameManager.CameraGameplaySettings.RocketExplosionScreenshakeSettings, worldPosition);
		RuntimeManager.PlayOneShot(GameManager.AudioSettings.RocketExplosionEvent, worldPosition);
	}

	private void OnHomingTargetHittableChanged(Hittable previousTarget, Hittable currentTarget)
	{
		UpdateHomingWarning();
	}

	private void OnLocalPlayerIsInGolfCartChanged()
	{
		UpdateHomingWarning();
	}

	private void OnLocalPlayerIsSpectatingChanged()
	{
		UpdateHomingWarning();
	}

	private void OnLocalPlayerSetSpectatingTarget(bool isInitialTarget)
	{
		UpdateHomingWarning();
	}

	private void OnLocalPlayerSpectatingTargetActiveGolfCartSeatChanged()
	{
		UpdateHomingWarning();
	}

	public Rocket()
	{
		_Mirror_SyncVarHookDelegate_homingTargetHittable = OnHomingTargetHittableChanged;
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
			((Rocket)obj).UserCode_RpcInformExploded__NetworkConnectionToClient__Vector3(null, reader.ReadVector3());
		}
	}

	static Rocket()
	{
		RemoteProcedureCalls.RegisterRpc(typeof(Rocket), "System.Void Rocket::RpcInformExploded(Mirror.NetworkConnectionToClient,UnityEngine.Vector3)", InvokeUserCode_RpcInformExploded__NetworkConnectionToClient__Vector3);
	}

	public override void SerializeSyncVars(NetworkWriter writer, bool forceAll)
	{
		base.SerializeSyncVars(writer, forceAll);
		if (forceAll)
		{
			writer.WriteNetworkBehaviour(Networklauncher);
			writer.WriteNetworkBehaviour(NetworkhomingTargetHittable);
			return;
		}
		writer.WriteVarULong(syncVarDirtyBits);
		if ((syncVarDirtyBits & 1L) != 0L)
		{
			writer.WriteNetworkBehaviour(Networklauncher);
		}
		if ((syncVarDirtyBits & 2L) != 0L)
		{
			writer.WriteNetworkBehaviour(NetworkhomingTargetHittable);
		}
	}

	public override void DeserializeSyncVars(NetworkReader reader, bool initialState)
	{
		base.DeserializeSyncVars(reader, initialState);
		if (initialState)
		{
			GeneratedSyncVarDeserialize_NetworkBehaviour(ref launcher, null, reader, ref ___launcherNetId);
			GeneratedSyncVarDeserialize_NetworkBehaviour(ref homingTargetHittable, _Mirror_SyncVarHookDelegate_homingTargetHittable, reader, ref ___homingTargetHittableNetId);
			return;
		}
		long num = (long)reader.ReadVarULong();
		if ((num & 1L) != 0L)
		{
			GeneratedSyncVarDeserialize_NetworkBehaviour(ref launcher, null, reader, ref ___launcherNetId);
		}
		if ((num & 2L) != 0L)
		{
			GeneratedSyncVarDeserialize_NetworkBehaviour(ref homingTargetHittable, _Mirror_SyncVarHookDelegate_homingTargetHittable, reader, ref ___homingTargetHittableNetId);
		}
	}
}
