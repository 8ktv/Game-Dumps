using TMPro;
using UnityEngine;

public class GameVersionLabel : MonoBehaviour
{
	[SerializeField]
	private TextMeshProUGUI label;

	public static string GetVersion()
	{
		return Resources.Load<TextAsset>("buildstring").text;
	}

	private void Awake()
	{
		label.text = GetVersion();
	}
}
