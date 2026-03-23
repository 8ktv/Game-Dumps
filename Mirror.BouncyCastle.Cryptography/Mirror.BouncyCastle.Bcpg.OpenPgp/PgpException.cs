using System;
using System.Runtime.Serialization;

namespace Mirror.BouncyCastle.Bcpg.OpenPgp;

[Serializable]
public class PgpException : Exception
{
	public PgpException()
	{
	}

	public PgpException(string message)
		: base(message)
	{
	}

	public PgpException(string message, Exception innerException)
		: base(message, innerException)
	{
	}

	protected PgpException(SerializationInfo info, StreamingContext context)
		: base(info, context)
	{
	}
}
