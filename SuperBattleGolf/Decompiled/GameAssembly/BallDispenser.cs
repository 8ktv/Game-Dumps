using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using Cysharp.Threading.Tasks;
using FMODUnity;
using Mirror;
using Mirror.RemoteCalls;
using UnityEngine;
using UnityEngine.Localization;

public class BallDispenser : NetworkBehaviour, IInteractable
{
	[StructLayout(LayoutKind.Auto)]
	[CompilerGenerated]
	private struct _003CCmdDispenseBallFor_003Ed__24 : IAsyncStateMachine
	{
		public int _003C_003E1__state;

		public AsyncVoidMethodBuilder _003C_003Et__builder;

		public PlayerGolfer player;

		public BallDispenser _003C_003E4__this;

		private UniTask.Awaiter _003C_003Eu__1;

		private void MoveNext()
		{
			int num = _003C_003E1__state;
			BallDispenser ballDispenser = _003C_003E4__this;
			try
			{
				if (num == 0 || (!(player == null) && ballDispenser.isInteractionEnabled && !ballDispenser.isDispensing))
				{
					try
					{
						UniTask.Awaiter awaiter;
						if (num != 0)
						{
							ballDispenser.isDispensing = true;
							ballDispenser._003CCmdDispenseBallFor_003Eg__DisableInteractionTemporarily_007C24_0(ballDispenser.cooldown + ballDispenser.vfx.BloatPartADuration);
							ballDispenser._003CCmdDispenseBallFor_003Eg__ServerStartDispensingForAllClients_007C24_1();
							awaiter = UniTask.WaitForSeconds(ballDispenser.vfx.BloatPartADuration).GetAwaiter();
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
						VfxManager.ServerPlayPooledVfxForAllClients(VfxType.BallDispenserEnd, ballDispenser.transform.position, ballDispenser.transform.rotation);
						GolfBall golfBall = ((player.OwnBall != null) ? player.OwnBall : player.ServerSpawnBall(ballDispenser.dispensingPoint.position, ballDispenser.dispensingPoint.rotation));
						golfBall.ServerInformNoLongerInHole();
						ballDispenser.DispenseBallInternal(golfBall);
						Dictionary<int, NetworkConnectionToClient>.ValueCollection.Enumerator enumerator = NetworkServer.connections.Values.GetEnumerator();
						try
						{
							while (enumerator.MoveNext())
							{
								NetworkConnectionToClient current = enumerator.Current;
								if (current != NetworkServer.localConnection)
								{
									ballDispenser.RpcDispenseBall(current, golfBall);
								}
							}
						}
						finally
						{
							if (num < 0)
							{
								((IDisposable)enumerator/*cast due to .constrained prefix*/).Dispose();
							}
						}
						CourseManager.InformBallDispensed(player);
					}
					finally
					{
						if (num < 0)
						{
							ballDispenser.isDispensing = false;
						}
					}
				}
			}
			catch (Exception exception)
			{
				_003C_003E1__state = -2;
				_003C_003Et__builder.SetException(exception);
				return;
			}
			_003C_003E1__state = -2;
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
	private Transform dispensingPoint;

	[SerializeField]
	private float dispenseLinearSpeed;

	[SerializeField]
	private float dispenseAngularSpeed;

	[SerializeField]
	private float cooldown;

	[SerializeField]
	private BallDispenserVfx vfx;

	[SyncVar]
	private bool isInteractionEnabled = true;

	private bool isDispensing;

	private WorldspaceIconUi worldspaceIcon;

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
		AsEntity.WillBeDestroyed += OnWillBeDestroyed;
		TutorialManager.ObjectiveChanged += OnTutorialObjectiveChanged;
	}

	private void Start()
	{
		DrivingRangeManager.RegisterBallDispenser(this);
	}

	private void OnWillBeDestroyed()
	{
		HideWorldspaceIcon();
		TutorialManager.ObjectiveChanged -= OnTutorialObjectiveChanged;
	}

	private void OnDestroy()
	{
		DrivingRangeManager.DeregisterBallDispenser(this);
	}

	public void ShowWorldspaceIcon()
	{
		if (!(worldspaceIcon != null))
		{
			worldspaceIcon = WorldspaceIconManager.GetUnusedIcon();
			InitializeWorldspaceIcon();
		}
	}

	public void HideWorldspaceIcon()
	{
		if (!(worldspaceIcon == null))
		{
			WorldspaceIconManager.ReturnIcon(worldspaceIcon);
			worldspaceIcon = null;
		}
	}

	private void InitializeWorldspaceIcon()
	{
		if (!(worldspaceIcon == null))
		{
			worldspaceIcon.Initialize(WorldspaceIconManager.BallDispenserIconSettings, base.transform, GameManager.LocalPlayerInfo.transform, (TutorialManager.ActiveObjective == TutorialObjective.GetBall) ? WorldspaceIconManager.ObjectiveIcon : WorldspaceIconManager.BallDispenserIcon);
		}
	}

	public void LocalPlayerInteract()
	{
		CmdDispenseBallFor(GameManager.LocalPlayerAsGolfer);
	}

	[AsyncStateMachine(typeof(_003CCmdDispenseBallFor_003Ed__24))]
	[Command(requiresAuthority = false)]
	private void CmdDispenseBallFor(PlayerGolfer player)
	{
		if (base.isServer && base.isClient)
		{
			UserCode_CmdDispenseBallFor__PlayerGolfer(player);
			return;
		}
		NetworkWriterPooled writer = NetworkWriterPool.Get();
		writer.WriteNetworkBehaviour(player);
		SendCommandInternal("System.Void BallDispenser::CmdDispenseBallFor(PlayerGolfer)", 225053472, writer, 0, requiresAuthority: false);
		NetworkWriterPool.Return(writer);
	}

	[TargetRpc]
	private void RpcStartDispensing(NetworkConnectionToClient connection)
	{
		NetworkWriterPooled writer = NetworkWriterPool.Get();
		SendTargetRPCInternal(connection, "System.Void BallDispenser::RpcStartDispensing(Mirror.NetworkConnectionToClient)", -1859526363, writer, 0);
		NetworkWriterPool.Return(writer);
	}

	private void StartDispensingInternal()
	{
		VfxManager.PlayPooledVfxLocalOnly(VfxType.BallDispenserStart, base.transform.position, base.transform.rotation);
		vfx.Dispensing();
		RuntimeManager.PlayOneShot(GameManager.AudioSettings.BallDispenserActivationEvent, dispensingPoint.position);
	}

	[TargetRpc]
	private void RpcDispenseBall(NetworkConnectionToClient connection, GolfBall ball)
	{
		NetworkWriterPooled writer = NetworkWriterPool.Get();
		writer.WriteNetworkBehaviour(ball);
		SendTargetRPCInternal(connection, "System.Void BallDispenser::RpcDispenseBall(Mirror.NetworkConnectionToClient,GolfBall)", 772664686, writer, 0);
		NetworkWriterPool.Return(writer);
	}

	private void DispenseBallInternal(GolfBall ball)
	{
		ball.AsEntity.InformWillTeleport();
		dispensingPoint.GetPositionAndRotation(out var position, out var rotation);
		ball.transform.SetPositionAndRotation(position, rotation);
		ball.Rigidbody.position = position;
		ball.Rigidbody.rotation = rotation;
		ball.AsEntity.InformTeleported();
		ball.AsEntity.TemporarilyIgnoreCollisionsWith(AsEntity, 0.5f);
		ball.Rigidbody.linearVelocity = dispenseLinearSpeed * dispensingPoint.forward;
		ball.Rigidbody.angularVelocity = dispenseAngularSpeed * dispensingPoint.right;
		ball.OnRespawned();
		TutorialManager.CompleteObjective(TutorialObjective.GetBall);
	}

	private void OnTutorialObjectiveChanged(TutorialObjective previousObjective, TutorialObjective currentObjective)
	{
		if (previousObjective == TutorialObjective.GetBall || currentObjective == TutorialObjective.GetBall)
		{
			InitializeWorldspaceIcon();
		}
	}

	[CompilerGenerated]
	private async void _003CCmdDispenseBallFor_003Eg__DisableInteractionTemporarily_007C24_0(float duration)
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
	private void _003CCmdDispenseBallFor_003Eg__ServerStartDispensingForAllClients_007C24_1()
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

	protected async void UserCode_CmdDispenseBallFor__PlayerGolfer(PlayerGolfer player)
	{
		if (player == null || !isInteractionEnabled || isDispensing)
		{
			return;
		}
		try
		{
			isDispensing = true;
			DisableInteractionTemporarily(cooldown + vfx.BloatPartADuration);
			ServerStartDispensingForAllClients();
			await UniTask.WaitForSeconds(vfx.BloatPartADuration);
			VfxManager.ServerPlayPooledVfxForAllClients(VfxType.BallDispenserEnd, base.transform.position, base.transform.rotation);
			GolfBall golfBall = ((player.OwnBall != null) ? player.OwnBall : player.ServerSpawnBall(dispensingPoint.position, dispensingPoint.rotation));
			golfBall.ServerInformNoLongerInHole();
			DispenseBallInternal(golfBall);
			foreach (NetworkConnectionToClient value in NetworkServer.connections.Values)
			{
				if (value != NetworkServer.localConnection)
				{
					RpcDispenseBall(value, golfBall);
				}
			}
			CourseManager.InformBallDispensed(player);
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
			foreach (NetworkConnectionToClient value2 in NetworkServer.connections.Values)
			{
				if (value2 != NetworkServer.localConnection)
				{
					RpcStartDispensing(value2);
				}
			}
		}
	}

	protected static void InvokeUserCode_CmdDispenseBallFor__PlayerGolfer(NetworkBehaviour obj, NetworkReader reader, NetworkConnectionToClient senderConnection)
	{
		if (!NetworkServer.active)
		{
			UnityEngine.Debug.LogError("Command CmdDispenseBallFor called on client.");
		}
		else
		{
			((BallDispenser)obj).UserCode_CmdDispenseBallFor__PlayerGolfer(reader.ReadNetworkBehaviour<PlayerGolfer>());
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
			((BallDispenser)obj).UserCode_RpcStartDispensing__NetworkConnectionToClient(null);
		}
	}

	protected void UserCode_RpcDispenseBall__NetworkConnectionToClient__GolfBall(NetworkConnectionToClient connection, GolfBall ball)
	{
		DispenseBallInternal(ball);
	}

	protected static void InvokeUserCode_RpcDispenseBall__NetworkConnectionToClient__GolfBall(NetworkBehaviour obj, NetworkReader reader, NetworkConnectionToClient senderConnection)
	{
		if (!NetworkClient.active)
		{
			UnityEngine.Debug.LogError("TargetRPC RpcDispenseBall called on server.");
		}
		else
		{
			((BallDispenser)obj).UserCode_RpcDispenseBall__NetworkConnectionToClient__GolfBall(null, reader.ReadNetworkBehaviour<GolfBall>());
		}
	}

	static BallDispenser()
	{
		RemoteProcedureCalls.RegisterCommand(typeof(BallDispenser), "System.Void BallDispenser::CmdDispenseBallFor(PlayerGolfer)", InvokeUserCode_CmdDispenseBallFor__PlayerGolfer, requiresAuthority: false);
		RemoteProcedureCalls.RegisterRpc(typeof(BallDispenser), "System.Void BallDispenser::RpcStartDispensing(Mirror.NetworkConnectionToClient)", InvokeUserCode_RpcStartDispensing__NetworkConnectionToClient);
		RemoteProcedureCalls.RegisterRpc(typeof(BallDispenser), "System.Void BallDispenser::RpcDispenseBall(Mirror.NetworkConnectionToClient,GolfBall)", InvokeUserCode_RpcDispenseBall__NetworkConnectionToClient__GolfBall);
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
