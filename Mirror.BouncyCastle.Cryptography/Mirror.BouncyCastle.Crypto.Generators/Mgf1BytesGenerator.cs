using System;
using Mirror.BouncyCastle.Crypto.Parameters;
using Mirror.BouncyCastle.Crypto.Utilities;

namespace Mirror.BouncyCastle.Crypto.Generators;

public sealed class Mgf1BytesGenerator : IDerivationFunction
{
	private readonly IDigest m_digest;

	private readonly int m_hLen;

	private byte[] m_buffer;

	public IDigest Digest => m_digest;

	public Mgf1BytesGenerator(IDigest digest)
	{
		m_digest = digest;
		m_hLen = digest.GetDigestSize();
	}

	public void Init(IDerivationParameters parameters)
	{
		if (!(parameters is MgfParameters mgfParameters))
		{
			throw new ArgumentException("MGF parameters required for MGF1Generator");
		}
		m_buffer = new byte[mgfParameters.SeedLength + 4 + m_hLen];
		mgfParameters.GetSeed(m_buffer, 0);
	}

	public int GenerateBytes(byte[] output, int outOff, int length)
	{
		Check.OutputLength(output, outOff, length, "output buffer too short");
		int num = m_buffer.Length - m_hLen;
		int off = num - 4;
		uint n = 0u;
		m_digest.Reset();
		int num2 = outOff + length;
		int num3 = num2 - m_hLen;
		while (outOff <= num3)
		{
			Pack.UInt32_To_BE(n++, m_buffer, off);
			m_digest.BlockUpdate(m_buffer, 0, num);
			m_digest.DoFinal(output, outOff);
			outOff += m_hLen;
		}
		if (outOff < num2)
		{
			Pack.UInt32_To_BE(n, m_buffer, off);
			m_digest.BlockUpdate(m_buffer, 0, num);
			m_digest.DoFinal(m_buffer, num);
			Array.Copy(m_buffer, num, output, outOff, num2 - outOff);
		}
		return length;
	}
}
