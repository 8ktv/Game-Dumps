using Mirror.BouncyCastle.Crypto.Prng.Drbg;
using Mirror.BouncyCastle.Security;

namespace Mirror.BouncyCastle.Crypto.Prng;

public class SP800SecureRandom : SecureRandom
{
	private readonly IDrbgProvider mDrbgProvider;

	private readonly bool mPredictionResistant;

	private readonly SecureRandom mRandomSource;

	private readonly IEntropySource mEntropySource;

	private ISP80090Drbg mDrbg;

	internal SP800SecureRandom(SecureRandom randomSource, IEntropySource entropySource, IDrbgProvider drbgProvider, bool predictionResistant)
		: base(null)
	{
		mRandomSource = randomSource;
		mEntropySource = entropySource;
		mDrbgProvider = drbgProvider;
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
			if (mDrbg == null)
			{
				mDrbg = mDrbgProvider.Get(mEntropySource);
			}
			if (mDrbg.Generate(buf, off, len, null, mPredictionResistant) < 0)
			{
				mDrbg.Reseed(null);
				mDrbg.Generate(buf, off, len, null, mPredictionResistant);
			}
		}
	}

	public override byte[] GenerateSeed(int numBytes)
	{
		return EntropyUtilities.GenerateSeed(mEntropySource, numBytes);
	}

	public virtual void Reseed(byte[] additionalInput)
	{
		lock (this)
		{
			if (mDrbg == null)
			{
				mDrbg = mDrbgProvider.Get(mEntropySource);
			}
			mDrbg.Reseed(additionalInput);
		}
	}
}
