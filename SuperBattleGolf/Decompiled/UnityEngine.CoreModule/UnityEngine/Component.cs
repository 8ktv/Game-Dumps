using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Security;
using UnityEngine.Bindings;
using UnityEngine.Internal;
using UnityEngine.Scripting;
using UnityEngineInternal;

namespace UnityEngine;

[NativeHeader("Runtime/Export/Scripting/Component.bindings.h")]
[NativeClass("Unity::Component")]
[RequiredByNativeCode]
public class Component : Object
{
	public Transform transform
	{
		[FreeFunction("GetTransform", HasExplicitThis = true, ThrowsException = true)]
		get
		{
			IntPtr intPtr = MarshalledUnityObject.MarshalNotNull(this);
			if (intPtr == (IntPtr)0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			return Unmarshal.UnmarshalUnityObject<Transform>(get_transform_Injected(intPtr));
		}
	}

	public GameObject gameObject
	{
		[FreeFunction("GetGameObject", HasExplicitThis = true)]
		get
		{
			IntPtr intPtr = MarshalledUnityObject.MarshalNotNull(this);
			if (intPtr == (IntPtr)0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			return Unmarshal.UnmarshalUnityObject<GameObject>(get_gameObject_Injected(intPtr));
		}
	}

	public string tag
	{
		get
		{
			return gameObject.tag;
		}
		set
		{
			gameObject.tag = value;
		}
	}

	[TypeInferenceRule(TypeInferenceRules.TypeReferencedByFirstArgument)]
	public Component GetComponent(Type type)
	{
		return gameObject.GetComponent(type);
	}

	[FreeFunction(HasExplicitThis = true, ThrowsException = true)]
	internal void GetComponentFastPath(Type type, IntPtr oneFurtherThanResultValue)
	{
		IntPtr intPtr = MarshalledUnityObject.MarshalNotNull(this);
		if (intPtr == (IntPtr)0)
		{
			ThrowHelper.ThrowNullReferenceException(this);
		}
		GetComponentFastPath_Injected(intPtr, type, oneFurtherThanResultValue);
	}

	[SecuritySafeCritical]
	public unsafe T GetComponent<T>()
	{
		CastHelper<T> castHelper = default(CastHelper<T>);
		GetComponentFastPath(typeof(T), new IntPtr(&castHelper.onePointerFurtherThanT));
		return castHelper.t;
	}

	[TypeInferenceRule(TypeInferenceRules.TypeReferencedByFirstArgument)]
	public bool TryGetComponent(Type type, out Component component)
	{
		return gameObject.TryGetComponent(type, out component);
	}

	[SecuritySafeCritical]
	public bool TryGetComponent<T>(out T component)
	{
		return gameObject.TryGetComponent<T>(out component);
	}

	[FreeFunction(HasExplicitThis = true)]
	public unsafe Component GetComponent(string type)
	{
		//The blocks IL_0039 are reachable both inside and outside the pinned region starting at IL_0028. ILSpy has duplicated these blocks in order to place them both within and outside the `fixed` statement.
		IntPtr component_Injected = default(IntPtr);
		Component result;
		try
		{
			IntPtr intPtr = MarshalledUnityObject.MarshalNotNull(this);
			if (intPtr == (IntPtr)0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			ManagedSpanWrapper managedSpanWrapper = default(ManagedSpanWrapper);
			if (!StringMarshaller.TryMarshalEmptyOrNullString(type, ref managedSpanWrapper))
			{
				ReadOnlySpan<char> readOnlySpan = MemoryExtensions.AsSpan(type);
				fixed (char* begin = readOnlySpan)
				{
					managedSpanWrapper = new ManagedSpanWrapper(begin, readOnlySpan.Length);
					component_Injected = GetComponent_Injected(intPtr, ref managedSpanWrapper);
				}
			}
			else
			{
				component_Injected = GetComponent_Injected(intPtr, ref managedSpanWrapper);
			}
		}
		finally
		{
			result = Unmarshal.UnmarshalUnityObject<Component>(component_Injected);
		}
		return result;
	}

	[TypeInferenceRule(TypeInferenceRules.TypeReferencedByFirstArgument)]
	public Component GetComponentInChildren(Type t, bool includeInactive)
	{
		return gameObject.GetComponentInChildren(t, includeInactive);
	}

	[TypeInferenceRule(TypeInferenceRules.TypeReferencedByFirstArgument)]
	public Component GetComponentInChildren(Type t)
	{
		return GetComponentInChildren(t, includeInactive: false);
	}

	public T GetComponentInChildren<T>([DefaultValue("false")] bool includeInactive)
	{
		return (T)(object)GetComponentInChildren(typeof(T), includeInactive);
	}

	[ExcludeFromDocs]
	public T GetComponentInChildren<T>()
	{
		return (T)(object)GetComponentInChildren(typeof(T), includeInactive: false);
	}

	public Component[] GetComponentsInChildren(Type t, bool includeInactive)
	{
		return gameObject.GetComponentsInChildren(t, includeInactive);
	}

	[ExcludeFromDocs]
	public Component[] GetComponentsInChildren(Type t)
	{
		return gameObject.GetComponentsInChildren(t, includeInactive: false);
	}

	public T[] GetComponentsInChildren<T>(bool includeInactive)
	{
		return gameObject.GetComponentsInChildren<T>(includeInactive);
	}

	public void GetComponentsInChildren<T>(bool includeInactive, List<T> result)
	{
		gameObject.GetComponentsInChildren(includeInactive, result);
	}

	public T[] GetComponentsInChildren<T>()
	{
		return GetComponentsInChildren<T>(includeInactive: false);
	}

	public void GetComponentsInChildren<T>(List<T> results)
	{
		GetComponentsInChildren(includeInactive: false, results);
	}

	[TypeInferenceRule(TypeInferenceRules.TypeReferencedByFirstArgument)]
	public Component GetComponentInParent(Type t, bool includeInactive)
	{
		return gameObject.GetComponentInParent(t, includeInactive);
	}

	[TypeInferenceRule(TypeInferenceRules.TypeReferencedByFirstArgument)]
	public Component GetComponentInParent(Type t)
	{
		return gameObject.GetComponentInParent(t, includeInactive: false);
	}

	public T GetComponentInParent<T>([DefaultValue("false")] bool includeInactive)
	{
		return (T)(object)GetComponentInParent(typeof(T), includeInactive);
	}

	public T GetComponentInParent<T>()
	{
		return (T)(object)GetComponentInParent(typeof(T), includeInactive: false);
	}

	public Component[] GetComponentsInParent(Type t, [DefaultValue("false")] bool includeInactive)
	{
		return gameObject.GetComponentsInParent(t, includeInactive);
	}

	[ExcludeFromDocs]
	public Component[] GetComponentsInParent(Type t)
	{
		return GetComponentsInParent(t, includeInactive: false);
	}

	public T[] GetComponentsInParent<T>(bool includeInactive)
	{
		return gameObject.GetComponentsInParent<T>(includeInactive);
	}

	public void GetComponentsInParent<T>(bool includeInactive, List<T> results)
	{
		gameObject.GetComponentsInParent(includeInactive, results);
	}

	public T[] GetComponentsInParent<T>()
	{
		return GetComponentsInParent<T>(includeInactive: false);
	}

	public Component[] GetComponents(Type type)
	{
		return gameObject.GetComponents(type);
	}

	[FreeFunction(HasExplicitThis = true, ThrowsException = true)]
	private void GetComponentsForListInternal(Type searchType, object resultList)
	{
		IntPtr intPtr = MarshalledUnityObject.MarshalNotNull(this);
		if (intPtr == (IntPtr)0)
		{
			ThrowHelper.ThrowNullReferenceException(this);
		}
		GetComponentsForListInternal_Injected(intPtr, searchType, resultList);
	}

	public void GetComponents(Type type, List<Component> results)
	{
		GetComponentsForListInternal(type, results);
	}

	public void GetComponents<T>(List<T> results)
	{
		GetComponentsForListInternal(typeof(T), results);
	}

	public T[] GetComponents<T>()
	{
		return gameObject.GetComponents<T>();
	}

	public int GetComponentIndex()
	{
		IntPtr intPtr = MarshalledUnityObject.MarshalNotNull(this);
		if (intPtr == (IntPtr)0)
		{
			ThrowHelper.ThrowNullReferenceException(this);
		}
		return GetComponentIndex_Injected(intPtr);
	}

	public bool CompareTag(string tag)
	{
		return gameObject.CompareTag(tag);
	}

	public bool CompareTag(TagHandle tag)
	{
		return gameObject.CompareTag(tag);
	}

	[FreeFunction(HasExplicitThis = true)]
	public unsafe void SendMessageUpwards(string methodName, [DefaultValue("null")] object value, [DefaultValue("SendMessageOptions.RequireReceiver")] SendMessageOptions options)
	{
		//The blocks IL_0039 are reachable both inside and outside the pinned region starting at IL_0028. ILSpy has duplicated these blocks in order to place them both within and outside the `fixed` statement.
		try
		{
			IntPtr intPtr = MarshalledUnityObject.MarshalNotNull(this);
			if (intPtr == (IntPtr)0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			ManagedSpanWrapper managedSpanWrapper = default(ManagedSpanWrapper);
			if (!StringMarshaller.TryMarshalEmptyOrNullString(methodName, ref managedSpanWrapper))
			{
				ReadOnlySpan<char> readOnlySpan = MemoryExtensions.AsSpan(methodName);
				fixed (char* begin = readOnlySpan)
				{
					managedSpanWrapper = new ManagedSpanWrapper(begin, readOnlySpan.Length);
					SendMessageUpwards_Injected(intPtr, ref managedSpanWrapper, value, options);
					return;
				}
			}
			SendMessageUpwards_Injected(intPtr, ref managedSpanWrapper, value, options);
		}
		finally
		{
		}
	}

	[ExcludeFromDocs]
	public void SendMessageUpwards(string methodName, object value)
	{
		SendMessageUpwards(methodName, value, SendMessageOptions.RequireReceiver);
	}

	[ExcludeFromDocs]
	public void SendMessageUpwards(string methodName)
	{
		SendMessageUpwards(methodName, null, SendMessageOptions.RequireReceiver);
	}

	public void SendMessageUpwards(string methodName, SendMessageOptions options)
	{
		SendMessageUpwards(methodName, null, options);
	}

	public void SendMessage(string methodName, object value)
	{
		SendMessage(methodName, value, SendMessageOptions.RequireReceiver);
	}

	public void SendMessage(string methodName)
	{
		SendMessage(methodName, null, SendMessageOptions.RequireReceiver);
	}

	[FreeFunction("SendMessage", HasExplicitThis = true)]
	public unsafe void SendMessage(string methodName, object value, SendMessageOptions options)
	{
		//The blocks IL_0039 are reachable both inside and outside the pinned region starting at IL_0028. ILSpy has duplicated these blocks in order to place them both within and outside the `fixed` statement.
		try
		{
			IntPtr intPtr = MarshalledUnityObject.MarshalNotNull(this);
			if (intPtr == (IntPtr)0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			ManagedSpanWrapper managedSpanWrapper = default(ManagedSpanWrapper);
			if (!StringMarshaller.TryMarshalEmptyOrNullString(methodName, ref managedSpanWrapper))
			{
				ReadOnlySpan<char> readOnlySpan = MemoryExtensions.AsSpan(methodName);
				fixed (char* begin = readOnlySpan)
				{
					managedSpanWrapper = new ManagedSpanWrapper(begin, readOnlySpan.Length);
					SendMessage_Injected(intPtr, ref managedSpanWrapper, value, options);
					return;
				}
			}
			SendMessage_Injected(intPtr, ref managedSpanWrapper, value, options);
		}
		finally
		{
		}
	}

	public void SendMessage(string methodName, SendMessageOptions options)
	{
		SendMessage(methodName, null, options);
	}

	[FreeFunction("BroadcastMessage", HasExplicitThis = true)]
	public unsafe void BroadcastMessage(string methodName, [DefaultValue("null")] object parameter, [DefaultValue("SendMessageOptions.RequireReceiver")] SendMessageOptions options)
	{
		//The blocks IL_0039 are reachable both inside and outside the pinned region starting at IL_0028. ILSpy has duplicated these blocks in order to place them both within and outside the `fixed` statement.
		try
		{
			IntPtr intPtr = MarshalledUnityObject.MarshalNotNull(this);
			if (intPtr == (IntPtr)0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			ManagedSpanWrapper managedSpanWrapper = default(ManagedSpanWrapper);
			if (!StringMarshaller.TryMarshalEmptyOrNullString(methodName, ref managedSpanWrapper))
			{
				ReadOnlySpan<char> readOnlySpan = MemoryExtensions.AsSpan(methodName);
				fixed (char* begin = readOnlySpan)
				{
					managedSpanWrapper = new ManagedSpanWrapper(begin, readOnlySpan.Length);
					BroadcastMessage_Injected(intPtr, ref managedSpanWrapper, parameter, options);
					return;
				}
			}
			BroadcastMessage_Injected(intPtr, ref managedSpanWrapper, parameter, options);
		}
		finally
		{
		}
	}

	[ExcludeFromDocs]
	public void BroadcastMessage(string methodName, object parameter)
	{
		BroadcastMessage(methodName, parameter, SendMessageOptions.RequireReceiver);
	}

	[ExcludeFromDocs]
	public void BroadcastMessage(string methodName)
	{
		BroadcastMessage(methodName, null, SendMessageOptions.RequireReceiver);
	}

	public void BroadcastMessage(string methodName, SendMessageOptions options)
	{
		BroadcastMessage(methodName, null, options);
	}

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern IntPtr get_transform_Injected(IntPtr _unity_self);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern IntPtr get_gameObject_Injected(IntPtr _unity_self);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern void GetComponentFastPath_Injected(IntPtr _unity_self, Type type, IntPtr oneFurtherThanResultValue);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern IntPtr GetComponent_Injected(IntPtr _unity_self, ref ManagedSpanWrapper type);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern void GetComponentsForListInternal_Injected(IntPtr _unity_self, Type searchType, object resultList);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern int GetComponentIndex_Injected(IntPtr _unity_self);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern void SendMessageUpwards_Injected(IntPtr _unity_self, ref ManagedSpanWrapper methodName, [DefaultValue("null")] object value, [DefaultValue("SendMessageOptions.RequireReceiver")] SendMessageOptions options);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern void SendMessage_Injected(IntPtr _unity_self, ref ManagedSpanWrapper methodName, object value, SendMessageOptions options);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern void BroadcastMessage_Injected(IntPtr _unity_self, ref ManagedSpanWrapper methodName, [DefaultValue("null")] object parameter, [DefaultValue("SendMessageOptions.RequireReceiver")] SendMessageOptions options);
}
