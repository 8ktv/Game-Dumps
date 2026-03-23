using System;
using System.Collections;
using System.Runtime.InteropServices;
using FMOD.Studio;
using FMODUnity;
using Mirror;
using Mirror.RemoteCalls;
using UnityEngine;

public class GolfCartMovement : NetworkBehaviour, IBUpdateCallback, IAnyBUpdateCallback, IFixedBUpdateCallback
{
	private const int accelerationCyclicBufferSampleCount = 30;

	[SerializeField]
	private WheelCollider frontLeftWheelCollider;

	[SerializeField]
	private WheelCollider frontRightWheelCollider;

	[SerializeField]
	private WheelCollider backLeftWheelCollider;

	[SerializeField]
	private WheelCollider backRightWheelCollider;

	[SerializeField]
	private Transform frontLeftWheelTransform;

	[SerializeField]
	private Transform frontRightWheelTransform;

	[SerializeField]
	private Transform backLeftWheelTransform;

	[SerializeField]
	private Transform backRightWheelTransform;

	[SerializeField]
	private GolfCartMovementSettings settings;

	[SyncVar(hook = "OnIsAcceleratingChanged")]
	private bool isAccelerating;

	[SyncVar(hook = "OnIsBrakingChanged")]
	private bool isBraking;

	[SyncVar]
	private float steering;

	private int forwardInput;

	private bool isWadingInWater;

	private EventInstance waterWadingSound;

	private bool isPlayingWaterWadingSound;

	private float wheelRpmExponentialMovingAverage;

	private const int wheelRpmExponentialMovingAverageWindowSize = 20;

	private const float wheelRpmExponentialMovingAverageAlpha = 2f / 21f;

	private Vector3[] localAccelerationRollingBuffer;

	private int accelerationBufferIndex;

	private Vector3 previousVelocity;

	private Vector3 rawLocalAcceleration;

	private bool isJumping;

	private double jumpTimestamp = double.MinValue;

	private bool shouldPlayJumpLandEffectsWhenGrounding;

	private Coroutine jumpRoutine;

	private double collisionEffectsTimestamp = double.MinValue;

	private Coroutine pipeVfxDelayedStopRoutine;

	private bool isDelayingPipeVfxStop;

	private readonly AntiCheatPerPlayerRateChecker serverInformTriggeredJumpCommandRateLimiter = new AntiCheatPerPlayerRateChecker("Inform golf cart jump triggered", 0.1f, 5, 10, 1f);

	public Action<bool, bool> _Mirror_SyncVarHookDelegate_isAccelerating;

	public Action<bool, bool> _Mirror_SyncVarHookDelegate_isBraking;

	public float SmoothedSteering { get; private set; }

	public Vector3 SmoothedLocalAcceleration { get; private set; }

	public Entity AsEntity { get; private set; }

	public GolfCartInfo GolfCartInfo { get; private set; }

	public bool NetworkisAccelerating
	{
		get
		{
			return isAccelerating;
		}
		[param: In]
		set
		{
			GeneratedSyncVarSetter(value, ref isAccelerating, 1uL, _Mirror_SyncVarHookDelegate_isAccelerating);
		}
	}

	public bool NetworkisBraking
	{
		get
		{
			return isBraking;
		}
		[param: In]
		set
		{
			GeneratedSyncVarSetter(value, ref isBraking, 2uL, _Mirror_SyncVarHookDelegate_isBraking);
		}
	}

	public float Networksteering
	{
		get
		{
			return steering;
		}
		[param: In]
		set
		{
			GeneratedSyncVarSetter(value, ref steering, 4uL, null);
		}
	}

	private void Awake()
	{
		GetComponent<Rigidbody>().SetCenterOfMassAndInertiaTensor(settings.LocalCenterOfMass);
		AsEntity = GetComponent<Entity>();
		GolfCartInfo = GetComponent<GolfCartInfo>();
		localAccelerationRollingBuffer = new Vector3[30];
		Collider[] componentsInChildren = GetComponentsInChildren<Collider>(includeInactive: true);
		Collider[] array = componentsInChildren;
		foreach (Collider collider in array)
		{
			Physics.IgnoreCollision(collider, frontLeftWheelCollider);
			Physics.IgnoreCollision(collider, frontRightWheelCollider);
			Physics.IgnoreCollision(collider, backLeftWheelCollider);
			Physics.IgnoreCollision(collider, backRightWheelCollider);
		}
		array = componentsInChildren;
		for (int i = 0; i < array.Length; i++)
		{
			array[i].hasModifiableContacts = true;
		}
		frontLeftWheelCollider.excludeLayers = GameManager.LayerSettings.FoliageMask;
		frontRightWheelCollider.excludeLayers = GameManager.LayerSettings.FoliageMask;
		backLeftWheelCollider.excludeLayers = GameManager.LayerSettings.FoliageMask;
		backRightWheelCollider.excludeLayers = GameManager.LayerSettings.FoliageMask;
		foreach (PlayerInfo remotePlayer in GameManager.RemotePlayers)
		{
			array = remotePlayer.GetComponentsInChildren<Collider>(includeInactive: true);
			foreach (Collider collider2 in array)
			{
				Collider[] array2 = componentsInChildren;
				for (int j = 0; j < array2.Length; j++)
				{
					Physics.IgnoreCollision(array2[j], collider2, ignore: true);
				}
			}
		}
		InformDriverChanged();
		GameManager.RemotePlayerRegistered += OnRemotePlayerRegistered;
		BUpdate.RegisterCallback(this);
	}

	private void Start()
	{
		AsEntity.Rigidbody.useGravity = false;
		PhysicsManager.RegisterPredictedEntity(AsEntity);
	}

	private void OnDestroy()
	{
		PhysicsManager.DeregisterPredictedEntity(AsEntity);
		BUpdate.DeregisterCallback(this);
		if (waterWadingSound.isValid())
		{
			waterWadingSound.stop(FMOD.Studio.STOP_MODE.ALLOWFADEOUT);
		}
		GameManager.RemotePlayerRegistered -= OnRemotePlayerRegistered;
	}

	private void OnCollisionEnter(Collision collision)
	{
		if (collision.contactCount <= 0 || BMath.GetTimeSince(collisionEffectsTimestamp) < 0.5f || collision.relativeVelocity.sqrMagnitude < 25f)
		{
			return;
		}
		Vector3 point = collision.GetContact(0).point;
		AudioHitObjectType audioHitObjectType;
		if (collision.rigidbody == null || !collision.rigidbody.TryGetComponent<Hittable>(out var component))
		{
			audioHitObjectType = AudioHitObjectType.Default;
		}
		else
		{
			if (component.AsEntity.IsPlayer)
			{
				return;
			}
			audioHitObjectType = (component.AsEntity.IsGolfCart ? AudioHitObjectType.GolfCart : AudioHitObjectType.Default);
		}
		VfxManager.PlayPooledVfxLocalOnly(VfxType.GolfCartCollision, point, Quaternion.identity);
		EventInstance eventInstance = RuntimeManager.CreateInstance(GameManager.AudioSettings.GolfCartCollisionEvent);
		eventInstance.set3DAttributes(point.To3DAttributes());
		eventInstance.setParameterByID(AudioSettings.ObjectId, (float)audioHitObjectType);
		eventInstance.start();
		eventInstance.release();
		collisionEffectsTimestamp = Time.timeAsDouble;
	}

	public void OnBUpdate()
	{
		GolfCartInfo.OnUpdate();
		bool hasDriver;
		bool hasSpeedBoost;
		float maxSpeedFactor;
		if (!AsEntity.IsDestroyed)
		{
			hasDriver = GolfCartInfo.TryGetDriver(out var driver);
			hasSpeedBoost = hasDriver && driver.Movement.StatusEffects.HasEffect(StatusEffect.SpeedBoost);
			maxSpeedFactor = (hasSpeedBoost ? settings.DriverSpeedBoostSpeedFactor : 1f);
			maxSpeedFactor *= MatchSetupRules.GetValue(MatchSetupRules.Rule.CartSpeed);
			bool flag = !IsAnyWheelGrounded();
			ApplyForwardInput(out var isMoving, out var forwardSpeed);
			ApplySteering();
			UpdateWheelDamping(hasDriver, forwardSpeed);
			UpdateWheelVisuals();
			UpdateSmoothedAcceleration();
			UpdateVfx(isMoving, forwardSpeed);
			UpdateEngineSound(flag, isMoving, forwardSpeed);
			if (shouldPlayJumpLandEffectsWhenGrounding && !flag)
			{
				VfxManager.PlayPooledVfxLocalOnly(VfxType.GolfCartJumpEnd, base.transform.position, base.transform.rotation);
				RuntimeManager.PlayOneShotAttached(GameManager.AudioSettings.GolfCartLandEvent, base.gameObject);
				shouldPlayJumpLandEffectsWhenGrounding = false;
			}
		}
		void ApplyForwardInput(out bool reference, out float reference2)
		{
			reference = AsEntity.Rigidbody.linearVelocity.sqrMagnitude > 1f;
			reference2 = (reference ? Vector3.Dot(AsEntity.Rigidbody.linearVelocity, base.transform.forward) : 0f);
			if (!hasDriver || forwardInput == 0)
			{
				DisableMotorsAndBrakes(hasDriver, reference2);
			}
			else
			{
				int num = BMath.Sign(reference2);
				float allWheelBrakeTorque;
				float allWheelMotorTorque;
				if (reference && num != forwardInput)
				{
					allWheelBrakeTorque = settings.ActiveBrakeTorque;
					allWheelMotorTorque = 0f;
				}
				else
				{
					allWheelBrakeTorque = 0f;
					allWheelMotorTorque = ((forwardInput <= 0) ? BMath.RemapClamped(settings.BackwardAccelerationAttenuationSpeedThreshold, settings.MaxBackwardSpeed * maxSpeedFactor, 0f - settings.MaxBackwardAccelerationTorque, 0f, 0f - reference2) : BMath.RemapClamped(settings.ForwardAccelerationAttenuationSpeedThreshold, settings.MaxForwardSpeed * maxSpeedFactor, settings.MaxForwardAccelerationTorque, 0f, reference2));
				}
				SetAllWheelBrakeTorque(allWheelBrakeTorque);
				SetAllWheelMotorTorque(allWheelMotorTorque);
			}
		}
		void ApplySteering()
		{
			float steerAngle = steering * settings.MaxSteeringAngle;
			frontLeftWheelCollider.steerAngle = steerAngle;
			frontRightWheelCollider.steerAngle = steerAngle;
			SmoothedSteering = BMath.LerpClamped(SmoothedSteering, steering, 4f * Time.deltaTime);
		}
		void CancelPipeVfxDelayedStopRoutine()
		{
			if (isDelayingPipeVfxStop)
			{
				isDelayingPipeVfxStop = false;
				if (pipeVfxDelayedStopRoutine != null)
				{
					StopCoroutine(pipeVfxDelayedStopRoutine);
				}
			}
		}
		void DisableMotorsAndBrakes(bool flag2, float value)
		{
			SetAllWheelMotorTorque(0f);
			if (flag2)
			{
				if (BMath.Abs(value) < settings.NoForwardInputBrakeSpeedThreshold)
				{
					SetAllWheelBrakeTorque(settings.NoForwardInputBrakeTorque);
				}
				else
				{
					SetAllWheelBrakeTorque(0f);
				}
			}
			else
			{
				float num = BMath.Abs(value);
				float allWheelBrakeTorque = BMath.RemapClamped(settings.DriverlessMinBrakeSpeed, settings.DriverlessMaxBrakeSpeed, settings.DriverlessMinBrakeTorque, settings.DriverlessMaxBrakeTorque, BMath.Abs(value));
				if (num <= 0.1f && BMath.Abs(base.transform.forward.GetPitchDeg()) > settings.SlopeRollPitchThreshold)
				{
					SetAllWheelBrakeTorque(0f);
				}
				else
				{
					SetAllWheelBrakeTorque(allWheelBrakeTorque);
				}
			}
		}
		float GetNormalizedSpeed()
		{
			return BMath.InverseLerpClamped(settings.WaterWadingSoundMinSpeed, settings.WaterWadingSoundMaxSpeed, P_0.forwardSpeed);
		}
		bool IsWading()
		{
			if (AsEntity.LevelBoundsTracker.CurrentSecondaryHazardLocalOnly == null)
			{
				if (MainOutOfBoundsHazard.Type != OutOfBoundsHazard.Water)
				{
					return false;
				}
			}
			else if (AsEntity.LevelBoundsTracker.CurrentSecondaryHazardLocalOnly.Type != OutOfBoundsHazard.Water)
			{
				return false;
			}
			Vector3 b = base.transform.InverseTransformVector(Vector3.down);
			Vector3 b2 = BMath.Sign(Vector3.Scale(settings.WaterWadingLocalBoundsSize, b));
			Vector3 position = settings.WaterWadingLocalBoundsCenter + Vector3.Scale(settings.WaterWadingLocalBoundsSize, b2) / 2f;
			Vector3 vector = base.transform.TransformPoint(position);
			float currentOutOfBoundsHazardWorldHeightLocalOnly = AsEntity.LevelBoundsTracker.CurrentOutOfBoundsHazardWorldHeightLocalOnly;
			if (vector.y > currentOutOfBoundsHazardWorldHeightLocalOnly)
			{
				return false;
			}
			return true;
		}
		bool ShouldPlay()
		{
			if (!isWadingInWater)
			{
				return false;
			}
			if (P_0.forwardSpeed < settings.WaterWadingSoundMinSpeed)
			{
				return false;
			}
			return true;
		}
		IEnumerator StopPipeVfxDelayedRoutine(float delay)
		{
			isDelayingPipeVfxStop = true;
			yield return new WaitForSeconds(delay);
			GolfCartInfo.Vfx.SetPipeVfxPlaying(playing: false);
			isDelayingPipeVfxStop = false;
		}
		void UpdateEngineSound(bool areAllWheelsInAir, bool flag2, float num2)
		{
			if (!hasDriver)
			{
				wheelRpmExponentialMovingAverage = 0f;
				GolfCartInfo.UpdateEngineSoundIntensity(0f, 2f);
			}
			else
			{
				float to = BMath.Average(BMath.Abs(frontLeftWheelCollider.rpm), BMath.Abs(frontRightWheelCollider.rpm), BMath.Abs(backLeftWheelCollider.rpm), BMath.Abs(backRightWheelCollider.rpm));
				wheelRpmExponentialMovingAverage = BMath.Lerp(wheelRpmExponentialMovingAverage, to, 2f / 21f);
				if (areAllWheelsInAir)
				{
					float targetIntensity = BMath.InverseLerpClamped(0f, 1500f, wheelRpmExponentialMovingAverage);
					GolfCartInfo.UpdateEngineSoundIntensity(targetIntensity, 4f);
				}
				else if (wheelRpmExponentialMovingAverage > 1500f)
				{
					GolfCartInfo.UpdateEngineSoundIntensity(0.8f, 4f);
				}
				else
				{
					int num = (flag2 ? BMath.Sign(num2) : 0);
					float targetIntensity2;
					if (forwardInput == 0)
					{
						if (flag2)
						{
							float t = ((num > 0) ? (num2 / settings.MaxForwardSpeed) : ((0f - num2) / settings.MaxBackwardSpeed));
							targetIntensity2 = BMath.LerpClamped(0f, 0.75f, t);
						}
						else
						{
							targetIntensity2 = 0f;
						}
					}
					else if (flag2)
					{
						if (num != forwardInput)
						{
							targetIntensity2 = 0.3f;
						}
						else
						{
							float t2 = ((num > 0) ? (num2 / (settings.MaxForwardSpeed * maxSpeedFactor)) : ((0f - num2) / (settings.MaxBackwardSpeed * maxSpeedFactor)));
							targetIntensity2 = BMath.LerpClamped(0.85f, hasSpeedBoost ? 0.75f : 0.6f, t2);
						}
					}
					else
					{
						targetIntensity2 = 0.85f;
					}
					GolfCartInfo.UpdateEngineSoundIntensity(targetIntensity2, 4f);
				}
			}
		}
		void UpdateSmoothedAcceleration()
		{
			SmoothedLocalAcceleration = Vector3.Lerp(SmoothedLocalAcceleration, rawLocalAcceleration, 12f * Time.deltaTime);
		}
		void UpdateVfx(bool flag2, float num)
		{
			UpdateWaterWading();
			UpdateWaterWadingSound();
			if (!hasDriver)
			{
				GolfCartInfo.Vfx.ResetLocalAcceleration();
				CancelPipeVfxDelayedStopRoutine();
				GolfCartInfo.Vfx.SetPipeVfxPlaying(playing: false);
			}
			else
			{
				GolfCartInfo.Vfx.SetLocalVelocity(base.transform.InverseTransformDirection(AsEntity.Rigidbody.linearVelocity));
				if (forwardInput > 0 && flag2 && num > 0f)
				{
					CancelPipeVfxDelayedStopRoutine();
					GolfCartInfo.Vfx.SetPipeVfxPlaying(playing: true);
				}
				else if (!isDelayingPipeVfxStop && GolfCartInfo.Vfx.ArePipeVfxPlaying)
				{
					float delay = BMath.RemapClamped(0f, settings.MaxForwardSpeed / 2f, 0.25f, 1f, num);
					pipeVfxDelayedStopRoutine = StartCoroutine(StopPipeVfxDelayedRoutine(delay));
				}
			}
		}
		void UpdateWaterWading()
		{
			bool flag2 = isWadingInWater;
			isWadingInWater = IsWading();
			if (isWadingInWater)
			{
				GolfCartInfo.Vfx.SetWadingWaterWorldHeight(AsEntity.LevelBoundsTracker.CurrentOutOfBoundsHazardWorldHeightLocalOnly);
			}
			if (isWadingInWater != flag2)
			{
				GolfCartInfo.Vfx.SetIsWadingInWater(isWadingInWater);
			}
		}
		void UpdateWaterWadingSound()
		{
			bool flag2 = isPlayingWaterWadingSound;
			isPlayingWaterWadingSound = ShouldPlay();
			if (isPlayingWaterWadingSound == flag2)
			{
				if (isPlayingWaterWadingSound)
				{
					waterWadingSound.setParameterByID(AudioSettings.SpeedId, GetNormalizedSpeed());
				}
			}
			else if (isPlayingWaterWadingSound)
			{
				waterWadingSound = RuntimeManager.CreateInstance(GameManager.AudioSettings.GolfCartWaterWadeEvent);
				RuntimeManager.AttachInstanceToGameObject(waterWadingSound, base.gameObject);
				waterWadingSound.setParameterByID(AudioSettings.SpeedId, GetNormalizedSpeed());
				waterWadingSound.start();
				waterWadingSound.release();
			}
			else if (waterWadingSound.isValid())
			{
				waterWadingSound.stop(FMOD.Studio.STOP_MODE.ALLOWFADEOUT);
			}
		}
		void UpdateWheelDamping(bool flag2, float num2)
		{
			float num = ((num2 > settings.MaxForwardSpeed * maxSpeedFactor) ? BMath.RemapClamped(settings.MaxForwardSpeed * maxSpeedFactor, settings.MaxSpeedExceededMaxWheelDampingForwardSpeedThreshold, settings.ActiveForwardInputWheelDamping, settings.MaxSpeedExceededMaxWheelDamping, num2) : ((!(num2 < (0f - settings.MaxBackwardSpeed) * maxSpeedFactor)) ? ((!flag2) ? settings.DriverlessWheelDamping : ((forwardInput == 0) ? settings.DefaultWheelDamping : settings.ActiveForwardInputWheelDamping)) : BMath.RemapClamped(settings.MaxBackwardSpeed * maxSpeedFactor, settings.MaxSpeedExceededMaxWheelDampingBackwardSpeedThreshold, settings.ActiveForwardInputWheelDamping, settings.MaxSpeedExceededMaxWheelDamping, 0f - num2)));
			float num3 = ((!flag2) ? settings.DriverlessUngroundedWheelDamping : ((forwardInput == 0) ? settings.UngroundedDefaultWheelDamping : settings.UngroundedActiveForwardInputWheelDamping));
			frontLeftWheelCollider.wheelDampingRate = (frontLeftWheelCollider.isGrounded ? num : num3);
			frontRightWheelCollider.wheelDampingRate = (frontRightWheelCollider.isGrounded ? num : num3);
			backLeftWheelCollider.wheelDampingRate = (backLeftWheelCollider.isGrounded ? num : num3);
			backRightWheelCollider.wheelDampingRate = (backRightWheelCollider.isGrounded ? num : num3);
		}
		void UpdateWheelVisuals()
		{
			frontLeftWheelCollider.GetWorldPose(out var pos, out var quat);
			frontLeftWheelTransform.SetPositionAndRotation(pos, quat);
			frontRightWheelCollider.GetWorldPose(out pos, out quat);
			frontRightWheelTransform.SetPositionAndRotation(pos, quat);
			backLeftWheelCollider.GetWorldPose(out pos, out quat);
			backLeftWheelTransform.SetPositionAndRotation(pos, quat);
			backRightWheelCollider.GetWorldPose(out pos, out quat);
			backRightWheelTransform.SetPositionAndRotation(pos, quat);
		}
	}

	public void OnFixedBUpdate()
	{
		ApplyElectromagnetShieldRepulsion();
		UpdateTrackedAcceleration();
		ApplyGravity();
		void ApplyElectromagnetShieldRepulsion()
		{
			if (GolfCartInfo.IsHidden)
			{
				return;
			}
			foreach (PlayerInfo activeShield in ElectromagnetShieldManager.ActiveShields)
			{
				if (GolfCartInfo.passengers.Contains(activeShield))
				{
					break;
				}
				Vector3 position = activeShield.ElectromagnetShieldCollider.transform.position;
				Vector3 vector = GolfCartInfo.ItemCollectorCollider.ClosestPoint(position);
				float sqrMagnitude = (vector - position).sqrMagnitude;
				if (sqrMagnitude >= activeShield.ElectromagnetShieldCollider.radius * activeShield.ElectromagnetShieldCollider.radius)
				{
					break;
				}
				if (sqrMagnitude == 0f)
				{
					vector = AsEntity.Rigidbody.centerOfMass;
				}
				float value = BMath.Sqrt(sqrMagnitude);
				Vector3 normalized = (vector - position).normalized;
				float num = BMath.RemapClamped(0f, activeShield.ElectromagnetShieldCollider.radius, GameManager.ItemSettings.ElectromagnetShieldMaxGolfCartRepulsionAcceleration, 0f, value);
				AsEntity.Rigidbody.AddForceAtPosition(num * normalized, vector, ForceMode.Acceleration);
			}
		}
		void ApplyGravity()
		{
			float num = 1f;
			if (isJumping)
			{
				float time = BMath.GetTimeSince(jumpTimestamp) / settings.JumpDuration;
				num = settings.JumpGravityFactorCurve.Evaluate(time);
			}
			AsEntity.Rigidbody.AddForce(Physics.gravity * num, ForceMode.Acceleration);
		}
		void UpdateTrackedAcceleration()
		{
			if (GolfCartInfo.GetPassengerCount() > 0)
			{
				Vector3 direction = (AsEntity.Rigidbody.linearVelocity - previousVelocity) / Time.fixedDeltaTime;
				Vector3 vector = base.transform.InverseTransformDirection(direction);
				localAccelerationRollingBuffer[accelerationBufferIndex] = vector;
				accelerationBufferIndex = BMath.Wrap(accelerationBufferIndex + 1, 30);
				Vector3 zero = Vector3.zero;
				float num = 0f;
				int num2 = accelerationBufferIndex;
				for (int i = 0; i < 30; i++)
				{
					float num3 = i;
					num += num3;
					zero += localAccelerationRollingBuffer[num2] * num3;
					num2 = BMath.Wrap(num2 + 1, 30);
				}
				zero /= num;
				rawLocalAcceleration = zero;
				previousVelocity = AsEntity.Rigidbody.linearVelocity;
			}
		}
	}

	public bool TryTriggerJump()
	{
		if (!CanJump())
		{
			return false;
		}
		OnJumpTriggered();
		CmdInformTriggeredJump();
		return true;
		bool CanJump()
		{
			if (isJumping)
			{
				return false;
			}
			if (!GolfCartInfo.TryGetDriver(out var driver))
			{
				return false;
			}
			if (driver != GameManager.LocalPlayerInfo)
			{
				return false;
			}
			if (GetGroundedWheelCount() < 2)
			{
				return false;
			}
			return true;
		}
		[Command]
		void CmdInformTriggeredJump(NetworkConnectionToClient sender = null)
		{
			if (base.isServer && base.isClient)
			{
				UserCode__003CTryTriggerJump_003Eg__CmdInformTriggeredJump_007C54_1__NetworkConnectionToClient(sender);
			}
			else
			{
				NetworkWriterPooled writer = NetworkWriterPool.Get();
				SendCommandInternal("System.Void GolfCartMovement::<TryTriggerJump>g__CmdInformTriggeredJump|54_1(Mirror.NetworkConnectionToClient)", -573712446, writer, 0);
				NetworkWriterPool.Return(writer);
			}
		}
		Vector3 GetJumpDirection()
		{
			Vector3 zero = Vector3.zero;
			int num = 0;
			if (frontLeftWheelCollider.isGrounded && frontLeftWheelCollider.GetGroundHit(out var hit))
			{
				zero += hit.normal;
				num++;
			}
			if (frontRightWheelCollider.isGrounded && frontRightWheelCollider.GetGroundHit(out hit))
			{
				zero += hit.normal;
				num++;
			}
			if (backLeftWheelCollider.isGrounded && backLeftWheelCollider.GetGroundHit(out hit))
			{
				zero += hit.normal;
				num++;
			}
			if (backRightWheelCollider.isGrounded && backRightWheelCollider.GetGroundHit(out hit))
			{
				zero += hit.normal;
				num++;
			}
			if (num <= 0)
			{
				return base.transform.up;
			}
			return zero / num;
		}
		IEnumerator JumpRoutine()
		{
			isJumping = true;
			jumpTimestamp = Time.timeAsDouble;
			Vector3 vector = GetJumpDirection();
			float num = Vector3.Dot(AsEntity.Rigidbody.linearVelocity, vector);
			if (num < 0f)
			{
				AsEntity.Rigidbody.linearVelocity -= num * vector;
			}
			AsEntity.Rigidbody.linearVelocity += settings.JumpSpeed * vector;
			if (steering != 0f)
			{
				float value = Vector3.Dot(AsEntity.Rigidbody.linearVelocity, base.transform.forward);
				float num2 = ((forwardInput > 0) ? settings.MaxForwardSpeed : settings.MaxBackwardSpeed);
				float num3 = BMath.Clamp01(BMath.Abs(value) / num2);
				float num4 = 1f - num3;
				AsEntity.Rigidbody.angularVelocity += steering * num4 * settings.JumpMaxYawSpeed * base.transform.up;
			}
			if (VfxPersistentData.TryGetPooledVfx(VfxType.GolfCartJumpStart, out var particleSystem))
			{
				particleSystem.transform.SetParent(base.transform);
				particleSystem.transform.SetLocalPositionAndRotation(Vector3.zero, Quaternion.identity);
				particleSystem.Play();
			}
			RuntimeManager.PlayOneShotAttached(GameManager.AudioSettings.GolfCartJumpEvent, base.gameObject);
			float num5 = 0f;
			bool setUpLandingVfx = false;
			while (num5 < settings.JumpDuration)
			{
				yield return null;
				num5 = BMath.GetTimeSince(jumpTimestamp);
				if (!setUpLandingVfx && !IsAnyWheelGrounded())
				{
					shouldPlayJumpLandEffectsWhenGrounding = true;
					setUpLandingVfx = true;
				}
			}
			isJumping = false;
		}
		void OnJumpTriggered()
		{
			if (jumpRoutine != null)
			{
				StopCoroutine(jumpRoutine);
			}
			jumpRoutine = StartCoroutine(JumpRoutine());
		}
	}

	public bool IsAnyWheelGrounded()
	{
		if (!frontLeftWheelCollider.isGrounded && !frontRightWheelCollider.isGrounded && !backLeftWheelCollider.isGrounded)
		{
			return backRightWheelCollider.isGrounded;
		}
		return true;
	}

	public int GetGroundedWheelCount()
	{
		int num = 0;
		if (frontLeftWheelCollider.isGrounded)
		{
			num++;
		}
		if (frontRightWheelCollider.isGrounded)
		{
			num++;
		}
		if (backLeftWheelCollider.isGrounded)
		{
			num++;
		}
		if (backRightWheelCollider.isGrounded)
		{
			num++;
		}
		return num;
	}

	public void InformDriverChanged()
	{
		NetworkisAccelerating = false;
		NetworkisBraking = false;
		forwardInput = 0;
		SetAllWheelBrakeTorque(0f);
		SetAllWheelMotorTorque(0f);
		SmoothedLocalAcceleration = Vector3.zero;
		previousVelocity = Vector3.zero;
		for (int i = 0; i < 30; i++)
		{
			localAccelerationRollingBuffer[i] = Vector3.zero;
		}
	}

	public void InformPassengersChanged()
	{
		if (GolfCartInfo.GetPassengerCount() <= 0)
		{
			for (int i = 0; i < 30; i++)
			{
				localAccelerationRollingBuffer[i] = Vector3.zero;
			}
		}
	}

	public void SetIsAccelerating(bool isAccelerating)
	{
		if (!GolfCartInfo.TryGetDriver(out var driver) || driver != GameManager.LocalPlayerInfo)
		{
			Debug.LogError("Local player tried to accelerate a golf cart without being its driver", base.gameObject);
		}
		else
		{
			NetworkisAccelerating = isAccelerating;
		}
	}

	public void SetIsBraking(bool isBraking)
	{
		if (!GolfCartInfo.TryGetDriver(out var driver) || driver != GameManager.LocalPlayerInfo)
		{
			Debug.LogError("Local player tried to brake a golf cart without being its driver", base.gameObject);
		}
		else
		{
			NetworkisBraking = isBraking;
		}
	}

	public void SetSteering(float steering)
	{
		if (!GolfCartInfo.TryGetDriver(out var driver) || driver != GameManager.LocalPlayerInfo)
		{
			Debug.LogError("Local player tried to steer a golf cart without being its driver", base.gameObject);
		}
		else
		{
			Networksteering = BMath.LerpClamped(this.steering, steering, settings.SteeringInputSensitivity * Time.deltaTime);
		}
	}

	private void UpdateForwardInput()
	{
		if (isAccelerating == isBraking)
		{
			forwardInput = 0;
		}
		else if (isAccelerating)
		{
			forwardInput = 1;
		}
		else
		{
			forwardInput = -1;
		}
	}

	private void SetAllWheelBrakeTorque(float brakeTorque)
	{
		frontLeftWheelCollider.brakeTorque = brakeTorque;
		frontRightWheelCollider.brakeTorque = brakeTorque;
		backLeftWheelCollider.brakeTorque = brakeTorque;
		backRightWheelCollider.brakeTorque = brakeTorque;
	}

	private void SetAllWheelMotorTorque(float motorTorque)
	{
		frontLeftWheelCollider.motorTorque = motorTorque;
		frontRightWheelCollider.motorTorque = motorTorque;
		backLeftWheelCollider.motorTorque = motorTorque;
		backRightWheelCollider.motorTorque = motorTorque;
	}

	private void OnRemotePlayerRegistered(PlayerInfo playerInfo)
	{
		Collider[] componentsInChildren = GetComponentsInChildren<Collider>(includeInactive: true);
		Collider[] componentsInChildren2 = playerInfo.GetComponentsInChildren<Collider>(includeInactive: true);
		foreach (Collider collider in componentsInChildren2)
		{
			Collider[] array = componentsInChildren;
			for (int j = 0; j < array.Length; j++)
			{
				Physics.IgnoreCollision(array[j], collider, ignore: true);
			}
		}
	}

	private void OnIsAcceleratingChanged(bool wasAccelerating, bool isAccelerating)
	{
		UpdateForwardInput();
	}

	private void OnIsBrakingChanged(bool wasAccelerating, bool isAccelerating)
	{
		UpdateForwardInput();
	}

	public GolfCartMovement()
	{
		_Mirror_SyncVarHookDelegate_isAccelerating = OnIsAcceleratingChanged;
		_Mirror_SyncVarHookDelegate_isBraking = OnIsBrakingChanged;
	}

	public override bool Weaved()
	{
		return true;
	}

	protected void UserCode__003CTryTriggerJump_003Eg__CmdInformTriggeredJump_007C54_1__NetworkConnectionToClient(NetworkConnectionToClient sender)
	{
		if (!serverInformTriggeredJumpCommandRateLimiter.RegisterHit(sender))
		{
			return;
		}
		if (sender != null && sender != NetworkServer.localConnection)
		{
			OnJumpTriggered();
		}
		foreach (NetworkConnectionToClient value2 in NetworkServer.connections.Values)
		{
			if (value2 != NetworkServer.localConnection && value2 != sender)
			{
				RpcInformTriggeredJump(value2);
			}
		}
		Vector3 GetJumpDirection()
		{
			Vector3 zero = Vector3.zero;
			int num = 0;
			if (frontLeftWheelCollider.isGrounded && frontLeftWheelCollider.GetGroundHit(out var hit))
			{
				zero += hit.normal;
				num++;
			}
			if (frontRightWheelCollider.isGrounded && frontRightWheelCollider.GetGroundHit(out hit))
			{
				zero += hit.normal;
				num++;
			}
			if (backLeftWheelCollider.isGrounded && backLeftWheelCollider.GetGroundHit(out hit))
			{
				zero += hit.normal;
				num++;
			}
			if (backRightWheelCollider.isGrounded && backRightWheelCollider.GetGroundHit(out hit))
			{
				zero += hit.normal;
				num++;
			}
			if (num <= 0)
			{
				return base.transform.up;
			}
			return zero / num;
		}
		IEnumerator JumpRoutine()
		{
			isJumping = true;
			jumpTimestamp = Time.timeAsDouble;
			Vector3 vector = GetJumpDirection();
			float num = Vector3.Dot(AsEntity.Rigidbody.linearVelocity, vector);
			if (num < 0f)
			{
				AsEntity.Rigidbody.linearVelocity -= num * vector;
			}
			AsEntity.Rigidbody.linearVelocity += settings.JumpSpeed * vector;
			if (steering != 0f)
			{
				float value = Vector3.Dot(AsEntity.Rigidbody.linearVelocity, base.transform.forward);
				float num2 = ((forwardInput > 0) ? settings.MaxForwardSpeed : settings.MaxBackwardSpeed);
				float num3 = BMath.Clamp01(BMath.Abs(value) / num2);
				float num4 = 1f - num3;
				AsEntity.Rigidbody.angularVelocity += steering * num4 * settings.JumpMaxYawSpeed * base.transform.up;
			}
			if (VfxPersistentData.TryGetPooledVfx(VfxType.GolfCartJumpStart, out var particleSystem))
			{
				particleSystem.transform.SetParent(base.transform);
				particleSystem.transform.SetLocalPositionAndRotation(Vector3.zero, Quaternion.identity);
				particleSystem.Play();
			}
			RuntimeManager.PlayOneShotAttached(GameManager.AudioSettings.GolfCartJumpEvent, base.gameObject);
			float num5 = 0f;
			bool setUpLandingVfx = false;
			while (num5 < settings.JumpDuration)
			{
				yield return null;
				num5 = BMath.GetTimeSince(jumpTimestamp);
				if (!setUpLandingVfx && !IsAnyWheelGrounded())
				{
					shouldPlayJumpLandEffectsWhenGrounding = true;
					setUpLandingVfx = true;
				}
			}
			isJumping = false;
		}
		void OnJumpTriggered()
		{
			if (jumpRoutine != null)
			{
				StopCoroutine(jumpRoutine);
			}
			jumpRoutine = StartCoroutine(JumpRoutine());
		}
		[TargetRpc]
		void RpcInformTriggeredJump(NetworkConnectionToClient connection)
		{
			NetworkWriterPooled writer = NetworkWriterPool.Get();
			SendTargetRPCInternal(connection, "System.Void GolfCartMovement::<TryTriggerJump>g__RpcInformTriggeredJump|54_2(Mirror.NetworkConnectionToClient)", -324300968, writer, 0);
			NetworkWriterPool.Return(writer);
		}
	}

	protected static void InvokeUserCode__003CTryTriggerJump_003Eg__CmdInformTriggeredJump_007C54_1__NetworkConnectionToClient(NetworkBehaviour obj, NetworkReader reader, NetworkConnectionToClient senderConnection)
	{
		if (!NetworkServer.active)
		{
			Debug.LogError("Command <TryTriggerJump>g__CmdInformTriggeredJump|54_1 called on client.");
		}
		else
		{
			((GolfCartMovement)obj).UserCode__003CTryTriggerJump_003Eg__CmdInformTriggeredJump_007C54_1__NetworkConnectionToClient(senderConnection);
		}
	}

	protected void UserCode__003CTryTriggerJump_003Eg__RpcInformTriggeredJump_007C54_2__NetworkConnectionToClient(NetworkConnectionToClient connection)
	{
		OnJumpTriggered();
		Vector3 GetJumpDirection()
		{
			Vector3 zero = Vector3.zero;
			int num = 0;
			if (frontLeftWheelCollider.isGrounded && frontLeftWheelCollider.GetGroundHit(out var hit))
			{
				zero += hit.normal;
				num++;
			}
			if (frontRightWheelCollider.isGrounded && frontRightWheelCollider.GetGroundHit(out hit))
			{
				zero += hit.normal;
				num++;
			}
			if (backLeftWheelCollider.isGrounded && backLeftWheelCollider.GetGroundHit(out hit))
			{
				zero += hit.normal;
				num++;
			}
			if (backRightWheelCollider.isGrounded && backRightWheelCollider.GetGroundHit(out hit))
			{
				zero += hit.normal;
				num++;
			}
			if (num <= 0)
			{
				return base.transform.up;
			}
			return zero / num;
		}
		IEnumerator JumpRoutine()
		{
			isJumping = true;
			jumpTimestamp = Time.timeAsDouble;
			Vector3 vector = GetJumpDirection();
			float num = Vector3.Dot(AsEntity.Rigidbody.linearVelocity, vector);
			if (num < 0f)
			{
				AsEntity.Rigidbody.linearVelocity -= num * vector;
			}
			AsEntity.Rigidbody.linearVelocity += settings.JumpSpeed * vector;
			if (steering != 0f)
			{
				float value = Vector3.Dot(AsEntity.Rigidbody.linearVelocity, base.transform.forward);
				float num2 = ((forwardInput > 0) ? settings.MaxForwardSpeed : settings.MaxBackwardSpeed);
				float num3 = BMath.Clamp01(BMath.Abs(value) / num2);
				float num4 = 1f - num3;
				AsEntity.Rigidbody.angularVelocity += steering * num4 * settings.JumpMaxYawSpeed * base.transform.up;
			}
			if (VfxPersistentData.TryGetPooledVfx(VfxType.GolfCartJumpStart, out var particleSystem))
			{
				particleSystem.transform.SetParent(base.transform);
				particleSystem.transform.SetLocalPositionAndRotation(Vector3.zero, Quaternion.identity);
				particleSystem.Play();
			}
			RuntimeManager.PlayOneShotAttached(GameManager.AudioSettings.GolfCartJumpEvent, base.gameObject);
			float num5 = 0f;
			bool setUpLandingVfx = false;
			while (num5 < settings.JumpDuration)
			{
				yield return null;
				num5 = BMath.GetTimeSince(jumpTimestamp);
				if (!setUpLandingVfx && !IsAnyWheelGrounded())
				{
					shouldPlayJumpLandEffectsWhenGrounding = true;
					setUpLandingVfx = true;
				}
			}
			isJumping = false;
		}
		void OnJumpTriggered()
		{
			if (jumpRoutine != null)
			{
				StopCoroutine(jumpRoutine);
			}
			jumpRoutine = StartCoroutine(JumpRoutine());
		}
	}

	protected static void InvokeUserCode__003CTryTriggerJump_003Eg__RpcInformTriggeredJump_007C54_2__NetworkConnectionToClient(NetworkBehaviour obj, NetworkReader reader, NetworkConnectionToClient senderConnection)
	{
		if (!NetworkClient.active)
		{
			Debug.LogError("TargetRPC <TryTriggerJump>g__RpcInformTriggeredJump|54_2 called on server.");
		}
		else
		{
			((GolfCartMovement)obj).UserCode__003CTryTriggerJump_003Eg__RpcInformTriggeredJump_007C54_2__NetworkConnectionToClient(null);
		}
	}

	static GolfCartMovement()
	{
		RemoteProcedureCalls.RegisterCommand(typeof(GolfCartMovement), "System.Void GolfCartMovement::<TryTriggerJump>g__CmdInformTriggeredJump|54_1(Mirror.NetworkConnectionToClient)", InvokeUserCode__003CTryTriggerJump_003Eg__CmdInformTriggeredJump_007C54_1__NetworkConnectionToClient, requiresAuthority: true);
		RemoteProcedureCalls.RegisterRpc(typeof(GolfCartMovement), "System.Void GolfCartMovement::<TryTriggerJump>g__RpcInformTriggeredJump|54_2(Mirror.NetworkConnectionToClient)", InvokeUserCode__003CTryTriggerJump_003Eg__RpcInformTriggeredJump_007C54_2__NetworkConnectionToClient);
	}

	public override void SerializeSyncVars(NetworkWriter writer, bool forceAll)
	{
		base.SerializeSyncVars(writer, forceAll);
		if (forceAll)
		{
			writer.WriteBool(isAccelerating);
			writer.WriteBool(isBraking);
			writer.WriteFloat(steering);
			return;
		}
		writer.WriteVarULong(syncVarDirtyBits);
		if ((syncVarDirtyBits & 1L) != 0L)
		{
			writer.WriteBool(isAccelerating);
		}
		if ((syncVarDirtyBits & 2L) != 0L)
		{
			writer.WriteBool(isBraking);
		}
		if ((syncVarDirtyBits & 4L) != 0L)
		{
			writer.WriteFloat(steering);
		}
	}

	public override void DeserializeSyncVars(NetworkReader reader, bool initialState)
	{
		base.DeserializeSyncVars(reader, initialState);
		if (initialState)
		{
			GeneratedSyncVarDeserialize(ref isAccelerating, _Mirror_SyncVarHookDelegate_isAccelerating, reader.ReadBool());
			GeneratedSyncVarDeserialize(ref isBraking, _Mirror_SyncVarHookDelegate_isBraking, reader.ReadBool());
			GeneratedSyncVarDeserialize(ref steering, null, reader.ReadFloat());
			return;
		}
		long num = (long)reader.ReadVarULong();
		if ((num & 1L) != 0L)
		{
			GeneratedSyncVarDeserialize(ref isAccelerating, _Mirror_SyncVarHookDelegate_isAccelerating, reader.ReadBool());
		}
		if ((num & 2L) != 0L)
		{
			GeneratedSyncVarDeserialize(ref isBraking, _Mirror_SyncVarHookDelegate_isBraking, reader.ReadBool());
		}
		if ((num & 4L) != 0L)
		{
			GeneratedSyncVarDeserialize(ref steering, null, reader.ReadFloat());
		}
	}
}
