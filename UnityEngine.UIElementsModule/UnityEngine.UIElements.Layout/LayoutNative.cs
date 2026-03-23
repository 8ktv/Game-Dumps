using System;
using System.Runtime.CompilerServices;
using UnityEngine.Bindings;
using UnityEngine.Scripting;

namespace UnityEngine.UIElements.Layout;

[NativeHeader("Modules/UIElements/Core/Layout/Native/LayoutNative.h")]
internal static class LayoutNative
{
	internal enum LayoutLogEventType
	{
		None,
		Error,
		Measure,
		Layout,
		CacheUsage,
		BeginLayout,
		EndLayout
	}

	internal class LayoutLogData
	{
		public LayoutNode node;

		public LayoutLogEventType eventType;

		public string message;
	}

	internal static event Action<LayoutLogData> onLayoutLog;

	[MethodImpl(MethodImplOptions.InternalCall)]
	[NativeMethod(IsThreadSafe = false)]
	internal static extern void CalculateLayout(IntPtr node, float parentWidth, float parentHeight, int parentDirection, IntPtr state, IntPtr exceptionGCHandle);

	[RequiredByNativeCode]
	private unsafe static void LayoutLog_Internal(IntPtr nodePtr, LayoutLogEventType type, string message)
	{
		LayoutLogData layoutLogData = new LayoutLogData();
		layoutLogData.node = *(LayoutNode*)(void*)nodePtr;
		layoutLogData.message = message;
		layoutLogData.eventType = type;
		LayoutNative.onLayoutLog(layoutLogData);
	}
}
