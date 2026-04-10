#define UNITY_ASSERTIONS
using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using Unity.Collections;
using Unity.Collections.LowLevel.Unsafe;
using UnityEngine.Assertions;
using UnityEngine.Bindings;
using UnityEngineInternal;

namespace UnityEngine;

[NativeHeader("Runtime/Misc/ResourceManagerUtility.h")]
[NativeHeader("Runtime/Export/Resources/Resources.bindings.h")]
public sealed class Resources
{
	internal static T[] ConvertObjects<T>(Object[] rawObjects) where T : Object
	{
		if (rawObjects == null)
		{
			return null;
		}
		T[] array = new T[rawObjects.Length];
		for (int i = 0; i < array.Length; i++)
		{
			array[i] = (T)rawObjects[i];
		}
		return array;
	}

	public static Object[] FindObjectsOfTypeAll(Type type)
	{
		return ResourcesAPI.ActiveAPI.FindObjectsOfTypeAll(type);
	}

	public static T[] FindObjectsOfTypeAll<T>() where T : Object
	{
		return ConvertObjects<T>(FindObjectsOfTypeAll(typeof(T)));
	}

	public static Object Load(string path)
	{
		return Load(path, typeof(Object));
	}

	public static T Load<T>(string path) where T : Object
	{
		return (T)Load(path, typeof(T));
	}

	public static Object Load(string path, Type systemTypeInstance)
	{
		return ResourcesAPI.ActiveAPI.Load(path, systemTypeInstance);
	}

	public static ResourceRequest LoadAsync(string path)
	{
		return LoadAsync(path, typeof(Object));
	}

	public static ResourceRequest LoadAsync<T>(string path) where T : Object
	{
		return LoadAsync(path, typeof(T));
	}

	public static ResourceRequest LoadAsync(string path, Type type)
	{
		return ResourcesAPI.ActiveAPI.LoadAsync(path, type);
	}

	public static Object[] LoadAll(string path, Type systemTypeInstance)
	{
		return ResourcesAPI.ActiveAPI.LoadAll(path, systemTypeInstance);
	}

	public static Object[] LoadAll(string path)
	{
		return LoadAll(path, typeof(Object));
	}

	public static T[] LoadAll<T>(string path) where T : Object
	{
		return ConvertObjects<T>(LoadAll(path, typeof(T)));
	}

	[FreeFunction("GetScriptingBuiltinResource", ThrowsException = true)]
	[TypeInferenceRule(TypeInferenceRules.TypeReferencedByFirstArgument)]
	public unsafe static Object GetBuiltinResource([NotNull] Type type, string path)
	{
		//The blocks IL_0039 are reachable both inside and outside the pinned region starting at IL_0028. ILSpy has duplicated these blocks in order to place them both within and outside the `fixed` statement.
		if ((object)type == null)
		{
			ThrowHelper.ThrowArgumentNullException(type, "type");
		}
		IntPtr builtinResource_Injected = default(IntPtr);
		Object result;
		try
		{
			ManagedSpanWrapper managedSpanWrapper = default(ManagedSpanWrapper);
			if (!StringMarshaller.TryMarshalEmptyOrNullString(path, ref managedSpanWrapper))
			{
				ReadOnlySpan<char> readOnlySpan = MemoryExtensions.AsSpan(path);
				fixed (char* begin = readOnlySpan)
				{
					managedSpanWrapper = new ManagedSpanWrapper(begin, readOnlySpan.Length);
					builtinResource_Injected = GetBuiltinResource_Injected(type, ref managedSpanWrapper);
				}
			}
			else
			{
				builtinResource_Injected = GetBuiltinResource_Injected(type, ref managedSpanWrapper);
			}
		}
		finally
		{
			result = Unmarshal.UnmarshalUnityObject<Object>(builtinResource_Injected);
		}
		return result;
	}

	public static T GetBuiltinResource<T>(string path) where T : Object
	{
		return (T)GetBuiltinResource(typeof(T), path);
	}

	public static void UnloadAsset(Object assetToUnload)
	{
		ResourcesAPI.ActiveAPI.UnloadAsset(assetToUnload);
	}

	[FreeFunction("Scripting::UnloadAssetFromScripting")]
	private static void UnloadAssetImplResourceManager(Object assetToUnload)
	{
		UnloadAssetImplResourceManager_Injected(Object.MarshalledUnityObject.Marshal(assetToUnload));
	}

	[FreeFunction("Resources_Bindings::UnloadUnusedAssets")]
	public static AsyncOperation UnloadUnusedAssets()
	{
		IntPtr intPtr = UnloadUnusedAssets_Injected();
		return (intPtr == (IntPtr)0) ? null : AsyncOperation.BindingsMarshaller.ConvertToManaged(intPtr);
	}

	[FreeFunction("Resources_Bindings::InstanceIDToObject")]
	public static Object EntityIdToObject(EntityId entityId)
	{
		return Unmarshal.UnmarshalUnityObject<Object>(EntityIdToObject_Injected(ref entityId));
	}

	[Obsolete("InstanceIDToObject is obsolete. Use EntityIdToObject instead.")]
	public static Object InstanceIDToObject(int instanceID)
	{
		return EntityIdToObject(instanceID);
	}

	[FreeFunction("Resources_Bindings::IsInstanceLoaded")]
	internal static bool IsObjectLoaded(EntityId entityId)
	{
		return IsObjectLoaded_Injected(ref entityId);
	}

	internal static bool IsInstanceLoaded(int instanceID)
	{
		return IsObjectLoaded(instanceID);
	}

	[MethodImpl(MethodImplOptions.InternalCall)]
	[FreeFunction("Resources_Bindings::InstanceIDToObjectList", IsThreadSafe = true)]
	private static extern void EntityIdsToObjectList(IntPtr entityIds, int instanceCount, List<Object> objects);

	public unsafe static void EntityIdsToObjectList(NativeArray<EntityId> entityIds, List<Object> objects)
	{
		if (!entityIds.IsCreated)
		{
			throw new ArgumentException("NativeArray is uninitialized", "entityIds");
		}
		if (objects == null)
		{
			throw new ArgumentNullException("objects");
		}
		if (entityIds.Length == 0)
		{
			objects.Clear();
		}
		else
		{
			EntityIdsToObjectList((IntPtr)entityIds.GetUnsafeReadOnlyPtr(), entityIds.Length, objects);
		}
	}

	[Obsolete("InstanceIDToObjectList is obsolete. Use EntityIdsToObjectList instead.")]
	public unsafe static void InstanceIDToObjectList(NativeArray<int> instanceIDs, List<Object> objects)
	{
		Debug.Assert(4 == sizeof(EntityId), "Update this path to 64bit when we support 64bit");
		if (!instanceIDs.IsCreated)
		{
			throw new ArgumentException("NativeArray is uninitialized", "instanceIDs");
		}
		if (objects == null)
		{
			throw new ArgumentNullException("objects");
		}
		if (instanceIDs.Length == 0)
		{
			objects.Clear();
		}
		else
		{
			EntityIdsToObjectList((IntPtr)instanceIDs.GetUnsafeReadOnlyPtr(), instanceIDs.Length, objects);
		}
	}

	[MethodImpl(MethodImplOptions.InternalCall)]
	[FreeFunction("Resources_Bindings::InstanceIDsToValidArray", IsThreadSafe = true)]
	private static extern void InstanceIDsToValidArray_Internal(IntPtr instanceIDs, int instanceCount, IntPtr validArray, int validArrayCount);

	[FreeFunction("Resources_Bindings::DoesObjectWithInstanceIDExist", IsThreadSafe = true)]
	public static bool EntityIdIsValid(EntityId entityId)
	{
		return EntityIdIsValid_Injected(ref entityId);
	}

	[Obsolete("InstanceIDIsValid is obsolete. Use EntityIdIsValid instead.")]
	public static bool InstanceIDIsValid(int instanceId)
	{
		return EntityIdIsValid(instanceId);
	}

	[Obsolete("InstanceIDsToValidArray is obsolete. Use EntityIdsToValidArray instead.")]
	public unsafe static void InstanceIDsToValidArray(NativeArray<int> instanceIDs, NativeArray<bool> validArray)
	{
		if (!instanceIDs.IsCreated)
		{
			throw new ArgumentException("NativeArray is uninitialized", "instanceIDs");
		}
		if (!validArray.IsCreated)
		{
			throw new ArgumentException("NativeArray is uninitialized", "validArray");
		}
		if (instanceIDs.Length != validArray.Length)
		{
			throw new ArgumentException("Size mismatch! Both arrays must be the same length.");
		}
		if (instanceIDs.Length != 0)
		{
			Assert.AreEqual(4, sizeof(EntityId));
			InstanceIDsToValidArray_Internal((IntPtr)instanceIDs.GetUnsafeReadOnlyPtr(), instanceIDs.Length, (IntPtr)validArray.GetUnsafePtr(), validArray.Length);
		}
	}

	public unsafe static void EntityIdsToValidArray(NativeArray<EntityId> entityIDs, NativeArray<bool> validArray)
	{
		if (!entityIDs.IsCreated)
		{
			throw new ArgumentException("NativeArray is uninitialized", "entityIDs");
		}
		if (!validArray.IsCreated)
		{
			throw new ArgumentException("NativeArray is uninitialized", "validArray");
		}
		if (entityIDs.Length != validArray.Length)
		{
			throw new ArgumentException("Size mismatch! Both arrays must be the same length.");
		}
		if (entityIDs.Length != 0)
		{
			InstanceIDsToValidArray_Internal((IntPtr)entityIDs.GetUnsafeReadOnlyPtr(), entityIDs.Length, (IntPtr)validArray.GetUnsafePtr(), validArray.Length);
		}
	}

	public unsafe static void InstanceIDsToValidArray(ReadOnlySpan<int> instanceIDs, Span<bool> validArray)
	{
		if (instanceIDs.Length != validArray.Length)
		{
			throw new ArgumentException("Size mismatch! Both arrays must be the same length.");
		}
		if (instanceIDs.Length == 0)
		{
			return;
		}
		Assert.AreEqual(4, sizeof(EntityId));
		fixed (int* ptr = instanceIDs)
		{
			fixed (bool* ptr2 = validArray)
			{
				InstanceIDsToValidArray_Internal((IntPtr)ptr, instanceIDs.Length, (IntPtr)ptr2, validArray.Length);
			}
		}
	}

	public unsafe static void EntityIdsToValidArray(ReadOnlySpan<EntityId> entityIds, Span<bool> validArray)
	{
		if (entityIds.Length != validArray.Length)
		{
			throw new ArgumentException("Size mismatch! Both arrays must be the same length.");
		}
		if (entityIds.Length == 0)
		{
			return;
		}
		fixed (EntityId* ptr = entityIds)
		{
			fixed (bool* ptr2 = validArray)
			{
				InstanceIDsToValidArray_Internal((IntPtr)ptr, entityIds.Length, (IntPtr)ptr2, validArray.Length);
			}
		}
	}

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern IntPtr GetBuiltinResource_Injected(Type type, ref ManagedSpanWrapper path);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern void UnloadAssetImplResourceManager_Injected(IntPtr assetToUnload);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern IntPtr UnloadUnusedAssets_Injected();

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern IntPtr EntityIdToObject_Injected([In] ref EntityId entityId);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern bool IsObjectLoaded_Injected([In] ref EntityId entityId);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern bool EntityIdIsValid_Injected([In] ref EntityId entityId);
}
