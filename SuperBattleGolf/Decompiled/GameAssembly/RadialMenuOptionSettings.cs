using UnityEngine;

[CreateAssetMenu(fileName = "Radial menu option settings", menuName = "Settings/UI/Radial menu option")]
public class RadialMenuOptionSettings : ScriptableObject
{
	[field: Header("General")]
	[field: SerializeField]
	public Material Material { get; private set; }

	[field: SerializeField]
	public float SpacingWidth { get; private set; }

	[field: SerializeField]
	public float HighlightAnimationDuration { get; private set; }

	[field: Header("Default")]
	[field: SerializeField]
	public Color DefaultColor { get; private set; }

	[field: SerializeField]
	public float DefaultInnerRadius { get; private set; }

	[field: SerializeField]
	public float DefaultThickness { get; private set; }

	[field: Header("Highlight")]
	[field: SerializeField]
	public Color HighlightedColor { get; private set; }

	[field: SerializeField]
	public float HighlightedThickness { get; private set; }

	[field: SerializeField]
	[field: HideInInspector]
	public float DefaultIconRadius { get; private set; }

	[field: SerializeField]
	[field: HideInInspector]
	public float HighlightedIconRadius { get; private set; }

	public void OnValidate()
	{
		float num = 0.575f;
		DefaultIconRadius = DefaultInnerRadius + DefaultThickness * num;
		HighlightedIconRadius = DefaultInnerRadius + HighlightedThickness * num;
	}
}
