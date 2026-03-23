using Mirror.BouncyCastle.Security;

namespace Mirror.BouncyCastle.Pqc.Crypto.Cmce;

internal interface ICmceEngine
{
	int CipherTextSize { get; }

	int DefaultSessionKeySize { get; }

	int PrivateKeySize { get; }

	int PublicKeySize { get; }

	byte[] DecompressPrivateKey(byte[] sk);

	byte[] GeneratePublicKeyFromPrivateKey(byte[] sk);

	int KemDec(byte[] key, byte[] cipher_text, byte[] sk);

	int KemEnc(byte[] cipher_text, byte[] key, byte[] pk, SecureRandom random);

	void KemKeypair(byte[] pk, byte[] sk, SecureRandom random);
}
