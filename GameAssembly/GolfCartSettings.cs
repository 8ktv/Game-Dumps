using UnityEngine;

[CreateAssetMenu(fileName = "Golf cart settings", menuName = "Settings/Gameplay/Golf cart/General")]
public class GolfCartSettings : ScriptableObject
{
	[field: SerializeField]
	public GolfCartInfo Prefab { get; private set; }

	[field: SerializeField]
	[field: ElementName("Seat")]
	public GolfCartSeatSettings[] SeatSettings { get; private set; }

	[field: SerializeField]
	public float ExitForcedDiveSpeed { get; private set; }

	[field: SerializeField]
	public float ResponsiblePlayerTimeout { get; private set; }

	[field: SerializeField]
	public float RunOverPlayerKnockoutMinSpeed { get; private set; }

	[field: SerializeField]
	public float RunOverPlayerKnockoutMinRelativeSpeed { get; private set; }

	[field: SerializeField]
	public float RunOverPlayerKnockoutMaxRelativeSpeed { get; private set; }

	[field: SerializeField]
	public float RunOverPlayerKnockoutMinHorizontalKnockback { get; private set; }

	[field: SerializeField]
	public float RunOverPlayerKnockoutMaxHorizontalKnockback { get; private set; }

	[field: SerializeField]
	public float RunOverPlayerKnockoutMinVerticalKnockback { get; private set; }

	[field: SerializeField]
	public float RunOverPlayerKnockoutMaxVerticalKnockback { get; private set; }

	[field: SerializeField]
	public float HonkVfxCooldown { get; private set; }

	public int MaxPassengers { get; private set; }

	public float ExitForcedDiveSpeedSquared { get; private set; }

	public float RunOverPlayerKnockoutMinSpeedSquared { get; private set; }

	public float RunOverPlayerKnockoutMinRelativeSpeedSquared { get; private set; }

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
		MaxPassengers = SeatSettings.Length;
		ExitForcedDiveSpeedSquared = ExitForcedDiveSpeed * ExitForcedDiveSpeed;
		RunOverPlayerKnockoutMinSpeedSquared = RunOverPlayerKnockoutMinSpeed * RunOverPlayerKnockoutMinSpeed;
		RunOverPlayerKnockoutMinRelativeSpeedSquared = RunOverPlayerKnockoutMinRelativeSpeed * RunOverPlayerKnockoutMinRelativeSpeed;
	}
}
