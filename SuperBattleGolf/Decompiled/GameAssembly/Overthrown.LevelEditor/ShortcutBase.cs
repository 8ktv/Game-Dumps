using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.UI;

namespace Overthrown.LevelEditor;

public abstract class ShortcutBase : MonoBehaviour
{
	public InputAction keyCombination;

	private void OnEnable()
	{
		keyCombination?.Enable();
	}

	private void OnDisable()
	{
		keyCombination?.Disable();
	}

	private void Update()
	{
		if (!FullScreenMessage.IsDisplayingAnyMessage || !(GetComponentInParent<FullScreenMessage>() == null))
		{
			GameObject currentSelectedGameObject = EventSystem.current.currentSelectedGameObject;
			if ((!(currentSelectedGameObject != null) || !currentSelectedGameObject.TryGetComponent<Selectable>(out var component) || !component.gameObject.activeSelf || (!(component is TMP_InputField) && !(component is TMP_Dropdown { IsExpanded: not false }))) && keyCombination.triggered)
			{
				OnShortcutPressed();
			}
		}
	}

	protected abstract void OnShortcutPressed();
}
