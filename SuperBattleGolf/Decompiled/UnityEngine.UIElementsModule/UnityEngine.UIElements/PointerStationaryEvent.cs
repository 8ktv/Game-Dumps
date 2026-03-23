using System;

namespace UnityEngine.UIElements;

[Obsolete("Not sent by input backend.")]
public sealed class PointerStationaryEvent : PointerEventBase<PointerStationaryEvent>
{
	static PointerStationaryEvent()
	{
		EventBase<PointerStationaryEvent>.SetCreateFunction(() => new PointerStationaryEvent());
	}

	protected override void Init()
	{
		base.Init();
		LocalInit();
	}

	private void LocalInit()
	{
		base.propagation = EventPropagation.BubblesOrTricklesDown;
		base.recomputeTopElementUnderPointer = true;
	}

	public PointerStationaryEvent()
	{
		LocalInit();
	}

	internal override void Dispatch(BaseVisualElementPanel panel)
	{
		EventDispatchUtilities.DispatchToCapturingElementOrElementUnderPointer(this, panel, base.pointerId, base.position);
	}
}
