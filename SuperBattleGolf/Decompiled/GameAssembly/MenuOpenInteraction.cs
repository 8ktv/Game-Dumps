using UnityEngine;
using UnityEngine.Localization;

public class MenuOpenInteraction : MonoBehaviour, IInteractable
{
	public enum Menu
	{
		None,
		PlayerCustomize,
		MatchSetup
	}

	[SerializeField]
	private Menu menuToOpen;

	[SerializeField]
	private LocalizedString interactString;

	private WorldspaceIconUi worldspaceIcon;

	public Entity AsEntity { get; private set; }

	public bool IsInteractionEnabled
	{
		get
		{
			if (menuToOpen > Menu.None)
			{
				return !IsMenuOpen();
			}
			return false;
		}
	}

	public LocalizedString InteractString => interactString;

	private void Awake()
	{
		AsEntity = GetComponent<Entity>();
		AsEntity.WillBeDestroyed += OnWillBeDestroyed;
		if (menuToOpen == Menu.PlayerCustomize || menuToOpen == Menu.MatchSetup)
		{
			TutorialObjectiveUi.ObjectiveTextUpdated += OnObjectiveTextUpdated;
			GameManager.LocalPlayerRegistered += OnLocalPlayerRegistered;
		}
	}

	private void Start()
	{
		UpdateWorldspaceIcon();
	}

	private void OnWillBeDestroyed()
	{
		HideWorldspaceIcon();
		AsEntity.WillBeDestroyed -= OnWillBeDestroyed;
		if (menuToOpen == Menu.PlayerCustomize || menuToOpen == Menu.MatchSetup)
		{
			TutorialObjectiveUi.ObjectiveTextUpdated -= OnObjectiveTextUpdated;
			GameManager.LocalPlayerRegistered -= OnLocalPlayerRegistered;
		}
	}

	public void LocalPlayerInteract()
	{
		StopAllCoroutines();
		switch (menuToOpen)
		{
		case Menu.PlayerCustomize:
			SingletonBehaviour<PlayerCustomizationMenu>.Instance.SetEnabled(enabled: true, fromInteraction: true);
			break;
		case Menu.MatchSetup:
			SingletonNetworkBehaviour<MatchSetupMenu>.Instance.SetEnabled(enabled: true);
			break;
		}
	}

	private bool IsMenuOpen()
	{
		if (menuToOpen == Menu.PlayerCustomize)
		{
			if (SingletonBehaviour<PlayerCustomizationMenu>.HasInstance)
			{
				return SingletonBehaviour<PlayerCustomizationMenu>.Instance.menu.activeSelf;
			}
			return false;
		}
		return false;
	}

	private void UpdateWorldspaceIcon()
	{
		bool flag = worldspaceIcon != null;
		bool flag2 = ShouldHaveIcon();
		if (flag2 != flag)
		{
			if (flag2)
			{
				ShowIcon();
			}
			else
			{
				HideWorldspaceIcon();
			}
		}
		bool ShouldHaveIcon()
		{
			if (menuToOpen == Menu.PlayerCustomize)
			{
				return TutorialObjectiveUi.DisplayedObjective == TutorialObjective.CustomizeAppearance;
			}
			if (menuToOpen == Menu.MatchSetup)
			{
				return TutorialObjectiveUi.DisplayedObjective == TutorialObjective.StartMatch;
			}
			return false;
		}
		void ShowIcon()
		{
			worldspaceIcon = WorldspaceIconManager.GetUnusedIcon();
			worldspaceIcon.Initialize(WorldspaceIconManager.ObjectiveIconSettings, AsEntity.TargetReticlePosition.transform, GetWorldspaceIconDistanceReference(), WorldspaceIconManager.ObjectiveIcon);
		}
	}

	private void HideWorldspaceIcon()
	{
		if (!(worldspaceIcon == null))
		{
			WorldspaceIconManager.ReturnIcon(worldspaceIcon);
			worldspaceIcon = null;
		}
	}

	private void UpdateWorldspaceIconDistanceReference()
	{
		if (worldspaceIcon != null)
		{
			worldspaceIcon.SetDistanceReference(GetWorldspaceIconDistanceReference());
		}
	}

	private Transform GetWorldspaceIconDistanceReference()
	{
		if (GameManager.LocalPlayerInfo == null)
		{
			return null;
		}
		return GameManager.LocalPlayerInfo.transform;
	}

	private void OnObjectiveTextUpdated()
	{
		UpdateWorldspaceIcon();
	}

	private void OnLocalPlayerRegistered()
	{
		UpdateWorldspaceIconDistanceReference();
	}
}
