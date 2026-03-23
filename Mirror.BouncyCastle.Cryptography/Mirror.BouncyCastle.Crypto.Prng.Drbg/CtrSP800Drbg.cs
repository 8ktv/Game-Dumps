using System;
using Mirror.BouncyCastle.Crypto.Parameters;
using Mirror.BouncyCastle.Crypto.Utilities;
using Mirror.BouncyCastle.Utilities;
using Mirror.BouncyCastle.Utilities.Encoders;

namespace Mirror.BouncyCastle.Crypto.Prng.Drbg;

public sealed class CtrSP800Drbg : ISP80090Drbg
{
	private static readonly long TDEA_RESEED_MAX = 2147483648L;

	private static readonly long AES_RESEED_MAX = 140737488355328L;

	private static readonly int TDEA_MAX_BITS_REQUEST = 4096;

	private static readonly int AES_MAX_BITS_REQUEST = 262144;

	private readonly IEntropySource mEntropySource;

	private readonly IBlockCipher mEngine;

	private readonly int mKeySizeInBits;

	private readonly int mSeedLength;

	private readonly int mSecurityStrength;

	private byte[] mKey;

	private byte[] mV;

	private long mReseedCounter;

	private bool mIsTdea;

	private static readonly byte[] K_BITS = Hex.DecodeStrict("000102030405060708090A0B0C0D0E0F101112131415161718191A1B1C1D1E1F");

	public int BlockSize => mV.Length * 8;

	public CtrSP800Drbg(IBlockCipher engine, int keySizeInBits, int securityStrength, IEntropySource entropySource, byte[] personalizationString, byte[] nonce)
	{
		if (securityStrength > 256)
		{
			throw new ArgumentException("Requested security strength is not supported by the derivation function");
		}
		if (GetMaxSecurityStrength(engine, keySizeInBits) < securityStrength)
		{
			throw new ArgumentException("Requested security strength is not supported by block cipher and key size");
		}
		if (entropySource.EntropySize < securityStrength)
		{
			throw new ArgumentException("Not enough entropy for security strength required");
		}
		mEntropySource = entropySource;
		mEngine = engine;
		mKeySizeInBits = keySizeInBits;
		mSecurityStrength = securityStrength;
		mSeedLength = keySizeInBits + engine.GetBlockSize() * 8;
		mIsTdea = IsTdea(engine);
		CTR_DRBG_Instantiate_algorithm(personalizationString, nonce);
	}

	private void CTR_DRBG_Instantiate_algorithm(byte[] personalisationString, byte[] nonce)
	{
		byte[] entropy = GetEntropy();
		byte[] input = Arrays.ConcatenateAll(entropy, nonce, personalisationString);
		byte[] seed = BlockCipherDF(input, mSeedLength / 8);
		int blockSize = mEngine.GetBlockSize();
		mKey = new byte[(mKeySizeInBits + 7) / 8];
		mV = new byte[blockSize];
		CTR_DRBG_Update(seed, mKey, mV);
		mReseedCounter = 1L;
	}

	private void CTR_DRBG_Update(byte[] seed, byte[] key, byte[] v)
	{
		int num = seed.Length;
		byte[] array = new byte[num];
		byte[] array2 = new byte[mEngine.GetBlockSize()];
		int i = 0;
		int blockSize = mEngine.GetBlockSize();
		mEngine.Init(forEncryption: true, ExpandToKeyParameter(key));
		for (; i * blockSize < num; i++)
		{
			AddOneTo(v);
			mEngine.ProcessBlock(v, 0, array2, 0);
			int length = System.Math.Min(blockSize, array.Length - i * blockSize);
			Array.Copy(array2, 0, array, i * blockSize, length);
		}
		Bytes.XorTo(num, seed, array);
		Array.Copy(array, 0, key, 0, key.Length);
		Array.Copy(array, key.Length, v, 0, v.Length);
	}

	private void CTR_DRBG_Reseed_algorithm(byte[] additionalInput)
	{
		byte[] entropy = GetEntropy();
		byte[] array = Arrays.Concatenate(entropy, additionalInput);
		Array.Clear(entropy, 0, entropy.Length);
		byte[] array2 = BlockCipherDF(array, mSeedLength / 8);
		Array.Clear(array, 0, array.Length);
		CTR_DRBG_Update(array2, mKey, mV);
		Array.Clear(array2, 0, array2.Length);
		mReseedCounter = 1L;
	}

	private void AddOneTo(byte[] longer)
	{
		uint num = 1u;
		int num2 = longer.Length;
		while (--num2 >= 0)
		{
			num += longer[num2];
			longer[num2] = (byte)num;
			num >>= 8;
		}
	}

	private byte[] GetEntropy()
	{
		byte[] entropy = mEntropySource.GetEntropy();
		if (entropy == null || entropy.Length < (mSecurityStrength + 7) / 8)
		{
			throw new InvalidOperationException("Insufficient entropy provided by entropy source");
		}
		return entropy;
	}

	private byte[] BlockCipherDF(byte[] input, int N)
	{
		int blockSize = mEngine.GetBlockSize();
		int num = input.Length;
		byte[] array = new byte[(8 + num + 1 + blockSize - 1) / blockSize * blockSize];
		Pack.UInt32_To_BE((uint)num, array, 0);
		Pack.UInt32_To_BE((uint)N, array, 4);
		Array.Copy(input, 0, array, 8, num);
		array[8 + num] = 128;
		byte[] array2 = new byte[mKeySizeInBits / 8 + blockSize];
		byte[] array3 = new byte[blockSize];
		byte[] array4 = new byte[blockSize];
		int i = 0;
		byte[] array5 = new byte[mKeySizeInBits / 8];
		Array.Copy(K_BITS, 0, array5, 0, array5.Length);
		KeyParameter parameters = ExpandToKeyParameter(array5);
		mEngine.Init(forEncryption: true, parameters);
		for (; i * blockSize * 8 < mKeySizeInBits + blockSize * 8; i++)
		{
			Pack.UInt32_To_BE((uint)i, array4, 0);
			BCC(array3, array4, array);
			int length = System.Math.Min(blockSize, array2.Length - i * blockSize);
			Array.Copy(array3, 0, array2, i * blockSize, length);
		}
		byte[] array6 = new byte[blockSize];
		Array.Copy(array2, 0, array5, 0, array5.Length);
		Array.Copy(array2, array5.Length, array6, 0, array6.Length);
		array2 = new byte[N];
		i = 0;
		mEngine.Init(forEncryption: true, ExpandToKeyParameter(array5));
		for (; i * blockSize < array2.Length; i++)
		{
			mEngine.ProcessBlock(array6, 0, array6, 0);
			int length2 = System.Math.Min(blockSize, array2.Length - i * blockSize);
			Array.Copy(array6, 0, array2, i * blockSize, length2);
		}
		return array2;
	}

	private void BCC(byte[] bccOut, byte[] iV, byte[] data)
	{
		int blockSize = mEngine.GetBlockSize();
		byte[] array = new byte[blockSize];
		byte[] array2 = new byte[blockSize];
		mEngine.ProcessBlock(iV, 0, array, 0);
		int num = data.Length / blockSize;
		for (int i = 0; i < num; i++)
		{
			Bytes.Xor(blockSize, array, 0, data, i * blockSize, array2, 0);
			mEngine.ProcessBlock(array2, 0, array, 0);
		}
		Array.Copy(array, 0, bccOut, 0, bccOut.Length);
	}

	public int Generate(byte[] output, int outputOff, int outputLen, byte[] additionalInput, bool predictionResistant)
	{
		if (mIsTdea)
		{
			if (mReseedCounter > TDEA_RESEED_MAX)
			{
				return -1;
			}
			if (outputLen > TDEA_MAX_BITS_REQUEST / 8)
			{
				int tDEA_MAX_BITS_REQUEST = TDEA_MAX_BITS_REQUEST;
				throw new ArgumentException("Number of bits per request limited to " + tDEA_MAX_BITS_REQUEST, "output");
			}
		}
		else
		{
			if (mReseedCounter > AES_RESEED_MAX)
			{
				return -1;
			}
			if (outputLen > AES_MAX_BITS_REQUEST / 8)
			{
				int tDEA_MAX_BITS_REQUEST = AES_MAX_BITS_REQUEST;
				throw new ArgumentException("Number of bits per request limited to " + tDEA_MAX_BITS_REQUEST, "output");
			}
		}
		if (predictionResistant)
		{
			CTR_DRBG_Reseed_algorithm(additionalInput);
			additionalInput = null;
		}
		if (additionalInput != null)
		{
			additionalInput = BlockCipherDF(additionalInput, mSeedLength / 8);
			CTR_DRBG_Update(additionalInput, mKey, mV);
		}
		else
		{
			additionalInput = new byte[mSeedLength];
		}
		byte[] array = new byte[mV.Length];
		mEngine.Init(forEncryption: true, ExpandToKeyParameter(mKey));
		int i = 0;
		for (int num = outputLen / array.Length; i <= num; i++)
		{
			int num2 = System.Math.Min(array.Length, outputLen - i * array.Length);
			if (num2 != 0)
			{
				AddOneTo(mV);
				mEngine.ProcessBlock(mV, 0, array, 0);
				Array.Copy(array, 0, output, outputOff + i * array.Length, num2);
			}
		}
		CTR_DRBG_Update(additionalInput, mKey, mV);
		mReseedCounter++;
		return outputLen * 8;
	}

	public void Reseed(byte[] additionalInput)
	{
		CTR_DRBG_Reseed_algorithm(additionalInput);
	}

	private bool IsTdea(IBlockCipher cipher)
	{
		if (!cipher.AlgorithmName.Equals("DESede"))
		{
			return cipher.AlgorithmName.Equals("TDEA");
		}
		return true;
	}

	private int GetMaxSecurityStrength(IBlockCipher cipher, int keySizeInBits)
	{
		if (IsTdea(cipher) && keySizeInBits == 168)
		{
			return 112;
		}
		if (cipher.AlgorithmName.Equals("AES"))
		{
			return keySizeInBits;
		}
		return -1;
	}

	private KeyParameter ExpandToKeyParameter(byte[] key)
	{
		if (!mIsTdea)
		{
			return new KeyParameter(key);
		}
		byte[] array = new byte[24];
		PadKey(key, 0, array, 0);
		PadKey(key, 7, array, 8);
		PadKey(key, 14, array, 16);
		return new KeyParameter(array);
	}

	private void PadKey(byte[] keyMaster, int keyOff, byte[] tmp, int tmpOff)
	{
		tmp[tmpOff] = (byte)(keyMaster[keyOff] & 0xFE);
		tmp[tmpOff + 1] = (byte)((keyMaster[keyOff] << 7) | ((keyMaster[keyOff + 1] & 0xFC) >> 1));
		tmp[tmpOff + 2] = (byte)((keyMaster[keyOff + 1] << 6) | ((keyMaster[keyOff + 2] & 0xF8) >> 2));
		tmp[tmpOff + 3] = (byte)((keyMaster[keyOff + 2] << 5) | ((keyMaster[keyOff + 3] & 0xF0) >> 3));
		tmp[tmpOff + 4] = (byte)((keyMaster[keyOff + 3] << 4) | ((keyMaster[keyOff + 4] & 0xE0) >> 4));
		tmp[tmpOff + 5] = (byte)((keyMaster[keyOff + 4] << 3) | ((keyMaster[keyOff + 5] & 0xC0) >> 5));
		tmp[tmpOff + 6] = (byte)((keyMaster[keyOff + 5] << 2) | ((keyMaster[keyOff + 6] & 0x80) >> 6));
		tmp[tmpOff + 7] = (byte)(keyMaster[keyOff + 6] << 1);
		DesParameters.SetOddParity(tmp, tmpOff, 8);
	}
}
