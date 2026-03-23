using System;

namespace Microsoft.Win32.SafeHandles;

public sealed class SafePipeHandle : SafeHandleZeroOrMinusOneIsInvalid
{
	private const int DefaultInvalidHandle = 0;

	protected override bool ReleaseHandle()
	{
		return global::Interop.Kernel32.CloseHandle(handle);
	}

	internal SafePipeHandle()
		: this(new IntPtr(0), ownsHandle: true)
	{
	}

	public SafePipeHandle(IntPtr preexistingHandle, bool ownsHandle)
		: base(ownsHandle)
	{
		SetHandle(preexistingHandle);
	}

	internal void SetHandle(int descriptor)
	{
		SetHandle((IntPtr)descriptor);
	}
}
