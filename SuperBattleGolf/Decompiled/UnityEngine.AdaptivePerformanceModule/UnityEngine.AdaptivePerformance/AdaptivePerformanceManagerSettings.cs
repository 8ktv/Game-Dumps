using System.Collections;
using System.Collections.Generic;

namespace UnityEngine.AdaptivePerformance;

public sealed class AdaptivePerformanceManagerSettings : ScriptableObject
{
	[HideInInspector]
	private bool m_InitializationComplete = false;

	[SerializeField]
	[Tooltip("Determines if the Adaptive Performance Manager instance is responsible for creating and destroying the appropriate loader instance.")]
	private bool m_AutomaticLoading = false;

	[Tooltip("Determines if the Adaptive Performance Manager instance is responsible for starting and stopping subsystems for the active loader instance.")]
	[SerializeField]
	private bool m_AutomaticRunning = false;

	[SerializeField]
	[Tooltip("List of Adaptive Performance Loader instances arranged in desired load order.")]
	private List<AdaptivePerformanceLoader> m_Loaders = new List<AdaptivePerformanceLoader>();

	[HideInInspector]
	private static AdaptivePerformanceLoader s_ActiveLoader;

	public bool automaticLoading
	{
		get
		{
			return m_AutomaticLoading;
		}
		set
		{
			m_AutomaticLoading = value;
		}
	}

	public bool automaticRunning
	{
		get
		{
			return m_AutomaticRunning;
		}
		set
		{
			m_AutomaticRunning = value;
		}
	}

	public List<AdaptivePerformanceLoader> loaders
	{
		get
		{
			return m_Loaders;
		}
		set
		{
			m_Loaders = value;
		}
	}

	public bool isInitializationComplete => m_InitializationComplete;

	[HideInInspector]
	public AdaptivePerformanceLoader activeLoader
	{
		get
		{
			return s_ActiveLoader;
		}
		private set
		{
			s_ActiveLoader = value;
		}
	}

	public T ActiveLoaderAs<T>() where T : AdaptivePerformanceLoader
	{
		return activeLoader as T;
	}

	internal void InitializeLoaderSync()
	{
		if (isInitializationComplete && activeLoader != null)
		{
			Debug.LogWarning("Adaptive Performance Management has already initialized an active loader in this scene.Please make sure to stop all subsystems and deinitialize the active loader before initializing a new one.");
			return;
		}
		foreach (AdaptivePerformanceLoader loader in loaders)
		{
			if (loader != null && loader.Initialize())
			{
				activeLoader = loader;
				m_InitializationComplete = true;
				return;
			}
		}
		activeLoader = null;
	}

	internal IEnumerator InitializeLoader()
	{
		if (isInitializationComplete && activeLoader != null)
		{
			Debug.LogWarning("Adaptive Performance Management has already initialized an active loader in this scene.Please make sure to stop all subsystems and deinitialize the active loader before initializing a new one.");
			yield break;
		}
		foreach (AdaptivePerformanceLoader loader in loaders)
		{
			if (loader != null && loader.Initialize())
			{
				activeLoader = loader;
				m_InitializationComplete = true;
				yield break;
			}
			yield return null;
		}
		activeLoader = null;
	}

	internal void StartSubsystems()
	{
		if (!m_InitializationComplete)
		{
			Debug.LogWarning("Call to StartSubsystems without an initialized manager.Please make sure to wait for initialization to complete before calling this API.");
		}
		else if (!(activeLoader == null))
		{
			activeLoader.Start();
		}
	}

	internal void StopSubsystems()
	{
		if (!m_InitializationComplete)
		{
			Debug.LogWarning("Call to StopSubsystems without an initialized manager.Please make sure to wait for initialization to complete before calling this API.");
		}
		else if (!(activeLoader == null))
		{
			activeLoader.Stop();
		}
	}

	internal void DeinitializeLoader()
	{
		if (!m_InitializationComplete)
		{
			Debug.LogWarning("Call to DeinitializeLoader without an initialized manager.Please make sure to wait for initialization to complete before calling this API.");
			return;
		}
		StopSubsystems();
		if (activeLoader != null)
		{
			activeLoader.Deinitialize();
			activeLoader = null;
		}
		m_InitializationComplete = false;
	}

	private void OnDisable()
	{
		if (automaticLoading && automaticRunning)
		{
			StopSubsystems();
		}
	}

	private void OnDestroy()
	{
		if (automaticLoading)
		{
			DeinitializeLoader();
		}
	}
}
