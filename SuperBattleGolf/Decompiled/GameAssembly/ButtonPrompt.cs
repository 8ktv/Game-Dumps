using System;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Localization;
using UnityEngine.Localization.Components;
using UnityEngine.UI;

public class ButtonPrompt : MonoBehaviour
{
	public LocalizeStringEvent label;

	public Image primaryIcon;

	public Image secondaryIcon;

	public CanvasGroup canvasGroup;

	[NonSerialized]
	public ButtonPromptManager.Type promptType;

	private InputAction inputAction;

	public void Initialize(InputAction inputAction, LocalizedString localizedString)
	{
		this.inputAction = inputAction;
		label.StringReference = localizedString;
		label.RefreshString();
		UpdatePrompt();
	}

	private void OnEnable()
	{
		InputManager.SwitchedInputDeviceType += UpdatePrompt;
	}

	private void OnDisable()
	{
		InputManager.SwitchedInputDeviceType -= UpdatePrompt;
	}

	private void UpdatePrompt()
	{
		primaryIcon.sprite = InputManager.GetInputIcons(inputAction, out var secondary);
		secondaryIcon.sprite = secondary;
		secondaryIcon.gameObject.SetActive(secondary != null);
	}
}
