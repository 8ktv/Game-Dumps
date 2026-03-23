using System;
using System.IO;

namespace Mirror.BouncyCastle.Tls;

public sealed class CertificateVerify
{
	private readonly int m_algorithm;

	private readonly byte[] m_signature;

	public int Algorithm => m_algorithm;

	public byte[] Signature => m_signature;

	public CertificateVerify(int algorithm, byte[] signature)
	{
		if (!TlsUtilities.IsValidUint16(algorithm))
		{
			throw new ArgumentException("algorithm");
		}
		if (signature == null)
		{
			throw new ArgumentNullException("signature");
		}
		m_algorithm = algorithm;
		m_signature = signature;
	}

	public void Encode(Stream output)
	{
		TlsUtilities.WriteUint16(m_algorithm, output);
		TlsUtilities.WriteOpaque16(m_signature, output);
	}

	public static CertificateVerify Parse(TlsContext context, Stream input)
	{
		if (!TlsUtilities.IsTlsV13(context))
		{
			throw new InvalidOperationException();
		}
		int algorithm = TlsUtilities.ReadUint16(input);
		byte[] signature = TlsUtilities.ReadOpaque16(input);
		return new CertificateVerify(algorithm, signature);
	}
}
