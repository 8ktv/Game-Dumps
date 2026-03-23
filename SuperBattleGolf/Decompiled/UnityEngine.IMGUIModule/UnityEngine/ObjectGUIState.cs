using System;
using System.Runtime.CompilerServices;
using UnityEngine.Bindings;

namespace UnityEngine;

[VisibleToOtherModules(new string[] { "UnityEngine.UIElementsModule" })]
[NativeHeader("Modules/IMGUI/GUIState.h")]
internal class ObjectGUIState : IDisposable
{
	internal static class BindingsMarshaller
	{
		public static IntPtr ConvertToNative(ObjectGUIState objectGUIState)
		{
			return objectGUIState.m_Ptr;
		}
	}

	internal IntPtr m_Ptr;

	public ObjectGUIState()
	{
		m_Ptr = Internal_Create();
	}

	public void Dispose()
	{
		Destroy();
		GC.SuppressFinalize(this);
	}

	~ObjectGUIState()
	{
		Destroy();
	}

	private void Destroy()
	{
		if (m_Ptr != IntPtr.Zero)
		{
			Internal_Destroy(m_Ptr);
			m_Ptr = IntPtr.Zero;
		}
	}

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern IntPtr Internal_Create();

	[MethodImpl(MethodImplOptions.InternalCall)]
	[NativeMethod(IsThreadSafe = true)]
	private static extern void Internal_Destroy(IntPtr ptr);
}
