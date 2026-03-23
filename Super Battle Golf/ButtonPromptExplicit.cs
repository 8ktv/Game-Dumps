using UnityEngine;
using UnityEngine.InputSystem.Layouts;
using UnityEngine.UI;

public class ButtonPromptExplicit : MonoBehaviour
{
	[InputControl]
	public string bindingPath;

	public Image icon;

	private void OnEnable()
	{
		InputManager.SwitchedInputDeviceType += UpdateIcon;
		UpdateIcon();
	}

	private void OnDisable()
	{
		InputManager.SwitchedInputDeviceType -= UpdateIcon;
	}

	private void UpdateIcon()
	{
		Sprite inputIcon = InputManager.GetInputIcon(bindingPath);
		icon.sprite = inputIcon;
		icon.color = ((inputIcon != null) ? Color.white : Color.clear);
	}
}
