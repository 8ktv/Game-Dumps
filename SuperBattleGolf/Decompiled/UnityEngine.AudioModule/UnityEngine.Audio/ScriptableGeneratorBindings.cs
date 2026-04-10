using System.Runtime.CompilerServices;
using UnityEngine.Bindings;
using UnityEngine.Scripting;

namespace UnityEngine.Audio;

[NativeHeader("Modules/Audio/Public/ScriptableProcessors/ScriptBindings/ScriptableProcessor.bindings.h")]
internal static class ScriptableGeneratorBindings
{
	[RequiredByNativeCode(GenerateProxy = true)]
	internal unsafe static void InstantiateGeneratorFromObject(Object generatorObjectDefinition, ref ControlHeader control, out GeneratorInstance runtimeHandle)
	{
		if (generatorObjectDefinition is IAudioGenerator audioGenerator)
		{
			fixed (ControlHeader* headerThatShouldBeOfResourceType = &control)
			{
				ControlContext context = new ControlContext(headerThatShouldBeOfResourceType);
				runtimeHandle = audioGenerator.CreateInstance(context, null, default(ProcessorInstance.CreationParameters));
				if (context.Exists(runtimeHandle))
				{
					GeneratorInstance.Configuration configuration = context.GetConfiguration(runtimeHandle);
					if (audioGenerator.isFinite != configuration.IsFinite)
					{
						Debug.LogError($"Generator {generatorObjectDefinition} has inconsistent isFinite declaration: {audioGenerator.isFinite} vs {configuration.IsFinite}");
					}
					if (audioGenerator.isRealtime != configuration.isRealtime)
					{
						Debug.LogError($"Generator {generatorObjectDefinition} has inconsistent isRealtime declaration: {audioGenerator.isRealtime} vs {configuration.isRealtime}");
					}
					if (audioGenerator.length != configuration.length)
					{
						Debug.LogError($"Generator {generatorObjectDefinition} has inconsistent length declaration: {audioGenerator.length} vs {configuration.length}");
					}
				}
			}
		}
		else
		{
			runtimeHandle = default(GeneratorInstance);
			Debug.LogError(string.Format("Trying to play object {0}, but it doesn't implement {1}", generatorObjectDefinition, "IAudioGenerator"));
		}
	}

	internal unsafe static void InitializeGeneratorHandle(GeneratorInstance.GeneratorHeader* header, ControlHeader* control, AudioConfiguration* nestedConfiguration, ProcessorInstance.InitializationFlags flags)
	{
		InternalInitializeGeneratorHandle(header, control, nestedConfiguration, flags);
	}

	[MethodImpl(MethodImplOptions.InternalCall)]
	[NativeMethod(Name = "audio::InitializeGeneratorHandle", IsFreeFunction = true, ThrowsException = true)]
	private unsafe static extern void InternalInitializeGeneratorHandle(void* header, void* control, AudioConfiguration* nestedConfiguration, ProcessorInstance.InitializationFlags flags);
}
