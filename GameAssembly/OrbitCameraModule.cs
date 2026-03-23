using System;
using System.Collections;
using System.Collections.Generic;
using Brimstone.Geometry;
using UnityEngine;

public class OrbitCameraModule : CameraModule
{
	private enum TrackingMode
	{
		BoundsCenter,
		ExplicitSubjectSpacePosition
	}

	[Flags]
	private enum TrackingOffsetMode
	{
		None = 0,
		SubjectSpace = 1,
		CameraSpace = 2
	}

	private const float physicsWidthPadding = 0.3f;

	private const float physicsHeightPadding = 0.3f;

	private const float physicsDepthPadding = 0.3f;

	private const float defaultExternalFovChangeDurationPerDegree = 0.01f;

	[Header("Subject")]
	[SerializeField]
	private Transform subject;

	[Header("General")]
	[Name("Base FOV")]
	[SerializeField]
	private float baseFov = 65f;

	[Name("Over the shoulder viewport X")]
	[SerializeField]
	private float overTheShoulderViewportTargetX = 0.35f;

	[Name("Input sensitivity")]
	[SerializeField]
	private float axisSensitivity;

	[Name("Max pitch")]
	[SerializeField]
	private float maxPitch = 90f;

	[Header("Initial state")]
	[Name("Pitch")]
	[Tooltip("Pitch of the camera's forward direction; positive when pointed below the horizon.")]
	[SerializeField]
	private float initialPitch = 20f;

	[Name("Yaw")]
	[Tooltip("Yaw of the camera's forward direction relative to the world positive Z axis.")]
	[SerializeField]
	private float initialYaw;

	[Header("Pitch-dependent values")]
	[Name("Subject Y offset")]
	[Tooltip("Height offset of the effective subject point.")]
	[SerializeField]
	private AnimationCurve subjectYOffsetPitchCurve;

	[Name("Water subject Y offset")]
	[Tooltip("Height offset of the effective subject point when in water.")]
	[SerializeField]
	private AnimationCurve waterSubjectYOffsetPitchCurve;

	[Name("FOV addition")]
	[SerializeField]
	private AnimationCurve fovAdditionPitchCurve;

	[Name("Distance addition")]
	[SerializeField]
	private AnimationCurve distanceAdditionPitchCurve;

	[Name("Water distance addition")]
	[SerializeField]
	private AnimationCurve waterDistanceAdditionPitchCurve;

	[Name("Input sensitivity factor")]
	[SerializeField]
	private AnimationCurve sensitivityFactorPitchCurve;

	[Header("Heading forwardness-dependent values")]
	[Name("Horizontal follow speed")]
	[SerializeField]
	private AnimationCurve horizontalFollowSpeedHeadingForwardnessCurve;

	[Name("Distance addition")]
	[SerializeField]
	private AnimationCurve distanceAdditionHeadingForwardnessCurve;

	[Header("Timing")]
	[SerializeField]
	private float subjectBoundsSpeed = 6f;

	[Tooltip("")]
	[SerializeField]
	private float upwardsFollowSpeed = 4f;

	[Tooltip("")]
	[SerializeField]
	private float downwardsFollowSpeed = 8f;

	[Tooltip("The speed at which the base target distance changes, such as when the subject bounds are changed.")]
	[SerializeField]
	private float targetDistanceSpeed = 4f;

	[Tooltip("The speed at which the base distance changes, such as when reacting to the environment's geometry.")]
	[SerializeField]
	private float distanceSpeed = 24f;

	[Tooltip("The speed at which the camera is offset within its viewing plane, such as when triggering over-the-shoulder view.")]
	[SerializeField]
	private float viewPlaneOffsetSpeed = 8f;

	[Tooltip("")]
	[SerializeField]
	private float hardZoneSnapSpeed = 30f;

	[Tooltip("")]
	[SerializeField]
	private float headingForwardnessDistanceAdditionSpeed = 2f;

	[Header("Hard zone")]
	[Name("Center")]
	[SerializeField]
	private Vector2 hardZoneCenter = 0.5f * Vector2.one;

	[Name("Size")]
	[SerializeField]
	private Vector2 hardZoneSize = 0.5f * Vector2.one;

	[Header("Lock on soft zone")]
	private Camera camera;

	private Bounds subjectLocalBounds;

	private Bounds effectiveSubjectLocalBounds;

	private Bounds effectiveSubjectWorldBounds;

	private float subjectBoundsTransitionSpeedFactor = 1f;

	private Vector3 previousSubjectTrackedPoint;

	private Bounds previousSubjectBounds;

	private bool isTransitioningSubjects;

	private float subjectTransitionTime;

	private float subjectTransitionFactor;

	private float totalSubjectTransitionTime;

	private float distance;

	private float headingForwardnessDistanceAddition;

	private float pitch;

	private float yaw;

	private float roll;

	private float normalizedPitch;

	private float fov;

	private Vector3 viewPlaneOffset;

	private float targetDistanceRaw;

	private Rect hardZone;

	private Plane[] frustumPlanes = new Plane[6];

	private Coroutine yawAdditionRoutine;

	private Coroutine transitionRollToRoutine;

	private float externalFovOffset;

	private Coroutine fovOffsetAnimationRoutine;

	private float externalDistanceAddition;

	private bool useHorizontalTrackingSpeedForVertical;

	private bool yawLockedToSubjectForward;

	private float lockedYawOffset;

	[NonSerialized]
	public bool overTheShoulder;

	[NonSerialized]
	public bool disablePhysics;

	private HashSet<Collider> collidersToIgnore = new HashSet<Collider>();

	public readonly RaycastHit[] raycastHitBuffer = new RaycastHit[30];

	private bool clearCollidersToIgnoreAfterNextSubjectTransition;

	private TrackingMode trackingMode;

	private Vector3 explicitTrackedPointSubjectSpacePosition;

	private TrackingOffsetMode trackingOffsetMode;

	private Vector3 subjectSpaceTrackingOffset;

	private Vector3 cameraSpaceTrackingOffset;

	[CVar("drawCameraDebug", "", "", false, true, callback = "VerboseChanged", resetOnSceneChangeOrCheatsDisabled = false)]
	public static bool drawDebug;

	public override CameraModuleType Type => CameraModuleType.Orbit;

	public Vector3 TrackedPoint { get; private set; }

	public Transform Subject => subject;

	public float Pitch => pitch;

	public float Yaw => yaw;

	public float RotationSpeed { get; private set; }

	public bool IsAnimatingExternalFov { get; private set; }

	public override bool ControlsFieldOfView => subject != null;

	public override float FieldOfView => fov;

	public static event Action OrbitCameraAwoken;

	private static void VerboseChanged()
	{
		OrbitCameraModule orbitCameraModule = UnityEngine.Object.FindAnyObjectByType<OrbitCameraModule>();
		if (drawDebug)
		{
			BDebug.RegisterOnGuiCallback(orbitCameraModule.VerboseGUI);
		}
		else
		{
			BDebug.DeregisterOnGuiCallback(orbitCameraModule.VerboseGUI);
		}
	}

	private void Awake()
	{
		camera = GetComponent<Camera>();
		SetPitch(initialPitch);
		SetYaw(initialYaw);
		OrbitCameraModule.OrbitCameraAwoken?.Invoke();
	}

	public override void OnTransitionStart(bool transitioningToThis)
	{
		if (transitioningToThis)
		{
			SyncFromTransform();
		}
	}

	public void SetPitch(float pitch)
	{
		this.pitch = BMath.Clamp(pitch, 0f - maxPitch, maxPitch);
		normalizedPitch = BMath.Remap(-90f, 90f, 0f, 1f, pitch);
	}

	public void SetYaw(float yaw)
	{
		this.yaw = yaw.WrapAngleDeg();
	}

	public void SetRoll(float roll)
	{
		this.roll = roll.WrapAngleDeg();
	}

	public void Rotate(Vector2 axis)
	{
		float num = (BMath.AreSignsOpposite(pitch, axis.y) ? sensitivityFactorPitchCurve.Evaluate(normalizedPitch) : 1f);
		Vector2 vector = new Vector2(axis.x * axisSensitivity, (0f - axis.y) * axisSensitivity * num);
		SetYaw(yaw + vector.x);
		SetPitch(pitch + vector.y);
		RotationSpeed = vector.magnitude / Time.deltaTime;
	}

	public void InformNoRotation()
	{
		RotationSpeed = 0f;
	}

	public void SetSubject(Transform subject)
	{
		SetSubjectInternal(subject);
		FinishTransition();
	}

	public void SetSubjectLocalBounds(Bounds bounds)
	{
		subjectLocalBounds = bounds;
	}

	public void SetSubjectBoundsTransitionSpeedFactor(float speedFactor)
	{
		subjectBoundsTransitionSpeedFactor = speedFactor;
	}

	public void TransitionToSubject(Transform subject, float transitionTime)
	{
		previousSubjectTrackedPoint = TrackedPoint;
		previousSubjectBounds = subjectLocalBounds;
		SetSubjectInternal(subject);
		isTransitioningSubjects = true;
		subjectTransitionTime = 0f;
		totalSubjectTransitionTime = transitionTime;
	}

	public void LockYawToSubjectForward(float offset = 0f)
	{
		yawLockedToSubjectForward = true;
		SetLockedYawOffset(offset);
	}

	public void SetLockedYawOffset(float offset)
	{
		lockedYawOffset = offset;
	}

	public void UnlockYaw()
	{
		yawLockedToSubjectForward = false;
	}

	public void SetCollidersToIgnore(IEnumerable<Collider> colliders)
	{
		collidersToIgnore.Clear();
		collidersToIgnore.UnionWith(colliders);
	}

	public void ClearCollidersToIgnore()
	{
		collidersToIgnore.Clear();
	}

	public void SetExplicitTrackedPointLocalPosition(Vector3 localPosition)
	{
		trackingMode = TrackingMode.ExplicitSubjectSpacePosition;
		explicitTrackedPointSubjectSpacePosition = localPosition;
	}

	public void ClearExplicitTrackedPointLocalPosition()
	{
		trackingMode = TrackingMode.BoundsCenter;
	}

	public void SetSubjectSpaceTrackingOffset(Vector3 offset)
	{
		if (offset == Vector3.zero)
		{
			ClearSubjectSpaceTrackingOffset();
			return;
		}
		trackingOffsetMode |= TrackingOffsetMode.SubjectSpace;
		subjectSpaceTrackingOffset = offset;
	}

	public void ClearSubjectSpaceTrackingOffset()
	{
		trackingOffsetMode &= ~TrackingOffsetMode.SubjectSpace;
	}

	public void SetCameraSpaceTrackingOffset(Vector3 offset)
	{
		if (offset == Vector3.zero)
		{
			ClearCameraSpaceTrackingOffset();
			return;
		}
		trackingOffsetMode |= TrackingOffsetMode.CameraSpace;
		cameraSpaceTrackingOffset = offset;
	}

	public void SetDistanceAddition(float distanceAddition)
	{
		externalDistanceAddition = distanceAddition;
	}

	public void SetUseHorizontalTrackingSpeedForVertical(bool shouldUse)
	{
		useHorizontalTrackingSpeedForVertical = shouldUse;
	}

	public void ClearCameraSpaceTrackingOffset()
	{
		trackingOffsetMode &= ~TrackingOffsetMode.CameraSpace;
	}

	public void ClearCollidersToIgnoreAfterNextSubjectTransition()
	{
		clearCollidersToIgnoreAfterNextSubjectTransition = true;
	}

	public void AddYawOverTime(float addition, float duration)
	{
		if (yawAdditionRoutine != null)
		{
			StopCoroutine(yawAdditionRoutine);
		}
		yawAdditionRoutine = StartCoroutine(AddYawOverTimeRoutine(addition, duration));
	}

	private IEnumerator AddYawOverTimeRoutine(float addition, float duration)
	{
		float additionPerSecond = addition / duration;
		for (float time = 0f; time < duration; time += Time.deltaTime)
		{
			yield return null;
			SetYaw(yaw + Time.deltaTime * additionPerSecond);
		}
		SetYaw(yaw + addition);
	}

	public void TransitionRollTo(float targetRoll, float duration, Func<float, float> Easing)
	{
		if (transitionRollToRoutine != null)
		{
			StopCoroutine(transitionRollToRoutine);
		}
		transitionRollToRoutine = StartCoroutine(TransitionRollToRoutine(targetRoll, duration, Easing));
	}

	private IEnumerator TransitionRollToRoutine(float targetRoll, float duration, Func<float, float> Easing)
	{
		float initialRoll = roll;
		float time = 0f;
		while (time < duration)
		{
			yield return null;
			time += Time.deltaTime;
			float arg = time / duration;
			SetRoll(BMath.Lerp(initialRoll, targetRoll, Easing(arg)));
		}
		SetRoll(targetRoll);
	}

	public bool IsPointVisible(Vector3 worldPoint)
	{
		Plane[] array = frustumPlanes;
		foreach (Plane plane in array)
		{
			if (!plane.GetSide(worldPoint))
			{
				return false;
			}
		}
		return true;
	}

	public bool AreBoundsVisible(Bounds worldBounds)
	{
		return GeometryUtility.TestPlanesAABB(frustumPlanes, worldBounds);
	}

	public void ForceUpdateModule()
	{
		if (!(subject == null))
		{
			effectiveSubjectLocalBounds = subjectLocalBounds;
			effectiveSubjectWorldBounds = subject.TransformBounds(effectiveSubjectLocalBounds);
			if (yawLockedToSubjectForward)
			{
				SetYaw((subject.forward.GetYawDeg() + lockedYawOffset).WrapAngleDeg());
			}
			rotation = GetRotation(pitch, yaw, roll);
			hardZone = GetAppliedHardZone(hardZoneCenter, hardZoneSize);
			fov = GetFov(normalizedPitch);
			float subjectHeadingForwardness = GetSubjectHeadingForwardness();
			Vector3 targetTrackedPoint = (TrackedPoint = GetCurrentTargetTrackedPoint());
			targetDistanceRaw = GetRawTargetDistance(effectiveSubjectWorldBounds, fov);
			float num = distanceAdditionHeadingForwardnessCurve.Evaluate(subjectHeadingForwardness);
			headingForwardnessDistanceAddition = num;
			float num2 = GetDistancePitchAddition(normalizedPitch) + externalDistanceAddition;
			float targetDistance = targetDistanceRaw + headingForwardnessDistanceAddition + num2;
			targetDistance = PhysicsCorrectTargetDistance(targetDistance, TrackedPoint, camera.nearClipPlane);
			distance = targetDistance;
			Vector2 targetTrackedPointViewPlanePosition = GetTargetTrackedPointViewPlanePosition();
			Vector3 vector = TrackedPoint - GetCurrentForward() * distance;
			Vector3 rawTargetViewPlaneOffsetFor = GetRawTargetViewPlaneOffsetFor(TrackedPoint, vector, targetTrackedPointViewPlanePosition);
			viewPlaneOffset = PhysicsCorrectViewPlaneOffset(rawTargetViewPlaneOffsetFor, vector, camera.nearClipPlane);
			Vector3 hardZoneCorrectedViewPlaneOffset = GetHardZoneCorrectedViewPlaneOffset(viewPlaneOffset, vector, targetTrackedPoint, targetTrackedPointViewPlanePosition);
			viewPlaneOffset = hardZoneCorrectedViewPlaneOffset;
			position = vector + viewPlaneOffset;
			UpdateFrustumPlanes();
		}
	}

	public override void UpdateModule()
	{
		if (subject == null)
		{
			return;
		}
		float t = subjectBoundsTransitionSpeedFactor * subjectBoundsSpeed * Time.deltaTime;
		effectiveSubjectLocalBounds = new Bounds(Vector3.Lerp(effectiveSubjectLocalBounds.center, subjectLocalBounds.center, t), Vector3.Lerp(effectiveSubjectLocalBounds.size, subjectLocalBounds.size, t));
		effectiveSubjectWorldBounds = subject.TransformBounds(effectiveSubjectLocalBounds);
		if (yawLockedToSubjectForward)
		{
			float to = (subject.forward.GetYawDeg() + lockedYawOffset - yaw).WrapAngleDeg();
			SetYaw(yaw + BMath.LerpClamped(0f, to, 4f * Time.deltaTime));
		}
		rotation = GetRotation(pitch, yaw, roll);
		hardZone = GetAppliedHardZone(hardZoneCenter, hardZoneSize);
		fov = GetFov(normalizedPitch);
		float subjectHeadingForwardness = GetSubjectHeadingForwardness();
		Vector3 vector;
		if (isTransitioningSubjects)
		{
			vector = GetSubjectTransitionTargetPoint();
			TrackedPoint = Vector3.Lerp(TrackedPoint, vector, 40f * Time.deltaTime);
			targetDistanceRaw = GetSubjectTransitionTargetDistance();
		}
		else
		{
			vector = GetCurrentTargetTrackedPoint();
			TrackedPoint = GetSmoothedTrackedPoint(TrackedPoint, vector, subjectHeadingForwardness);
			targetDistanceRaw = GetSmoothedRawTargetDistance(targetDistanceRaw, effectiveSubjectWorldBounds, fov);
		}
		float to2 = distanceAdditionHeadingForwardnessCurve.Evaluate(subjectHeadingForwardness);
		headingForwardnessDistanceAddition = BMath.LerpClamped(headingForwardnessDistanceAddition, to2, headingForwardnessDistanceAdditionSpeed * Time.deltaTime);
		float num = GetDistancePitchAddition(normalizedPitch) + externalDistanceAddition;
		float targetDistance = targetDistanceRaw + headingForwardnessDistanceAddition + num;
		targetDistance = PhysicsCorrectTargetDistance(targetDistance, TrackedPoint, camera.nearClipPlane);
		distance = BMath.LerpClamped(distance, targetDistance, distanceSpeed * Time.deltaTime);
		Vector2 targetTrackedPointViewPlanePosition = GetTargetTrackedPointViewPlanePosition();
		Vector3 vector2 = TrackedPoint - GetCurrentForward() * distance;
		Vector3 rawTargetViewPlaneOffsetFor = GetRawTargetViewPlaneOffsetFor(TrackedPoint, vector2, targetTrackedPointViewPlanePosition);
		rawTargetViewPlaneOffsetFor = PhysicsCorrectViewPlaneOffset(rawTargetViewPlaneOffsetFor, vector2, camera.nearClipPlane);
		viewPlaneOffset = Vector3.Lerp(viewPlaneOffset, rawTargetViewPlaneOffsetFor, viewPlaneOffsetSpeed * Time.deltaTime);
		Vector3 hardZoneCorrectedViewPlaneOffset = GetHardZoneCorrectedViewPlaneOffset(viewPlaneOffset, vector2, vector, targetTrackedPointViewPlanePosition);
		viewPlaneOffset = Vector3.Lerp(viewPlaneOffset, hardZoneCorrectedViewPlaneOffset, hardZoneSnapSpeed * Time.deltaTime);
		position = vector2 + viewPlaneOffset;
		UpdateFrustumPlanes();
		if (isTransitioningSubjects)
		{
			subjectTransitionTime += Time.deltaTime;
			if (subjectTransitionTime >= totalSubjectTransitionTime)
			{
				subjectTransitionTime = 0f;
				subjectTransitionFactor = 0f;
				FinishTransition();
			}
			else
			{
				subjectTransitionFactor = BMath.EaseInOut(subjectTransitionTime / totalSubjectTransitionTime);
			}
		}
	}

	public void SetFovOffset(float fovOffset)
	{
		CancelExternalFovOffsetAnimation();
		externalFovOffset = fovOffset;
	}

	public void AnimateFovOffset(float targetFovOffset)
	{
		CancelExternalFovOffsetAnimation();
		fovOffsetAnimationRoutine = StartCoroutine(AnimateExternalFovOffset(targetFovOffset, 1f, BMath.EaseOut));
	}

	public void AnimateFovOffset(float targetFovOffset, float speedFactor, Func<float, float> Easing)
	{
		CancelExternalFovOffsetAnimation();
		fovOffsetAnimationRoutine = StartCoroutine(AnimateExternalFovOffset(targetFovOffset, speedFactor, Easing));
	}

	public void AnimateFovOffsetOverTime(float targetFovOffset, float duration, Func<float, float> Easing)
	{
		if (fovOffsetAnimationRoutine != null)
		{
			StopCoroutine(fovOffsetAnimationRoutine);
		}
		fovOffsetAnimationRoutine = StartCoroutine(AnimateExternalFovOffset(externalFovOffset, targetFovOffset, duration, Easing));
	}

	private void CancelExternalFovOffsetAnimation()
	{
		IsAnimatingExternalFov = false;
		if (fovOffsetAnimationRoutine != null)
		{
			StopCoroutine(fovOffsetAnimationRoutine);
		}
	}

	private IEnumerator AnimateExternalFovOffset(float targetOffset, float speedFactor, Func<float, float> Easing)
	{
		float num = 0.01f / speedFactor;
		float num2 = externalFovOffset;
		float duration = BMath.Abs(targetOffset - num2) * num;
		yield return AnimateExternalFovOffset(num2, targetOffset, duration, Easing);
	}

	private IEnumerator AnimateExternalFovOffset(float initialOffset, float targetOffset, float duration, Func<float, float> Easing)
	{
		IsAnimatingExternalFov = true;
		float time = 0f;
		while (time < duration)
		{
			yield return null;
			time += Time.deltaTime;
			externalFovOffset = BMath.Lerp(initialOffset, targetOffset, Easing(time / duration));
		}
		externalFovOffset = targetOffset;
		IsAnimatingExternalFov = false;
	}

	private void SetSubjectInternal(Transform subject)
	{
		this.subject = subject;
	}

	private Vector3 GetHardZoneCorrectedViewPlaneOffset(Vector3 viewPlaneOffset, Vector3 cameraPositionBeforeOffset, Vector3 targetTrackedPoint, Vector2 targetTrackedPointViewportPosition)
	{
		Vector3 vector = cameraPositionBeforeOffset + viewPlaneOffset;
		Vector3 vector2 = camera.WorldToViewportPoint(vector, rotation, targetTrackedPoint);
		if (hardZone.Contains(vector2))
		{
			return viewPlaneOffset;
		}
		if (!hardZone.Contains(targetTrackedPointViewportPosition))
		{
			throw new InvalidOperationException("The target is outside of the hard zone, but so is the target viewport position.");
		}
		BGeo.SegmentRectangleIntersection2d(vector2, targetTrackedPointViewportPosition, hardZone.center, hardZone.size, out var closeIntersection, out var _);
		Vector3 lineDirection = camera.ViewportToWorldPoint(vector, rotation, new Vector3(closeIntersection.x, closeIntersection.y, 1f)) - vector;
		if (BGeo.LinePlaneIntersection(targetTrackedPoint, lineDirection, vector, GetCurrentForward(), out var intersection))
		{
			return intersection - cameraPositionBeforeOffset;
		}
		return viewPlaneOffset;
	}

	private void UpdateFrustumPlanes()
	{
		GeometryUtility.CalculateFrustumPlanes(camera, frustumPlanes);
	}

	private Quaternion GetRotation(float pitch, float yaw, float roll)
	{
		return Quaternion.Euler(pitch, yaw, roll);
	}

	private Rect GetAppliedHardZone(Vector2 center, Vector2 size)
	{
		return new Rect(center - size * 0.5f, size);
	}

	private Vector3 GetCurrentTargetTrackedPoint()
	{
		Vector3 vector = ((trackingMode != TrackingMode.ExplicitSubjectSpacePosition) ? new Vector3(subject.position.x, effectiveSubjectWorldBounds.center.y, subject.position.z) : subject.TransformPoint(explicitTrackedPointSubjectSpacePosition));
		switch (trackingOffsetMode)
		{
		case TrackingOffsetMode.SubjectSpace:
			vector += subject.TransformVector(subjectSpaceTrackingOffset);
			break;
		case TrackingOffsetMode.CameraSpace:
			vector += base.transform.TransformVector(cameraSpaceTrackingOffset);
			break;
		}
		float num = 1f;
		num *= subjectYOffsetPitchCurve.Evaluate(normalizedPitch);
		return vector + new Vector3(0f, num, 0f);
	}

	private Vector3 GetSmoothedTrackedPoint(Vector3 trackedPoint, Vector3 targetTrackedPoint, float subjectHeadingForwardness)
	{
		if (isTransitioningSubjects)
		{
			return Vector3.Lerp(trackedPoint, targetTrackedPoint, 40f * Time.deltaTime);
		}
		float num = horizontalFollowSpeedHeadingForwardnessCurve.Evaluate(subjectHeadingForwardness);
		bool flag = targetTrackedPoint.y > trackedPoint.y;
		if (useHorizontalTrackingSpeedForVertical)
		{
			return Vector3.Lerp(trackedPoint, targetTrackedPoint, num * Time.deltaTime);
		}
		return new Vector3(BMath.LerpClamped(trackedPoint.x, targetTrackedPoint.x, num * Time.deltaTime), BMath.LerpClamped(trackedPoint.y, targetTrackedPoint.y, (flag ? upwardsFollowSpeed : downwardsFollowSpeed) * Time.deltaTime), BMath.LerpClamped(trackedPoint.z, targetTrackedPoint.z, num * Time.deltaTime));
	}

	private Vector3 GetSubjectTransitionTargetPoint()
	{
		Vector3 currentTargetTrackedPoint = GetCurrentTargetTrackedPoint();
		return Vector3.Lerp(previousSubjectTrackedPoint, currentTargetTrackedPoint, subjectTransitionFactor);
	}

	private float GetFov(float normalizedPitch)
	{
		return baseFov + fovAdditionPitchCurve.Evaluate(normalizedPitch) + externalFovOffset;
	}

	private float GetSubjectHeadingForwardness()
	{
		return Vector3.Dot(subject.forward.Horizontalized(), GetCurrentForward().Horizontalized());
	}

	private Vector2 GetTargetTrackedPointViewPlanePosition()
	{
		Vector2 result = 0.5f * Vector2.one;
		if (overTheShoulder)
		{
			result.x = overTheShoulderViewportTargetX;
		}
		return result;
	}

	private Vector3 GetRawTargetViewPlaneOffsetFor(Vector3 trackedPoint, Vector3 effectiveCameraPosition, Vector2 targetTrackedPointViewportPosition)
	{
		Vector3 lineDirection = camera.ViewportToWorldPoint(effectiveCameraPosition, rotation, new Vector3(targetTrackedPointViewportPosition.x, targetTrackedPointViewportPosition.y, 1f)) - effectiveCameraPosition;
		if (BGeo.LinePlaneIntersection(trackedPoint, lineDirection, effectiveCameraPosition, GetCurrentForward(), out var intersection))
		{
			return intersection - effectiveCameraPosition;
		}
		return Vector3.zero;
	}

	private float GetRawTargetDistance(Bounds subjectBounds, float fov)
	{
		return ((subjectBounds.extents.y > float.Epsilon) ? subjectBounds.extents.y : 2f) / BMath.Tan(fov * 0.5f * (MathF.PI / 180f));
	}

	private float GetDistancePitchAddition(float normalizedPitch)
	{
		distanceAdditionPitchCurve.Evaluate(normalizedPitch);
		return distanceAdditionPitchCurve.Evaluate(normalizedPitch);
	}

	private float GetSmoothedRawTargetDistance(float currentTargetDistance, Bounds subjectBounds, float fov)
	{
		float to = ((subjectBounds.extents.y > float.Epsilon) ? subjectBounds.extents.y : 2f) / BMath.Tan(fov * 0.5f * (MathF.PI / 180f));
		return BMath.LerpClamped(currentTargetDistance, to, targetDistanceSpeed * Time.deltaTime);
	}

	private float GetSubjectTransitionTargetDistance()
	{
		float num = 1f / BMath.Tan(fov * 0.5f * (MathF.PI / 180f));
		float num2 = previousSubjectBounds.extents.y * num;
		float to = effectiveSubjectWorldBounds.extents.y * num;
		float to2 = BMath.Lerp(num2, to, subjectTransitionFactor);
		return BMath.LerpClamped(targetDistanceRaw, to2, 40f * Time.deltaTime);
	}

	private float PhysicsCorrectTargetDistance(float targetDistance, Vector3 trackedPoint, float nearClipPlaneDistance)
	{
		if (disablePhysics)
		{
			return targetDistance;
		}
		float frustumHeightFromDistance = camera.GetFrustumHeightFromDistance(nearClipPlaneDistance);
		Vector3 vector = new Vector3(camera.GetFrustumWidthFromHeight(frustumHeightFromDistance) + 0.3f, frustumHeightFromDistance + 0.3f, 0.3f) * 0.5f;
		Vector3 vector2 = -GetCurrentForward();
		float num = targetDistance - nearClipPlaneDistance + 1f;
		int num2 = (Physics.CheckBox(trackedPoint, vector, rotation, GameManager.LayerSettings.CameraCollidablesMask, QueryTriggerInteraction.Ignore) ? Physics.RaycastNonAlloc(trackedPoint, vector2, maxDistance: num, layerMask: GameManager.LayerSettings.CameraCollidablesMask, results: raycastHitBuffer, queryTriggerInteraction: QueryTriggerInteraction.Ignore) : Physics.BoxCastNonAlloc(trackedPoint, vector, orientation: rotation, direction: vector2, maxDistance: num, layerMask: GameManager.LayerSettings.CameraCollidablesMask, results: raycastHitBuffer, queryTriggerInteraction: QueryTriggerInteraction.Ignore));
		bool flag = false;
		RaycastHit raycastHit = new RaycastHit
		{
			distance = float.MaxValue
		};
		for (int i = 0; i < num2; i++)
		{
			RaycastHit raycastHit2 = raycastHitBuffer[i];
			if (!collidersToIgnore.Contains(raycastHit2.collider) && raycastHit2.distance < raycastHit.distance)
			{
				flag = true;
				raycastHit = raycastHit2;
			}
		}
		if (flag)
		{
			return BMath.Clamp(raycastHit.distance + nearClipPlaneDistance, 0f, targetDistance);
		}
		return targetDistance;
	}

	private Vector3 PhysicsCorrectViewPlaneOffset(Vector3 viewPlaneOffset, Vector3 cameraPositionBeforeOffset, float nearClipPlaneDistance)
	{
		if (disablePhysics || viewPlaneOffset.sqrMagnitude < 0.001f)
		{
			return viewPlaneOffset;
		}
		float frustumHeightFromDistance = camera.GetFrustumHeightFromDistance(nearClipPlaneDistance);
		Vector3 vector = new Vector3(camera.GetFrustumWidthFromHeight(frustumHeightFromDistance) + 0.3f, frustumHeightFromDistance + 0.3f, 0.3f) * 0.5f;
		Vector3 normalized = viewPlaneOffset.normalized;
		float magnitude = viewPlaneOffset.magnitude;
		int num = (Physics.CheckBox(cameraPositionBeforeOffset, vector, rotation, GameManager.LayerSettings.CameraCollidablesMask, QueryTriggerInteraction.Ignore) ? Physics.RaycastNonAlloc(cameraPositionBeforeOffset, normalized, maxDistance: magnitude, layerMask: GameManager.LayerSettings.CameraCollidablesMask, results: raycastHitBuffer, queryTriggerInteraction: QueryTriggerInteraction.Ignore) : Physics.BoxCastNonAlloc(cameraPositionBeforeOffset, vector, orientation: rotation, direction: normalized, maxDistance: magnitude, layerMask: GameManager.LayerSettings.CameraCollidablesMask, results: raycastHitBuffer, queryTriggerInteraction: QueryTriggerInteraction.Ignore));
		bool flag = false;
		RaycastHit raycastHit = new RaycastHit
		{
			distance = float.MaxValue
		};
		for (int i = 0; i < num; i++)
		{
			RaycastHit raycastHit2 = raycastHitBuffer[i];
			if (!collidersToIgnore.Contains(raycastHit2.collider) && raycastHit2.distance < raycastHit.distance)
			{
				flag = true;
				raycastHit = raycastHit2;
			}
		}
		if (flag)
		{
			return viewPlaneOffset / magnitude * raycastHit.distance;
		}
		return viewPlaneOffset;
	}

	private void FinishTransition()
	{
		isTransitioningSubjects = false;
		if (clearCollidersToIgnoreAfterNextSubjectTransition)
		{
			ClearCollidersToIgnore();
			clearCollidersToIgnoreAfterNextSubjectTransition = false;
		}
	}

	private void VerboseGUI()
	{
		int width = camera.pixelWidth;
		int height = camera.pixelHeight;
		BGui.SetCamera(camera);
		BGui.SetColor(new Color(1f, 0f, 0f, 0.2f));
		DrawDeadZone(hardZone);
		Vector3 currentTargetTrackedPoint = GetCurrentTargetTrackedPoint();
		Vector2 targetTrackedPointViewPlanePosition = GetTargetTrackedPointViewPlanePosition();
		Vector2 center = new Vector2(targetTrackedPointViewPlanePosition.x * (float)width, (1f - targetTrackedPointViewPlanePosition.y) * (float)height);
		Vector2 center2 = camera.WorldToScreenPoint(TrackedPoint);
		Vector2 center3 = camera.WorldToScreenPoint(currentTargetTrackedPoint);
		BGui.SetColor(Color.yellow);
		BGui.DrawOutlineRect(center, 7.5f, 2f, alphaBlend: false);
		BGui.DrawOutlineRect(center2, 5.5f, 2f, alphaBlend: false);
		BGui.DrawRect(center3, 4f);
		BGui.SetColor(new Color(0f, 0f, 0f, 0.1f));
		float num = 5f;
		for (int i = 0; (float)i < num - 1f; i++)
		{
			float num2 = (float)(i + 1) / num;
			float x = (float)width * num2;
			float y = (float)height * num2;
			BGui.DrawLine(new Vector2(x, 0f), new Vector2(x, height), 1f, alphaBlend: true);
			BGui.DrawLine(new Vector2(0f, y), new Vector2(width, y), 1f, alphaBlend: true);
		}
		void DrawDeadZone(Rect zone)
		{
			Rect rect = default(Rect);
			if (zone.xMin > 0f)
			{
				rect.min = new Vector2(0f, zone.yMax * (float)height);
				rect.max = new Vector2(zone.xMin * (float)width, zone.yMin * (float)height);
				BGui.DrawRect(rect, alphaBlend: true);
			}
			if (zone.xMax < 1f)
			{
				rect.min = new Vector2(zone.xMax * (float)width, zone.yMax * (float)height);
				rect.max = new Vector2(width, zone.yMin * (float)height);
				BGui.DrawRect(rect, alphaBlend: true);
			}
			if (zone.yMin > 0f)
			{
				rect.min = Vector2.zero;
				rect.max = new Vector2(width, zone.yMin * (float)height);
				BGui.DrawRect(rect, alphaBlend: true);
			}
			if (zone.yMax < 1f)
			{
				rect.min = new Vector2(0f, zone.yMax * (float)height);
				rect.max = new Vector2(width, height);
				BGui.DrawRect(rect, alphaBlend: true);
			}
		}
	}
}
