namespace UnityEngine.UIElements;

public class FocusInEvent : FocusEventBase<FocusInEvent>
{
	static FocusInEvent()
	{
		EventBase<FocusInEvent>.SetCreateFunction(() => new FocusInEvent());
	}

	protected override void Init()
	{
		base.Init();
		LocalInit();
	}

	private void LocalInit()
	{
		base.propagation = EventPropagation.BubblesOrTricklesDown;
	}

	public FocusInEvent()
	{
		LocalInit();
	}

	protected internal override void PostDispatch(IPanel panel)
	{
		base.focusController.ProcessPendingFocusChange(base.elementTarget);
		base.PostDispatch(panel);
	}
}
