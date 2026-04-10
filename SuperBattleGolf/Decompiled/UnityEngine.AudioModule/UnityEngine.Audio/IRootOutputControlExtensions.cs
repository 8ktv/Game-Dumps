using System;
using System.ComponentModel;
using System.Runtime.InteropServices;
using Unity.Burst;
using Unity.Collections.LowLevel.Unsafe;
using Unity.Jobs.LowLevel.Unsafe;

namespace UnityEngine.Audio;

[EditorBrowsable(EditorBrowsableState.Never)]
internal static class IRootOutputControlExtensions
{
	[StructLayout(LayoutKind.Sequential, Size = 1)]
	internal struct JobStruct<TUserControl, TUserProcessor> where TUserControl : unmanaged, RootOutputInstance.IControl<TUserProcessor> where TUserProcessor : unmanaged, RootOutputInstance.IRealtime
	{
		internal struct ControlStorage
		{
			public IRootOutputProcessorExtensions.JobStruct<TUserProcessor>.Storage HeaderAndProcessor;

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
				storage.UserControl.Configure(new ControlContext(ptr->ControlContext), ref storage.HeaderAndProcessor.UserProcessor, new AudioFormat(ptr->Now)).Complete();
			}
			else
			{
				ProcessorExtensions.DispatchGenericControl(ref storage.UserControl, ref storage.HeaderAndProcessor.UserProcessor, in storage.HeaderAndProcessor.Header, (void*)additionalPtr, controlFunction);
			}
		}
	}

	internal static IntPtr GetReflectionData<TUserControl, TUserProcessor>() where TUserControl : unmanaged, RootOutputInstance.IControl<TUserProcessor> where TUserProcessor : unmanaged, RootOutputInstance.IRealtime
	{
		JobStruct<TUserControl, TUserProcessor>.Initialize();
		return JobStruct<TUserControl, TUserProcessor>.jobReflectionData.Data;
	}
}
