using System;
using System.Runtime.InteropServices;

namespace UnityEngine.UIElements.Experimental;

[StructLayout(LayoutKind.Sequential, Size = 1)]
internal struct EventDebuggerLogExecuteDefaultAction : IDisposable
{
	public EventDebuggerLogExecuteDefaultAction(EventBase evt)
	{
	}

	public void Dispose()
	{
	}
}
