using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Cysharp.Threading.Tasks;
using FMODUnity;
using Steamworks;
using TMPro;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.InputSystem;
using UnityEngine.Pool;
using UnityEngine.ResourceManagement.AsyncOperations;
using UnityEngine.ResourceManagement.ResourceLocations;
using UnityEngine.UI;

public class PlayerCustomizationMenu : SingletonBehaviour<PlayerCustomizationMenu>, IBUpdateCallback, IAnyBUpdateCallback
{
	public enum NavigationMode
	{
		Cosmetics,
		ColorVariations,
		SkinColors
	}

	private class MetadataCmp : IComparer<MetadataReference>
	{
		public int Compare(MetadataReference x, MetadataReference y)
		{
			bool flag = !CosmeticsUnlocksManager.IsDefaultUnlock(x.metadata.PersistentGuid);
			bool flag2 = !CosmeticsUnlocksManager.IsDefaultUnlock(y.metadata.PersistentGuid);
			if (flag != flag2)
			{
				return flag.CompareTo(flag2);
			}
			if (!flag && !flag2)
			{
				return CosmeticsUnlocksManager.GetDefaultUnlockOrder(x.metadata.PersistentGuid).CompareTo(CosmeticsUnlocksManager.GetDefaultUnlockOrder(y.metadata.PersistentGuid));
			}
			bool flag3 = CosmeticsUnlocksManager.OwnsCosmetic(x.metadata);
			bool flag4 = CosmeticsUnlocksManager.OwnsCosmetic(y.metadata);
			if (flag3 != flag4)
			{
				return flag4.CompareTo(flag3);
			}
			if (flag3 && flag4)
			{
				return CosmeticsUnlocksManager.GetUnlockOrder(x.metadata.PersistentGuid).CompareTo(CosmeticsUnlocksManager.GetUnlockOrder(y.metadata.PersistentGuid));
			}
			bool flag5 = x.metadata.IsHidden();
			bool flag6 = y.metadata.IsHidden();
			if (flag5 != flag6)
			{
				int num = (int)(flag5 ? x.metadata.requiredAchievement : ((AchievementId)(-1)));
				int value = (int)(flag6 ? y.metadata.requiredAchievement : ((AchievementId)(-1)));
				return num.CompareTo(value);
			}
			if (x.metadata.cost == y.metadata.cost)
			{
				if (x.metadata.unlockedByAchievment != y.metadata.unlockedByAchievment)
				{
					return y.metadata.unlockedByAchievment.CompareTo(x.metadata.unlockedByAchievment);
				}
				return x.metadata.name.CompareTo(y.metadata.name);
			}
			return x.metadata.cost.CompareTo(y.metadata.cost);
		}
	}

	private class MetadataReference : IDisposable
	{
		public PlayerCosmeticsMetadata metadata;

		public string metadataKey;

		public void Dispose()
		{
			Addressables.Release(metadata);
		}
	}

	[Serializable]
	private class PersistentData
	{
		[SerializeField]
		private List<string> acknowledgedGuids = new List<string>();

		public bool HasAcknowledgedCosmetic(string guid)
		{
			return acknowledgedGuids.Contains(guid);
		}

		public void AcknowledgeCosmetic(string guid)
		{
			if (!acknowledgedGuids.Contains(guid))
			{
				acknowledgedGuids.Add(guid);
			}
		}

		public bool IsValid()
		{
			return acknowledgedGuids != null;
		}
	}

	private const string PERSISTENTDATA_FILENAME = "CustomizationMenu.json";

	[Header("References")]
	public List<PlayerCustomizationCosmeticButton> cosmeticsButtons = new List<PlayerCustomizationCosmeticButton>();

	public List<Button> colorButtons = new List<Button>();

	public List<Button> skinButtons = new List<Button>();

	public GameObject menu;

	public PlayerCosmeticsSettings settings;

	public CharacterPreview characterPreview;

	public PlayerCustomizationCosmeticButton buttonTemplate;

	public GameObject colorTemplate;

	public GameObject skinColorTemplate;

	public TMP_Text unlocked;

	public Button[] tabs;

	public MenuNavigation navigation;

	public MenuNavigation colorVariationsNavigation;

	public MenuNavigation skinColorNavigation;

	public ButtonPromptExplicit colorVariationsPrompt;

	public ButtonPromptExplicit skinColorPrompt;

	public ScrollRect scrollRect;

	public CurrencyDisplay currencyDisplay;

	public UiTooltip tooltip;

	public CanvasGroup loadoutSelectGroup;

	public Toggle[] loadoutSelectToggles;

	[Header("Buy prompt")]
	public GameObject buyPrompt;

	public TMP_Text costLabel;

	public Button buyButton;

	public Color normalPriceColor;

	public Color tooExpensivePriceColor;

	[Header("Colors")]
	public Color tabIconNormalColor;

	public Color tabIconSelectedColor;

	public HorizontalLayoutGroup colorsGroup;

	[Header("Default Icons")]
	public Sprite defaultEyesIcon;

	public Sprite defaultBrowsIcon;

	public Sprite defaultCheeksIcon;

	public Sprite defaultMouthIcon;

	public Sprite defaultClubIcon;

	public Sprite defaultGolfBallIcon;

	public Sprite defaultNoneButtonBackground;

	public Sprite defaultButtonBackground;

	public Sprite notOwnedButtonBackground;

	public Sprite notUnlockedButtonBackground;

	[Header("Dances")]
	public PlayerCosmeticsVictoryDances allVictoryDances;

	private PlayerCosmeticsMetadata.Category currentCategory;

	private int lastCosmeticVariationIndex;

	private MetadataReference[] selectedCosmetics = new MetadataReference[13];

	private PlayerCosmeticsSwitcher.CosmeticKey[] selectedCosmeticKeys = new PlayerCosmeticsSwitcher.CosmeticKey[13];

	private VictoryDance selectedVictoryDance;

	private List<MetadataReference> allMetadata = new List<MetadataReference>();

	private MultiDictionary<PlayerCosmeticsMetadata.Category, MetadataReference, List<MetadataReference>> categories = new MultiDictionary<PlayerCosmeticsMetadata.Category, MetadataReference, List<MetadataReference>>();

	private PersistentData persistentData;

	private bool isEnabled;

	private bool wasLastEnabledFromInteraction;

	private bool hasColorVariations;

	private NavigationMode currentNavigationMode;

	private int selectedLoadout;

	public static bool IsActive
	{
		get
		{
			if (SingletonBehaviour<PlayerCustomizationMenu>.HasInstance)
			{
				return SingletonBehaviour<PlayerCustomizationMenu>.Instance.isEnabled;
			}
			return false;
		}
	}

	public static event Action OnOpened;

	public static event Action OnClosed;

	[CCommand("openPlayerCustomization", "", false, false)]
	private static void CommandOpenMenu()
	{
		if (!SingletonBehaviour<PlayerCustomizationMenu>.HasInstance)
		{
			Debug.LogError("This command can only be used in the driving range!");
		}
		else
		{
			SingletonBehaviour<PlayerCustomizationMenu>.Instance.SetEnabled(enabled: true, fromInteraction: false);
		}
	}

	[CCommand("stresstestCosmeticsSwitcher", "", false, false)]
	private static async void StressTest()
	{
		if (!SingletonBehaviour<PlayerCustomizationMenu>.HasInstance || !IsActive)
		{
			Debug.LogError("Stress test requires customization menu to be open");
			return;
		}
		SingletonBehaviour<PlayerCustomizationMenu>.Instance.categories.TryGetValues(SingletonBehaviour<PlayerCustomizationMenu>.Instance.currentCategory, out var values);
		Action<PlayerCosmeticsSwitcher.CosmeticKey, bool> setCosmetic = SingletonBehaviour<PlayerCustomizationMenu>.Instance.GetSetCosmeticCallback(SingletonBehaviour<PlayerCustomizationMenu>.Instance.currentCategory);
		List<MetadataReference> shuffled = new List<MetadataReference>();
		shuffled.AddRange(values);
		shuffled.Shuffle();
		for (int i = 0; i < shuffled.Count; i++)
		{
			Debug.Log("Loading " + shuffled[i].metadata.name);
			PlayerCosmeticsSwitcher.CosmeticKey obj = new PlayerCosmeticsSwitcher.CosmeticKey
			{
				metadataKey = shuffled[i].metadataKey
			};
			PlayerCosmeticsMetadata.Variation[] variations = shuffled[i].metadata.variations;
			obj.variationIndex = (sbyte)UnityEngine.Random.Range(0, (variations != null) ? variations.Length : 0);
			setCosmetic(obj, arg2: false);
			for (int j = 0; j < UnityEngine.Random.Range(0, 4); j++)
			{
				await UniTask.Yield();
			}
		}
	}

	private void Start()
	{
		if (!LoadPersistentData())
		{
			persistentData = new PersistentData();
		}
		LoadCategories().Forget();
		characterPreview.Initialize();
		characterPreview.Refresh();
		characterPreview.cosmeticsSwitcher.rightHand.SetEquipmentPreviewLocal(EquipmentType.GolfClub);
		buttonTemplate.gameObject.SetActive(value: false);
		colorTemplate.SetActive(value: false);
		skinColorTemplate.SetActive(value: false);
		InitSkinColors();
		currencyDisplay.gameObject.SetActive(value: false);
		menu.SetActive(value: false);
		navigation.OnExitEvent += OnMenuExit;
		colorVariationsNavigation.OnExitEvent += delegate
		{
			SetNavigationMode(NavigationMode.Cosmetics);
		};
		skinColorNavigation.OnExitEvent += delegate
		{
			SetNavigationMode(NavigationMode.Cosmetics);
		};
		OnInputDeviceChange();
		InputManager.SwitchedInputDeviceType += OnInputDeviceChange;
	}

	protected override void OnDestroy()
	{
		base.OnDestroy();
		foreach (MetadataReference allMetadatum in allMetadata)
		{
			allMetadatum.Dispose();
		}
		if (isEnabled)
		{
			CleanUpCallbacks();
		}
		InputManager.SwitchedInputDeviceType -= OnInputDeviceChange;
	}

	private void OnInputDeviceChange()
	{
		colorsGroup.padding.left = (InputManager.UsingGamepad ? 40 : 0);
		colorsGroup.spacing = (InputManager.UsingGamepad ? 8 : 12);
	}

	public void OnBUpdate()
	{
		PlayerCosmeticObject.ModelSlot modelSlot = CategoryToSlot(currentCategory);
		if (modelSlot != PlayerCosmeticObject.ModelSlot.None && categories.TryGetValues(currentCategory, out var values))
		{
			int num = -1;
			if (InputManager.UsingKeyboard && RectTransformUtility.RectangleContainsScreenPoint(scrollRect.content, Mouse.current.position.value))
			{
				Vector2 value = Mouse.current.position.value;
				for (int i = 0; i < values.Count; i++)
				{
					if (RectTransformUtility.RectangleContainsScreenPoint(cosmeticsButtons[i].select.image.rectTransform, value))
					{
						num = i - 1;
						break;
					}
				}
			}
			else if (InputManager.UsingGamepad)
			{
				for (int j = 0; j < values.Count; j++)
				{
					if (cosmeticsButtons[j].GetComponentInChildren<ControllerSelectable>().IsSelected)
					{
						num = j - 1;
						break;
					}
				}
			}
			PlayerCosmeticObject.ModelSlot modelSlot2 = PlayerCosmeticObject.ModelSlot.None;
			if (num >= 0)
			{
				MetadataReference metadataReference = values[num];
				if (!metadataReference.metadata.IsHidden())
				{
					modelSlot2 = characterPreview.cosmeticsSwitcher.GetSlotsWillBeUnequipped(metadataReference.metadata.allowedCosmetics, modelSlot);
					if (modelSlot == PlayerCosmeticObject.ModelSlot.Head)
					{
						PlayerCosmeticsSwitcher.CosmeticKey currentHeadRuntimeKey = characterPreview.cosmeticsSwitcher.CurrentHeadRuntimeKey;
						if (currentHeadRuntimeKey != null && currentHeadRuntimeKey.HasValidKey())
						{
							modelSlot2 |= PlayerCosmeticObject.ModelSlot.Head;
						}
					}
				}
			}
			UpdateCategoryUnequipWarning(modelSlot2, Time.deltaTime * 16f);
		}
		if (InputManager.CurrentGamepad == null || InputManager.CurrentModeMask.HasMode(InputMode.ForceDisabled))
		{
			return;
		}
		if (InputManager.CurrentGamepad.rightShoulder.wasPressedThisFrame)
		{
			CycleCategoryRight();
		}
		if (InputManager.CurrentGamepad.leftShoulder.wasPressedThisFrame)
		{
			CycleCategoryLeft();
		}
		if (InputManager.CurrentGamepad.leftTrigger.wasPressedThisFrame && loadoutSelectGroup.interactable)
		{
			Toggle obj = loadoutSelectToggles[(selectedLoadout + 1) % loadoutSelectToggles.Length];
			obj.isOn = true;
			if (obj.TryGetComponent<UiSfx>(out var component))
			{
				component.PlaySelectSfx(InputManager.UsingGamepad);
			}
		}
		if (!skinColorNavigation.enabled && !colorVariationsNavigation.enabled)
		{
			if (InputManager.CurrentGamepad.xButton.wasPressedThisFrame)
			{
				SetNavigationMode(NavigationMode.SkinColors);
			}
			else if (hasColorVariations && InputManager.CurrentGamepad.yButton.wasPressedThisFrame)
			{
				SetNavigationMode(NavigationMode.ColorVariations);
			}
		}
	}

	private void UpdateCategoryUnequipWarning(PlayerCosmeticObject.ModelSlot unequip, float delta)
	{
		for (int i = 0; i < tabs.Length; i++)
		{
			Image component = tabs[i].transform.GetChild(3).GetComponent<Image>();
			PlayerCosmeticsMetadata.Category category = (PlayerCosmeticsMetadata.Category)i;
			PlayerCosmeticObject.ModelSlot modelSlot = CategoryToSlot(category);
			if (category == currentCategory || modelSlot == PlayerCosmeticObject.ModelSlot.None)
			{
				SetAlpha(component, 0f);
				continue;
			}
			if (modelSlot == PlayerCosmeticObject.ModelSlot.Head && unequip.HasFlag(modelSlot))
			{
				PlayerCosmeticsSwitcher.CosmeticKey runtimeKey = characterPreview.cosmeticsSwitcher.CurrentHeadRuntimeKey;
				if (runtimeKey != null && !string.IsNullOrEmpty(runtimeKey.metadataKey))
				{
					MetadataReference metadataReference = allMetadata.FirstOrDefault((MetadataReference x) => x.metadataKey == runtimeKey.metadataKey);
					SetAlpha(component, (metadataReference != null && metadataReference.metadata.category == category) ? 1 : 0);
					continue;
				}
			}
			SetAlpha(component, unequip.HasFlag(modelSlot) ? 1 : 0);
		}
		void SetAlpha(Image image, float targetAlpha)
		{
			Color color = image.color;
			color.a = BMath.Lerp(color.a, targetAlpha, delta);
			image.color = color;
		}
	}

	private PlayerCosmeticObject.ModelSlot CategoryToSlot(PlayerCosmeticsMetadata.Category category)
	{
		return category switch
		{
			PlayerCosmeticsMetadata.Category.Head => PlayerCosmeticObject.ModelSlot.Head, 
			PlayerCosmeticsMetadata.Category.Hair => PlayerCosmeticObject.ModelSlot.Head, 
			PlayerCosmeticsMetadata.Category.Hat => PlayerCosmeticObject.ModelSlot.Hat, 
			PlayerCosmeticsMetadata.Category.Face => PlayerCosmeticObject.ModelSlot.Face, 
			PlayerCosmeticsMetadata.Category.FaceLower => PlayerCosmeticObject.ModelSlot.FaceLower, 
			_ => PlayerCosmeticObject.ModelSlot.None, 
		};
	}

	public void Close()
	{
		SetEnabled(enabled: false, fromInteraction: false);
	}

	public async void SetLoadout(int index, bool resetCategory, bool setPrevToPlayer)
	{
		PlayerCosmetics.Loadout loadout = GameManager.LocalPlayerInfo.Cosmetics.GetLoadout(index);
		_ = selectedLoadout;
		if (setPrevToPlayer)
		{
			GameManager.LocalPlayerInfo.Cosmetics.SetLoadout(characterPreview.cosmeticsSwitcher, selectedVictoryDance, selectedLoadout, equip: false, save: false).Forget();
		}
		characterPreview.Refresh();
		characterPreview.SetPreviewEnabled(enabled: false);
		selectedVictoryDance = loadout.GetVictoryDance();
		loadoutSelectGroup.interactable = false;
		selectedLoadout = index;
		try
		{
			List<UniTask> value;
			using (CollectionPool<List<UniTask>, UniTask>.Get(out value))
			{
				value.Add(SetHeadModel(loadout.head, isPlayerSelection: false));
				value.Add(SetHatModel(loadout.hat, isPlayerSelection: false));
				value.Add(SetFaceModel(loadout.upperFace, isPlayerSelection: false));
				value.Add(SetLowerFaceModel(loadout.lowerFace, isPlayerSelection: false));
				value.Add(SetClubModel(loadout.club, isPlayerSelection: false));
				value.Add(SetBodyModel(loadout.body, isPlayerSelection: false));
				value.Add(SetMouthTexture(loadout.mouth, isPlayerSelection: false));
				value.Add(SetEyesTexture(loadout.eyes, isPlayerSelection: false));
				value.Add(SetCheeksTexture(loadout.cheeks, isPlayerSelection: false));
				value.Add(SetBrowsTexture(loadout.brows, isPlayerSelection: false));
				value.Add(SetGolfBallModel(loadout.golfBall, isPlayerSelection: false));
				SetSkinColorIndex(loadout.skinColorIndex, isPlayerSelection: false);
				await UniTask.WhenAll(value);
				SetCategory((int)((!resetCategory) ? currentCategory : PlayerCosmeticsMetadata.Category.Head));
			}
		}
		finally
		{
			loadoutSelectGroup.interactable = true;
			characterPreview.SetPreviewEnabled(enabled: true);
		}
	}

	private void UpdateLoadoutToggles()
	{
		for (int i = 0; i < loadoutSelectToggles.Length; i++)
		{
			loadoutSelectToggles[i].onValueChanged.RemoveAllListeners();
		}
		for (int j = 0; j < loadoutSelectToggles.Length; j++)
		{
			Toggle obj = loadoutSelectToggles[j];
			int loadout = j;
			obj.isOn = j == selectedLoadout;
			obj.onValueChanged.AddListener(delegate(bool isOn)
			{
				if (isOn)
				{
					SetLoadout(loadout, resetCategory: false, setPrevToPlayer: true);
				}
			});
		}
	}

	private bool LoadPersistentData()
	{
		try
		{
			string text = null;
			if (!SteamRemoteStorage.FileExists("CustomizationMenu.json"))
			{
				return false;
			}
			byte[] bytes = SteamRemoteStorage.FileRead("CustomizationMenu.json");
			text = Encoding.UTF8.GetString(bytes);
			persistentData = JsonUtility.FromJson<PersistentData>(text);
			return persistentData.IsValid();
		}
		catch (Exception exception)
		{
			Debug.LogError("Caught exception while loading PersistentData!");
			Debug.LogException(exception);
			return false;
		}
	}

	private void SavePersistentData()
	{
		try
		{
			string s = JsonUtility.ToJson(persistentData, prettyPrint: true);
			byte[] bytes = Encoding.UTF8.GetBytes(s);
			SteamRemoteStorage.FileWrite("CustomizationMenu.json", bytes);
		}
		catch (Exception exception)
		{
			Debug.LogError("Caught exception while saving PersistentData!");
			Debug.LogException(exception);
		}
	}

	public void SetEnabled(bool enabled, bool fromInteraction)
	{
		if (enabled)
		{
			wasLastEnabledFromInteraction = fromInteraction;
		}
		if (enabled == isEnabled)
		{
			return;
		}
		if (enabled)
		{
			if (!LoadPersistentData())
			{
				persistentData = new PersistentData();
			}
			SetEnabled(enabled: true);
			InputManager.EnableMode(InputMode.Paused);
			InputManager.SwitchedInputDeviceType += OnSwitchedInputDevice;
			SetLoadout(GameManager.LocalPlayerInfo.Cosmetics.GetEquippedLoadoutIndex(), resetCategory: true, setPrevToPlayer: false);
			UpdateLoadoutToggles();
			RegisterBUpdateDelayed();
			for (int i = 0; i < tabs.Length; i++)
			{
				tabs[i].transform.GetChild(2).gameObject.SetActive(value: false);
			}
			if (wasLastEnabledFromInteraction)
			{
				PlayerCustomizationBuilding.InformLocalPlayerEntered();
			}
		}
		else
		{
			int num = 0;
			PlayerCosmeticsVictoryDanceMetadata dance = allVictoryDances.GetDance(selectedVictoryDance);
			if (!CosmeticsUnlocksManager.OwnsDance(dance))
			{
				num += dance.cost;
			}
			for (int j = 0; j < 13; j++)
			{
				if (GetSetCosmeticCallback((PlayerCosmeticsMetadata.Category)j) != null)
				{
					MetadataReference metadataReference = selectedCosmetics[j];
					if (metadataReference != null && !CosmeticsUnlocksManager.OwnsCosmetic(metadataReference?.metadata))
					{
						num += metadataReference.metadata.cost;
					}
				}
			}
			if (num > 0)
			{
				if (CanAfford(num))
				{
					FullScreenMessage.Show(string.Format(Localization.UI.CUSTOMIZE_Prompt_BuyEquipped, num.ToString()), new FullScreenMessage.ButtonEntry(Localization.UI.MISC_Yes, BuyAndApplyCosmetics), new FullScreenMessage.ButtonEntry(Localization.UI.MISC_No, ApplyCosmetics), new FullScreenMessage.ButtonEntry(Localization.UI.MISC_Cancel, FullScreenMessage.Hide));
				}
				else
				{
					FullScreenMessage.Show(Localization.UI.CUSTOMIZE_Prompt_NotEnoughCredits, new FullScreenMessage.ButtonEntry(Localization.UI.MISC_Yes, LeaveShop), new FullScreenMessage.ButtonEntry(Localization.UI.MISC_No, FullScreenMessage.Hide));
				}
			}
			else
			{
				ApplyCosmetics();
			}
			SavePersistentData();
			if (wasLastEnabledFromInteraction)
			{
				PlayerCustomizationBuilding.InformLocalPlayerExited();
			}
		}
		buyPrompt.SetActive(value: false);
		void ApplyCosmetics()
		{
			CleanUpCallbacks();
			GameManager.LocalPlayerInfo.Cosmetics.SetLoadout(characterPreview.cosmeticsSwitcher, selectedVictoryDance, selectedLoadout, equip: true, save: true).Forget();
			SetEnabled(enabled: false);
			FullScreenMessage.Hide();
		}
		void BuyAndApplyCosmetics()
		{
			VictoryDance dance2 = selectedVictoryDance;
			PlayerCosmeticsVictoryDanceMetadata dance3 = allVictoryDances.GetDance(dance2);
			if (!CosmeticsUnlocksManager.OwnsDance(dance3))
			{
				CosmeticsUnlocksManager.PurchaseDance(dance3);
			}
			for (int k = 0; k < 13; k++)
			{
				if (GetSetCosmeticCallback((PlayerCosmeticsMetadata.Category)k) != null)
				{
					MetadataReference metadataReference2 = selectedCosmetics[k];
					if (metadataReference2 != null && !CosmeticsUnlocksManager.OwnsCosmetic(metadataReference2?.metadata))
					{
						CosmeticsUnlocksManager.PurchaseCosmetic(metadataReference2.metadata);
					}
				}
			}
			RuntimeManager.PlayOneShot(GameManager.AudioSettings.CosmeticsPurchase);
			ApplyCosmetics();
		}
		void LeaveShop()
		{
			CleanUpCallbacks();
			SetEnabled(enabled: false);
			FullScreenMessage.Hide();
		}
		void SetEnabled(bool flag)
		{
			menu.SetActive(flag);
			currencyDisplay.gameObject.SetActive(flag);
			isEnabled = flag;
			if (flag)
			{
				PlayerCustomizationMenu.OnOpened?.Invoke();
				RuntimeManager.PlayOneShot(GameManager.AudioSettings.CosmeticsOpen);
			}
			else
			{
				PlayerCustomizationMenu.OnClosed?.Invoke();
				RuntimeManager.PlayOneShot(GameManager.AudioSettings.CosmeticsOpen);
			}
		}
	}

	private void CleanUpCallbacks()
	{
		InputManager.DisableMode(InputMode.Paused);
		InputManager.SwitchedInputDeviceType -= OnSwitchedInputDevice;
		BUpdate.DeregisterCallback(this);
	}

	private async void RegisterBUpdateDelayed()
	{
		await UniTask.Yield();
		await UniTask.Yield();
		if (IsActive)
		{
			BUpdate.RegisterCallback(this);
		}
	}

	private void SetNavigationMode(NavigationMode mode)
	{
		colorVariationsNavigation.enabled = hasColorVariations && mode == NavigationMode.ColorVariations;
		skinColorNavigation.enabled = mode == NavigationMode.SkinColors;
		currentNavigationMode = mode;
		if (mode == NavigationMode.Cosmetics)
		{
			navigation.enabled = true;
		}
		UpdateNavigationPrompts();
	}

	private void UpdateNavigationPrompts()
	{
		skinColorPrompt.gameObject.SetActive(currentNavigationMode == NavigationMode.Cosmetics);
		colorVariationsPrompt.gameObject.SetActive(hasColorVariations && currentNavigationMode == NavigationMode.Cosmetics);
	}

	private void OnSwitchedInputDevice()
	{
		if (!InputManager.UsingGamepad)
		{
			SetNavigationMode(NavigationMode.Cosmetics);
		}
	}

	public void OnMenuExit()
	{
		SetEnabled(enabled: false, fromInteraction: false);
	}

	private void CycleCategoryLeft()
	{
		CycleCategory(-1);
	}

	private void CycleCategoryRight()
	{
		CycleCategory(1);
	}

	private void CycleCategory(int offset)
	{
		Button obj = tabs[(int)currentCategory];
		int num = obj.transform.GetSiblingIndex() - 1;
		int num2 = WrapCategory(num + offset);
		Button component = obj.transform.parent.GetChild(num2 + 1).GetComponent<Button>();
		int category = Array.IndexOf(tabs, component);
		if (component.TryGetComponent<UiSfx>(out var component2))
		{
			component2.PlaySelectSfx(InputManager.UsingGamepad);
		}
		SetCategory(category);
	}

	private int WrapCategory(int category)
	{
		if (category < 0)
		{
			category = 12;
		}
		else if (category >= 13)
		{
			category = 0;
		}
		return category;
	}

	public void SetCategory(int category)
	{
		PlayerCosmeticsMetadata.Category category2 = currentCategory;
		currentCategory = (PlayerCosmeticsMetadata.Category)category;
		lastCosmeticVariationIndex = 0;
		if (currentCategory != PlayerCosmeticsMetadata.Category.VictoryDance)
		{
			UpdateCosmeticsButtons();
			if (category2 == PlayerCosmeticsMetadata.Category.VictoryDance)
			{
				SetPreviewAnimation(VictoryDance.None);
			}
			characterPreview.cosmeticsSwitcher.rightHand.SetEquipmentPreviewLocal(EquipmentType.GolfClub);
		}
		else
		{
			UpdateDanceButtons();
		}
		for (int i = 0; i < tabs.Length; i++)
		{
			Button button = tabs[i];
			if (!(button == null))
			{
				SetTabSelected(button.gameObject, i == category);
			}
		}
		SetNavigationMode(NavigationMode.Cosmetics);
		UpdateCategoryUnequipWarning(PlayerCosmeticObject.ModelSlot.None, 1f);
	}

	private Sprite GetDefaultIcon(PlayerCosmeticsMetadata.Category category)
	{
		return category switch
		{
			PlayerCosmeticsMetadata.Category.Brows => defaultBrowsIcon, 
			PlayerCosmeticsMetadata.Category.Cheeks => defaultCheeksIcon, 
			PlayerCosmeticsMetadata.Category.Mouth => defaultMouthIcon, 
			PlayerCosmeticsMetadata.Category.Eyes => defaultEyesIcon, 
			PlayerCosmeticsMetadata.Category.Club => defaultClubIcon, 
			PlayerCosmeticsMetadata.Category.Golfball => defaultGolfBallIcon, 
			_ => null, 
		};
	}

	private async UniTaskVoid LoadCategories()
	{
		AsyncOperationHandle<IList<IResourceLocation>> handle = Addressables.LoadResourceLocationsAsync("cosmetics-metadata");
		await handle;
		List<UniTask> value;
		using (CollectionPool<List<UniTask>, UniTask>.Get(out value))
		{
			foreach (IResourceLocation item in handle.Result)
			{
				value.Add(LoadMetadata(item.PrimaryKey));
			}
			await UniTask.WhenAll(value);
			UpdateCosmeticsButtons();
			Addressables.Release(handle);
			SetCategory(0);
		}
		async UniTask LoadMetadata(string key)
		{
			AsyncOperationHandle<PlayerCosmeticsMetadata> metadataHandle = Addressables.LoadAssetAsync<PlayerCosmeticsMetadata>(key);
			await metadataHandle;
			if (!(metadataHandle.Result == null))
			{
				MetadataReference metadataReference = new MetadataReference
				{
					metadata = metadataHandle.Result,
					metadataKey = metadataHandle.Result.PersistentGuid
				};
				allMetadata.Add(metadataReference);
				categories.Add(metadataHandle.Result.category, metadataReference);
			}
		}
	}

	private void EnsureButtonInstances(int targetCount)
	{
		int num = targetCount - cosmeticsButtons.Count;
		for (int i = 0; i < num; i++)
		{
			PlayerCustomizationCosmeticButton playerCustomizationCosmeticButton = UnityEngine.Object.Instantiate(buttonTemplate);
			playerCustomizationCosmeticButton.transform.SetParent(buttonTemplate.transform.parent);
			playerCustomizationCosmeticButton.transform.localScale = Vector3.one;
			cosmeticsButtons.Add(playerCustomizationCosmeticButton);
		}
	}

	private void UpdateDanceButtons()
	{
		EnsureButtonInstances(allVictoryDances.Length);
		int index = 0;
		for (int i = 0; i < cosmeticsButtons.Count; i++)
		{
			PlayerCustomizationCosmeticButton playerCustomizationCosmeticButton = cosmeticsButtons[i];
			if (playerCustomizationCosmeticButton == null)
			{
				return;
			}
			playerCustomizationCosmeticButton.select.onClick.RemoveAllListeners();
			playerCustomizationCosmeticButton.unlockedHighlight.SetActive(value: false);
			tooltip.DeregisterTooltip(playerCustomizationCosmeticButton.select.image.rectTransform);
			if (i < allVictoryDances.Length)
			{
				PlayerCosmeticsVictoryDanceMetadata playerCosmeticsVictoryDanceMetadata = allVictoryDances[i];
				playerCustomizationCosmeticButton.gameObject.SetActive(value: true);
				playerCustomizationCosmeticButton.select.image.sprite = defaultButtonBackground;
				playerCustomizationCosmeticButton.fallbackText.text = playerCosmeticsVictoryDanceMetadata.dance.ToString();
				playerCustomizationCosmeticButton.icon.sprite = playerCosmeticsVictoryDanceMetadata.icon;
				playerCustomizationCosmeticButton.icon.gameObject.SetActive(playerCosmeticsVictoryDanceMetadata.icon != null);
				playerCustomizationCosmeticButton.fallbackText.gameObject.SetActive(playerCosmeticsVictoryDanceMetadata.icon == null);
				playerCustomizationCosmeticButton.select.image.sprite = defaultButtonBackground;
				playerCustomizationCosmeticButton.isUnlocked = true;
				int danceIndex = i;
				playerCustomizationCosmeticButton.select.onClick.AddListener(delegate
				{
					SetDance(danceIndex, isPlayerSelection: true);
				});
				if (playerCosmeticsVictoryDanceMetadata.dance == selectedVictoryDance)
				{
					index = i;
				}
				UpdateButtonCost(playerCustomizationCosmeticButton, playerCosmeticsVictoryDanceMetadata);
			}
			else
			{
				playerCustomizationCosmeticButton.gameObject.SetActive(value: false);
			}
		}
		Canvas.ForceUpdateCanvases();
		SetDance(index, isPlayerSelection: false);
		void SetDance(int num, bool isPlayerSelection)
		{
			SetButtonSelected(num);
			VictoryDance victoryDance = selectedVictoryDance;
			VictoryDance victoryDance2 = (selectedVictoryDance = allVictoryDances[num].dance);
			characterPreview.cosmeticsSwitcher.rightHand.SetEquipmentPreviewLocal(EquipmentType.None);
			SetPreviewAnimation(victoryDance2);
			if (isPlayerSelection && victoryDance2 != victoryDance)
			{
				TutorialManager.CompleteObjective(TutorialObjective.CustomizeAppearance);
			}
			UpdateOwnershipWarnings(PlayerCosmeticsMetadata.Category.VictoryDance, allVictoryDances[num]);
		}
	}

	private void SetPreviewAnimation(VictoryDance dance)
	{
		Animator component = characterPreview.cosmeticsSwitcher.GetComponent<Animator>();
		component.SetInteger(PlayerAnimatorIo.victoryDanceHash, 0);
		component.gameObject.SetActive(value: false);
		component.gameObject.SetActive(value: true);
		characterPreview.Refresh();
		component.SetInteger(PlayerAnimatorIo.victoryDanceHash, (int)dance);
	}

	private void UpdateCosmeticsButtons()
	{
		if (!categories.TryGetValues(currentCategory, out var values))
		{
			Debug.LogWarning("Invalid category!!!");
		}
		values.Sort(new MetadataCmp());
		EnsureButtonInstances(values.Count + 1);
		PlayerCosmeticsSwitcher.CosmeticKey equippedCosmeticKey = GetEquippedCosmeticKey(currentCategory);
		int num = 0;
		int num2 = 0;
		int num3 = 0;
		bool flag = false;
		Sprite defaultIcon = GetDefaultIcon(currentCategory);
		bool flag2 = defaultIcon != null;
		for (int i = 0; i < cosmeticsButtons.Count; i++)
		{
			PlayerCustomizationCosmeticButton playerCustomizationCosmeticButton = cosmeticsButtons[i];
			if (playerCustomizationCosmeticButton == null)
			{
				return;
			}
			playerCustomizationCosmeticButton.select.onClick.RemoveAllListeners();
			MetadataReference reference = null;
			bool flag3 = true;
			if (i == 0)
			{
				playerCustomizationCosmeticButton.gameObject.SetActive(value: true);
				playerCustomizationCosmeticButton.fallbackText.gameObject.SetActive(value: false);
				playerCustomizationCosmeticButton.icon.sprite = defaultIcon;
				playerCustomizationCosmeticButton.icon.gameObject.SetActive(flag2);
				playerCustomizationCosmeticButton.unlockedHighlight.SetActive(value: false);
				playerCustomizationCosmeticButton.select.image.sprite = (flag2 ? defaultButtonBackground : defaultNoneButtonBackground);
				UpdateButtonCost(playerCustomizationCosmeticButton, null);
				tooltip.DeregisterTooltip(playerCustomizationCosmeticButton.select.image.rectTransform);
			}
			else
			{
				bool flag4 = values != null && i - 1 < values.Count;
				playerCustomizationCosmeticButton.gameObject.SetActive(flag4);
				if (flag4)
				{
					reference = values[i - 1];
					PlayerCosmeticsMetadata metadata = reference.metadata;
					if (metadata == null)
					{
						continue;
					}
					if (metadata.IsHidden())
					{
						playerCustomizationCosmeticButton.gameObject.SetActive(value: true);
						playerCustomizationCosmeticButton.fallbackText.gameObject.SetActive(value: false);
						playerCustomizationCosmeticButton.icon.gameObject.SetActive(value: false);
						playerCustomizationCosmeticButton.select.image.sprite = notUnlockedButtonBackground;
						UpdateButtonCost(playerCustomizationCosmeticButton, null);
						flag3 = false;
						RegisterTooltip(playerCustomizationCosmeticButton, metadata);
						playerCustomizationCosmeticButton.unlockedHighlight.SetActive(value: false);
					}
					else
					{
						bool flag5 = CosmeticsUnlocksManager.OwnsCosmetic(metadata);
						if (flag5)
						{
							num3++;
						}
						UpdateButtonCost(playerCustomizationCosmeticButton, metadata);
						playerCustomizationCosmeticButton.select.image.sprite = (flag5 ? defaultButtonBackground : notOwnedButtonBackground);
						playerCustomizationCosmeticButton.fallbackText.text = metadata.name;
						playerCustomizationCosmeticButton.icon.sprite = metadata.icon;
						flag |= (string)metadata.model.RuntimeKey == string.Empty;
						playerCustomizationCosmeticButton.icon.gameObject.SetActive(metadata.icon != null);
						playerCustomizationCosmeticButton.fallbackText.gameObject.SetActive(metadata.icon == null);
						if ((equippedCosmeticKey?.metadataKey == reference.metadataKey || equippedCosmeticKey?.metadataKey == reference.metadata.PersistentGuid) | ((equippedCosmeticKey == null || equippedCosmeticKey.metadataKey == string.Empty) && (string)metadata.model.RuntimeKey == string.Empty))
						{
							num = i;
							persistentData.AcknowledgeCosmetic(metadata.PersistentGuid);
						}
						playerCustomizationCosmeticButton.unlockedHighlight.SetActive(metadata.unlockedByAchievment && !persistentData.HasAcknowledgedCosmetic(metadata.PersistentGuid));
						if (metadata.unlockedByAchievment || !string.IsNullOrEmpty(metadata.externalIpCredit))
						{
							RegisterTooltip(playerCustomizationCosmeticButton, metadata);
						}
						else
						{
							tooltip.DeregisterTooltip(playerCustomizationCosmeticButton.select.image.rectTransform);
						}
					}
					num2++;
				}
				else
				{
					tooltip.DeregisterTooltip(playerCustomizationCosmeticButton.select.image.rectTransform);
				}
			}
			if (flag3)
			{
				int buttonIndex = i;
				playerCustomizationCosmeticButton.select.onClick.AddListener(delegate
				{
					OnCosmeticButtonPressed(buttonIndex, reference, GetSetCosmeticCallback(currentCategory));
				});
			}
			playerCustomizationCosmeticButton.isUnlocked = flag3;
		}
		if (currentCategory == PlayerCosmeticsMetadata.Category.Clothing)
		{
			cosmeticsButtons[0].gameObject.SetActive(value: false);
		}
		unlocked.text = string.Format(Localization.UI.CUSTOMIZE_Label_Unlocked, num3, num2);
		UpdateColors((num == 0) ? null : values[num - 1]);
		Canvas.ForceUpdateCanvases();
		SetButtonSelected(num);
		UpdateOwnershipWarnings(currentCategory, (num > 0) ? values[num - 1].metadata : null);
		void RegisterTooltip(PlayerCustomizationCosmeticButton button, PlayerCosmeticsMetadata playerCosmeticsMetadata)
		{
			string message = string.Empty;
			string secondaryMessage = string.Empty;
			if (playerCosmeticsMetadata.unlockedByAchievment)
			{
				message = string.Format(playerCosmeticsMetadata.IsHidden() ? Localization.UI.CUSTOMIZE_Tooltip_UnlockedByAchievement : Localization.UI.CUSTOMIZE_Tooltip_AchievementUnlockable, string.Concat(str1: LocalizationManager.GetString(StringTable.Achievements, $"ACHIEVEMENT_Title_{playerCosmeticsMetadata.requiredAchievement}"), str0: GameManager.UiSettings.TextOrangeHighlightStartTag, str2: GameManager.UiSettings.TextColorEndTag));
			}
			if (!string.IsNullOrEmpty(playerCosmeticsMetadata.externalIpCredit))
			{
				secondaryMessage = string.Format(Localization.UI.CUSTOMIZE_Tooltip_IpCredit, GameManager.UiSettings.TextOrangeHighlightStartTag + playerCosmeticsMetadata.externalIpCredit + GameManager.UiSettings.TextColorEndTag);
			}
			tooltip.RegisterTooltip(button.select.image.rectTransform, message, secondaryMessage);
		}
	}

	private void UpdateButtonCost(PlayerCustomizationCosmeticButton button, ScriptableObject metadata)
	{
		bool flag = false;
		int cost = 0;
		Action buy = null;
		PlayerCosmeticsMetadata cosmetic = metadata as PlayerCosmeticsMetadata;
		if ((object)cosmetic != null)
		{
			flag = CosmeticsUnlocksManager.OwnsCosmetic(cosmetic);
			cost = cosmetic.cost;
			buy = delegate
			{
				if (CosmeticsUnlocksManager.PurchaseCosmetic(cosmetic))
				{
					RuntimeManager.PlayOneShot(GameManager.AudioSettings.CosmeticsPurchase);
				}
				UpdateOwnershipWarnings(currentCategory, cosmetic);
			};
		}
		else
		{
			PlayerCosmeticsVictoryDanceMetadata dance = metadata as PlayerCosmeticsVictoryDanceMetadata;
			if ((object)dance != null)
			{
				flag = CosmeticsUnlocksManager.OwnsDance(dance);
				cost = dance.cost;
				buy = delegate
				{
					if (CosmeticsUnlocksManager.PurchaseDance(dance))
					{
						RuntimeManager.PlayOneShot(GameManager.AudioSettings.CosmeticsPurchase);
					}
					UpdateOwnershipWarnings(currentCategory, dance);
				};
			}
			else if (metadata != null)
			{
				throw new ArgumentException("Invalid metadata!");
			}
		}
		button.buy.onClick.RemoveAllListeners();
		if (metadata != null)
		{
			button.price.color = (CanAfford(cost) ? normalPriceColor : tooExpensivePriceColor);
			button.buy.gameObject.SetActive(!flag);
			if (!flag)
			{
				button.price.text = cost.ToString();
				button.buy.onClick.AddListener(delegate
				{
					buy();
					SetCategory((int)currentCategory);
				});
			}
		}
		else
		{
			button.buy.gameObject.SetActive(value: false);
		}
	}

	private void UpdateColors(MetadataReference reference, int variationIndexOverride = -1, bool updateNavigation = true)
	{
		PlayerCosmeticsMetadata metadata = reference?.metadata ?? null;
		int valueOrDefault = (metadata?.variations?.Length).GetValueOrDefault();
		int num = valueOrDefault - colorButtons.Count;
		for (int i = 0; i < num; i++)
		{
			GameObject gameObject = UnityEngine.Object.Instantiate(colorTemplate);
			gameObject.transform.SetParent(colorTemplate.transform.parent);
			gameObject.transform.localScale = Vector3.one;
			colorButtons.Add(gameObject.GetComponent<Button>());
		}
		for (int j = 0; j < colorButtons.Count; j++)
		{
			Button button = colorButtons[j];
			if (button == null)
			{
				return;
			}
			bool flag = metadata != null && metadata.variations != null && j < metadata.variations.Length;
			button.gameObject.SetActive(flag);
			if (flag)
			{
				Image component = button.transform.GetChild(2).GetComponent<Image>();
				Color menuColor = metadata.variations[j].menuColor;
				menuColor.a = 1f;
				component.color = menuColor;
				int buttonIndex = j;
				button.onClick.RemoveAllListeners();
				button.onClick.AddListener(delegate
				{
					GetSetCosmeticCallback(metadata.category)(new PlayerCosmeticsSwitcher.CosmeticKey
					{
						metadataKey = reference.metadataKey,
						variationIndex = (sbyte)buttonIndex
					}, arg2: true);
					SetColorButtonSelected(buttonIndex);
				});
			}
		}
		hasColorVariations = valueOrDefault > 0;
		if (hasColorVariations)
		{
			SetColorButtonSelected((variationIndexOverride >= 0) ? variationIndexOverride : (GetEquippedCosmeticKey(metadata.category)?.variationIndex ?? 0), updateNavigation);
		}
		UpdateNavigationPrompts();
	}

	private void InitSkinColors()
	{
		for (int i = 0; i < settings.skinColors.Length; i++)
		{
			GameObject obj = UnityEngine.Object.Instantiate(skinColorTemplate);
			obj.transform.SetParent(skinColorTemplate.transform.parent);
			obj.transform.localScale = Vector3.one;
			obj.SetActive(value: true);
			Button component = obj.GetComponent<Button>();
			skinButtons.Add(component);
			Image component2 = component.transform.GetChild(2).GetComponent<Image>();
			Color iconColor = settings.skinColors[i].iconColor;
			iconColor.a = 1f;
			component2.color = iconColor;
			int skinColorIndex = i;
			component.onClick.AddListener(delegate
			{
				SetSkinColorIndex(skinColorIndex, isPlayerSelection: true);
			});
		}
		SetSkinColorIndex(characterPreview.cosmeticsSwitcher.CurrentSkinColorIndex, isPlayerSelection: false);
	}

	private void SetSkinColorIndex(int index, bool isPlayerSelection)
	{
		for (int i = 0; i < skinButtons.Count; i++)
		{
			skinButtons[i].transform.GetChild(0).gameObject.SetActive(i == index);
		}
		int currentSkinColorIndex = characterPreview.cosmeticsSwitcher.CurrentSkinColorIndex;
		characterPreview.cosmeticsSwitcher.SetSkinColor(index);
		if (isPlayerSelection && index != currentSkinColorIndex)
		{
			TutorialManager.CompleteObjective(TutorialObjective.CustomizeAppearance);
		}
	}

	private PlayerCosmeticsSwitcher.CosmeticKey GetEquippedCosmeticKey(PlayerCosmeticsMetadata.Category category)
	{
		switch (category)
		{
		case PlayerCosmeticsMetadata.Category.Hat:
			return characterPreview.cosmeticsSwitcher.CurrentHatRuntimeKey;
		case PlayerCosmeticsMetadata.Category.Face:
			return characterPreview.cosmeticsSwitcher.CurrentFaceRuntimeKey;
		case PlayerCosmeticsMetadata.Category.FaceLower:
			return characterPreview.cosmeticsSwitcher.CurrentLowerFaceRuntimeKey;
		case PlayerCosmeticsMetadata.Category.Head:
		case PlayerCosmeticsMetadata.Category.Hair:
			return characterPreview.cosmeticsSwitcher.CurrentHeadRuntimeKey;
		case PlayerCosmeticsMetadata.Category.Club:
			return characterPreview.cosmeticsSwitcher.CurrentClubRuntimeKey;
		case PlayerCosmeticsMetadata.Category.Clothing:
			return characterPreview.cosmeticsSwitcher.CurrentBodyRuntimeKey;
		case PlayerCosmeticsMetadata.Category.Mouth:
			return characterPreview.cosmeticsSwitcher.CurrentMouthRuntimeKey;
		case PlayerCosmeticsMetadata.Category.Eyes:
			return characterPreview.cosmeticsSwitcher.CurrentEyesRuntimeKey;
		case PlayerCosmeticsMetadata.Category.Brows:
			return characterPreview.cosmeticsSwitcher.CurrentBrowsRuntimeKey;
		case PlayerCosmeticsMetadata.Category.Cheeks:
			return characterPreview.cosmeticsSwitcher.CurrentCheeksRuntimeKey;
		case PlayerCosmeticsMetadata.Category.Golfball:
			return characterPreview.cosmeticsSwitcher.CurrentGolfBallRuntimeKey;
		default:
			return null;
		}
	}

	private void SetTabSelected(GameObject tab, bool selected)
	{
		GameObject gameObject = tab.transform.GetChild(0).gameObject;
		Image component = tab.transform.GetChild(1).GetComponent<Image>();
		gameObject.SetActive(selected);
		component.color = (selected ? tabIconSelectedColor : tabIconNormalColor);
	}

	private void SetButtonSelected(int buttonIndex)
	{
		for (int i = 0; i < cosmeticsButtons.Count; i++)
		{
			PlayerCustomizationCosmeticButton playerCustomizationCosmeticButton = cosmeticsButtons[i];
			bool flag = i == buttonIndex;
			playerCustomizationCosmeticButton.highlight.SetActive(flag);
			if (flag)
			{
				scrollRect.EnsureVisibility(playerCustomizationCosmeticButton.GetComponent<RectTransform>(), GameManager.UiSettings.ScrollRectControllerReselectDefaultPadding);
			}
		}
		if (InputManager.UsingGamepad && cosmeticsButtons[buttonIndex].TryGetComponent<ControllerSelectable>(out var component))
		{
			navigation.Select(component);
		}
	}

	private void SetColorButtonSelected(int buttonIndex, bool updateNavigation = true)
	{
		for (int i = 0; i < colorButtons.Count; i++)
		{
			SetButtonSelectionActive(colorButtons[i].gameObject, i == buttonIndex);
		}
		if (updateNavigation && InputManager.UsingGamepad && colorButtons[buttonIndex].TryGetComponent<ControllerSelectable>(out var component))
		{
			navigation.Select(component);
		}
		lastCosmeticVariationIndex = buttonIndex;
		static void SetButtonSelectionActive(GameObject button, bool selected)
		{
			button.transform.GetChild(0).gameObject.SetActive(selected);
		}
	}

	private bool IsCosmeticButtonSelected(int buttonIndex)
	{
		return IsButtonSelected(buttonIndex, cosmeticsButtons);
	}

	private bool IsButtonSelected(int buttonIndex, List<PlayerCustomizationCosmeticButton> buttons, int selectionChildIndex = 0)
	{
		return buttons[buttonIndex].highlight.activeSelf;
	}

	private Action<PlayerCosmeticsSwitcher.CosmeticKey, bool> GetSetCosmeticCallback(PlayerCosmeticsMetadata.Category currentCategory)
	{
		return currentCategory switch
		{
			PlayerCosmeticsMetadata.Category.Hat => delegate(PlayerCosmeticsSwitcher.CosmeticKey key, bool player)
			{
				SetHatModel(key, player).Forget();
			}, 
			PlayerCosmeticsMetadata.Category.Face => delegate(PlayerCosmeticsSwitcher.CosmeticKey key, bool player)
			{
				SetFaceModel(key, player).Forget();
			}, 
			PlayerCosmeticsMetadata.Category.FaceLower => delegate(PlayerCosmeticsSwitcher.CosmeticKey key, bool player)
			{
				SetLowerFaceModel(key, player).Forget();
			}, 
			PlayerCosmeticsMetadata.Category.Hair => delegate(PlayerCosmeticsSwitcher.CosmeticKey key, bool player)
			{
				SetHairModel(key, player).Forget();
			}, 
			PlayerCosmeticsMetadata.Category.Head => delegate(PlayerCosmeticsSwitcher.CosmeticKey key, bool player)
			{
				SetHeadModel(key, player).Forget();
			}, 
			PlayerCosmeticsMetadata.Category.Club => delegate(PlayerCosmeticsSwitcher.CosmeticKey key, bool player)
			{
				SetClubModel(key, player).Forget();
			}, 
			PlayerCosmeticsMetadata.Category.Clothing => delegate(PlayerCosmeticsSwitcher.CosmeticKey key, bool player)
			{
				SetBodyModel(key, player).Forget();
			}, 
			PlayerCosmeticsMetadata.Category.Mouth => delegate(PlayerCosmeticsSwitcher.CosmeticKey key, bool player)
			{
				SetMouthTexture(key, player).Forget();
			}, 
			PlayerCosmeticsMetadata.Category.Eyes => delegate(PlayerCosmeticsSwitcher.CosmeticKey key, bool player)
			{
				SetEyesTexture(key, player).Forget();
			}, 
			PlayerCosmeticsMetadata.Category.Cheeks => delegate(PlayerCosmeticsSwitcher.CosmeticKey key, bool player)
			{
				SetCheeksTexture(key, player).Forget();
			}, 
			PlayerCosmeticsMetadata.Category.Brows => delegate(PlayerCosmeticsSwitcher.CosmeticKey key, bool player)
			{
				SetBrowsTexture(key, player).Forget();
			}, 
			PlayerCosmeticsMetadata.Category.Golfball => delegate(PlayerCosmeticsSwitcher.CosmeticKey key, bool player)
			{
				SetGolfBallModel(key, player).Forget();
			}, 
			_ => null, 
		};
	}

	private void OnCosmeticButtonPressed(int buttonIndex, MetadataReference reference, Action<PlayerCosmeticsSwitcher.CosmeticKey, bool> SetCosmetic)
	{
		if (IsCosmeticButtonSelected(buttonIndex))
		{
			int num = ((!cosmeticsButtons[0].gameObject.activeSelf) ? 1 : 0);
			if (num != buttonIndex)
			{
				cosmeticsButtons[num].select.onClick.Invoke();
				return;
			}
		}
		SetButtonSelected(buttonIndex);
		lastCosmeticVariationIndex = BMath.Clamp(lastCosmeticVariationIndex, 0, ((reference == null) ? ((int?)null) : (reference.metadata.variations?.Length - 1)).GetValueOrDefault());
		SetCosmetic(new PlayerCosmeticsSwitcher.CosmeticKey
		{
			metadataKey = (reference?.metadataKey ?? string.Empty),
			variationIndex = (sbyte)lastCosmeticVariationIndex
		}, arg2: true);
		UpdateColors(reference, lastCosmeticVariationIndex, updateNavigation: false);
		if (reference != null)
		{
			persistentData.AcknowledgeCosmetic(reference.metadata.PersistentGuid);
			cosmeticsButtons[buttonIndex].unlockedHighlight.SetActive(value: false);
		}
	}

	private void RefreshUnequippedOwnershipWarnings()
	{
		if (characterPreview.cosmeticsSwitcher.CurrentHeadRuntimeKey == null)
		{
			UpdateOwnershipWarnings(PlayerCosmeticsMetadata.Category.Head, (PlayerCosmeticsMetadata)null);
			UpdateOwnershipWarnings(PlayerCosmeticsMetadata.Category.Hair, (PlayerCosmeticsMetadata)null);
		}
		if (characterPreview.cosmeticsSwitcher.CurrentHatRuntimeKey == null)
		{
			UpdateOwnershipWarnings(PlayerCosmeticsMetadata.Category.Hat, (PlayerCosmeticsMetadata)null);
		}
		if (characterPreview.cosmeticsSwitcher.CurrentFaceRuntimeKey == null)
		{
			UpdateOwnershipWarnings(PlayerCosmeticsMetadata.Category.Face, (PlayerCosmeticsMetadata)null);
		}
		if (characterPreview.cosmeticsSwitcher.CurrentLowerFaceRuntimeKey == null)
		{
			UpdateOwnershipWarnings(PlayerCosmeticsMetadata.Category.FaceLower, (PlayerCosmeticsMetadata)null);
		}
	}

	private void UpdateOwnershipWarnings(PlayerCosmeticsMetadata.Category selectedCategory, PlayerCosmeticsMetadata metadata)
	{
		UpdateOwnershipWarningsInternal(selectedCategory, metadata != null, () => CosmeticsUnlocksManager.OwnsCosmetic(metadata), delegate
		{
			CosmeticsUnlocksManager.PurchaseCosmetic(metadata);
		}, metadata?.PersistentGuid, metadata?.cost ?? (-1));
	}

	private void UpdateOwnershipWarnings(PlayerCosmeticsMetadata.Category selectedCategory, PlayerCosmeticsVictoryDanceMetadata metadata)
	{
		UpdateOwnershipWarningsInternal(selectedCategory, metadata != null, () => CosmeticsUnlocksManager.OwnsDance(metadata), delegate
		{
			CosmeticsUnlocksManager.PurchaseDance(metadata);
		}, metadata?.persistentGuid, metadata?.cost ?? (-1));
	}

	private void UpdateOwnershipWarningsInternal(PlayerCosmeticsMetadata.Category selectedCategory, bool validCosmetic, Func<bool> OwnsCosmetic, Action PurchaseCosmetic, string persistentGuid, int cost)
	{
		bool flag = selectedCategory == currentCategory;
		if (validCosmetic)
		{
			bool flag2 = OwnsCosmetic?.Invoke() ?? false;
			tabs[(int)selectedCategory].transform.GetChild(2).gameObject.SetActive(!flag2);
			if (!flag)
			{
				return;
			}
			buyPrompt.SetActive(!flag2);
			if (!flag2)
			{
				bool flag3 = CanAfford(cost);
				costLabel.text = cost.ToString();
				costLabel.color = (flag3 ? normalPriceColor : tooExpensivePriceColor);
				buyButton.onClick.RemoveAllListeners();
				if (flag3)
				{
					buyButton.onClick.AddListener(OnBuy);
				}
				buyButton.interactable = flag3;
			}
		}
		else
		{
			tabs[(int)selectedCategory].transform.GetChild(2).gameObject.SetActive(value: false);
			if (flag)
			{
				buyPrompt.SetActive(value: false);
			}
		}
		void OnBuy()
		{
			PurchaseCosmetic?.Invoke();
			UpdateOwnershipWarningsInternal(selectedCategory, validCosmetic, OwnsCosmetic, PurchaseCosmetic, persistentGuid, cost);
			SetCategory((int)selectedCategory);
			buyPrompt.SetActive(value: false);
			RuntimeManager.PlayOneShot(GameManager.AudioSettings.CosmeticsPurchase);
		}
	}

	private bool CanAfford(int cost)
	{
		return cost <= CosmeticsUnlocksManager.GetCredits();
	}

	private void UpdateCategorySelection(PlayerCosmeticsMetadata.Category category, PlayerCosmeticsSwitcher.CosmeticKey cosmeticKey)
	{
		categories.TryGetValues(category, out var values);
		MetadataReference metadataReference = null;
		if (cosmeticKey != null && !string.IsNullOrEmpty(cosmeticKey.metadataKey))
		{
			for (int i = 0; i < values.Count; i++)
			{
				if (values[i].metadataKey == cosmeticKey.metadataKey)
				{
					metadataReference = values[i];
					break;
				}
			}
		}
		selectedCosmetics[(int)category] = metadataReference;
		selectedCosmeticKeys[(int)category] = cosmeticKey;
		UpdateOwnershipWarnings(category, metadataReference?.metadata);
	}

	private async UniTask SetHatModel(PlayerCosmeticsSwitcher.CosmeticKey cosmeticKey, bool isPlayerSelection)
	{
		PlayerCosmeticsSwitcher.CosmeticKey previousKey = characterPreview.cosmeticsSwitcher.CurrentHatRuntimeKey;
		await characterPreview.cosmeticsSwitcher.SetHatModel(cosmeticKey);
		if (isPlayerSelection && !cosmeticKey.Equals(previousKey))
		{
			TutorialManager.CompleteObjective(TutorialObjective.CustomizeAppearance);
		}
		UpdateCategorySelection(PlayerCosmeticsMetadata.Category.Hat, cosmeticKey);
		RefreshUnequippedOwnershipWarnings();
	}

	private async UniTask SetHeadModel(PlayerCosmeticsSwitcher.CosmeticKey cosmeticKey, bool isPlayerSelection)
	{
		PlayerCosmeticsSwitcher.CosmeticKey previousKey = characterPreview.cosmeticsSwitcher.CurrentHeadRuntimeKey;
		await characterPreview.cosmeticsSwitcher.SetHeadModel(cosmeticKey);
		if (isPlayerSelection && !cosmeticKey.Equals(previousKey))
		{
			TutorialManager.CompleteObjective(TutorialObjective.CustomizeAppearance);
		}
		UpdateCategorySelection(PlayerCosmeticsMetadata.Category.Hair, null);
		UpdateCategorySelection(PlayerCosmeticsMetadata.Category.Head, cosmeticKey);
		RefreshUnequippedOwnershipWarnings();
	}

	private async UniTask SetHairModel(PlayerCosmeticsSwitcher.CosmeticKey cosmeticKey, bool isPlayerSelection)
	{
		PlayerCosmeticsSwitcher.CosmeticKey previousKey = characterPreview.cosmeticsSwitcher.CurrentHeadRuntimeKey;
		await characterPreview.cosmeticsSwitcher.SetHeadModel(cosmeticKey);
		if (isPlayerSelection && !cosmeticKey.Equals(previousKey))
		{
			TutorialManager.CompleteObjective(TutorialObjective.CustomizeAppearance);
		}
		UpdateCategorySelection(PlayerCosmeticsMetadata.Category.Head, null);
		UpdateCategorySelection(PlayerCosmeticsMetadata.Category.Hair, cosmeticKey);
		RefreshUnequippedOwnershipWarnings();
	}

	private async UniTask SetClubModel(PlayerCosmeticsSwitcher.CosmeticKey cosmeticKey, bool isPlayerSelection)
	{
		PlayerCosmeticsSwitcher.CosmeticKey previousKey = characterPreview.cosmeticsSwitcher.CurrentClubRuntimeKey;
		await characterPreview.cosmeticsSwitcher.SetClubModel(cosmeticKey);
		if (isPlayerSelection && !cosmeticKey.Equals(previousKey))
		{
			TutorialManager.CompleteObjective(TutorialObjective.CustomizeAppearance);
		}
		UpdateCategorySelection(PlayerCosmeticsMetadata.Category.Club, cosmeticKey);
	}

	private async UniTask SetFaceModel(PlayerCosmeticsSwitcher.CosmeticKey cosmeticKey, bool isPlayerSelection)
	{
		PlayerCosmeticsSwitcher.CosmeticKey previousKey = characterPreview.cosmeticsSwitcher.CurrentFaceRuntimeKey;
		await characterPreview.cosmeticsSwitcher.SetFaceModel(cosmeticKey);
		if (isPlayerSelection && !cosmeticKey.Equals(previousKey))
		{
			TutorialManager.CompleteObjective(TutorialObjective.CustomizeAppearance);
		}
		UpdateCategorySelection(PlayerCosmeticsMetadata.Category.Face, cosmeticKey);
		RefreshUnequippedOwnershipWarnings();
	}

	private async UniTask SetLowerFaceModel(PlayerCosmeticsSwitcher.CosmeticKey cosmeticKey, bool isPlayerSelection)
	{
		PlayerCosmeticsSwitcher.CosmeticKey previousKey = characterPreview.cosmeticsSwitcher.CurrentLowerFaceRuntimeKey;
		await characterPreview.cosmeticsSwitcher.SetLowerFaceModel(cosmeticKey);
		if (isPlayerSelection && !cosmeticKey.Equals(previousKey))
		{
			TutorialManager.CompleteObjective(TutorialObjective.CustomizeAppearance);
		}
		UpdateCategorySelection(PlayerCosmeticsMetadata.Category.FaceLower, cosmeticKey);
		RefreshUnequippedOwnershipWarnings();
	}

	private async UniTask SetBodyModel(PlayerCosmeticsSwitcher.CosmeticKey cosmeticKey, bool isPlayerSelection)
	{
		PlayerCosmeticsSwitcher.CosmeticKey previousKey = characterPreview.cosmeticsSwitcher.CurrentBodyRuntimeKey;
		await characterPreview.cosmeticsSwitcher.SetBodyModel(cosmeticKey);
		if (isPlayerSelection && !cosmeticKey.Equals(previousKey))
		{
			TutorialManager.CompleteObjective(TutorialObjective.CustomizeAppearance);
		}
		UpdateCategorySelection(PlayerCosmeticsMetadata.Category.Clothing, cosmeticKey);
	}

	private async UniTask SetMouthTexture(PlayerCosmeticsSwitcher.CosmeticKey cosmeticKey, bool isPlayerSelection)
	{
		PlayerCosmeticsSwitcher.CosmeticKey previousKey = characterPreview.cosmeticsSwitcher.CurrentMouthRuntimeKey;
		await characterPreview.cosmeticsSwitcher.SetMouthTexture(cosmeticKey);
		if (isPlayerSelection && !cosmeticKey.Equals(previousKey))
		{
			TutorialManager.CompleteObjective(TutorialObjective.CustomizeAppearance);
		}
		UpdateCategorySelection(PlayerCosmeticsMetadata.Category.Mouth, cosmeticKey);
	}

	private async UniTask SetEyesTexture(PlayerCosmeticsSwitcher.CosmeticKey cosmeticKey, bool isPlayerSelection)
	{
		PlayerCosmeticsSwitcher.CosmeticKey previousKey = characterPreview.cosmeticsSwitcher.CurrentEyesRuntimeKey;
		await characterPreview.cosmeticsSwitcher.SetEyesTexture(cosmeticKey);
		if (isPlayerSelection && !cosmeticKey.Equals(previousKey))
		{
			TutorialManager.CompleteObjective(TutorialObjective.CustomizeAppearance);
		}
		UpdateCategorySelection(PlayerCosmeticsMetadata.Category.Eyes, cosmeticKey);
	}

	private async UniTask SetBrowsTexture(PlayerCosmeticsSwitcher.CosmeticKey cosmeticKey, bool isPlayerSelection)
	{
		PlayerCosmeticsSwitcher.CosmeticKey previousKey = characterPreview.cosmeticsSwitcher.CurrentBrowsRuntimeKey;
		await characterPreview.cosmeticsSwitcher.SetBrowTexture(cosmeticKey);
		if (isPlayerSelection && !cosmeticKey.Equals(previousKey))
		{
			TutorialManager.CompleteObjective(TutorialObjective.CustomizeAppearance);
		}
		UpdateCategorySelection(PlayerCosmeticsMetadata.Category.Brows, cosmeticKey);
	}

	private async UniTask SetCheeksTexture(PlayerCosmeticsSwitcher.CosmeticKey cosmeticKey, bool isPlayerSelection)
	{
		PlayerCosmeticsSwitcher.CosmeticKey previousKey = characterPreview.cosmeticsSwitcher.CurrentCheeksRuntimeKey;
		await characterPreview.cosmeticsSwitcher.SetCheeksTexture(cosmeticKey);
		if (isPlayerSelection && !cosmeticKey.Equals(previousKey))
		{
			TutorialManager.CompleteObjective(TutorialObjective.CustomizeAppearance);
		}
		UpdateCategorySelection(PlayerCosmeticsMetadata.Category.Cheeks, cosmeticKey);
	}

	private async UniTask SetGolfBallModel(PlayerCosmeticsSwitcher.CosmeticKey cosmeticKey, bool isPlayerSelection)
	{
		PlayerCosmeticsSwitcher.CosmeticKey previousKey = characterPreview.cosmeticsSwitcher.CurrentGolfBallRuntimeKey;
		await characterPreview.cosmeticsSwitcher.SetGolfBallModel(cosmeticKey);
		if (isPlayerSelection && !cosmeticKey.Equals(previousKey))
		{
			TutorialManager.CompleteObjective(TutorialObjective.CustomizeAppearance);
		}
		UpdateCategorySelection(PlayerCosmeticsMetadata.Category.Golfball, cosmeticKey);
	}
}
