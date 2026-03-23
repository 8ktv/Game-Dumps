using System;
using System.Runtime.Serialization;

namespace Mirror.BouncyCastle.Bcpg.OpenPgp;

[Serializable]
public class PgpKeyValidationException : PgpException
{
	public PgpKeyValidationException()
	{
	}

	public PgpKeyValidationException(string message)
		: base(message)
	{
	}

	public PgpKeyValidationException(string message, Exception innerException)
		: base(message, innerException)
	{
	}

	protected PgpKeyValidationException(SerializationInfo info, StreamingContext context)
		: base(info, context)
	{
	}
}
