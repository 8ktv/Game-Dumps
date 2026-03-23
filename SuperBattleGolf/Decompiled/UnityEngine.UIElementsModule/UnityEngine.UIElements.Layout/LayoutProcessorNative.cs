using System;
using System.Runtime.ExceptionServices;
using System.Runtime.InteropServices;

namespace UnityEngine.UIElements.Layout;

internal class LayoutProcessorNative : ILayoutProcessor
{
	private LayoutState m_State = LayoutState.Default;

	unsafe void ILayoutProcessor.CalculateLayout(LayoutNode node, float parentWidth, float parentHeight, LayoutDirection parentDirection)
	{
		IntPtr node2 = (IntPtr)(&node);
		IntPtr zero = IntPtr.Zero;
		fixed (LayoutState* state = &m_State)
		{
			void* ptr = state;
			IntPtr state2 = (IntPtr)ptr;
			LayoutNative.CalculateLayout(node2, parentWidth, parentHeight, (int)parentDirection, state2, (IntPtr)(&zero));
			if (zero != IntPtr.Zero)
			{
				GCHandle gCHandle = GCHandle.FromIntPtr(zero);
				Exception source = gCHandle.Target as Exception;
				gCHandle.Free();
				m_State.error = false;
				ExceptionDispatchInfo exceptionDispatchInfo = ExceptionDispatchInfo.Capture(source);
				exceptionDispatchInfo.Throw();
			}
		}
	}
}
