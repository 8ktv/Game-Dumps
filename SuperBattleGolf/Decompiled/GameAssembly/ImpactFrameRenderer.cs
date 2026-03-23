using UnityEngine;
using UnityEngine.Rendering.PostProcessing;

public sealed class ImpactFrameRenderer : PostProcessEffectRenderer<ImpactFrame>
{
	public override void Render(PostProcessRenderContext context)
	{
		PropertySheet propertySheet = context.propertySheets.Get(Shader.Find("Hidden/VFX/Post Processing/Impact Frame"));
		propertySheet.properties.SetFloat("_Blend", base.settings.blend);
		propertySheet.properties.SetFloat("_Steps", Mathf.Floor(base.settings.steps));
		propertySheet.properties.SetFloat("_Invert", base.settings.invert ? 1f : 0f);
		propertySheet.properties.SetColor("_ColorA", base.settings.colorA);
		propertySheet.properties.SetColor("_ColorB", base.settings.colorB);
		propertySheet.properties.SetFloat("_UseLut", base.settings.useLut ? 1f : 0f);
		propertySheet.properties.SetTexture("_Lut", base.settings.lut);
		propertySheet.properties.SetVector("_SmoothstepFactors", base.settings.smoothstepFactors);
		propertySheet.properties.SetFloat("_DistortionStrength", base.settings.distortionStrength);
		propertySheet.properties.SetFloat("_DistortionNoiseScale", base.settings.distortionNoiseScale);
		propertySheet.properties.SetVector("_DistortionSmoothstepFactors", base.settings.distortionSmoothstepFactors);
		propertySheet.properties.SetVector("_DistortionCenter", base.settings.distortionCenter);
		propertySheet.properties.SetFloat("_DistortionDivisions", Mathf.Floor(base.settings.distortionDivisions));
		context.command.BlitFullscreenTriangle(context.source, context.destination, propertySheet, 0);
	}
}
