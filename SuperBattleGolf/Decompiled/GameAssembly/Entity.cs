using System;
using System.Collections;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using FMOD.Studio;
using FMODUnity;
using Mirror;
using UnityEngine;

public class Entity : MonoBehaviour
{
	private double lastMovementInFoliageTimestamp;

	private EventInstance movingInFoliageLoopSoundInstance;

	private bool isPlayingFoliageSound;

	[field: SerializeField]
	public bool IsTree { get; private set; }

	public Rigidbody Rigidbody { get; private set; }

	public NetworkRigidbodyUnreliable NetworkRigidbody { get; private set; }

	public PredictedRigidbody PredictedRigidbody { get; private set; }

	public PredictedGolfCart PredictedGolfCart { get; private set; }

	public Hittable AsHittable { get; private set; }

	public LevelBoundsTracker LevelBoundsTracker { get; private set; }

	public LockOnTarget AsLockOnTarget { get; private set; }

	public TargetReticlePosition TargetReticlePosition { get; private set; }

	public PlayerInfo PlayerInfo { get; private set; }

	public GolfTee AsGolfTee { get; private set; }

	public GolfBall AsGolfBall { get; private set; }

	public GolfCartInfo AsGolfCart { get; private set; }

	public GolfHole AsGolfHole { get; private set; }

	public PhysicalItem AsItem { get; private set; }

	public JumpPad AsJumpPad { get; private set; }

	public bool IsDestroyed { get; private set; }

	public bool IsMovingInFoliage { get; private set; }

	public HashSet<Entity> TemporarilyIgnoredEntities { get; private set; }

	public bool HasRigidbody => Rigidbody != null;

	public bool IsHittable => AsHittable != null;

	public bool HasLevelBoundsTracker => LevelBoundsTracker != null;

	public bool IsLockOnTarget => AsLockOnTarget != null;

	public bool HasTargetReticlePosition => TargetReticlePosition != null;

	public bool IsPlayer => PlayerInfo != null;

	public bool IsGolfTee => AsGolfTee != null;

	public bool IsGolfBall => AsGolfBall != null;

	public bool IsGolfCart => AsGolfCart != null;

	public bool IsGolfHole => AsGolfHole != null;

	public bool IsItem => AsItem != null;

	public bool IsJumpPad => AsJumpPad != null;

	public bool IsPredicted
	{
		get
		{
			if (!(PredictedRigidbody != null))
			{
				return PredictedGolfCart != null;
			}
			return true;
		}
	}

	public event Action<Entity> FinishedTemporarilyIgnoringCollisionsWith;

	public event Action WillBeDestroyed;

	public event Action<Entity> WillBeDestroyedReferenced;

	private void Awake()
	{
		Rigidbody = GetComponent<Rigidbody>();
		NetworkRigidbody = GetComponent<NetworkRigidbodyUnreliable>();
		PredictedRigidbody = GetComponent<PredictedRigidbody>();
		TargetReticlePosition = GetComponentInChildren<TargetReticlePosition>();
		AsHittable = GetComponent<Hittable>();
		LevelBoundsTracker = GetComponent<LevelBoundsTracker>();
		AsLockOnTarget = GetComponent<LockOnTarget>();
		GetMutuallyExclusiveReferences();
		void GetMutuallyExclusiveReferences()
		{
			PlayerInfo = GetComponent<PlayerInfo>();
			if (!IsPlayer)
			{
				AsGolfBall = GetComponent<GolfBall>();
				if (!IsGolfBall)
				{
					AsGolfCart = GetComponent<GolfCartInfo>();
					if (IsGolfCart)
					{
						PredictedGolfCart = GetComponent<PredictedGolfCart>();
					}
					else
					{
						AsGolfTee = GetComponent<GolfTee>();
						if (!IsGolfTee)
						{
							AsGolfHole = GetComponent<GolfHole>();
							if (!IsGolfHole)
							{
								AsItem = GetComponent<PhysicalItem>();
								if (!IsItem)
								{
									AsJumpPad = GetComponent<JumpPad>();
								}
							}
						}
					}
				}
			}
		}
	}

	private void OnDestroy()
	{
		if (movingInFoliageLoopSoundInstance.isValid())
		{
			movingInFoliageLoopSoundInstance.stop(FMOD.Studio.STOP_MODE.ALLOWFADEOUT);
		}
		OnWillBeDestroyed();
	}

	public void InformInFoliage()
	{
		if (!IsSimulatingRigidbody())
		{
			return;
		}
		if (TryGetLinearDragFactor(out var dragFactor))
		{
			float sqrMagnitude = Rigidbody.linearVelocity.sqrMagnitude;
			float num = dragFactor * sqrMagnitude;
			Rigidbody.linearVelocity *= BMath.Max(0f, 1f - num * Time.fixedDeltaTime);
		}
		if (Rigidbody.linearVelocity.sqrMagnitude >= AudioSettings.MinMovementInFoliageSpeedSquared)
		{
			lastMovementInFoliageTimestamp = Time.timeAsDouble;
			if (!IsMovingInFoliage)
			{
				StartCoroutine(MoveInFoliageRoutine());
			}
		}
		IEnumerator MoveInFoliageRoutine()
		{
			SetIsMovingInFoliage(isMoving: true);
			while (BMath.GetTimeSince(lastMovementInFoliageTimestamp) < GameManager.AudioSettings.MovementInFoliageExitDelay)
			{
				yield return null;
			}
			SetIsMovingInFoliage(isMoving: false);
		}
		void SetIsMovingInFoliage(bool isMoving)
		{
			bool isMovingInFoliage = IsMovingInFoliage;
			IsMovingInFoliage = isMoving;
			if (IsPlayer)
			{
				if (isMoving)
				{
					PlayerInfo.InformIsMovingInFoliage();
				}
				else if (isMovingInFoliage)
				{
					PlayerInfo.InformNoLongerMovingInFoliage();
				}
			}
			else
			{
				if (IsMovingInFoliage != isMovingInFoliage && IsGolfBall && IsMovingInFoliage)
				{
					AsGolfBall.InformStartedMovingInFoliage();
				}
				if (isMoving)
				{
					PlayOrUpdateFoliageSoundLocalOnly();
				}
				else if (isMovingInFoliage)
				{
					StopFoliageSoundLocalOnly();
				}
			}
		}
		bool TryGetLinearDragFactor(out float reference)
		{
			reference = 0f;
			if (IsPlayer)
			{
				return false;
			}
			if (IsGolfCart)
			{
				return false;
			}
			if (IsGolfBall)
			{
				reference = GameManager.GolfBallSettings.LinearFoliageDragFactor;
			}
			else if (IsItem)
			{
				reference = PhysicsManager.Settings.ItemLinearFoliageDragFactor;
			}
			else
			{
				reference = PhysicsManager.Settings.DefaultLinearFoliageDragFactor;
			}
			return true;
		}
	}

	public bool IsGrounded()
	{
		if (IsPlayer)
		{
			return PlayerInfo.Movement.IsGrounded;
		}
		if (IsGolfBall)
		{
			return AsGolfBall.IsGrounded;
		}
		return false;
	}

	public Vector3 GetTargetReticleWorldPosition()
	{
		if (TargetReticlePosition == null)
		{
			return base.transform.position;
		}
		return TargetReticlePosition.transform.position;
	}

	public Vector3 GetNetworkedVelocity()
	{
		if (!HasRigidbody)
		{
			return Vector3.zero;
		}
		if (IsSimulatingRigidbody())
		{
			return Rigidbody.linearVelocity;
		}
		return NetworkRigidbody.velocity;
	}

	public Vector3 GetNetworkedAngularVelocity()
	{
		if (!HasRigidbody)
		{
			return Vector3.zero;
		}
		if (IsSimulatingRigidbody())
		{
			return Rigidbody.angularVelocity;
		}
		return NetworkRigidbody.angularVelocity;
	}

	public Vector3 GetNetworkedPointVelocity(Vector3 worldPoint)
	{
		if (!HasRigidbody)
		{
			return Vector3.zero;
		}
		if (IsSimulatingRigidbody())
		{
			return Rigidbody.GetPointVelocity(worldPoint);
		}
		return worldPoint.GetPointVelocity(Rigidbody.worldCenterOfMass, NetworkRigidbody.velocity, NetworkRigidbody.angularVelocity);
	}

	public bool IsSimulatingRigidbody()
	{
		if (PredictedRigidbody != null)
		{
			return true;
		}
		if (PredictedGolfCart != null)
		{
			return true;
		}
		return HasRigidbodyAuthority();
	}

	public bool HasRigidbodyAuthority()
	{
		if (NetworkRigidbody == null)
		{
			return false;
		}
		if (NetworkRigidbody.isServer && NetworkRigidbody.isClient)
		{
			if (NetworkRigidbody.syncDirection != SyncDirection.ServerToClient)
			{
				if (NetworkRigidbody.isClient)
				{
					return NetworkRigidbody.isOwned;
				}
				return false;
			}
			return true;
		}
		if (NetworkRigidbody.isServer)
		{
			return NetworkRigidbody.syncDirection == SyncDirection.ServerToClient;
		}
		if (NetworkRigidbody.isClient)
		{
			if (NetworkRigidbody.isClient)
			{
				return NetworkRigidbody.isOwned;
			}
			return false;
		}
		return false;
	}

	public async void TemporarilyIgnoreCollisionsWith(Entity entity, float duration, bool includeOwnTriggers = false)
	{
		IEnumerable<Collider> otherColliders;
		if (entity.HasRigidbody)
		{
			otherColliders = entity.Rigidbody.GetAttachedColliders();
		}
		else
		{
			otherColliders = entity.GetComponentsInChildren<Collider>(includeInactive: true);
		}
		List<Collider> ownColliders;
		if (HasRigidbody)
		{
			ownColliders = Rigidbody.GetAttachedColliders(includeInactive: false, includeOwnTriggers);
		}
		else
		{
			ownColliders = new List<Collider>();
			Collider[] componentsInChildren = GetComponentsInChildren<Collider>(includeInactive: true);
			foreach (Collider collider in componentsInChildren)
			{
				if (!collider.isTrigger || includeOwnTriggers)
				{
					ownColliders.Add(collider);
				}
			}
		}
		if (TemporarilyIgnoredEntities == null)
		{
			TemporarilyIgnoredEntities = new HashSet<Entity>();
		}
		TemporarilyIgnoredEntities.Add(entity);
		entity.WillBeDestroyed += OnIgnoredEntityWillBeDestroyed;
		foreach (Collider item in ownColliders)
		{
			foreach (Collider item2 in otherColliders)
			{
				Physics.IgnoreCollision(item, item2, ignore: true);
			}
		}
		await UniTask.WaitForSeconds(duration);
		if (!(this == null))
		{
			FinishIgnoringEntity();
		}
		void FinishIgnoringEntity()
		{
			TemporarilyIgnoredEntities.Remove(entity);
			entity.WillBeDestroyed -= OnIgnoredEntityWillBeDestroyed;
			if (!(entity == null))
			{
				if (!entity.IsDestroyed)
				{
					foreach (Collider item3 in ownColliders)
					{
						foreach (Collider item4 in otherColliders)
						{
							if (item3 != null && item4 != null)
							{
								Physics.IgnoreCollision(item3, item4, ignore: false);
							}
						}
					}
				}
				this.FinishedTemporarilyIgnoringCollisionsWith?.Invoke(entity);
				entity.FinishedTemporarilyIgnoringCollisionsWith?.Invoke(this);
			}
		}
		void OnIgnoredEntityWillBeDestroyed()
		{
			FinishIgnoringEntity();
		}
	}

	public void InformWillTeleport()
	{
		if (IsHittable)
		{
			AsHittable.OnWillTeleport();
		}
		if (IsGolfBall)
		{
			AsGolfBall.OnWillTeleport();
		}
	}

	public void InformTeleported()
	{
		if (IsHittable)
		{
			AsHittable.OnTeleported();
		}
		if (IsGolfBall)
		{
			AsGolfBall.OnTeleported();
		}
	}

	public void DestroyEntity()
	{
		if (!IsDestroyed)
		{
			OnWillBeDestroyed();
			UnityEngine.Object.Destroy(base.gameObject);
		}
	}

	public void InformClientSceneChange()
	{
		OnWillBeDestroyed();
	}

	private void PlayOrUpdateFoliageSoundLocalOnly()
	{
		float value = BMath.InverseLerpClamped(GameManager.AudioSettings.MinMovementInFoliageSpeed, GameManager.AudioSettings.MaxMovementInFoliageSpeed, Rigidbody.linearVelocity.magnitude);
		if (isPlayingFoliageSound)
		{
			movingInFoliageLoopSoundInstance.setParameterByID(AudioSettings.VelocityId, value);
			return;
		}
		isPlayingFoliageSound = true;
		EventInstance instance = RuntimeManager.CreateInstance(GameManager.AudioSettings.FoliageImpactEvent);
		RuntimeManager.AttachInstanceToGameObject(instance, base.gameObject);
		instance.setParameterByID(AudioSettings.VelocityId, value);
		instance.start();
		instance.release();
		movingInFoliageLoopSoundInstance = RuntimeManager.CreateInstance(GameManager.AudioSettings.FoliageLoopEvent);
		RuntimeManager.AttachInstanceToGameObject(movingInFoliageLoopSoundInstance, base.gameObject);
		movingInFoliageLoopSoundInstance.setParameterByID(AudioSettings.VelocityId, value);
		movingInFoliageLoopSoundInstance.start();
		movingInFoliageLoopSoundInstance.release();
	}

	private void StopFoliageSoundLocalOnly()
	{
		if (isPlayingFoliageSound)
		{
			isPlayingFoliageSound = false;
			float num = BMath.InverseLerpClamped(GameManager.AudioSettings.MinMovementInFoliageSpeed, GameManager.AudioSettings.MaxMovementInFoliageSpeed, Rigidbody.linearVelocity.magnitude);
			EventInstance instance = RuntimeManager.CreateInstance(GameManager.AudioSettings.FoliageImpactEvent);
			RuntimeManager.AttachInstanceToGameObject(instance, base.gameObject);
			instance.setParameterByID(AudioSettings.VelocityId, num * 0.25f);
			instance.start();
			instance.release();
			if (movingInFoliageLoopSoundInstance.isValid())
			{
				movingInFoliageLoopSoundInstance.stop(FMOD.Studio.STOP_MODE.ALLOWFADEOUT);
			}
		}
	}

	private void OnWillBeDestroyed()
	{
		if (IsDestroyed)
		{
			return;
		}
		IsDestroyed = true;
		if (IsHittable)
		{
			AsHittable.OnWillBeDestroyed();
		}
		if (IsPlayer)
		{
			PlayerInfo.OnWillBeDestroyed();
		}
		else if (IsGolfBall)
		{
			AsGolfBall.OnWillBeDestroyed();
		}
		else if (IsGolfHole)
		{
			AsGolfHole.OnWillBeDestroyed();
		}
		else if (IsGolfCart)
		{
			AsGolfCart.OnWillBeDestroyed();
		}
		try
		{
			this.WillBeDestroyed?.Invoke();
			this.WillBeDestroyedReferenced?.Invoke(this);
		}
		catch (Exception exception)
		{
			Debug.LogError("An exception was thrown while invoking " + base.name + "'s WillBeDestroyed method; see the next log for details", base.gameObject);
			Debug.LogException(exception, base.gameObject);
		}
	}
}
