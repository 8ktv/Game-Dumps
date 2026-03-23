using System;
using System.Runtime.InteropServices;

namespace UnityEngine.UIElements.Experimental;

[StructLayout(LayoutKind.Sequential, Size = 1)]
internal struct EventDebuggerLogIMGUICall : IDisposable
{
	public EventDebuggerLogIMGUICall(EventBase evt)
	{
	}

	public void Dispose()
	{
	}
}
