using System.IO;

namespace Mirror.BouncyCastle.Asn1;

internal class DLOutputStream : Asn1OutputStream
{
	internal override int Encoding => 2;

	internal DLOutputStream(Stream os, bool leaveOpen)
		: base(os, leaveOpen)
	{
	}
}
