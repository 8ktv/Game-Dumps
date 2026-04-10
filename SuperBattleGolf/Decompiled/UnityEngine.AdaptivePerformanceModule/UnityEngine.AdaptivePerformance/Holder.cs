namespace UnityEngine.AdaptivePerformance;

public static class Holder
{
	private static IAdaptivePerformance m_Instance;

	public static IAdaptivePerformance Instance
	{
		get
		{
			return m_Instance;
		}
		internal set
		{
			if (value == null)
			{
				Holder.LifecycleEventHandler?.Invoke(m_Instance, LifecycleChangeType.Destroyed);
			}
			else
			{
				Holder.LifecycleEventHandler?.Invoke(value, LifecycleChangeType.Created);
			}
			m_Instance = value;
		}
	}

	public static event LifecycleEventHandler LifecycleEventHandler;

	public static void Initialize()
	{
		if (Instance == null)
		{
			AdaptivePerformanceInitializer.Initialize();
			if (Instance != null)
			{
				Instance.InitializeAdaptivePerformance();
			}
		}
	}

	public static void Deinitialize()
	{
		if (Instance != null)
		{
			Instance.DeinitializeAdaptivePerformance();
		}
		AdaptivePerformanceInitializer.Deinitialize();
		Instance = null;
	}
}
