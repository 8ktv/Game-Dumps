using System;

public class FixedBUpdateCallback : IFixedBUpdateCallback, IAnyBUpdateCallback, IDisposable
{
	private readonly Action Callback;

	public FixedBUpdateCallback(Action Callback)
	{
		this.Callback = Callback;
		BUpdate.RegisterCallback(this);
	}

	public void Dispose()
	{
		BUpdate.DeregisterCallback(this);
	}

	public void OnFixedBUpdate()
	{
		Callback();
	}
}
