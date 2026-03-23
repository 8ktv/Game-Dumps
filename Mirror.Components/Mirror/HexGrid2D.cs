using System;
using UnityEngine;

namespace Mirror;

internal class HexGrid2D
{
	internal float cellRadius;

	private Vector2 originOffset;

	private readonly float sqrt3Div3;

	private readonly float oneDiv3;

	private readonly float twoDiv3;

	private readonly float sqrt3;

	private readonly float sqrt3Div2;

	private static readonly Cell2D[] neighborCellsBase = new Cell2D[7]
	{
		new Cell2D(0, 0),
		new Cell2D(1, -1),
		new Cell2D(1, 0),
		new Cell2D(0, 1),
		new Cell2D(-1, 1),
		new Cell2D(-1, 0),
		new Cell2D(0, -1)
	};

	internal HexGrid2D(ushort visRange)
	{
		cellRadius = (float)(int)visRange / 2f;
		originOffset = Vector2.zero;
		sqrt3Div3 = Mathf.Sqrt(3f) / 3f;
		oneDiv3 = 1f / 3f;
		twoDiv3 = 2f / 3f;
		sqrt3 = Mathf.Sqrt(3f);
		sqrt3Div2 = Mathf.Sqrt(3f) / 2f;
	}

	internal Vector2 CellToWorld(Cell2D cell)
	{
		float x = cellRadius * (sqrt3 * (float)cell.q + sqrt3Div2 * (float)cell.r);
		float y = cellRadius * (1.5f * (float)cell.r);
		return new Vector2(x, y) - originOffset;
	}

	internal Cell2D WorldToCell(Vector2 position)
	{
		position += originOffset;
		float q = (sqrt3Div3 * position.x - oneDiv3 * position.y) / cellRadius;
		float r = twoDiv3 * position.y / cellRadius;
		return RoundToCell(q, r);
	}

	private Cell2D RoundToCell(float q, float r)
	{
		float num = 0f - q - r;
		int num2 = Mathf.RoundToInt(q);
		int num3 = Mathf.RoundToInt(r);
		int num4 = Mathf.RoundToInt(num);
		float num5 = Mathf.Abs(q - (float)num2);
		float num6 = Mathf.Abs(r - (float)num3);
		float num7 = Mathf.Abs(num - (float)num4);
		if (num5 > num6 && num5 > num7)
		{
			num2 = -num3 - num4;
		}
		else if (num6 > num7)
		{
			num3 = -num2 - num4;
		}
		return new Cell2D(num2, num3);
	}

	internal void GetNeighborCells(Cell2D center, Cell2D[] neighbors)
	{
		if (neighbors.Length != 7)
		{
			throw new ArgumentException("Neighbor array must have exactly 7 elements");
		}
		for (int i = 0; i < neighborCellsBase.Length; i++)
		{
			neighbors[i] = new Cell2D(center.q + neighborCellsBase[i].q, center.r + neighborCellsBase[i].r);
		}
	}
}
