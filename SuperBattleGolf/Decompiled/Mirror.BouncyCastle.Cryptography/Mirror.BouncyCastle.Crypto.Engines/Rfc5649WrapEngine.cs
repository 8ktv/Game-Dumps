using System;
using Mirror.BouncyCastle.Crypto.Parameters;
using Mirror.BouncyCastle.Crypto.Utilities;
using Mirror.BouncyCastle.Utilities;

namespace Mirror.BouncyCastle.Crypto.Engines;

public class Rfc5649WrapEngine : IWrapper
{
	private static readonly byte[] DefaultIV = new byte[4] { 166, 89, 89, 166 };

	private readonly IBlockCipher m_engine;

	private readonly byte[] m_preIV = new byte[4];

	private KeyParameter m_key;

	private bool m_forWrapping = true;

	public virtual string AlgorithmName => m_engine.AlgorithmName;

	public Rfc5649WrapEngine(IBlockCipher engine)
	{
		m_engine = engine;
	}

	public virtual void Init(bool forWrapping, ICipherParameters parameters)
	{
		m_forWrapping = forWrapping;
		if (parameters is ParametersWithRandom parametersWithRandom)
		{
			parameters = parametersWithRandom.Parameters;
		}
		if (parameters is KeyParameter key)
		{
			m_key = key;
			Array.Copy(DefaultIV, 0, m_preIV, 0, 4);
		}
		else if (parameters is ParametersWithIV parametersWithIV)
		{
			byte[] iV = parametersWithIV.GetIV();
			if (iV.Length != 4)
			{
				throw new ArgumentException("IV length not equal to 4", "parameters");
			}
			m_key = (KeyParameter)parametersWithIV.Parameters;
			Array.Copy(iV, 0, m_preIV, 0, 4);
		}
	}

	public virtual byte[] Wrap(byte[] input, int inOff, int length)
	{
		if (!m_forWrapping)
		{
			throw new InvalidOperationException("not set for wrapping");
		}
		byte[] array = new byte[8];
		Array.Copy(m_preIV, 0, array, 0, 4);
		Pack.UInt32_To_BE((uint)length, array, 4);
		byte[] array2 = new byte[length];
		Array.Copy(input, inOff, array2, 0, length);
		byte[] array3 = PadPlaintext(array2);
		if (array3.Length == 8)
		{
			byte[] array4 = new byte[array3.Length + array.Length];
			Array.Copy(array, 0, array4, 0, array.Length);
			Array.Copy(array3, 0, array4, array.Length, array3.Length);
			m_engine.Init(forEncryption: true, m_key);
			int i = 0;
			for (int blockSize = m_engine.GetBlockSize(); i < array4.Length; i += blockSize)
			{
				m_engine.ProcessBlock(array4, i, array4, i);
			}
			return array4;
		}
		Rfc3394WrapEngine rfc3394WrapEngine = new Rfc3394WrapEngine(m_engine);
		ParametersWithIV parameters = new ParametersWithIV(m_key, array);
		rfc3394WrapEngine.Init(forWrapping: true, parameters);
		return rfc3394WrapEngine.Wrap(array3, 0, array3.Length);
	}

	public virtual byte[] Unwrap(byte[] input, int inOff, int length)
	{
		if (m_forWrapping)
		{
			throw new InvalidOperationException("not set for unwrapping");
		}
		int num = length / 8;
		if (num * 8 != length)
		{
			throw new InvalidCipherTextException("unwrap data must be a multiple of 8 bytes");
		}
		if (num <= 1)
		{
			throw new InvalidCipherTextException("unwrap data must be at least 16 bytes");
		}
		byte[] array = new byte[length];
		Array.Copy(input, inOff, array, 0, length);
		byte[] array2 = new byte[length];
		byte[] array3 = new byte[8];
		byte[] array4;
		if (num == 2)
		{
			m_engine.Init(forEncryption: false, m_key);
			int i = 0;
			for (int blockSize = m_engine.GetBlockSize(); i < array.Length; i += blockSize)
			{
				m_engine.ProcessBlock(array, i, array2, i);
			}
			Array.Copy(array2, 0, array3, 0, 8);
			array4 = new byte[array2.Length - 8];
			Array.Copy(array2, 8, array4, 0, array4.Length);
		}
		else
		{
			array2 = Rfc3394UnwrapNoIvCheck(input, inOff, length, array3);
			array4 = array2;
		}
		byte[] array5 = new byte[4];
		Array.Copy(array3, 0, array5, 0, 4);
		int num2 = (int)Pack.BE_To_UInt32(array3, 4);
		bool flag = Arrays.FixedTimeEquals(array5, m_preIV);
		int num3 = array4.Length;
		int num4 = num3 - 8;
		if (num2 <= num4)
		{
			flag = false;
		}
		if (num2 > num3)
		{
			flag = false;
		}
		int num5 = num3 - num2;
		if (num5 >= 8 || num5 < 0)
		{
			flag = false;
			num5 = 4;
		}
		byte[] b = new byte[num5];
		byte[] array6 = new byte[num5];
		Array.Copy(array4, array4.Length - num5, array6, 0, num5);
		if (!Arrays.FixedTimeEquals(array6, b))
		{
			flag = false;
		}
		if (!flag)
		{
			throw new InvalidCipherTextException("checksum failed");
		}
		byte[] array7 = new byte[num2];
		Array.Copy(array4, 0, array7, 0, array7.Length);
		return array7;
	}

	private byte[] Rfc3394UnwrapNoIvCheck(byte[] input, int inOff, int inLen, byte[] extractedAIV)
	{
		byte[] array = new byte[inLen - 8];
		byte[] array2 = new byte[16];
		Array.Copy(input, inOff, array2, 0, 8);
		Array.Copy(input, inOff + 8, array, 0, inLen - 8);
		m_engine.Init(forEncryption: false, m_key);
		int num = inLen / 8;
		num--;
		for (int num2 = 5; num2 >= 0; num2--)
		{
			for (int num3 = num; num3 >= 1; num3--)
			{
				Array.Copy(array, 8 * (num3 - 1), array2, 8, 8);
				uint num4 = (uint)(num * num2 + num3);
				int num5 = 1;
				while (num4 != 0)
				{
					array2[8 - num5] ^= (byte)num4;
					num4 >>= 8;
					num5++;
				}
				m_engine.ProcessBlock(array2, 0, array2, 0);
				Array.Copy(array2, 8, array, 8 * (num3 - 1), 8);
			}
		}
		Array.Copy(array2, 0, extractedAIV, 0, 8);
		return array;
	}

	private static byte[] PadPlaintext(byte[] plaintext)
	{
		int num = plaintext.Length;
		int num2 = (8 - num % 8) % 8;
		byte[] array = new byte[num + num2];
		Array.Copy(plaintext, 0, array, 0, num);
		if (num2 != 0)
		{
			Array.Copy(new byte[num2], 0, array, num, num2);
		}
		return array;
	}
}
