using System;
using System.Threading;
using Mirror.BouncyCastle.Crypto.Parameters;
using Mirror.BouncyCastle.Security;
using Mirror.BouncyCastle.Utilities;

namespace Mirror.BouncyCastle.Crypto.Encodings;

public class Pkcs1Encoding : IAsymmetricBlockCipher
{
	public const string StrictLengthEnabledProperty = "Mirror.BouncyCastle.Pkcs1.Strict";

	private const int HeaderLength = 10;

	private static long m_strictLengthEnabled;

	private SecureRandom random;

	private IAsymmetricBlockCipher engine;

	private bool forEncryption;

	private bool forPrivateKey;

	private bool useStrictLength;

	private int pLen = -1;

	private byte[] fallback;

	private byte[] blockBuffer;

	public static bool StrictLengthEnabled
	{
		get
		{
			return Convert.ToBoolean(Interlocked.Read(ref m_strictLengthEnabled));
		}
		set
		{
			Interlocked.Exchange(ref m_strictLengthEnabled, Convert.ToInt64(value));
		}
	}

	public string AlgorithmName => engine.AlgorithmName + "/PKCS1Padding";

	public IAsymmetricBlockCipher UnderlyingCipher => engine;

	static Pkcs1Encoding()
	{
		string environmentVariable = Platform.GetEnvironmentVariable("Mirror.BouncyCastle.Pkcs1.Strict");
		m_strictLengthEnabled = Convert.ToInt64(environmentVariable == null || Platform.EqualsIgnoreCase("true", environmentVariable));
	}

	public Pkcs1Encoding(IAsymmetricBlockCipher cipher)
	{
		engine = cipher;
		useStrictLength = StrictLengthEnabled;
	}

	public Pkcs1Encoding(IAsymmetricBlockCipher cipher, int pLen)
	{
		engine = cipher;
		useStrictLength = StrictLengthEnabled;
		this.pLen = pLen;
	}

	public Pkcs1Encoding(IAsymmetricBlockCipher cipher, byte[] fallback)
	{
		engine = cipher;
		useStrictLength = StrictLengthEnabled;
		this.fallback = fallback;
		pLen = fallback.Length;
	}

	public void Init(bool forEncryption, ICipherParameters parameters)
	{
		AsymmetricKeyParameter asymmetricKeyParameter;
		if (parameters is ParametersWithRandom parametersWithRandom)
		{
			asymmetricKeyParameter = (AsymmetricKeyParameter)parametersWithRandom.Parameters;
			random = parametersWithRandom.Random;
		}
		else
		{
			asymmetricKeyParameter = (AsymmetricKeyParameter)parameters;
			random = ((forEncryption && !asymmetricKeyParameter.IsPrivate) ? CryptoServicesRegistrar.GetSecureRandom() : null);
		}
		engine.Init(forEncryption, parameters);
		forPrivateKey = asymmetricKeyParameter.IsPrivate;
		this.forEncryption = forEncryption;
		blockBuffer = new byte[engine.GetOutputBlockSize()];
	}

	public int GetInputBlockSize()
	{
		int inputBlockSize = engine.GetInputBlockSize();
		if (!forEncryption)
		{
			return inputBlockSize;
		}
		return inputBlockSize - 10;
	}

	public int GetOutputBlockSize()
	{
		int outputBlockSize = engine.GetOutputBlockSize();
		if (!forEncryption)
		{
			return outputBlockSize - 10;
		}
		return outputBlockSize;
	}

	public byte[] ProcessBlock(byte[] input, int inOff, int length)
	{
		if (!forEncryption)
		{
			return DecodeBlock(input, inOff, length);
		}
		return EncodeBlock(input, inOff, length);
	}

	private byte[] EncodeBlock(byte[] input, int inOff, int inLen)
	{
		if (inLen > GetInputBlockSize())
		{
			throw new ArgumentException("input data too large", "inLen");
		}
		byte[] array = new byte[engine.GetInputBlockSize()];
		int num = array.Length - 1 - inLen;
		if (forPrivateKey)
		{
			array[0] = 1;
			for (int i = 1; i < num; i++)
			{
				array[i] = byte.MaxValue;
			}
		}
		else
		{
			random.NextBytes(array);
			array[0] = 2;
			for (int j = 1; j < num; j++)
			{
				while (array[j] == 0)
				{
					array[j] = (byte)random.NextInt();
				}
			}
		}
		array[num] = 0;
		Array.Copy(input, inOff, array, array.Length - inLen, inLen);
		return engine.ProcessBlock(array, 0, array.Length);
	}

	private static int CheckPkcs1Encoding1(byte[] buf)
	{
		int num = 0;
		int num2 = 0;
		int num3 = -(buf[0] ^ 1);
		for (int i = 1; i < buf.Length; i++)
		{
			byte num4 = buf[i];
			int num5 = (num4 ^ 0) - 1 >> 31;
			int num6 = (num4 ^ 0xFF) - 1 >> 31;
			num2 ^= i & ~num & num5;
			num |= num5;
			num3 |= ~(num | num6);
		}
		num3 |= num2 - 9;
		return (buf.Length - 1 - num2) | (num3 >> 31);
	}

	private static int CheckPkcs1Encoding2(byte[] buf)
	{
		int num = 0;
		int num2 = 0;
		int num3 = -(buf[0] ^ 2);
		for (int i = 1; i < buf.Length; i++)
		{
			int num4 = (buf[i] ^ 0) - 1 >> 31;
			num2 ^= i & ~num & num4;
			num |= num4;
		}
		num3 |= num2 - 9;
		return (buf.Length - 1 - num2) | (num3 >> 31);
	}

	private static int CheckPkcs1Encoding2(byte[] buf, int plaintextLength)
	{
		int num = -(buf[0] ^ 2);
		int num2 = buf.Length - 1 - plaintextLength;
		num |= num2 - 9;
		for (int i = 1; i < num2; i++)
		{
			num |= buf[i] - 1;
		}
		num |= -buf[num2];
		return num >> 31;
	}

	private byte[] DecodeBlockOrRandom(byte[] input, int inOff, int inLen)
	{
		if (!forPrivateKey)
		{
			throw new InvalidCipherTextException("sorry, this method is only for decryption, not for signing");
		}
		int num = pLen;
		byte[] nextBytes = fallback;
		if (fallback == null)
		{
			nextBytes = SecureRandom.GetNextBytes(random, num);
		}
		int num2 = 0;
		int outputBlockSize = engine.GetOutputBlockSize();
		byte[] array = engine.ProcessBlock(input, inOff, inLen);
		byte[] array2 = array;
		if (array.Length != outputBlockSize && (useStrictLength || array.Length < outputBlockSize))
		{
			array2 = blockBuffer;
		}
		num2 |= CheckPkcs1Encoding2(array2, num);
		int num3 = array2.Length - num;
		byte[] array3 = new byte[num];
		for (int i = 0; i < num; i++)
		{
			array3[i] = (byte)((array2[num3 + i] & ~num2) | (nextBytes[i] & num2));
		}
		Arrays.Fill(array, 0);
		Arrays.Fill(blockBuffer, 0, System.Math.Max(0, blockBuffer.Length - array.Length), 0);
		return array3;
	}

	private byte[] DecodeBlock(byte[] input, int inOff, int inLen)
	{
		if (forPrivateKey && pLen != -1)
		{
			return DecodeBlockOrRandom(input, inOff, inLen);
		}
		int outputBlockSize = engine.GetOutputBlockSize();
		byte[] array = engine.ProcessBlock(input, inOff, inLen);
		bool flag = useStrictLength & (array.Length != outputBlockSize);
		byte[] array2 = array;
		if (array.Length < outputBlockSize)
		{
			array2 = blockBuffer;
		}
		int num = (forPrivateKey ? CheckPkcs1Encoding2(array2) : CheckPkcs1Encoding1(array2));
		try
		{
			if (num < 0)
			{
				throw new InvalidCipherTextException("block incorrect");
			}
			if (flag)
			{
				throw new InvalidCipherTextException("block incorrect size");
			}
			byte[] array3 = new byte[num];
			Array.Copy(array2, array2.Length - num, array3, 0, num);
			return array3;
		}
		finally
		{
			Arrays.Fill(array, 0);
			Arrays.Fill(blockBuffer, 0, System.Math.Max(0, blockBuffer.Length - array.Length), 0);
		}
	}
}
