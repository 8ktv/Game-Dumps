using System;

public class PostFixedBUpdateCallback : IPostFixedBUpdateCallback, IAnyBUpdateCallback, IDisposable
{
	private readonly Action Callback;

	public PostFixedBUpdateCallback(Action Callback)
	{
		this.Callback = Callback;
		BUpdate.RegisterCallback(this);
	}

	public void Dispose()
	{
		BUpdate.DeregisterCallback(this);
	}

	public void OnPostFixedBUpdate()
	{
		Callback();
	}
}
