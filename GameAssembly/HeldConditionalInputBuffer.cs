using System;
using UnityEngine.InputSystem;

public class HeldConditionalInputBuffer : IInputBuffer
{
	private readonly float duration;

	private readonly Func<bool> TryUseInputInternal;

	private readonly Action OnTimedOut;

	private readonly Action OnRelease;

	private readonly InputAction inputAction;

	private float timeSinceHeld;

	private bool holding;

	public bool IsActive { get; private set; }

	public HeldConditionalInputBuffer(InputAction action, Func<bool> TryUseInput, float duration, Action OnRelease, Action OnTimedOut)
	{
		inputAction = action;
		this.duration = duration;
		TryUseInputInternal = TryUseInput;
		this.OnTimedOut = OnTimedOut;
		this.OnRelease = OnRelease;
		action.started += StartHolding;
		action.canceled += StopHolding;
	}

	public void Update(float deltaTime)
	{
		if (IsActive)
		{
			if (!holding)
			{
				timeSinceHeld += deltaTime;
			}
			if (timeSinceHeld > duration)
			{
				IsActive = false;
				holding = false;
				OnTimedOut?.Invoke();
			}
			else if (TryUseInputInternal())
			{
				IsActive = false;
				holding = false;
			}
		}
	}

	public void TryUseInput()
	{
		if (IsActive && TryUseInputInternal())
		{
			IsActive = false;
			holding = false;
		}
	}

	public void Cancel()
	{
		IsActive = false;
	}

	private void StartHolding(InputAction.CallbackContext context)
	{
		bool flag = TryUseInputInternal();
		holding = !flag;
		IsActive = !flag;
		timeSinceHeld = 0f;
	}

	private void StopHolding(InputAction.CallbackContext context)
	{
		timeSinceHeld = 0f;
		holding = false;
		OnRelease?.Invoke();
	}

	public void OnDestroy()
	{
		inputAction.started -= StartHolding;
		inputAction.canceled -= StopHolding;
	}
}
