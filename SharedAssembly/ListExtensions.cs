using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;

public static class ListExtensions
{
	public static void RemoveAtSwapBack<T>(this List<T> list, int index)
	{
		list[index] = list[list.Count - 1];
		list.RemoveAt(list.Count - 1);
	}

	public static bool RemoveLast<T>(this List<T> list, T item)
	{
		int num = list.LastIndexOf(item);
		if (num >= 0)
		{
			list.RemoveAt(num);
			return true;
		}
		return false;
	}

	public static T Random<T>(this List<T> list)
	{
		if (list.Count == 0)
		{
			return default(T);
		}
		return list[UnityEngine.Random.Range(0, list.Count)];
	}

	public static T Random<T>(this List<T> list, out int index)
	{
		if (list.Count == 0)
		{
			index = 0;
			return default(T);
		}
		index = UnityEngine.Random.Range(0, list.Count);
		return list[index];
	}

	public static T Random<T>(this List<T> list, ref Unity.Mathematics.Random random)
	{
		if (list.Count == 0)
		{
			return default(T);
		}
		return list[random.NextInt(0, list.Count)];
	}

	public static T Random<T>(this List<T> list, ref Unity.Mathematics.Random random, out int index)
	{
		if (list.Count == 0)
		{
			index = 0;
			return default(T);
		}
		index = random.NextInt(0, list.Count);
		return list[index];
	}

	public static void Shuffle<T>(this List<T> list, ref Unity.Mathematics.Random random)
	{
		int count = list.Count;
		for (int i = 0; i < count - 1; i++)
		{
			int index = i + random.NextInt(count - i);
			T value = list[index];
			list[index] = list[i];
			list[i] = value;
		}
	}

	public static void Shuffle<T>(this List<T> list)
	{
		int count = list.Count;
		for (int i = 0; i < count - 1; i++)
		{
			int index = i + UnityEngine.Random.Range(0, count - i);
			T value = list[index];
			list[index] = list[i];
			list[i] = value;
		}
	}
}
