using System;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using UnityEngine.Bindings;

namespace UnityEngine.SceneManagement;

[StaticAccessor("SceneManagerBindings", StaticAccessorType.DoubleColon)]
[NativeHeader("Runtime/SceneManager/SceneManager.h")]
[NativeHeader("Runtime/Export/SceneManager/SceneManager.bindings.h")]
internal static class SceneManagerAPIInternal
{
	[MethodImpl(MethodImplOptions.InternalCall)]
	public static extern int GetNumScenesInBuildSettings();

	[NativeThrows]
	public static Scene GetSceneByBuildIndex(int buildIndex)
	{
		GetSceneByBuildIndex_Injected(buildIndex, out var ret);
		return ret;
	}

	[NativeThrows]
	public unsafe static AsyncOperation LoadSceneAsyncNameIndexInternal(string sceneName, int sceneBuildIndex, LoadSceneParameters parameters, bool mustCompleteNextFrame)
	{
		//The blocks IL_0029 are reachable both inside and outside the pinned region starting at IL_0018. ILSpy has duplicated these blocks in order to place them both within and outside the `fixed` statement.
		IntPtr intPtr = default(IntPtr);
		AsyncOperation result;
		try
		{
			ManagedSpanWrapper managedSpanWrapper = default(ManagedSpanWrapper);
			if (!StringMarshaller.TryMarshalEmptyOrNullString(sceneName, ref managedSpanWrapper))
			{
				ReadOnlySpan<char> readOnlySpan = MemoryExtensions.AsSpan(sceneName);
				fixed (char* begin = readOnlySpan)
				{
					managedSpanWrapper = new ManagedSpanWrapper(begin, readOnlySpan.Length);
					intPtr = LoadSceneAsyncNameIndexInternal_Injected(ref managedSpanWrapper, sceneBuildIndex, ref parameters, mustCompleteNextFrame);
				}
			}
			else
			{
				intPtr = LoadSceneAsyncNameIndexInternal_Injected(ref managedSpanWrapper, sceneBuildIndex, ref parameters, mustCompleteNextFrame);
			}
		}
		finally
		{
			IntPtr intPtr2 = intPtr;
			result = ((intPtr2 == (IntPtr)0) ? null : AsyncOperation.BindingsMarshaller.ConvertToManaged(intPtr2));
		}
		return result;
	}

	[NativeThrows]
	public unsafe static AsyncOperation UnloadSceneNameIndexInternal(string sceneName, int sceneBuildIndex, bool immediately, UnloadSceneOptions options, out bool outSuccess)
	{
		//The blocks IL_0029 are reachable both inside and outside the pinned region starting at IL_0018. ILSpy has duplicated these blocks in order to place them both within and outside the `fixed` statement.
		IntPtr intPtr = default(IntPtr);
		AsyncOperation result;
		try
		{
			ManagedSpanWrapper managedSpanWrapper = default(ManagedSpanWrapper);
			if (!StringMarshaller.TryMarshalEmptyOrNullString(sceneName, ref managedSpanWrapper))
			{
				ReadOnlySpan<char> readOnlySpan = MemoryExtensions.AsSpan(sceneName);
				fixed (char* begin = readOnlySpan)
				{
					managedSpanWrapper = new ManagedSpanWrapper(begin, readOnlySpan.Length);
					intPtr = UnloadSceneNameIndexInternal_Injected(ref managedSpanWrapper, sceneBuildIndex, immediately, options, out outSuccess);
				}
			}
			else
			{
				intPtr = UnloadSceneNameIndexInternal_Injected(ref managedSpanWrapper, sceneBuildIndex, immediately, options, out outSuccess);
			}
		}
		finally
		{
			IntPtr intPtr2 = intPtr;
			result = ((intPtr2 == (IntPtr)0) ? null : AsyncOperation.BindingsMarshaller.ConvertToManaged(intPtr2));
		}
		return result;
	}

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern void GetSceneByBuildIndex_Injected(int buildIndex, out Scene ret);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern IntPtr LoadSceneAsyncNameIndexInternal_Injected(ref ManagedSpanWrapper sceneName, int sceneBuildIndex, [In] ref LoadSceneParameters parameters, bool mustCompleteNextFrame);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern IntPtr UnloadSceneNameIndexInternal_Injected(ref ManagedSpanWrapper sceneName, int sceneBuildIndex, bool immediately, UnloadSceneOptions options, out bool outSuccess);
}
