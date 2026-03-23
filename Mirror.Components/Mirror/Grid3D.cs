using System.Collections.Generic;
using UnityEngine;

namespace Mirror;

public struct Grid3D<T>
{
	private readonly Dictionary<Vector3Int, HashSet<T>> grid;

	private readonly Vector3Int[] neighbourOffsets;

	public Grid3D(int initialCapacity)
	{
		grid = new Dictionary<Vector3Int, HashSet<T>>(initialCapacity);
		neighbourOffsets = new Vector3Int[27];
		int num = 0;
		for (int i = -1; i <= 1; i++)
		{
			for (int j = -1; j <= 1; j++)
			{
				for (int k = -1; k <= 1; k++)
				{
					neighbourOffsets[num] = new Vector3Int(i, j, k);
					num++;
				}
			}
		}
	}

	public void Add(Vector3Int position, T value)
	{
		if (!grid.TryGetValue(position, out var value2))
		{
			value2 = new HashSet<T>(128);
			grid[position] = value2;
		}
		value2.Add(value);
	}

	private void GetAt(Vector3Int position, HashSet<T> result)
	{
		if (!grid.TryGetValue(position, out var value))
		{
			return;
		}
		foreach (T item in value)
		{
			result.Add(item);
		}
	}

	public void GetWithNeighbours(Vector3Int position, HashSet<T> result)
	{
		result.Clear();
		Vector3Int[] array = neighbourOffsets;
		foreach (Vector3Int vector3Int in array)
		{
			GetAt(position + vector3Int, result);
		}
	}

	public void ClearNonAlloc()
	{
		foreach (HashSet<T> value in grid.Values)
		{
			value.Clear();
		}
	}
}
