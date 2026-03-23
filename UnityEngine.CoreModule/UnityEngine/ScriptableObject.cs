using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using UnityEngine.Bindings;
using UnityEngine.Scripting;

namespace UnityEngine;

[StructLayout(LayoutKind.Sequential)]
[NativeClass(null)]
[NativeHeader("Runtime/Mono/MonoBehaviour.h")]
[ExtensionOfNativeClass]
[RequiredByNativeCode]
public class ScriptableObject : Object
{
	public ScriptableObject()
	{
		CreateScriptableObject(this);
	}

	[Obsolete("Use EditorUtility.SetDirty instead")]
	[NativeConditional("ENABLE_MONO")]
	public void SetDirty()
	{
		IntPtr intPtr = MarshalledUnityObject.MarshalNotNull(this);
		if (intPtr == (IntPtr)0)
		{
			ThrowHelper.ThrowNullReferenceException(this);
		}
		SetDirty_Injected(intPtr);
	}

	public static ScriptableObject CreateInstance(string className)
	{
		return CreateScriptableObjectInstanceFromName(className);
	}

	public static ScriptableObject CreateInstance(Type type)
	{
		return CreateScriptableObjectInstanceFromType(type, applyDefaultsAndReset: true);
	}

	public static T CreateInstance<T>() where T : ScriptableObject
	{
		return (T)CreateInstance(typeof(T));
	}

	[EditorBrowsable(EditorBrowsableState.Never)]
	internal static ScriptableObject CreateInstance(Type type, Action<ScriptableObject> initialize)
	{
		if (!typeof(ScriptableObject).IsAssignableFrom(type))
		{
			throw new ArgumentException("Type must inherit ScriptableObject.", "type");
		}
		ScriptableObject scriptableObject = CreateScriptableObjectInstanceFromType(type, applyDefaultsAndReset: false);
		try
		{
			initialize(scriptableObject);
		}
		finally
		{
			ResetAndApplyDefaultInstances(scriptableObject);
		}
		return scriptableObject;
	}

	[MethodImpl(MethodImplOptions.InternalCall)]
	[NativeMethod(IsThreadSafe = true, ThrowsException = true)]
	private static extern void CreateScriptableObject([Writable] ScriptableObject self);

	[FreeFunction("Scripting::CreateScriptableObject")]
	private unsafe static ScriptableObject CreateScriptableObjectInstanceFromName(string className)
	{
		//The blocks IL_0029 are reachable both inside and outside the pinned region starting at IL_0018. ILSpy has duplicated these blocks in order to place them both within and outside the `fixed` statement.
		IntPtr gcHandlePtr = default(IntPtr);
		ScriptableObject result;
		try
		{
			ManagedSpanWrapper managedSpanWrapper = default(ManagedSpanWrapper);
			if (!StringMarshaller.TryMarshalEmptyOrNullString(className, ref managedSpanWrapper))
			{
				ReadOnlySpan<char> readOnlySpan = MemoryExtensions.AsSpan(className);
				fixed (char* begin = readOnlySpan)
				{
					managedSpanWrapper = new ManagedSpanWrapper(begin, readOnlySpan.Length);
					gcHandlePtr = CreateScriptableObjectInstanceFromName_Injected(ref managedSpanWrapper);
				}
			}
			else
			{
				gcHandlePtr = CreateScriptableObjectInstanceFromName_Injected(ref managedSpanWrapper);
			}
		}
		finally
		{
			result = Unmarshal.UnmarshalUnityObject<ScriptableObject>(gcHandlePtr);
		}
		return result;
	}

	[NativeMethod(Name = "Scripting::CreateScriptableObjectWithType", IsFreeFunction = true, ThrowsException = true)]
	internal static ScriptableObject CreateScriptableObjectInstanceFromType(Type type, bool applyDefaultsAndReset)
	{
		return Unmarshal.UnmarshalUnityObject<ScriptableObject>(CreateScriptableObjectInstanceFromType_Injected(type, applyDefaultsAndReset));
	}

	[FreeFunction("Scripting::ResetAndApplyDefaultInstances")]
	internal static void ResetAndApplyDefaultInstances([NotNull] Object obj)
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
		ResetAndApplyDefaultInstances_Injected(intPtr);
	}

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern void SetDirty_Injected(IntPtr _unity_self);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern IntPtr CreateScriptableObjectInstanceFromName_Injected(ref ManagedSpanWrapper className);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern IntPtr CreateScriptableObjectInstanceFromType_Injected(Type type, bool applyDefaultsAndReset);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern void ResetAndApplyDefaultInstances_Injected(IntPtr obj);
}
