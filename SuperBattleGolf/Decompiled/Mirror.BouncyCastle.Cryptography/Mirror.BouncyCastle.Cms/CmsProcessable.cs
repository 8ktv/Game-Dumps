using System.IO;

namespace Mirror.BouncyCastle.Cms;

public interface CmsProcessable
{
	void Write(Stream outStream);
}
