using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Net;
using System.Runtime.CompilerServices;

namespace Mirror;

public static class Extensions
{
	public static string ToHexString(this ArraySegment<byte> segment)
	{
		return BitConverter.ToString(segment.Array, segment.Offset, segment.Count);
	}

	public static int GetStableHashCode(this string text)
	{
		uint num = 2166136261u;
		uint num2 = 16777619u;
		for (int i = 0; i < text.Length; i++)
		{
			byte b = (byte)text[i];
			num ^= b;
			num *= num2;
		}
		return (int)num;
	}

	public static ushort GetStableHashCode16(this string text)
	{
		int stableHashCode = text.GetStableHashCode();
		return (ushort)((stableHashCode >> 16) ^ stableHashCode);
	}

	public static string GetMethodName(this Delegate func)
	{
		return func.Method.Name;
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static void CopyTo<T>(this IEnumerable<T> source, List<T> destination)
	{
		destination.AddRange(source);
	}

	public static bool TryDequeue<T>(this Queue<T> source, out T element)
	{
		if (source.Count > 0)
		{
			element = source.Dequeue();
			return true;
		}
		element = default(T);
		return false;
	}

	public static void Clear<T>(this ConcurrentQueue<T> source)
	{
		int count = source.Count;
		for (int i = 0; i < count; i++)
		{
			source.TryDequeue(out var _);
		}
	}

	public static string PrettyAddress(this IPEndPoint endPoint)
	{
		if (endPoint == null)
		{
			return "";
		}
		if (!endPoint.Address.IsIPv4MappedToIPv6)
		{
			return endPoint.Address.ToString();
		}
		return endPoint.Address.MapToIPv4().ToString();
	}
}
