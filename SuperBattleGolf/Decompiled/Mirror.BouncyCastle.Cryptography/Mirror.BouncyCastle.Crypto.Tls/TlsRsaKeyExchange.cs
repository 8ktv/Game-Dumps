using System;
using Mirror.BouncyCastle.Crypto.Parameters;
using Mirror.BouncyCastle.Crypto.Utilities;
using Mirror.BouncyCastle.Math;
using Mirror.BouncyCastle.Security;
using Mirror.BouncyCastle.Utilities;

namespace Mirror.BouncyCastle.Crypto.Tls;

public static class TlsRsaKeyExchange
{
	public const int PreMasterSecretLength = 48;

	public static byte[] DecryptPreMasterSecret(byte[] buf, int off, int len, RsaKeyParameters privateKey, int protocolVersion, SecureRandom secureRandom)
	{
		if (buf == null || len < 1 || len > GetInputLimit(privateKey) || off < 0 || off > buf.Length - len)
		{
			throw new ArgumentException("input not a valid EncryptedPreMasterSecret");
		}
		if (!privateKey.IsPrivate)
		{
			throw new ArgumentException("must be an RSA private key", "privateKey");
		}
		BigInteger modulus = privateKey.Modulus;
		int bitLength = modulus.BitLength;
		if (bitLength < 512)
		{
			throw new ArgumentException("must be at least 512 bits", "privateKey");
		}
		if ((protocolVersion & 0xFFFF) != protocolVersion)
		{
			throw new ArgumentException("must be a 16 bit value", "protocolVersion");
		}
		secureRandom = CryptoServicesRegistrar.GetSecureRandom(secureRandom);
		byte[] array = new byte[48];
		secureRandom.NextBytes(array);
		try
		{
			BigInteger input = ConvertInput(modulus, buf, off, len);
			byte[] array2 = RsaBlinded(privateKey, input, secureRandom);
			int pkcs1Length = (bitLength - 1) / 8;
			int num = array2.Length - 48;
			int num2 = CheckPkcs1Encoding2(array2, pkcs1Length, 48);
			int num3 = -(Pack.BE_To_UInt16(array2, num) ^ protocolVersion) >> 31;
			int num4 = num2 | num3;
			for (int i = 0; i < 48; i++)
			{
				array[i] = (byte)((array[i] & num4) | (array2[num + i] & ~num4));
			}
			Arrays.Fill(array2, 0);
		}
		catch (Exception)
		{
		}
		return array;
	}

	public static int GetInputLimit(RsaKeyParameters privateKey)
	{
		return (privateKey.Modulus.BitLength + 7) / 8;
	}

	private static int CAddTo(int len, int cond, byte[] x, byte[] z)
	{
		int num = 0;
		for (int num2 = len - 1; num2 >= 0; num2--)
		{
			num += z[num2] + (x[num2] & cond);
			z[num2] = (byte)num;
			num >>= 8;
		}
		return num;
	}

	private static int CheckPkcs1Encoding2(byte[] buf, int pkcs1Length, int plaintextLength)
	{
		int num = pkcs1Length - plaintextLength - 10;
		int num2 = buf.Length - pkcs1Length;
		int num3 = buf.Length - 1 - plaintextLength;
		for (int i = 0; i < num2; i++)
		{
			num |= -buf[i];
		}
		num |= -(buf[num2] ^ 2);
		for (int j = num2 + 1; j < num3; j++)
		{
			num |= buf[j] - 1;
		}
		num |= -buf[num3];
		return num >> 31;
	}

	private static BigInteger ConvertInput(BigInteger modulus, byte[] buf, int off, int len)
	{
		BigInteger bigInteger = BigIntegers.FromUnsignedByteArray(buf, off, len);
		if (bigInteger.CompareTo(modulus) < 0)
		{
			return bigInteger;
		}
		throw new DataLengthException("input too large for RSA cipher.");
	}

	private static BigInteger Rsa(RsaKeyParameters privateKey, BigInteger input)
	{
		return input.ModPow(privateKey.Exponent, privateKey.Modulus);
	}

	private static byte[] RsaBlinded(RsaKeyParameters privateKey, BigInteger input, SecureRandom secureRandom)
	{
		BigInteger modulus = privateKey.Modulus;
		int num = (modulus.BitLength + 7) / 8;
		if (!(privateKey is RsaPrivateCrtKeyParameters { PublicExponent: var publicExponent } rsaPrivateCrtKeyParameters))
		{
			return BigIntegers.AsUnsignedByteArray(num, Rsa(privateKey, input));
		}
		BigInteger bigInteger = BigIntegers.CreateRandomInRange(BigInteger.One, modulus.Subtract(BigInteger.One), secureRandom);
		BigInteger bigInteger2 = bigInteger.ModPow(publicExponent, modulus);
		BigInteger bigInteger3 = BigIntegers.ModOddInverse(modulus, bigInteger);
		BigInteger input2 = bigInteger2.ModMultiply(input, modulus);
		BigInteger bigInteger4 = RsaCrt(rsaPrivateCrtKeyParameters, input2);
		BigInteger n = bigInteger3.Add(BigInteger.One).ModMultiply(bigInteger4, modulus);
		byte[] x = BigIntegers.AsUnsignedByteArray(num, bigInteger4);
		byte[] x2 = BigIntegers.AsUnsignedByteArray(num, modulus);
		byte[] array = BigIntegers.AsUnsignedByteArray(num, n);
		int cond = SubFrom(num, x, array);
		CAddTo(num, cond, x2, array);
		return array;
	}

	private static BigInteger RsaCrt(RsaPrivateCrtKeyParameters crtKey, BigInteger input)
	{
		BigInteger publicExponent = crtKey.PublicExponent;
		BigInteger p = crtKey.P;
		BigInteger q = crtKey.Q;
		BigInteger dP = crtKey.DP;
		BigInteger dQ = crtKey.DQ;
		BigInteger qInv = crtKey.QInv;
		BigInteger bigInteger = input.Remainder(p).ModPow(dP, p);
		BigInteger bigInteger2 = input.Remainder(q).ModPow(dQ, q);
		BigInteger bigInteger3 = bigInteger.Subtract(bigInteger2).ModMultiply(qInv, p).Multiply(q)
			.Add(bigInteger2);
		if (!bigInteger3.ModPow(publicExponent, crtKey.Modulus).Equals(input))
		{
			throw new InvalidOperationException("RSA engine faulty decryption/signing detected");
		}
		return bigInteger3;
	}

	private static int SubFrom(int len, byte[] x, byte[] z)
	{
		int num = 0;
		for (int num2 = len - 1; num2 >= 0; num2--)
		{
			num += z[num2] - x[num2];
			z[num2] = (byte)num;
			num >>= 8;
		}
		return num;
	}
}
