using System;
using System.Collections.Generic;
using System.IO;
using Mirror.BouncyCastle.Asn1;
using Mirror.BouncyCastle.Asn1.Ocsp;
using Mirror.BouncyCastle.Asn1.X509;
using Mirror.BouncyCastle.Crypto;
using Mirror.BouncyCastle.Crypto.IO;
using Mirror.BouncyCastle.Security;
using Mirror.BouncyCastle.Security.Certificates;
using Mirror.BouncyCastle.Utilities;
using Mirror.BouncyCastle.X509;

namespace Mirror.BouncyCastle.Ocsp;

public class OcspReqGenerator
{
	private class RequestObject
	{
		internal CertificateID certId;

		internal X509Extensions extensions;

		public RequestObject(CertificateID certId, X509Extensions extensions)
		{
			this.certId = certId;
			this.extensions = extensions;
		}

		public Request ToRequest()
		{
			return new Request(certId.ToAsn1Object(), extensions);
		}
	}

	private List<RequestObject> list = new List<RequestObject>();

	private GeneralName requestorName;

	private X509Extensions requestExtensions;

	public IEnumerable<string> SignatureAlgNames => OcspUtilities.AlgNames;

	public void AddRequest(CertificateID certId)
	{
		list.Add(new RequestObject(certId, null));
	}

	public void AddRequest(CertificateID certId, X509Extensions singleRequestExtensions)
	{
		list.Add(new RequestObject(certId, singleRequestExtensions));
	}

	public void SetRequestorName(X509Name requestorName)
	{
		try
		{
			this.requestorName = new GeneralName(4, requestorName);
		}
		catch (Exception innerException)
		{
			throw new ArgumentException("cannot encode principal", innerException);
		}
	}

	public void SetRequestorName(GeneralName requestorName)
	{
		this.requestorName = requestorName;
	}

	public void SetRequestExtensions(X509Extensions requestExtensions)
	{
		this.requestExtensions = requestExtensions;
	}

	private OcspReq GenerateRequest(DerObjectIdentifier signingAlgorithm, AsymmetricKeyParameter privateKey, X509Certificate[] chain, SecureRandom random)
	{
		Asn1EncodableVector asn1EncodableVector = new Asn1EncodableVector(list.Count);
		foreach (RequestObject item in list)
		{
			try
			{
				asn1EncodableVector.Add(item.ToRequest());
			}
			catch (Exception innerException)
			{
				throw new OcspException("exception creating Request", innerException);
			}
		}
		TbsRequest tbsRequest = new TbsRequest(requestorName, new DerSequence(asn1EncodableVector), requestExtensions);
		Signature optionalSignature = null;
		if (signingAlgorithm != null)
		{
			if (requestorName == null)
			{
				throw new OcspException("requestorName must be specified if request is signed.");
			}
			ISigner signer;
			try
			{
				signer = SignerUtilities.InitSigner(signingAlgorithm, forSigning: true, privateKey, random);
			}
			catch (Exception ex)
			{
				throw new OcspException("exception creating signature: " + ex, ex);
			}
			DerBitString signatureValue;
			try
			{
				tbsRequest.EncodeTo(new SignerSink(signer), "DER");
				signatureValue = new DerBitString(signer.GenerateSignature());
			}
			catch (Exception ex2)
			{
				throw new OcspException("exception processing TBSRequest: " + ex2, ex2);
			}
			AlgorithmIdentifier signatureAlgorithm = new AlgorithmIdentifier(signingAlgorithm, DerNull.Instance);
			Asn1Sequence certs = null;
			if (!Arrays.IsNullOrEmpty(chain))
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
			optionalSignature = new Signature(signatureAlgorithm, signatureValue, certs);
		}
		return new OcspReq(new OcspRequest(tbsRequest, optionalSignature));
	}

	public OcspReq Generate()
	{
		return GenerateRequest(null, null, null, null);
	}

	public OcspReq Generate(string signingAlgorithm, AsymmetricKeyParameter privateKey, X509Certificate[] chain)
	{
		return Generate(signingAlgorithm, privateKey, chain, null);
	}

	public OcspReq Generate(string signingAlgorithm, AsymmetricKeyParameter privateKey, X509Certificate[] chain, SecureRandom random)
	{
		if (signingAlgorithm == null)
		{
			throw new ArgumentException("no signing algorithm specified");
		}
		try
		{
			DerObjectIdentifier algorithmOid = OcspUtilities.GetAlgorithmOid(signingAlgorithm);
			return GenerateRequest(algorithmOid, privateKey, chain, random);
		}
		catch (ArgumentException)
		{
			throw new ArgumentException("unknown signing algorithm specified: " + signingAlgorithm);
		}
	}
}
