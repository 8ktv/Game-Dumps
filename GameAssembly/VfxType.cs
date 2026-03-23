using UnityEngine;

public enum VfxType
{
	None = -1,
	[InspectorName("Basic/Ball/Projectile Trail")]
	BasicBallProjectileTrail,
	[InspectorName("Basic/Ball/Launch")]
	BasicBallLaunch,
	[InspectorName("Basic/Ball/Hit")]
	BasicBallHit,
	[InspectorName("Basic/Ball/Win Start")]
	BasicBallWinStart,
	[InspectorName("Basic/Ball/Win End")]
	BasicBallWinEnd,
	[InspectorName("Item Box/Item Box Acquire")]
	ItemBoxAcquire,
	[InspectorName("Basic/Club/Hit")]
	BasicClubHit,
	[InspectorName("Poofs/Directional Poof Mesh")]
	DirectionalPoofMesh,
	[InspectorName("Character/Footprint")]
	CharacterFootprint,
	[InspectorName("UI/First Place Crown")]
	FirstPlaceCrown,
	[InspectorName("Items/Post Hit Spin")]
	ItemPostHitSpin,
	[InspectorName("Basic/Ball/Putting Trail")]
	BasicBallPuttingTrail,
	[InspectorName("Player/UI/Voice Chat")]
	VoiceChat,
	[InspectorName("Items/Dueling Pistol/Muzzle")]
	DuelingPistolMuzzle,
	[InspectorName("Items/Dueling Pistol/Impact")]
	DuelingPistolImpact,
	[InspectorName("Character/Knocked Out")]
	KnockedOut,
	[InspectorName("Character/Knocked Out End")]
	KnockedOutEnd,
	[InspectorName("Items/Shotgun/Muzzle")]
	ShotgunMuzzle,
	[InspectorName("Items/Shotgun/Tracer")]
	ShotgunTracer,
	[InspectorName("Items/Shotgun/Impact")]
	ShotgunImpact,
	[InspectorName("Golf Cart/Jump Start")]
	GolfCartJumpStart,
	[InspectorName("Golf Cart/Jump End")]
	GolfCartJumpEnd,
	[InspectorName("Golf Cart/Collision")]
	GolfCartCollision,
	[InspectorName("Items/Mine/Burrow")]
	MineBurrow,
	[InspectorName("Items/Mine/Armed Start")]
	MineArmedStart,
	[InspectorName("Items/Mine/Explosion")]
	MineExplosion,
	[InspectorName("Items/Airhorn/Range Indicator")]
	AirhornRangeIndicator,
	[InspectorName("Items/Airhorn/Activation")]
	AirhornActivation,
	[InspectorName("Items/Airhorn/Player Triggered")]
	AirhornPlayerTriggered,
	[InspectorName("Items/Orbital Laser/Anticipation")]
	OrbitalLaserAnticipation,
	[InspectorName("Items/Orbital Laser/Explosion")]
	OrbitalLaserExplosion,
	[InspectorName("Items/Orbital Laser/End")]
	OrbitalLaserEnd,
	[InspectorName("Target Dummy/Spin CW")]
	TargetDummySpinCW,
	[InspectorName("Target Dummy/Spin CCW")]
	TargetDummySpinCCW,
	[InspectorName("Items/Rocket Launcher/Back Blast")]
	RocketLauncherBackBlast,
	[InspectorName("Items/Rocket Launcher/Muzzle")]
	RocketLauncherMuzzle,
	[InspectorName("Items/Rocket Launcher/Rocket Trail")]
	RocketLauncherRocketTrail,
	[InspectorName("Items/Rocket Launcher/Rocket Explosion")]
	RocketLauncherRocketExplosion,
	[InspectorName("Items/Electromagnet/Electromagnet Activation")]
	ElectromagnetActivation,
	[InspectorName("Items/Electromagnet/Electromagnet Shield")]
	ElectromagnetShield,
	[InspectorName("Items/Electromagnet/Electromagnet Shield End")]
	ElectromagnetShieldEnd,
	[InspectorName("Items/Electromagnet/Electromagnet Shield Hit")]
	ElectromagnetShieldHit,
	[InspectorName("Decals/Explosion Decal")]
	ExplosionDecal,
	[InspectorName("Checkpoint/Activation")]
	CheckpointActivation,
	[InspectorName("Items/Orbital Laser/Remote Activation")]
	OrbitalLaserRemoteActivation,
	[InspectorName("Water/Small Impact")]
	WaterImpactSmall,
	[InspectorName("Water/Medium Impact")]
	WaterImpactMedium,
	[InspectorName("Water/Large Impact")]
	WaterImpactLarge,
	[InspectorName("Character/Knock Out Shield")]
	KnockOutShield,
	[InspectorName("Character/Knock Out Shield End")]
	KnockOutShieldEnd,
	[InspectorName("Character/Knock Out Blocked")]
	KnockOutBlocked,
	[InspectorName("Props/Ball Dispenser/Start")]
	BallDispenserStart,
	[InspectorName("Props/Ball Dispenser/End")]
	BallDispenserEnd,
	[InspectorName("Props/Coffee Dispenser/Start")]
	CoffeeDispenserStart,
	[InspectorName("Props/Coffee Dispenser/End")]
	CoffeeDispenserEnd,
	[InspectorName("Props/Door/Double Door Open")]
	DoubleDoorOpen,
	[InspectorName("UI/Rivalry")]
	Rivalry,
	[InspectorName("Items/Spring Boots/Spring")]
	SpringBootsSpring,
	[InspectorName("Items/Spring Boots/Launch")]
	SpringBootsLaunch,
	[InspectorName("Items/Spring Boots/Landing")]
	SpringBootsLanding,
	[InspectorName("Item Box/Item Box Spawn")]
	ItemBoxSpawn,
	[InspectorName("Character/Respawn")]
	Respawn,
	[InspectorName("Spectator Camera/Heart")]
	SpectatorCameraHeart,
	[InspectorName("Spectator Camera/Shocked")]
	SpectatorCameraShocked,
	[InspectorName("Spectator Camera/Sad")]
	SpectatorCameraSad,
	[InspectorName("Spectator Camera/Confused")]
	SpectatorCameraConfused,
	[InspectorName("Golf Cart/Horn Short")]
	GolfCartHornShort,
	[InspectorName("Golf Cart/Horn Long")]
	GolfCartHornLong,
	[InspectorName("Swing/Slash")]
	SwingSlash,
	[InspectorName("Swing/Nice Shot")]
	SwingNiceShot,
	[InspectorName("Water/Player Out Of Bounds")]
	WaterPlayerOutOfBounds,
	[InspectorName("Swing/Overcharged Club")]
	SwingOverchargedClub,
	[InspectorName("Swing/Overcharged Hit")]
	SwingOverchargedHit,
	[InspectorName("Items/Golf Cart Briefcase/Open Start")]
	GolfCartBriefcaseOpenStart,
	[InspectorName("Items/Golf Cart Briefcase/Open End")]
	GolfCartBriefcaseOpenEnd,
	[InspectorName("Golf Cart/Spawn")]
	GolfCartSpawn,
	[InspectorName("Water/Item Out Of Bounds")]
	WaterItemOutOfBounds,
	[InspectorName("Water/Golf Cart Out Of Bounds")]
	WaterGolfCartOutOfBounds,
	[InspectorName("Fog/Player Out Of Bounds")]
	FogPlayerOutOfBounds,
	[InspectorName("Fog/Item Out Of Bounds")]
	FogItemOutOfBounds,
	[InspectorName("Fog/Golf Cart Out Of Bounds")]
	FogGolfCartOutOfBounds,
	[InspectorName("Boundary/Out Of Bounds Sparkle")]
	BoundaryOutOfBoundsSparkle,
	[InspectorName("Boundary/Item Out Of Bounds")]
	BoundaryItemOutOfBounds,
	[InspectorName("Boundary/Ball Out Of Bounds")]
	BoundaryBallOutOfBounds,
	[InspectorName("Ball/Respawn")]
	BallRespawn
}
