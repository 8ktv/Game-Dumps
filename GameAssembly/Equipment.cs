using UnityEngine;

public class Equipment : MonoBehaviour
{
	[field: SerializeField]
	public EquipmentType Type { get; private set; }

	[field: SerializeField]
	public bool HasCosmetics { get; private set; }
}
