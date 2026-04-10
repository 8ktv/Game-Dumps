using System;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using Unity.Audio;
using Unity.Burst;
using Unity.Collections.LowLevel.Unsafe;
using Unity.Jobs;
using Unity.Jobs.LowLevel.Unsafe;
using UnityEngine.Bindings;

namespace UnityEngine.Audio;

[NativeHeader("Modules/Audio/Public/ScriptableProcessors/ScriptBindings/ScriptableProcessor.bindings.h")]
internal static class IRootOutputProcessorExtensions
{
	internal struct ProcessPhaseUpdateArguments
	{
		internal unsafe RealtimeContext* Context;

		internal JobHandle InOut;

		internal Handle Self;

		internal unsafe float* AudioBuffer;

		internal int OutputFrameCount;

		internal int OutputChannelCount;
	}

	[StructLayout(LayoutKind.Sequential, Size = 1)]
	internal struct JobStruct<TUserProcessor> where TUserProcessor : unmanaged, RootOutputInstance.IRealtime
	{
		internal struct Storage
		{
			public ProcessorHeader Header;

			public TUserProcessor UserProcessor;
		}

		internal delegate void ExecuteJobFunction(ref Storage storage, IntPtr additionalPtr, IntPtr additionalPtr2, ref JobRanges ranges, int jobIndex);

		internal static readonly BurstLike.SharedStatic<IntPtr> jobReflectionData = BurstLike.SharedStatic<IntPtr>.GetOrCreate<JobStruct<TUserProcessor>>();

		[BurstDiscard]
		internal static void Initialize()
		{
			if (jobReflectionData.Data == IntPtr.Zero)
			{
				jobReflectionData.Data = JobsUtility.CreateJobReflectionData(typeof(Storage), new ExecuteJobFunction(Execute));
			}
		}

		public unsafe static void Execute(ref Storage storage, IntPtr additionalPtr, IntPtr processorFunction, ref JobRanges ranges, int jobIndex)
		{
			ProcessorFunction processorFunction2 = (ProcessorFunction)(int)processorFunction;
			switch (processorFunction2)
			{
			case ProcessorFunction.OutputProcessEarly:
			{
				ProcessPhaseUpdateArguments* ptr3 = (ProcessPhaseUpdateArguments*)(void*)additionalPtr;
				ptr3->InOut = storage.UserProcessor.EarlyProcessing(in *ptr3->Context, new ProcessorInstance.Pipe(ptr3->Self, null));
				break;
			}
			case ProcessorFunction.OutputProcess:
			{
				ProcessPhaseUpdateArguments* ptr2 = (ProcessPhaseUpdateArguments*)(void*)additionalPtr;
				storage.UserProcessor.Process(in *ptr2->Context, new ProcessorInstance.Pipe(ptr2->Self, null), ptr2->InOut);
				break;
			}
			case ProcessorFunction.OutputProcessEnd:
			{
				ProcessPhaseUpdateArguments* ptr = (ProcessPhaseUpdateArguments*)(void*)additionalPtr;
				Span<float> buffer = new Span<float>(ptr->AudioBuffer, ptr->OutputChannelCount * ptr->OutputFrameCount);
				ChannelBuffer output = new ChannelBuffer(buffer, ptr->OutputChannelCount);
				storage.UserProcessor.EndProcessing(in *ptr->Context, new ProcessorInstance.Pipe(ptr->Self, null), output);
				break;
			}
			case ProcessorFunction.OutputRemoved:
				storage.UserProcessor.RemovedFromProcessing();
				break;
			default:
				ProcessorExtensions.DispatchGenericProcessor(ref storage.UserProcessor, in storage.Header, (void*)additionalPtr, processorFunction2);
				break;
			}
		}
	}

	internal static IntPtr GetReflectionData<T>() where T : unmanaged, RootOutputInstance.IRealtime
	{
		JobStruct<T>.Initialize();
		return JobStruct<T>.jobReflectionData.Data;
	}

	internal unsafe static void InitializeRootOutputHandle(ProcessorHeader* header, ControlHeader* control, ProcessorInstance.InitializationFlags flags)
	{
		InternalInitializeRootOutputHandle(header, control, flags);
	}

	[MethodImpl(MethodImplOptions.InternalCall)]
	[NativeMethod(Name = "audio::InitializeRootOutputHandle", IsFreeFunction = true, ThrowsException = true)]
	private unsafe static extern void InternalInitializeRootOutputHandle(void* header, void* control, ProcessorInstance.InitializationFlags flags);
}
