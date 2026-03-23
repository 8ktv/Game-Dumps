using Mirror.BouncyCastle.Asn1.Crmf;
using Mirror.BouncyCastle.Asn1.X509;

namespace Mirror.BouncyCastle.Asn1.Cmp;

public class RevRepContent : Asn1Encodable
{
	private readonly Asn1Sequence m_status;

	private readonly Asn1Sequence m_revCerts;

	private readonly Asn1Sequence m_crls;

	public static RevRepContent GetInstance(object obj)
	{
		if (obj == null)
		{
			return null;
		}
		if (obj is RevRepContent result)
		{
			return result;
		}
		return new RevRepContent(Asn1Sequence.GetInstance(obj));
	}

	public static RevRepContent GetInstance(Asn1TaggedObject taggedObject, bool declaredExplicit)
	{
		return new RevRepContent(Asn1Sequence.GetInstance(taggedObject, declaredExplicit));
	}

	private RevRepContent(Asn1Sequence seq)
	{
		m_status = Asn1Sequence.GetInstance(seq[0]);
		for (int i = 1; i < seq.Count; i++)
		{
			Asn1TaggedObject instance = Asn1TaggedObject.GetInstance(seq[i]);
			if (instance.HasContextTag(0))
			{
				m_revCerts = Asn1Sequence.GetInstance(instance, declaredExplicit: true);
			}
			else if (instance.HasContextTag(1))
			{
				m_crls = Asn1Sequence.GetInstance(instance, declaredExplicit: true);
			}
		}
	}

	public virtual PkiStatusInfo[] GetStatus()
	{
		return m_status.MapElements(PkiStatusInfo.GetInstance);
	}

	public virtual CertId[] GetRevCerts()
	{
		return m_revCerts?.MapElements(CertId.GetInstance);
	}

	public virtual CertificateList[] GetCrls()
	{
		return m_crls?.MapElements(CertificateList.GetInstance);
	}

	public override Asn1Object ToAsn1Object()
	{
		Asn1EncodableVector asn1EncodableVector = new Asn1EncodableVector(3);
		asn1EncodableVector.Add(m_status);
		asn1EncodableVector.AddOptionalTagged(isExplicit: true, 0, m_revCerts);
		asn1EncodableVector.AddOptionalTagged(isExplicit: true, 1, m_crls);
		return new DerSequence(asn1EncodableVector);
	}
}
