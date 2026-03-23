using System;
using Mirror.BouncyCastle.Utilities;

namespace Mirror.BouncyCastle.Crypto.Parameters;

public sealed class Blake3Parameters : ICipherParameters
{
	private const int KeyLen = 32;

	private byte[] m_theKey;

	private byte[] m_theContext;

	public static Blake3Parameters Context(byte[] pContext)
	{
		if (pContext == null)
		{
			throw new ArgumentNullException("pContext");
		}
		return new Blake3Parameters
		{
			m_theContext = Arrays.Clone(pContext)
		};
	}

	public static Blake3Parameters Key(byte[] pKey)
	{
		if (pKey == null)
		{
			throw new ArgumentNullException("pKey");
		}
		if (pKey.Length != 32)
		{
			throw new ArgumentException("Invalid key length", "pKey");
		}
		return new Blake3Parameters
		{
			m_theKey = Arrays.Clone(pKey)
		};
	}

	public byte[] GetKey()
	{
		return Arrays.Clone(m_theKey);
	}

	public void ClearKey()
	{
		Arrays.Fill(m_theKey, 0);
	}

	public byte[] GetContext()
	{
		return Arrays.Clone(m_theContext);
	}
}
