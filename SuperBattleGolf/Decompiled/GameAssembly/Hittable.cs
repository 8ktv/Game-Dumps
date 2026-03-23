#define DEBUG_DRAW
using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using Cysharp.Threading.Tasks;
using FMOD.Studio;
using FMODUnity;
using Mirror;
using Mirror.RemoteCalls;
using UnityEngine;

public class Hittable : NetworkBehaviour, IFixedBUpdateCallback, IAnyBUpdateCallback
{
	private const float maxCollisionVfxTimeWindow = 0.2f;

	private const int maxCollisionVfxInTimeWindow = 3;

	[SerializeField]
	private HittableSettings settings;

	[SerializeField]
	private HittableVfxSettings vfxSettings;

	private PlayerGolfer responsibleSwingProjectilePlayer;

	[SyncVar]
	private Vector3 swingHitPosition;

	private Vector3 swingProjectilePreviousVelocity;

	private Vector3 swingProjectilePreviousAngularVelocity;

	private Vector3 previousVelocity;

	[SyncVar(hook = "OnHomingTargetHittableChanged")]
	private Hittable homingTargetHittable;

	private LockOnTarget homingTarget;

	[SyncVar]
	private float homingInitialHorizontalDistance;

	[SyncVar(hook = "OnSideSpinChanged")]
	private float sideSpin;

	private bool isUpdateLoopRegistered;

	private WorldspaceIconUi homingWarningWorldspaceIcon;

	private EventInstance homingWarningSoundInstance;

	private Coroutine homingWarningUpdateRoutine;

	private ProjectileTrail projectileTrailVfx;

	private int recentCollisionVfxCount;

	private double recentCollisionVfxStartTimestamp = double.MinValue;

	private static readonly HashSet<(Hittable, Hittable)> resolvedCollisionVfxHittablePairs;

	private static readonly HashSet<(Hittable, Rigidbody)> resolvedCollisionVfxNonHittablePairs;

	private static readonly HashSet<(Hittable, GameObject)> resolvedCollisionVfxNonRigidbodyPairs;

	private static bool isResettingResolvedCollisionVfxPairs;

	private AntiCheatPerPlayerRateChecker serverHitWithGolfSwingCommandRateLimiter;

	private AntiCheatPerPlayerRateChecker serverHitWithSwingProjectileCommandRateLimiter;

	private AntiCheatPerPlayerRateChecker serverHitWithDiveCommandRateLimiter;

	private AntiCheatPerPlayerRateChecker serverHitWithItemCommandRateLimiter;

	private AntiCheatPerPlayerRateChecker serverHitWithRocketBackBlastCommandRateLimiter;

	private AntiCheatPerPlayerRateChecker serverRequestInitialStateCommandRateLimiter;

	[CVar("drawHomingProjectileDebug", "", "", false, true)]
	private static bool drawHomingProjectileDebug;

	protected NetworkBehaviourSyncVar ___homingTargetHittableNetId;

	public Action<Hittable, Hittable> _Mirror_SyncVarHookDelegate_homingTargetHittable;

	public Action<float, float> _Mirror_SyncVarHookDelegate_sideSpin;

	public Entity AsEntity { get; private set; }

	public SwingProjectileState SwingProjectileState { get; private set; }

	public float SideSpin => sideSpin;

	public bool IsPlayingHomingWarning { get; private set; }

	public SwingHittableSettings SwingSettings => settings.Swing;

	public ProjectileHittableSettings ProjectileSettings => settings.Projectile;

	public DiveHittableSettings DiveSettings => settings.Dive;

	public ItemHittableSettings ItemSettings => settings.Item;

	public PlayerGolfer ResponsibleSwingProjectilePlayer => responsibleSwingProjectilePlayer;

	public Vector3 NetworkswingHitPosition
	{
		get
		{
			return swingHitPosition;
		}
		[param: In]
		set
		{
			GeneratedSyncVarSetter(value, ref swingHitPosition, 1uL, null);
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

	public float NetworkhomingInitialHorizontalDistance
	{
		get
		{
			return homingInitialHorizontalDistance;
		}
		[param: In]
		set
		{
			GeneratedSyncVarSetter(value, ref homingInitialHorizontalDistance, 4uL, null);
		}
	}

	public float NetworksideSpin
	{
		get
		{
			return sideSpin;
		}
		[param: In]
		set
		{
			GeneratedSyncVarSetter(value, ref sideSpin, 8uL, _Mirror_SyncVarHookDelegate_sideSpin);
		}
	}

	public event Action<PlayerGolfer, float, Vector3, Vector3, Vector3> WillApplyGolfSwingHitPhysics;

	public event Action<PlayerGolfer, Vector3, float> WasHitByGolfSwing;

	public event Action<Hittable, Vector3, Vector3, Vector3> WillApplySwingProjectileHitPhysics;

	public event Action<Hittable, Vector3> WasHitBySwingProjectile;

	public event Action<PlayerMovement, Vector3> WillApplyDiveHitPhysics;

	public event Action<PlayerMovement> WasHitByDive;

	public event Action<PlayerInventory, ItemType, ItemUseId, Vector3, float, Vector3, bool, bool> WillApplyItemHitPhysics;

	public event Action<PlayerInventory, ItemType, ItemUseId, Vector3, float, bool> WasHitByItem;

	public event Action<PlayerInventory, Vector3, Vector3, Vector3> WillApplyRocketLauncherBackBlastHitPhysics;

	public event Action<PlayerInventory, Vector3> WasHitByRocketLauncherBackBlast;

	public event Action<Vector3> WillApplyReturnedBallHitPhysics;

	public event Action WasHitByReturnedBall;

	public event Action WillApplyScoreKnockbackPhysics;

	public event Action WasHitByScoreKnockback;

	public event Action<Hittable> HitAsSwingProjectile;

	public event Action IsPlayingHomingWarningChanged;

	private void Awake()
	{
		AsEntity = GetComponent<Entity>();
		if (SwingSettings.CanBecomeSwingProjectile && vfxSettings != null && vfxSettings.ProjectileTrail != VfxType.None && VfxPersistentData.TryGetPooledVfx(vfxSettings.ProjectileTrail, out var particleSystem))
		{
			if (!particleSystem.TryGetComponent<ProjectileTrail>(out projectileTrailVfx))
			{
				Debug.LogError("Pooled VFX does not have the ProjectileTrail component");
				particleSystem.ReturnToPool();
			}
			else
			{
				projectileTrailVfx.Initialize(this);
			}
		}
		PlayerInfo.LocalPlayerIsInGolfCartChanged += OnLocalPlayerIsInGolfCartChanged;
		PlayerSpectator.LocalPlayerIsSpectatingChanged += OnLocalPlayerIsSpectatingChanged;
		PlayerSpectator.LocalPlayerSetSpectatingTarget += OnLocalPlayerSetSpectatingTarget;
		PlayerSpectator.LocalPlayerSpectatingTargetIsInGolfCartChanged += OnLocalPlayerSpectatingTargetActiveGolfCartSeatChanged;
	}

	private void Start()
	{
		if (AsEntity.HasRigidbody)
		{
			AsEntity.Rigidbody.maxAngularVelocity = BMath.Max(AsEntity.Rigidbody.maxAngularVelocity, SwingSettings.SwingHitSpinSpeed, SwingSettings.ProjectilePostHitSpinSpeed);
		}
	}

	public void OnWillBeDestroyed()
	{
		if (projectileTrailVfx != null)
		{
			projectileTrailVfx.ReturnToPool();
			projectileTrailVfx = null;
		}
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
		serverHitWithGolfSwingCommandRateLimiter = new AntiCheatPerPlayerRateChecker(base.name + " hit with golf swing", 0.5f, 5, 10, 2f);
		serverHitWithSwingProjectileCommandRateLimiter = new AntiCheatPerPlayerRateChecker(base.name + " hit with swing projectile", 0.5f, 5, 10, 2f);
		serverHitWithDiveCommandRateLimiter = new AntiCheatPerPlayerRateChecker(base.name + " hit with dive", 0.5f, 5, 10, 2f);
		serverHitWithItemCommandRateLimiter = new AntiCheatPerPlayerRateChecker(base.name + " hit with item", 0.5f, 5, 10, 2f);
		serverHitWithRocketBackBlastCommandRateLimiter = new AntiCheatPerPlayerRateChecker(base.name + " hit with rocket back blast", 0.5f, 5, 10, 2f);
		serverRequestInitialStateCommandRateLimiter = new AntiCheatPerPlayerRateChecker(base.name + " request intial hittable state", 100000f, 2, 2, 200000f);
	}

	public override void OnStopServer()
	{
		if (isUpdateLoopRegistered)
		{
			BUpdate.DeregisterCallback(this);
		}
	}

	public override void OnStartClient()
	{
		if (!base.isServer)
		{
			CmdRequestInitialState();
		}
	}

	public override void OnStopClient()
	{
		if (isUpdateLoopRegistered)
		{
			BUpdate.DeregisterCallback(this);
		}
	}

	private void OnCollisionEnter(Collision collision)
	{
		Hittable hitHittable;
		if (collision.collider.attachedRigidbody != null)
		{
			if (!collision.collider.attachedRigidbody.TryGetComponentInParent<Hittable>(out hitHittable, includeInactive: true))
			{
				hitHittable = null;
			}
		}
		else if (!collision.collider.TryGetComponentInParent<Hittable>(out hitHittable, includeInactive: true))
		{
			hitHittable = null;
		}
		TryPlayCollisionVfx();
		if (NetworkServer.active)
		{
			ParseCollision(out var hitElectromagnetShield, out var hitHomingTarget);
			NetworksideSpin = 0f;
			if (!hitElectromagnetShield)
			{
				ServerClearHomingTarget(hitHomingTarget);
			}
		}
		void ParseCollision(out bool reference, out bool reference2)
		{
			reference = false;
			reference2 = false;
			if (SwingProjectileState != SwingProjectileState.None && !(hitHittable == null) && hitHittable.ProjectileSettings.CanBeHitBySwingProjectiles)
			{
				if (hitHittable.AsEntity.IsPlayer && hitHittable.AsEntity.PlayerInfo.IsElectromagnetShieldActive)
				{
					ServerReflectOffElectromagnetShield(collision.GetContact(0).point, hitHittable.AsEntity.PlayerInfo.ElectromagnetShieldCollider, hitHittable.AsEntity.PlayerInfo);
					reference = true;
				}
				else
				{
					ContactPoint contact = collision.GetContact(0);
					if (hitHittable.AsEntity.IsPlayer && hitHittable.AsEntity.PlayerInfo.IsElectromagnetShieldActive && Physics.GetIgnoreCollision(contact.thisCollider, contact.otherCollider))
					{
						reference = true;
					}
					else
					{
						reference2 = hitHittable == NetworkhomingTargetHittable;
						Vector3 localHitPosition = hitHittable.transform.InverseTransformPoint(contact.point);
						Vector3 pointVelocity = contact.point.GetPointVelocity(AsEntity.Rigidbody.worldCenterOfMass, swingProjectilePreviousVelocity, swingProjectilePreviousAngularVelocity);
						Vector3 networkedPointVelocity = hitHittable.AsEntity.GetNetworkedPointVelocity(contact.point);
						float value = BMath.Abs(Vector3.Dot(pointVelocity - networkedPointVelocity, contact.normal));
						float normalizedHitSpeed = BMath.InverseLerpClamped(SwingSettings.ProjectileMinHitCollisionSpeed, SwingSettings.ProjectileMaxHitCollisionSpeed, value);
						Vector3 worldHitDirection = -contact.normal;
						hitHittable.HitWithSwingProjectile(localHitPosition, worldHitDirection, normalizedHitSpeed, this, NetworkhomingTargetHittable != null, responsibleSwingProjectilePlayer);
						ServerStopBeingSwingProjectile();
						ServerApplyPostHitBounce(contact.normal);
						this.HitAsSwingProjectile?.Invoke(hitHittable);
					}
				}
			}
		}
		static async void ResetResolvedCollisionVfxPairsDelayed()
		{
			if (!isResettingResolvedCollisionVfxPairs)
			{
				isResettingResolvedCollisionVfxPairs = true;
				await UniTask.Yield(PlayerLoopTiming.FixedUpdate);
				resolvedCollisionVfxHittablePairs.Clear();
				resolvedCollisionVfxNonHittablePairs.Clear();
				resolvedCollisionVfxNonRigidbodyPairs.Clear();
				isResettingResolvedCollisionVfxPairs = false;
			}
		}
		void ServerReflectOffElectromagnetShield(Vector3 hitWorldPosition, SphereCollider shieldCollider, PlayerInfo shieldOwner)
		{
			if (!(shieldOwner == null) && !(shieldOwner == responsibleSwingProjectilePlayer.PlayerInfo))
			{
				NetworkhomingTargetHittable = ((NetworkhomingTargetHittable != null && responsibleSwingProjectilePlayer != null) ? responsibleSwingProjectilePlayer.PlayerInfo.AsHittable : null);
				if (homingTarget != null)
				{
					NetworkhomingInitialHorizontalDistance = (homingTarget.GetLockOnPosition() - AsEntity.Rigidbody.worldCenterOfMass).AsHorizontal2().magnitude;
				}
				BecomeSwingProjectile(shieldOwner.AsGolfer, isReflected: true);
				Vector3 normalized = (hitWorldPosition - shieldCollider.transform.position).normalized;
				Vector3 current;
				if (NetworkhomingTargetHittable != null)
				{
					current = SwingSettings.MaxPowerSwingHitSpeed * (1f + GameManager.GolfSettings.MaxSwingOvercharge) * normalized;
					current = Vector3.RotateTowards(current, Vector3.up, GameManager.ItemSettings.ElectromagnetShieldProjectileReflectTiltUpMaxAngle * (MathF.PI / 180f), 0f);
				}
				else
				{
					float maxLength = SwingSettings.MaxPowerSwingHitSpeed * (1f + GameManager.GolfSettings.MaxSwingOvercharge);
					current = Vector3.Reflect(previousVelocity, normalized) * GameManager.ItemSettings.ElectromagnetShieldProjectileNonHomingReflectSpeedFactor;
					current = Vector3.ClampMagnitude(current, maxLength);
				}
				AsEntity.Rigidbody.linearVelocity = current;
				NetworkswingHitPosition = base.transform.position;
				shieldOwner.PlayElectromagnetShieldHitForAllClients(hitWorldPosition - shieldOwner.ElectromagnetShieldCollider.transform.position);
			}
		}
		void TryPlayCollisionVfx()
		{
			if (!(vfxSettings == null) && vfxSettings.CollisionVfx != VfxType.None)
			{
				if (BMath.GetTimeSince(recentCollisionVfxStartTimestamp) >= 0.2f)
				{
					recentCollisionVfxCount = 0;
				}
				if (recentCollisionVfxCount <= 3 && AsEntity.IsSimulatingRigidbody() && collision.contactCount >= 1)
				{
					if (hitHittable != null)
					{
						if (!resolvedCollisionVfxHittablePairs.Add((this, hitHittable)))
						{
							return;
						}
						ResetResolvedCollisionVfxPairsDelayed();
						if (hitHittable.vfxSettings != null && vfxSettings.CollisionVfx == hitHittable.vfxSettings.CollisionVfx && resolvedCollisionVfxHittablePairs.Contains((hitHittable, this)))
						{
							return;
						}
					}
					else if (collision.rigidbody != null)
					{
						if (!resolvedCollisionVfxNonHittablePairs.Add((this, collision.rigidbody)))
						{
							return;
						}
						ResetResolvedCollisionVfxPairsDelayed();
					}
					else
					{
						if (!resolvedCollisionVfxNonRigidbodyPairs.Add((this, collision.collider.gameObject)))
						{
							return;
						}
						ResetResolvedCollisionVfxPairsDelayed();
					}
					ContactPoint contact = collision.GetContact(0);
					if (AsEntity.Rigidbody.linearVelocity.sqrMagnitude >= vfxSettings.CollisionMinimumSpeedSquared && Vector3.Dot(previousVelocity.normalized, -contact.normal) >= vfxSettings.CollisionMinimumAlignment)
					{
						VfxManager.PlayPooledVfxLocalOnly(vfxSettings.CollisionVfx, contact.point, Quaternion.LookRotation(contact.normal));
						if (recentCollisionVfxCount++ <= 0)
						{
							recentCollisionVfxStartTimestamp = Time.timeAsDouble;
						}
					}
				}
			}
		}
	}

	public void OnFixedBUpdate()
	{
		if (!AsEntity.IsGolfBall)
		{
			ApplySpinForce();
			ApplyAirDamping(PhysicsManager.Settings.ItemLinearAirDragFactor);
		}
		if (!base.isServer || SwingProjectileState == SwingProjectileState.None)
		{
			previousVelocity = AsEntity.Rigidbody.linearVelocity;
			return;
		}
		float num = (AsEntity.IsGrounded() ? SwingSettings.GroundedMinProjectileStopSpeedSquared : SwingSettings.AirMinProjectileStopSpeedSquared);
		if (homingTarget == null && AsEntity.Rigidbody.linearVelocity.sqrMagnitude < num)
		{
			ServerStopBeingSwingProjectile();
		}
		if (base.isServer)
		{
			swingProjectilePreviousVelocity = AsEntity.Rigidbody.linearVelocity;
			swingProjectilePreviousAngularVelocity = AsEntity.Rigidbody.angularVelocity;
		}
		previousVelocity = AsEntity.Rigidbody.linearVelocity;
	}

	public void ApplySpinForce()
	{
		UpdateHomingTarget(out var homingTargetPosition);
		if (homingTarget != null)
		{
			ApplyHomingSpin(homingTargetPosition);
		}
		else if (SideSpin != 0f)
		{
			ApplySideSpin();
		}
		void ApplyHomingSpin(Vector3 vector)
		{
			float value = (vector - AsEntity.Rigidbody.worldCenterOfMass).AsHorizontal2().magnitude / homingInitialHorizontalDistance;
			float num = BMath.InverseLerpClamped(1f, GameManager.GolfSettings.HomingProjectileInitialHorizontalDistanceFractionMaxHoming, value);
			Vector3 linearVelocity = AsEntity.Rigidbody.linearVelocity;
			float magnitude = linearVelocity.magnitude;
			Vector3 networkedVelocity = homingTarget.AsEntity.GetNetworkedVelocity();
			float num2 = BMath.Min((vector - AsEntity.Rigidbody.worldCenterOfMass).magnitude / magnitude, 0.5f);
			linearVelocity = Vector3.RotateTowards(target: vector + networkedVelocity * num2 - AsEntity.Rigidbody.worldCenterOfMass, maxRadiansDelta: num * GameManager.GolfSettings.HomingProjectileMaxVelocityRotationPerSecond * Time.fixedDeltaTime * (MathF.PI / 180f), current: linearVelocity, maxMagnitudeDelta: 0f);
			float num3 = SwingSettings.MaxPowerSwingHitSpeed - magnitude;
			float b = num * GameManager.GolfSettings.HomingProjectileMaxAcceleration * Time.fixedDeltaTime;
			float num4 = (float)BMath.Sign(num3) * BMath.Min(BMath.Abs(num3), b);
			linearVelocity = linearVelocity.normalized * (magnitude + num4);
			AsEntity.Rigidbody.linearVelocity = linearVelocity;
			if (drawHomingProjectileDebug)
			{
				BDebug.DrawLine(base.transform.position, vector, Color.red);
			}
		}
		void ApplySideSpin()
		{
			Vector3 vector = AsEntity.Rigidbody.linearVelocity.Horizontalized();
			float magnitude = vector.magnitude;
			if (!(magnitude <= 0f))
			{
				float y = vector.GetYawDeg() + (float)BMath.Sign(SideSpin) * 90f;
				Vector3 vector2 = Quaternion.Euler(0f, y, 0f) * vector / magnitude;
				float num = magnitude * BMath.Abs(SideSpin);
				AsEntity.Rigidbody.linearVelocity += num * vector2;
			}
		}
		void UpdateHomingTarget(out Vector3 reference)
		{
			if (homingTarget == null)
			{
				reference = default(Vector3);
			}
			else
			{
				reference = homingTarget.GetLockOnPosition();
				if (base.isServer)
				{
					Vector3 planeNormal = reference - swingHitPosition;
					if (AsEntity.Rigidbody.worldCenterOfMass.SignedDistanceFromPlane(reference, planeNormal) > GameManager.GolfSettings.HomingProjectileCancelDistancePastTarget)
					{
						ServerClearHomingTarget(dueToHomingTargetHit: false);
					}
				}
			}
		}
	}

	public void ApplyAirDamping(float linearAirDragFactor)
	{
		float sqrMagnitude = AsEntity.Rigidbody.linearVelocity.sqrMagnitude;
		float num = linearAirDragFactor * sqrMagnitude;
		AsEntity.Rigidbody.linearVelocity *= BMath.Max(0f, 1f - num * Time.fixedDeltaTime);
	}

	public void ServerApplyPostHitBounce(Vector3 hitNormal)
	{
		if (!base.isServer)
		{
			return;
		}
		ApplyPostHitBounceInternal(hitNormal);
		foreach (NetworkConnectionToClient value in NetworkServer.connections.Values)
		{
			if (value != NetworkServer.localConnection)
			{
				RpcApplyPostHitBounce(value, hitNormal);
			}
		}
	}

	[TargetRpc]
	private void RpcApplyPostHitBounce(NetworkConnectionToClient connection, Vector3 hitNormal)
	{
		NetworkWriterPooled writer = NetworkWriterPool.Get();
		writer.WriteVector3(hitNormal);
		SendTargetRPCInternal(connection, "System.Void Hittable::RpcApplyPostHitBounce(Mirror.NetworkConnectionToClient,UnityEngine.Vector3)", -1428442210, writer, 0);
		NetworkWriterPool.Return(writer);
	}

	private void ApplyPostHitBounceInternal(Vector3 hitNormal)
	{
		Vector3 vector = Vector3.Cross(Vector3.up, hitNormal.normalized);
		Quaternion rotation = Quaternion.LookRotation(vector);
		AsEntity.Rigidbody.linearVelocity = SwingSettings.ProjectilePostHitBounceSpeed * Vector3.up;
		AsEntity.Rigidbody.angularVelocity = SwingSettings.ProjectilePostHitSpinSpeed * vector;
		VfxManager.PlayPooledVfxLocalOnly(VfxType.ItemPostHitSpin, base.transform.position, rotation, Vector3.one, base.netId);
	}

	public void OnWillTeleport()
	{
		if (projectileTrailVfx != null)
		{
			projectileTrailVfx.BeforeTeleport();
		}
	}

	public void OnTeleported()
	{
		if (projectileTrailVfx != null)
		{
			projectileTrailVfx.AfterTeleport();
		}
	}

	private bool IsUnhittable()
	{
		if (AsEntity.IsGolfBall && AsEntity.AsGolfBall.IsStationary)
		{
			return true;
		}
		return false;
	}

	public void HitWithGolfSwing(Vector3 localHitPosition, Vector3 localOrigin, Vector3 worldDirection, bool isPutt, float power, float sideSpin, PlayerGolfer hitter, Hittable homingTargetHittable)
	{
		if (IsHittableBySwing(hitter))
		{
			HitWithGolfSwingInternal(localHitPosition, localOrigin, worldDirection, isPutt, power, sideSpin, hitter, homingTargetHittable);
			CmdHitWithGolfSwing(localHitPosition, localOrigin, worldDirection, isPutt, power, sideSpin, hitter, homingTargetHittable);
		}
	}

	[Command(requiresAuthority = false)]
	private void CmdHitWithGolfSwing(Vector3 localHitPosition, Vector3 localOrigin, Vector3 worldDirection, bool isPutt, float power, float sideSpin, PlayerGolfer hitter, Hittable homingTargetHittable, NetworkConnectionToClient sender = null)
	{
		if (base.isServer && base.isClient)
		{
			UserCode_CmdHitWithGolfSwing__Vector3__Vector3__Vector3__Boolean__Single__Single__PlayerGolfer__Hittable__NetworkConnectionToClient(localHitPosition, localOrigin, worldDirection, isPutt, power, sideSpin, hitter, homingTargetHittable, sender);
			return;
		}
		NetworkWriterPooled writer = NetworkWriterPool.Get();
		writer.WriteVector3(localHitPosition);
		writer.WriteVector3(localOrigin);
		writer.WriteVector3(worldDirection);
		writer.WriteBool(isPutt);
		writer.WriteFloat(power);
		writer.WriteFloat(sideSpin);
		writer.WriteNetworkBehaviour(hitter);
		writer.WriteNetworkBehaviour(homingTargetHittable);
		SendCommandInternal("System.Void Hittable::CmdHitWithGolfSwing(UnityEngine.Vector3,UnityEngine.Vector3,UnityEngine.Vector3,System.Boolean,System.Single,System.Single,PlayerGolfer,Hittable,Mirror.NetworkConnectionToClient)", 650517749, writer, 0, requiresAuthority: false);
		NetworkWriterPool.Return(writer);
	}

	[TargetRpc]
	private void RpcHitWithGolfSwing(NetworkConnectionToClient connection, Vector3 localHitPosition, Vector3 localOrigin, Vector3 worldDirection, bool isPutt, float power, float sideSpin, PlayerGolfer hitter, Hittable homingTargetHittable)
	{
		NetworkWriterPooled writer = NetworkWriterPool.Get();
		writer.WriteVector3(localHitPosition);
		writer.WriteVector3(localOrigin);
		writer.WriteVector3(worldDirection);
		writer.WriteBool(isPutt);
		writer.WriteFloat(power);
		writer.WriteFloat(sideSpin);
		writer.WriteNetworkBehaviour(hitter);
		writer.WriteNetworkBehaviour(homingTargetHittable);
		SendTargetRPCInternal(connection, "System.Void Hittable::RpcHitWithGolfSwing(Mirror.NetworkConnectionToClient,UnityEngine.Vector3,UnityEngine.Vector3,UnityEngine.Vector3,System.Boolean,System.Single,System.Single,PlayerGolfer,Hittable)", -1196171200, writer, 0);
		NetworkWriterPool.Return(writer);
	}

	private void HitWithGolfSwingInternal(Vector3 localHitPosition, Vector3 localOrigin, Vector3 worldDirection, bool isPutt, float power, float sideSpin, PlayerGolfer hitter, Hittable homingTargetHittable)
	{
		if (hitter == null)
		{
			return;
		}
		AsEntity.TemporarilyIgnoreCollisionsWith(hitter.PlayerInfo.AsEntity, 0.25f);
		float num = GetSwingHitSpeed(isPutt, power) * MatchSetupRules.GetValue(MatchSetupRules.Rule.SwingPower);
		if (SwingSettings.CanBecomeSwingProjectile && num >= SwingSettings.MinProjectileSwingSpeed)
		{
			BecomeSwingProjectile(hitter, isReflected: false);
		}
		if (!AsEntity.IsGolfBall && !AsEntity.IsGolfTee)
		{
			VfxManager.PlayPooledVfxLocalOnly(hitter.ClubVfxSettings.Hit, base.transform.TransformPoint(localHitPosition), Quaternion.identity);
		}
		if (!CanApplyPhysics())
		{
			this.WasHitByGolfSwing?.Invoke(hitter, worldDirection, power);
			return;
		}
		if (base.isServer)
		{
			NetworkswingHitPosition = base.transform.position;
			if (CanApplySpin())
			{
				NetworksideSpin = sideSpin;
				NetworkhomingTargetHittable = homingTargetHittable;
				if (homingTarget != null)
				{
					NetworkhomingInitialHorizontalDistance = (homingTarget.GetLockOnPosition() - AsEntity.Rigidbody.worldCenterOfMass).AsHorizontal2().magnitude;
				}
			}
		}
		Vector3 normalized = worldDirection.normalized;
		Vector3 vector = normalized * num;
		this.WillApplyGolfSwingHitPhysics?.Invoke(hitter, power, localHitPosition, localOrigin, vector);
		AsEntity.Rigidbody.linearVelocity += vector;
		AsEntity.Rigidbody.angularVelocity += SwingSettings.SwingHitSpinSpeed * Vector3.Cross(normalized, Vector3.up);
		this.WasHitByGolfSwing?.Invoke(hitter, worldDirection, power);
		bool CanApplySpin()
		{
			if (!AsEntity.HasRigidbody)
			{
				return false;
			}
			if (AsEntity.IsGolfBall)
			{
				return true;
			}
			if (AsEntity.IsItem)
			{
				return true;
			}
			return false;
		}
	}

	private bool IsHittableBySwing(PlayerGolfer hitter)
	{
		if (!MatchSetupRules.GetValueAsBool(MatchSetupRules.Rule.HitOtherPlayersBalls) && AsEntity.IsGolfBall && AsEntity.AsGolfBall.Owner != hitter)
		{
			return false;
		}
		return true;
	}

	public void HitWithSwingProjectile(Vector3 localHitPosition, Vector3 worldHitDirection, float normalizedHitSpeed, Hittable hitter, bool wasHoming, PlayerGolfer responsiblePlayer)
	{
		if (!IsUnhittable())
		{
			HitWithSwingProjectileInternal(localHitPosition, worldHitDirection, normalizedHitSpeed, hitter, wasHoming, responsiblePlayer);
			CmdHitWithSwingProjectile(localHitPosition, worldHitDirection, normalizedHitSpeed, hitter, wasHoming, responsiblePlayer);
		}
	}

	[Command(requiresAuthority = false)]
	private void CmdHitWithSwingProjectile(Vector3 localHitPosition, Vector3 worldHitDirection, float normalizedHitSpeed, Hittable hitter, bool wasHoming, PlayerGolfer responsiblePlayer, NetworkConnectionToClient sender = null)
	{
		if (base.isServer && base.isClient)
		{
			UserCode_CmdHitWithSwingProjectile__Vector3__Vector3__Single__Hittable__Boolean__PlayerGolfer__NetworkConnectionToClient(localHitPosition, worldHitDirection, normalizedHitSpeed, hitter, wasHoming, responsiblePlayer, sender);
			return;
		}
		NetworkWriterPooled writer = NetworkWriterPool.Get();
		writer.WriteVector3(localHitPosition);
		writer.WriteVector3(worldHitDirection);
		writer.WriteFloat(normalizedHitSpeed);
		writer.WriteNetworkBehaviour(hitter);
		writer.WriteBool(wasHoming);
		writer.WriteNetworkBehaviour(responsiblePlayer);
		SendCommandInternal("System.Void Hittable::CmdHitWithSwingProjectile(UnityEngine.Vector3,UnityEngine.Vector3,System.Single,Hittable,System.Boolean,PlayerGolfer,Mirror.NetworkConnectionToClient)", -990257822, writer, 0, requiresAuthority: false);
		NetworkWriterPool.Return(writer);
	}

	[TargetRpc]
	private void RpcHitWithSwingProjectile(NetworkConnectionToClient connection, Vector3 localHitPosition, Vector3 worldHitDirection, float normalizedHitSpeed, Hittable hitter, bool wasHoming, PlayerGolfer responsiblePlayer)
	{
		NetworkWriterPooled writer = NetworkWriterPool.Get();
		writer.WriteVector3(localHitPosition);
		writer.WriteVector3(worldHitDirection);
		writer.WriteFloat(normalizedHitSpeed);
		writer.WriteNetworkBehaviour(hitter);
		writer.WriteBool(wasHoming);
		writer.WriteNetworkBehaviour(responsiblePlayer);
		SendTargetRPCInternal(connection, "System.Void Hittable::RpcHitWithSwingProjectile(Mirror.NetworkConnectionToClient,UnityEngine.Vector3,UnityEngine.Vector3,System.Single,Hittable,System.Boolean,PlayerGolfer)", -1332340089, writer, 0);
		NetworkWriterPool.Return(writer);
	}

	private void HitWithSwingProjectileInternal(Vector3 localHitPosition, Vector3 worldHitDirection, float normalizedHitSpeed, Hittable hitter, bool wasHoming, PlayerGolfer responsiblePlayer)
	{
		if (!(hitter == null))
		{
			worldHitDirection = worldHitDirection.normalized;
			AsEntity.TemporarilyIgnoreCollisionsWith(hitter.AsEntity, 1f);
			Vector3 vector = base.transform.TransformPoint(localHitPosition);
			VfxManager.PlayPooledVfxLocalOnly(VfxType.BasicBallHit, vector, Quaternion.LookRotation(-worldHitDirection));
			if (wasHoming && responsiblePlayer == GameManager.LocalPlayerAsGolfer)
			{
				TutorialManager.CompletePrompt(TutorialPrompt.HomingShot);
			}
			if (!CanApplyPhysics())
			{
				PlayHitSound(vector);
				this.WasHitBySwingProjectile?.Invoke(hitter, worldHitDirection);
				return;
			}
			Vector3 vector2 = BMath.LerpClamped(ProjectileSettings.SwingProjectileMinResultingSpeed, ProjectileSettings.SwingProjectileMaxResultingSpeed, normalizedHitSpeed) * worldHitDirection;
			this.WillApplySwingProjectileHitPhysics?.Invoke(hitter, localHitPosition, hitter.swingHitPosition, vector2);
			AsEntity.Rigidbody.AddForceAtPosition(vector2, vector, ForceMode.VelocityChange);
			PlayHitSound(vector);
			this.WasHitBySwingProjectile?.Invoke(hitter, worldHitDirection);
		}
	}

	private void BecomeSwingProjectile(PlayerGolfer responsiblePlayer, bool isReflected)
	{
		OnBecameSwingProjectile(responsiblePlayer, isReflected);
		if (!base.isServer)
		{
			return;
		}
		swingProjectilePreviousVelocity = AsEntity.Rigidbody.linearVelocity;
		swingProjectilePreviousAngularVelocity = AsEntity.Rigidbody.angularVelocity;
		foreach (NetworkConnectionToClient value in NetworkServer.connections.Values)
		{
			if (value != NetworkServer.localConnection)
			{
				RpcBecomeSwingProjectile(value, responsiblePlayer, isReflected);
			}
		}
	}

	[Server]
	public void ServerStopBeingSwingProjectile()
	{
		if (!NetworkServer.active)
		{
			Debug.LogWarning("[Server] function 'System.Void Hittable::ServerStopBeingSwingProjectile()' called when server was not active");
		}
		else
		{
			if (SwingProjectileState == SwingProjectileState.None)
			{
				return;
			}
			OnStoppedBeingSwingProjectile();
			foreach (NetworkConnectionToClient value in NetworkServer.connections.Values)
			{
				if (value != NetworkServer.localConnection)
				{
					RpcStopBeingSwingProjectile(value);
				}
			}
		}
	}

	[TargetRpc]
	private void RpcBecomeSwingProjectile(NetworkConnectionToClient connection, PlayerGolfer responsiblePlayer, bool isReflected)
	{
		NetworkWriterPooled writer = NetworkWriterPool.Get();
		writer.WriteNetworkBehaviour(responsiblePlayer);
		writer.WriteBool(isReflected);
		SendTargetRPCInternal(connection, "System.Void Hittable::RpcBecomeSwingProjectile(Mirror.NetworkConnectionToClient,PlayerGolfer,System.Boolean)", 332136187, writer, 0);
		NetworkWriterPool.Return(writer);
	}

	[TargetRpc]
	private void RpcStopBeingSwingProjectile(NetworkConnectionToClient connection)
	{
		NetworkWriterPooled writer = NetworkWriterPool.Get();
		SendTargetRPCInternal(connection, "System.Void Hittable::RpcStopBeingSwingProjectile(Mirror.NetworkConnectionToClient)", -1957084110, writer, 0);
		NetworkWriterPool.Return(writer);
	}

	private void OnBecameSwingProjectile(PlayerGolfer responsiblePlayer, bool isReflected)
	{
		SwingProjectileState = ((!isReflected) ? SwingProjectileState.Projectile : SwingProjectileState.ReflectedProjectile);
		responsibleSwingProjectilePlayer = responsiblePlayer;
		Rigidbody rigidbody = AsEntity.Rigidbody;
		rigidbody.includeLayers = (int)rigidbody.includeLayers | (int)GameManager.LayerSettings.ProjectileHittablesMask;
		AsEntity.TemporarilyIgnoreCollisionsWith(responsiblePlayer.PlayerInfo.Rigidbody, 0.1f);
		UpdateIsUpdateLoopRegistered();
	}

	private void OnStoppedBeingSwingProjectile()
	{
		SwingProjectileState = SwingProjectileState.None;
		responsibleSwingProjectilePlayer = null;
		Rigidbody rigidbody = AsEntity.Rigidbody;
		rigidbody.includeLayers = (int)rigidbody.includeLayers & ~(int)GameManager.LayerSettings.ProjectileHittablesMask;
		UpdateIsUpdateLoopRegistered();
	}

	public void HitWithDive(Vector3 relativeHitVelocity, PlayerMovement hitter)
	{
		if (DiveSettings.CanBeHit && !IsUnhittable())
		{
			HitWithDiveInternal(relativeHitVelocity, hitter);
			CmdHitWithDive(relativeHitVelocity, hitter);
		}
	}

	[Command(requiresAuthority = false)]
	private void CmdHitWithDive(Vector3 relativeHitVelocity, PlayerMovement hitter, NetworkConnectionToClient sender = null)
	{
		if (base.isServer && base.isClient)
		{
			UserCode_CmdHitWithDive__Vector3__PlayerMovement__NetworkConnectionToClient(relativeHitVelocity, hitter, sender);
			return;
		}
		NetworkWriterPooled writer = NetworkWriterPool.Get();
		writer.WriteVector3(relativeHitVelocity);
		writer.WriteNetworkBehaviour(hitter);
		SendCommandInternal("System.Void Hittable::CmdHitWithDive(UnityEngine.Vector3,PlayerMovement,Mirror.NetworkConnectionToClient)", -1813046725, writer, 0, requiresAuthority: false);
		NetworkWriterPool.Return(writer);
	}

	[TargetRpc]
	private void RpcHitWithDive(NetworkConnectionToClient connection, Vector3 relativeHitVelocity, PlayerMovement hitter)
	{
		NetworkWriterPooled writer = NetworkWriterPool.Get();
		writer.WriteVector3(relativeHitVelocity);
		writer.WriteNetworkBehaviour(hitter);
		SendTargetRPCInternal(connection, "System.Void Hittable::RpcHitWithDive(Mirror.NetworkConnectionToClient,UnityEngine.Vector3,PlayerMovement)", 1077696692, writer, 0);
		NetworkWriterPool.Return(writer);
	}

	private void HitWithDiveInternal(Vector3 relativeHitVelocity, PlayerMovement hitter)
	{
		if (!(hitter == null))
		{
			if (!CanApplyPhysics())
			{
				this.WasHitByDive?.Invoke(hitter);
				return;
			}
			Vector3 normalized = relativeHitVelocity.normalized;
			float magnitude = relativeHitVelocity.magnitude;
			float t = BMath.InverseLerpClamped(DiveSettings.MinKnockbackHitRelativeSpeed, DiveSettings.MaxKnockbackHitRelativeSpeed, magnitude);
			float num = BMath.Lerp(DiveSettings.MinKnockbackSpeed, DiveSettings.MaxKnockbackSpeed, t);
			float num2 = BMath.Lerp(DiveSettings.MinUpwardsSpeed, DiveSettings.MaxUpwardsSpeed, t);
			Vector3 vector = num * normalized + num2 * Vector3.up;
			this.WillApplyDiveHitPhysics?.Invoke(hitter, vector);
			AsEntity.Rigidbody.linearVelocity += vector;
			this.WasHitByDive?.Invoke(hitter);
		}
	}

	public void HitWithItem(ItemType itemType, ItemUseId itemUseId, Vector3 hitLocalPosition, Vector3 direction, Vector3 localOrigin, float distance, PlayerInventory itemUser, bool isReflected, bool isInSpecialState, bool canHitWithNoUser)
	{
		if (!IsUnhittable())
		{
			HitWithItemInternal(itemType, itemUseId, hitLocalPosition, direction, localOrigin, distance, itemUser, isReflected, isInSpecialState, canHitWithNoUser);
			CmdHitWithItem(itemType, itemUseId, hitLocalPosition, direction, localOrigin, distance, itemUser, isReflected, isInSpecialState, canHitWithNoUser);
		}
	}

	[Command(requiresAuthority = false)]
	private void CmdHitWithItem(ItemType itemType, ItemUseId itemUseId, Vector3 hitLocalPosition, Vector3 direction, Vector3 localOrigin, float distance, PlayerInventory itemUser, bool isReflected, bool isInSpecialState, bool canHitWithNoUser, NetworkConnectionToClient sender = null)
	{
		if (base.isServer && base.isClient)
		{
			UserCode_CmdHitWithItem__ItemType__ItemUseId__Vector3__Vector3__Vector3__Single__PlayerInventory__Boolean__Boolean__Boolean__NetworkConnectionToClient(itemType, itemUseId, hitLocalPosition, direction, localOrigin, distance, itemUser, isReflected, isInSpecialState, canHitWithNoUser, sender);
			return;
		}
		NetworkWriterPooled writer = NetworkWriterPool.Get();
		GeneratedNetworkCode._Write_ItemType(writer, itemType);
		GeneratedNetworkCode._Write_ItemUseId(writer, itemUseId);
		writer.WriteVector3(hitLocalPosition);
		writer.WriteVector3(direction);
		writer.WriteVector3(localOrigin);
		writer.WriteFloat(distance);
		writer.WriteNetworkBehaviour(itemUser);
		writer.WriteBool(isReflected);
		writer.WriteBool(isInSpecialState);
		writer.WriteBool(canHitWithNoUser);
		SendCommandInternal("System.Void Hittable::CmdHitWithItem(ItemType,ItemUseId,UnityEngine.Vector3,UnityEngine.Vector3,UnityEngine.Vector3,System.Single,PlayerInventory,System.Boolean,System.Boolean,System.Boolean,Mirror.NetworkConnectionToClient)", 1276951017, writer, 0, requiresAuthority: false);
		NetworkWriterPool.Return(writer);
	}

	[TargetRpc]
	private void RpcHitWithItem(NetworkConnectionToClient connection, ItemType itemType, ItemUseId itemUseId, Vector3 hitLocalPosition, Vector3 direction, Vector3 localOrigin, float distance, PlayerInventory itemUser, bool isReflected, bool isInSpecialState, bool canHitWithNoUser)
	{
		NetworkWriterPooled writer = NetworkWriterPool.Get();
		GeneratedNetworkCode._Write_ItemType(writer, itemType);
		GeneratedNetworkCode._Write_ItemUseId(writer, itemUseId);
		writer.WriteVector3(hitLocalPosition);
		writer.WriteVector3(direction);
		writer.WriteVector3(localOrigin);
		writer.WriteFloat(distance);
		writer.WriteNetworkBehaviour(itemUser);
		writer.WriteBool(isReflected);
		writer.WriteBool(isInSpecialState);
		writer.WriteBool(canHitWithNoUser);
		SendTargetRPCInternal(connection, "System.Void Hittable::RpcHitWithItem(Mirror.NetworkConnectionToClient,ItemType,ItemUseId,UnityEngine.Vector3,UnityEngine.Vector3,UnityEngine.Vector3,System.Single,PlayerInventory,System.Boolean,System.Boolean,System.Boolean)", -1798276400, writer, 0);
		NetworkWriterPool.Return(writer);
	}

	private void HitWithItemInternal(ItemType itemType, ItemUseId itemUseId, Vector3 hitLocalPosition, Vector3 direction, Vector3 localOrigin, float distance, PlayerInventory itemUser, bool isReflected, bool isInSpecialState, bool canHitWithNoUser)
	{
		if (!canHitWithNoUser && itemUser == null)
		{
			return;
		}
		if (!CanApplyPhysics())
		{
			this.WasHitByItem?.Invoke(itemUser, itemType, itemUseId, direction, distance, isReflected);
			return;
		}
		Vector3 hitWorldPosition = base.transform.TransformPoint(hitLocalPosition);
		direction = direction.normalized;
		Vector3 vector;
		switch (itemType)
		{
		case ItemType.DuelingPistol:
			vector = BMath.RemapClamped(ItemSettings.DuelingPistolMaxKnockbackDistance, ItemSettings.DuelingPistolMinKnockbackDistance, ItemSettings.DuelingPistolMaxKnockbackSpeed, ItemSettings.DuelingPistolMinKnockbackSpeed, distance) * direction;
			break;
		case ItemType.ElephantGun:
			vector = BMath.RemapClamped(ItemSettings.ElephantGunMaxKnockbackDistance, ItemSettings.ElephantGunMinKnockbackDistance, ItemSettings.ElephantGunMaxKnockbackSpeed, ItemSettings.ElephantGunMinKnockbackSpeed, distance) * direction;
			break;
		case ItemType.RocketLauncher:
		{
			float t = BMath.InverseLerpClamped(ItemSettings.RocketLauncherMaxKnockbackDistance, ItemSettings.RocketLauncherMinKnockbackDistance, distance);
			float num3 = BMath.Lerp(ItemSettings.RocketLauncherMaxKnockbackSpeed, ItemSettings.RocketLauncherMinKnockbackSpeed, t);
			float b = BMath.Lerp(ItemSettings.RocketLauncherMaxMinUpwardsKnockbackSpeed, ItemSettings.RocketLauncherMinMinUpwardsKnockbackSpeed, t);
			vector = num3 * direction;
			if (ItemSettings.RocketLauncherHasMinUpwardsKnockbackSpeed)
			{
				vector.y = BMath.Max(vector.y, b);
			}
			break;
		}
		case ItemType.Landmine:
		{
			float t = BMath.InverseLerpClamped(ItemSettings.LandmineMaxKnockbackDistance, ItemSettings.LandmineMinKnockbackDistance, distance);
			float num2 = BMath.Lerp(ItemSettings.LandmineMaxKnockbackSpeed, ItemSettings.LandmineMinKnockbackSpeed, t);
			float b = BMath.Lerp(ItemSettings.LandmineMaxMinUpwardsKnockbackSpeed, ItemSettings.LandmineMinMinUpwardsKnockbackSpeed, t);
			vector = num2 * direction;
			if (ItemSettings.LandmineHasMinUpwardsKnockbackSpeed)
			{
				vector.y = BMath.Max(vector.y, b);
			}
			break;
		}
		case ItemType.OrbitalLaser:
		{
			float t = BMath.InverseLerpClamped(ItemSettings.OrbitalLaserMaxKnockbackDistance, ItemSettings.OrbitalLaserMinKnockbackDistance, distance);
			float num = BMath.Lerp(ItemSettings.OrbitalLaserMaxKnockbackSpeed, ItemSettings.OrbitalLaserMinKnockbackSpeed, t);
			float b = BMath.Lerp(ItemSettings.OrbitalLaserMaxMinUpwardsKnockbackSpeed, ItemSettings.OrbitalLaserMinMinUpwardsKnockbackSpeed, t);
			vector = num * direction;
			if (ItemSettings.OrbitalLaserHasMinUpwardsKnockbackSpeed)
			{
				vector.y = BMath.Max(vector.y, b);
			}
			break;
		}
		default:
			vector = Vector3.zero;
			break;
		}
		this.WillApplyItemHitPhysics?.Invoke(itemUser, itemType, itemUseId, localOrigin, distance, vector, isReflected, isInSpecialState);
		AsEntity.Rigidbody.AddForceAtPosition(vector, hitWorldPosition, ForceMode.VelocityChange);
		if (AsEntity.IsGolfCart && GameManager.AllItems.TryGetItemData(itemType, out var itemData) && itemData.HitTransfersToGolfCartPassengers)
		{
			TransferHitToGolfCartPassengers();
		}
		if (base.isServer)
		{
			ServerClearHomingTarget(dueToHomingTargetHit: false);
		}
		this.WasHitByItem?.Invoke(itemUser, itemType, itemUseId, direction, distance, isReflected);
		void TransferHitToGolfCartPassengers()
		{
			for (int i = 0; i < AsEntity.AsGolfCart.passengers.Count; i++)
			{
				PlayerInfo playerInfo = AsEntity.AsGolfCart.passengers[i];
				if (!(playerInfo == null))
				{
					playerInfo.AsEntity.AsHittable.HitWithItemInternal(itemType, itemUseId, playerInfo.transform.InverseTransformPoint(hitWorldPosition), direction, localOrigin, distance, itemUser, isReflected, isInSpecialState, canHitWithNoUser);
				}
			}
		}
	}

	public void HitWithRocketLauncherBackBlast(Vector3 hitLocalPosition, Vector3 localOrigin, Vector3 direction, PlayerInventory rocketLauncherUser)
	{
		if (!IsUnhittable())
		{
			HitWithRocketLauncherBackBlastInternal(hitLocalPosition, localOrigin, direction, rocketLauncherUser);
			CmdHitWithRocketLauncherBackBlast(hitLocalPosition, localOrigin, direction, rocketLauncherUser);
		}
	}

	[Command(requiresAuthority = false)]
	private void CmdHitWithRocketLauncherBackBlast(Vector3 hitLocalPosition, Vector3 localOrigin, Vector3 direction, PlayerInventory rocketLauncherUser, NetworkConnectionToClient sender = null)
	{
		if (base.isServer && base.isClient)
		{
			UserCode_CmdHitWithRocketLauncherBackBlast__Vector3__Vector3__Vector3__PlayerInventory__NetworkConnectionToClient(hitLocalPosition, localOrigin, direction, rocketLauncherUser, sender);
			return;
		}
		NetworkWriterPooled writer = NetworkWriterPool.Get();
		writer.WriteVector3(hitLocalPosition);
		writer.WriteVector3(localOrigin);
		writer.WriteVector3(direction);
		writer.WriteNetworkBehaviour(rocketLauncherUser);
		SendCommandInternal("System.Void Hittable::CmdHitWithRocketLauncherBackBlast(UnityEngine.Vector3,UnityEngine.Vector3,UnityEngine.Vector3,PlayerInventory,Mirror.NetworkConnectionToClient)", -2146888601, writer, 0, requiresAuthority: false);
		NetworkWriterPool.Return(writer);
	}

	[TargetRpc]
	private void RpcHitWithRocketLauncherBackBlast(NetworkConnectionToClient connection, Vector3 hitLocalPosition, Vector3 localOrigin, Vector3 direction, PlayerInventory rocketLauncherUser)
	{
		NetworkWriterPooled writer = NetworkWriterPool.Get();
		writer.WriteVector3(hitLocalPosition);
		writer.WriteVector3(localOrigin);
		writer.WriteVector3(direction);
		writer.WriteNetworkBehaviour(rocketLauncherUser);
		SendTargetRPCInternal(connection, "System.Void Hittable::RpcHitWithRocketLauncherBackBlast(Mirror.NetworkConnectionToClient,UnityEngine.Vector3,UnityEngine.Vector3,UnityEngine.Vector3,PlayerInventory)", -1778325788, writer, 0);
		NetworkWriterPool.Return(writer);
	}

	private void HitWithRocketLauncherBackBlastInternal(Vector3 hitLocalPosition, Vector3 localOrigin, Vector3 direction, PlayerInventory rocketLauncherUser)
	{
		if (!(rocketLauncherUser == null))
		{
			if (!CanApplyPhysics())
			{
				this.WasHitByRocketLauncherBackBlast?.Invoke(rocketLauncherUser, direction);
				return;
			}
			direction = direction.normalized;
			Vector3 vector = ItemSettings.RocketLauncherBackBlastKnockbackSpeed * direction;
			this.WillApplyRocketLauncherBackBlastHitPhysics?.Invoke(rocketLauncherUser, hitLocalPosition, localOrigin, vector);
			AsEntity.Rigidbody.AddForceAtPosition(vector, base.transform.TransformPoint(hitLocalPosition), ForceMode.VelocityChange);
			this.WasHitByRocketLauncherBackBlast?.Invoke(rocketLauncherUser, direction);
		}
	}

	public void ServerHitWithReturnedBall()
	{
		if (IsUnhittable())
		{
			return;
		}
		HitWithReturnedBallInternal();
		foreach (NetworkConnectionToClient value in NetworkServer.connections.Values)
		{
			if (value != NetworkServer.localConnection)
			{
				RpcHitWithReturnedBall(value);
			}
		}
	}

	[TargetRpc]
	private void RpcHitWithReturnedBall(NetworkConnectionToClient connection)
	{
		NetworkWriterPooled writer = NetworkWriterPool.Get();
		SendTargetRPCInternal(connection, "System.Void Hittable::RpcHitWithReturnedBall(Mirror.NetworkConnectionToClient)", 806288765, writer, 0);
		NetworkWriterPool.Return(writer);
	}

	private void HitWithReturnedBallInternal()
	{
		PlayHitSound(base.transform.position);
		if (!CanApplyPhysics())
		{
			this.WasHitByReturnedBall?.Invoke();
			return;
		}
		Vector3 vector = -base.transform.forward.Horizontalized().normalized;
		Vector3 vector2 = vector * GameManager.GolfSettings.BallReturnToBoundsDropOnHeadKnockbackSpeed + Vector3.up * GameManager.GolfSettings.BallReturnToBoundsDropOnHeadUpwardsSpeed;
		this.WillApplyReturnedBallHitPhysics?.Invoke(vector2);
		AsEntity.Rigidbody.linearVelocity += vector2;
		AsEntity.Rigidbody.angularVelocity = GameManager.GolfSettings.BallReturnToBoundsDropOnHeadSpinSpeed * Vector3.Cross(Vector3.up, vector.normalized);
		this.WasHitByReturnedBall?.Invoke();
	}

	[Server]
	public void ServerHitWithScoreKnockback(GolfHole hole)
	{
		if (!NetworkServer.active)
		{
			Debug.LogWarning("[Server] function 'System.Void Hittable::ServerHitWithScoreKnockback(GolfHole)' called when server was not active");
		}
		else
		{
			if (hole == null || IsUnhittable())
			{
				return;
			}
			HitWithScoreKnockbackInternal(hole);
			foreach (NetworkConnectionToClient value in NetworkServer.connections.Values)
			{
				if (value != NetworkServer.localConnection)
				{
					RpcHitWithScoreKnockback(value, hole);
				}
			}
		}
	}

	[TargetRpc]
	private void RpcHitWithScoreKnockback(NetworkConnectionToClient connection, GolfHole hole)
	{
		NetworkWriterPooled writer = NetworkWriterPool.Get();
		writer.WriteNetworkBehaviour(hole);
		SendTargetRPCInternal(connection, "System.Void Hittable::RpcHitWithScoreKnockback(Mirror.NetworkConnectionToClient,GolfHole)", -1936362654, writer, 0);
		NetworkWriterPool.Return(writer);
	}

	private void HitWithScoreKnockbackInternal(GolfHole hole)
	{
		if (hole == null)
		{
			return;
		}
		if (!CanApplyPhysics())
		{
			this.WasHitByScoreKnockback?.Invoke();
			return;
		}
		this.WillApplyScoreKnockbackPhysics?.Invoke();
		Vector3 rhs = base.transform.position - hole.transform.position;
		float magnitude = rhs.magnitude;
		float t = BMath.InverseLerpClamped(hole.Settings.MaxKnockbackDistance, hole.Settings.MinKnockbackDistance, magnitude);
		float num = BMath.Lerp(settings.ScoreKnockback.MaxKnockbackSpeed, settings.ScoreKnockback.MinKnockbackSpeed, t);
		float num2 = BMath.Lerp(settings.ScoreKnockback.MaxMinUpwardsKnockback, settings.ScoreKnockback.MinMinUpwardsKnockback, t);
		float num3 = BMath.Lerp(settings.ScoreKnockback.MaxKnockbackAngularSpeed, settings.ScoreKnockback.MinKnockbackAngularSpeed, t);
		Vector3 vector = num * rhs.normalized;
		if (vector.y < num2)
		{
			vector.y = num2;
		}
		Vector3 vector2 = Vector3.Cross(Vector3.up, rhs);
		Vector3 vector3 = num3 * vector2;
		AsEntity.Rigidbody.linearVelocity += vector;
		AsEntity.Rigidbody.angularVelocity += vector3;
		this.WasHitByScoreKnockback?.Invoke();
	}

	[Server]
	private void ServerClearHomingTarget(bool dueToHomingTargetHit)
	{
		if (!NetworkServer.active)
		{
			Debug.LogWarning("[Server] function 'System.Void Hittable::ServerClearHomingTarget(System.Boolean)' called when server was not active");
		}
		else if (base.isServer)
		{
			if (NetworkhomingTargetHittable != null && !dueToHomingTargetHit && NetworkhomingTargetHittable.AsEntity.IsPlayer)
			{
				NetworkhomingTargetHittable.AsEntity.PlayerInfo.RpcInformEvadedHomingProjectile();
			}
			NetworkhomingTargetHittable = null;
		}
	}

	private void UpdateIsUpdateLoopRegistered()
	{
		bool num = isUpdateLoopRegistered;
		isUpdateLoopRegistered = ShouldBeRegistered();
		if (num != isUpdateLoopRegistered)
		{
			if (isUpdateLoopRegistered)
			{
				BUpdate.RegisterCallback(this);
			}
			else
			{
				BUpdate.DeregisterCallback(this);
			}
		}
		bool ShouldBeRegistered()
		{
			if (SwingProjectileState != SwingProjectileState.None)
			{
				return true;
			}
			if (SideSpin != 0f || homingTarget != null)
			{
				return !AsEntity.IsGolfBall;
			}
			return false;
		}
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
				homingWarningWorldspaceIcon.Initialize(WorldspaceIconManager.HomingWarningIconSettings, AsEntity.HasTargetReticlePosition ? AsEntity.TargetReticlePosition.transform : base.transform, GetWorldspaceIconDistanceReference(), WorldspaceIconManager.HomingWarningIcon);
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
			this.IsPlayingHomingWarningChanged?.Invoke();
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

	private bool CanApplyPhysics()
	{
		if (!AsEntity.HasRigidbody)
		{
			return false;
		}
		if (AsEntity.Rigidbody.isKinematic)
		{
			return false;
		}
		return AsEntity.IsSimulatingRigidbody();
	}

	private float GetSwingHitSpeed(bool isPutt, float power)
	{
		power = BMath.Max(0f, power);
		return BMath.Lerp(0f, isPutt ? SwingSettings.MaxPowerPuttHitSpeed : SwingSettings.MaxPowerSwingHitSpeed, power);
	}

	private void PlayHitSound(Vector3 worldPosition)
	{
		AudioHitObjectType audioHitObjectType = (AsEntity.IsPlayer ? AudioHitObjectType.Player : (AsEntity.IsGolfCart ? AudioHitObjectType.GolfCart : AudioHitObjectType.Default));
		EventInstance eventInstance = RuntimeManager.CreateInstance(GameManager.AudioSettings.ProjectileHitEvent);
		eventInstance.set3DAttributes(worldPosition.To3DAttributes());
		eventInstance.setParameterByID(AudioSettings.ObjectId, (float)audioHitObjectType);
		eventInstance.start();
		eventInstance.release();
	}

	[Command(requiresAuthority = false)]
	private void CmdRequestInitialState(NetworkConnectionToClient sender = null)
	{
		if (base.isServer && base.isClient)
		{
			UserCode_CmdRequestInitialState__NetworkConnectionToClient(sender);
			return;
		}
		NetworkWriterPooled writer = NetworkWriterPool.Get();
		SendCommandInternal("System.Void Hittable::CmdRequestInitialState(Mirror.NetworkConnectionToClient)", 2072440371, writer, 0, requiresAuthority: false);
		NetworkWriterPool.Return(writer);
	}

	[TargetRpc]
	private void RpcSetInitialState(NetworkConnectionToClient connection, SwingProjectileState swingProjectileState, PlayerGolfer responsibleSwingProjectilePlayer)
	{
		NetworkWriterPooled writer = NetworkWriterPool.Get();
		GeneratedNetworkCode._Write_SwingProjectileState(writer, swingProjectileState);
		writer.WriteNetworkBehaviour(responsibleSwingProjectilePlayer);
		SendTargetRPCInternal(connection, "System.Void Hittable::RpcSetInitialState(Mirror.NetworkConnectionToClient,SwingProjectileState,PlayerGolfer)", -1524593085, writer, 0);
		NetworkWriterPool.Return(writer);
	}

	private void OnHomingTargetHittableChanged(Hittable previousTarget, Hittable currentTarget)
	{
		homingTarget = ((NetworkhomingTargetHittable != null) ? NetworkhomingTargetHittable.AsEntity.AsLockOnTarget : null);
		UpdateIsUpdateLoopRegistered();
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

	private void OnSideSpinChanged(float previousSideSpin, float currentSideSpin)
	{
		UpdateIsUpdateLoopRegistered();
	}

	public Hittable()
	{
		_Mirror_SyncVarHookDelegate_homingTargetHittable = OnHomingTargetHittableChanged;
		_Mirror_SyncVarHookDelegate_sideSpin = OnSideSpinChanged;
	}

	static Hittable()
	{
		resolvedCollisionVfxHittablePairs = new HashSet<(Hittable, Hittable)>();
		resolvedCollisionVfxNonHittablePairs = new HashSet<(Hittable, Rigidbody)>();
		resolvedCollisionVfxNonRigidbodyPairs = new HashSet<(Hittable, GameObject)>();
		RemoteProcedureCalls.RegisterCommand(typeof(Hittable), "System.Void Hittable::CmdHitWithGolfSwing(UnityEngine.Vector3,UnityEngine.Vector3,UnityEngine.Vector3,System.Boolean,System.Single,System.Single,PlayerGolfer,Hittable,Mirror.NetworkConnectionToClient)", InvokeUserCode_CmdHitWithGolfSwing__Vector3__Vector3__Vector3__Boolean__Single__Single__PlayerGolfer__Hittable__NetworkConnectionToClient, requiresAuthority: false);
		RemoteProcedureCalls.RegisterCommand(typeof(Hittable), "System.Void Hittable::CmdHitWithSwingProjectile(UnityEngine.Vector3,UnityEngine.Vector3,System.Single,Hittable,System.Boolean,PlayerGolfer,Mirror.NetworkConnectionToClient)", InvokeUserCode_CmdHitWithSwingProjectile__Vector3__Vector3__Single__Hittable__Boolean__PlayerGolfer__NetworkConnectionToClient, requiresAuthority: false);
		RemoteProcedureCalls.RegisterCommand(typeof(Hittable), "System.Void Hittable::CmdHitWithDive(UnityEngine.Vector3,PlayerMovement,Mirror.NetworkConnectionToClient)", InvokeUserCode_CmdHitWithDive__Vector3__PlayerMovement__NetworkConnectionToClient, requiresAuthority: false);
		RemoteProcedureCalls.RegisterCommand(typeof(Hittable), "System.Void Hittable::CmdHitWithItem(ItemType,ItemUseId,UnityEngine.Vector3,UnityEngine.Vector3,UnityEngine.Vector3,System.Single,PlayerInventory,System.Boolean,System.Boolean,System.Boolean,Mirror.NetworkConnectionToClient)", InvokeUserCode_CmdHitWithItem__ItemType__ItemUseId__Vector3__Vector3__Vector3__Single__PlayerInventory__Boolean__Boolean__Boolean__NetworkConnectionToClient, requiresAuthority: false);
		RemoteProcedureCalls.RegisterCommand(typeof(Hittable), "System.Void Hittable::CmdHitWithRocketLauncherBackBlast(UnityEngine.Vector3,UnityEngine.Vector3,UnityEngine.Vector3,PlayerInventory,Mirror.NetworkConnectionToClient)", InvokeUserCode_CmdHitWithRocketLauncherBackBlast__Vector3__Vector3__Vector3__PlayerInventory__NetworkConnectionToClient, requiresAuthority: false);
		RemoteProcedureCalls.RegisterCommand(typeof(Hittable), "System.Void Hittable::CmdRequestInitialState(Mirror.NetworkConnectionToClient)", InvokeUserCode_CmdRequestInitialState__NetworkConnectionToClient, requiresAuthority: false);
		RemoteProcedureCalls.RegisterRpc(typeof(Hittable), "System.Void Hittable::RpcApplyPostHitBounce(Mirror.NetworkConnectionToClient,UnityEngine.Vector3)", InvokeUserCode_RpcApplyPostHitBounce__NetworkConnectionToClient__Vector3);
		RemoteProcedureCalls.RegisterRpc(typeof(Hittable), "System.Void Hittable::RpcHitWithGolfSwing(Mirror.NetworkConnectionToClient,UnityEngine.Vector3,UnityEngine.Vector3,UnityEngine.Vector3,System.Boolean,System.Single,System.Single,PlayerGolfer,Hittable)", InvokeUserCode_RpcHitWithGolfSwing__NetworkConnectionToClient__Vector3__Vector3__Vector3__Boolean__Single__Single__PlayerGolfer__Hittable);
		RemoteProcedureCalls.RegisterRpc(typeof(Hittable), "System.Void Hittable::RpcHitWithSwingProjectile(Mirror.NetworkConnectionToClient,UnityEngine.Vector3,UnityEngine.Vector3,System.Single,Hittable,System.Boolean,PlayerGolfer)", InvokeUserCode_RpcHitWithSwingProjectile__NetworkConnectionToClient__Vector3__Vector3__Single__Hittable__Boolean__PlayerGolfer);
		RemoteProcedureCalls.RegisterRpc(typeof(Hittable), "System.Void Hittable::RpcBecomeSwingProjectile(Mirror.NetworkConnectionToClient,PlayerGolfer,System.Boolean)", InvokeUserCode_RpcBecomeSwingProjectile__NetworkConnectionToClient__PlayerGolfer__Boolean);
		RemoteProcedureCalls.RegisterRpc(typeof(Hittable), "System.Void Hittable::RpcStopBeingSwingProjectile(Mirror.NetworkConnectionToClient)", InvokeUserCode_RpcStopBeingSwingProjectile__NetworkConnectionToClient);
		RemoteProcedureCalls.RegisterRpc(typeof(Hittable), "System.Void Hittable::RpcHitWithDive(Mirror.NetworkConnectionToClient,UnityEngine.Vector3,PlayerMovement)", InvokeUserCode_RpcHitWithDive__NetworkConnectionToClient__Vector3__PlayerMovement);
		RemoteProcedureCalls.RegisterRpc(typeof(Hittable), "System.Void Hittable::RpcHitWithItem(Mirror.NetworkConnectionToClient,ItemType,ItemUseId,UnityEngine.Vector3,UnityEngine.Vector3,UnityEngine.Vector3,System.Single,PlayerInventory,System.Boolean,System.Boolean,System.Boolean)", InvokeUserCode_RpcHitWithItem__NetworkConnectionToClient__ItemType__ItemUseId__Vector3__Vector3__Vector3__Single__PlayerInventory__Boolean__Boolean__Boolean);
		RemoteProcedureCalls.RegisterRpc(typeof(Hittable), "System.Void Hittable::RpcHitWithRocketLauncherBackBlast(Mirror.NetworkConnectionToClient,UnityEngine.Vector3,UnityEngine.Vector3,UnityEngine.Vector3,PlayerInventory)", InvokeUserCode_RpcHitWithRocketLauncherBackBlast__NetworkConnectionToClient__Vector3__Vector3__Vector3__PlayerInventory);
		RemoteProcedureCalls.RegisterRpc(typeof(Hittable), "System.Void Hittable::RpcHitWithReturnedBall(Mirror.NetworkConnectionToClient)", InvokeUserCode_RpcHitWithReturnedBall__NetworkConnectionToClient);
		RemoteProcedureCalls.RegisterRpc(typeof(Hittable), "System.Void Hittable::RpcHitWithScoreKnockback(Mirror.NetworkConnectionToClient,GolfHole)", InvokeUserCode_RpcHitWithScoreKnockback__NetworkConnectionToClient__GolfHole);
		RemoteProcedureCalls.RegisterRpc(typeof(Hittable), "System.Void Hittable::RpcSetInitialState(Mirror.NetworkConnectionToClient,SwingProjectileState,PlayerGolfer)", InvokeUserCode_RpcSetInitialState__NetworkConnectionToClient__SwingProjectileState__PlayerGolfer);
	}

	public override bool Weaved()
	{
		return true;
	}

	protected void UserCode_RpcApplyPostHitBounce__NetworkConnectionToClient__Vector3(NetworkConnectionToClient connection, Vector3 hitNormal)
	{
		ApplyPostHitBounceInternal(hitNormal);
	}

	protected static void InvokeUserCode_RpcApplyPostHitBounce__NetworkConnectionToClient__Vector3(NetworkBehaviour obj, NetworkReader reader, NetworkConnectionToClient senderConnection)
	{
		if (!NetworkClient.active)
		{
			Debug.LogError("TargetRPC RpcApplyPostHitBounce called on server.");
		}
		else
		{
			((Hittable)obj).UserCode_RpcApplyPostHitBounce__NetworkConnectionToClient__Vector3(null, reader.ReadVector3());
		}
	}

	protected void UserCode_CmdHitWithGolfSwing__Vector3__Vector3__Vector3__Boolean__Single__Single__PlayerGolfer__Hittable__NetworkConnectionToClient(Vector3 localHitPosition, Vector3 localOrigin, Vector3 worldDirection, bool isPutt, float power, float sideSpin, PlayerGolfer hitter, Hittable homingTargetHittable, NetworkConnectionToClient sender)
	{
		if (!serverHitWithGolfSwingCommandRateLimiter.RegisterHit(sender) || hitter == null || !IsHittableBySwing(hitter))
		{
			return;
		}
		foreach (NetworkConnectionToClient value in NetworkServer.connections.Values)
		{
			if (value != NetworkServer.localConnection && value != sender)
			{
				RpcHitWithGolfSwing(value, localHitPosition, localOrigin, worldDirection, isPutt, power, sideSpin, hitter, homingTargetHittable);
			}
		}
		if (sender != null && sender != NetworkServer.localConnection)
		{
			HitWithGolfSwingInternal(localHitPosition, localOrigin, worldDirection, isPutt, power, sideSpin, hitter, homingTargetHittable);
		}
	}

	protected static void InvokeUserCode_CmdHitWithGolfSwing__Vector3__Vector3__Vector3__Boolean__Single__Single__PlayerGolfer__Hittable__NetworkConnectionToClient(NetworkBehaviour obj, NetworkReader reader, NetworkConnectionToClient senderConnection)
	{
		if (!NetworkServer.active)
		{
			Debug.LogError("Command CmdHitWithGolfSwing called on client.");
		}
		else
		{
			((Hittable)obj).UserCode_CmdHitWithGolfSwing__Vector3__Vector3__Vector3__Boolean__Single__Single__PlayerGolfer__Hittable__NetworkConnectionToClient(reader.ReadVector3(), reader.ReadVector3(), reader.ReadVector3(), reader.ReadBool(), reader.ReadFloat(), reader.ReadFloat(), reader.ReadNetworkBehaviour<PlayerGolfer>(), reader.ReadNetworkBehaviour<Hittable>(), senderConnection);
		}
	}

	protected void UserCode_RpcHitWithGolfSwing__NetworkConnectionToClient__Vector3__Vector3__Vector3__Boolean__Single__Single__PlayerGolfer__Hittable(NetworkConnectionToClient connection, Vector3 localHitPosition, Vector3 localOrigin, Vector3 worldDirection, bool isPutt, float power, float sideSpin, PlayerGolfer hitter, Hittable homingTargetHittable)
	{
		HitWithGolfSwingInternal(localHitPosition, localOrigin, worldDirection, isPutt, power, sideSpin, hitter, homingTargetHittable);
	}

	protected static void InvokeUserCode_RpcHitWithGolfSwing__NetworkConnectionToClient__Vector3__Vector3__Vector3__Boolean__Single__Single__PlayerGolfer__Hittable(NetworkBehaviour obj, NetworkReader reader, NetworkConnectionToClient senderConnection)
	{
		if (!NetworkClient.active)
		{
			Debug.LogError("TargetRPC RpcHitWithGolfSwing called on server.");
		}
		else
		{
			((Hittable)obj).UserCode_RpcHitWithGolfSwing__NetworkConnectionToClient__Vector3__Vector3__Vector3__Boolean__Single__Single__PlayerGolfer__Hittable(null, reader.ReadVector3(), reader.ReadVector3(), reader.ReadVector3(), reader.ReadBool(), reader.ReadFloat(), reader.ReadFloat(), reader.ReadNetworkBehaviour<PlayerGolfer>(), reader.ReadNetworkBehaviour<Hittable>());
		}
	}

	protected void UserCode_CmdHitWithSwingProjectile__Vector3__Vector3__Single__Hittable__Boolean__PlayerGolfer__NetworkConnectionToClient(Vector3 localHitPosition, Vector3 worldHitDirection, float normalizedHitSpeed, Hittable hitter, bool wasHoming, PlayerGolfer responsiblePlayer, NetworkConnectionToClient sender)
	{
		if (!serverHitWithSwingProjectileCommandRateLimiter.RegisterHit(sender) || hitter == null || IsUnhittable())
		{
			return;
		}
		foreach (NetworkConnectionToClient value in NetworkServer.connections.Values)
		{
			if (value != NetworkServer.localConnection && value != sender)
			{
				RpcHitWithSwingProjectile(value, localHitPosition, worldHitDirection, normalizedHitSpeed, hitter, wasHoming, responsiblePlayer);
			}
		}
		if (sender != null && sender != NetworkServer.localConnection)
		{
			HitWithSwingProjectileInternal(localHitPosition, worldHitDirection, normalizedHitSpeed, hitter, wasHoming, responsiblePlayer);
		}
	}

	protected static void InvokeUserCode_CmdHitWithSwingProjectile__Vector3__Vector3__Single__Hittable__Boolean__PlayerGolfer__NetworkConnectionToClient(NetworkBehaviour obj, NetworkReader reader, NetworkConnectionToClient senderConnection)
	{
		if (!NetworkServer.active)
		{
			Debug.LogError("Command CmdHitWithSwingProjectile called on client.");
		}
		else
		{
			((Hittable)obj).UserCode_CmdHitWithSwingProjectile__Vector3__Vector3__Single__Hittable__Boolean__PlayerGolfer__NetworkConnectionToClient(reader.ReadVector3(), reader.ReadVector3(), reader.ReadFloat(), reader.ReadNetworkBehaviour<Hittable>(), reader.ReadBool(), reader.ReadNetworkBehaviour<PlayerGolfer>(), senderConnection);
		}
	}

	protected void UserCode_RpcHitWithSwingProjectile__NetworkConnectionToClient__Vector3__Vector3__Single__Hittable__Boolean__PlayerGolfer(NetworkConnectionToClient connection, Vector3 localHitPosition, Vector3 worldHitDirection, float normalizedHitSpeed, Hittable hitter, bool wasHoming, PlayerGolfer responsiblePlayer)
	{
		HitWithSwingProjectileInternal(localHitPosition, worldHitDirection, normalizedHitSpeed, hitter, wasHoming, responsiblePlayer);
	}

	protected static void InvokeUserCode_RpcHitWithSwingProjectile__NetworkConnectionToClient__Vector3__Vector3__Single__Hittable__Boolean__PlayerGolfer(NetworkBehaviour obj, NetworkReader reader, NetworkConnectionToClient senderConnection)
	{
		if (!NetworkClient.active)
		{
			Debug.LogError("TargetRPC RpcHitWithSwingProjectile called on server.");
		}
		else
		{
			((Hittable)obj).UserCode_RpcHitWithSwingProjectile__NetworkConnectionToClient__Vector3__Vector3__Single__Hittable__Boolean__PlayerGolfer(null, reader.ReadVector3(), reader.ReadVector3(), reader.ReadFloat(), reader.ReadNetworkBehaviour<Hittable>(), reader.ReadBool(), reader.ReadNetworkBehaviour<PlayerGolfer>());
		}
	}

	protected void UserCode_RpcBecomeSwingProjectile__NetworkConnectionToClient__PlayerGolfer__Boolean(NetworkConnectionToClient connection, PlayerGolfer responsiblePlayer, bool isReflected)
	{
		OnBecameSwingProjectile(responsiblePlayer, isReflected);
	}

	protected static void InvokeUserCode_RpcBecomeSwingProjectile__NetworkConnectionToClient__PlayerGolfer__Boolean(NetworkBehaviour obj, NetworkReader reader, NetworkConnectionToClient senderConnection)
	{
		if (!NetworkClient.active)
		{
			Debug.LogError("TargetRPC RpcBecomeSwingProjectile called on server.");
		}
		else
		{
			((Hittable)obj).UserCode_RpcBecomeSwingProjectile__NetworkConnectionToClient__PlayerGolfer__Boolean(null, reader.ReadNetworkBehaviour<PlayerGolfer>(), reader.ReadBool());
		}
	}

	protected void UserCode_RpcStopBeingSwingProjectile__NetworkConnectionToClient(NetworkConnectionToClient connection)
	{
		OnStoppedBeingSwingProjectile();
	}

	protected static void InvokeUserCode_RpcStopBeingSwingProjectile__NetworkConnectionToClient(NetworkBehaviour obj, NetworkReader reader, NetworkConnectionToClient senderConnection)
	{
		if (!NetworkClient.active)
		{
			Debug.LogError("TargetRPC RpcStopBeingSwingProjectile called on server.");
		}
		else
		{
			((Hittable)obj).UserCode_RpcStopBeingSwingProjectile__NetworkConnectionToClient(null);
		}
	}

	protected void UserCode_CmdHitWithDive__Vector3__PlayerMovement__NetworkConnectionToClient(Vector3 relativeHitVelocity, PlayerMovement hitter, NetworkConnectionToClient sender)
	{
		if (!serverHitWithDiveCommandRateLimiter.RegisterHit(sender) || hitter == null || !DiveSettings.CanBeHit || IsUnhittable())
		{
			return;
		}
		foreach (NetworkConnectionToClient value in NetworkServer.connections.Values)
		{
			if (value != NetworkServer.localConnection && value != sender)
			{
				RpcHitWithDive(value, relativeHitVelocity, hitter);
			}
		}
		if (sender != null && sender != NetworkServer.localConnection)
		{
			HitWithDiveInternal(relativeHitVelocity, hitter);
		}
	}

	protected static void InvokeUserCode_CmdHitWithDive__Vector3__PlayerMovement__NetworkConnectionToClient(NetworkBehaviour obj, NetworkReader reader, NetworkConnectionToClient senderConnection)
	{
		if (!NetworkServer.active)
		{
			Debug.LogError("Command CmdHitWithDive called on client.");
		}
		else
		{
			((Hittable)obj).UserCode_CmdHitWithDive__Vector3__PlayerMovement__NetworkConnectionToClient(reader.ReadVector3(), reader.ReadNetworkBehaviour<PlayerMovement>(), senderConnection);
		}
	}

	protected void UserCode_RpcHitWithDive__NetworkConnectionToClient__Vector3__PlayerMovement(NetworkConnectionToClient connection, Vector3 relativeHitVelocity, PlayerMovement hitter)
	{
		HitWithDiveInternal(relativeHitVelocity, hitter);
	}

	protected static void InvokeUserCode_RpcHitWithDive__NetworkConnectionToClient__Vector3__PlayerMovement(NetworkBehaviour obj, NetworkReader reader, NetworkConnectionToClient senderConnection)
	{
		if (!NetworkClient.active)
		{
			Debug.LogError("TargetRPC RpcHitWithDive called on server.");
		}
		else
		{
			((Hittable)obj).UserCode_RpcHitWithDive__NetworkConnectionToClient__Vector3__PlayerMovement(null, reader.ReadVector3(), reader.ReadNetworkBehaviour<PlayerMovement>());
		}
	}

	protected void UserCode_CmdHitWithItem__ItemType__ItemUseId__Vector3__Vector3__Vector3__Single__PlayerInventory__Boolean__Boolean__Boolean__NetworkConnectionToClient(ItemType itemType, ItemUseId itemUseId, Vector3 hitLocalPosition, Vector3 direction, Vector3 localOrigin, float distance, PlayerInventory itemUser, bool isReflected, bool isInSpecialState, bool canHitWithNoUser, NetworkConnectionToClient sender)
	{
		if (!serverHitWithItemCommandRateLimiter.RegisterHit(sender) || (!canHitWithNoUser && itemUser == null) || IsUnhittable())
		{
			return;
		}
		foreach (NetworkConnectionToClient value in NetworkServer.connections.Values)
		{
			if (value != NetworkServer.localConnection && value != sender)
			{
				RpcHitWithItem(value, itemType, itemUseId, hitLocalPosition, direction, localOrigin, distance, itemUser, isReflected, isInSpecialState, canHitWithNoUser);
			}
		}
		if (sender != null && sender != NetworkServer.localConnection)
		{
			HitWithItemInternal(itemType, itemUseId, hitLocalPosition, direction, localOrigin, distance, itemUser, isReflected, isInSpecialState, canHitWithNoUser);
		}
	}

	protected static void InvokeUserCode_CmdHitWithItem__ItemType__ItemUseId__Vector3__Vector3__Vector3__Single__PlayerInventory__Boolean__Boolean__Boolean__NetworkConnectionToClient(NetworkBehaviour obj, NetworkReader reader, NetworkConnectionToClient senderConnection)
	{
		if (!NetworkServer.active)
		{
			Debug.LogError("Command CmdHitWithItem called on client.");
		}
		else
		{
			((Hittable)obj).UserCode_CmdHitWithItem__ItemType__ItemUseId__Vector3__Vector3__Vector3__Single__PlayerInventory__Boolean__Boolean__Boolean__NetworkConnectionToClient(GeneratedNetworkCode._Read_ItemType(reader), GeneratedNetworkCode._Read_ItemUseId(reader), reader.ReadVector3(), reader.ReadVector3(), reader.ReadVector3(), reader.ReadFloat(), reader.ReadNetworkBehaviour<PlayerInventory>(), reader.ReadBool(), reader.ReadBool(), reader.ReadBool(), senderConnection);
		}
	}

	protected void UserCode_RpcHitWithItem__NetworkConnectionToClient__ItemType__ItemUseId__Vector3__Vector3__Vector3__Single__PlayerInventory__Boolean__Boolean__Boolean(NetworkConnectionToClient connection, ItemType itemType, ItemUseId itemUseId, Vector3 hitLocalPosition, Vector3 direction, Vector3 localOrigin, float distance, PlayerInventory itemUser, bool isReflected, bool isInSpecialState, bool canHitWithNoUser)
	{
		HitWithItemInternal(itemType, itemUseId, hitLocalPosition, direction, localOrigin, distance, itemUser, isReflected, isInSpecialState, canHitWithNoUser);
	}

	protected static void InvokeUserCode_RpcHitWithItem__NetworkConnectionToClient__ItemType__ItemUseId__Vector3__Vector3__Vector3__Single__PlayerInventory__Boolean__Boolean__Boolean(NetworkBehaviour obj, NetworkReader reader, NetworkConnectionToClient senderConnection)
	{
		if (!NetworkClient.active)
		{
			Debug.LogError("TargetRPC RpcHitWithItem called on server.");
		}
		else
		{
			((Hittable)obj).UserCode_RpcHitWithItem__NetworkConnectionToClient__ItemType__ItemUseId__Vector3__Vector3__Vector3__Single__PlayerInventory__Boolean__Boolean__Boolean(null, GeneratedNetworkCode._Read_ItemType(reader), GeneratedNetworkCode._Read_ItemUseId(reader), reader.ReadVector3(), reader.ReadVector3(), reader.ReadVector3(), reader.ReadFloat(), reader.ReadNetworkBehaviour<PlayerInventory>(), reader.ReadBool(), reader.ReadBool(), reader.ReadBool());
		}
	}

	protected void UserCode_CmdHitWithRocketLauncherBackBlast__Vector3__Vector3__Vector3__PlayerInventory__NetworkConnectionToClient(Vector3 hitLocalPosition, Vector3 localOrigin, Vector3 direction, PlayerInventory rocketLauncherUser, NetworkConnectionToClient sender)
	{
		if (!serverHitWithRocketBackBlastCommandRateLimiter.RegisterHit(sender) || rocketLauncherUser == null || IsUnhittable())
		{
			return;
		}
		foreach (NetworkConnectionToClient value in NetworkServer.connections.Values)
		{
			if (value != NetworkServer.localConnection && value != sender)
			{
				RpcHitWithRocketLauncherBackBlast(value, hitLocalPosition, localOrigin, direction, rocketLauncherUser);
			}
		}
		if (sender != null && sender != NetworkServer.localConnection)
		{
			HitWithRocketLauncherBackBlastInternal(hitLocalPosition, localOrigin, direction, rocketLauncherUser);
		}
	}

	protected static void InvokeUserCode_CmdHitWithRocketLauncherBackBlast__Vector3__Vector3__Vector3__PlayerInventory__NetworkConnectionToClient(NetworkBehaviour obj, NetworkReader reader, NetworkConnectionToClient senderConnection)
	{
		if (!NetworkServer.active)
		{
			Debug.LogError("Command CmdHitWithRocketLauncherBackBlast called on client.");
		}
		else
		{
			((Hittable)obj).UserCode_CmdHitWithRocketLauncherBackBlast__Vector3__Vector3__Vector3__PlayerInventory__NetworkConnectionToClient(reader.ReadVector3(), reader.ReadVector3(), reader.ReadVector3(), reader.ReadNetworkBehaviour<PlayerInventory>(), senderConnection);
		}
	}

	protected void UserCode_RpcHitWithRocketLauncherBackBlast__NetworkConnectionToClient__Vector3__Vector3__Vector3__PlayerInventory(NetworkConnectionToClient connection, Vector3 hitLocalPosition, Vector3 localOrigin, Vector3 direction, PlayerInventory rocketLauncherUser)
	{
		HitWithRocketLauncherBackBlastInternal(hitLocalPosition, direction, localOrigin, rocketLauncherUser);
	}

	protected static void InvokeUserCode_RpcHitWithRocketLauncherBackBlast__NetworkConnectionToClient__Vector3__Vector3__Vector3__PlayerInventory(NetworkBehaviour obj, NetworkReader reader, NetworkConnectionToClient senderConnection)
	{
		if (!NetworkClient.active)
		{
			Debug.LogError("TargetRPC RpcHitWithRocketLauncherBackBlast called on server.");
		}
		else
		{
			((Hittable)obj).UserCode_RpcHitWithRocketLauncherBackBlast__NetworkConnectionToClient__Vector3__Vector3__Vector3__PlayerInventory(null, reader.ReadVector3(), reader.ReadVector3(), reader.ReadVector3(), reader.ReadNetworkBehaviour<PlayerInventory>());
		}
	}

	protected void UserCode_RpcHitWithReturnedBall__NetworkConnectionToClient(NetworkConnectionToClient connection)
	{
		HitWithReturnedBallInternal();
	}

	protected static void InvokeUserCode_RpcHitWithReturnedBall__NetworkConnectionToClient(NetworkBehaviour obj, NetworkReader reader, NetworkConnectionToClient senderConnection)
	{
		if (!NetworkClient.active)
		{
			Debug.LogError("TargetRPC RpcHitWithReturnedBall called on server.");
		}
		else
		{
			((Hittable)obj).UserCode_RpcHitWithReturnedBall__NetworkConnectionToClient(null);
		}
	}

	protected void UserCode_RpcHitWithScoreKnockback__NetworkConnectionToClient__GolfHole(NetworkConnectionToClient connection, GolfHole hole)
	{
		HitWithScoreKnockbackInternal(hole);
	}

	protected static void InvokeUserCode_RpcHitWithScoreKnockback__NetworkConnectionToClient__GolfHole(NetworkBehaviour obj, NetworkReader reader, NetworkConnectionToClient senderConnection)
	{
		if (!NetworkClient.active)
		{
			Debug.LogError("TargetRPC RpcHitWithScoreKnockback called on server.");
		}
		else
		{
			((Hittable)obj).UserCode_RpcHitWithScoreKnockback__NetworkConnectionToClient__GolfHole(null, reader.ReadNetworkBehaviour<GolfHole>());
		}
	}

	protected void UserCode_CmdRequestInitialState__NetworkConnectionToClient(NetworkConnectionToClient sender)
	{
		if (serverRequestInitialStateCommandRateLimiter.RegisterHit(sender))
		{
			RpcSetInitialState(sender, SwingProjectileState, responsibleSwingProjectilePlayer);
		}
	}

	protected static void InvokeUserCode_CmdRequestInitialState__NetworkConnectionToClient(NetworkBehaviour obj, NetworkReader reader, NetworkConnectionToClient senderConnection)
	{
		if (!NetworkServer.active)
		{
			Debug.LogError("Command CmdRequestInitialState called on client.");
		}
		else
		{
			((Hittable)obj).UserCode_CmdRequestInitialState__NetworkConnectionToClient(senderConnection);
		}
	}

	protected void UserCode_RpcSetInitialState__NetworkConnectionToClient__SwingProjectileState__PlayerGolfer(NetworkConnectionToClient connection, SwingProjectileState swingProjectileState, PlayerGolfer responsibleSwingProjectilePlayer)
	{
		SwingProjectileState = swingProjectileState;
		this.responsibleSwingProjectilePlayer = responsibleSwingProjectilePlayer;
		UpdateIsUpdateLoopRegistered();
	}

	protected static void InvokeUserCode_RpcSetInitialState__NetworkConnectionToClient__SwingProjectileState__PlayerGolfer(NetworkBehaviour obj, NetworkReader reader, NetworkConnectionToClient senderConnection)
	{
		if (!NetworkClient.active)
		{
			Debug.LogError("TargetRPC RpcSetInitialState called on server.");
		}
		else
		{
			((Hittable)obj).UserCode_RpcSetInitialState__NetworkConnectionToClient__SwingProjectileState__PlayerGolfer(null, GeneratedNetworkCode._Read_SwingProjectileState(reader), reader.ReadNetworkBehaviour<PlayerGolfer>());
		}
	}

	public override void SerializeSyncVars(NetworkWriter writer, bool forceAll)
	{
		base.SerializeSyncVars(writer, forceAll);
		if (forceAll)
		{
			writer.WriteVector3(swingHitPosition);
			writer.WriteNetworkBehaviour(NetworkhomingTargetHittable);
			writer.WriteFloat(homingInitialHorizontalDistance);
			writer.WriteFloat(sideSpin);
			return;
		}
		writer.WriteVarULong(syncVarDirtyBits);
		if ((syncVarDirtyBits & 1L) != 0L)
		{
			writer.WriteVector3(swingHitPosition);
		}
		if ((syncVarDirtyBits & 2L) != 0L)
		{
			writer.WriteNetworkBehaviour(NetworkhomingTargetHittable);
		}
		if ((syncVarDirtyBits & 4L) != 0L)
		{
			writer.WriteFloat(homingInitialHorizontalDistance);
		}
		if ((syncVarDirtyBits & 8L) != 0L)
		{
			writer.WriteFloat(sideSpin);
		}
	}

	public override void DeserializeSyncVars(NetworkReader reader, bool initialState)
	{
		base.DeserializeSyncVars(reader, initialState);
		if (initialState)
		{
			GeneratedSyncVarDeserialize(ref swingHitPosition, null, reader.ReadVector3());
			GeneratedSyncVarDeserialize_NetworkBehaviour(ref homingTargetHittable, _Mirror_SyncVarHookDelegate_homingTargetHittable, reader, ref ___homingTargetHittableNetId);
			GeneratedSyncVarDeserialize(ref homingInitialHorizontalDistance, null, reader.ReadFloat());
			GeneratedSyncVarDeserialize(ref sideSpin, _Mirror_SyncVarHookDelegate_sideSpin, reader.ReadFloat());
			return;
		}
		long num = (long)reader.ReadVarULong();
		if ((num & 1L) != 0L)
		{
			GeneratedSyncVarDeserialize(ref swingHitPosition, null, reader.ReadVector3());
		}
		if ((num & 2L) != 0L)
		{
			GeneratedSyncVarDeserialize_NetworkBehaviour(ref homingTargetHittable, _Mirror_SyncVarHookDelegate_homingTargetHittable, reader, ref ___homingTargetHittableNetId);
		}
		if ((num & 4L) != 0L)
		{
			GeneratedSyncVarDeserialize(ref homingInitialHorizontalDistance, null, reader.ReadFloat());
		}
		if ((num & 8L) != 0L)
		{
			GeneratedSyncVarDeserialize(ref sideSpin, _Mirror_SyncVarHookDelegate_sideSpin, reader.ReadFloat());
		}
	}
}
