using System;
using Mirror.BouncyCastle.Asn1;
using Mirror.BouncyCastle.Asn1.Nist;
using Mirror.BouncyCastle.Crypto;
using Mirror.BouncyCastle.Security;

namespace Mirror.BouncyCastle.Pqc.Crypto.Lms;

public static class LmsUtilities
{
	internal class WrapperDigest : IDigest
	{
		private readonly IDigest m_digest;

		private readonly int m_length;

		public string AlgorithmName => m_digest.AlgorithmName;

		internal WrapperDigest(IDigest digest, int length)
		{
			m_digest = digest;
			m_length = length;
		}

		public void BlockUpdate(byte[] input, int inOff, int inLen)
		{
			m_digest.BlockUpdate(input, inOff, inLen);
		}

		public int DoFinal(byte[] output, int outOff)
		{
			byte[] array = new byte[m_digest.GetDigestSize()];
			m_digest.DoFinal(array, 0);
			Array.Copy(array, 0, output, outOff, m_length);
			return m_length;
		}

		public int GetByteLength()
		{
			return m_digest.GetByteLength();
		}

		public int GetDigestSize()
		{
			return m_length;
		}

		public void Reset()
		{
			m_digest.Reset();
		}

		public void Update(byte input)
		{
			m_digest.Update(input);
		}
	}

	public static void U32Str(int n, IDigest d)
	{
		d.Update((byte)(n >> 24));
		d.Update((byte)(n >> 16));
		d.Update((byte)(n >> 8));
		d.Update((byte)n);
	}

	public static void U16Str(short n, IDigest d)
	{
		d.Update((byte)(n >> 8));
		d.Update((byte)n);
	}

	public static void ByteArray(byte[] array, IDigest digest)
	{
		digest.BlockUpdate(array, 0, array.Length);
	}

	public static void ByteArray(byte[] array, int start, int len, IDigest digest)
	{
		digest.BlockUpdate(array, start, len);
	}

	public static int CalculateStrength(LmsParameters lmsParameters)
	{
		if (lmsParameters == null)
		{
			throw new ArgumentNullException("lmsParameters");
		}
		LMSigParameters lMSigParameters = lmsParameters.LMSigParameters;
		return lMSigParameters.M << lMSigParameters.H;
	}

	internal static IDigest GetDigest(LMOtsParameters otsParameters)
	{
		return CreateDigest(otsParameters.DigestOid, otsParameters.N);
	}

	internal static IDigest GetDigest(LMSigParameters sigParameters)
	{
		return CreateDigest(sigParameters.DigestOid, sigParameters.M);
	}

	private static IDigest CreateDigest(DerObjectIdentifier oid, int length)
	{
		IDigest digest = CreateDigest(oid);
		if (NistObjectIdentifiers.IdShake256Len.Equals(oid) || digest.GetDigestSize() != length)
		{
			return new WrapperDigest(digest, length);
		}
		return digest;
	}

	private static IDigest CreateDigest(DerObjectIdentifier oid)
	{
		if (NistObjectIdentifiers.IdSha256.Equals(oid))
		{
			return DigestUtilities.GetDigest(NistObjectIdentifiers.IdSha256);
		}
		if (NistObjectIdentifiers.IdShake256Len.Equals(oid))
		{
			return DigestUtilities.GetDigest(NistObjectIdentifiers.IdShake256);
		}
		throw new LmsException("unrecognized digest OID: " + oid);
	}
}
