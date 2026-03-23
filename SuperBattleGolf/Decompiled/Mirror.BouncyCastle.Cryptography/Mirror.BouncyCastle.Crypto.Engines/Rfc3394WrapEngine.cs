using System;
using Mirror.BouncyCastle.Crypto.Parameters;
using Mirror.BouncyCastle.Utilities;

namespace Mirror.BouncyCastle.Crypto.Engines;

public class Rfc3394WrapEngine : IWrapper
{
	private static readonly byte[] DefaultIV = new byte[8] { 166, 166, 166, 166, 166, 166, 166, 166 };

	private readonly IBlockCipher m_engine;

	private readonly bool m_wrapCipherMode;

	private readonly byte[] m_iv = new byte[8];

	private KeyParameter m_key;

	private bool m_forWrapping = true;

	public virtual string AlgorithmName => m_engine.AlgorithmName;

	public Rfc3394WrapEngine(IBlockCipher engine)
		: this(engine, useReverseDirection: false)
	{
	}

	public Rfc3394WrapEngine(IBlockCipher engine, bool useReverseDirection)
	{
		m_engine = engine;
		m_wrapCipherMode = !useReverseDirection;
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
			Array.Copy(DefaultIV, 0, m_iv, 0, 8);
		}
		else if (parameters is ParametersWithIV parametersWithIV)
		{
			byte[] iV = parametersWithIV.GetIV();
			if (iV.Length != 8)
			{
				throw new ArgumentException("IV length not equal to 8", "parameters");
			}
			m_key = (KeyParameter)parametersWithIV.Parameters;
			Array.Copy(iV, 0, m_iv, 0, 8);
		}
	}

	public virtual byte[] Wrap(byte[] input, int inOff, int inLen)
	{
		if (!m_forWrapping)
		{
			throw new InvalidOperationException("not set for wrapping");
		}
		if (inLen < 8)
		{
			throw new DataLengthException("wrap data must be at least 8 bytes");
		}
		int num = inLen / 8;
		if (num * 8 != inLen)
		{
			throw new DataLengthException("wrap data must be a multiple of 8 bytes");
		}
		m_engine.Init(m_wrapCipherMode, m_key);
		byte[] array = new byte[inLen + 8];
		Array.Copy(m_iv, 0, array, 0, 8);
		Array.Copy(input, inOff, array, 8, inLen);
		if (num == 1)
		{
			m_engine.ProcessBlock(array, 0, array, 0);
		}
		else
		{
			byte[] array2 = new byte[16];
			for (int i = 0; i != 6; i++)
			{
				for (int j = 1; j <= num; j++)
				{
					Array.Copy(array, 0, array2, 0, 8);
					Array.Copy(array, 8 * j, array2, 8, 8);
					m_engine.ProcessBlock(array2, 0, array2, 0);
					uint num2 = (uint)(num * i + j);
					int num3 = 1;
					while (num2 != 0)
					{
						array2[8 - num3] ^= (byte)num2;
						num2 >>= 8;
						num3++;
					}
					Array.Copy(array2, 0, array, 0, 8);
					Array.Copy(array2, 8, array, 8 * j, 8);
				}
			}
		}
		return array;
	}

	public virtual byte[] Unwrap(byte[] input, int inOff, int inLen)
	{
		if (m_forWrapping)
		{
			throw new InvalidOperationException("not set for unwrapping");
		}
		if (inLen < 16)
		{
			throw new InvalidCipherTextException("unwrap data too short");
		}
		int num = inLen / 8;
		if (num * 8 != inLen)
		{
			throw new InvalidCipherTextException("unwrap data must be a multiple of 8 bytes");
		}
		m_engine.Init(!m_wrapCipherMode, m_key);
		byte[] array = new byte[inLen - 8];
		byte[] array2 = new byte[8];
		byte[] array3 = new byte[16];
		num--;
		if (num == 1)
		{
			m_engine.ProcessBlock(input, inOff, array3, 0);
			Array.Copy(array3, 0, array2, 0, 8);
			Array.Copy(array3, 8, array, 0, 8);
		}
		else
		{
			Array.Copy(input, inOff, array2, 0, 8);
			Array.Copy(input, inOff + 8, array, 0, inLen - 8);
			for (int num2 = 5; num2 >= 0; num2--)
			{
				for (int num3 = num; num3 >= 1; num3--)
				{
					Array.Copy(array2, 0, array3, 0, 8);
					Array.Copy(array, 8 * (num3 - 1), array3, 8, 8);
					uint num4 = (uint)(num * num2 + num3);
					int num5 = 1;
					while (num4 != 0)
					{
						array3[8 - num5] ^= (byte)num4;
						num4 >>= 8;
						num5++;
					}
					m_engine.ProcessBlock(array3, 0, array3, 0);
					Array.Copy(array3, 0, array2, 0, 8);
					Array.Copy(array3, 8, array, 8 * (num3 - 1), 8);
				}
			}
		}
		if (!Arrays.FixedTimeEquals(array2, m_iv))
		{
			throw new InvalidCipherTextException("checksum failed");
		}
		return array;
	}
}
