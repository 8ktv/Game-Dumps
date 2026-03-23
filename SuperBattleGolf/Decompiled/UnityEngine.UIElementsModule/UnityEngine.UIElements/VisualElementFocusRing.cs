using System;
using System.Collections.Generic;

namespace UnityEngine.UIElements;

public class VisualElementFocusRing : IFocusRing
{
	public enum DefaultFocusOrder
	{
		ChildOrder,
		PositionXY,
		PositionYX
	}

	private class FocusRingRecord
	{
		public int m_AutoIndex;

		public Focusable m_Focusable;

		public bool m_IsSlot;

		public List<FocusRingRecord> m_ScopeNavigationOrder;
	}

	private readonly VisualElement root;

	private List<FocusRingRecord> m_FocusRing;

	private FocusController focusController => root.focusController;

	public DefaultFocusOrder defaultFocusOrder { get; set; }

	public VisualElementFocusRing(VisualElement root, DefaultFocusOrder dfo = DefaultFocusOrder.ChildOrder)
	{
		defaultFocusOrder = dfo;
		this.root = root;
		m_FocusRing = new List<FocusRingRecord>();
	}

	private int FocusRingAutoIndexSort(FocusRingRecord a, FocusRingRecord b)
	{
		switch (defaultFocusOrder)
		{
		default:
			return Comparer<int>.Default.Compare(a.m_AutoIndex, b.m_AutoIndex);
		case DefaultFocusOrder.PositionXY:
		{
			VisualElement visualElement3 = a.m_Focusable as VisualElement;
			VisualElement visualElement4 = b.m_Focusable as VisualElement;
			if (visualElement3 != null && visualElement4 != null)
			{
				if (visualElement3.layout.position.x < visualElement4.layout.position.x)
				{
					return -1;
				}
				if (visualElement3.layout.position.x > visualElement4.layout.position.x)
				{
					return 1;
				}
				if (visualElement3.layout.position.y < visualElement4.layout.position.y)
				{
					return -1;
				}
				if (visualElement3.layout.position.y > visualElement4.layout.position.y)
				{
					return 1;
				}
			}
			return Comparer<int>.Default.Compare(a.m_AutoIndex, b.m_AutoIndex);
		}
		case DefaultFocusOrder.PositionYX:
		{
			VisualElement visualElement = a.m_Focusable as VisualElement;
			VisualElement visualElement2 = b.m_Focusable as VisualElement;
			if (visualElement != null && visualElement2 != null)
			{
				if (visualElement.layout.position.y < visualElement2.layout.position.y)
				{
					return -1;
				}
				if (visualElement.layout.position.y > visualElement2.layout.position.y)
				{
					return 1;
				}
				if (visualElement.layout.position.x < visualElement2.layout.position.x)
				{
					return -1;
				}
				if (visualElement.layout.position.x > visualElement2.layout.position.x)
				{
					return 1;
				}
			}
			return Comparer<int>.Default.Compare(a.m_AutoIndex, b.m_AutoIndex);
		}
		}
	}

	private int FocusRingSort(FocusRingRecord a, FocusRingRecord b)
	{
		if (a.m_Focusable.tabIndex == 0 && b.m_Focusable.tabIndex == 0)
		{
			return FocusRingAutoIndexSort(a, b);
		}
		if (a.m_Focusable.tabIndex == 0)
		{
			return 1;
		}
		if (b.m_Focusable.tabIndex == 0)
		{
			return -1;
		}
		int num = Comparer<int>.Default.Compare(a.m_Focusable.tabIndex, b.m_Focusable.tabIndex);
		if (num == 0)
		{
			num = FocusRingAutoIndexSort(a, b);
		}
		return num;
	}

	private void DoUpdate()
	{
		m_FocusRing.Clear();
		if (root != null)
		{
			List<FocusRingRecord> list = new List<FocusRingRecord>();
			int scopeIndex = 0;
			BuildRingForScopeRecursive(root, ref scopeIndex, list);
			SortAndFlattenScopeLists(list);
		}
	}

	private void BuildRingForScopeRecursive(VisualElement ve, ref int scopeIndex, List<FocusRingRecord> scopeList)
	{
		int childCount = ve.hierarchy.childCount;
		for (int i = 0; i < childCount; i++)
		{
			VisualElement visualElement = ve.hierarchy[i];
			bool flag = visualElement.parent != null && visualElement == visualElement.parent.contentContainer;
			if (visualElement.isCompositeRoot || flag)
			{
				FocusRingRecord focusRingRecord = new FocusRingRecord
				{
					m_AutoIndex = scopeIndex++,
					m_Focusable = visualElement,
					m_IsSlot = flag,
					m_ScopeNavigationOrder = new List<FocusRingRecord>()
				};
				scopeList.Add(focusRingRecord);
				int scopeIndex2 = 0;
				BuildRingForScopeRecursive(visualElement, ref scopeIndex2, focusRingRecord.m_ScopeNavigationOrder);
			}
			else
			{
				if (visualElement.canGrabFocus && visualElement.areAncestorsAndSelfDisplayed && visualElement.tabIndex >= 0)
				{
					scopeList.Add(new FocusRingRecord
					{
						m_AutoIndex = scopeIndex++,
						m_Focusable = visualElement,
						m_IsSlot = false,
						m_ScopeNavigationOrder = null
					});
				}
				BuildRingForScopeRecursive(visualElement, ref scopeIndex, scopeList);
			}
		}
	}

	private void SortAndFlattenScopeLists(List<FocusRingRecord> rootScopeList)
	{
		if (rootScopeList == null)
		{
			return;
		}
		rootScopeList.Sort(FocusRingSort);
		foreach (FocusRingRecord rootScope in rootScopeList)
		{
			if (rootScope.m_Focusable.canGrabFocus && rootScope.m_Focusable.tabIndex >= 0)
			{
				if (!rootScope.m_Focusable.excludeFromFocusRing)
				{
					m_FocusRing.Add(rootScope);
				}
				SortAndFlattenScopeLists(rootScope.m_ScopeNavigationOrder);
			}
			else if (rootScope.m_IsSlot)
			{
				SortAndFlattenScopeLists(rootScope.m_ScopeNavigationOrder);
			}
			rootScope.m_ScopeNavigationOrder = null;
		}
	}

	private int GetFocusableInternalIndex(Focusable f)
	{
		if (f != null)
		{
			for (int i = 0; i < m_FocusRing.Count; i++)
			{
				if (f == m_FocusRing[i].m_Focusable)
				{
					return i;
				}
			}
		}
		return -1;
	}

	public FocusChangeDirection GetFocusChangeDirection(Focusable currentFocusable, EventBase e)
	{
		if (e == null)
		{
			throw new ArgumentNullException("e");
		}
		if (e.eventTypeId == EventBase<PointerDownEvent>.TypeId() && focusController.GetFocusableParentForPointerEvent(e.elementTarget, out var effectiveTarget))
		{
			return VisualElementFocusChangeTarget.GetPooled(effectiveTarget);
		}
		if (currentFocusable != null && currentFocusable.isIMGUIContainer)
		{
			return FocusChangeDirection.none;
		}
		if (e.eventTypeId == EventBase<NavigationMoveEvent>.TypeId())
		{
			return ((NavigationMoveEvent)e).direction switch
			{
				NavigationMoveEvent.Direction.Previous => VisualElementFocusChangeDirection.left, 
				NavigationMoveEvent.Direction.Next => VisualElementFocusChangeDirection.right, 
				_ => FocusChangeDirection.none, 
			};
		}
		return FocusChangeDirection.none;
	}

	public Focusable GetNextFocusable(Focusable currentFocusable, FocusChangeDirection direction)
	{
		if (direction == FocusChangeDirection.none || direction == FocusChangeDirection.unspecified)
		{
			return currentFocusable;
		}
		if (!(direction is VisualElementFocusChangeTarget { target: var target }))
		{
			return GetNextFocusableInSequence(currentFocusable, direction);
		}
		return target;
	}

	internal Focusable GetNextFocusableInSequence(Focusable currentFocusable, FocusChangeDirection direction)
	{
		DoUpdate();
		if (m_FocusRing.Count == 0)
		{
			return null;
		}
		int focusableInternalIndex = GetFocusableInternalIndex(currentFocusable);
		if (currentFocusable != null && focusableInternalIndex == -1)
		{
			if (direction == VisualElementFocusChangeDirection.right)
			{
				return GetNextFocusableInTree(currentFocusable as VisualElement);
			}
			if (direction == VisualElementFocusChangeDirection.left)
			{
				return GetPreviousFocusableInTree(currentFocusable as VisualElement);
			}
		}
		int num = 0;
		if (direction == VisualElementFocusChangeDirection.right)
		{
			num = focusableInternalIndex + 1;
			if (num == m_FocusRing.Count)
			{
				num = 0;
			}
			while (m_FocusRing[num].m_Focusable.delegatesFocus)
			{
				num++;
				if (num == m_FocusRing.Count)
				{
					return null;
				}
			}
		}
		else if (direction == VisualElementFocusChangeDirection.left)
		{
			num = focusableInternalIndex - 1;
			if (num < 0)
			{
				num = m_FocusRing.Count - 1;
			}
			while (m_FocusRing[num].m_Focusable.delegatesFocus)
			{
				num--;
				if (num == -1)
				{
					return null;
				}
			}
		}
		return m_FocusRing[num].m_Focusable;
	}

	internal VisualElement GetNextFocusableInTree(VisualElement currentFocusable)
	{
		if (currentFocusable == null)
		{
			return null;
		}
		VisualElement nextElementDepthFirst = GetNextElementDepthFirst(currentFocusable);
		while (!nextElementDepthFirst.canGrabFocus || nextElementDepthFirst.tabIndex < 0 || nextElementDepthFirst.excludeFromFocusRing)
		{
			nextElementDepthFirst = GetNextElementDepthFirst(nextElementDepthFirst);
			if (nextElementDepthFirst == currentFocusable)
			{
				return currentFocusable;
			}
		}
		return nextElementDepthFirst;
	}

	internal VisualElement GetPreviousFocusableInTree(VisualElement currentFocusable)
	{
		if (currentFocusable == null)
		{
			return null;
		}
		VisualElement previousElementDepthFirst = GetPreviousElementDepthFirst(currentFocusable);
		while (!previousElementDepthFirst.canGrabFocus || previousElementDepthFirst.tabIndex < 0 || previousElementDepthFirst.excludeFromFocusRing)
		{
			previousElementDepthFirst = GetPreviousElementDepthFirst(previousElementDepthFirst);
			if (previousElementDepthFirst == currentFocusable)
			{
				return currentFocusable;
			}
		}
		return previousElementDepthFirst;
	}

	private VisualElement GetNextElementDepthFirst(VisualElement ve)
	{
		ve = ve.GetNextElementDepthFirst();
		if (!root.Contains(ve))
		{
			ve = root;
		}
		return ve;
	}

	private VisualElement GetPreviousElementDepthFirst(VisualElement ve)
	{
		ve = ve.GetPreviousElementDepthFirst();
		if (!root.Contains(ve))
		{
			ve = root;
			while (ve.childCount > 0)
			{
				ve = ve.hierarchy.ElementAt(ve.childCount - 1);
			}
		}
		return ve;
	}
}
