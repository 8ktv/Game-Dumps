using System;
using Mirror.BouncyCastle.Crypto.Parameters;
using Mirror.BouncyCastle.Math;
using Mirror.BouncyCastle.Security;

namespace Mirror.BouncyCastle.Crypto.Signers;

public class DsaDigestSigner : ISigner
{
	private readonly IDsa dsa;

	private readonly IDigest digest;

	private readonly IDsaEncoding encoding;

	private bool forSigning;

	public virtual string AlgorithmName => digest.AlgorithmName + "with" + dsa.AlgorithmName;

	public DsaDigestSigner(IDsa dsa, IDigest digest)
		: this(dsa, digest, StandardDsaEncoding.Instance)
	{
	}

	public DsaDigestSigner(IDsa dsa, IDigest digest, IDsaEncoding encoding)
	{
		this.dsa = dsa;
		this.digest = digest;
		this.encoding = encoding;
	}

	public virtual void Init(bool forSigning, ICipherParameters parameters)
	{
		this.forSigning = forSigning;
		AsymmetricKeyParameter asymmetricKeyParameter = ((!(parameters is ParametersWithRandom parametersWithRandom)) ? ((AsymmetricKeyParameter)parameters) : ((AsymmetricKeyParameter)parametersWithRandom.Parameters));
		if (forSigning && !asymmetricKeyParameter.IsPrivate)
		{
			throw new InvalidKeyException("Signing Requires Private Key.");
		}
		if (!forSigning && asymmetricKeyParameter.IsPrivate)
		{
			throw new InvalidKeyException("Verification Requires Public Key.");
		}
		Reset();
		dsa.Init(forSigning, parameters);
	}

	public virtual void Update(byte input)
	{
		digest.Update(input);
	}

	public virtual void BlockUpdate(byte[] input, int inOff, int inLen)
	{
		digest.BlockUpdate(input, inOff, inLen);
	}

	public virtual int GetMaxSignatureSize()
	{
		return encoding.GetMaxEncodingSize(GetOrder());
	}

	public virtual byte[] GenerateSignature()
	{
		if (!forSigning)
		{
			throw new InvalidOperationException("DsaDigestSigner not initialized for signature generation.");
		}
		byte[] array = new byte[digest.GetDigestSize()];
		digest.DoFinal(array, 0);
		BigInteger[] array2 = dsa.GenerateSignature(array);
		try
		{
			return encoding.Encode(GetOrder(), array2[0], array2[1]);
		}
		catch (Exception)
		{
			throw new InvalidOperationException("unable to encode signature");
		}
	}

	public virtual bool VerifySignature(byte[] signature)
	{
		if (forSigning)
		{
			throw new InvalidOperationException("DsaDigestSigner not initialized for verification");
		}
		byte[] array = new byte[digest.GetDigestSize()];
		digest.DoFinal(array, 0);
		try
		{
			BigInteger[] array2 = encoding.Decode(GetOrder(), signature);
			return dsa.VerifySignature(array, array2[0], array2[1]);
		}
		catch (Exception)
		{
			return false;
		}
	}

	public virtual void Reset()
	{
		digest.Reset();
	}

	protected virtual BigInteger GetOrder()
	{
		return dsa.Order;
	}
}
