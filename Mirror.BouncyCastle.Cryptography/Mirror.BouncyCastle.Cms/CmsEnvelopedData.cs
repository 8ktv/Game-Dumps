using System.IO;
using Mirror.BouncyCastle.Asn1;
using Mirror.BouncyCastle.Asn1.Cms;
using Mirror.BouncyCastle.Asn1.X509;

namespace Mirror.BouncyCastle.Cms;

public class CmsEnvelopedData
{
	internal RecipientInformationStore recipientInfoStore;

	internal ContentInfo contentInfo;

	private AlgorithmIdentifier encAlg;

	private Asn1Set unprotectedAttributes;

	public AlgorithmIdentifier EncryptionAlgorithmID => encAlg;

	public string EncryptionAlgOid => encAlg.Algorithm.Id;

	public ContentInfo ContentInfo => contentInfo;

	public CmsEnvelopedData(byte[] envelopedData)
		: this(CmsUtilities.ReadContentInfo(envelopedData))
	{
	}

	public CmsEnvelopedData(Stream envelopedData)
		: this(CmsUtilities.ReadContentInfo(envelopedData))
	{
	}

	public CmsEnvelopedData(ContentInfo contentInfo)
	{
		this.contentInfo = contentInfo;
		EnvelopedData instance = EnvelopedData.GetInstance(contentInfo.Content);
		Asn1Set recipientInfos = instance.RecipientInfos;
		EncryptedContentInfo encryptedContentInfo = instance.EncryptedContentInfo;
		encAlg = encryptedContentInfo.ContentEncryptionAlgorithm;
		CmsReadable readable = new CmsProcessableByteArray(encryptedContentInfo.EncryptedContent.GetOctets());
		CmsSecureReadable secureReadable = new CmsEnvelopedHelper.CmsEnvelopedSecureReadable(encAlg, readable);
		recipientInfoStore = CmsEnvelopedHelper.BuildRecipientInformationStore(recipientInfos, secureReadable);
		unprotectedAttributes = instance.UnprotectedAttrs;
	}

	public RecipientInformationStore GetRecipientInfos()
	{
		return recipientInfoStore;
	}

	public Mirror.BouncyCastle.Asn1.Cms.AttributeTable GetUnprotectedAttributes()
	{
		if (unprotectedAttributes == null)
		{
			return null;
		}
		return new Mirror.BouncyCastle.Asn1.Cms.AttributeTable(unprotectedAttributes);
	}

	public byte[] GetEncoded()
	{
		return contentInfo.GetEncoded();
	}
}
