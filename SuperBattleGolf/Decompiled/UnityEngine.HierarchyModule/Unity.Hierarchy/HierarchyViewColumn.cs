using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

namespace Unity.Hierarchy;

internal sealed class HierarchyViewColumn : Column
{
	internal const string k_NonDefaultValue = "non-default-value";

	private readonly HierarchyView m_View;

	private readonly List<HierarchyViewCellDescriptor> m_CellDescriptors = new List<HierarchyViewCellDescriptor>();

	public readonly HierarchyViewColumnDescriptor Descriptor;

	internal HierarchyView View => m_View;

	public IReadOnlyCollection<HierarchyViewCellDescriptor> CellDescriptors => m_CellDescriptors;

	public HierarchyViewColumn(HierarchyView view, HierarchyViewColumnDescriptor descriptor)
	{
		m_View = view;
		Descriptor = descriptor;
		base.name = Descriptor.Id;
		base.resizable = true;
		base.stretchable = false;
		base.sortable = false;
		base.title = Descriptor.Title;
		if (Descriptor.MakeHeader != null)
		{
			base.makeHeader = (Func<VisualElement>)Delegate.Combine(base.makeHeader, new Func<VisualElement>(MakeHeader));
			base.bindHeader = (Action<VisualElement>)Delegate.Combine(base.bindHeader, new Action<VisualElement>(BindHeader));
			base.unbindHeader = (Action<VisualElement>)Delegate.Combine(base.unbindHeader, new Action<VisualElement>(UnbindHeader));
			base.destroyHeader = (Action<VisualElement>)Delegate.Combine(base.destroyHeader, new Action<VisualElement>(DestroyHeader));
		}
		else if ((bool)Descriptor.Icon)
		{
			base.icon = Background.FromTexture2D(Descriptor.Icon);
		}
		if (descriptor.DefaultWidth > 0)
		{
			base.width = descriptor.DefaultWidth;
		}
		base.makeCell = (Func<VisualElement>)Delegate.Combine(base.makeCell, new Func<VisualElement>(MakeCell));
		base.bindCell = (Action<VisualElement, int>)Delegate.Combine(base.bindCell, new Action<VisualElement, int>(BindCell));
		base.unbindCell = (Action<VisualElement, int>)Delegate.Combine(base.unbindCell, new Action<VisualElement, int>(UnbindCell));
	}

	public void AddCell(HierarchyViewCellDescriptor desc)
	{
		if (!desc.ValidForColumn(Descriptor))
		{
			Debug.LogError("Cannot register Cell: " + desc.ColumnId + " with Column: " + Descriptor.Id);
			return;
		}
		foreach (HierarchyViewCellDescriptor cellDescriptor in CellDescriptors)
		{
			if (cellDescriptor.HandlerType == desc.HandlerType)
			{
				Debug.LogError($"Cell: for NodeType {desc.HandlerType} is already registered.");
				return;
			}
		}
		m_CellDescriptors.Add(desc);
	}

	internal void ApplyDefaultColumnProperties()
	{
		if (Descriptor.DefaultWidth > 0)
		{
			SetWidth(this, Descriptor.DefaultWidth);
		}
		base.visible = Descriptor.DefaultVisibility;
	}

	internal static void SetWidth(Column col, float newWidth)
	{
		if (!(newWidth <= 0f))
		{
			col.width = newWidth;
			if (col.minWidth.value > newWidth)
			{
				col.minWidth = newWidth;
			}
		}
	}

	private VisualElement MakeHeader()
	{
		return Descriptor.MakeHeader();
	}

	private void BindHeader(VisualElement header)
	{
		Descriptor?.BindHeader(header, m_View);
	}

	private void UnbindHeader(VisualElement header)
	{
		Descriptor?.UnbindHeader(header, m_View);
	}

	private void DestroyHeader(VisualElement header)
	{
		Descriptor?.DestroyHeader(header, m_View);
	}

	private VisualElement MakeCell()
	{
		return new HierarchyViewCell(m_View, this);
	}

	internal void BindColumn(HierarchyView view)
	{
		Descriptor.InvokeBindColumn(this, view);
		foreach (HierarchyViewCellDescriptor cellDescriptor in CellDescriptors)
		{
			cellDescriptor.InvokeBindColumn(Descriptor, view);
		}
	}

	internal void UnbindColumn(HierarchyView view)
	{
		foreach (HierarchyViewCellDescriptor cellDescriptor in CellDescriptors)
		{
			cellDescriptor.InvokeUnbindColumn(Descriptor, view);
		}
		if (Descriptor.UnbindHeader != null || Descriptor.DestroyHeader != null)
		{
			List<VisualElement> list = view.ListView.Query<VisualElement>(null, "unity-multi-column-header__column").ToList();
			foreach (VisualElement item in list)
			{
				if (item.name == Descriptor.Id)
				{
					VisualElement arg = item.Q(null, "unity-multi-column-header__column__content");
					Descriptor.UnbindHeader?.Invoke(arg, view);
					Descriptor.DestroyHeader?.Invoke(arg, view);
					break;
				}
			}
		}
		Descriptor.InvokeUnbindColumn(this, view);
	}

	private void BindCell(VisualElement cellElement, int index)
	{
		if (!(cellElement is HierarchyViewCell hierarchyViewCell))
		{
			return;
		}
		HierarchyNode lhs = m_View.ViewModel[index];
		if (lhs == HierarchyNode.Null || !m_View.Source.Exists(in lhs))
		{
			return;
		}
		hierarchyViewCell.Node = lhs;
		hierarchyViewCell.NodeIndex = index;
		hierarchyViewCell.Handler = m_View.Source.GetNodeTypeHandler(in lhs);
		if (hierarchyViewCell.Handler == null)
		{
			return;
		}
		foreach (HierarchyViewCellDescriptor cellDescriptor in CellDescriptors)
		{
			if (cellDescriptor.HandlerType == null || cellDescriptor.HandlerType == hierarchyViewCell.Handler.GetType())
			{
				hierarchyViewCell.Descriptor = cellDescriptor;
				break;
			}
		}
		if (hierarchyViewCell.Descriptor != null)
		{
			hierarchyViewCell.BindCell();
		}
	}

	private void UnbindCell(VisualElement cellElement, int index)
	{
		if (cellElement is HierarchyViewCell hierarchyViewCell)
		{
			hierarchyViewCell.UnbindCell();
		}
	}

	public override string ToString()
	{
		return Descriptor.ToString();
	}
}
