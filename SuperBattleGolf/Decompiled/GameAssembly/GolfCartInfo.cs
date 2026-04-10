using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using Cysharp.Threading.Tasks;
using FMOD.Studio;
using FMODUnity;
using Mirror;
using Mirror.RemoteCalls;
using UnityEngine;
using UnityEngine.Localization;

public class GolfCartInfo : NetworkBehaviour, IInteractable
{
	[SerializeField]
	private Collider[] passengerColliders;

	[SerializeField]
	private BoxCollider itemCollectorCollider;

	[SerializeField]
	private GameObject enginePosition;

	[SerializeField]
	private GolfCartVfx vfx;

	public readonly SyncList<PlayerInfo> passengers = new SyncList<PlayerInfo>();

	private readonly Dictionary<PlayerInfo, int> passengerIndices = new Dictionary<PlayerInfo, int>();

	[SyncVar(hook = "OnDriverSeatReserverChanged")]
	private PlayerInfo driverSeatReserver;

	[SyncVar]
	private PlayerInfo responsiblePlayer;

	private Coroutine responsiblePlayerTimeoutRoutine;

	[SyncVar]
	private float outOfBoundsRemainingTime;

	private Coroutine turnOnHeadlightsRoutine;

	private EventInstance engineSoundInstance;

	private float engineSoundIntensity;

	private EventInstance honkSoundInstance;

	private EventInstance specialHonkSoundInstance;

	private double honkVfxTimestamp = double.MinValue;

	private readonly AntiCheatPerPlayerRateChecker serverEnterReservedGolfCartCommandRateLimiter = new AntiCheatPerPlayerRateChecker("Enter reserved golf cart", 0.5f, 5, 10, 2f);

	private readonly AntiCheatPerPlayerRateChecker serverEnterCommandRateLimiter = new AntiCheatPerPlayerRateChecker("Enter golf cart", 0.05f, 10, 20, 1f);

	private readonly AntiCheatPerPlayerRateChecker serverCancelDriverSeatReservationCommandRateLimiter = new AntiCheatPerPlayerRateChecker("Cancel driver seat reservation", 0.5f, 5, 10, 2f);

	private readonly AntiCheatPerPlayerRateChecker serverTryChangeSeatCommandRateLimiter = new AntiCheatPerPlayerRateChecker("Try change seat", 0.005f, 10, 20, 0.01f);

	private readonly AntiCheatPerPlayerRateChecker serverInformDrivingSeatReservationReceivedCommandRateLimiter = new AntiCheatPerPlayerRateChecker("Inform driving seat reservation received", 0.5f, 5, 10, 2f);

	private readonly AntiCheatPerPlayerRateChecker serverExitCommandRateLimiter = new AntiCheatPerPlayerRateChecker("Exit golf cart", 0.05f, 10, 20, 1f);

	private readonly AntiCheatPerPlayerRateChecker serverPlayHonkCommandRateLimiter = new AntiCheatPerPlayerRateChecker("Golf cart honk", 0.025f, 10, 20, 1f, 3);

	private readonly AntiCheatPerPlayerRateChecker serverStartPlayingSpecialHonkCommandRateLimiter = new AntiCheatPerPlayerRateChecker("Golf cart special honk start", 0.025f, 5, 10, 1f, 2);

	private readonly AntiCheatPerPlayerRateChecker serverEndSpecialHonkCommandRateLimiter = new AntiCheatPerPlayerRateChecker("Golf cart special honk end", 0.025f, 5, 10, 1f, 2);

	protected NetworkBehaviourSyncVar ___driverSeatReserverNetId;

	protected NetworkBehaviourSyncVar ___responsiblePlayerNetId;

	public Action<PlayerInfo, PlayerInfo> _Mirror_SyncVarHookDelegate_driverSeatReserver;

	[field: SerializeField]
	public Transform ExhaustPosition { get; private set; }

	public Entity AsEntity { get; private set; }

	public GolfCartMovement Movement { get; private set; }

	public PlayerInfo ResponsiblePlayer => NetworkresponsiblePlayer;

	public float OutOfBoundsRemainingTime => outOfBoundsRemainingTime;

	public bool IsInteractionEnabled { get; private set; }

	public LocalizedString InteractString => Localization.UI.PROMPT_Enter_Ref;

	public bool IsHidden { get; private set; }

	public BoxCollider ItemCollectorCollider => itemCollectorCollider;

	public GolfCartVfx Vfx => vfx;

	public PlayerInfo NetworkdriverSeatReserver
	{
		get
		{
			return GetSyncVarNetworkBehaviour(___driverSeatReserverNetId, ref driverSeatReserver);
		}
		[param: In]
		set
		{
			GeneratedSyncVarSetter_NetworkBehaviour(value, ref driverSeatReserver, 1uL, _Mirror_SyncVarHookDelegate_driverSeatReserver, ref ___driverSeatReserverNetId);
		}
	}

	public PlayerInfo NetworkresponsiblePlayer
	{
		get
		{
			return GetSyncVarNetworkBehaviour(___responsiblePlayerNetId, ref responsiblePlayer);
		}
		[param: In]
		set
		{
			GeneratedSyncVarSetter_NetworkBehaviour(value, ref responsiblePlayer, 2uL, null, ref ___responsiblePlayerNetId);
		}
	}

	public float NetworkoutOfBoundsRemainingTime
	{
		get
		{
			return outOfBoundsRemainingTime;
		}
		[param: In]
		set
		{
			GeneratedSyncVarSetter(value, ref outOfBoundsRemainingTime, 4uL, null);
		}
	}

	public event Action DriverSeatReserverChanged;

	public event Action PassengersChanged;

	private void Awake()
	{
		AsEntity = GetComponent<Entity>();
		Movement = GetComponent<GolfCartMovement>();
		for (int i = 0; i < GameManager.GolfCartSettings.MaxPassengers; i++)
		{
			passengerColliders[i].gameObject.SetActive(i < passengers.Count && passengers[i] != null);
		}
		SyncList<PlayerInfo> syncList = passengers;
		syncList.OnChange = (Action<SyncList<PlayerInfo>.Operation, int, PlayerInfo>)Delegate.Combine(syncList.OnChange, new Action<SyncList<PlayerInfo>.Operation, int, PlayerInfo>(OnPassengersChanged));
	}

	private void Start()
	{
		AsEntity.AsHittable.IsFrozenChanged += OnIsFrozenChanged;
	}

	public void OnWillBeDestroyed()
	{
		if (GameManager.LocalPlayerInfo != null && NetworkdriverSeatReserver == GameManager.LocalPlayerInfo)
		{
			GameManager.LocalPlayerInventory.ItemUseCancelled -= OnLocalPlayerDriverSearReserverItemUseCanceled;
		}
		if (engineSoundInstance.isValid())
		{
			engineSoundInstance.stop(FMOD.Studio.STOP_MODE.ALLOWFADEOUT);
		}
		if (honkSoundInstance.isValid())
		{
			honkSoundInstance.stop(FMOD.Studio.STOP_MODE.ALLOWFADEOUT);
		}
		if (specialHonkSoundInstance.isValid())
		{
			specialHonkSoundInstance.stop(FMOD.Studio.STOP_MODE.ALLOWFADEOUT);
		}
		if (BNetworkManager.IsChangingSceneOrShuttingDown)
		{
			return;
		}
		SyncList<PlayerInfo> syncList = passengers;
		syncList.OnChange = (Action<SyncList<PlayerInfo>.Operation, int, PlayerInfo>)Delegate.Remove(syncList.OnChange, new Action<SyncList<PlayerInfo>.Operation, int, PlayerInfo>(OnPassengersChanged));
		AsEntity.AsHittable.IsFrozenChanged -= OnIsFrozenChanged;
		if (base.isServer && NetworkdriverSeatReserver != null)
		{
			NetworkdriverSeatReserver.AsEntity.WillBeDestroyed -= OnServerDriverSeatReserverWillBeDestroyed;
		}
		foreach (PlayerInfo passenger in passengers)
		{
			if (!(passenger == null))
			{
				passenger.AsEntity.WillBeDestroyedReferenced -= OnServerPassengerWillBeDestroyed;
				if (passenger.isLocalPlayer)
				{
					passenger.ExitGolfCart(GolfCartExitType.Dive);
				}
			}
		}
	}

	public override void OnStartServer()
	{
		for (int i = 0; i < GameManager.GolfCartSettings.MaxPassengers; i++)
		{
			passengers.Add(null);
		}
		UpdateIsInteractionEnabled();
		AsEntity.LevelBoundsTracker.AuthoritativeBoundsStateChanged += OnServerBoundsStateChanged;
	}

	public override void OnStopServer()
	{
		AsEntity.LevelBoundsTracker.AuthoritativeBoundsStateChanged -= OnServerBoundsStateChanged;
	}

	public override void OnStartClient()
	{
		if (!base.isServer)
		{
			UpdateIsInteractionEnabled();
		}
		PlayerInfo driver;
		if (NetworkdriverSeatReserver != null)
		{
			SetMovementSyncDirectionInternal((!NetworkdriverSeatReserver.PlayerId.IsPartyLeader) ? SyncDirection.ClientToServer : SyncDirection.ServerToClient);
			if (GameManager.LocalPlayerInfo != null && NetworkdriverSeatReserver == GameManager.LocalPlayerInfo && !GameManager.LocalPlayerInventory.TrySetReservedGolfCart(this))
			{
				CmdCancelDriverSeatReservation();
			}
		}
		else if (TryGetDriver(out driver))
		{
			SetMovementSyncDirectionInternal((!driver.PlayerId.IsPartyLeader) ? SyncDirection.ClientToServer : SyncDirection.ServerToClient);
		}
		if (!engineSoundInstance.isValid() && TryGetDriver(out var _))
		{
			engineSoundInstance = RuntimeManager.CreateInstance(GameManager.AudioSettings.GolfCartEngine);
			RuntimeManager.AttachInstanceToGameObject(engineSoundInstance, enginePosition);
			engineSoundInstance.start();
			engineSoundInstance.release();
			engineSoundIntensity = 0f;
		}
		PlayerInfo.LocalPlayerIsElectromagnetShieldActiveChanged += OnClientLocalPlayerIsElectromagnetShieldActiveChanged;
	}

	public override void OnStopClient()
	{
		PlayerInfo.LocalPlayerIsElectromagnetShieldActiveChanged -= OnClientLocalPlayerIsElectromagnetShieldActiveChanged;
	}

	public void OnUpdate()
	{
		if (base.isServer)
		{
			ServerUpdateOutOfBounds();
		}
		void ServerUpdateOutOfBounds()
		{
			if (!SingletonBehaviour<DrivingRangeManager>.HasInstance && !IsHidden && AsEntity.LevelBoundsTracker.AuthoritativeBoundsState.HasFlag(BoundsState.OutOfBounds))
			{
				float timeSince = BMath.GetTimeSince(AsEntity.LevelBoundsTracker.OutOfBoundsTimestamp);
				NetworkoutOfBoundsRemainingTime = MatchSetupRules.GetValue(MatchSetupRules.Rule.OutOfBounds) - timeSince;
				if (ShouldDisappearOutOfBounds())
				{
					ServerPlayOutOfBoundsEliminationExplosionForAllClients();
					ServerDisappearOutOfBounds(EliminationReason.OutOfBounds);
				}
			}
		}
		bool ShouldDisappearOutOfBounds()
		{
			if (outOfBoundsRemainingTime > 0f)
			{
				return false;
			}
			if (Movement.IsAnyWheelGrounded())
			{
				return true;
			}
			if (Physics.CheckBox(base.transform.TransformPoint(itemCollectorCollider.center), itemCollectorCollider.size / 2f, base.transform.rotation, GameManager.LayerSettings.GolfCartGroundMask, QueryTriggerInteraction.Ignore))
			{
				return true;
			}
			return false;
		}
	}

	public void LocalPlayerInteract()
	{
		if (!TryGetFirstAvailableSeatFor(GameManager.LocalPlayerInfo, out var _))
		{
			Debug.LogError("Local player tried to enter a golf cart with no available seats", base.gameObject);
		}
		else if (CanLocalPlayerEnter())
		{
			GameManager.LocalPlayerInfo.InformAttemptingToEnterGolfCart(this);
			CmdEnter();
		}
		static bool CanLocalPlayerEnter()
		{
			if (GameManager.LocalPlayerInfo == null)
			{
				return false;
			}
			return GameManager.LocalPlayerInfo.CanEnterGolfCart();
		}
	}

	[Command]
	public void CmdEnterReservedGolfCart(NetworkConnectionToClient sender = null)
	{
		if (base.isServer && base.isClient)
		{
			UserCode_CmdEnterReservedGolfCart__NetworkConnectionToClient(sender);
			return;
		}
		NetworkWriterPooled writer = NetworkWriterPool.Get();
		SendCommandInternal("System.Void GolfCartInfo::CmdEnterReservedGolfCart(Mirror.NetworkConnectionToClient)", -973844186, writer, 0);
		NetworkWriterPool.Return(writer);
	}

	public bool TryGetFirstAvailableSeatFor(PlayerInfo player, out int seat)
	{
		for (int i = 0; i < GameManager.GolfCartSettings.MaxPassengers; i++)
		{
			if ((i != 0 || !(NetworkdriverSeatReserver != null) || !(NetworkdriverSeatReserver != player)) && passengers[i] == null)
			{
				seat = i;
				return true;
			}
		}
		seat = -1;
		return false;
	}

	public bool IsSeatFreeFor(PlayerInfo player, int seat)
	{
		if (seat < 0 || seat >= passengers.Count)
		{
			return false;
		}
		if (seat == 0 && NetworkdriverSeatReserver != null && NetworkdriverSeatReserver != player)
		{
			return false;
		}
		return passengers[seat] == null;
	}

	public bool TryGetDriver(out PlayerInfo driver)
	{
		if (passengers.Count <= 0)
		{
			driver = null;
			return false;
		}
		driver = passengers[0];
		return driver != null;
	}

	public bool TryGetPassengerFromCollider(Collider collider, out PlayerInfo passenger)
	{
		passenger = null;
		if (collider.gameObject.layer != GameManager.LayerSettings.GolfCartPassengersLayer)
		{
			return false;
		}
		for (int i = 0; i < passengerColliders.Length; i++)
		{
			if (passengerColliders[i] == collider)
			{
				passenger = passengers[i];
				return passenger != null;
			}
		}
		return false;
	}

	public int GetPassengerCount()
	{
		return passengerIndices.Count;
	}

	public void GetExitData(int seat, GolfCartExitType exitType, bool backup, out Vector3 worldPosition, out float worldYaw)
	{
		GolfCartSeatSettings golfCartSeatSettings = GameManager.GolfCartSettings.SeatSettings[seat];
		Vector3 position = ((exitType != GolfCartExitType.Knockout) ? (backup ? golfCartSeatSettings.backupExitPosition : golfCartSeatSettings.exitPosition) : (backup ? golfCartSeatSettings.backupKnockoutPosition : golfCartSeatSettings.knockoutPosition));
		worldPosition = base.transform.TransformPoint(position);
		float yawDeg = base.transform.forward.GetYawDeg();
		worldYaw = (yawDeg + ((golfCartSeatSettings.exitPosition.x < 0f) ? (-90f) : 90f)).WrapAngleDeg();
	}

	public Bounds GetOrbitCameraSubjectLocalBounds()
	{
		return base.transform.GetLocalCompoundBounds();
	}

	public void InformDriverStatusEffectsChanged()
	{
		UpdateSpeedBoostVfx();
	}

	public void UpdateEngineSoundIntensity(float targetIntensity, float smoothingSpeed)
	{
		if (engineSoundInstance.isValid())
		{
			engineSoundIntensity = BMath.LerpClamped(engineSoundIntensity, targetIntensity, smoothingSpeed * Time.deltaTime);
			engineSoundInstance.setParameterByID(AudioSettings.IntensityId, engineSoundIntensity);
		}
	}

	[Server]
	public void ServerReserveDriverSeatPreNetworkSpawn(PlayerInfo player)
	{
		if (!NetworkServer.active)
		{
			Debug.LogWarning("[Server] function 'System.Void GolfCartInfo::ServerReserveDriverSeatPreNetworkSpawn(PlayerInfo)' called when server was not active");
			return;
		}
		NetworkdriverSeatReserver = player;
		player.AsEntity.WillBeDestroyed += OnServerDriverSeatReserverWillBeDestroyed;
		DestroyIfUnusedDelayed();
		async void DestroyIfUnusedDelayed()
		{
			for (float time = 0f; time < 5f; time += Time.deltaTime)
			{
				await UniTask.Yield();
				if (this == null || NetworkdriverSeatReserver == null)
				{
					return;
				}
			}
			AsEntity.DestroyEntity();
		}
	}

	[Server]
	public void ServerReserveDriverSeatPostNetworkSpawn()
	{
		if (!NetworkServer.active)
		{
			Debug.LogWarning("[Server] function 'System.Void GolfCartInfo::ServerReserveDriverSeatPostNetworkSpawn()' called when server was not active");
		}
		else if (!(NetworkdriverSeatReserver == null))
		{
			if (NetworkdriverSeatReserver.isLocalPlayer)
			{
				ServerSetMovementSyncDirectionForAllClients(SyncDirection.ServerToClient);
				base.netIdentity.RemoveClientAuthority();
			}
			else
			{
				ServerSetMovementSyncDirectionForAllClients(SyncDirection.ClientToServer);
				base.netIdentity.AssignClientAuthority(NetworkdriverSeatReserver.netIdentity.connectionToClient);
			}
		}
	}

	[Server]
	private void ServerSetMovementSyncDirectionForAllClients(SyncDirection syncDirection)
	{
		if (!NetworkServer.active)
		{
			Debug.LogWarning("[Server] function 'System.Void GolfCartInfo::ServerSetMovementSyncDirectionForAllClients(Mirror.SyncDirection)' called when server was not active");
			return;
		}
		SetMovementSyncDirectionInternal(syncDirection);
		foreach (NetworkConnectionToClient value in NetworkServer.connections.Values)
		{
			if (value != NetworkServer.localConnection)
			{
				RpcSetMovementSyncDirection(value, syncDirection);
			}
		}
	}

	[TargetRpc]
	private void RpcSetMovementSyncDirection(NetworkConnectionToClient connection, SyncDirection syncDirection)
	{
		NetworkWriterPooled writer = NetworkWriterPool.Get();
		GeneratedNetworkCode._Write_Mirror_002ESyncDirection(writer, syncDirection);
		SendTargetRPCInternal(connection, "System.Void GolfCartInfo::RpcSetMovementSyncDirection(Mirror.NetworkConnectionToClient,Mirror.SyncDirection)", -34918881, writer, 0);
		NetworkWriterPool.Return(writer);
	}

	private void SetMovementSyncDirectionInternal(SyncDirection syncDirection)
	{
		AsEntity.PredictedGolfCart.syncDirection = syncDirection;
		Movement.syncDirection = syncDirection;
	}

	[Command(requiresAuthority = false)]
	private void CmdEnter(NetworkConnectionToClient sender = null)
	{
		if (base.isServer && base.isClient)
		{
			UserCode_CmdEnter__NetworkConnectionToClient(sender);
			return;
		}
		NetworkWriterPooled writer = NetworkWriterPool.Get();
		SendCommandInternal("System.Void GolfCartInfo::CmdEnter(Mirror.NetworkConnectionToClient)", -1684530868, writer, 0, requiresAuthority: false);
		NetworkWriterPool.Return(writer);
	}

	[Server]
	private void ServerEnter(PlayerInfo newPassenger)
	{
		if (!NetworkServer.active)
		{
			Debug.LogWarning("[Server] function 'System.Void GolfCartInfo::ServerEnter(PlayerInfo)' called when server was not active");
		}
		else if (!(newPassenger == null))
		{
			if (!TryGetFirstAvailableSeatFor(newPassenger, out var seat))
			{
				Debug.LogError(newPassenger.name + " tried to enter a golf cart with no available seats", base.gameObject);
				return;
			}
			bool fromReservation = newPassenger == NetworkdriverSeatReserver;
			ServerTryAssignPassengerToSeat(newPassenger, seat, fromReservation);
		}
	}

	[Command]
	private void CmdCancelDriverSeatReservation(NetworkConnectionToClient sender = null)
	{
		if (base.isServer && base.isClient)
		{
			UserCode_CmdCancelDriverSeatReservation__NetworkConnectionToClient(sender);
			return;
		}
		NetworkWriterPooled writer = NetworkWriterPool.Get();
		SendCommandInternal("System.Void GolfCartInfo::CmdCancelDriverSeatReservation(Mirror.NetworkConnectionToClient)", -148631917, writer, 0);
		NetworkWriterPool.Return(writer);
	}

	[Command(requiresAuthority = false)]
	public void CmdTryChangeLocalPlayerSeat(int seat, NetworkConnectionToClient sender = null)
	{
		if (base.isServer && base.isClient)
		{
			UserCode_CmdTryChangeLocalPlayerSeat__Int32__NetworkConnectionToClient(seat, sender);
			return;
		}
		NetworkWriterPooled writer = NetworkWriterPool.Get();
		writer.WriteVarInt(seat);
		SendCommandInternal("System.Void GolfCartInfo::CmdTryChangeLocalPlayerSeat(System.Int32,Mirror.NetworkConnectionToClient)", 801904613, writer, 0, requiresAuthority: false);
		NetworkWriterPool.Return(writer);
	}

	[TargetRpc]
	private void RpcInformPassengerSeatChangeFailed(NetworkConnectionToClient connection)
	{
		NetworkWriterPooled writer = NetworkWriterPool.Get();
		SendTargetRPCInternal(connection, "System.Void GolfCartInfo::RpcInformPassengerSeatChangeFailed(Mirror.NetworkConnectionToClient)", 1667594244, writer, 0);
		NetworkWriterPool.Return(writer);
	}

	[Server]
	private bool ServerTryAssignPassengerToSeat(PlayerInfo passenger, int seat, bool fromReservation)
	{
		if (!NetworkServer.active)
		{
			Debug.LogWarning("[Server] function 'System.Boolean GolfCartInfo::ServerTryAssignPassengerToSeat(PlayerInfo,System.Int32,System.Boolean)' called when server was not active");
			return default(bool);
		}
		if (passengers[seat] != null)
		{
			Debug.LogError($"{passenger.name} tried to take seat {seat} in a golf cart, but it's not available", base.gameObject);
			return false;
		}
		if (passengerIndices.TryGetValue(passenger, out var value))
		{
			passengers[value] = null;
			OnServerPassengerLeftSeat(passenger, value);
		}
		passengers[seat] = passenger;
		bool flag = passenger.connectionToClient == NetworkServer.localConnection;
		if (seat == 0)
		{
			if (flag)
			{
				ServerSetMovementSyncDirectionForAllClients(SyncDirection.ServerToClient);
				base.netIdentity.RemoveClientAuthority();
			}
			else
			{
				ServerSetMovementSyncDirectionForAllClients(SyncDirection.ClientToServer);
				base.netIdentity.AssignClientAuthority(passenger.netIdentity.connectionToClient);
			}
		}
		if (flag)
		{
			LocalPlayerSetSeat(seat, fromReservation);
		}
		else
		{
			RpcLocalPlayerSetSeat(passenger.connectionToClient, seat, fromReservation);
		}
		return true;
	}

	[TargetRpc]
	private void RpcLocalPlayerSetSeat(NetworkConnectionToClient connection, int seat, bool fromReservation)
	{
		NetworkWriterPooled writer = NetworkWriterPool.Get();
		writer.WriteVarInt(seat);
		writer.WriteBool(fromReservation);
		SendTargetRPCInternal(connection, "System.Void GolfCartInfo::RpcLocalPlayerSetSeat(Mirror.NetworkConnectionToClient,System.Int32,System.Boolean)", -1487607196, writer, 0);
		NetworkWriterPool.Return(writer);
	}

	private void LocalPlayerSetSeat(int seat, bool fromReservation)
	{
		if (!GameManager.LocalPlayerInfo.TryInformEnteredGolfCartSeat(this, seat, fromReservation))
		{
			CmdExit();
		}
		else if (seat == 0 && fromReservation)
		{
			Vector3 position = GameManager.LocalPlayerInfo.transform.position;
			if (GameManager.LocalPlayerMovement.IsGrounded)
			{
				position += GameManager.LocalPlayerMovement.GroundData.normal * 0.2f;
			}
			Quaternion rotation = GetLocalPlayerEffectiveRotation();
			Vector3 linearVelocity = GameManager.LocalPlayerInfo.Rigidbody.linearVelocity;
			ApplyDrivingSeatReservationReceivedData(position, rotation, linearVelocity);
			CmdInformDrivingSeatReservationReceived(position, rotation, linearVelocity);
		}
		static Quaternion GetLocalPlayerEffectiveRotation()
		{
			if (!GameManager.LocalPlayerMovement.IsGrounded)
			{
				return Quaternion.Euler(0f, GameManager.LocalPlayerInfo.transform.eulerAngles.y, 0f);
			}
			Vector3 forward = GameManager.LocalPlayerMovement.transform.forward;
			Vector3 normal = GameManager.LocalPlayerMovement.GroundData.normal;
			if (!forward.TryProjectOnPlaneAlong(Vector3.up, normal, out var projection))
			{
				return Quaternion.Euler(0f, GameManager.LocalPlayerInfo.transform.eulerAngles.y, 0f);
			}
			return Quaternion.LookRotation(projection, normal);
		}
	}

	[Command]
	private void CmdInformDrivingSeatReservationReceived(Vector3 position, Quaternion rotation, Vector3 velocity, NetworkConnectionToClient sender = null)
	{
		if (base.isServer && base.isClient)
		{
			UserCode_CmdInformDrivingSeatReservationReceived__Vector3__Quaternion__Vector3__NetworkConnectionToClient(position, rotation, velocity, sender);
			return;
		}
		NetworkWriterPooled writer = NetworkWriterPool.Get();
		writer.WriteVector3(position);
		writer.WriteQuaternion(rotation);
		writer.WriteVector3(velocity);
		SendCommandInternal("System.Void GolfCartInfo::CmdInformDrivingSeatReservationReceived(UnityEngine.Vector3,UnityEngine.Quaternion,UnityEngine.Vector3,Mirror.NetworkConnectionToClient)", -353765067, writer, 0);
		NetworkWriterPool.Return(writer);
	}

	[TargetRpc]
	private void RpcInformDrivingSeatReservationReceived(NetworkConnectionToClient connection, Vector3 position, Quaternion rotation, Vector3 velocity)
	{
		NetworkWriterPooled writer = NetworkWriterPool.Get();
		writer.WriteVector3(position);
		writer.WriteQuaternion(rotation);
		writer.WriteVector3(velocity);
		SendTargetRPCInternal(connection, "System.Void GolfCartInfo::RpcInformDrivingSeatReservationReceived(Mirror.NetworkConnectionToClient,UnityEngine.Vector3,UnityEngine.Quaternion,UnityEngine.Vector3)", -370188272, writer, 0);
		NetworkWriterPool.Return(writer);
	}

	private void ApplyDrivingSeatReservationReceivedData(Vector3 position, Quaternion rotation, Vector3 velocity)
	{
		UpdateIsHidden(forceUnhide: true);
		base.transform.SetPositionAndRotation(position, rotation);
		AsEntity.Rigidbody.position = position;
		AsEntity.Rigidbody.rotation = rotation;
		AsEntity.Rigidbody.linearVelocity = velocity;
		if (VfxPersistentData.TryGetPooledVfx(VfxType.GolfCartSpawn, out var particleSystem))
		{
			particleSystem.transform.SetParent(base.transform);
			particleSystem.transform.SetLocalPositionAndRotation(Vector3.zero, Quaternion.identity);
			particleSystem.Play();
		}
	}

	[Command(requiresAuthority = false)]
	public void CmdExit(NetworkConnectionToClient sender = null)
	{
		if (base.isServer && base.isClient)
		{
			UserCode_CmdExit__NetworkConnectionToClient(sender);
			return;
		}
		NetworkWriterPooled writer = NetworkWriterPool.Get();
		SendCommandInternal("System.Void GolfCartInfo::CmdExit(Mirror.NetworkConnectionToClient)", 625734170, writer, 0, requiresAuthority: false);
		NetworkWriterPool.Return(writer);
	}

	[Server]
	private void OnServerPassengerLeftSeat(PlayerInfo passenger, int seat)
	{
		if (!NetworkServer.active)
		{
			Debug.LogWarning("[Server] function 'System.Void GolfCartInfo::OnServerPassengerLeftSeat(PlayerInfo,System.Int32)' called when server was not active");
		}
		else if (seat == 0)
		{
			_ = passenger.connectionToClient;
			_ = NetworkServer.localConnection;
			ServerSetMovementSyncDirectionForAllClients(SyncDirection.ServerToClient);
			base.netIdentity.RemoveClientAuthority();
		}
	}

	private void UpdateIsInteractionEnabled()
	{
		IsInteractionEnabled = ShouldBeEnabled();
		bool ShouldBeEnabled()
		{
			if (AsEntity.AsHittable.IsFrozen)
			{
				return false;
			}
			for (int i = 0; i < GameManager.GolfCartSettings.MaxPassengers; i++)
			{
				if (passengers[i] == null)
				{
					return true;
				}
			}
			return false;
		}
	}

	private void UpdateSpeedBoostVfx()
	{
		vfx.SetSpeedUpVfxPlaying(ShouldPlayVfx());
		bool ShouldPlayVfx()
		{
			if (!TryGetDriver(out var driver))
			{
				return false;
			}
			return driver.Movement.StatusEffects.HasEffect(StatusEffect.SpeedBoost);
		}
	}

	private void CancelResponsiblePlayerTimeout()
	{
		if (responsiblePlayerTimeoutRoutine != null)
		{
			StopCoroutine(responsiblePlayerTimeoutRoutine);
		}
	}

	[Server]
	private void ServerSetResponsiblePlayer(PlayerInfo player)
	{
		if (!NetworkServer.active)
		{
			Debug.LogWarning("[Server] function 'System.Void GolfCartInfo::ServerSetResponsiblePlayer(PlayerInfo)' called when server was not active");
			return;
		}
		CancelResponsiblePlayerTimeout();
		if (player != null)
		{
			NetworkresponsiblePlayer = player;
		}
		else
		{
			responsiblePlayerTimeoutRoutine = StartCoroutine(TimeOutResponsiblePlayerRoutine());
		}
	}

	[Server]
	private void ServerStartResponsiblePlayerTimeout()
	{
		if (!NetworkServer.active)
		{
			Debug.LogWarning("[Server] function 'System.Void GolfCartInfo::ServerStartResponsiblePlayerTimeout()' called when server was not active");
			return;
		}
		CancelResponsiblePlayerTimeout();
		responsiblePlayerTimeoutRoutine = StartCoroutine(TimeOutResponsiblePlayerRoutine());
	}

	private IEnumerator TimeOutResponsiblePlayerRoutine()
	{
		yield return new WaitForSeconds(GameManager.GolfCartSettings.ResponsiblePlayerTimeout);
		NetworkresponsiblePlayer = null;
	}

	[Server]
	private void ServerPlayOutOfBoundsEliminationExplosionForAllClients()
	{
		if (!NetworkServer.active)
		{
			Debug.LogWarning("[Server] function 'System.Void GolfCartInfo::ServerPlayOutOfBoundsEliminationExplosionForAllClients()' called when server was not active");
			return;
		}
		PlayOutOfBoundsEliminationExplosionInternal();
		foreach (NetworkConnectionToClient value in NetworkServer.connections.Values)
		{
			if (value != NetworkServer.localConnection)
			{
				RpcPlayOutOfBoundsEliminationExplosion(value);
			}
		}
	}

	[TargetRpc]
	private void RpcPlayOutOfBoundsEliminationExplosion(NetworkConnectionToClient connection)
	{
		NetworkWriterPooled writer = NetworkWriterPool.Get();
		SendTargetRPCInternal(connection, "System.Void GolfCartInfo::RpcPlayOutOfBoundsEliminationExplosion(Mirror.NetworkConnectionToClient)", 1267072245, writer, 0);
		NetworkWriterPool.Return(writer);
	}

	private void PlayOutOfBoundsEliminationExplosionInternal()
	{
		VfxManager.PlayPooledVfxLocalOnly(VfxType.MineExplosion, base.transform.position, Quaternion.identity);
		if (GameplayCameraManager.ShouldPlayImpactFrameForExplosion(base.transform.position, GameManager.ItemSettings.LandmineExplosionRange, GameManager.CameraGameplaySettings.LandmineImpactFrameDistanceSquared))
		{
			CameraModuleController.PlayImpactFrame(base.transform.position);
		}
		CameraModuleController.Shake(GameManager.CameraGameplaySettings.LandmineExplosionScreenshakeSettings, base.transform.position);
		RuntimeManager.PlayOneShot(GameManager.AudioSettings.LandmineExplosionEvent, base.transform.position);
	}

	private void UpdateIsHidden(bool forceUnhide)
	{
		bool isHidden = IsHidden;
		IsHidden = !forceUnhide && NetworkdriverSeatReserver != null && (!TryGetDriver(out var driver) || driver != NetworkdriverSeatReserver);
		if (IsHidden == isHidden)
		{
			return;
		}
		foreach (Transform item in base.transform)
		{
			item.gameObject.SetActive(!IsHidden);
		}
		AsEntity.Rigidbody.isKinematic = IsHidden;
	}

	[Server]
	private void ServerDisappearOutOfBounds(EliminationReason eliminationReason)
	{
		if (!NetworkServer.active)
		{
			Debug.LogWarning("[Server] function 'System.Void GolfCartInfo::ServerDisappearOutOfBounds(EliminationReason)' called when server was not active");
			return;
		}
		int num = 0;
		PlayerInfo playerInfo = null;
		for (int i = 0; i < passengers.Count; i++)
		{
			PlayerInfo playerInfo2 = passengers[i];
			if (!(playerInfo2 == null))
			{
				num++;
				if (i == 0)
				{
					playerInfo = playerInfo2;
				}
				if (BMath.GetTimeSince(playerInfo2.AsGolfer.ServerOutOfBoundsTimerEliminationTimestamp) >= 2f)
				{
					playerInfo2.AsGolfer.ServerEliminate(eliminationReason);
				}
			}
		}
		if (playerInfo != null && num > 1)
		{
			playerInfo.RpcInformDroveGolfCartOutOfBoundsWithOtherPassengers(eliminationReason);
		}
		PlayVfx();
		AsEntity.DestroyEntity();
		void PlayVfx()
		{
			VfxType vfxType = eliminationReason switch
			{
				EliminationReason.FellIntoWater => VfxType.WaterGolfCartOutOfBounds, 
				EliminationReason.FellIntoFog => VfxType.FogGolfCartOutOfBounds, 
				_ => VfxType.None, 
			};
			if (vfxType != VfxType.None)
			{
				Vector3 worldCenterOfMass = AsEntity.Rigidbody.worldCenterOfMass;
				worldCenterOfMass.y = AsEntity.LevelBoundsTracker.CurrentOutOfBoundsHazardWorldHeightLocalOnly;
				if (vfxType == VfxType.WaterGolfCartOutOfBounds)
				{
					ServerPlayWaterSplashForAllClients(worldCenterOfMass);
				}
				VfxManager.ServerPlayPooledVfxForAllClients(vfxType, worldCenterOfMass, Quaternion.identity);
			}
		}
	}

	public void PlayHonkForAllClients()
	{
		PlayHonkInternal();
		CmdPlayHonkForAllClients();
	}

	[Command(requiresAuthority = false)]
	private void CmdPlayHonkForAllClients(NetworkConnectionToClient sender = null)
	{
		if (base.isServer && base.isClient)
		{
			UserCode_CmdPlayHonkForAllClients__NetworkConnectionToClient(sender);
			return;
		}
		NetworkWriterPooled writer = NetworkWriterPool.Get();
		SendCommandInternal("System.Void GolfCartInfo::CmdPlayHonkForAllClients(Mirror.NetworkConnectionToClient)", 697496700, writer, 0, requiresAuthority: false);
		NetworkWriterPool.Return(writer);
	}

	[TargetRpc]
	private void RpcPlayHonk(NetworkConnectionToClient connection)
	{
		NetworkWriterPooled writer = NetworkWriterPool.Get();
		SendTargetRPCInternal(connection, "System.Void GolfCartInfo::RpcPlayHonk(Mirror.NetworkConnectionToClient)", 1519377473, writer, 0);
		NetworkWriterPool.Return(writer);
	}

	private void PlayHonkInternal()
	{
		if (BMath.GetTimeSince(honkVfxTimestamp) > GameManager.GolfCartSettings.HonkVfxCooldown)
		{
			VfxManager.PlayPooledVfxLocalOnly(VfxType.GolfCartHornShort, Vector3.zero, Quaternion.identity, base.transform, default(Vector3), localSpace: true);
			honkVfxTimestamp = Time.timeAsDouble;
		}
		if (honkSoundInstance.isValid())
		{
			honkSoundInstance.stop(FMOD.Studio.STOP_MODE.ALLOWFADEOUT);
		}
		honkSoundInstance = RuntimeManager.CreateInstance(GameManager.AudioSettings.GolfCartHonkEvent);
		RuntimeManager.AttachInstanceToGameObject(honkSoundInstance, enginePosition);
		honkSoundInstance.start();
		honkSoundInstance.release();
	}

	public void StartPlayingSpecialHonkForAllClients()
	{
		StartPlayingSpecialHonkInternal();
		CmdStartPlayingSpecialHonkForAllClients();
	}

	[Command(requiresAuthority = false)]
	private void CmdStartPlayingSpecialHonkForAllClients(NetworkConnectionToClient sender = null)
	{
		if (base.isServer && base.isClient)
		{
			UserCode_CmdStartPlayingSpecialHonkForAllClients__NetworkConnectionToClient(sender);
			return;
		}
		NetworkWriterPooled writer = NetworkWriterPool.Get();
		SendCommandInternal("System.Void GolfCartInfo::CmdStartPlayingSpecialHonkForAllClients(Mirror.NetworkConnectionToClient)", 458256569, writer, 0, requiresAuthority: false);
		NetworkWriterPool.Return(writer);
	}

	[TargetRpc]
	private void RpcStartPlayingSpecialHonk(NetworkConnectionToClient connection)
	{
		NetworkWriterPooled writer = NetworkWriterPool.Get();
		SendTargetRPCInternal(connection, "System.Void GolfCartInfo::RpcStartPlayingSpecialHonk(Mirror.NetworkConnectionToClient)", 1953879816, writer, 0);
		NetworkWriterPool.Return(writer);
	}

	private void StartPlayingSpecialHonkInternal()
	{
		if (specialHonkSoundInstance.isValid())
		{
			specialHonkSoundInstance.stop(FMOD.Studio.STOP_MODE.ALLOWFADEOUT);
		}
		specialHonkSoundInstance = RuntimeManager.CreateInstance(GameManager.AudioSettings.GolfCartSpecialHonkEvent);
		RuntimeManager.AttachInstanceToGameObject(specialHonkSoundInstance, enginePosition);
		specialHonkSoundInstance.start();
		specialHonkSoundInstance.release();
	}

	public void EndSpecialHonkForAllClients()
	{
		EndSpecialHonkInternal();
		CmdEndSpecialHonkForAllClients();
	}

	[Command(requiresAuthority = false)]
	private void CmdEndSpecialHonkForAllClients(NetworkConnectionToClient sender = null)
	{
		if (base.isServer && base.isClient)
		{
			UserCode_CmdEndSpecialHonkForAllClients__NetworkConnectionToClient(sender);
			return;
		}
		NetworkWriterPooled writer = NetworkWriterPool.Get();
		SendCommandInternal("System.Void GolfCartInfo::CmdEndSpecialHonkForAllClients(Mirror.NetworkConnectionToClient)", -317824298, writer, 0, requiresAuthority: false);
		NetworkWriterPool.Return(writer);
	}

	[TargetRpc]
	private void RpcEndSpecialHonk(NetworkConnectionToClient connection)
	{
		NetworkWriterPooled writer = NetworkWriterPool.Get();
		SendTargetRPCInternal(connection, "System.Void GolfCartInfo::RpcEndSpecialHonk(Mirror.NetworkConnectionToClient)", 659885243, writer, 0);
		NetworkWriterPool.Return(writer);
	}

	private void EndSpecialHonkInternal()
	{
		if (specialHonkSoundInstance.isValid())
		{
			specialHonkSoundInstance.stop(FMOD.Studio.STOP_MODE.ALLOWFADEOUT);
		}
	}

	[Server]
	public void ServerPlayWaterSplashForAllClients(Vector3 worldPosition)
	{
		if (!NetworkServer.active)
		{
			Debug.LogWarning("[Server] function 'System.Void GolfCartInfo::ServerPlayWaterSplashForAllClients(UnityEngine.Vector3)' called when server was not active");
			return;
		}
		PlayWaterSplashInternal(worldPosition);
		foreach (NetworkConnectionToClient value in NetworkServer.connections.Values)
		{
			if (value != NetworkServer.localConnection)
			{
				RpcPlayWaterSplash(value, worldPosition);
			}
		}
	}

	[TargetRpc]
	private void RpcPlayWaterSplash(NetworkConnectionToClient connection, Vector3 worldPosition)
	{
		NetworkWriterPooled writer = NetworkWriterPool.Get();
		writer.WriteVector3(worldPosition);
		SendTargetRPCInternal(connection, "System.Void GolfCartInfo::RpcPlayWaterSplash(Mirror.NetworkConnectionToClient,UnityEngine.Vector3)", 1086696474, writer, 0);
		NetworkWriterPool.Return(writer);
	}

	private void PlayWaterSplashInternal(Vector3 worldPosition)
	{
		VfxManager.PlayPooledVfxLocalOnly(VfxType.WaterImpactLarge, worldPosition, Quaternion.identity);
		RuntimeManager.PlayOneShot(GameManager.AudioSettings.GolfCartWaterSplashEvent, worldPosition);
	}

	private void OnPassengersChanged(SyncList<PlayerInfo>.Operation operation, int index, PlayerInfo changedPassenger)
	{
		if ((uint)operation != 1u)
		{
			this.PassengersChanged?.Invoke();
			return;
		}
		UpdateIsInteractionEnabled();
		UpdateSpeedBoostVfx();
		bool flag = index == 0;
		if (flag)
		{
			Movement.InformDriverChanged();
		}
		if (changedPassenger != null)
		{
			passengerIndices.Remove(changedPassenger);
			passengerColliders[index].gameObject.SetActive(value: false);
			if (base.isServer)
			{
				changedPassenger.AsEntity.WillBeDestroyedReferenced -= OnServerPassengerWillBeDestroyed;
			}
		}
		PlayerInfo playerInfo = passengers[index];
		if (playerInfo != null)
		{
			passengerIndices.Add(playerInfo, index);
			passengerColliders[index].gameObject.SetActive(value: true);
			if (base.isServer)
			{
				playerInfo.AsEntity.WillBeDestroyedReferenced += OnServerPassengerWillBeDestroyed;
				if (flag)
				{
					ServerSetResponsiblePlayer(playerInfo);
				}
			}
		}
		else if (base.isServer)
		{
			ServerStartResponsiblePlayerTimeout();
		}
		if (flag)
		{
			if (turnOnHeadlightsRoutine != null)
			{
				StopCoroutine(turnOnHeadlightsRoutine);
			}
			if (changedPassenger != null && playerInfo == null)
			{
				vfx.SetHeadlightsOn(active: false);
				if (engineSoundInstance.isValid())
				{
					engineSoundInstance.stop(FMOD.Studio.STOP_MODE.ALLOWFADEOUT);
				}
				if (honkSoundInstance.isValid())
				{
					honkSoundInstance.stop(FMOD.Studio.STOP_MODE.ALLOWFADEOUT);
				}
				if (specialHonkSoundInstance.isValid())
				{
					specialHonkSoundInstance.stop(FMOD.Studio.STOP_MODE.ALLOWFADEOUT);
				}
				RuntimeManager.PlayOneShotAttached(GameManager.AudioSettings.GolfCartTurnEngineOff, enginePosition);
			}
			else if (changedPassenger == null && playerInfo != null)
			{
				turnOnHeadlightsRoutine = StartCoroutine(TurnHeadlightsOnRoutine());
				engineSoundInstance = RuntimeManager.CreateInstance(GameManager.AudioSettings.GolfCartEngine);
				RuntimeManager.AttachInstanceToGameObject(engineSoundInstance, enginePosition);
				engineSoundInstance.start();
				engineSoundInstance.release();
			}
		}
		Movement.InformPassengersChanged();
		this.PassengersChanged?.Invoke();
		IEnumerator TurnHeadlightsOnRoutine()
		{
			yield return new WaitForSeconds(0.5f);
			vfx.SetHeadlightsOn(active: true);
		}
	}

	private void OnIsFrozenChanged()
	{
		UpdateIsInteractionEnabled();
		vfx.SetIsFrozen(AsEntity.AsHittable.IsFrozen);
	}

	private void OnServerBoundsStateChanged(BoundsState previousState, BoundsState currentState)
	{
		if (!SingletonBehaviour<DrivingRangeManager>.HasInstance && currentState.IsInOutOfBoundsHazard())
		{
			ServerDisappearOutOfBounds(AsEntity.LevelBoundsTracker.GetPotentialOutOfBoundsHazardEliminationReason());
		}
	}

	private void OnClientLocalPlayerIsElectromagnetShieldActiveChanged()
	{
		UpdateIsInteractionEnabled();
	}

	private void OnServerPassengerWillBeDestroyed(Entity passengerEntity)
	{
		if (passengerEntity.IsPlayer)
		{
			PlayerInfo playerInfo = passengerEntity.PlayerInfo;
			if (passengerIndices.TryGetValue(playerInfo, out var value))
			{
				OnServerPassengerLeftSeat(playerInfo, value);
				passengers[value] = null;
			}
		}
	}

	private void OnServerDriverSeatReserverWillBeDestroyed()
	{
		AsEntity.DestroyEntity();
	}

	private void OnLocalPlayerDriverSearReserverItemUseCanceled(ItemType item)
	{
		if (item == ItemType.GolfCart)
		{
			CmdCancelDriverSeatReservation();
		}
	}

	private void OnDriverSeatReserverChanged(PlayerInfo previousReserver, PlayerInfo currentReserver)
	{
		UpdateIsHidden(forceUnhide: false);
		if (previousReserver == GameManager.LocalPlayerInfo)
		{
			GameManager.LocalPlayerInventory.ItemUseCancelled -= OnLocalPlayerDriverSearReserverItemUseCanceled;
		}
		if (currentReserver == GameManager.LocalPlayerInfo)
		{
			GameManager.LocalPlayerInventory.ItemUseCancelled += OnLocalPlayerDriverSearReserverItemUseCanceled;
		}
		this.DriverSeatReserverChanged?.Invoke();
	}

	public GolfCartInfo()
	{
		InitSyncObject(passengers);
		_Mirror_SyncVarHookDelegate_driverSeatReserver = OnDriverSeatReserverChanged;
	}

	public override bool Weaved()
	{
		return true;
	}

	protected void UserCode_CmdEnterReservedGolfCart__NetworkConnectionToClient(NetworkConnectionToClient sender)
	{
		if (!serverEnterReservedGolfCartCommandRateLimiter.RegisterHit(sender))
		{
			return;
		}
		PlayerInfo senderAsPlayer;
		if (sender == null)
		{
			senderAsPlayer = GameManager.LocalPlayerInfo;
		}
		else
		{
			GameManager.RemotePlayerPerConnectionId.TryGetValue(sender.connectionId, out senderAsPlayer);
		}
		if (!(senderAsPlayer == null) && !(senderAsPlayer != NetworkdriverSeatReserver))
		{
			if (!CanSenderEnter())
			{
				CmdCancelDriverSeatReservation();
			}
			else
			{
				ServerEnter(senderAsPlayer);
			}
		}
		bool CanSenderEnter()
		{
			if (AsEntity.AsHittable.IsFrozen)
			{
				return false;
			}
			if (senderAsPlayer.Movement.IsKnockedOutOrRecovering)
			{
				return false;
			}
			if (senderAsPlayer.AsHittable.IsFrozen)
			{
				return false;
			}
			return true;
		}
	}

	protected static void InvokeUserCode_CmdEnterReservedGolfCart__NetworkConnectionToClient(NetworkBehaviour obj, NetworkReader reader, NetworkConnectionToClient senderConnection)
	{
		if (!NetworkServer.active)
		{
			Debug.LogError("Command CmdEnterReservedGolfCart called on client.");
		}
		else
		{
			((GolfCartInfo)obj).UserCode_CmdEnterReservedGolfCart__NetworkConnectionToClient(senderConnection);
		}
	}

	protected void UserCode_RpcSetMovementSyncDirection__NetworkConnectionToClient__SyncDirection(NetworkConnectionToClient connection, SyncDirection syncDirection)
	{
		SetMovementSyncDirectionInternal(syncDirection);
	}

	protected static void InvokeUserCode_RpcSetMovementSyncDirection__NetworkConnectionToClient__SyncDirection(NetworkBehaviour obj, NetworkReader reader, NetworkConnectionToClient senderConnection)
	{
		if (!NetworkClient.active)
		{
			Debug.LogError("TargetRPC RpcSetMovementSyncDirection called on server.");
		}
		else
		{
			((GolfCartInfo)obj).UserCode_RpcSetMovementSyncDirection__NetworkConnectionToClient__SyncDirection(null, GeneratedNetworkCode._Read_Mirror_002ESyncDirection(reader));
		}
	}

	protected void UserCode_CmdEnter__NetworkConnectionToClient(NetworkConnectionToClient sender)
	{
		if (serverEnterCommandRateLimiter.RegisterHit(sender))
		{
			PlayerInfo newPassenger;
			bool succeeded = CanEnter(out newPassenger);
			if (newPassenger != null)
			{
				newPassenger.RpcInformOfGolfCartEnterAttemptResult(this, succeeded);
			}
			ServerEnter(newPassenger);
		}
		bool CanEnter(out PlayerInfo reference)
		{
			if (sender == null)
			{
				reference = GameManager.LocalPlayerInfo;
			}
			else
			{
				GameManager.RemotePlayerPerConnectionId.TryGetValue(sender.connectionId, out reference);
			}
			return reference.CanEnterGolfCart();
		}
	}

	protected static void InvokeUserCode_CmdEnter__NetworkConnectionToClient(NetworkBehaviour obj, NetworkReader reader, NetworkConnectionToClient senderConnection)
	{
		if (!NetworkServer.active)
		{
			Debug.LogError("Command CmdEnter called on client.");
		}
		else
		{
			((GolfCartInfo)obj).UserCode_CmdEnter__NetworkConnectionToClient(senderConnection);
		}
	}

	protected void UserCode_CmdCancelDriverSeatReservation__NetworkConnectionToClient(NetworkConnectionToClient sender)
	{
		if (serverCancelDriverSeatReservationCommandRateLimiter.RegisterHit(sender))
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
			if (!(value == null) && !(value != NetworkdriverSeatReserver))
			{
				AsEntity.DestroyEntity();
			}
		}
	}

	protected static void InvokeUserCode_CmdCancelDriverSeatReservation__NetworkConnectionToClient(NetworkBehaviour obj, NetworkReader reader, NetworkConnectionToClient senderConnection)
	{
		if (!NetworkServer.active)
		{
			Debug.LogError("Command CmdCancelDriverSeatReservation called on client.");
		}
		else
		{
			((GolfCartInfo)obj).UserCode_CmdCancelDriverSeatReservation__NetworkConnectionToClient(senderConnection);
		}
	}

	protected void UserCode_CmdTryChangeLocalPlayerSeat__Int32__NetworkConnectionToClient(int seat, NetworkConnectionToClient sender)
	{
		PlayerInfo passenger;
		if (serverTryChangeSeatCommandRateLimiter.RegisterHit(sender))
		{
			if (sender == null)
			{
				passenger = GameManager.LocalPlayerInfo;
			}
			else
			{
				GameManager.RemotePlayerPerConnectionId.TryGetValue(sender.connectionId, out passenger);
			}
			if (!(passenger == null) && !TryChangeSeat())
			{
				RpcInformPassengerSeatChangeFailed(passenger.connectionToClient);
			}
		}
		bool TryChangeSeat()
		{
			if (!passengerIndices.ContainsKey(passenger))
			{
				Debug.LogError($"{passenger.name} tried to change to seat {seat} in a golf cart, but they're not passengers", base.gameObject);
				return false;
			}
			if (!ServerTryAssignPassengerToSeat(passenger, seat, fromReservation: false))
			{
				return false;
			}
			return true;
		}
	}

	protected static void InvokeUserCode_CmdTryChangeLocalPlayerSeat__Int32__NetworkConnectionToClient(NetworkBehaviour obj, NetworkReader reader, NetworkConnectionToClient senderConnection)
	{
		if (!NetworkServer.active)
		{
			Debug.LogError("Command CmdTryChangeLocalPlayerSeat called on client.");
		}
		else
		{
			((GolfCartInfo)obj).UserCode_CmdTryChangeLocalPlayerSeat__Int32__NetworkConnectionToClient(reader.ReadVarInt(), senderConnection);
		}
	}

	protected void UserCode_RpcInformPassengerSeatChangeFailed__NetworkConnectionToClient(NetworkConnectionToClient connection)
	{
		if (!(GameManager.LocalPlayerInfo == null))
		{
			GameManager.LocalPlayerInfo.InformGolfCartSeatChangeFailed();
		}
	}

	protected static void InvokeUserCode_RpcInformPassengerSeatChangeFailed__NetworkConnectionToClient(NetworkBehaviour obj, NetworkReader reader, NetworkConnectionToClient senderConnection)
	{
		if (!NetworkClient.active)
		{
			Debug.LogError("TargetRPC RpcInformPassengerSeatChangeFailed called on server.");
		}
		else
		{
			((GolfCartInfo)obj).UserCode_RpcInformPassengerSeatChangeFailed__NetworkConnectionToClient(null);
		}
	}

	protected void UserCode_RpcLocalPlayerSetSeat__NetworkConnectionToClient__Int32__Boolean(NetworkConnectionToClient connection, int seat, bool fromReservation)
	{
		LocalPlayerSetSeat(seat, fromReservation);
	}

	protected static void InvokeUserCode_RpcLocalPlayerSetSeat__NetworkConnectionToClient__Int32__Boolean(NetworkBehaviour obj, NetworkReader reader, NetworkConnectionToClient senderConnection)
	{
		if (!NetworkClient.active)
		{
			Debug.LogError("TargetRPC RpcLocalPlayerSetSeat called on server.");
		}
		else
		{
			((GolfCartInfo)obj).UserCode_RpcLocalPlayerSetSeat__NetworkConnectionToClient__Int32__Boolean(null, reader.ReadVarInt(), reader.ReadBool());
		}
	}

	protected void UserCode_CmdInformDrivingSeatReservationReceived__Vector3__Quaternion__Vector3__NetworkConnectionToClient(Vector3 position, Quaternion rotation, Vector3 velocity, NetworkConnectionToClient sender)
	{
		if (!serverInformDrivingSeatReservationReceivedCommandRateLimiter.RegisterHit(sender) || NetworkdriverSeatReserver == null)
		{
			return;
		}
		PlayerInfo value;
		if (sender == null)
		{
			value = GameManager.LocalPlayerInfo;
		}
		else
		{
			GameManager.RemotePlayerPerConnectionId.TryGetValue(sender.connectionId, out value);
		}
		if (value == null || value != NetworkdriverSeatReserver)
		{
			return;
		}
		NetworkdriverSeatReserver.AsEntity.WillBeDestroyed -= OnServerDriverSeatReserverWillBeDestroyed;
		NetworkdriverSeatReserver = null;
		if (sender != null && sender != NetworkServer.localConnection)
		{
			ApplyDrivingSeatReservationReceivedData(position, rotation, velocity);
		}
		foreach (NetworkConnectionToClient value2 in NetworkServer.connections.Values)
		{
			if (value2 != NetworkServer.localConnection && value2 != sender)
			{
				RpcInformDrivingSeatReservationReceived(value2, position, rotation, velocity);
			}
		}
	}

	protected static void InvokeUserCode_CmdInformDrivingSeatReservationReceived__Vector3__Quaternion__Vector3__NetworkConnectionToClient(NetworkBehaviour obj, NetworkReader reader, NetworkConnectionToClient senderConnection)
	{
		if (!NetworkServer.active)
		{
			Debug.LogError("Command CmdInformDrivingSeatReservationReceived called on client.");
		}
		else
		{
			((GolfCartInfo)obj).UserCode_CmdInformDrivingSeatReservationReceived__Vector3__Quaternion__Vector3__NetworkConnectionToClient(reader.ReadVector3(), reader.ReadQuaternion(), reader.ReadVector3(), senderConnection);
		}
	}

	protected void UserCode_RpcInformDrivingSeatReservationReceived__NetworkConnectionToClient__Vector3__Quaternion__Vector3(NetworkConnectionToClient connection, Vector3 position, Quaternion rotation, Vector3 velocity)
	{
		ApplyDrivingSeatReservationReceivedData(position, rotation, velocity);
	}

	protected static void InvokeUserCode_RpcInformDrivingSeatReservationReceived__NetworkConnectionToClient__Vector3__Quaternion__Vector3(NetworkBehaviour obj, NetworkReader reader, NetworkConnectionToClient senderConnection)
	{
		if (!NetworkClient.active)
		{
			Debug.LogError("TargetRPC RpcInformDrivingSeatReservationReceived called on server.");
		}
		else
		{
			((GolfCartInfo)obj).UserCode_RpcInformDrivingSeatReservationReceived__NetworkConnectionToClient__Vector3__Quaternion__Vector3(null, reader.ReadVector3(), reader.ReadQuaternion(), reader.ReadVector3());
		}
	}

	protected void UserCode_CmdExit__NetworkConnectionToClient(NetworkConnectionToClient sender)
	{
		if (serverExitCommandRateLimiter.RegisterHit(sender))
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
			if (!passengerIndices.TryGetValue(value, out var value2))
			{
				Debug.LogError(value.name + " tried to exit a golf cart, but they're not in it", base.gameObject);
				return;
			}
			OnServerPassengerLeftSeat(value, value2);
			passengers[value2] = null;
		}
	}

	protected static void InvokeUserCode_CmdExit__NetworkConnectionToClient(NetworkBehaviour obj, NetworkReader reader, NetworkConnectionToClient senderConnection)
	{
		if (!NetworkServer.active)
		{
			Debug.LogError("Command CmdExit called on client.");
		}
		else
		{
			((GolfCartInfo)obj).UserCode_CmdExit__NetworkConnectionToClient(senderConnection);
		}
	}

	protected void UserCode_RpcPlayOutOfBoundsEliminationExplosion__NetworkConnectionToClient(NetworkConnectionToClient connection)
	{
		PlayOutOfBoundsEliminationExplosionInternal();
	}

	protected static void InvokeUserCode_RpcPlayOutOfBoundsEliminationExplosion__NetworkConnectionToClient(NetworkBehaviour obj, NetworkReader reader, NetworkConnectionToClient senderConnection)
	{
		if (!NetworkClient.active)
		{
			Debug.LogError("TargetRPC RpcPlayOutOfBoundsEliminationExplosion called on server.");
		}
		else
		{
			((GolfCartInfo)obj).UserCode_RpcPlayOutOfBoundsEliminationExplosion__NetworkConnectionToClient(null);
		}
	}

	protected void UserCode_CmdPlayHonkForAllClients__NetworkConnectionToClient(NetworkConnectionToClient sender)
	{
		if (!serverPlayHonkCommandRateLimiter.RegisterHit(sender))
		{
			return;
		}
		if (sender != null && sender != NetworkServer.localConnection)
		{
			PlayHonkInternal();
		}
		foreach (NetworkConnectionToClient value in NetworkServer.connections.Values)
		{
			if (value != NetworkServer.localConnection && value != sender)
			{
				RpcPlayHonk(value);
			}
		}
	}

	protected static void InvokeUserCode_CmdPlayHonkForAllClients__NetworkConnectionToClient(NetworkBehaviour obj, NetworkReader reader, NetworkConnectionToClient senderConnection)
	{
		if (!NetworkServer.active)
		{
			Debug.LogError("Command CmdPlayHonkForAllClients called on client.");
		}
		else
		{
			((GolfCartInfo)obj).UserCode_CmdPlayHonkForAllClients__NetworkConnectionToClient(senderConnection);
		}
	}

	protected void UserCode_RpcPlayHonk__NetworkConnectionToClient(NetworkConnectionToClient connection)
	{
		PlayHonkInternal();
	}

	protected static void InvokeUserCode_RpcPlayHonk__NetworkConnectionToClient(NetworkBehaviour obj, NetworkReader reader, NetworkConnectionToClient senderConnection)
	{
		if (!NetworkClient.active)
		{
			Debug.LogError("TargetRPC RpcPlayHonk called on server.");
		}
		else
		{
			((GolfCartInfo)obj).UserCode_RpcPlayHonk__NetworkConnectionToClient(null);
		}
	}

	protected void UserCode_CmdStartPlayingSpecialHonkForAllClients__NetworkConnectionToClient(NetworkConnectionToClient sender)
	{
		if (!serverStartPlayingSpecialHonkCommandRateLimiter.RegisterHit(sender))
		{
			return;
		}
		if (sender != null && sender != NetworkServer.localConnection)
		{
			StartPlayingSpecialHonkInternal();
		}
		foreach (NetworkConnectionToClient value in NetworkServer.connections.Values)
		{
			if (value != NetworkServer.localConnection && value != sender)
			{
				RpcStartPlayingSpecialHonk(value);
			}
		}
	}

	protected static void InvokeUserCode_CmdStartPlayingSpecialHonkForAllClients__NetworkConnectionToClient(NetworkBehaviour obj, NetworkReader reader, NetworkConnectionToClient senderConnection)
	{
		if (!NetworkServer.active)
		{
			Debug.LogError("Command CmdStartPlayingSpecialHonkForAllClients called on client.");
		}
		else
		{
			((GolfCartInfo)obj).UserCode_CmdStartPlayingSpecialHonkForAllClients__NetworkConnectionToClient(senderConnection);
		}
	}

	protected void UserCode_RpcStartPlayingSpecialHonk__NetworkConnectionToClient(NetworkConnectionToClient connection)
	{
		StartPlayingSpecialHonkInternal();
	}

	protected static void InvokeUserCode_RpcStartPlayingSpecialHonk__NetworkConnectionToClient(NetworkBehaviour obj, NetworkReader reader, NetworkConnectionToClient senderConnection)
	{
		if (!NetworkClient.active)
		{
			Debug.LogError("TargetRPC RpcStartPlayingSpecialHonk called on server.");
		}
		else
		{
			((GolfCartInfo)obj).UserCode_RpcStartPlayingSpecialHonk__NetworkConnectionToClient(null);
		}
	}

	protected void UserCode_CmdEndSpecialHonkForAllClients__NetworkConnectionToClient(NetworkConnectionToClient sender)
	{
		if (!serverEndSpecialHonkCommandRateLimiter.RegisterHit(sender))
		{
			return;
		}
		if (sender != null && sender != NetworkServer.localConnection)
		{
			EndSpecialHonkInternal();
		}
		foreach (NetworkConnectionToClient value in NetworkServer.connections.Values)
		{
			if (value != NetworkServer.localConnection && value != sender)
			{
				RpcEndSpecialHonk(value);
			}
		}
	}

	protected static void InvokeUserCode_CmdEndSpecialHonkForAllClients__NetworkConnectionToClient(NetworkBehaviour obj, NetworkReader reader, NetworkConnectionToClient senderConnection)
	{
		if (!NetworkServer.active)
		{
			Debug.LogError("Command CmdEndSpecialHonkForAllClients called on client.");
		}
		else
		{
			((GolfCartInfo)obj).UserCode_CmdEndSpecialHonkForAllClients__NetworkConnectionToClient(senderConnection);
		}
	}

	protected void UserCode_RpcEndSpecialHonk__NetworkConnectionToClient(NetworkConnectionToClient connection)
	{
		EndSpecialHonkInternal();
	}

	protected static void InvokeUserCode_RpcEndSpecialHonk__NetworkConnectionToClient(NetworkBehaviour obj, NetworkReader reader, NetworkConnectionToClient senderConnection)
	{
		if (!NetworkClient.active)
		{
			Debug.LogError("TargetRPC RpcEndSpecialHonk called on server.");
		}
		else
		{
			((GolfCartInfo)obj).UserCode_RpcEndSpecialHonk__NetworkConnectionToClient(null);
		}
	}

	protected void UserCode_RpcPlayWaterSplash__NetworkConnectionToClient__Vector3(NetworkConnectionToClient connection, Vector3 worldPosition)
	{
		PlayWaterSplashInternal(worldPosition);
	}

	protected static void InvokeUserCode_RpcPlayWaterSplash__NetworkConnectionToClient__Vector3(NetworkBehaviour obj, NetworkReader reader, NetworkConnectionToClient senderConnection)
	{
		if (!NetworkClient.active)
		{
			Debug.LogError("TargetRPC RpcPlayWaterSplash called on server.");
		}
		else
		{
			((GolfCartInfo)obj).UserCode_RpcPlayWaterSplash__NetworkConnectionToClient__Vector3(null, reader.ReadVector3());
		}
	}

	static GolfCartInfo()
	{
		RemoteProcedureCalls.RegisterCommand(typeof(GolfCartInfo), "System.Void GolfCartInfo::CmdEnterReservedGolfCart(Mirror.NetworkConnectionToClient)", InvokeUserCode_CmdEnterReservedGolfCart__NetworkConnectionToClient, requiresAuthority: true);
		RemoteProcedureCalls.RegisterCommand(typeof(GolfCartInfo), "System.Void GolfCartInfo::CmdEnter(Mirror.NetworkConnectionToClient)", InvokeUserCode_CmdEnter__NetworkConnectionToClient, requiresAuthority: false);
		RemoteProcedureCalls.RegisterCommand(typeof(GolfCartInfo), "System.Void GolfCartInfo::CmdCancelDriverSeatReservation(Mirror.NetworkConnectionToClient)", InvokeUserCode_CmdCancelDriverSeatReservation__NetworkConnectionToClient, requiresAuthority: true);
		RemoteProcedureCalls.RegisterCommand(typeof(GolfCartInfo), "System.Void GolfCartInfo::CmdTryChangeLocalPlayerSeat(System.Int32,Mirror.NetworkConnectionToClient)", InvokeUserCode_CmdTryChangeLocalPlayerSeat__Int32__NetworkConnectionToClient, requiresAuthority: false);
		RemoteProcedureCalls.RegisterCommand(typeof(GolfCartInfo), "System.Void GolfCartInfo::CmdInformDrivingSeatReservationReceived(UnityEngine.Vector3,UnityEngine.Quaternion,UnityEngine.Vector3,Mirror.NetworkConnectionToClient)", InvokeUserCode_CmdInformDrivingSeatReservationReceived__Vector3__Quaternion__Vector3__NetworkConnectionToClient, requiresAuthority: true);
		RemoteProcedureCalls.RegisterCommand(typeof(GolfCartInfo), "System.Void GolfCartInfo::CmdExit(Mirror.NetworkConnectionToClient)", InvokeUserCode_CmdExit__NetworkConnectionToClient, requiresAuthority: false);
		RemoteProcedureCalls.RegisterCommand(typeof(GolfCartInfo), "System.Void GolfCartInfo::CmdPlayHonkForAllClients(Mirror.NetworkConnectionToClient)", InvokeUserCode_CmdPlayHonkForAllClients__NetworkConnectionToClient, requiresAuthority: false);
		RemoteProcedureCalls.RegisterCommand(typeof(GolfCartInfo), "System.Void GolfCartInfo::CmdStartPlayingSpecialHonkForAllClients(Mirror.NetworkConnectionToClient)", InvokeUserCode_CmdStartPlayingSpecialHonkForAllClients__NetworkConnectionToClient, requiresAuthority: false);
		RemoteProcedureCalls.RegisterCommand(typeof(GolfCartInfo), "System.Void GolfCartInfo::CmdEndSpecialHonkForAllClients(Mirror.NetworkConnectionToClient)", InvokeUserCode_CmdEndSpecialHonkForAllClients__NetworkConnectionToClient, requiresAuthority: false);
		RemoteProcedureCalls.RegisterRpc(typeof(GolfCartInfo), "System.Void GolfCartInfo::RpcSetMovementSyncDirection(Mirror.NetworkConnectionToClient,Mirror.SyncDirection)", InvokeUserCode_RpcSetMovementSyncDirection__NetworkConnectionToClient__SyncDirection);
		RemoteProcedureCalls.RegisterRpc(typeof(GolfCartInfo), "System.Void GolfCartInfo::RpcInformPassengerSeatChangeFailed(Mirror.NetworkConnectionToClient)", InvokeUserCode_RpcInformPassengerSeatChangeFailed__NetworkConnectionToClient);
		RemoteProcedureCalls.RegisterRpc(typeof(GolfCartInfo), "System.Void GolfCartInfo::RpcLocalPlayerSetSeat(Mirror.NetworkConnectionToClient,System.Int32,System.Boolean)", InvokeUserCode_RpcLocalPlayerSetSeat__NetworkConnectionToClient__Int32__Boolean);
		RemoteProcedureCalls.RegisterRpc(typeof(GolfCartInfo), "System.Void GolfCartInfo::RpcInformDrivingSeatReservationReceived(Mirror.NetworkConnectionToClient,UnityEngine.Vector3,UnityEngine.Quaternion,UnityEngine.Vector3)", InvokeUserCode_RpcInformDrivingSeatReservationReceived__NetworkConnectionToClient__Vector3__Quaternion__Vector3);
		RemoteProcedureCalls.RegisterRpc(typeof(GolfCartInfo), "System.Void GolfCartInfo::RpcPlayOutOfBoundsEliminationExplosion(Mirror.NetworkConnectionToClient)", InvokeUserCode_RpcPlayOutOfBoundsEliminationExplosion__NetworkConnectionToClient);
		RemoteProcedureCalls.RegisterRpc(typeof(GolfCartInfo), "System.Void GolfCartInfo::RpcPlayHonk(Mirror.NetworkConnectionToClient)", InvokeUserCode_RpcPlayHonk__NetworkConnectionToClient);
		RemoteProcedureCalls.RegisterRpc(typeof(GolfCartInfo), "System.Void GolfCartInfo::RpcStartPlayingSpecialHonk(Mirror.NetworkConnectionToClient)", InvokeUserCode_RpcStartPlayingSpecialHonk__NetworkConnectionToClient);
		RemoteProcedureCalls.RegisterRpc(typeof(GolfCartInfo), "System.Void GolfCartInfo::RpcEndSpecialHonk(Mirror.NetworkConnectionToClient)", InvokeUserCode_RpcEndSpecialHonk__NetworkConnectionToClient);
		RemoteProcedureCalls.RegisterRpc(typeof(GolfCartInfo), "System.Void GolfCartInfo::RpcPlayWaterSplash(Mirror.NetworkConnectionToClient,UnityEngine.Vector3)", InvokeUserCode_RpcPlayWaterSplash__NetworkConnectionToClient__Vector3);
	}

	public override void SerializeSyncVars(NetworkWriter writer, bool forceAll)
	{
		base.SerializeSyncVars(writer, forceAll);
		if (forceAll)
		{
			writer.WriteNetworkBehaviour(NetworkdriverSeatReserver);
			writer.WriteNetworkBehaviour(NetworkresponsiblePlayer);
			writer.WriteFloat(outOfBoundsRemainingTime);
			return;
		}
		writer.WriteVarULong(syncVarDirtyBits);
		if ((syncVarDirtyBits & 1L) != 0L)
		{
			writer.WriteNetworkBehaviour(NetworkdriverSeatReserver);
		}
		if ((syncVarDirtyBits & 2L) != 0L)
		{
			writer.WriteNetworkBehaviour(NetworkresponsiblePlayer);
		}
		if ((syncVarDirtyBits & 4L) != 0L)
		{
			writer.WriteFloat(outOfBoundsRemainingTime);
		}
	}

	public override void DeserializeSyncVars(NetworkReader reader, bool initialState)
	{
		base.DeserializeSyncVars(reader, initialState);
		if (initialState)
		{
			GeneratedSyncVarDeserialize_NetworkBehaviour(ref driverSeatReserver, _Mirror_SyncVarHookDelegate_driverSeatReserver, reader, ref ___driverSeatReserverNetId);
			GeneratedSyncVarDeserialize_NetworkBehaviour(ref responsiblePlayer, null, reader, ref ___responsiblePlayerNetId);
			GeneratedSyncVarDeserialize(ref outOfBoundsRemainingTime, null, reader.ReadFloat());
			return;
		}
		long num = (long)reader.ReadVarULong();
		if ((num & 1L) != 0L)
		{
			GeneratedSyncVarDeserialize_NetworkBehaviour(ref driverSeatReserver, _Mirror_SyncVarHookDelegate_driverSeatReserver, reader, ref ___driverSeatReserverNetId);
		}
		if ((num & 2L) != 0L)
		{
			GeneratedSyncVarDeserialize_NetworkBehaviour(ref responsiblePlayer, null, reader, ref ___responsiblePlayerNetId);
		}
		if ((num & 4L) != 0L)
		{
			GeneratedSyncVarDeserialize(ref outOfBoundsRemainingTime, null, reader.ReadFloat());
		}
	}
}
