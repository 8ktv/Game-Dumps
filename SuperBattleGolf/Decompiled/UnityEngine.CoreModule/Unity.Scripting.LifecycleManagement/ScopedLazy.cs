using System;

namespace Unity.Scripting.LifecycleManagement;

internal sealed class ScopedLazy<TValue, TScope> where TValue : class
{
	private Lazy<TValue> _data;

	public TValue Value => _data.Value;

	public ScopedLazy(Func<TValue> factory, bool checkScopeActive = true)
	{
		_data = new Lazy<TValue>(factory);
	}

	public ScopedLazy(bool checkScopeActive = true)
		: this((Func<TValue>)Activator.CreateInstance<TValue>, checkScopeActive)
	{
	}

	public void Cleanup()
	{
		_data = null;
	}
}
