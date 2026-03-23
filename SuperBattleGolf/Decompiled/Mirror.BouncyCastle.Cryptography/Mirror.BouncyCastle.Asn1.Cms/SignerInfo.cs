using System.Collections.Generic;
using Mirror.BouncyCastle.Asn1.X509;

namespace Mirror.BouncyCastle.Asn1.Cms;

public class SignerInfo : Asn1Encodable
{
	private DerInteger version;

	private SignerIdentifier sid;

	private AlgorithmIdentifier digAlgorithm;

	private Asn1Set authenticatedAttributes;

	private AlgorithmIdentifier digEncryptionAlgorithm;

	private Asn1OctetString encryptedDigest;

	private Asn1Set unauthenticatedAttributes;

	public DerInteger Version => version;

	public SignerIdentifier SignerID => sid;

	public Asn1Set AuthenticatedAttributes => authenticatedAttributes;

	public AlgorithmIdentifier DigestAlgorithm => digAlgorithm;

	public Asn1OctetString EncryptedDigest => encryptedDigest;

	public AlgorithmIdentifier DigestEncryptionAlgorithm => digEncryptionAlgorithm;

	public Asn1Set UnauthenticatedAttributes => unauthenticatedAttributes;

	public static SignerInfo GetInstance(object obj)
	{
		if (obj == null)
		{
			return null;
		}
		if (obj is SignerInfo result)
		{
			return result;
		}
		return new SignerInfo(Asn1Sequence.GetInstance(obj));
	}

	public static SignerInfo GetInstance(Asn1TaggedObject taggedObject, bool declaredExplicit)
	{
		return new SignerInfo(Asn1Sequence.GetInstance(taggedObject, declaredExplicit));
	}

	public SignerInfo(SignerIdentifier sid, AlgorithmIdentifier digAlgorithm, Asn1Set authenticatedAttributes, AlgorithmIdentifier digEncryptionAlgorithm, Asn1OctetString encryptedDigest, Asn1Set unauthenticatedAttributes)
	{
		version = new DerInteger((!sid.IsTagged) ? 1 : 3);
		this.sid = sid;
		this.digAlgorithm = digAlgorithm;
		this.authenticatedAttributes = authenticatedAttributes;
		this.digEncryptionAlgorithm = digEncryptionAlgorithm;
		this.encryptedDigest = encryptedDigest;
		this.unauthenticatedAttributes = unauthenticatedAttributes;
	}

	public SignerInfo(SignerIdentifier sid, AlgorithmIdentifier digAlgorithm, Attributes authenticatedAttributes, AlgorithmIdentifier digEncryptionAlgorithm, Asn1OctetString encryptedDigest, Attributes unauthenticatedAttributes)
	{
		version = new DerInteger((!sid.IsTagged) ? 1 : 3);
		this.sid = sid;
		this.digAlgorithm = digAlgorithm;
		this.authenticatedAttributes = Asn1Set.GetInstance(authenticatedAttributes);
		this.digEncryptionAlgorithm = digEncryptionAlgorithm;
		this.encryptedDigest = encryptedDigest;
		this.unauthenticatedAttributes = Asn1Set.GetInstance(unauthenticatedAttributes);
	}

	private SignerInfo(Asn1Sequence seq)
	{
		IEnumerator<Asn1Encodable> enumerator = seq.GetEnumerator();
		enumerator.MoveNext();
		version = (DerInteger)enumerator.Current;
		enumerator.MoveNext();
		sid = SignerIdentifier.GetInstance(enumerator.Current.ToAsn1Object());
		enumerator.MoveNext();
		digAlgorithm = AlgorithmIdentifier.GetInstance(enumerator.Current.ToAsn1Object());
		enumerator.MoveNext();
		Asn1Object asn1Object = enumerator.Current.ToAsn1Object();
		if (asn1Object is Asn1TaggedObject taggedObject)
		{
			authenticatedAttributes = Asn1Set.GetInstance(taggedObject, declaredExplicit: false);
			enumerator.MoveNext();
			digEncryptionAlgorithm = AlgorithmIdentifier.GetInstance(enumerator.Current.ToAsn1Object());
		}
		else
		{
			authenticatedAttributes = null;
			digEncryptionAlgorithm = AlgorithmIdentifier.GetInstance(asn1Object);
		}
		enumerator.MoveNext();
		encryptedDigest = Asn1OctetString.GetInstance(enumerator.Current.ToAsn1Object());
		if (enumerator.MoveNext())
		{
			unauthenticatedAttributes = Asn1Set.GetInstance((Asn1TaggedObject)enumerator.Current.ToAsn1Object(), declaredExplicit: false);
		}
		else
		{
			unauthenticatedAttributes = null;
		}
	}

	public override Asn1Object ToAsn1Object()
	{
		Asn1EncodableVector asn1EncodableVector = new Asn1EncodableVector(version, sid, digAlgorithm);
		asn1EncodableVector.AddOptionalTagged(isExplicit: false, 0, authenticatedAttributes);
		asn1EncodableVector.Add(digEncryptionAlgorithm, encryptedDigest);
		asn1EncodableVector.AddOptionalTagged(isExplicit: false, 1, unauthenticatedAttributes);
		return new DerSequence(asn1EncodableVector);
	}
}
