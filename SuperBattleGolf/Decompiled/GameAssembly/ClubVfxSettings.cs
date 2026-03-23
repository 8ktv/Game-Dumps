using UnityEngine;

[CreateAssetMenu(fileName = "Club VFX Settings", menuName = "Settings/VFX/Club VFX Settings")]
public class ClubVfxSettings : ScriptableObject
{
	[SerializeField]
	private VfxType hit;

	public VfxType Hit => hit;
}
