using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Localization.Components;
using UnityEngine.UI;

public class MatchSetupHole : MonoBehaviour, IPointerEnterHandler, IEventSystemHandler, IPointerMoveHandler, IPointerExitHandler, IPointerDownHandler, IPointerUpHandler
{
	public MatchSetupMenu menu;

	public HoleData holeData;

	public LocalizeStringEvent holeNameLocalizeStringEvent;

	public TextMeshProUGUI holeNumber;

	public Image background;

	public Image[] difficultyIcons;

	public Sprite[] difficultyIconSprites;

	private ControllerSelectable selectable;

	private float lastClickTime;

	private bool mouseDown;

	private void Start()
	{
		if (TryGetComponent<ControllerSelectable>(out selectable))
		{
			selectable.Submitted += OnControllerSubmit;
			selectable.Selected += delegate
			{
				SetHover(hover: true);
			};
			selectable.Deselected += delegate
			{
				SetHover(hover: false);
			};
		}
		InputManager.SwitchedInputDeviceType += InputDeviceChanged;
	}

	private void OnControllerSubmit()
	{
		if (TryGetComponent<Button>(out var component) && component.IsInteractable())
		{
			ReorderableList componentInParent = GetComponentInParent<ReorderableList>();
			Swap(componentInParent);
		}
	}

	private void Update()
	{
		if (InputManager.UsingGamepad && selectable.IsSelected && !InputManager.CurrentModeMask.HasMode(InputMode.ForceDisabled))
		{
			if (InputManager.CurrentGamepad.leftTrigger.wasPressedThisFrame)
			{
				MoveUp();
			}
			if (InputManager.CurrentGamepad.rightTrigger.wasPressedThisFrame)
			{
				MoveDown();
			}
		}
	}

	private void OnDisable()
	{
		if (menu != null && menu.holePrefab != null)
		{
			menu.holePreview.SetActive(active: false);
		}
	}

	private void Swap(ReorderableList currentParent)
	{
		ReorderableList reorderableList = ((!(currentParent == menu.activeHoles)) ? menu.activeHoles : menu.inactiveHoles);
		int num = 0;
		if (reorderableList == menu.inactiveHoles)
		{
			MatchSetupHole[] componentsInChildren = reorderableList.GetComponentsInChildren<MatchSetupHole>();
			for (int i = 0; i < componentsInChildren.Length && componentsInChildren[i].holeData.GlobalIndex <= holeData.GlobalIndex; i++)
			{
				num++;
			}
		}
		else
		{
			num = BMath.Max(0, reorderableList.transform.childCount - 1);
		}
		Move(reorderableList, currentParent, num, updateSelection: true);
	}

	private void MoveUp()
	{
		int siblingIndex = base.transform.GetSiblingIndex();
		if (!(base.transform.parent == null) && siblingIndex > 0)
		{
			ReorderableList componentInParent = GetComponentInParent<ReorderableList>();
			if (!(componentInParent == null) && !(componentInParent == menu.inactiveHoles))
			{
				Move(componentInParent, componentInParent, siblingIndex - 1, updateSelection: false);
			}
		}
	}

	private void MoveDown()
	{
		int siblingIndex = base.transform.GetSiblingIndex();
		if (!(base.transform.parent == null) && siblingIndex < base.transform.parent.childCount)
		{
			ReorderableList componentInParent = GetComponentInParent<ReorderableList>();
			if (!(componentInParent == null) && !(componentInParent == menu.inactiveHoles))
			{
				Move(componentInParent, componentInParent, siblingIndex + 1, updateSelection: false);
			}
		}
	}

	private void Move(ReorderableList target, ReorderableList currentParent, int targetIndex, bool updateSelection)
	{
		GetComponent<ReorderableListElement>().AssignTo(target, currentParent, targetIndex, updateSelection);
		SetHover(hover: false);
	}

	public void OnPointerEnter(PointerEventData eventData)
	{
		SetHover(hover: true);
	}

	public void OnPointerExit(PointerEventData eventData)
	{
		SetHover(hover: false);
	}

	public void OnPointerMove(PointerEventData eventData)
	{
		SetHover(ReorderableListElement.Current == null);
	}

	private void SetHover(bool hover)
	{
		if (hover)
		{
			if (holeData.ScreenshotsThumbnail != null && holeData.ScreenshotsThumbnail.Count > 0)
			{
				menu.holePreviewImage.sprite = holeData.ScreenshotsThumbnail[0];
				menu.holePreview.transform.position = base.transform.position;
			}
			else
			{
				Debug.LogWarning("Hole " + holeData.name + " is missing thumbnail screenshot!");
				hover = false;
			}
		}
		menu.holePreview.SetActive(hover);
	}

	private void InputDeviceChanged()
	{
		SetHover(hover: false);
	}

	public void OnPointerDown(PointerEventData eventData)
	{
		if (!mouseDown && TryGetComponent<Button>(out var component) && component.IsInteractable())
		{
			lastClickTime = Time.time;
			mouseDown = true;
		}
	}

	public void OnPointerUp(PointerEventData eventData)
	{
		if (mouseDown)
		{
			float num = Time.time - lastClickTime;
			ReorderableList currentParent = GetComponent<ReorderableListElement>().CurrentParent;
			if (currentParent == ReorderableList.Current && num < 0.25f)
			{
				Swap(currentParent);
			}
			mouseDown = false;
		}
	}
}
