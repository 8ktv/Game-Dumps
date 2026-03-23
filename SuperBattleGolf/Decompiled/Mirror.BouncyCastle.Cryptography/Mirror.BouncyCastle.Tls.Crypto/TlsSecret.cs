namespace Mirror.BouncyCastle.Tls.Crypto;

public interface TlsSecret
{
	int Length { get; }

	byte[] CalculateHmac(int cryptoHashAlgorithm, byte[] buf, int off, int len);

	TlsSecret DeriveUsingPrf(int prfAlgorithm, string label, byte[] seed, int length);

	void Destroy();

	byte[] Encrypt(TlsEncryptor encryptor);

	byte[] Extract();

	TlsSecret HkdfExpand(int cryptoHashAlgorithm, byte[] info, int length);

	TlsSecret HkdfExtract(int cryptoHashAlgorithm, TlsSecret ikm);

	bool IsAlive();
}
