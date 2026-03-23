using System;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using UnityEngine.Bindings;

namespace UnityEngine;

[StructLayout(LayoutKind.Sequential)]
[NativeAsStruct]
[NativeHeader("Runtime/Utilities/DiagnosticSwitch.h")]
[VisibleToOtherModules(new string[] { "UnityEngine.UIElementsModule", "UnityEngine.TextCoreTextEngineModule", "UnityEngine.IMGUIModule" })]
[NativeClass("DiagnosticSwitch", "struct DiagnosticSwitch;")]
internal class DiagnosticSwitch
{
	[Flags]
	internal enum Flags
	{
		None = 0,
		CanChangeAfterEngineStart = 1,
		PropagateToAssetImportWorkerProcess = 2
	}

	internal static class BindingsMarshaller
	{
		public static IntPtr ConvertToNative(DiagnosticSwitch diagnosticSwitch)
		{
			return diagnosticSwitch.m_Ptr;
		}

		public static DiagnosticSwitch ConvertToManaged(IntPtr ptr)
		{
			return new DiagnosticSwitch(ptr);
		}
	}

	private IntPtr m_Ptr;

	public string name
	{
		get
		{
			ManagedSpanWrapper ret = default(ManagedSpanWrapper);
			string stringAndDispose;
			try
			{
				IntPtr intPtr = BindingsMarshaller.ConvertToNative(this);
				if (intPtr == (IntPtr)0)
				{
					ThrowHelper.ThrowNullReferenceException(this);
				}
				get_name_Injected(intPtr, out ret);
			}
			finally
			{
				stringAndDispose = OutStringMarshaller.GetStringAndDispose(ret);
			}
			return stringAndDispose;
		}
	}

	public string description
	{
		get
		{
			ManagedSpanWrapper ret = default(ManagedSpanWrapper);
			string stringAndDispose;
			try
			{
				IntPtr intPtr = BindingsMarshaller.ConvertToNative(this);
				if (intPtr == (IntPtr)0)
				{
					ThrowHelper.ThrowNullReferenceException(this);
				}
				get_description_Injected(intPtr, out ret);
			}
			finally
			{
				stringAndDispose = OutStringMarshaller.GetStringAndDispose(ret);
			}
			return stringAndDispose;
		}
	}

	[NativeName("OwningModuleName")]
	public string owningModule
	{
		get
		{
			ManagedSpanWrapper ret = default(ManagedSpanWrapper);
			string stringAndDispose;
			try
			{
				IntPtr intPtr = BindingsMarshaller.ConvertToNative(this);
				if (intPtr == (IntPtr)0)
				{
					ThrowHelper.ThrowNullReferenceException(this);
				}
				get_owningModule_Injected(intPtr, out ret);
			}
			finally
			{
				stringAndDispose = OutStringMarshaller.GetStringAndDispose(ret);
			}
			return stringAndDispose;
		}
	}

	public Flags flags
	{
		get
		{
			IntPtr intPtr = BindingsMarshaller.ConvertToNative(this);
			if (intPtr == (IntPtr)0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			return get_flags_Injected(intPtr);
		}
	}

	public object value
	{
		get
		{
			return GetScriptingValue();
		}
		set
		{
			SetScriptingValue(value, setPersistent: false);
		}
	}

	[NativeName("ScriptingDefaultValue")]
	public object defaultValue
	{
		get
		{
			IntPtr intPtr = BindingsMarshaller.ConvertToNative(this);
			if (intPtr == (IntPtr)0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			return get_defaultValue_Injected(intPtr);
		}
	}

	[NativeName("ScriptingMinValue")]
	public object minValue
	{
		get
		{
			IntPtr intPtr = BindingsMarshaller.ConvertToNative(this);
			if (intPtr == (IntPtr)0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			return get_minValue_Injected(intPtr);
		}
	}

	[NativeName("ScriptingMaxValue")]
	public object maxValue
	{
		get
		{
			IntPtr intPtr = BindingsMarshaller.ConvertToNative(this);
			if (intPtr == (IntPtr)0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			return get_maxValue_Injected(intPtr);
		}
	}

	public object persistentValue
	{
		get
		{
			return GetScriptingPersistentValue();
		}
		set
		{
			SetScriptingValue(value, setPersistent: true);
		}
	}

	[NativeName("ScriptingEnumInfo")]
	public EnumInfo enumInfo
	{
		get
		{
			IntPtr intPtr = BindingsMarshaller.ConvertToNative(this);
			if (intPtr == (IntPtr)0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			return get_enumInfo_Injected(intPtr);
		}
	}

	public bool isSetToDefault => object.Equals(persistentValue, defaultValue);

	public bool needsRestart => !object.Equals(value, persistentValue);

	private DiagnosticSwitch(IntPtr ptr)
	{
		m_Ptr = ptr;
	}

	private object GetScriptingValue()
	{
		IntPtr intPtr = BindingsMarshaller.ConvertToNative(this);
		if (intPtr == (IntPtr)0)
		{
			ThrowHelper.ThrowNullReferenceException(this);
		}
		return GetScriptingValue_Injected(intPtr);
	}

	private object GetScriptingPersistentValue()
	{
		IntPtr intPtr = BindingsMarshaller.ConvertToNative(this);
		if (intPtr == (IntPtr)0)
		{
			ThrowHelper.ThrowNullReferenceException(this);
		}
		return GetScriptingPersistentValue_Injected(intPtr);
	}

	[NativeThrows]
	private void SetScriptingValue(object value, bool setPersistent)
	{
		IntPtr intPtr = BindingsMarshaller.ConvertToNative(this);
		if (intPtr == (IntPtr)0)
		{
			ThrowHelper.ThrowNullReferenceException(this);
		}
		SetScriptingValue_Injected(intPtr, value, setPersistent);
	}

	public override string ToString()
	{
		string text = ((value == null) ? "<null>" : value.ToString());
		return name + " = " + text;
	}

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern void get_name_Injected(IntPtr _unity_self, out ManagedSpanWrapper ret);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern void get_description_Injected(IntPtr _unity_self, out ManagedSpanWrapper ret);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern void get_owningModule_Injected(IntPtr _unity_self, out ManagedSpanWrapper ret);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern Flags get_flags_Injected(IntPtr _unity_self);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern object get_defaultValue_Injected(IntPtr _unity_self);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern object get_minValue_Injected(IntPtr _unity_self);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern object get_maxValue_Injected(IntPtr _unity_self);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern EnumInfo get_enumInfo_Injected(IntPtr _unity_self);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern object GetScriptingValue_Injected(IntPtr _unity_self);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern object GetScriptingPersistentValue_Injected(IntPtr _unity_self);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern void SetScriptingValue_Injected(IntPtr _unity_self, object value, bool setPersistent);
}
