using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Security;
using Unity.Collections;
using Unity.Collections.LowLevel.Unsafe;
using UnityEngine.Bindings;
using UnityEngine.Internal;
using UnityEngine.SceneManagement;
using UnityEngine.Scripting;
using UnityEngineInternal;

namespace UnityEngine;

[NativeHeader("Runtime/Export/Scripting/GameObject.bindings.h")]
[ExcludeFromPreset]
[UsedByNativeCode]
public sealed class GameObject : Object
{
	public Transform transform
	{
		[FreeFunction("GameObjectBindings::GetTransform", HasExplicitThis = true)]
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

	public int layer
	{
		get
		{
			IntPtr intPtr = MarshalledUnityObject.MarshalNotNull(this);
			if (intPtr == (IntPtr)0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			return get_layer_Injected(intPtr);
		}
		set
		{
			IntPtr intPtr = MarshalledUnityObject.MarshalNotNull(this);
			if (intPtr == (IntPtr)0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			set_layer_Injected(intPtr, value);
		}
	}

	[Obsolete("GameObject.active is obsolete. Use GameObject.SetActive(), GameObject.activeSelf or GameObject.activeInHierarchy.")]
	public bool active
	{
		[NativeMethod(Name = "IsActive")]
		get
		{
			IntPtr intPtr = MarshalledUnityObject.MarshalNotNull(this);
			if (intPtr == (IntPtr)0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			return get_active_Injected(intPtr);
		}
		[NativeMethod(Name = "SetSelfActive")]
		set
		{
			IntPtr intPtr = MarshalledUnityObject.MarshalNotNull(this);
			if (intPtr == (IntPtr)0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			set_active_Injected(intPtr, value);
		}
	}

	public bool activeSelf
	{
		[NativeMethod(Name = "IsSelfActive")]
		get
		{
			IntPtr intPtr = MarshalledUnityObject.MarshalNotNull(this);
			if (intPtr == (IntPtr)0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			return get_activeSelf_Injected(intPtr);
		}
	}

	public bool activeInHierarchy
	{
		[NativeMethod(Name = "IsActive")]
		get
		{
			IntPtr intPtr = MarshalledUnityObject.MarshalNotNull(this);
			if (intPtr == (IntPtr)0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			return get_activeInHierarchy_Injected(intPtr);
		}
	}

	public bool isStatic
	{
		[NativeMethod(Name = "GetIsStaticDeprecated")]
		get
		{
			IntPtr intPtr = MarshalledUnityObject.MarshalNotNull(this);
			if (intPtr == (IntPtr)0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			return get_isStatic_Injected(intPtr);
		}
		[NativeMethod(Name = "SetIsStaticDeprecated")]
		set
		{
			IntPtr intPtr = MarshalledUnityObject.MarshalNotNull(this);
			if (intPtr == (IntPtr)0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			set_isStatic_Injected(intPtr, value);
		}
	}

	internal bool isStaticBatchable
	{
		[NativeMethod(Name = "IsStaticBatchable")]
		get
		{
			IntPtr intPtr = MarshalledUnityObject.MarshalNotNull(this);
			if (intPtr == (IntPtr)0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			return get_isStaticBatchable_Injected(intPtr);
		}
	}

	public unsafe string tag
	{
		[FreeFunction("GameObjectBindings::GetTag", HasExplicitThis = true)]
		get
		{
			ManagedSpanWrapper ret = default(ManagedSpanWrapper);
			string stringAndDispose;
			try
			{
				IntPtr intPtr = MarshalledUnityObject.MarshalNotNull(this);
				if (intPtr == (IntPtr)0)
				{
					ThrowHelper.ThrowNullReferenceException(this);
				}
				get_tag_Injected(intPtr, out ret);
			}
			finally
			{
				stringAndDispose = OutStringMarshaller.GetStringAndDispose(ret);
			}
			return stringAndDispose;
		}
		[FreeFunction("GameObjectBindings::SetTag", HasExplicitThis = true)]
		set
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
				if (!StringMarshaller.TryMarshalEmptyOrNullString(value, ref managedSpanWrapper))
				{
					ReadOnlySpan<char> readOnlySpan = MemoryExtensions.AsSpan(value);
					fixed (char* begin = readOnlySpan)
					{
						managedSpanWrapper = new ManagedSpanWrapper(begin, readOnlySpan.Length);
						set_tag_Injected(intPtr, ref managedSpanWrapper);
						return;
					}
				}
				set_tag_Injected(intPtr, ref managedSpanWrapper);
			}
			finally
			{
			}
		}
	}

	public Scene scene
	{
		[FreeFunction("GameObjectBindings::GetScene", HasExplicitThis = true)]
		get
		{
			IntPtr intPtr = MarshalledUnityObject.MarshalNotNull(this);
			if (intPtr == (IntPtr)0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			get_scene_Injected(intPtr, out var ret);
			return ret;
		}
	}

	public ulong sceneCullingMask
	{
		[FreeFunction(Name = "GameObjectBindings::GetSceneCullingMask", HasExplicitThis = true)]
		get
		{
			IntPtr intPtr = MarshalledUnityObject.MarshalNotNull(this);
			if (intPtr == (IntPtr)0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			return get_sceneCullingMask_Injected(intPtr);
		}
	}

	public GameObject gameObject => this;

	[FreeFunction("GameObjectBindings::CreatePrimitive")]
	public static GameObject CreatePrimitive(PrimitiveType type)
	{
		return Unmarshal.UnmarshalUnityObject<GameObject>(CreatePrimitive_Injected(type));
	}

	[SecuritySafeCritical]
	public unsafe T GetComponent<T>()
	{
		CastHelper<T> castHelper = default(CastHelper<T>);
		GetComponentFastPath(typeof(T), new IntPtr(&castHelper.onePointerFurtherThanT));
		return castHelper.t;
	}

	[TypeInferenceRule(TypeInferenceRules.TypeReferencedByFirstArgument)]
	[FreeFunction(Name = "GameObjectBindings::GetComponentFromType", HasExplicitThis = true, ThrowsException = true)]
	public Component GetComponent(Type type)
	{
		IntPtr intPtr = MarshalledUnityObject.MarshalNotNull(this);
		if (intPtr == (IntPtr)0)
		{
			ThrowHelper.ThrowNullReferenceException(this);
		}
		return Unmarshal.UnmarshalUnityObject<Component>(GetComponent_Injected(intPtr, type));
	}

	[FreeFunction(Name = "GameObjectBindings::GetComponentFastPath", HasExplicitThis = true, ThrowsException = true)]
	internal void GetComponentFastPath(Type type, IntPtr oneFurtherThanResultValue)
	{
		IntPtr intPtr = MarshalledUnityObject.MarshalNotNull(this);
		if (intPtr == (IntPtr)0)
		{
			ThrowHelper.ThrowNullReferenceException(this);
		}
		GetComponentFastPath_Injected(intPtr, type, oneFurtherThanResultValue);
	}

	[FreeFunction(Name = "Scripting::GetScriptingWrapperOfComponentOfGameObject", HasExplicitThis = true)]
	internal unsafe Component GetComponentByName(string type)
	{
		//The blocks IL_0039 are reachable both inside and outside the pinned region starting at IL_0028. ILSpy has duplicated these blocks in order to place them both within and outside the `fixed` statement.
		IntPtr componentByName_Injected = default(IntPtr);
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
					componentByName_Injected = GetComponentByName_Injected(intPtr, ref managedSpanWrapper);
				}
			}
			else
			{
				componentByName_Injected = GetComponentByName_Injected(intPtr, ref managedSpanWrapper);
			}
		}
		finally
		{
			result = Unmarshal.UnmarshalUnityObject<Component>(componentByName_Injected);
		}
		return result;
	}

	[FreeFunction(Name = "Scripting::GetScriptingWrapperOfComponentOfGameObjectWithCase", HasExplicitThis = true)]
	internal unsafe Component GetComponentByNameWithCase(string type, bool caseSensitive)
	{
		//The blocks IL_0039 are reachable both inside and outside the pinned region starting at IL_0028. ILSpy has duplicated these blocks in order to place them both within and outside the `fixed` statement.
		IntPtr componentByNameWithCase_Injected = default(IntPtr);
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
					componentByNameWithCase_Injected = GetComponentByNameWithCase_Injected(intPtr, ref managedSpanWrapper, caseSensitive);
				}
			}
			else
			{
				componentByNameWithCase_Injected = GetComponentByNameWithCase_Injected(intPtr, ref managedSpanWrapper, caseSensitive);
			}
		}
		finally
		{
			result = Unmarshal.UnmarshalUnityObject<Component>(componentByNameWithCase_Injected);
		}
		return result;
	}

	public Component GetComponent(string type)
	{
		return GetComponentByName(type);
	}

	[FreeFunction(Name = "GameObjectBindings::GetComponentInChildren", HasExplicitThis = true, ThrowsException = true)]
	[TypeInferenceRule(TypeInferenceRules.TypeReferencedByFirstArgument)]
	public Component GetComponentInChildren(Type type, bool includeInactive)
	{
		IntPtr intPtr = MarshalledUnityObject.MarshalNotNull(this);
		if (intPtr == (IntPtr)0)
		{
			ThrowHelper.ThrowNullReferenceException(this);
		}
		return Unmarshal.UnmarshalUnityObject<Component>(GetComponentInChildren_Injected(intPtr, type, includeInactive));
	}

	[TypeInferenceRule(TypeInferenceRules.TypeReferencedByFirstArgument)]
	public Component GetComponentInChildren(Type type)
	{
		return GetComponentInChildren(type, includeInactive: false);
	}

	[ExcludeFromDocs]
	public T GetComponentInChildren<T>()
	{
		bool includeInactive = false;
		return GetComponentInChildren<T>(includeInactive);
	}

	public T GetComponentInChildren<T>([DefaultValue("false")] bool includeInactive)
	{
		return (T)(object)GetComponentInChildren(typeof(T), includeInactive);
	}

	[TypeInferenceRule(TypeInferenceRules.TypeReferencedByFirstArgument)]
	[FreeFunction(Name = "GameObjectBindings::GetComponentInParent", HasExplicitThis = true, ThrowsException = true)]
	public Component GetComponentInParent(Type type, bool includeInactive)
	{
		IntPtr intPtr = MarshalledUnityObject.MarshalNotNull(this);
		if (intPtr == (IntPtr)0)
		{
			ThrowHelper.ThrowNullReferenceException(this);
		}
		return Unmarshal.UnmarshalUnityObject<Component>(GetComponentInParent_Injected(intPtr, type, includeInactive));
	}

	[TypeInferenceRule(TypeInferenceRules.TypeReferencedByFirstArgument)]
	public Component GetComponentInParent(Type type)
	{
		return GetComponentInParent(type, includeInactive: false);
	}

	[ExcludeFromDocs]
	public T GetComponentInParent<T>()
	{
		bool includeInactive = false;
		return GetComponentInParent<T>(includeInactive);
	}

	public T GetComponentInParent<T>([DefaultValue("false")] bool includeInactive)
	{
		return (T)(object)GetComponentInParent(typeof(T), includeInactive);
	}

	[FreeFunction(Name = "GameObjectBindings::GetComponentsInternal", HasExplicitThis = true, ThrowsException = true)]
	private Array GetComponentsInternal(Type type, bool useSearchTypeAsArrayReturnType, bool recursive, bool includeInactive, bool reverse, object resultList)
	{
		IntPtr intPtr = MarshalledUnityObject.MarshalNotNull(this);
		if (intPtr == (IntPtr)0)
		{
			ThrowHelper.ThrowNullReferenceException(this);
		}
		return GetComponentsInternal_Injected(intPtr, type, useSearchTypeAsArrayReturnType, recursive, includeInactive, reverse, resultList);
	}

	public Component[] GetComponents(Type type)
	{
		return (Component[])GetComponentsInternal(type, useSearchTypeAsArrayReturnType: false, recursive: false, includeInactive: true, reverse: false, null);
	}

	public T[] GetComponents<T>()
	{
		return (T[])GetComponentsInternal(typeof(T), useSearchTypeAsArrayReturnType: true, recursive: false, includeInactive: true, reverse: false, null);
	}

	public void GetComponents(Type type, List<Component> results)
	{
		GetComponentsInternal(type, useSearchTypeAsArrayReturnType: false, recursive: false, includeInactive: true, reverse: false, results);
	}

	public void GetComponents<T>(List<T> results)
	{
		GetComponentsInternal(typeof(T), useSearchTypeAsArrayReturnType: true, recursive: false, includeInactive: true, reverse: false, results);
	}

	[ExcludeFromDocs]
	public Component[] GetComponentsInChildren(Type type)
	{
		bool includeInactive = false;
		return GetComponentsInChildren(type, includeInactive);
	}

	public Component[] GetComponentsInChildren(Type type, [DefaultValue("false")] bool includeInactive)
	{
		return (Component[])GetComponentsInternal(type, useSearchTypeAsArrayReturnType: false, recursive: true, includeInactive, reverse: false, null);
	}

	public T[] GetComponentsInChildren<T>(bool includeInactive)
	{
		return (T[])GetComponentsInternal(typeof(T), useSearchTypeAsArrayReturnType: true, recursive: true, includeInactive, reverse: false, null);
	}

	public void GetComponentsInChildren<T>(bool includeInactive, List<T> results)
	{
		GetComponentsInternal(typeof(T), useSearchTypeAsArrayReturnType: true, recursive: true, includeInactive, reverse: false, results);
	}

	public T[] GetComponentsInChildren<T>()
	{
		return GetComponentsInChildren<T>(includeInactive: false);
	}

	public void GetComponentsInChildren<T>(List<T> results)
	{
		GetComponentsInChildren(includeInactive: false, results);
	}

	[ExcludeFromDocs]
	public Component[] GetComponentsInParent(Type type)
	{
		bool includeInactive = false;
		return GetComponentsInParent(type, includeInactive);
	}

	public Component[] GetComponentsInParent(Type type, [DefaultValue("false")] bool includeInactive)
	{
		return (Component[])GetComponentsInternal(type, useSearchTypeAsArrayReturnType: false, recursive: true, includeInactive, reverse: true, null);
	}

	public void GetComponentsInParent<T>(bool includeInactive, List<T> results)
	{
		GetComponentsInternal(typeof(T), useSearchTypeAsArrayReturnType: true, recursive: true, includeInactive, reverse: true, results);
	}

	public T[] GetComponentsInParent<T>(bool includeInactive)
	{
		return (T[])GetComponentsInternal(typeof(T), useSearchTypeAsArrayReturnType: true, recursive: true, includeInactive, reverse: true, null);
	}

	public T[] GetComponentsInParent<T>()
	{
		return GetComponentsInParent<T>(includeInactive: false);
	}

	[SecuritySafeCritical]
	public unsafe bool TryGetComponent<T>(out T component)
	{
		CastHelper<T> castHelper = default(CastHelper<T>);
		TryGetComponentFastPath(typeof(T), new IntPtr(&castHelper.onePointerFurtherThanT));
		component = castHelper.t;
		return castHelper.t != null;
	}

	public bool TryGetComponent(Type type, out Component component)
	{
		component = TryGetComponentInternal(type);
		return component != null;
	}

	[FreeFunction(Name = "GameObjectBindings::TryGetComponentFromType", HasExplicitThis = true, ThrowsException = true)]
	[TypeInferenceRule(TypeInferenceRules.TypeReferencedByFirstArgument)]
	internal Component TryGetComponentInternal(Type type)
	{
		IntPtr intPtr = MarshalledUnityObject.MarshalNotNull(this);
		if (intPtr == (IntPtr)0)
		{
			ThrowHelper.ThrowNullReferenceException(this);
		}
		return Unmarshal.UnmarshalUnityObject<Component>(TryGetComponentInternal_Injected(intPtr, type));
	}

	[FreeFunction(Name = "GameObjectBindings::TryGetComponentFastPath", HasExplicitThis = true, ThrowsException = true)]
	internal void TryGetComponentFastPath(Type type, IntPtr oneFurtherThanResultValue)
	{
		IntPtr intPtr = MarshalledUnityObject.MarshalNotNull(this);
		if (intPtr == (IntPtr)0)
		{
			ThrowHelper.ThrowNullReferenceException(this);
		}
		TryGetComponentFastPath_Injected(intPtr, type, oneFurtherThanResultValue);
	}

	public static GameObject FindWithTag(string tag)
	{
		return FindGameObjectWithTag(tag);
	}

	[FreeFunction(Name = "GameObjectBindings::FindGameObjectsWithTagForListInternal", ThrowsException = true)]
	private unsafe static void FindGameObjectsWithTagForListInternal(string tag, object results)
	{
		//The blocks IL_0029 are reachable both inside and outside the pinned region starting at IL_0018. ILSpy has duplicated these blocks in order to place them both within and outside the `fixed` statement.
		try
		{
			ManagedSpanWrapper managedSpanWrapper = default(ManagedSpanWrapper);
			if (!StringMarshaller.TryMarshalEmptyOrNullString(tag, ref managedSpanWrapper))
			{
				ReadOnlySpan<char> readOnlySpan = MemoryExtensions.AsSpan(tag);
				fixed (char* begin = readOnlySpan)
				{
					managedSpanWrapper = new ManagedSpanWrapper(begin, readOnlySpan.Length);
					FindGameObjectsWithTagForListInternal_Injected(ref managedSpanWrapper, results);
					return;
				}
			}
			FindGameObjectsWithTagForListInternal_Injected(ref managedSpanWrapper, results);
		}
		finally
		{
		}
	}

	public static void FindGameObjectsWithTag(string tag, List<GameObject> results)
	{
		FindGameObjectsWithTagForListInternal(tag, results);
	}

	public void SendMessageUpwards(string methodName, SendMessageOptions options)
	{
		SendMessageUpwards(methodName, null, options);
	}

	public void SendMessage(string methodName, SendMessageOptions options)
	{
		SendMessage(methodName, null, options);
	}

	public void BroadcastMessage(string methodName, SendMessageOptions options)
	{
		BroadcastMessage(methodName, null, options);
	}

	[FreeFunction(Name = "MonoAddComponent", HasExplicitThis = true)]
	internal unsafe Component AddComponentInternal(string className)
	{
		//The blocks IL_0039 are reachable both inside and outside the pinned region starting at IL_0028. ILSpy has duplicated these blocks in order to place them both within and outside the `fixed` statement.
		IntPtr gcHandlePtr = default(IntPtr);
		Component result;
		try
		{
			IntPtr intPtr = MarshalledUnityObject.MarshalNotNull(this);
			if (intPtr == (IntPtr)0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			ManagedSpanWrapper managedSpanWrapper = default(ManagedSpanWrapper);
			if (!StringMarshaller.TryMarshalEmptyOrNullString(className, ref managedSpanWrapper))
			{
				ReadOnlySpan<char> readOnlySpan = MemoryExtensions.AsSpan(className);
				fixed (char* begin = readOnlySpan)
				{
					managedSpanWrapper = new ManagedSpanWrapper(begin, readOnlySpan.Length);
					gcHandlePtr = AddComponentInternal_Injected(intPtr, ref managedSpanWrapper);
				}
			}
			else
			{
				gcHandlePtr = AddComponentInternal_Injected(intPtr, ref managedSpanWrapper);
			}
		}
		finally
		{
			result = Unmarshal.UnmarshalUnityObject<Component>(gcHandlePtr);
		}
		return result;
	}

	[FreeFunction(Name = "MonoAddComponentWithType", HasExplicitThis = true)]
	private Component Internal_AddComponentWithType(Type componentType)
	{
		IntPtr intPtr = MarshalledUnityObject.MarshalNotNull(this);
		if (intPtr == (IntPtr)0)
		{
			ThrowHelper.ThrowNullReferenceException(this);
		}
		return Unmarshal.UnmarshalUnityObject<Component>(Internal_AddComponentWithType_Injected(intPtr, componentType));
	}

	[TypeInferenceRule(TypeInferenceRules.TypeReferencedByFirstArgument)]
	public Component AddComponent(Type componentType)
	{
		return Internal_AddComponentWithType(componentType);
	}

	public T AddComponent<T>() where T : Component
	{
		return AddComponent(typeof(T)) as T;
	}

	public int GetComponentCount()
	{
		IntPtr intPtr = MarshalledUnityObject.MarshalNotNull(this);
		if (intPtr == (IntPtr)0)
		{
			ThrowHelper.ThrowNullReferenceException(this);
		}
		return GetComponentCount_Injected(intPtr);
	}

	[NativeName("QueryComponentAtIndex<Unity::Component>")]
	internal Component QueryComponentAtIndex(int index)
	{
		IntPtr intPtr = MarshalledUnityObject.MarshalNotNull(this);
		if (intPtr == (IntPtr)0)
		{
			ThrowHelper.ThrowNullReferenceException(this);
		}
		return Unmarshal.UnmarshalUnityObject<Component>(QueryComponentAtIndex_Injected(intPtr, index));
	}

	public Component GetComponentAtIndex(int index)
	{
		if (index < 0 || index >= GetComponentCount())
		{
			throw new ArgumentOutOfRangeException("index", "Valid range is 0 to GetComponentCount() - 1.");
		}
		return QueryComponentAtIndex(index);
	}

	public T GetComponentAtIndex<T>(int index) where T : Component
	{
		T val = (T)GetComponentAtIndex(index);
		if (val == null)
		{
			throw new InvalidCastException();
		}
		return val;
	}

	public int GetComponentIndex(Component component)
	{
		IntPtr intPtr = MarshalledUnityObject.MarshalNotNull(this);
		if (intPtr == (IntPtr)0)
		{
			ThrowHelper.ThrowNullReferenceException(this);
		}
		return GetComponentIndex_Injected(intPtr, MarshalledUnityObject.Marshal(component));
	}

	[NativeMethod(Name = "SetSelfActive")]
	public void SetActive(bool value)
	{
		IntPtr intPtr = MarshalledUnityObject.MarshalNotNull(this);
		if (intPtr == (IntPtr)0)
		{
			ThrowHelper.ThrowNullReferenceException(this);
		}
		SetActive_Injected(intPtr, value);
	}

	[NativeMethod(Name = "SetActiveRecursivelyDeprecated")]
	[Obsolete("gameObject.SetActiveRecursively() is obsolete. Use GameObject.SetActive(), which is now inherited by children.")]
	public void SetActiveRecursively(bool state)
	{
		IntPtr intPtr = MarshalledUnityObject.MarshalNotNull(this);
		if (intPtr == (IntPtr)0)
		{
			ThrowHelper.ThrowNullReferenceException(this);
		}
		SetActiveRecursively_Injected(intPtr, state);
	}

	public bool CompareTag(string tag)
	{
		return CompareTag_Internal(tag);
	}

	public bool CompareTag(TagHandle tag)
	{
		return CompareTagHandle_Internal(tag);
	}

	[FreeFunction(Name = "GameObjectBindings::CompareTag", HasExplicitThis = true)]
	private unsafe bool CompareTag_Internal(string tag)
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
			if (!StringMarshaller.TryMarshalEmptyOrNullString(tag, ref managedSpanWrapper))
			{
				ReadOnlySpan<char> readOnlySpan = MemoryExtensions.AsSpan(tag);
				fixed (char* begin = readOnlySpan)
				{
					managedSpanWrapper = new ManagedSpanWrapper(begin, readOnlySpan.Length);
					return CompareTag_Internal_Injected(intPtr, ref managedSpanWrapper);
				}
			}
			return CompareTag_Internal_Injected(intPtr, ref managedSpanWrapper);
		}
		finally
		{
		}
	}

	[FreeFunction(Name = "GameObjectBindings::CompareTagHandle", HasExplicitThis = true)]
	private bool CompareTagHandle_Internal(TagHandle tag)
	{
		IntPtr intPtr = MarshalledUnityObject.MarshalNotNull(this);
		if (intPtr == (IntPtr)0)
		{
			ThrowHelper.ThrowNullReferenceException(this);
		}
		return CompareTagHandle_Internal_Injected(intPtr, ref tag);
	}

	[FreeFunction(Name = "GameObjectBindings::FindGameObjectWithTag", ThrowsException = true)]
	public unsafe static GameObject FindGameObjectWithTag(string tag)
	{
		//The blocks IL_0029 are reachable both inside and outside the pinned region starting at IL_0018. ILSpy has duplicated these blocks in order to place them both within and outside the `fixed` statement.
		IntPtr gcHandlePtr = default(IntPtr);
		GameObject result;
		try
		{
			ManagedSpanWrapper managedSpanWrapper = default(ManagedSpanWrapper);
			if (!StringMarshaller.TryMarshalEmptyOrNullString(tag, ref managedSpanWrapper))
			{
				ReadOnlySpan<char> readOnlySpan = MemoryExtensions.AsSpan(tag);
				fixed (char* begin = readOnlySpan)
				{
					managedSpanWrapper = new ManagedSpanWrapper(begin, readOnlySpan.Length);
					gcHandlePtr = FindGameObjectWithTag_Injected(ref managedSpanWrapper);
				}
			}
			else
			{
				gcHandlePtr = FindGameObjectWithTag_Injected(ref managedSpanWrapper);
			}
		}
		finally
		{
			result = Unmarshal.UnmarshalUnityObject<GameObject>(gcHandlePtr);
		}
		return result;
	}

	[FreeFunction(Name = "GameObjectBindings::FindGameObjectsWithTag", ThrowsException = true)]
	public unsafe static GameObject[] FindGameObjectsWithTag(string tag)
	{
		//The blocks IL_0029 are reachable both inside and outside the pinned region starting at IL_0018. ILSpy has duplicated these blocks in order to place them both within and outside the `fixed` statement.
		try
		{
			ManagedSpanWrapper managedSpanWrapper = default(ManagedSpanWrapper);
			if (!StringMarshaller.TryMarshalEmptyOrNullString(tag, ref managedSpanWrapper))
			{
				ReadOnlySpan<char> readOnlySpan = MemoryExtensions.AsSpan(tag);
				fixed (char* begin = readOnlySpan)
				{
					managedSpanWrapper = new ManagedSpanWrapper(begin, readOnlySpan.Length);
					return FindGameObjectsWithTag_Injected(ref managedSpanWrapper);
				}
			}
			return FindGameObjectsWithTag_Injected(ref managedSpanWrapper);
		}
		finally
		{
		}
	}

	[FreeFunction(Name = "Scripting::SendScriptingMessageUpwards", HasExplicitThis = true)]
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
		SendMessageOptions options = SendMessageOptions.RequireReceiver;
		SendMessageUpwards(methodName, value, options);
	}

	[ExcludeFromDocs]
	public void SendMessageUpwards(string methodName)
	{
		SendMessageOptions options = SendMessageOptions.RequireReceiver;
		object value = null;
		SendMessageUpwards(methodName, value, options);
	}

	[FreeFunction(Name = "Scripting::SendScriptingMessage", HasExplicitThis = true)]
	public unsafe void SendMessage(string methodName, [DefaultValue("null")] object value, [DefaultValue("SendMessageOptions.RequireReceiver")] SendMessageOptions options)
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

	[ExcludeFromDocs]
	public void SendMessage(string methodName, object value)
	{
		SendMessageOptions options = SendMessageOptions.RequireReceiver;
		SendMessage(methodName, value, options);
	}

	[ExcludeFromDocs]
	public void SendMessage(string methodName)
	{
		SendMessageOptions options = SendMessageOptions.RequireReceiver;
		object value = null;
		SendMessage(methodName, value, options);
	}

	[FreeFunction(Name = "Scripting::BroadcastScriptingMessage", HasExplicitThis = true)]
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
		SendMessageOptions options = SendMessageOptions.RequireReceiver;
		BroadcastMessage(methodName, parameter, options);
	}

	[ExcludeFromDocs]
	public void BroadcastMessage(string methodName)
	{
		SendMessageOptions options = SendMessageOptions.RequireReceiver;
		object parameter = null;
		BroadcastMessage(methodName, parameter, options);
	}

	public GameObject(string name)
	{
		Internal_CreateGameObject(this, name);
	}

	public GameObject()
	{
		Internal_CreateGameObject(this, null);
	}

	public GameObject(string name, params Type[] components)
	{
		Internal_CreateGameObject(this, name);
		foreach (Type componentType in components)
		{
			AddComponent(componentType);
		}
	}

	[FreeFunction(Name = "GameObjectBindings::Internal_CreateGameObject")]
	private unsafe static void Internal_CreateGameObject([Writable] GameObject self, string name)
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
					Internal_CreateGameObject_Injected(self, ref managedSpanWrapper);
					return;
				}
			}
			Internal_CreateGameObject_Injected(self, ref managedSpanWrapper);
		}
		finally
		{
		}
	}

	[FreeFunction(Name = "GameObjectBindings::Find")]
	public unsafe static GameObject Find(string name)
	{
		//The blocks IL_0029 are reachable both inside and outside the pinned region starting at IL_0018. ILSpy has duplicated these blocks in order to place them both within and outside the `fixed` statement.
		IntPtr gcHandlePtr = default(IntPtr);
		GameObject result;
		try
		{
			ManagedSpanWrapper managedSpanWrapper = default(ManagedSpanWrapper);
			if (!StringMarshaller.TryMarshalEmptyOrNullString(name, ref managedSpanWrapper))
			{
				ReadOnlySpan<char> readOnlySpan = MemoryExtensions.AsSpan(name);
				fixed (char* begin = readOnlySpan)
				{
					managedSpanWrapper = new ManagedSpanWrapper(begin, readOnlySpan.Length);
					gcHandlePtr = Find_Injected(ref managedSpanWrapper);
				}
			}
			else
			{
				gcHandlePtr = Find_Injected(ref managedSpanWrapper);
			}
		}
		finally
		{
			result = Unmarshal.UnmarshalUnityObject<GameObject>(gcHandlePtr);
		}
		return result;
	}

	[MethodImpl(MethodImplOptions.InternalCall)]
	[FreeFunction(Name = "GameObjectBindings::SetGameObjectsActiveByInstanceID")]
	private static extern void SetGameObjectsActive(IntPtr instanceIds, int instanceCount, bool active);

	public unsafe static void SetGameObjectsActive(NativeArray<int> instanceIDs, bool active)
	{
		if (!instanceIDs.IsCreated)
		{
			throw new ArgumentException("NativeArray is uninitialized", "instanceIDs");
		}
		if (instanceIDs.Length != 0)
		{
			SetGameObjectsActive((IntPtr)instanceIDs.GetUnsafeReadOnlyPtr(), instanceIDs.Length, active);
		}
	}

	public unsafe static void SetGameObjectsActive(ReadOnlySpan<int> instanceIDs, bool active)
	{
		if (instanceIDs.Length != 0)
		{
			fixed (int* ptr = instanceIDs)
			{
				SetGameObjectsActive((IntPtr)ptr, instanceIDs.Length, active);
			}
		}
	}

	[FreeFunction("GameObjectBindings::InstantiateGameObjectsByInstanceID")]
	private static void InstantiateGameObjects(int sourceInstanceID, IntPtr newInstanceIDs, IntPtr newTransformInstanceIDs, int count, Scene destinationScene)
	{
		InstantiateGameObjects_Injected(sourceInstanceID, newInstanceIDs, newTransformInstanceIDs, count, ref destinationScene);
	}

	public unsafe static void InstantiateGameObjects(int sourceInstanceID, int count, NativeArray<int> newInstanceIDs, NativeArray<int> newTransformInstanceIDs, Scene destinationScene = default(Scene))
	{
		if (!newInstanceIDs.IsCreated)
		{
			throw new ArgumentException("NativeArray is uninitialized", "newInstanceIDs");
		}
		if (!newTransformInstanceIDs.IsCreated)
		{
			throw new ArgumentException("NativeArray is uninitialized", "newTransformInstanceIDs");
		}
		if (count != 0)
		{
			if (count != newInstanceIDs.Length || count != newTransformInstanceIDs.Length)
			{
				throw new ArgumentException("Size mismatch! Both arrays must already be the size of count.");
			}
			InstantiateGameObjects(sourceInstanceID, (IntPtr)newInstanceIDs.GetUnsafeReadOnlyPtr(), (IntPtr)newTransformInstanceIDs.GetUnsafeReadOnlyPtr(), newInstanceIDs.Length, destinationScene);
		}
	}

	[FreeFunction(Name = "GameObjectBindings::GetSceneByInstanceID")]
	public static Scene GetScene(int instanceID)
	{
		GetScene_Injected(instanceID, out var ret);
		return ret;
	}

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern IntPtr CreatePrimitive_Injected(PrimitiveType type);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern IntPtr GetComponent_Injected(IntPtr _unity_self, Type type);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern void GetComponentFastPath_Injected(IntPtr _unity_self, Type type, IntPtr oneFurtherThanResultValue);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern IntPtr GetComponentByName_Injected(IntPtr _unity_self, ref ManagedSpanWrapper type);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern IntPtr GetComponentByNameWithCase_Injected(IntPtr _unity_self, ref ManagedSpanWrapper type, bool caseSensitive);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern IntPtr GetComponentInChildren_Injected(IntPtr _unity_self, Type type, bool includeInactive);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern IntPtr GetComponentInParent_Injected(IntPtr _unity_self, Type type, bool includeInactive);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern Array GetComponentsInternal_Injected(IntPtr _unity_self, Type type, bool useSearchTypeAsArrayReturnType, bool recursive, bool includeInactive, bool reverse, object resultList);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern IntPtr TryGetComponentInternal_Injected(IntPtr _unity_self, Type type);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern void TryGetComponentFastPath_Injected(IntPtr _unity_self, Type type, IntPtr oneFurtherThanResultValue);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern void FindGameObjectsWithTagForListInternal_Injected(ref ManagedSpanWrapper tag, object results);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern IntPtr AddComponentInternal_Injected(IntPtr _unity_self, ref ManagedSpanWrapper className);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern IntPtr Internal_AddComponentWithType_Injected(IntPtr _unity_self, Type componentType);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern int GetComponentCount_Injected(IntPtr _unity_self);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern IntPtr QueryComponentAtIndex_Injected(IntPtr _unity_self, int index);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern int GetComponentIndex_Injected(IntPtr _unity_self, IntPtr component);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern IntPtr get_transform_Injected(IntPtr _unity_self);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern int get_layer_Injected(IntPtr _unity_self);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern void set_layer_Injected(IntPtr _unity_self, int value);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern bool get_active_Injected(IntPtr _unity_self);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern void set_active_Injected(IntPtr _unity_self, bool value);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern void SetActive_Injected(IntPtr _unity_self, bool value);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern bool get_activeSelf_Injected(IntPtr _unity_self);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern bool get_activeInHierarchy_Injected(IntPtr _unity_self);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern void SetActiveRecursively_Injected(IntPtr _unity_self, bool state);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern bool get_isStatic_Injected(IntPtr _unity_self);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern void set_isStatic_Injected(IntPtr _unity_self, bool value);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern bool get_isStaticBatchable_Injected(IntPtr _unity_self);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern void get_tag_Injected(IntPtr _unity_self, out ManagedSpanWrapper ret);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern void set_tag_Injected(IntPtr _unity_self, ref ManagedSpanWrapper value);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern bool CompareTag_Internal_Injected(IntPtr _unity_self, ref ManagedSpanWrapper tag);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern bool CompareTagHandle_Internal_Injected(IntPtr _unity_self, [In] ref TagHandle tag);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern IntPtr FindGameObjectWithTag_Injected(ref ManagedSpanWrapper tag);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern GameObject[] FindGameObjectsWithTag_Injected(ref ManagedSpanWrapper tag);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern void SendMessageUpwards_Injected(IntPtr _unity_self, ref ManagedSpanWrapper methodName, [DefaultValue("null")] object value, [DefaultValue("SendMessageOptions.RequireReceiver")] SendMessageOptions options);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern void SendMessage_Injected(IntPtr _unity_self, ref ManagedSpanWrapper methodName, [DefaultValue("null")] object value, [DefaultValue("SendMessageOptions.RequireReceiver")] SendMessageOptions options);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern void BroadcastMessage_Injected(IntPtr _unity_self, ref ManagedSpanWrapper methodName, [DefaultValue("null")] object parameter, [DefaultValue("SendMessageOptions.RequireReceiver")] SendMessageOptions options);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern void Internal_CreateGameObject_Injected([Writable] GameObject self, ref ManagedSpanWrapper name);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern IntPtr Find_Injected(ref ManagedSpanWrapper name);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern void InstantiateGameObjects_Injected(int sourceInstanceID, IntPtr newInstanceIDs, IntPtr newTransformInstanceIDs, int count, [In] ref Scene destinationScene);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern void GetScene_Injected(int instanceID, out Scene ret);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern void get_scene_Injected(IntPtr _unity_self, out Scene ret);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern ulong get_sceneCullingMask_Injected(IntPtr _unity_self);
}
