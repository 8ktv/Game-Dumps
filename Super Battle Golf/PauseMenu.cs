using System;
using System.Collections.Generic;
using FMOD.Studio;
using FMODUnity;
using Mirror;
using Mirror.FizzySteam;
using Steamworks;
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

	public Button resumeButton;

	public Button exitGameButton;

	public Transform playerEntryContainer;

	public MatchSetupMenu matchSetupMenu;

	public MenuNavigation menuNavigation;

	public float buttonHorizontalOffset = 30f;

	private bool isPaused;

	private Button[] allButtons;

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
		PlayerId.AnyPlayerNameChanged += OnAnyPlayerNameChanged;
		PlayerId.AnyPlayerGuidChanged += OnAnyPlayerGuidChanged;
		CourseManager.PlayerStatesChanged += OnPlayerStatesChanged;
		GameManager.RemotePlayerRegistered += OnAnyPlayerChanged;
		BNetworkManager.SteamPlayerRelationshipChanged += SteamPlayerRelationshipChanged;
		menuNavigation.OnExitEvent += Unpause;
	}

	private void UpdateButtonOffsets()
	{
		float x = allButtons[0].GetComponent<RectTransform>().sizeDelta.x;
		for (int i = 1; i < allButtons.Length; i++)
		{
			allButtons[i].GetComponent<RectTransform>().SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, x - buttonHorizontalOffset * (float)i);
		}
	}

	private void OnEnable()
	{
		OnInternetConnectionChanged();
		OnLobbyStateChanged();
		BNetworkManager.ConnectedToInternet += OnInternetConnectionChanged;
		BNetworkManager.DisconnectedFromInternet += OnInternetConnectionChanged;
		BNetworkManager.LobbyStateChanged += OnLobbyStateChanged;
		UpdateButtonOffsets();
	}

	private void OnDisable()
	{
		BNetworkManager.ConnectedToInternet -= OnInternetConnectionChanged;
		BNetworkManager.DisconnectedFromInternet -= OnInternetConnectionChanged;
		BNetworkManager.LobbyStateChanged -= OnLobbyStateChanged;
	}

	protected override void OnDestroy()
	{
		base.OnDestroy();
		UnpauseInternal(forceClose: true);
		if (pausedSnapshot.isValid())
		{
			pausedSnapshot.stop(FMOD.Studio.STOP_MODE.ALLOWFADEOUT);
		}
		PlayerId.AnyPlayerNameChanged -= OnAnyPlayerNameChanged;
		PlayerId.AnyPlayerGuidChanged -= OnAnyPlayerGuidChanged;
		CourseManager.PlayerStatesChanged -= OnPlayerStatesChanged;
		GameManager.RemotePlayerRegistered -= OnAnyPlayerChanged;
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
					playerEntries.Add(pauseMenuPlayerEntry);
				}
				ulong selectedVolumePlayerGuid = 0uL;
				ulong selectedMutePlayerGuid = 0uL;
				ulong selectedKickPlayerGuid = 0uL;
				for (int j = 0; j < playerEntries.Count; j++)
				{
					PauseMenuPlayerEntry pauseMenuPlayerEntry2 = playerEntries[j];
					UpdateSelectedValues(pauseMenuPlayerEntry2);
					bool flag = j < value.Count && !string.IsNullOrEmpty(value[j].name);
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

	private void OnAnyPlayerChanged(PlayerInfo player)
	{
		UpdatePlayerEntries();
	}

	private void SteamPlayerRelationshipChanged(ulong guid, Relationship relationship)
	{
		UpdatePlayerEntries();
	}

	private void OnAnyPlayerNameChanged(PlayerId playerId)
	{
		OnAnyPlayerChanged(playerId.PlayerInfo);
	}

	private void OnAnyPlayerGuidChanged(PlayerId playerId)
	{
		OnAnyPlayerChanged(playerId.PlayerInfo);
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
				if (playerState.isConnected != state.isConnected)
				{
					return true;
				}
				if (playerState.playerGuid != state.playerGuid)
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
}
