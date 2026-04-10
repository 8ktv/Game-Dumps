using System;
using System.Collections.Generic;
using Mirror;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerInput : NetworkBehaviour
{
	[SerializeField]
	private BInputSettings settings;

	private PlayerInfo playerInfo;

	private bool initialized;

	private InputMode playerInputMode;

	private int gamepadPitchInputDirection;

	private double gamepadPitchInputDirectionChangedTimestamp;

	private double gamepadPitchInputRepeatTimestamp;

	private ManualConditionalInputBuffer hotkeyInputBuffer;

	private readonly List<IInputBuffer> bufferedInputs = new List<IInputBuffer>();

	private static bool wasPushToToggleVoiceChatEnabledOnSceneChange;

	private int lastExitFrame = int.MinValue;

	public static GameControls Controls => InputManager.Controls;

	public bool IsHoldingAimSwing { get; private set; }

	public bool IsHoldingChargeSwing { get; private set; }

	public bool IsHoldingWalk { get; private set; }

	public bool IsPushingToTalk { get; private set; }

	public static bool UsingKeyboard => InputManager.UsingKeyboard;

	public static bool UsingGamepad => InputManager.UsingGamepad;

	public static event Action SwitchedToGamepad;

	public static event Action SwitchedToKeyboard;

	public static event Action SwitchedInputDeviceType;

	private FullDurationInputBuffer AddFullDurationInputBuffer(InputAction action, Action OnActivated, Action OnTimedOut = null, float bufferDuration = -1f)
	{
		if (bufferDuration <= 0f)
		{
			bufferDuration = settings.DefaultBufferDuration;
		}
		FullDurationInputBuffer fullDurationInputBuffer = new FullDurationInputBuffer(action, OnActivated, bufferDuration, OnTimedOut);
		bufferedInputs.Add(fullDurationInputBuffer);
		return fullDurationInputBuffer;
	}

	private HeldConditionalInputBuffer AddHeldInputBuffer(InputAction action, Func<bool> TryUseInput, Action OnRelease, Action OnTimedOut = null, float bufferDuration = -1f)
	{
		if (bufferDuration <= 0f)
		{
			bufferDuration = settings.DefaultBufferDuration;
		}
		HeldConditionalInputBuffer heldConditionalInputBuffer = new HeldConditionalInputBuffer(action, TryUseInput, bufferDuration, OnRelease, OnTimedOut);
		bufferedInputs.Add(heldConditionalInputBuffer);
		return heldConditionalInputBuffer;
	}

	private HeldConditionalInputBuffer AddHeldInputInstantBuffer(InputAction action, Func<bool> TryUseInput, Action OnRelease, Action OnTimedOut = null)
	{
		HeldConditionalInputBuffer heldConditionalInputBuffer = new HeldConditionalInputBuffer(action, TryUseInput, 0f, OnRelease, OnTimedOut);
		bufferedInputs.Add(heldConditionalInputBuffer);
		return heldConditionalInputBuffer;
	}

	private ConditionalInputBuffer AddInputBuffer(InputAction action, Func<bool> TryUseInputOnActivation, Func<bool> TryUseBufferedInput, Action OnDeactivated = null, float bufferDuration = -1f)
	{
		if (bufferDuration <= 0f)
		{
			bufferDuration = settings.DefaultBufferDuration;
		}
		ConditionalInputBuffer conditionalInputBuffer = new ConditionalInputBuffer(action, TryUseInputOnActivation, TryUseBufferedInput, bufferDuration, OnDeactivated);
		bufferedInputs.Add(conditionalInputBuffer);
		return conditionalInputBuffer;
	}

	private ManualConditionalInputBuffer AddManualInputBuffer(Action OnDeactivated = null, float bufferDuration = -1f)
	{
		if (bufferDuration <= 0f)
		{
			bufferDuration = settings.DefaultBufferDuration;
		}
		ManualConditionalInputBuffer manualConditionalInputBuffer = new ManualConditionalInputBuffer(bufferDuration, OnDeactivated);
		bufferedInputs.Add(manualConditionalInputBuffer);
		return manualConditionalInputBuffer;
	}

	private void Awake()
	{
		playerInfo = GetComponent<PlayerInfo>();
	}

	public override void OnStartLocalPlayer()
	{
		AddInputBuffer(Controls.Gameplay.Jump, Jump, Jump);
		Controls.Gameplay.Swing.started += StartChargingSwing;
		Controls.Gameplay.Swing.canceled += FinishChargingSwing;
		AddInputBuffer(Controls.Gameplay.Swing, UseItem, UseItem);
		AddInputBuffer(Controls.Gameplay.Interact, Interact, Interact, null, settings.InteractBufferDuration);
		Controls.Gameplay.Cancel.performed += Cancel;
		Controls.Gameplay.DropItem.performed += DropItem;
		Controls.Gameplay.Dive.started += Dive;
		Controls.Gameplay.Walk.started += StartWalk;
		Controls.Gameplay.Walk.canceled += EndWalk;
		Controls.Gameplay.CycleSwingAngle.performed += CycleSwingAngle;
		Controls.Gameplay.Restart.performed += RestartMatch;
		AddInputBuffer(Controls.GolfCartDriver.Jump, GolfCartJump, GolfCartJump);
		Controls.GolfCartDriver.Honk.started += GolfCartHonk;
		Controls.GolfCartShared.Exit.started += ExitGolfCart;
		Controls.GolfCartShared.DiveOut.started += DiveOutOfGolfCart;
		hotkeyInputBuffer = AddManualInputBuffer();
		Controls.Hotkeys.Hotkey1.performed += SelectHotkey1;
		Controls.Hotkeys.Hotkey2.performed += SelectHotkey2;
		Controls.Hotkeys.Hotkey3.performed += SelectHotkey3;
		Controls.Hotkeys.Hotkey4.performed += SelectHotkey4;
		Controls.Hotkeys.Hotkey5.performed += SelectHotkey5;
		Controls.Hotkeys.Hotkey6.performed += SelectHotkey6;
		Controls.Hotkeys.Hotkey7.performed += SelectHotkey7;
		Controls.Hotkeys.Hotkey8.performed += SelectHotkey8;
		Controls.Hotkeys.CycleLeft.performed += CycleHotkeysLeft;
		Controls.Hotkeys.CycleRight.performed += CycleHotkeysRight;
		Controls.Hotkeys.Toggle.performed += HotkeyToggleLastSelected;
		Controls.Spectate.CyclePreviousPlayer.performed += SpectatorCyclePreviousPlayer;
		Controls.Spectate.CycleNextPlayer.performed += SpectatorCycleNextPlayer;
		Controls.Ingame.ShowScoreboard.started += ShowScoreboard;
		Controls.Ingame.ShowScoreboard.canceled += HideScoreboard;
		Controls.Ingame.Pause.started += Pause;
		Controls.Ingame.OpenChat.started += OpenChat;
		Controls.Ingame.ToggleEmoteMenu.performed += ToggleEmoteMenu;
		Controls.VoiceChat.PushToTalk.started += StartHoldingPushToTalk;
		Controls.VoiceChat.PushToTalk.canceled += StopHoldingPushToTalk;
		Controls.RadialMenu.Select.started += SelectRadialMenuOption;
		Controls.RadialMenu.Cancel.started += CancelRadialMenu;
		Controls.UI.Cancel.started += ExitUi;
		Controls.UI.Exit.started += ExitUi;
		EnablePlayerInputModeInternal(InputMode.Regular);
		IsPushingToTalk = GameSettings.All.Audio.MicInputMode switch
		{
			GameSettings.AudioSettings.InputMode.PushToTalk => Controls.VoiceChat.PushToTalk.IsPressed(), 
			GameSettings.AudioSettings.InputMode.PushToToggle => wasPushToToggleVoiceChatEnabledOnSceneChange, 
			_ => false, 
		};
		InputManager.SwitchedToGamepad += OnSwitchedToGamepad;
		InputManager.SwitchedToKeyboard += OnSwitchedToKeyboard;
		InputManager.SwitchedInputDeviceType += OnSwitchedInput;
		Hotkeys.ModeChanged += OnHotkeyModeChanged;
		GameSettings.AudioSettings.MicInputModeChanged += OnMicInputModeChanged;
		if (UsingGamepad)
		{
			PlayerInput.SwitchedToGamepad?.Invoke();
		}
		else
		{
			PlayerInput.SwitchedToKeyboard?.Invoke();
		}
		PlayerInput.SwitchedInputDeviceType?.Invoke();
		initialized = true;
	}

	public override void OnStopLocalPlayer()
	{
		if (BNetworkManager.IsShuttingDown)
		{
			wasPushToToggleVoiceChatEnabledOnSceneChange = false;
		}
		else if (BNetworkManager.IsChangingScene)
		{
			wasPushToToggleVoiceChatEnabledOnSceneChange = GameSettings.All.Audio.MicInputMode == GameSettings.AudioSettings.InputMode.PushToToggle && IsPushingToTalk;
		}
		Controls.Gameplay.Swing.started -= StartChargingSwing;
		Controls.Gameplay.Swing.canceled -= FinishChargingSwing;
		Controls.Gameplay.Cancel.performed -= Cancel;
		Controls.Gameplay.DropItem.performed -= DropItem;
		Controls.Gameplay.Dive.started -= Dive;
		Controls.Gameplay.Walk.started -= StartWalk;
		Controls.Gameplay.Walk.canceled -= EndWalk;
		Controls.Gameplay.CycleSwingAngle.performed -= CycleSwingAngle;
		Controls.Gameplay.Restart.performed -= RestartMatch;
		Controls.GolfCartDriver.Honk.started -= GolfCartHonk;
		Controls.GolfCartShared.Exit.started -= ExitGolfCart;
		Controls.GolfCartShared.DiveOut.started -= DiveOutOfGolfCart;
		Controls.Hotkeys.Hotkey1.performed -= SelectHotkey1;
		Controls.Hotkeys.Hotkey2.performed -= SelectHotkey2;
		Controls.Hotkeys.Hotkey3.performed -= SelectHotkey3;
		Controls.Hotkeys.Hotkey4.performed -= SelectHotkey4;
		Controls.Hotkeys.Hotkey5.performed -= SelectHotkey5;
		Controls.Hotkeys.Hotkey6.performed -= SelectHotkey6;
		Controls.Hotkeys.Hotkey7.performed -= SelectHotkey7;
		Controls.Hotkeys.Hotkey8.performed -= SelectHotkey8;
		Controls.Hotkeys.CycleLeft.performed -= CycleHotkeysLeft;
		Controls.Hotkeys.CycleRight.performed -= CycleHotkeysRight;
		Controls.Hotkeys.Toggle.performed -= HotkeyToggleLastSelected;
		Controls.Spectate.CyclePreviousPlayer.performed -= SpectatorCyclePreviousPlayer;
		Controls.Spectate.CycleNextPlayer.performed -= SpectatorCycleNextPlayer;
		Controls.Ingame.ShowScoreboard.started -= ShowScoreboard;
		Controls.Ingame.ShowScoreboard.canceled -= HideScoreboard;
		Controls.Ingame.Pause.started -= Pause;
		Controls.Ingame.OpenChat.started -= OpenChat;
		Controls.Ingame.ToggleEmoteMenu.performed -= ToggleEmoteMenu;
		Controls.VoiceChat.PushToTalk.started -= StartHoldingPushToTalk;
		Controls.VoiceChat.PushToTalk.canceled -= StopHoldingPushToTalk;
		Controls.RadialMenu.Select.started -= SelectRadialMenuOption;
		Controls.RadialMenu.Cancel.started -= CancelRadialMenu;
		Controls.UI.Cancel.started -= ExitUi;
		Controls.UI.Exit.started -= ExitUi;
		DisablePlayerInputModeInternal(playerInputMode);
		InputManager.SwitchedToGamepad -= OnSwitchedToGamepad;
		InputManager.SwitchedToKeyboard -= OnSwitchedToKeyboard;
		InputManager.SwitchedInputDeviceType -= OnSwitchedInput;
		Hotkeys.ModeChanged -= OnHotkeyModeChanged;
		GameSettings.AudioSettings.MicInputModeChanged -= OnMicInputModeChanged;
		foreach (IInputBuffer bufferedInput in bufferedInputs)
		{
			bufferedInput.OnDestroy();
		}
	}

	private void Update()
	{
		if (!base.isLocalPlayer || !initialized || PhotoCameraModule.Active)
		{
			return;
		}
		foreach (IInputBuffer bufferedInput in bufferedInputs)
		{
			bufferedInput.Update(Time.deltaTime);
		}
		UpdateMovementInput();
		UpdateAimingState();
		UpdatePitch();
		UpdateCameraInput();
		static float ApplyDeadZone(float value, float deadZone)
		{
			if (BMath.Abs(value) < deadZone)
			{
				return 0f;
			}
			return value;
		}
		void ApplyPitchTick()
		{
			int num = (playerInfo.AsGolfer.IsAimingSwing ? settings.AimingPitchChangePerTick : settings.NonAimingPitchChangePerTick);
			float swingPitch = playerInfo.AsGolfer.SwingPitch;
			float num2 = ((P_0.pitchInputDirection > 0) ? BMath.CeilToMultipleOf(swingPitch, num) : BMath.FloorToMultipleOf(swingPitch, num));
			float num3 = (swingPitch.Approximately(num2) ? (swingPitch + (float)(P_0.pitchInputDirection * num)) : num2);
			if (0f < num3 && num3 < (float)settings.MinNonPuttPitch)
			{
				num3 = ((P_0.pitchInputDirection > 0) ? ((float)settings.MinNonPuttPitch) : 0f);
			}
			playerInfo.AsGolfer.SetPitch(num3);
		}
		bool CanChangePitch()
		{
			if (playerInfo.ActiveGolfCartSeat.IsValid())
			{
				return false;
			}
			if (!playerInfo.Movement.IsVisible)
			{
				return false;
			}
			if (playerInfo.AsGolfer.IsMatchResolved)
			{
				return false;
			}
			if (playerInfo.AsSpectator.IsSpectating)
			{
				return false;
			}
			ItemType effectivelyEquippedItem = playerInfo.Inventory.GetEffectivelyEquippedItem();
			if (effectivelyEquippedItem != ItemType.None && effectivelyEquippedItem != ItemType.RocketDriver)
			{
				return false;
			}
			return true;
		}
		static Vector2 MimicLegacyInputAxes(Vector2 current, Vector2 target)
		{
			current = new Vector2((current.x * target.x >= 0f) ? current.x : 0f, (current.y * target.y >= 0f) ? current.y : 0f);
			Vector2 vector = target.Abs() / 0.25f;
			target = new Vector2((BMath.Abs(target.x) < 0.25f) ? (target.x * BMath.EaseIn(vector.x)) : target.x, (BMath.Abs(target.y) < 0.25f) ? (target.y * BMath.EaseIn(vector.y)) : target.y);
			Vector2 vector2 = target - current;
			float num = BMath.Min(vector2.magnitude / Time.deltaTime, 21f);
			return current + num * Time.deltaTime * vector2.normalized;
		}
		void UpdateAimingState()
		{
			bool isHoldingAimSwing = IsHoldingAimSwing;
			IsHoldingAimSwing = Controls.Gameplay.Aim.ReadValue<float>() > 0.05f;
			if (IsHoldingAimSwing != isHoldingAimSwing && base.isLocalPlayer)
			{
				playerInfo.AsGolfer.InformLocalPlayerIsHoldingAimChanged();
				playerInfo.Inventory.InformLocalPlayerIsHoldingAimChanged();
			}
		}
		void UpdateCameraInput()
		{
			if (CameraModuleController.CurrentModuleType == CameraModuleType.Orbit)
			{
				float num = 1f;
				if (CameraModuleController.IsTransitioning && CameraModuleController.PreviousModuleType == CameraModuleType.Overview)
				{
					num = BMath.InverseLerpClamped(CameraModuleController.TransitionDuration - 0.5f, CameraModuleController.TransitionDuration, CameraModuleController.TransitionTime);
				}
				OrbitCameraModule orbitCameraModule = CameraModuleController.CurrentModule as OrbitCameraModule;
				bool isAiming = IsHoldingAimSwing && (playerInfo.AsGolfer.IsAimingSwing || playerInfo.Inventory.IsAimingItem);
				Vector2 axis = Controls.Camera.Look.ReadValue<Vector2>() * num * (UsingGamepad ? Time.unscaledDeltaTime : 0.6f);
				axis = GameSettings.All.Controls.Camera.ScaleInput(axis, isAiming);
				if (CursorManager.CursorLocked)
				{
					orbitCameraModule.Rotate(axis);
				}
				else
				{
					orbitCameraModule.InformNoRotation();
				}
			}
		}
		void UpdateGolfCartMovementInput()
		{
			if (playerInfo.ActiveGolfCartSeat.IsDriver())
			{
				GolfCartMovement movement = playerInfo.ActiveGolfCartSeat.golfCart.Movement;
				movement.SetIsAccelerating(Controls.GolfCartDriver.Accelerate.ReadValue<float>() > 0.5f);
				movement.SetIsBraking(Controls.GolfCartDriver.Brake.ReadValue<float>() > 0.5f);
				movement.SetSteering(ApplyDeadZone(Controls.GolfCartDriver.Steer.ReadValue<float>(), 0.1f));
			}
		}
		void UpdateMovementInput()
		{
			if (InputManager.CurrentMode != InputMode.GolfCartPassenger)
			{
				if (InputManager.CurrentMode == InputMode.GolfCartDriver)
				{
					UpdateGolfCartMovementInput();
				}
				else
				{
					Vector2 rawMoveVector2d = Controls.Gameplay.Move.ReadValue<Vector2>();
					playerInfo.Movement.rawMoveVector2d = rawMoveVector2d;
					playerInfo.Movement.moveVector2d = MimicLegacyInputAxes(playerInfo.Movement.moveVector2d, playerInfo.Movement.rawMoveVector2d);
				}
			}
		}
		void UpdatePitch()
		{
			int pitchInputDirection;
			if (CanChangePitch())
			{
				float num = Controls.Gameplay.Pitch.ReadValue<float>();
				pitchInputDirection = ((BMath.Abs(num) > 0.05f) ? BMath.Sign(num) : 0);
				if (InputManager.CurrentDeviceType == InputManager.DeviceType.KeyboardAndMouse)
				{
					UpdateMouseAndKeyboardPitch();
				}
				else
				{
					UpdateGamepadPitch();
				}
			}
			void UpdateGamepadPitch()
			{
				int num2 = gamepadPitchInputDirection;
				gamepadPitchInputDirection = pitchInputDirection;
				bool flag = gamepadPitchInputDirection != num2;
				if (flag)
				{
					gamepadPitchInputDirectionChangedTimestamp = Time.timeAsDouble;
				}
				if (gamepadPitchInputDirection != 0)
				{
					if (BMath.GetTimeSince(gamepadPitchInputDirectionChangedTimestamp) < settings.GamepadPitchHoldRepeatStartTime)
					{
						if (flag)
						{
							ApplyPitchTick();
						}
						gamepadPitchInputRepeatTimestamp = double.MinValue;
					}
					else if (!(BMath.GetTimeSince(gamepadPitchInputRepeatTimestamp) < settings.GamepadPitchRepeatInterval))
					{
						gamepadPitchInputRepeatTimestamp = Time.timeAsDouble;
						ApplyPitchTick();
					}
				}
			}
			void UpdateMouseAndKeyboardPitch()
			{
				if (pitchInputDirection != 0)
				{
					ApplyPitchTick();
				}
			}
		}
	}

	public void EnablePlayerInputMode(InputMode inputMode)
	{
		EnablePlayerInputModeInternal(inputMode);
	}

	public void DisablePlayerInputMode(InputMode inputMode)
	{
		DisablePlayerInputModeInternal(inputMode);
	}

	public void UpdateHotkeyMode()
	{
		if (playerInfo.ActiveGolfCartSeat.IsValid())
		{
			Hotkeys.SetMode(HotkeyMode.GolfCart);
			Hotkeys.UpdateOccupiedGolfCartSeats(playerInfo.ActiveGolfCartSeat);
			return;
		}
		if (!playerInfo.AsGolfer.IsAimingSwing)
		{
			Hotkeys.SetMode(HotkeyMode.Inventory);
			return;
		}
		Hotkeys.SetMode(HotkeyMode.SwingPitch);
		for (int i = 0; i < GameManager.GolfSettings.HotkeySwingPresets.Length; i++)
		{
			if (playerInfo.AsGolfer.SwingPitch.Approximately(GameManager.GolfSettings.HotkeySwingPresets[i], 0.5f))
			{
				Hotkeys.Select(i);
				break;
			}
		}
	}

	private void EnablePlayerInputModeInternal(InputMode inputMode)
	{
		playerInputMode |= inputMode;
		InputManager.EnableMode(inputMode);
	}

	private void DisablePlayerInputModeInternal(InputMode inputMode)
	{
		playerInputMode &= ~inputMode;
		InputManager.DisableMode(inputMode);
	}

	private bool Jump()
	{
		bool num = playerInfo.Movement.TryTriggerJump();
		if (num)
		{
			TutorialManager.CompletePrompt(TutorialPrompt.Jump);
		}
		return num;
	}

	private void StartChargingSwing(InputAction.CallbackContext context)
	{
		IsHoldingChargeSwing = true;
		playerInfo.AsGolfer.TryStartChargingSwing();
	}

	private void FinishChargingSwing(InputAction.CallbackContext context)
	{
		IsHoldingChargeSwing = false;
		playerInfo.AsGolfer.ReleaseSwingCharge();
	}

	private bool UseItem()
	{
		bool shouldEatInput;
		return playerInfo.Inventory.TryUseItem(isAirhornReaction: false, out shouldEatInput) || shouldEatInput;
	}

	private bool Interact()
	{
		bool num = playerInfo.AsInteractor.TryInteract();
		if (num)
		{
			TutorialManager.CompletePrompt(TutorialPrompt.Interact);
		}
		return num;
	}

	private void Cancel(InputAction.CallbackContext context)
	{
		if (playerInfo.AsGolfer.TryCancelSwingCharge(fromInput: true))
		{
			TutorialManager.CompletePrompt(TutorialPrompt.CancelSwing);
		}
	}

	private void DropItem(InputAction.CallbackContext context)
	{
		playerInfo.Inventory.DropItem();
	}

	private void Dive(InputAction.CallbackContext context)
	{
		if (playerInfo.Movement.TryDive())
		{
			TutorialManager.CompletePrompt(TutorialPrompt.Dive);
		}
	}

	private void StartWalk(InputAction.CallbackContext context)
	{
		IsHoldingWalk = true;
	}

	private void EndWalk(InputAction.CallbackContext context)
	{
		IsHoldingWalk = false;
	}

	private void CycleSwingAngle(InputAction.CallbackContext context)
	{
		playerInfo.AsGolfer.CycleSwingPitchPreset(1);
	}

	private void RestartMatch(InputAction.CallbackContext context)
	{
		playerInfo.RestartPlayer();
	}

	private void ToggleEmoteMenu(InputAction.CallbackContext context)
	{
		RadialMenuMode radialMenuMode = ((!playerInfo.AsSpectator.IsSpectating) ? RadialMenuMode.Emote : RadialMenuMode.SpectatorEmote);
		if (RadialMenu.CurrentMode == radialMenuMode)
		{
			RadialMenu.Hide();
		}
		else
		{
			if (!CanOpenEmoteMenu())
			{
				return;
			}
			Action<int> action = default(Action<int>);
			switch (radialMenuMode)
			{
			case RadialMenuMode.Emote:
				action = OnEmoteSelected;
				break;
			case RadialMenuMode.SpectatorEmote:
				action = OnSpectatorEmoteSelected;
				break;
			default:
				global::_003CPrivateImplementationDetails_003E.ThrowSwitchExpressionException(radialMenuMode);
				break;
			}
			Action<int> onSelected = action;
			if (!RadialMenu.TryShow(radialMenuMode, onSelected))
			{
				return;
			}
			RadialMenu.ClearOptions();
			if (RadialMenu.CurrentMode == RadialMenuMode.Emote)
			{
				for (int i = 0; i < GameManager.EmoteSettings.EmoteSettings.Length; i++)
				{
					EmoteSettings emoteSettings = GameManager.EmoteSettings.EmoteSettings[i];
					RadialMenu.AddOption((emoteSettings.emote == Emote.VictoryDance) ? CosmeticsUnlocksManager.AllDances.GetDance(playerInfo.Cosmetics.victoryDance).icon : emoteSettings.icon, (int)emoteSettings.emote);
				}
			}
			else
			{
				for (int j = 0; j < GameManager.SpectatorEmoteSettings.EmoteSettings.Length; j++)
				{
					SpectatorEmoteSettings spectatorEmoteSettings = GameManager.SpectatorEmoteSettings.EmoteSettings[j];
					RadialMenu.AddOption(spectatorEmoteSettings.icon, (int)spectatorEmoteSettings.emote);
				}
			}
			RadialMenu.DistributeOptions();
		}
		bool CanOpenEmoteMenu()
		{
			if (Scoreboard.IsVisible)
			{
				return false;
			}
			if (!playerInfo.AsGolfer.IsInitialized)
			{
				return false;
			}
			if (!SingletonBehaviour<DrivingRangeManager>.HasInstance && (CourseManager.MatchState < MatchState.TeeOff || CourseManager.MatchState >= MatchState.Ended))
			{
				return false;
			}
			if (playerInfo.ActiveGolfCartSeat.IsValid())
			{
				return false;
			}
			if (playerInfo.Movement.IsRespawning)
			{
				return false;
			}
			if (playerInfo.Movement.IsKnockedOutOrRecovering)
			{
				return false;
			}
			if (playerInfo.AsHittable.IsFrozen)
			{
				return false;
			}
			return true;
		}
		void OnEmoteSelected(int emoteIndex)
		{
			playerInfo.SetEmoteToPlay((Emote)emoteIndex);
			if (RadialMenu.CurrentMode == RadialMenuMode.Emote)
			{
				RadialMenu.Hide();
			}
		}
		void OnSpectatorEmoteSelected(int emoteIndex)
		{
			playerInfo.AsSpectator.PlayEmote((SpectatorEmote)emoteIndex);
			if (RadialMenu.CurrentMode == RadialMenuMode.SpectatorEmote)
			{
				RadialMenu.Hide();
			}
		}
	}

	private bool GolfCartJump()
	{
		if (!playerInfo.ActiveGolfCartSeat.IsDriver())
		{
			return false;
		}
		return playerInfo.ActiveGolfCartSeat.golfCart.Movement.TryTriggerJump();
	}

	private void GolfCartHonk(InputAction.CallbackContext context)
	{
		if (!playerInfo.ActiveGolfCartSeat.IsValid() || !playerInfo.ActiveGolfCartSeat.golfCart.AsEntity.AsHittable.IsFrozen)
		{
			playerInfo.HonkGolfCart();
		}
	}

	private void ExitGolfCart(InputAction.CallbackContext context)
	{
		if (!playerInfo.ActiveGolfCartSeat.IsValid() || !playerInfo.ActiveGolfCartSeat.golfCart.AsEntity.AsHittable.IsFrozen)
		{
			playerInfo.ExitGolfCart(GolfCartExitType.Default);
		}
	}

	private void DiveOutOfGolfCart(InputAction.CallbackContext context)
	{
		if (!playerInfo.ActiveGolfCartSeat.IsValid() || !playerInfo.ActiveGolfCartSeat.golfCart.AsEntity.AsHittable.IsFrozen)
		{
			playerInfo.ExitGolfCart(GolfCartExitType.Dive);
		}
	}

	private void SelectHotkey1(InputAction.CallbackContext context)
	{
		hotkeyInputBuffer.Activate(TrySelect, TrySelect);
		static bool TrySelect()
		{
			return Hotkeys.Select(0);
		}
	}

	private void SelectHotkey2(InputAction.CallbackContext context)
	{
		hotkeyInputBuffer.Activate(TrySelect, TrySelect);
		static bool TrySelect()
		{
			return Hotkeys.Select(1);
		}
	}

	private void SelectHotkey3(InputAction.CallbackContext context)
	{
		hotkeyInputBuffer.Activate(TrySelect, TrySelect);
		static bool TrySelect()
		{
			return Hotkeys.Select(2);
		}
	}

	private void SelectHotkey4(InputAction.CallbackContext context)
	{
		hotkeyInputBuffer.Activate(TrySelect, TrySelect);
		static bool TrySelect()
		{
			return Hotkeys.Select(3);
		}
	}

	private void SelectHotkey5(InputAction.CallbackContext context)
	{
		hotkeyInputBuffer.Activate(TrySelect, TrySelect);
		static bool TrySelect()
		{
			return Hotkeys.Select(4);
		}
	}

	private void SelectHotkey6(InputAction.CallbackContext context)
	{
		hotkeyInputBuffer.Activate(TrySelect, TrySelect);
		static bool TrySelect()
		{
			return Hotkeys.Select(5);
		}
	}

	private void SelectHotkey7(InputAction.CallbackContext context)
	{
		hotkeyInputBuffer.Activate(TrySelect, TrySelect);
		static bool TrySelect()
		{
			return Hotkeys.Select(6);
		}
	}

	private void SelectHotkey8(InputAction.CallbackContext context)
	{
		hotkeyInputBuffer.Activate(TrySelect, TrySelect);
		static bool TrySelect()
		{
			return Hotkeys.Select(7);
		}
	}

	private void CycleHotkeysLeft(InputAction.CallbackContext context)
	{
		hotkeyInputBuffer.Activate(TryCycle, TryCycle);
		static bool TryCycle()
		{
			return Hotkeys.CycleLeft();
		}
	}

	private void CycleHotkeysRight(InputAction.CallbackContext context)
	{
		hotkeyInputBuffer.Activate(TryCycle, TryCycle);
		static bool TryCycle()
		{
			return Hotkeys.CycleRight();
		}
	}

	private void HotkeyToggleLastSelected(InputAction.CallbackContext context)
	{
		hotkeyInputBuffer.Activate(TryToggle, TryToggle);
		static bool TryToggle()
		{
			if (Hotkeys.CurrentMode != HotkeyMode.Inventory)
			{
				return false;
			}
			return Hotkeys.TogglePrevious();
		}
	}

	private void SpectatorCyclePreviousPlayer(InputAction.CallbackContext context)
	{
		if (CanCycleSpectatorTarget())
		{
			playerInfo.AsSpectator.CyclePreviousTarget(canBeginNewSpectate: false);
		}
	}

	private void SpectatorCycleNextPlayer(InputAction.CallbackContext context)
	{
		if (CanCycleSpectatorTarget())
		{
			playerInfo.AsSpectator.CycleNextTarget(canBeginNewSpectate: false);
		}
	}

	private bool CanCycleSpectatorTarget()
	{
		if (!SingletonBehaviour<DrivingRangeManager>.HasInstance && CourseManager.MatchState < MatchState.TeeOff)
		{
			return false;
		}
		if (RadialMenu.IsVisible)
		{
			return false;
		}
		if (RadialMenu.LastSelectionFrame == Time.frameCount)
		{
			return false;
		}
		return true;
	}

	private void ShowScoreboard(InputAction.CallbackContext context)
	{
		Scoreboard.Show();
	}

	private void HideScoreboard(InputAction.CallbackContext context)
	{
		Scoreboard.Hide();
	}

	private void Pause(InputAction.CallbackContext context)
	{
		if (RadialMenu.WasOpenThisOrLastFrame)
		{
			if (context.control.device is Keyboard)
			{
				return;
			}
			if (context.control.device is Gamepad)
			{
				RadialMenu.Hide();
			}
		}
		PauseMenu.Pause();
	}

	private void StartHoldingPushToTalk(InputAction.CallbackContext context)
	{
		switch (GameSettings.All.Audio.MicInputMode)
		{
		case GameSettings.AudioSettings.InputMode.PushToTalk:
			IsPushingToTalk = true;
			break;
		case GameSettings.AudioSettings.InputMode.PushToToggle:
			IsPushingToTalk = !IsPushingToTalk;
			break;
		}
	}

	private void StopHoldingPushToTalk(InputAction.CallbackContext context)
	{
		if (GameSettings.All.Audio.MicInputMode == GameSettings.AudioSettings.InputMode.PushToTalk)
		{
			IsPushingToTalk = false;
		}
	}

	private void OpenChat(InputAction.CallbackContext context)
	{
		TextChatUi.SetEnabled(enabled: true);
	}

	private void SelectRadialMenuOption(InputAction.CallbackContext context)
	{
		RadialMenu.Select();
	}

	private void CancelRadialMenu(InputAction.CallbackContext context)
	{
		RadialMenu.Cancel();
	}

	private void ExitUi(InputAction.CallbackContext context)
	{
		if (Time.frameCount != lastExitFrame)
		{
			lastExitFrame = Time.frameCount;
			MenuNavigation.SendExitEvent();
		}
	}

	private void OnSwitchedToGamepad()
	{
		gamepadPitchInputDirection = 0;
		gamepadPitchInputRepeatTimestamp = double.MinValue;
		PlayerInput.SwitchedToGamepad?.Invoke();
	}

	private void OnSwitchedToKeyboard()
	{
		PlayerInput.SwitchedToKeyboard?.Invoke();
	}

	private void OnSwitchedInput()
	{
		PlayerInput.SwitchedInputDeviceType?.Invoke();
	}

	private void OnHotkeyModeChanged()
	{
		if (hotkeyInputBuffer != null)
		{
			hotkeyInputBuffer.Cancel();
		}
	}

	private void OnMicInputModeChanged()
	{
		if (GameSettings.All.Audio.MicInputMode == GameSettings.AudioSettings.InputMode.PushToTalk)
		{
			IsPushingToTalk = Controls.VoiceChat.PushToTalk.IsPressed();
		}
		else
		{
			IsPushingToTalk = false;
		}
	}

	public override bool Weaved()
	{
		return true;
	}
}
