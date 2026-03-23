using System;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Localization.Components;
using UnityEngine.Pool;
using UnityEngine.UI;

public class ControlsRebind : MonoBehaviour
{
	[SerializeField]
	private ControlRebindSettings settings;

	[SerializeField]
	private GameObject heading;

	[SerializeField]
	private GameObject actionMapSubheading;

	[SerializeField]
	private GameObject resetAllButtonPrefab;

	[SerializeField]
	private Selectable lastExplicitSelectable;

	[SerializeField]
	private Scrollbar verticalScrollBar;

	[SerializeField]
	private ActionRebindOption actionRebindOption;

	[SerializeField]
	private Transform holder;

	private Button resetAllButton;

	private readonly List<ActionRebindOption> actionRebinds = new List<ActionRebindOption>();

	private InputActionRebindingExtensions.RebindingOperation rebindOperation;

	public static float RebindCanceledTimestamp { get; private set; }

	private void Awake()
	{
		LocalizeStringEvent componentInChildren = UnityEngine.Object.Instantiate(heading, holder).GetComponentInChildren<LocalizeStringEvent>();
		componentInChildren.StringReference = Localization.UI.SETTINGS_Keybinds_Category_Ref;
		componentInChildren.transform.parent.gameObject.name = "Rebinds";
		if (resetAllButtonPrefab != null)
		{
			resetAllButton = UnityEngine.Object.Instantiate(resetAllButtonPrefab, holder).GetComponentInChildren<Button>();
			resetAllButton.GetComponentInChildren<LocalizeStringEvent>().StringReference = Localization.UI.SETTINGS_Keybinds_ResetAll_Ref;
			resetAllButton.transform.parent.gameObject.name = "Reset all";
			resetAllButton.onClick.AddListener(OnResetAll);
			Navigation navigation = lastExplicitSelectable.navigation;
			navigation.selectOnDown = resetAllButton;
			lastExplicitSelectable.navigation = navigation;
			Navigation navigation2 = resetAllButton.navigation;
			navigation2.mode = Navigation.Mode.Explicit;
			navigation2.selectOnUp = lastExplicitSelectable;
			resetAllButton.navigation = navigation2;
		}
		AddRebindOptions();
		void AddRebindOptions()
		{
			foreach (InputActionMap actionMap in InputManager.Controls.asset.actionMaps)
			{
				ParseActionMap(actionMap);
			}
			if (actionRebinds.Count > 0)
			{
				List<ActionRebindOption> list = actionRebinds;
				ActionRebindOption obj = list[list.Count - 1];
				obj.SetVerticalNavigationTarget(verticalScrollBar, isUp: false);
				obj.SetAsSelectableNavigationDownTarget(verticalScrollBar, isUp: true);
			}
		}
		void ParseAction(InputAction action, InputActionMap map, ref bool addedSubheading)
		{
			if (!settings.allUnrebindableActions.Contains(action.id) && (!settings.actionsToIgnoreInOwnMapGuids.Contains(action.id) || !(map.id == action.actionMap.id)))
			{
				int firstBindingIndex = InputManager.GetFirstBindingIndex(action, isGamepad: false);
				int firstBindingIndex2 = InputManager.GetFirstBindingIndex(action, isGamepad: true);
				if (firstBindingIndex >= 0)
				{
					if (!addedSubheading)
					{
						LocalizeStringEvent componentInChildren2 = UnityEngine.Object.Instantiate(actionMapSubheading, holder).GetComponentInChildren<LocalizeStringEvent>();
						componentInChildren2.StringReference = LocalizationManager.GetLocalizedString(StringTable.UI, "SETTINGS_Keybinds_" + action.actionMap.name.RemoveWhitespace());
						componentInChildren2.transform.parent.gameObject.name = action.actionMap.name + " subheading";
						addedSubheading = true;
					}
					AddRebindOption(action, firstBindingIndex, firstBindingIndex2);
				}
			}
		}
		void ParseActionMap(InputActionMap map)
		{
			if (!settings.allUnrebindableMaps.Contains(map.name))
			{
				bool addedSubheading = false;
				foreach (InputAction action in map.actions)
				{
					ParseAction(action, map, ref addedSubheading);
				}
				if (settings.addedActionsPerMap.TryGetValue(map.name, out var value))
				{
					foreach (InputActionReference item in value)
					{
						ParseAction(InputManager.GetAction(item), map, ref addedSubheading);
					}
				}
			}
		}
	}

	private void OnEnable()
	{
		RefreshBindingDisplay();
		InputSystem.onActionChange += OnActionChange;
	}

	private void OnDisable()
	{
		GameSettings.SaveInputBindings();
		rebindOperation?.Dispose();
		rebindOperation = null;
		InputSystem.onActionChange -= OnActionChange;
	}

	private void AddRebindOption(InputAction action, int keyboardBindingIndex, int gamepadBindingIndex)
	{
		if (!action.bindings[keyboardBindingIndex].isComposite)
		{
			ActionRebindOption actionRebindOption = UnityEngine.Object.Instantiate(this.actionRebindOption, holder);
			actionRebindOption.Initialize(this, action, keyboardBindingIndex, gamepadBindingIndex);
			AddOption(actionRebindOption);
			return;
		}
		for (int i = 1; i + keyboardBindingIndex < action.bindings.Count && action.bindings[keyboardBindingIndex + i].isPartOfComposite && (gamepadBindingIndex < 0 || (i + gamepadBindingIndex < action.bindings.Count && action.bindings[gamepadBindingIndex + i].isPartOfComposite)); i++)
		{
			ActionRebindOption actionRebindOption2 = UnityEngine.Object.Instantiate(this.actionRebindOption, holder);
			actionRebindOption2.Initialize(this, action, keyboardBindingIndex + i, (gamepadBindingIndex < 0) ? (-1) : (gamepadBindingIndex + i));
			AddOption(actionRebindOption2);
		}
		void AddOption(ActionRebindOption option)
		{
			if (actionRebinds.Count <= 0)
			{
				option.SetAsSelectableNavigationDownTarget(resetAllButton, isUp: false);
				option.SetVerticalNavigationTarget(resetAllButton, isUp: true);
			}
			else
			{
				List<ActionRebindOption> list = actionRebinds;
				ActionRebindOption actionRebindOption3 = list[list.Count - 1];
				actionRebindOption3.SetVerticalNavigationTarget(option, isUp: false);
				option.SetVerticalNavigationTarget(actionRebindOption3, isUp: true);
			}
			actionRebinds.Add(option);
		}
	}

	public void StartRebind(InputAction action, int bindingIndex, bool isGamepad, Action onComplete)
	{
		if (bindingIndex >= 0)
		{
			PerformRebind(action, bindingIndex, onComplete, allCompositeParts: false, isGamepad);
		}
	}

	private void PerformRebind(InputAction action, int bindingIndex, Action onComplete, bool allCompositeParts = false, bool isGamepad = false)
	{
		rebindOperation?.Cancel();
		bool actionWasEnabled = action.enabled;
		if (actionWasEnabled)
		{
			action.Disable();
		}
		bool[] actionMapEnabled = new bool[InputManager.Controls.asset.actionMaps.Count];
		for (int i = 0; i < actionMapEnabled.Length; i++)
		{
			InputActionMap inputActionMap = InputManager.Controls.asset.actionMaps[i];
			actionMapEnabled[i] = inputActionMap.enabled;
			inputActionMap.Disable();
		}
		string text = (isGamepad ? InputManager.GamepadGroup : InputManager.KeyboardGroup);
		InputBinding previousBinding = action.bindings[bindingIndex];
		rebindOperation = action.PerformInteractiveRebinding(bindingIndex).WithBindingGroup(text).WithCancelingThrough("<Keyboard>/escape")
			.OnCancel(delegate
			{
				FullScreenMessage.Hide();
				CleanUp();
				RebindCanceledTimestamp = Time.unscaledTime;
			})
			.OnComplete(delegate(InputActionRebindingExtensions.RebindingOperation operation)
			{
				List<(InputAction, int)> value;
				using (CollectionPool<List<(InputAction, int)>, (InputAction, int)>.Get(out value))
				{
					if (ValidateBinding(operation.action, bindingIndex, isGamepad, value))
					{
						CompleteSuccess();
					}
					else
					{
						HandleOverlap(operation.action, bindingIndex, value, previousBinding, CompleteSuccess);
					}
				}
			});
		if (action.bindings[bindingIndex].isPartOfComposite)
		{
			rebindOperation.WithControlsExcluding("<Mouse>/scroll/x").WithControlsExcluding("<Mouse>/scroll/y").WithControlsExcluding("<Gamepad>/leftStick/x")
				.WithControlsExcluding("<Gamepad>/leftStick/y")
				.WithControlsExcluding("<Gamepad>/rightStick/x")
				.WithControlsExcluding("<Gamepad>/rightStick/y")
				.WithControlsExcluding("<Gamepad>/dpad/x")
				.WithControlsExcluding("<Gamepad>/dpad/y");
		}
		if (isGamepad)
		{
			rebindOperation.WithControlsExcluding("<Mouse>").WithControlsExcluding("<Keyboard>");
		}
		foreach (InputBinding binding in InputManager.Controls.Ingame.Pause.bindings)
		{
			rebindOperation.WithControlsExcluding(binding.path);
		}
		foreach (InputBinding binding2 in InputManager.Controls.Camera.Look.bindings)
		{
			rebindOperation.WithControlsExcluding(binding2.path);
		}
		string text2 = "SETTINGS_Keybinds_" + action.actionMap.name.RemoveWhitespace() + "_" + action.name.RemoveWhitespace();
		if (action.bindings[bindingIndex].isPartOfComposite)
		{
			text2 = text2 + "_" + action.bindings[bindingIndex].name;
		}
		FullScreenMessage.Show(LocalizationManager.GetString(StringTable.UI, text2) + "\n\n" + Localization.UI.SETTINGS_Keybinds_RebindWaiting);
		rebindOperation.Start();
		async void CleanUp()
		{
			await UniTask.Yield();
			rebindOperation?.Dispose();
			rebindOperation = null;
			await UniTask.WaitForSeconds(0.15f, ignoreTimeScale: true);
			if (actionWasEnabled)
			{
				action.Enable();
			}
			for (int j = 0; j < actionMapEnabled.Length; j++)
			{
				InputActionMap inputActionMap2 = InputManager.Controls.asset.actionMaps[j];
				if (actionMapEnabled[j])
				{
					inputActionMap2.Enable();
				}
			}
		}
		void CompleteSuccess()
		{
			CleanUp();
			FullScreenMessage.Hide();
			onComplete?.Invoke();
		}
	}

	public bool ValidateBinding(InputAction action, int bindingIndex, bool isGamepad, List<(InputAction, int)> actionOverlaps)
	{
		string text = GetBindingPath(action.bindings[bindingIndex]);
		string text2 = (isGamepad ? InputManager.GamepadGroup : InputManager.KeyboardGroup);
		List<InputActionMap> value;
		using (CollectionPool<List<InputActionMap>, InputActionMap>.Get(out value))
		{
			InputManager.GetOverlappingActionMaps(action.actionMap, value);
			foreach (InputActionMap item in value)
			{
				foreach (InputAction item2 in item)
				{
					if (item2.GetBindingIndex(text2) < 0)
					{
						continue;
					}
					for (int i = 0; i < item2.bindings.Count; i++)
					{
						InputBinding binding = item2.bindings[i];
						if (action != item2 || i != bindingIndex)
						{
							string text3 = GetBindingPath(binding);
							if (text3 != null && !(text3 == string.Empty) && text3 == text)
							{
								actionOverlaps.Add((item2, i));
							}
						}
					}
				}
			}
			return actionOverlaps.Count == 0;
		}
		static string GetBindingPath(InputBinding inputBinding)
		{
			if (!inputBinding.hasOverrides)
			{
				return inputBinding.path;
			}
			return inputBinding.overridePath;
		}
	}

	public void HandleOverlap(InputAction current, int currentBindingIndex, List<(InputAction, int)> overlapList, InputBinding previousBinding, Action onComplete)
	{
		List<(InputAction, int)> overlaps = CollectionPool<List<(InputAction, int)>, (InputAction, int)>.Get();
		overlaps.AddRange(overlapList);
		string text = string.Empty;
		string message;
		if (overlaps.Count == 1)
		{
			message = string.Format(Localization.UI.SETTINGS_Controls_SingleBindingConflict, GetBindingName(overlaps[0].Item1, overlaps[0].Item2));
		}
		else if (overlaps.Count == 2)
		{
			string arg = GetBindingName(overlaps[0].Item1, overlaps[0].Item2);
			string arg2 = GetBindingName(overlaps[1].Item1, overlaps[1].Item2);
			message = string.Format(Localization.UI.SETTINGS_Controls_DoubleBindingConflict, arg, arg2);
		}
		else
		{
			foreach (var item in overlaps)
			{
				text = text + GetBindingName(item.Item1, item.Item2) + ", ";
			}
			if (text.Length > 0)
			{
				text = text.Remove(text.Length - 2);
			}
			message = string.Format(Localization.UI.SETTINGS_Controls_MultipleBindingConflict, text);
		}
		FullScreenMessage.Show(message, new FullScreenMessage.ButtonEntry(Localization.UI.SETTINGS_Controls_Override, Override), new FullScreenMessage.ButtonEntry(Localization.UI.MISC_Cancel, Cancel));
		void Cancel()
		{
			current.ApplyBindingOverride(currentBindingIndex, previousBinding);
			Complete();
		}
		void Complete()
		{
			RefreshBindingDisplay();
			FullScreenMessage.Hide();
			onComplete?.Invoke();
			CollectionPool<List<(InputAction, int)>, (InputAction, int)>.Release(overlaps);
		}
		static string GetBindingName(InputAction action, int bindingIndex)
		{
			return GameManager.UiSettings.ApplyColorTag(ActionRebindOption.GetLocalizedBindingName(action, bindingIndex), TextHighlight.Regular);
		}
		void Override()
		{
			foreach (var item2 in overlaps)
			{
				item2.Item1.ApplyBindingOverride(item2.Item2, string.Empty);
			}
			Complete();
		}
	}

	private void OnActionChange(object obj, InputActionChange change)
	{
		if (change == InputActionChange.BoundControlsChanged)
		{
			RefreshBindingDisplay();
		}
	}

	public void RefreshBindingDisplay()
	{
		foreach (ActionRebindOption actionRebind in actionRebinds)
		{
			actionRebind.RefreshBindingDisplay();
		}
	}

	private void OnResetAll()
	{
		InputManager.RemoveAllBindingOverrides();
	}
}
