using System;
using UnityEngine;
using UnityEngine.UIElements;

namespace Unity.Hierarchy;

internal sealed class HierarchyViewColumnDescriptor
{
	private bool m_IsBound;

	public readonly string Id;

	public string Title;

	public Texture2D Icon;

	public string Tooltip;

	public int DefaultPriority;

	public int DefaultWidth = -1;

	public bool DefaultVisibility;

	public object UserData;

	public Func<VisualElement> MakeHeader;

	public Action<VisualElement, HierarchyView> BindHeader;

	public Action<VisualElement, HierarchyView> UnbindHeader;

	public Action<VisualElement, HierarchyView> DestroyHeader;

	public Action<HierarchyViewColumn, HierarchyView> BindColumn;

	public Action<HierarchyViewColumn, HierarchyView> UnbindColumn;

	internal void InvokeBindColumn(HierarchyViewColumn column, HierarchyView view)
	{
		if (!m_IsBound)
		{
			BindColumn?.Invoke(column, view);
			m_IsBound = true;
		}
	}

	internal void InvokeUnbindColumn(HierarchyViewColumn column, HierarchyView view)
	{
		if (m_IsBound)
		{
			UnbindColumn?.Invoke(column, view);
			m_IsBound = false;
		}
	}

	public HierarchyViewColumnDescriptor(string columnId)
	{
		Id = columnId;
	}

	public override string ToString()
	{
		return Id;
	}
}
