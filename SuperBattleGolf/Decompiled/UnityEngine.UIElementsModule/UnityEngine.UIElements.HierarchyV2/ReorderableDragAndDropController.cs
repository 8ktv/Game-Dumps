using System.Collections.Generic;

namespace UnityEngine.UIElements.HierarchyV2;

internal class ReorderableDragAndDropController : ICollectionDragAndDropController, IDragAndDropController<IListDragAndDropArgs>, IReorderable
{
	private readonly CollectionView m_CollectionView;

	private readonly List<int> m_SortedSelectedIndices = new List<int>();

	public bool enableReordering { get; set; } = true;

	public IEnumerable<int> GetSortedSelectedIndices()
	{
		return m_SortedSelectedIndices;
	}

	public ReorderableDragAndDropController(CollectionView view)
	{
		m_CollectionView = view;
	}

	public DragVisualMode HandleDragAndDrop(IListDragAndDropArgs args)
	{
		if (args.dragAndDropPosition == DragAndDropPosition.OverItem)
		{
			return DragVisualMode.Rejected;
		}
		return (args.dragAndDropData.source == m_CollectionView) ? DragVisualMode.Move : DragVisualMode.Rejected;
	}

	public void OnDrop(IListDragAndDropArgs args)
	{
		int insertAtIndex = args.insertAtIndex;
		int num = 0;
		int num2 = 0;
		for (int num3 = m_SortedSelectedIndices.Count - 1; num3 >= 0; num3--)
		{
			int num4 = m_SortedSelectedIndices[num3];
			if (num4 >= 0)
			{
				int num5 = insertAtIndex - num;
				if (num4 >= insertAtIndex)
				{
					num4 += num2;
					num2++;
				}
				else if (num4 < num5)
				{
					num++;
					num5--;
				}
				m_CollectionView.Move(num4, num5);
			}
		}
		if (m_CollectionView.selectionType != SelectionType.None)
		{
			List<int> list = new List<int>();
			for (int i = 0; i < m_SortedSelectedIndices.Count; i++)
			{
				list.Add(insertAtIndex - num + i);
			}
			m_CollectionView.SetSelectionWithoutNotify(list);
		}
		else
		{
			m_CollectionView.ClearSelection();
		}
		m_CollectionView.RefreshItems();
	}

	public bool CanStartDrag(IEnumerable<int> itemIndices)
	{
		return enableReordering;
	}

	public virtual bool CanDrop()
	{
		return true;
	}

	public StartDragArgs SetupDragAndDrop(IEnumerable<int> itemIndices, bool skipText = false)
	{
		m_SortedSelectedIndices.Clear();
		string text = string.Empty;
		if (itemIndices != null)
		{
			foreach (int itemIndex in itemIndices)
			{
				m_SortedSelectedIndices.Add(itemIndex);
				if (!skipText)
				{
					if (string.IsNullOrEmpty(text))
					{
						Label label = m_CollectionView.GetRootElementForIndex(itemIndex)?.Q<Label>();
						text = ((label != null) ? label.text : $"Item {itemIndex}");
					}
					else
					{
						text = "<Multiple>";
						skipText = true;
					}
				}
			}
		}
		m_SortedSelectedIndices.Sort(CompareIndex);
		return new StartDragArgs(text, DragVisualMode.Move);
	}

	private int CompareIndex(int index1, int index2)
	{
		return index1.CompareTo(index2);
	}
}
