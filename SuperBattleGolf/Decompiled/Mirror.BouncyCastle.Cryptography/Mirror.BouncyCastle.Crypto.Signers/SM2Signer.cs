using System;
using Mirror.BouncyCastle.Crypto.Digests;
using Mirror.BouncyCastle.Crypto.Parameters;
using Mirror.BouncyCastle.Math;
using Mirror.BouncyCastle.Math.EC;
using Mirror.BouncyCastle.Math.EC.Multiplier;
using Mirror.BouncyCastle.Security;
using Mirror.BouncyCastle.Utilities;
using Mirror.BouncyCastle.Utilities.Encoders;

namespace Mirror.BouncyCastle.Crypto.Signers;

public class SM2Signer : ISigner
{
	private enum State
	{
		Uninitialized,
		Init,
		Data
	}

	private readonly IDsaKCalculator kCalculator = new RandomDsaKCalculator();

	private readonly IDigest digest;

	private readonly IDsaEncoding encoding;

	private State m_state;

	private ECDomainParameters ecParams;

	private ECPoint pubPoint;

	private ECKeyParameters ecKey;

	private byte[] z;

	public virtual string AlgorithmName => "SM2Sign";

	public SM2Signer()
		: this(StandardDsaEncoding.Instance, new SM3Digest())
	{
	}

	public SM2Signer(IDigest digest)
		: this(StandardDsaEncoding.Instance, digest)
	{
	}

	public SM2Signer(IDsaEncoding encoding)
		: this(encoding, new SM3Digest())
	{
	}

	public SM2Signer(IDsaEncoding encoding, IDigest digest)
	{
		this.encoding = encoding;
		this.digest = digest;
	}

	public virtual void Init(bool forSigning, ICipherParameters parameters)
	{
		ICipherParameters cipherParameters;
		byte[] array;
		if (parameters is ParametersWithID parametersWithID)
		{
			cipherParameters = parametersWithID.Parameters;
			array = parametersWithID.GetID();
			if (array.Length >= 8192)
			{
				throw new ArgumentException("SM2 user ID must be less than 2^16 bits long");
			}
		}
		else
		{
			cipherParameters = parameters;
			array = Hex.DecodeStrict("31323334353637383132333435363738");
		}
		if (forSigning)
		{
			SecureRandom secureRandom = null;
			if (cipherParameters is ParametersWithRandom parametersWithRandom)
			{
				ecKey = (ECKeyParameters)parametersWithRandom.Parameters;
				ecParams = ecKey.Parameters;
				secureRandom = parametersWithRandom.Random;
			}
			else
			{
				ecKey = (ECKeyParameters)cipherParameters;
				ecParams = ecKey.Parameters;
			}
			if (!kCalculator.IsDeterministic)
			{
				secureRandom = CryptoServicesRegistrar.GetSecureRandom(secureRandom);
			}
			kCalculator.Init(ecParams.N, secureRandom);
			pubPoint = CreateBasePointMultiplier().Multiply(ecParams.G, ((ECPrivateKeyParameters)ecKey).D).Normalize();
		}
		else
		{
			ecKey = (ECKeyParameters)cipherParameters;
			ecParams = ecKey.Parameters;
			pubPoint = ((ECPublicKeyParameters)ecKey).Q;
		}
		digest.Reset();
		z = GetZ(array);
		m_state = State.Init;
	}

	public virtual void Update(byte b)
	{
		CheckData();
		digest.Update(b);
	}

	public virtual void BlockUpdate(byte[] input, int inOff, int inLen)
	{
		CheckData();
		digest.BlockUpdate(input, inOff, inLen);
	}

	public virtual int GetMaxSignatureSize()
	{
		return encoding.GetMaxEncodingSize(ecParams.N);
	}

	public virtual byte[] GenerateSignature()
	{
		CheckData();
		byte[] message = DigestUtilities.DoFinal(digest);
		BigInteger n = ecParams.N;
		BigInteger bigInteger = CalculateE(n, message);
		BigInteger d = ((ECPrivateKeyParameters)ecKey).D;
		ECMultiplier eCMultiplier = CreateBasePointMultiplier();
		BigInteger bigInteger3;
		BigInteger val;
		while (true)
		{
			BigInteger bigInteger2 = kCalculator.NextK();
			ECPoint eCPoint = eCMultiplier.Multiply(ecParams.G, bigInteger2).Normalize();
			bigInteger3 = bigInteger.Add(eCPoint.AffineXCoord.ToBigInteger()).Mod(n);
			if (bigInteger3.SignValue != 0 && !bigInteger3.Add(bigInteger2).Equals(n))
			{
				BigInteger bigInteger4 = BigIntegers.ModOddInverse(n, d.Add(BigIntegers.One));
				val = bigInteger2.Subtract(bigInteger3.Multiply(d)).Mod(n);
				val = bigInteger4.Multiply(val).Mod(n);
				if (val.SignValue != 0)
				{
					break;
				}
			}
		}
		try
		{
			return encoding.Encode(ecParams.N, bigInteger3, val);
		}
		catch (Exception ex)
		{
			throw new CryptoException("unable to encode signature: " + ex.Message, ex);
		}
		finally
		{
			Reset();
		}
	}

	public virtual bool VerifySignature(byte[] signature)
	{
		CheckData();
		try
		{
			BigInteger[] array = encoding.Decode(ecParams.N, signature);
			return VerifySignature(array[0], array[1]);
		}
		catch (Exception)
		{
		}
		finally
		{
			Reset();
		}
		return false;
	}

	public virtual void Reset()
	{
		switch (m_state)
		{
		case State.Init:
			break;
		default:
			throw new InvalidOperationException(AlgorithmName + " needs to be initialized");
		case State.Data:
			digest.Reset();
			m_state = State.Init;
			break;
		}
	}

	private bool VerifySignature(BigInteger r, BigInteger s)
	{
		BigInteger n = ecParams.N;
		if (r.CompareTo(BigInteger.One) < 0 || r.CompareTo(n) >= 0)
		{
			return false;
		}
		if (s.CompareTo(BigInteger.One) < 0 || s.CompareTo(n) >= 0)
		{
			return false;
		}
		byte[] message = DigestUtilities.DoFinal(digest);
		BigInteger bigInteger = CalculateE(n, message);
		BigInteger bigInteger2 = r.Add(s).Mod(n);
		if (bigInteger2.SignValue == 0)
		{
			return false;
		}
		ECPoint q = ((ECPublicKeyParameters)ecKey).Q;
		ECPoint eCPoint = ECAlgorithms.SumOfTwoMultiplies(ecParams.G, s, q, bigInteger2).Normalize();
		if (eCPoint.IsInfinity)
		{
			return false;
		}
		return bigInteger.Add(eCPoint.AffineXCoord.ToBigInteger()).Mod(n).Equals(r);
	}

	private void CheckData()
	{
		switch (m_state)
		{
		case State.Data:
			break;
		default:
			throw new InvalidOperationException(AlgorithmName + " needs to be initialized");
		case State.Init:
			digest.BlockUpdate(z, 0, z.Length);
			m_state = State.Data;
			break;
		}
	}

	private byte[] GetZ(byte[] userID)
	{
		AddUserID(digest, userID);
		AddFieldElement(digest, ecParams.Curve.A);
		AddFieldElement(digest, ecParams.Curve.B);
		AddFieldElement(digest, ecParams.G.AffineXCoord);
		AddFieldElement(digest, ecParams.G.AffineYCoord);
		AddFieldElement(digest, pubPoint.AffineXCoord);
		AddFieldElement(digest, pubPoint.AffineYCoord);
		return DigestUtilities.DoFinal(digest);
	}

	private void AddUserID(IDigest digest, byte[] userID)
	{
		int num = userID.Length * 8;
		digest.Update((byte)(num >> 8));
		digest.Update((byte)num);
		digest.BlockUpdate(userID, 0, userID.Length);
	}

	private void AddFieldElement(IDigest digest, ECFieldElement v)
	{
		byte[] encoded = v.GetEncoded();
		digest.BlockUpdate(encoded, 0, encoded.Length);
	}

	protected virtual BigInteger CalculateE(BigInteger n, byte[] message)
	{
		return new BigInteger(1, message);
	}

	protected virtual ECMultiplier CreateBasePointMultiplier()
	{
		return new FixedPointCombMultiplier();
	}
}
