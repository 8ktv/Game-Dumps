using Mirror.BouncyCastle.Asn1.Cmp;
using Mirror.BouncyCastle.Asn1.Crmf;
using Mirror.BouncyCastle.Asn1.X509;
using Mirror.BouncyCastle.X509;

namespace Mirror.BouncyCastle.Crmf;

internal class PKMacValueVerifier
{
	private readonly PKMacBuilder m_builder;

	internal PKMacValueVerifier(PKMacBuilder builder)
	{
		m_builder = builder;
	}

	internal virtual bool IsValid(PKMacValue value, char[] password, SubjectPublicKeyInfo keyInfo)
	{
		m_builder.SetParameters(PbmParameter.GetInstance(value.AlgID.Parameters));
		return X509Utilities.VerifyMac(m_builder.Build(password), keyInfo, value.MacValue);
	}
}
