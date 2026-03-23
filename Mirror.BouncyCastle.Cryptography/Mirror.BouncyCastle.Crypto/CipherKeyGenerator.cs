using System;
using Mirror.BouncyCastle.Crypto.Parameters;
using Mirror.BouncyCastle.Security;

namespace Mirror.BouncyCastle.Crypto;

public class CipherKeyGenerator
{
	protected internal SecureRandom random;

	protected internal int strength;

	private bool uninitialised = true;

	private int defaultStrength;

	public int DefaultStrength => defaultStrength;

	public CipherKeyGenerator()
	{
	}

	internal CipherKeyGenerator(int defaultStrength)
	{
		if (defaultStrength < 1)
		{
			throw new ArgumentException("strength must be a positive value", "defaultStrength");
		}
		this.defaultStrength = defaultStrength;
	}

	public void Init(KeyGenerationParameters parameters)
	{
		if (parameters == null)
		{
			throw new ArgumentNullException("parameters");
		}
		uninitialised = false;
		EngineInit(parameters);
	}

	protected virtual void EngineInit(KeyGenerationParameters parameters)
	{
		random = parameters.Random;
		strength = (parameters.Strength + 7) / 8;
	}

	public byte[] GenerateKey()
	{
		EnsureInitialized();
		return EngineGenerateKey();
	}

	public KeyParameter GenerateKeyParameter()
	{
		EnsureInitialized();
		return EngineGenerateKeyParameter();
	}

	protected virtual byte[] EngineGenerateKey()
	{
		return SecureRandom.GetNextBytes(random, strength);
	}

	protected virtual KeyParameter EngineGenerateKeyParameter()
	{
		return new KeyParameter(EngineGenerateKey());
	}

	protected virtual void EnsureInitialized()
	{
		if (uninitialised)
		{
			if (defaultStrength < 1)
			{
				throw new InvalidOperationException("Generator has not been initialised");
			}
			uninitialised = false;
			EngineInit(new KeyGenerationParameters(CryptoServicesRegistrar.GetSecureRandom(), defaultStrength));
		}
	}
}
