using System.Collections.Generic;
using Mirror.BouncyCastle.Math;
using Mirror.BouncyCastle.Security;

namespace Mirror.BouncyCastle.Tls.Crypto;

public interface TlsCrypto
{
	SecureRandom SecureRandom { get; }

	bool HasAnyStreamVerifiers(IList<SignatureAndHashAlgorithm> signatureAndHashAlgorithms);

	bool HasAnyStreamVerifiersLegacy(short[] clientCertificateTypes);

	bool HasCryptoHashAlgorithm(int cryptoHashAlgorithm);

	bool HasCryptoSignatureAlgorithm(int cryptoSignatureAlgorithm);

	bool HasDHAgreement();

	bool HasECDHAgreement();

	bool HasEncryptionAlgorithm(int encryptionAlgorithm);

	bool HasHkdfAlgorithm(int cryptoHashAlgorithm);

	bool HasMacAlgorithm(int macAlgorithm);

	bool HasNamedGroup(int namedGroup);

	bool HasRsaEncryption();

	bool HasSignatureAlgorithm(short signatureAlgorithm);

	bool HasSignatureAndHashAlgorithm(SignatureAndHashAlgorithm sigAndHashAlgorithm);

	bool HasSignatureScheme(int signatureScheme);

	bool HasSrpAuthentication();

	TlsSecret CreateSecret(byte[] data);

	TlsSecret GenerateRsaPreMasterSecret(ProtocolVersion clientVersion);

	TlsCertificate CreateCertificate(byte[] encoding);

	TlsCertificate CreateCertificate(short type, byte[] encoding);

	TlsCipher CreateCipher(TlsCryptoParameters cryptoParams, int encryptionAlgorithm, int macAlgorithm);

	TlsDHDomain CreateDHDomain(TlsDHConfig dhConfig);

	TlsECDomain CreateECDomain(TlsECConfig ecConfig);

	TlsSecret AdoptSecret(TlsSecret secret);

	TlsHash CreateHash(int cryptoHashAlgorithm);

	TlsHmac CreateHmac(int macAlgorithm);

	TlsHmac CreateHmacForHash(int cryptoHashAlgorithm);

	TlsNonceGenerator CreateNonceGenerator(byte[] additionalSeedMaterial);

	TlsSrp6Client CreateSrp6Client(TlsSrpConfig srpConfig);

	TlsSrp6Server CreateSrp6Server(TlsSrpConfig srpConfig, BigInteger srpVerifier);

	TlsSrp6VerifierGenerator CreateSrp6VerifierGenerator(TlsSrpConfig srpConfig);

	TlsSecret HkdfInit(int cryptoHashAlgorithm);
}
