using System;
using System.Collections.Generic;
using System.IO;
using Mirror.BouncyCastle.Asn1.Ocsp;
using Mirror.BouncyCastle.Asn1.X509;

namespace Mirror.BouncyCastle.Tls;

public sealed class OcspStatusRequest
{
	private readonly IList<ResponderID> m_responderIDList;

	private readonly X509Extensions m_requestExtensions;

	public IList<ResponderID> ResponderIDList => m_responderIDList;

	public X509Extensions RequestExtensions => m_requestExtensions;

	public OcspStatusRequest(IList<ResponderID> responderIDList, X509Extensions requestExtensions)
	{
		m_responderIDList = responderIDList;
		m_requestExtensions = requestExtensions;
	}

	public void Encode(Stream output)
	{
		if (m_responderIDList == null || m_responderIDList.Count < 1)
		{
			TlsUtilities.WriteUint16(0, output);
		}
		else
		{
			MemoryStream memoryStream = new MemoryStream();
			foreach (ResponderID responderID in m_responderIDList)
			{
				TlsUtilities.WriteOpaque16(responderID.GetEncoded("DER"), memoryStream);
			}
			TlsUtilities.CheckUint16(memoryStream.Length);
			TlsUtilities.WriteUint16(Convert.ToInt32(memoryStream.Length), output);
			memoryStream.WriteTo(output);
		}
		if (m_requestExtensions == null)
		{
			TlsUtilities.WriteUint16(0, output);
			return;
		}
		byte[] encoded = m_requestExtensions.GetEncoded("DER");
		TlsUtilities.CheckUint16(encoded.Length);
		TlsUtilities.WriteUint16(encoded.Length, output);
		output.Write(encoded, 0, encoded.Length);
	}

	public static OcspStatusRequest Parse(Stream input)
	{
		List<ResponderID> list = new List<ResponderID>();
		byte[] array = TlsUtilities.ReadOpaque16(input);
		if (array.Length != 0)
		{
			MemoryStream memoryStream = new MemoryStream(array, writable: false);
			do
			{
				byte[] encoding = TlsUtilities.ReadOpaque16(memoryStream, 1);
				ResponderID instance = ResponderID.GetInstance(TlsUtilities.ReadAsn1Object(encoding));
				TlsUtilities.RequireDerEncoding(instance, encoding);
				list.Add(instance);
			}
			while (memoryStream.Position < memoryStream.Length);
		}
		X509Extensions requestExtensions = null;
		byte[] array2 = TlsUtilities.ReadOpaque16(input);
		if (array2.Length != 0)
		{
			X509Extensions instance2 = X509Extensions.GetInstance(TlsUtilities.ReadAsn1Object(array2));
			TlsUtilities.RequireDerEncoding(instance2, array2);
			requestExtensions = instance2;
		}
		return new OcspStatusRequest(list, requestExtensions);
	}
}
