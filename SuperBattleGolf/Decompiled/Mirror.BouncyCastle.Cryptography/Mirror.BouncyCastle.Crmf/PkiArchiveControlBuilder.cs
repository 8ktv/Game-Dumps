using System;
using System.IO;
using Mirror.BouncyCastle.Asn1.Cms;
using Mirror.BouncyCastle.Asn1.Crmf;
using Mirror.BouncyCastle.Asn1.Pkcs;
using Mirror.BouncyCastle.Asn1.X509;
using Mirror.BouncyCastle.Cms;
using Mirror.BouncyCastle.Crypto;

namespace Mirror.BouncyCastle.Crmf;

public class PkiArchiveControlBuilder
{
	private readonly CmsEnvelopedDataGenerator m_envGen;

	private readonly CmsProcessableByteArray m_keyContent;

	public PkiArchiveControlBuilder(PrivateKeyInfo privateKeyInfo, GeneralName generalName)
	{
		EncKeyWithID encKeyWithID = new EncKeyWithID(privateKeyInfo, generalName);
		try
		{
			m_keyContent = new CmsProcessableByteArray(CrmfObjectIdentifiers.id_ct_encKeyWithID, encKeyWithID.GetEncoded());
		}
		catch (IOException innerException)
		{
			throw new InvalidOperationException("unable to encode key and general name info", innerException);
		}
		m_envGen = new CmsEnvelopedDataGenerator();
	}

	public PkiArchiveControlBuilder AddRecipientGenerator(RecipientInfoGenerator recipientGen)
	{
		m_envGen.AddRecipientInfoGenerator(recipientGen);
		return this;
	}

	public PkiArchiveControl Build(ICipherBuilderWithKey contentEncryptor)
	{
		return new PkiArchiveControl(new PkiArchiveOptions(new EncryptedKey(EnvelopedData.GetInstance(m_envGen.Generate(m_keyContent, contentEncryptor).ContentInfo.Content))));
	}
}
