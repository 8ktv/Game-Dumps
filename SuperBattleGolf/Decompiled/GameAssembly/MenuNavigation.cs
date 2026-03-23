using System;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using Steamworks;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.Localization.Components;
using UnityEngine.Pool;
using UnityEngine.UI;

public class MenuNavigation : MonoBehaviour
{
	[Flags]
	private enum NavigationDirection
	{
		None = 0,
		Up = 1,
		Down = 4,
		Left = 8,
		Right = 0x10,
		Vertical = 5,
		Horizontal = 0x18
	}

	private const float autoShiftStartDelay = 0.4f;

	private const float autoShiftInterval = 0.1f;

	private const float scrollDeadzone = 0.2f;

	private const float scrollSpeed = 4f;

	private static readonly List<MenuNavigation> activeInstances = new List<MenuNavigation>();

	public bool disableComponentOnExitEvent;

	public bool disableGameObjectOnExitEvent;

	public ControllerSelectable defaultSelection;

	private ControllerSelectable currentSelection;

	private NavigationDirection previousNavigationDirection;

	private float autoShiftTimer;

	private static int lastStackUpdateFrame;

	public static bool ShouldShowVirtualKeyboard
	{
		get
		{
			if (SteamEnabler.IsSteamEnabled)
			{
				if (!SteamUtils.IsRunningOnSteamDeck)
				{
					return SteamUtils.IsSteamInBigPictureMode;
				}
				return true;
			}
			return false;
		}
	}

	public event Action OnExitEvent;

	public static void SendExitEvent()
	{
		if (activeInstances.Count == 0)
		{
			return;
		}
		List<MenuNavigation> list = activeInstances;
		MenuNavigation menuNavigation = list[list.Count - 1];
		if (menuNavigation.currentSelection != null && menuNavigation.currentSelection.ActiveMode == SelectableActiveMode.Active)
		{
			menuNavigation.currentSelection.SetActiveMode(SelectableActiveMode.Inactive);
			return;
		}
		if (menuNavigation.disableComponentOnExitEvent)
		{
			menuNavigation.enabled = false;
		}
		if (menuNavigation.disableGameObjectOnExitEvent)
		{
			menuNavigation.gameObject.SetActive(value: false);
		}
		menuNavigation.OnExitEvent?.Invoke();
	}

	private void OnEnable()
	{
		activeInstances.Add(this);
		Reselect();
		SteamUtils.OnGamepadTextInputDismissed += OnSteamVirtualKeyboardDismissed;
		lastStackUpdateFrame = Time.frameCount;
	}

	private void OnDisable()
	{
		activeInstances.Remove(this);
		if (currentSelection != null)
		{
			currentSelection.Deselect();
		}
		currentSelection = null;
		if (activeInstances.Count > 0)
		{
			List<MenuNavigation> list = activeInstances;
			list[list.Count - 1].Reselect();
		}
		SteamUtils.OnGamepadTextInputDismissed -= OnSteamVirtualKeyboardDismissed;
		lastStackUpdateFrame = Time.frameCount;
	}

	public void AssertFocus()
	{
		if (!base.isActiveAndEnabled)
		{
			Debug.LogWarning("MenuNavigation component isn't active or gameobject is disabled, can't assert focus!");
			return;
		}
		activeInstances.Remove(this);
		activeInstances.Add(this);
		Reselect();
	}

	private void OnSteamVirtualKeyboardDismissed(bool success)
	{
		if (success && currentSelection.IsInputField && currentSelection.AsInputField.IsInteractable())
		{
			currentSelection.AsInputField.text = SteamUtils.GetEnteredGamepadText();
			currentSelection.Submit();
		}
	}

	public async void Reselect()
	{
		if (!InputManager.UsingGamepad)
		{
			return;
		}
		await UniTask.Yield();
		if (this == null || !IsAtTopOfStack())
		{
			return;
		}
		List<ControllerSelectable> value;
		using (CollectionPool<List<ControllerSelectable>, ControllerSelectable>.Get(out value))
		{
			GetComponentsInChildren(includeInactive: false, value);
			ControllerSelectable controllerSelectable = null;
			if (currentSelection != null && currentSelection.isActiveAndEnabled)
			{
				controllerSelectable = currentSelection;
			}
			else if (defaultSelection != null)
			{
				controllerSelectable = defaultSelection;
			}
			else if (value.Count > 0)
			{
				controllerSelectable = ((!base.gameObject.TryGetComponentInParent<TMP_Dropdown>(out var foundComponent, includeInactive: false) || foundComponent.value >= value.Count) ? value[0] : value[foundComponent.value]);
			}
			for (int i = 0; i < value.Count; i++)
			{
				if (!(value[i] == controllerSelectable))
				{
					value[i].Deselect();
				}
			}
			if (controllerSelectable != null && controllerSelectable.IsInteractableSelf)
			{
				Select(controllerSelectable);
			}
			previousNavigationDirection = (NavigationDirection)(-1);
		}
	}

	public bool IsAtTopOfStack()
	{
		if (activeInstances.Count > 0)
		{
			List<MenuNavigation> list = activeInstances;
			return list[list.Count - 1] == this;
		}
		return false;
	}

	public bool CanUpdate()
	{
		if (Time.frameCount == lastStackUpdateFrame)
		{
			return false;
		}
		if (!IsAtTopOfStack())
		{
			return false;
		}
		if (!InputManager.UsingGamepad)
		{
			return false;
		}
		if (InputManager.CurrentGamepad == null)
		{
			return false;
		}
		if (InputManager.CurrentModeMask.HasMode(InputMode.ForceDisabled))
		{
			return false;
		}
		if (ColorOverlay.IsActive)
		{
			return false;
		}
		if (LoadingScreen.IsVisible)
		{
			return false;
		}
		return true;
	}

	private void Update()
	{
		NavigationDirection navigationPressedThisFrame;
		if (CanUpdate())
		{
			if (currentSelection != null && !currentSelection.IsEffectivelyInteractable())
			{
				currentSelection.SetActiveMode(SelectableActiveMode.Inactive);
			}
			ParseNavigationInput(out navigationPressedThisFrame);
			ApplyInputToCurrentSelection(out var eatNavigationInput);
			if (!eatNavigationInput)
			{
				ApplyNavigation();
			}
		}
		void ApplyInputToCurrentSelection(out bool reference)
		{
			reference = false;
			if (!(currentSelection == null))
			{
				if (currentSelection.TryGetComponentInParent<ScrollRect>(out var foundComponent, includeInactive: false))
				{
					Vector2 value = InputManager.CurrentGamepad.rightStick.value;
					Vector2 vector = new Vector2(foundComponent.horizontal ? 1 : 0, foundComponent.vertical ? 1 : 0);
					if (!value.Approximately(Vector2.zero, 0.2f))
					{
						foundComponent.content.anchoredPosition -= value * 4f * vector;
					}
				}
				if (currentSelection.IsSlider && currentSelection.IsVisible(fullyVisibleOnly: false))
				{
					Navigation navigation = currentSelection.AsSlider.navigation;
					if (!currentSelection.IsEffectivelyInteractable())
					{
						currentSelection.SetActiveMode(SelectableActiveMode.Inactive);
					}
					else if (!(navigation.selectOnLeft != null) && !(navigation.selectOnRight != null))
					{
						currentSelection.SetActiveMode(SelectableActiveMode.Auto);
					}
					else if (InputManager.CurrentGamepad.aButton.wasPressedThisFrame || (currentSelection.IsActiveAtAll && InputManager.CurrentGamepad.bButton.wasPressedThisFrame))
					{
						currentSelection.SetActiveMode((!currentSelection.IsActiveAtAll) ? SelectableActiveMode.Active : SelectableActiveMode.Inactive);
					}
					if (currentSelection.IsActiveAtAll)
					{
						NavigationDirection navigationDirection = navigationPressedThisFrame & NavigationDirection.Horizontal;
						if (navigationDirection != NavigationDirection.None)
						{
							float num = (currentSelection.AsSlider.maxValue - currentSelection.AsSlider.minValue) * 0.05f;
							float value2 = currentSelection.AsSlider.value;
							float num2 = num * (float)((navigationDirection != NavigationDirection.Left) ? 1 : (-1));
							float num3 = value2;
							do
							{
								num3 = BMath.RoundToMultipleOf(num3 + num2, num);
								currentSelection.AsSlider.value = num3;
							}
							while (num3 > currentSelection.AsSlider.minValue + float.Epsilon && num3 < currentSelection.AsSlider.maxValue - float.Epsilon && Mathf.Approximately(currentSelection.AsSlider.value, value2));
							reference = true;
						}
						return;
					}
				}
				if (InputManager.CurrentGamepad.aButton.wasPressedThisFrame)
				{
					SubmitCurrentSelection();
				}
				if (InputManager.CurrentGamepad.bButton.wasPressedThisFrame)
				{
					currentSelection.Cancel();
				}
			}
		}
		void ApplyNavigation()
		{
			if (navigationPressedThisFrame == NavigationDirection.None)
			{
				return;
			}
			List<ControllerSelectable> selectables;
			using (CollectionPool<List<ControllerSelectable>, ControllerSelectable>.Get(out selectables))
			{
				GetComponentsInChildren(includeInactive: false, selectables);
				if (selectables.Count > 0)
				{
					ControllerSelectable targetSelectable;
					if (currentSelection == null || !selectables.Contains(currentSelection))
					{
						targetSelectable = selectables[0];
					}
					else
					{
						Vector2 navigationVector = NavigationDirectionToVector(navigationPressedThisFrame);
						if (!TryGetTargetSelectableInDirection(currentSelection, navigationVector, out targetSelectable))
						{
							targetSelectable = null;
						}
					}
					if (targetSelectable != null && targetSelectable.IsInteractableSelf && selectables.Contains(targetSelectable))
					{
						Select(targetSelectable);
					}
				}
			}
			bool TryGetTargetSelectableInDirection(ControllerSelectable currentSelectable, Vector2 navigationVector2, out ControllerSelectable reference)
			{
				reference = currentSelectable;
				bool flag = !currentSelectable.IsVisible(fullyVisibleOnly: false);
				if (flag && TryGetFirstVisible(navigationVector2, out reference))
				{
					return true;
				}
				while (TryGetNextTargetSelectableInDirection(reference, navigationVector2, out reference))
				{
					if (reference.IsInteractableSelf && (!flag || reference.IsVisible(fullyVisibleOnly: false)))
					{
						return true;
					}
				}
				return false;
			}
		}
		static NavigationDirection GetCurrentNavigation()
		{
			NavigationDirection navigationDirection = NavigationDirection.None;
			Gamepad currentGamepad = InputManager.CurrentGamepad;
			if (currentGamepad == null)
			{
				return navigationDirection;
			}
			if (currentGamepad.leftStick.up.isPressed || currentGamepad.dpad.up.isPressed)
			{
				navigationDirection |= NavigationDirection.Up;
			}
			else if (currentGamepad.leftStick.down.isPressed || currentGamepad.dpad.down.isPressed)
			{
				navigationDirection |= NavigationDirection.Down;
			}
			if (currentGamepad.leftStick.left.isPressed || currentGamepad.dpad.left.isPressed)
			{
				navigationDirection |= NavigationDirection.Left;
			}
			else if (currentGamepad.leftStick.right.isPressed || currentGamepad.dpad.right.isPressed)
			{
				navigationDirection |= NavigationDirection.Right;
			}
			return navigationDirection;
		}
		NavigationDirection ParseNavigationInput(out NavigationDirection reference)
		{
			NavigationDirection navigationDirection = GetCurrentNavigation();
			reference = navigationDirection & ~previousNavigationDirection;
			if (reference != NavigationDirection.None)
			{
				autoShiftTimer = 0.4f;
			}
			if (navigationDirection != NavigationDirection.None)
			{
				if (autoShiftTimer > 0f)
				{
					autoShiftTimer -= Time.deltaTime;
				}
				else
				{
					reference = navigationDirection;
					autoShiftTimer = 0.1f;
				}
			}
			previousNavigationDirection = navigationDirection;
			return navigationDirection;
		}
		bool TryGetFirstVisible(Vector2 navigationVector, out ControllerSelectable targetSelectable)
		{
			bool flag = BMath.Abs(navigationVector.y) > BMath.Abs(navigationVector.x);
			targetSelectable = null;
			foreach (ControllerSelectable item in P_2.selectables)
			{
				if (item.IsInteractableSelf && item.IsVisible(fullyVisibleOnly: true))
				{
					if (targetSelectable == null)
					{
						targetSelectable = item;
					}
					else
					{
						if (flag)
						{
							if (item.transform.position.y.Approximately(targetSelectable.transform.position.y, 1f))
							{
								if (item.transform.position.x >= targetSelectable.transform.position.x)
								{
									continue;
								}
							}
							else if (navigationVector.y <= 0f)
							{
								if (item.transform.position.y <= targetSelectable.transform.position.y)
								{
									continue;
								}
							}
							else if (item.transform.position.y >= targetSelectable.transform.position.y)
							{
								continue;
							}
						}
						else if (item.transform.position.x.Approximately(targetSelectable.transform.position.x, 1f))
						{
							if (item.transform.position.x <= targetSelectable.transform.position.x)
							{
								continue;
							}
						}
						else if (navigationVector.x <= 0f)
						{
							if (item.transform.position.x <= targetSelectable.transform.position.x)
							{
								continue;
							}
						}
						else if (item.transform.position.x >= targetSelectable.transform.position.x)
						{
							continue;
						}
						targetSelectable = item;
					}
				}
			}
			return targetSelectable != null;
		}
		bool TryGetNextTargetSelectableInDirection(ControllerSelectable currentSelectable, Vector2 navigationVector, out ControllerSelectable targetSelectable)
		{
			targetSelectable = currentSelectable.FindSelectable(navigationVector);
			if (targetSelectable == null)
			{
				return false;
			}
			if (!P_3.selectables.Contains(targetSelectable))
			{
				return false;
			}
			return true;
		}
	}

	private Vector2 NavigationDirectionToVector(NavigationDirection direction)
	{
		Vector2 zero = Vector2.zero;
		if ((direction & NavigationDirection.Up) != NavigationDirection.None)
		{
			zero.y = 1f;
		}
		else if ((direction & NavigationDirection.Down) != NavigationDirection.None)
		{
			zero.y = -1f;
		}
		if ((direction & NavigationDirection.Right) != NavigationDirection.None)
		{
			zero.x = 1f;
		}
		else if ((direction & NavigationDirection.Left) != NavigationDirection.None)
		{
			zero.x = -1f;
		}
		return zero;
	}

	public void SelectAndSubmit(ControllerSelectable selectable)
	{
		Select(selectable);
		SubmitCurrentSelection();
	}

	public void Select(ControllerSelectable selectable)
	{
		EventSystem.current.SetSelectedGameObject(null);
		if (currentSelection != null)
		{
			currentSelection.Deselect();
		}
		if (!(selectable == null))
		{
			currentSelection = selectable;
			currentSelection.Select();
		}
	}

	private void SubmitCurrentSelection()
	{
		if (currentSelection.IsInputField && currentSelection.AsInputField.IsInteractable())
		{
			Debug.Log("Show virtual keyboard if applicable");
			if (ShouldShowVirtualKeyboard)
			{
				SteamUtils.ShowGamepadTextInput((currentSelection.AsInputField.inputType == TMP_InputField.InputType.Password) ? GamepadTextInputMode.Password : GamepadTextInputMode.Normal, currentSelection.AsInputField.multiLine ? GamepadTextInputLineMode.MultipleLines : GamepadTextInputLineMode.SingleLine, currentSelection.AsInputField.TryGetComponentInChildren<LocalizeStringEvent>(out var foundComponent, includeInactive: true) ? foundComponent.StringReference.GetLocalizedString() : string.Empty, (currentSelection.AsInputField.characterLimit <= 0) ? 1024 : currentSelection.AsInputField.characterLimit, currentSelection.AsInputField.text);
			}
		}
		else
		{
			currentSelection.Submit();
		}
	}
}
