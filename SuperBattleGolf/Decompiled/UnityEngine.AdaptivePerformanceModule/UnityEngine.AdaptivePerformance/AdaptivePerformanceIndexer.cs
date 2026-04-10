using System.Collections.Generic;
using UnityEngine.Profiling;

namespace UnityEngine.AdaptivePerformance;

public class AdaptivePerformanceIndexer
{
	private List<AdaptivePerformanceScaler> m_UnappliedScalers;

	private List<AdaptivePerformanceScaler> m_AppliedScalers;

	private List<AdaptivePerformanceScaler> m_DisabledScalers;

	private ThermalStateTracker m_ThermalStateTracker;

	private PerformanceStateTracker m_PerformanceStateTracker;

	private AdaptivePerformanceScalerEfficiencyTracker m_ScalerEfficiencyTracker;

	private IAdaptivePerformanceSettings m_Settings;

	private const string m_FeatureName = "Indexer";

	public float TimeUntilNextAction { get; private set; }

	public StateAction ThermalAction { get; private set; }

	public StateAction PerformanceAction { get; private set; }

	public void GetAppliedScalers(ref List<AdaptivePerformanceScaler> scalers)
	{
		scalers.Clear();
		scalers.AddRange(m_AppliedScalers);
	}

	public void GetUnappliedScalers(ref List<AdaptivePerformanceScaler> scalers)
	{
		scalers.Clear();
		scalers.AddRange(m_UnappliedScalers);
	}

	public void GetDisabledScalers(ref List<AdaptivePerformanceScaler> scalers)
	{
		scalers.Clear();
		scalers.AddRange(m_DisabledScalers);
	}

	public void GetAllRegisteredScalers(ref List<AdaptivePerformanceScaler> scalers)
	{
		scalers.Clear();
		scalers.AddRange(m_DisabledScalers);
		scalers.AddRange(m_UnappliedScalers);
		scalers.AddRange(m_AppliedScalers);
	}

	public void UnapplyAllScalers()
	{
		TimeUntilNextAction = m_Settings.indexerSettings.thermalActionDelay;
		while (m_AppliedScalers.Count != 0)
		{
			AdaptivePerformanceScaler scaler = m_AppliedScalers[0];
			UnapplyScaler(scaler);
		}
	}

	internal void UpdateOverrideLevel(AdaptivePerformanceScaler scaler)
	{
		if (scaler.OverrideLevel != -1)
		{
			while (scaler.OverrideLevel > scaler.CurrentLevel)
			{
				ApplyScaler(scaler);
			}
			while (scaler.OverrideLevel < scaler.CurrentLevel)
			{
				UnapplyScaler(scaler);
			}
		}
	}

	internal void AddScaler(AdaptivePerformanceScaler scaler)
	{
		if (!m_UnappliedScalers.Contains(scaler) && !m_AppliedScalers.Contains(scaler))
		{
			m_UnappliedScalers.Add(scaler);
		}
	}

	internal bool RemoveScaler(AdaptivePerformanceScaler scaler)
	{
		bool result = false;
		if (m_UnappliedScalers.Contains(scaler))
		{
			m_UnappliedScalers.Remove(scaler);
			result = true;
		}
		if (m_DisabledScalers.Contains(scaler))
		{
			m_DisabledScalers.Remove(scaler);
			result = true;
		}
		if (m_AppliedScalers.Contains(scaler))
		{
			while (!scaler.NotLeveled)
			{
				scaler.DecreaseLevel();
			}
			m_AppliedScalers.Remove(scaler);
			result = true;
		}
		return result;
	}

	internal AdaptivePerformanceIndexer(ref IAdaptivePerformanceSettings settings, PerformanceStateTracker tracker)
	{
		m_Settings = settings;
		TimeUntilNextAction = m_Settings.indexerSettings.thermalActionDelay;
		m_ThermalStateTracker = new ThermalStateTracker();
		m_PerformanceStateTracker = tracker;
		m_UnappliedScalers = new List<AdaptivePerformanceScaler>();
		m_AppliedScalers = new List<AdaptivePerformanceScaler>();
		m_DisabledScalers = new List<AdaptivePerformanceScaler>();
		m_ScalerEfficiencyTracker = new AdaptivePerformanceScalerEfficiencyTracker();
	}

	internal void Update()
	{
		if (Holder.Instance == null || !m_Settings.indexerSettings.active)
		{
			return;
		}
		DeactivateDisabledScalers();
		ActivateEnabledScalers();
		StateAction stateAction = m_ThermalStateTracker.Update();
		StateAction stateAction2 = m_PerformanceStateTracker.Update();
		ThermalAction = stateAction;
		PerformanceAction = stateAction2;
		if (Profiler.enabled)
		{
			CollectProfilerStats();
		}
		TimeUntilNextAction = Mathf.Max(TimeUntilNextAction - DeltaTime(), 0f);
		if (TimeUntilNextAction == 0f)
		{
			if (m_ScalerEfficiencyTracker.IsRunning)
			{
				m_ScalerEfficiencyTracker.Stop();
			}
			if (stateAction == StateAction.Increase && stateAction2 == StateAction.Stale)
			{
				UnapplyHighestCostScaler();
				TimeUntilNextAction = m_Settings.indexerSettings.thermalActionDelay;
			}
			else if (stateAction == StateAction.Stale && stateAction2 == StateAction.Stale)
			{
				UnapplyHighestCostScaler();
				TimeUntilNextAction = m_Settings.indexerSettings.thermalActionDelay;
			}
			else if (stateAction == StateAction.Decrease)
			{
				ApplyLowestCostScaler();
				TimeUntilNextAction = m_Settings.indexerSettings.thermalActionDelay;
			}
			else if (stateAction2 == StateAction.Decrease)
			{
				ApplyLowestCostScaler();
				TimeUntilNextAction = m_Settings.indexerSettings.performanceActionDelay;
			}
			else if (stateAction == StateAction.FastDecrease)
			{
				ApplyLowestCostScaler();
				TimeUntilNextAction = m_Settings.indexerSettings.thermalActionDelay / 2f;
			}
			else if (stateAction2 == StateAction.FastDecrease)
			{
				ApplyLowestCostScaler();
				TimeUntilNextAction = m_Settings.indexerSettings.performanceActionDelay / 2f;
			}
		}
	}

	protected virtual float DeltaTime()
	{
		return Time.deltaTime;
	}

	private void CollectProfilerStats()
	{
		for (int num = m_UnappliedScalers.Count - 1; num >= 0; num--)
		{
			AdaptivePerformanceScaler adaptivePerformanceScaler = m_UnappliedScalers[num];
		}
		for (int num2 = m_AppliedScalers.Count - 1; num2 >= 0; num2--)
		{
			AdaptivePerformanceScaler adaptivePerformanceScaler2 = m_AppliedScalers[num2];
		}
		for (int num3 = m_DisabledScalers.Count - 1; num3 >= 0; num3--)
		{
			AdaptivePerformanceScaler adaptivePerformanceScaler3 = m_DisabledScalers[num3];
		}
		AdaptivePerformanceProfilerStats.FlushScalerDataToProfilerStream();
	}

	private void DeactivateDisabledScalers()
	{
		for (int num = m_UnappliedScalers.Count - 1; num >= 0; num--)
		{
			AdaptivePerformanceScaler adaptivePerformanceScaler = m_UnappliedScalers[num];
			if (!adaptivePerformanceScaler.Enabled && !m_DisabledScalers.Contains(adaptivePerformanceScaler))
			{
				APLog.Debug("[Indexer] Deactivated " + adaptivePerformanceScaler.Name + " scaler.");
				adaptivePerformanceScaler.Deactivate();
				m_DisabledScalers.Add(adaptivePerformanceScaler);
				m_UnappliedScalers.RemoveAt(num);
			}
		}
		for (int num2 = m_AppliedScalers.Count - 1; num2 >= 0; num2--)
		{
			AdaptivePerformanceScaler adaptivePerformanceScaler2 = m_AppliedScalers[num2];
			if (!adaptivePerformanceScaler2.Enabled && !m_DisabledScalers.Contains(adaptivePerformanceScaler2))
			{
				APLog.Debug("[Indexer] Deactivated " + adaptivePerformanceScaler2.Name + " scaler.");
				adaptivePerformanceScaler2.Deactivate();
				m_DisabledScalers.Add(adaptivePerformanceScaler2);
				m_AppliedScalers.RemoveAt(num2);
			}
		}
	}

	private void ActivateEnabledScalers()
	{
		for (int num = m_DisabledScalers.Count - 1; num >= 0; num--)
		{
			AdaptivePerformanceScaler adaptivePerformanceScaler = m_DisabledScalers[num];
			if (adaptivePerformanceScaler.Enabled)
			{
				adaptivePerformanceScaler.Activate();
				AddScaler(adaptivePerformanceScaler);
				m_DisabledScalers.RemoveAt(num);
				APLog.Debug("[Indexer] Activated " + adaptivePerformanceScaler.Name + " scaler.");
			}
		}
	}

	private bool ApplyLowestCostScaler()
	{
		AdaptivePerformanceScaler adaptivePerformanceScaler = null;
		float num = float.PositiveInfinity;
		foreach (AdaptivePerformanceScaler unappliedScaler in m_UnappliedScalers)
		{
			if (unappliedScaler.Enabled && unappliedScaler.OverrideLevel == -1)
			{
				int num2 = unappliedScaler.CalculateCost();
				if (num > (float)num2)
				{
					adaptivePerformanceScaler = unappliedScaler;
					num = num2;
				}
			}
		}
		foreach (AdaptivePerformanceScaler appliedScaler in m_AppliedScalers)
		{
			if (appliedScaler.Enabled && appliedScaler.OverrideLevel == -1 && !appliedScaler.IsMaxLevel)
			{
				int num3 = appliedScaler.CalculateCost();
				if (num > (float)num3)
				{
					adaptivePerformanceScaler = appliedScaler;
					num = num3;
				}
			}
		}
		if (adaptivePerformanceScaler != null)
		{
			m_ScalerEfficiencyTracker.Start(adaptivePerformanceScaler, isApply: true);
			ApplyScaler(adaptivePerformanceScaler);
			return true;
		}
		return false;
	}

	private void ApplyScaler(AdaptivePerformanceScaler scaler)
	{
		APLog.Debug($"[Indexer] Applying {scaler.Name} scaler at level {scaler.CurrentLevel} and try to increase level to {scaler.CurrentLevel + 1}");
		if (scaler.NotLeveled)
		{
			m_UnappliedScalers.Remove(scaler);
			m_AppliedScalers.Add(scaler);
		}
		scaler.IncreaseLevel();
	}

	private bool UnapplyHighestCostScaler()
	{
		AdaptivePerformanceScaler adaptivePerformanceScaler = null;
		float num = float.NegativeInfinity;
		foreach (AdaptivePerformanceScaler appliedScaler in m_AppliedScalers)
		{
			if (appliedScaler.OverrideLevel == -1)
			{
				int num2 = appliedScaler.CalculateCost();
				if (num < (float)num2)
				{
					adaptivePerformanceScaler = appliedScaler;
					num = num2;
				}
			}
		}
		if (adaptivePerformanceScaler != null)
		{
			m_ScalerEfficiencyTracker.Start(adaptivePerformanceScaler, isApply: false);
			UnapplyScaler(adaptivePerformanceScaler);
			return true;
		}
		return false;
	}

	private void UnapplyScaler(AdaptivePerformanceScaler scaler)
	{
		APLog.Debug($"[Indexer] Unapplying {scaler.Name} scaler at level {scaler.CurrentLevel} and try to decrease level to {scaler.CurrentLevel - 1}");
		scaler.DecreaseLevel();
		if (scaler.NotLeveled)
		{
			m_AppliedScalers.Remove(scaler);
			m_UnappliedScalers.Add(scaler);
		}
	}
}
