using System.IO;

namespace Mirror.BouncyCastle.Asn1;

public abstract class Asn1Encodable : IAsn1Convertible
{
	public const string Ber = "BER";

	public const string Der = "DER";

	public const string DL = "DL";

	public virtual void EncodeTo(Stream output)
	{
		ToAsn1Object().EncodeTo(output);
	}

	public virtual void EncodeTo(Stream output, string encoding)
	{
		ToAsn1Object().EncodeTo(output, encoding);
	}

	public byte[] GetEncoded()
	{
		return ToAsn1Object().InternalGetEncoded("BER");
	}

	public byte[] GetEncoded(string encoding)
	{
		return ToAsn1Object().InternalGetEncoded(encoding);
	}

	public byte[] GetDerEncoded()
	{
		try
		{
			return GetEncoded("DER");
		}
		catch (IOException)
		{
			return null;
		}
	}

	public sealed override int GetHashCode()
	{
		return ToAsn1Object().CallAsn1GetHashCode();
	}

	public sealed override bool Equals(object obj)
	{
		if (obj == this)
		{
			return true;
		}
		if (!(obj is IAsn1Convertible asn1Convertible))
		{
			return false;
		}
		Asn1Object asn1Object = ToAsn1Object();
		Asn1Object asn1Object2 = asn1Convertible.ToAsn1Object();
		if (asn1Object != asn1Object2)
		{
			if (asn1Object2 != null)
			{
				return asn1Object.CallAsn1Equals(asn1Object2);
			}
			return false;
		}
		return true;
	}

	public abstract Asn1Object ToAsn1Object();
}
