using System;
using Mirror.BouncyCastle.Asn1;
using Mirror.BouncyCastle.Asn1.Cms;
using Mirror.BouncyCastle.Asn1.Crmf;
using Mirror.BouncyCastle.Cms;

namespace Mirror.BouncyCastle.Crmf;

public class PkiArchiveControl : IControl
{
	public static readonly int encryptedPrivKey = 0;

	public static readonly int keyGenParameters = 1;

	public static readonly int archiveRemGenPrivKey = 2;

	private readonly PkiArchiveOptions m_pkiArchiveOptions;

	public DerObjectIdentifier Type => CrmfObjectIdentifiers.id_regCtrl_pkiArchiveOptions;

	public Asn1Encodable Value => m_pkiArchiveOptions;

	public int ArchiveType => m_pkiArchiveOptions.Type;

	[Obsolete("Use 'IsEnvelopedData' instead")]
	public bool EnvelopedData => IsEnvelopedData();

	public PkiArchiveControl(PkiArchiveOptions pkiArchiveOptions)
	{
		m_pkiArchiveOptions = pkiArchiveOptions;
	}

	public bool IsEnvelopedData()
	{
		return !EncryptedKey.GetInstance(m_pkiArchiveOptions.Value).IsEncryptedValue;
	}

	public CmsEnvelopedData GetEnvelopedData()
	{
		try
		{
			EnvelopedData instance = Mirror.BouncyCastle.Asn1.Cms.EnvelopedData.GetInstance(EncryptedKey.GetInstance(m_pkiArchiveOptions.Value).Value);
			return new CmsEnvelopedData(new ContentInfo(CmsObjectIdentifiers.EnvelopedData, instance));
		}
		catch (CmsException ex)
		{
			throw new CrmfException("CMS parsing error: " + ex.Message, ex);
		}
		catch (Exception ex2)
		{
			throw new CrmfException("CRMF parsing error: " + ex2.Message, ex2);
		}
	}
}
