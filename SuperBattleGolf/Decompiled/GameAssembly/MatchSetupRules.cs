using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using Cysharp.Threading.Tasks;
using Mirror;
using TMPro;
using UnityEngine;
using UnityEngine.Localization;
using UnityEngine.Localization.Components;
using UnityEngine.Pool;
using UnityEngine.UI;

public class MatchSetupRules : SingletonNetworkBehaviour<MatchSetupRules>
{
	[Serializable]
	public struct ItemPoolId
	{
		public ItemType itemType;

		public int itemPoolIndex;

		public static ItemPoolId Get(int index, ItemType itemType)
		{
			return new ItemPoolId
			{
				itemPoolIndex = index,
				itemType = itemType
			};
		}
	}

	public enum Rule
	{
		Countdown,
		OutOfBounds,
		MaxTimeBasedOnPar,
		PlayerSpeed,
		CartSpeed,
		SwingPower,
		OverchargeSidespin,
		HomingShots,
		KnockoutSpeedBoost,
		HitOtherPlayers,
		HitOtherPlayersBalls,
		ConsoleCommands,
		OnOrBelowPar,
		Speedrun,
		ChipIn,
		Knockouts,
		Comeback,
		RepeatRecoveryProtection,
		DominationProtection,
		WhiteFlag,
		Wind,
		Count
	}

	public enum RuleCategory
	{
		BonusScore,
		Time,
		Player,
		Battle,
		Protections,
		Wind,
		Cheats,
		Count
	}

	public enum Preset
	{
		Invalid = -1,
		Classic,
		ProGolf,
		Custom
	}

	[Serializable]
	private struct PresetButton
	{
		public Button button;

		public Preset preset;
	}

	public static readonly RuleCategory[] RuleCategoryLookup = new RuleCategory[21]
	{
		RuleCategory.Time,
		RuleCategory.Time,
		RuleCategory.Time,
		RuleCategory.Player,
		RuleCategory.Player,
		RuleCategory.Player,
		RuleCategory.Player,
		RuleCategory.Battle,
		RuleCategory.Battle,
		RuleCategory.Battle,
		RuleCategory.Battle,
		RuleCategory.Cheats,
		RuleCategory.BonusScore,
		RuleCategory.BonusScore,
		RuleCategory.BonusScore,
		RuleCategory.BonusScore,
		RuleCategory.BonusScore,
		RuleCategory.Protections,
		RuleCategory.Protections,
		RuleCategory.Protections,
		RuleCategory.Wind
	};

	[SerializeField]
	private GameObject rulesTab;

	[SerializeField]
	private GridNavigationGroup gridNavigationGroup;

	[Header("Bonus Score")]
	public DropdownOption onOrBelowPar;

	public DropdownOption speedrun;

	public DropdownOption chipIn;

	public DropdownOption knockouts;

	public DropdownOption comeback;

	[Header("Time")]
	public SliderOption countdown;

	public SliderOption outOfBounds;

	public DropdownOption maxTimeBasedOnPar;

	[Header("Player")]
	public SliderOption playerSpeed;

	public SliderOption cartSpeed;

	public SliderOption swingPower;

	public DropdownOption overchargeSideSpin;

	[Header("Battle")]
	public DropdownOption homingShots;

	public DropdownOption knockoutSpeedBoost;

	public DropdownOption hitOtherPlayers;

	public DropdownOption hitOtherPlayersBalls;

	[Header("Protections")]
	public DropdownOption repeatRecoveryProtection;

	public DropdownOption dominationProtection;

	public DropdownOption whiteFlag;

	[Header("Wind")]
	public DropdownOption wind;

	[Header("Cheats")]
	public DropdownOption consoleCommands;

	[Header("Items")]
	public ItemSpawnerSettings itemSpawnerSettings;

	public ItemSpawnerSettings mobilityItemSpawnerSettings;

	public SetLayoutInformer itemLayoutInformer;

	public Transform[] itemTabParents;

	public RectTransform itemsParent;

	public GridLayoutGroup itemGridLayout;

	public Button itemTabPrefab;

	public GameObject itemPrefab;

	[SerializeField]
	[DynamicElementName("preset")]
	private PresetButton[] presetButtons;

	public ItemType[] itemOrder;

	public int[] itemOrderLookup;

	public Button resetItemSpawnChancesButton;

	[Header("Labels")]
	public LocalizeStringEvent rulesLabelLobby;

	[Header("Tooltip")]
	public UiTooltip tooltip;

	private readonly Dictionary<Preset, Button> buttonsPerPreset = new Dictionary<Preset, Button>();

	[SyncVar(hook = "OnPresetChanged")]
	private Preset currentPreset;

	private SyncDictionary<Rule, float> rules = new SyncDictionary<Rule, float>();

	private static Dictionary<Rule, float> serverPersistentRules = new Dictionary<Rule, float>();

	private SyncDictionary<ItemPoolId, float> spawnChanceWeights = new SyncDictionary<ItemPoolId, float>();

	private Dictionary<int, float> totalWeightPerPool = new Dictionary<int, float>();

	private static Dictionary<ItemPoolId, float> serverPersistentSpawnChanceWeights = new Dictionary<ItemPoolId, float>();

	private bool currentItemPoolDirty;

	private int currentItemPoolIndex = 1;

	private Dictionary<Rule, SliderOption> sliderLookup = new Dictionary<Rule, SliderOption>();

	private Dictionary<Rule, DropdownOption> onOffDropdownLookup = new Dictionary<Rule, DropdownOption>();

	private Dictionary<Rule, DropdownOption> dropdownLookup = new Dictionary<Rule, DropdownOption>();

	private List<SliderOption> spawnChanceSliders = new List<SliderOption>();

	private List<Button> itemTabs = new List<Button>();

	private bool isInitialized;

	private bool isPresetSuppressed;

	public static bool CheatsWarningShowed;

	[HideInInspector]
	public bool IsRulesPopulated;

	public Action<Preset, Preset> _Mirror_SyncVarHookDelegate_currentPreset;

	public Preset CurrentPreset => currentPreset;

	public Preset NetworkcurrentPreset
	{
		get
		{
			return currentPreset;
		}
		[param: In]
		set
		{
			GeneratedSyncVarSetter(value, ref currentPreset, 1uL, _Mirror_SyncVarHookDelegate_currentPreset);
		}
	}

	private event Action UpdateItemSpawnersLoc;

	public static event Action RulesPopulated;

	public static event Action RulesChanged;

	public static bool IsCheatsEnabled()
	{
		if (!SingletonNetworkBehaviour<MatchSetupRules>.HasInstance)
		{
			return false;
		}
		return SingletonNetworkBehaviour<MatchSetupRules>.Instance.GetValueAsBoolInternal(Rule.ConsoleCommands);
	}

	protected override void OnValidate()
	{
		base.OnValidate();
		itemOrderLookup = new int[itemOrder.Length];
		for (int i = 0; i < itemOrder.Length; i++)
		{
			itemOrderLookup[i] = Array.IndexOf(itemOrder, (ItemType)(i + 1));
		}
	}

	protected override void Awake()
	{
		base.Awake();
		PresetButton[] array = presetButtons;
		for (int i = 0; i < array.Length; i++)
		{
			PresetButton presetButton = array[i];
			buttonsPerPreset.Add(presetButton.preset, presetButton.button);
			presetButton.button.onClick.RemoveAllListeners();
			presetButton.button.onClick.AddListener(delegate
			{
				SetPreset(presetButton.preset);
			});
		}
		itemLayoutInformer.SetLayoutHorizontalCalled += OnItemLayoutSet;
	}

	public void Initialize()
	{
		bool activeSelf = rulesTab.activeSelf;
		rulesTab.SetActive(value: true);
		if (base.isServer)
		{
			foreach (KeyValuePair<Rule, float> serverPersistentRule in serverPersistentRules)
			{
				rules[serverPersistentRule.Key] = serverPersistentRule.Value;
			}
			serverPersistentRules.Clear();
			foreach (KeyValuePair<ItemPoolId, float> serverPersistentSpawnChanceWeight in serverPersistentSpawnChanceWeights)
			{
				SetSpawnChance(serverPersistentSpawnChanceWeight.Key.itemPoolIndex, serverPersistentSpawnChanceWeight.Key.itemType, serverPersistentSpawnChanceWeight.Value);
			}
			serverPersistentSpawnChanceWeights.Clear();
			IsRulesPopulated = true;
			MatchSetupRules.RulesPopulated?.Invoke();
		}
		bool flag = isPresetSuppressed;
		isPresetSuppressed = true;
		InitDropdownOnOff(onOrBelowPar, Rule.OnOrBelowPar);
		InitDropdownOnOff(speedrun, Rule.Speedrun);
		InitDropdownOnOff(chipIn, Rule.ChipIn);
		InitDropdownOnOff(knockouts, Rule.Knockouts);
		InitSlider(countdown, Rule.Countdown, "{0}s", (float x) => SnapTo(x, 5f));
		InitSlider(outOfBounds, Rule.OutOfBounds, "{0}s", OutOfBoundsSnapping);
		InitDropdownOnOff(maxTimeBasedOnPar, Rule.MaxTimeBasedOnPar);
		InitSlider(playerSpeed, Rule.PlayerSpeed, "{0}%", (float x) => SnapTo(x, 0.1f), 100f);
		InitSlider(cartSpeed, Rule.CartSpeed, "{0}%", (float x) => SnapTo(x, 0.1f), 100f);
		InitSlider(swingPower, Rule.SwingPower, "{0}%", (float x) => SnapTo(x, 0.1f), 100f);
		InitDropdownOnOff(overchargeSideSpin, Rule.OverchargeSidespin);
		InitDropdownOnOff(homingShots, Rule.HomingShots);
		InitDropdownOnOff(knockoutSpeedBoost, Rule.KnockoutSpeedBoost);
		InitDropdownOnOff(hitOtherPlayers, Rule.HitOtherPlayers);
		InitDropdownOnOff(hitOtherPlayersBalls, Rule.HitOtherPlayersBalls);
		InitDropdownOnOff(comeback, Rule.Comeback);
		InitDropdownOnOff(repeatRecoveryProtection, Rule.RepeatRecoveryProtection);
		InitDropdownOnOff(dominationProtection, Rule.DominationProtection);
		InitDropdownOnOff(whiteFlag, Rule.WhiteFlag);
		InitDropdown(wind, Rule.Wind);
		InitDropdownOnOff(consoleCommands, Rule.ConsoleCommands);
		UpdateRule(Rule.ConsoleCommands);
		UpdateRule(Rule.HitOtherPlayers);
		Queue<Transform> tabParentQueue = new Queue<Transform>();
		Transform[] array = itemTabParents;
		foreach (Transform transform in array)
		{
			transform.gameObject.SetActive(value: false);
			tabParentQueue.Enqueue(transform);
		}
		tabParentQueue.Peek().gameObject.SetActive(value: true);
		int num2 = 0;
		InstantiateSpawnChanceTab(delegate(TMP_Text label)
		{
			label.text = Localization.UI.MATCHSETUP_Title_AheadOwnBall;
		}, 0);
		RegisterItemPool(itemSpawnerSettings.AheadOfBallItemPool, 0);
		num2++;
		foreach (ItemSpawnerSettings.ItemPoolData itemPool in itemSpawnerSettings.ItemPools)
		{
			ItemSpawnerSettings.ItemPoolData pool = itemPool;
			InstantiateSpawnChanceTab(UpdateLocString, num2);
			RegisterItemPool(pool.pool, num2);
			num2++;
			void UpdateLocString(TMP_Text label)
			{
				label.text = GetPoolLocString(pool);
			}
		}
		InstantiateSpawnChanceTab(delegate(TMP_Text label)
		{
			label.text = Localization.UI.MATCHSETUP_Title_MobilityItemBoxes;
		}, 5);
		RegisterItemPool(mobilityItemSpawnerSettings.ItemPools[0].pool, 5);
		itemTabs[5].transform.SetAsLastSibling();
		itemTabs[0].transform.SetAsLastSibling();
		GameSettings.GeneralSettings.DistanceUnitChanged = (Action)Delegate.Combine(GameSettings.GeneralSettings.DistanceUnitChanged, new Action(DistanceUnitChanged));
		for (int num3 = 0; num3 < itemOrder.Length; num3++)
		{
			GameObject gameObject = UnityEngine.Object.Instantiate(itemPrefab);
			gameObject.transform.SetParent(itemsParent);
			gameObject.transform.localScale = Vector3.one;
			SliderOption componentInChildren = gameObject.GetComponentInChildren<SliderOption>();
			spawnChanceSliders.Add(componentInChildren);
			if (GameManager.AllItems.TryGetItemData(itemOrder[num3], out var itemData))
			{
				gameObject.GetComponentInChildren<LocalizeStringEvent>().StringReference = itemData.LocalizedName;
				itemData.LocalizedName.GetLocalizedString();
			}
		}
		isInitialized = true;
		SyncDictionary<Rule, float> syncDictionary = rules;
		syncDictionary.OnChange = (Action<SyncIDictionary<Rule, float>.Operation, Rule, float>)Delegate.Combine(syncDictionary.OnChange, new Action<SyncIDictionary<Rule, float>.Operation, Rule, float>(OnRulesChanged));
		SyncDictionary<ItemPoolId, float> syncDictionary2 = spawnChanceWeights;
		syncDictionary2.OnSet = (Action<ItemPoolId, float>)Delegate.Combine(syncDictionary2.OnSet, new Action<ItemPoolId, float>(SpawnChanceUpdated));
		SyncDictionary<ItemPoolId, float> syncDictionary3 = spawnChanceWeights;
		syncDictionary3.OnAdd = (Action<ItemPoolId>)Delegate.Combine(syncDictionary3.OnAdd, new Action<ItemPoolId>(SpawnChanceUpdated));
		LocalizationManager.LanguageChanged += OnLanguageChanged;
		isPresetSuppressed = flag;
		OnPresetChanged(Preset.Invalid, currentPreset);
		UpdateCurrentItemPool(currentItemPoolIndex);
		CheckAndShowCheatsWarning();
		rulesTab.SetActive(activeSelf);
		UpdateTooltips();
		if (base.isServer)
		{
			BNetworkManager.singleton.ServerUpdateRules();
		}
		void InstantiateSpawnChanceTab(Action<TMP_Text> UpdateLoc, int poolIndex)
		{
			Transform transform2;
			switch (poolIndex)
			{
			case 0:
				transform2 = itemTabParents[^1];
				break;
			case 5:
				transform2 = itemTabParents[^1];
				break;
			default:
				transform2 = tabParentQueue.Peek();
				if (transform2.childCount >= 3)
				{
					tabParentQueue.Dequeue();
					transform2 = tabParentQueue.Peek();
					transform2.gameObject.SetActive(value: true);
				}
				break;
			}
			Button button = UnityEngine.Object.Instantiate(itemTabPrefab, transform2);
			itemTabs.Add(button);
			button.transform.GetChild(0).gameObject.SetActive(value: false);
			TMP_Text label = button.GetComponentInChildren<TMP_Text>();
			UpdateLoc(label);
			UpdateItemSpawnersLoc += delegate
			{
				UpdateLoc(label);
			};
			int itemPoolIndex = poolIndex;
			button.onClick.AddListener(delegate
			{
				UpdateCurrentItemPool(itemPoolIndex);
			});
		}
		void RegisterItemPool(ItemPool itemPool, int poolIndex)
		{
			if (base.isServer)
			{
				ItemPool.ItemSpawnChance[] spawnChances = itemPool.SpawnChances;
				for (int i = 0; i < spawnChances.Length; i++)
				{
					ItemPool.ItemSpawnChance itemSpawnChance = spawnChances[i];
					spawnChanceWeights.TryAdd(ItemPoolId.Get(poolIndex, itemSpawnChance.item), itemSpawnChance.spawnChanceWeight);
				}
			}
			UpdateTotalWeightForPool(poolIndex);
		}
	}

	public string GetPoolLocString(ItemSpawnerSettings.ItemPoolData pool)
	{
		int distanceInCurrentUnits = GameSettings.All.General.GetDistanceInCurrentUnits(pool.minDistanceBehindLeader);
		string arg = string.Format(GameSettings.All.General.GetLocalizedDistanceUnitNameFull(), distanceInCurrentUnits);
		if (!(pool.minDistanceBehindLeader > float.Epsilon))
		{
			return Localization.UI.MATCHSETUP_Title_Ahead;
		}
		return string.Format(Localization.UI.MATCHSETUP_Title_Behind, arg);
	}

	private void UpdateTooltips()
	{
		tooltip.DeregisterTooltip(onOrBelowPar.GetComponent<RectTransform>());
		tooltip.DeregisterTooltip(speedrun.GetComponent<RectTransform>());
		tooltip.DeregisterTooltip(chipIn.GetComponent<RectTransform>());
		tooltip.DeregisterTooltip(knockouts.GetComponent<RectTransform>());
		tooltip.DeregisterTooltip(countdown.GetComponent<RectTransform>());
		tooltip.DeregisterTooltip(outOfBounds.GetComponent<RectTransform>());
		tooltip.DeregisterTooltip(maxTimeBasedOnPar.GetComponent<RectTransform>());
		tooltip.DeregisterTooltip(overchargeSideSpin.GetComponent<RectTransform>());
		tooltip.DeregisterTooltip(homingShots.GetComponent<RectTransform>());
		tooltip.DeregisterTooltip(knockoutSpeedBoost.GetComponent<RectTransform>());
		tooltip.DeregisterTooltip(hitOtherPlayers.GetComponent<RectTransform>());
		tooltip.DeregisterTooltip(hitOtherPlayersBalls.GetComponent<RectTransform>());
		tooltip.DeregisterTooltip(comeback.GetComponent<RectTransform>());
		tooltip.DeregisterTooltip(repeatRecoveryProtection.GetComponent<RectTransform>());
		tooltip.DeregisterTooltip(dominationProtection.GetComponent<RectTransform>());
		tooltip.DeregisterTooltip(whiteFlag.GetComponent<RectTransform>());
		tooltip.DeregisterTooltip(wind.GetComponent<RectTransform>());
		tooltip.DeregisterTooltip(consoleCommands.GetComponent<RectTransform>());
		tooltip.RegisterTooltip(onOrBelowPar.GetComponent<RectTransform>(), Localization.UI.MATCHSETUP_Tooltip_OnOrBelowPar);
		tooltip.RegisterTooltip(speedrun.GetComponent<RectTransform>(), Localization.UI.MATCHSETUP_Tooltip_Speedrun);
		tooltip.RegisterTooltip(chipIn.GetComponent<RectTransform>(), Localization.UI.MATCHSETUP_Tooltip_ChipIn);
		tooltip.RegisterTooltip(knockouts.GetComponent<RectTransform>(), Localization.UI.MATCHSETUP_Tooltip_Knockouts);
		tooltip.RegisterTooltip(countdown.GetComponent<RectTransform>(), Localization.UI.MATCHSETUP_Tooltip_Countdown);
		tooltip.RegisterTooltip(outOfBounds.GetComponent<RectTransform>(), Localization.UI.MATCHSETUP_Tooltip_OutOfBounds);
		tooltip.RegisterTooltip(maxTimeBasedOnPar.GetComponent<RectTransform>(), Localization.UI.MATCHSETUP_Tooltip_MaxTimeBasedOnPar);
		tooltip.RegisterTooltip(overchargeSideSpin.GetComponent<RectTransform>(), Localization.UI.MATCHSETUP_Tooltip_OverchargeSidespin);
		tooltip.RegisterTooltip(homingShots.GetComponent<RectTransform>(), Localization.UI.MATCHSETUP_Tooltip_HomingShots);
		tooltip.RegisterTooltip(knockoutSpeedBoost.GetComponent<RectTransform>(), Localization.UI.MATCHSETUP_Tooltip_KnockoutSpeedBoost);
		tooltip.RegisterTooltip(hitOtherPlayers.GetComponent<RectTransform>(), Localization.UI.MATCHSETUP_Tooltip_HitOtherPlayers);
		tooltip.RegisterTooltip(hitOtherPlayersBalls.GetComponent<RectTransform>(), Localization.UI.MATCHSETUP_Tooltip_HitOtherPlayersBalls);
		tooltip.RegisterTooltip(comeback.GetComponent<RectTransform>(), Localization.UI.MATCHSETUP_Tooltip_Comeback);
		tooltip.RegisterTooltip(repeatRecoveryProtection.GetComponent<RectTransform>(), Localization.UI.MATCHSETUP_Tooltip_RepeatRecoveryProtection);
		tooltip.RegisterTooltip(dominationProtection.GetComponent<RectTransform>(), string.Format(Localization.UI.MATCHSETUP_Tooltip_DominationProtection, GameManager.MatchSettings.RedShieldKnockoutStreak));
		tooltip.RegisterTooltip(whiteFlag.GetComponent<RectTransform>(), Localization.UI.MATCHSETUP_Tooltip_WhiteFlag);
		tooltip.RegisterTooltip(wind.GetComponent<RectTransform>(), Localization.UI.MATCHSETUP_Tooltip_Wind);
		tooltip.RegisterTooltip(consoleCommands.GetComponent<RectTransform>(), Localization.UI.MATCHSETUP_Tooltip_ConsoleCommands);
		if (buttonsPerPreset.TryGetValue(Preset.Classic, out var value))
		{
			tooltip.DeregisterTooltip(value.GetComponent<RectTransform>());
			tooltip.RegisterTooltip(value.GetComponent<RectTransform>(), Localization.UI.MATCHSETUP_Tooltip_Preset_Classic);
		}
		if (buttonsPerPreset.TryGetValue(Preset.ProGolf, out var value2))
		{
			tooltip.DeregisterTooltip(value2.GetComponent<RectTransform>());
			tooltip.RegisterTooltip(value2.GetComponent<RectTransform>(), Localization.UI.MATCHSETUP_Tooltip_Preset_ProGolf);
		}
		if (buttonsPerPreset.TryGetValue(Preset.Custom, out var value3))
		{
			tooltip.DeregisterTooltip(value3.GetComponent<RectTransform>());
			tooltip.RegisterTooltip(value3.GetComponent<RectTransform>(), Localization.UI.MATCHSETUP_Tooltip_Preset_Custom);
		}
		for (int i = 0; i < itemTabs.Count; i++)
		{
			Button button = itemTabs[i];
			TMP_Text componentInChildren = button.GetComponentInChildren<TMP_Text>();
			RectTransform component = button.GetComponent<RectTransform>();
			tooltip.DeregisterTooltip(component);
			string text;
			if (string.Compare(componentInChildren.text, Localization.UI.MATCHSETUP_Title_AheadOwnBall) == 0)
			{
				text = Localization.UI.MATCHSETUP_Tooltip_AheadOwnBall;
			}
			else if (string.Compare(componentInChildren.text, Localization.UI.MATCHSETUP_Title_Ahead) == 0)
			{
				text = Localization.UI.MATCHSETUP_Tooltip_Ahead;
			}
			else if (string.Compare(componentInChildren.text, Localization.UI.MATCHSETUP_Title_MobilityItemBoxes) == 0)
			{
				text = Localization.UI.MATCHSETUP_Tooltip_MobilityItemBoxes;
			}
			else if (i >= 2)
			{
				string arg = string.Format(GameSettings.All.General.GetLocalizedDistanceUnitNameFull(), GameSettings.All.General.GetDistanceInCurrentUnits(itemSpawnerSettings.ItemPools[i - 1].minDistanceBehindLeader));
				text = string.Format(Localization.UI.MATCHSETUP_Tooltip_Behind, arg);
			}
			else
			{
				text = "";
			}
			if (text != "")
			{
				tooltip.RegisterTooltip(component, text);
			}
		}
	}

	private void DistanceUnitChanged()
	{
		UpdateTooltips();
		this.UpdateItemSpawnersLoc?.Invoke();
	}

	private void UpdateCurrentItemPool(int newIndex)
	{
		Button button = itemTabs[currentItemPoolIndex];
		Button button2 = itemTabs[newIndex];
		currentItemPoolIndex = newIndex;
		button.transform.GetChild(0).gameObject.SetActive(value: false);
		button2.transform.GetChild(0).gameObject.SetActive(value: true);
		bool flag = isPresetSuppressed;
		isPresetSuppressed = true;
		try
		{
			ItemPool itemPool = GetItemPool(newIndex);
			for (int i = 0; i < spawnChanceSliders.Count; i++)
			{
				SliderOption slider = spawnChanceSliders[i];
				int num = i;
				ItemType item = itemOrder[i];
				bool itemExistsInPool = Array.Exists(itemPool.SpawnChances, (ItemPool.ItemSpawnChance sc) => sc.item == item);
				slider.Initialize(delegate
				{
					slider.valueWithoutNotify = (itemExistsInPool ? SnapTo(slider.value, 0.025f) : 0f);
					slider.Slider.interactable = itemExistsInPool;
					float value = (itemExistsInPool ? (slider.value * 100f) : 0f);
					if (base.isServer)
					{
						spawnChanceWeights[ItemPoolId.Get(currentItemPoolIndex, item)] = value;
						SetPreset(Preset.Custom);
					}
				}, (!itemExistsInPool) ? 0f : ((num < spawnChanceWeights.Count) ? (spawnChanceWeights[ItemPoolId.Get(currentItemPoolIndex, item)] / 100f) : 0f));
				if (base.isServer)
				{
					ServerUpdateSpawnChanceValue(ItemPoolId.Get(currentItemPoolIndex, item));
				}
			}
		}
		finally
		{
			isPresetSuppressed = flag;
			currentItemPoolDirty = true;
		}
	}

	private void SpawnChanceUpdated(ItemPoolId itemPoolId, float val)
	{
		SpawnChanceUpdated(itemPoolId);
	}

	private void SpawnChanceUpdated(ItemPoolId itemPoolId)
	{
		if (base.isServer)
		{
			ServerUpdateSpawnChanceValue(itemPoolId);
		}
		if (currentItemPoolIndex == itemPoolId.itemPoolIndex)
		{
			int index = itemOrderLookup[(int)(itemPoolId.itemType - 1)];
			SliderOption sliderOption = spawnChanceSliders[index];
			float valueWithoutNotify = spawnChanceWeights[itemPoolId] / 100f;
			sliderOption.valueWithoutNotify = valueWithoutNotify;
			currentItemPoolDirty = true;
		}
		UpdateTotalWeightForPool(itemPoolId.itemPoolIndex);
		if (PauseMenu.IsPaused)
		{
			SingletonBehaviour<PauseMenu>.Instance.UpdateItemProbabilites();
		}
	}

	private void UpdateSliderGreyedOut(SliderOption slider)
	{
		Color color = slider.label.color;
		bool flag = slider.value > 0f;
		color.a = (flag ? 1f : 0.5f);
		slider.label.color = color;
		SetLabelGreyedOut(slider.gameObject, !flag);
	}

	private void UpdateDropdownGreyedOut(DropdownOption dropdown)
	{
		int value = dropdown.value;
		Color color = dropdown.captionText.color;
		color.a = ((value == 0) ? 1f : 0.5f);
		dropdown.captionText.color = color;
		SetLabelGreyedOut(dropdown.gameObject, value != 0);
	}

	private void UpdateDropdownGreyedOut(DropdownOption dropdown, int greyedOutOption)
	{
		int value = dropdown.value;
		Color color = dropdown.captionText.color;
		color.a = ((value == greyedOutOption) ? 0.5f : 1f);
		dropdown.captionText.color = color;
		SetLabelGreyedOut(dropdown.gameObject, value == greyedOutOption);
	}

	private void UpdateTotalWeightForPool(int poolIndex)
	{
		float num = 0f;
		ItemPool.ItemSpawnChance[] spawnChances = GetItemPool(poolIndex).SpawnChances;
		for (int i = 0; i < spawnChances.Length; i++)
		{
			ItemPool.ItemSpawnChance itemSpawnChance = spawnChances[i];
			num += spawnChanceWeights[ItemPoolId.Get(poolIndex, itemSpawnChance.item)];
		}
		totalWeightPerPool[poolIndex] = num;
	}

	[Server]
	private void ServerUpdateSpawnChanceValue(ItemPoolId itemPoolRef)
	{
		if (!NetworkServer.active)
		{
			Debug.LogWarning("[Server] function 'System.Void MatchSetupRules::ServerUpdateSpawnChanceValue(MatchSetupRules/ItemPoolId)' called when server was not active");
			return;
		}
		int itemPoolIndex = itemPoolRef.itemPoolIndex;
		switch (itemPoolIndex)
		{
		case 0:
		{
			ItemPool aheadOfBallItemPool = itemSpawnerSettings.AheadOfBallItemPool;
			SetSpawnChance(aheadOfBallItemPool);
			aheadOfBallItemPool.UpdateTotalWeight();
			break;
		}
		case 5:
		{
			ItemSpawnerSettings.ItemPoolData value2 = mobilityItemSpawnerSettings.ItemPools[0];
			SetSpawnChance(mobilityItemSpawnerSettings.ItemPools[0].pool);
			value2.pool.UpdateTotalWeight();
			mobilityItemSpawnerSettings.ItemPools[0] = value2;
			break;
		}
		default:
		{
			itemPoolIndex--;
			ItemSpawnerSettings.ItemPoolData value = itemSpawnerSettings.ItemPools[itemPoolIndex];
			SetSpawnChance(value.pool);
			value.pool.UpdateTotalWeight();
			itemSpawnerSettings.ItemPools[itemPoolIndex] = value;
			break;
		}
		}
		void SetSpawnChance(ItemPool pool)
		{
			for (int i = 0; i < pool.SpawnChances.Length; i++)
			{
				ItemPool.ItemSpawnChance itemSpawnChance = pool.SpawnChances[i];
				if (itemSpawnChance.item == itemPoolRef.itemType)
				{
					itemSpawnChance.spawnChanceWeight = spawnChanceWeights[itemPoolRef];
					pool.SpawnChances[i] = itemSpawnChance;
				}
			}
		}
	}

	public void SetPreset(Preset preset)
	{
		if (!isPresetSuppressed)
		{
			NetworkcurrentPreset = preset;
		}
	}

	private void OnPresetChanged(Preset previousPreset, Preset currentPreset)
	{
		foreach (var (preset2, button2) in buttonsPerPreset)
		{
			button2.transform.GetChild(0).gameObject.SetActive(preset2 == currentPreset);
		}
		Button value;
		LocalizedString stringReference = (buttonsPerPreset.TryGetValue(currentPreset, out value) ? value.GetComponentInChildren<LocalizeStringEvent>().StringReference : ((currentPreset >= Preset.Classic) ? buttonsPerPreset[Preset.Custom].GetComponentInChildren<LocalizeStringEvent>().StringReference : buttonsPerPreset[Preset.Classic].GetComponentInChildren<LocalizeStringEvent>().StringReference));
		rulesLabelLobby.StringReference = stringReference;
		rulesLabelLobby.RefreshString();
		if (base.isServer && currentPreset != Preset.Custom)
		{
			bool flag = isPresetSuppressed;
			isPresetSuppressed = true;
			try
			{
				ResetRules(isInitialized && currentPreset != Preset.ProGolf);
				if (currentPreset == Preset.ProGolf)
				{
					TrySetDropdownOnOff(Rule.HomingShots, value: false);
					TrySetDropdownOnOff(Rule.MaxTimeBasedOnPar, value: true);
					TrySetDropdownOnOff(Rule.HitOtherPlayers, value: true);
					List<ItemPoolId> value2;
					using (CollectionPool<List<ItemPoolId>, ItemPoolId>.Get(out value2))
					{
						value2.AddRange(spawnChanceWeights.Keys);
						foreach (ItemPoolId item in value2)
						{
							SetSpawnChance(item.itemPoolIndex, item.itemType, (item.itemType == ItemType.SpringBoots || item.itemType == ItemType.Coffee) ? 100 : 0);
						}
					}
				}
			}
			finally
			{
				isPresetSuppressed = flag;
			}
		}
		if (base.isServer)
		{
			BNetworkManager.singleton.ServerUpdateRules();
		}
		resetItemSpawnChancesButton.interactable = currentPreset == Preset.Custom && base.isServer;
		if (PauseMenu.IsPaused)
		{
			SingletonBehaviour<PauseMenu>.Instance.UpdateGameInfoLabels();
		}
		void TrySetDropdownOnOff(Rule rule, bool flag2)
		{
			if (onOffDropdownLookup.TryGetValue(rule, out var value3))
			{
				value3.SetValue((!flag2) ? 1 : 0);
			}
		}
	}

	private void ResetRules(bool resetSpawnChances)
	{
		rules.Clear();
		Rule key;
		foreach (KeyValuePair<Rule, SliderOption> item in sliderLookup)
		{
			item.Deconstruct(out key, out var value);
			Rule rule = key;
			value.SetValue(GetValue(rule));
		}
		DropdownOption value2;
		foreach (KeyValuePair<Rule, DropdownOption> item2 in onOffDropdownLookup)
		{
			item2.Deconstruct(out key, out value2);
			Rule rule2 = key;
			value2.SetValue((!GetValueAsBool(rule2)) ? 1 : 0);
		}
		foreach (KeyValuePair<Rule, DropdownOption> item3 in dropdownLookup)
		{
			item3.Deconstruct(out key, out value2);
			Rule rule3 = key;
			value2.SetValue((int)GetDefaultValue(rule3));
		}
		if (resetSpawnChances)
		{
			ResetSpawnChances();
		}
	}

	public void ResetSpawnChances()
	{
		itemSpawnerSettings.ResetRuntimeData();
		mobilityItemSpawnerSettings.ResetRuntimeData();
		ResetItemPool(itemSpawnerSettings.AheadOfBallItemPool, 0);
		ResetItemPool(mobilityItemSpawnerSettings.ItemPools[0].pool, 5);
		for (int i = 0; i < itemSpawnerSettings.ItemPools.Count; i++)
		{
			ResetItemPool(itemSpawnerSettings.ItemPools[i].pool, i + 1);
		}
		UpdateCurrentItemPool(currentItemPoolIndex);
		void ResetItemPool(ItemPool itemPool, int itemPoolIndex)
		{
			ItemPool.ItemSpawnChance[] spawnChances = itemPool.SpawnChances;
			for (int j = 0; j < spawnChances.Length; j++)
			{
				ItemPool.ItemSpawnChance itemSpawnChance = spawnChances[j];
				SetSpawnChance(itemPoolIndex, itemSpawnChance.item, itemSpawnChance.spawnChanceWeight);
			}
		}
	}

	public bool IsSpawnChangeDefault(int poolIndex, ItemType itemType)
	{
		ItemPoolId itemPoolId = ItemPoolId.Get(poolIndex, itemType);
		if (!spawnChanceWeights.ContainsKey(itemPoolId))
		{
			return true;
		}
		return Mathf.Approximately(spawnChanceWeights[itemPoolId], GetDefaultWeight(poolIndex, itemType));
	}

	private void SetSpawnChance(int itemPoolIndex, ItemType item, float spawnChance)
	{
		if (itemPoolIndex < 0 || itemPoolIndex >= itemSpawnerSettings.ItemPools.Count + 2)
		{
			Debug.LogWarning("Invalid item pool index!! " + itemPoolIndex);
		}
		else
		{
			spawnChanceWeights[ItemPoolId.Get(itemPoolIndex, item)] = spawnChance;
		}
	}

	private void OnWindChanged()
	{
		WindManager.Initialize(force: true);
	}

	private void OnLanguageChanged()
	{
		this.UpdateItemSpawnersLoc?.Invoke();
		UpdateTooltips();
	}

	public override void OnStartClient()
	{
		if (!base.isServer)
		{
			Initialize();
		}
	}

	public override void OnStopServer()
	{
		if (BNetworkManager.IsServerShuttingDown)
		{
			return;
		}
		foreach (KeyValuePair<Rule, float> rule in rules)
		{
			serverPersistentRules[rule.Key] = rule.Value;
		}
		foreach (KeyValuePair<ItemPoolId, float> spawnChanceWeight in spawnChanceWeights)
		{
			serverPersistentSpawnChanceWeights.Add(spawnChanceWeight.Key, spawnChanceWeight.Value);
		}
	}

	protected override void OnDestroy()
	{
		base.OnDestroy();
		itemLayoutInformer.SetLayoutHorizontalCalled -= OnItemLayoutSet;
		LocalizationManager.LanguageChanged -= OnLanguageChanged;
		GameSettings.GeneralSettings.DistanceUnitChanged = (Action)Delegate.Remove(GameSettings.GeneralSettings.DistanceUnitChanged, new Action(DistanceUnitChanged));
	}

	private void Update()
	{
		if (!MatchSetupMenu.IsActive || !currentItemPoolDirty)
		{
			return;
		}
		ItemPool currentItemPool = GetCurrentItemPool();
		float num = totalWeightPerPool[currentItemPoolIndex];
		foreach (SliderOption spawnChanceSlider in spawnChanceSliders)
		{
			if (!spawnChanceSlider.Slider.interactable)
			{
				spawnChanceSlider.SetValueText($"{0:0.#}%");
				UpdateSliderGreyedOut(spawnChanceSlider);
			}
		}
		ItemPool.ItemSpawnChance[] spawnChances = currentItemPool.SpawnChances;
		for (int i = 0; i < spawnChances.Length; i++)
		{
			ItemPool.ItemSpawnChance itemSpawnChance = spawnChances[i];
			SliderOption sliderOption = spawnChanceSliders[itemOrderLookup[(int)(itemSpawnChance.item - 1)]];
			float num2 = ((num > float.Epsilon) ? (spawnChanceWeights[ItemPoolId.Get(currentItemPoolIndex, itemSpawnChance.item)] / num) : 0f);
			sliderOption.SetValueText($"{num2 * 100f:0.#}%");
			UpdateSliderGreyedOut(sliderOption);
		}
		currentItemPoolDirty = false;
	}

	public float GetItemPoolTotalWeight(int index)
	{
		if (totalWeightPerPool.TryGetValue(index, out var value))
		{
			return value;
		}
		return 0f;
	}

	public float GetWeight(int poolIndex, ItemType itemType)
	{
		spawnChanceWeights.TryGetValue(ItemPoolId.Get(poolIndex, itemType), out var value);
		return value;
	}

	public float GetDefaultWeight(int poolIndex, ItemType itemType)
	{
		return GetDefaultItemPool(poolIndex).GetSpawnChanceWeight(itemType);
	}

	public float GetDefaultItemFactor(int poolIndex, ItemType itemType)
	{
		ItemPool defaultItemPool = GetDefaultItemPool(poolIndex);
		float spawnChanceWeight = defaultItemPool.GetSpawnChanceWeight(itemType);
		if (spawnChanceWeight > 0f && defaultItemPool.TotalSpawnChanceWeight > 0f)
		{
			return spawnChanceWeight / defaultItemPool.TotalSpawnChanceWeight;
		}
		return 0f;
	}

	private ItemPool GetItemPool(int index)
	{
		return index switch
		{
			0 => itemSpawnerSettings.AheadOfBallItemPool, 
			5 => mobilityItemSpawnerSettings.ItemPools[0].pool, 
			_ => itemSpawnerSettings.ItemPools[index - 1].pool, 
		};
	}

	private ItemPool GetDefaultItemPool(int index)
	{
		return index switch
		{
			0 => itemSpawnerSettings.AheadOfBallItemPoolDefaults, 
			5 => mobilityItemSpawnerSettings.ItemPoolsDefaults[0].pool, 
			_ => itemSpawnerSettings.ItemPoolsDefaults[index - 1].pool, 
		};
	}

	private ItemPool GetCurrentItemPool()
	{
		return GetItemPool(currentItemPoolIndex);
	}

	private float SnapTo(float value, float interval)
	{
		return BMath.Round(value / interval) * interval;
	}

	private float OutOfBoundsSnapping(float value)
	{
		if (SnapTo(value, 3f) == 3f)
		{
			return 3f;
		}
		return SnapTo(value, 5f);
	}

	public void Serialize(MatchSetupMenu.ServerValues serverValues)
	{
		serverValues.ruleKeys.AddRange(rules.Keys);
		serverValues.ruleValues.AddRange(rules.Values);
		serverValues.spawnChanceKeys.AddRange(spawnChanceWeights.Keys);
		serverValues.spawnChanceValues.AddRange(spawnChanceWeights.Values);
		serverValues.rulePreset = currentPreset;
	}

	public void Deserialize(MatchSetupMenu.ServerValues serverValues)
	{
		List<Rule> ruleKeys = serverValues.ruleKeys;
		List<float> ruleValues = serverValues.ruleValues;
		rules.Clear();
		spawnChanceWeights.Clear();
		NetworkcurrentPreset = serverValues.rulePreset;
		if (currentPreset != Preset.Custom)
		{
			return;
		}
		if (ruleKeys != null && ruleValues != null)
		{
			for (int i = 0; i < ruleKeys.Count; i++)
			{
				rules.Add(ruleKeys[i], ruleValues[i]);
			}
		}
		List<ItemPoolId> spawnChanceKeys = serverValues.spawnChanceKeys;
		List<float> spawnChanceValues = serverValues.spawnChanceValues;
		if (spawnChanceKeys != null && spawnChanceValues != null)
		{
			for (int j = 0; j < spawnChanceKeys.Count; j++)
			{
				int itemPoolIndex = spawnChanceKeys[j].itemPoolIndex;
				ItemType itemType = spawnChanceKeys[j].itemType;
				SetSpawnChance(itemPoolIndex, itemType, spawnChanceValues[j]);
			}
		}
		foreach (ItemPoolId key in spawnChanceWeights.Keys)
		{
			ServerUpdateSpawnChanceValue(key);
		}
	}

	public void SetLabelGreyedOut(GameObject child, bool greyed)
	{
		TMP_Text componentInChildren = child.transform.GetComponentInChildren<TMP_Text>();
		Color color = componentInChildren.color;
		color.a = (greyed ? 0.5f : 1f);
		componentInChildren.color = color;
	}

	private void OnRulesChanged(SyncIDictionary<Rule, float>.Operation operation, Rule rule, float value)
	{
		if ((uint)operation == 0u || (uint)operation == 1u)
		{
			if (!base.isServer)
			{
				SliderOption value3;
				DropdownOption value4;
				if (onOffDropdownLookup.TryGetValue(rule, out var value2))
				{
					value2.SetValue((!GetValueAsBoolInternal(rule)) ? 1 : 0);
				}
				else if (sliderLookup.TryGetValue(rule, out value3))
				{
					value3.SetValue(GetValueInternal(rule));
				}
				else if (dropdownLookup.TryGetValue(rule, out value4))
				{
					value4.SetValue((int)GetValueInternal(rule));
				}
			}
			UpdateRule(rule);
		}
		if (rule == Rule.ConsoleCommands)
		{
			CheckAndShowCheatsWarning();
		}
		if (rule == Rule.Wind)
		{
			OnWindChanged();
		}
		if (PauseMenu.IsPaused)
		{
			SingletonBehaviour<PauseMenu>.Instance.UpdateRules();
		}
		MatchSetupRules.RulesChanged?.Invoke();
	}

	private void CheckAndShowCheatsWarning()
	{
		bool valueAsBool = GetValueAsBool(Rule.ConsoleCommands);
		if (valueAsBool && !CheatsWarningShowed)
		{
			TextChatUi.ShowMessage(string.Format(Localization.UI.TEXTCHAT_Info_CheatsEnabled, GameManager.UiSettings.TextRedHighlightStartTag, GameManager.UiSettings.TextColorEndTag));
			Debug.Log("CHEATS ARE ENABLED");
			CheatsWarningShowed = true;
		}
		else if (!valueAsBool && CheatsWarningShowed)
		{
			TextChatUi.ShowMessage(string.Format(Localization.UI.TEXTCHAT_Info_CheatsDisabled, GameManager.UiSettings.TextHighlightStartTag, GameManager.UiSettings.TextColorEndTag));
			DevConsole.ResetCVars();
			Debug.Log("CHEATS ARE DISABLED");
			CheatsWarningShowed = false;
		}
	}

	private void UpdateRule(Rule rule)
	{
		switch (rule)
		{
		case Rule.ConsoleCommands:
			GameManager.UpdateConsoleEnabled();
			break;
		case Rule.HitOtherPlayers:
			GameManager.LayerSettings.SetPlayerCollisions(GetValueAsBoolInternal(Rule.HitOtherPlayers));
			if (GameManager.LocalPlayerMovement != null)
			{
				GameManager.LocalPlayerMovement.UpdateEnabledColliders();
			}
			{
				foreach (PlayerInfo remotePlayer in GameManager.RemotePlayers)
				{
					remotePlayer.Movement.UpdateEnabledColliders();
				}
				break;
			}
		}
	}

	private void InitSlider(SliderOption sliderOption, Rule rule, string format, Func<float, float> snapping, float valueMultiplier = 1f)
	{
		sliderOption.Initialize(delegate
		{
			if (snapping != null)
			{
				sliderOption.valueWithoutNotify = snapping(sliderOption.value);
			}
			sliderOption.SetValueText(string.Format(format, BMath.RoundToInt(sliderOption.value * valueMultiplier).ToString()));
			SetValue(rule, sliderOption.value);
			if (base.isServer)
			{
				SetPreset(Preset.Custom);
			}
			UpdateSliderGreyedOut(sliderOption);
		}, GetValueInternal(rule));
		sliderLookup[rule] = sliderOption;
	}

	private void InitDropdownOnOff(DropdownOption dropdownOption, Rule rule, Action AfterChanged = null)
	{
		dropdownOption.Initialize(delegate
		{
			SetValue(rule, dropdownOption.value == 0);
			if (base.isServer)
			{
				SetPreset(Preset.Custom);
			}
			UpdateDropdownGreyedOut(dropdownOption);
			AfterChanged?.Invoke();
		}, (!GetValueAsBoolInternal(rule)) ? 1 : 0);
		onOffDropdownLookup[rule] = dropdownOption;
	}

	private void InitDropdown(DropdownOption dropdownOption, Rule rule, Action AfterChanged = null)
	{
		dropdownOption.Initialize(delegate
		{
			SetValue(rule, dropdownOption.value);
			if (base.isServer)
			{
				SetPreset(Preset.Custom);
			}
			UpdateDropdownGreyedOut(dropdownOption, 0);
			AfterChanged?.Invoke();
		}, (int)GetValueInternal(rule));
		dropdownLookup[rule] = dropdownOption;
	}

	public string GetFormattedValue(Rule rule)
	{
		if (onOffDropdownLookup.TryGetValue(rule, out var value))
		{
			return value.Localized.GetLocalizedOption().GetLocalizedString();
		}
		if (dropdownLookup.TryGetValue(rule, out var value2))
		{
			return value2.Localized.GetLocalizedOption().GetLocalizedString();
		}
		if (sliderLookup.TryGetValue(rule, out var value3))
		{
			return value3.label.text;
		}
		return string.Empty;
	}

	public bool IsDropdown(Rule rule)
	{
		return onOffDropdownLookup.ContainsKey(rule);
	}

	private void SetValue(Rule rule, float value)
	{
		if (base.isServer)
		{
			rules[rule] = value;
		}
	}

	private void SetValue(Rule rule, bool value)
	{
		if (base.isServer)
		{
			rules[rule] = (value ? 1 : 0);
		}
	}

	public static float GetValue(Rule rule)
	{
		if (!SingletonNetworkBehaviour<MatchSetupRules>.HasInstance)
		{
			return GetDefaultValue(rule);
		}
		return SingletonNetworkBehaviour<MatchSetupRules>.Instance.GetValueInternal(rule);
	}

	public static bool GetValueAsBool(Rule rule)
	{
		return GetValue(rule) > 0f;
	}

	public static bool IsDefaultValue(Rule rule)
	{
		return GetDefaultValue(rule) == GetValue(rule);
	}

	private static float GetDefaultValue(Rule rule)
	{
		return rule switch
		{
			Rule.Countdown => GameManager.MatchSettings.MatchEndCountdownDuration, 
			Rule.OutOfBounds => GameManager.GolfSettings.OutOfBoundsEliminationTime, 
			Rule.MaxTimeBasedOnPar => 0f, 
			Rule.PlayerSpeed => 1f, 
			Rule.CartSpeed => 1f, 
			Rule.SwingPower => 1f, 
			Rule.OverchargeSidespin => 1f, 
			Rule.HomingShots => 1f, 
			Rule.KnockoutSpeedBoost => 1f, 
			Rule.HitOtherPlayers => 1f, 
			Rule.HitOtherPlayersBalls => 0f, 
			Rule.ConsoleCommands => 0f, 
			Rule.OnOrBelowPar => 1f, 
			Rule.Speedrun => 1f, 
			Rule.ChipIn => 1f, 
			Rule.Knockouts => 0f, 
			Rule.Comeback => 1f, 
			Rule.RepeatRecoveryProtection => 1f, 
			Rule.DominationProtection => 1f, 
			Rule.WhiteFlag => 1f, 
			Rule.Wind => 1f, 
			_ => 0f, 
		};
	}

	private float GetValueInternal(Rule rule)
	{
		if (rules.TryGetValue(rule, out var value))
		{
			return value;
		}
		return GetDefaultValue(rule);
	}

	private bool GetValueAsBoolInternal(Rule rule)
	{
		return GetValueInternal(rule) > 0f;
	}

	private void OnItemLayoutSet()
	{
		itemLayoutInformer.SetLayoutHorizontalCalled -= OnItemLayoutSet;
		UpdateNavigationDelayed();
		async void UpdateNavigationDelayed()
		{
			await UniTask.Yield();
			if (!(this == null))
			{
				int index = 1;
				Transform[] array = itemTabParents;
				for (int i = 0; i < array.Length; i++)
				{
					Selectable[] componentsInChildren = array[i].GetComponentsInChildren<Selectable>(includeInactive: true);
					if (componentsInChildren.Length != 0)
					{
						GridNavigationGroup.Row row = GridNavigationGroup.Row.CreateBlankRow();
						Selectable[] array2 = componentsInChildren;
						foreach (Selectable selectable in array2)
						{
							row.elements.Add(new GridNavigationGroup.Element
							{
								selectable = selectable
							});
						}
						gridNavigationGroup.InsertRow(index++, row);
					}
				}
				gridNavigationGroup.InsertRowsFrom(index, itemGridLayout);
				gridNavigationGroup.UpdateNavigation();
			}
		}
	}

	public MatchSetupRules()
	{
		InitSyncObject(rules);
		InitSyncObject(spawnChanceWeights);
		_Mirror_SyncVarHookDelegate_currentPreset = OnPresetChanged;
	}

	public override bool Weaved()
	{
		return true;
	}

	public override void SerializeSyncVars(NetworkWriter writer, bool forceAll)
	{
		base.SerializeSyncVars(writer, forceAll);
		if (forceAll)
		{
			GeneratedNetworkCode._Write_MatchSetupRules_002FPreset(writer, currentPreset);
			return;
		}
		writer.WriteVarULong(syncVarDirtyBits);
		if ((syncVarDirtyBits & 1L) != 0L)
		{
			GeneratedNetworkCode._Write_MatchSetupRules_002FPreset(writer, currentPreset);
		}
	}

	public override void DeserializeSyncVars(NetworkReader reader, bool initialState)
	{
		base.DeserializeSyncVars(reader, initialState);
		if (initialState)
		{
			GeneratedSyncVarDeserialize(ref currentPreset, _Mirror_SyncVarHookDelegate_currentPreset, GeneratedNetworkCode._Read_MatchSetupRules_002FPreset(reader));
			return;
		}
		long num = (long)reader.ReadVarULong();
		if ((num & 1L) != 0L)
		{
			GeneratedSyncVarDeserialize(ref currentPreset, _Mirror_SyncVarHookDelegate_currentPreset, GeneratedNetworkCode._Read_MatchSetupRules_002FPreset(reader));
		}
	}
}
