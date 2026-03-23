using System;
using System.Collections.Generic;
using System.IO;
using Mirror.BouncyCastle.Asn1;
using Mirror.BouncyCastle.Asn1.Pkcs;
using Mirror.BouncyCastle.Asn1.X509;
using Mirror.BouncyCastle.Security.Certificates;
using Mirror.BouncyCastle.Utilities.IO;

namespace Mirror.BouncyCastle.X509;

public class X509CertificateParser
{
	private static readonly PemParser PemCertParser = new PemParser("CERTIFICATE");

	private Asn1Set sData;

	private int sDataObjectCount;

	private Stream currentStream;

	private X509Certificate ReadDerCertificate(Asn1InputStream dIn)
	{
		Asn1Sequence asn1Sequence = (Asn1Sequence)dIn.ReadObject();
		if (asn1Sequence.Count > 1 && asn1Sequence[0] is DerObjectIdentifier && asn1Sequence[0].Equals(PkcsObjectIdentifiers.SignedData))
		{
			sData = SignedData.GetInstance(Asn1Sequence.GetInstance((Asn1TaggedObject)asn1Sequence[1], declaredExplicit: true)).Certificates;
			return GetCertificate();
		}
		return new X509Certificate(X509CertificateStructure.GetInstance(asn1Sequence));
	}

	private X509Certificate ReadPemCertificate(Stream inStream)
	{
		Asn1Sequence asn1Sequence = PemCertParser.ReadPemObject(inStream);
		if (asn1Sequence != null)
		{
			return new X509Certificate(X509CertificateStructure.GetInstance(asn1Sequence));
		}
		return null;
	}

	private X509Certificate GetCertificate()
	{
		if (sData != null)
		{
			while (sDataObjectCount < sData.Count)
			{
				object obj = sData[sDataObjectCount++];
				if (obj is Asn1Sequence)
				{
					return new X509Certificate(X509CertificateStructure.GetInstance(obj));
				}
			}
		}
		return null;
	}

	public X509Certificate ReadCertificate(byte[] input)
	{
		return ReadCertificate(new MemoryStream(input, writable: false));
	}

	public IList<X509Certificate> ReadCertificates(byte[] input)
	{
		return ReadCertificates(new MemoryStream(input, writable: false));
	}

	public X509Certificate ReadCertificate(Stream inStream)
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
			using Asn1InputStream dIn = new Asn1InputStream(inStream, int.MaxValue, leaveOpen: true);
			return ReadDerCertificate(dIn);
		}
		catch (CertificateException)
		{
			throw;
		}
		catch (Exception innerException)
		{
			throw new CertificateException("Failed to read certificate", innerException);
		}
	}

	public IList<X509Certificate> ReadCertificates(Stream inStream)
	{
		return new List<X509Certificate>(ParseCertificates(inStream));
	}

	public IEnumerable<X509Certificate> ParseCertificates(Stream inStream)
	{
		X509Certificate x509Certificate;
		while ((x509Certificate = ReadCertificate(inStream)) != null)
		{
			yield return x509Certificate;
		}
	}
}
