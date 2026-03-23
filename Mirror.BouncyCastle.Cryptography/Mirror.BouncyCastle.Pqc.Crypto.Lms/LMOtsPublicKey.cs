using System;
using System.IO;
using Mirror.BouncyCastle.Crypto;
using Mirror.BouncyCastle.Utilities;
using Mirror.BouncyCastle.Utilities.IO;

namespace Mirror.BouncyCastle.Pqc.Crypto.Lms;

public sealed class LMOtsPublicKey
{
	private readonly LMOtsParameters m_parameters;

	private readonly byte[] m_I;

	private readonly int m_q;

	private readonly byte[] m_K;

	public LMOtsParameters Parameters => m_parameters;

	public int Q => m_q;

	[Obsolete("Use 'GetI' instead")]
	public byte[] I => m_I;

	[Obsolete("Use 'GetK' instead")]
	public byte[] K => m_K;

	public LMOtsPublicKey(LMOtsParameters parameters, byte[] i, int q, byte[] k)
	{
		m_parameters = parameters;
		m_I = i;
		m_q = q;
		m_K = k;
	}

	public static LMOtsPublicKey GetInstance(object src)
	{
		if (src is LMOtsPublicKey result)
		{
			return result;
		}
		if (src is BinaryReader binaryReader)
		{
			return Parse(binaryReader);
		}
		if (src is Stream stream)
		{
			return BinaryReaders.Parse(Parse, stream, leaveOpen: true);
		}
		if (src is byte[] buffer)
		{
			return BinaryReaders.Parse(Parse, new MemoryStream(buffer, writable: false), leaveOpen: false);
		}
		throw new ArgumentException($"cannot parse {src}");
	}

	internal static LMOtsPublicKey Parse(BinaryReader binaryReader)
	{
		LMOtsParameters lMOtsParameters = LMOtsParameters.ParseByID(binaryReader);
		byte[] i = BinaryReaders.ReadBytesFully(binaryReader, 16);
		int q = BinaryReaders.ReadInt32BigEndian(binaryReader);
		byte[] k = BinaryReaders.ReadBytesFully(binaryReader, lMOtsParameters.N);
		return new LMOtsPublicKey(lMOtsParameters, i, q, k);
	}

	public byte[] GetI()
	{
		return Arrays.Clone(m_I);
	}

	public byte[] GetK()
	{
		return Arrays.Clone(m_K);
	}

	public override bool Equals(object obj)
	{
		if (this == obj)
		{
			return true;
		}
		if (obj is LMOtsPublicKey lMOtsPublicKey && m_q == lMOtsPublicKey.m_q && object.Equals(m_parameters, lMOtsPublicKey.m_parameters) && Arrays.AreEqual(m_I, lMOtsPublicKey.m_I))
		{
			return Arrays.AreEqual(m_K, lMOtsPublicKey.m_K);
		}
		return false;
	}

	public override int GetHashCode()
	{
		int q = m_q;
		q = 31 * q + Objects.GetHashCode(m_parameters);
		q = 31 * q + Arrays.GetHashCode(m_I);
		return 31 * q + Arrays.GetHashCode(m_K);
	}

	public byte[] GetEncoded()
	{
		return Composer.Compose().U32Str(m_parameters.ID).Bytes(m_I)
			.U32Str(m_q)
			.Bytes(m_K)
			.Build();
	}

	internal LmsContext CreateOtsContext(LMOtsSignature signature)
	{
		IDigest digest = LmsUtilities.GetDigest(m_parameters);
		LmsUtilities.ByteArray(m_I, digest);
		LmsUtilities.U32Str(m_q, digest);
		LmsUtilities.U16Str((short)LMOts.D_MESG, digest);
		LmsUtilities.ByteArray(signature.C, digest);
		return new LmsContext(this, signature, digest);
	}

	internal LmsContext CreateOtsContext(LmsSignature signature)
	{
		IDigest digest = LmsUtilities.GetDigest(m_parameters);
		LmsUtilities.ByteArray(m_I, digest);
		LmsUtilities.U32Str(m_q, digest);
		LmsUtilities.U16Str((short)LMOts.D_MESG, digest);
		LmsUtilities.ByteArray(signature.OtsSignature.C, digest);
		return new LmsContext(this, signature, digest);
	}
}
