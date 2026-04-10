using System;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Security;
using System.Threading;
using UnityEngine.Bindings;
using UnityEngine.Internal;
using UnityEngine.SceneManagement;
using UnityEngine.Scripting;
using UnityEngineInternal;

namespace UnityEngine;

[StructLayout(LayoutKind.Sequential)]
[NativeHeader("Runtime/SceneManager/SceneManager.h")]
[NativeHeader("Runtime/GameCode/CloneObject.h")]
[NativeHeader("Runtime/Export/Scripting/UnityEngineObject.bindings.h")]
[RequiredByNativeCode(GenerateProxy = true)]
public class Object
{
	[VisibleToOtherModules]
	internal static class MarshalledUnityObject
	{
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static IntPtr Marshal<T>(T obj) where T : Object
		{
			if ((object)obj == null)
			{
				return IntPtr.Zero;
			}
			return MarshalNotNull(obj);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static IntPtr MarshalNotNull<T>(T obj) where T : Object
		{
			return obj.m_CachedPtr;
		}

		public static void TryThrowEditorNullExceptionObject(Object unityObj, string paramterName)
		{
		}
	}

	private const int kInstanceID_None = 0;

	private IntPtr m_CachedPtr;

	internal static readonly int OffsetOfInstanceIDInCPlusPlusObject = GetOffsetOfInstanceIDInCPlusPlusObject();

	private const string objectIsNullMessage = "The Object you want to instantiate is null.";

	private const string cloneDestroyedMessage = "Instantiate failed because the clone was destroyed during creation. This can happen if DestroyImmediate is called in MonoBehaviour.Awake.";

	public string name
	{
		get
		{
			return GetName();
		}
		set
		{
			SetName(value);
		}
	}

	public HideFlags hideFlags
	{
		get
		{
			IntPtr intPtr = MarshalledUnityObject.MarshalNotNull(this);
			if (intPtr == (IntPtr)0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			return get_hideFlags_Injected(intPtr);
		}
		set
		{
			IntPtr intPtr = MarshalledUnityObject.MarshalNotNull(this);
			if (intPtr == (IntPtr)0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			set_hideFlags_Injected(intPtr, value);
		}
	}

	[SecuritySafeCritical]
	public unsafe EntityId GetEntityId()
	{
		if (m_CachedPtr == IntPtr.Zero)
		{
			return EntityId.None;
		}
		return *(int*)((byte*)(void*)m_CachedPtr + OffsetOfInstanceIDInCPlusPlusObject);
	}

	[SecuritySafeCritical]
	public unsafe int GetInstanceID()
	{
		if (m_CachedPtr == IntPtr.Zero)
		{
			return 0;
		}
		return *(int*)((byte*)(void*)m_CachedPtr + OffsetOfInstanceIDInCPlusPlusObject);
	}

	public override int GetHashCode()
	{
		return base.GetHashCode();
	}

	public override bool Equals(object other)
	{
		Object obj = other as Object;
		if (obj == null && other != null && !(other is Object))
		{
			return false;
		}
		return CompareBaseObjects(this, obj);
	}

	public static implicit operator bool([MaybeNullWhen(false)][NotNullWhen(true)] Object exists)
	{
		return !CompareBaseObjects(exists, null);
	}

	private static bool CompareBaseObjects(Object lhs, Object rhs)
	{
		bool flag = (object)lhs == null;
		bool flag2 = (object)rhs == null;
		if (flag2 && flag)
		{
			return true;
		}
		if (flag2)
		{
			return !IsNativeObjectAlive(lhs);
		}
		if (flag)
		{
			return !IsNativeObjectAlive(rhs);
		}
		return (object)lhs == rhs;
	}

	private void EnsureRunningOnMainThread()
	{
		if (!CurrentThreadIsMainThread())
		{
			throw new InvalidOperationException("EnsureRunningOnMainThread can only be called from the main thread");
		}
	}

	private static bool IsNativeObjectAlive(Object o)
	{
		return o.GetCachedPtr() != IntPtr.Zero;
	}

	private IntPtr GetCachedPtr()
	{
		return m_CachedPtr;
	}

	public static AsyncInstantiateOperation<T> InstantiateAsync<T>(T original) where T : Object
	{
		return InstantiateAsync(original, new InstantiateParameters
		{
			worldSpace = true
		});
	}

	public static AsyncInstantiateOperation<T> InstantiateAsync<T>(T original, Transform parent) where T : Object
	{
		return InstantiateAsync(original, new InstantiateParameters
		{
			worldSpace = true,
			parent = parent
		});
	}

	public static AsyncInstantiateOperation<T> InstantiateAsync<T>(T original, Vector3 position, Quaternion rotation) where T : Object
	{
		return InstantiateAsync(original, position, rotation, new InstantiateParameters
		{
			worldSpace = true
		});
	}

	public static AsyncInstantiateOperation<T> InstantiateAsync<T>(T original, Transform parent, Vector3 position, Quaternion rotation) where T : Object
	{
		return InstantiateAsync(original, position, rotation, new InstantiateParameters
		{
			worldSpace = true,
			parent = parent
		});
	}

	public static AsyncInstantiateOperation<T> InstantiateAsync<T>(T original, int count) where T : Object
	{
		return InstantiateAsync(original, count, new InstantiateParameters
		{
			worldSpace = true
		});
	}

	public static AsyncInstantiateOperation<T> InstantiateAsync<T>(T original, int count, Transform parent) where T : Object
	{
		return InstantiateAsync(original, count, new InstantiateParameters
		{
			worldSpace = true,
			parent = parent
		});
	}

	public static AsyncInstantiateOperation<T> InstantiateAsync<T>(T original, int count, Vector3 position, Quaternion rotation) where T : Object
	{
		return InstantiateAsync(original, count, position, rotation, new InstantiateParameters
		{
			worldSpace = true
		});
	}

	public static AsyncInstantiateOperation<T> InstantiateAsync<T>(T original, int count, ReadOnlySpan<Vector3> positions, ReadOnlySpan<Quaternion> rotations) where T : Object
	{
		return InstantiateAsync(original, count, positions, rotations, new InstantiateParameters
		{
			worldSpace = true
		});
	}

	public static AsyncInstantiateOperation<T> InstantiateAsync<T>(T original, int count, Transform parent, Vector3 position, Quaternion rotation) where T : Object
	{
		return InstantiateAsync(original, count, position, rotation, new InstantiateParameters
		{
			worldSpace = true,
			parent = parent
		});
	}

	public static AsyncInstantiateOperation<T> InstantiateAsync<T>(T original, int count, Transform parent, Vector3 position, Quaternion rotation, CancellationToken cancellationToken) where T : Object
	{
		return InstantiateAsync(original, count, position, rotation, new InstantiateParameters
		{
			worldSpace = true,
			parent = parent
		}, cancellationToken);
	}

	public static AsyncInstantiateOperation<T> InstantiateAsync<T>(T original, int count, Transform parent, ReadOnlySpan<Vector3> positions, ReadOnlySpan<Quaternion> rotations) where T : Object
	{
		return InstantiateAsync(original, count, positions, rotations, new InstantiateParameters
		{
			worldSpace = true,
			parent = parent
		});
	}

	public static AsyncInstantiateOperation<T> InstantiateAsync<T>(T original, int count, Transform parent, ReadOnlySpan<Vector3> positions, ReadOnlySpan<Quaternion> rotations, CancellationToken cancellationToken) where T : Object
	{
		return InstantiateAsync(original, count, positions, rotations, new InstantiateParameters
		{
			worldSpace = true,
			parent = parent
		}, cancellationToken);
	}

	public static AsyncInstantiateOperation<T> InstantiateAsync<T>(T original, InstantiateParameters parameters, CancellationToken cancellationToken = default(CancellationToken)) where T : Object
	{
		return InstantiateAsync(original, 1, parameters, cancellationToken);
	}

	public static AsyncInstantiateOperation<T> InstantiateAsync<T>(T original, int count, InstantiateParameters parameters, CancellationToken cancellationToken = default(CancellationToken)) where T : Object
	{
		return InstantiateAsync(original, count, ReadOnlySpan<Vector3>.Empty, ReadOnlySpan<Quaternion>.Empty, parameters, cancellationToken);
	}

	public static AsyncInstantiateOperation<T> InstantiateAsync<T>(T original, Vector3 position, Quaternion rotation, InstantiateParameters parameters, CancellationToken cancellationToken = default(CancellationToken)) where T : Object
	{
		return InstantiateAsync(original, 1, position, rotation, parameters, cancellationToken);
	}

	public unsafe static AsyncInstantiateOperation<T> InstantiateAsync<T>(T original, int count, Vector3 position, Quaternion rotation, InstantiateParameters parameters, CancellationToken cancellationToken = default(CancellationToken)) where T : Object
	{
		return InstantiateAsync(original, count, new ReadOnlySpan<Vector3>(&position, 1), new ReadOnlySpan<Quaternion>(&rotation, 1), parameters, cancellationToken);
	}

	[MethodImpl((MethodImplOptions)768)]
	public unsafe static AsyncInstantiateOperation<T> InstantiateAsync<T>(T original, int count, ReadOnlySpan<Vector3> positions, ReadOnlySpan<Quaternion> rotations, InstantiateParameters parameters, CancellationToken cancellationToken = default(CancellationToken)) where T : Object
	{
		CheckNullArgument(original, "The Object you want to instantiate is null.");
		if (count <= 0)
		{
			throw new ArgumentException("Cannot call instantiate multiple with count less or equal to zero");
		}
		fixed (Vector3* ptr = positions)
		{
			fixed (Quaternion* ptr2 = rotations)
			{
				return new AsyncInstantiateOperation<T>(Internal_InstantiateAsyncWithParams(original, count, parameters, (IntPtr)ptr, positions.Length, (IntPtr)ptr2, rotations.Length), cancellationToken);
			}
		}
	}

	[TypeInferenceRule(TypeInferenceRules.TypeOfFirstArgument)]
	public static Object Instantiate(Object original, Vector3 position, Quaternion rotation)
	{
		CheckNullArgument(original, "The Object you want to instantiate is null.");
		if (original is ScriptableObject)
		{
			throw new ArgumentException("Cannot instantiate a ScriptableObject with a position and rotation");
		}
		Object obj = Internal_InstantiateSingle(original, position, rotation);
		if (obj == null)
		{
			throw new UnityException("Instantiate failed because the clone was destroyed during creation. This can happen if DestroyImmediate is called in MonoBehaviour.Awake.");
		}
		return obj;
	}

	[TypeInferenceRule(TypeInferenceRules.TypeOfFirstArgument)]
	public static Object Instantiate(Object original, Vector3 position, Quaternion rotation, Transform parent)
	{
		if (parent == null)
		{
			return Instantiate(original, position, rotation);
		}
		CheckNullArgument(original, "The Object you want to instantiate is null.");
		Object obj = Internal_InstantiateSingleWithParent(original, parent, position, rotation);
		if (obj == null)
		{
			throw new UnityException("Instantiate failed because the clone was destroyed during creation. This can happen if DestroyImmediate is called in MonoBehaviour.Awake.");
		}
		return obj;
	}

	[TypeInferenceRule(TypeInferenceRules.TypeOfFirstArgument)]
	public static Object Instantiate(Object original)
	{
		CheckNullArgument(original, "The Object you want to instantiate is null.");
		Object obj = Internal_CloneSingle(original);
		if (obj == null)
		{
			throw new UnityException("Instantiate failed because the clone was destroyed during creation. This can happen if DestroyImmediate is called in MonoBehaviour.Awake.");
		}
		return obj;
	}

	[TypeInferenceRule(TypeInferenceRules.TypeOfFirstArgument)]
	public static Object Instantiate(Object original, Scene scene)
	{
		CheckNullArgument(original, "The Object you want to instantiate is null.");
		Object obj = Internal_CloneSingleWithScene(original, scene);
		if (obj == null)
		{
			throw new UnityException("Instantiate failed because the clone was destroyed during creation. This can happen if DestroyImmediate is called in MonoBehaviour.Awake.");
		}
		return obj;
	}

	public static T Instantiate<T>(T original, InstantiateParameters parameters) where T : Object
	{
		CheckNullArgument(original, "The Object you want to instantiate is null.");
		T val = (T)Internal_CloneSingleWithParams(original, parameters);
		if (val == null)
		{
			throw new UnityException("Instantiate failed because the clone was destroyed during creation. This can happen if DestroyImmediate is called in MonoBehaviour.Awake.");
		}
		return val;
	}

	public static T Instantiate<T>(T original, Vector3 position, Quaternion rotation, InstantiateParameters parameters) where T : Object
	{
		CheckNullArgument(original, "The Object you want to instantiate is null.");
		T val = (T)Internal_InstantiateSingleWithParams(original, position, rotation, parameters);
		if (val == null)
		{
			throw new UnityException("Instantiate failed because the clone was destroyed during creation. This can happen if DestroyImmediate is called in MonoBehaviour.Awake.");
		}
		return val;
	}

	[TypeInferenceRule(TypeInferenceRules.TypeOfFirstArgument)]
	public static Object Instantiate(Object original, Transform parent)
	{
		return Instantiate(original, parent, instantiateInWorldSpace: false);
	}

	[TypeInferenceRule(TypeInferenceRules.TypeOfFirstArgument)]
	public static Object Instantiate(Object original, Transform parent, bool instantiateInWorldSpace)
	{
		if (parent == null)
		{
			return Instantiate(original);
		}
		CheckNullArgument(original, "The Object you want to instantiate is null.");
		Object obj = Internal_CloneSingleWithParent(original, parent, instantiateInWorldSpace);
		if (obj == null)
		{
			throw new UnityException("Instantiate failed because the clone was destroyed during creation. This can happen if DestroyImmediate is called in MonoBehaviour.Awake.");
		}
		return obj;
	}

	public static T Instantiate<T>(T original) where T : Object
	{
		CheckNullArgument(original, "The Object you want to instantiate is null.");
		T val = (T)Internal_CloneSingle(original);
		if (val == null)
		{
			throw new UnityException("Instantiate failed because the clone was destroyed during creation. This can happen if DestroyImmediate is called in MonoBehaviour.Awake.");
		}
		return val;
	}

	public static T Instantiate<T>(T original, Vector3 position, Quaternion rotation) where T : Object
	{
		return (T)Instantiate((Object)original, position, rotation);
	}

	public static T Instantiate<T>(T original, Vector3 position, Quaternion rotation, Transform parent) where T : Object
	{
		return (T)Instantiate((Object)original, position, rotation, parent);
	}

	public static T Instantiate<T>(T original, Transform parent) where T : Object
	{
		return Instantiate(original, parent, worldPositionStays: false);
	}

	public static T Instantiate<T>(T original, Transform parent, bool worldPositionStays) where T : Object
	{
		return (T)Instantiate((Object)original, parent, worldPositionStays);
	}

	[NativeMethod(Name = "Scripting::DestroyObjectFromScripting", IsFreeFunction = true, ThrowsException = true)]
	public static void Destroy(Object obj, [DefaultValue("0.0F")] float t)
	{
		Destroy_Injected(MarshalledUnityObject.Marshal(obj), t);
	}

	[ExcludeFromDocs]
	public static void Destroy(Object obj)
	{
		float t = 0f;
		Destroy(obj, t);
	}

	[NativeMethod(Name = "Scripting::DestroyObjectFromScriptingImmediate", IsFreeFunction = true, ThrowsException = true)]
	public static void DestroyImmediate(Object obj, [DefaultValue("false")] bool allowDestroyingAssets)
	{
		DestroyImmediate_Injected(MarshalledUnityObject.Marshal(obj), allowDestroyingAssets);
	}

	[ExcludeFromDocs]
	public static void DestroyImmediate(Object obj)
	{
		bool allowDestroyingAssets = false;
		DestroyImmediate(obj, allowDestroyingAssets);
	}

	[Obsolete("Object.FindObjectsOfType has been deprecated. Use Object.FindObjectsByType instead which lets you decide whether you need the results sorted or not.  FindObjectsOfType sorts the results by InstanceID, but if you do not need this using FindObjectSortMode.None is considerably faster.", false)]
	public static Object[] FindObjectsOfType(Type type)
	{
		return FindObjectsOfType(type, includeInactive: false);
	}

	[MethodImpl(MethodImplOptions.InternalCall)]
	[TypeInferenceRule(TypeInferenceRules.ArrayOfTypeReferencedByFirstArgument)]
	[FreeFunction("UnityEngineObjectBindings::FindObjectsOfType")]
	[Obsolete("Object.FindObjectsOfType has been deprecated. Use Object.FindObjectsByType instead which lets you decide whether you need the results sorted or not.  FindObjectsOfType sorts the results by InstanceID but if you do not need this using FindObjectSortMode.None is considerably faster.", false)]
	[return: UnityMarshalAs(NativeType.ScriptingObjectPtr)]
	public static extern Object[] FindObjectsOfType(Type type, bool includeInactive);

	public static Object[] FindObjectsByType(Type type, FindObjectsSortMode sortMode)
	{
		return FindObjectsByType(type, FindObjectsInactive.Exclude, sortMode);
	}

	[MethodImpl(MethodImplOptions.InternalCall)]
	[FreeFunction("UnityEngineObjectBindings::FindObjectsByType")]
	[TypeInferenceRule(TypeInferenceRules.ArrayOfTypeReferencedByFirstArgument)]
	[return: UnityMarshalAs(NativeType.ScriptingObjectPtr)]
	public static extern Object[] FindObjectsByType(Type type, FindObjectsInactive findObjectsInactive, FindObjectsSortMode sortMode);

	[FreeFunction("GetSceneManager().DontDestroyOnLoad", ThrowsException = true)]
	public static void DontDestroyOnLoad([UnityEngine.Bindings.NotNull] Object target)
	{
		if ((object)target == null)
		{
			ThrowHelper.ThrowArgumentNullException(target, "target");
		}
		IntPtr intPtr = MarshalledUnityObject.MarshalNotNull(target);
		if (intPtr == (IntPtr)0)
		{
			ThrowHelper.ThrowArgumentNullException(target, "target");
		}
		DontDestroyOnLoad_Injected(intPtr);
	}

	[Obsolete("use Object.Destroy instead.")]
	public static void DestroyObject(Object obj, [DefaultValue("0.0F")] float t)
	{
		Destroy(obj, t);
	}

	[Obsolete("use Object.Destroy instead.")]
	[ExcludeFromDocs]
	public static void DestroyObject(Object obj)
	{
		float t = 0f;
		Destroy(obj, t);
	}

	[Obsolete("Object.FindSceneObjectsOfType has been deprecated, Use Object.FindObjectsByType instead which lets you decide whether you need the results sorted or not.  FindSceneObjectsOfType sorts the results by InstanceID but if you do not need this using FindObjectSortMode.None is considerably faster.", false)]
	public static Object[] FindSceneObjectsOfType(Type type)
	{
		return FindObjectsOfType(type);
	}

	[MethodImpl(MethodImplOptions.InternalCall)]
	[FreeFunction("UnityEngineObjectBindings::FindObjectsOfTypeIncludingAssets")]
	[Obsolete("use Resources.FindObjectsOfTypeAll instead.")]
	[return: UnityMarshalAs(NativeType.ScriptingObjectPtr)]
	public static extern Object[] FindObjectsOfTypeIncludingAssets(Type type);

	[Obsolete("Object.FindObjectsOfType has been deprecated. Use Object.FindObjectsByType instead which lets you decide whether you need the results sorted or not.  FindObjectsOfType sorts the results by InstanceID but if you do not need this using FindObjectSortMode.None is considerably faster.", false)]
	public static T[] FindObjectsOfType<T>() where T : Object
	{
		return Resources.ConvertObjects<T>(FindObjectsOfType(typeof(T), includeInactive: false));
	}

	public static T[] FindObjectsByType<T>(FindObjectsSortMode sortMode) where T : Object
	{
		return Resources.ConvertObjects<T>(FindObjectsByType(typeof(T), FindObjectsInactive.Exclude, sortMode));
	}

	[Obsolete("Object.FindObjectsOfType has been deprecated. Use Object.FindObjectsByType instead which lets you decide whether you need the results sorted or not.  FindObjectsOfType sorts the results by InstanceID but if you do not need this using FindObjectSortMode.None is considerably faster.", false)]
	public static T[] FindObjectsOfType<T>(bool includeInactive) where T : Object
	{
		return Resources.ConvertObjects<T>(FindObjectsOfType(typeof(T), includeInactive));
	}

	public static T[] FindObjectsByType<T>(FindObjectsInactive findObjectsInactive, FindObjectsSortMode sortMode) where T : Object
	{
		return Resources.ConvertObjects<T>(FindObjectsByType(typeof(T), findObjectsInactive, sortMode));
	}

	[Obsolete("Object.FindObjectOfType has been deprecated. Use Object.FindFirstObjectByType instead or if finding any instance is acceptable the faster Object.FindAnyObjectByType", false)]
	public static T FindObjectOfType<T>() where T : Object
	{
		return (T)FindObjectOfType(typeof(T), includeInactive: false);
	}

	[Obsolete("Object.FindObjectOfType has been deprecated. Use Object.FindFirstObjectByType instead or if finding any instance is acceptable the faster Object.FindAnyObjectByType", false)]
	public static T FindObjectOfType<T>(bool includeInactive) where T : Object
	{
		return (T)FindObjectOfType(typeof(T), includeInactive);
	}

	public static T FindFirstObjectByType<T>() where T : Object
	{
		return (T)FindFirstObjectByType(typeof(T), FindObjectsInactive.Exclude);
	}

	public static T FindAnyObjectByType<T>() where T : Object
	{
		return (T)FindAnyObjectByType(typeof(T), FindObjectsInactive.Exclude);
	}

	public static T FindFirstObjectByType<T>(FindObjectsInactive findObjectsInactive) where T : Object
	{
		return (T)FindFirstObjectByType(typeof(T), findObjectsInactive);
	}

	public static T FindAnyObjectByType<T>(FindObjectsInactive findObjectsInactive) where T : Object
	{
		return (T)FindAnyObjectByType(typeof(T), findObjectsInactive);
	}

	[Obsolete("Please use Resources.FindObjectsOfTypeAll instead")]
	public static Object[] FindObjectsOfTypeAll(Type type)
	{
		return Resources.FindObjectsOfTypeAll(type);
	}

	private static void CheckNullArgument(object arg, string message)
	{
		if (arg == null)
		{
			throw new ArgumentException(message);
		}
	}

	[TypeInferenceRule(TypeInferenceRules.TypeReferencedByFirstArgument)]
	[Obsolete("Object.FindObjectOfType has been deprecated. Use Object.FindFirstObjectByType instead or if finding any instance is acceptable the faster Object.FindAnyObjectByType", false)]
	public static Object FindObjectOfType(Type type)
	{
		Object[] array = FindObjectsOfType(type, includeInactive: false);
		if (array.Length != 0)
		{
			return array[0];
		}
		return null;
	}

	public static Object FindFirstObjectByType(Type type)
	{
		Object[] array = FindObjectsByType(type, FindObjectsInactive.Exclude, FindObjectsSortMode.InstanceID);
		return (array.Length != 0) ? array[0] : null;
	}

	public static Object FindAnyObjectByType(Type type)
	{
		Object[] array = FindObjectsByType(type, FindObjectsInactive.Exclude, FindObjectsSortMode.None);
		return (array.Length != 0) ? array[0] : null;
	}

	[TypeInferenceRule(TypeInferenceRules.TypeReferencedByFirstArgument)]
	[Obsolete("Object.FindObjectOfType has been deprecated. Use Object.FindFirstObjectByType instead or if finding any instance is acceptable the faster Object.FindAnyObjectByType", false)]
	public static Object FindObjectOfType(Type type, bool includeInactive)
	{
		Object[] array = FindObjectsOfType(type, includeInactive);
		if (array.Length != 0)
		{
			return array[0];
		}
		return null;
	}

	public static Object FindFirstObjectByType(Type type, FindObjectsInactive findObjectsInactive)
	{
		Object[] array = FindObjectsByType(type, findObjectsInactive, FindObjectsSortMode.InstanceID);
		return (array.Length != 0) ? array[0] : null;
	}

	public static Object FindAnyObjectByType(Type type, FindObjectsInactive findObjectsInactive)
	{
		Object[] array = FindObjectsByType(type, findObjectsInactive, FindObjectsSortMode.None);
		return (array.Length != 0) ? array[0] : null;
	}

	public override string ToString()
	{
		return ToString(this);
	}

	public static bool operator ==(Object x, Object y)
	{
		return CompareBaseObjects(x, y);
	}

	public static bool operator !=(Object x, Object y)
	{
		return !CompareBaseObjects(x, y);
	}

	[MethodImpl(MethodImplOptions.InternalCall)]
	[NativeMethod(Name = "Object::GetOffsetOfInstanceIdMember", IsFreeFunction = true, IsThreadSafe = true)]
	private static extern int GetOffsetOfInstanceIDInCPlusPlusObject();

	[MethodImpl(MethodImplOptions.InternalCall)]
	[NativeMethod(Name = "CurrentThreadIsMainThread", IsFreeFunction = true, IsThreadSafe = true)]
	private static extern bool CurrentThreadIsMainThread();

	[NativeMethod(Name = "CloneObject", IsFreeFunction = true, ThrowsException = true)]
	private static Object Internal_CloneSingle([UnityEngine.Bindings.NotNull] Object data)
	{
		if ((object)data == null)
		{
			ThrowHelper.ThrowArgumentNullException(data, "data");
		}
		IntPtr intPtr = MarshalledUnityObject.MarshalNotNull(data);
		if (intPtr == (IntPtr)0)
		{
			ThrowHelper.ThrowArgumentNullException(data, "data");
		}
		return Unmarshal.UnmarshalUnityObject<Object>(Internal_CloneSingle_Injected(intPtr));
	}

	[FreeFunction("CloneObjectToScene")]
	private static Object Internal_CloneSingleWithScene([UnityEngine.Bindings.NotNull] Object data, Scene scene)
	{
		if ((object)data == null)
		{
			ThrowHelper.ThrowArgumentNullException(data, "data");
		}
		IntPtr intPtr = MarshalledUnityObject.MarshalNotNull(data);
		if (intPtr == (IntPtr)0)
		{
			ThrowHelper.ThrowArgumentNullException(data, "data");
		}
		return Unmarshal.UnmarshalUnityObject<Object>(Internal_CloneSingleWithScene_Injected(intPtr, ref scene));
	}

	[FreeFunction("CloneObjectWithParams")]
	private static Object Internal_CloneSingleWithParams([UnityEngine.Bindings.NotNull] Object data, InstantiateParameters parameters)
	{
		if ((object)data == null)
		{
			ThrowHelper.ThrowArgumentNullException(data, "data");
		}
		IntPtr intPtr = MarshalledUnityObject.MarshalNotNull(data);
		if (intPtr == (IntPtr)0)
		{
			ThrowHelper.ThrowArgumentNullException(data, "data");
		}
		return Unmarshal.UnmarshalUnityObject<Object>(Internal_CloneSingleWithParams_Injected(intPtr, ref parameters));
	}

	[FreeFunction("InstantiateObjectWithParams")]
	private static Object Internal_InstantiateSingleWithParams([UnityEngine.Bindings.NotNull] Object data, Vector3 position, Quaternion rotation, InstantiateParameters parameters)
	{
		if ((object)data == null)
		{
			ThrowHelper.ThrowArgumentNullException(data, "data");
		}
		IntPtr intPtr = MarshalledUnityObject.MarshalNotNull(data);
		if (intPtr == (IntPtr)0)
		{
			ThrowHelper.ThrowArgumentNullException(data, "data");
		}
		return Unmarshal.UnmarshalUnityObject<Object>(Internal_InstantiateSingleWithParams_Injected(intPtr, ref position, ref rotation, ref parameters));
	}

	[FreeFunction("CloneObject")]
	private static Object Internal_CloneSingleWithParent([UnityEngine.Bindings.NotNull] Object data, [UnityEngine.Bindings.NotNull] Transform parent, bool worldPositionStays)
	{
		if ((object)data == null)
		{
			ThrowHelper.ThrowArgumentNullException(data, "data");
		}
		if ((object)parent == null)
		{
			ThrowHelper.ThrowArgumentNullException(parent, "parent");
		}
		IntPtr intPtr = MarshalledUnityObject.MarshalNotNull(data);
		if (intPtr == (IntPtr)0)
		{
			ThrowHelper.ThrowArgumentNullException(data, "data");
		}
		IntPtr intPtr2 = MarshalledUnityObject.MarshalNotNull(parent);
		if (intPtr2 == (IntPtr)0)
		{
			ThrowHelper.ThrowArgumentNullException(parent, "parent");
		}
		return Unmarshal.UnmarshalUnityObject<Object>(Internal_CloneSingleWithParent_Injected(intPtr, intPtr2, worldPositionStays));
	}

	[FreeFunction("InstantiateAsyncObjects")]
	private static IntPtr Internal_InstantiateAsyncWithParams([UnityEngine.Bindings.NotNull] Object original, int count, InstantiateParameters parameters, IntPtr positions, int positionsCount, IntPtr rotations, int rotationsCount)
	{
		if ((object)original == null)
		{
			ThrowHelper.ThrowArgumentNullException(original, "original");
		}
		IntPtr intPtr = MarshalledUnityObject.MarshalNotNull(original);
		if (intPtr == (IntPtr)0)
		{
			ThrowHelper.ThrowArgumentNullException(original, "original");
		}
		return Internal_InstantiateAsyncWithParams_Injected(intPtr, count, ref parameters, positions, positionsCount, rotations, rotationsCount);
	}

	[FreeFunction("InstantiateObject")]
	private static Object Internal_InstantiateSingle([UnityEngine.Bindings.NotNull] Object data, Vector3 pos, Quaternion rot)
	{
		if ((object)data == null)
		{
			ThrowHelper.ThrowArgumentNullException(data, "data");
		}
		IntPtr intPtr = MarshalledUnityObject.MarshalNotNull(data);
		if (intPtr == (IntPtr)0)
		{
			ThrowHelper.ThrowArgumentNullException(data, "data");
		}
		return Unmarshal.UnmarshalUnityObject<Object>(Internal_InstantiateSingle_Injected(intPtr, ref pos, ref rot));
	}

	[FreeFunction("InstantiateObject")]
	private static Object Internal_InstantiateSingleWithParent([UnityEngine.Bindings.NotNull] Object data, [UnityEngine.Bindings.NotNull] Transform parent, Vector3 pos, Quaternion rot)
	{
		if ((object)data == null)
		{
			ThrowHelper.ThrowArgumentNullException(data, "data");
		}
		if ((object)parent == null)
		{
			ThrowHelper.ThrowArgumentNullException(parent, "parent");
		}
		IntPtr intPtr = MarshalledUnityObject.MarshalNotNull(data);
		if (intPtr == (IntPtr)0)
		{
			ThrowHelper.ThrowArgumentNullException(data, "data");
		}
		IntPtr intPtr2 = MarshalledUnityObject.MarshalNotNull(parent);
		if (intPtr2 == (IntPtr)0)
		{
			ThrowHelper.ThrowArgumentNullException(parent, "parent");
		}
		return Unmarshal.UnmarshalUnityObject<Object>(Internal_InstantiateSingleWithParent_Injected(intPtr, intPtr2, ref pos, ref rot));
	}

	[FreeFunction("UnityEngineObjectBindings::ToString")]
	private static string ToString(Object obj)
	{
		ManagedSpanWrapper ret = default(ManagedSpanWrapper);
		string stringAndDispose;
		try
		{
			ToString_Injected(MarshalledUnityObject.Marshal(obj), out ret);
		}
		finally
		{
			stringAndDispose = OutStringMarshaller.GetStringAndDispose(ret);
		}
		return stringAndDispose;
	}

	[FreeFunction("UnityEngineObjectBindings::GetName", HasExplicitThis = true)]
	private string GetName()
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
			GetName_Injected(intPtr, out ret);
		}
		finally
		{
			stringAndDispose = OutStringMarshaller.GetStringAndDispose(ret);
		}
		return stringAndDispose;
	}

	[FreeFunction("UnityEngineObjectBindings::IsPersistent")]
	internal static bool IsPersistent([UnityEngine.Bindings.NotNull] Object obj)
	{
		if ((object)obj == null)
		{
			ThrowHelper.ThrowArgumentNullException(obj, "obj");
		}
		IntPtr intPtr = MarshalledUnityObject.MarshalNotNull(obj);
		if (intPtr == (IntPtr)0)
		{
			ThrowHelper.ThrowArgumentNullException(obj, "obj");
		}
		return IsPersistent_Injected(intPtr);
	}

	[FreeFunction("UnityEngineObjectBindings::SetName", HasExplicitThis = true)]
	private unsafe void SetName(string name)
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
			if (!StringMarshaller.TryMarshalEmptyOrNullString(name, ref managedSpanWrapper))
			{
				ReadOnlySpan<char> readOnlySpan = MemoryExtensions.AsSpan(name);
				fixed (char* begin = readOnlySpan)
				{
					managedSpanWrapper = new ManagedSpanWrapper(begin, readOnlySpan.Length);
					SetName_Injected(intPtr, ref managedSpanWrapper);
					return;
				}
			}
			SetName_Injected(intPtr, ref managedSpanWrapper);
		}
		finally
		{
		}
	}

	[NativeMethod(Name = "UnityEngineObjectBindings::DoesObjectWithInstanceIDExist", IsFreeFunction = true, IsThreadSafe = true)]
	internal static bool DoesObjectWithInstanceIDExist(EntityId instanceID)
	{
		return DoesObjectWithInstanceIDExist_Injected(ref instanceID);
	}

	[FreeFunction("UnityEngineObjectBindings::FindObjectFromInstanceID")]
	[VisibleToOtherModules]
	internal static Object FindObjectFromInstanceID(EntityId instanceID)
	{
		return Unmarshal.UnmarshalUnityObject<Object>(FindObjectFromInstanceID_Injected(ref instanceID));
	}

	[FreeFunction("UnityEngineObjectBindings::GetPtrFromInstanceID")]
	private static IntPtr GetPtrFromInstanceID(EntityId instanceID, Type objectType, out bool isMonoBehaviour)
	{
		return GetPtrFromInstanceID_Injected(ref instanceID, objectType, out isMonoBehaviour);
	}

	[FreeFunction("UnityEngineObjectBindings::ForceLoadFromInstanceID")]
	[VisibleToOtherModules]
	internal static Object ForceLoadFromInstanceID(EntityId instanceID)
	{
		return Unmarshal.UnmarshalUnityObject<Object>(ForceLoadFromInstanceID_Injected(ref instanceID));
	}

	[FreeFunction("UnityEngineObjectBindings::MarkObjectDirty", HasExplicitThis = true)]
	internal void MarkDirty()
	{
		IntPtr intPtr = MarshalledUnityObject.MarshalNotNull(this);
		if (intPtr == (IntPtr)0)
		{
			ThrowHelper.ThrowNullReferenceException(this);
		}
		MarkDirty_Injected(intPtr);
	}

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern void Destroy_Injected(IntPtr obj, [DefaultValue("0.0F")] float t);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern void DestroyImmediate_Injected(IntPtr obj, [DefaultValue("false")] bool allowDestroyingAssets);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern void DontDestroyOnLoad_Injected(IntPtr target);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern HideFlags get_hideFlags_Injected(IntPtr _unity_self);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern void set_hideFlags_Injected(IntPtr _unity_self, HideFlags value);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern IntPtr Internal_CloneSingle_Injected(IntPtr data);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern IntPtr Internal_CloneSingleWithScene_Injected(IntPtr data, [In] ref Scene scene);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern IntPtr Internal_CloneSingleWithParams_Injected(IntPtr data, [In] ref InstantiateParameters parameters);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern IntPtr Internal_InstantiateSingleWithParams_Injected(IntPtr data, [In] ref Vector3 position, [In] ref Quaternion rotation, [In] ref InstantiateParameters parameters);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern IntPtr Internal_CloneSingleWithParent_Injected(IntPtr data, IntPtr parent, bool worldPositionStays);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern IntPtr Internal_InstantiateAsyncWithParams_Injected(IntPtr original, int count, [In] ref InstantiateParameters parameters, IntPtr positions, int positionsCount, IntPtr rotations, int rotationsCount);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern IntPtr Internal_InstantiateSingle_Injected(IntPtr data, [In] ref Vector3 pos, [In] ref Quaternion rot);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern IntPtr Internal_InstantiateSingleWithParent_Injected(IntPtr data, IntPtr parent, [In] ref Vector3 pos, [In] ref Quaternion rot);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern void ToString_Injected(IntPtr obj, out ManagedSpanWrapper ret);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern void GetName_Injected(IntPtr _unity_self, out ManagedSpanWrapper ret);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern bool IsPersistent_Injected(IntPtr obj);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern void SetName_Injected(IntPtr _unity_self, ref ManagedSpanWrapper name);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern bool DoesObjectWithInstanceIDExist_Injected([In] ref EntityId instanceID);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern IntPtr FindObjectFromInstanceID_Injected([In] ref EntityId instanceID);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern IntPtr GetPtrFromInstanceID_Injected([In] ref EntityId instanceID, Type objectType, out bool isMonoBehaviour);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern IntPtr ForceLoadFromInstanceID_Injected([In] ref EntityId instanceID);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern void MarkDirty_Injected(IntPtr _unity_self);
}
