using System;
using System.Collections.Generic;
using Mirror.BouncyCastle.Asn1;
using Mirror.BouncyCastle.Asn1.Cms;
using Mirror.BouncyCastle.Asn1.CryptoPro;
using Mirror.BouncyCastle.Asn1.Nist;
using Mirror.BouncyCastle.Asn1.Oiw;
using Mirror.BouncyCastle.Asn1.Pkcs;
using Mirror.BouncyCastle.Asn1.Rosstandart;
using Mirror.BouncyCastle.Asn1.TeleTrust;
using Mirror.BouncyCastle.Asn1.X509;
using Mirror.BouncyCastle.Asn1.X9;
using Mirror.BouncyCastle.Crypto;
using Mirror.BouncyCastle.Security;
using Mirror.BouncyCastle.Utilities.Collections;
using Mirror.BouncyCastle.X509;

namespace Mirror.BouncyCastle.Cms;

public abstract class CmsSignedGenerator
{
	public static readonly string Data = CmsObjectIdentifiers.Data.Id;

	public static readonly string DigestSha1 = OiwObjectIdentifiers.IdSha1.Id;

	public static readonly string DigestSha224 = NistObjectIdentifiers.IdSha224.Id;

	public static readonly string DigestSha256 = NistObjectIdentifiers.IdSha256.Id;

	public static readonly string DigestSha384 = NistObjectIdentifiers.IdSha384.Id;

	public static readonly string DigestSha512 = NistObjectIdentifiers.IdSha512.Id;

	public static readonly string DigestSha512_224 = NistObjectIdentifiers.IdSha512_224.Id;

	public static readonly string DigestSha512_256 = NistObjectIdentifiers.IdSha512_256.Id;

	public static readonly string DigestMD5 = PkcsObjectIdentifiers.MD5.Id;

	public static readonly string DigestGost3411 = CryptoProObjectIdentifiers.GostR3411.Id;

	public static readonly string DigestRipeMD128 = TeleTrusTObjectIdentifiers.RipeMD128.Id;

	public static readonly string DigestRipeMD160 = TeleTrusTObjectIdentifiers.RipeMD160.Id;

	public static readonly string DigestRipeMD256 = TeleTrusTObjectIdentifiers.RipeMD256.Id;

	public static readonly string EncryptionRsa = PkcsObjectIdentifiers.RsaEncryption.Id;

	public static readonly string EncryptionDsa = X9ObjectIdentifiers.IdDsaWithSha1.Id;

	public static readonly string EncryptionECDsa = X9ObjectIdentifiers.ECDsaWithSha1.Id;

	public static readonly string EncryptionRsaPss = PkcsObjectIdentifiers.IdRsassaPss.Id;

	public static readonly string EncryptionGost3410 = CryptoProObjectIdentifiers.GostR3410x94.Id;

	public static readonly string EncryptionECGost3410 = CryptoProObjectIdentifiers.GostR3410x2001.Id;

	public static readonly string EncryptionECGost3410_2012_256 = RosstandartObjectIdentifiers.id_tc26_gost_3410_12_256.Id;

	public static readonly string EncryptionECGost3410_2012_512 = RosstandartObjectIdentifiers.id_tc26_gost_3410_12_512.Id;

	internal List<Asn1Encodable> _certs = new List<Asn1Encodable>();

	internal List<Asn1Encodable> _crls = new List<Asn1Encodable>();

	internal IList<SignerInformation> _signers = new List<SignerInformation>();

	internal IDictionary<DerObjectIdentifier, byte[]> m_digests = new Dictionary<DerObjectIdentifier, byte[]>();

	internal bool _useDerForCerts;

	internal bool _useDerForCrls;

	protected readonly SecureRandom m_random;

	public bool UseDerForCerts
	{
		get
		{
			return _useDerForCerts;
		}
		set
		{
			_useDerForCerts = value;
		}
	}

	public bool UseDerForCrls
	{
		get
		{
			return _useDerForCrls;
		}
		set
		{
			_useDerForCrls = value;
		}
	}

	protected CmsSignedGenerator()
		: this(CryptoServicesRegistrar.GetSecureRandom())
	{
	}

	protected CmsSignedGenerator(SecureRandom random)
	{
		if (random == null)
		{
			throw new ArgumentNullException("random");
		}
		m_random = random;
	}

	protected internal virtual IDictionary<CmsAttributeTableParameter, object> GetBaseParameters(DerObjectIdentifier contentType, AlgorithmIdentifier digAlgId, byte[] hash)
	{
		Dictionary<CmsAttributeTableParameter, object> dictionary = new Dictionary<CmsAttributeTableParameter, object>();
		if (contentType != null)
		{
			dictionary[CmsAttributeTableParameter.ContentType] = contentType;
		}
		dictionary[CmsAttributeTableParameter.DigestAlgorithmIdentifier] = digAlgId;
		dictionary[CmsAttributeTableParameter.Digest] = hash.Clone();
		return dictionary;
	}

	protected internal virtual Asn1Set GetAttributeSet(Mirror.BouncyCastle.Asn1.Cms.AttributeTable attr)
	{
		if (attr != null)
		{
			return DerSet.FromVector(attr.ToAsn1EncodableVector());
		}
		return null;
	}

	public void AddAttributeCertificate(X509V2AttributeCertificate attrCert)
	{
		_certs.Add(new DerTaggedObject(isExplicit: false, 2, attrCert.AttributeCertificate));
	}

	public void AddAttributeCertificates(IStore<X509V2AttributeCertificate> attrCertStore)
	{
		_certs.AddRange(CmsUtilities.GetAttributeCertificatesFromStore(attrCertStore));
	}

	public void AddCertificate(X509Certificate cert)
	{
		_certs.Add(cert.CertificateStructure);
	}

	public void AddCertificates(IStore<X509Certificate> certStore)
	{
		_certs.AddRange(CmsUtilities.GetCertificatesFromStore(certStore));
	}

	public void AddCrl(X509Crl crl)
	{
		_crls.Add(crl.CertificateList);
	}

	public void AddCrls(IStore<X509Crl> crlStore)
	{
		_crls.AddRange(CmsUtilities.GetCrlsFromStore(crlStore));
	}

	public void AddOtherRevocationInfo(OtherRevocationInfoFormat otherRevocationInfo)
	{
		CmsUtilities.ValidateOtherRevocationInfo(otherRevocationInfo);
		_crls.Add(new DerTaggedObject(isExplicit: false, 1, otherRevocationInfo));
	}

	public void AddOtherRevocationInfos(IStore<OtherRevocationInfoFormat> otherRevocationInfoStore)
	{
		_crls.AddRange(CmsUtilities.GetOtherRevocationInfosFromStore(otherRevocationInfoStore));
	}

	public void AddOtherRevocationInfos(DerObjectIdentifier otherRevInfoFormat, IStore<Asn1Encodable> otherRevInfoStore)
	{
		_crls.AddRange(CmsUtilities.GetOtherRevocationInfosFromStore(otherRevInfoStore, otherRevInfoFormat));
	}

	public void AddSigners(SignerInformationStore signerStore)
	{
		foreach (SignerInformation signer in signerStore.GetSigners())
		{
			_signers.Add(signer);
			AddSignerCallback(signer);
		}
	}

	public IDictionary<string, byte[]> GetGeneratedDigests()
	{
		Dictionary<string, byte[]> dictionary = new Dictionary<string, byte[]>(StringComparer.OrdinalIgnoreCase);
		foreach (KeyValuePair<DerObjectIdentifier, byte[]> digest in m_digests)
		{
			dictionary.Add(digest.Key.GetID(), digest.Value);
		}
		return dictionary;
	}

	internal virtual void AddSignerCallback(SignerInformation si)
	{
	}

	internal static SignerIdentifier GetSignerIdentifier(X509Certificate cert)
	{
		return new SignerIdentifier(CmsUtilities.GetIssuerAndSerialNumber(cert));
	}

	internal static SignerIdentifier GetSignerIdentifier(byte[] subjectKeyIdentifier)
	{
		return new SignerIdentifier(new DerOctetString(subjectKeyIdentifier));
	}
}
