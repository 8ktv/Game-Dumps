using UnityEngine;

[CreateAssetMenu(fileName = "Out of bounds hazard settings", menuName = "Settings/Gameplay/Out of bounds hazard")]
public class OutOfBoundsHazardSettings : ScriptableObject
{
	[field: SerializeField]
	public OutOfBoundsHazard Type { get; private set; }

	[field: SerializeField]
	public Color SwingPowerBarColor { get; private set; }

	[field: SerializeField]
	public Color SwingPowerBarOutOfBoundsOverlayColor { get; private set; }
}
