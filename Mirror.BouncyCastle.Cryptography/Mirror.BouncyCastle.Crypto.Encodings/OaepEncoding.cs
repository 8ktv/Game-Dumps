using System;
using Mirror.BouncyCastle.Crypto.Digests;
using Mirror.BouncyCastle.Crypto.Parameters;
using Mirror.BouncyCastle.Crypto.Utilities;
using Mirror.BouncyCastle.Security;
using Mirror.BouncyCastle.Utilities;

namespace Mirror.BouncyCastle.Crypto.Encodings;

public class OaepEncoding : IAsymmetricBlockCipher
{
	private readonly IAsymmetricBlockCipher engine;

	private readonly IDigest mgf1Hash;

	private readonly int mgf1NoMemoLimit;

	private readonly byte[] defHash;

	private SecureRandom random;

	private bool forEncryption;

	public string AlgorithmName => engine.AlgorithmName + "/OAEPPadding";

	public IAsymmetricBlockCipher UnderlyingCipher => engine;

	private static int GetMgf1NoMemoLimit(IDigest d)
	{
		if (d is IMemoable)
		{
			return d.GetByteLength() - 1;
		}
		return int.MaxValue;
	}

	public OaepEncoding(IAsymmetricBlockCipher cipher)
		: this(cipher, new Sha1Digest(), null)
	{
	}

	public OaepEncoding(IAsymmetricBlockCipher cipher, IDigest hash)
		: this(cipher, hash, null)
	{
	}

	public OaepEncoding(IAsymmetricBlockCipher cipher, IDigest hash, byte[] encodingParams)
		: this(cipher, hash, hash, encodingParams)
	{
	}

	public OaepEncoding(IAsymmetricBlockCipher cipher, IDigest hash, IDigest mgf1Hash, byte[] encodingParams)
	{
		engine = cipher;
		this.mgf1Hash = mgf1Hash;
		mgf1NoMemoLimit = GetMgf1NoMemoLimit(mgf1Hash);
		defHash = new byte[hash.GetDigestSize()];
		hash.Reset();
		if (encodingParams != null)
		{
			hash.BlockUpdate(encodingParams, 0, encodingParams.Length);
		}
		hash.DoFinal(defHash, 0);
	}

	public void Init(bool forEncryption, ICipherParameters parameters)
	{
		SecureRandom secureRandom = null;
		if (parameters is ParametersWithRandom parametersWithRandom)
		{
			secureRandom = parametersWithRandom.Random;
		}
		random = (forEncryption ? CryptoServicesRegistrar.GetSecureRandom(secureRandom) : null);
		this.forEncryption = forEncryption;
		engine.Init(forEncryption, parameters);
	}

	public int GetInputBlockSize()
	{
		int inputBlockSize = engine.GetInputBlockSize();
		if (forEncryption)
		{
			return inputBlockSize - 1 - 2 * defHash.Length;
		}
		return inputBlockSize;
	}

	public int GetOutputBlockSize()
	{
		int outputBlockSize = engine.GetOutputBlockSize();
		if (forEncryption)
		{
			return outputBlockSize;
		}
		return outputBlockSize - 1 - 2 * defHash.Length;
	}

	public byte[] ProcessBlock(byte[] inBytes, int inOff, int inLen)
	{
		if (!forEncryption)
		{
			return DecodeBlock(inBytes, inOff, inLen);
		}
		return EncodeBlock(inBytes, inOff, inLen);
	}

	private byte[] EncodeBlock(byte[] inBytes, int inOff, int inLen)
	{
		int inputBlockSize = GetInputBlockSize();
		Check.DataLength(inLen > inputBlockSize, "input data too long");
		byte[] array = new byte[inputBlockSize + 1 + 2 * defHash.Length];
		Array.Copy(inBytes, inOff, array, array.Length - inLen, inLen);
		array[array.Length - inLen - 1] = 1;
		Array.Copy(defHash, 0, array, defHash.Length, defHash.Length);
		random.NextBytes(array, 0, defHash.Length);
		mgf1Hash.Reset();
		MaskGeneratorFunction(array, 0, defHash.Length, array, defHash.Length, array.Length - defHash.Length);
		MaskGeneratorFunction(array, defHash.Length, array.Length - defHash.Length, array, 0, defHash.Length);
		return engine.ProcessBlock(array, 0, array.Length);
	}

	private byte[] DecodeBlock(byte[] inBytes, int inOff, int inLen)
	{
		int num = GetOutputBlockSize() >> 31;
		byte[] array = new byte[engine.GetOutputBlockSize()];
		byte[] array2 = engine.ProcessBlock(inBytes, inOff, inLen);
		num |= array.Length - array2.Length >> 31;
		int num2 = System.Math.Min(array.Length, array2.Length);
		Array.Copy(array2, 0, array, array.Length - num2, num2);
		Array.Clear(array2, 0, array2.Length);
		mgf1Hash.Reset();
		MaskGeneratorFunction(array, defHash.Length, array.Length - defHash.Length, array, 0, defHash.Length);
		MaskGeneratorFunction(array, 0, defHash.Length, array, defHash.Length, array.Length - defHash.Length);
		for (int i = 0; i != defHash.Length; i++)
		{
			num |= defHash[i] ^ array[defHash.Length + i];
		}
		int num3 = -1;
		for (int j = 2 * defHash.Length; j != array.Length; j++)
		{
			int num4 = (-array[j] & num3) >> 31;
			num3 += j & num4;
		}
		num |= num3 >> 31;
		num3++;
		if ((num | (array[num3] ^ 1)) != 0)
		{
			Array.Clear(array, 0, array.Length);
			throw new InvalidCipherTextException("data wrong");
		}
		num3++;
		byte[] array3 = new byte[array.Length - num3];
		Array.Copy(array, num3, array3, 0, array3.Length);
		Array.Clear(array, 0, array.Length);
		return array3;
	}

	private void MaskGeneratorFunction(byte[] z, int zOff, int zLen, byte[] mask, int maskOff, int maskLen)
	{
		if (mgf1Hash is IXof xof)
		{
			byte[] array = new byte[maskLen];
			xof.BlockUpdate(z, zOff, zLen);
			xof.OutputFinal(array, 0, maskLen);
			Bytes.XorTo(maskLen, array, 0, mask, maskOff);
		}
		else
		{
			MaskGeneratorFunction1(z, zOff, zLen, mask, maskOff, maskLen);
		}
	}

	private void MaskGeneratorFunction1(byte[] z, int zOff, int zLen, byte[] mask, int maskOff, int maskLen)
	{
		int digestSize = mgf1Hash.GetDigestSize();
		byte[] array = new byte[digestSize];
		byte[] array2 = new byte[4];
		int n = 0;
		int num = maskOff + maskLen;
		int num2 = num - digestSize;
		int i = maskOff;
		mgf1Hash.BlockUpdate(z, zOff, zLen);
		if (zLen > mgf1NoMemoLimit)
		{
			IMemoable memoable = (IMemoable)mgf1Hash;
			IMemoable other = memoable.Copy();
			for (; i < num2; i += digestSize)
			{
				Pack.UInt32_To_BE((uint)n++, array2);
				mgf1Hash.BlockUpdate(array2, 0, array2.Length);
				mgf1Hash.DoFinal(array, 0);
				memoable.Reset(other);
				Bytes.XorTo(digestSize, array, 0, mask, i);
			}
		}
		else
		{
			for (; i < num2; i += digestSize)
			{
				Pack.UInt32_To_BE((uint)n++, array2);
				mgf1Hash.BlockUpdate(array2, 0, array2.Length);
				mgf1Hash.DoFinal(array, 0);
				mgf1Hash.BlockUpdate(z, zOff, zLen);
				Bytes.XorTo(digestSize, array, 0, mask, i);
			}
		}
		Pack.UInt32_To_BE((uint)n, array2);
		mgf1Hash.BlockUpdate(array2, 0, array2.Length);
		mgf1Hash.DoFinal(array, 0);
		Bytes.XorTo(num - i, array, 0, mask, i);
	}
}
