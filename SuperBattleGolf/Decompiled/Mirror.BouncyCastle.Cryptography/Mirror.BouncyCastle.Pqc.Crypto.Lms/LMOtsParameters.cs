using System;
using System.Collections.Generic;
using System.IO;
using Mirror.BouncyCastle.Asn1;
using Mirror.BouncyCastle.Asn1.Nist;
using Mirror.BouncyCastle.Utilities.Collections;
using Mirror.BouncyCastle.Utilities.IO;

namespace Mirror.BouncyCastle.Pqc.Crypto.Lms;

public sealed class LMOtsParameters
{
	public static LMOtsParameters sha256_n32_w1 = new LMOtsParameters(1, 32, 1, 265, 7, 8516u, NistObjectIdentifiers.IdSha256);

	public static LMOtsParameters sha256_n32_w2 = new LMOtsParameters(2, 32, 2, 133, 6, 4292u, NistObjectIdentifiers.IdSha256);

	public static LMOtsParameters sha256_n32_w4 = new LMOtsParameters(3, 32, 4, 67, 4, 2180u, NistObjectIdentifiers.IdSha256);

	public static LMOtsParameters sha256_n32_w8 = new LMOtsParameters(4, 32, 8, 34, 0, 1124u, NistObjectIdentifiers.IdSha256);

	public static LMOtsParameters sha256_n24_w1 = new LMOtsParameters(5, 24, 1, 200, 8, 5436u, NistObjectIdentifiers.IdSha256);

	public static LMOtsParameters sha256_n24_w2 = new LMOtsParameters(6, 24, 2, 101, 6, 2940u, NistObjectIdentifiers.IdSha256);

	public static LMOtsParameters sha256_n24_w4 = new LMOtsParameters(7, 24, 4, 51, 4, 1500u, NistObjectIdentifiers.IdSha256);

	public static LMOtsParameters sha256_n24_w8 = new LMOtsParameters(8, 24, 8, 26, 0, 1020u, NistObjectIdentifiers.IdSha256);

	public static LMOtsParameters shake256_n32_w1 = new LMOtsParameters(9, 32, 1, 265, 7, 8516u, NistObjectIdentifiers.IdShake256Len);

	public static LMOtsParameters shake256_n32_w2 = new LMOtsParameters(10, 32, 2, 133, 6, 4292u, NistObjectIdentifiers.IdShake256Len);

	public static LMOtsParameters shake256_n32_w4 = new LMOtsParameters(11, 32, 4, 67, 4, 2180u, NistObjectIdentifiers.IdShake256Len);

	public static LMOtsParameters shake256_n32_w8 = new LMOtsParameters(12, 32, 8, 34, 0, 1124u, NistObjectIdentifiers.IdShake256Len);

	public static LMOtsParameters shake256_n24_w1 = new LMOtsParameters(13, 24, 1, 200, 8, 5436u, NistObjectIdentifiers.IdShake256Len);

	public static LMOtsParameters shake256_n24_w2 = new LMOtsParameters(14, 24, 2, 101, 6, 2940u, NistObjectIdentifiers.IdShake256Len);

	public static LMOtsParameters shake256_n24_w4 = new LMOtsParameters(15, 24, 4, 51, 4, 1500u, NistObjectIdentifiers.IdShake256Len);

	public static LMOtsParameters shake256_n24_w8 = new LMOtsParameters(16, 24, 8, 26, 0, 1020u, NistObjectIdentifiers.IdShake256Len);

	private static Dictionary<int, LMOtsParameters> ParametersByID = new Dictionary<int, LMOtsParameters>
	{
		{ sha256_n32_w1.ID, sha256_n32_w1 },
		{ sha256_n32_w2.ID, sha256_n32_w2 },
		{ sha256_n32_w4.ID, sha256_n32_w4 },
		{ sha256_n32_w8.ID, sha256_n32_w8 },
		{ sha256_n24_w1.ID, sha256_n24_w1 },
		{ sha256_n24_w2.ID, sha256_n24_w2 },
		{ sha256_n24_w4.ID, sha256_n24_w4 },
		{ sha256_n24_w8.ID, sha256_n24_w8 },
		{ shake256_n32_w1.ID, shake256_n32_w1 },
		{ shake256_n32_w2.ID, shake256_n32_w2 },
		{ shake256_n32_w4.ID, shake256_n32_w4 },
		{ shake256_n32_w8.ID, shake256_n32_w8 },
		{ shake256_n24_w1.ID, shake256_n24_w1 },
		{ shake256_n24_w2.ID, shake256_n24_w2 },
		{ shake256_n24_w4.ID, shake256_n24_w4 },
		{ shake256_n24_w8.ID, shake256_n24_w8 }
	};

	private readonly int m_id;

	private readonly int m_n;

	private readonly int m_w;

	private readonly int m_p;

	private readonly int m_ls;

	private readonly uint m_sigLen;

	private readonly DerObjectIdentifier m_digestOid;

	public int ID => m_id;

	public int N => m_n;

	public int W => m_w;

	public int P => m_p;

	public int Ls => m_ls;

	public int SigLen => Convert.ToInt32(m_sigLen);

	public DerObjectIdentifier DigestOid => m_digestOid;

	public static LMOtsParameters GetParametersByID(int id)
	{
		return CollectionUtilities.GetValueOrNull(ParametersByID, id);
	}

	internal static LMOtsParameters ParseByID(BinaryReader binaryReader)
	{
		int num = BinaryReaders.ReadInt32BigEndian(binaryReader);
		if (!ParametersByID.TryGetValue(num, out var value))
		{
			throw new InvalidDataException($"unknown LMOtsParameters {num}");
		}
		return value;
	}

	internal LMOtsParameters(int id, int n, int w, int p, int ls, uint sigLen, DerObjectIdentifier digestOid)
	{
		m_id = id;
		m_n = n;
		m_w = w;
		m_p = p;
		m_ls = ls;
		m_sigLen = sigLen;
		m_digestOid = digestOid;
	}
}
