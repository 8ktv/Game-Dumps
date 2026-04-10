using DG.Tweening;
using DG.Tweening.Core;
using DG.Tweening.Plugins.Options;
using Mirror;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class PauseMenuPlayerEntry : MonoBehaviour, IPointerEnterHandler, IEventSystemHandler, IPointerExitHandler
{
	private const float maxPlayerVolume = 3f;

	public Sprite defaultIcon;

	public Image playerIcon;

	public TMP_Text playerName;

	public Image background;

	public Color localPlayerBackgroundColor;

	public GameObject leaderIcon;

	public SliderOption volumeSlider;

	public ControllerSelectable volumeSelectable;

	public Toggle muteToggle;

	public Image muteHighlight;

	public ControllerSelectable muteSelectable;

	public Button kickButton;

	public ControllerSelectable kickSelectable;

	public Sprite mutedSprite;

	public Sprite unmutedSprite;

	public Sprite mutedHighlightSprite;

	public Sprite unmuteHighlightSprite;

	public RectTransform slideOut1;

	public RectTransform slideOut2;

	public GameObject voiceChatIndicator;

	public VoiceChatVfx voiceChatVfx;

	private bool isAnySelectableSelected;

	private bool isHoveredOverByPointer;

	private bool areSlideoutsExtended;

	private Color defaultBackgroundColor;

	private float maxSlideout1SlideAmount;

	private float maxSlideout2SlideAmount;

	private bool wasAssignedLocalPlayer;

	private CourseManager.PlayerState playerState;

	private double activeTimeSeconds;

	private float activeTimeUpdateTimer;

	private bool shouldUpdateActiveTime;

	public ulong CurrentPlayerGuid { get; private set; }

	private bool IsLocalPlayer => CurrentPlayerGuid == BNetworkManager.LocalPlayerGuidOnServer;

	private void Awake()
	{
		defaultBackgroundColor = background.color;
		maxSlideout1SlideAmount = GetSlideAmount(slideOut1);
		maxSlideout2SlideAmount = GetSlideAmount(slideOut2);
		SetSlideAmount(slideOut1, 0f);
		SetSlideAmount(slideOut2, 0f);
		Navigation navigation = volumeSlider.navigation;
		navigation.selectOnLeft = PauseMenu.ResumeButton;
		volumeSlider.navigation = navigation;
		volumeSelectable.IsSelectedChanged += OnAnySelectableIsSelectedChanged;
		muteSelectable.IsSelectedChanged += OnAnySelectableIsSelectedChanged;
		kickSelectable.IsSelectedChanged += OnAnySelectableIsSelectedChanged;
		InputManager.SwitchedInputDeviceType += OnSwitchedInputDeviceType;
		SingletonBehaviour<PauseMenu>.Instance.playerActiveTimeTooltip.HoverChanged += OnHoverChanged;
	}

	private void OnDisable()
	{
		DOTween.Kill(this);
		SetSlideAmount(slideOut1, 0f);
		SetSlideAmount(slideOut2, 0f);
	}

	private void OnDestroy()
	{
		volumeSelectable.IsSelectedChanged -= OnAnySelectableIsSelectedChanged;
		muteSelectable.IsSelectedChanged -= OnAnySelectableIsSelectedChanged;
		kickSelectable.IsSelectedChanged -= OnAnySelectableIsSelectedChanged;
		if (wasAssignedLocalPlayer)
		{
			GameSettings.AudioSettings.MicInputVolumeChanged -= OnMicInputVolumeChanged;
		}
		InputManager.SwitchedInputDeviceType -= OnSwitchedInputDeviceType;
		if (SingletonBehaviour<PauseMenu>.Instance != null)
		{
			SingletonBehaviour<PauseMenu>.Instance.playerActiveTimeTooltip.HoverChanged -= OnHoverChanged;
		}
	}

	private void Update()
	{
		PlayerInfo playerInfo;
		bool flag = GameManager.TryFindPlayerByGuid(CurrentPlayerGuid, out playerInfo) && playerInfo.VoiceChat.voiceNetworker.IsTalking;
		voiceChatIndicator.SetActive(flag);
		voiceChatVfx.SetPlaying(flag);
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
			SingletonBehaviour<PauseMenu>.Instance.playerActiveTimeTooltip.OverrideText(playerName.rectTransform, GetActiveTimeTooltip());
		}
	}

	private void UpdateMutedToggle(bool isOn)
	{
		UpdateMutedToggleSprite();
		PlayerVoiceChat.SetIsPlayerMuted(CurrentPlayerGuid, isOn);
	}

	private void UpdateMutedToggleSprite()
	{
		muteToggle.GetComponent<Image>().sprite = (muteToggle.isOn ? mutedSprite : unmutedSprite);
		muteHighlight.sprite = (muteToggle.isOn ? mutedHighlightSprite : unmuteHighlightSprite);
	}

	public void AssignPlayer(CourseManager.PlayerState playerState)
	{
		this.playerState = playerState;
		if (CurrentPlayerGuid == playerState.playerGuid)
		{
			RefreshMutedToggle();
			UpdateTooltips();
			return;
		}
		if (wasAssignedLocalPlayer)
		{
			GameSettings.AudioSettings.MicInputVolumeChanged -= OnMicInputVolumeChanged;
		}
		CurrentPlayerGuid = playerState.playerGuid;
		playerName.text = GameManager.RichTextNoParse(playerState.name);
		playerIcon.sprite = PlayerIconManager.GetPlayerIcon(playerState.playerGuid, PlayerIconManager.IconSize.Medium);
		wasAssignedLocalPlayer = IsLocalPlayer;
		if (IsLocalPlayer)
		{
			background.color = localPlayerBackgroundColor;
			volumeSlider.Initialize(delegate
			{
				OnSetOwnVolume();
			}, GameSettings.All.Audio.MicInputVolume);
			GameSettings.AudioSettings.MicInputVolumeChanged += OnMicInputVolumeChanged;
		}
		else
		{
			background.color = defaultBackgroundColor;
			volumeSlider.Initialize(delegate
			{
				PlayerVoiceChat.SetPlayerVolume(CurrentPlayerGuid, volumeSlider.SetPercentageValue(3f, force1ToMiddle: true));
			}, SliderOption.RemapValueSliderValueMiddleLinear(PlayerVoiceChat.GetPlayerStatus(CurrentPlayerGuid).volume, 0f, 3f, 1f));
		}
		RefreshMutedToggle();
		kickButton.gameObject.SetActive(value: true);
		kickButton.interactable = !IsLocalPlayer && !playerState.isHost && VoteKickManager.CanKickOrVotekick;
		UpdateTooltips();
		leaderIcon.SetActive(playerState.isHost);
		void OnSetOwnVolume()
		{
			if (volumeSlider.SetPercentageValue() != GameSettings.All.Audio.MicInputVolume)
			{
				GameSettings.All.Audio.MicInputVolume = volumeSlider.SetPercentageValue();
				PauseMenu.InformGameSettingsChanged();
			}
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
			UpdateMutedToggleSprite();
			muteToggle.onValueChanged.AddListener(UpdateMutedToggle);
		}
		void UpdateHostTooltip()
		{
			if (playerState.isHost)
			{
				SingletonBehaviour<PauseMenu>.Instance.tooltip.DeregisterTooltip(leaderIcon.GetComponent<RectTransform>());
				string pAUSE_Tooltip_Host = Localization.UI.PAUSE_Tooltip_Host;
				if (!string.IsNullOrEmpty(pAUSE_Tooltip_Host))
				{
					SingletonBehaviour<PauseMenu>.Instance.tooltip.RegisterTooltip(leaderIcon.GetComponent<RectTransform>(), pAUSE_Tooltip_Host);
				}
			}
		}
		void UpdateKickTooltip()
		{
			SingletonBehaviour<PauseMenu>.Instance.warningTooltip.DeregisterTooltip(kickButton.image.rectTransform);
			string text = (IsLocalPlayer ? Localization.UI.MATCHSETUP_Tooltip_CantKickSelf : (playerState.isHost ? Localization.UI.MATCHSETUP_Tooltip_CantKickHost : (VoteKickManager.CanVotekick ? Localization.UI.MATCHSETUP_Tooltip_Votekick : ((!VoteKickManager.CanKick) ? string.Empty : Localization.UI.MATCHSETUP_Tooltip_Kick))));
			if (!string.IsNullOrEmpty(text))
			{
				SingletonBehaviour<PauseMenu>.Instance.warningTooltip.RegisterTooltip(kickButton.image.rectTransform, text);
			}
			kickButton.onClick.RemoveAllListeners();
			if (VoteKickManager.CanKickOrVotekick)
			{
				kickButton.onClick.AddListener(KickPlayer);
			}
		}
		void UpdateMuteTooltip()
		{
			SingletonBehaviour<PauseMenu>.Instance.warningTooltip.DeregisterTooltip(muteToggle.GetComponent<RectTransform>());
			string text = ((!IsLocalPlayer) ? Localization.UI.PAUSE_Tooltip_Mute : Localization.UI.PAUSE_Tooltip_MuteSelf);
			if (!string.IsNullOrEmpty(text))
			{
				SingletonBehaviour<PauseMenu>.Instance.warningTooltip.RegisterTooltip(muteToggle.GetComponent<RectTransform>(), text);
			}
		}
		void UpdateTooltips()
		{
			UpdateKickTooltip();
			UpdateMuteTooltip();
			UpdateHostTooltip();
			UpdateVoiceVolumeTooltip();
			UpdateActiveTimeTooltip();
		}
		void UpdateVoiceVolumeTooltip()
		{
			SingletonBehaviour<PauseMenu>.Instance.tooltip.DeregisterTooltip(volumeSlider.GetComponent<RectTransform>());
			string pAUSE_Tooltip_VoiceChatVolume = Localization.UI.PAUSE_Tooltip_VoiceChatVolume;
			if (!string.IsNullOrEmpty(pAUSE_Tooltip_VoiceChatVolume))
			{
				SingletonBehaviour<PauseMenu>.Instance.tooltip.RegisterTooltip(volumeSlider.GetComponent<RectTransform>(), pAUSE_Tooltip_VoiceChatVolume);
			}
		}
	}

	private void UpdateActiveTimeTooltip()
	{
		SingletonBehaviour<PauseMenu>.Instance.playerActiveTimeTooltip.DeregisterTooltip(playerName.rectTransform);
		string activeTimeTooltip = GetActiveTimeTooltip();
		if (!string.IsNullOrEmpty(activeTimeTooltip))
		{
			SingletonBehaviour<PauseMenu>.Instance.playerActiveTimeTooltip.RegisterTooltip(playerName.rectTransform, activeTimeTooltip);
		}
	}

	private string GetActiveTimeTooltip()
	{
		string arg = Mathf.FloorToInt((float)(activeTimeSeconds / 60.0)).ToString("D2");
		string arg2 = Mathf.FloorToInt((float)(activeTimeSeconds % 60.0)).ToString("D2");
		return string.Format(Localization.UI.PAUSE_Tooltip_ActiveTime, arg, arg2);
	}

	public void SetVerticalNavigationTarget(PauseMenuPlayerEntry targetEntry, bool isUp)
	{
		Navigation navigation = volumeSlider.navigation;
		Selectable slider = targetEntry.volumeSlider.Slider;
		if (isUp)
		{
			navigation.selectOnUp = slider;
		}
		else
		{
			navigation.selectOnDown = slider;
		}
		volumeSlider.navigation = navigation;
		navigation = muteToggle.navigation;
		slider = targetEntry.muteToggle;
		if (isUp)
		{
			navigation.selectOnUp = slider;
		}
		else
		{
			navigation.selectOnDown = slider;
		}
		muteToggle.navigation = navigation;
		navigation = kickButton.navigation;
		slider = (targetEntry.kickButton.gameObject.activeSelf ? ((Selectable)targetEntry.kickButton) : ((Selectable)targetEntry.muteToggle));
		if (isUp)
		{
			navigation.selectOnUp = slider;
		}
		else
		{
			navigation.selectOnDown = slider;
		}
		kickButton.navigation = navigation;
	}

	public void SetVerticalNavigationTarget(Selectable target, bool isUp)
	{
		Navigation navigation = volumeSlider.navigation;
		if (isUp)
		{
			navigation.selectOnUp = target;
		}
		else
		{
			navigation.selectOnDown = target;
		}
		volumeSlider.navigation = navigation;
		navigation = muteToggle.navigation;
		if (isUp)
		{
			navigation.selectOnUp = target;
		}
		else
		{
			navigation.selectOnDown = target;
		}
		muteToggle.navigation = navigation;
		navigation = kickButton.navigation;
		if (isUp)
		{
			navigation.selectOnUp = target;
		}
		else
		{
			navigation.selectOnDown = target;
		}
		kickButton.navigation = navigation;
	}

	private void KickPlayer()
	{
		if (VoteKickManager.CanKickOrVotekick && !IsLocalPlayer)
		{
			if (!GameManager.TryFindPlayerByGuid(CurrentPlayerGuid, out var playerInfo))
			{
				Debug.LogError("Invalid player guid on pause menu player entry");
			}
			else
			{
				VoteKickManager.BeginKick(playerInfo);
			}
		}
	}

	private void UpdateAreSlideoutsExtended()
	{
		bool flag = areSlideoutsExtended;
		areSlideoutsExtended = (InputManager.UsingKeyboard ? isHoveredOverByPointer : isAnySelectableSelected);
		if (areSlideoutsExtended != flag)
		{
			float endValue;
			float endValue2;
			if (areSlideoutsExtended)
			{
				endValue = maxSlideout1SlideAmount;
				endValue2 = maxSlideout2SlideAmount;
			}
			else
			{
				endValue = 0f;
				endValue2 = 0f;
			}
			DOTween.Kill(this);
			TweenerCore<float, float, FloatOptions> t = DOTween.To(() => GetSlideAmount(slideOut1), delegate(float amount)
			{
				SetSlideAmount(slideOut1, amount);
			}, endValue, 0.2f).SetEase(Ease.OutCubic).SetTarget(this);
			TweenerCore<float, float, FloatOptions> t2 = DOTween.To(() => GetSlideAmount(slideOut2), delegate(float amount)
			{
				SetSlideAmount(slideOut2, amount);
			}, endValue2, 0.2f).SetEase(Ease.OutCubic).SetTarget(this);
			if (areSlideoutsExtended)
			{
				t2.SetDelay(0.05f);
			}
			else
			{
				t.SetDelay(0.075f);
			}
		}
	}

	public void OnPointerEnter(PointerEventData eventData)
	{
		isHoveredOverByPointer = true;
		UpdateAreSlideoutsExtended();
	}

	public void OnPointerExit(PointerEventData eventData)
	{
		isHoveredOverByPointer = false;
		UpdateAreSlideoutsExtended();
	}

	private void OnAnySelectableIsSelectedChanged()
	{
		isAnySelectableSelected = volumeSelectable.IsSelected || muteSelectable.IsSelected || kickSelectable.IsSelected;
		UpdateAreSlideoutsExtended();
	}

	private void OnMicInputVolumeChanged()
	{
		float micInputVolume = GameSettings.All.Audio.MicInputVolume;
		volumeSlider.valueWithoutNotify = micInputVolume;
		volumeSlider.SetPercentageValue(1f, force1ToMiddle: false, snapOnKeyboard: false);
	}

	private void OnSwitchedInputDeviceType()
	{
		UpdateAreSlideoutsExtended();
	}

	private void OnHoverChanged(bool isHovering)
	{
		shouldUpdateActiveTime = isHovering;
		UpdateActiveTime(force: true);
	}

	private float GetSlideAmount(RectTransform slideout)
	{
		return slideout.sizeDelta.x;
	}

	private void SetSlideAmount(RectTransform slideout, float amount)
	{
		slideout.anchoredPosition = new Vector2((0f - amount) / 2f, slideout.anchoredPosition.y);
		slideout.sizeDelta = new Vector2(amount, 0f);
	}
}
