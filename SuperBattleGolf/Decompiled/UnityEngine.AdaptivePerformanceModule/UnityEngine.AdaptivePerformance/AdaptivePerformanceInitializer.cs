using UnityEngine.Scripting;

namespace UnityEngine.AdaptivePerformance;

internal static class AdaptivePerformanceInitializer
{
	private static AdaptivePerformanceManagerSpawner s_Spawner;

	[RequiredByNativeCode(false)]
	public static void AutoInitializeAdaptivePerformanceManaged()
	{
		InitializeSpawner(isAuto: true);
	}

	public static void Initialize()
	{
		InitializeSpawner(isAuto: false);
	}

	public static void Deinitialize()
	{
		if (!(s_Spawner == null))
		{
			s_Spawner.Deinitialize();
			Object.Destroy(s_Spawner);
			s_Spawner = null;
		}
	}

	private static void InitializeSpawner(bool isAuto)
	{
		if (s_Spawner == null)
		{
			s_Spawner = ScriptableObject.CreateInstance<AdaptivePerformanceManagerSpawner>();
		}
		if (!(s_Spawner != null) || !(s_Spawner.ManagerGameObject != null))
		{
			s_Spawner.Initialize(isAuto);
		}
	}
}
