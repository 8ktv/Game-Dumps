using System;

namespace Mirror.BouncyCastle.Asn1.Cms;

public class AuthEnvelopedData : Asn1Encodable
{
	private DerInteger version;

	private OriginatorInfo originatorInfo;

	private Asn1Set recipientInfos;

	private EncryptedContentInfo authEncryptedContentInfo;

	private Asn1Set authAttrs;

	private Asn1OctetString mac;

	private Asn1Set unauthAttrs;

	public DerInteger Version => version;

	public OriginatorInfo OriginatorInfo => originatorInfo;

	public Asn1Set RecipientInfos => recipientInfos;

	public EncryptedContentInfo AuthEncryptedContentInfo => authEncryptedContentInfo;

	public Asn1Set AuthAttrs => authAttrs;

	public Asn1OctetString Mac => mac;

	public Asn1Set UnauthAttrs => unauthAttrs;

	public static AuthEnvelopedData GetInstance(object obj)
	{
		if (obj == null)
		{
			return null;
		}
		if (obj is AuthEnvelopedData result)
		{
			return result;
		}
		return new AuthEnvelopedData(Asn1Sequence.GetInstance(obj));
	}

	public static AuthEnvelopedData GetInstance(Asn1TaggedObject obj, bool isExplicit)
	{
		return new AuthEnvelopedData(Asn1Sequence.GetInstance(obj, isExplicit));
	}

	public AuthEnvelopedData(OriginatorInfo originatorInfo, Asn1Set recipientInfos, EncryptedContentInfo authEncryptedContentInfo, Asn1Set authAttrs, Asn1OctetString mac, Asn1Set unauthAttrs)
	{
		version = new DerInteger(0);
		this.originatorInfo = originatorInfo;
		this.recipientInfos = recipientInfos;
		if (this.recipientInfos.Count < 1)
		{
			throw new ArgumentException("AuthEnvelopedData requires at least 1 RecipientInfo");
		}
		this.authEncryptedContentInfo = authEncryptedContentInfo;
		this.authAttrs = authAttrs;
		if (!authEncryptedContentInfo.ContentType.Equals(CmsObjectIdentifiers.Data) && (authAttrs == null || authAttrs.Count < 1))
		{
			throw new ArgumentException("authAttrs must be present with non-data content");
		}
		this.mac = mac;
		this.unauthAttrs = unauthAttrs;
	}

	private AuthEnvelopedData(Asn1Sequence seq)
	{
		int num = 0;
		Asn1Object obj = seq[num++].ToAsn1Object();
		version = DerInteger.GetInstance(obj);
		if (!version.HasValue(0))
		{
			throw new ArgumentException("AuthEnvelopedData version number must be 0");
		}
		obj = seq[num++].ToAsn1Object();
		if (obj is Asn1TaggedObject obj2)
		{
			originatorInfo = OriginatorInfo.GetInstance(obj2, explicitly: false);
			obj = seq[num++].ToAsn1Object();
		}
		recipientInfos = Asn1Set.GetInstance(obj);
		if (recipientInfos.Count < 1)
		{
			throw new ArgumentException("AuthEnvelopedData requires at least 1 RecipientInfo");
		}
		obj = seq[num++].ToAsn1Object();
		authEncryptedContentInfo = EncryptedContentInfo.GetInstance(obj);
		obj = seq[num++].ToAsn1Object();
		if (obj is Asn1TaggedObject taggedObject)
		{
			authAttrs = Asn1Set.GetInstance(taggedObject, declaredExplicit: false);
			obj = seq[num++].ToAsn1Object();
		}
		else if (!authEncryptedContentInfo.ContentType.Equals(CmsObjectIdentifiers.Data) && (authAttrs == null || authAttrs.Count < 1))
		{
			throw new ArgumentException("authAttrs must be present with non-data content");
		}
		mac = Asn1OctetString.GetInstance(obj);
		if (seq.Count > num)
		{
			obj = seq[num++].ToAsn1Object();
			unauthAttrs = Asn1Set.GetInstance((Asn1TaggedObject)obj, declaredExplicit: false);
		}
	}

	public override Asn1Object ToAsn1Object()
	{
		Asn1EncodableVector asn1EncodableVector = new Asn1EncodableVector(version);
		asn1EncodableVector.AddOptionalTagged(isExplicit: false, 0, originatorInfo);
		asn1EncodableVector.Add(recipientInfos, authEncryptedContentInfo);
		asn1EncodableVector.AddOptionalTagged(isExplicit: false, 1, authAttrs);
		asn1EncodableVector.Add(mac);
		asn1EncodableVector.AddOptionalTagged(isExplicit: false, 2, unauthAttrs);
		return new BerSequence(asn1EncodableVector);
	}
}
