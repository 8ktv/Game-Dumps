namespace UnityEngine.AdaptivePerformance;

internal class BottleneckUtil
{
	public static PerformanceBottleneck DetermineBottleneck(PerformanceBottleneck prevBottleneck, float averageCpuFrameTime, float averageGpuFrametime, float averageOverallFrametime, float targetFrameTime)
	{
		if (HittingFrameRateLimit(averageOverallFrametime, (prevBottleneck == PerformanceBottleneck.TargetFrameRate) ? 0.03f : 0.02f, targetFrameTime))
		{
			return PerformanceBottleneck.TargetFrameRate;
		}
		if (averageGpuFrametime >= averageOverallFrametime)
		{
			return PerformanceBottleneck.GPU;
		}
		if (averageCpuFrameTime >= averageOverallFrametime)
		{
			return PerformanceBottleneck.CPU;
		}
		bool flag = prevBottleneck == PerformanceBottleneck.GPU;
		bool flag2 = prevBottleneck == PerformanceBottleneck.CPU;
		float num = averageGpuFrametime / averageOverallFrametime;
		float num2 = averageCpuFrameTime / averageOverallFrametime;
		float num3 = (flag2 ? 0.87f : 0.9f);
		if (num2 > num3)
		{
			return PerformanceBottleneck.CPU;
		}
		float num4 = (flag ? 0.87f : 0.9f);
		if (averageGpuFrametime > num4)
		{
			return PerformanceBottleneck.GPU;
		}
		if (averageGpuFrametime > averageCpuFrameTime)
		{
			float num5 = (flag ? 0.9f : 0.92f);
			if (num > num5)
			{
				float num6 = (flag ? 0.92f : 0.9f);
				if (averageGpuFrametime * num6 > averageCpuFrameTime)
				{
					return PerformanceBottleneck.GPU;
				}
			}
		}
		else
		{
			float num7 = (flag2 ? 0.5f : 0.52f);
			if (num2 > num7 && averageGpuFrametime < averageCpuFrameTime)
			{
				float num8 = (flag2 ? 0.85f : 0.8f);
				if (averageCpuFrameTime * num8 > averageGpuFrametime)
				{
					return PerformanceBottleneck.CPU;
				}
			}
		}
		return PerformanceBottleneck.Unknown;
	}

	private static bool HittingFrameRateLimit(float actualFrameTime, float thresholdFactor, float targetFrameTime)
	{
		if (targetFrameTime <= 0f)
		{
			return false;
		}
		if (actualFrameTime <= targetFrameTime)
		{
			return true;
		}
		if (actualFrameTime - targetFrameTime < thresholdFactor * targetFrameTime)
		{
			return true;
		}
		return false;
	}
}
