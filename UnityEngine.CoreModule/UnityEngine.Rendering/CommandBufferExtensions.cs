using System;
using System.Runtime.CompilerServices;
using UnityEngine.Bindings;
using UnityEngine.Scripting;

namespace UnityEngine.Rendering;

[NativeHeader("Runtime/Export/Graphics/RenderingCommandBufferExtensions.bindings.h")]
[UsedByNativeCode]
public static class CommandBufferExtensions
{
	[FreeFunction("RenderingCommandBufferExtensions_Bindings::Internal_SwitchIntoFastMemory")]
	private static void Internal_SwitchIntoFastMemory([NotNull] CommandBuffer cmd, ref RenderTargetIdentifier rt, FastMemoryFlags fastMemoryFlags, float residency, bool copyContents)
	{
		if (cmd == null)
		{
			ThrowHelper.ThrowArgumentNullException(cmd, "cmd");
		}
		IntPtr intPtr = CommandBuffer.BindingsMarshaller.ConvertToNative(cmd);
		if (intPtr == (IntPtr)0)
		{
			ThrowHelper.ThrowArgumentNullException(cmd, "cmd");
		}
		Internal_SwitchIntoFastMemory_Injected(intPtr, ref rt, fastMemoryFlags, residency, copyContents);
	}

	[FreeFunction("RenderingCommandBufferExtensions_Bindings::Internal_SwitchOutOfFastMemory")]
	private static void Internal_SwitchOutOfFastMemory([NotNull] CommandBuffer cmd, ref RenderTargetIdentifier rt, bool copyContents)
	{
		if (cmd == null)
		{
			ThrowHelper.ThrowArgumentNullException(cmd, "cmd");
		}
		IntPtr intPtr = CommandBuffer.BindingsMarshaller.ConvertToNative(cmd);
		if (intPtr == (IntPtr)0)
		{
			ThrowHelper.ThrowArgumentNullException(cmd, "cmd");
		}
		Internal_SwitchOutOfFastMemory_Injected(intPtr, ref rt, copyContents);
	}

	[NativeConditional("UNITY_XBOXONE || UNITY_GAMECORE_XBOXONE")]
	public static void SwitchIntoFastMemory(this CommandBuffer cmd, RenderTargetIdentifier rid, FastMemoryFlags fastMemoryFlags, float residency, bool copyContents)
	{
		Internal_SwitchIntoFastMemory(cmd, ref rid, fastMemoryFlags, residency, copyContents);
	}

	[NativeConditional("UNITY_XBOXONE || UNITY_GAMECORE_XBOXONE")]
	public static void SwitchOutOfFastMemory(this CommandBuffer cmd, RenderTargetIdentifier rid, bool copyContents)
	{
		Internal_SwitchOutOfFastMemory(cmd, ref rid, copyContents);
	}

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern void Internal_SwitchIntoFastMemory_Injected(IntPtr cmd, ref RenderTargetIdentifier rt, FastMemoryFlags fastMemoryFlags, float residency, bool copyContents);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern void Internal_SwitchOutOfFastMemory_Injected(IntPtr cmd, ref RenderTargetIdentifier rt, bool copyContents);
}
