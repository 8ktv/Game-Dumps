#define UNITY_ASSERTIONS
using System;
using System.Collections.Generic;
using UnityEngine.Experimental.Rendering;
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

		private PostProcessingPass m_Effect;

		private FilterFunction m_Filter;

		public RectInt bounds;

		public RectInt drawSourceBounds;

		public Vector4 drawSourceTexOffsets;

		public RenderTexture dstTexture;

		public TextureId dstTextureId;

		public DrawOperation parent;

		public DrawOperation firstChild;

		public DrawOperation lastChild;

		public DrawOperation prevSibling;

		public DrawOperation nextSibling;

		public DrawOperationType type => m_Type;

		public VisualElement visualElement => m_VisualElement;

		public RenderTree renderTree => m_RenderTree;

		public PostProcessingPass effect => m_Effect;

		public FilterFunction filter => m_Filter;

		public void Init(VisualElement ve, in PostProcessingPass effect, FilterFunction filter)
		{
			m_Type = DrawOperationType.Effect;
			m_VisualElement = ve;
			m_Effect = effect;
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
			m_Effect = default(PostProcessingPass);
			m_Filter = default(FilterFunction);
			dstTexture = null;
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

	private List<RenderTexture> m_AllocatedRenderTextures = new List<RenderTexture>();

	private MaterialPropertyBlock m_Block = new MaterialPropertyBlock();

	private ObjectPool<DrawOperation> m_DrawOperationPool = new ObjectPool<DrawOperation>(() => new DrawOperation());

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

	private static void UpdateDrawBounds_PostOrder(DrawOperation op)
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
			PostProcessingMargins margins = default(PostProcessingMargins);
			PostProcessingMargins margins2 = default(PostProcessingMargins);
			DrawOperation parent = op.parent;
			RectInt bounds;
			if (parent != null && parent.type == DrawOperationType.Effect)
			{
				margins = GetReadMargins(parent.effect, parent.filter);
				margins2 = GetWriteMargins(parent.effect, parent.filter);
				Rect rect3 = UIRUtility.InflateByMargins(UIRUtility.InflateByMargins(value, margins), margins2);
				bounds = UIRUtility.CastToRectInt(rect3);
			}
			else
			{
				bounds = UIRUtility.CastToRectInt(value);
			}
			DrawOperation parent2 = op.parent;
			if (parent2 != null && parent2.type == DrawOperationType.Effect)
			{
				Rect r = value;
				r = UIRUtility.InflateByMargins(r, margins2);
				op.parent.drawSourceBounds = UIRUtility.CastToRectInt(r);
				Vector4 drawSourceTexOffsets = new Vector4(margins.left, margins.top, margins.right, margins.bottom);
				if (bounds.width > 0 && bounds.height > 0)
				{
					float num = 1f / (float)bounds.width;
					float num2 = 1f / (float)bounds.height;
					drawSourceTexOffsets.x *= num;
					drawSourceTexOffsets.y *= num2;
					drawSourceTexOffsets.z *= num;
					drawSourceTexOffsets.w *= num2;
				}
				else
				{
					drawSourceTexOffsets = Vector4.zero;
				}
				op.parent.drawSourceTexOffsets = drawSourceTexOffsets;
			}
			op.bounds = bounds;
		}
		else
		{
			op.bounds = RectInt.zero;
		}
		DrawOperation parent3 = op.parent;
		if (parent3 != null && parent3.type == DrawOperationType.RenderTree)
		{
			op.renderTree.quadRect = op.bounds;
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
		GraphicsFormat colorFormat = ((QualitySettings.activeColorSpace == ColorSpace.Linear) ? GraphicsFormat.R8G8B8A8_SRGB : GraphicsFormat.R8G8B8A8_UNorm);
		RenderTextureDescriptor desc = new RenderTextureDescriptor(bounds.width, bounds.height, colorFormat, GraphicsFormat.D24_UNorm_S8_UInt);
		op.dstTexture = RenderTexture.GetTemporary(desc);
		m_AllocatedRenderTextures.Add(op.dstTexture);
		if (op.dstTextureId.IsValid())
		{
			m_RenderTreeManager.textureRegistry.UpdateDynamic(op.dstTextureId, op.dstTexture);
		}
		switch (op.type)
		{
		case DrawOperationType.Effect:
			try
			{
				RenderTexture active = RenderTexture.active;
				RenderTexture.active = op.dstTexture;
				GL.Clear(clearDepth: false, clearColor: true, Color.clear);
				op.effect.material.SetPass(op.effect.passIndex);
				m_Block.SetTexture("_MainTex", op.firstChild.dstTexture);
				if (op.effect.prepareMaterialPropertyBlockCallback != null)
				{
					op.effect.prepareMaterialPropertyBlockCallback(m_Block, op.filter);
				}
				else
				{
					ApplyEffectParameters(op.effect, op.filter, op.visualElement);
				}
				Utility.SetPropertyBlock(m_Block);
				Matrix4x4 mat = ProjectionUtils.Ortho(bounds.xMin, bounds.xMax, bounds.yMax, bounds.yMin, 0f, 1f);
				GL.LoadProjectionMatrix(mat);
				GL.modelview = Matrix4x4.identity;
				RectInt drawSourceBounds = op.drawSourceBounds;
				Vector4 drawSourceTexOffsets = op.drawSourceTexOffsets;
				GL.Viewport(new Rect(0f, 0f, bounds.width, bounds.height));
				GL.Begin(7);
				GL.TexCoord2(drawSourceTexOffsets.x, drawSourceTexOffsets.w);
				GL.Vertex3(drawSourceBounds.xMin, drawSourceBounds.yMax, 0.5f);
				GL.TexCoord2(drawSourceTexOffsets.x, 1f - drawSourceTexOffsets.y);
				GL.Vertex3(drawSourceBounds.xMin, drawSourceBounds.yMin, 0.5f);
				GL.TexCoord2(1f - drawSourceTexOffsets.z, 1f - drawSourceTexOffsets.y);
				GL.Vertex3(drawSourceBounds.xMax, drawSourceBounds.yMin, 0.5f);
				GL.TexCoord2(1f - drawSourceTexOffsets.z, drawSourceTexOffsets.w);
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
			m_RenderTreeManager.RenderSingleTree(op.renderTree, op.dstTexture, bounds);
			break;
		default:
			throw new NotImplementedException();
		}
	}

	private void ApplyEffectParameters(PostProcessingPass effect, FilterFunction filter, VisualElement source)
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
				m_Block.SetColor(parameterBinding.name, filterParameter.colorValue);
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
		for (int i = 0; i < m_AllocatedRenderTextures.Count; i++)
		{
			RenderTexture.ReleaseTemporary(m_AllocatedRenderTextures[i]);
		}
		m_AllocatedRenderTextures.Clear();
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
