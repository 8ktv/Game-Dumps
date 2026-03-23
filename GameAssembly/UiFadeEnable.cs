using UnityEngine;

public class UiFadeEnable : MonoBehaviour
{
	public float fadeSpeed = 10f;

	private CanvasGroup canvasGroup;

	private float targetAlpha;

	private void Awake()
	{
		canvasGroup = GetComponent<CanvasGroup>();
	}

	private void OnDisable()
	{
		canvasGroup.alpha = 0f;
	}

	private void Update()
	{
		canvasGroup.alpha = BMath.Lerp(canvasGroup.alpha, targetAlpha, Time.deltaTime * fadeSpeed);
		if (targetAlpha < float.Epsilon && canvasGroup.alpha < float.Epsilon)
		{
			base.gameObject.SetActive(value: false);
		}
	}

	public void SetActive(bool active)
	{
		float num = targetAlpha;
		targetAlpha = (active ? 1 : 0);
		if (num != targetAlpha)
		{
			base.gameObject.SetActive(value: true);
		}
	}
}
