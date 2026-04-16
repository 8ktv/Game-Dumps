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

	private CourseManager.PlayerState playerState;

	private double activeTimeSeconds;

	private float activeTimeUpdateTimer;

	private bool shouldUpdateActiveTime;

	private bool wasNewInLobby;

	public ulong Guid => currentPlayerGuid;

	private bool IsLocalPlayer => currentPlayerGuid == BNetworkManager.LocalPlayerGuidOnServer;

	private float TimeSinceJoined => (float)(NetworkTime.time - playerState.joinTimestamp);

	private bool IsNewInLobby
	{
		get
		{
			if (!playerState.isHost)
			{
				return TimeSinceJoined <= (float)VoteKickManager.VoteKickPlayerMinActiveTime;
			}
			return false;
		}
	}

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
		kickButton.onClick.AddListener(KickPlayer);
	}

	private void OnEnable()
	{
		SingletonNetworkBehaviour<MatchSetupMenu>.Instance.activeTimeTooltip.HoverChanged += OnHoverChanged;
		PlayerId.LocalPlayerGuidChanged += OnLocalPlayerGuidChanged;
		MatchSetupMenu.LobbyModeChanged += OnLobbyModeChanged;
		UpdateKickTooltip();
	}

	private void OnDisable()
	{
		SingletonNetworkBehaviour<MatchSetupMenu>.Instance.activeTimeTooltip.HoverChanged -= OnHoverChanged;
		PlayerId.LocalPlayerGuidChanged -= OnLocalPlayerGuidChanged;
		MatchSetupMenu.LobbyModeChanged -= OnLobbyModeChanged;
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
		if (wasNewInLobby && !IsNewInLobby)
		{
			UpdateKickTooltip();
			wasNewInLobby = false;
		}
	}

	private void LateUpdate()
	{
		if (shouldUpdateActiveTime)
		{
			UpdateActiveTime(force: false);
		}
	}

	private void UpdateActiveTime(bool force)
	{
		activeTimeSeconds = NetworkTime.time - playerState.joinTimestamp;
		activeTimeUpdateTimer += Time.deltaTime;
		if (activeTimeUpdateTimer >= 1f || force)
		{
			activeTimeUpdateTimer = 0f;
			SingletonNetworkBehaviour<MatchSetupMenu>.Instance.activeTimeTooltip.OverrideText(playerNickname.rectTransform, GetActiveTimeTooltip());
		}
	}

	private void UpdateActiveTimeTooltip()
	{
		SingletonNetworkBehaviour<MatchSetupMenu>.Instance.activeTimeTooltip.DeregisterTooltip(playerNickname.rectTransform);
		string activeTimeTooltip = GetActiveTimeTooltip();
		if (!string.IsNullOrEmpty(activeTimeTooltip))
		{
			SingletonNetworkBehaviour<MatchSetupMenu>.Instance.activeTimeTooltip.RegisterTooltip(playerNickname.rectTransform, activeTimeTooltip);
		}
	}

	private string GetActiveTimeTooltip()
	{
		string arg = Mathf.FloorToInt((float)(activeTimeSeconds / 60.0)).ToString("D2");
		string arg2 = Mathf.FloorToInt((float)(activeTimeSeconds % 60.0)).ToString("D2");
		return string.Format(Localization.UI.PAUSE_Tooltip_ActiveTime, arg, arg2);
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
		kickButtonPrompt.SetActive(!hidden);
		muteButtonPrompt.SetActive(!hidden);
	}

	public void AssignPlayer(CourseManager.PlayerState playerState)
	{
		if (currentPlayerGuid == playerState.playerGuid)
		{
			RefreshMutedToggle();
			UpdateTooltips();
			return;
		}
		currentPlayerGuid = playerState.playerGuid;
		this.playerState = playerState;
		wasNewInLobby = IsNewInLobby;
		leaderIcon.SetActive(playerState.isHost);
		playerNickname.text = string.Empty;
		playerNickname.text = GameManager.RichTextNoParse(playerState.name);
		GetComponent<ReorderableListElement>().Button.interactable = IsLocalPlayer;
		SetButtonsHidden(hidden: true);
		playerIcon.sprite = PlayerIconManager.GetPlayerIcon(playerState.playerGuid, PlayerIconManager.IconSize.Medium);
		RefreshMutedToggle();
		UpdateTooltips();
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
		void UpdateHostTooltip()
		{
			if (playerState.isHost)
			{
				SingletonNetworkBehaviour<MatchSetupMenu>.Instance.compactTooltip.DeregisterTooltip(leaderIcon.GetComponent<RectTransform>());
				string pAUSE_Tooltip_Host = Localization.UI.PAUSE_Tooltip_Host;
				if (!string.IsNullOrEmpty(pAUSE_Tooltip_Host))
				{
					SingletonNetworkBehaviour<MatchSetupMenu>.Instance.compactTooltip.RegisterTooltip(leaderIcon.GetComponent<RectTransform>(), pAUSE_Tooltip_Host);
				}
			}
		}
		void UpdateMuteTooltip()
		{
			SingletonNetworkBehaviour<MatchSetupMenu>.Instance.warningTooltip.DeregisterTooltip(muteToggle.GetComponent<RectTransform>());
			string text = ((!IsLocalPlayer) ? Localization.UI.PAUSE_Tooltip_Mute : Localization.UI.PAUSE_Tooltip_MuteSelf);
			if (!string.IsNullOrEmpty(text))
			{
				SingletonNetworkBehaviour<MatchSetupMenu>.Instance.warningTooltip.RegisterTooltip(muteToggle.GetComponent<RectTransform>(), text);
			}
		}
		void UpdateTooltips()
		{
			UpdateKickTooltip();
			UpdateMuteTooltip();
			UpdateHostTooltip();
			UpdateActiveTimeTooltip();
		}
	}

	private void UpdateKickTooltip()
	{
		SingletonNetworkBehaviour<MatchSetupMenu>.Instance.warningTooltip.DeregisterTooltip(kickButton.image.rectTransform);
		string text = (IsLocalPlayer ? Localization.UI.MATCHSETUP_Tooltip_CantKickSelf : (playerState.isHost ? Localization.UI.MATCHSETUP_Tooltip_CantKickHost : (VoteKickManager.CanKickPlayerImmediately(currentPlayerGuid) ? Localization.UI.MATCHSETUP_Tooltip_Kick : ((!ShouldShowVotekickTooltip()) ? string.Empty : Localization.UI.MATCHSETUP_Tooltip_Votekick))));
		if (!string.IsNullOrEmpty(text))
		{
			SingletonNetworkBehaviour<MatchSetupMenu>.Instance.warningTooltip.RegisterTooltip(kickButton.image.rectTransform, text);
		}
		static bool ShouldShowVotekickTooltip()
		{
			if (GameManager.LocalPlayerId == null)
			{
				return false;
			}
			if (VoteKickManager.CanPlayerInitiateVotekick(GameManager.LocalPlayerId.Guid, out var dueToMinActiveTime))
			{
				return true;
			}
			if (dueToMinActiveTime)
			{
				return true;
			}
			return false;
		}
	}

	private void KickPlayer()
	{
		if (!IsLocalPlayer && (VoteKickManager.CanKickFreely() || VoteKickManager.CanInitiateVotekickAtAll()))
		{
			CourseManager.PlayerState state;
			if (!GameManager.TryFindPlayerByGuid(currentPlayerGuid, out var playerInfo))
			{
				Debug.LogError("Invalid player guid on pause menu player entry");
			}
			else if (CourseManager.TryGetPlayerState(currentPlayerGuid, out state) && !state.isHost)
			{
				VoteKickManager.BeginKick(playerInfo);
			}
		}
	}

	private void OnHoverChanged(bool isHovering)
	{
		shouldUpdateActiveTime = isHovering;
		UpdateActiveTime(force: true);
	}

	private void OnLocalPlayerGuidChanged()
	{
		UpdateKickTooltip();
	}

	private void OnLobbyModeChanged()
	{
		UpdateKickTooltip();
	}
}
