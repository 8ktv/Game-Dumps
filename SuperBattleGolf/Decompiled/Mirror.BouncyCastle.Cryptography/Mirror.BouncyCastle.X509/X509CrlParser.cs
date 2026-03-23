using System;
using System.Collections.Generic;
using System.IO;
using Mirror.BouncyCastle.Asn1;
using Mirror.BouncyCastle.Asn1.Pkcs;
using Mirror.BouncyCastle.Asn1.X509;
using Mirror.BouncyCastle.Security.Certificates;
using Mirror.BouncyCastle.Utilities.IO;

namespace Mirror.BouncyCastle.X509;

public class X509CrlParser
{
	private static readonly PemParser PemCrlParser = new PemParser("CRL");

	private Asn1Set sCrlData;

	private int sCrlDataObjectCount;

	private Stream currentCrlStream;

	public X509CrlParser()
	{
	}

	[Obsolete("Will be removed")]
	public X509CrlParser(bool lazyAsn1)
	{
	}

	private X509Crl ReadDerCrl(Asn1InputStream dIn)
	{
		Asn1Sequence asn1Sequence = (Asn1Sequence)dIn.ReadObject();
		if (asn1Sequence.Count > 1 && asn1Sequence[0] is DerObjectIdentifier && asn1Sequence[0].Equals(PkcsObjectIdentifiers.SignedData))
		{
			sCrlData = SignedData.GetInstance(Asn1Sequence.GetInstance((Asn1TaggedObject)asn1Sequence[1], declaredExplicit: true)).Crls;
			return GetCrl();
		}
		return new X509Crl(CertificateList.GetInstance(asn1Sequence));
	}

	private X509Crl ReadPemCrl(Stream inStream)
	{
		Asn1Sequence asn1Sequence = PemCrlParser.ReadPemObject(inStream);
		if (asn1Sequence != null)
		{
			return new X509Crl(CertificateList.GetInstance(asn1Sequence));
		}
		return null;
	}

	private X509Crl GetCrl()
	{
		if (sCrlData == null || sCrlDataObjectCount >= sCrlData.Count)
		{
			return null;
		}
		return new X509Crl(CertificateList.GetInstance(sCrlData[sCrlDataObjectCount++]));
	}

	public X509Crl ReadCrl(byte[] input)
	{
		return ReadCrl(new MemoryStream(input, writable: false));
	}

	public IList<X509Crl> ReadCrls(byte[] input)
	{
		return ReadCrls(new MemoryStream(input, writable: false));
	}

	public X509Crl ReadCrl(Stream inStream)
	{
		if (inStream == null)
		{
			throw new ArgumentNullException("inStream");
		}
		if (!inStream.CanRead)
		{
			throw new ArgumentException("inStream must be read-able", "inStream");
		}
		if (currentCrlStream == null)
		{
			currentCrlStream = inStream;
			sCrlData = null;
			sCrlDataObjectCount = 0;
		}
		else if (currentCrlStream != inStream)
		{
			currentCrlStream = inStream;
			sCrlData = null;
			sCrlDataObjectCount = 0;
		}
		try
		{
			if (sCrlData != null)
			{
				if (sCrlDataObjectCount != sCrlData.Count)
				{
					return GetCrl();
				}
				sCrlData = null;
				sCrlDataObjectCount = 0;
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
				return ReadPemCrl(inStream);
			}
			using Asn1InputStream dIn = new Asn1InputStream(inStream, int.MaxValue, leaveOpen: true);
			return ReadDerCrl(dIn);
		}
		catch (CrlException)
		{
			throw;
		}
		catch (Exception ex2)
		{
			throw new CrlException(ex2.ToString());
		}
	}

	public IList<X509Crl> ReadCrls(Stream inStream)
	{
		return new List<X509Crl>(ParseCrls(inStream));
	}

	public IEnumerable<X509Crl> ParseCrls(Stream inStream)
	{
		X509Crl x509Crl;
		while ((x509Crl = ReadCrl(inStream)) != null)
		{
			yield return x509Crl;
		}
	}
}
