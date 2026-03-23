using UnityEngine;
using UnityEngine.UI;

public class ScrollRectControllerInput : MonoBehaviour
{
	[SerializeField]
	private ScrollRect scrollRect;

	[SerializeField]
	private float gamepadScrollDeadzone = 0.2f;

	[SerializeField]
	private Vector2 gamepadScrollSpeed = Vector2.one * 5f;

	private void OnEnable()
	{
		RefreshMovementType();
		InputManager.SwitchedToGamepad += RefreshMovementType;
		InputManager.SwitchedToKeyboard += RefreshMovementType;
	}

	private void OnDisable()
	{
		InputManager.SwitchedToGamepad += RefreshMovementType;
		InputManager.SwitchedToKeyboard += RefreshMovementType;
	}

	private void Update()
	{
		if (!FullScreenMessage.IsDisplayingAnyMessage && PlayerInput.UsingGamepad && InputManager.CurrentGamepad != null && !InputManager.CurrentModeMask.HasMode(InputMode.ForceDisabled))
		{
			Vector2 value = InputManager.CurrentGamepad.rightStick.value;
			if (!value.Approximately(Vector2.zero, gamepadScrollDeadzone))
			{
				scrollRect.content.anchoredPosition -= value * gamepadScrollSpeed;
			}
		}
	}

	private void RefreshMovementType()
	{
		scrollRect.movementType = ((!PlayerInput.UsingGamepad) ? ScrollRect.MovementType.Elastic : ScrollRect.MovementType.Clamped);
	}
}
