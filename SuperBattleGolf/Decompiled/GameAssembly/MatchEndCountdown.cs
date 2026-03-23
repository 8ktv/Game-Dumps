using System;
using System.Collections;
using System.Runtime.InteropServices;
using FMODUnity;
using Mirror;
using TMPro;
using UnityEngine;

public class MatchEndCountdown : SingletonNetworkBehaviour<MatchEndCountdown>
{
	[SerializeField]
	private TextMeshProUGUI time;

	[SerializeField]
	private GameObject overtimeLabel;

	[SerializeField]
	private int preOvertimeUrgencyThreshold;

	[SerializeField]
	private float urgencyPopSizeFactor;

	[SerializeField]
	private float urgencyPopDuration;

	[SerializeField]
	private Color urgencyColor;

	[SyncVar(hook = "OnIsVisibleChanged")]
	private bool isVisible;

	[SyncVar(hook = "OnIsInOvertimeChanged")]
	private bool isInOvertime;

	[SyncVar(hook = "OnDisplayedTimeChanged")]
	private int displayedTime = -1;

	private Color defaultColor;

	private Coroutine popRoutine;

	public Action<bool, bool> _Mirror_SyncVarHookDelegate_isVisible;

	public Action<bool, bool> _Mirror_SyncVarHookDelegate_isInOvertime;

	public Action<int, int> _Mirror_SyncVarHookDelegate_displayedTime;

	public static int DisplayedTime
	{
		get
		{
			if (!SingletonNetworkBehaviour<MatchEndCountdown>.HasInstance)
			{
				return -1;
			}
			return SingletonNetworkBehaviour<MatchEndCountdown>.Instance.displayedTime;
		}
	}

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

	public bool NetworkisInOvertime
	{
		get
		{
			return isInOvertime;
		}
		[param: In]
		set
		{
			GeneratedSyncVarSetter(value, ref isInOvertime, 2uL, _Mirror_SyncVarHookDelegate_isInOvertime);
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
		defaultColor = time.color;
		ApplyVisibility();
	}

	public static void Show()
	{
		if (SingletonNetworkBehaviour<MatchEndCountdown>.HasInstance)
		{
			SingletonNetworkBehaviour<MatchEndCountdown>.Instance.ShowInternal();
		}
	}

	public static void Hide()
	{
		if (SingletonNetworkBehaviour<MatchEndCountdown>.HasInstance)
		{
			SingletonNetworkBehaviour<MatchEndCountdown>.Instance.HideInternal();
		}
	}

	public static void EnterOvertime()
	{
		if (SingletonNetworkBehaviour<MatchEndCountdown>.HasInstance)
		{
			SingletonNetworkBehaviour<MatchEndCountdown>.Instance.EnterOvertimeInternal();
		}
	}

	public static void SetTime(float time)
	{
		if (SingletonNetworkBehaviour<MatchEndCountdown>.HasInstance)
		{
			SingletonNetworkBehaviour<MatchEndCountdown>.Instance.SetTimeInternal(time);
		}
	}

	[Server]
	private void ShowInternal()
	{
		if (!NetworkServer.active)
		{
			Debug.LogWarning("[Server] function 'System.Void MatchEndCountdown::ShowInternal()' called when server was not active");
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
			Debug.LogWarning("[Server] function 'System.Void MatchEndCountdown::HideInternal()' called when server was not active");
		}
		else
		{
			NetworkisVisible = false;
		}
	}

	private void ApplyVisibility()
	{
		time.gameObject.SetActive(isVisible);
		overtimeLabel.SetActive(isVisible && isInOvertime);
	}

	[Server]
	private void EnterOvertimeInternal()
	{
		if (!NetworkServer.active)
		{
			Debug.LogWarning("[Server] function 'System.Void MatchEndCountdown::EnterOvertimeInternal()' called when server was not active");
		}
		else
		{
			NetworkisInOvertime = true;
		}
	}

	[Server]
	private void SetTimeInternal(float time)
	{
		if (!NetworkServer.active)
		{
			Debug.LogWarning("[Server] function 'System.Void MatchEndCountdown::SetTimeInternal(System.Single)' called when server was not active");
		}
		else
		{
			NetworkdisplayedTime = BMath.CeilToInt(time);
		}
	}

	private void UpdateMessageInternal()
	{
		time.text = displayedTime.ToString();
	}

	private void Pop()
	{
		if (popRoutine != null)
		{
			StopCoroutine(popRoutine);
		}
		popRoutine = StartCoroutine(PopRoutine());
		RuntimeManager.PlayOneShot(GameManager.AudioSettings.MatchEndCountdownUrgentTickEvent);
		IEnumerator PopRoutine()
		{
			for (float time = 0f; time < urgencyPopDuration; time += Time.deltaTime)
			{
				float t = time / urgencyPopDuration;
				float num = BMath.Lerp(urgencyPopSizeFactor, 1f, BMath.EaseIn(t));
				this.time.rectTransform.localScale = Vector3.one * num;
				yield return null;
			}
			this.time.rectTransform.localScale = Vector3.one;
		}
	}

	private void OnIsVisibleChanged(bool wasVisible, bool isVisible)
	{
		ApplyVisibility();
	}

	private void OnIsInOvertimeChanged(bool wasInOvertime, bool isInOvertime)
	{
		ApplyVisibility();
	}

	private void OnDisplayedTimeChanged(int previousDisplayedTime, int currentDisplayedTime)
	{
		UpdateMessageInternal();
		if (!isInOvertime && displayedTime <= preOvertimeUrgencyThreshold)
		{
			time.color = urgencyColor;
			Pop();
		}
		else
		{
			time.color = defaultColor;
			time.rectTransform.localScale = Vector3.one;
		}
		if (!isInOvertime && displayedTime == 10)
		{
			CourseManager.PlayAnnouncerLineLocalOnly(AnnouncerLine.Last10Seconds);
		}
	}

	public MatchEndCountdown()
	{
		_Mirror_SyncVarHookDelegate_isVisible = OnIsVisibleChanged;
		_Mirror_SyncVarHookDelegate_isInOvertime = OnIsInOvertimeChanged;
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
			writer.WriteBool(isInOvertime);
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
			writer.WriteBool(isInOvertime);
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
			GeneratedSyncVarDeserialize(ref isInOvertime, _Mirror_SyncVarHookDelegate_isInOvertime, reader.ReadBool());
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
			GeneratedSyncVarDeserialize(ref isInOvertime, _Mirror_SyncVarHookDelegate_isInOvertime, reader.ReadBool());
		}
		if ((num & 4L) != 0L)
		{
			GeneratedSyncVarDeserialize(ref displayedTime, _Mirror_SyncVarHookDelegate_displayedTime, reader.ReadVarInt());
		}
	}
}
