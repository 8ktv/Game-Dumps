using System;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine.Bindings;

namespace UnityEngine.AdaptivePerformance;

public class IAdaptivePerformanceSettings : ScriptableObject
{
	[SerializeField]
	[Tooltip("Enable Logging in Devmode")]
	private bool m_Logging = true;

	[SerializeField]
	[Tooltip("Automatic Performance Mode")]
	private bool m_AutomaticPerformanceModeEnabled = true;

	[SerializeField]
	[Tooltip("Automatic Game Mode")]
	private bool m_AutomaticGameModeEnabled = false;

	[SerializeField]
	[Tooltip("Enables the CPU and GPU boost mode before engine startup to decrease startup time.")]
	private bool m_EnableBoostOnStartup = true;

	[Tooltip("Logging Frequency (Development mode only)")]
	[SerializeField]
	private int m_StatsLoggingFrequencyInFrames = 50;

	[Tooltip("Indexer Settings")]
	[SerializeField]
	private AdaptivePerformanceIndexerSettings m_IndexerSettings;

	[SerializeField]
	[Tooltip("Scaler Settings")]
	private AdaptivePerformanceScalerSettings m_ScalerSettings;

	[SerializeField]
	private List<AdaptivePerformanceScaler> m_AddedScalerViaScan = new List<AdaptivePerformanceScaler>();

	[SerializeField]
	private AdaptivePerformanceScalerProfile m_ActiveScalerProfile;

	[SerializeField]
	private AdaptivePerformanceScalerProfile[] m_scalerProfileList = new AdaptivePerformanceScalerProfile[1]
	{
		new AdaptivePerformanceScalerProfile()
	};

	[SerializeField]
	internal int m_DefaultScalerProfilerIndex = 0;

	[SerializeField]
	private int k_AssetVersion = 3;

	public bool logging
	{
		get
		{
			return m_Logging;
		}
		set
		{
			m_Logging = value;
		}
	}

	public bool automaticPerformanceMode
	{
		get
		{
			return m_AutomaticPerformanceModeEnabled;
		}
		set
		{
			m_AutomaticPerformanceModeEnabled = value;
		}
	}

	public bool automaticGameMode
	{
		get
		{
			return m_AutomaticGameModeEnabled;
		}
		set
		{
			m_AutomaticGameModeEnabled = value;
		}
	}

	public bool enableBoostOnStartup
	{
		get
		{
			return m_EnableBoostOnStartup;
		}
		set
		{
			m_EnableBoostOnStartup = value;
		}
	}

	public int statsLoggingFrequencyInFrames
	{
		get
		{
			return m_StatsLoggingFrequencyInFrames;
		}
		set
		{
			m_StatsLoggingFrequencyInFrames = value;
		}
	}

	public AdaptivePerformanceIndexerSettings indexerSettings
	{
		get
		{
			return m_IndexerSettings;
		}
		set
		{
			m_IndexerSettings = value;
		}
	}

	public AdaptivePerformanceScalerSettings scalerSettings
	{
		get
		{
			return m_ScalerSettings;
		}
		set
		{
			m_ScalerSettings = value;
		}
	}

	public AdaptivePerformanceScalerProfile[] ScalerProfiles => m_scalerProfileList;

	public AdaptivePerformanceScalerProfile ActiveScalerProfile
	{
		get
		{
			if (m_ActiveScalerProfile == null && m_scalerProfileList.Length != 0)
			{
				return m_scalerProfileList[0];
			}
			return m_ActiveScalerProfile;
		}
		set
		{
			m_ActiveScalerProfile = value;
		}
	}

	[VisibleToOtherModules(new string[] { "UnityEditor.AdaptivePerformanceModule" })]
	internal List<AdaptivePerformanceScaler> AddedScalerViaScan
	{
		get
		{
			return m_AddedScalerViaScan;
		}
		set
		{
			m_AddedScalerViaScan = value;
		}
	}

	public int defaultScalerProfilerIndex
	{
		get
		{
			return m_DefaultScalerProfilerIndex;
		}
		set
		{
			m_DefaultScalerProfilerIndex = value;
		}
	}

	public void AddScalerProfileWithDefaultScalers(string name = "")
	{
		AdaptivePerformanceScalerProfile[] scalerProfiles = ScalerProfiles;
		foreach (AdaptivePerformanceScalerProfile adaptivePerformanceScalerProfile in scalerProfiles)
		{
			if (adaptivePerformanceScalerProfile.Name == name)
			{
				Debug.LogWarning(adaptivePerformanceScalerProfile.Name + " already exists in the profile list");
				return;
			}
		}
		Array.Resize(ref m_scalerProfileList, m_scalerProfileList.Length + 1);
		m_scalerProfileList[^1] = new AdaptivePerformanceScalerProfile();
		if (!string.IsNullOrEmpty(name))
		{
			m_scalerProfileList[^1].Name = name;
		}
	}

	public void DeleteScalerProfileAt(int index)
	{
		if (index >= ScalerProfiles.Length || index < 0)
		{
			return;
		}
		AdaptivePerformanceScalerProfile[] array = new AdaptivePerformanceScalerProfile[ScalerProfiles.Length - 1];
		int num = 0;
		for (int i = 0; i < ScalerProfiles.Length; i++)
		{
			if (i != index)
			{
				array[num] = ScalerProfiles[i];
				num++;
			}
		}
		m_scalerProfileList = array;
	}

	public void LoadScalerProfile(string scalerProfileName)
	{
		if (scalerProfileName == null || scalerProfileName.Length <= 0)
		{
			APLog.Debug("Scaler profile name empty. Can not load and apply profile.");
			return;
		}
		if (m_scalerProfileList.Length == 0)
		{
			APLog.Debug("No scaler profiles available. Can not load and apply profile. Add more profiles in the Adaptive Performance settings.");
			return;
		}
		if (ActiveScalerProfile != null && ActiveScalerProfile.Name == scalerProfileName)
		{
			APLog.Debug("The " + ActiveScalerProfile.Name + " scaler profile is already loaded.");
			return;
		}
		if (m_scalerProfileList.Length == 1)
		{
			APLog.Debug("Only default scaler profile available. Reset all scalers to default profile.");
		}
		for (int i = 0; i < m_scalerProfileList.Length; i++)
		{
			AdaptivePerformanceScalerProfile adaptivePerformanceScalerProfile = m_scalerProfileList[i];
			if (adaptivePerformanceScalerProfile == null)
			{
				APLog.Debug("Scaler profile is null. Can not load and apply profile. Check Adaptive Performance settings.");
				return;
			}
			if (adaptivePerformanceScalerProfile.Name == null || adaptivePerformanceScalerProfile.Name.Length <= 0)
			{
				APLog.Debug("Scaler profile name is null or empty. Can not load and apply profile. Check Adaptive Performance settings.");
				return;
			}
			if (!(adaptivePerformanceScalerProfile.Name == scalerProfileName))
			{
				continue;
			}
			if (ActiveScalerProfile != null)
			{
				ActiveScalerProfile.RemoveAllAddedScalersFromIndexer();
			}
			scalerSettings.ApplySettings(adaptivePerformanceScalerProfile);
			if (adaptivePerformanceScalerProfile.AddedScalers != null && adaptivePerformanceScalerProfile.AddedScalers.Count > 0)
			{
				adaptivePerformanceScalerProfile.EnableAddedScalers();
				for (int j = 0; j < AddedScalerViaScan.Count; j++)
				{
					AddedScalerViaScan[j].RemoveScaler();
				}
			}
			ActiveScalerProfile = adaptivePerformanceScalerProfile;
			break;
		}
		if (ApplyScalerProfileToAllScalers())
		{
			APLog.Debug("Scaler profile " + scalerProfileName + " loaded.");
		}
	}

	private bool ApplyScalerProfileToAllScalers()
	{
		bool result = false;
		if (Holder.Instance == null || Holder.Instance.Indexer == null)
		{
			return result;
		}
		List<AdaptivePerformanceScaler> list = new List<AdaptivePerformanceScaler>();
		List<AdaptivePerformanceScaler> scalers = new List<AdaptivePerformanceScaler>();
		Holder.Instance.Indexer.GetUnappliedScalers(ref scalers);
		list.AddRange(scalers);
		Holder.Instance.Indexer.GetAppliedScalers(ref scalers);
		list.AddRange(scalers);
		Holder.Instance.Indexer.GetDisabledScalers(ref scalers);
		list.AddRange(scalers);
		if (list.Count <= 0)
		{
			APLog.Debug("No scalers found. No scaler profile applied.");
			return result;
		}
		PropertyInfo[] properties = typeof(AdaptivePerformanceScalerSettings).GetProperties();
		PropertyInfo[] array = properties;
		foreach (PropertyInfo property in array)
		{
			AdaptivePerformanceScaler adaptivePerformanceScaler = list.Find((AdaptivePerformanceScaler s) => s.GetType().ToString().Contains(property.Name));
			if ((bool)adaptivePerformanceScaler)
			{
				PropertyInfo property2 = typeof(AdaptivePerformanceScalerSettings).GetProperty(property.Name);
				object value = property2.GetValue(scalerSettings);
				adaptivePerformanceScaler.Deactivate();
				adaptivePerformanceScaler.ApplyDefaultSetting((AdaptivePerformanceScalerSettingsBase)value);
				adaptivePerformanceScaler.Activate();
				result = true;
			}
		}
		return result;
	}

	public string[] GetAvailableScalerProfiles()
	{
		string[] array = new string[m_scalerProfileList.Length];
		if (m_scalerProfileList.Length == 0)
		{
			APLog.Debug("No scaler profiles available. You can not load and apply profiles. Add more profiles in the Adaptive Performance settings.");
			return array;
		}
		for (int i = 0; i < m_scalerProfileList.Length; i++)
		{
			AdaptivePerformanceScalerProfile adaptivePerformanceScalerProfile = m_scalerProfileList[i];
			array[i] = adaptivePerformanceScalerProfile.Name;
		}
		return array;
	}

	public void OnEnable()
	{
		if (k_AssetVersion < 3)
		{
			k_AssetVersion = 2;
		}
	}
}
