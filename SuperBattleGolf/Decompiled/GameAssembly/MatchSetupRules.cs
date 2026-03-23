using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using Cysharp.Threading.Tasks;
using Mirror;
using TMPro;
using UnityEngine;
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
		OverChargeSideSpin,
		HomingShots,
		KnockoutSpeedBoost,
		HitOtherPlayers,
		HitOtherPlayersBalls,
		ConsoleCommands,
		OnOrBelowParBonus,
		SpeedrunBonus,
		ChipInBonus,
		KnockoutsBonus
	}

	public enum Preset
	{
		Classic,
		ProGolf,
		Custom
	}

	[SerializeField]
	private GameObject rulesTab;

	[SerializeField]
	private GridNavigationGroup gridNavigationGroup;

	[Header("Bonus Score")]
	public DropdownOption onOrBelowPar;

	public DropdownOption speedrun;

	public DropdownOption chipIn;

	public DropdownOption knockouts;

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

	[Header("Cheats")]
	public GameObject[] cheatsOptions;

	public DropdownOption consoleCommands;

	[Header("Items")]
	public ItemSpawnerSettings itemSpawnerSettings;

	public SetLayoutInformer itemLayoutInformer;

	public Transform[] itemTabParents;

	public RectTransform itemsParent;

	public GridLayoutGroup itemGridLayout;

	public Button itemTabPrefab;

	public GameObject itemPrefab;

	public Button[] presetCategories;

	public ItemType[] itemOrder;

	public int[] itemOrderLookup;

	public Button resetItemSpawnChancesButton;

	[Header("Labels")]
	public LocalizeStringEvent rulesLabelLobby;

	[SyncVar(hook = "OnPresetChanged")]
	private Preset currentPreset;

	private SyncDictionary<Rule, float> rules = new SyncDictionary<Rule, float>();

	private static Dictionary<Rule, float> serverPersistentRules = new Dictionary<Rule, float>();

	private SyncDictionary<ItemPoolId, float> spawnChanceWeights = new SyncDictionary<ItemPoolId, float>();

	private static Dictionary<ItemPoolId, float> serverPersistentSpawnChanceWeights = new Dictionary<ItemPoolId, float>();

	private bool currentItemPoolDirty;

	private int currentItemPoolIndex;

	private Dictionary<Rule, SliderOption> sliderLookup = new Dictionary<Rule, SliderOption>();

	private Dictionary<Rule, DropdownOption> onOffDropdownLookup = new Dictionary<Rule, DropdownOption>();

	private List<SliderOption> spawnChanceSliders = new List<SliderOption>();

	private List<Button> itemTabs = new List<Button>();

	private bool supressPreset;

	public static bool CheatsWarningShowed;

	public Action<Preset, Preset> _Mirror_SyncVarHookDelegate_currentPreset;

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
		}
		supressPreset = true;
		InitDropdownOnOff(onOrBelowPar, Rule.OnOrBelowParBonus);
		InitDropdownOnOff(speedrun, Rule.SpeedrunBonus);
		InitDropdownOnOff(chipIn, Rule.ChipInBonus);
		InitDropdownOnOff(knockouts, Rule.KnockoutsBonus);
		InitSlider(countdown, Rule.Countdown, "{0}s", (float x) => SnapTo(x, 5f));
		InitSlider(outOfBounds, Rule.OutOfBounds, "{0}s", OutOfBoundsSnapping);
		InitDropdownOnOff(maxTimeBasedOnPar, Rule.MaxTimeBasedOnPar);
		InitSlider(playerSpeed, Rule.PlayerSpeed, "{0}%", (float x) => SnapTo(x, 0.1f), 100f);
		InitSlider(cartSpeed, Rule.CartSpeed, "{0}%", (float x) => SnapTo(x, 0.1f), 100f);
		InitSlider(swingPower, Rule.SwingPower, "{0}%", (float x) => SnapTo(x, 0.1f), 100f);
		InitDropdownOnOff(overchargeSideSpin, Rule.OverChargeSideSpin);
		InitDropdownOnOff(homingShots, Rule.HomingShots);
		InitDropdownOnOff(knockoutSpeedBoost, Rule.KnockoutSpeedBoost);
		InitDropdownOnOff(hitOtherPlayers, Rule.HitOtherPlayers);
		InitDropdownOnOff(hitOtherPlayersBalls, Rule.HitOtherPlayersBalls);
		InitDropdownOnOff(consoleCommands, Rule.ConsoleCommands);
		UpdateRule(Rule.ConsoleCommands);
		UpdateRule(Rule.HitOtherPlayers);
		int num = 0;
		Queue<Transform> tabParentQueue = new Queue<Transform>();
		Transform[] array = itemTabParents;
		foreach (Transform transform in array)
		{
			transform.gameObject.SetActive(value: false);
			tabParentQueue.Enqueue(transform);
		}
		tabParentQueue.Peek().gameObject.SetActive(value: true);
		InstantiateSpawnChanceTab(delegate(TMP_Text label)
		{
			label.text = Localization.UI.MATCHSETUP_Title_AheadOwnBall;
		}, 0);
		RegisterItemPool(itemSpawnerSettings.AheadOfBallItemPool, 0);
		num++;
		foreach (ItemSpawnerSettings.ItemPoolData itemPool in itemSpawnerSettings.ItemPools)
		{
			ItemSpawnerSettings.ItemPoolData pool = itemPool;
			InstantiateSpawnChanceTab(UpdateLocString, num);
			RegisterItemPool(pool.pool, num);
			num++;
			void UpdateLocString(TMP_Text label)
			{
				label.text = ((pool.minDistanceBehindLeader > float.Epsilon) ? string.Format(Localization.UI.MATCHSETUP_Title_BehindLead, ">" + pool.minDistanceBehindLeader) : Localization.UI.MATCHSETUP_Title_Ahead);
			}
		}
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
		SyncDictionary<Rule, float> syncDictionary = rules;
		syncDictionary.OnChange = (Action<SyncIDictionary<Rule, float>.Operation, Rule, float>)Delegate.Combine(syncDictionary.OnChange, new Action<SyncIDictionary<Rule, float>.Operation, Rule, float>(RuleUpdated));
		SyncDictionary<ItemPoolId, float> syncDictionary2 = spawnChanceWeights;
		syncDictionary2.OnSet = (Action<ItemPoolId, float>)Delegate.Combine(syncDictionary2.OnSet, new Action<ItemPoolId, float>(SpawnChanceUpdated));
		SyncDictionary<ItemPoolId, float> syncDictionary3 = spawnChanceWeights;
		syncDictionary3.OnAdd = (Action<ItemPoolId>)Delegate.Combine(syncDictionary3.OnAdd, new Action<ItemPoolId>(SpawnChanceUpdated));
		LocalizationManager.LanguageChanged += OnLanguageChanged;
		supressPreset = false;
		OnPresetChanged(currentPreset, currentPreset);
		UpdateCurrentItemPool(currentItemPoolIndex);
		CheckAndShowCheatsWarning();
		rulesTab.SetActive(activeSelf);
		void InstantiateSpawnChanceTab(Action<TMP_Text> UpdateLoc, int poolIndex)
		{
			Transform transform2 = tabParentQueue.Peek();
			if (transform2.childCount >= 3)
			{
				tabParentQueue.Dequeue();
				transform2 = tabParentQueue.Peek();
				transform2.gameObject.SetActive(value: true);
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
		}
	}

	private void UpdateCurrentItemPool(int newIndex)
	{
		Button button = itemTabs[currentItemPoolIndex];
		Button button2 = itemTabs[newIndex];
		currentItemPoolIndex = newIndex;
		button.transform.GetChild(0).gameObject.SetActive(value: false);
		button2.transform.GetChild(0).gameObject.SetActive(value: true);
		supressPreset = true;
		try
		{
			for (int i = 0; i < spawnChanceSliders.Count; i++)
			{
				SliderOption slider = spawnChanceSliders[i];
				int num = i;
				ItemType item = itemOrder[i];
				slider.Initialize(delegate
				{
					slider.valueWithoutNotify = SnapTo(slider.value, 0.025f);
					float value = slider.value * 100f;
					if (base.isServer)
					{
						spawnChanceWeights[ItemPoolId.Get(currentItemPoolIndex, item)] = value;
						SetPreset(Preset.Custom);
					}
				}, (num < spawnChanceWeights.Count) ? (spawnChanceWeights[ItemPoolId.Get(currentItemPoolIndex, item)] / 100f) : 0f);
				if (base.isServer)
				{
					ServerUpdateSpawnChanceValue(ItemPoolId.Get(currentItemPoolIndex, item));
				}
			}
		}
		finally
		{
			supressPreset = false;
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
			spawnChanceSliders[index].valueWithoutNotify = spawnChanceWeights[itemPoolId] / 100f;
			currentItemPoolDirty = true;
		}
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
		if (itemPoolIndex == 0)
		{
			SetSpawnChance(itemSpawnerSettings.AheadOfBallItemPool);
			return;
		}
		itemPoolIndex--;
		ItemSpawnerSettings.ItemPoolData value = itemSpawnerSettings.ItemPools[itemPoolIndex];
		SetSpawnChance(value.pool);
		value.pool.UpdateTotalWeight();
		itemSpawnerSettings.ItemPools[itemPoolIndex] = value;
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

	public void SetPreset(int preset)
	{
		SetPreset((Preset)preset);
	}

	public void SetPreset(Preset preset)
	{
		if (!supressPreset)
		{
			NetworkcurrentPreset = preset;
		}
	}

	private void OnPresetChanged(Preset prev, Preset curr)
	{
		for (int i = 0; i < presetCategories.Length; i++)
		{
			presetCategories[i].transform.GetChild(0).gameObject.SetActive(i == (int)curr);
		}
		rulesLabelLobby.StringReference = presetCategories[BMath.Clamp((int)curr, 0, presetCategories.Length - 1)].GetComponentInChildren<LocalizeStringEvent>().StringReference;
		rulesLabelLobby.RefreshString();
		if (base.isServer && curr != Preset.Custom)
		{
			supressPreset = true;
			try
			{
				ResetRules(curr == Preset.Classic);
				if (curr == Preset.ProGolf)
				{
					TrySetDropdown(Rule.HomingShots, value: false);
					TrySetDropdown(Rule.MaxTimeBasedOnPar, value: true);
					TrySetDropdown(Rule.HitOtherPlayers, value: true);
					List<ItemPoolId> value;
					using (CollectionPool<List<ItemPoolId>, ItemPoolId>.Get(out value))
					{
						value.AddRange(spawnChanceWeights.Keys);
						foreach (ItemPoolId item in value)
						{
							SetSpawnChance(item.itemPoolIndex, item.itemType, (item.itemType == ItemType.SpringBoots || item.itemType == ItemType.Coffee) ? 100 : 0);
						}
					}
				}
			}
			finally
			{
				supressPreset = false;
			}
		}
		resetItemSpawnChancesButton.interactable = curr == Preset.Custom && base.isServer;
		void TrySetDropdown(Rule rule, bool flag)
		{
			if (onOffDropdownLookup.TryGetValue(rule, out var value2))
			{
				value2.SetValue((!flag) ? 1 : 0);
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
		foreach (KeyValuePair<Rule, DropdownOption> item2 in onOffDropdownLookup)
		{
			item2.Deconstruct(out key, out var value2);
			Rule rule2 = key;
			value2.SetValue((!GetValueAsBool(rule2)) ? 1 : 0);
		}
		if (resetSpawnChances)
		{
			ResetSpawnChances();
		}
	}

	public void ResetSpawnChances()
	{
		itemSpawnerSettings.ResetRuntimeData();
		ResetItemPool(itemSpawnerSettings.AheadOfBallItemPool, 0);
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

	private void SetSpawnChance(int itemPoolIndex, ItemType item, float spawnChance)
	{
		if (itemPoolIndex < 0 || itemPoolIndex >= itemSpawnerSettings.ItemPools.Count + 1)
		{
			Debug.LogWarning("Invalid item pool index!! " + itemPoolIndex);
		}
		else
		{
			spawnChanceWeights[ItemPoolId.Get(itemPoolIndex, item)] = spawnChance;
		}
	}

	private void OnLanguageChanged()
	{
		this.UpdateItemSpawnersLoc?.Invoke();
	}

	public void OnOpenMenu()
	{
		GameObject[] array = cheatsOptions;
		for (int i = 0; i < array.Length; i++)
		{
			array[i].SetActive(GameSettings.All.General.DevConsole);
		}
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
	}

	private void Update()
	{
		if (MatchSetupMenu.IsActive && currentItemPoolDirty)
		{
			ItemPool currentItemPool = GetCurrentItemPool();
			float num = 0f;
			ItemPool.ItemSpawnChance[] spawnChances = currentItemPool.SpawnChances;
			for (int i = 0; i < spawnChances.Length; i++)
			{
				ItemPool.ItemSpawnChance itemSpawnChance = spawnChances[i];
				num += spawnChanceWeights[ItemPoolId.Get(currentItemPoolIndex, itemSpawnChance.item)];
			}
			spawnChances = currentItemPool.SpawnChances;
			for (int i = 0; i < spawnChances.Length; i++)
			{
				ItemPool.ItemSpawnChance itemSpawnChance2 = spawnChances[i];
				SliderOption sliderOption = spawnChanceSliders[itemOrderLookup[(int)(itemSpawnChance2.item - 1)]];
				float num2 = ((num > float.Epsilon) ? (spawnChanceWeights[ItemPoolId.Get(currentItemPoolIndex, itemSpawnChance2.item)] / num) : 0f);
				sliderOption.SetValueText($"{num2 * 100f:0.#}%");
			}
			currentItemPoolDirty = false;
		}
	}

	private ItemPool GetItemPool(int index)
	{
		if (index != 0)
		{
			return itemSpawnerSettings.ItemPools[index - 1].pool;
		}
		return itemSpawnerSettings.AheadOfBallItemPool;
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

	private void RuleUpdated(SyncIDictionary<Rule, float>.Operation operation, Rule rule, float value)
	{
		if ((uint)operation == 0u || (uint)operation == 1u)
		{
			if (!base.isServer)
			{
				SliderOption value3;
				if (onOffDropdownLookup.TryGetValue(rule, out var value2))
				{
					value2.SetValue((!GetValueAsBoolInternal(rule)) ? 1 : 0);
				}
				else if (sliderLookup.TryGetValue(rule, out value3))
				{
					value3.SetValue(GetValueInternal(rule));
				}
			}
			UpdateRule(rule);
		}
		if (rule == Rule.ConsoleCommands)
		{
			CheckAndShowCheatsWarning();
		}
	}

	private void CheckAndShowCheatsWarning()
	{
		bool valueAsBool = GetValueAsBool(Rule.ConsoleCommands);
		if (valueAsBool && !CheatsWarningShowed)
		{
			TextChatUi.ShowMessage(string.Format(Localization.UI.TEXTCHAT_Info_CheatsEnabled, GameManager.UiSettings.TextRedHighlightStartTag, GameManager.UiSettings.TextColorEndTag));
			CheatsWarningShowed = true;
		}
		else if (!valueAsBool && CheatsWarningShowed)
		{
			TextChatUi.ShowMessage(string.Format(Localization.UI.TEXTCHAT_Info_CheatsDisabled, GameManager.UiSettings.TextHighlightStartTag, GameManager.UiSettings.TextColorEndTag));
			DevConsole.ResetCVars();
			CheatsWarningShowed = false;
		}
	}

	private void UpdateRule(Rule rule)
	{
		switch (rule)
		{
		case Rule.ConsoleCommands:
			GameSettings.All.General.UpdateDevConsoleEnabled();
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
			SetValue(rule, sliderOption.value);
			sliderOption.SetValueText(string.Format(format, BMath.RoundToInt(sliderOption.value * valueMultiplier).ToString()));
			if (base.isServer)
			{
				SetPreset(Preset.Custom);
			}
		}, GetValueInternal(rule));
		sliderLookup[rule] = sliderOption;
	}

	private void InitDropdownOnOff(DropdownOption dropdownOption, Rule rule)
	{
		dropdownOption.Initialize(delegate
		{
			SetValue(rule, dropdownOption.value == 0);
			if (base.isServer)
			{
				SetPreset(Preset.Custom);
			}
		}, (!GetValueAsBoolInternal(rule)) ? 1 : 0);
		onOffDropdownLookup[rule] = dropdownOption;
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
			Rule.OverChargeSideSpin => 1f, 
			Rule.HomingShots => 1f, 
			Rule.KnockoutSpeedBoost => 1f, 
			Rule.HitOtherPlayers => 1f, 
			Rule.HitOtherPlayersBalls => 0f, 
			Rule.ConsoleCommands => 0f, 
			Rule.OnOrBelowParBonus => 1f, 
			Rule.SpeedrunBonus => 1f, 
			Rule.ChipInBonus => 1f, 
			Rule.KnockoutsBonus => 0f, 
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
