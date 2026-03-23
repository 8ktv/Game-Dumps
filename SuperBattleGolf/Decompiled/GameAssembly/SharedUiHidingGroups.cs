using UnityEngine;

[CreateAssetMenu(fileName = "Shared UI hiding groups", menuName = "Settings/UI/Shared hiding groups")]
public class SharedUiHidingGroups : ScriptableObject
{
	[field: SerializeField]
	public UiHidingGroup HidingGroups { get; private set; }
}
