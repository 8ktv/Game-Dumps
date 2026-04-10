using System;

namespace UnityEngine.UIElements;

public class ContextualMenuManipulator : PointerManipulator
{
	private Action<ContextualMenuPopulateEvent> m_MenuBuilder;

	public ContextualMenuManipulator(Action<ContextualMenuPopulateEvent> menuBuilder)
	{
		m_MenuBuilder = menuBuilder;
		base.activators.Add(new ManipulatorActivationFilter
		{
			button = MouseButton.RightMouse
		});
		if (IsOSXContextualMenuPlatform())
		{
			base.activators.Add(new ManipulatorActivationFilter
			{
				button = MouseButton.LeftMouse,
				modifiers = EventModifiers.Control
			});
		}
	}

	protected override void RegisterCallbacksOnTarget()
	{
		if (IsOSXContextualMenuPlatform())
		{
			base.target.RegisterCallback<PointerDownEvent>(OnPointerDownEventOSX);
			base.target.RegisterCallback<PointerUpEvent>(OnPointerUpEventOSX);
			base.target.RegisterCallback<PointerMoveEvent>(OnPointerMoveEventOSX);
		}
		else
		{
			base.target.RegisterCallback<PointerUpEvent>(OnPointerUpEvent);
			base.target.RegisterCallback<PointerMoveEvent>(OnPointerMoveEvent);
		}
		base.target.RegisterCallback<KeyUpEvent>(OnKeyUpEvent);
		base.target.RegisterCallback<ContextualMenuPopulateEvent>(OnContextualMenuEvent);
	}

	protected override void UnregisterCallbacksFromTarget()
	{
		if (IsOSXContextualMenuPlatform())
		{
			base.target.UnregisterCallback<PointerDownEvent>(OnPointerDownEventOSX);
			base.target.UnregisterCallback<PointerUpEvent>(OnPointerUpEventOSX);
			base.target.UnregisterCallback<PointerMoveEvent>(OnPointerMoveEventOSX);
		}
		else
		{
			base.target.UnregisterCallback<PointerUpEvent>(OnPointerUpEvent);
			base.target.UnregisterCallback<PointerMoveEvent>(OnPointerMoveEvent);
		}
		base.target.UnregisterCallback<KeyUpEvent>(OnKeyUpEvent);
		base.target.UnregisterCallback<ContextualMenuPopulateEvent>(OnContextualMenuEvent);
	}

	protected bool IsOSXContextualMenuPlatform()
	{
		return UIElementsUtility.isOSXContextualMenuPlatform;
	}

	private void OnPointerUpEvent(IPointerEvent evt)
	{
		ProcessPointerEvent(evt);
	}

	private void OnPointerDownEventOSX(IPointerEvent evt)
	{
		ProcessPointerEvent(evt);
	}

	private void OnPointerUpEventOSX(IPointerEvent evt)
	{
		ContextualMenuManager contextualMenuManager = base.target.elementPanel?.contextualMenuManager;
		if (contextualMenuManager == null || !contextualMenuManager.displayMenuHandledOSX)
		{
			ProcessPointerEvent(evt);
		}
	}

	private void OnPointerMoveEvent(PointerMoveEvent evt)
	{
		if (evt.isPointerUp)
		{
			OnPointerUpEvent(evt);
		}
	}

	private void OnPointerMoveEventOSX(PointerMoveEvent evt)
	{
		if (evt.isPointerUp)
		{
			OnPointerUpEventOSX(evt);
		}
		else if (evt.isPointerDown)
		{
			OnPointerDownEventOSX(evt);
		}
	}

	private void ProcessPointerEvent(IPointerEvent evt)
	{
		if (CanStartManipulation(evt))
		{
			DoDisplayMenu(evt as EventBase);
		}
	}

	private void OnKeyUpEvent(KeyUpEvent evt)
	{
		if (evt.keyCode == KeyCode.Menu)
		{
			DoDisplayMenu(evt);
		}
	}

	private void DoDisplayMenu(EventBase evt)
	{
		if (base.target.elementPanel?.contextualMenuManager != null)
		{
			base.target.elementPanel.contextualMenuManager.DisplayMenu(evt, base.target);
			evt.StopPropagation();
		}
	}

	private void OnContextualMenuEvent(ContextualMenuPopulateEvent evt)
	{
		m_MenuBuilder?.Invoke(evt);
	}
}
