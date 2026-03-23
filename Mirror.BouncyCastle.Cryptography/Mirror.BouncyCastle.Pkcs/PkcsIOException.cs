using System;
using System.IO;
using System.Runtime.Serialization;

namespace Mirror.BouncyCastle.Pkcs;

[Serializable]
public class PkcsIOException : IOException
{
	public PkcsIOException()
	{
	}

	public PkcsIOException(string message)
		: base(message)
	{
	}

	public PkcsIOException(string message, Exception innerException)
		: base(message, innerException)
	{
	}

	protected PkcsIOException(SerializationInfo info, StreamingContext context)
		: base(info, context)
	{
	}
}
