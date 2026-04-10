using System;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using Unity.Audio;
using Unity.Collections.LowLevel.Unsafe;
using Unity.IntegerTime;
using UnityEngine.Bindings;
using UnityEngine.Scripting;

namespace UnityEngine.Audio;

[RequiredByNativeCode]
[NativeHeader("Modules/Audio/Public/ScriptableProcessors/ScriptBindings/ScriptableProcessor.bindings.h")]
public struct ControlContext : ProcessorInstance.IContext
{
	public struct Manual : IDisposable
	{
		private ControlContext m_Context;

		public ControlContext context => m_Context;

		public unsafe RealtimeContext? BeginMix(ulong dspTick)
		{
			RealtimeContext value = default(RealtimeContext);
			if (InternalBeginManualMixFromControlContext(m_Context.m_Header, dspTick, &value))
			{
				return value;
			}
			return null;
		}

		public unsafe void EndMix(ChannelBuffer result)
		{
			InternalEndMixManualControlContext(m_Context.m_Header, result.Buffer);
		}

		public unsafe void Update()
		{
			InternalUpdateManualControlContext(m_Context.m_Header);
		}

		public unsafe void Dispose()
		{
			m_Context.m_Handle.CheckValidOrThrow();
			InternalDestroyControlContext(m_Context.m_Header);
		}

		internal Manual(in ControlContext context)
		{
			m_Context = context;
		}
	}

	[StructLayout(LayoutKind.Sequential, Size = 1)]
	[Obsolete("ControlContext.ProcessorUpdateSetting has been deprecated. Use ProcessorInstance.UpdateSetting instead. (UnityUpgradable) -> ProcessorInstance/UpdateSetting", true)]
	public struct ProcessorUpdateSetting
	{
	}

	[StructLayout(LayoutKind.Sequential, Size = 1)]
	[Obsolete("ControlContext.ProcessorCreationParameters has been deprecated. Use ProcessorInstance.CreationParameters instead. (UnityUpgradable) -> ProcessorInstance/CreationParameters", true)]
	public struct ProcessorCreationParameters
	{
	}

	private unsafe ControlHeader* m_Header;

	internal Handle m_Handle;

	internal unsafe readonly ControlHeader* Header => m_Header;

	public unsafe static ControlContext builtIn => new ControlContext(InternalGetBuiltInControlHeader());

	internal unsafe ControlContext(void* headerThatShouldBeOfResourceType)
	{
		m_Handle = ((ControlHeader*)headerThatShouldBeOfResourceType)->Handle;
		m_Handle.CheckValidOrThrow();
		m_Header = (ControlHeader*)headerThatShouldBeOfResourceType;
	}

	public unsafe readonly GeneratorInstance AllocateGenerator<TRealtime, TControl>(in TRealtime realtimeState, in TControl controlState, AudioFormat? nestedFormat = null, in ProcessorInstance.CreationParameters creationParameters = default(ProcessorInstance.CreationParameters)) where TRealtime : unmanaged, GeneratorInstance.IRealtime where TControl : unmanaged, GeneratorInstance.IControl<TRealtime>
	{
		m_Handle.CheckValidOrThrow();
		IGeneratorControlExtensions.JobStruct<TControl, TRealtime>.ControlStorage* ptr = ProcessorExtensions.CAllocChunk<IGeneratorControlExtensions.JobStruct<TControl, TRealtime>.ControlStorage>();
		GeneratorInstance.GeneratorHeader* ptr2 = &ptr->HeaderAndProcessor.Header;
		ptr2->Processor.ProcessorReflectionData = IGeneratorProcessorExtensions.GetReflectionData<TRealtime>();
		ptr2->Processor.ControlReflectionData = IGeneratorControlExtensions.GetReflectionData<TControl, TRealtime>();
		ptr2->Configuration.IsRealtime = realtimeState.isRealtime;
		ptr2->Configuration.IsFinite = realtimeState.isFinite;
		DiscreteTime? length = realtimeState.length;
		if (length.HasValue)
		{
			DiscreteTime valueOrDefault = length.GetValueOrDefault();
			if (true)
			{
				ptr2->Configuration.ReportedLength = valueOrDefault;
				ptr2->Configuration.HasKnownLength = true;
			}
		}
		ptr->HeaderAndProcessor.UserProcessor = realtimeState;
		ptr->UserControl = controlState;
		AudioConfiguration audioConfiguration = nestedFormat.GetValueOrDefault().audioConfiguration;
		ScriptableGeneratorBindings.InitializeGeneratorHandle(ptr2, m_Header, nestedFormat.HasValue ? (&audioConfiguration) : null, creationParameters.BuildInitializationFlags());
		return new GeneratorInstance(ptr2);
	}

	public unsafe readonly RootOutputInstance AllocateRootOutput<TRealtime, TControl>(in TRealtime realtimeState, in TControl controlState, in ProcessorInstance.CreationParameters creationParameters = default(ProcessorInstance.CreationParameters)) where TRealtime : unmanaged, RootOutputInstance.IRealtime where TControl : unmanaged, RootOutputInstance.IControl<TRealtime>
	{
		m_Handle.CheckValidOrThrow();
		IRootOutputControlExtensions.JobStruct<TControl, TRealtime>.ControlStorage* ptr = ProcessorExtensions.CAllocChunk<IRootOutputControlExtensions.JobStruct<TControl, TRealtime>.ControlStorage>();
		ProcessorHeader* ptr2 = &ptr->HeaderAndProcessor.Header;
		ptr2->ProcessorReflectionData = IRootOutputProcessorExtensions.GetReflectionData<TRealtime>();
		ptr2->ControlReflectionData = IRootOutputControlExtensions.GetReflectionData<TControl, TRealtime>();
		ptr->HeaderAndProcessor.UserProcessor = realtimeState;
		ptr->UserControl = controlState;
		IRootOutputProcessorExtensions.InitializeRootOutputHandle(ptr2, m_Header, creationParameters.BuildInitializationFlags());
		return new RootOutputInstance(ptr2);
	}

	public unsafe readonly bool IsGenerator<TRealtime, TControl>(ProcessorInstance processorInstance) where TRealtime : unmanaged, GeneratorInstance.IRealtime where TControl : unmanaged, GeneratorInstance.IControl<TRealtime>
	{
		m_Handle.CheckValidOrThrow();
		processorInstance.Handle.CheckValidOrThrow();
		return processorInstance.Header->ControlReflectionData == IGeneratorControlExtensions.GetReflectionData<TControl, TRealtime>();
	}

	public unsafe readonly bool IsRootOutput<TRealtime, TControl>(ProcessorInstance processorInstance) where TRealtime : unmanaged, RootOutputInstance.IRealtime where TControl : unmanaged, RootOutputInstance.IControl<TRealtime>
	{
		m_Handle.CheckValidOrThrow();
		processorInstance.Handle.CheckValidOrThrow();
		return processorInstance.Header->ControlReflectionData == IRootOutputControlExtensions.GetReflectionData<TControl, TRealtime>();
	}

	public unsafe bool Exists(ProcessorInstance processorInstance)
	{
		m_Handle.CheckValidOrThrow();
		return ScriptableProcessorBindings.CheckProcessorExists(processorInstance.Handle, m_Header);
	}

	public unsafe ProcessorInstance.Response SendMessage<T>(ProcessorInstance processorInstance, ref T message) where T : unmanaged
	{
		m_Handle.CheckValidOrThrow();
		processorInstance.Handle.CheckValidOrThrow();
		fixed (T* data = &message)
		{
			ProcessorInstance.Message message2 = new ProcessorInstance.Message
			{
				TypeHash = BurstRuntime.GetHashCode64<T>(),
				Data = data,
				ManagedHandle = default(IntPtr)
			};
			return ScriptableProcessorBindings.SendMessageToProcessor(processorInstance.Header, Header, &message2);
		}
	}

	internal unsafe ProcessorInstance.Response SendManagedMessage<T>(ProcessorInstance processorInstance, T message) where T : class
	{
		m_Handle.CheckValidOrThrow();
		processorInstance.Handle.CheckValidOrThrow();
		object target = null;
		GCHandle value;
		if (Header->ManagedTransport != IntPtr.Zero)
		{
			value = GCHandle.FromIntPtr(Header->ManagedTransport);
			target = value.Target;
			value.Target = message;
		}
		else
		{
			value = GCHandle.Alloc(message, GCHandleType.Normal);
			Header->ManagedTransport = GCHandle.ToIntPtr(value);
		}
		ProcessorInstance.Message message2 = new ProcessorInstance.Message
		{
			TypeHash = BurstRuntime.GetHashCode64<T>(),
			Data = null,
			ManagedHandle = GCHandle.ToIntPtr(value)
		};
		ProcessorInstance.Response result = ScriptableProcessorBindings.SendMessageToProcessor(processorInstance.Header, Header, &message2);
		value.Target = target;
		return result;
	}

	public void Destroy(GeneratorInstance generatorInstance)
	{
		DestroyProcessor(generatorInstance.m_ProcessorInstance);
	}

	public void Destroy(RootOutputInstance rootOutputInstance)
	{
		DestroyProcessor(rootOutputInstance.m_ProcessorInstance);
	}

	public unsafe GeneratorInstance.Configuration GetConfiguration(GeneratorInstance generatorInstance)
	{
		m_Handle.CheckValidOrThrow();
		generatorInstance.m_ProcessorInstance.Handle.CheckValidOrThrow();
		return ((GeneratorInstance.GeneratorHeader*)generatorInstance.m_ProcessorInstance.Header)->Configuration;
	}

	public unsafe void Configure(GeneratorInstance generatorInstance, in AudioFormat format)
	{
		ScriptableProcessorBindings.PerformRecursiveConfigure(generatorInstance.m_ProcessorInstance.Handle, Header, format.audioConfiguration);
	}

	public unsafe void Update(GeneratorInstance generatorInstance)
	{
		ScriptableProcessorBindings.PerformRecursiveUpdate(generatorInstance.m_ProcessorInstance.Handle, Header);
	}

	public unsafe static void WaitForBuiltInQueueFlush()
	{
		InternalWaitForQueueFlush(builtIn.m_Header);
	}

	public unsafe static Manual CreateManualControlContext(in AudioFormat format)
	{
		ControlContext context = new ControlContext(InternalCreateControlContext());
		context.m_Handle.CheckValidOrThrow();
		InternalSetConfigurationManualControlContext(context.m_Header, format.audioConfiguration);
		return new Manual(in context);
	}

	unsafe ProcessorInstance.AvailableData ProcessorInstance.IContext.GetAvailableData(Handle handle)
	{
		m_Handle.CheckValidOrThrow();
		if (!handle.Valid)
		{
			throw new InvalidOperationException("Invalid handle provided to GetAvailableData");
		}
		ProcessorInstance.AvailableData.Element* availableDataForControl = ScriptableProcessorBindings.GetAvailableDataForControl(m_Header, in handle);
		return new ProcessorInstance.AvailableData(availableDataForControl);
	}

	unsafe bool ProcessorInstance.IContext.SendData(Handle handle, void* data, int size, int align, long typehash)
	{
		return ScriptableProcessorBindings.AddDataToProcessorHandle(m_Header, in handle, data, size, align, typehash);
	}

	internal unsafe void DestroyProcessor(ProcessorInstance processorInstance)
	{
		m_Handle.CheckValidOrThrow();
		if (processorInstance.Handle.Equals(default(Handle)))
		{
			throw new InvalidOperationException("Default / zero-initialized value of processor being destroyed");
		}
		ScriptableProcessorBindings.QueueProcessorDispose(processorInstance.Header, m_Header);
	}

	[RequiredByNativeCode(GenerateProxy = true)]
	internal static void CleanupHeader(ref ControlHeader header)
	{
		if (header.ManagedTransport != IntPtr.Zero)
		{
			GCHandle gCHandle = GCHandle.FromIntPtr(header.ManagedTransport);
			if (gCHandle.IsAllocated)
			{
				gCHandle.Free();
			}
		}
	}

	[MethodImpl(MethodImplOptions.InternalCall)]
	[NativeMethod(Name = "audio::GetBuiltInControlHeader", IsFreeFunction = true)]
	internal unsafe static extern void* InternalGetBuiltInControlHeader();

	[MethodImpl(MethodImplOptions.InternalCall)]
	[NativeMethod(Name = "audio::WaitForQueueFlush", IsFreeFunction = true)]
	private unsafe static extern void InternalWaitForQueueFlush(void* header);

	[MethodImpl(MethodImplOptions.InternalCall)]
	[NativeMethod(Name = "audio::CreateControlContext", IsFreeFunction = true)]
	private unsafe static extern void* InternalCreateControlContext();

	[MethodImpl(MethodImplOptions.InternalCall)]
	[NativeMethod(Name = "audio::DestroyControlContext", IsFreeFunction = true, ThrowsException = true)]
	private unsafe static extern void InternalDestroyControlContext(void* header);

	[MethodImpl(MethodImplOptions.InternalCall)]
	[NativeMethod(Name = "audio::BeginMixManualControlContext ", IsFreeFunction = true, ThrowsException = true)]
	private unsafe static extern bool InternalBeginManualMixFromControlContext(void* header, ulong dspTick, void* resultContext);

	[NativeMethod(Name = "audio::EndMixManualControlContext", IsFreeFunction = true, ThrowsException = true)]
	private unsafe static void InternalEndMixManualControlContext(void* header, Span<float> data)
	{
		Span<float> span = data;
		fixed (float* begin = span)
		{
			ManagedSpanWrapper data2 = new ManagedSpanWrapper(begin, span.Length);
			InternalEndMixManualControlContext_Injected(header, ref data2);
		}
	}

	[MethodImpl(MethodImplOptions.InternalCall)]
	[NativeMethod(Name = "audio::UpdateManualControlContext", IsFreeFunction = true, ThrowsException = true)]
	private unsafe static extern void InternalUpdateManualControlContext(void* header);

	[NativeMethod(Name = "audio::SetConfigurationManualControlContext", IsFreeFunction = true, ThrowsException = true)]
	private unsafe static void InternalSetConfigurationManualControlContext(void* header, AudioConfiguration config)
	{
		InternalSetConfigurationManualControlContext_Injected(header, ref config);
	}

	[Obsolete("ControlContext.GetAvailableData has been deprecated. Use ControlContext.SendMessage instead.", true)]
	public ProcessorInstance.AvailableData GetAvailableData(ProcessorInstance processorInstance)
	{
		throw new NotImplementedException();
	}

	[Obsolete("ControlContext.SendData has been deprecated. Use ControlContext.SendMessage instead.", true)]
	public void SendData<T>(ProcessorInstance processorInstance, in T data) where T : unmanaged
	{
		throw new NotImplementedException();
	}

	[MethodImpl(MethodImplOptions.InternalCall)]
	private unsafe static extern void InternalEndMixManualControlContext_Injected(void* header, ref ManagedSpanWrapper data);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private unsafe static extern void InternalSetConfigurationManualControlContext_Injected(void* header, [In] ref AudioConfiguration config);
}
