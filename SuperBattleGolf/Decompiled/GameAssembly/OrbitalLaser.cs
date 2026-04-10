#define DEBUG_DRAW
using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using FMOD.Studio;
using FMODUnity;
using Mirror;
using UnityEngine;
using UnityEngine.Pool;

public class OrbitalLaser : NetworkBehaviour, IBUpdateCallback, IAnyBUpdateCallback, IFixedBUpdateCallback
{
	[SyncVar(hook = "OnStateChanged")]
	private OrbitalLaserState state;

	[SyncVar]
	private Vector3 targetPosition;

	private HashSet<Hittable> hitHittableBuffer;

	[SyncVar]
	private Hittable target;

	private PlayerInventory owner;

	private ItemUseId itemUseId;

	private double activationTimestamp;

	private PoolableParticleSystem anticipationVfx;

	private PoolableParticleSystem explosionVfx;

	private PoolableParticleSystem explosionEndVfx;

	private EventInstance anticipationSoundInstance;

	private EventInstance explosionSoundInstance;

	[CVar("drawOrbitalLaserDebug", "", "", false, true)]
	private static bool drawOrbitalLaserDebug;

	protected NetworkBehaviourSyncVar ___targetNetId;

	public Action<OrbitalLaserState, OrbitalLaserState> _Mirror_SyncVarHookDelegate_state;

	public OrbitalLaserState Networkstate
	{
		get
		{
			return state;
		}
		[param: In]
		set
		{
			GeneratedSyncVarSetter(value, ref state, 1uL, _Mirror_SyncVarHookDelegate_state);
		}
	}

	public Vector3 NetworktargetPosition
	{
		get
		{
			return targetPosition;
		}
		[param: In]
		set
		{
			GeneratedSyncVarSetter(value, ref targetPosition, 2uL, null);
		}
	}

	public Hittable Networktarget
	{
		get
		{
			return GetSyncVarNetworkBehaviour(___targetNetId, ref target);
		}
		[param: In]
		set
		{
			GeneratedSyncVarSetter_NetworkBehaviour(value, ref target, 4uL, null, ref ___targetNetId);
		}
	}

	private void OnDestroy()
	{
		if (BNetworkManager.IsChangingSceneOrShuttingDown)
		{
			if (anticipationSoundInstance.isValid())
			{
				anticipationSoundInstance.stop(FMOD.Studio.STOP_MODE.IMMEDIATE);
			}
			if (explosionSoundInstance.isValid())
			{
				explosionSoundInstance.stop(FMOD.Studio.STOP_MODE.IMMEDIATE);
			}
		}
	}

	public override void OnStartServer()
	{
		if (!base.isClient)
		{
			BUpdate.RegisterCallback(this);
		}
		hitHittableBuffer = CollectionPool<HashSet<Hittable>, Hittable>.Get();
	}

	public override void OnStopServer()
	{
		if (!base.isClient)
		{
			BUpdate.DeregisterCallback(this);
		}
		CollectionPool<HashSet<Hittable>, Hittable>.Release(hitHittableBuffer);
		ReturnAllVfxToPool();
	}

	public override void OnStartClient()
	{
		BUpdate.RegisterCallback(this);
		Vector3 position = ((Networktarget != null) ? Networktarget.transform.position : targetPosition);
		base.transform.position = SnapHeight(position);
	}

	public override void OnStopClient()
	{
		BUpdate.DeregisterCallback(this);
		ReturnAllVfxToPool();
	}

	public void OnBUpdate()
	{
		if (!base.isServer)
		{
			UpdatePosition();
			return;
		}
		float timeSinceActivation = BMath.GetTimeSince(activationTimestamp);
		switch (state)
		{
		case OrbitalLaserState.AnticipationFollow:
			ServerUpdateAnticipationFollow();
			break;
		case OrbitalLaserState.AnticipationStationary:
			ServerUpdateAnticipationStationary();
			break;
		case OrbitalLaserState.Exploding:
			ServerUpdateExploding();
			break;
		case OrbitalLaserState.Ending:
			ServerUpdateEnding();
			break;
		}
		UpdatePosition();
		void ServerUpdateAnticipationFollow()
		{
			if (timeSinceActivation >= GameManager.ItemSettings.OrbitalLaserActivationStopFollowTime || !OrbitalLaserManager.CanTarget(Networktarget))
			{
				Networktarget = null;
				Networkstate = OrbitalLaserState.AnticipationStationary;
			}
			else
			{
				NetworktargetPosition = Networktarget.transform.position;
			}
		}
		void ServerUpdateAnticipationStationary()
		{
			if (timeSinceActivation >= GameManager.ItemSettings.OrbitalLaserAnticipationDuration)
			{
				Networkstate = OrbitalLaserState.Exploding;
			}
		}
		void ServerUpdateEnding()
		{
			if (timeSinceActivation >= GameManager.ItemSettings.OrbitalLaserEndingEndTime)
			{
				NetworkServer.Destroy(base.gameObject);
			}
		}
		void ServerUpdateExploding()
		{
			if (timeSinceActivation >= GameManager.ItemSettings.OrbitalLaserExplosionEndTime)
			{
				Networkstate = OrbitalLaserState.Ending;
			}
		}
		void UpdatePosition()
		{
			Vector3 position = ((Networktarget != null) ? Networktarget.transform.position : targetPosition);
			position = SnapHeight(position);
			Vector3 position2 = Vector3.Lerp(base.transform.position, position, GameManager.ItemSettings.OrbitalLaserAnticipationPositionSmoothing * Time.deltaTime);
			position2.y = position.y;
			base.transform.position = position2;
			if (drawOrbitalLaserDebug)
			{
				BDebug.DrawWireSphere(base.transform.position, 0.1f, Color.red);
			}
		}
	}

	public void OnFixedBUpdate()
	{
		if (base.isServer && state == OrbitalLaserState.Exploding)
		{
			Vector3 position = base.transform.position;
			Vector3 vector = base.transform.position + GameManager.ItemSettings.OrbitalLaserExplosionLaserHeight * Vector3.up;
			int num = Physics.OverlapCapsuleNonAlloc(position, vector, GameManager.ItemSettings.OrbitalLaserExplosionLaserRadius, layerMask: GameManager.LayerSettings.OrbitalLaserHittablesMask, results: PlayerGolfer.overlappingColliderBuffer, queryTriggerInteraction: QueryTriggerInteraction.Ignore);
			for (int i = 0; i < num; i++)
			{
				ParseHitCollider(PlayerGolfer.overlappingColliderBuffer[i], hitByLaser: true);
			}
			num = Physics.OverlapSphereNonAlloc(base.transform.position, GameManager.ItemSettings.OrbitalLaserExplosionMaxRange + GameManager.Achievements.DangerCloseDistanceFromExplosion, layerMask: GameManager.LayerSettings.OrbitalLaserHittablesMask, results: PlayerGolfer.overlappingColliderBuffer, queryTriggerInteraction: QueryTriggerInteraction.Ignore);
			for (int j = 0; j < num; j++)
			{
				ParseHitCollider(PlayerGolfer.overlappingColliderBuffer[j], hitByLaser: false);
			}
			if (drawOrbitalLaserDebug)
			{
				BDebug.DrawWireSphere(base.transform.position, GameManager.ItemSettings.OrbitalLaserExplosionMaxRange, Color.magenta, 0f, drawInsideLines: true);
				BDebug.DrawWireCapsule(position, vector, GameManager.ItemSettings.OrbitalLaserExplosionLaserRadius, Color.magenta);
			}
		}
		void ParseHitCollider(Collider hitCollider, bool hitByLaser)
		{
			if (TryGetHittable(hitCollider, out var hittable) && hitHittableBuffer.Add(hittable))
			{
				if (!hittable.TryGetClosestPointOnAllActiveColliders(base.transform.position, out var closestPoint, out var distanceSquared))
				{
					distanceSquared = 0f;
					closestPoint = ((!hittable.AsEntity.HasRigidbody) ? hittable.transform.position : hittable.AsEntity.Rigidbody.worldCenterOfMass);
				}
				if (distanceSquared > GameManager.ItemSettings.OrbitalLaserExplosionMaxRangeSquared)
				{
					if (!hitByLaser && hittable.AsEntity.IsPlayer)
					{
						hittable.AsEntity.PlayerInfo.RpcInformOfOrbitalLaserCloseCall();
					}
				}
				else
				{
					Vector3 direction = closestPoint - base.transform.position;
					float num2 = (hitByLaser ? 0f : BMath.Sqrt(distanceSquared));
					if (num2 <= GameManager.ItemSettings.OrbitalLaserExplosionMaxRange)
					{
						hittable.HitWithItem(ItemType.OrbitalLaser, itemUseId, hittable.transform.InverseTransformPoint(closestPoint), direction, hittable.transform.InverseTransformPoint(base.transform.position), num2, owner, isReflected: false, isInSpecialState: false, canHitWithNoUser: true);
					}
					if (drawOrbitalLaserDebug)
					{
						BDebug.DrawLine(base.transform.position, hittable.transform.position, Color.cyan, 2f);
					}
				}
			}
		}
		static bool TryGetHittable(Collider collider, out Hittable hittable)
		{
			if (collider.attachedRigidbody != null)
			{
				return collider.attachedRigidbody.TryGetComponentInParent<Hittable>(out hittable, includeInactive: true);
			}
			return collider.TryGetComponentInParent<Hittable>(out hittable, includeInactive: true);
		}
	}

	[Server]
	public void ServerActivate(Hittable target, Vector3 fallbackWorldPosition, PlayerInventory owner, ItemUseId itemUseId)
	{
		if (!NetworkServer.active)
		{
			Debug.LogWarning("[Server] function 'System.Void OrbitalLaser::ServerActivate(Hittable,UnityEngine.Vector3,PlayerInventory,ItemUseId)' called when server was not active");
			return;
		}
		Networktarget = target;
		this.owner = owner;
		this.itemUseId = itemUseId;
		activationTimestamp = Time.timeAsDouble;
		if (target != null)
		{
			Networkstate = OrbitalLaserState.AnticipationFollow;
			NetworktargetPosition = target.transform.position;
		}
		else
		{
			Networkstate = OrbitalLaserState.AnticipationStationary;
			NetworktargetPosition = fallbackWorldPosition;
		}
	}

	private Vector3 SnapHeight(Vector3 position)
	{
		Ray ray = new Ray(position + 0.1f * Vector3.up, Vector3.down);
		RaycastHit hitInfo;
		bool flag = Physics.Raycast(ray, out hitInfo, 500f, GameManager.LayerSettings.OrbitalLaserHeightSnappingMask, QueryTriggerInteraction.Ignore);
		if (flag)
		{
			position.y = BMath.Min(position.y, hitInfo.point.y);
		}
		if (drawOrbitalLaserDebug)
		{
			BDebug.DrawLine(ray.origin, flag ? hitInfo.point : (ray.origin + ray.direction * 500f), flag ? Color.green : Color.red);
		}
		return position;
	}

	private void ReturnAllVfxToPool()
	{
		if (anticipationVfx != null)
		{
			anticipationVfx.ReturnToPool();
		}
		if (explosionVfx != null)
		{
			explosionVfx.ReturnToPool();
		}
		if (explosionEndVfx != null)
		{
			explosionEndVfx.ReturnToPool();
		}
	}

	private void OnAnticipationVfxReturnedToPool()
	{
		anticipationVfx = null;
	}

	private void OnExplosionVfxReturnedToPool()
	{
		explosionVfx = null;
	}

	private void OnStateChanged(OrbitalLaserState previousState, OrbitalLaserState currentState)
	{
		switch (state)
		{
		case OrbitalLaserState.AnticipationFollow:
		case OrbitalLaserState.AnticipationStationary:
			if (previousState != OrbitalLaserState.AnticipationFollow && previousState != OrbitalLaserState.AnticipationStationary)
			{
				StartAnticipation();
			}
			break;
		case OrbitalLaserState.Exploding:
			StartExploding();
			break;
		case OrbitalLaserState.Ending:
			StartEnding();
			break;
		}
		void StartAnticipation()
		{
			if (!VfxPersistentData.TryGetPooledVfx(VfxType.OrbitalLaserAnticipation, out anticipationVfx))
			{
				Debug.LogError("Failed to get orbital laser anticipation VFX");
			}
			else
			{
				anticipationVfx.ReturnedToPool += OnAnticipationVfxReturnedToPool;
				anticipationVfx.transform.SetParent(base.transform);
				anticipationVfx.transform.SetLocalPositionAndRotation(Vector3.zero, Quaternion.identity);
				anticipationVfx.Play();
				anticipationSoundInstance = RuntimeManager.CreateInstance(GameManager.AudioSettings.OrbitalLaserAnticipationEvent);
				RuntimeManager.AttachInstanceToGameObject(anticipationSoundInstance, base.gameObject);
				anticipationSoundInstance.start();
				anticipationSoundInstance.release();
			}
		}
		void StartEnding()
		{
			if (!VfxPersistentData.TryGetPooledVfx(VfxType.OrbitalLaserEnd, out explosionEndVfx))
			{
				Debug.LogError("Failed to get orbital laser explosion VFX");
			}
			else
			{
				explosionEndVfx.ReturnedToPool += OnExplosionVfxReturnedToPool;
				explosionEndVfx.transform.SetParent(base.transform);
				explosionEndVfx.transform.SetLocalPositionAndRotation(Vector3.zero, Quaternion.identity);
				explosionEndVfx.Play();
				if (GameSettings.All.General.FlashingEffects)
				{
					CameraModuleController.PlayImpactFrame(base.transform.position);
				}
			}
		}
		void StartExploding()
		{
			if (!VfxPersistentData.TryGetPooledVfx(VfxType.OrbitalLaserExplosion, out explosionVfx))
			{
				Debug.LogError("Failed to get orbital laser explosion VFX");
			}
			else
			{
				explosionVfx.ReturnedToPool += OnExplosionVfxReturnedToPool;
				explosionVfx.transform.SetParent(base.transform);
				explosionVfx.transform.SetLocalPositionAndRotation(Vector3.zero, Quaternion.identity);
				explosionVfx.Play();
				CameraModuleController.Shake(GameManager.CameraGameplaySettings.OrbitalLaserScreenshakeSettings);
				if (GameSettings.All.General.FlashingEffects)
				{
					CameraModuleController.PlayImpactFrame(base.transform.position);
				}
				explosionSoundInstance = RuntimeManager.CreateInstance(GameManager.AudioSettings.OrbitalLaserExplosionEvent);
				RuntimeManager.AttachInstanceToGameObject(explosionSoundInstance, base.gameObject);
				explosionSoundInstance.start();
				explosionSoundInstance.release();
			}
		}
	}

	public OrbitalLaser()
	{
		_Mirror_SyncVarHookDelegate_state = OnStateChanged;
	}

	public override bool Weaved()
	{
		return true;
	}

	public override void SerializeSyncVars(NetworkWriter writer, bool forceAll)
	{
		base.SerializeSyncVars(writer, forceAll);
		if (forceAll)
		{
			GeneratedNetworkCode._Write_OrbitalLaserState(writer, state);
			writer.WriteVector3(targetPosition);
			writer.WriteNetworkBehaviour(Networktarget);
			return;
		}
		writer.WriteVarULong(syncVarDirtyBits);
		if ((syncVarDirtyBits & 1L) != 0L)
		{
			GeneratedNetworkCode._Write_OrbitalLaserState(writer, state);
		}
		if ((syncVarDirtyBits & 2L) != 0L)
		{
			writer.WriteVector3(targetPosition);
		}
		if ((syncVarDirtyBits & 4L) != 0L)
		{
			writer.WriteNetworkBehaviour(Networktarget);
		}
	}

	public override void DeserializeSyncVars(NetworkReader reader, bool initialState)
	{
		base.DeserializeSyncVars(reader, initialState);
		if (initialState)
		{
			GeneratedSyncVarDeserialize(ref state, _Mirror_SyncVarHookDelegate_state, GeneratedNetworkCode._Read_OrbitalLaserState(reader));
			GeneratedSyncVarDeserialize(ref targetPosition, null, reader.ReadVector3());
			GeneratedSyncVarDeserialize_NetworkBehaviour(ref target, null, reader, ref ___targetNetId);
			return;
		}
		long num = (long)reader.ReadVarULong();
		if ((num & 1L) != 0L)
		{
			GeneratedSyncVarDeserialize(ref state, _Mirror_SyncVarHookDelegate_state, GeneratedNetworkCode._Read_OrbitalLaserState(reader));
		}
		if ((num & 2L) != 0L)
		{
			GeneratedSyncVarDeserialize(ref targetPosition, null, reader.ReadVector3());
		}
		if ((num & 4L) != 0L)
		{
			GeneratedSyncVarDeserialize_NetworkBehaviour(ref target, null, reader, ref ___targetNetId);
		}
	}
}
