using System.IO;

namespace Mirror.BouncyCastle.Asn1;

internal class DerOutputStream : Asn1OutputStream
{
	internal override int Encoding => 3;

	internal DerOutputStream(Stream os, bool leaveOpen)
		: base(os, leaveOpen)
	{
	}
}
