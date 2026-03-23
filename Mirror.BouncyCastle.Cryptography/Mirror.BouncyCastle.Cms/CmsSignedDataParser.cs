using System;
using System.Collections.Generic;
using System.IO;
using Mirror.BouncyCastle.Asn1;
using Mirror.BouncyCastle.Asn1.Cms;
using Mirror.BouncyCastle.Asn1.X509;
using Mirror.BouncyCastle.Crypto;
using Mirror.BouncyCastle.Crypto.IO;
using Mirror.BouncyCastle.Security;
using Mirror.BouncyCastle.Utilities.Collections;
using Mirror.BouncyCastle.Utilities.IO;
using Mirror.BouncyCastle.X509;

namespace Mirror.BouncyCastle.Cms;

public class CmsSignedDataParser : CmsContentInfoParser
{
	private SignedDataParser _signedData;

	private DerObjectIdentifier _signedContentType;

	private CmsTypedStream _signedContent;

	private Dictionary<string, IDigest> m_digests;

	private HashSet<string> m_digestOids;

	private SignerInformationStore _signerInfoStore;

	private Asn1Set _certSet;

	private Asn1Set _crlSet;

	private bool _isCertCrlParsed;

	public int Version => _signedData.Version.IntValueExact;

	public ISet<string> DigestOids => new HashSet<string>(m_digestOids);

	public DerObjectIdentifier SignedContentType => _signedContentType;

	public CmsSignedDataParser(byte[] sigBlock)
		: this(new MemoryStream(sigBlock, writable: false))
	{
	}

	public CmsSignedDataParser(CmsTypedStream signedContent, byte[] sigBlock)
		: this(signedContent, new MemoryStream(sigBlock, writable: false))
	{
	}

	public CmsSignedDataParser(Stream sigData)
		: this(null, sigData)
	{
	}

	public CmsSignedDataParser(CmsTypedStream signedContent, Stream sigData)
		: base(sigData)
	{
		try
		{
			_signedContent = signedContent;
			_signedData = SignedDataParser.GetInstance(contentInfo.GetContent(16));
			m_digests = new Dictionary<string, IDigest>(StringComparer.OrdinalIgnoreCase);
			m_digestOids = new HashSet<string>();
			Asn1SetParser digestAlgorithms = _signedData.GetDigestAlgorithms();
			IAsn1Convertible obj;
			while ((obj = digestAlgorithms.ReadObject()) != null)
			{
				AlgorithmIdentifier instance = AlgorithmIdentifier.GetInstance(obj);
				try
				{
					DerObjectIdentifier algorithm = instance.Algorithm;
					string digestAlgName = CmsSignedHelper.GetDigestAlgName(algorithm);
					if (!m_digests.ContainsKey(digestAlgName))
					{
						m_digests[digestAlgName] = CmsSignedHelper.GetDigestInstance(digestAlgName);
						m_digestOids.Add(algorithm.Id);
					}
				}
				catch (SecurityUtilityException)
				{
				}
			}
			ContentInfoParser encapContentInfo = _signedData.GetEncapContentInfo();
			Asn1OctetStringParser asn1OctetStringParser = (Asn1OctetStringParser)encapContentInfo.GetContent(4);
			if (asn1OctetStringParser != null)
			{
				CmsTypedStream cmsTypedStream = new CmsTypedStream(encapContentInfo.ContentType.Id, asn1OctetStringParser.GetOctetStream());
				if (_signedContent == null)
				{
					_signedContent = cmsTypedStream;
				}
				else
				{
					cmsTypedStream.Drain();
				}
			}
			_signedContentType = ((_signedContent == null) ? encapContentInfo.ContentType : new DerObjectIdentifier(_signedContent.ContentType));
		}
		catch (IOException ex2)
		{
			throw new CmsException("io exception: " + ex2.Message, ex2);
		}
	}

	public SignerInformationStore GetSignerInfos()
	{
		if (_signerInfoStore == null)
		{
			PopulateCertCrlSets();
			List<SignerInformation> list = new List<SignerInformation>();
			Dictionary<string, byte[]> dictionary = new Dictionary<string, byte[]>(StringComparer.OrdinalIgnoreCase);
			foreach (KeyValuePair<string, IDigest> digest in m_digests)
			{
				dictionary[digest.Key] = DigestUtilities.DoFinal(digest.Value);
			}
			try
			{
				Asn1SetParser signerInfos = _signedData.GetSignerInfos();
				IAsn1Convertible obj;
				while ((obj = signerInfos.ReadObject()) != null)
				{
					SignerInfo instance = SignerInfo.GetInstance(obj);
					string digestAlgName = CmsSignedHelper.GetDigestAlgName(instance.DigestAlgorithm.Algorithm);
					byte[] calculatedDigest = dictionary[digestAlgName];
					list.Add(new SignerInformation(instance, _signedContentType, null, calculatedDigest));
				}
			}
			catch (IOException ex)
			{
				throw new CmsException("io exception: " + ex.Message, ex);
			}
			_signerInfoStore = new SignerInformationStore(list);
		}
		return _signerInfoStore;
	}

	public IStore<X509V2AttributeCertificate> GetAttributeCertificates()
	{
		PopulateCertCrlSets();
		return CmsSignedHelper.GetAttributeCertificates(_certSet);
	}

	public IStore<X509Certificate> GetCertificates()
	{
		PopulateCertCrlSets();
		return CmsSignedHelper.GetCertificates(_certSet);
	}

	public IStore<X509Crl> GetCrls()
	{
		PopulateCertCrlSets();
		return CmsSignedHelper.GetCrls(_crlSet);
	}

	public IStore<Asn1Encodable> GetOtherRevInfos(DerObjectIdentifier otherRevInfoFormat)
	{
		PopulateCertCrlSets();
		return CmsSignedHelper.GetOtherRevInfos(_crlSet, otherRevInfoFormat);
	}

	private void PopulateCertCrlSets()
	{
		if (_isCertCrlParsed)
		{
			return;
		}
		_isCertCrlParsed = true;
		try
		{
			_certSet = GetAsn1Set(_signedData.GetCertificates());
			_crlSet = GetAsn1Set(_signedData.GetCrls());
		}
		catch (IOException innerException)
		{
			throw new CmsException("problem parsing cert/crl sets", innerException);
		}
	}

	public CmsTypedStream GetSignedContent()
	{
		if (_signedContent == null)
		{
			return null;
		}
		Stream stream = _signedContent.ContentStream;
		foreach (IDigest value in m_digests.Values)
		{
			stream = new DigestStream(stream, value, null);
		}
		return new CmsTypedStream(_signedContent.ContentType, stream);
	}

	public static Stream ReplaceSigners(Stream original, SignerInformationStore signerInformationStore, Stream outStr)
	{
		CmsSignedDataStreamGenerator cmsSignedDataStreamGenerator = new CmsSignedDataStreamGenerator();
		CmsSignedDataParser cmsSignedDataParser = new CmsSignedDataParser(original);
		cmsSignedDataStreamGenerator.AddSigners(signerInformationStore);
		CmsTypedStream signedContent = cmsSignedDataParser.GetSignedContent();
		bool flag = signedContent != null;
		Stream stream = cmsSignedDataStreamGenerator.Open(outStr, cmsSignedDataParser.SignedContentType.Id, flag);
		if (flag)
		{
			Streams.PipeAll(signedContent.ContentStream, stream);
		}
		cmsSignedDataStreamGenerator.AddAttributeCertificates(cmsSignedDataParser.GetAttributeCertificates());
		cmsSignedDataStreamGenerator.AddCertificates(cmsSignedDataParser.GetCertificates());
		cmsSignedDataStreamGenerator.AddCrls(cmsSignedDataParser.GetCrls());
		stream.Dispose();
		return outStr;
	}

	public static Stream ReplaceCertificatesAndCrls(Stream original, IStore<X509Certificate> x509Certs, IStore<X509Crl> x509Crls, IStore<X509V2AttributeCertificate> x509AttrCerts, Stream outStr)
	{
		CmsSignedDataStreamGenerator cmsSignedDataStreamGenerator = new CmsSignedDataStreamGenerator();
		CmsSignedDataParser cmsSignedDataParser = new CmsSignedDataParser(original);
		cmsSignedDataStreamGenerator.AddDigests(cmsSignedDataParser.DigestOids);
		CmsTypedStream signedContent = cmsSignedDataParser.GetSignedContent();
		bool flag = signedContent != null;
		Stream stream = cmsSignedDataStreamGenerator.Open(outStr, cmsSignedDataParser.SignedContentType.Id, flag);
		if (flag)
		{
			Streams.PipeAll(signedContent.ContentStream, stream);
		}
		if (x509AttrCerts != null)
		{
			cmsSignedDataStreamGenerator.AddAttributeCertificates(x509AttrCerts);
		}
		if (x509Certs != null)
		{
			cmsSignedDataStreamGenerator.AddCertificates(x509Certs);
		}
		if (x509Crls != null)
		{
			cmsSignedDataStreamGenerator.AddCrls(x509Crls);
		}
		cmsSignedDataStreamGenerator.AddSigners(cmsSignedDataParser.GetSignerInfos());
		stream.Dispose();
		return outStr;
	}

	private static Asn1Set GetAsn1Set(Asn1SetParser asn1SetParser)
	{
		if (asn1SetParser != null)
		{
			return Asn1Set.GetInstance(asn1SetParser);
		}
		return null;
	}
}
