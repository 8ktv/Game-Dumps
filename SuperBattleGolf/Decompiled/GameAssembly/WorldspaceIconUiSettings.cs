using UnityEngine;

[CreateAssetMenu(fileName = "Worldspace icon settings", menuName = "Settings/UI/Worldspace icon")]
public class WorldspaceIconUiSettings : ScriptableObject
{
	[field: Header("Position")]
	[field: SerializeField]
	public Vector3 LocalOffset { get; private set; }

	[field: SerializeField]
	public Vector3 WorldOffset { get; private set; }

	[field: Header("Fading")]
	[field: SerializeField]
	public float FadeStartDistance { get; private set; }

	[field: SerializeField]
	public float FadeEndDistance { get; private set; }

	[field: Header("Onscreen icon")]
	[field: SerializeField]
	public float MinSize { get; private set; }

	[field: SerializeField]
	public float MaxSize { get; private set; }

	[field: SerializeField]
	public float MinSizeDistance { get; private set; }

	[field: SerializeField]
	public float MaxSizeDistance { get; private set; }

	[field: SerializeField]
	public float OnscreenArrowSizeFactor { get; private set; }

	[field: SerializeField]
	public float OnscreenIconYPivot { get; private set; }

	[field: SerializeField]
	public float OnscreenMinIconLocalYPosition { get; private set; }

	[field: SerializeField]
	public float OnscreenMaxIconLocalYPosition { get; private set; }

	[field: Header("Offscreen icon")]
	[field: SerializeField]
	public float OffscreenSize { get; private set; }

	[field: SerializeField]
	public float OffscreenArrowDistance { get; private set; }

	[field: SerializeField]
	public float OffscreenDistanceFromScreenEdge { get; private set; }

	[field: Header("Colors")]
	[field: SerializeField]
	public Color DistanceLabelColor { get; private set; }

	[field: SerializeField]
	public Color ArrowColor { get; private set; }
}
