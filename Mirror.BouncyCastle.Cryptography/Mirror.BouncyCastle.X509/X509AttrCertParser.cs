using System;
using System.Collections.Generic;
using System.IO;
using Mirror.BouncyCastle.Asn1;
using Mirror.BouncyCastle.Asn1.Pkcs;
using Mirror.BouncyCastle.Asn1.X509;
using Mirror.BouncyCastle.Security.Certificates;
using Mirror.BouncyCastle.Utilities.IO;

namespace Mirror.BouncyCastle.X509;

public class X509AttrCertParser
{
	private static readonly PemParser PemAttrCertParser = new PemParser("ATTRIBUTE CERTIFICATE");

	private Asn1Set sData;

	private int sDataObjectCount;

	private Stream currentStream;

	private X509V2AttributeCertificate ReadDerCertificate(Asn1InputStream dIn)
	{
		Asn1Sequence asn1Sequence = (Asn1Sequence)dIn.ReadObject();
		if (asn1Sequence.Count > 1 && asn1Sequence[0] is DerObjectIdentifier && asn1Sequence[0].Equals(PkcsObjectIdentifiers.SignedData))
		{
			sData = SignedData.GetInstance(Asn1Sequence.GetInstance((Asn1TaggedObject)asn1Sequence[1], declaredExplicit: true)).Certificates;
			return GetCertificate();
		}
		return new X509V2AttributeCertificate(AttributeCertificate.GetInstance(asn1Sequence));
	}

	private X509V2AttributeCertificate GetCertificate()
	{
		if (sData != null)
		{
			while (sDataObjectCount < sData.Count)
			{
				if (sData[sDataObjectCount++].ToAsn1Object() is Asn1TaggedObject { TagNo: 2 } asn1TaggedObject)
				{
					return new X509V2AttributeCertificate(AttributeCertificate.GetInstance(Asn1Sequence.GetInstance(asn1TaggedObject, declaredExplicit: false)));
				}
			}
		}
		return null;
	}

	private X509V2AttributeCertificate ReadPemCertificate(Stream inStream)
	{
		Asn1Sequence asn1Sequence = PemAttrCertParser.ReadPemObject(inStream);
		if (asn1Sequence != null)
		{
			return new X509V2AttributeCertificate(AttributeCertificate.GetInstance(asn1Sequence));
		}
		return null;
	}

	public X509V2AttributeCertificate ReadAttrCert(byte[] input)
	{
		return ReadAttrCert(new MemoryStream(input, writable: false));
	}

	public IList<X509V2AttributeCertificate> ReadAttrCerts(byte[] input)
	{
		return ReadAttrCerts(new MemoryStream(input, writable: false));
	}

	public X509V2AttributeCertificate ReadAttrCert(Stream inStream)
	{
		if (inStream == null)
		{
			throw new ArgumentNullException("inStream");
		}
		if (!inStream.CanRead)
		{
			throw new ArgumentException("inStream must be read-able", "inStream");
		}
		if (currentStream == null)
		{
			currentStream = inStream;
			sData = null;
			sDataObjectCount = 0;
		}
		else if (currentStream != inStream)
		{
			currentStream = inStream;
			sData = null;
			sDataObjectCount = 0;
		}
		try
		{
			if (sData != null)
			{
				if (sDataObjectCount != sData.Count)
				{
					return GetCertificate();
				}
				sData = null;
				sDataObjectCount = 0;
				return null;
			}
			int num = inStream.ReadByte();
			if (num < 0)
			{
				return null;
			}
			if (inStream.CanSeek)
			{
				inStream.Seek(-1L, SeekOrigin.Current);
			}
			else
			{
				PushbackStream pushbackStream = new PushbackStream(inStream);
				pushbackStream.Unread(num);
				inStream = pushbackStream;
			}
			if (num != 48)
			{
				return ReadPemCertificate(inStream);
			}
			return ReadDerCertificate(new Asn1InputStream(inStream));
		}
		catch (CertificateException)
		{
			throw;
		}
		catch (Exception ex2)
		{
			throw new CertificateException(ex2.ToString());
		}
	}

	public IList<X509V2AttributeCertificate> ReadAttrCerts(Stream inStream)
	{
		List<X509V2AttributeCertificate> list = new List<X509V2AttributeCertificate>();
		X509V2AttributeCertificate item;
		while ((item = ReadAttrCert(inStream)) != null)
		{
			list.Add(item);
		}
		return list;
	}
}
