using System;
using Mirror.BouncyCastle.Crypto.Parameters;
using Mirror.BouncyCastle.Math;
using Mirror.BouncyCastle.Utilities;

namespace Mirror.BouncyCastle.Crypto.Signers;

public class X931Signer : ISigner
{
	private IDigest digest;

	private IAsymmetricBlockCipher cipher;

	private RsaKeyParameters kParam;

	private int trailer;

	private int keyBits;

	private byte[] block;

	public virtual string AlgorithmName => digest.AlgorithmName + "with" + cipher.AlgorithmName + "/X9.31";

	public X931Signer(IAsymmetricBlockCipher cipher, IDigest digest)
		: this(cipher, digest, isImplicit: false)
	{
	}

	public X931Signer(IAsymmetricBlockCipher cipher, IDigest digest, bool isImplicit)
	{
		this.cipher = cipher;
		this.digest = digest;
		if (isImplicit)
		{
			trailer = 188;
			return;
		}
		if (IsoTrailers.NoTrailerAvailable(digest))
		{
			throw new ArgumentException("no valid trailer", "digest");
		}
		trailer = IsoTrailers.GetTrailer(digest);
	}

	public virtual void Init(bool forSigning, ICipherParameters parameters)
	{
		if (parameters is ParametersWithRandom parametersWithRandom)
		{
			kParam = (RsaKeyParameters)parametersWithRandom.Parameters;
		}
		else
		{
			kParam = (RsaKeyParameters)parameters;
		}
		cipher.Init(forSigning, parameters);
		keyBits = kParam.Modulus.BitLength;
		block = new byte[(keyBits + 7) / 8];
		Reset();
	}

	public virtual void Update(byte b)
	{
		digest.Update(b);
	}

	public virtual void BlockUpdate(byte[] input, int inOff, int inLen)
	{
		digest.BlockUpdate(input, inOff, inLen);
	}

	public virtual int GetMaxSignatureSize()
	{
		return BigIntegers.GetUnsignedByteLength(kParam.Modulus);
	}

	public virtual byte[] GenerateSignature()
	{
		CreateSignatureBlock();
		BigInteger bigInteger = new BigInteger(1, cipher.ProcessBlock(block, 0, block.Length));
		Arrays.Fill(block, 0);
		bigInteger = bigInteger.Min(kParam.Modulus.Subtract(bigInteger));
		return BigIntegers.AsUnsignedByteArray(BigIntegers.GetUnsignedByteLength(kParam.Modulus), bigInteger);
	}

	public virtual bool VerifySignature(byte[] signature)
	{
		try
		{
			block = cipher.ProcessBlock(signature, 0, signature.Length);
		}
		catch (Exception)
		{
			return false;
		}
		BigInteger bigInteger = new BigInteger(1, block);
		BigInteger n;
		if ((bigInteger.IntValue & 0xF) == 12)
		{
			n = bigInteger;
		}
		else
		{
			bigInteger = kParam.Modulus.Subtract(bigInteger);
			if ((bigInteger.IntValue & 0xF) != 12)
			{
				return false;
			}
			n = bigInteger;
		}
		CreateSignatureBlock();
		byte[] array = BigIntegers.AsUnsignedByteArray(block.Length, n);
		bool result = Arrays.FixedTimeEquals(block, array);
		Arrays.Fill(block, 0);
		Arrays.Fill<byte>(array, (byte)0);
		return result;
	}

	public virtual void Reset()
	{
		digest.Reset();
	}

	private void CreateSignatureBlock()
	{
		int digestSize = digest.GetDigestSize();
		int num;
		if (trailer == 188)
		{
			num = block.Length - digestSize - 1;
			digest.DoFinal(block, num);
			block[block.Length - 1] = 188;
		}
		else
		{
			num = block.Length - digestSize - 2;
			digest.DoFinal(block, num);
			block[block.Length - 2] = (byte)(trailer >> 8);
			block[block.Length - 1] = (byte)trailer;
		}
		block[0] = 107;
		for (int num2 = num - 2; num2 != 0; num2--)
		{
			block[num2] = 187;
		}
		block[num - 1] = 186;
	}
}
