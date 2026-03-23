using System;
using System.Runtime.Serialization;
using Mirror.BouncyCastle.Security;

namespace Mirror.BouncyCastle.Pkix;

[Serializable]
public class PkixCertPathValidatorException : GeneralSecurityException
{
	protected readonly int m_index = -1;

	public int Index => m_index;

	public PkixCertPathValidatorException()
	{
	}

	public PkixCertPathValidatorException(string message)
		: base(message)
	{
	}

	public PkixCertPathValidatorException(string message, Exception innerException)
		: base(message, innerException)
	{
	}

	public PkixCertPathValidatorException(string message, Exception innerException, int index)
		: base(message, innerException)
	{
		if (index < -1)
		{
			throw new ArgumentException("cannot be < -1", "index");
		}
		m_index = index;
	}

	protected PkixCertPathValidatorException(SerializationInfo info, StreamingContext context)
		: base(info, context)
	{
		m_index = info.GetInt32("index");
	}

	public override void GetObjectData(SerializationInfo info, StreamingContext context)
	{
		base.GetObjectData(info, context);
		info.AddValue("index", m_index);
	}
}
