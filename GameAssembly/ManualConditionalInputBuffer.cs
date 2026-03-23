using System;

public class ManualConditionalInputBuffer : IInputBuffer
{
	private readonly float duration;

	private Func<bool> TryUseBufferedInput;

	private readonly Action OnDeactivated;

	private float timeSinceActivated;

	public bool IsActive { get; private set; }

	public ManualConditionalInputBuffer(float duration, Action onDeactivated)
	{
		this.duration = duration;
		OnDeactivated = onDeactivated;
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

	public void Activate(Func<bool> TryUseInputOnActivation, Func<bool> TryUseBufferedInput)
	{
		this.TryUseBufferedInput = TryUseBufferedInput;
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
	}
}
