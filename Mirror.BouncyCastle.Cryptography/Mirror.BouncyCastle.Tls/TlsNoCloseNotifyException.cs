using System;
using System.IO;
using System.Runtime.Serialization;

namespace Mirror.BouncyCastle.Tls;

[Serializable]
public class TlsNoCloseNotifyException : EndOfStreamException
{
	public TlsNoCloseNotifyException()
		: base("No close_notify alert received before connection closed")
	{
	}

	protected TlsNoCloseNotifyException(SerializationInfo info, StreamingContext context)
		: base(info, context)
	{
	}
}
