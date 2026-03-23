using Mirror.BouncyCastle.Asn1;
using Mirror.BouncyCastle.Math;

namespace Mirror.BouncyCastle.Tls.Crypto;

public interface TlsCertificate
{
	BigInteger SerialNumber { get; }

	string SigAlgOid { get; }

	TlsEncryptor CreateEncryptor(int tlsCertificateRole);

	TlsVerifier CreateVerifier(short signatureAlgorithm);

	Tls13Verifier CreateVerifier(int signatureScheme);

	byte[] GetEncoded();

	byte[] GetExtension(DerObjectIdentifier extensionOid);

	Asn1Encodable GetSigAlgParams();

	short GetLegacySignatureAlgorithm();

	bool SupportsSignatureAlgorithm(short signatureAlgorithm);

	bool SupportsSignatureAlgorithmCA(short signatureAlgorithm);

	TlsCertificate CheckUsageInRole(int tlsCertificateRole);
}
