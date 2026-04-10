using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Utilities;

public class GameControls : IInputActionCollection2, IInputActionCollection, IEnumerable<InputAction>, IEnumerable, IDisposable
{
	public struct GameplayActions
	{
		private GameControls m_Wrapper;

		public InputAction Move => m_Wrapper.m_Gameplay_Move;

		public InputAction Jump => m_Wrapper.m_Gameplay_Jump;

		public InputAction Aim => m_Wrapper.m_Gameplay_Aim;

		public InputAction Swing => m_Wrapper.m_Gameplay_Swing;

		public InputAction UseItem => m_Wrapper.m_Gameplay_UseItem;

		public InputAction Pitch => m_Wrapper.m_Gameplay_Pitch;

		public InputAction Interact => m_Wrapper.m_Gameplay_Interact;

		public InputAction Cancel => m_Wrapper.m_Gameplay_Cancel;

		public InputAction DropItem => m_Wrapper.m_Gameplay_DropItem;

		public InputAction Dive => m_Wrapper.m_Gameplay_Dive;

		public InputAction Walk => m_Wrapper.m_Gameplay_Walk;

		public InputAction CycleSwingAngle => m_Wrapper.m_Gameplay_CycleSwingAngle;

		public InputAction Restart => m_Wrapper.m_Gameplay_Restart;

		public bool enabled => Get().enabled;

		public GameplayActions(GameControls wrapper)
		{
			m_Wrapper = wrapper;
		}

		public InputActionMap Get()
		{
			return m_Wrapper.m_Gameplay;
		}

		public void Enable()
		{
			Get().Enable();
		}

		public void Disable()
		{
			Get().Disable();
		}

		public static implicit operator InputActionMap(GameplayActions set)
		{
			return set.Get();
		}

		public void AddCallbacks(IGameplayActions instance)
		{
			if (instance != null && !m_Wrapper.m_GameplayActionsCallbackInterfaces.Contains(instance))
			{
				m_Wrapper.m_GameplayActionsCallbackInterfaces.Add(instance);
				Move.started += instance.OnMove;
				Move.performed += instance.OnMove;
				Move.canceled += instance.OnMove;
				Jump.started += instance.OnJump;
				Jump.performed += instance.OnJump;
				Jump.canceled += instance.OnJump;
				Aim.started += instance.OnAim;
				Aim.performed += instance.OnAim;
				Aim.canceled += instance.OnAim;
				Swing.started += instance.OnSwing;
				Swing.performed += instance.OnSwing;
				Swing.canceled += instance.OnSwing;
				UseItem.started += instance.OnUseItem;
				UseItem.performed += instance.OnUseItem;
				UseItem.canceled += instance.OnUseItem;
				Pitch.started += instance.OnPitch;
				Pitch.performed += instance.OnPitch;
				Pitch.canceled += instance.OnPitch;
				Interact.started += instance.OnInteract;
				Interact.performed += instance.OnInteract;
				Interact.canceled += instance.OnInteract;
				Cancel.started += instance.OnCancel;
				Cancel.performed += instance.OnCancel;
				Cancel.canceled += instance.OnCancel;
				DropItem.started += instance.OnDropItem;
				DropItem.performed += instance.OnDropItem;
				DropItem.canceled += instance.OnDropItem;
				Dive.started += instance.OnDive;
				Dive.performed += instance.OnDive;
				Dive.canceled += instance.OnDive;
				Walk.started += instance.OnWalk;
				Walk.performed += instance.OnWalk;
				Walk.canceled += instance.OnWalk;
				CycleSwingAngle.started += instance.OnCycleSwingAngle;
				CycleSwingAngle.performed += instance.OnCycleSwingAngle;
				CycleSwingAngle.canceled += instance.OnCycleSwingAngle;
				Restart.started += instance.OnRestart;
				Restart.performed += instance.OnRestart;
				Restart.canceled += instance.OnRestart;
			}
		}

		private void UnregisterCallbacks(IGameplayActions instance)
		{
			Move.started -= instance.OnMove;
			Move.performed -= instance.OnMove;
			Move.canceled -= instance.OnMove;
			Jump.started -= instance.OnJump;
			Jump.performed -= instance.OnJump;
			Jump.canceled -= instance.OnJump;
			Aim.started -= instance.OnAim;
			Aim.performed -= instance.OnAim;
			Aim.canceled -= instance.OnAim;
			Swing.started -= instance.OnSwing;
			Swing.performed -= instance.OnSwing;
			Swing.canceled -= instance.OnSwing;
			UseItem.started -= instance.OnUseItem;
			UseItem.performed -= instance.OnUseItem;
			UseItem.canceled -= instance.OnUseItem;
			Pitch.started -= instance.OnPitch;
			Pitch.performed -= instance.OnPitch;
			Pitch.canceled -= instance.OnPitch;
			Interact.started -= instance.OnInteract;
			Interact.performed -= instance.OnInteract;
			Interact.canceled -= instance.OnInteract;
			Cancel.started -= instance.OnCancel;
			Cancel.performed -= instance.OnCancel;
			Cancel.canceled -= instance.OnCancel;
			DropItem.started -= instance.OnDropItem;
			DropItem.performed -= instance.OnDropItem;
			DropItem.canceled -= instance.OnDropItem;
			Dive.started -= instance.OnDive;
			Dive.performed -= instance.OnDive;
			Dive.canceled -= instance.OnDive;
			Walk.started -= instance.OnWalk;
			Walk.performed -= instance.OnWalk;
			Walk.canceled -= instance.OnWalk;
			CycleSwingAngle.started -= instance.OnCycleSwingAngle;
			CycleSwingAngle.performed -= instance.OnCycleSwingAngle;
			CycleSwingAngle.canceled -= instance.OnCycleSwingAngle;
			Restart.started -= instance.OnRestart;
			Restart.performed -= instance.OnRestart;
			Restart.canceled -= instance.OnRestart;
		}

		public void RemoveCallbacks(IGameplayActions instance)
		{
			if (m_Wrapper.m_GameplayActionsCallbackInterfaces.Remove(instance))
			{
				UnregisterCallbacks(instance);
			}
		}

		public void SetCallbacks(IGameplayActions instance)
		{
			foreach (IGameplayActions gameplayActionsCallbackInterface in m_Wrapper.m_GameplayActionsCallbackInterfaces)
			{
				UnregisterCallbacks(gameplayActionsCallbackInterface);
			}
			m_Wrapper.m_GameplayActionsCallbackInterfaces.Clear();
			AddCallbacks(instance);
		}
	}

	public struct GolfCartDriverActions
	{
		private GameControls m_Wrapper;

		public InputAction Accelerate => m_Wrapper.m_GolfCartDriver_Accelerate;

		public InputAction Brake => m_Wrapper.m_GolfCartDriver_Brake;

		public InputAction Steer => m_Wrapper.m_GolfCartDriver_Steer;

		public InputAction Jump => m_Wrapper.m_GolfCartDriver_Jump;

		public InputAction Honk => m_Wrapper.m_GolfCartDriver_Honk;

		public bool enabled => Get().enabled;

		public GolfCartDriverActions(GameControls wrapper)
		{
			m_Wrapper = wrapper;
		}

		public InputActionMap Get()
		{
			return m_Wrapper.m_GolfCartDriver;
		}

		public void Enable()
		{
			Get().Enable();
		}

		public void Disable()
		{
			Get().Disable();
		}

		public static implicit operator InputActionMap(GolfCartDriverActions set)
		{
			return set.Get();
		}

		public void AddCallbacks(IGolfCartDriverActions instance)
		{
			if (instance != null && !m_Wrapper.m_GolfCartDriverActionsCallbackInterfaces.Contains(instance))
			{
				m_Wrapper.m_GolfCartDriverActionsCallbackInterfaces.Add(instance);
				Accelerate.started += instance.OnAccelerate;
				Accelerate.performed += instance.OnAccelerate;
				Accelerate.canceled += instance.OnAccelerate;
				Brake.started += instance.OnBrake;
				Brake.performed += instance.OnBrake;
				Brake.canceled += instance.OnBrake;
				Steer.started += instance.OnSteer;
				Steer.performed += instance.OnSteer;
				Steer.canceled += instance.OnSteer;
				Jump.started += instance.OnJump;
				Jump.performed += instance.OnJump;
				Jump.canceled += instance.OnJump;
				Honk.started += instance.OnHonk;
				Honk.performed += instance.OnHonk;
				Honk.canceled += instance.OnHonk;
			}
		}

		private void UnregisterCallbacks(IGolfCartDriverActions instance)
		{
			Accelerate.started -= instance.OnAccelerate;
			Accelerate.performed -= instance.OnAccelerate;
			Accelerate.canceled -= instance.OnAccelerate;
			Brake.started -= instance.OnBrake;
			Brake.performed -= instance.OnBrake;
			Brake.canceled -= instance.OnBrake;
			Steer.started -= instance.OnSteer;
			Steer.performed -= instance.OnSteer;
			Steer.canceled -= instance.OnSteer;
			Jump.started -= instance.OnJump;
			Jump.performed -= instance.OnJump;
			Jump.canceled -= instance.OnJump;
			Honk.started -= instance.OnHonk;
			Honk.performed -= instance.OnHonk;
			Honk.canceled -= instance.OnHonk;
		}

		public void RemoveCallbacks(IGolfCartDriverActions instance)
		{
			if (m_Wrapper.m_GolfCartDriverActionsCallbackInterfaces.Remove(instance))
			{
				UnregisterCallbacks(instance);
			}
		}

		public void SetCallbacks(IGolfCartDriverActions instance)
		{
			foreach (IGolfCartDriverActions golfCartDriverActionsCallbackInterface in m_Wrapper.m_GolfCartDriverActionsCallbackInterfaces)
			{
				UnregisterCallbacks(golfCartDriverActionsCallbackInterface);
			}
			m_Wrapper.m_GolfCartDriverActionsCallbackInterfaces.Clear();
			AddCallbacks(instance);
		}
	}

	public struct GolfCartSharedActions
	{
		private GameControls m_Wrapper;

		public InputAction Exit => m_Wrapper.m_GolfCartShared_Exit;

		public InputAction DiveOut => m_Wrapper.m_GolfCartShared_DiveOut;

		public bool enabled => Get().enabled;

		public GolfCartSharedActions(GameControls wrapper)
		{
			m_Wrapper = wrapper;
		}

		public InputActionMap Get()
		{
			return m_Wrapper.m_GolfCartShared;
		}

		public void Enable()
		{
			Get().Enable();
		}

		public void Disable()
		{
			Get().Disable();
		}

		public static implicit operator InputActionMap(GolfCartSharedActions set)
		{
			return set.Get();
		}

		public void AddCallbacks(IGolfCartSharedActions instance)
		{
			if (instance != null && !m_Wrapper.m_GolfCartSharedActionsCallbackInterfaces.Contains(instance))
			{
				m_Wrapper.m_GolfCartSharedActionsCallbackInterfaces.Add(instance);
				Exit.started += instance.OnExit;
				Exit.performed += instance.OnExit;
				Exit.canceled += instance.OnExit;
				DiveOut.started += instance.OnDiveOut;
				DiveOut.performed += instance.OnDiveOut;
				DiveOut.canceled += instance.OnDiveOut;
			}
		}

		private void UnregisterCallbacks(IGolfCartSharedActions instance)
		{
			Exit.started -= instance.OnExit;
			Exit.performed -= instance.OnExit;
			Exit.canceled -= instance.OnExit;
			DiveOut.started -= instance.OnDiveOut;
			DiveOut.performed -= instance.OnDiveOut;
			DiveOut.canceled -= instance.OnDiveOut;
		}

		public void RemoveCallbacks(IGolfCartSharedActions instance)
		{
			if (m_Wrapper.m_GolfCartSharedActionsCallbackInterfaces.Remove(instance))
			{
				UnregisterCallbacks(instance);
			}
		}

		public void SetCallbacks(IGolfCartSharedActions instance)
		{
			foreach (IGolfCartSharedActions golfCartSharedActionsCallbackInterface in m_Wrapper.m_GolfCartSharedActionsCallbackInterfaces)
			{
				UnregisterCallbacks(golfCartSharedActionsCallbackInterface);
			}
			m_Wrapper.m_GolfCartSharedActionsCallbackInterfaces.Clear();
			AddCallbacks(instance);
		}
	}

	public struct HotkeysActions
	{
		private GameControls m_Wrapper;

		public InputAction Hotkey1 => m_Wrapper.m_Hotkeys_Hotkey1;

		public InputAction Hotkey2 => m_Wrapper.m_Hotkeys_Hotkey2;

		public InputAction Hotkey3 => m_Wrapper.m_Hotkeys_Hotkey3;

		public InputAction Hotkey4 => m_Wrapper.m_Hotkeys_Hotkey4;

		public InputAction Hotkey5 => m_Wrapper.m_Hotkeys_Hotkey5;

		public InputAction Hotkey6 => m_Wrapper.m_Hotkeys_Hotkey6;

		public InputAction Hotkey7 => m_Wrapper.m_Hotkeys_Hotkey7;

		public InputAction Hotkey8 => m_Wrapper.m_Hotkeys_Hotkey8;

		public InputAction CycleLeft => m_Wrapper.m_Hotkeys_CycleLeft;

		public InputAction CycleRight => m_Wrapper.m_Hotkeys_CycleRight;

		public InputAction Toggle => m_Wrapper.m_Hotkeys_Toggle;

		public bool enabled => Get().enabled;

		public HotkeysActions(GameControls wrapper)
		{
			m_Wrapper = wrapper;
		}

		public InputActionMap Get()
		{
			return m_Wrapper.m_Hotkeys;
		}

		public void Enable()
		{
			Get().Enable();
		}

		public void Disable()
		{
			Get().Disable();
		}

		public static implicit operator InputActionMap(HotkeysActions set)
		{
			return set.Get();
		}

		public void AddCallbacks(IHotkeysActions instance)
		{
			if (instance != null && !m_Wrapper.m_HotkeysActionsCallbackInterfaces.Contains(instance))
			{
				m_Wrapper.m_HotkeysActionsCallbackInterfaces.Add(instance);
				Hotkey1.started += instance.OnHotkey1;
				Hotkey1.performed += instance.OnHotkey1;
				Hotkey1.canceled += instance.OnHotkey1;
				Hotkey2.started += instance.OnHotkey2;
				Hotkey2.performed += instance.OnHotkey2;
				Hotkey2.canceled += instance.OnHotkey2;
				Hotkey3.started += instance.OnHotkey3;
				Hotkey3.performed += instance.OnHotkey3;
				Hotkey3.canceled += instance.OnHotkey3;
				Hotkey4.started += instance.OnHotkey4;
				Hotkey4.performed += instance.OnHotkey4;
				Hotkey4.canceled += instance.OnHotkey4;
				Hotkey5.started += instance.OnHotkey5;
				Hotkey5.performed += instance.OnHotkey5;
				Hotkey5.canceled += instance.OnHotkey5;
				Hotkey6.started += instance.OnHotkey6;
				Hotkey6.performed += instance.OnHotkey6;
				Hotkey6.canceled += instance.OnHotkey6;
				Hotkey7.started += instance.OnHotkey7;
				Hotkey7.performed += instance.OnHotkey7;
				Hotkey7.canceled += instance.OnHotkey7;
				Hotkey8.started += instance.OnHotkey8;
				Hotkey8.performed += instance.OnHotkey8;
				Hotkey8.canceled += instance.OnHotkey8;
				CycleLeft.started += instance.OnCycleLeft;
				CycleLeft.performed += instance.OnCycleLeft;
				CycleLeft.canceled += instance.OnCycleLeft;
				CycleRight.started += instance.OnCycleRight;
				CycleRight.performed += instance.OnCycleRight;
				CycleRight.canceled += instance.OnCycleRight;
				Toggle.started += instance.OnToggle;
				Toggle.performed += instance.OnToggle;
				Toggle.canceled += instance.OnToggle;
			}
		}

		private void UnregisterCallbacks(IHotkeysActions instance)
		{
			Hotkey1.started -= instance.OnHotkey1;
			Hotkey1.performed -= instance.OnHotkey1;
			Hotkey1.canceled -= instance.OnHotkey1;
			Hotkey2.started -= instance.OnHotkey2;
			Hotkey2.performed -= instance.OnHotkey2;
			Hotkey2.canceled -= instance.OnHotkey2;
			Hotkey3.started -= instance.OnHotkey3;
			Hotkey3.performed -= instance.OnHotkey3;
			Hotkey3.canceled -= instance.OnHotkey3;
			Hotkey4.started -= instance.OnHotkey4;
			Hotkey4.performed -= instance.OnHotkey4;
			Hotkey4.canceled -= instance.OnHotkey4;
			Hotkey5.started -= instance.OnHotkey5;
			Hotkey5.performed -= instance.OnHotkey5;
			Hotkey5.canceled -= instance.OnHotkey5;
			Hotkey6.started -= instance.OnHotkey6;
			Hotkey6.performed -= instance.OnHotkey6;
			Hotkey6.canceled -= instance.OnHotkey6;
			Hotkey7.started -= instance.OnHotkey7;
			Hotkey7.performed -= instance.OnHotkey7;
			Hotkey7.canceled -= instance.OnHotkey7;
			Hotkey8.started -= instance.OnHotkey8;
			Hotkey8.performed -= instance.OnHotkey8;
			Hotkey8.canceled -= instance.OnHotkey8;
			CycleLeft.started -= instance.OnCycleLeft;
			CycleLeft.performed -= instance.OnCycleLeft;
			CycleLeft.canceled -= instance.OnCycleLeft;
			CycleRight.started -= instance.OnCycleRight;
			CycleRight.performed -= instance.OnCycleRight;
			CycleRight.canceled -= instance.OnCycleRight;
			Toggle.started -= instance.OnToggle;
			Toggle.performed -= instance.OnToggle;
			Toggle.canceled -= instance.OnToggle;
		}

		public void RemoveCallbacks(IHotkeysActions instance)
		{
			if (m_Wrapper.m_HotkeysActionsCallbackInterfaces.Remove(instance))
			{
				UnregisterCallbacks(instance);
			}
		}

		public void SetCallbacks(IHotkeysActions instance)
		{
			foreach (IHotkeysActions hotkeysActionsCallbackInterface in m_Wrapper.m_HotkeysActionsCallbackInterfaces)
			{
				UnregisterCallbacks(hotkeysActionsCallbackInterface);
			}
			m_Wrapper.m_HotkeysActionsCallbackInterfaces.Clear();
			AddCallbacks(instance);
		}
	}

	public struct SpectateActions
	{
		private GameControls m_Wrapper;

		public InputAction CyclePreviousPlayer => m_Wrapper.m_Spectate_CyclePreviousPlayer;

		public InputAction CycleNextPlayer => m_Wrapper.m_Spectate_CycleNextPlayer;

		public bool enabled => Get().enabled;

		public SpectateActions(GameControls wrapper)
		{
			m_Wrapper = wrapper;
		}

		public InputActionMap Get()
		{
			return m_Wrapper.m_Spectate;
		}

		public void Enable()
		{
			Get().Enable();
		}

		public void Disable()
		{
			Get().Disable();
		}

		public static implicit operator InputActionMap(SpectateActions set)
		{
			return set.Get();
		}

		public void AddCallbacks(ISpectateActions instance)
		{
			if (instance != null && !m_Wrapper.m_SpectateActionsCallbackInterfaces.Contains(instance))
			{
				m_Wrapper.m_SpectateActionsCallbackInterfaces.Add(instance);
				CyclePreviousPlayer.started += instance.OnCyclePreviousPlayer;
				CyclePreviousPlayer.performed += instance.OnCyclePreviousPlayer;
				CyclePreviousPlayer.canceled += instance.OnCyclePreviousPlayer;
				CycleNextPlayer.started += instance.OnCycleNextPlayer;
				CycleNextPlayer.performed += instance.OnCycleNextPlayer;
				CycleNextPlayer.canceled += instance.OnCycleNextPlayer;
			}
		}

		private void UnregisterCallbacks(ISpectateActions instance)
		{
			CyclePreviousPlayer.started -= instance.OnCyclePreviousPlayer;
			CyclePreviousPlayer.performed -= instance.OnCyclePreviousPlayer;
			CyclePreviousPlayer.canceled -= instance.OnCyclePreviousPlayer;
			CycleNextPlayer.started -= instance.OnCycleNextPlayer;
			CycleNextPlayer.performed -= instance.OnCycleNextPlayer;
			CycleNextPlayer.canceled -= instance.OnCycleNextPlayer;
		}

		public void RemoveCallbacks(ISpectateActions instance)
		{
			if (m_Wrapper.m_SpectateActionsCallbackInterfaces.Remove(instance))
			{
				UnregisterCallbacks(instance);
			}
		}

		public void SetCallbacks(ISpectateActions instance)
		{
			foreach (ISpectateActions spectateActionsCallbackInterface in m_Wrapper.m_SpectateActionsCallbackInterfaces)
			{
				UnregisterCallbacks(spectateActionsCallbackInterface);
			}
			m_Wrapper.m_SpectateActionsCallbackInterfaces.Clear();
			AddCallbacks(instance);
		}
	}

	public struct IngameActions
	{
		private GameControls m_Wrapper;

		public InputAction ShowScoreboard => m_Wrapper.m_Ingame_ShowScoreboard;

		public InputAction Pause => m_Wrapper.m_Ingame_Pause;

		public InputAction OpenChat => m_Wrapper.m_Ingame_OpenChat;

		public InputAction ToggleEmoteMenu => m_Wrapper.m_Ingame_ToggleEmoteMenu;

		public bool enabled => Get().enabled;

		public IngameActions(GameControls wrapper)
		{
			m_Wrapper = wrapper;
		}

		public InputActionMap Get()
		{
			return m_Wrapper.m_Ingame;
		}

		public void Enable()
		{
			Get().Enable();
		}

		public void Disable()
		{
			Get().Disable();
		}

		public static implicit operator InputActionMap(IngameActions set)
		{
			return set.Get();
		}

		public void AddCallbacks(IIngameActions instance)
		{
			if (instance != null && !m_Wrapper.m_IngameActionsCallbackInterfaces.Contains(instance))
			{
				m_Wrapper.m_IngameActionsCallbackInterfaces.Add(instance);
				ShowScoreboard.started += instance.OnShowScoreboard;
				ShowScoreboard.performed += instance.OnShowScoreboard;
				ShowScoreboard.canceled += instance.OnShowScoreboard;
				Pause.started += instance.OnPause;
				Pause.performed += instance.OnPause;
				Pause.canceled += instance.OnPause;
				OpenChat.started += instance.OnOpenChat;
				OpenChat.performed += instance.OnOpenChat;
				OpenChat.canceled += instance.OnOpenChat;
				ToggleEmoteMenu.started += instance.OnToggleEmoteMenu;
				ToggleEmoteMenu.performed += instance.OnToggleEmoteMenu;
				ToggleEmoteMenu.canceled += instance.OnToggleEmoteMenu;
			}
		}

		private void UnregisterCallbacks(IIngameActions instance)
		{
			ShowScoreboard.started -= instance.OnShowScoreboard;
			ShowScoreboard.performed -= instance.OnShowScoreboard;
			ShowScoreboard.canceled -= instance.OnShowScoreboard;
			Pause.started -= instance.OnPause;
			Pause.performed -= instance.OnPause;
			Pause.canceled -= instance.OnPause;
			OpenChat.started -= instance.OnOpenChat;
			OpenChat.performed -= instance.OnOpenChat;
			OpenChat.canceled -= instance.OnOpenChat;
			ToggleEmoteMenu.started -= instance.OnToggleEmoteMenu;
			ToggleEmoteMenu.performed -= instance.OnToggleEmoteMenu;
			ToggleEmoteMenu.canceled -= instance.OnToggleEmoteMenu;
		}

		public void RemoveCallbacks(IIngameActions instance)
		{
			if (m_Wrapper.m_IngameActionsCallbackInterfaces.Remove(instance))
			{
				UnregisterCallbacks(instance);
			}
		}

		public void SetCallbacks(IIngameActions instance)
		{
			foreach (IIngameActions ingameActionsCallbackInterface in m_Wrapper.m_IngameActionsCallbackInterfaces)
			{
				UnregisterCallbacks(ingameActionsCallbackInterface);
			}
			m_Wrapper.m_IngameActionsCallbackInterfaces.Clear();
			AddCallbacks(instance);
		}
	}

	public struct VoiceChatActions
	{
		private GameControls m_Wrapper;

		public InputAction PushToTalk => m_Wrapper.m_VoiceChat_PushToTalk;

		public bool enabled => Get().enabled;

		public VoiceChatActions(GameControls wrapper)
		{
			m_Wrapper = wrapper;
		}

		public InputActionMap Get()
		{
			return m_Wrapper.m_VoiceChat;
		}

		public void Enable()
		{
			Get().Enable();
		}

		public void Disable()
		{
			Get().Disable();
		}

		public static implicit operator InputActionMap(VoiceChatActions set)
		{
			return set.Get();
		}

		public void AddCallbacks(IVoiceChatActions instance)
		{
			if (instance != null && !m_Wrapper.m_VoiceChatActionsCallbackInterfaces.Contains(instance))
			{
				m_Wrapper.m_VoiceChatActionsCallbackInterfaces.Add(instance);
				PushToTalk.started += instance.OnPushToTalk;
				PushToTalk.performed += instance.OnPushToTalk;
				PushToTalk.canceled += instance.OnPushToTalk;
			}
		}

		private void UnregisterCallbacks(IVoiceChatActions instance)
		{
			PushToTalk.started -= instance.OnPushToTalk;
			PushToTalk.performed -= instance.OnPushToTalk;
			PushToTalk.canceled -= instance.OnPushToTalk;
		}

		public void RemoveCallbacks(IVoiceChatActions instance)
		{
			if (m_Wrapper.m_VoiceChatActionsCallbackInterfaces.Remove(instance))
			{
				UnregisterCallbacks(instance);
			}
		}

		public void SetCallbacks(IVoiceChatActions instance)
		{
			foreach (IVoiceChatActions voiceChatActionsCallbackInterface in m_Wrapper.m_VoiceChatActionsCallbackInterfaces)
			{
				UnregisterCallbacks(voiceChatActionsCallbackInterface);
			}
			m_Wrapper.m_VoiceChatActionsCallbackInterfaces.Clear();
			AddCallbacks(instance);
		}
	}

	public struct CameraActions
	{
		private GameControls m_Wrapper;

		public InputAction Look => m_Wrapper.m_Camera_Look;

		public bool enabled => Get().enabled;

		public CameraActions(GameControls wrapper)
		{
			m_Wrapper = wrapper;
		}

		public InputActionMap Get()
		{
			return m_Wrapper.m_Camera;
		}

		public void Enable()
		{
			Get().Enable();
		}

		public void Disable()
		{
			Get().Disable();
		}

		public static implicit operator InputActionMap(CameraActions set)
		{
			return set.Get();
		}

		public void AddCallbacks(ICameraActions instance)
		{
			if (instance != null && !m_Wrapper.m_CameraActionsCallbackInterfaces.Contains(instance))
			{
				m_Wrapper.m_CameraActionsCallbackInterfaces.Add(instance);
				Look.started += instance.OnLook;
				Look.performed += instance.OnLook;
				Look.canceled += instance.OnLook;
			}
		}

		private void UnregisterCallbacks(ICameraActions instance)
		{
			Look.started -= instance.OnLook;
			Look.performed -= instance.OnLook;
			Look.canceled -= instance.OnLook;
		}

		public void RemoveCallbacks(ICameraActions instance)
		{
			if (m_Wrapper.m_CameraActionsCallbackInterfaces.Remove(instance))
			{
				UnregisterCallbacks(instance);
			}
		}

		public void SetCallbacks(ICameraActions instance)
		{
			foreach (ICameraActions cameraActionsCallbackInterface in m_Wrapper.m_CameraActionsCallbackInterfaces)
			{
				UnregisterCallbacks(cameraActionsCallbackInterface);
			}
			m_Wrapper.m_CameraActionsCallbackInterfaces.Clear();
			AddCallbacks(instance);
		}
	}

	public struct RadialMenuActions
	{
		private GameControls m_Wrapper;

		public InputAction Select => m_Wrapper.m_RadialMenu_Select;

		public InputAction Cancel => m_Wrapper.m_RadialMenu_Cancel;

		public bool enabled => Get().enabled;

		public RadialMenuActions(GameControls wrapper)
		{
			m_Wrapper = wrapper;
		}

		public InputActionMap Get()
		{
			return m_Wrapper.m_RadialMenu;
		}

		public void Enable()
		{
			Get().Enable();
		}

		public void Disable()
		{
			Get().Disable();
		}

		public static implicit operator InputActionMap(RadialMenuActions set)
		{
			return set.Get();
		}

		public void AddCallbacks(IRadialMenuActions instance)
		{
			if (instance != null && !m_Wrapper.m_RadialMenuActionsCallbackInterfaces.Contains(instance))
			{
				m_Wrapper.m_RadialMenuActionsCallbackInterfaces.Add(instance);
				Select.started += instance.OnSelect;
				Select.performed += instance.OnSelect;
				Select.canceled += instance.OnSelect;
				Cancel.started += instance.OnCancel;
				Cancel.performed += instance.OnCancel;
				Cancel.canceled += instance.OnCancel;
			}
		}

		private void UnregisterCallbacks(IRadialMenuActions instance)
		{
			Select.started -= instance.OnSelect;
			Select.performed -= instance.OnSelect;
			Select.canceled -= instance.OnSelect;
			Cancel.started -= instance.OnCancel;
			Cancel.performed -= instance.OnCancel;
			Cancel.canceled -= instance.OnCancel;
		}

		public void RemoveCallbacks(IRadialMenuActions instance)
		{
			if (m_Wrapper.m_RadialMenuActionsCallbackInterfaces.Remove(instance))
			{
				UnregisterCallbacks(instance);
			}
		}

		public void SetCallbacks(IRadialMenuActions instance)
		{
			foreach (IRadialMenuActions radialMenuActionsCallbackInterface in m_Wrapper.m_RadialMenuActionsCallbackInterfaces)
			{
				UnregisterCallbacks(radialMenuActionsCallbackInterface);
			}
			m_Wrapper.m_RadialMenuActionsCallbackInterfaces.Clear();
			AddCallbacks(instance);
		}
	}

	public struct UIActions
	{
		private GameControls m_Wrapper;

		public InputAction Navigate => m_Wrapper.m_UI_Navigate;

		public InputAction Submit => m_Wrapper.m_UI_Submit;

		public InputAction Cancel => m_Wrapper.m_UI_Cancel;

		public InputAction Point => m_Wrapper.m_UI_Point;

		public InputAction Click => m_Wrapper.m_UI_Click;

		public InputAction ScrollWheel => m_Wrapper.m_UI_ScrollWheel;

		public InputAction MiddleClick => m_Wrapper.m_UI_MiddleClick;

		public InputAction RightClick => m_Wrapper.m_UI_RightClick;

		public InputAction Exit => m_Wrapper.m_UI_Exit;

		public InputAction VirtualCursorMove => m_Wrapper.m_UI_VirtualCursorMove;

		public InputAction VirtualCursorClick => m_Wrapper.m_UI_VirtualCursorClick;

		public InputAction VirtualCursorScrollWheel => m_Wrapper.m_UI_VirtualCursorScrollWheel;

		public bool enabled => Get().enabled;

		public UIActions(GameControls wrapper)
		{
			m_Wrapper = wrapper;
		}

		public InputActionMap Get()
		{
			return m_Wrapper.m_UI;
		}

		public void Enable()
		{
			Get().Enable();
		}

		public void Disable()
		{
			Get().Disable();
		}

		public static implicit operator InputActionMap(UIActions set)
		{
			return set.Get();
		}

		public void AddCallbacks(IUIActions instance)
		{
			if (instance != null && !m_Wrapper.m_UIActionsCallbackInterfaces.Contains(instance))
			{
				m_Wrapper.m_UIActionsCallbackInterfaces.Add(instance);
				Navigate.started += instance.OnNavigate;
				Navigate.performed += instance.OnNavigate;
				Navigate.canceled += instance.OnNavigate;
				Submit.started += instance.OnSubmit;
				Submit.performed += instance.OnSubmit;
				Submit.canceled += instance.OnSubmit;
				Cancel.started += instance.OnCancel;
				Cancel.performed += instance.OnCancel;
				Cancel.canceled += instance.OnCancel;
				Point.started += instance.OnPoint;
				Point.performed += instance.OnPoint;
				Point.canceled += instance.OnPoint;
				Click.started += instance.OnClick;
				Click.performed += instance.OnClick;
				Click.canceled += instance.OnClick;
				ScrollWheel.started += instance.OnScrollWheel;
				ScrollWheel.performed += instance.OnScrollWheel;
				ScrollWheel.canceled += instance.OnScrollWheel;
				MiddleClick.started += instance.OnMiddleClick;
				MiddleClick.performed += instance.OnMiddleClick;
				MiddleClick.canceled += instance.OnMiddleClick;
				RightClick.started += instance.OnRightClick;
				RightClick.performed += instance.OnRightClick;
				RightClick.canceled += instance.OnRightClick;
				Exit.started += instance.OnExit;
				Exit.performed += instance.OnExit;
				Exit.canceled += instance.OnExit;
				VirtualCursorMove.started += instance.OnVirtualCursorMove;
				VirtualCursorMove.performed += instance.OnVirtualCursorMove;
				VirtualCursorMove.canceled += instance.OnVirtualCursorMove;
				VirtualCursorClick.started += instance.OnVirtualCursorClick;
				VirtualCursorClick.performed += instance.OnVirtualCursorClick;
				VirtualCursorClick.canceled += instance.OnVirtualCursorClick;
				VirtualCursorScrollWheel.started += instance.OnVirtualCursorScrollWheel;
				VirtualCursorScrollWheel.performed += instance.OnVirtualCursorScrollWheel;
				VirtualCursorScrollWheel.canceled += instance.OnVirtualCursorScrollWheel;
			}
		}

		private void UnregisterCallbacks(IUIActions instance)
		{
			Navigate.started -= instance.OnNavigate;
			Navigate.performed -= instance.OnNavigate;
			Navigate.canceled -= instance.OnNavigate;
			Submit.started -= instance.OnSubmit;
			Submit.performed -= instance.OnSubmit;
			Submit.canceled -= instance.OnSubmit;
			Cancel.started -= instance.OnCancel;
			Cancel.performed -= instance.OnCancel;
			Cancel.canceled -= instance.OnCancel;
			Point.started -= instance.OnPoint;
			Point.performed -= instance.OnPoint;
			Point.canceled -= instance.OnPoint;
			Click.started -= instance.OnClick;
			Click.performed -= instance.OnClick;
			Click.canceled -= instance.OnClick;
			ScrollWheel.started -= instance.OnScrollWheel;
			ScrollWheel.performed -= instance.OnScrollWheel;
			ScrollWheel.canceled -= instance.OnScrollWheel;
			MiddleClick.started -= instance.OnMiddleClick;
			MiddleClick.performed -= instance.OnMiddleClick;
			MiddleClick.canceled -= instance.OnMiddleClick;
			RightClick.started -= instance.OnRightClick;
			RightClick.performed -= instance.OnRightClick;
			RightClick.canceled -= instance.OnRightClick;
			Exit.started -= instance.OnExit;
			Exit.performed -= instance.OnExit;
			Exit.canceled -= instance.OnExit;
			VirtualCursorMove.started -= instance.OnVirtualCursorMove;
			VirtualCursorMove.performed -= instance.OnVirtualCursorMove;
			VirtualCursorMove.canceled -= instance.OnVirtualCursorMove;
			VirtualCursorClick.started -= instance.OnVirtualCursorClick;
			VirtualCursorClick.performed -= instance.OnVirtualCursorClick;
			VirtualCursorClick.canceled -= instance.OnVirtualCursorClick;
			VirtualCursorScrollWheel.started -= instance.OnVirtualCursorScrollWheel;
			VirtualCursorScrollWheel.performed -= instance.OnVirtualCursorScrollWheel;
			VirtualCursorScrollWheel.canceled -= instance.OnVirtualCursorScrollWheel;
		}

		public void RemoveCallbacks(IUIActions instance)
		{
			if (m_Wrapper.m_UIActionsCallbackInterfaces.Remove(instance))
			{
				UnregisterCallbacks(instance);
			}
		}

		public void SetCallbacks(IUIActions instance)
		{
			foreach (IUIActions uIActionsCallbackInterface in m_Wrapper.m_UIActionsCallbackInterfaces)
			{
				UnregisterCallbacks(uIActionsCallbackInterface);
			}
			m_Wrapper.m_UIActionsCallbackInterfaces.Clear();
			AddCallbacks(instance);
		}
	}

	public struct VoteActions
	{
		private GameControls m_Wrapper;

		public InputAction Yes => m_Wrapper.m_Vote_Yes;

		public InputAction No => m_Wrapper.m_Vote_No;

		public bool enabled => Get().enabled;

		public VoteActions(GameControls wrapper)
		{
			m_Wrapper = wrapper;
		}

		public InputActionMap Get()
		{
			return m_Wrapper.m_Vote;
		}

		public void Enable()
		{
			Get().Enable();
		}

		public void Disable()
		{
			Get().Disable();
		}

		public static implicit operator InputActionMap(VoteActions set)
		{
			return set.Get();
		}

		public void AddCallbacks(IVoteActions instance)
		{
			if (instance != null && !m_Wrapper.m_VoteActionsCallbackInterfaces.Contains(instance))
			{
				m_Wrapper.m_VoteActionsCallbackInterfaces.Add(instance);
				Yes.started += instance.OnYes;
				Yes.performed += instance.OnYes;
				Yes.canceled += instance.OnYes;
				No.started += instance.OnNo;
				No.performed += instance.OnNo;
				No.canceled += instance.OnNo;
			}
		}

		private void UnregisterCallbacks(IVoteActions instance)
		{
			Yes.started -= instance.OnYes;
			Yes.performed -= instance.OnYes;
			Yes.canceled -= instance.OnYes;
			No.started -= instance.OnNo;
			No.performed -= instance.OnNo;
			No.canceled -= instance.OnNo;
		}

		public void RemoveCallbacks(IVoteActions instance)
		{
			if (m_Wrapper.m_VoteActionsCallbackInterfaces.Remove(instance))
			{
				UnregisterCallbacks(instance);
			}
		}

		public void SetCallbacks(IVoteActions instance)
		{
			foreach (IVoteActions voteActionsCallbackInterface in m_Wrapper.m_VoteActionsCallbackInterfaces)
			{
				UnregisterCallbacks(voteActionsCallbackInterface);
			}
			m_Wrapper.m_VoteActionsCallbackInterfaces.Clear();
			AddCallbacks(instance);
		}
	}

	public interface IGameplayActions
	{
		void OnMove(InputAction.CallbackContext context);

		void OnJump(InputAction.CallbackContext context);

		void OnAim(InputAction.CallbackContext context);

		void OnSwing(InputAction.CallbackContext context);

		void OnUseItem(InputAction.CallbackContext context);

		void OnPitch(InputAction.CallbackContext context);

		void OnInteract(InputAction.CallbackContext context);

		void OnCancel(InputAction.CallbackContext context);

		void OnDropItem(InputAction.CallbackContext context);

		void OnDive(InputAction.CallbackContext context);

		void OnWalk(InputAction.CallbackContext context);

		void OnCycleSwingAngle(InputAction.CallbackContext context);

		void OnRestart(InputAction.CallbackContext context);
	}

	public interface IGolfCartDriverActions
	{
		void OnAccelerate(InputAction.CallbackContext context);

		void OnBrake(InputAction.CallbackContext context);

		void OnSteer(InputAction.CallbackContext context);

		void OnJump(InputAction.CallbackContext context);

		void OnHonk(InputAction.CallbackContext context);
	}

	public interface IGolfCartSharedActions
	{
		void OnExit(InputAction.CallbackContext context);

		void OnDiveOut(InputAction.CallbackContext context);
	}

	public interface IHotkeysActions
	{
		void OnHotkey1(InputAction.CallbackContext context);

		void OnHotkey2(InputAction.CallbackContext context);

		void OnHotkey3(InputAction.CallbackContext context);

		void OnHotkey4(InputAction.CallbackContext context);

		void OnHotkey5(InputAction.CallbackContext context);

		void OnHotkey6(InputAction.CallbackContext context);

		void OnHotkey7(InputAction.CallbackContext context);

		void OnHotkey8(InputAction.CallbackContext context);

		void OnCycleLeft(InputAction.CallbackContext context);

		void OnCycleRight(InputAction.CallbackContext context);

		void OnToggle(InputAction.CallbackContext context);
	}

	public interface ISpectateActions
	{
		void OnCyclePreviousPlayer(InputAction.CallbackContext context);

		void OnCycleNextPlayer(InputAction.CallbackContext context);
	}

	public interface IIngameActions
	{
		void OnShowScoreboard(InputAction.CallbackContext context);

		void OnPause(InputAction.CallbackContext context);

		void OnOpenChat(InputAction.CallbackContext context);

		void OnToggleEmoteMenu(InputAction.CallbackContext context);
	}

	public interface IVoiceChatActions
	{
		void OnPushToTalk(InputAction.CallbackContext context);
	}

	public interface ICameraActions
	{
		void OnLook(InputAction.CallbackContext context);
	}

	public interface IRadialMenuActions
	{
		void OnSelect(InputAction.CallbackContext context);

		void OnCancel(InputAction.CallbackContext context);
	}

	public interface IUIActions
	{
		void OnNavigate(InputAction.CallbackContext context);

		void OnSubmit(InputAction.CallbackContext context);

		void OnCancel(InputAction.CallbackContext context);

		void OnPoint(InputAction.CallbackContext context);

		void OnClick(InputAction.CallbackContext context);

		void OnScrollWheel(InputAction.CallbackContext context);

		void OnMiddleClick(InputAction.CallbackContext context);

		void OnRightClick(InputAction.CallbackContext context);

		void OnExit(InputAction.CallbackContext context);

		void OnVirtualCursorMove(InputAction.CallbackContext context);

		void OnVirtualCursorClick(InputAction.CallbackContext context);

		void OnVirtualCursorScrollWheel(InputAction.CallbackContext context);
	}

	public interface IVoteActions
	{
		void OnYes(InputAction.CallbackContext context);

		void OnNo(InputAction.CallbackContext context);
	}

	private readonly InputActionMap m_Gameplay;

	private List<IGameplayActions> m_GameplayActionsCallbackInterfaces = new List<IGameplayActions>();

	private readonly InputAction m_Gameplay_Move;

	private readonly InputAction m_Gameplay_Jump;

	private readonly InputAction m_Gameplay_Aim;

	private readonly InputAction m_Gameplay_Swing;

	private readonly InputAction m_Gameplay_UseItem;

	private readonly InputAction m_Gameplay_Pitch;

	private readonly InputAction m_Gameplay_Interact;

	private readonly InputAction m_Gameplay_Cancel;

	private readonly InputAction m_Gameplay_DropItem;

	private readonly InputAction m_Gameplay_Dive;

	private readonly InputAction m_Gameplay_Walk;

	private readonly InputAction m_Gameplay_CycleSwingAngle;

	private readonly InputAction m_Gameplay_Restart;

	private readonly InputActionMap m_GolfCartDriver;

	private List<IGolfCartDriverActions> m_GolfCartDriverActionsCallbackInterfaces = new List<IGolfCartDriverActions>();

	private readonly InputAction m_GolfCartDriver_Accelerate;

	private readonly InputAction m_GolfCartDriver_Brake;

	private readonly InputAction m_GolfCartDriver_Steer;

	private readonly InputAction m_GolfCartDriver_Jump;

	private readonly InputAction m_GolfCartDriver_Honk;

	private readonly InputActionMap m_GolfCartShared;

	private List<IGolfCartSharedActions> m_GolfCartSharedActionsCallbackInterfaces = new List<IGolfCartSharedActions>();

	private readonly InputAction m_GolfCartShared_Exit;

	private readonly InputAction m_GolfCartShared_DiveOut;

	private readonly InputActionMap m_Hotkeys;

	private List<IHotkeysActions> m_HotkeysActionsCallbackInterfaces = new List<IHotkeysActions>();

	private readonly InputAction m_Hotkeys_Hotkey1;

	private readonly InputAction m_Hotkeys_Hotkey2;

	private readonly InputAction m_Hotkeys_Hotkey3;

	private readonly InputAction m_Hotkeys_Hotkey4;

	private readonly InputAction m_Hotkeys_Hotkey5;

	private readonly InputAction m_Hotkeys_Hotkey6;

	private readonly InputAction m_Hotkeys_Hotkey7;

	private readonly InputAction m_Hotkeys_Hotkey8;

	private readonly InputAction m_Hotkeys_CycleLeft;

	private readonly InputAction m_Hotkeys_CycleRight;

	private readonly InputAction m_Hotkeys_Toggle;

	private readonly InputActionMap m_Spectate;

	private List<ISpectateActions> m_SpectateActionsCallbackInterfaces = new List<ISpectateActions>();

	private readonly InputAction m_Spectate_CyclePreviousPlayer;

	private readonly InputAction m_Spectate_CycleNextPlayer;

	private readonly InputActionMap m_Ingame;

	private List<IIngameActions> m_IngameActionsCallbackInterfaces = new List<IIngameActions>();

	private readonly InputAction m_Ingame_ShowScoreboard;

	private readonly InputAction m_Ingame_Pause;

	private readonly InputAction m_Ingame_OpenChat;

	private readonly InputAction m_Ingame_ToggleEmoteMenu;

	private readonly InputActionMap m_VoiceChat;

	private List<IVoiceChatActions> m_VoiceChatActionsCallbackInterfaces = new List<IVoiceChatActions>();

	private readonly InputAction m_VoiceChat_PushToTalk;

	private readonly InputActionMap m_Camera;

	private List<ICameraActions> m_CameraActionsCallbackInterfaces = new List<ICameraActions>();

	private readonly InputAction m_Camera_Look;

	private readonly InputActionMap m_RadialMenu;

	private List<IRadialMenuActions> m_RadialMenuActionsCallbackInterfaces = new List<IRadialMenuActions>();

	private readonly InputAction m_RadialMenu_Select;

	private readonly InputAction m_RadialMenu_Cancel;

	private readonly InputActionMap m_UI;

	private List<IUIActions> m_UIActionsCallbackInterfaces = new List<IUIActions>();

	private readonly InputAction m_UI_Navigate;

	private readonly InputAction m_UI_Submit;

	private readonly InputAction m_UI_Cancel;

	private readonly InputAction m_UI_Point;

	private readonly InputAction m_UI_Click;

	private readonly InputAction m_UI_ScrollWheel;

	private readonly InputAction m_UI_MiddleClick;

	private readonly InputAction m_UI_RightClick;

	private readonly InputAction m_UI_Exit;

	private readonly InputAction m_UI_VirtualCursorMove;

	private readonly InputAction m_UI_VirtualCursorClick;

	private readonly InputAction m_UI_VirtualCursorScrollWheel;

	private readonly InputActionMap m_Vote;

	private List<IVoteActions> m_VoteActionsCallbackInterfaces = new List<IVoteActions>();

	private readonly InputAction m_Vote_Yes;

	private readonly InputAction m_Vote_No;

	private int m_GamepadSchemeIndex = -1;

	private int m_KeyboardMouseSchemeIndex = -1;

	public InputActionAsset asset { get; }

	public InputBinding? bindingMask
	{
		get
		{
			return asset.bindingMask;
		}
		set
		{
			asset.bindingMask = value;
		}
	}

	public ReadOnlyArray<InputDevice>? devices
	{
		get
		{
			return asset.devices;
		}
		set
		{
			asset.devices = value;
		}
	}

	public ReadOnlyArray<InputControlScheme> controlSchemes => asset.controlSchemes;

	public IEnumerable<InputBinding> bindings => asset.bindings;

	public GameplayActions Gameplay => new GameplayActions(this);

	public GolfCartDriverActions GolfCartDriver => new GolfCartDriverActions(this);

	public GolfCartSharedActions GolfCartShared => new GolfCartSharedActions(this);

	public HotkeysActions Hotkeys => new HotkeysActions(this);

	public SpectateActions Spectate => new SpectateActions(this);

	public IngameActions Ingame => new IngameActions(this);

	public VoiceChatActions VoiceChat => new VoiceChatActions(this);

	public CameraActions Camera => new CameraActions(this);

	public RadialMenuActions RadialMenu => new RadialMenuActions(this);

	public UIActions UI => new UIActions(this);

	public VoteActions Vote => new VoteActions(this);

	public InputControlScheme GamepadScheme
	{
		get
		{
			if (m_GamepadSchemeIndex == -1)
			{
				m_GamepadSchemeIndex = asset.FindControlSchemeIndex("Gamepad");
			}
			return asset.controlSchemes[m_GamepadSchemeIndex];
		}
	}

	public InputControlScheme KeyboardMouseScheme
	{
		get
		{
			if (m_KeyboardMouseSchemeIndex == -1)
			{
				m_KeyboardMouseSchemeIndex = asset.FindControlSchemeIndex("Keyboard Mouse");
			}
			return asset.controlSchemes[m_KeyboardMouseSchemeIndex];
		}
	}

	public GameControls()
	{
		asset = InputActionAsset.FromJson("{\r\n    \"version\": 1,\r\n    \"name\": \"GameControls\",\r\n    \"maps\": [\r\n        {\r\n            \"name\": \"Gameplay\",\r\n            \"id\": \"79e0c5b4-32e1-4ff9-967f-b1091a065991\",\r\n            \"actions\": [\r\n                {\r\n                    \"name\": \"Move\",\r\n                    \"type\": \"Value\",\r\n                    \"id\": \"bdacee7a-482d-45eb-bcd8-86edf9e03a10\",\r\n                    \"expectedControlType\": \"Vector2\",\r\n                    \"processors\": \"\",\r\n                    \"interactions\": \"\",\r\n                    \"initialStateCheck\": true\r\n                },\r\n                {\r\n                    \"name\": \"Jump\",\r\n                    \"type\": \"Button\",\r\n                    \"id\": \"c2954cb0-9401-42d0-bbc0-b9bd8bfd65c2\",\r\n                    \"expectedControlType\": \"\",\r\n                    \"processors\": \"\",\r\n                    \"interactions\": \"\",\r\n                    \"initialStateCheck\": false\r\n                },\r\n                {\r\n                    \"name\": \"Aim\",\r\n                    \"type\": \"Button\",\r\n                    \"id\": \"a6ea8bcf-88cd-46bc-b17f-0fae4837ba41\",\r\n                    \"expectedControlType\": \"\",\r\n                    \"processors\": \"\",\r\n                    \"interactions\": \"\",\r\n                    \"initialStateCheck\": false\r\n                },\r\n                {\r\n                    \"name\": \"Swing\",\r\n                    \"type\": \"Button\",\r\n                    \"id\": \"a94432d6-909f-438f-b798-ff00fb2d7db0\",\r\n                    \"expectedControlType\": \"\",\r\n                    \"processors\": \"\",\r\n                    \"interactions\": \"\",\r\n                    \"initialStateCheck\": false\r\n                },\r\n                {\r\n                    \"name\": \"Use Item\",\r\n                    \"type\": \"Button\",\r\n                    \"id\": \"83975d4e-0af4-4fec-ab0b-8cf113f1b4e0\",\r\n                    \"expectedControlType\": \"\",\r\n                    \"processors\": \"\",\r\n                    \"interactions\": \"\",\r\n                    \"initialStateCheck\": false\r\n                },\r\n                {\r\n                    \"name\": \"Pitch\",\r\n                    \"type\": \"Value\",\r\n                    \"id\": \"bec6de06-4d26-426c-8169-63e846687214\",\r\n                    \"expectedControlType\": \"Axis\",\r\n                    \"processors\": \"\",\r\n                    \"interactions\": \"\",\r\n                    \"initialStateCheck\": true\r\n                },\r\n                {\r\n                    \"name\": \"Interact\",\r\n                    \"type\": \"Button\",\r\n                    \"id\": \"baaa4628-661d-484b-adbe-6458a9e9beab\",\r\n                    \"expectedControlType\": \"\",\r\n                    \"processors\": \"\",\r\n                    \"interactions\": \"\",\r\n                    \"initialStateCheck\": false\r\n                },\r\n                {\r\n                    \"name\": \"Cancel\",\r\n                    \"type\": \"Button\",\r\n                    \"id\": \"cab9af4d-119b-4254-b5e6-a6ca821b858e\",\r\n                    \"expectedControlType\": \"\",\r\n                    \"processors\": \"\",\r\n                    \"interactions\": \"\",\r\n                    \"initialStateCheck\": false\r\n                },\r\n                {\r\n                    \"name\": \"Drop Item\",\r\n                    \"type\": \"Button\",\r\n                    \"id\": \"d040b68f-b261-49d7-b247-fd669bd4522c\",\r\n                    \"expectedControlType\": \"\",\r\n                    \"processors\": \"\",\r\n                    \"interactions\": \"\",\r\n                    \"initialStateCheck\": false\r\n                },\r\n                {\r\n                    \"name\": \"Dive\",\r\n                    \"type\": \"Button\",\r\n                    \"id\": \"28059a0f-c02a-4468-b545-da2e49bb4214\",\r\n                    \"expectedControlType\": \"\",\r\n                    \"processors\": \"\",\r\n                    \"interactions\": \"\",\r\n                    \"initialStateCheck\": false\r\n                },\r\n                {\r\n                    \"name\": \"Walk\",\r\n                    \"type\": \"Button\",\r\n                    \"id\": \"0de61405-3ff8-497d-88e7-c2b58eb3db2d\",\r\n                    \"expectedControlType\": \"\",\r\n                    \"processors\": \"\",\r\n                    \"interactions\": \"\",\r\n                    \"initialStateCheck\": false\r\n                },\r\n                {\r\n                    \"name\": \"Cycle Swing Angle\",\r\n                    \"type\": \"Button\",\r\n                    \"id\": \"c8d6d2f0-d393-4494-a573-43302259548e\",\r\n                    \"expectedControlType\": \"\",\r\n                    \"processors\": \"\",\r\n                    \"interactions\": \"\",\r\n                    \"initialStateCheck\": false\r\n                },\r\n                {\r\n                    \"name\": \"Restart\",\r\n                    \"type\": \"Button\",\r\n                    \"id\": \"53bfb680-6eb1-4a4d-9a0d-84ac65cdac95\",\r\n                    \"expectedControlType\": \"\",\r\n                    \"processors\": \"\",\r\n                    \"interactions\": \"Hold(duration=2)\",\r\n                    \"initialStateCheck\": false\r\n                }\r\n            ],\r\n            \"bindings\": [\r\n                {\r\n                    \"name\": \"WASD\",\r\n                    \"id\": \"5dfdad70-3e37-4c98-af78-85aafaef4fbb\",\r\n                    \"path\": \"2DVector\",\r\n                    \"interactions\": \"\",\r\n                    \"processors\": \"\",\r\n                    \"groups\": \"\",\r\n                    \"action\": \"Move\",\r\n                    \"isComposite\": true,\r\n                    \"isPartOfComposite\": false\r\n                },\r\n                {\r\n                    \"name\": \"up\",\r\n                    \"id\": \"ff7d426a-5c0a-4556-8967-a8c6733e9e2c\",\r\n                    \"path\": \"<Keyboard>/w\",\r\n                    \"interactions\": \"\",\r\n                    \"processors\": \"\",\r\n                    \"groups\": \";Keyboard Mouse\",\r\n                    \"action\": \"Move\",\r\n                    \"isComposite\": false,\r\n                    \"isPartOfComposite\": true\r\n                },\r\n                {\r\n                    \"name\": \"left\",\r\n                    \"id\": \"3e064d1a-8942-454e-97d3-e57dcb5b1f24\",\r\n                    \"path\": \"<Keyboard>/a\",\r\n                    \"interactions\": \"\",\r\n                    \"processors\": \"\",\r\n                    \"groups\": \"Keyboard Mouse\",\r\n                    \"action\": \"Move\",\r\n                    \"isComposite\": false,\r\n                    \"isPartOfComposite\": true\r\n                },\r\n                {\r\n                    \"name\": \"down\",\r\n                    \"id\": \"44871ff0-0eec-446b-9ba2-48a156767341\",\r\n                    \"path\": \"<Keyboard>/s\",\r\n                    \"interactions\": \"\",\r\n                    \"processors\": \"\",\r\n                    \"groups\": \";Keyboard Mouse\",\r\n                    \"action\": \"Move\",\r\n                    \"isComposite\": false,\r\n                    \"isPartOfComposite\": true\r\n                },\r\n                {\r\n                    \"name\": \"right\",\r\n                    \"id\": \"cec14805-ff8e-42d5-a6bb-d5d81632658d\",\r\n                    \"path\": \"<Keyboard>/d\",\r\n                    \"interactions\": \"\",\r\n                    \"processors\": \"\",\r\n                    \"groups\": \"Keyboard Mouse\",\r\n                    \"action\": \"Move\",\r\n                    \"isComposite\": false,\r\n                    \"isPartOfComposite\": true\r\n                },\r\n                {\r\n                    \"name\": \"Left Stick\",\r\n                    \"id\": \"4dd344c2-a810-4c41-b893-f6266770acc0\",\r\n                    \"path\": \"2DVector(mode=2)\",\r\n                    \"interactions\": \"\",\r\n                    \"processors\": \"StickDeadzone\",\r\n                    \"groups\": \"\",\r\n                    \"action\": \"Move\",\r\n                    \"isComposite\": true,\r\n                    \"isPartOfComposite\": false\r\n                },\r\n                {\r\n                    \"name\": \"up\",\r\n                    \"id\": \"05577182-73c7-443c-83d2-2c885045b378\",\r\n                    \"path\": \"<Gamepad>/leftStick/up\",\r\n                    \"interactions\": \"\",\r\n                    \"processors\": \"\",\r\n                    \"groups\": \";Gamepad\",\r\n                    \"action\": \"Move\",\r\n                    \"isComposite\": false,\r\n                    \"isPartOfComposite\": true\r\n                },\r\n                {\r\n                    \"name\": \"left\",\r\n                    \"id\": \"43830bd2-71da-4f1a-8a41-c60ccdd501e4\",\r\n                    \"path\": \"<Gamepad>/leftStick/left\",\r\n                    \"interactions\": \"\",\r\n                    \"processors\": \"\",\r\n                    \"groups\": \"Gamepad\",\r\n                    \"action\": \"Move\",\r\n                    \"isComposite\": false,\r\n                    \"isPartOfComposite\": true\r\n                },\r\n                {\r\n                    \"name\": \"down\",\r\n                    \"id\": \"51fae5f3-d8cf-4003-b6a0-51602a472290\",\r\n                    \"path\": \"<Gamepad>/leftStick/down\",\r\n                    \"interactions\": \"\",\r\n                    \"processors\": \"\",\r\n                    \"groups\": \";Gamepad\",\r\n                    \"action\": \"Move\",\r\n                    \"isComposite\": false,\r\n                    \"isPartOfComposite\": true\r\n                },\r\n                {\r\n                    \"name\": \"right\",\r\n                    \"id\": \"fe950ff9-e88c-486f-ac03-6cc8d7835d03\",\r\n                    \"path\": \"<Gamepad>/leftStick/right\",\r\n                    \"interactions\": \"\",\r\n                    \"processors\": \"\",\r\n                    \"groups\": \"Gamepad\",\r\n                    \"action\": \"Move\",\r\n                    \"isComposite\": false,\r\n                    \"isPartOfComposite\": true\r\n                },\r\n                {\r\n                    \"name\": \"\",\r\n                    \"id\": \"cdbe0717-d25f-42d2-9f60-018667358759\",\r\n                    \"path\": \"<Keyboard>/space\",\r\n                    \"interactions\": \"\",\r\n                    \"processors\": \"\",\r\n                    \"groups\": \"Keyboard Mouse\",\r\n                    \"action\": \"Jump\",\r\n                    \"isComposite\": false,\r\n                    \"isPartOfComposite\": false\r\n                },\r\n                {\r\n                    \"name\": \"\",\r\n                    \"id\": \"974fc0e6-a065-4b20-976a-44c07634deaa\",\r\n                    \"path\": \"<Gamepad>/buttonSouth\",\r\n                    \"interactions\": \"\",\r\n                    \"processors\": \"\",\r\n                    \"groups\": \"Gamepad\",\r\n                    \"action\": \"Jump\",\r\n                    \"isComposite\": false,\r\n                    \"isPartOfComposite\": false\r\n                },\r\n                {\r\n                    \"name\": \"\",\r\n                    \"id\": \"81703d59-1d99-4a0c-8f94-b91c6d4ce65c\",\r\n                    \"path\": \"<Mouse>/rightButton\",\r\n                    \"interactions\": \"\",\r\n                    \"processors\": \"\",\r\n                    \"groups\": \"Keyboard Mouse\",\r\n                    \"action\": \"Aim\",\r\n                    \"isComposite\": false,\r\n                    \"isPartOfComposite\": false\r\n                },\r\n                {\r\n                    \"name\": \"\",\r\n                    \"id\": \"4c234d02-fe30-440c-a9a7-7ebf77ecaacc\",\r\n                    \"path\": \"<Gamepad>/leftTrigger\",\r\n                    \"interactions\": \"\",\r\n                    \"processors\": \"\",\r\n                    \"groups\": \"Gamepad\",\r\n                    \"action\": \"Aim\",\r\n                    \"isComposite\": false,\r\n                    \"isPartOfComposite\": false\r\n                },\r\n                {\r\n                    \"name\": \"\",\r\n                    \"id\": \"2cbc427c-fbec-4181-b8fe-e23de0a69285\",\r\n                    \"path\": \"<Mouse>/leftButton\",\r\n                    \"interactions\": \"\",\r\n                    \"processors\": \"\",\r\n                    \"groups\": \";Keyboard Mouse\",\r\n                    \"action\": \"Swing\",\r\n                    \"isComposite\": false,\r\n                    \"isPartOfComposite\": false\r\n                },\r\n                {\r\n                    \"name\": \"\",\r\n                    \"id\": \"d5f84d4e-5b1e-485b-83eb-9f54be73e8d3\",\r\n                    \"path\": \"<Gamepad>/rightTrigger\",\r\n                    \"interactions\": \"\",\r\n                    \"processors\": \"\",\r\n                    \"groups\": \";Gamepad\",\r\n                    \"action\": \"Swing\",\r\n                    \"isComposite\": false,\r\n                    \"isPartOfComposite\": false\r\n                },\r\n                {\r\n                    \"name\": \"\",\r\n                    \"id\": \"652cad9e-0d57-4ec7-9328-e4401d4261bc\",\r\n                    \"path\": \"<Keyboard>/f\",\r\n                    \"interactions\": \"\",\r\n                    \"processors\": \"\",\r\n                    \"groups\": \";Keyboard Mouse\",\r\n                    \"action\": \"Cancel\",\r\n                    \"isComposite\": false,\r\n                    \"isPartOfComposite\": false\r\n                },\r\n                {\r\n                    \"name\": \"\",\r\n                    \"id\": \"71223026-fb4c-4b36-9900-d9f8625bc179\",\r\n                    \"path\": \"<Gamepad>/dpad/down\",\r\n                    \"interactions\": \"\",\r\n                    \"processors\": \"\",\r\n                    \"groups\": \";Gamepad\",\r\n                    \"action\": \"Cancel\",\r\n                    \"isComposite\": false,\r\n                    \"isPartOfComposite\": false\r\n                },\r\n                {\r\n                    \"name\": \"\",\r\n                    \"id\": \"1d4cdd4f-923f-48e4-ba3f-16bbd1ca8eb6\",\r\n                    \"path\": \"<Keyboard>/f\",\r\n                    \"interactions\": \"\",\r\n                    \"processors\": \"\",\r\n                    \"groups\": \";Keyboard Mouse\",\r\n                    \"action\": \"Drop Item\",\r\n                    \"isComposite\": false,\r\n                    \"isPartOfComposite\": false\r\n                },\r\n                {\r\n                    \"name\": \"\",\r\n                    \"id\": \"9f8473a1-ffb7-4fb7-80a0-6e81666eae13\",\r\n                    \"path\": \"<Gamepad>/rightStickPress\",\r\n                    \"interactions\": \"\",\r\n                    \"processors\": \"\",\r\n                    \"groups\": \";Gamepad\",\r\n                    \"action\": \"Drop Item\",\r\n                    \"isComposite\": false,\r\n                    \"isPartOfComposite\": false\r\n                },\r\n                {\r\n                    \"name\": \"1D Axis\",\r\n                    \"id\": \"e213e64f-6ff2-4dfc-ae53-da369b059366\",\r\n                    \"path\": \"1DAxis\",\r\n                    \"interactions\": \"\",\r\n                    \"processors\": \"\",\r\n                    \"groups\": \"\",\r\n                    \"action\": \"Pitch\",\r\n                    \"isComposite\": true,\r\n                    \"isPartOfComposite\": false\r\n                },\r\n                {\r\n                    \"name\": \"negative\",\r\n                    \"id\": \"4962862f-95ff-418a-8471-6ba811cfa502\",\r\n                    \"path\": \"<Mouse>/scroll/down\",\r\n                    \"interactions\": \"\",\r\n                    \"processors\": \"\",\r\n                    \"groups\": \";Keyboard Mouse\",\r\n                    \"action\": \"Pitch\",\r\n                    \"isComposite\": false,\r\n                    \"isPartOfComposite\": true\r\n                },\r\n                {\r\n                    \"name\": \"positive\",\r\n                    \"id\": \"d088585b-506a-49db-b020-3554ffe6ef6e\",\r\n                    \"path\": \"<Mouse>/scroll/up\",\r\n                    \"interactions\": \"\",\r\n                    \"processors\": \"\",\r\n                    \"groups\": \";Keyboard Mouse\",\r\n                    \"action\": \"Pitch\",\r\n                    \"isComposite\": false,\r\n                    \"isPartOfComposite\": true\r\n                },\r\n                {\r\n                    \"name\": \"1D Axis\",\r\n                    \"id\": \"559d4153-b50e-45fe-ba68-edde390ebf27\",\r\n                    \"path\": \"1DAxis\",\r\n                    \"interactions\": \"\",\r\n                    \"processors\": \"\",\r\n                    \"groups\": \"\",\r\n                    \"action\": \"Pitch\",\r\n                    \"isComposite\": true,\r\n                    \"isPartOfComposite\": false\r\n                },\r\n                {\r\n                    \"name\": \"negative\",\r\n                    \"id\": \"792b597c-3560-45a9-964b-c4ec98173570\",\r\n                    \"path\": \"\",\r\n                    \"interactions\": \"\",\r\n                    \"processors\": \"\",\r\n                    \"groups\": \";Gamepad\",\r\n                    \"action\": \"Pitch\",\r\n                    \"isComposite\": false,\r\n                    \"isPartOfComposite\": true\r\n                },\r\n                {\r\n                    \"name\": \"positive\",\r\n                    \"id\": \"513265ea-0948-4991-a0d5-577e9a61e64c\",\r\n                    \"path\": \"\",\r\n                    \"interactions\": \"\",\r\n                    \"processors\": \"\",\r\n                    \"groups\": \";Gamepad\",\r\n                    \"action\": \"Pitch\",\r\n                    \"isComposite\": false,\r\n                    \"isPartOfComposite\": true\r\n                },\r\n                {\r\n                    \"name\": \"\",\r\n                    \"id\": \"ccf7f00a-6f6f-48e2-ae17-965d01a33caf\",\r\n                    \"path\": \"<Keyboard>/leftAlt\",\r\n                    \"interactions\": \"\",\r\n                    \"processors\": \"\",\r\n                    \"groups\": \";Keyboard Mouse\",\r\n                    \"action\": \"Walk\",\r\n                    \"isComposite\": false,\r\n                    \"isPartOfComposite\": false\r\n                },\r\n                {\r\n                    \"name\": \"\",\r\n                    \"id\": \"b68e4279-e3c8-4633-b944-49a83e1382d1\",\r\n                    \"path\": \"\",\r\n                    \"interactions\": \"\",\r\n                    \"processors\": \"\",\r\n                    \"groups\": \";Gamepad\",\r\n                    \"action\": \"Walk\",\r\n                    \"isComposite\": false,\r\n                    \"isPartOfComposite\": false\r\n                },\r\n                {\r\n                    \"name\": \"\",\r\n                    \"id\": \"8229cd76-3d05-46bf-aef4-fc44b5acb407\",\r\n                    \"path\": \"<Mouse>/leftButton\",\r\n                    \"interactions\": \"\",\r\n                    \"processors\": \"\",\r\n                    \"groups\": \";Keyboard Mouse\",\r\n                    \"action\": \"Use Item\",\r\n                    \"isComposite\": false,\r\n                    \"isPartOfComposite\": false\r\n                },\r\n                {\r\n                    \"name\": \"\",\r\n                    \"id\": \"d49f7d34-413f-4558-b00c-ad4510407287\",\r\n                    \"path\": \"<Gamepad>/rightTrigger\",\r\n                    \"interactions\": \"\",\r\n                    \"processors\": \"\",\r\n                    \"groups\": \";Gamepad\",\r\n                    \"action\": \"Use Item\",\r\n                    \"isComposite\": false,\r\n                    \"isPartOfComposite\": false\r\n                },\r\n                {\r\n                    \"name\": \"\",\r\n                    \"id\": \"def25f67-9492-4df2-a6da-df5008557d33\",\r\n                    \"path\": \"<Keyboard>/e\",\r\n                    \"interactions\": \"\",\r\n                    \"processors\": \"\",\r\n                    \"groups\": \";Keyboard Mouse\",\r\n                    \"action\": \"Interact\",\r\n                    \"isComposite\": false,\r\n                    \"isPartOfComposite\": false\r\n                },\r\n                {\r\n                    \"name\": \"\",\r\n                    \"id\": \"cdfbf9f0-9a7a-4c1f-9e69-95b2642280a3\",\r\n                    \"path\": \"<Gamepad>/buttonWest\",\r\n                    \"interactions\": \"\",\r\n                    \"processors\": \"\",\r\n                    \"groups\": \";Gamepad\",\r\n                    \"action\": \"Interact\",\r\n                    \"isComposite\": false,\r\n                    \"isPartOfComposite\": false\r\n                },\r\n                {\r\n                    \"name\": \"\",\r\n                    \"id\": \"24104375-4080-4ab2-9898-dde4f9708490\",\r\n                    \"path\": \"<Keyboard>/leftCtrl\",\r\n                    \"interactions\": \"\",\r\n                    \"processors\": \"\",\r\n                    \"groups\": \";Keyboard Mouse\",\r\n                    \"action\": \"Dive\",\r\n                    \"isComposite\": false,\r\n                    \"isPartOfComposite\": false\r\n                },\r\n                {\r\n                    \"name\": \"\",\r\n                    \"id\": \"a7690137-f2f6-4753-8e63-ded4eb491107\",\r\n                    \"path\": \"<Gamepad>/buttonEast\",\r\n                    \"interactions\": \"\",\r\n                    \"processors\": \"\",\r\n                    \"groups\": \";Gamepad\",\r\n                    \"action\": \"Dive\",\r\n                    \"isComposite\": false,\r\n                    \"isPartOfComposite\": false\r\n                },\r\n                {\r\n                    \"name\": \"\",\r\n                    \"id\": \"f279af6d-84c9-4425-9f8d-cda396f07b3f\",\r\n                    \"path\": \"<Gamepad>/buttonNorth\",\r\n                    \"interactions\": \"\",\r\n                    \"processors\": \"\",\r\n                    \"groups\": \";Gamepad\",\r\n                    \"action\": \"Cycle Swing Angle\",\r\n                    \"isComposite\": false,\r\n                    \"isPartOfComposite\": false\r\n                },\r\n                {\r\n                    \"name\": \"\",\r\n                    \"id\": \"acf2c9a3-504e-44bf-8a72-e146aae37a2f\",\r\n                    \"path\": \"\",\r\n                    \"interactions\": \"\",\r\n                    \"processors\": \"\",\r\n                    \"groups\": \";Keyboard Mouse\",\r\n                    \"action\": \"Cycle Swing Angle\",\r\n                    \"isComposite\": false,\r\n                    \"isPartOfComposite\": false\r\n                },\r\n                {\r\n                    \"name\": \"\",\r\n                    \"id\": \"6978eda9-b3d5-4f3a-a134-390ae9698d88\",\r\n                    \"path\": \"<Gamepad>/dpad/down\",\r\n                    \"interactions\": \"\",\r\n                    \"processors\": \"\",\r\n                    \"groups\": \";Gamepad\",\r\n                    \"action\": \"Restart\",\r\n                    \"isComposite\": false,\r\n                    \"isPartOfComposite\": false\r\n                },\r\n                {\r\n                    \"name\": \"\",\r\n                    \"id\": \"3fb74875-06d4-4399-aa22-a21b6b6e928a\",\r\n                    \"path\": \"<Keyboard>/r\",\r\n                    \"interactions\": \"\",\r\n                    \"processors\": \"\",\r\n                    \"groups\": \";Keyboard Mouse\",\r\n                    \"action\": \"Restart\",\r\n                    \"isComposite\": false,\r\n                    \"isPartOfComposite\": false\r\n                }\r\n            ]\r\n        },\r\n        {\r\n            \"name\": \"Golf Cart Driver\",\r\n            \"id\": \"c0d41579-22e1-40b0-9295-4c92fe597728\",\r\n            \"actions\": [\r\n                {\r\n                    \"name\": \"Accelerate\",\r\n                    \"type\": \"Button\",\r\n                    \"id\": \"ac34ea04-40ff-4c45-80fd-2fff1c51b1f5\",\r\n                    \"expectedControlType\": \"\",\r\n                    \"processors\": \"\",\r\n                    \"interactions\": \"\",\r\n                    \"initialStateCheck\": true\r\n                },\r\n                {\r\n                    \"name\": \"Brake\",\r\n                    \"type\": \"Button\",\r\n                    \"id\": \"985da31e-3e18-429c-8002-8a016dfa7d1a\",\r\n                    \"expectedControlType\": \"\",\r\n                    \"processors\": \"\",\r\n                    \"interactions\": \"\",\r\n                    \"initialStateCheck\": true\r\n                },\r\n                {\r\n                    \"name\": \"Steer\",\r\n                    \"type\": \"Value\",\r\n                    \"id\": \"a3ec7971-da46-47b6-b410-c869dd9de318\",\r\n                    \"expectedControlType\": \"Axis\",\r\n                    \"processors\": \"\",\r\n                    \"interactions\": \"\",\r\n                    \"initialStateCheck\": true\r\n                },\r\n                {\r\n                    \"name\": \"Jump\",\r\n                    \"type\": \"Button\",\r\n                    \"id\": \"d6713902-db19-402d-8d8a-58c3fd1566b9\",\r\n                    \"expectedControlType\": \"\",\r\n                    \"processors\": \"\",\r\n                    \"interactions\": \"\",\r\n                    \"initialStateCheck\": false\r\n                },\r\n                {\r\n                    \"name\": \"Honk\",\r\n                    \"type\": \"Button\",\r\n                    \"id\": \"49e0cdaa-62cd-4e4a-9a62-9afc7d4c3df0\",\r\n                    \"expectedControlType\": \"\",\r\n                    \"processors\": \"\",\r\n                    \"interactions\": \"\",\r\n                    \"initialStateCheck\": false\r\n                }\r\n            ],\r\n            \"bindings\": [\r\n                {\r\n                    \"name\": \"\",\r\n                    \"id\": \"9a0977cb-61ea-4009-a546-a9543d74e3db\",\r\n                    \"path\": \"<Keyboard>/w\",\r\n                    \"interactions\": \"\",\r\n                    \"processors\": \"\",\r\n                    \"groups\": \";Keyboard Mouse\",\r\n                    \"action\": \"Accelerate\",\r\n                    \"isComposite\": false,\r\n                    \"isPartOfComposite\": false\r\n                },\r\n                {\r\n                    \"name\": \"\",\r\n                    \"id\": \"6e871ec3-082f-48ac-8936-49a08d7b59a7\",\r\n                    \"path\": \"<Gamepad>/rightTrigger\",\r\n                    \"interactions\": \"\",\r\n                    \"processors\": \"\",\r\n                    \"groups\": \";Gamepad;Keyboard Mouse\",\r\n                    \"action\": \"Accelerate\",\r\n                    \"isComposite\": false,\r\n                    \"isPartOfComposite\": false\r\n                },\r\n                {\r\n                    \"name\": \"\",\r\n                    \"id\": \"1d873d9d-3b9c-4cb2-a478-72a1296eb427\",\r\n                    \"path\": \"<Keyboard>/s\",\r\n                    \"interactions\": \"\",\r\n                    \"processors\": \"\",\r\n                    \"groups\": \";Keyboard Mouse\",\r\n                    \"action\": \"Brake\",\r\n                    \"isComposite\": false,\r\n                    \"isPartOfComposite\": false\r\n                },\r\n                {\r\n                    \"name\": \"\",\r\n                    \"id\": \"c4a05e4c-a34c-41c9-b2a5-723450444f74\",\r\n                    \"path\": \"<Gamepad>/leftTrigger\",\r\n                    \"interactions\": \"\",\r\n                    \"processors\": \"\",\r\n                    \"groups\": \";Gamepad\",\r\n                    \"action\": \"Brake\",\r\n                    \"isComposite\": false,\r\n                    \"isPartOfComposite\": false\r\n                },\r\n                {\r\n                    \"name\": \"Keyboard\",\r\n                    \"id\": \"014946ee-94b9-4ad2-9220-cd1f943a6524\",\r\n                    \"path\": \"1DAxis\",\r\n                    \"interactions\": \"\",\r\n                    \"processors\": \"\",\r\n                    \"groups\": \"\",\r\n                    \"action\": \"Steer\",\r\n                    \"isComposite\": true,\r\n                    \"isPartOfComposite\": false\r\n                },\r\n                {\r\n                    \"name\": \"negative\",\r\n                    \"id\": \"936e5600-6f2f-4383-a9c7-fbacf639cad6\",\r\n                    \"path\": \"<Keyboard>/a\",\r\n                    \"interactions\": \"\",\r\n                    \"processors\": \"\",\r\n                    \"groups\": \";Keyboard Mouse\",\r\n                    \"action\": \"Steer\",\r\n                    \"isComposite\": false,\r\n                    \"isPartOfComposite\": true\r\n                },\r\n                {\r\n                    \"name\": \"positive\",\r\n                    \"id\": \"76546fcf-797f-4b99-ac38-d003e10b532c\",\r\n                    \"path\": \"<Keyboard>/d\",\r\n                    \"interactions\": \"\",\r\n                    \"processors\": \"\",\r\n                    \"groups\": \";Keyboard Mouse\",\r\n                    \"action\": \"Steer\",\r\n                    \"isComposite\": false,\r\n                    \"isPartOfComposite\": true\r\n                },\r\n                {\r\n                    \"name\": \"Gamepad\",\r\n                    \"id\": \"8beaa6f5-f197-41e9-85a3-7d1b8921321e\",\r\n                    \"path\": \"1DAxis\",\r\n                    \"interactions\": \"\",\r\n                    \"processors\": \"\",\r\n                    \"groups\": \"\",\r\n                    \"action\": \"Steer\",\r\n                    \"isComposite\": true,\r\n                    \"isPartOfComposite\": false\r\n                },\r\n                {\r\n                    \"name\": \"negative\",\r\n                    \"id\": \"16498794-efe7-499d-b728-133f0b9a92f0\",\r\n                    \"path\": \"<Gamepad>/leftStick/left\",\r\n                    \"interactions\": \"\",\r\n                    \"processors\": \"\",\r\n                    \"groups\": \";Keyboard Mouse;Gamepad\",\r\n                    \"action\": \"Steer\",\r\n                    \"isComposite\": false,\r\n                    \"isPartOfComposite\": true\r\n                },\r\n                {\r\n                    \"name\": \"positive\",\r\n                    \"id\": \"ee36d304-f049-4b2b-877c-376dbec9e16d\",\r\n                    \"path\": \"<Gamepad>/leftStick/right\",\r\n                    \"interactions\": \"\",\r\n                    \"processors\": \"\",\r\n                    \"groups\": \";Keyboard Mouse;Gamepad\",\r\n                    \"action\": \"Steer\",\r\n                    \"isComposite\": false,\r\n                    \"isPartOfComposite\": true\r\n                },\r\n                {\r\n                    \"name\": \"\",\r\n                    \"id\": \"f2c88bf4-e4cf-4d84-a1ac-78a1c2bc1cc6\",\r\n                    \"path\": \"<Mouse>/leftButton\",\r\n                    \"interactions\": \"\",\r\n                    \"processors\": \"\",\r\n                    \"groups\": \";Keyboard Mouse\",\r\n                    \"action\": \"Honk\",\r\n                    \"isComposite\": false,\r\n                    \"isPartOfComposite\": false\r\n                },\r\n                {\r\n                    \"name\": \"\",\r\n                    \"id\": \"ee0a05b3-24f4-4e04-b7ad-71b995a46066\",\r\n                    \"path\": \"<Gamepad>/leftStickPress\",\r\n                    \"interactions\": \"\",\r\n                    \"processors\": \"\",\r\n                    \"groups\": \";Gamepad\",\r\n                    \"action\": \"Honk\",\r\n                    \"isComposite\": false,\r\n                    \"isPartOfComposite\": false\r\n                },\r\n                {\r\n                    \"name\": \"\",\r\n                    \"id\": \"0a759efe-9bae-49e1-b2a5-e4192efcd56d\",\r\n                    \"path\": \"<Keyboard>/space\",\r\n                    \"interactions\": \"\",\r\n                    \"processors\": \"\",\r\n                    \"groups\": \"Keyboard Mouse\",\r\n                    \"action\": \"Jump\",\r\n                    \"isComposite\": false,\r\n                    \"isPartOfComposite\": false\r\n                },\r\n                {\r\n                    \"name\": \"\",\r\n                    \"id\": \"f8fdddc9-3001-416a-9a8d-1d40c051639c\",\r\n                    \"path\": \"<Gamepad>/buttonSouth\",\r\n                    \"interactions\": \"\",\r\n                    \"processors\": \"\",\r\n                    \"groups\": \"Gamepad\",\r\n                    \"action\": \"Jump\",\r\n                    \"isComposite\": false,\r\n                    \"isPartOfComposite\": false\r\n                }\r\n            ]\r\n        },\r\n        {\r\n            \"name\": \"Golf Cart Shared\",\r\n            \"id\": \"ca31d406-a80a-4d5c-91c3-475229e49d58\",\r\n            \"actions\": [\r\n                {\r\n                    \"name\": \"Exit\",\r\n                    \"type\": \"Button\",\r\n                    \"id\": \"d6f009a9-186c-48b8-8599-8ee5e2575254\",\r\n                    \"expectedControlType\": \"\",\r\n                    \"processors\": \"\",\r\n                    \"interactions\": \"\",\r\n                    \"initialStateCheck\": false\r\n                },\r\n                {\r\n                    \"name\": \"Dive Out\",\r\n                    \"type\": \"Button\",\r\n                    \"id\": \"b18a1bb4-4369-4240-b2bb-6d0fce55c605\",\r\n                    \"expectedControlType\": \"\",\r\n                    \"processors\": \"\",\r\n                    \"interactions\": \"\",\r\n                    \"initialStateCheck\": false\r\n                }\r\n            ],\r\n            \"bindings\": [\r\n                {\r\n                    \"name\": \"\",\r\n                    \"id\": \"be58b73d-a4a3-47eb-ada2-1b133d235727\",\r\n                    \"path\": \"<Keyboard>/e\",\r\n                    \"interactions\": \"\",\r\n                    \"processors\": \"\",\r\n                    \"groups\": \";Keyboard Mouse\",\r\n                    \"action\": \"Exit\",\r\n                    \"isComposite\": false,\r\n                    \"isPartOfComposite\": false\r\n                },\r\n                {\r\n                    \"name\": \"\",\r\n                    \"id\": \"f8e39f74-a5f9-4eae-8b8e-d557bd6e50e6\",\r\n                    \"path\": \"<Gamepad>/buttonWest\",\r\n                    \"interactions\": \"\",\r\n                    \"processors\": \"\",\r\n                    \"groups\": \";Gamepad\",\r\n                    \"action\": \"Exit\",\r\n                    \"isComposite\": false,\r\n                    \"isPartOfComposite\": false\r\n                },\r\n                {\r\n                    \"name\": \"\",\r\n                    \"id\": \"c82c2e3e-6c72-4f4b-8499-6634d7cab746\",\r\n                    \"path\": \"<Keyboard>/leftCtrl\",\r\n                    \"interactions\": \"\",\r\n                    \"processors\": \"\",\r\n                    \"groups\": \";Keyboard Mouse\",\r\n                    \"action\": \"Dive Out\",\r\n                    \"isComposite\": false,\r\n                    \"isPartOfComposite\": false\r\n                },\r\n                {\r\n                    \"name\": \"\",\r\n                    \"id\": \"def6676f-de1c-4d99-a87e-2ce609cd2590\",\r\n                    \"path\": \"<Gamepad>/buttonEast\",\r\n                    \"interactions\": \"\",\r\n                    \"processors\": \"\",\r\n                    \"groups\": \";Gamepad\",\r\n                    \"action\": \"Dive Out\",\r\n                    \"isComposite\": false,\r\n                    \"isPartOfComposite\": false\r\n                }\r\n            ]\r\n        },\r\n        {\r\n            \"name\": \"Hotkeys\",\r\n            \"id\": \"212eb9a3-ed96-4888-86d1-1ca7598064a2\",\r\n            \"actions\": [\r\n                {\r\n                    \"name\": \"Hotkey 1\",\r\n                    \"type\": \"Button\",\r\n                    \"id\": \"bf515930-e562-4bd6-8c38-570deb888d0e\",\r\n                    \"expectedControlType\": \"\",\r\n                    \"processors\": \"\",\r\n                    \"interactions\": \"\",\r\n                    \"initialStateCheck\": false\r\n                },\r\n                {\r\n                    \"name\": \"Hotkey 2\",\r\n                    \"type\": \"Button\",\r\n                    \"id\": \"dd2b3dc0-7643-41a8-a903-26ed0f8d6e1a\",\r\n                    \"expectedControlType\": \"\",\r\n                    \"processors\": \"\",\r\n                    \"interactions\": \"\",\r\n                    \"initialStateCheck\": false\r\n                },\r\n                {\r\n                    \"name\": \"Hotkey 3\",\r\n                    \"type\": \"Button\",\r\n                    \"id\": \"2d9693a9-0db6-4879-82f3-761b58a3929a\",\r\n                    \"expectedControlType\": \"\",\r\n                    \"processors\": \"\",\r\n                    \"interactions\": \"\",\r\n                    \"initialStateCheck\": false\r\n                },\r\n                {\r\n                    \"name\": \"Hotkey 4\",\r\n                    \"type\": \"Button\",\r\n                    \"id\": \"ad32d0cf-b440-471a-baef-aac046b3a45c\",\r\n                    \"expectedControlType\": \"\",\r\n                    \"processors\": \"\",\r\n                    \"interactions\": \"\",\r\n                    \"initialStateCheck\": false\r\n                },\r\n                {\r\n                    \"name\": \"Hotkey 5\",\r\n                    \"type\": \"Button\",\r\n                    \"id\": \"c78db740-c915-48fe-b972-7240e507a545\",\r\n                    \"expectedControlType\": \"\",\r\n                    \"processors\": \"\",\r\n                    \"interactions\": \"\",\r\n                    \"initialStateCheck\": false\r\n                },\r\n                {\r\n                    \"name\": \"Hotkey 6\",\r\n                    \"type\": \"Button\",\r\n                    \"id\": \"ffcdcb06-03af-4940-a88c-1db682aa1e54\",\r\n                    \"expectedControlType\": \"\",\r\n                    \"processors\": \"\",\r\n                    \"interactions\": \"\",\r\n                    \"initialStateCheck\": false\r\n                },\r\n                {\r\n                    \"name\": \"Hotkey 7\",\r\n                    \"type\": \"Button\",\r\n                    \"id\": \"840017f3-3121-4bab-8033-bdfe7a431545\",\r\n                    \"expectedControlType\": \"\",\r\n                    \"processors\": \"\",\r\n                    \"interactions\": \"\",\r\n                    \"initialStateCheck\": false\r\n                },\r\n                {\r\n                    \"name\": \"Hotkey 8\",\r\n                    \"type\": \"Button\",\r\n                    \"id\": \"a46eaaba-6b6d-400d-8c3b-6e8098f243a7\",\r\n                    \"expectedControlType\": \"\",\r\n                    \"processors\": \"\",\r\n                    \"interactions\": \"\",\r\n                    \"initialStateCheck\": false\r\n                },\r\n                {\r\n                    \"name\": \"Cycle Left\",\r\n                    \"type\": \"Button\",\r\n                    \"id\": \"fb6247ba-d008-4155-9b41-142095c6135f\",\r\n                    \"expectedControlType\": \"\",\r\n                    \"processors\": \"\",\r\n                    \"interactions\": \"\",\r\n                    \"initialStateCheck\": false\r\n                },\r\n                {\r\n                    \"name\": \"Cycle Right\",\r\n                    \"type\": \"Button\",\r\n                    \"id\": \"e9d42029-ecd1-431a-8e57-a497d3e7bede\",\r\n                    \"expectedControlType\": \"\",\r\n                    \"processors\": \"\",\r\n                    \"interactions\": \"\",\r\n                    \"initialStateCheck\": false\r\n                },\r\n                {\r\n                    \"name\": \"Toggle\",\r\n                    \"type\": \"Button\",\r\n                    \"id\": \"d33a78c1-b820-419a-a22b-0a87c56e9e2e\",\r\n                    \"expectedControlType\": \"\",\r\n                    \"processors\": \"\",\r\n                    \"interactions\": \"\",\r\n                    \"initialStateCheck\": false\r\n                }\r\n            ],\r\n            \"bindings\": [\r\n                {\r\n                    \"name\": \"\",\r\n                    \"id\": \"39758d15-b4d3-4a9d-9118-88606f358fba\",\r\n                    \"path\": \"<Keyboard>/1\",\r\n                    \"interactions\": \"\",\r\n                    \"processors\": \"\",\r\n                    \"groups\": \";Keyboard Mouse\",\r\n                    \"action\": \"Hotkey 1\",\r\n                    \"isComposite\": false,\r\n                    \"isPartOfComposite\": false\r\n                },\r\n                {\r\n                    \"name\": \"\",\r\n                    \"id\": \"aa336a32-2936-49f1-837c-eb75724d34d3\",\r\n                    \"path\": \"<Keyboard>/2\",\r\n                    \"interactions\": \"\",\r\n                    \"processors\": \"\",\r\n                    \"groups\": \";Keyboard Mouse\",\r\n                    \"action\": \"Hotkey 2\",\r\n                    \"isComposite\": false,\r\n                    \"isPartOfComposite\": false\r\n                },\r\n                {\r\n                    \"name\": \"\",\r\n                    \"id\": \"14fc964e-aa31-4bff-8a76-46e1eb31cc65\",\r\n                    \"path\": \"<Keyboard>/3\",\r\n                    \"interactions\": \"\",\r\n                    \"processors\": \"\",\r\n                    \"groups\": \";Keyboard Mouse\",\r\n                    \"action\": \"Hotkey 3\",\r\n                    \"isComposite\": false,\r\n                    \"isPartOfComposite\": false\r\n                },\r\n                {\r\n                    \"name\": \"\",\r\n                    \"id\": \"1d8e9563-90a3-4a79-b0ed-dcc207f5e5fa\",\r\n                    \"path\": \"<Keyboard>/4\",\r\n                    \"interactions\": \"\",\r\n                    \"processors\": \"\",\r\n                    \"groups\": \";Keyboard Mouse\",\r\n                    \"action\": \"Hotkey 4\",\r\n                    \"isComposite\": false,\r\n                    \"isPartOfComposite\": false\r\n                },\r\n                {\r\n                    \"name\": \"\",\r\n                    \"id\": \"f82e9014-b46d-4ad4-ab4d-62b962c8326d\",\r\n                    \"path\": \"<Keyboard>/5\",\r\n                    \"interactions\": \"\",\r\n                    \"processors\": \"\",\r\n                    \"groups\": \";Keyboard Mouse\",\r\n                    \"action\": \"Hotkey 5\",\r\n                    \"isComposite\": false,\r\n                    \"isPartOfComposite\": false\r\n                },\r\n                {\r\n                    \"name\": \"\",\r\n                    \"id\": \"ea7046eb-2278-4332-8115-69df89ad0d41\",\r\n                    \"path\": \"<Keyboard>/6\",\r\n                    \"interactions\": \"\",\r\n                    \"processors\": \"\",\r\n                    \"groups\": \";Keyboard Mouse\",\r\n                    \"action\": \"Hotkey 6\",\r\n                    \"isComposite\": false,\r\n                    \"isPartOfComposite\": false\r\n                },\r\n                {\r\n                    \"name\": \"\",\r\n                    \"id\": \"bc64917a-d051-417f-b2a8-b9c4523e1748\",\r\n                    \"path\": \"<Keyboard>/7\",\r\n                    \"interactions\": \"\",\r\n                    \"processors\": \"\",\r\n                    \"groups\": \";Keyboard Mouse\",\r\n                    \"action\": \"Hotkey 7\",\r\n                    \"isComposite\": false,\r\n                    \"isPartOfComposite\": false\r\n                },\r\n                {\r\n                    \"name\": \"\",\r\n                    \"id\": \"0d789e6c-4073-4bd9-b0e0-0aef5385de5a\",\r\n                    \"path\": \"<Keyboard>/8\",\r\n                    \"interactions\": \"\",\r\n                    \"processors\": \"\",\r\n                    \"groups\": \";Keyboard Mouse\",\r\n                    \"action\": \"Hotkey 8\",\r\n                    \"isComposite\": false,\r\n                    \"isPartOfComposite\": false\r\n                },\r\n                {\r\n                    \"name\": \"\",\r\n                    \"id\": \"b337a12d-b738-48c0-983e-0ed7db22532d\",\r\n                    \"path\": \"<Gamepad>/leftShoulder\",\r\n                    \"interactions\": \"\",\r\n                    \"processors\": \"\",\r\n                    \"groups\": \";Gamepad\",\r\n                    \"action\": \"Cycle Left\",\r\n                    \"isComposite\": false,\r\n                    \"isPartOfComposite\": false\r\n                },\r\n                {\r\n                    \"name\": \"\",\r\n                    \"id\": \"ce0aa46b-4230-43a1-9369-87cc321d31c9\",\r\n                    \"path\": \"\",\r\n                    \"interactions\": \"\",\r\n                    \"processors\": \"\",\r\n                    \"groups\": \";Keyboard Mouse\",\r\n                    \"action\": \"Cycle Left\",\r\n                    \"isComposite\": false,\r\n                    \"isPartOfComposite\": false\r\n                },\r\n                {\r\n                    \"name\": \"\",\r\n                    \"id\": \"9d6f1034-1c1d-4053-a4f3-db5a2fa365f1\",\r\n                    \"path\": \"<Gamepad>/rightShoulder\",\r\n                    \"interactions\": \"\",\r\n                    \"processors\": \"\",\r\n                    \"groups\": \";Gamepad\",\r\n                    \"action\": \"Cycle Right\",\r\n                    \"isComposite\": false,\r\n                    \"isPartOfComposite\": false\r\n                },\r\n                {\r\n                    \"name\": \"\",\r\n                    \"id\": \"61d8d355-23c3-4bb2-b90d-2e73a823d404\",\r\n                    \"path\": \"<Keyboard>/q\",\r\n                    \"interactions\": \"\",\r\n                    \"processors\": \"\",\r\n                    \"groups\": \";Keyboard Mouse\",\r\n                    \"action\": \"Cycle Right\",\r\n                    \"isComposite\": false,\r\n                    \"isPartOfComposite\": false\r\n                },\r\n                {\r\n                    \"name\": \"\",\r\n                    \"id\": \"ca3bfda3-5f69-46c1-a75f-4359296b903b\",\r\n                    \"path\": \"\",\r\n                    \"interactions\": \"\",\r\n                    \"processors\": \"\",\r\n                    \"groups\": \";Gamepad\",\r\n                    \"action\": \"Toggle\",\r\n                    \"isComposite\": false,\r\n                    \"isPartOfComposite\": false\r\n                },\r\n                {\r\n                    \"name\": \"\",\r\n                    \"id\": \"a09093ab-88ce-453a-9057-facc473e4fe2\",\r\n                    \"path\": \"\",\r\n                    \"interactions\": \"\",\r\n                    \"processors\": \"\",\r\n                    \"groups\": \";Keyboard Mouse\",\r\n                    \"action\": \"Toggle\",\r\n                    \"isComposite\": false,\r\n                    \"isPartOfComposite\": false\r\n                }\r\n            ]\r\n        },\r\n        {\r\n            \"name\": \"Spectate\",\r\n            \"id\": \"63b711a4-ce2c-457b-a1ce-62e4266d5157\",\r\n            \"actions\": [\r\n                {\r\n                    \"name\": \"Cycle Previous Player\",\r\n                    \"type\": \"Value\",\r\n                    \"id\": \"22dddb9b-bff1-4d59-9fca-c615cbbf144b\",\r\n                    \"expectedControlType\": \"Axis\",\r\n                    \"processors\": \"\",\r\n                    \"interactions\": \"\",\r\n                    \"initialStateCheck\": true\r\n                },\r\n                {\r\n                    \"name\": \"Cycle Next Player\",\r\n                    \"type\": \"Value\",\r\n                    \"id\": \"93a1b4ac-9e14-4763-ba89-5be39957524a\",\r\n                    \"expectedControlType\": \"Axis\",\r\n                    \"processors\": \"\",\r\n                    \"interactions\": \"\",\r\n                    \"initialStateCheck\": true\r\n                }\r\n            ],\r\n            \"bindings\": [\r\n                {\r\n                    \"name\": \"\",\r\n                    \"id\": \"1b235829-990a-4f80-859b-a8cf48d0190f\",\r\n                    \"path\": \"<Mouse>/leftButton\",\r\n                    \"interactions\": \"\",\r\n                    \"processors\": \"\",\r\n                    \"groups\": \";Keyboard Mouse\",\r\n                    \"action\": \"Cycle Next Player\",\r\n                    \"isComposite\": false,\r\n                    \"isPartOfComposite\": false\r\n                },\r\n                {\r\n                    \"name\": \"\",\r\n                    \"id\": \"5f4bee6d-cbe9-4407-bcf0-046c5870e2f4\",\r\n                    \"path\": \"<Gamepad>/rightShoulder\",\r\n                    \"interactions\": \"\",\r\n                    \"processors\": \"\",\r\n                    \"groups\": \";Gamepad\",\r\n                    \"action\": \"Cycle Next Player\",\r\n                    \"isComposite\": false,\r\n                    \"isPartOfComposite\": false\r\n                },\r\n                {\r\n                    \"name\": \"\",\r\n                    \"id\": \"3637e8e6-abcf-4d54-99b6-1abfaabd7ad8\",\r\n                    \"path\": \"<Mouse>/rightButton\",\r\n                    \"interactions\": \"\",\r\n                    \"processors\": \"\",\r\n                    \"groups\": \";Keyboard Mouse\",\r\n                    \"action\": \"Cycle Previous Player\",\r\n                    \"isComposite\": false,\r\n                    \"isPartOfComposite\": false\r\n                },\r\n                {\r\n                    \"name\": \"\",\r\n                    \"id\": \"081cbb8b-0c7a-499c-b559-8683d495d242\",\r\n                    \"path\": \"<Gamepad>/leftShoulder\",\r\n                    \"interactions\": \"\",\r\n                    \"processors\": \"\",\r\n                    \"groups\": \";Gamepad\",\r\n                    \"action\": \"Cycle Previous Player\",\r\n                    \"isComposite\": false,\r\n                    \"isPartOfComposite\": false\r\n                }\r\n            ]\r\n        },\r\n        {\r\n            \"name\": \"Ingame\",\r\n            \"id\": \"3f198e88-d2e6-48b2-b30e-0bf3d8d742a7\",\r\n            \"actions\": [\r\n                {\r\n                    \"name\": \"Show Scoreboard\",\r\n                    \"type\": \"Button\",\r\n                    \"id\": \"51fe412e-aadd-4ed5-b70c-86e2f5a10bc2\",\r\n                    \"expectedControlType\": \"\",\r\n                    \"processors\": \"\",\r\n                    \"interactions\": \"\",\r\n                    \"initialStateCheck\": false\r\n                },\r\n                {\r\n                    \"name\": \"Pause\",\r\n                    \"type\": \"Button\",\r\n                    \"id\": \"94e16d4d-adbd-44f5-891c-55465efe190f\",\r\n                    \"expectedControlType\": \"\",\r\n                    \"processors\": \"\",\r\n                    \"interactions\": \"\",\r\n                    \"initialStateCheck\": false\r\n                },\r\n                {\r\n                    \"name\": \"Open Chat\",\r\n                    \"type\": \"Button\",\r\n                    \"id\": \"e5a3c4ca-1017-4761-b1d3-1da9797fe010\",\r\n                    \"expectedControlType\": \"\",\r\n                    \"processors\": \"\",\r\n                    \"interactions\": \"\",\r\n                    \"initialStateCheck\": false\r\n                },\r\n                {\r\n                    \"name\": \"Toggle Emote Menu\",\r\n                    \"type\": \"Button\",\r\n                    \"id\": \"01b9b560-a054-488b-a855-2185074cf94c\",\r\n                    \"expectedControlType\": \"\",\r\n                    \"processors\": \"\",\r\n                    \"interactions\": \"\",\r\n                    \"initialStateCheck\": false\r\n                }\r\n            ],\r\n            \"bindings\": [\r\n                {\r\n                    \"name\": \"\",\r\n                    \"id\": \"3a7ee91e-5ee2-49b3-9337-d56b20189bcf\",\r\n                    \"path\": \"<Keyboard>/tab\",\r\n                    \"interactions\": \"\",\r\n                    \"processors\": \"\",\r\n                    \"groups\": \";Keyboard Mouse\",\r\n                    \"action\": \"Show Scoreboard\",\r\n                    \"isComposite\": false,\r\n                    \"isPartOfComposite\": false\r\n                },\r\n                {\r\n                    \"name\": \"\",\r\n                    \"id\": \"7d093edf-4e31-48f5-bb1a-c77a9ba44178\",\r\n                    \"path\": \"<Gamepad>/select\",\r\n                    \"interactions\": \"\",\r\n                    \"processors\": \"\",\r\n                    \"groups\": \";Gamepad\",\r\n                    \"action\": \"Show Scoreboard\",\r\n                    \"isComposite\": false,\r\n                    \"isPartOfComposite\": false\r\n                },\r\n                {\r\n                    \"name\": \"\",\r\n                    \"id\": \"456c8391-5af1-4b3d-8168-1228f610d568\",\r\n                    \"path\": \"<Keyboard>/escape\",\r\n                    \"interactions\": \"\",\r\n                    \"processors\": \"\",\r\n                    \"groups\": \";Keyboard Mouse\",\r\n                    \"action\": \"Pause\",\r\n                    \"isComposite\": false,\r\n                    \"isPartOfComposite\": false\r\n                },\r\n                {\r\n                    \"name\": \"\",\r\n                    \"id\": \"950f1f4d-2720-45b6-8604-fccc65b88118\",\r\n                    \"path\": \"<Gamepad>/start\",\r\n                    \"interactions\": \"\",\r\n                    \"processors\": \"\",\r\n                    \"groups\": \";Gamepad\",\r\n                    \"action\": \"Pause\",\r\n                    \"isComposite\": false,\r\n                    \"isPartOfComposite\": false\r\n                },\r\n                {\r\n                    \"name\": \"\",\r\n                    \"id\": \"304881e8-8637-455d-a104-393f11d12433\",\r\n                    \"path\": \"<Keyboard>/enter\",\r\n                    \"interactions\": \"\",\r\n                    \"processors\": \"\",\r\n                    \"groups\": \";Keyboard Mouse\",\r\n                    \"action\": \"Open Chat\",\r\n                    \"isComposite\": false,\r\n                    \"isPartOfComposite\": false\r\n                },\r\n                {\r\n                    \"name\": \"\",\r\n                    \"id\": \"10bb7796-7384-4275-9710-794eed99bb1a\",\r\n                    \"path\": \"\",\r\n                    \"interactions\": \"\",\r\n                    \"processors\": \"\",\r\n                    \"groups\": \";Gamepad\",\r\n                    \"action\": \"Open Chat\",\r\n                    \"isComposite\": false,\r\n                    \"isPartOfComposite\": false\r\n                },\r\n                {\r\n                    \"name\": \"\",\r\n                    \"id\": \"387c6e50-859b-4d76-827a-c02961bde94f\",\r\n                    \"path\": \"<Keyboard>/t\",\r\n                    \"interactions\": \"\",\r\n                    \"processors\": \"\",\r\n                    \"groups\": \";Keyboard Mouse\",\r\n                    \"action\": \"Toggle Emote Menu\",\r\n                    \"isComposite\": false,\r\n                    \"isPartOfComposite\": false\r\n                },\r\n                {\r\n                    \"name\": \"\",\r\n                    \"id\": \"af1b4cc9-7e04-4b44-889a-8885ff7885e9\",\r\n                    \"path\": \"<Gamepad>/dpad/right\",\r\n                    \"interactions\": \"\",\r\n                    \"processors\": \"\",\r\n                    \"groups\": \";Gamepad\",\r\n                    \"action\": \"Toggle Emote Menu\",\r\n                    \"isComposite\": false,\r\n                    \"isPartOfComposite\": false\r\n                }\r\n            ]\r\n        },\r\n        {\r\n            \"name\": \"Voice Chat\",\r\n            \"id\": \"176dd6ea-9620-4367-ab64-99bd83523165\",\r\n            \"actions\": [\r\n                {\r\n                    \"name\": \"Push To Talk\",\r\n                    \"type\": \"Button\",\r\n                    \"id\": \"3ef31535-a1be-4855-aeee-a9ca7582cd09\",\r\n                    \"expectedControlType\": \"\",\r\n                    \"processors\": \"\",\r\n                    \"interactions\": \"\",\r\n                    \"initialStateCheck\": false\r\n                }\r\n            ],\r\n            \"bindings\": [\r\n                {\r\n                    \"name\": \"\",\r\n                    \"id\": \"57776d6c-dee3-48ca-b0f1-2d36b5930851\",\r\n                    \"path\": \"<Keyboard>/v\",\r\n                    \"interactions\": \"\",\r\n                    \"processors\": \"\",\r\n                    \"groups\": \";Keyboard Mouse\",\r\n                    \"action\": \"Push To Talk\",\r\n                    \"isComposite\": false,\r\n                    \"isPartOfComposite\": false\r\n                },\r\n                {\r\n                    \"name\": \"\",\r\n                    \"id\": \"04bd1edd-6e43-4cf7-97ad-b87534e136e6\",\r\n                    \"path\": \"<Gamepad>/dpad/up\",\r\n                    \"interactions\": \"\",\r\n                    \"processors\": \"\",\r\n                    \"groups\": \";Gamepad\",\r\n                    \"action\": \"Push To Talk\",\r\n                    \"isComposite\": false,\r\n                    \"isPartOfComposite\": false\r\n                }\r\n            ]\r\n        },\r\n        {\r\n            \"name\": \"Camera\",\r\n            \"id\": \"2b4555d2-64d9-4e4b-b680-d45b38480556\",\r\n            \"actions\": [\r\n                {\r\n                    \"name\": \"Look\",\r\n                    \"type\": \"Value\",\r\n                    \"id\": \"72e273cd-a4dc-48b8-a724-102e50463960\",\r\n                    \"expectedControlType\": \"Vector2\",\r\n                    \"processors\": \"\",\r\n                    \"interactions\": \"\",\r\n                    \"initialStateCheck\": true\r\n                }\r\n            ],\r\n            \"bindings\": [\r\n                {\r\n                    \"name\": \"\",\r\n                    \"id\": \"54b6c168-8740-42fd-ae6e-eda16321b573\",\r\n                    \"path\": \"<Gamepad>/rightStick\",\r\n                    \"interactions\": \"\",\r\n                    \"processors\": \"StickDeadzone,VectorSensitivityScale\",\r\n                    \"groups\": \"Gamepad\",\r\n                    \"action\": \"Look\",\r\n                    \"isComposite\": false,\r\n                    \"isPartOfComposite\": false\r\n                },\r\n                {\r\n                    \"name\": \"\",\r\n                    \"id\": \"509518a3-f7b1-4144-8d64-acc292b9c716\",\r\n                    \"path\": \"<Mouse>/delta\",\r\n                    \"interactions\": \"\",\r\n                    \"processors\": \"ScaleVector2(x=0.001,y=0.001)\",\r\n                    \"groups\": \";Keyboard Mouse\",\r\n                    \"action\": \"Look\",\r\n                    \"isComposite\": false,\r\n                    \"isPartOfComposite\": false\r\n                }\r\n            ]\r\n        },\r\n        {\r\n            \"name\": \"Radial Menu\",\r\n            \"id\": \"6891db2f-c483-49f7-a4d5-335cd8d70769\",\r\n            \"actions\": [\r\n                {\r\n                    \"name\": \"Select\",\r\n                    \"type\": \"Button\",\r\n                    \"id\": \"c594719d-7268-480a-b671-ee43aecd40ea\",\r\n                    \"expectedControlType\": \"\",\r\n                    \"processors\": \"\",\r\n                    \"interactions\": \"\",\r\n                    \"initialStateCheck\": false\r\n                },\r\n                {\r\n                    \"name\": \"Cancel\",\r\n                    \"type\": \"Button\",\r\n                    \"id\": \"5b2ad9e1-b692-488f-ada3-758919de6b6b\",\r\n                    \"expectedControlType\": \"\",\r\n                    \"processors\": \"\",\r\n                    \"interactions\": \"\",\r\n                    \"initialStateCheck\": false\r\n                }\r\n            ],\r\n            \"bindings\": [\r\n                {\r\n                    \"name\": \"\",\r\n                    \"id\": \"db1dce5a-da25-4b19-980c-419e5fab5d3a\",\r\n                    \"path\": \"<Mouse>/leftButton\",\r\n                    \"interactions\": \"\",\r\n                    \"processors\": \"\",\r\n                    \"groups\": \";Keyboard Mouse\",\r\n                    \"action\": \"Select\",\r\n                    \"isComposite\": false,\r\n                    \"isPartOfComposite\": false\r\n                },\r\n                {\r\n                    \"name\": \"\",\r\n                    \"id\": \"d262ab93-85e2-4ac0-af0a-9aee9f51c99a\",\r\n                    \"path\": \"<Gamepad>/buttonSouth\",\r\n                    \"interactions\": \"\",\r\n                    \"processors\": \"\",\r\n                    \"groups\": \";Gamepad\",\r\n                    \"action\": \"Select\",\r\n                    \"isComposite\": false,\r\n                    \"isPartOfComposite\": false\r\n                },\r\n                {\r\n                    \"name\": \"\",\r\n                    \"id\": \"5ffb2638-58cb-4269-b36a-a6d33e2b0f36\",\r\n                    \"path\": \"<Keyboard>/escape\",\r\n                    \"interactions\": \"\",\r\n                    \"processors\": \"\",\r\n                    \"groups\": \";Keyboard Mouse\",\r\n                    \"action\": \"Cancel\",\r\n                    \"isComposite\": false,\r\n                    \"isPartOfComposite\": false\r\n                },\r\n                {\r\n                    \"name\": \"\",\r\n                    \"id\": \"bdaf7db0-c024-49b1-855f-05864d8b0453\",\r\n                    \"path\": \"<Gamepad>/buttonEast\",\r\n                    \"interactions\": \"\",\r\n                    \"processors\": \"\",\r\n                    \"groups\": \";Gamepad\",\r\n                    \"action\": \"Cancel\",\r\n                    \"isComposite\": false,\r\n                    \"isPartOfComposite\": false\r\n                }\r\n            ]\r\n        },\r\n        {\r\n            \"name\": \"UI\",\r\n            \"id\": \"960a3729-b8f2-4ed4-8313-1df4e6cd7ff2\",\r\n            \"actions\": [\r\n                {\r\n                    \"name\": \"Navigate\",\r\n                    \"type\": \"PassThrough\",\r\n                    \"id\": \"bf686cb4-088a-4ebc-a519-2e4b2e53d6b9\",\r\n                    \"expectedControlType\": \"Vector2\",\r\n                    \"processors\": \"\",\r\n                    \"interactions\": \"\",\r\n                    \"initialStateCheck\": true\r\n                },\r\n                {\r\n                    \"name\": \"Submit\",\r\n                    \"type\": \"Button\",\r\n                    \"id\": \"5da3d3ca-5592-427e-ba2a-3847c5a3dd5f\",\r\n                    \"expectedControlType\": \"\",\r\n                    \"processors\": \"\",\r\n                    \"interactions\": \"\",\r\n                    \"initialStateCheck\": false\r\n                },\r\n                {\r\n                    \"name\": \"Cancel\",\r\n                    \"type\": \"Button\",\r\n                    \"id\": \"c639914a-237b-4977-857d-14bb5f43bcd5\",\r\n                    \"expectedControlType\": \"\",\r\n                    \"processors\": \"\",\r\n                    \"interactions\": \"\",\r\n                    \"initialStateCheck\": false\r\n                },\r\n                {\r\n                    \"name\": \"Point\",\r\n                    \"type\": \"PassThrough\",\r\n                    \"id\": \"15d80fc6-8cc7-4f24-a554-e50a10e88252\",\r\n                    \"expectedControlType\": \"Vector2\",\r\n                    \"processors\": \"\",\r\n                    \"interactions\": \"\",\r\n                    \"initialStateCheck\": false\r\n                },\r\n                {\r\n                    \"name\": \"Click\",\r\n                    \"type\": \"PassThrough\",\r\n                    \"id\": \"0b314b26-bbcc-431c-8f75-72c289ac719c\",\r\n                    \"expectedControlType\": \"Button\",\r\n                    \"processors\": \"\",\r\n                    \"interactions\": \"\",\r\n                    \"initialStateCheck\": false\r\n                },\r\n                {\r\n                    \"name\": \"Scroll Wheel\",\r\n                    \"type\": \"PassThrough\",\r\n                    \"id\": \"349a53c6-c9a0-4994-8b29-74f0c67a9853\",\r\n                    \"expectedControlType\": \"Vector2\",\r\n                    \"processors\": \"\",\r\n                    \"interactions\": \"\",\r\n                    \"initialStateCheck\": false\r\n                },\r\n                {\r\n                    \"name\": \"Middle Click\",\r\n                    \"type\": \"PassThrough\",\r\n                    \"id\": \"b2b41075-77c7-4afe-80e0-6d381e2f9bf1\",\r\n                    \"expectedControlType\": \"\",\r\n                    \"processors\": \"\",\r\n                    \"interactions\": \"\",\r\n                    \"initialStateCheck\": false\r\n                },\r\n                {\r\n                    \"name\": \"Right Click\",\r\n                    \"type\": \"PassThrough\",\r\n                    \"id\": \"ada08af7-c5d0-4880-b436-3fea59025767\",\r\n                    \"expectedControlType\": \"\",\r\n                    \"processors\": \"\",\r\n                    \"interactions\": \"\",\r\n                    \"initialStateCheck\": false\r\n                },\r\n                {\r\n                    \"name\": \"Exit\",\r\n                    \"type\": \"Button\",\r\n                    \"id\": \"a7f9a474-4b27-43e2-9931-5e878f0c8ef7\",\r\n                    \"expectedControlType\": \"\",\r\n                    \"processors\": \"\",\r\n                    \"interactions\": \"\",\r\n                    \"initialStateCheck\": false\r\n                },\r\n                {\r\n                    \"name\": \"VirtualCursorMove\",\r\n                    \"type\": \"Value\",\r\n                    \"id\": \"5b34e483-cbde-4ded-918e-4c6512b40f03\",\r\n                    \"expectedControlType\": \"Vector2\",\r\n                    \"processors\": \"\",\r\n                    \"interactions\": \"\",\r\n                    \"initialStateCheck\": true\r\n                },\r\n                {\r\n                    \"name\": \"VirtualCursorClick\",\r\n                    \"type\": \"Button\",\r\n                    \"id\": \"6f783ec7-603c-421c-8d5d-fa06240d7f84\",\r\n                    \"expectedControlType\": \"\",\r\n                    \"processors\": \"\",\r\n                    \"interactions\": \"\",\r\n                    \"initialStateCheck\": false\r\n                },\r\n                {\r\n                    \"name\": \"VirtualCursorScrollWheel\",\r\n                    \"type\": \"Value\",\r\n                    \"id\": \"f9f8e5c9-37d3-479e-99f8-a1e45d0a2495\",\r\n                    \"expectedControlType\": \"Vector2\",\r\n                    \"processors\": \"\",\r\n                    \"interactions\": \"\",\r\n                    \"initialStateCheck\": true\r\n                }\r\n            ],\r\n            \"bindings\": [\r\n                {\r\n                    \"name\": \"\",\r\n                    \"id\": \"8fd546de-e178-437d-b939-5676b20b4f5b\",\r\n                    \"path\": \"<Mouse>/rightButton\",\r\n                    \"interactions\": \"\",\r\n                    \"processors\": \"\",\r\n                    \"groups\": \"Keyboard&Mouse;Keyboard Mouse\",\r\n                    \"action\": \"Right Click\",\r\n                    \"isComposite\": false,\r\n                    \"isPartOfComposite\": false\r\n                },\r\n                {\r\n                    \"name\": \"\",\r\n                    \"id\": \"ae2d22cf-c485-4297-ab5f-d0f8a37a71bd\",\r\n                    \"path\": \"<Gamepad>/dpad\",\r\n                    \"interactions\": \"\",\r\n                    \"processors\": \"\",\r\n                    \"groups\": \";Gamepad\",\r\n                    \"action\": \"Navigate\",\r\n                    \"isComposite\": false,\r\n                    \"isPartOfComposite\": false\r\n                },\r\n                {\r\n                    \"name\": \"Joystick\",\r\n                    \"id\": \"a712b300-1f23-48c6-8a22-d8150356f304\",\r\n                    \"path\": \"2DVector\",\r\n                    \"interactions\": \"\",\r\n                    \"processors\": \"StickDeadzone\",\r\n                    \"groups\": \"\",\r\n                    \"action\": \"Navigate\",\r\n                    \"isComposite\": true,\r\n                    \"isPartOfComposite\": false\r\n                },\r\n                {\r\n                    \"name\": \"up\",\r\n                    \"id\": \"b1653739-568e-4419-9dbd-151a10a2c26d\",\r\n                    \"path\": \"<Joystick>/stick/up\",\r\n                    \"interactions\": \"\",\r\n                    \"processors\": \"\",\r\n                    \"groups\": \"Joystick;Gamepad\",\r\n                    \"action\": \"Navigate\",\r\n                    \"isComposite\": false,\r\n                    \"isPartOfComposite\": true\r\n                },\r\n                {\r\n                    \"name\": \"down\",\r\n                    \"id\": \"327af56a-bce8-4c60-8d9f-55dccef0a656\",\r\n                    \"path\": \"<Joystick>/stick/down\",\r\n                    \"interactions\": \"\",\r\n                    \"processors\": \"\",\r\n                    \"groups\": \"Joystick;Gamepad\",\r\n                    \"action\": \"Navigate\",\r\n                    \"isComposite\": false,\r\n                    \"isPartOfComposite\": true\r\n                },\r\n                {\r\n                    \"name\": \"left\",\r\n                    \"id\": \"7e6d6516-6b8b-4eb4-b8a6-2694450ec4f9\",\r\n                    \"path\": \"<Joystick>/stick/left\",\r\n                    \"interactions\": \"\",\r\n                    \"processors\": \"\",\r\n                    \"groups\": \"Joystick;Gamepad\",\r\n                    \"action\": \"Navigate\",\r\n                    \"isComposite\": false,\r\n                    \"isPartOfComposite\": true\r\n                },\r\n                {\r\n                    \"name\": \"right\",\r\n                    \"id\": \"21e56af9-26dc-4eec-9d99-90ce86568df4\",\r\n                    \"path\": \"<Joystick>/stick/right\",\r\n                    \"interactions\": \"\",\r\n                    \"processors\": \"\",\r\n                    \"groups\": \"Joystick;Gamepad\",\r\n                    \"action\": \"Navigate\",\r\n                    \"isComposite\": false,\r\n                    \"isPartOfComposite\": true\r\n                },\r\n                {\r\n                    \"name\": \"Keyboard\",\r\n                    \"id\": \"c4a48a6e-02aa-45a9-9f6a-e58c47cd2d7a\",\r\n                    \"path\": \"2DVector\",\r\n                    \"interactions\": \"\",\r\n                    \"processors\": \"\",\r\n                    \"groups\": \"\",\r\n                    \"action\": \"Navigate\",\r\n                    \"isComposite\": true,\r\n                    \"isPartOfComposite\": false\r\n                },\r\n                {\r\n                    \"name\": \"up\",\r\n                    \"id\": \"92c4b623-0ba3-49ae-a110-ab4822e96635\",\r\n                    \"path\": \"<Keyboard>/upArrow\",\r\n                    \"interactions\": \"\",\r\n                    \"processors\": \"\",\r\n                    \"groups\": \"Keyboard&Mouse;Keyboard Mouse\",\r\n                    \"action\": \"Navigate\",\r\n                    \"isComposite\": false,\r\n                    \"isPartOfComposite\": true\r\n                },\r\n                {\r\n                    \"name\": \"down\",\r\n                    \"id\": \"a8e4b69a-9429-41e3-815b-234e7c328e91\",\r\n                    \"path\": \"<Keyboard>/downArrow\",\r\n                    \"interactions\": \"\",\r\n                    \"processors\": \"\",\r\n                    \"groups\": \"Keyboard&Mouse;Keyboard Mouse\",\r\n                    \"action\": \"Navigate\",\r\n                    \"isComposite\": false,\r\n                    \"isPartOfComposite\": true\r\n                },\r\n                {\r\n                    \"name\": \"left\",\r\n                    \"id\": \"a1f33c33-32e8-4c2f-a191-f87a343e0e45\",\r\n                    \"path\": \"<Keyboard>/leftArrow\",\r\n                    \"interactions\": \"\",\r\n                    \"processors\": \"\",\r\n                    \"groups\": \"Keyboard&Mouse;Keyboard Mouse\",\r\n                    \"action\": \"Navigate\",\r\n                    \"isComposite\": false,\r\n                    \"isPartOfComposite\": true\r\n                },\r\n                {\r\n                    \"name\": \"right\",\r\n                    \"id\": \"a1828b93-27bf-417a-9118-ba21016d46e1\",\r\n                    \"path\": \"<Keyboard>/rightArrow\",\r\n                    \"interactions\": \"\",\r\n                    \"processors\": \"\",\r\n                    \"groups\": \"Keyboard&Mouse;Keyboard Mouse\",\r\n                    \"action\": \"Navigate\",\r\n                    \"isComposite\": false,\r\n                    \"isPartOfComposite\": true\r\n                },\r\n                {\r\n                    \"name\": \"\",\r\n                    \"id\": \"9bb3288a-d0c1-4205-b0f7-1290676de091\",\r\n                    \"path\": \"<Keyboard>/space\",\r\n                    \"interactions\": \"\",\r\n                    \"processors\": \"\",\r\n                    \"groups\": \"Keyboard Mouse\",\r\n                    \"action\": \"Submit\",\r\n                    \"isComposite\": false,\r\n                    \"isPartOfComposite\": false\r\n                },\r\n                {\r\n                    \"name\": \"\",\r\n                    \"id\": \"b0895c00-8aa2-4ecf-addc-54e3710f5b7a\",\r\n                    \"path\": \"<Keyboard>/enter\",\r\n                    \"interactions\": \"\",\r\n                    \"processors\": \"\",\r\n                    \"groups\": \"Keyboard Mouse\",\r\n                    \"action\": \"Submit\",\r\n                    \"isComposite\": false,\r\n                    \"isPartOfComposite\": false\r\n                },\r\n                {\r\n                    \"name\": \"\",\r\n                    \"id\": \"78a80c12-88a0-4949-8654-ceb128244133\",\r\n                    \"path\": \"<Gamepad>/buttonEast\",\r\n                    \"interactions\": \"\",\r\n                    \"processors\": \"\",\r\n                    \"groups\": \"Gamepad\",\r\n                    \"action\": \"Cancel\",\r\n                    \"isComposite\": false,\r\n                    \"isPartOfComposite\": false\r\n                },\r\n                {\r\n                    \"name\": \"\",\r\n                    \"id\": \"bfb94ebf-851e-4f86-9d7e-1ca37822322f\",\r\n                    \"path\": \"<Keyboard>/escape\",\r\n                    \"interactions\": \"\",\r\n                    \"processors\": \"\",\r\n                    \"groups\": \"Keyboard Mouse\",\r\n                    \"action\": \"Cancel\",\r\n                    \"isComposite\": false,\r\n                    \"isPartOfComposite\": false\r\n                },\r\n                {\r\n                    \"name\": \"\",\r\n                    \"id\": \"c016a3a0-9046-4f13-a325-ae15b0a1094c\",\r\n                    \"path\": \"<Mouse>/position\",\r\n                    \"interactions\": \"\",\r\n                    \"processors\": \"\",\r\n                    \"groups\": \"Keyboard&Mouse;Keyboard Mouse;Gamepad\",\r\n                    \"action\": \"Point\",\r\n                    \"isComposite\": false,\r\n                    \"isPartOfComposite\": false\r\n                },\r\n                {\r\n                    \"name\": \"\",\r\n                    \"id\": \"b303d2cd-2a59-485b-add6-12a250586bb9\",\r\n                    \"path\": \"<Mouse>/scroll\",\r\n                    \"interactions\": \"\",\r\n                    \"processors\": \"\",\r\n                    \"groups\": \"Keyboard&Mouse;Keyboard Mouse\",\r\n                    \"action\": \"Scroll Wheel\",\r\n                    \"isComposite\": false,\r\n                    \"isPartOfComposite\": false\r\n                },\r\n                {\r\n                    \"name\": \"\",\r\n                    \"id\": \"21f7ba94-52f1-4720-a506-b59404244a22\",\r\n                    \"path\": \"<Mouse>/middleButton\",\r\n                    \"interactions\": \"\",\r\n                    \"processors\": \"\",\r\n                    \"groups\": \"Keyboard&Mouse;Keyboard Mouse\",\r\n                    \"action\": \"Middle Click\",\r\n                    \"isComposite\": false,\r\n                    \"isPartOfComposite\": false\r\n                },\r\n                {\r\n                    \"name\": \"\",\r\n                    \"id\": \"3c44cfda-d3d5-4622-9a00-158d475b5c1f\",\r\n                    \"path\": \"<Gamepad>/start\",\r\n                    \"interactions\": \"\",\r\n                    \"processors\": \"\",\r\n                    \"groups\": \"Gamepad\",\r\n                    \"action\": \"Exit\",\r\n                    \"isComposite\": false,\r\n                    \"isPartOfComposite\": false\r\n                },\r\n                {\r\n                    \"name\": \"\",\r\n                    \"id\": \"cc554c69-17ce-4c96-a2d9-058b4b9947c6\",\r\n                    \"path\": \"<Keyboard>/escape\",\r\n                    \"interactions\": \"\",\r\n                    \"processors\": \"\",\r\n                    \"groups\": \"Keyboard Mouse\",\r\n                    \"action\": \"Exit\",\r\n                    \"isComposite\": false,\r\n                    \"isPartOfComposite\": false\r\n                },\r\n                {\r\n                    \"name\": \"Gamepad\",\r\n                    \"id\": \"538b4246-da08-4d45-9e3b-071e56fd0ba4\",\r\n                    \"path\": \"2DVector(mode=2)\",\r\n                    \"interactions\": \"\",\r\n                    \"processors\": \"StickDeadzone,VectorSensitivityScale\",\r\n                    \"groups\": \"\",\r\n                    \"action\": \"VirtualCursorMove\",\r\n                    \"isComposite\": true,\r\n                    \"isPartOfComposite\": false\r\n                },\r\n                {\r\n                    \"name\": \"up\",\r\n                    \"id\": \"153eb79b-293c-42c6-b66e-5b53361be64a\",\r\n                    \"path\": \"<Gamepad>/leftStick/up\",\r\n                    \"interactions\": \"\",\r\n                    \"processors\": \"\",\r\n                    \"groups\": \"Gamepad\",\r\n                    \"action\": \"VirtualCursorMove\",\r\n                    \"isComposite\": false,\r\n                    \"isPartOfComposite\": true\r\n                },\r\n                {\r\n                    \"name\": \"down\",\r\n                    \"id\": \"44b01b66-c201-4e67-bcf7-01d949527758\",\r\n                    \"path\": \"<Gamepad>/leftStick/down\",\r\n                    \"interactions\": \"\",\r\n                    \"processors\": \"\",\r\n                    \"groups\": \"Gamepad\",\r\n                    \"action\": \"VirtualCursorMove\",\r\n                    \"isComposite\": false,\r\n                    \"isPartOfComposite\": true\r\n                },\r\n                {\r\n                    \"name\": \"left\",\r\n                    \"id\": \"8204697f-863f-4575-b9e6-ae7dc08a500f\",\r\n                    \"path\": \"<Gamepad>/leftStick/left\",\r\n                    \"interactions\": \"\",\r\n                    \"processors\": \"\",\r\n                    \"groups\": \"Gamepad\",\r\n                    \"action\": \"VirtualCursorMove\",\r\n                    \"isComposite\": false,\r\n                    \"isPartOfComposite\": true\r\n                },\r\n                {\r\n                    \"name\": \"right\",\r\n                    \"id\": \"d5019c19-35b2-412a-bd51-441b98eb6571\",\r\n                    \"path\": \"<Gamepad>/leftStick/right\",\r\n                    \"interactions\": \"\",\r\n                    \"processors\": \"\",\r\n                    \"groups\": \"Gamepad\",\r\n                    \"action\": \"VirtualCursorMove\",\r\n                    \"isComposite\": false,\r\n                    \"isPartOfComposite\": true\r\n                },\r\n                {\r\n                    \"name\": \"\",\r\n                    \"id\": \"fe6817e8-ae5a-4e4d-aa6a-c6230628e9fc\",\r\n                    \"path\": \"<Gamepad>/rightStick\",\r\n                    \"interactions\": \"\",\r\n                    \"processors\": \"StickDeadzone\",\r\n                    \"groups\": \"Gamepad\",\r\n                    \"action\": \"VirtualCursorScrollWheel\",\r\n                    \"isComposite\": false,\r\n                    \"isPartOfComposite\": false\r\n                },\r\n                {\r\n                    \"name\": \"\",\r\n                    \"id\": \"6b347a75-a92c-4e08-8f63-e205e54d6891\",\r\n                    \"path\": \"<Mouse>/leftButton\",\r\n                    \"interactions\": \"\",\r\n                    \"processors\": \"\",\r\n                    \"groups\": \"Keyboard Mouse\",\r\n                    \"action\": \"Click\",\r\n                    \"isComposite\": false,\r\n                    \"isPartOfComposite\": false\r\n                },\r\n                {\r\n                    \"name\": \"\",\r\n                    \"id\": \"1c6c8856-b277-482a-8fae-f64fecafcc05\",\r\n                    \"path\": \"<Gamepad>/buttonSouth\",\r\n                    \"interactions\": \"\",\r\n                    \"processors\": \"\",\r\n                    \"groups\": \"Gamepad\",\r\n                    \"action\": \"VirtualCursorClick\",\r\n                    \"isComposite\": false,\r\n                    \"isPartOfComposite\": false\r\n                }\r\n            ]\r\n        },\r\n        {\r\n            \"name\": \"Vote\",\r\n            \"id\": \"e59684a0-7c28-42c8-ab85-0dd1a610134f\",\r\n            \"actions\": [\r\n                {\r\n                    \"name\": \"Yes\",\r\n                    \"type\": \"Button\",\r\n                    \"id\": \"d5123612-e6eb-4910-8566-b9f876452bb8\",\r\n                    \"expectedControlType\": \"\",\r\n                    \"processors\": \"\",\r\n                    \"interactions\": \"\",\r\n                    \"initialStateCheck\": false\r\n                },\r\n                {\r\n                    \"name\": \"No\",\r\n                    \"type\": \"Button\",\r\n                    \"id\": \"73c06731-77d8-43bf-af22-852c861fb1fa\",\r\n                    \"expectedControlType\": \"\",\r\n                    \"processors\": \"\",\r\n                    \"interactions\": \"\",\r\n                    \"initialStateCheck\": false\r\n                }\r\n            ],\r\n            \"bindings\": [\r\n                {\r\n                    \"name\": \"\",\r\n                    \"id\": \"875a15b3-a1e7-4276-9c52-529ec9dbda7e\",\r\n                    \"path\": \"<Keyboard>/f1\",\r\n                    \"interactions\": \"\",\r\n                    \"processors\": \"\",\r\n                    \"groups\": \";Keyboard Mouse\",\r\n                    \"action\": \"Yes\",\r\n                    \"isComposite\": false,\r\n                    \"isPartOfComposite\": false\r\n                },\r\n                {\r\n                    \"name\": \"\",\r\n                    \"id\": \"58744fae-4ca2-4579-8052-61c19583b7e6\",\r\n                    \"path\": \"<Gamepad>/dpad/right\",\r\n                    \"interactions\": \"\",\r\n                    \"processors\": \"\",\r\n                    \"groups\": \";Gamepad\",\r\n                    \"action\": \"Yes\",\r\n                    \"isComposite\": false,\r\n                    \"isPartOfComposite\": false\r\n                },\r\n                {\r\n                    \"name\": \"\",\r\n                    \"id\": \"10a2e842-2ba6-43d6-a80e-119d039da239\",\r\n                    \"path\": \"<Keyboard>/f2\",\r\n                    \"interactions\": \"\",\r\n                    \"processors\": \"\",\r\n                    \"groups\": \";Keyboard Mouse\",\r\n                    \"action\": \"No\",\r\n                    \"isComposite\": false,\r\n                    \"isPartOfComposite\": false\r\n                },\r\n                {\r\n                    \"name\": \"\",\r\n                    \"id\": \"83c36f61-8b35-4909-81a5-e59f67578ff4\",\r\n                    \"path\": \"<Gamepad>/dpad/left\",\r\n                    \"interactions\": \"\",\r\n                    \"processors\": \"\",\r\n                    \"groups\": \";Gamepad\",\r\n                    \"action\": \"No\",\r\n                    \"isComposite\": false,\r\n                    \"isPartOfComposite\": false\r\n                }\r\n            ]\r\n        }\r\n    ],\r\n    \"controlSchemes\": [\r\n        {\r\n            \"name\": \"Gamepad\",\r\n            \"bindingGroup\": \"Gamepad\",\r\n            \"devices\": [\r\n                {\r\n                    \"devicePath\": \"<Gamepad>\",\r\n                    \"isOptional\": false,\r\n                    \"isOR\": false\r\n                },\r\n                {\r\n                    \"devicePath\": \"<VirtualMouse>\",\r\n                    \"isOptional\": false,\r\n                    \"isOR\": false\r\n                }\r\n            ]\r\n        },\r\n        {\r\n            \"name\": \"Keyboard Mouse\",\r\n            \"bindingGroup\": \"Keyboard Mouse\",\r\n            \"devices\": [\r\n                {\r\n                    \"devicePath\": \"<Keyboard>\",\r\n                    \"isOptional\": false,\r\n                    \"isOR\": false\r\n                },\r\n                {\r\n                    \"devicePath\": \"<Mouse>\",\r\n                    \"isOptional\": false,\r\n                    \"isOR\": false\r\n                }\r\n            ]\r\n        }\r\n    ]\r\n}");
		m_Gameplay = asset.FindActionMap("Gameplay", throwIfNotFound: true);
		m_Gameplay_Move = m_Gameplay.FindAction("Move", throwIfNotFound: true);
		m_Gameplay_Jump = m_Gameplay.FindAction("Jump", throwIfNotFound: true);
		m_Gameplay_Aim = m_Gameplay.FindAction("Aim", throwIfNotFound: true);
		m_Gameplay_Swing = m_Gameplay.FindAction("Swing", throwIfNotFound: true);
		m_Gameplay_UseItem = m_Gameplay.FindAction("Use Item", throwIfNotFound: true);
		m_Gameplay_Pitch = m_Gameplay.FindAction("Pitch", throwIfNotFound: true);
		m_Gameplay_Interact = m_Gameplay.FindAction("Interact", throwIfNotFound: true);
		m_Gameplay_Cancel = m_Gameplay.FindAction("Cancel", throwIfNotFound: true);
		m_Gameplay_DropItem = m_Gameplay.FindAction("Drop Item", throwIfNotFound: true);
		m_Gameplay_Dive = m_Gameplay.FindAction("Dive", throwIfNotFound: true);
		m_Gameplay_Walk = m_Gameplay.FindAction("Walk", throwIfNotFound: true);
		m_Gameplay_CycleSwingAngle = m_Gameplay.FindAction("Cycle Swing Angle", throwIfNotFound: true);
		m_Gameplay_Restart = m_Gameplay.FindAction("Restart", throwIfNotFound: true);
		m_GolfCartDriver = asset.FindActionMap("Golf Cart Driver", throwIfNotFound: true);
		m_GolfCartDriver_Accelerate = m_GolfCartDriver.FindAction("Accelerate", throwIfNotFound: true);
		m_GolfCartDriver_Brake = m_GolfCartDriver.FindAction("Brake", throwIfNotFound: true);
		m_GolfCartDriver_Steer = m_GolfCartDriver.FindAction("Steer", throwIfNotFound: true);
		m_GolfCartDriver_Jump = m_GolfCartDriver.FindAction("Jump", throwIfNotFound: true);
		m_GolfCartDriver_Honk = m_GolfCartDriver.FindAction("Honk", throwIfNotFound: true);
		m_GolfCartShared = asset.FindActionMap("Golf Cart Shared", throwIfNotFound: true);
		m_GolfCartShared_Exit = m_GolfCartShared.FindAction("Exit", throwIfNotFound: true);
		m_GolfCartShared_DiveOut = m_GolfCartShared.FindAction("Dive Out", throwIfNotFound: true);
		m_Hotkeys = asset.FindActionMap("Hotkeys", throwIfNotFound: true);
		m_Hotkeys_Hotkey1 = m_Hotkeys.FindAction("Hotkey 1", throwIfNotFound: true);
		m_Hotkeys_Hotkey2 = m_Hotkeys.FindAction("Hotkey 2", throwIfNotFound: true);
		m_Hotkeys_Hotkey3 = m_Hotkeys.FindAction("Hotkey 3", throwIfNotFound: true);
		m_Hotkeys_Hotkey4 = m_Hotkeys.FindAction("Hotkey 4", throwIfNotFound: true);
		m_Hotkeys_Hotkey5 = m_Hotkeys.FindAction("Hotkey 5", throwIfNotFound: true);
		m_Hotkeys_Hotkey6 = m_Hotkeys.FindAction("Hotkey 6", throwIfNotFound: true);
		m_Hotkeys_Hotkey7 = m_Hotkeys.FindAction("Hotkey 7", throwIfNotFound: true);
		m_Hotkeys_Hotkey8 = m_Hotkeys.FindAction("Hotkey 8", throwIfNotFound: true);
		m_Hotkeys_CycleLeft = m_Hotkeys.FindAction("Cycle Left", throwIfNotFound: true);
		m_Hotkeys_CycleRight = m_Hotkeys.FindAction("Cycle Right", throwIfNotFound: true);
		m_Hotkeys_Toggle = m_Hotkeys.FindAction("Toggle", throwIfNotFound: true);
		m_Spectate = asset.FindActionMap("Spectate", throwIfNotFound: true);
		m_Spectate_CyclePreviousPlayer = m_Spectate.FindAction("Cycle Previous Player", throwIfNotFound: true);
		m_Spectate_CycleNextPlayer = m_Spectate.FindAction("Cycle Next Player", throwIfNotFound: true);
		m_Ingame = asset.FindActionMap("Ingame", throwIfNotFound: true);
		m_Ingame_ShowScoreboard = m_Ingame.FindAction("Show Scoreboard", throwIfNotFound: true);
		m_Ingame_Pause = m_Ingame.FindAction("Pause", throwIfNotFound: true);
		m_Ingame_OpenChat = m_Ingame.FindAction("Open Chat", throwIfNotFound: true);
		m_Ingame_ToggleEmoteMenu = m_Ingame.FindAction("Toggle Emote Menu", throwIfNotFound: true);
		m_VoiceChat = asset.FindActionMap("Voice Chat", throwIfNotFound: true);
		m_VoiceChat_PushToTalk = m_VoiceChat.FindAction("Push To Talk", throwIfNotFound: true);
		m_Camera = asset.FindActionMap("Camera", throwIfNotFound: true);
		m_Camera_Look = m_Camera.FindAction("Look", throwIfNotFound: true);
		m_RadialMenu = asset.FindActionMap("Radial Menu", throwIfNotFound: true);
		m_RadialMenu_Select = m_RadialMenu.FindAction("Select", throwIfNotFound: true);
		m_RadialMenu_Cancel = m_RadialMenu.FindAction("Cancel", throwIfNotFound: true);
		m_UI = asset.FindActionMap("UI", throwIfNotFound: true);
		m_UI_Navigate = m_UI.FindAction("Navigate", throwIfNotFound: true);
		m_UI_Submit = m_UI.FindAction("Submit", throwIfNotFound: true);
		m_UI_Cancel = m_UI.FindAction("Cancel", throwIfNotFound: true);
		m_UI_Point = m_UI.FindAction("Point", throwIfNotFound: true);
		m_UI_Click = m_UI.FindAction("Click", throwIfNotFound: true);
		m_UI_ScrollWheel = m_UI.FindAction("Scroll Wheel", throwIfNotFound: true);
		m_UI_MiddleClick = m_UI.FindAction("Middle Click", throwIfNotFound: true);
		m_UI_RightClick = m_UI.FindAction("Right Click", throwIfNotFound: true);
		m_UI_Exit = m_UI.FindAction("Exit", throwIfNotFound: true);
		m_UI_VirtualCursorMove = m_UI.FindAction("VirtualCursorMove", throwIfNotFound: true);
		m_UI_VirtualCursorClick = m_UI.FindAction("VirtualCursorClick", throwIfNotFound: true);
		m_UI_VirtualCursorScrollWheel = m_UI.FindAction("VirtualCursorScrollWheel", throwIfNotFound: true);
		m_Vote = asset.FindActionMap("Vote", throwIfNotFound: true);
		m_Vote_Yes = m_Vote.FindAction("Yes", throwIfNotFound: true);
		m_Vote_No = m_Vote.FindAction("No", throwIfNotFound: true);
	}

	~GameControls()
	{
	}

	public void Dispose()
	{
		UnityEngine.Object.Destroy(asset);
	}

	public bool Contains(InputAction action)
	{
		return asset.Contains(action);
	}

	public IEnumerator<InputAction> GetEnumerator()
	{
		return asset.GetEnumerator();
	}

	IEnumerator IEnumerable.GetEnumerator()
	{
		return GetEnumerator();
	}

	public void Enable()
	{
		asset.Enable();
	}

	public void Disable()
	{
		asset.Disable();
	}

	public InputAction FindAction(string actionNameOrId, bool throwIfNotFound = false)
	{
		return asset.FindAction(actionNameOrId, throwIfNotFound);
	}

	public int FindBinding(InputBinding bindingMask, out InputAction action)
	{
		return asset.FindBinding(bindingMask, out action);
	}
}
