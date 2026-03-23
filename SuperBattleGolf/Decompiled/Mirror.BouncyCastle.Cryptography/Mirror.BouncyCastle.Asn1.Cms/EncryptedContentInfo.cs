using System;
using Mirror.BouncyCastle.Asn1.X509;

namespace Mirror.BouncyCastle.Asn1.Cms;

public class EncryptedContentInfo : Asn1Encodable
{
	private DerObjectIdentifier contentType;

	private AlgorithmIdentifier contentEncryptionAlgorithm;

	private Asn1OctetString encryptedContent;

	public DerObjectIdentifier ContentType => contentType;

	public AlgorithmIdentifier ContentEncryptionAlgorithm => contentEncryptionAlgorithm;

	public Asn1OctetString EncryptedContent => encryptedContent;

	public static EncryptedContentInfo GetInstance(object obj)
	{
		if (obj == null)
		{
			return null;
		}
		if (obj is EncryptedContentInfo result)
		{
			return result;
		}
		return new EncryptedContentInfo(Asn1Sequence.GetInstance(obj));
	}

	public static EncryptedContentInfo GetInstance(Asn1TaggedObject taggedObject, bool declaredExplicit)
	{
		return new EncryptedContentInfo(Asn1Sequence.GetInstance(taggedObject, declaredExplicit));
	}

	public EncryptedContentInfo(DerObjectIdentifier contentType, AlgorithmIdentifier contentEncryptionAlgorithm, Asn1OctetString encryptedContent)
	{
		this.contentType = contentType;
		this.contentEncryptionAlgorithm = contentEncryptionAlgorithm;
		this.encryptedContent = encryptedContent;
	}

	[Obsolete("Use 'GetInstance' instead")]
	public EncryptedContentInfo(Asn1Sequence seq)
	{
		contentType = (DerObjectIdentifier)seq[0];
		contentEncryptionAlgorithm = AlgorithmIdentifier.GetInstance(seq[1]);
		if (seq.Count > 2)
		{
			encryptedContent = Asn1OctetString.GetInstance((Asn1TaggedObject)seq[2], declaredExplicit: false);
		}
	}

	public override Asn1Object ToAsn1Object()
	{
		Asn1EncodableVector asn1EncodableVector = new Asn1EncodableVector(contentType, contentEncryptionAlgorithm);
		if (encryptedContent != null)
		{
			asn1EncodableVector.Add(new BerTaggedObject(isExplicit: false, 0, encryptedContent));
		}
		return new BerSequence(asn1EncodableVector);
	}
}
