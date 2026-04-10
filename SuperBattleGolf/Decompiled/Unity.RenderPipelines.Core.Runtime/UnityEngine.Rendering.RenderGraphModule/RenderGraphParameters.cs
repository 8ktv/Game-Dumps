using System;
using UnityEngine.Scripting.APIUpdating;

namespace UnityEngine.Rendering.RenderGraphModule;

[MovedFrom(true, "UnityEngine.Experimental.Rendering.RenderGraphModule", "UnityEngine.Rendering.RenderGraphModule", null)]
public struct RenderGraphParameters
{
	[Obsolete("Not used anymore. The debugging tools use the name of the object identified by executionId. #from(6000.3)")]
	public string executionName;

	public EntityId executionId;

	public bool generateDebugData;

	public int currentFrameIndex;

	public bool rendererListCulling;

	public ScriptableRenderContext scriptableRenderContext;

	public CommandBuffer commandBuffer;

	internal bool invalidContextForTesting;

	public RenderTextureUVOriginStrategy renderTextureUVOriginStrategy;
}
