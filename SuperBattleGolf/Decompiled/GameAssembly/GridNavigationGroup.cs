using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Pool;
using UnityEngine.UI;

public class GridNavigationGroup : MonoBehaviour
{
	[Serializable]
	public class Row
	{
		public List<Element> elements;

		public int index;

		public static Row CreateBlankRow()
		{
			return new Row
			{
				elements = new List<Element>()
			};
		}
	}

	[Serializable]
	public class Element
	{
		[NonSerialized]
		public Row row;

		public int index;

		public Selectable selectable;

		public float horizontalPosition;
	}

	public List<Row> rows = new List<Row>();

	public void InsertRow(int index, Row row)
	{
		if (index < 0 || index > rows.Count)
		{
			Debug.LogError("Attempted to insert grid navigation row outside of range", base.gameObject);
		}
		else
		{
			rows.Insert(index, row);
		}
	}

	public void InsertRowsFrom(int index, GridLayoutGroup gridLayout)
	{
		if (index < 0 || index > rows.Count)
		{
			Debug.LogError("Attempted to insert grid navigation row outside of range", base.gameObject);
			return;
		}
		Selectable[] componentsInChildren = gridLayout.GetComponentsInChildren<Selectable>(includeInactive: true);
		if (componentsInChildren.Length == 0)
		{
			return;
		}
		List<Row> value;
		using (CollectionPool<List<Row>, Row>.Get(out value))
		{
			Selectable[] array = componentsInChildren;
			foreach (Selectable selectable in array)
			{
				Vector3 position = selectable.transform.position;
				Row row = null;
				foreach (Row item2 in value)
				{
					if (position.y.Approximately(item2.elements[0].selectable.transform.position.y, 1f))
					{
						row = item2;
						break;
					}
				}
				if (row == null)
				{
					row = Row.CreateBlankRow();
					bool flag = false;
					for (int j = 0; j < value.Count; j++)
					{
						if (position.y > value[j].elements[0].selectable.transform.position.y - 1f)
						{
							value.Insert(j, row);
							flag = true;
						}
					}
					if (!flag)
					{
						value.Add(row);
					}
				}
				Element item = new Element
				{
					selectable = selectable
				};
				bool flag2 = false;
				for (int k = 0; k < row.elements.Count; k++)
				{
					if (position.x < row.elements[k].selectable.transform.position.x - 1f)
					{
						row.elements.Insert(k, item);
						flag2 = true;
					}
				}
				if (!flag2)
				{
					row.elements.Add(item);
				}
			}
			rows.InsertRange(index, value);
		}
	}

	public void UpdateNavigation()
	{
		float num = 0f;
		for (int i = 0; i < rows.Count; i++)
		{
			Row row = rows[i];
			row.index = i;
			num = BMath.Max(num, row.elements.Count);
		}
		float num2 = 1f / num;
		foreach (Row row3 in rows)
		{
			float num3 = (float)row3.elements.Count * num2;
			float num4 = 0.5f - num3 / 2f;
			for (int j = 0; j < row3.elements.Count; j++)
			{
				Element element = row3.elements[j];
				element.row = row3;
				element.index = j;
				element.horizontalPosition = num4 + (float)j * num2;
			}
		}
		for (int k = 0; k < rows.Count; k++)
		{
			Row row2 = rows[k];
			for (int l = 0; l < row2.elements.Count; l++)
			{
				Element element2 = row2.elements[l];
				if (!(element2.selectable == null))
				{
					Navigation navigation = element2.selectable.navigation;
					navigation.mode = Navigation.Mode.Explicit;
					navigation.selectOnUp = FindNavigationUpTarget(element2);
					navigation.selectOnDown = FindNavigationDownTarget(element2);
					navigation.selectOnLeft = FindNavigationLeftTarget(element2);
					navigation.selectOnRight = FindNavigationRightTarget(element2);
					element2.selectable.navigation = navigation;
				}
			}
		}
		Selectable FindNavigationDownTarget(Element element3)
		{
			Row foundRow = element3.row;
			while (TryFindRowBelow(foundRow, out foundRow))
			{
				if (TryFindNearestElementInRow(element3.horizontalPosition, foundRow, out var element4))
				{
					return element4.selectable;
				}
			}
			return null;
		}
		Selectable FindNavigationLeftTarget(Element element3)
		{
			if (TryFindElementToLeftOfInRow(element3.horizontalPosition, element3.row, out var element4))
			{
				return element4.selectable;
			}
			if (TryFindRowAbove(element3.row, out var foundRow) && TryFindElementToLeftOfInRow(element3.horizontalPosition, foundRow, out element4))
			{
				return element4.selectable;
			}
			if (TryFindRowBelow(element3.row, out foundRow) && TryFindElementToLeftOfInRow(element3.horizontalPosition, foundRow, out element4))
			{
				return element4.selectable;
			}
			return null;
		}
		Selectable FindNavigationRightTarget(Element element3)
		{
			if (TryFindElementToRightOfInRow(element3.horizontalPosition, element3.row, out var element4))
			{
				return element4.selectable;
			}
			if (TryFindRowAbove(element3.row, out var foundRow) && TryFindElementToRightOfInRow(element3.horizontalPosition, foundRow, out element4))
			{
				return element4.selectable;
			}
			if (TryFindRowBelow(element3.row, out foundRow) && TryFindElementToRightOfInRow(element3.horizontalPosition, foundRow, out element4))
			{
				return element4.selectable;
			}
			return null;
		}
		Selectable FindNavigationUpTarget(Element element3)
		{
			Row foundRow = element3.row;
			while (TryFindRowAbove(foundRow, out foundRow))
			{
				if (TryFindNearestElementInRow(element3.horizontalPosition, foundRow, out var element4))
				{
					return element4.selectable;
				}
			}
			return null;
		}
		static bool TryFindElementToLeftOfInRow(float horizontalPosition, Row row3, out Element reference)
		{
			reference = null;
			for (int num5 = row3.elements.Count - 1; num5 >= 0; num5--)
			{
				Element element3 = row3.elements[num5];
				if (!(element3.selectable == null) && !(element3.horizontalPosition >= horizontalPosition))
				{
					reference = element3;
					break;
				}
			}
			return reference != null;
		}
		static bool TryFindElementToRightOfInRow(float horizontalPosition, Row row3, out Element reference)
		{
			reference = null;
			for (int m = 0; m < row3.elements.Count; m++)
			{
				Element element3 = row3.elements[m];
				if (!(element3.selectable == null) && !(element3.horizontalPosition <= horizontalPosition))
				{
					reference = element3;
					break;
				}
			}
			return reference != null;
		}
		static bool TryFindNearestElementInRow(float horizontalPosition, Row row3, out Element reference)
		{
			reference = null;
			float num5 = float.MaxValue;
			for (int m = 0; m < row3.elements.Count; m++)
			{
				Element element3 = row3.elements[m];
				if (element3 != null)
				{
					float num6 = BMath.Abs(element3.horizontalPosition - horizontalPosition);
					if (!(num6 >= num5 - 0.0001f))
					{
						num5 = num6;
						reference = element3;
					}
				}
			}
			return reference != null;
		}
		bool TryFindRowAbove(Row row3, out Row foundRow)
		{
			int num5 = row3.index - 1;
			foundRow = ((num5 >= 0) ? rows[num5] : null);
			return foundRow != null;
		}
		bool TryFindRowBelow(Row row3, out Row foundRow)
		{
			int num5 = row3.index + 1;
			foundRow = ((num5 < rows.Count) ? rows[num5] : null);
			return foundRow != null;
		}
	}
}
