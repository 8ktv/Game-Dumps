using System;
using System.Collections.Generic;
using System.IO;
using Mirror.BouncyCastle.Asn1;
using Mirror.BouncyCastle.Asn1.Ocsp;
using Mirror.BouncyCastle.Asn1.X509;
using Mirror.BouncyCastle.Crypto;
using Mirror.BouncyCastle.Security;
using Mirror.BouncyCastle.Utilities.Collections;
using Mirror.BouncyCastle.X509;

namespace Mirror.BouncyCastle.Ocsp;

public class BasicOcspResp : X509ExtensionBase
{
	private readonly BasicOcspResponse resp;

	private readonly ResponseData data;

	public int Version => data.Version.IntValueExact + 1;

	public RespID ResponderId => new RespID(data.ResponderID);

	public DateTime ProducedAt => data.ProducedAt.ToDateTime();

	public SingleResp[] Responses => data.Responses.MapElements((Asn1Encodable element) => new SingleResp(SingleResponse.GetInstance(element)));

	public X509Extensions ResponseExtensions => data.ResponseExtensions;

	public string SignatureAlgName => OcspUtilities.GetAlgorithmName(resp.SignatureAlgorithm.Algorithm);

	public string SignatureAlgOid => resp.SignatureAlgorithm.Algorithm.Id;

	public BasicOcspResp(BasicOcspResponse resp)
	{
		this.resp = resp;
		data = resp.TbsResponseData;
	}

	public byte[] GetTbsResponseData()
	{
		try
		{
			return data.GetDerEncoded();
		}
		catch (IOException innerException)
		{
			throw new OcspException("problem encoding tbsResponseData", innerException);
		}
	}

	protected override X509Extensions GetX509Extensions()
	{
		return ResponseExtensions;
	}

	public byte[] GetSignature()
	{
		return resp.GetSignatureOctets();
	}

	private List<X509Certificate> GetCertList()
	{
		List<X509Certificate> list = new List<X509Certificate>();
		Asn1Sequence certs = resp.Certs;
		if (certs != null)
		{
			foreach (Asn1Encodable item in certs)
			{
				X509CertificateStructure instance = X509CertificateStructure.GetInstance(item);
				if (instance != null)
				{
					list.Add(new X509Certificate(instance));
				}
			}
		}
		return list;
	}

	public X509Certificate[] GetCerts()
	{
		return GetCertList().ToArray();
	}

	public IStore<X509Certificate> GetCertificates()
	{
		return CollectionUtilities.CreateStore(GetCertList());
	}

	public bool Verify(AsymmetricKeyParameter publicKey)
	{
		try
		{
			ISigner signer = SignerUtilities.GetSigner(SignatureAlgName);
			signer.Init(forSigning: false, publicKey);
			byte[] derEncoded = data.GetDerEncoded();
			signer.BlockUpdate(derEncoded, 0, derEncoded.Length);
			return signer.VerifySignature(GetSignature());
		}
		catch (Exception ex)
		{
			throw new OcspException("exception processing sig: " + ex, ex);
		}
	}

	public byte[] GetEncoded()
	{
		return resp.GetEncoded();
	}

	public override bool Equals(object obj)
	{
		if (obj == this)
		{
			return true;
		}
		if (!(obj is BasicOcspResp basicOcspResp))
		{
			return false;
		}
		return resp.Equals(basicOcspResp.resp);
	}

	public override int GetHashCode()
	{
		return resp.GetHashCode();
	}
}
