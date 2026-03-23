using System;
using System.Collections.Generic;
using System.IO;
using Mirror.BouncyCastle.Asn1;
using Mirror.BouncyCastle.Asn1.X509;
using Mirror.BouncyCastle.Crypto;
using Mirror.BouncyCastle.Crypto.Operators;
using Mirror.BouncyCastle.Math;
using Mirror.BouncyCastle.Security;
using Mirror.BouncyCastle.Security.Certificates;

namespace Mirror.BouncyCastle.X509;

public class X509V2AttributeCertificate : X509ExtensionBase
{
	private readonly AttributeCertificate cert;

	private readonly DateTime notBefore;

	private readonly DateTime notAfter;

	public virtual AttributeCertificate AttributeCertificate => cert;

	public virtual int Version => cert.ACInfo.Version.IntValueExact + 1;

	public virtual BigInteger SerialNumber => cert.ACInfo.SerialNumber.Value;

	public virtual AttributeCertificateHolder Holder => new AttributeCertificateHolder((Asn1Sequence)cert.ACInfo.Holder.ToAsn1Object());

	public virtual AttributeCertificateIssuer Issuer => new AttributeCertificateIssuer(cert.ACInfo.Issuer);

	public virtual DateTime NotBefore => notBefore;

	public virtual DateTime NotAfter => notAfter;

	public virtual bool IsValidNow => IsValid(DateTime.UtcNow);

	public virtual AlgorithmIdentifier SignatureAlgorithm => cert.SignatureAlgorithm;

	private static AttributeCertificate GetObject(Stream input)
	{
		try
		{
			return AttributeCertificate.GetInstance(Asn1Object.FromStream(input));
		}
		catch (IOException)
		{
			throw;
		}
		catch (Exception innerException)
		{
			throw new IOException("exception decoding certificate structure", innerException);
		}
	}

	public X509V2AttributeCertificate(Stream encIn)
		: this(GetObject(encIn))
	{
	}

	public X509V2AttributeCertificate(byte[] encoded)
		: this(new MemoryStream(encoded, writable: false))
	{
	}

	public X509V2AttributeCertificate(AttributeCertificate cert)
	{
		this.cert = cert;
		try
		{
			notAfter = cert.ACInfo.AttrCertValidityPeriod.NotAfterTime.ToDateTime();
			notBefore = cert.ACInfo.AttrCertValidityPeriod.NotBeforeTime.ToDateTime();
		}
		catch (Exception innerException)
		{
			throw new IOException("invalid data structure in certificate!", innerException);
		}
	}

	public virtual bool[] GetIssuerUniqueID()
	{
		DerBitString issuerUniqueID = cert.ACInfo.IssuerUniqueID;
		if (issuerUniqueID != null)
		{
			byte[] bytes = issuerUniqueID.GetBytes();
			bool[] array = new bool[bytes.Length * 8 - issuerUniqueID.PadBits];
			for (int i = 0; i != array.Length; i++)
			{
				array[i] = (bytes[i / 8] & (128 >> i % 8)) != 0;
			}
			return array;
		}
		return null;
	}

	public virtual bool IsValid(DateTime date)
	{
		if (date.CompareTo(NotBefore) >= 0)
		{
			return date.CompareTo(NotAfter) <= 0;
		}
		return false;
	}

	public virtual void CheckValidity()
	{
		CheckValidity(DateTime.UtcNow);
	}

	public virtual void CheckValidity(DateTime date)
	{
		if (date.CompareTo(NotAfter) > 0)
		{
			throw new CertificateExpiredException("certificate expired on " + NotAfter);
		}
		if (date.CompareTo(NotBefore) < 0)
		{
			throw new CertificateNotYetValidException("certificate not valid until " + NotBefore);
		}
	}

	public virtual byte[] GetSignature()
	{
		return cert.GetSignatureOctets();
	}

	public virtual bool IsSignatureValid(AsymmetricKeyParameter key)
	{
		return CheckSignatureValid(new Asn1VerifierFactory(cert.SignatureAlgorithm, key));
	}

	public virtual bool IsSignatureValid(IVerifierFactoryProvider verifierProvider)
	{
		return CheckSignatureValid(verifierProvider.CreateVerifierFactory(cert.SignatureAlgorithm));
	}

	public virtual void Verify(AsymmetricKeyParameter key)
	{
		CheckSignature(new Asn1VerifierFactory(cert.SignatureAlgorithm, key));
	}

	public virtual void Verify(IVerifierFactoryProvider verifierProvider)
	{
		CheckSignature(verifierProvider.CreateVerifierFactory(cert.SignatureAlgorithm));
	}

	protected virtual void CheckSignature(IVerifierFactory verifier)
	{
		if (!CheckSignatureValid(verifier))
		{
			throw new InvalidKeyException("Public key presented not for certificate signature");
		}
	}

	protected virtual bool CheckSignatureValid(IVerifierFactory verifier)
	{
		AttributeCertificateInfo aCInfo = cert.ACInfo;
		if (!cert.SignatureAlgorithm.Equals(aCInfo.Signature))
		{
			throw new CertificateException("Signature algorithm in certificate info not same as outer certificate");
		}
		return X509Utilities.VerifySignature(verifier, aCInfo, cert.SignatureValue);
	}

	public virtual byte[] GetEncoded()
	{
		return cert.GetEncoded();
	}

	protected override X509Extensions GetX509Extensions()
	{
		return cert.ACInfo.Extensions;
	}

	public virtual X509Attribute[] GetAttributes()
	{
		return cert.ACInfo.Attributes.MapElements((Asn1Encodable element) => new X509Attribute(element));
	}

	public virtual X509Attribute[] GetAttributes(string oid)
	{
		Asn1Sequence attributes = cert.ACInfo.Attributes;
		List<X509Attribute> list = new List<X509Attribute>();
		for (int i = 0; i != attributes.Count; i++)
		{
			X509Attribute x509Attribute = new X509Attribute(attributes[i]);
			if (x509Attribute.Oid.Equals(oid))
			{
				list.Add(x509Attribute);
			}
		}
		if (list.Count < 1)
		{
			return null;
		}
		return list.ToArray();
	}

	public override bool Equals(object obj)
	{
		if (obj == this)
		{
			return true;
		}
		if (!(obj is X509V2AttributeCertificate x509V2AttributeCertificate))
		{
			return false;
		}
		return cert.Equals(x509V2AttributeCertificate.cert);
	}

	public override int GetHashCode()
	{
		return cert.GetHashCode();
	}
}
