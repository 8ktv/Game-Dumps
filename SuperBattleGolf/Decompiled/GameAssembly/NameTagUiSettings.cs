using UnityEngine;

[CreateAssetMenu(fileName = "Name tag settings", menuName = "Settings/UI/Name tag")]
public class NameTagUiSettings : ScriptableObject
{
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
	[field: HideInInspector]
	public float MinScale { get; private set; }

	[field: SerializeField]
	public bool FadeOnPlayerProne { get; private set; }

	[field: SerializeField]
	public float ProneFadeoutDelay { get; private set; }

	[field: SerializeField]
	public float ProneFadeoutDuration { get; private set; }

	[field: SerializeField]
	public float PlayerOcclusionFadeoutDelay { get; private set; }

	[field: SerializeField]
	public float PlayerOcclusionFadeoutDuration { get; private set; }

	private void OnValidate()
	{
		MinScale = MinTextSize / MaxTextSize;
	}
}
