namespace UnityEngine.UIElements;

public class FocusOutEvent : FocusEventBase<FocusOutEvent>
{
	static FocusOutEvent()
	{
		EventBase<FocusOutEvent>.SetCreateFunction(() => new FocusOutEvent());
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

	public FocusOutEvent()
	{
		LocalInit();
	}

	protected internal override void PostDispatch(IPanel panel)
	{
		if (base.relatedTarget == null)
		{
			base.focusController.ProcessPendingFocusChange(null);
		}
		base.PostDispatch(panel);
	}
}
