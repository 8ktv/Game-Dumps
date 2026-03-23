using System;
using Mirror.BouncyCastle.Crypto;
using Mirror.BouncyCastle.Utilities;

namespace Mirror.BouncyCastle.Pqc.Crypto.Lms;

public sealed class SeedDerive
{
	private readonly byte[] m_I;

	private readonly byte[] m_masterSeed;

	private readonly IDigest m_digest;

	public int J { get; set; }

	public int Q { get; set; }

	[Obsolete("Use 'GetI' instead")]
	public byte[] I => m_I;

	[Obsolete("Use 'GetMasterSeed' instead")]
	public byte[] MasterSeed => m_masterSeed;

	public SeedDerive(byte[] I, byte[] masterSeed, IDigest digest)
	{
		m_I = I;
		m_masterSeed = masterSeed;
		m_digest = digest;
	}

	public byte[] GetI()
	{
		return Arrays.Clone(m_I);
	}

	public byte[] GetMasterSeed()
	{
		return Arrays.Clone(m_masterSeed);
	}

	public byte[] DeriveSeed(bool incJ, byte[] target, int offset)
	{
		if (target.Length - offset < m_digest.GetDigestSize())
		{
			throw new ArgumentException("target length is less than digest size.", "target");
		}
		int q = Q;
		int j = J;
		m_digest.BlockUpdate(I, 0, I.Length);
		m_digest.Update((byte)(q >> 24));
		m_digest.Update((byte)(q >> 16));
		m_digest.Update((byte)(q >> 8));
		m_digest.Update((byte)q);
		m_digest.Update((byte)(j >> 8));
		m_digest.Update((byte)j);
		m_digest.Update(byte.MaxValue);
		m_digest.BlockUpdate(m_masterSeed, 0, m_masterSeed.Length);
		m_digest.DoFinal(target, offset);
		if (incJ)
		{
			int j2 = J + 1;
			J = j2;
		}
		return target;
	}
}
