using Unity.Jobs;

namespace UnityEngine.UIElements;

internal static class ManagedJobExtension
{
	public static JobHandle ScheduleOrRunJob<T>(this T jobData, int arrayLength, int innerloopBatchCount, JobHandle dependsOn = default(JobHandle)) where T : struct, IJobParallelFor
	{
		return jobData.Schedule(arrayLength, innerloopBatchCount, dependsOn);
	}
}
