using System;
using System.Collections.Generic;
using System.IO;
using Mirror.BouncyCastle.Asn1;
using Mirror.BouncyCastle.Asn1.X509;
using Mirror.BouncyCastle.Crypto;
using Mirror.BouncyCastle.Math;
using Mirror.BouncyCastle.Security.Certificates;

namespace Mirror.BouncyCastle.X509;

public class X509V2CrlGenerator
{
	private readonly X509ExtensionsGenerator extGenerator = new X509ExtensionsGenerator();

	private V2TbsCertListGenerator tbsGen;

	public IEnumerable<string> SignatureAlgNames => X509Utilities.GetAlgNames();

	public X509V2CrlGenerator()
	{
		tbsGen = new V2TbsCertListGenerator();
	}

	public X509V2CrlGenerator(X509Crl template)
		: this(template.CertificateList)
	{
	}

	public X509V2CrlGenerator(CertificateList template)
	{
		tbsGen = new V2TbsCertListGenerator();
		tbsGen.SetIssuer(template.Issuer);
		tbsGen.SetThisUpdate(template.ThisUpdate);
		tbsGen.SetNextUpdate(template.NextUpdate);
		AddCrl(new X509Crl(template));
		X509Extensions extensions = template.TbsCertList.Extensions;
		if (extensions == null)
		{
			return;
		}
		foreach (DerObjectIdentifier extensionOid in extensions.ExtensionOids)
		{
			if (!X509Extensions.AltSignatureAlgorithm.Equals(extensionOid) && !X509Extensions.AltSignatureValue.Equals(extensionOid))
			{
				X509Extension extension = extensions.GetExtension(extensionOid);
				extGenerator.AddExtension(extensionOid, extension.critical, extension.Value.GetOctets());
			}
		}
	}

	public void Reset()
	{
		tbsGen = new V2TbsCertListGenerator();
		extGenerator.Reset();
	}

	public void SetIssuerDN(X509Name issuer)
	{
		tbsGen.SetIssuer(issuer);
	}

	public void SetThisUpdate(DateTime date)
	{
		tbsGen.SetThisUpdate(new Time(date));
	}

	public void SetNextUpdate(DateTime date)
	{
		tbsGen.SetNextUpdate(new Time(date));
	}

	public void AddCrlEntry(BigInteger userCertificate, DateTime revocationDate, int reason)
	{
		tbsGen.AddCrlEntry(new DerInteger(userCertificate), new Time(revocationDate), reason);
	}

	public void AddCrlEntry(BigInteger userCertificate, DateTime revocationDate, int reason, DateTime invalidityDate)
	{
		tbsGen.AddCrlEntry(new DerInteger(userCertificate), new Time(revocationDate), reason, Rfc5280Asn1Utilities.CreateGeneralizedTime(invalidityDate));
	}

	public void AddCrlEntry(BigInteger userCertificate, DateTime revocationDate, X509Extensions extensions)
	{
		tbsGen.AddCrlEntry(new DerInteger(userCertificate), new Time(revocationDate), extensions);
	}

	public void AddCrl(X509Crl other)
	{
		if (other == null)
		{
			throw new ArgumentNullException("other");
		}
		ISet<X509CrlEntry> revokedCertificates = other.GetRevokedCertificates();
		if (revokedCertificates == null)
		{
			return;
		}
		foreach (X509CrlEntry item in revokedCertificates)
		{
			try
			{
				tbsGen.AddCrlEntry(Asn1Sequence.GetInstance(Asn1Object.FromByteArray(item.GetEncoded())));
			}
			catch (IOException innerException)
			{
				throw new CrlException("exception processing encoding of CRL", innerException);
			}
		}
	}

	public void AddExtension(string oid, bool critical, Asn1Encodable extensionValue)
	{
		extGenerator.AddExtension(new DerObjectIdentifier(oid), critical, extensionValue);
	}

	public void AddExtension(DerObjectIdentifier oid, bool critical, Asn1Encodable extensionValue)
	{
		extGenerator.AddExtension(oid, critical, extensionValue);
	}

	public void AddExtension(string oid, bool critical, byte[] extensionValue)
	{
		extGenerator.AddExtension(new DerObjectIdentifier(oid), critical, new DerOctetString(extensionValue));
	}

	public void AddExtension(DerObjectIdentifier oid, bool critical, byte[] extensionValue)
	{
		extGenerator.AddExtension(oid, critical, new DerOctetString(extensionValue));
	}

	public X509Crl Generate(ISignatureFactory signatureFactory)
	{
		AlgorithmIdentifier algorithmIdentifier = (AlgorithmIdentifier)signatureFactory.AlgorithmDetails;
		tbsGen.SetSignature(algorithmIdentifier);
		if (!extGenerator.IsEmpty)
		{
			tbsGen.SetExtensions(extGenerator.Generate());
		}
		TbsCertificateList tbsCertificateList = tbsGen.GenerateTbsCertList();
		DerBitString derBitString = X509Utilities.GenerateSignature(signatureFactory, tbsCertificateList);
		return new X509Crl(CertificateList.GetInstance(new DerSequence(tbsCertificateList, algorithmIdentifier, derBitString)));
	}

	public X509Crl Generate(ISignatureFactory signatureFactory, bool isCritical, ISignatureFactory altSignatureFactory)
	{
		tbsGen.SetSignature(null);
		AlgorithmIdentifier extValue = (AlgorithmIdentifier)altSignatureFactory.AlgorithmDetails;
		extGenerator.AddExtension(X509Extensions.AltSignatureAlgorithm, isCritical, extValue);
		tbsGen.SetExtensions(extGenerator.Generate());
		DerBitString extValue2 = X509Utilities.GenerateSignature(altSignatureFactory, tbsGen.GeneratePreTbsCertList());
		extGenerator.AddExtension(X509Extensions.AltSignatureValue, isCritical, extValue2);
		return Generate(signatureFactory);
	}
}
