using System;
using Unity.Profiling;

namespace UnityEngine.UIElements.UIR;

internal class RenderChainCommand : LinkedPoolItem<RenderChainCommand>
{
	public RenderData owner;

	public RenderChainCommand prev;

	public RenderChainCommand next;

	public CommandType type;

	public CommandFlags flags;

	public Material material;

	public MaterialPropertyBlock userProps;

	public TextureId texture;

	public int stencilRef;

	public float sdfScale;

	public float sharpness;

	public MeshHandle mesh;

	public int indexOffset;

	public int indexCount;

	public Action callback;

	private static ProfilerMarker s_ImmediateOverheadMarker = new ProfilerMarker("UIR.ImmediateOverhead");

	public RenderChainCommand()
	{
		Reset();
	}

	public void Reset()
	{
		owner = null;
		prev = (next = null);
		type = CommandType.Draw;
		flags = CommandFlags.None;
		material = null;
		userProps = null;
		texture = TextureId.invalid;
		stencilRef = 0;
		sdfScale = 0f;
		sharpness = 0f;
		mesh = null;
		indexOffset = (indexCount = 0);
		callback = null;
	}

	public void ExecuteNonDrawMesh(DrawParams drawParams, float pixelsPerPoint, ref Exception immediateException)
	{
		switch (type)
		{
		case CommandType.ImmediateCull:
			if (!RectPointsToPixelsAndFlipYAxis(owner.owner.worldBound, pixelsPerPoint).Overlaps(Utility.GetActiveViewport()))
			{
				break;
			}
			goto case CommandType.Immediate;
		case CommandType.Immediate:
			if (immediateException == null && !(owner.compositeOpacity < 0.001f))
			{
				Matrix4x4 unityProjectionMatrix = Utility.GetUnityProjectionMatrix();
				Camera current = Camera.current;
				RenderTexture active = RenderTexture.active;
				UIRUtility.ComputeMatrixRelativeToRenderTree(owner, out var transform2);
				GL.modelview = transform2;
				PushScissor(drawParams, owner.clippingRect, pixelsPerPoint);
				try
				{
					callback();
				}
				catch (Exception ex)
				{
					immediateException = ex;
				}
				PopScissor(drawParams, pixelsPerPoint);
				Camera.SetupCurrent(current);
				RenderTexture.active = active;
				GL.modelview = drawParams.view.Peek();
				GL.LoadProjectionMatrix(unityProjectionMatrix);
			}
			break;
		case CommandType.PushView:
		{
			UIRUtility.ComputeMatrixRelativeToRenderTree(owner, out var transform);
			drawParams.view.Push(transform);
			GL.modelview = transform;
			Rect scissor = owner.parent?.clippingRect ?? DrawParams.k_FullNormalizedRect;
			PushScissor(drawParams, scissor, pixelsPerPoint);
			break;
		}
		case CommandType.PopView:
			drawParams.view.Pop();
			GL.modelview = drawParams.view.Peek();
			PopScissor(drawParams, pixelsPerPoint);
			break;
		case CommandType.PushScissor:
			PushScissor(drawParams, owner.clippingRect, pixelsPerPoint);
			break;
		case CommandType.PopScissor:
			PopScissor(drawParams, pixelsPerPoint);
			break;
		case CommandType.PushDefaultMaterial:
		case CommandType.PopDefaultMaterial:
			break;
		}
	}

	public static void PushScissor(DrawParams drawParams, Rect scissor, float pixelsPerPoint)
	{
		Rect rect = CombineScissorRects(scissor, drawParams.scissor.Peek());
		drawParams.scissor.Push(rect);
		Utility.SetScissorRect(RectPointsToPixelsAndFlipYAxis(rect, pixelsPerPoint));
	}

	public static void PopScissor(DrawParams drawParams, float pixelsPerPoint)
	{
		drawParams.scissor.Pop();
		Rect rect = drawParams.scissor.Peek();
		if (rect.x == DrawParams.k_UnlimitedRect.x)
		{
			Utility.DisableScissor();
		}
		else
		{
			Utility.SetScissorRect(RectPointsToPixelsAndFlipYAxis(rect, pixelsPerPoint));
		}
	}

	private static Rect CombineScissorRects(Rect r0, Rect r1)
	{
		Rect result = new Rect(0f, 0f, 0f, 0f);
		result.x = Math.Max(r0.x, r1.x);
		result.y = Math.Max(r0.y, r1.y);
		result.xMax = Math.Max(result.x, Math.Min(r0.xMax, r1.xMax));
		result.yMax = Math.Max(result.y, Math.Min(r0.yMax, r1.yMax));
		return result;
	}

	private static RectInt RectPointsToPixelsAndFlipYAxis(Rect rect, float pixelsPerPoint)
	{
		float num = Utility.GetActiveViewport().height;
		RectInt result = new RectInt(0, 0, 0, 0);
		result.x = Mathf.RoundToInt(rect.x * pixelsPerPoint);
		result.y = Mathf.RoundToInt(num - rect.yMax * pixelsPerPoint);
		result.width = Mathf.RoundToInt(rect.width * pixelsPerPoint);
		result.height = Mathf.RoundToInt(rect.height * pixelsPerPoint);
		return result;
	}
}
