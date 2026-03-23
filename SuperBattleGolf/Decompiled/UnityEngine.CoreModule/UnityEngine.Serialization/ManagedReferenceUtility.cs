using System;
using System.Runtime.CompilerServices;
using UnityEngine.Bindings;

namespace UnityEngine.Serialization;

[NativeHeader("Runtime/Serialize/ManagedReferenceUtility.h")]
public sealed class ManagedReferenceUtility
{
	public const long RefIdUnknown = -1L;

	public const long RefIdNull = -2L;

	[NativeMethod("SetManagedReferenceIdForObject")]
	private static bool SetManagedReferenceIdForObjectInternal(Object obj, object scriptObj, long refId)
	{
		return SetManagedReferenceIdForObjectInternal_Injected(Object.MarshalledUnityObject.Marshal(obj), scriptObj, refId);
	}

	public static bool SetManagedReferenceIdForObject(Object obj, object scriptObj, long refId)
	{
		if (scriptObj == null)
		{
			return refId == -2;
		}
		Type type = scriptObj.GetType();
		if (type == typeof(Object) || type.IsSubclassOf(typeof(Object)))
		{
			throw new InvalidOperationException("Cannot assign an object deriving from UnityEngine.Object to a managed reference. This is not supported.");
		}
		return SetManagedReferenceIdForObjectInternal(obj, scriptObj, refId);
	}

	[NativeMethod("GetManagedReferenceIdForObject")]
	private static long GetManagedReferenceIdForObjectInternal(Object obj, object scriptObj)
	{
		return GetManagedReferenceIdForObjectInternal_Injected(Object.MarshalledUnityObject.Marshal(obj), scriptObj);
	}

	public static long GetManagedReferenceIdForObject(Object obj, object scriptObj)
	{
		return GetManagedReferenceIdForObjectInternal(obj, scriptObj);
	}

	[NativeMethod("GetManagedReference")]
	private static object GetManagedReferenceInternal(Object obj, long id)
	{
		return GetManagedReferenceInternal_Injected(Object.MarshalledUnityObject.Marshal(obj), id);
	}

	public static object GetManagedReference(Object obj, long id)
	{
		return GetManagedReferenceInternal(obj, id);
	}

	[NativeMethod("GetManagedReferenceIds")]
	private static long[] GetManagedReferenceIdsForObjectInternal(Object obj)
	{
		BlittableArrayWrapper ret = default(BlittableArrayWrapper);
		long[] result;
		try
		{
			GetManagedReferenceIdsForObjectInternal_Injected(Object.MarshalledUnityObject.Marshal(obj), out ret);
		}
		finally
		{
			long[] array = default(long[]);
			ret.Unmarshal(ref array);
			result = array;
		}
		return result;
	}

	public static long[] GetManagedReferenceIds(Object obj)
	{
		return GetManagedReferenceIdsForObjectInternal(obj);
	}

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern bool SetManagedReferenceIdForObjectInternal_Injected(IntPtr obj, object scriptObj, long refId);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern long GetManagedReferenceIdForObjectInternal_Injected(IntPtr obj, object scriptObj);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern object GetManagedReferenceInternal_Injected(IntPtr obj, long id);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern void GetManagedReferenceIdsForObjectInternal_Injected(IntPtr obj, out BlittableArrayWrapper ret);
}
