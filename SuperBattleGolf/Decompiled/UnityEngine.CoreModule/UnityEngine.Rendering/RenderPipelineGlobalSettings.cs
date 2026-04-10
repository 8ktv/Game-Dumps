using System;
using System.Collections.Generic;

namespace UnityEngine.Rendering;

public abstract class RenderPipelineGlobalSettings : ScriptableObject, ISerializationCallbackReceiver
{
	protected virtual List<IRenderPipelineGraphicsSettings> settingsList
	{
		get
		{
			Debug.LogWarning(string.Format("To be able to use {0} in your {1} you must override {2}", "IRenderPipelineGraphicsSettings", GetType(), "settingsList"));
			Debug.LogWarning(string.Format("Create your own '[{0}] List<{1}> m_Settings = new();' in your {2} and override {3} returning m_Settings;", "SerializeReference", "IRenderPipelineGraphicsSettings", GetType(), "settingsList"));
			return null;
		}
	}

	private Dictionary<Type, int> settingsMap { get; } = new Dictionary<Type, int>();

	private void RecreateSettingsMap()
	{
		settingsMap.Clear();
		if (settingsList == null)
		{
			return;
		}
		for (int i = 0; i < settingsList.Count; i++)
		{
			IRenderPipelineGraphicsSettings renderPipelineGraphicsSettings = settingsList[i];
			if (renderPipelineGraphicsSettings != null)
			{
				settingsMap[renderPipelineGraphicsSettings.GetType()] = i;
			}
		}
	}

	protected internal bool TryGet(Type type, out IRenderPipelineGraphicsSettings settings)
	{
		settings = null;
		if (settingsList == null)
		{
			return false;
		}
		if (!settingsMap.TryGetValue(type, out var value))
		{
			return false;
		}
		settings = settingsList[value];
		return settings != null;
	}

	protected internal bool TryGetFirstSettingsImplementingInterface<TSettingsInterfaceType>(out TSettingsInterfaceType settings) where TSettingsInterfaceType : class, IRenderPipelineGraphicsSettings
	{
		settings = null;
		if (settingsList == null)
		{
			return false;
		}
		for (int i = 0; i < settingsList.Count; i++)
		{
			if (settingsList[i] is TSettingsInterfaceType val)
			{
				settings = val;
				return true;
			}
		}
		return false;
	}

	protected internal bool GetSettingsImplementingInterface<TSettingsInterfaceType>(out List<TSettingsInterfaceType> settings) where TSettingsInterfaceType : class, IRenderPipelineGraphicsSettings
	{
		settings = new List<TSettingsInterfaceType>();
		if (settingsList == null)
		{
			return false;
		}
		for (int i = 0; i < settingsList.Count; i++)
		{
			if (settingsList[i] is TSettingsInterfaceType item)
			{
				settings.Add(item);
			}
		}
		return settings.Count > 0;
	}

	protected internal bool Contains(Type type)
	{
		return settingsList != null && settingsMap.ContainsKey(type);
	}

	public virtual void OnBeforeSerialize()
	{
	}

	public virtual void OnAfterDeserialize()
	{
		RecreateSettingsMap();
	}
}
