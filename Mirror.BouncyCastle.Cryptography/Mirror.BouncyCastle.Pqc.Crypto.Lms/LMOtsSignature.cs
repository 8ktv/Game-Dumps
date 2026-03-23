using System;
using System.IO;
using Mirror.BouncyCastle.Utilities;
using Mirror.BouncyCastle.Utilities.IO;

namespace Mirror.BouncyCastle.Pqc.Crypto.Lms;

public sealed class LMOtsSignature : IEncodable
{
	private readonly LMOtsParameters m_paramType;

	private readonly byte[] m_C;

	private readonly byte[] m_y;

	public LMOtsParameters ParamType => m_paramType;

	[Obsolete("Use 'GetC' instead")]
	public byte[] C => m_C;

	[Obsolete("Use 'GetY' instead")]
	public byte[] Y => m_y;

	public LMOtsSignature(LMOtsParameters paramType, byte[] c, byte[] y)
	{
		m_paramType = paramType;
		m_C = c;
		m_y = y;
	}

	public static LMOtsSignature GetInstance(object src)
	{
		if (src is LMOtsSignature result)
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

	internal static LMOtsSignature Parse(BinaryReader binaryReader)
	{
		LMOtsParameters lMOtsParameters = LMOtsParameters.ParseByID(binaryReader);
		byte[] c = BinaryReaders.ReadBytesFully(binaryReader, lMOtsParameters.N);
		byte[] y = BinaryReaders.ReadBytesFully(binaryReader, lMOtsParameters.P * lMOtsParameters.N);
		return new LMOtsSignature(lMOtsParameters, c, y);
	}

	public byte[] GetC()
	{
		return Arrays.Clone(m_C);
	}

	public byte[] GetY()
	{
		return Arrays.Clone(m_y);
	}

	public override bool Equals(object obj)
	{
		if (this == obj)
		{
			return true;
		}
		if (!(obj is LMOtsSignature lMOtsSignature))
		{
			return false;
		}
		if (object.Equals(m_paramType, lMOtsSignature.m_paramType) && Arrays.AreEqual(m_C, lMOtsSignature.m_C))
		{
			return Arrays.AreEqual(m_y, lMOtsSignature.m_y);
		}
		return false;
	}

	public override int GetHashCode()
	{
		int hashCode = Objects.GetHashCode(m_paramType);
		hashCode = 31 * hashCode + Arrays.GetHashCode(m_C);
		return 31 * hashCode + Arrays.GetHashCode(m_y);
	}

	public byte[] GetEncoded()
	{
		return Composer.Compose().U32Str(m_paramType.ID).Bytes(m_C)
			.Bytes(m_y)
			.Build();
	}
}
