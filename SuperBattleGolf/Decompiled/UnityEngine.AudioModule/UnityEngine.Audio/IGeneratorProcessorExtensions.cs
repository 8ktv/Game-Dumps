using System;
using System.ComponentModel;
using System.Runtime.InteropServices;
using Unity.Audio;
using Unity.Burst;
using Unity.Collections.LowLevel.Unsafe;
using Unity.Jobs.LowLevel.Unsafe;

namespace UnityEngine.Audio;

[EditorBrowsable(EditorBrowsableState.Never)]
internal static class IGeneratorProcessorExtensions
{
	internal ref struct ProcessArguments
	{
		internal unsafe RealtimeContext* Context;

		internal unsafe float* AudioBuffer;

		internal Handle Self;

		internal int FrameCount;

		internal GeneratorInstance.Arguments GeneratorArguments;

		internal GeneratorInstance.Result Result;
	}

	[StructLayout(LayoutKind.Sequential, Size = 1)]
	internal struct JobStruct<TUserProcessor> where TUserProcessor : unmanaged, GeneratorInstance.IRealtime
	{
		internal struct Storage
		{
			public GeneratorInstance.GeneratorHeader Header;

			public TUserProcessor UserProcessor;
		}

		internal delegate void ExecuteJobFunction(ref Storage storage, IntPtr additionalPtr, IntPtr additionalPtr2, ref JobRanges ranges, int jobIndex);

		internal static readonly BurstLike.SharedStatic<IntPtr> jobReflectionData = BurstLike.SharedStatic<IntPtr>.GetOrCreate<JobStruct<TUserProcessor>>();

		[BurstDiscard]
		internal static void Initialize()
		{
			if (jobReflectionData.Data == IntPtr.Zero)
			{
				jobReflectionData.Data = JobsUtility.CreateJobReflectionData(typeof(Storage), typeof(TUserProcessor), new ExecuteJobFunction(Execute));
			}
		}

		public unsafe static void Execute(ref Storage storage, IntPtr additionalPtr, IntPtr additionalPtr2, ref JobRanges ranges, int jobIndex)
		{
			ProcessorFunction processorFunction = (ProcessorFunction)(int)additionalPtr2;
			ProcessorFunction processorFunction2 = processorFunction;
			ProcessorFunction processorFunction3 = processorFunction2;
			if (processorFunction3 == ProcessorFunction.Process)
			{
				ProcessArguments* ptr = (ProcessArguments*)(void*)additionalPtr;
				Span<float> buffer = new Span<float>(ptr->AudioBuffer, storage.Header.Configuration.Setup.speakerMode.ChannelCount() * ptr->FrameCount);
				ChannelBuffer buffer2 = new ChannelBuffer(buffer, storage.Header.Configuration.Setup.speakerMode.ChannelCount());
				ptr->Result = storage.UserProcessor.Process(in *ptr->Context, new ProcessorInstance.Pipe(ptr->Self, null), buffer2, ptr->GeneratorArguments);
			}
			else
			{
				ProcessorExtensions.DispatchGenericProcessor(ref storage.UserProcessor, in storage.Header.Processor, (void*)additionalPtr, processorFunction);
			}
		}
	}

	internal static IntPtr GetReflectionData<TUserProcessor>() where TUserProcessor : unmanaged, GeneratorInstance.IRealtime
	{
		JobStruct<TUserProcessor>.Initialize();
		return JobStruct<TUserProcessor>.jobReflectionData.Data;
	}
}
