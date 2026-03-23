namespace Mirror.BouncyCastle.Crypto.Parameters;

public class ParametersWithSBox : ICipherParameters
{
	private readonly ICipherParameters m_parameters;

	private readonly byte[] m_sBox;

	public ICipherParameters Parameters => m_parameters;

	public ParametersWithSBox(ICipherParameters parameters, byte[] sBox)
	{
		m_parameters = parameters;
		m_sBox = sBox;
	}

	public byte[] GetSBox()
	{
		return m_sBox;
	}
}
