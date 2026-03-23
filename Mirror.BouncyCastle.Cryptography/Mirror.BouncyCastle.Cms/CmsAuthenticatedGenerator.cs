using Mirror.BouncyCastle.Security;

namespace Mirror.BouncyCastle.Cms;

public class CmsAuthenticatedGenerator : CmsEnvelopedGenerator
{
	public CmsAuthenticatedGenerator()
	{
	}

	public CmsAuthenticatedGenerator(SecureRandom random)
		: base(random)
	{
	}
}
