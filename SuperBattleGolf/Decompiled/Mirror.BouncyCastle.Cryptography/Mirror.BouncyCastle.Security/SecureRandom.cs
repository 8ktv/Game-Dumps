using System;
using System.Threading;
using Mirror.BouncyCastle.Crypto;
using Mirror.BouncyCastle.Crypto.Prng;
using Mirror.BouncyCastle.Crypto.Utilities;

namespace Mirror.BouncyCastle.Security;

public class SecureRandom : Random
{
	private static long counter = DateTime.UtcNow.Ticks;

	private static readonly SecureRandom MasterRandom = new SecureRandom(new CryptoApiRandomGenerator());

	internal static readonly SecureRandom ArbitraryRandom = new SecureRandom(new VmpcRandomGenerator(), 16);

	protected readonly IRandomGenerator generator;

	private static readonly double DoubleScale = 1.0 / Convert.ToDouble(9007199254740992L);

	private static long NextCounterValue()
	{
		return Interlocked.Increment(ref counter);
	}

	private static DigestRandomGenerator CreatePrng(string digestName, bool autoSeed)
	{
		IDigest digest = DigestUtilities.GetDigest(digestName);
		if (digest == null)
		{
			return null;
		}
		DigestRandomGenerator result = new DigestRandomGenerator(digest);
		if (autoSeed)
		{
			AutoSeed(result, 2 * digest.GetDigestSize());
		}
		return result;
	}

	public static byte[] GetNextBytes(SecureRandom secureRandom, int length)
	{
		byte[] array = new byte[length];
		secureRandom.NextBytes(array);
		return array;
	}

	public static SecureRandom GetInstance(string algorithm)
	{
		return GetInstance(algorithm, autoSeed: true);
	}

	public static SecureRandom GetInstance(string algorithm, bool autoSeed)
	{
		if (algorithm == null)
		{
			throw new ArgumentNullException("algorithm");
		}
		if (algorithm.EndsWith("PRNG", StringComparison.OrdinalIgnoreCase))
		{
			DigestRandomGenerator digestRandomGenerator = CreatePrng(algorithm.Substring(0, algorithm.Length - "PRNG".Length), autoSeed);
			if (digestRandomGenerator != null)
			{
				return new SecureRandom(digestRandomGenerator);
			}
		}
		throw new ArgumentException("Unrecognised PRNG algorithm: " + algorithm, "algorithm");
	}

	public SecureRandom()
		: this(CreatePrng("SHA256", autoSeed: true))
	{
	}

	public SecureRandom(IRandomGenerator generator)
		: base(0)
	{
		this.generator = generator;
	}

	public SecureRandom(IRandomGenerator generator, int autoSeedLengthInBytes)
		: base(0)
	{
		AutoSeed(generator, autoSeedLengthInBytes);
		this.generator = generator;
	}

	public virtual byte[] GenerateSeed(int length)
	{
		return GetNextBytes(MasterRandom, length);
	}

	public virtual void SetSeed(byte[] seed)
	{
		generator.AddSeedMaterial(seed);
	}

	public virtual void SetSeed(long seed)
	{
		generator.AddSeedMaterial(seed);
	}

	public override int Next()
	{
		return NextInt() & 0x7FFFFFFF;
	}

	public override int Next(int maxValue)
	{
		if (maxValue < 2)
		{
			if (maxValue < 0)
			{
				throw new ArgumentOutOfRangeException("maxValue", "cannot be negative");
			}
			return 0;
		}
		int num;
		if ((maxValue & (maxValue - 1)) == 0)
		{
			num = NextInt() & 0x7FFFFFFF;
			return (int)((long)num * (long)maxValue >> 31);
		}
		int num2;
		do
		{
			num = NextInt() & 0x7FFFFFFF;
			num2 = num % maxValue;
		}
		while (num - num2 + (maxValue - 1) < 0);
		return num2;
	}

	public override int Next(int minValue, int maxValue)
	{
		if (maxValue <= minValue)
		{
			if (maxValue == minValue)
			{
				return minValue;
			}
			throw new ArgumentException("maxValue cannot be less than minValue");
		}
		int num = maxValue - minValue;
		if (num > 0)
		{
			return minValue + Next(num);
		}
		int num2;
		do
		{
			num2 = NextInt();
		}
		while (num2 < minValue || num2 >= maxValue);
		return num2;
	}

	public override void NextBytes(byte[] buf)
	{
		generator.NextBytes(buf);
	}

	public virtual void NextBytes(byte[] buf, int off, int len)
	{
		generator.NextBytes(buf, off, len);
	}

	public override double NextDouble()
	{
		return Convert.ToDouble((ulong)NextLong() >> 11) * DoubleScale;
	}

	public virtual int NextInt()
	{
		byte[] array = new byte[4];
		NextBytes(array);
		return (int)Pack.BE_To_UInt32(array);
	}

	public virtual long NextLong()
	{
		byte[] array = new byte[8];
		NextBytes(array);
		return (long)Pack.BE_To_UInt64(array);
	}

	private static void AutoSeed(IRandomGenerator generator, int seedLength)
	{
		generator.AddSeedMaterial(NextCounterValue());
		byte[] array = new byte[seedLength];
		MasterRandom.NextBytes(array);
		generator.AddSeedMaterial(array);
	}
}
