using System;
using System.IO;
using Mirror.BouncyCastle.Asn1.X509;
using Mirror.BouncyCastle.Utilities;

namespace Mirror.BouncyCastle.Asn1.IsisMtt.Ocsp;

public class RequestedCertificate : Asn1Encodable, IAsn1Choice
{
	public enum Choice
	{
		Certificate = -1,
		PublicKeyCertificate,
		AttributeCertificate
	}

	private readonly X509CertificateStructure cert;

	private readonly byte[] publicKeyCert;

	private readonly byte[] attributeCert;

	public Choice Type
	{
		get
		{
			if (cert != null)
			{
				return Choice.Certificate;
			}
			if (publicKeyCert != null)
			{
				return Choice.PublicKeyCertificate;
			}
			return Choice.AttributeCertificate;
		}
	}

	public static RequestedCertificate GetInstance(object obj)
	{
		if (obj == null)
		{
			return null;
		}
		if (obj is RequestedCertificate result)
		{
			return result;
		}
		if (obj is Asn1Sequence)
		{
			return new RequestedCertificate(X509CertificateStructure.GetInstance(obj));
		}
		if (obj is Asn1TaggedObject tagged)
		{
			return new RequestedCertificate(tagged);
		}
		throw new ArgumentException("unknown object in factory: " + Platform.GetTypeName(obj), "obj");
	}

	public static RequestedCertificate GetInstance(Asn1TaggedObject obj, bool isExplicit)
	{
		return Asn1Utilities.GetInstanceFromChoice(obj, isExplicit, GetInstance);
	}

	private RequestedCertificate(Asn1TaggedObject tagged)
	{
		switch ((Choice)tagged.TagNo)
		{
		case Choice.AttributeCertificate:
			attributeCert = Asn1OctetString.GetInstance(tagged, declaredExplicit: true).GetOctets();
			break;
		case Choice.PublicKeyCertificate:
			publicKeyCert = Asn1OctetString.GetInstance(tagged, declaredExplicit: true).GetOctets();
			break;
		default:
			throw new ArgumentException("unknown tag number: " + tagged.TagNo);
		}
	}

	public RequestedCertificate(X509CertificateStructure certificate)
	{
		cert = certificate;
	}

	public RequestedCertificate(Choice type, byte[] certificateOctets)
		: this(new DerTaggedObject((int)type, new DerOctetString(certificateOctets)))
	{
	}

	public byte[] GetCertificateBytes()
	{
		if (cert != null)
		{
			try
			{
				return cert.GetEncoded();
			}
			catch (IOException ex)
			{
				throw new InvalidOperationException("can't decode certificate: " + ex);
			}
		}
		if (publicKeyCert != null)
		{
			return publicKeyCert;
		}
		return attributeCert;
	}

	public override Asn1Object ToAsn1Object()
	{
		if (publicKeyCert != null)
		{
			return new DerTaggedObject(0, new DerOctetString(publicKeyCert));
		}
		if (attributeCert != null)
		{
			return new DerTaggedObject(1, new DerOctetString(attributeCert));
		}
		return cert.ToAsn1Object();
	}
}
