using System;
using System.Collections.Generic;
using System.IO;
using Mirror.BouncyCastle.Asn1;
using Mirror.BouncyCastle.Asn1.Ocsp;
using Mirror.BouncyCastle.Asn1.X509;
using Mirror.BouncyCastle.Crypto;
using Mirror.BouncyCastle.Crypto.Operators;
using Mirror.BouncyCastle.Security;
using Mirror.BouncyCastle.Security.Certificates;
using Mirror.BouncyCastle.X509;

namespace Mirror.BouncyCastle.Ocsp;

public class BasicOcspRespGenerator
{
	private class ResponseObject
	{
		internal CertificateID certId;

		internal CertStatus certStatus;

		internal DerGeneralizedTime thisUpdate;

		internal DerGeneralizedTime nextUpdate;

		internal X509Extensions extensions;

		internal ResponseObject(CertificateID certId, CertificateStatus certStatus, DateTime thisUpdate, DateTime? nextUpdate, X509Extensions extensions)
		{
			this.certId = certId;
			if (certStatus == null)
			{
				this.certStatus = new CertStatus();
			}
			else if (certStatus is UnknownStatus)
			{
				this.certStatus = new CertStatus(2, DerNull.Instance);
			}
			else
			{
				RevokedStatus revokedStatus = (RevokedStatus)certStatus;
				CrlReason revocationReason = (revokedStatus.HasRevocationReason ? new CrlReason(revokedStatus.RevocationReason) : null);
				RevokedInfo info = new RevokedInfo(Rfc5280Asn1Utilities.CreateGeneralizedTime(revokedStatus.RevocationTime), revocationReason);
				this.certStatus = new CertStatus(info);
			}
			this.thisUpdate = Rfc5280Asn1Utilities.CreateGeneralizedTime(thisUpdate);
			this.nextUpdate = (nextUpdate.HasValue ? Rfc5280Asn1Utilities.CreateGeneralizedTime(nextUpdate.Value) : null);
			this.extensions = extensions;
		}

		public SingleResponse ToResponse()
		{
			return new SingleResponse(certId.ToAsn1Object(), certStatus, thisUpdate, nextUpdate, extensions);
		}
	}

	private readonly List<ResponseObject> list = new List<ResponseObject>();

	private X509Extensions responseExtensions;

	private RespID responderID;

	public IEnumerable<string> SignatureAlgNames => OcspUtilities.AlgNames;

	public BasicOcspRespGenerator(RespID responderID)
	{
		this.responderID = responderID;
	}

	public BasicOcspRespGenerator(AsymmetricKeyParameter publicKey)
	{
		responderID = new RespID(publicKey);
	}

	public void AddResponse(CertificateID certID, CertificateStatus certStatus)
	{
		list.Add(new ResponseObject(certID, certStatus, DateTime.UtcNow, null, null));
	}

	public void AddResponse(CertificateID certID, CertificateStatus certStatus, X509Extensions singleExtensions)
	{
		list.Add(new ResponseObject(certID, certStatus, DateTime.UtcNow, null, singleExtensions));
	}

	public void AddResponse(CertificateID certID, CertificateStatus certStatus, DateTime? nextUpdate, X509Extensions singleExtensions)
	{
		list.Add(new ResponseObject(certID, certStatus, DateTime.UtcNow, nextUpdate, singleExtensions));
	}

	public void AddResponse(CertificateID certID, CertificateStatus certStatus, DateTime thisUpdate, DateTime? nextUpdate, X509Extensions singleExtensions)
	{
		list.Add(new ResponseObject(certID, certStatus, thisUpdate, nextUpdate, singleExtensions));
	}

	public void SetResponseExtensions(X509Extensions responseExtensions)
	{
		this.responseExtensions = responseExtensions;
	}

	private BasicOcspResp GenerateResponse(ISignatureFactory signatureFactory, X509Certificate[] chain, DateTime producedAt)
	{
		DerObjectIdentifier algorithm = ((AlgorithmIdentifier)signatureFactory.AlgorithmDetails).Algorithm;
		Asn1EncodableVector asn1EncodableVector = new Asn1EncodableVector();
		foreach (ResponseObject item in list)
		{
			try
			{
				asn1EncodableVector.Add(item.ToResponse());
			}
			catch (Exception innerException)
			{
				throw new OcspException("exception creating Request", innerException);
			}
		}
		ResponseData responseData = new ResponseData(responderID.ToAsn1Object(), Rfc5280Asn1Utilities.CreateGeneralizedTime(producedAt), new DerSequence(asn1EncodableVector), responseExtensions);
		DerBitString signature;
		try
		{
			signature = Mirror.BouncyCastle.X509.X509Utilities.GenerateSignature(signatureFactory, responseData);
		}
		catch (Exception ex)
		{
			throw new OcspException("exception processing TBSRequest: " + ex, ex);
		}
		AlgorithmIdentifier sigAlgID = OcspUtilities.GetSigAlgID(algorithm);
		DerSequence certs = null;
		if (chain != null && chain.Length != 0)
		{
			Asn1EncodableVector asn1EncodableVector2 = new Asn1EncodableVector(chain.Length);
			try
			{
				for (int i = 0; i != chain.Length; i++)
				{
					asn1EncodableVector2.Add(chain[i].CertificateStructure);
				}
			}
			catch (IOException innerException2)
			{
				throw new OcspException("error processing certs", innerException2);
			}
			catch (CertificateEncodingException innerException3)
			{
				throw new OcspException("error encoding certs", innerException3);
			}
			certs = new DerSequence(asn1EncodableVector2);
		}
		return new BasicOcspResp(new BasicOcspResponse(responseData, sigAlgID, signature, certs));
	}

	public BasicOcspResp Generate(string signingAlgorithm, AsymmetricKeyParameter privateKey, X509Certificate[] chain, DateTime thisUpdate)
	{
		return Generate(signingAlgorithm, privateKey, chain, thisUpdate, null);
	}

	public BasicOcspResp Generate(string signingAlgorithm, AsymmetricKeyParameter privateKey, X509Certificate[] chain, DateTime producedAt, SecureRandom random)
	{
		if (signingAlgorithm == null)
		{
			throw new ArgumentException("no signing algorithm specified");
		}
		return GenerateResponse(new Asn1SignatureFactory(signingAlgorithm, privateKey, random), chain, producedAt);
	}

	public BasicOcspResp Generate(ISignatureFactory signatureCalculatorFactory, X509Certificate[] chain, DateTime producedAt)
	{
		if (signatureCalculatorFactory == null)
		{
			throw new ArgumentException("no signature calculator specified");
		}
		return GenerateResponse(signatureCalculatorFactory, chain, producedAt);
	}
}
