using System.Collections;
using UnityEngine;

public class GameplayCameraManager : SingletonBehaviour<GameplayCameraManager>
{
	private bool isInAimCamera;

	private Vector3 aimCameraTrackingOffset;

	private float aimCameraDistanceAddition;

	private float swingChargeCameraDistanceAddition;

	private Coroutine aimCameraOffsetRoutine;

	private Coroutine swingChargeCameraOffsetRoutine;

	private static Vector3[] explosionVisibilityPointWorldOffsets;

	private CameraModuleType desiredCameraType;

	private static bool forceEnabledCrtEffect;

	private static bool photoCameraEnabled;

	[CCommand("forceEnabledCrtEffect", "", false, false)]
	private static void SetForceEnabledCrtEffect(bool forced)
	{
		if (SingletonBehaviour<GameplayCameraManager>.HasInstance)
		{
			forceEnabledCrtEffect = forced;
			SingletonBehaviour<GameplayCameraManager>.Instance.UpdateCrtEffect();
		}
	}

	[CCommand("togglePhotoCamera", "", false, false)]
	private static void TogglePhotoCamera()
	{
		if (SingletonBehaviour<GameplayCameraManager>.HasInstance)
		{
			photoCameraEnabled = !photoCameraEnabled;
			CameraModuleType cameraModuleType = (photoCameraEnabled ? CameraModuleType.Photo : SingletonBehaviour<GameplayCameraManager>.Instance.desiredCameraType);
			if (CameraModuleController.CurrentModuleType != cameraModuleType)
			{
				CameraModuleController.TransitionTo(cameraModuleType, 0f);
			}
		}
	}

	protected override void Awake()
	{
		base.Awake();
		UpdateOrbitCameraSubject(canSnapYaw: true);
		if (explosionVisibilityPointWorldOffsets == null)
		{
			explosionVisibilityPointWorldOffsets = new Vector3[7]
			{
				Vector3.zero,
				Vector3.up,
				Vector3.forward,
				Vector3.right,
				Vector3.back,
				Vector3.left,
				Vector3.down
			};
		}
		UpdateCrtEffect();
		OrbitCameraModule.OrbitCameraAwoken += OnOrbitCameraAwoken;
		GameManager.LocalPlayerRegistered += OnLocalPlayerRegistered;
		GameManager.LocalPlayerDeregistered += OnLocalPlayerDeregistered;
		PlayerMovement.LocalPlayerTeleportedToRespawnPosition += OnLocalPlayerTeleportedToRespawnPosition;
		PlayerInfo.LocalPlayerEnteredGolfCart += OnLocalPlayerEnteredGolfCart;
		PlayerInfo.LocalPlayerExitedGolfCart += OnLocalPlayerExitedGolfCart;
		PlayerInventory.LocalPlayerSuccessfullyEnteredReservedGolfCart += OnLocalPlayerSuccessfullyEnteredReservedGolfCart;
		PlayerSpectator.LocalPlayerIsSpectatingChanged += OnLocalPlayerIsSpectatingChanged;
		PlayerSpectator.LocalPlayerSetSpectatingTarget += OnLocalPlayerSpectatingTargetSet;
		GameSettings.GraphicsSettings.CrtSettingsChanged += OnCrtSettingsChanged;
	}

	protected override void OnDestroy()
	{
		base.OnDestroy();
		OrbitCameraModule.OrbitCameraAwoken -= OnOrbitCameraAwoken;
		GameManager.LocalPlayerRegistered -= OnLocalPlayerRegistered;
		GameManager.LocalPlayerDeregistered -= OnLocalPlayerDeregistered;
		PlayerMovement.LocalPlayerTeleportedToRespawnPosition -= OnLocalPlayerTeleportedToRespawnPosition;
		PlayerInfo.LocalPlayerEnteredGolfCart -= OnLocalPlayerEnteredGolfCart;
		PlayerInfo.LocalPlayerExitedGolfCart -= OnLocalPlayerExitedGolfCart;
		PlayerInventory.LocalPlayerSuccessfullyEnteredReservedGolfCart -= OnLocalPlayerSuccessfullyEnteredReservedGolfCart;
		PlayerSpectator.LocalPlayerIsSpectatingChanged -= OnLocalPlayerIsSpectatingChanged;
		PlayerSpectator.LocalPlayerSetSpectatingTarget -= OnLocalPlayerSpectatingTargetSet;
		GameSettings.GraphicsSettings.CrtSettingsChanged -= OnCrtSettingsChanged;
	}

	public static void TransitionTo(CameraModuleType cameraType, float duration)
	{
		if (SingletonBehaviour<GameplayCameraManager>.HasInstance)
		{
			SingletonBehaviour<GameplayCameraManager>.Instance.TransitionToInternal(cameraType, duration);
		}
	}

	public static void TransitionTo(CameraModuleType cameraType, AnimationCurve explicitTransitionCurve)
	{
		if (SingletonBehaviour<GameplayCameraManager>.HasInstance)
		{
			SingletonBehaviour<GameplayCameraManager>.Instance.TransitionToInternal(cameraType, explicitTransitionCurve);
		}
	}

	public static void ReachOrbitCameraSteadyState()
	{
		if (SingletonBehaviour<GameplayCameraManager>.HasInstance)
		{
			SingletonBehaviour<GameplayCameraManager>.Instance.ReachOrbitCameraSteadyStateInternal();
		}
	}

	public static void UpdateAimCamera()
	{
		if (SingletonBehaviour<GameplayCameraManager>.HasInstance)
		{
			SingletonBehaviour<GameplayCameraManager>.Instance.UpdateAimCameraInternal();
		}
	}

	public static void StartSwingCharge()
	{
		if (SingletonBehaviour<GameplayCameraManager>.HasInstance)
		{
			SingletonBehaviour<GameplayCameraManager>.Instance.StartSwingChargeInternal();
		}
	}

	public static void EndSwingCharge()
	{
		if (SingletonBehaviour<GameplayCameraManager>.HasInstance)
		{
			SingletonBehaviour<GameplayCameraManager>.Instance.EndSwingChargeInternal();
		}
	}

	public static bool ShouldPlayImpactFrameForExplosion(Vector3 worldPosition, float radius, float maxImpactFrameDistanceSquared)
	{
		if (!SingletonBehaviour<GameplayCameraManager>.HasInstance)
		{
			return false;
		}
		return SingletonBehaviour<GameplayCameraManager>.Instance.ShouldPlayImpactFrameForExplosionInternal(worldPosition, radius, maxImpactFrameDistanceSquared);
	}

	private void Update()
	{
		if (GameManager.LocalPlayerAsGolfer != null && GameManager.LocalPlayerAsGolfer.IsChargingSwing)
		{
			swingChargeCameraDistanceAddition = BMath.LerpClamped(0f, GameManager.CameraGameplaySettings.MaxSwingChargeDistanceAddition, GameManager.LocalPlayerAsGolfer.SwingNormalizedCharge);
			if (CameraModuleController.TryGetOrbitModule(out var orbitModule))
			{
				orbitModule.SetDistanceAddition(aimCameraDistanceAddition + swingChargeCameraDistanceAddition);
			}
		}
	}

	private void UpdateOrbitCameraSubject(bool canSnapYaw)
	{
		if (!CameraModuleController.TryGetOrbitModule(out var orbitModule) || GameManager.LocalPlayerMovement == null)
		{
			return;
		}
		if (GameManager.LocalPlayerAsSpectator.IsSpectating)
		{
			orbitModule.SetSubject(GameManager.LocalPlayerAsSpectator.Target);
			if (canSnapYaw)
			{
				orbitModule.SetYaw(GameManager.LocalPlayerAsSpectator.Target.forward.GetYawDeg());
			}
			orbitModule.SetSubjectLocalBounds(GameManager.LocalPlayerAsSpectator.GetTargetLocalBounds());
		}
		else if (GameManager.LocalPlayerInfo.ActiveGolfCartSeat.IsValid())
		{
			orbitModule.SetSubject(GameManager.LocalPlayerInfo.ActiveGolfCartSeat.golfCart.transform);
			orbitModule.SetSubjectLocalBounds(GameManager.LocalPlayerInfo.ActiveGolfCartSeat.golfCart.GetOrbitCameraSubjectLocalBounds());
		}
		else
		{
			orbitModule.SetSubject(GameManager.LocalPlayerMovement.transform);
			if (canSnapYaw)
			{
				orbitModule.SetYaw(GameManager.LocalPlayerMovement.Yaw);
			}
			orbitModule.SetSubjectLocalBounds(GameManager.LocalPlayerMovement.GetOrbitCameraSubjectLocalBounds());
		}
		orbitModule.ForceUpdateModule();
	}

	private void TransitionToInternal(CameraModuleType cameraType, float duration)
	{
		desiredCameraType = cameraType;
		CameraModuleController.TransitionTo(cameraType, duration);
	}

	private void TransitionToInternal(CameraModuleType cameraType, AnimationCurve explicitTransitionCurve)
	{
		desiredCameraType = cameraType;
		CameraModuleController.TransitionTo(cameraType, explicitTransitionCurve);
	}

	private void ReachOrbitCameraSteadyStateInternal()
	{
		if (CameraModuleController.TryGetOrbitModule(out var orbitModule))
		{
			orbitModule.ForceUpdateModule();
		}
	}

	private void UpdateAimCameraInternal()
	{
		bool flag = isInAimCamera;
		isInAimCamera = ShouldBeInAimCamera();
		if (isInAimCamera != flag)
		{
			if (isInAimCamera)
			{
				EnterAimCameraInternal();
			}
			else
			{
				ExitAimCameraInternal();
			}
		}
		void EnterAimCameraInternal()
		{
			if (aimCameraOffsetRoutine != null)
			{
				StopCoroutine(aimCameraOffsetRoutine);
			}
			aimCameraOffsetRoutine = StartCoroutine(AimCameraOffsetRoutine(isAimingSwing: true));
		}
		void ExitAimCameraInternal()
		{
			if (aimCameraOffsetRoutine != null)
			{
				StopCoroutine(aimCameraOffsetRoutine);
			}
			aimCameraOffsetRoutine = StartCoroutine(AimCameraOffsetRoutine(isAimingSwing: false));
		}
		static bool ShouldBeInAimCamera()
		{
			if (GameManager.LocalPlayerInfo == null)
			{
				return false;
			}
			if (GameManager.LocalPlayerAsGolfer.IsAimingSwing)
			{
				return true;
			}
			if (GameManager.LocalPlayerInventory.IsAimingItem)
			{
				return true;
			}
			return false;
		}
	}

	private void StartSwingChargeInternal()
	{
		if (swingChargeCameraOffsetRoutine != null)
		{
			StopCoroutine(swingChargeCameraOffsetRoutine);
		}
	}

	private void EndSwingChargeInternal()
	{
		if (swingChargeCameraOffsetRoutine != null)
		{
			StopCoroutine(swingChargeCameraOffsetRoutine);
		}
		swingChargeCameraOffsetRoutine = StartCoroutine(ReleaseSwingChargeCameraOffsetRoutine());
	}

	private bool ShouldPlayImpactFrameForExplosionInternal(Vector3 worldPosition, float radius, float maxImpactFrameDistanceSquared)
	{
		if (!GameSettings.All.General.FlashingEffects)
		{
			return false;
		}
		float sqrMagnitude = (worldPosition - GameManager.Camera.transform.position).sqrMagnitude;
		if (sqrMagnitude > maxImpactFrameDistanceSquared)
		{
			return false;
		}
		if (sqrMagnitude < radius * radius * 2f * 2f)
		{
			return true;
		}
		float num = radius / 2f;
		bool flag = false;
		Vector3[] array = explosionVisibilityPointWorldOffsets;
		foreach (Vector3 vector in array)
		{
			if (IsPointOnScreen(worldPosition + vector * num))
			{
				flag = true;
				break;
			}
		}
		if (!flag)
		{
			return false;
		}
		bool flag2 = true;
		array = explosionVisibilityPointWorldOffsets;
		foreach (Vector3 vector2 in array)
		{
			if (!IsPointOccluded(worldPosition + vector2 * num))
			{
				flag2 = false;
				break;
			}
		}
		if (flag2)
		{
			return false;
		}
		return true;
		static bool IsPointOccluded(Vector3 worldPoint)
		{
			Vector3 position = GameManager.Camera.transform.position;
			Vector3 direction = worldPoint - position;
			return Physics.Raycast(new Ray(position, direction), direction.magnitude, GameManager.LayerSettings.ExplosionOccludersMask, QueryTriggerInteraction.Ignore);
		}
		static bool IsPointOnScreen(Vector3 worldPoint)
		{
			Vector3 vector3 = GameManager.Camera.WorldToViewportPoint(worldPoint);
			if (vector3.z > 0f && vector3.x > 0f && vector3.y > 0f && vector3.x < 1f)
			{
				return vector3.y < 1f;
			}
			return false;
		}
	}

	private IEnumerator AimCameraOffsetRoutine(bool isAimingSwing)
	{
		if (CameraModuleController.TryGetOrbitModule(out var orbitCamera))
		{
			orbitCamera.SetUseHorizontalTrackingSpeedForVertical(shouldUse: true);
			Vector3 initialTrackingOffset = aimCameraTrackingOffset;
			float initialDistanceAddition = aimCameraDistanceAddition;
			float duration;
			Vector3 targetTrackingOffset;
			float targetDistanceAddition;
			if (isAimingSwing)
			{
				duration = GameManager.CameraGameplaySettings.IntoSwingAimTransitionDuration;
				targetTrackingOffset = new Vector3(GameManager.GolfSettings.SwingHitBoxLocalCenter.x, GameManager.CameraGameplaySettings.SwingAimVerticalTrackingOffset, 0f);
				targetDistanceAddition = GameManager.CameraGameplaySettings.SwingAimDistanceAddition;
			}
			else
			{
				duration = GameManager.CameraGameplaySettings.OutOfSwingAimTransitionDuration;
				targetTrackingOffset = Vector3.zero;
				targetDistanceAddition = 0f;
			}
			float time = 0f;
			while (time < duration)
			{
				yield return null;
				time += Time.deltaTime;
				float t = time / duration;
				float t2 = (isAimingSwing ? BMath.EaseOut(t) : BMath.EaseIn(t));
				aimCameraTrackingOffset = Vector3.LerpUnclamped(initialTrackingOffset, targetTrackingOffset, t2);
				orbitCamera.SetCameraSpaceTrackingOffset(aimCameraTrackingOffset);
				aimCameraDistanceAddition = BMath.Lerp(initialDistanceAddition, targetDistanceAddition, t2);
				orbitCamera.SetDistanceAddition(aimCameraDistanceAddition + swingChargeCameraDistanceAddition);
			}
			aimCameraTrackingOffset = targetTrackingOffset;
			orbitCamera.SetCameraSpaceTrackingOffset(aimCameraTrackingOffset);
			aimCameraDistanceAddition = targetDistanceAddition;
			orbitCamera.SetDistanceAddition(aimCameraDistanceAddition + swingChargeCameraDistanceAddition);
			yield return new WaitForSeconds(0.3f);
			orbitCamera.SetUseHorizontalTrackingSpeedForVertical(shouldUse: false);
		}
	}

	private IEnumerator ReleaseSwingChargeCameraOffsetRoutine()
	{
		if (CameraModuleController.TryGetOrbitModule(out var orbitCamera))
		{
			float initialDistanceAddition = swingChargeCameraDistanceAddition;
			float time = 0f;
			while (time < GameManager.CameraGameplaySettings.SwingChargeDistanceAdditionReleaseDuration)
			{
				yield return null;
				time += Time.deltaTime;
				float t = time / GameManager.CameraGameplaySettings.SwingChargeDistanceAdditionReleaseDuration;
				swingChargeCameraDistanceAddition = BMath.Lerp(initialDistanceAddition, 0f, BMath.EaseOut(t));
				orbitCamera.SetDistanceAddition(aimCameraDistanceAddition + swingChargeCameraDistanceAddition);
			}
			swingChargeCameraDistanceAddition = 0f;
			orbitCamera.SetDistanceAddition(aimCameraDistanceAddition + swingChargeCameraDistanceAddition);
		}
	}

	private void UpdateCrtEffect()
	{
		CameraModuleController.SetIsCrtEffectEnabled(ShouldEnable());
		CameraModuleController.UpdateCrtEffectSettings();
		static bool ShouldEnable()
		{
			if (forceEnabledCrtEffect)
			{
				return true;
			}
			if (GameManager.LocalPlayerInfo == null)
			{
				return false;
			}
			if (!GameManager.LocalPlayerAsSpectator.IsSpectating)
			{
				return false;
			}
			if (!GameSettings.All.Graphics.CrtEnabled)
			{
				return false;
			}
			return true;
		}
	}

	private void OnOrbitCameraAwoken()
	{
		UpdateOrbitCameraSubject(canSnapYaw: true);
	}

	private void OnLocalPlayerRegistered()
	{
		UpdateOrbitCameraSubject(canSnapYaw: true);
		UpdateCrtEffect();
	}

	private void OnLocalPlayerDeregistered()
	{
		UpdateOrbitCameraSubject(canSnapYaw: true);
		UpdateCrtEffect();
	}

	private void OnLocalPlayerTeleportedToRespawnPosition()
	{
		UpdateOrbitCameraSubject(canSnapYaw: true);
	}

	private void OnLocalPlayerEnteredGolfCart(bool fromDriverSeatReservation)
	{
		if (!fromDriverSeatReservation)
		{
			UpdateOrbitCameraSubject(canSnapYaw: false);
		}
	}

	private void OnLocalPlayerExitedGolfCart()
	{
		UpdateOrbitCameraSubject(canSnapYaw: false);
	}

	private void OnLocalPlayerSuccessfullyEnteredReservedGolfCart()
	{
		UpdateOrbitCameraSubject(canSnapYaw: false);
	}

	private void OnLocalPlayerIsSpectatingChanged()
	{
		if (!GameManager.LocalPlayerAsSpectator.IsSpectating)
		{
			UpdateOrbitCameraSubject(canSnapYaw: false);
		}
		UpdateCrtEffect();
	}

	private void OnLocalPlayerSpectatingTargetSet(bool isInitialTarget)
	{
		UpdateOrbitCameraSubject(isInitialTarget && GameManager.LocalPlayerAsSpectator.TargetPlayer != null);
	}

	private void OnCrtSettingsChanged()
	{
		UpdateCrtEffect();
	}
}
