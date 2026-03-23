using System;
using Mirror.BouncyCastle.Security;
using Mirror.BouncyCastle.Utilities;

namespace Mirror.BouncyCastle.Pqc.Crypto.Falcon;

internal class FalconNist
{
	private FalconCodec codec;

	private FalconVrfy vrfy;

	private FalconCommon common;

	private SecureRandom random;

	private uint logn;

	private uint noncelen;

	private int CRYPTO_BYTES;

	private int CRYPTO_PUBLICKEYBYTES;

	private int CRYPTO_SECRETKEYBYTES;

	internal uint NonceLength => noncelen;

	internal uint LogN => logn;

	internal int CryptoBytes => CRYPTO_BYTES;

	internal FalconNist(SecureRandom random, uint logn, uint noncelen)
	{
		this.logn = logn;
		codec = new FalconCodec();
		common = new FalconCommon();
		vrfy = new FalconVrfy(common);
		this.random = random;
		this.noncelen = noncelen;
		int num = 1 << (int)logn;
		CRYPTO_PUBLICKEYBYTES = 1 + 14 * num / 8;
		switch (logn)
		{
		case 10u:
			CRYPTO_SECRETKEYBYTES = 2305;
			CRYPTO_BYTES = 1330;
			break;
		case 8u:
		case 9u:
			CRYPTO_SECRETKEYBYTES = 1 + 6 * num * 2 / 8 + num;
			CRYPTO_BYTES = 690;
			break;
		case 6u:
		case 7u:
			CRYPTO_SECRETKEYBYTES = 1 + 7 * num * 2 / 8 + num;
			CRYPTO_BYTES = 690;
			break;
		default:
			CRYPTO_SECRETKEYBYTES = 1 + num * 2 + num;
			CRYPTO_BYTES = 690;
			break;
		}
	}

	internal int crypto_sign_keypair(out byte[] pk, out byte[] fEnc, out byte[] gEnc, out byte[] FEnc)
	{
		byte[] array = new byte[CRYPTO_SECRETKEYBYTES];
		pk = new byte[CRYPTO_PUBLICKEYBYTES];
		int num = 1 << (int)logn;
		SHAKE256 sHAKE = new SHAKE256();
		sbyte[] array2 = new sbyte[num];
		sbyte[] array3 = new sbyte[num];
		sbyte[] array4 = new sbyte[num];
		ushort[] array5 = new ushort[num];
		byte[] array6 = new byte[48];
		FalconKeygen falconKeygen = new FalconKeygen(codec, vrfy);
		random.NextBytes(array6);
		sHAKE.i_shake256_init();
		sHAKE.i_shake256_inject(array6, 0, array6.Length);
		sHAKE.i_shake256_flip();
		falconKeygen.keygen(sHAKE, array2, 0, array3, 0, array4, 0, null, 0, array5, 0, logn);
		array[0] = (byte)(80 + logn);
		int num2 = 1;
		int num3 = codec.trim_i8_encode(array, num2, CRYPTO_SECRETKEYBYTES - num2, array2, 0, logn, codec.max_fg_bits[logn]);
		if (num3 == 0)
		{
			throw new InvalidOperationException("f encode failed");
		}
		fEnc = Arrays.CopyOfRange(array, num2, num2 + num3);
		num2 += num3;
		num3 = codec.trim_i8_encode(array, num2, CRYPTO_SECRETKEYBYTES - num2, array3, 0, logn, codec.max_fg_bits[logn]);
		if (num3 == 0)
		{
			throw new InvalidOperationException("g encode failed");
		}
		gEnc = Arrays.CopyOfRange(array, num2, num2 + num3);
		num2 += num3;
		num3 = codec.trim_i8_encode(array, num2, CRYPTO_SECRETKEYBYTES - num2, array4, 0, logn, codec.max_FG_bits[logn]);
		if (num3 == 0)
		{
			throw new InvalidOperationException("F encode failed");
		}
		FEnc = Arrays.CopyOfRange(array, num2, num2 + num3);
		num2 += num3;
		if (num2 != CRYPTO_SECRETKEYBYTES)
		{
			throw new InvalidOperationException("secret key encoding failed");
		}
		pk[0] = (byte)logn;
		num3 = codec.modq_encode(pk, 1, CRYPTO_PUBLICKEYBYTES - 1, array5, 0, logn);
		if (num3 != CRYPTO_PUBLICKEYBYTES - 1)
		{
			throw new InvalidOperationException("public key encoding failed");
		}
		pk = Arrays.CopyOfRange(pk, 1, pk.Length);
		return 0;
	}

	internal byte[] crypto_sign(bool attached, byte[] sm, byte[] msrc, int m, uint mlen, byte[] sksrc, int sk)
	{
		int num = 1 << (int)logn;
		sbyte[] array = new sbyte[num];
		sbyte[] array2 = new sbyte[num];
		sbyte[] array3 = new sbyte[num];
		sbyte[] gsrc = new sbyte[num];
		short[] array4 = new short[num];
		ushort[] array5 = new ushort[num];
		byte[] array6 = new byte[48];
		byte[] array7 = new byte[noncelen];
		byte[] array8 = new byte[CRYPTO_BYTES - 2 - noncelen];
		SHAKE256 sHAKE = new SHAKE256();
		FalconSign falconSign = new FalconSign(common);
		int num2 = 0;
		int num3 = codec.trim_i8_decode(array, 0, logn, codec.max_fg_bits[logn], sksrc, sk + num2, CRYPTO_SECRETKEYBYTES - num2);
		if (num3 == 0)
		{
			throw new InvalidOperationException("f decode failed");
		}
		num2 += num3;
		num3 = codec.trim_i8_decode(array2, 0, logn, codec.max_fg_bits[logn], sksrc, sk + num2, CRYPTO_SECRETKEYBYTES - num2);
		if (num3 == 0)
		{
			throw new InvalidOperationException("g decode failed");
		}
		num2 += num3;
		num3 = codec.trim_i8_decode(array3, 0, logn, codec.max_FG_bits[logn], sksrc, sk + num2, CRYPTO_SECRETKEYBYTES - num2);
		if (num3 == 0)
		{
			throw new InvalidOperationException("F decode failed");
		}
		num2 += num3;
		if (num2 != CRYPTO_SECRETKEYBYTES - 1)
		{
			throw new InvalidOperationException("full Key not used");
		}
		if (vrfy.complete_private(gsrc, 0, array, 0, array2, 0, array3, 0, logn, new ushort[2 * num], 0) == 0)
		{
			throw new InvalidOperationException("complete private failed");
		}
		random.NextBytes(array7);
		sHAKE.i_shake256_init();
		sHAKE.i_shake256_inject(array7, 0, array7.Length);
		sHAKE.i_shake256_inject(msrc, m, (int)mlen);
		sHAKE.i_shake256_flip();
		common.hash_to_point_vartime(sHAKE, array5, 0, logn);
		random.NextBytes(array6);
		sHAKE.i_shake256_init();
		sHAKE.i_shake256_inject(array6, 0, array6.Length);
		sHAKE.i_shake256_flip();
		falconSign.sign_dyn(array4, 0, sHAKE, array, 0, array2, 0, array3, 0, gsrc, 0, array5, 0, logn, new FalconFPR[10 * num], 0);
		int num4;
		if (attached)
		{
			array8[0] = (byte)(32 + logn);
			num4 = codec.comp_encode(array8, 1, array8.Length - 1, array4, 0, logn);
			if (num4 == 0)
			{
				throw new InvalidOperationException("signature failed to generate");
			}
			num4++;
		}
		else
		{
			num4 = codec.comp_encode(array8, 0, array8.Length, array4, 0, logn);
			if (num4 == 0)
			{
				throw new InvalidOperationException("signature failed to generate");
			}
		}
		sm[0] = (byte)(48 + logn);
		Array.Copy(array7, 0L, sm, 1L, noncelen);
		Array.Copy(array8, 0L, sm, 1 + noncelen, num4);
		return Arrays.CopyOfRange(sm, 0, (int)(1 + noncelen) + num4);
	}

	internal int crypto_sign_open(bool attached, byte[] sig_encoded, byte[] nonce, byte[] m, byte[] pksrc, int pk)
	{
		int num = 1 << (int)logn;
		ushort[] array = new ushort[num];
		ushort[] array2 = new ushort[num];
		short[] array3 = new short[num];
		SHAKE256 sHAKE = new SHAKE256();
		if (codec.modq_decode(array, 0, logn, pksrc, pk, CRYPTO_PUBLICKEYBYTES - 1) != CRYPTO_PUBLICKEYBYTES - 1)
		{
			return -1;
		}
		vrfy.to_ntt_monty(array, 0, logn);
		int num2 = sig_encoded.Length;
		_ = m.Length;
		if (attached)
		{
			if (num2 < 1 || sig_encoded[0] != (byte)(32 + logn))
			{
				return -1;
			}
			if (codec.comp_decode(array3, 0, logn, sig_encoded, 1, num2 - 1) != num2 - 1)
			{
				return -1;
			}
		}
		else if (num2 < 1 || codec.comp_decode(array3, 0, logn, sig_encoded, 0, num2) != num2)
		{
			return -1;
		}
		sHAKE.i_shake256_init();
		sHAKE.i_shake256_inject(nonce, 0, (int)noncelen);
		sHAKE.i_shake256_inject(m, 0, m.Length);
		sHAKE.i_shake256_flip();
		common.hash_to_point_vartime(sHAKE, array2, 0, logn);
		if (!vrfy.verify_raw(array2, 0, array3, 0, array, 0, logn, new ushort[num], 0))
		{
			return -1;
		}
		return 0;
	}
}
