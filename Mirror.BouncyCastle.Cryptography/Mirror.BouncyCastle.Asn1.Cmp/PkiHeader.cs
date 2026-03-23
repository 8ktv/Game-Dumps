using System;
using Mirror.BouncyCastle.Asn1.X509;

namespace Mirror.BouncyCastle.Asn1.Cmp;

public class PkiHeader : Asn1Encodable
{
	public static readonly GeneralName NULL_NAME = new GeneralName(X509Name.GetInstance(new DerSequence()));

	public static readonly int CMP_1999 = 1;

	public static readonly int CMP_2000 = 2;

	private readonly DerInteger pvno;

	private readonly GeneralName sender;

	private readonly GeneralName recipient;

	private readonly Asn1GeneralizedTime messageTime;

	private readonly AlgorithmIdentifier protectionAlg;

	private readonly Asn1OctetString senderKID;

	private readonly Asn1OctetString recipKID;

	private readonly Asn1OctetString transactionID;

	private readonly Asn1OctetString senderNonce;

	private readonly Asn1OctetString recipNonce;

	private readonly PkiFreeText freeText;

	private readonly Asn1Sequence generalInfo;

	public virtual DerInteger Pvno => pvno;

	public virtual GeneralName Sender => sender;

	public virtual GeneralName Recipient => recipient;

	public virtual Asn1GeneralizedTime MessageTime => messageTime;

	public virtual AlgorithmIdentifier ProtectionAlg => protectionAlg;

	public virtual Asn1OctetString SenderKID => senderKID;

	public virtual Asn1OctetString RecipKID => recipKID;

	public virtual Asn1OctetString TransactionID => transactionID;

	public virtual Asn1OctetString SenderNonce => senderNonce;

	public virtual Asn1OctetString RecipNonce => recipNonce;

	public virtual PkiFreeText FreeText => freeText;

	public static PkiHeader GetInstance(object obj)
	{
		if (obj == null)
		{
			return null;
		}
		if (obj is PkiHeader result)
		{
			return result;
		}
		return new PkiHeader(Asn1Sequence.GetInstance(obj));
	}

	public static PkiHeader GetInstance(Asn1TaggedObject taggedObject, bool declaredExplicit)
	{
		return new PkiHeader(Asn1Sequence.GetInstance(taggedObject, declaredExplicit));
	}

	private PkiHeader(Asn1Sequence seq)
	{
		pvno = DerInteger.GetInstance(seq[0]);
		sender = GeneralName.GetInstance(seq[1]);
		recipient = GeneralName.GetInstance(seq[2]);
		for (int i = 3; i < seq.Count; i++)
		{
			Asn1TaggedObject instance = Asn1TaggedObject.GetInstance(seq[i]);
			if (!instance.HasContextTag())
			{
				throw new ArgumentException("unknown tag: " + Asn1Utilities.GetTagText(instance));
			}
			switch (instance.TagNo)
			{
			case 0:
				messageTime = Asn1GeneralizedTime.GetInstance(instance, declaredExplicit: true);
				break;
			case 1:
				protectionAlg = AlgorithmIdentifier.GetInstance(instance, explicitly: true);
				break;
			case 2:
				senderKID = Asn1OctetString.GetInstance(instance, declaredExplicit: true);
				break;
			case 3:
				recipKID = Asn1OctetString.GetInstance(instance, declaredExplicit: true);
				break;
			case 4:
				transactionID = Asn1OctetString.GetInstance(instance, declaredExplicit: true);
				break;
			case 5:
				senderNonce = Asn1OctetString.GetInstance(instance, declaredExplicit: true);
				break;
			case 6:
				recipNonce = Asn1OctetString.GetInstance(instance, declaredExplicit: true);
				break;
			case 7:
				freeText = PkiFreeText.GetInstance(instance, declaredExplicit: true);
				break;
			case 8:
				generalInfo = Asn1Sequence.GetInstance(instance, declaredExplicit: true);
				break;
			default:
				throw new ArgumentException("unknown tag number: " + instance.TagNo);
			}
		}
	}

	public PkiHeader(int pvno, GeneralName sender, GeneralName recipient)
		: this(new DerInteger(pvno), sender, recipient)
	{
	}

	private PkiHeader(DerInteger pvno, GeneralName sender, GeneralName recipient)
	{
		this.pvno = pvno;
		this.sender = sender;
		this.recipient = recipient;
	}

	public virtual InfoTypeAndValue[] GetGeneralInfo()
	{
		return generalInfo?.MapElements(InfoTypeAndValue.GetInstance);
	}

	public override Asn1Object ToAsn1Object()
	{
		Asn1EncodableVector asn1EncodableVector = new Asn1EncodableVector(pvno, sender, recipient);
		asn1EncodableVector.AddOptionalTagged(isExplicit: true, 0, messageTime);
		asn1EncodableVector.AddOptionalTagged(isExplicit: true, 1, protectionAlg);
		asn1EncodableVector.AddOptionalTagged(isExplicit: true, 2, senderKID);
		asn1EncodableVector.AddOptionalTagged(isExplicit: true, 3, recipKID);
		asn1EncodableVector.AddOptionalTagged(isExplicit: true, 4, transactionID);
		asn1EncodableVector.AddOptionalTagged(isExplicit: true, 5, senderNonce);
		asn1EncodableVector.AddOptionalTagged(isExplicit: true, 6, recipNonce);
		asn1EncodableVector.AddOptionalTagged(isExplicit: true, 7, freeText);
		asn1EncodableVector.AddOptionalTagged(isExplicit: true, 8, generalInfo);
		return new DerSequence(asn1EncodableVector);
	}
}
