using System;
using System.Reflection;
using Unity.Collections;

namespace UnityEngine.Rendering;

[AttributeUsage(AttributeTargets.Class, AllowMultiple = false)]
public class SupportedOnRenderPipelineAttribute : Attribute
{
	public enum SupportedMode
	{
		Unsupported,
		Supported,
		SupportedByBaseClass
	}

	private static readonly Lazy<Type[]> k_DefaultRenderPipelineAsset = new Lazy<Type[]>(() => new Type[1] { typeof(RenderPipelineAsset) });

	public Type[] renderPipelineTypes { get; }

	public bool isSupportedOnCurrentPipeline => GetSupportedMode(renderPipelineTypes, GraphicsSettings.currentRenderPipelineAssetType) != SupportedMode.Unsupported;

	public SupportedOnRenderPipelineAttribute(Type renderPipeline)
		: this(new Type[1] { renderPipeline })
	{
	}

	public SupportedOnRenderPipelineAttribute(params Type[] renderPipeline)
	{
		if (renderPipeline == null)
		{
			Debug.LogError("The SupportedOnRenderPipelineAttribute parameters cannot be null.");
			return;
		}
		foreach (Type type in renderPipeline)
		{
			if (!(type != null) || !typeof(RenderPipelineAsset).IsAssignableFrom(type))
			{
				Debug.LogError("The SupportedOnRenderPipelineAttribute Attribute targets an invalid RenderPipelineAsset. One of the types cannot be assigned from RenderPipelineAsset: [" + renderPipeline.SerializedView((Type t) => t.Name) + "].");
				return;
			}
		}
		renderPipelineTypes = ((renderPipeline.Length == 0) ? k_DefaultRenderPipelineAsset.Value : renderPipeline);
	}

	public SupportedMode GetSupportedMode(Type renderPipelineAssetType)
	{
		return GetSupportedMode(renderPipelineTypes, renderPipelineAssetType);
	}

	internal static SupportedMode GetSupportedMode(Type[] renderPipelineTypes, Type renderPipelineAssetType)
	{
		if (renderPipelineTypes == null)
		{
			throw new ArgumentNullException("Parameter renderPipelineTypes cannot be null.");
		}
		if (renderPipelineAssetType == null)
		{
			return SupportedMode.Unsupported;
		}
		for (int i = 0; i < renderPipelineTypes.Length; i++)
		{
			if (renderPipelineTypes[i] == renderPipelineAssetType)
			{
				return SupportedMode.Supported;
			}
		}
		for (int j = 0; j < renderPipelineTypes.Length; j++)
		{
			if (renderPipelineTypes[j].IsAssignableFrom(renderPipelineAssetType))
			{
				return SupportedMode.SupportedByBaseClass;
			}
		}
		return SupportedMode.Unsupported;
	}

	public static bool IsTypeSupportedOnRenderPipeline(Type type, Type renderPipelineAssetType)
	{
		SupportedOnRenderPipelineAttribute customAttribute = type.GetCustomAttribute<SupportedOnRenderPipelineAttribute>();
		return customAttribute == null || customAttribute.GetSupportedMode(renderPipelineAssetType) != SupportedMode.Unsupported;
	}
}
