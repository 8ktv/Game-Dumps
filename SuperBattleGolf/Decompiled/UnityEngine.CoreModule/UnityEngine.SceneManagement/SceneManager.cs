using System;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using Unity.Collections;
using Unity.Collections.LowLevel.Unsafe;
using UnityEngine.Bindings;
using UnityEngine.Events;
using UnityEngine.Internal;
using UnityEngine.Scripting;

namespace UnityEngine.SceneManagement;

[RequiredByNativeCode]
[NativeHeader("Runtime/Export/SceneManager/SceneManager.bindings.h")]
public class SceneManager
{
	internal static bool s_AllowLoadScene = true;

	public static extern int sceneCount
	{
		[MethodImpl(MethodImplOptions.InternalCall)]
		[StaticAccessor("GetSceneManager()", StaticAccessorType.Dot)]
		[NativeMethod("GetSceneCount")]
		[NativeHeader("Runtime/SceneManager/SceneManager.h")]
		get;
	}

	public static extern int loadedSceneCount
	{
		[MethodImpl(MethodImplOptions.InternalCall)]
		[NativeMethod("GetLoadedSceneCount")]
		[StaticAccessor("GetSceneManager()", StaticAccessorType.Dot)]
		[NativeHeader("Runtime/SceneManager/SceneManager.h")]
		get;
	}

	public static int sceneCountInBuildSettings => SceneManagerAPI.ActiveAPI.GetNumScenesInBuildSettings();

	public static event UnityAction<Scene, LoadSceneMode> sceneLoaded;

	public static event UnityAction<Scene> sceneUnloaded;

	public static event UnityAction<Scene, Scene> activeSceneChanged;

	[StaticAccessor("SceneManagerBindings", StaticAccessorType.DoubleColon)]
	internal static bool CanSetAsActiveScene(Scene scene)
	{
		return CanSetAsActiveScene_Injected(ref scene);
	}

	[StaticAccessor("SceneManagerBindings", StaticAccessorType.DoubleColon)]
	public static Scene GetActiveScene()
	{
		GetActiveScene_Injected(out var ret);
		return ret;
	}

	[NativeThrows]
	[StaticAccessor("SceneManagerBindings", StaticAccessorType.DoubleColon)]
	public static bool SetActiveScene(Scene scene)
	{
		return SetActiveScene_Injected(ref scene);
	}

	[StaticAccessor("SceneManagerBindings", StaticAccessorType.DoubleColon)]
	public unsafe static Scene GetSceneByPath(string scenePath)
	{
		//The blocks IL_0029 are reachable both inside and outside the pinned region starting at IL_0018. ILSpy has duplicated these blocks in order to place them both within and outside the `fixed` statement.
		Scene ret = default(Scene);
		Scene result;
		try
		{
			ManagedSpanWrapper managedSpanWrapper = default(ManagedSpanWrapper);
			if (!StringMarshaller.TryMarshalEmptyOrNullString(scenePath, ref managedSpanWrapper))
			{
				ReadOnlySpan<char> readOnlySpan = MemoryExtensions.AsSpan(scenePath);
				fixed (char* begin = readOnlySpan)
				{
					managedSpanWrapper = new ManagedSpanWrapper(begin, readOnlySpan.Length);
					GetSceneByPath_Injected(ref managedSpanWrapper, out ret);
				}
			}
			else
			{
				GetSceneByPath_Injected(ref managedSpanWrapper, out ret);
			}
		}
		finally
		{
			result = ret;
		}
		return result;
	}

	[StaticAccessor("SceneManagerBindings", StaticAccessorType.DoubleColon)]
	public unsafe static Scene GetSceneByName(string name)
	{
		//The blocks IL_0029 are reachable both inside and outside the pinned region starting at IL_0018. ILSpy has duplicated these blocks in order to place them both within and outside the `fixed` statement.
		Scene ret = default(Scene);
		Scene result;
		try
		{
			ManagedSpanWrapper managedSpanWrapper = default(ManagedSpanWrapper);
			if (!StringMarshaller.TryMarshalEmptyOrNullString(name, ref managedSpanWrapper))
			{
				ReadOnlySpan<char> readOnlySpan = MemoryExtensions.AsSpan(name);
				fixed (char* begin = readOnlySpan)
				{
					managedSpanWrapper = new ManagedSpanWrapper(begin, readOnlySpan.Length);
					GetSceneByName_Injected(ref managedSpanWrapper, out ret);
				}
			}
			else
			{
				GetSceneByName_Injected(ref managedSpanWrapper, out ret);
			}
		}
		finally
		{
			result = ret;
		}
		return result;
	}

	public static Scene GetSceneByBuildIndex(int buildIndex)
	{
		return SceneManagerAPI.ActiveAPI.GetSceneByBuildIndex(buildIndex);
	}

	[NativeThrows]
	[StaticAccessor("SceneManagerBindings", StaticAccessorType.DoubleColon)]
	public static Scene GetSceneAt(int index)
	{
		GetSceneAt_Injected(index, out var ret);
		return ret;
	}

	[StaticAccessor("SceneManagerBindings", StaticAccessorType.DoubleColon)]
	[NativeThrows]
	public unsafe static Scene CreateScene([NotNull] string sceneName, CreateSceneParameters parameters)
	{
		//The blocks IL_0038 are reachable both inside and outside the pinned region starting at IL_0027. ILSpy has duplicated these blocks in order to place them both within and outside the `fixed` statement.
		if (sceneName == null)
		{
			ThrowHelper.ThrowArgumentNullException(sceneName, "sceneName");
		}
		Scene ret = default(Scene);
		Scene result;
		try
		{
			ManagedSpanWrapper managedSpanWrapper = default(ManagedSpanWrapper);
			if (!StringMarshaller.TryMarshalEmptyOrNullString(sceneName, ref managedSpanWrapper))
			{
				ReadOnlySpan<char> readOnlySpan = MemoryExtensions.AsSpan(sceneName);
				fixed (char* begin = readOnlySpan)
				{
					managedSpanWrapper = new ManagedSpanWrapper(begin, readOnlySpan.Length);
					CreateScene_Injected(ref managedSpanWrapper, ref parameters, out ret);
				}
			}
			else
			{
				CreateScene_Injected(ref managedSpanWrapper, ref parameters, out ret);
			}
		}
		finally
		{
			result = ret;
		}
		return result;
	}

	[NativeThrows]
	[StaticAccessor("SceneManagerBindings", StaticAccessorType.DoubleColon)]
	private static bool UnloadSceneInternal(Scene scene, UnloadSceneOptions options)
	{
		return UnloadSceneInternal_Injected(ref scene, options);
	}

	[StaticAccessor("SceneManagerBindings", StaticAccessorType.DoubleColon)]
	[NativeThrows]
	private static AsyncOperation UnloadSceneAsyncInternal(Scene scene, UnloadSceneOptions options)
	{
		IntPtr intPtr = UnloadSceneAsyncInternal_Injected(ref scene, options);
		return (intPtr == (IntPtr)0) ? null : AsyncOperation.BindingsMarshaller.ConvertToManaged(intPtr);
	}

	private static AsyncOperation LoadSceneAsyncNameIndexInternal(string sceneName, int sceneBuildIndex, LoadSceneParameters parameters, bool mustCompleteNextFrame)
	{
		if (!s_AllowLoadScene)
		{
			return null;
		}
		return SceneManagerAPI.ActiveAPI.LoadSceneAsyncByNameOrIndex(sceneName, sceneBuildIndex, parameters, mustCompleteNextFrame);
	}

	private static AsyncOperation UnloadSceneNameIndexInternal(string sceneName, int sceneBuildIndex, bool immediately, UnloadSceneOptions options, out bool outSuccess)
	{
		if (!s_AllowLoadScene)
		{
			outSuccess = false;
			return null;
		}
		return SceneManagerAPI.ActiveAPI.UnloadSceneAsyncByNameOrIndex(sceneName, sceneBuildIndex, immediately, options, out outSuccess);
	}

	[NativeThrows]
	[StaticAccessor("SceneManagerBindings", StaticAccessorType.DoubleColon)]
	public static void MergeScenes(Scene sourceScene, Scene destinationScene)
	{
		MergeScenes_Injected(ref sourceScene, ref destinationScene);
	}

	[NativeThrows]
	[StaticAccessor("SceneManagerBindings", StaticAccessorType.DoubleColon)]
	public static void MoveGameObjectToScene([NotNull] GameObject go, Scene scene)
	{
		if ((object)go == null)
		{
			ThrowHelper.ThrowArgumentNullException(go, "go");
		}
		IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull(go);
		if (intPtr == (IntPtr)0)
		{
			ThrowHelper.ThrowArgumentNullException(go, "go");
		}
		MoveGameObjectToScene_Injected(intPtr, ref scene);
	}

	[NativeThrows]
	[StaticAccessor("SceneManagerBindings", StaticAccessorType.DoubleColon)]
	private static void MoveGameObjectsToSceneByInstanceId(IntPtr instanceIds, int instanceCount, Scene scene)
	{
		MoveGameObjectsToSceneByInstanceId_Injected(instanceIds, instanceCount, ref scene);
	}

	public unsafe static void MoveGameObjectsToScene(NativeArray<int> instanceIDs, Scene scene)
	{
		if (!instanceIDs.IsCreated)
		{
			throw new ArgumentException("NativeArray is uninitialized", "instanceIDs");
		}
		if (instanceIDs.Length != 0)
		{
			MoveGameObjectsToSceneByInstanceId((IntPtr)instanceIDs.GetUnsafeReadOnlyPtr(), instanceIDs.Length, scene);
		}
	}

	[RequiredByNativeCode]
	internal static AsyncOperation LoadFirstScene_Internal(bool async)
	{
		return SceneManagerAPI.ActiveAPI.LoadFirstScene(async);
	}

	[Obsolete("Use SceneManager.sceneCount and SceneManager.GetSceneAt(int index) to loop the all scenes instead.")]
	public static Scene[] GetAllScenes()
	{
		Scene[] array = new Scene[sceneCount];
		for (int i = 0; i < sceneCount; i++)
		{
			array[i] = GetSceneAt(i);
		}
		return array;
	}

	public static Scene CreateScene(string sceneName)
	{
		CreateSceneParameters parameters = new CreateSceneParameters(LocalPhysicsMode.None);
		return CreateScene(sceneName, parameters);
	}

	public static void LoadScene(string sceneName, [DefaultValue("LoadSceneMode.Single")] LoadSceneMode mode)
	{
		LoadSceneParameters parameters = new LoadSceneParameters(mode);
		LoadScene(sceneName, parameters);
	}

	[ExcludeFromDocs]
	public static void LoadScene(string sceneName)
	{
		LoadSceneParameters parameters = new LoadSceneParameters(LoadSceneMode.Single);
		LoadScene(sceneName, parameters);
	}

	public static Scene LoadScene(string sceneName, LoadSceneParameters parameters)
	{
		LoadSceneAsyncNameIndexInternal(sceneName, -1, parameters, mustCompleteNextFrame: true);
		return GetSceneAt(sceneCount - 1);
	}

	public static void LoadScene(int sceneBuildIndex, [DefaultValue("LoadSceneMode.Single")] LoadSceneMode mode)
	{
		LoadSceneParameters parameters = new LoadSceneParameters(mode);
		LoadScene(sceneBuildIndex, parameters);
	}

	[ExcludeFromDocs]
	public static void LoadScene(int sceneBuildIndex)
	{
		LoadSceneParameters parameters = new LoadSceneParameters(LoadSceneMode.Single);
		LoadScene(sceneBuildIndex, parameters);
	}

	public static Scene LoadScene(int sceneBuildIndex, LoadSceneParameters parameters)
	{
		LoadSceneAsyncNameIndexInternal(null, sceneBuildIndex, parameters, mustCompleteNextFrame: true);
		return GetSceneAt(sceneCount - 1);
	}

	public static AsyncOperation LoadSceneAsync(int sceneBuildIndex, [DefaultValue("LoadSceneMode.Single")] LoadSceneMode mode)
	{
		LoadSceneParameters parameters = new LoadSceneParameters(mode);
		return LoadSceneAsync(sceneBuildIndex, parameters);
	}

	[ExcludeFromDocs]
	public static AsyncOperation LoadSceneAsync(int sceneBuildIndex)
	{
		LoadSceneParameters parameters = new LoadSceneParameters(LoadSceneMode.Single);
		return LoadSceneAsync(sceneBuildIndex, parameters);
	}

	public static AsyncOperation LoadSceneAsync(int sceneBuildIndex, LoadSceneParameters parameters)
	{
		return LoadSceneAsyncNameIndexInternal(null, sceneBuildIndex, parameters, mustCompleteNextFrame: false);
	}

	public static AsyncOperation LoadSceneAsync(string sceneName, [DefaultValue("LoadSceneMode.Single")] LoadSceneMode mode)
	{
		LoadSceneParameters parameters = new LoadSceneParameters(mode);
		return LoadSceneAsync(sceneName, parameters);
	}

	[ExcludeFromDocs]
	public static AsyncOperation LoadSceneAsync(string sceneName)
	{
		LoadSceneParameters parameters = new LoadSceneParameters(LoadSceneMode.Single);
		return LoadSceneAsync(sceneName, parameters);
	}

	public static AsyncOperation LoadSceneAsync(string sceneName, LoadSceneParameters parameters)
	{
		return LoadSceneAsyncNameIndexInternal(sceneName, -1, parameters, mustCompleteNextFrame: false);
	}

	[Obsolete("Use SceneManager.UnloadSceneAsync. This function is not safe to use during triggers and under other circumstances. See Scripting reference for more details.")]
	public static bool UnloadScene(Scene scene)
	{
		return UnloadSceneInternal(scene, UnloadSceneOptions.None);
	}

	[Obsolete("Use SceneManager.UnloadSceneAsync. This function is not safe to use during triggers and under other circumstances. See Scripting reference for more details.")]
	public static bool UnloadScene(int sceneBuildIndex)
	{
		UnloadSceneNameIndexInternal("", sceneBuildIndex, immediately: true, UnloadSceneOptions.None, out var outSuccess);
		return outSuccess;
	}

	[Obsolete("Use SceneManager.UnloadSceneAsync. This function is not safe to use during triggers and under other circumstances. See Scripting reference for more details.")]
	public static bool UnloadScene(string sceneName)
	{
		UnloadSceneNameIndexInternal(sceneName, -1, immediately: true, UnloadSceneOptions.None, out var outSuccess);
		return outSuccess;
	}

	public static AsyncOperation UnloadSceneAsync(int sceneBuildIndex)
	{
		bool outSuccess;
		return UnloadSceneNameIndexInternal("", sceneBuildIndex, immediately: false, UnloadSceneOptions.None, out outSuccess);
	}

	public static AsyncOperation UnloadSceneAsync(string sceneName)
	{
		bool outSuccess;
		return UnloadSceneNameIndexInternal(sceneName, -1, immediately: false, UnloadSceneOptions.None, out outSuccess);
	}

	public static AsyncOperation UnloadSceneAsync(Scene scene)
	{
		return UnloadSceneAsyncInternal(scene, UnloadSceneOptions.None);
	}

	public static AsyncOperation UnloadSceneAsync(int sceneBuildIndex, UnloadSceneOptions options)
	{
		bool outSuccess;
		return UnloadSceneNameIndexInternal("", sceneBuildIndex, immediately: false, options, out outSuccess);
	}

	public static AsyncOperation UnloadSceneAsync(string sceneName, UnloadSceneOptions options)
	{
		bool outSuccess;
		return UnloadSceneNameIndexInternal(sceneName, -1, immediately: false, options, out outSuccess);
	}

	public static AsyncOperation UnloadSceneAsync(Scene scene, UnloadSceneOptions options)
	{
		return UnloadSceneAsyncInternal(scene, options);
	}

	[RequiredByNativeCode]
	private static void Internal_SceneLoaded(Scene scene, LoadSceneMode mode)
	{
		if (SceneManager.sceneLoaded != null)
		{
			SceneManager.sceneLoaded(scene, mode);
		}
	}

	[RequiredByNativeCode]
	private static void Internal_SceneUnloaded(Scene scene)
	{
		if (SceneManager.sceneUnloaded != null)
		{
			SceneManager.sceneUnloaded(scene);
		}
	}

	[RequiredByNativeCode]
	private static void Internal_ActiveSceneChanged(Scene previousActiveScene, Scene newActiveScene)
	{
		if (SceneManager.activeSceneChanged != null)
		{
			SceneManager.activeSceneChanged(previousActiveScene, newActiveScene);
		}
	}

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern bool CanSetAsActiveScene_Injected([In] ref Scene scene);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern void GetActiveScene_Injected(out Scene ret);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern bool SetActiveScene_Injected([In] ref Scene scene);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern void GetSceneByPath_Injected(ref ManagedSpanWrapper scenePath, out Scene ret);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern void GetSceneByName_Injected(ref ManagedSpanWrapper name, out Scene ret);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern void GetSceneAt_Injected(int index, out Scene ret);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern void CreateScene_Injected(ref ManagedSpanWrapper sceneName, [In] ref CreateSceneParameters parameters, out Scene ret);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern bool UnloadSceneInternal_Injected([In] ref Scene scene, UnloadSceneOptions options);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern IntPtr UnloadSceneAsyncInternal_Injected([In] ref Scene scene, UnloadSceneOptions options);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern void MergeScenes_Injected([In] ref Scene sourceScene, [In] ref Scene destinationScene);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern void MoveGameObjectToScene_Injected(IntPtr go, [In] ref Scene scene);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern void MoveGameObjectsToSceneByInstanceId_Injected(IntPtr instanceIds, int instanceCount, [In] ref Scene scene);
}
