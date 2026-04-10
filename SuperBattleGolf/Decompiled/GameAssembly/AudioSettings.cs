using FMOD.Studio;
using FMODUnity;
using UnityEngine;

[CreateAssetMenu(fileName = "Audio settings", menuName = "Settings/Audio/Audio")]
public class AudioSettings : ScriptableObject
{
	[field: Header("Game")]
	[field: SerializeField]
	public EventReference ItemBoxCollectedEvent { get; private set; }

	[field: SerializeField]
	public EventReference BallInHoleEvent { get; private set; }

	[field: Header("Player")]
	[field: SerializeField]
	public EventReference FootstepEvent { get; private set; }

	[field: SerializeField]
	public EventReference JumpEvent { get; private set; }

	[field: SerializeField]
	public EventReference SwingChargeEvent { get; private set; }

	[field: SerializeField]
	public EventReference SwingEvent { get; private set; }

	[field: SerializeField]
	public EventReference OverchargedSwingEvent { get; private set; }

	[field: SerializeField]
	public EventReference SwingHitEvent { get; private set; }

	[field: SerializeField]
	public EventReference KnockedOutLoopEvent { get; private set; }

	[field: SerializeField]
	public EventReference KnockoutImmunityStartEvent { get; private set; }

	[field: SerializeField]
	public EventReference KnockoutImmunityEndEvent { get; private set; }

	[field: SerializeField]
	public EventReference KnockoutImmunityBlockedKnockoutEvent { get; private set; }

	[field: SerializeField]
	public EventReference SpeedBoostEvent { get; private set; }

	[field: SerializeField]
	public EventReference RespawnEvent { get; private set; }

	[field: SerializeField]
	public EventReference PlayerWaterSplashEvent { get; private set; }

	[field: SerializeField]
	public EventReference PlayerWaterWadeEvent { get; private set; }

	[field: SerializeField]
	public EventReference PlayerEmoteClapEvent { get; private set; }

	[field: SerializeField]
	public EventReference PlayerEmoteFacepalmEvent { get; private set; }

	[field: SerializeField]
	public EventReference PlayerEmotePointLaughEvent { get; private set; }

	[field: SerializeField]
	public EventReference PlayerEmoteThumbsUpEvent { get; private set; }

	[field: SerializeField]
	public EventReference PlayerEmoteVPoseEvent { get; private set; }

	[field: SerializeField]
	public EventReference PlayerEmoteWaveEvent { get; private set; }

	[field: SerializeField]
	public EventReference PlayerEmoteChickenEvent { get; private set; }

	[field: SerializeField]
	public EventReference PlayerEmoteFistPumpEvent { get; private set; }

	[field: SerializeField]
	public EventReference PlayerEmoteHandsUpEvent { get; private set; }

	[field: SerializeField]
	public EventReference PlayerRestartEvent { get; private set; }

	[field: SerializeField]
	public EventReference FallingInHoleEvent { get; private set; }

	[field: Header("Ball")]
	[field: SerializeField]
	public EventReference BallRespawnEvent { get; private set; }

	[field: Header("Items")]
	[field: SerializeField]
	public EventReference ItemWaterSplash { get; private set; }

	[field: SerializeField]
	public EventReference ThrownItemDisappearEvent { get; private set; }

	[field: SerializeField]
	public EventReference CoffeeDrinkEvent { get; private set; }

	[field: SerializeField]
	public EventReference GunAimEvent { get; private set; }

	[field: SerializeField]
	public EventReference PistolShotEvent { get; private set; }

	[field: SerializeField]
	public EventReference ElephantGunShotEvent { get; private set; }

	[field: SerializeField]
	public EventReference RocketLauncherAimEvent { get; private set; }

	[field: SerializeField]
	public EventReference RocketLauncherShotEvent { get; private set; }

	[field: SerializeField]
	public EventReference RocketEngineEvent { get; private set; }

	[field: SerializeField]
	public EventReference RocketExplosionEvent { get; private set; }

	[field: SerializeField]
	public EventReference AirhornEvent { get; private set; }

	[field: SerializeField]
	public EventReference LandminePlantJamEvent { get; private set; }

	[field: SerializeField]
	public EventReference LandminePlantStompEvent { get; private set; }

	[field: SerializeField]
	public EventReference LandmineArmEvent { get; private set; }

	[field: SerializeField]
	public EventReference LandmineArmedBeepEvent { get; private set; }

	[field: SerializeField]
	public EventReference LandmineExplosionEvent { get; private set; }

	[field: SerializeField]
	public EventReference ElectromagnetIdleEvent { get; private set; }

	[field: SerializeField]
	public EventReference ElectromagnetActivationEvent { get; private set; }

	[field: SerializeField]
	public EventReference ElectromagnetShieldActivationEvent { get; private set; }

	[field: SerializeField]
	public EventReference ElectromagnetShieldDeactivationEvent { get; private set; }

	[field: SerializeField]
	public EventReference ElectromagnetShieldHumEvent { get; private set; }

	[field: SerializeField]
	public EventReference ElectromagnetShieldHitEvent { get; private set; }

	[field: SerializeField]
	public EventReference OrbitalLaserActivationEvent { get; private set; }

	[field: SerializeField]
	public EventReference OrbitalLaserAnticipationEvent { get; private set; }

	[field: SerializeField]
	public EventReference OrbitalLaserExplosionEvent { get; private set; }

	[field: SerializeField]
	public EventReference SpringBootsActivationEvent { get; private set; }

	[field: SerializeField]
	public EventReference SpringBootFlipEvent { get; private set; }

	[field: SerializeField]
	public EventReference GolfCartBriefcaseOpenStart { get; private set; }

	[field: SerializeField]
	public EventReference GolfCartBriefcaseOpenEnd { get; private set; }

	[field: SerializeField]
	public EventReference RocketDriverIdleEvent { get; private set; }

	[field: SerializeField]
	public EventReference RocketDriverAimEvent { get; private set; }

	[field: SerializeField]
	public EventReference RocketDriverEnteredOverchargeEvent { get; private set; }

	[field: SerializeField]
	public EventReference RocketDriverSwingEvent { get; private set; }

	[field: SerializeField]
	public EventReference RocketDriverOverchargedSwingEvent { get; private set; }

	[field: SerializeField]
	public EventReference RocketDriverSwingHitEvent { get; private set; }

	[field: SerializeField]
	public EventReference RocketDriverPostHitSpinEvent { get; private set; }

	[field: SerializeField]
	public EventReference RocketDriverThrownUsedLoopEvent { get; private set; }

	[field: SerializeField]
	public EventReference RocketDriverUsedThrownExplosionEvent { get; private set; }

	[field: SerializeField]
	public EventReference RocketDriverTrailLoopEvent { get; private set; }

	[field: SerializeField]
	public EventReference RocketDriverTrailStopEvent { get; private set; }

	[field: SerializeField]
	public EventReference FreezeBombAimEvent { get; private set; }

	[field: SerializeField]
	public EventReference FreezeBombShotEvent { get; private set; }

	[field: SerializeField]
	public EventReference FreezeBombProjectileLoopEvent { get; private set; }

	[field: SerializeField]
	public EventReference FreezeBombExplosionEvent { get; private set; }

	[field: SerializeField]
	public EventReference FreezeBombUnfreezeEvent { get; private set; }

	[field: SerializeField]
	public EventReference FreezeBombPlatformShakeLoopEvent { get; private set; }

	[field: SerializeField]
	public EventReference FreezeBombPlatformBreakEvent { get; private set; }

	[field: Header("Golf cart")]
	[field: SerializeField]
	public EventReference GolfCartEngine { get; private set; }

	[field: SerializeField]
	public EventReference GolfCartTurnEngineOff { get; private set; }

	[field: SerializeField]
	public EventReference GolfCartHonkEvent { get; private set; }

	[field: SerializeField]
	public EventReference GolfCartSpecialHonkEvent { get; private set; }

	[field: SerializeField]
	public EventReference GolfCartCollisionEvent { get; private set; }

	[field: SerializeField]
	public EventReference GolfCartJumpEvent { get; private set; }

	[field: SerializeField]
	public EventReference GolfCartLandEvent { get; private set; }

	[field: SerializeField]
	public EventReference GolfCartWaterSplashEvent { get; private set; }

	[field: SerializeField]
	public EventReference GolfCartWaterWadeEvent { get; private set; }

	[field: Header("Projectiles")]
	[field: SerializeField]
	public EventReference ProjectileHitEvent { get; private set; }

	[field: SerializeField]
	public EventReference HomingWarningEvent { get; private set; }

	[field: SerializeField]
	public float HomingProjectileWarningMinDistance { get; private set; }

	[field: SerializeField]
	public float HomingProjectileWarningMaxDistance { get; private set; }

	[field: SerializeField]
	public float HomingRocketWarningMinDistance { get; private set; }

	[field: SerializeField]
	public float HomingRocketWarningMaxDistance { get; private set; }

	[field: Header("Checkpoints")]
	[field: SerializeField]
	public EventReference CheckpointActivationEvent { get; private set; }

	[field: Header("Music")]
	[field: SerializeField]
	public EventReference MainMenuMusicEvent { get; private set; }

	[field: SerializeField]
	public EventReference DrivingRangeMusicEvent { get; private set; }

	[field: Header("Tee-off")]
	[field: SerializeField]
	public EventReference TeeOff5Event { get; private set; }

	[field: SerializeField]
	public EventReference TeeOff4Event { get; private set; }

	[field: SerializeField]
	public EventReference TeeOff3Event { get; private set; }

	[field: SerializeField]
	public EventReference TeeOff2Event { get; private set; }

	[field: SerializeField]
	public EventReference TeeOff1Event { get; private set; }

	[field: SerializeField]
	public EventReference TeeOffGolfEvent { get; private set; }

	[field: Header("Environment")]
	[field: SerializeField]
	public EventReference BallDispenserActivationEvent { get; private set; }

	[field: SerializeField]
	public EventReference FoliageImpactEvent { get; private set; }

	[field: SerializeField]
	public EventReference FoliageLoopEvent { get; private set; }

	[field: SerializeField]
	public float MinMovementInFoliageSpeed { get; private set; }

	[field: SerializeField]
	public float MaxMovementInFoliageSpeed { get; private set; }

	[field: SerializeField]
	public float MovementInFoliageExitDelay { get; private set; }

	[field: SerializeField]
	public EventReference JumpPadTriggeredEvent { get; private set; }

	[field: SerializeField]
	public EventReference BreakableIceCrackingEvent { get; private set; }

	[field: SerializeField]
	public EventReference BreakableIceBreakingEvent { get; private set; }

	[field: Header("Announcer")]
	[field: SerializeField]
	public EventReference AnnouncerMainMenuTitle { get; private set; }

	[field: SerializeField]
	public EventReference AnnouncerTeeOff5Event { get; private set; }

	[field: SerializeField]
	public EventReference AnnouncerTeeOff4Event { get; private set; }

	[field: SerializeField]
	public EventReference AnnouncerTeeOff3Event { get; private set; }

	[field: SerializeField]
	public EventReference AnnouncerTeeOff2Event { get; private set; }

	[field: SerializeField]
	public EventReference AnnouncerTeeOff1Event { get; private set; }

	[field: SerializeField]
	public EventReference AnnouncerTeeOffGolfEvent { get; private set; }

	[field: SerializeField]
	public EventReference Announcer10SecondsRemainingEvent { get; private set; }

	[field: SerializeField]
	public EventReference AnnouncerOvertimeEvent { get; private set; }

	[field: SerializeField]
	public EventReference AnnouncerFinishedEvent { get; private set; }

	[field: SerializeField]
	public EventReference AnnouncerNiceShotEvent { get; private set; }

	[field: SerializeField]
	public EventReference AnnouncerFirstPlaceEvent { get; private set; }

	[field: SerializeField]
	public EventReference AnnouncerChipInEvent { get; private set; }

	[field: SerializeField]
	public EventReference AnnouncerParEvent { get; private set; }

	[field: SerializeField]
	public EventReference AnnouncerBirdieEvent { get; private set; }

	[field: SerializeField]
	public EventReference AnnouncerEagleEvent { get; private set; }

	[field: SerializeField]
	public EventReference AnnouncerAlbatrossEvent { get; private set; }

	[field: SerializeField]
	public EventReference AnnouncerCondorEvent { get; private set; }

	[field: SerializeField]
	public EventReference AnnouncerHoleInOneEvent { get; private set; }

	[field: SerializeField]
	public EventReference AnnouncerSpeedrunEvent { get; private set; }

	[field: Header("Match end")]
	[field: SerializeField]
	public EventReference MatchEndCountdownUrgentTickEvent { get; private set; }

	[field: SerializeField]
	public EventReference FinishedWhistleEvent { get; private set; }

	[field: Header("Camera")]
	[field: SerializeField]
	public EventReference UnderwaterCameraAmbience { get; private set; }

	[field: SerializeField]
	public EventReference CameraSplashIntoWaterEvent { get; private set; }

	[field: SerializeField]
	public EventReference CameraSplashOutOfWaterEvent { get; private set; }

	[field: SerializeField]
	public EventReference CameraEmoteShockedEvent { get; private set; }

	[field: SerializeField]
	public EventReference CameraEmoteHeartEvent { get; private set; }

	[field: SerializeField]
	public EventReference CameraEmoteWorriedEvent { get; private set; }

	[field: SerializeField]
	public EventReference CameraEmoteConfusedEvent { get; private set; }

	[field: SerializeField]
	public EventReference CameraEmoteCheerEvent { get; private set; }

	[field: Header("Item dispensers")]
	[field: SerializeField]
	public EventReference CoffeeDispenserActivationEvent { get; private set; }

	[field: Header("Match setup station")]
	[field: SerializeField]
	public EventReference MatchSetupStationInteractEvent { get; private set; }

	[field: SerializeField]
	public EventReference DrivingRangeNextCameraButtonPressedEvent { get; private set; }

	[field: Header("Snapshots")]
	[field: SerializeField]
	public EventReference ElectromagnetShieldMuffleSnapshot { get; private set; }

	[field: SerializeField]
	public EventReference UnderwaterCameraSnapshot { get; private set; }

	[field: SerializeField]
	public EventReference FrozenInIceSnapshot { get; private set; }

	[field: Header("UI")]
	[field: SerializeField]
	public EventReference MainMenuHover { get; private set; }

	[field: SerializeField]
	public EventReference MainMenuSelect { get; private set; }

	[field: SerializeField]
	public EventReference PauseMenuHover { get; private set; }

	[field: SerializeField]
	public EventReference PauseMenuSelect { get; private set; }

	[field: SerializeField]
	public EventReference GenericButtonSelect { get; private set; }

	[field: SerializeField]
	public EventReference SettingsTabSelect { get; private set; }

	[field: SerializeField]
	public EventReference MatchSetupTabSelect { get; private set; }

	[field: SerializeField]
	public EventReference MatchSetupOpen { get; private set; }

	[field: SerializeField]
	public EventReference MatchSetupClose { get; private set; }

	[field: SerializeField]
	public EventReference MatchSetupReorderableSelect { get; private set; }

	[field: SerializeField]
	public EventReference MatchSetupReorderableAssigned { get; private set; }

	[field: SerializeField]
	public EventReference CosmeticsButtonHover { get; private set; }

	[field: SerializeField]
	public EventReference CosmeticsButtonSelect { get; private set; }

	[field: SerializeField]
	public EventReference CosmeticsButtonSelectDisabled { get; private set; }

	[field: SerializeField]
	public EventReference CosmeticsPurchase { get; private set; }

	[field: SerializeField]
	public EventReference CosmeticsOpen { get; private set; }

	[field: SerializeField]
	public EventReference SliderOnValueChange { get; private set; }

	[field: SerializeField]
	public EventReference DropdownOpen { get; private set; }

	[field: SerializeField]
	public EventReference DropdownOptionSelect { get; private set; }

	[field: SerializeField]
	public EventReference StartMatchButtonSelect { get; private set; }

	[field: SerializeField]
	public EventReference FullScreenMessageOpen { get; private set; }

	[field: SerializeField]
	public EventReference FullScreenMessageClose { get; private set; }

	[field: SerializeField]
	public EventReference RadialMenuHover { get; private set; }

	[field: SerializeField]
	public EventReference RadialMenuSelect { get; private set; }

	[field: SerializeField]
	public EventReference GenericControllerHover { get; private set; }

	[field: SerializeField]
	public EventReference SliderControllerActivation { get; private set; }

	[field: SerializeField]
	public EventReference SliderControllerDeactivation { get; private set; }

	[field: Header("Ambience")]
	[field: SerializeField]
	public EventReference AmbienceWind { get; private set; }

	[field: SerializeField]
	public EventReference AmbienceSnowWind { get; private set; }

	public static float MinMovementInFoliageSpeedSquared { get; private set; }

	public static PARAMETER_ID MaterialTypeId { get; private set; }

	public static PARAMETER_ID StrengthId { get; private set; }

	public static PARAMETER_ID LockedOnId { get; private set; }

	public static PARAMETER_ID ObjectId { get; private set; }

	public static PARAMETER_ID IntensityId { get; private set; }

	public static PARAMETER_ID DistanceId { get; private set; }

	public static PARAMETER_ID PanningId { get; private set; }

	public static PARAMETER_ID VelocityId { get; private set; }

	public static PARAMETER_ID CameraInWaterId { get; private set; }

	public static PARAMETER_ID SpeedId { get; private set; }

	public static PARAMETER_ID WindSpeedId { get; private set; }

	public static PARAMETER_ID WindPanningId { get; private set; }

	public static PARAMETER_ID ChargeUpId { get; private set; }

	public void Initialize()
	{
		MinMovementInFoliageSpeedSquared = MinMovementInFoliageSpeed * MinMovementInFoliageSpeed;
		RuntimeManager.GetEventDescription(FootstepEvent).getParameterDescriptionByName("MaterialType", out var parameter);
		MaterialTypeId = parameter.id;
		RuntimeManager.GetEventDescription(SwingEvent).getParameterDescriptionByName("Strength", out parameter);
		StrengthId = parameter.id;
		RuntimeManager.GetEventDescription(OverchargedSwingEvent).getParameterDescriptionByName("locked on", out parameter);
		LockedOnId = parameter.id;
		RuntimeManager.GetEventDescription(SwingHitEvent).getParameterDescriptionByName("Object", out parameter);
		ObjectId = parameter.id;
		RuntimeManager.GetEventDescription(GolfCartEngine).getParameterDescriptionByName("Intensity", out parameter);
		IntensityId = parameter.id;
		EventDescription eventDescription = RuntimeManager.GetEventDescription(HomingWarningEvent);
		eventDescription.getParameterDescriptionByName("Distance", out parameter);
		DistanceId = parameter.id;
		eventDescription.getParameterDescriptionByName("Panning", out parameter);
		PanningId = parameter.id;
		RuntimeManager.GetEventDescription(FoliageLoopEvent).getParameterDescriptionByName("Velocity", out parameter);
		VelocityId = parameter.id;
		RuntimeManager.GetEventDescription(UnderwaterCameraSnapshot).getParameterDescriptionByName("Camera In Water", out parameter);
		CameraInWaterId = parameter.id;
		RuntimeManager.GetEventDescription(CameraSplashIntoWaterEvent).getParameterDescriptionByName("Speed", out parameter);
		SpeedId = parameter.id;
		eventDescription = RuntimeManager.GetEventDescription(AmbienceWind);
		eventDescription.getParameterDescriptionByName("Wind Speed", out parameter);
		WindSpeedId = parameter.id;
		eventDescription.getParameterDescriptionByName("Panning", out parameter);
		WindPanningId = parameter.id;
		RuntimeManager.GetEventDescription(RocketDriverIdleEvent).getParameterDescriptionByName("Charge Up", out parameter);
		ChargeUpId = parameter.id;
	}
}
