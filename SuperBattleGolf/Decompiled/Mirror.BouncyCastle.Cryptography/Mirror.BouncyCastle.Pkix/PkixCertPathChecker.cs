using System.Collections.Generic;
using Mirror.BouncyCastle.X509;

namespace Mirror.BouncyCastle.Pkix;

public abstract class PkixCertPathChecker
{
	public abstract void Init(bool forward);

	public abstract bool IsForwardCheckingSupported();

	public abstract ISet<string> GetSupportedExtensions();

	public abstract void Check(X509Certificate cert, ISet<string> unresolvedCritExts);

	public virtual object Clone()
	{
		return MemberwiseClone();
	}
}
