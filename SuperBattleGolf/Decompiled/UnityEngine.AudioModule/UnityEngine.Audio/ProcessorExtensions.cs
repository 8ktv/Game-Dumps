using System;
using Unity.Collections;
using Unity.Collections.LowLevel.Unsafe;

namespace UnityEngine.Audio;

internal static class ProcessorExtensions
{
	internal unsafe static T* CAllocChunk<T>() where T : unmanaged
	{
		T* ptr = (T*)UnsafeUtility.MallocTracked(sizeof(T), UnsafeUtility.AlignOf<T>(), Allocator.Persistent, 3);
		*ptr = default(T);
		return ptr;
	}

	public unsafe static void DispatchGenericControl<TControl, TRealtime>(ref TControl control, ref TRealtime realtime, in ProcessorHeader header, void* additionalPtr, ControlFunction function) where TControl : unmanaged, ProcessorInstance.IControl<TRealtime> where TRealtime : unmanaged, ProcessorInstance.IRealtime
	{
		switch (function)
		{
		case ControlFunction.Dispose:
			control.Dispose(new ControlContext(((DisposeArguments*)additionalPtr)->ControlContext), ref realtime);
			fixed (ProcessorHeader* memory = &header)
			{
				UnsafeUtility.FreeTracked(memory, Allocator.Persistent);
			}
			break;
		case ControlFunction.Update:
			control.Update(new ControlContext(((UpdateArguments*)additionalPtr)->ControlContext), new ProcessorInstance.Pipe(((UpdateArguments*)additionalPtr)->Self, null));
			break;
		case ControlFunction.Message:
			((MessageArguments*)additionalPtr)->StatusReturn = control.OnMessage(new ControlContext(((MessageArguments*)additionalPtr)->Context), new ProcessorInstance.Pipe(((MessageArguments*)additionalPtr)->Self, null), *((MessageArguments*)additionalPtr)->MessageData);
			break;
		default:
			throw new ArgumentOutOfRangeException();
		}
	}

	public unsafe static void DispatchGenericProcessor<T>(ref T processor, in ProcessorHeader header, void* additionalPtr, ProcessorFunction function) where T : unmanaged, ProcessorInstance.IRealtime
	{
		if (function == ProcessorFunction.Update)
		{
			ProcessorRealtimeUpdateArguments* ptr = *(ProcessorRealtimeUpdateArguments**)additionalPtr;
			processor.Update(new ProcessorInstance.UpdatedDataContext(in ptr->Access), new ProcessorInstance.Pipe(ptr->Self, ptr->Head));
			return;
		}
		throw new ArgumentOutOfRangeException();
	}
}
