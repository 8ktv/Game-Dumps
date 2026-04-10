using System;
using UnityEngine.Scripting;

namespace UnityEngine.Audio;

[UsedByNativeCode]
public interface IAudioGenerator : GeneratorInstance.ICapabilities
{
	[Serializable]
	public struct Serializable
	{
		[SerializeField]
		internal Object Reference;

		public IAudioGenerator definition
		{
			get
			{
				return Reference as IAudioGenerator;
			}
			set
			{
				Reference = (Object)value;
			}
		}

		public T Get<T>() where T : Object, IAudioGenerator
		{
			return Reference as T;
		}

		public void Set<T>(T value) where T : Object, IAudioGenerator
		{
			Reference = value;
		}

		public Serializable(IAudioGenerator audioGenerator)
		{
			Reference = (Object)audioGenerator;
		}
	}

	GeneratorInstance CreateInstance(ControlContext context, AudioFormat? nestedFormat, ProcessorInstance.CreationParameters creationParameters);
}
