using UnityEngine;
using UnityEngine.Rendering.PostProcessing;

public sealed class CrtScreenRenderer : PostProcessEffectRenderer<CrtScreen>
{
	public override void Render(PostProcessRenderContext context)
	{
		PropertySheet propertySheet = context.propertySheets.Get(Shader.Find("Hidden/VFX/Post Processing/CRT Screen"));
		propertySheet.properties.SetFloat("_BulgeEnabled", base.settings.bulgeEnabled ? 1f : 0f);
		propertySheet.properties.SetFloat("_BulgeIntensity", base.settings.bulgeIntensity);
		propertySheet.properties.SetColor("_VignetteColor", base.settings.vignetteColor);
		propertySheet.properties.SetVector("_BorderSmoothstepFactors", base.settings.borderSmoothstepFactors);
		propertySheet.properties.SetFloat("_AberrationEnabled", base.settings.aberrationEnabled ? 1f : 0f);
		propertySheet.properties.SetFloat("_AberrationIntensity", base.settings.aberrationIntensity);
		propertySheet.properties.SetFloat("_ScanlineEnabled", base.settings.scanlineEnabled ? 1f : 0f);
		propertySheet.properties.SetFloat("_ScanlineOpacity", base.settings.scanlineOpacity);
		propertySheet.properties.SetFloat("_ScanlineWidth", base.settings.scanlineWidth);
		propertySheet.properties.SetFloat("_RollingLinesEnabled", base.settings.rollingLinesEnabled ? 1f : 0f);
		propertySheet.properties.SetFloat("_NoiseOpacity", base.settings.noiseOpacity);
		propertySheet.properties.SetFloat("_NoiseSpeed", base.settings.noiseSpeed);
		propertySheet.properties.SetFloat("_NoiseScale", base.settings.noiseScale);
		propertySheet.properties.SetVector("_NoiseSmoothstepFactors", base.settings.noiseSmoothstepFactors);
		propertySheet.properties.SetColor("_NoiseColor", base.settings.noiseColor);
		context.command.BlitFullscreenTriangle(context.source, context.destination, propertySheet, 0);
	}
}
