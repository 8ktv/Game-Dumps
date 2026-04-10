using UnityEngine;

[CreateAssetMenu(fileName = "Item settings", menuName = "Settings/Gameplay/Items")]
public class ItemSettings : ScriptableObject
{
	[field: Header("Coffee")]
	[field: SerializeField]
	public float CoffeeDrinkEffectStartFrames { get; private set; }

	[field: SerializeField]
	public float CoffeeTotalFrames { get; private set; }

	[field: SerializeField]
	public float CoffeeEffectDuration { get; private set; }

	[field: SerializeField]
	public float CoffeeThrowPlateFrame { get; private set; }

	[field: SerializeField]
	public float CoffeeThrowMugFrame { get; private set; }

	[field: SerializeField]
	public float CoffeeThrowPlateSpeed { get; private set; }

	[field: SerializeField]
	public Vector3 CoffeeThrowPlateDirectionLocalRotationEuler { get; private set; }

	[field: SerializeField]
	public Vector3 CoffeeThrowPlateLocalAngularVelocity { get; private set; }

	[field: SerializeField]
	public float CoffeeThrowMugSpeed { get; private set; }

	[field: SerializeField]
	public Vector3 CoffeeThrowMugDirectionLocalRotationEuler { get; private set; }

	[field: SerializeField]
	public Vector3 CoffeeThrowMugLocalAngularVelocity { get; private set; }

	[field: Header("Dueling pistol")]
	[field: SerializeField]
	public float DuelingPistolShotFrames { get; private set; }

	[field: SerializeField]
	public Vector3 DuelingPistolLocalBarrelEnd { get; private set; }

	[field: SerializeField]
	public float DuelingPistolMaxAimingDistance { get; private set; }

	[field: SerializeField]
	public float DuelingPistolMaxShotDistance { get; private set; }

	[field: SerializeField]
	public float DuelingPistolMaxInaccuracyAngle { get; private set; }

	[field: SerializeField]
	public float DuelingPistolThrowFrame { get; private set; }

	[field: SerializeField]
	public float DuelingPistolThrowSpeed { get; private set; }

	[field: SerializeField]
	public Vector3 DuelingPistolThrowDirectionLocalRotationEuler { get; private set; }

	[field: SerializeField]
	public Vector3 DuelingPistolThrowLocalAngularVelocity { get; private set; }

	[field: Header("Elephant gun")]
	[field: SerializeField]
	public float ElephantGunShotFrames { get; private set; }

	[field: SerializeField]
	public Vector3 ElephantGunLocalBarrelEnd { get; private set; }

	[field: SerializeField]
	public float ElephantGunMaxAimingDistance { get; private set; }

	[field: SerializeField]
	public float ElephantGunMaxShotDistance { get; private set; }

	[field: SerializeField]
	public float ElephantGunShotCooldown { get; private set; }

	[field: SerializeField]
	public float ElephantGunMaxInaccuracyAngle { get; private set; }

	[field: SerializeField]
	public float ElephantGunShotDiveSpeed { get; private set; }

	[field: SerializeField]
	public float ElephantGunShotDiveMinUpwardsSpeed { get; private set; }

	[field: SerializeField]
	public float ElephantGunShotDiveMaxAimYaw { get; private set; }

	[field: SerializeField]
	public float ElephantGunGetUpThrowFrame { get; private set; }

	[field: SerializeField]
	public float ElephantGunThrowSpeed { get; private set; }

	[field: SerializeField]
	public Vector3 ElephantGunThrowDirectionLocalRotationEuler { get; private set; }

	[field: SerializeField]
	public Vector3 ElephantGunThrowLocalAngularVelocity { get; private set; }

	[field: Header("Rocket launcher")]
	[field: SerializeField]
	public float RocketLauncherLockOnMaxDistance { get; private set; }

	[field: SerializeField]
	public float RocketLauncherLockOnMaxYawFromCenterScreen { get; private set; }

	[field: SerializeField]
	public float RocketLauncherLockOnYawWeight { get; private set; }

	[field: SerializeField]
	public float RocketLauncherShotFrames { get; private set; }

	[field: SerializeField]
	public Vector3 RocketLauncherLocalRocketPosition { get; private set; }

	[field: SerializeField]
	public Vector3 RocketLauncherLocalRocketRotationEuler { get; private set; }

	[field: SerializeField]
	public Vector3 RocketLauncherLocalBarrelFrontEnd { get; private set; }

	[field: SerializeField]
	public Vector3 RocketLauncherLocalBarrelBackEnd { get; private set; }

	[field: SerializeField]
	public float RocketLauncherBackBlastDistance { get; private set; }

	[field: SerializeField]
	public float RocketLauncherBackBlastWidth { get; private set; }

	[field: SerializeField]
	public float RocketLauncherThrowFrame { get; private set; }

	[field: SerializeField]
	public float RocketLauncherThrowSpeed { get; private set; }

	[field: SerializeField]
	public Vector3 RocketLauncherThrowDirectionLocalRotationEuler { get; private set; }

	[field: SerializeField]
	public Vector3 RocketLauncherThrowLocalAngularVelocity { get; private set; }

	[field: SerializeField]
	public Rocket RocketPrefab { get; private set; }

	[field: SerializeField]
	public float RocketLauncherMaxAimingDistance { get; private set; }

	[field: SerializeField]
	public float RocketVelocity { get; private set; }

	[field: SerializeField]
	public float RocketPostLaunchFullHomingTime { get; private set; }

	[field: SerializeField]
	public float RocketMaxVelocityRotationPerSecond { get; private set; }

	[field: SerializeField]
	public float RocketExplosionRange { get; private set; }

	[field: SerializeField]
	public float RocketMaxTravelDistance { get; private set; }

	[field: Header("Land mine")]
	[field: SerializeField]
	public Vector3 LandmineLocalCenter { get; private set; }

	[field: SerializeField]
	public Vector3 LandmineLocalCenterOfMass { get; private set; }

	[field: SerializeField]
	public float LandminePlantingOffsetIntoGround { get; private set; }

	[field: SerializeField]
	public float LandminePlantingStickIntoGroundFrames { get; private set; }

	[field: SerializeField]
	public float LandminePlantingFrames { get; private set; }

	[field: SerializeField]
	public float LandminePlantingTotalFrames { get; private set; }

	[field: SerializeField]
	public float LandminePlantingArmDelay { get; private set; }

	[field: SerializeField]
	public Vector3 LandmineTossingDirectionLocalRotationEuler { get; private set; }

	[field: SerializeField]
	public float LandmineTossingSpeed { get; private set; }

	[field: SerializeField]
	public Vector3 LandmineTossingLocalAngularVelocity { get; private set; }

	[field: SerializeField]
	public float LandmineTossingFrames { get; private set; }

	[field: SerializeField]
	public float LandmineTossingTotalFrames { get; private set; }

	[field: SerializeField]
	public float LandmineTossingMinArmDelay { get; private set; }

	[field: SerializeField]
	public float LandmineTossingMaxArmDelay { get; private set; }

	[field: SerializeField]
	public float LandmineTossingMaxArmSpeed { get; private set; }

	[field: SerializeField]
	public float LandmineHitArmDelay { get; private set; }

	[field: SerializeField]
	public float LandmineCollisionRange { get; private set; }

	[field: SerializeField]
	public float LandmineDetectionRange { get; private set; }

	[field: SerializeField]
	public float LandmineDetectionMinSpeed { get; private set; }

	[field: SerializeField]
	public float LandmineExplosionRange { get; private set; }

	[field: SerializeField]
	public float LandmineArmedBlinkPeriod { get; private set; }

	[field: SerializeField]
	public float LandminePlantingCollisionCheckRange { get; private set; }

	[field: Header("Airhorn")]
	[field: SerializeField]
	public float AirhornUseFrames { get; private set; }

	[field: SerializeField]
	public float AirhornRange { get; private set; }

	[field: SerializeField]
	public float AirhornThrowFrame { get; private set; }

	[field: SerializeField]
	public float AirhornThrowSpeed { get; private set; }

	[field: SerializeField]
	public Vector3 AirhornThrowDirectionLocalRotationEuler { get; private set; }

	[field: SerializeField]
	public Vector3 AirhornThrowLocalAngularVelocity { get; private set; }

	[field: Header("Spring boots")]
	[field: SerializeField]
	public float SpringBootsUseDuration { get; private set; }

	[field: SerializeField]
	public float SpringBootsJumpHorizontalSpeed { get; private set; }

	[field: SerializeField]
	public float SpringBootsJumpUpwardsSpeed { get; private set; }

	[field: SerializeField]
	public float SpringBootsJumpMovementSpeed { get; private set; }

	[field: SerializeField]
	public float SpringBootsJumpPeakLingerStartSpeed { get; private set; }

	[field: SerializeField]
	public float SpringBootsJumpPeakLingerEndSpeed { get; private set; }

	[field: SerializeField]
	public float SpringBootsJumpPeakLingerGravityFactor { get; private set; }

	[field: SerializeField]
	public float SpringBootLeftPopOffSpeed { get; private set; }

	[field: SerializeField]
	public float SpringBootRightPopOffSpeed { get; private set; }

	[field: SerializeField]
	public Vector3 SpringBootLeftPopOffDirectionLocalRotationEuler { get; private set; }

	[field: SerializeField]
	public Vector3 SpringBootRightPopOffDirectionLocalRotationEuler { get; private set; }

	[field: SerializeField]
	public Vector3 SpringBootLeftPopOffLocalAngularVelocity { get; private set; }

	[field: SerializeField]
	public Vector3 SpringBootRightPopOffLocalAngularVelocity { get; private set; }

	[field: Header("Electromagnet")]
	[field: SerializeField]
	public float ElectromagnetShieldDuration { get; private set; }

	[field: SerializeField]
	public float ElectromagnetShieldMaxPlayerGroundedRepulsionAcceleration { get; private set; }

	[field: SerializeField]
	public float ElectromagnetShieldMaxPlayerKnockedOutRepulsionAcceleration { get; private set; }

	[field: SerializeField]
	public float ElectromagnetShieldMaxPlayerDefaultRepulsionAcceleration { get; private set; }

	[field: SerializeField]
	public float ElectromagnetShieldMaxGolfCartRepulsionAcceleration { get; private set; }

	[field: SerializeField]
	public float ElectromagnetShieldProjectileReflectTiltUpMaxAngle { get; private set; }

	[field: SerializeField]
	public float ElectromagnetShieldProjectileNonHomingReflectSpeedFactor { get; private set; }

	[field: SerializeField]
	public float ElectromagnetShieldRocketReflectTiltUpMaxAngle { get; private set; }

	[field: SerializeField]
	public float ElectromagnetActivationTotalFrames { get; private set; }

	[field: SerializeField]
	public float ElectromagnetThrowFrame { get; private set; }

	[field: SerializeField]
	public float ElectromagnetThrowSpeed { get; private set; }

	[field: SerializeField]
	public Vector3 ElectromagnetThrowDirectionLocalRotationEuler { get; private set; }

	[field: SerializeField]
	public Vector3 ElectromagnetThrowLocalAngularVelocity { get; private set; }

	[field: Header("Orbital laser")]
	[field: SerializeField]
	public float OrbitalLaserAnticipationDuration { get; private set; }

	[field: SerializeField]
	public float OrbitalLaserAnticipationPositionSmoothing { get; private set; }

	[field: SerializeField]
	public float OrbitalLaserAnticipationStopFollowingDuration { get; private set; }

	[field: SerializeField]
	public float OrbitalLaserExplosionDuration { get; private set; }

	[field: SerializeField]
	public float OrbitalLaserEndingDuration { get; private set; }

	[field: SerializeField]
	public float OrbitalLaserExplosionMaxRange { get; private set; }

	[field: SerializeField]
	public float OrbitalLaserExplosionCenterRadius { get; private set; }

	[field: SerializeField]
	public float OrbitalLaserExplosionLaserHeight { get; private set; }

	[field: SerializeField]
	public float OrbitalLaserExplosionLaserRadius { get; private set; }

	[field: SerializeField]
	public float OrbitalLaserActivationFrame { get; private set; }

	[field: SerializeField]
	public float OrbitalLaserActivationTotalFrames { get; private set; }

	[field: SerializeField]
	public float OrbitalLaserThrowFrame { get; private set; }

	[field: SerializeField]
	public float OrbitalLaserThrowSpeed { get; private set; }

	[field: SerializeField]
	public Vector3 OrbitalLaserThrowDirectionLocalRotationEuler { get; private set; }

	[field: SerializeField]
	public Vector3 OrbitalLaserThrowLocalAngularVelocity { get; private set; }

	[field: Header("Golf cart")]
	[field: SerializeField]
	public float GolfCartSeparatePartsFrames { get; private set; }

	[field: SerializeField]
	public float GolfCartBriefcaseOpenStartFrame { get; private set; }

	[field: SerializeField]
	public float GolfBriefcaseOpenEndFrame { get; private set; }

	[field: SerializeField]
	public float GolfCartPlacementTotalFrames { get; private set; }

	[field: SerializeField]
	public float GolfCartThrowBaseSpeed { get; private set; }

	[field: SerializeField]
	public Vector3 GolfCartThrowBaseDirectionLocalRotationEuler { get; private set; }

	[field: SerializeField]
	public Vector3 GolfCartThrowBaseLocalAngularVelocity { get; private set; }

	[field: SerializeField]
	public float GolfCartThrowLidSpeed { get; private set; }

	[field: SerializeField]
	public Vector3 GolfCartThrowLidDirectionLocalRotationEuler { get; private set; }

	[field: SerializeField]
	public Vector3 GolfCartThrowLidLocalAngularVelocity { get; private set; }

	[field: Header("Rocket driver")]
	[field: SerializeField]
	public float RocketDriverBaseNormalizedSwingPower { get; private set; }

	[field: SerializeField]
	public float RocketDriverFullNormalizedSwingPower { get; private set; }

	[field: SerializeField]
	public float RocketDriverBaseSwingMissDiveSpeed { get; private set; }

	[field: SerializeField]
	public float RocketDriverFullSwingMissDiveSpeed { get; private set; }

	[field: SerializeField]
	public float RocketDriverSwingMissMinUpwardsSpeed { get; private set; }

	[field: SerializeField]
	public float RocketDriverSwingPostHitSpinStartFrame { get; private set; }

	[field: SerializeField]
	public float RocketDriverSwingPostHitSpinEndFrame { get; private set; }

	[field: SerializeField]
	public Vector3 RocketDriverSwingPostHitSpinLocalOrigin { get; private set; }

	[field: SerializeField]
	public float RocketDriverSwingPostHitSpinRadius { get; private set; }

	[field: SerializeField]
	public float RocketDriverMaxLockOnDistance { get; private set; }

	[field: SerializeField]
	public float RocketDriverSwingHitThrowFrame { get; private set; }

	[field: SerializeField]
	public float RocketDriverSwingHitThrowSpeed { get; private set; }

	[field: SerializeField]
	public float RocketDriverSwingMissThrowSpeed { get; private set; }

	[field: SerializeField]
	public Vector3 RocketDriverSwingHitThrowDirectionLocalRotationEuler { get; private set; }

	[field: SerializeField]
	public Vector3 RocketDriverSwingMissThrowDirectionLocalRotationEuler { get; private set; }

	[field: SerializeField]
	public Vector3 RocketDriverSwingHitThrowLocalAngularVelocity { get; private set; }

	[field: SerializeField]
	public Vector3 RocketDriverSwingMissThrowLocalAngularVelocity { get; private set; }

	[field: Header("Freeze bomb")]
	[field: SerializeField]
	public float FreezeBombAimPitchOffset { get; private set; }

	[field: SerializeField]
	public float FreezeBombNoOffsetAimPitch { get; private set; }

	[field: SerializeField]
	public float FreezeBombFullOffsetAimPitch { get; private set; }

	[field: SerializeField]
	public float FreezeBombAimMaxPitch { get; private set; }

	[field: SerializeField]
	public float FreezeBombAimMinPitch { get; private set; }

	[field: SerializeField]
	public float FreezeBombShotFrames { get; private set; }

	[field: SerializeField]
	public float FreezeBombMaxTravelDistance { get; private set; }

	[field: SerializeField]
	public float FreezeBombExplosionRange { get; private set; }

	[field: SerializeField]
	public Vector3 FreezeBombLocalBombPosition { get; private set; }

	[field: SerializeField]
	public float FreezeBombShotSpeed { get; private set; }

	[field: SerializeField]
	public float FreezeBombShotAngularSpeed { get; private set; }

	[field: SerializeField]
	public float FreezeBombShotRotationAxisMaxRotation { get; private set; }

	[field: SerializeField]
	public FreezeBomb FreezeBombBombPrefab { get; private set; }

	[field: SerializeField]
	public float FreezeBombFreezeDuration { get; private set; }

	[field: SerializeField]
	public float FreezeBombThrowFrame { get; private set; }

	[field: SerializeField]
	public float FreezeBombThrowSpeed { get; private set; }

	[field: SerializeField]
	public Vector3 FreezeBombThrowDirectionLocalRotationEuler { get; private set; }

	[field: SerializeField]
	public Vector3 FreezeBombThrowLocalAngularVelocity { get; private set; }

	[field: SerializeField]
	public float FreezeBombPlatformShakeStartTime { get; private set; }

	[field: SerializeField]
	public float FreezeBombPlatformDuration { get; private set; }

	[field: SerializeField]
	public float FreezeBombPlatformMaxHeightOffset { get; private set; }

	[field: SerializeField]
	public float FreezeBombMaxExplosionHeightAboveWaterToCreatePlatform { get; private set; }

	[field: SerializeField]
	public FreezeBombPlatform FreezeBombPlatformPrefab { get; private set; }

	[field: SerializeField]
	public float FreezeBombFlourishVfxStartFrame { get; private set; }

	public float CoffeeDrinkEffectStartTime { get; private set; }

	public float CoffeePostTotalDuration { get; private set; }

	public float CoffeeThrowPlateTime { get; private set; }

	public float CoffeeThrowMugTime { get; private set; }

	public Quaternion CoffeeThrowPlateDirectionLocalRotation { get; private set; }

	public Quaternion CoffeeThrowMugDirectionLocalRotation { get; private set; }

	public float DuelingPistolShotDuration { get; private set; }

	public float DuelingPistolThrowTime { get; private set; }

	public Quaternion DuelingPistolThrowDirectionLocalRotation { get; private set; }

	public float ElephantGunShotDuration { get; private set; }

	public float ElephantGunGetUpThrowTime { get; private set; }

	public Quaternion ElephantGunThrowDirectionLocalRotation { get; private set; }

	public float RocketLauncherShotDuration { get; private set; }

	public float RocketLauncherLockOnMaxDistanceSquared { get; private set; }

	public Quaternion RocketLauncherLocalRocketRotation { get; private set; }

	public Vector3 RocketLauncherBackBlastSize { get; private set; }

	public float RocketLauncherThrowTime { get; private set; }

	public Quaternion RocketLauncherThrowDirectionLocalRotation { get; private set; }

	public float LandmineCollisionRangeSquared { get; private set; }

	public float LandminePlantingStickIntoGroundTime { get; private set; }

	public float LandminePlantingTime { get; private set; }

	public float LandminePostPlantingTime { get; private set; }

	public Quaternion LandmineTossingDirectionLocalRotation { get; private set; }

	public float LandmineTossingTime { get; private set; }

	public float LandminePostTossingTime { get; private set; }

	public float LandmineTossingMaxArmSpeedSquared { get; private set; }

	public float LandmineDetectionMinSpeedSquared { get; private set; }

	public float AirhornUseDuration { get; private set; }

	public float AirhornThrowTime { get; private set; }

	public Quaternion AirhornThrowDirectionLocalRotation { get; private set; }

	public Quaternion SpringBootLeftPopOffDirectionLocalRotation { get; private set; }

	public Quaternion SpringBootRightPopOffDirectionLocalRotation { get; private set; }

	public float ElectromagnetActivationTotalDuration { get; private set; }

	public float ElectromagnetThrowTime { get; private set; }

	public Quaternion ElectromagnetThrowDirectionLocalRotation { get; private set; }

	public float OrbitalLaserExplosionEndTime { get; private set; }

	public float OrbitalLaserEndingEndTime { get; private set; }

	public float OrbitalLaserActivationTime { get; private set; }

	public float OrbitalLaserActivationStopFollowTime { get; private set; }

	public float OrbitalLaserActivationTotalDuration { get; private set; }

	public float OrbitalLaserThrowTime { get; private set; }

	public float OrbitalLaserExplosionMaxRangeSquared { get; private set; }

	public Quaternion OrbitalLaserThrowDirectionLocalRotation { get; private set; }

	public float GolfCartSeparatePartsDuration { get; private set; }

	public float GolfCartBriefcaseOpenStartTime { get; private set; }

	public float GolfBriefcaseOpenEndTime { get; private set; }

	public float GolfCartPlacementTotalDuration { get; private set; }

	public Quaternion GolfCartThrowBaseDirectionLocalRotation { get; private set; }

	public Quaternion GolfCartThrowLidDirectionLocalRotation { get; private set; }

	public float RocketDriverSwingPostHitSpinStartTime { get; private set; }

	public float RocketDriverSwingPostHitSpinEndTime { get; private set; }

	public float RocketDriverMaxLockOnDistanceSquared { get; private set; }

	public float RocketDriverSwingHitThrowTime { get; private set; }

	public Quaternion RocketDriverSwingHitThrowDirectionLocalRotation { get; private set; }

	public Quaternion RocketDriverSwingMissThrowDirectionLocalRotation { get; private set; }

	public float FreezeBombShotDuration { get; private set; }

	public float FreezeBombThrowTime { get; private set; }

	public Quaternion FreezeBombThrowDirectionLocalRotation { get; private set; }

	public float FreezeBombFlourishVfxStartTime { get; private set; }

	private void OnValidate()
	{
		Initialize();
	}

	private void OnEnable()
	{
		Initialize();
	}

	private void Initialize()
	{
		float num = 30f;
		float num2 = 1f / num;
		CoffeeDrinkEffectStartTime = CoffeeDrinkEffectStartFrames * num2;
		CoffeePostTotalDuration = CoffeeTotalFrames * num2;
		CoffeeThrowPlateTime = CoffeeThrowPlateFrame * num2;
		CoffeeThrowMugTime = CoffeeThrowMugFrame * num2;
		CoffeeThrowPlateDirectionLocalRotation = Quaternion.Euler(CoffeeThrowPlateDirectionLocalRotationEuler);
		CoffeeThrowMugDirectionLocalRotation = Quaternion.Euler(CoffeeThrowMugDirectionLocalRotationEuler);
		DuelingPistolShotDuration = DuelingPistolShotFrames * num2;
		DuelingPistolThrowTime = DuelingPistolThrowFrame * num2;
		DuelingPistolThrowDirectionLocalRotation = Quaternion.Euler(DuelingPistolThrowDirectionLocalRotationEuler);
		ElephantGunShotDuration = ElephantGunShotFrames * num2;
		ElephantGunGetUpThrowTime = ElephantGunGetUpThrowFrame * num2;
		ElephantGunThrowDirectionLocalRotation = Quaternion.Euler(ElephantGunThrowDirectionLocalRotationEuler);
		RocketLauncherShotDuration = RocketLauncherShotFrames * num2;
		RocketLauncherLockOnMaxDistanceSquared = RocketLauncherLockOnMaxDistance * RocketLauncherLockOnMaxDistance;
		RocketLauncherLocalRocketRotation = Quaternion.Euler(RocketLauncherLocalRocketRotationEuler);
		RocketLauncherBackBlastSize = new Vector3(RocketLauncherBackBlastWidth, RocketLauncherBackBlastWidth, RocketLauncherBackBlastDistance);
		RocketLauncherThrowTime = RocketLauncherThrowFrame * num2;
		RocketLauncherThrowDirectionLocalRotation = Quaternion.Euler(RocketLauncherThrowDirectionLocalRotationEuler);
		LandmineCollisionRangeSquared = LandmineCollisionRange * LandmineCollisionRange;
		LandminePlantingStickIntoGroundTime = LandminePlantingStickIntoGroundFrames * num2;
		LandminePlantingTime = LandminePlantingFrames * num2;
		LandminePostPlantingTime = LandminePlantingTotalFrames * num2 - LandminePlantingTime;
		LandmineTossingDirectionLocalRotation = Quaternion.Euler(LandmineTossingDirectionLocalRotationEuler);
		LandmineTossingTime = LandmineTossingFrames * num2;
		LandminePostTossingTime = LandmineTossingTotalFrames * num2 - LandmineTossingTime;
		LandmineTossingMaxArmSpeedSquared = LandmineTossingMaxArmSpeed * LandmineTossingMaxArmSpeed;
		LandmineDetectionMinSpeedSquared = LandmineDetectionMinSpeed * LandmineDetectionMinSpeed;
		AirhornUseDuration = AirhornUseFrames * num2;
		AirhornThrowTime = AirhornThrowFrame * num2;
		AirhornThrowDirectionLocalRotation = Quaternion.Euler(AirhornThrowDirectionLocalRotationEuler);
		SpringBootLeftPopOffDirectionLocalRotation = Quaternion.Euler(SpringBootLeftPopOffDirectionLocalRotationEuler);
		SpringBootRightPopOffDirectionLocalRotation = Quaternion.Euler(SpringBootRightPopOffDirectionLocalRotationEuler);
		ElectromagnetActivationTotalDuration = ElectromagnetActivationTotalFrames * num2;
		ElectromagnetThrowTime = ElectromagnetThrowFrame * num2;
		ElectromagnetThrowDirectionLocalRotation = Quaternion.Euler(ElectromagnetThrowDirectionLocalRotationEuler);
		OrbitalLaserExplosionEndTime = OrbitalLaserAnticipationDuration + OrbitalLaserExplosionDuration;
		OrbitalLaserEndingEndTime = OrbitalLaserExplosionEndTime + OrbitalLaserEndingDuration;
		OrbitalLaserActivationTime = OrbitalLaserActivationFrame * num2;
		OrbitalLaserActivationStopFollowTime = OrbitalLaserAnticipationDuration - OrbitalLaserAnticipationStopFollowingDuration;
		OrbitalLaserActivationTotalDuration = OrbitalLaserActivationTotalFrames * num2;
		OrbitalLaserThrowTime = OrbitalLaserThrowFrame * num2;
		OrbitalLaserExplosionMaxRangeSquared = OrbitalLaserExplosionMaxRange * OrbitalLaserExplosionMaxRange;
		OrbitalLaserThrowDirectionLocalRotation = Quaternion.Euler(OrbitalLaserThrowDirectionLocalRotationEuler);
		GolfCartSeparatePartsDuration = GolfCartSeparatePartsFrames * num2;
		GolfCartBriefcaseOpenStartTime = GolfCartBriefcaseOpenStartFrame * num2;
		GolfBriefcaseOpenEndTime = GolfBriefcaseOpenEndFrame * num2;
		GolfCartPlacementTotalDuration = GolfCartPlacementTotalFrames * num2;
		GolfCartThrowBaseDirectionLocalRotation = Quaternion.Euler(GolfCartThrowBaseDirectionLocalRotationEuler);
		GolfCartThrowLidDirectionLocalRotation = Quaternion.Euler(GolfCartThrowLidDirectionLocalRotationEuler);
		RocketDriverSwingPostHitSpinStartTime = RocketDriverSwingPostHitSpinStartFrame * num2;
		RocketDriverSwingPostHitSpinEndTime = RocketDriverSwingPostHitSpinEndFrame * num2;
		RocketDriverMaxLockOnDistanceSquared = RocketDriverMaxLockOnDistance * RocketDriverMaxLockOnDistance;
		RocketDriverSwingHitThrowTime = RocketDriverSwingHitThrowFrame * num2;
		RocketDriverSwingMissThrowDirectionLocalRotation = Quaternion.Euler(RocketDriverSwingMissThrowDirectionLocalRotationEuler);
		RocketDriverSwingHitThrowDirectionLocalRotation = Quaternion.Euler(RocketDriverSwingHitThrowDirectionLocalRotationEuler);
		FreezeBombShotDuration = FreezeBombShotFrames * num2;
		FreezeBombThrowTime = FreezeBombThrowFrame * num2;
		FreezeBombThrowDirectionLocalRotation = Quaternion.Euler(FreezeBombThrowDirectionLocalRotationEuler);
		FreezeBombFlourishVfxStartTime = FreezeBombFlourishVfxStartFrame * num2;
	}
}
