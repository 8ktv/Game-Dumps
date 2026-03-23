using System;
using System.IO;

namespace Mirror.BouncyCastle.Tls;

public sealed class DigitallySigned
{
	private readonly SignatureAndHashAlgorithm m_algorithm;

	private readonly byte[] m_signature;

	public SignatureAndHashAlgorithm Algorithm => m_algorithm;

	public byte[] Signature => m_signature;

	public DigitallySigned(SignatureAndHashAlgorithm algorithm, byte[] signature)
	{
		if (signature == null)
		{
			throw new ArgumentNullException("signature");
		}
		m_algorithm = algorithm;
		m_signature = signature;
	}

	public void Encode(Stream output)
	{
		if (m_algorithm != null)
		{
			m_algorithm.Encode(output);
		}
		TlsUtilities.WriteOpaque16(m_signature, output);
	}

	public static DigitallySigned Parse(TlsContext context, Stream input)
	{
		SignatureAndHashAlgorithm signatureAndHashAlgorithm = null;
		if (TlsUtilities.IsTlsV12(context))
		{
			signatureAndHashAlgorithm = SignatureAndHashAlgorithm.Parse(input);
			if (signatureAndHashAlgorithm.Signature == 0)
			{
				throw new TlsFatalAlert(47);
			}
		}
		byte[] signature = TlsUtilities.ReadOpaque16(input);
		return new DigitallySigned(signatureAndHashAlgorithm, signature);
	}
}
