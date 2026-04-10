using System;
using System.Runtime.InteropServices;
using Unity.Collections.LowLevel.Unsafe;
using Unity.Profiling.LowLevel.Unsafe;
using UnityEngine.Scripting;

namespace Unity.Collections;

[StructLayout(LayoutKind.Sequential, Size = 16)]
public readonly struct MemoryLabel
{
	[NativeDisableUnsafePtrRestriction]
	internal readonly IntPtr pointer;

	internal readonly Allocator allocator;

	internal long RelatedMemorySize => ProfilerUnsafeUtility.GetMemLabelRelatedMemorySize(pointer);

	public bool IsCreated => allocator != Allocator.Invalid;

	public MemoryLabel(string areaName, string objectName, Allocator allocator = Allocator.Persistent)
	{
		if (IsNullOrEmpty(areaName))
		{
			throw new ArgumentNullException("areaName");
		}
		if (IsNullOrEmpty(objectName))
		{
			throw new ArgumentNullException("objectName");
		}
		if (!SupportsAllocator(allocator))
		{
			throw new ArgumentException("Only Allocator.Persistent and Allocator.Domain support allocating with a label");
		}
		this.allocator = allocator;
		pointer = ProfilerUnsafeUtility.GetOrCreateMemLabel(areaName, objectName);
	}

	public static bool SupportsAllocator(Allocator allocator)
	{
		return allocator == Allocator.Persistent || allocator == Allocator.Domain;
	}

	private static bool IsNullOrEmpty(string str)
	{
		return string.IsNullOrEmpty(str);
	}

	[RequiredMember]
	private unsafe static bool IsNullOrEmpty__Unmanaged(byte* name, int nameLen)
	{
		return name == null || nameLen <= 0;
	}

	internal void CheckArgument()
	{
		if (!IsCreated)
		{
			throw new ArgumentException("MemoryLabel has not been created. Use the constructor to create it.");
		}
	}
}
