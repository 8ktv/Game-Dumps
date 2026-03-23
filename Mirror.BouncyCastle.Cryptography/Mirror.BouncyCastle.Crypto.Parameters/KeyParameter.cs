using System;
using Mirror.BouncyCastle.Utilities;

namespace Mirror.BouncyCastle.Crypto.Parameters;

public class KeyParameter : ICipherParameters
{
	private readonly byte[] m_key;

	public int KeyLength => m_key.Length;

	public KeyParameter(byte[] key)
	{
		if (key == null)
		{
			throw new ArgumentNullException("key");
		}
		m_key = (byte[])key.Clone();
	}

	public KeyParameter(byte[] key, int keyOff, int keyLen)
	{
		if (key == null)
		{
			throw new ArgumentNullException("key");
		}
		if (keyOff < 0 || keyOff > key.Length)
		{
			throw new ArgumentOutOfRangeException("keyOff");
		}
		if (keyLen < 0 || keyLen > key.Length - keyOff)
		{
			throw new ArgumentOutOfRangeException("keyLen");
		}
		m_key = new byte[keyLen];
		Array.Copy(key, keyOff, m_key, 0, keyLen);
	}

	private KeyParameter(int length)
	{
		if (length < 1)
		{
			throw new ArgumentOutOfRangeException("length");
		}
		m_key = new byte[length];
	}

	public void CopyTo(byte[] buf, int off, int len)
	{
		if (m_key.Length != len)
		{
			throw new ArgumentOutOfRangeException("len");
		}
		Array.Copy(m_key, 0, buf, off, len);
	}

	public byte[] GetKey()
	{
		return (byte[])m_key.Clone();
	}

	internal bool FixedTimeEquals(byte[] data)
	{
		return Arrays.FixedTimeEquals(m_key, data);
	}

	public KeyParameter Reverse()
	{
		KeyParameter keyParameter = new KeyParameter(m_key.Length);
		Arrays.Reverse(m_key, keyParameter.m_key);
		return keyParameter;
	}
}
