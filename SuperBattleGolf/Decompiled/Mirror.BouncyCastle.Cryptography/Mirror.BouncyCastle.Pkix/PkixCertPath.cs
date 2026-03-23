using System;
using System.Collections.Generic;
using System.IO;
using Mirror.BouncyCastle.Asn1;
using Mirror.BouncyCastle.Asn1.Pkcs;
using Mirror.BouncyCastle.Asn1.X509;
using Mirror.BouncyCastle.OpenSsl;
using Mirror.BouncyCastle.Security.Certificates;
using Mirror.BouncyCastle.Utilities;
using Mirror.BouncyCastle.Utilities.Collections;
using Mirror.BouncyCastle.X509;

namespace Mirror.BouncyCastle.Pkix;

public class PkixCertPath
{
	private static readonly List<string> EncodingNames = new List<string> { "PkiPath", "PEM", "PKCS7" };

	private readonly IList<X509Certificate> m_certificates;

	public virtual IEnumerable<string> Encodings => CollectionUtilities.Proxy(EncodingNames);

	public virtual IList<X509Certificate> Certificates => CollectionUtilities.ReadOnly(m_certificates);

	private static IList<X509Certificate> SortCerts(IList<X509Certificate> certs)
	{
		if (certs.Count < 2)
		{
			return certs;
		}
		X509Name issuerDN = certs[0].IssuerDN;
		bool flag = true;
		for (int i = 1; i != certs.Count; i++)
		{
			X509Certificate x509Certificate = certs[i];
			if (issuerDN.Equivalent(x509Certificate.SubjectDN, inOrder: true))
			{
				issuerDN = x509Certificate.IssuerDN;
				continue;
			}
			flag = false;
			break;
		}
		if (flag)
		{
			return certs;
		}
		List<X509Certificate> list = new List<X509Certificate>(certs.Count);
		List<X509Certificate> result = new List<X509Certificate>(certs);
		for (int j = 0; j < certs.Count; j++)
		{
			X509Certificate x509Certificate2 = certs[j];
			bool flag2 = false;
			X509Name subjectDN = x509Certificate2.SubjectDN;
			foreach (X509Certificate cert in certs)
			{
				if (cert.IssuerDN.Equivalent(subjectDN, inOrder: true))
				{
					flag2 = true;
					break;
				}
			}
			if (!flag2)
			{
				list.Add(x509Certificate2);
				certs.RemoveAt(j);
			}
		}
		if (list.Count > 1)
		{
			return result;
		}
		for (int k = 0; k != list.Count; k++)
		{
			issuerDN = list[k].IssuerDN;
			for (int l = 0; l < certs.Count; l++)
			{
				X509Certificate x509Certificate3 = certs[l];
				if (issuerDN.Equivalent(x509Certificate3.SubjectDN, inOrder: true))
				{
					list.Add(x509Certificate3);
					certs.RemoveAt(l);
					break;
				}
			}
		}
		if (certs.Count > 0)
		{
			return result;
		}
		return list;
	}

	public PkixCertPath(IList<X509Certificate> certificates)
	{
		m_certificates = SortCerts(new List<X509Certificate>(certificates));
	}

	public PkixCertPath(Stream inStream)
		: this(inStream, "PkiPath")
	{
	}

	public PkixCertPath(Stream inStream, string encoding)
	{
		IList<X509Certificate> certs;
		try
		{
			if (Platform.EqualsIgnoreCase("PkiPath", encoding))
			{
				using Asn1InputStream asn1InputStream = new Asn1InputStream(inStream, int.MaxValue, leaveOpen: true);
				X509Certificate[] array = ((asn1InputStream.ReadObject() as Asn1Sequence) ?? throw new CertificateException("input stream does not contain a ASN1 SEQUENCE while reading PkiPath encoded data to load CertPath")).MapElements((Asn1Encodable element) => new X509Certificate(X509CertificateStructure.GetInstance(element.ToAsn1Object())));
				Array.Reverse((Array)array);
				certs = new List<X509Certificate>(array);
			}
			else
			{
				if (!Platform.EqualsIgnoreCase("PEM", encoding) && !Platform.EqualsIgnoreCase("PKCS7", encoding))
				{
					throw new CertificateException("unsupported encoding: " + encoding);
				}
				certs = new X509CertificateParser().ReadCertificates(inStream);
			}
		}
		catch (IOException ex)
		{
			throw new CertificateException("IOException throw while decoding CertPath:\n" + ex.ToString());
		}
		m_certificates = SortCerts(certs);
	}

	public override bool Equals(object obj)
	{
		if (this == obj)
		{
			return true;
		}
		if (!(obj is PkixCertPath pkixCertPath))
		{
			return false;
		}
		IList<X509Certificate> certificates = Certificates;
		IList<X509Certificate> certificates2 = pkixCertPath.Certificates;
		if (certificates.Count != certificates2.Count)
		{
			return false;
		}
		IEnumerator<X509Certificate> enumerator = certificates.GetEnumerator();
		IEnumerator<X509Certificate> enumerator2 = certificates2.GetEnumerator();
		while (enumerator.MoveNext())
		{
			enumerator2.MoveNext();
			if (!object.Equals(enumerator.Current, enumerator2.Current))
			{
				return false;
			}
		}
		return true;
	}

	public override int GetHashCode()
	{
		return m_certificates.GetHashCode();
	}

	public virtual byte[] GetEncoded()
	{
		return GetEncoded(EncodingNames[0]);
	}

	public virtual byte[] GetEncoded(string encoding)
	{
		if (Platform.EqualsIgnoreCase(encoding, "PkiPath"))
		{
			Asn1EncodableVector asn1EncodableVector = new Asn1EncodableVector(m_certificates.Count);
			for (int num = m_certificates.Count - 1; num >= 0; num--)
			{
				asn1EncodableVector.Add(ToAsn1Object(m_certificates[num]));
			}
			return ToDerEncoded(new DerSequence(asn1EncodableVector));
		}
		if (Platform.EqualsIgnoreCase(encoding, "PKCS7"))
		{
			ContentInfo contentInfo = new ContentInfo(PkcsObjectIdentifiers.Data, null);
			Asn1EncodableVector asn1EncodableVector2 = new Asn1EncodableVector(m_certificates.Count);
			foreach (X509Certificate certificate in m_certificates)
			{
				asn1EncodableVector2.Add(ToAsn1Object(certificate));
			}
			SignedData content = new SignedData(new DerInteger(1), new DerSet(), contentInfo, DerSet.FromVector(asn1EncodableVector2), null, new DerSet());
			return ToDerEncoded(new ContentInfo(PkcsObjectIdentifiers.SignedData, content));
		}
		if (Platform.EqualsIgnoreCase(encoding, "PEM"))
		{
			MemoryStream memoryStream = new MemoryStream();
			try
			{
				using PemWriter pemWriter = new PemWriter(new StreamWriter(memoryStream));
				foreach (X509Certificate certificate2 in m_certificates)
				{
					pemWriter.WriteObject(certificate2);
				}
			}
			catch (Exception)
			{
				throw new CertificateEncodingException("can't encode certificate for PEM encoded path");
			}
			return memoryStream.ToArray();
		}
		throw new CertificateEncodingException("unsupported encoding: " + encoding);
	}

	private Asn1Object ToAsn1Object(X509Certificate cert)
	{
		try
		{
			return cert.CertificateStructure.ToAsn1Object();
		}
		catch (Exception innerException)
		{
			throw new CertificateEncodingException("Exception while encoding certificate", innerException);
		}
	}

	private byte[] ToDerEncoded(Asn1Encodable obj)
	{
		try
		{
			return obj.GetEncoded("DER");
		}
		catch (IOException innerException)
		{
			throw new CertificateEncodingException("Exception thrown", innerException);
		}
	}
}
