using System.IO;
using Mirror.BouncyCastle.Asn1;
using Mirror.BouncyCastle.Asn1.Cms;
using Mirror.BouncyCastle.Asn1.X509;

namespace Mirror.BouncyCastle.Cms;

public class CmsEnvelopedDataParser : CmsContentInfoParser
{
	internal RecipientInformationStore recipientInfoStore;

	internal EnvelopedDataParser envelopedData;

	private AlgorithmIdentifier _encAlg;

	private Mirror.BouncyCastle.Asn1.Cms.AttributeTable _unprotectedAttributes;

	private bool _attrNotRead;

	public AlgorithmIdentifier EncryptionAlgorithmID => _encAlg;

	public string EncryptionAlgOid => _encAlg.Algorithm.Id;

	public Asn1Object EncryptionAlgParams => _encAlg.Parameters?.ToAsn1Object();

	public CmsEnvelopedDataParser(byte[] envelopedData)
		: this(new MemoryStream(envelopedData, writable: false))
	{
	}

	public CmsEnvelopedDataParser(Stream envelopedData)
		: base(envelopedData)
	{
		_attrNotRead = true;
		this.envelopedData = new EnvelopedDataParser((Asn1SequenceParser)contentInfo.GetContent(16));
		Asn1Set instance = Asn1Set.GetInstance(this.envelopedData.GetRecipientInfos().ToAsn1Object());
		EncryptedContentInfoParser encryptedContentInfo = this.envelopedData.GetEncryptedContentInfo();
		_encAlg = encryptedContentInfo.ContentEncryptionAlgorithm;
		CmsReadable readable = new CmsProcessableInputStream(((Asn1OctetStringParser)encryptedContentInfo.GetEncryptedContent(4)).GetOctetStream());
		CmsSecureReadable secureReadable = new CmsEnvelopedHelper.CmsEnvelopedSecureReadable(_encAlg, readable);
		recipientInfoStore = CmsEnvelopedHelper.BuildRecipientInformationStore(instance, secureReadable);
	}

	public RecipientInformationStore GetRecipientInfos()
	{
		return recipientInfoStore;
	}

	public Mirror.BouncyCastle.Asn1.Cms.AttributeTable GetUnprotectedAttributes()
	{
		if (_unprotectedAttributes == null && _attrNotRead)
		{
			Asn1SetParser unprotectedAttrs = envelopedData.GetUnprotectedAttrs();
			_attrNotRead = false;
			if (unprotectedAttrs != null)
			{
				_unprotectedAttributes = CmsUtilities.ParseAttributeTable(unprotectedAttrs);
			}
		}
		return _unprotectedAttributes;
	}
}
