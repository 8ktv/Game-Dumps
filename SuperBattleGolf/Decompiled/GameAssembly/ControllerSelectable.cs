using System;
using FMODUnity;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class ControllerSelectable : MonoBehaviour
{
	[SerializeField]
	private CanvasGroup highlight;

	[SerializeField]
	private CanvasGroup activeHighlight;

	[SerializeField]
	[HideInInspector]
	private Selectable selectable;

	[SerializeField]
	[HideInInspector]
	private Color highlightColor;

	private RectTransform rectTransform;

	private static readonly Vector3[] rectTransformCornerBuffer = new Vector3[4];

	public bool IsInteractableSelf
	{
		get
		{
			return selectable.interactable;
		}
		set
		{
			selectable.interactable = value;
		}
	}

	public Slider AsSlider { get; private set; }

	public TMP_Dropdown AsDropdown { get; private set; }

	public TMP_InputField AsInputField { get; private set; }

	public bool IsSelected { get; private set; }

	public SelectableActiveMode ActiveMode { get; private set; }

	public bool IsActiveAtAll => ActiveMode != SelectableActiveMode.Inactive;

	public Selectable Selectable => selectable;

	public bool IsSlider => AsSlider != null;

	public bool IsDropdown => AsDropdown != null;

	public bool IsInputField => AsInputField != null;

	public event Action Selected;

	public event Action Deselected;

	public event Action IsSelectedChanged;

	public event Action Submitted;

	private void Awake()
	{
		rectTransform = base.transform as RectTransform;
		AsSlider = GetComponentInParent<Slider>(includeInactive: true);
		AsDropdown = GetComponentInParent<TMP_Dropdown>(includeInactive: true);
		AsInputField = GetComponentInParent<TMP_InputField>(includeInactive: true);
	}

	private void OnEnable()
	{
		UpdateSelection();
		UpdateActiveHighlight();
		InputManager.SwitchedInputDeviceType += OnSwitchedInputDeviceType;
	}

	private void OnDisable()
	{
		InputManager.SwitchedInputDeviceType -= OnSwitchedInputDeviceType;
	}

	private void OnValidate()
	{
		selectable = GetComponent<Selectable>();
		highlightColor = selectable.colors.highlightedColor;
	}

	public bool IsEffectivelyInteractable()
	{
		return selectable.IsInteractable();
	}

	public void Select()
	{
		bool isSelected = IsSelected;
		IsSelected = true;
		UpdateSelection();
		this.Selected?.Invoke();
		if (!isSelected)
		{
			this.IsSelectedChanged?.Invoke();
		}
	}

	public void Deselect()
	{
		bool isSelected = IsSelected;
		SelectableActiveMode activeMode = ActiveMode;
		IsSelected = false;
		ActiveMode = SelectableActiveMode.Inactive;
		UpdateSelection();
		UpdateActiveHighlight();
		if (IsSlider && activeMode == SelectableActiveMode.Active)
		{
			RuntimeManager.PlayOneShot(GameManager.AudioSettings.SliderControllerDeactivation);
		}
		this.Deselected?.Invoke();
		if (isSelected)
		{
			this.IsSelectedChanged?.Invoke();
		}
	}

	public void SetActiveMode(SelectableActiveMode mode)
	{
		SelectableActiveMode activeMode = ActiveMode;
		ActiveMode = mode;
		UpdateActiveHighlight();
		if (IsSlider)
		{
			if (activeMode == SelectableActiveMode.Inactive && ActiveMode == SelectableActiveMode.Active)
			{
				RuntimeManager.PlayOneShot(GameManager.AudioSettings.SliderControllerActivation);
			}
			else if (activeMode == SelectableActiveMode.Active && ActiveMode == SelectableActiveMode.Inactive)
			{
				RuntimeManager.PlayOneShot(GameManager.AudioSettings.SliderControllerDeactivation);
			}
		}
	}

	public void Submit()
	{
		ExecuteEvents.Execute(base.gameObject, new BaseEventData(EventSystem.current), ExecuteEvents.submitHandler);
		this.Submitted?.Invoke();
	}

	public void Cancel()
	{
		if (base.gameObject.TryGetComponentInParent<ICancelHandler>(out var foundComponent, includeInactive: false))
		{
			foundComponent.OnCancel(new BaseEventData(EventSystem.current));
		}
	}

	public ControllerSelectable FindSelectable(Vector3 direction)
	{
		Selectable selectable = null;
		if (this.selectable.navigation.mode == Navigation.Mode.Automatic)
		{
			selectable = this.selectable.FindSelectable(direction);
		}
		else if (this.selectable.navigation.mode == Navigation.Mode.Explicit)
		{
			if (BMath.Abs(direction.x) > BMath.Abs(direction.y))
			{
				if (direction.x > 0f)
				{
					selectable = this.selectable.navigation.selectOnRight;
				}
				else if (direction.x < 0f)
				{
					selectable = this.selectable.navigation.selectOnLeft;
				}
			}
			else if (direction.y > 0f)
			{
				selectable = this.selectable.navigation.selectOnUp;
			}
			else if (direction.y < 0f)
			{
				selectable = this.selectable.navigation.selectOnDown;
			}
		}
		if (selectable == null)
		{
			return null;
		}
		if (selectable == this.selectable)
		{
			Debug.LogError($"{this.selectable} navigates to itself navigation direction {direction}");
			return null;
		}
		return selectable.GetComponent<ControllerSelectable>();
	}

	private void UpdateSelection()
	{
		ColorBlock colors = selectable.colors;
		colors.highlightedColor = (InputManager.UsingGamepad ? Color.clear : highlightColor);
		selectable.colors = colors;
		if (highlight != null)
		{
			highlight.alpha = ((InputManager.UsingGamepad && IsSelected) ? 1f : 0f);
		}
		if (IsSelected && InputManager.UsingGamepad)
		{
			if (selectable is Slider)
			{
				selectable.Select();
			}
			if (base.gameObject.TryGetComponentInParent<ScrollRect>(out var foundComponent, includeInactive: false))
			{
				foundComponent.EnsureVisibility(rectTransform, GameManager.UiSettings.ScrollRectControllerReselectDefaultPadding);
			}
		}
	}

	private void UpdateActiveHighlight()
	{
		if (activeHighlight != null)
		{
			activeHighlight.alpha = ((InputManager.UsingGamepad && ActiveMode != SelectableActiveMode.Inactive) ? 1f : 0f);
		}
	}

	private void OnSwitchedInputDeviceType()
	{
		UpdateSelection();
		UpdateActiveHighlight();
	}

	public bool IsVisible(bool fullyVisibleOnly)
	{
		if (!this.TryGetComponentInParent<ScrollRect>(out var foundComponent, includeInactive: false))
		{
			return true;
		}
		Rect other = GetWorldRect((selectable.targetGraphic != null) ? selectable.targetGraphic.rectTransform : rectTransform);
		Rect rect = GetWorldRect(foundComponent.viewport);
		if (fullyVisibleOnly)
		{
			return rect.Contains(other);
		}
		return other.Overlaps(rect);
		static Rect GetWorldRect(RectTransform rectTransform)
		{
			rectTransform.GetWorldCorners(rectTransformCornerBuffer);
			Vector2 position = rectTransformCornerBuffer[0];
			Vector2 size = new Vector2(rectTransformCornerBuffer[2].x - rectTransformCornerBuffer[0].x, rectTransformCornerBuffer[2].y - rectTransformCornerBuffer[0].y);
			return new Rect(position, size);
		}
	}
}
