namespace UnityEngine.UIElements;

internal class NavigateFocusRing : IFocusRing
{
	public class ChangeDirection : FocusChangeDirection
	{
		public ChangeDirection(int i)
			: base(i)
		{
		}
	}

	private struct FocusableHierarchyTraversal
	{
		public VisualElement root;

		public VisualElement currentFocusable;

		public Rect validRect;

		public bool firstPass;

		public ChangeDirection direction;

		private bool ValidateHierarchyTraversal(VisualElement v)
		{
			return IsActive(v) && v.ChangeCoordinatesTo(root, v.boundingBox).Overlaps(validRect);
		}

		private bool ValidateElement(VisualElement v)
		{
			return IsNavigable(v) && v.ChangeCoordinatesTo(root, v.rect).Overlaps(validRect);
		}

		private int Order(VisualElement a, VisualElement b)
		{
			Rect ra = a.ChangeCoordinatesTo(root, a.rect);
			Rect rb = b.ChangeCoordinatesTo(root, b.rect);
			int num = StrictOrder(ra, rb);
			return (num != 0) ? num : TieBreaker(ra, rb);
		}

		private int StrictOrder(VisualElement a, VisualElement b)
		{
			return StrictOrder(a.ChangeCoordinatesTo(root, a.rect), b.ChangeCoordinatesTo(root, b.rect));
		}

		private int StrictOrder(Rect ra, Rect rb)
		{
			float num = 0f;
			if (direction == Up)
			{
				num = rb.yMax - ra.yMax;
			}
			else if (direction == Down)
			{
				num = ra.yMin - rb.yMin;
			}
			else if (direction == Left)
			{
				num = rb.xMax - ra.xMax;
			}
			else if (direction == Right)
			{
				num = ra.xMin - rb.xMin;
			}
			if (!Mathf.Approximately(num, 0f))
			{
				return (num > 0f) ? 1 : (-1);
			}
			return 0;
		}

		private int TieBreaker(Rect ra, Rect rb)
		{
			Rect rect = currentFocusable.ChangeCoordinatesTo(root, currentFocusable.rect);
			float num = (ra.min - rect.min).sqrMagnitude - (rb.min - rect.min).sqrMagnitude;
			if (!Mathf.Approximately(num, 0f))
			{
				return (num > 0f) ? 1 : (-1);
			}
			return 0;
		}

		public VisualElement GetBestOverall(VisualElement candidate, VisualElement bestSoFar = null)
		{
			if (!ValidateHierarchyTraversal(candidate))
			{
				return bestSoFar;
			}
			if (ValidateElement(candidate))
			{
				if ((!firstPass || StrictOrder(candidate, currentFocusable) > 0) && (bestSoFar == null || Order(bestSoFar, candidate) > 0))
				{
					bestSoFar = candidate;
				}
				return bestSoFar;
			}
			int childCount = candidate.hierarchy.childCount;
			for (int i = 0; i < childCount; i++)
			{
				VisualElement candidate2 = candidate.hierarchy[i];
				bestSoFar = GetBestOverall(candidate2, bestSoFar);
			}
			return bestSoFar;
		}
	}

	public static readonly ChangeDirection Left = new ChangeDirection(1);

	public static readonly ChangeDirection Right = new ChangeDirection(2);

	public static readonly ChangeDirection Up = new ChangeDirection(3);

	public static readonly ChangeDirection Down = new ChangeDirection(4);

	public static readonly FocusChangeDirection Next = VisualElementFocusChangeDirection.right;

	public static readonly FocusChangeDirection Previous = VisualElementFocusChangeDirection.left;

	private readonly VisualElement m_Root;

	private readonly VisualElementFocusRing m_Ring;

	private FocusController focusController => m_Root.focusController;

	public NavigateFocusRing(VisualElement root)
	{
		m_Root = root;
		m_Ring = new VisualElementFocusRing(root);
	}

	public FocusChangeDirection GetFocusChangeDirection(Focusable currentFocusable, EventBase e)
	{
		if (e.eventTypeId == EventBase<PointerDownEvent>.TypeId() && focusController.GetFocusableParentForPointerEvent(e.elementTarget, out var effectiveTarget))
		{
			return VisualElementFocusChangeTarget.GetPooled(effectiveTarget);
		}
		if (e.eventTypeId == EventBase<NavigationMoveEvent>.TypeId())
		{
			switch (((NavigationMoveEvent)e).direction)
			{
			case NavigationMoveEvent.Direction.Left:
				return Left;
			case NavigationMoveEvent.Direction.Up:
				return Up;
			case NavigationMoveEvent.Direction.Right:
				return Right;
			case NavigationMoveEvent.Direction.Down:
				return Down;
			case NavigationMoveEvent.Direction.Next:
				return Next;
			case NavigationMoveEvent.Direction.Previous:
				return Previous;
			}
		}
		return FocusChangeDirection.none;
	}

	public virtual Focusable GetNextFocusable(Focusable currentFocusable, FocusChangeDirection direction)
	{
		if (direction == FocusChangeDirection.none || direction == FocusChangeDirection.unspecified)
		{
			return currentFocusable;
		}
		if (!(direction is VisualElementFocusChangeTarget { target: var target }))
		{
			VisualElement root = m_Root;
			VisualElement root2 = m_Root;
			if (root2 != null && root2.elementPanel?.isFlat == false)
			{
				if (!IsWorldSpaceNavigationValid(currentFocusable, out var document))
				{
					return null;
				}
				if (direction == Next || direction == Previous)
				{
					return document.focusRing?.GetNextFocusableInSequence(currentFocusable, direction);
				}
				root = document.rootVisualElement;
			}
			if (direction == Up || direction == Down || direction == Right || direction == Left)
			{
				return GetNextFocusable2D(currentFocusable, (ChangeDirection)direction, root);
			}
			return m_Ring.GetNextFocusableInSequence(currentFocusable, direction);
		}
		return target;
	}

	private bool IsWorldSpaceNavigationValid(Focusable currentFocusable, out UIDocument document)
	{
		document = null;
		if (!(currentFocusable is VisualElement element))
		{
			return false;
		}
		document = UIDocument.FindRootUIDocument(element);
		if (document == null || document.rootVisualElement == null)
		{
			return false;
		}
		return true;
	}

	private Focusable GetNextFocusable2D(Focusable currentFocusable, ChangeDirection direction, VisualElement root)
	{
		VisualElement visualElement = currentFocusable as VisualElement;
		if (visualElement == null)
		{
			visualElement = root;
		}
		Rect boundingBox = root.boundingBox;
		Rect rect = new Rect(boundingBox.position - Vector2.one, boundingBox.size + Vector2.one * 2f);
		Rect rect2 = visualElement.ChangeCoordinatesTo(root, visualElement.rect);
		Rect validRect = new Rect(rect2.position - Vector2.one, rect2.size + Vector2.one * 2f);
		if (direction == Up)
		{
			validRect.yMin = rect.yMin;
		}
		else if (direction == Down)
		{
			validRect.yMax = rect.yMax;
		}
		else if (direction == Left)
		{
			validRect.xMin = rect.xMin;
		}
		else if (direction == Right)
		{
			validRect.xMax = rect.xMax;
		}
		VisualElement bestOverall = new FocusableHierarchyTraversal
		{
			root = root,
			currentFocusable = visualElement,
			direction = direction,
			validRect = validRect,
			firstPass = true
		}.GetBestOverall(root);
		if (bestOverall != null)
		{
			return bestOverall;
		}
		validRect = new Rect(rect2.position - Vector2.one, rect2.size + Vector2.one * 2f);
		if (direction == Down)
		{
			validRect.yMin = rect.yMin;
		}
		else if (direction == Up)
		{
			validRect.yMax = rect.yMax;
		}
		else if (direction == Right)
		{
			validRect.xMin = rect.xMin;
		}
		else if (direction == Left)
		{
			validRect.xMax = rect.xMax;
		}
		bestOverall = new FocusableHierarchyTraversal
		{
			root = root,
			currentFocusable = visualElement,
			direction = direction,
			validRect = validRect,
			firstPass = false
		}.GetBestOverall(root);
		if (bestOverall != null)
		{
			return bestOverall;
		}
		return currentFocusable;
	}

	private static bool IsActive(VisualElement v)
	{
		return v.resolvedStyle.display != DisplayStyle.None && v.enabledInHierarchy;
	}

	private static bool IsNavigable(Focusable focusable)
	{
		return focusable.canGrabFocus && focusable.tabIndex >= 0 && !focusable.delegatesFocus && !focusable.excludeFromFocusRing;
	}
}
