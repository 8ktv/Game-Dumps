using System;
using Mirror.BouncyCastle.Security;
using Mirror.BouncyCastle.Utilities;

namespace Mirror.BouncyCastle.Pqc.Crypto.Crystals.Kyber;

internal class KyberEngine
{
	private SecureRandom m_random;

	private KyberIndCpa m_indCpa;

	public const int N = 256;

	public const int Q = 3329;

	public const int QInv = 62209;

	public static int SymBytes = 32;

	private const int SharedSecretBytes = 32;

	public static int PolyBytes = 384;

	public const int Eta2 = 2;

	public int IndCpaMsgBytes = SymBytes;

	public Symmetric Symmetric { get; private set; }

	public int K { get; private set; }

	public int PolyVecBytes { get; private set; }

	public int PolyCompressedBytes { get; private set; }

	public int PolyVecCompressedBytes { get; private set; }

	public int Eta1 { get; private set; }

	public int IndCpaPublicKeyBytes { get; private set; }

	public int IndCpaSecretKeyBytes { get; private set; }

	public int IndCpaBytes { get; private set; }

	public int PublicKeyBytes { get; private set; }

	public int SecretKeyBytes { get; private set; }

	public int CipherTextBytes { get; private set; }

	public int CryptoBytes { get; private set; }

	public int CryptoSecretKeyBytes { get; private set; }

	public int CryptoPublicKeyBytes { get; private set; }

	public int CryptoCipherTextBytes { get; private set; }

	public KyberEngine(int k, bool usingAes)
	{
		K = k;
		switch (k)
		{
		case 2:
			Eta1 = 3;
			PolyCompressedBytes = 128;
			PolyVecCompressedBytes = K * 320;
			break;
		case 3:
			Eta1 = 2;
			PolyCompressedBytes = 128;
			PolyVecCompressedBytes = K * 320;
			break;
		case 4:
			Eta1 = 2;
			PolyCompressedBytes = 160;
			PolyVecCompressedBytes = K * 352;
			break;
		}
		PolyVecBytes = k * PolyBytes;
		IndCpaPublicKeyBytes = PolyVecBytes + SymBytes;
		IndCpaSecretKeyBytes = PolyVecBytes;
		IndCpaBytes = PolyVecCompressedBytes + PolyCompressedBytes;
		PublicKeyBytes = IndCpaPublicKeyBytes;
		SecretKeyBytes = IndCpaSecretKeyBytes + IndCpaPublicKeyBytes + 2 * SymBytes;
		CipherTextBytes = IndCpaBytes;
		CryptoBytes = 32;
		CryptoSecretKeyBytes = SecretKeyBytes;
		CryptoPublicKeyBytes = PublicKeyBytes;
		CryptoCipherTextBytes = CipherTextBytes;
		if (usingAes)
		{
			Symmetric = new Symmetric.AesSymmetric();
		}
		else
		{
			Symmetric = new Symmetric.ShakeSymmetric();
		}
		m_indCpa = new KyberIndCpa(this);
	}

	internal void Init(SecureRandom random)
	{
		m_random = random;
	}

	internal void GenerateKemKeyPair(out byte[] t, out byte[] rho, out byte[] s, out byte[] hpk, out byte[] nonce)
	{
		m_indCpa.GenerateKeyPair(out var pk, out var sk);
		s = Arrays.CopyOfRange(sk, 0, IndCpaSecretKeyBytes);
		hpk = new byte[32];
		Symmetric.Hash_h(hpk, pk, 0);
		nonce = new byte[SymBytes];
		m_random.NextBytes(nonce);
		t = Arrays.CopyOfRange(pk, 0, IndCpaPublicKeyBytes - 32);
		rho = Arrays.CopyOfRange(pk, IndCpaPublicKeyBytes - 32, IndCpaPublicKeyBytes);
	}

	internal void KemEncrypt(byte[] cipherText, byte[] sharedSecret, byte[] pk)
	{
		byte[] array = new byte[SymBytes];
		byte[] array2 = new byte[2 * SymBytes];
		byte[] array3 = new byte[2 * SymBytes];
		m_random.NextBytes(array, 0, SymBytes);
		Array.Copy(array, 0, array2, 0, SymBytes);
		Symmetric.Hash_h(array2, pk, SymBytes);
		Symmetric.Hash_g(array3, array2);
		m_indCpa.Encrypt(cipherText, Arrays.CopyOfRange(array2, 0, SymBytes), pk, Arrays.CopyOfRange(array3, SymBytes, 2 * SymBytes));
		Array.Copy(array3, 0, sharedSecret, 0, sharedSecret.Length);
	}

	internal void KemDecrypt(byte[] sharedSecret, byte[] cipherText, byte[] secretKey)
	{
		byte[] array = new byte[2 * SymBytes];
		byte[] array2 = new byte[2 * SymBytes];
		byte[] array3 = new byte[CipherTextBytes];
		byte[] pk = Arrays.CopyOfRange(secretKey, IndCpaSecretKeyBytes, secretKey.Length);
		m_indCpa.Decrypt(array, cipherText, secretKey);
		Array.Copy(secretKey, SecretKeyBytes - 2 * SymBytes, array, SymBytes, SymBytes);
		Symmetric.Hash_g(array2, array);
		m_indCpa.Encrypt(array3, Arrays.CopyOf(array, SymBytes), pk, Arrays.CopyOfRange(array2, SymBytes, array2.Length));
		bool b = !Arrays.FixedTimeEquals(cipherText, array3);
		Symmetric.Hash_h(array2, cipherText, SymBytes);
		CMov(array2, Arrays.CopyOfRange(secretKey, SecretKeyBytes - SymBytes, SecretKeyBytes), SymBytes, b);
		Array.Copy(array2, 0, sharedSecret, 0, sharedSecret.Length);
	}

	private void CMov(byte[] r, byte[] x, int len, bool b)
	{
		if (b)
		{
			Array.Copy(x, 0, r, 0, len);
		}
		else
		{
			Array.Copy(r, 0, r, 0, len);
		}
	}

	internal void RandomBytes(byte[] buf, int len)
	{
		m_random.NextBytes(buf, 0, len);
	}
}
