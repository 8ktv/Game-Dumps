#define DEBUG_DRAW
using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using FMODUnity;
using Mirror;
using Mirror.RemoteCalls;
using UnityEngine;
using UnityEngine.Pool;

public class GolfBall : NetworkBehaviour, IFixedBUpdateCallback, IAnyBUpdateCallback, ILateBUpdateCallback
{
	private enum FrictionMode
	{
		Air,
		Ground
	}

	[SerializeField]
	private BallVfxSettings vfxSettings;

	private LevelBoundsTracker levelBoundsTracker;

	private Rigidbody rigidbody;

	private SphereCollider collider;

	private Renderer[] renderers;

	private readonly List<MeshRenderer> meshRenderers = new List<MeshRenderer>();

	private readonly List<Material> meshRendererOriginalMaterials = new List<Material>();

	[SyncVar(hook = "OnOwnerChanged")]
	private PlayerGolfer owner;

	[SyncVar]
	private float rollingDownhillTime;

	private FrictionMode frictionMode;

	[SyncVar(hook = "OnOutOfBoundsReturnStateChanged")]
	private BallOutOfBoundsReturnState outOfBoundsReturnState;

	private bool performedBallOutOfBoundsTeleport;

	private bool hasBallBeenMovedToHangOverHeadPosition;

	private double returnToBoundsDropOnHeadStartTimestamp;

	private Coroutine returnToBoundsRoutine;

	private HashSet<Collider> temporarilyIgnoredColliders = new HashSet<Collider>();

	private NameTagUi nameTag;

	private WorldspaceIconUi worldspaceIcon;

	private float fullStopFactor;

	private bool isDisplayingNotAllowedVisuals;

	private PuttingTrail puttingTrailVfx;

	private PoolableParticleSystem respawnVfx;

	[SyncVar(hook = "OnIsHiddenChanged")]
	private bool isHidden;

	private bool isInHole;

	private bool canBeAffectedByWind;

	public bool serverLastStrokeTrajectoryDeflected;

	public bool serverLastStrokeTrajectoryDeflectedByTree;

	public bool serverLastStrokeSlowedByFoliage;

	private readonly RaycastHit[] raycastHitBuffer = new RaycastHit[10];

	[CVar("drawBallGroundingDebug", "", "", false, true)]
	private static bool drawBallGroundingDebug;

	private static bool drawBallDistanceFromPlayerDebug;

	private static bool enableNotAllowedVisuals;

	protected NetworkBehaviourSyncVar ___ownerNetId;

	public Action<PlayerGolfer, PlayerGolfer> _Mirror_SyncVarHookDelegate_owner;

	public Action<BallOutOfBoundsReturnState, BallOutOfBoundsReturnState> _Mirror_SyncVarHookDelegate_outOfBoundsReturnState;

	public Action<bool, bool> _Mirror_SyncVarHookDelegate_isHidden;

	public Entity AsEntity { get; private set; }

	public bool IsGrounded { get; private set; }

	public BallGroundData GroundData { get; private set; }

	public double GroundTimestamp { get; private set; }

	public double LastRespawnTimestamp { get; private set; }

	public BallOutOfBoundsReturnState OutOfBoundsReturnState => outOfBoundsReturnState;

	public bool IsStationary { get; private set; } = true;

	public bool IsHidden => isHidden;

	public Vector3 ServerLastStrokePosition { get; private set; }

	public Rigidbody Rigidbody => rigidbody;

	public PlayerGolfer Owner => Networkowner;

	public SphereCollider Collider => collider;

	public BallVfxSettings VfxSettings => vfxSettings;

	public PlayerGolfer Networkowner
	{
		get
		{
			return GetSyncVarNetworkBehaviour(___ownerNetId, ref owner);
		}
		[param: In]
		set
		{
			GeneratedSyncVarSetter_NetworkBehaviour(value, ref owner, 1uL, _Mirror_SyncVarHookDelegate_owner, ref ___ownerNetId);
		}
	}

	public float NetworkrollingDownhillTime
	{
		get
		{
			return rollingDownhillTime;
		}
		[param: In]
		set
		{
			GeneratedSyncVarSetter(value, ref rollingDownhillTime, 2uL, null);
		}
	}

	public BallOutOfBoundsReturnState NetworkoutOfBoundsReturnState
	{
		get
		{
			return outOfBoundsReturnState;
		}
		[param: In]
		set
		{
			GeneratedSyncVarSetter(value, ref outOfBoundsReturnState, 4uL, _Mirror_SyncVarHookDelegate_outOfBoundsReturnState);
		}
	}

	public bool NetworkisHidden
	{
		get
		{
			return isHidden;
		}
		[param: In]
		set
		{
			GeneratedSyncVarSetter(value, ref isHidden, 8uL, _Mirror_SyncVarHookDelegate_isHidden);
		}
	}

	public event Action OutOfBoundsReturnStateChanged;

	public event Action IsHiddenChanged;

	public event Action<GolfBall> IsHiddenChangedReferenced;

	public static event Action LocalPlayerBallIsHiddenChanged;

	public static event Action LocalPlayerBallIsStationaryChanged;

	[CCommand("drawBallDistanceFromPlayerDebug", "", false, false)]
	private static void DrawBallDistanceFromPlayerDebug(bool draw)
	{
		if (draw == drawBallDistanceFromPlayerDebug)
		{
			return;
		}
		drawBallDistanceFromPlayerDebug = draw;
		if (draw)
		{
			GolfBall[] array = UnityEngine.Object.FindObjectsByType<GolfBall>(FindObjectsSortMode.None);
			for (int i = 0; i < array.Length; i++)
			{
				BDebug.RegisterOnGuiCallback(array[i].DrawOnGuiDebug);
			}
		}
		else
		{
			GolfBall[] array = UnityEngine.Object.FindObjectsByType<GolfBall>(FindObjectsSortMode.None);
			for (int i = 0; i < array.Length; i++)
			{
				BDebug.DeregisterOnGuiCallback(array[i].DrawOnGuiDebug);
			}
		}
	}

	[CCommand("setBallNotAllowedVisuals", "", false, false)]
	private static void SetNotAllowedVisuals(bool enabled)
	{
		enableNotAllowedVisuals = enabled;
		GolfBall[] array = UnityEngine.Object.FindObjectsByType<GolfBall>(FindObjectsSortMode.None);
		for (int i = 0; i < array.Length; i++)
		{
			array[i].UpdateNotAllowedVisuals(forced: true);
		}
	}

	private void Awake()
	{
		AsEntity = GetComponent<Entity>();
		levelBoundsTracker = GetComponent<LevelBoundsTracker>();
		rigidbody = GetComponent<Rigidbody>();
		collider = GetComponent<SphereCollider>();
		UpdateRenderers();
		collider.sharedMaterial = PhysicsManager.Settings.BallMaterial;
		rigidbody.useGravity = false;
		rigidbody.linearDamping = 0f;
		UpdateFrictionMode(forced: true);
		collider.hasModifiableContacts = true;
		BUpdate.RegisterCallback(this);
		UpdateNotAllowedVisuals();
		OnRespawned();
		if (VfxPersistentData.TryGetPooledVfx(vfxSettings.PuttingTrail, out var particleSystem))
		{
			if (!particleSystem.TryGetComponent<PuttingTrail>(out puttingTrailVfx))
			{
				Debug.LogError("Pooled VFX does not have the PuttingTrail component");
				particleSystem.ReturnToPool();
			}
			else
			{
				puttingTrailVfx.Initialize(this);
			}
		}
		GameManager.LocalPlayerRegistered += OnLocalPlayerRegistered;
		GameManager.RemotePlayerRegistered += OnRemotePlayerRegistered;
		CourseManager.PlayerKnockoutStreaksChanged += OnPlayerKnockoutStreakChanged;
		PlayerId.AnyPlayerGuidChanged += OnAnyPlayerGuidChanged;
		PlayerSpectator.LocalPlayerIsSpectatingChanged += OnLocalPlayerIsSpectatingChanged;
		PlayerSpectator.LocalPlayerSetSpectatingTarget += OnLocalPlayerSetSpectatingTarget;
		PlayerSpectator.LocalPlayerStoppedSpectating += OnLocalPlayerStoppedSpectating;
		AsEntity.FinishedTemporarilyIgnoringCollisionsWith += OnFinishedTemporarilyIgnoringCollisionsWith;
		AsEntity.AsHittable.WillApplyGolfSwingHitPhysics += OnWillApplyGolfSwingHitPhysics;
		AsEntity.AsHittable.WillApplyItemHitPhysics += OnWillApplyItemHitPhysics;
		AsEntity.AsHittable.WillApplyRocketLauncherBackBlastHitPhysics += OnWillApplyRocketLauncherBackBlastHitPhysics;
		AsEntity.AsHittable.WillApplyRocketDriverSwingPostHitSpinHitPhysics += OnWillApplyRocketDriverSwingPostHitSpinHitPhysics;
		AsEntity.AsHittable.WillApplyScoreKnockbackPhysics += OnWillApplyScoreKnockbackPhysics;
		AsEntity.AsHittable.WillApplyJumpPadPhysics += OnWillApplyJumpPadPhysics;
		AsEntity.AsHittable.WasHitByGolfSwing += OnWasHitByGolfSwing;
		AsEntity.AsHittable.IsPlayingHomingWarningChanged += OnIsPlayingHomingWarningChanged;
		AsEntity.AsHittable.AppliedPostHitBounce += OnAppliedPostHitBounce;
		GetComponent<PlayerCosmeticsObjectSwitcher>().OnModelOverride += OnModelOverride;
		if (drawBallDistanceFromPlayerDebug)
		{
			BDebug.RegisterOnGuiCallback(DrawOnGuiDebug);
		}
	}

	private void UpdateRenderers()
	{
		for (int i = 0; i < meshRenderers.Count; i++)
		{
			if (meshRenderers[i] != null)
			{
				meshRenderers[i].sharedMaterial = meshRendererOriginalMaterials[i];
			}
		}
		meshRenderers.Clear();
		meshRendererOriginalMaterials.Clear();
		renderers = GetComponentsInChildren<Renderer>();
		Renderer[] array = renderers;
		for (int j = 0; j < array.Length; j++)
		{
			if (array[j] is MeshRenderer meshRenderer)
			{
				meshRenderers.Add(meshRenderer);
				meshRendererOriginalMaterials.Add(meshRenderer.sharedMaterial);
			}
		}
	}

	private void Start()
	{
		PhysicsManager.RegisterBallColliderId(collider.GetInstanceID());
	}

	public override void OnStartServer()
	{
		CourseManager.RegisterActiveBall(this);
		ServerLastStrokePosition = base.transform.position;
		CourseManager.MatchStateChanged += OnServerMatchStateChanged;
	}

	public override void OnStopServer()
	{
		CourseManager.MatchStateChanged -= OnServerMatchStateChanged;
		if (!BNetworkManager.IsChangingSceneOrShuttingDown)
		{
			CourseManager.DeregisterActiveBall(this);
		}
	}

	public void OnWillBeDestroyed()
	{
		BUpdate.DeregisterCallback(this);
		PhysicsManager.DeregisterBallColliderId(collider.GetInstanceID());
		if (drawBallDistanceFromPlayerDebug)
		{
			BDebug.DeregisterOnGuiCallback(DrawOnGuiDebug);
		}
		RemoveNameTag();
		RemoveWorldspaceIcon();
		GameManager.LocalPlayerRegistered -= OnLocalPlayerRegistered;
		GameManager.RemotePlayerRegistered -= OnRemotePlayerRegistered;
		PlayerId.AnyPlayerGuidChanged -= OnAnyPlayerGuidChanged;
		CourseManager.PlayerKnockoutStreaksChanged -= OnPlayerKnockoutStreakChanged;
		PlayerSpectator.LocalPlayerIsSpectatingChanged -= OnLocalPlayerIsSpectatingChanged;
		PlayerSpectator.LocalPlayerSetSpectatingTarget -= OnLocalPlayerSetSpectatingTarget;
		PlayerSpectator.LocalPlayerStoppedSpectating -= OnLocalPlayerStoppedSpectating;
		if (puttingTrailVfx != null)
		{
			puttingTrailVfx.ReturnToPool();
			puttingTrailVfx = null;
		}
		if (respawnVfx != null)
		{
			respawnVfx.ReturnToPool();
			respawnVfx = null;
		}
		if (BNetworkManager.IsChangingSceneOrShuttingDown)
		{
			return;
		}
		if (Networkowner != null)
		{
			Networkowner.PlayerInfo.PlayerId.NameChanged -= OnOwnerNameChanged;
			Networkowner.PlayerInfo.Movement.IsVisibleChanged -= OnOwnerIsVisibleChanged;
			Networkowner.PlayerInfo.AsGolfer.MatchResolutionChanged -= OnOwnerMatchResolutionChanged;
			if (Networkowner.isLocalPlayer)
			{
				BallStatusMessage.Clear(forced: true);
			}
		}
		AsEntity.FinishedTemporarilyIgnoringCollisionsWith -= OnFinishedTemporarilyIgnoringCollisionsWith;
		AsEntity.AsHittable.WillApplyGolfSwingHitPhysics -= OnWillApplyGolfSwingHitPhysics;
		AsEntity.AsHittable.WillApplyItemHitPhysics -= OnWillApplyItemHitPhysics;
		AsEntity.AsHittable.WillApplyRocketLauncherBackBlastHitPhysics -= OnWillApplyRocketLauncherBackBlastHitPhysics;
		AsEntity.AsHittable.WillApplyRocketDriverSwingPostHitSpinHitPhysics -= OnWillApplyRocketDriverSwingPostHitSpinHitPhysics;
		AsEntity.AsHittable.WillApplyScoreKnockbackPhysics -= OnWillApplyScoreKnockbackPhysics;
		AsEntity.AsHittable.WillApplyJumpPadPhysics -= OnWillApplyJumpPadPhysics;
		AsEntity.AsHittable.WasHitByGolfSwing -= OnWasHitByGolfSwing;
		AsEntity.AsHittable.IsPlayingHomingWarningChanged -= OnIsPlayingHomingWarningChanged;
		AsEntity.AsHittable.AppliedPostHitBounce -= OnAppliedPostHitBounce;
		GetComponent<PlayerCosmeticsObjectSwitcher>().OnModelOverride -= OnModelOverride;
	}

	private void OnCollisionEnter(Collision collision)
	{
		if (!NetworkServer.active || collision.contactCount <= 0)
		{
			return;
		}
		int hitObjectLayerMask = 1 << collision.collider.gameObject.layer;
		if ((hitObjectLayerMask & (int)GameManager.LayerSettings.BallTrajectoryDeflectorMask) != 0)
		{
			serverLastStrokeTrajectoryDeflected = true;
		}
		Entity foundComponent2;
		if (collision.collider.attachedRigidbody != null)
		{
			if (collision.collider.attachedRigidbody.TryGetComponentInParent<Entity>(out var foundComponent, includeInactive: true) && foundComponent.IsTree)
			{
				serverLastStrokeTrajectoryDeflectedByTree = true;
			}
		}
		else if (collision.collider.TryGetComponentInParent<Entity>(out foundComponent2, includeInactive: true) && foundComponent2.IsTree)
		{
			serverLastStrokeTrajectoryDeflectedByTree = true;
		}
		if (ShouldBeReturnedFromOutOfBounds())
		{
			ServerReturnToBounds(suppressDisappearanceVfx: false);
		}
		bool ShouldBeReturnedFromOutOfBounds()
		{
			if (outOfBoundsReturnState != BallOutOfBoundsReturnState.None)
			{
				return false;
			}
			if (levelBoundsTracker.AuthoritativeBoundsState.HasState(BoundsState.OutOfBounds))
			{
				return true;
			}
			if ((hitObjectLayerMask & (int)GameManager.LayerSettings.BallGroundableMask) == 0)
			{
				return false;
			}
			if (BallOutOfBoundsTriggerManager.IsBallInOutOfBoundsTrigger(this))
			{
				return true;
			}
			return false;
		}
	}

	public void OnFixedBUpdate()
	{
		if (base.isServer)
		{
			ServerCheckOutOfBounds();
		}
		if (outOfBoundsReturnState != BallOutOfBoundsReturnState.None)
		{
			UpdateOutOfBoundsReturnAnimation();
			UpdateIsStationary();
		}
		else if (!isHidden)
		{
			UpdateTemporarilyIgnoredColliders();
			UpdateGroundingState();
			UpdateDownhillState();
			ApplyGravity();
			AsEntity.AsHittable.ApplySpinForce();
			ApplyLinearDamping();
			UpdateIsStationary();
		}
		void UpdateOutOfBoundsReturnAnimation()
		{
			if (!(Networkowner == null))
			{
				if (outOfBoundsReturnState == BallOutOfBoundsReturnState.AppearingOverHead || outOfBoundsReturnState == BallOutOfBoundsReturnState.HangingOverHead)
				{
					if (hasBallBeenMovedToHangOverHeadPosition)
					{
						hasBallBeenMovedToHangOverHeadPosition = false;
						PlayRespawnSound();
					}
					Vector3 position = Networkowner.PlayerInfo.HeadBone.position + GameManager.GolfSettings.BallReturnToBoundsDropOnHeadCurve[0].value * Vector3.up;
					if (!performedBallOutOfBoundsTeleport)
					{
						AsEntity.InformWillTeleport();
						AsEntity.Rigidbody.position = position;
						AsEntity.InformTeleported();
						performedBallOutOfBoundsTeleport = true;
						hasBallBeenMovedToHangOverHeadPosition = true;
					}
					else
					{
						rigidbody.MovePosition(position);
					}
				}
				else if (outOfBoundsReturnState == BallOutOfBoundsReturnState.DroppingOnHead)
				{
					float timeSince = BMath.GetTimeSince(returnToBoundsDropOnHeadStartTimestamp);
					Vector3 position2 = Networkowner.PlayerInfo.HeadBone.position + GameManager.GolfSettings.BallReturnToBoundsDropOnHeadCurve.Evaluate(timeSince) * Vector3.up;
					rigidbody.MovePosition(position2);
				}
			}
		}
	}

	public void OnLateBUpdate()
	{
		UpdateNotAllowedVisuals();
	}

	public void Initialize(PlayerGolfer owner)
	{
		Networkowner = owner;
	}

	public void ServerInformEnteredHole(GolfHole hole)
	{
		isInHole = true;
		ServerUpdateIsHidden();
		OnEnteredHole(hole);
		RpcInformEnteredHole(hole);
		if (Networkowner != null)
		{
			if (serverLastStrokeTrajectoryDeflected)
			{
				Networkowner.PlayerInfo.RpcInformBallEnteredHoleAfterTrajectoryDeflection();
			}
			if (serverLastStrokeTrajectoryDeflectedByTree || serverLastStrokeSlowedByFoliage)
			{
				Networkowner.PlayerInfo.RpcInformBallEnteredHoleAfterTrajectoryDeflectionByTreeOrFoliage();
			}
		}
	}

	public void ServerInformNoLongerInHole()
	{
		isInHole = false;
		ServerUpdateIsHidden();
	}

	public void InformStartedMovingInFoliage()
	{
		if (base.isServer)
		{
			serverLastStrokeSlowedByFoliage = true;
		}
	}

	[ClientRpc]
	private void RpcInformEnteredHole(GolfHole hole)
	{
		NetworkWriterPooled writer = NetworkWriterPool.Get();
		writer.WriteNetworkBehaviour(hole);
		SendRPCInternal("System.Void GolfBall::RpcInformEnteredHole(GolfHole)", 1534920083, writer, 0, includeOwner: true);
		NetworkWriterPool.Return(writer);
	}

	private void OnEnteredHole(GolfHole hole)
	{
		if (!(Networkowner == null))
		{
			Networkowner.InformScored(hole);
		}
	}

	public void OnRespawned()
	{
		if (base.isServer)
		{
			LastRespawnTimestamp = Time.timeAsDouble;
		}
	}

	public Bounds GetOrbitCameraSubjectLocalBounds()
	{
		return new Bounds(collider.center, collider.radius * Vector3.one);
	}

	public void OnWillTeleport()
	{
		if (puttingTrailVfx != null)
		{
			puttingTrailVfx.BeforeTeleport();
		}
	}

	public void OnTeleported()
	{
		canBeAffectedByWind = false;
		if (puttingTrailVfx != null)
		{
			puttingTrailVfx.AfterTeleport();
		}
	}

	private void ServerCheckOutOfBounds()
	{
		if (base.isServer && ShouldReturnToBounds(out var _))
		{
			ServerReturnToBounds(suppressDisappearanceVfx: false);
		}
		bool ShouldReturnToBounds(out bool reference)
		{
			reference = false;
			if (outOfBoundsReturnState != BallOutOfBoundsReturnState.None)
			{
				return false;
			}
			bool flag = (reference = levelBoundsTracker.AuthoritativeBoundsState.HasState(BoundsState.OutOfBounds));
			if (levelBoundsTracker.AuthoritativeBoundsState.IsInOutOfBoundsHazard())
			{
				return true;
			}
			if (!flag)
			{
				return false;
			}
			if (BMath.GetTimeSince(levelBoundsTracker.OutOfBoundsTimestamp) > GameManager.GolfSettings.BallMaxOutOfBoundsTime)
			{
				return true;
			}
			if (!IsGrounded)
			{
				return false;
			}
			return true;
		}
	}

	private void UpdateTemporarilyIgnoredColliders()
	{
		if (temporarilyIgnoredColliders.Count <= 0)
		{
			return;
		}
		HashSet<Collider> value;
		using (CollectionPool<HashSet<UnityEngine.Collider>, UnityEngine.Collider>.Get(out value))
		{
			value.UnionWith(temporarilyIgnoredColliders);
			int num = Physics.OverlapSphereNonAlloc(rigidbody.position, collider.radius, layerMask: GameManager.LayerSettings.BallTemporarilyIgnorableMask, results: PlayerGolfer.overlappingColliderBuffer, queryTriggerInteraction: QueryTriggerInteraction.Ignore);
			for (int i = 0; i < num; i++)
			{
				value.Remove(PlayerGolfer.overlappingColliderBuffer[i]);
			}
			foreach (Collider item in value)
			{
				Physics.IgnoreCollision(item, collider, ignore: false);
				temporarilyIgnoredColliders.Remove(item);
			}
		}
	}

	private void UpdateGroundingState()
	{
		bool isGrounded = this.IsGrounded;
		this.IsGrounded = IsGrounded(out var groundData);
		GroundData = groundData;
		if (isGrounded != this.IsGrounded)
		{
			GroundTimestamp = Time.timeAsDouble;
			UpdateFrictionMode();
		}
		bool CanGround()
		{
			if (outOfBoundsReturnState != BallOutOfBoundsReturnState.None)
			{
				return false;
			}
			if (AsEntity.GetNetworkedVelocity().y > 1f)
			{
				return false;
			}
			return true;
		}
		bool IsGrounded(out BallGroundData reference)
		{
			reference = default(BallGroundData);
			if (!CanGround())
			{
				return false;
			}
			Ray ray = new Ray(rigidbody.position, Vector3.down);
			float num = collider.radius * 0.95f;
			float num2 = collider.radius * 1.1f;
			int num3 = Physics.SphereCastNonAlloc(ray, num, maxDistance: num2, layerMask: GameManager.LayerSettings.BallGroundableMask, results: raycastHitBuffer, queryTriggerInteraction: QueryTriggerInteraction.Ignore);
			if (num3 <= 0)
			{
				if (drawBallGroundingDebug)
				{
					BDebug.DrawSphereCast(ray, num, num2, Color.red);
				}
				return false;
			}
			RaycastHit raycastHit = new RaycastHit
			{
				distance = float.MaxValue
			};
			for (int i = 0; i < num3; i++)
			{
				RaycastHit raycastHit2 = raycastHitBuffer[i];
				if (!(raycastHit2.collider == null) && !(raycastHit2.distance >= raycastHit.distance))
				{
					raycastHit = raycastHit2;
				}
			}
			if (raycastHit.distance == 0f)
			{
				ray.origin -= ray.direction * num;
				num2 = collider.radius * 3f;
				if (!raycastHit.collider.Raycast(ray, out var hitInfo, num2))
				{
					if (drawBallGroundingDebug)
					{
						BDebug.DrawLine(ray.origin, ray.origin + ray.direction * num2, Color.red);
					}
					return false;
				}
				raycastHit = hitInfo;
				if (drawBallGroundingDebug)
				{
					BDebug.DrawLine(ray.origin, hitInfo.point, Color.red);
				}
			}
			if (drawBallGroundingDebug)
			{
				BDebug.DrawSphereCast(ray, num, raycastHit.distance, Color.green);
			}
			reference.point = raycastHit.point;
			reference.normal = raycastHit.normal;
			TerrainAddition foundComponent;
			if (raycastHit.collider is TerrainCollider)
			{
				reference.groundTerrainType = GroundTerrainType.Terrain;
				reference.terrainDominantGlobalLayer = TerrainManager.GetDominantGlobalLayerAtPoint(base.transform.position);
			}
			else if (raycastHit.collider.TryGetComponentInParent<TerrainAddition>(out foundComponent, includeInactive: true))
			{
				reference.groundTerrainType = GroundTerrainType.TerrainAddition;
				reference.terrainDominantGlobalLayer = foundComponent.TerrainLayer;
			}
			else
			{
				reference.groundTerrainType = GroundTerrainType.NotTerrain;
				reference.terrainDominantGlobalLayer = (TerrainLayer)(-1);
			}
			return true;
		}
	}

	private void UpdateDownhillState()
	{
		if (base.isServer)
		{
			if (!IsRollingDownhill())
			{
				NetworkrollingDownhillTime = 0f;
			}
			else
			{
				NetworkrollingDownhillTime = rollingDownhillTime + Time.deltaTime;
			}
		}
		bool IsRollingDownhill()
		{
			if (!IsGrounded)
			{
				return false;
			}
			return rigidbody.linearVelocity.y < -0.005f;
		}
	}

	private void ApplyGravity()
	{
		rigidbody.linearVelocity += GetGravity() * Time.fixedDeltaTime;
		Vector3 GetGravity()
		{
			if (!TryGetSpecialGravityRotation(out var rotation))
			{
				return Physics.gravity;
			}
			return rotation * Physics.gravity;
		}
		bool TryGetSpecialGravityRotation(out Quaternion rotation)
		{
			rotation = default(Quaternion);
			if (!IsGrounded)
			{
				return false;
			}
			if (fullStopFactor <= 0f)
			{
				return false;
			}
			float num = ((GroundData.groundTerrainType == GroundTerrainType.NotTerrain) ? GameManager.GolfBallSettings.GroundFullStopMaxPitch : TerrainManager.Settings.LayerSettings[GroundData.terrainDominantGlobalLayer].FullStopMaxPitch);
			if (90f + GroundData.normal.GetPitchDeg() > num)
			{
				return false;
			}
			rotation = Quaternion.FromToRotation(Physics.gravity, -GroundData.normal);
			return true;
		}
	}

	private void ApplyLinearDamping()
	{
		fullStopFactor = 0f;
		if (IsGrounded)
		{
			ApplyGroundDamping();
		}
		else
		{
			AsEntity.AsHittable.ApplyAirDamping(GameManager.GolfBallSettings.LinearAirDragFactor, GameManager.GolfBallSettings.RocketDriverSwingLinearAirDragFactor, canBeAffectedByWind);
		}
		void ApplyGroundDamping()
		{
			Vector3 velocityAlongGround = rigidbody.linearVelocity.ProjectOnPlane(GroundData.normal);
			float num = GetDamping();
			Vector3 vector = velocityAlongGround * BMath.Max(0f, 1f - num * Time.fixedDeltaTime);
			rigidbody.linearVelocity += vector - velocityAlongGround;
			float GetDamping()
			{
				float num2 = 90f + GroundData.normal.GetPitchDeg();
				float num3;
				float num4;
				if (GroundData.groundTerrainType != GroundTerrainType.NotTerrain)
				{
					TerrainLayerSettings terrainLayerSettings = TerrainManager.Settings.LayerSettings[GroundData.terrainDominantGlobalLayer];
					num3 = terrainLayerSettings.FullStopMaxPitch;
					num4 = terrainLayerSettings.FullRollMinPitch;
				}
				else
				{
					num3 = GameManager.GolfBallSettings.GroundFullStopMaxPitch;
					num4 = GameManager.GolfBallSettings.GroundFullRollMinPitch;
				}
				if (velocityAlongGround.y < 0.5f && num2 >= num4)
				{
					fullStopFactor = 0f;
					return 0f;
				}
				float sqrMagnitude = velocityAlongGround.sqrMagnitude;
				float num5 = BMath.RemapClamped(GameManager.GolfBallSettings.FullStopRollingDownhillStartTime, GameManager.GolfBallSettings.FullStopRollingDownhillEndTime, 1f, GameManager.GolfBallSettings.FullStopRollingDownhillEndDampingSpeedFactor, rollingDownhillTime);
				float num6 = num5 * GameManager.GolfBallSettings.FullStopMaxDampingSpeed;
				if (num2 < num3 && sqrMagnitude < num6 * num6)
				{
					fullStopFactor = 1f;
					return GameManager.GolfBallSettings.FullStopLinearDamping;
				}
				float num7;
				AnimationCurve animationCurve;
				if (GroundData.groundTerrainType != GroundTerrainType.NotTerrain)
				{
					TerrainLayerSettings terrainLayerSettings2 = TerrainManager.Settings.LayerSettings[GroundData.terrainDominantGlobalLayer];
					num7 = terrainLayerSettings2.LinearDamping;
					animationCurve = terrainLayerSettings2.BallFullStopToFullRollCurve;
				}
				else
				{
					num7 = GameManager.GolfBallSettings.GroundFrictionLinearDamping;
					animationCurve = GameManager.GolfBallSettings.GroundFullStopToFullRollCurve;
				}
				float num8 = BMath.RemapClamped(0f, 90f, 1f, 0f, num2, BMath.EaseIn) * num7;
				float num9 = num5 * GameManager.GolfBallSettings.FullStopMinDampingSpeed;
				if (sqrMagnitude >= num9 * num9)
				{
					fullStopFactor = 0f;
					return num8;
				}
				float value = BMath.Sqrt(sqrMagnitude);
				fullStopFactor = BMath.InverseLerp(num9, num6, value);
				fullStopFactor *= animationCurve.Evaluate(BMath.InverseLerpClamped(num3, num4, num2));
				return BMath.Lerp(num8, GameManager.GolfBallSettings.FullStopLinearDamping, fullStopFactor);
			}
		}
	}

	private void Unground()
	{
		bool isGrounded = IsGrounded;
		IsGrounded = false;
		if (isGrounded != IsGrounded)
		{
			UpdateFrictionMode();
			UpdateIsStationary();
		}
	}

	[Server]
	private void ServerReturnToBounds(bool suppressDisappearanceVfx)
	{
		if (!NetworkServer.active)
		{
			Debug.LogWarning("[Server] function 'System.Void GolfBall::ServerReturnToBounds(System.Boolean)' called when server was not active");
			return;
		}
		if (CourseManager.MatchState >= MatchState.Overtime)
		{
			AsEntity.DestroyEntity();
			return;
		}
		if (returnToBoundsRoutine != null)
		{
			StopCoroutine(returnToBoundsRoutine);
		}
		Networkowner.PlayerInfo.AsHittable.ServerStopBeingSwingProjectile();
		CourseManager.AddPenaltyStroke(Networkowner, suppressPopup: false);
		if (!suppressDisappearanceVfx)
		{
			PlayDisappearanceVfx();
		}
		returnToBoundsRoutine = StartCoroutine(ReturnToBoundsRoutine());
		void PlayDisappearanceVfx()
		{
			VfxType vfxType;
			Quaternion rotation;
			if (AsEntity.LevelBoundsTracker.AuthoritativeBoundsState.HasState(BoundsState.InMainOutOfBoundsHazard))
			{
				vfxType = MainOutOfBoundsHazard.Type switch
				{
					OutOfBoundsHazard.Water => VfxType.WaterItemOutOfBounds, 
					OutOfBoundsHazard.Fog => VfxType.FogItemOutOfBounds, 
					_ => VfxType.None, 
				};
				rotation = Quaternion.identity;
			}
			else if (AsEntity.LevelBoundsTracker.AuthoritativeBoundsState.HasState(BoundsState.InSecondaryOutOfBoundsHazard))
			{
				vfxType = AsEntity.LevelBoundsTracker.CurrentSecondaryHazardLocalOnly.Type switch
				{
					OutOfBoundsHazard.Water => VfxType.WaterItemOutOfBounds, 
					OutOfBoundsHazard.Fog => VfxType.FogItemOutOfBounds, 
					_ => VfxType.None, 
				};
				rotation = Quaternion.identity;
			}
			else
			{
				vfxType = VfxType.BoundaryBallOutOfBounds;
				rotation = Quaternion.LookRotation(IsGrounded ? GroundData.normal : Vector3.up);
			}
			if (vfxType != VfxType.None)
			{
				Vector3 worldCenterOfMass = rigidbody.worldCenterOfMass;
				worldCenterOfMass.y = AsEntity.LevelBoundsTracker.CurrentOutOfBoundsHazardWorldHeightLocalOnly;
				if (vfxType == VfxType.WaterItemOutOfBounds)
				{
					ServerPlayWaterSplashForAllClients(worldCenterOfMass);
				}
				VfxManager.ServerPlayPooledVfxForAllClients(vfxType, worldCenterOfMass, rotation);
			}
		}
	}

	[Server]
	private void ServerPlayWaterSplashForAllClients(Vector3 worldPosition)
	{
		if (!NetworkServer.active)
		{
			Debug.LogWarning("[Server] function 'System.Void GolfBall::ServerPlayWaterSplashForAllClients(UnityEngine.Vector3)' called when server was not active");
			return;
		}
		PlayWaterSplashInternal(worldPosition);
		foreach (NetworkConnectionToClient value in NetworkServer.connections.Values)
		{
			if (value != NetworkServer.localConnection)
			{
				RpcPlayWaterSplash(value, worldPosition);
			}
		}
	}

	[TargetRpc]
	private void RpcPlayWaterSplash(NetworkConnectionToClient connection, Vector3 worldPosition)
	{
		NetworkWriterPooled writer = NetworkWriterPool.Get();
		writer.WriteVector3(worldPosition);
		SendTargetRPCInternal(connection, "System.Void GolfBall::RpcPlayWaterSplash(Mirror.NetworkConnectionToClient,UnityEngine.Vector3)", 1886952913, writer, 0);
		NetworkWriterPool.Return(writer);
	}

	private void PlayWaterSplashInternal(Vector3 worldPosition)
	{
		VfxManager.PlayPooledVfxLocalOnly(VfxType.WaterImpactSmall, worldPosition, Quaternion.identity);
		RuntimeManager.PlayOneShot(GameManager.AudioSettings.ItemWaterSplash, worldPosition);
	}

	private IEnumerator ReturnToBoundsRoutine()
	{
		if (Networkowner == null)
		{
			levelBoundsTracker.ServerReturnToBounds();
			ServerSetOutOfBoundsReturnState(BallOutOfBoundsReturnState.None);
			yield break;
		}
		ServerSetOutOfBoundsReturnState(BallOutOfBoundsReturnState.AppearingOverHead);
		bool waitUntilInBounds = Networkowner.PlayerInfo.LevelBoundsTracker.AuthoritativeBoundsState.HasState(BoundsState.OutOfBounds);
		bool waitUntilOverLand = Networkowner.PlayerInfo.LevelBoundsTracker.IsInOrOverOutOfBoundsHazard();
		float hangTimer = ((waitUntilInBounds || waitUntilOverLand) ? GameManager.GolfSettings.BallReturnToBoundsHangOverHeadPostReturnToBoundsDuration : GameManager.GolfSettings.BallReturnToBoundsHangOverHeadDefaultDuration);
		float hangOverWaterTime = 0f;
		float hangTime = 0f;
		while (hangTimer > 0f)
		{
			yield return null;
			if (!(Networkowner == null))
			{
				UpdateHangTimer();
				continue;
			}
			ServerSetOutOfBoundsReturnState(BallOutOfBoundsReturnState.None);
			yield break;
		}
		ServerSetOutOfBoundsReturnState(BallOutOfBoundsReturnState.DroppingOnHead);
		float dropTime = 0f;
		while (dropTime < GameManager.GolfSettings.BallReturnToBoundsDropOnHeadCurve[GameManager.GolfSettings.BallReturnToBoundsDropOnHeadCurve.length - 1].time)
		{
			yield return null;
			if (Networkowner == null)
			{
				ServerSetOutOfBoundsReturnState(BallOutOfBoundsReturnState.None);
				yield break;
			}
			if (!Networkowner.PlayerInfo.Movement.IsRespawning)
			{
				dropTime += Time.deltaTime;
			}
		}
		ServerSetOutOfBoundsReturnState(BallOutOfBoundsReturnState.None);
		Networkowner.PlayerInfo.AsHittable.ServerHitWithReturnedBall();
		AsEntity.AsHittable.ServerApplyPostHitBounce(-Networkowner.transform.forward);
		void UpdateHangTimer()
		{
			hangTime += Time.deltaTime;
			if (hangTime >= GameManager.GolfSettings.BallReturnToBoundsHangOverHeadDefaultDuration)
			{
				ServerSetOutOfBoundsReturnState(BallOutOfBoundsReturnState.HangingOverHead);
			}
			if (Networkowner.PlayerInfo.Movement.IsRespawning)
			{
				waitUntilInBounds = false;
				waitUntilOverLand = false;
				hangTimer = GameManager.GolfSettings.BallReturnToBoundsHangOverHeadDefaultDuration;
			}
			else
			{
				if (waitUntilInBounds)
				{
					if (Networkowner.PlayerInfo.LevelBoundsTracker.AuthoritativeBoundsState.HasState(BoundsState.OutOfBounds))
					{
						hangTimer = GameManager.GolfSettings.BallReturnToBoundsHangOverHeadPostReturnToBoundsDuration;
						return;
					}
					waitUntilInBounds = false;
				}
				if (waitUntilOverLand)
				{
					if (Networkowner.PlayerInfo.LevelBoundsTracker.IsInOrOverOutOfBoundsHazard())
					{
						if (!Networkowner.PlayerInfo.Movement.IsKnockedOutOrRecovering)
						{
							hangOverWaterTime += Time.deltaTime;
						}
						if (hangOverWaterTime >= GameManager.GolfSettings.BallReturnToBoundsHangOverHeadOverWaterTimeout)
						{
							hangTimer = 0f;
						}
						else
						{
							hangTimer = GameManager.GolfSettings.BallReturnToBoundsHangOverHeadPostReturnToBoundsDuration;
						}
						return;
					}
					waitUntilOverLand = false;
				}
				hangTimer -= Time.deltaTime;
			}
		}
	}

	[Server]
	private void ServerSetOutOfBoundsReturnState(BallOutOfBoundsReturnState state)
	{
		if (!NetworkServer.active)
		{
			Debug.LogWarning("[Server] function 'System.Void GolfBall::ServerSetOutOfBoundsReturnState(BallOutOfBoundsReturnState)' called when server was not active");
			return;
		}
		NetworkoutOfBoundsReturnState = state;
		ServerUpdateIsHidden();
	}

	private void UpdateFrictionMode(bool forced = false)
	{
		FrictionMode frictionMode = this.frictionMode;
		this.frictionMode = GetCurrentFrictionMode();
		if (forced || frictionMode != this.frictionMode)
		{
			ApplyFrictionMode();
		}
		void ApplyFrictionMode()
		{
			switch (this.frictionMode)
			{
			case FrictionMode.Air:
				rigidbody.angularDamping = GameManager.GolfBallSettings.AirFrictionAngularDamping;
				break;
			case FrictionMode.Ground:
				rigidbody.angularDamping = GameManager.GolfBallSettings.GroundFrictionAngularDamping;
				break;
			}
		}
		FrictionMode GetCurrentFrictionMode()
		{
			if (IsGrounded)
			{
				return FrictionMode.Ground;
			}
			return FrictionMode.Air;
		}
	}

	private void UpdatePhysics()
	{
		bool flag = outOfBoundsReturnState != BallOutOfBoundsReturnState.None;
		bool flag2 = !isHidden && !flag;
		rigidbody.isKinematic = !flag2;
		collider.enabled = flag2;
		AsEntity.PredictedRigidbody.suppressCorrections = flag;
	}

	[Server]
	private void ServerUpdateIsHidden()
	{
		if (!NetworkServer.active)
		{
			Debug.LogWarning("[Server] function 'System.Void GolfBall::ServerUpdateIsHidden()' called when server was not active");
		}
		else
		{
			NetworkisHidden = ShouldBeHidden();
		}
		bool ShouldBeHidden()
		{
			if (outOfBoundsReturnState == BallOutOfBoundsReturnState.AppearingOverHead)
			{
				return true;
			}
			if (isInHole)
			{
				return true;
			}
			if (Networkowner != null && !Networkowner.PlayerInfo.Movement.IsVisible && outOfBoundsReturnState != BallOutOfBoundsReturnState.None)
			{
				return true;
			}
			return false;
		}
	}

	private void UpdateNameTagEnabled()
	{
		if (Networkowner == null)
		{
			RemoveNameTag();
			return;
		}
		bool flag = nameTag != null;
		bool flag2 = ShouldBeEnabled();
		if (!flag && flag2)
		{
			InitializeNewNameTag();
		}
		else if (flag && !flag2)
		{
			RemoveNameTag();
		}
		void InitializeNewNameTag()
		{
			nameTag = NameTagManager.GetUnusedNameTag();
			nameTag.Initialize(NameTagManager.BallNameTagSettings, base.transform, GameManager.UiSettings.BallNameTagLocalOffset, GameManager.UiSettings.BallNameTagWorldOffset, string.Format(Localization.UI.BALL_OwnerName, Networkowner.PlayerInfo.PlayerId.PlayerNameNoRichText), Networkowner.PlayerInfo, nameTagIsPlayer: false);
		}
		bool ShouldBeEnabled()
		{
			if (Networkowner.isLocalPlayer)
			{
				return false;
			}
			if (!Networkowner.PlayerInfo.Movement.IsVisible)
			{
				return false;
			}
			if (isHidden)
			{
				return false;
			}
			return true;
		}
	}

	private void UpdateNameTag()
	{
		if (!(nameTag == null))
		{
			nameTag.SetName(string.Format(Localization.UI.BALL_OwnerName, Networkowner.PlayerInfo.PlayerId.PlayerNameNoRichText));
		}
	}

	private void RemoveNameTag()
	{
		if (!(nameTag == null))
		{
			NameTagManager.ReturnNameTag(nameTag);
			nameTag = null;
		}
	}

	private void UpdateWorldspaceIconEnabled()
	{
		if (Networkowner == null)
		{
			RemoveWorldspaceIcon();
			return;
		}
		bool isLocalPlayerSpectating = GameManager.LocalPlayerAsSpectator != null && GameManager.LocalPlayerAsSpectator.IsSpectating;
		bool flag = worldspaceIcon != null;
		bool flag2 = ShouldBeEnabled();
		if (flag2 == flag)
		{
			if (flag2 && isLocalPlayerSpectating && worldspaceIcon != null)
			{
				worldspaceIcon.SetDistanceReference(GetWorldspaceIconDistanceReference());
			}
		}
		else if (flag2)
		{
			InitializeNewWorldspaceIcon();
		}
		else
		{
			RemoveWorldspaceIcon();
		}
		Transform GetWorldspaceIconDistanceReference()
		{
			if (!isLocalPlayerSpectating)
			{
				return GameManager.LocalPlayerAsGolfer.transform;
			}
			return GameManager.LocalPlayerAsSpectator.Target;
		}
		void InitializeNewWorldspaceIcon()
		{
			worldspaceIcon = WorldspaceIconManager.GetUnusedIcon();
			worldspaceIcon.Initialize(WorldspaceIconManager.BallIconSettings, base.transform, GetWorldspaceIconDistanceReference(), WorldspaceIconManager.BallIcon);
		}
		bool ShouldBeEnabled()
		{
			if (GameManager.LocalPlayerInfo == null)
			{
				return false;
			}
			if (!Networkowner.isLocalPlayer && !isLocalPlayerSpectating)
			{
				return false;
			}
			if (Networkowner.IsMatchResolved && !Networkowner.PlayerInfo.Movement.IsVisible)
			{
				return false;
			}
			if (isHidden)
			{
				return false;
			}
			if (AsEntity.AsHittable.IsPlayingHomingWarning)
			{
				return false;
			}
			if (isLocalPlayerSpectating && GameManager.LocalPlayerAsSpectator.TargetPlayer != Networkowner.PlayerInfo)
			{
				return false;
			}
			return true;
		}
	}

	private void RemoveWorldspaceIcon()
	{
		if (!(worldspaceIcon == null))
		{
			WorldspaceIconManager.ReturnIcon(worldspaceIcon);
			worldspaceIcon = null;
		}
	}

	private void UpdateIsStationary()
	{
		bool isStationary = IsStationary;
		IsStationary = ShouldBeStationary();
		if (IsStationary)
		{
			canBeAffectedByWind = false;
		}
		if (IsStationary != isStationary)
		{
			base.gameObject.SetLayerRecursively(IsStationary ? GameManager.LayerSettings.StationaryBallLayer : GameManager.LayerSettings.DynamicBallLayer);
			if (Networkowner.isLocalPlayer)
			{
				GolfBall.LocalPlayerBallIsStationaryChanged?.Invoke();
			}
		}
		bool ShouldBeStationary()
		{
			if (!IsGrounded)
			{
				return false;
			}
			if (rigidbody.linearVelocity.sqrMagnitude > GameManager.GolfBallSettings.StationaryStateSpeedThresholdSquared)
			{
				return false;
			}
			return true;
		}
	}

	private void UpdateCanPassThrough(PlayerInfo player)
	{
		if (!(player == null))
		{
			SetCanPassThrough(ShouldPassThrough());
		}
		void SetCanPassThrough(bool canPassThrough)
		{
			Physics.IgnoreCollision(collider, player.Movement.UprightCollider, canPassThrough);
			Physics.IgnoreCollision(collider, player.Movement.DivingCollider, canPassThrough);
			Physics.IgnoreCollision(collider, player.Movement.HittableCollider, canPassThrough);
		}
		bool ShouldPassThrough()
		{
			if (Networkowner == null)
			{
				return false;
			}
			return player.Movement.IsKnockoutProtectedFromPlayer(Networkowner.PlayerInfo);
		}
	}

	private void UpdateNotAllowedVisuals(bool forced = false)
	{
		PlayerInfo viewedPlayer = GameManager.GetViewedOrLocalPlayer();
		bool flag = isDisplayingNotAllowedVisuals;
		isDisplayingNotAllowedVisuals = ShouldDisplayNotAllowedVisuals();
		if (!forced && isDisplayingNotAllowedVisuals == flag)
		{
			return;
		}
		for (int i = 0; i < meshRenderers.Count; i++)
		{
			MeshRenderer meshRenderer = meshRenderers[i];
			if (meshRenderer != null)
			{
				meshRenderer.sharedMaterial = (isDisplayingNotAllowedVisuals ? GameManager.GolfBallSettings.NotAllowedMaterial : meshRendererOriginalMaterials[i]);
			}
		}
		bool ShouldDisplayNotAllowedVisuals()
		{
			if (viewedPlayer == null || !enableNotAllowedVisuals)
			{
				return false;
			}
			if (Networkowner == viewedPlayer.AsGolfer)
			{
				return ShouldOwnedBallDisplayNotAllowedVisuals();
			}
			return ShouldUnownedBallDisplayNotAllowedVisuals();
		}
		bool ShouldOwnedBallDisplayNotAllowedVisuals()
		{
			if (!IsStationary)
			{
				return false;
			}
			if (!viewedPlayer.Inventory.IsAimingItemNetworked())
			{
				return false;
			}
			if (!GameManager.AllItems.TryGetItemData(viewedPlayer.Inventory.GetEffectivelyEquippedItem(), out var itemData))
			{
				return false;
			}
			if (!itemData.CanUsageAffectBalls)
			{
				return false;
			}
			return true;
		}
		bool ShouldUnownedBallDisplayNotAllowedVisuals()
		{
			if (!MatchSetupRules.GetValueAsBool(MatchSetupRules.Rule.HitOtherPlayersBalls) && IsStationary)
			{
				return true;
			}
			if (!viewedPlayer.isLocalPlayer && viewedPlayer.AsGolfer.IsAimingSwingNetworked())
			{
				return true;
			}
			if (!viewedPlayer.Inventory.IsAimingItemNetworked())
			{
				return false;
			}
			if (!GameManager.AllItems.TryGetItemData(viewedPlayer.Inventory.GetEffectivelyEquippedItem(), out var itemData))
			{
				return false;
			}
			if (!itemData.CanUsageAffectBalls)
			{
				return true;
			}
			return false;
		}
	}

	private void OnWasHitByGolfSwing(PlayerGolfer hitter, Vector3 worldDirection, float power, bool isRocketDriver)
	{
		if (base.isServer)
		{
			ServerLastStrokePosition = base.transform.position;
			serverLastStrokeTrajectoryDeflected = false;
			serverLastStrokeTrajectoryDeflectedByTree = false;
			serverLastStrokeSlowedByFoliage = AsEntity.IsMovingInFoliage;
		}
		canBeAffectedByWind = true;
		if (vfxSettings != null)
		{
			if (isRocketDriver)
			{
				VfxManager.PlayPooledVfxLocalOnly(VfxType.RocketDriverRegularHit, base.transform.position, Quaternion.LookRotation(worldDirection));
			}
			else
			{
				VfxManager.PlayPooledVfxLocalOnly(vfxSettings.Launch, base.transform.position, Quaternion.identity);
			}
		}
	}

	private void OnIsPlayingHomingWarningChanged()
	{
		UpdateWorldspaceIconEnabled();
	}

	private void OnAppliedPostHitBounce()
	{
		canBeAffectedByWind = false;
	}

	private void OnServerMatchStateChanged(MatchState previousState, MatchState currentState)
	{
		if (currentState == MatchState.Ended)
		{
			AsEntity.DestroyEntity();
		}
	}

	private void OnLocalPlayerRegistered()
	{
		UpdateWorldspaceIconEnabled();
		UpdateCanPassThrough(GameManager.LocalPlayerInfo);
	}

	private void OnRemotePlayerRegistered(PlayerInfo remotePlayer)
	{
		UpdateCanPassThrough(remotePlayer);
	}

	private void OnAnyPlayerGuidChanged(PlayerId playerId)
	{
		UpdateCanPassThrough(playerId.PlayerInfo);
	}

	private void OnPlayerKnockoutStreakChanged(SyncIDictionary<CourseManager.PlayerPair, CourseManager.KnockoutStreak>.Operation operation, CourseManager.PlayerPair playerPair, CourseManager.KnockoutStreak streak)
	{
		if (GameManager.TryFindPlayerByGuid(playerPair.playerAGuid, out var playerInfo) && !(playerInfo == null) && !(playerInfo.AsGolfer != Networkowner) && GameManager.TryFindPlayerByGuid(playerPair.playerBGuid, out var playerInfo2) && !(playerInfo2 == null))
		{
			UpdateCanPassThrough(playerInfo2);
		}
	}

	private void OnLocalPlayerIsSpectatingChanged()
	{
		UpdateWorldspaceIconEnabled();
	}

	private void OnLocalPlayerSetSpectatingTarget(bool isInitialTarget)
	{
		UpdateWorldspaceIconEnabled();
	}

	private void OnLocalPlayerStoppedSpectating()
	{
		UpdateWorldspaceIconEnabled();
	}

	private void OnFinishedTemporarilyIgnoringCollisionsWith(Entity otherEntity)
	{
		if (otherEntity.IsPlayer)
		{
			UpdateCanPassThrough(otherEntity.PlayerInfo);
		}
	}

	private void OnWillApplyGolfSwingHitPhysics(PlayerGolfer hitter, float power, Vector3 localHitPosition, Vector3 localOrigin, Vector3 incomingVelocityChange, bool isRocketDriver)
	{
		Unground();
	}

	private void OnWillApplyItemHitPhysics(PlayerInventory itemUser, ItemType itemType, ItemUseId itemUseId, Vector3 localOrigin, float distance, Vector3 incomingVelocityChange, bool isReflected, bool isInSpecialState)
	{
		Unground();
		if (base.isServer)
		{
			serverLastStrokeTrajectoryDeflected = true;
		}
	}

	private void OnWillApplyRocketLauncherBackBlastHitPhysics(PlayerInventory rocketLauncherUser, Vector3 hitLocalPosition, Vector3 localOrigin, Vector3 incomingVelocityChange)
	{
		Unground();
		if (base.isServer)
		{
			serverLastStrokeTrajectoryDeflected = true;
		}
	}

	private void OnWillApplyRocketDriverSwingPostHitSpinHitPhysics(PlayerGolfer hitter, Vector3 hitLocalPosition, Vector3 localOrigin, Vector3 incomingVelocityChange)
	{
		Unground();
		if (base.isServer)
		{
			serverLastStrokeTrajectoryDeflected = true;
		}
	}

	private void OnWillApplyScoreKnockbackPhysics()
	{
		Unground();
		if (base.isServer)
		{
			serverLastStrokeTrajectoryDeflected = true;
		}
	}

	private void OnWillApplyJumpPadPhysics()
	{
		Unground();
		if (base.isServer)
		{
			serverLastStrokeTrajectoryDeflected = true;
		}
	}

	private void OnIsHiddenChanged(bool wasHidden, bool isHidden)
	{
		bool flag = Networkowner != null && Networkowner == GameManager.LocalPlayerAsGolfer;
		Renderer[] array = renderers;
		foreach (Renderer renderer in array)
		{
			if (renderer == null)
			{
				Debug.LogError("Found null in ball renderers", base.gameObject);
			}
			else if (!(renderer is ParticleSystemRenderer))
			{
				renderer.enabled = !isHidden;
			}
		}
		UpdatePhysics();
		if (flag && isHidden)
		{
			BallStatusMessage.Clear(forced: true);
		}
		UpdateNameTagEnabled();
		UpdateWorldspaceIconEnabled();
		if (base.isServer && isHidden && outOfBoundsReturnState > BallOutOfBoundsReturnState.AppearingOverHead)
		{
			ServerReturnToBounds(suppressDisappearanceVfx: true);
		}
		if (flag)
		{
			GolfBall.LocalPlayerBallIsHiddenChanged?.Invoke();
		}
		this.IsHiddenChanged?.Invoke();
		this.IsHiddenChangedReferenced?.Invoke(this);
	}

	private void OnOwnerNameChanged()
	{
		UpdateNameTag();
	}

	private void OnOwnerIsVisibleChanged()
	{
		UpdateNameTagEnabled();
		UpdateWorldspaceIconEnabled();
		if (base.isServer)
		{
			ServerUpdateIsHidden();
			if (isHidden && outOfBoundsReturnState > BallOutOfBoundsReturnState.AppearingOverHead)
			{
				ServerReturnToBounds(suppressDisappearanceVfx: true);
			}
		}
	}

	private void OnOwnerMatchResolutionChanged(PlayerMatchResolution previousResolution, PlayerMatchResolution currentResolution)
	{
		UpdateWorldspaceIconEnabled();
	}

	private void OnOwnerChanged(PlayerGolfer previousOwner, PlayerGolfer currentOwner)
	{
		if (previousOwner != null)
		{
			previousOwner.PlayerInfo.PlayerId.NameChanged -= OnOwnerNameChanged;
			previousOwner.PlayerInfo.Movement.IsVisibleChanged -= OnOwnerIsVisibleChanged;
			previousOwner.PlayerInfo.AsGolfer.MatchResolutionChanged -= OnOwnerMatchResolutionChanged;
		}
		if (currentOwner != null)
		{
			currentOwner.PlayerInfo.PlayerId.NameChanged += OnOwnerNameChanged;
			currentOwner.PlayerInfo.Movement.IsVisibleChanged += OnOwnerIsVisibleChanged;
			currentOwner.PlayerInfo.AsGolfer.MatchResolutionChanged += OnOwnerMatchResolutionChanged;
		}
		UpdateCanPassThrough(GameManager.LocalPlayerInfo);
		foreach (PlayerInfo remotePlayer in GameManager.RemotePlayers)
		{
			UpdateCanPassThrough(remotePlayer);
		}
		UpdateNameTagEnabled();
		UpdateNameTag();
		UpdateWorldspaceIconEnabled();
	}

	private void OnModelOverride()
	{
		UpdateRenderers();
		UpdateNotAllowedVisuals(forced: true);
	}

	private void OnOutOfBoundsReturnStateChanged(BallOutOfBoundsReturnState previousState, BallOutOfBoundsReturnState currentState)
	{
		bool flag = previousState != BallOutOfBoundsReturnState.None;
		bool flag2 = outOfBoundsReturnState != BallOutOfBoundsReturnState.None;
		if (flag2 != flag)
		{
			UpdatePhysics();
			if (flag2)
			{
				if (base.isServer)
				{
					AsEntity.AsHittable.ServerStopBeingSwingProjectile();
				}
			}
			else
			{
				EnsureNoOverlapWithEnvironment();
			}
		}
		if (outOfBoundsReturnState == BallOutOfBoundsReturnState.AppearingOverHead)
		{
			performedBallOutOfBoundsTeleport = false;
			PlayRespawnVfx();
		}
		else if (outOfBoundsReturnState == BallOutOfBoundsReturnState.DroppingOnHead)
		{
			returnToBoundsDropOnHeadStartTimestamp = Time.timeAsDouble;
		}
		if (!isHidden && Networkowner == GameManager.LocalPlayerAsGolfer && outOfBoundsReturnState == BallOutOfBoundsReturnState.HangingOverHead)
		{
			BallStatusMessage.SetReturned();
		}
		this.OutOfBoundsReturnStateChanged?.Invoke();
		void EnsureNoOverlapWithEnvironment()
		{
			Vector3 position = rigidbody.position;
			for (int i = 0; i < 10; i++)
			{
				int num = Physics.OverlapSphereNonAlloc(rigidbody.position, this.collider.radius, layerMask: GameManager.LayerSettings.BallGroundableMask, results: PlayerGolfer.overlappingColliderBuffer, queryTriggerInteraction: QueryTriggerInteraction.Ignore);
				if (num > 0)
				{
					int num2 = BMath.Min(num, 10);
					float num3 = 0f;
					Vector3 vector = default(Vector3);
					for (int j = 0; j < num2; j++)
					{
						Collider collider = PlayerGolfer.overlappingColliderBuffer[j];
						if (Physics.ComputePenetration(this.collider, rigidbody.position, rigidbody.rotation, collider, collider.transform.position, collider.transform.rotation, out var direction, out var distance) && !(distance <= num3))
						{
							num3 = distance;
							vector = direction;
						}
					}
					if (num3 <= 0f)
					{
						break;
					}
					position += num3 * vector;
				}
			}
			int num4 = Physics.OverlapSphereNonAlloc(rigidbody.position, this.collider.radius, layerMask: GameManager.LayerSettings.BallTemporarilyIgnorableMask, results: PlayerGolfer.overlappingColliderBuffer, queryTriggerInteraction: QueryTriggerInteraction.Ignore);
			for (int k = 0; k < num4; k++)
			{
				Collider collider2 = PlayerGolfer.overlappingColliderBuffer[k];
				Physics.IgnoreCollision(collider2, this.collider, ignore: true);
				temporarilyIgnoredColliders.Add(collider2);
			}
		}
		void PlayRespawnVfx()
		{
			if (!VfxPersistentData.TryGetPooledVfx(VfxType.BallRespawn, out respawnVfx))
			{
				Debug.LogError("Failed to get ball respawn VFX", base.gameObject);
			}
			else
			{
				respawnVfx.transform.SetParent(base.transform);
				respawnVfx.transform.SetLocalPositionAndRotation(Vector3.zero, Quaternion.identity);
				respawnVfx.Play();
			}
		}
	}

	private void PlayRespawnSound()
	{
		RuntimeManager.PlayOneShot(GameManager.AudioSettings.BallRespawnEvent, base.transform.position);
	}

	private void DrawOnGuiDebug()
	{
		if (!(GameManager.LocalPlayerMovement == null))
		{
			Vector3 position = base.transform.position;
			Vector3 position2 = GameManager.LocalPlayerMovement.transform.position;
			Vector3 vector = new Vector3(position.x, position2.y, position.z);
			bool skipLabel = BMath.Abs((position - position2).GetPitchDeg()) < 10f;
			DrawLineAndLabel(position2, position, 1f);
			DrawLineAndLabel(position2, vector, 0.5f, skipLabel);
			DrawLineAndLabel(vector, position, 0.5f, skipLabel);
		}
		static void DrawLineAndLabel(Vector3 from, Vector3 to, float alpha, bool flag = false)
		{
			Color red = Color.red;
			red.a = alpha;
			BDebug.DrawLine(from, to, red);
			if (!flag)
			{
				float magnitude = (to - from).magnitude;
				Vector3 s = new Vector3((float)Screen.width / 1280f, (float)Screen.height / 720f, 1f);
				Vector3 a = GameManager.Camera.WorldToViewportPoint(from);
				Vector3 b = GameManager.Camera.WorldToViewportPoint(to);
				Vector3 vector2 = BMath.Average(a, b);
				if (!(vector2.z < 0f))
				{
					vector2.y = 1f - vector2.y;
					float num = 60f;
					float num2 = 25f;
					Rect screenRect = new Rect((float)Screen.width / s.x * vector2.x - num / 2f, (float)Screen.height / s.y * vector2.y - num2 / 2f, num, num2);
					Matrix4x4 matrix = GUI.matrix;
					GUI.matrix = Matrix4x4.TRS(new Vector3(0f, 0f, 0f), Quaternion.identity, s);
					GUIStyle centerAlignedDebugTextStyle = BDebug.CenterAlignedDebugTextStyle;
					GUILayout.BeginArea(screenRect);
					GUILayout.BeginVertical("box");
					GUILayout.Label($"{magnitude:0.00} m", centerAlignedDebugTextStyle);
					GUILayout.EndVertical();
					GUILayout.EndArea();
					GUI.matrix = matrix;
				}
			}
		}
	}

	public GolfBall()
	{
		_Mirror_SyncVarHookDelegate_owner = OnOwnerChanged;
		_Mirror_SyncVarHookDelegate_outOfBoundsReturnState = OnOutOfBoundsReturnStateChanged;
		_Mirror_SyncVarHookDelegate_isHidden = OnIsHiddenChanged;
	}

	static GolfBall()
	{
		enableNotAllowedVisuals = true;
		RemoteProcedureCalls.RegisterRpc(typeof(GolfBall), "System.Void GolfBall::RpcInformEnteredHole(GolfHole)", InvokeUserCode_RpcInformEnteredHole__GolfHole);
		RemoteProcedureCalls.RegisterRpc(typeof(GolfBall), "System.Void GolfBall::RpcPlayWaterSplash(Mirror.NetworkConnectionToClient,UnityEngine.Vector3)", InvokeUserCode_RpcPlayWaterSplash__NetworkConnectionToClient__Vector3);
	}

	public override bool Weaved()
	{
		return true;
	}

	protected void UserCode_RpcInformEnteredHole__GolfHole(GolfHole hole)
	{
		if (!base.isServer)
		{
			OnEnteredHole(hole);
		}
	}

	protected static void InvokeUserCode_RpcInformEnteredHole__GolfHole(NetworkBehaviour obj, NetworkReader reader, NetworkConnectionToClient senderConnection)
	{
		if (!NetworkClient.active)
		{
			Debug.LogError("RPC RpcInformEnteredHole called on server.");
		}
		else
		{
			((GolfBall)obj).UserCode_RpcInformEnteredHole__GolfHole(reader.ReadNetworkBehaviour<GolfHole>());
		}
	}

	protected void UserCode_RpcPlayWaterSplash__NetworkConnectionToClient__Vector3(NetworkConnectionToClient connection, Vector3 worldPosition)
	{
		PlayWaterSplashInternal(worldPosition);
	}

	protected static void InvokeUserCode_RpcPlayWaterSplash__NetworkConnectionToClient__Vector3(NetworkBehaviour obj, NetworkReader reader, NetworkConnectionToClient senderConnection)
	{
		if (!NetworkClient.active)
		{
			Debug.LogError("TargetRPC RpcPlayWaterSplash called on server.");
		}
		else
		{
			((GolfBall)obj).UserCode_RpcPlayWaterSplash__NetworkConnectionToClient__Vector3(null, reader.ReadVector3());
		}
	}

	public override void SerializeSyncVars(NetworkWriter writer, bool forceAll)
	{
		base.SerializeSyncVars(writer, forceAll);
		if (forceAll)
		{
			writer.WriteNetworkBehaviour(Networkowner);
			writer.WriteFloat(rollingDownhillTime);
			GeneratedNetworkCode._Write_BallOutOfBoundsReturnState(writer, outOfBoundsReturnState);
			writer.WriteBool(isHidden);
			return;
		}
		writer.WriteVarULong(syncVarDirtyBits);
		if ((syncVarDirtyBits & 1L) != 0L)
		{
			writer.WriteNetworkBehaviour(Networkowner);
		}
		if ((syncVarDirtyBits & 2L) != 0L)
		{
			writer.WriteFloat(rollingDownhillTime);
		}
		if ((syncVarDirtyBits & 4L) != 0L)
		{
			GeneratedNetworkCode._Write_BallOutOfBoundsReturnState(writer, outOfBoundsReturnState);
		}
		if ((syncVarDirtyBits & 8L) != 0L)
		{
			writer.WriteBool(isHidden);
		}
	}

	public override void DeserializeSyncVars(NetworkReader reader, bool initialState)
	{
		base.DeserializeSyncVars(reader, initialState);
		if (initialState)
		{
			GeneratedSyncVarDeserialize_NetworkBehaviour(ref owner, _Mirror_SyncVarHookDelegate_owner, reader, ref ___ownerNetId);
			GeneratedSyncVarDeserialize(ref rollingDownhillTime, null, reader.ReadFloat());
			GeneratedSyncVarDeserialize(ref outOfBoundsReturnState, _Mirror_SyncVarHookDelegate_outOfBoundsReturnState, GeneratedNetworkCode._Read_BallOutOfBoundsReturnState(reader));
			GeneratedSyncVarDeserialize(ref isHidden, _Mirror_SyncVarHookDelegate_isHidden, reader.ReadBool());
			return;
		}
		long num = (long)reader.ReadVarULong();
		if ((num & 1L) != 0L)
		{
			GeneratedSyncVarDeserialize_NetworkBehaviour(ref owner, _Mirror_SyncVarHookDelegate_owner, reader, ref ___ownerNetId);
		}
		if ((num & 2L) != 0L)
		{
			GeneratedSyncVarDeserialize(ref rollingDownhillTime, null, reader.ReadFloat());
		}
		if ((num & 4L) != 0L)
		{
			GeneratedSyncVarDeserialize(ref outOfBoundsReturnState, _Mirror_SyncVarHookDelegate_outOfBoundsReturnState, GeneratedNetworkCode._Read_BallOutOfBoundsReturnState(reader));
		}
		if ((num & 8L) != 0L)
		{
			GeneratedSyncVarDeserialize(ref isHidden, _Mirror_SyncVarHookDelegate_isHidden, reader.ReadBool());
		}
	}
}
