using System;
using System.Collections.Generic;
using FMOD.Studio;
using FMODUnity;
using Mirror;
using Mirror.FizzySteam;
using Steamworks;
using TMPro;
using UnityEngine;
using UnityEngine.Pool;
using UnityEngine.UI;

public class PauseMenu : SingletonBehaviour<PauseMenu>
{
	[SerializeField]
	private PauseMenuPlayerEntry playerEntryPrefab;

	public GameObject menuContainer;

	public GameObject menuButtonContainer;

	public GameObject postProcessing;

	public GameObject inviteButton;

	public GameObject gameVersionLabel;

	public GameObject offlineLabel;

	public GameObject settingsMenu;

	public TMP_Text rulesLabel;

	public TMP_Text courseLabel;

	public int headerLabelBigFontSize = 64;

	public Button resumeButton;

	public Button exitGameButton;

	public Transform playerEntryContainer;

	public MatchSetupMenu matchSetupMenu;

	public MenuNavigation menuNavigation;

	public UiTooltip tooltip;

	public UiTooltip warningTooltip;

	public UiTooltip playerActiveTimeTooltip;

	public float buttonHorizontalOffset = 30f;

	public RectTransform rulesContainer;

	public HorizontalLayoutGroup rulesLayoutGroup;

	public GameObject rulesDummy;

	public GameObject rulesTemplate;

	public GameObject rulesLeftButton;

	public GameObject rulesRightButton;

	public ScrollViewSnap scrollViewSnap;

	public GameObject cheatsWarningMessage;

	public GameObject itemProbabilityTemplate;

	public UiTooltip itemProbabilityTooltip;

	public GameObject rulesCycleRightButtonPrompt;

	public GameObject rulesCycleLeftButtonPrompt;

	private bool isPaused;

	private Button[] allButtons;

	private GameObject[] allRules;

	private GameObject[] allItemProbabilities;

	private string[] itemPoolLocStrings;

	private readonly List<PauseMenuPlayerEntry> playerEntries = new List<PauseMenuPlayerEntry>();

	private bool saveGameSettingsWhenUnpausing;

	private EventInstance pausedSnapshot;

	public static bool IsPaused
	{
		get
		{
			if (SingletonBehaviour<PauseMenu>.HasInstance)
			{
				return SingletonBehaviour<PauseMenu>.Instance.isPaused;
			}
			return false;
		}
	}

	public static Button ResumeButton
	{
		get
		{
			if (!SingletonBehaviour<PauseMenu>.HasInstance)
			{
				return null;
			}
			return SingletonBehaviour<PauseMenu>.Instance.resumeButton;
		}
	}

	public static event Action Paused;

	public static event Action Unpaused;

	protected override void Awake()
	{
		base.Awake();
		menuContainer.SetActive(value: false);
		gameVersionLabel.SetActive(value: false);
		postProcessing.SetActive(value: false);
		allButtons = menuButtonContainer.GetComponentsInChildren<Button>(includeInactive: true);
		CourseManager.PlayerStatesChanged += OnPlayerStatesChanged;
		BNetworkManager.SteamPlayerRelationshipChanged += SteamPlayerRelationshipChanged;
		menuNavigation.OnExitEvent += Unpause;
		allRules = new GameObject[6];
		for (int i = 0; i < allRules.Length; i++)
		{
			allRules[i] = UnityEngine.Object.Instantiate(rulesTemplate, rulesTemplate.transform.parent);
		}
		rulesDummy.transform.SetAsLastSibling();
		rulesTemplate.gameObject.SetActive(value: false);
		allItemProbabilities = new GameObject[GameManager.AllItems.Count];
		for (int j = 0; j < GameManager.AllItems.Count; j++)
		{
			ItemData itemAtIndex = GameManager.AllItems.GetItemAtIndex(j);
			(allItemProbabilities[j] = UnityEngine.Object.Instantiate(itemProbabilityTemplate, itemProbabilityTemplate.transform.parent)).transform.GetChild(1).GetComponent<Image>().sprite = itemAtIndex.Icon;
		}
		for (int k = 0; k < GameManager.AllItems.Count; k++)
		{
			Button component = allItemProbabilities[k].GetComponent<Button>();
			Navigation navigation = component.navigation;
			navigation.mode = Navigation.Mode.Explicit;
			if (k >= 10)
			{
				navigation.selectOnUp = allItemProbabilities[k - 10].GetComponent<Button>();
				navigation.selectOnDown = resumeButton;
			}
			else if (k < 10)
			{
				navigation.selectOnDown = allItemProbabilities[BMath.Min(k + 10, allItemProbabilities.Length - 1)].GetComponent<Button>();
			}
			if (k > 0)
			{
				navigation.selectOnLeft = allItemProbabilities[k - 1].GetComponent<Button>();
			}
			if (k < 10 || (k >= 10 && k < allItemProbabilities.Length - 1))
			{
				navigation.selectOnRight = allItemProbabilities[k + 1].GetComponent<Button>();
			}
			component.navigation = navigation;
		}
		Navigation navigation2 = resumeButton.navigation;
		navigation2.selectOnUp = allItemProbabilities[^1].GetComponent<Button>();
		resumeButton.navigation = navigation2;
		itemProbabilityTemplate.SetActive(value: false);
	}

	public void UpdateItemProbabilites()
	{
		UpdateItemPoolLocStrings();
		for (int i = 0; i < GameManager.AllItems.Count; i++)
		{
			ItemData itemAtIndex = GameManager.AllItems.GetItemAtIndex(i);
			GameObject gameObject = allItemProbabilities[i];
			RectTransform component = gameObject.GetComponent<RectTransform>();
			bool flag = true;
			string text = "<color=#b05d39>" + itemAtIndex.LocalizedName.GetLocalizedString() + "</color>\n";
			int[] array = new int[6] { 1, 2, 3, 4, 5, 0 };
			foreach (int num in array)
			{
				bool flag2 = SingletonNetworkBehaviour<MatchSetupRules>.Instance.IsSpawnChangeDefault(num, itemAtIndex.Type);
				flag = flag && flag2;
				float weight = SingletonNetworkBehaviour<MatchSetupRules>.Instance.GetWeight(num, itemAtIndex.Type);
				float itemPoolTotalWeight = SingletonNetworkBehaviour<MatchSetupRules>.Instance.GetItemPoolTotalWeight(num);
				if (itemPoolTotalWeight > 0f)
				{
					weight /= itemPoolTotalWeight;
					weight *= 100f;
				}
				else
				{
					weight = 0f;
				}
				string text2 = weight.ToString("0") + "%";
				string text3 = ((!flag2) ? ((!(SingletonNetworkBehaviour<MatchSetupRules>.Instance.GetDefaultItemFactor(num, itemAtIndex.Type) * 100f > weight)) ? "#4E9444" : "#DB4654") : ((!(weight > 0f)) ? "#96928F" : "#312F2F"));
				text = text + "<color=" + text3 + ">" + text2 + " " + itemPoolLocStrings[num] + "</color>\n";
			}
			itemProbabilityTooltip.RegisterTooltip(component, text);
			gameObject.transform.GetChild(2).gameObject.SetActive(!flag);
		}
	}

	private void UpdateItemPoolLocStrings()
	{
		if (itemPoolLocStrings == null)
		{
			itemPoolLocStrings = new string[SingletonNetworkBehaviour<MatchSetupRules>.Instance.itemSpawnerSettings.ItemPools.Count + 2];
		}
		bool flag = LocalizationManager.CurrentLanguage == "pl";
		itemPoolLocStrings[0] = (flag ? Localization.UI.MATCHSETUP_Title_AheadOwnBall.ToLower() : Localization.UI.MATCHSETUP_Title_AheadOwnBall);
		for (int i = 0; i < SingletonNetworkBehaviour<MatchSetupRules>.Instance.itemSpawnerSettings.ItemPools.Count; i++)
		{
			ItemSpawnerSettings.ItemPoolData pool = SingletonNetworkBehaviour<MatchSetupRules>.Instance.itemSpawnerSettings.ItemPools[i];
			string text = SingletonNetworkBehaviour<MatchSetupRules>.Instance.GetPoolLocString(pool);
			if (flag)
			{
				text = text.ToLower();
			}
			itemPoolLocStrings[i + 1] = text;
		}
		itemPoolLocStrings[5] = (flag ? Localization.UI.MATCHSETUP_Title_MobilityItemBoxes.ToLower() : Localization.UI.MATCHSETUP_Title_MobilityItemBoxes);
	}

	private void OnRectTransformDimensionsChange()
	{
		if (Application.isPlaying && IsPaused)
		{
			UpdateRules();
		}
	}

	public void UpdateRules()
	{
		string[] array = new string[allRules.Length];
		int num = 0;
		for (int i = 0; i < 21; i++)
		{
			MatchSetupRules.RuleCategory ruleCategory = MatchSetupRules.RuleCategoryLookup[i];
			if (ruleCategory != MatchSetupRules.RuleCategory.Cheats)
			{
				MatchSetupRules.Rule rule = (MatchSetupRules.Rule)i;
				string text = rule switch
				{
					MatchSetupRules.Rule.ConsoleCommands => Localization.UI.MATCHSETUP_Title_ConsoleCommands, 
					MatchSetupRules.Rule.OnOrBelowPar => Localization.UI.MATHCSETUP_Rule_OnOrBelowPar, 
					_ => LocalizationManager.GetString(StringTable.UI, $"MATCHSETUP_Rule_{rule}"), 
				};
				string text2 = array[(int)ruleCategory];
				text2 = text2 + text + " ";
				string text3 = SingletonNetworkBehaviour<MatchSetupRules>.Instance.GetFormattedValue(rule);
				if (SingletonNetworkBehaviour<MatchSetupRules>.Instance.IsDropdown(rule))
				{
					text3 = text3.ToUpper();
				}
				bool flag = MatchSetupRules.IsDefaultValue(rule);
				text2 = text2 + GameManager.UiSettings.ApplyColorTag(text3, (!flag) ? TextHighlight.Red : TextHighlight.Regular) + "\n";
				if (!flag)
				{
					num |= 1 << (int)ruleCategory;
				}
				array[(int)ruleCategory] = text2;
			}
		}
		float size = (rulesContainer.rect.width - (float)rulesLayoutGroup.padding.left - (float)rulesLayoutGroup.padding.right - rulesLayoutGroup.spacing) * 0.5f;
		int num2 = 0;
		for (int j = 0; j < array.Length; j++)
		{
			GameObject gameObject = allRules[j];
			bool flag2 = (num & (1 << j)) != 0;
			gameObject.gameObject.SetActive(flag2);
			if (flag2)
			{
				gameObject.GetComponent<RectTransform>().SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, size);
				gameObject.transform.GetChild(0).GetComponent<TMP_Text>().text = LocalizationManager.GetString(StringTable.UI, $"MATCHSETUP_Title_{(MatchSetupRules.RuleCategory)j}").ToUpper();
				gameObject.transform.GetChild(1).GetComponent<TMP_Text>().text = array[j];
				num2++;
			}
		}
		scrollViewSnap.SetSteps(BMath.CeilToInt((float)num2 / 2f));
		rulesDummy.SetActive(num2 % 2 != 0);
		rulesDummy.GetComponent<RectTransform>().SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, size);
		rulesContainer.gameObject.SetActive(num2 > 0);
		rulesLeftButton.SetActive(num2 > 2);
		rulesRightButton.SetActive(num2 > 2);
		rulesCycleLeftButtonPrompt.SetActive(num2 > 2);
		rulesCycleRightButtonPrompt.SetActive(num2 > 2);
		cheatsWarningMessage.SetActive(MatchSetupRules.GetValueAsBool(MatchSetupRules.Rule.ConsoleCommands));
	}

	private void UpdateButtonOffsets()
	{
		float num = allButtons[0].GetComponent<RectTransform>().sizeDelta.x - buttonHorizontalOffset;
		for (int i = 0; i < allButtons.Length; i++)
		{
			Button button = allButtons[i];
			Navigation navigation = button.navigation;
			if (i == 0)
			{
				navigation.selectOnUp = allItemProbabilities[^1].GetComponent<Button>();
			}
			else
			{
				navigation.selectOnUp = GetButton(i, -1);
			}
			if (i < allButtons.Length - 1)
			{
				navigation.selectOnDown = GetButton(i, 1);
			}
			navigation.mode = Navigation.Mode.Explicit;
			button.navigation = navigation;
			if (i > 0 && button.gameObject.activeSelf)
			{
				button.GetComponent<RectTransform>().SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, num);
				num -= buttonHorizontalOffset;
			}
		}
		Button GetButton(int start, int offset)
		{
			for (int j = start + offset; j >= 0 && j < allButtons.Length; j += offset)
			{
				if (allButtons[j].gameObject.activeSelf && allButtons[j].interactable)
				{
					return allButtons[j];
				}
			}
			return null;
		}
	}

	private void OnEnable()
	{
		OnInternetConnectionChanged();
		OnLobbyStateChanged();
		BNetworkManager.ConnectedToInternet += OnInternetConnectionChanged;
		BNetworkManager.DisconnectedFromInternet += OnInternetConnectionChanged;
		BNetworkManager.LobbyStateChanged += OnLobbyStateChanged;
		LocalizationManager.LanguageChanged += OnLanguageChanged;
		GameSettings.GeneralSettings.DistanceUnitChanged = (Action)Delegate.Combine(GameSettings.GeneralSettings.DistanceUnitChanged, new Action(OnDistanceUnitChanged));
		UpdateButtonOffsets();
	}

	private void OnDisable()
	{
		BNetworkManager.ConnectedToInternet -= OnInternetConnectionChanged;
		BNetworkManager.DisconnectedFromInternet -= OnInternetConnectionChanged;
		BNetworkManager.LobbyStateChanged -= OnLobbyStateChanged;
		LocalizationManager.LanguageChanged -= OnLanguageChanged;
		GameSettings.GeneralSettings.DistanceUnitChanged = (Action)Delegate.Remove(GameSettings.GeneralSettings.DistanceUnitChanged, new Action(OnDistanceUnitChanged));
	}

	protected override void OnDestroy()
	{
		base.OnDestroy();
		UnpauseInternal(forceClose: true);
		if (pausedSnapshot.isValid())
		{
			pausedSnapshot.stop(FMOD.Studio.STOP_MODE.ALLOWFADEOUT);
		}
		CourseManager.PlayerStatesChanged -= OnPlayerStatesChanged;
		BNetworkManager.SteamPlayerRelationshipChanged -= SteamPlayerRelationshipChanged;
	}

	public static void Pause()
	{
		if (SingletonBehaviour<PauseMenu>.HasInstance)
		{
			SingletonBehaviour<PauseMenu>.Instance.PauseInternal();
		}
	}

	public static void Unpause()
	{
		if (SingletonBehaviour<PauseMenu>.HasInstance)
		{
			SingletonBehaviour<PauseMenu>.Instance.UnpauseInternal(forceClose: false);
		}
	}

	public static void ExitUiInput(bool closeSubMenus)
	{
		if (SingletonBehaviour<PauseMenu>.HasInstance)
		{
			SingletonBehaviour<PauseMenu>.Instance.UnpauseInternal(forceClose: false, closeSubMenus);
		}
	}

	public static void InformGameSettingsChanged()
	{
		if (SingletonBehaviour<PauseMenu>.HasInstance)
		{
			SingletonBehaviour<PauseMenu>.Instance.InformGameSettingsChangedInternal();
		}
	}

	public void UpdateGameInfoLabels()
	{
		string text = MatchSetupMenu.GetCourseLocalizedString(SingletonNetworkBehaviour<MatchSetupMenu>.Instance.activeCourse).GetLocalizedString();
		if (SingletonNetworkBehaviour<MatchSetupMenu>.Instance.randomEnabled)
		{
			text = text + " " + Localization.UI.MATCHSETUP_Courses_Random;
		}
		string localizedString = SingletonNetworkBehaviour<MatchSetupMenu>.Instance.rules.rulesLabelLobby.StringReference.GetLocalizedString();
		courseLabel.text = string.Format(Localization.UI.PAUSE_Course, $"<u><size={headerLabelBigFontSize}>{text}</u></size>");
		rulesLabel.text = string.Format(Localization.UI.PAUSE_Rules, $"<u><size={headerLabelBigFontSize}>{localizedString}</u></size>");
	}

	private void OnLanguageChanged()
	{
		UpdateGameInfoLabels();
		UpdateRules();
		UpdateItemProbabilites();
	}

	public void ExitToMainMenu()
	{
		GameManager.ExitToMainMenu(showConfirmation: true, OnUnpaused);
	}

	private void PauseInternal()
	{
		if (!isPaused)
		{
			isPaused = true;
			UpdateInviteButtonEnabled();
			menuContainer.SetActive(value: true);
			gameVersionLabel.SetActive(value: true);
			postProcessing.SetActive(value: true);
			UpdatePlayerEntries();
			UpdateGameInfoLabels();
			UpdateRules();
			UpdateItemProbabilites();
			InputManager.EnableMode(InputMode.Paused);
			GameManager.HideUiGroup(UiHidingGroup.Paused);
			pausedSnapshot = RuntimeManager.CreateInstance(GameManager.AudioSettings.UnderwaterCameraSnapshot);
			pausedSnapshot.start();
			pausedSnapshot.release();
			PauseMenu.Paused?.Invoke();
		}
	}

	private void UnpauseInternal(bool forceClose, bool closeSubMenus = true)
	{
		if (!isPaused)
		{
			return;
		}
		if (pausedSnapshot.isValid())
		{
			pausedSnapshot.stop(FMOD.Studio.STOP_MODE.ALLOWFADEOUT);
		}
		if (settingsMenu.activeSelf)
		{
			if (closeSubMenus)
			{
				settingsMenu.SetActive(value: false);
			}
			if (!forceClose)
			{
				return;
			}
		}
		if (MatchSetupMenu.IsActive)
		{
			if (closeSubMenus)
			{
				matchSetupMenu.SetEnabled(enabled: false);
			}
			if (!forceClose)
			{
				return;
			}
		}
		isPaused = false;
		menuContainer.SetActive(value: false);
		gameVersionLabel.SetActive(value: false);
		OnUnpaused();
		PauseMenu.Unpaused?.Invoke();
	}

	private void InformGameSettingsChangedInternal()
	{
		if (isPaused)
		{
			saveGameSettingsWhenUnpausing = true;
		}
	}

	public void UpdateInviteButtonEnabled()
	{
		inviteButton.SetActive(CanInvite());
		UpdateButtonOffsets();
		static bool CanInvite()
		{
			if (!(BNetworkManager.singleton.transport is FizzyFacepunch))
			{
				return false;
			}
			if (!BNetworkManager.IsAcceptingConnections)
			{
				if (!NetworkServer.active)
				{
					return NetworkClient.active;
				}
				return false;
			}
			return true;
		}
	}

	private void OnUnpaused()
	{
		postProcessing.SetActive(value: false);
		if (saveGameSettingsWhenUnpausing)
		{
			saveGameSettingsWhenUnpausing = false;
			GameSettings.ApplyAndSave();
		}
		InputManager.DisableMode(InputMode.Paused);
		GameManager.UnhideUiGroup(UiHidingGroup.Paused);
	}

	public void InviteFriend()
	{
		if (!BNetworkManager.IsOffline && BNetworkManager.TryGetSteamLobby(out var lobby))
		{
			SteamFriends.OpenGameInviteOverlay(lobby.Id);
		}
	}

	public void UpdatePlayerEntries()
	{
		if (!isPaused || BNetworkManager.IsChangingSceneOrShuttingDown)
		{
			return;
		}
		try
		{
			List<CourseManager.PlayerState> value;
			using (CollectionPool<List<CourseManager.PlayerState>, CourseManager.PlayerState>.Get(out value))
			{
				CourseManager.GetConnectedPlayerStates(value);
				int num = value.Count - playerEntries.Count;
				for (int i = 0; i < num; i++)
				{
					PauseMenuPlayerEntry pauseMenuPlayerEntry = UnityEngine.Object.Instantiate(playerEntryPrefab);
					pauseMenuPlayerEntry.transform.SetParent(playerEntryContainer);
					pauseMenuPlayerEntry.transform.localScale = Vector3.one;
					if (!value[i].isHost)
					{
						pauseMenuPlayerEntry.NewPlayerIndicatorDisabled += UpdatePlayerEntries;
					}
					playerEntries.Add(pauseMenuPlayerEntry);
				}
				ulong selectedVolumePlayerGuid = 0uL;
				ulong selectedMutePlayerGuid = 0uL;
				ulong selectedKickPlayerGuid = 0uL;
				for (int j = 0; j < playerEntries.Count; j++)
				{
					PauseMenuPlayerEntry pauseMenuPlayerEntry2 = playerEntries[j];
					UpdateSelectedValues(pauseMenuPlayerEntry2);
					bool flag = j < value.Count && value[j].playerGuid != 0 && !string.IsNullOrEmpty(value[j].name);
					pauseMenuPlayerEntry2.gameObject.SetActive(flag);
					if (!flag)
					{
						continue;
					}
					pauseMenuPlayerEntry2.AssignPlayer(value[j]);
					if (j == 0)
					{
						Button[] array = allButtons;
						foreach (Button obj in array)
						{
							Navigation navigation = obj.navigation;
							navigation.selectOnRight = pauseMenuPlayerEntry2.volumeSlider.Slider;
							obj.navigation = navigation;
						}
					}
					else
					{
						PauseMenuPlayerEntry pauseMenuPlayerEntry3 = playerEntries[j - 1];
						pauseMenuPlayerEntry2.SetVerticalNavigationTarget(pauseMenuPlayerEntry3, isUp: true);
						pauseMenuPlayerEntry3.SetVerticalNavigationTarget(pauseMenuPlayerEntry2, isUp: false);
					}
					if (j == value.Count - 1)
					{
						if (playerEntries.Count == 1)
						{
							pauseMenuPlayerEntry2.SetVerticalNavigationTarget(exitGameButton, isUp: true);
							pauseMenuPlayerEntry2.SetVerticalNavigationTarget(resumeButton, isUp: false);
						}
						else
						{
							PauseMenuPlayerEntry pauseMenuPlayerEntry4 = playerEntries[0];
							pauseMenuPlayerEntry2.SetVerticalNavigationTarget(pauseMenuPlayerEntry4, isUp: false);
							pauseMenuPlayerEntry4.SetVerticalNavigationTarget(pauseMenuPlayerEntry2, isUp: true);
						}
					}
				}
				Reselect();
				void Reselect()
				{
					ulong num2 = 0uL;
					if (selectedVolumePlayerGuid != 0L)
					{
						num2 = selectedVolumePlayerGuid;
					}
					else if (selectedMutePlayerGuid != 0L)
					{
						num2 = selectedVolumePlayerGuid;
					}
					else if (selectedKickPlayerGuid != 0L)
					{
						num2 = selectedKickPlayerGuid;
					}
					if (num2 != 0L)
					{
						PauseMenuPlayerEntry pauseMenuPlayerEntry5 = null;
						foreach (PauseMenuPlayerEntry playerEntry in playerEntries)
						{
							if (playerEntry.gameObject.activeSelf && playerEntry.CurrentPlayerGuid == selectedKickPlayerGuid)
							{
								pauseMenuPlayerEntry5 = playerEntry;
								break;
							}
						}
						if (!(pauseMenuPlayerEntry5 == null))
						{
							MenuNavigation componentInParent = GetComponentInParent<MenuNavigation>();
							if (selectedVolumePlayerGuid != 0L)
							{
								componentInParent.Select(pauseMenuPlayerEntry5.volumeSelectable);
							}
							else if (selectedMutePlayerGuid != 0L)
							{
								componentInParent.Select(pauseMenuPlayerEntry5.muteSelectable);
							}
							else if (selectedKickPlayerGuid != 0L)
							{
								componentInParent.Select(pauseMenuPlayerEntry5.kickSelectable);
							}
						}
					}
				}
				void UpdateSelectedValues(PauseMenuPlayerEntry currentPlayerEntry)
				{
					if (selectedVolumePlayerGuid == 0L && selectedMutePlayerGuid == 0L && selectedKickPlayerGuid == 0L)
					{
						if (currentPlayerEntry.volumeSelectable.IsSelected)
						{
							selectedVolumePlayerGuid = currentPlayerEntry.CurrentPlayerGuid;
						}
						else if (currentPlayerEntry.muteSelectable.IsSelected)
						{
							selectedMutePlayerGuid = currentPlayerEntry.CurrentPlayerGuid;
						}
						else if (currentPlayerEntry.kickSelectable.IsSelected)
						{
							selectedKickPlayerGuid = currentPlayerEntry.CurrentPlayerGuid;
						}
					}
				}
			}
		}
		catch (Exception exception)
		{
			Debug.LogError("Encountered exception while updating pause menu player entires. See next log for details", base.gameObject);
			Debug.LogException(exception, base.gameObject);
		}
	}

	public void OpenMatchSetup()
	{
		matchSetupMenu.SetEnabled(enabled: true);
	}

	private void OnInternetConnectionChanged()
	{
		offlineLabel.SetActive(BNetworkManager.IsOffline);
		UpdateInviteButtonEnabled();
	}

	private void OnLobbyStateChanged()
	{
		UpdateInviteButtonEnabled();
	}

	private void SteamPlayerRelationshipChanged(ulong guid, Relationship relationship)
	{
		UpdatePlayerEntries();
	}

	private void OnPlayerStatesChanged(SyncList<CourseManager.PlayerState>.Operation op, int index, CourseManager.PlayerState state)
	{
		if (ShouldUpdatePlayerEntires())
		{
			UpdatePlayerEntries();
		}
		bool ShouldUpdatePlayerEntires()
		{
			if (!isPaused)
			{
				return false;
			}
			if ((uint)op == 1u)
			{
				CourseManager.PlayerState playerState = CourseManager.PlayerStates[index];
				if (playerState.playerGuid == 0L)
				{
					return false;
				}
				if (playerState.isConnected != state.isConnected)
				{
					return true;
				}
				if (playerState.playerGuid != state.playerGuid)
				{
					return true;
				}
				if (playerState.name != state.name)
				{
					return true;
				}
				if (playerState.isHost != state.isHost)
				{
					return true;
				}
				return false;
			}
			return true;
		}
	}

	private void OnDistanceUnitChanged()
	{
		UpdateGameInfoLabels();
		UpdateRules();
		UpdateItemProbabilites();
	}
}
