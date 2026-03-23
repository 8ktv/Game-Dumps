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

public class OcspReq : X509ExtensionBase
{
	private OcspRequest req;

	public int Version => req.TbsRequest.Version.IntValueExact + 1;

	public GeneralName RequestorName => GeneralName.GetInstance(req.TbsRequest.RequestorName);

	public X509Extensions RequestExtensions => X509Extensions.GetInstance(req.TbsRequest.RequestExtensions);

	public string SignatureAlgOid
	{
		get
		{
			if (!IsSigned)
			{
				return null;
			}
			return req.OptionalSignature.SignatureAlgorithm.Algorithm.Id;
		}
	}

	public bool IsSigned => req.OptionalSignature != null;

	public OcspReq(OcspRequest req)
	{
		this.req = req;
	}

	public OcspReq(byte[] req)
		: this(new Asn1InputStream(req))
	{
	}

	public OcspReq(Stream inStr)
		: this(new Asn1InputStream(inStr))
	{
	}

	private OcspReq(Asn1InputStream aIn)
	{
		try
		{
			req = OcspRequest.GetInstance(aIn.ReadObject());
		}
		catch (ArgumentException ex)
		{
			throw new IOException("malformed request: " + ex.Message);
		}
		catch (InvalidCastException ex2)
		{
			throw new IOException("malformed request: " + ex2.Message);
		}
	}

	public byte[] GetTbsRequest()
	{
		try
		{
			return req.TbsRequest.GetEncoded();
		}
		catch (IOException innerException)
		{
			throw new OcspException("problem encoding tbsRequest", innerException);
		}
	}

	public Req[] GetRequestList()
	{
		return req.TbsRequest.RequestList.MapElements((Asn1Encodable element) => new Req(Request.GetInstance(element)));
	}

	protected override X509Extensions GetX509Extensions()
	{
		return RequestExtensions;
	}

	public byte[] GetSignature()
	{
		if (!IsSigned)
		{
			return null;
		}
		return req.OptionalSignature.GetSignatureOctets();
	}

	private List<X509Certificate> GetCertList()
	{
		List<X509Certificate> list = new List<X509Certificate>();
		Asn1Sequence certs = req.OptionalSignature.Certs;
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
		if (!IsSigned)
		{
			return null;
		}
		return GetCertList().ToArray();
	}

	public IStore<X509Certificate> GetCertificates()
	{
		if (!IsSigned)
		{
			return null;
		}
		return CollectionUtilities.CreateStore(GetCertList());
	}

	public bool Verify(AsymmetricKeyParameter publicKey)
	{
		if (!IsSigned)
		{
			throw new OcspException("attempt to Verify signature on unsigned object");
		}
		try
		{
			ISigner signer = SignerUtilities.GetSigner(SignatureAlgOid);
			signer.Init(forSigning: false, publicKey);
			byte[] encoded = req.TbsRequest.GetEncoded();
			signer.BlockUpdate(encoded, 0, encoded.Length);
			return signer.VerifySignature(GetSignature());
		}
		catch (Exception ex)
		{
			throw new OcspException("exception processing sig: " + ex, ex);
		}
	}

	public byte[] GetEncoded()
	{
		return req.GetEncoded();
	}
}
