using Mirror.BouncyCastle.Asn1.Crmf;
using Mirror.BouncyCastle.Asn1.X509;

namespace Mirror.BouncyCastle.Crmf;

public class EncryptedValueParser
{
	private readonly EncryptedValue m_value;

	private readonly IEncryptedValuePadder m_padder;

	public virtual AlgorithmIdentifier IntendedAlg => m_value.IntendedAlg;

	public EncryptedValueParser(EncryptedValue value)
		: this(value, null)
	{
	}

	public EncryptedValueParser(EncryptedValue value, IEncryptedValuePadder padder)
	{
		m_value = value;
		m_padder = padder;
	}

	private byte[] UnpadData(byte[] data)
	{
		return m_padder?.GetUnpaddedData(data) ?? data;
	}
}
