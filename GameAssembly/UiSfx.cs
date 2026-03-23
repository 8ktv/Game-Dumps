using FMODUnity;
using UnityEngine;
using UnityEngine.EventSystems;

public class UiSfx : MonoBehaviour, IPointerEnterHandler, IEventSystemHandler, IPointerClickHandler
{
	public enum Type
	{
		MainMenuButton,
		PauseMenuButton,
		SettingsTab,
		MatchSetupTab,
		GenericButton,
		CosmeticsButton,
		MatchSetupHole,
		MatchSetupPlayer,
		Dropdown,
		DropdownOption,
		StartMatch,
		CosmeticsTab,
		Slider,
		SliderHighlight
	}

	public Type type;

	private ControllerSelectable selectable;

	private MenuNavigation menuNavigation;

	private float onEnableTime;

	public void OnPointerClick(PointerEventData eventData)
	{
		PlaySelectSfx(fromGamepad: false);
	}

	public void OnPointerEnter(PointerEventData eventData)
	{
		PlayHoverSfx(fromGamepad: false);
	}

	private void Start()
	{
		menuNavigation = GetComponent<MenuNavigation>();
		if (TryGetComponent<ControllerSelectable>(out var component))
		{
			component.Selected += OnControllerSelectableSelected;
			component.Submitted += OnControllerSelectableSubmitted;
		}
		if ((type != Type.MatchSetupHole && type != Type.MatchSetupPlayer) || !TryGetComponent<ReorderableListElement>(out var component2))
		{
			return;
		}
		component2.OnAssignedToParent += delegate
		{
			if (!ShouldSupressSfx())
			{
				RuntimeManager.PlayOneShot(GameManager.AudioSettings.MatchSetupReorderableAssigned);
			}
		};
		component2.OnSelected += delegate
		{
			if (!ShouldSupressSfx())
			{
				RuntimeManager.PlayOneShot(GameManager.AudioSettings.MatchSetupReorderableSelect);
			}
		};
	}

	private void OnEnable()
	{
		onEnableTime = Time.time;
	}

	private bool ShouldSupressSfx()
	{
		if (menuNavigation != null && !menuNavigation.IsAtTopOfStack())
		{
			return true;
		}
		if (selectable != null && !selectable.IsEffectivelyInteractable())
		{
			return true;
		}
		return Time.time - onEnableTime < 0.1f;
	}

	public void PlayHoverSfx(bool fromGamepad)
	{
		if (ShouldSupressSfx())
		{
			return;
		}
		switch (type)
		{
		case Type.MainMenuButton:
			RuntimeManager.PlayOneShot(GameManager.AudioSettings.MainMenuHover);
			return;
		case Type.PauseMenuButton:
			RuntimeManager.PlayOneShot(GameManager.AudioSettings.PauseMenuHover);
			return;
		case Type.CosmeticsButton:
			RuntimeManager.PlayOneShot(GameManager.AudioSettings.CosmeticsButtonHover);
			return;
		}
		if (fromGamepad)
		{
			RuntimeManager.PlayOneShot(GameManager.AudioSettings.GenericControllerHover);
		}
	}

	public void PlaySelectSfx(bool fromGamepad)
	{
		if (ShouldSupressSfx())
		{
			return;
		}
		switch (type)
		{
		case Type.MainMenuButton:
			RuntimeManager.PlayOneShot(GameManager.AudioSettings.MainMenuSelect);
			break;
		case Type.PauseMenuButton:
			RuntimeManager.PlayOneShot(GameManager.AudioSettings.PauseMenuSelect);
			break;
		case Type.GenericButton:
			RuntimeManager.PlayOneShot(GameManager.AudioSettings.GenericButtonSelect);
			break;
		case Type.SettingsTab:
			RuntimeManager.PlayOneShot(GameManager.AudioSettings.SettingsTabSelect);
			break;
		case Type.MatchSetupTab:
		case Type.CosmeticsTab:
			RuntimeManager.PlayOneShot(GameManager.AudioSettings.MatchSetupTabSelect);
			break;
		case Type.CosmeticsButton:
			if (GetComponentInParent<PlayerCustomizationCosmeticButton>().isUnlocked)
			{
				RuntimeManager.PlayOneShot(GameManager.AudioSettings.CosmeticsButtonSelect);
			}
			else
			{
				RuntimeManager.PlayOneShot(GameManager.AudioSettings.CosmeticsButtonSelectDisabled);
			}
			break;
		case Type.Dropdown:
			RuntimeManager.PlayOneShot(GameManager.AudioSettings.DropdownOpen);
			break;
		case Type.DropdownOption:
			RuntimeManager.PlayOneShot(GameManager.AudioSettings.DropdownOptionSelect);
			break;
		case Type.MatchSetupHole:
		case Type.MatchSetupPlayer:
		case Type.StartMatch:
			break;
		}
	}

	private void OnControllerSelectableSelected()
	{
		PlayHoverSfx(fromGamepad: true);
	}

	private void OnControllerSelectableSubmitted()
	{
		PlaySelectSfx(fromGamepad: true);
	}
}
