using System;
using System.Collections;
using System.Runtime.InteropServices;
using Mirror;
using UnityEngine;

public class PlayerAnimatorIo : NetworkBehaviour
{
	[SerializeField]
	private Animator animator;

	[SerializeField]
	private NetworkAnimator networkAnimator;

	private PlayerInfo playerInfo;

	private RuntimeAnimatorController defaultAnimatorController;

	private static readonly int rawInputMagnitudeHash = Animator.StringToHash("Input magnitude");

	private static readonly int localSmoothedInputXHash = Animator.StringToHash("Smooth input right");

	private static readonly int localSmoothedInputYHash = Animator.StringToHash("Smooth input forward");

	private static readonly int horizontalSpeedHash = Animator.StringToHash("Horizontal speed");

	private static readonly int smoothedHorizontalSpeedHash = Animator.StringToHash("Smoothed horizontal speed");

	private static readonly int movementLocalYawHash = Animator.StringToHash("Movement local yaw");

	private static readonly int leadingFootIntHash = Animator.StringToHash("Leading foot int");

	private static readonly int leadingFootFloatHash = Animator.StringToHash("Leading foot float");

	private static readonly int lastGroundedLeadingFootIntHash = Animator.StringToHash("Last grounded leading foot int");

	private static readonly int lastGroundedLeadingFootFloatHash = Animator.StringToHash("Last grounded leading foot float");

	private static readonly int isGroundedHash = Animator.StringToHash("Is grounded");

	private static readonly int landedHash = Animator.StringToHash("Landed");

	private static readonly int stepHash = Animator.StringToHash("Step");

	private static readonly int strafeStrengthHash = Animator.StringToHash("Strafe strength");

	private static readonly int wadingInWaterFloatHash = Animator.StringToHash("Wading in water float");

	private static readonly int waterWadingSpeedHash = Animator.StringToHash("Water wading speed");

	private static readonly int jumpHash = Animator.StringToHash("Jump");

	private static readonly int jumpTypeHash = Animator.StringToHash("Jump type");

	private static readonly int diveHash = Animator.StringToHash("Dive");

	private static readonly int diveTypeHash = Animator.StringToHash("Dive type");

	private static readonly int divingStateHash = Animator.StringToHash("Diving state");

	private static readonly int equippedItemHash = Animator.StringToHash("Equipped item");

	private static readonly int isAimingItemHash = Animator.StringToHash("Is aiming item");

	private static readonly int itemAimPitchHash = Animator.StringToHash("Item aim pitch");

	private static readonly int useItemHash = Animator.StringToHash("Use item");

	private static readonly int itemUseTypeHash = Animator.StringToHash("Item use type");

	private static readonly int flourishItemHash = Animator.StringToHash("Flourish item");

	private static readonly int isFlourishingItemHash = Animator.StringToHash("Is flourishing item");

	private static readonly int isAimingSwingHash = Animator.StringToHash("Is aiming swing");

	private static readonly int isChargingSwingHash = Animator.StringToHash("Is charging swing");

	private static readonly int swingChargeHash = Animator.StringToHash("Swing charge");

	private static readonly int isInPuttingModeHash = Animator.StringToHash("Is in putting mode");

	private static readonly int startSwingHash = Animator.StringToHash("Start swing");

	private static readonly int isSwingingHash = Animator.StringToHash("Is swinging");

	private static readonly int enterGolfCartHash = Animator.StringToHash("Enter golf cart");

	private static readonly int isInGolfCartHash = Animator.StringToHash("Is in golf cart");

	private static readonly int golfCartSeatHash = Animator.StringToHash("Golf cart seat");

	private static readonly int golfCartSteeringHash = Animator.StringToHash("Golf cart steering");

	private static readonly int golfCartForwardAccelerationHash = Animator.StringToHash("Golf cart forward acceleration");

	private static readonly int golfCartRightAccelerationHash = Animator.StringToHash("Golf cart right acceleration");

	private static readonly int knockOutHash = Animator.StringToHash("Knock out");

	private static readonly int knockoutStateHash = Animator.StringToHash("Knockout state");

	private static readonly int isKnockoutLegSweepHash = Animator.StringToHash("Is knockout leg sweep");

	private static readonly int isKnockedOutHash = Animator.StringToHash("Is knocked out");

	private static readonly int knockoutGroundednessHash = Animator.StringToHash("Knockout groundedness");

	private static readonly int knockoutUpLocalNormalizedYawHash = Animator.StringToHash("Knockout roll normalized yaw");

	private static readonly int knockoutHitFlinchHash = Animator.StringToHash("Knockout hit flinch");

	private static readonly int knockoutReoveryTypeHash = Animator.StringToHash("Knockout recovery type");

	private static readonly int freezeHash = Animator.StringToHash("Freeze");

	private static readonly int isFrozenHash = Animator.StringToHash("Is frozen");

	public static readonly int startDrowningHash = Animator.StringToHash("Start drowning");

	public static readonly int isDrowningHash = Animator.StringToHash("Is drowning");

	public static readonly int victoryDanceHash = Animator.StringToHash("Victory dance");

	public static readonly int lossAnimationHash = Animator.StringToHash("Loss animation");

	public static readonly int emoteHash = Animator.StringToHash("Emote");

	private static readonly int reevaluateUpperBodyHash = Animator.StringToHash("Reevaluate upper body");

	[SerializeField]
	[DisableField]
	private float leftFootLeading;

	[SerializeField]
	[DisableField]
	private float rightFootLeading;

	[SerializeField]
	[DisableField]
	private float leftStep;

	[SerializeField]
	[DisableField]
	private float rightStep;

	[SerializeField]
	[DisableField]
	private float spineStraighteningWeight;

	private int upperBodyLayerIndex;

	private Coroutine upperBodyLayerBlendRoutine;

	private Coroutine knockoutGroundednessTransitionRoutine;

	private Coroutine knockoutHitFlinchRoutine;

	private Coroutine wadingInWaterFloatRoutine;

	private Foot currentLeadingFoot;

	private float previousLeftFootLeadingValue;

	private float previousRightFootLeadingValue;

	private float previousLeftStepValue;

	private float previousRightStepValue;

	private float smoothedHorizontalSpeed;

	[SyncVar]
	private float aimingYawOffset;

	public bool ShouldGolfCartBriefcaseBeSplit { get; private set; }

	public float AimingYawOffset => aimingYawOffset;

	public float SpineStraighteningWeight => spineStraighteningWeight;

	public float NetworkaimingYawOffset
	{
		get
		{
			return aimingYawOffset;
		}
		[param: In]
		set
		{
			GeneratedSyncVarSetter(value, ref aimingYawOffset, 1uL, null);
		}
	}

	public event Action<Foot> Footstep;

	public event Action<Foot> FootLifted;

	[CCommand("setVictoryDance", "Enter the dance's name or index. 0 or \"None\" disables dancing", false, false)]
	public static void SetVictoryDanceFromConsole(VictoryDance dance)
	{
		if (!(GameManager.LocalPlayerInfo == null))
		{
			GameManager.LocalPlayerInfo.AnimatorIo.SetVictoryDance(dance);
		}
	}

	protected override void OnValidate()
	{
		base.OnValidate();
		networkAnimator.parametersToSkipNameHashes = new int[4] { leadingFootIntHash, leadingFootFloatHash, golfCartForwardAccelerationHash, golfCartRightAccelerationHash };
	}

	private void Awake()
	{
		playerInfo = GetComponent<PlayerInfo>();
		defaultAnimatorController = animator.runtimeAnimatorController;
		upperBodyLayerIndex = animator.GetLayerIndex("Upper body");
		syncDirection = SyncDirection.ClientToServer;
	}

	public override void OnStartLocalPlayer()
	{
		UpdateUpperBodyLayerEnabled();
	}

	private void Update()
	{
		ProcessFootstepValues();
		UpdateAimingAngle(instant: false);
		void ProcessFootstepValues()
		{
			if (previousLeftFootLeadingValue <= 0.5f && leftFootLeading > 0.5f)
			{
				currentLeadingFoot = Foot.Left;
			}
			else if (previousRightFootLeadingValue <= 0.5f && rightFootLeading > 0.5f)
			{
				currentLeadingFoot = Foot.Right;
			}
			if (previousLeftStepValue <= 0.5f && leftStep > 0.5f)
			{
				this.Footstep?.Invoke(Foot.Left);
				OnStep(Foot.Left);
			}
			else if (previousRightStepValue <= 0.5f && rightStep > 0.5f)
			{
				this.Footstep?.Invoke(Foot.Right);
				OnStep(Foot.Right);
			}
			if (previousLeftStepValue > 0.5f && leftStep <= 0.5f)
			{
				this.FootLifted?.Invoke(Foot.Left);
			}
			else if (previousRightStepValue > 0.5f && rightStep <= 0.5f)
			{
				this.FootLifted?.Invoke(Foot.Right);
			}
			previousLeftFootLeadingValue = leftFootLeading;
			previousRightFootLeadingValue = rightFootLeading;
			previousLeftStepValue = leftStep;
			previousRightStepValue = rightStep;
			if (base.isLocalPlayer)
			{
				animator.SetInteger(leadingFootIntHash, (int)currentLeadingFoot);
				animator.SetFloat(leadingFootFloatHash, (float)currentLeadingFoot);
			}
		}
	}

	private void OnAnimatorMove()
	{
		ResetSingleFrameTriggers();
		void ResetSingleFrameTriggers()
		{
			animator.ResetTrigger(landedHash);
			animator.ResetTrigger(jumpHash);
			animator.ResetTrigger(diveHash);
			animator.ResetTrigger(useItemHash);
			animator.ResetTrigger(flourishItemHash);
			animator.ResetTrigger(startSwingHash);
			animator.ResetTrigger(stepHash);
			animator.ResetTrigger(startDrowningHash);
		}
	}

	public void UpdateAimingAngleInstantly()
	{
		UpdateAimingAngle(instant: true);
	}

	private void UpdateAimingAngle(bool instant)
	{
		if (base.isLocalPlayer && (playerInfo.Inventory.IsAimingItem || playerInfo.Inventory.IsUsingItemAtAll))
		{
			float maxDistance;
			int layerMask;
			switch (playerInfo.NetworkedEquippedItem)
			{
			default:
				return;
			case ItemType.DuelingPistol:
				maxDistance = GameManager.ItemSettings.DuelingPistolMaxAimingDistance;
				layerMask = GameManager.LayerSettings.GunHittablesMask;
				break;
			case ItemType.ElephantGun:
				maxDistance = GameManager.ItemSettings.ElephantGunMaxAimingDistance;
				layerMask = GameManager.LayerSettings.GunHittablesMask;
				break;
			case ItemType.RocketLauncher:
				maxDistance = GameManager.ItemSettings.RocketLauncherMaxAimingDistance;
				layerMask = GameManager.LayerSettings.RocketHittablesMask;
				break;
			case ItemType.FreezeBomb:
			{
				playerInfo.Inventory.GetFreezeBombAimDirection(out var pitch);
				SmoothAndSetItemAimPitch(pitch);
				return;
			}
			}
			Vector3 position = playerInfo.NeckBone.position;
			float localYaw;
			Vector3 vector = playerInfo.Inventory.GetFirearmAimPoint(maxDistance, layerMask, out localYaw) - position;
			float pitchDeg = vector.GetPitchDeg();
			if (instant)
			{
				SetItemAimPitch(pitchDeg);
			}
			else
			{
				SmoothAndSetItemAimPitch(vector.GetPitchDeg());
			}
			NetworkaimingYawOffset = (vector.GetYawDeg() - base.transform.forward.GetYawDeg()).WrapAngleDeg();
			if (playerInfo.NetworkedEquippedItem == ItemType.ElephantGun && playerInfo.Inventory.IsUsingItemAtAll)
			{
				NetworkaimingYawOffset = BMath.Clamp(aimingYawOffset, 0f - GameManager.ItemSettings.ElephantGunShotDiveMaxAimYaw, GameManager.ItemSettings.ElephantGunShotDiveMaxAimYaw);
			}
		}
		void SmoothAndSetItemAimPitch(float targetPitch)
		{
			float itemAimPitch = BMath.LerpClamped(animator.GetFloat(itemAimPitchHash), targetPitch, 16f * Time.deltaTime);
			SetItemAimPitch(itemAimPitch);
		}
	}

	public void SetMovementInput(float magnitude, Vector3 localSmoothed)
	{
		animator.SetFloat(rawInputMagnitudeHash, magnitude);
		animator.SetFloat(localSmoothedInputXHash, localSmoothed.x);
		animator.SetFloat(localSmoothedInputYHash, localSmoothed.z);
	}

	public void SetLocalVelocity(Vector3 localVelocity, float deltaTime)
	{
		float magnitude = localVelocity.AsHorizontal2().magnitude;
		float num = ((magnitude > smoothedHorizontalSpeed) ? 8f : 16f);
		smoothedHorizontalSpeed = BMath.LerpClamped(smoothedHorizontalSpeed, magnitude, num * deltaTime);
		animator.SetFloat(horizontalSpeedHash, magnitude);
		animator.SetFloat(smoothedHorizontalSpeedHash, smoothedHorizontalSpeed);
		animator.SetFloat(movementLocalYawHash, localVelocity.GetYawDeg());
	}

	public void SetAnimationSpeedLocalOnly(float speed)
	{
		animator.speed = speed;
	}

	public void SetIsGrounded(bool grounded)
	{
		animator.SetBool(isGroundedHash, grounded);
		if (!grounded)
		{
			animator.SetInteger(lastGroundedLeadingFootIntHash, (int)currentLeadingFoot);
			animator.SetFloat(lastGroundedLeadingFootFloatHash, (float)currentLeadingFoot);
		}
	}

	public void BeginJump(JumpType jumpType)
	{
		animator.SetInteger(jumpTypeHash, (int)jumpType);
		networkAnimator.SetTrigger(jumpHash);
		ResetLanded();
	}

	public void SetLanded()
	{
		networkAnimator.SetTrigger(landedHash);
	}

	private void ResetLanded()
	{
		networkAnimator.ResetTrigger(landedHash);
	}

	public void Dive(DiveType type)
	{
		networkAnimator.SetTrigger(diveHash);
		animator.SetInteger(diveTypeHash, (int)type);
	}

	public void SetDivingState(DivingState state)
	{
		animator.SetInteger(divingStateHash, (int)state);
	}

	public void SetIsAimingSwing(bool isAiming)
	{
		animator.SetBool(isAimingSwingHash, isAiming);
	}

	public void SetIsChargingSwing(bool isCharging)
	{
		animator.SetBool(isChargingSwingHash, isCharging);
		UpdateUpperBodyLayerEnabled();
		if (isCharging)
		{
			TriggerReevaluateUpperBody();
		}
	}

	public void SetSwingCharge(float charge)
	{
		animator.SetFloat(swingChargeHash, charge);
	}

	public void SetIsInPuttingMode(bool isInPuttingMode)
	{
		animator.SetBool(isInPuttingModeHash, isInPuttingMode);
	}

	public void SetIsSwinging(bool isSwinging)
	{
		if (isSwinging)
		{
			networkAnimator.SetTrigger(startSwingHash);
		}
		animator.SetBool(isSwingingHash, isSwinging);
	}

	public void EnterGolfCart(int seat)
	{
		networkAnimator.SetTrigger(enterGolfCartHash);
		animator.SetBool(isInGolfCartHash, value: true);
		animator.SetInteger(golfCartSeatHash, seat);
	}

	public void ExitGolfCart()
	{
		networkAnimator.ResetTrigger(enterGolfCartHash);
		animator.SetBool(isInGolfCartHash, value: false);
	}

	public void SetGolfCartSteering(float steering)
	{
		animator.SetFloat(golfCartSteeringHash, steering);
	}

	public void SetGolfCartAcceleration(Vector3 localAcceleration)
	{
		animator.SetFloat(golfCartForwardAccelerationHash, localAcceleration.z / 10f);
		animator.SetFloat(golfCartRightAccelerationHash, localAcceleration.x / 15f);
	}

	public void KnockOut(bool isLegSweep)
	{
		networkAnimator.SetTrigger(knockOutHash);
		animator.SetBool(isKnockedOutHash, value: true);
		animator.SetBool(isKnockoutLegSweepHash, isLegSweep);
		if (!isLegSweep)
		{
			TriggerKnockdownHitFlinch();
		}
	}

	public void SetKnockoutState(KnockoutState state)
	{
		animator.SetInteger(knockoutStateHash, (int)state);
	}

	public void TransitionKnockoutGroundednessTo(float groundednessTarget)
	{
		if (animator.GetFloat(knockoutGroundednessHash) != groundednessTarget)
		{
			if (knockoutGroundednessTransitionRoutine != null)
			{
				StopCoroutine(knockoutGroundednessTransitionRoutine);
			}
			knockoutGroundednessTransitionRoutine = StartCoroutine(TransitionFloatRoutine(knockoutGroundednessHash, groundednessTarget, 0.1f));
		}
	}

	public void TriggerKnockdownHitFlinch()
	{
		if (knockoutHitFlinchRoutine != null)
		{
			StopCoroutine(knockoutHitFlinchRoutine);
		}
		knockoutHitFlinchRoutine = StartCoroutine(AnimateKnockounHitFlinch());
	}

	public void SetKnockoutWorldUpNormalizedLocalYaw(float normalizedYaw)
	{
		animator.SetFloat(knockoutUpLocalNormalizedYawHash, normalizedYaw);
	}

	public void SetKnockoutRecoveryType(KnockoutRecoveryType type)
	{
		animator.SetInteger(knockoutReoveryTypeHash, (int)type);
	}

	public void RecoverFromKnockOut()
	{
		networkAnimator.ResetTrigger(knockOutHash);
		animator.SetBool(isKnockedOutHash, value: false);
	}

	public void SetIsFrozen(bool isFrozen)
	{
		if (isFrozen)
		{
			networkAnimator.SetTrigger(freezeHash);
		}
		animator.SetBool(isFrozenHash, isFrozen);
	}

	public void SetIsDrowning(bool isDrowning)
	{
		if (isDrowning)
		{
			networkAnimator.SetTrigger(startDrowningHash);
		}
		animator.SetBool(isDrowningHash, isDrowning);
	}

	public void SetVictoryDance(VictoryDance dance)
	{
		animator.SetInteger(victoryDanceHash, (int)dance);
	}

	public void SetLost()
	{
		int value = UnityEngine.Random.Range(1, 4);
		animator.SetInteger(lossAnimationHash, value);
	}

	public void SetEmote(Emote emote)
	{
		animator.SetInteger(emoteHash, (int)emote);
		UpdateUpperBodyLayerEnabled();
	}

	public void SetEquippedItem(ItemType equippedItem)
	{
		animator.SetInteger(equippedItemHash, (int)equippedItem);
		UpdateUpperBodyLayerEnabled();
		if (equippedItem != ItemType.None)
		{
			TriggerReevaluateUpperBody();
		}
	}

	public void OnNetworkedEquippedItemChanged(ItemType equippedItem)
	{
		ItemData itemData;
		if (equippedItem == ItemType.None)
		{
			animator.runtimeAnimatorController = defaultAnimatorController;
		}
		else if (!GameManager.AllItems.TryGetItemData(equippedItem, out itemData))
		{
			Debug.LogError($"Could not find data for item {equippedItem}");
			animator.runtimeAnimatorController = defaultAnimatorController;
		}
		else
		{
			RuntimeAnimatorController animatorOverrideController = itemData.AnimatorOverrideController;
			animator.runtimeAnimatorController = ((animatorOverrideController != null) ? animatorOverrideController : defaultAnimatorController);
		}
	}

	public void SetIsAimingItem(bool isAimingItem)
	{
		animator.SetBool(isAimingItemHash, isAimingItem);
	}

	private void SetItemAimPitch(float aimingPitch)
	{
		animator.SetFloat(itemAimPitchHash, aimingPitch);
	}

	public void SetItemUseType(ItemUseType itemUse)
	{
		animator.SetInteger(itemUseTypeHash, (int)itemUse);
		if (itemUse != ItemUseType.None)
		{
			networkAnimator.SetTrigger(useItemHash);
		}
	}

	public void SetIsFlourishingItem(bool isFlourishing)
	{
		animator.SetBool(isFlourishingItemHash, isFlourishing);
		if (isFlourishing)
		{
			networkAnimator.SetTrigger(flourishItemHash);
		}
	}

	public void SetStrafeStrength(float strafeStrength)
	{
		animator.SetFloat(strafeStrengthHash, strafeStrength);
	}

	private void OnStep(Foot foot)
	{
		playerInfo.PlayerAudio.PlayFootstepLocalOnly(foot);
		animator.SetTrigger(stepHash);
	}

	public void SetIsWadingInWater(bool isWading)
	{
		if (wadingInWaterFloatRoutine != null)
		{
			StopCoroutine(wadingInWaterFloatRoutine);
		}
		wadingInWaterFloatRoutine = StartCoroutine(TransitionFloatRoutine(wadingInWaterFloatHash, isWading ? 1f : 0f, 0.1f));
	}

	public void SetHasSpeedBoost(bool hasSpeedBoost)
	{
		animator.SetFloat(waterWadingSpeedHash, hasSpeedBoost ? 1f : 0f);
	}

	private void TriggerReevaluateUpperBody()
	{
		networkAnimator.SetTrigger(reevaluateUpperBodyHash);
	}

	private IEnumerator TransitionFloatRoutine(int parameterHash, float targetValue, float fullBlendDuration)
	{
		float initialValue = animator.GetFloat(parameterHash);
		if (initialValue != targetValue)
		{
			float time = 0f;
			float duration = BMath.Abs(targetValue - initialValue) * fullBlendDuration;
			while (time < duration)
			{
				yield return null;
				time += Time.deltaTime;
				animator.SetFloat(parameterHash, BMath.Lerp(initialValue, targetValue, time / duration));
			}
			animator.SetFloat(parameterHash, targetValue);
		}
	}

	private void UpdateUpperBodyLayerEnabled()
	{
		bool num = ShouldUpperBodyLayerBeEnabled();
		if (upperBodyLayerBlendRoutine != null)
		{
			StopCoroutine(upperBodyLayerBlendRoutine);
		}
		float targetValue = (num ? 1f : 0f);
		upperBodyLayerBlendRoutine = StartCoroutine(BlendLayerRoutine(upperBodyLayerIndex, targetValue, 0.1f));
		bool ShouldUpperBodyLayerBeEnabled()
		{
			if (playerInfo.AsGolfer.IsChargingSwing)
			{
				return true;
			}
			ItemType integer = (ItemType)animator.GetInteger(equippedItemHash);
			if (integer != ItemType.None && integer != ItemType.SpringBoots && integer != ItemType.RocketDriver)
			{
				if (playerInfo.IsPlayingEmote)
				{
					return false;
				}
				return true;
			}
			return false;
		}
	}

	private IEnumerator BlendLayerRoutine(int layer, float targetValue, float fullBlendDuration)
	{
		float initialValue = animator.GetLayerWeight(layer);
		if (initialValue != targetValue)
		{
			float time = 0f;
			float duration = BMath.Abs(targetValue - initialValue) * fullBlendDuration;
			while (time < duration)
			{
				yield return null;
				time += Time.deltaTime;
				animator.SetLayerWeight(layer, BMath.Lerp(initialValue, targetValue, time / duration));
			}
			animator.SetLayerWeight(layer, targetValue);
		}
	}

	private IEnumerator AnimateKnockounHitFlinch()
	{
		animator.SetFloat(knockoutHitFlinchHash, 1f);
		float time = 0f;
		float duration = 0.6f;
		while (time < duration)
		{
			yield return null;
			time += Time.deltaTime;
			float num = BMath.EaseIn(time / duration);
			animator.SetFloat(knockoutHitFlinchHash, 1f - num);
		}
		animator.SetFloat(knockoutHitFlinchHash, 0f);
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
			writer.WriteFloat(aimingYawOffset);
			return;
		}
		writer.WriteVarULong(syncVarDirtyBits);
		if ((syncVarDirtyBits & 1L) != 0L)
		{
			writer.WriteFloat(aimingYawOffset);
		}
	}

	public override void DeserializeSyncVars(NetworkReader reader, bool initialState)
	{
		base.DeserializeSyncVars(reader, initialState);
		if (initialState)
		{
			GeneratedSyncVarDeserialize(ref aimingYawOffset, null, reader.ReadFloat());
			return;
		}
		long num = (long)reader.ReadVarULong();
		if ((num & 1L) != 0L)
		{
			GeneratedSyncVarDeserialize(ref aimingYawOffset, null, reader.ReadFloat());
		}
	}
}
