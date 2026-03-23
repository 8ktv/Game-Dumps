using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Overthrown.LevelEditor;

public class ShortcutButton : ShortcutBase
{
	protected override void OnShortcutPressed()
	{
		ControllerInputReselect componentInParent = GetComponentInParent<ControllerInputReselect>();
		if ((!(componentInParent != null) || componentInParent.IsOnTop()) && base.gameObject.activeInHierarchy)
		{
			Button component = GetComponent<Button>();
			component.OnSubmit(new BaseEventData(EventSystem.current));
			if (component.navigation.mode != Navigation.Mode.None && base.gameObject.activeInHierarchy)
			{
				component.Select();
			}
		}
	}
}
