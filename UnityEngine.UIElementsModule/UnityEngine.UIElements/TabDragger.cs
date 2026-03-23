using System.Collections.Generic;

namespace UnityEngine.UIElements;

internal class TabDragger : PointerManipulator
{
	private const float k_StartDragDistance = 5f;

	private float m_StartPos;

	private float m_LastPos;

	private bool m_Moving;

	private bool m_Cancelled;

	private VisualElement m_Header;

	private TabView m_TabView;

	private VisualElement m_PreviewElement;

	private TabDragLocationPreview m_LocationPreviewElement;

	private VisualElement m_TabToMove;

	private float m_TabToMovePos;

	private VisualElement m_DestinationTab;

	private bool m_MoveBeforeDestination;

	private int m_DraggingPointerId = PointerId.invalidPointerId;

	private TabLayout tabLayout { get; set; }

	internal bool active { get; set; }

	internal bool isVertical { get; set; }

	internal bool moving
	{
		get
		{
			return m_Moving;
		}
		private set
		{
			if (m_Moving != value)
			{
				m_Moving = value;
				m_TabToMove.EnableInClassList(Tab.draggingUssClassName, moving);
			}
		}
	}

	public TabDragger()
	{
		base.activators.Add(new ManipulatorActivationFilter
		{
			button = MouseButton.LeftMouse
		});
	}

	protected override void RegisterCallbacksOnTarget()
	{
		base.target.RegisterCallback<PointerDownEvent>(OnPointerDown, TrickleDown.TrickleDown);
		base.target.RegisterCallback<PointerMoveEvent>(OnPointerMove);
		base.target.RegisterCallback<PointerUpEvent>(OnPointerUp, TrickleDown.TrickleDown);
		base.target.RegisterCallback<PointerCancelEvent>(OnPointerCancel);
		base.target.RegisterCallback<PointerCaptureOutEvent>(OnPointerCaptureOut);
		base.target.RegisterCallback<KeyDownEvent>(OnKeyDown);
	}

	protected override void UnregisterCallbacksFromTarget()
	{
		base.target.UnregisterCallback<PointerDownEvent>(OnPointerDown);
		base.target.UnregisterCallback<PointerMoveEvent>(OnPointerMove);
		base.target.UnregisterCallback<PointerUpEvent>(OnPointerUp);
		base.target.UnregisterCallback<PointerCancelEvent>(OnPointerCancel);
		base.target.UnregisterCallback<PointerCaptureOutEvent>(OnPointerCaptureOut);
		base.target.UnregisterCallback<KeyDownEvent>(OnKeyDown);
	}

	private void OnPointerDown(PointerDownEvent evt)
	{
		if (CanStartManipulation(evt))
		{
			if (active)
			{
				evt.StopImmediatePropagation();
			}
			else
			{
				ProcessDownEvent(evt, evt.localPosition, evt.pointerId);
			}
		}
	}

	private void OnPointerMove(PointerMoveEvent evt)
	{
		if (active)
		{
			ProcessMoveEvent(evt, evt.localPosition);
		}
	}

	private void OnPointerUp(PointerUpEvent evt)
	{
		if (active && CanStopManipulation(evt))
		{
			ProcessUpEvent(evt, evt.localPosition, evt.pointerId);
		}
	}

	private void OnPointerCancel(PointerCancelEvent evt)
	{
		if (active && CanStopManipulation(evt))
		{
			ProcessCancelEvent(evt, evt.pointerId);
		}
	}

	private void OnPointerCaptureOut(PointerCaptureOutEvent evt)
	{
		if (active)
		{
			ProcessCancelEvent(evt, evt.pointerId);
		}
	}

	private void ProcessCancelEvent(EventBase evt, int pointerId)
	{
		active = false;
		base.target.ReleasePointer(pointerId);
		if (!(evt is IPointerEvent))
		{
			base.target.panel.ProcessPointerCapture(pointerId);
		}
		if (moving)
		{
			EndDragMove(cancelled: true);
		}
	}

	private void OnKeyDown(KeyDownEvent e)
	{
		if (e.keyCode == KeyCode.Escape && moving)
		{
			active = false;
			if (m_DraggingPointerId != PointerId.invalidPointerId)
			{
				base.target.ReleasePointer(m_DraggingPointerId);
			}
			EndDragMove(cancelled: true);
			e.StopPropagation();
		}
	}

	private void ProcessDownEvent(EventBase evt, Vector2 localPosition, int pointerId)
	{
		VisualElement visualElement = evt.currentTarget as VisualElement;
		TabView tabView = visualElement?.GetFirstAncestorOfType<TabView>();
		if (tabView != null && tabView.reorderable)
		{
			base.target.CapturePointer(pointerId);
			m_DraggingPointerId = pointerId;
			if (!(evt is IPointerEvent))
			{
				base.target.panel.ProcessPointerCapture(pointerId);
			}
			m_TabView = tabView;
			m_Header = tabView.header;
			isVertical = m_Header.resolvedStyle.flexDirection == FlexDirection.Column;
			tabLayout = new TabLayout(m_TabView, isVertical);
			Vector2 vector = visualElement.ChangeCoordinatesTo(m_Header, localPosition);
			m_Cancelled = false;
			m_StartPos = (isVertical ? vector.y : vector.x);
			active = true;
			evt.StopPropagation();
		}
	}

	private void ProcessMoveEvent(EventBase e, Vector2 localPosition)
	{
		if (!m_Cancelled)
		{
			VisualElement src = e.currentTarget as VisualElement;
			Vector2 vector = src.ChangeCoordinatesTo(m_Header, localPosition);
			float num = (isVertical ? vector.y : vector.x);
			if (!moving && Mathf.Abs(m_StartPos - num) > 5f)
			{
				BeginDragMove(m_StartPos);
			}
			if (moving)
			{
				DragMove(num);
			}
			e.StopPropagation();
		}
	}

	private void ProcessUpEvent(EventBase evt, Vector2 localPosition, int pointerId)
	{
		active = false;
		base.target.ReleasePointer(pointerId);
		if (!(evt is IPointerEvent))
		{
			base.target.panel.ProcessPointerCapture(pointerId);
		}
		EndDragMove(cancelled: false);
		evt.StopPropagation();
	}

	private void BeginDragMove(float pos)
	{
		float num = 0f;
		List<VisualElement> tabHeaders = m_TabView.tabHeaders;
		m_TabToMove = m_TabView.tabHeaders[0];
		foreach (VisualElement item in tabHeaders)
		{
			num += (isVertical ? TabLayout.GetHeight(item) : TabLayout.GetWidth(item));
			if (num > pos)
			{
				m_TabToMove = item;
				break;
			}
		}
		moving = true;
		m_LastPos = pos;
		m_PreviewElement = new TabDragPreview();
		m_LocationPreviewElement = new TabDragLocationPreview
		{
			classList = { isVertical ? TabDragLocationPreview.verticalUssClassName : TabDragLocationPreview.horizontalUssClassName }
		};
		m_Header.hierarchy.Add(m_PreviewElement);
		m_Header.Add(m_LocationPreviewElement);
		int index = m_TabView.tabHeaders.IndexOf(m_TabToMove);
		Tab activeTab = m_TabView.tabs[index];
		m_TabView.activeTab = activeTab;
		m_TabToMovePos = tabLayout.GetTabOffset(m_TabToMove);
		UpdateMoveLocation();
	}

	private void DragMove(float pos)
	{
		m_LastPos = pos;
		UpdateMoveLocation();
	}

	private void UpdatePreviewPosition()
	{
		float num = m_TabToMovePos + m_LastPos - m_StartPos;
		float width = TabLayout.GetWidth(m_TabToMove);
		float tabOffset = tabLayout.GetTabOffset(m_DestinationTab);
		float num2 = (isVertical ? TabLayout.GetHeight(m_DestinationTab) : TabLayout.GetWidth(m_DestinationTab));
		float num3 = ((!m_MoveBeforeDestination) ? num2 : 0f);
		if (isVertical)
		{
			m_PreviewElement.style.top = num;
			m_PreviewElement.style.height = TabLayout.GetHeight(m_TabToMove);
			m_PreviewElement.style.width = width;
			if (m_DestinationTab != null)
			{
				m_LocationPreviewElement.preview.style.width = width;
				m_LocationPreviewElement.style.top = tabOffset + num3;
			}
		}
		else
		{
			m_PreviewElement.style.left = num;
			m_PreviewElement.style.width = width;
			if (m_DestinationTab != null)
			{
				m_LocationPreviewElement.style.left = tabOffset + num3;
			}
		}
	}

	private void UpdateMoveLocation()
	{
		float num = 0f;
		m_DestinationTab = null;
		m_MoveBeforeDestination = false;
		foreach (VisualElement tabHeader in m_TabView.tabHeaders)
		{
			m_DestinationTab = tabHeader;
			float num2 = (isVertical ? TabLayout.GetHeight(m_DestinationTab) : TabLayout.GetWidth(m_DestinationTab));
			float num3 = num + num2 / 2f;
			num += num2;
			if (num > m_LastPos)
			{
				m_MoveBeforeDestination = m_LastPos < num3;
				break;
			}
		}
		UpdatePreviewPosition();
	}

	private void EndDragMove(bool cancelled)
	{
		if (!moving || m_Cancelled)
		{
			return;
		}
		m_Cancelled = cancelled;
		if (!cancelled)
		{
			int num = m_TabView.tabHeaders.IndexOf(m_TabToMove);
			int num2 = m_TabView.tabHeaders.IndexOf(m_DestinationTab);
			if (!m_MoveBeforeDestination)
			{
				num2++;
			}
			if (num < num2)
			{
				num2--;
			}
			if (num != num2)
			{
				tabLayout.ReorderDisplay(num, num2);
			}
		}
		m_PreviewElement?.RemoveFromHierarchy();
		m_PreviewElement = null;
		m_LocationPreviewElement?.RemoveFromHierarchy();
		m_LocationPreviewElement = null;
		moving = false;
		m_TabToMove = null;
	}
}
