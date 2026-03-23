using System;
using Mirror.BouncyCastle.Asn1.X509;
using Mirror.BouncyCastle.Security.Certificates;

namespace Mirror.BouncyCastle.X509;

public class X509CertificatePair
{
	private readonly X509Certificate m_forward;

	private readonly X509Certificate m_reverse;

	public X509Certificate Forward => m_forward;

	public X509Certificate Reverse => m_reverse;

	public X509CertificatePair(X509Certificate forward, X509Certificate reverse)
	{
		if (forward == null && reverse == null)
		{
			throw new ArgumentException("At least one of the pair shall be present");
		}
		m_forward = forward;
		m_reverse = reverse;
	}

	public X509CertificatePair(CertificatePair pair)
	{
		X509CertificateStructure forward = pair.Forward;
		X509CertificateStructure reverse = pair.Reverse;
		m_forward = ((forward == null) ? null : new X509Certificate(forward));
		m_reverse = ((reverse == null) ? null : new X509Certificate(reverse));
	}

	public CertificatePair GetCertificatePair()
	{
		return new CertificatePair(m_forward?.CertificateStructure, m_reverse?.CertificateStructure);
	}

	public byte[] GetEncoded()
	{
		try
		{
			return GetCertificatePair().GetEncoded("DER");
		}
		catch (Exception ex)
		{
			throw new CertificateEncodingException(ex.Message, ex);
		}
	}

	public override bool Equals(object obj)
	{
		if (obj == this)
		{
			return true;
		}
		if (!(obj is X509CertificatePair x509CertificatePair))
		{
			return false;
		}
		if (object.Equals(m_forward, x509CertificatePair.m_forward))
		{
			return object.Equals(m_reverse, x509CertificatePair.m_reverse);
		}
		return false;
	}

	public override int GetHashCode()
	{
		int num = -1;
		if (m_forward != null)
		{
			num ^= m_forward.GetHashCode();
		}
		if (m_reverse != null)
		{
			num *= 17;
			num ^= m_reverse.GetHashCode();
		}
		return num;
	}
}
