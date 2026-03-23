using TMPro;
using UnityEngine;

public class TextChatMessageUi : MonoBehaviour, IBUpdateCallback, IAnyBUpdateCallback
{
	public GameObject stripes;

	public float timeout = -1f;

	public TMP_Text messageText;

	public CanvasGroup canvasGroup;

	private float initTime;

	private bool callbackRegistered;

	private void OnDestroy()
	{
		if (callbackRegistered)
		{
			BUpdate.DeregisterCallback(this);
		}
	}

	public void Initialize(string message)
	{
		messageText.text = message;
		initTime = Time.time;
		if (!callbackRegistered)
		{
			BUpdate.RegisterCallback(this);
			callbackRegistered = true;
		}
		UpdateAlpha();
	}

	public void OnBUpdate()
	{
		UpdateAlpha();
	}

	private void UpdateAlpha()
	{
		float num = Time.time - initTime;
		if (num > InfoFeed.MessageSlideInDuration)
		{
			if (timeout > 0f)
			{
				if (num > timeout + InfoFeed.MessageFadeOutDuration)
				{
					base.gameObject.SetActive(value: false);
				}
				else
				{
					canvasGroup.alpha = BMath.Remap(0f, InfoFeed.MessageFadeOutDuration, 1f, 0f, num - timeout);
				}
			}
			else if (callbackRegistered)
			{
				BUpdate.DeregisterCallback(this);
				callbackRegistered = false;
			}
		}
		else
		{
			canvasGroup.alpha = BMath.Remap(0f, InfoFeed.MessageSlideInDuration, 0f, 1f, num);
		}
	}
}
