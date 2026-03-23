using UnityEngine;

[CreateAssetMenu(fileName = "Text popup settings", menuName = "Settings/UI/Text popup")]
public class TextPopupUiSettings : ScriptableObject
{
	[field: SerializeField]
	public Color Color { get; private set; } = Color.white;

	[field: SerializeField]
	public float FadeStartDistance { get; private set; }

	[field: SerializeField]
	public float FadeEndDistance { get; private set; }

	[field: SerializeField]
	public float MinTextSize { get; private set; }

	[field: SerializeField]
	public float MaxTextSize { get; private set; }

	[field: SerializeField]
	public float MinTextSizeDistance { get; private set; }

	[field: SerializeField]
	public float MaxTextSizeDistance { get; private set; }

	[field: SerializeField]
	public float PopSizeFactor { get; private set; }

	[field: SerializeField]
	public float PopDuration { get; private set; }

	[field: SerializeField]
	public float Duration { get; private set; }

	[field: SerializeField]
	public float FadeOutDuration { get; private set; }

	[field: SerializeField]
	public float FinalHeightOffset { get; private set; }

	[field: SerializeField]
	public float HeightOffsetDuration { get; private set; }

	public float MinScale { get; private set; }

	public float DurationIncludingFadeOut { get; private set; }

	private void OnValidate()
	{
		Initialize();
	}

	private void OnEnable()
	{
		Initialize();
	}

	private void Initialize()
	{
		MinScale = MinTextSize / MaxTextSize;
		DurationIncludingFadeOut = Duration + FadeOutDuration;
	}
}
