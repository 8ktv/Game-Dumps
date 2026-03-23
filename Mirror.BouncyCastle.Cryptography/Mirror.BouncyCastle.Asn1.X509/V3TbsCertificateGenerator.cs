using System;

namespace Mirror.BouncyCastle.Asn1.X509;

public class V3TbsCertificateGenerator
{
	internal DerTaggedObject version = new DerTaggedObject(0, new DerInteger(2));

	internal DerInteger serialNumber;

	internal AlgorithmIdentifier signature;

	internal X509Name issuer;

	internal Time startDate;

	internal Time endDate;

	internal X509Name subject;

	internal SubjectPublicKeyInfo subjectPublicKeyInfo;

	internal X509Extensions extensions;

	private bool altNamePresentAndCritical;

	private DerBitString issuerUniqueID;

	private DerBitString subjectUniqueID;

	public void SetSerialNumber(DerInteger serialNumber)
	{
		this.serialNumber = serialNumber;
	}

	public void SetSignature(AlgorithmIdentifier signature)
	{
		this.signature = signature;
	}

	public void SetIssuer(X509Name issuer)
	{
		this.issuer = issuer;
	}

	public void SetStartDate(Asn1UtcTime startDate)
	{
		this.startDate = new Time(startDate);
	}

	public void SetStartDate(Time startDate)
	{
		this.startDate = startDate;
	}

	public void SetEndDate(Asn1UtcTime endDate)
	{
		this.endDate = new Time(endDate);
	}

	public void SetEndDate(Time endDate)
	{
		this.endDate = endDate;
	}

	public void SetSubject(X509Name subject)
	{
		this.subject = subject;
	}

	public void SetIssuerUniqueID(DerBitString uniqueID)
	{
		issuerUniqueID = uniqueID;
	}

	public void SetSubjectUniqueID(DerBitString uniqueID)
	{
		subjectUniqueID = uniqueID;
	}

	public void SetSubjectPublicKeyInfo(SubjectPublicKeyInfo pubKeyInfo)
	{
		subjectPublicKeyInfo = pubKeyInfo;
	}

	public void SetExtensions(X509Extensions extensions)
	{
		this.extensions = extensions;
		if (extensions != null)
		{
			X509Extension extension = extensions.GetExtension(X509Extensions.SubjectAlternativeName);
			if (extension != null && extension.IsCritical)
			{
				altNamePresentAndCritical = true;
			}
		}
	}

	public Asn1Sequence GeneratePreTbsCertificate()
	{
		if (signature != null)
		{
			throw new InvalidOperationException("signature field should not be set in PreTBSCertificate");
		}
		if (serialNumber == null || issuer == null || startDate == null || endDate == null || (subject == null && !altNamePresentAndCritical) || subjectPublicKeyInfo == null)
		{
			throw new InvalidOperationException("not all mandatory fields set in V3 TBScertificate generator");
		}
		return GenerateTbsStructure();
	}

	public TbsCertificateStructure GenerateTbsCertificate()
	{
		if (serialNumber == null || signature == null || issuer == null || startDate == null || endDate == null || (subject == null && !altNamePresentAndCritical) || subjectPublicKeyInfo == null)
		{
			throw new InvalidOperationException("not all mandatory fields set in V3 TBScertificate generator");
		}
		return TbsCertificateStructure.GetInstance(GenerateTbsStructure());
	}

	private Asn1Sequence GenerateTbsStructure()
	{
		Asn1EncodableVector asn1EncodableVector = new Asn1EncodableVector(10);
		asn1EncodableVector.Add(version);
		asn1EncodableVector.Add(serialNumber);
		asn1EncodableVector.AddOptional(signature);
		asn1EncodableVector.Add(issuer);
		asn1EncodableVector.Add(new DerSequence(startDate, endDate));
		if (subject != null)
		{
			asn1EncodableVector.Add(subject);
		}
		else
		{
			asn1EncodableVector.Add(DerSequence.Empty);
		}
		asn1EncodableVector.Add(subjectPublicKeyInfo);
		asn1EncodableVector.AddOptionalTagged(isExplicit: false, 1, issuerUniqueID);
		asn1EncodableVector.AddOptionalTagged(isExplicit: false, 2, subjectUniqueID);
		asn1EncodableVector.AddOptionalTagged(isExplicit: true, 3, extensions);
		return new DerSequence(asn1EncodableVector);
	}
}
