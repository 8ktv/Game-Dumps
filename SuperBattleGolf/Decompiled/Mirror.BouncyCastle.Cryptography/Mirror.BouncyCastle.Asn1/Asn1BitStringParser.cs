using System.IO;

namespace Mirror.BouncyCastle.Asn1;

public interface Asn1BitStringParser : IAsn1Convertible
{
	int PadBits { get; }

	Stream GetBitStream();

	Stream GetOctetStream();
}
