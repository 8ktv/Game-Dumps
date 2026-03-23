using System;
using UnityEngine;
using UnityEngine.Rendering.PostProcessing;

[Serializable]
[PostProcess(typeof(CrtScreenRenderer), PostProcessEvent.BeforeStack, "Custom/CRT Screen", true)]
public class CrtScreen : PostProcessEffectSettings
{
	[Range(0f, 1f)]
	[Tooltip("Impact Frame effect intensity")]
	public BoolParameter bulgeEnabled = new BoolParameter
	{
		value = true
	};

	public FloatParameter bulgeIntensity = new FloatParameter
	{
		value = 0f
	};

	public ColorParameter vignetteColor = new ColorParameter
	{
		value = Color.black
	};

	public Vector2Parameter borderSmoothstepFactors = new Vector2Parameter
	{
		value = new Vector2(0.85f, 0.15f)
	};

	public BoolParameter aberrationEnabled = new BoolParameter
	{
		value = true
	};

	public FloatParameter aberrationIntensity = new FloatParameter
	{
		value = 0f
	};

	public BoolParameter scanlineEnabled = new BoolParameter
	{
		value = true
	};

	public FloatParameter scanlineOpacity = new FloatParameter
	{
		value = 0f
	};

	public FloatParameter scanlineWidth = new FloatParameter
	{
		value = 0.5f
	};

	public BoolParameter rollingLinesEnabled = new BoolParameter
	{
		value = true
	};

	public FloatParameter noiseOpacity = new FloatParameter
	{
		value = 0f
	};

	public FloatParameter noiseSpeed = new FloatParameter
	{
		value = 1f
	};

	public FloatParameter noiseScale = new FloatParameter
	{
		value = 1f
	};

	public Vector2Parameter noiseSmoothstepFactors = new Vector2Parameter
	{
		value = new Vector2(0.85f, 0.15f)
	};

	public ColorParameter noiseColor = new ColorParameter
	{
		value = Color.white
	};
}
