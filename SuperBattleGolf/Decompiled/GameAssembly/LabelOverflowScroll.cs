using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;

public class LabelOverflowScroll : MonoBehaviour
{
	private const float START_DELAY = 0.5f;

	private const float RESET_DELAY = 2f;

	private const float DURATION = 3f;

	private const float SCROLL_PADDING = 16f;

	public TMP_Text text;

	private Vector3 textStartPos;

	private bool positionSet;

	private RectTransform rectTransform;

	private ControllerSelectable selectable;

	private bool isActive;

	private bool setTextMode;

	private double startTime;

	private float targetOffset;

	private void Awake()
	{
		rectTransform = GetComponent<RectTransform>();
		selectable = GetComponentInParent<ControllerSelectable>();
	}

	private void OnDisable()
	{
		if (isActive)
		{
			Reset();
		}
	}

	private void Update()
	{
		bool flag = isActive;
		isActive = (InputManager.UsingGamepad ? selectable.IsSelected : RectTransformUtility.RectangleContainsScreenPoint(rectTransform, Mouse.current.position.value));
		if (isActive)
		{
			if (!flag)
			{
				setTextMode = false;
				startTime = Time.timeAsDouble;
			}
			float timeSince = BMath.GetTimeSince(startTime);
			if (!(timeSince > 0.5f))
			{
				return;
			}
			if (!setTextMode)
			{
				text.overflowMode = TextOverflowModes.Overflow;
				text.alignment = TextAlignmentOptions.Left;
				float fontSize = text.fontSize;
				text.enableAutoSizing = false;
				text.fontSize = fontSize;
				targetOffset = text.preferredWidth - rectTransform.rect.width + 16f;
				setTextMode = true;
				if (!positionSet)
				{
					textStartPos = text.transform.localPosition;
					positionSet = true;
				}
			}
			timeSince -= 0.5f;
			if (timeSince < 3f)
			{
				text.transform.localPosition = textStartPos + Vector3.left * targetOffset * BMath.Clamp01(timeSince / 3f) + new Vector3(0f, -1f, 0f);
			}
			else if (timeSince > 5f)
			{
				Reset();
			}
		}
		else if (flag && !isActive)
		{
			Reset();
		}
	}

	private void Reset()
	{
		isActive = false;
		text.enableAutoSizing = true;
		if (positionSet)
		{
			text.transform.localPosition = textStartPos;
		}
		text.overflowMode = TextOverflowModes.Ellipsis;
		text.alignment = TextAlignmentOptions.Center;
		setTextMode = false;
	}
}
