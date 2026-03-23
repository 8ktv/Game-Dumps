#define DEBUG_DRAW
using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using Brimstone.BallDistanceJobs;
using Brimstone.Geometry;
using Mirror;
using Mirror.RemoteCalls;
using Unity.Collections;
using Unity.Jobs;
using Unity.Mathematics;
using UnityEngine;

public class PlayerGolfer : NetworkBehaviour, IBUpdateCallback, IAnyBUpdateCallback
{
	public struct SwingDistanceEstimation
	{
		public float distance;

		public TerrainLayer layer;

		public OutOfBoundsHazard outOfBoundsHazard;

		public SwingDistanceEstimation(float distance, TerrainLayer layer, OutOfBoundsHazard outOfBoundsHazard)
		{
			this.distance = distance;
			this.layer = layer;
			this.outOfBoundsHazard = outOfBoundsHazard;
		}
	}

	public struct TerrainLayerNormalizedSwingPower
	{
		public TerrainLayer layer;

		public OutOfBoundsHazard outOfBoundsHazard;

		public float startNormalizedPower;

		public float endNormalizedPower;

		public static TerrainLayerNormalizedSwingPower Invalid => new TerrainLayerNormalizedSwingPower
		{
			startNormalizedPower = -1f
		};

		public readonly bool IsInvalid()
		{
			return startNormalizedPower < 0f;
		}
	}

	[StructLayout(LayoutKind.Sequential, Size = 1)]
	public struct TerrainLayerNormalizedSwingPowerComparer : IComparer<TerrainLayerNormalizedSwingPower>
	{
		public int Compare(TerrainLayerNormalizedSwingPower x, TerrainLayerNormalizedSwingPower y)
		{
			if (x.IsInvalid() && y.IsInvalid())
			{
				return 0;
			}
			if (x.IsInvalid())
			{
				return 1;
			}
			if (y.IsInvalid())
			{
				return -1;
			}
			if (x.outOfBoundsHazard >= OutOfBoundsHazard.Water || y.outOfBoundsHazard >= OutOfBoundsHazard.Water)
			{
				return x.outOfBoundsHazard.CompareTo(y.outOfBoundsHazard);
			}
			return y.layer.CompareTo(x.layer);
		}
	}

	public static bool JoinAsSpectator;

	public static readonly Collider[] overlappingSingleColliderBuffer;

	public static readonly Collider[] overlappingColliderBuffer;

	public static readonly HashSet<Hittable> processedHittableBuffer;

	public static readonly HashSet<Entity> processedEntityBuffer;

	public static readonly RaycastHit[] raycastSingleHitBuffer;

	public static readonly RaycastHit[] raycastHitBuffer;

	private const int distanceEstimationSubdivisionCount = 300;

	private const int distanceEstimationsCount = 301;

	private const float distanceEstimationNormalizedInitialSpeedStep = 0.0033333334f;

	private static NativeArray<float> distanceEstimationNormalizedInitialSpeeds;

	private static NativeArray<SwingDistanceEstimation> estimatedDistances;

	private static NativeReference<float> desiredDistanceRequiredNormalizedSpeed;

	private static NativeList<TerrainLayerNormalizedSwingPower> terrainLayerNormalizedSwingPowers;

	private static bool initializedStatics;

	[SyncVar(hook = "OnIsInitializedChanged")]
	private bool isInitialized;

	[SyncVar]
	private GolfTee ownTee;

	[SyncVar(hook = "OnOwnBallChanged")]
	private GolfBall ownBall;

	private PlayerGolfer playerResponsibleForPotentialElimination;

	private EliminationReason potentialEliminationReason;

	private bool potentialEliminationDurationForcedFromKnockdownRecovery;

	private double potentialEliminationResponsibilityTimestamp = double.MinValue;

	[SyncVar(hook = "OnMatchResolutionChanged")]
	private PlayerMatchResolution matchResolution = PlayerMatchResolution.Uninitialized;

	[SerializeField]
	private ClubVfxSettings clubVfxSettings;

	private double swingPowerTimestamp;

	private double swingTimestamp = double.MinValue;

	private TeeingSpot teeingSpot;

	private Coroutine swingRoutine;

	private bool isAwaitingDistanceEstimation;

	private JobHandle distanceEstimationHandle;

	[SyncVar(hook = "OnIsAheadOfBallChanged")]
	private bool isAheadOfBall;

	private bool serverSpectatedEntireHole;

	private PoolableParticleSystem overchargedVfx;

	private AntiCheatRateChecker serverReturnBallCommandRateLimiter;

	private AntiCheatRateChecker serverRestartBallCommandRateLimiter;

	private AntiCheatRateChecker serverFinishHoleCommandRateLimiter;

	private AntiCheatRateChecker serverSetPotentialEliminationReasonCommandRateLimiter;

	private AntiCheatRateChecker serverSwingVfxCommandRateLimiter;

	private AntiCheatRateChecker serverHitOwnBallCommandRateLimiter;

	[CVar("drawLockOnLineOfSightDebug", "", "", false, true)]
	private static bool drawLockOnLineOfSightDebug;

	[CVar("drawGolfSwingDebug", "", "", false, true)]
	private static bool drawGolfSwingDebug;

	[CVar("drawShoveDebug", "", "", false, true)]
	private static bool drawShoveDebug;

	private ButtonPrompt swingStancePrompt;

	private ButtonPrompt cancelSwingPrompt;

	private ButtonPrompt adjustAnglePrompt;

	private ButtonPrompt dropItemPrompt;

	protected NetworkBehaviourSyncVar ___ownTeeNetId;

	protected NetworkBehaviourSyncVar ___ownBallNetId;

	public Action<bool, bool> _Mirror_SyncVarHookDelegate_isInitialized;

	public Action<GolfBall, GolfBall> _Mirror_SyncVarHookDelegate_ownBall;

	public Action<PlayerMatchResolution, PlayerMatchResolution> _Mirror_SyncVarHookDelegate_matchResolution;

	public Action<bool, bool> _Mirror_SyncVarHookDelegate_isAheadOfBall;

	public PlayerInfo PlayerInfo { get; private set; }

	public bool IsInitialized => isInitialized;

	public GolfBall OwnBall => NetworkownBall;

	public EliminationReason LocalPlayerLatestImmediateEliminationReason { get; private set; }

	public Vector3 LocalPlayerLatestEliminationPosition { get; private set; }

	public double LocalPlayerEliminationTimestamp { get; private set; } = double.MinValue;

	public float SwingNormalizedPower { get; private set; }

	public float SwingPitch { get; private set; }

	public double ServerOutOfBoundsTimerEliminationTimestamp { get; private set; } = double.MinValue;

	public bool IsAimingSwing { get; private set; }

	public bool IsChargingSwing { get; private set; }

	public bool IsSwinging { get; private set; }

	public LockOnTarget LockOnTarget { get; private set; }

	public bool IsActiveOnGreen { get; private set; }

	public bool IsAheadOfBall => isAheadOfBall;

	public PlayerMatchResolution MatchResolution => matchResolution;

	public bool IsMatchResolved => matchResolution.IsResolved();

	public ClubVfxSettings ClubVfxSettings => clubVfxSettings;

	public bool NetworkisInitialized
	{
		get
		{
			return isInitialized;
		}
		[param: In]
		set
		{
			GeneratedSyncVarSetter(value, ref isInitialized, 1uL, _Mirror_SyncVarHookDelegate_isInitialized);
		}
	}

	public GolfTee NetworkownTee
	{
		get
		{
			return GetSyncVarNetworkBehaviour(___ownTeeNetId, ref ownTee);
		}
		[param: In]
		set
		{
			GeneratedSyncVarSetter_NetworkBehaviour(value, ref ownTee, 2uL, null, ref ___ownTeeNetId);
		}
	}

	public GolfBall NetworkownBall
	{
		get
		{
			return GetSyncVarNetworkBehaviour(___ownBallNetId, ref ownBall);
		}
		[param: In]
		set
		{
			GeneratedSyncVarSetter_NetworkBehaviour(value, ref ownBall, 4uL, _Mirror_SyncVarHookDelegate_ownBall, ref ___ownBallNetId);
		}
	}

	public PlayerMatchResolution NetworkmatchResolution
	{
		get
		{
			return matchResolution;
		}
		[param: In]
		set
		{
			GeneratedSyncVarSetter(value, ref matchResolution, 8uL, _Mirror_SyncVarHookDelegate_matchResolution);
		}
	}

	public bool NetworkisAheadOfBall
	{
		get
		{
			return isAheadOfBall;
		}
		[param: In]
		set
		{
			GeneratedSyncVarSetter(value, ref isAheadOfBall, 16uL, _Mirror_SyncVarHookDelegate_isAheadOfBall);
		}
	}

	public event Action Initialized;

	public event Action<PlayerMatchResolution, PlayerMatchResolution> MatchResolutionChanged;

	public static event Action LocalPlayerOwnBallChanged;

	public static event Action LocalPlayerIsAimingSwingChanged;

	public static event Action LocalPlayerStartedChargingSwing;

	public static event Action LocalPlayerStoppedChargingSwing;

	public static event Action<PlayerMatchResolution, PlayerMatchResolution> LocalPlayerMatchResolutionChanged;

	public static event Action<PlayerGolfer> PlayerHitOwnBall;

	public static event Action<PlayerGolfer, PlayerMatchResolution, PlayerMatchResolution> AnyPlayerMatchResolutionChanged;

	public static event Action<PlayerGolfer> AnyPlayerEliminated;

	private void ReturnButtonPrompts()
	{
		if (swingStancePrompt != null)
		{
			ButtonPromptManager.ReturnButtonPrompt(swingStancePrompt);
		}
		if (cancelSwingPrompt != null)
		{
			ButtonPromptManager.ReturnButtonPrompt(cancelSwingPrompt);
		}
		if (adjustAnglePrompt != null)
		{
			ButtonPromptManager.ReturnButtonPrompt(adjustAnglePrompt);
		}
		if (dropItemPrompt != null)
		{
			ButtonPromptManager.ReturnButtonPrompt(dropItemPrompt);
		}
		swingStancePrompt = null;
		cancelSwingPrompt = null;
		adjustAnglePrompt = null;
		dropItemPrompt = null;
	}

	[CCommand("returnBallToPlayer", "", false, false)]
	private static void ReturnBallToPlayer()
	{
		if (!(GameManager.LocalPlayerAsGolfer == null))
		{
			GameManager.LocalPlayerAsGolfer.CmdReturnBallToPlayerFromConsole();
		}
	}

	[CCommand("finishHole", "", false, false)]
	private static void FinishHole()
	{
		if (!(GameManager.LocalPlayerAsGolfer == null) && MatchSetupRules.IsCheatsEnabled())
		{
			GameManager.LocalPlayerAsGolfer.CmdFinishHole();
		}
	}

	[Command]
	public void CmdReturnBallToPlayerFromConsole()
	{
		if (base.isServer && base.isClient)
		{
			UserCode_CmdReturnBallToPlayerFromConsole();
			return;
		}
		NetworkWriterPooled writer = NetworkWriterPool.Get();
		SendCommandInternal("System.Void PlayerGolfer::CmdReturnBallToPlayerFromConsole()", -2133853855, writer, 0);
		NetworkWriterPool.Return(writer);
	}

	[Command]
	public void CmdRestartBall()
	{
		if (base.isServer && base.isClient)
		{
			UserCode_CmdRestartBall();
			return;
		}
		NetworkWriterPooled writer = NetworkWriterPool.Get();
		SendCommandInternal("System.Void PlayerGolfer::CmdRestartBall()", -1185597277, writer, 0);
		NetworkWriterPool.Return(writer);
	}

	[Server]
	private void ServerReturnBallToPlayer(bool isRestart)
	{
		if (!NetworkServer.active)
		{
			Debug.LogWarning("[Server] function 'System.Void PlayerGolfer::ServerReturnBallToPlayer(System.Boolean)' called when server was not active");
			return;
		}
		Vector3 position = base.transform.position + Vector3.up * 0.25f + base.transform.right * GameManager.GolfSettings.SwingHitBoxLocalCenter.x;
		NetworkownBall.AsEntity.InformWillTeleport();
		NetworkownBall.transform.SetPositionAndRotation(position, base.transform.rotation);
		NetworkownBall.AsEntity.Rigidbody.position = position;
		NetworkownBall.AsEntity.Rigidbody.rotation = base.transform.rotation;
		NetworkownBall.Rigidbody.linearVelocity = Vector3.zero;
		NetworkownBall.Rigidbody.angularVelocity = Vector3.zero;
		NetworkownBall.AsEntity.InformTeleported();
		NetworkownBall.ServerInformNoLongerInHole();
		NetworkownBall.OnRespawned();
		if (isRestart)
		{
			CourseManager.AddPenaltyStroke(this, suppressPopup: false);
		}
	}

	[Command]
	private void CmdFinishHole()
	{
		if (base.isServer && base.isClient)
		{
			UserCode_CmdFinishHole();
			return;
		}
		NetworkWriterPooled writer = NetworkWriterPool.Get();
		SendCommandInternal("System.Void PlayerGolfer::CmdFinishHole()", 1012668652, writer, 0);
		NetworkWriterPool.Return(writer);
	}

	public static void InitializeStatics()
	{
		if (initializedStatics)
		{
			return;
		}
		if (!distanceEstimationNormalizedInitialSpeeds.IsCreated)
		{
			distanceEstimationNormalizedInitialSpeeds = new NativeArray<float>(301, Allocator.Persistent, NativeArrayOptions.UninitializedMemory);
			estimatedDistances = new NativeArray<SwingDistanceEstimation>(301, Allocator.Persistent, NativeArrayOptions.UninitializedMemory);
			desiredDistanceRequiredNormalizedSpeed = new NativeReference<float>(Allocator.Persistent, NativeArrayOptions.UninitializedMemory);
			terrainLayerNormalizedSwingPowers = new NativeList<TerrainLayerNormalizedSwingPower>(301, Allocator.Persistent);
			for (int i = 0; i < 301; i++)
			{
				distanceEstimationNormalizedInitialSpeeds[i] = (float)i * 0.0033333334f;
			}
		}
		initializedStatics = true;
	}

	private void Awake()
	{
		PlayerInfo = GetComponent<PlayerInfo>();
	}

	public void OnWillBeDestroyed()
	{
		ClearOverchargedVfx();
		distanceEstimationHandle.Complete();
		SetLockOnTarget(null);
		ReturnButtonPrompts();
		AheadOfBallMessage.Hide();
		if (GameManager.IsApplicationQuitting && distanceEstimationNormalizedInitialSpeeds.IsCreated)
		{
			distanceEstimationNormalizedInitialSpeeds.Dispose();
			estimatedDistances.Dispose();
			desiredDistanceRequiredNormalizedSpeed.Dispose();
			terrainLayerNormalizedSwingPowers.Dispose();
		}
	}

	public override void OnStartServer()
	{
		CourseManager.RegisterMatchParticipant(this);
		if (!base.isLocalPlayer)
		{
			BUpdate.RegisterCallback(this);
		}
		serverReturnBallCommandRateLimiter = new AntiCheatRateChecker("Return ball to player", base.connectionToClient.connectionId, 0.1f, 3, 10, 1f);
		serverRestartBallCommandRateLimiter = new AntiCheatRateChecker("Restart ball", base.connectionToClient.connectionId, 3f, 3, 10, 6f);
		serverFinishHoleCommandRateLimiter = new AntiCheatRateChecker("Finish hole", base.connectionToClient.connectionId, 0.1f, 3, 6, 1f);
		serverSetPotentialEliminationReasonCommandRateLimiter = new AntiCheatRateChecker("Set potential elimination reason", base.connectionToClient.connectionId, 0.025f, 15, 50, 1f, 10);
		serverSwingVfxCommandRateLimiter = new AntiCheatRateChecker("Swing VFX", base.connectionToClient.connectionId, 0.5f, 5, 10, 2f);
		serverHitOwnBallCommandRateLimiter = new AntiCheatRateChecker("Hit own ball", base.connectionToClient.connectionId, 0.5f, 5, 10, 2f);
		PlayerInfo.AsHittable.WasHitByItem += OnServerWasHitByItem;
		PlayerInfo.AsHittable.WasHitByRocketLauncherBackBlast += OnServerWasHitByRocketLauncherBackBlast;
		PlayerInfo.AsHittable.WasHitByDive += OnServerWasHitByDive;
		PlayerInfo.LevelBoundsTracker.AuthoritativeIsOnGreenChanged += OnServerIsOnGreenChanged;
		PlayerInfo.LevelBoundsTracker.AuthoritativeBoundsStateChanged += OnServerBoundsStateChanged;
		CourseManager.MatchStateChanged += OnServerMatchStateChanged;
	}

	public override void OnStopServer()
	{
		CourseManager.DeregisterMatchParticipant(this);
		if (IsActiveOnGreen)
		{
			CourseManager.DeregisterActivePlayerOnGreen(this);
		}
		if (!base.isLocalPlayer)
		{
			BUpdate.DeregisterCallback(this);
		}
		CourseManager.MatchStateChanged -= OnServerMatchStateChanged;
		if (!BNetworkManager.IsChangingSceneOrShuttingDown)
		{
			if (teeingSpot != null)
			{
				teeingSpot.ReleaseBy(base.connectionToClient.connectionId);
			}
			if (NetworkownTee != null)
			{
				NetworkServer.Destroy(NetworkownTee.gameObject);
			}
			if (NetworkownBall != null)
			{
				NetworkServer.Destroy(NetworkownBall.gameObject);
			}
			PlayerInfo.AsHittable.WasHitByItem -= OnServerWasHitByItem;
			PlayerInfo.AsHittable.WasHitByRocketLauncherBackBlast -= OnServerWasHitByRocketLauncherBackBlast;
			PlayerInfo.AsHittable.WasHitByDive -= OnServerWasHitByDive;
			PlayerInfo.LevelBoundsTracker.AuthoritativeIsOnGreenChanged -= OnServerIsOnGreenChanged;
			PlayerInfo.LevelBoundsTracker.AuthoritativeBoundsStateChanged -= OnServerBoundsStateChanged;
		}
	}

	public override void OnStartLocalPlayer()
	{
		SetPitchInternal(GameManager.GolfSettings.InitialSwingPitch, suppressHotkeySelection: false);
		BUpdate.RegisterCallback(this);
		PlayerInfo.Movement.IsGroundedChanged += OnLocalPlayerIsGroundedChanged;
		PlayerInfo.Movement.IsKnockedOutOrRecoveringChanged += OnLocalPlayerIsKnockedOutChanged;
		PlayerInfo.Inventory.EquippedItemChanged += OnLocalPlayerEquippedItemChanged;
		InputManager.SwitchedInputDeviceType += UpdateAdjustAnglePrompt;
		if (JoinAsSpectator)
		{
			JoinAsSpectator = false;
			MatchSetupMenu.SetLocalPlayerSpectator();
		}
	}

	public override void OnStopLocalPlayer()
	{
		BUpdate.DeregisterCallback(this);
		InputManager.SwitchedInputDeviceType -= UpdateAdjustAnglePrompt;
		if (!BNetworkManager.IsChangingSceneOrShuttingDown)
		{
			PlayerInfo.Movement.IsGroundedChanged -= OnLocalPlayerIsGroundedChanged;
			PlayerInfo.Movement.IsKnockedOutOrRecoveringChanged -= OnLocalPlayerIsKnockedOutChanged;
			PlayerInfo.Inventory.EquippedItemChanged -= OnLocalPlayerEquippedItemChanged;
		}
	}

	public void OnBUpdate()
	{
		if (base.isServer)
		{
			ServerUpdatePotentialEliminationReason();
			NetworkisAheadOfBall = IsAheadOfBall();
		}
		if (base.isLocalPlayer)
		{
			bool isAimingAtProjectile = false;
			if (IsAimingSwing)
			{
				UpdateSwingTrajectoryPreview(out isAimingAtProjectile);
				UpdateOverchargedVfx();
			}
			if (IsChargingSwing)
			{
				UpdateSwingNormalizedPower(forSwingRelease: false, isForcedFumble: false);
				UpdateLockOn(isAimingAtProjectile);
			}
			UpdateSwingStanceButtonPrompt();
		}
		bool IsAheadOfBall()
		{
			if (SingletonBehaviour<DrivingRangeManager>.HasInstance)
			{
				return false;
			}
			if (NetworkownBall == null)
			{
				return false;
			}
			if (IsMatchResolved)
			{
				return false;
			}
			if (!PlayerInfo.Movement.IsVisible)
			{
				return false;
			}
			if (PlayerInfo.AsSpectator.IsSpectating)
			{
				return false;
			}
			if (GolfHoleManager.MainHole == null)
			{
				return false;
			}
			float magnitude = (NetworkownBall.transform.position - GolfHoleManager.MainHole.transform.position).magnitude;
			float magnitude2 = (base.transform.position - GolfHoleManager.MainHole.transform.position).magnitude;
			return magnitude - magnitude2 > GameManager.GolfSettings.AheadOfBallDistance;
		}
		void ServerUpdatePotentialEliminationReason()
		{
			if (potentialEliminationReason != EliminationReason.None)
			{
				EliminationSettings eliminationSettings = GameManager.MatchSettings.GetEliminationSettings(potentialEliminationReason);
				if (potentialEliminationResponsibilityTimestamp < double.MaxValue)
				{
					if (BMath.GetTimeSince(potentialEliminationResponsibilityTimestamp) > eliminationSettings.Duration)
					{
						ServerClearPotentialEliminationResponsibility();
					}
				}
				else if ((potentialEliminationDurationForcedFromKnockdownRecovery || eliminationSettings.DurationType == EliminationDurationType.KnockoutRecovery) && !PlayerInfo.Movement.IsKnockedOutOrRecovering)
				{
					potentialEliminationResponsibilityTimestamp = Time.timeAsDouble;
				}
				else if (eliminationSettings.DurationType == EliminationDurationType.Grounded && PlayerInfo.Movement.IsGrounded)
				{
					potentialEliminationResponsibilityTimestamp = Time.timeAsDouble;
				}
			}
		}
		void UpdateHoleDistanceEstimationForOwnBall()
		{
			if (NetworkownBall == null)
			{
				SwingPowerBarUi.HideFlagIcon();
				SwingPowerBarUi.HideTerrainLayers();
			}
			else
			{
				float desiredDistance = -1f;
				float num = 0f;
				if (GolfHoleManager.MainHole != null)
				{
					Vector3 vector = GolfHoleManager.MainHole.transform.position - NetworkownBall.transform.position;
					float yawDeg = vector.GetYawDeg();
					float yawDeg2 = base.transform.forward.GetYawDeg();
					num = (yawDeg - yawDeg2).WrapAngleDeg();
					if (BMath.Abs(num) <= GameManager.UiSettings.SwingPowerBarFlagPreviewMaxYaw)
					{
						desiredDistance = vector.Horizontalized().magnitude;
					}
					else
					{
						SwingPowerBarUi.HideFlagIcon();
					}
				}
				if (isAwaitingDistanceEstimation)
				{
					distanceEstimationHandle.Complete();
					if (desiredDistanceRequiredNormalizedSpeed.Value >= 0f)
					{
						SwingPowerBarUi.SetFlagIcon(num, desiredDistanceRequiredNormalizedSpeed.Value);
					}
					else
					{
						SwingPowerBarUi.HideFlagIcon();
					}
					SwingPowerBarUi.SetTerrainLayers(terrainLayerNormalizedSwingPowers);
				}
				else
				{
					SwingPowerBarUi.HideFlagIcon();
					SwingPowerBarUi.HideTerrainLayers();
				}
				float2 initialWorldPosition2d = NetworkownBall.transform.position.AsHorizontal2();
				JobHandle dependsOn = ((!(SwingPitch > 0f)) ? IJobParallelForExtensions.Schedule(new CalculateGroundRollStopDistancesJob
				{
					normalizedInitialSpeeds = distanceEstimationNormalizedInitialSpeeds,
					globalTerrainLayerIndicesPerLevelTerrainLayer = TerrainManager.GlobalTerrainLayerIndicesPerLevelTerrainLayer,
					ballTerrainPhysicsSettingsPerTerrainLayer = TerrainSettings.JobsBallTerrainCollisionMaterialPerTerrainLayer,
					spatiallyHashedTerrains = TerrainManager.SpatiallyHashedJobsTerrains,
					allTerrainLayerWeights = TerrainManager.AllTerrainLayerWeights,
					allTerrainHeights = TerrainManager.AllTerrainHeights,
					secondaryOutOfBoundsHazards = BoundsManager.SecondaryOutOfBoundsHazardInstances,
					estimatedDistances = estimatedDistances,
					mainOutOfBoundsHazardHeight = MainOutOfBoundsHazard.Height,
					mainOutOfBoundsHazardType = MainOutOfBoundsHazard.Type,
					fullInitialSpeed = NetworkownBall.AsEntity.AsHittable.SwingSettings.MaxPowerPuttHitSpeed * MatchSetupRules.GetValue(MatchSetupRules.Rule.SwingPower) * base.transform.forward.AsHorizontal2(),
					movementDirectionRightInitialAngularSpeed = 0f - NetworkownBall.AsEntity.AsHittable.SwingSettings.SwingHitSpinSpeed,
					initialWorldPosition2d = initialWorldPosition2d,
					terrainSize = TerrainManager.TerrainSize,
					absoluteVerticalGravity = 0f - Physics.gravity.y,
					ballMass = OwnBall.Rigidbody.mass,
					ballRadius = OwnBall.Collider.radius,
					ballMaxAngularSpeed = OwnBall.Rigidbody.maxAngularVelocity,
					ballFullStopMinDampingSpeed = GameManager.GolfBallSettings.FullStopMinDampingSpeed,
					ballFullStopMaxDampingSpeed = GameManager.GolfBallSettings.FullStopMaxDampingSpeed,
					ballFullStopLinearDamping = GameManager.GolfBallSettings.FullStopLinearDamping,
					deltaTime = Time.fixedDeltaTime
				}, 301, 1) : IJobParallelForExtensions.Schedule(new CalculateFirstGroundHitDistancesJob
				{
					normalizedInitialSpeeds = distanceEstimationNormalizedInitialSpeeds,
					spatiallyHashedTerrains = TerrainManager.SpatiallyHashedJobsTerrains,
					globalTerrainLayerIndicesPerLevelTerrainLayer = TerrainManager.GlobalTerrainLayerIndicesPerLevelTerrainLayer,
					allTerrainLayerWeights = TerrainManager.AllTerrainLayerWeights,
					allTerrainHeights = TerrainManager.AllTerrainHeights,
					secondaryOutOfBoundsHazards = BoundsManager.SecondaryOutOfBoundsHazardInstances,
					estimatedDistances = estimatedDistances,
					mainOutOfBoundsHazardHeight = MainOutOfBoundsHazard.Height,
					mainOutOfBoundsHazardType = MainOutOfBoundsHazard.Type,
					terrainSize = TerrainManager.TerrainSize,
					initialWorldPosition2d = initialWorldPosition2d,
					yawRad = base.transform.forward.GetYawRad(),
					fullInitialSpeed = NetworkownBall.AsEntity.AsHittable.SwingSettings.MaxPowerSwingHitSpeed * MatchSetupRules.GetValue(MatchSetupRules.Rule.SwingPower),
					verticalGravity = Physics.gravity.y,
					pitchRad = SwingPitch * (MathF.PI / 180f),
					airDragCoefficient = GameManager.GolfBallSettings.LinearAirDragFactor,
					deltaTime = Time.fixedDeltaTime
				}, 301, 1));
				ProcessDistanceEstimationsJob jobData = new ProcessDistanceEstimationsJob
				{
					normalizedInitialSpeeds = distanceEstimationNormalizedInitialSpeeds,
					estimatedDistances = estimatedDistances,
					requiredNormalizedSpeed = desiredDistanceRequiredNormalizedSpeed,
					terrainLayerNormalizedSwingPowers = terrainLayerNormalizedSwingPowers,
					desiredDistance = desiredDistance
				};
				distanceEstimationHandle = jobData.Schedule(dependsOn);
				isAwaitingDistanceEstimation = true;
			}
		}
		void UpdateLockOn(bool isAimingSwingAtProjectile)
		{
			if (!isAimingSwingAtProjectile || SwingNormalizedPower <= 1f || SwingPitch <= 0f)
			{
				SetLockOnTarget(null);
			}
			else
			{
				float value = MatchSetupRules.GetValue(MatchSetupRules.Rule.SwingPower);
				if (TryGetBestLockOnTarget(GameManager.GolfSettings.LockOnMaxDistanceSquared * value * value, GameManager.GolfSettings.LockOnMaxYawFromCenterScreen, GameManager.GolfSettings.LockOnYawWeight, out var bestLockOnTarget))
				{
					SetLockOnTarget(bestLockOnTarget);
				}
				else
				{
					SetLockOnTarget(null);
				}
			}
		}
		void UpdateSwingTrajectoryPreview(out bool reference)
		{
			reference = false;
			Box swingHitBox = GetSwingHitBox();
			int num = Physics.OverlapBoxNonAlloc(swingHitBox.center, swingHitBox.HalfSize, orientation: base.transform.rotation, mask: GameManager.LayerSettings.SwingHittableMask, results: overlappingColliderBuffer, queryTriggerInteraction: QueryTriggerInteraction.Ignore);
			bool flag = false;
			Hittable hittable = null;
			int num2 = int.MinValue;
			float num3 = float.MaxValue;
			for (int i = 0; i < num; i++)
			{
				Collider collider = overlappingColliderBuffer[i];
				if (CanHitCollider(collider, out var hittable2))
				{
					if (!flag && NetworkownBall != null && hittable2 == NetworkownBall.AsEntity.AsHittable)
					{
						flag = true;
					}
					if (hittable2.SwingSettings.CanBecomeSwingProjectile)
					{
						reference = true;
					}
					int swingHittablePriority = GetSwingHittablePriority(hittable2);
					if (swingHittablePriority >= num2)
					{
						float sqrMagnitude = (hittable2.transform.position - swingHitBox.center).sqrMagnitude;
						if (swingHittablePriority != num2 || !(sqrMagnitude > num3))
						{
							hittable = hittable2;
							num2 = swingHittablePriority;
							num3 = sqrMagnitude;
						}
					}
				}
			}
			Vector3 worldOrigin;
			if (hittable != null)
			{
				worldOrigin = hittable.transform.position;
			}
			else
			{
				worldOrigin = swingHitBox.center;
				worldOrigin.y = base.transform.position.y;
				SwingPowerBarUi.HideFlagIcon();
				SwingPowerBarUi.HideTerrainLayers();
			}
			if (flag)
			{
				UpdateHoleDistanceEstimationForOwnBall();
			}
			else
			{
				SwingPowerBarUi.HideFlagIcon();
				SwingPowerBarUi.HideTerrainLayers();
			}
			SwingTrajectoryPreview.SetData(worldOrigin, GetSwingDirection(SwingPitch));
		}
	}

	[Server]
	public void ServerInitializeAsParticipant(TeeingSpot teeingSpot)
	{
		if (!NetworkServer.active)
		{
			Debug.LogWarning("[Server] function 'System.Void PlayerGolfer::ServerInitializeAsParticipant(TeeingSpot)' called when server was not active");
			return;
		}
		NetworkisInitialized = true;
		this.teeingSpot = teeingSpot;
		if (teeingSpot != null)
		{
			teeingSpot.ServerEnsureSpawned();
			teeingSpot.ClaimFor(base.connectionToClient.connectionId);
			if (teeingSpot.tee != null)
			{
				ClaimTee(teeingSpot.tee);
				SpawnBallOnTee();
			}
		}
		ServerTrySetMatchResolution(PlayerMatchResolution.None);
		void ClaimTee(GolfTee tee)
		{
			NetworkownTee = tee;
			tee.SetOwner(this);
		}
		void SpawnBallOnTee()
		{
			if (NetworkownTee == null)
			{
				Debug.LogError(base.name + " attempted to spawn a ball without a tee", base.gameObject);
			}
			else
			{
				ServerSpawnBall(NetworkownTee.GetBallWorldSpawnPosition(), Quaternion.identity);
			}
		}
	}

	[Server]
	public void ServerInitializeAsSpectator(bool fromHoleStart)
	{
		if (!NetworkServer.active)
		{
			Debug.LogWarning("[Server] function 'System.Void PlayerGolfer::ServerInitializeAsSpectator(System.Boolean)' called when server was not active");
			return;
		}
		NetworkisInitialized = true;
		if (ServerTrySetMatchResolution(PlayerMatchResolution.JoinedAsSpectator) && fromHoleStart)
		{
			serverSpectatedEntireHole = true;
		}
	}

	[Server]
	public GolfBall ServerSpawnBall(Vector3 position, Quaternion rotation)
	{
		if (!NetworkServer.active)
		{
			Debug.LogWarning("[Server] function 'GolfBall PlayerGolfer::ServerSpawnBall(UnityEngine.Vector3,UnityEngine.Quaternion)' called when server was not active");
			return null;
		}
		GameObject gameObject = UnityEngine.Object.Instantiate(GameManager.GolfSettings.BallPrefab, position, rotation);
		if (gameObject == null || !gameObject.TryGetComponent<GolfBall>(out var component))
		{
			Debug.LogError(base.name + "'s ball did not instantiate properly", (gameObject == null) ? base.gameObject : gameObject);
			return null;
		}
		NetworkServer.Spawn(gameObject);
		NetworkownBall = component;
		component.Initialize(this);
		return component;
	}

	public void InformIsSpectatingChanged()
	{
		if (!base.isServer && PlayerInfo.AsSpectator.IsSpectating)
		{
			serverSpectatedEntireHole = false;
		}
	}

	public void InformEnteredGolfCart()
	{
		if (base.isLocalPlayer && PlayerInfo.ActiveGolfCartSeat.IsValid())
		{
			CancelAllActions();
		}
	}

	public bool TryReactToBlownAirhorn()
	{
		if (!base.isLocalPlayer)
		{
			Debug.LogError("Only the local player is allowed to react to an airhorn", base.gameObject);
			return false;
		}
		if (IsAimingSwing && !IsChargingSwing && !IsSwinging)
		{
			TryStartChargingSwing();
		}
		if (!IsChargingSwing)
		{
			return false;
		}
		ReleaseSwingChargeInternal(isForcedFumble: true);
		return true;
	}

	public void InformScored(GolfHole hole)
	{
		if (base.isLocalPlayer)
		{
			TutorialManager.CompleteObjective(TutorialObjective.FinishHole);
			TutorialManager.AllowPromptCategory(TutorialPromptCategory.Scoreboard);
			if (!SingletonBehaviour<DrivingRangeManager>.HasInstance)
			{
				GameManager.AchievementsManager.IncrementProgress(AchievementId.GrizzledVeteran, 1);
				if (CourseManager.CountActivePlayers() > 1)
				{
					if (CourseManager.GetCurrentHolePar() >= GameManager.Achievements.LivingOnTheEdgeMinPar && !CheckpointManager.TryGetLocalPlayerActiveCheckpoint(out var _))
					{
						GameManager.AchievementsManager.Unlock(AchievementId.LivingOnTheEdge);
					}
					if (CourseManager.TryGetPlayerState(PlayerInfo, out var state) && state.matchKnockedOut >= GameManager.Achievements.NeverGiveUpMinMatchKnockouts)
					{
						GameManager.AchievementsManager.Unlock(AchievementId.NeverGiveUp);
					}
					if (CourseManager.MatchState == MatchState.Overtime)
					{
						GameManager.AchievementsManager.Unlock(AchievementId.SnailsPace);
					}
				}
			}
		}
		if (base.isServer)
		{
			ServerTrySetMatchResolution(PlayerMatchResolution.Scored);
			CourseManager.InformPlayerScored(this, hole);
		}
	}

	public void InformCourseStateChanged(CourseManager.PlayerState previousState, CourseManager.PlayerState currentState)
	{
		if (base.isServer && currentState.isSpectator)
		{
			if (NetworkownTee != null)
			{
				NetworkServer.Destroy(NetworkownTee.gameObject);
				NetworkownTee = null;
			}
			if (NetworkownBall != null)
			{
				NetworkServer.Destroy(NetworkownBall.gameObject);
				NetworkownBall = null;
			}
		}
	}

	public void InformLocalPlayerIsHoldingAimChanged()
	{
		UpdateIsAimingSwing();
	}

	public void InformLocalPlayerIsRespawningChanged()
	{
		UpdateIsAimingSwing();
	}

	public void InformLocalPlayerKnockedOut(PlayerGolfer responsiblePlayer, KnockoutType knockoutType)
	{
		CmdSetPotentialEliminationReason(responsiblePlayer, knockoutType switch
		{
			KnockoutType.Swing => EliminationReason.Swing, 
			KnockoutType.SwingProjectile => EliminationReason.SwingProjectile, 
			KnockoutType.Fall => EliminationReason.Fall, 
			KnockoutType.ReturnedBall => EliminationReason.ReturnedBall, 
			KnockoutType.DuelingPistol => EliminationReason.DuelingPistol, 
			_ => EliminationReason.None, 
		});
	}

	public void InformLocalPlayerStartedDiving()
	{
		if (PlayerInfo.Movement.DivingState != DivingState.None)
		{
			CancelAllActions();
		}
	}

	[Server]
	public void ServerEliminate(EliminationReason immediateEliminationReason)
	{
		if (!NetworkServer.active)
		{
			Debug.LogWarning("[Server] function 'System.Void PlayerGolfer::ServerEliminate(EliminationReason)' called when server was not active");
			return;
		}
		if (base.isLocalPlayer)
		{
			OnLocalPlayerWillBeEliminated(immediateEliminationReason, base.transform.position);
		}
		else
		{
			RpcInformWillBeEliminated(immediateEliminationReason, base.transform.position);
		}
		if (immediateEliminationReason == EliminationReason.OutOfBounds || immediateEliminationReason == EliminationReason.FellIntoWater || immediateEliminationReason == EliminationReason.FellIntoFog)
		{
			ServerOutOfBoundsTimerEliminationTimestamp = Time.timeAsDouble;
		}
		if (CanRespawn())
		{
			if (PlayerInfo.Movement.TryBeginRespawn(isRestart: false))
			{
				ReportElimination();
			}
		}
		else if (!ServerTrySetMatchResolution(PlayerMatchResolution.Eliminated))
		{
			if (IsMatchResolved)
			{
				PlayerInfo.Movement.RpcSetIsForceHidden(isForcedHidden: true);
			}
		}
		else
		{
			ReportElimination();
		}
		bool CanRespawn()
		{
			if (IsMatchResolved)
			{
				return false;
			}
			if (immediateEliminationReason == EliminationReason.TimedOut)
			{
				return false;
			}
			if (immediateEliminationReason == EliminationReason.OutOfBounds && CourseManager.MatchState >= MatchState.Overtime)
			{
				return false;
			}
			return true;
		}
		void ReportElimination()
		{
			PlayerGolfer responsiblePlayer;
			EliminationReason eliminationReason = GetEliminationReason(immediateEliminationReason, out responsiblePlayer);
			CourseManager.InformPlayerEliminated(this, responsiblePlayer, eliminationReason, immediateEliminationReason);
		}
	}

	[TargetRpc]
	private void RpcInformWillBeEliminated(EliminationReason immediateEliminationReason, Vector3 eliminationPosition)
	{
		NetworkWriterPooled writer = NetworkWriterPool.Get();
		GeneratedNetworkCode._Write_EliminationReason(writer, immediateEliminationReason);
		writer.WriteVector3(eliminationPosition);
		SendTargetRPCInternal(null, "System.Void PlayerGolfer::RpcInformWillBeEliminated(EliminationReason,UnityEngine.Vector3)", -1943518104, writer, 0);
		NetworkWriterPool.Return(writer);
	}

	private void OnLocalPlayerWillBeEliminated(EliminationReason immediateEliminationReason, Vector3 eliminationPosition)
	{
		LocalPlayerLatestImmediateEliminationReason = immediateEliminationReason;
		LocalPlayerLatestEliminationPosition = eliminationPosition;
		LocalPlayerEliminationTimestamp = Time.timeAsDouble;
		PlayerInfo.ExitGolfCart(GolfCartExitType.Default);
		PlayerInfo.Movement.InformWillBeEliminated();
	}

	private EliminationReason GetEliminationReason(EliminationReason immediateEliminationReason, out PlayerGolfer responsiblePlayer)
	{
		if (matchResolution.IsResolved() || potentialEliminationReason == EliminationReason.None)
		{
			responsiblePlayer = null;
			return immediateEliminationReason;
		}
		responsiblePlayer = playerResponsibleForPotentialElimination;
		return potentialEliminationReason;
	}

	[Command]
	private void CmdSetPotentialEliminationReason(PlayerGolfer responsiblePlayer, EliminationReason reason)
	{
		if (base.isServer && base.isClient)
		{
			UserCode_CmdSetPotentialEliminationReason__PlayerGolfer__EliminationReason(responsiblePlayer, reason);
			return;
		}
		NetworkWriterPooled writer = NetworkWriterPool.Get();
		writer.WriteNetworkBehaviour(responsiblePlayer);
		GeneratedNetworkCode._Write_EliminationReason(writer, reason);
		SendCommandInternal("System.Void PlayerGolfer::CmdSetPotentialEliminationReason(PlayerGolfer,EliminationReason)", 996789415, writer, 0);
		NetworkWriterPool.Return(writer);
	}

	[Server]
	private void ServerSetPotentialEliminationReason(PlayerGolfer responsiblePlayer, EliminationReason reason)
	{
		if (!NetworkServer.active)
		{
			Debug.LogWarning("[Server] function 'System.Void PlayerGolfer::ServerSetPotentialEliminationReason(PlayerGolfer,EliminationReason)' called when server was not active");
			return;
		}
		if (reason == EliminationReason.Fall && playerResponsibleForPotentialElimination != null && responsiblePlayer != this)
		{
			potentialEliminationDurationForcedFromKnockdownRecovery = true;
			return;
		}
		playerResponsibleForPotentialElimination = responsiblePlayer;
		potentialEliminationReason = reason;
		potentialEliminationDurationForcedFromKnockdownRecovery = false;
		potentialEliminationResponsibilityTimestamp = Time.timeAsDouble;
	}

	[Server]
	private void ServerClearPotentialEliminationResponsibility()
	{
		if (!NetworkServer.active)
		{
			Debug.LogWarning("[Server] function 'System.Void PlayerGolfer::ServerClearPotentialEliminationResponsibility()' called when server was not active");
			return;
		}
		playerResponsibleForPotentialElimination = null;
		potentialEliminationReason = EliminationReason.None;
		potentialEliminationDurationForcedFromKnockdownRecovery = false;
		potentialEliminationResponsibilityTimestamp = double.MinValue;
	}

	public void SetPitch(float pitch)
	{
		SetPitchInternal(pitch, suppressHotkeySelection: false);
	}

	public void ApplySwingPitchPreset(int index)
	{
		SetPitchInternal(GetSwingPreset(index), suppressHotkeySelection: true);
	}

	private float GetSwingPreset(int index)
	{
		return GameManager.GolfSettings.HotkeySwingPresets[index];
	}

	public void CycleSwingPitchPreset(int dir)
	{
		int pitchPresetIndex = GetPitchPresetIndex();
		if (pitchPresetIndex < 0)
		{
			SetPitchInternal(GetSwingPreset(0), suppressHotkeySelection: false);
			return;
		}
		pitchPresetIndex = BMath.Wrap(pitchPresetIndex + dir, GameManager.GolfSettings.HotkeySwingPresets.Length);
		SetPitchInternal(GetSwingPreset(pitchPresetIndex), suppressHotkeySelection: false);
	}

	public bool TryStartChargingSwing()
	{
		if (!CanStartSwing())
		{
			return false;
		}
		SetIsChargingSwing(isCharging: true);
		UpdateSwingNormalizedPower(forSwingRelease: false, isForcedFumble: false);
		return true;
		bool CanStartSwing()
		{
			if (!IsInitialized)
			{
				return false;
			}
			if (IsMatchResolved)
			{
				return false;
			}
			if (PlayerInfo.AsSpectator.IsSpectating)
			{
				return false;
			}
			if (PlayerInfo.Inventory.GetEffectivelyEquippedItem() != ItemType.None)
			{
				return false;
			}
			if (!IsAimingSwing && !CanAimSwing())
			{
				return false;
			}
			if (IsChargingSwing)
			{
				return false;
			}
			if (RadialMenu.IsVisible)
			{
				return false;
			}
			if (!SingletonBehaviour<DrivingRangeManager>.HasInstance && CourseManager.MatchState < MatchState.TeeOff)
			{
				return false;
			}
			if (!CanInterruptSwing())
			{
				return false;
			}
			return true;
		}
	}

	public void ReleaseSwingCharge()
	{
		ReleaseSwingChargeInternal(isForcedFumble: false);
	}

	private void ReleaseSwingChargeInternal(bool isForcedFumble)
	{
		if (IsChargingSwing)
		{
			if (!SingletonBehaviour<DrivingRangeManager>.HasInstance && CourseManager.MatchState <= MatchState.TeeOff)
			{
				TryCancelSwingCharge();
				return;
			}
			PlayerInfo.Movement.AlignWithCameraImmediately();
			CancelSwing();
			swingRoutine = StartCoroutine(SwingRoutine(isForcedFumble));
		}
	}

	private IEnumerator SwingRoutine(bool isForcedFumble)
	{
		IsSwinging = true;
		PlayerInfo.AnimatorIo.SetIsSwinging(isSwinging: true);
		PlayerInfo.Input.UpdateHotkeyMode();
		LockOnTarget swingLockOnTarget = (isForcedFumble ? null : LockOnTarget);
		SetLockOnTarget(null);
		UpdateSwingNormalizedPower(forSwingRelease: true, isForcedFumble);
		SwingPowerBarUi.ReleaseSwingCharge();
		SetIsChargingSwing(isCharging: false);
		swingTimestamp = Time.timeAsDouble;
		bool isPerfectShot = SwingNormalizedPower > 0.99f && SwingNormalizedPower <= 1f;
		bool isOvercharged = SwingNormalizedPower > 1f;
		Vector3 swingDirection = GetSwingDirection(SwingPitch);
		bool isPutt = SwingPitch <= 0f;
		float sideSpin = ((!MatchSetupRules.GetValueAsBool(MatchSetupRules.Rule.OverChargeSideSpin)) ? 0f : (isOvercharged ? (UnityEngine.Random.Range(-1f, 1f) * GameManager.GolfSettings.MaxSwingFumbleSideSpin) : 0f));
		if (isOvercharged)
		{
			PlayerInfo.PlayerAudio.PlayOverchargedSwingForAllClients(swingLockOnTarget != null);
		}
		else
		{
			PlayerInfo.PlayerAudio.PlaySwingForAllClients(SwingNormalizedPower);
		}
		bool playedSwingVfx = false;
		while (BMath.GetTimeSince(swingTimestamp) < GameManager.GolfSettings.SwingHitStartTime)
		{
			TryPlaySwingVfx();
			yield return null;
		}
		bool didTriggerNiceShotSound = false;
		processedHittableBuffer.Clear();
		while (BMath.GetTimeSince(swingTimestamp) < GameManager.GolfSettings.SwingHitEndTime)
		{
			TryPlaySwingVfx();
			Box swingHitBox = GetSwingHitBox();
			int num = Physics.OverlapBoxNonAlloc(swingHitBox.center, swingHitBox.HalfSize, orientation: base.transform.rotation, mask: GameManager.LayerSettings.SwingHittableMask, results: overlappingColliderBuffer, queryTriggerInteraction: QueryTriggerInteraction.Ignore);
			if (drawGolfSwingDebug)
			{
				swingHitBox.DrawDebug(Color.red);
			}
			bool flag = false;
			bool flag2 = false;
			for (int i = 0; i < num; i++)
			{
				Collider collider = overlappingColliderBuffer[i];
				if (!CanHitCollider(collider, out var hittable) || processedHittableBuffer.Contains(hittable))
				{
					continue;
				}
				flag = true;
				if (hittable.AsEntity.IsGolfBall && hittable.AsEntity.AsGolfBall == NetworkownBall)
				{
					flag2 = true;
					if (isPerfectShot && BMath.GetTimeSince(CourseManager.TeeoffEndTimestamp) < GameManager.GolfSettings.TeeOffPerfectShotSpeedBoostTimeWindow)
					{
						PlayerInfo.Movement.InformOfPerfectShotAtTeeOff();
					}
				}
				processedHittableBuffer.Add(hittable);
				bool flag3 = swingLockOnTarget != null && hittable.SwingSettings.CanBecomeSwingProjectile;
				LockOnTarget lockOnTarget;
				Vector3 vector;
				if (flag3)
				{
					lockOnTarget = swingLockOnTarget;
					float pitch = GetEffectiveLockOnTargetSwingPitchFor(hittable);
					vector = (flag3 ? GetSwingDirection(pitch) : swingDirection);
				}
				else
				{
					lockOnTarget = null;
					vector = swingDirection;
				}
				Vector3 position = collider.ClosestPoint(swingHitBox.center);
				Vector3 localHitPosition = hittable.transform.InverseTransformPoint(position);
				Vector3 localOrigin = hittable.transform.InverseTransformPoint(swingHitBox.center);
				hittable.HitWithGolfSwing(localHitPosition, localOrigin, vector, isPutt, SwingNormalizedPower, sideSpin, this, (lockOnTarget != null) ? lockOnTarget.AsEntity.AsHittable : null);
				if (isPerfectShot)
				{
					if (base.isServer)
					{
						VfxManager.ServerPlayPooledVfxForAllClients(VfxType.SwingNiceShot, position, Quaternion.identity);
					}
					else
					{
						VfxManager.ClientPlayPooledVfxForAllClients(VfxType.SwingNiceShot, position, Quaternion.identity);
					}
					if (!didTriggerNiceShotSound)
					{
						CourseManager.PlayAnnouncerLineLocalOnly(AnnouncerLine.NiceShot);
						didTriggerNiceShotSound = true;
					}
				}
				if (isOvercharged)
				{
					Quaternion rotation = Quaternion.LookRotation(vector);
					if (base.isServer)
					{
						VfxManager.ServerPlayPooledVfxForAllClients(VfxType.SwingOverchargedHit, position, rotation);
					}
					else
					{
						VfxManager.ClientPlayPooledVfxForAllClients(VfxType.SwingOverchargedHit, position, rotation);
					}
				}
				PlayerInfo.PlayerAudio.PlaySwingHitForAllClients(hittable);
				if (drawGolfSwingDebug)
				{
					BDebug.DrawWireSphere(hittable.transform.position, 0.05f, Color.red, 1f);
				}
			}
			if (flag)
			{
				if (!isOvercharged && SwingNormalizedPower >= TutorialManager.Settings.ChargeSwingMinimumSwingNormalizedPower)
				{
					TutorialManager.CompletePrompt(TutorialPrompt.ChargeSwing);
				}
				if (isPutt && SwingNormalizedPower >= TutorialManager.Settings.PuttMinimumSwingNormalizedPower)
				{
					TutorialManager.CompletePrompt(TutorialPrompt.Putt);
				}
			}
			if (flag2)
			{
				HoleProgressBarUi.IncrementStrokes();
				OnPlayerHitOwnBall();
				if (!base.isServer)
				{
					CmdInformHitOwnBall();
				}
			}
			yield return null;
		}
		for (float swingTime = BMath.GetTimeSince(swingTimestamp); swingTime < GameManager.GolfSettings.SwingTotalDuration; swingTime = BMath.GetTimeSince(swingTimestamp))
		{
			yield return null;
			if (ShouldInterruptSwing(swingTime))
			{
				break;
			}
		}
		OnFinishedSwinging();
		float GetEffectiveLockOnTargetSwingPitchFor(Hittable hitHittable)
		{
			Vector3 lockOnPosition = swingLockOnTarget.GetLockOnPosition();
			Vector3 position2 = hitHittable.transform.position;
			float magnitude = (lockOnPosition - position2).AsHorizontal2().magnitude;
			Vector3 vector2 = BMath.Average(position2, lockOnPosition);
			float num2 = BMath.RemapClamped(20f, GameManager.GolfSettings.LockOnMaxDistance, 0f, 20f, magnitude);
			Vector3 vector3 = vector2 + num2 * Vector3.up;
			BDebug.DrawWireSphere(vector3, 0.2f, Color.red, 3f);
			BDebug.DrawLine(position2, vector3, Color.red, 3f);
			return BMath.Clamp(0f - (vector3 - position2).GetPitchDeg(), 0f, 45f);
		}
		void PlaySwingVfxForAllClients(float power, bool isPerfectShot2, bool isOvercharged2)
		{
			PlaySwingVfxInternal(power, isPerfectShot2, isOvercharged2);
			CmdPlaySwingVfxForAllClients(power, isPerfectShot2, isOvercharged2);
		}
		bool ShouldInterruptSwing(float num2)
		{
			if (num2 < GameManager.GolfSettings.SwingMinInterruptionTime)
			{
				return false;
			}
			if (PlayerInfo.Movement.RawMoveVectorMagnitude > 0.1f)
			{
				return true;
			}
			if (PlayerInfo.Input.IsHoldingAimSwing && PlayerInfo.Input.IsHoldingChargeSwing)
			{
				return true;
			}
			return false;
		}
		void TryPlaySwingVfx()
		{
			if (!isPutt && !playedSwingVfx && !(BMath.GetTimeSince(swingTimestamp) < GameManager.GolfSettings.SwingVfxTime))
			{
				playedSwingVfx = true;
				PlaySwingVfxForAllClients(SwingNormalizedPower, isPerfectShot, isOvercharged);
			}
		}
	}

	[Command]
	private void CmdPlaySwingVfxForAllClients(float power, bool isPerfectShot, bool isOvercharged, NetworkConnectionToClient sender = null)
	{
		if (base.isServer && base.isClient)
		{
			UserCode_CmdPlaySwingVfxForAllClients__Single__Boolean__Boolean__NetworkConnectionToClient(power, isPerfectShot, isOvercharged, sender);
			return;
		}
		NetworkWriterPooled writer = NetworkWriterPool.Get();
		writer.WriteFloat(power);
		writer.WriteBool(isPerfectShot);
		writer.WriteBool(isOvercharged);
		SendCommandInternal("System.Void PlayerGolfer::CmdPlaySwingVfxForAllClients(System.Single,System.Boolean,System.Boolean,Mirror.NetworkConnectionToClient)", -1704284589, writer, 0);
		NetworkWriterPool.Return(writer);
	}

	[TargetRpc]
	private void RpcPlaySwingVfx(NetworkConnectionToClient connection, float power, bool isPerfectShot, bool isOvercharged)
	{
		NetworkWriterPooled writer = NetworkWriterPool.Get();
		writer.WriteFloat(power);
		writer.WriteBool(isPerfectShot);
		writer.WriteBool(isOvercharged);
		SendTargetRPCInternal(connection, "System.Void PlayerGolfer::RpcPlaySwingVfx(Mirror.NetworkConnectionToClient,System.Single,System.Boolean,System.Boolean)", 1188878940, writer, 0);
		NetworkWriterPool.Return(writer);
	}

	private void PlaySwingVfxInternal(float power, bool isPerfectShot, bool isOvercharged)
	{
		SwingSlashVfx component;
		if (!VfxPersistentData.TryGetPooledVfx(VfxType.SwingSlash, out var particleSystem))
		{
			Debug.LogError("Failed to get swing VFX");
		}
		else if (!particleSystem.TryGetComponent<SwingSlashVfx>(out component))
		{
			Debug.LogError("Swing VFX doesn't have SwingSlashVfx component");
			particleSystem.ReturnToPool();
		}
		else
		{
			component.transform.SetPositionAndRotation(base.transform.position, base.transform.rotation);
			component.SetData(power, isPerfectShot, isOvercharged);
			particleSystem.Play();
		}
	}

	public void CancelAllActions()
	{
		TryCancelSwingCharge();
		CancelSwing();
		SetLockOnTarget(null);
	}

	public bool TryCancelSwingCharge()
	{
		if (!IsChargingSwing)
		{
			return false;
		}
		SetIsChargingSwing(isCharging: false);
		SwingPowerBarUi.CancelSwingCharge();
		return true;
	}

	public void CancelSwing()
	{
		if (IsSwinging)
		{
			if (swingRoutine != null)
			{
				StopCoroutine(swingRoutine);
			}
			OnFinishedSwinging();
		}
	}

	private void OnFinishedSwinging()
	{
		IsSwinging = false;
		PlayerInfo.AnimatorIo.SetIsSwinging(isSwinging: false);
		PlayerInfo.Input.UpdateHotkeyMode();
		UpdateIsAimingSwing();
		if (PlayerInfo.Input.IsHoldingChargeSwing)
		{
			TryStartChargingSwing();
		}
	}

	[Command]
	private void CmdInformHitOwnBall()
	{
		if (base.isServer && base.isClient)
		{
			UserCode_CmdInformHitOwnBall();
			return;
		}
		NetworkWriterPooled writer = NetworkWriterPool.Get();
		SendCommandInternal("System.Void PlayerGolfer::CmdInformHitOwnBall()", -1158658858, writer, 0);
		NetworkWriterPool.Return(writer);
	}

	private void OnPlayerHitOwnBall()
	{
		PlayerGolfer.PlayerHitOwnBall?.Invoke(this);
	}

	public bool TryGetBestLockOnTarget(float maxDistanceSquared, float maxYawFromCenterScreen, float yawWeight, out LockOnTarget bestLockOnTarget)
	{
		if (!MatchSetupRules.GetValueAsBool(MatchSetupRules.Rule.HomingShots))
		{
			bestLockOnTarget = null;
			return false;
		}
		Camera camera = GameManager.Camera;
		bestLockOnTarget = null;
		float num = float.MaxValue;
		foreach (LockOnTarget target in LockOnTargetManager.Targets)
		{
			if (target == null || target == PlayerInfo.AsEntity.AsLockOnTarget || !target.IsValid())
			{
				continue;
			}
			Vector3 lockOnPosition = target.GetLockOnPosition();
			if ((lockOnPosition - base.transform.position).sqrMagnitude > maxDistanceSquared)
			{
				continue;
			}
			if (!camera.transform.position.TryProjectPointOnPlaneAlong(camera.transform.forward, base.transform.position, base.transform.forward.Horizontalized(), out var projection))
			{
				projection = camera.transform.position;
			}
			float num2 = BMath.Abs(((lockOnPosition - projection).GetYawDeg() - camera.transform.forward.GetYawDeg()).WrapAngleDeg());
			if (num2 > maxYawFromCenterScreen)
			{
				continue;
			}
			float num3 = BMath.Abs(((lockOnPosition - camera.transform.position).GetPitchDeg() - camera.transform.forward.GetPitchDeg()).WrapAngleDeg());
			if (num3 > GameManager.Camera.fieldOfView / 2f)
			{
				continue;
			}
			float num4 = num2 * yawWeight + num3;
			if (num4 >= num)
			{
				continue;
			}
			Vector3 position = camera.transform.position;
			Vector3 vector = lockOnPosition;
			Vector3 vector2 = vector - position;
			int num5 = Physics.RaycastNonAlloc(position, vector2, maxDistance: vector2.magnitude, layerMask: GameManager.LayerSettings.LockOnLineOfSightBlockerMask, results: raycastHitBuffer);
			bool flag = true;
			for (int i = 0; i < num5; i++)
			{
				RaycastHit raycastHit = raycastHitBuffer[i];
				if (!raycastHit.collider.TryGetComponentInParent<Hittable>(out var foundComponent, includeInactive: true))
				{
					flag = false;
					if (drawLockOnLineOfSightDebug)
					{
						BDebug.DrawLine(position, position + vector2.normalized * raycastHit.distance, Color.red);
					}
					break;
				}
				if (!(foundComponent == PlayerInfo.AsHittable) && !(foundComponent == target.AsEntity.AsHittable))
				{
					flag = false;
					if (drawLockOnLineOfSightDebug)
					{
						BDebug.DrawLine(position, position + vector2.normalized * raycastHit.distance, Color.red);
					}
					break;
				}
			}
			if (flag)
			{
				if (drawLockOnLineOfSightDebug)
				{
					BDebug.DrawLine(position, vector, Color.green);
				}
				bestLockOnTarget = target;
				num = num4;
			}
		}
		return bestLockOnTarget != null;
	}

	private bool CanHitCollider(Collider collider, out Hittable hittable)
	{
		if (!collider.TryGetComponentInParent<Hittable>(out hittable, includeInactive: false))
		{
			return false;
		}
		if (hittable == PlayerInfo.AsHittable)
		{
			return false;
		}
		if (hittable.AsEntity.IsGolfBall)
		{
			if (!MatchSetupRules.GetValueAsBool(MatchSetupRules.Rule.HitOtherPlayersBalls) && hittable.AsEntity.AsGolfBall.Owner != null && hittable.AsEntity.AsGolfBall.Owner != this)
			{
				return false;
			}
		}
		else if (hittable.AsEntity.IsGolfTee && hittable.AsEntity.AsGolfTee.Owner != null && hittable.AsEntity.AsGolfTee.Owner != this)
		{
			return false;
		}
		return true;
	}

	private void SetPitchInternal(float pitch, bool suppressHotkeySelection)
	{
		float swingPitch = SwingPitch;
		SwingPitch = BMath.Clamp(pitch, 0f, GameManager.GolfSettings.MaxSwingPitch);
		PlayerInfo.AnimatorIo.SetIsInPuttingMode(pitch <= 0f);
		Hotkeys.SetPitch(SwingPitch);
		if (!suppressHotkeySelection && Hotkeys.CurrentMode == HotkeyMode.SwingPitch)
		{
			Hotkeys.Select(GetPitchPresetIndex(), uiOnly: true);
		}
		if (SwingPitch != swingPitch)
		{
			TutorialManager.CompletePrompt(TutorialPrompt.AdjustAngle);
			if (BMath.Abs(SwingPitch - 45f) <= 0.5f)
			{
				TutorialManager.CompletePrompt(TutorialPrompt.OptimalAngle);
			}
		}
	}

	private int GetPitchPresetIndex()
	{
		int result = -1;
		for (int i = 0; i < GameManager.GolfSettings.HotkeySwingPresets.Length; i++)
		{
			if (SwingPitch.Approximately(GameManager.GolfSettings.HotkeySwingPresets[i], 0.5f))
			{
				result = i;
				break;
			}
		}
		return result;
	}

	private void UpdateIsAimingSwing()
	{
		bool isAimingSwing = IsAimingSwing;
		IsAimingSwing = ShouldAimSwing();
		if (isAimingSwing != IsAimingSwing)
		{
			if (IsAimingSwing)
			{
				GameplayCameraManager.EnterSwingAimCamera();
				SwingTrajectoryPreview.SetIsEnabled(isEnabled: true);
				SwingTrajectoryPreview.ClearData();
				PlayerInfo.CancelEmote(canHideEmoteMenu: false);
				PlayerInfo.Inventory.CancelItemFlourish();
			}
			else
			{
				GameplayCameraManager.ExitSwingAimCamera();
				SwingTrajectoryPreview.SetIsEnabled(isEnabled: false);
			}
			UpdateAdjustAnglePrompt();
			UpdateSwingStanceButtonPrompt();
			PlayerInfo.SetIsAimingSwing(IsAimingSwing);
			PlayerInfo.Input.UpdateHotkeyMode();
			PlayerInfo.AnimatorIo.SetIsAimingSwing(IsAimingSwing);
			PlayerGolfer.LocalPlayerIsAimingSwingChanged?.Invoke();
		}
		if (base.isLocalPlayer)
		{
			bool flag = PlayerInfo.Inventory.GetEffectivelyEquippedItem() != ItemType.None;
			if (flag && dropItemPrompt == null)
			{
				dropItemPrompt = ButtonPromptManager.GetButtonPrompt(PlayerInput.Controls.Gameplay.DropItem, Localization.UI.PROMPT_Drop_Ref);
			}
			else if (!flag && dropItemPrompt != null)
			{
				ButtonPromptManager.ReturnButtonPrompt(dropItemPrompt);
				dropItemPrompt = null;
			}
		}
		bool ShouldAimSwing()
		{
			if (!CanAimSwing())
			{
				return false;
			}
			if (!IsChargingSwing && !IsSwinging && !PlayerInfo.Input.IsHoldingAimSwing)
			{
				return false;
			}
			return true;
		}
	}

	private bool CanAimSwing()
	{
		if (!SingletonBehaviour<DrivingRangeManager>.HasInstance && CourseManager.MatchState < MatchState.TeeOff)
		{
			return false;
		}
		if (IsMatchResolved)
		{
			return false;
		}
		if (PlayerInfo.AsSpectator.IsSpectating)
		{
			return false;
		}
		if (PlayerInfo.Movement.IsRespawning)
		{
			return false;
		}
		if (!PlayerInfo.Movement.IsGrounded)
		{
			return false;
		}
		if (PlayerInfo.Movement.IsKnockedOutOrRecovering)
		{
			return false;
		}
		if (PlayerInfo.Movement.DivingState != DivingState.None)
		{
			return false;
		}
		if (RadialMenu.IsVisible)
		{
			return false;
		}
		if (PlayerInfo.Inventory.GetEffectivelyEquippedItem() != ItemType.None)
		{
			return false;
		}
		return true;
	}

	private void SetIsChargingSwing(bool isCharging)
	{
		bool isChargingSwing = IsChargingSwing;
		IsChargingSwing = isCharging;
		if (IsChargingSwing == isChargingSwing)
		{
			return;
		}
		if (!IsChargingSwing)
		{
			GameplayCameraManager.EndSwingCharge();
			LockOnTargetUiManager.ClearTargets();
			PlayerInfo.PlayerAudio.StopSwingChargeLocalOnly();
			if (cancelSwingPrompt != null)
			{
				ButtonPromptManager.ReturnButtonPrompt(cancelSwingPrompt);
				cancelSwingPrompt = null;
			}
		}
		else
		{
			SwingNormalizedPower = 0f;
			swingPowerTimestamp = Time.timeAsDouble;
			SetLockOnTarget(null);
			GameplayCameraManager.StartSwingCharge();
			PlayerInfo.CancelEmote(canHideEmoteMenu: false);
			PlayerInfo.Inventory.CancelItemFlourish();
			PlayerInfo.PlayerAudio.PlaySwingChargeLocalOnly();
			if (base.isLocalPlayer)
			{
				cancelSwingPrompt = ButtonPromptManager.GetButtonPrompt(PlayerInput.Controls.Gameplay.Cancel, Localization.UI.PROMPT_Cancel_Ref);
			}
		}
		UpdateIsAimingSwing();
		UpdateOverchargedVfx();
		if (base.isLocalPlayer)
		{
			if (isCharging)
			{
				PlayerInfo.AnimatorIo.SetSwingCharge(SwingNormalizedPower);
				PlayerGolfer.LocalPlayerStartedChargingSwing?.Invoke();
			}
			else
			{
				PlayerGolfer.LocalPlayerStoppedChargingSwing?.Invoke();
			}
			PlayerInfo.AnimatorIo.SetIsChargingSwing(isCharging);
		}
	}

	private void SetLockOnTarget(LockOnTarget target)
	{
		LockOnTarget lockOnTarget = LockOnTarget;
		LockOnTarget = target;
		if (LockOnTarget == lockOnTarget)
		{
			return;
		}
		if (lockOnTarget != null)
		{
			LockOnTargetUiManager.RemoveTarget(lockOnTarget);
			lockOnTarget.AsEntity.WillBeDestroyed -= OnLockOnTargetWillBeDestroyed;
			if (lockOnTarget.AsEntity.IsPlayer)
			{
				lockOnTarget.AsEntity.PlayerInfo.Movement.IsVisibleChanged -= OnLockOnTargetPlayerIsVisibleChanged;
				lockOnTarget.AsEntity.PlayerInfo.AsGolfer.MatchResolutionChanged -= OnLockOnTargetPlayerMatchResolutionChanged;
			}
		}
		if (LockOnTarget != null)
		{
			LockOnTargetUiManager.AddTarget(LockOnTarget);
			LockOnTarget.AsEntity.WillBeDestroyed += OnLockOnTargetWillBeDestroyed;
			if (LockOnTarget.AsEntity.IsPlayer)
			{
				LockOnTarget.AsEntity.PlayerInfo.Movement.IsVisibleChanged += OnLockOnTargetPlayerIsVisibleChanged;
				LockOnTarget.AsEntity.PlayerInfo.AsGolfer.MatchResolutionChanged += OnLockOnTargetPlayerMatchResolutionChanged;
			}
		}
		SwingTrajectoryPreview.SetIsLockedOn(LockOnTarget != null);
	}

	[Server]
	private bool ServerTrySetMatchResolution(PlayerMatchResolution resolution)
	{
		if (!NetworkServer.active)
		{
			Debug.LogWarning("[Server] function 'System.Boolean PlayerGolfer::ServerTrySetMatchResolution(PlayerMatchResolution)' called when server was not active");
			return default(bool);
		}
		PlayerGolfer playerGolfer = this;
		PlayerMatchResolution resolution2 = resolution;
		if (SingletonBehaviour<DrivingRangeManager>.HasInstance && resolution2.IsResolved())
		{
			return false;
		}
		if (!CanSetResolution())
		{
			return false;
		}
		NetworkmatchResolution = resolution2;
		if (resolution2 == PlayerMatchResolution.Eliminated)
		{
			PlayerGolfer.AnyPlayerEliminated?.Invoke(this);
		}
		return true;
		bool CanSetResolution()
		{
			if (!IsMatchResolved)
			{
				return true;
			}
			if (resolution2 == PlayerMatchResolution.None)
			{
				return true;
			}
			if (matchResolution == PlayerMatchResolution.Eliminated && resolution2 == PlayerMatchResolution.Scored)
			{
				return true;
			}
			return false;
		}
	}

	private void UpdateSwingNormalizedPower(bool forSwingRelease, bool isForcedFumble)
	{
		float num = 1f + GameManager.GolfSettings.MaxSwingOvercharge;
		if (isForcedFumble)
		{
			SwingNormalizedPower = num;
		}
		else
		{
			float timeSince = BMath.GetTimeSince(swingPowerTimestamp);
			SwingNormalizedPower = BMath.RemapClamped(value: (timeSince < GameManager.GolfSettings.ChargeTimeForRegularFullCharge) ? timeSince : ((!(timeSince < GameManager.GolfSettings.ChargeTimeForRegularFullCharge + GameManager.GolfSettings.SwingRegularFullChargeCoyoteTime)) ? (timeSince - GameManager.GolfSettings.SwingRegularFullChargeCoyoteTime) : GameManager.GolfSettings.ChargeTimeForRegularFullCharge), fromMin: 0f, fromMax: GameManager.GolfSettings.SwingChargeRiseDuration, toMin: 0f, toMax: num, Easing: BMath.EaseIn);
			if (forSwingRelease)
			{
				SwingNormalizedPower = BMath.Max(SwingNormalizedPower, GameManager.GolfSettings.MinSwingReleaseNormalizedPower);
			}
		}
		SwingPowerBarUi.SetNormalizedPower(SwingNormalizedPower);
		PlayerInfo.AnimatorIo.SetSwingCharge(SwingNormalizedPower);
	}

	private Box GetSwingHitBox()
	{
		return new Box(base.transform.TransformPoint(GameManager.GolfSettings.SwingHitBoxLocalCenter), GameManager.GolfSettings.SwingHitBoxSize, base.transform.rotation);
	}

	private Vector3 GetSwingDirection(float pitch)
	{
		return Quaternion.AngleAxis(0f - pitch, base.transform.right) * base.transform.forward;
	}

	private int GetSwingHittablePriority(Hittable hittable)
	{
		if (hittable == null)
		{
			return 0;
		}
		if (hittable.AsEntity.IsGolfBall)
		{
			if (!(hittable.AsEntity.AsGolfBall.Owner == this))
			{
				return 3;
			}
			return 4;
		}
		if (hittable.AsEntity.IsPlayer)
		{
			return 1;
		}
		return 2;
	}

	private void UpdateOverchargedVfx()
	{
		bool flag = overchargedVfx != null;
		bool flag2 = ShouldPlay();
		if (flag2 != flag)
		{
			PlayerInfo.SetShouldPlayOverchargedVfx(flag2);
		}
		bool ShouldPlay()
		{
			if (!IsChargingSwing)
			{
				return false;
			}
			if (SwingNormalizedPower <= 1f)
			{
				return false;
			}
			return true;
		}
	}

	public void SetIsPlayingOverchargedVfx(bool isPlaying)
	{
		bool flag = overchargedVfx != null;
		if (isPlaying != flag)
		{
			if (isPlaying)
			{
				PlayOverchargedVfx();
			}
			else
			{
				ClearOverchargedVfx();
			}
		}
		void PlayOverchargedVfx()
		{
			if (!VfxPersistentData.TryGetPooledVfx(VfxType.SwingOverchargedClub, out overchargedVfx))
			{
				Debug.LogError("Failed to get overcharged swing VFX");
			}
			else
			{
				overchargedVfx.transform.SetParent(PlayerInfo.RightHandEquipmentSwitcher.transform);
				overchargedVfx.transform.SetLocalPositionAndRotation(Vector3.zero, Quaternion.identity);
				overchargedVfx.Play();
			}
		}
	}

	private void ClearOverchargedVfx()
	{
		if (!(overchargedVfx == null))
		{
			overchargedVfx.Stop(ParticleSystemStopBehavior.StopEmittingAndClear);
			overchargedVfx = null;
		}
	}

	private void ServerUpdateIsActiveOnGreen()
	{
		bool isActiveOnGreen = this.IsActiveOnGreen;
		this.IsActiveOnGreen = IsActiveOnGreen();
		if (this.IsActiveOnGreen != isActiveOnGreen)
		{
			if (this.IsActiveOnGreen)
			{
				CourseManager.RegisterActivePlayerOnGreen(this);
			}
			else
			{
				CourseManager.DeregisterActivePlayerOnGreen(this);
			}
		}
		bool IsActiveOnGreen()
		{
			if (matchResolution != PlayerMatchResolution.None)
			{
				return false;
			}
			if (!PlayerInfo.LevelBoundsTracker.AuthoritativeIsOnGreen)
			{
				return false;
			}
			return true;
		}
	}

	public bool IsAimingSwingNetworked()
	{
		if (!base.isLocalPlayer)
		{
			return PlayerInfo.NetworkedIsAimingSwing;
		}
		return IsAimingSwing;
	}

	public bool CanRotate()
	{
		if (IsMatchResolved)
		{
			return false;
		}
		if (!CanInterruptSwing())
		{
			return false;
		}
		return true;
	}

	public bool CanMove()
	{
		if (IsMatchResolved)
		{
			return false;
		}
		if (!CanInterruptSwing())
		{
			return false;
		}
		return true;
	}

	public bool CanInterruptSwing()
	{
		if (!IsSwinging)
		{
			return true;
		}
		return BMath.GetTimeSince(swingTimestamp) >= GameManager.GolfSettings.SwingMinInterruptionTime;
	}

	private void OnServerWasHitByItem(PlayerInventory itemUser, ItemType itemType, ItemUseId itemUseId, Vector3 direction, float distance, bool isReflected)
	{
		if (!itemType.TryGetEliminationReason(distance, isReflected, out var eliminationReason))
		{
			return;
		}
		ServerSetPotentialEliminationReason((itemUser != null) ? itemUser.PlayerInfo.AsGolfer : null, eliminationReason);
		if (eliminationReason == EliminationReason.OrbitalLaserCenter)
		{
			ServerEliminate(eliminationReason);
			if (itemUser != null)
			{
				itemUser.PlayerInfo.Movement.RpcInformKnockedOutOtherPlayer();
			}
			CourseManager.MarkLatestValidKnockout(PlayerInfo, itemUseId);
		}
	}

	private void OnServerWasHitByRocketLauncherBackBlast(PlayerInventory rocketLauncherUser, Vector3 direction)
	{
		ServerSetPotentialEliminationReason(rocketLauncherUser.PlayerInfo.AsGolfer, EliminationReason.RocketBackBlast);
	}

	private void OnServerWasHitByDive(PlayerMovement hitter)
	{
		ServerSetPotentialEliminationReason(hitter.PlayerInfo.AsGolfer, EliminationReason.Dive);
	}

	private void OnServerIsOnGreenChanged()
	{
		if (!PlayerInfo.LevelBoundsTracker.AuthoritativeIsOnGreen && CourseManager.MatchState == MatchState.Overtime)
		{
			ServerEliminate(EliminationReason.OutOfBounds);
		}
		ServerUpdateIsActiveOnGreen();
	}

	private void OnServerBoundsStateChanged(BoundsState previousState, BoundsState currentState)
	{
		if (!currentState.HasState(BoundsState.OutOfBounds))
		{
			ServerOutOfBoundsTimerEliminationTimestamp = double.MinValue;
		}
	}

	private void OnServerMatchStateChanged(MatchState previousState, MatchState currentState)
	{
		if (currentState == MatchState.Ended)
		{
			if (matchResolution == PlayerMatchResolution.None)
			{
				ServerEliminate(EliminationReason.TimedOut);
			}
			if (serverSpectatedEntireHole)
			{
				PlayerInfo.RpcInformSpectatedEntireHole();
			}
		}
	}

	private void OnLocalPlayerIsGroundedChanged()
	{
		if (!PlayerInfo.Movement.IsGrounded)
		{
			CancelAllActions();
		}
		UpdateIsAimingSwing();
	}

	private void OnLocalPlayerIsKnockedOutChanged()
	{
		UpdateIsAimingSwing();
		CancelAllActions();
	}

	private void OnLocalPlayerEquippedItemChanged()
	{
		UpdateIsAimingSwing();
	}

	private void OnLockOnTargetWillBeDestroyed()
	{
		SetLockOnTarget(null);
	}

	private void OnLockOnTargetPlayerIsVisibleChanged()
	{
		if (!LockOnTarget.IsValid())
		{
			SetLockOnTarget(null);
		}
	}

	private void OnLockOnTargetPlayerMatchResolutionChanged(PlayerMatchResolution previousResolution, PlayerMatchResolution currentResolution)
	{
		if (!LockOnTarget.IsValid())
		{
			SetLockOnTarget(null);
		}
	}

	private void OnIsInitializedChanged(bool wasInitialized, bool isInitialized)
	{
		if (isInitialized)
		{
			this.Initialized?.Invoke();
		}
	}

	private void OnOwnBallChanged(GolfBall previousBall, GolfBall currentBall)
	{
		if (base.isLocalPlayer)
		{
			if (currentBall == null)
			{
				TutorialManager.DisallowPromptCategory(TutorialPromptCategory.Ball);
			}
			else
			{
				TutorialManager.AllowPromptCategory(TutorialPromptCategory.Ball);
			}
			PlayerGolfer.LocalPlayerOwnBallChanged?.Invoke();
		}
		PlayerInfo.Cosmetics.UpdateGolfBallCosmetic(allowUnequip: false);
	}

	private void OnMatchResolutionChanged(PlayerMatchResolution previousResolution, PlayerMatchResolution currentResolution)
	{
		if (base.isLocalPlayer)
		{
			if (IsMatchResolved)
			{
				OnLocalPlayerMatchResolved();
			}
			PlayerInfo.AnimatorIo.SetVictoryDance((currentResolution == PlayerMatchResolution.Scored) ? PlayerInfo.Cosmetics.victoryDance : VictoryDance.None);
			if (currentResolution == PlayerMatchResolution.Eliminated)
			{
				PlayerInfo.AnimatorIo.SetLost();
			}
			if (currentResolution == PlayerMatchResolution.Scored)
			{
				CosmeticsUnlocksManager.RewardCredits(CourseManager.GetCurrentHolePar() * 25);
			}
		}
		if (base.isServer)
		{
			ServerUpdateIsActiveOnGreen();
		}
		this.MatchResolutionChanged?.Invoke(previousResolution, currentResolution);
		PlayerGolfer.AnyPlayerMatchResolutionChanged?.Invoke(this, previousResolution, currentResolution);
		if (base.isLocalPlayer)
		{
			PlayerGolfer.LocalPlayerMatchResolutionChanged?.Invoke(previousResolution, currentResolution);
		}
		void OnLocalPlayerMatchResolved()
		{
			if (!BNetworkManager.IsChangingSceneOrShuttingDown && !PlayerInfo.AsSpectator.IsSpectating)
			{
				CancelAllActions();
				PlayerInfo.CancelEmote(canHideEmoteMenu: true);
				PlayerInfo.ExitGolfCart(GolfCartExitType.Default);
				PlayerInfo.AsSpectator.StartSpectatingDelayed(GameManager.MatchSettings.MatchResolvedSpectateStartDelay, canRestartDelay: false);
			}
		}
	}

	private void OnIsAheadOfBallChanged(bool wasAhead, bool isAhead)
	{
		if (base.isLocalPlayer)
		{
			if (isAhead)
			{
				AheadOfBallMessage.Show();
			}
			else
			{
				AheadOfBallMessage.Hide();
			}
		}
	}

	private void UpdateSwingStanceButtonPrompt()
	{
		bool flag = !IsAimingSwing && PlayerInfo.RightHandEquipmentSwitcher.CurrentEquipment != null && PlayerInfo.RightHandEquipmentSwitcher.CurrentEquipment.Type == EquipmentType.GolfClub && NetworkownBall != null && Vector3.Distance(base.transform.position, NetworkownBall.transform.position) < 5f;
		if (flag && swingStancePrompt == null)
		{
			swingStancePrompt = ButtonPromptManager.GetButtonPrompt(PlayerInput.Controls.Gameplay.Aim, Localization.UI.PROMPT_SwingStance_Ref);
		}
		else if (!flag && swingStancePrompt != null)
		{
			ButtonPromptManager.ReturnButtonPrompt(swingStancePrompt);
			swingStancePrompt = null;
		}
	}

	private void UpdateAdjustAnglePrompt()
	{
		if (!IsAimingSwing)
		{
			ReturnPrompt();
		}
		else if (base.isLocalPlayer)
		{
			ReturnPrompt();
			adjustAnglePrompt = ButtonPromptManager.GetButtonPrompt(InputManager.UsingKeyboard ? PlayerInput.Controls.Gameplay.Pitch : PlayerInput.Controls.Gameplay.CycleSwingAngle, Localization.UI.PROMPT_AdjustAngle_Ref);
		}
		void ReturnPrompt()
		{
			if (adjustAnglePrompt != null)
			{
				ButtonPromptManager.ReturnButtonPrompt(adjustAnglePrompt);
				adjustAnglePrompt = null;
			}
		}
	}

	public PlayerGolfer()
	{
		_Mirror_SyncVarHookDelegate_isInitialized = OnIsInitializedChanged;
		_Mirror_SyncVarHookDelegate_ownBall = OnOwnBallChanged;
		_Mirror_SyncVarHookDelegate_matchResolution = OnMatchResolutionChanged;
		_Mirror_SyncVarHookDelegate_isAheadOfBall = OnIsAheadOfBallChanged;
	}

	static PlayerGolfer()
	{
		JoinAsSpectator = false;
		overlappingSingleColliderBuffer = new Collider[1];
		overlappingColliderBuffer = new Collider[100];
		processedHittableBuffer = new HashSet<Hittable>();
		processedEntityBuffer = new HashSet<Entity>();
		raycastSingleHitBuffer = new RaycastHit[1];
		raycastHitBuffer = new RaycastHit[100];
		RemoteProcedureCalls.RegisterCommand(typeof(PlayerGolfer), "System.Void PlayerGolfer::CmdReturnBallToPlayerFromConsole()", InvokeUserCode_CmdReturnBallToPlayerFromConsole, requiresAuthority: true);
		RemoteProcedureCalls.RegisterCommand(typeof(PlayerGolfer), "System.Void PlayerGolfer::CmdRestartBall()", InvokeUserCode_CmdRestartBall, requiresAuthority: true);
		RemoteProcedureCalls.RegisterCommand(typeof(PlayerGolfer), "System.Void PlayerGolfer::CmdFinishHole()", InvokeUserCode_CmdFinishHole, requiresAuthority: true);
		RemoteProcedureCalls.RegisterCommand(typeof(PlayerGolfer), "System.Void PlayerGolfer::CmdSetPotentialEliminationReason(PlayerGolfer,EliminationReason)", InvokeUserCode_CmdSetPotentialEliminationReason__PlayerGolfer__EliminationReason, requiresAuthority: true);
		RemoteProcedureCalls.RegisterCommand(typeof(PlayerGolfer), "System.Void PlayerGolfer::CmdPlaySwingVfxForAllClients(System.Single,System.Boolean,System.Boolean,Mirror.NetworkConnectionToClient)", InvokeUserCode_CmdPlaySwingVfxForAllClients__Single__Boolean__Boolean__NetworkConnectionToClient, requiresAuthority: true);
		RemoteProcedureCalls.RegisterCommand(typeof(PlayerGolfer), "System.Void PlayerGolfer::CmdInformHitOwnBall()", InvokeUserCode_CmdInformHitOwnBall, requiresAuthority: true);
		RemoteProcedureCalls.RegisterRpc(typeof(PlayerGolfer), "System.Void PlayerGolfer::RpcInformWillBeEliminated(EliminationReason,UnityEngine.Vector3)", InvokeUserCode_RpcInformWillBeEliminated__EliminationReason__Vector3);
		RemoteProcedureCalls.RegisterRpc(typeof(PlayerGolfer), "System.Void PlayerGolfer::RpcPlaySwingVfx(Mirror.NetworkConnectionToClient,System.Single,System.Boolean,System.Boolean)", InvokeUserCode_RpcPlaySwingVfx__NetworkConnectionToClient__Single__Boolean__Boolean);
	}

	public override bool Weaved()
	{
		return true;
	}

	protected void UserCode_CmdReturnBallToPlayerFromConsole()
	{
		if (serverReturnBallCommandRateLimiter.RegisterHit() && !(NetworkownBall == null) && MatchSetupRules.IsCheatsEnabled())
		{
			ServerReturnBallToPlayer(isRestart: false);
		}
	}

	protected static void InvokeUserCode_CmdReturnBallToPlayerFromConsole(NetworkBehaviour obj, NetworkReader reader, NetworkConnectionToClient senderConnection)
	{
		if (!NetworkServer.active)
		{
			Debug.LogError("Command CmdReturnBallToPlayerFromConsole called on client.");
		}
		else
		{
			((PlayerGolfer)obj).UserCode_CmdReturnBallToPlayerFromConsole();
		}
	}

	protected void UserCode_CmdRestartBall()
	{
		if (serverRestartBallCommandRateLimiter.RegisterHit() && !(NetworkownBall == null))
		{
			ServerReturnBallToPlayer(isRestart: true);
		}
	}

	protected static void InvokeUserCode_CmdRestartBall(NetworkBehaviour obj, NetworkReader reader, NetworkConnectionToClient senderConnection)
	{
		if (!NetworkServer.active)
		{
			Debug.LogError("Command CmdRestartBall called on client.");
		}
		else
		{
			((PlayerGolfer)obj).UserCode_CmdRestartBall();
		}
	}

	protected void UserCode_CmdFinishHole()
	{
		if (serverFinishHoleCommandRateLimiter.RegisterHit() && !(NetworkownBall == null) && !(GolfHoleManager.MainHole == null) && (!IsMatchResolved || matchResolution == PlayerMatchResolution.Eliminated))
		{
			Vector3 position = GolfHoleManager.MainHole.transform.position;
			NetworkownBall.AsEntity.InformWillTeleport();
			NetworkownBall.transform.SetPositionAndRotation(position, base.transform.rotation);
			NetworkownBall.AsEntity.Rigidbody.position = position;
			NetworkownBall.AsEntity.Rigidbody.rotation = base.transform.rotation;
			NetworkownBall.Rigidbody.linearVelocity = Vector3.zero;
			NetworkownBall.Rigidbody.angularVelocity = Vector3.zero;
			NetworkownBall.AsEntity.InformTeleported();
			CourseManager.AddPenaltyStroke(this, suppressPopup: true);
		}
	}

	protected static void InvokeUserCode_CmdFinishHole(NetworkBehaviour obj, NetworkReader reader, NetworkConnectionToClient senderConnection)
	{
		if (!NetworkServer.active)
		{
			Debug.LogError("Command CmdFinishHole called on client.");
		}
		else
		{
			((PlayerGolfer)obj).UserCode_CmdFinishHole();
		}
	}

	protected void UserCode_RpcInformWillBeEliminated__EliminationReason__Vector3(EliminationReason immediateEliminationReason, Vector3 eliminationPosition)
	{
		OnLocalPlayerWillBeEliminated(immediateEliminationReason, eliminationPosition);
	}

	protected static void InvokeUserCode_RpcInformWillBeEliminated__EliminationReason__Vector3(NetworkBehaviour obj, NetworkReader reader, NetworkConnectionToClient senderConnection)
	{
		if (!NetworkClient.active)
		{
			Debug.LogError("TargetRPC RpcInformWillBeEliminated called on server.");
		}
		else
		{
			((PlayerGolfer)obj).UserCode_RpcInformWillBeEliminated__EliminationReason__Vector3(GeneratedNetworkCode._Read_EliminationReason(reader), reader.ReadVector3());
		}
	}

	protected void UserCode_CmdSetPotentialEliminationReason__PlayerGolfer__EliminationReason(PlayerGolfer responsiblePlayer, EliminationReason reason)
	{
		if (serverSetPotentialEliminationReasonCommandRateLimiter.RegisterHit())
		{
			ServerSetPotentialEliminationReason(responsiblePlayer, reason);
		}
	}

	protected static void InvokeUserCode_CmdSetPotentialEliminationReason__PlayerGolfer__EliminationReason(NetworkBehaviour obj, NetworkReader reader, NetworkConnectionToClient senderConnection)
	{
		if (!NetworkServer.active)
		{
			Debug.LogError("Command CmdSetPotentialEliminationReason called on client.");
		}
		else
		{
			((PlayerGolfer)obj).UserCode_CmdSetPotentialEliminationReason__PlayerGolfer__EliminationReason(reader.ReadNetworkBehaviour<PlayerGolfer>(), GeneratedNetworkCode._Read_EliminationReason(reader));
		}
	}

	protected void UserCode_CmdPlaySwingVfxForAllClients__Single__Boolean__Boolean__NetworkConnectionToClient(float power, bool isPerfectShot, bool isOvercharged, NetworkConnectionToClient sender)
	{
		if (!serverSwingVfxCommandRateLimiter.RegisterHit())
		{
			return;
		}
		if (sender != null && sender != NetworkServer.localConnection)
		{
			PlaySwingVfxInternal(power, isPerfectShot, isOvercharged);
		}
		foreach (NetworkConnectionToClient value in NetworkServer.connections.Values)
		{
			if (value != NetworkServer.localConnection && value != sender)
			{
				RpcPlaySwingVfx(value, power, isPerfectShot, isOvercharged);
			}
		}
	}

	protected static void InvokeUserCode_CmdPlaySwingVfxForAllClients__Single__Boolean__Boolean__NetworkConnectionToClient(NetworkBehaviour obj, NetworkReader reader, NetworkConnectionToClient senderConnection)
	{
		if (!NetworkServer.active)
		{
			Debug.LogError("Command CmdPlaySwingVfxForAllClients called on client.");
		}
		else
		{
			((PlayerGolfer)obj).UserCode_CmdPlaySwingVfxForAllClients__Single__Boolean__Boolean__NetworkConnectionToClient(reader.ReadFloat(), reader.ReadBool(), reader.ReadBool(), senderConnection);
		}
	}

	protected void UserCode_RpcPlaySwingVfx__NetworkConnectionToClient__Single__Boolean__Boolean(NetworkConnectionToClient connection, float power, bool isPerfectShot, bool isOvercharged)
	{
		PlaySwingVfxInternal(power, isPerfectShot, isOvercharged);
	}

	protected static void InvokeUserCode_RpcPlaySwingVfx__NetworkConnectionToClient__Single__Boolean__Boolean(NetworkBehaviour obj, NetworkReader reader, NetworkConnectionToClient senderConnection)
	{
		if (!NetworkClient.active)
		{
			Debug.LogError("TargetRPC RpcPlaySwingVfx called on server.");
		}
		else
		{
			((PlayerGolfer)obj).UserCode_RpcPlaySwingVfx__NetworkConnectionToClient__Single__Boolean__Boolean(null, reader.ReadFloat(), reader.ReadBool(), reader.ReadBool());
		}
	}

	protected void UserCode_CmdInformHitOwnBall()
	{
		if (serverHitOwnBallCommandRateLimiter.RegisterHit())
		{
			OnPlayerHitOwnBall();
		}
	}

	protected static void InvokeUserCode_CmdInformHitOwnBall(NetworkBehaviour obj, NetworkReader reader, NetworkConnectionToClient senderConnection)
	{
		if (!NetworkServer.active)
		{
			Debug.LogError("Command CmdInformHitOwnBall called on client.");
		}
		else
		{
			((PlayerGolfer)obj).UserCode_CmdInformHitOwnBall();
		}
	}

	public override void SerializeSyncVars(NetworkWriter writer, bool forceAll)
	{
		base.SerializeSyncVars(writer, forceAll);
		if (forceAll)
		{
			writer.WriteBool(isInitialized);
			writer.WriteNetworkBehaviour(NetworkownTee);
			writer.WriteNetworkBehaviour(NetworkownBall);
			GeneratedNetworkCode._Write_PlayerMatchResolution(writer, matchResolution);
			writer.WriteBool(isAheadOfBall);
			return;
		}
		writer.WriteVarULong(syncVarDirtyBits);
		if ((syncVarDirtyBits & 1L) != 0L)
		{
			writer.WriteBool(isInitialized);
		}
		if ((syncVarDirtyBits & 2L) != 0L)
		{
			writer.WriteNetworkBehaviour(NetworkownTee);
		}
		if ((syncVarDirtyBits & 4L) != 0L)
		{
			writer.WriteNetworkBehaviour(NetworkownBall);
		}
		if ((syncVarDirtyBits & 8L) != 0L)
		{
			GeneratedNetworkCode._Write_PlayerMatchResolution(writer, matchResolution);
		}
		if ((syncVarDirtyBits & 0x10L) != 0L)
		{
			writer.WriteBool(isAheadOfBall);
		}
	}

	public override void DeserializeSyncVars(NetworkReader reader, bool initialState)
	{
		base.DeserializeSyncVars(reader, initialState);
		if (initialState)
		{
			GeneratedSyncVarDeserialize(ref isInitialized, _Mirror_SyncVarHookDelegate_isInitialized, reader.ReadBool());
			GeneratedSyncVarDeserialize_NetworkBehaviour(ref ownTee, null, reader, ref ___ownTeeNetId);
			GeneratedSyncVarDeserialize_NetworkBehaviour(ref ownBall, _Mirror_SyncVarHookDelegate_ownBall, reader, ref ___ownBallNetId);
			GeneratedSyncVarDeserialize(ref matchResolution, _Mirror_SyncVarHookDelegate_matchResolution, GeneratedNetworkCode._Read_PlayerMatchResolution(reader));
			GeneratedSyncVarDeserialize(ref isAheadOfBall, _Mirror_SyncVarHookDelegate_isAheadOfBall, reader.ReadBool());
			return;
		}
		long num = (long)reader.ReadVarULong();
		if ((num & 1L) != 0L)
		{
			GeneratedSyncVarDeserialize(ref isInitialized, _Mirror_SyncVarHookDelegate_isInitialized, reader.ReadBool());
		}
		if ((num & 2L) != 0L)
		{
			GeneratedSyncVarDeserialize_NetworkBehaviour(ref ownTee, null, reader, ref ___ownTeeNetId);
		}
		if ((num & 4L) != 0L)
		{
			GeneratedSyncVarDeserialize_NetworkBehaviour(ref ownBall, _Mirror_SyncVarHookDelegate_ownBall, reader, ref ___ownBallNetId);
		}
		if ((num & 8L) != 0L)
		{
			GeneratedSyncVarDeserialize(ref matchResolution, _Mirror_SyncVarHookDelegate_matchResolution, GeneratedNetworkCode._Read_PlayerMatchResolution(reader));
		}
		if ((num & 0x10L) != 0L)
		{
			GeneratedSyncVarDeserialize(ref isAheadOfBall, _Mirror_SyncVarHookDelegate_isAheadOfBall, reader.ReadBool());
		}
	}
}
