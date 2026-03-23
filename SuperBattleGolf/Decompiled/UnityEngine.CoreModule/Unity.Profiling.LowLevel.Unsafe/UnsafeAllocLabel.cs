using System;
using Unity.Collections;

namespace Unity.Profiling.LowLevel.Unsafe;

internal readonly struct UnsafeAllocLabel
{
	internal readonly IntPtr pointer;

	internal readonly Allocator allocator;

	internal long RelatedMemorySize => ProfilerUnsafeUtility.GetMemLabelRelatedMemorySize(pointer);

	public bool Created => pointer != IntPtr.Zero;

	public UnsafeAllocLabel(string areaName, string objectName, Allocator allocator = Allocator.Persistent)
	{
		if (string.IsNullOrEmpty(areaName))
		{
			throw new ArgumentNullException("areaName");
		}
		if (string.IsNullOrEmpty(objectName))
		{
			throw new ArgumentNullException("objectName");
		}
		if (allocator != Allocator.Persistent && allocator != Allocator.Domain)
		{
			throw new ArgumentException("Only Allocator.Persistent and Allocator.Domain support allocating with a label");
		}
		this.allocator = allocator;
		pointer = ProfilerUnsafeUtility.GetOrCreateMemLabel(areaName, objectName);
	}
}
