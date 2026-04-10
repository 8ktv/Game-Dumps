#define UNITY_ASSERTIONS
using System;
using System.Runtime.CompilerServices;
using Unity.Jobs.LowLevel.Unsafe;
using Unity.Profiling;
using UnityEngine.UIElements.UIR;

namespace UnityEngine.UIElements;

internal static class UIRUtility
{
	private static readonly ProfilerMarker k_ComputeTransformMatrixMarker = new ProfilerMarker("UIR.ComputeTransformMatrix");

	public static readonly string k_DefaultShaderName = Shaders.k_Default;

	public const float k_Epsilon = 1E-30f;

	public const float k_ClearZ = 0.99f;

	public const float k_MeshPosZ = 0f;

	public const float k_MaskPosZ = 1f;

	public const int k_MaxMaskDepth = 7;

	public const float k_RenderTextureMargin = 1f;

	public const byte k_DynamicColorDisabled = 0;

	public const byte k_DynamicColorEnabled = 1;

	public const byte k_DynamicColorEnabledText = 2;

	[ThreadStatic]
	private static int? s_ThreadIndex;

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static bool ShapeWindingIsClockwise(int maskDepth, int stencilRef)
	{
		Debug.Assert(maskDepth == stencilRef || maskDepth == stencilRef + 1);
		return maskDepth == stencilRef;
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static Rect Encapsulate(Rect a, Rect b)
	{
		Vector2 vector = Vector2.Min(a.min, b.min);
		Vector2 vector2 = Vector2.Max(a.max, b.max);
		return new Rect(vector, vector2 - vector);
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static Rect Inflate(Rect r, float i)
	{
		return new Rect(r.xMin - i, r.yMin - i, r.width + i + i, r.height + i + i);
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static Rect InflateByMargins(Rect r, PostProcessingMargins margins)
	{
		return new Rect(r.xMin - margins.left, r.yMin - margins.top, r.width + margins.left + margins.right, r.height + margins.top + margins.bottom);
	}

	private static void ComputeMatrixRelativeToAncestor(RenderData renderData, RenderData ancestor, out Matrix4x4 transform)
	{
		if (ancestor.worldTransformScaleZero)
		{
			ComputeTransformMatrix(renderData, ancestor, out transform);
		}
		else
		{
			VisualElement.MultiplyMatrix34(ref ancestor.owner.worldTransformInverse, ref renderData.owner.worldTransformRef, out transform);
		}
	}

	public static void ComputeMatrixRelativeToRenderTree(RenderData renderData, out Matrix4x4 transform)
	{
		RenderData rootRenderData = renderData.renderTree.rootRenderData;
		if (rootRenderData.isNestedRenderTreeRoot)
		{
			ComputeMatrixRelativeToAncestor(renderData, rootRenderData, out transform);
		}
		else
		{
			transform = renderData.owner.worldTransform;
		}
	}

	public static void GetVerticesTransformInfo(RenderData renderData, out Matrix4x4 transform)
	{
		if (RenderData.AllocatesID(renderData.transformID) || renderData.isGroupTransform || renderData.isNestedRenderTreeRoot)
		{
			transform = Matrix4x4.identity;
		}
		else if (renderData.boneTransformAncestor != null)
		{
			ComputeMatrixRelativeToAncestor(renderData, renderData.boneTransformAncestor, out transform);
		}
		else if (renderData.groupTransformAncestor != null)
		{
			ComputeMatrixRelativeToAncestor(renderData, renderData.groupTransformAncestor, out transform);
		}
		else
		{
			ComputeMatrixRelativeToRenderTree(renderData, out transform);
		}
		BaseVisualElementPanel elementPanel = renderData.owner.elementPanel;
		if (elementPanel != null && elementPanel.isFlat)
		{
			transform.m22 = 1f;
		}
	}

	internal static void ComputeTransformMatrix(RenderData renderData, RenderData ancestor, out Matrix4x4 result)
	{
		Debug.Assert(renderData.renderTree == ancestor.renderTree);
		renderData.owner.GetPivotedMatrixWithLayout(out result);
		RenderData parent = renderData.parent;
		if (parent == null || ancestor == parent)
		{
			return;
		}
		Matrix4x4 rhs = default(Matrix4x4);
		bool flag = true;
		do
		{
			parent.owner.GetPivotedMatrixWithLayout(out var result2);
			if (flag)
			{
				VisualElement.MultiplyMatrix34(ref result2, ref result, out rhs);
			}
			else
			{
				VisualElement.MultiplyMatrix34(ref result2, ref rhs, out result);
			}
			parent = parent.parent;
			flag = !flag;
		}
		while (parent != null && ancestor != parent);
		if (!flag)
		{
			result = rhs;
		}
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static Vector4 ToVector4(Rect rc)
	{
		return new Vector4(rc.xMin, rc.yMin, rc.xMax, rc.yMax);
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static bool RectHasArea(Rect rect)
	{
		return rect.width > 1E-30f && rect.height > 1E-30f;
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static bool RectHasArea(RectInt rect)
	{
		return rect.width > 0 && rect.height > 0;
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static Rect CastToRect(RectInt rect)
	{
		return new Rect(rect.xMin, rect.yMin, rect.width, rect.height);
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static RectInt CastToRectInt(Rect rect)
	{
		return new RectInt(Mathf.FloorToInt(rect.xMin), Mathf.FloorToInt(rect.yMin), Mathf.CeilToInt(rect.width), Mathf.CeilToInt(rect.height));
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static bool IsRoundRect(VisualElement ve)
	{
		IResolvedStyle resolvedStyle = ve.resolvedStyle;
		return !(resolvedStyle.borderTopLeftRadius < 1E-30f) || !(resolvedStyle.borderTopRightRadius < 1E-30f) || !(resolvedStyle.borderBottomLeftRadius < 1E-30f) || !(resolvedStyle.borderBottomRightRadius < 1E-30f);
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static void Multiply2D(this Quaternion rotation, ref Vector2 point)
	{
		float num = rotation.z * 2f;
		float num2 = 1f - rotation.z * num;
		float num3 = rotation.w * num;
		point = new Vector2(num2 * point.x - num3 * point.y, num3 * point.x + num2 * point.y);
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static bool IsVectorImageBackground(VisualElement ve)
	{
		return ve.computedStyle.backgroundImage.vectorImage != null;
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static bool IsElementSelfHidden(VisualElement ve)
	{
		return ve.resolvedStyle.visibility == Visibility.Hidden;
	}

	public static void Destroy(Object obj)
	{
		if (!(obj == null))
		{
			if (Application.isPlaying)
			{
				Object.Destroy(obj);
			}
			else
			{
				Object.DestroyImmediate(obj);
			}
		}
	}

	public static int GetPrevPow2(int n)
	{
		int num = 0;
		while (n > 1)
		{
			n >>= 1;
			num++;
		}
		return 1 << num;
	}

	public static int GetNextPow2(int n)
	{
		int num;
		for (num = 1; num < n; num <<= 1)
		{
		}
		return num;
	}

	public static int GetNextPow2Exp(int n)
	{
		int num = 1;
		int num2 = 0;
		while (num < n)
		{
			num <<= 1;
			num2++;
		}
		return num2;
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static int GetThreadIndex()
	{
		int? num = s_ThreadIndex;
		if (num.HasValue)
		{
			return num.Value;
		}
		int threadIndex = JobsUtility.ThreadIndex;
		s_ThreadIndex = threadIndex;
		return threadIndex;
	}
}
