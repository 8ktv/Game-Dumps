using System;
using Brimstone.BallDistanceJobs;
using Unity.Jobs;
using UnityEngine;
using UnityEngine.Jobs;

[Unity.Jobs.DOTSCompilerGenerated]
internal class __JobReflectionRegistrationOutput__15695277335665486751
{
	public static void CreateJobReflectionData()
	{
		try
		{
			IJobParallelForTransformExtensions.EarlyJobInit<UpdateLandmineTransformsJob>();
			IJobParallelForExtensions.EarlyJobInit<SetUpLandmineOverlapChecksJob>();
			IJobParallelForExtensions.EarlyJobInit<FindApproximateNearestPointOnCurvesJob>();
			IJobExtensions.EarlyJobInit<FindNearestCurveJob>();
			IJobParallelForExtensions.EarlyJobInit<FindApproximateNearestPointsOnCurveSegmentsJob>();
			IJobExtensions.EarlyJobInit<FindNearestCurveSegmentPointJob>();
			IJobParallelForExtensions.EarlyJobInit<GetBezierWindingAnglesForSinglePointJob>();
			IJobExtensions.EarlyJobInit<ProcessSinglePointWindingAnglesJob>();
			IJobExtensions.EarlyJobInit<PrepareBoundsStateUpdateJob>();
			IJobParallelForTransformExtensions.EarlyJobInit<UpdateLevelBoundTrackerTransformsJob>();
			IJobParallelForExtensions.EarlyJobInit<UpdateOutOfBoundsHazardStateJob>();
			IJobParallelForExtensions.EarlyJobInit<GetBezierWindingAnglesJob>();
			IJobParallelForExtensions.EarlyJobInit<GetBezierWindingAnglesFilteredJob>();
			IJobParallelForExtensions.EarlyJobInit<GetWindingNumbersFromAnglesJob>();
			IJobParallelForExtensions.EarlyJobInit<ProcessGreenBoundsUpdateResultsJob>();
			IJobParallelForExtensions.EarlyJobInit<ProcessLevelBoundsUpdateResultsJob>();
			IJobExtensions.EarlyJobInit<ProcessBoundsCheckResultsJob>();
			IJobParallelForTransformExtensions.EarlyJobInit<PlayerOcclusionManager.SetupRaycastsJob>();
			IJobParallelForExtensions.EarlyJobInit<PlayerOcclusionManager.UpdateOcclusion>();
			IJobParallelForExtensions.EarlyJobInit<TerrainGrassRenderer.GetValidTilesJob>();
			IJobParallelForDeferExtensions.EarlyJobInit<TerrainGrassRenderer.GenerateBatchJob>();
			IJobParallelForExtensions.EarlyJobInit<AmbientVfxManager.AmbientVfxDistanceJob>();
			IJobParallelForExtensions.EarlyJobInit<CalculateFirstGroundHitDistancesJob>();
			IJobParallelForExtensions.EarlyJobInit<CalculateGroundRollStopDistancesJob>();
			IJobExtensions.EarlyJobInit<ProcessDistanceEstimationsJob>();
		}
		catch (Exception ex)
		{
			EarlyInitHelpers.JobReflectionDataCreationFailed(ex);
		}
	}

	[RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterAssembliesLoaded)]
	public static void EarlyInit()
	{
		CreateJobReflectionData();
	}
}
