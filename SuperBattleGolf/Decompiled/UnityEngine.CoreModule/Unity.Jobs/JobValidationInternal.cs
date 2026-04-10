using System;
using System.Diagnostics;
using Unity.Burst;
using UnityEngine.Bindings;

namespace Unity.Jobs;

[VisibleToOtherModules(new string[] { "UnityEngine.ParticleSystemModule" })]
internal static class JobValidationInternal
{
	[Conditional("ENABLE_UNITY_COLLECTIONS_CHECKS")]
	[VisibleToOtherModules(new string[] { "UnityEngine.ParticleSystemModule" })]
	internal static void CheckReflectionDataCorrect<T>(IntPtr reflectionData)
	{
	}

	[BurstDiscard]
	[Conditional("ENABLE_UNITY_COLLECTIONS_CHECKS")]
	private static void CheckReflectionDataCorrectInternal<T>(IntPtr reflectionData, ref bool burstCompiled)
	{
		if (reflectionData == IntPtr.Zero)
		{
			throw new InvalidOperationException($"Reflection data was not set up by an Initialize() call. Support for burst compiled calls to Schedule depends on the Collections package.\n\nFor generic job types, please include [assembly: RegisterGenericJobType(typeof({typeof(T)}))] in your source file.");
		}
		burstCompiled = false;
	}
}
