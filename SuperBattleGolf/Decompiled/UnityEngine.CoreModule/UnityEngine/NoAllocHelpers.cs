using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Unity.Collections.LowLevel.Unsafe;
using UnityEngine.Bindings;
using UnityEngine.Scripting;

namespace UnityEngine;

[VisibleToOtherModules]
internal static class NoAllocHelpers
{
	private class ListPrivateFieldAccess<T>
	{
		internal T[] _items;

		internal int _size;

		internal int _version;
	}

	public static void EnsureListElemCount<T>(List<T> list, int count)
	{
		if (list == null)
		{
			throw new ArgumentNullException("list");
		}
		if (count < 0)
		{
			throw new ArgumentException("invalid size to resize.", "list");
		}
		list.Clear();
		if (list.Capacity < count)
		{
			list.Capacity = count;
		}
		if (count != list.Count)
		{
			ListPrivateFieldAccess<T> listPrivateFieldAccess = UnsafeUtility.As<List<T>, ListPrivateFieldAccess<T>>(ref list);
			listPrivateFieldAccess._size = count;
			listPrivateFieldAccess._version++;
		}
	}

	public static int SafeLength(Array values)
	{
		return values?.Length ?? 0;
	}

	public static int SafeLength<T>(List<T> values)
	{
		return values?.Count ?? 0;
	}

	[Obsolete("Use ExtractArrayFromList", false)]
	public static T[] ExtractArrayFromListT<T>(List<T> list)
	{
		return ExtractArrayFromList(list);
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static T[] ExtractArrayFromList<T>(List<T> list)
	{
		if (list == null)
		{
			return null;
		}
		ListPrivateFieldAccess<T> listPrivateFieldAccess = UnsafeUtility.As<ListPrivateFieldAccess<T>>(list);
		return listPrivateFieldAccess._items;
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static Span<T> CreateSpan<T>(List<T> list)
	{
		if (list == null)
		{
			return default(Span<T>);
		}
		ListPrivateFieldAccess<T> listPrivateFieldAccess = UnsafeUtility.As<ListPrivateFieldAccess<T>>(list);
		return new Span<T>(listPrivateFieldAccess._items, 0, listPrivateFieldAccess._size);
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static ReadOnlySpan<T> CreateReadOnlySpan<T>(List<T> list)
	{
		if (list == null)
		{
			return default(ReadOnlySpan<T>);
		}
		ListPrivateFieldAccess<T> listPrivateFieldAccess = UnsafeUtility.As<ListPrivateFieldAccess<T>>(list);
		return new ReadOnlySpan<T>(listPrivateFieldAccess._items, 0, listPrivateFieldAccess._size);
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static void ResetListContents<T>(List<T> list, ReadOnlySpan<T> span)
	{
		ListPrivateFieldAccess<T> listPrivateFieldAccess = UnsafeUtility.As<ListPrivateFieldAccess<T>>(list);
		listPrivateFieldAccess._items = span.ToArray();
		listPrivateFieldAccess._size = span.Length;
		listPrivateFieldAccess._version++;
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static void ResetListContents<T>(List<T> list, T[] array)
	{
		ListPrivateFieldAccess<T> listPrivateFieldAccess = UnsafeUtility.As<ListPrivateFieldAccess<T>>(list);
		listPrivateFieldAccess._items = array;
		listPrivateFieldAccess._size = array.Length;
		listPrivateFieldAccess._version++;
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static void ResetListSize<T>(List<T> list, int size)
	{
		if (list.Capacity < size)
		{
			throw new ArgumentException($"Resetting to {size} which is bigger than capacity {list.Capacity} is not allowed!");
		}
		ListPrivateFieldAccess<T> listPrivateFieldAccess = UnsafeUtility.As<ListPrivateFieldAccess<T>>(list);
		if (RuntimeHelpers.IsReferenceOrContainsReferences<T>() && listPrivateFieldAccess._size > size)
		{
			Array.Clear(listPrivateFieldAccess._items, size, listPrivateFieldAccess._size - size);
		}
		listPrivateFieldAccess._size = size;
		listPrivateFieldAccess._version++;
	}

	[RequiredByNativeCode]
	private static Array PrepareListForNativeFill(object list, Type elementType, int newSize)
	{
		ListPrivateFieldAccess<byte> listPrivateFieldAccess = UnsafeUtility.As<ListPrivateFieldAccess<byte>>(list);
		ref byte[] items = ref listPrivateFieldAccess._items;
		int num = items.Length;
		int size = listPrivateFieldAccess._size;
		if (num < newSize)
		{
			items = UnsafeUtility.As<byte[]>(Array.CreateInstance(elementType, newSize));
		}
		else if (size > newSize)
		{
			Array.Clear(items, newSize, size - newSize);
		}
		listPrivateFieldAccess._size = newSize;
		listPrivateFieldAccess._version++;
		return items;
	}
}
