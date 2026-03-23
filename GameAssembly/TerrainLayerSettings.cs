using UnityEngine;

[CreateAssetMenu(fileName = "Terrain layer settings", menuName = "Settings/Gameplay/Terrain layer")]
public class TerrainLayerSettings : ScriptableObject
{
	public struct JobsPhysicsData
	{
		public float dynamicFriction;

		public float linearDamping;

		public JobsPhysicsData(TerrainLayerSettings regularData)
		{
			dynamicFriction = regularData.DynamicFriction;
			linearDamping = regularData.LinearDamping;
		}
	}

	[field: SerializeField]
	public TerrainLayer Layer { get; private set; }

	[field: SerializeField]
	public UnityEngine.TerrainLayer LayerAsset { get; private set; }

	[field: SerializeField]
	public Color SwingPowerBarColor { get; private set; }

	[field: SerializeField]
	public bool IsOutOfBounds { get; private set; }

	[field: Header("Physics")]
	[field: SerializeField]
	[field: Min(0f)]
	public float DynamicFriction { get; private set; } = 0.6f;

	[field: SerializeField]
	[field: Min(0f)]
	public float StaticFriction { get; private set; } = 0.6f;

	[field: SerializeField]
	[field: Range(0f, 1f)]
	public float Bounciness { get; private set; }

	[field: SerializeField]
	[field: Min(0f)]
	public float LinearDamping { get; private set; } = 0.5f;

	[field: SerializeField]
	[field: Min(0f)]
	public float FullStopMaxPitch { get; private set; } = 20f;

	[field: Header("VFX")]
	[field: SerializeField]
	public VfxTerrainMaterial VfxMaterial { get; private set; }

	[field: SerializeField]
	public bool LeavesFootprints { get; private set; }

	[field: Header("Audio")]
	[field: SerializeField]
	public FootstepAudioMaterial FootstepAudioMaterial { get; private set; }
}
