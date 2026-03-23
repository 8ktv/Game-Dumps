using System;
using Mirror.BouncyCastle.Crypto.Digests;
using Mirror.BouncyCastle.Crypto.Parameters;
using Mirror.BouncyCastle.Crypto.Utilities;
using Mirror.BouncyCastle.Math;
using Mirror.BouncyCastle.Math.EC;
using Mirror.BouncyCastle.Math.EC.Multiplier;
using Mirror.BouncyCastle.Security;
using Mirror.BouncyCastle.Utilities;

namespace Mirror.BouncyCastle.Crypto.Engines;

public class SM2Engine
{
	public enum Mode
	{
		C1C2C3,
		C1C3C2
	}

	private readonly IDigest mDigest;

	private readonly Mode mMode;

	private bool mForEncryption;

	private ECKeyParameters mECKey;

	private ECDomainParameters mECParams;

	private int mCurveLength;

	private SecureRandom mRandom;

	public SM2Engine()
		: this(new SM3Digest())
	{
	}

	public SM2Engine(Mode mode)
		: this(new SM3Digest(), mode)
	{
	}

	public SM2Engine(IDigest digest)
		: this(digest, Mode.C1C2C3)
	{
	}

	public SM2Engine(IDigest digest, Mode mode)
	{
		mDigest = digest;
		mMode = mode;
	}

	public virtual void Init(bool forEncryption, ICipherParameters param)
	{
		mForEncryption = forEncryption;
		SecureRandom secureRandom = null;
		if (param is ParametersWithRandom parametersWithRandom)
		{
			param = parametersWithRandom.Parameters;
			secureRandom = parametersWithRandom.Random;
		}
		mECKey = (ECKeyParameters)param;
		mECParams = mECKey.Parameters;
		if (forEncryption)
		{
			mRandom = CryptoServicesRegistrar.GetSecureRandom(secureRandom);
			if (((ECPublicKeyParameters)mECKey).Q.Multiply(mECParams.H).IsInfinity)
			{
				throw new ArgumentException("invalid key: [h]Q at infinity");
			}
		}
		else
		{
			mRandom = null;
		}
		mCurveLength = mECParams.Curve.FieldElementEncodingLength;
	}

	public virtual byte[] ProcessBlock(byte[] input, int inOff, int inLen)
	{
		if (inOff + inLen > input.Length || inLen == 0)
		{
			throw new DataLengthException("input buffer too short");
		}
		if (mForEncryption)
		{
			return Encrypt(input, inOff, inLen);
		}
		return Decrypt(input, inOff, inLen);
	}

	protected virtual ECMultiplier CreateBasePointMultiplier()
	{
		return new FixedPointCombMultiplier();
	}

	private byte[] Encrypt(byte[] input, int inOff, int inLen)
	{
		byte[] array = new byte[inLen];
		Array.Copy(input, inOff, array, 0, array.Length);
		ECMultiplier eCMultiplier = CreateBasePointMultiplier();
		BigInteger bigInteger;
		ECPoint eCPoint;
		do
		{
			bigInteger = NextK();
			eCPoint = ((ECPublicKeyParameters)mECKey).Q.Multiply(bigInteger).Normalize();
			Kdf(mDigest, eCPoint, array);
		}
		while (NotEncrypted(array, input, inOff));
		byte[] encoded = eCMultiplier.Multiply(mECParams.G, bigInteger).Normalize().GetEncoded(compressed: false);
		AddFieldElement(mDigest, eCPoint.AffineXCoord);
		mDigest.BlockUpdate(input, inOff, inLen);
		AddFieldElement(mDigest, eCPoint.AffineYCoord);
		byte[] array2 = DigestUtilities.DoFinal(mDigest);
		if (mMode == Mode.C1C3C2)
		{
			return Arrays.ConcatenateAll(encoded, array2, array);
		}
		return Arrays.ConcatenateAll(encoded, array, array2);
	}

	private byte[] Decrypt(byte[] input, int inOff, int inLen)
	{
		byte[] array = new byte[mCurveLength * 2 + 1];
		Array.Copy(input, inOff, array, 0, array.Length);
		ECPoint eCPoint = mECParams.Curve.DecodePoint(array);
		if (eCPoint.Multiply(mECParams.H).IsInfinity)
		{
			throw new InvalidCipherTextException("[h]C1 at infinity");
		}
		eCPoint = eCPoint.Multiply(((ECPrivateKeyParameters)mECKey).D).Normalize();
		int digestSize = mDigest.GetDigestSize();
		byte[] array2 = new byte[inLen - array.Length - digestSize];
		if (mMode == Mode.C1C3C2)
		{
			Array.Copy(input, inOff + array.Length + digestSize, array2, 0, array2.Length);
		}
		else
		{
			Array.Copy(input, inOff + array.Length, array2, 0, array2.Length);
		}
		Kdf(mDigest, eCPoint, array2);
		AddFieldElement(mDigest, eCPoint.AffineXCoord);
		mDigest.BlockUpdate(array2, 0, array2.Length);
		AddFieldElement(mDigest, eCPoint.AffineYCoord);
		byte[] array3 = DigestUtilities.DoFinal(mDigest);
		int num = 0;
		if (mMode == Mode.C1C3C2)
		{
			for (int i = 0; i != array3.Length; i++)
			{
				num |= array3[i] ^ input[inOff + array.Length + i];
			}
		}
		else
		{
			for (int j = 0; j != array3.Length; j++)
			{
				num |= array3[j] ^ input[inOff + array.Length + array2.Length + j];
			}
		}
		Arrays.Fill(array, 0);
		Arrays.Fill(array3, 0);
		if (num != 0)
		{
			Arrays.Fill(array2, 0);
			throw new InvalidCipherTextException("invalid cipher text");
		}
		return array2;
	}

	private bool NotEncrypted(byte[] encData, byte[] input, int inOff)
	{
		for (int i = 0; i != encData.Length; i++)
		{
			if (encData[i] != input[inOff + i])
			{
				return false;
			}
		}
		return true;
	}

	private void Kdf(IDigest digest, ECPoint c1, byte[] encData)
	{
		int digestSize = digest.GetDigestSize();
		byte[] array = new byte[System.Math.Max(4, digestSize)];
		int i = 0;
		IMemoable memoable = digest as IMemoable;
		IMemoable other = null;
		if (memoable != null)
		{
			AddFieldElement(digest, c1.AffineXCoord);
			AddFieldElement(digest, c1.AffineYCoord);
			other = memoable.Copy();
		}
		uint num = 0u;
		int num2;
		for (; i < encData.Length; i += num2)
		{
			if (memoable != null)
			{
				memoable.Reset(other);
			}
			else
			{
				AddFieldElement(digest, c1.AffineXCoord);
				AddFieldElement(digest, c1.AffineYCoord);
			}
			num2 = System.Math.Min(digestSize, encData.Length - i);
			Pack.UInt32_To_BE(++num, array, 0);
			digest.BlockUpdate(array, 0, 4);
			digest.DoFinal(array, 0);
			Bytes.XorTo(num2, array, 0, encData, i);
		}
	}

	private BigInteger NextK()
	{
		int bitLength = mECParams.N.BitLength;
		BigInteger bigInteger;
		do
		{
			bigInteger = new BigInteger(bitLength, mRandom);
		}
		while (bigInteger.SignValue == 0 || bigInteger.CompareTo(mECParams.N) >= 0);
		return bigInteger;
	}

	private void AddFieldElement(IDigest digest, ECFieldElement v)
	{
		byte[] encoded = v.GetEncoded();
		digest.BlockUpdate(encoded, 0, encoded.Length);
	}
}
