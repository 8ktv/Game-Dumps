using System.Collections.Generic;
using UnityEngine.Bindings;
using UnityEngine.Pool;

namespace UnityEngine.UIElements;

public class FocusController
{
	private struct FocusedElement
	{
		public VisualElement m_SubTreeRoot;

		public VisualElement m_FocusedElement;
	}

	private TextElement m_SelectedTextElement;

	private List<FocusedElement> m_FocusedElements = new List<FocusedElement>();

	private Focusable m_LastFocusedElement;

	[VisibleToOtherModules(new string[] { "UnityEditor.UIBuilderModule" })]
	internal Focusable m_LastPendingFocusedElement;

	private int m_PendingFocusCount = 0;

	private IFocusRing focusRing { get; }

	[VisibleToOtherModules(new string[] { "UnityEditor.UIBuilderModule" })]
	internal TextElement selectedTextElement
	{
		get
		{
			return m_SelectedTextElement;
		}
		set
		{
			if (m_SelectedTextElement != value)
			{
				m_SelectedTextElement?.selection.SelectNone();
				m_SelectedTextElement = value;
			}
		}
	}

	public Focusable focusedElement
	{
		get
		{
			Focusable retargetedFocusedElement = GetRetargetedFocusedElement(null);
			return IsLocalElement(retargetedFocusedElement) ? retargetedFocusedElement : null;
		}
	}

	internal int imguiKeyboardControl { get; set; }

	public FocusController(IFocusRing focusRing)
	{
		this.focusRing = focusRing;
		imguiKeyboardControl = 0;
	}

	public void IgnoreEvent(EventBase evt)
	{
		evt.processedByFocusController = true;
		if (evt is IMouseEventInternal { sourcePointerEvent: EventBase sourcePointerEvent })
		{
			sourcePointerEvent.processedByFocusController = true;
		}
	}

	internal bool IsFocused(Focusable f)
	{
		if (!IsLocalElement(f))
		{
			return false;
		}
		foreach (FocusedElement focusedElement in m_FocusedElements)
		{
			if (focusedElement.m_FocusedElement == f)
			{
				return true;
			}
		}
		return false;
	}

	internal Focusable GetRetargetedFocusedElement(VisualElement retargetAgainst)
	{
		VisualElement visualElement = retargetAgainst?.hierarchy.parent;
		if (visualElement == null)
		{
			if (m_FocusedElements.Count > 0)
			{
				return m_FocusedElements[m_FocusedElements.Count - 1].m_FocusedElement;
			}
		}
		else
		{
			while (!visualElement.isCompositeRoot && visualElement.hierarchy.parent != null)
			{
				visualElement = visualElement.hierarchy.parent;
			}
			foreach (FocusedElement focusedElement in m_FocusedElements)
			{
				if (focusedElement.m_SubTreeRoot == visualElement)
				{
					return focusedElement.m_FocusedElement;
				}
			}
		}
		return null;
	}

	[VisibleToOtherModules(new string[] { "UnityEditor.UIBuilderModule" })]
	internal Focusable GetLeafFocusedElement()
	{
		if (m_FocusedElements.Count > 0)
		{
			VisualElement visualElement = m_FocusedElements[0].m_FocusedElement;
			return IsLocalElement(visualElement) ? visualElement : null;
		}
		return null;
	}

	private bool IsLocalElement(Focusable f)
	{
		return f?.focusController == this;
	}

	internal void ClearPendingFocusEvents()
	{
		m_PendingFocusCount = 0;
		m_LastPendingFocusedElement = null;
	}

	[VisibleToOtherModules(new string[] { "UnityEditor.UIBuilderModule" })]
	internal bool IsPendingFocus(Focusable f)
	{
		for (VisualElement visualElement = m_LastPendingFocusedElement as VisualElement; visualElement != null; visualElement = visualElement.hierarchy.parent)
		{
			if (f == visualElement)
			{
				return true;
			}
		}
		return false;
	}

	internal void SetFocusToLastFocusedElement()
	{
		if (m_LastFocusedElement != null && !(m_LastFocusedElement is IMGUIContainer))
		{
			m_LastFocusedElement.Focus();
		}
	}

	[VisibleToOtherModules(new string[] { "UnityEditor.UIBuilderModule" })]
	internal void BlurLastFocusedElement()
	{
		selectedTextElement = null;
		if (m_LastFocusedElement != null && !(m_LastFocusedElement is IMGUIContainer))
		{
			Focusable lastFocusedElement = m_LastFocusedElement;
			m_LastFocusedElement.Blur();
			m_LastFocusedElement = lastFocusedElement;
		}
	}

	[VisibleToOtherModules(new string[] { "UnityEditor.UIBuilderModule" })]
	internal void DoFocusChange(Focusable f)
	{
		m_FocusedElements.Clear();
		GetFocusTargets(f, m_FocusedElements);
	}

	internal void ProcessPendingFocusChange(Focusable f)
	{
		m_PendingFocusCount--;
		if (m_PendingFocusCount == 0)
		{
			m_LastPendingFocusedElement = null;
		}
		foreach (FocusedElement focusedElement in m_FocusedElements)
		{
			focusedElement.m_FocusedElement.pseudoStates &= ~PseudoStates.Focus;
		}
		DoFocusChange(f);
		foreach (FocusedElement focusedElement2 in m_FocusedElements)
		{
			focusedElement2.m_FocusedElement.pseudoStates |= PseudoStates.Focus;
		}
	}

	private static void GetFocusTargets(Focusable f, List<FocusedElement> outTargets)
	{
		VisualElement visualElement = f as VisualElement;
		for (VisualElement visualElement2 = visualElement; visualElement2 != null; visualElement2 = visualElement2.hierarchy.parent)
		{
			if (visualElement2.hierarchy.parent == null || visualElement2.isCompositeRoot)
			{
				outTargets.Add(new FocusedElement
				{
					m_SubTreeRoot = visualElement2,
					m_FocusedElement = visualElement
				});
				visualElement = visualElement2;
			}
		}
	}

	internal Focusable FocusNextInDirection(Focusable currentFocusable, FocusChangeDirection direction)
	{
		Focusable nextFocusable = focusRing.GetNextFocusable(currentFocusable, direction);
		direction.ApplyTo(this, nextFocusable);
		return nextFocusable;
	}

	private void AboutToReleaseFocus(Focusable focusable, Focusable willGiveFocusTo, FocusChangeDirection direction, DispatchMode dispatchMode)
	{
		using FocusOutEvent e = FocusEventBase<FocusOutEvent>.GetPooled(focusable, willGiveFocusTo, direction, this);
		focusable.SendEvent(e, dispatchMode);
	}

	private void ReleaseFocus(Focusable focusable, Focusable willGiveFocusTo, FocusChangeDirection direction, DispatchMode dispatchMode)
	{
		List<FocusedElement> value;
		using (CollectionPool<List<FocusedElement>, FocusedElement>.Get(out value))
		{
			GetFocusTargets(focusable, value);
			foreach (FocusedElement item in value)
			{
				using BlurEvent blurEvent = FocusEventBase<BlurEvent>.GetPooled(item.m_FocusedElement, willGiveFocusTo, direction, this);
				blurEvent.target = item.m_FocusedElement;
				focusable.SendEvent(blurEvent, dispatchMode);
			}
		}
	}

	private void AboutToGrabFocus(Focusable focusable, Focusable willTakeFocusFrom, FocusChangeDirection direction, DispatchMode dispatchMode)
	{
		using FocusInEvent e = FocusEventBase<FocusInEvent>.GetPooled(focusable, willTakeFocusFrom, direction, this);
		focusable.SendEvent(e, dispatchMode);
	}

	private void GrabFocus(Focusable focusable, Focusable willTakeFocusFrom, FocusChangeDirection direction, bool bIsFocusDelegated, DispatchMode dispatchMode)
	{
		List<FocusedElement> value;
		using (CollectionPool<List<FocusedElement>, FocusedElement>.Get(out value))
		{
			GetFocusTargets(focusable, value);
			foreach (FocusedElement item in value)
			{
				using FocusEvent focusEvent = FocusEventBase<FocusEvent>.GetPooled(item.m_FocusedElement, willTakeFocusFrom, direction, this, bIsFocusDelegated);
				focusEvent.target = item.m_FocusedElement;
				focusable.SendEvent(focusEvent, dispatchMode);
			}
		}
	}

	internal void Blur(Focusable focusable, bool bIsFocusDelegated = false, DispatchMode dispatchMode = DispatchMode.Default)
	{
		if ((m_PendingFocusCount > 0) ? IsPendingFocus(focusable) : IsFocused(focusable))
		{
			SwitchFocus(null, bIsFocusDelegated, dispatchMode);
		}
	}

	internal void SwitchFocus(Focusable newFocusedElement, bool bIsFocusDelegated = false, DispatchMode dispatchMode = DispatchMode.Default)
	{
		SwitchFocus(newFocusedElement, FocusChangeDirection.unspecified, bIsFocusDelegated, dispatchMode);
	}

	internal void SwitchFocus(Focusable newFocusedElement, FocusChangeDirection direction, bool bIsFocusDelegated = false, DispatchMode dispatchMode = DispatchMode.Default)
	{
		m_LastFocusedElement = newFocusedElement;
		Focusable focusable = ((m_PendingFocusCount > 0) ? m_LastPendingFocusedElement : GetLeafFocusedElement());
		if (focusable == newFocusedElement)
		{
			return;
		}
		if (newFocusedElement is VisualElement visualElement && newFocusedElement.canGrabFocus)
		{
			IPanel panel = visualElement.panel;
			if (panel != null)
			{
				Focusable willGiveFocusTo = visualElement?.RetargetElement(focusable as VisualElement) ?? newFocusedElement;
				Focusable willTakeFocusFrom = (focusable as VisualElement)?.RetargetElement(visualElement) ?? focusable;
				m_LastPendingFocusedElement = newFocusedElement;
				m_PendingFocusCount++;
				using (new EventDispatcherGate(panel.dispatcher))
				{
					if (focusable != null)
					{
						AboutToReleaseFocus(focusable, willGiveFocusTo, direction, dispatchMode);
					}
					AboutToGrabFocus(newFocusedElement, willTakeFocusFrom, direction, dispatchMode);
					if (focusable != null)
					{
						ReleaseFocus(focusable, willGiveFocusTo, direction, dispatchMode);
					}
					GrabFocus(newFocusedElement, willTakeFocusFrom, direction, bIsFocusDelegated, dispatchMode);
					return;
				}
			}
		}
		if (!(focusable is VisualElement { elementPanel: var elementPanel }))
		{
			return;
		}
		if (elementPanel != null)
		{
			m_LastPendingFocusedElement = null;
			m_PendingFocusCount++;
			using (new EventDispatcherGate(elementPanel.dispatcher))
			{
				AboutToReleaseFocus(focusable, null, direction, dispatchMode);
				ReleaseFocus(focusable, null, direction, dispatchMode);
				return;
			}
		}
		ProcessPendingFocusChange(null);
	}

	internal void SwitchFocusOnEvent(Focusable currentFocusable, EventBase e)
	{
		if (e.processedByFocusController)
		{
			return;
		}
		using FocusChangeDirection focusChangeDirection = focusRing.GetFocusChangeDirection(currentFocusable, e);
		if (focusChangeDirection != FocusChangeDirection.none)
		{
			FocusNextInDirection(currentFocusable, focusChangeDirection);
			e.processedByFocusController = true;
		}
	}

	internal void ReevaluateFocus()
	{
		if (focusedElement is VisualElement visualElement && (!visualElement.areAncestorsAndSelfDisplayed || !visualElement.visible))
		{
			visualElement.Blur();
		}
	}

	internal bool GetFocusableParentForPointerEvent(Focusable target, out Focusable effectiveTarget)
	{
		if (target == null || !target.focusable)
		{
			effectiveTarget = target;
			return target != null;
		}
		effectiveTarget = target;
		while (effectiveTarget is VisualElement visualElement && (!visualElement.enabledInHierarchy || !visualElement.focusable || !visualElement.isEligibleToReceiveFocusFromDisabledChild) && visualElement.hierarchy.parent != null)
		{
			effectiveTarget = visualElement.hierarchy.parent;
		}
		return !IsFocused(effectiveTarget);
	}

	internal void SyncIMGUIFocus(int imguiKeyboardControlID, Focusable imguiContainerHavingKeyboardControl, bool forceSwitch)
	{
		imguiKeyboardControl = imguiKeyboardControlID;
		if (forceSwitch || imguiKeyboardControl != 0)
		{
			SwitchFocus(imguiContainerHavingKeyboardControl, FocusChangeDirection.unspecified);
		}
		else
		{
			SwitchFocus(null, FocusChangeDirection.unspecified);
		}
	}
}
