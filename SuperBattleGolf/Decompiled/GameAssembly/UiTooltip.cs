using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class UiTooltip : MonoBehaviour
{
	public bool useSecondaryLabel = true;

	public TextMeshProUGUI primaryLabel;

	public GameObject primaryTooltip;

	[DisplayIf("useSecondaryLabel", true)]
	public TextMeshProUGUI secondaryLabel;

	[DisplayIf("useSecondaryLabel", true)]
	public GameObject secondaryTooltip;

	private RectTransform rectTransform;

	private CanvasGroup canvasGroup;

	private Dictionary<RectTransform, (string, string)> tooltipSources = new Dictionary<RectTransform, (string, string)>();

	private readonly Dictionary<RectTransform, (string, string)> pendingOverrides = new Dictionary<RectTransform, (string, string)>();

	private MenuNavigation parentNavigation;

	private RectTransform currentTooltipSource;

	private float showTooltipGamepadDelay = 1f;

	private float showTooltipGamepadTimer;

	private bool isFlippedVertically;

	private static Vector3[] canvasCorners = new Vector3[4];

	public event Action<bool> HoverChanged;

	public void RegisterTooltip(RectTransform rectTransform, string message, string secondaryMessage = null)
	{
		tooltipSources[rectTransform] = (message, secondaryMessage);
	}

	public void DeregisterTooltip(RectTransform rectTransform)
	{
		tooltipSources.Remove(rectTransform);
	}

	public void OverrideText(RectTransform rectTransform, string message, string secondaryMessage = null)
	{
		if (tooltipSources.ContainsKey(rectTransform))
		{
			pendingOverrides[rectTransform] = (message, secondaryMessage);
		}
	}

	private void Awake()
	{
		rectTransform = GetComponent<RectTransform>();
		canvasGroup = GetComponent<CanvasGroup>();
		parentNavigation = GetComponentInParent<MenuNavigation>();
	}

	private void OnEnable()
	{
		canvasGroup.alpha = 0f;
	}

	private void LateUpdate()
	{
		RectTransform key;
		(string, string) value;
		foreach (KeyValuePair<RectTransform, (string, string)> pendingOverride in pendingOverrides)
		{
			pendingOverride.Deconstruct(out key, out value);
			(string, string) tuple = value;
			RectTransform key2 = key;
			var (item, item2) = tuple;
			if (tooltipSources.ContainsKey(key2))
			{
				tooltipSources[key2] = (item, item2);
			}
		}
		pendingOverrides.Clear();
		float target = 0f;
		foreach (KeyValuePair<RectTransform, (string, string)> tooltipSource in tooltipSources)
		{
			tooltipSource.Deconstruct(out key, out value);
			(string, string) tuple3 = value;
			RectTransform rectTransform = key;
			var (text, text2) = tuple3;
			if (Hover(rectTransform))
			{
				bool flag = false;
				if (currentTooltipSource != rectTransform)
				{
					currentTooltipSource = rectTransform;
					showTooltipGamepadTimer = 0f;
					flag = true;
					this.HoverChanged?.Invoke(obj: true);
				}
				showTooltipGamepadTimer += Time.deltaTime;
				if (!InputManager.UsingGamepad || !(showTooltipGamepadTimer < showTooltipGamepadDelay))
				{
					target = 1f;
					primaryTooltip.gameObject.SetActive(!string.IsNullOrEmpty(text));
					primaryLabel.text = text;
					if (secondaryTooltip != null)
					{
						secondaryTooltip.gameObject.SetActive(!string.IsNullOrEmpty(text2));
						secondaryLabel.text = text2;
					}
					if (flag)
					{
						LayoutRebuilder.ForceRebuildLayoutImmediate(this.rectTransform);
						isFlippedVertically = ShouldFlipVertically(InputManager.UsingKeyboard ? Mouse.current.position.value : ((Vector2)rectTransform.position));
					}
					if (!InputManager.UsingKeyboard)
					{
						base.transform.position = GetClampedPosition(rectTransform.position);
					}
					break;
				}
			}
			else if (rectTransform == currentTooltipSource)
			{
				currentTooltipSource = null;
				this.HoverChanged?.Invoke(obj: false);
			}
		}
		canvasGroup.alpha = BMath.MoveTowards(canvasGroup.alpha, target, Time.deltaTime * 10f);
		if (InputManager.UsingKeyboard && canvasGroup.alpha > float.Epsilon)
		{
			base.transform.position = GetClampedPosition(Mouse.current.position.value);
		}
		bool Hover(RectTransform tooltip)
		{
			if (!tooltip.gameObject.activeInHierarchy)
			{
				return false;
			}
			if (parentNavigation != null && !parentNavigation.IsAtTopOfStack())
			{
				return false;
			}
			if (InputManager.UsingGamepad)
			{
				if (!IsSelectedAndVisible(tooltip) && (!tooltip.TryGetComponent<DropdownOption>(out var component) || !IsSelectedAndVisible(component.Selectable)) && (!tooltip.TryGetComponent<SliderOption>(out var component2) || !IsSelectedAndVisible(component2.Slider)))
				{
					if (tooltip.TryGetComponent<Button>(out var component3))
					{
						return IsSelectedAndVisible(component3);
					}
					return false;
				}
				return true;
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
		static bool IsSelectedAndVisible(Component component)
		{
			if (component.TryGetComponent<ControllerSelectable>(out var component2) && component2.IsSelected)
			{
				return component2.IsVisible(fullyVisibleOnly: false);
			}
			return false;
		}
		bool ShouldFlipVertically(Vector2 pos)
		{
			this.rectTransform.GetWorldCorners(canvasCorners);
			float num = canvasCorners[1].y - canvasCorners[0].y;
			pos.y = BMath.Clamp(pos.y + num, 0f, Screen.height) - num;
			return pos.y < num;
		}
	}

	private Vector2 GetClampedPosition(Vector2 pos)
	{
		rectTransform.GetWorldCorners(canvasCorners);
		float num = canvasCorners[^1].x - canvasCorners[0].x;
		float num2 = canvasCorners[1].y - canvasCorners[0].y;
		Vector2 pivot = rectTransform.pivot;
		pivot.x = 1f - pivot.x;
		num *= pivot.x;
		num *= pivot.y;
		pos.x = BMath.Clamp(pos.x + num, 0f, Screen.width) - num;
		pos.y = BMath.Clamp(pos.y + num2, 0f, Screen.height) - num2;
		if (isFlippedVertically)
		{
			pos.y += num2;
		}
		return pos;
	}
}
