using Mirror.BouncyCastle.Asn1.X509;

namespace Mirror.BouncyCastle.Asn1.Cmp;

public class PkiHeaderBuilder
{
	private DerInteger pvno;

	private GeneralName sender;

	private GeneralName recipient;

	private Asn1GeneralizedTime messageTime;

	private AlgorithmIdentifier protectionAlg;

	private Asn1OctetString senderKID;

	private Asn1OctetString recipKID;

	private Asn1OctetString transactionID;

	private Asn1OctetString senderNonce;

	private Asn1OctetString recipNonce;

	private PkiFreeText freeText;

	private Asn1Sequence generalInfo;

	public PkiHeaderBuilder(int pvno, GeneralName sender, GeneralName recipient)
		: this(new DerInteger(pvno), sender, recipient)
	{
	}

	private PkiHeaderBuilder(DerInteger pvno, GeneralName sender, GeneralName recipient)
	{
		this.pvno = pvno;
		this.sender = sender;
		this.recipient = recipient;
	}

	public virtual PkiHeaderBuilder SetMessageTime(Asn1GeneralizedTime time)
	{
		messageTime = time;
		return this;
	}

	public virtual PkiHeaderBuilder SetProtectionAlg(AlgorithmIdentifier aid)
	{
		protectionAlg = aid;
		return this;
	}

	public virtual PkiHeaderBuilder SetSenderKID(byte[] kid)
	{
		return SetSenderKID((kid == null) ? null : new DerOctetString(kid));
	}

	public virtual PkiHeaderBuilder SetSenderKID(Asn1OctetString kid)
	{
		senderKID = kid;
		return this;
	}

	public virtual PkiHeaderBuilder SetRecipKID(byte[] kid)
	{
		return SetRecipKID((kid == null) ? null : new DerOctetString(kid));
	}

	public virtual PkiHeaderBuilder SetRecipKID(Asn1OctetString kid)
	{
		recipKID = kid;
		return this;
	}

	public virtual PkiHeaderBuilder SetTransactionID(byte[] tid)
	{
		return SetTransactionID((tid == null) ? null : new DerOctetString(tid));
	}

	public virtual PkiHeaderBuilder SetTransactionID(Asn1OctetString tid)
	{
		transactionID = tid;
		return this;
	}

	public virtual PkiHeaderBuilder SetSenderNonce(byte[] nonce)
	{
		return SetSenderNonce((nonce == null) ? null : new DerOctetString(nonce));
	}

	public virtual PkiHeaderBuilder SetSenderNonce(Asn1OctetString nonce)
	{
		senderNonce = nonce;
		return this;
	}

	public virtual PkiHeaderBuilder SetRecipNonce(byte[] nonce)
	{
		return SetRecipNonce((nonce == null) ? null : new DerOctetString(nonce));
	}

	public virtual PkiHeaderBuilder SetRecipNonce(Asn1OctetString nonce)
	{
		recipNonce = nonce;
		return this;
	}

	public virtual PkiHeaderBuilder SetFreeText(PkiFreeText text)
	{
		freeText = text;
		return this;
	}

	public virtual PkiHeaderBuilder SetGeneralInfo(InfoTypeAndValue genInfo)
	{
		return SetGeneralInfo(MakeGeneralInfoSeq(genInfo));
	}

	public virtual PkiHeaderBuilder SetGeneralInfo(InfoTypeAndValue[] genInfos)
	{
		return SetGeneralInfo(MakeGeneralInfoSeq(genInfos));
	}

	public virtual PkiHeaderBuilder SetGeneralInfo(Asn1Sequence seqOfInfoTypeAndValue)
	{
		generalInfo = seqOfInfoTypeAndValue;
		return this;
	}

	private static Asn1Sequence MakeGeneralInfoSeq(InfoTypeAndValue generalInfo)
	{
		return new DerSequence(generalInfo);
	}

	private static Asn1Sequence MakeGeneralInfoSeq(InfoTypeAndValue[] generalInfos)
	{
		if (generalInfos != null)
		{
			return new DerSequence(generalInfos);
		}
		return null;
	}

	public virtual PkiHeader Build()
	{
		Asn1EncodableVector asn1EncodableVector = new Asn1EncodableVector(12);
		asn1EncodableVector.Add(pvno, sender, recipient);
		asn1EncodableVector.AddOptionalTagged(isExplicit: true, 0, messageTime);
		asn1EncodableVector.AddOptionalTagged(isExplicit: true, 1, protectionAlg);
		asn1EncodableVector.AddOptionalTagged(isExplicit: true, 2, senderKID);
		asn1EncodableVector.AddOptionalTagged(isExplicit: true, 3, recipKID);
		asn1EncodableVector.AddOptionalTagged(isExplicit: true, 4, transactionID);
		asn1EncodableVector.AddOptionalTagged(isExplicit: true, 5, senderNonce);
		asn1EncodableVector.AddOptionalTagged(isExplicit: true, 6, recipNonce);
		asn1EncodableVector.AddOptionalTagged(isExplicit: true, 7, freeText);
		asn1EncodableVector.AddOptionalTagged(isExplicit: true, 8, generalInfo);
		messageTime = null;
		protectionAlg = null;
		senderKID = null;
		recipKID = null;
		transactionID = null;
		senderNonce = null;
		recipNonce = null;
		freeText = null;
		generalInfo = null;
		return PkiHeader.GetInstance(new DerSequence(asn1EncodableVector));
	}
}
