using System;
using System.IO;
using System.Text;
using Mirror.BouncyCastle.Utilities;
using Mirror.BouncyCastle.Utilities.IO;

namespace Mirror.BouncyCastle.Pqc.Crypto.Lms;

public sealed class HssSignature : IEncodable
{
	private readonly int m_lMinus1;

	private readonly LmsSignedPubKey[] m_signedPubKeys;

	private readonly LmsSignature m_signature;

	public int LMinus1 => m_lMinus1;

	internal LmsSignedPubKey[] SignedPubKeys => m_signedPubKeys;

	public LmsSignature Signature => m_signature;

	public HssSignature(int lMinus1, LmsSignedPubKey[] signedPubKey, LmsSignature signature)
	{
		m_lMinus1 = lMinus1;
		m_signedPubKeys = signedPubKey;
		m_signature = signature;
	}

	public static HssSignature GetInstance(object src, int L)
	{
		if (src is HssSignature result)
		{
			return result;
		}
		if (src is BinaryReader binaryReader)
		{
			return Parse(L, binaryReader);
		}
		if (src is Stream stream)
		{
			return Parse(L, stream, leaveOpen: true);
		}
		if (src is byte[] buffer)
		{
			return Parse(L, new MemoryStream(buffer, writable: false), leaveOpen: false);
		}
		throw new ArgumentException($"cannot parse {src}");
	}

	internal static HssSignature Parse(int L, BinaryReader binaryReader)
	{
		int num = BinaryReaders.ReadInt32BigEndian(binaryReader);
		if (num != L - 1)
		{
			throw new Exception("nspk exceeded maxNspk");
		}
		LmsSignedPubKey[] array = new LmsSignedPubKey[num];
		for (int i = 0; i < num; i++)
		{
			LmsSignature signature = LmsSignature.Parse(binaryReader);
			LmsPublicKeyParameters publicKey = LmsPublicKeyParameters.Parse(binaryReader);
			array[i] = new LmsSignedPubKey(signature, publicKey);
		}
		LmsSignature signature2 = LmsSignature.Parse(binaryReader);
		return new HssSignature(num, array, signature2);
	}

	private static HssSignature Parse(int L, Stream stream, bool leaveOpen)
	{
		using BinaryReader binaryReader = new BinaryReader(stream, Encoding.UTF8, leaveOpen);
		return Parse(L, binaryReader);
	}

	[Obsolete("Use 'LMinus1' instead")]
	public int GetLMinus1()
	{
		return m_lMinus1;
	}

	public LmsSignedPubKey[] GetSignedPubKeys()
	{
		return (LmsSignedPubKey[])m_signedPubKeys?.Clone();
	}

	public override bool Equals(object other)
	{
		if (this == other)
		{
			return true;
		}
		if (other is HssSignature hssSignature && m_lMinus1 == hssSignature.m_lMinus1)
		{
			object[] signedPubKeys = m_signedPubKeys;
			object[] a = signedPubKeys;
			signedPubKeys = hssSignature.m_signedPubKeys;
			if (Arrays.AreEqual(a, signedPubKeys))
			{
				return object.Equals(m_signature, hssSignature.m_signature);
			}
		}
		return false;
	}

	public override int GetHashCode()
	{
		int lMinus = m_lMinus1;
		int num = 31 * lMinus;
		object[] signedPubKeys = m_signedPubKeys;
		lMinus = num + Arrays.GetHashCode(signedPubKeys);
		return 31 * lMinus + Objects.GetHashCode(m_signature);
	}

	public byte[] GetEncoded()
	{
		Composer composer = Composer.Compose();
		composer.U32Str(m_lMinus1);
		if (m_signedPubKeys != null)
		{
			LmsSignedPubKey[] signedPubKeys = m_signedPubKeys;
			foreach (LmsSignedPubKey encodable in signedPubKeys)
			{
				composer.Bytes(encodable);
			}
		}
		composer.Bytes(m_signature);
		return composer.Build();
	}
}
