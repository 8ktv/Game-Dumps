using System;
using System.Collections.Generic;
using System.IO;

namespace Mirror.BouncyCastle.Asn1.X509;

public class V2TbsCertListGenerator
{
	private DerInteger version = new DerInteger(1);

	private AlgorithmIdentifier signature;

	private X509Name issuer;

	private Time thisUpdate;

	private Time nextUpdate;

	private X509Extensions extensions;

	private List<Asn1Sequence> crlEntries;

	public void SetSignature(AlgorithmIdentifier signature)
	{
		this.signature = signature;
	}

	public void SetIssuer(X509Name issuer)
	{
		this.issuer = issuer;
	}

	public void SetThisUpdate(Asn1UtcTime thisUpdate)
	{
		this.thisUpdate = new Time(thisUpdate);
	}

	public void SetNextUpdate(Asn1UtcTime nextUpdate)
	{
		this.nextUpdate = ((nextUpdate != null) ? new Time(nextUpdate) : null);
	}

	public void SetThisUpdate(Time thisUpdate)
	{
		this.thisUpdate = thisUpdate;
	}

	public void SetNextUpdate(Time nextUpdate)
	{
		this.nextUpdate = nextUpdate;
	}

	public void AddCrlEntry(Asn1Sequence crlEntry)
	{
		if (crlEntries == null)
		{
			crlEntries = new List<Asn1Sequence>();
		}
		crlEntries.Add(crlEntry);
	}

	public void AddCrlEntry(DerInteger userCertificate, Asn1UtcTime revocationDate, int reason)
	{
		AddCrlEntry(userCertificate, new Time(revocationDate), reason);
	}

	public void AddCrlEntry(DerInteger userCertificate, Time revocationDate, int reason)
	{
		AddCrlEntry(userCertificate, revocationDate, reason, null);
	}

	public void AddCrlEntry(DerInteger userCertificate, Time revocationDate, int reason, Asn1GeneralizedTime invalidityDate)
	{
		List<DerObjectIdentifier> list = new List<DerObjectIdentifier>();
		List<X509Extension> list2 = new List<X509Extension>();
		if (reason != 0)
		{
			CrlReason crlReason = new CrlReason(reason);
			try
			{
				list.Add(X509Extensions.ReasonCode);
				list2.Add(new X509Extension(critical: false, new DerOctetString(crlReason.GetEncoded())));
			}
			catch (IOException ex)
			{
				throw new ArgumentException("error encoding reason: " + ex);
			}
		}
		if (invalidityDate != null)
		{
			try
			{
				list.Add(X509Extensions.InvalidityDate);
				list2.Add(new X509Extension(critical: false, new DerOctetString(invalidityDate.GetEncoded())));
			}
			catch (IOException ex2)
			{
				throw new ArgumentException("error encoding invalidityDate: " + ex2);
			}
		}
		if (list.Count != 0)
		{
			AddCrlEntry(userCertificate, revocationDate, new X509Extensions(list, list2));
		}
		else
		{
			AddCrlEntry(userCertificate, revocationDate, null);
		}
	}

	public void AddCrlEntry(DerInteger userCertificate, Time revocationDate, X509Extensions extensions)
	{
		Asn1EncodableVector asn1EncodableVector = new Asn1EncodableVector(userCertificate, revocationDate);
		if (extensions != null)
		{
			asn1EncodableVector.Add(extensions);
		}
		AddCrlEntry(new DerSequence(asn1EncodableVector));
	}

	public void SetExtensions(X509Extensions extensions)
	{
		this.extensions = extensions;
	}

	public Asn1Sequence GeneratePreTbsCertList()
	{
		if (signature != null)
		{
			throw new InvalidOperationException("signature should not be set in PreTBSCertList generator");
		}
		if (issuer == null || thisUpdate == null)
		{
			throw new InvalidOperationException("Not all mandatory fields set in V2 PreTBSCertList generator");
		}
		return GenerateTbsCertificateStructure();
	}

	public TbsCertificateList GenerateTbsCertList()
	{
		if (signature == null || issuer == null || thisUpdate == null)
		{
			throw new InvalidOperationException("Not all mandatory fields set in V2 TbsCertList generator.");
		}
		return TbsCertificateList.GetInstance(GenerateTbsCertificateStructure());
	}

	private Asn1Sequence GenerateTbsCertificateStructure()
	{
		Asn1EncodableVector asn1EncodableVector = new Asn1EncodableVector(7);
		asn1EncodableVector.Add(version);
		asn1EncodableVector.AddOptional(signature);
		asn1EncodableVector.Add(issuer);
		asn1EncodableVector.Add(thisUpdate);
		asn1EncodableVector.AddOptional(nextUpdate);
		if (crlEntries != null && crlEntries.Count > 0)
		{
			Asn1Encodable[] elements = crlEntries.ToArray();
			asn1EncodableVector.Add(new DerSequence(elements));
		}
		asn1EncodableVector.AddOptionalTagged(isExplicit: true, 0, extensions);
		return new DerSequence(asn1EncodableVector);
	}
}
