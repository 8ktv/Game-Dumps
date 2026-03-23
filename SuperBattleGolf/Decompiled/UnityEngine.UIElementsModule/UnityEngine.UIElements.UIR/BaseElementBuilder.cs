#define UNITY_ASSERTIONS
namespace UnityEngine.UIElements.UIR;

internal abstract class BaseElementBuilder
{
	public abstract bool RequiresStencilMask(VisualElement ve);

	public void Build(MeshGenerationContext mgc)
	{
		if (mgc.renderData.isSubTreeQuad)
		{
			BuildRenderTreeQuadElement(mgc);
		}
		else
		{
			BuildStandardElement(mgc);
		}
	}

	private void BuildRenderTreeQuadElement(MeshGenerationContext mgc)
	{
		VisualElement visualElement = mgc.visualElement;
		RenderTree renderTree = visualElement.nestedRenderData.renderTree;
		RectInt quadRect = renderTree.quadRect;
		if (quadRect != RectInt.zero)
		{
			Color white = Color.white;
			mgc.AllocateTempMesh(4, 6, out var vertices, out var indices);
			vertices[0] = new Vertex
			{
				position = new Vector3(quadRect.xMin, quadRect.yMax, Vertex.nearZ),
				tint = white,
				uv = new Vector2(0f, 0f)
			};
			vertices[1] = new Vertex
			{
				position = new Vector3(quadRect.xMin, quadRect.yMin, Vertex.nearZ),
				tint = white,
				uv = new Vector2(0f, 1f)
			};
			vertices[2] = new Vertex
			{
				position = new Vector3(quadRect.xMax, quadRect.yMin, Vertex.nearZ),
				tint = white,
				uv = new Vector2(1f, 1f)
			};
			vertices[3] = new Vertex
			{
				position = new Vector3(quadRect.xMax, quadRect.yMax, Vertex.nearZ),
				tint = white,
				uv = new Vector2(1f, 0f)
			};
			indices[0] = 0;
			indices[1] = 1;
			indices[2] = 2;
			indices[3] = 2;
			indices[4] = 3;
			indices[5] = 0;
			mgc.entryRecorder.DrawMesh(mgc.parentEntry, vertices, indices, renderTree.quadTextureId, isPremultiplied: true);
		}
		mgc.entryRecorder.DrawChildren(mgc.parentEntry);
	}

	private void BuildStandardElement(MeshGenerationContext mgc)
	{
		VisualElement visualElement = mgc.visualElement;
		RenderData renderData = mgc.renderData;
		Debug.Assert(visualElement.areAncestorsAndSelfDisplayed);
		if (visualElement.isWorldSpaceRootUIDocument)
		{
			mgc.entryRecorder.CutRenderChain(mgc.parentEntry);
		}
		bool isGroupTransform = renderData.isGroupTransform;
		if (isGroupTransform)
		{
			mgc.entryRecorder.PushGroupMatrix(mgc.parentEntry);
		}
		bool flag = visualElement.defaultMaterial != null;
		if (flag)
		{
			mgc.entryRecorder.PushDefaultMaterial(mgc.parentEntry, visualElement.defaultMaterial);
		}
		bool flag2 = false;
		if (visualElement.visible)
		{
			DrawVisualElementBackground(mgc);
			DrawVisualElementBorder(mgc);
			PushVisualElementClipping(mgc);
			flag2 = true;
			InvokeGenerateVisualContent(mgc);
		}
		else
		{
			bool flag3 = renderData.clipMethod == ClipMethod.Stencil;
			bool flag4 = renderData.clipMethod == ClipMethod.Scissor;
			if (flag4 || flag3)
			{
				flag2 = true;
				PushVisualElementClipping(mgc);
			}
		}
		mgc.entryRecorder.DrawChildren(mgc.parentEntry);
		if (flag2)
		{
			PopVisualElementClipping(mgc);
		}
		if (flag)
		{
			mgc.entryRecorder.PopDefaultMaterial(mgc.parentEntry);
		}
		if (isGroupTransform)
		{
			mgc.entryRecorder.PopGroupMatrix(mgc.parentEntry);
		}
	}

	protected abstract void DrawVisualElementBackground(MeshGenerationContext mgc);

	protected abstract void DrawVisualElementBorder(MeshGenerationContext mgc);

	protected abstract void DrawVisualElementStencilMask(MeshGenerationContext mgc);

	public abstract void ScheduleMeshGenerationJobs(MeshGenerationContext mgc);

	private void PushVisualElementClipping(MeshGenerationContext mgc)
	{
		RenderData renderData = mgc.renderData;
		if (renderData.clipMethod == ClipMethod.Scissor)
		{
			mgc.entryRecorder.PushScissors(mgc.parentEntry);
		}
		else if (renderData.clipMethod == ClipMethod.Stencil)
		{
			mgc.entryRecorder.BeginStencilMask(mgc.parentEntry);
			DrawVisualElementStencilMask(mgc);
			mgc.entryRecorder.EndStencilMask(mgc.parentEntry);
		}
		mgc.entryRecorder.PushClippingRect(mgc.parentEntry);
	}

	private static void PopVisualElementClipping(MeshGenerationContext mgc)
	{
		RenderData renderData = mgc.renderData;
		mgc.entryRecorder.PopClippingRect(mgc.parentEntry);
		if (renderData.clipMethod == ClipMethod.Scissor)
		{
			mgc.entryRecorder.PopScissors(mgc.parentEntry);
		}
		else if (renderData.clipMethod == ClipMethod.Stencil)
		{
			mgc.entryRecorder.PopStencilMask(mgc.parentEntry);
		}
	}

	private static void InvokeGenerateVisualContent(MeshGenerationContext mgc)
	{
		VisualElement visualElement = mgc.visualElement;
		Painter2D.isPainterActive = true;
		visualElement.InvokeGenerateVisualContent(mgc);
		Painter2D.isPainterActive = false;
	}
}
