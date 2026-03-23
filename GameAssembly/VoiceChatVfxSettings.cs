using System;
using UnityEngine;

[CreateAssetMenu(fileName = "Voice Chat VFX Settings", menuName = "Settings/VFX/Voice Chat VFX Settings")]
public class VoiceChatVfxSettings : ScriptableObject
{
	[Serializable]
	public struct MouthCameraThreshold
	{
		public float maxUpwardness;

		public float forwardednessThreshold;
	}

	[field: SerializeField]
	public Vector2 VolumeRange { get; private set; } = new Vector2(0f, 1f);

	[field: SerializeField]
	public Vector2 LifetimeRange { get; private set; } = new Vector2(0.175f, 0.12f);

	[field: SerializeField]
	public Vector2 BurstCountRange { get; private set; } = new Vector2(3f, 6f);

	[field: SerializeField]
	public Vector2 DefaultXSize { get; private set; } = new Vector2(0.12f, 0.16f);

	[field: SerializeField]
	public Vector2 YSizeRangeMin { get; private set; } = new Vector2(0.3f, 0.65f);

	[field: SerializeField]
	public Vector2 YSizeRangeMax { get; private set; } = new Vector2(0.5f, 0.85f);

	[field: SerializeField]
	public Vector2 ShapeLengthRange { get; private set; } = new Vector2(0.1f, 0.25f);

	[field: SerializeField]
	[field: ElementName("Threshold")]
	public MouthCameraThreshold[] MouthCameraThresholds { get; private set; }
}
