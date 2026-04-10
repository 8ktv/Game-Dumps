using System;
using System.Collections.Generic;

namespace UnityEngine.AdaptivePerformance;

[Serializable]
public class AdaptivePerformanceScalerProfile : AdaptivePerformanceScalerSettings
{
	[SerializeField]
	private List<AdaptivePerformanceScaler> m_AddedScalers = new List<AdaptivePerformanceScaler>();

	[SerializeField]
	[Tooltip("Name of the scaler profile.")]
	private string m_Name = "Default Scaler Profile";

	public string Name
	{
		get
		{
			return m_Name;
		}
		set
		{
			m_Name = value;
		}
	}

	public List<AdaptivePerformanceScaler> AddedScalers
	{
		get
		{
			return m_AddedScalers;
		}
		set
		{
			m_AddedScalers = value;
		}
	}

	internal void EnableAddedScalers()
	{
		for (int i = 0; i < m_AddedScalers.Count; i++)
		{
			if ((bool)m_AddedScalers[i])
			{
				m_AddedScalers[i].InitializeScaler();
			}
			else
			{
				APLog.Debug("Null scaler is added to the scaler list");
			}
		}
	}

	internal void RemoveAllAddedScalersFromIndexer()
	{
		foreach (AdaptivePerformanceScaler addedScaler in m_AddedScalers)
		{
			if ((bool)addedScaler)
			{
				addedScaler.RemoveScaler();
			}
		}
	}
}
