using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;

namespace Mirror;

public class PredictedGolfCart : NetworkBehaviour
{
	[SerializeField]
	private WheelCollider wheelCollider1;

	[SerializeField]
	private WheelCollider wheelCollider2;

	[SerializeField]
	private WheelCollider wheelCollider3;

	[SerializeField]
	private WheelCollider wheelCollider4;

	private Transform tf;

	public Rigidbody predictedRigidbody;

	private Transform predictedRigidbodyTransform;

	private Vector3 lastPosition;

	[Header("Motion Smoothing")]
	[Tooltip("Prediction supports two different modes: Smooth and Fast:\n\nSmooth: Physics are separated from the GameObject & applied in the background. Rendering smoothly follows the physics for perfectly smooth interpolation results. Much softer, can be even too soft where sharp collisions won't look as sharp (i.e. Billiard balls avoid the wall before even hitting it).\n\nFast: Physics remain on the GameObject and corrections are applied hard. Much faster since we don't need to update a separate GameObject, a bit harsher, more precise.")]
	public PredictionMode mode;

	[Tooltip("Smoothing via Ghost-following only happens on demand, while moving with a minimum velocity.")]
	public float motionSmoothingVelocityThreshold = 0.1f;

	private float motionSmoothingVelocityThresholdSqr;

	public float motionSmoothingAngularVelocityThreshold = 5f;

	private float motionSmoothingAngularVelocityThresholdSqr;

	public float motionSmoothingTimeTolerance = 0.5f;

	private double motionSmoothingLastMovedTime;

	[Header("State History")]
	public int stateHistoryLimit = 32;

	private readonly SortedList<double, GolfCartState> stateHistory = new SortedList<double, GolfCartState>();

	public float recordInterval = 0.05f;

	[Tooltip("(Optional) performance optimization where FixedUpdate.RecordState() only inserts state into history if the state actually changed.\nThis is generally a good idea.")]
	public bool onlyRecordChanges = true;

	[Tooltip("(Optional) performance optimization where received state is compared to the LAST recorded state first, before sampling the whole history.\n\nThis can save significant traversal overhead for idle objects with a tiny chance of missing corrections for objects which revisisted the same position in the recent history twice.")]
	public bool compareLastFirst = true;

	[Header("Reconciliation")]
	[Tooltip("Correction threshold in meters. For example, 0.1 means that if the client is off by more than 10cm, it gets corrected.")]
	public double positionCorrectionThreshold = 0.1;

	private double positionCorrectionThresholdSqr;

	[Tooltip("Correction threshold in degrees. For example, 5 means that if the client is off by more than 5 degrees, it gets corrected.")]
	public double rotationCorrectionThreshold = 5.0;

	[Tooltip("Applying server corrections one frame ahead gives much better results. We don't know why yet, so this is an option for now.")]
	public bool oneFrameAhead = true;

	[Header("Smoothing")]
	[Tooltip("Snap to the server state directly when velocity is < threshold. This is useful to reduce jitter/fighting effects before coming to rest.\nNote this applies position, rotation and velocity(!) so it's still smooth.")]
	public float snapThreshold = 2f;

	[Header("Visual Interpolation")]
	[Tooltip("After creating the visual interpolation object, keep showing the original Rigidbody with a ghost (transparent) material for debugging.")]
	public bool showGhost = true;

	[Tooltip("Physics components are moved onto a ghost object beyond this threshold. Main object visually interpolates to it.")]
	public float ghostVelocityThreshold = 0.1f;

	[Tooltip("After creating the visual interpolation object, replace this object's renderer materials with the ghost (ideally transparent) material.")]
	public Material localGhostMaterial;

	public Material remoteGhostMaterial;

	[Tooltip("Performance optimization: only create/destroy ghosts every n-th frame is enough.")]
	public int checkGhostsEveryNthFrame = 4;

	[Tooltip("How fast to interpolate to the target position, relative to how far we are away from it.\nHigher value will be more jitter but sharper moves, lower value will be less jitter but a little too smooth / rounded moves.")]
	public float positionInterpolationSpeed = 15f;

	public float rotationInterpolationSpeed = 10f;

	[Tooltip("Teleport if we are further than 'multiplier x collider size' behind.")]
	public float teleportDistanceMultiplier = 10f;

	[Header("Bandwidth")]
	[Tooltip("Reduce sends while velocity==0. Client's objects may slightly move due to gravity/physics, so we still want to send corrections occasionally even if an object is idle on the server the whole time.")]
	public bool reduceSendsWhileIdle = true;

	protected GameObject physicsCopy;

	private float smoothFollowThreshold;

	private float smoothFollowThresholdSqr;

	protected GameObject remoteCopy;

	private Vector3 initialPosition;

	private Quaternion initialRotation;

	private Color originalColor;

	private bool lastMoving;

	private GolfCartState lastRecorded;

	private double lastRecordTime;

	protected virtual void Awake()
	{
		mode = PredictionMode.Fast;
		tf = base.transform;
		predictedRigidbody = GetComponent<Rigidbody>();
		if (predictedRigidbody == null)
		{
			throw new InvalidOperationException("Prediction: " + base.name + " is missing a Rigidbody component.");
		}
		predictedRigidbodyTransform = predictedRigidbody.transform;
		if (mode == PredictionMode.Fast)
		{
			predictedRigidbody.interpolation = RigidbodyInterpolation.Interpolate;
		}
		float magnitude = GetComponentInChildren<Collider>().bounds.size.magnitude;
		smoothFollowThreshold = magnitude * teleportDistanceMultiplier;
		smoothFollowThresholdSqr = smoothFollowThreshold * smoothFollowThreshold;
		initialPosition = tf.position;
		initialRotation = tf.rotation;
		motionSmoothingVelocityThresholdSqr = motionSmoothingVelocityThreshold * motionSmoothingVelocityThreshold;
		motionSmoothingAngularVelocityThresholdSqr = motionSmoothingAngularVelocityThreshold * motionSmoothingAngularVelocityThreshold;
		positionCorrectionThresholdSqr = positionCorrectionThreshold * positionCorrectionThreshold;
	}

	protected virtual void CopyRenderersAsGhost(GameObject destination, Material material)
	{
		MeshRenderer componentInChildren = GetComponentInChildren<MeshRenderer>(includeInactive: true);
		MeshFilter componentInChildren2 = GetComponentInChildren<MeshFilter>(includeInactive: true);
		if (componentInChildren != null && componentInChildren2 != null)
		{
			destination.AddComponent<MeshFilter>().mesh = componentInChildren2.mesh;
			MeshRenderer meshRenderer = destination.AddComponent<MeshRenderer>();
			meshRenderer.material = componentInChildren.material;
			if (componentInChildren.materials != null)
			{
				Material[] array = new Material[componentInChildren.materials.Length];
				for (int i = 0; i < array.Length; i++)
				{
					array[i] = material;
				}
				meshRenderer.materials = array;
			}
		}
		else
		{
			Debug.LogWarning("PredictedRigidbody: " + base.name + " found no renderer to copy onto the visual object. If you are using a custom setup, please overwrite PredictedRigidbody.CreateVisualCopy().");
		}
	}

	protected virtual void CreateGhosts()
	{
		if (!base.isServer && !(physicsCopy != null))
		{
			physicsCopy = new GameObject(base.name + "_Physical");
			physicsCopy.layer = base.gameObject.layer;
			PredictedRigidbodyPhysicsGhost predictedRigidbodyPhysicsGhost = physicsCopy.AddComponent<PredictedRigidbodyPhysicsGhost>();
			predictedRigidbodyPhysicsGhost.target = tf;
			Vector3 position = tf.position;
			Quaternion rotation = tf.rotation;
			predictedRigidbodyPhysicsGhost.transform.position = tf.position;
			predictedRigidbodyPhysicsGhost.transform.rotation = tf.rotation;
			predictedRigidbodyPhysicsGhost.transform.localScale = tf.lossyScale;
			PredictionUtils.MovePhysicsComponents(base.gameObject, physicsCopy);
			Transform obj = predictedRigidbodyPhysicsGhost.transform;
			Vector3 position2 = (tf.position = position);
			obj.position = position2;
			Transform obj2 = predictedRigidbodyPhysicsGhost.transform;
			Quaternion rotation2 = (tf.rotation = rotation);
			obj2.rotation = rotation2;
			if (showGhost)
			{
				CopyRenderersAsGhost(physicsCopy, localGhostMaterial);
				remoteCopy = new GameObject(base.name + "_Remote");
				remoteCopy.transform.position = tf.position;
				remoteCopy.transform.rotation = tf.rotation;
				remoteCopy.transform.localScale = tf.lossyScale;
				CopyRenderersAsGhost(remoteCopy, remoteGhostMaterial);
			}
			predictedRigidbody = physicsCopy.GetComponent<Rigidbody>();
			predictedRigidbodyTransform = predictedRigidbody.transform;
		}
	}

	protected virtual void DestroyGhosts()
	{
		if (physicsCopy != null)
		{
			Vector3 position = tf.position;
			Quaternion rotation = tf.rotation;
			Vector3 localScale = tf.localScale;
			physicsCopy.transform.position = tf.position;
			physicsCopy.transform.rotation = tf.rotation;
			physicsCopy.transform.localScale = tf.lossyScale;
			PredictionUtils.MovePhysicsComponents(physicsCopy, base.gameObject);
			tf.position = position;
			tf.rotation = rotation;
			tf.localScale = localScale;
			UnityEngine.Object.Destroy(physicsCopy);
			predictedRigidbody = GetComponent<Rigidbody>();
			predictedRigidbodyTransform = predictedRigidbody.transform;
		}
		if (remoteCopy != null)
		{
			UnityEngine.Object.Destroy(remoteCopy);
		}
	}

	protected virtual void SmoothFollowPhysicsCopy()
	{
		tf.GetPositionAndRotation(out var position, out var rotation);
		predictedRigidbodyTransform.GetPositionAndRotation(out var position2, out var rotation2);
		float deltaTime = Time.deltaTime;
		Vector3 vector = position2 - position;
		float num = Vector3.SqrMagnitude(vector);
		float num2 = Mathf.Sqrt(num);
		if (num > smoothFollowThresholdSqr)
		{
			tf.SetPositionAndRotation(position2, rotation2);
			Debug.Log($"[PredictedRigidbody] Teleported because distance to physics copy = {num2:F2} > threshold {smoothFollowThreshold:F2}");
			return;
		}
		float num3 = num2 * positionInterpolationSpeed;
		Vector3 position3 = MoveTowardsCustom(position, position2, vector, num, num2, num3 * deltaTime);
		Quaternion normalized = Quaternion.Slerp(rotation, rotation2, rotationInterpolationSpeed * deltaTime).normalized;
		tf.SetPositionAndRotation(position3, normalized);
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	private static Vector3 MoveTowardsCustom(Vector3 current, Vector3 target, Vector3 _delta, float _sqrDistance, float _distance, float maxDistanceDelta)
	{
		if ((double)_sqrDistance == 0.0 || ((double)maxDistanceDelta >= 0.0 && _sqrDistance <= maxDistanceDelta * maxDistanceDelta))
		{
			return target;
		}
		float num = maxDistanceDelta / _distance;
		return new Vector3(current.x + _delta.x * num, current.y + _delta.y * num, current.z + _delta.z * num);
	}

	public override void OnStopClient()
	{
		DestroyGhosts();
	}

	private void UpdateOwner()
	{
		if (reduceSendsWhileIdle)
		{
			syncInterval = ((!IsMoving()) ? 1 : 0);
		}
		SetDirty();
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	protected virtual bool IsMoving()
	{
		if (!(predictedRigidbody.linearVelocity.sqrMagnitude >= motionSmoothingVelocityThresholdSqr))
		{
			return predictedRigidbody.angularVelocity.sqrMagnitude >= motionSmoothingAngularVelocityThresholdSqr;
		}
		return true;
	}

	private void UpdateGhosting()
	{
		if (Time.frameCount % checkGhostsEveryNthFrame != 0)
		{
			return;
		}
		if (physicsCopy == null)
		{
			if (IsMoving())
			{
				CreateGhosts();
				OnBeginPrediction();
			}
		}
		else if (IsMoving())
		{
			motionSmoothingLastMovedTime = NetworkTime.time;
		}
		else if (NetworkTime.time >= motionSmoothingLastMovedTime + (double)motionSmoothingTimeTolerance)
		{
			DestroyGhosts();
			OnEndPrediction();
			physicsCopy = null;
		}
	}

	private void UpdateState()
	{
		if (Time.frameCount % checkGhostsEveryNthFrame == 0)
		{
			bool flag = IsMoving();
			if (flag && !lastMoving)
			{
				OnBeginPrediction();
				lastMoving = true;
			}
			else if (!flag && lastMoving && NetworkTime.time >= motionSmoothingLastMovedTime + (double)motionSmoothingTimeTolerance)
			{
				OnEndPrediction();
				lastMoving = false;
			}
		}
	}

	private void Update()
	{
		if (base.authority)
		{
			UpdateOwner();
		}
		else if (mode == PredictionMode.Smooth)
		{
			UpdateGhosting();
		}
		else if (mode == PredictionMode.Fast)
		{
			UpdateState();
		}
	}

	private void LateUpdate()
	{
		if (base.authority && mode == PredictionMode.Smooth && (bool)physicsCopy)
		{
			SmoothFollowPhysicsCopy();
		}
	}

	private void FixedUpdate()
	{
		if (!base.authority)
		{
			return;
		}
		if (onlyRecordChanges)
		{
			tf.GetPositionAndRotation(out var position, out var rotation);
			if ((double)(lastRecorded.position - position).sqrMagnitude < positionCorrectionThresholdSqr && (double)Quaternion.Angle(lastRecorded.rotation, rotation) < rotationCorrectionThreshold)
			{
				return;
			}
		}
		RecordState();
	}

	private void RecordState()
	{
		double time = NetworkTime.time;
		if (time < lastRecordTime + (double)recordInterval)
		{
			return;
		}
		lastRecordTime = time;
		double predictedTime = NetworkTime.predictedTime;
		if (predictedTime != lastRecorded.timestamp)
		{
			if (stateHistory.Count >= stateHistoryLimit)
			{
				stateHistory.RemoveAt(0);
			}
			tf.GetPositionAndRotation(out var position, out var rotation);
			Vector3 linearVelocity = predictedRigidbody.linearVelocity;
			Vector3 angularVelocity = predictedRigidbody.angularVelocity;
			Vector4 vector = new Vector4(wheelCollider1.rotationSpeed, wheelCollider2.rotationSpeed, wheelCollider3.rotationSpeed, wheelCollider4.rotationSpeed);
			Vector3 positionDelta = Vector3.zero;
			Vector3 velocityDelta = Vector3.zero;
			Vector3 angularVelocityDelta = Vector3.zero;
			Quaternion rotationDelta = Quaternion.identity;
			Vector4 wheelSpeedsDelta = Vector4.zero;
			int count = stateHistory.Count;
			if (count > 0)
			{
				GolfCartState golfCartState = stateHistory.Values[count - 1];
				positionDelta = position - golfCartState.position;
				velocityDelta = linearVelocity - golfCartState.velocity;
				rotationDelta = (rotation * Quaternion.Inverse(golfCartState.rotation)).normalized;
				angularVelocityDelta = angularVelocity - golfCartState.angularVelocity;
				wheelSpeedsDelta = vector - golfCartState.wheelSpeeds;
			}
			GolfCartState value = new GolfCartState(predictedTime, positionDelta, position, rotationDelta, rotation, velocityDelta, linearVelocity, angularVelocityDelta, angularVelocity, wheelSpeedsDelta, vector);
			stateHistory.Add(predictedTime, value);
			lastRecorded = value;
		}
	}

	protected virtual void OnSnappedIntoPlace()
	{
	}

	protected virtual void OnBeforeApplyState()
	{
	}

	protected virtual void OnCorrected()
	{
	}

	protected virtual void OnBeginPrediction()
	{
	}

	protected virtual void OnEndPrediction()
	{
	}

	private void ApplyState(double timestamp, Vector3 position, Quaternion rotation, Vector3 velocity, Vector3 angularVelocity, Vector4 wheelSpeeds)
	{
		if (predictedRigidbody.linearVelocity.magnitude <= snapThreshold && predictedRigidbody.angularVelocity.magnitude <= snapThreshold)
		{
			predictedRigidbody.position = position;
			predictedRigidbody.rotation = rotation;
			if (!predictedRigidbody.isKinematic)
			{
				predictedRigidbody.linearVelocity = velocity;
				predictedRigidbody.angularVelocity = angularVelocity;
			}
			stateHistory.Clear();
			stateHistory.Add(timestamp, new GolfCartState(timestamp, Vector3.zero, position, Quaternion.identity, rotation, Vector3.zero, velocity, Vector3.zero, angularVelocity, Vector4.zero, wheelSpeeds));
			OnSnappedIntoPlace();
			return;
		}
		OnBeforeApplyState();
		if (mode == PredictionMode.Smooth)
		{
			predictedRigidbody.position = position;
			predictedRigidbody.rotation = rotation;
		}
		else if (mode == PredictionMode.Fast)
		{
			predictedRigidbody.MovePosition(position);
			predictedRigidbody.MoveRotation(rotation);
		}
		if (!predictedRigidbody.isKinematic)
		{
			predictedRigidbody.linearVelocity = velocity;
			predictedRigidbody.angularVelocity = angularVelocity;
		}
	}

	private void OnReceivedState(double timestamp, GolfCartState state)
	{
		if (remoteCopy != null)
		{
			Transform obj = remoteCopy.transform;
			obj.SetPositionAndRotation(state.position, state.rotation);
			obj.localScale = tf.lossyScale;
		}
		predictedRigidbodyTransform.GetPositionAndRotation(out var position, out var rotation);
		float num = Vector3.SqrMagnitude(state.position - position);
		if (compareLastFirst && (double)num < positionCorrectionThresholdSqr && (double)Quaternion.Angle(state.rotation, rotation) < rotationCorrectionThreshold)
		{
			return;
		}
		RecordState();
		if (stateHistory.Count < 2)
		{
			return;
		}
		GolfCartState golfCartState = stateHistory.Values[0];
		GolfCartState golfCartState2 = stateHistory.Values[stateHistory.Count - 1];
		GolfCartState before;
		GolfCartState after;
		int afterIndex;
		double t;
		if (state.timestamp < golfCartState.timestamp)
		{
			if (stateHistory.Count >= stateHistoryLimit)
			{
				Debug.LogWarning($"Hard correcting client object {base.name} because the client is too far behind the server. History of size={stateHistory.Count} @ t={timestamp:F3} oldest={golfCartState.timestamp:F3} newest={golfCartState2.timestamp:F3}. This would cause the client to be out of sync as long as it's behind.");
			}
			ApplyState(state.timestamp, state.position, state.rotation, state.velocity, state.angularVelocity, state.wheelSpeeds);
		}
		else if (golfCartState2.timestamp < state.timestamp)
		{
			if ((double)num >= positionCorrectionThresholdSqr)
			{
				ApplyState(state.timestamp, state.position, state.rotation, state.velocity, state.angularVelocity, state.wheelSpeeds);
			}
		}
		else if (!Prediction.Sample(stateHistory, timestamp, out before, out after, out afterIndex, out t))
		{
			Debug.LogError($"Failed to sample history of size={stateHistory.Count} @ t={timestamp:F3} oldest={golfCartState.timestamp:F3} newest={golfCartState2.timestamp:F3}. This should never happen because the timestamp is within history.");
			ApplyState(state.timestamp, state.position, state.rotation, state.velocity, state.angularVelocity, state.wheelSpeeds);
		}
		else
		{
			GolfCartState golfCartState3 = GolfCartState.Interpolate(before, after, (float)t);
			float num2 = Vector3.SqrMagnitude(state.position - golfCartState3.position);
			float num3 = Quaternion.Angle(state.rotation, golfCartState3.rotation);
			if ((double)num2 >= positionCorrectionThresholdSqr || (double)num3 >= rotationCorrectionThreshold)
			{
				GolfCartState golfCartState4 = Prediction.CorrectHistory(stateHistory, stateHistoryLimit, state, before, after, afterIndex);
				ApplyState(golfCartState4.timestamp, golfCartState4.position, golfCartState4.rotation, golfCartState4.velocity, golfCartState4.angularVelocity, golfCartState4.wheelSpeeds);
				OnCorrected();
			}
		}
	}

	public override void OnSerialize(NetworkWriter writer, bool initialState)
	{
		tf.GetPositionAndRotation(out var position, out var rotation);
		PredictedGolfCartSyncData data = new PredictedGolfCartSyncData(Time.deltaTime, position, rotation, predictedRigidbody.linearVelocity, predictedRigidbody.angularVelocity, new Vector4(wheelCollider1.rotationSpeed, wheelCollider2.rotationSpeed, wheelCollider3.rotationSpeed, wheelCollider4.rotationSpeed));
		writer.WritePredictedGolfCartSyncData(data);
	}

	public override void OnDeserialize(NetworkReader reader, bool initialState)
	{
		double remoteTimeStamp = NetworkClient.connection.remoteTimeStamp;
		PredictedGolfCartSyncData predictedGolfCartSyncData = reader.ReadPredictedGolfCartSyncData();
		double num = predictedGolfCartSyncData.deltaTime;
		Vector3 position = predictedGolfCartSyncData.position;
		Quaternion rotation = predictedGolfCartSyncData.rotation;
		Vector3 velocity = predictedGolfCartSyncData.velocity;
		Vector3 angularVelocity = predictedGolfCartSyncData.angularVelocity;
		Vector4 wheelSpeeds = predictedGolfCartSyncData.wheelSpeeds;
		remoteTimeStamp += num;
		if (oneFrameAhead)
		{
			remoteTimeStamp += num;
		}
		OnReceivedState(remoteTimeStamp, new GolfCartState(remoteTimeStamp, Vector3.zero, position, Quaternion.identity, rotation, Vector3.zero, velocity, Vector3.zero, angularVelocity, Vector4.zero, wheelSpeeds));
	}

	protected override void OnValidate()
	{
		base.OnValidate();
		mode = PredictionMode.Fast;
		syncDirection = SyncDirection.ServerToClient;
		syncInterval = 0f;
	}

	public static bool IsPredicted(Rigidbody rb, out PredictedRigidbody predictedRigidbody)
	{
		if (rb.TryGetComponent<PredictedRigidbody>(out predictedRigidbody))
		{
			return true;
		}
		if (rb.TryGetComponent<PredictedRigidbodyPhysicsGhost>(out var component))
		{
			predictedRigidbody = component.target.GetComponent<PredictedRigidbody>();
			return true;
		}
		predictedRigidbody = null;
		return false;
	}

	public static bool IsPredicted(Collider co, out PredictedRigidbody predictedRigidbody)
	{
		predictedRigidbody = co.GetComponentInParent<PredictedRigidbody>();
		if (predictedRigidbody != null)
		{
			return true;
		}
		PredictedRigidbodyPhysicsGhost componentInParent = co.GetComponentInParent<PredictedRigidbodyPhysicsGhost>();
		if (componentInParent != null && componentInParent.target != null && componentInParent.target.TryGetComponent<PredictedRigidbody>(out predictedRigidbody))
		{
			return true;
		}
		predictedRigidbody = null;
		return false;
	}

	public override bool Weaved()
	{
		return true;
	}
}
