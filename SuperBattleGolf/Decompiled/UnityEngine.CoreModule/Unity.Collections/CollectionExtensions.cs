using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using UnityEngine.Bindings;

namespace Unity.Collections;

[VisibleToOtherModules]
internal static class CollectionExtensions
{
	[VisibleToOtherModules(new string[] { "UnityEngine.UIElementsModule" })]
	internal static void AddToArray<T>(ref T[] array, T item)
	{
		Array.Resize(ref array, array.Length + 1);
		array[^1] = item;
	}

	[VisibleToOtherModules(new string[] { "UnityEngine.UIElementsModule" })]
	internal static void InsertIntoArray<T>(ref T[] array, int index, T item)
	{
		if (index < 0 || index > array.Length)
		{
			throw new IndexOutOfRangeException("Trying to insert into an array out of bounds.");
		}
		T[] array2 = array;
		array = new T[array2.Length + 1];
		Array.Copy(array2, array, index);
		Array.Copy(array2, index, array, index + 1, array2.Length - index);
		array[index] = item;
	}

	[VisibleToOtherModules(new string[] { "UnityEngine.UIElementsModule", "UnityEditor.UIBuilderModule" })]
	internal static bool RemoveFromArray<T>(ref T[] array, T item)
	{
		int num = Array.IndexOf(array, item);
		if (num == -1)
		{
			return false;
		}
		RemoveFromArray(ref array, num);
		return true;
	}

	[VisibleToOtherModules(new string[] { "UnityEngine.UIElementsModule", "UnityEditor.UIBuilderModule" })]
	internal static void RemoveFromArray<T>(ref T[] array, int index)
	{
		if (index < 0 || index >= array.Length)
		{
			throw new ArgumentOutOfRangeException("index");
		}
		int i = 0;
		int num = 0;
		for (; i < array.Length; i++)
		{
			if (i != index)
			{
				array[num++] = array[i];
			}
		}
		Array.Resize(ref array, array.Length - 1);
	}

	internal static void AddSorted<T>([DisallowNull] this List<T> list, T item, IComparer<T> comparer = null)
	{
		if (list == null)
		{
			throw new ArgumentNullException("list must not be null.");
		}
		if (comparer == null)
		{
			comparer = Comparer<T>.Default;
		}
		if (list.Count == 0)
		{
			list.Add(item);
			return;
		}
		if (comparer.Compare(list[list.Count - 1], item) <= 0)
		{
			list.Add(item);
			return;
		}
		if (comparer.Compare(list[0], item) >= 0)
		{
			list.Insert(0, item);
			return;
		}
		int num = list.BinarySearch(item, comparer);
		if (num < 0)
		{
			num = ~num;
		}
		list.Insert(num, item);
	}

	public static void Fill<T>([DisallowNull] this List<T> dest, T value, int count)
	{
		if (dest == null)
		{
			throw new ArgumentNullException("dest");
		}
		dest.Capacity = Math.Max(dest.Capacity, dest.Count + count);
		while (count-- > 0)
		{
			dest.Add(value);
		}
	}

	public static T FirstOrDefaultSorted<T>(this IEnumerable<T> collection, IComparer<T> comparer = null)
	{
		if (collection == null)
		{
			throw new ArgumentNullException("collection must not be null.");
		}
		if (comparer == null)
		{
			comparer = Comparer<T>.Default;
		}
		bool flag = false;
		T val = default(T);
		foreach (T item in collection)
		{
			if (!flag)
			{
				val = item;
				flag = true;
			}
			if (comparer.Compare(item, val) < 0)
			{
				val = item;
			}
		}
		return val;
	}

	internal static string SerializedView<T>([DisallowNull] this IEnumerable<T> collection, [DisallowNull] Func<T, string> serializeElement)
	{
		if (collection == null)
		{
			throw new ArgumentNullException("collection must not be null.");
		}
		if (serializeElement == null)
		{
			throw new ArgumentNullException("Argument serializeElement must not be null.");
		}
		return "[" + string.Join(",", collection.Select((T t) => (t == null) ? "null" : serializeElement(t))) + "]";
	}

	internal static bool ContainsByEquals<T>([DisallowNull] this IEnumerable<T> collection, T element)
	{
		if (collection == null)
		{
			throw new ArgumentNullException("collection must not be null.");
		}
		foreach (T item in collection)
		{
			if (item.Equals(element))
			{
				return true;
			}
		}
		return false;
	}
}
