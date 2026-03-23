using UnityEngine;

[CreateAssetMenu(fileName = "Ball VFX Settings", menuName = "Settings/VFX/Ball VFX Settings")]
public class BallVfxSettings : ScriptableObject
{
	[SerializeField]
	private VfxType puttingTrail;

	[SerializeField]
	private VfxType launch;

	[SerializeField]
	private VfxType hit;

	[SerializeField]
	private VfxType winStart;

	[SerializeField]
	private VfxType winEnd;

	public VfxType PuttingTrail => puttingTrail;

	public VfxType Launch => launch;

	public VfxType Hit => hit;

	public VfxType WinStart => winStart;

	public VfxType WinEnd => winEnd;
}
