using System.Collections.Generic;
using FMOD.Studio;
using FMODUnity;
using UnityEngine;

public class CameraModuleController : SingletonBehaviour<CameraModuleController>, IPreLateBUpdateCallback, IAnyBUpdateCallback
{
	[SerializeField]
	private ImpactFrameController impactFrameController;

	[SerializeField]
	private CrtScreenController crtScreenController;

	private readonly Dictionary<CameraModuleType, CameraModule> modules = new Dictionary<CameraModuleType, CameraModule>();

	private CameraModule previousModule;

	private CameraModule currentModule;

	private CameraModuleType previousModuleType;

	private CameraModuleType currentModuleType;

	private Camera camera;

	private LevelBoundsTracker levelBoundsTracker;

	private bool isTransitioning;

	private float transitionTime;

	private float transitionNormalizedTime;

	private float transitionDuration;

	private AnimationCurve explicitTransitionCurve;

	private bool isShaking;

	private bool isShake3d;

	private float shakeSourceInitialDistance;

	private float currentShakeDuration;

	private bool isShakingAwayFromCenter;

	private ScreenshakeSettings currentScreenshakeSettings;

	private double shakeStartTimestamp;

	private Vector2 shakePositionOffset;

	private Vector2 shakeRotationOffset;

	private float defaultFieldOfView;

	private float defaultNearClipPlane;

	private float defaultFarClipPlane;

	private float defaultOrthographicSize;

	private EventInstance underwaterSnapshot;

	private EventInstance underwaterAmbience;

	private bool isPlayingUnderwaterSounds;

	public float DistanceBelowOutOfBoundsHazardSurface { get; private set; }

	public static CameraModule CurrentModule
	{
		get
		{
			if (!SingletonBehaviour<CameraModuleController>.HasInstance)
			{
				return null;
			}
			return SingletonBehaviour<CameraModuleController>.Instance.currentModule;
		}
	}

	public static CameraModuleType PreviousModuleType
	{
		get
		{
			if (!SingletonBehaviour<CameraModuleController>.HasInstance)
			{
				return CameraModuleType.None;
			}
			return SingletonBehaviour<CameraModuleController>.Instance.previousModuleType;
		}
	}

	public static CameraModuleType CurrentModuleType
	{
		get
		{
			if (!SingletonBehaviour<CameraModuleController>.HasInstance)
			{
				return CameraModuleType.None;
			}
			return SingletonBehaviour<CameraModuleController>.Instance.currentModuleType;
		}
	}

	public static bool IsTransitioning
	{
		get
		{
			if (SingletonBehaviour<CameraModuleController>.HasInstance)
			{
				return SingletonBehaviour<CameraModuleController>.Instance.isTransitioning;
			}
			return false;
		}
	}

	public static float TransitionTime
	{
		get
		{
			if (!SingletonBehaviour<CameraModuleController>.HasInstance)
			{
				return 0f;
			}
			return SingletonBehaviour<CameraModuleController>.Instance.transitionTime;
		}
	}

	public static float TransitionNormalizedTime
	{
		get
		{
			if (!SingletonBehaviour<CameraModuleController>.HasInstance)
			{
				return 0f;
			}
			return SingletonBehaviour<CameraModuleController>.Instance.transitionNormalizedTime;
		}
	}

	public static float TransitionDuration
	{
		get
		{
			if (!SingletonBehaviour<CameraModuleController>.HasInstance)
			{
				return 0f;
			}
			return SingletonBehaviour<CameraModuleController>.Instance.transitionDuration;
		}
	}

	protected override void Awake()
	{
		base.Awake();
		camera = GetComponent<Camera>();
		levelBoundsTracker = GetComponent<LevelBoundsTracker>();
		defaultFieldOfView = camera.fieldOfView;
		defaultNearClipPlane = camera.nearClipPlane;
		defaultFarClipPlane = camera.farClipPlane;
		defaultOrthographicSize = camera.orthographicSize;
		UpdateCrtEffectSettingsInternal();
		bool flag = false;
		CameraModule[] components = GetComponents<CameraModule>();
		foreach (CameraModule cameraModule in components)
		{
			modules.Add(cameraModule.Type, cameraModule);
			cameraModule.enabled = false;
			if (!flag && (currentModuleType == CameraModuleType.None || cameraModule.Type == currentModuleType))
			{
				SetCurrentModule(cameraModule);
				cameraModule.enabled = true;
				flag = true;
			}
		}
		BUpdate.RegisterCallback(this);
	}

	protected override void OnDestroy()
	{
		base.OnDestroy();
		BUpdate.DeregisterCallback(this);
		if (underwaterSnapshot.isValid())
		{
			underwaterSnapshot.stop(FMOD.Studio.STOP_MODE.ALLOWFADEOUT);
		}
		if (underwaterAmbience.isValid())
		{
			underwaterAmbience.stop(FMOD.Studio.STOP_MODE.ALLOWFADEOUT);
		}
	}

	public static void Shake(ScreenshakeSettings settings)
	{
		if (SingletonBehaviour<CameraModuleController>.HasInstance)
		{
			SingletonBehaviour<CameraModuleController>.Instance.ShakeInternal(settings);
		}
	}

	public static void Shake(ScreenshakeSettings settings, Vector3 sourcePosition)
	{
		if (SingletonBehaviour<CameraModuleController>.HasInstance)
		{
			SingletonBehaviour<CameraModuleController>.Instance.ShakeInternal(settings, sourcePosition);
		}
	}

	public static void PlayImpactFrame(Vector3 impactWorldPosition)
	{
		if (SingletonBehaviour<CameraModuleController>.HasInstance)
		{
			SingletonBehaviour<CameraModuleController>.Instance.PlayImpactFrameInternal(impactWorldPosition);
		}
	}

	public static void SetIsCrtEffectEnabled(bool enabled)
	{
		if (SingletonBehaviour<CameraModuleController>.HasInstance)
		{
			SingletonBehaviour<CameraModuleController>.Instance.SetIsCrtEffectEnabledInternal(enabled);
		}
	}

	public static void UpdateCrtEffectSettings()
	{
		if (SingletonBehaviour<CameraModuleController>.HasInstance)
		{
			SingletonBehaviour<CameraModuleController>.Instance.UpdateCrtEffectSettingsInternal();
		}
	}

	public static Vector3 WorldToScreenPoint(Vector3 worldPoint)
	{
		if (!SingletonBehaviour<CameraModuleController>.HasInstance)
		{
			return worldPoint;
		}
		return SingletonBehaviour<CameraModuleController>.Instance.WorldToScreenPointInternal(worldPoint);
	}

	public static Vector3 WorldToViewportPoint(Vector3 worldPoint)
	{
		if (!SingletonBehaviour<CameraModuleController>.HasInstance)
		{
			return worldPoint;
		}
		return SingletonBehaviour<CameraModuleController>.Instance.WorldToViewportPointInternal(worldPoint);
	}

	public void OnPreLateBUpdate()
	{
		if (isTransitioning)
		{
			UpdateTransition(Time.unscaledDeltaTime);
		}
		else
		{
			UpdateCamera();
		}
		UpdateShake(Time.unscaledDeltaTime);
		UpdateOutOfBoundsHazardState();
		void UpdateOutOfBoundsHazardState()
		{
			float previousDistanceBelowSurface = DistanceBelowOutOfBoundsHazardSurface;
			Vector3 vector = base.transform.position + base.transform.forward * camera.nearClipPlane;
			OutOfBoundsHazard hazard = ((levelBoundsTracker.CurrentSecondaryHazardLocalOnly == null) ? MainOutOfBoundsHazard.Type : levelBoundsTracker.CurrentSecondaryHazardLocalOnly.Type);
			DistanceBelowOutOfBoundsHazardSurface = levelBoundsTracker.CurrentOutOfBoundsHazardWorldHeightLocalOnly - vector.y;
			PostProcessingManager.SetDistanceBelowOutOfBoundsHazardSurface(DistanceBelowOutOfBoundsHazardSurface, hazard);
			UpdateUnderwaterSounds();
			void UpdateUnderwaterSounds()
			{
				bool flag = !LoadingScreen.IsVisible && !LoadingScreen.IsFadingScreenOut;
				if (hazard == OutOfBoundsHazard.Water && flag)
				{
					if (previousDistanceBelowSurface < 0.1f && DistanceBelowOutOfBoundsHazardSurface > 0.1f)
					{
						float value = (DistanceBelowOutOfBoundsHazardSurface - previousDistanceBelowSurface) / Time.deltaTime;
						EventInstance eventInstance = RuntimeManager.CreateInstance(GameManager.AudioSettings.CameraSplashIntoWaterEvent);
						eventInstance.setParameterByID(AudioSettings.SpeedId, BMath.InverseLerpClamped(2f, 7f, value));
						eventInstance.start();
						eventInstance.release();
					}
					else if (previousDistanceBelowSurface > -0.1f && DistanceBelowOutOfBoundsHazardSurface < -0.1f)
					{
						float value2 = (DistanceBelowOutOfBoundsHazardSurface - previousDistanceBelowSurface) / Time.deltaTime;
						EventInstance eventInstance2 = RuntimeManager.CreateInstance(GameManager.AudioSettings.CameraSplashOutOfWaterEvent);
						eventInstance2.setParameterByID(AudioSettings.SpeedId, BMath.InverseLerpClamped(-2f, -7f, value2));
						eventInstance2.start();
						eventInstance2.release();
					}
				}
				bool flag2 = isPlayingUnderwaterSounds;
				isPlayingUnderwaterSounds = flag && hazard == OutOfBoundsHazard.Water && DistanceBelowOutOfBoundsHazardSurface >= 0f;
				if (isPlayingUnderwaterSounds == flag2)
				{
					if (isPlayingUnderwaterSounds)
					{
						underwaterSnapshot.setParameterByID(AudioSettings.CameraInWaterId, BMath.InverseLerpClamped(0f, 0.4f, DistanceBelowOutOfBoundsHazardSurface));
					}
				}
				else if (isPlayingUnderwaterSounds)
				{
					underwaterSnapshot = RuntimeManager.CreateInstance(GameManager.AudioSettings.UnderwaterCameraSnapshot);
					underwaterSnapshot.setParameterByID(AudioSettings.CameraInWaterId, BMath.InverseLerpClamped(0f, 0.4f, DistanceBelowOutOfBoundsHazardSurface));
					underwaterSnapshot.start();
					underwaterSnapshot.release();
					underwaterAmbience = RuntimeManager.CreateInstance(GameManager.AudioSettings.UnderwaterCameraAmbience);
					underwaterAmbience.start();
					underwaterAmbience.release();
				}
				else
				{
					if (underwaterSnapshot.isValid())
					{
						underwaterSnapshot.stop(FMOD.Studio.STOP_MODE.ALLOWFADEOUT);
					}
					if (underwaterAmbience.isValid())
					{
						underwaterAmbience.stop(FMOD.Studio.STOP_MODE.ALLOWFADEOUT);
					}
				}
			}
		}
	}

	public static bool TryGetModule(CameraModuleType type, out CameraModule module)
	{
		if (!SingletonBehaviour<CameraModuleController>.HasInstance)
		{
			module = null;
			return false;
		}
		return SingletonBehaviour<CameraModuleController>.Instance.modules.TryGetValue(type, out module);
	}

	public static bool TryGetOrbitModule(out OrbitCameraModule orbitModule)
	{
		orbitModule = null;
		if (!SingletonBehaviour<CameraModuleController>.HasInstance)
		{
			return false;
		}
		if (!SingletonBehaviour<CameraModuleController>.Instance.modules.TryGetValue(CameraModuleType.Orbit, out var value))
		{
			return false;
		}
		orbitModule = value as OrbitCameraModule;
		return true;
	}

	public static void TransitionTo(CameraModuleType cameraType, float duration)
	{
		if (SingletonBehaviour<CameraModuleController>.HasInstance)
		{
			SingletonBehaviour<CameraModuleController>.Instance.TransitionToInternal(cameraType, duration, null);
		}
	}

	public static void TransitionTo(CameraModuleType cameraType, AnimationCurve explicitTransitionCurve)
	{
		if (SingletonBehaviour<CameraModuleController>.HasInstance)
		{
			SingletonBehaviour<CameraModuleController>.Instance.TransitionToInternal(cameraType, -1f, explicitTransitionCurve);
		}
	}

	private void ShakeInternal(ScreenshakeSettings settings)
	{
		if (CanBeginShake())
		{
			currentScreenshakeSettings = settings;
			isShaking = true;
			isShake3d = false;
			currentShakeDuration = settings.Duration;
			isShakingAwayFromCenter = true;
			shakeStartTimestamp = Time.unscaledTimeAsDouble;
		}
		bool CanBeginShake()
		{
			if (!CanShakeAtAll())
			{
				return false;
			}
			bool flag = settings.Type.HasType(ScreenshakeType.Position);
			bool flag2 = settings.Type.HasType(ScreenshakeType.Rotation);
			if (!flag && !flag2)
			{
				return false;
			}
			if (!isShaking)
			{
				return true;
			}
			if (flag)
			{
				float screenShakePositionIntensity = GetScreenShakePositionIntensity();
				if (settings.GetPositionIntensity(0f) >= screenShakePositionIntensity)
				{
					return true;
				}
			}
			if (flag2)
			{
				float screenShakeRotationIntensity = GetScreenShakeRotationIntensity();
				if (settings.GetRotationIntensity(0f) >= screenShakeRotationIntensity)
				{
					return true;
				}
			}
			return false;
		}
	}

	private void ShakeInternal(ScreenshakeSettings settings, Vector3 sourcePosition)
	{
		if (CanBeginShake(out var duration))
		{
			currentScreenshakeSettings = settings;
			isShaking = true;
			isShake3d = true;
			shakeSourceInitialDistance = (sourcePosition - base.transform.position).magnitude;
			currentShakeDuration = duration;
			isShakingAwayFromCenter = true;
			shakeStartTimestamp = Time.unscaledTimeAsDouble;
		}
		bool CanBeginShake(out float reference)
		{
			reference = 0f;
			if (!CanShakeAtAll())
			{
				return false;
			}
			float magnitude = (sourcePosition - base.transform.position).magnitude;
			reference = settings.DistanceDurationFactor.Evaluate(shakeSourceInitialDistance) * settings.Duration;
			if (reference <= 0f)
			{
				return false;
			}
			if (!isShaking)
			{
				if (!(settings.GetPositionIntensity3d(0f, magnitude) > 0f))
				{
					return settings.GetRotationIntensity3d(0f, magnitude) > 0f;
				}
				return true;
			}
			float screenShakePositionIntensity = GetScreenShakePositionIntensity();
			if (settings.GetPositionIntensity3d(0f, magnitude) >= screenShakePositionIntensity)
			{
				return true;
			}
			float screenShakeRotationIntensity = GetScreenShakeRotationIntensity();
			if (settings.GetRotationIntensity3d(0f, magnitude) >= screenShakeRotationIntensity)
			{
				return true;
			}
			return false;
		}
	}

	private void PlayImpactFrameInternal(Vector3 impactWorldPosition)
	{
		impactFrameController.PlayImpactFrame(impactWorldPosition);
	}

	private void SetIsCrtEffectEnabledInternal(bool enabled)
	{
		bool flag = crtScreenController.IsEnabled();
		if (enabled != flag)
		{
			crtScreenController.SetCrtScreenEnabled(enabled);
			if (enabled)
			{
				UpdateCrtEffectSettingsInternal();
			}
		}
	}

	private void UpdateCrtEffectSettingsInternal()
	{
		crtScreenController.SetBulgeEnabled(GameSettings.All.Graphics.CrtDistortionEnabled);
		crtScreenController.SetAberrationEnabled(GameSettings.All.Graphics.CrtChromaticAberrationEnabled);
		crtScreenController.SetScanlineEnabled(GameSettings.All.Graphics.CrtScanLinesEnabled);
	}

	private Vector3 WorldToScreenPointInternal(Vector3 worldPoint)
	{
		Vector3 vector = camera.WorldToScreenPoint(worldPoint);
		if (!crtScreenController.TryApplyBulgeOffsetToScreenPoint(vector, out var buldgedScreenPoint))
		{
			return vector;
		}
		return buldgedScreenPoint;
	}

	private Vector3 WorldToViewportPointInternal(Vector3 worldPoint)
	{
		Vector3 vector = camera.WorldToViewportPoint(worldPoint);
		if (!crtScreenController.TryApplyBulgeOffsetToViewportPoint(vector, out var buldgedViewportPoint))
		{
			return vector;
		}
		return buldgedViewportPoint;
	}

	private void UpdateCamera()
	{
		currentModule.UpdateModule();
		base.transform.SetPositionAndRotation(currentModule.position, currentModule.rotation);
		if (currentModule.ControlsFieldOfView)
		{
			camera.fieldOfView = currentModule.FieldOfView;
		}
	}

	private void TransitionToInternal(CameraModuleType cameraType, float duration, AnimationCurve explicitTransitionCurve)
	{
		if (currentModuleType == cameraType)
		{
			return;
		}
		CameraModule value;
		if (!base.didAwake)
		{
			currentModuleType = cameraType;
		}
		else if (modules.TryGetValue(cameraType, out value))
		{
			isTransitioning = true;
			transitionTime = 0f;
			transitionNormalizedTime = 0f;
			if (explicitTransitionCurve != null)
			{
				transitionDuration = explicitTransitionCurve.keys[^1].time;
				this.explicitTransitionCurve = explicitTransitionCurve;
			}
			else
			{
				transitionDuration = duration;
				this.explicitTransitionCurve = null;
			}
			SetPreviousModule(currentModule);
			SetCurrentModule(value);
			currentModule.enabled = true;
			previousModule.OnTransitionStart(transitioningToThis: false);
			currentModule.OnTransitionStart(transitioningToThis: true);
			if (transitionDuration <= 0f)
			{
				EndTransition();
			}
		}
	}

	private void SetCurrentModule(CameraModule module)
	{
		currentModule = module;
		currentModuleType = ((module != null) ? module.Type : CameraModuleType.None);
	}

	private void SetPreviousModule(CameraModule module)
	{
		previousModule = module;
		previousModuleType = ((module != null) ? module.Type : CameraModuleType.None);
	}

	private void UpdateTransition(float deltaTime)
	{
		isTransitioning = true;
		transitionTime += deltaTime;
		transitionNormalizedTime = transitionTime / transitionDuration;
		if (transitionNormalizedTime > 1f)
		{
			EndTransition();
			return;
		}
		float t = ((explicitTransitionCurve == null) ? BMath.EaseInOut(BMath.EaseInOut(transitionNormalizedTime)) : explicitTransitionCurve.Evaluate(transitionTime));
		previousModule.UpdateModule();
		currentModule.UpdateModule();
		Vector3 position = Vector3.Lerp(previousModule.position, currentModule.position, t);
		Quaternion rotation = Quaternion.Slerp(previousModule.rotation, currentModule.rotation, t);
		base.transform.SetPositionAndRotation(position, rotation);
		float num = (previousModule.ControlsFieldOfView ? previousModule.FieldOfView : defaultFieldOfView);
		float to = (currentModule.ControlsFieldOfView ? currentModule.FieldOfView : defaultFieldOfView);
		camera.fieldOfView = BMath.Lerp(num, to, t);
	}

	private void EndTransition()
	{
		previousModule.enabled = false;
		previousModule.OnTransitionEnd(transitionedToThis: false);
		currentModule.OnTransitionEnd(transitionedToThis: true);
		base.transform.SetPositionAndRotation(currentModule.position, currentModule.rotation);
		camera.ResetProjectionMatrix();
		camera.fieldOfView = (currentModule.ControlsFieldOfView ? currentModule.FieldOfView : defaultFieldOfView);
		isTransitioning = false;
	}

	private void UpdateShake(float deltaTime)
	{
		if (!isShaking)
		{
			return;
		}
		float num = BMath.GetUnscaledTimeSince(shakeStartTimestamp) / currentShakeDuration;
		if (num >= 1f)
		{
			EndShake();
			return;
		}
		Vector2 b = Vector2.zero;
		Vector2 b2 = Vector2.zero;
		if (isShakingAwayFromCenter)
		{
			float num2;
			float num3;
			if (isShake3d)
			{
				num2 = currentScreenshakeSettings.GetPositionIntensity3d(num, shakeSourceInitialDistance);
				num3 = currentScreenshakeSettings.GetRotationIntensity3d(num, shakeSourceInitialDistance);
			}
			else
			{
				num2 = currentScreenshakeSettings.GetPositionIntensity(num);
				num3 = currentScreenshakeSettings.GetRotationIntensity(num);
			}
			float screenshakeFactor = GameSettings.All.General.ScreenshakeFactor;
			b = num2 * screenshakeFactor * Random.insideUnitCircle;
			b2 = num3 * screenshakeFactor * Random.insideUnitCircle;
		}
		float t = currentScreenshakeSettings.SmoothingSpeed * deltaTime;
		shakePositionOffset = Vector2.Lerp(shakePositionOffset, b, t);
		shakeRotationOffset = Vector2.Lerp(shakeRotationOffset, b2, t);
		base.transform.position += shakePositionOffset.x * base.transform.right + shakePositionOffset.y * base.transform.up;
		base.transform.rotation *= Quaternion.Euler(shakeRotationOffset.x, shakeRotationOffset.y, 0f);
		isShakingAwayFromCenter = !isShakingAwayFromCenter;
	}

	private bool CanShakeAtAll()
	{
		if (GameSettings.All.General.ScreenshakeFactor <= 0f)
		{
			return false;
		}
		return true;
	}

	private float GetScreenShakePositionIntensity()
	{
		if (!isShaking)
		{
			return 0f;
		}
		float normalizedTime = BMath.GetUnscaledTimeSince(shakeStartTimestamp) / currentShakeDuration;
		if (isShake3d)
		{
			return currentScreenshakeSettings.GetPositionIntensity3d(normalizedTime, shakeSourceInitialDistance);
		}
		return currentScreenshakeSettings.GetPositionIntensity(normalizedTime);
	}

	private float GetScreenShakeRotationIntensity()
	{
		if (!isShaking)
		{
			return 0f;
		}
		float normalizedTime = BMath.GetUnscaledTimeSince(shakeStartTimestamp) / currentShakeDuration;
		if (isShake3d)
		{
			return currentScreenshakeSettings.GetRotationIntensity3d(normalizedTime, shakeSourceInitialDistance);
		}
		return currentScreenshakeSettings.GetRotationIntensity(normalizedTime);
	}

	private void EndShake()
	{
		shakePositionOffset = Vector3.zero;
		shakeRotationOffset = Vector3.zero;
		isShaking = false;
	}
}
