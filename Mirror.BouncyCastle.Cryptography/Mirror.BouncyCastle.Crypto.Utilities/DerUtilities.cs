using System;
using Mirror.BouncyCastle.Asn1;

namespace Mirror.BouncyCastle.Crypto.Utilities;

internal class DerUtilities
{
	internal static Asn1OctetString GetOctetString(byte[] data)
	{
		return new DerOctetString((data == null) ? Array.Empty<byte>() : ((byte[])data.Clone()));
	}

	internal static byte[] ToByteArray(Asn1Object asn1Object)
	{
		return asn1Object.GetEncoded();
	}
}
