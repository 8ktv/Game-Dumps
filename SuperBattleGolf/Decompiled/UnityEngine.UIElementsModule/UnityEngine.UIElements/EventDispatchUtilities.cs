#define UNITY_ASSERTIONS
using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using JetBrains.Annotations;
using UnityEngine.Pool;
using UnityEngine.UIElements.Experimental;

namespace UnityEngine.UIElements;

internal static class EventDispatchUtilities
{
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	private static void PropagateEvent(EventBase evt, [NotNull] BaseVisualElementPanel panel, [NotNull] VisualElement target, bool isCapturingTarget)
	{
		if ((evt as IPointerEventInternal)?.compatibilityMouseEvent is EventBase eventBase)
		{
			eventBase.AssignTimeStamp(evt.timestamp);
			HandleEventAcrossPropagationPathWithCompatibilityEvent(evt, eventBase, panel, target, isCapturingTarget);
		}
		else
		{
			HandleEventAcrossPropagationPath(evt, panel, target, isCapturingTarget);
		}
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static void SendEventDirectlyToTarget(EventBase evt, BaseVisualElementPanel panel, [NotNull] VisualElement target)
	{
		evt.elementTarget = target;
		evt.AssignTimeStamp(target.TimeSinceStartupMs());
		HandleEventAtTargetAndDefaultPhase(evt, panel, target);
	}

	public static void HandleEventAtTargetAndDefaultPhase(EventBase evt, BaseVisualElementPanel panel, VisualElement target)
	{
		int eventCategories = evt.eventCategories;
		if (!target.HasSelfEventInterests(eventCategories) || evt.isPropagationStopped)
		{
			return;
		}
		evt.currentTarget = target;
		try
		{
			Debug.Assert(!(evt is IPointerEventInternal pointerEventInternal) || pointerEventInternal.compatibilityMouseEvent == null, "!(evt is IPointerEventInternal pe) || pe.compatibilityMouseEvent == null");
			evt.propagationPhase = PropagationPhase.TrickleDown;
			if (target.HasTrickleDownEventCallbacks(eventCategories))
			{
				HandleEvent_TrickleDownCallbacks(evt, panel, target);
				if (evt.isImmediatePropagationStopped)
				{
					return;
				}
			}
			if (target.HasTrickleDownHandleEvent(eventCategories))
			{
				HandleEvent_TrickleDownHandleEvent(evt, panel, target, Disabled(evt, target));
			}
			if (evt.isPropagationStopped)
			{
				return;
			}
			evt.propagationPhase = PropagationPhase.BubbleUp;
			if (target.HasBubbleUpHandleEvent(eventCategories))
			{
				bool disabled = Disabled(evt, target);
				HandleEvent_DefaultActionAtTarget(evt, panel, target, disabled);
				HandleEvent_BubbleUpHandleEvent(evt, panel, target, disabled);
				HandleEvent_DefaultAction(evt, panel, target, disabled);
				if (evt.isImmediatePropagationStopped)
				{
					return;
				}
			}
			if (target.HasBubbleUpEventCallbacks(eventCategories))
			{
				HandleEvent_BubbleUpCallbacks(evt, panel, target);
			}
		}
		finally
		{
			evt.currentTarget = null;
			evt.propagationPhase = PropagationPhase.None;
		}
	}

	private static void HandleEventAcrossPropagationPath(EventBase evt, [NotNull] BaseVisualElementPanel panel, [NotNull] VisualElement target, bool isCapturingTarget)
	{
		int eventCategories = evt.eventCategories;
		if (!target.HasParentEventInterests(eventCategories) || evt.isPropagationStopped)
		{
			return;
		}
		using PropagationPaths propagationPaths = PropagationPaths.Build(target, evt, eventCategories);
		try
		{
			Debug.Assert(!evt.dispatch, "Event is being dispatched recursively.");
			evt.dispatch = true;
			evt.propagationPhase = PropagationPhase.TrickleDown;
			int num = propagationPaths.trickleDownPath.Count - 1;
			if (isCapturingTarget && num >= 0)
			{
				num = ((propagationPaths.trickleDownPath[0] != target) ? (-1) : 0);
			}
			while (num >= 0)
			{
				VisualElement visualElement = (VisualElement)(evt.currentTarget = propagationPaths.trickleDownPath[num]);
				if (visualElement.HasTrickleDownEventCallbacks(eventCategories))
				{
					HandleEvent_TrickleDownCallbacks(evt, panel, visualElement);
					if (evt.isImmediatePropagationStopped)
					{
						return;
					}
				}
				if (visualElement.HasTrickleDownHandleEvent(eventCategories))
				{
					HandleEvent_TrickleDownHandleEvent(evt, panel, visualElement, Disabled(evt, visualElement));
				}
				if (evt.isPropagationStopped)
				{
					return;
				}
				num--;
			}
			evt.propagationPhase = PropagationPhase.BubbleUp;
			using List<VisualElement>.Enumerator enumerator = propagationPaths.bubbleUpPath.GetEnumerator();
			while (enumerator.MoveNext())
			{
				VisualElement visualElement2 = (VisualElement)(evt.currentTarget = enumerator.Current);
				if (visualElement2.HasBubbleUpHandleEvent(eventCategories))
				{
					HandleEvent_BubbleUpAllDefaultActions(evt, panel, visualElement2, Disabled(evt, visualElement2), isCapturingTarget);
					if (evt.isImmediatePropagationStopped)
					{
						break;
					}
				}
				if (visualElement2.HasBubbleUpEventCallbacks(eventCategories) && (!isCapturingTarget || visualElement2 == target))
				{
					HandleEvent_BubbleUpCallbacks(evt, panel, visualElement2);
				}
				if (evt.isPropagationStopped)
				{
					break;
				}
			}
		}
		finally
		{
			evt.currentTarget = null;
			evt.propagationPhase = PropagationPhase.None;
			evt.dispatch = false;
		}
	}

	private static void HandleEventAcrossPropagationPathWithCompatibilityEvent(EventBase evt, [NotNull] EventBase compatibilityEvt, [NotNull] BaseVisualElementPanel panel, [NotNull] VisualElement target, bool isCapturingTarget)
	{
		int eventCategories = evt.eventCategories | compatibilityEvt.eventCategories;
		if (!target.HasParentEventInterests(eventCategories) || evt.isPropagationStopped || compatibilityEvt.isPropagationStopped)
		{
			return;
		}
		compatibilityEvt.elementTarget = target;
		compatibilityEvt.skipDisabledElements = evt.skipDisabledElements;
		using PropagationPaths propagationPaths = PropagationPaths.Build(target, evt, eventCategories);
		try
		{
			Debug.Assert(!evt.dispatch, "Event is being dispatched recursively.");
			evt.dispatch = true;
			evt.propagationPhase = PropagationPhase.TrickleDown;
			compatibilityEvt.propagationPhase = PropagationPhase.TrickleDown;
			int num = propagationPaths.trickleDownPath.Count - 1;
			if (isCapturingTarget && num >= 0)
			{
				num = ((propagationPaths.trickleDownPath[0] != target) ? (-1) : 0);
			}
			while (num >= 0)
			{
				VisualElement visualElement = (VisualElement)(compatibilityEvt.currentTarget = (evt.currentTarget = propagationPaths.trickleDownPath[num]));
				if (visualElement.HasTrickleDownEventCallbacks(eventCategories))
				{
					HandleEvent_TrickleDownCallbacks(evt, panel, visualElement);
					if (evt.isImmediatePropagationStopped)
					{
						return;
					}
					if (panel.ShouldSendCompatibilityMouseEvents((IPointerEvent)evt))
					{
						HandleEvent_TrickleDownCallbacks(compatibilityEvt, panel, visualElement);
						if (evt.isImmediatePropagationStopped)
						{
							return;
						}
					}
				}
				if (visualElement.HasTrickleDownHandleEvent(eventCategories))
				{
					bool disabled = Disabled(evt, visualElement);
					HandleEvent_TrickleDownHandleEvent(evt, panel, visualElement, disabled);
					if (evt.isImmediatePropagationStopped)
					{
						return;
					}
					if (panel.ShouldSendCompatibilityMouseEvents((IPointerEvent)evt))
					{
						HandleEvent_TrickleDownHandleEvent(compatibilityEvt, panel, visualElement, disabled);
						if (compatibilityEvt.isImmediatePropagationStopped)
						{
							return;
						}
					}
				}
				if (evt.isPropagationStopped || compatibilityEvt.isPropagationStopped)
				{
					return;
				}
				num--;
			}
			evt.propagationPhase = PropagationPhase.BubbleUp;
			compatibilityEvt.propagationPhase = PropagationPhase.BubbleUp;
			using List<VisualElement>.Enumerator enumerator = propagationPaths.bubbleUpPath.GetEnumerator();
			while (enumerator.MoveNext())
			{
				VisualElement visualElement2 = (VisualElement)(compatibilityEvt.currentTarget = (evt.currentTarget = enumerator.Current));
				if (visualElement2.HasBubbleUpHandleEvent(eventCategories))
				{
					bool disabled2 = Disabled(evt, visualElement2);
					HandleEvent_BubbleUpAllDefaultActions(evt, panel, visualElement2, disabled2, isCapturingTarget);
					if (evt.isImmediatePropagationStopped)
					{
						break;
					}
					if (panel.ShouldSendCompatibilityMouseEvents((IPointerEvent)evt))
					{
						HandleEvent_BubbleUpAllDefaultActions(compatibilityEvt, panel, visualElement2, disabled2, isCapturingTarget);
						if (compatibilityEvt.isImmediatePropagationStopped)
						{
							break;
						}
					}
				}
				if (visualElement2.HasBubbleUpEventCallbacks(eventCategories) && (!isCapturingTarget || visualElement2 == target))
				{
					HandleEvent_BubbleUpCallbacks(evt, panel, visualElement2);
					if (evt.isImmediatePropagationStopped)
					{
						break;
					}
					if (panel.ShouldSendCompatibilityMouseEvents((IPointerEvent)evt))
					{
						HandleEvent_BubbleUpCallbacks(compatibilityEvt, panel, visualElement2);
						if (compatibilityEvt.isImmediatePropagationStopped)
						{
							break;
						}
					}
				}
				if (evt.isPropagationStopped || compatibilityEvt.isPropagationStopped)
				{
					break;
				}
			}
		}
		finally
		{
			evt.currentTarget = null;
			evt.propagationPhase = PropagationPhase.None;
			compatibilityEvt.currentTarget = null;
			compatibilityEvt.propagationPhase = PropagationPhase.None;
			evt.dispatch = false;
		}
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	private static void HandleEvent_DefaultActionAtTarget(EventBase evt, [NotNull] BaseVisualElementPanel panel, [NotNull] VisualElement element, bool disabled)
	{
		if (element.elementPanel != panel)
		{
			return;
		}
		using (new EventDebuggerLogExecuteDefaultAction(evt))
		{
			if (disabled)
			{
				element.ExecuteDefaultActionDisabledAtTargetInternal(evt);
			}
			else
			{
				element.ExecuteDefaultActionAtTargetInternal(evt);
			}
		}
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	private static void HandleEvent_DefaultAction(EventBase evt, [NotNull] BaseVisualElementPanel panel, [NotNull] VisualElement element, bool disabled)
	{
		if (element.elementPanel != panel)
		{
			return;
		}
		using (new EventDebuggerLogExecuteDefaultAction(evt))
		{
			if (disabled)
			{
				element.ExecuteDefaultActionDisabledInternal(evt);
			}
			else
			{
				element.ExecuteDefaultActionInternal(evt);
			}
		}
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	private static void HandleEvent_TrickleDownCallbacks(EventBase evt, [NotNull] BaseVisualElementPanel panel, [NotNull] VisualElement element)
	{
		element.m_CallbackRegistry?.m_TrickleDownCallbacks.Invoke(evt, panel, element);
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	private static void HandleEvent_BubbleUpCallbacks(EventBase evt, [NotNull] BaseVisualElementPanel panel, [NotNull] VisualElement element)
	{
		element.m_CallbackRegistry?.m_BubbleUpCallbacks.Invoke(evt, panel, element);
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	private static void HandleEvent_TrickleDownHandleEvent(EventBase evt, [NotNull] BaseVisualElementPanel panel, [NotNull] VisualElement element, bool disabled)
	{
		if (element.elementPanel == panel)
		{
			if (disabled)
			{
				element.HandleEventTrickleDownDisabled(evt);
			}
			else
			{
				element.HandleEventTrickleDownInternal(evt);
			}
		}
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	private static void HandleEvent_BubbleUpHandleEvent(EventBase evt, [NotNull] BaseVisualElementPanel panel, [NotNull] VisualElement element, bool disabled)
	{
		if (element.elementPanel == panel)
		{
			if (disabled)
			{
				element.HandleEventBubbleUpDisabled(evt);
			}
			else
			{
				element.HandleEventBubbleUpInternal(evt);
			}
		}
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	private static void HandleEvent_BubbleUpAllDefaultActions(EventBase evt, [NotNull] BaseVisualElementPanel panel, [NotNull] VisualElement element, bool disabled, bool isCapturingTarget)
	{
		bool flag = element == evt.target || !isCapturingTarget;
		bool flag2 = element == evt.target || element.isCompositeRoot;
		if (flag2)
		{
			HandleEvent_DefaultActionAtTarget(evt, panel, element, disabled);
		}
		if (flag)
		{
			HandleEvent_BubbleUpHandleEvent(evt, panel, element, disabled);
		}
		if (flag2)
		{
			HandleEvent_DefaultAction(evt, panel, element, disabled);
		}
	}

	private static bool Disabled([NotNull] EventBase evt, [NotNull] VisualElement target)
	{
		return evt.skipDisabledElements && !target.enabledInHierarchy;
	}

	public static void HandleEvent([NotNull] EventBase evt, [NotNull] VisualElement target)
	{
		if (evt.isPropagationStopped)
		{
			return;
		}
		BaseVisualElementPanel elementPanel = target.elementPanel;
		bool disabled = Disabled(evt, target);
		switch (evt.propagationPhase)
		{
		case PropagationPhase.TrickleDown:
			HandleEvent_TrickleDownCallbacks(evt, elementPanel, target);
			if (!evt.isImmediatePropagationStopped)
			{
				HandleEvent_TrickleDownHandleEvent(evt, elementPanel, target, disabled);
			}
			break;
		case PropagationPhase.BubbleUp:
			HandleEvent_BubbleUpAllDefaultActions(evt, elementPanel, target, disabled, isCapturingTarget: false);
			if (!evt.isImmediatePropagationStopped)
			{
				HandleEvent_BubbleUpCallbacks(evt, elementPanel, target);
			}
			break;
		}
	}

	public static void DispatchToFocusedElementOrPanelRoot(EventBase evt, [NotNull] BaseVisualElementPanel panel)
	{
		bool flag = false;
		VisualElement visualElement = evt.elementTarget;
		if (visualElement == null)
		{
			Focusable leafFocusedElement = panel.focusController.GetLeafFocusedElement();
			if (leafFocusedElement is VisualElement visualElement2)
			{
				visualElement = visualElement2;
			}
			else
			{
				visualElement = panel.visualTree;
				flag = true;
			}
			if (panel.GetCapturingElement(PointerId.mousePointerId) is VisualElement visualElement3 && visualElement3 != visualElement && !visualElement3.Contains(visualElement) && visualElement3.HasSelfEventInterests(evt.eventCategories))
			{
				evt.elementTarget = visualElement3;
				bool skipDisabledElements = evt.skipDisabledElements;
				evt.skipDisabledElements = false;
				HandleEventAtTargetAndDefaultPhase(evt, panel, visualElement3);
				evt.skipDisabledElements = skipDisabledElements;
			}
			evt.elementTarget = visualElement;
		}
		PropagateEvent(evt, panel, visualElement, isCapturingTarget: false);
		if (flag && evt.propagateToIMGUI)
		{
			PropagateToRemainingIMGUIContainers(evt, panel.visualTree);
		}
	}

	public static void DispatchToElementUnderPointerOrPanelRoot(EventBase evt, [NotNull] BaseVisualElementPanel panel, int pointerId, Vector2 position)
	{
		bool flag = false;
		VisualElement visualElement = evt.elementTarget;
		if (visualElement == null)
		{
			visualElement = panel.GetTopElementUnderPointer(pointerId);
			if (visualElement == null)
			{
				visualElement = panel.visualTree;
				flag = true;
			}
			evt.elementTarget = visualElement;
		}
		PropagateEvent(evt, panel, visualElement, isCapturingTarget: false);
		if (flag && evt.propagateToIMGUI)
		{
			PropagateToRemainingIMGUIContainers(evt, panel.visualTree);
		}
	}

	public static void DispatchToAssignedTarget(EventBase evt, [NotNull] BaseVisualElementPanel panel)
	{
		VisualElement elementTarget = evt.elementTarget;
		if (elementTarget == null)
		{
			throw new ArgumentException($"Event target not set. Event type {evt.GetType()} requires a target.");
		}
		PropagateEvent(evt, panel, elementTarget, isCapturingTarget: false);
	}

	public static void DefaultDispatch(EventBase evt, [NotNull] BaseVisualElementPanel panel)
	{
		VisualElement elementTarget = evt.elementTarget;
		if (elementTarget != null)
		{
			if (evt.bubblesOrTricklesDown)
			{
				PropagateEvent(evt, panel, elementTarget, isCapturingTarget: false);
			}
			else
			{
				HandleEventAtTargetAndDefaultPhase(evt, panel, elementTarget);
			}
		}
	}

	public static void DispatchToCapturingElementOrElementUnderPointer(EventBase evt, [NotNull] BaseVisualElementPanel panel, int pointerId, Vector2 position)
	{
		if (!DispatchToCapturingElement(evt, panel, pointerId))
		{
			DispatchToElementUnderPointerOrPanelRoot(evt, panel, pointerId, position);
		}
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	private static bool DispatchToCapturingElement(EventBase evt, [NotNull] BaseVisualElementPanel panel, int pointerId)
	{
		VisualElement visualElement = panel.GetCapturingElement(pointerId) as VisualElement;
		if (visualElement == null)
		{
			if (!(evt is IPointerEventInternal { compatibilityMouseEvent: not null }))
			{
				return false;
			}
			visualElement = panel.GetCapturingElement(PointerId.mousePointerId) as VisualElement;
			if (visualElement == null)
			{
				return false;
			}
		}
		if (evt.target != null && evt.target != visualElement)
		{
			return false;
		}
		if (visualElement.panel != panel)
		{
			return false;
		}
		evt.skipDisabledElements = false;
		evt.elementTarget = visualElement;
		PropagateEvent(evt, panel, visualElement, isCapturingTarget: true);
		return true;
	}

	internal static void DispatchToPanelRoot(EventBase evt, [NotNull] BaseVisualElementPanel panel)
	{
		VisualElement visualElement = (evt.elementTarget = panel.visualTree);
		VisualElement target = visualElement;
		PropagateEvent(evt, panel, target, isCapturingTarget: false);
	}

	internal static void PropagateToRemainingIMGUIContainers(EventBase evt, [NotNull] VisualElement root)
	{
		if (evt.imguiEvent != null && root.elementPanel.contextType != ContextType.Player)
		{
			PropagateToRemainingIMGUIContainerRecursive(evt, root);
		}
	}

	private static void PropagateToRemainingIMGUIContainerRecursive(EventBase evt, [NotNull] VisualElement root)
	{
		if (root.isIMGUIContainer)
		{
			if (root != evt.target)
			{
				IMGUIContainer iMGUIContainer = (IMGUIContainer)root;
				bool flag = evt.elementTarget?.focusable ?? false;
				if (iMGUIContainer.SendEventToIMGUI(evt, !flag))
				{
					evt.StopPropagation();
				}
				if (evt.imguiEvent.rawType == EventType.Used)
				{
					Debug.Assert(evt.isPropagationStopped, "evt.isPropagationStopped");
				}
			}
		}
		else
		{
			if (root.imguiContainerDescendantCount <= 0)
			{
				return;
			}
			List<VisualElement> value;
			using (CollectionPool<List<VisualElement>, VisualElement>.Get(out value))
			{
				value.AddRange(root.hierarchy.children);
				foreach (VisualElement item in value)
				{
					if (item.hierarchy.parent == root)
					{
						PropagateToRemainingIMGUIContainerRecursive(evt, item);
						if (evt.isPropagationStopped)
						{
							break;
						}
					}
				}
			}
		}
	}
}
