using System;
using Unity.IntegerTime;
using Unity.Jobs.LowLevel.Unsafe;
using UnityEngine.Bindings;
using UnityEngine.Scripting;

namespace UnityEngine.Audio;

public struct GeneratorInstance : IEquatable<GeneratorInstance>
{
	[Obsolete("IProcessor has been deprecated. Use IRealtime instead. (UnityUpgradable) -> GeneratorInstance/IRealtime", true)]
	public interface IProcessor
	{
	}

	public interface ICapabilities
	{
		bool isFinite { get; }

		bool isRealtime { get; }

		DiscreteTime? length { get; }
	}

	public readonly struct Setup
	{
		public readonly AudioSpeakerMode speakerMode;

		public readonly int sampleRate;

		public Setup(AudioSpeakerMode speakerMode, int sampleRate)
		{
			this.speakerMode = speakerMode;
			this.sampleRate = sampleRate;
		}

		public Setup(in AudioFormat fromFormat)
			: this(fromFormat.speakerMode, fromFormat.sampleRate)
		{
		}
	}

	public struct Properties
	{
		private byte m_Reserved;
	}

	public struct Configuration
	{
		internal Setup Setup;

		internal Properties Properties;

		internal DiscreteTime ReportedLength;

		internal bool IsFinite;

		internal bool IsRealtime;

		internal bool HasKnownLength;

		public Setup setup => Setup;

		public Properties properties => Properties;

		public bool isFinite => IsFinite;

		public bool isRealtime => IsRealtime;

		public DiscreteTime? length => HasKnownLength ? new DiscreteTime?(ReportedLength) : ((DiscreteTime?)null);
	}

	public ref struct Result
	{
		internal int m_ProcessedFrames;

		public int processedFrames => m_ProcessedFrames;

		public static implicit operator Result(int processedFrames)
		{
			return new Result
			{
				m_ProcessedFrames = processedFrames
			};
		}
	}

	public ref struct Arguments
	{
		internal float Speed;
	}

	[JobProducerType(typeof(IGeneratorControlExtensions.JobStruct<, >))]
	public interface IControl<TRealtime> : ProcessorInstance.IControl<TRealtime> where TRealtime : unmanaged, ProcessorInstance.IRealtime
	{
		void Configure(ControlContext context, ref TRealtime realtime, in AudioFormat format, out Setup setup, ref Properties properties);
	}

	[JobProducerType(typeof(IGeneratorProcessorExtensions.JobStruct<>))]
	public interface IRealtime : ProcessorInstance.IRealtime, ICapabilities
	{
		Result Process(in RealtimeContext context, ProcessorInstance.Pipe pipe, ChannelBuffer buffer, Arguments args);
	}

	[RequiredByNativeCode]
	[NativeHeader("Modules/Audio/Public/ScriptableProcessors/ScriptBindings/GeneratorHandle.h")]
	internal struct GeneratorHeader
	{
		internal ProcessorHeader Processor;

		internal Configuration Configuration;
	}

	internal readonly ProcessorInstance m_ProcessorInstance;

	[Obsolete("GeneratorInstance.Configure has been deprecated. Use ControlContext.Configure instead.", true)]
	public void Configure(ControlContext context, in AudioFormat format)
	{
		throw new NotImplementedException();
	}

	[Obsolete("GeneratorInstance.Update has been deprecated. Use ControlContext.Update instead.", true)]
	public void Update(ControlContext context)
	{
		throw new NotImplementedException();
	}

	[Obsolete("GeneratorInstance.Process has been deprecated. Use RealtimeContext.Process instead.", true)]
	public Result Process(RealtimeContext context, ChannelBuffer buffer, Arguments args)
	{
		throw new NotImplementedException();
	}

	public static implicit operator ProcessorInstance(in GeneratorInstance generatorInstance)
	{
		return generatorInstance.m_ProcessorInstance;
	}

	public bool Equals(GeneratorInstance other)
	{
		return m_ProcessorInstance.Equals(other.m_ProcessorInstance);
	}

	public override bool Equals(object obj)
	{
		if (obj == null)
		{
			return false;
		}
		return obj is GeneratorInstance other && Equals(other);
	}

	public static bool operator ==(GeneratorInstance a, GeneratorInstance b)
	{
		return a.Equals(b);
	}

	public static bool operator !=(GeneratorInstance a, GeneratorInstance b)
	{
		return !a.Equals(b);
	}

	public override int GetHashCode()
	{
		return m_ProcessorInstance.GetHashCode();
	}

	internal unsafe GeneratorInstance(GeneratorHeader* header)
	{
		m_ProcessorInstance = new ProcessorInstance(header->Processor.DualThreadHandle, &header->Processor);
	}
}
