using System;

namespace Mirror.BouncyCastle.Crypto.Parameters;

public class ParametersWithSalt : ICipherParameters
{
	private readonly ICipherParameters m_parameters;

	private readonly byte[] m_salt;

	public ICipherParameters Parameters => m_parameters;

	public ParametersWithSalt(ICipherParameters parameters, byte[] salt)
	{
		if (salt == null)
		{
			throw new ArgumentNullException("salt");
		}
		m_parameters = parameters;
		m_salt = (byte[])salt.Clone();
	}

	public ParametersWithSalt(ICipherParameters parameters, byte[] salt, int saltOff, int saltLen)
	{
		if (salt == null)
		{
			throw new ArgumentNullException("salt");
		}
		m_parameters = parameters;
		m_salt = new byte[saltLen];
		Array.Copy(salt, saltOff, m_salt, 0, saltLen);
	}

	public byte[] GetSalt()
	{
		return (byte[])m_salt.Clone();
	}
}
