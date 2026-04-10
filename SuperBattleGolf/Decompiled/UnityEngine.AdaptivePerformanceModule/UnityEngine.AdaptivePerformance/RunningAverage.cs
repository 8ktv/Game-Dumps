using System;

namespace UnityEngine.AdaptivePerformance;

internal class RunningAverage
{
	private float[] m_Values = null;

	private int m_NumValues = 0;

	private int m_LastIndex = -1;

	private float m_AverageValue = 0f;

	public RunningAverage(int sampleWindowSize = 100)
	{
		m_Values = new float[sampleWindowSize];
	}

	public int GetNumValues()
	{
		return m_NumValues;
	}

	public int GetSampleWindowSize()
	{
		return m_Values.Length;
	}

	public float GetAverageOr(float defaultValue)
	{
		return (m_NumValues > 0) ? m_AverageValue : defaultValue;
	}

	public float GetMostRecentValueOr(float defaultValue)
	{
		return (m_NumValues > 0) ? m_Values[m_LastIndex] : defaultValue;
	}

	public void AddValue(float NewValue)
	{
		int num = (m_LastIndex + 1) % m_Values.Length;
		float num2 = m_Values[num];
		m_LastIndex = num;
		m_Values[m_LastIndex] = NewValue;
		float num3 = m_AverageValue * (float)m_NumValues + NewValue - num2;
		m_NumValues = Mathf.Min(m_NumValues + 1, m_Values.Length);
		m_AverageValue = num3 / (float)m_NumValues;
	}

	public void Reset()
	{
		m_NumValues = 0;
		m_LastIndex = -1;
		m_AverageValue = 0f;
		Array.Clear(m_Values, 0, m_Values.Length);
	}
}
