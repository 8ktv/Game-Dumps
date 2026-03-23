using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
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

	[HideInInspector]
	[SerializeField]
	private int m_Handle;

	public int handle => m_Handle;

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

	[MethodImpl(MethodImplOptions.InternalCall)]
	[StaticAccessor("SceneBindings", StaticAccessorType.DoubleColon)]
	private static extern bool IsValidInternal(int sceneHandle);

	[StaticAccessor("SceneBindings", StaticAccessorType.DoubleColon)]
	private static string GetPathInternal(int sceneHandle)
	{
		ManagedSpanWrapper ret = default(ManagedSpanWrapper);
		string stringAndDispose;
		try
		{
			GetPathInternal_Injected(sceneHandle, out ret);
		}
		finally
		{
			stringAndDispose = OutStringMarshaller.GetStringAndDispose(ret);
		}
		return stringAndDispose;
	}

	[StaticAccessor("SceneBindings", StaticAccessorType.DoubleColon)]
	private unsafe static void SetPathAndGUIDInternal(int sceneHandle, string path, string guid)
	{
		//The blocks IL_002a, IL_0037, IL_0045, IL_0053, IL_0058 are reachable both inside and outside the pinned region starting at IL_0019. ILSpy has duplicated these blocks in order to place them both within and outside the `fixed` statement.
		//The blocks IL_0058 are reachable both inside and outside the pinned region starting at IL_0045. ILSpy has duplicated these blocks in order to place them both within and outside the `fixed` statement.
		//The blocks IL_0058 are reachable both inside and outside the pinned region starting at IL_0045. ILSpy has duplicated these blocks in order to place them both within and outside the `fixed` statement.
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
							SetPathAndGUIDInternal_Injected(sceneHandle, ref reference, ref managedSpanWrapper2);
							return;
						}
					}
					SetPathAndGUIDInternal_Injected(sceneHandle, ref reference, ref managedSpanWrapper2);
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
					SetPathAndGUIDInternal_Injected(sceneHandle, ref reference, ref managedSpanWrapper2);
					return;
				}
			}
			SetPathAndGUIDInternal_Injected(sceneHandle, ref reference, ref managedSpanWrapper2);
		}
		finally
		{
		}
	}

	[StaticAccessor("SceneBindings", StaticAccessorType.DoubleColon)]
	private static string GetNameInternal(int sceneHandle)
	{
		ManagedSpanWrapper ret = default(ManagedSpanWrapper);
		string stringAndDispose;
		try
		{
			GetNameInternal_Injected(sceneHandle, out ret);
		}
		finally
		{
			stringAndDispose = OutStringMarshaller.GetStringAndDispose(ret);
		}
		return stringAndDispose;
	}

	[NativeThrows]
	[StaticAccessor("SceneBindings", StaticAccessorType.DoubleColon)]
	private unsafe static void SetNameInternal(int sceneHandle, string name)
	{
		//The blocks IL_002a are reachable both inside and outside the pinned region starting at IL_0019. ILSpy has duplicated these blocks in order to place them both within and outside the `fixed` statement.
		try
		{
			ManagedSpanWrapper managedSpanWrapper = default(ManagedSpanWrapper);
			if (!StringMarshaller.TryMarshalEmptyOrNullString(name, ref managedSpanWrapper))
			{
				ReadOnlySpan<char> readOnlySpan = MemoryExtensions.AsSpan(name);
				fixed (char* begin = readOnlySpan)
				{
					managedSpanWrapper = new ManagedSpanWrapper(begin, readOnlySpan.Length);
					SetNameInternal_Injected(sceneHandle, ref managedSpanWrapper);
					return;
				}
			}
			SetNameInternal_Injected(sceneHandle, ref managedSpanWrapper);
		}
		finally
		{
		}
	}

	[StaticAccessor("SceneBindings", StaticAccessorType.DoubleColon)]
	private static string GetGUIDInternal(int sceneHandle)
	{
		ManagedSpanWrapper ret = default(ManagedSpanWrapper);
		string stringAndDispose;
		try
		{
			GetGUIDInternal_Injected(sceneHandle, out ret);
		}
		finally
		{
			stringAndDispose = OutStringMarshaller.GetStringAndDispose(ret);
		}
		return stringAndDispose;
	}

	[MethodImpl(MethodImplOptions.InternalCall)]
	[StaticAccessor("SceneBindings", StaticAccessorType.DoubleColon)]
	private static extern bool IsSubScene(int sceneHandle);

	[MethodImpl(MethodImplOptions.InternalCall)]
	[StaticAccessor("SceneBindings", StaticAccessorType.DoubleColon)]
	private static extern void SetIsSubScene(int sceneHandle, bool value);

	[MethodImpl(MethodImplOptions.InternalCall)]
	[StaticAccessor("SceneBindings", StaticAccessorType.DoubleColon)]
	private static extern bool GetIsLoadedInternal(int sceneHandle);

	[MethodImpl(MethodImplOptions.InternalCall)]
	[StaticAccessor("SceneBindings", StaticAccessorType.DoubleColon)]
	private static extern LoadingState GetLoadingStateInternal(int sceneHandle);

	[MethodImpl(MethodImplOptions.InternalCall)]
	[StaticAccessor("SceneBindings", StaticAccessorType.DoubleColon)]
	private static extern bool GetIsDirtyInternal(int sceneHandle);

	[MethodImpl(MethodImplOptions.InternalCall)]
	[StaticAccessor("SceneBindings", StaticAccessorType.DoubleColon)]
	private static extern int GetDirtyID(int sceneHandle);

	[MethodImpl(MethodImplOptions.InternalCall)]
	[StaticAccessor("SceneBindings", StaticAccessorType.DoubleColon)]
	private static extern int GetBuildIndexInternal(int sceneHandle);

	[MethodImpl(MethodImplOptions.InternalCall)]
	[StaticAccessor("SceneBindings", StaticAccessorType.DoubleColon)]
	private static extern int GetRootCountInternal(int sceneHandle);

	[MethodImpl(MethodImplOptions.InternalCall)]
	[StaticAccessor("SceneBindings", StaticAccessorType.DoubleColon)]
	private static extern void GetRootGameObjectsInternal(int sceneHandle, object resultRootList);

	internal Scene(int handle)
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
		return m_Handle;
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
	private static extern void GetPathInternal_Injected(int sceneHandle, out ManagedSpanWrapper ret);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern void SetPathAndGUIDInternal_Injected(int sceneHandle, ref ManagedSpanWrapper path, ref ManagedSpanWrapper guid);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern void GetNameInternal_Injected(int sceneHandle, out ManagedSpanWrapper ret);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern void SetNameInternal_Injected(int sceneHandle, ref ManagedSpanWrapper name);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern void GetGUIDInternal_Injected(int sceneHandle, out ManagedSpanWrapper ret);
}
