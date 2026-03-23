using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class UiTooltip : MonoBehaviour
{
	public TextMeshProUGUI primaryLabel;

	public TextMeshProUGUI secondaryLabel;

	public GameObject primaryTooltip;

	public GameObject secondaryTooltip;

	private RectTransform rectTransform;

	private CanvasGroup canvasGroup;

	private Dictionary<RectTransform, (string, string)> tooltipSources = new Dictionary<RectTransform, (string, string)>();

	private static Vector3[] canvasCorners = new Vector3[4];

	public void RegisterTooltip(RectTransform rectTransform, string message, string secondaryMessage)
	{
		tooltipSources[rectTransform] = (message, secondaryMessage);
	}

	public void DeregisterTooltip(RectTransform rectTransform)
	{
		tooltipSources.Remove(rectTransform);
	}

	private void Awake()
	{
		rectTransform = GetComponent<RectTransform>();
		canvasGroup = GetComponent<CanvasGroup>();
	}

	private void OnEnable()
	{
		canvasGroup.alpha = 0f;
	}

	private void LateUpdate()
	{
		float target = 0f;
		foreach (KeyValuePair<RectTransform, (string, string)> tooltipSource in tooltipSources)
		{
			tooltipSource.Deconstruct(out var key, out var value);
			(string, string) tuple = value;
			RectTransform rectTransform = key;
			var (text, text2) = tuple;
			if (Hover(rectTransform))
			{
				target = 1f;
				primaryTooltip.gameObject.SetActive(!string.IsNullOrEmpty(text));
				primaryLabel.text = text;
				secondaryTooltip.gameObject.SetActive(!string.IsNullOrEmpty(text2));
				secondaryLabel.text = text2;
				if (!InputManager.UsingKeyboard)
				{
					base.transform.position = GetClampedPosition(rectTransform.position);
				}
				break;
			}
		}
		canvasGroup.alpha = BMath.MoveTowards(canvasGroup.alpha, target, Time.deltaTime * 10f);
		if (InputManager.UsingKeyboard && canvasGroup.alpha > float.Epsilon)
		{
			base.transform.position = GetClampedPosition(Mouse.current.position.value);
		}
		static bool Hover(RectTransform tooltip)
		{
			if (InputManager.UsingGamepad)
			{
				if (tooltip.TryGetComponent<ControllerSelectable>(out var component) && component.IsSelected)
				{
					return component.IsVisible(fullyVisibleOnly: false);
				}
				return false;
			}
			if (Mouse.current != null)
			{
				Mask componentInParent = tooltip.GetComponentInParent<Mask>();
				if (componentInParent != null && !RectTransformUtility.RectangleContainsScreenPoint(componentInParent.rectTransform, Mouse.current.position.value))
				{
					return false;
				}
				return RectTransformUtility.RectangleContainsScreenPoint(tooltip, Mouse.current.position.value);
			}
			return false;
		}
	}

	private Vector2 GetClampedPosition(Vector2 pos)
	{
		rectTransform.GetWorldCorners(canvasCorners);
		float num = canvasCorners[^1].x - canvasCorners[0].x;
		float num2 = canvasCorners[1].y - canvasCorners[0].y;
		pos.x = BMath.Clamp(pos.x + num, 0f, Screen.width) - num;
		pos.y = BMath.Clamp(pos.y + num2, 0f, Screen.height) - num2;
		return pos;
	}
}
