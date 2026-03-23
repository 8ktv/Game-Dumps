using System;
using UnityEngine;
using UnityEngine.Rendering.PostProcessing;

[Serializable]
[PostProcess(typeof(PostProcessingFogRenderer), PostProcessEvent.BeforeStack, "Custom/Fog", true)]
public sealed class PostProcessingFog : PostProcessEffectSettings
{
	[Range(0f, 1f)]
	[Tooltip("Impact Frame effect intensity")]
	public ColorParameter color = new ColorParameter
	{
		value = Color.white
	};

	public FloatParameter startOffset = new FloatParameter
	{
		value = 0f
	};

	public FloatParameter density = new FloatParameter
	{
		value = 0.001f
	};
}
