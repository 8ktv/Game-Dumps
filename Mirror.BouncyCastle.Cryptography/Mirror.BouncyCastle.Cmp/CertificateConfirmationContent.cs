using System;
using Mirror.BouncyCastle.Asn1.Cmp;
using Mirror.BouncyCastle.Cms;
using Mirror.BouncyCastle.Operators.Utilities;

namespace Mirror.BouncyCastle.Cmp;

public class CertificateConfirmationContent
{
	private readonly CertConfirmContent m_content;

	private readonly IDigestAlgorithmFinder m_digestAlgorithmFinder;

	public static CertificateConfirmationContent FromPkiBody(PkiBody pkiBody)
	{
		return FromPkiBody(pkiBody, DefaultDigestAlgorithmFinder.Instance);
	}

	public static CertificateConfirmationContent FromPkiBody(PkiBody pkiBody, IDigestAlgorithmFinder digestAlgorithmFinder)
	{
		if (!IsCertificateConfirmationContent(pkiBody.Type))
		{
			throw new ArgumentException("content of PKIBody wrong type: " + pkiBody.Type);
		}
		return new CertificateConfirmationContent(CertConfirmContent.GetInstance(pkiBody.Content), digestAlgorithmFinder);
	}

	public static bool IsCertificateConfirmationContent(int bodyType)
	{
		return 24 == bodyType;
	}

	public CertificateConfirmationContent(CertConfirmContent content)
		: this(content, DefaultDigestAlgorithmFinder.Instance)
	{
	}

	[Obsolete("Use constructor taking 'IDigestAlgorithmFinder' instead")]
	public CertificateConfirmationContent(CertConfirmContent content, DefaultDigestAlgorithmIdentifierFinder digestAlgFinder)
		: this(content, (IDigestAlgorithmFinder)digestAlgFinder)
	{
	}

	public CertificateConfirmationContent(CertConfirmContent content, IDigestAlgorithmFinder digestAlgorithmFinder)
	{
		m_content = content;
		m_digestAlgorithmFinder = digestAlgorithmFinder;
	}

	public CertConfirmContent ToAsn1Structure()
	{
		return m_content;
	}

	public CertificateStatus[] GetStatusMessages()
	{
		return Array.ConvertAll(m_content.ToCertStatusArray(), (CertStatus element) => new CertificateStatus(m_digestAlgorithmFinder, element));
	}
}
