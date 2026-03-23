public interface IInputBuffer
{
	public enum ActionState
	{
		Performed,
		Started,
		Cancelled
	}

	bool IsActive { get; }

	void Update(float deltaTime);

	void TryUseInput();

	void Cancel();

	void OnDestroy();
}
