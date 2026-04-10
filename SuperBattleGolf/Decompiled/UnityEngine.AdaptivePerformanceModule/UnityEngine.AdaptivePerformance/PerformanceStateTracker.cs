using System.Collections.Generic;

namespace UnityEngine.AdaptivePerformance;

internal class PerformanceStateTracker
{
	private Queue<float> m_Samples;

	private int m_SampleCapacity;

	public float Trend { get; set; }

	public PerformanceStateTracker(int sampleCapacity)
	{
		m_Samples = new Queue<float>(sampleCapacity);
		m_SampleCapacity = sampleCapacity;
	}

	public StateAction Update()
	{
		float averageFrameTime = Holder.Instance.PerformanceStatus.FrameTiming.AverageFrameTime;
		if (averageFrameTime > 0f)
		{
			float num = 1f / GetEffectiveTargetFrameRate();
			float item = averageFrameTime / num - 1f;
			m_Samples.Enqueue(item);
			if (m_Samples.Count > m_SampleCapacity)
			{
				m_Samples.Dequeue();
			}
		}
		float num2 = 0f;
		foreach (float sample in m_Samples)
		{
			num2 += sample;
		}
		num2 /= (float)m_Samples.Count;
		Trend = num2;
		if ((double)Trend >= 0.3)
		{
			return StateAction.FastDecrease;
		}
		if ((double)Trend >= 0.15)
		{
			return StateAction.Decrease;
		}
		return StateAction.Stale;
	}

	protected virtual float GetEffectiveTargetFrameRate()
	{
		return AdaptivePerformanceManager.EffectiveTargetFrameRate();
	}
}
