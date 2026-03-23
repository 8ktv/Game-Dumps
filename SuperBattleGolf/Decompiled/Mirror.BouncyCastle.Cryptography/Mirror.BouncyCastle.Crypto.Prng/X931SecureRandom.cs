using Mirror.BouncyCastle.Security;

namespace Mirror.BouncyCastle.Crypto.Prng;

public class X931SecureRandom : SecureRandom
{
	private readonly bool mPredictionResistant;

	private readonly SecureRandom mRandomSource;

	private readonly X931Rng mDrbg;

	internal X931SecureRandom(SecureRandom randomSource, X931Rng drbg, bool predictionResistant)
		: base(null)
	{
		mRandomSource = randomSource;
		mDrbg = drbg;
		mPredictionResistant = predictionResistant;
	}

	public override void SetSeed(byte[] seed)
	{
		lock (this)
		{
			if (mRandomSource != null)
			{
				mRandomSource.SetSeed(seed);
			}
		}
	}

	public override void SetSeed(long seed)
	{
		lock (this)
		{
			if (mRandomSource != null)
			{
				mRandomSource.SetSeed(seed);
			}
		}
	}

	public override void NextBytes(byte[] bytes)
	{
		NextBytes(bytes, 0, bytes.Length);
	}

	public override void NextBytes(byte[] buf, int off, int len)
	{
		lock (this)
		{
			if (mDrbg.Generate(buf, off, len, mPredictionResistant) < 0)
			{
				mDrbg.Reseed();
				mDrbg.Generate(buf, off, len, mPredictionResistant);
			}
		}
	}

	public override byte[] GenerateSeed(int numBytes)
	{
		return EntropyUtilities.GenerateSeed(mDrbg.EntropySource, numBytes);
	}
}
