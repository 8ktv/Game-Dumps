using System;
using Mirror.BouncyCastle.Crypto.Digests;
using Mirror.BouncyCastle.Crypto.Modes;
using Mirror.BouncyCastle.Crypto.Parameters;
using Mirror.BouncyCastle.Security;
using Mirror.BouncyCastle.Utilities;

namespace Mirror.BouncyCastle.Crypto.Engines;

public class DesEdeWrapEngine : IWrapper
{
	private CbcBlockCipher engine;

	private KeyParameter param;

	private ParametersWithIV paramPlusIV;

	private byte[] iv;

	private bool forWrapping;

	private static readonly byte[] IV2 = new byte[8] { 74, 221, 162, 44, 121, 232, 33, 5 };

	private readonly IDigest sha1 = new Sha1Digest();

	private readonly byte[] digest = new byte[20];

	public virtual string AlgorithmName => "DESede";

	public virtual void Init(bool forWrapping, ICipherParameters parameters)
	{
		this.forWrapping = forWrapping;
		engine = new CbcBlockCipher(new DesEdeEngine());
		SecureRandom secureRandom = null;
		if (parameters is ParametersWithRandom parametersWithRandom)
		{
			parameters = parametersWithRandom.Parameters;
			secureRandom = parametersWithRandom.Random;
		}
		if (parameters is KeyParameter keyParameter)
		{
			param = keyParameter;
			if (this.forWrapping)
			{
				iv = new byte[8];
				CryptoServicesRegistrar.GetSecureRandom(secureRandom).NextBytes(iv);
				paramPlusIV = new ParametersWithIV(param, iv);
			}
		}
		else if (parameters is ParametersWithIV parametersWithIV)
		{
			if (!forWrapping)
			{
				throw new ArgumentException("You should not supply an IV for unwrapping");
			}
			paramPlusIV = parametersWithIV;
			iv = parametersWithIV.GetIV();
			param = (KeyParameter)parametersWithIV.Parameters;
			if (iv.Length != 8)
			{
				throw new ArgumentException("IV is not 8 octets", "parameters");
			}
		}
	}

	public virtual byte[] Wrap(byte[] input, int inOff, int length)
	{
		if (!forWrapping)
		{
			throw new InvalidOperationException("Not initialized for wrapping");
		}
		byte[] array = new byte[length];
		Array.Copy(input, inOff, array, 0, length);
		byte[] array2 = CalculateCmsKeyChecksum(array);
		byte[] array3 = new byte[array.Length + array2.Length];
		Array.Copy(array, 0, array3, 0, array.Length);
		Array.Copy(array2, 0, array3, array.Length, array2.Length);
		int blockSize = engine.GetBlockSize();
		if (array3.Length % blockSize != 0)
		{
			throw new InvalidOperationException("Not multiple of block length");
		}
		engine.Init(forEncryption: true, paramPlusIV);
		byte[] array4 = new byte[array3.Length];
		for (int i = 0; i != array3.Length; i += blockSize)
		{
			engine.ProcessBlock(array3, i, array4, i);
		}
		byte[] array5 = new byte[iv.Length + array4.Length];
		Array.Copy(iv, 0, array5, 0, iv.Length);
		Array.Copy(array4, 0, array5, iv.Length, array4.Length);
		Array.Reverse((Array)array5);
		ParametersWithIV parameters = new ParametersWithIV(param, IV2);
		engine.Init(forEncryption: true, parameters);
		for (int j = 0; j != array5.Length; j += blockSize)
		{
			engine.ProcessBlock(array5, j, array5, j);
		}
		return array5;
	}

	public virtual byte[] Unwrap(byte[] input, int inOff, int length)
	{
		if (forWrapping)
		{
			throw new InvalidOperationException("Not set for unwrapping");
		}
		if (input == null)
		{
			throw new InvalidCipherTextException("Null pointer as ciphertext");
		}
		int blockSize = engine.GetBlockSize();
		if (length % blockSize != 0)
		{
			throw new InvalidCipherTextException("Ciphertext not multiple of " + blockSize);
		}
		ParametersWithIV parameters = new ParametersWithIV(param, IV2);
		engine.Init(forEncryption: false, parameters);
		byte[] array = new byte[length];
		for (int i = 0; i != array.Length; i += blockSize)
		{
			engine.ProcessBlock(input, inOff + i, array, i);
		}
		Array.Reverse((Array)array);
		iv = new byte[8];
		byte[] array2 = new byte[array.Length - 8];
		Array.Copy(array, 0, iv, 0, 8);
		Array.Copy(array, 8, array2, 0, array.Length - 8);
		paramPlusIV = new ParametersWithIV(param, iv);
		engine.Init(forEncryption: false, paramPlusIV);
		byte[] array3 = new byte[array2.Length];
		for (int j = 0; j != array3.Length; j += blockSize)
		{
			engine.ProcessBlock(array2, j, array3, j);
		}
		byte[] array4 = new byte[array3.Length - 8];
		byte[] array5 = new byte[8];
		Array.Copy(array3, 0, array4, 0, array3.Length - 8);
		Array.Copy(array3, array3.Length - 8, array5, 0, 8);
		if (!CheckCmsKeyChecksum(array4, array5))
		{
			throw new InvalidCipherTextException("Checksum inside ciphertext is corrupted");
		}
		return array4;
	}

	private byte[] CalculateCmsKeyChecksum(byte[] key)
	{
		sha1.BlockUpdate(key, 0, key.Length);
		sha1.DoFinal(digest, 0);
		byte[] array = new byte[8];
		Array.Copy(digest, 0, array, 0, 8);
		return array;
	}

	private bool CheckCmsKeyChecksum(byte[] key, byte[] checksum)
	{
		return Arrays.FixedTimeEquals(CalculateCmsKeyChecksum(key), checksum);
	}
}
