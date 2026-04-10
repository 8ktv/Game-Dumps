using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using FMODUnity;
using Mirror;
using Mirror.RemoteCalls;
using Steamworks;
using TMPro;
using UnityEngine;
using UnityEngine.Localization;
using UnityEngine.Localization.Components;
using UnityEngine.Pool;
using UnityEngine.UI;

public class MatchSetupMenu : SingletonNetworkBehaviour<MatchSetupMenu>
{
	private class SelectableChildIndexCmp : IComparer<Selectable>
	{
		public int Compare(Selectable a, Selectable b)
		{
			return a.transform.GetSiblingIndex().CompareTo(b.transform.GetSiblingIndex());
		}
	}

	[Serializable]
	public class ServerValues
	{
		public int maxPlayers = 8;

		public int maxSpectators;

		public LobbyMode lobbyMode = LobbyMode.Friends;

		public string password = string.Empty;

		public string serverName = string.Empty;

		public int activeCourse;

		public int randomCupNumHoles = 9;

		public string[] customActiveHoles;

		public List<MatchSetupRules.Rule> ruleKeys;

		public List<float> ruleValues;

		public List<MatchSetupRules.ItemPoolId> spawnChanceKeys;

		public List<float> spawnChanceValues;

		public MatchSetupRules.Preset rulePreset;

		public bool randomEnabled;
	}

	public GameObject menu;

	public Button[] startMatchButtons;

	public LocalizeStringEvent[] startMatchLabels;

	[Header("Match Setup")]
	public CanvasGroup matchSetupTab;

	public MatchSetupPlayer playerPrefab;

	public ReorderableList playersList;

	public ReorderableList spectatorsList;

	public DropdownOption lobbyModeDropdown;

	public TMP_InputField passwordField;

	public TMP_InputField serverNameField;

	public LocalizeStringEvent currentCourseLocalizeStringEvent;

	public Selectable playersAreaTopLeft;

	public Selectable playersAreaTopRight;

	public Selectable playersAreaBotLeft;

	public Selectable playersAreaBotRight;

	public UiTooltip tooltip;

	public UiTooltip compactTooltip;

	public UiTooltip warningTooltip;

	public UiTooltip activeTimeTooltip;

	[Header("Players slider")]
	public SliderOption maxPlayersSlider;

	public GameObject playerCountRecommendation;

	public GameObject warningBackdrop;

	public GameObject warningIcon;

	[Header("Courses")]
	public CanvasGroup courseSelectTab;

	public Button categoryPrefab;

	public MatchSetupHole holePrefab;

	public Transform categoryRoot;

	public ReorderableList activeHoles;

	public ReorderableList inactiveHoles;

	public TMP_Text activeLabel;

	public TMP_Text inactiveLabel;

	public UiFadeEnable holePreview;

	public Image holePreviewImage;

	public CanvasGroup numberOfHoles;

	public SliderOption numberOfHolesSlider;

	public CanvasGroup coursesControllerPrompt;

	public Sprite customHolesIcon;

	public LocalizeStringEvent courseLabel;

	public GameObject courseRandom;

	public Toggle courseRandomToggle;

	[Header("Rules")]
	public MatchSetupRules rules;

	public CanvasGroup rulesTab;

	[Header("Network")]
	[SyncVar(hook = "OnMaxPlayersChange")]
	public int maxPlayers;

	[SyncVar(hook = "OnLobbyModeChange")]
	public LobbyMode lobbyMode;

	[SyncVar(hook = "OnHasPasswordChange")]
	public bool hasPassword;

	[SyncVar(hook = "OnServerNameChange")]
	public string serverName;

	[SyncVar(hook = "OnActiveCourseChanged")]
	public int activeCourse;

	[SyncVar(hook = "OnRandomCupNumHolesChanged")]
	public int randomCupNumHoles;

	[SyncVar(hook = "OnRandomEnabledChanged")]
	public bool randomEnabled;

	public SyncList<int> activeHolesList = new SyncList<int>();

	public SyncList<int> inactiveHolesList = new SyncList<int>();

	private const int CUSTOM_COURSE = -1;

	private ServerValues serverValues;

	private readonly List<MatchSetupPlayer> players = new List<MatchSetupPlayer>();

	private readonly List<MatchSetupHole> holes = new List<MatchSetupHole>();

	private readonly AntiCheatPerPlayerRateChecker serverRequestSpectatorCommandRateLimiter = new AntiCheatPerPlayerRateChecker("Request spectator", 0.25f, 5, 10, 2f);

	private const string SAVE_FILENAME = "Lobby.json";

	private bool isEnabled;

	private List<Button> cachedCourseButtons = new List<Button>();

	public Action<int, int> _Mirror_SyncVarHookDelegate_maxPlayers;

	public Action<LobbyMode, LobbyMode> _Mirror_SyncVarHookDelegate_lobbyMode;

	public Action<bool, bool> _Mirror_SyncVarHookDelegate_hasPassword;

	public Action<string, string> _Mirror_SyncVarHookDelegate_serverName;

	public Action<int, int> _Mirror_SyncVarHookDelegate_activeCourse;

	public Action<int, int> _Mirror_SyncVarHookDelegate_randomCupNumHoles;

	public Action<bool, bool> _Mirror_SyncVarHookDelegate_randomEnabled;

	public static CourseData CustomCourseData { get; private set; }

	public static CourseData RandomCourseData { get; private set; }

	public static bool IsActive
	{
		get
		{
			if (SingletonNetworkBehaviour<MatchSetupMenu>.HasInstance)
			{
				return SingletonNetworkBehaviour<MatchSetupMenu>.Instance.isEnabled;
			}
			return false;
		}
	}

	public static string ServerName
	{
		get
		{
			if (!SingletonNetworkBehaviour<MatchSetupMenu>.HasInstance || GameSettings.All.General.StreamerMode)
			{
				return "*****";
			}
			return SingletonNetworkBehaviour<MatchSetupMenu>.Instance.serverName;
		}
	}

	public int NetworkmaxPlayers
	{
		get
		{
			return maxPlayers;
		}
		[param: In]
		set
		{
			GeneratedSyncVarSetter(value, ref maxPlayers, 1uL, _Mirror_SyncVarHookDelegate_maxPlayers);
		}
	}

	public LobbyMode NetworklobbyMode
	{
		get
		{
			return lobbyMode;
		}
		[param: In]
		set
		{
			GeneratedSyncVarSetter(value, ref lobbyMode, 2uL, _Mirror_SyncVarHookDelegate_lobbyMode);
		}
	}

	public bool NetworkhasPassword
	{
		get
		{
			return hasPassword;
		}
		[param: In]
		set
		{
			GeneratedSyncVarSetter(value, ref hasPassword, 4uL, _Mirror_SyncVarHookDelegate_hasPassword);
		}
	}

	public string NetworkserverName
	{
		get
		{
			return serverName;
		}
		[param: In]
		set
		{
			GeneratedSyncVarSetter(value, ref serverName, 8uL, _Mirror_SyncVarHookDelegate_serverName);
		}
	}

	public int NetworkactiveCourse
	{
		get
		{
			return activeCourse;
		}
		[param: In]
		set
		{
			GeneratedSyncVarSetter(value, ref activeCourse, 16uL, _Mirror_SyncVarHookDelegate_activeCourse);
		}
	}

	public int NetworkrandomCupNumHoles
	{
		get
		{
			return randomCupNumHoles;
		}
		[param: In]
		set
		{
			GeneratedSyncVarSetter(value, ref randomCupNumHoles, 32uL, _Mirror_SyncVarHookDelegate_randomCupNumHoles);
		}
	}

	public bool NetworkrandomEnabled
	{
		get
		{
			return randomEnabled;
		}
		[param: In]
		set
		{
			GeneratedSyncVarSetter(value, ref randomEnabled, 64uL, _Mirror_SyncVarHookDelegate_randomEnabled);
		}
	}

	protected override void OnDestroy()
	{
		if (isEnabled)
		{
			InputManager.DisableMode(InputMode.MatchSetup);
		}
		InputManager.SwitchedInputDeviceType -= OnInputDeviceChange;
		LocalizationManager.LanguageChanged -= OnLanguageChanged;
		CourseManager.PlayerStatesChanged -= OnPlayerStatesChanged;
		BNetworkManager.SteamPlayerRelationshipChanged -= SteamPlayerRelationshipChanged;
		playersList.OnElementMoved -= OnPlayerMovedToPlayers;
		spectatorsList.OnElementMoved -= OnPlayerMovedToSpectate;
		base.OnDestroy();
	}

	private void Start()
	{
		menu.GetComponent<MenuNavigation>().OnExitEvent += OnMenuExit;
		InputManager.SwitchedInputDeviceType += OnInputDeviceChange;
	}

	private void OnInputDeviceChange()
	{
		coursesControllerPrompt.alpha = (InputManager.UsingGamepad ? 1 : 0);
	}

	public static void InitializeStatics()
	{
		if (CustomCourseData == null)
		{
			CustomCourseData = ScriptableObject.CreateInstance<CourseData>();
			CustomCourseData.OverrideHoles(new HoleData[0]);
			CustomCourseData.name = "Custom Course";
			CustomCourseData.SetLocalizedName(Localization.UI.HOLE_INFO_CourseName_Custom_Ref);
		}
		if (RandomCourseData == null)
		{
			RandomCourseData = ScriptableObject.CreateInstance<CourseData>();
			RandomCourseData.OverrideHoles(new HoleData[0]);
			RandomCourseData.name = "Random Course";
			RandomCourseData.SetLocalizedName(Localization.UI.HOLE_INFO_CourseName_Random_Ref);
		}
	}

	public void SetEnabled(bool enabled)
	{
		if (enabled == isEnabled)
		{
			return;
		}
		isEnabled = enabled;
		holePreview.SetActive(active: false);
		menu.SetActive(enabled);
		if (!enabled)
		{
			InputManager.DisableMode(InputMode.MatchSetup);
			if (base.isServer)
			{
				ServerSaveSettings();
			}
			RuntimeManager.PlayOneShot(GameManager.AudioSettings.MatchSetupOpen);
		}
		else
		{
			InputManager.EnableMode(InputMode.MatchSetup);
			OnPlayersUpdated();
			coursesControllerPrompt.alpha = (InputManager.UsingGamepad ? 1 : 0);
			RuntimeManager.PlayOneShot(GameManager.AudioSettings.MatchSetupClose);
		}
		RefreshStartMatchButton();
		matchSetupTab.interactable = base.isServer;
		lobbyModeDropdown.Interactable = base.isServer && SingletonBehaviour<DrivingRangeManager>.HasInstance;
		courseSelectTab.interactable = base.isServer && GameManager.DrivingRangeHoleData.Scene.LoadedScene.IsValid();
		rulesTab.interactable = base.isServer && SingletonBehaviour<DrivingRangeManager>.HasInstance;
		serverNameField.inputType = (GameSettings.All.General.StreamerMode ? TMP_InputField.InputType.Password : TMP_InputField.InputType.Standard);
		serverNameField.ForceLabelUpdate();
		if (enabled)
		{
			tooltip.RegisterTooltip(maxPlayersSlider.GetComponent<RectTransform>(), Localization.UI.MATCHSETUP_Tooltip_Capacity);
			tooltip.RegisterTooltip(lobbyModeDropdown.GetComponent<RectTransform>(), Localization.UI.MATCHSETUP_Tooltip_LobbyMode);
		}
		else
		{
			tooltip.DeregisterTooltip(maxPlayersSlider.GetComponent<RectTransform>());
			tooltip.DeregisterTooltip(lobbyModeDropdown.GetComponent<RectTransform>());
		}
	}

	public void OnMenuExit()
	{
		SetEnabled(enabled: false);
	}

	private void UpdateCourseTooltips()
	{
		CourseCollection allCourses = GameManager.AllCourses;
		for (int i = 0; i < cachedCourseButtons.Count; i++)
		{
			if (!(cachedCourseButtons[i] == null))
			{
				if (i < allCourses.Courses.Length)
				{
					compactTooltip.DeregisterTooltip(cachedCourseButtons[i].GetComponent<RectTransform>());
					compactTooltip.RegisterTooltip(cachedCourseButtons[i].GetComponent<RectTransform>(), allCourses.Courses[i].LocalizedName.GetLocalizedString());
				}
				else
				{
					compactTooltip.DeregisterTooltip(cachedCourseButtons[i].GetComponent<RectTransform>());
					compactTooltip.RegisterTooltip(cachedCourseButtons[i].GetComponent<RectTransform>(), Localization.UI.MATCHSETUP_Tooltip_Custom);
				}
			}
		}
		tooltip.DeregisterTooltip(courseRandomToggle.GetComponent<RectTransform>());
		tooltip.RegisterTooltip(courseRandomToggle.GetComponent<RectTransform>(), Localization.UI.MATCHSETUP_Tooltip_RandomOrder);
		tooltip.DeregisterTooltip(numberOfHolesSlider.GetComponent<RectTransform>());
		tooltip.RegisterTooltip(numberOfHolesSlider.GetComponent<RectTransform>(), Localization.UI.MATCHSETUP_Tooltip_NumberOfHoles);
		tooltip.DeregisterTooltip(activeLabel.GetComponent<RectTransform>());
		tooltip.RegisterTooltip(activeLabel.GetComponent<RectTransform>(), Localization.UI.MATCHSETUP_Tooltip_ActiveHoles);
		tooltip.DeregisterTooltip(inactiveLabel.GetComponent<RectTransform>());
		tooltip.RegisterTooltip(inactiveLabel.GetComponent<RectTransform>(), Localization.UI.MATCHSETUP_Tooltip_InactiveHoles);
	}

	private void InitializeCourses()
	{
		CourseCollection allCourses = GameManager.AllCourses;
		cachedCourseButtons.Clear();
		for (int i = 0; i < allCourses.Courses.Length; i++)
		{
			int cat = i;
			Button button = AddCategory(allCourses.Courses[i].CategoryIcon);
			cachedCourseButtons.Add(button);
			button.onClick.AddListener(delegate
			{
				SetCourse(cat);
			});
		}
		Button button2 = AddCategory(customHolesIcon);
		cachedCourseButtons.Add(button2);
		button2.onClick.AddListener(delegate
		{
			SetCourse(-1);
		});
		UpdateCourseTooltips();
		if (base.isServer)
		{
			courseRandomToggle.onValueChanged.AddListener(delegate
			{
				NetworkrandomEnabled = courseRandomToggle.isOn;
			});
		}
		else
		{
			courseRandomToggle.interactable = false;
		}
		foreach (HoleData allHole in allCourses.allHoles)
		{
			RegisterHole(allHole);
		}
		inactiveHoles.OnElementMoved += OnHoleOrderUpdate;
		activeHoles.OnElementMoved += OnHoleOrderUpdate;
		Button AddCategory(Sprite icon = null)
		{
			Button button3 = UnityEngine.Object.Instantiate(categoryPrefab);
			button3.transform.SetParent(categoryRoot);
			button3.transform.localScale = Vector3.one;
			if (icon != null)
			{
				button3.transform.GetChild(1).GetComponent<Image>().sprite = icon;
			}
			return button3;
		}
		void RegisterHole(HoleData holeData)
		{
			MatchSetupHole matchSetupHole = UnityEngine.Object.Instantiate(holePrefab);
			matchSetupHole.transform.SetParent(activeHoles.contentRoot);
			matchSetupHole.transform.localScale = Vector3.one;
			matchSetupHole.holeData = holeData;
			matchSetupHole.menu = this;
			matchSetupHole.holeNameLocalizeStringEvent.StringReference = holeData.LocalizedName;
			matchSetupHole.holeNumber.text = (holeData.ParentCourseIndex + 1).ToString();
			matchSetupHole.holeNumber.color = holeData.ParentCourse.HoleLabelColor;
			matchSetupHole.background.sprite = holeData.ParentCourse.MenuBackground;
			for (int j = 0; j < matchSetupHole.difficultyIcons.Length; j++)
			{
				bool flag = (int)holeData.Difficulty >= j;
				matchSetupHole.difficultyIcons[j].gameObject.SetActive(flag);
				if (flag)
				{
					matchSetupHole.difficultyIcons[j].sprite = matchSetupHole.difficultyIconSprites[(int)holeData.Difficulty];
				}
			}
			matchSetupHole.GetComponent<ReorderableListElement>().enabled = base.isServer;
			holes.Add(matchSetupHole);
		}
	}

	private void OnHoleOrderUpdate(ReorderableListElement element = null)
	{
		SortInactiveHoles();
		activeHolesList.Clear();
		inactiveHolesList.Clear();
		List<ReorderableListElement> value;
		using (CollectionPool<List<ReorderableListElement>, ReorderableListElement>.Get(out value))
		{
			List<ReorderableListElement> value2;
			using (CollectionPool<List<ReorderableListElement>, ReorderableListElement>.Get(out value2))
			{
				List<HoleData> value3;
				using (CollectionPool<List<HoleData>, HoleData>.Get(out value3))
				{
					activeHoles.contentRoot.GetComponentsInChildren(value);
					inactiveHoles.contentRoot.GetComponentsInChildren(value2);
					AddHoleGlobalIndices(value, ref activeHolesList, value3);
					AddHoleGlobalIndices(value2, ref inactiveHolesList);
					CustomCourseData.OverrideHoles(value3.ToArray());
					if (activeCourse >= 0)
					{
						SetCourse(-1);
					}
					UpdateRandomMaxHolesSlider();
					UpdateHoleLabels();
					UpdateHoleNavigation();
					RefreshStartMatchButton();
				}
			}
		}
		static void AddHoleGlobalIndices(List<ReorderableListElement> elementList, ref SyncList<int> holeGlobalIndexList, List<HoleData> holeDataList = null)
		{
			foreach (ReorderableListElement element2 in elementList)
			{
				int globalIndex = element2.GetComponent<MatchSetupHole>().holeData.GlobalIndex;
				if (globalIndex >= 0)
				{
					holeGlobalIndexList.Add(globalIndex);
					if (holeDataList != null)
					{
						HoleData item = GameManager.AllCourses.allHoles[globalIndex];
						holeDataList.Add(item);
					}
				}
			}
		}
	}

	private int GetRandomMaxHolesSliderDefaultValue(CourseData courseData)
	{
		return BMath.Min(courseData.Holes.Length, 18);
	}

	private void UpdateRandomMaxHolesSlider()
	{
		CourseData currentCourseData = GetCurrentCourseData();
		numberOfHolesSlider.value = (randomEnabled ? BMath.Clamp(randomCupNumHoles, 1, currentCourseData.Holes.Length) : GetCurrentCourseData().Holes.Length);
		numberOfHolesSlider.SetLimits(1f, currentCourseData.Holes.Length);
	}

	private void SetCourse(int courseIndex, bool fromSerialization = false)
	{
		SelectCategory(categoryRoot, courseIndex);
		_ = activeCourse;
		ServerValues obj = serverValues;
		int num = (NetworkactiveCourse = courseIndex);
		obj.activeCourse = num;
		CourseData currentCourseData = GetCurrentCourseData();
		UpdateCourses(currentCourseData);
		RefreshStartMatchButton();
		UpdateRandomMaxHolesSlider();
		if (courseIndex != -1)
		{
			int randomMaxHolesSliderDefaultValue = GetRandomMaxHolesSliderDefaultValue(currentCourseData);
			if (randomEnabled)
			{
				numberOfHolesSlider.value = randomMaxHolesSliderDefaultValue;
				return;
			}
			numberOfHolesSlider.value = currentCourseData.Holes.Length;
			NetworkrandomCupNumHoles = randomMaxHolesSliderDefaultValue;
		}
	}

	private void OnRandomEnabledChanged(bool prev, bool curr)
	{
		courseRandom.SetActive(randomEnabled);
		numberOfHoles.interactable = base.isServer && randomEnabled;
		numberOfHoles.alpha = (numberOfHoles.interactable ? 1f : 0.5f);
		if (base.isServer)
		{
			serverValues.randomEnabled = randomEnabled;
		}
		else
		{
			courseRandomToggle.isOn = randomEnabled;
		}
		UpdateRandomMaxHolesSlider();
		if (PauseMenu.IsPaused)
		{
			SingletonBehaviour<PauseMenu>.Instance.UpdateGameInfoLabels();
		}
	}

	private CourseData GetCurrentCourseData()
	{
		CourseData[] courses = GameManager.AllCourses.Courses;
		if (activeCourse >= 0)
		{
			return courses[BMath.Clamp(activeCourse, 0, courses.Length - 1)];
		}
		return CustomCourseData;
	}

	private void SelectCategory(Transform root, int index)
	{
		if (index < 0)
		{
			index = root.childCount + index;
		}
		for (int i = 0; i < root.childCount; i++)
		{
			root.GetChild(i).GetChild(2).gameObject.SetActive(i == index);
		}
	}

	private void RefreshStartMatchButton()
	{
		CourseData currentCourseData = GetCurrentCourseData();
		bool interactable = false;
		if (base.isHost)
		{
			interactable = !SingletonBehaviour<DrivingRangeManager>.HasInstance || (currentCourseData != null && currentCourseData.Holes.Length != 0 && CourseManager.CountActivePlayers() > 0);
		}
		Button[] array = startMatchButtons;
		foreach (Button obj in array)
		{
			obj.interactable = interactable;
			if (obj.TryGetComponent<UiSfx>(out var component))
			{
				component.type = (SingletonBehaviour<DrivingRangeManager>.HasInstance ? UiSfx.Type.StartMatch : UiSfx.Type.GenericButton);
			}
		}
		LocalizeStringEvent[] array2 = startMatchLabels;
		for (int i = 0; i < array2.Length; i++)
		{
			array2[i].StringReference = (SingletonBehaviour<DrivingRangeManager>.HasInstance ? Localization.UI.MATCHSETUP_Button_StartMatch_Ref : Localization.UI.MATCHSETUP_Button_CancelMatch_Ref);
		}
	}

	private void UpdateCourses(CourseData activeCourse)
	{
		activeHolesList.Clear();
		inactiveHolesList.Clear();
		for (int i = 0; i < GameManager.AllCourses.allHoles.Count; i++)
		{
			holes[i].transform.SetParent(inactiveHoles.contentRoot);
			inactiveHolesList.Add(i);
		}
		for (int j = 0; j < activeCourse.Holes.Length; j++)
		{
			int globalIndex = activeCourse.Holes[j].GlobalIndex;
			holes[globalIndex].transform.SetParent(activeHoles.contentRoot);
			activeHolesList.Add(globalIndex);
			inactiveHolesList.Remove(globalIndex);
		}
		activeHoles.ResetDummy();
		inactiveHoles.ResetDummy();
		SortInactiveHoles();
		UpdateHoleLabels();
		UpdateHoleNavigation();
	}

	private void SortInactiveHoles()
	{
		foreach (MatchSetupHole item in from x in inactiveHoles.contentRoot.GetComponentsInChildren<MatchSetupHole>()
			orderby x.holeData.GlobalIndex
			select x)
		{
			item.transform.SetAsLastSibling();
		}
	}

	private void UpdateHoleLabels()
	{
		activeLabel.text = $"{Localization.UI.MATCHSETUP_Label_Active} ({activeHolesList.Count})";
		inactiveLabel.text = $"{Localization.UI.MATCHSETUP_Label_Inactive} ({inactiveHolesList.Count}/{holes.Count})";
	}

	private void UpdateHoleNavigation()
	{
		List<ControllerSelectable> value;
		using (CollectionPool<List<ControllerSelectable>, ControllerSelectable>.Get(out value))
		{
			List<ControllerSelectable> value2;
			using (CollectionPool<List<ControllerSelectable>, ControllerSelectable>.Get(out value2))
			{
				List<Button> value3;
				using (CollectionPool<List<Button>, Button>.Get(out value3))
				{
					categoryRoot.GetComponentsInChildren(value3);
					activeHoles.GetComponentsInChildren(value);
					inactiveHoles.GetComponentsInChildren(value2);
					Button button = startMatchButtons[1];
					int num = value3.Count / 2;
					for (int i = 0; i < value3.Count; i++)
					{
						Button button2 = value3[i];
						Navigation navigation = new Navigation
						{
							mode = Navigation.Mode.Explicit
						};
						if (i < value3.Count - 1)
						{
							navigation.selectOnRight = value3[i + 1];
						}
						if (i > 0)
						{
							navigation.selectOnLeft = value3[i - 1];
						}
						if (value.Count == 0)
						{
							navigation.selectOnDown = value2[0].Selectable;
						}
						else if (value2.Count == 0)
						{
							navigation.selectOnDown = value[0].Selectable;
						}
						else
						{
							navigation.selectOnDown = ((i <= num) ? value[0].Selectable : value2[0].Selectable);
						}
						button2.navigation = navigation;
					}
					for (int j = 0; j < BMath.Max(value.Count, value2.Count); j++)
					{
						ControllerSelectable selectable;
						bool num2 = TryGetSelectable(value, j, clamp: false, out selectable);
						ControllerSelectable selectable2;
						bool flag = TryGetSelectable(value2, j, clamp: false, out selectable2);
						if (num2)
						{
							Navigation navigation2 = new Navigation
							{
								mode = Navigation.Mode.Explicit
							};
							ControllerSelectable selectable3;
							if (j == 0)
							{
								navigation2.selectOnUp = categoryRoot.GetChild(0).GetComponent<Selectable>();
							}
							else if (TryGetSelectable(value, j - 1, clamp: false, out selectable3))
							{
								navigation2.selectOnUp = selectable3.Selectable;
							}
							ControllerSelectable selectable4;
							if (j == value.Count - 1)
							{
								navigation2.selectOnDown = courseRandomToggle;
							}
							else if (TryGetSelectable(value, j + 1, clamp: false, out selectable4))
							{
								navigation2.selectOnDown = selectable4.Selectable;
							}
							if (TryGetSelectable(value2, j, clamp: true, out var selectable5))
							{
								navigation2.selectOnRight = selectable5.Selectable;
							}
							selectable.Selectable.navigation = navigation2;
						}
						if (flag)
						{
							Navigation navigation3 = new Navigation
							{
								mode = Navigation.Mode.Explicit
							};
							ControllerSelectable selectable6;
							if (j == 0)
							{
								navigation3.selectOnUp = categoryRoot.GetChild(categoryRoot.childCount - 1).GetComponent<Selectable>();
							}
							else if (TryGetSelectable(value2, j - 1, clamp: false, out selectable6))
							{
								navigation3.selectOnUp = selectable6.Selectable;
							}
							ControllerSelectable selectable7;
							if (j == value2.Count - 1)
							{
								navigation3.selectOnDown = courseRandomToggle;
							}
							else if (TryGetSelectable(value2, j + 1, clamp: false, out selectable7))
							{
								navigation3.selectOnDown = selectable7.Selectable;
							}
							if (TryGetSelectable(value, j, clamp: true, out var selectable8))
							{
								navigation3.selectOnLeft = selectable8.Selectable;
							}
							selectable2.Selectable.navigation = navigation3;
						}
					}
					button.navigation = new Navigation
					{
						mode = Navigation.Mode.Explicit,
						selectOnUp = courseRandomToggle
					};
					Navigation navigation4 = new Navigation
					{
						mode = Navigation.Mode.Explicit
					};
					Selectable selectable9;
					if (value.Count < value2.Count)
					{
						List<ControllerSelectable> list = value2;
						selectable9 = list[list.Count - 1].Selectable;
					}
					else
					{
						List<ControllerSelectable> list2 = value;
						selectable9 = list2[list2.Count - 1].Selectable;
					}
					navigation4.selectOnUp = selectable9;
					navigation4.selectOnRight = numberOfHolesSlider.Slider;
					navigation4.selectOnDown = button;
					courseRandomToggle.navigation = navigation4;
					Navigation navigation5 = navigation4;
					navigation5.selectOnRight = null;
					navigation5.selectOnLeft = courseRandomToggle;
					numberOfHolesSlider.Slider.navigation = navigation5;
				}
			}
		}
		static bool TryGetSelectable(List<ControllerSelectable> list3, int index, bool clamp, out ControllerSelectable reference)
		{
			reference = null;
			if (list3.Count == 0)
			{
				return false;
			}
			if (clamp)
			{
				index = BMath.Min(index, list3.Count - 1);
			}
			else if (index >= list3.Count)
			{
				return false;
			}
			reference = list3[index];
			return true;
		}
	}

	[Server]
	private void ServerSaveSettings()
	{
		if (!NetworkServer.active)
		{
			Debug.LogWarning("[Server] function 'System.Void MatchSetupMenu::ServerSaveSettings()' called when server was not active");
			return;
		}
		serverValues.customActiveHoles = SerializeCourse(CustomCourseData);
		serverValues.ruleKeys = new List<MatchSetupRules.Rule>();
		serverValues.ruleValues = new List<float>();
		serverValues.spawnChanceKeys = new List<MatchSetupRules.ItemPoolId>();
		serverValues.spawnChanceValues = new List<float>();
		rules.Serialize(serverValues);
		string s = JsonUtility.ToJson(serverValues, prettyPrint: true);
		byte[] bytes = Encoding.UTF8.GetBytes(s);
		SteamRemoteStorage.FileWrite("Lobby.json", bytes);
		static string[] SerializeCourse(CourseData course)
		{
			List<string> value;
			if (course != null && course.Holes != null)
			{
				using (CollectionPool<List<string>, string>.Get(out value))
				{
					HoleData[] array = course.Holes;
					foreach (HoleData holeData in array)
					{
						value.Add(holeData.name);
					}
					return value.ToArray();
				}
			}
			return null;
		}
	}

	private void SteamPlayerRelationshipChanged(ulong guid, Relationship relationship)
	{
		OnPlayersUpdated();
	}

	private void OnPlayersUpdated()
	{
		if (!IsActive || BNetworkManager.IsChangingSceneOrShuttingDown)
		{
			return;
		}
		try
		{
			List<CourseManager.PlayerState> value;
			using (CollectionPool<List<CourseManager.PlayerState>, CourseManager.PlayerState>.Get(out value))
			{
				CourseManager.GetConnectedPlayerStates(value);
				int num = value.Count - players.Count;
				for (int i = 0; i < num; i++)
				{
					MatchSetupPlayer matchSetupPlayer = UnityEngine.Object.Instantiate(playerPrefab);
					matchSetupPlayer.transform.localScale = Vector3.one;
					matchSetupPlayer.menu = this;
					players.Add(matchSetupPlayer);
				}
				List<Selectable> value2;
				using (CollectionPool<List<Selectable>, Selectable>.Get(out value2))
				{
					List<Selectable> value3;
					using (CollectionPool<List<Selectable>, Selectable>.Get(out value3))
					{
						Selectable selectable = null;
						for (int j = 0; j < players.Count; j++)
						{
							MatchSetupPlayer matchSetupPlayer2 = players[j];
							bool flag = j < value.Count && matchSetupPlayer2 != null && value[j].playerGuid != 0 && !string.IsNullOrEmpty(value[j].name);
							matchSetupPlayer2.gameObject.SetActive(flag);
							if (!flag)
							{
								continue;
							}
							CourseManager.PlayerState playerState = value[j];
							matchSetupPlayer2.AssignPlayer(playerState);
							if (playerState.playerGuid == BNetworkManager.LocalPlayerGuidOnServer)
							{
								selectable = matchSetupPlayer2.GetComponent<Selectable>();
							}
							if (ReorderableListElement.Current == null || ReorderableListElement.Current.gameObject != matchSetupPlayer2.gameObject)
							{
								matchSetupPlayer2.transform.SetParent(playerState.isSpectator ? spectatorsList.contentRoot : playersList.contentRoot, worldPositionStays: false);
								if (playerState.isSpectator)
								{
									value3.Add(matchSetupPlayer2.GetComponent<Selectable>());
								}
								else
								{
									value2.Add(matchSetupPlayer2.GetComponent<Selectable>());
								}
							}
						}
						if (base.isHost)
						{
							if (value2.Count > 0)
							{
								value2.Sort(new SelectableChildIndexCmp());
								Selectable downLeft;
								Selectable downRight;
								if (value3.Count > 0)
								{
									downLeft = value3[0];
									downRight = ((value3.Count > 1) ? value3[1] : value3[0]);
								}
								else
								{
									downLeft = playersAreaBotLeft;
									downRight = playersAreaBotRight;
								}
								SetNavigation(playersAreaTopLeft, null, null, null, value2[0]);
								SetNavigation(playersAreaTopRight, null, null, null, (value2.Count > 1) ? value2[1] : value2[0]);
								if (value3.Count == 0)
								{
									Selectable selectable2 = playersAreaBotRight;
									List<Selectable> list = value2;
									SetNavigation(selectable2, null, null, list[list.Count - 1]);
									Selectable selectable3 = playersAreaBotLeft;
									Selectable up;
									if (value2.Count <= 1)
									{
										List<Selectable> list2 = value2;
										up = list2[list2.Count - 1];
									}
									else
									{
										List<Selectable> list3 = value2;
										up = list3[list3.Count - 2];
									}
									SetNavigation(selectable3, null, null, up);
								}
								SetListNavigation(value2, playersAreaTopLeft, playersAreaTopRight, downLeft, downRight);
							}
							if (value3.Count > 0)
							{
								value3.Sort(new SelectableChildIndexCmp());
								Selectable upRight;
								Selectable upLeft;
								if (value2.Count > 0)
								{
									List<Selectable> list4 = value2;
									upRight = list4[list4.Count - 1];
									Selectable selectable4;
									if (value2.Count <= 1)
									{
										List<Selectable> list5 = value2;
										selectable4 = list5[list5.Count - 1];
									}
									else
									{
										List<Selectable> list6 = value2;
										selectable4 = list6[list6.Count - 2];
									}
									upLeft = selectable4;
								}
								else
								{
									upLeft = playersAreaTopLeft;
									upRight = playersAreaTopRight;
								}
								Selectable selectable5 = playersAreaBotRight;
								List<Selectable> list7 = value3;
								SetNavigation(selectable5, null, null, list7[list7.Count - 1]);
								Selectable selectable6 = playersAreaBotLeft;
								Selectable up2;
								if (value3.Count <= 1)
								{
									List<Selectable> list8 = value3;
									up2 = list8[list8.Count - 1];
								}
								else
								{
									List<Selectable> list9 = value3;
									up2 = list9[list9.Count - 2];
								}
								SetNavigation(selectable6, null, null, up2);
								if (value2.Count == 0)
								{
									SetNavigation(playersAreaTopLeft, null, null, null, value3[0]);
									SetNavigation(playersAreaTopRight, null, null, null, (value3.Count > 1) ? value3[1] : value3[0]);
								}
								SetListNavigation(value3, upLeft, upRight, playersAreaBotLeft, playersAreaBotRight);
							}
						}
						else
						{
							bool flag2 = BMath.Max(value2.IndexOf(selectable), value3.IndexOf(selectable)) % 2 == 0;
							SetNavigation(selectable, null, null, flag2 ? playersAreaTopLeft : playersAreaTopRight, flag2 ? playersAreaBotLeft : playersAreaBotRight, clear: true);
							SetNavigation(playersAreaTopLeft, null, null, null, selectable);
							SetNavigation(playersAreaTopRight, null, null, null, selectable);
							SetNavigation(playersAreaBotLeft, null, null, selectable);
							SetNavigation(playersAreaBotRight, null, null, selectable);
						}
						if (base.isServer)
						{
							RefreshStartMatchButton();
						}
					}
				}
			}
		}
		catch (Exception exception)
		{
			Debug.LogError("Caught exception when updating players in match setup!");
			Debug.LogException(exception);
		}
		static void SetListNavigation(List<Selectable> selectables, Selectable selectable7, Selectable selectable8, Selectable selectable10, Selectable selectable11)
		{
			for (int k = 0; k < selectables.Count; k++)
			{
				bool flag3 = k % 2 == 0;
				Selectable up3 = ((k >= 2) ? selectables[k - 2] : (flag3 ? selectable7 : selectable8));
				Selectable right;
				Selectable left;
				if (selectables.Count > 0)
				{
					right = (flag3 ? selectables[BMath.Min(k + 1, selectables.Count - 1)] : null);
					left = ((!flag3) ? selectables[BMath.Max(k - 1, 0)] : null);
				}
				else
				{
					right = null;
					left = null;
				}
				Selectable selectable9;
				if (k > selectables.Count - 3)
				{
					selectable9 = (flag3 ? selectable10 : selectable11);
					SetNavigation(selectable9, null, null, selectables[k]);
				}
				else
				{
					selectable9 = selectables[k + 2];
				}
				SetNavigation(selectables[k], left, right, up3, selectable9, clear: true);
			}
		}
		static void SetNavigation(Selectable selectable7, Selectable left = null, Selectable right = null, Selectable selectable8 = null, Selectable down = null, bool clear = false)
		{
			Navigation navigation = selectable7.navigation;
			navigation.mode = Navigation.Mode.Explicit;
			if (clear || left != null)
			{
				navigation.selectOnLeft = left;
			}
			if (clear || right != null)
			{
				navigation.selectOnRight = right;
			}
			if (clear || selectable8 != null)
			{
				navigation.selectOnUp = selectable8;
			}
			if (clear || down != null)
			{
				navigation.selectOnDown = down;
			}
			selectable7.navigation = navigation;
		}
	}

	private void LoadValues()
	{
		if (!base.isServer)
		{
			return;
		}
		try
		{
			if (SteamRemoteStorage.FileExists("Lobby.json"))
			{
				byte[] bytes = SteamRemoteStorage.FileRead("Lobby.json");
				string json = Encoding.UTF8.GetString(bytes);
				serverValues = JsonUtility.FromJson<ServerValues>(json);
			}
		}
		catch (Exception exception)
		{
			Debug.LogError("Caught exception when deserializing lobby data");
			Debug.LogException(exception);
		}
		if (serverValues == null)
		{
			serverValues = new ServerValues();
			serverValues.serverName = BNetworkManager.LobbyName;
			serverValues.lobbyMode = BNetworkManager.LobbyMode;
		}
		if (serverValues.customActiveHoles != null)
		{
			CustomCourseData.OverrideHoles(GetHoleDataArray(serverValues.customActiveHoles));
		}
		NetworkactiveCourse = BMath.Min(serverValues.activeCourse, GameManager.AllCourses.Courses.Length - 1);
		SetCourse(activeCourse, fromSerialization: true);
		rules.Deserialize(serverValues);
		NetworkmaxPlayers = serverValues.maxPlayers;
		maxPlayersSlider.Initialize(delegate
		{
			NetworkmaxPlayers = (int)maxPlayersSlider.value;
			bool flag2 = false;
			if (maxPlayers < GameManager.RemotePlayers.Count + 1)
			{
				NetworkmaxPlayers = GameManager.RemotePlayers.Count + 1;
				flag2 = true;
			}
			if (maxPlayers > 16)
			{
				NetworkmaxPlayers = 16;
				flag2 = true;
			}
			if (flag2)
			{
				maxPlayersSlider.valueWithoutNotify = maxPlayers;
			}
			OnMaxPlayersSliderValueChange();
		}, serverValues.maxPlayers);
		NetworklobbyMode = serverValues.lobbyMode;
		lobbyModeDropdown.Initialize(delegate
		{
			NetworklobbyMode = (LobbyMode)lobbyModeDropdown.value;
		}, (int)serverValues.lobbyMode);
		BNetworkManager.SetLobbyMode(lobbyMode);
		passwordField.onEndEdit.RemoveAllListeners();
		string serverPassword = (passwordField.text = serverValues.password);
		BClientAuthenticator.serverPassword = serverPassword;
		BNetworkManager.singleton.ServerUpdatePasswordRequired();
		NetworkhasPassword = serverValues.password.Length > 0;
		passwordField.onEndEdit.AddListener(delegate(string value)
		{
			serverValues.password = value;
			BClientAuthenticator.serverPassword = serverValues.password;
			NetworkhasPassword = serverValues.password.Length > 0;
			BNetworkManager.singleton.ServerUpdatePasswordRequired();
		});
		serverNameField.onValueChanged.RemoveAllListeners();
		SetServerName(serverValues.serverName);
		serverNameField.text = BNetworkManager.LobbyName;
		serverNameField.onEndEdit.AddListener(SetServerName);
		NetworkrandomCupNumHoles = ((activeCourse == -1) ? serverValues.randomCupNumHoles : GetCurrentCourseData().Holes.Length);
		Toggle toggle = courseRandomToggle;
		bool isOn = (NetworkrandomEnabled = serverValues.randomEnabled);
		toggle.isOn = isOn;
		static HoleData[] GetHoleDataArray(string[] activeHoles)
		{
			List<HoleData> value;
			using (CollectionPool<List<HoleData>, HoleData>.Get(out value))
			{
				foreach (string holeName in activeHoles)
				{
					HoleData holeData = GameManager.AllCourses.allHoles.FirstOrDefault((HoleData x) => x.name == holeName);
					if (!(holeData == null))
					{
						value.Add(holeData);
					}
				}
				return value.ToArray();
			}
		}
	}

	public override void OnStartClient()
	{
		InitializeCourses();
		maxPlayersSlider.SetLimits(0f, 16f);
		if (base.isServer)
		{
			LoadValues();
		}
		else
		{
			maxPlayersSlider.Initialize(OnMaxPlayersSliderValueChange, maxPlayers);
			OnHasPasswordChange(hasPassword, hasPassword);
			ClientRebuildCourseList();
		}
		numberOfHolesSlider.SetLimits(1f, GameManager.AllCourses.allHoles.Count);
		numberOfHolesSlider.Initialize(delegate
		{
			if (base.isServer && randomEnabled)
			{
				NetworkrandomCupNumHoles = (int)numberOfHolesSlider.value;
			}
			int num = (int)numberOfHolesSlider.value;
			numberOfHolesSlider.SetValueText(num + "/" + activeHolesList.Count);
		}, randomCupNumHoles);
		if (base.isServer)
		{
			rules.Initialize();
		}
		OnActiveCourseChanged(activeCourse, activeCourse);
		OnRandomEnabledChanged(randomEnabled, randomEnabled);
		UpdateHoleNavigation();
		menu.SetActive(value: false);
		CourseManager.PlayerStatesChanged += OnPlayerStatesChanged;
		LocalizationManager.LanguageChanged += OnLanguageChanged;
		BNetworkManager.SteamPlayerRelationshipChanged += SteamPlayerRelationshipChanged;
		playersList.OnElementMoved += OnPlayerMovedToPlayers;
		spectatorsList.OnElementMoved += OnPlayerMovedToSpectate;
		if (!base.isServer)
		{
			SyncList<int> syncList = activeHolesList;
			syncList.OnChange = (Action<SyncList<int>.Operation, int, int>)Delegate.Combine(syncList.OnChange, new Action<SyncList<int>.Operation, int, int>(OnCourseListChange));
			SyncList<int> syncList2 = inactiveHolesList;
			syncList2.OnChange = (Action<SyncList<int>.Operation, int, int>)Delegate.Combine(syncList2.OnChange, new Action<SyncList<int>.Operation, int, int>(OnCourseListChange));
		}
	}

	private void OnPlayerStatesChanged(SyncList<CourseManager.PlayerState>.Operation op, int index, CourseManager.PlayerState state)
	{
		if (!ShouldUpdatePlayerEntries(out var shouldUpdateStartMatchButtons))
		{
			if (shouldUpdateStartMatchButtons)
			{
				RefreshStartMatchButton();
			}
		}
		else
		{
			OnPlayersUpdated();
		}
		bool ShouldUpdatePlayerEntries(out bool reference)
		{
			reference = false;
			if (!IsActive)
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
					reference = true;
					return true;
				}
				reference = playerState.isSpectator != state.isSpectator;
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

	private void OnPlayerMovedToSpectate(ReorderableListElement playerElement)
	{
		if (GameManager.TryFindPlayerByGuid(playerElement.GetComponent<MatchSetupPlayer>().Guid, out var playerInfo) && (playerInfo == null || !SetPlayerSpectator(playerInfo.AsGolfer, isSpectator: true)))
		{
			playerElement.transform.SetParent(playersList.contentRoot);
		}
	}

	private void OnPlayerMovedToPlayers(ReorderableListElement playerElement)
	{
		if (GameManager.TryFindPlayerByGuid(playerElement.GetComponent<MatchSetupPlayer>().Guid, out var playerInfo) && (playerInfo == null || !SetPlayerSpectator(playerInfo.AsGolfer, isSpectator: false)))
		{
			playerElement.transform.SetParent(spectatorsList.contentRoot);
		}
	}

	public static void SetLocalPlayerSpectator()
	{
		if (!SingletonNetworkBehaviour<MatchSetupMenu>.HasInstance)
		{
			Debug.LogError("No instance of MatchSetupMenu was found!");
		}
		else
		{
			SingletonNetworkBehaviour<MatchSetupMenu>.Instance.SetPlayerSpectator(GameManager.LocalPlayerAsGolfer, isSpectator: true);
		}
	}

	private bool SetPlayerSpectator(PlayerGolfer player, bool isSpectator)
	{
		if (!base.isHost && !player.isLocalPlayer)
		{
			return false;
		}
		if (base.isHost)
		{
			CourseManager.SetPlayerSpectator(player, isSpectator);
		}
		else
		{
			CmdRequestSpectator(isSpectator);
		}
		return true;
	}

	[Command(requiresAuthority = false)]
	private void CmdRequestSpectator(bool isSpectator, NetworkConnectionToClient sender = null)
	{
		if (base.isServer && base.isClient)
		{
			UserCode_CmdRequestSpectator__Boolean__NetworkConnectionToClient(isSpectator, sender);
			return;
		}
		NetworkWriterPooled writer = NetworkWriterPool.Get();
		writer.WriteBool(isSpectator);
		SendCommandInternal("System.Void MatchSetupMenu::CmdRequestSpectator(System.Boolean,Mirror.NetworkConnectionToClient)", -2059610292, writer, 0, requiresAuthority: false);
		NetworkWriterPool.Return(writer);
	}

	private void OnLanguageChanged()
	{
		UpdateHoleLabels();
		OnActiveCourseChanged(activeCourse, activeCourse);
		UpdateCourseTooltips();
	}

	private void OnMaxPlayersSliderValueChange()
	{
		maxPlayersSlider.SetValueText(maxPlayersSlider.value.ToString());
		bool active = maxPlayersSlider.value > 8f;
		playerCountRecommendation.SetActive(active);
		warningBackdrop.SetActive(active);
		warningIcon.SetActive(active);
	}

	public void StartOrCancelMatch()
	{
		if (!NetworkServer.active)
		{
			return;
		}
		if (SingletonBehaviour<DrivingRangeManager>.HasInstance)
		{
			if (randomEnabled)
			{
				List<HoleData> value;
				using (CollectionPool<List<HoleData>, HoleData>.Get(out value))
				{
					value.AddRange(GetCurrentCourseData().Holes.Distinct());
					value.Shuffle();
					while (value.Count > randomCupNumHoles)
					{
						value.RemoveAt(value.Count - 1);
					}
					RandomCourseData.OverrideHoles(value.ToArray());
					CourseManager.ServerSetCourse(RandomCourseData);
				}
			}
			else
			{
				CourseManager.ServerSetCourse(GetCurrentCourseData());
			}
			ServerSaveSettings();
			CourseManager.StartCourse();
			DisableButton(playSfx: true);
		}
		else
		{
			FullScreenMessage.Show(Localization.UI.MATCHSETUP_ConfirmCancelPrompt, new FullScreenMessage.ButtonEntry(Localization.UI.MISC_Yes, delegate
			{
				DisableButton(playSfx: false);
				FullScreenMessage.Hide();
				SetEnabled(enabled: false);
				PauseMenu.Unpause();
				CourseManager.EndCourse();
			}), new FullScreenMessage.ButtonEntry(Localization.UI.MISC_Cancel, FullScreenMessage.Hide));
		}
		void DisableButton(bool playSfx)
		{
			Button[] array = startMatchButtons;
			foreach (Button button in array)
			{
				if (playSfx && button.isActiveAndEnabled)
				{
					RuntimeManager.PlayOneShot(GameManager.AudioSettings.StartMatchButtonSelect);
				}
				button.interactable = false;
			}
		}
	}

	private void OnMaxPlayersChange(int prev, int curr)
	{
		if (!base.isServer)
		{
			maxPlayersSlider.value = curr;
			return;
		}
		serverValues.maxPlayers = curr;
		BNetworkManager.singleton.ServerSetMaxPlayers(curr);
	}

	private void OnLobbyModeChange(LobbyMode prev, LobbyMode curr)
	{
		if (!base.isServer)
		{
			lobbyModeDropdown.value = (int)curr;
			return;
		}
		serverValues.lobbyMode = curr;
		BNetworkManager.SetLobbyMode(curr);
	}

	private void OnHasPasswordChange(bool prev, bool curr)
	{
		if (!base.isServer)
		{
			passwordField.text = (curr ? "password" : string.Empty);
		}
	}

	private void SetServerName(string newName)
	{
		GameManager.FilterProfanity(newName, out newName);
		BNetworkManager.LobbyName = newName;
		NetworkserverName = (serverValues.serverName = BNetworkManager.LobbyName);
		serverNameField.SetTextWithoutNotify(BNetworkManager.LobbyName);
	}

	private void OnServerNameChange(string previousName, string currentName)
	{
		Debug.Log("Server name set to " + currentName);
		if (!base.isServer)
		{
			GameManager.FilterProfanity(currentName, out currentName);
			serverNameField.text = currentName;
		}
	}

	private void OnActiveCourseChanged(int previousCourse, int currentCourseIndex)
	{
		LocalizedString courseLocalizedString = GetCourseLocalizedString(currentCourseIndex);
		currentCourseLocalizeStringEvent.StringReference = courseLocalizedString;
		if (!base.isServer)
		{
			SelectCategory(categoryRoot, currentCourseIndex);
		}
		if (PauseMenu.IsPaused)
		{
			SingletonBehaviour<PauseMenu>.Instance.UpdateGameInfoLabels();
		}
		CourseData currentCourseData = GetCurrentCourseData();
		if (currentCourseData == CustomCourseData)
		{
			courseLabel.StringReference = Localization.UI.HOLE_INFO_CourseName_Custom_Ref;
		}
		else
		{
			courseLabel.StringReference = currentCourseData.LocalizedName;
		}
		courseLabel.RefreshString();
		UpdateRandomMaxHolesSlider();
	}

	public static LocalizedString GetCourseLocalizedString(int currentCourse)
	{
		if (currentCourse >= 0)
		{
			return GameManager.AllCourses.Courses[currentCourse].LocalizedName;
		}
		if (currentCourse == -1)
		{
			return Localization.UI.MATCHSETUP_Button_Custom_Ref;
		}
		return Localization.UI.MATCHSETUP_Button_Random_Ref;
	}

	private void OnCourseListChange(SyncList<int>.Operation op, int i, int value)
	{
		ClientRebuildCourseList();
	}

	private void ClientRebuildCourseList()
	{
		foreach (int activeHoles in activeHolesList)
		{
			MatchSetupHole matchSetupHole = holes[activeHoles];
			matchSetupHole.transform.SetParent(this.activeHoles.contentRoot);
			matchSetupHole.transform.SetAsLastSibling();
		}
		foreach (int inactiveHoles in inactiveHolesList)
		{
			MatchSetupHole matchSetupHole2 = holes[inactiveHoles];
			matchSetupHole2.transform.SetParent(this.inactiveHoles.contentRoot);
			matchSetupHole2.transform.SetAsLastSibling();
		}
		SortInactiveHoles();
		UpdateHoleLabels();
	}

	private void OnRandomCupNumHolesChanged(int prev, int curr)
	{
		if (base.isServer)
		{
			serverValues.randomCupNumHoles = randomCupNumHoles;
			if (activeCourse >= 0 && randomCupNumHoles != GetRandomMaxHolesSliderDefaultValue(GetCurrentCourseData()))
			{
				OnHoleOrderUpdate();
			}
		}
		else
		{
			UpdateRandomMaxHolesSlider();
		}
	}

	public MatchSetupMenu()
	{
		InitSyncObject(activeHolesList);
		InitSyncObject(inactiveHolesList);
		_Mirror_SyncVarHookDelegate_maxPlayers = OnMaxPlayersChange;
		_Mirror_SyncVarHookDelegate_lobbyMode = OnLobbyModeChange;
		_Mirror_SyncVarHookDelegate_hasPassword = OnHasPasswordChange;
		_Mirror_SyncVarHookDelegate_serverName = OnServerNameChange;
		_Mirror_SyncVarHookDelegate_activeCourse = OnActiveCourseChanged;
		_Mirror_SyncVarHookDelegate_randomCupNumHoles = OnRandomCupNumHolesChanged;
		_Mirror_SyncVarHookDelegate_randomEnabled = OnRandomEnabledChanged;
	}

	public override bool Weaved()
	{
		return true;
	}

	protected void UserCode_CmdRequestSpectator__Boolean__NetworkConnectionToClient(bool isSpectator, NetworkConnectionToClient sender)
	{
		if (!serverRequestSpectatorCommandRateLimiter.RegisterHit(sender))
		{
			return;
		}
		foreach (NetworkIdentity item in sender.owned)
		{
			if (item.TryGetComponent<PlayerGolfer>(out var component))
			{
				CourseManager.SetPlayerSpectator(component, isSpectator);
				return;
			}
		}
		throw new Exception("Trying to set spectator on player without player object!");
	}

	protected static void InvokeUserCode_CmdRequestSpectator__Boolean__NetworkConnectionToClient(NetworkBehaviour obj, NetworkReader reader, NetworkConnectionToClient senderConnection)
	{
		if (!NetworkServer.active)
		{
			Debug.LogError("Command CmdRequestSpectator called on client.");
		}
		else
		{
			((MatchSetupMenu)obj).UserCode_CmdRequestSpectator__Boolean__NetworkConnectionToClient(reader.ReadBool(), senderConnection);
		}
	}

	static MatchSetupMenu()
	{
		RemoteProcedureCalls.RegisterCommand(typeof(MatchSetupMenu), "System.Void MatchSetupMenu::CmdRequestSpectator(System.Boolean,Mirror.NetworkConnectionToClient)", InvokeUserCode_CmdRequestSpectator__Boolean__NetworkConnectionToClient, requiresAuthority: false);
	}

	public override void SerializeSyncVars(NetworkWriter writer, bool forceAll)
	{
		base.SerializeSyncVars(writer, forceAll);
		if (forceAll)
		{
			writer.WriteVarInt(maxPlayers);
			GeneratedNetworkCode._Write_LobbyMode(writer, lobbyMode);
			writer.WriteBool(hasPassword);
			writer.WriteString(serverName);
			writer.WriteVarInt(activeCourse);
			writer.WriteVarInt(randomCupNumHoles);
			writer.WriteBool(randomEnabled);
			return;
		}
		writer.WriteVarULong(syncVarDirtyBits);
		if ((syncVarDirtyBits & 1L) != 0L)
		{
			writer.WriteVarInt(maxPlayers);
		}
		if ((syncVarDirtyBits & 2L) != 0L)
		{
			GeneratedNetworkCode._Write_LobbyMode(writer, lobbyMode);
		}
		if ((syncVarDirtyBits & 4L) != 0L)
		{
			writer.WriteBool(hasPassword);
		}
		if ((syncVarDirtyBits & 8L) != 0L)
		{
			writer.WriteString(serverName);
		}
		if ((syncVarDirtyBits & 0x10L) != 0L)
		{
			writer.WriteVarInt(activeCourse);
		}
		if ((syncVarDirtyBits & 0x20L) != 0L)
		{
			writer.WriteVarInt(randomCupNumHoles);
		}
		if ((syncVarDirtyBits & 0x40L) != 0L)
		{
			writer.WriteBool(randomEnabled);
		}
	}

	public override void DeserializeSyncVars(NetworkReader reader, bool initialState)
	{
		base.DeserializeSyncVars(reader, initialState);
		if (initialState)
		{
			GeneratedSyncVarDeserialize(ref maxPlayers, _Mirror_SyncVarHookDelegate_maxPlayers, reader.ReadVarInt());
			GeneratedSyncVarDeserialize(ref lobbyMode, _Mirror_SyncVarHookDelegate_lobbyMode, GeneratedNetworkCode._Read_LobbyMode(reader));
			GeneratedSyncVarDeserialize(ref hasPassword, _Mirror_SyncVarHookDelegate_hasPassword, reader.ReadBool());
			GeneratedSyncVarDeserialize(ref serverName, _Mirror_SyncVarHookDelegate_serverName, reader.ReadString());
			GeneratedSyncVarDeserialize(ref activeCourse, _Mirror_SyncVarHookDelegate_activeCourse, reader.ReadVarInt());
			GeneratedSyncVarDeserialize(ref randomCupNumHoles, _Mirror_SyncVarHookDelegate_randomCupNumHoles, reader.ReadVarInt());
			GeneratedSyncVarDeserialize(ref randomEnabled, _Mirror_SyncVarHookDelegate_randomEnabled, reader.ReadBool());
			return;
		}
		long num = (long)reader.ReadVarULong();
		if ((num & 1L) != 0L)
		{
			GeneratedSyncVarDeserialize(ref maxPlayers, _Mirror_SyncVarHookDelegate_maxPlayers, reader.ReadVarInt());
		}
		if ((num & 2L) != 0L)
		{
			GeneratedSyncVarDeserialize(ref lobbyMode, _Mirror_SyncVarHookDelegate_lobbyMode, GeneratedNetworkCode._Read_LobbyMode(reader));
		}
		if ((num & 4L) != 0L)
		{
			GeneratedSyncVarDeserialize(ref hasPassword, _Mirror_SyncVarHookDelegate_hasPassword, reader.ReadBool());
		}
		if ((num & 8L) != 0L)
		{
			GeneratedSyncVarDeserialize(ref serverName, _Mirror_SyncVarHookDelegate_serverName, reader.ReadString());
		}
		if ((num & 0x10L) != 0L)
		{
			GeneratedSyncVarDeserialize(ref activeCourse, _Mirror_SyncVarHookDelegate_activeCourse, reader.ReadVarInt());
		}
		if ((num & 0x20L) != 0L)
		{
			GeneratedSyncVarDeserialize(ref randomCupNumHoles, _Mirror_SyncVarHookDelegate_randomCupNumHoles, reader.ReadVarInt());
		}
		if ((num & 0x40L) != 0L)
		{
			GeneratedSyncVarDeserialize(ref randomEnabled, _Mirror_SyncVarHookDelegate_randomEnabled, reader.ReadBool());
		}
	}
}
