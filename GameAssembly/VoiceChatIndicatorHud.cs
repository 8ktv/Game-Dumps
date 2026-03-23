using UnityEngine;
using UnityEngine.UI;

public class VoiceChatIndicatorHud : MonoBehaviour
{
	public Image icon;

	private void Update()
	{
		if (!(GameManager.LocalPlayerInfo == null))
		{
			bool isTalking = GameManager.LocalPlayerInfo.VoiceChat.voiceNetworker.IsTalking;
			icon.gameObject.SetActive(isTalking);
		}
	}
}
