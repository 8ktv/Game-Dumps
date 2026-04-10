using TMPro;
using UnityEngine;

public class GameVersionLabel : MonoBehaviour
{
	[SerializeField]
	private TextMeshProUGUI label;

	[SerializeField]
	private bool shorten;

	public static string GetVersion(bool shorten = false)
	{
		string text = Resources.Load<TextAsset>("buildstring").text;
		return (shorten ? "SBG" : "Super Battle Golf") + " " + text;
	}

	private void Awake()
	{
		label.text = GetVersion(shorten);
	}
}
