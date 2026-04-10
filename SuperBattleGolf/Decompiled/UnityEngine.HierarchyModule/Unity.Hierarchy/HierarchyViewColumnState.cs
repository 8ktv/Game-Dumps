using System;

namespace Unity.Hierarchy;

[Serializable]
internal sealed class HierarchyViewColumnState
{
	public string ColumnId;

	public bool Visible;

	public float Width;

	public int Index = -1;

	public override string ToString()
	{
		return $"{ColumnId} Visible:{Visible} Index:{Index} Width:{Width}";
	}
}
