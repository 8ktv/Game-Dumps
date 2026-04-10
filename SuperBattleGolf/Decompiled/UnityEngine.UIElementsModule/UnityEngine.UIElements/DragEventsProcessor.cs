#define UNITY_ASSERTIONS
using UnityEngine.Assertions;

namespace UnityEngine.UIElements;

internal abstract class DragEventsProcessor
{
	internal enum DragState
	{
		None,
		CanStartDrag,
		Dragging
	}

	private bool m_IsRegistered;

	private DragState m_DragState;

	private Vector3 m_Start;

	private bool m_PendingPerformDrag;

	protected readonly VisualElement m_Target;

	internal bool isRegistered => m_IsRegistered;

	internal DragState dragState => m_DragState;

	protected virtual bool supportsDragEvents => true;

	private bool useDragEvents => isEditorContext && supportsDragEvents;

	protected IDragAndDrop dragAndDrop => DragAndDropUtility.GetDragAndDrop(m_Target.panel);

	internal virtual bool isEditorContext
	{
		get
		{
			Assert.IsNotNull(m_Target);
			Assert.IsNotNull(m_Target.parent);
			return m_Target.panel.contextType == ContextType.Editor;
		}
	}

	internal DragEventsProcessor(VisualElement target)
	{
		m_Target = target;
		m_Target.RegisterCallback<AttachToPanelEvent>(RegisterCallbacksFromTarget);
		m_Target.RegisterCallback<DetachFromPanelEvent>(UnregisterCallbacksFromTarget);
		RegisterCallbacksFromTarget();
	}

	private void RegisterCallbacksFromTarget(AttachToPanelEvent evt)
	{
		RegisterCallbacksFromTarget();
	}

	private void RegisterCallbacksFromTarget()
	{
		if (!m_IsRegistered)
		{
			m_IsRegistered = true;
			m_Target.RegisterCallback<PointerDownEvent>(OnPointerDownEvent);
			m_Target.RegisterCallback<PointerUpEvent>(OnPointerUpEvent, TrickleDown.TrickleDown);
			m_Target.RegisterCallback<PointerLeaveEvent>(OnPointerLeaveEvent);
			m_Target.RegisterCallback<PointerMoveEvent>(OnPointerMoveEvent);
			m_Target.RegisterCallback<PointerCancelEvent>(OnPointerCancelEvent);
			m_Target.RegisterCallback<PointerCaptureOutEvent>(OnPointerCapturedOut);
			m_Target.RegisterCallback<PointerOutEvent>(OnPointerOutEvent);
			m_Target.RegisterCallback<GeometryChangedEvent>(OnGeometryChanged);
		}
	}

	private void UnregisterCallbacksFromTarget(DetachFromPanelEvent evt)
	{
		UnregisterCallbacksFromTarget();
	}

	internal void UnregisterCallbacksFromTarget(bool unregisterPanelEvents = false)
	{
		m_IsRegistered = false;
		m_Target.UnregisterCallback<PointerDownEvent>(OnPointerDownEvent);
		m_Target.UnregisterCallback<PointerUpEvent>(OnPointerUpEvent, TrickleDown.TrickleDown);
		m_Target.UnregisterCallback<PointerLeaveEvent>(OnPointerLeaveEvent);
		m_Target.UnregisterCallback<PointerMoveEvent>(OnPointerMoveEvent);
		m_Target.UnregisterCallback<PointerCancelEvent>(OnPointerCancelEvent);
		m_Target.UnregisterCallback<PointerCaptureOutEvent>(OnPointerCapturedOut);
		m_Target.UnregisterCallback<PointerOutEvent>(OnPointerOutEvent);
		m_Target.UnregisterCallback<GeometryChangedEvent>(OnGeometryChanged);
		if (unregisterPanelEvents)
		{
			m_Target.UnregisterCallback<AttachToPanelEvent>(RegisterCallbacksFromTarget);
			m_Target.UnregisterCallback<DetachFromPanelEvent>(UnregisterCallbacksFromTarget);
		}
	}

	protected abstract bool CanStartDrag(Vector3 pointerPosition, EventModifiers modifiers);

	protected internal abstract StartDragArgs StartDrag(Vector3 pointerPosition, EventModifiers modifiers);

	protected internal abstract void UpdateDrag(Vector3 pointerPosition, EventModifiers modifiers);

	protected internal abstract void OnDrop(Vector3 pointerPosition, EventModifiers modifiers);

	protected abstract void ClearDragAndDropUI(bool dragCancelled);

	private void OnPointerDownEvent(PointerDownEvent evt)
	{
		if (evt.button != 0)
		{
			m_DragState = DragState.None;
		}
		else if (CanStartDrag(evt.position, evt.modifiers))
		{
			m_DragState = DragState.CanStartDrag;
			m_Start = evt.position;
		}
	}

	private void OnPointerOutEvent(PointerOutEvent evt)
	{
		if (m_DragState == DragState.CanStartDrag && !((m_Start - evt.position).sqrMagnitude <= 0f))
		{
			m_PendingPerformDrag = true;
			evt.StopPropagation();
		}
	}

	internal void OnPointerUpEvent(PointerUpEvent evt)
	{
		if (!useDragEvents && m_DragState == DragState.Dragging)
		{
			DragEventsProcessor dragEventsProcessor = GetDropTarget(evt.position) ?? this;
			dragEventsProcessor.UpdateDrag(evt.position, evt.modifiers);
			dragEventsProcessor.OnDrop(evt.position, evt.modifiers);
			dragEventsProcessor.ClearDragAndDropUI(dragCancelled: false);
			evt.StopPropagation();
		}
		m_Target.ReleasePointer(evt.pointerId);
		ClearDragAndDropUI(m_DragState == DragState.Dragging);
		dragAndDrop.DragCleanup();
		m_DragState = DragState.None;
		m_PendingPerformDrag = false;
	}

	private void OnPointerLeaveEvent(PointerLeaveEvent evt)
	{
		ClearDragAndDropUI(dragCancelled: false);
	}

	private void OnPointerCancelEvent(PointerCancelEvent evt)
	{
		CancelDragAndDrop(evt.pointerId);
	}

	private void OnPointerCapturedOut(PointerCaptureOutEvent evt)
	{
		CancelDragAndDrop();
	}

	private void OnGeometryChanged(GeometryChangedEvent evt)
	{
		if (m_Target.resolvedStyle.display == DisplayStyle.None)
		{
			CancelDragAndDrop();
		}
	}

	private void CancelDragAndDrop(int releaseCapturePointerId = -1)
	{
		if (m_DragState != DragState.None || m_PendingPerformDrag)
		{
			if (!useDragEvents)
			{
				ClearDragAndDropUI(dragCancelled: true);
			}
			if (releaseCapturePointerId != -1)
			{
				m_Target.ReleasePointer(releaseCapturePointerId);
			}
			ClearDragAndDropUI(m_DragState == DragState.Dragging);
			dragAndDrop.DragCleanup();
			m_DragState = DragState.None;
			m_PendingPerformDrag = false;
		}
	}

	private void OnPointerMoveEvent(PointerMoveEvent evt)
	{
		if (evt.isHandledByDraggable)
		{
			return;
		}
		if (!useDragEvents && m_DragState == DragState.Dragging)
		{
			DragEventsProcessor dragEventsProcessor = GetDropTarget(evt.position) ?? this;
			dragEventsProcessor.UpdateDrag(evt.position, evt.modifiers);
			m_PendingPerformDrag = false;
		}
		else
		{
			if (m_DragState != DragState.CanStartDrag || (!((m_Start - evt.position).sqrMagnitude >= 100f) && !m_PendingPerformDrag))
			{
				return;
			}
			StartDragArgs args = StartDrag(m_Start, evt.modifiers);
			if (args.visualMode == DragVisualMode.Rejected)
			{
				m_DragState = DragState.None;
				return;
			}
			if (!useDragEvents)
			{
				if (supportsDragEvents)
				{
					dragAndDrop.StartDrag(args, evt.position);
				}
			}
			else
			{
				if (Event.current != null && Event.current.type != EventType.MouseDown && Event.current.type != EventType.MouseDrag)
				{
					return;
				}
				dragAndDrop.StartDrag(args, evt.position);
			}
			m_DragState = DragState.Dragging;
			m_Target.CapturePointer(evt.pointerId);
			evt.isHandledByDraggable = true;
			m_PendingPerformDrag = false;
			evt.StopPropagation();
		}
	}

	private DragEventsProcessor GetDropTarget(Vector2 position)
	{
		DragEventsProcessor result = null;
		if (m_Target.worldBound.Contains(position))
		{
			result = this;
		}
		else if (supportsDragEvents)
		{
			result = (m_Target.elementPanel.Pick(position)?.GetFirstOfType<BaseVerticalCollectionView>())?.dragger;
		}
		return result;
	}
}
