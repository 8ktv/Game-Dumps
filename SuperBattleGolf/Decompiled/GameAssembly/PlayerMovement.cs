#define DEBUG_DRAW
using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using Brimstone.Geometry;
using Cysharp.Threading.Tasks;
using FMOD.Studio;
using FMODUnity;
using Mirror;
using Mirror.RemoteCalls;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerMovement : NetworkBehaviour
{
	private struct KnockedOutVfxData
	{
		public int totalStarCount;

		public int coloredStarCount;

		public static KnockedOutVfxData None => default(KnockedOutVfxData);

		public KnockedOutVfxData(int totalStarCount, int coloredStarCount)
		{
			this.totalStarCount = totalStarCount;
			this.coloredStarCount = coloredStarCount;
		}
	}

	public struct KnockOutImmunity
	{
		public bool hasImmunity;

		public KnockOutVfxColor color;

		public static KnockOutImmunity Get(KnockOutVfxColor color)
		{
			return new KnockOutImmunity
			{
				hasImmunity = true,
				color = color
			};
		}

		public static KnockOutImmunity Reset(KnockOutImmunity prev)
		{
			prev.hasImmunity = false;
			return prev;
		}
	}

	private const float speedBoostEffectCooldown = 0.5f;

	[SerializeField]
	private CapsuleCollider uprightCollider;

	[SerializeField]
	private CapsuleCollider divingCollider;

	[SerializeField]
	private CapsuleCollider hittableCollider;

	private Rigidbody rigidbody;

	private readonly List<Renderer> renderers = new List<Renderer>();

	private Vector3 cameraCorrectedWorldForward;

	private Vector3 cameraCorrectedWorldMoveDirection;

	private Vector3 worldMoveVector3d;

	private Vector3 rawWorldMoveVector3d;

	private float worldMoveVectorYaw;

	private float rawWorldMoveVectorYaw;

	private Vector3 localSmoothedMoveVector3d;

	private Vector2 persistentWorldMoveDirection2dRaw;

	private float previousPersistentWorldMoveDirectionYaw;

	private float movementSensitivityFactor = 1f;

	private bool movementSuppressedUntilInputReleased;

	private readonly RaycastHit[] raycastHitCache = new RaycastHit[10];

	private Vector3 initialPosition;

	private Quaternion initialRotation;

	[HideInInspector]
	public Vector2 moveVector2d;

	[HideInInspector]
	public Vector2 rawMoveVector2d;

	private float gravityFactor;

	private float horizontalDrag;

	private float rotationDragFactor;

	private float speed;

	private float rotationSpeedFactor = 1f;

	private float movementAccelerationChangeSpeed = 6f;

	private float targetYaw;

	private bool turningOnADime;

	[SyncVar]
	private Vector3 syncedVelocity;

	private bool wasGrounded;

	[SyncVar(hook = "OnIsGroundedChanged")]
	private bool isGrounded;

	[SyncVar]
	private GroundTerrainType groundTerrainType;

	[SyncVar]
	private TerrainLayer groundTerrainDominantGlobalLayer;

	private bool isGroundedAtAll;

	private bool isGroundedAtAllInitialized;

	private Vector3 ungroundedLastGroundedAtAllPosition;

	private bool lastUngroundedDueToJumpPad;

	private Vector3 anchorVelocity;

	[SyncVar(hook = "OnIsWadingInWaterChanged")]
	private bool isWadingInWater;

	private bool wasWadingInWaterWhenLastGrounded;

	private float groundTime;

	private float verticalTerminalVelocity;

	[SyncVar(hook = "OnIsVisibleChanged")]
	private bool isVisible;

	private bool isForcedHidden;

	[SyncVar]
	private KnockoutState knockoutState;

	private KnockoutType knockoutType;

	private double knockoutTimestamp;

	private RaycastHit knockoutGroundRaycastHit;

	private bool wasLastKnockoutLegSweep;

	[SyncVar(hook = "OnKnockedOutVfxDataChanged")]
	private KnockedOutVfxData knockedOutVfxData;

	private Coroutine knockoutRecoveryRoutine;

	[SyncVar(hook = "OnKnockoutImmunityStatusChanged")]
	private KnockOutImmunity knockoutImmunityStatus;

	private Coroutine knockoutImmunityRoutine;

	private KnockedOutVfx knockedOutVfx;

	private PoolableParticleSystem knockoutImmunityVfx;

	private double longKnockoutImmunityIncrementTimestamp;

	private int recentKnockoutImmunityCount;

	private double lastHitByDaveTimestamp = double.MinValue;

	private int recentHitsByDiveCount;

	[SyncVar(hook = "OnStatusEffectsChanged")]
	private StatusEffect statusEffects;

	private float continuousSpeedBoostTime;

	private double speedBoostAdditionTimestamp;

	[SyncVar(hook = "OnDivingStateChanged")]
	private DivingState divingState;

	[SyncVar(hook = "OnDiveTypeChanged")]
	private DiveType diveType;

	private double diveOnGroundTimestampLocal;

	private double diveStartTimestamp;

	private readonly HashSet<Hittable> diveHitHittables = new HashSet<Hittable>();

	private PoolableParticleSystem rocketDriverSwingMissTrailVfx;

	private EventInstance rocketDriverSwingMissTrailSound;

	private bool isDrowning;

	[SyncVar(hook = "OnRespawnStateChanged")]
	private RespawnState respawnState;

	private Coroutine respawnRoutine;

	private float localPlayerOutOfBoundsRemainingTime;

	private double localPlayerExplorerAchievementLastOutOfBoundsTimestamp;

	private Quaternion straightenedSpine1Rotation;

	private double airhornHeadShakeStartTimestamp = double.MinValue;

	private bool isStrafing;

	private float strafeStrength;

	private Coroutine strafeStrengthBlendRoutine;

	private Vector3 previousWorldCenterOfMass;

	private Vector3 previousVelocity;

	private Vector3 previousAngularVelocity;

	private readonly HashSet<GolfCartInfo> golfCartsUnableToKnockOut = new HashSet<GolfCartInfo>();

	private GolfCartInfo golfCartBeingEntered;

	private Coroutine timeOutGolfCartBeingEnteredRoutine;

	private AntiCheatRateChecker serverKnockoutBlockedCommandRateLimiter;

	private AntiCheatRateChecker serverInformKnockedOutCommandRateLimiter;

	private AntiCheatRateChecker serverRespawnEffectCommandRateLimiter;

	private AntiCheatRateChecker serverInformGroundedCommandRateLimiter;

	private AntiCheatRateChecker serverSpringBootsLandingCommandRateLimiter;

	private AntiCheatRateChecker serverInformTeleportedCommandRateLimiter;

	private AntiCheatRateChecker serverSpeedBoostEffectCommandRateLimiter;

	private AntiCheatRateChecker serverOutOfBoundsEliminationExplosionCommandRateLimiter;

	private AntiCheatRateChecker serverGolfCartKnockoutEffectsCommandRateLimiter;

	private AntiCheatRateChecker serverRestartInformCommandRateLimiter;

	private bool isKnockoutProtectedFromLocalPlayer;

	private Dictionary<ulong, double> knockoutTimePerPlayerGuid = new Dictionary<ulong, double>();

	[CVar("drawPlayerGroundingDebug", "", "", false, true)]
	private static bool drawPlayerGroundingDebug;

	[CVar("drawGolfCartExitDebug", "", "", false, true)]
	private static bool drawGolfCartExitDebug;

	[CVar("moveSpeedFactor", "", "", false, true)]
	private static float consoleSpeedFactor;

	[CVar("noclip", "Zip zap zoop around the level", "", false, true, callback = "NoClipChanged")]
	private static bool noClipEnabled;

	public Action<bool, bool> _Mirror_SyncVarHookDelegate_isGrounded;

	public Action<bool, bool> _Mirror_SyncVarHookDelegate_isWadingInWater;

	public Action<bool, bool> _Mirror_SyncVarHookDelegate_isVisible;

	public Action<KnockedOutVfxData, KnockedOutVfxData> _Mirror_SyncVarHookDelegate_knockedOutVfxData;

	public Action<KnockOutImmunity, KnockOutImmunity> _Mirror_SyncVarHookDelegate_knockoutImmunityStatus;

	public Action<StatusEffect, StatusEffect> _Mirror_SyncVarHookDelegate_statusEffects;

	public Action<DivingState, DivingState> _Mirror_SyncVarHookDelegate_divingState;

	public Action<DiveType, DiveType> _Mirror_SyncVarHookDelegate_diveType;

	public Action<RespawnState, RespawnState> _Mirror_SyncVarHookDelegate_respawnState;

	public PlayerInfo PlayerInfo { get; private set; }

	public float RawMoveVectorMagnitude { get; private set; }

	public float MoveVectorMagnitude { get; private set; }

	public Vector3 SyncedVelocity => syncedVelocity;

	public PlayerGroundData GroundData { get; private set; }

	public bool IsWadingInWater => isWadingInWater;

	public bool IsInSpringBootsJump { get; private set; }

	public double IsKnockedOutTimestamp { get; private set; } = double.MinValue;

	public KnockOutImmunity KnockoutImmunityStatus => knockoutImmunityStatus;

	public float SpeedBoostRemainingTime { get; private set; }

	public DivingState DivingState => divingState;

	public DiveType DiveType => diveType;

	public double TimeSinceDiveGrounded
	{
		get
		{
			if (divingState != DivingState.OnGround)
			{
				return 0.0;
			}
			return Time.timeAsDouble - diveOnGroundTimestampLocal;
		}
	}

	public double DivingStateTimestamp { get; private set; }

	public bool IsRespawning => respawnState != RespawnState.None;

	public CapsuleCollider UprightCollider => uprightCollider;

	public CapsuleCollider DivingCollider => divingCollider;

	public CapsuleCollider HittableCollider => hittableCollider;

	public bool IsGrounded => isGrounded;

	public GroundTerrainType GroundTerrainType => groundTerrainType;

	public TerrainLayer GroundTerrainDominantGlobalLayer => groundTerrainDominantGlobalLayer;

	public bool IsKnockedOutOrRecovering => knockoutState != KnockoutState.None;

	public bool IsKnockedOut
	{
		get
		{
			if (knockoutState != KnockoutState.None)
			{
				return knockoutState != KnockoutState.Recovering;
			}
			return false;
		}
	}

	public bool IsVisible => isVisible;

	public StatusEffect StatusEffects => statusEffects;

	public Vector3 Position
	{
		get
		{
			return rigidbody.position;
		}
		private set
		{
			rigidbody.MovePosition(value);
		}
	}

	public Quaternion Rotation
	{
		get
		{
			return rigidbody.rotation;
		}
		private set
		{
			rigidbody.MoveRotation(value);
		}
	}

	public Vector3 Velocity
	{
		get
		{
			return rigidbody.linearVelocity - anchorVelocity;
		}
		private set
		{
			if (!rigidbody.isKinematic)
			{
				rigidbody.linearVelocity = value + anchorVelocity;
			}
		}
	}

	public float Yaw
	{
		get
		{
			return rigidbody.rotation.eulerAngles.y;
		}
		private set
		{
			rigidbody.MoveRotation(Quaternion.Euler(0f, value, 0f));
		}
	}

	public float YawSpeedDeg
	{
		get
		{
			return rigidbody.angularVelocity.y * (180f / MathF.PI);
		}
		private set
		{
			if (!rigidbody.isKinematic)
			{
				rigidbody.angularVelocity = Vector3.up * value * (MathF.PI / 180f);
			}
		}
	}

	public float YawSpeedRad
	{
		get
		{
			return rigidbody.angularVelocity.y;
		}
		private set
		{
			if (!rigidbody.isKinematic)
			{
				rigidbody.angularVelocity = Vector3.up * value;
			}
		}
	}

	public static bool NoClipEnabled => noClipEnabled;

	public Vector3 NetworksyncedVelocity
	{
		get
		{
			return syncedVelocity;
		}
		[param: In]
		set
		{
			GeneratedSyncVarSetter(value, ref syncedVelocity, 1uL, null);
		}
	}

	public bool NetworkisGrounded
	{
		get
		{
			return isGrounded;
		}
		[param: In]
		set
		{
			GeneratedSyncVarSetter(value, ref isGrounded, 2uL, _Mirror_SyncVarHookDelegate_isGrounded);
		}
	}

	public GroundTerrainType NetworkgroundTerrainType
	{
		get
		{
			return groundTerrainType;
		}
		[param: In]
		set
		{
			GeneratedSyncVarSetter(value, ref groundTerrainType, 4uL, null);
		}
	}

	public TerrainLayer NetworkgroundTerrainDominantGlobalLayer
	{
		get
		{
			return groundTerrainDominantGlobalLayer;
		}
		[param: In]
		set
		{
			GeneratedSyncVarSetter(value, ref groundTerrainDominantGlobalLayer, 8uL, null);
		}
	}

	public bool NetworkisWadingInWater
	{
		get
		{
			return isWadingInWater;
		}
		[param: In]
		set
		{
			GeneratedSyncVarSetter(value, ref isWadingInWater, 16uL, _Mirror_SyncVarHookDelegate_isWadingInWater);
		}
	}

	public bool NetworkisVisible
	{
		get
		{
			return isVisible;
		}
		[param: In]
		set
		{
			GeneratedSyncVarSetter(value, ref isVisible, 32uL, _Mirror_SyncVarHookDelegate_isVisible);
		}
	}

	public KnockoutState NetworkknockoutState
	{
		get
		{
			return knockoutState;
		}
		[param: In]
		set
		{
			GeneratedSyncVarSetter(value, ref knockoutState, 64uL, null);
		}
	}

	public KnockedOutVfxData NetworkknockedOutVfxData
	{
		get
		{
			return knockedOutVfxData;
		}
		[param: In]
		set
		{
			GeneratedSyncVarSetter(value, ref knockedOutVfxData, 128uL, _Mirror_SyncVarHookDelegate_knockedOutVfxData);
		}
	}

	public KnockOutImmunity NetworkknockoutImmunityStatus
	{
		get
		{
			return knockoutImmunityStatus;
		}
		[param: In]
		set
		{
			GeneratedSyncVarSetter(value, ref knockoutImmunityStatus, 256uL, _Mirror_SyncVarHookDelegate_knockoutImmunityStatus);
		}
	}

	public StatusEffect NetworkstatusEffects
	{
		get
		{
			return statusEffects;
		}
		[param: In]
		set
		{
			GeneratedSyncVarSetter(value, ref statusEffects, 512uL, _Mirror_SyncVarHookDelegate_statusEffects);
		}
	}

	public DivingState NetworkdivingState
	{
		get
		{
			return divingState;
		}
		[param: In]
		set
		{
			GeneratedSyncVarSetter(value, ref divingState, 1024uL, _Mirror_SyncVarHookDelegate_divingState);
		}
	}

	public DiveType NetworkdiveType
	{
		get
		{
			return diveType;
		}
		[param: In]
		set
		{
			GeneratedSyncVarSetter(value, ref diveType, 2048uL, _Mirror_SyncVarHookDelegate_diveType);
		}
	}

	public RespawnState NetworkrespawnState
	{
		get
		{
			return respawnState;
		}
		[param: In]
		set
		{
			GeneratedSyncVarSetter(value, ref respawnState, 4096uL, _Mirror_SyncVarHookDelegate_respawnState);
		}
	}

	public event Action IsGroundedChanged;

	public event Action IsKnockedOutOrRecoveringChanged;

	public event Action HasKnockoutImmunityChanged;

	public event Action IsVisibleChanged;

	public event Action IsRespawningChanged;

	public event Action Teleported;

	public static event Action<PlayerMovement> AnyPlayerIsRespawningChanged;

	public static event Action LocalPlayerTeleportedToRespawnPosition;

	private static void NoClipChanged()
	{
		PlayerMovement localPlayerMovement = GameManager.LocalPlayerMovement;
		if (!(localPlayerMovement == null))
		{
			localPlayerMovement.GetComponentInChildren<Animator>().enabled = !NoClipEnabled;
			localPlayerMovement.rigidbody.detectCollisions = !NoClipEnabled;
		}
	}

	private void Awake()
	{
		PlayerInfo = GetComponent<PlayerInfo>();
		rigidbody = GetComponent<Rigidbody>();
		speed = GameManager.PlayerMovementSettings.DefaultMoveSpeed;
		verticalTerminalVelocity = GameManager.PlayerMovementSettings.DefaultTerminalFallingSpeed;
		base.transform.GetPositionAndRotation(out initialPosition, out initialRotation);
		syncDirection = SyncDirection.ClientToServer;
		GetComponentsInChildren(includeInactive: false, renderers);
		ApplyVisibility();
		UpdateEnabledColliders();
	}

	private void Start()
	{
		PlayerInfo.LevelBoundsTracker.LocalBoundsStateChanged += OnLocalBoundsStateChanged;
		PlayerInfo.AnimatorIo.Footstep += OnFootstep;
		CourseManager.PlayerDominationsChanged += OnPlayerDominationsChanged;
	}

	public void OnWillBeDestroyed()
	{
		if (knockedOutVfx != null)
		{
			knockedOutVfx.AsPoolable.Stop(ParticleSystemStopBehavior.StopEmittingAndClear);
			knockedOutVfx = null;
		}
		UpdateKnockoutImmunityVfx();
		UpdateRocketDriverSwingMissTrailEffects();
		CourseManager.PlayerDominationsChanged -= OnPlayerDominationsChanged;
		if (!BNetworkManager.IsChangingSceneOrShuttingDown)
		{
			PlayerInfo.LevelBoundsTracker.LocalBoundsStateChanged -= OnLocalBoundsStateChanged;
			PlayerInfo.AnimatorIo.Footstep -= OnFootstep;
		}
	}

	public override void OnStartServer()
	{
		serverKnockoutBlockedCommandRateLimiter = new AntiCheatRateChecker("Knockout blocked", base.connectionToClient.connectionId, 0.025f, 20, 50, 0.5f);
		serverInformKnockedOutCommandRateLimiter = new AntiCheatRateChecker("Inform knocked out", base.connectionToClient.connectionId, GameManager.PlayerMovementSettings.KnockoutDefaultGroundDuration * 0.5f, 5, 10, GameManager.PlayerMovementSettings.KnockoutDefaultGroundDuration * 2f);
		serverRespawnEffectCommandRateLimiter = new AntiCheatRateChecker("Respawn effect", base.connectionToClient.connectionId, 0.5f * (GameManager.MatchSettings.RespawnPostEliminationDelay + GameManager.MatchSettings.RespawnAnimationDuration), 5, 10, (GameManager.MatchSettings.RespawnPostEliminationDelay + GameManager.MatchSettings.RespawnAnimationDuration) * 2f);
		serverInformGroundedCommandRateLimiter = new AntiCheatRateChecker("Inform grounded", base.connectionToClient.connectionId, 0.05f, 10, 30, 1f, 3);
		serverSpringBootsLandingCommandRateLimiter = new AntiCheatRateChecker("Spring boots landing", base.connectionToClient.connectionId, 1f, 5, 10, 2f);
		serverInformTeleportedCommandRateLimiter = new AntiCheatRateChecker("Inform teleported", base.connectionToClient.connectionId, 0.05f, 10, 30, 2f);
		serverSpeedBoostEffectCommandRateLimiter = new AntiCheatRateChecker("Speed boost effect", base.connectionToClient.connectionId, 0.25f, 5, 10, 1f);
		serverOutOfBoundsEliminationExplosionCommandRateLimiter = new AntiCheatRateChecker("Out of bounds elimination explosion", base.connectionToClient.connectionId, GameManager.GolfSettings.OutOfBoundsEliminationTime * 0.5f, 5, 10, GameManager.GolfSettings.OutOfBoundsEliminationTime * 2f);
		serverGolfCartKnockoutEffectsCommandRateLimiter = new AntiCheatRateChecker("Golf cart knockout effects", base.connectionToClient.connectionId, 0.025f, 20, 50, 0.5f, 3);
		serverRestartInformCommandRateLimiter = new AntiCheatRateChecker("Player restart inform", base.connectionToClient.connectionId, 0.5f * (GameManager.MatchSettings.RespawnPostEliminationDelay + GameManager.MatchSettings.RespawnAnimationDuration), 5, 10, (GameManager.MatchSettings.RespawnPostEliminationDelay + GameManager.MatchSettings.RespawnAnimationDuration) * 2f);
		PlayerInfo.LevelBoundsTracker.AuthoritativeBoundsStateChanged += OnServerBoundsStateChanged;
	}

	public override void OnStopServer()
	{
		if (!BNetworkManager.IsChangingSceneOrShuttingDown)
		{
			PlayerInfo.LevelBoundsTracker.AuthoritativeBoundsStateChanged -= OnServerBoundsStateChanged;
		}
	}

	public override void OnStartLocalPlayer()
	{
		if (CameraModuleController.TryGetOrbitModule(out var orbitModule))
		{
			orbitModule.SetSubject(base.transform);
			Bounds subjectLocalBounds = new Bounds(uprightCollider.center, new Vector3(uprightCollider.radius, uprightCollider.height + uprightCollider.radius, uprightCollider.radius));
			orbitModule.SetSubjectLocalBounds(subjectLocalBounds);
			orbitModule.ForceUpdateModule();
		}
		rigidbody.constraints = (RigidbodyConstraints)80;
		rigidbody.useGravity = false;
		ResetTargetYaw();
		LocalPlayerUpdateVisibility();
		if (PlayerInfo.AsEntity.LevelBoundsTracker.AuthoritativeBoundsState.HasState(BoundsState.OutOfBounds))
		{
			localPlayerExplorerAchievementLastOutOfBoundsTimestamp = Time.timeAsDouble;
		}
		PlayerInfo.LevelBoundsTracker.AuthoritativeBoundsStateChanged += OnLocalPlayerBoundsStateChanged;
		PlayerInfo.AsGolfer.MatchResolutionChanged += OnLocalPlayerMatchResolutionChanged;
		PlayerInfo.AsSpectator.IsSpectatingChanged += OnLocalPlayerIsSpectatingChanged;
		PlayerInfo.AsHittable.WillApplyGolfSwingHitPhysics += OnLocalPlayerWillApplyGolfSwingHitPhysics;
		PlayerInfo.AsHittable.WillApplySwingProjectileHitPhysics += OnLocalPlayerWillApplySwingProjectileHitPhysics;
		PlayerInfo.AsHittable.WillApplyDiveHitPhysics += OnLocalPlayerWillApplyDiveHitPhysics;
		PlayerInfo.AsHittable.WillApplyItemHitPhysics += OnLocalPlayerWillApplyItemHitPhysics;
		PlayerInfo.AsHittable.WillApplyRocketLauncherBackBlastHitPhysics += OnLocalPlayerWillApplyRocketLauncherBackBlastHitPhysics;
		PlayerInfo.AsHittable.WillApplyRocketDriverSwingPostHitSpinHitPhysics += OnLocalPlayerWillWillApplyRocketDriverSwingPostHitSpinHitPhysics;
		PlayerInfo.AsHittable.WillApplyReturnedBallHitPhysics += OnLocalPlayerWillApplyReturnedBallHitPhysics;
		PlayerInfo.AsHittable.WillApplyScoreKnockbackPhysics += OnLocalPlayerWillApplyScoreKnockbackPhysics;
		PlayerInfo.AsHittable.WillApplyJumpPadPhysics += OnLocalPlayerWillApplyJumpPadPhysics;
		PlayerInfo.AsHittable.IsFrozenChanged += OnLocalPlayerIsFrozenChanged;
		PlayerCustomizationMenu.OnOpened += LocalPlayerUpdateVisibility;
		PlayerCustomizationMenu.OnClosed += LocalPlayerUpdateVisibility;
	}

	public override void OnStopLocalPlayer()
	{
		if (CameraModuleController.TryGetOrbitModule(out var orbitModule) && orbitModule.Subject == base.transform)
		{
			orbitModule.SetSubject(null);
		}
		PlayerCustomizationMenu.OnOpened -= LocalPlayerUpdateVisibility;
		PlayerCustomizationMenu.OnClosed -= LocalPlayerUpdateVisibility;
		if (!BNetworkManager.IsChangingSceneOrShuttingDown)
		{
			PlayerInfo.LevelBoundsTracker.AuthoritativeBoundsStateChanged -= OnLocalPlayerBoundsStateChanged;
			PlayerInfo.AsGolfer.MatchResolutionChanged -= OnLocalPlayerMatchResolutionChanged;
			PlayerInfo.AsSpectator.IsSpectatingChanged -= OnLocalPlayerIsSpectatingChanged;
			PlayerInfo.AsHittable.WillApplyGolfSwingHitPhysics -= OnLocalPlayerWillApplyGolfSwingHitPhysics;
			PlayerInfo.AsHittable.WillApplySwingProjectileHitPhysics += OnLocalPlayerWillApplySwingProjectileHitPhysics;
			PlayerInfo.AsHittable.WillApplyDiveHitPhysics -= OnLocalPlayerWillApplyDiveHitPhysics;
			PlayerInfo.AsHittable.WillApplyItemHitPhysics -= OnLocalPlayerWillApplyItemHitPhysics;
			PlayerInfo.AsHittable.WillApplyRocketLauncherBackBlastHitPhysics -= OnLocalPlayerWillApplyRocketLauncherBackBlastHitPhysics;
			PlayerInfo.AsHittable.WillApplyRocketDriverSwingPostHitSpinHitPhysics -= OnLocalPlayerWillWillApplyRocketDriverSwingPostHitSpinHitPhysics;
			PlayerInfo.AsHittable.WillApplyReturnedBallHitPhysics -= OnLocalPlayerWillApplyReturnedBallHitPhysics;
			PlayerInfo.AsHittable.WillApplyScoreKnockbackPhysics -= OnLocalPlayerWillApplyScoreKnockbackPhysics;
			PlayerInfo.AsHittable.WillApplyJumpPadPhysics -= OnLocalPlayerWillApplyJumpPadPhysics;
			PlayerInfo.AsHittable.IsFrozenChanged -= OnLocalPlayerIsFrozenChanged;
		}
	}

	public override void OnSerialize(NetworkWriter writer, bool initialState)
	{
		base.OnSerialize(writer, initialState);
	}

	public override void OnDeserialize(NetworkReader reader, bool initialState)
	{
		base.OnDeserialize(reader, initialState);
	}

	public void AwaitSpawning()
	{
		rigidbody.isKinematic = true;
	}

	[TargetRpc]
	public void RpcInformSpawned(Vector3 position, Quaternion rotation)
	{
		NetworkWriterPooled writer = NetworkWriterPool.Get();
		writer.WriteVector3(position);
		writer.WriteQuaternion(rotation);
		SendTargetRPCInternal(null, "System.Void PlayerMovement::RpcInformSpawned(UnityEngine.Vector3,UnityEngine.Quaternion)", -1207548897, writer, 0);
		NetworkWriterPool.Return(writer);
	}

	private void OnCollisionEnter(Collision collision)
	{
		Entity hitEntity;
		if (base.isLocalPlayer && collision.contactCount > 0)
		{
			if (collision.rigidbody != null)
			{
				hitEntity = collision.rigidbody.GetComponentInParent<Entity>(includeInactive: true);
			}
			else
			{
				hitEntity = collision.collider.GetComponentInParent<Entity>(includeInactive: true);
			}
			if (hitEntity != null && hitEntity.IsGolfCart)
			{
				HandleGolfCartCollision();
			}
			if (IsKnockedOut)
			{
				HandleKnockedOutCollision();
			}
			else if (divingState == DivingState.Diving)
			{
				HandleDivingCollision();
			}
		}
		void HandleDivingCollision()
		{
			if (!(hitEntity == null) && hitEntity.IsHittable)
			{
				Hittable asHittable = hitEntity.AsHittable;
				if (asHittable.DiveSettings.CanBeHit && !diveHitHittables.Contains(asHittable))
				{
					diveHitHittables.Add(asHittable);
					ContactPoint contact = collision.GetContact(0);
					Vector3 relativeHitVelocity = Vector3.Dot(contact.point.GetPointVelocity(previousWorldCenterOfMass, previousVelocity, previousAngularVelocity) - asHittable.AsEntity.GetNetworkedPointVelocity(contact.point), contact.normal) * contact.normal;
					asHittable.HitWithDive(relativeHitVelocity, this);
				}
			}
		}
		void HandleGolfCartCollision()
		{
			if (!IsKnockedOutOrRecovering && !PlayerInfo.AsHittable.IsFrozen && !(hitEntity.AsGolfCart == golfCartBeingEntered) && !golfCartsUnableToKnockOut.Contains(hitEntity.AsGolfCart) && !(hitEntity.GetNetworkedVelocity().sqrMagnitude < GameManager.GolfCartSettings.RunOverPlayerKnockoutMinSpeedSquared))
			{
				float sqrMagnitude = collision.relativeVelocity.sqrMagnitude;
				if (!(sqrMagnitude < GameManager.GolfCartSettings.RunOverPlayerKnockoutMinRelativeSpeedSquared))
				{
					float value = BMath.Sqrt(sqrMagnitude);
					float t = BMath.InverseLerpClamped(GameManager.GolfCartSettings.RunOverPlayerKnockoutMinRelativeSpeed, GameManager.GolfCartSettings.RunOverPlayerKnockoutMaxRelativeSpeed, value);
					float num = BMath.Lerp(GameManager.GolfCartSettings.RunOverPlayerKnockoutMinHorizontalKnockback, GameManager.GolfCartSettings.RunOverPlayerKnockoutMaxHorizontalKnockback, t);
					float num2 = BMath.Lerp(GameManager.GolfCartSettings.RunOverPlayerKnockoutMinVerticalKnockback, GameManager.GolfCartSettings.RunOverPlayerKnockoutMaxVerticalKnockback, t);
					Vector3 normalized = collision.GetContact(0).normal.Horizontalized().normalized;
					PlayerInfo.AsEntity.TemporarilyIgnoreCollisionsWith(hitEntity, 0.5f);
					Vector3 vector = num * normalized + num2 * Vector3.up;
					TryKnockOut(hitEntity.AsGolfCart.ResponsiblePlayer, KnockoutType.GolfCart, isLegSweep: false, base.transform.InverseTransformPoint(hitEntity.transform.position), 0f, vector, canBeBlockedByElectromagnetShield: true, ItemUseId.Invalid, fromSpecialState: false, canFallbackToUnground: true, out var _);
					Velocity += vector;
					Vector3 point = collision.GetContact(0).point;
					PlayGolfCartKnockoutEffectsForAllClients(base.transform.InverseTransformPoint(point));
					if (PlayerInfo.IsElectromagnetShieldActive)
					{
						PlayerInfo.PlayElectromagnetShieldHitForAllClients(point - PlayerInfo.ElectromagnetShieldCollider.transform.position);
					}
				}
			}
		}
		void HandleKnockedOutCollision()
		{
			if (!wasLastKnockoutLegSweep || !(BMath.GetTimeSince(knockoutTimestamp) < 0.3f))
			{
				ContactPoint contact = collision.GetContact(0);
				Vector3 pointVelocity = rigidbody.GetPointVelocity(contact.point);
				Vector3 vector = ((hitEntity != null) ? hitEntity.GetNetworkedPointVelocity(contact.point) : Vector3.zero);
				if ((Vector3.Dot(pointVelocity - vector, contact.normal) * contact.normal).sqrMagnitude > 0.25f)
				{
					PlayerInfo.AnimatorIo.TriggerKnockdownHitFlinch();
				}
			}
		}
	}

	private void Update()
	{
		UpdateOutOfBounds(forceConsiderAsGrounded: false);
		UpdateVfx();
		if (base.isLocalPlayer)
		{
			ProcessMovementInput();
			UpdateStatusEffects();
		}
		bool CanTickDownSpeedBoost()
		{
			if (!PlayerInfo.Inventory.IsUsingItemAtAll)
			{
				return true;
			}
			if (PlayerInfo.Inventory.GetEffectivelyEquippedItem() != ItemType.Coffee)
			{
				return true;
			}
			if (BMath.GetTimeSince(PlayerInfo.Inventory.ItemUseTimestamp) > GameManager.ItemSettings.CoffeeDrinkEffectStartTime)
			{
				return true;
			}
			return false;
		}
		void UpdateStatusEffects()
		{
			if (statusEffects.HasEffect(StatusEffect.SpeedBoost))
			{
				if (CanTickDownSpeedBoost())
				{
					SpeedBoostRemainingTime -= Time.deltaTime;
				}
				if (SpeedBoostRemainingTime <= 0f)
				{
					RemoveStatusEffect(StatusEffect.SpeedBoost);
				}
				else
				{
					continuousSpeedBoostTime += Time.deltaTime;
					if (continuousSpeedBoostTime > (float)GameManager.Achievements.CaffeinatedSpeedBoostTime && !SingletonBehaviour<DrivingRangeManager>.HasInstance && CourseManager.CountActivePlayers() > 1)
					{
						GameManager.AchievementsManager.Unlock(AchievementId.Caffeinated);
					}
				}
			}
		}
		void UpdateVfx()
		{
			if (isWadingInWater)
			{
				PlayerInfo.Vfx.SetWadingWaterWorldHeight(PlayerInfo.LevelBoundsTracker.CurrentOutOfBoundsHazardWorldHeightLocalOnly);
			}
		}
	}

	private void FixedUpdate()
	{
		if (!base.isLocalPlayer)
		{
			return;
		}
		if (!isVisible)
		{
			NetworksyncedVelocity = Vector3.zero;
			return;
		}
		if (PlayerInfo.ActiveGolfCartSeat.IsValid())
		{
			UpdateWaterState();
			NetworksyncedVelocity = Vector3.zero;
			return;
		}
		if (NoClipEnabled)
		{
			UpdateNoClip();
			NetworksyncedVelocity = Vector3.zero;
			return;
		}
		UpdateTimers();
		EnsureRotationConstraints();
		UpdateGroundingState(suppressLandingAnimation: false);
		UpdateWaterState();
		UpdateTerminalVelocity();
		if (isGrounded)
		{
			SnapToGround();
		}
		else
		{
			ApplyGravity();
		}
		UpdateRotationParameters();
		UpdatePhysicsParameters();
		if (!PlayerInfo.AsHittable.IsFrozen)
		{
			if (IsKnockedOutOrRecovering)
			{
				UpdateKnockOutState();
			}
			else if (divingState != DivingState.None)
			{
				UpdateDivingState();
			}
			else
			{
				ApplyRotation();
				ApplyMovement();
				ApplyHorizontalDrag();
				if (!isGrounded)
				{
					ApplyVerticalDrag();
				}
				ApplyRotationDrag();
			}
		}
		ApplyElectromagnetShieldRepulsion();
		movementSensitivityFactor = (isGrounded ? 1f : 0.2f);
		PlayerInfo.AnimatorIo.SetLocalVelocity(base.transform.InverseTransformDirection(Velocity), Time.fixedDeltaTime);
		previousWorldCenterOfMass = rigidbody.worldCenterOfMass;
		previousVelocity = rigidbody.linearVelocity;
		previousAngularVelocity = rigidbody.angularVelocity;
		NetworksyncedVelocity = rigidbody.linearVelocity;
		void ApplyElectromagnetShieldRepulsion()
		{
			if (!isVisible)
			{
				return;
			}
			foreach (PlayerInfo activeShield in ElectromagnetShieldManager.ActiveShields)
			{
				if (!(activeShield == PlayerInfo))
				{
					Vector3 position = activeShield.ElectromagnetShieldCollider.transform.position;
					Vector3 vector = (hittableCollider.enabled ? hittableCollider.ClosestPoint(position) : ((!divingCollider.enabled) ? rigidbody.centerOfMass : divingCollider.ClosestPoint(position)));
					float sqrMagnitude = (vector - position).sqrMagnitude;
					if (sqrMagnitude >= activeShield.ElectromagnetShieldCollider.radius * activeShield.ElectromagnetShieldCollider.radius)
					{
						break;
					}
					if (sqrMagnitude == 0f)
					{
						vector = rigidbody.centerOfMass;
					}
					float value = BMath.Sqrt(sqrMagnitude);
					float toMin = ((IsKnockedOutOrRecovering || PlayerInfo.AsHittable.IsFrozen) ? GameManager.ItemSettings.ElectromagnetShieldMaxPlayerKnockedOutRepulsionAcceleration : ((!isGrounded) ? GameManager.ItemSettings.ElectromagnetShieldMaxPlayerDefaultRepulsionAcceleration : GameManager.ItemSettings.ElectromagnetShieldMaxPlayerGroundedRepulsionAcceleration));
					Vector3 normalized = (vector - position).normalized;
					float num = BMath.RemapClamped(0f, activeShield.ElectromagnetShieldCollider.radius, toMin, 0f, value);
					if (IsKnockedOutOrRecovering || PlayerInfo.AsHittable.IsFrozen)
					{
						rigidbody.AddForceAtPosition(num * normalized, vector, ForceMode.Acceleration);
					}
					else
					{
						rigidbody.linearVelocity += num * Time.fixedDeltaTime * normalized;
					}
				}
			}
		}
		void EnsureRotationConstraints()
		{
			if (!IsKnockedOut && !PlayerInfo.AsHittable.IsFrozen)
			{
				Yaw = Yaw;
			}
		}
		void UpdateNoClip()
		{
			Yaw = cameraCorrectedWorldForward.GetYawDeg();
			Vector3 zero = Vector3.zero;
			zero += CameraModuleController.CurrentModule.GetCurrentForward() * moveVector2d.y;
			zero += Quaternion.AngleAxis(90f, Vector3.up) * cameraCorrectedWorldForward * moveVector2d.x;
			if (Keyboard.current.spaceKey.isPressed)
			{
				zero += Vector3.up;
			}
			if (Keyboard.current.fKey.isPressed)
			{
				zero -= Vector3.up;
			}
			float num = GameManager.PlayerMovementSettings.DefaultMoveSpeed * (Keyboard.current.leftShiftKey.isPressed ? 6f : 2f);
			Velocity = zero.normalized * num;
		}
		void UpdateTimers()
		{
			groundTime += Time.fixedDeltaTime;
		}
	}

	private void LateUpdate()
	{
		if (PlayerInfo.ActiveGolfCartSeat.IsValid())
		{
			FollowGolfCart();
		}
		if (base.isLocalPlayer && IsKnockedOut)
		{
			KnockoutState effectiveAnimationKnockoutState = GetEffectiveAnimationKnockoutState();
			PlayerInfo.AnimatorIo.TransitionKnockoutGroundednessTo((effectiveAnimationKnockoutState == KnockoutState.OnGround) ? 1f : 0f);
			if (effectiveAnimationKnockoutState == KnockoutState.OnGround)
			{
				float yawDeg = base.transform.InverseTransformDirection(knockoutGroundRaycastHit.normal).GetYawDeg();
				PlayerInfo.AnimatorIo.SetKnockoutWorldUpNormalizedLocalYaw(yawDeg / 180f);
			}
		}
		UpdateSpineStraightening();
		ApplyAirhornAirShake();
		void ApplyAirhornAirShake()
		{
			float timeSince = BMath.GetTimeSince(airhornHeadShakeStartTimestamp);
			if (!(timeSince >= 1f))
			{
				float num = 1f - BMath.EaseIn(timeSince / 1f);
				float num2 = 2.5f * num;
				float num3 = 7.5f * num;
				Quaternion quaternion = Quaternion.Euler(num2 * UnityEngine.Random.Range(-1f, 1f), 0f, num3 * UnityEngine.Random.Range(-1f, 1f));
				PlayerInfo.NeckBone.rotation = PlayerInfo.NeckBone.rotation * quaternion;
			}
		}
		void FollowGolfCart()
		{
			GolfCartMovement movement = PlayerInfo.ActiveGolfCartSeat.golfCart.Movement;
			movement.transform.GetPositionAndRotation(out var position, out var rotation);
			base.transform.SetPositionAndRotation(position, rotation);
			rigidbody.position = position;
			rigidbody.rotation = rotation;
			if (!movement.AsEntity.AsHittable.IsFrozen)
			{
				PlayerInfo.AnimatorIo.SetGolfCartSteering(movement.SmoothedSteering);
				PlayerInfo.AnimatorIo.SetGolfCartAcceleration(movement.SmoothedLocalAcceleration);
			}
		}
	}

	private void UpdateSpineStraightening()
	{
		if (PlayerInfo.AnimatorIo.SpineStraighteningWeight <= 0f)
		{
			straightenedSpine1Rotation = PlayerInfo.Spine1Bone.rotation;
			return;
		}
		Quaternion b = GetTargetSpine1Rotation();
		straightenedSpine1Rotation = Quaternion.Slerp(straightenedSpine1Rotation, b, 12f * Time.deltaTime);
		Vector3 eulerAngles = straightenedSpine1Rotation.eulerAngles;
		float y = eulerAngles.y;
		float y2 = PlayerInfo.Spine1Bone.rotation.eulerAngles.y;
		float value = (y - y2).WrapAngleDeg();
		value = BMath.Clamp(value, -55f, 55f);
		float y3 = (y2 + value).WrapAngleDeg();
		straightenedSpine1Rotation = Quaternion.Euler(eulerAngles.x, y3, eulerAngles.z);
		PlayerInfo.Spine1Bone.rotation = Quaternion.SlerpUnclamped(PlayerInfo.Spine1Bone.rotation, straightenedSpine1Rotation, PlayerInfo.AnimatorIo.SpineStraighteningWeight);
		Quaternion GetTargetSpine1Rotation()
		{
			Vector3 vector = PlayerInfo.HipBone.eulerAngles.WrapAngleDeg();
			if (PlayerInfo.NetworkedEquippedItem == ItemType.DuelingPistol)
			{
				float num = (base.isLocalPlayer ? BMath.Max(17.5f, PlayerInfo.AnimatorIo.AimingYawOffset) : PlayerInfo.AnimatorIo.AimingYawOffset);
				float y4 = (Yaw + num + 65f).WrapAngleDeg();
				return Quaternion.Euler(0f, y4, 0f);
			}
			if (PlayerInfo.NetworkedEquippedItem == ItemType.ElephantGun)
			{
				float num2 = ((base.isLocalPlayer && !PlayerInfo.Inventory.IsUsingItemAtAll) ? BMath.Max(12.5f, PlayerInfo.AnimatorIo.AimingYawOffset) : PlayerInfo.AnimatorIo.AimingYawOffset);
				float y5 = (Yaw + num2 + 55f).WrapAngleDeg();
				return Quaternion.Euler(0f, y5, 0f);
			}
			if (PlayerInfo.NetworkedEquippedItem == ItemType.RocketLauncher)
			{
				float num3 = (base.isLocalPlayer ? BMath.Max(15f, PlayerInfo.AnimatorIo.AimingYawOffset) : PlayerInfo.AnimatorIo.AimingYawOffset);
				float y6 = (Yaw + num3 + 23f).WrapAngleDeg();
				return Quaternion.Euler(0f, y6, 0f);
			}
			if (PlayerInfo.NetworkedEquippedItem == ItemType.FreezeBomb)
			{
				float y7 = (Yaw + (base.isLocalPlayer ? 57f : 47f)).WrapAngleDeg();
				return Quaternion.Euler(0f, y7, 0f);
			}
			return Quaternion.Euler(vector.x * 0.65f, Yaw, vector.z * 0.65f);
		}
	}

	public bool TryTriggerJump()
	{
		if (!CanJump())
		{
			return false;
		}
		TriggerJumpInternal();
		return true;
	}

	public bool TryDive()
	{
		return TryDiveInternal(DiveType.Regular);
	}

	private bool TryDiveInternal(DiveType type, Vector3 forcedVector = default(Vector3))
	{
		bool isElephantGunDive = type.IsElephantGunDive();
		if (!CanDive())
		{
			return false;
		}
		NetworkdiveType = type;
		SetDivingState(DivingState.Diving);
		diveStartTimestamp = Time.timeAsDouble;
		PlayerInfo.AnimatorIo.Dive(type);
		Unground();
		SetIsInSpringBootsJump(isInJump: false, fromLanding: false);
		if (isElephantGunDive)
		{
			ApplyElephantGunDive();
		}
		else if (type == DiveType.RocketDriverSwingMiss)
		{
			ApplyRocketDriverSwingMissDive();
		}
		else
		{
			ApplyRegularDive();
		}
		return true;
		void ApplyElephantGunDive()
		{
			if (forcedVector.sqrMagnitude < 0.001f)
			{
				Debug.LogError("An elephant gun dive should always receive a vector", base.gameObject);
			}
			else
			{
				Vector3 normalized = forcedVector.normalized;
				Vector3 vector = forcedVector.normalized * GameManager.ItemSettings.ElephantGunShotDiveSpeed;
				float num = (Yaw = (vector.IsWithin01DegFrom(Vector3.up) ? Yaw : (vector.GetYawDeg() + 180f).WrapAngleDeg()));
				targetYaw = num;
				Vector3 linearVelocity = rigidbody.linearVelocity;
				float num3 = Vector3.Dot(linearVelocity, normalized);
				if (num3 < 0f)
				{
					linearVelocity -= num3 * normalized;
				}
				linearVelocity += vector;
				linearVelocity.y = BMath.Max(linearVelocity.y, GameManager.ItemSettings.ElephantGunShotDiveMinUpwardsSpeed);
				rigidbody.linearVelocity = linearVelocity;
				rigidbody.angularVelocity = Vector3.zero;
			}
		}
		void ApplyRegularDive()
		{
			Vector3 vector = ((forcedVector.sqrMagnitude > 0.001f) ? forcedVector.Horizontalized().normalized : ((!(MoveVectorMagnitude > 0.1f)) ? base.transform.forward.Horizontalized().normalized : worldMoveVector3d.Horizontalized().normalized));
			float num = (Yaw = vector.GetYawDeg());
			targetYaw = num;
			rigidbody.linearVelocity = GameManager.PlayerMovementSettings.DiveHorizontalSpeed * vector + GameManager.PlayerMovementSettings.DiveUpwardsSpeed * Vector3.up;
			rigidbody.angularVelocity = Vector3.zero;
		}
		void ApplyRocketDriverSwingMissDive()
		{
			if (forcedVector.sqrMagnitude < 0.001f)
			{
				Debug.LogError("A rocket driver swing miss dive should always receive a vector", base.gameObject);
			}
			else
			{
				float num = (Yaw = forcedVector.GetYawDeg());
				targetYaw = num;
				rigidbody.linearVelocity = forcedVector;
				rigidbody.angularVelocity = Vector3.zero;
			}
		}
		bool CanDive()
		{
			if (!PlayerInfo.AsGolfer.IsInitialized)
			{
				return false;
			}
			if (isElephantGunDive)
			{
				return true;
			}
			if (!SingletonBehaviour<DrivingRangeManager>.HasInstance && CourseManager.MatchState <= MatchState.TeeOff)
			{
				return false;
			}
			if (IsKnockedOutOrRecovering)
			{
				return false;
			}
			if (IsRespawning)
			{
				return false;
			}
			if (PlayerInfo.AsHittable.IsFrozen)
			{
				return false;
			}
			if (PlayerInfo.AsSpectator.IsSpectating)
			{
				return false;
			}
			if (divingState != DivingState.None)
			{
				return false;
			}
			if (!PlayerInfo.AsGolfer.CanMove())
			{
				return false;
			}
			return true;
		}
	}

	public bool TryKnockOut(PlayerInfo responsiblePlayer, KnockoutType knockoutType, bool isLegSweep, Vector3 localOrigin, float distance, Vector3 incomingVelocityChange, bool canBeBlockedByElectromagnetShield, ItemUseId itemUseId, bool fromSpecialState, bool canFallbackToUnground, out bool isNewKnockout)
	{
		if (!CanBeKnockedOutBy(responsiblePlayer, canBeBlockedByElectromagnetShield, playBlockedEffects: true))
		{
			if (canFallbackToUnground && Velocity.y + incomingVelocityChange.y > GameManager.PlayerMovementSettings.VerticalVelocityGroundingThreshold)
			{
				Unground();
			}
			isNewKnockout = false;
			return false;
		}
		ItemType effectivelyEquippedItem = PlayerInfo.Inventory.GetEffectivelyEquippedItem();
		bool isKnockedOut = IsKnockedOut;
		if (!isKnockedOut)
		{
			this.knockoutType = knockoutType;
		}
		SetKnockOutState(KnockoutState.InAir);
		PlayerInfo.AsGolfer.InformLocalPlayerKnockedOut((responsiblePlayer != null) ? responsiblePlayer.AsGolfer : null, knockoutType);
		if (!isKnockedOut)
		{
			wasLastKnockoutLegSweep = isLegSweep;
			RemoveStatusEffect(StatusEffect.SpeedBoost);
			SetIsInSpringBootsJump(isInJump: false, fromLanding: false);
			PlayerInfo.AnimatorIo.KnockOut(isLegSweep);
			CmdInformKnockedOut(responsiblePlayer, knockoutType, localOrigin, distance, effectivelyEquippedItem, itemUseId, fromSpecialState);
		}
		isNewKnockout = !isKnockedOut;
		return true;
	}

	private bool CanBeKnockedOutBy(PlayerInfo player, bool canBeBlockedByElectromagnetShield, bool playBlockedEffects)
	{
		if (knockoutImmunityStatus.hasImmunity || IsKnockoutProtectedFromPlayer(player))
		{
			if (playBlockedEffects)
			{
				PlayKnockoutBlockedEffectForAllClients();
			}
			return false;
		}
		if (PlayerInfo.AsHittable.IsFrozen)
		{
			return false;
		}
		if (canBeBlockedByElectromagnetShield && PlayerInfo.IsElectromagnetShieldActive)
		{
			return false;
		}
		return true;
	}

	public bool CanBeFrozenBy(PlayerInfo player, bool playBlockedEffects)
	{
		return CanBeKnockedOutBy(player, canBeBlockedByElectromagnetShield: true, playBlockedEffects);
	}

	private void PlayKnockoutBlockedEffectForAllClients()
	{
		PlayKnockoutBlockedEffectInternal();
		CmdPlayKnockoutBlockedEffectForAllClients();
	}

	[Command]
	private void CmdPlayKnockoutBlockedEffectForAllClients(NetworkConnectionToClient sender = null)
	{
		if (base.isServer && base.isClient)
		{
			UserCode_CmdPlayKnockoutBlockedEffectForAllClients__NetworkConnectionToClient(sender);
			return;
		}
		NetworkWriterPooled writer = NetworkWriterPool.Get();
		SendCommandInternal("System.Void PlayerMovement::CmdPlayKnockoutBlockedEffectForAllClients(Mirror.NetworkConnectionToClient)", -919359957, writer, 0);
		NetworkWriterPool.Return(writer);
	}

	[TargetRpc]
	private void RpcPlayKnockoutBlockedEffect(NetworkConnectionToClient connection)
	{
		NetworkWriterPooled writer = NetworkWriterPool.Get();
		SendTargetRPCInternal(connection, "System.Void PlayerMovement::RpcPlayKnockoutBlockedEffect(Mirror.NetworkConnectionToClient)", -1941772974, writer, 0);
		NetworkWriterPool.Return(writer);
	}

	private void PlayKnockoutBlockedEffectInternal()
	{
		PlayEffectAtEndOfFrame();
		void PlayEffect()
		{
			if (VfxPersistentData.TryGetPooledVfx(VfxType.KnockOutBlocked, out var particleSystem))
			{
				UpdateColor(particleSystem);
				particleSystem.transform.SetParent(PlayerInfo.ChestBone);
				particleSystem.transform.localPosition = Vector3.zero;
				particleSystem.Play();
			}
			PlayerInfo.PlayerAudio.PlayKnockoutImmunityBlockedKnockoutLocalOnly();
		}
		async void PlayEffectAtEndOfFrame()
		{
			await UniTask.WaitForEndOfFrame();
			await UniTask.WaitForEndOfFrame();
			if (!(this == null))
			{
				PlayEffect();
			}
		}
		void UpdateColor(PoolableParticleSystem knockoutBlockedVfx)
		{
			if (isKnockoutProtectedFromLocalPlayer || knockoutImmunityVfx == null || !knockoutImmunityVfx.IsPlaying)
			{
				knockoutBlockedVfx.GetComponent<KnockOutVfxVisuals>().SetColor(KnockOutVfxColor.Red);
			}
			else if (knockoutImmunityVfx != null)
			{
				knockoutBlockedVfx.GetComponent<KnockOutVfxVisuals>().SetColor(knockoutImmunityVfx.GetComponent<KnockOutVfxVisuals>().CurrentColor);
			}
		}
	}

	[Command]
	private void CmdInformKnockedOut(PlayerInfo responsiblePlayer, KnockoutType knockoutType, Vector3 localOrigin, float distance, ItemType heldItem, ItemUseId itemUseId, bool fromSpecialState)
	{
		if (base.isServer && base.isClient)
		{
			UserCode_CmdInformKnockedOut__PlayerInfo__KnockoutType__Vector3__Single__ItemType__ItemUseId__Boolean(responsiblePlayer, knockoutType, localOrigin, distance, heldItem, itemUseId, fromSpecialState);
			return;
		}
		NetworkWriterPooled writer = NetworkWriterPool.Get();
		writer.WriteNetworkBehaviour(responsiblePlayer);
		GeneratedNetworkCode._Write_KnockoutType(writer, knockoutType);
		writer.WriteVector3(localOrigin);
		writer.WriteFloat(distance);
		GeneratedNetworkCode._Write_ItemType(writer, heldItem);
		GeneratedNetworkCode._Write_ItemUseId(writer, itemUseId);
		writer.WriteBool(fromSpecialState);
		SendCommandInternal("System.Void PlayerMovement::CmdInformKnockedOut(PlayerInfo,KnockoutType,UnityEngine.Vector3,System.Single,ItemType,ItemUseId,System.Boolean)", 924411333, writer, 0);
		NetworkWriterPool.Return(writer);
	}

	[Server]
	public void ServerInformKnockedOut(PlayerInfo responsiblePlayer, KnockoutType knockoutType, Vector3 localOrigin, float distance, ItemType heldItem, ItemUseId itemUseId, bool fromSpecialState)
	{
		if (!NetworkServer.active)
		{
			Debug.LogWarning("[Server] function 'System.Void PlayerMovement::ServerInformKnockedOut(PlayerInfo,KnockoutType,UnityEngine.Vector3,System.Single,ItemType,ItemUseId,System.Boolean)' called when server was not active");
		}
		else
		{
			InformKnockedOutInternal(responsiblePlayer, knockoutType, localOrigin, distance, heldItem, itemUseId, fromSpecialState);
		}
	}

	private void InformKnockedOutInternal(PlayerInfo responsiblePlayer, KnockoutType knockoutType, Vector3 localOrigin, float distance, ItemType heldItem, ItemUseId itemUseId, bool fromSpecialState)
	{
		if (serverInformKnockedOutCommandRateLimiter.RegisterHit())
		{
			this.knockoutType = knockoutType;
			CourseManager.InformPlayerKnockedOut(this, responsiblePlayer, knockoutType, out var knockoutCounted);
			if (responsiblePlayer == PlayerInfo)
			{
				CourseManager.MarkLatestValidKnockout(PlayerInfo, itemUseId);
			}
			else if (knockoutCounted)
			{
				CourseManager.MarkLatestValidKnockout(PlayerInfo, itemUseId);
				bool authoritativeIsOnGreen = PlayerInfo.LevelBoundsTracker.AuthoritativeIsOnGreen;
				responsiblePlayer.RpcInformKnockedOutOtherPlayer(knockoutType, localOrigin, distance, heldItem, authoritativeIsOnGreen, fromSpecialState);
			}
		}
	}

	public void AlignWithCameraImmediately()
	{
		float num = (Yaw = GameManager.Camera.transform.forward.GetYawDeg());
		targetYaw = num;
		base.transform.rotation = rigidbody.rotation;
		if (PlayerInfo.AnimatorIo.SpineStraighteningWeight > 0f)
		{
			PlayerInfo.AnimatorIo.UpdateAimingAngleInstantly();
			UpdateSpineStraightening();
		}
	}

	public void SuppressMovementUntilInputReleased()
	{
		movementSuppressedUntilInputReleased = true;
		ProcessMovementInput();
	}

	public void ReturnToBounds()
	{
		if (base.isLocalPlayer)
		{
			ReturnToBoundsInternal();
		}
		else if (base.isServer)
		{
			RpcReturnToBounds(base.connectionToClient);
		}
		else
		{
			Debug.LogError(base.name + " attempted to initiate a return to bounds, but they're neither the local player or the server", base.gameObject);
		}
	}

	public Bounds GetOrbitCameraSubjectLocalBounds()
	{
		return new Bounds(uprightCollider.center, new Vector3(uprightCollider.radius, uprightCollider.height + uprightCollider.radius, uprightCollider.radius));
	}

	public bool TryBeginRespawn(bool isRestart, RespawnTarget respawnTarget)
	{
		if (!base.isLocalPlayer && !base.isServer)
		{
			Debug.LogError("Only the local player and the server are allowed to initiate a respawn", base.gameObject);
			return false;
		}
		if (!CanBeginRespawning())
		{
			return false;
		}
		if (!base.isLocalPlayer)
		{
			RpcBeginRespawn(isRestart, respawnTarget);
		}
		else
		{
			LocalPlayerBeginRespawn(isRestart, respawnTarget);
		}
		if (isRestart)
		{
			VfxManager.ServerPlayPooledVfxForAllClients(VfxType.PlayerRestart, base.transform.position, Quaternion.identity);
			ServerPlayRestartSoundForAllClients();
		}
		return true;
	}

	[TargetRpc]
	private void RpcBeginRespawn(bool isRestart, RespawnTarget respawnTarget)
	{
		NetworkWriterPooled writer = NetworkWriterPool.Get();
		writer.WriteBool(isRestart);
		GeneratedNetworkCode._Write_RespawnTarget(writer, respawnTarget);
		SendTargetRPCInternal(null, "System.Void PlayerMovement::RpcBeginRespawn(System.Boolean,RespawnTarget)", -703314959, writer, 0);
		NetworkWriterPool.Return(writer);
	}

	private void LocalPlayerBeginRespawn(bool isRestart, RespawnTarget respawnTarget)
	{
		if (!base.isLocalPlayer)
		{
			Debug.LogError("Only the local player is allowed to respawn themselves", base.gameObject);
		}
		else if (CanBeginRespawning())
		{
			respawnRoutine = StartCoroutine(RespawnRoutine());
		}
		void PlayRespawnEffectsForAllClients(Checkpoint checkpoint)
		{
			PlayRespawnEffectsInternal(checkpoint);
			CmdPlayRespawnEffectsForAllClients(checkpoint);
		}
		IEnumerator RespawnRoutine()
		{
			NetworkrespawnState = RespawnState.WaitingToRespawn;
			CancelKnockoutImmunity();
			if (!isRestart && PlayerInfo.AsGolfer.LocalPlayerLatestImmediateEliminationReason == EliminationReason.FellIntoWater)
			{
				SetIsDrowning(isDrowning: true);
			}
			else
			{
				rigidbody.isKinematic = true;
				if (!isRestart && PlayerInfo.AsGolfer.LocalPlayerLatestImmediateEliminationReason == EliminationReason.FellIntoFog)
				{
					if (base.isServer)
					{
						VfxManager.ServerPlayPooledVfxForAllClients(VfxType.FogPlayerOutOfBounds, PlayerInfo.ChestBone.position, Quaternion.identity);
					}
					else
					{
						VfxManager.ClientPlayPooledVfxForAllClients(VfxType.FogPlayerOutOfBounds, PlayerInfo.ChestBone.position, Quaternion.identity);
					}
				}
			}
			LocalPlayerUpdateVisibility();
			LocalPlayerUpdateIsOutOfBoundsMessageShown();
			RemoveStatusEffect(StatusEffect.SpeedBoost);
			double startTime = Time.timeAsDouble;
			while (BMath.GetTimeSince(startTime) < GameManager.MatchSettings.RespawnPostEliminationDelay)
			{
				yield return null;
				if (PlayerInfo.ActiveGolfCartSeat.IsValid())
				{
					PlayerInfo.ExitGolfCart(GolfCartExitType.Default);
				}
			}
			NetworkrespawnState = RespawnState.ActivelyRespawning;
			rigidbody.isKinematic = true;
			if (!isRestart && PlayerInfo.AsGolfer.LocalPlayerLatestImmediateEliminationReason == EliminationReason.FellIntoWater)
			{
				if (base.isServer)
				{
					VfxManager.ServerPlayPooledVfxForAllClients(VfxType.WaterPlayerOutOfBounds, PlayerInfo.ChestBone.position, Quaternion.identity);
				}
				else
				{
					VfxManager.ClientPlayPooledVfxForAllClients(VfxType.WaterPlayerOutOfBounds, PlayerInfo.ChestBone.position, Quaternion.identity);
				}
			}
			TeleportToRespawnPosition(out var checkpoint);
			PlayRespawnEffectsForAllClients(checkpoint);
			LocalPlayerUpdateVisibility();
			SetIsDrowning(isDrowning: false);
			yield return new WaitForSeconds(GameManager.MatchSettings.RespawnAnimationDuration);
			NetworkrespawnState = RespawnState.None;
			rigidbody.isKinematic = false;
			LocalPlayerUpdateVisibility();
			LocalPlayerUpdateIsOutOfBoundsMessageShown();
			if (isRestart)
			{
				if (respawnTarget != RespawnTarget.Ball && !SingletonBehaviour<DrivingRangeManager>.HasInstance && PlayerInfo.AsGolfer.OwnBall != null)
				{
					PlayerInfo.AsGolfer.CmdRestartBall();
				}
				CmdClientInformRestarted();
			}
			StartKnockoutImmunity(fromPlayerAggression: false);
		}
		void TeleportToRespawnPosition(out Checkpoint checkpoint)
		{
			GetRespawnPosition(respawnTarget, out checkpoint, out var position, out var rotation);
			Teleport(position, rotation, resetState: true);
			PlayerInfo.LevelBoundsTracker.CmdInformTeleportedIntoBounds(position, rotation);
			UpdateGroundingState(suppressLandingAnimation: true);
			if (!base.isServer)
			{
				OutOfBoundsMessage.ForceHideTemporarily((float)NetworkTime.rtt * 3f);
			}
			PlayerMovement.LocalPlayerTeleportedToRespawnPosition?.Invoke();
		}
	}

	public void GetRespawnPosition(RespawnTarget respawnTarget, out Checkpoint checkpoint, out Vector3 position, out Quaternion rotation)
	{
		checkpoint = null;
		if (respawnTarget == RespawnTarget.TeeOrCheckpoint && CheckpointManager.TryGetLocalPlayerActiveCheckpoint(out checkpoint))
		{
			position = checkpoint.GetRespawnPosition();
			rotation = checkpoint.transform.rotation;
		}
		else if (respawnTarget == RespawnTarget.Ball && PlayerInfo.AsGolfer.OwnBall != null && PlayerInfo.AsGolfer.OwnBall.IsStationary)
		{
			position = PlayerInfo.AsGolfer.OwnBall.transform.position;
			rotation = initialRotation;
		}
		else
		{
			position = initialPosition;
			rotation = initialRotation;
		}
	}

	[Command]
	private void CmdClientInformRestarted()
	{
		if (base.isServer && base.isClient)
		{
			UserCode_CmdClientInformRestarted();
			return;
		}
		NetworkWriterPooled writer = NetworkWriterPool.Get();
		SendCommandInternal("System.Void PlayerMovement::CmdClientInformRestarted()", -434584613, writer, 0);
		NetworkWriterPool.Return(writer);
	}

	[Command]
	private void CmdPlayRespawnEffectsForAllClients(Checkpoint checkpoint, NetworkConnectionToClient sender = null)
	{
		if (base.isServer && base.isClient)
		{
			UserCode_CmdPlayRespawnEffectsForAllClients__Checkpoint__NetworkConnectionToClient(checkpoint, sender);
			return;
		}
		NetworkWriterPooled writer = NetworkWriterPool.Get();
		writer.WriteNetworkBehaviour(checkpoint);
		SendCommandInternal("System.Void PlayerMovement::CmdPlayRespawnEffectsForAllClients(Checkpoint,Mirror.NetworkConnectionToClient)", -862312768, writer, 0);
		NetworkWriterPool.Return(writer);
	}

	[TargetRpc]
	private void RpcPlayRespawnEffects(NetworkConnectionToClient connection, Checkpoint checkpoint)
	{
		NetworkWriterPooled writer = NetworkWriterPool.Get();
		writer.WriteNetworkBehaviour(checkpoint);
		SendTargetRPCInternal(connection, "System.Void PlayerMovement::RpcPlayRespawnEffects(Mirror.NetworkConnectionToClient,Checkpoint)", -1168729551, writer, 0);
		NetworkWriterPool.Return(writer);
	}

	private void PlayRespawnEffectsInternal(Checkpoint checkpoint)
	{
		if (checkpoint != null)
		{
			checkpoint.PlayRespawnAnimation();
		}
		PlayerInfo.PlayerAudio.PlayRespawnLocalOnly();
		RespawnVfx component;
		if (!VfxPersistentData.TryGetPooledVfx(VfxType.Respawn, out var particleSystem))
		{
			Debug.LogError("Failed to get respawn VFX");
		}
		else if (!particleSystem.TryGetComponent<RespawnVfx>(out component))
		{
			Debug.LogError("Pooled VFX does not have the RespawnVfx component");
			particleSystem.ReturnToPool();
		}
		else
		{
			component.transform.SetParent(base.transform);
			component.transform.SetLocalPositionAndRotation(Vector3.zero, Quaternion.identity);
			component.PlayAnimation();
		}
	}

	private void CancelRespawn()
	{
		if (IsRespawning)
		{
			NetworkrespawnState = RespawnState.None;
			rigidbody.isKinematic = true;
			if (respawnRoutine != null)
			{
				StopCoroutine(respawnRoutine);
			}
			LocalPlayerUpdateVisibility();
			LocalPlayerUpdateIsOutOfBoundsMessageShown();
			SetIsDrowning(isDrowning: false);
			PlayerInfo.PlayerAudio.CancelRespawnForAllClients();
		}
	}

	private bool CanBeginRespawning()
	{
		if (!PlayerInfo.AsGolfer.IsInitialized)
		{
			return false;
		}
		if (IsRespawning)
		{
			return false;
		}
		if (PlayerInfo.AsGolfer.IsMatchResolved)
		{
			return false;
		}
		return true;
	}

	[Server]
	private void ServerPlayRestartSoundForAllClients()
	{
		if (!NetworkServer.active)
		{
			Debug.LogWarning("[Server] function 'System.Void PlayerMovement::ServerPlayRestartSoundForAllClients()' called when server was not active");
			return;
		}
		PlayRestartSoundInternal();
		foreach (NetworkConnectionToClient value in NetworkServer.connections.Values)
		{
			if (value != NetworkServer.localConnection)
			{
				RpcPlayRestartSound(value);
			}
		}
	}

	[TargetRpc]
	private void RpcPlayRestartSound(NetworkConnectionToClient connection)
	{
		NetworkWriterPooled writer = NetworkWriterPool.Get();
		SendTargetRPCInternal(connection, "System.Void PlayerMovement::RpcPlayRestartSound(Mirror.NetworkConnectionToClient)", 1392793859, writer, 0);
		NetworkWriterPool.Return(writer);
	}

	private void PlayRestartSoundInternal()
	{
		RuntimeManager.PlayOneShot(GameManager.AudioSettings.PlayerRestartEvent, base.transform.position);
	}

	[TargetRpc]
	public void RpcSetIsForceHidden(bool isForcedHidden)
	{
		NetworkWriterPooled writer = NetworkWriterPool.Get();
		writer.WriteBool(isForcedHidden);
		SendTargetRPCInternal(null, "System.Void PlayerMovement::RpcSetIsForceHidden(System.Boolean)", 646273394, writer, 0);
		NetworkWriterPool.Return(writer);
	}

	public void InformWillBeEliminated()
	{
		if (PlayerInfo.AsGolfer.LocalPlayerLatestImmediateEliminationReason == EliminationReason.FellIntoWater)
		{
			LocalPlayerUpdateVisibilityDelayed(GameManager.MatchSettings.EliminationInWaterDrowningDuration + 0.05f);
		}
	}

	public void InformIsAimingItemChanged()
	{
		isStrafing = PlayerInfo.Inventory.IsAimingItem;
		if (isStrafing)
		{
			BlendStrafeStrengthTo(1f, 0.1f);
		}
		else
		{
			BlendStrafeStrengthTo(0f, 0.1f);
		}
	}

	public void InformShotElephantGun(Vector3 shotDirection, bool isFinalShot)
	{
		TryDiveInternal((!isFinalShot) ? DiveType.ElephantGun : DiveType.ElephantGunFinalShot, -shotDirection);
	}

	public void InformReactedToAirhorn()
	{
		airhornHeadShakeStartTimestamp = Time.timeAsDouble;
	}

	public void InformEnteringGolfCart(GolfCartInfo golfCart)
	{
		golfCartBeingEntered = golfCart;
		if (timeOutGolfCartBeingEnteredRoutine != null)
		{
			StopCoroutine(timeOutGolfCartBeingEnteredRoutine);
		}
		timeOutGolfCartBeingEnteredRoutine = StartCoroutine(TimeOutGolfCartBeingEnteredRoutine());
		IEnumerator TimeOutGolfCartBeingEnteredRoutine()
		{
			float seconds = BMath.Max(1f, (float)NetworkTime.rtt * 4f);
			yield return new WaitForSeconds(seconds);
			golfCartBeingEntered = null;
		}
	}

	public void InformNoLongerEnteringGolfCartBeingEntered(GolfCartInfo golfCart)
	{
		if (golfCart == golfCartBeingEntered)
		{
			golfCartBeingEntered = null;
		}
	}

	public void InformEnteredGolfCart()
	{
		UpdateEnabledColliders();
		PlayerInfo.Vfx.SetSpeedUpVfxHidden(hidden: true);
		if (base.isLocalPlayer)
		{
			SetDivingState(DivingState.None);
		}
	}

	public void InformLocalPlayerExitedGolfCart(GolfCartSeat previousSeat, GolfCartExitType exitType)
	{
		if (!previousSeat.IsValid())
		{
			return;
		}
		Unground();
		previousSeat.golfCart.GetExitData(previousSeat.seat, exitType, backup: false, out var worldPosition, out var worldYaw);
		if (!CanExitToPosition(worldPosition, out worldPosition))
		{
			previousSeat.golfCart.GetExitData(previousSeat.seat, exitType, backup: true, out worldPosition, out worldYaw);
			if (!CanExitToPosition(worldPosition, out worldPosition))
			{
				GetFallbackExitData(out worldPosition, out worldYaw);
			}
		}
		Quaternion rotation = Quaternion.Euler(0f, worldYaw, 0f);
		Teleport(worldPosition, rotation, resetState: false);
		Yaw = worldYaw;
		targetYaw = worldYaw;
		isGroundedAtAllInitialized = false;
		switch (exitType)
		{
		case GolfCartExitType.Dive:
			TryDiveInternal(DiveType.Regular, Quaternion.Euler(0f, worldYaw, 0f) * Vector3.forward);
			rigidbody.linearVelocity += previousSeat.golfCart.AsEntity.Rigidbody.linearVelocity;
			break;
		default:
			rigidbody.linearVelocity = previousSeat.golfCart.AsEntity.Rigidbody.linearVelocity;
			break;
		case GolfCartExitType.Knockout:
			break;
		}
		PlayerInfo.AsEntity.TemporarilyIgnoreCollisionsWith(previousSeat.golfCart.AsEntity, 0.25f);
		bool CanExitToPosition(Vector3 desiredPosition, out Vector3 position)
		{
			Vector3 vector = (position = desiredPosition);
			if (drawGolfCartExitDebug)
			{
				BDebug.DrawWireSphere(vector, 0.1f, Color.blue, 5f);
			}
			float worldHeightAtPoint = TerrainManager.GetWorldHeightAtPoint(position);
			float num = worldHeightAtPoint - position.y;
			if (0f < num && num <= 2f)
			{
				if (drawGolfCartExitDebug)
				{
					BDebug.DrawLine(position, position + num * Vector3.up, Color.red, 5f);
				}
				position.y = worldHeightAtPoint;
			}
			(Vector3 centerAlongAxis, Vector3 centerOppositeAxis) capsuleSphereCenters = BGeo.GetCapsuleSphereCenters(position + uprightCollider.center, Vector3.up, uprightCollider.radius, uprightCollider.height);
			Vector3 item = capsuleSphereCenters.centerAlongAxis;
			Vector3 item2 = capsuleSphereCenters.centerOppositeAxis;
			int num2 = Physics.OverlapCapsuleNonAlloc(item, item2, uprightCollider.radius, layerMask: GameManager.LayerSettings.PlayerGroundableMask, results: PlayerGolfer.overlappingSingleColliderBuffer);
			if (drawGolfCartExitDebug)
			{
				BDebug.DrawWireCapsule(item, item2, uprightCollider.radius, (num2 > 0) ? Color.red : Color.green, 5f);
			}
			int num3 = 0;
			while (num2 > 0)
			{
				Collider collider = PlayerGolfer.overlappingSingleColliderBuffer[0];
				if (!Physics.ComputePenetration(uprightCollider, position, Quaternion.identity, collider, collider.transform.position, collider.transform.rotation, out var direction, out var distance))
				{
					if (drawGolfCartExitDebug)
					{
						BDebug.DrawWireCapsule(item, item2, uprightCollider.radius, Color.green, 5f);
					}
					break;
				}
				if (drawGolfCartExitDebug)
				{
					BDebug.DrawCapsuleCast(item, item2, uprightCollider.radius, direction * distance, Color.red, 5f);
				}
				position += direction * (distance + 0.001f);
				num2 = Physics.OverlapCapsuleNonAlloc(item, item2, uprightCollider.radius, layerMask: GameManager.LayerSettings.PlayerGroundableMask, results: PlayerGolfer.overlappingSingleColliderBuffer);
				num3++;
			}
			return (position - vector).sqrMagnitude < 25f;
		}
		void GetFallbackExitData(out Vector3 position, out float yaw)
		{
			Ray ray = new Ray(previousSeat.golfCart.transform.TransformPoint(0.4f * Vector3.up) + 200f * Vector3.up, Vector3.down);
			(Vector3 centerAlongAxis, Vector3 centerOppositeAxis) capsuleSphereCenters = BGeo.GetCapsuleSphereCenters(ray.origin + uprightCollider.center, Vector3.up, uprightCollider.radius, uprightCollider.height);
			Vector3 item = capsuleSphereCenters.centerAlongAxis;
			Vector3 item2 = capsuleSphereCenters.centerOppositeAxis;
			int num = Physics.RaycastNonAlloc(ray, layerMask: GameManager.LayerSettings.PlayerGroundableMask, results: PlayerGolfer.raycastHitBuffer, maxDistance: 203f);
			RaycastHit raycastHit = new RaycastHit
			{
				point = ray.origin,
				distance = float.MinValue
			};
			for (int i = 0; i < num; i++)
			{
				RaycastHit raycastHit2 = PlayerGolfer.raycastHitBuffer[i];
				if (!(raycastHit2.distance < raycastHit.distance))
				{
					Vector3 vector = ray.direction * raycastHit2.distance;
					if (Physics.OverlapCapsuleNonAlloc(item + vector, item2 + vector, uprightCollider.radius, layerMask: GameManager.LayerSettings.PlayerGroundableMask, results: PlayerGolfer.overlappingSingleColliderBuffer) <= 0)
					{
						raycastHit = raycastHit2;
					}
				}
			}
			position = raycastHit.point;
			yaw = previousSeat.golfCart.transform.forward.GetYawDeg();
		}
	}

	public void InformExitedGolfCart(GolfCartInfo golfCart)
	{
		UpdateEnabledColliders();
		PlayerInfo.Vfx.SetSpeedUpVfxHidden(hidden: false);
		if (base.isLocalPlayer)
		{
			if (rigidbody.isKinematic)
			{
				rigidbody.linearVelocity = Vector3.zero;
				rigidbody.angularVelocity = Vector3.zero;
			}
			TemporarilyIgnoreKnockoutsFrom(golfCart);
		}
		async void TemporarilyIgnoreKnockoutsFrom(GolfCartInfo item)
		{
			golfCartsUnableToKnockOut.Add(item);
			await UniTask.WaitForSeconds(GameManager.PlayerMovementSettings.ExitedGolfcartKnockoutImmunityDuration);
			if (!(this == null))
			{
				golfCartsUnableToKnockOut.Remove(item);
			}
		}
	}

	public void InformMissedRocketDriverSwing(Vector3 swingDirection, float swingNormalizedPower)
	{
		Vector3 forcedVector = BMath.Remap(GameManager.ItemSettings.RocketDriverBaseNormalizedSwingPower, GameManager.ItemSettings.RocketDriverFullNormalizedSwingPower, GameManager.ItemSettings.RocketDriverBaseSwingMissDiveSpeed, GameManager.ItemSettings.RocketDriverFullSwingMissDiveSpeed, swingNormalizedPower) * swingDirection;
		forcedVector.y = BMath.Max(GameManager.ItemSettings.RocketDriverSwingMissMinUpwardsSpeed, forcedVector.y);
		TryDiveInternal(DiveType.RocketDriverSwingMiss, forcedVector);
	}

	public void InformFrozeGolfCart(int otherPassengerCount)
	{
		AddSpeedBoost(GameManager.PlayerMovementSettings.KnockOutSpeedBoostDuration * (float)otherPassengerCount);
	}

	private void ProcessMovementInput()
	{
		Vector2 vector;
		Vector2 vector2;
		if (ShouldSuppressMovementInput(out var suppressInstantly))
		{
			vector = rawMoveVector2d;
			vector2 = Vector2.zero;
		}
		else
		{
			vector = rawMoveVector2d;
			vector2 = moveVector2d;
			if (ShouldClampToWalkingSpeed())
			{
				vector2 *= GameManager.PlayerMovementSettings.WalkSpeedFactor;
			}
		}
		OrbitCameraModule orbitModule;
		float y = (CameraModuleController.TryGetOrbitModule(out orbitModule) ? orbitModule.Yaw : 0f);
		RawMoveVectorMagnitude = vector.magnitude;
		MoveVectorMagnitude = vector2.magnitude;
		if (RawMoveVectorMagnitude < MoveVectorMagnitude)
		{
			vector2 = vector;
			MoveVectorMagnitude = RawMoveVectorMagnitude;
		}
		Quaternion quaternion = Quaternion.Euler(0f, y, 0f);
		cameraCorrectedWorldForward = quaternion * Vector3.forward;
		cameraCorrectedWorldMoveDirection = quaternion * vector2.AsHorizontal3();
		worldMoveVector3d = quaternion * vector2.AsHorizontal3();
		worldMoveVectorYaw = worldMoveVector3d.GetYawDeg();
		rawWorldMoveVector3d = quaternion * vector.AsHorizontal3();
		rawWorldMoveVectorYaw = rawWorldMoveVector3d.GetYawDeg();
		if (!worldMoveVector3d.Approximately(Vector3.zero))
		{
			persistentWorldMoveDirection2dRaw = worldMoveVector3d.AsHorizontal2().normalized;
		}
		Vector3 vector3 = Quaternion.Euler(0f, 0f - base.transform.forward.GetYawDeg(), 0f) * worldMoveVector3d;
		localSmoothedMoveVector3d = (suppressInstantly ? vector3 : Vector3.Lerp(localSmoothedMoveVector3d, vector3, 20f * movementSensitivityFactor * Time.deltaTime));
		PlayerInfo.AnimatorIo.SetMovementInput(MoveVectorMagnitude, localSmoothedMoveVector3d);
		bool ShouldClampToWalkingSpeed()
		{
			if (isStrafing)
			{
				return true;
			}
			if (PlayerInfo.Input.IsHoldingWalk)
			{
				return true;
			}
			return false;
		}
		bool ShouldSuppressMovementInput(out bool reference)
		{
			reference = false;
			if (!CanMove())
			{
				return true;
			}
			if (divingState != DivingState.None)
			{
				return true;
			}
			if (IsRespawning)
			{
				return true;
			}
			if (movementSuppressedUntilInputReleased)
			{
				if (!(rawMoveVector2d.magnitude <= 0.05f))
				{
					reference = true;
					return true;
				}
				movementSuppressedUntilInputReleased = false;
			}
			return false;
		}
	}

	private void UpdateRotationParameters()
	{
		UpdateTargetYaw();
		UpdateRotationSpeedFactor();
	}

	private void UpdateTargetYaw()
	{
		if ((isGrounded || PlayerInfo.Inventory.IsAimingItem) && divingState == DivingState.None)
		{
			if ((PlayerInfo.AsGolfer.IsAimingSwing || PlayerInfo.Inventory.IsAimingItem) && CameraModuleController.TryGetOrbitModule(out var orbitModule))
			{
				targetYaw = orbitModule.transform.forward.GetYawDeg();
			}
			else if (MoveVectorMagnitude > 0.01f)
			{
				targetYaw = worldMoveVectorYaw;
			}
			else
			{
				ResetTargetYaw();
			}
		}
	}

	private void ResetTargetYaw()
	{
		targetYaw = Yaw;
	}

	private void UpdateRotationSpeedFactor()
	{
		turningOnADime = BMath.Abs((worldMoveVectorYaw - Yaw).WrapAngleDeg()) > 130f && Vector3.Dot(Velocity, base.transform.forward) > 0.5f;
		float value = (targetYaw - Yaw).WrapAngleDeg();
		if (!isGrounded)
		{
			rotationSpeedFactor = 1.25f;
		}
		else if (PlayerInfo.AsGolfer.IsAimingSwing || PlayerInfo.Inventory.IsAimingItem)
		{
			rotationSpeedFactor = GameManager.PlayerMovementSettings.AimingRotationSpeedFactor;
		}
		else if (turningOnADime)
		{
			rotationSpeedFactor = 1f;
		}
		else if (!turningOnADime && BMath.Abs(value) > 70f)
		{
			rotationSpeedFactor = 2.5f;
		}
		else
		{
			rotationSpeedFactor = BMath.LerpClamped(rotationSpeedFactor, 1f, 4f * Time.fixedDeltaTime);
		}
	}

	private void ApplyRotation()
	{
		if (CanRotate())
		{
			float num = GameManager.PlayerMovementSettings.MaxPlayerRotationSpeedDeg * rotationSpeedFactor;
			float to = BMath.Clamp((targetYaw - Yaw).WrapAngleDeg() / Time.fixedDeltaTime, 0f - num, num);
			YawSpeedDeg = BMath.LerpClamped(YawSpeedDeg, to, 5f * Time.fixedDeltaTime);
		}
		bool CanRotate()
		{
			if (!SingletonBehaviour<DrivingRangeManager>.HasInstance && CourseManager.MatchState < MatchState.TeeOff)
			{
				return false;
			}
			if (!PlayerInfo.AsGolfer.CanRotate())
			{
				return false;
			}
			if (PlayerInfo.AsSpectator.IsSpectating)
			{
				return false;
			}
			if (PlayerInfo.Inventory.GetEffectivelyEquippedItem() == ItemType.Landmine && PlayerInfo.Inventory.IsUsingItemAtAll)
			{
				return false;
			}
			return true;
		}
	}

	private void ApplyMovement()
	{
		UpdateMovementSpeed();
		if (CanMove())
		{
			Vector3 vector = worldMoveVector3d;
			if (isGrounded && vector.TryProjectOnPlaneAlongKeepMagnitude(Vector3.up, GroundData.normal, out var projection))
			{
				vector = projection;
			}
			float num = BMath.Max(0f, 1f - horizontalDrag * Time.fixedDeltaTime);
			Vector3 vector2;
			if (num <= 0.001f)
			{
				vector2 = Vector3.zero;
			}
			else
			{
				float num2 = speed / num;
				float num3 = (1f - num) * (num2 / Time.fixedDeltaTime);
				vector2 = vector * num3;
			}
			Velocity += consoleSpeedFactor * Time.fixedDeltaTime * vector2;
		}
	}

	private void UpdateMovementSpeed()
	{
		float num = (isGrounded ? GetTargetSpeedOnGround() : GetTargetSpeedInAir());
		num *= MatchSetupRules.GetValue(MatchSetupRules.Rule.PlayerSpeed);
		speed = BMath.LerpClamped(speed, num, movementAccelerationChangeSpeed * Time.fixedDeltaTime);
		float GetTargetSpeedInAir()
		{
			float num2 = (IsInSpringBootsJump ? GameManager.ItemSettings.SpringBootsJumpMovementSpeed : ((!wasWadingInWaterWhenLastGrounded) ? GameManager.PlayerMovementSettings.DefaultMoveSpeed : GameManager.PlayerMovementSettings.WadingInWaterSpeed));
			if (statusEffects.HasEffect(StatusEffect.SpeedBoost))
			{
				return num2 * GameManager.PlayerMovementSettings.SpeedBoostSpeedFactor;
			}
			return num2;
		}
		float GetTargetSpeedOnGround()
		{
			if (PlayerInfo.AsGolfer.IsChargingSwing)
			{
				return GameManager.PlayerMovementSettings.SwingChargingSpeed;
			}
			if (PlayerInfo.AsGolfer.IsAimingSwing)
			{
				return GameManager.PlayerMovementSettings.SwingAimingSpeed;
			}
			if (turningOnADime)
			{
				return GameManager.PlayerMovementSettings.DefaultMoveSpeed * 0.1f;
			}
			float num2 = (isWadingInWater ? GameManager.PlayerMovementSettings.WadingInWaterSpeed : GameManager.PlayerMovementSettings.DefaultMoveSpeed);
			if (statusEffects.HasEffect(StatusEffect.SpeedBoost))
			{
				return num2 * GameManager.PlayerMovementSettings.SpeedBoostSpeedFactor;
			}
			return num2;
		}
	}

	private void ApplyHorizontalDrag()
	{
		float num = BMath.Max(0f, 1f - horizontalDrag * Time.fixedDeltaTime);
		if (isGrounded && groundTime > 0.1f)
		{
			Vector3 vector = Velocity.ProjectOnPlane(GroundData.normal);
			Velocity = vector * num + Vector3.Dot(Velocity, GroundData.normal) * GroundData.normal;
		}
		else
		{
			Velocity = new Vector3(Velocity.x * num, Velocity.y, Velocity.z * num);
		}
	}

	private void ApplyVerticalDrag()
	{
		if (isDrowning)
		{
			float num = ((Velocity.y > 0f) ? 20f : 5f);
			float num2 = BMath.Max(0f, 1f - num * Time.fixedDeltaTime);
			Velocity = new Vector3(Velocity.x, Velocity.y * num2, Velocity.z);
		}
		else
		{
			float num3 = BMath.Max(0f, 1f - BMath.Abs(GetBaseGravitySpeedDelta()) / verticalTerminalVelocity);
			Velocity = new Vector3(Velocity.x, Velocity.y * num3, Velocity.z);
		}
	}

	private void ApplyRotationDrag()
	{
		float num = BMath.Max(0f, 1f - rotationDragFactor * Time.fixedDeltaTime);
		YawSpeedRad *= num;
	}

	private void UpdateKnockOutState()
	{
		if (IsKnockedOut && knockoutState != KnockoutState.Recovering)
		{
			bool flag = TryFindGroundWhileKnockedOut(out knockoutGroundRaycastHit);
			if (ShouldBeginRecovering())
			{
				RecoverFromKnockout();
			}
			else
			{
				SetKnockOutState(flag ? KnockoutState.OnGround : KnockoutState.InAir);
			}
		}
		bool ShouldBeginRecovering()
		{
			if (BMath.GetTimeSince(IsKnockedOutTimestamp) >= GameManager.PlayerMovementSettings.KnockoutTimeOutDuration)
			{
				return true;
			}
			float timeSince = BMath.GetTimeSince(knockoutTimestamp);
			float num = ((knockoutType == KnockoutType.ReturnedBall) ? GameManager.PlayerMovementSettings.KnockoutBallReturnedGroundDuration : GameManager.PlayerMovementSettings.KnockoutDefaultGroundDuration);
			if (timeSince < num)
			{
				float num2 = timeSince / num;
				int totalStarCount = BMath.CeilToInt(num);
				int coloredStarCount = BMath.CeilToInt((1f - num2) * num);
				NetworkknockedOutVfxData = new KnockedOutVfxData(totalStarCount, coloredStarCount);
				return false;
			}
			return true;
		}
		bool TryFindGroundWhileKnockedOut(out RaycastHit hit)
		{
			(Vector3 centerAlongAxis, Vector3 centerOppositeAxis) capsuleSphereCenters = BGeo.GetCapsuleSphereCenters(base.transform.TransformPoint(uprightCollider.center), uprightCollider.transform.up, uprightCollider.radius, uprightCollider.height);
			Vector3 item = capsuleSphereCenters.centerAlongAxis;
			Vector3 item2 = capsuleSphereCenters.centerOppositeAxis;
			int num = Physics.CapsuleCastNonAlloc(item, item2, uprightCollider.radius, Vector3.down, layerMask: GameManager.LayerSettings.PlayerGroundableMask, results: raycastHitCache, maxDistance: 0.2f, queryTriggerInteraction: QueryTriggerInteraction.Ignore);
			hit = new RaycastHit
			{
				distance = float.MaxValue
			};
			for (int i = 0; i < num; i++)
			{
				RaycastHit raycastHit = raycastHitCache[i];
				if (raycastHit.distance < hit.distance)
				{
					hit = raycastHit;
				}
			}
			bool flag2 = hit.distance < float.MaxValue;
			if (drawPlayerGroundingDebug)
			{
				float num2 = (flag2 ? hit.distance : 0.2f);
				Color color = (flag2 ? Color.red : Color.green);
				BDebug.DrawCapsuleCast(item, item2, uprightCollider.radius, Vector3.down * num2, color);
			}
			return hit.distance < float.MaxValue;
		}
	}

	private void UpdateDivingState()
	{
		if (divingState == DivingState.None)
		{
			return;
		}
		if (divingState == DivingState.GettingUp)
		{
			if (BMath.GetTimeSince(DivingStateTimestamp) >= GameManager.PlayerMovementSettings.DiveGetUpDuration)
			{
				SetDivingState(DivingState.None);
			}
			return;
		}
		RaycastHit hit;
		bool flag = TryFindGroundWhileDiving(out hit);
		if (ShouldStartGettingUp())
		{
			SetDivingState(DivingState.GettingUp);
		}
		else if (flag)
		{
			UpdateGroundData(hit, hit.point);
			SetDivingState(DivingState.OnGround);
		}
		else
		{
			SetDivingState(DivingState.Diving);
		}
		bool ShouldStartGettingUp()
		{
			if (PlayerInfo.AsGolfer.IsMatchResolved)
			{
				return true;
			}
			if (PlayerInfo.AsSpectator.IsSpectating)
			{
				return true;
			}
			if (divingState != DivingState.OnGround && BMath.GetTimeSince(diveStartTimestamp) > GameManager.PlayerMovementSettings.DiveTimeOut)
			{
				return true;
			}
			if (divingState != DivingState.OnGround)
			{
				return false;
			}
			if (BMath.GetTimeSince(DivingStateTimestamp) < GameManager.PlayerMovementSettings.DiveMinGroundTimeToGetUp)
			{
				return false;
			}
			if (!(RawMoveVectorMagnitude > 0.1f))
			{
				return PlayerInfo.WantsToPlayEmote;
			}
			return true;
		}
		bool TryFindGroundWhileDiving(out RaycastHit reference)
		{
			if (rigidbody.linearVelocity.y > 1f)
			{
				reference = default(RaycastHit);
				return false;
			}
			(Vector3 centerAlongAxis, Vector3 centerOppositeAxis) capsuleSphereCenters = BGeo.GetCapsuleSphereCenters(base.transform.TransformPoint(divingCollider.center), divingCollider.transform.forward, divingCollider.radius, divingCollider.height);
			Vector3 item = capsuleSphereCenters.centerAlongAxis;
			Vector3 item2 = capsuleSphereCenters.centerOppositeAxis;
			float num = 0.2f;
			if (rigidbody.linearVelocity.y < 0f)
			{
				num -= rigidbody.linearVelocity.y * Time.fixedDeltaTime;
			}
			float num2 = divingCollider.radius * 0.98f;
			int num3 = Physics.CapsuleCastNonAlloc(item, item2, num2, Vector3.down, maxDistance: num, layerMask: GameManager.LayerSettings.PlayerGroundableMask, results: raycastHitCache, queryTriggerInteraction: QueryTriggerInteraction.Ignore);
			reference = new RaycastHit
			{
				distance = float.MaxValue
			};
			for (int i = 0; i < num3; i++)
			{
				RaycastHit raycastHit = raycastHitCache[i];
				if (raycastHit.distance < reference.distance)
				{
					reference = raycastHit;
				}
			}
			bool flag2 = reference.distance < float.MaxValue;
			if (drawPlayerGroundingDebug)
			{
				float num4 = (flag2 ? reference.distance : num);
				Color color = (flag2 ? Color.red : Color.green);
				BDebug.DrawCapsuleCast(item, item2, num2, Vector3.down * num4, color);
			}
			return reference.distance < float.MaxValue;
		}
	}

	private void TriggerJumpInternal(bool applySquashAndStretch = true, bool faceJumpDirection = false)
	{
		JumpType jumpType = GetJumpType();
		bool num = PlayerInfo.Inventory.TryUseSpringBoots();
		Vector3 vector = Velocity;
		float num2 = (num ? GameManager.ItemSettings.SpringBootsJumpUpwardsSpeed : GameManager.PlayerMovementSettings.JumpUpwardsSpeed);
		if (vector.y <= 0f)
		{
			vector.y = num2;
		}
		else if (isGrounded)
		{
			float y = vector.ProjectOnPlane(GroundData.normal).y;
			vector.y = y + num2;
		}
		else
		{
			vector += num2 * Vector3.up;
		}
		if (num)
		{
			Vector3 vector2 = rawWorldMoveVector3d * GameManager.ItemSettings.SpringBootsJumpHorizontalSpeed;
			if (Vector3.Dot(vector, vector2) <= 0f)
			{
				vector = vector.ProjectOnPlane(rawWorldMoveVector3d);
			}
			vector += vector2;
			SetIsInSpringBootsJump(isInJump: true, fromLanding: false);
		}
		Velocity = vector;
		Unground();
		if (faceJumpDirection && RawMoveVectorMagnitude > 0.01f)
		{
			targetYaw = worldMoveVector3d.GetYawDeg();
		}
		PlayerInfo.CancelEmote(canHideEmoteMenu: false);
		PlayerInfo.AnimatorIo.BeginJump(jumpType);
		PlayerInfo.PlayerAudio.PlayJumpForAllClients();
		JumpType GetJumpType()
		{
			if (Velocity.AsHorizontal2().sqrMagnitude < 4f)
			{
				return JumpType.Stationary;
			}
			if (!isStrafing)
			{
				if (Vector3.Angle(Velocity.Horizontalized(), base.transform.forward) < 45f)
				{
					return JumpType.Forward;
				}
				return JumpType.Stationary;
			}
			float yawDeg = base.transform.InverseTransformVector(Velocity).GetYawDeg();
			if (yawDeg >= 135f)
			{
				return JumpType.Back;
			}
			if (yawDeg >= 45f)
			{
				return JumpType.Right;
			}
			if (yawDeg >= -45f)
			{
				return JumpType.Forward;
			}
			if (yawDeg >= -135f)
			{
				return JumpType.Left;
			}
			return JumpType.Back;
		}
	}

	private void SetIsInSpringBootsJump(bool isInJump, bool fromLanding)
	{
		bool isInSpringBootsJump = IsInSpringBootsJump;
		IsInSpringBootsJump = isInJump;
		if (IsInSpringBootsJump != isInSpringBootsJump && !IsInSpringBootsJump)
		{
			PlayerInfo.Inventory.InformNoLongerInSpringBootsJump(fromLanding);
		}
	}

	private void UpdateGroundingState(bool suppressLandingAnimation)
	{
		wasGrounded = isGrounded;
		NetworkisGrounded = PerformGroundCheck();
		if (isGrounded && PhysicsManager.JumpPadsByCollider.TryGetValue(GroundData.collider, out var value) && value.TryTriggerJumpFor(PlayerInfo.AsHittable))
		{
			PlayerInfo.AnimatorIo.BeginJump(JumpType.Stationary);
			return;
		}
		anchorVelocity = ((isGrounded && GroundData.hasRigidbody) ? GroundData.rigidbody.linearVelocity : Vector3.zero);
		if (isGrounded && MoveVectorMagnitude > 0.1f)
		{
			PlayerInfo.CancelEmote(canHideEmoteMenu: false);
		}
		if (isGrounded == wasGrounded)
		{
			return;
		}
		groundTime = 0f;
		if (isGrounded)
		{
			if (!suppressLandingAnimation)
			{
				PlayerInfo.AnimatorIo.SetLanded();
			}
			if (IsInSpringBootsJump)
			{
				SetIsInSpringBootsJump(isInJump: false, fromLanding: true);
				PlaySpringBootsLandingForAllClients(base.transform.position);
			}
			CmdInformGrounded();
		}
		else
		{
			wasWadingInWaterWhenLastGrounded = isWadingInWater;
			lastUngroundedDueToJumpPad = false;
		}
		UpdateIsGroundedAtAll();
		PlayerInfo.Inventory.InformLocalPlayerGroundedChanged();
		PlayerInfo.AnimatorIo.SetIsGrounded(isGrounded);
		this.IsGroundedChanged?.Invoke();
		void PlaySpringBootsLandingForAllClients(Vector3 worldPosition)
		{
			PlaySpringBootsLandingInternal(worldPosition);
			CmdPlaySpringBootsLandingForAllClients(worldPosition);
		}
	}

	private void UpdateWaterState()
	{
		NetworkisWadingInWater = IsWading();
		bool IsWading()
		{
			if (!isGrounded)
			{
				return false;
			}
			if (PlayerInfo.ActiveGolfCartSeat.IsValid())
			{
				return false;
			}
			if (PlayerInfo.LevelBoundsTracker.CurrentSecondaryHazardLocalOnly == null)
			{
				if (MainOutOfBoundsHazard.Type != OutOfBoundsHazard.Water)
				{
					return false;
				}
			}
			else if (PlayerInfo.LevelBoundsTracker.CurrentSecondaryHazardLocalOnly.Type != OutOfBoundsHazard.Water)
			{
				return false;
			}
			float currentOutOfBoundsHazardWorldHeightLocalOnly = PlayerInfo.LevelBoundsTracker.CurrentOutOfBoundsHazardWorldHeightLocalOnly;
			if (Position.y + GameManager.PlayerMovementSettings.ShallowWaterWadingHeightThreshold > currentOutOfBoundsHazardWorldHeightLocalOnly)
			{
				return false;
			}
			return true;
		}
	}

	[Command]
	private void CmdInformGrounded()
	{
		if (base.isServer && base.isClient)
		{
			UserCode_CmdInformGrounded();
			return;
		}
		NetworkWriterPooled writer = NetworkWriterPool.Get();
		SendCommandInternal("System.Void PlayerMovement::CmdInformGrounded()", 2096744024, writer, 0);
		NetworkWriterPool.Return(writer);
	}

	[Command]
	private void CmdPlaySpringBootsLandingForAllClients(Vector3 worldPosition, NetworkConnectionToClient sender = null)
	{
		if (base.isServer && base.isClient)
		{
			UserCode_CmdPlaySpringBootsLandingForAllClients__Vector3__NetworkConnectionToClient(worldPosition, sender);
			return;
		}
		NetworkWriterPooled writer = NetworkWriterPool.Get();
		writer.WriteVector3(worldPosition);
		SendCommandInternal("System.Void PlayerMovement::CmdPlaySpringBootsLandingForAllClients(UnityEngine.Vector3,Mirror.NetworkConnectionToClient)", 236943702, writer, 0);
		NetworkWriterPool.Return(writer);
	}

	[TargetRpc]
	private void RpcPlaySpringBootsLanding(NetworkConnectionToClient connection, Vector3 worldPosition)
	{
		NetworkWriterPooled writer = NetworkWriterPool.Get();
		writer.WriteVector3(worldPosition);
		SendTargetRPCInternal(connection, "System.Void PlayerMovement::RpcPlaySpringBootsLanding(Mirror.NetworkConnectionToClient,UnityEngine.Vector3)", 647086363, writer, 0);
		NetworkWriterPool.Return(writer);
	}

	private void PlaySpringBootsLandingInternal(Vector3 worldPosition)
	{
		VfxManager.PlayPooledVfxLocalOnly(VfxType.SpringBootsLanding, worldPosition, base.transform.rotation);
		PlayerInfo.Inventory.InformOfSpringBootsLanding(worldPosition);
	}

	private void UpdatePhysicsParameters()
	{
		horizontalDrag = GameManager.PlayerMovementSettings.DefaultHorizontalDrag;
		gravityFactor = (IsKnockedOutOrRecovering ? GameManager.PlayerMovementSettings.KnockOutGravityFactor : 1f);
		if (isDrowning)
		{
			horizontalDrag = GameManager.PlayerMovementSettings.DrowningHorizontalDrag;
		}
		else if (isGrounded)
		{
			if (groundTerrainType != GroundTerrainType.NotTerrain && TerrainManager.Settings.LayerSettings.TryGetValue(groundTerrainDominantGlobalLayer, out var value) && value.DoesOverridePlayerDrag)
			{
				horizontalDrag = value.PlayerOverrideDrag;
			}
			else
			{
				horizontalDrag = GameManager.PlayerMovementSettings.GroundedHorizontalDrag;
			}
		}
		else
		{
			horizontalDrag = GameManager.PlayerMovementSettings.AirHorizontalDrag;
		}
		UpdateRotationDragFactor();
		UpdateRigidbodyDamping();
		void UpdateRigidbodyDamping()
		{
			if (IsKnockedOut)
			{
				if (knockoutState == KnockoutState.OnGround)
				{
					rigidbody.linearDamping = GameManager.PlayerMovementSettings.KnockOutGroundLinearDamping;
					rigidbody.angularDamping = GameManager.PlayerMovementSettings.KnockOutGroundAngularDamping;
				}
				else if (knockoutState == KnockoutState.InAir)
				{
					rigidbody.linearDamping = GameManager.PlayerMovementSettings.KnockOutAirLinearDamping;
					rigidbody.angularDamping = GameManager.PlayerMovementSettings.KnockOutAirAngularDamping;
				}
			}
			else if (divingState != DivingState.None)
			{
				if (divingState == DivingState.Diving)
				{
					rigidbody.linearDamping = GameManager.PlayerMovementSettings.DivingAirLinearDamping;
					rigidbody.angularDamping = GameManager.PlayerMovementSettings.DivingAirAngularDamping;
				}
				else if (divingState == DivingState.OnGround)
				{
					if (groundTerrainType != GroundTerrainType.NotTerrain && TerrainManager.Settings.LayerSettings.TryGetValue(groundTerrainDominantGlobalLayer, out var value2) && value2.DoesOverridePlayerDiveDamping)
					{
						rigidbody.linearDamping = value2.PlayerOverrideDiveDamping;
					}
					else
					{
						rigidbody.linearDamping = GameManager.PlayerMovementSettings.DivingGroundLinearDamping;
					}
					rigidbody.angularDamping = GameManager.PlayerMovementSettings.DivingGroundAngularDamping;
				}
			}
			else
			{
				rigidbody.linearDamping = 0f;
				rigidbody.angularDamping = 0f;
			}
		}
	}

	private void UpdateRotationDragFactor()
	{
		float value = BMath.Abs((targetYaw - Yaw).WrapAngleDeg());
		rotationDragFactor = BMath.RemapClamped(5f, 30f, 20f, 1f, value);
	}

	private void UpdateSpecialStatePhysics()
	{
		if (PlayerInfo.AsHittable.IsFrozen)
		{
			rigidbody.constraints = RigidbodyConstraints.None;
			rigidbody.useGravity = true;
		}
		else if (IsKnockedOut)
		{
			rigidbody.constraints = RigidbodyConstraints.None;
		}
		else if (knockoutState == KnockoutState.Recovering)
		{
			rigidbody.constraints = (RigidbodyConstraints)80;
		}
		else
		{
			rigidbody.constraints = (RigidbodyConstraints)80;
			rigidbody.useGravity = false;
		}
		if (!IsKnockedOut && divingState == DivingState.None)
		{
			uprightCollider.sharedMaterial = PhysicsManager.Settings.PlayerMaterial;
			divingCollider.sharedMaterial = PhysicsManager.Settings.PlayerMaterial;
		}
		else if (IsKnockedOut)
		{
			uprightCollider.sharedMaterial = PhysicsManager.Settings.PlayerKnockedOutMaterial;
			divingCollider.sharedMaterial = PhysicsManager.Settings.PlayerKnockedOutMaterial;
		}
		else if (divingState != DivingState.None)
		{
			uprightCollider.sharedMaterial = PhysicsManager.Settings.PlayerDivingMaterial;
			divingCollider.sharedMaterial = PhysicsManager.Settings.PlayerDivingMaterial;
		}
	}

	private bool PerformGroundCheck()
	{
		if (!CanGround())
		{
			return false;
		}
		float additionalGroundCheckDistance = ((wasGrounded && Velocity.y <= 1f) ? GameManager.PlayerMovementSettings.GroundCheckDistanceAdditionWhenGrounded : 0f);
		if (!TryFindGround(additionalGroundCheckDistance, out var groundingRayHitInfo, out var contactPoint))
		{
			return false;
		}
		UpdateGroundData(groundingRayHitInfo, contactPoint);
		return true;
	}

	private bool TryFindGround(float additionalGroundCheckDistance, out RaycastHit groundingRayHitInfo, out Vector3 contactPoint)
	{
		Ray ray = new Ray(uprightCollider.transform.TransformPoint(uprightCollider.center), Vector3.down);
		float num = uprightCollider.center.y + 0.2f;
		if (additionalGroundCheckDistance > 0f)
		{
			num += additionalGroundCheckDistance;
			if (isGrounded)
			{
				num = BMath.Max(num, ray.origin.y - GroundData.point.y);
			}
		}
		bool flag = false;
		float num2 = float.PositiveInfinity;
		groundingRayHitInfo = default(RaycastHit);
		contactPoint = Vector3.zero;
		Box box = new Box(ray.origin, 0.01f);
		if (!box.Check(GameManager.LayerSettings.PlayerGroundableMask))
		{
			int hitCount = Physics.RaycastNonAlloc(ray, maxDistance: num, layerMask: GameManager.LayerSettings.PlayerGroundableMask, results: raycastHitCache, queryTriggerInteraction: QueryTriggerInteraction.Ignore);
			flag = TryGetBestCachedHit(hitCount, out groundingRayHitInfo);
		}
		bool flag2;
		if (!flag)
		{
			flag2 = false;
		}
		else
		{
			Vector3 normal = groundingRayHitInfo.normal;
			flag2 = 90f + normal.GetPitchDeg() <= GameManager.PlayerMovementSettings.UngroundableGroundPitchThreshold;
			if (flag2)
			{
				num2 = groundingRayHitInfo.distance;
				contactPoint = groundingRayHitInfo.point;
			}
		}
		if (drawPlayerGroundingDebug)
		{
			if (flag2)
			{
				BDebug.DrawLine(ray.origin, contactPoint, Color.red);
			}
			else
			{
				BDebug.DrawLine(ray.origin, ray.origin + ray.direction * num, flag ? Color.green : Color.yellow);
			}
		}
		if (!flag2)
		{
			groundingRayHitInfo.distance = num;
			Quaternion quaternion = Quaternion.AngleAxis(base.transform.forward.GetYawDeg(), Vector3.up);
			for (int i = 0; i < GameManager.PlayerMovementSettings.AdditionalGroundingRaycastCount; i++)
			{
				float angle = MathF.PI * 2f * (float)i * GameManager.PlayerMovementSettings.InverseAdditionalGroundingRaycastCount;
				ray.origin = uprightCollider.bounds.center + quaternion * new Vector3(BMath.Sin(angle), 0f, BMath.Cos(angle)) * GameManager.PlayerMovementSettings.AdditionalGroundingRaycastsRadius;
				box.center = ray.origin;
				if (box.Check(GameManager.LayerSettings.PlayerGroundableMask))
				{
					continue;
				}
				int hitCount2 = Physics.RaycastNonAlloc(ray, maxDistance: num, layerMask: GameManager.LayerSettings.PlayerGroundableMask, results: raycastHitCache, queryTriggerInteraction: QueryTriggerInteraction.Ignore);
				RaycastHit bestHit;
				bool num3 = TryGetBestCachedHit(hitCount2, out bestHit);
				if (drawPlayerGroundingDebug)
				{
					BDebug.DrawLine(ray.origin, ray.origin + ray.direction * num, Color.green);
				}
				if (!num3)
				{
					continue;
				}
				contactPoint = bestHit.point;
				if (!CorrectGroundRayHit(ref bestHit))
				{
					continue;
				}
				Vector3 normal2 = bestHit.normal;
				if (90f + normal2.GetPitchDeg() <= GameManager.PlayerMovementSettings.UngroundableGroundPitchThreshold && !(bestHit.distance >= num2))
				{
					groundingRayHitInfo = bestHit;
					num2 = bestHit.distance;
					flag2 = true;
					if (drawPlayerGroundingDebug)
					{
						BDebug.DrawLine(ray.origin, ray.origin + ray.direction * num, Color.red);
					}
				}
			}
		}
		return flag2;
		bool TryGetBestCachedHit(int num4, out RaycastHit reference)
		{
			bool result = false;
			reference = new RaycastHit
			{
				distance = float.PositiveInfinity
			};
			for (int j = 0; j < num4; j++)
			{
				RaycastHit raycastHit = raycastHitCache[j];
				if (!(raycastHit.distance >= reference.distance))
				{
					reference = raycastHit;
					result = true;
				}
			}
			return result;
		}
	}

	private void UpdateGroundData(RaycastHit groundingRayHitInfo, Vector3 contactPoint)
	{
		TerrainAddition foundComponent;
		if (groundingRayHitInfo.collider is TerrainCollider)
		{
			NetworkgroundTerrainType = GroundTerrainType.Terrain;
			NetworkgroundTerrainDominantGlobalLayer = TerrainManager.GetDominantGlobalLayerAtPoint(contactPoint);
		}
		else if (groundingRayHitInfo.collider.TryGetComponentInParent<TerrainAddition>(out foundComponent, includeInactive: true))
		{
			NetworkgroundTerrainType = GroundTerrainType.TerrainAddition;
			NetworkgroundTerrainDominantGlobalLayer = foundComponent.TerrainLayer;
		}
		else
		{
			NetworkgroundTerrainType = GroundTerrainType.NotTerrain;
			NetworkgroundTerrainDominantGlobalLayer = (TerrainLayer)(-1);
		}
		PlayerGroundData playerGroundData = (GroundData = new PlayerGroundData
		{
			point = groundingRayHitInfo.point,
			contactPoint = contactPoint,
			normal = groundingRayHitInfo.normal,
			collider = groundingRayHitInfo.collider,
			hasRigidbody = (groundingRayHitInfo.rigidbody != null),
			rigidbody = groundingRayHitInfo.rigidbody
		});
		if (!playerGroundData.hasRigidbody)
		{
			anchorVelocity = Vector3.zero;
		}
		else
		{
			playerGroundData.rigidbody.GetPointVelocity(playerGroundData.point);
		}
	}

	private bool CorrectGroundRayHit(ref RaycastHit groundHit)
	{
		Collider collider = groundHit.collider;
		Vector3 farIntersection;
		if (collider is SphereCollider { bounds: { center: var center } } sphereCollider)
		{
			if (BGeo.RaySphereIntersection(Position, Vector3.down, center, sphereCollider.radius, out var closeIntersection, out farIntersection) == 0)
			{
				return false;
			}
			groundHit.point = closeIntersection;
			groundHit.normal = (closeIntersection - center).normalized;
			return true;
		}
		if (collider is CapsuleCollider capsuleCollider)
		{
			if (BGeo.RaySphereIntersection(Position, Vector3.down, capsuleCollider.bounds.center, capsuleCollider.radius, out var closeIntersection2, out farIntersection) == 0)
			{
				return false;
			}
			groundHit.point = closeIntersection2;
			Vector3 center2 = capsuleCollider.bounds.center;
			Vector3 zero = Vector3.zero;
			zero[capsuleCollider.direction] = 1f;
			Vector3 vector = (0.5f * capsuleCollider.height - capsuleCollider.radius) * zero;
			Vector3 capsuleCenter = center2 - vector;
			Vector3 capsuleCenter2 = center2 + vector;
			groundHit.normal = BGeo.GetCapsuleNormalTowards(closeIntersection2, capsuleCenter, capsuleCenter2, capsuleCollider.radius);
			return true;
		}
		Vector3 intersection;
		bool result = BGeo.LinePlaneIntersection(Position, Vector3.down, groundHit.point, groundHit.normal, out intersection);
		groundHit.point = intersection;
		return result;
	}

	private void ApplyGravity()
	{
		if (CanApplyGravity())
		{
			float num = 1f;
			if (IsInSpringBootsJump)
			{
				float value = BMath.Abs(Velocity.y);
				num = BMath.RemapClamped(GameManager.ItemSettings.SpringBootsJumpPeakLingerStartSpeed, GameManager.ItemSettings.SpringBootsJumpPeakLingerEndSpeed, 1f, GameManager.ItemSettings.SpringBootsJumpPeakLingerGravityFactor, value);
			}
			float y = Velocity.y + num * GetBaseGravitySpeedDelta();
			Velocity = new Vector3(Velocity.x, y, Velocity.z);
		}
		bool CanApplyGravity()
		{
			return !isDrowning;
		}
	}

	private float GetBaseGravitySpeedDelta()
	{
		return Physics.gravity.y * GameManager.PlayerMovementSettings.BaseGravityFactor * gravityFactor * Time.fixedDeltaTime;
	}

	private void UpdateTerminalVelocity()
	{
		verticalTerminalVelocity = GameManager.PlayerMovementSettings.DefaultTerminalFallingSpeed;
	}

	private Vector3 GetNextFrameGroundPoint(float velocityFactor = 1f)
	{
		Vector3 vector = Position + Time.fixedDeltaTime * velocityFactor * Velocity;
		if (vector.TryProjectPointOnPlaneAlong(Vector3.up, GroundData.point, GroundData.normal, out var projection))
		{
			return projection;
		}
		return vector;
	}

	private void SnapToGround()
	{
		float num = (GetNextFrameGroundPoint().y - Position.y) / Time.fixedDeltaTime;
		Velocity = new Vector3(Velocity.x, num, Velocity.z);
		if (drawPlayerGroundingDebug)
		{
			BDebug.DrawWireArrow(Position, GetNextFrameGroundPoint(), Color.yellow);
			BDebug.DrawWireArrow(Position, Position + num * Vector3.up, Color.red);
		}
	}

	public void Unground(bool fromJumpPad = false)
	{
		bool flag = isGrounded;
		NetworkisGrounded = false;
		if (fromJumpPad)
		{
			lastUngroundedDueToJumpPad = true;
		}
		if (isGrounded != flag)
		{
			groundTime = 0f;
			wasWadingInWaterWhenLastGrounded = isWadingInWater;
			if (!fromJumpPad)
			{
				lastUngroundedDueToJumpPad = false;
			}
			UpdateIsGroundedAtAll();
			UpdateWaterState();
			PlayerInfo.Inventory.InformLocalPlayerGroundedChanged();
			PlayerInfo.AnimatorIo.SetIsGrounded(isGrounded);
			this.IsGroundedChanged?.Invoke();
		}
	}

	private void UpdateIsGroundedAtAll()
	{
		bool flag = isGroundedAtAll;
		isGroundedAtAll = isGrounded || divingState == DivingState.OnGround || knockoutState == KnockoutState.OnGround;
		bool flag2 = !isGroundedAtAllInitialized && isGroundedAtAll;
		if (isGroundedAtAll)
		{
			isGroundedAtAllInitialized = true;
		}
		if (isGroundedAtAll != flag)
		{
			if (!isGroundedAtAll)
			{
				ungroundedLastGroundedAtAllPosition = base.transform.position;
			}
			else if (!flag2 && !lastUngroundedDueToJumpPad && !SingletonBehaviour<DrivingRangeManager>.HasInstance && CourseManager.CountActivePlayers() > 1 && (base.transform.position - ungroundedLastGroundedAtAllPosition).sqrMagnitude >= GameManager.Achievements.FrogLegsDistanceSquared)
			{
				GameManager.AchievementsManager.Unlock(AchievementId.FrogLegs);
			}
		}
	}

	[TargetRpc]
	private void RpcSetOutOfBoundsMessageRemainingTime(float remainingTime)
	{
		NetworkWriterPooled writer = NetworkWriterPool.Get();
		writer.WriteFloat(remainingTime);
		SendTargetRPCInternal(null, "System.Void PlayerMovement::RpcSetOutOfBoundsMessageRemainingTime(System.Single)", -359689967, writer, 0);
		NetworkWriterPool.Return(writer);
	}

	[TargetRpc]
	private void RpcReturnToBounds(NetworkConnectionToClient connection)
	{
		NetworkWriterPooled writer = NetworkWriterPool.Get();
		SendTargetRPCInternal(connection, "System.Void PlayerMovement::RpcReturnToBounds(Mirror.NetworkConnectionToClient)", -581649243, writer, 0);
		NetworkWriterPool.Return(writer);
	}

	private void ReturnToBoundsInternal()
	{
		Vector3 directionIntoLevel;
		Vector3 nearestPointOnReturnSplines = BoundsManager.GetNearestPointOnReturnSplines(base.transform.position, out directionIntoLevel);
		nearestPointOnReturnSplines.y = TerrainManager.GetWorldHeightAtPoint(nearestPointOnReturnSplines);
		Quaternion rotation = Quaternion.LookRotation(directionIntoLevel);
		Teleport(nearestPointOnReturnSplines, rotation, resetState: true);
		if (!base.isServer)
		{
			OutOfBoundsMessage.ForceHideTemporarily((float)NetworkTime.rtt * 3f);
		}
	}

	public void Teleport(Vector3 position, Quaternion rotation, bool resetState)
	{
		if (resetState)
		{
			SetKnockOutState(KnockoutState.None);
			SetDivingState(DivingState.None);
		}
		base.transform.SetPositionAndRotation(position, rotation);
		rigidbody.position = position;
		rigidbody.rotation = rotation;
		if (!rigidbody.isKinematic)
		{
			rigidbody.linearVelocity = Vector3.zero;
			rigidbody.angularVelocity = Vector3.zero;
		}
		PlayerInfo.NetworkRigidbody.CmdTeleport(position, rotation);
		isGroundedAtAllInitialized = false;
		GameplayCameraManager.ReachOrbitCameraSteadyState();
		CmdInformTeleported(position, rotation);
		this.Teleported?.Invoke();
	}

	[Command]
	private void CmdInformTeleported(Vector3 position, Quaternion rotation, NetworkConnectionToClient sender = null)
	{
		if (base.isServer && base.isClient)
		{
			UserCode_CmdInformTeleported__Vector3__Quaternion__NetworkConnectionToClient(position, rotation, sender);
			return;
		}
		NetworkWriterPooled writer = NetworkWriterPool.Get();
		writer.WriteVector3(position);
		writer.WriteQuaternion(rotation);
		SendCommandInternal("System.Void PlayerMovement::CmdInformTeleported(UnityEngine.Vector3,UnityEngine.Quaternion,Mirror.NetworkConnectionToClient)", 1108124897, writer, 0);
		NetworkWriterPool.Return(writer);
	}

	[TargetRpc]
	private void RpcInformedTeleported(NetworkConnectionToClient connection, Vector3 position, Quaternion rotation)
	{
		NetworkWriterPooled writer = NetworkWriterPool.Get();
		writer.WriteVector3(position);
		writer.WriteQuaternion(rotation);
		SendTargetRPCInternal(connection, "System.Void PlayerMovement::RpcInformedTeleported(Mirror.NetworkConnectionToClient,UnityEngine.Vector3,UnityEngine.Quaternion)", -1628742867, writer, 0);
		NetworkWriterPool.Return(writer);
	}

	private void OnRemotePlayerTeleported(Vector3 position, Quaternion rotation)
	{
		rigidbody.position = position;
		rigidbody.rotation = rotation;
		if (!rigidbody.isKinematic)
		{
			rigidbody.linearVelocity = Vector3.zero;
			rigidbody.angularVelocity = Vector3.zero;
		}
		this.Teleported?.Invoke();
	}

	private void RecoverFromKnockout()
	{
		if (!IsKnockedOutOrRecovering)
		{
			return;
		}
		if (ShouldRecoverInstantly())
		{
			SetKnockOutState(KnockoutState.None);
		}
		else if (knockoutState != KnockoutState.Recovering)
		{
			if (knockoutRecoveryRoutine != null)
			{
				StopCoroutine(knockoutRecoveryRoutine);
			}
			knockoutRecoveryRoutine = StartCoroutine(KnockoutRecoveryRoutine());
		}
		IEnumerator KnockoutRecoveryRoutine()
		{
			float num = Vector3.SignedAngle(base.transform.forward, Vector3.up, base.transform.up);
			var (knockoutRecoveryType, vector) = ((num <= 45f) ? ((num <= -135f) ? (KnockoutRecoveryType.FromFront, base.transform.up.Horizontalized()) : ((!(num <= -45f)) ? (KnockoutRecoveryType.FromBack, -base.transform.up.Horizontalized()) : (KnockoutRecoveryType.FromRight, base.transform.forward.Horizontalized()))) : ((!(num <= 135f)) ? (KnockoutRecoveryType.FromFront, base.transform.up.Horizontalized()) : (KnockoutRecoveryType.FromLeft, base.transform.forward.Horizontalized())));
			Vector3 position;
			if (Physics.Raycast(PlayerInfo.HipBone.position, Vector3.down, out var hitInfo, uprightCollider.radius * 1.5f, GameManager.LayerSettings.PlayerGroundableMask, QueryTriggerInteraction.Ignore))
			{
				position = hitInfo.point;
			}
			else
			{
				position = PlayerInfo.HipBone.position;
				position.y = base.transform.position.y - uprightCollider.radius;
			}
			Quaternion rotation = Quaternion.LookRotation(vector.Horizontalized());
			SetKnockOutState(KnockoutState.Recovering);
			base.transform.SetPositionAndRotation(position, rotation);
			rigidbody.position = position;
			rigidbody.rotation = rotation;
			rigidbody.linearVelocity = Vector3.zero;
			rigidbody.angularVelocity = Vector3.zero;
			PlayerInfo.AnimatorIo.SetKnockoutRecoveryType(knockoutRecoveryType);
			yield return new WaitForSeconds(GameManager.PlayerMovementSettings.KnockoutRecoveryDuration);
			SetKnockOutState(KnockoutState.None);
		}
		bool ShouldRecoverInstantly()
		{
			if (knockoutState != KnockoutState.OnGround && knockoutState != KnockoutState.Recovering)
			{
				return true;
			}
			if (BMath.Abs(base.transform.up.GetPitchDeg()) >= 45f)
			{
				return true;
			}
			return false;
		}
	}

	private void SetKnockOutState(KnockoutState state)
	{
		if (!isVisible && state != KnockoutState.None)
		{
			return;
		}
		KnockoutState knockoutState = this.knockoutState;
		bool isKnockedOutOrRecovering = IsKnockedOutOrRecovering;
		NetworkknockoutState = state;
		if (this.knockoutState == knockoutState)
		{
			return;
		}
		if (this.knockoutState == KnockoutState.None && knockoutRecoveryRoutine != null)
		{
			StopCoroutine(knockoutRecoveryRoutine);
		}
		if (IsKnockedOutOrRecovering != isKnockedOutOrRecovering)
		{
			IsKnockedOutTimestamp = Time.timeAsDouble;
			if (IsKnockedOutOrRecovering)
			{
				knockoutTimestamp = Time.timeAsDouble;
				Unground();
				SetDivingState(DivingState.None);
				PlayerInfo.CancelEmote(canHideEmoteMenu: true);
				PlayerInfo.Inventory.CancelItemFlourish();
				PlayerInfo.ExitGolfCart(GolfCartExitType.Knockout);
			}
			else
			{
				PlayerInfo.AnimatorIo.RecoverFromKnockOut();
				NetworkknockedOutVfxData = KnockedOutVfxData.None;
				UpdateGroundingState(suppressLandingAnimation: true);
			}
		}
		if (knockoutState != KnockoutState.Recovering && state == KnockoutState.Recovering)
		{
			NetworkknockedOutVfxData = KnockedOutVfxData.None;
		}
		if (this.knockoutState == KnockoutState.Recovering || (this.knockoutState == KnockoutState.None && knockoutState != KnockoutState.Recovering))
		{
			StartKnockoutImmunity(fromPlayerAggression: true);
		}
		UpdateSpecialStatePhysics();
		UpdateIsGroundedAtAll();
		PlayerInfo.AnimatorIo.SetKnockoutState(this.knockoutState);
		this.IsKnockedOutOrRecoveringChanged?.Invoke();
		PlayerInfo.Cosmetics.Switcher.SetKnockedOut(IsKnockedOut);
	}

	private void StartKnockoutImmunity(bool fromPlayerAggression)
	{
		if (knockoutImmunityRoutine != null)
		{
			StopCoroutine(knockoutImmunityRoutine);
		}
		knockoutImmunityRoutine = StartCoroutine(KnockoutImmunityRoutine());
		bool IsStillInPlayerAgressionImmunity(float duration)
		{
			if (knockoutState == KnockoutState.Recovering)
			{
				return true;
			}
			if (!IsKnockedOutOrRecovering && BMath.GetTimeSince(IsKnockedOutTimestamp) < duration)
			{
				return true;
			}
			if (!PlayerInfo.AsHittable.IsFrozen && BMath.GetTimeSince(PlayerInfo.AsHittable.IsFrozenChangeTimestamp) < duration)
			{
				return true;
			}
			if ((float)recentHitsByDiveCount >= GameManager.PlayerMovementSettings.RepeatedDiveHitCountForKnockoutImmunity && BMath.GetTimeSince(lastHitByDaveTimestamp) < duration)
			{
				return true;
			}
			return false;
		}
		IEnumerator KnockoutImmunityRoutine()
		{
			if (fromPlayerAggression)
			{
				if (BMath.GetTimeSince(longKnockoutImmunityIncrementTimestamp) > GameManager.PlayerMovementSettings.KnockoutImmunityLongDurationCooldown)
				{
					recentKnockoutImmunityCount = 0;
				}
				recentKnockoutImmunityCount++;
				longKnockoutImmunityIncrementTimestamp = Time.timeAsDouble;
				bool flag = recentKnockoutImmunityCount < GameManager.PlayerMovementSettings.MinKnockoutImmunityCountBeforeLongDuration;
				float duration = (flag ? GameManager.PlayerMovementSettings.PostKnockoutImmunityDuration : GameManager.PlayerMovementSettings.PostKnockoutImmunityLongDuration);
				NetworkknockoutImmunityStatus = KnockOutImmunity.Get((!flag) ? KnockOutVfxColor.Orange : KnockOutVfxColor.Blue);
				while (IsStillInPlayerAgressionImmunity(duration))
				{
					yield return null;
				}
			}
			else
			{
				NetworkknockoutImmunityStatus = KnockOutImmunity.Get(KnockOutVfxColor.Blue);
				double timestamp = Time.timeAsDouble;
				while (BMath.GetTimeSince(timestamp) < GameManager.PlayerMovementSettings.PostKnockoutImmunityDuration)
				{
					yield return null;
				}
			}
			NetworkknockoutImmunityStatus = KnockOutImmunity.Reset(knockoutImmunityStatus);
			recentHitsByDiveCount = 0;
		}
	}

	private void CancelKnockoutImmunity()
	{
		if (knockoutImmunityStatus.hasImmunity)
		{
			if (knockoutImmunityRoutine != null)
			{
				StopCoroutine(knockoutImmunityRoutine);
			}
			NetworkknockoutImmunityStatus = KnockOutImmunity.Reset(knockoutImmunityStatus);
			recentHitsByDiveCount = 0;
		}
	}

	private KnockoutState GetEffectiveAnimationKnockoutState()
	{
		if (knockoutState == KnockoutState.None)
		{
			return KnockoutState.None;
		}
		if (knockoutState != KnockoutState.OnGround || knockoutGroundRaycastHit.colliderInstanceID == 0)
		{
			return KnockoutState.InAir;
		}
		if (!(BMath.Abs(Vector3.Angle(knockoutGroundRaycastHit.normal, base.transform.up) - 90f) > 45f))
		{
			return KnockoutState.OnGround;
		}
		return KnockoutState.InAir;
	}

	private void SetDivingState(DivingState state)
	{
		if (!isVisible && state != DivingState.None)
		{
			return;
		}
		DivingState divingState = this.divingState;
		NetworkdivingState = state;
		if (this.divingState != divingState)
		{
			bool flag = divingState != DivingState.None;
			bool flag2 = this.divingState != DivingState.None;
			DivingStateTimestamp = Time.timeAsDouble;
			UpdateSpecialStatePhysics();
			if (this.divingState == DivingState.Diving)
			{
				diveHitHittables.Clear();
			}
			if (!flag && flag2)
			{
				PlayerInfo.CancelEmote(canHideEmoteMenu: false);
				PlayerInfo.Inventory.CancelItemFlourish();
				PlayerInfo.AsGolfer.InformLocalPlayerStartedDiving();
			}
			else if (flag && !flag2)
			{
				UpdateGroundingState(suppressLandingAnimation: true);
			}
			if (diveType == DiveType.RocketDriverSwingMiss && this.divingState == DivingState.OnGround)
			{
				NetworkdiveType = DiveType.Regular;
			}
			UpdateIsGroundedAtAll();
			PlayerInfo.Inventory.InformLocalPlayerDivingStateChanged();
			PlayerInfo.AnimatorIo.SetDivingState(state);
		}
	}

	private void SetIsDrowning(bool isDrowning)
	{
		if (isDrowning != this.isDrowning)
		{
			this.isDrowning = isDrowning;
			PlayerInfo.AnimatorIo.SetIsDrowning(isDrowning);
			if (isDrowning && !PlayerInfo.LevelBoundsTracker.IsInWaterLocalOnly(out var _, 0.5f))
			{
				Teleport(PlayerInfo.AsGolfer.LocalPlayerLatestEliminationPosition, base.transform.rotation, resetState: true);
			}
			LocalPlayerUpdateVisibility();
		}
	}

	[TargetRpc]
	public void RpcInformKnockedOutOtherPlayer(PlayerInfo knockedOutPlayer, bool addSpeedBoost)
	{
		NetworkWriterPooled writer = NetworkWriterPool.Get();
		writer.WriteNetworkBehaviour(knockedOutPlayer);
		writer.WriteBool(addSpeedBoost);
		SendTargetRPCInternal(null, "System.Void PlayerMovement::RpcInformKnockedOutOtherPlayer(PlayerInfo,System.Boolean)", -768549841, writer, 0);
		NetworkWriterPool.Return(writer);
	}

	public void InformDrankCoffee()
	{
		AddSpeedBoost(GameManager.ItemSettings.CoffeeEffectDuration);
	}

	public void InformOfPerfectShotAtTeeOff()
	{
		AddSpeedBoost(GameManager.GolfSettings.TeeOffPerfectShotSpeedBoostDuration);
	}

	private void AddSpeedBoost(float duration)
	{
		if (!base.isLocalPlayer)
		{
			Debug.LogError("Only the local player is allowed to add a speed boost to themselves", base.gameObject);
		}
		else if (!(duration <= 0f) && (!IsKnockedOutOrRecovering || !(BMath.GetTimeSince(knockoutTimestamp) < GameManager.PlayerMovementSettings.KnockoutDisallowSpeedBoostDuration)) && (!PlayerInfo.AsHittable.IsFrozen || !(BMath.GetTimeSince(PlayerInfo.AsHittable.IsFrozenChangeTimestamp) < GameManager.PlayerMovementSettings.KnockoutDisallowSpeedBoostDuration)))
		{
			AddSpeedBoostAtEndOfFrame(duration);
		}
		async void AddSpeedBoostAtEndOfFrame(float num)
		{
			await UniTask.WaitForEndOfFrame();
			if (!(this == null) && CanAddSpeedBoost())
			{
				bool flag = statusEffects.HasEffect(StatusEffect.SpeedBoost);
				AddStatusEffect(StatusEffect.SpeedBoost);
				SpeedBoostRemainingTime = (flag ? BMath.Min(SpeedBoostRemainingTime + num, GameManager.PlayerMovementSettings.MaxSpeedBoostDuration) : BMath.Min(num, GameManager.PlayerMovementSettings.MaxSpeedBoostDuration));
				if (BMath.GetTimeSince(speedBoostAdditionTimestamp) >= 0.5f)
				{
					PlaySpeedBoostEffectsForAllClients();
				}
				speedBoostAdditionTimestamp = Time.timeAsDouble;
			}
		}
		bool CanAddSpeedBoost()
		{
			if (IsRespawning)
			{
				return false;
			}
			if (PlayerInfo.AsGolfer.IsMatchResolved)
			{
				return false;
			}
			if (PlayerInfo.AsSpectator.IsSpectating)
			{
				return false;
			}
			if (IsKnockedOutOrRecovering && BMath.GetTimeSince(knockoutTimestamp) < GameManager.PlayerMovementSettings.KnockoutDisallowSpeedBoostDuration)
			{
				return false;
			}
			if (PlayerInfo.AsHittable.IsFrozen && BMath.GetTimeSince(PlayerInfo.AsHittable.IsFrozenChangeTimestamp) < GameManager.PlayerMovementSettings.KnockoutDisallowSpeedBoostDuration)
			{
				return false;
			}
			return true;
		}
		void PlaySpeedBoostEffectsForAllClients()
		{
			PlaySpeedBoostEffectsInternal();
			CmdPlaySpeedBoostEffectsForAllClients();
		}
	}

	[Command]
	private void CmdPlaySpeedBoostEffectsForAllClients(NetworkConnectionToClient sender = null)
	{
		if (base.isServer && base.isClient)
		{
			UserCode_CmdPlaySpeedBoostEffectsForAllClients__NetworkConnectionToClient(sender);
			return;
		}
		NetworkWriterPooled writer = NetworkWriterPool.Get();
		SendCommandInternal("System.Void PlayerMovement::CmdPlaySpeedBoostEffectsForAllClients(Mirror.NetworkConnectionToClient)", -782922024, writer, 0);
		NetworkWriterPool.Return(writer);
	}

	[TargetRpc]
	private void RpcPlaySpeedBoostEffects(NetworkConnectionToClient connection)
	{
		NetworkWriterPooled writer = NetworkWriterPool.Get();
		SendTargetRPCInternal(connection, "System.Void PlayerMovement::RpcPlaySpeedBoostEffects(Mirror.NetworkConnectionToClient)", -1677680769, writer, 0);
		NetworkWriterPool.Return(writer);
	}

	private void PlaySpeedBoostEffectsInternal()
	{
		PlayerInfo.Vfx.PlaySpeedBoostEffects();
		PlayerInfo.PlayerAudio.PlaySpeedBoostLocalOnly();
	}

	private void AddStatusEffect(StatusEffect statusEffect)
	{
		if (!base.isLocalPlayer)
		{
			Debug.LogError("Only the local player is allowed to add their own status effects", base.gameObject);
		}
		else
		{
			NetworkstatusEffects = statusEffects | statusEffect;
		}
	}

	private void RemoveStatusEffect(StatusEffect statusEffect)
	{
		if (!base.isLocalPlayer)
		{
			Debug.LogError("Only the local player is allowed to remove their own status effects", base.gameObject);
			return;
		}
		NetworkstatusEffects = statusEffects & ~statusEffect;
		if (statusEffect == StatusEffect.SpeedBoost)
		{
			continuousSpeedBoostTime = 0f;
		}
	}

	private void BlendStrafeStrengthTo(float targetValue, float fullBlendDuration)
	{
		if (strafeStrengthBlendRoutine != null)
		{
			StopCoroutine(strafeStrengthBlendRoutine);
		}
		strafeStrengthBlendRoutine = StartCoroutine(BlendStrafeStrengthRoutine(targetValue, fullBlendDuration));
	}

	private IEnumerator BlendStrafeStrengthRoutine(float targetValue, float fullBlendDuration)
	{
		if (strafeStrength != targetValue)
		{
			float time = 0f;
			float initialValue = strafeStrength;
			float duration = BMath.Abs(targetValue - initialValue) * fullBlendDuration;
			while (time < duration)
			{
				yield return null;
				time += Time.deltaTime;
				strafeStrength = BMath.Lerp(initialValue, targetValue, time / duration);
				PlayerInfo.AnimatorIo.SetStrafeStrength(strafeStrength);
			}
			strafeStrength = targetValue;
			PlayerInfo.AnimatorIo.SetStrafeStrength(strafeStrength);
		}
	}

	private async void LocalPlayerUpdateVisibilityDelayed(float delay)
	{
		await UniTask.WaitForSeconds(delay);
		if (!(this == null))
		{
			LocalPlayerUpdateVisibility();
		}
	}

	private void LocalPlayerUpdateVisibility()
	{
		NetworkisVisible = ShouldBeVisible();
		bool ShouldBeVisible()
		{
			if (isForcedHidden)
			{
				return false;
			}
			if (IsRespawning && !isDrowning)
			{
				return false;
			}
			if (!PlayerInfo.AsGolfer.IsInitialized)
			{
				return false;
			}
			if (PlayerInfo.AsGolfer.MatchResolution == PlayerMatchResolution.Uninitialized)
			{
				return false;
			}
			if (PlayerInfo.AsGolfer.MatchResolution == PlayerMatchResolution.Eliminated)
			{
				switch (PlayerInfo.AsGolfer.LocalPlayerLatestImmediateEliminationReason)
				{
				case EliminationReason.FellIntoFog:
					return false;
				case EliminationReason.FellIntoHole:
					return false;
				case EliminationReason.OrbitalLaserCenter:
					return false;
				case EliminationReason.OutOfBounds:
					return false;
				case EliminationReason.FellIntoWater:
					if (BMath.GetTimeSince(PlayerInfo.AsGolfer.LocalPlayerEliminationTimestamp) >= GameManager.MatchSettings.EliminationInWaterDrowningDuration)
					{
						return false;
					}
					break;
				}
			}
			if (PlayerInfo.AsGolfer.MatchResolution == PlayerMatchResolution.JoinedAsSpectator)
			{
				return false;
			}
			if (PlayerCustomizationMenu.IsActive)
			{
				return false;
			}
			return true;
		}
	}

	private void ApplyVisibility()
	{
		foreach (Renderer renderer in renderers)
		{
			if (renderer != null)
			{
				renderer.enabled = isVisible;
			}
		}
		UpdateEnabledColliders();
		if (!isVisible)
		{
			if (base.isLocalPlayer)
			{
				SetKnockOutState(KnockoutState.None);
				SetDivingState(DivingState.None);
			}
			if (!rigidbody.isKinematic)
			{
				rigidbody.linearVelocity = Vector3.zero;
				rigidbody.angularVelocity = Vector3.zero;
			}
		}
	}

	public void UpdateEnabledColliders()
	{
		if (!isVisible || PlayerInfo.ActiveGolfCartSeat.IsValid())
		{
			uprightCollider.enabled = false;
			divingCollider.enabled = false;
			hittableCollider.enabled = false;
		}
		else
		{
			bool flag = !MatchSetupRules.GetValueAsBool(MatchSetupRules.Rule.HitOtherPlayers);
			bool flag2 = divingState != DivingState.None;
			uprightCollider.enabled = !flag2;
			divingCollider.enabled = flag2;
			hittableCollider.enabled = !flag2 && !flag;
		}
	}

	private void UpdateOutOfBounds(bool forceConsiderAsGrounded)
	{
		bool flag = PlayerInfo.ActiveGolfCartSeat.IsValid();
		if (flag && PlayerInfo.ActiveGolfCartSeat.golfCart.AsEntity.LevelBoundsTracker.AuthoritativeBoundsState.HasFlag(BoundsState.OutOfBounds))
		{
			LocalPlayerUpdateOutOfBoundsMessageRemainingTime();
		}
		UpdateExplorerAchievementProgress();
		if (base.isServer && !NoClipEnabled && PlayerInfo.AsGolfer.IsInitialized && isVisible && PlayerInfo.LevelBoundsTracker.AuthoritativeBoundsState.HasFlag(BoundsState.OutOfBounds) && !(BMath.GetTimeSince(PlayerInfo.AsGolfer.ServerOutOfBoundsTimerEliminationTimestamp) < 2f))
		{
			float timeSince = BMath.GetTimeSince(PlayerInfo.LevelBoundsTracker.OutOfBoundsTimestamp);
			float num = MatchSetupRules.GetValue(MatchSetupRules.Rule.OutOfBounds) - timeSince;
			RpcSetOutOfBoundsMessageRemainingTime(num);
			if (num <= 0f && !flag && (forceConsiderAsGrounded || isGrounded || knockoutState == KnockoutState.OnGround || divingState == DivingState.OnGround))
			{
				PlayerInfo.AsGolfer.ServerEliminate(EliminationReason.OutOfBounds);
				PlayOutOfBoundsEliminationExplosionForAllClients();
			}
		}
		void PlayOutOfBoundsEliminationExplosionForAllClients()
		{
			PlayOutOfBoundsEliminationExplosionInternal();
			CmdPlayOutOfBoundsEliminationExplosionForAllClients();
		}
		void UpdateExplorerAchievementProgress()
		{
			if (base.isLocalPlayer && isVisible && !SingletonBehaviour<DrivingRangeManager>.HasInstance && PlayerInfo.AsGolfer.IsInitialized && !PlayerInfo.AsGolfer.IsMatchResolved && PlayerInfo.LevelBoundsTracker.AuthoritativeBoundsState.HasFlag(BoundsState.OutOfBounds) && (CourseManager.MatchState == MatchState.Ongoing || CourseManager.MatchState == MatchState.CountingDownToEnd))
			{
				BMath.Wrap(BMath.GetTimeSince(localPlayerExplorerAchievementLastOutOfBoundsTimestamp), GameManager.Achievements.ExplorerOutOfBoundsTimeStep, out var wrapCount);
				if (wrapCount > 0 && CourseManager.CountActivePlayers() > 1)
				{
					GameManager.AchievementsManager.IncrementProgress(AchievementId.Explorer, GameManager.Achievements.ExplorerOutOfBoundsTimeStep * wrapCount);
					localPlayerExplorerAchievementLastOutOfBoundsTimestamp = Time.timeAsDouble;
				}
			}
		}
	}

	private void LocalPlayerUpdateIsOutOfBoundsMessageShown()
	{
		if (ShouldShow())
		{
			OutOfBoundsMessage.Show();
		}
		else
		{
			OutOfBoundsMessage.Hide();
		}
		bool ShouldShow()
		{
			if (NoClipEnabled)
			{
				return false;
			}
			if (IsRespawning)
			{
				return false;
			}
			if (!PlayerInfo.AsGolfer.IsInitialized)
			{
				return false;
			}
			if (PlayerInfo.AsGolfer.IsMatchResolved)
			{
				return false;
			}
			if (PlayerInfo.AsSpectator.IsSpectating)
			{
				return false;
			}
			if (!PlayerInfo.LevelBoundsTracker.AuthoritativeBoundsState.HasState(BoundsState.OutOfBounds))
			{
				return false;
			}
			return true;
		}
	}

	private void LocalPlayerUpdateOutOfBoundsMessageRemainingTime()
	{
		OutOfBoundsMessage.SetRemainingTime((PlayerInfo.ActiveGolfCartSeat.IsValid() && PlayerInfo.ActiveGolfCartSeat.golfCart.AsEntity.LevelBoundsTracker.AuthoritativeBoundsState.HasFlag(BoundsState.OutOfBounds)) ? PlayerInfo.ActiveGolfCartSeat.golfCart.OutOfBoundsRemainingTime : localPlayerOutOfBoundsRemainingTime);
	}

	[Command]
	private void CmdPlayOutOfBoundsEliminationExplosionForAllClients(NetworkConnectionToClient sender = null)
	{
		if (base.isServer && base.isClient)
		{
			UserCode_CmdPlayOutOfBoundsEliminationExplosionForAllClients__NetworkConnectionToClient(sender);
			return;
		}
		NetworkWriterPooled writer = NetworkWriterPool.Get();
		SendCommandInternal("System.Void PlayerMovement::CmdPlayOutOfBoundsEliminationExplosionForAllClients(Mirror.NetworkConnectionToClient)", 155584192, writer, 0);
		NetworkWriterPool.Return(writer);
	}

	[TargetRpc]
	private void RpcPlayOutOfBoundsEliminationExplosion(NetworkConnectionToClient connection)
	{
		NetworkWriterPooled writer = NetworkWriterPool.Get();
		SendTargetRPCInternal(connection, "System.Void PlayerMovement::RpcPlayOutOfBoundsEliminationExplosion(Mirror.NetworkConnectionToClient)", -847658745, writer, 0);
		NetworkWriterPool.Return(writer);
	}

	private void PlayOutOfBoundsEliminationExplosionInternal()
	{
		VfxManager.PlayPooledVfxLocalOnly(VfxType.MineExplosion, base.transform.position, Quaternion.identity);
		if (GameplayCameraManager.ShouldPlayImpactFrameForExplosion(base.transform.position, GameManager.ItemSettings.LandmineExplosionRange, GameManager.CameraGameplaySettings.LandmineImpactFrameDistanceSquared))
		{
			CameraModuleController.PlayImpactFrame(base.transform.position);
		}
		CameraModuleController.Shake(GameManager.CameraGameplaySettings.LandmineExplosionScreenshakeSettings, base.transform.position);
		RuntimeManager.PlayOneShot(GameManager.AudioSettings.LandmineExplosionEvent, base.transform.position);
	}

	private void UpdateKnockoutImmunityVfx()
	{
		bool flag = knockoutImmunityVfx != null;
		bool isPlaying = ShouldPlay();
		bool wasKnockoutProtectedFromLocalPlayer = false;
		if (!base.isLocalPlayer)
		{
			wasKnockoutProtectedFromLocalPlayer = isKnockoutProtectedFromLocalPlayer;
			isKnockoutProtectedFromLocalPlayer = IsKnockoutProtectedFromPlayer(GameManager.LocalPlayerInfo);
			isPlaying |= isKnockoutProtectedFromLocalPlayer;
		}
		isPlaying &= !PlayerInfo.AsEntity.IsDestroyed;
		if (isPlaying == flag)
		{
			UpdateColor();
		}
		else if (isPlaying)
		{
			PlayVfx();
		}
		else
		{
			ClearVfx();
		}
		void ClearVfx()
		{
			if (knockoutImmunityVfx != null)
			{
				knockoutImmunityVfx.Stop(ParticleSystemStopBehavior.StopEmittingAndClear);
				knockoutImmunityVfx = null;
			}
			if (!PlayerInfo.AsEntity.IsDestroyed && VfxPersistentData.TryGetPooledVfx(VfxType.KnockOutShieldEnd, out var particleSystem))
			{
				particleSystem.GetComponent<KnockOutVfxVisuals>().SetColor(wasKnockoutProtectedFromLocalPlayer ? KnockOutVfxColor.Red : knockoutImmunityStatus.color);
				particleSystem.transform.SetParent(PlayerInfo.ChestBone);
				particleSystem.transform.localPosition = Vector3.zero;
				particleSystem.Play();
			}
		}
		void PlayVfx()
		{
			if (knockoutImmunityVfx == null && VfxPersistentData.TryGetPooledVfx(VfxType.KnockOutShield, out knockoutImmunityVfx))
			{
				UpdateColor();
				knockoutImmunityVfx.transform.SetParent(PlayerInfo.ChestBone);
				knockoutImmunityVfx.transform.localPosition = Vector3.zero;
				knockoutImmunityVfx.Play();
			}
		}
		bool ShouldPlay()
		{
			if (!knockoutImmunityStatus.hasImmunity)
			{
				return false;
			}
			if (!isVisible)
			{
				return false;
			}
			return true;
		}
		void UpdateColor()
		{
			if (isPlaying && knockoutImmunityVfx != null)
			{
				knockoutImmunityVfx.GetComponent<KnockOutVfxVisuals>().SetColor(isKnockoutProtectedFromLocalPlayer ? KnockOutVfxColor.Red : knockoutImmunityStatus.color);
			}
		}
	}

	private void UpdateCanPassThroughDynamicBalls()
	{
		SetCollidersCanPassThrough(ShouldPassThrough());
		static void SetColliderCanPassThrough(Collider collider, bool canPassThrough)
		{
			if (canPassThrough)
			{
				collider.excludeLayers = (int)collider.excludeLayers | (int)GameManager.LayerSettings.DynamicBallMask;
			}
			else
			{
				collider.excludeLayers = (int)collider.excludeLayers & ~(int)GameManager.LayerSettings.DynamicBallMask;
			}
		}
		void SetCollidersCanPassThrough(bool canPassThrough)
		{
			SetColliderCanPassThrough(uprightCollider, canPassThrough);
			SetColliderCanPassThrough(divingCollider, canPassThrough);
			SetColliderCanPassThrough(hittableCollider, canPassThrough);
		}
		bool ShouldPassThrough()
		{
			return knockoutImmunityStatus.hasImmunity;
		}
	}

	public bool IsKnockoutProtectedFromPlayer(PlayerInfo otherPlayer)
	{
		if (otherPlayer == null || otherPlayer == PlayerInfo)
		{
			return false;
		}
		if (!CourseManager.PlayerKnockoutStreaks.TryGetValue(new CourseManager.PlayerPair(otherPlayer.PlayerId.Guid, PlayerInfo.PlayerId.Guid), out var value))
		{
			return false;
		}
		if (value < 6)
		{
			return false;
		}
		return true;
	}

	private bool CanGround()
	{
		if (IsKnockedOut)
		{
			return false;
		}
		if (divingState != DivingState.None)
		{
			return false;
		}
		if (respawnState == RespawnState.WaitingToRespawn)
		{
			return false;
		}
		if (PlayerInfo.AsHittable.IsFrozen)
		{
			return false;
		}
		if (!isGrounded && Velocity.y >= GameManager.PlayerMovementSettings.VerticalVelocityGroundingThreshold)
		{
			return false;
		}
		return true;
	}

	private bool CanMove()
	{
		if (!SingletonBehaviour<DrivingRangeManager>.HasInstance && CourseManager.MatchState <= MatchState.TeeOff)
		{
			return false;
		}
		if (isDrowning)
		{
			return false;
		}
		if (!PlayerInfo.AsGolfer.CanMove())
		{
			return false;
		}
		if (PlayerInfo.AsSpectator.IsSpectating)
		{
			return false;
		}
		if (PlayerInfo.Inventory.GetEffectivelyEquippedItem() == ItemType.Landmine && PlayerInfo.Inventory.CurrentItemUse == ItemUseType.Regular)
		{
			return false;
		}
		return true;
	}

	private bool CanJump()
	{
		if (!isGrounded)
		{
			return false;
		}
		if (PlayerInfo.AsGolfer.IsMatchResolved)
		{
			return false;
		}
		if (PlayerInfo.AsSpectator.IsSpectating)
		{
			return false;
		}
		if (!PlayerInfo.AsGolfer.CanInterruptSwing())
		{
			return false;
		}
		if (PlayerInfo.Inventory.GetEffectivelyEquippedItem() == ItemType.Landmine && PlayerInfo.Inventory.IsUsingItemAtAll)
		{
			return false;
		}
		return true;
	}

	public void PlayGolfCartKnockoutEffectsForAllClients(Vector3 localPosition)
	{
		PlayGolfCartKnockoutEffectsInternal(localPosition);
		CmdPlayGolfCartKnockoutEffectsForAllClients(localPosition);
	}

	[Command]
	private void CmdPlayGolfCartKnockoutEffectsForAllClients(Vector3 localPosition, NetworkConnectionToClient sender = null)
	{
		if (base.isServer && base.isClient)
		{
			UserCode_CmdPlayGolfCartKnockoutEffectsForAllClients__Vector3__NetworkConnectionToClient(localPosition, sender);
			return;
		}
		NetworkWriterPooled writer = NetworkWriterPool.Get();
		writer.WriteVector3(localPosition);
		SendCommandInternal("System.Void PlayerMovement::CmdPlayGolfCartKnockoutEffectsForAllClients(UnityEngine.Vector3,Mirror.NetworkConnectionToClient)", 1168678433, writer, 0);
		NetworkWriterPool.Return(writer);
	}

	[TargetRpc]
	private void RpcPlayGolfCartKnockoutEffects(NetworkConnectionToClient connection, Vector3 localPosition)
	{
		NetworkWriterPooled writer = NetworkWriterPool.Get();
		writer.WriteVector3(localPosition);
		SendTargetRPCInternal(connection, "System.Void PlayerMovement::RpcPlayGolfCartKnockoutEffects(Mirror.NetworkConnectionToClient,UnityEngine.Vector3)", 973802672, writer, 0);
		NetworkWriterPool.Return(writer);
	}

	private void PlayGolfCartKnockoutEffectsInternal(Vector3 localPosition)
	{
		Vector3 vector = base.transform.TransformPoint(localPosition);
		PlayerInfo.PlayerAudio.PlayGolfCartKnockoutLocalOnly(vector);
		VfxManager.PlayPooledVfxLocalOnly(VfxType.GolfCartCollision, vector, Quaternion.identity);
	}

	private void UpdateRocketDriverSwingMissTrailEffects()
	{
		bool flag = rocketDriverSwingMissTrailVfx != null;
		bool flag2 = ShouldPlay();
		if (flag2 != flag)
		{
			if (rocketDriverSwingMissTrailSound.isValid())
			{
				rocketDriverSwingMissTrailSound.stop(FMOD.Studio.STOP_MODE.ALLOWFADEOUT);
			}
			if (flag2)
			{
				PlayVfx();
			}
			else
			{
				ClearVfx();
			}
		}
		void ClearVfx()
		{
			if (rocketDriverSwingMissTrailVfx != null)
			{
				rocketDriverSwingMissTrailVfx.Stop();
				rocketDriverSwingMissTrailVfx = null;
			}
			if (!BNetworkManager.IsChangingSceneOrShuttingDown)
			{
				RuntimeManager.PlayOneShot(GameManager.AudioSettings.RocketDriverTrailStopEvent, base.transform.position);
			}
		}
		void PlayVfx()
		{
			if (rocketDriverSwingMissTrailVfx == null && VfxPersistentData.TryGetPooledVfx(VfxType.RocketDriverTrail, out rocketDriverSwingMissTrailVfx))
			{
				rocketDriverSwingMissTrailVfx.transform.SetParent(PlayerInfo.ChestBone);
				rocketDriverSwingMissTrailVfx.transform.localPosition = Vector3.zero;
				rocketDriverSwingMissTrailVfx.Play();
			}
			rocketDriverSwingMissTrailSound = RuntimeManager.CreateInstance(GameManager.AudioSettings.RocketDriverTrailLoopEvent);
			RuntimeManager.AttachInstanceToGameObject(rocketDriverSwingMissTrailSound, base.gameObject);
			rocketDriverSwingMissTrailSound.start();
			rocketDriverSwingMissTrailSound.release();
		}
		bool ShouldPlay()
		{
			if (PlayerInfo.AsEntity.IsDestroyed)
			{
				return false;
			}
			if (divingState != DivingState.Diving)
			{
				return false;
			}
			if (diveType != DiveType.RocketDriverSwingMiss)
			{
				return false;
			}
			if (!isVisible)
			{
				return false;
			}
			return true;
		}
	}

	private void OnLocalBoundsStateChanged(BoundsState previousState, BoundsState currentState)
	{
		bool flag = previousState.HasState(BoundsState.OutOfBounds);
		bool flag2 = currentState.HasState(BoundsState.OutOfBounds);
		bool flag3 = previousState.HasState(BoundsState.InMainOutOfBoundsHazard | BoundsState.InSecondaryOutOfBoundsHazard);
		bool flag4 = currentState.HasState(BoundsState.InMainOutOfBoundsHazard | BoundsState.InSecondaryOutOfBoundsHazard);
		if (flag2 != flag && flag2)
		{
			localPlayerExplorerAchievementLastOutOfBoundsTimestamp = Time.timeAsDouble;
		}
		if (flag4 != flag3 && flag4 && ((!currentState.HasState(BoundsState.InMainOutOfBoundsHazard)) ? (PlayerInfo.LevelBoundsTracker.CurrentSecondaryHazardLocalOnly.Type == OutOfBoundsHazard.Water) : (MainOutOfBoundsHazard.Type == OutOfBoundsHazard.Water)))
		{
			Vector3 position = PlayerInfo.ChestBone.position;
			position.y = PlayerInfo.LevelBoundsTracker.CurrentOutOfBoundsHazardWorldHeightLocalOnly;
			VfxManager.PlayPooledVfxLocalOnly(VfxType.WaterImpactMedium, position, Quaternion.identity);
			PlayerInfo.PlayerAudio.PlayWaterSplashLocalOnly(position);
		}
	}

	private void OnFootstep(Foot foot)
	{
		if (isWadingInWater)
		{
			PlayerInfo.PlayerAudio.PlayWaterWadeLocalOnly(foot.Opposite());
		}
	}

	private void OnServerBoundsStateChanged(BoundsState previousState, BoundsState currentState)
	{
		if (!NoClipEnabled && PlayerInfo.AsGolfer.IsInitialized && currentState.IsInOutOfBoundsHazard() && !PlayerInfo.ActiveGolfCartSeat.IsValid())
		{
			PlayerInfo.AsGolfer.ServerEliminate(PlayerInfo.LevelBoundsTracker.GetPotentialOutOfBoundsHazardEliminationReason());
		}
	}

	private void OnLocalPlayerBoundsStateChanged(BoundsState previousState, BoundsState currentState)
	{
		LocalPlayerUpdateIsOutOfBoundsMessageShown();
		if (diveType == DiveType.RocketDriverSwingMiss && currentState.IsInOutOfBoundsHazard())
		{
			NetworkdiveType = DiveType.Regular;
		}
	}

	private void OnLocalPlayerMatchResolutionChanged(PlayerMatchResolution previousResolution, PlayerMatchResolution currentResolution)
	{
		if (PlayerInfo.AsGolfer.IsMatchResolved)
		{
			CancelRespawn();
			RemoveStatusEffect(StatusEffect.SpeedBoost);
			OutOfBoundsMessage.Hide();
		}
		if (currentResolution == PlayerMatchResolution.Eliminated)
		{
			SetKnockOutState(KnockoutState.None);
		}
		LocalPlayerUpdateVisibility();
		LocalPlayerUpdateIsOutOfBoundsMessageShown();
		if (currentResolution == PlayerMatchResolution.Eliminated && PlayerInfo.AsGolfer.LocalPlayerLatestImmediateEliminationReason == EliminationReason.FellIntoWater)
		{
			SetIsDrowning(isDrowning: true);
		}
	}

	private void OnLocalPlayerIsSpectatingChanged()
	{
		LocalPlayerUpdateIsOutOfBoundsMessageShown();
	}

	private void OnLocalPlayerWillApplyGolfSwingHitPhysics(PlayerGolfer hitter, float power, Vector3 hitLocalPosition, Vector3 localOrigin, Vector3 incomingVelocityChange, bool isRocketDriver)
	{
		if (!TryKnockOut(hitter.PlayerInfo, isRocketDriver ? KnockoutType.RocketDriverSwing : KnockoutType.Swing, power < 0.2f, localOrigin, 0f, incomingVelocityChange, canBeBlockedByElectromagnetShield: true, ItemUseId.Invalid, fromSpecialState: false, canFallbackToUnground: true, out var _) && PlayerInfo.IsElectromagnetShieldActive)
		{
			Vector3 vector = base.transform.TransformPoint(hitLocalPosition);
			PlayerInfo.PlayElectromagnetShieldHitForAllClients(vector - PlayerInfo.ElectromagnetShieldCollider.transform.position);
		}
	}

	private void OnLocalPlayerWillApplySwingProjectileHitPhysics(Hittable hitter, Vector3 localHitPosition, Vector3 projectileSwingHitPosition, Vector3 incomingVelocityChange, bool wasSwungByRocketDriver)
	{
		KnockoutType knockoutType = ((hitter.SwingProjectileState == SwingProjectileState.ReflectedProjectile) ? KnockoutType.ReflectedSwingProjectile : ((!wasSwungByRocketDriver) ? KnockoutType.SwingProjectile : KnockoutType.RocketDriverSwingProjectile));
		Vector3 vector = base.transform.TransformPoint(localHitPosition);
		if (TryKnockOut(hitter.ResponsibleSwingProjectilePlayer.PlayerInfo, knockoutType, isLegSweep: false, localHitPosition, (vector - projectileSwingHitPosition).magnitude, incomingVelocityChange, canBeBlockedByElectromagnetShield: true, ItemUseId.Invalid, fromSpecialState: false, canFallbackToUnground: true, out var isNewKnockout) && isNewKnockout && (knockoutType == KnockoutType.SwingProjectile || knockoutType == KnockoutType.RocketDriverSwingProjectile) && hitter.ResponsibleSwingProjectilePlayer == PlayerInfo.AsGolfer && PlayerInfo.AsGolfer.OwnBall != null && hitter == PlayerInfo.AsGolfer.OwnBall.AsEntity.AsHittable)
		{
			GameManager.AchievementsManager.Unlock(AchievementId.HowIsThatEvenPossible);
		}
	}

	private void OnLocalPlayerWillApplyDiveHitPhysics(PlayerMovement hitter, Vector3 appliedVelocity)
	{
		if (!isGrounded)
		{
			return;
		}
		if (BMath.GetTimeSince(lastHitByDaveTimestamp) >= GameManager.PlayerMovementSettings.RepeatedDiveHitKnockoutImmunityTimeWindow)
		{
			recentHitsByDiveCount = 0;
		}
		if (!knockoutImmunityStatus.hasImmunity && (hitter == null || !IsKnockoutProtectedFromPlayer(hitter.PlayerInfo)))
		{
			recentHitsByDiveCount++;
			lastHitByDaveTimestamp = Time.timeAsDouble;
			if ((float)recentHitsByDiveCount >= GameManager.PlayerMovementSettings.RepeatedDiveHitCountForKnockoutImmunity)
			{
				StartKnockoutImmunity(fromPlayerAggression: true);
			}
		}
		if (knockoutImmunityStatus.hasImmunity || (hitter != null && IsKnockoutProtectedFromPlayer(hitter.PlayerInfo)))
		{
			PlayKnockoutBlockedEffectForAllClients();
		}
		else if (Vector3.Dot(GroundData.normal, appliedVelocity) > 0f)
		{
			Unground();
		}
	}

	private void OnLocalPlayerWillApplyItemHitPhysics(PlayerInventory itemUser, ItemType itemType, ItemUseId itemUseId, Vector3 localOrigin, float distance, Vector3 incomingVelocityChange, bool isReflected, bool isInSpecialState)
	{
		PlayerInfo responsiblePlayer = ((itemUser != null) ? itemUser.PlayerInfo : null);
		KnockoutType knockoutType = (KnockoutType)(-1);
		switch (itemType)
		{
		case ItemType.DuelingPistol:
			knockoutType = (isReflected ? KnockoutType.DeflectedDuelingPistolShot : KnockoutType.DuelingPistol);
			break;
		case ItemType.ElephantGun:
			knockoutType = (isReflected ? KnockoutType.DeflectedElephantGunShot : KnockoutType.ElephantGun);
			break;
		case ItemType.RocketLauncher:
			knockoutType = (isReflected ? KnockoutType.ReflectedRocket : KnockoutType.Rocket);
			break;
		case ItemType.Landmine:
			knockoutType = KnockoutType.Landmine;
			break;
		case ItemType.OrbitalLaser:
			if (distance > GameManager.ItemSettings.OrbitalLaserExplosionCenterRadius)
			{
				knockoutType = KnockoutType.OrbitalLaserPeriphery;
			}
			break;
		case ItemType.RocketDriver:
			knockoutType = KnockoutType.RocketDriverSwing;
			break;
		}
		if (knockoutType >= KnockoutType.Swing)
		{
			bool isNewKnockout;
			bool flag = TryKnockOut(responsiblePlayer, knockoutType, isLegSweep: false, localOrigin, distance, incomingVelocityChange, canBeBlockedByElectromagnetShield: true, itemUseId, isInSpecialState, canFallbackToUnground: true, out isNewKnockout);
			if (((uint)(itemType - 7) <= 1u || itemType == ItemType.OrbitalLaser) && !flag && PlayerInfo.IsElectromagnetShieldActive)
			{
				PlayElectromagnetShieldHit();
			}
		}
		void PlayElectromagnetShieldHit()
		{
			Vector3 vector = base.transform.TransformPoint(localOrigin);
			PlayerInfo.PlayElectromagnetShieldHitForAllClients(vector - PlayerInfo.ElectromagnetShieldCollider.transform.position);
		}
	}

	private void OnLocalPlayerWillApplyRocketLauncherBackBlastHitPhysics(PlayerInventory rocketLauncherUser, Vector3 hitLocalPosition, Vector3 localOrigin, Vector3 incomingVelocityChange)
	{
		if (!TryKnockOut(rocketLauncherUser.PlayerInfo, KnockoutType.RocketBackBlast, isLegSweep: false, localOrigin, localOrigin.magnitude, incomingVelocityChange, canBeBlockedByElectromagnetShield: true, ItemUseId.Invalid, fromSpecialState: false, canFallbackToUnground: true, out var _) && PlayerInfo.IsElectromagnetShieldActive)
		{
			Vector3 vector = base.transform.TransformPoint(hitLocalPosition);
			PlayerInfo.PlayElectromagnetShieldHitForAllClients(vector - PlayerInfo.ElectromagnetShieldCollider.transform.position);
		}
	}

	private void OnLocalPlayerWillWillApplyRocketDriverSwingPostHitSpinHitPhysics(PlayerGolfer hitter, Vector3 hitLocalPosition, Vector3 localOrigin, Vector3 incomingVelocityChange)
	{
		if (!TryKnockOut(hitter.PlayerInfo, KnockoutType.RocketDriverSwingPostHitSpin, isLegSweep: false, localOrigin, localOrigin.magnitude, incomingVelocityChange, canBeBlockedByElectromagnetShield: true, ItemUseId.Invalid, fromSpecialState: false, canFallbackToUnground: true, out var _) && PlayerInfo.IsElectromagnetShieldActive)
		{
			Vector3 vector = base.transform.TransformPoint(hitLocalPosition);
			PlayerInfo.PlayElectromagnetShieldHitForAllClients(vector - PlayerInfo.ElectromagnetShieldCollider.transform.position);
		}
	}

	private void OnLocalPlayerWillApplyReturnedBallHitPhysics(Vector3 incomingVelocityChange)
	{
		if (!TryKnockOut(PlayerInfo, KnockoutType.ReturnedBall, isLegSweep: false, Vector3.zero, 0f, incomingVelocityChange, canBeBlockedByElectromagnetShield: true, ItemUseId.Invalid, fromSpecialState: false, canFallbackToUnground: false, out var _) && PlayerInfo.IsElectromagnetShieldActive)
		{
			PlayerInfo.PlayElectromagnetShieldHitForAllClients(Vector3.up);
		}
	}

	private void OnLocalPlayerWillApplyScoreKnockbackPhysics()
	{
		Unground();
	}

	private void OnLocalPlayerWillApplyJumpPadPhysics()
	{
		Unground(fromJumpPad: true);
	}

	private void OnLocalPlayerIsFrozenChanged()
	{
		if (PlayerInfo.AsHittable.IsFrozen)
		{
			RecoverFromKnockout();
			SetDivingState(DivingState.None);
			RemoveStatusEffect(StatusEffect.SpeedBoost);
			SetIsInSpringBootsJump(isInJump: false, fromLanding: false);
			Unground();
		}
		else
		{
			StartKnockoutImmunity(fromPlayerAggression: true);
		}
		UpdateSpecialStatePhysics();
		PlayerInfo.AnimatorIo.SetIsFrozen(PlayerInfo.AsHittable.IsFrozen);
	}

	private void OnIsGroundedChanged(bool wasGrounded, bool isGrounded)
	{
		if (!base.isLocalPlayer)
		{
			this.IsGroundedChanged?.Invoke();
		}
	}

	private void OnIsWadingInWaterChanged(bool wasWading, bool isWading)
	{
		PlayerInfo.Vfx.SetIsWadingInWater(isWadingInWater);
		PlayerInfo.Vfx.SetWadingWaterWorldHeight(PlayerInfo.LevelBoundsTracker.CurrentOutOfBoundsHazardWorldHeightLocalOnly);
		if (!base.isLocalPlayer)
		{
			return;
		}
		if (isWading)
		{
			Vector3 vector = Velocity.Horizontalized();
			float magnitude = vector.magnitude;
			float num = GameManager.PlayerMovementSettings.DefaultMoveSpeed * 1.25f;
			if (statusEffects.HasEffect(StatusEffect.SpeedBoost))
			{
				num *= GameManager.PlayerMovementSettings.SpeedBoostSpeedFactor;
			}
			if (magnitude > GameManager.PlayerMovementSettings.WadingInWaterSpeed && magnitude <= num)
			{
				Velocity = vector * GameManager.PlayerMovementSettings.WadingInWaterSpeed / magnitude + Velocity.Verticalized();
			}
		}
		PlayerInfo.AnimatorIo.SetIsWadingInWater(isWadingInWater);
	}

	private void OnIsVisibleChanged(bool wasVisible, bool isVisible)
	{
		ApplyVisibility();
		UpdateKnockoutImmunityVfx();
		UpdateRocketDriverSwingMissTrailEffects();
		this.IsVisibleChanged?.Invoke();
	}

	private void OnKnockedOutVfxDataChanged(KnockedOutVfxData previousData, KnockedOutVfxData currentData)
	{
		bool num = previousData.totalStarCount > 0;
		bool flag = currentData.totalStarCount > 0;
		if (num != flag)
		{
			if (flag)
			{
				if (knockedOutVfx == null && VfxPersistentData.TryGetPooledVfx(VfxType.KnockedOut, out var particleSystem))
				{
					knockedOutVfx = particleSystem.GetComponent<KnockedOutVfx>();
					knockedOutVfx.transform.SetParent(PlayerInfo.HeadBone);
					knockedOutVfx.transform.localPosition = Vector3.zero;
					knockedOutVfx.Initialize(currentData.totalStarCount, currentData.totalStarCount > 3);
					knockedOutVfx.AsPoolable.Play();
				}
				PlayerInfo.PlayerAudio.StartKnockoutLoopLocalOnly();
			}
			else
			{
				if (knockedOutVfx != null)
				{
					knockedOutVfx.AsPoolable.Stop(ParticleSystemStopBehavior.StopEmittingAndClear);
					knockedOutVfx = null;
				}
				if (VfxPersistentData.TryGetPooledVfx(VfxType.KnockedOutEnd, out var particleSystem2))
				{
					particleSystem2.transform.SetParent(PlayerInfo.HeadBone);
					particleSystem2.transform.localPosition = Vector3.zero;
					particleSystem2.Play();
				}
				PlayerInfo.PlayerAudio.EndKnockoutLoopLocalOnly();
			}
		}
		if (knockedOutVfx != null)
		{
			knockedOutVfx.SetColoredStarCount(currentData.coloredStarCount);
		}
	}

	private void OnKnockoutImmunityStatusChanged(KnockOutImmunity previousStatus, KnockOutImmunity currentStatus)
	{
		UpdateKnockoutImmunityVfx();
		if (currentStatus.hasImmunity)
		{
			PlayerInfo.PlayerAudio.PlayKnockoutImmunityStartLocalOnly();
		}
		else
		{
			PlayerInfo.PlayerAudio.PlayKnockoutImmunityEndLocalOnly();
		}
		if (currentStatus.hasImmunity != previousStatus.hasImmunity)
		{
			UpdateCanPassThroughDynamicBalls();
			this.HasKnockoutImmunityChanged?.Invoke();
		}
	}

	private void OnStatusEffectsChanged(StatusEffect previousEffects, StatusEffect currentEffects)
	{
		bool num = previousEffects.HasEffect(StatusEffect.SpeedBoost);
		bool flag = currentEffects.HasEffect(StatusEffect.SpeedBoost);
		if (num != flag)
		{
			if (base.isLocalPlayer)
			{
				PlayerInfo.AnimatorIo.SetHasSpeedBoost(flag);
			}
			if (!flag)
			{
				PlayerInfo.Vfx.StopSpeedBoostEffects();
			}
		}
		if (PlayerInfo.ActiveGolfCartSeat.IsDriver())
		{
			PlayerInfo.ActiveGolfCartSeat.golfCart.InformDriverStatusEffectsChanged();
		}
	}

	private void OnDivingStateChanged(DivingState previousState, DivingState currentState)
	{
		if (currentState == DivingState.OnGround)
		{
			diveOnGroundTimestampLocal = Time.timeAsDouble;
		}
		UpdateEnabledColliders();
		UpdateRocketDriverSwingMissTrailEffects();
	}

	private void OnDiveTypeChanged(DiveType previousType, DiveType currentType)
	{
		UpdateRocketDriverSwingMissTrailEffects();
	}

	private void OnRespawnStateChanged(RespawnState previousState, RespawnState currentState)
	{
		bool flag = previousState != RespawnState.None;
		if (IsRespawning == flag)
		{
			return;
		}
		if (base.isLocalPlayer)
		{
			PlayerInfo.AsGolfer.InformLocalPlayerIsRespawningChanged();
			PlayerInfo.Inventory.InformLocalPlayerIsRespawningChanged();
			if (IsRespawning)
			{
				PlayerInfo.CancelEmote(canHideEmoteMenu: true);
				PlayerInfo.Inventory.CancelItemFlourish();
			}
		}
		this.IsRespawningChanged?.Invoke();
		PlayerMovement.AnyPlayerIsRespawningChanged?.Invoke(this);
	}

	private void OnPlayerDominationsChanged(SyncSet<CourseManager.PlayerPair>.Operation operation, CourseManager.PlayerPair pair)
	{
		if (!(GameManager.LocalPlayerId == null) && pair.playerAGuid == GameManager.LocalPlayerId.Guid && pair.playerBGuid == PlayerInfo.PlayerId.Guid)
		{
			UpdateKnockoutImmunityVfx();
		}
	}

	public PlayerMovement()
	{
		_Mirror_SyncVarHookDelegate_isGrounded = OnIsGroundedChanged;
		_Mirror_SyncVarHookDelegate_isWadingInWater = OnIsWadingInWaterChanged;
		_Mirror_SyncVarHookDelegate_isVisible = OnIsVisibleChanged;
		_Mirror_SyncVarHookDelegate_knockedOutVfxData = OnKnockedOutVfxDataChanged;
		_Mirror_SyncVarHookDelegate_knockoutImmunityStatus = OnKnockoutImmunityStatusChanged;
		_Mirror_SyncVarHookDelegate_statusEffects = OnStatusEffectsChanged;
		_Mirror_SyncVarHookDelegate_divingState = OnDivingStateChanged;
		_Mirror_SyncVarHookDelegate_diveType = OnDiveTypeChanged;
		_Mirror_SyncVarHookDelegate_respawnState = OnRespawnStateChanged;
	}

	static PlayerMovement()
	{
		consoleSpeedFactor = 1f;
		RemoteProcedureCalls.RegisterCommand(typeof(PlayerMovement), "System.Void PlayerMovement::CmdPlayKnockoutBlockedEffectForAllClients(Mirror.NetworkConnectionToClient)", InvokeUserCode_CmdPlayKnockoutBlockedEffectForAllClients__NetworkConnectionToClient, requiresAuthority: true);
		RemoteProcedureCalls.RegisterCommand(typeof(PlayerMovement), "System.Void PlayerMovement::CmdInformKnockedOut(PlayerInfo,KnockoutType,UnityEngine.Vector3,System.Single,ItemType,ItemUseId,System.Boolean)", InvokeUserCode_CmdInformKnockedOut__PlayerInfo__KnockoutType__Vector3__Single__ItemType__ItemUseId__Boolean, requiresAuthority: true);
		RemoteProcedureCalls.RegisterCommand(typeof(PlayerMovement), "System.Void PlayerMovement::CmdClientInformRestarted()", InvokeUserCode_CmdClientInformRestarted, requiresAuthority: true);
		RemoteProcedureCalls.RegisterCommand(typeof(PlayerMovement), "System.Void PlayerMovement::CmdPlayRespawnEffectsForAllClients(Checkpoint,Mirror.NetworkConnectionToClient)", InvokeUserCode_CmdPlayRespawnEffectsForAllClients__Checkpoint__NetworkConnectionToClient, requiresAuthority: true);
		RemoteProcedureCalls.RegisterCommand(typeof(PlayerMovement), "System.Void PlayerMovement::CmdInformGrounded()", InvokeUserCode_CmdInformGrounded, requiresAuthority: true);
		RemoteProcedureCalls.RegisterCommand(typeof(PlayerMovement), "System.Void PlayerMovement::CmdPlaySpringBootsLandingForAllClients(UnityEngine.Vector3,Mirror.NetworkConnectionToClient)", InvokeUserCode_CmdPlaySpringBootsLandingForAllClients__Vector3__NetworkConnectionToClient, requiresAuthority: true);
		RemoteProcedureCalls.RegisterCommand(typeof(PlayerMovement), "System.Void PlayerMovement::CmdInformTeleported(UnityEngine.Vector3,UnityEngine.Quaternion,Mirror.NetworkConnectionToClient)", InvokeUserCode_CmdInformTeleported__Vector3__Quaternion__NetworkConnectionToClient, requiresAuthority: true);
		RemoteProcedureCalls.RegisterCommand(typeof(PlayerMovement), "System.Void PlayerMovement::CmdPlaySpeedBoostEffectsForAllClients(Mirror.NetworkConnectionToClient)", InvokeUserCode_CmdPlaySpeedBoostEffectsForAllClients__NetworkConnectionToClient, requiresAuthority: true);
		RemoteProcedureCalls.RegisterCommand(typeof(PlayerMovement), "System.Void PlayerMovement::CmdPlayOutOfBoundsEliminationExplosionForAllClients(Mirror.NetworkConnectionToClient)", InvokeUserCode_CmdPlayOutOfBoundsEliminationExplosionForAllClients__NetworkConnectionToClient, requiresAuthority: true);
		RemoteProcedureCalls.RegisterCommand(typeof(PlayerMovement), "System.Void PlayerMovement::CmdPlayGolfCartKnockoutEffectsForAllClients(UnityEngine.Vector3,Mirror.NetworkConnectionToClient)", InvokeUserCode_CmdPlayGolfCartKnockoutEffectsForAllClients__Vector3__NetworkConnectionToClient, requiresAuthority: true);
		RemoteProcedureCalls.RegisterRpc(typeof(PlayerMovement), "System.Void PlayerMovement::RpcInformSpawned(UnityEngine.Vector3,UnityEngine.Quaternion)", InvokeUserCode_RpcInformSpawned__Vector3__Quaternion);
		RemoteProcedureCalls.RegisterRpc(typeof(PlayerMovement), "System.Void PlayerMovement::RpcPlayKnockoutBlockedEffect(Mirror.NetworkConnectionToClient)", InvokeUserCode_RpcPlayKnockoutBlockedEffect__NetworkConnectionToClient);
		RemoteProcedureCalls.RegisterRpc(typeof(PlayerMovement), "System.Void PlayerMovement::RpcBeginRespawn(System.Boolean,RespawnTarget)", InvokeUserCode_RpcBeginRespawn__Boolean__RespawnTarget);
		RemoteProcedureCalls.RegisterRpc(typeof(PlayerMovement), "System.Void PlayerMovement::RpcPlayRespawnEffects(Mirror.NetworkConnectionToClient,Checkpoint)", InvokeUserCode_RpcPlayRespawnEffects__NetworkConnectionToClient__Checkpoint);
		RemoteProcedureCalls.RegisterRpc(typeof(PlayerMovement), "System.Void PlayerMovement::RpcPlayRestartSound(Mirror.NetworkConnectionToClient)", InvokeUserCode_RpcPlayRestartSound__NetworkConnectionToClient);
		RemoteProcedureCalls.RegisterRpc(typeof(PlayerMovement), "System.Void PlayerMovement::RpcSetIsForceHidden(System.Boolean)", InvokeUserCode_RpcSetIsForceHidden__Boolean);
		RemoteProcedureCalls.RegisterRpc(typeof(PlayerMovement), "System.Void PlayerMovement::RpcPlaySpringBootsLanding(Mirror.NetworkConnectionToClient,UnityEngine.Vector3)", InvokeUserCode_RpcPlaySpringBootsLanding__NetworkConnectionToClient__Vector3);
		RemoteProcedureCalls.RegisterRpc(typeof(PlayerMovement), "System.Void PlayerMovement::RpcSetOutOfBoundsMessageRemainingTime(System.Single)", InvokeUserCode_RpcSetOutOfBoundsMessageRemainingTime__Single);
		RemoteProcedureCalls.RegisterRpc(typeof(PlayerMovement), "System.Void PlayerMovement::RpcReturnToBounds(Mirror.NetworkConnectionToClient)", InvokeUserCode_RpcReturnToBounds__NetworkConnectionToClient);
		RemoteProcedureCalls.RegisterRpc(typeof(PlayerMovement), "System.Void PlayerMovement::RpcInformedTeleported(Mirror.NetworkConnectionToClient,UnityEngine.Vector3,UnityEngine.Quaternion)", InvokeUserCode_RpcInformedTeleported__NetworkConnectionToClient__Vector3__Quaternion);
		RemoteProcedureCalls.RegisterRpc(typeof(PlayerMovement), "System.Void PlayerMovement::RpcInformKnockedOutOtherPlayer(PlayerInfo,System.Boolean)", InvokeUserCode_RpcInformKnockedOutOtherPlayer__PlayerInfo__Boolean);
		RemoteProcedureCalls.RegisterRpc(typeof(PlayerMovement), "System.Void PlayerMovement::RpcPlaySpeedBoostEffects(Mirror.NetworkConnectionToClient)", InvokeUserCode_RpcPlaySpeedBoostEffects__NetworkConnectionToClient);
		RemoteProcedureCalls.RegisterRpc(typeof(PlayerMovement), "System.Void PlayerMovement::RpcPlayOutOfBoundsEliminationExplosion(Mirror.NetworkConnectionToClient)", InvokeUserCode_RpcPlayOutOfBoundsEliminationExplosion__NetworkConnectionToClient);
		RemoteProcedureCalls.RegisterRpc(typeof(PlayerMovement), "System.Void PlayerMovement::RpcPlayGolfCartKnockoutEffects(Mirror.NetworkConnectionToClient,UnityEngine.Vector3)", InvokeUserCode_RpcPlayGolfCartKnockoutEffects__NetworkConnectionToClient__Vector3);
	}

	public override bool Weaved()
	{
		return true;
	}

	protected void UserCode_RpcInformSpawned__Vector3__Quaternion(Vector3 position, Quaternion rotation)
	{
		initialPosition = position;
		initialRotation = rotation;
		rigidbody.isKinematic = false;
		Teleport(position, rotation, resetState: true);
		if (CameraModuleController.TryGetOrbitModule(out var orbitModule))
		{
			orbitModule.SetYaw(Yaw);
		}
	}

	protected static void InvokeUserCode_RpcInformSpawned__Vector3__Quaternion(NetworkBehaviour obj, NetworkReader reader, NetworkConnectionToClient senderConnection)
	{
		if (!NetworkClient.active)
		{
			Debug.LogError("TargetRPC RpcInformSpawned called on server.");
		}
		else
		{
			((PlayerMovement)obj).UserCode_RpcInformSpawned__Vector3__Quaternion(reader.ReadVector3(), reader.ReadQuaternion());
		}
	}

	protected void UserCode_CmdPlayKnockoutBlockedEffectForAllClients__NetworkConnectionToClient(NetworkConnectionToClient sender)
	{
		if (!serverKnockoutBlockedCommandRateLimiter.RegisterHit())
		{
			return;
		}
		if (sender != null && sender != NetworkServer.localConnection)
		{
			PlayKnockoutBlockedEffectInternal();
		}
		foreach (NetworkConnectionToClient value in NetworkServer.connections.Values)
		{
			if (value != NetworkServer.localConnection && value != sender)
			{
				RpcPlayKnockoutBlockedEffect(value);
			}
		}
	}

	protected static void InvokeUserCode_CmdPlayKnockoutBlockedEffectForAllClients__NetworkConnectionToClient(NetworkBehaviour obj, NetworkReader reader, NetworkConnectionToClient senderConnection)
	{
		if (!NetworkServer.active)
		{
			Debug.LogError("Command CmdPlayKnockoutBlockedEffectForAllClients called on client.");
		}
		else
		{
			((PlayerMovement)obj).UserCode_CmdPlayKnockoutBlockedEffectForAllClients__NetworkConnectionToClient(senderConnection);
		}
	}

	protected void UserCode_RpcPlayKnockoutBlockedEffect__NetworkConnectionToClient(NetworkConnectionToClient connection)
	{
		PlayKnockoutBlockedEffectInternal();
	}

	protected static void InvokeUserCode_RpcPlayKnockoutBlockedEffect__NetworkConnectionToClient(NetworkBehaviour obj, NetworkReader reader, NetworkConnectionToClient senderConnection)
	{
		if (!NetworkClient.active)
		{
			Debug.LogError("TargetRPC RpcPlayKnockoutBlockedEffect called on server.");
		}
		else
		{
			((PlayerMovement)obj).UserCode_RpcPlayKnockoutBlockedEffect__NetworkConnectionToClient(null);
		}
	}

	protected void UserCode_CmdInformKnockedOut__PlayerInfo__KnockoutType__Vector3__Single__ItemType__ItemUseId__Boolean(PlayerInfo responsiblePlayer, KnockoutType knockoutType, Vector3 localOrigin, float distance, ItemType heldItem, ItemUseId itemUseId, bool fromSpecialState)
	{
		InformKnockedOutInternal(responsiblePlayer, knockoutType, localOrigin, distance, heldItem, itemUseId, fromSpecialState);
	}

	protected static void InvokeUserCode_CmdInformKnockedOut__PlayerInfo__KnockoutType__Vector3__Single__ItemType__ItemUseId__Boolean(NetworkBehaviour obj, NetworkReader reader, NetworkConnectionToClient senderConnection)
	{
		if (!NetworkServer.active)
		{
			Debug.LogError("Command CmdInformKnockedOut called on client.");
		}
		else
		{
			((PlayerMovement)obj).UserCode_CmdInformKnockedOut__PlayerInfo__KnockoutType__Vector3__Single__ItemType__ItemUseId__Boolean(reader.ReadNetworkBehaviour<PlayerInfo>(), GeneratedNetworkCode._Read_KnockoutType(reader), reader.ReadVector3(), reader.ReadFloat(), GeneratedNetworkCode._Read_ItemType(reader), GeneratedNetworkCode._Read_ItemUseId(reader), reader.ReadBool());
		}
	}

	protected void UserCode_RpcBeginRespawn__Boolean__RespawnTarget(bool isRestart, RespawnTarget respawnTarget)
	{
		LocalPlayerBeginRespawn(isRestart, respawnTarget);
	}

	protected static void InvokeUserCode_RpcBeginRespawn__Boolean__RespawnTarget(NetworkBehaviour obj, NetworkReader reader, NetworkConnectionToClient senderConnection)
	{
		if (!NetworkClient.active)
		{
			Debug.LogError("TargetRPC RpcBeginRespawn called on server.");
		}
		else
		{
			((PlayerMovement)obj).UserCode_RpcBeginRespawn__Boolean__RespawnTarget(reader.ReadBool(), GeneratedNetworkCode._Read_RespawnTarget(reader));
		}
	}

	protected void UserCode_CmdClientInformRestarted()
	{
		if (serverRestartInformCommandRateLimiter.RegisterHit())
		{
			CourseManager.AddPenaltyStroke(PlayerInfo.AsGolfer, suppressPopup: false);
		}
	}

	protected static void InvokeUserCode_CmdClientInformRestarted(NetworkBehaviour obj, NetworkReader reader, NetworkConnectionToClient senderConnection)
	{
		if (!NetworkServer.active)
		{
			Debug.LogError("Command CmdClientInformRestarted called on client.");
		}
		else
		{
			((PlayerMovement)obj).UserCode_CmdClientInformRestarted();
		}
	}

	protected void UserCode_CmdPlayRespawnEffectsForAllClients__Checkpoint__NetworkConnectionToClient(Checkpoint checkpoint, NetworkConnectionToClient sender)
	{
		if (!serverRespawnEffectCommandRateLimiter.RegisterHit())
		{
			return;
		}
		if (sender != null && sender != NetworkServer.localConnection)
		{
			PlayRespawnEffectsInternal(checkpoint);
		}
		foreach (NetworkConnectionToClient value in NetworkServer.connections.Values)
		{
			if (value != NetworkServer.localConnection && value != sender)
			{
				RpcPlayRespawnEffects(value, checkpoint);
			}
		}
	}

	protected static void InvokeUserCode_CmdPlayRespawnEffectsForAllClients__Checkpoint__NetworkConnectionToClient(NetworkBehaviour obj, NetworkReader reader, NetworkConnectionToClient senderConnection)
	{
		if (!NetworkServer.active)
		{
			Debug.LogError("Command CmdPlayRespawnEffectsForAllClients called on client.");
		}
		else
		{
			((PlayerMovement)obj).UserCode_CmdPlayRespawnEffectsForAllClients__Checkpoint__NetworkConnectionToClient(reader.ReadNetworkBehaviour<Checkpoint>(), senderConnection);
		}
	}

	protected void UserCode_RpcPlayRespawnEffects__NetworkConnectionToClient__Checkpoint(NetworkConnectionToClient connection, Checkpoint checkpoint)
	{
		PlayRespawnEffectsInternal(checkpoint);
	}

	protected static void InvokeUserCode_RpcPlayRespawnEffects__NetworkConnectionToClient__Checkpoint(NetworkBehaviour obj, NetworkReader reader, NetworkConnectionToClient senderConnection)
	{
		if (!NetworkClient.active)
		{
			Debug.LogError("TargetRPC RpcPlayRespawnEffects called on server.");
		}
		else
		{
			((PlayerMovement)obj).UserCode_RpcPlayRespawnEffects__NetworkConnectionToClient__Checkpoint(null, reader.ReadNetworkBehaviour<Checkpoint>());
		}
	}

	protected void UserCode_RpcPlayRestartSound__NetworkConnectionToClient(NetworkConnectionToClient connection)
	{
		PlayRestartSoundInternal();
	}

	protected static void InvokeUserCode_RpcPlayRestartSound__NetworkConnectionToClient(NetworkBehaviour obj, NetworkReader reader, NetworkConnectionToClient senderConnection)
	{
		if (!NetworkClient.active)
		{
			Debug.LogError("TargetRPC RpcPlayRestartSound called on server.");
		}
		else
		{
			((PlayerMovement)obj).UserCode_RpcPlayRestartSound__NetworkConnectionToClient(null);
		}
	}

	protected void UserCode_RpcSetIsForceHidden__Boolean(bool isForcedHidden)
	{
		this.isForcedHidden = isForcedHidden;
		LocalPlayerUpdateVisibility();
	}

	protected static void InvokeUserCode_RpcSetIsForceHidden__Boolean(NetworkBehaviour obj, NetworkReader reader, NetworkConnectionToClient senderConnection)
	{
		if (!NetworkClient.active)
		{
			Debug.LogError("TargetRPC RpcSetIsForceHidden called on server.");
		}
		else
		{
			((PlayerMovement)obj).UserCode_RpcSetIsForceHidden__Boolean(reader.ReadBool());
		}
	}

	protected void UserCode_CmdInformGrounded()
	{
		if (serverInformGroundedCommandRateLimiter.RegisterHit() && base.isServer)
		{
			UpdateOutOfBounds(forceConsiderAsGrounded: true);
		}
	}

	protected static void InvokeUserCode_CmdInformGrounded(NetworkBehaviour obj, NetworkReader reader, NetworkConnectionToClient senderConnection)
	{
		if (!NetworkServer.active)
		{
			Debug.LogError("Command CmdInformGrounded called on client.");
		}
		else
		{
			((PlayerMovement)obj).UserCode_CmdInformGrounded();
		}
	}

	protected void UserCode_CmdPlaySpringBootsLandingForAllClients__Vector3__NetworkConnectionToClient(Vector3 worldPosition, NetworkConnectionToClient sender)
	{
		if (!serverSpringBootsLandingCommandRateLimiter.RegisterHit())
		{
			return;
		}
		if (sender != null && sender != NetworkServer.localConnection)
		{
			PlaySpringBootsLandingInternal(worldPosition);
		}
		foreach (NetworkConnectionToClient value in NetworkServer.connections.Values)
		{
			if (value != NetworkServer.localConnection && value != sender)
			{
				RpcPlaySpringBootsLanding(value, worldPosition);
			}
		}
	}

	protected static void InvokeUserCode_CmdPlaySpringBootsLandingForAllClients__Vector3__NetworkConnectionToClient(NetworkBehaviour obj, NetworkReader reader, NetworkConnectionToClient senderConnection)
	{
		if (!NetworkServer.active)
		{
			Debug.LogError("Command CmdPlaySpringBootsLandingForAllClients called on client.");
		}
		else
		{
			((PlayerMovement)obj).UserCode_CmdPlaySpringBootsLandingForAllClients__Vector3__NetworkConnectionToClient(reader.ReadVector3(), senderConnection);
		}
	}

	protected void UserCode_RpcPlaySpringBootsLanding__NetworkConnectionToClient__Vector3(NetworkConnectionToClient connection, Vector3 worldPosition)
	{
		PlaySpringBootsLandingInternal(worldPosition);
	}

	protected static void InvokeUserCode_RpcPlaySpringBootsLanding__NetworkConnectionToClient__Vector3(NetworkBehaviour obj, NetworkReader reader, NetworkConnectionToClient senderConnection)
	{
		if (!NetworkClient.active)
		{
			Debug.LogError("TargetRPC RpcPlaySpringBootsLanding called on server.");
		}
		else
		{
			((PlayerMovement)obj).UserCode_RpcPlaySpringBootsLanding__NetworkConnectionToClient__Vector3(null, reader.ReadVector3());
		}
	}

	protected void UserCode_RpcSetOutOfBoundsMessageRemainingTime__Single(float remainingTime)
	{
		if (!NoClipEnabled)
		{
			localPlayerOutOfBoundsRemainingTime = remainingTime;
			LocalPlayerUpdateOutOfBoundsMessageRemainingTime();
		}
	}

	protected static void InvokeUserCode_RpcSetOutOfBoundsMessageRemainingTime__Single(NetworkBehaviour obj, NetworkReader reader, NetworkConnectionToClient senderConnection)
	{
		if (!NetworkClient.active)
		{
			Debug.LogError("TargetRPC RpcSetOutOfBoundsMessageRemainingTime called on server.");
		}
		else
		{
			((PlayerMovement)obj).UserCode_RpcSetOutOfBoundsMessageRemainingTime__Single(reader.ReadFloat());
		}
	}

	protected void UserCode_RpcReturnToBounds__NetworkConnectionToClient(NetworkConnectionToClient connection)
	{
		ReturnToBoundsInternal();
	}

	protected static void InvokeUserCode_RpcReturnToBounds__NetworkConnectionToClient(NetworkBehaviour obj, NetworkReader reader, NetworkConnectionToClient senderConnection)
	{
		if (!NetworkClient.active)
		{
			Debug.LogError("TargetRPC RpcReturnToBounds called on server.");
		}
		else
		{
			((PlayerMovement)obj).UserCode_RpcReturnToBounds__NetworkConnectionToClient(null);
		}
	}

	protected void UserCode_CmdInformTeleported__Vector3__Quaternion__NetworkConnectionToClient(Vector3 position, Quaternion rotation, NetworkConnectionToClient sender)
	{
		if (!serverInformTeleportedCommandRateLimiter.RegisterHit())
		{
			return;
		}
		if (sender != null && sender != NetworkServer.localConnection)
		{
			OnRemotePlayerTeleported(position, rotation);
		}
		foreach (NetworkConnectionToClient value in NetworkServer.connections.Values)
		{
			if (value != NetworkServer.localConnection && value != sender)
			{
				RpcInformedTeleported(value, position, rotation);
			}
		}
	}

	protected static void InvokeUserCode_CmdInformTeleported__Vector3__Quaternion__NetworkConnectionToClient(NetworkBehaviour obj, NetworkReader reader, NetworkConnectionToClient senderConnection)
	{
		if (!NetworkServer.active)
		{
			Debug.LogError("Command CmdInformTeleported called on client.");
		}
		else
		{
			((PlayerMovement)obj).UserCode_CmdInformTeleported__Vector3__Quaternion__NetworkConnectionToClient(reader.ReadVector3(), reader.ReadQuaternion(), senderConnection);
		}
	}

	protected void UserCode_RpcInformedTeleported__NetworkConnectionToClient__Vector3__Quaternion(NetworkConnectionToClient connection, Vector3 position, Quaternion rotation)
	{
		OnRemotePlayerTeleported(position, rotation);
	}

	protected static void InvokeUserCode_RpcInformedTeleported__NetworkConnectionToClient__Vector3__Quaternion(NetworkBehaviour obj, NetworkReader reader, NetworkConnectionToClient senderConnection)
	{
		if (!NetworkClient.active)
		{
			Debug.LogError("TargetRPC RpcInformedTeleported called on server.");
		}
		else
		{
			((PlayerMovement)obj).UserCode_RpcInformedTeleported__NetworkConnectionToClient__Vector3__Quaternion(null, reader.ReadVector3(), reader.ReadQuaternion());
		}
	}

	protected void UserCode_RpcInformKnockedOutOtherPlayer__PlayerInfo__Boolean(PlayerInfo knockedOutPlayer, bool addSpeedBoost)
	{
		if (knockedOutPlayer == null || !MatchSetupRules.GetValueAsBool(MatchSetupRules.Rule.KnockoutSpeedBoost))
		{
			return;
		}
		if (knockoutTimePerPlayerGuid.TryGetValue(knockedOutPlayer.PlayerId.Guid, out var value) && BMath.GetTimeSince(value) < 0.5f)
		{
			Debug.LogWarning($"Server is sending too many knockout informs for player {knockedOutPlayer.PlayerId.PlayerName} ({knockedOutPlayer.PlayerId.Guid})");
			return;
		}
		if (addSpeedBoost)
		{
			AddSpeedBoost(GameManager.PlayerMovementSettings.KnockOutSpeedBoostDuration);
		}
		if (!SingletonBehaviour<DrivingRangeManager>.HasInstance)
		{
			CosmeticsUnlocksManager.RewardCredits(5);
		}
		knockoutTimePerPlayerGuid[knockedOutPlayer.PlayerId.Guid] = Time.timeAsDouble;
	}

	protected static void InvokeUserCode_RpcInformKnockedOutOtherPlayer__PlayerInfo__Boolean(NetworkBehaviour obj, NetworkReader reader, NetworkConnectionToClient senderConnection)
	{
		if (!NetworkClient.active)
		{
			Debug.LogError("TargetRPC RpcInformKnockedOutOtherPlayer called on server.");
		}
		else
		{
			((PlayerMovement)obj).UserCode_RpcInformKnockedOutOtherPlayer__PlayerInfo__Boolean(reader.ReadNetworkBehaviour<PlayerInfo>(), reader.ReadBool());
		}
	}

	protected void UserCode_CmdPlaySpeedBoostEffectsForAllClients__NetworkConnectionToClient(NetworkConnectionToClient sender)
	{
		if (!serverSpeedBoostEffectCommandRateLimiter.RegisterHit())
		{
			return;
		}
		if (sender != null && sender != NetworkServer.localConnection)
		{
			PlaySpeedBoostEffectsInternal();
		}
		foreach (NetworkConnectionToClient value in NetworkServer.connections.Values)
		{
			if (value != NetworkServer.localConnection && value != sender)
			{
				RpcPlaySpeedBoostEffects(value);
			}
		}
	}

	protected static void InvokeUserCode_CmdPlaySpeedBoostEffectsForAllClients__NetworkConnectionToClient(NetworkBehaviour obj, NetworkReader reader, NetworkConnectionToClient senderConnection)
	{
		if (!NetworkServer.active)
		{
			Debug.LogError("Command CmdPlaySpeedBoostEffectsForAllClients called on client.");
		}
		else
		{
			((PlayerMovement)obj).UserCode_CmdPlaySpeedBoostEffectsForAllClients__NetworkConnectionToClient(senderConnection);
		}
	}

	protected void UserCode_RpcPlaySpeedBoostEffects__NetworkConnectionToClient(NetworkConnectionToClient connection)
	{
		PlaySpeedBoostEffectsInternal();
	}

	protected static void InvokeUserCode_RpcPlaySpeedBoostEffects__NetworkConnectionToClient(NetworkBehaviour obj, NetworkReader reader, NetworkConnectionToClient senderConnection)
	{
		if (!NetworkClient.active)
		{
			Debug.LogError("TargetRPC RpcPlaySpeedBoostEffects called on server.");
		}
		else
		{
			((PlayerMovement)obj).UserCode_RpcPlaySpeedBoostEffects__NetworkConnectionToClient(null);
		}
	}

	protected void UserCode_CmdPlayOutOfBoundsEliminationExplosionForAllClients__NetworkConnectionToClient(NetworkConnectionToClient sender)
	{
		if (!serverOutOfBoundsEliminationExplosionCommandRateLimiter.RegisterHit())
		{
			return;
		}
		if (sender != null && sender != NetworkServer.localConnection)
		{
			PlayOutOfBoundsEliminationExplosionInternal();
		}
		foreach (NetworkConnectionToClient value in NetworkServer.connections.Values)
		{
			if (value != NetworkServer.localConnection && value != sender)
			{
				RpcPlayOutOfBoundsEliminationExplosion(value);
			}
		}
	}

	protected static void InvokeUserCode_CmdPlayOutOfBoundsEliminationExplosionForAllClients__NetworkConnectionToClient(NetworkBehaviour obj, NetworkReader reader, NetworkConnectionToClient senderConnection)
	{
		if (!NetworkServer.active)
		{
			Debug.LogError("Command CmdPlayOutOfBoundsEliminationExplosionForAllClients called on client.");
		}
		else
		{
			((PlayerMovement)obj).UserCode_CmdPlayOutOfBoundsEliminationExplosionForAllClients__NetworkConnectionToClient(senderConnection);
		}
	}

	protected void UserCode_RpcPlayOutOfBoundsEliminationExplosion__NetworkConnectionToClient(NetworkConnectionToClient connection)
	{
		PlayOutOfBoundsEliminationExplosionInternal();
	}

	protected static void InvokeUserCode_RpcPlayOutOfBoundsEliminationExplosion__NetworkConnectionToClient(NetworkBehaviour obj, NetworkReader reader, NetworkConnectionToClient senderConnection)
	{
		if (!NetworkClient.active)
		{
			Debug.LogError("TargetRPC RpcPlayOutOfBoundsEliminationExplosion called on server.");
		}
		else
		{
			((PlayerMovement)obj).UserCode_RpcPlayOutOfBoundsEliminationExplosion__NetworkConnectionToClient(null);
		}
	}

	protected void UserCode_CmdPlayGolfCartKnockoutEffectsForAllClients__Vector3__NetworkConnectionToClient(Vector3 localPosition, NetworkConnectionToClient sender)
	{
		if (!serverGolfCartKnockoutEffectsCommandRateLimiter.RegisterHit())
		{
			return;
		}
		if (sender != null && sender != NetworkServer.localConnection)
		{
			PlayGolfCartKnockoutEffectsInternal(localPosition);
		}
		foreach (NetworkConnectionToClient value in NetworkServer.connections.Values)
		{
			if (value != NetworkServer.localConnection && value != sender)
			{
				RpcPlayGolfCartKnockoutEffects(value, localPosition);
			}
		}
	}

	protected static void InvokeUserCode_CmdPlayGolfCartKnockoutEffectsForAllClients__Vector3__NetworkConnectionToClient(NetworkBehaviour obj, NetworkReader reader, NetworkConnectionToClient senderConnection)
	{
		if (!NetworkServer.active)
		{
			Debug.LogError("Command CmdPlayGolfCartKnockoutEffectsForAllClients called on client.");
		}
		else
		{
			((PlayerMovement)obj).UserCode_CmdPlayGolfCartKnockoutEffectsForAllClients__Vector3__NetworkConnectionToClient(reader.ReadVector3(), senderConnection);
		}
	}

	protected void UserCode_RpcPlayGolfCartKnockoutEffects__NetworkConnectionToClient__Vector3(NetworkConnectionToClient connection, Vector3 localPosition)
	{
		PlayGolfCartKnockoutEffectsInternal(localPosition);
	}

	protected static void InvokeUserCode_RpcPlayGolfCartKnockoutEffects__NetworkConnectionToClient__Vector3(NetworkBehaviour obj, NetworkReader reader, NetworkConnectionToClient senderConnection)
	{
		if (!NetworkClient.active)
		{
			Debug.LogError("TargetRPC RpcPlayGolfCartKnockoutEffects called on server.");
		}
		else
		{
			((PlayerMovement)obj).UserCode_RpcPlayGolfCartKnockoutEffects__NetworkConnectionToClient__Vector3(null, reader.ReadVector3());
		}
	}

	public override void SerializeSyncVars(NetworkWriter writer, bool forceAll)
	{
		base.SerializeSyncVars(writer, forceAll);
		if (forceAll)
		{
			writer.WriteVector3(syncedVelocity);
			writer.WriteBool(isGrounded);
			GeneratedNetworkCode._Write_GroundTerrainType(writer, groundTerrainType);
			GeneratedNetworkCode._Write_TerrainLayer(writer, groundTerrainDominantGlobalLayer);
			writer.WriteBool(isWadingInWater);
			writer.WriteBool(isVisible);
			GeneratedNetworkCode._Write_KnockoutState(writer, knockoutState);
			GeneratedNetworkCode._Write_PlayerMovement_002FKnockedOutVfxData(writer, knockedOutVfxData);
			GeneratedNetworkCode._Write_PlayerMovement_002FKnockOutImmunity(writer, knockoutImmunityStatus);
			GeneratedNetworkCode._Write_StatusEffect(writer, statusEffects);
			GeneratedNetworkCode._Write_DivingState(writer, divingState);
			GeneratedNetworkCode._Write_DiveType(writer, diveType);
			GeneratedNetworkCode._Write_RespawnState(writer, respawnState);
			return;
		}
		writer.WriteVarULong(syncVarDirtyBits);
		if ((syncVarDirtyBits & 1L) != 0L)
		{
			writer.WriteVector3(syncedVelocity);
		}
		if ((syncVarDirtyBits & 2L) != 0L)
		{
			writer.WriteBool(isGrounded);
		}
		if ((syncVarDirtyBits & 4L) != 0L)
		{
			GeneratedNetworkCode._Write_GroundTerrainType(writer, groundTerrainType);
		}
		if ((syncVarDirtyBits & 8L) != 0L)
		{
			GeneratedNetworkCode._Write_TerrainLayer(writer, groundTerrainDominantGlobalLayer);
		}
		if ((syncVarDirtyBits & 0x10L) != 0L)
		{
			writer.WriteBool(isWadingInWater);
		}
		if ((syncVarDirtyBits & 0x20L) != 0L)
		{
			writer.WriteBool(isVisible);
		}
		if ((syncVarDirtyBits & 0x40L) != 0L)
		{
			GeneratedNetworkCode._Write_KnockoutState(writer, knockoutState);
		}
		if ((syncVarDirtyBits & 0x80L) != 0L)
		{
			GeneratedNetworkCode._Write_PlayerMovement_002FKnockedOutVfxData(writer, knockedOutVfxData);
		}
		if ((syncVarDirtyBits & 0x100L) != 0L)
		{
			GeneratedNetworkCode._Write_PlayerMovement_002FKnockOutImmunity(writer, knockoutImmunityStatus);
		}
		if ((syncVarDirtyBits & 0x200L) != 0L)
		{
			GeneratedNetworkCode._Write_StatusEffect(writer, statusEffects);
		}
		if ((syncVarDirtyBits & 0x400L) != 0L)
		{
			GeneratedNetworkCode._Write_DivingState(writer, divingState);
		}
		if ((syncVarDirtyBits & 0x800L) != 0L)
		{
			GeneratedNetworkCode._Write_DiveType(writer, diveType);
		}
		if ((syncVarDirtyBits & 0x1000L) != 0L)
		{
			GeneratedNetworkCode._Write_RespawnState(writer, respawnState);
		}
	}

	public override void DeserializeSyncVars(NetworkReader reader, bool initialState)
	{
		base.DeserializeSyncVars(reader, initialState);
		if (initialState)
		{
			GeneratedSyncVarDeserialize(ref syncedVelocity, null, reader.ReadVector3());
			GeneratedSyncVarDeserialize(ref isGrounded, _Mirror_SyncVarHookDelegate_isGrounded, reader.ReadBool());
			GeneratedSyncVarDeserialize(ref groundTerrainType, null, GeneratedNetworkCode._Read_GroundTerrainType(reader));
			GeneratedSyncVarDeserialize(ref groundTerrainDominantGlobalLayer, null, GeneratedNetworkCode._Read_TerrainLayer(reader));
			GeneratedSyncVarDeserialize(ref isWadingInWater, _Mirror_SyncVarHookDelegate_isWadingInWater, reader.ReadBool());
			GeneratedSyncVarDeserialize(ref isVisible, _Mirror_SyncVarHookDelegate_isVisible, reader.ReadBool());
			GeneratedSyncVarDeserialize(ref knockoutState, null, GeneratedNetworkCode._Read_KnockoutState(reader));
			GeneratedSyncVarDeserialize(ref knockedOutVfxData, _Mirror_SyncVarHookDelegate_knockedOutVfxData, GeneratedNetworkCode._Read_PlayerMovement_002FKnockedOutVfxData(reader));
			GeneratedSyncVarDeserialize(ref knockoutImmunityStatus, _Mirror_SyncVarHookDelegate_knockoutImmunityStatus, GeneratedNetworkCode._Read_PlayerMovement_002FKnockOutImmunity(reader));
			GeneratedSyncVarDeserialize(ref statusEffects, _Mirror_SyncVarHookDelegate_statusEffects, GeneratedNetworkCode._Read_StatusEffect(reader));
			GeneratedSyncVarDeserialize(ref divingState, _Mirror_SyncVarHookDelegate_divingState, GeneratedNetworkCode._Read_DivingState(reader));
			GeneratedSyncVarDeserialize(ref diveType, _Mirror_SyncVarHookDelegate_diveType, GeneratedNetworkCode._Read_DiveType(reader));
			GeneratedSyncVarDeserialize(ref respawnState, _Mirror_SyncVarHookDelegate_respawnState, GeneratedNetworkCode._Read_RespawnState(reader));
			return;
		}
		long num = (long)reader.ReadVarULong();
		if ((num & 1L) != 0L)
		{
			GeneratedSyncVarDeserialize(ref syncedVelocity, null, reader.ReadVector3());
		}
		if ((num & 2L) != 0L)
		{
			GeneratedSyncVarDeserialize(ref isGrounded, _Mirror_SyncVarHookDelegate_isGrounded, reader.ReadBool());
		}
		if ((num & 4L) != 0L)
		{
			GeneratedSyncVarDeserialize(ref groundTerrainType, null, GeneratedNetworkCode._Read_GroundTerrainType(reader));
		}
		if ((num & 8L) != 0L)
		{
			GeneratedSyncVarDeserialize(ref groundTerrainDominantGlobalLayer, null, GeneratedNetworkCode._Read_TerrainLayer(reader));
		}
		if ((num & 0x10L) != 0L)
		{
			GeneratedSyncVarDeserialize(ref isWadingInWater, _Mirror_SyncVarHookDelegate_isWadingInWater, reader.ReadBool());
		}
		if ((num & 0x20L) != 0L)
		{
			GeneratedSyncVarDeserialize(ref isVisible, _Mirror_SyncVarHookDelegate_isVisible, reader.ReadBool());
		}
		if ((num & 0x40L) != 0L)
		{
			GeneratedSyncVarDeserialize(ref knockoutState, null, GeneratedNetworkCode._Read_KnockoutState(reader));
		}
		if ((num & 0x80L) != 0L)
		{
			GeneratedSyncVarDeserialize(ref knockedOutVfxData, _Mirror_SyncVarHookDelegate_knockedOutVfxData, GeneratedNetworkCode._Read_PlayerMovement_002FKnockedOutVfxData(reader));
		}
		if ((num & 0x100L) != 0L)
		{
			GeneratedSyncVarDeserialize(ref knockoutImmunityStatus, _Mirror_SyncVarHookDelegate_knockoutImmunityStatus, GeneratedNetworkCode._Read_PlayerMovement_002FKnockOutImmunity(reader));
		}
		if ((num & 0x200L) != 0L)
		{
			GeneratedSyncVarDeserialize(ref statusEffects, _Mirror_SyncVarHookDelegate_statusEffects, GeneratedNetworkCode._Read_StatusEffect(reader));
		}
		if ((num & 0x400L) != 0L)
		{
			GeneratedSyncVarDeserialize(ref divingState, _Mirror_SyncVarHookDelegate_divingState, GeneratedNetworkCode._Read_DivingState(reader));
		}
		if ((num & 0x800L) != 0L)
		{
			GeneratedSyncVarDeserialize(ref diveType, _Mirror_SyncVarHookDelegate_diveType, GeneratedNetworkCode._Read_DiveType(reader));
		}
		if ((num & 0x1000L) != 0L)
		{
			GeneratedSyncVarDeserialize(ref respawnState, _Mirror_SyncVarHookDelegate_respawnState, GeneratedNetworkCode._Read_RespawnState(reader));
		}
	}
}
