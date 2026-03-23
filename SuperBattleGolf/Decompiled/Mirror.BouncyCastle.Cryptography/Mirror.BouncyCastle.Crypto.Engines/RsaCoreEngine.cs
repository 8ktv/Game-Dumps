using System;
using Mirror.BouncyCastle.Crypto.Parameters;
using Mirror.BouncyCastle.Math;
using Mirror.BouncyCastle.Security;
using Mirror.BouncyCastle.Utilities;

namespace Mirror.BouncyCastle.Crypto.Engines;

public class RsaCoreEngine : IRsa
{
	private RsaKeyParameters m_key;

	private bool m_forEncryption;

	private int m_bitSize;

	public virtual void Init(bool forEncryption, ICipherParameters parameters)
	{
		if (parameters is ParametersWithRandom parametersWithRandom)
		{
			parameters = parametersWithRandom.Parameters;
		}
		m_key = (parameters as RsaKeyParameters) ?? throw new InvalidKeyException("Not an RSA key");
		m_forEncryption = forEncryption;
		m_bitSize = m_key.Modulus.BitLength;
	}

	public virtual int GetInputBlockSize()
	{
		CheckInitialised();
		if (m_forEncryption)
		{
			return (m_bitSize - 1) / 8;
		}
		return (m_bitSize + 7) / 8;
	}

	public virtual int GetOutputBlockSize()
	{
		CheckInitialised();
		if (m_forEncryption)
		{
			return (m_bitSize + 7) / 8;
		}
		return (m_bitSize - 1) / 8;
	}

	public virtual BigInteger ConvertInput(byte[] inBuf, int inOff, int inLen)
	{
		CheckInitialised();
		int num = (m_bitSize + 7) / 8;
		if (inLen > num)
		{
			throw new DataLengthException("input too large for RSA cipher.");
		}
		BigInteger bigInteger = new BigInteger(1, inBuf, inOff, inLen);
		if (bigInteger.CompareTo(m_key.Modulus) >= 0)
		{
			throw new DataLengthException("input too large for RSA cipher.");
		}
		return bigInteger;
	}

	public virtual byte[] ConvertOutput(BigInteger result)
	{
		CheckInitialised();
		if (!m_forEncryption)
		{
			return BigIntegers.AsUnsignedByteArray(result);
		}
		return BigIntegers.AsUnsignedByteArray(GetOutputBlockSize(), result);
	}

	public virtual BigInteger ProcessBlock(BigInteger input)
	{
		CheckInitialised();
		if (!(m_key is RsaPrivateCrtKeyParameters { P: var p, Q: var q, DP: var dP, DQ: var dQ, QInv: var qInv } rsaPrivateCrtKeyParameters))
		{
			return input.ModPow(m_key.Exponent, m_key.Modulus);
		}
		BigInteger bigInteger = input.Remainder(p).ModPow(dP, p);
		BigInteger bigInteger2 = input.Remainder(q).ModPow(dQ, q);
		BigInteger bigInteger3 = bigInteger.Subtract(bigInteger2).Multiply(qInv).Mod(p)
			.Multiply(q)
			.Add(bigInteger2);
		if (!bigInteger3.ModPow(rsaPrivateCrtKeyParameters.PublicExponent, rsaPrivateCrtKeyParameters.Modulus).Equals(input))
		{
			throw new InvalidOperationException("RSA engine faulty decryption/signing detected");
		}
		return bigInteger3;
	}

	private void CheckInitialised()
	{
		if (m_key == null)
		{
			throw new InvalidOperationException("RSA engine not initialised");
		}
	}
}
