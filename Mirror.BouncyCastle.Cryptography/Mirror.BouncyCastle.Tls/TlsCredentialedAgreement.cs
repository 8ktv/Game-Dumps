using Mirror.BouncyCastle.Tls.Crypto;

namespace Mirror.BouncyCastle.Tls;

public interface TlsCredentialedAgreement : TlsCredentials
{
	TlsSecret GenerateAgreement(TlsCertificate peerCertificate);
}
