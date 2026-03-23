using Mirror.BouncyCastle.Asn1;
using Mirror.BouncyCastle.Asn1.Cms;
using Mirror.BouncyCastle.Asn1.X509;
using Mirror.BouncyCastle.Crypto;
using Mirror.BouncyCastle.Crypto.Parameters;
using Mirror.BouncyCastle.Security;
using Mirror.BouncyCastle.X509;

namespace Mirror.BouncyCastle.Cms;

public class KeyTransRecipientInfoGenerator : RecipientInfoGenerator
{
	private readonly IKeyWrapper m_keyWrapper;

	private IssuerAndSerialNumber m_issuerAndSerialNumber;

	private Asn1OctetString m_subjectKeyIdentifier;

	protected virtual AlgorithmIdentifier AlgorithmDetails => (AlgorithmIdentifier)m_keyWrapper.AlgorithmDetails;

	public KeyTransRecipientInfoGenerator(X509Certificate recipCert, IKeyWrapper keyWrapper)
		: this(new IssuerAndSerialNumber(recipCert.CertificateStructure), keyWrapper)
	{
	}

	public KeyTransRecipientInfoGenerator(IssuerAndSerialNumber issuerAndSerial, IKeyWrapper keyWrapper)
	{
		m_issuerAndSerialNumber = issuerAndSerial;
		m_keyWrapper = keyWrapper;
	}

	public KeyTransRecipientInfoGenerator(byte[] subjectKeyID, IKeyWrapper keyWrapper)
	{
		m_subjectKeyIdentifier = new DerOctetString(subjectKeyID);
		m_keyWrapper = keyWrapper;
	}

	public RecipientInfo Generate(KeyParameter contentEncryptionKey, SecureRandom random)
	{
		AlgorithmIdentifier algorithmDetails = AlgorithmDetails;
		byte[] contents = GenerateWrappedKey(contentEncryptionKey);
		RecipientIdentifier rid = ((m_issuerAndSerialNumber == null) ? new RecipientIdentifier(m_subjectKeyIdentifier) : new RecipientIdentifier(m_issuerAndSerialNumber));
		return new RecipientInfo(new KeyTransRecipientInfo(rid, algorithmDetails, new DerOctetString(contents)));
	}

	protected virtual byte[] GenerateWrappedKey(KeyParameter contentEncryptionKey)
	{
		return m_keyWrapper.Wrap(contentEncryptionKey.GetKey()).Collect();
	}
}
