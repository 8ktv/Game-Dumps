using System;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using Cysharp.Threading.Tasks;
using FMOD.Studio;
using FMODUnity;
using Mirror;
using Mirror.RemoteCalls;
using UnityEngine;
using UnityEngine.Localization;

public class ItemDispenser : NetworkBehaviour, IInteractable
{
	[StructLayout(LayoutKind.Auto)]
	[CompilerGenerated]
	private struct _003CCmdDispenseItemFor_003Ed__20 : IAsyncStateMachine
	{
		public int _003C_003E1__state;

		public AsyncVoidMethodBuilder _003C_003Et__builder;

		public ItemDispenser _003C_003E4__this;

		private ItemData _003CitemData_003E5__2;

		private UniTask.Awaiter _003C_003Eu__1;

		private void MoveNext()
		{
			int num = _003C_003E1__state;
			ItemDispenser itemDispenser = _003C_003E4__this;
			try
			{
				if (num == 0)
				{
					goto IL_0063;
				}
				if (itemDispenser.isInteractionEnabled && !itemDispenser.isDispensing)
				{
					if (GameManager.AllItems.TryGetItemData(itemDispenser.itemType, out _003CitemData_003E5__2))
					{
						goto IL_0063;
					}
					UnityEngine.Debug.LogError($"Could not find data for item {itemDispenser.itemType}");
				}
				goto end_IL_000e;
				IL_0063:
				try
				{
					UniTask.Awaiter awaiter;
					if (num != 0)
					{
						itemDispenser.isDispensing = true;
						itemDispenser._003CCmdDispenseItemFor_003Eg__DisableInteractionTemporarily_007C20_0(itemDispenser.cooldown + 0.5f);
						itemDispenser._003CCmdDispenseItemFor_003Eg__ServerStartDispensingForAllClients_007C20_1();
						awaiter = UniTask.WaitForSeconds(0.5f).GetAwaiter();
						if (!awaiter.IsCompleted)
						{
							num = (_003C_003E1__state = 0);
							_003C_003Eu__1 = awaiter;
							_003C_003Et__builder.AwaitUnsafeOnCompleted(ref awaiter, ref this);
							return;
						}
					}
					else
					{
						awaiter = _003C_003Eu__1;
						_003C_003Eu__1 = default(UniTask.Awaiter);
						num = (_003C_003E1__state = -1);
					}
					awaiter.GetResult();
					VfxManager.ServerPlayPooledVfxForAllClients(VfxType.CoffeeDispenserEnd, itemDispenser.transform.position, itemDispenser.transform.rotation);
					CourseManager.ServerSpawnItem(itemDispenser.itemType, _003CitemData_003E5__2.MaxUses, itemDispenser.dispensingPoint.position, itemDispenser.dispensingPoint.rotation, itemDispenser.dispenseLinearSpeed * itemDispenser.dispensingPoint.forward, itemDispenser.dispenseAngularSpeed * itemDispenser.dispensingPoint.right, ItemUseId.Invalid, null);
				}
				finally
				{
					if (num < 0)
					{
						itemDispenser.isDispensing = false;
					}
				}
				end_IL_000e:;
			}
			catch (Exception exception)
			{
				_003C_003E1__state = -2;
				_003CitemData_003E5__2 = null;
				_003C_003Et__builder.SetException(exception);
				return;
			}
			_003C_003E1__state = -2;
			_003CitemData_003E5__2 = null;
			_003C_003Et__builder.SetResult();
		}

		void IAsyncStateMachine.MoveNext()
		{
			//ILSpy generated this explicit interface implementation from .override directive in MoveNext
			this.MoveNext();
		}

		[DebuggerHidden]
		private void SetStateMachine(IAsyncStateMachine stateMachine)
		{
			_003C_003Et__builder.SetStateMachine(stateMachine);
		}

		void IAsyncStateMachine.SetStateMachine(IAsyncStateMachine stateMachine)
		{
			//ILSpy generated this explicit interface implementation from .override directive in SetStateMachine
			this.SetStateMachine(stateMachine);
		}
	}

	[SerializeField]
	private ItemType itemType;

	[SerializeField]
	private Transform dispensingPoint;

	[SerializeField]
	private float dispenseLinearSpeed;

	[SerializeField]
	private float dispenseAngularSpeed;

	[SerializeField]
	private float cooldown;

	[SerializeField]
	private CoffeeDispenserVfx vfx;

	[SyncVar]
	private bool isInteractionEnabled = true;

	private bool isDispensing;

	private FMOD.Studio.EventInstance coffeeDispenserActivationInstance;

	public Entity AsEntity { get; private set; }

	public bool IsInteractionEnabled => isInteractionEnabled;

	public LocalizedString InteractString => Localization.UI.PROMPT_Activate_Ref;

	public bool NetworkisInteractionEnabled
	{
		get
		{
			return isInteractionEnabled;
		}
		[param: In]
		set
		{
			GeneratedSyncVarSetter(value, ref isInteractionEnabled, 1uL, null);
		}
	}

	private void Awake()
	{
		AsEntity = GetComponent<Entity>();
	}

	private void OnDestroy()
	{
		if (coffeeDispenserActivationInstance.isValid())
		{
			coffeeDispenserActivationInstance.stop(FMOD.Studio.STOP_MODE.ALLOWFADEOUT);
		}
	}

	public void LocalPlayerInteract()
	{
		CmdDispenseItemFor();
	}

	[AsyncStateMachine(typeof(_003CCmdDispenseItemFor_003Ed__20))]
	[Command(requiresAuthority = false)]
	public void CmdDispenseItemFor()
	{
		if (base.isServer && base.isClient)
		{
			UserCode_CmdDispenseItemFor();
			return;
		}
		NetworkWriterPooled writer = NetworkWriterPool.Get();
		SendCommandInternal("System.Void ItemDispenser::CmdDispenseItemFor()", -1266037394, writer, 0, requiresAuthority: false);
		NetworkWriterPool.Return(writer);
	}

	[TargetRpc]
	private void RpcStartDispensing(NetworkConnectionToClient connection)
	{
		NetworkWriterPooled writer = NetworkWriterPool.Get();
		SendTargetRPCInternal(connection, "System.Void ItemDispenser::RpcStartDispensing(Mirror.NetworkConnectionToClient)", -1759443417, writer, 0);
		NetworkWriterPool.Return(writer);
	}

	private void StartDispensingInternal()
	{
		VfxManager.PlayPooledVfxLocalOnly(VfxType.BallDispenserStart, base.transform.position, base.transform.rotation);
		vfx.Dispensing();
		coffeeDispenserActivationInstance = RuntimeManager.CreateInstance(GameManager.AudioSettings.CoffeeDispenserActivationEvent);
		RuntimeManager.AttachInstanceToGameObject(coffeeDispenserActivationInstance, AsEntity.TargetReticlePosition.gameObject);
		coffeeDispenserActivationInstance.start();
		coffeeDispenserActivationInstance.release();
	}

	[CompilerGenerated]
	private async void _003CCmdDispenseItemFor_003Eg__DisableInteractionTemporarily_007C20_0(float duration)
	{
		NetworkisInteractionEnabled = false;
		for (float time = 0f; time < duration; time += Time.deltaTime)
		{
			await UniTask.Yield();
			if (this == null)
			{
				return;
			}
		}
		NetworkisInteractionEnabled = true;
	}

	[CompilerGenerated]
	private void _003CCmdDispenseItemFor_003Eg__ServerStartDispensingForAllClients_007C20_1()
	{
		StartDispensingInternal();
		foreach (NetworkConnectionToClient value in NetworkServer.connections.Values)
		{
			if (value != NetworkServer.localConnection)
			{
				RpcStartDispensing(value);
			}
		}
	}

	public override bool Weaved()
	{
		return true;
	}

	protected async void UserCode_CmdDispenseItemFor()
	{
		if (!isInteractionEnabled || isDispensing)
		{
			return;
		}
		if (!GameManager.AllItems.TryGetItemData(itemType, out var itemData))
		{
			UnityEngine.Debug.LogError($"Could not find data for item {itemType}");
			return;
		}
		try
		{
			isDispensing = true;
			DisableInteractionTemporarily(cooldown + 0.5f);
			ServerStartDispensingForAllClients();
			await UniTask.WaitForSeconds(0.5f);
			VfxManager.ServerPlayPooledVfxForAllClients(VfxType.CoffeeDispenserEnd, base.transform.position, base.transform.rotation);
			CourseManager.ServerSpawnItem(itemType, itemData.MaxUses, dispensingPoint.position, dispensingPoint.rotation, dispenseLinearSpeed * dispensingPoint.forward, dispenseAngularSpeed * dispensingPoint.right, ItemUseId.Invalid, null);
		}
		finally
		{
			isDispensing = false;
		}
		async void DisableInteractionTemporarily(float duration)
		{
			NetworkisInteractionEnabled = false;
			for (float time = 0f; time < duration; time += Time.deltaTime)
			{
				await UniTask.Yield();
				if (this == null)
				{
					return;
				}
			}
			NetworkisInteractionEnabled = true;
		}
		void ServerStartDispensingForAllClients()
		{
			StartDispensingInternal();
			foreach (NetworkConnectionToClient value in NetworkServer.connections.Values)
			{
				if (value != NetworkServer.localConnection)
				{
					RpcStartDispensing(value);
				}
			}
		}
	}

	protected static void InvokeUserCode_CmdDispenseItemFor(NetworkBehaviour obj, NetworkReader reader, NetworkConnectionToClient senderConnection)
	{
		if (!NetworkServer.active)
		{
			UnityEngine.Debug.LogError("Command CmdDispenseItemFor called on client.");
		}
		else
		{
			((ItemDispenser)obj).UserCode_CmdDispenseItemFor();
		}
	}

	protected void UserCode_RpcStartDispensing__NetworkConnectionToClient(NetworkConnectionToClient connection)
	{
		StartDispensingInternal();
	}

	protected static void InvokeUserCode_RpcStartDispensing__NetworkConnectionToClient(NetworkBehaviour obj, NetworkReader reader, NetworkConnectionToClient senderConnection)
	{
		if (!NetworkClient.active)
		{
			UnityEngine.Debug.LogError("TargetRPC RpcStartDispensing called on server.");
		}
		else
		{
			((ItemDispenser)obj).UserCode_RpcStartDispensing__NetworkConnectionToClient(null);
		}
	}

	static ItemDispenser()
	{
		RemoteProcedureCalls.RegisterCommand(typeof(ItemDispenser), "System.Void ItemDispenser::CmdDispenseItemFor()", InvokeUserCode_CmdDispenseItemFor, requiresAuthority: false);
		RemoteProcedureCalls.RegisterRpc(typeof(ItemDispenser), "System.Void ItemDispenser::RpcStartDispensing(Mirror.NetworkConnectionToClient)", InvokeUserCode_RpcStartDispensing__NetworkConnectionToClient);
	}

	public override void SerializeSyncVars(NetworkWriter writer, bool forceAll)
	{
		base.SerializeSyncVars(writer, forceAll);
		if (forceAll)
		{
			writer.WriteBool(isInteractionEnabled);
			return;
		}
		writer.WriteVarULong(syncVarDirtyBits);
		if ((syncVarDirtyBits & 1L) != 0L)
		{
			writer.WriteBool(isInteractionEnabled);
		}
	}

	public override void DeserializeSyncVars(NetworkReader reader, bool initialState)
	{
		base.DeserializeSyncVars(reader, initialState);
		if (initialState)
		{
			GeneratedSyncVarDeserialize(ref isInteractionEnabled, null, reader.ReadBool());
			return;
		}
		long num = (long)reader.ReadVarULong();
		if ((num & 1L) != 0L)
		{
			GeneratedSyncVarDeserialize(ref isInteractionEnabled, null, reader.ReadBool());
		}
	}
}
