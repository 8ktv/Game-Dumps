using System;

namespace UnityEngine.AdaptivePerformance;

internal class TemperatureTrend
{
	private bool m_UseProviderTrend;

	private double m_SumX;

	private double m_SumY;

	private double m_SumXY;

	private double m_SumXX;

	private const int MeasurementTimeframeSeconds = 20;

	private const int UpdateFrequency = 10;

	private const int SamplesCapacity = 200;

	private const double SlopeAtMaxTrend = 0.005;

	private float[] m_TimeStamps = new float[200];

	private float[] m_Temperature = new float[200];

	private int m_NumValues;

	private int m_NextValueIndex;

	private int m_OldestValueIndex;

	public float ThermalTrend { get; private set; }

	public int NumValues
	{
		get
		{
			return m_NumValues;
		}
		set
		{
			m_NumValues = value;
		}
	}

	private void PopOldestValue()
	{
		double num = m_TimeStamps[m_OldestValueIndex];
		double num2 = m_Temperature[m_OldestValueIndex];
		m_SumX -= num;
		m_SumY -= num2;
		m_SumXY -= num * num2;
		m_SumXX -= num * num;
		m_OldestValueIndex = (m_OldestValueIndex + 1) % 200;
		m_NumValues--;
	}

	private void PushNewValue(float tempLevel, float timestamp)
	{
		m_TimeStamps[m_NextValueIndex] = timestamp;
		m_Temperature[m_NextValueIndex] = tempLevel;
		m_NextValueIndex = (m_NextValueIndex + 1) % 200;
		m_NumValues++;
		double num = timestamp;
		double num2 = tempLevel;
		m_SumX += num;
		m_SumY += num2;
		m_SumXY += num * num2;
		m_SumXX += num * num;
	}

	public TemperatureTrend(bool useProviderTrend)
	{
		m_UseProviderTrend = useProviderTrend;
	}

	public void Reset()
	{
		m_NumValues = 0;
		m_OldestValueIndex = 0;
		m_NextValueIndex = 0;
		m_SumX = 0.0;
		m_SumY = 0.0;
		m_SumXY = 0.0;
		m_SumXX = 0.0;
		ThermalTrend = 0f;
	}

	private void UpdateTrend()
	{
		if (m_NumValues < 2)
		{
			ThermalTrend = 0f;
			return;
		}
		double num = (double)m_NumValues * m_SumXY - m_SumX * m_SumY;
		double num2 = (double)m_NumValues * m_SumXX - m_SumX * m_SumX;
		double num3 = num / num2;
		num3 /= 0.005;
		if (num3 >= 1.0)
		{
			ThermalTrend = 1f;
		}
		else if (num3 >= -1.0)
		{
			if (Math.Abs(num3) < 1E-05)
			{
				ThermalTrend = 0f;
			}
			else
			{
				ThermalTrend = (float)num3;
			}
		}
		else if (num3 <= -1.0)
		{
			ThermalTrend = -1f;
		}
		else
		{
			ThermalTrend = 0f;
		}
	}

	public void Update(float temperatureTrendFromProvider, float newTemperatureLevel, bool changed, float newTemperatureTimestamp)
	{
		if (m_UseProviderTrend)
		{
			ThermalTrend = temperatureTrendFromProvider;
			return;
		}
		newTemperatureLevel = newTemperatureLevel * newTemperatureLevel * newTemperatureLevel;
		if (m_NumValues == 0)
		{
			PushNewValue(newTemperatureLevel, newTemperatureTimestamp);
			UpdateTrend();
			return;
		}
		bool flag = false;
		float num = m_TimeStamps[m_OldestValueIndex];
		float num2 = num + 0.1f * (float)m_NumValues;
		if (newTemperatureTimestamp - num > 20f)
		{
			PopOldestValue();
			flag = true;
		}
		if (changed || newTemperatureTimestamp >= num2)
		{
			if (m_NumValues == 200)
			{
				PopOldestValue();
			}
			PushNewValue(newTemperatureLevel, newTemperatureTimestamp);
			flag = true;
		}
		if (flag)
		{
			UpdateTrend();
		}
	}
}
