using System.IO;
using Mirror.BouncyCastle.Utilities.IO;

namespace Mirror.BouncyCastle.Cms;

public class CmsProcessableFile : CmsProcessable, CmsReadable
{
	private const int DefaultBufSize = 32768;

	private readonly FileInfo _file;

	private readonly int _bufSize;

	public CmsProcessableFile(FileInfo file)
		: this(file, 32768)
	{
	}

	public CmsProcessableFile(FileInfo file, int bufSize)
	{
		_file = file;
		_bufSize = bufSize;
	}

	public virtual Stream GetInputStream()
	{
		return new FileStream(_file.FullName, FileMode.Open, FileAccess.Read, FileShare.Read, _bufSize);
	}

	public virtual void Write(Stream zOut)
	{
		using FileStream inStr = _file.OpenRead();
		Streams.PipeAll(inStr, zOut, _bufSize);
	}
}
