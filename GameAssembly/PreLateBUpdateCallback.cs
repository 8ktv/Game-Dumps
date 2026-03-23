using System;

public class PreLateBUpdateCallback : IPreLateBUpdateCallback, IAnyBUpdateCallback, IDisposable
{
	private readonly Action Callback;

	public PreLateBUpdateCallback(Action Callback)
	{
		this.Callback = Callback;
		BUpdate.RegisterCallback(this);
	}

	public void Dispose()
	{
		BUpdate.DeregisterCallback(this);
	}

	public void OnPreLateBUpdate()
	{
		Callback();
	}
}
