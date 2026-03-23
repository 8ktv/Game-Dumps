using System;
using Unity.Profiling;

namespace UnityEngine.UIElements.UIR;

internal class RenderChainCommand : LinkedPoolItem<RenderChainCommand>
{
	internal RenderData owner;

	internal RenderChainCommand prev;

	internal RenderChainCommand next;

	internal bool isTail;

	internal CommandType type;

	internal State state;

	internal MeshHandle mesh;

	internal int indexOffset;

	internal int indexCount;

	internal Action callback;

	private static readonly int k_ID_MainTex = Shader.PropertyToID("_MainTex");

	private static ProfilerMarker s_ImmediateOverheadMarker = new ProfilerMarker("UIR.ImmediateOverhead");

	internal void Reset()
	{
		owner = null;
		prev = (next = null);
		isTail = false;
		type = CommandType.Draw;
		state = default(State);
		mesh = null;
		indexOffset = (indexCount = 0);
		callback = null;
	}

	internal void ExecuteNonDrawMesh(DrawParams drawParams, float pixelsPerPoint, ref Exception immediateException)
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
		{
			if (immediateException != null)
			{
				break;
			}
			Matrix4x4 unityProjectionMatrix = Utility.GetUnityProjectionMatrix();
			Camera current = Camera.current;
			RenderTexture active = RenderTexture.active;
			bool flag = drawParams.scissor.Count > 1;
			if (flag)
			{
				Utility.DisableScissor();
			}
			using (new GUIClip.ParentClipScope(owner.owner.worldTransform, owner.owner.worldClip))
			{
				try
				{
					callback();
				}
				catch (Exception ex)
				{
					immediateException = ex;
				}
			}
			Camera.SetupCurrent(current);
			RenderTexture.active = active;
			GL.modelview = drawParams.view.Peek();
			GL.LoadProjectionMatrix(unityProjectionMatrix);
			if (flag)
			{
				Utility.SetScissorRect(RectPointsToPixelsAndFlipYAxis(drawParams.scissor.Peek(), pixelsPerPoint));
			}
			break;
		}
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

	internal static void PushScissor(DrawParams drawParams, Rect scissor, float pixelsPerPoint)
	{
		Rect rect = CombineScissorRects(scissor, drawParams.scissor.Peek());
		drawParams.scissor.Push(rect);
		Utility.SetScissorRect(RectPointsToPixelsAndFlipYAxis(rect, pixelsPerPoint));
	}

	internal static void PopScissor(DrawParams drawParams, float pixelsPerPoint)
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

	private void Blit(Texture source, RenderTexture destination, float depth)
	{
		GL.PushMatrix();
		GL.LoadOrtho();
		RenderTexture.active = destination;
		state.material.SetTexture(k_ID_MainTex, source);
		state.material.SetPass(0);
		GL.Begin(7);
		GL.TexCoord2(0f, 0f);
		GL.Vertex3(0f, 0f, depth);
		GL.TexCoord2(0f, 1f);
		GL.Vertex3(0f, 1f, depth);
		GL.TexCoord2(1f, 1f);
		GL.Vertex3(1f, 1f, depth);
		GL.TexCoord2(1f, 0f);
		GL.Vertex3(1f, 0f, depth);
		GL.End();
		GL.PopMatrix();
	}

	private static Vector4 RectToClipSpace(Rect rc)
	{
		Matrix4x4 deviceProjectionMatrix = Utility.GetDeviceProjectionMatrix();
		Vector3 vector = deviceProjectionMatrix.MultiplyPoint(new Vector3(rc.xMin, rc.yMin, 0f));
		Vector3 vector2 = deviceProjectionMatrix.MultiplyPoint(new Vector3(rc.xMax, rc.yMax, 0f));
		return new Vector4(Mathf.Min(vector.x, vector2.x), Mathf.Min(vector.y, vector2.y), Mathf.Max(vector.x, vector2.x), Mathf.Max(vector.y, vector2.y));
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
