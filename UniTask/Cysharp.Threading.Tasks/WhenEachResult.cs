using System;
using System.Runtime.ExceptionServices;

namespace Cysharp.Threading.Tasks;

public readonly struct WhenEachResult<T>
{
	public T Result { get; }

	public Exception Exception { get; }

	public bool IsCompletedSuccessfully => Exception == null;

	public bool IsFaulted => Exception != null;

	public WhenEachResult(T result)
	{
		Result = result;
		Exception = null;
	}

	public WhenEachResult(Exception exception)
	{
		if (exception == null)
		{
			throw new ArgumentNullException("exception");
		}
		Result = default(T);
		Exception = exception;
	}

	public void TryThrow()
	{
		if (IsFaulted)
		{
			ExceptionDispatchInfo.Capture(Exception).Throw();
		}
	}

	public T GetResult()
	{
		if (IsFaulted)
		{
			ExceptionDispatchInfo.Capture(Exception).Throw();
		}
		return Result;
	}

	public override string ToString()
	{
		if (IsCompletedSuccessfully)
		{
			T result = Result;
			return ((result != null) ? result.ToString() : null) ?? "";
		}
		return "Exception{" + Exception.Message + "}";
	}
}
