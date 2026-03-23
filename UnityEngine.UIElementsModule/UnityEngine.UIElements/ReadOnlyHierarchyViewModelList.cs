using System;
using System.Collections;
using Unity.Hierarchy;

namespace UnityEngine.UIElements;

internal class ReadOnlyHierarchyViewModelList : IList, ICollection, IEnumerable
{
	private struct Enumerator : IEnumerator
	{
		private readonly HierarchyViewModel m_HierarchyViewModel;

		private HierarchyViewModel.Enumerator m_Enumerator;

		public object Current => m_Enumerator.Current;

		public Enumerator(HierarchyViewModel hierarchyViewModel)
		{
			m_HierarchyViewModel = hierarchyViewModel;
			m_Enumerator = hierarchyViewModel.GetEnumerator();
		}

		public bool MoveNext()
		{
			return m_Enumerator.MoveNext();
		}

		public void Reset()
		{
			m_Enumerator = m_HierarchyViewModel.GetEnumerator();
		}
	}

	private readonly HierarchyViewModel m_HierarchyViewModel;

	public bool IsFixedSize => true;

	public bool IsReadOnly => true;

	public int Count => m_HierarchyViewModel.Count;

	public object this[int index]
	{
		get
		{
			return m_HierarchyViewModel[index];
		}
		set
		{
			throw new NotSupportedException();
		}
	}

	public bool IsSynchronized
	{
		get
		{
			throw new NotSupportedException();
		}
	}

	public object SyncRoot
	{
		get
		{
			throw new NotSupportedException();
		}
	}

	public bool Contains(object value)
	{
		return value is HierarchyNode node && m_HierarchyViewModel.Contains(in node);
	}

	public int IndexOf(object value)
	{
		return (value is HierarchyNode node) ? m_HierarchyViewModel.IndexOf(in node) : BaseTreeView.invalidId;
	}

	public ReadOnlyHierarchyViewModelList(HierarchyViewModel viewModel)
	{
		m_HierarchyViewModel = viewModel;
	}

	public void CopyTo(Array array, int index)
	{
		for (int i = index; i < m_HierarchyViewModel.Count; i++)
		{
			array.SetValue(m_HierarchyViewModel[i], i - index);
		}
	}

	public IEnumerator GetEnumerator()
	{
		return new Enumerator(m_HierarchyViewModel);
	}

	public int Add(object value)
	{
		throw new NotSupportedException();
	}

	public void Clear()
	{
		throw new NotSupportedException();
	}

	public void Insert(int index, object value)
	{
		throw new NotSupportedException();
	}

	public void Remove(object value)
	{
		throw new NotSupportedException();
	}

	public void RemoveAt(int index)
	{
		throw new NotSupportedException();
	}
}
