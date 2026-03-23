using System;
using System.Collections.Generic;
using Mirror.BouncyCastle.Asn1;
using Mirror.BouncyCastle.Asn1.X509;
using Mirror.BouncyCastle.Crypto;
using Mirror.BouncyCastle.Math;
using Mirror.BouncyCastle.Security.Certificates;
using Mirror.BouncyCastle.X509.Extension;

namespace Mirror.BouncyCastle.X509;

public class X509V3CertificateGenerator
{
	private readonly X509ExtensionsGenerator extGenerator = new X509ExtensionsGenerator();

	private V3TbsCertificateGenerator tbsGen;

	public IEnumerable<string> SignatureAlgNames => X509Utilities.GetAlgNames();

	public X509V3CertificateGenerator()
	{
		tbsGen = new V3TbsCertificateGenerator();
	}

	public X509V3CertificateGenerator(X509Certificate template)
		: this(template.CertificateStructure)
	{
	}

	public X509V3CertificateGenerator(X509CertificateStructure template)
	{
		tbsGen = new V3TbsCertificateGenerator();
		tbsGen.SetSerialNumber(template.SerialNumber);
		tbsGen.SetIssuer(template.Issuer);
		tbsGen.SetStartDate(template.StartDate);
		tbsGen.SetEndDate(template.EndDate);
		tbsGen.SetSubject(template.Subject);
		tbsGen.SetSubjectPublicKeyInfo(template.SubjectPublicKeyInfo);
		X509Extensions extensions = template.TbsCertificate.Extensions;
		foreach (DerObjectIdentifier extensionOid in extensions.ExtensionOids)
		{
			if (!X509Extensions.SubjectAltPublicKeyInfo.Equals(extensionOid) && !X509Extensions.AltSignatureAlgorithm.Equals(extensionOid) && !X509Extensions.AltSignatureValue.Equals(extensionOid))
			{
				X509Extension extension = extensions.GetExtension(extensionOid);
				extGenerator.AddExtension(extensionOid, extension.critical, extension.Value.GetOctets());
			}
		}
	}

	public void Reset()
	{
		tbsGen = new V3TbsCertificateGenerator();
		extGenerator.Reset();
	}

	public void SetSerialNumber(BigInteger serialNumber)
	{
		if (serialNumber.SignValue <= 0)
		{
			throw new ArgumentException("serial number must be a positive integer", "serialNumber");
		}
		tbsGen.SetSerialNumber(new DerInteger(serialNumber));
	}

	public void SetIssuerDN(X509Name issuer)
	{
		tbsGen.SetIssuer(issuer);
	}

	public void SetNotBefore(DateTime date)
	{
		tbsGen.SetStartDate(new Time(date));
	}

	public void SetNotAfter(DateTime date)
	{
		tbsGen.SetEndDate(new Time(date));
	}

	public void SetSubjectDN(X509Name subject)
	{
		tbsGen.SetSubject(subject);
	}

	public void SetPublicKey(AsymmetricKeyParameter publicKey)
	{
		tbsGen.SetSubjectPublicKeyInfo(SubjectPublicKeyInfoFactory.CreateSubjectPublicKeyInfo(publicKey));
	}

	public void SetSubjectPublicKeyInfo(SubjectPublicKeyInfo subjectPublicKeyInfo)
	{
		tbsGen.SetSubjectPublicKeyInfo(subjectPublicKeyInfo);
	}

	public void SetSubjectUniqueID(bool[] uniqueID)
	{
		tbsGen.SetSubjectUniqueID(BooleanToBitString(uniqueID));
	}

	public void SetIssuerUniqueID(bool[] uniqueID)
	{
		tbsGen.SetIssuerUniqueID(BooleanToBitString(uniqueID));
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

	public void CopyAndAddExtension(string oid, bool critical, X509Certificate cert)
	{
		CopyAndAddExtension(new DerObjectIdentifier(oid), critical, cert);
	}

	public void CopyAndAddExtension(DerObjectIdentifier oid, bool critical, X509Certificate cert)
	{
		Asn1OctetString extensionValue = cert.GetExtensionValue(oid);
		if (extensionValue == null)
		{
			throw new CertificateParsingException("extension " + oid?.ToString() + " not present");
		}
		try
		{
			Asn1Encodable extensionValue2 = X509ExtensionUtilities.FromExtensionValue(extensionValue);
			AddExtension(oid, critical, extensionValue2);
		}
		catch (Exception ex)
		{
			throw new CertificateParsingException(ex.Message, ex);
		}
	}

	public X509Certificate Generate(ISignatureFactory signatureFactory)
	{
		AlgorithmIdentifier algorithmIdentifier = (AlgorithmIdentifier)signatureFactory.AlgorithmDetails;
		tbsGen.SetSignature(algorithmIdentifier);
		if (!extGenerator.IsEmpty)
		{
			tbsGen.SetExtensions(extGenerator.Generate());
		}
		TbsCertificateStructure tbsCertificateStructure = tbsGen.GenerateTbsCertificate();
		DerBitString sig = X509Utilities.GenerateSignature(signatureFactory, tbsCertificateStructure);
		return new X509Certificate(new X509CertificateStructure(tbsCertificateStructure, algorithmIdentifier, sig));
	}

	public X509Certificate Generate(ISignatureFactory signatureFactory, bool isCritical, ISignatureFactory altSignatureFactory)
	{
		tbsGen.SetSignature(null);
		AlgorithmIdentifier extValue = (AlgorithmIdentifier)altSignatureFactory.AlgorithmDetails;
		extGenerator.AddExtension(X509Extensions.AltSignatureAlgorithm, isCritical, extValue);
		tbsGen.SetExtensions(extGenerator.Generate());
		DerBitString extValue2 = X509Utilities.GenerateSignature(altSignatureFactory, tbsGen.GeneratePreTbsCertificate());
		extGenerator.AddExtension(X509Extensions.AltSignatureValue, isCritical, extValue2);
		return Generate(signatureFactory);
	}

	internal static DerBitString BooleanToBitString(bool[] id)
	{
		byte[] array = new byte[(id.Length + 7) / 8];
		for (int i = 0; i != id.Length; i++)
		{
			if (id[i])
			{
				array[i >> 3] |= (byte)(128 >> (i & 7));
			}
		}
		return new DerBitString(array, (8 - id.Length) & 7);
	}
}
