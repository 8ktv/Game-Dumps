using System;
using Mirror.BouncyCastle.Crypto.Digests;
using Mirror.BouncyCastle.Crypto.Modes;
using Mirror.BouncyCastle.Crypto.Parameters;
using Mirror.BouncyCastle.Security;
using Mirror.BouncyCastle.Utilities;

namespace Mirror.BouncyCastle.Crypto.Engines;

public class RC2WrapEngine : IWrapper
{
	private CbcBlockCipher engine;

	private ICipherParameters parameters;

	private ParametersWithIV paramPlusIV;

	private byte[] iv;

	private bool forWrapping;

	private SecureRandom sr;

	private static readonly byte[] IV2 = new byte[8] { 74, 221, 162, 44, 121, 232, 33, 5 };

	private readonly IDigest sha1 = new Sha1Digest();

	private readonly byte[] digest = new byte[20];

	public virtual string AlgorithmName => "RC2";

	public virtual void Init(bool forWrapping, ICipherParameters parameters)
	{
		this.forWrapping = forWrapping;
		engine = new CbcBlockCipher(new RC2Engine());
		if (parameters is ParametersWithRandom parametersWithRandom)
		{
			sr = parametersWithRandom.Random;
			parameters = parametersWithRandom.Parameters;
		}
		else
		{
			sr = (forWrapping ? CryptoServicesRegistrar.GetSecureRandom() : null);
		}
		if (parameters is ParametersWithIV)
		{
			if (!forWrapping)
			{
				throw new ArgumentException("You should not supply an IV for unwrapping");
			}
			paramPlusIV = (ParametersWithIV)parameters;
			iv = paramPlusIV.GetIV();
			this.parameters = paramPlusIV.Parameters;
			if (iv.Length != 8)
			{
				throw new ArgumentException("IV is not 8 octets");
			}
		}
		else
		{
			this.parameters = parameters;
			if (this.forWrapping)
			{
				iv = new byte[8];
				sr.NextBytes(iv);
				paramPlusIV = new ParametersWithIV(this.parameters, iv);
			}
		}
	}

	public virtual byte[] Wrap(byte[] input, int inOff, int length)
	{
		if (!forWrapping)
		{
			throw new InvalidOperationException("Not initialized for wrapping");
		}
		int num = (length + 8) & -8;
		int num2 = iv.Length;
		byte[] array = Arrays.CopyOf(iv, num2 + num + 8);
		array[num2] = (byte)length;
		Array.Copy(input, inOff, array, num2 + 1, length);
		int num3 = num - length - 1;
		if (num3 > 0)
		{
			sr.NextBytes(array, num2 + num - num3, num3);
		}
		CalculateCmsKeyChecksum(array, num2, num, array, num2 + num);
		int blockSize = engine.GetBlockSize();
		engine.Init(forEncryption: true, paramPlusIV);
		int i;
		for (i = num2; i < array.Length; i += blockSize)
		{
			engine.ProcessBlock(array, i, array, i);
		}
		if (i != array.Length)
		{
			throw new InvalidOperationException("Not multiple of block length");
		}
		Array.Reverse((Array)array);
		engine.Init(forEncryption: true, new ParametersWithIV(parameters, IV2));
		int j;
		for (j = 0; j < array.Length; j += blockSize)
		{
			engine.ProcessBlock(array, j, array, j);
		}
		if (j != array.Length)
		{
			throw new InvalidOperationException("Not multiple of block length");
		}
		return array;
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
		if (length % engine.GetBlockSize() != 0)
		{
			throw new InvalidCipherTextException("Ciphertext not multiple of " + engine.GetBlockSize());
		}
		int blockSize = engine.GetBlockSize();
		byte[] array = new byte[length];
		engine.Init(forEncryption: false, new ParametersWithIV(parameters, IV2));
		int i;
		for (i = 0; i < array.Length; i += blockSize)
		{
			engine.ProcessBlock(input, inOff + i, array, i);
		}
		if (i != array.Length)
		{
			throw new InvalidOperationException("Not multiple of block length");
		}
		Array.Reverse((Array)array);
		iv = Arrays.CopyOf(array, 8);
		paramPlusIV = new ParametersWithIV(parameters, iv);
		engine.Init(forEncryption: false, paramPlusIV);
		int j;
		for (j = 8; j < array.Length; j += blockSize)
		{
			engine.ProcessBlock(array, j, array, j);
		}
		if (j != array.Length)
		{
			throw new InvalidOperationException("Not multiple of block length");
		}
		if (!CheckCmsKeyChecksum(array, 8, array.Length - 16, array, array.Length - 8))
		{
			throw new InvalidCipherTextException("Checksum inside ciphertext is corrupted");
		}
		int num = array.Length - 16 - array[8] - 1;
		if ((num & 7) != num)
		{
			throw new InvalidCipherTextException("Invalid padding length (" + num + ")");
		}
		return Arrays.CopyOfRange(array, 9, 9 + array[8]);
	}

	private void CalculateCmsKeyChecksum(byte[] key, int keyOff, int keyLen, byte[] cks, int cksOff)
	{
		sha1.BlockUpdate(key, keyOff, keyLen);
		sha1.DoFinal(digest, 0);
		Array.Copy(digest, 0, cks, cksOff, 8);
	}

	private bool CheckCmsKeyChecksum(byte[] key, int keyOff, int keyLen, byte[] cks, int cksOff)
	{
		sha1.BlockUpdate(key, keyOff, keyLen);
		sha1.DoFinal(digest, 0);
		return Arrays.FixedTimeEquals(8, digest, 0, cks, cksOff);
	}
}
