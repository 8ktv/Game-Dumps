using System;
using System.Security.Cryptography;

namespace Mirror.BouncyCastle.Crypto.Prng;

public sealed class CryptoApiRandomGenerator : IRandomGenerator, IDisposable
{
	private readonly RandomNumberGenerator m_randomNumberGenerator;

	public CryptoApiRandomGenerator()
		: this(RandomNumberGenerator.Create())
	{
	}

	public CryptoApiRandomGenerator(RandomNumberGenerator randomNumberGenerator)
	{
		m_randomNumberGenerator = randomNumberGenerator ?? throw new ArgumentNullException("randomNumberGenerator");
	}

	public void AddSeedMaterial(byte[] seed)
	{
	}

	public void AddSeedMaterial(long seed)
	{
	}

	public void NextBytes(byte[] bytes)
	{
		m_randomNumberGenerator.GetBytes(bytes);
	}

	public void NextBytes(byte[] bytes, int start, int len)
	{
		m_randomNumberGenerator.GetBytes(bytes, start, len);
	}

	public void Dispose()
	{
		m_randomNumberGenerator.Dispose();
		GC.SuppressFinalize(this);
	}
}
