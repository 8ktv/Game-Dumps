using System;

namespace Mirror.BouncyCastle.Asn1.Cms;

public class EncryptedData : Asn1Encodable
{
	private readonly DerInteger version;

	private readonly EncryptedContentInfo encryptedContentInfo;

	private readonly Asn1Set unprotectedAttrs;

	public virtual DerInteger Version => version;

	public virtual EncryptedContentInfo EncryptedContentInfo => encryptedContentInfo;

	public virtual Asn1Set UnprotectedAttrs => unprotectedAttrs;

	public static EncryptedData GetInstance(object obj)
	{
		if (obj == null)
		{
			return null;
		}
		if (obj is EncryptedData result)
		{
			return result;
		}
		return new EncryptedData(Asn1Sequence.GetInstance(obj));
	}

	public static EncryptedData GetInstance(Asn1TaggedObject taggedObject, bool declaredExplicit)
	{
		return new EncryptedData(Asn1Sequence.GetInstance(taggedObject, declaredExplicit));
	}

	public EncryptedData(EncryptedContentInfo encInfo)
		: this(encInfo, null)
	{
	}

	public EncryptedData(EncryptedContentInfo encInfo, Asn1Set unprotectedAttrs)
	{
		if (encInfo == null)
		{
			throw new ArgumentNullException("encInfo");
		}
		version = new DerInteger((unprotectedAttrs != null) ? 2 : 0);
		encryptedContentInfo = encInfo;
		this.unprotectedAttrs = unprotectedAttrs;
	}

	private EncryptedData(Asn1Sequence seq)
	{
		int count = seq.Count;
		if (count < 2 || count > 3)
		{
			throw new ArgumentException("Bad sequence size: " + count, "seq");
		}
		int sequencePosition = 0;
		version = DerInteger.GetInstance(seq[sequencePosition++]);
		encryptedContentInfo = EncryptedContentInfo.GetInstance(seq[sequencePosition++]);
		unprotectedAttrs = Asn1Utilities.ReadOptionalContextTagged(seq, ref sequencePosition, 0, state: false, Asn1Set.GetInstance);
		if (sequencePosition != count)
		{
			throw new ArgumentException("Unexpected elements in sequence", "seq");
		}
	}

	public override Asn1Object ToAsn1Object()
	{
		Asn1EncodableVector asn1EncodableVector = new Asn1EncodableVector(version, encryptedContentInfo);
		if (unprotectedAttrs != null)
		{
			asn1EncodableVector.Add(new BerTaggedObject(isExplicit: false, 1, unprotectedAttrs));
		}
		return new BerSequence(asn1EncodableVector);
	}
}
