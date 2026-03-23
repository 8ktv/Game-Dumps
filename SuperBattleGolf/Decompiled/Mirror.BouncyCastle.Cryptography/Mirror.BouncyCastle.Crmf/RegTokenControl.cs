using Mirror.BouncyCastle.Asn1;
using Mirror.BouncyCastle.Asn1.Crmf;

namespace Mirror.BouncyCastle.Crmf;

public class RegTokenControl : IControl
{
	private readonly DerUtf8String m_token;

	public DerObjectIdentifier Type => CrmfObjectIdentifiers.id_regCtrl_regToken;

	public Asn1Encodable Value => m_token;

	public RegTokenControl(DerUtf8String token)
	{
		m_token = token;
	}

	public RegTokenControl(string token)
	{
		m_token = new DerUtf8String(token);
	}
}
