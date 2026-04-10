using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using Brimstone.Geometry;
using Mirror;
using Mirror.RemoteCalls;
using Steamworks;
using UnityEngine;

public class PlayerInfo : NetworkBehaviour
{
	[SyncVar]
	private bool networkedIsAimingSwing;

	[SyncVar(hook = "OnIsPlayingOverchargedVfxChanged")]
	private bool shouldPlayOverchargedVfx;

	[SyncVar]
	private bool networkedIsAimingItem;

	[SyncVar(hook = "OnNetworkedEquippedItemIndexChanged")]
	private int networkedEquippedItemIndex;

	[SyncVar(hook = "OnNetworkedEquippedItemChanged")]
	private ItemType networkedEquippedItem;

	[SyncVar(hook = "OnNetworkedRocketDriverThrustPowerChanged")]
	private float networkedRocketDriverNormalizedCharge;

	[SyncVar(hook = "OnIsElectromagnetShieldActiveChanged")]
	private bool isElectromagnetShieldActive;

	[SyncVar]
	private ItemUseId electromagnetShieldItemUseId = ItemUseId.Invalid;

	private double localPlayerElectromagnetShieldActivationTimestamp = double.MinValue;

	private Coroutine localPlayerElectromagnetRoutine;

	[SyncVar(hook = "OnActiveGolfCartSeatChanged")]
	private GolfCartSeat activeGolfCartSeat;

	private bool isFirstPlace;

	private bool isRival;

	private bool isDominated;

	private FirstPlaceCrownVfx firstPlaceVfx;

	private RivalryVfx rivalVfx;

	private DominatedVfx dominatingVfx;

	private AirhornPlayerTriggeredVfx airhornReactionVfx;

	private PoolableParticleSystem electromagnetShieldVfx;

	private readonly HashSet<TextPopupUi> textPopups = new HashSet<TextPopupUi>();

	private readonly Queue<(string, bool)> textPopupQueue = new Queue<(string, bool)>();

	private double lastPopupTimestamp = double.MinValue;

	private bool isExecutingQueuedPopups;

	private Coroutine executeQueuedPopupsRoutine;

	private Emote emoteToPlay;

	private Coroutine emoteRoutine;

	private AntiCheatRateChecker serverRestartPlayerCommandRateLimiter;

	private AntiCheatPerPlayerRateChecker serverInformPlayerOfBlownAirhornCommandRateLimiter;

	private AntiCheatRateChecker serverAirhornReactionVfxCommandRateLimiter;

	private AntiCheatPerPlayerRateChecker serverElectromagnetShieldHitCommandRateLimiter;

	private AntiCheatRateChecker serverCancelEmoteCommandRateLimiter;

	public static readonly Dictionary<ulong, PlayerInfo> playerInfoPerPlayerGuid;

	private ButtonPrompt exitButtonPrompt;

	public Action<bool, bool> _Mirror_SyncVarHookDelegate_shouldPlayOverchargedVfx;

	public Action<int, int> _Mirror_SyncVarHookDelegate_networkedEquippedItemIndex;

	public Action<ItemType, ItemType> _Mirror_SyncVarHookDelegate_networkedEquippedItem;

	public Action<float, float> _Mirror_SyncVarHookDelegate_networkedRocketDriverNormalizedCharge;

	public Action<bool, bool> _Mirror_SyncVarHookDelegate_isElectromagnetShieldActive;

	public Action<GolfCartSeat, GolfCartSeat> _Mirror_SyncVarHookDelegate_activeGolfCartSeat;

	[field: SerializeField]
	public Transform HipBone { get; private set; }

	[field: SerializeField]
	public Transform Spine1Bone { get; private set; }

	[field: SerializeField]
	public Transform ChestBone { get; private set; }

	[field: SerializeField]
	public Transform NeckBone { get; private set; }

	[field: SerializeField]
	public Transform HeadBone { get; private set; }

	[field: SerializeField]
	public Transform LeftFootBone { get; private set; }

	[field: SerializeField]
	public Transform RightFootBone { get; private set; }

	[field: SerializeField]
	public Transform LeftToeBone { get; private set; }

	[field: SerializeField]
	public Transform RightToeBone { get; private set; }

	[field: SerializeField]
	public Transform LeftFootCenter { get; private set; }

	[field: SerializeField]
	public Transform RightFootCenter { get; private set; }

	[field: SerializeField]
	public EquipmentSwitcher RightHandEquipmentSwitcher { get; private set; }

	[field: SerializeField]
	public EquipmentSwitcher LeftHandEquipmentSwitcher { get; private set; }

	[field: SerializeField]
	public GameObject Shoes { get; private set; }

	[field: SerializeField]
	public GameObject SpringBoots { get; private set; }

	[field: SerializeField]
	public SphereCollider ElectromagnetShieldCollider { get; private set; }

	public PlayerId PlayerId { get; private set; }

	public PlayerMovement Movement { get; private set; }

	public PlayerGolfer AsGolfer { get; private set; }

	public PlayerInteractableTargeter AsTargeter { get; private set; }

	public PlayerInteractor AsInteractor { get; private set; }

	public PlayerInventory Inventory { get; private set; }

	public PlayerSpectator AsSpectator { get; private set; }

	public PlayerInput Input { get; private set; }

	public PlayerAnimatorIo AnimatorIo { get; private set; }

	public PlayerVoiceChat VoiceChat { get; private set; }

	public PlayerAudio PlayerAudio { get; private set; }

	public PlayerCosmetics Cosmetics { get; private set; }

	public PlayerVfx Vfx { get; private set; }

	public PlayerOcclusion Occlusion { get; private set; }

	public Entity AsEntity { get; private set; }

	public Hittable AsHittable { get; private set; }

	public LevelBoundsTracker LevelBoundsTracker { get; private set; }

	public Rigidbody Rigidbody { get; private set; }

	public NetworkRigidbodyUnreliable NetworkRigidbody { get; private set; }

	public bool NetworkedIsAimingSwing => networkedIsAimingSwing;

	public bool NetworkedIsAimingItem => networkedIsAimingItem;

	public int NetworkedEquippedItemIndex => networkedEquippedItemIndex;

	public ItemType NetworkedEquippedItem => networkedEquippedItem;

	public bool IsElectromagnetShieldActive => isElectromagnetShieldActive;

	public ItemUseId ElectromagnetShieldItemUseId => electromagnetShieldItemUseId;

	public GolfCartSeat ActiveGolfCartSeat => activeGolfCartSeat;

	public Emote EmoteBeingPlayed { get; private set; }

	public bool WantsToPlayEmote => emoteToPlay != Emote.None;

	public bool IsPlayingEmote => EmoteBeingPlayed != Emote.None;

	public bool NetworknetworkedIsAimingSwing
	{
		get
		{
			return networkedIsAimingSwing;
		}
		[param: In]
		set
		{
			GeneratedSyncVarSetter(value, ref networkedIsAimingSwing, 1uL, null);
		}
	}

	public bool NetworkshouldPlayOverchargedVfx
	{
		get
		{
			return shouldPlayOverchargedVfx;
		}
		[param: In]
		set
		{
			GeneratedSyncVarSetter(value, ref shouldPlayOverchargedVfx, 2uL, _Mirror_SyncVarHookDelegate_shouldPlayOverchargedVfx);
		}
	}

	public bool NetworknetworkedIsAimingItem
	{
		get
		{
			return networkedIsAimingItem;
		}
		[param: In]
		set
		{
			GeneratedSyncVarSetter(value, ref networkedIsAimingItem, 4uL, null);
		}
	}

	public int NetworknetworkedEquippedItemIndex
	{
		get
		{
			return networkedEquippedItemIndex;
		}
		[param: In]
		set
		{
			GeneratedSyncVarSetter(value, ref networkedEquippedItemIndex, 8uL, _Mirror_SyncVarHookDelegate_networkedEquippedItemIndex);
		}
	}

	public ItemType NetworknetworkedEquippedItem
	{
		get
		{
			return networkedEquippedItem;
		}
		[param: In]
		set
		{
			GeneratedSyncVarSetter(value, ref networkedEquippedItem, 16uL, _Mirror_SyncVarHookDelegate_networkedEquippedItem);
		}
	}

	public float NetworknetworkedRocketDriverNormalizedCharge
	{
		get
		{
			return networkedRocketDriverNormalizedCharge;
		}
		[param: In]
		set
		{
			GeneratedSyncVarSetter(value, ref networkedRocketDriverNormalizedCharge, 32uL, _Mirror_SyncVarHookDelegate_networkedRocketDriverNormalizedCharge);
		}
	}

	public bool NetworkisElectromagnetShieldActive
	{
		get
		{
			return isElectromagnetShieldActive;
		}
		[param: In]
		set
		{
			GeneratedSyncVarSetter(value, ref isElectromagnetShieldActive, 64uL, _Mirror_SyncVarHookDelegate_isElectromagnetShieldActive);
		}
	}

	public ItemUseId NetworkelectromagnetShieldItemUseId
	{
		get
		{
			return electromagnetShieldItemUseId;
		}
		[param: In]
		set
		{
			GeneratedSyncVarSetter(value, ref electromagnetShieldItemUseId, 128uL, null);
		}
	}

	public GolfCartSeat NetworkactiveGolfCartSeat
	{
		get
		{
			return activeGolfCartSeat;
		}
		[param: In]
		set
		{
			GeneratedSyncVarSetter(value, ref activeGolfCartSeat, 256uL, _Mirror_SyncVarHookDelegate_activeGolfCartSeat);
		}
	}

	public event Action Destroyed;

	public event Action NetworkedEquippedItemIndexChanged;

	public event Action IsInGolfCartChanged;

	public event Action EmoteBeingPlayedChanged;

	public static event Action LocalPlayerIsElectromagnetShieldActiveChanged;

	public static event Action<bool> LocalPlayerEnteredGolfCart;

	public static event Action LocalPlayerExitedGolfCart;

	public static event Action LocalPlayerIsInGolfCartChanged;

	private void ReturnButtonPrompts()
	{
		if (exitButtonPrompt != null)
		{
			ButtonPromptManager.ReturnButtonPrompt(exitButtonPrompt);
		}
		exitButtonPrompt = null;
	}

	[CCommand("popupRegular", "", false, false)]
	public static void PopupRegular(string text)
	{
		if (!(GameManager.LocalPlayerInfo == null))
		{
			GameManager.LocalPlayerInfo.PopUpText(text, isPenalty: false);
		}
	}

	[CCommand("popupPenalty", "", false, false)]
	public static void PopupPenalty(string text)
	{
		if (!(GameManager.LocalPlayerInfo == null))
		{
			GameManager.LocalPlayerInfo.PopUpText(text, isPenalty: true);
		}
	}

	private void Awake()
	{
		PlayerId = GetComponent<PlayerId>();
		Movement = GetComponent<PlayerMovement>();
		AsGolfer = GetComponent<PlayerGolfer>();
		AsTargeter = GetComponent<PlayerInteractableTargeter>();
		AsInteractor = GetComponent<PlayerInteractor>();
		Inventory = GetComponent<PlayerInventory>();
		AsSpectator = GetComponent<PlayerSpectator>();
		Input = GetComponent<PlayerInput>();
		AnimatorIo = GetComponent<PlayerAnimatorIo>();
		VoiceChat = GetComponent<PlayerVoiceChat>();
		PlayerAudio = GetComponent<PlayerAudio>();
		Cosmetics = GetComponent<PlayerCosmetics>();
		Vfx = GetComponent<PlayerVfx>();
		Occlusion = GetComponent<PlayerOcclusion>();
		AsEntity = GetComponent<Entity>();
		AsHittable = GetComponent<Hittable>();
		LevelBoundsTracker = GetComponent<LevelBoundsTracker>();
		Rigidbody = GetComponent<Rigidbody>();
		NetworkRigidbody = GetComponent<NetworkRigidbodyUnreliable>();
		syncDirection = SyncDirection.ClientToServer;
		OnIsElectromagnetShieldActiveChanged(wasActive: false, isElectromagnetShieldActive);
		LoadingScreen.Hide();
		Movement.IsVisibleChanged += OnIsVisibleChanged;
		AsHittable.IsFrozenChanged += OnIsFrozenChanged;
		PlayerId.AnyPlayerGuidChanged += OnAnyPlayerGuidChanged;
		CourseManager.PlayerDominationsChanged += OnPlayerDominationsChanged;
	}

	private void Start()
	{
		UpdateDominationState();
	}

	public void OnWillBeDestroyed()
	{
		playerInfoPerPlayerGuid.Remove(PlayerId.Guid);
		ClearTextPopups();
		UpdateOverheadMarkVfx();
		if (airhornReactionVfx != null)
		{
			airhornReactionVfx.AsPoolableParticleSystem.Stop(ParticleSystemStopBehavior.StopEmittingAndClear);
		}
		if (electromagnetShieldVfx != null)
		{
			electromagnetShieldVfx.Stop(ParticleSystemStopBehavior.StopEmittingAndClear);
		}
		PlayerId.OnWillBeDestroyed();
		Movement.OnWillBeDestroyed();
		AsGolfer.OnWillBeDestroyed();
		Inventory.OnWillBeDestroyed();
		RightHandEquipmentSwitcher.OnWillBeDestroyed();
		LeftHandEquipmentSwitcher.OnWillBeDestroyed();
		Vfx.OnWillBeDestroyed();
		ReturnButtonPrompts();
		if (BNetworkManager.IsChangingSceneOrShuttingDown)
		{
			return;
		}
		if (isElectromagnetShieldActive)
		{
			ElectromagnetShieldManager.DeregisterActiveShield(this);
		}
		Movement.IsVisibleChanged -= OnIsVisibleChanged;
		AsHittable.IsFrozenChanged -= OnIsFrozenChanged;
		PlayerId.AnyPlayerGuidChanged -= OnAnyPlayerGuidChanged;
		CourseManager.PlayerDominationsChanged -= OnPlayerDominationsChanged;
		if (activeGolfCartSeat.IsValid())
		{
			activeGolfCartSeat.golfCart.AsEntity.AsHittable.IsFrozenChanged -= OnCurrentGolfCartIsFrozenChanged;
			if (base.isLocalPlayer)
			{
				activeGolfCartSeat.golfCart.DriverSeatReserverChanged -= OnLocalPlayerGolfCartDriverSeatReserverChanged;
				activeGolfCartSeat.golfCart.PassengersChanged -= OnLocalPlayerGolfCartPassengersChanged;
			}
		}
	}

	public override void OnStartServer()
	{
		serverRestartPlayerCommandRateLimiter = new AntiCheatRateChecker("Restart player", base.connectionToClient.connectionId, 1f, 3, 6, 4f);
		serverInformPlayerOfBlownAirhornCommandRateLimiter = new AntiCheatPerPlayerRateChecker("Inform of blown airhorn", 0.5f, 2, 10, 2f);
		serverAirhornReactionVfxCommandRateLimiter = new AntiCheatRateChecker("Airhorn reaction VFX", base.connectionToClient.connectionId, 0.1f, 10, 20, 1f, 5);
		serverElectromagnetShieldHitCommandRateLimiter = new AntiCheatPerPlayerRateChecker("Electromagnet shield hit", 0.1f, 10, 20, 1f, 5);
		serverCancelEmoteCommandRateLimiter = new AntiCheatRateChecker("Cancel emote", base.connectionToClient.connectionId, 0.1f, 5, 20, 2f);
	}

	public override void OnStartClient()
	{
		GameManager.RegisterPlayer(this);
		Shoes.SetPlayerShaderIndexOnRenderers(this);
		SpringBoots.SetPlayerShaderIndexOnRenderers(this);
	}

	public override void OnStopClient()
	{
		GameManager.DeregisterPlayer(this);
	}

	public bool IsBlockedOnSteam()
	{
		if (!SteamEnabler.IsSteamEnabled || base.isLocalPlayer || !BNetworkManager.TryGetPlayerRelationship(PlayerId.Guid, out var relationship))
		{
			return false;
		}
		return IsBlockedOnSteam(relationship);
	}

	public static bool IsBlockedOnSteam(Relationship relationship)
	{
		if (relationship != Relationship.Blocked && relationship != Relationship.Ignored)
		{
			return relationship == Relationship.IgnoredFriend;
		}
		return true;
	}

	public void OnGuidChanged(ulong previousGuid, ulong currentGuid)
	{
		playerInfoPerPlayerGuid.Remove(previousGuid);
		playerInfoPerPlayerGuid.Add(currentGuid, this);
	}

	[TargetRpc]
	public void RpcAwaitSpawning()
	{
		NetworkWriterPooled writer = NetworkWriterPool.Get();
		SendTargetRPCInternal(null, "System.Void PlayerInfo::RpcAwaitSpawning()", -920497192, writer, 0);
		NetworkWriterPool.Return(writer);
	}

	[Server]
	public void ServerInitializeAsParticipant(Vector3 position, Quaternion rotation, TeeingSpot teeingSpot)
	{
		if (!NetworkServer.active)
		{
			Debug.LogWarning("[Server] function 'System.Void PlayerInfo::ServerInitializeAsParticipant(UnityEngine.Vector3,UnityEngine.Quaternion,TeeingSpot)' called when server was not active");
			return;
		}
		AsGolfer.ServerInitializeAsParticipant(teeingSpot);
		Movement.RpcInformSpawned(position, rotation);
	}

	[Server]
	public void ServerInitializeAsSpectator(bool fromHoleStart)
	{
		if (!NetworkServer.active)
		{
			Debug.LogWarning("[Server] function 'System.Void PlayerInfo::ServerInitializeAsSpectator(System.Boolean)' called when server was not active");
		}
		else
		{
			AsGolfer.ServerInitializeAsSpectator(fromHoleStart);
		}
	}

	public void SetIsAimingSwing(bool isAiming)
	{
		NetworknetworkedIsAimingSwing = isAiming;
	}

	public void SetShouldPlayOverchargedVfx(bool isPlaying)
	{
		NetworkshouldPlayOverchargedVfx = isPlaying;
	}

	public void SetIsAimingItem(bool isAiming)
	{
		NetworknetworkedIsAimingItem = isAiming;
	}

	public void SetEquippedItemIndex(int index)
	{
		NetworknetworkedEquippedItemIndex = index;
	}

	public void SetEquippedItem(ItemType item)
	{
		NetworknetworkedEquippedItem = item;
	}

	public void SetRocketDriverThrustPower(float power)
	{
		NetworknetworkedRocketDriverNormalizedCharge = power;
	}

	public void SetEmoteToPlay(Emote emote)
	{
		if (emote != emoteToPlay)
		{
			CancelEmote(canHideEmoteMenu: false);
			emoteRoutine = StartCoroutine(EmoteRoutine());
		}
		bool CanEmote(out bool shouldCancel)
		{
			shouldCancel = true;
			if (activeGolfCartSeat.IsValid())
			{
				return false;
			}
			if (AsSpectator.IsSpectating)
			{
				return false;
			}
			if (AsGolfer.IsMatchResolved)
			{
				return false;
			}
			if (AsGolfer.IsAimingSwing)
			{
				return false;
			}
			if (AsGolfer.IsSwinging && !AsGolfer.CanInterruptSwing())
			{
				return false;
			}
			if (AsGolfer.IsChargingSwing)
			{
				return false;
			}
			if (Inventory.IsAimingItem)
			{
				return false;
			}
			if (Movement.IsRespawning)
			{
				return false;
			}
			if (Movement.IsKnockedOutOrRecovering)
			{
				return false;
			}
			shouldCancel = false;
			if (!Movement.IsGrounded)
			{
				return false;
			}
			if (Movement.DivingState != DivingState.None)
			{
				return false;
			}
			if (Inventory.IsUsingItemAtAll)
			{
				return false;
			}
			return true;
		}
		bool DoesEmoteQualifyforSweetMovesAchievement()
		{
			if (SingletonBehaviour<DrivingRangeManager>.HasInstance)
			{
				return false;
			}
			if (CourseManager.CountActivePlayers() <= 1)
			{
				return false;
			}
			if (EmoteBeingPlayed != Emote.VictoryDance)
			{
				return false;
			}
			if (!LevelBoundsTracker.AuthoritativeIsOnGreen)
			{
				return false;
			}
			if (CourseManager.DidAnyPlayerScore)
			{
				return false;
			}
			return true;
		}
		IEnumerator EmoteRoutine()
		{
			if (!GameManager.EmoteSettings.emotesByType.TryGetValue(emote, out var emoteSettings))
			{
				Debug.LogError($"Could not find settings for emote of type {emote}", base.gameObject);
			}
			else
			{
				emoteToPlay = emote;
				bool shouldCancel;
				while (!CanEmote(out shouldCancel))
				{
					if (shouldCancel)
					{
						CancelEmote(canHideEmoteMenu: false);
						yield break;
					}
					yield return null;
				}
				SetEmoteBeingPlayed(emoteToPlay);
				if (Movement.DivingState == DivingState.None)
				{
					Movement.SuppressMovementUntilInputReleased();
				}
				if (AsGolfer.IsSwinging)
				{
					AsGolfer.CancelSwing();
				}
				Inventory.CancelItemFlourish();
				bool canQualifyForSweetMovesAchievement = DoesEmoteQualifyforSweetMovesAchievement();
				bool unlockedSweetMovesAchievement = false;
				float time = 0f;
				bool shouldCancel2;
				while (CanEmote(out shouldCancel2))
				{
					if (!emoteSettings.loops && time >= emoteSettings.duration)
					{
						CancelEmote(canHideEmoteMenu: false);
						yield break;
					}
					yield return null;
					time += Time.deltaTime;
					if (!unlockedSweetMovesAchievement)
					{
						if (canQualifyForSweetMovesAchievement)
						{
							canQualifyForSweetMovesAchievement = DoesEmoteQualifyforSweetMovesAchievement();
						}
						if (canQualifyForSweetMovesAchievement && time >= GameManager.Achievements.SweetMovesMinTime)
						{
							GameManager.AchievementsManager.Unlock(AchievementId.SweetMoves);
							unlockedSweetMovesAchievement = true;
						}
					}
				}
				CancelEmote(canHideEmoteMenu: false);
			}
		}
	}

	public void CancelEmote(bool canHideEmoteMenu)
	{
		if (canHideEmoteMenu && (RadialMenu.CurrentMode == RadialMenuMode.Emote || RadialMenu.CurrentMode == RadialMenuMode.SpectatorEmote))
		{
			RadialMenu.Hide();
		}
		if (emoteToPlay != Emote.None)
		{
			if (emoteRoutine != null)
			{
				StopCoroutine(emoteRoutine);
			}
			if (EmoteBeingPlayed == Emote.VictoryDance)
			{
				AnimatorIo.SetVictoryDance(VictoryDance.None);
			}
			emoteToPlay = Emote.None;
			SetEmoteBeingPlayed(Emote.None);
			AnimatorIo.SetEmote(Emote.None);
			CmdInformCancelledEmote();
		}
	}

	[Command]
	private void CmdInformCancelledEmote()
	{
		if (base.isServer && base.isClient)
		{
			UserCode_CmdInformCancelledEmote();
			return;
		}
		NetworkWriterPooled writer = NetworkWriterPool.Get();
		SendCommandInternal("System.Void PlayerInfo::CmdInformCancelledEmote()", 657875652, writer, 0);
		NetworkWriterPool.Return(writer);
	}

	[ClientRpc]
	private void RpcInformCancelledEmote()
	{
		NetworkWriterPooled writer = NetworkWriterPool.Get();
		SendRPCInternal("System.Void PlayerInfo::RpcInformCancelledEmote()", -1888783173, writer, 0, includeOwner: true);
		NetworkWriterPool.Return(writer);
	}

	private void SetEmoteBeingPlayed(Emote emote)
	{
		Emote emoteBeingPlayed = EmoteBeingPlayed;
		EmoteBeingPlayed = emote;
		if (EmoteBeingPlayed != emoteBeingPlayed)
		{
			bool num = emoteBeingPlayed != Emote.None;
			if (emoteBeingPlayed == Emote.VictoryDance && AsGolfer.MatchResolution != PlayerMatchResolution.Scored)
			{
				AnimatorIo.SetVictoryDance(VictoryDance.None);
			}
			if (EmoteBeingPlayed == Emote.VictoryDance)
			{
				AnimatorIo.SetVictoryDance(Cosmetics.victoryDance);
			}
			else
			{
				AnimatorIo.SetEmote(emote);
			}
			if (num != IsPlayingEmote)
			{
				Inventory.InformIsPlayingEmoteChanged();
			}
			this.EmoteBeingPlayedChanged?.Invoke();
		}
	}

	public void LocalPlayerActivateElectromagnetShield(ItemUseId itemUseId)
	{
		localPlayerElectromagnetShieldActivationTimestamp = Time.timeAsDouble;
		LocalPlayerUpdateIsElectromagnetShieldActive(itemUseId);
	}

	public void InformAttemptingToEnterGolfCart(GolfCartInfo golfCart)
	{
		Movement.InformEnteringGolfCart(golfCart);
	}

	[TargetRpc]
	public void RpcInformOfGolfCartEnterAttemptResult(GolfCartInfo golfCart, bool succeeded)
	{
		NetworkWriterPooled writer = NetworkWriterPool.Get();
		writer.WriteNetworkBehaviour(golfCart);
		writer.WriteBool(succeeded);
		SendTargetRPCInternal(null, "System.Void PlayerInfo::RpcInformOfGolfCartEnterAttemptResult(GolfCartInfo,System.Boolean)", 466754772, writer, 0);
		NetworkWriterPool.Return(writer);
	}

	public bool TryInformEnteredGolfCartSeat(GolfCartInfo golfCart, int seat, bool fromReservation)
	{
		if (!base.isLocalPlayer)
		{
			Debug.LogError("Only the local player is allowed to enter a golf cart", base.gameObject);
			return false;
		}
		bool wasInGolfCart = activeGolfCartSeat.IsValid();
		if (!CanEnter())
		{
			return false;
		}
		bool flag = seat == 0;
		NetworkactiveGolfCartSeat = new GolfCartSeat(golfCart, seat);
		if (flag)
		{
			Input.EnablePlayerInputMode(InputMode.GolfCartDriver);
			Input.DisablePlayerInputMode(InputMode.GolfCartPassenger);
		}
		else
		{
			Input.DisablePlayerInputMode(InputMode.GolfCartDriver);
			Input.EnablePlayerInputMode(InputMode.GolfCartPassenger);
		}
		if (!wasInGolfCart)
		{
			CancelEmote(canHideEmoteMenu: true);
			Inventory.CancelItemFlourish();
			PlayerInfo.LocalPlayerEnteredGolfCart?.Invoke(flag && fromReservation);
		}
		return true;
		bool CanEnter()
		{
			if (wasInGolfCart && activeGolfCartSeat.golfCart != golfCart)
			{
				return false;
			}
			return CanEnterGolfCart();
		}
	}

	public bool CanEnterGolfCart()
	{
		if (AsGolfer.IsMatchResolved)
		{
			return false;
		}
		if (Movement.IsKnockedOutOrRecovering)
		{
			return false;
		}
		if (Movement.IsRespawning)
		{
			return false;
		}
		if (AsHittable.IsFrozen)
		{
			return false;
		}
		if (Inventory.IsUsingItemAtAll && Inventory.GetEffectivelyEquippedItem() != ItemType.GolfCart)
		{
			return false;
		}
		return true;
	}

	public bool TrySwitchGolfCartSeatTo(int seat)
	{
		if (!activeGolfCartSeat.IsValid())
		{
			return false;
		}
		if (!activeGolfCartSeat.golfCart.IsSeatFreeFor(this, seat))
		{
			return false;
		}
		activeGolfCartSeat.golfCart.CmdTryChangeLocalPlayerSeat(seat);
		return true;
	}

	public void InformGolfCartSeatChangeFailed()
	{
		if (activeGolfCartSeat.IsValid())
		{
			Hotkeys.Select(activeGolfCartSeat.seat, uiOnly: true);
		}
	}

	public void ExitGolfCart(GolfCartExitType exitType)
	{
		if (!base.isLocalPlayer)
		{
			Debug.LogError("Only the local player is allowed to exit a golf cart", base.gameObject);
		}
		else if (activeGolfCartSeat.IsValid())
		{
			GolfCartSeat previousSeat = activeGolfCartSeat;
			NetworkactiveGolfCartSeat = GolfCartSeat.Invalid;
			previousSeat.golfCart.CmdExit();
			Input.DisablePlayerInputMode(InputMode.GolfCartDriver);
			Input.DisablePlayerInputMode(InputMode.GolfCartPassenger);
			Movement.InformLocalPlayerExitedGolfCart(previousSeat, exitType);
			PlayerInfo.LocalPlayerExitedGolfCart?.Invoke();
		}
	}

	public void RestartPlayer()
	{
		if (!base.isLocalPlayer)
		{
			Debug.LogError("This can only be called from local player!");
		}
		else if (RestartPrompt.CanBePressed(this))
		{
			CmdRestartPlayer();
		}
	}

	[Command]
	private void CmdRestartPlayer()
	{
		if (base.isServer && base.isClient)
		{
			UserCode_CmdRestartPlayer();
			return;
		}
		NetworkWriterPooled writer = NetworkWriterPool.Get();
		SendCommandInternal("System.Void PlayerInfo::CmdRestartPlayer()", 900255672, writer, 0);
		NetworkWriterPool.Return(writer);
	}

	public void HonkGolfCart()
	{
		if (!base.isLocalPlayer)
		{
			Debug.LogError("Only the local player is allowed to honk a golf cart", base.gameObject);
		}
		else if (activeGolfCartSeat.IsValid() && activeGolfCartSeat.seat == 0)
		{
			activeGolfCartSeat.golfCart.PlayHonkForAllClients();
		}
	}

	public void StartSpecialGolfCartHonk()
	{
		if (!base.isLocalPlayer)
		{
			Debug.LogError("Only the local player is allowed to honk a golf cart", base.gameObject);
		}
	}

	public void EndSpecialGolfCartHonk()
	{
		if (!base.isLocalPlayer)
		{
			Debug.LogError("Only the local player is allowed to end a golf cart honk", base.gameObject);
		}
		else if (activeGolfCartSeat.IsValid() && activeGolfCartSeat.seat == 0)
		{
			activeGolfCartSeat.golfCart.EndSpecialHonkForAllClients();
		}
	}

	public void MarkFirstPlace()
	{
		isFirstPlace = true;
		UpdateOverheadMarkVfx();
	}

	public void UnmarkFirstPlace()
	{
		isFirstPlace = false;
		UpdateOverheadMarkVfx();
	}

	public void InformIsMovingInFoliage()
	{
		if (!base.isLocalPlayer)
		{
			Debug.LogError("Only the local player should be informed of moving in foliage", base.gameObject);
		}
		else
		{
			PlayerAudio.PlayOrUpdateMovingInFoliageForAllClients(Rigidbody.linearVelocity.magnitude);
		}
	}

	public void InformNoLongerMovingInFoliage()
	{
		if (!base.isLocalPlayer)
		{
			Debug.LogError("Only the local player should be informed of no longer moving in foliage", base.gameObject);
		}
		else
		{
			PlayerAudio.StopMovingInFoliageForAllClients();
		}
	}

	public void InformCourseStateChanged(CourseManager.PlayerState previousState, CourseManager.PlayerState currentState)
	{
		if (this == GameManager.GetViewedOrLocalPlayer() && currentState.matchStrokes != previousState.matchStrokes)
		{
			HoleProgressBarUi.UpdateStrokes();
		}
		AsGolfer.InformCourseStateChanged(previousState, currentState);
	}

	[TargetRpc]
	public void RpcInformFrozeGolfCart(int otherPassengerCount)
	{
		NetworkWriterPooled writer = NetworkWriterPool.Get();
		writer.WriteVarInt(otherPassengerCount);
		SendTargetRPCInternal(null, "System.Void PlayerInfo::RpcInformFrozeGolfCart(System.Int32)", 1644328919, writer, 0);
		NetworkWriterPool.Return(writer);
	}

	[TargetRpc]
	public void RpcInformPlayerFrozen(bool isFrozen)
	{
		NetworkWriterPooled writer = NetworkWriterPool.Get();
		writer.WriteBool(isFrozen);
		SendTargetRPCInternal(null, "System.Void PlayerInfo::RpcInformPlayerFrozen(System.Boolean)", 1461172404, writer, 0);
		NetworkWriterPool.Return(writer);
	}

	[ClientRpc]
	public void RpcInformAllClientsPlayerFrozen(bool isFrozen)
	{
		NetworkWriterPooled writer = NetworkWriterPool.Get();
		writer.WriteBool(isFrozen);
		SendRPCInternal("System.Void PlayerInfo::RpcInformAllClientsPlayerFrozen(System.Boolean)", -1985668283, writer, 0, includeOwner: true);
		NetworkWriterPool.Return(writer);
	}

	[ClientRpc]
	public void RpcPopUpPlacementScore(int placement, int score)
	{
		NetworkWriterPooled writer = NetworkWriterPool.Get();
		writer.WriteVarInt(placement);
		writer.WriteVarInt(score);
		SendRPCInternal("System.Void PlayerInfo::RpcPopUpPlacementScore(System.Int32,System.Int32)", 695638470, writer, 0, includeOwner: true);
		NetworkWriterPool.Return(writer);
	}

	[ClientRpc]
	public void RpcPopUpDrivingRangeScore(int score)
	{
		NetworkWriterPooled writer = NetworkWriterPool.Get();
		writer.WriteVarInt(score);
		SendRPCInternal("System.Void PlayerInfo::RpcPopUpDrivingRangeScore(System.Int32)", 978199250, writer, 0, includeOwner: true);
		NetworkWriterPool.Return(writer);
	}

	[ClientRpc]
	public void RpcPopUp(PlayerTextPopupType popupType, int number)
	{
		NetworkWriterPooled writer = NetworkWriterPool.Get();
		GeneratedNetworkCode._Write_PlayerTextPopupType(writer, popupType);
		writer.WriteVarInt(number);
		SendRPCInternal("System.Void PlayerInfo::RpcPopUp(PlayerTextPopupType,System.Int32)", -837608140, writer, 0, includeOwner: true);
		NetworkWriterPool.Return(writer);
	}

	private void PopUpInternal(string text, bool isPenalty)
	{
		if (CanPopUp())
		{
			PopUpText(text, isPenalty);
			return;
		}
		textPopupQueue.Enqueue((text, isPenalty));
		if (!isExecutingQueuedPopups)
		{
			executeQueuedPopupsRoutine = StartCoroutine(ExecuteQueuedPopupsRoutine());
		}
		bool CanPopUp()
		{
			return BMath.GetTimeSince(lastPopupTimestamp) > GameManager.UiSettings.TimeBetweenPlayerTextPopups;
		}
		IEnumerator ExecuteQueuedPopupsRoutine()
		{
			isExecutingQueuedPopups = true;
			while (textPopupQueue.Count > 0)
			{
				if (CanPopUp())
				{
					var (text2, isPenalty2) = textPopupQueue.Dequeue();
					PopUpText(text2, isPenalty2);
				}
				yield return null;
			}
			isExecutingQueuedPopups = false;
		}
	}

	private string GetPopupLocalizedString(PlayerTextPopupType popupType)
	{
		return LocalizationManager.GetString(StringTable.UI, $"POPUP_{popupType}");
	}

	private string AddScoreToPopupText(string text, int score)
	{
		return $"{text} +{score}";
	}

	private string AddPenaltyToPopupText(string text, int penalty)
	{
		return $"+{penalty} {text}";
	}

	public void UpdateDominationState()
	{
		bool flag = isRival;
		isRival = IsRival();
		bool flag2 = isDominated;
		isDominated = IsDominated();
		if (isRival != flag || isDominated != flag2)
		{
			UpdateOverheadMarkVfx();
		}
		bool IsDominated()
		{
			if (!LocalPlayerValid())
			{
				return false;
			}
			return CourseManager.PlayerDominations.Contains(new CourseManager.PlayerPair(GameManager.LocalPlayerId.Guid, PlayerId.Guid));
		}
		bool IsRival()
		{
			if (!LocalPlayerValid())
			{
				return false;
			}
			return CourseManager.PlayerDominations.Contains(new CourseManager.PlayerPair(PlayerId.Guid, GameManager.LocalPlayerId.Guid));
		}
		bool LocalPlayerValid()
		{
			if (base.isLocalPlayer)
			{
				return false;
			}
			if (GameManager.LocalPlayerInfo == null)
			{
				return false;
			}
			if (CourseManager.PlayerDominations == null)
			{
				return false;
			}
			return true;
		}
	}

	private void UpdateOverheadMarkVfx()
	{
		if (!CanDisplayAnyMark())
		{
			ClearMark();
		}
		else if (isDominated)
		{
			ApplyDominatingMark();
		}
		else if (isRival)
		{
			ApplyRivalMark();
		}
		else if (isFirstPlace)
		{
			ApplyFirstPlaceMark();
		}
		else
		{
			ClearMark();
		}
		void ApplyDominatingMark()
		{
			RemoveFirstMark();
			RemoveRivalMark();
			if (!(dominatingVfx != null) && VfxPersistentData.TryGetPooledVfx(VfxType.Dominated, out var particleSystem))
			{
				dominatingVfx = particleSystem.GetComponent<DominatedVfx>();
				if (!(dominatingVfx == null))
				{
					dominatingVfx.Spawn(HeadBone);
				}
			}
		}
		void ApplyFirstPlaceMark()
		{
			RemoveRivalMark();
			RemoveDominatingMark();
			if (!(firstPlaceVfx != null) && VfxPersistentData.TryGetPooledVfx(VfxType.FirstPlaceCrown, out var particleSystem))
			{
				firstPlaceVfx = particleSystem.GetComponent<FirstPlaceCrownVfx>();
				if (!(firstPlaceVfx == null))
				{
					firstPlaceVfx.Spawn(HeadBone);
				}
			}
		}
		void ApplyRivalMark()
		{
			RemoveFirstMark();
			RemoveDominatingMark();
			if (!(rivalVfx != null) && VfxPersistentData.TryGetPooledVfx(VfxType.Rivalry, out var particleSystem))
			{
				rivalVfx = particleSystem.GetComponent<RivalryVfx>();
				if (!(rivalVfx == null))
				{
					rivalVfx.Spawn(HeadBone);
				}
			}
		}
		bool CanDisplayAnyMark()
		{
			if (!Movement.IsVisible)
			{
				return false;
			}
			if (AsEntity.IsDestroyed)
			{
				return false;
			}
			return true;
		}
		void ClearMark()
		{
			RemoveRivalMark();
			RemoveFirstMark();
			RemoveDominatingMark();
		}
		void RemoveDominatingMark()
		{
			if (!(dominatingVfx == null))
			{
				dominatingVfx.Despawn();
				dominatingVfx = null;
			}
		}
		void RemoveFirstMark()
		{
			if (!(firstPlaceVfx == null))
			{
				firstPlaceVfx.Despawn();
				firstPlaceVfx = null;
			}
		}
		void RemoveRivalMark()
		{
			if (!(rivalVfx == null))
			{
				rivalVfx.Despawn();
				rivalVfx = null;
			}
		}
	}

	[TargetRpc]
	public void RpcInformOfHoleFinishStrokesUnderPar(StrokesUnderParType strokesUnderParType)
	{
		NetworkWriterPooled writer = NetworkWriterPool.Get();
		GeneratedNetworkCode._Write_StrokesUnderParType(writer, strokesUnderParType);
		SendTargetRPCInternal(null, "System.Void PlayerInfo::RpcInformOfHoleFinishStrokesUnderPar(StrokesUnderParType)", 1600724246, writer, 0);
		NetworkWriterPool.Return(writer);
	}

	[TargetRpc]
	public void RpcInformKnockedOutOtherPlayer(KnockoutType knockoutType, Vector3 otherPlayerLocalOrigin, float distance, ItemType knockedOutPlayerHeldItem, bool onGreen, bool fromSpecialState)
	{
		NetworkWriterPooled writer = NetworkWriterPool.Get();
		GeneratedNetworkCode._Write_KnockoutType(writer, knockoutType);
		writer.WriteVector3(otherPlayerLocalOrigin);
		writer.WriteFloat(distance);
		GeneratedNetworkCode._Write_ItemType(writer, knockedOutPlayerHeldItem);
		writer.WriteBool(onGreen);
		writer.WriteBool(fromSpecialState);
		SendTargetRPCInternal(null, "System.Void PlayerInfo::RpcInformKnockedOutOtherPlayer(KnockoutType,UnityEngine.Vector3,System.Single,ItemType,System.Boolean,System.Boolean)", 1419052651, writer, 0);
		NetworkWriterPool.Return(writer);
	}

	[TargetRpc]
	public void RpcInformPickedUpItemFromItemSpawner()
	{
		NetworkWriterPooled writer = NetworkWriterPool.Get();
		SendTargetRPCInternal(null, "System.Void PlayerInfo::RpcInformPickedUpItemFromItemSpawner()", -1362785701, writer, 0);
		NetworkWriterPool.Return(writer);
	}

	[TargetRpc]
	public void RpcInformScoredWithNoMatchKnockouts()
	{
		NetworkWriterPooled writer = NetworkWriterPool.Get();
		SendTargetRPCInternal(null, "System.Void PlayerInfo::RpcInformScoredWithNoMatchKnockouts()", 710310337, writer, 0);
		NetworkWriterPool.Return(writer);
	}

	[TargetRpc]
	public void RpcInformScoredRevengeKnockout()
	{
		NetworkWriterPooled writer = NetworkWriterPool.Get();
		SendTargetRPCInternal(null, "System.Void PlayerInfo::RpcInformScoredRevengeKnockout()", -385445420, writer, 0);
		NetworkWriterPool.Return(writer);
	}

	public void InformDominationCountChanged(int dominationCount)
	{
		if (!SingletonBehaviour<DrivingRangeManager>.HasInstance && dominationCount >= GameManager.Achievements.BullyDominationRequirement)
		{
			GameManager.AchievementsManager.Unlock(AchievementId.Bully);
		}
	}

	[TargetRpc]
	public void RpcInformEvadedHomingProjectile()
	{
		NetworkWriterPooled writer = NetworkWriterPool.Get();
		SendTargetRPCInternal(null, "System.Void PlayerInfo::RpcInformEvadedHomingProjectile()", -1253734078, writer, 0);
		NetworkWriterPool.Return(writer);
	}

	[TargetRpc]
	public void RpcInformOfOrbitalLaserCloseCall()
	{
		NetworkWriterPooled writer = NetworkWriterPool.Get();
		SendTargetRPCInternal(null, "System.Void PlayerInfo::RpcInformOfOrbitalLaserCloseCall()", 834360863, writer, 0);
		NetworkWriterPool.Return(writer);
	}

	[TargetRpc]
	public void RpcInformBallEnteredHoleAfterTrajectoryDeflection()
	{
		NetworkWriterPooled writer = NetworkWriterPool.Get();
		SendTargetRPCInternal(null, "System.Void PlayerInfo::RpcInformBallEnteredHoleAfterTrajectoryDeflection()", -1486989652, writer, 0);
		NetworkWriterPool.Return(writer);
	}

	[TargetRpc]
	public void RpcInformBallEnteredHoleAfterTrajectoryDeflectionByTreeOrFoliage()
	{
		NetworkWriterPooled writer = NetworkWriterPool.Get();
		SendTargetRPCInternal(null, "System.Void PlayerInfo::RpcInformBallEnteredHoleAfterTrajectoryDeflectionByTreeOrFoliage()", 2102009009, writer, 0);
		NetworkWriterPool.Return(writer);
	}

	[TargetRpc]
	public void RpcInformSpectatedEntireHole()
	{
		NetworkWriterPooled writer = NetworkWriterPool.Get();
		SendTargetRPCInternal(null, "System.Void PlayerInfo::RpcInformSpectatedEntireHole()", -1132060838, writer, 0);
		NetworkWriterPool.Return(writer);
	}

	[TargetRpc]
	public void RpcInformDisarmedLandmine()
	{
		NetworkWriterPooled writer = NetworkWriterPool.Get();
		SendTargetRPCInternal(null, "System.Void PlayerInfo::RpcInformDisarmedLandmine()", 139188411, writer, 0);
		NetworkWriterPool.Return(writer);
	}

	[TargetRpc]
	public void RpcInformDroveGolfCartOutOfBoundsWithOtherPassengers(EliminationReason eliminationReason)
	{
		NetworkWriterPooled writer = NetworkWriterPool.Get();
		GeneratedNetworkCode._Write_EliminationReason(writer, eliminationReason);
		SendTargetRPCInternal(null, "System.Void PlayerInfo::RpcInformDroveGolfCartOutOfBoundsWithOtherPassengers(EliminationReason)", -270191228, writer, 0);
		NetworkWriterPool.Return(writer);
	}

	[TargetRpc]
	public void RpcInformEliminatedOtherPlayer(EliminationReason reason, EliminationReason immediateEliminationReason)
	{
		NetworkWriterPooled writer = NetworkWriterPool.Get();
		GeneratedNetworkCode._Write_EliminationReason(writer, reason);
		GeneratedNetworkCode._Write_EliminationReason(writer, immediateEliminationReason);
		SendTargetRPCInternal(null, "System.Void PlayerInfo::RpcInformEliminatedOtherPlayer(EliminationReason,EliminationReason)", 1963146611, writer, 0);
		NetworkWriterPool.Return(writer);
	}

	[TargetRpc]
	public void RpcInformOtherPlayerEliminatedWhileFrozenBySelf()
	{
		NetworkWriterPooled writer = NetworkWriterPool.Get();
		SendTargetRPCInternal(null, "System.Void PlayerInfo::RpcInformOtherPlayerEliminatedWhileFrozenBySelf()", 491020215, writer, 0);
		NetworkWriterPool.Return(writer);
	}

	[TargetRpc]
	public void InformKnockedOutSelfAndOtherPlayerWithExplosive()
	{
		NetworkWriterPooled writer = NetworkWriterPool.Get();
		SendTargetRPCInternal(null, "System.Void PlayerInfo::InformKnockedOutSelfAndOtherPlayerWithExplosive()", -8204037, writer, 0);
		NetworkWriterPool.Return(writer);
	}

	[TargetRpc]
	public void RpcInformQualifiedForThereCanBeOnlyOneAchievement()
	{
		NetworkWriterPooled writer = NetworkWriterPool.Get();
		SendTargetRPCInternal(null, "System.Void PlayerInfo::RpcInformQualifiedForThereCanBeOnlyOneAchievement()", -1435222921, writer, 0);
		NetworkWriterPool.Return(writer);
	}

	[TargetRpc]
	public void RpcInformQualifiedTargetRichEnvironmentAchievement()
	{
		NetworkWriterPooled writer = NetworkWriterPool.Get();
		SendTargetRPCInternal(null, "System.Void PlayerInfo::RpcInformQualifiedTargetRichEnvironmentAchievement()", -2117895927, writer, 0);
		NetworkWriterPool.Return(writer);
	}

	[TargetRpc]
	public void RpcInformQualifiedForOneTrueKingAchievement()
	{
		NetworkWriterPooled writer = NetworkWriterPool.Get();
		SendTargetRPCInternal(null, "System.Void PlayerInfo::RpcInformQualifiedForOneTrueKingAchievement()", -1005800463, writer, 0);
		NetworkWriterPool.Return(writer);
	}

	[TargetRpc]
	public void RpcInformOfCourseEndState(CourseManager.PlayerState localPlayerState)
	{
		NetworkWriterPooled writer = NetworkWriterPool.Get();
		GeneratedNetworkCode._Write_CourseManager_002FPlayerState(writer, localPlayerState);
		SendTargetRPCInternal(null, "System.Void PlayerInfo::RpcInformOfCourseEndState(CourseManager/PlayerState)", -612532271, writer, 0);
		NetworkWriterPool.Return(writer);
	}

	[Command(requiresAuthority = false)]
	public void CmdInformPlayerOfBlownAirhorn(NetworkConnectionToClient sender = null)
	{
		if (base.isServer && base.isClient)
		{
			UserCode_CmdInformPlayerOfBlownAirhorn__NetworkConnectionToClient(sender);
			return;
		}
		NetworkWriterPooled writer = NetworkWriterPool.Get();
		SendCommandInternal("System.Void PlayerInfo::CmdInformPlayerOfBlownAirhorn(Mirror.NetworkConnectionToClient)", 605177689, writer, 0, requiresAuthority: false);
		NetworkWriterPool.Return(writer);
	}

	[TargetRpc]
	public void RpcInformOfBlownAirhorn(PlayerInventory airhornBlower)
	{
		NetworkWriterPooled writer = NetworkWriterPool.Get();
		writer.WriteNetworkBehaviour(airhornBlower);
		SendTargetRPCInternal(null, "System.Void PlayerInfo::RpcInformOfBlownAirhorn(PlayerInventory)", -610301781, writer, 0);
		NetworkWriterPool.Return(writer);
	}

	[Command]
	private void CmdPlayAirhornReactionVfxForAllClients(bool reacted, NetworkConnectionToClient sender = null)
	{
		if (base.isServer && base.isClient)
		{
			UserCode_CmdPlayAirhornReactionVfxForAllClients__Boolean__NetworkConnectionToClient(reacted, sender);
			return;
		}
		NetworkWriterPooled writer = NetworkWriterPool.Get();
		writer.WriteBool(reacted);
		SendCommandInternal("System.Void PlayerInfo::CmdPlayAirhornReactionVfxForAllClients(System.Boolean,Mirror.NetworkConnectionToClient)", -766786090, writer, 0);
		NetworkWriterPool.Return(writer);
	}

	[TargetRpc]
	private void RpcPlayAirhornReactionVfx(NetworkConnectionToClient connection, bool reacted)
	{
		NetworkWriterPooled writer = NetworkWriterPool.Get();
		writer.WriteBool(reacted);
		SendTargetRPCInternal(connection, "System.Void PlayerInfo::RpcPlayAirhornReactionVfx(Mirror.NetworkConnectionToClient,System.Boolean)", 1433193511, writer, 0);
		NetworkWriterPool.Return(writer);
	}

	[TargetRpc]
	public void RpcInformInterruptedPlayerWithAirhorn()
	{
		NetworkWriterPooled writer = NetworkWriterPool.Get();
		SendTargetRPCInternal(null, "System.Void PlayerInfo::RpcInformInterruptedPlayerWithAirhorn()", -1492329430, writer, 0);
		NetworkWriterPool.Return(writer);
	}

	private void PlayAirhornReactionVfxInternal(bool reacted)
	{
		Movement.InformReactedToAirhorn();
		if (VfxPersistentData.TryGetPooledVfx(VfxType.AirhornPlayerTriggered, out var particleSystem) && particleSystem.TryGetComponent<AirhornPlayerTriggeredVfx>(out airhornReactionVfx))
		{
			airhornReactionVfx.transform.SetParent(HeadBone);
			airhornReactionVfx.transform.SetLocalPositionAndRotation(Vector3.zero, Quaternion.identity);
			airhornReactionVfx.SetItemTriggered(reacted);
			airhornReactionVfx.AsPoolableParticleSystem.Play();
		}
	}

	public Transform GetFootBone(Foot foot)
	{
		if (foot != Foot.Right)
		{
			return LeftFootBone;
		}
		return RightFootBone;
	}

	private void PopUpText(string text, bool isPenalty)
	{
		if (Movement.IsVisible)
		{
			lastPopupTimestamp = Time.timeAsDouble;
			TextPopupUi unusedPopup = TextPopupManager.GetUnusedPopup();
			unusedPopup.Initialize(isPenalty ? TextPopupManager.PenaltyPopupSettings : TextPopupManager.DefaultPopupSettings, HeadBone, GameManager.UiSettings.PlayerTextPopupLocalOffset, GameManager.UiSettings.PlayerTextPopupWorldOffset, text);
			unusedPopup.Disappeared += RemoveTextPopup;
			textPopups.Add(unusedPopup);
		}
	}

	private void RemoveTextPopup(TextPopupUi popup)
	{
		if (textPopups != null)
		{
			popup.Disappeared -= OnScorePopupDisappeared;
			textPopups.Remove(popup);
		}
	}

	private void ClearTextPopups()
	{
		foreach (TextPopupUi textPopup in textPopups)
		{
			TextPopupManager.ReturnPopup(textPopup);
		}
		textPopups.Clear();
		textPopupQueue.Clear();
		if (executeQueuedPopupsRoutine != null)
		{
			StopCoroutine(executeQueuedPopupsRoutine);
		}
	}

	private void LocalPlayerUpdateIsElectromagnetShieldActive(ItemUseId itemUseId)
	{
		bool flag = isElectromagnetShieldActive;
		NetworkisElectromagnetShieldActive = ShouldBeActive();
		if (isElectromagnetShieldActive != flag && isElectromagnetShieldActive)
		{
			NetworkelectromagnetShieldItemUseId = itemUseId;
			if (localPlayerElectromagnetRoutine != null)
			{
				StopCoroutine(localPlayerElectromagnetRoutine);
			}
			localPlayerElectromagnetRoutine = StartCoroutine(ElectromagnetShieldRoutine());
		}
		IEnumerator ElectromagnetShieldRoutine()
		{
			while (isElectromagnetShieldActive)
			{
				yield return null;
				LocalPlayerUpdateIsElectromagnetShieldActive(itemUseId);
			}
		}
		bool ShouldBeActive()
		{
			if (!Movement.IsVisible)
			{
				return false;
			}
			if (BMath.GetTimeSince(localPlayerElectromagnetShieldActivationTimestamp) > GameManager.ItemSettings.ElectromagnetShieldDuration)
			{
				return false;
			}
			return true;
		}
	}

	public void PlayElectromagnetShieldHitForAllClients(Vector3 worldDirection)
	{
		PlayElectromagnetShieldHitInternal(worldDirection);
		CmdPlayElectromagnetShieldHitForAllClients(worldDirection);
	}

	[Command(requiresAuthority = false)]
	private void CmdPlayElectromagnetShieldHitForAllClients(Vector3 worldDirection, NetworkConnectionToClient sender = null)
	{
		if (base.isServer && base.isClient)
		{
			UserCode_CmdPlayElectromagnetShieldHitForAllClients__Vector3__NetworkConnectionToClient(worldDirection, sender);
			return;
		}
		NetworkWriterPooled writer = NetworkWriterPool.Get();
		writer.WriteVector3(worldDirection);
		SendCommandInternal("System.Void PlayerInfo::CmdPlayElectromagnetShieldHitForAllClients(UnityEngine.Vector3,Mirror.NetworkConnectionToClient)", -469505504, writer, 0, requiresAuthority: false);
		NetworkWriterPool.Return(writer);
	}

	[TargetRpc]
	private void RpcPlayElectromagnetShieldHit(NetworkConnectionToClient connection, Vector3 worldDirection)
	{
		NetworkWriterPooled writer = NetworkWriterPool.Get();
		writer.WriteVector3(worldDirection);
		SendTargetRPCInternal(connection, "System.Void PlayerInfo::RpcPlayElectromagnetShieldHit(Mirror.NetworkConnectionToClient,UnityEngine.Vector3)", -1411641167, writer, 0);
		NetworkWriterPool.Return(writer);
	}

	private void PlayElectromagnetShieldHitInternal(Vector3 worldDirection)
	{
		Quaternion quaternion = Quaternion.LookRotation(worldDirection);
		if (VfxPersistentData.TryGetPooledVfx(VfxType.ElectromagnetShieldHit, out var particleSystem))
		{
			particleSystem.transform.SetParent(ElectromagnetShieldCollider.transform);
			particleSystem.transform.localPosition = Vector3.zero;
			particleSystem.transform.rotation = quaternion;
			particleSystem.Play();
		}
		Vector3 vector = quaternion * Vector3.forward;
		Vector3 worldPosition = ElectromagnetShieldCollider.transform.position + ElectromagnetShieldCollider.radius * vector;
		PlayerAudio.PlayElectromagnetShieldHitLocalOnly(worldPosition);
	}

	private void UpdateAnimationSpeedLocalOnly()
	{
		AnimatorIo.SetAnimationSpeedLocalOnly(ShouldAnimationBeFrozen() ? 0f : 1f);
		bool ShouldAnimationBeFrozen()
		{
			if (!activeGolfCartSeat.IsValid())
			{
				return false;
			}
			if (!activeGolfCartSeat.golfCart.AsEntity.AsHittable.IsFrozen)
			{
				return false;
			}
			return true;
		}
	}

	private void OnAnyPlayerGuidChanged(PlayerId playerId)
	{
		if (ShouldUpdateIsRival())
		{
			UpdateDominationState();
		}
		bool ShouldUpdateIsRival()
		{
			PlayerInfo playerInfo = PlayerId.PlayerInfo;
			if (playerInfo == this)
			{
				return true;
			}
			if (GameManager.LocalPlayerInfo != null && playerInfo == GameManager.LocalPlayerInfo)
			{
				return true;
			}
			return false;
		}
	}

	private void OnIsVisibleChanged()
	{
		if (base.isLocalPlayer)
		{
			LocalPlayerUpdateIsElectromagnetShieldActive(electromagnetShieldItemUseId);
		}
		PlayerId.OnIsVisibleChanged();
		UpdateOverheadMarkVfx();
		if (!Movement.IsVisible)
		{
			ClearTextPopups();
		}
	}

	private void OnIsFrozenChanged()
	{
		if (base.isLocalPlayer && !AsHittable.IsFrozen && AsGolfer.IsMatchResolved)
		{
			ExitGolfCart(GolfCartExitType.Default);
		}
	}

	private void OnPlayerDominationsChanged(SyncSet<CourseManager.PlayerPair>.Operation operation, CourseManager.PlayerPair value)
	{
		UpdateDominationState();
	}

	private void OnCurrentGolfCartIsFrozenChanged()
	{
		UpdateAnimationSpeedLocalOnly();
	}

	private void OnLocalPlayerGolfCartDriverSeatReserverChanged()
	{
		Hotkeys.UpdateOccupiedGolfCartSeats(activeGolfCartSeat);
	}

	private void OnLocalPlayerGolfCartPassengersChanged()
	{
		Hotkeys.UpdateOccupiedGolfCartSeats(activeGolfCartSeat);
	}

	private void OnIsPlayingOverchargedVfxChanged(bool wasPlaying, bool isPlaying)
	{
		AsGolfer.SetIsPlayingOverchargedVfx(shouldPlayOverchargedVfx);
	}

	private void OnNetworkedEquippedItemIndexChanged(int previousIndex, int currentIndex)
	{
		this.NetworkedEquippedItemIndexChanged?.Invoke();
	}

	private void OnNetworkedEquippedItemChanged(ItemType previousItem, ItemType currentItem)
	{
		AnimatorIo.OnNetworkedEquippedItemChanged(networkedEquippedItem);
	}

	private void UpdateExitButtonPrompt()
	{
		bool flag = activeGolfCartSeat.IsValid();
		if (flag && exitButtonPrompt == null)
		{
			exitButtonPrompt = ButtonPromptManager.GetButtonPrompt(PlayerInput.Controls.GolfCartShared.Exit, Localization.UI.PROMPT_Exit_Ref, ButtonPromptManager.Type.Center);
		}
		else if (!flag && exitButtonPrompt != null)
		{
			ButtonPromptManager.ReturnButtonPrompt(exitButtonPrompt);
			exitButtonPrompt = null;
		}
	}

	private void OnIsElectromagnetShieldActiveChanged(bool wasActive, bool isActive)
	{
		ElectromagnetShieldCollider.enabled = isElectromagnetShieldActive;
		if (isElectromagnetShieldActive)
		{
			ElectromagnetShieldManager.RegisterActiveShield(this);
			if (VfxPersistentData.TryGetPooledVfx(VfxType.ElectromagnetShield, out electromagnetShieldVfx))
			{
				electromagnetShieldVfx.transform.SetParent(ElectromagnetShieldCollider.transform);
				electromagnetShieldVfx.transform.SetLocalPositionAndRotation(Vector3.zero, Quaternion.identity);
				electromagnetShieldVfx.Play();
			}
			PlayerAudio.SetElectromagnetShieldActiveLocalOnly(isActive: true);
		}
		else
		{
			ElectromagnetShieldManager.DeregisterActiveShield(this);
			if (electromagnetShieldVfx != null)
			{
				electromagnetShieldVfx.Stop(ParticleSystemStopBehavior.StopEmittingAndClear);
			}
			if (wasActive && VfxPersistentData.TryGetPooledVfx(VfxType.ElectromagnetShieldEnd, out var particleSystem))
			{
				particleSystem.transform.SetParent(ElectromagnetShieldCollider.transform);
				particleSystem.transform.SetLocalPositionAndRotation(Vector3.zero, Quaternion.identity);
				particleSystem.Play();
			}
			PlayerAudio.SetElectromagnetShieldActiveLocalOnly(isActive: false);
		}
		if (base.isLocalPlayer)
		{
			PlayerInfo.LocalPlayerIsElectromagnetShieldActiveChanged?.Invoke();
		}
	}

	private void OnActiveGolfCartSeatChanged(GolfCartSeat previousSeat, GolfCartSeat currentSeat)
	{
		bool wasInGolfCart = previousSeat.IsValid();
		bool isInGolfCart = currentSeat.IsValid();
		if (base.isLocalPlayer)
		{
			Input.UpdateHotkeyMode();
			if (isInGolfCart)
			{
				AnimatorIo.EnterGolfCart(currentSeat.seat);
			}
			UpdateExitButtonPrompt();
		}
		if (isInGolfCart != wasInGolfCart)
		{
			if (isInGolfCart)
			{
				Movement.InformEnteredGolfCart();
				AsGolfer.InformEnteredGolfCart();
				Inventory.InformEnteredGolfCart();
				currentSeat.golfCart.AsEntity.AsHittable.IsFrozenChanged += OnCurrentGolfCartIsFrozenChanged;
			}
			else
			{
				Movement.InformExitedGolfCart(previousSeat.golfCart);
				Inventory.InformExitedGolfCart();
				previousSeat.golfCart.AsEntity.AsHittable.IsFrozenChanged -= OnCurrentGolfCartIsFrozenChanged;
			}
			if (base.isLocalPlayer && !isInGolfCart)
			{
				AnimatorIo.ExitGolfCart();
			}
		}
		if (base.isLocalPlayer && previousSeat.golfCart != currentSeat.golfCart)
		{
			if (previousSeat.golfCart != null)
			{
				previousSeat.golfCart.DriverSeatReserverChanged -= OnLocalPlayerGolfCartDriverSeatReserverChanged;
				previousSeat.golfCart.PassengersChanged -= OnLocalPlayerGolfCartPassengersChanged;
			}
			if (currentSeat.golfCart != null)
			{
				currentSeat.golfCart.DriverSeatReserverChanged += OnLocalPlayerGolfCartDriverSeatReserverChanged;
				currentSeat.golfCart.PassengersChanged += OnLocalPlayerGolfCartPassengersChanged;
			}
		}
		if (base.isServer && ShouldEliminate(out var eliminationReason))
		{
			AsGolfer.ServerEliminate(eliminationReason);
		}
		AsEntity.NetworkRigidbody.syncPosition = !isInGolfCart;
		AsEntity.NetworkRigidbody.syncRotation = !isInGolfCart;
		UpdateAnimationSpeedLocalOnly();
		if (isInGolfCart != wasInGolfCart)
		{
			this.IsInGolfCartChanged?.Invoke();
			if (base.isLocalPlayer)
			{
				PlayerInfo.LocalPlayerIsInGolfCartChanged?.Invoke();
			}
		}
		bool ShouldEliminate(out EliminationReason reference)
		{
			reference = EliminationReason.None;
			if (!wasInGolfCart)
			{
				return false;
			}
			if (isInGolfCart)
			{
				return false;
			}
			if (Movement.IsRespawning)
			{
				return false;
			}
			if (AsGolfer.IsMatchResolved)
			{
				return false;
			}
			if (!LevelBoundsTracker.AuthoritativeBoundsState.IsInOutOfBoundsHazard())
			{
				return false;
			}
			if (BMath.GetTimeSince(AsGolfer.ServerOutOfBoundsTimerEliminationTimestamp) < 2f)
			{
				return false;
			}
			reference = LevelBoundsTracker.GetPotentialOutOfBoundsHazardEliminationReason();
			return true;
		}
	}

	private void OnNetworkedRocketDriverThrustPowerChanged(float previousPower, float currentPower)
	{
		if (NetworkedEquippedItem == ItemType.RocketDriver && RightHandEquipmentSwitcher.TryGetComponentInChildren<RocketDriverEquipment>(out var foundComponent, includeInactive: true))
		{
			foundComponent.SetNormalizedCharge(networkedRocketDriverNormalizedCharge);
			foundComponent.Vfx.SetThrusterPower(BMath.Clamp01(networkedRocketDriverNormalizedCharge));
			foundComponent.Vfx.SetOvercharged(networkedRocketDriverNormalizedCharge > 1f);
		}
	}

	private void OnScorePopupDisappeared(TextPopupUi popup)
	{
		RemoveTextPopup(popup);
	}

	public PlayerInfo()
	{
		_Mirror_SyncVarHookDelegate_shouldPlayOverchargedVfx = OnIsPlayingOverchargedVfxChanged;
		_Mirror_SyncVarHookDelegate_networkedEquippedItemIndex = OnNetworkedEquippedItemIndexChanged;
		_Mirror_SyncVarHookDelegate_networkedEquippedItem = OnNetworkedEquippedItemChanged;
		_Mirror_SyncVarHookDelegate_networkedRocketDriverNormalizedCharge = OnNetworkedRocketDriverThrustPowerChanged;
		_Mirror_SyncVarHookDelegate_isElectromagnetShieldActive = OnIsElectromagnetShieldActiveChanged;
		_Mirror_SyncVarHookDelegate_activeGolfCartSeat = OnActiveGolfCartSeatChanged;
	}

	static PlayerInfo()
	{
		playerInfoPerPlayerGuid = new Dictionary<ulong, PlayerInfo>();
		RemoteProcedureCalls.RegisterCommand(typeof(PlayerInfo), "System.Void PlayerInfo::CmdInformCancelledEmote()", InvokeUserCode_CmdInformCancelledEmote, requiresAuthority: true);
		RemoteProcedureCalls.RegisterCommand(typeof(PlayerInfo), "System.Void PlayerInfo::CmdRestartPlayer()", InvokeUserCode_CmdRestartPlayer, requiresAuthority: true);
		RemoteProcedureCalls.RegisterCommand(typeof(PlayerInfo), "System.Void PlayerInfo::CmdInformPlayerOfBlownAirhorn(Mirror.NetworkConnectionToClient)", InvokeUserCode_CmdInformPlayerOfBlownAirhorn__NetworkConnectionToClient, requiresAuthority: false);
		RemoteProcedureCalls.RegisterCommand(typeof(PlayerInfo), "System.Void PlayerInfo::CmdPlayAirhornReactionVfxForAllClients(System.Boolean,Mirror.NetworkConnectionToClient)", InvokeUserCode_CmdPlayAirhornReactionVfxForAllClients__Boolean__NetworkConnectionToClient, requiresAuthority: true);
		RemoteProcedureCalls.RegisterCommand(typeof(PlayerInfo), "System.Void PlayerInfo::CmdPlayElectromagnetShieldHitForAllClients(UnityEngine.Vector3,Mirror.NetworkConnectionToClient)", InvokeUserCode_CmdPlayElectromagnetShieldHitForAllClients__Vector3__NetworkConnectionToClient, requiresAuthority: false);
		RemoteProcedureCalls.RegisterRpc(typeof(PlayerInfo), "System.Void PlayerInfo::RpcInformCancelledEmote()", InvokeUserCode_RpcInformCancelledEmote);
		RemoteProcedureCalls.RegisterRpc(typeof(PlayerInfo), "System.Void PlayerInfo::RpcInformAllClientsPlayerFrozen(System.Boolean)", InvokeUserCode_RpcInformAllClientsPlayerFrozen__Boolean);
		RemoteProcedureCalls.RegisterRpc(typeof(PlayerInfo), "System.Void PlayerInfo::RpcPopUpPlacementScore(System.Int32,System.Int32)", InvokeUserCode_RpcPopUpPlacementScore__Int32__Int32);
		RemoteProcedureCalls.RegisterRpc(typeof(PlayerInfo), "System.Void PlayerInfo::RpcPopUpDrivingRangeScore(System.Int32)", InvokeUserCode_RpcPopUpDrivingRangeScore__Int32);
		RemoteProcedureCalls.RegisterRpc(typeof(PlayerInfo), "System.Void PlayerInfo::RpcPopUp(PlayerTextPopupType,System.Int32)", InvokeUserCode_RpcPopUp__PlayerTextPopupType__Int32);
		RemoteProcedureCalls.RegisterRpc(typeof(PlayerInfo), "System.Void PlayerInfo::RpcAwaitSpawning()", InvokeUserCode_RpcAwaitSpawning);
		RemoteProcedureCalls.RegisterRpc(typeof(PlayerInfo), "System.Void PlayerInfo::RpcInformOfGolfCartEnterAttemptResult(GolfCartInfo,System.Boolean)", InvokeUserCode_RpcInformOfGolfCartEnterAttemptResult__GolfCartInfo__Boolean);
		RemoteProcedureCalls.RegisterRpc(typeof(PlayerInfo), "System.Void PlayerInfo::RpcInformFrozeGolfCart(System.Int32)", InvokeUserCode_RpcInformFrozeGolfCart__Int32);
		RemoteProcedureCalls.RegisterRpc(typeof(PlayerInfo), "System.Void PlayerInfo::RpcInformPlayerFrozen(System.Boolean)", InvokeUserCode_RpcInformPlayerFrozen__Boolean);
		RemoteProcedureCalls.RegisterRpc(typeof(PlayerInfo), "System.Void PlayerInfo::RpcInformOfHoleFinishStrokesUnderPar(StrokesUnderParType)", InvokeUserCode_RpcInformOfHoleFinishStrokesUnderPar__StrokesUnderParType);
		RemoteProcedureCalls.RegisterRpc(typeof(PlayerInfo), "System.Void PlayerInfo::RpcInformKnockedOutOtherPlayer(KnockoutType,UnityEngine.Vector3,System.Single,ItemType,System.Boolean,System.Boolean)", InvokeUserCode_RpcInformKnockedOutOtherPlayer__KnockoutType__Vector3__Single__ItemType__Boolean__Boolean);
		RemoteProcedureCalls.RegisterRpc(typeof(PlayerInfo), "System.Void PlayerInfo::RpcInformPickedUpItemFromItemSpawner()", InvokeUserCode_RpcInformPickedUpItemFromItemSpawner);
		RemoteProcedureCalls.RegisterRpc(typeof(PlayerInfo), "System.Void PlayerInfo::RpcInformScoredWithNoMatchKnockouts()", InvokeUserCode_RpcInformScoredWithNoMatchKnockouts);
		RemoteProcedureCalls.RegisterRpc(typeof(PlayerInfo), "System.Void PlayerInfo::RpcInformScoredRevengeKnockout()", InvokeUserCode_RpcInformScoredRevengeKnockout);
		RemoteProcedureCalls.RegisterRpc(typeof(PlayerInfo), "System.Void PlayerInfo::RpcInformEvadedHomingProjectile()", InvokeUserCode_RpcInformEvadedHomingProjectile);
		RemoteProcedureCalls.RegisterRpc(typeof(PlayerInfo), "System.Void PlayerInfo::RpcInformOfOrbitalLaserCloseCall()", InvokeUserCode_RpcInformOfOrbitalLaserCloseCall);
		RemoteProcedureCalls.RegisterRpc(typeof(PlayerInfo), "System.Void PlayerInfo::RpcInformBallEnteredHoleAfterTrajectoryDeflection()", InvokeUserCode_RpcInformBallEnteredHoleAfterTrajectoryDeflection);
		RemoteProcedureCalls.RegisterRpc(typeof(PlayerInfo), "System.Void PlayerInfo::RpcInformBallEnteredHoleAfterTrajectoryDeflectionByTreeOrFoliage()", InvokeUserCode_RpcInformBallEnteredHoleAfterTrajectoryDeflectionByTreeOrFoliage);
		RemoteProcedureCalls.RegisterRpc(typeof(PlayerInfo), "System.Void PlayerInfo::RpcInformSpectatedEntireHole()", InvokeUserCode_RpcInformSpectatedEntireHole);
		RemoteProcedureCalls.RegisterRpc(typeof(PlayerInfo), "System.Void PlayerInfo::RpcInformDisarmedLandmine()", InvokeUserCode_RpcInformDisarmedLandmine);
		RemoteProcedureCalls.RegisterRpc(typeof(PlayerInfo), "System.Void PlayerInfo::RpcInformDroveGolfCartOutOfBoundsWithOtherPassengers(EliminationReason)", InvokeUserCode_RpcInformDroveGolfCartOutOfBoundsWithOtherPassengers__EliminationReason);
		RemoteProcedureCalls.RegisterRpc(typeof(PlayerInfo), "System.Void PlayerInfo::RpcInformEliminatedOtherPlayer(EliminationReason,EliminationReason)", InvokeUserCode_RpcInformEliminatedOtherPlayer__EliminationReason__EliminationReason);
		RemoteProcedureCalls.RegisterRpc(typeof(PlayerInfo), "System.Void PlayerInfo::RpcInformOtherPlayerEliminatedWhileFrozenBySelf()", InvokeUserCode_RpcInformOtherPlayerEliminatedWhileFrozenBySelf);
		RemoteProcedureCalls.RegisterRpc(typeof(PlayerInfo), "System.Void PlayerInfo::InformKnockedOutSelfAndOtherPlayerWithExplosive()", InvokeUserCode_InformKnockedOutSelfAndOtherPlayerWithExplosive);
		RemoteProcedureCalls.RegisterRpc(typeof(PlayerInfo), "System.Void PlayerInfo::RpcInformQualifiedForThereCanBeOnlyOneAchievement()", InvokeUserCode_RpcInformQualifiedForThereCanBeOnlyOneAchievement);
		RemoteProcedureCalls.RegisterRpc(typeof(PlayerInfo), "System.Void PlayerInfo::RpcInformQualifiedTargetRichEnvironmentAchievement()", InvokeUserCode_RpcInformQualifiedTargetRichEnvironmentAchievement);
		RemoteProcedureCalls.RegisterRpc(typeof(PlayerInfo), "System.Void PlayerInfo::RpcInformQualifiedForOneTrueKingAchievement()", InvokeUserCode_RpcInformQualifiedForOneTrueKingAchievement);
		RemoteProcedureCalls.RegisterRpc(typeof(PlayerInfo), "System.Void PlayerInfo::RpcInformOfCourseEndState(CourseManager/PlayerState)", InvokeUserCode_RpcInformOfCourseEndState__PlayerState);
		RemoteProcedureCalls.RegisterRpc(typeof(PlayerInfo), "System.Void PlayerInfo::RpcInformOfBlownAirhorn(PlayerInventory)", InvokeUserCode_RpcInformOfBlownAirhorn__PlayerInventory);
		RemoteProcedureCalls.RegisterRpc(typeof(PlayerInfo), "System.Void PlayerInfo::RpcPlayAirhornReactionVfx(Mirror.NetworkConnectionToClient,System.Boolean)", InvokeUserCode_RpcPlayAirhornReactionVfx__NetworkConnectionToClient__Boolean);
		RemoteProcedureCalls.RegisterRpc(typeof(PlayerInfo), "System.Void PlayerInfo::RpcInformInterruptedPlayerWithAirhorn()", InvokeUserCode_RpcInformInterruptedPlayerWithAirhorn);
		RemoteProcedureCalls.RegisterRpc(typeof(PlayerInfo), "System.Void PlayerInfo::RpcPlayElectromagnetShieldHit(Mirror.NetworkConnectionToClient,UnityEngine.Vector3)", InvokeUserCode_RpcPlayElectromagnetShieldHit__NetworkConnectionToClient__Vector3);
	}

	public override bool Weaved()
	{
		return true;
	}

	protected void UserCode_RpcAwaitSpawning()
	{
		Movement.AwaitSpawning();
	}

	protected static void InvokeUserCode_RpcAwaitSpawning(NetworkBehaviour obj, NetworkReader reader, NetworkConnectionToClient senderConnection)
	{
		if (!NetworkClient.active)
		{
			Debug.LogError("TargetRPC RpcAwaitSpawning called on server.");
		}
		else
		{
			((PlayerInfo)obj).UserCode_RpcAwaitSpawning();
		}
	}

	protected void UserCode_CmdInformCancelledEmote()
	{
		if (serverCancelEmoteCommandRateLimiter.RegisterHit())
		{
			RpcInformCancelledEmote();
		}
	}

	protected static void InvokeUserCode_CmdInformCancelledEmote(NetworkBehaviour obj, NetworkReader reader, NetworkConnectionToClient senderConnection)
	{
		if (!NetworkServer.active)
		{
			Debug.LogError("Command CmdInformCancelledEmote called on client.");
		}
		else
		{
			((PlayerInfo)obj).UserCode_CmdInformCancelledEmote();
		}
	}

	protected void UserCode_RpcInformCancelledEmote()
	{
		PlayerAudio.InformCancelledEmote();
	}

	protected static void InvokeUserCode_RpcInformCancelledEmote(NetworkBehaviour obj, NetworkReader reader, NetworkConnectionToClient senderConnection)
	{
		if (!NetworkClient.active)
		{
			Debug.LogError("RPC RpcInformCancelledEmote called on server.");
		}
		else
		{
			((PlayerInfo)obj).UserCode_RpcInformCancelledEmote();
		}
	}

	protected void UserCode_RpcInformOfGolfCartEnterAttemptResult__GolfCartInfo__Boolean(GolfCartInfo golfCart, bool succeeded)
	{
		Movement.InformNoLongerEnteringGolfCartBeingEntered(golfCart);
	}

	protected static void InvokeUserCode_RpcInformOfGolfCartEnterAttemptResult__GolfCartInfo__Boolean(NetworkBehaviour obj, NetworkReader reader, NetworkConnectionToClient senderConnection)
	{
		if (!NetworkClient.active)
		{
			Debug.LogError("TargetRPC RpcInformOfGolfCartEnterAttemptResult called on server.");
		}
		else
		{
			((PlayerInfo)obj).UserCode_RpcInformOfGolfCartEnterAttemptResult__GolfCartInfo__Boolean(reader.ReadNetworkBehaviour<GolfCartInfo>(), reader.ReadBool());
		}
	}

	protected void UserCode_CmdRestartPlayer()
	{
		if (serverRestartPlayerCommandRateLimiter.RegisterHit() && RestartPrompt.CanBePressed(this))
		{
			RespawnTarget respawnTarget = GetRespawnTarget();
			if (Movement.TryBeginRespawn(isRestart: true, respawnTarget) && respawnTarget == RespawnTarget.Ball)
			{
				Movement.GetRespawnPosition(respawnTarget, out var _, out var position, out var _);
				CheckpointManager.ResetCheckpointForPlayerRestart(this, position);
			}
		}
		RespawnTarget GetRespawnTarget()
		{
			if (AsGolfer.IsAheadOfBall && AsGolfer.OwnBall != null && AsGolfer.OwnBall.IsStationary)
			{
				return RespawnTarget.Ball;
			}
			return RespawnTarget.TeeOrCheckpoint;
		}
	}

	protected static void InvokeUserCode_CmdRestartPlayer(NetworkBehaviour obj, NetworkReader reader, NetworkConnectionToClient senderConnection)
	{
		if (!NetworkServer.active)
		{
			Debug.LogError("Command CmdRestartPlayer called on client.");
		}
		else
		{
			((PlayerInfo)obj).UserCode_CmdRestartPlayer();
		}
	}

	protected void UserCode_RpcInformFrozeGolfCart__Int32(int otherPassengerCount)
	{
		Movement.InformFrozeGolfCart(otherPassengerCount);
	}

	protected static void InvokeUserCode_RpcInformFrozeGolfCart__Int32(NetworkBehaviour obj, NetworkReader reader, NetworkConnectionToClient senderConnection)
	{
		if (!NetworkClient.active)
		{
			Debug.LogError("TargetRPC RpcInformFrozeGolfCart called on server.");
		}
		else
		{
			((PlayerInfo)obj).UserCode_RpcInformFrozeGolfCart__Int32(reader.ReadVarInt());
		}
	}

	protected void UserCode_RpcInformPlayerFrozen__Boolean(bool isFrozen)
	{
		PlayerAudio.InformPlayerFrozen(isFrozen);
	}

	protected static void InvokeUserCode_RpcInformPlayerFrozen__Boolean(NetworkBehaviour obj, NetworkReader reader, NetworkConnectionToClient senderConnection)
	{
		if (!NetworkClient.active)
		{
			Debug.LogError("TargetRPC RpcInformPlayerFrozen called on server.");
		}
		else
		{
			((PlayerInfo)obj).UserCode_RpcInformPlayerFrozen__Boolean(reader.ReadBool());
		}
	}

	protected void UserCode_RpcInformAllClientsPlayerFrozen__Boolean(bool isFrozen)
	{
		VoiceChat.InformPlayerFrozen(isFrozen);
	}

	protected static void InvokeUserCode_RpcInformAllClientsPlayerFrozen__Boolean(NetworkBehaviour obj, NetworkReader reader, NetworkConnectionToClient senderConnection)
	{
		if (!NetworkClient.active)
		{
			Debug.LogError("RPC RpcInformAllClientsPlayerFrozen called on server.");
		}
		else
		{
			((PlayerInfo)obj).UserCode_RpcInformAllClientsPlayerFrozen__Boolean(reader.ReadBool());
		}
	}

	protected void UserCode_RpcPopUpPlacementScore__Int32__Int32(int placement, int score)
	{
		if (placement >= 0)
		{
			int num = placement + 1;
			string text = ((num > 8) ? string.Format(Localization.UI.POPUP_FinishedHigh, num) : LocalizationManager.GetString(StringTable.UI, $"POPUP_Finished{num}"));
			string text2 = text;
			text2 = AddScoreToPopupText(text2, score);
			PopUpInternal(text2, isPenalty: false);
		}
	}

	protected static void InvokeUserCode_RpcPopUpPlacementScore__Int32__Int32(NetworkBehaviour obj, NetworkReader reader, NetworkConnectionToClient senderConnection)
	{
		if (!NetworkClient.active)
		{
			Debug.LogError("RPC RpcPopUpPlacementScore called on server.");
		}
		else
		{
			((PlayerInfo)obj).UserCode_RpcPopUpPlacementScore__Int32__Int32(reader.ReadVarInt(), reader.ReadVarInt());
		}
	}

	protected void UserCode_RpcPopUpDrivingRangeScore__Int32(int score)
	{
		string popupLocalizedString = GetPopupLocalizedString(PlayerTextPopupType.ScoredOnDrivingRange);
		popupLocalizedString = AddScoreToPopupText(popupLocalizedString, score);
		PopUpInternal(popupLocalizedString, isPenalty: false);
	}

	protected static void InvokeUserCode_RpcPopUpDrivingRangeScore__Int32(NetworkBehaviour obj, NetworkReader reader, NetworkConnectionToClient senderConnection)
	{
		if (!NetworkClient.active)
		{
			Debug.LogError("RPC RpcPopUpDrivingRangeScore called on server.");
		}
		else
		{
			((PlayerInfo)obj).UserCode_RpcPopUpDrivingRangeScore__Int32(reader.ReadVarInt());
		}
	}

	protected void UserCode_RpcPopUp__PlayerTextPopupType__Int32(PlayerTextPopupType popupType, int number)
	{
		string popupLocalizedString = GetPopupLocalizedString(popupType);
		bool flag = popupType == PlayerTextPopupType.PenaltyStroke;
		popupLocalizedString = ((!flag) ? AddScoreToPopupText(popupLocalizedString, number) : AddPenaltyToPopupText(popupLocalizedString, number));
		PopUpInternal(popupLocalizedString, flag);
	}

	protected static void InvokeUserCode_RpcPopUp__PlayerTextPopupType__Int32(NetworkBehaviour obj, NetworkReader reader, NetworkConnectionToClient senderConnection)
	{
		if (!NetworkClient.active)
		{
			Debug.LogError("RPC RpcPopUp called on server.");
		}
		else
		{
			((PlayerInfo)obj).UserCode_RpcPopUp__PlayerTextPopupType__Int32(GeneratedNetworkCode._Read_PlayerTextPopupType(reader), reader.ReadVarInt());
		}
	}

	protected void UserCode_RpcInformOfHoleFinishStrokesUnderPar__StrokesUnderParType(StrokesUnderParType strokesUnderParType)
	{
		if (!SingletonBehaviour<DrivingRangeManager>.HasInstance)
		{
			if (strokesUnderParType == StrokesUnderParType.HoleInOne)
			{
				GameManager.AchievementsManager.Unlock(AchievementId.ACutAbove);
			}
			if (strokesUnderParType >= StrokesUnderParType.Birdie)
			{
				GameManager.AchievementsManager.IncrementProgress(AchievementId.EarlyBird, 1);
			}
		}
	}

	protected static void InvokeUserCode_RpcInformOfHoleFinishStrokesUnderPar__StrokesUnderParType(NetworkBehaviour obj, NetworkReader reader, NetworkConnectionToClient senderConnection)
	{
		if (!NetworkClient.active)
		{
			Debug.LogError("TargetRPC RpcInformOfHoleFinishStrokesUnderPar called on server.");
		}
		else
		{
			((PlayerInfo)obj).UserCode_RpcInformOfHoleFinishStrokesUnderPar__StrokesUnderParType(GeneratedNetworkCode._Read_StrokesUnderParType(reader));
		}
	}

	protected void UserCode_RpcInformKnockedOutOtherPlayer__KnockoutType__Vector3__Single__ItemType__Boolean__Boolean(KnockoutType knockoutType, Vector3 otherPlayerLocalOrigin, float distance, ItemType knockedOutPlayerHeldItem, bool onGreen, bool fromSpecialState)
	{
		switch (knockoutType)
		{
		case KnockoutType.Swing:
			if (!SingletonBehaviour<DrivingRangeManager>.HasInstance && onGreen)
			{
				GameManager.AchievementsManager.IncrementProgress(AchievementId.Goalie, 1);
			}
			break;
		case KnockoutType.SwingProjectile:
		case KnockoutType.RocketDriverSwingProjectile:
			if (!SingletonBehaviour<DrivingRangeManager>.HasInstance && distance > GameManager.Achievements.HomeRunDistance)
			{
				GameManager.AchievementsManager.Unlock(AchievementId.HomeRun);
			}
			break;
		case KnockoutType.DuelingPistol:
		case KnockoutType.DeflectedDuelingPistolShot:
			if (!SingletonBehaviour<DrivingRangeManager>.HasInstance)
			{
				if (knockedOutPlayerHeldItem == ItemType.DuelingPistol)
				{
					GameManager.AchievementsManager.Unlock(AchievementId.AMatterOfHonor);
				}
				if (distance > GameManager.Achievements.GunslingerDistance)
				{
					GameManager.AchievementsManager.IncrementProgress(AchievementId.Gunslinger, 1);
				}
			}
			break;
		case KnockoutType.ElephantGun:
		case KnockoutType.DeflectedElephantGunShot:
			if (!SingletonBehaviour<DrivingRangeManager>.HasInstance && distance > GameManager.Achievements.NowThatsSoldieringDistance)
			{
				GameManager.AchievementsManager.IncrementProgress(AchievementId.NowThatsSoldiering, 1);
			}
			break;
		case KnockoutType.Landmine:
			if (!SingletonBehaviour<DrivingRangeManager>.HasInstance && fromSpecialState)
			{
				GameManager.AchievementsManager.IncrementProgress(AchievementId.GreenThumb, 1);
			}
			break;
		case KnockoutType.RocketDriverSwingPostHitSpin:
			if (!SingletonBehaviour<DrivingRangeManager>.HasInstance)
			{
				GameManager.AchievementsManager.Unlock(AchievementId.RocketPirouette);
			}
			break;
		case KnockoutType.GolfCart:
			if (!SingletonBehaviour<DrivingRangeManager>.HasInstance)
			{
				GameManager.AchievementsManager.IncrementProgress(AchievementId.RoadRage, 1);
			}
			break;
		}
		if (!SingletonBehaviour<DrivingRangeManager>.HasInstance)
		{
			if (otherPlayerLocalOrigin.z < 0f && knockoutType != KnockoutType.OrbitalLaserPeriphery)
			{
				GameManager.AchievementsManager.IncrementProgress(AchievementId.BehindYou, 1);
				GameManager.AchievementsManager.IndicateProgressOnMultipleOf(AchievementId.BehindYou, 50);
			}
			CapsuleCollider uprightCollider = Movement.UprightCollider;
			var (start, end) = BGeo.GetCapsuleSphereCenters(base.transform.TransformPoint(uprightCollider.center), uprightCollider.transform.up, uprightCollider.radius, uprightCollider.height);
			if (Physics.CheckCapsule(start, end, uprightCollider.radius, GameManager.LayerSettings.FoliageMask))
			{
				GameManager.AchievementsManager.Unlock(AchievementId.LyingInWait);
			}
		}
	}

	protected static void InvokeUserCode_RpcInformKnockedOutOtherPlayer__KnockoutType__Vector3__Single__ItemType__Boolean__Boolean(NetworkBehaviour obj, NetworkReader reader, NetworkConnectionToClient senderConnection)
	{
		if (!NetworkClient.active)
		{
			Debug.LogError("TargetRPC RpcInformKnockedOutOtherPlayer called on server.");
		}
		else
		{
			((PlayerInfo)obj).UserCode_RpcInformKnockedOutOtherPlayer__KnockoutType__Vector3__Single__ItemType__Boolean__Boolean(GeneratedNetworkCode._Read_KnockoutType(reader), reader.ReadVector3(), reader.ReadFloat(), GeneratedNetworkCode._Read_ItemType(reader), reader.ReadBool(), reader.ReadBool());
		}
	}

	protected void UserCode_RpcInformPickedUpItemFromItemSpawner()
	{
		if (!SingletonBehaviour<DrivingRangeManager>.HasInstance)
		{
			GameManager.AchievementsManager.IncrementProgress(AchievementId.Hoarder, 1);
			GameManager.AchievementsManager.IndicateProgressOnMultipleOf(AchievementId.Hoarder, 200);
		}
	}

	protected static void InvokeUserCode_RpcInformPickedUpItemFromItemSpawner(NetworkBehaviour obj, NetworkReader reader, NetworkConnectionToClient senderConnection)
	{
		if (!NetworkClient.active)
		{
			Debug.LogError("TargetRPC RpcInformPickedUpItemFromItemSpawner called on server.");
		}
		else
		{
			((PlayerInfo)obj).UserCode_RpcInformPickedUpItemFromItemSpawner();
		}
	}

	protected void UserCode_RpcInformScoredWithNoMatchKnockouts()
	{
		if (!SingletonBehaviour<DrivingRangeManager>.HasInstance && CourseManager.CountActivePlayers() > 1)
		{
			GameManager.AchievementsManager.Unlock(AchievementId.Pacifist);
		}
	}

	protected static void InvokeUserCode_RpcInformScoredWithNoMatchKnockouts(NetworkBehaviour obj, NetworkReader reader, NetworkConnectionToClient senderConnection)
	{
		if (!NetworkClient.active)
		{
			Debug.LogError("TargetRPC RpcInformScoredWithNoMatchKnockouts called on server.");
		}
		else
		{
			((PlayerInfo)obj).UserCode_RpcInformScoredWithNoMatchKnockouts();
		}
	}

	protected void UserCode_RpcInformScoredRevengeKnockout()
	{
		if (!SingletonBehaviour<DrivingRangeManager>.HasInstance)
		{
			GameManager.AchievementsManager.IncrementProgress(AchievementId.BestServedCold, 1);
		}
	}

	protected static void InvokeUserCode_RpcInformScoredRevengeKnockout(NetworkBehaviour obj, NetworkReader reader, NetworkConnectionToClient senderConnection)
	{
		if (!NetworkClient.active)
		{
			Debug.LogError("TargetRPC RpcInformScoredRevengeKnockout called on server.");
		}
		else
		{
			((PlayerInfo)obj).UserCode_RpcInformScoredRevengeKnockout();
		}
	}

	protected void UserCode_RpcInformEvadedHomingProjectile()
	{
		if (!SingletonBehaviour<DrivingRangeManager>.HasInstance)
		{
			GameManager.AchievementsManager.IncrementProgress(AchievementId.CatlikeReflexes, 1);
		}
	}

	protected static void InvokeUserCode_RpcInformEvadedHomingProjectile(NetworkBehaviour obj, NetworkReader reader, NetworkConnectionToClient senderConnection)
	{
		if (!NetworkClient.active)
		{
			Debug.LogError("TargetRPC RpcInformEvadedHomingProjectile called on server.");
		}
		else
		{
			((PlayerInfo)obj).UserCode_RpcInformEvadedHomingProjectile();
		}
	}

	protected void UserCode_RpcInformOfOrbitalLaserCloseCall()
	{
		if (!SingletonBehaviour<DrivingRangeManager>.HasInstance && !AsGolfer.IsMatchResolved)
		{
			GameManager.AchievementsManager.Unlock(AchievementId.DangerClose);
		}
	}

	protected static void InvokeUserCode_RpcInformOfOrbitalLaserCloseCall(NetworkBehaviour obj, NetworkReader reader, NetworkConnectionToClient senderConnection)
	{
		if (!NetworkClient.active)
		{
			Debug.LogError("TargetRPC RpcInformOfOrbitalLaserCloseCall called on server.");
		}
		else
		{
			((PlayerInfo)obj).UserCode_RpcInformOfOrbitalLaserCloseCall();
		}
	}

	protected void UserCode_RpcInformBallEnteredHoleAfterTrajectoryDeflection()
	{
		if (!SingletonBehaviour<DrivingRangeManager>.HasInstance && CourseManager.CountActivePlayers() > 1)
		{
			GameManager.AchievementsManager.Unlock(AchievementId.FailedSuccessfully);
		}
	}

	protected static void InvokeUserCode_RpcInformBallEnteredHoleAfterTrajectoryDeflection(NetworkBehaviour obj, NetworkReader reader, NetworkConnectionToClient senderConnection)
	{
		if (!NetworkClient.active)
		{
			Debug.LogError("TargetRPC RpcInformBallEnteredHoleAfterTrajectoryDeflection called on server.");
		}
		else
		{
			((PlayerInfo)obj).UserCode_RpcInformBallEnteredHoleAfterTrajectoryDeflection();
		}
	}

	protected void UserCode_RpcInformBallEnteredHoleAfterTrajectoryDeflectionByTreeOrFoliage()
	{
		if (!SingletonBehaviour<DrivingRangeManager>.HasInstance)
		{
			GameManager.AchievementsManager.Unlock(AchievementId.TigerInTheWoods);
		}
	}

	protected static void InvokeUserCode_RpcInformBallEnteredHoleAfterTrajectoryDeflectionByTreeOrFoliage(NetworkBehaviour obj, NetworkReader reader, NetworkConnectionToClient senderConnection)
	{
		if (!NetworkClient.active)
		{
			Debug.LogError("TargetRPC RpcInformBallEnteredHoleAfterTrajectoryDeflectionByTreeOrFoliage called on server.");
		}
		else
		{
			((PlayerInfo)obj).UserCode_RpcInformBallEnteredHoleAfterTrajectoryDeflectionByTreeOrFoliage();
		}
	}

	protected void UserCode_RpcInformSpectatedEntireHole()
	{
		if (!SingletonBehaviour<DrivingRangeManager>.HasInstance)
		{
			GameManager.AchievementsManager.Unlock(AchievementId.CouchPotato);
		}
	}

	protected static void InvokeUserCode_RpcInformSpectatedEntireHole(NetworkBehaviour obj, NetworkReader reader, NetworkConnectionToClient senderConnection)
	{
		if (!NetworkClient.active)
		{
			Debug.LogError("TargetRPC RpcInformSpectatedEntireHole called on server.");
		}
		else
		{
			((PlayerInfo)obj).UserCode_RpcInformSpectatedEntireHole();
		}
	}

	protected void UserCode_RpcInformDisarmedLandmine()
	{
		if (!SingletonBehaviour<DrivingRangeManager>.HasInstance)
		{
			GameManager.AchievementsManager.Unlock(AchievementId.SafetyFirst);
		}
	}

	protected static void InvokeUserCode_RpcInformDisarmedLandmine(NetworkBehaviour obj, NetworkReader reader, NetworkConnectionToClient senderConnection)
	{
		if (!NetworkClient.active)
		{
			Debug.LogError("TargetRPC RpcInformDisarmedLandmine called on server.");
		}
		else
		{
			((PlayerInfo)obj).UserCode_RpcInformDisarmedLandmine();
		}
	}

	protected void UserCode_RpcInformDroveGolfCartOutOfBoundsWithOtherPassengers__EliminationReason(EliminationReason eliminationReason)
	{
		if (!SingletonBehaviour<DrivingRangeManager>.HasInstance && eliminationReason == EliminationReason.FellIntoWater)
		{
			GameManager.AchievementsManager.Unlock(AchievementId.UnderwaterExpedition);
		}
	}

	protected static void InvokeUserCode_RpcInformDroveGolfCartOutOfBoundsWithOtherPassengers__EliminationReason(NetworkBehaviour obj, NetworkReader reader, NetworkConnectionToClient senderConnection)
	{
		if (!NetworkClient.active)
		{
			Debug.LogError("TargetRPC RpcInformDroveGolfCartOutOfBoundsWithOtherPassengers called on server.");
		}
		else
		{
			((PlayerInfo)obj).UserCode_RpcInformDroveGolfCartOutOfBoundsWithOtherPassengers__EliminationReason(GeneratedNetworkCode._Read_EliminationReason(reader));
		}
	}

	protected void UserCode_RpcInformEliminatedOtherPlayer__EliminationReason__EliminationReason(EliminationReason reason, EliminationReason immediateEliminationReason)
	{
		if (!SingletonBehaviour<DrivingRangeManager>.HasInstance && immediateEliminationReason == EliminationReason.FellIntoWater)
		{
			GameManager.AchievementsManager.IncrementProgress(AchievementId.WalkThePlank, 1);
		}
	}

	protected static void InvokeUserCode_RpcInformEliminatedOtherPlayer__EliminationReason__EliminationReason(NetworkBehaviour obj, NetworkReader reader, NetworkConnectionToClient senderConnection)
	{
		if (!NetworkClient.active)
		{
			Debug.LogError("TargetRPC RpcInformEliminatedOtherPlayer called on server.");
		}
		else
		{
			((PlayerInfo)obj).UserCode_RpcInformEliminatedOtherPlayer__EliminationReason__EliminationReason(GeneratedNetworkCode._Read_EliminationReason(reader), GeneratedNetworkCode._Read_EliminationReason(reader));
		}
	}

	protected void UserCode_RpcInformOtherPlayerEliminatedWhileFrozenBySelf()
	{
		if (!SingletonBehaviour<DrivingRangeManager>.HasInstance)
		{
			GameManager.AchievementsManager.IncrementProgress(AchievementId.IceToSeeYou, 1);
		}
	}

	protected static void InvokeUserCode_RpcInformOtherPlayerEliminatedWhileFrozenBySelf(NetworkBehaviour obj, NetworkReader reader, NetworkConnectionToClient senderConnection)
	{
		if (!NetworkClient.active)
		{
			Debug.LogError("TargetRPC RpcInformOtherPlayerEliminatedWhileFrozenBySelf called on server.");
		}
		else
		{
			((PlayerInfo)obj).UserCode_RpcInformOtherPlayerEliminatedWhileFrozenBySelf();
		}
	}

	protected void UserCode_InformKnockedOutSelfAndOtherPlayerWithExplosive()
	{
		if (!SingletonBehaviour<DrivingRangeManager>.HasInstance)
		{
			GameManager.AchievementsManager.Unlock(AchievementId.PlayingWithFire);
		}
	}

	protected static void InvokeUserCode_InformKnockedOutSelfAndOtherPlayerWithExplosive(NetworkBehaviour obj, NetworkReader reader, NetworkConnectionToClient senderConnection)
	{
		if (!NetworkClient.active)
		{
			Debug.LogError("TargetRPC InformKnockedOutSelfAndOtherPlayerWithExplosive called on server.");
		}
		else
		{
			((PlayerInfo)obj).UserCode_InformKnockedOutSelfAndOtherPlayerWithExplosive();
		}
	}

	protected void UserCode_RpcInformQualifiedForThereCanBeOnlyOneAchievement()
	{
		if (!SingletonBehaviour<DrivingRangeManager>.HasInstance)
		{
			GameManager.AchievementsManager.Unlock(AchievementId.ThereCanBeOnlyOne);
		}
	}

	protected static void InvokeUserCode_RpcInformQualifiedForThereCanBeOnlyOneAchievement(NetworkBehaviour obj, NetworkReader reader, NetworkConnectionToClient senderConnection)
	{
		if (!NetworkClient.active)
		{
			Debug.LogError("TargetRPC RpcInformQualifiedForThereCanBeOnlyOneAchievement called on server.");
		}
		else
		{
			((PlayerInfo)obj).UserCode_RpcInformQualifiedForThereCanBeOnlyOneAchievement();
		}
	}

	protected void UserCode_RpcInformQualifiedTargetRichEnvironmentAchievement()
	{
		if (!SingletonBehaviour<DrivingRangeManager>.HasInstance)
		{
			GameManager.AchievementsManager.Unlock(AchievementId.TargetRichEnvironment);
		}
	}

	protected static void InvokeUserCode_RpcInformQualifiedTargetRichEnvironmentAchievement(NetworkBehaviour obj, NetworkReader reader, NetworkConnectionToClient senderConnection)
	{
		if (!NetworkClient.active)
		{
			Debug.LogError("TargetRPC RpcInformQualifiedTargetRichEnvironmentAchievement called on server.");
		}
		else
		{
			((PlayerInfo)obj).UserCode_RpcInformQualifiedTargetRichEnvironmentAchievement();
		}
	}

	protected void UserCode_RpcInformQualifiedForOneTrueKingAchievement()
	{
		if (!SingletonBehaviour<DrivingRangeManager>.HasInstance)
		{
			GameManager.AchievementsManager.Unlock(AchievementId.OneTrueKing);
		}
	}

	protected static void InvokeUserCode_RpcInformQualifiedForOneTrueKingAchievement(NetworkBehaviour obj, NetworkReader reader, NetworkConnectionToClient senderConnection)
	{
		if (!NetworkClient.active)
		{
			Debug.LogError("TargetRPC RpcInformQualifiedForOneTrueKingAchievement called on server.");
		}
		else
		{
			((PlayerInfo)obj).UserCode_RpcInformQualifiedForOneTrueKingAchievement();
		}
	}

	protected void UserCode_RpcInformOfCourseEndState__PlayerState(CourseManager.PlayerState localPlayerState)
	{
		TryAwardAchievements();
		void TryAwardAchievements()
		{
			if (!SingletonBehaviour<DrivingRangeManager>.HasInstance && !(GameManager.CurrentCourse == null) && !(GameManager.LocalPlayerInfo == null))
			{
				TryAwardCoolheadedAchievement();
			}
		}
		void TryAwardCoolheadedAchievement()
		{
			int num = GameManager.CurrentCourse.Holes.Length;
			if (num >= GameManager.Achievements.CoolheadedMinHoleCount && localPlayerState.finishes >= num && localPlayerState.TryGetStrokesRelativeToPar(out var strokes) && strokes <= GameManager.Achievements.CoolheadedMaxStrokesRelativeToPar)
			{
				GameManager.AchievementsManager.Unlock(AchievementId.Coolheaded);
			}
		}
	}

	protected static void InvokeUserCode_RpcInformOfCourseEndState__PlayerState(NetworkBehaviour obj, NetworkReader reader, NetworkConnectionToClient senderConnection)
	{
		if (!NetworkClient.active)
		{
			Debug.LogError("TargetRPC RpcInformOfCourseEndState called on server.");
		}
		else
		{
			((PlayerInfo)obj).UserCode_RpcInformOfCourseEndState__PlayerState(GeneratedNetworkCode._Read_CourseManager_002FPlayerState(reader));
		}
	}

	protected void UserCode_CmdInformPlayerOfBlownAirhorn__NetworkConnectionToClient(NetworkConnectionToClient sender)
	{
		if (serverInformPlayerOfBlownAirhornCommandRateLimiter.RegisterHit(sender))
		{
			PlayerInfo value;
			if (sender == null)
			{
				value = GameManager.LocalPlayerInfo;
			}
			else
			{
				GameManager.RemotePlayerPerConnectionId.TryGetValue(sender.connectionId, out value);
			}
			if (!(value == null))
			{
				RpcInformOfBlownAirhorn(value.Inventory);
			}
		}
	}

	protected static void InvokeUserCode_CmdInformPlayerOfBlownAirhorn__NetworkConnectionToClient(NetworkBehaviour obj, NetworkReader reader, NetworkConnectionToClient senderConnection)
	{
		if (!NetworkServer.active)
		{
			Debug.LogError("Command CmdInformPlayerOfBlownAirhorn called on client.");
		}
		else
		{
			((PlayerInfo)obj).UserCode_CmdInformPlayerOfBlownAirhorn__NetworkConnectionToClient(senderConnection);
		}
	}

	protected void UserCode_RpcInformOfBlownAirhorn__PlayerInventory(PlayerInventory airhornBlower)
	{
		bool flag = TryReactToBlownAirhorn();
		PlayAirhornReactionVfxForAllClients(flag);
		if (flag && airhornBlower != null)
		{
			airhornBlower.CmdInformInterruptedPlayerWithAirhorn();
		}
		void PlayAirhornReactionVfxForAllClients(bool reacted)
		{
			PlayAirhornReactionVfxInternal(reacted);
			CmdPlayAirhornReactionVfxForAllClients(reacted);
		}
		bool TryReactToBlownAirhorn()
		{
			if (Inventory.TryReactToBlownAirhorn())
			{
				return true;
			}
			if (AsGolfer.TryReactToBlownAirhorn())
			{
				return true;
			}
			return false;
		}
	}

	protected static void InvokeUserCode_RpcInformOfBlownAirhorn__PlayerInventory(NetworkBehaviour obj, NetworkReader reader, NetworkConnectionToClient senderConnection)
	{
		if (!NetworkClient.active)
		{
			Debug.LogError("TargetRPC RpcInformOfBlownAirhorn called on server.");
		}
		else
		{
			((PlayerInfo)obj).UserCode_RpcInformOfBlownAirhorn__PlayerInventory(reader.ReadNetworkBehaviour<PlayerInventory>());
		}
	}

	protected void UserCode_CmdPlayAirhornReactionVfxForAllClients__Boolean__NetworkConnectionToClient(bool reacted, NetworkConnectionToClient sender)
	{
		if (!serverAirhornReactionVfxCommandRateLimiter.RegisterHit())
		{
			return;
		}
		if (sender != null && sender != NetworkServer.localConnection)
		{
			PlayAirhornReactionVfxInternal(reacted);
		}
		foreach (NetworkConnectionToClient value in NetworkServer.connections.Values)
		{
			if (value != NetworkServer.localConnection && value != sender)
			{
				RpcPlayAirhornReactionVfx(value, reacted);
			}
		}
	}

	protected static void InvokeUserCode_CmdPlayAirhornReactionVfxForAllClients__Boolean__NetworkConnectionToClient(NetworkBehaviour obj, NetworkReader reader, NetworkConnectionToClient senderConnection)
	{
		if (!NetworkServer.active)
		{
			Debug.LogError("Command CmdPlayAirhornReactionVfxForAllClients called on client.");
		}
		else
		{
			((PlayerInfo)obj).UserCode_CmdPlayAirhornReactionVfxForAllClients__Boolean__NetworkConnectionToClient(reader.ReadBool(), senderConnection);
		}
	}

	protected void UserCode_RpcPlayAirhornReactionVfx__NetworkConnectionToClient__Boolean(NetworkConnectionToClient connection, bool reacted)
	{
		PlayAirhornReactionVfxInternal(reacted);
	}

	protected static void InvokeUserCode_RpcPlayAirhornReactionVfx__NetworkConnectionToClient__Boolean(NetworkBehaviour obj, NetworkReader reader, NetworkConnectionToClient senderConnection)
	{
		if (!NetworkClient.active)
		{
			Debug.LogError("TargetRPC RpcPlayAirhornReactionVfx called on server.");
		}
		else
		{
			((PlayerInfo)obj).UserCode_RpcPlayAirhornReactionVfx__NetworkConnectionToClient__Boolean(null, reader.ReadBool());
		}
	}

	protected void UserCode_RpcInformInterruptedPlayerWithAirhorn()
	{
		if (!SingletonBehaviour<DrivingRangeManager>.HasInstance)
		{
			GameManager.AchievementsManager.IncrementProgress(AchievementId.ClowningAround, 1);
		}
	}

	protected static void InvokeUserCode_RpcInformInterruptedPlayerWithAirhorn(NetworkBehaviour obj, NetworkReader reader, NetworkConnectionToClient senderConnection)
	{
		if (!NetworkClient.active)
		{
			Debug.LogError("TargetRPC RpcInformInterruptedPlayerWithAirhorn called on server.");
		}
		else
		{
			((PlayerInfo)obj).UserCode_RpcInformInterruptedPlayerWithAirhorn();
		}
	}

	protected void UserCode_CmdPlayElectromagnetShieldHitForAllClients__Vector3__NetworkConnectionToClient(Vector3 worldDirection, NetworkConnectionToClient sender)
	{
		if (!serverElectromagnetShieldHitCommandRateLimiter.RegisterHit(sender))
		{
			return;
		}
		if (sender != null && sender != NetworkServer.localConnection)
		{
			PlayElectromagnetShieldHitInternal(worldDirection);
		}
		foreach (NetworkConnectionToClient value in NetworkServer.connections.Values)
		{
			if (value != NetworkServer.localConnection && value != sender)
			{
				RpcPlayElectromagnetShieldHit(value, worldDirection);
			}
		}
	}

	protected static void InvokeUserCode_CmdPlayElectromagnetShieldHitForAllClients__Vector3__NetworkConnectionToClient(NetworkBehaviour obj, NetworkReader reader, NetworkConnectionToClient senderConnection)
	{
		if (!NetworkServer.active)
		{
			Debug.LogError("Command CmdPlayElectromagnetShieldHitForAllClients called on client.");
		}
		else
		{
			((PlayerInfo)obj).UserCode_CmdPlayElectromagnetShieldHitForAllClients__Vector3__NetworkConnectionToClient(reader.ReadVector3(), senderConnection);
		}
	}

	protected void UserCode_RpcPlayElectromagnetShieldHit__NetworkConnectionToClient__Vector3(NetworkConnectionToClient connection, Vector3 worldDirection)
	{
		PlayElectromagnetShieldHitInternal(worldDirection);
	}

	protected static void InvokeUserCode_RpcPlayElectromagnetShieldHit__NetworkConnectionToClient__Vector3(NetworkBehaviour obj, NetworkReader reader, NetworkConnectionToClient senderConnection)
	{
		if (!NetworkClient.active)
		{
			Debug.LogError("TargetRPC RpcPlayElectromagnetShieldHit called on server.");
		}
		else
		{
			((PlayerInfo)obj).UserCode_RpcPlayElectromagnetShieldHit__NetworkConnectionToClient__Vector3(null, reader.ReadVector3());
		}
	}

	public override void SerializeSyncVars(NetworkWriter writer, bool forceAll)
	{
		base.SerializeSyncVars(writer, forceAll);
		if (forceAll)
		{
			writer.WriteBool(networkedIsAimingSwing);
			writer.WriteBool(shouldPlayOverchargedVfx);
			writer.WriteBool(networkedIsAimingItem);
			writer.WriteVarInt(networkedEquippedItemIndex);
			GeneratedNetworkCode._Write_ItemType(writer, networkedEquippedItem);
			writer.WriteFloat(networkedRocketDriverNormalizedCharge);
			writer.WriteBool(isElectromagnetShieldActive);
			GeneratedNetworkCode._Write_ItemUseId(writer, electromagnetShieldItemUseId);
			GeneratedNetworkCode._Write_GolfCartSeat(writer, activeGolfCartSeat);
			return;
		}
		writer.WriteVarULong(syncVarDirtyBits);
		if ((syncVarDirtyBits & 1L) != 0L)
		{
			writer.WriteBool(networkedIsAimingSwing);
		}
		if ((syncVarDirtyBits & 2L) != 0L)
		{
			writer.WriteBool(shouldPlayOverchargedVfx);
		}
		if ((syncVarDirtyBits & 4L) != 0L)
		{
			writer.WriteBool(networkedIsAimingItem);
		}
		if ((syncVarDirtyBits & 8L) != 0L)
		{
			writer.WriteVarInt(networkedEquippedItemIndex);
		}
		if ((syncVarDirtyBits & 0x10L) != 0L)
		{
			GeneratedNetworkCode._Write_ItemType(writer, networkedEquippedItem);
		}
		if ((syncVarDirtyBits & 0x20L) != 0L)
		{
			writer.WriteFloat(networkedRocketDriverNormalizedCharge);
		}
		if ((syncVarDirtyBits & 0x40L) != 0L)
		{
			writer.WriteBool(isElectromagnetShieldActive);
		}
		if ((syncVarDirtyBits & 0x80L) != 0L)
		{
			GeneratedNetworkCode._Write_ItemUseId(writer, electromagnetShieldItemUseId);
		}
		if ((syncVarDirtyBits & 0x100L) != 0L)
		{
			GeneratedNetworkCode._Write_GolfCartSeat(writer, activeGolfCartSeat);
		}
	}

	public override void DeserializeSyncVars(NetworkReader reader, bool initialState)
	{
		base.DeserializeSyncVars(reader, initialState);
		if (initialState)
		{
			GeneratedSyncVarDeserialize(ref networkedIsAimingSwing, null, reader.ReadBool());
			GeneratedSyncVarDeserialize(ref shouldPlayOverchargedVfx, _Mirror_SyncVarHookDelegate_shouldPlayOverchargedVfx, reader.ReadBool());
			GeneratedSyncVarDeserialize(ref networkedIsAimingItem, null, reader.ReadBool());
			GeneratedSyncVarDeserialize(ref networkedEquippedItemIndex, _Mirror_SyncVarHookDelegate_networkedEquippedItemIndex, reader.ReadVarInt());
			GeneratedSyncVarDeserialize(ref networkedEquippedItem, _Mirror_SyncVarHookDelegate_networkedEquippedItem, GeneratedNetworkCode._Read_ItemType(reader));
			GeneratedSyncVarDeserialize(ref networkedRocketDriverNormalizedCharge, _Mirror_SyncVarHookDelegate_networkedRocketDriverNormalizedCharge, reader.ReadFloat());
			GeneratedSyncVarDeserialize(ref isElectromagnetShieldActive, _Mirror_SyncVarHookDelegate_isElectromagnetShieldActive, reader.ReadBool());
			GeneratedSyncVarDeserialize(ref electromagnetShieldItemUseId, null, GeneratedNetworkCode._Read_ItemUseId(reader));
			GeneratedSyncVarDeserialize(ref activeGolfCartSeat, _Mirror_SyncVarHookDelegate_activeGolfCartSeat, GeneratedNetworkCode._Read_GolfCartSeat(reader));
			return;
		}
		long num = (long)reader.ReadVarULong();
		if ((num & 1L) != 0L)
		{
			GeneratedSyncVarDeserialize(ref networkedIsAimingSwing, null, reader.ReadBool());
		}
		if ((num & 2L) != 0L)
		{
			GeneratedSyncVarDeserialize(ref shouldPlayOverchargedVfx, _Mirror_SyncVarHookDelegate_shouldPlayOverchargedVfx, reader.ReadBool());
		}
		if ((num & 4L) != 0L)
		{
			GeneratedSyncVarDeserialize(ref networkedIsAimingItem, null, reader.ReadBool());
		}
		if ((num & 8L) != 0L)
		{
			GeneratedSyncVarDeserialize(ref networkedEquippedItemIndex, _Mirror_SyncVarHookDelegate_networkedEquippedItemIndex, reader.ReadVarInt());
		}
		if ((num & 0x10L) != 0L)
		{
			GeneratedSyncVarDeserialize(ref networkedEquippedItem, _Mirror_SyncVarHookDelegate_networkedEquippedItem, GeneratedNetworkCode._Read_ItemType(reader));
		}
		if ((num & 0x20L) != 0L)
		{
			GeneratedSyncVarDeserialize(ref networkedRocketDriverNormalizedCharge, _Mirror_SyncVarHookDelegate_networkedRocketDriverNormalizedCharge, reader.ReadFloat());
		}
		if ((num & 0x40L) != 0L)
		{
			GeneratedSyncVarDeserialize(ref isElectromagnetShieldActive, _Mirror_SyncVarHookDelegate_isElectromagnetShieldActive, reader.ReadBool());
		}
		if ((num & 0x80L) != 0L)
		{
			GeneratedSyncVarDeserialize(ref electromagnetShieldItemUseId, null, GeneratedNetworkCode._Read_ItemUseId(reader));
		}
		if ((num & 0x100L) != 0L)
		{
			GeneratedSyncVarDeserialize(ref activeGolfCartSeat, _Mirror_SyncVarHookDelegate_activeGolfCartSeat, GeneratedNetworkCode._Read_GolfCartSeat(reader));
		}
	}
}
