using Mirror.BouncyCastle.Utilities;

namespace Mirror.BouncyCastle.Bcpg.Sig;

public class PolicyUrl : SignatureSubpacket
{
	public string Url => Strings.FromUtf8ByteArray(data);

	public PolicyUrl(bool critical, string url)
		: this(critical, isLongLength: false, Strings.ToUtf8ByteArray(url))
	{
	}

	public PolicyUrl(bool critical, bool isLongLength, byte[] data)
		: base(SignatureSubpacketTag.PolicyUrl, critical, isLongLength, data)
	{
	}

	public byte[] GetRawUrl()
	{
		return Arrays.Clone(data);
	}
}
