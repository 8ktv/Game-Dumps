using UnityEngine;

[CreateAssetMenu(fileName = "Player inventory settings", menuName = "Settings/Gameplay/Player inventory")]
public class PlayerInventorySettings : ScriptableObject
{
	[field: SerializeField]
	public int MaxItems { get; private set; }

	[field: SerializeField]
	public float DropItemVerticalOffset { get; private set; }
}
