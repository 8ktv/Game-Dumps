using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using Cysharp.Threading.Tasks;
using FMODUnity;
using Mirror;
using Mirror.RemoteCalls;
using UnityEngine;

public class GolfHole : NetworkBehaviour
{
	[StructLayout(LayoutKind.Auto)]
	[CompilerGenerated]
	private struct _003CServerOnBallScored_003Ed__26 : IAsyncStateMachine
	{
		public int _003C_003E1__state;

		public AsyncVoidMethodBuilder _003C_003Et__builder;

		public GolfBall ball;

		public GolfHole _003C_003E4__this;

		private BallVfxSettings _003CvfxSettings_003E5__2;

		private UniTask.Awaiter _003C_003Eu__1;

		private void MoveNext()
		{
			int num = _003C_003E1__state;
			GolfHole golfHole = _003C_003E4__this;
			try
			{
				UniTask.Awaiter awaiter;
				if (num != 0)
				{
					_003CvfxSettings_003E5__2 = ((ball != null && ball.VfxSettings != null) ? ball.VfxSettings : VfxPersistentData.DefaultBallVfxSettings);
					VfxManager.ServerPlayPooledVfxForAllClients(_003CvfxSettings_003E5__2.WinStart, golfHole.transform.position, Quaternion.identity);
					golfHole.PlayBallInHoleSoundInternal();
					Dictionary<int, NetworkConnectionToClient>.ValueCollection.Enumerator enumerator = NetworkServer.connections.Values.GetEnumerator();
					try
					{
						while (enumerator.MoveNext())
						{
							NetworkConnectionToClient current = enumerator.Current;
							if (current != NetworkServer.localConnection)
							{
								golfHole.RpcPlayBallInHoleSound(current);
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
				if (!(golfHole == null))
				{
					golfHole._003CServerOnBallScored_003Eg__ApplyScoreKnockback_007C26_0();
					VfxManager.ServerPlayPooledVfxForAllClients(_003CvfxSettings_003E5__2.WinEnd, golfHole.transform.position, Quaternion.identity);
				}
			}
			catch (Exception exception)
			{
				_003C_003E1__state = -2;
				_003CvfxSettings_003E5__2 = null;
				_003C_003Et__builder.SetException(exception);
				return;
			}
			_003C_003E1__state = -2;
			_003CvfxSettings_003E5__2 = null;
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
	private bool isMainHole;

	[SerializeField]
	private GameObject beamVfx;

	[SerializeField]
	private Transform flag;

	[SerializeField]
	private Collider collider;

	[SerializeField]
	private GolfHoleSettings settings;

	private WorldspaceIconUi worldspaceIcon;

	private float defaultFlagLocalHeight;

	private bool areAnyPlayersOnGreen;

	private bool areAnyPlayersInGreenTrigger;

	[SyncVar(hook = "OnFlagIsRaisedChanged")]
	private bool isFlagRaised;

	private Coroutine moveFlagRoutine;

	public Action<bool, bool> _Mirror_SyncVarHookDelegate_isFlagRaised;

	public bool IsMainHole => isMainHole;

	public GolfHoleSettings Settings => settings;

	public bool NetworkisFlagRaised
	{
		get
		{
			return isFlagRaised;
		}
		[param: In]
		set
		{
			GeneratedSyncVarSetter(value, ref isFlagRaised, 1uL, _Mirror_SyncVarHookDelegate_isFlagRaised);
		}
	}

	[CCommand("triggerHoleScoreBlast", "", false, false, serverOnly = true)]
	private static void TriggerHoleScoreBlast()
	{
		if (!(GolfHoleManager.MainHole == null))
		{
			GolfHoleManager.MainHole.ServerOnBallScored(null);
		}
	}

	private void Start()
	{
		if (isMainHole)
		{
			GolfHoleManager.RegisterMainHole(this);
		}
		else
		{
			beamVfx.SetActive(value: false);
		}
		PhysicsManager.RegisterHoleCollider(collider);
		UpdateWorldspaceIcon();
		GameManager.LocalPlayerRegistered += OnLocalPlayerRegistered;
		PlayerGolfer.LocalPlayerOwnBallChanged += OnLocalPlayerOwnBallChanged;
		PlayerSpectator.LocalPlayerIsSpectatingChanged += OnLocalPlayerIsSpectatingChanged;
		PlayerSpectator.LocalPlayerSetSpectatingTarget += OnLocalPlayerSetSpectatingTarget;
		PlayerSpectator.LocalPlayerStoppedSpectating += OnLocalPlayerStoppedSpectating;
		GolfBall.LocalPlayerBallIsHiddenChanged += OnLocalPlayerBallIsHiddenChanged;
		if (isMainHole)
		{
			TutorialManager.ObjectiveChanged += OnTutorialObjectiveChanged;
		}
	}

	public void OnWillBeDestroyed()
	{
		HideWorldspaceIcon();
		GameManager.LocalPlayerRegistered -= OnLocalPlayerRegistered;
		PlayerGolfer.LocalPlayerOwnBallChanged -= OnLocalPlayerOwnBallChanged;
		PlayerSpectator.LocalPlayerIsSpectatingChanged -= OnLocalPlayerIsSpectatingChanged;
		PlayerSpectator.LocalPlayerSetSpectatingTarget -= OnLocalPlayerSetSpectatingTarget;
		PlayerSpectator.LocalPlayerStoppedSpectating -= OnLocalPlayerStoppedSpectating;
		GolfBall.LocalPlayerBallIsHiddenChanged -= OnLocalPlayerBallIsHiddenChanged;
		if (isMainHole)
		{
			TutorialManager.ObjectiveChanged -= OnTutorialObjectiveChanged;
		}
		if (!BNetworkManager.IsChangingSceneOrShuttingDown)
		{
			if (isMainHole)
			{
				GolfHoleManager.DeregisterMainHole(this);
			}
			PhysicsManager.DeregisterHoleCollider(collider);
		}
	}

	public override void OnStartServer()
	{
		defaultFlagLocalHeight = flag.localPosition.y;
	}

	public override void OnStartClient()
	{
		if (isFlagRaised)
		{
			if (moveFlagRoutine != null)
			{
				StopCoroutine(moveFlagRoutine);
			}
			Vector3 localPosition = flag.localPosition;
			localPosition.y = GameManager.GolfSettings.PlayersOnGreenFlagRaiseHeight;
			flag.localPosition = localPosition;
		}
	}

	[Server]
	public void ServerInformFellIn(Entity entity)
	{
		if (!NetworkServer.active)
		{
			UnityEngine.Debug.LogWarning("[Server] function 'System.Void GolfHole::ServerInformFellIn(Entity)' called when server was not active");
		}
		else if (entity.IsGolfBall)
		{
			HandleBall(entity.AsGolfBall);
		}
		else if (entity.IsPlayer)
		{
			HandlePlayer(entity.PlayerInfo);
		}
		else
		{
			entity.DestroyEntity();
		}
		void HandleBall(GolfBall ball)
		{
			ball.ServerInformEnteredHole(this);
			ServerOnBallScored(ball);
		}
		static void HandlePlayer(PlayerInfo player)
		{
			player.AsGolfer.ServerEliminate(EliminationReason.FellIntoHole);
		}
	}

	[Server]
	public void ServerSetAreAnyPlayersOnGreen(bool areAnyPlayersOnGreen)
	{
		if (!NetworkServer.active)
		{
			UnityEngine.Debug.LogWarning("[Server] function 'System.Void GolfHole::ServerSetAreAnyPlayersOnGreen(System.Boolean)' called when server was not active");
			return;
		}
		this.areAnyPlayersOnGreen = areAnyPlayersOnGreen;
		ServerUpdateIsFlagRaised();
	}

	[Server]
	public void ServerSetAreAnyPlayersInGreenTrigger(bool areAnyPlayersInGreenTrigger)
	{
		if (!NetworkServer.active)
		{
			UnityEngine.Debug.LogWarning("[Server] function 'System.Void GolfHole::ServerSetAreAnyPlayersInGreenTrigger(System.Boolean)' called when server was not active");
			return;
		}
		this.areAnyPlayersInGreenTrigger = areAnyPlayersInGreenTrigger;
		ServerUpdateIsFlagRaised();
	}

	[Server]
	private void ServerUpdateIsFlagRaised()
	{
		if (!NetworkServer.active)
		{
			UnityEngine.Debug.LogWarning("[Server] function 'System.Void GolfHole::ServerUpdateIsFlagRaised()' called when server was not active");
		}
		else
		{
			NetworkisFlagRaised = areAnyPlayersOnGreen || areAnyPlayersInGreenTrigger;
		}
	}

	public bool IsPointInGreenTrigger(Vector3 worldPoint)
	{
		if (!this.TryGetComponentInChildren<GolfHoleGreenTrigger>(out var foundComponent, includeInactive: true))
		{
			return false;
		}
		return foundComponent.IsPointInTrigger(worldPoint);
	}

	private IEnumerator MoveFlagRoutine(bool isRaised)
	{
		float initialHeight = flag.localPosition.y;
		float targetHeight = (isRaised ? GameManager.GolfSettings.PlayersOnGreenFlagRaiseHeight : defaultFlagLocalHeight);
		float duration = (isRaised ? GameManager.GolfSettings.PlayersOnGreenFlagRaiseDuration : GameManager.GolfSettings.PlayersOnGreenFlagLowerDuration);
		bool isRaising = targetHeight > initialHeight;
		Vector3 flagPosition = flag.localPosition;
		for (float time = 0f; time < duration; time += Time.deltaTime)
		{
			float t = time / duration;
			float t2 = (isRaising ? BMath.EaseOut(t) : BMath.EaseIn(t));
			flagPosition.y = BMath.Lerp(initialHeight, targetHeight, t2);
			flag.localPosition = flagPosition;
			yield return null;
		}
		flagPosition.y = targetHeight;
		flag.localPosition = flagPosition;
	}

	[AsyncStateMachine(typeof(_003CServerOnBallScored_003Ed__26))]
	[Server]
	private void ServerOnBallScored(GolfBall ball)
	{
		if (!NetworkServer.active)
		{
			UnityEngine.Debug.LogWarning("[Server] function 'System.Void GolfHole::ServerOnBallScored(GolfBall)' called when server was not active");
			return;
		}
		_003CServerOnBallScored_003Ed__26 stateMachine = default(_003CServerOnBallScored_003Ed__26);
		stateMachine._003C_003Et__builder = AsyncVoidMethodBuilder.Create();
		stateMachine._003C_003E4__this = this;
		stateMachine.ball = ball;
		stateMachine._003C_003E1__state = -1;
		stateMachine._003C_003Et__builder.Start(ref stateMachine);
	}

	[TargetRpc]
	private void RpcPlayBallInHoleSound(NetworkConnectionToClient connection)
	{
		NetworkWriterPooled writer = NetworkWriterPool.Get();
		SendTargetRPCInternal(connection, "System.Void GolfHole::RpcPlayBallInHoleSound(Mirror.NetworkConnectionToClient)", 1326992246, writer, 0);
		NetworkWriterPool.Return(writer);
	}

	private void PlayBallInHoleSoundInternal()
	{
		RuntimeManager.PlayOneShot(GameManager.AudioSettings.BallInHoleEvent, base.transform.position);
	}

	private void HideWorldspaceIcon()
	{
		if (!(worldspaceIcon == null))
		{
			WorldspaceIconManager.ReturnIcon(worldspaceIcon);
			worldspaceIcon = null;
		}
	}

	private void UpdateWorldspaceIcon()
	{
		bool flag = worldspaceIcon != null;
		bool flag2 = ShouldHaveIcon();
		if (flag2 != flag)
		{
			if (flag2)
			{
				worldspaceIcon = WorldspaceIconManager.GetUnusedIcon();
				InitializeWorldspaceIcon();
			}
			else
			{
				HideWorldspaceIcon();
			}
		}
		bool ShouldHaveIcon()
		{
			if (!isMainHole)
			{
				return false;
			}
			if (SingletonBehaviour<DrivingRangeManager>.HasInstance)
			{
				if (GameManager.LocalPlayerAsGolfer == null)
				{
					return false;
				}
				if (GameManager.LocalPlayerAsGolfer.OwnBall == null)
				{
					return false;
				}
				if (GameManager.LocalPlayerAsGolfer.OwnBall.IsHidden)
				{
					return false;
				}
			}
			return true;
		}
	}

	private void UpdateWorldspaceIconDistanceReference()
	{
		if (worldspaceIcon != null)
		{
			worldspaceIcon.SetDistanceReference(GetWorldspaceIconDistanceReference());
		}
	}

	private void InitializeWorldspaceIcon()
	{
		if (!(worldspaceIcon == null))
		{
			worldspaceIcon.Initialize(WorldspaceIconManager.HoleIconSettings, base.transform, GetWorldspaceIconDistanceReference(), (TutorialManager.ActiveObjective == TutorialObjective.FinishHole) ? WorldspaceIconManager.ObjectiveIcon : WorldspaceIconManager.HoleIcon);
		}
	}

	private Transform GetWorldspaceIconDistanceReference()
	{
		if (GameManager.LocalPlayerInfo == null)
		{
			return null;
		}
		if (GameManager.LocalPlayerAsSpectator.IsSpectating)
		{
			return GameManager.LocalPlayerAsSpectator.Target;
		}
		return GameManager.LocalPlayerInfo.transform;
	}

	private void OnLocalPlayerRegistered()
	{
		UpdateWorldspaceIconDistanceReference();
	}

	private void OnLocalPlayerOwnBallChanged()
	{
		UpdateWorldspaceIcon();
	}

	private void OnLocalPlayerIsSpectatingChanged()
	{
		UpdateWorldspaceIconDistanceReference();
	}

	private void OnLocalPlayerSetSpectatingTarget(bool isInitialTarget)
	{
		UpdateWorldspaceIconDistanceReference();
	}

	private void OnLocalPlayerStoppedSpectating()
	{
		UpdateWorldspaceIconDistanceReference();
	}

	private void OnLocalPlayerBallIsHiddenChanged()
	{
		UpdateWorldspaceIcon();
	}

	private void OnFlagIsRaisedChanged(bool previouslyRaised, bool currentlyRaised)
	{
		if (moveFlagRoutine != null)
		{
			StopCoroutine(moveFlagRoutine);
		}
		moveFlagRoutine = StartCoroutine(MoveFlagRoutine(isFlagRaised));
	}

	private void OnTutorialObjectiveChanged(TutorialObjective previousObjective, TutorialObjective currentObjective)
	{
		if (previousObjective == TutorialObjective.FinishHole || currentObjective == TutorialObjective.FinishHole)
		{
			InitializeWorldspaceIcon();
		}
	}

	public GolfHole()
	{
		_Mirror_SyncVarHookDelegate_isFlagRaised = OnFlagIsRaisedChanged;
	}

	[CompilerGenerated]
	private void _003CServerOnBallScored_003Eg__ApplyScoreKnockback_007C26_0()
	{
		int num = Physics.OverlapSphereNonAlloc(base.transform.position, settings.MaxRange, layerMask: GameManager.LayerSettings.ScoreKnockbackMask, results: PlayerGolfer.overlappingColliderBuffer, queryTriggerInteraction: QueryTriggerInteraction.Ignore);
		PlayerGolfer.processedHittableBuffer.Clear();
		for (int i = 0; i < num; i++)
		{
			if (PlayerGolfer.overlappingColliderBuffer[i].TryGetComponentInParent<Hittable>(out var foundComponent, includeInactive: true) && PlayerGolfer.processedHittableBuffer.Add(foundComponent))
			{
				foundComponent.ServerHitWithScoreKnockback(this);
			}
		}
	}

	public override bool Weaved()
	{
		return true;
	}

	protected void UserCode_RpcPlayBallInHoleSound__NetworkConnectionToClient(NetworkConnectionToClient connection)
	{
		PlayBallInHoleSoundInternal();
	}

	protected static void InvokeUserCode_RpcPlayBallInHoleSound__NetworkConnectionToClient(NetworkBehaviour obj, NetworkReader reader, NetworkConnectionToClient senderConnection)
	{
		if (!NetworkClient.active)
		{
			UnityEngine.Debug.LogError("TargetRPC RpcPlayBallInHoleSound called on server.");
		}
		else
		{
			((GolfHole)obj).UserCode_RpcPlayBallInHoleSound__NetworkConnectionToClient(null);
		}
	}

	static GolfHole()
	{
		RemoteProcedureCalls.RegisterRpc(typeof(GolfHole), "System.Void GolfHole::RpcPlayBallInHoleSound(Mirror.NetworkConnectionToClient)", InvokeUserCode_RpcPlayBallInHoleSound__NetworkConnectionToClient);
	}

	public override void SerializeSyncVars(NetworkWriter writer, bool forceAll)
	{
		base.SerializeSyncVars(writer, forceAll);
		if (forceAll)
		{
			writer.WriteBool(isFlagRaised);
			return;
		}
		writer.WriteVarULong(syncVarDirtyBits);
		if ((syncVarDirtyBits & 1L) != 0L)
		{
			writer.WriteBool(isFlagRaised);
		}
	}

	public override void DeserializeSyncVars(NetworkReader reader, bool initialState)
	{
		base.DeserializeSyncVars(reader, initialState);
		if (initialState)
		{
			GeneratedSyncVarDeserialize(ref isFlagRaised, _Mirror_SyncVarHookDelegate_isFlagRaised, reader.ReadBool());
			return;
		}
		long num = (long)reader.ReadVarULong();
		if ((num & 1L) != 0L)
		{
			GeneratedSyncVarDeserialize(ref isFlagRaised, _Mirror_SyncVarHookDelegate_isFlagRaised, reader.ReadBool());
		}
	}
}
