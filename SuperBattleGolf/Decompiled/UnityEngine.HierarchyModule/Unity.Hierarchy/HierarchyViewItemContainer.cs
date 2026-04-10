using System;
using Unity.Scripting.LifecycleManagement;
using UnityEngine.Pool;
using UnityEngine.UIElements;

namespace Unity.Hierarchy;

internal class HierarchyViewItemContainer : VisualElement
{
	[AutoStaticsCleanupOnCodeReload(CleanupStrategy = CleanupStrategy.Clear)]
	private static readonly UnityEngine.Pool.ObjectPool<HierarchyViewItem> s_ViewItemPool = new UnityEngine.Pool.ObjectPool<HierarchyViewItem>(() => new HierarchyViewItem());

	private HierarchyView m_View;

	private HierarchyViewItem m_ViewItem;

	private HierarchyNodeTypeHandler m_ViewItemNodeTypeHandler;

	public HierarchyView View => m_View;

	public HierarchyViewItem ViewItem => m_ViewItem;

	public HierarchyNodeTypeHandler ViewItemNodeTypeHandler => m_ViewItemNodeTypeHandler;

	public void Bind(in HierarchyNode node, HierarchyView view)
	{
		if (node == HierarchyNode.Null)
		{
			throw new ArgumentNullException("node");
		}
		if (view == null)
		{
			throw new ArgumentNullException("view");
		}
		HierarchyNodeTypeHandler nodeTypeHandler = view.Source.GetNodeTypeHandler(in node);
		if (m_ViewItem == null || m_ViewItemNodeTypeHandler != nodeTypeHandler)
		{
			ReleaseViewItem();
			m_ViewItem = ((nodeTypeHandler != null) ? nodeTypeHandler.ViewItemPool.Get() : s_ViewItemPool.Get());
			if (m_ViewItem == null)
			{
				throw new NullReferenceException("Failed to get a view item from the pool");
			}
			Add(m_ViewItem);
			m_ViewItemNodeTypeHandler = nodeTypeHandler;
		}
		m_View = view;
		m_ViewItem.Bind(in node, m_View);
	}

	public void Unbind()
	{
		m_ViewItem?.Unbind();
	}

	public void ReleaseViewItem()
	{
		if (m_ViewItem != null)
		{
			if (m_ViewItem.Bound)
			{
				m_ViewItem.Unbind();
			}
			Remove(m_ViewItem);
			if (m_ViewItemNodeTypeHandler != null)
			{
				m_ViewItemNodeTypeHandler.ViewItemPool.Release(m_ViewItem);
			}
			else
			{
				s_ViewItemPool.Release(m_ViewItem);
			}
			m_ViewItem = null;
		}
		m_View = null;
		m_ViewItemNodeTypeHandler = null;
	}
}
