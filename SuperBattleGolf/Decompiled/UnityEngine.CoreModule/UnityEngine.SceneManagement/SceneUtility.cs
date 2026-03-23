using System;
using System.Runtime.CompilerServices;
using UnityEngine.Bindings;

namespace UnityEngine.SceneManagement;

[NativeHeader("Runtime/Export/SceneManager/SceneUtility.bindings.h")]
public static class SceneUtility
{
	[StaticAccessor("SceneUtilityBindings", StaticAccessorType.DoubleColon)]
	public static string GetScenePathByBuildIndex(int buildIndex)
	{
		ManagedSpanWrapper ret = default(ManagedSpanWrapper);
		string stringAndDispose;
		try
		{
			GetScenePathByBuildIndex_Injected(buildIndex, out ret);
		}
		finally
		{
			stringAndDispose = OutStringMarshaller.GetStringAndDispose(ret);
		}
		return stringAndDispose;
	}

	[StaticAccessor("SceneUtilityBindings", StaticAccessorType.DoubleColon)]
	public unsafe static int GetBuildIndexByScenePath(string scenePath)
	{
		//The blocks IL_0029 are reachable both inside and outside the pinned region starting at IL_0018. ILSpy has duplicated these blocks in order to place them both within and outside the `fixed` statement.
		try
		{
			ManagedSpanWrapper managedSpanWrapper = default(ManagedSpanWrapper);
			if (!StringMarshaller.TryMarshalEmptyOrNullString(scenePath, ref managedSpanWrapper))
			{
				ReadOnlySpan<char> readOnlySpan = MemoryExtensions.AsSpan(scenePath);
				fixed (char* begin = readOnlySpan)
				{
					managedSpanWrapper = new ManagedSpanWrapper(begin, readOnlySpan.Length);
					return GetBuildIndexByScenePath_Injected(ref managedSpanWrapper);
				}
			}
			return GetBuildIndexByScenePath_Injected(ref managedSpanWrapper);
		}
		finally
		{
		}
	}

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern void GetScenePathByBuildIndex_Injected(int buildIndex, out ManagedSpanWrapper ret);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern int GetBuildIndexByScenePath_Injected(ref ManagedSpanWrapper scenePath);
}
