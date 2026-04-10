using System.Collections.Generic;
using UnityEngine.Pool;

namespace UnityEngine.UIElements.HierarchyV2;

internal class RecycledItem
{
	private static UnityEngine.Pool.ObjectPool<RecycledItem> s_ItemPool = new UnityEngine.Pool.ObjectPool<RecycledItem>(() => new RecycledItem(), null, delegate(RecycledItem i)
	{
		i.DetachElement();
	}, delegate(RecycledItem i)
	{
		i.DestroyElement();
	});

	public int index;

	public float renderedHeight;

	public bool isLastItem;

	public const int k_UndefinedIndex = -1;

	private CollectionView m_CollectionView;

	private VisualElement m_Element;

	public LinkedListNode<RecycledItem> node { get; set; }

	public VisualElement element
	{
		get
		{
			return m_Element;
		}
		private set
		{
			m_Element = value;
		}
	}

	public float verticalOffset
	{
		get
		{
			return m_Element.resolvedStyle.translate.y;
		}
		set
		{
			Vector3 translate = m_Element.resolvedStyle.translate;
			translate.y = value;
			m_Element.style.translate = translate;
		}
	}

	public static RecycledItem AllocateItem(VisualElement element, CollectionView parent)
	{
		RecycledItem recycledItem = s_ItemPool.Get();
		recycledItem.Assign(element, parent);
		recycledItem.node = new LinkedListNode<RecycledItem>(recycledItem);
		return recycledItem;
	}

	public static void Recycle(RecycledItem item)
	{
		s_ItemPool.Release(item);
	}

	public static void ClearItemPool()
	{
		s_ItemPool.Clear();
	}

	public void Assign(VisualElement element, CollectionView parent)
	{
		m_CollectionView = parent;
		renderedHeight = -1f;
		this.element = element;
		index = -1;
		element.AddToClassList(BaseVerticalCollectionView.itemUssClassName);
		element.RegisterCallback<GeometryChangedEvent>(OnSizeChange);
	}

	private void OnSizeChange(GeometryChangedEvent evt)
	{
		renderedHeight = evt.newRect.height;
		if (evt.layoutPass < 4)
		{
			UpdatePositions(this);
		}
	}

	public static void UpdatePositions(RecycledItem item)
	{
		for (LinkedListNode<RecycledItem> next = item.node; next != null; next = next.Next)
		{
			float num = next.Value.renderedHeight;
			if (!float.IsNaN(num) && num > 0f)
			{
				next.Value.UpdatePosition();
				if (next.Next == null)
				{
					next.Value.m_CollectionView.ItemPositionUpdated(next.Value);
				}
			}
		}
	}

	private void UpdatePosition()
	{
		float a = 0f;
		if (node.Previous != null)
		{
			a = node.Previous.Value.verticalOffset + node.Previous.Value.renderedHeight;
		}
		if (!Mathf.Approximately(a, verticalOffset))
		{
			verticalOffset = a;
		}
	}

	public void DetachElement()
	{
		if (element != null)
		{
			element.UnregisterCallback<GeometryChangedEvent>(OnSizeChange);
			element.RemoveFromClassList(BaseVerticalCollectionView.itemUssClassName);
			element.RemoveFromHierarchy();
			SetSelected(selected: false);
			index = -1;
		}
	}

	private void DestroyElement()
	{
		m_CollectionView.OnDestroyItem(this);
	}

	public void SetSelected(bool selected)
	{
		if (element != null)
		{
			if (selected)
			{
				element.AddToClassList(BaseVerticalCollectionView.itemSelectedVariantUssClassName);
				element.pseudoStates |= PseudoStates.Checked;
			}
			else
			{
				element.RemoveFromClassList(BaseVerticalCollectionView.itemSelectedVariantUssClassName);
				element.pseudoStates &= ~PseudoStates.Checked;
			}
		}
	}
}
