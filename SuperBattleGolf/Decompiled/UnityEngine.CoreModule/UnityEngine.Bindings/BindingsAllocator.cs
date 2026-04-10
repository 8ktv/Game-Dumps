using System.Runtime.CompilerServices;

namespace UnityEngine.Bindings;

[StaticAccessor("Marshalling::BindingsAllocator", StaticAccessorType.DoubleColon)]
[VisibleToOtherModules]
[NativeHeader("Runtime/Scripting/Marshalling/BindingsAllocator.h")]
internal static class BindingsAllocator
{
	private struct NativeOwnedMemory
	{
		public unsafe void* data;
	}

	[MethodImpl(MethodImplOptions.InternalCall)]
	[ThreadSafe]
	public unsafe static extern void* Malloc(int size);

	[MethodImpl(MethodImplOptions.InternalCall)]
	[ThreadSafe]
	public unsafe static extern void Free(void* ptr);

	[MethodImpl(MethodImplOptions.InternalCall)]
	[ThreadSafe]
	public unsafe static extern void FreeNativeOwnedMemory(void* ptr);

	public unsafe static void* GetNativeOwnedDataPointer(void* ptr)
	{
		return ((NativeOwnedMemory*)ptr)->data;
	}
}
