using UnityEngine;
using UnityEngine.Localization;

[CreateAssetMenu(fileName = "UI settings", menuName = "Settings/UI/UI settings")]
public class UiSettings : ScriptableObject
{
	[field: Header("Icons")]
	[field: SerializeField]
	public Sprite UnknownItemIcon { get; private set; }

	[field: Header("General")]
	[field: SerializeField]
	public Color LoadingScreenBackgroundColor { get; private set; }

	[field: SerializeField]
	public Color TextHighlightColor { get; private set; }

	[field: SerializeField]
	public Color TextRedHighlightColor { get; private set; }

	[field: SerializeField]
	public Color TextOrangeHighlightColor { get; private set; }

	[field: SerializeField]
	public Vector2 ScrollRectControllerReselectDefaultPadding { get; private set; }

	[field: Header("Strings")]
	[field: SerializeField]
	public LocalizedString GolfClubLocalizedName { get; private set; }

	[field: Header("Swings")]
	[field: SerializeField]
	public float SwingPitchDisplayScreenCenterDuration { get; private set; }

	[field: SerializeField]
	public float SwingPowerBarFlagPreviewMaxYaw { get; private set; }

	[field: Header("Name tags")]
	[field: SerializeField]
	public Vector3 PlayerNameTagLocalOffset { get; private set; }

	[field: SerializeField]
	public Vector3 PlayerNameTagWorldOffset { get; private set; }

	[field: SerializeField]
	public Vector3 BallNameTagLocalOffset { get; private set; }

	[field: SerializeField]
	public Vector3 BallNameTagWorldOffset { get; private set; }

	[field: SerializeField]
	public Vector3 SpectatorNameTagLocalOffset { get; private set; }

	[field: SerializeField]
	public Vector3 SpectatorNameTagWorldOffset { get; private set; }

	[field: Header("Player text popups")]
	[field: SerializeField]
	public float TimeBetweenPlayerTextPopups { get; private set; }

	[field: SerializeField]
	public Vector3 PlayerTextPopupLocalOffset { get; private set; }

	[field: SerializeField]
	public Vector3 PlayerTextPopupWorldOffset { get; private set; }

	[field: Header("Restart/give up prompt")]
	[field: SerializeField]
	public float RestartPromptStandStillShowDelay { get; private set; }

	[field: SerializeField]
	public float RestartPromptFadeInTime { get; private set; }

	[field: SerializeField]
	public float RestartPromptFadeOutTime { get; private set; }

	[field: SerializeField]
	[field: HideInInspector]
	public string TextHighlightStartTag { get; private set; }

	[field: SerializeField]
	[field: HideInInspector]
	public string TextRedHighlightStartTag { get; private set; }

	[field: SerializeField]
	[field: HideInInspector]
	public string TextOrangeHighlightStartTag { get; private set; }

	public string TextColorEndTag { get; private set; } = "</color>";

	private void OnValidate()
	{
		TextHighlightStartTag = "<color=#" + ColorUtility.ToHtmlStringRGBA(TextHighlightColor) + ">";
		TextRedHighlightStartTag = "<color=#" + ColorUtility.ToHtmlStringRGBA(TextRedHighlightColor) + ">";
		TextOrangeHighlightStartTag = "<color=#" + ColorUtility.ToHtmlStringRGBA(TextOrangeHighlightColor) + ">";
	}

	public string ApplyColorTag(string text, TextHighlight highlight)
	{
		string text2 = ((highlight != TextHighlight.Red) ? TextHighlightStartTag : TextRedHighlightStartTag);
		return text2 + text + TextColorEndTag;
	}
}
