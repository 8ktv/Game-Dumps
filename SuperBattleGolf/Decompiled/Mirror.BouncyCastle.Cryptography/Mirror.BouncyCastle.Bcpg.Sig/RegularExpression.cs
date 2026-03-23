using System;
using Mirror.BouncyCastle.Utilities;

namespace Mirror.BouncyCastle.Bcpg.Sig;

public class RegularExpression : SignatureSubpacket
{
	public string Regex => Strings.FromUtf8ByteArray(data, 0, data.Length - 1);

	public RegularExpression(bool critical, bool isLongLength, byte[] data)
		: base(SignatureSubpacketTag.RegExp, critical, isLongLength, data)
	{
		if (data[^1] != 0)
		{
			throw new ArgumentException("data in regex missing null termination");
		}
	}

	public RegularExpression(bool critical, string regex)
		: base(SignatureSubpacketTag.RegExp, critical, isLongLength: false, ToNullTerminatedUtf8ByteArray(regex))
	{
	}

	public byte[] GetRawRegex()
	{
		return Arrays.Clone(data);
	}

	private static byte[] ToNullTerminatedUtf8ByteArray(string str)
	{
		return Arrays.Append(Strings.ToUtf8ByteArray(str), 0);
	}
}
