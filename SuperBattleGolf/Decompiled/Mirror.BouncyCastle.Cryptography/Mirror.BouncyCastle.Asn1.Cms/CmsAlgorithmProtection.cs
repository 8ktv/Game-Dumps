using System;
using Mirror.BouncyCastle.Asn1.X509;

namespace Mirror.BouncyCastle.Asn1.Cms;

public class CmsAlgorithmProtection : Asn1Encodable
{
	public static readonly int Signature = 1;

	public static readonly int Mac = 2;

	private readonly AlgorithmIdentifier digestAlgorithm;

	private readonly AlgorithmIdentifier signatureAlgorithm;

	private readonly AlgorithmIdentifier macAlgorithm;

	public AlgorithmIdentifier DigestAlgorithm => digestAlgorithm;

	public AlgorithmIdentifier MacAlgorithm => macAlgorithm;

	public AlgorithmIdentifier SignatureAlgorithm => signatureAlgorithm;

	public static CmsAlgorithmProtection GetInstance(object obj)
	{
		if (obj == null)
		{
			return null;
		}
		if (obj is CmsAlgorithmProtection result)
		{
			return result;
		}
		return new CmsAlgorithmProtection(Asn1Sequence.GetInstance(obj));
	}

	public static CmsAlgorithmProtection GetInstance(Asn1TaggedObject taggedObject, bool declaredExplicit)
	{
		return new CmsAlgorithmProtection(Asn1Sequence.GetInstance(taggedObject, declaredExplicit));
	}

	public CmsAlgorithmProtection(AlgorithmIdentifier digestAlgorithm, int type, AlgorithmIdentifier algorithmIdentifier)
	{
		if (digestAlgorithm == null || algorithmIdentifier == null)
		{
			throw new ArgumentException("AlgorithmIdentifiers cannot be null");
		}
		this.digestAlgorithm = digestAlgorithm;
		switch (type)
		{
		case 1:
			signatureAlgorithm = algorithmIdentifier;
			macAlgorithm = null;
			break;
		case 2:
			signatureAlgorithm = null;
			macAlgorithm = algorithmIdentifier;
			break;
		default:
			throw new ArgumentException("Unknown type: " + type);
		}
	}

	private CmsAlgorithmProtection(Asn1Sequence sequence)
	{
		if (sequence.Count != 2)
		{
			throw new ArgumentException("Sequence wrong size: One of signatureAlgorithm or macAlgorithm must be present");
		}
		digestAlgorithm = AlgorithmIdentifier.GetInstance(sequence[0]);
		Asn1TaggedObject instance = Asn1TaggedObject.GetInstance(sequence[1]);
		if (instance.TagNo == 1)
		{
			signatureAlgorithm = AlgorithmIdentifier.GetInstance(instance, explicitly: false);
			macAlgorithm = null;
			return;
		}
		if (instance.TagNo == 2)
		{
			signatureAlgorithm = null;
			macAlgorithm = AlgorithmIdentifier.GetInstance(instance, explicitly: false);
			return;
		}
		throw new ArgumentException("Unknown tag found: " + instance.TagNo);
	}

	public override Asn1Object ToAsn1Object()
	{
		Asn1EncodableVector asn1EncodableVector = new Asn1EncodableVector(3);
		asn1EncodableVector.Add(digestAlgorithm);
		asn1EncodableVector.AddOptionalTagged(isExplicit: false, 1, signatureAlgorithm);
		asn1EncodableVector.AddOptionalTagged(isExplicit: false, 2, macAlgorithm);
		return new DerSequence(asn1EncodableVector);
	}
}
