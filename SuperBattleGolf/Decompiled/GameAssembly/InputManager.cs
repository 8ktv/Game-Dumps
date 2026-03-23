using System;
using System.Collections.Generic;
using System.Linq;
using Steamworks;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.DualShock;
using UnityEngine.InputSystem.Switch;
using UnityEngine.InputSystem.UI;
using UnityEngine.InputSystem.XInput;

public static class InputManager
{
	public enum DeviceType
	{
		KeyboardAndMouse,
		Xbox,
		Steam,
		PS5,
		SwitchPro
	}

	private static GameControls controls;

	private static InputIconsSets iconSets;

	private static InputIconsSet currentDeviceIconSet;

	private static int currentDeviceId = -1;

	private static bool cursorForceLocked;

	private static readonly List<string> bindingPathBuffer = new List<string>();

	private static bool isInitialized;

	public static readonly string GamepadGroup = "Gamepad";

	public static readonly string KeyboardGroup = "Keyboard Mouse";

	public static GameControls Controls => controls;

	public static DeviceType CurrentDeviceType { get; private set; }

	public static InputMode CurrentMode { get; private set; }

	public static InputMode CurrentModeMask { get; private set; }

	public static Gamepad CurrentGamepad { get; private set; }

	public static bool UsingKeyboard => CurrentDeviceType == DeviceType.KeyboardAndMouse;

	public static bool UsingGamepad => CurrentDeviceType != DeviceType.KeyboardAndMouse;

	public static bool IsRadialMenuInputEnabled
	{
		get
		{
			if (controls != null)
			{
				return controls.RadialMenu.enabled;
			}
			return false;
		}
	}

	public static event Action SwitchedToGamepad;

	public static event Action SwitchedToKeyboard;

	public static event Action SwitchedInputDeviceType;

	public static event Action UpdatedKeyBindings;

	public static void Initialize()
	{
		if (!isInitialized)
		{
			if (DevConsoleGui.Active)
			{
				EnableMode(InputMode.DevConsole);
			}
			if (GameManager.IsSteamOverlayActive)
			{
				EnableMode(InputMode.SteamOverlay);
			}
			iconSets = Resources.Load<InputIconsSets>("InputIconsSets");
			RefreshCurrentDeviceType(Gamepad.current);
			currentDeviceIconSet = iconSets.GetIconSet(CurrentDeviceType);
			controls = new GameControls();
			InputSystem.onActionChange += OnActionChange;
			InputSystem.onDeviceChange += OnDeviceChange;
			DevConsoleGui.Activated += OnDevConsoleActivated;
			DevConsoleGui.Deactivated += OnDevConsoleDeactivated;
			GameManager.IsSteamOverlayActiveChanged += OnIsSteamOverlayActiveChanged;
			Application.quitting += OnApplicationQuit;
			isInitialized = true;
			if (SteamEnabler.IsSteamEnabled && !SteamClient.IsValid)
			{
				SteamManager.OnInitialized += RefreshOnSteamInit;
			}
		}
		static void RefreshOnSteamInit()
		{
			SteamManager.OnInitialized -= RefreshOnSteamInit;
			if (!SteamUtils.IsRunningOnSteamDeck)
			{
				return;
			}
			try
			{
				RefreshCurrentDeviceType(Gamepad.current, force: true);
			}
			catch (Exception exception)
			{
				Debug.LogError("Encountered exception force refreshing input after Steam init!");
				Debug.LogException(exception);
			}
		}
	}

	public static void EnableMode(InputMode mode)
	{
		if (EnableModeInternal(mode))
		{
			ApplyCurrentMode();
		}
	}

	public static void DisableMode(InputMode mode)
	{
		if (DisableModeInternal(mode))
		{
			ApplyCurrentMode();
		}
	}

	public static void SetIsRadialMenuInputEnabled(bool enabled)
	{
		if (enabled)
		{
			controls.RadialMenu.Enable();
		}
		else
		{
			controls.RadialMenu.Disable();
		}
		UpdateCursorLock();
	}

	public static void SetCursorForceLocked(bool locked)
	{
		cursorForceLocked = locked;
		UpdateCursorLock();
	}

	private static bool EnableModeInternal(InputMode mode)
	{
		if (CurrentModeMask.HasMode(mode))
		{
			return false;
		}
		InputMode currentMode = CurrentMode;
		CurrentModeMask |= mode;
		UpdateCurrentMode();
		return currentMode != CurrentMode;
	}

	private static bool DisableModeInternal(InputMode mode)
	{
		if (!CurrentModeMask.HasMode(mode))
		{
			return false;
		}
		InputMode currentMode = CurrentMode;
		CurrentModeMask &= ~mode;
		UpdateCurrentMode();
		return currentMode != CurrentMode;
	}

	private static void UpdateCurrentMode()
	{
		if (CurrentModeMask.HasMode(InputMode.DevConsole))
		{
			CurrentMode = InputMode.DevConsole;
		}
		else if (CurrentModeMask.HasMode(InputMode.ForceDisabled))
		{
			CurrentMode = InputMode.ForceDisabled;
		}
		else if (CurrentModeMask.HasMode(InputMode.SteamOverlay))
		{
			CurrentMode = InputMode.SteamOverlay;
		}
		else if (CurrentModeMask.HasMode(InputMode.MainMenu))
		{
			CurrentMode = InputMode.MainMenu;
		}
		else if (CurrentModeMask.HasMode(InputMode.MatchSetup))
		{
			CurrentMode = InputMode.MatchSetup;
		}
		else if (CurrentModeMask.HasMode(InputMode.Paused))
		{
			CurrentMode = InputMode.Paused;
		}
		else if (CurrentModeMask.HasMode(InputMode.TextChat))
		{
			CurrentMode = InputMode.TextChat;
		}
		else if (CurrentModeMask.HasMode(InputMode.Spectate))
		{
			CurrentMode = InputMode.Spectate;
		}
		else if (CurrentModeMask.HasMode(InputMode.GolfCartDriver))
		{
			CurrentMode = InputMode.GolfCartDriver;
		}
		else if (CurrentModeMask.HasMode(InputMode.GolfCartPassenger))
		{
			CurrentMode = InputMode.GolfCartPassenger;
		}
		else if (CurrentModeMask.HasMode(InputMode.Regular))
		{
			CurrentMode = InputMode.Regular;
		}
		else
		{
			CurrentMode = InputMode.None;
		}
	}

	public static void GetOverlappingActionMaps(InputActionMap actionMap, List<InputActionMap> overlaps)
	{
		overlaps.Add(actionMap);
		if (actionMap.name == "Gameplay")
		{
			overlaps.Add(controls.Hotkeys);
			overlaps.Add(controls.Ingame);
			overlaps.Add(controls.Camera);
		}
		else if (actionMap.name == "Golf Cart Driver")
		{
			overlaps.Add(controls.GolfCartShared);
			overlaps.Add(controls.Hotkeys);
			overlaps.Add(controls.Ingame);
			overlaps.Add(controls.Camera);
		}
		else if (actionMap.name == "Golf Cart Shared")
		{
			overlaps.Add(controls.GolfCartDriver);
			overlaps.Add(controls.Hotkeys);
			overlaps.Add(controls.Ingame);
			overlaps.Add(controls.Camera);
		}
		else if (actionMap.name == "Hotkeys")
		{
			overlaps.Add(controls.Gameplay);
			overlaps.Add(controls.Ingame);
			overlaps.Add(controls.Camera);
			overlaps.Add(controls.GolfCartDriver);
			overlaps.Add(controls.GolfCartShared);
		}
		else if (actionMap.name == "Spectate")
		{
			overlaps.Add(controls.Ingame);
			overlaps.Add(controls.Camera);
		}
		else if (actionMap.name == "Ingame")
		{
			overlaps.Add(controls.Hotkeys);
			overlaps.Add(controls.Camera);
			overlaps.Add(controls.GolfCartShared);
			overlaps.Add(controls.GolfCartDriver);
			overlaps.Add(controls.Gameplay);
		}
	}

	private static void ApplyCurrentMode()
	{
		UpdateCursorLock();
		switch (CurrentMode)
		{
		case InputMode.Regular:
			controls.Gameplay.Enable();
			controls.GolfCartDriver.Disable();
			controls.GolfCartShared.Disable();
			controls.Hotkeys.Enable();
			controls.Spectate.Disable();
			controls.Ingame.Enable();
			controls.VoiceChat.Enable();
			controls.Camera.Enable();
			controls.UI.Disable();
			break;
		case InputMode.GolfCartDriver:
			controls.Gameplay.Disable();
			controls.GolfCartDriver.Enable();
			controls.GolfCartShared.Enable();
			controls.Hotkeys.Enable();
			controls.Spectate.Disable();
			controls.Ingame.Enable();
			controls.VoiceChat.Enable();
			controls.Camera.Enable();
			controls.UI.Disable();
			break;
		case InputMode.GolfCartPassenger:
			controls.Gameplay.Disable();
			controls.GolfCartDriver.Disable();
			controls.GolfCartShared.Enable();
			controls.Hotkeys.Enable();
			controls.Spectate.Disable();
			controls.Ingame.Enable();
			controls.VoiceChat.Enable();
			controls.Camera.Enable();
			controls.UI.Disable();
			break;
		case InputMode.Spectate:
			controls.Gameplay.Disable();
			controls.GolfCartDriver.Disable();
			controls.GolfCartShared.Disable();
			controls.Hotkeys.Disable();
			controls.Spectate.Enable();
			controls.Ingame.Enable();
			controls.VoiceChat.Enable();
			controls.Camera.Enable();
			controls.UI.Disable();
			break;
		case InputMode.Paused:
		case InputMode.MatchSetup:
			controls.Gameplay.Disable();
			controls.GolfCartDriver.Disable();
			controls.GolfCartShared.Disable();
			controls.Hotkeys.Disable();
			controls.Spectate.Disable();
			controls.Ingame.Disable();
			controls.VoiceChat.Enable();
			controls.Camera.Disable();
			controls.UI.Enable();
			break;
		case InputMode.MainMenu:
			controls.Gameplay.Disable();
			controls.GolfCartDriver.Disable();
			controls.GolfCartShared.Disable();
			controls.Hotkeys.Disable();
			controls.Spectate.Disable();
			controls.Ingame.Disable();
			controls.VoiceChat.Disable();
			controls.Camera.Disable();
			controls.UI.Enable();
			break;
		case InputMode.TextChat:
			controls.Gameplay.Disable();
			controls.GolfCartDriver.Disable();
			controls.GolfCartShared.Disable();
			controls.Hotkeys.Disable();
			controls.Spectate.Disable();
			controls.Ingame.Disable();
			controls.VoiceChat.Disable();
			controls.Camera.Disable();
			controls.UI.Enable();
			break;
		case InputMode.None:
		case InputMode.DevConsole:
		case InputMode.SteamOverlay:
		case InputMode.ForceDisabled:
			controls.Gameplay.Disable();
			controls.GolfCartDriver.Disable();
			controls.GolfCartShared.Disable();
			controls.Hotkeys.Disable();
			controls.Spectate.Disable();
			controls.Ingame.Disable();
			controls.VoiceChat.Disable();
			controls.Camera.Disable();
			controls.UI.Disable();
			break;
		}
		if (EventSystem.current != null && EventSystem.current.TryGetComponent<InputSystemUIInputModule>(out var component))
		{
			component.enabled = !CurrentMode.DisablesUiInputModule();
		}
	}

	private static void UpdateCursorLock()
	{
		CursorManager.SetCursorLock(ShouldCursorBeLocked());
		static bool ShouldCursorBeLocked()
		{
			if (cursorForceLocked)
			{
				return true;
			}
			if (IsRadialMenuInputEnabled)
			{
				return false;
			}
			if (CurrentMode == InputMode.Regular)
			{
				return true;
			}
			if (CurrentMode == InputMode.GolfCartDriver)
			{
				return true;
			}
			if (CurrentMode == InputMode.GolfCartPassenger)
			{
				return true;
			}
			if (CurrentMode == InputMode.Spectate)
			{
				return true;
			}
			return false;
		}
	}

	private static void RefreshCurrentDeviceType(InputDevice device, bool force = false)
	{
		int num = device?.deviceId ?? (-1);
		if (!force && num == currentDeviceId)
		{
			return;
		}
		DeviceType currentDeviceType = CurrentDeviceType;
		if (device == null || (device is Mouse && device.name != "VirtualMouse") || device is Keyboard)
		{
			CurrentDeviceType = DeviceType.KeyboardAndMouse;
		}
		else if (device is XInputController)
		{
			CurrentDeviceType = DeviceType.Xbox;
		}
		else if (device is DualShockGamepad)
		{
			CurrentDeviceType = DeviceType.PS5;
		}
		else if (device is SwitchProControllerHID)
		{
			CurrentDeviceType = DeviceType.SwitchPro;
		}
		else
		{
			CurrentDeviceType = DeviceType.Xbox;
		}
		if (SteamEnabler.IsSteamEnabled && SteamClient.IsValid && SteamUtils.IsRunningOnSteamDeck && CurrentDeviceType == DeviceType.Xbox)
		{
			CurrentDeviceType = DeviceType.Steam;
		}
		if (device is Gamepad currentGamepad)
		{
			CurrentGamepad = currentGamepad;
		}
		currentDeviceId = num;
		if (currentDeviceType != CurrentDeviceType)
		{
			currentDeviceIconSet = iconSets.GetIconSet(CurrentDeviceType);
			InputManager.SwitchedInputDeviceType?.Invoke();
			if (currentDeviceType == DeviceType.KeyboardAndMouse)
			{
				InputManager.SwitchedToGamepad?.Invoke();
			}
			else if (CurrentDeviceType == DeviceType.KeyboardAndMouse)
			{
				InputManager.SwitchedToKeyboard?.Invoke();
			}
		}
	}

	public static Sprite GetInputIcon(InputAction action)
	{
		if (action == null)
		{
			return null;
		}
		int bindingIndex;
		return GetInputIcon(GetBindingPath(action, UsingGamepad, out bindingIndex));
	}

	public static Sprite GetInputIcon(string bindingPath)
	{
		if (currentDeviceIconSet == null)
		{
			return null;
		}
		return currentDeviceIconSet.GetIcon(bindingPath);
	}

	public static Sprite GetInputIcon(string bindingPath, DeviceType deviceType)
	{
		if (deviceType == CurrentDeviceType)
		{
			return GetInputIcon(bindingPath);
		}
		return iconSets.GetIconSet(deviceType)?.GetIcon(bindingPath);
	}

	public static Sprite GetInputIcons(InputAction action, out Sprite secondary)
	{
		secondary = null;
		if (action == null)
		{
			return null;
		}
		int bindingIndex;
		Sprite inputIcon = GetInputIcon(GetBindingPath(action, UsingGamepad, out bindingIndex));
		if (bindingIndex > -1 && action.bindings[bindingIndex].isPartOfComposite && action.bindings.Count > bindingIndex + 1)
		{
			secondary = GetInputIcon(action.bindings[bindingIndex + 1].effectivePath);
		}
		return inputIcon;
	}

	public static string GetInputIconRichTextTag(InputAction action)
	{
		if (action == null)
		{
			return null;
		}
		int bindingIndex;
		return GetInputIconRichTextTag(GetBindingPath(action, UsingGamepad, out bindingIndex));
	}

	public static string GetCompositeInputIconRichTextTags(InputAction action, string delimiter, bool canMerge)
	{
		if (action == null)
		{
			return null;
		}
		GetCompositeBindingPaths(action, UsingGamepad, bindingPathBuffer, canMerge);
		if (bindingPathBuffer.Count <= 0)
		{
			return null;
		}
		string text = GetInputIconRichTextTag(bindingPathBuffer[0]);
		for (int i = 1; i < bindingPathBuffer.Count; i++)
		{
			text = text + delimiter + GetInputIconRichTextTag(bindingPathBuffer[i]);
		}
		return text;
	}

	public static string GetInputIconRichTextTag(string bindingPath)
	{
		if (currentDeviceIconSet == null)
		{
			return null;
		}
		return currentDeviceIconSet.GetIconRichTextTag(bindingPath);
	}

	public static string GetBindingPath(InputAction action, bool isGamepad, out int bindingIndex)
	{
		bindingIndex = -1;
		if (action == null)
		{
			return string.Empty;
		}
		string text = (isGamepad ? GamepadGroup : KeyboardGroup);
		int count = action.bindings.Count;
		for (int i = 0; i < count; i++)
		{
			bindingIndex = i;
			InputBinding binding = action.bindings[bindingIndex];
			if (InputBinding.MaskByGroup(text).Matches(binding) && !binding.isComposite && !binding.isPartOfComposite)
			{
				return binding.effectivePath;
			}
			if (i + 1 < count && binding.isComposite)
			{
				bindingIndex = i + 1;
				InputBinding binding2 = action.bindings[bindingIndex];
				if (InputBinding.MaskByGroup(text).Matches(binding2))
				{
					return action.bindings[bindingIndex].effectivePath;
				}
			}
		}
		return string.Empty;
	}

	public static void GetCompositeBindingPaths(InputAction action, bool isGamepad, List<string> bindingPaths, bool canMerge)
	{
		bindingPaths.Clear();
		if (action == null)
		{
			return;
		}
		string text = (isGamepad ? GamepadGroup : KeyboardGroup);
		int count = action.bindings.Count;
		for (int i = 0; i < count; i++)
		{
			InputBinding binding = action.bindings[i];
			if (InputBinding.MaskByGroup(text).Matches(binding))
			{
				if (!binding.isComposite)
				{
					bindingPaths.Add(binding.effectivePath);
				}
				if (!binding.isComposite && !binding.isPartOfComposite)
				{
					break;
				}
			}
		}
		if (canMerge)
		{
			while (TryMergeBindingPaths())
			{
			}
		}
		bool TryMergeBindingPaths()
		{
			if (currentDeviceIconSet == null)
			{
				return false;
			}
			for (int num = bindingPaths.Count - 1; num >= 1; num--)
			{
				for (int num2 = num - 1; num2 >= 0; num2--)
				{
					if (currentDeviceIconSet.TryMerge(bindingPaths[num], bindingPaths[num2], out var result))
					{
						bindingPaths.RemoveAt(num);
						bindingPaths.RemoveAt(num2);
						bindingPaths.Add(result.bindingPath);
						return true;
					}
				}
			}
			return false;
		}
	}

	public static string GetBindingDisplayStringOrCompositeName(InputAction action, bool isGamepad, InputBinding.DisplayStringOptions displayOptions = InputBinding.DisplayStringOptions.DontIncludeInteractions)
	{
		string text = (isGamepad ? GamepadGroup : KeyboardGroup);
		int count = action.bindings.Count;
		for (int i = 0; i < count; i++)
		{
			InputBinding binding = action.bindings[i];
			if (InputBinding.MaskByGroup(text).Matches(binding) && !binding.isComposite && !binding.isPartOfComposite)
			{
				return action.GetBindingDisplayString(displayOptions, text);
			}
			if (i + 1 < count && binding.isComposite)
			{
				InputBinding binding2 = action.bindings[i + 1];
				if (InputBinding.MaskByGroup(text).Matches(binding2))
				{
					return action.GetBindingDisplayString(i, displayOptions);
				}
			}
		}
		return string.Empty;
	}

	public static int GetFirstBindingIndex(InputAction action, bool isGamepad)
	{
		string text = (isGamepad ? GamepadGroup : KeyboardGroup);
		int count = action.bindings.Count;
		for (int i = 0; i < count; i++)
		{
			InputBinding binding = action.bindings[i];
			if (InputBinding.MaskByGroup(text).Matches(binding) && !binding.isComposite && !binding.isPartOfComposite)
			{
				return i;
			}
			if (i + 1 < count && binding.isComposite)
			{
				InputBinding binding2 = action.bindings[i + 1];
				if (InputBinding.MaskByGroup(text).Matches(binding2))
				{
					return i;
				}
			}
		}
		return -1;
	}

	public static InputAction GetAction(InputActionReference inputActionReference)
	{
		if (inputActionReference == null)
		{
			return null;
		}
		return controls.FindAction(inputActionReference.action.id.ToString());
	}

	public static void RemoveBindingOverride(InputAction action, int bindingIndex, bool checkSyncedActions = true)
	{
		if (bindingIndex < 0)
		{
			return;
		}
		if (action.bindings[bindingIndex].isComposite)
		{
			for (int i = bindingIndex + 1; i < action.bindings.Count && action.bindings[i].isPartOfComposite; i++)
			{
				action.RemoveBindingOverride(i);
			}
		}
		else
		{
			action.RemoveBindingOverride(bindingIndex);
		}
	}

	public static void RemoveAllBindingOverrides()
	{
		foreach (InputActionMap actionMap in controls.asset.actionMaps)
		{
			actionMap.RemoveAllBindingOverrides();
		}
	}

	public static void OverrideUIInputModuleMoveRepeatRate(float rate)
	{
		if (EventSystem.current != null && EventSystem.current.currentInputModule is InputSystemUIInputModule inputSystemUIInputModule)
		{
			inputSystemUIInputModule.moveRepeatRate = rate;
		}
	}

	public static void RevertUIInputModuleMoveRepeatRate()
	{
		if (EventSystem.current != null && EventSystem.current.currentInputModule is InputSystemUIInputModule inputSystemUIInputModule)
		{
			inputSystemUIInputModule.moveRepeatRate = 0.1f;
		}
	}

	private static void OnActionChange(object obj, InputActionChange change)
	{
		switch (change)
		{
		case InputActionChange.ActionStarted:
			RefreshCurrentDeviceType((obj as InputAction).activeControl.device);
			break;
		case InputActionChange.BoundControlsChanged:
			InputManager.UpdatedKeyBindings?.Invoke();
			break;
		}
	}

	private static void OnDeviceChange(InputDevice device, InputDeviceChange change)
	{
		if (CurrentGamepad != null && device == CurrentGamepad && (change == InputDeviceChange.Removed || change == InputDeviceChange.Disconnected || !Gamepad.all.Contains(CurrentGamepad)))
		{
			CurrentGamepad = null;
		}
	}

	private static void OnDevConsoleActivated()
	{
		EnableMode(InputMode.DevConsole);
	}

	private static void OnDevConsoleDeactivated()
	{
		DisableMode(InputMode.DevConsole);
	}

	private static void OnIsSteamOverlayActiveChanged()
	{
		if (GameManager.IsSteamOverlayActive)
		{
			EnableMode(InputMode.SteamOverlay);
		}
		else
		{
			DisableMode(InputMode.SteamOverlay);
		}
	}

	private static void OnApplicationQuit()
	{
		InputSystem.onActionChange -= OnActionChange;
		InputSystem.onDeviceChange -= OnDeviceChange;
		Application.quitting -= OnApplicationQuit;
		controls?.Dispose();
		controls = null;
		isInitialized = false;
	}
}
