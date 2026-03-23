using System;
using UnityEngine;
using UnityEngine.Rendering.PostProcessing;

[Serializable]
[PostProcess(typeof(ImpactFrameRenderer), PostProcessEvent.BeforeStack, "Custom/Impact Frame", true)]
public sealed class ImpactFrame : PostProcessEffectSettings
{
	[Range(0f, 1f)]
	[Tooltip("Impact Frame effect intensity")]
	public FloatParameter blend = new FloatParameter
	{
		value = 1f
	};

	public BoolParameter invert = new BoolParameter
	{
		value = false
	};

	public FloatParameter steps = new FloatParameter
	{
		value = 3f
	};

	public BoolParameter useLut = new BoolParameter
	{
		value = false
	};

	public TextureParameter lut = new TextureParameter
	{
		value = null
	};

	public ColorParameter colorA = new ColorParameter
	{
		value = Color.black
	};

	public ColorParameter colorB = new ColorParameter
	{
		value = Color.white
	};

	public Vector2Parameter smoothstepFactors = new Vector2Parameter
	{
		value = new Vector2(0.25f, 0.25f)
	};

	[Range(0f, 1f)]
	[Tooltip("Distortion intensity")]
	public FloatParameter distortionStrength = new FloatParameter
	{
		value = 0.1f
	};

	public FloatParameter distortionNoiseScale = new FloatParameter
	{
		value = 10f
	};

	public Vector2Parameter distortionSmoothstepFactors = new Vector2Parameter
	{
		value = new Vector2(0.25f, 0.25f)
	};

	public Vector2Parameter distortionCenter = new Vector2Parameter
	{
		value = new Vector2(0.5f, 0.5f)
	};

	public FloatParameter distortionDivisions = new FloatParameter
	{
		value = 15f
	};

	public override bool IsEnabledAndSupported(PostProcessRenderContext context)
	{
		if (enabled.value)
		{
			return blend.value > 0f;
		}
		return false;
	}
}
