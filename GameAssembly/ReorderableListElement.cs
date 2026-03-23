using System;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class ReorderableListElement : MonoBehaviour, IPointerDownHandler, IEventSystemHandler, IPointerUpHandler
{
	public static ReorderableListElement Current;

	private Button button;

	private ReorderableList currentParent;

	private int prevSiblingIndex;

	private bool pointerDown;

	private Vector2 pointerDelta;

	public Button Button => button;

	public ReorderableList CurrentParent => currentParent;

	public event Action OnAssignedToParent;

	public event Action OnSelected;

	private void Awake()
	{
		button = GetComponent<Button>();
	}

	public void OnPointerDown(PointerEventData eventData)
	{
		if (!(Current != null) && button.IsInteractable())
		{
			currentParent = GetComponentInParent<ReorderableList>();
			prevSiblingIndex = base.transform.GetSiblingIndex();
			base.transform.SetParent(GetComponentInParent<Canvas>().transform);
			pointerDown = true;
			pointerDelta = (Vector2)base.transform.position - Mouse.current.position.value;
			Current = this;
			currentParent.RefreshList();
			Canvas.ForceUpdateCanvases();
			this.OnSelected?.Invoke();
		}
	}

	public void OnPointerUp(PointerEventData eventData)
	{
		if (pointerDown)
		{
			Current = null;
			pointerDown = false;
			if (ReorderableList.Current != null)
			{
				currentParent = ReorderableList.Current;
			}
			currentParent.AssignElement(this);
		}
	}

	public void AssignTo(ReorderableList target)
	{
		ReorderableList componentInParent = GetComponentInParent<ReorderableList>();
		AssignTo(target, componentInParent);
	}

	public void AssignTo(ReorderableList target, ReorderableList parent, int indexOverride = 0, bool updateSelection = true)
	{
		int num = base.transform.GetSiblingIndex();
		base.transform.SetParent(GetComponentInParent<Canvas>().transform);
		target.AssignElement(GetComponent<ReorderableListElement>(), indexOverride);
		if (updateSelection && parent.transform.childCount - 1 > 0)
		{
			if (num >= parent.transform.childCount - 1)
			{
				num--;
			}
			ControllerSelectable component = parent.transform.GetChild(BMath.Clamp(num, 0, parent.transform.childCount - 1)).GetComponent<ControllerSelectable>();
			parent.GetComponentInParent<MenuNavigation>().Select(component);
		}
	}

	public void InformAssigned()
	{
		this.OnAssignedToParent?.Invoke();
	}

	public void Reset()
	{
		if (Current == this)
		{
			Current = null;
		}
		base.transform.SetParent(currentParent.transform);
		base.transform.SetSiblingIndex(prevSiblingIndex);
		pointerDown = false;
	}

	private void Update()
	{
		if (pointerDown && button.IsInteractable())
		{
			base.transform.position = Mouse.current.position.value + pointerDelta;
			if (!currentParent.isActiveAndEnabled)
			{
				Reset();
			}
		}
	}
}
