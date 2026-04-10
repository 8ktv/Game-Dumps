using System;
using Unity.Scripting.LifecycleManagement;
using UnityEngine.Bindings;
using UnityEngine.Pool;
using UnityEngine.UIElements;

namespace Unity.Hierarchy;

[VisibleToOtherModules(new string[] { "UnityEditor.HierarchyModule" })]
internal class HierarchyViewItemColumn : Column
{
	internal const string k_HierarchyNameColumnName = "HierarchyViewColumn Name";

	[NoAutoStaticsCleanup]
	private static readonly BindingId k_ColumnStretchableProperty = "stretchable";

	private static readonly Length k_DefaultMinimumWidth = new Length(35f, LengthUnit.Pixel);

	private static readonly Length k_MinimumWidth = new Length(200f, LengthUnit.Pixel);

	private static readonly Length k_DefaultWidth = new Length(300f, LengthUnit.Pixel);

	private readonly HierarchyView m_View;

	private readonly UnityEngine.Pool.ObjectPool<HierarchyViewItemContainer> m_ViewItemContainerPool;

	public event Action<HierarchyViewItem> OnBindItem;

	public event Action<HierarchyViewItem> OnUnbindItem;

	public HierarchyViewItemColumn(HierarchyView view)
	{
		m_View = view;
		m_ViewItemContainerPool = new UnityEngine.Pool.ObjectPool<HierarchyViewItemContainer>(() => new HierarchyViewItemContainer());
		base.title = "Name";
		base.name = "HierarchyViewColumn Name";
		ApplyDefaultColumnProperties();
		base.makeCell = MakeCell;
		base.destroyCell = DestroyCell;
		base.bindCell = BindCell;
		base.unbindCell = UnbindCell;
		base.propertyChanged += delegate(object _, BindablePropertyChangedEventArgs args)
		{
			if (args.propertyName == k_ColumnStretchableProperty)
			{
				base.minWidth = (base.stretchable ? k_MinimumWidth : k_DefaultMinimumWidth);
				if (base.width.value < base.minWidth.value)
				{
					base.width = base.minWidth;
				}
			}
		};
	}

	private VisualElement MakeCell()
	{
		return m_ViewItemContainerPool.Get();
	}

	internal void ApplyDefaultColumnProperties()
	{
		base.width = k_DefaultWidth;
		base.minWidth = (base.stretchable ? k_MinimumWidth : k_DefaultMinimumWidth);
		base.visible = true;
		base.optional = false;
		base.resizable = true;
		base.sortable = false;
	}

	private void DestroyCell(VisualElement element)
	{
		if (element == null)
		{
			throw new ArgumentNullException("element");
		}
		if (!(element is HierarchyViewItemContainer hierarchyViewItemContainer))
		{
			throw new ArgumentException("Expected element to be a HierarchyViewItemContainer");
		}
		hierarchyViewItemContainer.ReleaseViewItem();
		m_ViewItemContainerPool.Release(hierarchyViewItemContainer);
	}

	private void BindCell(VisualElement element, int index)
	{
		if (element == null)
		{
			throw new ArgumentNullException("element");
		}
		if (!(element is HierarchyViewItemContainer hierarchyViewItemContainer))
		{
			throw new ArgumentException("Expected element to be a HierarchyViewItemContainer");
		}
		HierarchyNode lhs = m_View.ViewModel[index];
		if (lhs == HierarchyNode.Null)
		{
			throw new InvalidOperationException("Expected node to be valid");
		}
		if (m_View.Source.Exists(in lhs))
		{
			hierarchyViewItemContainer.Bind(in lhs, m_View);
			this.OnBindItem?.Invoke(hierarchyViewItemContainer.ViewItem);
		}
	}

	private void UnbindCell(VisualElement element, int index)
	{
		if (element == null)
		{
			throw new ArgumentNullException("element");
		}
		if (!(element is HierarchyViewItemContainer hierarchyViewItemContainer))
		{
			throw new ArgumentException("Expected element to be a HierarchyViewItemContainer");
		}
		if (hierarchyViewItemContainer.ViewItem != null)
		{
			this.OnUnbindItem?.Invoke(hierarchyViewItemContainer.ViewItem);
		}
		hierarchyViewItemContainer.Unbind();
	}
}
