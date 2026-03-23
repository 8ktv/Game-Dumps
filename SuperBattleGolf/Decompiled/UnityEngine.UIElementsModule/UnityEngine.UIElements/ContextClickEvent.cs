namespace UnityEngine.UIElements;

public class ContextClickEvent : MouseEventBase<ContextClickEvent>
{
	static ContextClickEvent()
	{
		EventBase<ContextClickEvent>.SetCreateFunction(() => new ContextClickEvent());
	}

	public ContextClickEvent()
	{
		LocalInit();
	}

	protected override void Init()
	{
		base.Init();
		LocalInit();
	}

	private void LocalInit()
	{
	}
}
