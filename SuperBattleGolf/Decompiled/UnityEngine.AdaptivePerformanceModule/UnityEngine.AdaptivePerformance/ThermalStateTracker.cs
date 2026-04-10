using UnityEngine.AdaptivePerformance.Provider;

namespace UnityEngine.AdaptivePerformance;

internal class ThermalStateTracker
{
	private float warningTemp = 1f;

	private float throttlingTemp = 1f;

	public StateAction Update()
	{
		if (!Holder.Instance.SupportedFeature(Feature.TemperatureLevel))
		{
			return StateAction.Stale;
		}
		float temperatureTrend = Holder.Instance.ThermalStatus.ThermalMetrics.TemperatureTrend;
		float temperatureLevel = Holder.Instance.ThermalStatus.ThermalMetrics.TemperatureLevel;
		WarningLevel warningLevel = Holder.Instance.ThermalStatus.ThermalMetrics.WarningLevel;
		if (warningLevel == WarningLevel.ThrottlingImminent && warningTemp == 1f)
		{
			warningTemp = temperatureLevel;
		}
		if (warningLevel == WarningLevel.Throttling && throttlingTemp == 1f)
		{
			throttlingTemp = temperatureLevel;
		}
		if (warningLevel == WarningLevel.Throttling || temperatureLevel >= throttlingTemp)
		{
			return StateAction.FastDecrease;
		}
		if (warningLevel == WarningLevel.ThrottlingImminent || temperatureLevel >= warningTemp)
		{
			if (temperatureLevel > (warningTemp + throttlingTemp) / 2f)
			{
				return StateAction.Decrease;
			}
			if (temperatureTrend <= 0f)
			{
				return StateAction.Stale;
			}
			if ((double)temperatureTrend > 0.5)
			{
				return StateAction.FastDecrease;
			}
			return StateAction.Decrease;
		}
		if (warningLevel == WarningLevel.NoWarning && temperatureLevel < warningTemp)
		{
			if (temperatureTrend <= 0f)
			{
				return StateAction.Increase;
			}
			if ((double)temperatureTrend > 0.5)
			{
				return StateAction.FastDecrease;
			}
			if ((double)temperatureTrend > 0.1)
			{
				return StateAction.Decrease;
			}
		}
		return StateAction.Stale;
	}
}
