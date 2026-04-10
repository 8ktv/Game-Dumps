using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using UnityEngine.Bindings;

namespace UnityEngine.SceneManagement;

[Serializable]
[NativeHeader("Runtime/Export/SceneManager/Scene.bindings.h")]
public struct Scene
{
	internal enum LoadingState
	{
		NotLoaded,
		Loading,
		Loaded,
		Unloading
	}

	[SerializeField]
	[HideInInspector]
	private SceneHandle m_Handle;

	public SceneHandle handle => m_Handle;

	internal LoadingState loadingState => GetLoadingStateInternal(handle);

	internal string guid => GetGUIDInternal(handle);

	public string path => GetPathInternal(handle);

	public string name
	{
		get
		{
			return GetNameInternal(handle);
		}
		set
		{
			SetNameInternal(handle, value);
		}
	}

	public bool isLoaded => GetIsLoadedInternal(handle);

	public int buildIndex => GetBuildIndexInternal(handle);

	public bool isDirty => GetIsDirtyInternal(handle);

	internal int dirtyID => GetDirtyID(handle);

	public int rootCount => GetRootCountInternal(handle);

	public bool isSubScene
	{
		get
		{
			return IsSubScene(handle);
		}
		set
		{
			SetIsSubScene(handle, value);
		}
	}

	internal EntityId defaultParent
	{
		get
		{
			return GetDefaultParent(handle);
		}
		set
		{
			SetDefaultParent(handle, value);
		}
	}

	[StaticAccessor("SceneBindings", StaticAccessorType.DoubleColon)]
	private static bool IsValidInternal(SceneHandle sceneHandle)
	{
		return IsValidInternal_Injected(ref sceneHandle);
	}

	[StaticAccessor("SceneBindings", StaticAccessorType.DoubleColon)]
	private static string GetPathInternal(SceneHandle sceneHandle)
	{
		ManagedSpanWrapper ret = default(ManagedSpanWrapper);
		string stringAndDispose;
		try
		{
			GetPathInternal_Injected(ref sceneHandle, out ret);
		}
		finally
		{
			stringAndDispose = OutStringMarshaller.GetStringAndDispose(ret);
		}
		return stringAndDispose;
	}

	[StaticAccessor("SceneBindings", StaticAccessorType.DoubleColon)]
	private unsafe static void SetPathAndGUIDInternal(SceneHandle sceneHandle, string path, string guid)
	{
		//The blocks IL_002b, IL_0038, IL_0046, IL_0054, IL_0059 are reachable both inside and outside the pinned region starting at IL_001a. ILSpy has duplicated these blocks in order to place them both within and outside the `fixed` statement.
		//The blocks IL_0059 are reachable both inside and outside the pinned region starting at IL_0046. ILSpy has duplicated these blocks in order to place them both within and outside the `fixed` statement.
		//The blocks IL_0059 are reachable both inside and outside the pinned region starting at IL_0046. ILSpy has duplicated these blocks in order to place them both within and outside the `fixed` statement.
		try
		{
			ManagedSpanWrapper managedSpanWrapper = default(ManagedSpanWrapper);
			ref ManagedSpanWrapper reference;
			ManagedSpanWrapper managedSpanWrapper2 = default(ManagedSpanWrapper);
			ReadOnlySpan<char> readOnlySpan2;
			if (!StringMarshaller.TryMarshalEmptyOrNullString(path, ref managedSpanWrapper))
			{
				ReadOnlySpan<char> readOnlySpan = MemoryExtensions.AsSpan(path);
				fixed (char* begin = readOnlySpan)
				{
					managedSpanWrapper = new ManagedSpanWrapper(begin, readOnlySpan.Length);
					reference = ref managedSpanWrapper;
					if (!StringMarshaller.TryMarshalEmptyOrNullString(guid, ref managedSpanWrapper2))
					{
						readOnlySpan2 = MemoryExtensions.AsSpan(guid);
						fixed (char* begin2 = readOnlySpan2)
						{
							managedSpanWrapper2 = new ManagedSpanWrapper(begin2, readOnlySpan2.Length);
							SetPathAndGUIDInternal_Injected(ref sceneHandle, ref reference, ref managedSpanWrapper2);
							return;
						}
					}
					SetPathAndGUIDInternal_Injected(ref sceneHandle, ref reference, ref managedSpanWrapper2);
					return;
				}
			}
			reference = ref managedSpanWrapper;
			if (!StringMarshaller.TryMarshalEmptyOrNullString(guid, ref managedSpanWrapper2))
			{
				readOnlySpan2 = MemoryExtensions.AsSpan(guid);
				fixed (char* begin2 = readOnlySpan2)
				{
					managedSpanWrapper2 = new ManagedSpanWrapper(begin2, readOnlySpan2.Length);
					SetPathAndGUIDInternal_Injected(ref sceneHandle, ref reference, ref managedSpanWrapper2);
					return;
				}
			}
			SetPathAndGUIDInternal_Injected(ref sceneHandle, ref reference, ref managedSpanWrapper2);
		}
		finally
		{
		}
	}

	[StaticAccessor("SceneBindings", StaticAccessorType.DoubleColon)]
	private static string GetNameInternal(SceneHandle sceneHandle)
	{
		ManagedSpanWrapper ret = default(ManagedSpanWrapper);
		string stringAndDispose;
		try
		{
			GetNameInternal_Injected(ref sceneHandle, out ret);
		}
		finally
		{
			stringAndDispose = OutStringMarshaller.GetStringAndDispose(ret);
		}
		return stringAndDispose;
	}

	[StaticAccessor("SceneBindings", StaticAccessorType.DoubleColon)]
	[NativeThrows]
	private unsafe static void SetNameInternal(SceneHandle sceneHandle, string name)
	{
		//The blocks IL_002b are reachable both inside and outside the pinned region starting at IL_001a. ILSpy has duplicated these blocks in order to place them both within and outside the `fixed` statement.
		try
		{
			ManagedSpanWrapper managedSpanWrapper = default(ManagedSpanWrapper);
			if (!StringMarshaller.TryMarshalEmptyOrNullString(name, ref managedSpanWrapper))
			{
				ReadOnlySpan<char> readOnlySpan = MemoryExtensions.AsSpan(name);
				fixed (char* begin = readOnlySpan)
				{
					managedSpanWrapper = new ManagedSpanWrapper(begin, readOnlySpan.Length);
					SetNameInternal_Injected(ref sceneHandle, ref managedSpanWrapper);
					return;
				}
			}
			SetNameInternal_Injected(ref sceneHandle, ref managedSpanWrapper);
		}
		finally
		{
		}
	}

	[StaticAccessor("SceneBindings", StaticAccessorType.DoubleColon)]
	private static string GetGUIDInternal(SceneHandle sceneHandle)
	{
		ManagedSpanWrapper ret = default(ManagedSpanWrapper);
		string stringAndDispose;
		try
		{
			GetGUIDInternal_Injected(ref sceneHandle, out ret);
		}
		finally
		{
			stringAndDispose = OutStringMarshaller.GetStringAndDispose(ret);
		}
		return stringAndDispose;
	}

	[StaticAccessor("SceneBindings", StaticAccessorType.DoubleColon)]
	private static bool IsSubScene(SceneHandle sceneHandle)
	{
		return IsSubScene_Injected(ref sceneHandle);
	}

	[StaticAccessor("SceneBindings", StaticAccessorType.DoubleColon)]
	private static void SetIsSubScene(SceneHandle sceneHandle, bool value)
	{
		SetIsSubScene_Injected(ref sceneHandle, value);
	}

	[StaticAccessor("SceneBindings", StaticAccessorType.DoubleColon)]
	private static bool GetIsLoadedInternal(SceneHandle sceneHandle)
	{
		return GetIsLoadedInternal_Injected(ref sceneHandle);
	}

	[StaticAccessor("SceneBindings", StaticAccessorType.DoubleColon)]
	private static LoadingState GetLoadingStateInternal(SceneHandle sceneHandle)
	{
		return GetLoadingStateInternal_Injected(ref sceneHandle);
	}

	[StaticAccessor("SceneBindings", StaticAccessorType.DoubleColon)]
	private static bool GetIsDirtyInternal(SceneHandle sceneHandle)
	{
		return GetIsDirtyInternal_Injected(ref sceneHandle);
	}

	[StaticAccessor("SceneBindings", StaticAccessorType.DoubleColon)]
	private static int GetDirtyID(SceneHandle sceneHandle)
	{
		return GetDirtyID_Injected(ref sceneHandle);
	}

	[StaticAccessor("SceneBindings", StaticAccessorType.DoubleColon)]
	private static int GetBuildIndexInternal(SceneHandle sceneHandle)
	{
		return GetBuildIndexInternal_Injected(ref sceneHandle);
	}

	[StaticAccessor("SceneBindings", StaticAccessorType.DoubleColon)]
	private static int GetRootCountInternal(SceneHandle sceneHandle)
	{
		return GetRootCountInternal_Injected(ref sceneHandle);
	}

	[StaticAccessor("SceneBindings", StaticAccessorType.DoubleColon)]
	private static void GetRootGameObjectsInternal(SceneHandle sceneHandle, object resultRootList)
	{
		GetRootGameObjectsInternal_Injected(ref sceneHandle, resultRootList);
	}

	[StaticAccessor("SceneBindings", StaticAccessorType.DoubleColon)]
	private static EntityId GetDefaultParent(SceneHandle sceneHandle)
	{
		GetDefaultParent_Injected(ref sceneHandle, out var ret);
		return ret;
	}

	[StaticAccessor("SceneBindings", StaticAccessorType.DoubleColon)]
	private static void SetDefaultParent(SceneHandle sceneHandle, EntityId value)
	{
		SetDefaultParent_Injected(ref sceneHandle, ref value);
	}

	internal Scene(SceneHandle handle)
	{
		m_Handle = handle;
	}

	public bool IsValid()
	{
		return IsValidInternal(handle);
	}

	public GameObject[] GetRootGameObjects()
	{
		List<GameObject> list = new List<GameObject>(rootCount);
		GetRootGameObjects(list);
		return list.ToArray();
	}

	public void GetRootGameObjects(List<GameObject> rootGameObjects)
	{
		if (rootGameObjects.Capacity < rootCount)
		{
			rootGameObjects.Capacity = rootCount;
		}
		rootGameObjects.Clear();
		if (!IsValid())
		{
			throw new ArgumentException("The scene is invalid.");
		}
		if (!Application.isPlaying && !isLoaded)
		{
			throw new ArgumentException("The scene is not loaded.");
		}
		if (rootCount != 0)
		{
			GetRootGameObjectsInternal(handle, rootGameObjects);
		}
	}

	public static bool operator ==(Scene lhs, Scene rhs)
	{
		return lhs.handle == rhs.handle;
	}

	public static bool operator !=(Scene lhs, Scene rhs)
	{
		return lhs.handle != rhs.handle;
	}

	public override int GetHashCode()
	{
		return m_Handle.GetHashCode();
	}

	public override bool Equals(object other)
	{
		if (!(other is Scene scene))
		{
			return false;
		}
		return handle == scene.handle;
	}

	internal void SetPathAndGuid(string path, string guid)
	{
		SetPathAndGUIDInternal(m_Handle, path, guid);
	}

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern bool IsValidInternal_Injected([In] ref SceneHandle sceneHandle);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern void GetPathInternal_Injected([In] ref SceneHandle sceneHandle, out ManagedSpanWrapper ret);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern void SetPathAndGUIDInternal_Injected([In] ref SceneHandle sceneHandle, ref ManagedSpanWrapper path, ref ManagedSpanWrapper guid);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern void GetNameInternal_Injected([In] ref SceneHandle sceneHandle, out ManagedSpanWrapper ret);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern void SetNameInternal_Injected([In] ref SceneHandle sceneHandle, ref ManagedSpanWrapper name);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern void GetGUIDInternal_Injected([In] ref SceneHandle sceneHandle, out ManagedSpanWrapper ret);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern bool IsSubScene_Injected([In] ref SceneHandle sceneHandle);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern void SetIsSubScene_Injected([In] ref SceneHandle sceneHandle, bool value);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern bool GetIsLoadedInternal_Injected([In] ref SceneHandle sceneHandle);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern LoadingState GetLoadingStateInternal_Injected([In] ref SceneHandle sceneHandle);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern bool GetIsDirtyInternal_Injected([In] ref SceneHandle sceneHandle);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern int GetDirtyID_Injected([In] ref SceneHandle sceneHandle);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern int GetBuildIndexInternal_Injected([In] ref SceneHandle sceneHandle);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern int GetRootCountInternal_Injected([In] ref SceneHandle sceneHandle);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern void GetRootGameObjectsInternal_Injected([In] ref SceneHandle sceneHandle, object resultRootList);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern void GetDefaultParent_Injected([In] ref SceneHandle sceneHandle, out EntityId ret);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern void SetDefaultParent_Injected([In] ref SceneHandle sceneHandle, [In] ref EntityId value);
}
