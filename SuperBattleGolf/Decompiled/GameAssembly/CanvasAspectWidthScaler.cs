using UnityEngine;
using UnityEngine.UI;

public class CanvasAspectWidthScaler : MonoBehaviour
{
	public float minAspectScaleHeight;

	public float belowAspectWidthValue;

	private int prevWidth;

	private int prevHeight;

	private void Update()
	{
		if (prevWidth != Screen.width || prevHeight != Screen.height)
		{
			prevWidth = Screen.width;
			prevHeight = Screen.height;
			float num = (float)Screen.width / (float)Screen.height;
			GetComponent<CanvasScaler>().matchWidthOrHeight = ((num < minAspectScaleHeight - 0.01f) ? belowAspectWidthValue : 1f);
		}
	}
}
