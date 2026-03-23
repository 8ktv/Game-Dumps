using System;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using UnityEngine.Bindings;

namespace UnityEngine;

[NativeHeader("Runtime/Director/Core/ExposedPropertyTable.bindings.h")]
[NativeHeader("Runtime/Utilities/PropertyName.h")]
public struct ExposedPropertyResolver
{
	internal IntPtr table;

	internal static Object ResolveReferenceInternal(IntPtr ptr, PropertyName name, out bool isValid)
	{
		if (ptr == IntPtr.Zero)
		{
			throw new ArgumentNullException("Argument \"ptr\" can't be null.");
		}
		return ResolveReferenceBindingsInternal(ptr, name, out isValid);
	}

	[FreeFunction("ExposedPropertyTableBindings::ResolveReferenceInternal")]
	private static Object ResolveReferenceBindingsInternal(IntPtr ptr, PropertyName name, out bool isValid)
	{
		return Unmarshal.UnmarshalUnityObject<Object>(ResolveReferenceBindingsInternal_Injected(ptr, ref name, out isValid));
	}

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern IntPtr ResolveReferenceBindingsInternal_Injected(IntPtr ptr, [In] ref PropertyName name, out bool isValid);
}
