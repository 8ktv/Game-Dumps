using System;
using System.ComponentModel;
using UnityEngine.Scripting;

namespace UnityEngine.InputSystem.Interactions;

[Serializable]
[Preserve]
[DisplayName("PressAndRepeat")]
public class PressAndRepeatInteraction : IInputInteraction
{
	public float holdTime = 0.4f;

	public float repeatTime = 0.2f;

	public bool press;

	private InputInteractionContext ctx;

	private float heldTime;

	private bool firstEventSend;

	private float nextEventTime;

	static PressAndRepeatInteraction()
	{
		RegisterInteraction();
	}

	[RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterAssembliesLoaded)]
	private static void RegisterInteraction()
	{
		if (InputSystem.TryGetInteraction("PressAndRepeat") == null)
		{
			InputSystem.RegisterInteraction<PressAndRepeatInteraction>("PressAndRepeat");
		}
	}

	public void Process(ref InputInteractionContext context)
	{
		ctx = context;
		if (ctx.phase != InputActionPhase.Performed && ctx.phase != InputActionPhase.Started && ctx.ControlIsActuated(0.5f))
		{
			ctx.Started();
			if (press)
			{
				ctx.PerformedAndStayStarted();
			}
			InputSystem.onAfterUpdate -= OnUpdate;
			InputSystem.onAfterUpdate += OnUpdate;
		}
	}

	private void OnUpdate()
	{
		bool flag = ctx.ControlIsActuated(0.5f);
		InputActionPhase phase = ctx.phase;
		heldTime += Time.deltaTime;
		if (phase == InputActionPhase.Canceled || phase == InputActionPhase.Disabled || !ctx.action.actionMap.enabled || (!flag && (phase == InputActionPhase.Performed || phase == InputActionPhase.Started)))
		{
			Cancel(ref ctx);
		}
		else
		{
			if (heldTime < holdTime)
			{
				return;
			}
			if (!firstEventSend)
			{
				ctx.PerformedAndStayStarted();
				nextEventTime = heldTime + repeatTime;
				firstEventSend = true;
			}
			else
			{
				while (heldTime >= nextEventTime)
				{
					ctx.PerformedAndStayStarted();
					nextEventTime = heldTime + repeatTime;
				}
			}
		}
	}

	public void Reset()
	{
		Cancel(ref ctx);
	}

	private void Cancel(ref InputInteractionContext context)
	{
		InputSystem.onAfterUpdate -= OnUpdate;
		heldTime = 0f;
		firstEventSend = false;
		if (context.phase != InputActionPhase.Canceled)
		{
			context.Canceled();
		}
	}
}
