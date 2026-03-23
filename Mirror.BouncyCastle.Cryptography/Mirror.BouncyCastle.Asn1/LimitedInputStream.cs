using System.IO;
using Mirror.BouncyCastle.Utilities.IO;

namespace Mirror.BouncyCastle.Asn1;

internal abstract class LimitedInputStream : BaseInputStream
{
	protected readonly Stream _in;

	private int _limit;

	internal virtual int Limit => _limit;

	internal LimitedInputStream(Stream inStream, int limit)
	{
		_in = inStream;
		_limit = limit;
	}

	protected void SetParentEofDetect()
	{
		if (_in is IndefiniteLengthInputStream)
		{
			((IndefiniteLengthInputStream)_in).SetEofOn00(eofOn00: true);
		}
	}
}
