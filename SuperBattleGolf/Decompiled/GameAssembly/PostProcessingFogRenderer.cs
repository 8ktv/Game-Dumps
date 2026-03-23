using UnityEngine;
using UnityEngine.Rendering.PostProcessing;

public sealed class PostProcessingFogRenderer : PostProcessEffectRenderer<PostProcessingFog>
{
	public override void Render(PostProcessRenderContext context)
	{
		PropertySheet propertySheet = context.propertySheets.Get(Shader.Find("Hidden/VFX/Post Processing/Fog"));
		propertySheet.properties.SetColor("_Color", base.settings.color);
		propertySheet.properties.SetFloat("_StartOffset", base.settings.startOffset);
		propertySheet.properties.SetFloat("_Density", base.settings.density);
		context.command.BlitFullscreenTriangle(context.source, context.destination, propertySheet, 0);
	}
}
