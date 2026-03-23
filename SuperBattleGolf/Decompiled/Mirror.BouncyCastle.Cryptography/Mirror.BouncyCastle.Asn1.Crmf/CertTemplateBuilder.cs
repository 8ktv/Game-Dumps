using Mirror.BouncyCastle.Asn1.X509;

namespace Mirror.BouncyCastle.Asn1.Crmf;

public class CertTemplateBuilder
{
	private DerInteger version;

	private DerInteger serialNumber;

	private AlgorithmIdentifier signingAlg;

	private X509Name issuer;

	private OptionalValidity validity;

	private X509Name subject;

	private SubjectPublicKeyInfo publicKey;

	private DerBitString issuerUID;

	private DerBitString subjectUID;

	private X509Extensions extensions;

	public virtual CertTemplateBuilder SetVersion(int ver)
	{
		version = new DerInteger(ver);
		return this;
	}

	public virtual CertTemplateBuilder SetSerialNumber(DerInteger ser)
	{
		serialNumber = ser;
		return this;
	}

	public virtual CertTemplateBuilder SetSigningAlg(AlgorithmIdentifier aid)
	{
		signingAlg = aid;
		return this;
	}

	public virtual CertTemplateBuilder SetIssuer(X509Name name)
	{
		issuer = name;
		return this;
	}

	public virtual CertTemplateBuilder SetValidity(OptionalValidity v)
	{
		validity = v;
		return this;
	}

	public virtual CertTemplateBuilder SetSubject(X509Name name)
	{
		subject = name;
		return this;
	}

	public virtual CertTemplateBuilder SetPublicKey(SubjectPublicKeyInfo spki)
	{
		publicKey = spki;
		return this;
	}

	public virtual CertTemplateBuilder SetIssuerUID(DerBitString uid)
	{
		issuerUID = uid;
		return this;
	}

	public virtual CertTemplateBuilder SetSubjectUID(DerBitString uid)
	{
		subjectUID = uid;
		return this;
	}

	public virtual CertTemplateBuilder SetExtensions(X509Extensions extens)
	{
		extensions = extens;
		return this;
	}

	public virtual CertTemplate Build()
	{
		Asn1EncodableVector asn1EncodableVector = new Asn1EncodableVector(10);
		asn1EncodableVector.AddOptionalTagged(isExplicit: false, 0, version);
		asn1EncodableVector.AddOptionalTagged(isExplicit: false, 1, serialNumber);
		asn1EncodableVector.AddOptionalTagged(isExplicit: false, 2, signingAlg);
		asn1EncodableVector.AddOptionalTagged(isExplicit: true, 3, issuer);
		asn1EncodableVector.AddOptionalTagged(isExplicit: false, 4, validity);
		asn1EncodableVector.AddOptionalTagged(isExplicit: true, 5, subject);
		asn1EncodableVector.AddOptionalTagged(isExplicit: false, 6, publicKey);
		asn1EncodableVector.AddOptionalTagged(isExplicit: false, 7, issuerUID);
		asn1EncodableVector.AddOptionalTagged(isExplicit: false, 8, subjectUID);
		asn1EncodableVector.AddOptionalTagged(isExplicit: false, 9, extensions);
		return CertTemplate.GetInstance(new DerSequence(asn1EncodableVector));
	}
}
