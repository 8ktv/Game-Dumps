#define UNITY_ASSERTIONS
using System;
using System.Runtime.InteropServices;
using AOT;
using Unity.Profiling;

namespace UnityEngine.UIElements.Layout;

internal static class LayoutDelegates
{
	private static readonly ProfilerMarker s_InvokeMeasureFunctionMarker = new ProfilerMarker("InvokeMeasureFunction");

	private static readonly ProfilerMarker s_InvokeBaselineFunctionMarker = new ProfilerMarker("InvokeBaselineFunction");

	private static readonly InvokeMeasureFunctionDelegate s_InvokeMeasureDelegate = InvokeMeasureFunction;

	private static readonly InvokeBaselineFunctionDelegate s_InvokeBaselineDelegate = InvokeBaselineFunction;

	internal static readonly IntPtr s_InvokeMeasureFunction = Marshal.GetFunctionPointerForDelegate(s_InvokeMeasureDelegate);

	internal static readonly IntPtr s_InvokeBaselineFunction = Marshal.GetFunctionPointerForDelegate(s_InvokeBaselineDelegate);

	[MonoPInvokeCallback(typeof(InvokeMeasureFunctionDelegate))]
	private static void InvokeMeasureFunction(ref LayoutNode node, float width, LayoutMeasureMode widthMode, float height, LayoutMeasureMode heightMode, ref IntPtr exception, out LayoutSize result)
	{
		LayoutMeasureFunction measure = node.Config.Measure;
		if (measure == null)
		{
			Debug.Assert(condition: false, "No measure function set in this node's config");
			result = default(LayoutSize);
			return;
		}
		try
		{
			using (s_InvokeMeasureFunctionMarker.Auto())
			{
				measure(node.GetOwner(), ref node, width, widthMode, height, heightMode, out result);
			}
		}
		catch (Exception value)
		{
			GCHandle value2 = GCHandle.Alloc(value);
			exception = GCHandle.ToIntPtr(value2);
			result = default(LayoutSize);
		}
	}

	[MonoPInvokeCallback(typeof(InvokeBaselineFunctionDelegate))]
	private static float InvokeBaselineFunction(ref LayoutNode node, float width, float height)
	{
		LayoutBaselineFunction baseline = node.Config.Baseline;
		if (baseline == null)
		{
			Debug.Assert(condition: false, "No baselineFunction function set in this node's config");
			return 0f;
		}
		using (s_InvokeBaselineFunctionMarker.Auto())
		{
			return baseline(ref node, width, height);
		}
	}
}
