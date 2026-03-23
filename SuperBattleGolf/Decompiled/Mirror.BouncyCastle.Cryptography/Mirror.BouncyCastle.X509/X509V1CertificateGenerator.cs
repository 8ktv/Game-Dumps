using System;
using System.Collections.Generic;
using Mirror.BouncyCastle.Asn1;
using Mirror.BouncyCastle.Asn1.X509;
using Mirror.BouncyCastle.Crypto;
using Mirror.BouncyCastle.Math;

namespace Mirror.BouncyCastle.X509;

public class X509V1CertificateGenerator
{
	private V1TbsCertificateGenerator tbsGen;

	public IEnumerable<string> SignatureAlgNames => X509Utilities.GetAlgNames();

	public X509V1CertificateGenerator()
	{
		tbsGen = new V1TbsCertificateGenerator();
	}

	public void Reset()
	{
		tbsGen = new V1TbsCertificateGenerator();
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
		try
		{
			tbsGen.SetSubjectPublicKeyInfo(SubjectPublicKeyInfoFactory.CreateSubjectPublicKeyInfo(publicKey));
		}
		catch (Exception ex)
		{
			throw new ArgumentException("unable to process key - " + ex.ToString());
		}
	}

	public X509Certificate Generate(ISignatureFactory signatureFactory)
	{
		AlgorithmIdentifier algorithmIdentifier = (AlgorithmIdentifier)signatureFactory.AlgorithmDetails;
		tbsGen.SetSignature(algorithmIdentifier);
		TbsCertificateStructure tbsCertificateStructure = tbsGen.GenerateTbsCertificate();
		DerBitString sig = X509Utilities.GenerateSignature(signatureFactory, tbsCertificateStructure);
		return new X509Certificate(new X509CertificateStructure(tbsCertificateStructure, algorithmIdentifier, sig));
	}
}
