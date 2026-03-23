using System;
using System.Runtime.Serialization;

namespace Mirror.BouncyCastle.Utilities;

[Serializable]
public class MemoableResetException : InvalidCastException
{
	public MemoableResetException()
	{
	}

	public MemoableResetException(string message)
		: base(message)
	{
	}

	public MemoableResetException(string message, Exception innerException)
		: base(message, innerException)
	{
	}

	protected MemoableResetException(SerializationInfo info, StreamingContext context)
		: base(info, context)
	{
	}
}
