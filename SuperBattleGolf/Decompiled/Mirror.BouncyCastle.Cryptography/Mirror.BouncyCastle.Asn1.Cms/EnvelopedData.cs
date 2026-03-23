namespace Mirror.BouncyCastle.Asn1.Cms;

public class EnvelopedData : Asn1Encodable
{
	private DerInteger version;

	private OriginatorInfo originatorInfo;

	private Asn1Set recipientInfos;

	private EncryptedContentInfo encryptedContentInfo;

	private Asn1Set unprotectedAttrs;

	public DerInteger Version => version;

	public OriginatorInfo OriginatorInfo => originatorInfo;

	public Asn1Set RecipientInfos => recipientInfos;

	public EncryptedContentInfo EncryptedContentInfo => encryptedContentInfo;

	public Asn1Set UnprotectedAttrs => unprotectedAttrs;

	public static EnvelopedData GetInstance(object obj)
	{
		if (obj == null)
		{
			return null;
		}
		if (obj is EnvelopedData result)
		{
			return result;
		}
		return new EnvelopedData(Asn1Sequence.GetInstance(obj));
	}

	public static EnvelopedData GetInstance(Asn1TaggedObject obj, bool explicitly)
	{
		return new EnvelopedData(Asn1Sequence.GetInstance(obj, explicitly));
	}

	public EnvelopedData(OriginatorInfo originatorInfo, Asn1Set recipientInfos, EncryptedContentInfo encryptedContentInfo, Asn1Set unprotectedAttrs)
	{
		version = new DerInteger(CalculateVersion(originatorInfo, recipientInfos, unprotectedAttrs));
		this.originatorInfo = originatorInfo;
		this.recipientInfos = recipientInfos;
		this.encryptedContentInfo = encryptedContentInfo;
		this.unprotectedAttrs = unprotectedAttrs;
	}

	public EnvelopedData(OriginatorInfo originatorInfo, Asn1Set recipientInfos, EncryptedContentInfo encryptedContentInfo, Attributes unprotectedAttrs)
	{
		version = new DerInteger(CalculateVersion(originatorInfo, recipientInfos, Asn1Set.GetInstance(unprotectedAttrs)));
		this.originatorInfo = originatorInfo;
		this.recipientInfos = recipientInfos;
		this.encryptedContentInfo = encryptedContentInfo;
		this.unprotectedAttrs = Asn1Set.GetInstance(unprotectedAttrs);
	}

	private EnvelopedData(Asn1Sequence seq)
	{
		int num = 0;
		version = (DerInteger)seq[num++];
		object obj = seq[num++];
		if (obj is Asn1TaggedObject obj2)
		{
			originatorInfo = OriginatorInfo.GetInstance(obj2, explicitly: false);
			obj = seq[num++];
		}
		recipientInfos = Asn1Set.GetInstance(obj);
		encryptedContentInfo = EncryptedContentInfo.GetInstance(seq[num++]);
		if (seq.Count > num)
		{
			unprotectedAttrs = Asn1Set.GetInstance((Asn1TaggedObject)seq[num], declaredExplicit: false);
		}
	}

	public override Asn1Object ToAsn1Object()
	{
		Asn1EncodableVector asn1EncodableVector = new Asn1EncodableVector(version);
		asn1EncodableVector.AddOptionalTagged(isExplicit: false, 0, originatorInfo);
		asn1EncodableVector.Add(recipientInfos, encryptedContentInfo);
		asn1EncodableVector.AddOptionalTagged(isExplicit: false, 1, unprotectedAttrs);
		return new BerSequence(asn1EncodableVector);
	}

	public static int CalculateVersion(OriginatorInfo originatorInfo, Asn1Set recipientInfos, Asn1Set unprotectedAttrs)
	{
		if (originatorInfo != null || unprotectedAttrs != null)
		{
			return 2;
		}
		foreach (Asn1Encodable recipientInfo in recipientInfos)
		{
			if (!RecipientInfo.GetInstance(recipientInfo).Version.HasValue(0))
			{
				return 2;
			}
		}
		return 0;
	}
}
