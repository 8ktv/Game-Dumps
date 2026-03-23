using System;
using System.Collections.Generic;
using System.IO;
using Mirror.BouncyCastle.Asn1;
using Mirror.BouncyCastle.Asn1.Cms;
using Mirror.BouncyCastle.Asn1.Ess;
using Mirror.BouncyCastle.Asn1.Oiw;
using Mirror.BouncyCastle.Asn1.Pkcs;
using Mirror.BouncyCastle.Asn1.Tsp;
using Mirror.BouncyCastle.Asn1.X509;
using Mirror.BouncyCastle.Cms;
using Mirror.BouncyCastle.Crypto;
using Mirror.BouncyCastle.Crypto.Operators;
using Mirror.BouncyCastle.Math;
using Mirror.BouncyCastle.Utilities.Collections;
using Mirror.BouncyCastle.Utilities.Date;
using Mirror.BouncyCastle.X509;

namespace Mirror.BouncyCastle.Tsp;

public class TimeStampTokenGenerator
{
	private class TableGen : CmsAttributeTableGenerator
	{
		private readonly SignerInfoGenerator infoGen;

		private readonly EssCertID essCertID;

		public TableGen(SignerInfoGenerator infoGen, EssCertID essCertID)
		{
			this.infoGen = infoGen;
			this.essCertID = essCertID;
		}

		public Mirror.BouncyCastle.Asn1.Cms.AttributeTable GetAttributes(IDictionary<CmsAttributeTableParameter, object> parameters)
		{
			Mirror.BouncyCastle.Asn1.Cms.AttributeTable attributes = infoGen.signedGen.GetAttributes(parameters);
			if (attributes[PkcsObjectIdentifiers.IdAASigningCertificate] == null)
			{
				return attributes.Add(PkcsObjectIdentifiers.IdAASigningCertificate, new SigningCertificate(essCertID));
			}
			return attributes;
		}
	}

	private class TableGen2 : CmsAttributeTableGenerator
	{
		private readonly SignerInfoGenerator infoGen;

		private readonly EssCertIDv2 essCertID;

		public TableGen2(SignerInfoGenerator infoGen, EssCertIDv2 essCertID)
		{
			this.infoGen = infoGen;
			this.essCertID = essCertID;
		}

		public Mirror.BouncyCastle.Asn1.Cms.AttributeTable GetAttributes(IDictionary<CmsAttributeTableParameter, object> parameters)
		{
			Mirror.BouncyCastle.Asn1.Cms.AttributeTable attributes = infoGen.signedGen.GetAttributes(parameters);
			if (attributes[PkcsObjectIdentifiers.IdAASigningCertificateV2] == null)
			{
				return attributes.Add(PkcsObjectIdentifiers.IdAASigningCertificateV2, new SigningCertificateV2(essCertID));
			}
			return attributes;
		}
	}

	private int accuracySeconds = -1;

	private int accuracyMillis = -1;

	private int accuracyMicros = -1;

	private bool ordering;

	private GeneralName tsa;

	private DerObjectIdentifier tsaPolicyOID;

	private IStore<X509Certificate> x509Certs;

	private IStore<X509Crl> x509Crls;

	private IStore<X509V2AttributeCertificate> x509AttrCerts;

	private SignerInfoGenerator signerInfoGenerator;

	private IDigestFactory digestCalculator;

	private Resolution resolution;

	public Resolution Resolution
	{
		get
		{
			return resolution;
		}
		set
		{
			resolution = value;
		}
	}

	public TimeStampTokenGenerator(AsymmetricKeyParameter key, X509Certificate cert, string digestOID, string tsaPolicyOID)
		: this(key, cert, digestOID, tsaPolicyOID, null, null)
	{
	}

	public TimeStampTokenGenerator(SignerInfoGenerator signerInfoGen, IDigestFactory digestCalculator, DerObjectIdentifier tsaPolicy, bool isIssuerSerialIncluded)
	{
		signerInfoGenerator = signerInfoGen;
		this.digestCalculator = digestCalculator;
		tsaPolicyOID = tsaPolicy;
		if (signerInfoGenerator.certificate == null)
		{
			throw new ArgumentException("SignerInfoGenerator must have an associated certificate");
		}
		X509Certificate certificate = signerInfoGenerator.certificate;
		TspUtil.ValidateCertificate(certificate);
		try
		{
			byte[] encoded = certificate.GetEncoded();
			IStreamCalculator<IBlockResult> streamCalculator = digestCalculator.CreateCalculator();
			using (Stream stream = streamCalculator.Stream)
			{
				stream.Write(encoded, 0, encoded.Length);
			}
			if (((AlgorithmIdentifier)digestCalculator.AlgorithmDetails).Algorithm.Equals(OiwObjectIdentifiers.IdSha1))
			{
				EssCertID essCertID = new EssCertID(streamCalculator.GetResult().Collect(), isIssuerSerialIncluded ? new IssuerSerial(new GeneralNames(new GeneralName(certificate.IssuerDN)), new DerInteger(certificate.SerialNumber)) : null);
				signerInfoGenerator = signerInfoGen.NewBuilder().WithSignedAttributeGenerator(new TableGen(signerInfoGen, essCertID)).Build(signerInfoGen.contentSigner, signerInfoGen.certificate);
			}
			else
			{
				new AlgorithmIdentifier(((AlgorithmIdentifier)digestCalculator.AlgorithmDetails).Algorithm);
				EssCertIDv2 essCertID2 = new EssCertIDv2(streamCalculator.GetResult().Collect(), isIssuerSerialIncluded ? new IssuerSerial(new GeneralNames(new GeneralName(certificate.IssuerDN)), new DerInteger(certificate.SerialNumber)) : null);
				signerInfoGenerator = signerInfoGen.NewBuilder().WithSignedAttributeGenerator(new TableGen2(signerInfoGen, essCertID2)).Build(signerInfoGen.contentSigner, signerInfoGen.certificate);
			}
		}
		catch (Exception innerException)
		{
			throw new TspException("Exception processing certificate", innerException);
		}
	}

	public TimeStampTokenGenerator(AsymmetricKeyParameter key, X509Certificate cert, string digestOID, string tsaPolicyOID, Mirror.BouncyCastle.Asn1.Cms.AttributeTable signedAttr, Mirror.BouncyCastle.Asn1.Cms.AttributeTable unsignedAttr)
		: this(MakeInfoGenerator(key, cert, new DerObjectIdentifier(digestOID), signedAttr, unsignedAttr), Asn1DigestFactory.Get(OiwObjectIdentifiers.IdSha1), (tsaPolicyOID != null) ? new DerObjectIdentifier(tsaPolicyOID) : null, isIssuerSerialIncluded: false)
	{
	}

	internal static SignerInfoGenerator MakeInfoGenerator(AsymmetricKeyParameter key, X509Certificate cert, DerObjectIdentifier digestOid, Mirror.BouncyCastle.Asn1.Cms.AttributeTable signedAttr, Mirror.BouncyCastle.Asn1.Cms.AttributeTable unsignedAttr)
	{
		TspUtil.ValidateCertificate(cert);
		IDictionary<DerObjectIdentifier, object> attrs = ((signedAttr == null) ? new Dictionary<DerObjectIdentifier, object>() : signedAttr.ToDictionary());
		string digestAlgName = CmsSignedHelper.GetDigestAlgName(digestOid);
		DerObjectIdentifier encOid = CmsSignedHelper.GetEncOid(key, digestOid.Id);
		Asn1SignatureFactory contentSigner = new Asn1SignatureFactory(digestAlgName + "with" + CmsSignedHelper.GetEncryptionAlgName(encOid), key);
		return new SignerInfoGeneratorBuilder().WithSignedAttributeGenerator(new DefaultSignedAttributeTableGenerator(new Mirror.BouncyCastle.Asn1.Cms.AttributeTable(attrs))).WithUnsignedAttributeGenerator(new SimpleAttributeTableGenerator(unsignedAttr)).Build(contentSigner, cert);
	}

	public void SetAttributeCertificates(IStore<X509V2AttributeCertificate> attributeCertificates)
	{
		x509AttrCerts = attributeCertificates;
	}

	public void SetCertificates(IStore<X509Certificate> certificates)
	{
		x509Certs = certificates;
	}

	public void SetCrls(IStore<X509Crl> crls)
	{
		x509Crls = crls;
	}

	public void SetAccuracySeconds(int accuracySeconds)
	{
		this.accuracySeconds = accuracySeconds;
	}

	public void SetAccuracyMillis(int accuracyMillis)
	{
		this.accuracyMillis = accuracyMillis;
	}

	public void SetAccuracyMicros(int accuracyMicros)
	{
		this.accuracyMicros = accuracyMicros;
	}

	public void SetOrdering(bool ordering)
	{
		this.ordering = ordering;
	}

	public void SetTsa(GeneralName tsa)
	{
		this.tsa = tsa;
	}

	public TimeStampToken Generate(TimeStampRequest request, BigInteger serialNumber, DateTime genTime)
	{
		return Generate(request, serialNumber, genTime, null);
	}

	public TimeStampToken Generate(TimeStampRequest request, BigInteger serialNumber, DateTime genTime, X509Extensions additionalExtensions)
	{
		MessageImprint messageImprint = new MessageImprint(new AlgorithmIdentifier(new DerObjectIdentifier(request.MessageImprintAlgOid), DerNull.Instance), request.GetMessageImprintDigest());
		Accuracy accuracy = null;
		if (accuracySeconds > 0 || accuracyMillis > 0 || accuracyMicros > 0)
		{
			DerInteger seconds = null;
			if (accuracySeconds > 0)
			{
				seconds = new DerInteger(accuracySeconds);
			}
			DerInteger millis = null;
			if (accuracyMillis > 0)
			{
				millis = new DerInteger(accuracyMillis);
			}
			DerInteger micros = null;
			if (accuracyMicros > 0)
			{
				micros = new DerInteger(accuracyMicros);
			}
			accuracy = new Accuracy(seconds, millis, micros);
		}
		DerBoolean derBoolean = null;
		if (ordering)
		{
			derBoolean = DerBoolean.GetInstance(ordering);
		}
		DerInteger nonce = null;
		if (request.Nonce != null)
		{
			nonce = new DerInteger(request.Nonce);
		}
		DerObjectIdentifier derObjectIdentifier = tsaPolicyOID;
		if (request.ReqPolicy != null)
		{
			derObjectIdentifier = new DerObjectIdentifier(request.ReqPolicy);
		}
		if (derObjectIdentifier == null)
		{
			throw new TspValidationException("request contains no policy", 256);
		}
		X509Extensions x509Extensions = request.Extensions;
		if (additionalExtensions != null)
		{
			X509ExtensionsGenerator x509ExtensionsGenerator = new X509ExtensionsGenerator();
			if (x509Extensions != null)
			{
				foreach (DerObjectIdentifier extensionOid in x509Extensions.ExtensionOids)
				{
					DerObjectIdentifier instance = DerObjectIdentifier.GetInstance(extensionOid);
					x509ExtensionsGenerator.AddExtension(instance, x509Extensions.GetExtension(DerObjectIdentifier.GetInstance(instance)));
				}
			}
			foreach (DerObjectIdentifier extensionOid2 in additionalExtensions.ExtensionOids)
			{
				DerObjectIdentifier instance2 = DerObjectIdentifier.GetInstance(extensionOid2);
				x509ExtensionsGenerator.AddExtension(instance2, additionalExtensions.GetExtension(DerObjectIdentifier.GetInstance(instance2)));
			}
			x509Extensions = x509ExtensionsGenerator.Generate();
		}
		DerGeneralizedTime genTime2 = new DerGeneralizedTime(WithResolution(genTime, resolution));
		TstInfo tstInfo = new TstInfo(derObjectIdentifier, messageImprint, new DerInteger(serialNumber), genTime2, accuracy, derBoolean, nonce, tsa, x509Extensions);
		try
		{
			CmsSignedDataGenerator cmsSignedDataGenerator = new CmsSignedDataGenerator();
			byte[] derEncoded = tstInfo.GetDerEncoded();
			if (request.CertReq)
			{
				cmsSignedDataGenerator.AddCertificates(x509Certs);
				cmsSignedDataGenerator.AddAttributeCertificates(x509AttrCerts);
			}
			cmsSignedDataGenerator.AddCrls(x509Crls);
			cmsSignedDataGenerator.AddSignerInfoGenerator(signerInfoGenerator);
			return new TimeStampToken(cmsSignedDataGenerator.Generate(PkcsObjectIdentifiers.IdCTTstInfo.Id, new CmsProcessableByteArray(derEncoded), encapsulate: true));
		}
		catch (CmsException innerException)
		{
			throw new TspException("Error generating time-stamp token", innerException);
		}
		catch (IOException innerException2)
		{
			throw new TspException("Exception encoding info", innerException2);
		}
	}

	private static DateTime WithResolution(DateTime dateTime, Resolution resolution)
	{
		return resolution switch
		{
			Resolution.R_SECONDS => DateTimeUtilities.WithPrecisionSecond(dateTime), 
			Resolution.R_TENTHS_OF_SECONDS => DateTimeUtilities.WithPrecisionDecisecond(dateTime), 
			Resolution.R_HUNDREDTHS_OF_SECONDS => DateTimeUtilities.WithPrecisionCentisecond(dateTime), 
			Resolution.R_MILLISECONDS => DateTimeUtilities.WithPrecisionMillisecond(dateTime), 
			_ => throw new InvalidOperationException(), 
		};
	}
}
