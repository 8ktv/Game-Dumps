using System;
using System.Runtime.Serialization;

namespace Mirror.BouncyCastle.Tls;

[Serializable]
public class TlsFatalAlert : TlsException
{
	protected readonly byte m_alertDescription;

	public virtual short AlertDescription => m_alertDescription;

	private static string GetMessage(short alertDescription, string detailMessage)
	{
		string text = Mirror.BouncyCastle.Tls.AlertDescription.GetText(alertDescription);
		if (detailMessage != null)
		{
			text = text + "; " + detailMessage;
		}
		return text;
	}

	public TlsFatalAlert(short alertDescription)
		: this(alertDescription, null, null)
	{
	}

	public TlsFatalAlert(short alertDescription, string detailMessage)
		: this(alertDescription, detailMessage, null)
	{
	}

	public TlsFatalAlert(short alertDescription, Exception alertCause)
		: this(alertDescription, null, alertCause)
	{
	}

	public TlsFatalAlert(short alertDescription, string detailMessage, Exception alertCause)
		: base(GetMessage(alertDescription, detailMessage), alertCause)
	{
		if (!TlsUtilities.IsValidUint8(alertDescription))
		{
			throw new ArgumentOutOfRangeException("alertDescription");
		}
		m_alertDescription = (byte)alertDescription;
	}

	protected TlsFatalAlert(SerializationInfo info, StreamingContext context)
		: base(info, context)
	{
		m_alertDescription = info.GetByte("alertDescription");
	}

	public override void GetObjectData(SerializationInfo info, StreamingContext context)
	{
		base.GetObjectData(info, context);
		info.AddValue("alertDescription", m_alertDescription);
	}
}
