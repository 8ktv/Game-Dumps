using System;

namespace Mirror.BouncyCastle.Crypto.Parameters;

public class ParametersWithID : ICipherParameters
{
	private readonly ICipherParameters m_parameters;

	private readonly byte[] m_id;

	public ICipherParameters Parameters => m_parameters;

	public ParametersWithID(ICipherParameters parameters, byte[] id)
		: this(parameters, id, 0, id.Length)
	{
		if (id == null)
		{
			throw new ArgumentNullException("id");
		}
		m_parameters = parameters;
		m_id = (byte[])id.Clone();
	}

	public ParametersWithID(ICipherParameters parameters, byte[] id, int idOff, int idLen)
	{
		if (id == null)
		{
			throw new ArgumentNullException("id");
		}
		m_parameters = parameters;
		m_id = new byte[idLen];
		Array.Copy(id, idOff, m_id, 0, idLen);
	}

	public byte[] GetID()
	{
		return (byte[])m_id.Clone();
	}
}
