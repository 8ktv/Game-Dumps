using System;
using Mirror.BouncyCastle.Crypto.Parameters;
using Mirror.BouncyCastle.Crypto.Utilities;

namespace Mirror.BouncyCastle.Crypto.Agreement.Kdf;

public sealed class ConcatenationKdfGenerator : IDerivationFunction
{
	private readonly IDigest m_digest;

	private readonly int m_hLen;

	private byte[] m_buffer;

	public IDigest Digest => m_digest;

	public ConcatenationKdfGenerator(IDigest digest)
	{
		m_digest = digest;
		m_hLen = digest.GetDigestSize();
	}

	public void Init(IDerivationParameters param)
	{
		KdfParameters obj = (param as KdfParameters) ?? throw new ArgumentException("KDF parameters required for ConcatenationKdfGenerator");
		byte[] sharedSecret = obj.GetSharedSecret();
		byte[] iV = obj.GetIV();
		m_buffer = new byte[4 + sharedSecret.Length + ((iV != null) ? iV.Length : 0) + m_hLen];
		sharedSecret.CopyTo(m_buffer, 4);
		iV?.CopyTo(m_buffer, 4 + sharedSecret.Length);
	}

	public int GenerateBytes(byte[] output, int outOff, int length)
	{
		Check.OutputLength(output, outOff, length, "output buffer too short");
		int num = m_buffer.Length - m_hLen;
		uint n = 1u;
		m_digest.Reset();
		int num2 = outOff + length;
		int num3 = num2 - m_hLen;
		while (outOff <= num3)
		{
			Pack.UInt32_To_BE(n++, m_buffer, 0);
			m_digest.BlockUpdate(m_buffer, 0, num);
			m_digest.DoFinal(output, outOff);
			outOff += m_hLen;
		}
		if (outOff < num2)
		{
			Pack.UInt32_To_BE(n, m_buffer, 0);
			m_digest.BlockUpdate(m_buffer, 0, num);
			m_digest.DoFinal(m_buffer, num);
			Array.Copy(m_buffer, num, output, outOff, num2 - outOff);
		}
		return length;
	}
}
