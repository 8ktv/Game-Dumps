using System;
using UnityEngine;

namespace Mirror;

internal class HexGrid3D
{
	internal float cellRadius;

	internal float cellHeight;

	private Vector3 originOffset;

	private readonly float sqrt3Div3;

	private readonly float oneDiv3;

	private readonly float twoDiv3;

	private readonly float sqrt3;

	private readonly float sqrt3Div2;

	private static readonly Cell3D[] neighborCellsBase = new Cell3D[21]
	{
		new Cell3D(0, 0, 0),
		new Cell3D(0, 0, 1),
		new Cell3D(1, -1, 1),
		new Cell3D(1, 0, 1),
		new Cell3D(0, 1, 1),
		new Cell3D(-1, 1, 1),
		new Cell3D(-1, 0, 1),
		new Cell3D(0, -1, 1),
		new Cell3D(1, -1, 0),
		new Cell3D(1, 0, 0),
		new Cell3D(0, 1, 0),
		new Cell3D(-1, 1, 0),
		new Cell3D(-1, 0, 0),
		new Cell3D(0, -1, 0),
		new Cell3D(0, 0, -1),
		new Cell3D(1, -1, -1),
		new Cell3D(1, 0, -1),
		new Cell3D(0, 1, -1),
		new Cell3D(-1, 1, -1),
		new Cell3D(-1, 0, -1),
		new Cell3D(0, -1, -1)
	};

	internal HexGrid3D(ushort visRange, ushort height)
	{
		cellRadius = (float)(int)visRange / 2f;
		cellHeight = (int)height;
		originOffset = new Vector3(0f, (0f - cellHeight) / 2f, 0f);
		sqrt3Div3 = Mathf.Sqrt(3f) / 3f;
		oneDiv3 = 1f / 3f;
		twoDiv3 = 2f / 3f;
		sqrt3 = Mathf.Sqrt(3f);
		sqrt3Div2 = Mathf.Sqrt(3f) / 2f;
	}

	internal Vector3 CellToWorld(Cell3D cell)
	{
		float x = cellRadius * (sqrt3 * (float)cell.q + sqrt3Div2 * (float)cell.r);
		float z = cellRadius * (1.5f * (float)cell.r);
		float y = (float)cell.layer * cellHeight + cellHeight / 2f;
		return new Vector3(x, y, z) - originOffset;
	}

	internal Cell3D WorldToCell(Vector3 position)
	{
		position += originOffset;
		int layer = Mathf.FloorToInt(position.y / cellHeight);
		float q = (sqrt3Div3 * position.x - oneDiv3 * position.z) / cellRadius;
		float r = twoDiv3 * position.z / cellRadius;
		return RoundToCell(q, r, layer);
	}

	private Cell3D RoundToCell(float q, float r, int layer)
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
		return new Cell3D(num2, num3, layer);
	}

	internal void GetNeighborCells(Cell3D center, Cell3D[] neighbors)
	{
		if (neighbors.Length != 21)
		{
			throw new ArgumentException("Neighbor array must have exactly 21 elements");
		}
		for (int i = 0; i < neighborCellsBase.Length; i++)
		{
			neighbors[i] = new Cell3D(center.q + neighborCellsBase[i].q, center.r + neighborCellsBase[i].r, center.layer + neighborCellsBase[i].layer);
		}
	}
}
