using System.IO;
using Mirror.BouncyCastle.Tls.Crypto;

namespace Mirror.BouncyCastle.Tls;

internal class DigestInputBuffer : MemoryStream
{
	internal void UpdateDigest(TlsHash hash)
	{
		WriteTo(new TlsHashSink(hash));
	}

	internal void CopyInputTo(Stream output)
	{
		WriteTo(output);
	}
}
