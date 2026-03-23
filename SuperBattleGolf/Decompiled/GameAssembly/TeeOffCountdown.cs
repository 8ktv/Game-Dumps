using System;
using System.Collections;
using System.Runtime.InteropServices;
using FMODUnity;
using Mirror;
using Mirror.RemoteCalls;
using TMPro;
using UnityEngine;

public class TeeOffCountdown : SingletonNetworkBehaviour<TeeOffCountdown>
{
	[SerializeField]
	private TextMeshProUGUI[] countdownLabels;

	[SerializeField]
	private UiVisibilityController visibilityController;

	[SerializeField]
	private Animator animator;

	[SyncVar(hook = "OnIsVisibleChanged")]
	private bool isVisible;

	[SyncVar(hook = "OnDisplayedTimeChanged")]
	private int displayedTime = -1;

	private Coroutine hideRoutine;

	private static readonly int popHash;

	public Action<bool, bool> _Mirror_SyncVarHookDelegate_isVisible;

	public Action<int, int> _Mirror_SyncVarHookDelegate_displayedTime;

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

	public int NetworkdisplayedTime
	{
		get
		{
			return displayedTime;
		}
		[param: In]
		set
		{
			GeneratedSyncVarSetter(value, ref displayedTime, 2uL, _Mirror_SyncVarHookDelegate_displayedTime);
		}
	}

	protected override void Awake()
	{
		base.Awake();
		visibilityController.SetDesiredAlpha(0f);
	}

	public static void Show()
	{
		if (SingletonNetworkBehaviour<TeeOffCountdown>.HasInstance)
		{
			SingletonNetworkBehaviour<TeeOffCountdown>.Instance.ShowInternal();
		}
	}

	public static void Hide()
	{
		if (SingletonNetworkBehaviour<TeeOffCountdown>.HasInstance)
		{
			SingletonNetworkBehaviour<TeeOffCountdown>.Instance.HideInternal();
		}
	}

	public static void SetRemainingTime(float time)
	{
		if (SingletonNetworkBehaviour<TeeOffCountdown>.HasInstance)
		{
			SingletonNetworkBehaviour<TeeOffCountdown>.Instance.SetRemainingTimeInternal(time);
		}
	}

	[Server]
	private void ShowInternal()
	{
		if (!NetworkServer.active)
		{
			Debug.LogWarning("[Server] function 'System.Void TeeOffCountdown::ShowInternal()' called when server was not active");
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
			Debug.LogWarning("[Server] function 'System.Void TeeOffCountdown::HideInternal()' called when server was not active");
		}
		else
		{
			NetworkisVisible = false;
		}
	}

	private void ApplyVisibility()
	{
		if (hideRoutine != null)
		{
			StopCoroutine(hideRoutine);
		}
		if (isVisible)
		{
			visibilityController.SetDesiredAlpha(1f);
		}
		else
		{
			hideRoutine = StartCoroutine(HideRoutine());
		}
		IEnumerator HideRoutine()
		{
			float time = 0.5f;
			while (time > 0f)
			{
				visibilityController.SetDesiredAlpha(time / 0.5f);
				time -= Time.deltaTime;
				yield return null;
			}
			visibilityController.SetDesiredAlpha(0f);
		}
	}

	[Server]
	private void SetRemainingTimeInternal(float time)
	{
		if (!NetworkServer.active)
		{
			Debug.LogWarning("[Server] function 'System.Void TeeOffCountdown::SetRemainingTimeInternal(System.Single)' called when server was not active");
		}
		else
		{
			NetworkdisplayedTime = BMath.CeilToInt(time);
		}
	}

	private void UpdateMessageInternal()
	{
		string text = GetMessage();
		TextMeshProUGUI[] array = countdownLabels;
		for (int i = 0; i < array.Length; i++)
		{
			array[i].text = text;
		}
		string GetMessage()
		{
			if (displayedTime > 0)
			{
				return displayedTime.ToString();
			}
			return Localization.UI.TEE_OFF_Golf;
		}
	}

	private void Pop()
	{
		animator.SetTrigger(popHash);
	}

	private void OnIsVisibleChanged(bool wasVisible, bool isVisible)
	{
		ApplyVisibility();
	}

	private void OnDisplayedTimeChanged(int previousDisplayedTime, int currentDisplayedTime)
	{
		UpdateMessageInternal();
		Pop();
		if (base.isServer)
		{
			RpcPlayAnnouncerLine(currentDisplayedTime);
		}
	}

	[ClientRpc]
	private void RpcPlayAnnouncerLine(int time)
	{
		NetworkWriterPooled writer = NetworkWriterPool.Get();
		writer.WriteVarInt(time);
		SendRPCInternal("System.Void TeeOffCountdown::RpcPlayAnnouncerLine(System.Int32)", -657868106, writer, 0, includeOwner: true);
		NetworkWriterPool.Return(writer);
	}

	public TeeOffCountdown()
	{
		_Mirror_SyncVarHookDelegate_isVisible = OnIsVisibleChanged;
		_Mirror_SyncVarHookDelegate_displayedTime = OnDisplayedTimeChanged;
	}

	static TeeOffCountdown()
	{
		popHash = Animator.StringToHash("Pop");
		RemoteProcedureCalls.RegisterRpc(typeof(TeeOffCountdown), "System.Void TeeOffCountdown::RpcPlayAnnouncerLine(System.Int32)", InvokeUserCode_RpcPlayAnnouncerLine__Int32);
	}

	public override bool Weaved()
	{
		return true;
	}

	protected void UserCode_RpcPlayAnnouncerLine__Int32(int time)
	{
		var (eventReference, eventReference2) = time switch
		{
			5 => (GameManager.AudioSettings.AnnouncerTeeOff5Event, GameManager.AudioSettings.TeeOff5Event), 
			4 => (GameManager.AudioSettings.AnnouncerTeeOff4Event, GameManager.AudioSettings.TeeOff4Event), 
			3 => (GameManager.AudioSettings.AnnouncerTeeOff3Event, GameManager.AudioSettings.TeeOff3Event), 
			2 => (GameManager.AudioSettings.AnnouncerTeeOff2Event, GameManager.AudioSettings.TeeOff2Event), 
			1 => (GameManager.AudioSettings.AnnouncerTeeOff1Event, GameManager.AudioSettings.TeeOff1Event), 
			0 => (GameManager.AudioSettings.AnnouncerTeeOffGolfEvent, GameManager.AudioSettings.TeeOffGolfEvent), 
			_ => default((EventReference, EventReference)), 
		};
		if (!eventReference.IsNull)
		{
			RuntimeManager.PlayOneShot(eventReference);
			RuntimeManager.PlayOneShot(eventReference2);
		}
	}

	protected static void InvokeUserCode_RpcPlayAnnouncerLine__Int32(NetworkBehaviour obj, NetworkReader reader, NetworkConnectionToClient senderConnection)
	{
		if (!NetworkClient.active)
		{
			Debug.LogError("RPC RpcPlayAnnouncerLine called on server.");
		}
		else
		{
			((TeeOffCountdown)obj).UserCode_RpcPlayAnnouncerLine__Int32(reader.ReadVarInt());
		}
	}

	public override void SerializeSyncVars(NetworkWriter writer, bool forceAll)
	{
		base.SerializeSyncVars(writer, forceAll);
		if (forceAll)
		{
			writer.WriteBool(isVisible);
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
			writer.WriteVarInt(displayedTime);
		}
	}

	public override void DeserializeSyncVars(NetworkReader reader, bool initialState)
	{
		base.DeserializeSyncVars(reader, initialState);
		if (initialState)
		{
			GeneratedSyncVarDeserialize(ref isVisible, _Mirror_SyncVarHookDelegate_isVisible, reader.ReadBool());
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
			GeneratedSyncVarDeserialize(ref displayedTime, _Mirror_SyncVarHookDelegate_displayedTime, reader.ReadVarInt());
		}
	}
}
