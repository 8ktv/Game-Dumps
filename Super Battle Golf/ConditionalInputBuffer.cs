using System;
using UnityEngine.InputSystem;

public class ConditionalInputBuffer : IInputBuffer
{
	private readonly float duration;

	private readonly Func<bool> TryUseInputOnActivation;

	private readonly Func<bool> TryUseBufferedInput;

	private readonly Action OnDeactivated;

	private readonly InputAction inputAction;

	private float timeSinceActivated;

	public bool IsActive { get; private set; }

	public ConditionalInputBuffer(InputAction action, Func<bool> TryUseInputOnActivation, Func<bool> TryUseBufferedInput, float duration, Action OnDeactivated)
	{
		inputAction = action;
		this.duration = duration;
		this.TryUseInputOnActivation = TryUseInputOnActivation;
		this.TryUseBufferedInput = TryUseBufferedInput;
		this.OnDeactivated = OnDeactivated;
		action.performed += Activate;
	}

	public void Update(float deltaTime)
	{
		if (IsActive)
		{
			timeSinceActivated += deltaTime;
			if (timeSinceActivated > duration || TryUseBufferedInput())
			{
				IsActive = false;
				OnDeactivated?.Invoke();
			}
		}
	}

	public void TryUseInput()
	{
		if (IsActive && TryUseBufferedInput())
		{
			IsActive = false;
			OnDeactivated?.Invoke();
		}
	}

	public void Cancel()
	{
		IsActive = false;
	}

	private void Activate(InputAction.CallbackContext context)
	{
		timeSinceActivated = 0f;
		IsActive = true;
		if (TryUseInputOnActivation())
		{
			IsActive = false;
			OnDeactivated?.Invoke();
		}
	}

	public void OnDestroy()
	{
		inputAction.performed -= Activate;
	}
}
