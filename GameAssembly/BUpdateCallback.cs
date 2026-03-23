using System;

public class BUpdateCallback : IBUpdateCallback, IAnyBUpdateCallback, IDisposable
{
	private readonly Action Callback;

	public BUpdateCallback(Action Callback)
	{
		this.Callback = Callback;
		BUpdate.RegisterCallback(this);
	}

	public void Dispose()
	{
		BUpdate.DeregisterCallback(this);
	}

	public void OnBUpdate()
	{
		Callback();
	}
}
