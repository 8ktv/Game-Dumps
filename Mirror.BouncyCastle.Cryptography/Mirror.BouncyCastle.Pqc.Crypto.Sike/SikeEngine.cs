using System;
using Mirror.BouncyCastle.Crypto;
using Mirror.BouncyCastle.Crypto.Digests;
using Mirror.BouncyCastle.Security;

namespace Mirror.BouncyCastle.Pqc.Crypto.Sike;

internal sealed class SikeEngine
{
	internal Internal param;

	internal Isogeny isogeny;

	internal Fpx fpx;

	private Sidh sidh;

	private SidhCompressed sidhCompressed;

	private bool isCompressed;

	internal uint GetDefaultSessionKeySize()
	{
		return param.MSG_BYTES * 8;
	}

	internal int GetCipherTextSize()
	{
		return param.CRYPTO_CIPHERTEXTBYTES;
	}

	internal uint GetPrivateKeySize()
	{
		return param.CRYPTO_SECRETKEYBYTES;
	}

	internal uint GetPublicKeySize()
	{
		return param.CRYPTO_PUBLICKEYBYTES;
	}

	internal SikeEngine(int ver, bool isCompressed, SecureRandom random)
	{
		this.isCompressed = isCompressed;
		switch (ver)
		{
		case 434:
			param = new P434(isCompressed);
			break;
		case 503:
			param = new P503(isCompressed);
			break;
		case 610:
			param = new P610(isCompressed);
			break;
		case 751:
			param = new P751(isCompressed);
			break;
		}
		fpx = new Fpx(this);
		isogeny = new Isogeny(this);
		if (isCompressed)
		{
			sidhCompressed = new SidhCompressed(this);
		}
		sidh = new Sidh(this);
	}

	internal int crypto_kem_keypair(byte[] pk, byte[] sk, SecureRandom random)
	{
		random.NextBytes(sk, 0, (int)param.MSG_BYTES);
		if (isCompressed)
		{
			random.NextBytes(sk, (int)param.MSG_BYTES, (int)param.SECRETKEY_A_BYTES);
			sk[param.MSG_BYTES] &= 254;
			sk[param.MSG_BYTES + param.SECRETKEY_A_BYTES - 1] &= (byte)param.MASK_ALICE;
			sidhCompressed.EphemeralKeyGeneration_A_extended(sk, pk);
			Array.Copy(pk, 0L, sk, param.MSG_BYTES + param.SECRETKEY_A_BYTES, param.CRYPTO_PUBLICKEYBYTES);
		}
		else
		{
			random.NextBytes(sk, (int)param.MSG_BYTES, (int)param.SECRETKEY_B_BYTES);
			sk[param.MSG_BYTES + param.SECRETKEY_B_BYTES - 1] &= (byte)param.MASK_BOB;
			sidh.EphemeralKeyGeneration_B(sk, pk);
			Array.Copy(pk, 0L, sk, param.MSG_BYTES + param.SECRETKEY_B_BYTES, param.CRYPTO_PUBLICKEYBYTES);
		}
		return 0;
	}

	internal int crypto_kem_enc(byte[] ct, byte[] ss, byte[] pk, SecureRandom random)
	{
		if (isCompressed)
		{
			byte[] array = new byte[param.SECRETKEY_B_BYTES];
			byte[] array2 = new byte[param.FP2_ENCODED_BYTES];
			byte[] array3 = new byte[param.MSG_BYTES];
			byte[] array4 = new byte[param.CRYPTO_CIPHERTEXTBYTES + param.MSG_BYTES];
			random.NextBytes(array4, 0, (int)param.MSG_BYTES);
			Array.Copy(pk, 0L, array4, param.MSG_BYTES, param.CRYPTO_PUBLICKEYBYTES);
			IXof xof = new ShakeDigest(256);
			xof.BlockUpdate(array4, 0, (int)(param.CRYPTO_PUBLICKEYBYTES + param.MSG_BYTES));
			xof.OutputFinal(array, 0, (int)param.SECRETKEY_B_BYTES);
			sidhCompressed.FormatPrivKey_B(array);
			sidhCompressed.EphemeralKeyGeneration_B_extended(array, ct, 1u);
			sidhCompressed.EphemeralSecretAgreement_B(array, pk, array2);
			xof.BlockUpdate(array2, 0, (int)param.FP2_ENCODED_BYTES);
			xof.OutputFinal(array3, 0, (int)param.MSG_BYTES);
			for (int i = 0; i < param.MSG_BYTES; i++)
			{
				ct[i + param.PARTIALLY_COMPRESSED_CHUNK_CT] = (byte)(array4[i] ^ array3[i]);
			}
			Array.Copy(ct, 0L, array4, param.MSG_BYTES, param.CRYPTO_CIPHERTEXTBYTES);
			xof.BlockUpdate(array4, 0, (int)(param.CRYPTO_CIPHERTEXTBYTES + param.MSG_BYTES));
			xof.OutputFinal(ss, 0, (int)param.CRYPTO_BYTES);
			return 0;
		}
		byte[] array5 = new byte[param.SECRETKEY_A_BYTES];
		byte[] array6 = new byte[param.FP2_ENCODED_BYTES];
		byte[] array7 = new byte[param.MSG_BYTES];
		byte[] array8 = new byte[param.CRYPTO_CIPHERTEXTBYTES + param.MSG_BYTES];
		random.NextBytes(array8, 0, (int)param.MSG_BYTES);
		Array.Copy(pk, 0L, array8, param.MSG_BYTES, param.CRYPTO_PUBLICKEYBYTES);
		IXof xof2 = new ShakeDigest(256);
		xof2.BlockUpdate(array8, 0, (int)(param.CRYPTO_PUBLICKEYBYTES + param.MSG_BYTES));
		xof2.OutputFinal(array5, 0, (int)param.SECRETKEY_A_BYTES);
		array5[param.SECRETKEY_A_BYTES - 1] &= (byte)param.MASK_ALICE;
		sidh.EphemeralKeyGeneration_A(array5, ct);
		sidh.EphemeralSecretAgreement_A(array5, pk, array6);
		xof2.BlockUpdate(array6, 0, (int)param.FP2_ENCODED_BYTES);
		xof2.OutputFinal(array7, 0, (int)param.MSG_BYTES);
		for (int j = 0; j < param.MSG_BYTES; j++)
		{
			ct[j + param.CRYPTO_PUBLICKEYBYTES] = (byte)(array8[j] ^ array7[j]);
		}
		Array.Copy(ct, 0L, array8, param.MSG_BYTES, param.CRYPTO_CIPHERTEXTBYTES);
		xof2.BlockUpdate(array8, 0, (int)(param.CRYPTO_CIPHERTEXTBYTES + param.MSG_BYTES));
		xof2.OutputFinal(ss, 0, (int)param.CRYPTO_BYTES);
		return 0;
	}

	internal int crypto_kem_dec(byte[] ss, byte[] ct, byte[] sk)
	{
		if (isCompressed)
		{
			byte[] array = new byte[param.SECRETKEY_B_BYTES];
			byte[] array2 = new byte[param.FP2_ENCODED_BYTES + 2 * param.FP2_ENCODED_BYTES + param.SECRETKEY_A_BYTES];
			byte[] array3 = new byte[param.MSG_BYTES];
			byte[] array4 = new byte[param.CRYPTO_CIPHERTEXTBYTES + param.MSG_BYTES];
			byte[] tphiBKA_t = array2;
			sidhCompressed.EphemeralSecretAgreement_A_extended(sk, param.MSG_BYTES, ct, array2, 1u);
			IXof xof = new ShakeDigest(256);
			xof.BlockUpdate(array2, 0, (int)param.FP2_ENCODED_BYTES);
			xof.OutputFinal(array3, 0, (int)param.MSG_BYTES);
			for (int i = 0; i < param.MSG_BYTES; i++)
			{
				array4[i] = (byte)(ct[i + param.PARTIALLY_COMPRESSED_CHUNK_CT] ^ array3[i]);
			}
			Array.Copy(sk, param.MSG_BYTES + param.SECRETKEY_A_BYTES, array4, param.MSG_BYTES, param.CRYPTO_PUBLICKEYBYTES);
			xof.BlockUpdate(array4, 0, (int)(param.CRYPTO_PUBLICKEYBYTES + param.MSG_BYTES));
			xof.OutputFinal(array, 0, (int)param.SECRETKEY_B_BYTES);
			sidhCompressed.FormatPrivKey_B(array);
			byte selector = sidhCompressed.validate_ciphertext(array, ct, sk, param.MSG_BYTES + param.SECRETKEY_A_BYTES + param.CRYPTO_PUBLICKEYBYTES, tphiBKA_t, param.FP2_ENCODED_BYTES);
			fpx.ct_cmov(array4, sk, param.MSG_BYTES, selector);
			Array.Copy(ct, 0L, array4, param.MSG_BYTES, param.CRYPTO_CIPHERTEXTBYTES);
			xof.BlockUpdate(array4, 0, (int)(param.CRYPTO_CIPHERTEXTBYTES + param.MSG_BYTES));
			xof.OutputFinal(ss, 0, (int)param.CRYPTO_BYTES);
			return 0;
		}
		byte[] array5 = new byte[param.SECRETKEY_A_BYTES];
		byte[] array6 = new byte[param.FP2_ENCODED_BYTES];
		byte[] array7 = new byte[param.MSG_BYTES];
		byte[] array8 = new byte[param.CRYPTO_PUBLICKEYBYTES];
		byte[] array9 = new byte[param.CRYPTO_CIPHERTEXTBYTES + param.MSG_BYTES];
		sidh.EphemeralSecretAgreement_B(sk, ct, array6);
		IXof xof2 = new ShakeDigest(256);
		xof2.BlockUpdate(array6, 0, (int)param.FP2_ENCODED_BYTES);
		xof2.OutputFinal(array7, 0, (int)param.MSG_BYTES);
		for (int j = 0; j < param.MSG_BYTES; j++)
		{
			array9[j] = (byte)(ct[j + param.CRYPTO_PUBLICKEYBYTES] ^ array7[j]);
		}
		Array.Copy(sk, param.MSG_BYTES + param.SECRETKEY_B_BYTES, array9, param.MSG_BYTES, param.CRYPTO_PUBLICKEYBYTES);
		xof2.BlockUpdate(array9, 0, (int)(param.CRYPTO_PUBLICKEYBYTES + param.MSG_BYTES));
		xof2.OutputFinal(array5, 0, (int)param.SECRETKEY_A_BYTES);
		array5[param.SECRETKEY_A_BYTES - 1] &= (byte)param.MASK_ALICE;
		sidh.EphemeralKeyGeneration_A(array5, array8);
		byte selector2 = fpx.ct_compare(array8, ct, param.CRYPTO_PUBLICKEYBYTES);
		fpx.ct_cmov(array9, sk, param.MSG_BYTES, selector2);
		Array.Copy(ct, 0L, array9, param.MSG_BYTES, param.CRYPTO_CIPHERTEXTBYTES);
		xof2.BlockUpdate(array9, 0, (int)(param.CRYPTO_CIPHERTEXTBYTES + param.MSG_BYTES));
		xof2.OutputFinal(ss, 0, (int)param.CRYPTO_BYTES);
		return 0;
	}
}
