using System;
using System.Runtime.Serialization;

namespace Mirror.BouncyCastle.Pqc.Crypto.Lms;

[Serializable]
public class LmsException : Exception
{
	public LmsException()
	{
	}

	public LmsException(string message)
		: base(message)
	{
	}

	public LmsException(string message, Exception innerException)
		: base(message, innerException)
	{
	}

	protected LmsException(SerializationInfo info, StreamingContext context)
		: base(info, context)
	{
	}
}
