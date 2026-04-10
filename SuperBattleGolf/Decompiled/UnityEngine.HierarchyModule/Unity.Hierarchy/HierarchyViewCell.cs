using UnityEngine.Bindings;
using UnityEngine.UIElements;

namespace Unity.Hierarchy;

[VisibleToOtherModules(new string[] { "UnityEditor.UIToolkitAuthoringModule" })]
internal sealed class HierarchyViewCell : VisualElement
{
	private bool m_IsCellBound;

	private bool m_IsDefaultValue;

	public readonly HierarchyViewColumn Column;

	public readonly HierarchyView View;

	public HierarchyNodeTypeHandler Handler { get; internal set; }

	public HierarchyNode Node { get; internal set; }

	public int NodeIndex { get; internal set; }

	public HierarchyViewCellDescriptor Descriptor { get; internal set; }

	public object BoundObject { get; set; }

	public bool IsDefaultValue
	{
		get
		{
			return m_IsDefaultValue;
		}
		set
		{
			if (value)
			{
				RemoveFromClassList("non-default-value");
			}
			else
			{
				AddToClassList("non-default-value");
			}
			m_IsDefaultValue = value;
		}
	}

	internal void BindCell()
	{
		if (!m_IsCellBound)
		{
			Descriptor?.BindCell?.Invoke(this);
			m_IsCellBound = true;
		}
	}

	internal void UnbindCell()
	{
		if (!m_IsCellBound)
		{
			return;
		}
		if (Descriptor != null)
		{
			Descriptor?.UnbindCell?.Invoke(this);
			if (Descriptor.ClearCellContent)
			{
				Clear();
			}
		}
		BoundObject = null;
		Node = HierarchyNode.Null;
		NodeIndex = -1;
		Handler = null;
		Descriptor = null;
		m_IsCellBound = false;
	}

	internal HierarchyViewCell(HierarchyView view, HierarchyViewColumn column)
	{
		View = view;
		Column = column;
		base.name = "HierarchyViewCell";
	}

	public override string ToString()
	{
		string id = Column.Descriptor.Id;
		if (Descriptor != null)
		{
			return Descriptor.ToString();
		}
		return $"{Column.Descriptor} - NoCellDesc";
	}
}
