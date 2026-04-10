using System;
using UnityEngine.Bindings;

namespace Unity.Hierarchy;

[VisibleToOtherModules(new string[] { "UnityEditor.UIToolkitAuthoringModule" })]
internal sealed class HierarchyViewCellDescriptor
{
	private bool m_IsColumnBound;

	public readonly string ColumnId;

	public readonly Type HandlerType;

	public Action<HierarchyViewCell> BindCell;

	public Action<HierarchyViewCell> UnbindCell;

	public Action<HierarchyViewColumnDescriptor, HierarchyView> BindColumn;

	public Action<HierarchyViewColumnDescriptor, HierarchyView> UnbindColumn;

	public bool ClearCellContent;

	public object UserData;

	public HierarchyViewCellDescriptor(string columnId, Type handlerType = null)
	{
		ColumnId = columnId;
		HandlerType = handlerType;
		ClearCellContent = true;
	}

	internal void InvokeBindColumn(HierarchyViewColumnDescriptor descriptor, HierarchyView view)
	{
		if (!m_IsColumnBound)
		{
			BindColumn?.Invoke(descriptor, view);
			m_IsColumnBound = true;
		}
	}

	internal void InvokeUnbindColumn(HierarchyViewColumnDescriptor descriptor, HierarchyView view)
	{
		if (m_IsColumnBound)
		{
			UnbindColumn?.Invoke(descriptor, view);
			m_IsColumnBound = false;
		}
	}

	public bool ValidForColumn(HierarchyViewColumnDescriptor colDesc)
	{
		return ColumnId == colDesc.Id;
	}

	public override string ToString()
	{
		string text = ((HandlerType != null) ? HandlerType.Name : "<GenericNodeHandler>");
		return ColumnId + " - " + text;
	}
}
