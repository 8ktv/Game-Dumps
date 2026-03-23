using System;
using System.Collections.Generic;
using System.IO;
using Mirror.BouncyCastle.Asn1;
using Mirror.BouncyCastle.Asn1.Cms;
using Mirror.BouncyCastle.Asn1.X509;
using Mirror.BouncyCastle.Operators.Utilities;
using Mirror.BouncyCastle.Utilities.Collections;
using Mirror.BouncyCastle.X509;

namespace Mirror.BouncyCastle.Cms;

public class CmsSignedData
{
	private readonly CmsProcessable signedContent;

	private SignedData signedData;

	private ContentInfo contentInfo;

	private SignerInformationStore signerInfoStore;

	private IDictionary<string, byte[]> m_hashes;

	public int Version => signedData.Version.IntValueExact;

	public DerObjectIdentifier SignedContentType => signedData.EncapContentInfo.ContentType;

	public CmsProcessable SignedContent => signedContent;

	public ContentInfo ContentInfo => contentInfo;

	private CmsSignedData(CmsSignedData c)
	{
		signedData = c.signedData;
		contentInfo = c.contentInfo;
		signedContent = c.signedContent;
		signerInfoStore = c.signerInfoStore;
	}

	public CmsSignedData(byte[] sigBlock)
		: this(CmsUtilities.ReadContentInfo(new MemoryStream(sigBlock, writable: false)))
	{
	}

	public CmsSignedData(CmsProcessable signedContent, byte[] sigBlock)
		: this(signedContent, CmsUtilities.ReadContentInfo(new MemoryStream(sigBlock, writable: false)))
	{
	}

	public CmsSignedData(IDictionary<string, byte[]> hashes, byte[] sigBlock)
		: this(hashes, CmsUtilities.ReadContentInfo(sigBlock))
	{
	}

	public CmsSignedData(CmsProcessable signedContent, Stream sigData)
		: this(signedContent, CmsUtilities.ReadContentInfo(sigData))
	{
	}

	public CmsSignedData(Stream sigData)
		: this(CmsUtilities.ReadContentInfo(sigData))
	{
	}

	public CmsSignedData(CmsProcessable signedContent, ContentInfo sigData)
	{
		this.signedContent = signedContent;
		contentInfo = sigData;
		signedData = SignedData.GetInstance(contentInfo.Content);
	}

	public CmsSignedData(IDictionary<string, byte[]> hashes, ContentInfo sigData)
	{
		m_hashes = hashes;
		contentInfo = sigData;
		signedData = SignedData.GetInstance(contentInfo.Content);
	}

	public CmsSignedData(ContentInfo sigData)
	{
		contentInfo = sigData;
		signedData = SignedData.GetInstance(contentInfo.Content);
		if (signedData.EncapContentInfo.Content != null)
		{
			if (signedData.EncapContentInfo.Content is Asn1OctetString)
			{
				signedContent = new CmsProcessableByteArray(((Asn1OctetString)signedData.EncapContentInfo.Content).GetOctets());
			}
			else
			{
				signedContent = new Pkcs7ProcessableObject(signedData.EncapContentInfo.ContentType, signedData.EncapContentInfo.Content);
			}
		}
	}

	public SignerInformationStore GetSignerInfos()
	{
		if (signerInfoStore == null)
		{
			List<SignerInformation> list = new List<SignerInformation>();
			foreach (Asn1Encodable signerInfo in signedData.SignerInfos)
			{
				SignerInfo instance = SignerInfo.GetInstance(signerInfo);
				DerObjectIdentifier contentType = signedData.EncapContentInfo.ContentType;
				if (m_hashes == null)
				{
					list.Add(new SignerInformation(instance, contentType, signedContent, null));
					continue;
				}
				if (m_hashes.TryGetValue(instance.DigestAlgorithm.Algorithm.Id, out var value))
				{
					list.Add(new SignerInformation(instance, contentType, null, value));
					continue;
				}
				throw new InvalidOperationException();
			}
			signerInfoStore = new SignerInformationStore(list);
		}
		return signerInfoStore;
	}

	public IStore<X509V2AttributeCertificate> GetAttributeCertificates()
	{
		return CmsSignedHelper.GetAttributeCertificates(signedData.Certificates);
	}

	public IStore<X509Certificate> GetCertificates()
	{
		return CmsSignedHelper.GetCertificates(signedData.Certificates);
	}

	public IStore<X509Crl> GetCrls()
	{
		return CmsSignedHelper.GetCrls(signedData.CRLs);
	}

	public IStore<Asn1Encodable> GetOtherRevInfos(DerObjectIdentifier otherRevInfoFormat)
	{
		return CmsSignedHelper.GetOtherRevInfos(signedData.CRLs, otherRevInfoFormat);
	}

	public ISet<AlgorithmIdentifier> GetDigestAlgorithmIDs()
	{
		Asn1Set digestAlgorithms = signedData.DigestAlgorithms;
		HashSet<AlgorithmIdentifier> hashSet = new HashSet<AlgorithmIdentifier>();
		foreach (Asn1Encodable item in digestAlgorithms)
		{
			hashSet.Add(AlgorithmIdentifier.GetInstance(item));
		}
		return CollectionUtilities.ReadOnly(hashSet);
	}

	public byte[] GetEncoded()
	{
		return contentInfo.GetEncoded();
	}

	public byte[] GetEncoded(string encoding)
	{
		return contentInfo.GetEncoded(encoding);
	}

	public static CmsSignedData AddDigestAlgorithm(CmsSignedData signedData, AlgorithmIdentifier digestAlgorithm)
	{
		return AddDigestAlgorithm(signedData, digestAlgorithm, DefaultDigestAlgorithmFinder.Instance);
	}

	public static CmsSignedData AddDigestAlgorithm(CmsSignedData signedData, AlgorithmIdentifier digestAlgorithm, IDigestAlgorithmFinder digestAlgorithmFinder)
	{
		ISet<AlgorithmIdentifier> digestAlgorithmIDs = signedData.GetDigestAlgorithmIDs();
		AlgorithmIdentifier item = CmsSignedHelper.FixDigestAlgID(digestAlgorithm, digestAlgorithmFinder);
		if (digestAlgorithmIDs.Contains(item))
		{
			return signedData;
		}
		CmsSignedData cmsSignedData = new CmsSignedData(signedData);
		HashSet<AlgorithmIdentifier> hashSet = new HashSet<AlgorithmIdentifier>();
		foreach (AlgorithmIdentifier item2 in hashSet)
		{
			hashSet.Add(CmsSignedHelper.FixDigestAlgID(item2, digestAlgorithmFinder));
		}
		hashSet.Add(item);
		Asn1Set element = CmsUtilities.ConvertToDLSet(hashSet);
		Asn1Sequence asn1Sequence = (Asn1Sequence)signedData.signedData.ToAsn1Object();
		Asn1EncodableVector asn1EncodableVector = new Asn1EncodableVector(asn1Sequence.Count);
		asn1EncodableVector.Add(asn1Sequence[0]);
		asn1EncodableVector.Add(element);
		for (int i = 2; i != asn1Sequence.Count; i++)
		{
			asn1EncodableVector.Add(asn1Sequence[i]);
		}
		cmsSignedData.signedData = SignedData.GetInstance(new BerSequence(asn1EncodableVector));
		cmsSignedData.contentInfo = new ContentInfo(cmsSignedData.contentInfo.ContentType, cmsSignedData.signedData);
		return cmsSignedData;
	}

	public static CmsSignedData ReplaceSigners(CmsSignedData signedData, SignerInformationStore signerInformationStore)
	{
		return ReplaceSigners(signedData, signerInformationStore, DefaultDigestAlgorithmFinder.Instance);
	}

	public static CmsSignedData ReplaceSigners(CmsSignedData signedData, SignerInformationStore signerInformationStore, IDigestAlgorithmFinder digestAlgorithmFinder)
	{
		CmsSignedData cmsSignedData = new CmsSignedData(signedData);
		cmsSignedData.signerInfoStore = signerInformationStore;
		HashSet<AlgorithmIdentifier> digestAlgs = new HashSet<AlgorithmIdentifier>();
		IList<SignerInformation> signers = signerInformationStore.GetSigners();
		Asn1EncodableVector asn1EncodableVector = new Asn1EncodableVector(signers.Count);
		foreach (SignerInformation item in signers)
		{
			CmsUtilities.AddDigestAlgs(digestAlgs, item, digestAlgorithmFinder);
			asn1EncodableVector.Add(item.ToSignerInfo());
		}
		Asn1Set element = CmsUtilities.ConvertToDLSet(digestAlgs);
		Asn1Set element2 = DLSet.FromVector(asn1EncodableVector);
		Asn1Sequence asn1Sequence = (Asn1Sequence)signedData.signedData.ToAsn1Object();
		asn1EncodableVector = new Asn1EncodableVector(asn1Sequence.Count);
		asn1EncodableVector.Add(asn1Sequence[0]);
		asn1EncodableVector.Add(element);
		for (int i = 2; i != asn1Sequence.Count - 1; i++)
		{
			asn1EncodableVector.Add(asn1Sequence[i]);
		}
		asn1EncodableVector.Add(element2);
		cmsSignedData.signedData = SignedData.GetInstance(new BerSequence(asn1EncodableVector));
		cmsSignedData.contentInfo = new ContentInfo(cmsSignedData.contentInfo.ContentType, cmsSignedData.signedData);
		return cmsSignedData;
	}

	public static CmsSignedData ReplaceCertificatesAndCrls(CmsSignedData signedData, IStore<X509Certificate> x509Certs, IStore<X509Crl> x509Crls)
	{
		return ReplaceCertificatesAndRevocations(signedData, x509Certs, x509Crls, null, null);
	}

	public static CmsSignedData ReplaceCertificatesAndCrls(CmsSignedData signedData, IStore<X509Certificate> x509Certs, IStore<X509Crl> x509Crls, IStore<X509V2AttributeCertificate> x509AttrCerts)
	{
		return ReplaceCertificatesAndRevocations(signedData, x509Certs, x509Crls, x509AttrCerts, null);
	}

	public static CmsSignedData ReplaceCertificatesAndRevocations(CmsSignedData signedData, IStore<X509Certificate> x509Certs, IStore<X509Crl> x509Crls, IStore<X509V2AttributeCertificate> x509AttrCerts, IStore<OtherRevocationInfoFormat> otherRevocationInfos)
	{
		CmsSignedData cmsSignedData = new CmsSignedData(signedData);
		Asn1Set certificates = null;
		Asn1Set crls = null;
		if (x509Certs != null || x509AttrCerts != null)
		{
			List<Asn1Encodable> list = new List<Asn1Encodable>();
			if (x509Certs != null)
			{
				list.AddRange(CmsUtilities.GetCertificatesFromStore(x509Certs));
			}
			if (x509AttrCerts != null)
			{
				list.AddRange(CmsUtilities.GetAttributeCertificatesFromStore(x509AttrCerts));
			}
			Asn1Set asn1Set = CmsUtilities.CreateBerSetFromList(list);
			if (asn1Set.Count > 0)
			{
				certificates = asn1Set;
			}
		}
		if (x509Crls != null || otherRevocationInfos != null)
		{
			List<Asn1Encodable> list2 = new List<Asn1Encodable>();
			if (x509Crls != null)
			{
				list2.AddRange(CmsUtilities.GetCrlsFromStore(x509Crls));
			}
			if (otherRevocationInfos != null)
			{
				list2.AddRange(CmsUtilities.GetOtherRevocationInfosFromStore(otherRevocationInfos));
			}
			Asn1Set asn1Set2 = CmsUtilities.CreateBerSetFromList(list2);
			if (asn1Set2.Count > 0)
			{
				crls = asn1Set2;
			}
		}
		SignedData signedData2 = signedData.signedData;
		cmsSignedData.signedData = new SignedData(signedData2.DigestAlgorithms, signedData2.EncapContentInfo, certificates, crls, signedData2.SignerInfos);
		cmsSignedData.contentInfo = new ContentInfo(cmsSignedData.contentInfo.ContentType, cmsSignedData.signedData);
		return cmsSignedData;
	}
}
