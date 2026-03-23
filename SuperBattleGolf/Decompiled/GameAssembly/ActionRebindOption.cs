using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Localization.Components;
using UnityEngine.Pool;
using UnityEngine.UI;

public class ActionRebindOption : MonoBehaviour
{
	[Serializable]
	public struct RebindButton
	{
		public GameObject parentObject;

		public Image bindingIcon;

		public TextMeshProUGUI tmpBinding;

		public Button button;

		public ControllerSelectable buttonControllerSelectable;

		public Button resetButton;

		public ControllerSelectable resetButtonControllerSelectable;
	}

	[SerializeField]
	private LocalizeStringEvent localizedName;

	[SerializeField]
	private RebindButton keyboardRebindButton;

	[SerializeField]
	private RebindButton gamepadRebindButton;

	private ControlsRebind rebind;

	private InputAction action;

	private int keyboardBindingIndex = -1;

	private int gamepadBindingIndex = -1;

	private string keyboardBindingPath = "empty";

	private string gamepadBindingPath = "empty";

	public void Initialize(ControlsRebind rebind, InputAction action, int keyboardBindingIndex, int gamepadBindingIndex)
	{
		this.rebind = rebind;
		this.action = action;
		this.keyboardBindingIndex = keyboardBindingIndex;
		this.gamepadBindingIndex = gamepadBindingIndex;
		SetupRebindButton(keyboardRebindButton, isGamepad: false);
		SetupRebindButton(gamepadRebindButton, isGamepad: true);
		int num = ((keyboardBindingIndex > -1) ? keyboardBindingIndex : gamepadBindingIndex);
		string bindingLocalizationId = GetBindingLocalizationId(action, num);
		localizedName.StringReference = LocalizationManager.GetLocalizedString(StringTable.UI, bindingLocalizationId);
		if (action.bindings[num].isPartOfComposite)
		{
			base.gameObject.name = $"{action.name} {num} rebind";
		}
		else
		{
			base.gameObject.name = action.name + " rebind";
		}
		UpdateInternalNavigation();
	}

	public void SetVerticalNavigationTarget(ActionRebindOption otherRebindOption, bool isUp)
	{
		bool activeSelf = keyboardRebindButton.button.gameObject.activeSelf;
		bool activeSelf2 = gamepadRebindButton.button.gameObject.activeSelf;
		bool activeSelf3 = otherRebindOption.keyboardRebindButton.button.gameObject.activeSelf;
		bool activeSelf4 = otherRebindOption.gamepadRebindButton.button.gameObject.activeSelf;
		if (activeSelf)
		{
			Navigation navigation = keyboardRebindButton.button.navigation;
			Selectable selectable = (activeSelf3 ? otherRebindOption.keyboardRebindButton.button : (activeSelf4 ? otherRebindOption.gamepadRebindButton.button : null));
			if (isUp)
			{
				navigation.selectOnUp = selectable;
			}
			else
			{
				navigation.selectOnDown = selectable;
			}
			keyboardRebindButton.button.navigation = navigation;
			navigation = keyboardRebindButton.resetButton.navigation;
			if (isUp)
			{
				navigation.selectOnUp = selectable;
			}
			else
			{
				navigation.selectOnDown = selectable;
			}
			keyboardRebindButton.resetButton.navigation = navigation;
		}
		if (activeSelf2)
		{
			Navigation navigation2 = gamepadRebindButton.button.navigation;
			Selectable selectable2 = (activeSelf4 ? otherRebindOption.gamepadRebindButton.button : (activeSelf3 ? otherRebindOption.keyboardRebindButton.button : null));
			if (isUp)
			{
				navigation2.selectOnUp = selectable2;
			}
			else
			{
				navigation2.selectOnDown = selectable2;
			}
			gamepadRebindButton.button.navigation = navigation2;
			navigation2 = gamepadRebindButton.resetButton.navigation;
			if (isUp)
			{
				navigation2.selectOnUp = selectable2;
			}
			else
			{
				navigation2.selectOnDown = selectable2;
			}
			gamepadRebindButton.resetButton.navigation = navigation2;
		}
	}

	public void SetVerticalNavigationTarget(Selectable target, bool isUp)
	{
		bool activeSelf = keyboardRebindButton.button.gameObject.activeSelf;
		bool activeSelf2 = gamepadRebindButton.button.gameObject.activeSelf;
		if (activeSelf)
		{
			Navigation navigation = keyboardRebindButton.button.navigation;
			if (isUp)
			{
				navigation.selectOnUp = target;
			}
			else
			{
				navigation.selectOnDown = target;
			}
			keyboardRebindButton.button.navigation = navigation;
			navigation = keyboardRebindButton.resetButton.navigation;
			if (isUp)
			{
				navigation.selectOnUp = target;
			}
			else
			{
				navigation.selectOnDown = target;
			}
			keyboardRebindButton.resetButton.navigation = navigation;
		}
		if (activeSelf2)
		{
			Navigation navigation2 = gamepadRebindButton.button.navigation;
			if (isUp)
			{
				navigation2.selectOnUp = target;
			}
			else
			{
				navigation2.selectOnDown = target;
			}
			gamepadRebindButton.button.navigation = navigation2;
			navigation2 = gamepadRebindButton.resetButton.navigation;
			if (isUp)
			{
				navigation2.selectOnUp = target;
			}
			else
			{
				navigation2.selectOnDown = target;
			}
			gamepadRebindButton.resetButton.navigation = navigation2;
		}
	}

	public void SetAsSelectableNavigationDownTarget(Selectable selectable, bool isUp)
	{
		bool activeSelf = keyboardRebindButton.button.gameObject.activeSelf;
		bool activeSelf2 = gamepadRebindButton.button.gameObject.activeSelf;
		Navigation navigation = selectable.navigation;
		Selectable selectable2 = (activeSelf ? keyboardRebindButton.button : (activeSelf2 ? gamepadRebindButton.button : null));
		if (isUp)
		{
			navigation.selectOnUp = selectable2;
		}
		else
		{
			navigation.selectOnDown = selectable2;
		}
		selectable.navigation = navigation;
	}

	public static string GetLocalizedBindingName(InputAction action, int bindingIndex)
	{
		string bindingLocalizationId = GetBindingLocalizationId(action, bindingIndex);
		return LocalizationManager.GetString(StringTable.UI, bindingLocalizationId);
	}

	public static string GetBindingLocalizationId(InputAction action, int bindingIndex)
	{
		string text = "SETTINGS_Keybinds_" + action.actionMap.name.RemoveWhitespace() + "_" + action.name.RemoveWhitespace();
		if (action.bindings[bindingIndex].isPartOfComposite)
		{
			text = text + "_" + action.bindings[bindingIndex].name;
		}
		return text;
	}

	private void SetupRebindButton(RebindButton rebindButton, bool isGamepad)
	{
		if ((isGamepad ? gamepadBindingIndex : keyboardBindingIndex) < 0)
		{
			rebindButton.parentObject.SetActive(value: false);
			rebindButton.button.gameObject.SetActive(value: false);
			rebindButton.resetButton.gameObject.SetActive(value: false);
			return;
		}
		rebindButton.button.onClick.AddListener(delegate
		{
			OnRebind(isGamepad);
		});
		rebindButton.resetButton.onClick.AddListener(delegate
		{
			OnResetBinding(isGamepad);
		});
		RefreshBindingDisplay(rebindButton, isGamepad);
	}

	private void RefreshBindingDisplay(RebindButton rebindButton, bool isGamepad)
	{
		string text = (isGamepad ? gamepadBindingPath : keyboardBindingPath);
		int num = (isGamepad ? gamepadBindingIndex : keyboardBindingIndex);
		if (num < 0 || text == action.bindings[num].effectivePath)
		{
			return;
		}
		text = action.bindings[num].effectivePath;
		InputManager.DeviceType deviceType = ((!InputManager.UsingGamepad) ? InputManager.DeviceType.Xbox : InputManager.CurrentDeviceType);
		Sprite inputIcon = InputManager.GetInputIcon(text, isGamepad ? deviceType : InputManager.DeviceType.KeyboardAndMouse);
		if (inputIcon != null)
		{
			rebindButton.bindingIcon.sprite = inputIcon;
		}
		else
		{
			rebindButton.tmpBinding.text = action.GetBindingDisplayString(num, InputBinding.DisplayStringOptions.DontIncludeInteractions);
		}
		rebindButton.bindingIcon.enabled = inputIcon != null;
		rebindButton.tmpBinding.enabled = inputIcon == null;
		if (isGamepad)
		{
			gamepadBindingPath = text;
		}
		else
		{
			keyboardBindingPath = text;
		}
		bool activeSelf = rebindButton.resetButton.gameObject.activeSelf;
		bool flag = action.bindings[num].hasOverrides && action.bindings[num].path != action.bindings[num].overridePath;
		if (flag != activeSelf)
		{
			rebindButton.resetButton.gameObject.SetActive(flag);
			UpdateInternalNavigation();
			if (!flag && rebindButton.resetButtonControllerSelectable.IsSelected)
			{
				GetComponentInParent<MenuNavigation>().Select(rebindButton.buttonControllerSelectable);
			}
		}
	}

	public void RefreshBindingDisplay()
	{
		RefreshBindingDisplay(keyboardRebindButton, isGamepad: false);
		RefreshBindingDisplay(gamepadRebindButton, isGamepad: true);
	}

	private void UpdateInternalNavigation()
	{
		bool activeSelf = keyboardRebindButton.button.gameObject.activeSelf;
		bool activeSelf2 = keyboardRebindButton.resetButton.gameObject.activeSelf;
		bool activeSelf3 = gamepadRebindButton.button.gameObject.activeSelf;
		bool activeSelf4 = gamepadRebindButton.resetButton.gameObject.activeSelf;
		if (activeSelf)
		{
			Navigation navigation = keyboardRebindButton.button.navigation;
			navigation.mode = Navigation.Mode.Explicit;
			navigation.selectOnLeft = null;
			navigation.selectOnRight = (activeSelf2 ? keyboardRebindButton.resetButton : (activeSelf3 ? gamepadRebindButton.button : null));
			keyboardRebindButton.button.navigation = navigation;
		}
		if (activeSelf2)
		{
			Navigation navigation2 = keyboardRebindButton.resetButton.navigation;
			navigation2.mode = Navigation.Mode.Explicit;
			navigation2.selectOnLeft = keyboardRebindButton.button;
			navigation2.selectOnRight = (activeSelf3 ? gamepadRebindButton.button : null);
			keyboardRebindButton.resetButton.navigation = navigation2;
		}
		if (activeSelf3)
		{
			Navigation navigation3 = gamepadRebindButton.button.navigation;
			navigation3.mode = Navigation.Mode.Explicit;
			navigation3.selectOnLeft = (activeSelf2 ? keyboardRebindButton.resetButton : (activeSelf ? keyboardRebindButton.button : null));
			navigation3.selectOnRight = (activeSelf4 ? gamepadRebindButton.resetButton : null);
			gamepadRebindButton.button.navigation = navigation3;
		}
		if (activeSelf4)
		{
			Navigation navigation4 = gamepadRebindButton.resetButton.navigation;
			navigation4.mode = Navigation.Mode.Explicit;
			navigation4.selectOnLeft = gamepadRebindButton.button;
			navigation4.selectOnRight = null;
			gamepadRebindButton.resetButton.navigation = navigation4;
		}
	}

	private void OnRebind(bool isGamepad)
	{
		rebind.StartRebind(action, isGamepad ? gamepadBindingIndex : keyboardBindingIndex, isGamepad, RefreshBindingDisplay);
	}

	private void OnResetBinding(bool isGamepad)
	{
		int num = (isGamepad ? gamepadBindingIndex : keyboardBindingIndex);
		InputBinding previousBinding = action.bindings[num];
		InputManager.RemoveBindingOverride(action, num);
		List<(InputAction, int)> value;
		using (CollectionPool<List<(InputAction, int)>, (InputAction, int)>.Get(out value))
		{
			if (!rebind.ValidateBinding(action, num, isGamepad, value))
			{
				rebind.HandleOverlap(action, num, value, previousBinding, RefreshBindingDisplay);
			}
			else
			{
				RefreshBindingDisplay();
			}
		}
	}
}
