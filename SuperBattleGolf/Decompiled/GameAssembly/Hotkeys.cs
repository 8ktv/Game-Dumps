using System;
using System.Collections;
using System.Threading;
using TMPro;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;
using UnityEngine.UI;

public class Hotkeys : SingletonBehaviour<Hotkeys>, IBUpdateCallback, IAnyBUpdateCallback
{
	[SerializeField]
	private UiVisibilityController visibilityController;

	[SerializeField]
	private HotkeyUi[] hotkeyUis;

	[SerializeField]
	private HotkeyUi[] hotkeyPreviewUis;

	[SerializeField]
	private Image cycleLeftButtonPrompt;

	[SerializeField]
	private Image cycleRightButtonPrompt;

	[SerializeField]
	private Sprite[] golfCartSeatIcons;

	[SerializeField]
	private CanvasGroup inventoryPitchGroup;

	[SerializeField]
	private TMP_Text inventoryPitchLabel;

	[SerializeField]
	private TMP_Text swingPitchLabel;

	[SerializeField]
	private Image swingPitchFill;

	[SerializeField]
	private CanvasGroup inventoryGroup;

	[SerializeField]
	private CanvasGroup swingGroup;

	[SerializeField]
	private Image[] swingButtonPrompts;

	[SerializeField]
	private CanvasGroup swingButtonPromptsGroup;

	[SerializeField]
	private float inventoryPitchFadeInTime;

	[SerializeField]
	private float inventoryPitchFadeOutTime;

	[SerializeField]
	private Sprite defaultGolfClubIcon;

	private PlayerCosmeticsMetadata golfclubMetadata;

	private string golfclubMetadataKey;

	private int selectedIndex = -1;

	private int previousSelectedIndex = -1;

	private HotkeyMode currentMode;

	private bool isInventoryPitchVisible;

	private Coroutine inventoryPitchVisibilityRoutine;

	private float swingPitchFillTarget = 0.5f;

	private CancellationTokenSource golfClubIconCancellationSource;

	public static int SelectedIndex
	{
		get
		{
			if (!SingletonBehaviour<Hotkeys>.HasInstance)
			{
				return 0;
			}
			return SingletonBehaviour<Hotkeys>.Instance.selectedIndex;
		}
	}

	public static HotkeyMode CurrentMode
	{
		get
		{
			if (!SingletonBehaviour<Hotkeys>.HasInstance)
			{
				return HotkeyMode.Inventory;
			}
			return SingletonBehaviour<Hotkeys>.Instance.currentMode;
		}
	}

	public static event Action ModeChanged;

	protected override void Awake()
	{
		base.Awake();
		BUpdate.RegisterCallback(this);
		Reset();
		UpdateInventoryPitchVisibility();
		InputManager.SwitchedInputDeviceType += OnSwitchedInputDeviceType;
		PlayerSpectator.LocalPlayerIsSpectatingChanged += OnLocalPlayerIsSpectatingChanged;
		PlayerSpectator.LocalPlayerSetSpectatingTarget += OnLocalPlayerSetSpectatingTarget;
		PlayerInfo.LocalPlayerEnteredGolfCart += LocalPlayerEnteredGolfCart;
		PlayerInfo.LocalPlayerExitedGolfCart += UpdateCycleButtonPrompts;
		PlayerCosmetics.ClubCosmeticUpdated += OnPlayerClubUpdate;
		GameManager.LocalPlayerRegistered += Reset;
	}

	private void Reset()
	{
		HotkeyUi[] array = hotkeyUis;
		for (int i = 0; i < array.Length; i++)
		{
			array[i].HideName();
		}
		currentMode = HotkeyMode.Inventory;
		SetModeInternal(currentMode, forced: true);
		SelectInternal(0, uiOnly: false);
		OnSwitchedInputDeviceType();
	}

	protected override void OnDestroy()
	{
		base.OnDestroy();
		BUpdate.DeregisterCallback(this);
		InputManager.SwitchedInputDeviceType -= OnSwitchedInputDeviceType;
		PlayerSpectator.LocalPlayerIsSpectatingChanged -= OnLocalPlayerIsSpectatingChanged;
		PlayerSpectator.LocalPlayerSetSpectatingTarget -= OnLocalPlayerSetSpectatingTarget;
		PlayerInfo.LocalPlayerEnteredGolfCart -= LocalPlayerEnteredGolfCart;
		PlayerInfo.LocalPlayerExitedGolfCart -= UpdateCycleButtonPrompts;
		PlayerCosmetics.ClubCosmeticUpdated -= OnPlayerClubUpdate;
		GameManager.LocalPlayerRegistered -= Reset;
		if (golfclubMetadata != null)
		{
			Addressables.Release(golfclubMetadata);
		}
	}

	private void OnPlayerClubUpdate(PlayerCosmetics playerCosmetics)
	{
		PlayerInfo targetPlayer = GetTargetPlayer();
		if (!(targetPlayer == null) && !(targetPlayer.Cosmetics != playerCosmetics))
		{
			UpdateGolfClubIcon();
		}
	}

	private async void UpdateGolfClubIcon()
	{
		if (!CanUpdate())
		{
			return;
		}
		PlayerCosmeticsSwitcher switcher = GetTargetPlayer().Cosmetics.Switcher;
		if (switcher.CurrentClubRuntimeKey == null)
		{
			if (golfclubMetadata != null)
			{
				Addressables.Release(golfclubMetadata);
				golfclubMetadata = null;
			}
			hotkeyUis[0].SetIcon(defaultGolfClubIcon);
			return;
		}
		string metadataKey = switcher.CurrentClubRuntimeKey.metadataKey;
		if (golfclubMetadata != null && golfclubMetadataKey == metadataKey)
		{
			hotkeyUis[0].SetIcon(golfclubMetadata.icon);
			return;
		}
		if (golfClubIconCancellationSource != null)
		{
			golfClubIconCancellationSource.Cancel();
		}
		CancellationTokenSource cancellationToken = (golfClubIconCancellationSource = new CancellationTokenSource());
		hotkeyUis[0].SetIcon(defaultGolfClubIcon);
		if (golfclubMetadata != null)
		{
			Addressables.Release(golfclubMetadata);
			golfclubMetadata = null;
			golfclubMetadataKey = string.Empty;
		}
		try
		{
			_ = defaultGolfClubIcon;
			if (switcher.CurrentClubRuntimeKey != null && switcher.CurrentClubRuntimeKey.metadataKey != string.Empty)
			{
				AsyncOperationHandle<PlayerCosmeticsMetadata> metadata = Addressables.LoadAssetAsync<PlayerCosmeticsMetadata>(metadataKey);
				await metadata;
				if (metadata.Result == null || cancellationToken.IsCancellationRequested || !CanUpdate())
				{
					Addressables.Release(metadata);
					return;
				}
				golfclubMetadata = metadata.Result;
				golfclubMetadataKey = metadataKey;
				hotkeyUis[0].SetIcon(golfclubMetadata.icon);
			}
		}
		finally
		{
			if (golfClubIconCancellationSource == cancellationToken)
			{
				golfClubIconCancellationSource = null;
			}
			cancellationToken.Dispose();
		}
		bool CanUpdate()
		{
			if (currentMode == HotkeyMode.Inventory)
			{
				return GameManager.LocalPlayerInfo != null;
			}
			if (currentMode == HotkeyMode.Spectating)
			{
				if (GameManager.LocalPlayerAsSpectator != null)
				{
					return GameManager.LocalPlayerAsSpectator.TargetPlayer != null;
				}
				return false;
			}
			return false;
		}
	}

	private Sprite GetGolfClubIcon()
	{
		if (golfclubMetadata != null)
		{
			return golfclubMetadata.icon;
		}
		return defaultGolfClubIcon;
	}

	private PlayerInfo GetTargetPlayer()
	{
		if (currentMode == HotkeyMode.Inventory)
		{
			return GameManager.LocalPlayerInfo;
		}
		if (GameManager.LocalPlayerAsSpectator != null)
		{
			return GameManager.LocalPlayerAsSpectator.TargetPlayer;
		}
		return null;
	}

	public static void SetMode(HotkeyMode mode)
	{
		if (SingletonBehaviour<Hotkeys>.HasInstance)
		{
			SingletonBehaviour<Hotkeys>.Instance.SetModeInternal(mode);
		}
	}

	public static void ForceRefreshCurrentMode()
	{
		if (SingletonBehaviour<Hotkeys>.HasInstance)
		{
			SingletonBehaviour<Hotkeys>.Instance.ForceRefreshCurrentModeInternal();
		}
	}

	public static bool TogglePrevious(bool uiOnly = false)
	{
		if (!SingletonBehaviour<Hotkeys>.HasInstance)
		{
			return false;
		}
		return SingletonBehaviour<Hotkeys>.Instance.SelectInternal(SingletonBehaviour<Hotkeys>.Instance.previousSelectedIndex, uiOnly);
	}

	public static bool Select(int index, bool uiOnly = false)
	{
		if (!SingletonBehaviour<Hotkeys>.HasInstance)
		{
			return false;
		}
		return SingletonBehaviour<Hotkeys>.Instance.SelectInternal(index, uiOnly);
	}

	public static bool Deselect(bool uiOnly = false)
	{
		if (!SingletonBehaviour<Hotkeys>.HasInstance)
		{
			return false;
		}
		return SingletonBehaviour<Hotkeys>.Instance.DeselectInternal(uiOnly);
	}

	public static bool CycleLeft()
	{
		if (!SingletonBehaviour<Hotkeys>.HasInstance)
		{
			return false;
		}
		return SingletonBehaviour<Hotkeys>.Instance.CycleLeftInternal();
	}

	public static bool CycleRight()
	{
		if (!SingletonBehaviour<Hotkeys>.HasInstance)
		{
			return false;
		}
		return SingletonBehaviour<Hotkeys>.Instance.CycleRightInternal();
	}

	public static void UpdatePlayerInventoryIcon(int inventorySlotIndex)
	{
		if (SingletonBehaviour<Hotkeys>.HasInstance)
		{
			SingletonBehaviour<Hotkeys>.Instance.UpdatePlayerInventoryHoykeyInternal(inventorySlotIndex);
		}
	}

	public static void UpdateOccupiedGolfCartSeats(GolfCartSeat playerSeat)
	{
		if (SingletonBehaviour<Hotkeys>.HasInstance)
		{
			SingletonBehaviour<Hotkeys>.Instance.UpdateOccupiedGolfCartSeatsInternal(playerSeat);
		}
	}

	public static int HotkeyIndexToInventoryIndex(int hotkeyIndex)
	{
		if (!SingletonBehaviour<Hotkeys>.HasInstance)
		{
			return 0;
		}
		return SingletonBehaviour<Hotkeys>.Instance.HotkeyIndexToInventoryIndexInternal(hotkeyIndex);
	}

	public static int InventoryIndexToHotkeyIndex(int inventoryIndex)
	{
		if (!SingletonBehaviour<Hotkeys>.HasInstance)
		{
			return 0;
		}
		return SingletonBehaviour<Hotkeys>.Instance.InventoryIndexToHotkeyIndexInternal(inventoryIndex);
	}

	public static void SetPitch(float pitch)
	{
		if (SingletonBehaviour<Hotkeys>.HasInstance)
		{
			SingletonBehaviour<Hotkeys>.Instance.SetPitchInternal(pitch);
		}
	}

	private void SetPitchInternal(float pitch)
	{
		TMP_Text tMP_Text = swingPitchLabel;
		string text = (inventoryPitchLabel.text = BMath.Round(pitch) + "°");
		tMP_Text.text = text;
		swingPitchFillTarget = BMath.Remap(0f, 60f, 0f, 0.75f, pitch);
	}

	private void SetModeInternal(HotkeyMode mode, bool forced = false)
	{
		HotkeyMode hotkeyMode = currentMode;
		currentMode = mode;
		if (forced || currentMode != hotkeyMode)
		{
			switch (currentMode)
			{
			case HotkeyMode.Inventory:
				SetInventory();
				break;
			case HotkeyMode.SwingPitch:
				SetSwingPitch();
				break;
			case HotkeyMode.GolfCart:
				SetGolfCart();
				break;
			case HotkeyMode.Spectating:
				SetSpectating();
				break;
			}
			UpdateInventoryPitchVisibility();
			if (currentMode != hotkeyMode)
			{
				Hotkeys.ModeChanged?.Invoke();
			}
		}
		void SetGolfCart()
		{
			for (int i = 0; i < GameManager.GolfCartSettings.MaxPassengers; i++)
			{
				HotkeyUi obj = hotkeyUis[i];
				obj.gameObject.SetActive(value: true);
				obj.SetIcon(golfCartSeatIcons[BMath.Min(i, golfCartSeatIcons.Length - 1)]);
				obj.SetUses(0, 0);
				obj.SetIsGreyedOut(greyedOut: false);
			}
			bool flag = GameManager.LocalPlayerInventory != null;
			for (int j = 0; j < hotkeyPreviewUis.Length; j++)
			{
				HotkeyUi hotkeyUi = hotkeyPreviewUis[j];
				if (j >= GameManager.PlayerInventorySettings.MaxItems)
				{
					hotkeyUi.gameObject.SetActive(value: false);
				}
				else
				{
					hotkeyUi.gameObject.SetActive(value: true);
					if (!flag)
					{
						hotkeyUi.SetIcon(null);
						hotkeyUi.SetUses(0, 0);
						hotkeyUi.SetIsGreyedOut(greyedOut: false);
					}
					else
					{
						SetInventoryHotkeyData(hotkeyUi, GameManager.LocalPlayerInventory, j);
					}
				}
			}
			if (!GameManager.LocalPlayerInfo.ActiveGolfCartSeat.IsValid())
			{
				Debug.LogError("Hotkeys set to golf cart mode, but the local player is not in a golf cart", base.gameObject);
			}
			else
			{
				SelectInternal(GameManager.LocalPlayerInfo.ActiveGolfCartSeat.seat, uiOnly: true, forced: true, animate: false);
			}
		}
		void SetInventory()
		{
			HotkeyUi obj = hotkeyUis[0];
			obj.gameObject.SetActive(value: true);
			obj.SetIcon(GetGolfClubIcon());
			obj.SetUses(0, 0);
			obj.SetIsGreyedOut(greyedOut: false);
			bool flag = GameManager.LocalPlayerInventory != null;
			for (int i = 1; i < hotkeyUis.Length; i++)
			{
				int num = HotkeyIndexToInventoryIndexInternal(i);
				HotkeyUi hotkeyUi = hotkeyUis[i];
				if (num >= GameManager.PlayerInventorySettings.MaxItems)
				{
					hotkeyUi.gameObject.SetActive(value: false);
				}
				else
				{
					hotkeyUi.gameObject.SetActive(value: true);
					if (!flag)
					{
						hotkeyUi.SetIcon(null);
						hotkeyUi.SetUses(0, 0);
						hotkeyUi.SetIsGreyedOut(greyedOut: false);
					}
					else
					{
						SetInventoryHotkeyData(hotkeyUi, GameManager.LocalPlayerInventory, num);
					}
				}
			}
			HotkeyUi[] array = hotkeyPreviewUis;
			for (int j = 0; j < array.Length; j++)
			{
				array[j].gameObject.SetActive(value: false);
			}
			int index = ((flag && GameManager.LocalPlayerInventory.GetEffectivelyEquippedItem(ignoreEquipmentHiding: true) != ItemType.None) ? InventoryIndexToHotkeyIndexInternal(GameManager.LocalPlayerInventory.EquippedItemIndex) : 0);
			SelectInternal(index, uiOnly: true, forced: true, animate: false);
		}
		void SetSpectating()
		{
			HotkeyUi[] array = hotkeyPreviewUis;
			for (int i = 0; i < array.Length; i++)
			{
				array[i].gameObject.SetActive(value: false);
			}
			if (GameManager.LocalPlayerAsSpectator == null || GameManager.LocalPlayerAsSpectator.TargetPlayer == null)
			{
				array = hotkeyUis;
				for (int i = 0; i < array.Length; i++)
				{
					array[i].gameObject.SetActive(value: false);
				}
			}
			else
			{
				HotkeyUi obj = hotkeyUis[0];
				obj.gameObject.SetActive(value: true);
				obj.SetIcon(GetGolfClubIcon());
				obj.SetUses(0, 0);
				obj.SetIsGreyedOut(greyedOut: true);
				PlayerInventory inventory = GameManager.LocalPlayerAsSpectator.TargetPlayer.Inventory;
				int num = ((inventory.GetEffectivelyEquippedItem() != ItemType.None) ? (inventory.PlayerInfo.NetworkedEquippedItemIndex + 1) : 0);
				for (int j = 1; j < hotkeyUis.Length; j++)
				{
					int num2 = HotkeyIndexToInventoryIndexInternal(j);
					HotkeyUi hotkeyUi = hotkeyUis[j];
					if (num2 >= GameManager.PlayerInventorySettings.MaxItems)
					{
						hotkeyUi.gameObject.SetActive(value: false);
					}
					else
					{
						hotkeyUi.gameObject.SetActive(value: true);
						if (j != num)
						{
							SetInventoryUnknownHotkeyData(hotkeyUi);
						}
						else
						{
							SetInventoryHotkeyData(hotkeyUi, inventory, num2);
						}
					}
				}
				SelectInternal(num, uiOnly: true, forced: true, animate: false);
			}
		}
		void SetSwingPitch()
		{
			DeselectInternal(uiOnly: true);
		}
	}

	private void ForceRefreshCurrentModeInternal()
	{
		SetModeInternal(currentMode, forced: true);
	}

	private bool SelectInternal(int index, bool uiOnly, bool forced = false, bool animate = true)
	{
		if (!CanSelect())
		{
			return false;
		}
		int previousSelectedIndex = selectedIndex;
		selectedIndex = index;
		UpdateInventoryPitchVisibility();
		if (!forced && index == previousSelectedIndex)
		{
			return true;
		}
		if (currentMode == HotkeyMode.SwingPitch)
		{
			ApplyToSwingPitch();
			return true;
		}
		if (previousSelectedIndex >= 0 && previousSelectedIndex < hotkeyUis.Length)
		{
			OnDeselected(hotkeyUis[previousSelectedIndex]);
		}
		if (selectedIndex < 0 || selectedIndex >= hotkeyUis.Length)
		{
			return true;
		}
		HotkeyUi selectedHotkey = hotkeyUis[selectedIndex];
		OnSelected(selectedHotkey);
		this.previousSelectedIndex = previousSelectedIndex;
		switch (currentMode)
		{
		case HotkeyMode.Inventory:
		case HotkeyMode.Spectating:
			ApplyToInventory();
			break;
		case HotkeyMode.GolfCart:
			ApplyToGolfCart();
			break;
		}
		return true;
		void ApplyToGolfCart()
		{
			selectedHotkey.HideName();
			if (!uiOnly && !GameManager.LocalPlayerInfo.TrySwitchGolfCartSeatTo(index))
			{
				SelectInternal(previousSelectedIndex, uiOnly: true);
			}
		}
		void ApplyToInventory()
		{
			selectedHotkey.ShowName();
			if (currentMode == HotkeyMode.Spectating)
			{
				uiOnly = true;
				if (previousSelectedIndex > 0)
				{
					UpdatePlayerInventoryHoykeyInternal(HotkeyIndexToInventoryIndexInternal(previousSelectedIndex));
				}
				if (index > 0)
				{
					UpdatePlayerInventoryHoykeyInternal(HotkeyIndexToInventoryIndexInternal(index));
				}
			}
			PlayerInventory playerInventory = ((currentMode != HotkeyMode.Spectating) ? GameManager.LocalPlayerInventory : ((GameManager.LocalPlayerAsSpectator != null && GameManager.LocalPlayerAsSpectator.TargetPlayer != null) ? GameManager.LocalPlayerAsSpectator.TargetPlayer.Inventory : null));
			if (index <= 0)
			{
				if (!uiOnly)
				{
					playerInventory.TryDeselectItem();
				}
				selectedHotkey.SetName(GameManager.UiSettings.GolfClubLocalizedName);
			}
			else
			{
				if (!uiOnly)
				{
					playerInventory.TrySelectItemSlot(HotkeyIndexToInventoryIndexInternal(index));
				}
				ItemType effectivelyEquippedItem = playerInventory.GetEffectivelyEquippedItem(ignoreEquipmentHiding: true);
				if (!GameManager.AllItems.TryGetItemData(effectivelyEquippedItem, out var itemData))
				{
					Debug.LogError($"Could not find data for item {effectivelyEquippedItem}");
					selectedHotkey.HideName();
				}
				else
				{
					selectedHotkey.ShowName();
					selectedHotkey.SetName(itemData.LocalizedName);
				}
			}
		}
		void ApplyToSwingPitch()
		{
			for (int i = 0; i < swingButtonPrompts.Length; i++)
			{
				swingButtonPrompts[i].color = new Color(1f, 1f, 1f, (i == index) ? 1f : 0.5f);
			}
			if (!uiOnly)
			{
				GameManager.LocalPlayerAsGolfer.ApplySwingPitchPreset(index);
			}
		}
		bool CanSelect()
		{
			switch (currentMode)
			{
			case HotkeyMode.Inventory:
				if (GameManager.LocalPlayerInventory == null)
				{
					return false;
				}
				if (index == 0)
				{
					return GameManager.LocalPlayerInventory.CanDeselectItem();
				}
				return GameManager.LocalPlayerInventory.CanSelectItemAt(HotkeyIndexToInventoryIndexInternal(index));
			case HotkeyMode.SwingPitch:
				return index < GameManager.GolfSettings.HotkeySwingPresets.Length;
			default:
				return true;
			}
		}
		void OnDeselected(HotkeyUi hotkey)
		{
			hotkey.HideName();
			hotkey.ResetSize(animate);
		}
		void OnSelected(HotkeyUi hotkey)
		{
			hotkey.Expand(animate);
		}
	}

	private bool DeselectInternal(bool uiOnly)
	{
		return SelectInternal(-1, uiOnly);
	}

	private bool CycleLeftInternal()
	{
		for (int i = 1; i < hotkeyUis.Length; i++)
		{
			if (SelectInternal(BMath.Wrap(selectedIndex - i, hotkeyUis.Length), uiOnly: false))
			{
				return true;
			}
		}
		return false;
	}

	private bool CycleRightInternal()
	{
		for (int i = 1; i < hotkeyUis.Length; i++)
		{
			if (SelectInternal(BMath.Wrap(selectedIndex + i, hotkeyUis.Length), uiOnly: false))
			{
				return true;
			}
		}
		return false;
	}

	private void UpdateInventoryPitchVisibility()
	{
		bool flag = isInventoryPitchVisible;
		isInventoryPitchVisible = ShouldBeVisible();
		if (isInventoryPitchVisible != flag)
		{
			if (inventoryPitchVisibilityRoutine != null)
			{
				StopCoroutine(inventoryPitchVisibilityRoutine);
			}
			if (isInventoryPitchVisible)
			{
				inventoryPitchVisibilityRoutine = StartCoroutine(FadeToRoutine(1f, inventoryPitchFadeInTime, BMath.EaseOut));
			}
			else
			{
				inventoryPitchVisibilityRoutine = StartCoroutine(FadeToRoutine(0f, inventoryPitchFadeOutTime, BMath.EaseIn));
			}
		}
		IEnumerator FadeToRoutine(float targetAlpha, float duration, Func<float, float> Easing)
		{
			float time = 0f;
			float initialAlpha = inventoryPitchGroup.alpha;
			if (initialAlpha != targetAlpha)
			{
				for (; time < duration; time += Time.deltaTime)
				{
					float arg = time / duration;
					inventoryPitchGroup.alpha = BMath.LerpClamped(initialAlpha, targetAlpha, Easing(arg));
					yield return null;
				}
				inventoryPitchGroup.alpha = targetAlpha;
			}
		}
		bool ShouldBeVisible()
		{
			if (currentMode == HotkeyMode.Inventory && selectedIndex == 0)
			{
				return true;
			}
			if (currentMode == HotkeyMode.SwingPitch)
			{
				return true;
			}
			return false;
		}
	}

	private void UpdatePlayerInventoryHoykeyInternal(int inventorySlotIndex)
	{
		if (currentMode != HotkeyMode.Inventory && currentMode != HotkeyMode.GolfCart && currentMode != HotkeyMode.Spectating)
		{
			return;
		}
		int num = InventoryIndexToHotkeyIndexInternal(inventorySlotIndex);
		HotkeyUi hotkeyUi = ((currentMode != HotkeyMode.GolfCart) ? hotkeyUis[num] : hotkeyPreviewUis[inventorySlotIndex]);
		PlayerInventory playerInventory = ((currentMode != HotkeyMode.Spectating) ? GameManager.LocalPlayerInventory : ((GameManager.LocalPlayerAsSpectator != null && GameManager.LocalPlayerAsSpectator.TargetPlayer != null) ? GameManager.LocalPlayerAsSpectator.TargetPlayer.Inventory : null));
		if (playerInventory == null)
		{
			hotkeyUi.SetIcon(null);
			hotkeyUi.SetUses(0, 0);
			hotkeyUi.SetIsGreyedOut(greyedOut: false);
			return;
		}
		if (currentMode == HotkeyMode.Spectating && num != 0)
		{
			int num2 = ((playerInventory.GetEffectivelyEquippedItem() != ItemType.None) ? (playerInventory.PlayerInfo.NetworkedEquippedItemIndex + 1) : 0);
			if (num != num2)
			{
				SetInventoryUnknownHotkeyData(hotkeyUi);
				return;
			}
		}
		SetInventoryHotkeyData(hotkeyUi, playerInventory, inventorySlotIndex);
	}

	private void UpdateOccupiedGolfCartSeatsInternal(GolfCartSeat playerSeat)
	{
		if (!playerSeat.IsValid())
		{
			DeselectInternal(uiOnly: true);
			for (int i = 0; i < GameManager.GolfCartSettings.MaxPassengers; i++)
			{
				hotkeyUis[i].SetIsGreyedOut(greyedOut: false);
			}
			return;
		}
		GolfCartInfo golfCart = playerSeat.golfCart;
		SelectInternal(playerSeat.seat, uiOnly: true);
		for (int j = 0; j < GameManager.GolfCartSettings.MaxPassengers; j++)
		{
			hotkeyUis[j].SetIsGreyedOut(j != playerSeat.seat && !golfCart.IsSeatFreeFor(GameManager.LocalPlayerInfo, j));
		}
	}

	private void ShowHotkeyButtonPrompts()
	{
		HotkeyUi[] array = hotkeyUis;
		for (int i = 0; i < array.Length; i++)
		{
			array[i].ShowButtonPrompt();
		}
		swingButtonPromptsGroup.alpha = 1f;
	}

	private void HideHotkeyButtonPrompts()
	{
		HotkeyUi[] array = hotkeyUis;
		for (int i = 0; i < array.Length; i++)
		{
			array[i].HideButtonPrompt();
		}
		swingButtonPromptsGroup.alpha = 0f;
	}

	private void LocalPlayerEnteredGolfCart(bool fromDriverSeatReservation)
	{
		UpdateCycleButtonPrompts();
	}

	private void UpdateCycleButtonPrompts()
	{
		if ((GameManager.LocalPlayerAsSpectator != null && GameManager.LocalPlayerAsSpectator.IsSpectating) || InputManager.CurrentDeviceType == InputManager.DeviceType.KeyboardAndMouse)
		{
			cycleLeftButtonPrompt.gameObject.SetActive(value: false);
			cycleRightButtonPrompt.gameObject.SetActive(value: false);
			return;
		}
		cycleLeftButtonPrompt.gameObject.SetActive(value: true);
		cycleRightButtonPrompt.gameObject.SetActive(value: true);
		cycleLeftButtonPrompt.sprite = InputManager.GetInputIcon(PlayerInput.Controls.Hotkeys.CycleLeft);
		cycleRightButtonPrompt.sprite = InputManager.GetInputIcon(PlayerInput.Controls.Hotkeys.CycleRight);
	}

	private int HotkeyIndexToInventoryIndexInternal(int hotkeyIndex)
	{
		return hotkeyIndex - 1;
	}

	private int InventoryIndexToHotkeyIndexInternal(int inventoryIndex)
	{
		return inventoryIndex + 1;
	}

	public void OnBUpdate()
	{
		if (currentMode == HotkeyMode.Inventory)
		{
			UpdateInventory();
		}
		swingPitchFill.fillAmount = BMath.Lerp(swingPitchFill.fillAmount, swingPitchFillTarget, 16f * Time.deltaTime);
		swingGroup.alpha = BMath.Lerp(swingGroup.alpha, (currentMode == HotkeyMode.SwingPitch) ? 1 : 0, 24f * Time.deltaTime);
		inventoryGroup.alpha = 1f - swingGroup.alpha;
		static bool ShouldGolfClubBeGreyedOut()
		{
			if (GameManager.LocalPlayerInventory == null)
			{
				return true;
			}
			if (GameManager.LocalPlayerInventory.IsUsingItemAtAll)
			{
				return true;
			}
			return false;
		}
		void UpdateInventory()
		{
			hotkeyUis[0].SetIsGreyedOut(ShouldGolfClubBeGreyedOut());
			for (int i = 0; i < GameManager.PlayerInventorySettings.MaxItems; i++)
			{
				int num = InventoryIndexToHotkeyIndexInternal(i);
				HotkeyUi hotkey = hotkeyUis[num];
				SetInventoryHotkeyData(hotkey, GameManager.LocalPlayerInventory, i);
			}
		}
	}

	private void SetInventoryHotkeyData(HotkeyUi hotkey, PlayerInventory inventory, int inventorySlotIndex)
	{
		bool isLocalPlayerInventory;
		int remainingUses;
		int maxUses;
		if (!(hotkey == null) && !(inventory == null))
		{
			isLocalPlayerInventory = inventory == GameManager.LocalPlayerInventory;
			inventory.GetUsesForSlot(inventorySlotIndex, out remainingUses, out maxUses);
			hotkey.SetIcon(inventory.GetIconForSlot(inventorySlotIndex));
			hotkey.SetUses(remainingUses, maxUses);
			hotkey.SetIsGreyedOut(ShouldGreyOut());
		}
		bool ShouldGreyOut()
		{
			if (!isLocalPlayerInventory)
			{
				return true;
			}
			if (currentMode == HotkeyMode.GolfCart)
			{
				return true;
			}
			if (remainingUses <= 0 && maxUses > 0)
			{
				return true;
			}
			if (inventory.IsUsingItemAtAll)
			{
				return true;
			}
			if (!inventory.PlayerInfo.AsGolfer.CanInterruptSwing())
			{
				return true;
			}
			return false;
		}
	}

	private void SetInventoryUnknownHotkeyData(HotkeyUi hotkey)
	{
		if (!(hotkey == null))
		{
			hotkey.SetIcon(GameManager.UiSettings.UnknownItemIcon);
			hotkey.SetUses(0, 0);
			hotkey.SetIsGreyedOut(greyedOut: false);
		}
	}

	private void OnLocalPlayerIsSpectatingChanged()
	{
		if (GameManager.LocalPlayerAsSpectator.IsSpectating)
		{
			HideHotkeyButtonPrompts();
			UpdateCycleButtonPrompts();
		}
		else
		{
			OnSwitchedInputDeviceType();
		}
	}

	private void OnLocalPlayerSetSpectatingTarget(bool isInitialTarget)
	{
		UpdateGolfClubIcon();
	}

	private void OnSwitchedInputDeviceType()
	{
		if (!(GameManager.LocalPlayerAsSpectator != null) || !GameManager.LocalPlayerAsSpectator.IsSpectating)
		{
			if (InputManager.CurrentDeviceType == InputManager.DeviceType.KeyboardAndMouse)
			{
				ShowHotkeyButtonPrompts();
			}
			else
			{
				HideHotkeyButtonPrompts();
			}
			UpdateCycleButtonPrompts();
		}
	}
}
