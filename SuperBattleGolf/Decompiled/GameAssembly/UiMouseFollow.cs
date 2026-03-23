using UnityEngine;
using UnityEngine.InputSystem;

public class UiMouseFollow : MonoBehaviour
{
	private void Update()
	{
		if (InputManager.UsingKeyboard)
		{
			base.transform.position = Mouse.current.position.value;
		}
	}
}
