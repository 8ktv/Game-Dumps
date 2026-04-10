using System;
using Unity.Audio;
using UnityEngine.Bindings;
using UnityEngine.Scripting;

namespace UnityEngine.Audio;

[NativeHeader("Modules/Audio/Public/ScriptableProcessors/ScriptableProcessor.h")]
[RequiredByNativeCode]
internal struct ProcessorHeader
{
	private unsafe void* m_Control;

	internal Handle DualThreadHandle;

	internal unsafe delegate* unmanaged[Cdecl]<ProcessorHeader*, ProcessorFunction, void*, void> NativeProcessorFunction;

	internal unsafe delegate* unmanaged[Cdecl]<ProcessorHeader*, ControlFunction, void*, void> NativeControlFunction;

	internal IntPtr ProcessorReflectionData;

	internal IntPtr ControlReflectionData;

	public unsafe void InvokeProcessor(ProcessorFunction fn, void* args)
	{
		fixed (ProcessorHeader* ptr = &this)
		{
			if (fn - 2 <= ProcessorFunction.OutputProcess)
			{
				throw new NotSupportedException($"Cannot manually invoke {fn}, these are called automatically");
			}
			NativeProcessorFunction(ptr, fn, args);
		}
	}

	public unsafe bool IsSameControl(ControlHeader* other)
	{
		return m_Control == other;
	}
}
