using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using Cysharp.Threading.Tasks;
using FMOD;
using FMOD.Studio;
using FMODUnity;
using Mirror;
using Mirror.RemoteCalls;
using Steamworks;
using UnityEngine;
using UnityEngine.Localization;
using UnityEngine.Pool;

public class CourseManager : SingletonNetworkBehaviour<CourseManager>, IBUpdateCallback, IAnyBUpdateCallback
{
	public struct PlayerState : IComparable<PlayerState>
	{
		public ulong playerGuid;

		public int joinIndex;

		public bool isConnected;

		public bool isHost;

		public bool isRespawning;

		public bool isSpectator;

		public PlayerMatchResolution matchResolution;

		public int courseScore;

		public int matchScore;

		public double scoreTimestamp;

		public int courseStrokes;

		public int matchStrokes;

		public int courseStrokesOnFinishedHoles;

		public int courseParOnFinishedHoles;

		public int eliminations;

		public int matchKnockouts;

		public int courseKnockouts;

		public int matchKnockoutStreak;

		public int courseKnockedOut;

		public int matchKnockedOut;

		public int wins;

		public int finishes;

		public int multiplayerFinishes;

		public int multiplayerFirstPlaceStreak;

		public int losses;

		public int dominatingCount;

		public StrokesUnderParType bestHoleScore;

		public float avgFinishTime;

		public float longestChipIn;

		public int itemPickups;

		public double joinTimestamp;

		public string name => GetPlayerName(this);

		public PlayerState(ulong playerGuid, int joinIndex, bool isConnected, bool isHost, bool isSpectator)
		{
			this.playerGuid = playerGuid;
			this.joinIndex = joinIndex;
			this.isConnected = isConnected;
			this.isHost = isHost;
			this.isSpectator = isSpectator;
			isRespawning = false;
			matchResolution = PlayerMatchResolution.None;
			courseScore = 0;
			matchScore = 0;
			scoreTimestamp = double.MinValue;
			courseStrokes = 0;
			matchStrokes = 0;
			courseStrokesOnFinishedHoles = 0;
			courseParOnFinishedHoles = 0;
			eliminations = 0;
			matchKnockouts = 0;
			courseKnockouts = 0;
			matchKnockoutStreak = 0;
			wins = 0;
			losses = 0;
			dominatingCount = 0;
			finishes = 0;
			multiplayerFinishes = 0;
			multiplayerFirstPlaceStreak = 0;
			bestHoleScore = StrokesUnderParType.None;
			avgFinishTime = 0f;
			longestChipIn = float.MinValue;
			itemPickups = 0;
			courseKnockedOut = 0;
			matchKnockedOut = 0;
			joinTimestamp = NetworkTime.time;
		}

		public static PlayerState GetClearedState(PlayerState state)
		{
			return new PlayerState(state.playerGuid, state.joinIndex, state.isConnected, state.isHost, state.isSpectator);
		}

		public static PlayerState GetNewMatchState(PlayerState state)
		{
			state.isRespawning = false;
			state.matchResolution = PlayerMatchResolution.None;
			state.matchScore = 0;
			state.matchKnockouts = 0;
			state.matchKnockoutStreak = 0;
			state.matchStrokes = 0;
			state.matchKnockedOut = 0;
			return state;
		}

		public readonly int CompareTo(PlayerState other)
		{
			if (courseScore != other.courseScore)
			{
				return other.courseScore.CompareTo(courseScore);
			}
			if (wins != other.wins)
			{
				return other.wins.CompareTo(wins);
			}
			if (finishes != other.finishes)
			{
				return other.finishes.CompareTo(finishes);
			}
			if (courseStrokes != other.courseStrokes)
			{
				return courseStrokes.CompareTo(other.courseStrokes);
			}
			if (courseKnockouts != other.courseKnockouts)
			{
				return other.courseKnockouts.CompareTo(courseKnockouts);
			}
			if (losses != other.losses)
			{
				return losses.CompareTo(other.losses);
			}
			if (scoreTimestamp != other.scoreTimestamp)
			{
				return scoreTimestamp.CompareTo(other.scoreTimestamp);
			}
			return joinIndex.CompareTo(other.joinIndex);
		}

		public readonly bool TryGetStrokesRelativeToPar(out int strokes)
		{
			if (courseParOnFinishedHoles <= 0)
			{
				strokes = int.MaxValue;
				return false;
			}
			strokes = courseStrokesOnFinishedHoles - courseParOnFinishedHoles;
			return true;
		}
	}

	public struct PlayerPair
	{
		public ulong playerAGuid;

		public ulong playerBGuid;

		public PlayerPair(ulong playerAGuid, ulong playerBGuid)
		{
			this.playerAGuid = playerAGuid;
			this.playerBGuid = playerBGuid;
		}

		public readonly PlayerPair Inverse()
		{
			return new PlayerPair(playerBGuid, playerAGuid);
		}

		public override readonly bool Equals(object obj)
		{
			if (obj is PlayerPair playerPair && playerAGuid == playerPair.playerAGuid)
			{
				return playerBGuid == playerPair.playerBGuid;
			}
			return false;
		}

		public override int GetHashCode()
		{
			return HashCode.Combine(playerAGuid, playerBGuid);
		}
	}

	public struct KnockoutStreak
	{
		public int persistentStreak;

		public int redShieldStreak;
	}

	[StructLayout(LayoutKind.Auto)]
	[CompilerGenerated]
	private struct _003CEndCourseInternal_003Ed__149 : IAsyncStateMachine
	{
		public int _003C_003E1__state;

		public AsyncVoidMethodBuilder _003C_003Et__builder;

		public CourseManager _003C_003E4__this;

		private UniTask.Awaiter _003C_003Eu__1;

		private void MoveNext()
		{
			int num = _003C_003E1__state;
			CourseManager courseManager = _003C_003E4__this;
			try
			{
				if (num != 0)
				{
					courseManager.RpcInformEndingCourse();
					courseManager.ClearPlayerStates();
					ServerPersistentCourseData.WritePlayerStates();
					ServerPersistentCourseData.ClearPlayerInventories();
					ServerPersistentCourseData.ResetNextHoleIndex();
					ServerPersistentCourseData.RestorePlayerJoinTimestamps();
				}
				try
				{
					UniTask.Awaiter awaiter;
					if (num != 0)
					{
						InputManager.EnableMode(InputMode.ForceDisabled);
						FullScreenMessage.Hide();
						SingletonBehaviour<PauseMenu>.Instance.gameObject.SetActive(value: false);
						LoadingScreen.Show(Time.timeScale <= 0.25f);
						awaiter = UniTask.WaitWhile(() => LoadingScreen.IsFadingScreenIn).GetAwaiter();
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
				}
				catch (Exception exception)
				{
					UnityEngine.Debug.LogError("Encountered exception while fading to course end loading screen. See the next log for details");
					UnityEngine.Debug.LogException(exception);
				}
				finally
				{
					if (num < 0)
					{
						InputManager.DisableMode(InputMode.ForceDisabled);
					}
				}
				BNetworkManager.singleton.ServerChangeScene(GameManager.DrivingRangeHoleData.Scene.Name);
			}
			catch (Exception exception2)
			{
				_003C_003E1__state = -2;
				_003C_003Et__builder.SetException(exception2);
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

	[StructLayout(LayoutKind.Auto)]
	[CompilerGenerated]
	private struct _003CRpcInformEndingCourse_003Ed__150 : IAsyncStateMachine
	{
		public int _003C_003E1__state;

		public AsyncVoidMethodBuilder _003C_003Et__builder;

		public CourseManager _003C_003E4__this;

		private UniTask.Awaiter _003C_003Eu__1;

		private void MoveNext()
		{
			int num = _003C_003E1__state;
			CourseManager courseManager = _003C_003E4__this;
			try
			{
				if (num == 0 || !courseManager.isServer)
				{
					try
					{
						UniTask.Awaiter awaiter;
						if (num != 0)
						{
							InputManager.EnableMode(InputMode.ForceDisabled);
							LoadingScreen.Show(Time.timeScale <= 0.25f);
							awaiter = UniTask.WaitWhile(() => LoadingScreen.IsFadingScreenIn).GetAwaiter();
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
					}
					catch (Exception exception)
					{
						UnityEngine.Debug.LogError("Encountered exception while fading to course end loading screen. See the next log for details");
						UnityEngine.Debug.LogException(exception);
					}
					finally
					{
						if (num < 0)
						{
							InputManager.DisableMode(InputMode.ForceDisabled);
						}
					}
				}
			}
			catch (Exception exception2)
			{
				_003C_003E1__state = -2;
				_003C_003Et__builder.SetException(exception2);
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

	[StructLayout(LayoutKind.Auto)]
	[CompilerGenerated]
	private struct _003CRpcInformStartingCourse_003Ed__148 : IAsyncStateMachine
	{
		public int _003C_003E1__state;

		public AsyncVoidMethodBuilder _003C_003Et__builder;

		public CourseManager _003C_003E4__this;

		private UniTask.Awaiter _003C_003Eu__1;

		private void MoveNext()
		{
			int num = _003C_003E1__state;
			CourseManager courseManager = _003C_003E4__this;
			try
			{
				if (num == 0)
				{
					goto IL_0025;
				}
				TutorialManager.CompleteObjective(TutorialObjective.StartMatch);
				if (!courseManager.isServer)
				{
					goto IL_0025;
				}
				goto end_IL_000e;
				IL_0025:
				try
				{
					UniTask.Awaiter awaiter;
					if (num != 0)
					{
						InputManager.EnableMode(InputMode.ForceDisabled);
						LoadingScreen.Show(Time.timeScale <= 0.25f);
						awaiter = UniTask.WaitWhile(() => LoadingScreen.IsFadingScreenIn).GetAwaiter();
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
				}
				catch (Exception exception)
				{
					UnityEngine.Debug.LogError("Encountered exception while fading to course start loading screen. See the next log for details");
					UnityEngine.Debug.LogException(exception);
				}
				finally
				{
					if (num < 0)
					{
						InputManager.DisableMode(InputMode.ForceDisabled);
					}
				}
				end_IL_000e:;
			}
			catch (Exception exception2)
			{
				_003C_003E1__state = -2;
				_003C_003Et__builder.SetException(exception2);
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

	[StructLayout(LayoutKind.Auto)]
	[CompilerGenerated]
	private struct _003CRpcInformStartingNextMatch_003Ed__152 : IAsyncStateMachine
	{
		public int _003C_003E1__state;

		public AsyncVoidMethodBuilder _003C_003Et__builder;

		public CourseManager _003C_003E4__this;

		private UniTask.Awaiter _003C_003Eu__1;

		private void MoveNext()
		{
			int num = _003C_003E1__state;
			CourseManager courseManager = _003C_003E4__this;
			try
			{
				if (num == 0 || !courseManager.isServer)
				{
					try
					{
						UniTask.Awaiter awaiter;
						if (num != 0)
						{
							InputManager.EnableMode(InputMode.ForceDisabled);
							LoadingScreen.Show(Time.timeScale <= 0.25f);
							awaiter = UniTask.WaitWhile(() => LoadingScreen.IsFadingScreenIn).GetAwaiter();
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
					}
					catch (Exception exception)
					{
						UnityEngine.Debug.LogError("Encountered exception while fading to next match start loading screen. See the next log for details");
						UnityEngine.Debug.LogException(exception);
					}
					finally
					{
						if (num < 0)
						{
							InputManager.DisableMode(InputMode.ForceDisabled);
						}
					}
				}
			}
			catch (Exception exception2)
			{
				_003C_003E1__state = -2;
				_003C_003Et__builder.SetException(exception2);
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

	[StructLayout(LayoutKind.Auto)]
	[CompilerGenerated]
	private struct _003CServerStartNextMatch_003Ed__151 : IAsyncStateMachine
	{
		public int _003C_003E1__state;

		public AsyncVoidMethodBuilder _003C_003Et__builder;

		public CourseManager _003C_003E4__this;

		public bool skipPersistentInventories;

		private string _003CnextHole_003E5__2;

		private UniTask.Awaiter _003C_003Eu__1;

		private void MoveNext()
		{
			int num = _003C_003E1__state;
			CourseManager courseManager = _003C_003E4__this;
			try
			{
				if (num == 0)
				{
					goto IL_0078;
				}
				courseManager.RpcInformStartingNextMatch();
				courseManager._003CServerStartNextMatch_003Eg__ResetRedShieldKnockoutStreaks_007C151_1();
				ServerPersistentCourseData.WritePlayerStates();
				if (!skipPersistentInventories)
				{
					ServerPersistentCourseData.WritePlayerInventories();
				}
				if (!IsLastMatchOfCourse())
				{
					_003CnextHole_003E5__2 = GameManager.CurrentCourse.Holes[ServerPersistentCourseData.nextHoleIndex].Scene.Name;
					ServerPersistentCourseData.SetNextHoleIndex(ServerPersistentCourseData.nextHoleIndex + 1);
					goto IL_0078;
				}
				UnityEngine.Debug.LogError("Attempted to start next match but there are no more matches remaining in the course. Ending course instead");
				courseManager.EndCourseInternal();
				goto end_IL_000e;
				IL_0078:
				try
				{
					UniTask.Awaiter awaiter;
					if (num != 0)
					{
						InputManager.EnableMode(InputMode.ForceDisabled);
						FullScreenMessage.Hide();
						SingletonBehaviour<PauseMenu>.Instance.gameObject.SetActive(value: false);
						LoadingScreen.Show(Time.timeScale <= 0.25f);
						awaiter = UniTask.WaitWhile(() => LoadingScreen.IsFadingScreenIn).GetAwaiter();
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
				}
				catch (Exception exception)
				{
					UnityEngine.Debug.LogError("Encountered exception while fading to next match start loading screen. See the next log for details");
					UnityEngine.Debug.LogException(exception);
				}
				finally
				{
					if (num < 0)
					{
						InputManager.DisableMode(InputMode.ForceDisabled);
					}
				}
				BNetworkManager.singleton.ServerChangeScene(_003CnextHole_003E5__2);
				end_IL_000e:;
			}
			catch (Exception exception2)
			{
				_003C_003E1__state = -2;
				_003CnextHole_003E5__2 = null;
				_003C_003Et__builder.SetException(exception2);
				return;
			}
			_003C_003E1__state = -2;
			_003CnextHole_003E5__2 = null;
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

	[StructLayout(LayoutKind.Auto)]
	[CompilerGenerated]
	private struct _003CStartCourseInternal_003Ed__147 : IAsyncStateMachine
	{
		public int _003C_003E1__state;

		public AsyncVoidMethodBuilder _003C_003Et__builder;

		public CourseManager _003C_003E4__this;

		private UniTask.Awaiter _003C_003Eu__1;

		private void MoveNext()
		{
			int num = _003C_003E1__state;
			CourseManager courseManager = _003C_003E4__this;
			try
			{
				if (num != 0)
				{
					courseManager.RpcInformStartingCourse();
				}
				try
				{
					UniTask.Awaiter awaiter;
					if (num != 0)
					{
						InputManager.EnableMode(InputMode.ForceDisabled);
						FullScreenMessage.Hide();
						SingletonBehaviour<PauseMenu>.Instance.gameObject.SetActive(value: false);
						LoadingScreen.Show(Time.timeScale <= 0.25f);
						awaiter = UniTask.WaitWhile(() => LoadingScreen.IsFadingScreenIn).GetAwaiter();
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
				}
				catch (Exception exception)
				{
					UnityEngine.Debug.LogError("Encountered exception while fading to course start loading screen. See the next log for details");
					UnityEngine.Debug.LogException(exception);
				}
				finally
				{
					if (num < 0)
					{
						InputManager.DisableMode(InputMode.ForceDisabled);
					}
				}
				courseManager.ClearPlayerStates();
				ServerPersistentCourseData.WritePlayerStates();
				ServerPersistentCourseData.ClearPlayerInventories();
				courseManager.ServerStartNextMatch(skipPersistentInventories: true);
			}
			catch (Exception exception2)
			{
				_003C_003E1__state = -2;
				_003C_003Et__builder.SetException(exception2);
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

	private const float pingUpdatePeriod = 2f;

	private const float teeOffStartTimeout = 10f;

	private readonly SyncList<PlayerState> playerStates = new SyncList<PlayerState>();

	public readonly SyncDictionary<PlayerPair, KnockoutStreak> playerKnockoutStreaks = new SyncDictionary<PlayerPair, KnockoutStreak>();

	private readonly SyncDictionary<ulong, float> playerPingPerGuid = new SyncDictionary<ulong, float>();

	private readonly SyncDictionary<ulong, int> playerStateIndicesPerPlayerGuid = new SyncDictionary<ulong, int>();

	public readonly SyncHashSet<PlayerPair> playerDominations = new SyncHashSet<PlayerPair>();

	private readonly HashSet<PlayerGolfer> serverMatchParticipants = new HashSet<PlayerGolfer>();

	private readonly List<PlayerGolfer> matchScoredPlayers = new List<PlayerGolfer>();

	[SyncVar]
	private bool didAnyPlayerScore;

	private readonly HashSet<PlayerGolfer> activePlayersOnGreen = new HashSet<PlayerGolfer>();

	private readonly HashSet<GolfBall> activeBalls = new HashSet<GolfBall>();

	private readonly SyncList<GolfBall> overtimeActiveBalls = new SyncList<GolfBall>();

	private readonly Dictionary<GolfBall, float> overtimeTimeSinceMovedPerActiveBall = new Dictionary<GolfBall, float>();

	private readonly List<int> initialPlayersInMatchAwaitingSpawningConnectionIds = new List<int>();

	private readonly List<int> additionalPlayersInMatchAwaitingSpawningConnectionIds = new List<int>();

	private readonly Dictionary<int, TeeingSpot> reservedTeeingSpotsPerConnectionId = new Dictionary<int, TeeingSpot>();

	private readonly HashSet<int> initialPlayersInMatchParticipantConnectionIds = new HashSet<int>();

	private bool reservedTeeingSpotsForInitialPlayers;

	private readonly Dictionary<PlayerInfo, ItemUseId> latestValidKnockouts = new Dictionary<PlayerInfo, ItemUseId>();

	private readonly Dictionary<PlayerInfo, List<double>> recentScoredKnockoutTimestamps = new Dictionary<PlayerInfo, List<double>>();

	private readonly List<PlayerState> sortedPlayerStatesBuffer = new List<PlayerState>();

	private static readonly Dictionary<ulong, string> clientPlayerNames;

	[SyncVar(hook = "OnCurrentHoleCourseIndexChanged")]
	private int currentHoleCourseIndex = -1;

	[SyncVar(hook = "OnCurrentHoleGlobalIndexChanged")]
	private int currentHoleGlobalIndex = -1;

	[SyncVar(hook = "OnMatchStateChanged")]
	private MatchState matchState;

	[SyncVar(hook = "OnIsHoleOverviewFinishedChanged")]
	private bool isHoleOverviewFinished;

	[SyncVar(hook = "OnForceDisplayScoreboardChanged")]
	private bool forceDisplayScoreboard;

	[SyncVar(hook = "OnMarkedFirstPlacePlayerChanged")]
	private PlayerInfo markedFirstPlacePlayer;

	private PlayerState currentHoleFirstPlaceState;

	private Coroutine holeOverviewAndTeeOffCountdownRoutine;

	private Coroutine matchEndCountdownRoutine;

	private double matchInitializationTimestamp;

	private double matchStartTimestamp = double.MinValue;

	private float timeSincePingUpdate;

	private float countdownRemainingTime = float.MaxValue;

	private double teeoffEndTimestamp = double.MinValue;

	private FMOD.Studio.EventInstance holeMusicInstance;

	private PARAMETER_ID hurryUpMusicParameterId;

	private bool isPlayingHoleMusic;

	private FMOD.Studio.EventInstance announcerLineInstance;

	private readonly Queue<AnnouncerLine> announcerLineQueue = new Queue<AnnouncerLine>();

	private AnnouncerLine lastAnnouncerLinePlayed;

	private bool isPlayingQueuedAnnouncerLines;

	private readonly AntiCheatPerPlayerRateChecker serverSpawnGolfCartCommandRateLimiter = new AntiCheatPerPlayerRateChecker("Spawn golf cart", 0.5f, 2, 5, 2f);

	[CVar("simulateClientsJoiningHoleSlowly", "", "", false, true)]
	private static bool simulateClientsJoiningHoleSlowly;

	private bool localPlayerRewardedCourseBonus;

	protected NetworkBehaviourSyncVar ___markedFirstPlacePlayerNetId;

	public Action<int, int> _Mirror_SyncVarHookDelegate_currentHoleCourseIndex;

	public Action<int, int> _Mirror_SyncVarHookDelegate_currentHoleGlobalIndex;

	public Action<MatchState, MatchState> _Mirror_SyncVarHookDelegate_matchState;

	public Action<bool, bool> _Mirror_SyncVarHookDelegate_isHoleOverviewFinished;

	public Action<bool, bool> _Mirror_SyncVarHookDelegate_forceDisplayScoreboard;

	public Action<PlayerInfo, PlayerInfo> _Mirror_SyncVarHookDelegate_markedFirstPlacePlayer;

	public static SyncList<PlayerState> PlayerStates
	{
		get
		{
			if (!SingletonNetworkBehaviour<CourseManager>.HasInstance)
			{
				return null;
			}
			return SingletonNetworkBehaviour<CourseManager>.Instance.playerStates;
		}
	}

	public static SyncDictionary<PlayerPair, KnockoutStreak> PlayerKnockoutStreaks
	{
		get
		{
			if (!SingletonNetworkBehaviour<CourseManager>.HasInstance)
			{
				return null;
			}
			return SingletonNetworkBehaviour<CourseManager>.Instance.playerKnockoutStreaks;
		}
	}

	public static SyncHashSet<PlayerPair> PlayerDominations
	{
		get
		{
			if (!SingletonNetworkBehaviour<CourseManager>.HasInstance)
			{
				return null;
			}
			return SingletonNetworkBehaviour<CourseManager>.Instance.playerDominations;
		}
	}

	public static HashSet<PlayerGolfer> ServerMatchParticipants
	{
		get
		{
			if (!SingletonNetworkBehaviour<CourseManager>.HasInstance)
			{
				return null;
			}
			return SingletonNetworkBehaviour<CourseManager>.Instance.serverMatchParticipants;
		}
	}

	public static SyncDictionary<ulong, float> PlayerPingPerGuid
	{
		get
		{
			if (!SingletonNetworkBehaviour<CourseManager>.HasInstance)
			{
				return null;
			}
			return SingletonNetworkBehaviour<CourseManager>.Instance.playerPingPerGuid;
		}
	}

	public static SyncList<GolfBall> OvertimeActiveBalls
	{
		get
		{
			if (!SingletonNetworkBehaviour<CourseManager>.HasInstance)
			{
				return null;
			}
			return SingletonNetworkBehaviour<CourseManager>.Instance.overtimeActiveBalls;
		}
	}

	public static int CurrentHoleCourseIndex
	{
		get
		{
			if (!SingletonNetworkBehaviour<CourseManager>.HasInstance)
			{
				return -1;
			}
			return SingletonNetworkBehaviour<CourseManager>.Instance.currentHoleCourseIndex;
		}
	}

	public static int CurrentHoleGlobalIndex
	{
		get
		{
			if (!SingletonNetworkBehaviour<CourseManager>.HasInstance)
			{
				return -1;
			}
			return SingletonNetworkBehaviour<CourseManager>.Instance.currentHoleGlobalIndex;
		}
	}

	public static MatchState MatchState
	{
		get
		{
			if (!SingletonNetworkBehaviour<CourseManager>.HasInstance)
			{
				return MatchState.Initializing;
			}
			return SingletonNetworkBehaviour<CourseManager>.Instance.matchState;
		}
	}

	public static bool DidAnyPlayerScore
	{
		get
		{
			if (SingletonNetworkBehaviour<CourseManager>.HasInstance)
			{
				return SingletonNetworkBehaviour<CourseManager>.Instance.didAnyPlayerScore;
			}
			return false;
		}
	}

	public static bool ForceDisplayScoreboard
	{
		get
		{
			if (SingletonNetworkBehaviour<CourseManager>.HasInstance)
			{
				return SingletonNetworkBehaviour<CourseManager>.Instance.forceDisplayScoreboard;
			}
			return false;
		}
	}

	public static double TeeoffEndTimestamp
	{
		get
		{
			if (!SingletonNetworkBehaviour<CourseManager>.HasInstance)
			{
				return double.MinValue;
			}
			return SingletonNetworkBehaviour<CourseManager>.Instance.teeoffEndTimestamp;
		}
	}

	public bool NetworkdidAnyPlayerScore
	{
		get
		{
			return didAnyPlayerScore;
		}
		[param: In]
		set
		{
			GeneratedSyncVarSetter(value, ref didAnyPlayerScore, 1uL, null);
		}
	}

	public int NetworkcurrentHoleCourseIndex
	{
		get
		{
			return currentHoleCourseIndex;
		}
		[param: In]
		set
		{
			GeneratedSyncVarSetter(value, ref currentHoleCourseIndex, 2uL, _Mirror_SyncVarHookDelegate_currentHoleCourseIndex);
		}
	}

	public int NetworkcurrentHoleGlobalIndex
	{
		get
		{
			return currentHoleGlobalIndex;
		}
		[param: In]
		set
		{
			GeneratedSyncVarSetter(value, ref currentHoleGlobalIndex, 4uL, _Mirror_SyncVarHookDelegate_currentHoleGlobalIndex);
		}
	}

	public MatchState NetworkmatchState
	{
		get
		{
			return matchState;
		}
		[param: In]
		set
		{
			GeneratedSyncVarSetter(value, ref matchState, 8uL, _Mirror_SyncVarHookDelegate_matchState);
		}
	}

	public bool NetworkisHoleOverviewFinished
	{
		get
		{
			return isHoleOverviewFinished;
		}
		[param: In]
		set
		{
			GeneratedSyncVarSetter(value, ref isHoleOverviewFinished, 16uL, _Mirror_SyncVarHookDelegate_isHoleOverviewFinished);
		}
	}

	public bool NetworkforceDisplayScoreboard
	{
		get
		{
			return forceDisplayScoreboard;
		}
		[param: In]
		set
		{
			GeneratedSyncVarSetter(value, ref forceDisplayScoreboard, 32uL, _Mirror_SyncVarHookDelegate_forceDisplayScoreboard);
		}
	}

	public PlayerInfo NetworkmarkedFirstPlacePlayer
	{
		get
		{
			return GetSyncVarNetworkBehaviour(___markedFirstPlacePlayerNetId, ref markedFirstPlacePlayer);
		}
		[param: In]
		set
		{
			GeneratedSyncVarSetter_NetworkBehaviour(value, ref markedFirstPlacePlayer, 64uL, _Mirror_SyncVarHookDelegate_markedFirstPlacePlayer, ref ___markedFirstPlacePlayerNetId);
		}
	}

	public static event Action<MatchState, MatchState> MatchStateChanged;

	public static event Action CurrentHoleCourseIndexChanged;

	public static event Action CurrentHoleGlobalIndexChanged;

	public static event Action ForceDisplayScoreboardChanged;

	public static event Action<SyncList<PlayerState>.Operation, int, PlayerState> PlayerStatesChanged;

	public static event Action<SyncIDictionary<ulong, float>.Operation, ulong, float> PlayerPingsChanged;

	public static event Action<SyncSet<PlayerPair>.Operation, PlayerPair> PlayerDominationsChanged;

	public static event Action<SyncIDictionary<PlayerPair, KnockoutStreak>.Operation, PlayerPair, KnockoutStreak> PlayerKnockoutStreaksChanged;

	public static event Action<SyncList<GolfBall>.Operation, int, GolfBall> OvertimeActiveBallsChanged;

	[CCommand("skipHole", "", false, false, serverOnly = true)]
	private static void SkipHole()
	{
		if (SingletonNetworkBehaviour<CourseManager>.HasInstance && NetworkServer.active)
		{
			SingletonNetworkBehaviour<CourseManager>.Instance.ServerStartNextMatch(skipPersistentInventories: false);
		}
	}

	protected override void Awake()
	{
		base.Awake();
		if (SingletonBehaviour<DrivingRangeManager>.HasInstance)
		{
			GameplayCameraManager.TransitionTo(CameraModuleType.Orbit, 0f);
			TryPlayHoleMusic(hurryUpInstantly: false);
		}
		else
		{
			LoadingScreen.Hide();
			GameplayCameraManager.TransitionTo(CameraModuleType.Overview, 0f);
			HudManager.Hide(instant: true);
		}
	}

	protected override void OnDestroy()
	{
		base.OnDestroy();
		if (holeMusicInstance.isValid())
		{
			holeMusicInstance.stop(FMOD.Studio.STOP_MODE.ALLOWFADEOUT);
		}
		if (announcerLineInstance.isValid())
		{
			announcerLineInstance.stop(FMOD.Studio.STOP_MODE.ALLOWFADEOUT);
		}
		if (!BNetworkManager.IsChangingSceneOrShuttingDown && NetworkmarkedFirstPlacePlayer != null)
		{
			NetworkmarkedFirstPlacePlayer.Movement.IsVisibleChanged -= OnMarkedFirstPlacePlayerIsVisibleChanged;
		}
	}

	public override void OnStartServer()
	{
		if (!SingletonBehaviour<DrivingRangeManager>.HasInstance)
		{
			LoadingScreen.Hide();
		}
		foreach (PlayerState playerState in ServerPersistentCourseData.playerStates)
		{
			int count = playerStates.Count;
			if (playerStateIndicesPerPlayerGuid.TryAdd(playerState.playerGuid, count))
			{
				playerStates.Add(PlayerState.GetNewMatchState(playerState));
			}
		}
		ServerPersistentCourseData.RestorePlayerJoinTimestamps();
		foreach (var (playerPair2, value) in ServerPersistentCourseData.playerKnockoutStreaks)
		{
			playerKnockoutStreaks.Add(playerPair2, value);
			if (value.persistentStreak >= GameManager.MatchSettings.DominationKnockoutStreak)
			{
				playerDominations.Add(playerPair2);
			}
		}
		foreach (int serverConnectionId in BNetworkManager.ServerConnectionIds)
		{
			if (IsConnectionInitialMatchParticipant(serverConnectionId))
			{
				initialPlayersInMatchParticipantConnectionIds.Add(serverConnectionId);
			}
		}
		GolfTeeManager.UpdateActivePlatforms();
		BUpdate.RegisterCallback(this);
		matchInitializationTimestamp = Time.timeAsDouble;
		NetworkcurrentHoleCourseIndex = (SingletonBehaviour<DrivingRangeManager>.HasInstance ? (-1) : (ServerPersistentCourseData.nextHoleIndex - 1));
		NetworkcurrentHoleGlobalIndex = ((currentHoleCourseIndex >= 0) ? GameManager.CurrentCourse.Holes[currentHoleCourseIndex].GlobalIndex : (-1));
		if (!SingletonBehaviour<DrivingRangeManager>.HasInstance && matchState == MatchState.Initializing)
		{
			HoleOverviewCameraUi.Show();
			HoleOverviewCameraUi.SetState(HoleOverviewCameraUi.State.WaitingForPlayers);
		}
		if (!base.isClient)
		{
			SyncList<PlayerState> syncList = playerStates;
			syncList.OnChange = (Action<SyncList<PlayerState>.Operation, int, PlayerState>)Delegate.Combine(syncList.OnChange, new Action<SyncList<PlayerState>.Operation, int, PlayerState>(OnPlayerStatesChanged));
			SyncDictionary<ulong, float> syncDictionary = playerPingPerGuid;
			syncDictionary.OnChange = (Action<SyncIDictionary<ulong, float>.Operation, ulong, float>)Delegate.Combine(syncDictionary.OnChange, new Action<SyncIDictionary<ulong, float>.Operation, ulong, float>(OnPlayerPingsChanged));
			SyncDictionary<PlayerPair, KnockoutStreak> syncDictionary2 = PlayerKnockoutStreaks;
			syncDictionary2.OnChange = (Action<SyncIDictionary<PlayerPair, KnockoutStreak>.Operation, PlayerPair, KnockoutStreak>)Delegate.Combine(syncDictionary2.OnChange, new Action<SyncIDictionary<PlayerPair, KnockoutStreak>.Operation, PlayerPair, KnockoutStreak>(OnPlayerKnockoutStreaksChanged));
			SyncHashSet<PlayerPair> syncHashSet = playerDominations;
			syncHashSet.OnChange = (Action<SyncSet<PlayerPair>.Operation, PlayerPair>)Delegate.Combine(syncHashSet.OnChange, new Action<SyncSet<PlayerPair>.Operation, PlayerPair>(OnPlayerDominationsChanged));
			SyncList<GolfBall> syncList2 = overtimeActiveBalls;
			syncList2.OnChange = (Action<SyncList<GolfBall>.Operation, int, GolfBall>)Delegate.Combine(syncList2.OnChange, new Action<SyncList<GolfBall>.Operation, int, GolfBall>(OnOvertimeActiveBallsChanged));
		}
		PlayerId.AnyPlayerGuidChanged += OnServerAnyPlayerGuidChanged;
		PlayerMovement.AnyPlayerIsRespawningChanged += OnServerAnyPlayerIsRespawningChanged;
		PlayerGolfer.PlayerHitOwnBall += OnServerPlayerHitOwnBall;
		PlayerGolfer.AnyPlayerMatchResolutionChanged += OnServerAnyPlayerMatchResolutionChanged;
		BNetworkManager.singleton.ServerUpdateCourseProgress();
		WindManager.Initialize();
		bool IsConnectionInitialMatchParticipant(int connectionId)
		{
			if (SingletonBehaviour<DrivingRangeManager>.HasInstance)
			{
				return true;
			}
			if (!BNetworkManager.singleton.playerGuidPerConnectionId.TryGetValue(connectionId, out var value2))
			{
				return false;
			}
			if (!playerStateIndicesPerPlayerGuid.TryGetValue(value2, out var value3))
			{
				return false;
			}
			return !playerStates[value3].isSpectator;
		}
	}

	public override void OnStopServer()
	{
		if (BNetworkManager.IsShuttingDown)
		{
			ServerPersistentCourseData.ClearAll();
		}
		BUpdate.DeregisterCallback(this);
		if (!base.isClient)
		{
			SyncList<PlayerState> syncList = playerStates;
			syncList.OnChange = (Action<SyncList<PlayerState>.Operation, int, PlayerState>)Delegate.Remove(syncList.OnChange, new Action<SyncList<PlayerState>.Operation, int, PlayerState>(OnPlayerStatesChanged));
			SyncDictionary<ulong, float> syncDictionary = playerPingPerGuid;
			syncDictionary.OnChange = (Action<SyncIDictionary<ulong, float>.Operation, ulong, float>)Delegate.Remove(syncDictionary.OnChange, new Action<SyncIDictionary<ulong, float>.Operation, ulong, float>(OnPlayerPingsChanged));
			SyncDictionary<PlayerPair, KnockoutStreak> syncDictionary2 = PlayerKnockoutStreaks;
			syncDictionary2.OnChange = (Action<SyncIDictionary<PlayerPair, KnockoutStreak>.Operation, PlayerPair, KnockoutStreak>)Delegate.Remove(syncDictionary2.OnChange, new Action<SyncIDictionary<PlayerPair, KnockoutStreak>.Operation, PlayerPair, KnockoutStreak>(OnPlayerKnockoutStreaksChanged));
			SyncHashSet<PlayerPair> syncHashSet = playerDominations;
			syncHashSet.OnChange = (Action<SyncSet<PlayerPair>.Operation, PlayerPair>)Delegate.Remove(syncHashSet.OnChange, new Action<SyncSet<PlayerPair>.Operation, PlayerPair>(OnPlayerDominationsChanged));
			SyncList<GolfBall> syncList2 = overtimeActiveBalls;
			syncList2.OnChange = (Action<SyncList<GolfBall>.Operation, int, GolfBall>)Delegate.Remove(syncList2.OnChange, new Action<SyncList<GolfBall>.Operation, int, GolfBall>(OnOvertimeActiveBallsChanged));
		}
		PlayerId.AnyPlayerGuidChanged -= OnServerAnyPlayerGuidChanged;
		PlayerMovement.AnyPlayerIsRespawningChanged -= OnServerAnyPlayerIsRespawningChanged;
		PlayerGolfer.PlayerHitOwnBall -= OnServerPlayerHitOwnBall;
		PlayerGolfer.AnyPlayerMatchResolutionChanged -= OnServerAnyPlayerMatchResolutionChanged;
	}

	public override void OnStartClient()
	{
		if (!SingletonBehaviour<DrivingRangeManager>.HasInstance && matchState >= MatchState.TeeOff)
		{
			GameplayCameraManager.TransitionTo(CameraModuleType.Orbit, 0f);
			HudManager.Show(instant: true);
		}
		SyncList<PlayerState> syncList = playerStates;
		syncList.OnChange = (Action<SyncList<PlayerState>.Operation, int, PlayerState>)Delegate.Combine(syncList.OnChange, new Action<SyncList<PlayerState>.Operation, int, PlayerState>(OnPlayerStatesChanged));
		SyncDictionary<ulong, float> syncDictionary = playerPingPerGuid;
		syncDictionary.OnChange = (Action<SyncIDictionary<ulong, float>.Operation, ulong, float>)Delegate.Combine(syncDictionary.OnChange, new Action<SyncIDictionary<ulong, float>.Operation, ulong, float>(OnPlayerPingsChanged));
		SyncDictionary<PlayerPair, KnockoutStreak> syncDictionary2 = PlayerKnockoutStreaks;
		syncDictionary2.OnChange = (Action<SyncIDictionary<PlayerPair, KnockoutStreak>.Operation, PlayerPair, KnockoutStreak>)Delegate.Combine(syncDictionary2.OnChange, new Action<SyncIDictionary<PlayerPair, KnockoutStreak>.Operation, PlayerPair, KnockoutStreak>(OnPlayerKnockoutStreaksChanged));
		SyncHashSet<PlayerPair> syncHashSet = playerDominations;
		syncHashSet.OnChange = (Action<SyncSet<PlayerPair>.Operation, PlayerPair>)Delegate.Combine(syncHashSet.OnChange, new Action<SyncSet<PlayerPair>.Operation, PlayerPair>(OnPlayerDominationsChanged));
		SyncList<GolfBall> syncList2 = overtimeActiveBalls;
		syncList2.OnChange = (Action<SyncList<GolfBall>.Operation, int, GolfBall>)Delegate.Combine(syncList2.OnChange, new Action<SyncList<GolfBall>.Operation, int, GolfBall>(OnOvertimeActiveBallsChanged));
		if (!SteamEnabler.IsSteamEnabled)
		{
			return;
		}
		foreach (PlayerState playerState in playerStates)
		{
			if (playerState.isConnected)
			{
				if (!BNetworkManager.TryGetPlayerInLobby(playerState.playerGuid, out var player))
				{
					UnityEngine.Debug.LogWarning($"Player \"{GetPlayerName(playerState)}\" ({playerState.playerGuid}) is not present in lobby, host is suspicious!");
				}
				else
				{
					UnityEngine.Debug.Log($"Player in lobby \"{player.Name}\" ({player.Id})");
				}
			}
		}
	}

	public override void OnStopClient()
	{
		SyncList<PlayerState> syncList = playerStates;
		syncList.OnChange = (Action<SyncList<PlayerState>.Operation, int, PlayerState>)Delegate.Remove(syncList.OnChange, new Action<SyncList<PlayerState>.Operation, int, PlayerState>(OnPlayerStatesChanged));
		SyncDictionary<ulong, float> syncDictionary = playerPingPerGuid;
		syncDictionary.OnChange = (Action<SyncIDictionary<ulong, float>.Operation, ulong, float>)Delegate.Remove(syncDictionary.OnChange, new Action<SyncIDictionary<ulong, float>.Operation, ulong, float>(OnPlayerPingsChanged));
		SyncDictionary<PlayerPair, KnockoutStreak> syncDictionary2 = PlayerKnockoutStreaks;
		syncDictionary2.OnChange = (Action<SyncIDictionary<PlayerPair, KnockoutStreak>.Operation, PlayerPair, KnockoutStreak>)Delegate.Remove(syncDictionary2.OnChange, new Action<SyncIDictionary<PlayerPair, KnockoutStreak>.Operation, PlayerPair, KnockoutStreak>(OnPlayerKnockoutStreaksChanged));
		SyncHashSet<PlayerPair> syncHashSet = playerDominations;
		syncHashSet.OnChange = (Action<SyncSet<PlayerPair>.Operation, PlayerPair>)Delegate.Remove(syncHashSet.OnChange, new Action<SyncSet<PlayerPair>.Operation, PlayerPair>(OnPlayerDominationsChanged));
		SyncList<GolfBall> syncList2 = overtimeActiveBalls;
		syncList2.OnChange = (Action<SyncList<GolfBall>.Operation, int, GolfBall>)Delegate.Remove(syncList2.OnChange, new Action<SyncList<GolfBall>.Operation, int, GolfBall>(OnOvertimeActiveBallsChanged));
	}

	public static void StartCourse()
	{
		if (SingletonNetworkBehaviour<CourseManager>.HasInstance)
		{
			SingletonNetworkBehaviour<CourseManager>.Instance.StartCourseInternal();
		}
	}

	public static void EndCourse()
	{
		if (SingletonNetworkBehaviour<CourseManager>.HasInstance)
		{
			SingletonNetworkBehaviour<CourseManager>.Instance.EndCourseInternal();
		}
	}

	public static void ServerRegisterPlayer(NetworkConnectionToClient connection)
	{
		if (SingletonNetworkBehaviour<CourseManager>.HasInstance)
		{
			SingletonNetworkBehaviour<CourseManager>.Instance.ServerRegisterPlayerInternal(connection);
		}
	}

	public static void DeregisterPlayer(NetworkConnectionToClient connection)
	{
		if (SingletonNetworkBehaviour<CourseManager>.HasInstance)
		{
			SingletonNetworkBehaviour<CourseManager>.Instance.DeregisterPlayerInternal(connection);
		}
	}

	public static void RegisterMatchParticipant(PlayerGolfer participant)
	{
		if (SingletonNetworkBehaviour<CourseManager>.HasInstance)
		{
			SingletonNetworkBehaviour<CourseManager>.Instance.RegisterMatchParticipantInternal(participant);
		}
	}

	public static void DeregisterMatchParticipant(PlayerGolfer participant)
	{
		if (SingletonNetworkBehaviour<CourseManager>.HasInstance)
		{
			SingletonNetworkBehaviour<CourseManager>.Instance.DeregisterMatchParticipantInternal(participant);
		}
	}

	public static void ReportPlayerAwaitingSpawning(int connectionId)
	{
		if (SingletonNetworkBehaviour<CourseManager>.HasInstance)
		{
			SingletonNetworkBehaviour<CourseManager>.Instance.ReportPlayerAwaitingSpawningInternal(connectionId);
		}
	}

	public static bool DoesPlayerHaveReservedTeeingSpot(int connectionId, out TeeingSpot teeingSpot)
	{
		if (!SingletonNetworkBehaviour<CourseManager>.HasInstance)
		{
			teeingSpot = null;
			return false;
		}
		return SingletonNetworkBehaviour<CourseManager>.Instance.DoesPlayerHaveReservedTeeingSpotInternal(connectionId, out teeingSpot);
	}

	public static void ReportPlayerNoLongerAwaitingSpawning(int connectionId)
	{
		if (SingletonNetworkBehaviour<CourseManager>.HasInstance)
		{
			SingletonNetworkBehaviour<CourseManager>.Instance.ReportPlayerNoLongerAwaitingSpawningInternal(connectionId);
		}
	}

	public static void RegisterActivePlayerOnGreen(PlayerGolfer player)
	{
		if (SingletonNetworkBehaviour<CourseManager>.HasInstance)
		{
			SingletonNetworkBehaviour<CourseManager>.Instance.RegisterActivePlayerOnGreenInternal(player);
		}
	}

	public static void DeregisterActivePlayerOnGreen(PlayerGolfer player)
	{
		if (SingletonNetworkBehaviour<CourseManager>.HasInstance)
		{
			SingletonNetworkBehaviour<CourseManager>.Instance.DeregisterActivePlayerOnGreenInternal(player);
		}
	}

	public static void RegisterActiveBall(GolfBall ball)
	{
		if (SingletonNetworkBehaviour<CourseManager>.HasInstance)
		{
			SingletonNetworkBehaviour<CourseManager>.Instance.RegisterActiveBallInternal(ball);
		}
	}

	public static void DeregisterActiveBall(GolfBall ball)
	{
		if (SingletonNetworkBehaviour<CourseManager>.HasInstance)
		{
			SingletonNetworkBehaviour<CourseManager>.Instance.DeregisterActiveBallInternal(ball);
		}
	}

	public static void PlayAnnouncerLineLocalOnly(AnnouncerLine line)
	{
		if (SingletonNetworkBehaviour<CourseManager>.HasInstance)
		{
			SingletonNetworkBehaviour<CourseManager>.Instance.PlayAnnouncerLineLocalOnlyInternal(line);
		}
	}

	public static void InformPlayerScored(PlayerGolfer player, GolfHole hole)
	{
		if (SingletonNetworkBehaviour<CourseManager>.HasInstance)
		{
			SingletonNetworkBehaviour<CourseManager>.Instance.InformPlayerScoredInternal(player, hole);
		}
	}

	public static void InformPlayerKnockedOut(PlayerMovement knockedOutPlayer, PlayerInfo responsiblePlayer, KnockoutType knockoutType, out bool knockoutCounted)
	{
		if (!SingletonNetworkBehaviour<CourseManager>.HasInstance)
		{
			knockoutCounted = false;
		}
		else
		{
			SingletonNetworkBehaviour<CourseManager>.Instance.InformPlayerKnockedOutInternal(knockedOutPlayer, responsiblePlayer, knockoutType, out knockoutCounted);
		}
	}

	public static void MarkLatestValidKnockout(PlayerInfo knockedOutPlayer, ItemUseId itemUseId)
	{
		if (SingletonNetworkBehaviour<CourseManager>.HasInstance)
		{
			SingletonNetworkBehaviour<CourseManager>.Instance.MarkLatestValidKnockoutInternal(knockedOutPlayer, itemUseId);
		}
	}

	public static void InformPlayerEliminated(PlayerGolfer eliminatedPlayer, PlayerGolfer responsiblePlayer, EliminationReason reason, EliminationReason immediateEliminationReason)
	{
		if (SingletonNetworkBehaviour<CourseManager>.HasInstance)
		{
			SingletonNetworkBehaviour<CourseManager>.Instance.InformPlayerEliminatedInternal(eliminatedPlayer, responsiblePlayer, reason, immediateEliminationReason);
		}
	}

	public static void InformBallDispensed(PlayerGolfer owner)
	{
		if (SingletonNetworkBehaviour<CourseManager>.HasInstance)
		{
			SingletonNetworkBehaviour<CourseManager>.Instance.InformBallDispensedInternal(owner);
		}
	}

	public static void InformPlayerPickedUpItem(PlayerInfo owner)
	{
		if (SingletonNetworkBehaviour<CourseManager>.HasInstance)
		{
			SingletonNetworkBehaviour<CourseManager>.Instance.InformPlayerPickedUpItemInternal(owner);
		}
	}

	public static void AddPenaltyStroke(PlayerGolfer penalizedPlayer, bool suppressPopup)
	{
		if (SingletonNetworkBehaviour<CourseManager>.HasInstance)
		{
			SingletonNetworkBehaviour<CourseManager>.Instance.AddPenaltyStrokeInternal(penalizedPlayer, suppressPopup);
		}
	}

	public static void SetPlayerSpectator(PlayerGolfer player, bool isSpectator)
	{
		if (SingletonNetworkBehaviour<CourseManager>.HasInstance)
		{
			SingletonNetworkBehaviour<CourseManager>.Instance.SetPlayerSpectatorInternal(player, isSpectator);
		}
	}

	public static bool IsPlayerSpectator(PlayerGolfer player)
	{
		if (SingletonNetworkBehaviour<CourseManager>.HasInstance)
		{
			return SingletonNetworkBehaviour<CourseManager>.Instance.IsPlayerSpectatorInternal(player);
		}
		return false;
	}

	public static void ServerSetCourse(CourseData course)
	{
		if (SingletonNetworkBehaviour<CourseManager>.HasInstance)
		{
			SingletonNetworkBehaviour<CourseManager>.Instance.ServerSetCourseInternal(course);
		}
	}

	public static int GetCurrentHolePar()
	{
		if (!SingletonNetworkBehaviour<CourseManager>.HasInstance)
		{
			return -1;
		}
		return SingletonNetworkBehaviour<CourseManager>.Instance.GetCurrentHoleParInternal();
	}

	public static LocalizedString GetCurrentCourseLocalizedName()
	{
		if (!SingletonNetworkBehaviour<CourseManager>.HasInstance)
		{
			return null;
		}
		return SingletonNetworkBehaviour<CourseManager>.Instance.GetCurrentCourseLocalizedNameInternal();
	}

	public static LocalizedString GetCurrentHoleLocalizedName()
	{
		if (!SingletonNetworkBehaviour<CourseManager>.HasInstance)
		{
			return null;
		}
		return SingletonNetworkBehaviour<CourseManager>.Instance.GetCurrentHoleLocalizedNameInternal();
	}

	public static List<PlayerState> GetSortedPlayerStates()
	{
		if (!SingletonNetworkBehaviour<CourseManager>.HasInstance)
		{
			return null;
		}
		return SingletonNetworkBehaviour<CourseManager>.Instance.GetSortedPlayerStatesInternal();
	}

	public static bool TryGetPlayerState(ulong playerGuid, out PlayerState state)
	{
		if (!SingletonNetworkBehaviour<CourseManager>.HasInstance)
		{
			state = default(PlayerState);
			return false;
		}
		return SingletonNetworkBehaviour<CourseManager>.Instance.TryGetPlayerStateInternal(playerGuid, out state);
	}

	public static bool TryGetPlayerState(PlayerInfo player, out PlayerState state)
	{
		if (!SingletonNetworkBehaviour<CourseManager>.HasInstance)
		{
			state = default(PlayerState);
			return false;
		}
		return SingletonNetworkBehaviour<CourseManager>.Instance.TryGetPlayerStateInternal(player, out state);
	}

	public static PlayerState GetLocalPlayerState()
	{
		if (!SingletonNetworkBehaviour<CourseManager>.HasInstance)
		{
			return default(PlayerState);
		}
		if (GameManager.LocalPlayerId == null)
		{
			return default(PlayerState);
		}
		ulong guid = GameManager.LocalPlayerId.Guid;
		foreach (PlayerState playerState in PlayerStates)
		{
			if (playerState.playerGuid == guid)
			{
				return playerState;
			}
		}
		UnityEngine.Debug.LogError("Couldn't find local player state!!");
		return default(PlayerState);
	}

	public static PlayerState GetFirstPlaceState()
	{
		if (!SingletonNetworkBehaviour<CourseManager>.HasInstance)
		{
			return default(PlayerState);
		}
		return SingletonNetworkBehaviour<CourseManager>.Instance.GetFirstPlaceStateInternal();
	}

	public static PlayerState GetLastPlaceState()
	{
		if (!SingletonNetworkBehaviour<CourseManager>.HasInstance)
		{
			return default(PlayerState);
		}
		return SingletonNetworkBehaviour<CourseManager>.Instance.GetLastPlaceStateInternal();
	}

	public static int CountActivePlayers()
	{
		if (!SingletonNetworkBehaviour<CourseManager>.HasInstance)
		{
			return 0;
		}
		return SingletonNetworkBehaviour<CourseManager>.Instance.CountActivePlayersInternal();
	}

	public static void GetConnectedPlayerStates(List<PlayerState> connectedPlayerStates)
	{
		if (SingletonNetworkBehaviour<CourseManager>.HasInstance)
		{
			SingletonNetworkBehaviour<CourseManager>.Instance.GetConnectedPlayerStatesInternal(connectedPlayerStates);
		}
	}

	public static void ServerSpawnItem(ItemType item, int remainingUses, Vector3 position, Quaternion rotation, Vector3 velocity, Vector3 angularVelocity, ItemUseId itemUseId, PlayerInventory spawner)
	{
		if (SingletonNetworkBehaviour<CourseManager>.HasInstance)
		{
			SingletonNetworkBehaviour<CourseManager>.Instance.ServerSpawnItemInternal(item, remainingUses, position, rotation, velocity, angularVelocity, itemUseId, spawner);
		}
	}

	public static void ServerSpawnLandmine(Vector3 position, Quaternion rotation, Vector3 velocity, Vector3 angularVelocity, LandmineArmType armType, ItemUseId itemUseId, PlayerInventory owner)
	{
		if (SingletonNetworkBehaviour<CourseManager>.HasInstance)
		{
			SingletonNetworkBehaviour<CourseManager>.Instance.ServerSpawnLandmineInternal(position, rotation, velocity, angularVelocity, armType, itemUseId, owner);
		}
	}

	public static void CmdSpawnGolfCartForLocalPlayer()
	{
		if (SingletonNetworkBehaviour<CourseManager>.HasInstance)
		{
			SingletonNetworkBehaviour<CourseManager>.Instance.CmdSpawnGolfCartForLocalPlayerInternal();
		}
	}

	public void OnBUpdate()
	{
		if (matchState == MatchState.Initializing)
		{
			TryFinishMatchInitialization();
		}
		UpdateAllClientPings();
		void ReserveTeeingSpotFor(int connectionId)
		{
			TeeingSpot availableTeeingSpot = GolfTeeManager.GetAvailableTeeingSpot();
			reservedTeeingSpotsPerConnectionId.Add(connectionId, availableTeeingSpot);
			availableTeeingSpot.ReserveFor(connectionId);
		}
		void ReserveTeeingSpotsForAllAwaitingPlayers()
		{
			reservedTeeingSpotsForInitialPlayers = true;
			initialPlayersInMatchAwaitingSpawningConnectionIds.Shuffle();
			foreach (int initialPlayersInMatchAwaitingSpawningConnectionId in initialPlayersInMatchAwaitingSpawningConnectionIds)
			{
				ReserveTeeingSpotFor(initialPlayersInMatchAwaitingSpawningConnectionId);
			}
			foreach (int additionalPlayersInMatchAwaitingSpawningConnectionId in additionalPlayersInMatchAwaitingSpawningConnectionIds)
			{
				ReserveTeeingSpotFor(additionalPlayersInMatchAwaitingSpawningConnectionId);
			}
		}
		void TryFinishMatchInitialization()
		{
			if (!SingletonBehaviour<DrivingRangeManager>.HasInstance)
			{
				if (!reservedTeeingSpotsForInitialPlayers)
				{
					ValidateInitialPlayersInMatch();
					if ((!simulateClientsJoiningHoleSlowly && initialPlayersInMatchAwaitingSpawningConnectionIds.Count >= initialPlayersInMatchParticipantConnectionIds.Count) || BMath.GetTimeSince(matchInitializationTimestamp) > 10f)
					{
						ReserveTeeingSpotsForAllAwaitingPlayers();
					}
				}
				else if ((!simulateClientsJoiningHoleSlowly && initialPlayersInMatchAwaitingSpawningConnectionIds.Count <= 0) || BMath.GetTimeSince(matchInitializationTimestamp) > 10f)
				{
					BeginHoleOverviewAndTeeOffCountdown();
				}
			}
		}
		void UpdateAllClientPings()
		{
			if (timeSincePingUpdate < 2f)
			{
				timeSincePingUpdate += Time.deltaTime;
			}
			else
			{
				timeSincePingUpdate = 0f;
				for (int i = 0; i < playerStates.Count; i++)
				{
					PlayerState playerState = playerStates[i];
					if (BNetworkManager.singleton.ServerTryGetConnectionFromPlayerGuid(playerState.playerGuid, out var connection))
					{
						playerPingPerGuid[playerState.playerGuid] = (float)(connection.rtt * 1000.0);
					}
				}
			}
		}
		void ValidateInitialPlayersInMatch()
		{
			HashSet<int> hashSet = null;
			foreach (int initialPlayersInMatchParticipantConnectionId in initialPlayersInMatchParticipantConnectionIds)
			{
				if (!NetworkServer.connections.ContainsKey(initialPlayersInMatchParticipantConnectionId))
				{
					if (hashSet == null)
					{
						hashSet = new HashSet<int>();
					}
					hashSet.Add(initialPlayersInMatchParticipantConnectionId);
				}
			}
			if (hashSet != null)
			{
				foreach (int item in hashSet)
				{
					initialPlayersInMatchAwaitingSpawningConnectionIds.Remove(item);
					initialPlayersInMatchParticipantConnectionIds.Remove(item);
				}
			}
		}
	}

	[AsyncStateMachine(typeof(_003CStartCourseInternal_003Ed__147))]
	[Server]
	private void StartCourseInternal()
	{
		if (!NetworkServer.active)
		{
			UnityEngine.Debug.LogWarning("[Server] function 'System.Void CourseManager::StartCourseInternal()' called when server was not active");
			return;
		}
		_003CStartCourseInternal_003Ed__147 stateMachine = default(_003CStartCourseInternal_003Ed__147);
		stateMachine._003C_003Et__builder = AsyncVoidMethodBuilder.Create();
		stateMachine._003C_003E4__this = this;
		stateMachine._003C_003E1__state = -1;
		stateMachine._003C_003Et__builder.Start(ref stateMachine);
	}

	[AsyncStateMachine(typeof(_003CRpcInformStartingCourse_003Ed__148))]
	[ClientRpc]
	private void RpcInformStartingCourse()
	{
		NetworkWriterPooled writer = NetworkWriterPool.Get();
		SendRPCInternal("System.Void CourseManager::RpcInformStartingCourse()", 183172188, writer, 0, includeOwner: true);
		NetworkWriterPool.Return(writer);
	}

	[AsyncStateMachine(typeof(_003CEndCourseInternal_003Ed__149))]
	[Server]
	private void EndCourseInternal()
	{
		if (!NetworkServer.active)
		{
			UnityEngine.Debug.LogWarning("[Server] function 'System.Void CourseManager::EndCourseInternal()' called when server was not active");
			return;
		}
		_003CEndCourseInternal_003Ed__149 stateMachine = default(_003CEndCourseInternal_003Ed__149);
		stateMachine._003C_003Et__builder = AsyncVoidMethodBuilder.Create();
		stateMachine._003C_003E4__this = this;
		stateMachine._003C_003E1__state = -1;
		stateMachine._003C_003Et__builder.Start(ref stateMachine);
	}

	[AsyncStateMachine(typeof(_003CRpcInformEndingCourse_003Ed__150))]
	[ClientRpc]
	private void RpcInformEndingCourse()
	{
		NetworkWriterPooled writer = NetworkWriterPool.Get();
		SendRPCInternal("System.Void CourseManager::RpcInformEndingCourse()", -135860741, writer, 0, includeOwner: true);
		NetworkWriterPool.Return(writer);
	}

	[AsyncStateMachine(typeof(_003CServerStartNextMatch_003Ed__151))]
	[Server]
	private void ServerStartNextMatch(bool skipPersistentInventories)
	{
		if (!NetworkServer.active)
		{
			UnityEngine.Debug.LogWarning("[Server] function 'System.Void CourseManager::ServerStartNextMatch(System.Boolean)' called when server was not active");
			return;
		}
		_003CServerStartNextMatch_003Ed__151 stateMachine = default(_003CServerStartNextMatch_003Ed__151);
		stateMachine._003C_003Et__builder = AsyncVoidMethodBuilder.Create();
		stateMachine._003C_003E4__this = this;
		stateMachine.skipPersistentInventories = skipPersistentInventories;
		stateMachine._003C_003E1__state = -1;
		stateMachine._003C_003Et__builder.Start(ref stateMachine);
	}

	[AsyncStateMachine(typeof(_003CRpcInformStartingNextMatch_003Ed__152))]
	[ClientRpc]
	private void RpcInformStartingNextMatch()
	{
		NetworkWriterPooled writer = NetworkWriterPool.Get();
		SendRPCInternal("System.Void CourseManager::RpcInformStartingNextMatch()", -1418559341, writer, 0, includeOwner: true);
		NetworkWriterPool.Return(writer);
	}

	[Server]
	private void ServerRegisterPlayerInternal(NetworkConnectionToClient connection)
	{
		if (!NetworkServer.active)
		{
			UnityEngine.Debug.LogWarning("[Server] function 'System.Void CourseManager::ServerRegisterPlayerInternal(Mirror.NetworkConnectionToClient)' called when server was not active");
			return;
		}
		if (connection != NetworkServer.localConnection)
		{
			ServerSetCourseInternal(GameManager.CurrentCourse, connection);
		}
		int value;
		if (!BNetworkManager.singleton.ServerTryGetPlayerGuidFromConnection(connection, out var playerGuid))
		{
			UnityEngine.Debug.LogError($"Failed to get player GUID for connecion {connection.connectionId} while registering them", base.gameObject);
		}
		else if (playerStateIndicesPerPlayerGuid.TryGetValue(playerGuid, out value))
		{
			PlayerState value2 = playerStates[value];
			value2.isConnected = true;
			value2.isSpectator = false;
			value2.joinTimestamp = NetworkTime.time;
			playerStates[value] = value2;
			ServerPersistentCourseData.RegisterPlayerJoinTimestamp(playerGuid, NetworkTime.time);
		}
		else
		{
			int count = playerStates.Count;
			PlayerState item = new PlayerState(playerGuid, count, isConnected: true, connection == NetworkServer.localConnection, isSpectator: false);
			playerStateIndicesPerPlayerGuid.Add(playerGuid, count);
			playerStates.Add(item);
			playerPingPerGuid[playerGuid] = (float)(connection.rtt * 1000.0);
			ServerPersistentCourseData.RegisterPlayerJoinTimestamp(playerGuid, NetworkTime.time);
		}
	}

	[Server]
	private void DeregisterPlayerInternal(NetworkConnectionToClient connection)
	{
		if (!NetworkServer.active)
		{
			UnityEngine.Debug.LogWarning("[Server] function 'System.Void CourseManager::DeregisterPlayerInternal(Mirror.NetworkConnectionToClient)' called when server was not active");
		}
		else
		{
			if (BNetworkManager.IsShuttingDown)
			{
				return;
			}
			if (!BNetworkManager.singleton.ServerTryGetPlayerGuidFromConnection(connection, out var playerGuid))
			{
				UnityEngine.Debug.LogError($"Failed to get player GUID for connecion {connection.connectionId} while deregistering them", base.gameObject);
			}
			else
			{
				if (!playerStateIndicesPerPlayerGuid.TryGetValue(playerGuid, out var value))
				{
					return;
				}
				PlayerState value2 = playerStates[value];
				value2.isConnected = false;
				playerStates[value] = value2;
				clientPlayerNames.Remove(value2.playerGuid);
				ServerPersistentCourseData.DeregisterPlayerJoinTimestamp(value2.playerGuid);
				List<PlayerPair> value3;
				using (CollectionPool<List<PlayerPair>, PlayerPair>.Get(out value3))
				{
					value3.AddRange(playerDominations);
					foreach (PlayerPair item in value3)
					{
						if (item.playerAGuid == playerGuid || item.playerBGuid == playerGuid)
						{
							playerDominations.Remove(item);
						}
					}
					value3.Clear();
					value3.AddRange(playerKnockoutStreaks.Keys);
					foreach (PlayerPair item2 in value3)
					{
						if (item2.playerAGuid == playerGuid || item2.playerBGuid == playerGuid)
						{
							playerKnockoutStreaks.Remove(item2);
						}
					}
				}
			}
		}
	}

	[Server]
	private void RegisterMatchParticipantInternal(PlayerGolfer participant)
	{
		if (!NetworkServer.active)
		{
			UnityEngine.Debug.LogWarning("[Server] function 'System.Void CourseManager::RegisterMatchParticipantInternal(PlayerGolfer)' called when server was not active");
		}
		else if (!SingletonBehaviour<DrivingRangeManager>.HasInstance && !participant.IsMatchResolved && serverMatchParticipants.Add(participant))
		{
			OnServerMatchParticipantsChanged();
		}
	}

	[Server]
	private void DeregisterMatchParticipantInternal(PlayerGolfer participant)
	{
		if (!NetworkServer.active)
		{
			UnityEngine.Debug.LogWarning("[Server] function 'System.Void CourseManager::DeregisterMatchParticipantInternal(PlayerGolfer)' called when server was not active");
		}
		else if (serverMatchParticipants.Remove(participant))
		{
			OnServerMatchParticipantsChanged();
		}
	}

	[Server]
	private void ReportPlayerAwaitingSpawningInternal(int connectionId)
	{
		if (!NetworkServer.active)
		{
			UnityEngine.Debug.LogWarning("[Server] function 'System.Void CourseManager::ReportPlayerAwaitingSpawningInternal(System.Int32)' called when server was not active");
		}
		else if (initialPlayersInMatchParticipantConnectionIds.Contains(connectionId))
		{
			if (!initialPlayersInMatchAwaitingSpawningConnectionIds.Contains(connectionId))
			{
				initialPlayersInMatchAwaitingSpawningConnectionIds.Add(connectionId);
			}
		}
		else if (!additionalPlayersInMatchAwaitingSpawningConnectionIds.Contains(connectionId))
		{
			additionalPlayersInMatchAwaitingSpawningConnectionIds.Add(connectionId);
		}
	}

	[Server]
	private bool DoesPlayerHaveReservedTeeingSpotInternal(int connectionId, out TeeingSpot teeingSpot)
	{
		if (!NetworkServer.active)
		{
			UnityEngine.Debug.LogWarning("[Server] function 'System.Boolean CourseManager::DoesPlayerHaveReservedTeeingSpotInternal(System.Int32,TeeingSpot&)' called when server was not active");
			teeingSpot = null;
			return default(bool);
		}
		return reservedTeeingSpotsPerConnectionId.TryGetValue(connectionId, out teeingSpot);
	}

	[Server]
	private void ReportPlayerNoLongerAwaitingSpawningInternal(int connectionId)
	{
		if (!NetworkServer.active)
		{
			UnityEngine.Debug.LogWarning("[Server] function 'System.Void CourseManager::ReportPlayerNoLongerAwaitingSpawningInternal(System.Int32)' called when server was not active");
		}
		else if (!initialPlayersInMatchAwaitingSpawningConnectionIds.Remove(connectionId))
		{
			additionalPlayersInMatchAwaitingSpawningConnectionIds.Remove(connectionId);
		}
	}

	[Server]
	private void RegisterActivePlayerOnGreenInternal(PlayerGolfer player)
	{
		if (!NetworkServer.active)
		{
			UnityEngine.Debug.LogWarning("[Server] function 'System.Void CourseManager::RegisterActivePlayerOnGreenInternal(PlayerGolfer)' called when server was not active");
		}
		else if (activePlayersOnGreen.Add(player))
		{
			OnActivePlayersOnGreenChanged();
		}
	}

	[Server]
	private void DeregisterActivePlayerOnGreenInternal(PlayerGolfer player)
	{
		if (!NetworkServer.active)
		{
			UnityEngine.Debug.LogWarning("[Server] function 'System.Void CourseManager::DeregisterActivePlayerOnGreenInternal(PlayerGolfer)' called when server was not active");
		}
		else if (activePlayersOnGreen.Remove(player))
		{
			OnActivePlayersOnGreenChanged();
		}
	}

	[Server]
	private void RegisterActiveBallInternal(GolfBall ball)
	{
		if (!NetworkServer.active)
		{
			UnityEngine.Debug.LogWarning("[Server] function 'System.Void CourseManager::RegisterActiveBallInternal(GolfBall)' called when server was not active");
		}
		else
		{
			activeBalls.Add(ball);
		}
	}

	[Server]
	private void DeregisterActiveBallInternal(GolfBall ball)
	{
		if (!NetworkServer.active)
		{
			UnityEngine.Debug.LogWarning("[Server] function 'System.Void CourseManager::DeregisterActiveBallInternal(GolfBall)' called when server was not active");
			return;
		}
		activeBalls.Remove(ball);
		overtimeActiveBalls.Remove(ball);
		overtimeTimeSinceMovedPerActiveBall.Remove(ball);
	}

	private void PlayAnnouncerLineLocalOnlyInternal(AnnouncerLine line)
	{
		announcerLineQueue.Enqueue(line);
		if (!isPlayingQueuedAnnouncerLines)
		{
			PlayLineQueue();
		}
		static bool CanInterrupt(AnnouncerLine previousLine, AnnouncerLine newLine)
		{
			if (newLine == AnnouncerLine.None)
			{
				return false;
			}
			if (previousLine == AnnouncerLine.None)
			{
				return true;
			}
			if (previousLine.IsMatchStateLine() && newLine.IsMatchStateLine())
			{
				return true;
			}
			return false;
		}
		bool IsPlayingAnnouncerLineInternal()
		{
			if (!announcerLineInstance.isValid())
			{
				return false;
			}
			if (announcerLineInstance.getPlaybackState(out var state) != RESULT.OK)
			{
				return false;
			}
			if (state != PLAYBACK_STATE.STARTING && state != PLAYBACK_STATE.PLAYING)
			{
				return state == PLAYBACK_STATE.STOPPING;
			}
			return true;
		}
		void PlayLineImmediately(AnnouncerLine announcerLine)
		{
			EventReference eventReference = announcerLine switch
			{
				AnnouncerLine.HoleInOne => GameManager.AudioSettings.AnnouncerHoleInOneEvent, 
				AnnouncerLine.Last10Seconds => GameManager.AudioSettings.Announcer10SecondsRemainingEvent, 
				AnnouncerLine.Overtime => GameManager.AudioSettings.AnnouncerOvertimeEvent, 
				AnnouncerLine.Finished => GameManager.AudioSettings.AnnouncerFinishedEvent, 
				AnnouncerLine.NiceShot => GameManager.AudioSettings.AnnouncerNiceShotEvent, 
				AnnouncerLine.FirstPlace => GameManager.AudioSettings.AnnouncerFirstPlaceEvent, 
				AnnouncerLine.ChipIn => GameManager.AudioSettings.AnnouncerChipInEvent, 
				AnnouncerLine.Par => GameManager.AudioSettings.AnnouncerParEvent, 
				AnnouncerLine.Birdie => GameManager.AudioSettings.AnnouncerBirdieEvent, 
				AnnouncerLine.Eagle => GameManager.AudioSettings.AnnouncerEagleEvent, 
				AnnouncerLine.Albatross => GameManager.AudioSettings.AnnouncerAlbatrossEvent, 
				AnnouncerLine.Condor => GameManager.AudioSettings.AnnouncerCondorEvent, 
				AnnouncerLine.Speedrun => GameManager.AudioSettings.AnnouncerSpeedrunEvent, 
				_ => default(EventReference), 
			};
			if (eventReference.IsNull)
			{
				UnityEngine.Debug.LogError($"Attempted to play announcer line {announcerLine}, but it has no matching event");
			}
			else
			{
				announcerLineInstance = RuntimeManager.CreateInstance(eventReference);
				announcerLineInstance.start();
				announcerLineInstance.release();
			}
		}
		async void PlayLineQueue()
		{
			isPlayingQueuedAnnouncerLines = true;
			lastAnnouncerLinePlayed = AnnouncerLine.None;
			try
			{
				AnnouncerLine lineToPlay;
				while (announcerLineQueue.TryDequeue(out lineToPlay))
				{
					if (CanInterrupt(lastAnnouncerLinePlayed, lineToPlay) && announcerLineInstance.isValid())
					{
						announcerLineInstance.stop(FMOD.Studio.STOP_MODE.ALLOWFADEOUT);
					}
					await UniTask.WaitWhile(() => IsPlayingAnnouncerLineInternal());
					if (this == null)
					{
						break;
					}
					PlayLineImmediately(lineToPlay);
					lastAnnouncerLinePlayed = lineToPlay;
				}
			}
			catch (Exception exception)
			{
				UnityEngine.Debug.LogError("Encountered exception while playing queued announcer lines. See next log for details", base.gameObject);
				UnityEngine.Debug.LogException(exception, base.gameObject);
				announcerLineQueue.Clear();
			}
			finally
			{
				isPlayingQueuedAnnouncerLines = false;
			}
		}
	}

	[ClientRpc]
	private void RpcPlayAnnouncerLine(AnnouncerLine line)
	{
		NetworkWriterPooled writer = NetworkWriterPool.Get();
		GeneratedNetworkCode._Write_AnnouncerLine(writer, line);
		SendRPCInternal("System.Void CourseManager::RpcPlayAnnouncerLine(AnnouncerLine)", -1562665418, writer, 0, includeOwner: true);
		NetworkWriterPool.Return(writer);
	}

	[TargetRpc]
	private void RpcPlayAnnouncerLines(NetworkConnectionToClient connection, List<AnnouncerLine> lines)
	{
		NetworkWriterPooled writer = NetworkWriterPool.Get();
		GeneratedNetworkCode._Write_System_002ECollections_002EGeneric_002EList_00601_003CAnnouncerLine_003E(writer, lines);
		SendTargetRPCInternal(connection, "System.Void CourseManager::RpcPlayAnnouncerLines(Mirror.NetworkConnectionToClient,System.Collections.Generic.List`1<AnnouncerLine>)", 153459586, writer, 0);
		NetworkWriterPool.Return(writer);
	}

	[Server]
	private void InformPlayerScoredInternal(PlayerGolfer playerAsGolfer, GolfHole hole)
	{
		bool isInDrivingRange;
		int placement;
		StrokesUnderParType strokesUnderParType;
		bool isChipIn;
		bool isSpeedrun;
		if (!NetworkServer.active)
		{
			UnityEngine.Debug.LogWarning("[Server] function 'System.Void CourseManager::InformPlayerScoredInternal(PlayerGolfer,GolfHole)' called when server was not active");
		}
		else
		{
			if (!TryGetPlayerStateIndex(playerAsGolfer.connectionToClient, out var index))
			{
				return;
			}
			if (matchScoredPlayers.Contains(playerAsGolfer))
			{
				UnityEngine.Debug.LogError($"{playerAsGolfer} scored more than once in this match", playerAsGolfer);
				return;
			}
			isInDrivingRange = SingletonBehaviour<DrivingRangeManager>.HasInstance;
			placement = matchScoredPlayers.Count;
			if (!isInDrivingRange)
			{
				matchScoredPlayers.Add(playerAsGolfer);
				NetworkdidAnyPlayerScore = true;
			}
			int matchScore = GetMatchScore(placement);
			if (isInDrivingRange)
			{
				playerAsGolfer.PlayerInfo.RpcPopUpDrivingRangeScore(matchScore);
				InfoFeed.ShowScoredOnDrivingRange(playerAsGolfer.PlayerInfo);
			}
			else
			{
				playerAsGolfer.PlayerInfo.RpcPopUpPlacementScore(placement, matchScore);
				InfoFeed.ShowFinishedHoleMessage(playerAsGolfer.PlayerInfo, placement);
			}
			PlayerState value = playerStates[index];
			value.courseScore += matchScore;
			value.matchScore += matchScore;
			value.finishes++;
			if (CountActivePlayersInternal() > 1)
			{
				value.multiplayerFinishes++;
			}
			if (isInDrivingRange || placement == 0)
			{
				value.wins++;
			}
			value.scoreTimestamp = Time.timeAsDouble;
			int currentHoleParInternal = GetCurrentHoleParInternal();
			PlayerTextPopupType popupType;
			int strokesUnderPar;
			int num;
			if (value.matchStrokes <= 1)
			{
				strokesUnderParType = StrokesUnderParType.HoleInOne;
				popupType = PlayerTextPopupType.HoleInOne;
				strokesUnderPar = 0;
				num = GameManager.MatchSettings.HoleInOneScore;
			}
			else
			{
				strokesUnderPar = currentHoleParInternal - value.matchStrokes;
				strokesUnderParType = GetStrokesUnderParType(strokesUnderPar);
				(popupType, num) = strokesUnderParType switch
				{
					StrokesUnderParType.Par => (PlayerTextPopupType.Par, GameManager.MatchSettings.ParScore), 
					StrokesUnderParType.Birdie => (PlayerTextPopupType.Birdie, GameManager.MatchSettings.BirdieScore), 
					StrokesUnderParType.Eagle => (PlayerTextPopupType.Eagle, GameManager.MatchSettings.EagleScore), 
					StrokesUnderParType.Albatross => (PlayerTextPopupType.Albatross, GameManager.MatchSettings.AlbatrossScore), 
					StrokesUnderParType.Condor => (PlayerTextPopupType.Condor, GameManager.MatchSettings.CondorScore), 
					_ => (PlayerTextPopupType.None, 0), 
				};
			}
			if (MatchSetupRules.GetValueAsBool(MatchSetupRules.Rule.OnOrBelowPar) && strokesUnderParType != StrokesUnderParType.None)
			{
				value.courseScore += num;
				value.matchScore += num;
				playerAsGolfer.PlayerInfo.RpcPopUp(popupType, num);
				InfoFeed.ShowStrokesMessage(playerAsGolfer.PlayerInfo, strokesUnderParType, strokesUnderPar);
			}
			playerAsGolfer.PlayerInfo.RpcInformOfHoleFinishStrokesUnderPar(strokesUnderParType);
			if (isInDrivingRange)
			{
				value.courseStrokesOnFinishedHoles += value.matchStrokes;
				value.courseParOnFinishedHoles += GetCurrentHoleParInternal();
			}
			value.bestHoleScore = (StrokesUnderParType)BMath.Max((int)value.bestHoleScore, (int)strokesUnderParType);
			isChipIn = false;
			if (CanBeChipIn())
			{
				float magnitude = (hole.transform.position - playerAsGolfer.OwnBall.ServerLastStrokePosition).magnitude;
				if (magnitude >= (float)GameManager.MatchSettings.ChipInMinDistance)
				{
					isChipIn = true;
					int chipInScore = GameManager.MatchSettings.ChipInScore;
					value.courseScore += chipInScore;
					value.matchScore += chipInScore;
					value.longestChipIn = BMath.Max(value.longestChipIn, magnitude);
					playerAsGolfer.PlayerInfo.RpcPopUp(PlayerTextPopupType.ChipIn, chipInScore);
					InfoFeed.ShowChipInMessage(playerAsGolfer.PlayerInfo, magnitude);
				}
			}
			float timeSince = BMath.GetTimeSince((isInDrivingRange && playerAsGolfer.OwnBall != null) ? playerAsGolfer.OwnBall.LastRespawnTimestamp : matchStartTimestamp);
			float speedrunTimeForPar = GameManager.MatchSettings.GetSpeedrunTimeForPar(currentHoleParInternal);
			isSpeedrun = timeSince < speedrunTimeForPar;
			if (MatchSetupRules.GetValueAsBool(MatchSetupRules.Rule.Speedrun) && isSpeedrun)
			{
				int speedrunScore = GameManager.MatchSettings.SpeedrunScore;
				value.courseScore += speedrunScore;
				value.matchScore += speedrunScore;
				playerAsGolfer.PlayerInfo.RpcPopUp(PlayerTextPopupType.Speedrun, speedrunScore);
				InfoFeed.ShowSpeedrunMessage(playerAsGolfer.PlayerInfo, timeSince);
			}
			bool flag = currentHoleFirstPlaceState.playerGuid != value.playerGuid;
			if (MatchSetupRules.GetValueAsBool(MatchSetupRules.Rule.Comeback) && flag)
			{
				int num2 = currentHoleFirstPlaceState.courseScore - currentHoleFirstPlaceState.matchScore;
				int num3 = value.courseScore - value.matchScore;
				int num4 = num2 - num3;
				if (num4 >= GameManager.MatchSettings.LowerComebackBonus.MinPointsGap)
				{
					MatchSettings.ComebackBonus comebackBonus = ((num4 >= GameManager.MatchSettings.UpperComebackBonus.MinPointsGap) ? GameManager.MatchSettings.UpperComebackBonus : GameManager.MatchSettings.LowerComebackBonus);
					int num5 = (int)BMath.CeilToMultipleOf((float)value.matchScore * comebackBonus.BonusMultiplier, 5f);
					value.courseScore += num5;
					value.matchScore += num5;
					InfoFeed.ShowComebackMessage(playerAsGolfer.PlayerInfo, num5);
					playerAsGolfer.PlayerInfo.RpcPopUp(PlayerTextPopupType.Comeback, num5);
				}
			}
			if (float.IsFinite(timeSince))
			{
				value.avgFinishTime += (timeSince - value.avgFinishTime) / (float)value.finishes;
			}
			playerStates[index] = value;
			SendScoreAnnouncerLines();
			if (!isInDrivingRange && matchState < MatchState.CountingDownToEnd)
			{
				countdownRemainingTime = BMath.Min(countdownRemainingTime, MatchSetupRules.GetValue(MatchSetupRules.Rule.Countdown));
				NetworkmatchState = MatchState.CountingDownToEnd;
			}
			if (value.matchKnockouts <= 0)
			{
				playerAsGolfer.PlayerInfo.RpcInformScoredWithNoMatchKnockouts();
			}
		}
		bool CanBeChipIn()
		{
			if (!MatchSetupRules.GetValueAsBool(MatchSetupRules.Rule.ChipIn))
			{
				return false;
			}
			if (hole == null)
			{
				return false;
			}
			if (playerAsGolfer.OwnBall == null)
			{
				return false;
			}
			if (hole.IsMainHole)
			{
				if (BoundsManager.IsPointInGreenBoundsImmediate(playerAsGolfer.OwnBall.ServerLastStrokePosition))
				{
					return false;
				}
			}
			else if (hole.IsPointInGreenTrigger(playerAsGolfer.OwnBall.ServerLastStrokePosition))
			{
				return false;
			}
			return true;
		}
		void SendScoreAnnouncerLines()
		{
			List<AnnouncerLine> value2;
			using (CollectionPool<List<AnnouncerLine>, AnnouncerLine>.Get(out value2))
			{
				if (strokesUnderParType == StrokesUnderParType.HoleInOne)
				{
					RpcPlayAnnouncerLine(AnnouncerLine.HoleInOne);
				}
				if (!isInDrivingRange && placement == 0)
				{
					value2.Add(AnnouncerLine.FirstPlace);
				}
				if (isChipIn)
				{
					value2.Add(AnnouncerLine.ChipIn);
				}
				AnnouncerLine announcerLine = strokesUnderParType switch
				{
					StrokesUnderParType.Par => AnnouncerLine.Par, 
					StrokesUnderParType.Birdie => AnnouncerLine.Birdie, 
					StrokesUnderParType.Eagle => AnnouncerLine.Eagle, 
					StrokesUnderParType.Albatross => AnnouncerLine.Albatross, 
					StrokesUnderParType.Condor => AnnouncerLine.Condor, 
					_ => AnnouncerLine.None, 
				};
				if (announcerLine != AnnouncerLine.None)
				{
					value2.Add(announcerLine);
				}
				if (isSpeedrun)
				{
					value2.Add(AnnouncerLine.Speedrun);
				}
				RpcPlayAnnouncerLines(playerAsGolfer.connectionToClient, value2);
			}
		}
	}

	[Server]
	private void InformPlayerKnockedOutInternal(PlayerMovement knockedOutPlayer, PlayerInfo responsiblePlayer, KnockoutType knockoutType, out bool knockoutCounted)
	{
		if (!NetworkServer.active)
		{
			UnityEngine.Debug.LogWarning("[Server] function 'System.Void CourseManager::InformPlayerKnockedOutInternal(PlayerMovement,PlayerInfo,KnockoutType,System.Boolean&)' called when server was not active");
			knockoutCounted = default(bool);
			return;
		}
		knockoutCounted = false;
		if (knockedOutPlayer == null || !DoesKnockoutOnPlayerCount(knockedOutPlayer.PlayerInfo))
		{
			return;
		}
		bool flag = responsiblePlayer == null || responsiblePlayer == knockedOutPlayer.PlayerInfo;
		if (flag)
		{
			InfoFeed.ShowSelfKnockoutMessage(knockedOutPlayer.PlayerInfo, knockoutType);
			return;
		}
		InfoFeed.ShowKnockoutMessage(responsiblePlayer, knockedOutPlayer.PlayerInfo, knockoutType);
		if (!flag && TryGetPlayerStateIndex(responsiblePlayer.connectionToClient, out var index))
		{
			knockoutCounted = true;
			int num = (MatchSetupRules.GetValueAsBool(MatchSetupRules.Rule.Knockouts) ? GameManager.MatchSettings.KnockoutScore : 0);
			PlayerPair playerPair = new PlayerPair(responsiblePlayer.PlayerId.Guid, knockedOutPlayer.PlayerInfo.PlayerId.Guid);
			bool flag2 = MatchSetupRules.GetValueAsBool(MatchSetupRules.Rule.WhiteFlag) && playerDominations.Contains(playerPair);
			responsiblePlayer.Movement.RpcInformKnockedOutOtherPlayer(knockedOutPlayer.PlayerInfo, !flag2);
			if (num != 0)
			{
				responsiblePlayer.RpcPopUp(PlayerTextPopupType.Knockout, num);
			}
			PlayerState value = playerStates[index];
			value.matchKnockouts++;
			value.courseKnockouts++;
			value.matchKnockoutStreak++;
			value.courseScore += num;
			value.matchScore += num;
			playerStates[index] = value;
			IncrementRecentKnockouts();
			if (!playerKnockoutStreaks.TryGetValue(playerPair, out var value2))
			{
				value2 = default(KnockoutStreak);
			}
			value2.persistentStreak++;
			value2.redShieldStreak++;
			playerKnockoutStreaks[playerPair] = value2;
			bool num2 = value2.persistentStreak == GameManager.MatchSettings.DominationKnockoutStreak;
			bool flag3 = false;
			PlayerPair playerPair2 = playerPair.Inverse();
			if (playerKnockoutStreaks.TryGetValue(playerPair2, out var value3))
			{
				playerKnockoutStreaks.Remove(playerPair2);
				flag3 = value3.persistentStreak >= GameManager.MatchSettings.DominationKnockoutStreak;
			}
			if (num2)
			{
				playerDominations.Add(playerPair);
				InfoFeed.ShowDominationMessage(responsiblePlayer, knockedOutPlayer.PlayerInfo);
			}
			if (flag3)
			{
				playerDominations.Remove(playerPair2);
				responsiblePlayer.RpcInformScoredRevengeKnockout();
				InfoFeed.ShowRevengeMessage(responsiblePlayer, knockedOutPlayer.PlayerInfo);
			}
			if (TryGetPlayerStateIndex(knockedOutPlayer.connectionToClient, out var index2))
			{
				PlayerState value4 = playerStates[index2];
				value4.matchKnockedOut++;
				value4.courseKnockedOut++;
				value4.matchKnockoutStreak = 0;
				playerStates[index2] = value4;
			}
		}
		void IncrementRecentKnockouts()
		{
			if (!recentScoredKnockoutTimestamps.TryGetValue(responsiblePlayer, out var value5))
			{
				value5 = new List<double>();
				recentScoredKnockoutTimestamps.Add(responsiblePlayer, value5);
			}
			for (int num3 = value5.Count - 1; num3 >= 0; num3--)
			{
				if (BMath.GetTimeSince(value5[num3]) > (float)GameManager.Achievements.TargetRichEnvironmentKnockoutTimeWindow)
				{
					value5.RemoveAtSwapBack(num3);
				}
			}
			value5.Add(Time.timeAsDouble);
			if (value5.Count >= GameManager.Achievements.TargetRichEnvironmentKnockoutCount)
			{
				responsiblePlayer.RpcInformQualifiedTargetRichEnvironmentAchievement();
			}
		}
	}

	[Server]
	private void MarkLatestValidKnockoutInternal(PlayerInfo knockedOutPlayer, ItemUseId itemUseId)
	{
		if (!NetworkServer.active)
		{
			UnityEngine.Debug.LogWarning("[Server] function 'System.Void CourseManager::MarkLatestValidKnockoutInternal(PlayerInfo,ItemUseId)' called when server was not active");
			return;
		}
		bool flag = false;
		if (!latestValidKnockouts.TryGetValue(knockedOutPlayer, out var value))
		{
			latestValidKnockouts.Add(knockedOutPlayer, itemUseId);
			flag = true;
		}
		else if (!itemUseId.Equals(value))
		{
			latestValidKnockouts[knockedOutPlayer] = itemUseId;
			flag = true;
		}
		if (flag)
		{
			TryAwardPlayingWithFireAchievement();
		}
		void TryAwardPlayingWithFireAchievement()
		{
			if (itemUseId.IsValid() && GameManager.AllItems.TryGetItemData(itemUseId.itemType, out var itemData) && itemData.IsExplosive)
			{
				bool flag2 = false;
				bool flag3 = false;
				PlayerInfo playerInfo = null;
				foreach (var (playerInfo3, itemUseId3) in latestValidKnockouts)
				{
					if (itemUseId3.Equals(itemUseId))
					{
						bool flag4 = playerInfo3.PlayerId.Guid == itemUseId3.userGuid;
						if (!flag2 && flag4)
						{
							flag2 = true;
							playerInfo = playerInfo3;
							if (flag3)
							{
								break;
							}
						}
						if (!flag3 && !flag4)
						{
							flag3 = true;
							if (flag2)
							{
								break;
							}
						}
					}
				}
				if (flag2 && flag3 && playerInfo != null)
				{
					playerInfo.InformKnockedOutSelfAndOtherPlayerWithExplosive();
				}
			}
		}
	}

	[Server]
	private void InformPlayerEliminatedInternal(PlayerGolfer eliminatedPlayer, PlayerGolfer responsiblePlayer, EliminationReason reason, EliminationReason immediateEliminationReason)
	{
		if (!NetworkServer.active)
		{
			UnityEngine.Debug.LogWarning("[Server] function 'System.Void CourseManager::InformPlayerEliminatedInternal(PlayerGolfer,PlayerGolfer,EliminationReason,EliminationReason)' called when server was not active");
		}
		else
		{
			if (!TryGetPlayerStateIndex(eliminatedPlayer.connectionToClient, out var index))
			{
				return;
			}
			int index2 = -1;
			bool num = responsiblePlayer == null || responsiblePlayer == eliminatedPlayer || !TryGetPlayerStateIndex(responsiblePlayer.connectionToClient, out index2);
			PlayerState value = playerStates[index];
			value.losses++;
			playerStates[index] = value;
			if (!num)
			{
				PlayerState value2 = playerStates[index2];
				value2.eliminations++;
				playerStates[index2] = value2;
			}
			if (num)
			{
				InfoFeed.ShowSelfEliminationMessage(eliminatedPlayer.PlayerInfo, reason);
			}
			else
			{
				InfoFeed.ShowEliminationMessage(responsiblePlayer.PlayerInfo, eliminatedPlayer.PlayerInfo, reason);
			}
			if (!num && responsiblePlayer != null)
			{
				responsiblePlayer.PlayerInfo.RpcInformEliminatedOtherPlayer(reason, immediateEliminationReason);
			}
			if (eliminatedPlayer != null)
			{
				Hittable asHittable = eliminatedPlayer.PlayerInfo.AsHittable;
				if (asHittable.IsFrozen && asHittable.FreezerPlayerGuid != 0L && PlayerInfo.playerInfoPerPlayerGuid.TryGetValue(asHittable.FreezerPlayerGuid, out var value3))
				{
					value3.RpcInformOtherPlayerEliminatedWhileFrozenBySelf();
				}
			}
		}
	}

	[Server]
	private void InformBallDispensedInternal(PlayerGolfer owner)
	{
		int index;
		if (!NetworkServer.active)
		{
			UnityEngine.Debug.LogWarning("[Server] function 'System.Void CourseManager::InformBallDispensedInternal(PlayerGolfer)' called when server was not active");
		}
		else if (SingletonBehaviour<DrivingRangeManager>.HasInstance && TryGetPlayerStateIndex(owner.connectionToClient, out index))
		{
			PlayerState value = playerStates[index];
			value.matchStrokes = 0;
			playerStates[index] = value;
		}
	}

	[Server]
	private void InformPlayerPickedUpItemInternal(PlayerInfo player)
	{
		int index;
		if (!NetworkServer.active)
		{
			UnityEngine.Debug.LogWarning("[Server] function 'System.Void CourseManager::InformPlayerPickedUpItemInternal(PlayerInfo)' called when server was not active");
		}
		else if (TryGetPlayerStateIndex(player.connectionToClient, out index))
		{
			PlayerState value = playerStates[index];
			value.itemPickups++;
			playerStates[index] = value;
		}
	}

	[Server]
	private void AddPenaltyStrokeInternal(PlayerGolfer penalizedPlayer, bool suppressPopup)
	{
		int index;
		if (!NetworkServer.active)
		{
			UnityEngine.Debug.LogWarning("[Server] function 'System.Void CourseManager::AddPenaltyStrokeInternal(PlayerGolfer,System.Boolean)' called when server was not active");
		}
		else if (TryGetPlayerStateIndex(penalizedPlayer.connectionToClient, out index))
		{
			PlayerState value = playerStates[index];
			value.courseStrokes++;
			value.matchStrokes++;
			playerStates[index] = value;
			if (!suppressPopup)
			{
				penalizedPlayer.PlayerInfo.RpcPopUp(PlayerTextPopupType.PenaltyStroke, 1);
			}
		}
	}

	[Server]
	private void SetPlayerSpectatorInternal(PlayerGolfer player, bool isSpectator)
	{
		if (!NetworkServer.active)
		{
			UnityEngine.Debug.LogWarning("[Server] function 'System.Void CourseManager::SetPlayerSpectatorInternal(PlayerGolfer,System.Boolean)' called when server was not active");
			return;
		}
		if (!TryGetPlayerStateIndex(player.connectionToClient, out var index))
		{
			UnityEngine.Debug.LogError("Couldn't find player state from connection!");
			return;
		}
		PlayerState value = playerStates[index];
		bool isSpectator2 = value.isSpectator;
		if (isSpectator2 == isSpectator)
		{
			return;
		}
		value.isSpectator = isSpectator;
		playerStates[index] = value;
		if (!SingletonBehaviour<DrivingRangeManager>.HasInstance)
		{
			if (isSpectator2)
			{
				RespawnPlayer();
			}
			else
			{
				player.ServerInitializeAsSpectator(fromHoleStart: false);
			}
		}
		GolfTeeManager.UpdateActivePlatforms();
		async void RespawnPlayer()
		{
			NetworkConnectionToClient connection = player.connectionToClient;
			GameObject playerObject = player.gameObject;
			NetworkServer.DestroyPlayerForConnection(connection);
			await UniTask.WaitUntil(() => playerObject == null);
			BNetworkManager.singleton.OnServerAddPlayer(connection);
		}
	}

	private bool IsPlayerSpectatorInternal(PlayerGolfer player)
	{
		if (!TryFindPlayerStateInternal(player, out var playerState))
		{
			return false;
		}
		return playerState.isSpectator;
	}

	[Server]
	private void ServerSetCourseInternal(CourseData course, NetworkConnectionToClient specificClientConnection = null)
	{
		if (!NetworkServer.active)
		{
			UnityEngine.Debug.LogWarning("[Server] function 'System.Void CourseManager::ServerSetCourseInternal(CourseData,Mirror.NetworkConnectionToClient)' called when server was not active");
		}
		else if (!(course == null) && (specificClientConnection == null || specificClientConnection != NetworkServer.localConnection))
		{
			if (specificClientConnection == null)
			{
				GameManager.ServerSetCourse(course);
			}
			if (course == MatchSetupMenu.RandomCourseData)
			{
				SendNonStandardCourseToAllClients(isRandom: true);
			}
			else if (course == MatchSetupMenu.CustomCourseData)
			{
				SendNonStandardCourseToAllClients(isRandom: false);
			}
			else
			{
				SendStandardCourseToAllClients();
			}
		}
		void SendNonStandardCourseToAllClients(bool isRandom)
		{
			int[] array = new int[course.Holes.Length];
			for (int i = 0; i < course.Holes.Length; i++)
			{
				array[i] = course.Holes[i].GlobalIndex;
			}
			SetNonStandardCourseMessage message = new SetNonStandardCourseMessage
			{
				globalHoleIndices = array,
				isRandom = isRandom
			};
			if (specificClientConnection != null)
			{
				specificClientConnection.Send(message);
				return;
			}
			foreach (NetworkConnectionToClient value in NetworkServer.connections.Values)
			{
				if (value != NetworkServer.localConnection)
				{
					value.Send(message);
				}
			}
		}
		void SendStandardCourseToAllClients()
		{
			int num = Array.IndexOf(GameManager.AllCourses.Courses, course);
			if (num < 0)
			{
				UnityEngine.Debug.LogError($"Attempted to set course index to {num} for all clients, but it cannot be negative. To set a random or custom course, use the matching method");
			}
			else
			{
				SetStandardCourseMessage message = new SetStandardCourseMessage
				{
					courseIndex = num
				};
				if (specificClientConnection == null)
				{
					foreach (NetworkConnectionToClient value2 in NetworkServer.connections.Values)
					{
						if (value2 != NetworkServer.localConnection)
						{
							value2.Send(message);
						}
					}
					return;
				}
				specificClientConnection.Send(message);
			}
		}
	}

	private bool TryFindPlayerStateInternal(PlayerGolfer player, out PlayerState playerState)
	{
		playerState = default(PlayerState);
		if (base.isServer)
		{
			if (!TryGetPlayerStateIndex(player.connectionToClient, out var index))
			{
				return false;
			}
			playerState = playerStates[index];
			return true;
		}
		ulong guid = player.PlayerInfo.PlayerId.Guid;
		foreach (PlayerState playerState2 in playerStates)
		{
			if (playerState2.playerGuid == guid)
			{
				playerState = playerState2;
				return true;
			}
		}
		return false;
	}

	private int GetCurrentHoleParInternal()
	{
		if (SingletonBehaviour<DrivingRangeManager>.HasInstance)
		{
			return GameManager.DrivingRangeHoleData.Par;
		}
		if (currentHoleGlobalIndex < 0)
		{
			return -1;
		}
		return GameManager.AllCourses.allHoles[currentHoleGlobalIndex].Par;
	}

	private LocalizedString GetCurrentCourseLocalizedNameInternal()
	{
		if (SingletonBehaviour<DrivingRangeManager>.HasInstance)
		{
			return null;
		}
		if (currentHoleGlobalIndex < 0)
		{
			return null;
		}
		if (GameManager.CurrentCourse == null)
		{
			return null;
		}
		return GameManager.CurrentCourse.LocalizedName;
	}

	private LocalizedString GetCurrentHoleLocalizedNameInternal()
	{
		if (SingletonBehaviour<DrivingRangeManager>.HasInstance)
		{
			return Localization.UI.HOLE_INFO_DrivingRange_Ref;
		}
		if (currentHoleGlobalIndex < 0)
		{
			return null;
		}
		return GameManager.AllCourses.allHoles[currentHoleGlobalIndex].LocalizedName;
	}

	private List<PlayerState> GetSortedPlayerStatesInternal(bool includeSpectators = true)
	{
		sortedPlayerStatesBuffer.Clear();
		for (int i = 0; i < playerStates.Count; i++)
		{
			PlayerState item = playerStates[i];
			if (item.isConnected && (includeSpectators || !item.isSpectator))
			{
				sortedPlayerStatesBuffer.Add(item);
			}
		}
		sortedPlayerStatesBuffer.Sort();
		return sortedPlayerStatesBuffer;
	}

	private bool TryGetPlayerStateInternal(ulong playerGuid, out PlayerState state)
	{
		state = default(PlayerState);
		if (!playerStateIndicesPerPlayerGuid.TryGetValue(playerGuid, out var value))
		{
			return false;
		}
		state = playerStates[value];
		return true;
	}

	private bool TryGetPlayerStateInternal(PlayerInfo player, out PlayerState state)
	{
		state = default(PlayerState);
		if (player == null)
		{
			return false;
		}
		if (!playerStateIndicesPerPlayerGuid.TryGetValue(player.PlayerId.Guid, out var value))
		{
			return false;
		}
		state = playerStates[value];
		return true;
	}

	private PlayerState GetFirstPlaceStateInternal()
	{
		PlayerState result = playerStates[0];
		for (int i = 1; i < playerStates.Count; i++)
		{
			if (playerStates[i].isConnected && result.CompareTo(playerStates[i]) > 0)
			{
				result = playerStates[i];
			}
		}
		return result;
	}

	private PlayerState GetLastPlaceStateInternal()
	{
		PlayerState result = playerStates[0];
		for (int i = 1; i < playerStates.Count; i++)
		{
			if (playerStates[i].isConnected && result.CompareTo(playerStates[i]) < 0)
			{
				result = playerStates[i];
			}
		}
		return result;
	}

	private int CountActivePlayersInternal()
	{
		int num = 0;
		for (int i = 0; i < playerStates.Count; i++)
		{
			PlayerState playerState = playerStates[i];
			if (playerState.isConnected && !playerState.isSpectator)
			{
				num++;
			}
		}
		return num;
	}

	private void GetConnectedPlayerStatesInternal(List<PlayerState> connectedPlayerStates)
	{
		foreach (PlayerState playerState in playerStates)
		{
			if (playerState.isConnected)
			{
				connectedPlayerStates.Add(playerState);
			}
		}
	}

	[Server]
	private void ServerSpawnItemInternal(ItemType item, int remainingUses, Vector3 position, Quaternion rotation, Vector3 velocity, Vector3 angularVelocity, ItemUseId itemUseId, PlayerInventory spawner)
	{
		if (!NetworkServer.active)
		{
			UnityEngine.Debug.LogWarning("[Server] function 'System.Void CourseManager::ServerSpawnItemInternal(ItemType,System.Int32,UnityEngine.Vector3,UnityEngine.Quaternion,UnityEngine.Vector3,UnityEngine.Vector3,ItemUseId,PlayerInventory)' called when server was not active");
		}
		else
		{
			ServerSpawnItem(item, remainingUses, position, rotation, velocity, angularVelocity, spawner, itemUseId, networkSpawn: true);
		}
	}

	[Server]
	private PhysicalItem ServerSpawnItem(ItemType item, int remainingUses, Vector3 position, Quaternion rotation, Vector3 velocity, Vector3 angularVelocity, PlayerInventory spawner, ItemUseId itemUseId, bool networkSpawn)
	{
		if (!NetworkServer.active)
		{
			UnityEngine.Debug.LogWarning("[Server] function 'PhysicalItem CourseManager::ServerSpawnItem(ItemType,System.Int32,UnityEngine.Vector3,UnityEngine.Quaternion,UnityEngine.Vector3,UnityEngine.Vector3,PlayerInventory,ItemUseId,System.Boolean)' called when server was not active");
			return null;
		}
		if (!GameManager.AllItems.TryGetItemData(item, out var itemData))
		{
			UnityEngine.Debug.LogError($"Could not find data for item {item}");
			return null;
		}
		if (itemData.MaxUses > 0 && remainingUses <= 0)
		{
			UnityEngine.Debug.LogError($"Attempted to spawn an item of type {item} with {remainingUses} remaining uses", base.gameObject);
			return null;
		}
		GameObject gameObject = UnityEngine.Object.Instantiate(itemData.Prefab, position, rotation);
		if (gameObject == null)
		{
			UnityEngine.Debug.LogError($"Dropped item of type {item} did not instantiate properly", base.gameObject);
			return null;
		}
		if (!gameObject.TryGetComponent<PhysicalItem>(out var component))
		{
			UnityEngine.Debug.LogError(string.Format("Dropped item of type {0} does not have a {1} component", item, "PhysicalItem"), base.gameObject);
			UnityEngine.Object.Destroy(gameObject);
			return null;
		}
		component.Initialize(remainingUses);
		component.AsEntity.Rigidbody.linearVelocity = velocity;
		component.AsEntity.Rigidbody.angularVelocity = angularVelocity;
		if (item == ItemType.Landmine && spawner != null && component.TryGetComponent<Landmine>(out var component2))
		{
			component2.ServerInitialize(LandmineArmType.None, spawner, itemUseId);
		}
		if (networkSpawn)
		{
			NetworkServer.Spawn(gameObject);
		}
		return component;
	}

	[Server]
	private void ServerSpawnLandmineInternal(Vector3 position, Quaternion rotation, Vector3 velocity, Vector3 angularVelocity, LandmineArmType armType, ItemUseId itemUseId, PlayerInventory owner)
	{
		if (!NetworkServer.active)
		{
			UnityEngine.Debug.LogWarning("[Server] function 'System.Void CourseManager::ServerSpawnLandmineInternal(UnityEngine.Vector3,UnityEngine.Quaternion,UnityEngine.Vector3,UnityEngine.Vector3,LandmineArmType,ItemUseId,PlayerInventory)' called when server was not active");
		}
		else if (!(owner == null))
		{
			PhysicalItem physicalItem = ServerSpawnItem(ItemType.Landmine, 1, position, rotation, velocity, angularVelocity, null, itemUseId, networkSpawn: false);
			if (!physicalItem.TryGetComponent<Landmine>(out var component))
			{
				UnityEngine.Debug.LogError("Dropped landmine does not have a Landmine component", base.gameObject);
				UnityEngine.Object.Destroy(physicalItem.gameObject);
			}
			component.ServerInitialize(armType, owner, itemUseId);
			NetworkServer.Spawn(component.gameObject);
		}
	}

	[Command(requiresAuthority = false)]
	private void CmdSpawnGolfCartForLocalPlayerInternal(NetworkConnectionToClient sender = null)
	{
		if (base.isServer && base.isClient)
		{
			UserCode_CmdSpawnGolfCartForLocalPlayerInternal__NetworkConnectionToClient(sender);
			return;
		}
		NetworkWriterPooled writer = NetworkWriterPool.Get();
		SendCommandInternal("System.Void CourseManager::CmdSpawnGolfCartForLocalPlayerInternal(Mirror.NetworkConnectionToClient)", -69988197, writer, 0, requiresAuthority: false);
		NetworkWriterPool.Return(writer);
	}

	private void ClearPlayerStates()
	{
		for (int i = 0; i < playerStates.Count; i++)
		{
			playerStates[i] = PlayerState.GetClearedState(playerStates[i]);
		}
		playerKnockoutStreaks.Clear();
	}

	private bool TryGetPlayerStateIndex(NetworkConnectionToClient connection, out int index)
	{
		if (!BNetworkManager.singleton.ServerTryGetPlayerGuidFromConnection(connection, out var playerGuid))
		{
			UnityEngine.Debug.LogError($"Failed to get player GUID for connection {connection.connectionId}", base.gameObject);
			index = 0;
			return false;
		}
		return playerStateIndicesPerPlayerGuid.TryGetValue(playerGuid, out index);
	}

	private void BeginHoleOverviewAndTeeOffCountdown()
	{
		if (holeOverviewAndTeeOffCountdownRoutine != null)
		{
			StopCoroutine(holeOverviewAndTeeOffCountdownRoutine);
		}
		holeOverviewAndTeeOffCountdownRoutine = StartCoroutine(HoleOverviewAndTeeOffCountdownRoutine());
	}

	private void BeginCountdownToMatchEnd()
	{
		if (matchEndCountdownRoutine != null)
		{
			StopCoroutine(matchEndCountdownRoutine);
		}
		matchEndCountdownRoutine = StartCoroutine(CountDownToMatchEndRoutine());
	}

	private IEnumerator HoleOverviewAndTeeOffCountdownRoutine()
	{
		NetworkmatchState = MatchState.HoleOverview;
		HoleOverviewCameraUi.Hide();
		yield return new WaitForSeconds(GameManager.MatchSettings.HoleOverviewInitialBlankDuration);
		HoleOverviewCameraUi.SetState(HoleOverviewCameraUi.State.DisplayingHoleName);
		HoleOverviewCameraUi.Show();
		yield return new WaitForSeconds(GameManager.MatchSettings.HoleOverviewHoleNameDuration);
		HoleOverviewCameraUi.Hide();
		yield return new WaitForSeconds(GameManager.MatchSettings.HoleOverviewFinalBlankDuration);
		RpcSlowDownOverviewCamera(GameManager.MatchSettings.HoleOverviewCameraSlowdownDuration);
		yield return new WaitForSeconds(GameManager.MatchSettings.HoleOverviewCameraSlowdownDuration);
		NetworkisHoleOverviewFinished = true;
		yield return new WaitForSeconds(GameManager.MatchSettings.HoleOverviewFlyOverToTeeDuration);
		NetworkmatchState = MatchState.TeeOff;
		TeeOffCountdown.Show();
		for (float remainingTime = GameManager.MatchSettings.TeeOffCountdownDuration; remainingTime > 0f; remainingTime -= Time.deltaTime)
		{
			TeeOffCountdown.SetRemainingTime(remainingTime);
			yield return null;
		}
		TeeOffCountdown.SetRemainingTime(0f);
		NetworkmatchState = MatchState.Ongoing;
		matchStartTimestamp = Time.timeAsDouble;
		yield return new WaitForSeconds(1f);
		TeeOffCountdown.Hide();
		if (MatchSetupRules.GetValueAsBool(MatchSetupRules.Rule.MaxTimeBasedOnPar))
		{
			countdownRemainingTime = GetCurrentHoleParInternal() * 30;
			NetworkmatchState = MatchState.CountingDownToEnd;
		}
	}

	[ClientRpc]
	private void RpcSlowDownOverviewCamera(float duration)
	{
		NetworkWriterPooled writer = NetworkWriterPool.Get();
		writer.WriteFloat(duration);
		SendRPCInternal("System.Void CourseManager::RpcSlowDownOverviewCamera(System.Single)", -650303162, writer, 0, includeOwner: true);
		NetworkWriterPool.Return(writer);
	}

	private IEnumerator CountDownToMatchEndRoutine()
	{
		MatchEndCountdown.Show();
		while (countdownRemainingTime > 0f)
		{
			MatchEndCountdown.SetTime(countdownRemainingTime);
			yield return null;
			countdownRemainingTime -= Time.deltaTime;
		}
		PopulateOvertimeBalls();
		if (overtimeActiveBalls.Count > 0)
		{
			NetworkmatchState = MatchState.Overtime;
			countdownRemainingTime = GameManager.MatchSettings.OvertimeDuration;
			while (countdownRemainingTime > 0f && overtimeActiveBalls.Count > 0)
			{
				MatchEndCountdown.SetTime(countdownRemainingTime);
				yield return null;
				countdownRemainingTime -= Time.deltaTime;
				UpdateOvertime();
			}
		}
		NetworkmatchState = MatchState.Ended;
		static bool IsBallMoving(GolfBall ball)
		{
			return ball.Rigidbody.linearVelocity.sqrMagnitude > 0.0001f;
		}
		void PopulateOvertimeBalls()
		{
			overtimeActiveBalls.Clear();
			overtimeTimeSinceMovedPerActiveBall.Clear();
			foreach (GolfBall activeBall in activeBalls)
			{
				if (IsBallMoving(activeBall))
				{
					overtimeActiveBalls.Add(activeBall);
				}
			}
		}
		void UpdateOvertime()
		{
			for (int num = overtimeActiveBalls.Count - 1; num >= 0; num--)
			{
				GolfBall golfBall = overtimeActiveBalls[num];
				if (IsBallMoving(golfBall))
				{
					overtimeTimeSinceMovedPerActiveBall[golfBall] = 0f;
				}
				else if (golfBall.IsStationary)
				{
					overtimeActiveBalls.RemoveAt(num);
				}
				else
				{
					if (!overtimeTimeSinceMovedPerActiveBall.TryGetValue(golfBall, out var value))
					{
						value = 0f;
					}
					value += Time.deltaTime;
					if (value >= 2f)
					{
						overtimeActiveBalls.RemoveAt(num);
					}
					else
					{
						overtimeTimeSinceMovedPerActiveBall[golfBall] = value;
					}
				}
			}
		}
	}

	private void ServerUpdateMarkedFirstPlacePlayer()
	{
		NetworkmarkedFirstPlacePlayer = ((CanMarkFirstPlacePlayer() && PlayerInfo.playerInfoPerPlayerGuid.TryGetValue(GetFirstPlaceState().playerGuid, out var value)) ? value : null);
		bool CanMarkFirstPlacePlayer()
		{
			if (SingletonBehaviour<DrivingRangeManager>.HasInstance)
			{
				return false;
			}
			if (currentHoleCourseIndex == 0)
			{
				return false;
			}
			if (playerStates.Count <= 1)
			{
				return false;
			}
			return true;
		}
	}

	private void TryPlayHoleMusic(bool hurryUpInstantly)
	{
		if (!isPlayingHoleMusic)
		{
			if (SingletonBehaviour<DrivingRangeManager>.HasInstance)
			{
				PlayEvent(GameManager.AudioSettings.DrivingRangeMusicEvent, hurryUpInstantly: false);
			}
			else if (currentHoleGlobalIndex >= 0)
			{
				PlayEvent(GameManager.AllCourses.allHoles[currentHoleGlobalIndex].MusicEvent, hurryUpInstantly);
			}
		}
		void PlayEvent(EventReference eventReference, bool flag)
		{
			isPlayingHoleMusic = true;
			holeMusicInstance = RuntimeManager.CreateInstance(eventReference);
			if (flag)
			{
				HurryUpHoleMusic(instant: true);
			}
			holeMusicInstance.start();
			holeMusicInstance.release();
		}
	}

	private void HurryUpHoleMusic(bool instant)
	{
		if (hurryUpMusicParameterId.data1 == 0 && hurryUpMusicParameterId.data2 == 0)
		{
			RuntimeManager.GetEventDescription(GameManager.AllCourses.allHoles[currentHoleGlobalIndex].MusicEvent).getParameterDescriptionByName("Hurry Up", out var parameter);
			hurryUpMusicParameterId = parameter.id;
		}
		holeMusicInstance.setParameterByID(hurryUpMusicParameterId, instant ? 2f : 1f);
	}

	[Server]
	private void OnServerMatchParticipantsChanged()
	{
		if (!NetworkServer.active)
		{
			UnityEngine.Debug.LogWarning("[Server] function 'System.Void CourseManager::OnServerMatchParticipantsChanged()' called when server was not active");
			return;
		}
		ServerUpdateMarkedFirstPlacePlayer();
		if (!SingletonBehaviour<DrivingRangeManager>.HasInstance && serverMatchParticipants.Count <= 0 && (matchState != MatchState.Overtime || overtimeActiveBalls.Count <= 0))
		{
			NetworkmatchState = MatchState.Ended;
		}
	}

	private void OnActivePlayersOnGreenChanged()
	{
		if (!(GolfHoleManager.MainHole == null))
		{
			GolfHoleManager.MainHole.ServerSetAreAnyPlayersOnGreen(activePlayersOnGreen.Count > 0);
		}
	}

	private void OnPlayerStatesChanged(SyncList<PlayerState>.Operation operation, int itemIndex, PlayerState changedItem)
	{
		CourseManager.PlayerStatesChanged?.Invoke(operation, itemIndex, changedItem);
		if (base.isServer)
		{
			ServerUpdateMarkedFirstPlacePlayer();
		}
		PlayerState playerState = playerStates[itemIndex];
		if ((uint)operation == 1u)
		{
			string playerName = GetPlayerName(playerState);
			if (changedItem.isSpectator != playerState.isSpectator)
			{
				if (playerState.isSpectator)
				{
					TextChatUi.ShowMessage(string.Format(Localization.UI.TEXTCHAT_Info_PlayerJoinedSpectators, GameManager.UiSettings.ApplyColorTag(GameManager.RichTextNoParse(playerName), TextHighlight.Regular)));
				}
				else
				{
					TextChatUi.ShowMessage(string.Format(Localization.UI.TEXTCHAT_Info_PlayerJoinedGame, GameManager.UiSettings.ApplyColorTag(GameManager.RichTextNoParse(playerName), TextHighlight.Regular)));
				}
			}
			if (!BNetworkManager.IsChangingSceneOrShuttingDown && changedItem.isConnected && !playerState.isConnected)
			{
				TextChatUi.ShowMessage(string.Format(Localization.UI.TEXTCHAT_Info_PlayerDisconnected, GameManager.UiSettings.ApplyColorTag(GameManager.RichTextNoParse(playerName), TextHighlight.Regular)));
			}
			if (PlayerInfo.playerInfoPerPlayerGuid.TryGetValue(changedItem.playerGuid, out var value))
			{
				value.InformCourseStateChanged(changedItem, playerState);
			}
		}
		if (GameManager.LocalPlayerId != null && (uint)operation == 1u && playerState.playerGuid == GameManager.LocalPlayerId.Guid && !SingletonBehaviour<DrivingRangeManager>.HasInstance && changedItem.matchKnockoutStreak < GameManager.Achievements.BerserkerKnockoutStreak && playerState.matchKnockoutStreak >= GameManager.Achievements.BerserkerKnockoutStreak)
		{
			GameManager.AchievementsManager.Unlock(AchievementId.Berserker);
		}
		if (!SteamEnabler.IsSteamEnabled)
		{
			return;
		}
		if (BNetworkManager.TryGetPlayerInLobby(playerState.playerGuid, out var player))
		{
			switch (operation)
			{
			case SyncList<PlayerState>.Operation.OP_ADD:
				UnityEngine.Debug.Log($"Player \"{player.Name}\" ({player.Id}) connected");
				break;
			case SyncList<PlayerState>.Operation.OP_SET:
				if (playerState.isConnected != changedItem.isConnected)
				{
					if (playerState.isConnected)
					{
						UnityEngine.Debug.Log($"Player \"{player.Name}\" ({player.Id}) reconnected");
					}
					else
					{
						UnityEngine.Debug.Log($"Player \"{player.Name}\" ({player.Id}) disconnected");
					}
				}
				break;
			}
		}
		else if (playerState.isConnected)
		{
			UnityEngine.Debug.LogWarning($"Player with SteamId ({playerState.playerGuid}) doesn't exist in lobby, host is suspicious!");
		}
	}

	private void OnPlayerPingsChanged(SyncIDictionary<ulong, float>.Operation operation, ulong playerGuid, float ping)
	{
		CourseManager.PlayerPingsChanged?.Invoke(operation, playerGuid, ping);
	}

	private void OnPlayerKnockoutStreaksChanged(SyncIDictionary<PlayerPair, KnockoutStreak>.Operation operation, PlayerPair playerPair, KnockoutStreak streak)
	{
		CourseManager.PlayerKnockoutStreaksChanged?.Invoke(operation, playerPair, streak);
	}

	private void OnPlayerDominationsChanged(SyncSet<PlayerPair>.Operation operation, PlayerPair value)
	{
		CourseManager.PlayerDominationsChanged?.Invoke(operation, value);
		if (!playerStateIndicesPerPlayerGuid.TryGetValue(value.playerAGuid, out var value2))
		{
			return;
		}
		PlayerState value3 = playerStates[value2];
		if (base.isServer)
		{
			value3.dominatingCount = 0;
			foreach (PlayerPair playerDomination in playerDominations)
			{
				if (playerDomination.playerAGuid == value.playerAGuid)
				{
					value3.dominatingCount++;
				}
			}
			playerStates[value2] = value3;
		}
		if (GameManager.LocalPlayerId != null && GameManager.LocalPlayerId.Guid == value3.playerGuid)
		{
			GameManager.LocalPlayerInfo.InformDominationCountChanged(value3.dominatingCount);
		}
	}

	private void OnOvertimeActiveBallsChanged(SyncList<GolfBall>.Operation operation, int ballIndex, GolfBall changedBall)
	{
		CourseManager.OvertimeActiveBallsChanged?.Invoke(operation, ballIndex, changedBall);
	}

	private void OnServerAnyPlayerGuidChanged(PlayerId player)
	{
		ServerUpdateMarkedFirstPlacePlayer();
	}

	private void OnServerAnyPlayerIsRespawningChanged(PlayerMovement playerMovement)
	{
		if (TryGetPlayerStateIndex(playerMovement.connectionToClient, out var index))
		{
			PlayerState value = playerStates[index];
			value.isRespawning = playerMovement.IsRespawning;
			playerStates[index] = value;
		}
	}

	private void OnServerPlayerHitOwnBall(PlayerGolfer hitter)
	{
		if (!(hitter == null) && TryGetPlayerStateIndex(hitter.connectionToClient, out var index))
		{
			PlayerState value = playerStates[index];
			value.courseStrokes++;
			value.matchStrokes++;
			playerStates[index] = value;
		}
	}

	private void OnServerAnyPlayerMatchResolutionChanged(PlayerGolfer playerAsGolfer, PlayerMatchResolution previousResolution, PlayerMatchResolution currentResolution)
	{
		if (playerAsGolfer.IsMatchResolved && serverMatchParticipants.Remove(playerAsGolfer) && previousResolution != PlayerMatchResolution.Uninitialized)
		{
			OnServerMatchParticipantsChanged();
		}
		if (TryGetPlayerStateIndex(playerAsGolfer.connectionToClient, out var index))
		{
			PlayerState value = playerStates[index];
			value.matchResolution = playerAsGolfer.MatchResolution;
			if (value.matchResolution == PlayerMatchResolution.Scored)
			{
				value.courseStrokesOnFinishedHoles += value.matchStrokes;
				value.courseParOnFinishedHoles += GetCurrentHoleParInternal();
			}
			playerStates[index] = value;
		}
	}

	private void OnCurrentHoleCourseIndexChanged(int previousIndex, int currentIndex)
	{
		CourseManager.CurrentHoleCourseIndexChanged?.Invoke();
	}

	private void OnCurrentHoleGlobalIndexChanged(int previousIndex, int currentIndex)
	{
		TryPlayHoleMusic(hurryUpInstantly: false);
		CourseManager.CurrentHoleGlobalIndexChanged?.Invoke();
	}

	private void OnMatchStateChanged(MatchState previousState, MatchState currentState)
	{
		if (base.isServer)
		{
			switch (currentState)
			{
			case MatchState.CountingDownToEnd:
				BeginCountdownToMatchEnd();
				break;
			case MatchState.Overtime:
				MatchEndCountdown.EnterOvertime();
				break;
			default:
				if (matchEndCountdownRoutine != null)
				{
					StopCoroutine(matchEndCountdownRoutine);
				}
				MatchEndCountdown.Hide();
				break;
			}
			if (currentState == MatchState.Ended)
			{
				ServerInitiateMatchFinish();
			}
			if (currentState == MatchState.HoleOverview)
			{
				currentHoleFirstPlaceState = GetFirstPlaceState();
			}
		}
		if (currentState >= MatchState.TeeOff)
		{
			HudManager.Show(instant: false);
			if (previousState == MatchState.TeeOff)
			{
				teeoffEndTimestamp = Time.timeAsDouble;
			}
		}
		if (currentState < MatchState.Overtime)
		{
			bool num = currentState == MatchState.CountingDownToEnd;
			bool instant = num && previousState != MatchState.Ongoing;
			TryPlayHoleMusic(hurryUpInstantly: false);
			if (num)
			{
				HurryUpHoleMusic(instant);
			}
		}
		if (previousState > MatchState.Initializing)
		{
			if (previousState <= MatchState.CountingDownToEnd && currentState > MatchState.CountingDownToEnd)
			{
				RuntimeManager.PlayOneShot(GameManager.AudioSettings.FinishedWhistleEvent);
			}
			switch (currentState)
			{
			case MatchState.Overtime:
				PlayAnnouncerLineLocalOnlyInternal(AnnouncerLine.Overtime);
				break;
			case MatchState.Ended:
				PlayAnnouncerLineLocalOnlyInternal(AnnouncerLine.Finished);
				break;
			}
		}
		CourseManager.MatchStateChanged?.Invoke(previousState, currentState);
		async void AwardCourseBonus()
		{
			await UniTask.WaitForSeconds(1f);
			if (!(this == null))
			{
				List<PlayerState> sortedPlayerStatesInternal = GetSortedPlayerStatesInternal(includeSpectators: false);
				List<PlayerState> value;
				if (sortedPlayerStatesInternal.Count > 1)
				{
					using (CollectionPool<List<PlayerState>, PlayerState>.Get(out value))
					{
						for (int i = 0; i < sortedPlayerStatesInternal.Count; i++)
						{
							PlayerState playerState = sortedPlayerStatesInternal[i];
							if (playerState.isConnected && !playerState.isSpectator && BNetworkManager.singleton.ServerTryGetConnectionFromPlayerGuid(playerState.playerGuid, out var connection))
							{
								RpcAwardCourseBonus(awardMultiplier: (i == 0) ? 1f : ((i >= sortedPlayerStatesInternal.Count / 2) ? 0.5f : 0.75f), target: connection);
							}
						}
					}
				}
			}
		}
		async void ServerFinishMatchDelayed(bool isCourseFinished)
		{
			if (!MatchSetupRules.IsCheatsEnabled() && isCourseFinished)
			{
				AwardCourseBonus();
			}
			NextMatchCountdown.Show();
			NextMatchCountdown.SetIsCourseFinished(isCourseFinished);
			float delayDuration = (isCourseFinished ? GameManager.MatchSettings.FinishCourseDelay : GameManager.MatchSettings.StartNextMatchDelay);
			bool didEndPlayerStateCheck = false;
			for (float time = 0f; time < delayDuration; time += Time.deltaTime)
			{
				if (!didEndPlayerStateCheck && time >= 1f)
				{
					didEndPlayerStateCheck = true;
					int num2 = CountActivePlayersInternal();
					int num3 = 0;
					PlayerState playerState = default(PlayerState);
					PlayerState firstPlaceStateInternal = GetFirstPlaceStateInternal();
					for (int i = 0; i < playerStates.Count; i++)
					{
						PlayerState playerState2 = playerStates[i];
						if (isCourseFinished && PlayerInfo.playerInfoPerPlayerGuid.TryGetValue(playerState2.playerGuid, out var value))
						{
							value.RpcInformOfCourseEndState(playerState2);
						}
						if (playerState2.matchResolution == PlayerMatchResolution.Scored && ++num3 == 1)
						{
							playerState = playerState2;
						}
						if (playerState2.isConnected && num2 > 1 && playerState2.playerGuid == firstPlaceStateInternal.playerGuid)
						{
							playerState2.multiplayerFirstPlaceStreak++;
							if (playerState2.multiplayerFirstPlaceStreak >= GameManager.Achievements.OneTrueKingFirstPlaceStreak && PlayerInfo.playerInfoPerPlayerGuid.TryGetValue(playerState2.playerGuid, out var value2))
							{
								value2.RpcInformQualifiedForOneTrueKingAchievement();
							}
						}
						else
						{
							playerState2.multiplayerFirstPlaceStreak = 0;
						}
						playerStates[i] = playerState2;
					}
					if (num3 == 1 && num2 >= GameManager.Achievements.ThereCanBeOnlyOneMinTotalPlayerCount && PlayerInfo.playerInfoPerPlayerGuid.TryGetValue(playerState.playerGuid, out var value3))
					{
						value3.RpcInformQualifiedForThereCanBeOnlyOneAchievement();
					}
				}
				if (!forceDisplayScoreboard && time >= GameManager.MatchSettings.MatchEndScoreboardDisplayDelay)
				{
					NetworkforceDisplayScoreboard = true;
				}
				NextMatchCountdown.SetRemainingTime(delayDuration - time);
				await UniTask.Yield();
				if (this == null)
				{
					return;
				}
				if (!isCourseFinished && ShouldCourseFinish())
				{
					ServerFinishMatchDelayed(isCourseFinished: true);
					return;
				}
			}
			if (isCourseFinished)
			{
				EndCourseInternal();
			}
			else
			{
				ServerStartNextMatch(skipPersistentInventories: false);
			}
		}
		void ServerInitiateMatchFinish()
		{
			bool isCourseFinished = ShouldCourseFinish();
			ServerFinishMatchDelayed(isCourseFinished);
		}
		bool ShouldCourseFinish()
		{
			if (IsLastMatchOfCourse())
			{
				return true;
			}
			if (CountActivePlayersInternal() == 0)
			{
				return true;
			}
			return false;
		}
	}

	[TargetRpc]
	private void RpcAwardCourseBonus(NetworkConnectionToClient target, float awardMultiplier)
	{
		NetworkWriterPooled writer = NetworkWriterPool.Get();
		writer.WriteFloat(awardMultiplier);
		SendTargetRPCInternal(target, "System.Void CourseManager::RpcAwardCourseBonus(Mirror.NetworkConnectionToClient,System.Single)", 1118622923, writer, 0);
		NetworkWriterPool.Return(writer);
	}

	private void OnIsHoleOverviewFinishedChanged(bool wasFinished, bool isFinished)
	{
		if (isHoleOverviewFinished)
		{
			if (matchState >= MatchState.TeeOff)
			{
				GameplayCameraManager.TransitionTo(CameraModuleType.Orbit, 0f);
			}
			else
			{
				GameplayCameraManager.TransitionTo(CameraModuleType.Orbit, GameManager.MatchSettings.HoleOverviewFlyOverToTeeCurve);
			}
		}
		else
		{
			GameplayCameraManager.TransitionTo(CameraModuleType.Overview, 0f);
		}
	}

	private void OnForceDisplayScoreboardChanged(bool wasForceDisplaying, bool isForceDisplaying)
	{
		CourseManager.ForceDisplayScoreboardChanged?.Invoke();
	}

	private void OnMarkedFirstPlacePlayerChanged(PlayerInfo previousPlayer, PlayerInfo currentPlayer)
	{
		if (previousPlayer != null)
		{
			previousPlayer.UnmarkFirstPlace();
			previousPlayer.Movement.IsVisibleChanged -= OnMarkedFirstPlacePlayerIsVisibleChanged;
		}
		if (NetworkmarkedFirstPlacePlayer != null)
		{
			if (NetworkmarkedFirstPlacePlayer.Movement.IsVisible)
			{
				NetworkmarkedFirstPlacePlayer.MarkFirstPlace();
			}
			NetworkmarkedFirstPlacePlayer.Movement.IsVisibleChanged += OnMarkedFirstPlacePlayerIsVisibleChanged;
		}
	}

	private void OnMarkedFirstPlacePlayerIsVisibleChanged()
	{
		if (NetworkmarkedFirstPlacePlayer.Movement.IsVisible)
		{
			NetworkmarkedFirstPlacePlayer.MarkFirstPlace();
		}
		else
		{
			NetworkmarkedFirstPlacePlayer.UnmarkFirstPlace();
		}
	}

	public static bool DoesKnockoutOnPlayerCount(PlayerInfo knockedOutPlayer)
	{
		if (knockedOutPlayer.AsGolfer.IsMatchResolved)
		{
			return false;
		}
		if (knockedOutPlayer.AsSpectator.IsSpectating)
		{
			return false;
		}
		return true;
	}

	public static int GetMatchScore(int scoreIndex)
	{
		if (SingletonBehaviour<DrivingRangeManager>.HasInstance)
		{
			return GameManager.MatchSettings.Scores[0];
		}
		if (scoreIndex >= GameManager.MatchSettings.Scores.Length)
		{
			return GameManager.MatchSettings.Scores[^1];
		}
		return GameManager.MatchSettings.Scores[scoreIndex];
	}

	public static StrokesUnderParType GetStrokesUnderParType(int strokesUnderPar)
	{
		if (strokesUnderPar < 4)
		{
			return strokesUnderPar switch
			{
				0 => StrokesUnderParType.Par, 
				1 => StrokesUnderParType.Birdie, 
				2 => StrokesUnderParType.Eagle, 
				3 => StrokesUnderParType.Albatross, 
				_ => StrokesUnderParType.None, 
			};
		}
		return StrokesUnderParType.Condor;
	}

	public static bool IsLastMatchOfCourse()
	{
		return ServerPersistentCourseData.nextHoleIndex >= GameManager.CurrentCourse.Holes.Length;
	}

	public static string GetPlayerName(PlayerId playerId)
	{
		return GetPlayerName(playerId.Guid);
	}

	public static string GetPlayerName(PlayerState playerState)
	{
		return GetPlayerName(playerState.playerGuid);
	}

	public static string GetPlayerName(ulong guid)
	{
		if (guid == 0L)
		{
			return string.Empty;
		}
		if (clientPlayerNames.TryGetValue(guid, out var value))
		{
			return value;
		}
		if (GetName(out var text))
		{
			clientPlayerNames[guid] = text;
		}
		return text;
		bool GetName(out string name)
		{
			if (SteamEnabler.IsSteamEnabled && guid != 0L)
			{
				if (BNetworkManager.IsSteamLobbyValid())
				{
					if (BNetworkManager.TryGetPlayerInLobby(guid, out var player))
					{
						name = player.Name;
						return true;
					}
					UnityEngine.Debug.LogWarning($"Failed to retrieve player {guid} in lobby, using fallback!");
				}
				try
				{
					name = new Friend(guid).Name;
					return true;
				}
				catch (Exception exception)
				{
					UnityEngine.Debug.LogError("Encountered exception when retrieving steam friend name");
					UnityEngine.Debug.LogException(exception);
					name = guid.ToString();
					return false;
				}
			}
			name = "Player " + guid;
			return true;
		}
	}

	public CourseManager()
	{
		InitSyncObject(playerStates);
		InitSyncObject(playerKnockoutStreaks);
		InitSyncObject(playerPingPerGuid);
		InitSyncObject(playerStateIndicesPerPlayerGuid);
		InitSyncObject(playerDominations);
		InitSyncObject(overtimeActiveBalls);
		_Mirror_SyncVarHookDelegate_currentHoleCourseIndex = OnCurrentHoleCourseIndexChanged;
		_Mirror_SyncVarHookDelegate_currentHoleGlobalIndex = OnCurrentHoleGlobalIndexChanged;
		_Mirror_SyncVarHookDelegate_matchState = OnMatchStateChanged;
		_Mirror_SyncVarHookDelegate_isHoleOverviewFinished = OnIsHoleOverviewFinishedChanged;
		_Mirror_SyncVarHookDelegate_forceDisplayScoreboard = OnForceDisplayScoreboardChanged;
		_Mirror_SyncVarHookDelegate_markedFirstPlacePlayer = OnMarkedFirstPlacePlayerChanged;
	}

	static CourseManager()
	{
		clientPlayerNames = new Dictionary<ulong, string>();
		RemoteProcedureCalls.RegisterCommand(typeof(CourseManager), "System.Void CourseManager::CmdSpawnGolfCartForLocalPlayerInternal(Mirror.NetworkConnectionToClient)", InvokeUserCode_CmdSpawnGolfCartForLocalPlayerInternal__NetworkConnectionToClient, requiresAuthority: false);
		RemoteProcedureCalls.RegisterRpc(typeof(CourseManager), "System.Void CourseManager::RpcInformStartingCourse()", InvokeUserCode_RpcInformStartingCourse);
		RemoteProcedureCalls.RegisterRpc(typeof(CourseManager), "System.Void CourseManager::RpcInformEndingCourse()", InvokeUserCode_RpcInformEndingCourse);
		RemoteProcedureCalls.RegisterRpc(typeof(CourseManager), "System.Void CourseManager::RpcInformStartingNextMatch()", InvokeUserCode_RpcInformStartingNextMatch);
		RemoteProcedureCalls.RegisterRpc(typeof(CourseManager), "System.Void CourseManager::RpcPlayAnnouncerLine(AnnouncerLine)", InvokeUserCode_RpcPlayAnnouncerLine__AnnouncerLine);
		RemoteProcedureCalls.RegisterRpc(typeof(CourseManager), "System.Void CourseManager::RpcSlowDownOverviewCamera(System.Single)", InvokeUserCode_RpcSlowDownOverviewCamera__Single);
		RemoteProcedureCalls.RegisterRpc(typeof(CourseManager), "System.Void CourseManager::RpcPlayAnnouncerLines(Mirror.NetworkConnectionToClient,System.Collections.Generic.List`1<AnnouncerLine>)", InvokeUserCode_RpcPlayAnnouncerLines__NetworkConnectionToClient__List_00601);
		RemoteProcedureCalls.RegisterRpc(typeof(CourseManager), "System.Void CourseManager::RpcAwardCourseBonus(Mirror.NetworkConnectionToClient,System.Single)", InvokeUserCode_RpcAwardCourseBonus__NetworkConnectionToClient__Single);
	}

	[CompilerGenerated]
	private void _003CServerStartNextMatch_003Eg__ResetRedShieldKnockoutStreaks_007C151_1()
	{
		List<PlayerPair> value;
		using (CollectionPool<List<PlayerPair>, PlayerPair>.Get(out value))
		{
			value.AddRange(playerKnockoutStreaks.Keys);
			foreach (PlayerPair item in value)
			{
				if (playerKnockoutStreaks.TryGetValue(item, out var value2) && value2.redShieldStreak < GameManager.MatchSettings.RedShieldKnockoutStreak)
				{
					value2.redShieldStreak = 0;
					playerKnockoutStreaks[item] = value2;
				}
			}
		}
	}

	public override bool Weaved()
	{
		return true;
	}

	protected async void UserCode_RpcInformStartingCourse()
	{
		TutorialManager.CompleteObjective(TutorialObjective.StartMatch);
		if (base.isServer)
		{
			return;
		}
		try
		{
			InputManager.EnableMode(InputMode.ForceDisabled);
			LoadingScreen.Show(Time.timeScale <= 0.25f);
			await UniTask.WaitWhile(() => LoadingScreen.IsFadingScreenIn);
		}
		catch (Exception exception)
		{
			UnityEngine.Debug.LogError("Encountered exception while fading to course start loading screen. See the next log for details");
			UnityEngine.Debug.LogException(exception);
		}
		finally
		{
			InputManager.DisableMode(InputMode.ForceDisabled);
		}
	}

	protected static void InvokeUserCode_RpcInformStartingCourse(NetworkBehaviour obj, NetworkReader reader, NetworkConnectionToClient senderConnection)
	{
		if (!NetworkClient.active)
		{
			UnityEngine.Debug.LogError("RPC RpcInformStartingCourse called on server.");
		}
		else
		{
			((CourseManager)obj).UserCode_RpcInformStartingCourse();
		}
	}

	protected async void UserCode_RpcInformEndingCourse()
	{
		if (base.isServer)
		{
			return;
		}
		try
		{
			InputManager.EnableMode(InputMode.ForceDisabled);
			LoadingScreen.Show(Time.timeScale <= 0.25f);
			await UniTask.WaitWhile(() => LoadingScreen.IsFadingScreenIn);
		}
		catch (Exception exception)
		{
			UnityEngine.Debug.LogError("Encountered exception while fading to course end loading screen. See the next log for details");
			UnityEngine.Debug.LogException(exception);
		}
		finally
		{
			InputManager.DisableMode(InputMode.ForceDisabled);
		}
	}

	protected static void InvokeUserCode_RpcInformEndingCourse(NetworkBehaviour obj, NetworkReader reader, NetworkConnectionToClient senderConnection)
	{
		if (!NetworkClient.active)
		{
			UnityEngine.Debug.LogError("RPC RpcInformEndingCourse called on server.");
		}
		else
		{
			((CourseManager)obj).UserCode_RpcInformEndingCourse();
		}
	}

	protected async void UserCode_RpcInformStartingNextMatch()
	{
		if (base.isServer)
		{
			return;
		}
		try
		{
			InputManager.EnableMode(InputMode.ForceDisabled);
			LoadingScreen.Show(Time.timeScale <= 0.25f);
			await UniTask.WaitWhile(() => LoadingScreen.IsFadingScreenIn);
		}
		catch (Exception exception)
		{
			UnityEngine.Debug.LogError("Encountered exception while fading to next match start loading screen. See the next log for details");
			UnityEngine.Debug.LogException(exception);
		}
		finally
		{
			InputManager.DisableMode(InputMode.ForceDisabled);
		}
	}

	protected static void InvokeUserCode_RpcInformStartingNextMatch(NetworkBehaviour obj, NetworkReader reader, NetworkConnectionToClient senderConnection)
	{
		if (!NetworkClient.active)
		{
			UnityEngine.Debug.LogError("RPC RpcInformStartingNextMatch called on server.");
		}
		else
		{
			((CourseManager)obj).UserCode_RpcInformStartingNextMatch();
		}
	}

	protected void UserCode_RpcPlayAnnouncerLine__AnnouncerLine(AnnouncerLine line)
	{
		PlayAnnouncerLineLocalOnlyInternal(line);
	}

	protected static void InvokeUserCode_RpcPlayAnnouncerLine__AnnouncerLine(NetworkBehaviour obj, NetworkReader reader, NetworkConnectionToClient senderConnection)
	{
		if (!NetworkClient.active)
		{
			UnityEngine.Debug.LogError("RPC RpcPlayAnnouncerLine called on server.");
		}
		else
		{
			((CourseManager)obj).UserCode_RpcPlayAnnouncerLine__AnnouncerLine(GeneratedNetworkCode._Read_AnnouncerLine(reader));
		}
	}

	protected void UserCode_RpcPlayAnnouncerLines__NetworkConnectionToClient__List_00601(NetworkConnectionToClient connection, List<AnnouncerLine> lines)
	{
		foreach (AnnouncerLine line in lines)
		{
			PlayAnnouncerLineLocalOnlyInternal(line);
		}
	}

	protected static void InvokeUserCode_RpcPlayAnnouncerLines__NetworkConnectionToClient__List_00601(NetworkBehaviour obj, NetworkReader reader, NetworkConnectionToClient senderConnection)
	{
		if (!NetworkClient.active)
		{
			UnityEngine.Debug.LogError("TargetRPC RpcPlayAnnouncerLines called on server.");
		}
		else
		{
			((CourseManager)obj).UserCode_RpcPlayAnnouncerLines__NetworkConnectionToClient__List_00601(null, GeneratedNetworkCode._Read_System_002ECollections_002EGeneric_002EList_00601_003CAnnouncerLine_003E(reader));
		}
	}

	protected void UserCode_CmdSpawnGolfCartForLocalPlayerInternal__NetworkConnectionToClient(NetworkConnectionToClient sender)
	{
		if (!serverSpawnGolfCartCommandRateLimiter.RegisterHit(sender))
		{
			return;
		}
		PlayerInfo value;
		if (sender == null)
		{
			value = GameManager.LocalPlayerInfo;
		}
		else
		{
			GameManager.RemotePlayerPerConnectionId.TryGetValue(sender.connectionId, out value);
		}
		if (!(value == null))
		{
			GolfCartInfo golfCartInfo = UnityEngine.Object.Instantiate(GameManager.GolfCartSettings.Prefab, value.transform.position, Quaternion.Euler(0f, value.transform.eulerAngles.y, 0f));
			if (golfCartInfo == null)
			{
				UnityEngine.Debug.LogError("Golf cart did not instantiate properly", base.gameObject);
				return;
			}
			golfCartInfo.ServerReserveDriverSeatPreNetworkSpawn(value);
			NetworkServer.Spawn(golfCartInfo.gameObject);
			golfCartInfo.ServerReserveDriverSeatPostNetworkSpawn();
		}
	}

	protected static void InvokeUserCode_CmdSpawnGolfCartForLocalPlayerInternal__NetworkConnectionToClient(NetworkBehaviour obj, NetworkReader reader, NetworkConnectionToClient senderConnection)
	{
		if (!NetworkServer.active)
		{
			UnityEngine.Debug.LogError("Command CmdSpawnGolfCartForLocalPlayerInternal called on client.");
		}
		else
		{
			((CourseManager)obj).UserCode_CmdSpawnGolfCartForLocalPlayerInternal__NetworkConnectionToClient(senderConnection);
		}
	}

	protected void UserCode_RpcSlowDownOverviewCamera__Single(float duration)
	{
		if (CameraModuleController.CurrentModuleType == CameraModuleType.Overview)
		{
			(CameraModuleController.CurrentModule as OverviewCameraModule).BeginSlowdown(duration);
		}
	}

	protected static void InvokeUserCode_RpcSlowDownOverviewCamera__Single(NetworkBehaviour obj, NetworkReader reader, NetworkConnectionToClient senderConnection)
	{
		if (!NetworkClient.active)
		{
			UnityEngine.Debug.LogError("RPC RpcSlowDownOverviewCamera called on server.");
		}
		else
		{
			((CourseManager)obj).UserCode_RpcSlowDownOverviewCamera__Single(reader.ReadFloat());
		}
	}

	protected void UserCode_RpcAwardCourseBonus__NetworkConnectionToClient__Single(NetworkConnectionToClient target, float awardMultiplier)
	{
		if (!localPlayerRewardedCourseBonus && !(GameManager.LocalPlayerInfo == null) && TryGetPlayerState(GameManager.LocalPlayerInfo, out var state))
		{
			awardMultiplier = BMath.Clamp(awardMultiplier, 0.5f, 1f);
			CosmeticsUnlocksManager.RewardCredits(BMath.RoundToInt((float)state.matchScore * awardMultiplier));
		}
	}

	protected static void InvokeUserCode_RpcAwardCourseBonus__NetworkConnectionToClient__Single(NetworkBehaviour obj, NetworkReader reader, NetworkConnectionToClient senderConnection)
	{
		if (!NetworkClient.active)
		{
			UnityEngine.Debug.LogError("TargetRPC RpcAwardCourseBonus called on server.");
		}
		else
		{
			((CourseManager)obj).UserCode_RpcAwardCourseBonus__NetworkConnectionToClient__Single(null, reader.ReadFloat());
		}
	}

	public override void SerializeSyncVars(NetworkWriter writer, bool forceAll)
	{
		base.SerializeSyncVars(writer, forceAll);
		if (forceAll)
		{
			writer.WriteBool(didAnyPlayerScore);
			writer.WriteVarInt(currentHoleCourseIndex);
			writer.WriteVarInt(currentHoleGlobalIndex);
			GeneratedNetworkCode._Write_MatchState(writer, matchState);
			writer.WriteBool(isHoleOverviewFinished);
			writer.WriteBool(forceDisplayScoreboard);
			writer.WriteNetworkBehaviour(NetworkmarkedFirstPlacePlayer);
			return;
		}
		writer.WriteVarULong(syncVarDirtyBits);
		if ((syncVarDirtyBits & 1L) != 0L)
		{
			writer.WriteBool(didAnyPlayerScore);
		}
		if ((syncVarDirtyBits & 2L) != 0L)
		{
			writer.WriteVarInt(currentHoleCourseIndex);
		}
		if ((syncVarDirtyBits & 4L) != 0L)
		{
			writer.WriteVarInt(currentHoleGlobalIndex);
		}
		if ((syncVarDirtyBits & 8L) != 0L)
		{
			GeneratedNetworkCode._Write_MatchState(writer, matchState);
		}
		if ((syncVarDirtyBits & 0x10L) != 0L)
		{
			writer.WriteBool(isHoleOverviewFinished);
		}
		if ((syncVarDirtyBits & 0x20L) != 0L)
		{
			writer.WriteBool(forceDisplayScoreboard);
		}
		if ((syncVarDirtyBits & 0x40L) != 0L)
		{
			writer.WriteNetworkBehaviour(NetworkmarkedFirstPlacePlayer);
		}
	}

	public override void DeserializeSyncVars(NetworkReader reader, bool initialState)
	{
		base.DeserializeSyncVars(reader, initialState);
		if (initialState)
		{
			GeneratedSyncVarDeserialize(ref didAnyPlayerScore, null, reader.ReadBool());
			GeneratedSyncVarDeserialize(ref currentHoleCourseIndex, _Mirror_SyncVarHookDelegate_currentHoleCourseIndex, reader.ReadVarInt());
			GeneratedSyncVarDeserialize(ref currentHoleGlobalIndex, _Mirror_SyncVarHookDelegate_currentHoleGlobalIndex, reader.ReadVarInt());
			GeneratedSyncVarDeserialize(ref matchState, _Mirror_SyncVarHookDelegate_matchState, GeneratedNetworkCode._Read_MatchState(reader));
			GeneratedSyncVarDeserialize(ref isHoleOverviewFinished, _Mirror_SyncVarHookDelegate_isHoleOverviewFinished, reader.ReadBool());
			GeneratedSyncVarDeserialize(ref forceDisplayScoreboard, _Mirror_SyncVarHookDelegate_forceDisplayScoreboard, reader.ReadBool());
			GeneratedSyncVarDeserialize_NetworkBehaviour(ref markedFirstPlacePlayer, _Mirror_SyncVarHookDelegate_markedFirstPlacePlayer, reader, ref ___markedFirstPlacePlayerNetId);
			return;
		}
		long num = (long)reader.ReadVarULong();
		if ((num & 1L) != 0L)
		{
			GeneratedSyncVarDeserialize(ref didAnyPlayerScore, null, reader.ReadBool());
		}
		if ((num & 2L) != 0L)
		{
			GeneratedSyncVarDeserialize(ref currentHoleCourseIndex, _Mirror_SyncVarHookDelegate_currentHoleCourseIndex, reader.ReadVarInt());
		}
		if ((num & 4L) != 0L)
		{
			GeneratedSyncVarDeserialize(ref currentHoleGlobalIndex, _Mirror_SyncVarHookDelegate_currentHoleGlobalIndex, reader.ReadVarInt());
		}
		if ((num & 8L) != 0L)
		{
			GeneratedSyncVarDeserialize(ref matchState, _Mirror_SyncVarHookDelegate_matchState, GeneratedNetworkCode._Read_MatchState(reader));
		}
		if ((num & 0x10L) != 0L)
		{
			GeneratedSyncVarDeserialize(ref isHoleOverviewFinished, _Mirror_SyncVarHookDelegate_isHoleOverviewFinished, reader.ReadBool());
		}
		if ((num & 0x20L) != 0L)
		{
			GeneratedSyncVarDeserialize(ref forceDisplayScoreboard, _Mirror_SyncVarHookDelegate_forceDisplayScoreboard, reader.ReadBool());
		}
		if ((num & 0x40L) != 0L)
		{
			GeneratedSyncVarDeserialize_NetworkBehaviour(ref markedFirstPlacePlayer, _Mirror_SyncVarHookDelegate_markedFirstPlacePlayer, reader, ref ___markedFirstPlacePlayerNetId);
		}
	}
}
