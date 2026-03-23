using Mirror.BouncyCastle.Asn1.Cms;

namespace Mirror.BouncyCastle.Asn1.Crmf;

public class EncryptedKey : Asn1Encodable, IAsn1Choice
{
	private readonly EnvelopedData m_envelopedData;

	private readonly EncryptedValue m_encryptedValue;

	public virtual bool IsEncryptedValue => m_encryptedValue != null;

	public virtual Asn1Encodable Value
	{
		get
		{
			if (m_encryptedValue != null)
			{
				return m_encryptedValue;
			}
			return m_envelopedData;
		}
	}

	public static EncryptedKey GetInstance(object obj)
	{
		if (obj is EncryptedKey result)
		{
			return result;
		}
		if (obj is Asn1TaggedObject obj2)
		{
			return new EncryptedKey(EnvelopedData.GetInstance(obj2, explicitly: false));
		}
		return new EncryptedKey(EncryptedValue.GetInstance(obj));
	}

	public EncryptedKey(EnvelopedData envelopedData)
	{
		m_envelopedData = envelopedData;
	}

	public EncryptedKey(EncryptedValue encryptedValue)
	{
		m_encryptedValue = encryptedValue;
	}

	public override Asn1Object ToAsn1Object()
	{
		if (m_encryptedValue != null)
		{
			return m_encryptedValue.ToAsn1Object();
		}
		return new DerTaggedObject(isExplicit: false, 0, m_envelopedData);
	}
}
