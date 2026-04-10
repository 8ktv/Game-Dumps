using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using Unity.Audio;
using UnityEngine.Bindings;

namespace UnityEngine.Audio;

[NativeHeader("Modules/Audio/Public/ScriptableProcessors/ScriptBindings/ScriptableProcessor.bindings.h")]
internal static class ScriptableProcessorBindings
{
	public unsafe static void QueueProcessorDispose(ProcessorHeader* header, ControlHeader* control)
	{
		QueueProcessorDisposeInternal(header, control);
	}

	public unsafe static bool AddDataToProcessorHandle(ControlHeader* control, in Handle handle, void* data, int size, int align, long typeHash)
	{
		return AddDataToProcessorHandleInternal(control, in handle, data, size, align, typeHash);
	}

	public unsafe static ProcessorInstance.AvailableData.Element* GetAvailableDataForRealtime(in RealtimeAccess access, in Handle handle)
	{
		fixed (RealtimeAccess* access2 = &access)
		{
			return (ProcessorInstance.AvailableData.Element*)GetRealtimeDataElementListForProcessorInternal(access2, in handle);
		}
	}

	public unsafe static ProcessorInstance.AvailableData.Element* GetAvailableDataForControl(ControlHeader* control, in Handle handle)
	{
		return (ProcessorInstance.AvailableData.Element*)GetControlDataElementListForProcessorInternal(control, in handle);
	}

	public unsafe static void ReturnDataFromProcessor(in RealtimeAccess access, in Handle handle, void* data, int size, int align, long typeHash)
	{
		fixed (RealtimeAccess* access2 = &access)
		{
			ReturnDataFromProcessorInternal(access2, in handle, data, size, align, typeHash);
		}
	}

	public unsafe static void ValidateCanProcess(in Handle handle, in RealtimeContext ctx)
	{
		fixed (RealtimeContext* processingContext = &ctx)
		{
			ValidateCanProcessInternal(in handle, processingContext);
		}
	}

	public unsafe static bool CheckProcessorExists(Handle handle, ControlHeader* control)
	{
		return CheckProcessorExistsInternal(handle, control);
	}

	public unsafe static void PerformRecursiveConfigure(Handle handle, ControlHeader* control, in AudioConfiguration configuration)
	{
		PerformRecursiveConfigureInternal(handle, control, in configuration);
	}

	public unsafe static void PerformRecursiveUpdate(Handle handle, ControlHeader* control)
	{
		PerformRecursiveUpdateInternal(handle, control);
	}

	public unsafe static ProcessorInstance.Response SendMessageToProcessor(ProcessorHeader* header, ControlHeader* control, ProcessorInstance.Message* message)
	{
		return SendMessageToProcessorInternal(header, control, message);
	}

	[MethodImpl(MethodImplOptions.InternalCall)]
	[NativeMethod(Name = "audio::SendMessageToProcessor", IsFreeFunction = true, ThrowsException = true)]
	private unsafe static extern ProcessorInstance.Response SendMessageToProcessorInternal(void* header, void* control, void* message);

	[NativeMethod(Name = "audio::PerformRecursiveUpdate", IsFreeFunction = true, ThrowsException = true)]
	private unsafe static void PerformRecursiveUpdateInternal(Handle handle, void* control)
	{
		PerformRecursiveUpdateInternal_Injected(ref handle, control);
	}

	[NativeMethod(Name = "audio::PerformRecursiveConfigure", IsFreeFunction = true, ThrowsException = true)]
	private unsafe static void PerformRecursiveConfigureInternal(Handle handle, void* control, in AudioConfiguration configuration)
	{
		PerformRecursiveConfigureInternal_Injected(ref handle, control, in configuration);
	}

	[MethodImpl(MethodImplOptions.InternalCall)]
	[NativeMethod(Name = "audio::ValidateCanProcess", IsFreeFunction = true, IsThreadSafe = true, ThrowsException = true)]
	private unsafe static extern void ValidateCanProcessInternal(in Handle handle, void* processingContext);

	[MethodImpl(MethodImplOptions.InternalCall)]
	[NativeMethod(Name = "audio::QueueProcessorDispose", IsFreeFunction = true, ThrowsException = true)]
	private unsafe static extern void QueueProcessorDisposeInternal(void* header, void* control);

	[MethodImpl(MethodImplOptions.InternalCall)]
	[NativeMethod(Name = "audio::GetRealtimeDataElementListForProcessor", IsFreeFunction = true, ThrowsException = true)]
	private unsafe static extern void* GetRealtimeDataElementListForProcessorInternal(void* access, in Handle handle);

	[MethodImpl(MethodImplOptions.InternalCall)]
	[NativeMethod(Name = "audio::GetControlDataElementListForProcessor", IsFreeFunction = true)]
	private unsafe static extern void* GetControlDataElementListForProcessorInternal(void* control, in Handle handle);

	[MethodImpl(MethodImplOptions.InternalCall)]
	[NativeMethod(Name = "audio::ReturnDataFromProcessor", IsFreeFunction = true, IsThreadSafe = true, ThrowsException = true)]
	private unsafe static extern void ReturnDataFromProcessorInternal(void* access, in Handle handle, void* data, int size, int align, long typeHash);

	[MethodImpl(MethodImplOptions.InternalCall)]
	[NativeMethod(Name = "audio::AddDataToProcessor", IsFreeFunction = true, ThrowsException = true)]
	private unsafe static extern bool AddDataToProcessorHandleInternal(void* control, in Handle handle, void* data, int size, int align, long typeHash);

	[NativeMethod(Name = "audio::CheckProcessorExists", IsFreeFunction = true)]
	private unsafe static bool CheckProcessorExistsInternal(Handle handle, void* control)
	{
		return CheckProcessorExistsInternal_Injected(ref handle, control);
	}

	[MethodImpl(MethodImplOptions.InternalCall)]
	[NativeMethod(Name = "audio::ThrowScriptingExceptionForTest", IsFreeFunction = true, IsThreadSafe = true, ThrowsException = true)]
	internal static extern void ThrowScriptingExceptionForTest();

	[MethodImpl(MethodImplOptions.InternalCall)]
	private unsafe static extern void PerformRecursiveUpdateInternal_Injected([In] ref Handle handle, void* control);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private unsafe static extern void PerformRecursiveConfigureInternal_Injected([In] ref Handle handle, void* control, in AudioConfiguration configuration);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private unsafe static extern bool CheckProcessorExistsInternal_Injected([In] ref Handle handle, void* control);
}
