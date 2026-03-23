using System;
using System.Collections.Generic;
using System.IO;
using Mirror.BouncyCastle.Asn1;
using Mirror.BouncyCastle.Asn1.X509;
using Mirror.BouncyCastle.Security.Certificates;
using Mirror.BouncyCastle.Utilities.IO;

namespace Mirror.BouncyCastle.X509;

public class X509CertPairParser
{
	private Stream currentStream;

	private X509CertificatePair ReadDerCrossCertificatePair(Stream inStream)
	{
		using Asn1InputStream asn1InputStream = new Asn1InputStream(inStream, int.MaxValue, leaveOpen: true);
		return new X509CertificatePair(CertificatePair.GetInstance(asn1InputStream.ReadObject()));
	}

	public X509CertificatePair ReadCertPair(byte[] input)
	{
		return ReadCertPair(new MemoryStream(input, writable: false));
	}

	public IList<X509CertificatePair> ReadCertPairs(byte[] input)
	{
		return ReadCertPairs(new MemoryStream(input, writable: false));
	}

	public X509CertificatePair ReadCertPair(Stream inStream)
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
		}
		else if (currentStream != inStream)
		{
			currentStream = inStream;
		}
		try
		{
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
			return ReadDerCrossCertificatePair(inStream);
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

	public IList<X509CertificatePair> ReadCertPairs(Stream inStream)
	{
		List<X509CertificatePair> list = new List<X509CertificatePair>();
		X509CertificatePair item;
		while ((item = ReadCertPair(inStream)) != null)
		{
			list.Add(item);
		}
		return list;
	}
}
