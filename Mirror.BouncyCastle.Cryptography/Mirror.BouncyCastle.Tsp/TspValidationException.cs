using System;
using System.Runtime.Serialization;

namespace Mirror.BouncyCastle.Tsp;

[Serializable]
public class TspValidationException : TspException
{
	protected readonly int m_failureCode;

	public int FailureCode => m_failureCode;

	public TspValidationException(string message)
		: this(message, -1)
	{
	}

	public TspValidationException(string message, int failureCode)
		: base(message)
	{
		m_failureCode = failureCode;
	}

	protected TspValidationException(SerializationInfo info, StreamingContext context)
		: base(info, context)
	{
		m_failureCode = info.GetInt32("failureCode");
	}

	public override void GetObjectData(SerializationInfo info, StreamingContext context)
	{
		base.GetObjectData(info, context);
		info.AddValue("failureCode", m_failureCode);
	}
}
