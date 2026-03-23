using Mirror;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class MatchSetupPlayer : MonoBehaviour, IPointerEnterHandler, IEventSystemHandler, IPointerExitHandler
{
	public MatchSetupMenu menu;

	public Sprite defaultIcon;

	public Image playerIcon;

	public TMP_Text playerNickname;

	public GameObject leaderIcon;

	public Toggle muteToggle;

	public Button kickButton;

	public Sprite mutedSprite;

	public Sprite unmutedSprite;

	public GameObject kickButtonPrompt;

	public GameObject muteButtonPrompt;

	private ulong currentPlayerGuid;

	private ControllerSelectable selectable;

	public ulong Guid => currentPlayerGuid;

	private bool isLocalPlayer => currentPlayerGuid == BNetworkManager.LocalPlayerGuidOnServer;

	private void Awake()
	{
		selectable = GetComponent<ControllerSelectable>();
		selectable.Selected += delegate
		{
			SetButtonsHidden(hidden: false);
		};
		selectable.Deselected += delegate
		{
			SetButtonsHidden(hidden: true);
		};
		selectable.Submitted += OnControllerSubmit;
	}

	private void OnControllerSubmit()
	{
		ReorderableList componentInParent = GetComponentInParent<ReorderableList>();
		ReorderableList target = ((!(componentInParent == menu.playersList)) ? menu.playersList : menu.spectatorsList);
		GetComponent<ReorderableListElement>().AssignTo(target, componentInParent);
	}

	private void Update()
	{
		if (selectable.IsSelected && InputManager.CurrentGamepad != null && !InputManager.CurrentModeMask.HasMode(InputMode.ForceDisabled))
		{
			if (NetworkServer.active && InputManager.CurrentGamepad.buttonNorth.wasPressedThisFrame)
			{
				KickPlayer();
			}
			if (InputManager.CurrentGamepad.buttonWest.wasPressedThisFrame)
			{
				muteToggle.isOn = !muteToggle.isOn;
			}
		}
	}

	private void UpdateMutedToggle(bool isOn)
	{
		PlayerVoiceChat.SetIsPlayerMuted(currentPlayerGuid, isOn);
		UpdateMutedToggleSprite();
	}

	private void UpdateMutedToggleSprite()
	{
		muteToggle.GetComponent<Image>().sprite = (muteToggle.isOn ? mutedSprite : unmutedSprite);
	}

	public void OnPointerEnter(PointerEventData eventData)
	{
		SetButtonsHidden(hidden: false);
	}

	public void OnPointerExit(PointerEventData eventData)
	{
		SetButtonsHidden(hidden: true);
	}

	private void SetButtonsHidden(bool hidden)
	{
		kickButton.targetGraphic.color = (hidden ? Color.clear : Color.white);
		muteToggle.targetGraphic.color = ((hidden && !muteToggle.isOn) ? Color.clear : Color.white);
		kickButtonPrompt.SetActive(!hidden);
		muteButtonPrompt.SetActive(!hidden);
	}

	public void AssignPlayer(CourseManager.PlayerState playerState)
	{
		RefreshMutedToggle();
		if (currentPlayerGuid != playerState.playerGuid)
		{
			currentPlayerGuid = playerState.playerGuid;
			if (NetworkServer.active)
			{
				kickButton.onClick.RemoveAllListeners();
				kickButton.onClick.AddListener(KickPlayer);
				kickButton.gameObject.SetActive(!isLocalPlayer);
			}
			else
			{
				kickButton.gameObject.SetActive(value: false);
			}
			leaderIcon.SetActive(playerState.isHost);
			playerNickname.text = string.Empty;
			playerNickname.text = GameManager.RichTextNoParse(playerState.name);
			GetComponent<ReorderableListElement>().Button.interactable = NetworkServer.active || isLocalPlayer;
			SetButtonsHidden(hidden: true);
			playerIcon.sprite = PlayerIconManager.GetPlayerIcon(playerState.playerGuid, PlayerIconManager.IconSize.Medium);
		}
		void RefreshMutedToggle()
		{
			muteToggle.onValueChanged.RemoveAllListeners();
			if (GameManager.TryFindPlayerByGuid(playerState.playerGuid, out var playerInfo) && playerInfo.IsBlockedOnSteam())
			{
				muteToggle.isOn = true;
				muteToggle.interactable = false;
			}
			else
			{
				muteToggle.isOn = GameSettings.All.General.MuteChat || PlayerVoiceChat.IsMuted(playerState.playerGuid);
				muteToggle.interactable = !GameSettings.All.General.MuteChat;
			}
			muteToggle.onValueChanged.AddListener(UpdateMutedToggle);
			UpdateMutedToggleSprite();
		}
	}

	private void KickPlayer()
	{
		if (!NetworkServer.active || isLocalPlayer)
		{
			return;
		}
		if (!GameManager.TryFindPlayerByGuid(currentPlayerGuid, out var playerInfo))
		{
			Debug.LogError("Invalid player guid on player entry!");
			return;
		}
		FullScreenMessage.Show(string.Format(Localization.UI.MATCHSETUP_KickPlayer, playerInfo.PlayerId.PlayerNameNoRichText), new FullScreenMessage.ButtonEntry(Localization.UI.MISC_Yes, delegate
		{
			BNetworkManager.singleton.ServerKickConnection(playerInfo.connectionToClient);
			FullScreenMessage.Hide();
		}), new FullScreenMessage.ButtonEntry(Localization.UI.MISC_Cancel, FullScreenMessage.Hide));
	}
}
