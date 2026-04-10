using Unity.Audio;

namespace UnityEngine.Audio;

public struct RealtimeContext : ProcessorInstance.IContext
{
	internal RealtimeAccess Access;

	internal ulong m_DSPClock;

	public readonly ulong dspTime => m_DSPClock;

	public readonly bool isCreated => Access.IsCreated;

	unsafe ProcessorInstance.AvailableData ProcessorInstance.IContext.GetAvailableData(Handle handle)
	{
		return new ProcessorInstance.AvailableData(ScriptableProcessorBindings.GetAvailableDataForRealtime(in Access, in handle));
	}

	unsafe bool ProcessorInstance.IContext.SendData(Handle handle, void* data, int size, int align, long typehash)
	{
		ScriptableProcessorBindings.ReturnDataFromProcessor(in Access, in handle, data, size, align, typehash);
		return true;
	}

	public unsafe readonly GeneratorInstance.Result Process(GeneratorInstance generatorInstance, ChannelBuffer buffer, GeneratorInstance.Arguments args)
	{
		ScriptableProcessorBindings.ValidateCanProcess(in generatorInstance.m_ProcessorInstance.Handle, in this);
		fixed (float* audioBuffer = buffer.Buffer)
		{
			fixed (RealtimeContext* context = &this)
			{
				IGeneratorProcessorExtensions.ProcessArguments processArguments = new IGeneratorProcessorExtensions.ProcessArguments
				{
					AudioBuffer = audioBuffer,
					Context = context,
					FrameCount = buffer.frameCount
				};
				generatorInstance.m_ProcessorInstance.Header->InvokeProcessor(ProcessorFunction.Process, &processArguments);
				return processArguments.Result;
			}
		}
	}
}
