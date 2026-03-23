using System;
using UnityEngine.InputSystem;

public class FullDurationInputBuffer : IInputBuffer
{
	private readonly float duration;

	private readonly Action OnActivated;

	private readonly Action OnTimedOut;

	private readonly InputAction inputAction;

	private float timeSinceActivated;

	public bool IsActive { get; private set; }

	public FullDurationInputBuffer(InputAction action, Action OnActivated, float duration, Action OnTimedOut)
	{
		inputAction = action;
		this.duration = duration;
		this.OnActivated = OnActivated;
		this.OnTimedOut = OnTimedOut;
		action.performed += Activate;
	}

	public void Update(float deltaTime)
	{
		if (IsActive)
		{
			timeSinceActivated += deltaTime;
			if (timeSinceActivated > duration)
			{
				OnTimedOut?.Invoke();
				IsActive = false;
			}
		}
	}

	public void TryUseInput()
	{
	}

	public void Cancel()
	{
		IsActive = false;
	}

	private void Activate(InputAction.CallbackContext context)
	{
		timeSinceActivated = 0f;
		OnActivated();
		IsActive = true;
	}

	public void OnDestroy()
	{
		inputAction.performed -= Activate;
	}
}
