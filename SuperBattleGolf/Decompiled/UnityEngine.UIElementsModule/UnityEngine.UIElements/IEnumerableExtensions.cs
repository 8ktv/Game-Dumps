using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.Bindings;

namespace UnityEngine.UIElements;

[VisibleToOtherModules(new string[] { "UnityEditor.UIBuilderModule" })]
internal static class IEnumerableExtensions
{
	internal static bool HasValues(this IEnumerable<string> collection)
	{
		if (collection == null)
		{
			return false;
		}
		using (IEnumerator<string> enumerator = collection.GetEnumerator())
		{
			if (enumerator.MoveNext())
			{
				string current = enumerator.Current;
				return true;
			}
		}
		return false;
	}

	internal static bool NoElementOfTypeMatchesPredicate<T>(this IEnumerable collection, Func<T, bool> predicate)
	{
		foreach (object item in collection)
		{
			if (item is T arg && predicate(arg))
			{
				return false;
			}
		}
		return true;
	}

	[VisibleToOtherModules(new string[] { "UnityEditor.UIBuilderModule" })]
	internal static int GetCount(this IEnumerable collection)
	{
		int num = 0;
		foreach (object item in collection)
		{
			num++;
		}
		return num;
	}

	[VisibleToOtherModules(new string[] { "UnityEditor.UIBuilderModule" })]
	internal static bool Any<T>(this List<T> source, Func<T, bool> predicate)
	{
		foreach (T item in source)
		{
			if (predicate(item))
			{
				return true;
			}
		}
		return false;
	}

	[VisibleToOtherModules(new string[] { "UnityEditor.UIBuilderModule" })]
	internal static HashSet<TResult> UniqueSelect<TSource, TResult>(this IEnumerable<TSource> source, Func<TSource, TResult> selector)
	{
		HashSet<TResult> hashSet = new HashSet<TResult>();
		foreach (TSource item in source)
		{
			TResult val = selector(item);
			if (val != null)
			{
				hashSet.Add(selector(item));
			}
		}
		return hashSet;
	}
}
