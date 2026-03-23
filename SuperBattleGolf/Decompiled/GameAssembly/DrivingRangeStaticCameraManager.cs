using System;
using System.Runtime.InteropServices;
using Cysharp.Threading.Tasks;
using Mirror;
using Mirror.RemoteCalls;
using UnityEngine;

public class DrivingRangeStaticCameraManager : SingletonNetworkBehaviour<DrivingRangeStaticCameraManager>, ILateBUpdateCallback, IAnyBUpdateCallback
{
	[SerializeField]
	private DrivingRangeStaticCamera[] cameras;

	[SerializeField]
	private Renderer screenRenderer;

	[SerializeField]
	private float cycleButtonCooldown;

	[SyncVar(hook = "OnCurrentCameraIndexChanged")]
	private int currentCameraIndex = -1;

	private DrivingRangeStaticCamera currentCamera;

	[SyncVar]
	private bool isCycleNextButtonEnabled = true;

	private bool isInitialized;

	private AntiCheatPerPlayerRateChecker serverCycleNextCameraCommandRateLimiter;

	public Action<int, int> _Mirror_SyncVarHookDelegate_currentCameraIndex;

	public static bool IsCycleNextButtonEnabled
	{
		get
		{
			if (SingletonNetworkBehaviour<DrivingRangeStaticCameraManager>.HasInstance)
			{
				return SingletonNetworkBehaviour<DrivingRangeStaticCameraManager>.Instance.isCycleNextButtonEnabled;
			}
			return false;
		}
	}

	public int NetworkcurrentCameraIndex
	{
		get
		{
			return currentCameraIndex;
		}
		[param: In]
		set
		{
			GeneratedSyncVarSetter(value, ref currentCameraIndex, 1uL, _Mirror_SyncVarHookDelegate_currentCameraIndex);
		}
	}

	public bool NetworkisCycleNextButtonEnabled
	{
		get
		{
			return isCycleNextButtonEnabled;
		}
		[param: In]
		set
		{
			GeneratedSyncVarSetter(value, ref isCycleNextButtonEnabled, 2uL, null);
		}
	}

	protected override void Awake()
	{
		base.Awake();
		BUpdate.RegisterCallback(this);
	}

	protected override void OnDestroy()
	{
		base.OnDestroy();
		BUpdate.DeregisterCallback(this);
	}

	public override void OnStartServer()
	{
		if (isInitialized)
		{
			NetworkcurrentCameraIndex = 0;
			ApplyCurrentCameraIndex();
		}
		serverCycleNextCameraCommandRateLimiter = new AntiCheatPerPlayerRateChecker("Cycle next driving range camera", 0.05f, 5, 20, 1f);
	}

	public override void OnStartClient()
	{
		Initialize();
		async void Initialize()
		{
			await UniTask.WaitWhile(() => LoadingScreen.IsVisible || LoadingScreen.IsFadingScreenOut);
			if (!(this == null))
			{
				DrivingRangeStaticCamera[] array = cameras;
				foreach (DrivingRangeStaticCamera camera in array)
				{
					await UniTask.WaitForEndOfFrame();
					camera.RenderStaticTexture();
				}
				isInitialized = true;
				if (base.isServer)
				{
					NetworkcurrentCameraIndex = 0;
					ApplyCurrentCameraIndex();
				}
			}
		}
	}

	public static void CmdCycleNextCameraForAllClients()
	{
		if (SingletonNetworkBehaviour<DrivingRangeStaticCameraManager>.HasInstance)
		{
			SingletonNetworkBehaviour<DrivingRangeStaticCameraManager>.Instance.CmdCycleNextCameraForAllClientsInternal();
		}
	}

	public void OnLateBUpdate()
	{
		bool isVisible = screenRenderer.isVisible;
		if (currentCamera != null && currentCamera.IsRendering != isVisible)
		{
			currentCamera.SetCameraRendering(isVisible);
		}
	}

	[Command(requiresAuthority = false)]
	private void CmdCycleNextCameraForAllClientsInternal(NetworkConnectionToClient sender = null)
	{
		if (base.isServer && base.isClient)
		{
			UserCode_CmdCycleNextCameraForAllClientsInternal__NetworkConnectionToClient(sender);
			return;
		}
		NetworkWriterPooled writer = NetworkWriterPool.Get();
		SendCommandInternal("System.Void DrivingRangeStaticCameraManager::CmdCycleNextCameraForAllClientsInternal(Mirror.NetworkConnectionToClient)", -1344733857, writer, 0, requiresAuthority: false);
		NetworkWriterPool.Return(writer);
	}

	private void ApplyCurrentCameraIndex()
	{
		if (currentCamera != null)
		{
			currentCamera.SetCameraActive(isActive: false);
		}
		currentCamera = cameras[currentCameraIndex];
		if (currentCamera != null)
		{
			currentCamera.SetCameraActive(isActive: true);
		}
	}

	private void OnCurrentCameraIndexChanged(int previousIndex, int currentIndex)
	{
		ApplyCurrentCameraIndex();
	}

	public DrivingRangeStaticCameraManager()
	{
		_Mirror_SyncVarHookDelegate_currentCameraIndex = OnCurrentCameraIndexChanged;
	}

	public override bool Weaved()
	{
		return true;
	}

	protected void UserCode_CmdCycleNextCameraForAllClientsInternal__NetworkConnectionToClient(NetworkConnectionToClient sender)
	{
		if (serverCycleNextCameraCommandRateLimiter.RegisterHit(sender) && isCycleNextButtonEnabled)
		{
			NetworkcurrentCameraIndex = BMath.Wrap(currentCameraIndex + 1, cameras.Length);
			DisableCycleButtonTemporarily(cycleButtonCooldown);
		}
		async void DisableCycleButtonTemporarily(float duration)
		{
			NetworkisCycleNextButtonEnabled = false;
			for (float time = 0f; time < duration; time += Time.deltaTime)
			{
				await UniTask.Yield();
				if (this == null)
				{
					return;
				}
			}
			NetworkisCycleNextButtonEnabled = true;
		}
	}

	protected static void InvokeUserCode_CmdCycleNextCameraForAllClientsInternal__NetworkConnectionToClient(NetworkBehaviour obj, NetworkReader reader, NetworkConnectionToClient senderConnection)
	{
		if (!NetworkServer.active)
		{
			Debug.LogError("Command CmdCycleNextCameraForAllClientsInternal called on client.");
		}
		else
		{
			((DrivingRangeStaticCameraManager)obj).UserCode_CmdCycleNextCameraForAllClientsInternal__NetworkConnectionToClient(senderConnection);
		}
	}

	static DrivingRangeStaticCameraManager()
	{
		RemoteProcedureCalls.RegisterCommand(typeof(DrivingRangeStaticCameraManager), "System.Void DrivingRangeStaticCameraManager::CmdCycleNextCameraForAllClientsInternal(Mirror.NetworkConnectionToClient)", InvokeUserCode_CmdCycleNextCameraForAllClientsInternal__NetworkConnectionToClient, requiresAuthority: false);
	}

	public override void SerializeSyncVars(NetworkWriter writer, bool forceAll)
	{
		base.SerializeSyncVars(writer, forceAll);
		if (forceAll)
		{
			writer.WriteVarInt(currentCameraIndex);
			writer.WriteBool(isCycleNextButtonEnabled);
			return;
		}
		writer.WriteVarULong(syncVarDirtyBits);
		if ((syncVarDirtyBits & 1L) != 0L)
		{
			writer.WriteVarInt(currentCameraIndex);
		}
		if ((syncVarDirtyBits & 2L) != 0L)
		{
			writer.WriteBool(isCycleNextButtonEnabled);
		}
	}

	public override void DeserializeSyncVars(NetworkReader reader, bool initialState)
	{
		base.DeserializeSyncVars(reader, initialState);
		if (initialState)
		{
			GeneratedSyncVarDeserialize(ref currentCameraIndex, _Mirror_SyncVarHookDelegate_currentCameraIndex, reader.ReadVarInt());
			GeneratedSyncVarDeserialize(ref isCycleNextButtonEnabled, null, reader.ReadBool());
			return;
		}
		long num = (long)reader.ReadVarULong();
		if ((num & 1L) != 0L)
		{
			GeneratedSyncVarDeserialize(ref currentCameraIndex, _Mirror_SyncVarHookDelegate_currentCameraIndex, reader.ReadVarInt());
		}
		if ((num & 2L) != 0L)
		{
			GeneratedSyncVarDeserialize(ref isCycleNextButtonEnabled, null, reader.ReadBool());
		}
	}
}
