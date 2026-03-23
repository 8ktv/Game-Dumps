using System.IO;

namespace Mirror.BouncyCastle.Asn1;

public interface Asn1OctetStringParser : IAsn1Convertible
{
	Stream GetOctetStream();
}
