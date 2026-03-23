using System;
using System.Collections;
using System.Runtime.InteropServices;
using Mirror;
using UnityEngine;
using UnityEngine.Localization;
using UnityEngine.Localization.Components;

public class HoleOverviewCameraUi : SingletonNetworkBehaviour<HoleOverviewCameraUi>
{
	public enum State
	{
		None,
		WaitingForPlayers,
		DisplayingHoleName
	}

	[SerializeField]
	private LocalizeStringEvent[] messageLocalizeStringEvents;

	[SerializeField]
	private UiVisibilityController visibilityController;

	[SerializeField]
	private float fadeInDuration;

	[SerializeField]
	private float fadeOutDuration;

	[SyncVar(hook = "OnIsVisibleChanged")]
	private bool isVisible;

	[SyncVar(hook = "OnStateChanged")]
	private State state;

	private Coroutine visibilityRoutine;

	public Action<bool, bool> _Mirror_SyncVarHookDelegate_isVisible;

	public Action<State, State> _Mirror_SyncVarHookDelegate_state;

	public bool NetworkisVisible
	{
		get
		{
			return isVisible;
		}
		[param: In]
		set
		{
			GeneratedSyncVarSetter(value, ref isVisible, 1uL, _Mirror_SyncVarHookDelegate_isVisible);
		}
	}

	public State Networkstate
	{
		get
		{
			return state;
		}
		[param: In]
		set
		{
			GeneratedSyncVarSetter(value, ref state, 2uL, _Mirror_SyncVarHookDelegate_state);
		}
	}

	public override void OnStartClient()
	{
		visibilityController.SetDesiredAlpha(isVisible ? 1f : 0f);
		OnStateChanged(State.None, state);
	}

	public static void Show()
	{
		if (SingletonNetworkBehaviour<HoleOverviewCameraUi>.HasInstance)
		{
			SingletonNetworkBehaviour<HoleOverviewCameraUi>.Instance.ShowInternal();
		}
	}

	public static void Hide()
	{
		if (SingletonNetworkBehaviour<HoleOverviewCameraUi>.HasInstance)
		{
			SingletonNetworkBehaviour<HoleOverviewCameraUi>.Instance.HideInternal();
		}
	}

	public static void SetState(State state)
	{
		if (SingletonNetworkBehaviour<HoleOverviewCameraUi>.HasInstance)
		{
			SingletonNetworkBehaviour<HoleOverviewCameraUi>.Instance.SetStateInternal(state);
		}
	}

	[Server]
	private void ShowInternal()
	{
		if (!NetworkServer.active)
		{
			Debug.LogWarning("[Server] function 'System.Void HoleOverviewCameraUi::ShowInternal()' called when server was not active");
		}
		else
		{
			NetworkisVisible = true;
		}
	}

	[Server]
	private void HideInternal()
	{
		if (!NetworkServer.active)
		{
			Debug.LogWarning("[Server] function 'System.Void HoleOverviewCameraUi::HideInternal()' called when server was not active");
		}
		else
		{
			NetworkisVisible = false;
		}
	}

	[Server]
	private void SetStateInternal(State state)
	{
		if (!NetworkServer.active)
		{
			Debug.LogWarning("[Server] function 'System.Void HoleOverviewCameraUi::SetStateInternal(HoleOverviewCameraUi/State)' called when server was not active");
		}
		else
		{
			Networkstate = state;
		}
	}

	private void OnIsVisibleChanged(bool wasVisible, bool isVisible)
	{
		if (visibilityRoutine != null)
		{
			StopCoroutine(visibilityRoutine);
		}
		if (isVisible)
		{
			visibilityRoutine = StartCoroutine(FadeToRoutine(1f, fadeInDuration, BMath.EaseIn));
		}
		else if (LoadingScreen.IsVisible || LoadingScreen.IsFadingScreenOut)
		{
			visibilityController.SetDesiredAlpha(0f);
		}
		else
		{
			visibilityRoutine = StartCoroutine(FadeToRoutine(0f, fadeOutDuration, BMath.EaseOut));
		}
		IEnumerator FadeToRoutine(float targetAlpha, float duration, Func<float, float> Easing)
		{
			float time = 0f;
			float initialAlpha = visibilityController.DesiredAlpha;
			if (initialAlpha != targetAlpha)
			{
				for (; time < duration; time += Time.deltaTime)
				{
					float arg = time / duration;
					visibilityController.SetDesiredAlpha(BMath.LerpClamped(initialAlpha, targetAlpha, Easing(arg)));
					yield return null;
				}
				visibilityController.SetDesiredAlpha(targetAlpha);
			}
		}
	}

	private void OnStateChanged(State previousState, State currentState)
	{
		if (TryGetMessage(out var localizedMessage))
		{
			LocalizeStringEvent[] array = messageLocalizeStringEvents;
			for (int i = 0; i < array.Length; i++)
			{
				array[i].StringReference = localizedMessage;
			}
		}
		bool TryGetMessage(out LocalizedString reference)
		{
			reference = currentState switch
			{
				State.WaitingForPlayers => Localization.UI.TEE_OFF_WaitingForPlayers_Ref, 
				State.DisplayingHoleName => CourseManager.GetCurrentHoleLocalizedName(), 
				_ => null, 
			};
			return reference != null;
		}
	}

	public HoleOverviewCameraUi()
	{
		_Mirror_SyncVarHookDelegate_isVisible = OnIsVisibleChanged;
		_Mirror_SyncVarHookDelegate_state = OnStateChanged;
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
			writer.WriteBool(isVisible);
			GeneratedNetworkCode._Write_HoleOverviewCameraUi_002FState(writer, state);
			return;
		}
		writer.WriteVarULong(syncVarDirtyBits);
		if ((syncVarDirtyBits & 1L) != 0L)
		{
			writer.WriteBool(isVisible);
		}
		if ((syncVarDirtyBits & 2L) != 0L)
		{
			GeneratedNetworkCode._Write_HoleOverviewCameraUi_002FState(writer, state);
		}
	}

	public override void DeserializeSyncVars(NetworkReader reader, bool initialState)
	{
		base.DeserializeSyncVars(reader, initialState);
		if (initialState)
		{
			GeneratedSyncVarDeserialize(ref isVisible, _Mirror_SyncVarHookDelegate_isVisible, reader.ReadBool());
			GeneratedSyncVarDeserialize(ref state, _Mirror_SyncVarHookDelegate_state, GeneratedNetworkCode._Read_HoleOverviewCameraUi_002FState(reader));
			return;
		}
		long num = (long)reader.ReadVarULong();
		if ((num & 1L) != 0L)
		{
			GeneratedSyncVarDeserialize(ref isVisible, _Mirror_SyncVarHookDelegate_isVisible, reader.ReadBool());
		}
		if ((num & 2L) != 0L)
		{
			GeneratedSyncVarDeserialize(ref state, _Mirror_SyncVarHookDelegate_state, GeneratedNetworkCode._Read_HoleOverviewCameraUi_002FState(reader));
		}
	}
}
