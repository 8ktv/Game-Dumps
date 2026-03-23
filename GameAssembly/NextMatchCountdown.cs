using System;
using System.Runtime.InteropServices;
using Mirror;
using TMPro;
using UnityEngine;

public class NextMatchCountdown : SingletonNetworkBehaviour<NextMatchCountdown>
{
	[SerializeField]
	private GameObject background;

	[SerializeField]
	private TextMeshProUGUI message;

	[SyncVar(hook = "OnIsVisibleChanged")]
	private bool isVisible;

	[SyncVar(hook = "OnIsCourseFinishedChanged")]
	private bool isCourseFinished;

	[SyncVar(hook = "OnDisplayedTimeChanged")]
	private int displayedTime = -1;

	public Action<bool, bool> _Mirror_SyncVarHookDelegate_isVisible;

	public Action<bool, bool> _Mirror_SyncVarHookDelegate_isCourseFinished;

	public Action<int, int> _Mirror_SyncVarHookDelegate_displayedTime;

	private static string NextMatchCountdownMessage => Localization.UI.MATCH_NextMatchCountdown;

	private static string CourseEndedCountdownMessage => Localization.UI.MATCH_CourseEndedCountdown;

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

	public bool NetworkisCourseFinished
	{
		get
		{
			return isCourseFinished;
		}
		[param: In]
		set
		{
			GeneratedSyncVarSetter(value, ref isCourseFinished, 2uL, _Mirror_SyncVarHookDelegate_isCourseFinished);
		}
	}

	public int NetworkdisplayedTime
	{
		get
		{
			return displayedTime;
		}
		[param: In]
		set
		{
			GeneratedSyncVarSetter(value, ref displayedTime, 4uL, _Mirror_SyncVarHookDelegate_displayedTime);
		}
	}

	protected override void Awake()
	{
		base.Awake();
		ApplyVisibility();
		LocalizationManager.LanguageChanged += OnLocalizationLanguageChanged;
	}

	protected override void OnDestroy()
	{
		LocalizationManager.LanguageChanged -= OnLocalizationLanguageChanged;
		base.OnDestroy();
	}

	public static void Show()
	{
		if (SingletonNetworkBehaviour<NextMatchCountdown>.HasInstance)
		{
			SingletonNetworkBehaviour<NextMatchCountdown>.Instance.ShowInternal();
		}
	}

	public static void Hide()
	{
		if (SingletonNetworkBehaviour<NextMatchCountdown>.HasInstance)
		{
			SingletonNetworkBehaviour<NextMatchCountdown>.Instance.HideInternal();
		}
	}

	public static void SetIsCourseFinished(bool isFinished)
	{
		if (SingletonNetworkBehaviour<NextMatchCountdown>.HasInstance)
		{
			SingletonNetworkBehaviour<NextMatchCountdown>.Instance.SetIsCourseFinishedInternal(isFinished);
		}
	}

	public static void SetRemainingTime(float time)
	{
		if (SingletonNetworkBehaviour<NextMatchCountdown>.HasInstance)
		{
			SingletonNetworkBehaviour<NextMatchCountdown>.Instance.SetRemainingTimeInternal(time);
		}
	}

	[Server]
	private void ShowInternal()
	{
		if (!NetworkServer.active)
		{
			Debug.LogWarning("[Server] function 'System.Void NextMatchCountdown::ShowInternal()' called when server was not active");
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
			Debug.LogWarning("[Server] function 'System.Void NextMatchCountdown::HideInternal()' called when server was not active");
		}
		else
		{
			NetworkisVisible = false;
		}
	}

	private void ApplyVisibility()
	{
		background.SetActive(isVisible);
	}

	[Server]
	private void SetIsCourseFinishedInternal(bool isFinished)
	{
		if (!NetworkServer.active)
		{
			Debug.LogWarning("[Server] function 'System.Void NextMatchCountdown::SetIsCourseFinishedInternal(System.Boolean)' called when server was not active");
		}
		else
		{
			NetworkisCourseFinished = isFinished;
		}
	}

	[Server]
	private void SetRemainingTimeInternal(float time)
	{
		if (!NetworkServer.active)
		{
			Debug.LogWarning("[Server] function 'System.Void NextMatchCountdown::SetRemainingTimeInternal(System.Single)' called when server was not active");
		}
		else
		{
			NetworkdisplayedTime = BMath.CeilToInt(time);
		}
	}

	private void UpdateMessageInternal()
	{
		message.text = string.Format(isCourseFinished ? CourseEndedCountdownMessage : NextMatchCountdownMessage, displayedTime);
	}

	private void OnLocalizationLanguageChanged()
	{
		UpdateMessageInternal();
	}

	private void OnIsVisibleChanged(bool wasVisible, bool isVisible)
	{
		ApplyVisibility();
	}

	private void OnIsCourseFinishedChanged(bool wasCourseFinished, bool isCourseFinished)
	{
		UpdateMessageInternal();
	}

	private void OnDisplayedTimeChanged(int previousDisplayedTime, int currentDisplayedTime)
	{
		UpdateMessageInternal();
	}

	public NextMatchCountdown()
	{
		_Mirror_SyncVarHookDelegate_isVisible = OnIsVisibleChanged;
		_Mirror_SyncVarHookDelegate_isCourseFinished = OnIsCourseFinishedChanged;
		_Mirror_SyncVarHookDelegate_displayedTime = OnDisplayedTimeChanged;
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
			writer.WriteBool(isCourseFinished);
			writer.WriteVarInt(displayedTime);
			return;
		}
		writer.WriteVarULong(syncVarDirtyBits);
		if ((syncVarDirtyBits & 1L) != 0L)
		{
			writer.WriteBool(isVisible);
		}
		if ((syncVarDirtyBits & 2L) != 0L)
		{
			writer.WriteBool(isCourseFinished);
		}
		if ((syncVarDirtyBits & 4L) != 0L)
		{
			writer.WriteVarInt(displayedTime);
		}
	}

	public override void DeserializeSyncVars(NetworkReader reader, bool initialState)
	{
		base.DeserializeSyncVars(reader, initialState);
		if (initialState)
		{
			GeneratedSyncVarDeserialize(ref isVisible, _Mirror_SyncVarHookDelegate_isVisible, reader.ReadBool());
			GeneratedSyncVarDeserialize(ref isCourseFinished, _Mirror_SyncVarHookDelegate_isCourseFinished, reader.ReadBool());
			GeneratedSyncVarDeserialize(ref displayedTime, _Mirror_SyncVarHookDelegate_displayedTime, reader.ReadVarInt());
			return;
		}
		long num = (long)reader.ReadVarULong();
		if ((num & 1L) != 0L)
		{
			GeneratedSyncVarDeserialize(ref isVisible, _Mirror_SyncVarHookDelegate_isVisible, reader.ReadBool());
		}
		if ((num & 2L) != 0L)
		{
			GeneratedSyncVarDeserialize(ref isCourseFinished, _Mirror_SyncVarHookDelegate_isCourseFinished, reader.ReadBool());
		}
		if ((num & 4L) != 0L)
		{
			GeneratedSyncVarDeserialize(ref displayedTime, _Mirror_SyncVarHookDelegate_displayedTime, reader.ReadVarInt());
		}
	}
}
