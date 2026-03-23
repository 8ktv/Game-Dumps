using System;
using System.Collections.Generic;
using Unity.Collections;
using Unity.Jobs;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.Jobs;
using UnityEngine.Splines;

public class BoundsManager : SingletonBehaviour<BoundsManager>
{
	public struct SecondaryOutOfBoundsHazardInstance
	{
		public OutOfBoundsHazard type;

		public int instanceId;

		public float2 worldHorizontalMin;

		public float2 worldHorizontalMax;

		public float worldHeight;

		public SecondaryOutOfBoundsHazardInstance(SecondaryOutOfBoundsHazard hazard)
		{
			type = hazard.Type;
			instanceId = hazard.GetInstanceID();
			Vector2 size = hazard.GetSize();
			worldHorizontalMin = hazard.transform.position.AsHorizontal2() - size / 2f;
			worldHorizontalMax = hazard.transform.position.AsHorizontal2() + size / 2f;
			worldHeight = hazard.transform.position.y;
		}
	}

	public struct BoundsTrackerInstance
	{
		public float3 outOfBoundsHazardSubmersionLocalPosition;

		public float outOfBoundsHazardSubmersionWorldVerticalOffset;

		public LevelBoundsTrackingType levelBoundsTrackingType;

		public float3 worldPosition;

		public float3 outOfBoundsHazardSubmersionWorldPosition;

		public BoundsState boundsState;

		public float outOfBoundsHazardHeight;

		public int secondaryOutOfBoundsHazardInstanceId;

		public bool isOnGreen;

		public bool levelBoundsStateChanged;

		public bool outOfBoundsHazardHeightChanged;

		public bool secondaryOutOfBoundsHazardInstanceIdChanged;

		public bool greenBoundsStateChanged;

		public bool isInitialized;
	}

	public struct TrackerOutOfBoundsHazardState
	{
		public BoundsState hazardState;

		public float hazardHeight;

		public int secondaryHazardInstanceId;

		public TrackerOutOfBoundsHazardState(BoundsState hazardState, float hazardHeight, int secondaryHazardInstanceId)
		{
			this.hazardState = hazardState;
			this.hazardHeight = hazardHeight;
			this.secondaryHazardInstanceId = secondaryHazardInstanceId;
		}
	}

	public struct NearestPointOnCurve
	{
		public float3 point;

		public float distance;

		public float t;
	}

	private const int initializationSecondaryOutOfBoundsHazardCount = 8;

	private const int initializationLevelBoundsTrackersCount = 32;

	private const int initializationLevelBoundsSplineCount = 8;

	private const int initializationLevelBoundsCurveCount = 32;

	private const int initializationGreenBoundsTrackersCount = 16;

	private const int initializationGreenBoundsSplineCount = 8;

	private const int initializationGreenBoundsCurveCount = 32;

	[SerializeField]
	private SplineContainer levelBounds;

	[SerializeField]
	private SplineContainer greenBounds;

	[SerializeField]
	private SplineContainer returnSplines;

	private readonly List<LevelBoundsTracker> levelBoundsTrackers = new List<LevelBoundsTracker>();

	private readonly Dictionary<LevelBoundsTracker, int> levelBoundsTrackerIndices = new Dictionary<LevelBoundsTracker, int>();

	private readonly HashSet<LevelBoundsTracker> levelBoundsTrackerRegistrationBuffer = new HashSet<LevelBoundsTracker>();

	private readonly HashSet<LevelBoundsTracker> levelBoundsTrackerDeregistrationBuffer = new HashSet<LevelBoundsTracker>();

	private readonly Dictionary<LevelBoundsTracker, int> greenBoundsTrackerIndexIndices = new Dictionary<LevelBoundsTracker, int>();

	private readonly List<SecondaryOutOfBoundsHazard> secondaryOutOfBoundsHazards = new List<SecondaryOutOfBoundsHazard>();

	private readonly Dictionary<SecondaryOutOfBoundsHazard, int> secondaryOutOfBoundsHazardsIndices = new Dictionary<SecondaryOutOfBoundsHazard, int>();

	private readonly HashSet<SecondaryOutOfBoundsHazard> secondaryOutOfBoundsHazardRegistrationBuffer = new HashSet<SecondaryOutOfBoundsHazard>();

	private readonly HashSet<SecondaryOutOfBoundsHazard> secondaryOutOfBoundsHazardDeregistrationBuffer = new HashSet<SecondaryOutOfBoundsHazard>();

	private NativeArray<int> levelBoundsCurveStartIndices;

	private NativeArray<BezierCurve> levelBoundsWorldCurves;

	private NativeArray<int> greenBoundsCurveStartIndices;

	private NativeArray<BezierCurve> greenBoundsWorldCurves;

	private NativeArray<BezierCurve> returnSplinesWorldCurves;

	private NativeList<SecondaryOutOfBoundsHazardInstance> secondaryOutOfBoundsHazardInstances;

	private TransformAccessArray levelBoundsTrackerAccessArray;

	private NativeList<BoundsTrackerInstance> levelBoundsTrackerInstances;

	private NativeList<TrackerOutOfBoundsHazardState> trackerOutOfBoundsHazardStates;

	private NativeList<float> levelBoundsTrackerWindingAnglesRad;

	private NativeList<int> levelBoundsTrackerWindingNumbers;

	private NativeList<int> greenBoundsTrackerIndices;

	private NativeList<float> greenBoundsTrackerWindingAnglesRad;

	private NativeList<int> greenBoundsTrackerWindingNumbers;

	private NativeList<int> levelBoundsTrackerWithChangedSecondaryOutOfBoundsHazardInstanceIdIndices;

	private NativeList<int> levelBoundsTrackerWithChangedStateIndices;

	private NativeList<int> levelBoundsTrackerWithChangedOutOfBoundsHazardHeightIndices;

	private NativeList<int> greenBoundsTrackerWithChangedStateIndices;

	private bool isRunningJob;

	private JobHandle currentJob;

	public static NativeList<SecondaryOutOfBoundsHazardInstance> SecondaryOutOfBoundsHazardInstances
	{
		get
		{
			if (!SingletonBehaviour<BoundsManager>.HasInstance)
			{
				return default(NativeList<SecondaryOutOfBoundsHazardInstance>);
			}
			return SingletonBehaviour<BoundsManager>.Instance.secondaryOutOfBoundsHazardInstances;
		}
	}

	public static event Action UpdateFinished;

	protected override void Awake()
	{
		base.Awake();
		secondaryOutOfBoundsHazardInstances = new NativeList<SecondaryOutOfBoundsHazardInstance>(8, Allocator.Persistent);
		levelBoundsTrackerAccessArray = new TransformAccessArray(32);
		levelBoundsTrackerInstances = new NativeList<BoundsTrackerInstance>(32, Allocator.Persistent);
		if (!trackerOutOfBoundsHazardStates.IsCreated)
		{
			trackerOutOfBoundsHazardStates = new NativeList<TrackerOutOfBoundsHazardState>(32, Allocator.Persistent);
		}
		if (!levelBoundsTrackerWindingAnglesRad.IsCreated)
		{
			levelBoundsTrackerWindingAnglesRad = new NativeList<float>(1024, Allocator.Persistent);
		}
		if (!levelBoundsTrackerWindingNumbers.IsCreated)
		{
			levelBoundsTrackerWindingNumbers = new NativeList<int>(256, Allocator.Persistent);
		}
		if (!trackerOutOfBoundsHazardStates.IsCreated)
		{
			trackerOutOfBoundsHazardStates = new NativeList<TrackerOutOfBoundsHazardState>(16, Allocator.Persistent);
		}
		greenBoundsTrackerIndices = new NativeList<int>(16, Allocator.Persistent);
		if (!greenBoundsTrackerWindingAnglesRad.IsCreated)
		{
			greenBoundsTrackerWindingAnglesRad = new NativeList<float>(512, Allocator.Persistent);
		}
		if (!greenBoundsTrackerWindingNumbers.IsCreated)
		{
			greenBoundsTrackerWindingNumbers = new NativeList<int>(128, Allocator.Persistent);
		}
		levelBoundsTrackerWithChangedSecondaryOutOfBoundsHazardInstanceIdIndices = new NativeList<int>(32, Allocator.Persistent);
		levelBoundsTrackerWithChangedStateIndices = new NativeList<int>(32, Allocator.Persistent);
		levelBoundsTrackerWithChangedOutOfBoundsHazardHeightIndices = new NativeList<int>(32, Allocator.Persistent);
		greenBoundsTrackerWithChangedStateIndices = new NativeList<int>(16, Allocator.Persistent);
	}

	private void Start()
	{
		InitializeLevelBounds();
		InitializeGreenBounds();
		InitializeReturnSplines();
	}

	protected override void OnDestroy()
	{
		currentJob.Complete();
		DisposeOfLevelBoundsData();
		DisposeOfReturnSplinesData();
		if (secondaryOutOfBoundsHazardInstances.IsCreated)
		{
			secondaryOutOfBoundsHazardInstances.Dispose();
		}
		if (levelBoundsTrackerAccessArray.isCreated)
		{
			levelBoundsTrackerAccessArray.Dispose();
		}
		if (levelBoundsTrackerInstances.IsCreated)
		{
			levelBoundsTrackerInstances.Dispose();
		}
		if (trackerOutOfBoundsHazardStates.IsCreated)
		{
			trackerOutOfBoundsHazardStates.Dispose();
		}
		if (levelBoundsTrackerWindingAnglesRad.IsCreated)
		{
			levelBoundsTrackerWindingAnglesRad.Dispose();
		}
		if (levelBoundsTrackerWindingNumbers.IsCreated)
		{
			levelBoundsTrackerWindingNumbers.Dispose();
		}
		if (greenBoundsTrackerIndices.IsCreated)
		{
			greenBoundsTrackerIndices.Dispose();
		}
		if (greenBoundsTrackerWindingAnglesRad.IsCreated)
		{
			greenBoundsTrackerWindingAnglesRad.Dispose();
		}
		if (greenBoundsTrackerWindingNumbers.IsCreated)
		{
			greenBoundsTrackerWindingNumbers.Dispose();
		}
		if (levelBoundsTrackerWithChangedSecondaryOutOfBoundsHazardInstanceIdIndices.IsCreated)
		{
			levelBoundsTrackerWithChangedSecondaryOutOfBoundsHazardInstanceIdIndices.Dispose();
		}
		if (levelBoundsTrackerWithChangedStateIndices.IsCreated)
		{
			levelBoundsTrackerWithChangedStateIndices.Dispose();
		}
		if (levelBoundsTrackerWithChangedOutOfBoundsHazardHeightIndices.IsCreated)
		{
			levelBoundsTrackerWithChangedOutOfBoundsHazardHeightIndices.Dispose();
		}
		if (greenBoundsTrackerWithChangedStateIndices.IsCreated)
		{
			greenBoundsTrackerWithChangedStateIndices.Dispose();
		}
		if (greenBoundsWorldCurves.IsCreated)
		{
			greenBoundsWorldCurves.Dispose();
		}
		if (greenBoundsCurveStartIndices.IsCreated)
		{
			greenBoundsCurveStartIndices.Dispose();
		}
		base.OnDestroy();
	}

	public static void RegisterSecondaryOutOfBoundsHazard(SecondaryOutOfBoundsHazard hazard)
	{
		if (SingletonBehaviour<BoundsManager>.HasInstance)
		{
			SingletonBehaviour<BoundsManager>.Instance.RegisterSecondaryOutOfBoundsHazardInternal(hazard);
		}
	}

	public static void DeregisterSecondaryOutOfBoundsHazard(SecondaryOutOfBoundsHazard hazard)
	{
		if (SingletonBehaviour<BoundsManager>.HasInstance)
		{
			SingletonBehaviour<BoundsManager>.Instance.DeregisterSecondaryOutOfBoundsHazardInternal(hazard);
		}
	}

	public static void RegisterLevelBoundsTracker(LevelBoundsTracker tracker)
	{
		if (SingletonBehaviour<BoundsManager>.HasInstance)
		{
			SingletonBehaviour<BoundsManager>.Instance.RegisterLevelBoundsTrackerInternal(tracker);
		}
	}

	public static void DeregisterLevelBoundsTracker(LevelBoundsTracker tracker)
	{
		if (SingletonBehaviour<BoundsManager>.HasInstance)
		{
			SingletonBehaviour<BoundsManager>.Instance.DeregisterLevelBoundsTrackerInternal(tracker);
		}
	}

	public static void SetLevelBoundsState(LevelBoundsTracker tracker, BoundsState state)
	{
		if (SingletonBehaviour<BoundsManager>.HasInstance)
		{
			SingletonBehaviour<BoundsManager>.Instance.SetLevelBoundsStateInternal(tracker, state);
		}
	}

	public static Vector3 GetNearestPointOnReturnSplines(Vector3 worldPoint, out Vector3 directionIntoLevel)
	{
		if (!SingletonBehaviour<BoundsManager>.HasInstance)
		{
			directionIntoLevel = default(Vector3);
			return default(Vector3);
		}
		return SingletonBehaviour<BoundsManager>.Instance.GetNearestPointOnReturnSplinesInternal(worldPoint, out directionIntoLevel);
	}

	public static bool IsPointInLevelBoundsImmediate(Vector3 worldPoint)
	{
		if (!SingletonBehaviour<BoundsManager>.HasInstance)
		{
			return false;
		}
		return SingletonBehaviour<BoundsManager>.Instance.IsPointInLevelBoundsImmediateInternal(worldPoint);
	}

	public static bool IsPointInGreenBoundsImmediate(Vector3 worldPoint)
	{
		if (!SingletonBehaviour<BoundsManager>.HasInstance)
		{
			return false;
		}
		return SingletonBehaviour<BoundsManager>.Instance.IsPointInGreenBoundsImmediateInternal(worldPoint);
	}

	private void InitializeLevelBounds()
	{
		if (levelBounds == null)
		{
			Debug.LogError("Level bounds have not been set on the bounds manager", base.gameObject);
			return;
		}
		if (levelBoundsCurveStartIndices.IsCreated)
		{
			levelBoundsCurveStartIndices.Dispose();
		}
		levelBoundsCurveStartIndices = new NativeArray<int>(levelBounds.Splines.Count, Allocator.Persistent, NativeArrayOptions.UninitializedMemory);
		int num = levelBounds.Splines.Count * levelBoundsTrackers.Count;
		if (!levelBoundsTrackerWindingNumbers.IsCreated)
		{
			levelBoundsTrackerWindingNumbers = new NativeList<int>(num, Allocator.Persistent);
		}
		levelBoundsTrackerWindingNumbers.AddReplicate(0, num - levelBoundsTrackerWindingNumbers.Length);
		int num2 = 0;
		int num3 = 0;
		for (int i = 0; i < levelBounds.Splines.Count; i++)
		{
			int curveCount = levelBounds.Splines[i].GetCurveCount();
			num2 += curveCount;
			levelBoundsCurveStartIndices[i] = num3;
			num3 += curveCount;
		}
		if (levelBoundsWorldCurves.IsCreated)
		{
			levelBoundsWorldCurves.Dispose();
		}
		levelBoundsWorldCurves = new NativeArray<BezierCurve>(num2, Allocator.Persistent, NativeArrayOptions.UninitializedMemory);
		if (!trackerOutOfBoundsHazardStates.IsCreated)
		{
			trackerOutOfBoundsHazardStates = new NativeList<TrackerOutOfBoundsHazardState>(levelBoundsTrackers.Count, Allocator.Persistent);
		}
		trackerOutOfBoundsHazardStates.AddReplicate(default(TrackerOutOfBoundsHazardState), levelBoundsTrackers.Count - trackerOutOfBoundsHazardStates.Length);
		int num4 = num2 * levelBoundsTrackers.Count;
		if (!levelBoundsTrackerWindingAnglesRad.IsCreated)
		{
			levelBoundsTrackerWindingAnglesRad = new NativeList<float>(num4, Allocator.Persistent);
		}
		levelBoundsTrackerWindingAnglesRad.AddReplicate(0f, num4 - levelBoundsTrackerWindingAnglesRad.Length);
		int num5 = 0;
		foreach (Spline spline in levelBounds.Splines)
		{
			for (int j = 0; j < spline.GetCurveCount(); j++)
			{
				levelBoundsWorldCurves[num5++] = spline.GetCurve(j).Transform(levelBounds.transform.localToWorldMatrix);
			}
		}
	}

	private void InitializeGreenBounds()
	{
		if (greenBounds == null)
		{
			Debug.LogError("Green bounds have not been set on the bounds manager", base.gameObject);
			return;
		}
		if (greenBoundsCurveStartIndices.IsCreated)
		{
			greenBoundsCurveStartIndices.Dispose();
		}
		greenBoundsCurveStartIndices = new NativeArray<int>(greenBounds.Splines.Count, Allocator.Persistent, NativeArrayOptions.UninitializedMemory);
		int num = greenBounds.Splines.Count * greenBoundsTrackerIndices.Length;
		if (!greenBoundsTrackerWindingNumbers.IsCreated)
		{
			greenBoundsTrackerWindingNumbers = new NativeList<int>(num, Allocator.Persistent);
		}
		greenBoundsTrackerWindingNumbers.AddReplicate(0, num - greenBoundsTrackerWindingNumbers.Length);
		int num2 = 0;
		int num3 = 0;
		for (int i = 0; i < greenBounds.Splines.Count; i++)
		{
			int curveCount = greenBounds.Splines[i].GetCurveCount();
			num2 += curveCount;
			greenBoundsCurveStartIndices[i] = num3;
			num3 += curveCount;
		}
		if (greenBoundsWorldCurves.IsCreated)
		{
			greenBoundsWorldCurves.Dispose();
		}
		greenBoundsWorldCurves = new NativeArray<BezierCurve>(num2, Allocator.Persistent, NativeArrayOptions.UninitializedMemory);
		int num4 = num2 * greenBoundsTrackerIndices.Length;
		if (!greenBoundsTrackerWindingAnglesRad.IsCreated)
		{
			greenBoundsTrackerWindingAnglesRad = new NativeList<float>(num4, Allocator.Persistent);
		}
		greenBoundsTrackerWindingAnglesRad.AddReplicate(0f, num4 - greenBoundsTrackerWindingAnglesRad.Length);
		int num5 = 0;
		foreach (Spline spline in greenBounds.Splines)
		{
			for (int j = 0; j < spline.GetCurveCount(); j++)
			{
				greenBoundsWorldCurves[num5++] = spline.GetCurve(j).Transform(greenBounds.transform.localToWorldMatrix);
			}
		}
	}

	private void InitializeReturnSplines()
	{
		if (returnSplines == null)
		{
			Debug.LogError("Return splines have not been set on the bounds manager", base.gameObject);
			return;
		}
		int num = 0;
		for (int i = 0; i < returnSplines.Splines.Count; i++)
		{
			num += returnSplines.Splines[i].GetCurveCount();
		}
		if (returnSplinesWorldCurves.IsCreated)
		{
			returnSplinesWorldCurves.Dispose();
		}
		returnSplinesWorldCurves = new NativeArray<BezierCurve>(num, Allocator.Persistent, NativeArrayOptions.UninitializedMemory);
		int num2 = 0;
		foreach (Spline spline in returnSplines.Splines)
		{
			for (int j = 0; j < spline.GetCurveCount(); j++)
			{
				returnSplinesWorldCurves[num2++] = spline.GetCurve(j).Transform(returnSplines.transform.localToWorldMatrix);
			}
		}
	}

	private void RegisterSecondaryOutOfBoundsHazardInternal(SecondaryOutOfBoundsHazard hazard)
	{
		if (!(hazard == null))
		{
			if (isRunningJob)
			{
				secondaryOutOfBoundsHazardRegistrationBuffer.Add(hazard);
				secondaryOutOfBoundsHazardDeregistrationBuffer.Remove(hazard);
			}
			else if (secondaryOutOfBoundsHazardsIndices.TryAdd(hazard, levelBoundsTrackers.Count))
			{
				secondaryOutOfBoundsHazards.Add(hazard);
				secondaryOutOfBoundsHazardInstances.Add(new SecondaryOutOfBoundsHazardInstance(hazard));
			}
		}
	}

	private void DeregisterSecondaryOutOfBoundsHazardInternal(SecondaryOutOfBoundsHazard hazard)
	{
		if (!BNetworkManager.IsChangingSceneOrShuttingDown)
		{
			int value;
			if (isRunningJob)
			{
				secondaryOutOfBoundsHazardRegistrationBuffer.Remove(hazard);
				secondaryOutOfBoundsHazardDeregistrationBuffer.Add(hazard);
			}
			else if (secondaryOutOfBoundsHazardsIndices.TryGetValue(hazard, out value))
			{
				Dictionary<SecondaryOutOfBoundsHazard, int> dictionary = secondaryOutOfBoundsHazardsIndices;
				List<SecondaryOutOfBoundsHazard> list = secondaryOutOfBoundsHazards;
				dictionary[list[list.Count - 1]] = value;
				secondaryOutOfBoundsHazardsIndices.Remove(hazard);
				secondaryOutOfBoundsHazards.RemoveAtSwapBack(value);
				secondaryOutOfBoundsHazardInstances.RemoveAtSwapBack(value);
			}
		}
	}

	private void RegisterLevelBoundsTrackerInternal(LevelBoundsTracker tracker)
	{
		if (tracker == null)
		{
			return;
		}
		if (isRunningJob)
		{
			levelBoundsTrackerRegistrationBuffer.Add(tracker);
			levelBoundsTrackerDeregistrationBuffer.Remove(tracker);
			return;
		}
		int value = levelBoundsTrackers.Count;
		if (!levelBoundsTrackerIndices.TryAdd(tracker, value))
		{
			return;
		}
		levelBoundsTrackers.Add(tracker);
		levelBoundsTrackerAccessArray.Add(tracker.transform);
		levelBoundsTrackerInstances.Add(GetNewTrackerInstance(tracker));
		if (levelBounds != null)
		{
			if (trackerOutOfBoundsHazardStates.Length < levelBoundsTrackers.Count)
			{
				trackerOutOfBoundsHazardStates.Add(default(TrackerOutOfBoundsHazardState));
			}
			if (levelBoundsTrackerWindingAnglesRad.Length < levelBoundsTrackers.Count * levelBoundsWorldCurves.Length)
			{
				levelBoundsTrackerWindingAnglesRad.AddReplicate(0f, levelBoundsWorldCurves.Length);
			}
			if (levelBoundsTrackerWindingNumbers.Length < levelBoundsTrackers.Count * levelBounds.Splines.Count)
			{
				levelBoundsTrackerWindingNumbers.AddReplicate(0, levelBounds.Splines.Count);
			}
		}
		if (!tracker.Settings.TrackingType.HasType(LevelBoundsTrackingType.Green) || !greenBoundsTrackerIndexIndices.TryAdd(tracker, greenBoundsTrackerIndices.Length))
		{
			return;
		}
		greenBoundsTrackerIndices.Add(in value);
		if (greenBounds != null)
		{
			if (greenBoundsTrackerWindingAnglesRad.Length < greenBoundsTrackerIndices.Length * greenBoundsWorldCurves.Length)
			{
				greenBoundsTrackerWindingAnglesRad.AddReplicate(0f, greenBoundsWorldCurves.Length);
			}
			if (greenBoundsTrackerWindingNumbers.Length < greenBoundsTrackerIndices.Length * greenBounds.Splines.Count)
			{
				greenBoundsTrackerWindingNumbers.AddReplicate(0, greenBounds.Splines.Count);
			}
		}
		static BoundsTrackerInstance GetNewTrackerInstance(LevelBoundsTracker levelBoundsTracker)
		{
			return new BoundsTrackerInstance
			{
				outOfBoundsHazardSubmersionLocalPosition = levelBoundsTracker.Settings.OutOfBoundsHazardSubmersionLocalPoint,
				outOfBoundsHazardSubmersionWorldVerticalOffset = levelBoundsTracker.Settings.OutOfBoundsHazardSubmersionWorldVerticalOffset,
				levelBoundsTrackingType = levelBoundsTracker.Settings.TrackingType
			};
		}
	}

	private void DeregisterLevelBoundsTrackerInternal(LevelBoundsTracker tracker)
	{
		if (BNetworkManager.IsChangingSceneOrShuttingDown)
		{
			return;
		}
		int value;
		if (isRunningJob)
		{
			levelBoundsTrackerRegistrationBuffer.Remove(tracker);
			levelBoundsTrackerDeregistrationBuffer.Add(tracker);
		}
		else if (levelBoundsTrackerIndices.TryGetValue(tracker, out value))
		{
			if (tracker.Settings.TrackingType.HasType(LevelBoundsTrackingType.Green) && greenBoundsTrackerIndexIndices.TryGetValue(tracker, out var value2))
			{
				Dictionary<LevelBoundsTracker, int> dictionary = greenBoundsTrackerIndexIndices;
				List<LevelBoundsTracker> list = levelBoundsTrackers;
				ref NativeList<int> reference = ref greenBoundsTrackerIndices;
				dictionary[list[reference[reference.Length - 1]]] = value2;
				greenBoundsTrackerIndexIndices.Remove(tracker);
				greenBoundsTrackerIndices.RemoveAtSwapBack(value2);
			}
			List<LevelBoundsTracker> list2 = levelBoundsTrackers;
			LevelBoundsTracker key = list2[list2.Count - 1];
			levelBoundsTrackerIndices[key] = value;
			levelBoundsTrackerIndices.Remove(tracker);
			levelBoundsTrackers.RemoveAtSwapBack(value);
			levelBoundsTrackerAccessArray.RemoveAtSwapBack(value);
			levelBoundsTrackerInstances.RemoveAtSwapBack(value);
			if (greenBoundsTrackerIndexIndices.TryGetValue(key, out var value3))
			{
				greenBoundsTrackerIndices[value3] = value;
			}
		}
	}

	private void SetLevelBoundsStateInternal(LevelBoundsTracker tracker, BoundsState state)
	{
		if (levelBoundsTrackerIndices.TryGetValue(tracker, out var value))
		{
			if (isRunningJob)
			{
				currentJob.Complete();
			}
			BoundsTrackerInstance value2 = levelBoundsTrackerInstances[value];
			value2.boundsState = state;
			levelBoundsTrackerInstances[value] = value2;
		}
	}

	private Vector3 GetNearestPointOnReturnSplinesInternal(Vector3 worldPoint, out Vector3 directionIntoLevel)
	{
		if (!returnSplinesWorldCurves.IsCreated)
		{
			directionIntoLevel = default(Vector3);
			return default(Vector3);
		}
		int length = returnSplinesWorldCurves.Length;
		NativeArray<NearestPointOnCurve> nearestPointsPerCurve = new NativeArray<NearestPointOnCurve>(length, Allocator.TempJob, NativeArrayOptions.UninitializedMemory);
		NativeReference<int> nearestCurveIndex = new NativeReference<int>(Allocator.TempJob, NativeArrayOptions.UninitializedMemory);
		NativeArray<NearestPointOnCurve> nearestPointsPerSegment = new NativeArray<NearestPointOnCurve>(32, Allocator.TempJob, NativeArrayOptions.UninitializedMemory);
		try
		{
			FindApproximateNearestPointOnCurvesJob jobData = new FindApproximateNearestPointOnCurvesJob
			{
				curves = returnSplinesWorldCurves,
				nearestPointsPerCurve = nearestPointsPerCurve,
				approximateSpatialResolution = 4f,
				maxSubdivisionCount = 100,
				point = worldPoint
			};
			FindNearestCurveJob jobData2 = new FindNearestCurveJob
			{
				nearestPointsPerCurve = nearestPointsPerCurve,
				nearestCurveIndex = nearestCurveIndex
			};
			FindApproximateNearestPointsOnCurveSegmentsJob jobData3 = new FindApproximateNearestPointsOnCurveSegmentsJob
			{
				curves = returnSplinesWorldCurves,
				nearestCurveIndex = nearestCurveIndex,
				nearestPointsPerSegment = nearestPointsPerSegment,
				approximateSpatialResolution = 0.1f,
				maxSubdivisionCount = 100,
				segmentCount = 32,
				point = worldPoint
			};
			FindNearestCurveSegmentPointJob jobData4 = new FindNearestCurveSegmentPointJob
			{
				nearestPointsPerSegment = nearestPointsPerSegment
			};
			JobHandle dependsOn = IJobParallelForExtensions.Schedule(jobData, length, 1);
			JobHandle dependsOn2 = jobData2.Schedule(dependsOn);
			JobHandle dependsOn3 = IJobParallelForExtensions.Schedule(jobData3, 32, 1, dependsOn2);
			jobData4.Schedule(dependsOn3).Complete();
			Vector3 vector = CurveUtility.EvaluateTangent(returnSplinesWorldCurves[nearestCurveIndex.Value], nearestPointsPerSegment[0].t);
			directionIntoLevel = Quaternion.Euler(0f, 90f, 0f) * vector.Horizontalized().normalized;
			return nearestPointsPerSegment[0].point;
		}
		finally
		{
			nearestPointsPerCurve.Dispose();
			nearestCurveIndex.Dispose();
			nearestPointsPerSegment.Dispose();
		}
	}

	private bool IsPointInLevelBoundsImmediateInternal(Vector3 worldPoint)
	{
		if (levelBounds == null)
		{
			return false;
		}
		NativeArray<float> windingAnglesRad = new NativeArray<float>(levelBoundsWorldCurves.Length, Allocator.TempJob, NativeArrayOptions.UninitializedMemory);
		NativeReference<bool> isPointInside = new NativeReference<bool>(Allocator.TempJob, NativeArrayOptions.UninitializedMemory);
		try
		{
			GetBezierWindingAnglesForSinglePointJob jobData = new GetBezierWindingAnglesForSinglePointJob
			{
				curves = levelBoundsWorldCurves,
				windingAnglesRad = windingAnglesRad,
				point = worldPoint
			};
			ProcessSinglePointWindingAnglesJob jobData2 = new ProcessSinglePointWindingAnglesJob
			{
				windingAnglesRad = windingAnglesRad,
				isPointInside = isPointInside
			};
			JobHandle dependsOn = IJobParallelForExtensions.Schedule(jobData, levelBoundsWorldCurves.Length, 1);
			jobData2.Schedule(dependsOn).Complete();
			return isPointInside.Value;
		}
		finally
		{
			if (windingAnglesRad.IsCreated)
			{
				windingAnglesRad.Dispose();
			}
			if (isPointInside.IsCreated)
			{
				isPointInside.Dispose();
			}
		}
	}

	private bool IsPointInGreenBoundsImmediateInternal(Vector3 worldPoint)
	{
		if (greenBounds == null)
		{
			return false;
		}
		NativeArray<float> windingAnglesRad = new NativeArray<float>(greenBoundsWorldCurves.Length, Allocator.TempJob, NativeArrayOptions.UninitializedMemory);
		NativeReference<bool> isPointInside = new NativeReference<bool>(Allocator.TempJob, NativeArrayOptions.UninitializedMemory);
		try
		{
			GetBezierWindingAnglesForSinglePointJob jobData = new GetBezierWindingAnglesForSinglePointJob
			{
				curves = greenBoundsWorldCurves,
				windingAnglesRad = windingAnglesRad,
				point = worldPoint
			};
			ProcessSinglePointWindingAnglesJob jobData2 = new ProcessSinglePointWindingAnglesJob
			{
				windingAnglesRad = windingAnglesRad,
				isPointInside = isPointInside
			};
			JobHandle dependsOn = IJobParallelForExtensions.Schedule(jobData, greenBoundsWorldCurves.Length, 1);
			jobData2.Schedule(dependsOn).Complete();
			return isPointInside.Value;
		}
		finally
		{
			if (windingAnglesRad.IsCreated)
			{
				windingAnglesRad.Dispose();
			}
			if (isPointInside.IsCreated)
			{
				isPointInside.Dispose();
			}
		}
	}

	private void Update()
	{
		if (!currentJob.IsCompleted)
		{
			return;
		}
		if (isRunningJob)
		{
			isRunningJob = false;
			ProcessJobResults();
			FlushRegistrationBuffers();
			try
			{
				BoundsManager.UpdateFinished?.Invoke();
			}
			catch (Exception exception)
			{
				Debug.LogError("Encountered exception while invoking BoundsManager's UpdateFinished event. See the next log for details", base.gameObject);
				Debug.LogException(exception, base.gameObject);
			}
		}
		if (levelBoundsTrackers.Count > 0)
		{
			int length = levelBoundsCurveStartIndices.Length;
			int length2 = levelBoundsWorldCurves.Length;
			int length3 = levelBoundsTrackerInstances.Length;
			int length4 = greenBoundsCurveStartIndices.Length;
			int length5 = greenBoundsWorldCurves.Length;
			int length6 = greenBoundsTrackerIndices.Length;
			PrepareBoundsStateUpdateJob jobData = new PrepareBoundsStateUpdateJob
			{
				levelBoundsTrackerWithChangedStateIndices = levelBoundsTrackerWithChangedStateIndices,
				levelBoundsTrackerWithChangedSecondaryOutOfBoundsHazardInstanceIdIndices = levelBoundsTrackerWithChangedSecondaryOutOfBoundsHazardInstanceIdIndices,
				levelBoundsTrackerWithChangedOutOfBoundsHazardHeightIndices = levelBoundsTrackerWithChangedOutOfBoundsHazardHeightIndices,
				greenBoundsTrackerWithChangedStateIndices = greenBoundsTrackerWithChangedStateIndices
			};
			UpdateLevelBoundTrackerTransformsJob jobData2 = new UpdateLevelBoundTrackerTransformsJob
			{
				levelBoundsTrackers = levelBoundsTrackerInstances
			};
			UpdateOutOfBoundsHazardStateJob jobData3 = new UpdateOutOfBoundsHazardStateJob
			{
				levelBoundsTrackers = levelBoundsTrackerInstances,
				secondaryOutOfBoundsHazards = secondaryOutOfBoundsHazardInstances,
				trackerOutOfBoundsHazardStates = trackerOutOfBoundsHazardStates,
				mainOutOfBoundsHazardHeight = MainOutOfBoundsHazard.Height
			};
			GetBezierWindingAnglesJob jobData4 = new GetBezierWindingAnglesJob
			{
				curves = levelBoundsWorldCurves,
				boundsTrackers = levelBoundsTrackerInstances,
				windingAnglesRad = levelBoundsTrackerWindingAnglesRad
			};
			GetWindingNumbersFromAnglesJob jobData5 = new GetWindingNumbersFromAnglesJob
			{
				windingAnglesRad = levelBoundsTrackerWindingAnglesRad,
				splineCurveStartIndices = levelBoundsCurveStartIndices,
				windingNumbers = levelBoundsTrackerWindingNumbers,
				splineCount = levelBoundsCurveStartIndices.Length,
				curveCount = levelBoundsWorldCurves.Length
			};
			GetBezierWindingAnglesFilteredJob jobData6 = new GetBezierWindingAnglesFilteredJob
			{
				curves = greenBoundsWorldCurves,
				boundsTrackers = levelBoundsTrackerInstances,
				boundsTrackerIndices = greenBoundsTrackerIndices,
				windingAnglesRad = greenBoundsTrackerWindingAnglesRad
			};
			GetWindingNumbersFromAnglesJob jobData7 = new GetWindingNumbersFromAnglesJob
			{
				windingAnglesRad = greenBoundsTrackerWindingAnglesRad,
				splineCurveStartIndices = greenBoundsCurveStartIndices,
				windingNumbers = greenBoundsTrackerWindingNumbers,
				splineCount = greenBoundsCurveStartIndices.Length,
				curveCount = greenBoundsWorldCurves.Length
			};
			ProcessGreenBoundsUpdateResultsJob jobData8 = new ProcessGreenBoundsUpdateResultsJob
			{
				greenBoundsWindingNumbers = greenBoundsTrackerWindingNumbers,
				greenBoundsTrackerIndices = greenBoundsTrackerIndices,
				levelBoundsTrackers = levelBoundsTrackerInstances,
				greenBoundsSplineCount = greenBoundsCurveStartIndices.Length
			};
			ProcessLevelBoundsUpdateResultsJob jobData9 = new ProcessLevelBoundsUpdateResultsJob
			{
				levelBoundsWindingNumbers = levelBoundsTrackerWindingNumbers,
				trackerOutOfBoundsHazardStates = trackerOutOfBoundsHazardStates,
				levelBoundsTrackers = levelBoundsTrackerInstances,
				levelBoundsSplineCount = levelBoundsCurveStartIndices.Length
			};
			ProcessBoundsCheckResultsJob jobData10 = new ProcessBoundsCheckResultsJob
			{
				levelBoundsTrackers = levelBoundsTrackerInstances,
				levelBoundsTrackerWithChangedSecondaryOutOfBoundsHazardInstanceIdIndices = levelBoundsTrackerWithChangedSecondaryOutOfBoundsHazardInstanceIdIndices,
				levelBoundsTrackerWithChangedStateIndices = levelBoundsTrackerWithChangedStateIndices,
				levelBoundsTrackerWithChangedOutOfBoundsHazardHeightIndices = levelBoundsTrackerWithChangedOutOfBoundsHazardHeightIndices,
				greenBoundsTrackerWithChangedStateIndices = greenBoundsTrackerWithChangedStateIndices
			};
			JobHandle dependsOn = IJobParallelForTransformExtensions.ScheduleReadOnlyByRef(dependsOn: jobData.Schedule(), jobData: ref jobData2, transforms: levelBoundsTrackerAccessArray, batchSize: 1);
			JobHandle job = IJobParallelForExtensions.Schedule(jobData3, length3, 1, dependsOn);
			JobHandle dependsOn2 = IJobParallelForExtensions.Schedule(jobData4, length2 * length3, 1, dependsOn);
			JobHandle job2 = IJobParallelForExtensions.Schedule(jobData5, length * length3, 1, dependsOn2);
			JobHandle dependsOn3 = IJobParallelForExtensions.Schedule(jobData6, length5 * length6, 1, dependsOn);
			JobHandle job3 = IJobParallelForExtensions.Schedule(jobData7, length4 * length6, 1, dependsOn3);
			JobHandle dependsOn4 = JobHandle.CombineDependencies(job2, job, job3);
			JobHandle dependsOn5 = IJobParallelForExtensions.Schedule(jobData8, length6, 1, dependsOn4);
			JobHandle dependsOn6 = IJobParallelForExtensions.Schedule(jobData9, length3, 1, dependsOn5);
			currentJob = jobData10.Schedule(dependsOn6);
			isRunningJob = true;
		}
	}

	private void ProcessJobResults()
	{
		currentJob.Complete();
		foreach (int levelBoundsTrackerWithChangedSecondaryOutOfBoundsHazardInstanceIdIndex in levelBoundsTrackerWithChangedSecondaryOutOfBoundsHazardInstanceIdIndices)
		{
			levelBoundsTrackers[levelBoundsTrackerWithChangedSecondaryOutOfBoundsHazardInstanceIdIndex].InformSecondaryOutOfBoundsHazardChanged(levelBoundsTrackerInstances[levelBoundsTrackerWithChangedSecondaryOutOfBoundsHazardInstanceIdIndex].secondaryOutOfBoundsHazardInstanceId);
		}
		foreach (int levelBoundsTrackerWithChangedStateIndex in levelBoundsTrackerWithChangedStateIndices)
		{
			levelBoundsTrackers[levelBoundsTrackerWithChangedStateIndex].InformLevelBoundsStateChanged(levelBoundsTrackerInstances[levelBoundsTrackerWithChangedStateIndex].boundsState);
		}
		foreach (int levelBoundsTrackerWithChangedOutOfBoundsHazardHeightIndex in levelBoundsTrackerWithChangedOutOfBoundsHazardHeightIndices)
		{
			levelBoundsTrackers[levelBoundsTrackerWithChangedOutOfBoundsHazardHeightIndex].InformOutOfBoundsHazardHeightChanged(levelBoundsTrackerInstances[levelBoundsTrackerWithChangedOutOfBoundsHazardHeightIndex].outOfBoundsHazardHeight);
		}
		foreach (int greenBoundsTrackerWithChangedStateIndex in greenBoundsTrackerWithChangedStateIndices)
		{
			levelBoundsTrackers[greenBoundsTrackerWithChangedStateIndex].InformGreenBoundsStateChanged(levelBoundsTrackerInstances[greenBoundsTrackerWithChangedStateIndex].isOnGreen);
		}
	}

	private void FlushRegistrationBuffers()
	{
		foreach (LevelBoundsTracker item in levelBoundsTrackerRegistrationBuffer)
		{
			RegisterLevelBoundsTrackerInternal(item);
		}
		foreach (LevelBoundsTracker item2 in levelBoundsTrackerDeregistrationBuffer)
		{
			DeregisterLevelBoundsTrackerInternal(item2);
		}
		foreach (SecondaryOutOfBoundsHazard item3 in secondaryOutOfBoundsHazardRegistrationBuffer)
		{
			RegisterSecondaryOutOfBoundsHazardInternal(item3);
		}
		foreach (SecondaryOutOfBoundsHazard item4 in secondaryOutOfBoundsHazardDeregistrationBuffer)
		{
			DeregisterSecondaryOutOfBoundsHazardInternal(item4);
		}
		levelBoundsTrackerRegistrationBuffer.Clear();
		levelBoundsTrackerDeregistrationBuffer.Clear();
	}

	private void DisposeOfLevelBoundsData()
	{
		if (levelBoundsCurveStartIndices.IsCreated)
		{
			levelBoundsCurveStartIndices.Dispose();
		}
		if (levelBoundsWorldCurves.IsCreated)
		{
			levelBoundsWorldCurves.Dispose();
		}
	}

	private void DisposeOfReturnSplinesData()
	{
		if (returnSplinesWorldCurves.IsCreated)
		{
			returnSplinesWorldCurves.Dispose();
		}
	}
}
