using System;

public class LateBUpdateCallback : ILateBUpdateCallback, IAnyBUpdateCallback, IDisposable
{
	private readonly Action Callback;

	public LateBUpdateCallback(Action Callback)
	{
		this.Callback = Callback;
		BUpdate.RegisterCallback(this);
	}

	public void Dispose()
	{
		BUpdate.DeregisterCallback(this);
	}

	public void OnLateBUpdate()
	{
		Callback();
	}
}
