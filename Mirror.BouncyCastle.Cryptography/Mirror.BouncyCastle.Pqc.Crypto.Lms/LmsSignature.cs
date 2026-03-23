using System;
using System.IO;
using Mirror.BouncyCastle.Utilities;
using Mirror.BouncyCastle.Utilities.IO;

namespace Mirror.BouncyCastle.Pqc.Crypto.Lms;

public class LmsSignature : IEncodable
{
	private readonly int m_q;

	private readonly LMOtsSignature m_otsSignature;

	private readonly LMSigParameters m_parameters;

	private readonly byte[][] m_y;

	public LMOtsSignature OtsSignature => m_otsSignature;

	public int Q => m_q;

	public LMSigParameters SigParameters => m_parameters;

	public byte[][] Y => m_y;

	public LmsSignature(int q, LMOtsSignature otsSignature, LMSigParameters parameter, byte[][] y)
	{
		m_q = q;
		m_otsSignature = otsSignature;
		m_parameters = parameter;
		m_y = y;
	}

	public static LmsSignature GetInstance(object src)
	{
		if (src is LmsSignature result)
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

	internal static LmsSignature Parse(BinaryReader binaryReader)
	{
		int q = BinaryReaders.ReadInt32BigEndian(binaryReader);
		LMOtsSignature otsSignature = LMOtsSignature.Parse(binaryReader);
		LMSigParameters lMSigParameters = LMSigParameters.ParseByID(binaryReader);
		byte[][] array = new byte[lMSigParameters.H][];
		for (int i = 0; i < array.Length; i++)
		{
			array[i] = new byte[lMSigParameters.M];
			binaryReader.Read(array[i], 0, array[i].Length);
		}
		return new LmsSignature(q, otsSignature, lMSigParameters, array);
	}

	public override bool Equals(object o)
	{
		if (this == o)
		{
			return true;
		}
		if (o is LmsSignature lmsSignature && m_q == lmsSignature.m_q && object.Equals(m_otsSignature, lmsSignature.m_otsSignature) && object.Equals(m_parameters, lmsSignature.m_parameters))
		{
			return DeepEquals(m_y, lmsSignature.m_y);
		}
		return false;
	}

	public override int GetHashCode()
	{
		int q = m_q;
		q = 31 * q + Objects.GetHashCode(m_otsSignature);
		q = 31 * q + Objects.GetHashCode(m_parameters);
		return 31 * q + DeepGetHashCode(m_y);
	}

	public byte[] GetEncoded()
	{
		return Composer.Compose().U32Str(m_q).Bytes(m_otsSignature.GetEncoded())
			.U32Str(m_parameters.ID)
			.Bytes2(m_y)
			.Build();
	}

	private static bool DeepEquals(byte[][] a, byte[][] b)
	{
		if (a == b)
		{
			return true;
		}
		int num = a.Length;
		if (num != b.Length)
		{
			return false;
		}
		for (int i = 0; i < num; i++)
		{
			if (!Arrays.AreEqual(a[i], b[i]))
			{
				return false;
			}
		}
		return true;
	}

	private static int DeepGetHashCode(byte[][] a)
	{
		if (a == null)
		{
			return 0;
		}
		int num = a.Length;
		int num2 = num + 1;
		for (int i = 0; i < num; i++)
		{
			num2 *= 257;
			num2 ^= Arrays.GetHashCode(a[i]);
		}
		return num2;
	}
}
