using System.Collections.Generic;
using System.Linq;
using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class ControllerInputReselect : MonoBehaviour
{
	public Selectable defaultSelectable;

	public bool alwaysOnTop;

	private Selectable currentSelection;

	private Selectable lastSelectable;

	private CanvasGroup group;

	private bool hasCanvasGroup;

	private static List<ControllerInputReselect> activeInstances = new List<ControllerInputReselect>();

	private void Start()
	{
		Debug.LogError("ControllerInputReselect is deprecated, won't work right, and should not be used!!!");
		hasCanvasGroup = TryGetComponent<CanvasGroup>(out group);
	}

	private void OnEnable()
	{
		activeInstances.Add(this);
		InputManager.SwitchedToGamepad += OnSwitchedToGamepad;
		if (InputManager.UsingGamepad)
		{
			TryReselectSelectable(forced: true, updateVirtualCursor: true);
		}
	}

	private void OnDisable()
	{
		activeInstances.Remove(this);
		InputManager.SwitchedToGamepad -= OnSwitchedToGamepad;
		if (activeInstances.Count > 0)
		{
			List<ControllerInputReselect> list = activeInstances;
			list[list.Count - 1].TryReselectSelectable(forced: true, updateVirtualCursor: true);
		}
	}

	public void ResetLastSelectable()
	{
		lastSelectable = null;
	}

	public bool IsOnTop()
	{
		if (!alwaysOnTop)
		{
			if (activeInstances.Count > 0 && !activeInstances.Any((ControllerInputReselect x) => x.alwaysOnTop))
			{
				List<ControllerInputReselect> list = activeInstances;
				return list[list.Count - 1] == this;
			}
			return false;
		}
		return true;
	}

	public async void TryReselectSelectable(bool forced = false, bool updateVirtualCursor = false)
	{
		if (!IsOnTop())
		{
			return;
		}
		if (!(EventSystem.current == null))
		{
			if (forced)
			{
				currentSelection = null;
				EventSystem.current.SetSelectedGameObject(null, null);
			}
			if (!DevConsoleGui.Active && (forced || !(EventSystem.current.currentSelectedGameObject != null) || !EventSystem.current.currentSelectedGameObject.activeInHierarchy) && (!FullScreenMessage.IsDisplayingAnyMessage || !(GetComponentInParent<FullScreenMessage>() == null)) && (!hasCanvasGroup || group.interactable) && (forced || InputManager.Controls == null || !(InputManager.Controls.UI.Navigate.ReadValue<Vector2>().sqrMagnitude < float.Epsilon)))
			{
				await UniTask.WaitForEndOfFrame(this);
				if (IsSelectableValid(lastSelectable))
				{
					currentSelection = lastSelectable;
				}
				else if (IsSelectableValid(defaultSelectable))
				{
					currentSelection = defaultSelectable;
				}
				else
				{
					Selectable[] componentsInChildren = GetComponentsInChildren<Selectable>();
					foreach (Selectable selectable in componentsInChildren)
					{
						if (IsSelectableValid(selectable))
						{
							currentSelection = selectable;
							break;
						}
					}
				}
			}
		}
		await UniTask.WaitForEndOfFrame(this);
		if (currentSelection != null && GetComponentsInChildren<Selectable>().Contains(currentSelection))
		{
			lastSelectable = currentSelection;
		}
		if (updateVirtualCursor && currentSelection != null)
		{
			TryMoveVirtualMouseToSelected(currentSelection.gameObject);
		}
	}

	private static bool IsSelectableValid(Selectable selectable)
	{
		if (selectable == null)
		{
			return false;
		}
		if (selectable.TryGetComponentInParent<CanvasGroup>(out var foundComponent, includeInactive: false) && !foundComponent.interactable)
		{
			return false;
		}
		if (selectable.enabled && selectable.gameObject.activeInHierarchy && selectable.interactable)
		{
			return selectable.navigation.mode != Navigation.Mode.None;
		}
		return false;
	}

	private void OnSwitchedToGamepad()
	{
		TryReselectSelectable();
	}

	private void OnUINavigate()
	{
		TryReselectSelectable(forced: false, updateVirtualCursor: true);
	}

	private void OnUIDeselected()
	{
		if (InputManager.UsingGamepad)
		{
			TryReselectSelectable();
		}
	}

	public static void TryMoveVirtualMouseToSelected(GameObject selected, bool force = false)
	{
		if (!(selected == null))
		{
			Scrollbar component2;
			ScrollRect foundComponent;
			if (!force && selected.TryGetComponent<Selectable>(out var component) && !IsSelectableValid(component))
			{
				EventSystem.current.SetSelectedGameObject(null, null);
			}
			else if (!force && !selected.TryGetComponent<Scrollbar>(out component2) && selected.TryGetComponentInParent<ScrollRect>(out foundComponent, includeInactive: false))
			{
				foundComponent.EnsureVisibility(selected.GetComponent<RectTransform>(), GameManager.UiSettings.ScrollRectControllerReselectDefaultPadding);
			}
		}
	}
}
