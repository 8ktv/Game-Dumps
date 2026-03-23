using System;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using Steamworks;
using Steamworks.Data;
using TMPro;
using UnityEngine;
using UnityEngine.Localization;
using UnityEngine.Localization.Components;
using UnityEngine.Pool;
using UnityEngine.UI;

public class LobbyBrowser : SingletonBehaviour<LobbyBrowser>
{
	public class FriendInfo
	{
		public Friend SteamFriend { get; private set; }

		public Steamworks.Data.Lobby SteamLobby { get; private set; }

		public string FriendName { get; private set; }

		public bool IsPlayingThisGame { get; private set; }

		public string ConnectString { get; private set; }

		public bool IsConnectStringValid { get; private set; }

		public bool IsInLobby { get; private set; }

		public bool IsHost { get; private set; }

		public FriendInfo(Friend friend)
		{
			SteamFriend = friend;
			FriendName = friend.Name;
			ConnectString = friend.GetRichPresence("connect");
			IsConnectStringValid = ConnectString != null && ConnectString.StartsWith("+connectSteamLobby");
			IsPlayingThisGame = friend.IsPlayingThisGame;
			IsInLobby = BNetworkManager.TryGetSteamLobbyId(friend, out var id);
			if (IsInLobby)
			{
				SteamLobby = new Steamworks.Data.Lobby(id);
			}
			IsHost = friend.GetRichPresence("sbg_isHosting") == "true";
		}

		public Sprite GetIcon()
		{
			return PlayerIconManager.GetPlayerIconFromSteamId(SteamFriend.Id, PlayerIconManager.IconSize.Medium);
		}
	}

	public class LobbyCmp : IComparer<Lobby>
	{
		public enum SortMode
		{
			None = -1,
			Name,
			Course,
			Mode,
			Players,
			Ping
		}

		public SortMode mode;

		public bool reverse;

		public int Compare(Lobby x, Lobby y)
		{
			switch (mode)
			{
			case SortMode.Name:
				if (!reverse)
				{
					return x.GetName(filterProfanity: false).CompareTo(y.GetName(filterProfanity: false));
				}
				return y.GetName(filterProfanity: false).CompareTo(x.GetName(filterProfanity: false));
			case SortMode.Course:
			{
				x.GetCourseInfo(out var info, out var isOnDrivingRange);
				y.GetCourseInfo(out var info2, out var isOnDrivingRange2);
				if (isOnDrivingRange != isOnDrivingRange2)
				{
					if (!reverse)
					{
						return isOnDrivingRange2.CompareTo(isOnDrivingRange);
					}
					return isOnDrivingRange.CompareTo(isOnDrivingRange2);
				}
				if (!reverse)
				{
					return info.CompareTo(info2);
				}
				return info2.CompareTo(info);
			}
			case SortMode.Mode:
				return 0;
			case SortMode.Players:
			{
				int.TryParse(x.GetCurrentPlayerCount(), out var result);
				int.TryParse(y.GetCurrentPlayerCount(), out var result2);
				if (!reverse)
				{
					return result.CompareTo(result2);
				}
				return result2.CompareTo(result);
			}
			case SortMode.Ping:
				if (!reverse)
				{
					return x.Ping.CompareTo(y.Ping);
				}
				return y.Ping.CompareTo(x.Ping);
			default:
				return 0;
			}
		}
	}

	public class Lobby
	{
		public bool isFakeEntry;

		private int fakeEntrySeed;

		private static readonly string[] debugCourseNames = new string[5] { "DrivingRange", "HOLE_INFO_CourseName_Coast", "HOLE_INFO_CourseName_Forest", "HOLE_INFO_CourseName_Random", "HOLE_INFO_CourseName_Custom" };

		public Steamworks.Data.Lobby SteamLobby { get; private set; }

		public LobbyEntryUi Entry { get; private set; }

		public int Ping { get; private set; }

		public Lobby(Steamworks.Data.Lobby steamLobby)
		{
			SteamLobby = steamLobby;
			fakeEntrySeed = UnityEngine.Random.Range(-2147483647, int.MaxValue);
		}

		public void SetEntry(LobbyEntryUi entry)
		{
			Entry = entry;
		}

		private int GetFakeValue(int min, int max)
		{
			return min + BMath.Abs(fakeEntrySeed % (max - min));
		}

		public bool TryRefreshData()
		{
			return SteamLobby.Refresh();
		}

		public bool RefreshPing()
		{
			Ping = int.MinValue;
			if (isFakeEntry)
			{
				Ping = UnityEngine.Random.Range(20, 500);
				return true;
			}
			string hostLocation = GetHostLocation();
			if (string.IsNullOrEmpty(hostLocation))
			{
				return false;
			}
			NetPingLocation? netPingLocation = NetPingLocation.TryParseFromString(hostLocation);
			if (!netPingLocation.HasValue)
			{
				return false;
			}
			Ping = SteamNetworkingUtils.EstimatePingTo(netPingLocation.Value);
			return Ping >= 0;
		}

		public bool IsHostedByFriend()
		{
			if (isFakeEntry)
			{
				return GetFakeValue(0, 2) == 1;
			}
			return SteamLobby.Owner.IsFriend;
		}

		public bool RequiresPassword()
		{
			if (isFakeEntry)
			{
				return GetFakeValue(0, 2) == 1;
			}
			return SteamLobby.GetData("sbg_passwordRequired") == "true";
		}

		public string GetName(bool filterProfanity)
		{
			if (isFakeEntry)
			{
				return GetFakeValue(0, 256) + "Fake lobby with a long name for testing " + fakeEntrySeed;
			}
			if (filterProfanity)
			{
				GameManager.FilterProfanity(SteamLobby.GetData("lobbyName"), out var filteredString);
				return filteredString;
			}
			return SteamLobby.GetData("lobbyName");
		}

		public bool IsLobbyFull()
		{
			return GetMaxPlayers() == GetCurrentPlayerCount();
		}

		public string GetMaxPlayers()
		{
			if (isFakeEntry)
			{
				return GetFakeValue(8, 17).ToString();
			}
			return SteamLobby.GetData("maxPlayers");
		}

		public string GetCurrentPlayerCount()
		{
			if (isFakeEntry)
			{
				return GetFakeValue(0, GetFakeValue(8, 17) + 1).ToString();
			}
			return SteamLobby.GetData("currentPlayerCount");
		}

		public string GetHostLocation()
		{
			return SteamLobby.GetData("hostLocation");
		}

		public void GetCourseInfo(out string info, out bool isOnDrivingRange)
		{
			string text;
			string progress;
			string length;
			if (isFakeEntry)
			{
				text = debugCourseNames[GetFakeValue(0, debugCourseNames.Length)];
				progress = GetFakeValue(0, 10).ToString();
				length = GetFakeValue(9, 19).ToString();
			}
			else
			{
				text = SteamLobby.GetData("currentCourse");
				progress = SteamLobby.GetData("currentCourseProgress");
				length = SteamLobby.GetData("currentCourseLength");
			}
			info = GetFormattedString(text, progress, length);
			isOnDrivingRange = text == "DrivingRange";
			static string GetFormattedString(string course, string text2, string text3)
			{
				if (course == "DrivingRange")
				{
					return Localization.UI.HOLE_INFO_DrivingRange;
				}
				course = LocalizationManager.GetString(StringTable.UI, course);
				if (course == string.Empty)
				{
					return string.Empty;
				}
				return course + " (" + text2 + "/" + text3 + ")";
			}
		}
	}

	[SerializeField]
	private LobbyEntryUi lobbyEntryPrefab;

	[SerializeField]
	private Transform lobbyEntryParent;

	[SerializeField]
	private Button joinButton;

	[SerializeField]
	private Button joinAsSpectatorButton;

	[SerializeField]
	private Button refreshButton;

	[SerializeField]
	private Button quickRefreshButton;

	[SerializeField]
	private UnityEngine.UI.Image refreshingAllIcon;

	[SerializeField]
	private UnityEngine.UI.Image quickRefreshingIcon;

	[SerializeField]
	private LocalizeStringEvent noLobbiesLabel;

	[SerializeField]
	private UnityEngine.Color defaultEntryColor;

	[SerializeField]
	private UnityEngine.Color selectedEntryColor;

	[SerializeField]
	[Min(1f)]
	private float autoRefreshPeriod = 5f;

	[SerializeField]
	private Button[] sortButtons;

	[SerializeField]
	private TMP_InputField nameFilter;

	[SerializeField]
	private DropdownOption maxPingDropdown;

	[SerializeField]
	private Toggle friendsOnly;

	[SerializeField]
	private Toggle hidePassword;

	[SerializeField]
	private Toggle hideFull;

	[SerializeField]
	private CanvasGroup filterGroup;

	[SerializeField]
	private int pingInterval;

	[SerializeField]
	private int maxPing;

	[SerializeField]
	private TextMeshProUGUI onlinePlayersLabel;

	[SerializeField]
	private ScrollRect scrollRect;

	[SerializeField]
	private LocalizedString noLobbiesFound;

	[SerializeField]
	private LocalizedString noLobbiesFoundWithFilter;

	[SerializeField]
	private MenuNavigation navigation;

	[SerializeField]
	private GameObject screen;

	private readonly Queue<LobbyEntryUi> unusedLobbyEntries = new Queue<LobbyEntryUi>();

	private readonly List<Lobby> listedLobbies = new List<Lobby>();

	private readonly Dictionary<Steamworks.Data.Lobby, Lobby> lobbiesRefreshingData = new Dictionary<Steamworks.Data.Lobby, Lobby>();

	private readonly MultiDictionary<ulong, FriendInfo, List<FriendInfo>> friendsInLobbies = new MultiDictionary<ulong, FriendInfo, List<FriendInfo>>();

	private Lobby selectedLobby;

	private double lastGameSelectTimestamp = double.MinValue;

	private bool isRefreshing;

	private double lastRefreshTimestamp = double.MinValue;

	private LobbyCmp.SortMode sortMode = LobbyCmp.SortMode.None;

	private bool reverseSort;

	[CVar("lobbyBrowserFakeCount", "", "", false, true)]
	private static int fakeLobbyCount;

	private UnityEngine.UI.Image refreshingIcon;

	private Queue<Lobby> lobbyUpdateQueue = new Queue<Lobby>();

	private bool lobbiesDirty;

	public static bool IsActive
	{
		get
		{
			if (SingletonBehaviour<LobbyBrowser>.HasInstance)
			{
				return SingletonBehaviour<LobbyBrowser>.Instance.screen.activeInHierarchy;
			}
			return false;
		}
	}

	public static UnityEngine.Color DefaultEntryColor
	{
		get
		{
			if (!SingletonBehaviour<LobbyBrowser>.HasInstance)
			{
				return default(UnityEngine.Color);
			}
			return SingletonBehaviour<LobbyBrowser>.Instance.defaultEntryColor;
		}
	}

	public static UnityEngine.Color SelectedEntryColor
	{
		get
		{
			if (!SingletonBehaviour<LobbyBrowser>.HasInstance)
			{
				return default(UnityEngine.Color);
			}
			return SingletonBehaviour<LobbyBrowser>.Instance.selectedEntryColor;
		}
	}

	protected override void Awake()
	{
		base.Awake();
		refreshButton.onClick.AddListener(RefreshAll);
		joinButton.onClick.AddListener(delegate
		{
			JoinSelectedLobbyInternal(asSpectator: false);
		});
		joinAsSpectatorButton.onClick.AddListener(delegate
		{
			JoinSelectedLobbyInternal(asSpectator: true);
		});
		quickRefreshButton.onClick.AddListener(QuickRefresh);
		List<string> value;
		using (CollectionPool<List<string>, string>.Get(out value))
		{
			int num = maxPing / pingInterval;
			for (int num2 = 0; num2 < num; num2++)
			{
				int pingValue = GetPingValue(num2);
				string item = pingValue.ToString();
				if (num2 >= num - 1)
				{
					item = ">" + pingValue;
				}
				value.Add(item);
			}
			maxPingDropdown.SetOptions(value);
			maxPingDropdown.Initialize(ApplyFilters, value.Count / 2);
			hideFull.isOn = true;
			hidePassword.isOn = true;
			friendsOnly.isOn = false;
			nameFilter.onValueChanged.AddListener(delegate
			{
				ApplyFilters();
			});
			friendsOnly.onValueChanged.AddListener(delegate
			{
				ApplyFilters();
			});
			hidePassword.onValueChanged.AddListener(delegate
			{
				ApplyFilters();
			});
			hideFull.onValueChanged.AddListener(delegate
			{
				ApplyFilters();
			});
			onlinePlayersLabel.text = string.Format(Localization.UI.LOBBY_BROWSER_PlayersOnline, 0);
			ToggleSortMode(LobbyCmp.SortMode.Ping);
			for (int num3 = 0; num3 < sortButtons.Length; num3++)
			{
				LobbyCmp.SortMode mode = (LobbyCmp.SortMode)num3;
				sortButtons[num3].onClick.AddListener(delegate
				{
					ToggleSortMode(mode);
				});
			}
			SetEnabled(enabled: false);
			navigation.OnExitEvent += OnMenuExit;
		}
	}

	public void SetEnabled(bool enabled)
	{
		bool activeSelf = screen.gameObject.activeSelf;
		screen.gameObject.SetActive(enabled);
		if (activeSelf && !enabled)
		{
			SteamMatchmaking.OnLobbyDataChanged -= OnLobbyDataChanged;
		}
		else if (!activeSelf && enabled)
		{
			Deselect();
			RefreshAll();
			SteamMatchmaking.OnLobbyDataChanged += OnLobbyDataChanged;
		}
	}

	protected override void OnDestroy()
	{
		base.OnDestroy();
		SetEnabled(enabled: false);
	}

	public void OnMenuExit()
	{
		SetEnabled(enabled: false);
	}

	private int GetPingValue(int index)
	{
		return (index + 1) * pingInterval;
	}

	private void ApplyFilters()
	{
		int num = 0;
		foreach (Lobby listedLobby in listedLobbies)
		{
			bool flag = IsLobbyVisible(listedLobby);
			listedLobby.Entry.gameObject.SetActive(flag);
			if (flag)
			{
				num++;
			}
		}
		SetNoLobbiesLabelActive(num == 0);
		lobbiesDirty = true;
	}

	private void SetNoLobbiesLabelActive(bool active)
	{
		noLobbiesLabel.gameObject.SetActive(active);
		if (active)
		{
			noLobbiesLabel.StringReference = ((!IsAnyFilterActive()) ? noLobbiesFound : noLobbiesFoundWithFilter);
			noLobbiesLabel.RefreshString();
		}
	}

	private bool IsAnyFilterActive()
	{
		if (string.IsNullOrWhiteSpace(nameFilter.text) && maxPingDropdown.value >= maxPingDropdown.OptionsCount - 1 && !friendsOnly.isOn && !hidePassword.isOn)
		{
			return hideFull.isOn;
		}
		return true;
	}

	private bool IsLobbyVisible(Lobby lobby)
	{
		if (!string.IsNullOrWhiteSpace(nameFilter.text) && !lobby.GetName(filterProfanity: false).ToLower().Contains(nameFilter.text.ToLower()))
		{
			return false;
		}
		if (maxPingDropdown.value < maxPingDropdown.OptionsCount - 1 && lobby.Ping > GetPingValue(maxPingDropdown.value))
		{
			return false;
		}
		if (friendsOnly.isOn && !lobby.IsHostedByFriend())
		{
			return false;
		}
		if (hidePassword.isOn && lobby.RequiresPassword())
		{
			return false;
		}
		if (hideFull.isOn && lobby.IsLobbyFull())
		{
			return false;
		}
		return true;
	}

	private void ToggleSortMode(LobbyCmp.SortMode newSortMode)
	{
		LobbyCmp.SortMode num = sortMode;
		sortMode = newSortMode;
		if (num != sortMode)
		{
			reverseSort = false;
		}
		else
		{
			reverseSort = !reverseSort;
		}
		for (int i = 0; i < sortButtons.Length; i++)
		{
			UnityEngine.UI.Image[] componentsInChildren = sortButtons[i].GetComponentsInChildren<UnityEngine.UI.Image>();
			SetAlpha(componentsInChildren[0], (sortMode == (LobbyCmp.SortMode)i && !reverseSort) ? 1f : 0.66f);
			SetAlpha(componentsInChildren[1], (sortMode == (LobbyCmp.SortMode)i && reverseSort) ? 1f : 0.66f);
		}
		lobbiesDirty = true;
		static void SetAlpha(UnityEngine.UI.Image image, float alpha)
		{
			UnityEngine.Color color = image.color;
			color.a = alpha;
			image.color = color;
		}
	}

	private void UpdateSorting()
	{
		listedLobbies.Sort(new LobbyCmp
		{
			mode = sortMode,
			reverse = reverseSort
		});
		UpdateNavigation();
	}

	private void UpdateNavigation()
	{
		Lobby first = null;
		Lobby last = null;
		for (int i = 0; i < listedLobbies.Count; i++)
		{
			Lobby lobby = listedLobbies[i];
			lobby.Entry.transform.SetAsLastSibling();
			if (IsLobbyVisible(lobby))
			{
				if (first == null)
				{
					first = lobby;
				}
				if (last != null)
				{
					Selectable component = lobby.Entry.GetComponent<Selectable>();
					Selectable component2 = last.Entry.GetComponent<Selectable>();
					Navigation navigation = component.navigation;
					navigation.mode = Navigation.Mode.Explicit;
					navigation.selectOnUp = component2;
					component.navigation = navigation;
					navigation = component2.navigation;
					navigation.selectOnDown = component;
					component2.navigation = navigation;
				}
				last = lobby;
			}
		}
		if (selectedLobby != null && selectedLobby.Entry != null && selectedLobby.Entry.gameObject.activeInHierarchy)
		{
			scrollRect.EnsureVisibility(selectedLobby.Entry.GetComponent<RectTransform>(), Vector2.zero);
		}
		if (first != null)
		{
			SetNavigationDown(nameFilter);
			SetNavigationDown(maxPingDropdown.Selectable);
			SetNavigationDown(friendsOnly);
			SetNavigationDown(hidePassword);
			SetNavigationDown(hideFull);
			Selectable component3 = first.Entry.GetComponent<Selectable>();
			Navigation navigation2 = component3.navigation;
			navigation2.selectOnUp = hideFull;
			navigation2.mode = Navigation.Mode.Explicit;
			component3.navigation = navigation2;
		}
		if (last != null)
		{
			SetNavigationUp(quickRefreshButton);
			SetNavigationUp(refreshButton);
			SetNavigationUp(joinButton);
			SetNavigationUp(joinAsSpectatorButton);
			Selectable component4 = last.Entry.GetComponent<Selectable>();
			Navigation navigation3 = component4.navigation;
			navigation3.selectOnDown = joinButton;
			navigation3.mode = Navigation.Mode.Explicit;
			component4.navigation = navigation3;
		}
		void SetNavigationDown(Selectable selectable)
		{
			Navigation navigation4 = selectable.navigation;
			navigation4.selectOnDown = first?.Entry.GetComponent<Selectable>() ?? null;
			selectable.navigation = navigation4;
		}
		void SetNavigationUp(Selectable selectable)
		{
			Navigation navigation4 = selectable.navigation;
			navigation4.selectOnUp = last?.Entry.GetComponent<Selectable>() ?? null;
			selectable.navigation = navigation4;
		}
	}

	private void Update()
	{
		if (!screen.activeInHierarchy)
		{
			return;
		}
		if (isRefreshing)
		{
			refreshingIcon.rectTransform.Rotate(Vector3.forward, 360f * Time.deltaTime, Space.Self);
		}
		else if (listedLobbies.Count > 0 && !isRefreshing && BMath.GetTimeSince(lastRefreshTimestamp) > autoRefreshPeriod)
		{
			QuickRefresh();
		}
		int num = 0;
		while (lobbyUpdateQueue.Count > 0 && num < 10)
		{
			Lobby lobby = lobbyUpdateQueue.Dequeue();
			try
			{
				if (lobby.Entry == null)
				{
					AddLobby(lobby);
				}
				else
				{
					RefreshLobby(lobby);
				}
			}
			catch (Exception exception)
			{
				Debug.LogError("Encountered exception while updating lobby! See next log for exception");
				Debug.LogException(exception);
			}
			finally
			{
				num++;
			}
		}
		if (lobbiesDirty)
		{
			UpdateSorting();
			lobbiesDirty = false;
		}
		if (InputManager.CurrentGamepad != null && !InputManager.CurrentModeMask.HasMode(InputMode.ForceDisabled))
		{
			if (InputManager.CurrentGamepad.buttonWest.wasPressedThisFrame)
			{
				navigation.Select(joinButton.interactable ? joinButton.GetComponent<ControllerSelectable>() : refreshButton.GetComponent<ControllerSelectable>());
			}
			else if (InputManager.CurrentGamepad.buttonNorth.wasPressedThisFrame)
			{
				navigation.Select(hideFull.GetComponent<ControllerSelectable>());
			}
			if (InputManager.CurrentGamepad.buttonEast.wasPressedThisFrame)
			{
				SetEnabled(enabled: false);
			}
		}
	}

	private void DisableButtons(bool disableJoin)
	{
		joinButton.interactable = !disableJoin;
		joinAsSpectatorButton.interactable = !disableJoin;
		refreshButton.interactable = false;
	}

	private async void QuickRefresh()
	{
		if (!isRefreshing)
		{
			quickRefreshButton.GetComponent<UiSfx>().PlaySelectSfx(InputManager.UsingGamepad);
		}
		RefreshInternal(QuickRefreshInternal(), quickRefreshingIcon);
	}

	private void RefreshAll()
	{
		if (!isRefreshing)
		{
			refreshButton.GetComponent<UiSfx>().PlaySelectSfx(InputManager.UsingGamepad);
		}
		RefreshInternal(RefreshLobbiesInternal(), refreshingAllIcon);
	}

	private async void RefreshInternal(UniTask refreshTask, UnityEngine.UI.Image refreshing)
	{
		if (isRefreshing)
		{
			return;
		}
		StartRefreshing();
		try
		{
			await refreshTask;
			while (lobbyUpdateQueue.Count > 0 && !(this == null))
			{
				await UniTask.Yield();
			}
		}
		finally
		{
			FinishRefreshing();
		}
		void FinishRefreshing()
		{
			if (!(this == null))
			{
				isRefreshing = false;
				refreshingIcon.enabled = false;
				refreshButton.interactable = true;
				quickRefreshButton.interactable = true;
				lastRefreshTimestamp = Time.timeAsDouble;
			}
		}
		void StartRefreshing()
		{
			lobbyUpdateQueue.Clear();
			isRefreshing = true;
			quickRefreshingIcon.enabled = false;
			refreshingAllIcon.enabled = false;
			refreshingIcon = refreshing;
			refreshingIcon.enabled = true;
			refreshingIcon.rectTransform.localRotation = Quaternion.identity;
			refreshButton.interactable = false;
			quickRefreshButton.interactable = false;
		}
	}

	private async UniTask QuickRefreshInternal()
	{
		double refreshStartTimestamp = Time.timeAsDouble;
		RefreshFriends();
		foreach (Lobby listedLobby in listedLobbies)
		{
			if (!listedLobby.isFakeEntry)
			{
				listedLobby.SteamLobby.Refresh();
				lobbiesRefreshingData.TryAdd(listedLobby.SteamLobby, listedLobby);
			}
		}
		await WaitForLobbyRefresh(1f);
		while (BMath.GetTimeSince(refreshStartTimestamp) < 0.25f)
		{
			await UniTask.Yield();
			if (!base.enabled)
			{
				return;
			}
		}
		foreach (Lobby value in lobbiesRefreshingData.Values)
		{
			ReturnLobbyEntry(value.Entry);
			listedLobbies.Remove(value);
		}
		lobbiesRefreshingData.Clear();
	}

	private void RefreshFriends()
	{
		friendsInLobbies.Clear();
		foreach (Friend friend in SteamFriends.GetFriends())
		{
			FriendInfo friendInfo = new FriendInfo(friend);
			if (friendInfo.IsPlayingThisGame && friendInfo.IsInLobby && friendInfo.IsConnectStringValid)
			{
				friendsInLobbies.Add(friendInfo.SteamLobby.Id, friendInfo);
			}
		}
	}

	private async UniTask RefreshLobbiesInternal()
	{
		refreshButton.interactable = false;
		try
		{
			DisableButtons(disableJoin: false);
			await UpdateLobbies();
			if (!(this == null))
			{
				if (!base.enabled)
				{
					ClearLobbies();
				}
				else
				{
					ApplyFilters();
				}
			}
		}
		catch (Exception exception)
		{
			Debug.LogError("Encountered exception while refreshing lobbies. See next exception for details", base.gameObject);
			Debug.LogException(exception, base.gameObject);
			ClearLobbies();
		}
		finally
		{
			if (this != null)
			{
				refreshButton.interactable = true;
			}
		}
		void ClearLobbies()
		{
			if (!(this == null))
			{
				Deselect();
				foreach (Lobby listedLobby in listedLobbies)
				{
					ReturnLobbyEntry(listedLobby.Entry);
				}
				listedLobbies.Clear();
				SetNoLobbiesLabelActive(active: true);
			}
		}
		async UniTask UpdateLobbies()
		{
			double refreshStartTimestamp = Time.timeAsDouble;
			lobbiesRefreshingData.Clear();
			ClearLobbies();
			UniTask onlinePlayersTask = UpdateOnlinePlayers();
			if (SteamEnabler.IsSteamEnabled)
			{
				RefreshFriends();
				await SteamNetworkingUtils.WaitForPingDataAsync();
				if (this == null)
				{
					return;
				}
				if (!friendsOnly.isOn)
				{
					LobbyQuery lobbyQuery = SteamMatchmaking.LobbyList.WithMaxResults(50).FilterDistanceWorldwide().WithHigher("maxPlayers", 0);
					if (hidePassword.isOn)
					{
						lobbyQuery = lobbyQuery.WithKeyValue("sbg_passwordRequired", "false");
					}
					if (hideFull.isOn)
					{
						lobbyQuery = lobbyQuery.WithSlotsAvailable(1);
					}
					Steamworks.Data.Lobby[] array = await lobbyQuery.RequestAsync();
					if (this == null)
					{
						return;
					}
					if (array != null)
					{
						Steamworks.Data.Lobby[] array2 = array;
						for (int i = 0; i < array2.Length; i++)
						{
							Lobby lobby = new Lobby(array2[i]);
							if (lobby.TryRefreshData())
							{
								lobbiesRefreshingData.TryAdd(lobby.SteamLobby, lobby);
							}
						}
					}
				}
				foreach (FriendInfo allValue in friendsInLobbies.GetAllValues())
				{
					if (allValue.IsHost)
					{
						Lobby lobby2 = new Lobby(allValue.SteamLobby);
						if (lobby2.TryRefreshData())
						{
							lobbiesRefreshingData.TryAdd(lobby2.SteamLobby, lobby2);
						}
					}
				}
				await WaitForLobbyRefresh(5f);
				if (this == null)
				{
					return;
				}
				lobbiesRefreshingData.Clear();
			}
			for (int j = 0; j < fakeLobbyCount; j++)
			{
				AddLobby(new Lobby(default(Steamworks.Data.Lobby))
				{
					isFakeEntry = true
				});
				await UniTask.WaitForSeconds(UnityEngine.Random.Range(0f, 0.1f));
				if (this == null)
				{
					return;
				}
			}
			if (!(this == null) && base.enabled)
			{
				while (BMath.GetTimeSince(refreshStartTimestamp) < 0.25f)
				{
					await UniTask.Yield();
					if (this == null || !base.enabled)
					{
						return;
					}
				}
				await onlinePlayersTask;
			}
		}
		async UniTask UpdateOnlinePlayers()
		{
			int num = 1;
			if (SteamEnabler.IsSteamEnabled)
			{
				num = await SteamUserStats.PlayerCountAsync();
				if (num < 0)
				{
					Debug.LogWarning("Failed to get Steam online player count!!!");
				}
				if (this == null)
				{
					return;
				}
				if (SteamClient.State != FriendState.Invisible && SteamClient.State != FriendState.Offline)
				{
					num = BMath.Max(1, num);
				}
			}
			if (fakeLobbyCount > 0)
			{
				num += UnityEngine.Random.Range(0, fakeLobbyCount * 1000);
			}
			onlinePlayersLabel.text = string.Format((num == 1) ? Localization.UI.LOBBY_BROWSER_SinglePlayerOnline : Localization.UI.LOBBY_BROWSER_PlayersOnline, num.ToString());
		}
	}

	private async UniTask WaitForLobbyRefresh(float timeout)
	{
		float lobbyRefreshTime = 0f;
		while (lobbiesRefreshingData.Count > 0 && lobbyRefreshTime < timeout)
		{
			await UniTask.Yield();
			if (this == null || !base.enabled)
			{
				return;
			}
			lobbyRefreshTime += Time.deltaTime;
		}
		await SteamNetworkingUtils.WaitForPingDataAsync();
	}

	private void GetFriendsInLobby(ulong lobbyId, List<Sprite> friendIcons, bool fakeLobby)
	{
		if (fakeLobby)
		{
			for (int i = 0; i < UnityEngine.Random.Range(0, 17); i++)
			{
				friendIcons.Add(SingletonBehaviour<PlayerIconManager>.Instance.defaultIcon);
			}
		}
		else
		{
			if (!friendsInLobbies.TryGetValues(lobbyId, out var values))
			{
				return;
			}
			foreach (FriendInfo item in values)
			{
				friendIcons.Add(item.GetIcon());
			}
		}
	}

	private void AddLobby(Lobby lobby)
	{
		lobby.RefreshPing();
		LobbyEntryUi unusedLobbyEntry = GetUnusedLobbyEntry();
		unusedLobbyEntry.gameObject.SetActive(value: false);
		List<Sprite> value;
		using (CollectionPool<List<Sprite>, Sprite>.Get(out value))
		{
			GetFriendsInLobby(lobby.SteamLobby.Id, value, lobby.isFakeEntry);
			unusedLobbyEntry.Initialize(lobby, delegate
			{
				SetSelectedButton(lobby, checkDoubleClick: true);
			}, delegate
			{
				SetSelectedButton(lobby, checkDoubleClick: false);
			}, GetPingString(lobby), value);
			lobby.SetEntry(unusedLobbyEntry);
			listedLobbies.Add(lobby);
			UpdateLobbyFilters(lobby);
		}
	}

	private void RefreshLobby(Lobby lobby)
	{
		List<Sprite> value;
		using (CollectionPool<List<Sprite>, Sprite>.Get(out value))
		{
			GetFriendsInLobby(lobby.SteamLobby.Id, value, lobby.isFakeEntry);
			lobby.RefreshPing();
			lobby.Entry.RefreshLobby(lobby, GetPingString(lobby), value);
			UpdateLobbyFilters(lobby);
		}
	}

	private void UpdateLobbyFilters(Lobby lobby)
	{
		bool flag = IsLobbyVisible(lobby);
		lobby.Entry.transform.SetAsLastSibling();
		lobby.Entry.gameObject.SetActive(flag);
		if (flag)
		{
			noLobbiesLabel.gameObject.SetActive(value: false);
		}
		lobbiesDirty = true;
	}

	private string GetPingString(Lobby lobby)
	{
		if (lobby.Ping <= 0)
		{
			return "N/A";
		}
		return lobby.Ping.ToString();
	}

	private void JoinSelectedLobbyInternal(bool asSpectator)
	{
		if (selectedLobby != null)
		{
			PlayerGolfer.JoinAsSpectator = asSpectator;
			BNetworkManager.ConnectToSteamLobby(selectedLobby.SteamLobby.Id.Value.ToString(), canExitCurrentLobby: false);
		}
	}

	private void SetSelectedButton(Lobby lobby, bool checkDoubleClick)
	{
		Lobby lobby2 = selectedLobby;
		if (checkDoubleClick && lobby2 != null && lobby2 == lobby && BMath.GetTimeSince(lastGameSelectTimestamp) < 0.25f)
		{
			JoinSelectedLobbyInternal(asSpectator: false);
			return;
		}
		Deselect();
		selectedLobby = lobby;
		selectedLobby.Entry.SetIsSelected(isSelected: true);
		joinButton.interactable = true;
		joinAsSpectatorButton.interactable = true;
		if (checkDoubleClick)
		{
			lastGameSelectTimestamp = Time.timeAsDouble;
		}
	}

	private void Deselect()
	{
		joinButton.interactable = false;
		joinAsSpectatorButton.interactable = false;
		if (selectedLobby != null)
		{
			selectedLobby.Entry.SetIsSelected(isSelected: false);
		}
		selectedLobby = null;
	}

	private LobbyEntryUi GetUnusedLobbyEntry()
	{
		if (!unusedLobbyEntries.TryDequeue(out var result))
		{
			return UnityEngine.Object.Instantiate(lobbyEntryPrefab, lobbyEntryParent);
		}
		return result;
	}

	private void ReturnLobbyEntry(LobbyEntryUi entry)
	{
		entry.gameObject.SetActive(value: false);
		unusedLobbyEntries.Enqueue(entry);
	}

	private void OnLobbyDataChanged(Steamworks.Data.Lobby steamLobby)
	{
		if (lobbiesRefreshingData.TryGetValue(steamLobby, out var value))
		{
			lobbiesRefreshingData.Remove(steamLobby);
			lobbyUpdateQueue.Enqueue(value);
		}
	}
}
