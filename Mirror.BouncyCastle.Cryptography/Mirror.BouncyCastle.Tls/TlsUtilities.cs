using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Sockets;
using Mirror.BouncyCastle.Asn1;
using Mirror.BouncyCastle.Asn1.Bsi;
using Mirror.BouncyCastle.Asn1.Eac;
using Mirror.BouncyCastle.Asn1.EdEC;
using Mirror.BouncyCastle.Asn1.Nist;
using Mirror.BouncyCastle.Asn1.Oiw;
using Mirror.BouncyCastle.Asn1.Pkcs;
using Mirror.BouncyCastle.Asn1.Rosstandart;
using Mirror.BouncyCastle.Asn1.X509;
using Mirror.BouncyCastle.Asn1.X9;
using Mirror.BouncyCastle.Math;
using Mirror.BouncyCastle.Tls.Crypto;
using Mirror.BouncyCastle.Utilities;
using Mirror.BouncyCastle.Utilities.Collections;
using Mirror.BouncyCastle.Utilities.Date;
using Mirror.BouncyCastle.Utilities.Encoders;
using Mirror.BouncyCastle.Utilities.IO;

namespace Mirror.BouncyCastle.Tls;

public abstract class TlsUtilities
{
	private static readonly byte[] DowngradeTlsV11 = Hex.DecodeStrict("444F574E47524400");

	private static readonly byte[] DowngradeTlsV12 = Hex.DecodeStrict("444F574E47524401");

	private static readonly IDictionary<string, SignatureAndHashAlgorithm> CertSigAlgOids = CreateCertSigAlgOids();

	private static readonly IList<SignatureAndHashAlgorithm> DefaultSupportedSigAlgs = CreateDefaultSupportedSigAlgs();

	public static readonly byte[] EmptyBytes = new byte[0];

	public static readonly short[] EmptyShorts = new short[0];

	public static readonly int[] EmptyInts = new int[0];

	public static readonly long[] EmptyLongs = new long[0];

	public static readonly string[] EmptyStrings = new string[0];

	internal static short MinimumHashStrict = 2;

	internal static short MinimumHashPreferred = 4;

	private static void AddCertSigAlgOid(IDictionary<string, SignatureAndHashAlgorithm> d, DerObjectIdentifier oid, SignatureAndHashAlgorithm sigAndHash)
	{
		d[oid.Id] = sigAndHash;
	}

	private static void AddCertSigAlgOid(IDictionary<string, SignatureAndHashAlgorithm> d, DerObjectIdentifier oid, short hashAlgorithm, short signatureAlgorithm)
	{
		AddCertSigAlgOid(d, oid, SignatureAndHashAlgorithm.GetInstance(hashAlgorithm, signatureAlgorithm));
	}

	private static IDictionary<string, SignatureAndHashAlgorithm> CreateCertSigAlgOids()
	{
		Dictionary<string, SignatureAndHashAlgorithm> dictionary = new Dictionary<string, SignatureAndHashAlgorithm>();
		AddCertSigAlgOid(dictionary, NistObjectIdentifiers.DsaWithSha224, 3, 2);
		AddCertSigAlgOid(dictionary, NistObjectIdentifiers.DsaWithSha256, 4, 2);
		AddCertSigAlgOid(dictionary, NistObjectIdentifiers.DsaWithSha384, 5, 2);
		AddCertSigAlgOid(dictionary, NistObjectIdentifiers.DsaWithSha512, 6, 2);
		AddCertSigAlgOid(dictionary, OiwObjectIdentifiers.DsaWithSha1, 2, 2);
		AddCertSigAlgOid(dictionary, OiwObjectIdentifiers.Sha1WithRsa, 2, 1);
		AddCertSigAlgOid(dictionary, PkcsObjectIdentifiers.Sha1WithRsaEncryption, 2, 1);
		AddCertSigAlgOid(dictionary, PkcsObjectIdentifiers.Sha224WithRsaEncryption, 3, 1);
		AddCertSigAlgOid(dictionary, PkcsObjectIdentifiers.Sha256WithRsaEncryption, 4, 1);
		AddCertSigAlgOid(dictionary, PkcsObjectIdentifiers.Sha384WithRsaEncryption, 5, 1);
		AddCertSigAlgOid(dictionary, PkcsObjectIdentifiers.Sha512WithRsaEncryption, 6, 1);
		AddCertSigAlgOid(dictionary, X9ObjectIdentifiers.ECDsaWithSha1, 2, 3);
		AddCertSigAlgOid(dictionary, X9ObjectIdentifiers.ECDsaWithSha224, 3, 3);
		AddCertSigAlgOid(dictionary, X9ObjectIdentifiers.ECDsaWithSha256, 4, 3);
		AddCertSigAlgOid(dictionary, X9ObjectIdentifiers.ECDsaWithSha384, 5, 3);
		AddCertSigAlgOid(dictionary, X9ObjectIdentifiers.ECDsaWithSha512, 6, 3);
		AddCertSigAlgOid(dictionary, X9ObjectIdentifiers.IdDsaWithSha1, 2, 2);
		AddCertSigAlgOid(dictionary, EacObjectIdentifiers.id_TA_ECDSA_SHA_1, 2, 3);
		AddCertSigAlgOid(dictionary, EacObjectIdentifiers.id_TA_ECDSA_SHA_224, 3, 3);
		AddCertSigAlgOid(dictionary, EacObjectIdentifiers.id_TA_ECDSA_SHA_256, 4, 3);
		AddCertSigAlgOid(dictionary, EacObjectIdentifiers.id_TA_ECDSA_SHA_384, 5, 3);
		AddCertSigAlgOid(dictionary, EacObjectIdentifiers.id_TA_ECDSA_SHA_512, 6, 3);
		AddCertSigAlgOid(dictionary, EacObjectIdentifiers.id_TA_RSA_v1_5_SHA_1, 2, 1);
		AddCertSigAlgOid(dictionary, EacObjectIdentifiers.id_TA_RSA_v1_5_SHA_256, 4, 1);
		AddCertSigAlgOid(dictionary, BsiObjectIdentifiers.ecdsa_plain_SHA1, 2, 3);
		AddCertSigAlgOid(dictionary, BsiObjectIdentifiers.ecdsa_plain_SHA224, 3, 3);
		AddCertSigAlgOid(dictionary, BsiObjectIdentifiers.ecdsa_plain_SHA256, 4, 3);
		AddCertSigAlgOid(dictionary, BsiObjectIdentifiers.ecdsa_plain_SHA384, 5, 3);
		AddCertSigAlgOid(dictionary, BsiObjectIdentifiers.ecdsa_plain_SHA512, 6, 3);
		AddCertSigAlgOid(dictionary, EdECObjectIdentifiers.id_Ed25519, SignatureAndHashAlgorithm.ed25519);
		AddCertSigAlgOid(dictionary, EdECObjectIdentifiers.id_Ed448, SignatureAndHashAlgorithm.ed448);
		AddCertSigAlgOid(dictionary, RosstandartObjectIdentifiers.id_tc26_signwithdigest_gost_3410_12_256, SignatureAndHashAlgorithm.gostr34102012_256);
		AddCertSigAlgOid(dictionary, RosstandartObjectIdentifiers.id_tc26_signwithdigest_gost_3410_12_512, SignatureAndHashAlgorithm.gostr34102012_512);
		return dictionary;
	}

	private static IList<SignatureAndHashAlgorithm> CreateDefaultSupportedSigAlgs()
	{
		return new List<SignatureAndHashAlgorithm>
		{
			SignatureAndHashAlgorithm.ed25519,
			SignatureAndHashAlgorithm.ed448,
			SignatureAndHashAlgorithm.GetInstance(4, 3),
			SignatureAndHashAlgorithm.GetInstance(5, 3),
			SignatureAndHashAlgorithm.GetInstance(6, 3),
			SignatureAndHashAlgorithm.rsa_pss_rsae_sha256,
			SignatureAndHashAlgorithm.rsa_pss_rsae_sha384,
			SignatureAndHashAlgorithm.rsa_pss_rsae_sha512,
			SignatureAndHashAlgorithm.rsa_pss_pss_sha256,
			SignatureAndHashAlgorithm.rsa_pss_pss_sha384,
			SignatureAndHashAlgorithm.rsa_pss_pss_sha512,
			SignatureAndHashAlgorithm.GetInstance(4, 1),
			SignatureAndHashAlgorithm.GetInstance(5, 1),
			SignatureAndHashAlgorithm.GetInstance(6, 1),
			SignatureAndHashAlgorithm.GetInstance(4, 2),
			SignatureAndHashAlgorithm.GetInstance(5, 2),
			SignatureAndHashAlgorithm.GetInstance(6, 2),
			SignatureAndHashAlgorithm.GetInstance(3, 3),
			SignatureAndHashAlgorithm.GetInstance(3, 1),
			SignatureAndHashAlgorithm.GetInstance(3, 2),
			SignatureAndHashAlgorithm.GetInstance(2, 3),
			SignatureAndHashAlgorithm.GetInstance(2, 1),
			SignatureAndHashAlgorithm.GetInstance(2, 2)
		};
	}

	public static void CheckUint8(short i)
	{
		if (!IsValidUint8(i))
		{
			throw new TlsFatalAlert(80);
		}
	}

	public static void CheckUint8(int i)
	{
		if (!IsValidUint8(i))
		{
			throw new TlsFatalAlert(80);
		}
	}

	public static void CheckUint8(long i)
	{
		if (!IsValidUint8(i))
		{
			throw new TlsFatalAlert(80);
		}
	}

	public static void CheckUint16(int i)
	{
		if (!IsValidUint16(i))
		{
			throw new TlsFatalAlert(80);
		}
	}

	public static void CheckUint16(long i)
	{
		if (!IsValidUint16(i))
		{
			throw new TlsFatalAlert(80);
		}
	}

	public static void CheckUint24(int i)
	{
		if (!IsValidUint24(i))
		{
			throw new TlsFatalAlert(80);
		}
	}

	public static void CheckUint24(long i)
	{
		if (!IsValidUint24(i))
		{
			throw new TlsFatalAlert(80);
		}
	}

	public static void CheckUint32(long i)
	{
		if (!IsValidUint32(i))
		{
			throw new TlsFatalAlert(80);
		}
	}

	public static void CheckUint48(long i)
	{
		if (!IsValidUint48(i))
		{
			throw new TlsFatalAlert(80);
		}
	}

	public static void CheckUint64(long i)
	{
		if (!IsValidUint64(i))
		{
			throw new TlsFatalAlert(80);
		}
	}

	public static bool IsValidUint8(short i)
	{
		return (i & 0xFF) == i;
	}

	public static bool IsValidUint8(int i)
	{
		return (i & 0xFF) == i;
	}

	public static bool IsValidUint8(long i)
	{
		return (i & 0xFF) == i;
	}

	public static bool IsValidUint16(int i)
	{
		return (i & 0xFFFF) == i;
	}

	public static bool IsValidUint16(long i)
	{
		return (i & 0xFFFF) == i;
	}

	public static bool IsValidUint24(int i)
	{
		return (i & 0xFFFFFF) == i;
	}

	public static bool IsValidUint24(long i)
	{
		return (i & 0xFFFFFF) == i;
	}

	public static bool IsValidUint32(long i)
	{
		return (i & 0xFFFFFFFFu) == i;
	}

	public static bool IsValidUint48(long i)
	{
		return (i & 0xFFFFFFFFFFFFL) == i;
	}

	public static bool IsValidUint64(long i)
	{
		return true;
	}

	public static bool IsSsl(TlsContext context)
	{
		return context.ServerVersion.IsSsl;
	}

	public static bool IsTlsV10(ProtocolVersion version)
	{
		return ProtocolVersion.TLSv10.IsEqualOrEarlierVersionOf(version.GetEquivalentTlsVersion());
	}

	public static bool IsTlsV10(TlsContext context)
	{
		return IsTlsV10(context.ServerVersion);
	}

	public static bool IsTlsV11(ProtocolVersion version)
	{
		return ProtocolVersion.TLSv11.IsEqualOrEarlierVersionOf(version.GetEquivalentTlsVersion());
	}

	public static bool IsTlsV11(TlsContext context)
	{
		return IsTlsV11(context.ServerVersion);
	}

	public static bool IsTlsV12(ProtocolVersion version)
	{
		return ProtocolVersion.TLSv12.IsEqualOrEarlierVersionOf(version.GetEquivalentTlsVersion());
	}

	public static bool IsTlsV12(TlsContext context)
	{
		return IsTlsV12(context.ServerVersion);
	}

	public static bool IsTlsV13(ProtocolVersion version)
	{
		return ProtocolVersion.TLSv13.IsEqualOrEarlierVersionOf(version.GetEquivalentTlsVersion());
	}

	public static bool IsTlsV13(TlsContext context)
	{
		return IsTlsV13(context.ServerVersion);
	}

	public static void WriteUint8(short i, Stream output)
	{
		output.WriteByte((byte)i);
	}

	public static void WriteUint8(int i, Stream output)
	{
		output.WriteByte((byte)i);
	}

	public static void WriteUint8(short i, byte[] buf, int offset)
	{
		buf[offset] = (byte)i;
	}

	public static void WriteUint8(int i, byte[] buf, int offset)
	{
		buf[offset] = (byte)i;
	}

	public static void WriteUint16(int i, Stream output)
	{
		output.WriteByte((byte)(i >> 8));
		output.WriteByte((byte)i);
	}

	public static void WriteUint16(int i, byte[] buf, int offset)
	{
		buf[offset] = (byte)(i >> 8);
		buf[offset + 1] = (byte)i;
	}

	public static void WriteUint24(int i, Stream output)
	{
		output.WriteByte((byte)(i >> 16));
		output.WriteByte((byte)(i >> 8));
		output.WriteByte((byte)i);
	}

	public static void WriteUint24(int i, byte[] buf, int offset)
	{
		buf[offset] = (byte)(i >> 16);
		buf[offset + 1] = (byte)(i >> 8);
		buf[offset + 2] = (byte)i;
	}

	public static void WriteUint32(long i, Stream output)
	{
		output.WriteByte((byte)(i >> 24));
		output.WriteByte((byte)(i >> 16));
		output.WriteByte((byte)(i >> 8));
		output.WriteByte((byte)i);
	}

	public static void WriteUint32(long i, byte[] buf, int offset)
	{
		buf[offset] = (byte)(i >> 24);
		buf[offset + 1] = (byte)(i >> 16);
		buf[offset + 2] = (byte)(i >> 8);
		buf[offset + 3] = (byte)i;
	}

	public static void WriteUint48(long i, Stream output)
	{
		output.WriteByte((byte)(i >> 40));
		output.WriteByte((byte)(i >> 32));
		output.WriteByte((byte)(i >> 24));
		output.WriteByte((byte)(i >> 16));
		output.WriteByte((byte)(i >> 8));
		output.WriteByte((byte)i);
	}

	public static void WriteUint48(long i, byte[] buf, int offset)
	{
		buf[offset] = (byte)(i >> 40);
		buf[offset + 1] = (byte)(i >> 32);
		buf[offset + 2] = (byte)(i >> 24);
		buf[offset + 3] = (byte)(i >> 16);
		buf[offset + 4] = (byte)(i >> 8);
		buf[offset + 5] = (byte)i;
	}

	public static void WriteUint64(long i, Stream output)
	{
		output.WriteByte((byte)(i >> 56));
		output.WriteByte((byte)(i >> 48));
		output.WriteByte((byte)(i >> 40));
		output.WriteByte((byte)(i >> 32));
		output.WriteByte((byte)(i >> 24));
		output.WriteByte((byte)(i >> 16));
		output.WriteByte((byte)(i >> 8));
		output.WriteByte((byte)i);
	}

	public static void WriteUint64(long i, byte[] buf, int offset)
	{
		buf[offset] = (byte)(i >> 56);
		buf[offset + 1] = (byte)(i >> 48);
		buf[offset + 2] = (byte)(i >> 40);
		buf[offset + 3] = (byte)(i >> 32);
		buf[offset + 4] = (byte)(i >> 24);
		buf[offset + 5] = (byte)(i >> 16);
		buf[offset + 6] = (byte)(i >> 8);
		buf[offset + 7] = (byte)i;
	}

	public static void WriteOpaque8(byte[] buf, Stream output)
	{
		CheckUint8(buf.Length);
		WriteUint8(buf.Length, output);
		output.Write(buf, 0, buf.Length);
	}

	public static void WriteOpaque8(byte[] data, byte[] buf, int off)
	{
		CheckUint8(data.Length);
		WriteUint8(data.Length, buf, off);
		Array.Copy(data, 0, buf, off + 1, data.Length);
	}

	public static void WriteOpaque16(byte[] buf, Stream output)
	{
		CheckUint16(buf.Length);
		WriteUint16(buf.Length, output);
		output.Write(buf, 0, buf.Length);
	}

	public static void WriteOpaque16(byte[] data, byte[] buf, int off)
	{
		CheckUint16(data.Length);
		WriteUint16(data.Length, buf, off);
		Array.Copy(data, 0, buf, off + 2, data.Length);
	}

	public static void WriteOpaque24(byte[] buf, Stream output)
	{
		CheckUint24(buf.Length);
		WriteUint24(buf.Length, output);
		output.Write(buf, 0, buf.Length);
	}

	public static void WriteOpaque24(byte[] data, byte[] buf, int off)
	{
		CheckUint24(data.Length);
		WriteUint24(data.Length, buf, off);
		Array.Copy(data, 0, buf, off + 3, data.Length);
	}

	public static void WriteUint8Array(short[] u8s, Stream output)
	{
		for (int i = 0; i < u8s.Length; i++)
		{
			WriteUint8(u8s[i], output);
		}
	}

	public static void WriteUint8Array(short[] u8s, byte[] buf, int offset)
	{
		for (int i = 0; i < u8s.Length; i++)
		{
			WriteUint8(u8s[i], buf, offset);
			offset++;
		}
	}

	public static void WriteUint8ArrayWithUint8Length(short[] u8s, Stream output)
	{
		CheckUint8(u8s.Length);
		WriteUint8(u8s.Length, output);
		WriteUint8Array(u8s, output);
	}

	public static void WriteUint8ArrayWithUint8Length(short[] u8s, byte[] buf, int offset)
	{
		CheckUint8(u8s.Length);
		WriteUint8(u8s.Length, buf, offset);
		WriteUint8Array(u8s, buf, offset + 1);
	}

	public static void WriteUint16Array(int[] u16s, Stream output)
	{
		for (int i = 0; i < u16s.Length; i++)
		{
			WriteUint16(u16s[i], output);
		}
	}

	public static void WriteUint16Array(int[] u16s, byte[] buf, int offset)
	{
		for (int i = 0; i < u16s.Length; i++)
		{
			WriteUint16(u16s[i], buf, offset);
			offset += 2;
		}
	}

	public static void WriteUint16ArrayWithUint8Length(int[] u16s, byte[] buf, int offset)
	{
		int i = 2 * u16s.Length;
		CheckUint8(i);
		WriteUint8(i, buf, offset);
		WriteUint16Array(u16s, buf, offset + 1);
	}

	public static void WriteUint16ArrayWithUint16Length(int[] u16s, Stream output)
	{
		int i = 2 * u16s.Length;
		CheckUint16(i);
		WriteUint16(i, output);
		WriteUint16Array(u16s, output);
	}

	public static void WriteUint16ArrayWithUint16Length(int[] u16s, byte[] buf, int offset)
	{
		int i = 2 * u16s.Length;
		CheckUint16(i);
		WriteUint16(i, buf, offset);
		WriteUint16Array(u16s, buf, offset + 2);
	}

	public static byte[] DecodeOpaque8(byte[] buf)
	{
		return DecodeOpaque8(buf, 0);
	}

	public static byte[] DecodeOpaque8(byte[] buf, int minLength)
	{
		if (buf == null)
		{
			throw new ArgumentNullException("buf");
		}
		if (buf.Length < 1)
		{
			throw new TlsFatalAlert(50);
		}
		short num = ReadUint8(buf, 0);
		if (buf.Length != num + 1 || num < minLength)
		{
			throw new TlsFatalAlert(50);
		}
		return CopyOfRangeExact(buf, 1, buf.Length);
	}

	public static byte[] DecodeOpaque16(byte[] buf)
	{
		return DecodeOpaque16(buf, 0);
	}

	public static byte[] DecodeOpaque16(byte[] buf, int minLength)
	{
		if (buf == null)
		{
			throw new ArgumentNullException("buf");
		}
		if (buf.Length < 2)
		{
			throw new TlsFatalAlert(50);
		}
		int num = ReadUint16(buf, 0);
		if (buf.Length != num + 2 || num < minLength)
		{
			throw new TlsFatalAlert(50);
		}
		return CopyOfRangeExact(buf, 2, buf.Length);
	}

	public static short DecodeUint8(byte[] buf)
	{
		if (buf == null)
		{
			throw new ArgumentNullException("buf");
		}
		if (buf.Length != 1)
		{
			throw new TlsFatalAlert(50);
		}
		return ReadUint8(buf, 0);
	}

	public static short[] DecodeUint8ArrayWithUint8Length(byte[] buf)
	{
		if (buf == null)
		{
			throw new ArgumentNullException("buf");
		}
		if (buf.Length < 1)
		{
			throw new TlsFatalAlert(50);
		}
		int num = ReadUint8(buf, 0);
		if (buf.Length != num + 1)
		{
			throw new TlsFatalAlert(50);
		}
		short[] array = new short[num];
		for (int i = 0; i < num; i++)
		{
			array[i] = ReadUint8(buf, i + 1);
		}
		return array;
	}

	public static int DecodeUint16(byte[] buf)
	{
		if (buf == null)
		{
			throw new ArgumentNullException("buf");
		}
		if (buf.Length != 2)
		{
			throw new TlsFatalAlert(50);
		}
		return ReadUint16(buf, 0);
	}

	public static int[] DecodeUint16ArrayWithUint8Length(byte[] buf)
	{
		if (buf == null)
		{
			throw new ArgumentNullException("buf");
		}
		int num = ReadUint8(buf, 0);
		if (buf.Length != num + 1 || (num & 1) != 0)
		{
			throw new TlsFatalAlert(50);
		}
		int num2 = num / 2;
		int num3 = 1;
		int[] array = new int[num2];
		for (int i = 0; i < num2; i++)
		{
			array[i] = ReadUint16(buf, num3);
			num3 += 2;
		}
		return array;
	}

	public static long DecodeUint32(byte[] buf)
	{
		if (buf == null)
		{
			throw new ArgumentNullException("buf");
		}
		if (buf.Length != 4)
		{
			throw new TlsFatalAlert(50);
		}
		return ReadUint32(buf, 0);
	}

	public static byte[] EncodeOpaque8(byte[] buf)
	{
		CheckUint8(buf.Length);
		return Arrays.Prepend(buf, (byte)buf.Length);
	}

	public static byte[] EncodeOpaque16(byte[] buf)
	{
		CheckUint16(buf.Length);
		byte[] array = new byte[2 + buf.Length];
		WriteUint16(buf.Length, array, 0);
		Array.Copy(buf, 0, array, 2, buf.Length);
		return array;
	}

	public static byte[] EncodeOpaque24(byte[] buf)
	{
		CheckUint24(buf.Length);
		byte[] array = new byte[3 + buf.Length];
		WriteUint24(buf.Length, array, 0);
		Array.Copy(buf, 0, array, 3, buf.Length);
		return array;
	}

	public static byte[] EncodeUint8(short u8)
	{
		CheckUint8(u8);
		byte[] array = new byte[1];
		WriteUint8(u8, array, 0);
		return array;
	}

	public static byte[] EncodeUint8ArrayWithUint8Length(short[] u8s)
	{
		byte[] array = new byte[1 + u8s.Length];
		WriteUint8ArrayWithUint8Length(u8s, array, 0);
		return array;
	}

	public static byte[] EncodeUint16(int u16)
	{
		CheckUint16(u16);
		byte[] array = new byte[2];
		WriteUint16(u16, array, 0);
		return array;
	}

	public static byte[] EncodeUint16ArrayWithUint8Length(int[] u16s)
	{
		int num = 2 * u16s.Length;
		byte[] array = new byte[1 + num];
		WriteUint16ArrayWithUint8Length(u16s, array, 0);
		return array;
	}

	public static byte[] EncodeUint16ArrayWithUint16Length(int[] u16s)
	{
		int num = 2 * u16s.Length;
		byte[] array = new byte[2 + num];
		WriteUint16ArrayWithUint16Length(u16s, array, 0);
		return array;
	}

	public static byte[] EncodeUint24(int u24)
	{
		CheckUint24(u24);
		byte[] array = new byte[3];
		WriteUint24(u24, array, 0);
		return array;
	}

	public static byte[] EncodeUint32(long u32)
	{
		CheckUint32(u32);
		byte[] array = new byte[4];
		WriteUint32(u32, array, 0);
		return array;
	}

	public static byte[] EncodeVersion(ProtocolVersion version)
	{
		return new byte[2]
		{
			(byte)version.MajorVersion,
			(byte)version.MinorVersion
		};
	}

	public static int ReadInt32(byte[] buf, int offset)
	{
		return (buf[offset] << 24) | ((buf[++offset] & 0xFF) << 16) | ((buf[++offset] & 0xFF) << 8) | (buf[++offset] & 0xFF);
	}

	public static short ReadUint8(Stream input)
	{
		int num = input.ReadByte();
		if (num < 0)
		{
			throw new EndOfStreamException();
		}
		return (short)num;
	}

	public static short ReadUint8(byte[] buf, int offset)
	{
		return buf[offset];
	}

	public static int ReadUint16(Stream input)
	{
		int num = input.ReadByte();
		int num2 = input.ReadByte();
		if (num2 < 0)
		{
			throw new EndOfStreamException();
		}
		return (num << 8) | num2;
	}

	public static int ReadUint16(byte[] buf, int offset)
	{
		return (buf[offset] << 8) | buf[++offset];
	}

	public static int ReadUint24(Stream input)
	{
		int num = input.ReadByte();
		int num2 = input.ReadByte();
		int num3 = input.ReadByte();
		if (num3 < 0)
		{
			throw new EndOfStreamException();
		}
		return (num << 16) | (num2 << 8) | num3;
	}

	public static int ReadUint24(byte[] buf, int offset)
	{
		return ((buf[offset] & 0xFF) << 16) | ((buf[++offset] & 0xFF) << 8) | (buf[++offset] & 0xFF);
	}

	public static long ReadUint32(Stream input)
	{
		int num = input.ReadByte();
		int num2 = input.ReadByte();
		int num3 = input.ReadByte();
		int num4 = input.ReadByte();
		if (num4 < 0)
		{
			throw new EndOfStreamException();
		}
		return ((num << 24) | (num2 << 16) | (num3 << 8) | num4) & 0xFFFFFFFFu;
	}

	public static long ReadUint32(byte[] buf, int offset)
	{
		return (((buf[offset] & 0xFF) << 24) | ((buf[++offset] & 0xFF) << 16) | ((buf[++offset] & 0xFF) << 8) | (buf[++offset] & 0xFF)) & 0xFFFFFFFFu;
	}

	public static long ReadUint48(Stream input)
	{
		int num = ReadUint24(input);
		int num2 = ReadUint24(input);
		return ((num & 0xFFFFFFFFu) << 24) | (num2 & 0xFFFFFFFFu);
	}

	public static long ReadUint48(byte[] buf, int offset)
	{
		int num = ReadUint24(buf, offset);
		int num2 = ReadUint24(buf, offset + 3);
		return ((num & 0xFFFFFFFFu) << 24) | (num2 & 0xFFFFFFFFu);
	}

	public static byte[] ReadAllOrNothing(int length, Stream input)
	{
		if (length < 1)
		{
			return EmptyBytes;
		}
		byte[] array = new byte[length];
		int num = Streams.ReadFully(input, array);
		if (num == 0)
		{
			return null;
		}
		if (num != length)
		{
			throw new EndOfStreamException();
		}
		return array;
	}

	public static byte[] ReadFully(int length, Stream input)
	{
		if (length < 1)
		{
			return EmptyBytes;
		}
		byte[] array = new byte[length];
		if (length != Streams.ReadFully(input, array))
		{
			throw new EndOfStreamException();
		}
		return array;
	}

	public static void ReadFully(byte[] buf, Stream input)
	{
		int num = buf.Length;
		if (num > 0 && num != Streams.ReadFully(input, buf))
		{
			throw new EndOfStreamException();
		}
	}

	public static byte[] ReadOpaque8(Stream input)
	{
		return ReadFully(ReadUint8(input), input);
	}

	public static byte[] ReadOpaque8(Stream input, int minLength)
	{
		short num = ReadUint8(input);
		if (num < minLength)
		{
			throw new TlsFatalAlert(50);
		}
		return ReadFully(num, input);
	}

	public static byte[] ReadOpaque8(Stream input, int minLength, int maxLength)
	{
		short num = ReadUint8(input);
		if (num < minLength || maxLength < num)
		{
			throw new TlsFatalAlert(50);
		}
		return ReadFully(num, input);
	}

	public static byte[] ReadOpaque16(Stream input)
	{
		return ReadFully(ReadUint16(input), input);
	}

	public static byte[] ReadOpaque16(Stream input, int minLength)
	{
		int num = ReadUint16(input);
		if (num < minLength)
		{
			throw new TlsFatalAlert(50);
		}
		return ReadFully(num, input);
	}

	public static byte[] ReadOpaque24(Stream input)
	{
		return ReadFully(ReadUint24(input), input);
	}

	public static byte[] ReadOpaque24(Stream input, int minLength)
	{
		int num = ReadUint24(input);
		if (num < minLength)
		{
			throw new TlsFatalAlert(50);
		}
		return ReadFully(num, input);
	}

	public static short[] ReadUint8Array(int count, Stream input)
	{
		short[] array = new short[count];
		for (int i = 0; i < count; i++)
		{
			array[i] = ReadUint8(input);
		}
		return array;
	}

	public static short[] ReadUint8ArrayWithUint8Length(Stream input, int minLength)
	{
		short num = ReadUint8(input);
		if (num < minLength)
		{
			throw new TlsFatalAlert(50);
		}
		return ReadUint8Array(num, input);
	}

	public static int[] ReadUint16Array(int count, Stream input)
	{
		int[] array = new int[count];
		for (int i = 0; i < count; i++)
		{
			array[i] = ReadUint16(input);
		}
		return array;
	}

	public static ProtocolVersion ReadVersion(byte[] buf, int offset)
	{
		return ProtocolVersion.Get(buf[offset], buf[offset + 1]);
	}

	public static ProtocolVersion ReadVersion(Stream input)
	{
		int major = input.ReadByte();
		int num = input.ReadByte();
		if (num < 0)
		{
			throw new EndOfStreamException();
		}
		return ProtocolVersion.Get(major, num);
	}

	public static Asn1Object ReadAsn1Object(byte[] encoding)
	{
		using Asn1InputStream asn1InputStream = new Asn1InputStream(encoding);
		Asn1Object asn1Object = asn1InputStream.ReadObject();
		if (asn1Object == null)
		{
			throw new TlsFatalAlert(50);
		}
		if (encoding.Length != asn1InputStream.Position)
		{
			throw new TlsFatalAlert(50);
		}
		return asn1Object;
	}

	public static void RequireDerEncoding(Asn1Encodable asn1, byte[] encoding)
	{
		if (!Arrays.AreEqual(asn1.GetEncoded("DER"), encoding))
		{
			throw new TlsFatalAlert(50);
		}
	}

	public static void WriteGmtUnixTime(byte[] buf, int offset)
	{
		int num = (int)(DateTimeUtilities.CurrentUnixMs() / 1000);
		buf[offset] = (byte)(num >> 24);
		buf[offset + 1] = (byte)(num >> 16);
		buf[offset + 2] = (byte)(num >> 8);
		buf[offset + 3] = (byte)num;
	}

	public static void WriteVersion(ProtocolVersion version, Stream output)
	{
		output.WriteByte((byte)version.MajorVersion);
		output.WriteByte((byte)version.MinorVersion);
	}

	public static void WriteVersion(ProtocolVersion version, byte[] buf, int offset)
	{
		buf[offset] = (byte)version.MajorVersion;
		buf[offset + 1] = (byte)version.MinorVersion;
	}

	public static void AddIfSupported(IList<SignatureAndHashAlgorithm> supportedAlgs, TlsCrypto crypto, SignatureAndHashAlgorithm alg)
	{
		if (crypto.HasSignatureAndHashAlgorithm(alg))
		{
			supportedAlgs.Add(alg);
		}
	}

	public static void AddIfSupported(IList<int> supportedGroups, TlsCrypto crypto, int namedGroup)
	{
		if (crypto.HasNamedGroup(namedGroup))
		{
			supportedGroups.Add(namedGroup);
		}
	}

	public static void AddIfSupported(IList<int> supportedGroups, TlsCrypto crypto, int[] namedGroups)
	{
		for (int i = 0; i < namedGroups.Length; i++)
		{
			AddIfSupported(supportedGroups, crypto, namedGroups[i]);
		}
	}

	public static bool AddToSet<T>(IList<T> s, T i)
	{
		bool num = !s.Contains(i);
		if (num)
		{
			s.Add(i);
		}
		return num;
	}

	public static IList<SignatureAndHashAlgorithm> GetDefaultDssSignatureAlgorithms()
	{
		return GetDefaultSignatureAlgorithms(2);
	}

	public static IList<SignatureAndHashAlgorithm> GetDefaultECDsaSignatureAlgorithms()
	{
		return GetDefaultSignatureAlgorithms(3);
	}

	public static IList<SignatureAndHashAlgorithm> GetDefaultRsaSignatureAlgorithms()
	{
		return GetDefaultSignatureAlgorithms(1);
	}

	public static SignatureAndHashAlgorithm GetDefaultSignatureAlgorithm(short signatureAlgorithm)
	{
		if ((uint)(signatureAlgorithm - 1) <= 2u)
		{
			return SignatureAndHashAlgorithm.GetInstance(2, signatureAlgorithm);
		}
		return null;
	}

	public static IList<SignatureAndHashAlgorithm> GetDefaultSignatureAlgorithms(short signatureAlgorithm)
	{
		SignatureAndHashAlgorithm defaultSignatureAlgorithm = GetDefaultSignatureAlgorithm(signatureAlgorithm);
		if (defaultSignatureAlgorithm != null)
		{
			return VectorOfOne(defaultSignatureAlgorithm);
		}
		return new List<SignatureAndHashAlgorithm>();
	}

	public static IList<SignatureAndHashAlgorithm> GetDefaultSupportedSignatureAlgorithms(TlsContext context)
	{
		return GetSupportedSignatureAlgorithms(context, DefaultSupportedSigAlgs);
	}

	public static IList<SignatureAndHashAlgorithm> GetSupportedSignatureAlgorithms(TlsContext context, IList<SignatureAndHashAlgorithm> candidates)
	{
		TlsCrypto crypto = context.Crypto;
		List<SignatureAndHashAlgorithm> list = new List<SignatureAndHashAlgorithm>(candidates.Count);
		foreach (SignatureAndHashAlgorithm candidate in candidates)
		{
			AddIfSupported(list, crypto, candidate);
		}
		return list;
	}

	internal static SignatureAndHashAlgorithm GetSignatureAndHashAlgorithm(ProtocolVersion negotiatedVersion, TlsCredentialedSigner credentialedSigner)
	{
		SignatureAndHashAlgorithm signatureAndHashAlgorithm = null;
		if (IsSignatureAlgorithmsExtensionAllowed(negotiatedVersion))
		{
			signatureAndHashAlgorithm = credentialedSigner.SignatureAndHashAlgorithm;
			if (signatureAndHashAlgorithm == null)
			{
				throw new TlsFatalAlert(80);
			}
		}
		return signatureAndHashAlgorithm;
	}

	public static byte[] GetExtensionData(IDictionary<int, byte[]> extensions, int extensionType)
	{
		if (extensions == null || !extensions.TryGetValue(extensionType, out var value))
		{
			return null;
		}
		return value;
	}

	public static bool HasExpectedEmptyExtensionData(IDictionary<int, byte[]> extensions, int extensionType, short alertDescription)
	{
		byte[] extensionData = GetExtensionData(extensions, extensionType);
		if (extensionData == null)
		{
			return false;
		}
		if (extensionData.Length != 0)
		{
			throw new TlsFatalAlert(alertDescription);
		}
		return true;
	}

	public static TlsSession ImportSession(byte[] sessionID, SessionParameters sessionParameters)
	{
		return new TlsSessionImpl(sessionID, sessionParameters);
	}

	internal static bool IsExtendedMasterSecretOptional(ProtocolVersion protocolVersion)
	{
		ProtocolVersion equivalentTlsVersion = protocolVersion.GetEquivalentTlsVersion();
		if (!ProtocolVersion.TLSv12.Equals(equivalentTlsVersion) && !ProtocolVersion.TLSv11.Equals(equivalentTlsVersion))
		{
			return ProtocolVersion.TLSv10.Equals(equivalentTlsVersion);
		}
		return true;
	}

	internal static bool IsExtendedMasterSecretOptional(ProtocolVersion[] protocolVersions)
	{
		if (protocolVersions != null)
		{
			for (int i = 0; i < protocolVersions.Length; i++)
			{
				if (IsExtendedMasterSecretOptional(protocolVersions[i]))
				{
					return true;
				}
			}
		}
		return false;
	}

	public static bool IsNullOrContainsNull(object[] array)
	{
		if (array == null)
		{
			return true;
		}
		int num = array.Length;
		for (int i = 0; i < num; i++)
		{
			if (array[i] == null)
			{
				return true;
			}
		}
		return false;
	}

	public static bool IsNullOrEmpty<T>(T[] array)
	{
		if (array != null)
		{
			return array.Length < 1;
		}
		return true;
	}

	public static bool IsNullOrEmpty(string s)
	{
		if (s != null)
		{
			return s.Length < 1;
		}
		return true;
	}

	public static bool IsNullOrEmpty<T>(IList<T> v)
	{
		if (v != null)
		{
			return v.Count < 1;
		}
		return true;
	}

	public static bool IsSignatureAlgorithmsExtensionAllowed(ProtocolVersion version)
	{
		if (version != null)
		{
			return ProtocolVersion.TLSv12.IsEqualOrEarlierVersionOf(version.GetEquivalentTlsVersion());
		}
		return false;
	}

	public static short GetLegacyClientCertType(short signatureAlgorithm)
	{
		return signatureAlgorithm switch
		{
			1 => 1, 
			2 => 2, 
			3 => 64, 
			_ => -1, 
		};
	}

	public static short GetLegacySignatureAlgorithmClient(short clientCertificateType)
	{
		return clientCertificateType switch
		{
			2 => 2, 
			64 => 3, 
			1 => 1, 
			_ => -1, 
		};
	}

	public static short GetLegacySignatureAlgorithmClientCert(short clientCertificateType)
	{
		switch (clientCertificateType)
		{
		case 2:
		case 4:
			return 2;
		case 64:
		case 66:
			return 3;
		case 1:
		case 3:
		case 65:
			return 1;
		default:
			return -1;
		}
	}

	public static short GetLegacySignatureAlgorithmServer(int keyExchangeAlgorithm)
	{
		switch (keyExchangeAlgorithm)
		{
		case 3:
		case 22:
			return 2;
		case 17:
			return 3;
		case 5:
		case 19:
		case 23:
			return 1;
		default:
			return -1;
		}
	}

	public static short GetLegacySignatureAlgorithmServerCert(int keyExchangeAlgorithm)
	{
		switch (keyExchangeAlgorithm)
		{
		case 3:
		case 7:
		case 22:
			return 2;
		case 16:
		case 17:
			return 3;
		case 1:
		case 5:
		case 9:
		case 15:
		case 18:
		case 19:
		case 23:
			return 1;
		default:
			return -1;
		}
	}

	public static IList<SignatureAndHashAlgorithm> GetLegacySupportedSignatureAlgorithms()
	{
		return new List<SignatureAndHashAlgorithm>(3)
		{
			SignatureAndHashAlgorithm.GetInstance(2, 2),
			SignatureAndHashAlgorithm.GetInstance(2, 3),
			SignatureAndHashAlgorithm.GetInstance(2, 1)
		};
	}

	public static void EncodeSupportedSignatureAlgorithms(IList<SignatureAndHashAlgorithm> supportedSignatureAlgorithms, Stream output)
	{
		if (supportedSignatureAlgorithms == null || supportedSignatureAlgorithms.Count < 1 || supportedSignatureAlgorithms.Count >= 32768)
		{
			throw new ArgumentException("must have length from 1 to (2^15 - 1)", "supportedSignatureAlgorithms");
		}
		int i = 2 * supportedSignatureAlgorithms.Count;
		CheckUint16(i);
		WriteUint16(i, output);
		foreach (SignatureAndHashAlgorithm supportedSignatureAlgorithm in supportedSignatureAlgorithms)
		{
			if (supportedSignatureAlgorithm.Signature == 0)
			{
				throw new ArgumentException("SignatureAlgorithm.anonymous MUST NOT appear in the signature_algorithms extension");
			}
			supportedSignatureAlgorithm.Encode(output);
		}
	}

	public static IList<SignatureAndHashAlgorithm> ParseSupportedSignatureAlgorithms(Stream input)
	{
		int num = ReadUint16(input);
		if (num < 2 || (num & 1) != 0)
		{
			throw new TlsFatalAlert(50);
		}
		int num2 = num / 2;
		List<SignatureAndHashAlgorithm> list = new List<SignatureAndHashAlgorithm>(num2);
		for (int i = 0; i < num2; i++)
		{
			SignatureAndHashAlgorithm signatureAndHashAlgorithm = SignatureAndHashAlgorithm.Parse(input);
			if (signatureAndHashAlgorithm.Signature != 0)
			{
				list.Add(signatureAndHashAlgorithm);
			}
		}
		return list;
	}

	public static void VerifySupportedSignatureAlgorithm(IList<SignatureAndHashAlgorithm> supportedSignatureAlgorithms, SignatureAndHashAlgorithm signatureAlgorithm)
	{
		VerifySupportedSignatureAlgorithm(supportedSignatureAlgorithms, signatureAlgorithm, 47);
	}

	internal static void VerifySupportedSignatureAlgorithm(IList<SignatureAndHashAlgorithm> supportedSignatureAlgorithms, SignatureAndHashAlgorithm signatureAlgorithm, short alertDescription)
	{
		if (supportedSignatureAlgorithms == null || supportedSignatureAlgorithms.Count < 1 || supportedSignatureAlgorithms.Count >= 32768)
		{
			throw new ArgumentException("must have length from 1 to (2^15 - 1)", "supportedSignatureAlgorithms");
		}
		if (signatureAlgorithm == null)
		{
			throw new ArgumentNullException("signatureAlgorithm");
		}
		if (signatureAlgorithm.Signature == 0 || !ContainsSignatureAlgorithm(supportedSignatureAlgorithms, signatureAlgorithm))
		{
			throw new TlsFatalAlert(alertDescription);
		}
	}

	public static bool ContainsSignatureAlgorithm(IList<SignatureAndHashAlgorithm> supportedSignatureAlgorithms, SignatureAndHashAlgorithm signatureAlgorithm)
	{
		foreach (SignatureAndHashAlgorithm supportedSignatureAlgorithm in supportedSignatureAlgorithms)
		{
			if (supportedSignatureAlgorithm.Equals(signatureAlgorithm))
			{
				return true;
			}
		}
		return false;
	}

	public static bool ContainsAnySignatureAlgorithm(IList<SignatureAndHashAlgorithm> supportedSignatureAlgorithms, short signatureAlgorithm)
	{
		foreach (SignatureAndHashAlgorithm supportedSignatureAlgorithm in supportedSignatureAlgorithms)
		{
			if (supportedSignatureAlgorithm.Signature == signatureAlgorithm)
			{
				return true;
			}
		}
		return false;
	}

	public static TlsSecret Prf(SecurityParameters securityParameters, TlsSecret secret, string asciiLabel, byte[] seed, int length)
	{
		return secret.DeriveUsingPrf(securityParameters.PrfAlgorithm, asciiLabel, seed, length);
	}

	public static byte[] Clone(byte[] data)
	{
		if (data != null)
		{
			if (data.Length != 0)
			{
				return (byte[])data.Clone();
			}
			return EmptyBytes;
		}
		return null;
	}

	public static string[] Clone(string[] s)
	{
		if (s != null)
		{
			if (s.Length >= 1)
			{
				return (string[])s.Clone();
			}
			return EmptyStrings;
		}
		return null;
	}

	public static bool ConstantTimeAreEqual(int len, byte[] a, int aOff, byte[] b, int bOff)
	{
		int num = 0;
		for (int i = 0; i < len; i++)
		{
			num |= a[aOff + i] ^ b[bOff + i];
		}
		return num == 0;
	}

	public static byte[] CopyOfRangeExact(byte[] original, int from, int to)
	{
		int num = to - from;
		byte[] array = new byte[num];
		Array.Copy(original, from, array, 0, num);
		return array;
	}

	internal static byte[] Concat(byte[] a, byte[] b)
	{
		byte[] array = new byte[a.Length + b.Length];
		Array.Copy(a, 0, array, 0, a.Length);
		Array.Copy(b, 0, array, a.Length, b.Length);
		return array;
	}

	internal static byte[] CalculateEndPointHash(TlsContext context, TlsCertificate certificate, byte[] enc)
	{
		return CalculateEndPointHash(context, certificate, enc, 0, enc.Length);
	}

	internal static byte[] CalculateEndPointHash(TlsContext context, TlsCertificate certificate, byte[] enc, int encOff, int encLen)
	{
		short num = 0;
		string sigAlgOid = certificate.SigAlgOid;
		if (sigAlgOid != null)
		{
			SignatureAndHashAlgorithm value;
			if (PkcsObjectIdentifiers.IdRsassaPss.Id.Equals(sigAlgOid))
			{
				RsassaPssParameters instance = RsassaPssParameters.GetInstance(certificate.GetSigAlgParams());
				if (instance != null)
				{
					DerObjectIdentifier algorithm = instance.HashAlgorithm.Algorithm;
					if (NistObjectIdentifiers.IdSha256.Equals(algorithm))
					{
						num = 4;
					}
					else if (NistObjectIdentifiers.IdSha384.Equals(algorithm))
					{
						num = 5;
					}
					else if (NistObjectIdentifiers.IdSha512.Equals(algorithm))
					{
						num = 6;
					}
				}
			}
			else if (CertSigAlgOids.TryGetValue(sigAlgOid, out value))
			{
				num = value.Hash;
			}
		}
		switch (num)
		{
		case 8:
			num = 0;
			break;
		case 1:
		case 2:
			num = 4;
			break;
		}
		if (num != 0)
		{
			TlsHash tlsHash = CreateHash(context.Crypto, num);
			if (tlsHash != null)
			{
				tlsHash.Update(enc, encOff, encLen);
				return tlsHash.CalculateHash();
			}
		}
		return EmptyBytes;
	}

	public static byte[] CalculateExporterSeed(SecurityParameters securityParameters, byte[] context)
	{
		byte[] clientRandom = securityParameters.ClientRandom;
		byte[] serverRandom = securityParameters.ServerRandom;
		if (context == null)
		{
			return Arrays.Concatenate(clientRandom, serverRandom);
		}
		if (!IsValidUint16(context.Length))
		{
			throw new ArgumentException("must have length less than 2^16 (or be null)", "context");
		}
		byte[] array = new byte[2];
		WriteUint16(context.Length, array, 0);
		return Arrays.ConcatenateAll(clientRandom, serverRandom, array, context);
	}

	private static byte[] CalculateFinishedHmac(SecurityParameters securityParameters, TlsSecret baseKey, byte[] transcriptHash)
	{
		int prfCryptoHashAlgorithm = securityParameters.PrfCryptoHashAlgorithm;
		int prfHashLength = securityParameters.PrfHashLength;
		return CalculateFinishedHmac(prfCryptoHashAlgorithm, prfHashLength, baseKey, transcriptHash);
	}

	private static byte[] CalculateFinishedHmac(int prfCryptoHashAlgorithm, int prfHashLength, TlsSecret baseKey, byte[] transcriptHash)
	{
		TlsSecret tlsSecret = TlsCryptoUtilities.HkdfExpandLabel(baseKey, prfCryptoHashAlgorithm, "finished", EmptyBytes, prfHashLength);
		try
		{
			return tlsSecret.CalculateHmac(prfCryptoHashAlgorithm, transcriptHash, 0, transcriptHash.Length);
		}
		finally
		{
			tlsSecret.Destroy();
		}
	}

	internal static TlsSecret CalculateMasterSecret(TlsContext context, TlsSecret preMasterSecret)
	{
		SecurityParameters securityParameters = context.SecurityParameters;
		string asciiLabel;
		byte[] seed;
		if (securityParameters.IsExtendedMasterSecret)
		{
			asciiLabel = "extended master secret";
			seed = securityParameters.SessionHash;
		}
		else
		{
			asciiLabel = "master secret";
			seed = Concat(securityParameters.ClientRandom, securityParameters.ServerRandom);
		}
		return Prf(securityParameters, preMasterSecret, asciiLabel, seed, 48);
	}

	internal static byte[] CalculatePskBinder(TlsCrypto crypto, bool isExternalPsk, int pskCryptoHashAlgorithm, TlsSecret earlySecret, byte[] transcriptHash)
	{
		int hashOutputSize = TlsCryptoUtilities.GetHashOutputSize(pskCryptoHashAlgorithm);
		string label = (isExternalPsk ? "ext binder" : "res binder");
		byte[] transcriptHash2 = crypto.CreateHash(pskCryptoHashAlgorithm).CalculateHash();
		TlsSecret tlsSecret = DeriveSecret(pskCryptoHashAlgorithm, hashOutputSize, earlySecret, label, transcriptHash2);
		try
		{
			return CalculateFinishedHmac(pskCryptoHashAlgorithm, hashOutputSize, tlsSecret, transcriptHash);
		}
		finally
		{
			tlsSecret.Destroy();
		}
	}

	internal static byte[] CalculateVerifyData(TlsContext context, TlsHandshakeHash handshakeHash, bool isServer)
	{
		SecurityParameters securityParameters = context.SecurityParameters;
		ProtocolVersion negotiatedVersion = securityParameters.NegotiatedVersion;
		if (IsTlsV13(negotiatedVersion))
		{
			TlsSecret baseKey = (isServer ? securityParameters.BaseKeyServer : securityParameters.BaseKeyClient);
			byte[] currentPrfHash = GetCurrentPrfHash(handshakeHash);
			return CalculateFinishedHmac(securityParameters, baseKey, currentPrfHash);
		}
		if (negotiatedVersion.IsSsl)
		{
			return Ssl3Utilities.CalculateVerifyData(handshakeHash, isServer);
		}
		string asciiLabel = (isServer ? "server finished" : "client finished");
		byte[] currentPrfHash2 = GetCurrentPrfHash(handshakeHash);
		TlsSecret masterSecret = securityParameters.MasterSecret;
		int verifyDataLength = securityParameters.VerifyDataLength;
		return Prf(securityParameters, masterSecret, asciiLabel, currentPrfHash2, verifyDataLength).Extract();
	}

	internal static void Establish13PhaseSecrets(TlsContext context, TlsSecret pskEarlySecret, TlsSecret sharedSecret)
	{
		TlsCrypto crypto = context.Crypto;
		SecurityParameters securityParameters = context.SecurityParameters;
		int prfCryptoHashAlgorithm = securityParameters.PrfCryptoHashAlgorithm;
		TlsSecret tlsSecret = crypto.HkdfInit(prfCryptoHashAlgorithm);
		byte[] transcriptHash = crypto.CreateHash(prfCryptoHashAlgorithm).CalculateHash();
		TlsSecret tlsSecret2 = pskEarlySecret;
		if (tlsSecret2 == null)
		{
			tlsSecret2 = crypto.HkdfInit(prfCryptoHashAlgorithm).HkdfExtract(prfCryptoHashAlgorithm, tlsSecret);
		}
		if (sharedSecret == null)
		{
			sharedSecret = tlsSecret;
		}
		TlsSecret tlsSecret3 = DeriveSecret(securityParameters, tlsSecret2, "derived", transcriptHash).HkdfExtract(prfCryptoHashAlgorithm, sharedSecret);
		if (sharedSecret != tlsSecret)
		{
			sharedSecret.Destroy();
		}
		TlsSecret masterSecret = DeriveSecret(securityParameters, tlsSecret3, "derived", transcriptHash).HkdfExtract(prfCryptoHashAlgorithm, tlsSecret);
		securityParameters.m_earlySecret = tlsSecret2;
		securityParameters.m_handshakeSecret = tlsSecret3;
		securityParameters.m_masterSecret = masterSecret;
	}

	private static void Establish13TrafficSecrets(TlsContext context, byte[] transcriptHash, TlsSecret phaseSecret, string clientLabel, string serverLabel, RecordStream recordStream)
	{
		SecurityParameters securityParameters = context.SecurityParameters;
		securityParameters.m_trafficSecretClient = DeriveSecret(securityParameters, phaseSecret, clientLabel, transcriptHash);
		if (serverLabel != null)
		{
			securityParameters.m_trafficSecretServer = DeriveSecret(securityParameters, phaseSecret, serverLabel, transcriptHash);
		}
		recordStream.SetPendingCipher(InitCipher(context));
	}

	internal static void Establish13PhaseApplication(TlsContext context, byte[] serverFinishedTranscriptHash, RecordStream recordStream)
	{
		SecurityParameters securityParameters = context.SecurityParameters;
		TlsSecret masterSecret = securityParameters.MasterSecret;
		Establish13TrafficSecrets(context, serverFinishedTranscriptHash, masterSecret, "c ap traffic", "s ap traffic", recordStream);
		securityParameters.m_exporterMasterSecret = DeriveSecret(securityParameters, masterSecret, "exp master", serverFinishedTranscriptHash);
	}

	internal static void Establish13PhaseEarly(TlsContext context, byte[] clientHelloTranscriptHash, RecordStream recordStream)
	{
		SecurityParameters securityParameters = context.SecurityParameters;
		TlsSecret earlySecret = securityParameters.EarlySecret;
		if (recordStream != null)
		{
			Establish13TrafficSecrets(context, clientHelloTranscriptHash, earlySecret, "c e traffic", null, recordStream);
		}
		securityParameters.m_earlyExporterMasterSecret = DeriveSecret(securityParameters, earlySecret, "e exp master", clientHelloTranscriptHash);
	}

	internal static void Establish13PhaseHandshake(TlsContext context, byte[] serverHelloTranscriptHash, RecordStream recordStream)
	{
		SecurityParameters securityParameters = context.SecurityParameters;
		TlsSecret handshakeSecret = securityParameters.HandshakeSecret;
		Establish13TrafficSecrets(context, serverHelloTranscriptHash, handshakeSecret, "c hs traffic", "s hs traffic", recordStream);
		securityParameters.m_baseKeyClient = securityParameters.TrafficSecretClient;
		securityParameters.m_baseKeyServer = securityParameters.TrafficSecretServer;
	}

	internal static void Update13TrafficSecretLocal(TlsContext context)
	{
		Update13TrafficSecret(context, context.IsServer);
	}

	internal static void Update13TrafficSecretPeer(TlsContext context)
	{
		Update13TrafficSecret(context, !context.IsServer);
	}

	private static void Update13TrafficSecret(TlsContext context, bool forServer)
	{
		SecurityParameters securityParameters = context.SecurityParameters;
		TlsSecret tlsSecret;
		if (forServer)
		{
			tlsSecret = securityParameters.TrafficSecretServer;
			securityParameters.m_trafficSecretServer = Update13TrafficSecret(securityParameters, tlsSecret);
		}
		else
		{
			tlsSecret = securityParameters.TrafficSecretClient;
			securityParameters.m_trafficSecretClient = Update13TrafficSecret(securityParameters, tlsSecret);
		}
		tlsSecret?.Destroy();
	}

	private static TlsSecret Update13TrafficSecret(SecurityParameters securityParameters, TlsSecret secret)
	{
		return TlsCryptoUtilities.HkdfExpandLabel(secret, securityParameters.PrfCryptoHashAlgorithm, "traffic upd", EmptyBytes, securityParameters.PrfHashLength);
	}

	public static DerObjectIdentifier GetOidForHashAlgorithm(short hashAlgorithm)
	{
		return hashAlgorithm switch
		{
			1 => PkcsObjectIdentifiers.MD5, 
			2 => X509ObjectIdentifiers.IdSha1, 
			3 => NistObjectIdentifiers.IdSha224, 
			4 => NistObjectIdentifiers.IdSha256, 
			5 => NistObjectIdentifiers.IdSha384, 
			6 => NistObjectIdentifiers.IdSha512, 
			_ => throw new ArgumentException("invalid HashAlgorithm: " + HashAlgorithm.GetText(hashAlgorithm)), 
		};
	}

	internal static int GetPrfAlgorithm(SecurityParameters securityParameters, int cipherSuite)
	{
		ProtocolVersion negotiatedVersion = securityParameters.NegotiatedVersion;
		bool flag = IsTlsV13(negotiatedVersion);
		bool flag2 = !flag && IsTlsV12(negotiatedVersion);
		bool isSsl = negotiatedVersion.IsSsl;
		if (cipherSuite <= 199)
		{
			switch (cipherSuite)
			{
			case 198:
			case 199:
				break;
			case 59:
			case 60:
			case 61:
			case 62:
			case 63:
			case 64:
			case 103:
			case 104:
			case 105:
			case 106:
			case 107:
			case 108:
			case 109:
			case 156:
			case 158:
			case 160:
			case 162:
			case 164:
			case 166:
			case 168:
			case 170:
			case 172:
			case 186:
			case 187:
			case 188:
			case 189:
			case 190:
			case 191:
			case 192:
			case 193:
			case 194:
			case 195:
			case 196:
			case 197:
				goto IL_0511;
			case 157:
			case 159:
			case 161:
			case 163:
			case 165:
			case 167:
			case 169:
			case 171:
			case 173:
				goto IL_051e;
			case 175:
			case 177:
			case 179:
			case 181:
			case 183:
			case 185:
				goto IL_0538;
			default:
				goto IL_054f;
			}
			if (flag)
			{
				return 7;
			}
			throw new TlsFatalAlert(47);
		}
		switch (cipherSuite)
		{
		case 4865:
		case 4867:
		case 4868:
		case 4869:
			break;
		case 4866:
			goto IL_04f7;
		case 49187:
		case 49189:
		case 49191:
		case 49193:
		case 49195:
		case 49197:
		case 49199:
		case 49201:
		case 49212:
		case 49214:
		case 49216:
		case 49218:
		case 49220:
		case 49222:
		case 49224:
		case 49226:
		case 49228:
		case 49230:
		case 49232:
		case 49234:
		case 49236:
		case 49238:
		case 49240:
		case 49242:
		case 49244:
		case 49246:
		case 49248:
		case 49250:
		case 49252:
		case 49254:
		case 49256:
		case 49258:
		case 49260:
		case 49262:
		case 49264:
		case 49266:
		case 49268:
		case 49270:
		case 49272:
		case 49274:
		case 49276:
		case 49278:
		case 49280:
		case 49282:
		case 49284:
		case 49286:
		case 49288:
		case 49290:
		case 49292:
		case 49294:
		case 49296:
		case 49298:
		case 49308:
		case 49309:
		case 49310:
		case 49311:
		case 49312:
		case 49313:
		case 49314:
		case 49315:
		case 49316:
		case 49317:
		case 49318:
		case 49319:
		case 49320:
		case 49321:
		case 49322:
		case 49323:
		case 49324:
		case 49325:
		case 49326:
		case 49327:
		case 52392:
		case 52393:
		case 52394:
		case 52395:
		case 52396:
		case 52397:
		case 52398:
		case 53249:
		case 53251:
		case 53253:
			goto IL_0511;
		case 49188:
		case 49190:
		case 49192:
		case 49194:
		case 49196:
		case 49198:
		case 49200:
		case 49202:
		case 49213:
		case 49215:
		case 49217:
		case 49219:
		case 49221:
		case 49223:
		case 49225:
		case 49227:
		case 49229:
		case 49231:
		case 49233:
		case 49235:
		case 49237:
		case 49239:
		case 49241:
		case 49243:
		case 49245:
		case 49247:
		case 49249:
		case 49251:
		case 49253:
		case 49255:
		case 49257:
		case 49259:
		case 49261:
		case 49263:
		case 49265:
		case 49267:
		case 49269:
		case 49271:
		case 49273:
		case 49275:
		case 49277:
		case 49279:
		case 49281:
		case 49283:
		case 49285:
		case 49287:
		case 49289:
		case 49291:
		case 49293:
		case 49295:
		case 49297:
		case 49299:
		case 53250:
			goto IL_051e;
		case 49408:
		case 49409:
		case 49410:
			goto IL_052b;
		case 49208:
		case 49211:
		case 49301:
		case 49303:
		case 49305:
		case 49307:
			goto IL_0538;
		default:
			goto IL_054f;
		}
		if (flag)
		{
			return 4;
		}
		throw new TlsFatalAlert(47);
		IL_0538:
		if (flag)
		{
			throw new TlsFatalAlert(47);
		}
		if (flag2)
		{
			return 3;
		}
		if (isSsl)
		{
			return 0;
		}
		return 1;
		IL_051e:
		if (flag2)
		{
			return 3;
		}
		throw new TlsFatalAlert(47);
		IL_054f:
		if (flag)
		{
			throw new TlsFatalAlert(47);
		}
		if (flag2)
		{
			return 2;
		}
		if (isSsl)
		{
			return 0;
		}
		return 1;
		IL_052b:
		if (flag2)
		{
			return 8;
		}
		throw new TlsFatalAlert(47);
		IL_04f7:
		if (flag)
		{
			return 5;
		}
		throw new TlsFatalAlert(47);
		IL_0511:
		if (flag2)
		{
			return 2;
		}
		throw new TlsFatalAlert(47);
	}

	internal static int GetPrfAlgorithm13(int cipherSuite)
	{
		switch (cipherSuite)
		{
		case 4865:
		case 4867:
		case 4868:
		case 4869:
			return 4;
		case 4866:
			return 5;
		case 198:
		case 199:
			return 7;
		default:
			return -1;
		}
	}

	internal static int[] GetPrfAlgorithms13(int[] cipherSuites)
	{
		int[] array = new int[System.Math.Min(3, cipherSuites.Length)];
		int n = 0;
		for (int i = 0; i < cipherSuites.Length; i++)
		{
			int prfAlgorithm = GetPrfAlgorithm13(cipherSuites[i]);
			if (prfAlgorithm >= 0 && !Arrays.Contains(array, prfAlgorithm))
			{
				array[n++] = prfAlgorithm;
			}
		}
		return Truncate(array, n);
	}

	internal static byte[] CalculateSignatureHash(TlsContext context, SignatureAndHashAlgorithm algorithm, byte[] extraSignatureInput, DigestInputBuffer buf)
	{
		TlsCrypto crypto = context.Crypto;
		TlsHash tlsHash;
		if (algorithm != null)
		{
			tlsHash = CreateHash(crypto, algorithm);
		}
		else
		{
			TlsHash tlsHash2 = new CombinedHash(crypto);
			tlsHash = tlsHash2;
		}
		TlsHash tlsHash3 = tlsHash;
		SecurityParameters securityParameters = context.SecurityParameters;
		byte[] array = Arrays.Concatenate(securityParameters.ClientRandom, securityParameters.ServerRandom);
		tlsHash3.Update(array, 0, array.Length);
		if (extraSignatureInput != null)
		{
			tlsHash3.Update(extraSignatureInput, 0, extraSignatureInput.Length);
		}
		buf.UpdateDigest(tlsHash3);
		return tlsHash3.CalculateHash();
	}

	internal static void SendSignatureInput(TlsContext context, byte[] extraSignatureInput, DigestInputBuffer buf, Stream output)
	{
		SecurityParameters securityParameters = context.SecurityParameters;
		byte[] array = Arrays.Concatenate(securityParameters.ClientRandom, securityParameters.ServerRandom);
		output.Write(array, 0, array.Length);
		if (extraSignatureInput != null)
		{
			output.Write(extraSignatureInput, 0, extraSignatureInput.Length);
		}
		buf.CopyInputTo(output);
	}

	internal static DigitallySigned GenerateCertificateVerifyClient(TlsClientContext clientContext, TlsCredentialedSigner clientAuthSigner, SignatureAndHashAlgorithm clientAuthAlgorithm, TlsStreamSigner clientAuthStreamSigner, TlsHandshakeHash handshakeHash)
	{
		SecurityParameters securityParameters = clientContext.SecurityParameters;
		if (IsTlsV13(securityParameters.NegotiatedVersion))
		{
			throw new TlsFatalAlert(80);
		}
		byte[] signature;
		if (clientAuthStreamSigner != null)
		{
			handshakeHash.CopyBufferTo(clientAuthStreamSigner.Stream);
			signature = clientAuthStreamSigner.GetSignature();
		}
		else
		{
			byte[] hash = ((clientAuthAlgorithm != null) ? handshakeHash.GetFinalHash(SignatureScheme.GetCryptoHashAlgorithm(clientAuthAlgorithm)) : securityParameters.SessionHash);
			signature = clientAuthSigner.GenerateRawSignature(hash);
		}
		return new DigitallySigned(clientAuthAlgorithm, signature);
	}

	internal static DigitallySigned Generate13CertificateVerify(TlsContext context, TlsCredentialedSigner credentialedSigner, TlsHandshakeHash handshakeHash)
	{
		SignatureAndHashAlgorithm signatureAndHashAlgorithm = credentialedSigner.SignatureAndHashAlgorithm;
		if (signatureAndHashAlgorithm == null)
		{
			throw new TlsFatalAlert(80);
		}
		string contextString = (context.IsServer ? "TLS 1.3, server CertificateVerify" : "TLS 1.3, client CertificateVerify");
		byte[] signature = Generate13CertificateVerify(context.Crypto, credentialedSigner, contextString, handshakeHash, signatureAndHashAlgorithm);
		return new DigitallySigned(signatureAndHashAlgorithm, signature);
	}

	private static byte[] Generate13CertificateVerify(TlsCrypto crypto, TlsCredentialedSigner credentialedSigner, string contextString, TlsHandshakeHash handshakeHash, SignatureAndHashAlgorithm signatureAndHashAlgorithm)
	{
		TlsStreamSigner streamSigner = credentialedSigner.GetStreamSigner();
		byte[] certificateVerifyHeader = GetCertificateVerifyHeader(contextString);
		byte[] currentPrfHash = GetCurrentPrfHash(handshakeHash);
		if (streamSigner != null)
		{
			Stream stream = streamSigner.Stream;
			stream.Write(certificateVerifyHeader, 0, certificateVerifyHeader.Length);
			stream.Write(currentPrfHash, 0, currentPrfHash.Length);
			return streamSigner.GetSignature();
		}
		TlsHash tlsHash = CreateHash(crypto, signatureAndHashAlgorithm);
		tlsHash.Update(certificateVerifyHeader, 0, certificateVerifyHeader.Length);
		tlsHash.Update(currentPrfHash, 0, currentPrfHash.Length);
		byte[] hash = tlsHash.CalculateHash();
		return credentialedSigner.GenerateRawSignature(hash);
	}

	internal static void VerifyCertificateVerifyClient(TlsServerContext serverContext, CertificateRequest certificateRequest, DigitallySigned certificateVerify, TlsHandshakeHash handshakeHash)
	{
		SecurityParameters securityParameters = serverContext.SecurityParameters;
		TlsCertificate certificateAt = securityParameters.PeerCertificate.GetCertificateAt(0);
		SignatureAndHashAlgorithm algorithm = certificateVerify.Algorithm;
		short signatureAlgorithm;
		if (algorithm == null)
		{
			signatureAlgorithm = certificateAt.GetLegacySignatureAlgorithm();
			CheckClientCertificateType(certificateRequest, GetLegacyClientCertType(signatureAlgorithm), 43);
		}
		else
		{
			VerifySupportedSignatureAlgorithm(securityParameters.ServerSigAlgs, algorithm);
			signatureAlgorithm = algorithm.Signature;
			CheckClientCertificateType(certificateRequest, SignatureAlgorithm.GetClientCertificateType(signatureAlgorithm), 47);
		}
		bool flag;
		try
		{
			TlsVerifier tlsVerifier = certificateAt.CreateVerifier(signatureAlgorithm);
			TlsStreamVerifier streamVerifier = tlsVerifier.GetStreamVerifier(certificateVerify);
			if (streamVerifier != null)
			{
				handshakeHash.CopyBufferTo(streamVerifier.Stream);
				flag = streamVerifier.IsVerified();
			}
			else
			{
				byte[] hash = ((!IsTlsV12(serverContext)) ? securityParameters.SessionHash : handshakeHash.GetFinalHash(SignatureScheme.GetCryptoHashAlgorithm(algorithm)));
				flag = tlsVerifier.VerifyRawSignature(certificateVerify, hash);
			}
		}
		catch (TlsFatalAlert)
		{
			throw;
		}
		catch (Exception alertCause)
		{
			throw new TlsFatalAlert(51, alertCause);
		}
		if (!flag)
		{
			throw new TlsFatalAlert(51);
		}
	}

	internal static void Verify13CertificateVerifyClient(TlsServerContext serverContext, TlsHandshakeHash handshakeHash, CertificateVerify certificateVerify)
	{
		SecurityParameters securityParameters = serverContext.SecurityParameters;
		IList<SignatureAndHashAlgorithm> serverSigAlgs = securityParameters.ServerSigAlgs;
		TlsCertificate certificateAt = securityParameters.PeerCertificate.GetCertificateAt(0);
		Verify13CertificateVerify(serverSigAlgs, "TLS 1.3, client CertificateVerify", handshakeHash, certificateAt, certificateVerify);
	}

	internal static void Verify13CertificateVerifyServer(TlsClientContext clientContext, TlsHandshakeHash handshakeHash, CertificateVerify certificateVerify)
	{
		SecurityParameters securityParameters = clientContext.SecurityParameters;
		IList<SignatureAndHashAlgorithm> clientSigAlgs = securityParameters.ClientSigAlgs;
		TlsCertificate certificateAt = securityParameters.PeerCertificate.GetCertificateAt(0);
		Verify13CertificateVerify(clientSigAlgs, "TLS 1.3, server CertificateVerify", handshakeHash, certificateAt, certificateVerify);
	}

	private static void Verify13CertificateVerify(IList<SignatureAndHashAlgorithm> supportedAlgorithms, string contextString, TlsHandshakeHash handshakeHash, TlsCertificate certificate, CertificateVerify certificateVerify)
	{
		bool flag;
		try
		{
			int algorithm = certificateVerify.Algorithm;
			SignatureAndHashAlgorithm signatureAndHashAlgorithm = SignatureScheme.GetSignatureAndHashAlgorithm(algorithm);
			VerifySupportedSignatureAlgorithm(supportedAlgorithms, signatureAndHashAlgorithm);
			Tls13Verifier tls13Verifier = certificate.CreateVerifier(algorithm);
			byte[] certificateVerifyHeader = GetCertificateVerifyHeader(contextString);
			byte[] currentPrfHash = GetCurrentPrfHash(handshakeHash);
			Stream stream = tls13Verifier.Stream;
			stream.Write(certificateVerifyHeader, 0, certificateVerifyHeader.Length);
			stream.Write(currentPrfHash, 0, currentPrfHash.Length);
			flag = tls13Verifier.VerifySignature(certificateVerify.Signature);
		}
		catch (TlsFatalAlert)
		{
			throw;
		}
		catch (Exception alertCause)
		{
			throw new TlsFatalAlert(51, alertCause);
		}
		if (!flag)
		{
			throw new TlsFatalAlert(51);
		}
	}

	private static byte[] GetCertificateVerifyHeader(string contextString)
	{
		int length = contextString.Length;
		byte[] array = new byte[64 + length + 1];
		for (int i = 0; i < 64; i++)
		{
			array[i] = 32;
		}
		for (int j = 0; j < length; j++)
		{
			char c = contextString[j];
			array[64 + j] = (byte)c;
		}
		array[64 + length] = 0;
		return array;
	}

	internal static void GenerateServerKeyExchangeSignature(TlsContext context, TlsCredentialedSigner credentials, byte[] extraSignatureInput, DigestInputBuffer digestBuffer)
	{
		SignatureAndHashAlgorithm signatureAndHashAlgorithm = GetSignatureAndHashAlgorithm(context.ServerVersion, credentials);
		TlsStreamSigner streamSigner = credentials.GetStreamSigner();
		byte[] signature;
		if (streamSigner != null)
		{
			using (Stream output = streamSigner.Stream)
			{
				SendSignatureInput(context, extraSignatureInput, digestBuffer, output);
			}
			signature = streamSigner.GetSignature();
		}
		else
		{
			byte[] hash = CalculateSignatureHash(context, signatureAndHashAlgorithm, extraSignatureInput, digestBuffer);
			signature = credentials.GenerateRawSignature(hash);
		}
		new DigitallySigned(signatureAndHashAlgorithm, signature).Encode(digestBuffer);
	}

	internal static void VerifyServerKeyExchangeSignature(TlsContext context, Stream signatureInput, TlsCertificate serverCertificate, byte[] extraSignatureInput, DigestInputBuffer digestBuffer)
	{
		DigitallySigned digitallySigned = DigitallySigned.Parse(context, signatureInput);
		SecurityParameters securityParameters = context.SecurityParameters;
		int keyExchangeAlgorithm = securityParameters.KeyExchangeAlgorithm;
		SignatureAndHashAlgorithm algorithm = digitallySigned.Algorithm;
		short signatureAlgorithm;
		if (algorithm == null)
		{
			signatureAlgorithm = GetLegacySignatureAlgorithmServer(keyExchangeAlgorithm);
		}
		else
		{
			signatureAlgorithm = algorithm.Signature;
			if (!IsValidSignatureAlgorithmForServerKeyExchange(signatureAlgorithm, keyExchangeAlgorithm))
			{
				throw new TlsFatalAlert(47);
			}
			VerifySupportedSignatureAlgorithm(securityParameters.ClientSigAlgs, algorithm);
		}
		TlsVerifier tlsVerifier = serverCertificate.CreateVerifier(signatureAlgorithm);
		TlsStreamVerifier streamVerifier = tlsVerifier.GetStreamVerifier(digitallySigned);
		bool flag;
		if (streamVerifier != null)
		{
			using (Stream output = streamVerifier.Stream)
			{
				SendSignatureInput(context, null, digestBuffer, output);
			}
			flag = streamVerifier.IsVerified();
		}
		else
		{
			byte[] hash = CalculateSignatureHash(context, algorithm, null, digestBuffer);
			flag = tlsVerifier.VerifyRawSignature(digitallySigned, hash);
		}
		if (!flag)
		{
			throw new TlsFatalAlert(51);
		}
	}

	internal static void TrackHashAlgorithmClient(TlsHandshakeHash handshakeHash, SignatureAndHashAlgorithm signatureAndHashAlgorithm)
	{
		int cryptoHashAlgorithm = SignatureScheme.GetCryptoHashAlgorithm(signatureAndHashAlgorithm);
		if (cryptoHashAlgorithm >= 0)
		{
			handshakeHash.TrackHashAlgorithm(cryptoHashAlgorithm);
		}
	}

	internal static void TrackHashAlgorithms(TlsHandshakeHash handshakeHash, IList<SignatureAndHashAlgorithm> supportedSignatureAlgorithms)
	{
		foreach (SignatureAndHashAlgorithm supportedSignatureAlgorithm in supportedSignatureAlgorithms)
		{
			int cryptoHashAlgorithm = SignatureScheme.GetCryptoHashAlgorithm(supportedSignatureAlgorithm);
			if (cryptoHashAlgorithm >= 0)
			{
				handshakeHash.TrackHashAlgorithm(cryptoHashAlgorithm);
			}
			else if (8 == supportedSignatureAlgorithm.Hash)
			{
				handshakeHash.ForceBuffering();
			}
		}
	}

	public static bool HasSigningCapability(short clientCertificateType)
	{
		if ((uint)(clientCertificateType - 1) <= 1u || clientCertificateType == 64)
		{
			return true;
		}
		return false;
	}

	public static IList<T> VectorOfOne<T>(T obj)
	{
		return new List<T>(1) { obj };
	}

	public static int GetCipherType(int cipherSuite)
	{
		return GetEncryptionAlgorithmType(GetEncryptionAlgorithm(cipherSuite));
	}

	public static int GetEncryptionAlgorithm(int cipherSuite)
	{
		switch (cipherSuite)
		{
		case 49410:
			return 31;
		case 10:
		case 13:
		case 16:
		case 19:
		case 22:
		case 27:
		case 139:
		case 143:
		case 147:
		case 49155:
		case 49160:
		case 49165:
		case 49170:
		case 49175:
		case 49178:
		case 49179:
		case 49180:
		case 49204:
			return 7;
		case 47:
		case 48:
		case 49:
		case 50:
		case 51:
		case 52:
		case 60:
		case 62:
		case 63:
		case 64:
		case 103:
		case 108:
		case 140:
		case 144:
		case 148:
		case 174:
		case 178:
		case 182:
		case 49156:
		case 49161:
		case 49166:
		case 49171:
		case 49176:
		case 49181:
		case 49182:
		case 49183:
		case 49187:
		case 49189:
		case 49191:
		case 49193:
		case 49205:
		case 49207:
			return 8;
		case 4868:
		case 49308:
		case 49310:
		case 49316:
		case 49318:
		case 49324:
		case 53253:
			return 15;
		case 4869:
		case 49312:
		case 49314:
		case 49320:
		case 49322:
		case 49326:
		case 53251:
			return 16;
		case 156:
		case 158:
		case 160:
		case 162:
		case 164:
		case 166:
		case 168:
		case 170:
		case 172:
		case 4865:
		case 49195:
		case 49197:
		case 49199:
		case 49201:
		case 53249:
			return 10;
		case 53:
		case 54:
		case 55:
		case 56:
		case 57:
		case 58:
		case 61:
		case 104:
		case 105:
		case 106:
		case 107:
		case 109:
		case 141:
		case 145:
		case 149:
		case 175:
		case 179:
		case 183:
		case 49157:
		case 49162:
		case 49167:
		case 49172:
		case 49177:
		case 49184:
		case 49185:
		case 49186:
		case 49188:
		case 49190:
		case 49192:
		case 49194:
		case 49206:
		case 49208:
			return 9;
		case 49309:
		case 49311:
		case 49317:
		case 49319:
		case 49325:
			return 17;
		case 49313:
		case 49315:
		case 49321:
		case 49323:
		case 49327:
			return 18;
		case 157:
		case 159:
		case 161:
		case 163:
		case 165:
		case 167:
		case 169:
		case 171:
		case 173:
		case 4866:
		case 49196:
		case 49198:
		case 49200:
		case 49202:
		case 53250:
			return 11;
		case 49212:
		case 49214:
		case 49216:
		case 49218:
		case 49220:
		case 49222:
		case 49224:
		case 49226:
		case 49228:
		case 49230:
		case 49252:
		case 49254:
		case 49256:
		case 49264:
			return 22;
		case 49232:
		case 49234:
		case 49236:
		case 49238:
		case 49240:
		case 49242:
		case 49244:
		case 49246:
		case 49248:
		case 49250:
		case 49258:
		case 49260:
		case 49262:
			return 24;
		case 49213:
		case 49215:
		case 49217:
		case 49219:
		case 49221:
		case 49223:
		case 49225:
		case 49227:
		case 49229:
		case 49231:
		case 49253:
		case 49255:
		case 49257:
		case 49265:
			return 23;
		case 49233:
		case 49235:
		case 49237:
		case 49239:
		case 49241:
		case 49243:
		case 49245:
		case 49247:
		case 49249:
		case 49251:
		case 49259:
		case 49261:
		case 49263:
			return 25;
		case 65:
		case 66:
		case 67:
		case 68:
		case 69:
		case 70:
		case 186:
		case 187:
		case 188:
		case 189:
		case 190:
		case 191:
		case 49266:
		case 49268:
		case 49270:
		case 49272:
		case 49300:
		case 49302:
		case 49304:
		case 49306:
			return 12;
		case 49274:
		case 49276:
		case 49278:
		case 49280:
		case 49282:
		case 49284:
		case 49286:
		case 49288:
		case 49290:
		case 49292:
		case 49294:
		case 49296:
		case 49298:
			return 19;
		case 132:
		case 133:
		case 134:
		case 135:
		case 136:
		case 137:
		case 192:
		case 193:
		case 194:
		case 195:
		case 196:
		case 197:
		case 49267:
		case 49269:
		case 49271:
		case 49273:
		case 49301:
		case 49303:
		case 49305:
		case 49307:
			return 13;
		case 49275:
		case 49277:
		case 49279:
		case 49281:
		case 49283:
		case 49285:
		case 49287:
		case 49289:
		case 49291:
		case 49293:
		case 49295:
		case 49297:
		case 49299:
			return 20;
		case 4867:
		case 52392:
		case 52393:
		case 52394:
		case 52395:
		case 52396:
		case 52397:
		case 52398:
			return 21;
		case 49408:
			return 29;
		case 49409:
			return 30;
		case 2:
		case 44:
		case 45:
		case 46:
		case 49153:
		case 49158:
		case 49163:
		case 49168:
		case 49173:
		case 49209:
			return 0;
		case 59:
		case 176:
		case 180:
		case 184:
		case 49210:
			return 0;
		case 177:
		case 181:
		case 185:
		case 49211:
			return 0;
		case 4:
		case 5:
			return 2;
		case 150:
		case 151:
		case 152:
		case 153:
		case 154:
		case 155:
			return 14;
		case 199:
			return 26;
		case 198:
			return 27;
		default:
			return -1;
		}
	}

	public static int GetEncryptionAlgorithmType(int encryptionAlgorithm)
	{
		switch (encryptionAlgorithm)
		{
		case 10:
		case 11:
		case 15:
		case 16:
		case 17:
		case 18:
		case 19:
		case 20:
		case 21:
		case 24:
		case 25:
		case 26:
		case 27:
			return 2;
		case 3:
		case 4:
		case 5:
		case 6:
		case 7:
		case 8:
		case 9:
		case 12:
		case 13:
		case 14:
		case 22:
		case 23:
		case 28:
			return 1;
		case 0:
		case 1:
		case 2:
		case 29:
		case 30:
		case 31:
			return 0;
		default:
			return -1;
		}
	}

	public static int GetKeyExchangeAlgorithm(int cipherSuite)
	{
		switch (cipherSuite)
		{
		case 27:
		case 52:
		case 58:
		case 70:
		case 108:
		case 109:
		case 137:
		case 155:
		case 166:
		case 167:
		case 191:
		case 197:
		case 49222:
		case 49223:
		case 49242:
		case 49243:
		case 49284:
		case 49285:
			return 11;
		case 13:
		case 48:
		case 54:
		case 62:
		case 66:
		case 104:
		case 133:
		case 151:
		case 164:
		case 165:
		case 187:
		case 193:
		case 49214:
		case 49215:
		case 49240:
		case 49241:
		case 49282:
		case 49283:
			return 7;
		case 16:
		case 49:
		case 55:
		case 63:
		case 67:
		case 105:
		case 134:
		case 152:
		case 160:
		case 161:
		case 188:
		case 194:
		case 49216:
		case 49217:
		case 49236:
		case 49237:
		case 49278:
		case 49279:
			return 9;
		case 19:
		case 50:
		case 56:
		case 64:
		case 68:
		case 106:
		case 135:
		case 153:
		case 162:
		case 163:
		case 189:
		case 195:
		case 49218:
		case 49219:
		case 49238:
		case 49239:
		case 49280:
		case 49281:
			return 3;
		case 45:
		case 143:
		case 144:
		case 145:
		case 170:
		case 171:
		case 178:
		case 179:
		case 180:
		case 181:
		case 49254:
		case 49255:
		case 49260:
		case 49261:
		case 49296:
		case 49297:
		case 49302:
		case 49303:
		case 49318:
		case 49319:
		case 49322:
		case 49323:
		case 52397:
			return 14;
		case 22:
		case 51:
		case 57:
		case 69:
		case 103:
		case 107:
		case 136:
		case 154:
		case 158:
		case 159:
		case 190:
		case 196:
		case 49220:
		case 49221:
		case 49234:
		case 49235:
		case 49276:
		case 49277:
		case 49310:
		case 49311:
		case 49314:
		case 49315:
		case 52394:
			return 5;
		case 49173:
		case 49175:
		case 49176:
		case 49177:
			return 20;
		case 49153:
		case 49155:
		case 49156:
		case 49157:
		case 49189:
		case 49190:
		case 49197:
		case 49198:
		case 49226:
		case 49227:
		case 49246:
		case 49247:
		case 49268:
		case 49269:
		case 49288:
		case 49289:
			return 16;
		case 49163:
		case 49165:
		case 49166:
		case 49167:
		case 49193:
		case 49194:
		case 49201:
		case 49202:
		case 49230:
		case 49231:
		case 49250:
		case 49251:
		case 49272:
		case 49273:
		case 49292:
		case 49293:
			return 18;
		case 49158:
		case 49160:
		case 49161:
		case 49162:
		case 49187:
		case 49188:
		case 49195:
		case 49196:
		case 49224:
		case 49225:
		case 49244:
		case 49245:
		case 49266:
		case 49267:
		case 49286:
		case 49287:
		case 49324:
		case 49325:
		case 49326:
		case 49327:
		case 52393:
			return 17;
		case 49204:
		case 49205:
		case 49206:
		case 49207:
		case 49208:
		case 49209:
		case 49210:
		case 49211:
		case 49264:
		case 49265:
		case 49306:
		case 49307:
		case 52396:
		case 53249:
		case 53250:
		case 53251:
		case 53253:
			return 24;
		case 49168:
		case 49170:
		case 49171:
		case 49172:
		case 49191:
		case 49192:
		case 49199:
		case 49200:
		case 49228:
		case 49229:
		case 49248:
		case 49249:
		case 49270:
		case 49271:
		case 49290:
		case 49291:
		case 52392:
			return 19;
		case 49408:
		case 49409:
		case 49410:
			return 26;
		case 198:
		case 199:
		case 4865:
		case 4866:
		case 4867:
		case 4868:
		case 4869:
			return 0;
		case 44:
		case 139:
		case 140:
		case 141:
		case 168:
		case 169:
		case 174:
		case 175:
		case 176:
		case 177:
		case 49252:
		case 49253:
		case 49258:
		case 49259:
		case 49294:
		case 49295:
		case 49300:
		case 49301:
		case 49316:
		case 49317:
		case 49320:
		case 49321:
		case 52395:
			return 13;
		case 2:
		case 4:
		case 5:
		case 10:
		case 47:
		case 53:
		case 59:
		case 60:
		case 61:
		case 65:
		case 132:
		case 150:
		case 156:
		case 157:
		case 186:
		case 192:
		case 49212:
		case 49213:
		case 49232:
		case 49233:
		case 49274:
		case 49275:
		case 49308:
		case 49309:
		case 49312:
		case 49313:
			return 1;
		case 46:
		case 147:
		case 148:
		case 149:
		case 172:
		case 173:
		case 182:
		case 183:
		case 184:
		case 185:
		case 49256:
		case 49257:
		case 49262:
		case 49263:
		case 49298:
		case 49299:
		case 49304:
		case 49305:
		case 52398:
			return 15;
		case 49178:
		case 49181:
		case 49184:
			return 21;
		case 49180:
		case 49183:
		case 49186:
			return 22;
		case 49179:
		case 49182:
		case 49185:
			return 23;
		default:
			return -1;
		}
	}

	public static IList<int> GetKeyExchangeAlgorithms(int[] cipherSuites)
	{
		List<int> list = new List<int>();
		if (cipherSuites != null)
		{
			for (int i = 0; i < cipherSuites.Length; i++)
			{
				AddToSet(list, GetKeyExchangeAlgorithm(cipherSuites[i]));
			}
			list.Remove(-1);
		}
		return list;
	}

	public static int GetMacAlgorithm(int cipherSuite)
	{
		if (cipherSuite <= 49327)
		{
			switch (cipherSuite)
			{
			default:
				switch (cipherSuite)
				{
				case 4865:
				case 4866:
				case 4867:
				case 4868:
				case 4869:
				case 49195:
				case 49196:
				case 49197:
				case 49198:
				case 49199:
				case 49200:
				case 49201:
				case 49202:
				case 49232:
				case 49233:
				case 49234:
				case 49235:
				case 49236:
				case 49237:
				case 49238:
				case 49239:
				case 49240:
				case 49241:
				case 49242:
				case 49243:
				case 49244:
				case 49245:
				case 49246:
				case 49247:
				case 49248:
				case 49249:
				case 49250:
				case 49251:
				case 49258:
				case 49259:
				case 49260:
				case 49261:
				case 49262:
				case 49263:
				case 49274:
				case 49275:
				case 49276:
				case 49277:
				case 49278:
				case 49279:
				case 49280:
				case 49281:
				case 49282:
				case 49283:
				case 49284:
				case 49285:
				case 49286:
				case 49287:
				case 49288:
				case 49289:
				case 49290:
				case 49291:
				case 49292:
				case 49293:
				case 49294:
				case 49295:
				case 49296:
				case 49297:
				case 49298:
				case 49299:
				case 49308:
				case 49309:
				case 49310:
				case 49311:
				case 49312:
				case 49313:
				case 49314:
				case 49315:
				case 49316:
				case 49317:
				case 49318:
				case 49319:
				case 49320:
				case 49321:
				case 49322:
				case 49323:
				case 49324:
				case 49325:
				case 49326:
				case 49327:
					break;
				case 49153:
				case 49155:
				case 49156:
				case 49157:
				case 49158:
				case 49160:
				case 49161:
				case 49162:
				case 49163:
				case 49165:
				case 49166:
				case 49167:
				case 49168:
				case 49170:
				case 49171:
				case 49172:
				case 49173:
				case 49175:
				case 49176:
				case 49177:
				case 49178:
				case 49179:
				case 49180:
				case 49181:
				case 49182:
				case 49183:
				case 49184:
				case 49185:
				case 49186:
				case 49204:
				case 49205:
				case 49206:
				case 49209:
					goto IL_0622;
				case 49187:
				case 49189:
				case 49191:
				case 49193:
				case 49207:
				case 49210:
				case 49212:
				case 49214:
				case 49216:
				case 49218:
				case 49220:
				case 49222:
				case 49224:
				case 49226:
				case 49228:
				case 49230:
				case 49252:
				case 49254:
				case 49256:
				case 49264:
				case 49266:
				case 49268:
				case 49270:
				case 49272:
				case 49300:
				case 49302:
				case 49304:
				case 49306:
					goto IL_0624;
				case 49188:
				case 49190:
				case 49192:
				case 49194:
				case 49208:
				case 49211:
				case 49213:
				case 49215:
				case 49217:
				case 49219:
				case 49221:
				case 49223:
				case 49225:
				case 49227:
				case 49229:
				case 49231:
				case 49253:
				case 49255:
				case 49257:
				case 49265:
				case 49267:
				case 49269:
				case 49271:
				case 49273:
				case 49301:
				case 49303:
				case 49305:
				case 49307:
					goto IL_0626;
				default:
					goto IL_0628;
				}
				break;
			case 156:
			case 157:
			case 158:
			case 159:
			case 160:
			case 161:
			case 162:
			case 163:
			case 164:
			case 165:
			case 166:
			case 167:
			case 168:
			case 169:
			case 170:
			case 171:
			case 172:
			case 173:
			case 198:
			case 199:
				break;
			case 4:
				return 1;
			case 2:
			case 5:
			case 10:
			case 13:
			case 16:
			case 19:
			case 22:
			case 27:
			case 44:
			case 45:
			case 46:
			case 47:
			case 48:
			case 49:
			case 50:
			case 51:
			case 52:
			case 53:
			case 54:
			case 55:
			case 56:
			case 57:
			case 58:
			case 65:
			case 66:
			case 67:
			case 68:
			case 69:
			case 70:
			case 132:
			case 133:
			case 134:
			case 135:
			case 136:
			case 137:
			case 139:
			case 140:
			case 141:
			case 143:
			case 144:
			case 145:
			case 147:
			case 148:
			case 149:
			case 150:
			case 151:
			case 152:
			case 153:
			case 154:
			case 155:
				goto IL_0622;
			case 59:
			case 60:
			case 61:
			case 62:
			case 63:
			case 64:
			case 103:
			case 104:
			case 105:
			case 106:
			case 107:
			case 108:
			case 109:
			case 174:
			case 176:
			case 178:
			case 180:
			case 182:
			case 184:
			case 186:
			case 187:
			case 188:
			case 189:
			case 190:
			case 191:
			case 192:
			case 193:
			case 194:
			case 195:
			case 196:
			case 197:
				goto IL_0624;
			case 175:
			case 177:
			case 179:
			case 181:
			case 183:
			case 185:
				goto IL_0626;
			case 3:
			case 6:
			case 7:
			case 8:
			case 9:
			case 11:
			case 12:
			case 14:
			case 15:
			case 17:
			case 18:
			case 20:
			case 21:
			case 23:
			case 24:
			case 25:
			case 26:
			case 28:
			case 29:
			case 30:
			case 31:
			case 32:
			case 33:
			case 34:
			case 35:
			case 36:
			case 37:
			case 38:
			case 39:
			case 40:
			case 41:
			case 42:
			case 43:
			case 71:
			case 72:
			case 73:
			case 74:
			case 75:
			case 76:
			case 77:
			case 78:
			case 79:
			case 80:
			case 81:
			case 82:
			case 83:
			case 84:
			case 85:
			case 86:
			case 87:
			case 88:
			case 89:
			case 90:
			case 91:
			case 92:
			case 93:
			case 94:
			case 95:
			case 96:
			case 97:
			case 98:
			case 99:
			case 100:
			case 101:
			case 102:
			case 110:
			case 111:
			case 112:
			case 113:
			case 114:
			case 115:
			case 116:
			case 117:
			case 118:
			case 119:
			case 120:
			case 121:
			case 122:
			case 123:
			case 124:
			case 125:
			case 126:
			case 127:
			case 128:
			case 129:
			case 130:
			case 131:
			case 138:
			case 142:
			case 146:
				goto IL_0628;
				IL_0626:
				return 4;
				IL_0624:
				return 3;
				IL_0622:
				return 2;
			}
		}
		else if ((uint)(cipherSuite - 52392) > 6u && (uint)(cipherSuite - 53249) > 2u && cipherSuite != 53253)
		{
			goto IL_0628;
		}
		return 0;
		IL_0628:
		return -1;
	}

	public static ProtocolVersion GetMinimumVersion(int cipherSuite)
	{
		if (cipherSuite <= 4869)
		{
			switch (cipherSuite)
			{
			case 198:
			case 199:
			case 4865:
			case 4866:
			case 4867:
			case 4868:
			case 4869:
				return ProtocolVersion.TLSv13;
			case 59:
			case 60:
			case 61:
			case 62:
			case 63:
			case 64:
			case 103:
			case 104:
			case 105:
			case 106:
			case 107:
			case 108:
			case 109:
			case 156:
			case 157:
			case 158:
			case 159:
			case 160:
			case 161:
			case 162:
			case 163:
			case 164:
			case 165:
			case 166:
			case 167:
			case 168:
			case 169:
			case 170:
			case 171:
			case 172:
			case 173:
			case 186:
			case 187:
			case 188:
			case 189:
			case 190:
			case 191:
			case 192:
			case 193:
			case 194:
			case 195:
			case 196:
			case 197:
				break;
			default:
				goto IL_04ba;
			}
		}
		else if (cipherSuite <= 52398)
		{
			switch (cipherSuite)
			{
			default:
				if ((uint)(cipherSuite - 52392) <= 6u)
				{
					break;
				}
				goto IL_04ba;
			case 49187:
			case 49188:
			case 49189:
			case 49190:
			case 49191:
			case 49192:
			case 49193:
			case 49194:
			case 49195:
			case 49196:
			case 49197:
			case 49198:
			case 49199:
			case 49200:
			case 49201:
			case 49202:
			case 49212:
			case 49213:
			case 49214:
			case 49215:
			case 49216:
			case 49217:
			case 49218:
			case 49219:
			case 49220:
			case 49221:
			case 49222:
			case 49223:
			case 49224:
			case 49225:
			case 49226:
			case 49227:
			case 49228:
			case 49229:
			case 49230:
			case 49231:
			case 49232:
			case 49233:
			case 49234:
			case 49235:
			case 49236:
			case 49237:
			case 49238:
			case 49239:
			case 49240:
			case 49241:
			case 49242:
			case 49243:
			case 49244:
			case 49245:
			case 49246:
			case 49247:
			case 49248:
			case 49249:
			case 49250:
			case 49251:
			case 49252:
			case 49253:
			case 49254:
			case 49255:
			case 49256:
			case 49257:
			case 49258:
			case 49259:
			case 49260:
			case 49261:
			case 49262:
			case 49263:
			case 49264:
			case 49265:
			case 49266:
			case 49267:
			case 49268:
			case 49269:
			case 49270:
			case 49271:
			case 49272:
			case 49273:
			case 49274:
			case 49275:
			case 49276:
			case 49277:
			case 49278:
			case 49279:
			case 49280:
			case 49281:
			case 49282:
			case 49283:
			case 49284:
			case 49285:
			case 49286:
			case 49287:
			case 49288:
			case 49289:
			case 49290:
			case 49291:
			case 49292:
			case 49293:
			case 49294:
			case 49295:
			case 49296:
			case 49297:
			case 49298:
			case 49299:
			case 49308:
			case 49309:
			case 49310:
			case 49311:
			case 49312:
			case 49313:
			case 49314:
			case 49315:
			case 49316:
			case 49317:
			case 49318:
			case 49319:
			case 49320:
			case 49321:
			case 49322:
			case 49323:
			case 49324:
			case 49325:
			case 49326:
			case 49327:
			case 49408:
			case 49409:
			case 49410:
				break;
			case 49203:
			case 49204:
			case 49205:
			case 49206:
			case 49207:
			case 49208:
			case 49209:
			case 49210:
			case 49211:
			case 49300:
			case 49301:
			case 49302:
			case 49303:
			case 49304:
			case 49305:
			case 49306:
			case 49307:
			case 49328:
			case 49329:
			case 49330:
			case 49331:
			case 49332:
			case 49333:
			case 49334:
			case 49335:
			case 49336:
			case 49337:
			case 49338:
			case 49339:
			case 49340:
			case 49341:
			case 49342:
			case 49343:
			case 49344:
			case 49345:
			case 49346:
			case 49347:
			case 49348:
			case 49349:
			case 49350:
			case 49351:
			case 49352:
			case 49353:
			case 49354:
			case 49355:
			case 49356:
			case 49357:
			case 49358:
			case 49359:
			case 49360:
			case 49361:
			case 49362:
			case 49363:
			case 49364:
			case 49365:
			case 49366:
			case 49367:
			case 49368:
			case 49369:
			case 49370:
			case 49371:
			case 49372:
			case 49373:
			case 49374:
			case 49375:
			case 49376:
			case 49377:
			case 49378:
			case 49379:
			case 49380:
			case 49381:
			case 49382:
			case 49383:
			case 49384:
			case 49385:
			case 49386:
			case 49387:
			case 49388:
			case 49389:
			case 49390:
			case 49391:
			case 49392:
			case 49393:
			case 49394:
			case 49395:
			case 49396:
			case 49397:
			case 49398:
			case 49399:
			case 49400:
			case 49401:
			case 49402:
			case 49403:
			case 49404:
			case 49405:
			case 49406:
			case 49407:
				goto IL_04ba;
			}
		}
		else if ((uint)(cipherSuite - 53249) > 2u && cipherSuite != 53253)
		{
			goto IL_04ba;
		}
		return ProtocolVersion.TLSv12;
		IL_04ba:
		return ProtocolVersion.SSLv3;
	}

	public static IList<int> GetNamedGroupRoles(int[] cipherSuites)
	{
		return GetNamedGroupRoles(GetKeyExchangeAlgorithms(cipherSuites));
	}

	public static IList<int> GetNamedGroupRoles(IList<int> keyExchangeAlgorithms)
	{
		List<int> list = new List<int>();
		using IEnumerator<int> enumerator = keyExchangeAlgorithms.GetEnumerator();
		while (enumerator.MoveNext())
		{
			switch (enumerator.Current)
			{
			case 3:
			case 5:
			case 7:
			case 9:
			case 11:
			case 14:
				AddToSet(list, 1);
				break;
			case 18:
			case 19:
			case 20:
			case 24:
				AddToSet(list, 2);
				break;
			case 16:
			case 17:
				AddToSet(list, 2);
				AddToSet(list, 3);
				break;
			case 0:
				AddToSet(list, 1);
				AddToSet(list, 2);
				break;
			}
		}
		return list;
	}

	public static bool IsAeadCipherSuite(int cipherSuite)
	{
		return 2 == GetCipherType(cipherSuite);
	}

	public static bool IsBlockCipherSuite(int cipherSuite)
	{
		return 1 == GetCipherType(cipherSuite);
	}

	public static bool IsStreamCipherSuite(int cipherSuite)
	{
		return GetCipherType(cipherSuite) == 0;
	}

	public static bool IsValidCipherSuiteForSignatureAlgorithms(int cipherSuite, IList<short> sigAlgs)
	{
		int keyExchangeAlgorithm = GetKeyExchangeAlgorithm(cipherSuite);
		switch (keyExchangeAlgorithm)
		{
		default:
			return true;
		case 0:
		case 3:
		case 5:
		case 17:
		case 19:
		case 22:
		case 23:
			foreach (short sigAlg in sigAlgs)
			{
				if (IsValidSignatureAlgorithmForServerKeyExchange(sigAlg, keyExchangeAlgorithm))
				{
					return true;
				}
			}
			return false;
		}
	}

	internal static bool IsValidCipherSuiteSelection(int[] offeredCipherSuites, int cipherSuite)
	{
		if (offeredCipherSuites != null && Arrays.Contains(offeredCipherSuites, cipherSuite) && cipherSuite != 0)
		{
			return !CipherSuite.IsScsv(cipherSuite);
		}
		return false;
	}

	internal static bool IsValidKeyShareSelection(ProtocolVersion negotiatedVersion, int[] clientSupportedGroups, IDictionary<int, TlsAgreement> clientAgreements, int keyShareGroup)
	{
		if (clientSupportedGroups != null && Arrays.Contains(clientSupportedGroups, keyShareGroup) && !clientAgreements.ContainsKey(keyShareGroup))
		{
			return NamedGroup.CanBeNegotiated(keyShareGroup, negotiatedVersion);
		}
		return false;
	}

	internal static bool IsValidSignatureAlgorithmForServerKeyExchange(short signatureAlgorithm, int keyExchangeAlgorithm)
	{
		switch (keyExchangeAlgorithm)
		{
		case 5:
		case 19:
		case 23:
			switch (signatureAlgorithm)
			{
			case 1:
			case 4:
			case 5:
			case 6:
			case 9:
			case 10:
			case 11:
				return true;
			default:
				return false;
			}
		case 3:
		case 22:
			return 2 == signatureAlgorithm;
		case 17:
			if (signatureAlgorithm == 3 || (uint)(signatureAlgorithm - 7) <= 1u)
			{
				return true;
			}
			return false;
		case 0:
			return signatureAlgorithm != 0;
		default:
			return false;
		}
	}

	public static bool IsValidSignatureSchemeForServerKeyExchange(int signatureScheme, int keyExchangeAlgorithm)
	{
		return IsValidSignatureAlgorithmForServerKeyExchange(SignatureScheme.GetSignatureAlgorithm(signatureScheme), keyExchangeAlgorithm);
	}

	public static bool IsValidVersionForCipherSuite(int cipherSuite, ProtocolVersion version)
	{
		version = version.GetEquivalentTlsVersion();
		ProtocolVersion minimumVersion = GetMinimumVersion(cipherSuite);
		if (minimumVersion == version)
		{
			return true;
		}
		if (!minimumVersion.IsEarlierVersionOf(version))
		{
			return false;
		}
		if (!ProtocolVersion.TLSv13.IsEqualOrEarlierVersionOf(minimumVersion))
		{
			return ProtocolVersion.TLSv13.IsLaterVersionOf(version);
		}
		return true;
	}

	public static SignatureAndHashAlgorithm ChooseSignatureAndHashAlgorithm(TlsContext context, IList<SignatureAndHashAlgorithm> sigHashAlgs, short signatureAlgorithm)
	{
		return ChooseSignatureAndHashAlgorithm(context.ServerVersion, sigHashAlgs, signatureAlgorithm);
	}

	public static SignatureAndHashAlgorithm ChooseSignatureAndHashAlgorithm(ProtocolVersion negotiatedVersion, IList<SignatureAndHashAlgorithm> sigHashAlgs, short signatureAlgorithm)
	{
		if (!IsTlsV12(negotiatedVersion))
		{
			return null;
		}
		if (sigHashAlgs == null)
		{
			sigHashAlgs = GetDefaultSignatureAlgorithms(signatureAlgorithm);
		}
		SignatureAndHashAlgorithm signatureAndHashAlgorithm = null;
		foreach (SignatureAndHashAlgorithm sigHashAlg in sigHashAlgs)
		{
			if (sigHashAlg.Signature != signatureAlgorithm)
			{
				continue;
			}
			short hash = sigHashAlg.Hash;
			if (hash < MinimumHashStrict)
			{
				continue;
			}
			if (signatureAndHashAlgorithm == null)
			{
				signatureAndHashAlgorithm = sigHashAlg;
				continue;
			}
			short hash2 = signatureAndHashAlgorithm.Hash;
			if (hash2 < MinimumHashPreferred)
			{
				if (hash > hash2)
				{
					signatureAndHashAlgorithm = sigHashAlg;
				}
			}
			else if (hash >= MinimumHashPreferred && hash < hash2)
			{
				signatureAndHashAlgorithm = sigHashAlg;
			}
		}
		if (signatureAndHashAlgorithm == null)
		{
			throw new TlsFatalAlert(80);
		}
		return signatureAndHashAlgorithm;
	}

	public static IList<short> GetUsableSignatureAlgorithms(IList<SignatureAndHashAlgorithm> sigHashAlgs)
	{
		if (sigHashAlgs == null)
		{
			return new List<short> { 1, 2, 3 };
		}
		List<short> list = new List<short>();
		foreach (SignatureAndHashAlgorithm sigHashAlg in sigHashAlgs)
		{
			if (sigHashAlg.Hash >= MinimumHashStrict)
			{
				short signature = sigHashAlg.Signature;
				if (!list.Contains(signature))
				{
					list.Add(signature);
				}
			}
		}
		return list;
	}

	public static int GetCommonCipherSuite13(ProtocolVersion negotiatedVersion, int[] peerCipherSuites, int[] localCipherSuites, bool useLocalOrder)
	{
		int[] array = peerCipherSuites;
		int[] a = localCipherSuites;
		if (useLocalOrder)
		{
			array = localCipherSuites;
			a = peerCipherSuites;
		}
		foreach (int num in array)
		{
			if (Arrays.Contains(a, num) && IsValidVersionForCipherSuite(num, negotiatedVersion))
			{
				return num;
			}
		}
		return -1;
	}

	public static int[] GetCommonCipherSuites(int[] peerCipherSuites, int[] localCipherSuites, bool useLocalOrder)
	{
		int[] array = peerCipherSuites;
		int[] array2 = localCipherSuites;
		if (useLocalOrder)
		{
			array = localCipherSuites;
			array2 = peerCipherSuites;
		}
		int num = 0;
		int num2 = System.Math.Min(array.Length, array2.Length);
		int[] array3 = new int[num2];
		foreach (int num3 in array)
		{
			if (!Contains(array3, 0, num, num3) && Arrays.Contains(array2, num3))
			{
				array3[num++] = num3;
			}
		}
		if (num < num2)
		{
			array3 = Arrays.CopyOf(array3, num);
		}
		return array3;
	}

	public static int[] GetSupportedCipherSuites(TlsCrypto crypto, int[] suites)
	{
		return GetSupportedCipherSuites(crypto, suites, 0, suites.Length);
	}

	public static int[] GetSupportedCipherSuites(TlsCrypto crypto, int[] suites, int suitesOff, int suitesCount)
	{
		int[] array = new int[suitesCount];
		int num = 0;
		for (int i = 0; i < suitesCount; i++)
		{
			int num2 = suites[suitesOff + i];
			if (IsSupportedCipherSuite(crypto, num2))
			{
				array[num++] = num2;
			}
		}
		if (num < suitesCount)
		{
			array = Arrays.CopyOf(array, num);
		}
		return array;
	}

	public static bool IsSupportedCipherSuite(TlsCrypto crypto, int cipherSuite)
	{
		int keyExchangeAlgorithm = GetKeyExchangeAlgorithm(cipherSuite);
		if (!IsSupportedKeyExchange(crypto, keyExchangeAlgorithm))
		{
			return false;
		}
		int encryptionAlgorithm = GetEncryptionAlgorithm(cipherSuite);
		if (encryptionAlgorithm < 0 || !crypto.HasEncryptionAlgorithm(encryptionAlgorithm))
		{
			return false;
		}
		int macAlgorithm = GetMacAlgorithm(cipherSuite);
		if (macAlgorithm != 0 && (macAlgorithm < 0 || !crypto.HasMacAlgorithm(macAlgorithm)))
		{
			return false;
		}
		return true;
	}

	public static bool IsSupportedKeyExchange(TlsCrypto crypto, int keyExchangeAlgorithm)
	{
		switch (keyExchangeAlgorithm)
		{
		case 7:
		case 9:
		case 11:
		case 14:
			return crypto.HasDHAgreement();
		case 3:
			if (crypto.HasDHAgreement())
			{
				return crypto.HasSignatureAlgorithm(2);
			}
			return false;
		case 5:
			if (crypto.HasDHAgreement())
			{
				return HasAnyRsaSigAlgs(crypto);
			}
			return false;
		case 16:
		case 18:
		case 20:
		case 24:
			return crypto.HasECDHAgreement();
		case 17:
			if (crypto.HasECDHAgreement())
			{
				if (!crypto.HasSignatureAlgorithm(3) && !crypto.HasSignatureAlgorithm(7))
				{
					return crypto.HasSignatureAlgorithm(8);
				}
				return true;
			}
			return false;
		case 19:
			if (crypto.HasECDHAgreement())
			{
				return HasAnyRsaSigAlgs(crypto);
			}
			return false;
		case 0:
		case 13:
			return true;
		case 1:
		case 15:
			return crypto.HasRsaEncryption();
		case 21:
			return crypto.HasSrpAuthentication();
		case 22:
			if (crypto.HasSrpAuthentication())
			{
				return crypto.HasSignatureAlgorithm(2);
			}
			return false;
		case 23:
			if (crypto.HasSrpAuthentication())
			{
				return HasAnyRsaSigAlgs(crypto);
			}
			return false;
		default:
			return false;
		}
	}

	internal static bool HasAnyRsaSigAlgs(TlsCrypto crypto)
	{
		if (!crypto.HasSignatureAlgorithm(1) && !crypto.HasSignatureAlgorithm(4) && !crypto.HasSignatureAlgorithm(5) && !crypto.HasSignatureAlgorithm(6) && !crypto.HasSignatureAlgorithm(9) && !crypto.HasSignatureAlgorithm(10))
		{
			return crypto.HasSignatureAlgorithm(11);
		}
		return true;
	}

	internal static byte[] GetCurrentPrfHash(TlsHandshakeHash handshakeHash)
	{
		return handshakeHash.ForkPrfHash().CalculateHash();
	}

	private static TlsHash CreateHash(TlsCrypto crypto, short hashAlgorithm)
	{
		return crypto.CreateHash(TlsCryptoUtilities.GetHash(hashAlgorithm));
	}

	private static TlsHash CreateHash(TlsCrypto crypto, SignatureAndHashAlgorithm signatureAndHashAlgorithm)
	{
		return crypto.CreateHash(SignatureScheme.GetCryptoHashAlgorithm(signatureAndHashAlgorithm));
	}

	private static TlsKeyExchange CreateKeyExchangeClient(TlsClient client, int keyExchange)
	{
		TlsKeyExchangeFactory keyExchangeFactory = client.GetKeyExchangeFactory();
		switch (keyExchange)
		{
		case 11:
			return keyExchangeFactory.CreateDHanonKeyExchangeClient(keyExchange, client.GetDHGroupVerifier());
		case 7:
		case 9:
			return keyExchangeFactory.CreateDHKeyExchange(keyExchange);
		case 3:
		case 5:
			return keyExchangeFactory.CreateDheKeyExchangeClient(keyExchange, client.GetDHGroupVerifier());
		case 20:
			return keyExchangeFactory.CreateECDHanonKeyExchangeClient(keyExchange);
		case 16:
		case 18:
			return keyExchangeFactory.CreateECDHKeyExchange(keyExchange);
		case 17:
		case 19:
			return keyExchangeFactory.CreateECDheKeyExchangeClient(keyExchange);
		case 1:
			return keyExchangeFactory.CreateRsaKeyExchange(keyExchange);
		case 14:
			return keyExchangeFactory.CreatePskKeyExchangeClient(keyExchange, client.GetPskIdentity(), client.GetDHGroupVerifier());
		case 13:
		case 15:
		case 24:
			return keyExchangeFactory.CreatePskKeyExchangeClient(keyExchange, client.GetPskIdentity(), null);
		case 21:
		case 22:
		case 23:
			return keyExchangeFactory.CreateSrpKeyExchangeClient(keyExchange, client.GetSrpIdentity(), client.GetSrpConfigVerifier());
		default:
			throw new TlsFatalAlert(80);
		}
	}

	private static TlsKeyExchange CreateKeyExchangeServer(TlsServer server, int keyExchange)
	{
		TlsKeyExchangeFactory keyExchangeFactory = server.GetKeyExchangeFactory();
		switch (keyExchange)
		{
		case 11:
			return keyExchangeFactory.CreateDHanonKeyExchangeServer(keyExchange, server.GetDHConfig());
		case 7:
		case 9:
			return keyExchangeFactory.CreateDHKeyExchange(keyExchange);
		case 3:
		case 5:
			return keyExchangeFactory.CreateDheKeyExchangeServer(keyExchange, server.GetDHConfig());
		case 20:
			return keyExchangeFactory.CreateECDHanonKeyExchangeServer(keyExchange, server.GetECDHConfig());
		case 16:
		case 18:
			return keyExchangeFactory.CreateECDHKeyExchange(keyExchange);
		case 17:
		case 19:
			return keyExchangeFactory.CreateECDheKeyExchangeServer(keyExchange, server.GetECDHConfig());
		case 1:
			return keyExchangeFactory.CreateRsaKeyExchange(keyExchange);
		case 14:
			return keyExchangeFactory.CreatePskKeyExchangeServer(keyExchange, server.GetPskIdentityManager(), server.GetDHConfig(), null);
		case 24:
			return keyExchangeFactory.CreatePskKeyExchangeServer(keyExchange, server.GetPskIdentityManager(), null, server.GetECDHConfig());
		case 13:
		case 15:
			return keyExchangeFactory.CreatePskKeyExchangeServer(keyExchange, server.GetPskIdentityManager(), null, null);
		case 21:
		case 22:
		case 23:
			return keyExchangeFactory.CreateSrpKeyExchangeServer(keyExchange, server.GetSrpLoginParameters());
		default:
			throw new TlsFatalAlert(80);
		}
	}

	internal static TlsKeyExchange InitKeyExchangeClient(TlsClientContext clientContext, TlsClient client)
	{
		SecurityParameters securityParameters = clientContext.SecurityParameters;
		TlsKeyExchange tlsKeyExchange = CreateKeyExchangeClient(client, securityParameters.KeyExchangeAlgorithm);
		tlsKeyExchange.Init(clientContext);
		return tlsKeyExchange;
	}

	internal static TlsKeyExchange InitKeyExchangeServer(TlsServerContext serverContext, TlsServer server)
	{
		SecurityParameters securityParameters = serverContext.SecurityParameters;
		TlsKeyExchange tlsKeyExchange = CreateKeyExchangeServer(server, securityParameters.KeyExchangeAlgorithm);
		tlsKeyExchange.Init(serverContext);
		return tlsKeyExchange;
	}

	internal static TlsCipher InitCipher(TlsContext context)
	{
		int cipherSuite = context.SecurityParameters.CipherSuite;
		int encryptionAlgorithm = GetEncryptionAlgorithm(cipherSuite);
		int macAlgorithm = GetMacAlgorithm(cipherSuite);
		if (encryptionAlgorithm < 0 || macAlgorithm < 0)
		{
			throw new TlsFatalAlert(80);
		}
		return context.Crypto.CreateCipher(new TlsCryptoParameters(context), encryptionAlgorithm, macAlgorithm);
	}

	public static void CheckPeerSigAlgs(TlsContext context, TlsCertificate[] peerCertPath)
	{
		if (context.IsServer)
		{
			CheckSigAlgOfClientCerts(context, peerCertPath);
		}
		else
		{
			CheckSigAlgOfServerCerts(context, peerCertPath);
		}
	}

	private static void CheckSigAlgOfClientCerts(TlsContext context, TlsCertificate[] clientCertPath)
	{
		SecurityParameters securityParameters = context.SecurityParameters;
		short[] clientCertTypes = securityParameters.ClientCertTypes;
		IList<SignatureAndHashAlgorithm> serverSigAlgsCert = securityParameters.ServerSigAlgsCert;
		int num = clientCertPath.Length - 1;
		for (int i = 0; i < num; i++)
		{
			TlsCertificate subjectCert = clientCertPath[i];
			TlsCertificate issuerCert = clientCertPath[i + 1];
			SignatureAndHashAlgorithm certSigAndHashAlg = GetCertSigAndHashAlg(subjectCert, issuerCert);
			bool flag = false;
			if (certSigAndHashAlg != null)
			{
				if (serverSigAlgsCert == null)
				{
					if (clientCertTypes != null)
					{
						for (int j = 0; j < clientCertTypes.Length; j++)
						{
							short legacySignatureAlgorithmClientCert = GetLegacySignatureAlgorithmClientCert(clientCertTypes[j]);
							if (certSigAndHashAlg.Signature == legacySignatureAlgorithmClientCert)
							{
								flag = true;
								break;
							}
						}
					}
				}
				else
				{
					flag = ContainsSignatureAlgorithm(serverSigAlgsCert, certSigAndHashAlg);
				}
			}
			if (!flag)
			{
				throw new TlsFatalAlert(42);
			}
		}
	}

	private static void CheckSigAlgOfServerCerts(TlsContext context, TlsCertificate[] serverCertPath)
	{
		SecurityParameters securityParameters = context.SecurityParameters;
		IList<SignatureAndHashAlgorithm> clientSigAlgsCert = securityParameters.ClientSigAlgsCert;
		IList<SignatureAndHashAlgorithm> list = securityParameters.ClientSigAlgs;
		if (list == clientSigAlgsCert || IsTlsV13(securityParameters.NegotiatedVersion))
		{
			list = null;
		}
		int num = serverCertPath.Length - 1;
		for (int i = 0; i < num; i++)
		{
			TlsCertificate subjectCert = serverCertPath[i];
			TlsCertificate issuerCert = serverCertPath[i + 1];
			SignatureAndHashAlgorithm certSigAndHashAlg = GetCertSigAndHashAlg(subjectCert, issuerCert);
			bool flag = false;
			if (certSigAndHashAlg != null)
			{
				flag = ((clientSigAlgsCert != null) ? (ContainsSignatureAlgorithm(clientSigAlgsCert, certSigAndHashAlg) || (list != null && ContainsSignatureAlgorithm(list, certSigAndHashAlg))) : (GetLegacySignatureAlgorithmServerCert(securityParameters.KeyExchangeAlgorithm) == certSigAndHashAlg.Signature));
			}
			if (!flag)
			{
				throw new TlsFatalAlert(42);
			}
		}
	}

	internal static void CheckTlsFeatures(Certificate serverCertificate, IDictionary<int, byte[]> clientExtensions, IDictionary<int, byte[]> serverExtensions)
	{
		byte[] extension = serverCertificate.GetCertificateAt(0).GetExtension(TlsObjectIdentifiers.id_pe_tlsfeature);
		if (extension == null)
		{
			return;
		}
		Asn1Sequence asn1Sequence = (Asn1Sequence)ReadAsn1Object(extension);
		for (int i = 0; i < asn1Sequence.Count; i++)
		{
			if (!(asn1Sequence[i] is DerInteger))
			{
				throw new TlsFatalAlert(42);
			}
		}
		RequireDerEncoding(asn1Sequence, extension);
		foreach (DerInteger item in asn1Sequence)
		{
			BigInteger positiveValue = item.PositiveValue;
			if (positiveValue.BitLength <= 16)
			{
				int intValueExact = positiveValue.IntValueExact;
				if (clientExtensions.ContainsKey(intValueExact) && !serverExtensions.ContainsKey(intValueExact))
				{
					throw new TlsFatalAlert(46);
				}
			}
		}
	}

	internal static void ProcessClientCertificate(TlsServerContext serverContext, Certificate clientCertificate, TlsKeyExchange keyExchange, TlsServer server)
	{
		SecurityParameters securityParameters = serverContext.SecurityParameters;
		if (securityParameters.PeerCertificate != null)
		{
			throw new TlsFatalAlert(10);
		}
		if (!IsTlsV13(securityParameters.NegotiatedVersion))
		{
			if (clientCertificate.IsEmpty)
			{
				keyExchange.SkipClientCredentials();
			}
			else
			{
				keyExchange.ProcessClientCertificate(clientCertificate);
			}
		}
		securityParameters.m_peerCertificate = clientCertificate;
		server.NotifyClientCertificate(clientCertificate);
	}

	internal static void ProcessServerCertificate(TlsClientContext clientContext, CertificateStatus serverCertificateStatus, TlsKeyExchange keyExchange, TlsAuthentication clientAuthentication, IDictionary<int, byte[]> clientExtensions, IDictionary<int, byte[]> serverExtensions)
	{
		SecurityParameters securityParameters = clientContext.SecurityParameters;
		bool flag = IsTlsV13(securityParameters.NegotiatedVersion);
		if (clientAuthentication == null)
		{
			if (flag)
			{
				throw new TlsFatalAlert(80);
			}
			keyExchange.SkipServerCredentials();
			securityParameters.m_tlsServerEndPoint = EmptyBytes;
			return;
		}
		Certificate peerCertificate = securityParameters.PeerCertificate;
		CheckTlsFeatures(peerCertificate, clientExtensions, serverExtensions);
		if (!flag)
		{
			keyExchange.ProcessServerCertificate(peerCertificate);
		}
		clientAuthentication.NotifyServerCertificate(new TlsServerCertificateImpl(peerCertificate, serverCertificateStatus));
	}

	internal static SignatureAndHashAlgorithm GetCertSigAndHashAlg(TlsCertificate subjectCert, TlsCertificate issuerCert)
	{
		string sigAlgOid = subjectCert.SigAlgOid;
		if (sigAlgOid != null)
		{
			if (!PkcsObjectIdentifiers.IdRsassaPss.Id.Equals(sigAlgOid))
			{
				return CollectionUtilities.GetValueOrNull(CertSigAlgOids, sigAlgOid);
			}
			RsassaPssParameters instance = RsassaPssParameters.GetInstance(subjectCert.GetSigAlgParams());
			if (instance != null)
			{
				DerObjectIdentifier algorithm = instance.HashAlgorithm.Algorithm;
				if (NistObjectIdentifiers.IdSha256.Equals(algorithm))
				{
					if (issuerCert.SupportsSignatureAlgorithmCA(9))
					{
						return SignatureAndHashAlgorithm.rsa_pss_pss_sha256;
					}
					if (issuerCert.SupportsSignatureAlgorithmCA(4))
					{
						return SignatureAndHashAlgorithm.rsa_pss_rsae_sha256;
					}
				}
				else if (NistObjectIdentifiers.IdSha384.Equals(algorithm))
				{
					if (issuerCert.SupportsSignatureAlgorithmCA(10))
					{
						return SignatureAndHashAlgorithm.rsa_pss_pss_sha384;
					}
					if (issuerCert.SupportsSignatureAlgorithmCA(5))
					{
						return SignatureAndHashAlgorithm.rsa_pss_rsae_sha384;
					}
				}
				else if (NistObjectIdentifiers.IdSha512.Equals(algorithm))
				{
					if (issuerCert.SupportsSignatureAlgorithmCA(11))
					{
						return SignatureAndHashAlgorithm.rsa_pss_pss_sha512;
					}
					if (issuerCert.SupportsSignatureAlgorithmCA(6))
					{
						return SignatureAndHashAlgorithm.rsa_pss_rsae_sha512;
					}
				}
			}
		}
		return null;
	}

	internal static CertificateRequest ValidateCertificateRequest(CertificateRequest certificateRequest, TlsKeyExchange keyExchange)
	{
		short[] clientCertificateTypes = keyExchange.GetClientCertificateTypes();
		if (IsNullOrEmpty(clientCertificateTypes))
		{
			throw new TlsFatalAlert(10);
		}
		certificateRequest = NormalizeCertificateRequest(certificateRequest, clientCertificateTypes);
		if (certificateRequest == null)
		{
			throw new TlsFatalAlert(47);
		}
		return certificateRequest;
	}

	internal static CertificateRequest NormalizeCertificateRequest(CertificateRequest certificateRequest, short[] validClientCertificateTypes)
	{
		if (ContainsAll(validClientCertificateTypes, certificateRequest.CertificateTypes))
		{
			return certificateRequest;
		}
		short[] array = RetainAll(certificateRequest.CertificateTypes, validClientCertificateTypes);
		if (array.Length < 1)
		{
			return null;
		}
		return new CertificateRequest(array, certificateRequest.SupportedSignatureAlgorithms, certificateRequest.CertificateAuthorities);
	}

	internal static bool Contains(int[] buf, int off, int len, int value)
	{
		for (int i = 0; i < len; i++)
		{
			if (value == buf[off + i])
			{
				return true;
			}
		}
		return false;
	}

	internal static bool ContainsAll(short[] container, short[] elements)
	{
		for (int i = 0; i < elements.Length; i++)
		{
			if (!Arrays.Contains(container, elements[i]))
			{
				return false;
			}
		}
		return true;
	}

	internal static bool ContainsNot(short[] buf, int off, int len, short value)
	{
		for (int i = 0; i < len; i++)
		{
			if (value != buf[off + i])
			{
				return true;
			}
		}
		return false;
	}

	internal static short[] RetainAll(short[] retainer, short[] elements)
	{
		short[] array = new short[System.Math.Min(retainer.Length, elements.Length)];
		int n = 0;
		for (int i = 0; i < elements.Length; i++)
		{
			if (Arrays.Contains(retainer, elements[i]))
			{
				array[n++] = elements[i];
			}
		}
		return Truncate(array, n);
	}

	internal static short[] Truncate(short[] a, int n)
	{
		if (n >= a.Length)
		{
			return a;
		}
		short[] array = new short[n];
		Array.Copy(a, 0, array, 0, n);
		return array;
	}

	internal static int[] Truncate(int[] a, int n)
	{
		if (n >= a.Length)
		{
			return a;
		}
		int[] array = new int[n];
		Array.Copy(a, 0, array, 0, n);
		return array;
	}

	internal static TlsCredentialedAgreement RequireAgreementCredentials(TlsCredentials credentials)
	{
		if (!(credentials is TlsCredentialedAgreement))
		{
			throw new TlsFatalAlert(80);
		}
		return (TlsCredentialedAgreement)credentials;
	}

	internal static TlsCredentialedDecryptor RequireDecryptorCredentials(TlsCredentials credentials)
	{
		if (!(credentials is TlsCredentialedDecryptor))
		{
			throw new TlsFatalAlert(80);
		}
		return (TlsCredentialedDecryptor)credentials;
	}

	internal static TlsCredentialedSigner RequireSignerCredentials(TlsCredentials credentials)
	{
		if (!(credentials is TlsCredentialedSigner))
		{
			throw new TlsFatalAlert(80);
		}
		return (TlsCredentialedSigner)credentials;
	}

	private static void CheckClientCertificateType(CertificateRequest certificateRequest, short clientCertificateType, short alertDescription)
	{
		if (clientCertificateType < 0 || !Arrays.Contains(certificateRequest.CertificateTypes, clientCertificateType))
		{
			throw new TlsFatalAlert(alertDescription);
		}
	}

	private static void CheckDowngradeMarker(byte[] randomBlock, byte[] downgradeMarker)
	{
		int num = downgradeMarker.Length;
		if (ConstantTimeAreEqual(num, downgradeMarker, 0, randomBlock, randomBlock.Length - num))
		{
			throw new TlsFatalAlert(47);
		}
	}

	internal static void CheckDowngradeMarker(ProtocolVersion version, byte[] randomBlock)
	{
		version = version.GetEquivalentTlsVersion();
		if (version.IsEqualOrEarlierVersionOf(ProtocolVersion.TLSv11))
		{
			CheckDowngradeMarker(randomBlock, DowngradeTlsV11);
		}
		if (version.IsEqualOrEarlierVersionOf(ProtocolVersion.TLSv12))
		{
			CheckDowngradeMarker(randomBlock, DowngradeTlsV12);
		}
	}

	internal static void WriteDowngradeMarker(ProtocolVersion version, byte[] randomBlock)
	{
		version = version.GetEquivalentTlsVersion();
		byte[] array;
		if (ProtocolVersion.TLSv12 == version)
		{
			array = DowngradeTlsV12;
		}
		else
		{
			if (!version.IsEqualOrEarlierVersionOf(ProtocolVersion.TLSv11))
			{
				throw new TlsFatalAlert(80);
			}
			array = DowngradeTlsV11;
		}
		Array.Copy(array, 0, randomBlock, randomBlock.Length - array.Length, array.Length);
	}

	internal static TlsAuthentication ReceiveServerCertificate(TlsClientContext clientContext, TlsClient client, MemoryStream buf, IDictionary<int, byte[]> serverExtensions)
	{
		SecurityParameters securityParameters = clientContext.SecurityParameters;
		if (KeyExchangeAlgorithm.IsAnonymous(securityParameters.KeyExchangeAlgorithm) || securityParameters.PeerCertificate != null)
		{
			throw new TlsFatalAlert(10);
		}
		MemoryStream memoryStream = new MemoryStream();
		Certificate certificate = Certificate.Parse(new Certificate.ParseOptions
		{
			CertificateType = securityParameters.ServerCertificateType,
			MaxChainLength = client.GetMaxCertificateChainLength()
		}, clientContext, buf, memoryStream);
		TlsProtocol.AssertEmpty(buf);
		if (certificate.IsEmpty)
		{
			throw new TlsFatalAlert(50);
		}
		securityParameters.m_peerCertificate = certificate;
		securityParameters.m_tlsServerEndPoint = memoryStream.ToArray();
		TlsAuthentication authentication = client.GetAuthentication();
		if (authentication == null)
		{
			throw new TlsFatalAlert(80);
		}
		return authentication;
	}

	internal static TlsAuthentication Receive13ServerCertificate(TlsClientContext clientContext, TlsClient client, MemoryStream buf, IDictionary<int, byte[]> serverExtensions)
	{
		SecurityParameters securityParameters = clientContext.SecurityParameters;
		if (securityParameters.PeerCertificate != null)
		{
			throw new TlsFatalAlert(10);
		}
		Certificate certificate = Certificate.Parse(new Certificate.ParseOptions
		{
			CertificateType = securityParameters.ServerCertificateType,
			MaxChainLength = client.GetMaxCertificateChainLength()
		}, clientContext, buf, null);
		TlsProtocol.AssertEmpty(buf);
		if (certificate.GetCertificateRequestContext().Length != 0)
		{
			throw new TlsFatalAlert(47);
		}
		if (certificate.IsEmpty)
		{
			throw new TlsFatalAlert(50);
		}
		securityParameters.m_peerCertificate = certificate;
		securityParameters.m_tlsServerEndPoint = null;
		TlsAuthentication authentication = client.GetAuthentication();
		if (authentication == null)
		{
			throw new TlsFatalAlert(80);
		}
		return authentication;
	}

	internal static TlsAuthentication Skip13ServerCertificate(TlsClientContext clientContext)
	{
		SecurityParameters securityParameters = clientContext.SecurityParameters;
		if (securityParameters.PeerCertificate != null)
		{
			throw new TlsFatalAlert(80);
		}
		securityParameters.m_peerCertificate = null;
		securityParameters.m_tlsServerEndPoint = null;
		return null;
	}

	public static bool ContainsNonAscii(byte[] bs)
	{
		for (int i = 0; i < bs.Length; i++)
		{
			if (bs[i] >= 128)
			{
				return true;
			}
		}
		return false;
	}

	public static bool ContainsNonAscii(string s)
	{
		for (int i = 0; i < s.Length; i++)
		{
			if (s[i] >= '\u0080')
			{
				return true;
			}
		}
		return false;
	}

	internal static IDictionary<int, TlsAgreement> AddKeyShareToClientHello(TlsClientContext clientContext, TlsClient client, IDictionary<int, byte[]> clientExtensions)
	{
		if (!IsTlsV13(clientContext.ClientVersion) || !clientExtensions.ContainsKey(10))
		{
			return null;
		}
		int[] supportedGroupsExtension = TlsExtensionsUtilities.GetSupportedGroupsExtension(clientExtensions);
		IList<int> earlyKeyShareGroups = client.GetEarlyKeyShareGroups();
		Dictionary<int, TlsAgreement> dictionary = new Dictionary<int, TlsAgreement>(3);
		List<KeyShareEntry> clientShares = new List<KeyShareEntry>(2);
		CollectKeyShares(clientContext.Crypto, supportedGroupsExtension, earlyKeyShareGroups, dictionary, clientShares);
		TlsExtensionsUtilities.AddKeyShareClientHello(clientExtensions, clientShares);
		return dictionary;
	}

	internal static IDictionary<int, TlsAgreement> AddKeyShareToClientHelloRetry(TlsClientContext clientContext, IDictionary<int, byte[]> clientExtensions, int keyShareGroup)
	{
		int[] supportedGroups = new int[1] { keyShareGroup };
		IList<int> keyShareGroups = VectorOfOne(keyShareGroup);
		Dictionary<int, TlsAgreement> dictionary = new Dictionary<int, TlsAgreement>(1);
		List<KeyShareEntry> list = new List<KeyShareEntry>(1);
		CollectKeyShares(clientContext.Crypto, supportedGroups, keyShareGroups, dictionary, list);
		TlsExtensionsUtilities.AddKeyShareClientHello(clientExtensions, list);
		if (dictionary.Count < 1 || list.Count < 1)
		{
			throw new TlsFatalAlert(80);
		}
		return dictionary;
	}

	private static void CollectKeyShares(TlsCrypto crypto, int[] supportedGroups, IList<int> keyShareGroups, IDictionary<int, TlsAgreement> clientAgreements, IList<KeyShareEntry> clientShares)
	{
		if (IsNullOrEmpty(supportedGroups) || keyShareGroups == null || keyShareGroups.Count < 1)
		{
			return;
		}
		foreach (int num in supportedGroups)
		{
			if (!keyShareGroups.Contains(num) || clientAgreements.ContainsKey(num) || !crypto.HasNamedGroup(num))
			{
				continue;
			}
			TlsAgreement tlsAgreement = null;
			if (NamedGroup.RefersToAnECDHCurve(num))
			{
				if (crypto.HasECDHAgreement())
				{
					tlsAgreement = crypto.CreateECDomain(new TlsECConfig(num)).CreateECDH();
				}
			}
			else if (NamedGroup.RefersToASpecificFiniteField(num) && crypto.HasDHAgreement())
			{
				tlsAgreement = crypto.CreateDHDomain(new TlsDHConfig(num, padded: true)).CreateDH();
			}
			if (tlsAgreement != null)
			{
				byte[] keyExchange = tlsAgreement.GenerateEphemeral();
				KeyShareEntry item = new KeyShareEntry(num, keyExchange);
				clientShares.Add(item);
				clientAgreements[num] = tlsAgreement;
			}
		}
	}

	internal static KeyShareEntry SelectKeyShare(IList<KeyShareEntry> clientShares, int keyShareGroup)
	{
		if (clientShares != null && 1 == clientShares.Count)
		{
			KeyShareEntry keyShareEntry = clientShares[0];
			if (keyShareEntry != null && keyShareEntry.NamedGroup == keyShareGroup)
			{
				return keyShareEntry;
			}
		}
		return null;
	}

	internal static KeyShareEntry SelectKeyShare(TlsCrypto crypto, ProtocolVersion negotiatedVersion, IList<KeyShareEntry> clientShares, int[] clientSupportedGroups, int[] serverSupportedGroups)
	{
		if (clientShares != null && !IsNullOrEmpty(clientSupportedGroups) && !IsNullOrEmpty(serverSupportedGroups))
		{
			foreach (KeyShareEntry clientShare in clientShares)
			{
				int namedGroup = clientShare.NamedGroup;
				if (NamedGroup.CanBeNegotiated(namedGroup, negotiatedVersion) && Arrays.Contains(serverSupportedGroups, namedGroup) && Arrays.Contains(clientSupportedGroups, namedGroup) && crypto.HasNamedGroup(namedGroup) && (!NamedGroup.RefersToAnECDHCurve(namedGroup) || crypto.HasECDHAgreement()) && (!NamedGroup.RefersToASpecificFiniteField(namedGroup) || crypto.HasDHAgreement()))
				{
					return clientShare;
				}
			}
		}
		return null;
	}

	internal static int SelectKeyShareGroup(TlsCrypto crypto, ProtocolVersion negotiatedVersion, int[] clientSupportedGroups, int[] serverSupportedGroups)
	{
		if (!IsNullOrEmpty(clientSupportedGroups) && !IsNullOrEmpty(serverSupportedGroups))
		{
			foreach (int num in clientSupportedGroups)
			{
				if (NamedGroup.CanBeNegotiated(num, negotiatedVersion) && Arrays.Contains(serverSupportedGroups, num) && crypto.HasNamedGroup(num) && (!NamedGroup.RefersToAnECDHCurve(num) || crypto.HasECDHAgreement()) && (!NamedGroup.RefersToASpecificFiniteField(num) || crypto.HasDHAgreement()))
				{
					return num;
				}
			}
		}
		return -1;
	}

	internal static byte[] ReadEncryptedPms(TlsContext context, Stream input)
	{
		if (IsSsl(context))
		{
			return Ssl3Utilities.ReadEncryptedPms(input);
		}
		return ReadOpaque16(input);
	}

	internal static void WriteEncryptedPms(TlsContext context, byte[] encryptedPms, Stream output)
	{
		if (IsSsl(context))
		{
			Ssl3Utilities.WriteEncryptedPms(encryptedPms, output);
		}
		else
		{
			WriteOpaque16(encryptedPms, output);
		}
	}

	internal static byte[] GetSessionID(TlsSession tlsSession)
	{
		if (tlsSession != null)
		{
			byte[] sessionID = tlsSession.SessionID;
			if (sessionID != null && sessionID.Length != 0 && sessionID.Length <= 32)
			{
				return sessionID;
			}
		}
		return EmptyBytes;
	}

	internal static void AdjustTranscriptForRetry(TlsHandshakeHash handshakeHash)
	{
		byte[] currentPrfHash = GetCurrentPrfHash(handshakeHash);
		handshakeHash.Reset();
		int num = currentPrfHash.Length;
		CheckUint8(num);
		byte[] array = new byte[4 + num];
		WriteUint8((short)254, array, 0);
		WriteUint24(num, array, 1);
		Array.Copy(currentPrfHash, 0, array, 4, num);
		handshakeHash.Update(array, 0, array.Length);
	}

	internal static TlsCredentials EstablishClientCredentials(TlsAuthentication clientAuthentication, CertificateRequest certificateRequest)
	{
		return ValidateCredentials(clientAuthentication.GetClientCredentials(certificateRequest));
	}

	internal static TlsCredentialedSigner Establish13ClientCredentials(TlsAuthentication clientAuthentication, CertificateRequest certificateRequest)
	{
		return Validate13Credentials(clientAuthentication.GetClientCredentials(certificateRequest));
	}

	internal static void EstablishClientSigAlgs(SecurityParameters securityParameters, IDictionary<int, byte[]> clientExtensions)
	{
		securityParameters.m_clientSigAlgs = TlsExtensionsUtilities.GetSignatureAlgorithmsExtension(clientExtensions);
		securityParameters.m_clientSigAlgsCert = TlsExtensionsUtilities.GetSignatureAlgorithmsCertExtension(clientExtensions);
	}

	internal static TlsCredentials EstablishServerCredentials(TlsServer server)
	{
		return ValidateCredentials(server.GetCredentials());
	}

	internal static TlsCredentialedSigner Establish13ServerCredentials(TlsServer server)
	{
		return Validate13Credentials(server.GetCredentials());
	}

	internal static void EstablishServerSigAlgs(SecurityParameters securityParameters, CertificateRequest certificateRequest)
	{
		securityParameters.m_clientCertTypes = certificateRequest.CertificateTypes;
		securityParameters.m_serverSigAlgs = certificateRequest.SupportedSignatureAlgorithms;
		securityParameters.m_serverSigAlgsCert = certificateRequest.SupportedSignatureAlgorithmsCert;
		if (securityParameters.ServerSigAlgsCert == null)
		{
			securityParameters.m_serverSigAlgsCert = securityParameters.ServerSigAlgs;
		}
	}

	internal static TlsCredentials ValidateCredentials(TlsCredentials credentials)
	{
		if (credentials != null && 0 + ((credentials is TlsCredentialedAgreement) ? 1 : 0) + ((credentials is TlsCredentialedDecryptor) ? 1 : 0) + ((credentials is TlsCredentialedSigner) ? 1 : 0) != 1)
		{
			throw new TlsFatalAlert(80);
		}
		return credentials;
	}

	internal static TlsCredentialedSigner Validate13Credentials(TlsCredentials credentials)
	{
		if (credentials == null)
		{
			return null;
		}
		if (!(credentials is TlsCredentialedSigner))
		{
			throw new TlsFatalAlert(80);
		}
		return (TlsCredentialedSigner)credentials;
	}

	internal static void NegotiatedCipherSuite(SecurityParameters securityParameters, int cipherSuite)
	{
		securityParameters.m_cipherSuite = cipherSuite;
		securityParameters.m_keyExchangeAlgorithm = GetKeyExchangeAlgorithm(cipherSuite);
		int num = (securityParameters.m_prfAlgorithm = GetPrfAlgorithm(securityParameters, cipherSuite));
		if ((uint)num <= 1u)
		{
			securityParameters.m_prfCryptoHashAlgorithm = -1;
			securityParameters.m_prfHashLength = -1;
		}
		else
		{
			securityParameters.m_prfHashLength = TlsCryptoUtilities.GetHashOutputSize(securityParameters.m_prfCryptoHashAlgorithm = TlsCryptoUtilities.GetHashForPrf(num));
		}
		ProtocolVersion negotiatedVersion = securityParameters.NegotiatedVersion;
		if (IsTlsV13(negotiatedVersion))
		{
			securityParameters.m_verifyDataLength = securityParameters.PrfHashLength;
			return;
		}
		if (negotiatedVersion.IsSsl)
		{
			securityParameters.m_verifyDataLength = 36;
			return;
		}
		switch (cipherSuite)
		{
		case 49408:
		case 49409:
			securityParameters.m_verifyDataLength = 32;
			break;
		default:
			securityParameters.m_verifyDataLength = 12;
			break;
		}
	}

	internal static void NegotiatedVersion(SecurityParameters securityParameters)
	{
		if (!IsSignatureAlgorithmsExtensionAllowed(securityParameters.NegotiatedVersion))
		{
			securityParameters.m_clientSigAlgs = null;
			securityParameters.m_clientSigAlgsCert = null;
			return;
		}
		if (securityParameters.ClientSigAlgs == null)
		{
			securityParameters.m_clientSigAlgs = GetLegacySupportedSignatureAlgorithms();
		}
		if (securityParameters.ClientSigAlgsCert == null)
		{
			securityParameters.m_clientSigAlgsCert = securityParameters.ClientSigAlgs;
		}
	}

	internal static void NegotiatedVersionDtlsClient(TlsClientContext clientContext, TlsClient client)
	{
		SecurityParameters securityParameters = clientContext.SecurityParameters;
		ProtocolVersion negotiatedVersion = securityParameters.NegotiatedVersion;
		if (!ProtocolVersion.IsSupportedDtlsVersionClient(negotiatedVersion))
		{
			throw new TlsFatalAlert(80);
		}
		NegotiatedVersion(securityParameters);
		client.NotifyServerVersion(negotiatedVersion);
	}

	internal static void NegotiatedVersionDtlsServer(TlsServerContext serverContext)
	{
		SecurityParameters securityParameters = serverContext.SecurityParameters;
		if (!ProtocolVersion.IsSupportedDtlsVersionServer(securityParameters.NegotiatedVersion))
		{
			throw new TlsFatalAlert(80);
		}
		NegotiatedVersion(securityParameters);
	}

	internal static void NegotiatedVersionTlsClient(TlsClientContext clientContext, TlsClient client)
	{
		SecurityParameters securityParameters = clientContext.SecurityParameters;
		ProtocolVersion negotiatedVersion = securityParameters.NegotiatedVersion;
		if (!ProtocolVersion.IsSupportedTlsVersionClient(negotiatedVersion))
		{
			throw new TlsFatalAlert(80);
		}
		NegotiatedVersion(securityParameters);
		client.NotifyServerVersion(negotiatedVersion);
	}

	internal static void NegotiatedVersionTlsServer(TlsServerContext serverContext)
	{
		SecurityParameters securityParameters = serverContext.SecurityParameters;
		if (!ProtocolVersion.IsSupportedTlsVersionServer(securityParameters.NegotiatedVersion))
		{
			throw new TlsFatalAlert(80);
		}
		NegotiatedVersion(securityParameters);
	}

	internal static TlsSecret DeriveSecret(SecurityParameters securityParameters, TlsSecret secret, string label, byte[] transcriptHash)
	{
		int prfCryptoHashAlgorithm = securityParameters.PrfCryptoHashAlgorithm;
		int prfHashLength = securityParameters.PrfHashLength;
		return DeriveSecret(prfCryptoHashAlgorithm, prfHashLength, secret, label, transcriptHash);
	}

	internal static TlsSecret DeriveSecret(int prfCryptoHashAlgorithm, int prfHashLength, TlsSecret secret, string label, byte[] transcriptHash)
	{
		if (transcriptHash.Length != prfHashLength)
		{
			throw new TlsFatalAlert(80);
		}
		return TlsCryptoUtilities.HkdfExpandLabel(secret, prfCryptoHashAlgorithm, label, transcriptHash, prfHashLength);
	}

	internal static TlsSecret GetSessionMasterSecret(TlsCrypto crypto, TlsSecret masterSecret)
	{
		if (masterSecret != null)
		{
			lock (masterSecret)
			{
				if (masterSecret.IsAlive())
				{
					return crypto.AdoptSecret(masterSecret);
				}
			}
		}
		return null;
	}

	internal static bool IsPermittedExtensionType13(int handshakeType, int extensionType)
	{
		switch (extensionType)
		{
		case 0:
		case 1:
		case 10:
		case 14:
		case 15:
		case 16:
		case 19:
		case 20:
			if (handshakeType == 1 || handshakeType == 8)
			{
				return true;
			}
			return false;
		case 5:
		case 18:
			if (handshakeType == 1 || handshakeType == 11 || handshakeType == 13)
			{
				return true;
			}
			return false;
		case 13:
		case 27:
		case 47:
		case 50:
			if (handshakeType == 1 || handshakeType == 13)
			{
				return true;
			}
			return false;
		case 21:
		case 45:
		case 49:
			if (handshakeType == 1)
			{
				return true;
			}
			return false;
		case 43:
		case 51:
			if ((uint)(handshakeType - 1) <= 1u || handshakeType == 6)
			{
				return true;
			}
			return false;
		case 41:
			if ((uint)(handshakeType - 1) <= 1u)
			{
				return true;
			}
			return false;
		case 42:
			if (handshakeType == 1 || handshakeType == 4 || handshakeType == 8)
			{
				return true;
			}
			return false;
		case 44:
			if (handshakeType == 1 || handshakeType == 6)
			{
				return true;
			}
			return false;
		case 48:
			if (handshakeType == 13)
			{
				return true;
			}
			return false;
		default:
			return !ExtensionType.IsRecognized(extensionType);
		}
	}

	internal static void CheckExtensionData13(IDictionary<int, byte[]> extensions, int handshakeType, short alertDescription)
	{
		foreach (int key in extensions.Keys)
		{
			if (!IsPermittedExtensionType13(handshakeType, key))
			{
				throw new TlsFatalAlert(alertDescription, "Invalid extension: " + ExtensionType.GetText(key));
			}
		}
	}

	public static TlsSecret GenerateEncryptedPreMasterSecret(TlsContext context, TlsEncryptor encryptor, Stream output)
	{
		ProtocolVersion rsaPreMasterSecretVersion = context.RsaPreMasterSecretVersion;
		TlsSecret tlsSecret = context.Crypto.GenerateRsaPreMasterSecret(rsaPreMasterSecretVersion);
		byte[] encryptedPms = tlsSecret.Encrypt(encryptor);
		WriteEncryptedPms(context, encryptedPms, output);
		return tlsSecret;
	}

	public static bool IsTimeout(SocketException e)
	{
		return SocketError.TimedOut == e.SocketErrorCode;
	}

	internal static void AddPreSharedKeyToClientExtensions(TlsPsk[] psks, IDictionary<int, byte[]> clientExtensions)
	{
		List<PskIdentity> list = new List<PskIdentity>(psks.Length);
		foreach (TlsPsk tlsPsk in psks)
		{
			list.Add(new PskIdentity(tlsPsk.Identity, 0L));
		}
		TlsExtensionsUtilities.AddPreSharedKeyClientHello(clientExtensions, new OfferedPsks(list));
	}

	internal static OfferedPsks.BindersConfig AddPreSharedKeyToClientHello(TlsClientContext clientContext, TlsClient client, IDictionary<int, byte[]> clientExtensions, int[] offeredCipherSuites)
	{
		if (!IsTlsV13(clientContext.ClientVersion))
		{
			return null;
		}
		TlsPskExternal[] pskExternalsClient = GetPskExternalsClient(client, offeredCipherSuites);
		if (pskExternalsClient == null)
		{
			return null;
		}
		short[] pskKeyExchangeModes = client.GetPskKeyExchangeModes();
		if (IsNullOrEmpty(pskKeyExchangeModes))
		{
			throw new TlsFatalAlert(80, "External PSKs configured but no PskKeyExchangeMode available");
		}
		TlsCrypto crypto = clientContext.Crypto;
		TlsPsk[] psks = pskExternalsClient;
		TlsSecret[] pskEarlySecrets = GetPskEarlySecrets(crypto, psks);
		psks = pskExternalsClient;
		int bindersSize = OfferedPsks.GetBindersSize(psks);
		psks = pskExternalsClient;
		AddPreSharedKeyToClientExtensions(psks, clientExtensions);
		TlsExtensionsUtilities.AddPskKeyExchangeModesExtension(clientExtensions, pskKeyExchangeModes);
		psks = pskExternalsClient;
		return new OfferedPsks.BindersConfig(psks, pskKeyExchangeModes, pskEarlySecrets, bindersSize);
	}

	internal static OfferedPsks.BindersConfig AddPreSharedKeyToClientHelloRetry(TlsClientContext clientContext, OfferedPsks.BindersConfig clientBinders, IDictionary<int, byte[]> clientExtensions)
	{
		int prfAlgorithm = GetPrfAlgorithm13(clientContext.SecurityParameters.CipherSuite);
		IList<int> pskIndices = GetPskIndices(clientBinders.m_psks, prfAlgorithm);
		if (pskIndices.Count < 1)
		{
			return null;
		}
		OfferedPsks.BindersConfig bindersConfig = clientBinders;
		int count = pskIndices.Count;
		if (count < clientBinders.m_psks.Length)
		{
			TlsPsk[] array = new TlsPsk[count];
			TlsSecret[] array2 = new TlsSecret[count];
			for (int i = 0; i < count; i++)
			{
				int num = pskIndices[i];
				array[i] = clientBinders.m_psks[num];
				array2[i] = clientBinders.m_earlySecrets[num];
			}
			int bindersSize = OfferedPsks.GetBindersSize(array);
			bindersConfig = new OfferedPsks.BindersConfig(array, clientBinders.m_pskKeyExchangeModes, array2, bindersSize);
		}
		AddPreSharedKeyToClientExtensions(bindersConfig.m_psks, clientExtensions);
		return bindersConfig;
	}

	internal static OfferedPsks.SelectedConfig SelectPreSharedKey(TlsServerContext serverContext, TlsServer server, IDictionary<int, byte[]> clientHelloExtensions, HandshakeMessageInput clientHelloMessage, TlsHandshakeHash handshakeHash, bool afterHelloRetryRequest)
	{
		bool flag = false;
		OfferedPsks preSharedKeyClientHello = TlsExtensionsUtilities.GetPreSharedKeyClientHello(clientHelloExtensions);
		if (preSharedKeyClientHello != null)
		{
			short[] pskKeyExchangeModesExtension = TlsExtensionsUtilities.GetPskKeyExchangeModesExtension(clientHelloExtensions);
			if (IsNullOrEmpty(pskKeyExchangeModesExtension))
			{
				throw new TlsFatalAlert(109);
			}
			if (Arrays.Contains(pskKeyExchangeModesExtension, 1))
			{
				TlsPskExternal externalPsk = server.GetExternalPsk(preSharedKeyClientHello.Identities);
				if (externalPsk != null)
				{
					int indexOfIdentity = preSharedKeyClientHello.GetIndexOfIdentity(new PskIdentity(externalPsk.Identity, 0L));
					if (indexOfIdentity >= 0)
					{
						byte[] b = preSharedKeyClientHello.Binders[indexOfIdentity];
						TlsCrypto crypto = serverContext.Crypto;
						TlsSecret pskEarlySecret = GetPskEarlySecret(crypto, externalPsk);
						bool isExternalPsk = true;
						int hashForPrf = TlsCryptoUtilities.GetHashForPrf(externalPsk.PrfAlgorithm);
						flag = true;
						int bindersSize = preSharedKeyClientHello.BindersSize;
						clientHelloMessage.UpdateHashPrefix(handshakeHash, bindersSize);
						byte[] transcriptHash;
						if (afterHelloRetryRequest)
						{
							transcriptHash = handshakeHash.GetFinalHash(hashForPrf);
						}
						else
						{
							TlsHash tlsHash = crypto.CreateHash(hashForPrf);
							handshakeHash.CopyBufferTo(new TlsHashSink(tlsHash));
							transcriptHash = tlsHash.CalculateHash();
						}
						clientHelloMessage.UpdateHashSuffix(handshakeHash, bindersSize);
						if (Arrays.FixedTimeEquals(CalculatePskBinder(crypto, isExternalPsk, hashForPrf, pskEarlySecret, transcriptHash), b))
						{
							return new OfferedPsks.SelectedConfig(indexOfIdentity, externalPsk, pskKeyExchangeModesExtension, pskEarlySecret);
						}
					}
				}
			}
		}
		if (!flag)
		{
			clientHelloMessage.UpdateHash(handshakeHash);
		}
		return null;
	}

	internal static TlsSecret GetPskEarlySecret(TlsCrypto crypto, TlsPsk psk)
	{
		int hashForPrf = TlsCryptoUtilities.GetHashForPrf(psk.PrfAlgorithm);
		return crypto.HkdfInit(hashForPrf).HkdfExtract(hashForPrf, psk.Key);
	}

	internal static TlsSecret[] GetPskEarlySecrets(TlsCrypto crypto, TlsPsk[] psks)
	{
		int num = psks.Length;
		TlsSecret[] array = new TlsSecret[num];
		for (int i = 0; i < num; i++)
		{
			array[i] = GetPskEarlySecret(crypto, psks[i]);
		}
		return array;
	}

	internal static TlsPskExternal[] GetPskExternalsClient(TlsClient client, int[] offeredCipherSuites)
	{
		IList<TlsPskExternal> externalPsks = client.GetExternalPsks();
		if (IsNullOrEmpty(externalPsks))
		{
			return null;
		}
		int[] prfAlgorithms = GetPrfAlgorithms13(offeredCipherSuites);
		int count = externalPsks.Count;
		TlsPskExternal[] array = new TlsPskExternal[count];
		for (int i = 0; i < count; i++)
		{
			TlsPskExternal tlsPskExternal = externalPsks[i];
			if (tlsPskExternal == null)
			{
				throw new TlsFatalAlert(80, "External PSKs element is not a TlsPSKExternal");
			}
			if (!Arrays.Contains(prfAlgorithms, tlsPskExternal.PrfAlgorithm))
			{
				throw new TlsFatalAlert(80, "External PSK incompatible with offered cipher suites");
			}
			array[i] = tlsPskExternal;
		}
		return array;
	}

	internal static IList<int> GetPskIndices(TlsPsk[] psks, int prfAlgorithm)
	{
		List<int> list = new List<int>(psks.Length);
		for (int i = 0; i < psks.Length; i++)
		{
			if (psks[i].PrfAlgorithm == prfAlgorithm)
			{
				list.Add(i);
			}
		}
		return list;
	}

	internal static short ProcessMaxFragmentLengthExtension(IDictionary<int, byte[]> clientExtensions, IDictionary<int, byte[]> serverExtensions, short alertDescription)
	{
		short maxFragmentLengthExtension = TlsExtensionsUtilities.GetMaxFragmentLengthExtension(serverExtensions);
		if (maxFragmentLengthExtension >= 0 && (!MaxFragmentLength.IsValid(maxFragmentLengthExtension) || (clientExtensions != null && maxFragmentLengthExtension != TlsExtensionsUtilities.GetMaxFragmentLengthExtension(clientExtensions))))
		{
			throw new TlsFatalAlert(alertDescription);
		}
		return maxFragmentLengthExtension;
	}

	internal static short ProcessClientCertificateTypeExtension(IDictionary<int, byte[]> clientExtensions, IDictionary<int, byte[]> serverExtensions, short alertDescription)
	{
		short clientCertificateTypeExtensionServer = TlsExtensionsUtilities.GetClientCertificateTypeExtensionServer(serverExtensions);
		if (clientCertificateTypeExtensionServer < 0)
		{
			return 0;
		}
		if (!CertificateType.IsValid(clientCertificateTypeExtensionServer))
		{
			throw new TlsFatalAlert(alertDescription, "Unknown value for client_certificate_type");
		}
		short[] clientCertificateTypeExtensionClient = TlsExtensionsUtilities.GetClientCertificateTypeExtensionClient(clientExtensions);
		if (clientCertificateTypeExtensionClient == null || !Arrays.Contains(clientCertificateTypeExtensionClient, clientCertificateTypeExtensionServer))
		{
			throw new TlsFatalAlert(alertDescription, "Invalid selection for client_certificate_type");
		}
		return clientCertificateTypeExtensionServer;
	}

	internal static short ProcessClientCertificateTypeExtension13(IDictionary<int, byte[]> clientExtensions, IDictionary<int, byte[]> serverExtensions, short alertDescription)
	{
		return ValidateCertificateType13(ProcessClientCertificateTypeExtension(clientExtensions, serverExtensions, alertDescription), alertDescription);
	}

	internal static short ProcessServerCertificateTypeExtension(IDictionary<int, byte[]> clientExtensions, IDictionary<int, byte[]> serverExtensions, short alertDescription)
	{
		short serverCertificateTypeExtensionServer = TlsExtensionsUtilities.GetServerCertificateTypeExtensionServer(serverExtensions);
		if (serverCertificateTypeExtensionServer < 0)
		{
			return 0;
		}
		if (!CertificateType.IsValid(serverCertificateTypeExtensionServer))
		{
			throw new TlsFatalAlert(alertDescription, "Unknown value for server_certificate_type");
		}
		short[] serverCertificateTypeExtensionClient = TlsExtensionsUtilities.GetServerCertificateTypeExtensionClient(clientExtensions);
		if (serverCertificateTypeExtensionClient == null || !Arrays.Contains(serverCertificateTypeExtensionClient, serverCertificateTypeExtensionServer))
		{
			throw new TlsFatalAlert(alertDescription, "Invalid selection for server_certificate_type");
		}
		return serverCertificateTypeExtensionServer;
	}

	internal static short ProcessServerCertificateTypeExtension13(IDictionary<int, byte[]> clientExtensions, IDictionary<int, byte[]> serverExtensions, short alertDescription)
	{
		return ValidateCertificateType13(ProcessServerCertificateTypeExtension(clientExtensions, serverExtensions, alertDescription), alertDescription);
	}

	private static short ValidateCertificateType13(short certificateType, short alertDescription)
	{
		if (1 == certificateType)
		{
			throw new TlsFatalAlert(alertDescription, "The OpenPGP certificate type MUST NOT be used with TLS 1.3");
		}
		return certificateType;
	}

	internal static int GetHandshakeResendTimeMillis(TlsPeer tlsPeer)
	{
		if (tlsPeer is AbstractTlsPeer abstractTlsPeer)
		{
			return abstractTlsPeer.GetHandshakeResendTimeMillis();
		}
		return 1000;
	}

	internal static void NotifyConnectionClosed(TlsPeer tlsPeer)
	{
		if (tlsPeer is AbstractTlsPeer abstractTlsPeer)
		{
			abstractTlsPeer.NotifyConnectionClosed();
		}
	}

	internal static bool ShouldUseCompatibilityMode(TlsClient tlsClient)
	{
		if (tlsClient is AbstractTlsClient abstractTlsClient)
		{
			return abstractTlsClient.ShouldUseCompatibilityMode();
		}
		return true;
	}
}
