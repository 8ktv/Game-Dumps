using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.Properties;
using UnityEngine.Pool;

namespace UnityEngine.UIElements;

public abstract class BaseListViewController : CollectionViewController
{
	private protected class SerializedObjectListControllerImpl
	{
		private Func<ISerializedObjectList> m_SerializedObjectListGetter;

		private BaseListViewController m_BaseListViewController;

		private ISerializedObjectList serializedObjectList => m_SerializedObjectListGetter?.Invoke();

		internal SerializedObjectListControllerImpl(BaseListViewController baseListViewController, Func<ISerializedObjectList> serializedObjectListGetter)
		{
			m_BaseListViewController = baseListViewController;
			m_SerializedObjectListGetter = serializedObjectListGetter;
		}

		public int GetItemsCount()
		{
			return serializedObjectList?.Count ?? 0;
		}

		internal int GetItemsMinCount()
		{
			return serializedObjectList?.minArraySize ?? 0;
		}

		public void AddItems(int itemCount)
		{
			int itemsCount = GetItemsCount();
			serializedObjectList.arraySize += itemCount;
			serializedObjectList.ApplyChanges();
			List<int> list = CollectionPool<List<int>, int>.Get();
			try
			{
				for (int i = 0; i < itemCount; i++)
				{
					list.Add(itemsCount + i);
				}
				m_BaseListViewController.RaiseItemsAdded(list);
			}
			finally
			{
				CollectionPool<List<int>, int>.Release(list);
			}
			m_BaseListViewController.RaiseOnSizeChanged();
		}

		internal void RemoveItems(int itemCount)
		{
			int itemsCount = GetItemsCount();
			serializedObjectList.arraySize -= itemCount;
			List<int> list = CollectionPool<List<int>, int>.Get();
			try
			{
				for (int i = itemsCount - itemCount; i < itemsCount; i++)
				{
					list.Add(i);
				}
				m_BaseListViewController.RaiseItemsRemoved(list);
			}
			finally
			{
				CollectionPool<List<int>, int>.Release(list);
			}
			serializedObjectList.ApplyChanges();
			m_BaseListViewController.RaiseOnSizeChanged();
		}

		public void RemoveItems(List<int> indices)
		{
			indices.Sort();
			m_BaseListViewController.RaiseItemsRemoved(indices);
			int num = serializedObjectList.Count;
			for (int num2 = indices.Count - 1; num2 >= 0; num2--)
			{
				int index = indices[num2];
				serializedObjectList.RemoveAt(index, num);
				num--;
			}
			serializedObjectList.ApplyChanges();
			m_BaseListViewController.RaiseOnSizeChanged();
		}

		public void RemoveItem(int index)
		{
			List<int> value;
			using (CollectionPool<List<int>, int>.Get(out value))
			{
				value.Add(index);
				m_BaseListViewController.RaiseItemsRemoved(value);
			}
			serializedObjectList.RemoveAt(index);
			serializedObjectList.ApplyChanges();
			m_BaseListViewController.RaiseOnSizeChanged();
		}

		public void ClearItems()
		{
			IEnumerable<int> indices = Enumerable.Range(0, GetItemsMinCount() - 1);
			serializedObjectList.arraySize = 0;
			serializedObjectList.ApplyChanges();
			m_BaseListViewController.RaiseItemsRemoved(indices);
			m_BaseListViewController.RaiseOnSizeChanged();
		}

		public void Move(int srcIndex, int destIndex)
		{
			if (srcIndex != destIndex)
			{
				serializedObjectList.Move(srcIndex, destIndex);
				serializedObjectList.ApplyChanges();
				m_BaseListViewController.RaiseItemIndexChanged(srcIndex, destIndex);
			}
		}
	}

	internal const string k_NullItemSourceError = "Unable to add or remove items because the source is not defined. Please assign a valid items source.";

	internal const string k_FixedItemSourceError = "Unable to add or remove items because the items source is a fixed size.";

	protected BaseListView baseListView => base.view as BaseListView;

	public event Action itemsSourceSizeChanged;

	public event Action<IEnumerable<int>> itemsAdded;

	public event Action<IEnumerable<int>> itemsRemoved;

	internal override void InvokeMakeItem(ReusableCollectionItem reusableItem)
	{
		if (reusableItem is ReusableListViewItem reusableListViewItem)
		{
			reusableListViewItem.Init(MakeItem(), baseListView.reorderable && baseListView.reorderMode == ListViewReorderMode.Animated);
			PostInitRegistration(reusableListViewItem);
		}
	}

	internal void PostInitRegistration(ReusableListViewItem listItem)
	{
		listItem.bindableElement.style.position = Position.Relative;
		listItem.bindableElement.style.flexBasis = StyleKeyword.Initial;
		listItem.bindableElement.style.marginTop = 0f;
		listItem.bindableElement.style.marginBottom = 0f;
		listItem.bindableElement.style.paddingTop = 0f;
		listItem.bindableElement.style.flexGrow = 0f;
		listItem.bindableElement.style.flexShrink = 0f;
	}

	internal override void SetBindingContext(ReusableCollectionItem reusableItem, int index)
	{
		base.SetBindingContext(reusableItem, index);
		if (baseListView.autoAssignSource)
		{
			reusableItem.rootElement.dataSource = itemsSource;
			reusableItem.rootElement.dataSourcePath = PropertyPath.FromIndex(index);
		}
	}

	internal override void InvokeBindItem(ReusableCollectionItem reusableItem, int index)
	{
		if (reusableItem is ReusableListViewItem reusableListViewItem)
		{
			bool flag = baseListView.reorderable && baseListView.reorderMode == ListViewReorderMode.Animated;
			reusableListViewItem.UpdateDragHandle(flag && NeedsDragHandle(index));
		}
		base.InvokeBindItem(reusableItem, index);
	}

	public virtual bool NeedsDragHandle(int index)
	{
		return true;
	}

	public virtual void AddItems(int itemCount)
	{
		if (itemCount <= 0)
		{
			return;
		}
		EnsureItemSourceCanBeResized();
		int itemsCount = GetItemsCount();
		List<int> list = CollectionPool<List<int>, int>.Get();
		try
		{
			if (itemsSource.IsFixedSize)
			{
				itemsSource = AddToArray((Array)itemsSource, itemCount);
				for (int i = 0; i < itemCount; i++)
				{
					list.Add(itemsCount + i);
				}
			}
			else
			{
				Type type = itemsSource.GetType();
				Type type2 = type.GetInterfaces().FirstOrDefault(IsGenericList);
				if (type2 != null && type2.GetGenericArguments()[0].IsValueType)
				{
					Type type3 = type2.GetGenericArguments()[0];
					for (int j = 0; j < itemCount; j++)
					{
						list.Add(itemsCount + j);
						itemsSource.Add(Activator.CreateInstance(type3));
					}
				}
				else
				{
					for (int k = 0; k < itemCount; k++)
					{
						list.Add(itemsCount + k);
						itemsSource.Add(null);
					}
				}
			}
			RaiseItemsAdded(list);
		}
		finally
		{
			CollectionPool<List<int>, int>.Release(list);
		}
		RaiseOnSizeChanged();
		static bool IsGenericList(Type t)
		{
			return t.IsGenericType && t.GetGenericTypeDefinition() == typeof(IList<>);
		}
	}

	public virtual void Move(int index, int newIndex)
	{
		if (itemsSource == null || index == newIndex)
		{
			return;
		}
		int num = Mathf.Min(index, newIndex);
		int num2 = Mathf.Max(index, newIndex);
		if (num >= 0 && num2 < itemsSource.Count)
		{
			int dstIndex = newIndex;
			int num3 = ((newIndex < index) ? 1 : (-1));
			while (Mathf.Min(index, newIndex) < Mathf.Max(index, newIndex))
			{
				Swap(index, newIndex);
				newIndex += num3;
			}
			RaiseItemIndexChanged(index, dstIndex);
		}
	}

	public virtual void RemoveItem(int index)
	{
		List<int> value;
		using (CollectionPool<List<int>, int>.Get(out value))
		{
			value.Add(index);
			RemoveItems(value);
		}
	}

	public virtual void RemoveItems(List<int> indices)
	{
		EnsureItemSourceCanBeResized();
		if (indices == null)
		{
			return;
		}
		indices.Sort();
		RaiseItemsRemoved(indices);
		if (itemsSource.IsFixedSize)
		{
			itemsSource = RemoveFromArray((Array)itemsSource, indices);
		}
		else
		{
			for (int num = indices.Count - 1; num >= 0; num--)
			{
				itemsSource.RemoveAt(indices[num]);
			}
		}
		RaiseOnSizeChanged();
	}

	internal virtual void RemoveItems(int itemCount)
	{
		if (itemCount <= 0)
		{
			return;
		}
		int itemsCount = GetItemsCount();
		List<int> list = CollectionPool<List<int>, int>.Get();
		try
		{
			int num = itemsCount - itemCount;
			for (int i = num; i < itemsCount; i++)
			{
				list.Add(i);
			}
			RemoveItems(list);
		}
		finally
		{
			CollectionPool<List<int>, int>.Release(list);
		}
	}

	public virtual void ClearItems()
	{
		if (itemsSource != null)
		{
			EnsureItemSourceCanBeResized();
			IEnumerable<int> indices = Enumerable.Range(0, itemsSource.Count - 1);
			itemsSource.Clear();
			RaiseItemsRemoved(indices);
			RaiseOnSizeChanged();
		}
	}

	protected void RaiseOnSizeChanged()
	{
		this.itemsSourceSizeChanged?.Invoke();
	}

	protected void RaiseItemsAdded(IEnumerable<int> indices)
	{
		this.itemsAdded?.Invoke(indices);
	}

	protected void RaiseItemsRemoved(IEnumerable<int> indices)
	{
		this.itemsRemoved?.Invoke(indices);
	}

	private static Array AddToArray(Array source, int itemCount)
	{
		Type elementType = source.GetType().GetElementType();
		if (elementType == null)
		{
			throw new InvalidOperationException("Cannot resize source, because its size is fixed.");
		}
		Array array = Array.CreateInstance(elementType, source.Length + itemCount);
		Array.Copy(source, array, source.Length);
		return array;
	}

	private static Array RemoveFromArray(Array source, List<int> indicesToRemove)
	{
		int length = source.Length;
		int num = length - indicesToRemove.Count;
		if (num < 0)
		{
			throw new InvalidOperationException("Cannot remove more items than the current count from source.");
		}
		Type elementType = source.GetType().GetElementType();
		if (num == 0)
		{
			return Array.CreateInstance(elementType, 0);
		}
		Array array = Array.CreateInstance(elementType, num);
		int num2 = 0;
		int num3 = 0;
		for (int i = 0; i < source.Length; i++)
		{
			if (num3 < indicesToRemove.Count && indicesToRemove[num3] == i)
			{
				num3++;
				continue;
			}
			array.SetValue(source.GetValue(i), num2);
			num2++;
		}
		return array;
	}

	private void Swap(int lhs, int rhs)
	{
		IList list = itemsSource;
		IList list2 = itemsSource;
		object obj = itemsSource[rhs];
		object obj2 = itemsSource[lhs];
		object obj3 = (list[lhs] = obj);
		obj3 = (list2[rhs] = obj2);
	}

	private void EnsureItemSourceCanBeResized()
	{
		if (itemsSource == null)
		{
			throw new InvalidOperationException("Unable to add or remove items because the source is not defined. Please assign a valid items source.");
		}
		if (itemsSource.IsFixedSize && !itemsSource.GetType().IsArray)
		{
			throw new InvalidOperationException("Unable to add or remove items because the items source is a fixed size.");
		}
	}
}
