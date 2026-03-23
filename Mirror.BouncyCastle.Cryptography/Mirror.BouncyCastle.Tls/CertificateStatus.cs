using System;
using System.Collections.Generic;
using System.IO;
using Mirror.BouncyCastle.Asn1.Ocsp;

namespace Mirror.BouncyCastle.Tls;

public sealed class CertificateStatus
{
	private readonly short m_statusType;

	private readonly object m_response;

	public short StatusType => m_statusType;

	public object Response => m_response;

	public OcspResponse OcspResponse
	{
		get
		{
			if (!IsCorrectType(1, m_response))
			{
				throw new InvalidOperationException("'response' is not an OCSPResponse");
			}
			return (OcspResponse)m_response;
		}
	}

	public IList<OcspResponse> OcspResponseList
	{
		get
		{
			if (!IsCorrectType(2, m_response))
			{
				throw new InvalidOperationException("'response' is not an OCSPResponseList");
			}
			return (IList<OcspResponse>)m_response;
		}
	}

	public CertificateStatus(short statusType, object response)
	{
		if (!IsCorrectType(statusType, response))
		{
			throw new ArgumentException("not an instance of the correct type", "response");
		}
		m_statusType = statusType;
		m_response = response;
	}

	public void Encode(Stream output)
	{
		TlsUtilities.WriteUint8(m_statusType, output);
		switch (m_statusType)
		{
		case 1:
			TlsUtilities.WriteOpaque24(((OcspResponse)m_response).GetEncoded("DER"), output);
			break;
		case 2:
		{
			IList<OcspResponse> obj = (IList<OcspResponse>)m_response;
			List<byte[]> list = new List<byte[]>(obj.Count);
			long num = 0L;
			foreach (OcspResponse item in obj)
			{
				if (item == null)
				{
					list.Add(TlsUtilities.EmptyBytes);
				}
				else
				{
					byte[] encoded = item.GetEncoded("DER");
					list.Add(encoded);
					num += encoded.Length;
				}
				num += 3;
			}
			TlsUtilities.CheckUint24(num);
			TlsUtilities.WriteUint24((int)num, output);
			{
				foreach (byte[] item2 in list)
				{
					TlsUtilities.WriteOpaque24(item2, output);
				}
				break;
			}
		}
		default:
			throw new TlsFatalAlert(80);
		}
	}

	public static CertificateStatus Parse(TlsContext context, Stream input)
	{
		SecurityParameters securityParameters = context.SecurityParameters;
		Certificate peerCertificate = securityParameters.PeerCertificate;
		if (peerCertificate == null || peerCertificate.IsEmpty || peerCertificate.CertificateType != 0)
		{
			throw new TlsFatalAlert(80);
		}
		int length = peerCertificate.Length;
		int statusRequestVersion = securityParameters.StatusRequestVersion;
		short num = TlsUtilities.ReadUint8(input);
		object response;
		switch (num)
		{
		case 1:
			RequireStatusRequestVersion(1, statusRequestVersion);
			response = ParseOcspResponse(TlsUtilities.ReadOpaque24(input, 1));
			break;
		case 2:
		{
			RequireStatusRequestVersion(2, statusRequestVersion);
			MemoryStream memoryStream = new MemoryStream(TlsUtilities.ReadOpaque24(input, 1), writable: false);
			List<OcspResponse> list = new List<OcspResponse>();
			while (memoryStream.Position < memoryStream.Length)
			{
				if (list.Count >= length)
				{
					throw new TlsFatalAlert(47);
				}
				int num2 = TlsUtilities.ReadUint24(memoryStream);
				if (num2 < 1)
				{
					list.Add(null);
					continue;
				}
				byte[] derEncoding = TlsUtilities.ReadFully(num2, memoryStream);
				list.Add(ParseOcspResponse(derEncoding));
			}
			response = list;
			break;
		}
		default:
			throw new TlsFatalAlert(50);
		}
		return new CertificateStatus(num, response);
	}

	private static bool IsCorrectType(short statusType, object response)
	{
		return statusType switch
		{
			1 => response is OcspResponse, 
			2 => IsOcspResponseList(response), 
			_ => throw new ArgumentException("unsupported CertificateStatusType", "statusType"), 
		};
	}

	private static bool IsOcspResponseList(object response)
	{
		if (response is IList<OcspResponse> list)
		{
			return list.Count > 0;
		}
		return false;
	}

	private static OcspResponse ParseOcspResponse(byte[] derEncoding)
	{
		OcspResponse instance = OcspResponse.GetInstance(TlsUtilities.ReadAsn1Object(derEncoding));
		TlsUtilities.RequireDerEncoding(instance, derEncoding);
		return instance;
	}

	private static void RequireStatusRequestVersion(int minVersion, int statusRequestVersion)
	{
		if (statusRequestVersion < minVersion)
		{
			throw new TlsFatalAlert(50);
		}
	}
}
