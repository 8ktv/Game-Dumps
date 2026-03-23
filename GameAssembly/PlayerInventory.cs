#define DEBUG_DRAW
using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using Brimstone.Geometry;
using Mirror;
using Mirror.RemoteCalls;
using UnityEngine;

public class PlayerInventory : NetworkBehaviour, IBUpdateCallback, IAnyBUpdateCallback
{
	[Flags]
	private enum ThrownItemHand
	{
		None = 0,
		Left = 1,
		Right = 2
	}

	private const float maxAimLocalYaw = 45f;

	public static readonly HashSet<PlayerInfo> processedPlayerBuffer;

	public static readonly HashSet<PlayerInfo> playersToRemoveBuffer;

	public static readonly HashSet<Hittable> processedHittableBuffer;

	public readonly SyncList<InventorySlot> slots = new SyncList<InventorySlot>();

	private readonly Dictionary<int, InventorySlot> localPlayerSlotOverrides = new Dictionary<int, InventorySlot>();

	private Coroutine itemUseRoutine;

	private int latestItemUseIndex = -1;

	private Coroutine flourishRoutine;

	private int springBootsInUseSlotIndex;

	private Coroutine springBootsUseRoutine;

	private Coroutine waitToEnterPlacedGolfCartRoutine;

	private GolfCartInfo reservedGolfCart;

	private ThrownItemHand thrownItem;

	private bool didThrowElephantGun;

	[SyncVar]
	private bool isEquipmentForceHidden;

	private bool isUpdateLoopRunning;

	private readonly HashSet<PlayerInfo> airhornTargets = new HashSet<PlayerInfo>();

	private bool isWaitingForAltUseInputRelease;

	private bool shouldSplitGolfCartBriefcase;

	private PoolableParticleSystem airhornVfx;

	private AntiCheatRateChecker serverAddItemCheatCommandRateLimiter;

	private AntiCheatRateChecker serverSpringBootsActivationCommandRateLimiter;

	private AntiCheatRateChecker serverDropItemCommandRateLimiter;

	private AntiCheatRateChecker serverRemoveItemCommandRateLimiter;

	private AntiCheatRateChecker serverGolfCartBriefcaseEffectsCommandRateLimiter;

	private AntiCheatPerPlayerRateChecker serverSpawnLandmineCommandRateLimiter;

	private AntiCheatRateChecker serverActivateOrbitalLaserCommandRateLimiter;

	private AntiCheatRateChecker serverThrowUsedItemCommandRateLimiter;

	private AntiCheatPerPlayerRateChecker serverInformInterruptedPlayerWithAirhornRateLimiter;

	private AntiCheatRateChecker serverInformShotRocketCommandRateLimiter;

	private AntiCheatRateChecker serverActivateElectromagnetCommandRateLimiter;

	private AntiCheatRateChecker serverActivatedOrbitalLaserCommandRateLimiter;

	private AntiCheatRateChecker serverDecrementItemUseCommandRateLimiter;

	private AntiCheatRateChecker serverAirhornVfxCommandRateLimiter;

	private AntiCheatRateChecker serverCancelAirhornVfxCommandRateLimiter;

	[CVar("drawItemUseDebug", "", "", false, true)]
	private static bool drawItemUseDebug;

	[CVar("drawItemMeleeAttackDebug", "", "", false, true)]
	private static bool drawItemMeleeAttackDebug;

	private bool isEquipmentForceHiddenFromConsole;

	public PlayerInfo PlayerInfo { get; private set; }

	public ItemUseType CurrentItemUse { get; private set; }

	public double ItemUseTimestamp { get; private set; }

	public bool IsFlourishingItem { get; private set; }

	public bool IsUsingSpringBoots { get; private set; }

	public bool IsWaitingToEnterPlacedGolfCart { get; private set; }

	public bool IsAimingItem { get; private set; }

	public LockOnTarget LockOnTarget { get; private set; }

	public int EquippedItemIndex { get; private set; } = -1;

	public bool IsUsingItemAtAll => CurrentItemUse != ItemUseType.None;

	public bool NetworkisEquipmentForceHidden
	{
		get
		{
			return isEquipmentForceHidden;
		}
		[param: In]
		set
		{
			GeneratedSyncVarSetter(value, ref isEquipmentForceHidden, 1uL, null);
		}
	}

	public event Action EquippedItemChanged;

	public event Action<ItemType> ItemUseCancelled;

	public event Action<int> ItemSlotSet;

	public static event Action LocalPlayerIsAimingItemChanged;

	public static event Action LocalPlayerSuccessfullyEnteredReservedGolfCart;

	[CCommand("setEquipmentHidden", "", false, false)]
	public static void SetIsEquipmentForceHiddenFromConsole(bool isHidden)
	{
		if (!(GameManager.LocalPlayerInfo == null))
		{
			GameManager.LocalPlayerInfo.Inventory.isEquipmentForceHiddenFromConsole = isHidden;
			GameManager.LocalPlayerInfo.Inventory.UpdateIsEquipmentForceHidden();
		}
	}

	[CCommand("giveItem", "", false, false)]
	private static void GiveItemToLocalPlayer(ItemType item)
	{
		if (!(GameManager.LocalPlayerInventory == null) && MatchSetupRules.IsCheatsEnabled())
		{
			GameManager.LocalPlayerInventory.CmdAddItem(item);
		}
	}

	[Command]
	private void CmdAddItem(ItemType item)
	{
		if (base.isServer && base.isClient)
		{
			UserCode_CmdAddItem__ItemType(item);
			return;
		}
		NetworkWriterPooled writer = NetworkWriterPool.Get();
		GeneratedNetworkCode._Write_ItemType(writer, item);
		SendCommandInternal("System.Void PlayerInventory::CmdAddItem(ItemType)", 1975538977, writer, 0);
		NetworkWriterPool.Return(writer);
	}

	private void Awake()
	{
		PlayerInfo = GetComponent<PlayerInfo>();
		UpdateIsUpdateLoopRunning();
	}

	public void OnWillBeDestroyed()
	{
		SetLockOnTarget(null);
		foreach (PlayerInfo airhornTarget in airhornTargets)
		{
			LockOnTargetUiManager.RemoveTarget(airhornTarget.AsEntity.AsLockOnTarget);
		}
		if (isUpdateLoopRunning)
		{
			BUpdate.DeregisterCallback(this);
		}
	}

	public override void OnStartServer()
	{
		if (BNetworkManager.singleton.ServerTryGetPlayerGuidFromConnection(base.connectionToClient, out var playerGuid) && ServerPersistentCourseData.TryGetPlayerInventory(playerGuid, out var inventory))
		{
			for (int i = 0; i < inventory.Length; i++)
			{
				slots.Add(inventory[i]);
			}
		}
		for (int j = slots.Count; j < GameManager.PlayerInventorySettings.MaxItems; j++)
		{
			slots.Add(InventorySlot.Empty);
		}
		serverAddItemCheatCommandRateLimiter = new AntiCheatRateChecker("Add item cheat", base.connectionToClient.connectionId, 0.05f, 4, 8, 1f);
		serverSpringBootsActivationCommandRateLimiter = new AntiCheatRateChecker("Spring boots activation", base.connectionToClient.connectionId, 0.5f, 5, 10, 2f);
		serverDropItemCommandRateLimiter = new AntiCheatRateChecker("Drop item", base.connectionToClient.connectionId, 0.05f, 5, 10, 1f);
		serverRemoveItemCommandRateLimiter = new AntiCheatRateChecker("Remove item", base.connectionToClient.connectionId, 0.05f, 5, 10, 1f);
		serverGolfCartBriefcaseEffectsCommandRateLimiter = new AntiCheatRateChecker("Golf cart briefcase effects", base.connectionToClient.connectionId, 0.5f, 5, 10, 2f);
		serverSpawnLandmineCommandRateLimiter = new AntiCheatPerPlayerRateChecker(base.name + " spawn landmine", 0.25f, 5, 10, 1f);
		serverActivateOrbitalLaserCommandRateLimiter = new AntiCheatRateChecker("Activate orbital laser", base.connectionToClient.connectionId, 0.5f, 5, 10, 2f);
		serverThrowUsedItemCommandRateLimiter = new AntiCheatRateChecker("Throw used item", base.connectionToClient.connectionId, 0.5f, 5, 10, 2f);
		serverInformInterruptedPlayerWithAirhornRateLimiter = new AntiCheatPerPlayerRateChecker(base.name + " interrupted player with airhorn", 0.5f, 5, 10, 2f);
		serverInformShotRocketCommandRateLimiter = new AntiCheatRateChecker("Shot rocket", base.connectionToClient.connectionId, 0.5f, 5, 10, 2f);
		serverActivateElectromagnetCommandRateLimiter = new AntiCheatRateChecker("Activate electromagnet", base.connectionToClient.connectionId, 0.5f, 5, 10, 2f);
		serverActivatedOrbitalLaserCommandRateLimiter = new AntiCheatRateChecker("Activated orbital laser", base.connectionToClient.connectionId, 0.5f, 5, 10, 2f);
		serverDecrementItemUseCommandRateLimiter = new AntiCheatRateChecker("Decrement item use", base.connectionToClient.connectionId, 0.1f, 5, 10, 1f);
		serverAirhornVfxCommandRateLimiter = new AntiCheatRateChecker("Airhorn VFX", base.connectionToClient.connectionId, 0.5f, 5, 10, 2f);
		serverCancelAirhornVfxCommandRateLimiter = new AntiCheatRateChecker("Cancel airhorn VFX", base.connectionToClient.connectionId, 0.5f, 5, 10, 2f);
	}

	public override void OnStartClient()
	{
		SyncList<InventorySlot> syncList = slots;
		syncList.OnChange = (Action<SyncList<InventorySlot>.Operation, int, InventorySlot>)Delegate.Combine(syncList.OnChange, new Action<SyncList<InventorySlot>.Operation, int, InventorySlot>(OnItemSlotsChanged));
		if (base.isLocalPlayer)
		{
			TryDeselectItem();
		}
	}

	public override void OnStopClient()
	{
		if (!BNetworkManager.IsChangingSceneOrShuttingDown)
		{
			SyncList<InventorySlot> syncList = slots;
			syncList.OnChange = (Action<SyncList<InventorySlot>.Operation, int, InventorySlot>)Delegate.Remove(syncList.OnChange, new Action<SyncList<InventorySlot>.Operation, int, InventorySlot>(OnItemSlotsChanged));
		}
	}

	public override void OnStartLocalPlayer()
	{
		for (int i = 0; i < slots.Count; i++)
		{
			Hotkeys.UpdatePlayerInventoryIcon(i);
		}
		UpdateEquipmentSwitchers();
		PlayerInfo.Movement.IsVisibleChanged += OnLocalPlayerIsVisibleChanged;
		PlayerInfo.Movement.IsKnockedOutOrRecoveringChanged += OnLocalPlayerIsKnockedOutOrRecoveringChanged;
		PlayerInfo.Movement.IsRespawningChanged += OnLocalPlayerIsRespawningChanged;
		PlayerInfo.AsGolfer.MatchResolutionChanged += OnLocalPlayerMatchResolutionChanged;
		PlayerInfo.AsSpectator.IsSpectatingChanged += OnLocalPlayerIsSpectatingChanged;
	}

	public override void OnStopLocalPlayer()
	{
		if (!BNetworkManager.IsChangingSceneOrShuttingDown)
		{
			PlayerInfo.Movement.IsVisibleChanged -= OnLocalPlayerIsVisibleChanged;
			PlayerInfo.Movement.IsKnockedOutOrRecoveringChanged -= OnLocalPlayerIsKnockedOutOrRecoveringChanged;
			PlayerInfo.Movement.IsRespawningChanged -= OnLocalPlayerIsRespawningChanged;
			PlayerInfo.AsGolfer.MatchResolutionChanged -= OnLocalPlayerMatchResolutionChanged;
			PlayerInfo.AsSpectator.IsSpectatingChanged -= OnLocalPlayerIsSpectatingChanged;
		}
	}

	public void OnBUpdate()
	{
		InventorySlot effectiveSlot = GetEffectiveSlot(EquippedItemIndex);
		switch (effectiveSlot.itemType)
		{
		case ItemType.Airhorn:
			UpdateAirhornTargetIndicators();
			break;
		case ItemType.ElephantGun:
			UpdateElephantGunThrow();
			break;
		case ItemType.RocketLauncher:
			if (IsAimingItem)
			{
				UpdateRocketLauncherLockOnTarget();
			}
			break;
		case ItemType.OrbitalLaser:
			UpdateOrbitalLaserLockOnTarget();
			break;
		default:
			SetLockOnTarget(null);
			break;
		}
		void UpdateAirhornTargetIndicators()
		{
			if (IsUsingItemAtAll)
			{
				SetLockOnTarget(null);
				return;
			}
			int num = Physics.OverlapSphereNonAlloc(base.transform.position, GameManager.ItemSettings.AirhornRange, layerMask: GameManager.LayerSettings.PlayersMask, results: PlayerGolfer.overlappingColliderBuffer, queryTriggerInteraction: QueryTriggerInteraction.Ignore);
			processedPlayerBuffer.Clear();
			for (int i = 0; i < num; i++)
			{
				if (PlayerGolfer.overlappingColliderBuffer[i].TryGetComponentInParent<PlayerInfo>(out var foundComponent, includeInactive: true) && !(foundComponent == PlayerInfo))
				{
					processedPlayerBuffer.Add(foundComponent);
				}
			}
			playersToRemoveBuffer.Clear();
			foreach (PlayerInfo airhornTarget in airhornTargets)
			{
				if (!processedPlayerBuffer.Contains(airhornTarget))
				{
					LockOnTargetUiManager.RemoveTarget(airhornTarget.AsEntity.AsLockOnTarget);
					playersToRemoveBuffer.Add(airhornTarget);
				}
			}
			foreach (PlayerInfo item in processedPlayerBuffer)
			{
				if (!airhornTargets.Contains(item))
				{
					LockOnTargetUiManager.AddTarget(item.AsEntity.AsLockOnTarget);
					airhornTargets.Add(item);
				}
			}
			foreach (PlayerInfo item2 in playersToRemoveBuffer)
			{
				airhornTargets.Remove(item2);
			}
		}
		void UpdateElephantGunThrow()
		{
			if (!didThrowElephantGun && effectiveSlot.remainingUses <= 0 && PlayerInfo.Movement.DivingState == DivingState.GettingUp && !(BMath.GetTimeSince(PlayerInfo.Movement.DivingStateTimestamp) < GameManager.ItemSettings.ElephantGunGetUpThrowTime))
			{
				ThrowUsedItemForAllClients(ThrownUsedItemType.ElephantGun);
				MarkThrownItem(ThrownItemHand.Right);
				didThrowElephantGun = true;
			}
		}
		void UpdateOrbitalLaserLockOnTarget()
		{
			if (IsUsingItemAtAll)
			{
				SetLockOnTarget(null);
			}
			else
			{
				Vector3 fallbackPosition;
				Hittable target = OrbitalLaserManager.GetTarget(out fallbackPosition);
				if (target == null)
				{
					SetLockOnTarget(null);
				}
				else
				{
					SetLockOnTarget(target.AsEntity.AsLockOnTarget);
				}
			}
		}
		void UpdateRocketLauncherLockOnTarget()
		{
			LockOnTarget bestLockOnTarget;
			if (!IsAimingItem)
			{
				SetLockOnTarget(null);
			}
			else if (PlayerInfo.AsGolfer.TryGetBestLockOnTarget(GameManager.ItemSettings.RocketLauncherLockOnMaxDistanceSquared, GameManager.ItemSettings.RocketLauncherLockOnMaxYawFromCenterScreen, GameManager.ItemSettings.RocketLauncherLockOnYawWeight, out bestLockOnTarget))
			{
				SetLockOnTarget(bestLockOnTarget);
			}
			else
			{
				SetLockOnTarget(null);
			}
		}
	}

	public ItemType GetEffectivelyEquippedItem(bool ignoreEquipmentHiding = false)
	{
		if (isEquipmentForceHidden && !ignoreEquipmentHiding)
		{
			return ItemType.None;
		}
		int num = (base.isLocalPlayer ? EquippedItemIndex : PlayerInfo.NetworkedEquippedItemIndex);
		if (num < 0)
		{
			return ItemType.None;
		}
		return GetEffectiveSlot(num).itemType;
	}

	public Sprite GetIconForSlot(int index)
	{
		ItemType itemType = GetEffectiveSlot(index).itemType;
		return GameManager.AllItems.GetItemIcon(itemType);
	}

	public void GetUsesForSlot(int index, out int remainingUses, out int maxUses)
	{
		InventorySlot effectiveSlot = GetEffectiveSlot(index);
		if (effectiveSlot.itemType == ItemType.None)
		{
			remainingUses = 0;
			maxUses = 0;
			return;
		}
		remainingUses = effectiveSlot.remainingUses;
		if (!GameManager.AllItems.TryGetItemData(effectiveSlot.itemType, out var itemData))
		{
			Debug.LogError($"Could not find data for item {effectiveSlot.itemType}");
			maxUses = 0;
		}
		else
		{
			maxUses = itemData.MaxUses;
		}
	}

	public bool IsAimingItemNetworked()
	{
		if (!base.isLocalPlayer)
		{
			return PlayerInfo.NetworkedIsAimingItem;
		}
		return IsAimingItem;
	}

	public bool HasSpaceForItem(out int firstClearIndex)
	{
		for (int i = 0; i < slots.Count; i++)
		{
			if (GetEffectiveSlot(i).itemType == ItemType.None)
			{
				firstClearIndex = i;
				return true;
			}
		}
		firstClearIndex = -1;
		return false;
	}

	public bool IsItemSlotOccupied(int index)
	{
		return GetEffectiveSlot(index).itemType != ItemType.None;
	}

	public bool TrySelectItemSlot(int index)
	{
		if (!base.isLocalPlayer)
		{
			Debug.LogError("Only the local player is allowed to select items in their inventory", base.gameObject);
			return false;
		}
		if (!CanSelectItemAt(index))
		{
			return false;
		}
		ItemType effectivelyEquippedItem = GetEffectivelyEquippedItem();
		ItemType itemType = GetEffectiveSlot(index).itemType;
		EquippedItemIndex = index;
		if (itemType == ItemType.Landmine && PlayerInfo.Input.IsHoldingAimSwing)
		{
			isWaitingForAltUseInputRelease = true;
		}
		thrownItem = ThrownItemHand.None;
		shouldSplitGolfCartBriefcase = false;
		SetLockOnTarget(null);
		ClearAirhornTargets();
		UpdateEquipmentSwitchers();
		UpdateIsAimingItem();
		UpdateAimingReticle();
		UpdateIsUpdateLoopRunning();
		PlayerInfo.Cosmetics.UpdateSpringBootsEnabled();
		PlayerInfo.SetEquippedItemIndex(EquippedItemIndex);
		PlayerInfo.SetEquippedItem(itemType);
		PlayerInfo.CancelEmote(canHideEmoteMenu: false);
		CancelItemFlourish();
		PlayerInfo.AnimatorIo.SetEquippedItem(itemType);
		PlayerInfo.AsGolfer.CancelAllActions();
		if (itemType != effectivelyEquippedItem)
		{
			TutorialManager.CompletePrompt(TutorialPrompt.SelectItem);
			this.EquippedItemChanged?.Invoke();
		}
		return true;
	}

	public bool CanSelectItemAtAll()
	{
		if (IsUsingItemAtAll)
		{
			return false;
		}
		if (PlayerInfo.AsGolfer.IsMatchResolved)
		{
			return false;
		}
		if (PlayerInfo.AsSpectator.IsSpectating)
		{
			return false;
		}
		if (!PlayerInfo.AsGolfer.CanInterruptSwing())
		{
			return false;
		}
		return true;
	}

	public bool CanDeselectItem()
	{
		if (IsUsingItemAtAll)
		{
			return false;
		}
		if (PlayerInfo.AsGolfer.IsMatchResolved && PlayerInfo.AsGolfer.MatchResolution != PlayerMatchResolution.Eliminated)
		{
			return false;
		}
		if (PlayerInfo.AsSpectator.IsSpectating)
		{
			return false;
		}
		if (!PlayerInfo.AsGolfer.CanInterruptSwing())
		{
			return false;
		}
		return true;
	}

	public bool CanSelectItemAt(int index)
	{
		if (!CanSelectItemAtAll())
		{
			return false;
		}
		if (!IsItemSlotOccupied(index))
		{
			return false;
		}
		return true;
	}

	public bool TryDeselectItem()
	{
		if (!base.isLocalPlayer)
		{
			Debug.LogError("Only the local player is allowed to deselect items in their inventory", base.gameObject);
			return false;
		}
		if (!CanDeselectItem())
		{
			return false;
		}
		ItemType effectivelyEquippedItem = GetEffectivelyEquippedItem();
		EquippedItemIndex = -1;
		isWaitingForAltUseInputRelease = false;
		thrownItem = ThrownItemHand.None;
		shouldSplitGolfCartBriefcase = false;
		SetLockOnTarget(null);
		ClearAirhornTargets();
		UpdateEquipmentSwitchers();
		UpdateIsAimingItem();
		UpdateAimingReticle();
		UpdateIsUpdateLoopRunning();
		PlayerInfo.Cosmetics.UpdateSpringBootsEnabled();
		PlayerInfo.CancelEmote(canHideEmoteMenu: false);
		CancelItemFlourish();
		PlayerInfo.SetEquippedItemIndex(EquippedItemIndex);
		PlayerInfo.SetEquippedItem(ItemType.None);
		PlayerInfo.AnimatorIo.SetEquippedItem(ItemType.None);
		PlayerInfo.AsGolfer.CancelAllActions();
		if (Hotkeys.CurrentMode == HotkeyMode.Inventory)
		{
			Hotkeys.Select(0, uiOnly: true);
		}
		if (effectivelyEquippedItem != ItemType.None)
		{
			this.EquippedItemChanged?.Invoke();
		}
		return true;
	}

	public void DropItem()
	{
		if (!base.isLocalPlayer)
		{
			Debug.LogError("Only the local player is allowed to drop items from their inventory", base.gameObject);
			return;
		}
		int equippedItemIndex = EquippedItemIndex;
		InventorySlot droppedItemSlot = GetEffectiveSlot(equippedItemIndex);
		if (CanDropItem())
		{
			ItemUseId itemUseId = IncrementAndGetCurrentItemUseId(droppedItemSlot.itemType);
			if (!base.isServer)
			{
				RemoveItemAt(equippedItemIndex, localOnly: true);
			}
			CmdDropItemAt(equippedItemIndex, PlayerInfo.Rigidbody.linearVelocity, PlayerInfo.Rigidbody.angularVelocity, itemUseId);
			TutorialManager.CompletePrompt(TutorialPrompt.DropItem);
		}
		bool CanDropItem()
		{
			if (!SingletonBehaviour<DrivingRangeManager>.HasInstance && CourseManager.MatchState <= MatchState.TeeOff)
			{
				return false;
			}
			if (droppedItemSlot.itemType == ItemType.None)
			{
				return false;
			}
			if (droppedItemSlot.remainingUses <= 0)
			{
				return false;
			}
			if (IsUsingItemAtAll)
			{
				return false;
			}
			if (PlayerInfo.AsGolfer.IsMatchResolved)
			{
				return false;
			}
			if (PlayerInfo.AsSpectator.IsSpectating)
			{
				return false;
			}
			if (PlayerInfo.Movement.IsRespawning)
			{
				return false;
			}
			if (localPlayerSlotOverrides.TryGetValue(EquippedItemIndex, out var value) && value.itemType == ItemType.None)
			{
				return false;
			}
			return true;
		}
	}

	public bool TryUseItem(bool isAirhornReaction, out bool shouldEatInput)
	{
		shouldEatInput = true;
		if (!base.isLocalPlayer)
		{
			Debug.LogError("Only the local player is allowed to use items from their inventory", base.gameObject);
			return false;
		}
		if (!CanUseEquippedItem(altUse: false, isAirhornReaction, out var equippedSlot, out var equippedItemData, out shouldEatInput, out var isFlourish))
		{
			return false;
		}
		if (isFlourish)
		{
			FlourishItem();
			return true;
		}
		ItemUseTimestamp = Time.timeAsDouble;
		if (equippedSlot.itemType == ItemType.SpringBoots)
		{
			PlayerInfo.Movement.TryTriggerJump();
		}
		else
		{
			CancelItemUse();
			itemUseRoutine = StartCoroutine(GetItemUseRoutine());
		}
		PlayerInfo.CancelEmote(canHideEmoteMenu: false);
		CancelItemFlourish();
		return true;
		void FlourishItem()
		{
			CancelItemFlourish();
			flourishRoutine = StartCoroutine(FlourishRoutine(equippedItemData.FlourishDuration));
		}
		IEnumerator FlourishRoutine(float duration)
		{
			IsFlourishingItem = true;
			PlayerInfo.CancelEmote(canHideEmoteMenu: true);
			PlayerInfo.AnimatorIo.SetIsFlourishingItem(IsFlourishingItem);
			yield return new WaitForSeconds(duration);
			IsFlourishingItem = false;
			PlayerInfo.AnimatorIo.SetIsFlourishingItem(IsFlourishingItem);
		}
		IEnumerator GetItemUseRoutine()
		{
			ItemType itemType = equippedSlot.itemType;
			switch (itemType)
			{
			case ItemType.Coffee:
				return DrinkCoffeeRoutine();
			case ItemType.DuelingPistol:
				return ShootDuelingPistolRoutine();
			case ItemType.ElephantGun:
				return ShootElephantGunRoutine();
			case ItemType.RocketLauncher:
				return ShootRocketLauncherRoutine();
			case ItemType.Airhorn:
				return BlowAirhornRoutine();
			case ItemType.GolfCart:
				return PlaceGolfCartRoutine();
			case ItemType.Landmine:
				return (isAirhornReaction || PlayerInfo.Input.IsHoldingAimSwing) ? TossLandmineRoutine() : PlantLandmineRoutine();
			case ItemType.Electromagnet:
				return ActivateElectromagnetRoutine();
			case ItemType.OrbitalLaser:
				return ActivateOrbitalLaserRoutine();
			default:
			{
				global::_003CPrivateImplementationDetails_003E.ThrowSwitchExpressionException(itemType);
				IEnumerator result = default(IEnumerator);
				return result;
			}
			}
		}
	}

	private bool CanUseEquippedItem(bool altUse, bool isAirhornReaction, out InventorySlot equippedSlot, out ItemData equippedItemData, out bool shouldEatInput, out bool isFlourish)
	{
		equippedSlot = default(InventorySlot);
		equippedItemData = null;
		shouldEatInput = true;
		isFlourish = false;
		if (!SingletonBehaviour<DrivingRangeManager>.HasInstance && CourseManager.MatchState <= MatchState.TeeOff)
		{
			return false;
		}
		if (RadialMenu.IsVisible)
		{
			return false;
		}
		equippedSlot = GetEffectiveSlot(EquippedItemIndex);
		if (equippedSlot.itemType == ItemType.None)
		{
			return false;
		}
		if (equippedSlot.remainingUses <= 0)
		{
			return false;
		}
		if (PlayerInfo.AsGolfer.IsMatchResolved)
		{
			return false;
		}
		if (PlayerInfo.AsSpectator.IsSpectating)
		{
			return false;
		}
		if (PlayerInfo.Movement.IsRespawning)
		{
			return false;
		}
		if (!GameManager.AllItems.TryGetItemData(equippedSlot.itemType, out equippedItemData))
		{
			Debug.LogError($"Could not find data for item {equippedSlot.itemType}");
			return false;
		}
		if (!altUse && !IsAimingItem && equippedItemData.NonAimUse == ItemNonAimingUse.None)
		{
			shouldEatInput = PlayerInfo.Input.IsHoldingAimSwing;
			return false;
		}
		shouldEatInput = false;
		isFlourish = !altUse && !isAirhornReaction && !IsAimingItem && equippedItemData.NonAimUse == ItemNonAimingUse.Flourish;
		if (isFlourish && IsFlourishingItem)
		{
			return false;
		}
		if (IsUsingItemAtAll && (isFlourish || equippedSlot.itemType != ItemType.ElephantGun || BMath.GetTimeSince(ItemUseTimestamp) < GameManager.ItemSettings.ElephantGunShotCooldown))
		{
			return false;
		}
		if (PlayerInfo.Movement.IsKnockedOutOrRecovering)
		{
			return false;
		}
		if (PlayerInfo.Movement.DivingState != DivingState.None && (isFlourish || equippedSlot.itemType != ItemType.ElephantGun))
		{
			return false;
		}
		if (equippedSlot.itemType == ItemType.Landmine && !IsAimingItem && !PlayerInfo.Movement.IsGrounded)
		{
			return false;
		}
		return true;
	}

	public bool TryUseSpringBoots()
	{
		if (!CanUseSpringBoots())
		{
			return false;
		}
		springBootsUseRoutine = StartCoroutine(UseSpringBootsRoutine());
		return true;
		bool CanUseSpringBoots()
		{
			if (!SingletonBehaviour<DrivingRangeManager>.HasInstance && CourseManager.MatchState <= MatchState.TeeOff)
			{
				return false;
			}
			InventorySlot effectiveSlot = GetEffectiveSlot(EquippedItemIndex);
			if (effectiveSlot.itemType != ItemType.SpringBoots)
			{
				return false;
			}
			if (effectiveSlot.remainingUses <= 0)
			{
				return false;
			}
			if (IsUsingSpringBoots)
			{
				return false;
			}
			if (PlayerInfo.AsGolfer.IsMatchResolved)
			{
				return false;
			}
			if (PlayerInfo.AsSpectator.IsSpectating)
			{
				return false;
			}
			if (PlayerInfo.Movement.IsRespawning)
			{
				return false;
			}
			return true;
		}
		void PlaySpringBootsActivationForAllClients(Vector3 position)
		{
			PlaySpringBootsActivationInternal(position);
			CmdPlaySpringBootsActivationForAllClients(position);
		}
		IEnumerator UseSpringBootsRoutine()
		{
			IsUsingSpringBoots = true;
			springBootsInUseSlotIndex = EquippedItemIndex;
			PlayerInfo.Cosmetics.UpdateSpringBootsEnabled();
			PlaySpringBootsActivationForAllClients(base.transform.position);
			DecrementUseFromSlotAt(springBootsInUseSlotIndex);
			yield return new WaitForSeconds(GameManager.ItemSettings.SpringBootsUseDuration);
			yield return new WaitWhile(() => PlayerInfo.Movement.IsInSpringBootsJump);
			IsUsingSpringBoots = false;
			RemoveIfOutOfUses(springBootsInUseSlotIndex);
			PlayerInfo.Cosmetics.UpdateSpringBootsEnabled();
		}
	}

	[Command]
	private void CmdPlaySpringBootsActivationForAllClients(Vector3 position, NetworkConnectionToClient sender = null)
	{
		if (base.isServer && base.isClient)
		{
			UserCode_CmdPlaySpringBootsActivationForAllClients__Vector3__NetworkConnectionToClient(position, sender);
			return;
		}
		NetworkWriterPooled writer = NetworkWriterPool.Get();
		writer.WriteVector3(position);
		SendCommandInternal("System.Void PlayerInventory::CmdPlaySpringBootsActivationForAllClients(UnityEngine.Vector3,Mirror.NetworkConnectionToClient)", -1349002242, writer, 0);
		NetworkWriterPool.Return(writer);
	}

	[TargetRpc]
	private void RpcPlaySpringBootsActivation(NetworkConnectionToClient connection, Vector3 position)
	{
		NetworkWriterPooled writer = NetworkWriterPool.Get();
		writer.WriteVector3(position);
		SendTargetRPCInternal(connection, "System.Void PlayerInventory::RpcPlaySpringBootsActivation(Mirror.NetworkConnectionToClient,UnityEngine.Vector3)", -1062924843, writer, 0);
		NetworkWriterPool.Return(writer);
	}

	private void PlaySpringBootsActivationInternal(Vector3 position)
	{
		if (VfxPersistentData.TryGetPooledVfx(VfxType.SpringBootsSpring, out var particleSystem))
		{
			particleSystem.transform.SetParent(PlayerInfo.LeftToeBone);
			particleSystem.transform.SetLocalPositionAndRotation(Vector3.zero, Quaternion.identity);
			particleSystem.Play();
		}
		if (VfxPersistentData.TryGetPooledVfx(VfxType.SpringBootsSpring, out particleSystem))
		{
			particleSystem.transform.SetParent(PlayerInfo.RightToeBone);
			particleSystem.transform.SetLocalPositionAndRotation(Vector3.zero, Quaternion.Euler(0f, 0f, 180f));
			particleSystem.Play();
		}
		VfxManager.PlayPooledVfxLocalOnly(VfxType.SpringBootsLaunch, position, base.transform.rotation);
		PlayerInfo.PlayerAudio.PlaySpringBootsActivationLocalOnly();
	}

	[Server]
	public bool ServerTryAddItem(ItemType itemToAdd, int remainingUses)
	{
		if (!NetworkServer.active)
		{
			Debug.LogWarning("[Server] function 'System.Boolean PlayerInventory::ServerTryAddItem(ItemType,System.Int32)' called when server was not active");
			return default(bool);
		}
		if (itemToAdd == ItemType.None)
		{
			Debug.LogError($"Attempted to add {ItemType.None} to player's {base.name} inventory", base.gameObject);
			return false;
		}
		if (!HasSpaceForItem(out var firstClearIndex))
		{
			Debug.LogError("Attempted to add an item for player " + base.name + ", but they have no space", base.gameObject);
			return false;
		}
		if (!GameManager.AllItems.TryGetItemData(itemToAdd, out var itemData))
		{
			Debug.LogError($"Could not find data for item {itemToAdd}");
			return false;
		}
		if (remainingUses <= 0 && itemData.MaxUses > 0)
		{
			Debug.LogError($"Attempted to add an {itemToAdd} for player {base.name}, but it has no uses left", base.gameObject);
			return false;
		}
		slots[firstClearIndex] = new InventorySlot(itemToAdd, remainingUses);
		return true;
	}

	public bool TrySetReservedGolfCart(GolfCartInfo golfCart)
	{
		if (!base.isLocalPlayer)
		{
			Debug.LogError("Only the local player is allowed to set their reserved golf cart", base.gameObject);
			return false;
		}
		if (!IsWaitingToEnterPlacedGolfCart)
		{
			return false;
		}
		reservedGolfCart = golfCart;
		return true;
	}

	public void InformLocalPlayerGroundedChanged()
	{
		if (!PlayerInfo.Movement.IsGrounded && CurrentItemUse == ItemUseType.Regular && GetEffectivelyEquippedItem() == ItemType.Landmine)
		{
			CancelItemUse();
		}
	}

	public void InformLocalPlayerIsHoldingAimChanged()
	{
		UpdateIsAimingItem();
	}

	public void InformLocalPlayerIsRespawningChanged()
	{
		UpdateIsAimingItem();
	}

	public void InformLocalPlayerDivingStateChanged()
	{
		UpdateIsEquipmentForceHidden();
		UpdateIsUpdateLoopRunning();
	}

	public void InformIsPlayingEmoteChanged()
	{
		UpdateEquipmentSwitchers();
	}

	public void InformOfSpringBootsLanding(Vector3 worldPosition)
	{
		ThrowUsedItemInternal(ThrownUsedItemType.SpringBootLeft, forcePlayerPosition: true, worldPosition);
		ThrowUsedItemInternal(ThrownUsedItemType.SpringBootRight, forcePlayerPosition: true, worldPosition);
	}

	public bool TryReactToBlownAirhorn()
	{
		if (!base.isLocalPlayer)
		{
			Debug.LogError("Only the local player is allowed to react to an airhorn", base.gameObject);
			return false;
		}
		ItemType effectivelyEquippedItem = GetEffectivelyEquippedItem();
		if (effectivelyEquippedItem == ItemType.None)
		{
			return false;
		}
		if (!GameManager.AllItems.TryGetItemData(effectivelyEquippedItem, out var itemData))
		{
			Debug.LogError($"Could not find data for item {effectivelyEquippedItem}");
			return false;
		}
		if (itemData.AirhornReaction == ItemAirhornReaction.None)
		{
			return false;
		}
		if (itemData.AirhornReaction == ItemAirhornReaction.UsedIfAimed && !IsAimingItem)
		{
			return false;
		}
		bool shouldEatInput;
		return TryUseItem(isAirhornReaction: true, out shouldEatInput);
	}

	public void InformEnteredGolfCart()
	{
		if (base.isLocalPlayer)
		{
			UpdateIsEquipmentForceHidden();
			CancelSpringBootsUse();
			CancelEnterPlacedGolfCartRoutine();
		}
	}

	public void InformExitedGolfCart()
	{
		if (base.isLocalPlayer)
		{
			UpdateIsEquipmentForceHidden();
		}
	}

	public Vector3 GetDuelingPistolBarrelEndPosition()
	{
		return PlayerInfo.LeftHandEquipmentSwitcher.transform.TransformPoint(GameManager.ItemSettings.DuelingPistolLocalBarrelEnd);
	}

	public Quaternion GetDuelingPistolBarrelEndRotation()
	{
		return PlayerInfo.LeftHandEquipmentSwitcher.transform.rotation;
	}

	public Vector3 GetElephantGunBarrelEndPosition()
	{
		return PlayerInfo.RightHandEquipmentSwitcher.transform.TransformPoint(GameManager.ItemSettings.ElephantGunLocalBarrelEnd);
	}

	public Quaternion GetElephantGunBarrelEndRotation()
	{
		return PlayerInfo.RightHandEquipmentSwitcher.transform.rotation;
	}

	public Vector3 GetRocketLauncherRocketPosition()
	{
		return PlayerInfo.RightHandEquipmentSwitcher.transform.TransformPoint(GameManager.ItemSettings.RocketLauncherLocalRocketPosition);
	}

	public Vector3 GetRocketLauncherBarrelFrontEndPosition()
	{
		return PlayerInfo.RightHandEquipmentSwitcher.transform.TransformPoint(GameManager.ItemSettings.RocketLauncherLocalBarrelFrontEnd);
	}

	public Vector3 GetRocketLauncherBarrelBackEndPosition()
	{
		return PlayerInfo.RightHandEquipmentSwitcher.transform.TransformPoint(GameManager.ItemSettings.RocketLauncherLocalBarrelBackEnd);
	}

	public Quaternion GetRocketLauncherRocketRotation()
	{
		return PlayerInfo.RightHandEquipmentSwitcher.transform.rotation * GameManager.ItemSettings.RocketLauncherLocalRocketRotation;
	}

	[Command]
	private void CmdDropItemAt(int index, Vector3 playerVelocity, Vector3 playerLocalAngularVelocity, ItemUseId itemUseId)
	{
		if (base.isServer && base.isClient)
		{
			UserCode_CmdDropItemAt__Int32__Vector3__Vector3__ItemUseId(index, playerVelocity, playerLocalAngularVelocity, itemUseId);
			return;
		}
		NetworkWriterPooled writer = NetworkWriterPool.Get();
		writer.WriteVarInt(index);
		writer.WriteVector3(playerVelocity);
		writer.WriteVector3(playerLocalAngularVelocity);
		GeneratedNetworkCode._Write_ItemUseId(writer, itemUseId);
		SendCommandInternal("System.Void PlayerInventory::CmdDropItemAt(System.Int32,UnityEngine.Vector3,UnityEngine.Vector3,ItemUseId)", 401699825, writer, 0);
		NetworkWriterPool.Return(writer);
	}

	private void RemoveItemAt(int index, bool localOnly)
	{
		if (!base.isServer && !base.isLocalPlayer)
		{
			Debug.LogError("Only the server and the local player are allowed to remove items from an inventory", base.gameObject);
		}
		else if (index < 0 || index >= slots.Count)
		{
			Debug.LogError($"Attempted to remove item at index {index}, but it's out of range of {slots.Count} slots", base.gameObject);
		}
		else
		{
			if (localOnly && !base.isLocalPlayer)
			{
				return;
			}
			if (base.isLocalPlayer && index == EquippedItemIndex)
			{
				TryDeselectItem();
			}
			if (base.isServer)
			{
				slots[index] = InventorySlot.Empty;
				return;
			}
			localPlayerSlotOverrides[index] = InventorySlot.Empty;
			if (!localOnly)
			{
				CmdRemoveItemAt(index);
			}
			Hotkeys.UpdatePlayerInventoryIcon(index);
		}
	}

	[Command]
	private void CmdRemoveItemAt(int index)
	{
		if (base.isServer && base.isClient)
		{
			UserCode_CmdRemoveItemAt__Int32(index);
			return;
		}
		NetworkWriterPooled writer = NetworkWriterPool.Get();
		writer.WriteVarInt(index);
		SendCommandInternal("System.Void PlayerInventory::CmdRemoveItemAt(System.Int32)", 2033685589, writer, 0);
		NetworkWriterPool.Return(writer);
	}

	private void SetCurrentItemUse(ItemUseType itemUse)
	{
		ItemUseType currentItemUse = CurrentItemUse;
		CurrentItemUse = itemUse;
		if (CurrentItemUse != currentItemUse)
		{
			PlayerInfo.AnimatorIo.SetItemUseType(CurrentItemUse);
			UpdateIsUpdateLoopRunning();
			if (CurrentItemUse == ItemUseType.None)
			{
				thrownItem = ThrownItemHand.None;
				UpdateEquipmentSwitchers();
			}
		}
	}

	private void CancelItemUse()
	{
		if (IsUsingItemAtAll)
		{
			ItemType effectivelyEquippedItem = GetEffectivelyEquippedItem();
			SetCurrentItemUse(ItemUseType.None);
			PlayerInfo.PlayerAudio.CancelItemUseForAllClients();
			if (itemUseRoutine != null)
			{
				StopCoroutine(itemUseRoutine);
			}
			RemoveIfOutOfUses(EquippedItemIndex);
			if (effectivelyEquippedItem == ItemType.Airhorn)
			{
				CancelAirhornVfxInternal();
				CmdCancelAirhornVfxForAllClients();
			}
			this.ItemUseCancelled?.Invoke(effectivelyEquippedItem);
		}
	}

	public void CancelItemFlourish()
	{
		if (IsFlourishingItem)
		{
			IsFlourishingItem = false;
			PlayerInfo.AnimatorIo.SetIsFlourishingItem(isFlourishing: false);
			if (flourishRoutine != null)
			{
				StopCoroutine(flourishRoutine);
			}
		}
	}

	private void SetLockOnTarget(LockOnTarget target)
	{
		LockOnTarget lockOnTarget = LockOnTarget;
		LockOnTarget = target;
		if (LockOnTarget == lockOnTarget)
		{
			return;
		}
		if (lockOnTarget != null)
		{
			LockOnTargetUiManager.RemoveTarget(lockOnTarget);
			lockOnTarget.AsEntity.WillBeDestroyed -= OnLockOnTargetWillBeDestroyed;
			if (lockOnTarget.AsEntity.IsPlayer)
			{
				lockOnTarget.AsEntity.PlayerInfo.Movement.IsVisibleChanged -= OnLockOnTargetPlayerIsVisibleChanged;
				lockOnTarget.AsEntity.PlayerInfo.AsGolfer.MatchResolutionChanged -= OnLockOnTargetPlayerMatchResolutionChanged;
			}
		}
		if (LockOnTarget != null)
		{
			LockOnTargetUiManager.AddTarget(LockOnTarget);
			LockOnTarget.AsEntity.WillBeDestroyed += OnLockOnTargetWillBeDestroyed;
			if (LockOnTarget.AsEntity.IsPlayer)
			{
				LockOnTarget.AsEntity.PlayerInfo.Movement.IsVisibleChanged += OnLockOnTargetPlayerIsVisibleChanged;
				LockOnTarget.AsEntity.PlayerInfo.AsGolfer.MatchResolutionChanged += OnLockOnTargetPlayerMatchResolutionChanged;
			}
		}
	}

	private void CancelSpringBootsUse()
	{
		if (IsUsingSpringBoots)
		{
			IsUsingSpringBoots = false;
			RemoveIfOutOfUses(springBootsInUseSlotIndex);
			PlayerInfo.Cosmetics.UpdateSpringBootsEnabled();
			if (springBootsUseRoutine != null)
			{
				StopCoroutine(springBootsUseRoutine);
			}
			ThrowUsedItemForAllClients(ThrownUsedItemType.SpringBootLeft);
			ThrowUsedItemForAllClients(ThrownUsedItemType.SpringBootRight);
		}
	}

	private void CancelEnterPlacedGolfCartRoutine()
	{
		if (IsWaitingToEnterPlacedGolfCart)
		{
			CancelItemUse();
			IsWaitingToEnterPlacedGolfCart = false;
			if (waitToEnterPlacedGolfCartRoutine != null)
			{
				StopCoroutine(waitToEnterPlacedGolfCartRoutine);
			}
		}
	}

	private IEnumerator DrinkCoffeeRoutine()
	{
		SetCurrentItemUse(ItemUseType.Regular);
		PlayerInfo.PlayerAudio.PlayItemUseForAllClients(ItemType.Coffee);
		while (BMath.GetTimeSince(ItemUseTimestamp) < GameManager.ItemSettings.CoffeeDrinkEffectStartTime)
		{
			yield return null;
		}
		PlayerInfo.Movement.InformDrankCoffee();
		DecrementUseFromSlotAt(EquippedItemIndex);
		bool didThrowPlate = false;
		bool didThrowMug = false;
		for (float timeSince = BMath.GetTimeSince(ItemUseTimestamp); timeSince < GameManager.ItemSettings.CoffeePostTotalDuration; timeSince = BMath.GetTimeSince(ItemUseTimestamp))
		{
			if (!didThrowPlate && timeSince >= GameManager.ItemSettings.CoffeeThrowPlateTime)
			{
				ThrowUsedItemForAllClients(ThrownUsedItemType.CoffeePlate);
				MarkThrownItem(ThrownItemHand.Left);
				didThrowPlate = true;
			}
			if (!didThrowMug && timeSince >= GameManager.ItemSettings.CoffeeThrowMugTime)
			{
				ThrowUsedItemForAllClients(ThrownUsedItemType.CoffeeMug);
				MarkThrownItem(ThrownItemHand.Right);
				didThrowMug = true;
			}
			yield return null;
		}
		SetCurrentItemUse(ItemUseType.None);
		RemoveIfOutOfUses(EquippedItemIndex);
	}

	private IEnumerator ShootDuelingPistolRoutine()
	{
		SetCurrentItemUse(ItemUseType.Regular);
		Shoot();
		DecrementUseFromSlotAt(EquippedItemIndex);
		PlayerInfo.PlayerAudio.PlayPistolShotForAllClients();
		bool didThrow = false;
		for (float timeSince = BMath.GetTimeSince(ItemUseTimestamp); timeSince < GameManager.ItemSettings.DuelingPistolShotDuration; timeSince = BMath.GetTimeSince(ItemUseTimestamp))
		{
			if (!didThrow && timeSince >= GameManager.ItemSettings.DuelingPistolThrowTime)
			{
				ThrowUsedItemForAllClients(ThrownUsedItemType.DuelingPistol);
				MarkThrownItem(ThrownItemHand.Left);
				didThrow = true;
			}
			yield return null;
		}
		SetCurrentItemUse(ItemUseType.None);
		RemoveIfOutOfUses(EquippedItemIndex);
		void ReflectShotOffElectromagnetShield(Vector3 shieldHitPoint, Vector3 shotDirection, float travelledDistance, PlayerInfo shieldOwner)
		{
			Vector3 normalized = (shieldHitPoint - shieldOwner.ElectromagnetShieldCollider.transform.position).normalized;
			Vector3 vector = Vector3.Reflect(shotDirection, normalized);
			float num = GameManager.ItemSettings.DuelingPistolMaxShotDistance - travelledDistance;
			shieldOwner.PlayElectromagnetShieldHitForAllClients(normalized);
			int raycastHitCount = Physics.RaycastNonAlloc(new Ray(shieldHitPoint, vector), maxDistance: num, layerMask: GameManager.LayerSettings.GunHittablesMask, results: PlayerGolfer.raycastHitBuffer, queryTriggerInteraction: QueryTriggerInteraction.Ignore);
			if (!TryParseFirearmRaycastResults(PlayerGolfer.raycastHitBuffer, raycastHitCount, shieldOwner, out var raycastHit, out var hitHittable))
			{
				VfxManager.PlayDuelingPistolElectromagnetShieldDeflectedMissForAllClients(shieldOwner, normalized, vector, num);
				if (drawItemUseDebug)
				{
					BDebug.DrawLine(shieldHitPoint, shieldHitPoint + vector * num, Color.red, 2f);
				}
			}
			else
			{
				bool flag = hitHittable != null && CanHitWithGunshot(hitHittable, shieldOwner);
				Vector3 vector2 = default(Vector3);
				if (flag)
				{
					vector2 = hitHittable.transform.InverseTransformPoint(raycastHit.point);
					if (raycastHit.collider.gameObject.layer == GameManager.LayerSettings.ElectromagnetShieldLayer)
					{
						ReflectShotOffElectromagnetShield(raycastHit.point, vector, travelledDistance + raycastHit.distance, hitHittable.AsEntity.PlayerInfo);
						VfxManager.PlayDuelingPistolHitForAllClients(this, new VfxManager.GunShotHitVfxData(hitHittable, hitElectromagnetShield: true, vector2, raycastHit.point));
						if (drawItemUseDebug)
						{
							BDebug.DrawLine(shieldHitPoint, raycastHit.point, flag ? Color.blue : Color.yellow, 2f);
						}
						return;
					}
					hitHittable.HitWithItem(ItemType.DuelingPistol, shieldOwner.ElectromagnetShieldItemUseId, vector2, vector, hitHittable.transform.InverseTransformPoint(shieldHitPoint), raycastHit.distance, shieldOwner.Inventory, isReflected: true, isInSpecialState: false, canHitWithNoUser: false);
				}
				VfxManager.PlayDuelingPistolElectromagnetShieldDeflectedHitForAllClients(shieldOwner, normalized, new VfxManager.GunShotHitVfxData(hitHittable, hitElectromagnetShield: false, vector2, raycastHit.point));
				if (drawItemUseDebug)
				{
					BDebug.DrawLine(shieldHitPoint, raycastHit.point, flag ? Color.blue : Color.yellow, 2f);
				}
			}
		}
		void Shoot()
		{
			Vector3 duelingPistolBarrelEndPosition = GetDuelingPistolBarrelEndPosition();
			float localYaw;
			Vector3 firearmAimPoint = GetFirearmAimPoint(GameManager.ItemSettings.DuelingPistolMaxAimingDistance, GameManager.LayerSettings.GunHittablesMask, out localYaw);
			if (BMath.Abs(localYaw) > 45f)
			{
				PlayerInfo.Movement.AlignWithCameraImmediately();
				firearmAimPoint = GetFirearmAimPoint(GameManager.ItemSettings.DuelingPistolMaxAimingDistance, GameManager.LayerSettings.GunHittablesMask, out var _);
			}
			Vector3 direction = (firearmAimPoint - duelingPistolBarrelEndPosition).RandomlyRotatedDeg(GameManager.ItemSettings.DuelingPistolMaxInaccuracyAngle);
			Ray ray = new Ray(duelingPistolBarrelEndPosition, direction);
			int raycastHitCount = Physics.RaycastNonAlloc(ray, maxDistance: GameManager.ItemSettings.DuelingPistolMaxShotDistance, layerMask: GameManager.LayerSettings.GunHittablesMask, results: PlayerGolfer.raycastHitBuffer, queryTriggerInteraction: QueryTriggerInteraction.Ignore);
			if (!TryParseFirearmRaycastResults(PlayerGolfer.raycastHitBuffer, raycastHitCount, null, out var raycastHit, out var hitHittable))
			{
				VfxManager.PlayDuelingPistolMissForAllClients(this, ray.direction);
				if (drawItemUseDebug)
				{
					BDebug.DrawLine(ray.origin, ray.origin + ray.direction * GameManager.ItemSettings.DuelingPistolMaxShotDistance, Color.red, 2f);
				}
			}
			else
			{
				bool flag = hitHittable != null && CanHitWithGunshot(hitHittable, null);
				Vector3 vector = default(Vector3);
				if (flag)
				{
					vector = hitHittable.transform.InverseTransformPoint(raycastHit.point);
					if (hitHittable.AsEntity.IsPlayer && hitHittable.AsEntity.PlayerInfo.IsElectromagnetShieldActive)
					{
						ReflectShotOffElectromagnetShield(raycastHit.point, ray.direction, raycastHit.distance, hitHittable.AsEntity.PlayerInfo);
						VfxManager.PlayDuelingPistolHitForAllClients(this, new VfxManager.GunShotHitVfxData(hitHittable, hitElectromagnetShield: true, vector, raycastHit.point));
						if (drawItemUseDebug)
						{
							BDebug.DrawLine(ray.origin, raycastHit.point, flag ? Color.blue : Color.yellow, 2f);
						}
						return;
					}
					hitHittable.HitWithItem(ItemType.DuelingPistol, IncrementAndGetCurrentItemUseId(ItemType.DuelingPistol), vector, ray.direction, hitHittable.transform.InverseTransformPoint(duelingPistolBarrelEndPosition), raycastHit.distance, this, isReflected: false, isInSpecialState: false, canHitWithNoUser: false);
				}
				VfxManager.PlayDuelingPistolHitForAllClients(this, new VfxManager.GunShotHitVfxData(hitHittable, hitElectromagnetShield: false, vector, raycastHit.point));
				if (drawItemUseDebug)
				{
					BDebug.DrawLine(ray.origin, raycastHit.point, flag ? Color.blue : Color.yellow, 2f);
				}
			}
		}
	}

	private IEnumerator ShootElephantGunRoutine()
	{
		SetCurrentItemUse(ItemUseType.Regular);
		didThrowElephantGun = false;
		Shoot();
		DecrementUseFromSlotAt(EquippedItemIndex);
		UpdateIsAimingItem();
		PlayerInfo.PlayerAudio.PlayElephantGunShotForAllClients();
		float time = 0f;
		bool endedShootAnimation = false;
		while (PlayerInfo.Movement.DivingState != DivingState.None)
		{
			yield return null;
			if (!endedShootAnimation)
			{
				time += Time.deltaTime;
				if (time >= GameManager.ItemSettings.ElephantGunShotDuration)
				{
					UpdateIsAimingItem();
					PlayerInfo.AnimatorIo.SetItemUseType(ItemUseType.None);
					endedShootAnimation = true;
				}
			}
		}
		SetCurrentItemUse(ItemUseType.None);
		RemoveIfOutOfUses(EquippedItemIndex);
		void ReflectShotOffElectromagnetShield(Vector3 shieldHitPoint, Vector3 shotDirection, float travelledDistance, PlayerInfo shieldOwner)
		{
			Vector3 normalized = (shieldHitPoint - shieldOwner.ElectromagnetShieldCollider.transform.position).normalized;
			Vector3 vector = Vector3.Reflect(shotDirection, normalized);
			float num = GameManager.ItemSettings.ElephantGunMaxShotDistance - travelledDistance;
			shieldOwner.PlayElectromagnetShieldHitForAllClients(normalized);
			int raycastHitCount = Physics.RaycastNonAlloc(new Ray(shieldHitPoint, vector), maxDistance: num, layerMask: GameManager.LayerSettings.GunHittablesMask, results: PlayerGolfer.raycastHitBuffer, queryTriggerInteraction: QueryTriggerInteraction.Ignore);
			if (!TryParseFirearmRaycastResults(PlayerGolfer.raycastHitBuffer, raycastHitCount, shieldOwner, out var raycastHit, out var hitHittable))
			{
				VfxManager.PlayElephantGunElectromagnetShieldDeflectedMissForAllClients(shieldOwner, normalized, vector, num);
				if (drawItemUseDebug)
				{
					BDebug.DrawLine(shieldHitPoint, shieldHitPoint + vector * num, Color.red, 2f);
				}
			}
			else
			{
				bool flag = hitHittable != null && CanHitWithGunshot(hitHittable, shieldOwner);
				Vector3 vector2 = default(Vector3);
				if (flag)
				{
					vector2 = hitHittable.transform.InverseTransformPoint(raycastHit.point);
					if (raycastHit.collider.gameObject.layer == GameManager.LayerSettings.ElectromagnetShieldLayer)
					{
						ReflectShotOffElectromagnetShield(raycastHit.point, vector, travelledDistance + raycastHit.distance, hitHittable.AsEntity.PlayerInfo);
						VfxManager.PlayElephantGunHitForAllClients(this, new VfxManager.GunShotHitVfxData(hitHittable, hitElectromagnetShield: true, vector2, raycastHit.point));
						if (drawItemUseDebug)
						{
							BDebug.DrawLine(shieldHitPoint, raycastHit.point, flag ? Color.blue : Color.yellow, 2f);
						}
						return;
					}
					hitHittable.HitWithItem(ItemType.ElephantGun, shieldOwner.ElectromagnetShieldItemUseId, vector2, vector, hitHittable.transform.InverseTransformPoint(shieldHitPoint), raycastHit.distance, shieldOwner.Inventory, isReflected: true, isInSpecialState: false, canHitWithNoUser: false);
				}
				VfxManager.PlayElephantGunElectromagnetShieldDeflectedHitForAllClients(shieldOwner, normalized, new VfxManager.GunShotHitVfxData(hitHittable, hitElectromagnetShield: false, vector2, raycastHit.point));
				if (drawItemUseDebug)
				{
					BDebug.DrawLine(shieldHitPoint, raycastHit.point, flag ? Color.blue : Color.yellow, 2f);
				}
			}
		}
		void Shoot()
		{
			Vector3 elephantGunBarrelEndPosition = GetElephantGunBarrelEndPosition();
			float localYaw;
			Vector3 firearmAimPoint = GetFirearmAimPoint(GameManager.ItemSettings.ElephantGunMaxAimingDistance, GameManager.LayerSettings.GunHittablesMask, out localYaw);
			BMath.Abs(localYaw);
			if (BMath.Abs(localYaw) > 45f)
			{
				PlayerInfo.Movement.AlignWithCameraImmediately();
				firearmAimPoint = GetFirearmAimPoint(GameManager.ItemSettings.DuelingPistolMaxAimingDistance, GameManager.LayerSettings.GunHittablesMask, out var _);
			}
			Vector3 vector = (firearmAimPoint - elephantGunBarrelEndPosition).RandomlyRotatedDeg(GameManager.ItemSettings.ElephantGunMaxInaccuracyAngle);
			PlayerInfo.Movement.InformShotElephantGun(vector, GetEffectiveSlot(EquippedItemIndex).remainingUses <= 1);
			Ray ray = new Ray(elephantGunBarrelEndPosition, vector);
			int raycastHitCount = Physics.RaycastNonAlloc(ray, maxDistance: GameManager.ItemSettings.ElephantGunMaxShotDistance, layerMask: GameManager.LayerSettings.GunHittablesMask, results: PlayerGolfer.raycastHitBuffer, queryTriggerInteraction: QueryTriggerInteraction.Ignore);
			if (!TryParseFirearmRaycastResults(PlayerGolfer.raycastHitBuffer, raycastHitCount, null, out var raycastHit, out var hitHittable))
			{
				VfxManager.PlayElephantGunMissForAllClients(this, ray.direction);
				if (drawItemUseDebug)
				{
					BDebug.DrawLine(ray.origin, ray.origin + ray.direction * GameManager.ItemSettings.ElephantGunMaxShotDistance, Color.red, 2f);
				}
			}
			else
			{
				bool flag = hitHittable != null && CanHitWithGunshot(hitHittable, null);
				Vector3 vector2 = default(Vector3);
				if (flag)
				{
					vector2 = hitHittable.transform.InverseTransformPoint(raycastHit.point);
					if (hitHittable.AsEntity.IsPlayer && hitHittable.AsEntity.PlayerInfo.IsElectromagnetShieldActive)
					{
						ReflectShotOffElectromagnetShield(raycastHit.point, ray.direction, raycastHit.distance, hitHittable.AsEntity.PlayerInfo);
						VfxManager.PlayElephantGunHitForAllClients(this, new VfxManager.GunShotHitVfxData(hitHittable, hitElectromagnetShield: true, vector2, raycastHit.point));
						if (drawItemUseDebug)
						{
							BDebug.DrawLine(ray.origin, raycastHit.point, flag ? Color.blue : Color.yellow, 2f);
						}
						return;
					}
					hitHittable.HitWithItem(ItemType.ElephantGun, IncrementAndGetCurrentItemUseId(ItemType.ElephantGun), vector2, ray.direction, hitHittable.transform.InverseTransformPoint(elephantGunBarrelEndPosition), raycastHit.distance, this, isReflected: false, isInSpecialState: false, canHitWithNoUser: false);
				}
				VfxManager.PlayElephantGunHitForAllClients(this, new VfxManager.GunShotHitVfxData(hitHittable, hitElectromagnetShield: false, vector2, raycastHit.point));
				if (drawItemUseDebug)
				{
					BDebug.DrawLine(ray.origin, raycastHit.point, flag ? Color.blue : Color.yellow, 2f);
				}
			}
		}
	}

	private IEnumerator ShootRocketLauncherRoutine()
	{
		SetCurrentItemUse(ItemUseType.Regular);
		Shoot();
		DecrementUseFromSlotAt(EquippedItemIndex);
		CameraModuleController.Shake(GameManager.CameraGameplaySettings.RocketLaunchScreenshakeSettings);
		PlayerInfo.PlayerAudio.PlayRocketLauncherShotForAllClients();
		bool didThrow = false;
		for (float timeSince = BMath.GetTimeSince(ItemUseTimestamp); timeSince < GameManager.ItemSettings.RocketLauncherShotDuration; timeSince = BMath.GetTimeSince(ItemUseTimestamp))
		{
			if (!didThrow && timeSince >= GameManager.ItemSettings.RocketLauncherThrowTime)
			{
				ThrowUsedItemForAllClients(ThrownUsedItemType.RocketLauncher);
				MarkThrownItem(ThrownItemHand.Right);
				didThrow = true;
			}
			yield return null;
		}
		SetCurrentItemUse(ItemUseType.None);
		RemoveIfOutOfUses(EquippedItemIndex);
		void Shoot()
		{
			float localYaw;
			Vector3 firearmAimPoint = GetFirearmAimPoint(GameManager.ItemSettings.RocketLauncherMaxAimingDistance, GameManager.LayerSettings.RocketHittablesMask, out localYaw);
			if (BMath.Abs(localYaw) > 45f)
			{
				PlayerInfo.Movement.AlignWithCameraImmediately();
				firearmAimPoint = GetFirearmAimPoint(GameManager.ItemSettings.DuelingPistolMaxAimingDistance, GameManager.LayerSettings.GunHittablesMask, out var _);
			}
			Vector3 rocketLauncherRocketPosition = GetRocketLauncherRocketPosition();
			Quaternion quaternion = Quaternion.LookRotation(firearmAimPoint - rocketLauncherRocketPosition, base.transform.up);
			Vector3 rocketLauncherBarrelBackEndPosition = GetRocketLauncherBarrelBackEndPosition();
			Quaternion quaternion2 = Quaternion.AngleAxis(180f, base.transform.up) * quaternion;
			Vector3 vector = quaternion2 * Vector3.forward;
			Vector3 center = rocketLauncherBarrelBackEndPosition + vector * GameManager.ItemSettings.RocketLauncherBackBlastDistance / 2f;
			Box box = new Box(center, GameManager.ItemSettings.RocketLauncherBackBlastSize, quaternion2);
			int num = box.OverlapNonAlloc(GameManager.LayerSettings.RocketBackBlastHittablesMask, PlayerGolfer.overlappingColliderBuffer);
			if (drawItemUseDebug)
			{
				box.DrawDebug(Color.red, 5f);
			}
			processedHittableBuffer.Clear();
			for (int i = 0; i < num; i++)
			{
				if (PlayerGolfer.overlappingColliderBuffer[i].TryGetComponentInParent<Hittable>(out var foundComponent, includeInactive: true) && !(foundComponent == PlayerInfo.AsHittable) && processedHittableBuffer.Add(foundComponent))
				{
					if (!foundComponent.TryGetClosestPointOnAllActiveColliders(rocketLauncherBarrelBackEndPosition, out var closestPoint, out var _))
					{
						closestPoint = ((!foundComponent.AsEntity.HasRigidbody) ? foundComponent.transform.position : foundComponent.AsEntity.Rigidbody.worldCenterOfMass);
					}
					foundComponent.HitWithRocketLauncherBackBlast(foundComponent.transform.InverseTransformPoint(closestPoint), foundComponent.transform.InverseTransformPoint(rocketLauncherBarrelBackEndPosition), vector, this);
					if (drawItemUseDebug)
					{
						BDebug.DrawLine(rocketLauncherBarrelBackEndPosition, closestPoint, Color.yellow, 5f);
					}
				}
			}
			Hittable lockOnTargetAsHittable = ((LockOnTarget != null) ? LockOnTarget.AsEntity.AsHittable : null);
			OnShotRocket();
			CmdInformShotRocket(rocketLauncherRocketPosition, quaternion, lockOnTargetAsHittable, IncrementAndGetCurrentItemUseId(ItemType.RocketLauncher));
			VfxManager.PlayRocketLaunchLocalOnly(this, quaternion);
		}
	}

	private IEnumerator BlowAirhornRoutine()
	{
		SetCurrentItemUse(ItemUseType.Regular);
		BlowAirhorn();
		DecrementUseFromSlotAt(EquippedItemIndex);
		PlayerInfo.PlayerAudio.PlayItemUseForAllClients(ItemType.Airhorn);
		bool didThrow = false;
		for (float timeSince = BMath.GetTimeSince(ItemUseTimestamp); timeSince < GameManager.ItemSettings.AirhornUseDuration; timeSince = BMath.GetTimeSince(ItemUseTimestamp))
		{
			if (!didThrow && timeSince >= GameManager.ItemSettings.AirhornThrowTime)
			{
				ThrowUsedItemForAllClients(ThrownUsedItemType.Airhorn);
				MarkThrownItem(ThrownItemHand.Right);
				didThrow = true;
			}
			yield return null;
		}
		SetCurrentItemUse(ItemUseType.None);
		RemoveIfOutOfUses(EquippedItemIndex);
		void BlowAirhorn()
		{
			int num = Physics.OverlapSphereNonAlloc(base.transform.position, GameManager.ItemSettings.AirhornRange, layerMask: GameManager.LayerSettings.PlayersMask, results: PlayerGolfer.overlappingColliderBuffer, queryTriggerInteraction: QueryTriggerInteraction.Ignore);
			processedPlayerBuffer.Clear();
			for (int i = 0; i < num; i++)
			{
				if (PlayerGolfer.overlappingColliderBuffer[i].TryGetComponentInParent<PlayerInfo>(out var foundComponent, includeInactive: true) && !(foundComponent == PlayerInfo) && processedPlayerBuffer.Add(foundComponent))
				{
					foundComponent.CmdInformPlayerOfBlownAirhorn();
				}
			}
			PlayAirhornVfxInternal();
			CmdPlayAirhornVfxForAllClients();
		}
	}

	private IEnumerator PlaceGolfCartRoutine()
	{
		SetCurrentItemUse(ItemUseType.Regular);
		CourseManager.CmdSpawnGolfCartForLocalPlayer();
		waitToEnterPlacedGolfCartRoutine = StartCoroutine(WaitToEnterPlacedGolfCartRoutine());
		bool didSplit = false;
		bool startedOpening = false;
		bool finishedOpening = false;
		for (float timeSince = BMath.GetTimeSince(ItemUseTimestamp); timeSince < GameManager.ItemSettings.GolfCartPlacementTotalDuration; timeSince = BMath.GetTimeSince(ItemUseTimestamp))
		{
			if (!didSplit && timeSince >= GameManager.ItemSettings.GolfCartSeparatePartsDuration)
			{
				shouldSplitGolfCartBriefcase = true;
				UpdateEquipmentSwitchers();
				didSplit = true;
			}
			if (!startedOpening && timeSince >= GameManager.ItemSettings.GolfCartBriefcaseOpenStartTime)
			{
				PlayGolfCartBriefcaseEffectsForAllClients(isStart: true);
				startedOpening = true;
			}
			if (!finishedOpening && timeSince >= GameManager.ItemSettings.GolfBriefcaseOpenEndTime)
			{
				PlayGolfCartBriefcaseEffectsForAllClients(isStart: false);
				finishedOpening = true;
			}
			yield return null;
		}
		ThrowUsedItemForAllClients(ThrownUsedItemType.GolfCartBase);
		ThrowUsedItemForAllClients(ThrownUsedItemType.GolfCartLid);
		MarkThrownItem(ThrownItemHand.Right);
		MarkThrownItem(ThrownItemHand.Left);
		DecrementUseFromSlotAt(EquippedItemIndex);
		yield return waitToEnterPlacedGolfCartRoutine;
		SetCurrentItemUse(ItemUseType.None);
		RemoveIfOutOfUses(EquippedItemIndex);
		void PlayGolfCartBriefcaseEffectsForAllClients(bool isStart)
		{
			PlayGolfCartBriefcaseEffectsInternal(isStart);
			CmdPlayGolfCartBriefcaseEffectsForAllClients(isStart);
		}
		bool TryEnterReservedGolfCart()
		{
			if (reservedGolfCart == null)
			{
				return false;
			}
			reservedGolfCart.CmdEnterReservedGolfCart();
			return true;
		}
		IEnumerator WaitToEnterPlacedGolfCartRoutine()
		{
			IsWaitingToEnterPlacedGolfCart = true;
			yield return new WaitForSeconds(GameManager.ItemSettings.GolfCartPlacementTotalDuration);
			float timeout = BMath.Max(1f, (float)NetworkTime.rtt * 4f);
			float time = 0f;
			while (!TryEnterReservedGolfCart() && !TryEnterReservedGolfCart())
			{
				yield return null;
				time += Time.deltaTime;
				if (time > timeout)
				{
					CancelEnterPlacedGolfCartRoutine();
					yield break;
				}
			}
			while (!PlayerInfo.ActiveGolfCartSeat.IsValid())
			{
				yield return null;
				time += Time.deltaTime;
				if (time > timeout)
				{
					CancelEnterPlacedGolfCartRoutine();
					yield break;
				}
			}
			IsWaitingToEnterPlacedGolfCart = false;
			PlayerInventory.LocalPlayerSuccessfullyEnteredReservedGolfCart?.Invoke();
		}
	}

	[Command]
	private void CmdPlayGolfCartBriefcaseEffectsForAllClients(bool isStart, NetworkConnectionToClient sender = null)
	{
		if (base.isServer && base.isClient)
		{
			UserCode_CmdPlayGolfCartBriefcaseEffectsForAllClients__Boolean__NetworkConnectionToClient(isStart, sender);
			return;
		}
		NetworkWriterPooled writer = NetworkWriterPool.Get();
		writer.WriteBool(isStart);
		SendCommandInternal("System.Void PlayerInventory::CmdPlayGolfCartBriefcaseEffectsForAllClients(System.Boolean,Mirror.NetworkConnectionToClient)", -573411698, writer, 0);
		NetworkWriterPool.Return(writer);
	}

	[TargetRpc]
	private void RpcPlayGolfCartBriefcaseEffects(NetworkConnectionToClient connection, bool isStart)
	{
		NetworkWriterPooled writer = NetworkWriterPool.Get();
		writer.WriteBool(isStart);
		SendTargetRPCInternal(connection, "System.Void PlayerInventory::RpcPlayGolfCartBriefcaseEffects(Mirror.NetworkConnectionToClient,System.Boolean)", 1805712063, writer, 0);
		NetworkWriterPool.Return(writer);
	}

	private void PlayGolfCartBriefcaseEffectsInternal(bool isStart)
	{
		if (VfxPersistentData.TryGetPooledVfx(isStart ? VfxType.GolfCartBriefcaseOpenStart : VfxType.GolfCartBriefcaseOpenEnd, out var particleSystem))
		{
			particleSystem.transform.SetParent(PlayerInfo.RightHandEquipmentSwitcher.transform);
			particleSystem.transform.SetLocalPositionAndRotation(Vector3.zero, Quaternion.identity);
			particleSystem.Play();
		}
		PlayerInfo.PlayerAudio.PlayGolfCartBriefcaseOpenLocalOnly(isStart);
	}

	private IEnumerator PlantLandmineRoutine()
	{
		SetCurrentItemUse(ItemUseType.Regular);
		bool jammedIntoGround = false;
		for (float timeSince = BMath.GetTimeSince(ItemUseTimestamp); timeSince < GameManager.ItemSettings.LandminePlantingTime; timeSince = BMath.GetTimeSince(ItemUseTimestamp))
		{
			if (!jammedIntoGround && timeSince >= GameManager.ItemSettings.LandminePlantingStickIntoGroundTime)
			{
				Vector3 position = PlayerInfo.RightHandEquipmentSwitcher.transform.position;
				position.y = base.transform.position.y;
				if (base.isServer)
				{
					VfxManager.ServerPlayPooledVfxForAllClients(VfxType.MineBurrow, position, Quaternion.identity);
				}
				else
				{
					VfxManager.ClientPlayPooledVfxForAllClients(VfxType.MineBurrow, position, Quaternion.identity);
				}
				CameraModuleController.Shake(GameManager.CameraGameplaySettings.LandminePlantJamScreenshakeSettings, base.transform.position);
				PlayerInfo.PlayerAudio.PlayLandminePlantForAllClients(stomp: false);
				jammedIntoGround = true;
			}
			yield return null;
		}
		if (!PlayerInfo.Movement.IsGrounded)
		{
			CancelItemUse();
			yield break;
		}
		Vector3 position2 = base.transform.position;
		Quaternion quaternion = GetEffectiveRotation();
		Vector3 vector = quaternion * Vector3.down * GameManager.ItemSettings.LandminePlantingOffsetIntoGround;
		MarkThrownItem(ThrownItemHand.Right);
		CmdSpawnLandmine(position2 + vector, quaternion, Vector3.zero, Vector3.zero, LandmineArmType.Planted, IncrementAndGetCurrentItemUseId(ItemType.Landmine));
		if (base.isServer)
		{
			VfxManager.ServerPlayPooledVfxForAllClients(VfxType.MineBurrow, position2, Quaternion.identity);
		}
		else
		{
			VfxManager.ClientPlayPooledVfxForAllClients(VfxType.MineBurrow, position2, Quaternion.identity);
		}
		CameraModuleController.Shake(GameManager.CameraGameplaySettings.LandminePlantStompScreenshakeSettings, base.transform.position);
		PlayerInfo.PlayerAudio.PlayLandminePlantForAllClients(stomp: true);
		DecrementUseFromSlotAt(EquippedItemIndex);
		yield return new WaitForSeconds(GameManager.ItemSettings.LandminePostPlantingTime);
		SetCurrentItemUse(ItemUseType.None);
		RemoveIfOutOfUses(EquippedItemIndex);
		Quaternion GetEffectiveRotation()
		{
			Vector3 normal = PlayerInfo.Movement.GroundData.normal;
			if (!base.transform.forward.TryProjectOnPlaneAlong(Vector3.up, normal, out var projection))
			{
				return Quaternion.Euler(0f, GameManager.LocalPlayerInfo.transform.eulerAngles.y, 0f);
			}
			return Quaternion.LookRotation(projection, normal);
		}
	}

	private IEnumerator TossLandmineRoutine()
	{
		SetCurrentItemUse(ItemUseType.Alt);
		if (BMath.Abs((GameManager.Camera.transform.forward.GetYawDeg() - PlayerInfo.Movement.Yaw).WrapAngleDeg()) > 45f)
		{
			PlayerInfo.Movement.AlignWithCameraImmediately();
		}
		yield return new WaitForSeconds(GameManager.ItemSettings.LandmineTossingTime);
		PlayerInfo.RightHandEquipmentSwitcher.transform.GetPositionAndRotation(out var position, out var rotation);
		Vector3 vector = base.transform.TransformDirection(GameManager.ItemSettings.LandmineTossingDirectionLocalRotation * Vector3.forward);
		Vector3 vector2 = base.transform.TransformDirection(GameManager.ItemSettings.LandmineTossingDirectionLocalRotation * GameManager.ItemSettings.LandmineTossingLocalAngularVelocity);
		Vector3 velocity = PlayerInfo.Rigidbody.linearVelocity + vector * GameManager.ItemSettings.LandmineTossingSpeed;
		Vector3 angularVelocity = PlayerInfo.Rigidbody.angularVelocity + vector2;
		MarkThrownItem(ThrownItemHand.Right);
		CmdSpawnLandmine(position, rotation, velocity, angularVelocity, LandmineArmType.Tossed, IncrementAndGetCurrentItemUseId(ItemType.Landmine));
		DecrementUseFromSlotAt(EquippedItemIndex);
		yield return new WaitForSeconds(GameManager.ItemSettings.LandminePostTossingTime);
		SetCurrentItemUse(ItemUseType.None);
		RemoveIfOutOfUses(EquippedItemIndex);
	}

	private IEnumerator ActivateElectromagnetRoutine()
	{
		SetCurrentItemUse(ItemUseType.Regular);
		PlayerInfo.LocalPlayerActivateElectromagnetShield(IncrementAndGetCurrentItemUseId(ItemType.Electromagnet));
		DecrementUseFromSlotAt(EquippedItemIndex);
		OnActivatedElectromagnet();
		CmdActivatedElectromagnet();
		bool didThrow = false;
		for (float timeSince = BMath.GetTimeSince(ItemUseTimestamp); timeSince < GameManager.ItemSettings.ElectromagnetActivationTotalDuration; timeSince = BMath.GetTimeSince(ItemUseTimestamp))
		{
			if (!didThrow && timeSince >= GameManager.ItemSettings.ElectromagnetThrowTime)
			{
				ThrowUsedItemForAllClients(ThrownUsedItemType.Electromagnet);
				MarkThrownItem(ThrownItemHand.Right);
				didThrow = true;
			}
			yield return null;
		}
		SetCurrentItemUse(ItemUseType.None);
		RemoveIfOutOfUses(EquippedItemIndex);
	}

	[Command]
	private void CmdSpawnLandmine(Vector3 position, Quaternion rotation, Vector3 velocity, Vector3 angularVelocity, LandmineArmType armType, ItemUseId itemUseId, NetworkConnectionToClient sender = null)
	{
		if (base.isServer && base.isClient)
		{
			UserCode_CmdSpawnLandmine__Vector3__Quaternion__Vector3__Vector3__LandmineArmType__ItemUseId__NetworkConnectionToClient(position, rotation, velocity, angularVelocity, armType, itemUseId, sender);
			return;
		}
		NetworkWriterPooled writer = NetworkWriterPool.Get();
		writer.WriteVector3(position);
		writer.WriteQuaternion(rotation);
		writer.WriteVector3(velocity);
		writer.WriteVector3(angularVelocity);
		GeneratedNetworkCode._Write_LandmineArmType(writer, armType);
		GeneratedNetworkCode._Write_ItemUseId(writer, itemUseId);
		SendCommandInternal("System.Void PlayerInventory::CmdSpawnLandmine(UnityEngine.Vector3,UnityEngine.Quaternion,UnityEngine.Vector3,UnityEngine.Vector3,LandmineArmType,ItemUseId,Mirror.NetworkConnectionToClient)", -198085435, writer, 0);
		NetworkWriterPool.Return(writer);
	}

	private IEnumerator ActivateOrbitalLaserRoutine()
	{
		SetCurrentItemUse(ItemUseType.Regular);
		yield return new WaitForSeconds(GameManager.ItemSettings.OrbitalLaserActivationTime);
		Vector3 fallbackPosition;
		Hittable target = OrbitalLaserManager.GetTarget(out fallbackPosition);
		if (target == PlayerInfo.AsHittable && !SingletonBehaviour<DrivingRangeManager>.HasInstance && CourseManager.CountActivePlayers() > 1)
		{
			GameManager.AchievementsManager.Unlock(AchievementId.PhoneHome);
		}
		CmdActivateOrbitalLaser(target, fallbackPosition, IncrementAndGetCurrentItemUseId(ItemType.OrbitalLaser));
		DecrementUseFromSlotAt(EquippedItemIndex);
		OnActivatedOrbitalLaser();
		CmdActivatedOrbitalLaser();
		bool didThrow = false;
		for (float timeSince = BMath.GetTimeSince(ItemUseTimestamp); timeSince < GameManager.ItemSettings.OrbitalLaserActivationTotalDuration; timeSince = BMath.GetTimeSince(ItemUseTimestamp))
		{
			if (!didThrow && timeSince >= GameManager.ItemSettings.OrbitalLaserThrowTime)
			{
				ThrowUsedItemForAllClients(ThrownUsedItemType.OrbitalLaser);
				MarkThrownItem(ThrownItemHand.Right);
				didThrow = true;
			}
			yield return null;
		}
		SetCurrentItemUse(ItemUseType.None);
		RemoveIfOutOfUses(EquippedItemIndex);
	}

	[Command]
	private void CmdActivateOrbitalLaser(Hittable target, Vector3 fallbackWorldPosition, ItemUseId itemUseId)
	{
		if (base.isServer && base.isClient)
		{
			UserCode_CmdActivateOrbitalLaser__Hittable__Vector3__ItemUseId(target, fallbackWorldPosition, itemUseId);
			return;
		}
		NetworkWriterPooled writer = NetworkWriterPool.Get();
		writer.WriteNetworkBehaviour(target);
		writer.WriteVector3(fallbackWorldPosition);
		GeneratedNetworkCode._Write_ItemUseId(writer, itemUseId);
		SendCommandInternal("System.Void PlayerInventory::CmdActivateOrbitalLaser(Hittable,UnityEngine.Vector3,ItemUseId)", 1969500504, writer, 0);
		NetworkWriterPool.Return(writer);
	}

	private void ThrowUsedItemForAllClients(ThrownUsedItemType thrownItemType, bool forcePlayerPosition = false, Vector3 forcedPlayerPosition = default(Vector3))
	{
		ThrowUsedItemInternal(thrownItemType, forcePlayerPosition, forcedPlayerPosition);
		CmdThrowUsedItemForAllClients(thrownItemType, forcePlayerPosition, forcedPlayerPosition);
	}

	[Command]
	private void CmdThrowUsedItemForAllClients(ThrownUsedItemType thrownItemType, bool forcePlayerPosition, Vector3 forcedPlayerPosition, NetworkConnectionToClient sender = null)
	{
		if (base.isServer && base.isClient)
		{
			UserCode_CmdThrowUsedItemForAllClients__ThrownUsedItemType__Boolean__Vector3__NetworkConnectionToClient(thrownItemType, forcePlayerPosition, forcedPlayerPosition, sender);
			return;
		}
		NetworkWriterPooled writer = NetworkWriterPool.Get();
		GeneratedNetworkCode._Write_ThrownUsedItemType(writer, thrownItemType);
		writer.WriteBool(forcePlayerPosition);
		writer.WriteVector3(forcedPlayerPosition);
		SendCommandInternal("System.Void PlayerInventory::CmdThrowUsedItemForAllClients(ThrownUsedItemType,System.Boolean,UnityEngine.Vector3,Mirror.NetworkConnectionToClient)", 130731927, writer, 0);
		NetworkWriterPool.Return(writer);
	}

	[TargetRpc]
	private void RpcThrowUsedItem(NetworkConnectionToClient connection, ThrownUsedItemType thrownItemType, bool forcePlayerPosition, Vector3 forcedPlayerPosition)
	{
		NetworkWriterPooled writer = NetworkWriterPool.Get();
		GeneratedNetworkCode._Write_ThrownUsedItemType(writer, thrownItemType);
		writer.WriteBool(forcePlayerPosition);
		writer.WriteVector3(forcedPlayerPosition);
		SendTargetRPCInternal(connection, "System.Void PlayerInventory::RpcThrowUsedItem(Mirror.NetworkConnectionToClient,ThrownUsedItemType,System.Boolean,UnityEngine.Vector3)", -1308103398, writer, 0);
		NetworkWriterPool.Return(writer);
	}

	private void ThrowUsedItemInternal(ThrownUsedItemType thrownItemType, bool forcePlayerPosition, Vector3 forcedPlayerPosition)
	{
		(Transform, Quaternion, Vector3, float) tuple = default((Transform, Quaternion, Vector3, float));
		switch (thrownItemType)
		{
		case ThrownUsedItemType.Airhorn:
			tuple = (PlayerInfo.RightHandEquipmentSwitcher.transform, GameManager.ItemSettings.AirhornThrowDirectionLocalRotation, GameManager.ItemSettings.AirhornThrowLocalAngularVelocity, GameManager.ItemSettings.AirhornThrowSpeed);
			break;
		case ThrownUsedItemType.CoffeePlate:
			tuple = (PlayerInfo.LeftHandEquipmentSwitcher.transform, GameManager.ItemSettings.CoffeeThrowPlateDirectionLocalRotation, GameManager.ItemSettings.CoffeeThrowPlateLocalAngularVelocity, GameManager.ItemSettings.CoffeeThrowPlateSpeed);
			break;
		case ThrownUsedItemType.CoffeeMug:
			tuple = (PlayerInfo.RightHandEquipmentSwitcher.transform, GameManager.ItemSettings.CoffeeThrowMugDirectionLocalRotation, GameManager.ItemSettings.CoffeeThrowMugLocalAngularVelocity, GameManager.ItemSettings.CoffeeThrowMugSpeed);
			break;
		case ThrownUsedItemType.DuelingPistol:
			tuple = (PlayerInfo.LeftHandEquipmentSwitcher.transform, GameManager.ItemSettings.DuelingPistolThrowDirectionLocalRotation, GameManager.ItemSettings.DuelingPistolThrowLocalAngularVelocity, GameManager.ItemSettings.DuelingPistolThrowSpeed);
			break;
		case ThrownUsedItemType.RocketLauncher:
			tuple = (PlayerInfo.RightHandEquipmentSwitcher.transform, GameManager.ItemSettings.RocketLauncherThrowDirectionLocalRotation, GameManager.ItemSettings.RocketLauncherThrowLocalAngularVelocity, GameManager.ItemSettings.RocketLauncherThrowSpeed);
			break;
		case ThrownUsedItemType.Electromagnet:
			tuple = (PlayerInfo.RightHandEquipmentSwitcher.transform, GameManager.ItemSettings.ElectromagnetThrowDirectionLocalRotation, GameManager.ItemSettings.ElectromagnetThrowLocalAngularVelocity, GameManager.ItemSettings.ElectromagnetThrowSpeed);
			break;
		case ThrownUsedItemType.OrbitalLaser:
			tuple = (PlayerInfo.RightHandEquipmentSwitcher.transform, GameManager.ItemSettings.OrbitalLaserThrowDirectionLocalRotation, GameManager.ItemSettings.OrbitalLaserThrowLocalAngularVelocity, GameManager.ItemSettings.OrbitalLaserThrowSpeed);
			break;
		case ThrownUsedItemType.GolfCartBase:
			tuple = (PlayerInfo.RightHandEquipmentSwitcher.transform, GameManager.ItemSettings.GolfCartThrowBaseDirectionLocalRotation, GameManager.ItemSettings.GolfCartThrowBaseLocalAngularVelocity, GameManager.ItemSettings.GolfCartThrowBaseSpeed);
			break;
		case ThrownUsedItemType.GolfCartLid:
			tuple = (PlayerInfo.LeftHandEquipmentSwitcher.transform, GameManager.ItemSettings.GolfCartThrowLidDirectionLocalRotation, GameManager.ItemSettings.GolfCartThrowLidLocalAngularVelocity, GameManager.ItemSettings.GolfCartThrowLidSpeed);
			break;
		case ThrownUsedItemType.ElephantGun:
			tuple = (PlayerInfo.RightHandEquipmentSwitcher.transform, GameManager.ItemSettings.ElephantGunThrowDirectionLocalRotation, GameManager.ItemSettings.ElephantGunThrowLocalAngularVelocity, GameManager.ItemSettings.ElephantGunThrowSpeed);
			break;
		case ThrownUsedItemType.SpringBootLeft:
			tuple = (PlayerInfo.LeftFootCenter.transform, GameManager.ItemSettings.SpringBootLeftPopOffDirectionLocalRotation, GameManager.ItemSettings.SpringBootLeftPopOffLocalAngularVelocity, GameManager.ItemSettings.SpringBootLeftPopOffSpeed);
			break;
		case ThrownUsedItemType.SpringBootRight:
			tuple = (PlayerInfo.RightFootCenter.transform, GameManager.ItemSettings.SpringBootRightPopOffDirectionLocalRotation, GameManager.ItemSettings.SpringBootRightPopOffLocalAngularVelocity, GameManager.ItemSettings.SpringBootRightPopOffSpeed);
			break;
		default:
			global::_003CPrivateImplementationDetails_003E.ThrowSwitchExpressionException(thrownItemType);
			break;
		}
		var (transform, quaternion, vector, num) = tuple;
		transform.GetPositionAndRotation(out var position, out var rotation);
		if (forcePlayerPosition)
		{
			position += forcedPlayerPosition - base.transform.position;
		}
		Vector3 vector2 = PlayerInfo.Rigidbody.linearVelocity;
		Vector3 vector3 = base.transform.TransformDirection(quaternion * Vector3.forward);
		Vector3 vector4 = base.transform.TransformDirection(quaternion * vector);
		if (thrownItemType == ThrownUsedItemType.SpringBootLeft || thrownItemType == ThrownUsedItemType.SpringBootRight)
		{
			position.y += 0.15f;
			if (thrownItemType == ThrownUsedItemType.SpringBootLeft)
			{
				rotation *= Quaternion.Euler(90f, 0f, 90f);
			}
			else
			{
				rotation *= Quaternion.Euler(-90f, 0f, -90f);
			}
			vector2 = Vector3.zero;
			num *= UnityEngine.Random.Range(0.9f, 1.1f);
			vector3 = vector3.RandomlyRotatedDeg(20f);
			vector4 += UnityEngine.Random.insideUnitSphere;
		}
		Vector3 velocity = vector2 + vector3 * num;
		Vector3 angularVelocity = PlayerInfo.Rigidbody.angularVelocity + vector4;
		ThrownUsedItem unusedThrownItem = ThrownUsedItemManager.GetUnusedThrownItem(thrownItemType);
		unusedThrownItem.Initialize(position, rotation, velocity, angularVelocity);
		PlayerInfo.AsEntity.TemporarilyIgnoreCollisionsWith(unusedThrownItem.Rigidbody, 0.5f);
	}

	[Command(requiresAuthority = false)]
	public void CmdInformInterruptedPlayerWithAirhorn(NetworkConnectionToClient sender = null)
	{
		if (base.isServer && base.isClient)
		{
			UserCode_CmdInformInterruptedPlayerWithAirhorn__NetworkConnectionToClient(sender);
			return;
		}
		NetworkWriterPooled writer = NetworkWriterPool.Get();
		SendCommandInternal("System.Void PlayerInventory::CmdInformInterruptedPlayerWithAirhorn(Mirror.NetworkConnectionToClient)", 804066992, writer, 0, requiresAuthority: false);
		NetworkWriterPool.Return(writer);
	}

	[Command]
	private void CmdInformShotRocket(Vector3 rocketPosition, Quaternion rocketRotation, Hittable lockOnTargetAsHittable, ItemUseId itemUseId, NetworkConnectionToClient sender = null)
	{
		if (base.isServer && base.isClient)
		{
			UserCode_CmdInformShotRocket__Vector3__Quaternion__Hittable__ItemUseId__NetworkConnectionToClient(rocketPosition, rocketRotation, lockOnTargetAsHittable, itemUseId, sender);
			return;
		}
		NetworkWriterPooled writer = NetworkWriterPool.Get();
		writer.WriteVector3(rocketPosition);
		writer.WriteQuaternion(rocketRotation);
		writer.WriteNetworkBehaviour(lockOnTargetAsHittable);
		GeneratedNetworkCode._Write_ItemUseId(writer, itemUseId);
		SendCommandInternal("System.Void PlayerInventory::CmdInformShotRocket(UnityEngine.Vector3,UnityEngine.Quaternion,Hittable,ItemUseId,Mirror.NetworkConnectionToClient)", 2001705532, writer, 0);
		NetworkWriterPool.Return(writer);
	}

	[TargetRpc]
	private void RpcInformShotRocket(NetworkConnectionToClient connection)
	{
		NetworkWriterPooled writer = NetworkWriterPool.Get();
		SendTargetRPCInternal(connection, "System.Void PlayerInventory::RpcInformShotRocket(Mirror.NetworkConnectionToClient)", -1867143617, writer, 0);
		NetworkWriterPool.Return(writer);
	}

	private void OnShotRocket()
	{
		if (!base.isLocalPlayer)
		{
			VfxManager.PlayRocketLaunchLocalOnly(this);
		}
		if (!(PlayerInfo.RightHandEquipmentSwitcher.CurrentEquipment == null) && PlayerInfo.RightHandEquipmentSwitcher.CurrentEquipment.TryGetComponent<RocketLauncherEquipment>(out var component))
		{
			component.SetRocketMeshEnabled(enabled: false);
		}
	}

	[Command]
	private void CmdActivatedElectromagnet(NetworkConnectionToClient sender = null)
	{
		if (base.isServer && base.isClient)
		{
			UserCode_CmdActivatedElectromagnet__NetworkConnectionToClient(sender);
			return;
		}
		NetworkWriterPooled writer = NetworkWriterPool.Get();
		SendCommandInternal("System.Void PlayerInventory::CmdActivatedElectromagnet(Mirror.NetworkConnectionToClient)", 1092116594, writer, 0);
		NetworkWriterPool.Return(writer);
	}

	[TargetRpc]
	private void RpcInformActivatedElectromagnet(NetworkConnectionToClient connection)
	{
		NetworkWriterPooled writer = NetworkWriterPool.Get();
		SendTargetRPCInternal(connection, "System.Void PlayerInventory::RpcInformActivatedElectromagnet(Mirror.NetworkConnectionToClient)", -243845688, writer, 0);
		NetworkWriterPool.Return(writer);
	}

	private void OnActivatedElectromagnet()
	{
		if (!(PlayerInfo.RightHandEquipmentSwitcher.CurrentEquipment == null) && PlayerInfo.RightHandEquipmentSwitcher.CurrentEquipment.TryGetComponent<ElectromagnetEquipment>(out var component))
		{
			component.Activate();
		}
	}

	[Command]
	private void CmdActivatedOrbitalLaser(NetworkConnectionToClient sender = null)
	{
		if (base.isServer && base.isClient)
		{
			UserCode_CmdActivatedOrbitalLaser__NetworkConnectionToClient(sender);
			return;
		}
		NetworkWriterPooled writer = NetworkWriterPool.Get();
		SendCommandInternal("System.Void PlayerInventory::CmdActivatedOrbitalLaser(Mirror.NetworkConnectionToClient)", 595511568, writer, 0);
		NetworkWriterPool.Return(writer);
	}

	[TargetRpc]
	private void RpcInformActivatedOrbitalLaser(NetworkConnectionToClient connection)
	{
		NetworkWriterPooled writer = NetworkWriterPool.Get();
		SendTargetRPCInternal(connection, "System.Void PlayerInventory::RpcInformActivatedOrbitalLaser(Mirror.NetworkConnectionToClient)", -1176879774, writer, 0);
		NetworkWriterPool.Return(writer);
	}

	private void OnActivatedOrbitalLaser()
	{
		if (!(PlayerInfo.RightHandEquipmentSwitcher.CurrentEquipment == null) && PlayerInfo.RightHandEquipmentSwitcher.CurrentEquipment.TryGetComponent<OrbitalLaserEquipment>(out var component))
		{
			component.Activate();
		}
	}

	private bool TryParseFirearmRaycastResults(RaycastHit[] raycastResults, int raycastHitCount, PlayerInfo deflectedShotShieldOwner, out RaycastHit raycastHit, out Hittable hitHittable)
	{
		RaycastHit raycastHit2 = new RaycastHit
		{
			distance = float.MaxValue
		};
		Hittable hittable = null;
		bool flag = false;
		bool flag2 = false;
		for (int i = 0; i < raycastHitCount; i++)
		{
			RaycastHit raycastHit3 = raycastResults[i];
			if (!TryGetParentHittable(raycastHit3, out var parentHittable) || (deflectedShotShieldOwner != null && parentHittable == deflectedShotShieldOwner.AsHittable))
			{
				continue;
			}
			bool flag3;
			bool flag4;
			if (parentHittable.AsEntity.IsGolfCart)
			{
				flag3 = raycastHit3.collider.gameObject.layer == GameManager.LayerSettings.HittablesLayer;
				flag4 = !flag3 && raycastHit3.collider.gameObject.layer == GameManager.LayerSettings.GolfCartPassengersLayer;
			}
			else
			{
				flag3 = false;
				flag4 = false;
			}
			bool flag5 = false;
			if (flag2)
			{
				if (flag3 && hittable == parentHittable)
				{
					continue;
				}
			}
			else if (flag)
			{
				flag5 = flag4 && hittable == parentHittable;
			}
			if (flag5 || !(raycastHit3.distance > raycastHit2.distance))
			{
				raycastHit2 = raycastHit3;
				hittable = parentHittable;
				flag = flag3;
				flag2 = flag4;
			}
		}
		raycastHit = raycastHit2;
		if (flag2 && hittable.AsEntity.AsGolfCart.TryGetPassengerFromCollider(raycastHit2.collider, out var passenger))
		{
			hitHittable = passenger.AsHittable;
		}
		else
		{
			hitHittable = hittable;
		}
		return hittable != null;
		static bool TryGetParentHittable(RaycastHit raycastHit4, out Hittable foundComponent)
		{
			if (raycastHit4.rigidbody != null)
			{
				return raycastHit4.rigidbody.TryGetComponentInParent<Hittable>(out foundComponent, includeInactive: true);
			}
			return raycastHit4.collider.TryGetComponentInParent<Hittable>(out foundComponent, includeInactive: true);
		}
	}

	public Vector3 GetFirearmAimPoint(float maxDistance, int layerMask, out float localYaw)
	{
		Ray cameraRay = new Ray(GameManager.Camera.transform.position, GameManager.Camera.transform.forward);
		Ray ray = cameraRay;
		float maxDistance2 = maxDistance;
		int num = Physics.RaycastNonAlloc(ray, PlayerGolfer.raycastHitBuffer, maxDistance2, layerMask, QueryTriggerInteraction.Ignore);
		RaycastHit raycastHit = new RaycastHit
		{
			distance = float.MaxValue
		};
		for (int i = 0; i < num; i++)
		{
			RaycastHit raycastHit2 = PlayerGolfer.raycastHitBuffer[i];
			if ((!raycastHit2.collider.TryGetComponentInParent<PlayerInventory>(out var foundComponent, includeInactive: true) || !(foundComponent == this)) && !(raycastHit2.distance >= raycastHit.distance))
			{
				raycastHit = raycastHit2;
			}
		}
		bool num2 = raycastHit.distance < float.MaxValue;
		Vector3 vector = (num2 ? raycastHit.point : GetDefaultAimPoint());
		Vector3 vector2 = base.transform.position + base.transform.up * 1.5f;
		Vector3 vector3 = vector - vector2;
		localYaw = (vector3.GetYawDeg() - base.transform.forward.GetYawDeg()).WrapAngleDeg();
		if (!num2)
		{
			return vector;
		}
		if (BMath.Abs(localYaw) > 45f)
		{
			return GetDefaultAimPoint();
		}
		return vector;
		Vector3 GetDefaultAimPoint()
		{
			return cameraRay.origin + cameraRay.direction * maxDistance;
		}
	}

	private bool CanHitWithGunshot(Hittable hittable, PlayerInfo deflectedGunshotShieldOwner)
	{
		if (deflectedGunshotShieldOwner != null)
		{
			if (hittable == deflectedGunshotShieldOwner.AsHittable)
			{
				return false;
			}
		}
		else if (hittable == PlayerInfo.AsHittable)
		{
			return false;
		}
		return true;
	}

	private void DecrementUseFromSlotAt(int index)
	{
		if (!base.isServer && !base.isLocalPlayer)
		{
			Debug.LogError("Only the server and the local player are allowed to decrement uses from items in an inventory", base.gameObject);
			return;
		}
		InventorySlot effectiveSlot = GetEffectiveSlot(index);
		effectiveSlot.remainingUses--;
		if (base.isServer)
		{
			slots[index] = effectiveSlot;
			return;
		}
		localPlayerSlotOverrides[index] = effectiveSlot;
		CmdDecrementUseFromSlotAt(index);
		Hotkeys.UpdatePlayerInventoryIcon(index);
	}

	private void RemoveIfOutOfUses(int index)
	{
		if (GetEffectiveSlot(index).remainingUses <= 0)
		{
			RemoveItemAt(index, localOnly: false);
		}
	}

	[Command]
	private void CmdDecrementUseFromSlotAt(int index)
	{
		if (base.isServer && base.isClient)
		{
			UserCode_CmdDecrementUseFromSlotAt__Int32(index);
			return;
		}
		NetworkWriterPooled writer = NetworkWriterPool.Get();
		writer.WriteVarInt(index);
		SendCommandInternal("System.Void PlayerInventory::CmdDecrementUseFromSlotAt(System.Int32)", 51522030, writer, 0);
		NetworkWriterPool.Return(writer);
	}

	private void UpdateIsUpdateLoopRunning()
	{
		bool flag = isUpdateLoopRunning;
		isUpdateLoopRunning = ShouldRun();
		if (isUpdateLoopRunning != flag)
		{
			LockOnTargetUiManager.ClearTargets();
			ClearAirhornTargets();
			SetLockOnTarget(null);
			if (isUpdateLoopRunning)
			{
				BUpdate.RegisterCallback(this);
			}
			else
			{
				BUpdate.DeregisterCallback(this);
			}
		}
		bool ShouldRun()
		{
			switch (GetEffectivelyEquippedItem(ignoreEquipmentHiding: true))
			{
			case ItemType.Airhorn:
				return !IsUsingItemAtAll;
			case ItemType.ElephantGun:
				if (PlayerInfo.Movement.DiveType == DiveType.ElephantGunFinalShot)
				{
					return PlayerInfo.Movement.DivingState == DivingState.GettingUp;
				}
				return false;
			case ItemType.RocketLauncher:
				return IsAimingItem;
			case ItemType.OrbitalLaser:
				return !IsUsingItemAtAll;
			default:
				return false;
			}
		}
	}

	private void UpdateIsEquipmentForceHidden()
	{
		bool flag = isEquipmentForceHidden;
		NetworkisEquipmentForceHidden = ShouldBeHidden();
		if (isEquipmentForceHidden != flag)
		{
			if (isEquipmentForceHidden)
			{
				PlayerInfo.SetEquippedItem(ItemType.None);
				PlayerInfo.AnimatorIo.SetEquippedItem(ItemType.None);
			}
			else
			{
				ItemType effectivelyEquippedItem = GetEffectivelyEquippedItem();
				PlayerInfo.SetEquippedItem(effectivelyEquippedItem);
				PlayerInfo.AnimatorIo.SetEquippedItem(effectivelyEquippedItem);
			}
			UpdateEquipmentSwitchers();
			PlayerInfo.Cosmetics.UpdateSpringBootsEnabled();
			this.EquippedItemChanged?.Invoke();
		}
		bool ShouldBeHidden()
		{
			if (isEquipmentForceHiddenFromConsole)
			{
				return true;
			}
			if (PlayerInfo.ActiveGolfCartSeat.IsValid())
			{
				return true;
			}
			if (PlayerInfo.Movement.IsRespawning)
			{
				return true;
			}
			if (PlayerInfo.Movement.IsKnockedOutOrRecovering)
			{
				return true;
			}
			if (PlayerInfo.Movement.DivingState != DivingState.None && !PlayerInfo.Movement.DiveType.IsElephantGunDive())
			{
				return true;
			}
			return false;
		}
	}

	private void UpdateIsAimingItem()
	{
		if (isWaitingForAltUseInputRelease && !PlayerInfo.Input.IsHoldingAimSwing)
		{
			isWaitingForAltUseInputRelease = false;
		}
		InventorySlot equippedSlot = GetEffectiveSlot(EquippedItemIndex);
		ItemType equippedItem = equippedSlot.itemType;
		bool isAimingItem = IsAimingItem;
		IsAimingItem = ShouldAim();
		if (isAimingItem == IsAimingItem)
		{
			return;
		}
		if (IsAimingItem)
		{
			GameplayCameraManager.EnterSwingAimCamera();
			if (equippedItem == ItemType.DuelingPistol || equippedItem == ItemType.ElephantGun || equippedItem == ItemType.RocketLauncher)
			{
				PlayerInfo.PlayerAudio.PlayGunAimForAllClients(equippedItem);
			}
			PlayerInfo.CancelEmote(canHideEmoteMenu: false);
			CancelItemFlourish();
		}
		else
		{
			GameplayCameraManager.ExitSwingAimCamera();
		}
		UpdateAimingReticle();
		UpdateIsUpdateLoopRunning();
		PlayerInfo.SetIsAimingItem(IsAimingItem);
		PlayerInfo.Movement.InformIsAimingItemChanged();
		PlayerInfo.AnimatorIo.SetIsAimingItem(IsAimingItem);
		PlayerInventory.LocalPlayerIsAimingItemChanged?.Invoke();
		bool ShouldAim()
		{
			if (equippedItem == ItemType.None)
			{
				return false;
			}
			if (IsUsingItemAtAll && (equippedItem != ItemType.ElephantGun || equippedSlot.remainingUses <= 0 || BMath.GetTimeSince(ItemUseTimestamp) < GameManager.ItemSettings.ElephantGunShotDuration))
			{
				return false;
			}
			if (!PlayerInfo.Input.IsHoldingAimSwing)
			{
				return false;
			}
			if (PlayerInfo.AsGolfer.IsMatchResolved)
			{
				return false;
			}
			if (PlayerInfo.AsSpectator.IsSpectating)
			{
				return false;
			}
			if (PlayerInfo.Movement.IsRespawning)
			{
				return false;
			}
			if (PlayerInfo.Movement.IsKnockedOutOrRecovering)
			{
				return false;
			}
			if (RadialMenu.IsVisible)
			{
				return false;
			}
			return true;
		}
	}

	private void MarkThrownItem(ThrownItemHand thrownItem)
	{
		this.thrownItem |= thrownItem;
		UpdateEquipmentSwitchers();
	}

	private void UpdateEquipmentSwitchers()
	{
		if (!CanHoldEquipment())
		{
			PlayerInfo.RightHandEquipmentSwitcher.SetEquipment(EquipmentType.None);
			PlayerInfo.LeftHandEquipmentSwitcher.SetEquipment(EquipmentType.None);
			return;
		}
		var (equipmentType, equipmentType2) = GetEffectivelyEquippedItem() switch
		{
			ItemType.None => (EquipmentType.GolfClub, EquipmentType.None), 
			ItemType.Coffee => (EquipmentType.CoffeeMug, EquipmentType.CoffeePlate), 
			ItemType.DuelingPistol => (EquipmentType.None, EquipmentType.DuelingPistol), 
			ItemType.ElephantGun => (EquipmentType.ElephantGun, EquipmentType.None), 
			ItemType.Airhorn => (EquipmentType.Airhorn, EquipmentType.None), 
			ItemType.GolfCart => shouldSplitGolfCartBriefcase ? (EquipmentType.GolfCartBase, EquipmentType.GolfCartLid) : (EquipmentType.GolfCartWhole, EquipmentType.None), 
			ItemType.RocketLauncher => (EquipmentType.RocketLauncher, EquipmentType.None), 
			ItemType.Landmine => (EquipmentType.Landmine, EquipmentType.None), 
			ItemType.Electromagnet => (EquipmentType.Electromagnet, EquipmentType.None), 
			ItemType.OrbitalLaser => (EquipmentType.OrbitalLaser, EquipmentType.None), 
			_ => (EquipmentType.None, EquipmentType.None), 
		};
		PlayerInfo.RightHandEquipmentSwitcher.SetEquipment((!thrownItem.HasFlag(ThrownItemHand.Right)) ? equipmentType : EquipmentType.None);
		PlayerInfo.LeftHandEquipmentSwitcher.SetEquipment((!thrownItem.HasFlag(ThrownItemHand.Left)) ? equipmentType2 : EquipmentType.None);
		bool CanHoldEquipment()
		{
			if (isEquipmentForceHidden)
			{
				return false;
			}
			if (!PlayerInfo.Movement.IsVisible)
			{
				return false;
			}
			if (PlayerInfo.AsGolfer.IsMatchResolved && PlayerInfo.AsGolfer.MatchResolution != PlayerMatchResolution.Eliminated)
			{
				return false;
			}
			if (PlayerInfo.IsPlayingEmote)
			{
				return false;
			}
			return true;
		}
	}

	private void ClearAirhornTargets()
	{
		foreach (PlayerInfo airhornTarget in airhornTargets)
		{
			LockOnTargetUiManager.RemoveTarget(airhornTarget.AsEntity.AsLockOnTarget);
		}
		airhornTargets.Clear();
	}

	private void UpdateAimingReticle()
	{
		if (!IsAimingItem)
		{
			ReticleManager.Clear();
			return;
		}
		switch (GetEffectivelyEquippedItem())
		{
		case ItemType.DuelingPistol:
			ReticleManager.SetDuelingPistol();
			break;
		case ItemType.ElephantGun:
			ReticleManager.SetElephantGun();
			break;
		case ItemType.RocketLauncher:
			ReticleManager.SetRocketLauncher();
			break;
		default:
			ReticleManager.Clear();
			break;
		}
	}

	private ItemUseId IncrementAndGetCurrentItemUseId(ItemType itemType)
	{
		return new ItemUseId(PlayerInfo.PlayerId.Guid, ++latestItemUseIndex, itemType);
	}

	private InventorySlot GetEffectiveSlot(int index)
	{
		if (index < 0)
		{
			return default(InventorySlot);
		}
		if (index >= slots.Count)
		{
			return default(InventorySlot);
		}
		if (base.isLocalPlayer && localPlayerSlotOverrides.TryGetValue(index, out var value))
		{
			return value;
		}
		return slots[index];
	}

	[Command]
	private void CmdPlayAirhornVfxForAllClients(NetworkConnectionToClient sender = null)
	{
		if (base.isServer && base.isClient)
		{
			UserCode_CmdPlayAirhornVfxForAllClients__NetworkConnectionToClient(sender);
			return;
		}
		NetworkWriterPooled writer = NetworkWriterPool.Get();
		SendCommandInternal("System.Void PlayerInventory::CmdPlayAirhornVfxForAllClients(Mirror.NetworkConnectionToClient)", 1058108026, writer, 0);
		NetworkWriterPool.Return(writer);
	}

	[TargetRpc]
	private void RpcPlayAirhornVfx(NetworkConnectionToClient connection)
	{
		NetworkWriterPooled writer = NetworkWriterPool.Get();
		SendTargetRPCInternal(connection, "System.Void PlayerInventory::RpcPlayAirhornVfx(Mirror.NetworkConnectionToClient)", -1705709405, writer, 0);
		NetworkWriterPool.Return(writer);
	}

	private void PlayAirhornVfxInternal()
	{
		if (airhornVfx != null)
		{
			airhornVfx.ReturnToPool();
		}
		if (VfxPersistentData.TryGetPooledVfx(VfxType.AirhornActivation, out airhornVfx))
		{
			airhornVfx.transform.SetParent(PlayerInfo.RightHandEquipmentSwitcher.transform);
			airhornVfx.transform.SetLocalPositionAndRotation(Vector3.zero, Quaternion.identity);
			airhornVfx.Play();
		}
	}

	[Command]
	private void CmdCancelAirhornVfxForAllClients(NetworkConnectionToClient sender = null)
	{
		if (base.isServer && base.isClient)
		{
			UserCode_CmdCancelAirhornVfxForAllClients__NetworkConnectionToClient(sender);
			return;
		}
		NetworkWriterPooled writer = NetworkWriterPool.Get();
		SendCommandInternal("System.Void PlayerInventory::CmdCancelAirhornVfxForAllClients(Mirror.NetworkConnectionToClient)", 1137145346, writer, 0);
		NetworkWriterPool.Return(writer);
	}

	[TargetRpc]
	private void RpcCancelAirhornVfx(NetworkConnectionToClient connection)
	{
		NetworkWriterPooled writer = NetworkWriterPool.Get();
		SendTargetRPCInternal(connection, "System.Void PlayerInventory::RpcCancelAirhornVfx(Mirror.NetworkConnectionToClient)", -1080488721, writer, 0);
		NetworkWriterPool.Return(writer);
	}

	private void CancelAirhornVfxInternal()
	{
		if (airhornVfx != null)
		{
			airhornVfx.ReturnToPool();
		}
	}

	private void OnItemSlotsChanged(SyncList<InventorySlot>.Operation operation, int index, InventorySlot slot)
	{
		if (!base.isLocalPlayer)
		{
			if ((uint)operation == 1u)
			{
				this.ItemSlotSet?.Invoke(index);
			}
			return;
		}
		if ((uint)operation != 4u)
		{
			localPlayerSlotOverrides.Remove(index);
		}
		if (ShouldDeselect())
		{
			TryDeselectItem();
		}
		Hotkeys.UpdatePlayerInventoryIcon(index);
		bool flag = false;
		foreach (InventorySlot slot2 in slots)
		{
			if (slot2.itemType != ItemType.None)
			{
				flag = true;
				break;
			}
		}
		if (flag)
		{
			TutorialManager.AllowPromptCategory(TutorialPromptCategory.Item);
		}
		else
		{
			TutorialManager.DisallowPromptCategory(TutorialPromptCategory.Item);
		}
		if ((uint)operation == 1u)
		{
			this.ItemSlotSet?.Invoke(index);
		}
		bool ShouldDeselect()
		{
			if ((uint)operation != 1u)
			{
				return false;
			}
			if (index != EquippedItemIndex)
			{
				return false;
			}
			if (slots[index].itemType != ItemType.None)
			{
				return false;
			}
			return true;
		}
	}

	private void OnLocalPlayerIsVisibleChanged()
	{
		UpdateEquipmentSwitchers();
	}

	private void OnLocalPlayerIsKnockedOutOrRecoveringChanged()
	{
		UpdateIsEquipmentForceHidden();
		UpdateIsAimingItem();
		if (PlayerInfo.Movement.IsKnockedOutOrRecovering)
		{
			CancelItemUse();
			CancelSpringBootsUse();
			CancelEnterPlacedGolfCartRoutine();
		}
	}

	private void OnLocalPlayerIsRespawningChanged()
	{
		UpdateIsEquipmentForceHidden();
		UpdateIsAimingItem();
		if (PlayerInfo.Movement.IsRespawning)
		{
			CancelItemUse();
			CancelSpringBootsUse();
			CancelEnterPlacedGolfCartRoutine();
		}
	}

	private void OnLocalPlayerMatchResolutionChanged(PlayerMatchResolution previousResolution, PlayerMatchResolution currentResolution)
	{
		if (currentResolution == PlayerMatchResolution.Eliminated)
		{
			TryDeselectItem();
		}
		UpdateEquipmentSwitchers();
		UpdateIsAimingItem();
	}

	private void OnLocalPlayerIsSpectatingChanged()
	{
		UpdateIsAimingItem();
	}

	private void OnLockOnTargetWillBeDestroyed()
	{
		SetLockOnTarget(null);
	}

	private void OnLockOnTargetPlayerIsVisibleChanged()
	{
		if (!LockOnTarget.IsValid())
		{
			SetLockOnTarget(null);
		}
	}

	private void OnLockOnTargetPlayerMatchResolutionChanged(PlayerMatchResolution previousResolution, PlayerMatchResolution currentResolution)
	{
		if (!LockOnTarget.IsValid())
		{
			SetLockOnTarget(null);
		}
	}

	public PlayerInventory()
	{
		InitSyncObject(slots);
	}

	static PlayerInventory()
	{
		processedPlayerBuffer = new HashSet<PlayerInfo>();
		playersToRemoveBuffer = new HashSet<PlayerInfo>();
		processedHittableBuffer = new HashSet<Hittable>();
		RemoteProcedureCalls.RegisterCommand(typeof(PlayerInventory), "System.Void PlayerInventory::CmdAddItem(ItemType)", InvokeUserCode_CmdAddItem__ItemType, requiresAuthority: true);
		RemoteProcedureCalls.RegisterCommand(typeof(PlayerInventory), "System.Void PlayerInventory::CmdPlaySpringBootsActivationForAllClients(UnityEngine.Vector3,Mirror.NetworkConnectionToClient)", InvokeUserCode_CmdPlaySpringBootsActivationForAllClients__Vector3__NetworkConnectionToClient, requiresAuthority: true);
		RemoteProcedureCalls.RegisterCommand(typeof(PlayerInventory), "System.Void PlayerInventory::CmdDropItemAt(System.Int32,UnityEngine.Vector3,UnityEngine.Vector3,ItemUseId)", InvokeUserCode_CmdDropItemAt__Int32__Vector3__Vector3__ItemUseId, requiresAuthority: true);
		RemoteProcedureCalls.RegisterCommand(typeof(PlayerInventory), "System.Void PlayerInventory::CmdRemoveItemAt(System.Int32)", InvokeUserCode_CmdRemoveItemAt__Int32, requiresAuthority: true);
		RemoteProcedureCalls.RegisterCommand(typeof(PlayerInventory), "System.Void PlayerInventory::CmdPlayGolfCartBriefcaseEffectsForAllClients(System.Boolean,Mirror.NetworkConnectionToClient)", InvokeUserCode_CmdPlayGolfCartBriefcaseEffectsForAllClients__Boolean__NetworkConnectionToClient, requiresAuthority: true);
		RemoteProcedureCalls.RegisterCommand(typeof(PlayerInventory), "System.Void PlayerInventory::CmdSpawnLandmine(UnityEngine.Vector3,UnityEngine.Quaternion,UnityEngine.Vector3,UnityEngine.Vector3,LandmineArmType,ItemUseId,Mirror.NetworkConnectionToClient)", InvokeUserCode_CmdSpawnLandmine__Vector3__Quaternion__Vector3__Vector3__LandmineArmType__ItemUseId__NetworkConnectionToClient, requiresAuthority: true);
		RemoteProcedureCalls.RegisterCommand(typeof(PlayerInventory), "System.Void PlayerInventory::CmdActivateOrbitalLaser(Hittable,UnityEngine.Vector3,ItemUseId)", InvokeUserCode_CmdActivateOrbitalLaser__Hittable__Vector3__ItemUseId, requiresAuthority: true);
		RemoteProcedureCalls.RegisterCommand(typeof(PlayerInventory), "System.Void PlayerInventory::CmdThrowUsedItemForAllClients(ThrownUsedItemType,System.Boolean,UnityEngine.Vector3,Mirror.NetworkConnectionToClient)", InvokeUserCode_CmdThrowUsedItemForAllClients__ThrownUsedItemType__Boolean__Vector3__NetworkConnectionToClient, requiresAuthority: true);
		RemoteProcedureCalls.RegisterCommand(typeof(PlayerInventory), "System.Void PlayerInventory::CmdInformInterruptedPlayerWithAirhorn(Mirror.NetworkConnectionToClient)", InvokeUserCode_CmdInformInterruptedPlayerWithAirhorn__NetworkConnectionToClient, requiresAuthority: false);
		RemoteProcedureCalls.RegisterCommand(typeof(PlayerInventory), "System.Void PlayerInventory::CmdInformShotRocket(UnityEngine.Vector3,UnityEngine.Quaternion,Hittable,ItemUseId,Mirror.NetworkConnectionToClient)", InvokeUserCode_CmdInformShotRocket__Vector3__Quaternion__Hittable__ItemUseId__NetworkConnectionToClient, requiresAuthority: true);
		RemoteProcedureCalls.RegisterCommand(typeof(PlayerInventory), "System.Void PlayerInventory::CmdActivatedElectromagnet(Mirror.NetworkConnectionToClient)", InvokeUserCode_CmdActivatedElectromagnet__NetworkConnectionToClient, requiresAuthority: true);
		RemoteProcedureCalls.RegisterCommand(typeof(PlayerInventory), "System.Void PlayerInventory::CmdActivatedOrbitalLaser(Mirror.NetworkConnectionToClient)", InvokeUserCode_CmdActivatedOrbitalLaser__NetworkConnectionToClient, requiresAuthority: true);
		RemoteProcedureCalls.RegisterCommand(typeof(PlayerInventory), "System.Void PlayerInventory::CmdDecrementUseFromSlotAt(System.Int32)", InvokeUserCode_CmdDecrementUseFromSlotAt__Int32, requiresAuthority: true);
		RemoteProcedureCalls.RegisterCommand(typeof(PlayerInventory), "System.Void PlayerInventory::CmdPlayAirhornVfxForAllClients(Mirror.NetworkConnectionToClient)", InvokeUserCode_CmdPlayAirhornVfxForAllClients__NetworkConnectionToClient, requiresAuthority: true);
		RemoteProcedureCalls.RegisterCommand(typeof(PlayerInventory), "System.Void PlayerInventory::CmdCancelAirhornVfxForAllClients(Mirror.NetworkConnectionToClient)", InvokeUserCode_CmdCancelAirhornVfxForAllClients__NetworkConnectionToClient, requiresAuthority: true);
		RemoteProcedureCalls.RegisterRpc(typeof(PlayerInventory), "System.Void PlayerInventory::RpcPlaySpringBootsActivation(Mirror.NetworkConnectionToClient,UnityEngine.Vector3)", InvokeUserCode_RpcPlaySpringBootsActivation__NetworkConnectionToClient__Vector3);
		RemoteProcedureCalls.RegisterRpc(typeof(PlayerInventory), "System.Void PlayerInventory::RpcPlayGolfCartBriefcaseEffects(Mirror.NetworkConnectionToClient,System.Boolean)", InvokeUserCode_RpcPlayGolfCartBriefcaseEffects__NetworkConnectionToClient__Boolean);
		RemoteProcedureCalls.RegisterRpc(typeof(PlayerInventory), "System.Void PlayerInventory::RpcThrowUsedItem(Mirror.NetworkConnectionToClient,ThrownUsedItemType,System.Boolean,UnityEngine.Vector3)", InvokeUserCode_RpcThrowUsedItem__NetworkConnectionToClient__ThrownUsedItemType__Boolean__Vector3);
		RemoteProcedureCalls.RegisterRpc(typeof(PlayerInventory), "System.Void PlayerInventory::RpcInformShotRocket(Mirror.NetworkConnectionToClient)", InvokeUserCode_RpcInformShotRocket__NetworkConnectionToClient);
		RemoteProcedureCalls.RegisterRpc(typeof(PlayerInventory), "System.Void PlayerInventory::RpcInformActivatedElectromagnet(Mirror.NetworkConnectionToClient)", InvokeUserCode_RpcInformActivatedElectromagnet__NetworkConnectionToClient);
		RemoteProcedureCalls.RegisterRpc(typeof(PlayerInventory), "System.Void PlayerInventory::RpcInformActivatedOrbitalLaser(Mirror.NetworkConnectionToClient)", InvokeUserCode_RpcInformActivatedOrbitalLaser__NetworkConnectionToClient);
		RemoteProcedureCalls.RegisterRpc(typeof(PlayerInventory), "System.Void PlayerInventory::RpcPlayAirhornVfx(Mirror.NetworkConnectionToClient)", InvokeUserCode_RpcPlayAirhornVfx__NetworkConnectionToClient);
		RemoteProcedureCalls.RegisterRpc(typeof(PlayerInventory), "System.Void PlayerInventory::RpcCancelAirhornVfx(Mirror.NetworkConnectionToClient)", InvokeUserCode_RpcCancelAirhornVfx__NetworkConnectionToClient);
	}

	public override bool Weaved()
	{
		return true;
	}

	protected void UserCode_CmdAddItem__ItemType(ItemType item)
	{
		if (serverAddItemCheatCommandRateLimiter.RegisterHit())
		{
			if (!GameManager.AllItems.TryGetItemData(item, out var itemData))
			{
				Debug.LogError($"Could not find data for item {item}");
			}
			else
			{
				ServerTryAddItem(item, itemData.MaxUses);
			}
		}
	}

	protected static void InvokeUserCode_CmdAddItem__ItemType(NetworkBehaviour obj, NetworkReader reader, NetworkConnectionToClient senderConnection)
	{
		if (!NetworkServer.active)
		{
			Debug.LogError("Command CmdAddItem called on client.");
		}
		else
		{
			((PlayerInventory)obj).UserCode_CmdAddItem__ItemType(GeneratedNetworkCode._Read_ItemType(reader));
		}
	}

	protected void UserCode_CmdPlaySpringBootsActivationForAllClients__Vector3__NetworkConnectionToClient(Vector3 position, NetworkConnectionToClient sender)
	{
		if (!serverSpringBootsActivationCommandRateLimiter.RegisterHit())
		{
			return;
		}
		if (sender != null && sender != NetworkServer.localConnection)
		{
			PlaySpringBootsActivationInternal(position);
		}
		foreach (NetworkConnectionToClient value in NetworkServer.connections.Values)
		{
			if (value != NetworkServer.localConnection && value != sender)
			{
				RpcPlaySpringBootsActivation(value, position);
			}
		}
	}

	protected static void InvokeUserCode_CmdPlaySpringBootsActivationForAllClients__Vector3__NetworkConnectionToClient(NetworkBehaviour obj, NetworkReader reader, NetworkConnectionToClient senderConnection)
	{
		if (!NetworkServer.active)
		{
			Debug.LogError("Command CmdPlaySpringBootsActivationForAllClients called on client.");
		}
		else
		{
			((PlayerInventory)obj).UserCode_CmdPlaySpringBootsActivationForAllClients__Vector3__NetworkConnectionToClient(reader.ReadVector3(), senderConnection);
		}
	}

	protected void UserCode_RpcPlaySpringBootsActivation__NetworkConnectionToClient__Vector3(NetworkConnectionToClient connection, Vector3 position)
	{
		PlaySpringBootsActivationInternal(position);
	}

	protected static void InvokeUserCode_RpcPlaySpringBootsActivation__NetworkConnectionToClient__Vector3(NetworkBehaviour obj, NetworkReader reader, NetworkConnectionToClient senderConnection)
	{
		if (!NetworkClient.active)
		{
			Debug.LogError("TargetRPC RpcPlaySpringBootsActivation called on server.");
		}
		else
		{
			((PlayerInventory)obj).UserCode_RpcPlaySpringBootsActivation__NetworkConnectionToClient__Vector3(null, reader.ReadVector3());
		}
	}

	protected void UserCode_CmdDropItemAt__Int32__Vector3__Vector3__ItemUseId(int index, Vector3 playerVelocity, Vector3 playerLocalAngularVelocity, ItemUseId itemUseId)
	{
		if (!serverDropItemCommandRateLimiter.RegisterHit())
		{
			return;
		}
		if (index < 0 || index >= slots.Count)
		{
			Debug.LogError($"Attempted to remove item at index {index}, but it's out of range of {slots.Count} slots", base.gameObject);
			return;
		}
		InventorySlot inventorySlot = slots[index];
		RemoveItemAt(index, localOnly: false);
		if (inventorySlot.itemType != ItemType.None && inventorySlot.remainingUses > 0)
		{
			Quaternion rotation;
			if (GameManager.AllItems.TryGetItemData(inventorySlot.itemType, out var itemData))
			{
				rotation = base.transform.rotation * itemData.DroppedLocalRotation;
			}
			else
			{
				Debug.LogError($"Could not find data for item {inventorySlot.itemType}");
				rotation = base.transform.rotation;
			}
			Vector3 objectAngularVelocity = base.transform.TransformVector(playerLocalAngularVelocity);
			Vector3 vector = base.transform.position + Vector3.up * GameManager.PlayerInventorySettings.DropItemVerticalOffset + base.transform.right * GameManager.GolfSettings.SwingHitBoxLocalCenter.x;
			Vector3 velocity = vector.GetPointVelocity(PlayerInfo.Rigidbody.worldCenterOfMass, playerVelocity, objectAngularVelocity) * 0.25f;
			Vector3 angularVelocity = ((velocity.sqrMagnitude > 0.001f) ? (Vector3.Cross(Vector3.up, velocity.normalized) * 3f) : Vector3.zero);
			vector += (float)base.connectionToClient.rtt * playerVelocity;
			CourseManager.ServerSpawnItem(inventorySlot.itemType, inventorySlot.remainingUses, vector, rotation, velocity, angularVelocity, itemUseId, this);
		}
	}

	protected static void InvokeUserCode_CmdDropItemAt__Int32__Vector3__Vector3__ItemUseId(NetworkBehaviour obj, NetworkReader reader, NetworkConnectionToClient senderConnection)
	{
		if (!NetworkServer.active)
		{
			Debug.LogError("Command CmdDropItemAt called on client.");
		}
		else
		{
			((PlayerInventory)obj).UserCode_CmdDropItemAt__Int32__Vector3__Vector3__ItemUseId(reader.ReadVarInt(), reader.ReadVector3(), reader.ReadVector3(), GeneratedNetworkCode._Read_ItemUseId(reader));
		}
	}

	protected void UserCode_CmdRemoveItemAt__Int32(int index)
	{
		if (serverRemoveItemCommandRateLimiter.RegisterHit())
		{
			RemoveItemAt(index, localOnly: false);
		}
	}

	protected static void InvokeUserCode_CmdRemoveItemAt__Int32(NetworkBehaviour obj, NetworkReader reader, NetworkConnectionToClient senderConnection)
	{
		if (!NetworkServer.active)
		{
			Debug.LogError("Command CmdRemoveItemAt called on client.");
		}
		else
		{
			((PlayerInventory)obj).UserCode_CmdRemoveItemAt__Int32(reader.ReadVarInt());
		}
	}

	protected void UserCode_CmdPlayGolfCartBriefcaseEffectsForAllClients__Boolean__NetworkConnectionToClient(bool isStart, NetworkConnectionToClient sender)
	{
		if (!serverGolfCartBriefcaseEffectsCommandRateLimiter.RegisterHit())
		{
			return;
		}
		if (sender != null && sender != NetworkServer.localConnection)
		{
			PlayGolfCartBriefcaseEffectsInternal(isStart);
		}
		foreach (NetworkConnectionToClient value in NetworkServer.connections.Values)
		{
			if (value != NetworkServer.localConnection && value != sender)
			{
				RpcPlayGolfCartBriefcaseEffects(value, isStart);
			}
		}
	}

	protected static void InvokeUserCode_CmdPlayGolfCartBriefcaseEffectsForAllClients__Boolean__NetworkConnectionToClient(NetworkBehaviour obj, NetworkReader reader, NetworkConnectionToClient senderConnection)
	{
		if (!NetworkServer.active)
		{
			Debug.LogError("Command CmdPlayGolfCartBriefcaseEffectsForAllClients called on client.");
		}
		else
		{
			((PlayerInventory)obj).UserCode_CmdPlayGolfCartBriefcaseEffectsForAllClients__Boolean__NetworkConnectionToClient(reader.ReadBool(), senderConnection);
		}
	}

	protected void UserCode_RpcPlayGolfCartBriefcaseEffects__NetworkConnectionToClient__Boolean(NetworkConnectionToClient connection, bool isStart)
	{
		PlayGolfCartBriefcaseEffectsInternal(isStart);
	}

	protected static void InvokeUserCode_RpcPlayGolfCartBriefcaseEffects__NetworkConnectionToClient__Boolean(NetworkBehaviour obj, NetworkReader reader, NetworkConnectionToClient senderConnection)
	{
		if (!NetworkClient.active)
		{
			Debug.LogError("TargetRPC RpcPlayGolfCartBriefcaseEffects called on server.");
		}
		else
		{
			((PlayerInventory)obj).UserCode_RpcPlayGolfCartBriefcaseEffects__NetworkConnectionToClient__Boolean(null, reader.ReadBool());
		}
	}

	protected void UserCode_CmdSpawnLandmine__Vector3__Quaternion__Vector3__Vector3__LandmineArmType__ItemUseId__NetworkConnectionToClient(Vector3 position, Quaternion rotation, Vector3 velocity, Vector3 angularVelocity, LandmineArmType armType, ItemUseId itemUseId, NetworkConnectionToClient sender)
	{
		if (serverSpawnLandmineCommandRateLimiter.RegisterHit(sender))
		{
			CourseManager.ServerSpawnLandmine(position, rotation, velocity, angularVelocity, armType, itemUseId, this);
		}
	}

	protected static void InvokeUserCode_CmdSpawnLandmine__Vector3__Quaternion__Vector3__Vector3__LandmineArmType__ItemUseId__NetworkConnectionToClient(NetworkBehaviour obj, NetworkReader reader, NetworkConnectionToClient senderConnection)
	{
		if (!NetworkServer.active)
		{
			Debug.LogError("Command CmdSpawnLandmine called on client.");
		}
		else
		{
			((PlayerInventory)obj).UserCode_CmdSpawnLandmine__Vector3__Quaternion__Vector3__Vector3__LandmineArmType__ItemUseId__NetworkConnectionToClient(reader.ReadVector3(), reader.ReadQuaternion(), reader.ReadVector3(), reader.ReadVector3(), GeneratedNetworkCode._Read_LandmineArmType(reader), GeneratedNetworkCode._Read_ItemUseId(reader), senderConnection);
		}
	}

	protected void UserCode_CmdActivateOrbitalLaser__Hittable__Vector3__ItemUseId(Hittable target, Vector3 fallbackWorldPosition, ItemUseId itemUseId)
	{
		if (serverActivateOrbitalLaserCommandRateLimiter.RegisterHit())
		{
			OrbitalLaserManager.ServerActivateLaser(target, fallbackWorldPosition, this, itemUseId);
		}
	}

	protected static void InvokeUserCode_CmdActivateOrbitalLaser__Hittable__Vector3__ItemUseId(NetworkBehaviour obj, NetworkReader reader, NetworkConnectionToClient senderConnection)
	{
		if (!NetworkServer.active)
		{
			Debug.LogError("Command CmdActivateOrbitalLaser called on client.");
		}
		else
		{
			((PlayerInventory)obj).UserCode_CmdActivateOrbitalLaser__Hittable__Vector3__ItemUseId(reader.ReadNetworkBehaviour<Hittable>(), reader.ReadVector3(), GeneratedNetworkCode._Read_ItemUseId(reader));
		}
	}

	protected void UserCode_CmdThrowUsedItemForAllClients__ThrownUsedItemType__Boolean__Vector3__NetworkConnectionToClient(ThrownUsedItemType thrownItemType, bool forcePlayerPosition, Vector3 forcedPlayerPosition, NetworkConnectionToClient sender)
	{
		if (!serverThrowUsedItemCommandRateLimiter.RegisterHit())
		{
			return;
		}
		if (sender != null && sender != NetworkServer.localConnection)
		{
			ThrowUsedItemInternal(thrownItemType, forcePlayerPosition, forcedPlayerPosition);
		}
		foreach (NetworkConnectionToClient value in NetworkServer.connections.Values)
		{
			if (value != NetworkServer.localConnection && value != sender)
			{
				RpcThrowUsedItem(value, thrownItemType, forcePlayerPosition, forcedPlayerPosition);
			}
		}
	}

	protected static void InvokeUserCode_CmdThrowUsedItemForAllClients__ThrownUsedItemType__Boolean__Vector3__NetworkConnectionToClient(NetworkBehaviour obj, NetworkReader reader, NetworkConnectionToClient senderConnection)
	{
		if (!NetworkServer.active)
		{
			Debug.LogError("Command CmdThrowUsedItemForAllClients called on client.");
		}
		else
		{
			((PlayerInventory)obj).UserCode_CmdThrowUsedItemForAllClients__ThrownUsedItemType__Boolean__Vector3__NetworkConnectionToClient(GeneratedNetworkCode._Read_ThrownUsedItemType(reader), reader.ReadBool(), reader.ReadVector3(), senderConnection);
		}
	}

	protected void UserCode_RpcThrowUsedItem__NetworkConnectionToClient__ThrownUsedItemType__Boolean__Vector3(NetworkConnectionToClient connection, ThrownUsedItemType thrownItemType, bool forcePlayerPosition, Vector3 forcedPlayerPosition)
	{
		ThrowUsedItemInternal(thrownItemType, forcePlayerPosition, forcedPlayerPosition);
	}

	protected static void InvokeUserCode_RpcThrowUsedItem__NetworkConnectionToClient__ThrownUsedItemType__Boolean__Vector3(NetworkBehaviour obj, NetworkReader reader, NetworkConnectionToClient senderConnection)
	{
		if (!NetworkClient.active)
		{
			Debug.LogError("TargetRPC RpcThrowUsedItem called on server.");
		}
		else
		{
			((PlayerInventory)obj).UserCode_RpcThrowUsedItem__NetworkConnectionToClient__ThrownUsedItemType__Boolean__Vector3(null, GeneratedNetworkCode._Read_ThrownUsedItemType(reader), reader.ReadBool(), reader.ReadVector3());
		}
	}

	protected void UserCode_CmdInformInterruptedPlayerWithAirhorn__NetworkConnectionToClient(NetworkConnectionToClient sender)
	{
		if (serverInformInterruptedPlayerWithAirhornRateLimiter.RegisterHit(sender))
		{
			PlayerInfo.RpcInformInterruptedPlayerWithAirhorn();
		}
	}

	protected static void InvokeUserCode_CmdInformInterruptedPlayerWithAirhorn__NetworkConnectionToClient(NetworkBehaviour obj, NetworkReader reader, NetworkConnectionToClient senderConnection)
	{
		if (!NetworkServer.active)
		{
			Debug.LogError("Command CmdInformInterruptedPlayerWithAirhorn called on client.");
		}
		else
		{
			((PlayerInventory)obj).UserCode_CmdInformInterruptedPlayerWithAirhorn__NetworkConnectionToClient(senderConnection);
		}
	}

	protected void UserCode_CmdInformShotRocket__Vector3__Quaternion__Hittable__ItemUseId__NetworkConnectionToClient(Vector3 rocketPosition, Quaternion rocketRotation, Hittable lockOnTargetAsHittable, ItemUseId itemUseId, NetworkConnectionToClient sender)
	{
		if (!serverInformShotRocketCommandRateLimiter.RegisterHit())
		{
			return;
		}
		SpawnRocket();
		if (sender != null && sender != NetworkServer.localConnection)
		{
			OnShotRocket();
		}
		foreach (NetworkConnectionToClient value in NetworkServer.connections.Values)
		{
			if (value != NetworkServer.localConnection && value != sender)
			{
				RpcInformShotRocket(value);
			}
		}
		void SpawnRocket()
		{
			Rocket rocket = UnityEngine.Object.Instantiate(GameManager.ItemSettings.RocketPrefab, rocketPosition, rocketRotation);
			if (rocket == null)
			{
				Debug.LogError("Rocket did not instantiate properly", base.gameObject);
			}
			else
			{
				rocket.ServerInitialize(PlayerInfo, lockOnTargetAsHittable, itemUseId);
				NetworkServer.Spawn(rocket.gameObject);
			}
		}
	}

	protected static void InvokeUserCode_CmdInformShotRocket__Vector3__Quaternion__Hittable__ItemUseId__NetworkConnectionToClient(NetworkBehaviour obj, NetworkReader reader, NetworkConnectionToClient senderConnection)
	{
		if (!NetworkServer.active)
		{
			Debug.LogError("Command CmdInformShotRocket called on client.");
		}
		else
		{
			((PlayerInventory)obj).UserCode_CmdInformShotRocket__Vector3__Quaternion__Hittable__ItemUseId__NetworkConnectionToClient(reader.ReadVector3(), reader.ReadQuaternion(), reader.ReadNetworkBehaviour<Hittable>(), GeneratedNetworkCode._Read_ItemUseId(reader), senderConnection);
		}
	}

	protected void UserCode_RpcInformShotRocket__NetworkConnectionToClient(NetworkConnectionToClient connection)
	{
		OnShotRocket();
	}

	protected static void InvokeUserCode_RpcInformShotRocket__NetworkConnectionToClient(NetworkBehaviour obj, NetworkReader reader, NetworkConnectionToClient senderConnection)
	{
		if (!NetworkClient.active)
		{
			Debug.LogError("TargetRPC RpcInformShotRocket called on server.");
		}
		else
		{
			((PlayerInventory)obj).UserCode_RpcInformShotRocket__NetworkConnectionToClient(null);
		}
	}

	protected void UserCode_CmdActivatedElectromagnet__NetworkConnectionToClient(NetworkConnectionToClient sender)
	{
		if (!serverActivateElectromagnetCommandRateLimiter.RegisterHit())
		{
			return;
		}
		if (sender != null && sender != NetworkServer.localConnection)
		{
			OnActivatedElectromagnet();
		}
		foreach (NetworkConnectionToClient value in NetworkServer.connections.Values)
		{
			if (value != NetworkServer.localConnection && value != sender)
			{
				RpcInformActivatedElectromagnet(value);
			}
		}
	}

	protected static void InvokeUserCode_CmdActivatedElectromagnet__NetworkConnectionToClient(NetworkBehaviour obj, NetworkReader reader, NetworkConnectionToClient senderConnection)
	{
		if (!NetworkServer.active)
		{
			Debug.LogError("Command CmdActivatedElectromagnet called on client.");
		}
		else
		{
			((PlayerInventory)obj).UserCode_CmdActivatedElectromagnet__NetworkConnectionToClient(senderConnection);
		}
	}

	protected void UserCode_RpcInformActivatedElectromagnet__NetworkConnectionToClient(NetworkConnectionToClient connection)
	{
		OnActivatedElectromagnet();
	}

	protected static void InvokeUserCode_RpcInformActivatedElectromagnet__NetworkConnectionToClient(NetworkBehaviour obj, NetworkReader reader, NetworkConnectionToClient senderConnection)
	{
		if (!NetworkClient.active)
		{
			Debug.LogError("TargetRPC RpcInformActivatedElectromagnet called on server.");
		}
		else
		{
			((PlayerInventory)obj).UserCode_RpcInformActivatedElectromagnet__NetworkConnectionToClient(null);
		}
	}

	protected void UserCode_CmdActivatedOrbitalLaser__NetworkConnectionToClient(NetworkConnectionToClient sender)
	{
		if (!serverActivatedOrbitalLaserCommandRateLimiter.RegisterHit())
		{
			return;
		}
		if (sender != null && sender != NetworkServer.localConnection)
		{
			OnActivatedOrbitalLaser();
		}
		foreach (NetworkConnectionToClient value in NetworkServer.connections.Values)
		{
			if (value != NetworkServer.localConnection && value != sender)
			{
				RpcInformActivatedOrbitalLaser(value);
			}
		}
	}

	protected static void InvokeUserCode_CmdActivatedOrbitalLaser__NetworkConnectionToClient(NetworkBehaviour obj, NetworkReader reader, NetworkConnectionToClient senderConnection)
	{
		if (!NetworkServer.active)
		{
			Debug.LogError("Command CmdActivatedOrbitalLaser called on client.");
		}
		else
		{
			((PlayerInventory)obj).UserCode_CmdActivatedOrbitalLaser__NetworkConnectionToClient(senderConnection);
		}
	}

	protected void UserCode_RpcInformActivatedOrbitalLaser__NetworkConnectionToClient(NetworkConnectionToClient connection)
	{
		OnActivatedOrbitalLaser();
	}

	protected static void InvokeUserCode_RpcInformActivatedOrbitalLaser__NetworkConnectionToClient(NetworkBehaviour obj, NetworkReader reader, NetworkConnectionToClient senderConnection)
	{
		if (!NetworkClient.active)
		{
			Debug.LogError("TargetRPC RpcInformActivatedOrbitalLaser called on server.");
		}
		else
		{
			((PlayerInventory)obj).UserCode_RpcInformActivatedOrbitalLaser__NetworkConnectionToClient(null);
		}
	}

	protected void UserCode_CmdDecrementUseFromSlotAt__Int32(int index)
	{
		if (serverDecrementItemUseCommandRateLimiter.RegisterHit())
		{
			DecrementUseFromSlotAt(index);
		}
	}

	protected static void InvokeUserCode_CmdDecrementUseFromSlotAt__Int32(NetworkBehaviour obj, NetworkReader reader, NetworkConnectionToClient senderConnection)
	{
		if (!NetworkServer.active)
		{
			Debug.LogError("Command CmdDecrementUseFromSlotAt called on client.");
		}
		else
		{
			((PlayerInventory)obj).UserCode_CmdDecrementUseFromSlotAt__Int32(reader.ReadVarInt());
		}
	}

	protected void UserCode_CmdPlayAirhornVfxForAllClients__NetworkConnectionToClient(NetworkConnectionToClient sender)
	{
		if (!serverAirhornVfxCommandRateLimiter.RegisterHit())
		{
			return;
		}
		if (sender != null && sender != NetworkServer.localConnection)
		{
			PlayAirhornVfxInternal();
		}
		foreach (NetworkConnectionToClient value in NetworkServer.connections.Values)
		{
			if (value != NetworkServer.localConnection && value != sender)
			{
				RpcPlayAirhornVfx(value);
			}
		}
	}

	protected static void InvokeUserCode_CmdPlayAirhornVfxForAllClients__NetworkConnectionToClient(NetworkBehaviour obj, NetworkReader reader, NetworkConnectionToClient senderConnection)
	{
		if (!NetworkServer.active)
		{
			Debug.LogError("Command CmdPlayAirhornVfxForAllClients called on client.");
		}
		else
		{
			((PlayerInventory)obj).UserCode_CmdPlayAirhornVfxForAllClients__NetworkConnectionToClient(senderConnection);
		}
	}

	protected void UserCode_RpcPlayAirhornVfx__NetworkConnectionToClient(NetworkConnectionToClient connection)
	{
		PlayAirhornVfxInternal();
	}

	protected static void InvokeUserCode_RpcPlayAirhornVfx__NetworkConnectionToClient(NetworkBehaviour obj, NetworkReader reader, NetworkConnectionToClient senderConnection)
	{
		if (!NetworkClient.active)
		{
			Debug.LogError("TargetRPC RpcPlayAirhornVfx called on server.");
		}
		else
		{
			((PlayerInventory)obj).UserCode_RpcPlayAirhornVfx__NetworkConnectionToClient(null);
		}
	}

	protected void UserCode_CmdCancelAirhornVfxForAllClients__NetworkConnectionToClient(NetworkConnectionToClient sender)
	{
		if (!serverCancelAirhornVfxCommandRateLimiter.RegisterHit())
		{
			return;
		}
		if (sender != null && sender != NetworkServer.localConnection)
		{
			CancelAirhornVfxInternal();
		}
		foreach (NetworkConnectionToClient value in NetworkServer.connections.Values)
		{
			if (value != NetworkServer.localConnection && value != sender)
			{
				RpcCancelAirhornVfx(value);
			}
		}
	}

	protected static void InvokeUserCode_CmdCancelAirhornVfxForAllClients__NetworkConnectionToClient(NetworkBehaviour obj, NetworkReader reader, NetworkConnectionToClient senderConnection)
	{
		if (!NetworkServer.active)
		{
			Debug.LogError("Command CmdCancelAirhornVfxForAllClients called on client.");
		}
		else
		{
			((PlayerInventory)obj).UserCode_CmdCancelAirhornVfxForAllClients__NetworkConnectionToClient(senderConnection);
		}
	}

	protected void UserCode_RpcCancelAirhornVfx__NetworkConnectionToClient(NetworkConnectionToClient connection)
	{
		CancelAirhornVfxInternal();
	}

	protected static void InvokeUserCode_RpcCancelAirhornVfx__NetworkConnectionToClient(NetworkBehaviour obj, NetworkReader reader, NetworkConnectionToClient senderConnection)
	{
		if (!NetworkClient.active)
		{
			Debug.LogError("TargetRPC RpcCancelAirhornVfx called on server.");
		}
		else
		{
			((PlayerInventory)obj).UserCode_RpcCancelAirhornVfx__NetworkConnectionToClient(null);
		}
	}

	public override void SerializeSyncVars(NetworkWriter writer, bool forceAll)
	{
		base.SerializeSyncVars(writer, forceAll);
		if (forceAll)
		{
			writer.WriteBool(isEquipmentForceHidden);
			return;
		}
		writer.WriteVarULong(syncVarDirtyBits);
		if ((syncVarDirtyBits & 1L) != 0L)
		{
			writer.WriteBool(isEquipmentForceHidden);
		}
	}

	public override void DeserializeSyncVars(NetworkReader reader, bool initialState)
	{
		base.DeserializeSyncVars(reader, initialState);
		if (initialState)
		{
			GeneratedSyncVarDeserialize(ref isEquipmentForceHidden, null, reader.ReadBool());
			return;
		}
		long num = (long)reader.ReadVarULong();
		if ((num & 1L) != 0L)
		{
			GeneratedSyncVarDeserialize(ref isEquipmentForceHidden, null, reader.ReadBool());
		}
	}
}
