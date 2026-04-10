#define UNITY_ASSERTIONS
using System;
using System.Collections.Generic;
using UnityEngine.UIElements.Layout;

namespace UnityEngine.UIElements.UIR;

internal class RenderTreeCompositor : IDisposable
{
	private enum DrawOperationType
	{
		Undefined,
		RenderTree,
		Effect
	}

	private class DrawOperation
	{
		private DrawOperationType m_Type;

		private VisualElement m_VisualElement;

		private RenderTree m_RenderTree;

		private PostProcessingPass m_FilterPass;

		private int m_FilterPassIndex;

		private FilterFunction m_Filter;

		public RectInt bounds;

		public RectInt drawSourceBounds;

		public Vector4 drawSourceTexOffsets;

		public RenderTreeAtlas.AtlasBlock dstAtlasBlock;

		public TextureId dstTextureId;

		public DrawOperation parent;

		public DrawOperation firstChild;

		public DrawOperation lastChild;

		public DrawOperation prevSibling;

		public DrawOperation nextSibling;

		public DrawOperationType type => m_Type;

		public VisualElement visualElement => m_VisualElement;

		public RenderTree renderTree => m_RenderTree;

		public PostProcessingPass FilterPass => m_FilterPass;

		public int FilterPassIndex => m_FilterPassIndex;

		public FilterFunction filter => m_Filter;

		public void Init(VisualElement ve, in PostProcessingPass filterPass, int filterPassIndex, FilterFunction filter)
		{
			m_Type = DrawOperationType.Effect;
			m_VisualElement = ve;
			m_FilterPass = filterPass;
			m_FilterPassIndex = filterPassIndex;
			m_Filter = filter;
			m_RenderTree = ve.nestedRenderData.renderTree;
			InitPointers();
		}

		public void Init(RenderTree renderTree)
		{
			m_Type = DrawOperationType.RenderTree;
			m_VisualElement = renderTree.rootRenderData.owner;
			m_RenderTree = renderTree;
			InitPointers();
		}

		private void InitPointers()
		{
			parent = null;
			firstChild = null;
			lastChild = null;
			prevSibling = null;
			nextSibling = null;
		}

		public void Reset()
		{
			m_Type = DrawOperationType.Undefined;
			m_VisualElement = null;
			m_RenderTree = null;
			m_FilterPass = default(PostProcessingPass);
			m_Filter = default(FilterFunction);
			dstAtlasBlock = default(RenderTreeAtlas.AtlasBlock);
			dstTextureId = TextureId.invalid;
		}

		public void AddChild(DrawOperation op)
		{
			Debug.Assert(op.prevSibling == null);
			op.parent = this;
			op.nextSibling = firstChild;
			if (firstChild != null)
			{
				firstChild.prevSibling = op;
			}
			firstChild = op;
		}
	}

	private readonly RenderTreeManager m_RenderTreeManager;

	private DrawOperation m_RootOperation;

	private List<RenderTexture> m_AllocatedTextures = new List<RenderTexture>();

	private MaterialPropertyBlock m_Block = new MaterialPropertyBlock();

	private ObjectPool<DrawOperation> m_DrawOperationPool = new ObjectPool<DrawOperation>(() => new DrawOperation());

	private static Vector4[] s_UVRects = new Vector4[1];

	protected bool disposed { get; private set; }

	public RenderTreeCompositor(RenderTreeManager owner)
	{
		m_RenderTreeManager = owner;
	}

	public void Update(RenderTree rootRenderTree)
	{
		CleanupOperationTree();
		if (rootRenderTree != null)
		{
			BuildDrawOperationTree(rootRenderTree);
			UpdateDrawBounds_PostOrder(m_RootOperation);
			AssignTextureIds_DepthFirst(m_RootOperation);
		}
	}

	private void BuildDrawOperationTree(RenderTree rootRenderTree)
	{
		m_RootOperation = m_DrawOperationPool.Get();
		m_RootOperation.Init(rootRenderTree);
		for (RenderTree renderTree = rootRenderTree.firstChild; renderTree != null; renderTree = renderTree.nextSibling)
		{
			AddChildrenOperations_DepthFirst(m_RootOperation, renderTree);
		}
	}

	private void AddChildrenOperations_DepthFirst(DrawOperation parentOperation, RenderTree renderTree)
	{
		VisualElement owner = renderTree.rootRenderData.owner;
		if (!(owner.resolvedStyle.filter is List<FilterFunction> list))
		{
			throw new InvalidOperationException("Filter IEnumerable is not a List<FilterFunction>");
		}
		for (int num = list.Count - 1; num >= 0; num--)
		{
			FilterFunctionDefinition definition = list[num].GetDefinition();
			if (definition?.passes != null)
			{
				for (int num2 = definition.passes.Length - 1; num2 >= 0; num2--)
				{
					PostProcessingPass filterPass = definition.passes[num2];
					if (!(filterPass.material == null))
					{
						DrawOperation drawOperation = m_DrawOperationPool.Get();
						drawOperation.Init(owner, in filterPass, num2, list[num]);
						parentOperation.AddChild(drawOperation);
						parentOperation = drawOperation;
					}
				}
			}
		}
		DrawOperation drawOperation2 = m_DrawOperationPool.Get();
		drawOperation2.Init(renderTree);
		parentOperation.AddChild(drawOperation2);
		for (RenderTree renderTree2 = renderTree.firstChild; renderTree2 != null; renderTree2 = renderTree2.nextSibling)
		{
			AddChildrenOperations_DepthFirst(drawOperation2, renderTree2);
		}
	}

	private static PostProcessingMargins GetReadMargins(PostProcessingPass effect, FilterFunction func)
	{
		if (effect.computeRequiredReadMarginsCallback != null)
		{
			return effect.computeRequiredReadMarginsCallback(func);
		}
		return effect.readMargins;
	}

	private static PostProcessingMargins GetWriteMargins(PostProcessingPass effect, FilterFunction func)
	{
		if (effect.computeRequiredWriteMarginsCallback != null)
		{
			return effect.computeRequiredWriteMarginsCallback(func);
		}
		return effect.writeMargins;
	}

	private void UpdateDrawBounds_PostOrder(DrawOperation op)
	{
		Rect? rect = null;
		switch (op.type)
		{
		case DrawOperationType.Effect:
		{
			DrawOperation firstChild = op.firstChild;
			if (firstChild != null)
			{
				Debug.Assert(firstChild.nextSibling == null);
				UpdateDrawBounds_PostOrder(firstChild);
				if (UIRUtility.RectHasArea(op.drawSourceBounds))
				{
					rect = UIRUtility.CastToRect(op.drawSourceBounds);
				}
			}
			break;
		}
		case DrawOperationType.RenderTree:
		{
			for (DrawOperation drawOperation = op.firstChild; drawOperation != null; drawOperation = drawOperation.nextSibling)
			{
				UpdateDrawBounds_PostOrder(drawOperation);
				if (UIRUtility.RectHasArea(drawOperation.bounds))
				{
					UIRUtility.ComputeMatrixRelativeToRenderTree(drawOperation.visualElement.renderData, out var transform);
					Rect rect2 = VisualElement.CalculateConservativeRect(ref transform, UIRUtility.CastToRect(drawOperation.bounds));
					rect = ((!rect.HasValue) ? rect2 : UIRUtility.Encapsulate(rect.Value, rect2));
				}
			}
			Rect boundingBox = op.renderTree.rootRenderData.owner.boundingBox;
			if (UIRUtility.RectHasArea(boundingBox))
			{
				rect = ((!rect.HasValue) ? boundingBox : UIRUtility.Encapsulate(rect.Value, boundingBox));
			}
			else
			{
				Debug.Assert(!rect.HasValue);
			}
			break;
		}
		default:
			throw new NotImplementedException();
		}
		if (rect.HasValue)
		{
			Rect value = rect.Value;
			PostProcessingMargins postProcessingMargins = default(PostProcessingMargins);
			PostProcessingMargins postProcessingMargins2 = default(PostProcessingMargins);
			DrawOperation parent = op.parent;
			RectInt bounds;
			if (parent != null && parent.type == DrawOperationType.Effect)
			{
				postProcessingMargins = GetReadMargins(parent.FilterPass, parent.filter);
				postProcessingMargins2 = GetWriteMargins(parent.FilterPass, parent.filter);
				Rect rect3 = UIRUtility.InflateByMargins(UIRUtility.InflateByMargins(value, postProcessingMargins), postProcessingMargins2);
				bounds = UIRUtility.CastToRectInt(rect3);
				Rect r = value;
				r = UIRUtility.InflateByMargins(r, postProcessingMargins2);
				op.parent.drawSourceBounds = UIRUtility.CastToRectInt(r);
				float scaledPixelsPerPoint = op.renderTree.rootRenderData.owner.scaledPixelsPerPoint;
				op.parent.drawSourceTexOffsets = new Vector4(postProcessingMargins.left * scaledPixelsPerPoint, postProcessingMargins.top * scaledPixelsPerPoint, postProcessingMargins.right * scaledPixelsPerPoint, postProcessingMargins.bottom * scaledPixelsPerPoint);
			}
			else
			{
				bounds = UIRUtility.CastToRectInt(value);
			}
			op.bounds = bounds;
		}
		else
		{
			op.bounds = RectInt.zero;
		}
		if (op.parent == null)
		{
			return;
		}
		int width = op.bounds.width;
		int height = op.bounds.height;
		float scaledPixelsPerPoint2 = op.renderTree.rootRenderData.owner.scaledPixelsPerPoint;
		width = Mathf.CeilToInt((float)width * scaledPixelsPerPoint2);
		height = Mathf.CeilToInt((float)height * scaledPixelsPerPoint2);
		if (RenderTreeAtlas.ReserveSize(width, height, out var block))
		{
			op.dstAtlasBlock = block;
			if (op.parent.type == DrawOperationType.RenderTree)
			{
				op.renderTree.quadRect = op.bounds;
				op.renderTree.quadUVRect = block.uvRect;
			}
		}
	}

	private void AssignTextureIds_DepthFirst(DrawOperation op)
	{
		DrawOperation parent = op.parent;
		if (parent != null && parent.type == DrawOperationType.RenderTree)
		{
			Debug.Assert(!op.renderTree.quadTextureId.IsValid());
			TextureId quadTextureId = (op.dstTextureId = m_RenderTreeManager.textureRegistry.AllocAndAcquireDynamic());
			op.renderTree.quadTextureId = quadTextureId;
			op.parent.renderTree.OnRenderDataVisualsChanged(op.visualElement.renderData, hierarchical: false);
		}
		else
		{
			Debug.Assert(!op.dstTextureId.IsValid());
		}
		for (DrawOperation drawOperation = op.firstChild; drawOperation != null; drawOperation = drawOperation.nextSibling)
		{
			AssignTextureIds_DepthFirst(drawOperation);
		}
	}

	public void RenderNestedPasses()
	{
		ExecuteDrawOperation_PostOrder(m_RootOperation);
	}

	private void ExecuteDrawOperation_PostOrder(DrawOperation op)
	{
		for (DrawOperation drawOperation = op.firstChild; drawOperation != null; drawOperation = drawOperation.nextSibling)
		{
			ExecuteDrawOperation_PostOrder(drawOperation);
		}
		if (op.parent == null)
		{
			return;
		}
		RectInt bounds = op.bounds;
		if (bounds.width <= 0)
		{
			return;
		}
		Debug.Assert(bounds.height > 0);
		bool forceGammaRendering = m_RenderTreeManager.forceGammaRendering;
		DrawOperation parent = op.parent;
		bool flag = parent != null && parent.type == DrawOperationType.RenderTree;
		if (RenderTreeAtlas.CreateTextureForAtlasBlock(ref op.dstAtlasBlock, forceGammaRendering && !flag, out var allocatedNewTexture))
		{
			if (allocatedNewTexture)
			{
				m_AllocatedTextures.Add(op.dstAtlasBlock.texture);
			}
			if (op.dstTextureId.IsValid())
			{
				m_RenderTreeManager.textureRegistry.UpdateDynamic(op.dstTextureId, op.dstAtlasBlock.texture);
			}
			switch (op.type)
			{
			case DrawOperationType.Effect:
				try
				{
					Debug.Assert(op.firstChild != null, "An effect draw operation must have at least one child operation to render from.");
					RenderTexture active = RenderTexture.active;
					RenderTexture texture = op.dstAtlasBlock.texture;
					RenderTexture.active = texture;
					RectInt rect = op.dstAtlasBlock.rect;
					RenderTreeAtlas.AtlasBlock dstAtlasBlock = op.firstChild.dstAtlasBlock;
					Rect uvRect = dstAtlasBlock.uvRect;
					Material material = op.FilterPass.material;
					if (forceGammaRendering && flag)
					{
						material.EnableKeyword("_UIE_OUTPUT_LINEAR");
					}
					else
					{
						material.DisableKeyword("_UIE_OUTPUT_LINEAR");
					}
					material.SetPass(op.FilterPass.passIndex);
					m_Block.SetTexture("_MainTex", dstAtlasBlock.texture);
					s_UVRects[0] = new Vector4(uvRect.x, uvRect.y, uvRect.width, uvRect.height);
					m_Block.SetVectorArray("unity_uie_UVRect", s_UVRects);
					bool readsGamma = QualitySettings.activeColorSpace == ColorSpace.Gamma || forceGammaRendering;
					if (op.FilterPass.applySettingsCallback != null)
					{
						op.FilterPass.applySettingsCallback(m_Block, new FilterPassContext
						{
							filterFunction = op.filter,
							filterPassIndex = op.FilterPassIndex,
							readsGamma = readsGamma,
							writesGamma = (QualitySettings.activeColorSpace == ColorSpace.Gamma || (forceGammaRendering && flag)),
							scaledPixelsPerPoint = op.visualElement.scaledPixelsPerPoint
						});
					}
					else
					{
						ApplyEffectParameters(op.FilterPass, op.filter, op.visualElement, readsGamma);
					}
					Utility.SetPropertyBlock(m_Block);
					Matrix4x4 mat = ProjectionUtils.Ortho(bounds.xMin, bounds.xMax, bounds.yMax, bounds.yMin, 0f, 1f);
					GL.LoadProjectionMatrix(mat);
					GL.modelview = Matrix4x4.identity;
					RectInt drawSourceBounds = op.drawSourceBounds;
					Vector4 drawSourceTexOffsets = op.drawSourceTexOffsets;
					float num = dstAtlasBlock.texture.width;
					float num2 = dstAtlasBlock.texture.height;
					Rect rect2 = new Rect(uvRect.x + drawSourceTexOffsets.x / num, uvRect.y + drawSourceTexOffsets.y / num2, uvRect.width - (drawSourceTexOffsets.x + drawSourceTexOffsets.z) / num, uvRect.height - (drawSourceTexOffsets.y + drawSourceTexOffsets.w) / num2);
					GL.Viewport(new Rect(rect.xMin, rect.yMin, rect.width, rect.height));
					GL.Begin(7);
					GL.TexCoord2(rect2.xMin, rect2.yMin);
					GL.MultiTexCoord2(1, 0f, 0f);
					GL.Vertex3(drawSourceBounds.xMin, drawSourceBounds.yMax, 0.5f);
					GL.TexCoord2(rect2.xMin, rect2.yMax);
					GL.MultiTexCoord2(1, 0f, 0f);
					GL.Vertex3(drawSourceBounds.xMin, drawSourceBounds.yMin, 0.5f);
					GL.TexCoord2(rect2.xMax, rect2.yMax);
					GL.MultiTexCoord2(1, 0f, 0f);
					GL.Vertex3(drawSourceBounds.xMax, drawSourceBounds.yMin, 0.5f);
					GL.TexCoord2(rect2.xMax, rect2.yMin);
					GL.MultiTexCoord2(1, 0f, 0f);
					GL.Vertex3(drawSourceBounds.xMax, drawSourceBounds.yMax, 0.5f);
					GL.End();
					RenderTexture.active = active;
					break;
				}
				catch
				{
					break;
				}
			case DrawOperationType.RenderTree:
				m_RenderTreeManager.RenderSingleTree(op.renderTree, op.dstAtlasBlock.texture, op.dstAtlasBlock.rect, UIRUtility.CastToRect(bounds));
				break;
			default:
				throw new NotImplementedException();
			}
		}
		else
		{
			Debug.LogError($"Failed to create a texture for draw operation with bounds {bounds}.");
		}
	}

	private void ApplyEffectParameters(PostProcessingPass effect, FilterFunction filter, VisualElement source, bool readsGamma)
	{
		if (effect.parameterBindings == null)
		{
			return;
		}
		FixedBuffer4<FilterParameter> parameters = filter.parameters;
		int parameterCount = filter.parameterCount;
		for (int i = 0; i < effect.parameterBindings.Length && i < parameterCount; i++)
		{
			ParameterBinding parameterBinding = effect.parameterBindings[i];
			FilterParameter filterParameter = parameters[i];
			if (filterParameter.type == FilterParameterType.Float)
			{
				m_Block.SetFloat(parameterBinding.name, filterParameter.floatValue);
			}
			else if (filterParameter.type == FilterParameterType.Color)
			{
				m_Block.SetVector(parameterBinding.name, readsGamma ? filterParameter.colorValue : filterParameter.colorValue.linear);
			}
		}
	}

	private void CleanupOperationTree()
	{
		if (m_RootOperation != null)
		{
			CleanupOperation_PostOrder(m_RootOperation);
			m_RootOperation = null;
		}
		foreach (RenderTexture allocatedTexture in m_AllocatedTextures)
		{
			RenderTexture.ReleaseTemporary(allocatedTexture);
		}
		m_AllocatedTextures.Clear();
	}

	private void CleanupOperation_PostOrder(DrawOperation op)
	{
		for (DrawOperation drawOperation = op.firstChild; drawOperation != null; drawOperation = drawOperation.nextSibling)
		{
			CleanupOperation_PostOrder(drawOperation);
		}
		if (op.dstTextureId.IsValid())
		{
			m_RenderTreeManager.textureRegistry.Release(op.dstTextureId);
			op.dstTextureId = TextureId.invalid;
			op.renderTree.quadTextureId = TextureId.invalid;
		}
		op.Reset();
		m_DrawOperationPool.Release(op);
	}

	public void Dispose()
	{
		Dispose(disposing: true);
		GC.SuppressFinalize(this);
	}

	protected void Dispose(bool disposing)
	{
		if (!disposed)
		{
			if (disposing)
			{
				CleanupOperationTree();
			}
			disposed = true;
		}
	}
}
