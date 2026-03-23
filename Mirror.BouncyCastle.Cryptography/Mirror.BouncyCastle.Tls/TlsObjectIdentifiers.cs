using Mirror.BouncyCastle.Asn1;
using Mirror.BouncyCastle.Asn1.X509;

namespace Mirror.BouncyCastle.Tls;

public abstract class TlsObjectIdentifiers
{
	public static readonly DerObjectIdentifier id_pe_tlsfeature = X509ObjectIdentifiers.IdPE.Branch("24");
}
