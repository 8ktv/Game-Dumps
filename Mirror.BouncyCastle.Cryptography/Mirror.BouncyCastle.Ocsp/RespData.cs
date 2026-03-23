using System;
using Mirror.BouncyCastle.Asn1;
using Mirror.BouncyCastle.Asn1.Ocsp;
using Mirror.BouncyCastle.Asn1.X509;
using Mirror.BouncyCastle.X509;

namespace Mirror.BouncyCastle.Ocsp;

public class RespData : X509ExtensionBase
{
	internal readonly ResponseData data;

	public int Version => data.Version.IntValueExact + 1;

	public DateTime ProducedAt => data.ProducedAt.ToDateTime();

	public X509Extensions ResponseExtensions => data.ResponseExtensions;

	public RespData(ResponseData data)
	{
		this.data = data;
	}

	public RespID GetResponderId()
	{
		return new RespID(data.ResponderID);
	}

	public SingleResp[] GetResponses()
	{
		return data.Responses.MapElements((Asn1Encodable element) => new SingleResp(SingleResponse.GetInstance(element)));
	}

	protected override X509Extensions GetX509Extensions()
	{
		return ResponseExtensions;
	}
}
