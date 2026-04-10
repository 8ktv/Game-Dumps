using UnityEngine;

[CreateAssetMenu(fileName = "Wind hittable settings", menuName = "Settings/Gameplay/Hittables/Wind")]
public class WindHittableSettings : ScriptableObject
{
	[field: SerializeField]
	public bool IsAffectedByWind { get; private set; }

	[field: SerializeField]
	[field: DisplayIf("IsAffectedByWind", true)]
	public float WindFactor { get; private set; }

	[field: SerializeField]
	[field: DisplayIf("IsAffectedByWind", true)]
	public float CrossWindFactor { get; private set; }
}
