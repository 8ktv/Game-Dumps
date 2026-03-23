using System;

namespace UnityEngine.Rendering;

public abstract class RenderPipelineAsset : ScriptableObject
{
	public virtual Material defaultMaterial => null;

	public virtual Shader autodeskInteractiveShader => null;

	public virtual Shader autodeskInteractiveTransparentShader => null;

	public virtual Shader autodeskInteractiveMaskedShader => null;

	public virtual Shader terrainDetailLitShader => null;

	public virtual Shader terrainDetailGrassShader => null;

	public virtual Shader terrainDetailGrassBillboardShader => null;

	public virtual Material defaultParticleMaterial => null;

	public virtual Material defaultLineMaterial => null;

	public virtual Material defaultTerrainMaterial => null;

	public virtual Material defaultUIMaterial => null;

	public virtual Material defaultUIOverdrawMaterial => null;

	public virtual Material defaultUIETC1SupportedMaterial => null;

	public virtual Material default2DMaterial => null;

	public virtual Material default2DMaskMaterial => null;

	public virtual Shader defaultShader => null;

	public virtual Shader defaultSpeedTree7Shader => null;

	public virtual Shader defaultSpeedTree8Shader => null;

	public virtual Shader defaultSpeedTree9Shader => null;

	public virtual string renderPipelineShaderTag
	{
		get
		{
			Debug.LogWarning("The property renderPipelineShaderTag has not been overridden. At build time, any shader variants that use any RenderPipeline tag will be stripped.");
			return string.Empty;
		}
	}

	public virtual Type pipelineType
	{
		get
		{
			Debug.LogWarning("You must either inherit from RenderPipelineAsset<TRenderPipeline> or override pipelineType property.");
			return null;
		}
	}

	internal string pipelineTypeFullName => pipelineType?.FullName ?? string.Empty;

	[Obsolete("This property is obsolete. Use pipelineType instead. #from(23.2)", false)]
	protected internal virtual Type renderPipelineType
	{
		get
		{
			Debug.LogWarning("You must either inherit from RenderPipelineAsset<TRenderPipeline> or override renderPipelineType property");
			return null;
		}
	}

	[Obsolete("This property is obsolete. Use RenderingLayerMask API and Tags & Layers project settings instead. #from(23.3)", false)]
	public virtual string[] renderingLayerMaskNames => null;

	[Obsolete("This property is obsolete. Use RenderingLayerMask API and Tags & Layers project settings instead. #from(23.3)", false)]
	public virtual string[] prefixedRenderingLayerMaskNames => null;

	internal RenderPipeline InternalCreatePipeline()
	{
		RenderPipeline result = null;
		try
		{
			result = CreatePipeline();
		}
		catch (InvalidImportException)
		{
		}
		catch (Exception exception)
		{
			Debug.LogException(exception);
		}
		return result;
	}

	protected abstract RenderPipeline CreatePipeline();

	protected virtual void EnsureGlobalSettings()
	{
	}

	protected virtual void OnValidate()
	{
		RenderPipelineManager.RecreateCurrentPipeline(this);
	}

	protected virtual void OnDisable()
	{
		RenderPipelineManager.CleanupRenderPipeline();
	}
}
public abstract class RenderPipelineAsset<TRenderPipeline> : RenderPipelineAsset where TRenderPipeline : RenderPipeline
{
	public sealed override Type pipelineType => typeof(TRenderPipeline);

	public override string renderPipelineShaderTag => typeof(TRenderPipeline).Name;

	[Obsolete("This property is obsolete. Use pipelineType instead. #from(23.2)", false)]
	protected internal sealed override Type renderPipelineType => typeof(TRenderPipeline);
}
