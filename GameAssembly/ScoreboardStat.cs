using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ScoreboardStat : MonoBehaviour
{
	public TextMeshProUGUI label;

	public Image medal;

	public void Initialize(string value, Sprite medalIcon)
	{
		label.text = value;
		medal.enabled = medalIcon != null;
		if (medalIcon != null)
		{
			medal.sprite = medalIcon;
		}
	}
}
