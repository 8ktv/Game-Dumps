using System;
using System.Collections.Generic;
using System.IO;
using Mirror.BouncyCastle.Asn1;
using Mirror.BouncyCastle.Asn1.Cms;
using Mirror.BouncyCastle.Asn1.Ess;
using Mirror.BouncyCastle.Asn1.Nist;
using Mirror.BouncyCastle.Asn1.Oiw;
using Mirror.BouncyCastle.Asn1.Pkcs;
using Mirror.BouncyCastle.Asn1.Tsp;
using Mirror.BouncyCastle.Asn1.X509;
using Mirror.BouncyCastle.Cms;
using Mirror.BouncyCastle.Security;
using Mirror.BouncyCastle.Security.Certificates;
using Mirror.BouncyCastle.Utilities;
using Mirror.BouncyCastle.Utilities.Collections;
using Mirror.BouncyCastle.X509;

namespace Mirror.BouncyCastle.Tsp;

public class TimeStampToken
{
	private class CertID
	{
		private EssCertID certID;

		private EssCertIDv2 certIDv2;

		public IssuerSerial IssuerSerial
		{
			get
			{
				if (certID == null)
				{
					return certIDv2.IssuerSerial;
				}
				return certID.IssuerSerial;
			}
		}

		internal CertID(EssCertID certID)
		{
			this.certID = certID;
			certIDv2 = null;
		}

		internal CertID(EssCertIDv2 certID)
		{
			certIDv2 = certID;
			this.certID = null;
		}

		public string GetHashAlgorithmName()
		{
			if (certID != null)
			{
				return "SHA-1";
			}
			if (NistObjectIdentifiers.IdSha256.Equals(certIDv2.HashAlgorithm.Algorithm))
			{
				return "SHA-256";
			}
			return certIDv2.HashAlgorithm.Algorithm.Id;
		}

		public AlgorithmIdentifier GetHashAlgorithm()
		{
			if (certID == null)
			{
				return certIDv2.HashAlgorithm;
			}
			return new AlgorithmIdentifier(OiwObjectIdentifiers.IdSha1);
		}

		public byte[] GetCertHash()
		{
			if (certID == null)
			{
				return certIDv2.GetCertHash();
			}
			return certID.GetCertHash();
		}
	}

	private readonly CmsSignedData tsToken;

	private readonly SignerInformation tsaSignerInfo;

	private readonly TimeStampTokenInfo tstInfo;

	private readonly CertID certID;

	public TimeStampTokenInfo TimeStampInfo => tstInfo;

	public SignerID SignerID => tsaSignerInfo.SignerID;

	public Mirror.BouncyCastle.Asn1.Cms.AttributeTable SignedAttributes => tsaSignerInfo.SignedAttributes;

	public Mirror.BouncyCastle.Asn1.Cms.AttributeTable UnsignedAttributes => tsaSignerInfo.UnsignedAttributes;

	public TimeStampToken(Mirror.BouncyCastle.Asn1.Cms.ContentInfo contentInfo)
		: this(new CmsSignedData(contentInfo))
	{
	}

	public TimeStampToken(CmsSignedData signedData)
	{
		tsToken = signedData;
		if (!tsToken.SignedContentType.Equals(PkcsObjectIdentifiers.IdCTTstInfo))
		{
			throw new TspValidationException("ContentInfo object not for a time stamp.");
		}
		IList<SignerInformation> signers = tsToken.GetSignerInfos().GetSigners();
		if (signers.Count != 1)
		{
			throw new ArgumentException("Time-stamp token signed by " + signers.Count + " signers, but it must contain just the TSA signature.");
		}
		IEnumerator<SignerInformation> enumerator = signers.GetEnumerator();
		enumerator.MoveNext();
		tsaSignerInfo = enumerator.Current;
		try
		{
			CmsProcessable signedContent = tsToken.SignedContent;
			MemoryStream memoryStream = new MemoryStream();
			signedContent.Write(memoryStream);
			tstInfo = new TimeStampTokenInfo(TstInfo.GetInstance(Asn1Object.FromByteArray(memoryStream.ToArray())));
			Mirror.BouncyCastle.Asn1.Cms.Attribute attribute = tsaSignerInfo.SignedAttributes[PkcsObjectIdentifiers.IdAASigningCertificate];
			if (attribute != null)
			{
				if (attribute.AttrValues[0] is SigningCertificateV2)
				{
					SigningCertificateV2 instance = SigningCertificateV2.GetInstance(attribute.AttrValues[0]);
					certID = new CertID(EssCertIDv2.GetInstance(instance.GetCerts()[0]));
				}
				else
				{
					SigningCertificate instance2 = SigningCertificate.GetInstance(attribute.AttrValues[0]);
					certID = new CertID(EssCertID.GetInstance(instance2.GetCerts()[0]));
				}
				return;
			}
			attribute = tsaSignerInfo.SignedAttributes[PkcsObjectIdentifiers.IdAASigningCertificateV2];
			if (attribute == null)
			{
				throw new TspValidationException("no signing certificate attribute found, time stamp invalid.");
			}
			SigningCertificateV2 instance3 = SigningCertificateV2.GetInstance(attribute.AttrValues[0]);
			certID = new CertID(EssCertIDv2.GetInstance(instance3.GetCerts()[0]));
		}
		catch (CmsException ex)
		{
			throw new TspException(ex.Message, ex.InnerException);
		}
	}

	public IStore<X509V2AttributeCertificate> GetAttributeCertificates()
	{
		return tsToken.GetAttributeCertificates();
	}

	public IStore<X509Certificate> GetCertificates()
	{
		return tsToken.GetCertificates();
	}

	public IStore<X509Crl> GetCrls()
	{
		return tsToken.GetCrls();
	}

	public void Validate(X509Certificate cert)
	{
		try
		{
			byte[] b = DigestUtilities.CalculateDigest(certID.GetHashAlgorithmName(), cert.GetEncoded());
			if (!Arrays.FixedTimeEquals(certID.GetCertHash(), b))
			{
				throw new TspValidationException("certificate hash does not match certID hash.");
			}
			IssuerSerial issuerSerial = certID.IssuerSerial;
			if (issuerSerial != null)
			{
				if (!issuerSerial.Serial.HasValue(cert.SerialNumber))
				{
					throw new TspValidationException("certificate serial number does not match certID for signature.");
				}
				GeneralName[] names = certID.IssuerSerial.Issuer.GetNames();
				X509Name issuerDN = cert.IssuerDN;
				bool flag = false;
				for (int i = 0; i != names.Length; i++)
				{
					if (names[i].TagNo == 4 && X509Name.GetInstance(names[i].Name).Equivalent(issuerDN))
					{
						flag = true;
						break;
					}
				}
				if (!flag)
				{
					throw new TspValidationException("certificate name does not match certID for signature. ");
				}
			}
			TspUtil.ValidateCertificate(cert);
			cert.CheckValidity(tstInfo.GenTime);
			if (!tsaSignerInfo.Verify(cert))
			{
				throw new TspValidationException("signature not created by certificate.");
			}
		}
		catch (CmsException ex)
		{
			if (ex.InnerException != null)
			{
				throw new TspException(ex.Message, ex.InnerException);
			}
			throw new TspException("CMS exception: " + ex, ex);
		}
		catch (CertificateEncodingException ex2)
		{
			throw new TspException("problem processing certificate: " + ex2, ex2);
		}
		catch (SecurityUtilityException ex3)
		{
			throw new TspException("cannot find algorithm: " + ex3.Message, ex3);
		}
	}

	public CmsSignedData ToCmsSignedData()
	{
		return tsToken;
	}

	public byte[] GetEncoded()
	{
		return tsToken.GetEncoded("DL");
	}

	public byte[] GetEncoded(string encoding)
	{
		return tsToken.GetEncoded(encoding);
	}
}
