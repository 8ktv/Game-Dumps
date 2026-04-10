using System;
using System.ComponentModel;
using System.Runtime.InteropServices;
using Unity.Burst;
using Unity.Collections.LowLevel.Unsafe;
using Unity.Jobs.LowLevel.Unsafe;

namespace UnityEngine.Audio;

[EditorBrowsable(EditorBrowsableState.Never)]
internal static class IGeneratorControlExtensions
{
	[StructLayout(LayoutKind.Sequential, Size = 1)]
	internal struct JobStruct<TUserControl, TUserProcessor> where TUserControl : unmanaged, GeneratorInstance.IControl<TUserProcessor> where TUserProcessor : unmanaged, GeneratorInstance.IRealtime
	{
		internal struct ControlStorage
		{
			public IGeneratorProcessorExtensions.JobStruct<TUserProcessor>.Storage HeaderAndProcessor;

			public TUserControl UserControl;
		}

		internal delegate void ExecuteJobFunction(ref ControlStorage storage, IntPtr additionalPtr, IntPtr additionalPtr2, ref JobRanges ranges, int jobIndex);

		internal static readonly BurstLike.SharedStatic<IntPtr> jobReflectionData = BurstLike.SharedStatic<IntPtr>.GetOrCreate<JobStruct<TUserControl, TUserProcessor>>();

		[BurstDiscard]
		internal static void Initialize()
		{
			if (jobReflectionData.Data == IntPtr.Zero)
			{
				jobReflectionData.Data = JobsUtility.CreateJobReflectionData(typeof(ControlStorage), typeof(TUserControl), new ExecuteJobFunction(Execute));
			}
		}

		public unsafe static void Execute(ref ControlStorage storage, IntPtr additionalPtr, IntPtr additionalPtr2, ref JobRanges ranges, int jobIndex)
		{
			ControlFunction controlFunction = (ControlFunction)(int)additionalPtr2;
			ControlFunction controlFunction2 = controlFunction;
			ControlFunction controlFunction3 = controlFunction2;
			if (controlFunction3 == ControlFunction.Configure)
			{
				ConfigureArguments* ptr = (ConfigureArguments*)(void*)additionalPtr;
				storage.UserControl.Configure(new ControlContext(ptr->ControlContext), ref storage.HeaderAndProcessor.UserProcessor, new AudioFormat(ptr->Now), out storage.HeaderAndProcessor.Header.Configuration.Setup, ref storage.HeaderAndProcessor.Header.Configuration.Properties);
				if (storage.HeaderAndProcessor.Header.Configuration.IsRealtime && storage.HeaderAndProcessor.Header.Configuration.Setup.sampleRate != ptr->Now.sampleRate)
				{
					Debug.LogError("Realtime generators must obey system sampling rate");
				}
			}
			else
			{
				ProcessorExtensions.DispatchGenericControl(ref storage.UserControl, ref storage.HeaderAndProcessor.UserProcessor, in storage.HeaderAndProcessor.Header.Processor, (void*)additionalPtr, controlFunction);
			}
		}
	}

	internal static IntPtr GetReflectionData<TUserControl, TUserGenerator>() where TUserControl : unmanaged, GeneratorInstance.IControl<TUserGenerator> where TUserGenerator : unmanaged, GeneratorInstance.IRealtime
	{
		JobStruct<TUserControl, TUserGenerator>.Initialize();
		return JobStruct<TUserControl, TUserGenerator>.jobReflectionData.Data;
	}
}
