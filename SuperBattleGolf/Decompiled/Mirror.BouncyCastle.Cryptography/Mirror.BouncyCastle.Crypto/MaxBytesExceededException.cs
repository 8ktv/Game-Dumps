using System;
using System.Runtime.Serialization;

namespace Mirror.BouncyCastle.Crypto;

[Serializable]
public class MaxBytesExceededException : CryptoException
{
	public MaxBytesExceededException()
	{
	}

	public MaxBytesExceededException(string message)
		: base(message)
	{
	}

	public MaxBytesExceededException(string message, Exception innerException)
		: base(message, innerException)
	{
	}

	protected MaxBytesExceededException(SerializationInfo info, StreamingContext context)
		: base(info, context)
	{
	}
}
