using System;

namespace Mirror.BouncyCastle.Crypto.Parameters;

public class ParametersWithIV : ICipherParameters
{
	private readonly ICipherParameters m_parameters;

	private readonly byte[] m_iv;

	public int IVLength => m_iv.Length;

	public ICipherParameters Parameters => m_parameters;

	internal static ICipherParameters ApplyOptionalIV(ICipherParameters parameters, byte[] iv)
	{
		if (iv != null)
		{
			return new ParametersWithIV(parameters, iv);
		}
		return parameters;
	}

	public ParametersWithIV(ICipherParameters parameters, byte[] iv)
		: this(parameters, iv, 0, iv.Length)
	{
		if (iv == null)
		{
			throw new ArgumentNullException("iv");
		}
		m_parameters = parameters;
		m_iv = (byte[])iv.Clone();
	}

	public ParametersWithIV(ICipherParameters parameters, byte[] iv, int ivOff, int ivLen)
	{
		if (iv == null)
		{
			throw new ArgumentNullException("iv");
		}
		m_parameters = parameters;
		m_iv = new byte[ivLen];
		Array.Copy(iv, ivOff, m_iv, 0, ivLen);
	}

	private ParametersWithIV(ICipherParameters parameters, int ivLength)
	{
		if (ivLength < 0)
		{
			throw new ArgumentOutOfRangeException("ivLength");
		}
		m_parameters = parameters;
		m_iv = new byte[ivLength];
	}

	public void CopyIVTo(byte[] buf, int off, int len)
	{
		if (m_iv.Length != len)
		{
			throw new ArgumentOutOfRangeException("len");
		}
		Array.Copy(m_iv, 0, buf, off, len);
	}

	public byte[] GetIV()
	{
		return (byte[])m_iv.Clone();
	}
}
