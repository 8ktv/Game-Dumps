using System;
using Mirror.BouncyCastle.Asn1;
using Mirror.BouncyCastle.Asn1.Nist;
using Mirror.BouncyCastle.Asn1.Pkcs;
using Mirror.BouncyCastle.Asn1.Rosstandart;
using Mirror.BouncyCastle.Asn1.X509;

namespace Mirror.BouncyCastle.Tls.Crypto;

public abstract class TlsCryptoUtilities
{
	private static readonly byte[] Tls13Prefix = new byte[6] { 116, 108, 115, 49, 51, 32 };

	public static int GetHash(short hashAlgorithm)
	{
		return hashAlgorithm switch
		{
			1 => 1, 
			2 => 2, 
			3 => 3, 
			4 => 4, 
			5 => 5, 
			6 => 6, 
			_ => throw new ArgumentException("specified HashAlgorithm invalid: " + HashAlgorithm.GetText(hashAlgorithm)), 
		};
	}

	public static int GetHashForHmac(int macAlgorithm)
	{
		return macAlgorithm switch
		{
			1 => 1, 
			2 => 2, 
			3 => 4, 
			4 => 5, 
			5 => 6, 
			_ => throw new ArgumentException("specified MacAlgorithm not an HMAC: " + MacAlgorithm.GetText(macAlgorithm)), 
		};
	}

	public static int GetHashForPrf(int prfAlgorithm)
	{
		switch (prfAlgorithm)
		{
		case 0:
		case 1:
			throw new ArgumentException("legacy PRF not a valid algorithm");
		case 2:
		case 4:
			return 4;
		case 3:
		case 5:
			return 5;
		case 7:
			return 7;
		case 8:
			return 8;
		default:
			throw new ArgumentException("unknown PrfAlgorithm: " + PrfAlgorithm.GetText(prfAlgorithm));
		}
	}

	public static int GetHashInternalSize(int cryptoHashAlgorithm)
	{
		switch (cryptoHashAlgorithm)
		{
		case 1:
		case 2:
		case 3:
		case 4:
		case 7:
		case 8:
			return 64;
		case 5:
		case 6:
			return 128;
		default:
			throw new ArgumentException();
		}
	}

	public static int GetHashOutputSize(int cryptoHashAlgorithm)
	{
		switch (cryptoHashAlgorithm)
		{
		case 1:
			return 16;
		case 2:
			return 20;
		case 3:
			return 28;
		case 4:
		case 7:
		case 8:
			return 32;
		case 5:
			return 48;
		case 6:
			return 64;
		default:
			throw new ArgumentException();
		}
	}

	public static DerObjectIdentifier GetOidForHash(int cryptoHashAlgorithm)
	{
		return cryptoHashAlgorithm switch
		{
			1 => PkcsObjectIdentifiers.MD5, 
			2 => X509ObjectIdentifiers.IdSha1, 
			3 => NistObjectIdentifiers.IdSha224, 
			4 => NistObjectIdentifiers.IdSha256, 
			5 => NistObjectIdentifiers.IdSha384, 
			6 => NistObjectIdentifiers.IdSha512, 
			8 => RosstandartObjectIdentifiers.id_tc26_gost_3411_12_256, 
			_ => throw new ArgumentException(), 
		};
	}

	public static int GetSignature(short signatureAlgorithm)
	{
		return signatureAlgorithm switch
		{
			1 => 1, 
			2 => 2, 
			3 => 3, 
			4 => 4, 
			5 => 5, 
			6 => 6, 
			7 => 7, 
			8 => 8, 
			9 => 9, 
			10 => 10, 
			11 => 11, 
			26 => 26, 
			27 => 27, 
			28 => 28, 
			64 => 64, 
			65 => 65, 
			_ => throw new ArgumentException("specified SignatureAlgorithm invalid: " + SignatureAlgorithm.GetText(signatureAlgorithm)), 
		};
	}

	public static TlsSecret HkdfExpandLabel(TlsSecret secret, int cryptoHashAlgorithm, string label, byte[] context, int length)
	{
		int length2 = label.Length;
		if (length2 < 1)
		{
			throw new TlsFatalAlert(80);
		}
		int num = context.Length;
		int num2 = Tls13Prefix.Length + length2;
		byte[] array = new byte[2 + (1 + num2) + (1 + num)];
		TlsUtilities.CheckUint16(length);
		TlsUtilities.WriteUint16(length, array, 0);
		TlsUtilities.CheckUint8(num2);
		TlsUtilities.WriteUint8(num2, array, 2);
		Array.Copy(Tls13Prefix, 0, array, 3, Tls13Prefix.Length);
		int num3 = 2 + (1 + Tls13Prefix.Length);
		for (int i = 0; i < length2; i++)
		{
			char c = label[i];
			array[num3 + i] = (byte)c;
		}
		TlsUtilities.WriteOpaque8(context, array, 2 + (1 + num2));
		return secret.HkdfExpand(cryptoHashAlgorithm, array, length);
	}
}
