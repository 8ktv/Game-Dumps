using System;
using System.Collections.Generic;
using Mirror.BouncyCastle.Crypto.Utilities;
using Mirror.BouncyCastle.Utilities;

namespace Mirror.BouncyCastle.Crypto.Prng.Drbg;

public sealed class HashSP800Drbg : ISP80090Drbg
{
	private static readonly byte[] ONE;

	private static readonly long RESEED_MAX;

	private static readonly int MAX_BITS_REQUEST;

	private static readonly IDictionary<string, int> SeedLens;

	private readonly IDigest mDigest;

	private readonly IEntropySource mEntropySource;

	private readonly int mSecurityStrength;

	private readonly int mSeedLength;

	private byte[] mV;

	private byte[] mC;

	private long mReseedCounter;

	public int BlockSize => mDigest.GetDigestSize() * 8;

	static HashSP800Drbg()
	{
		ONE = new byte[1] { 1 };
		RESEED_MAX = 140737488355328L;
		MAX_BITS_REQUEST = 262144;
		SeedLens = new Dictionary<string, int>(StringComparer.OrdinalIgnoreCase);
		SeedLens.Add("SHA-1", 440);
		SeedLens.Add("SHA-224", 440);
		SeedLens.Add("SHA-256", 440);
		SeedLens.Add("SHA-512/256", 440);
		SeedLens.Add("SHA-512/224", 440);
		SeedLens.Add("SHA-384", 888);
		SeedLens.Add("SHA-512", 888);
	}

	public HashSP800Drbg(IDigest digest, int securityStrength, IEntropySource entropySource, byte[] personalizationString, byte[] nonce)
	{
		if (securityStrength > DrbgUtilities.GetMaxSecurityStrength(digest))
		{
			throw new ArgumentException("Requested security strength is not supported by the derivation function");
		}
		if (entropySource.EntropySize < securityStrength)
		{
			throw new ArgumentException("Not enough entropy for security strength required");
		}
		mDigest = digest;
		mEntropySource = entropySource;
		mSecurityStrength = securityStrength;
		mSeedLength = SeedLens[digest.AlgorithmName];
		byte[] entropy = GetEntropy();
		byte[] seedMaterial = Arrays.ConcatenateAll(entropy, nonce, personalizationString);
		mV = new byte[(mSeedLength + 7) / 8];
		DrbgUtilities.HashDF(mDigest, seedMaterial, mSeedLength, mV);
		byte[] array = new byte[mV.Length + 1];
		Array.Copy(mV, 0, array, 1, mV.Length);
		mC = new byte[(mSeedLength + 7) / 8];
		DrbgUtilities.HashDF(mDigest, array, mSeedLength, mC);
		mReseedCounter = 1L;
	}

	public int Generate(byte[] output, int outputOff, int outputLen, byte[] additionalInput, bool predictionResistant)
	{
		int num = outputLen * 8;
		if (num > MAX_BITS_REQUEST)
		{
			int mAX_BITS_REQUEST = MAX_BITS_REQUEST;
			throw new ArgumentException("Number of bits per request limited to " + mAX_BITS_REQUEST, "output");
		}
		if (mReseedCounter > RESEED_MAX)
		{
			return -1;
		}
		if (predictionResistant)
		{
			Reseed(additionalInput);
			additionalInput = null;
		}
		if (additionalInput != null)
		{
			byte[] array = new byte[1 + mV.Length + additionalInput.Length];
			array[0] = 2;
			Array.Copy(mV, 0, array, 1, mV.Length);
			Array.Copy(additionalInput, 0, array, 1 + mV.Length, additionalInput.Length);
			byte[] shorter = Hash(array);
			AddTo(mV, shorter);
		}
		byte[] sourceArray = Hashgen(mV, outputLen);
		byte[] array2 = new byte[mV.Length + 1];
		Array.Copy(mV, 0, array2, 1, mV.Length);
		array2[0] = 3;
		AddTo(shorter: Hash(array2), longer: mV);
		AddTo(mV, mC);
		byte[] array3 = new byte[4];
		Pack.UInt32_To_BE((uint)mReseedCounter, array3);
		AddTo(mV, array3);
		mReseedCounter++;
		Array.Copy(sourceArray, 0, output, outputOff, outputLen);
		return num;
	}

	private byte[] GetEntropy()
	{
		byte[] entropy = mEntropySource.GetEntropy();
		if (entropy.Length < (mSecurityStrength + 7) / 8)
		{
			throw new InvalidOperationException("Insufficient entropy provided by entropy source");
		}
		return entropy;
	}

	private void AddTo(byte[] longer, byte[] shorter)
	{
		int num = longer.Length - shorter.Length;
		uint num2 = 0u;
		int num3 = shorter.Length;
		while (--num3 >= 0)
		{
			num2 += (uint)(longer[num + num3] + shorter[num3]);
			longer[num + num3] = (byte)num2;
			num2 >>= 8;
		}
		num3 = num;
		while (--num3 >= 0)
		{
			num2 += longer[num3];
			longer[num3] = (byte)num2;
			num2 >>= 8;
		}
	}

	public void Reseed(byte[] additionalInput)
	{
		byte[] entropy = GetEntropy();
		byte[] seedMaterial = Arrays.ConcatenateAll(ONE, mV, entropy, additionalInput);
		DrbgUtilities.HashDF(mDigest, seedMaterial, mSeedLength, mV);
		byte[] array = new byte[mV.Length + 1];
		array[0] = 0;
		Array.Copy(mV, 0, array, 1, mV.Length);
		DrbgUtilities.HashDF(mDigest, array, mSeedLength, mC);
		mReseedCounter = 1L;
	}

	private void DoHash(byte[] input, byte[] output)
	{
		mDigest.BlockUpdate(input, 0, input.Length);
		mDigest.DoFinal(output, 0);
	}

	private byte[] Hash(byte[] input)
	{
		byte[] array = new byte[mDigest.GetDigestSize()];
		DoHash(input, array);
		return array;
	}

	private byte[] Hashgen(byte[] input, int length)
	{
		int digestSize = mDigest.GetDigestSize();
		int num = length / digestSize;
		byte[] array = (byte[])input.Clone();
		byte[] array2 = new byte[length];
		byte[] array3 = new byte[digestSize];
		for (int i = 0; i <= num; i++)
		{
			DoHash(array, array3);
			int length2 = System.Math.Min(digestSize, length - i * digestSize);
			Array.Copy(array3, 0, array2, i * digestSize, length2);
			AddTo(array, ONE);
		}
		return array2;
	}
}
