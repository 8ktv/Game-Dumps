using System;
using Mirror.BouncyCastle.Asn1;
using Mirror.BouncyCastle.Asn1.X509;
using Mirror.BouncyCastle.Operators.Utilities;

namespace Mirror.BouncyCastle.Cms;

[Obsolete("Use 'Mirror.BouncyCastle.Operators.Utilities.DefaultDigestAlgorithmFinder' instead")]
public class DefaultDigestAlgorithmIdentifierFinder : IDigestAlgorithmFinder
{
	public AlgorithmIdentifier Find(AlgorithmIdentifier sigAlgId)
	{
		return DefaultDigestAlgorithmFinder.Instance.Find(sigAlgId);
	}

	public virtual AlgorithmIdentifier Find(DerObjectIdentifier digAlgOid)
	{
		return DefaultDigestAlgorithmFinder.Instance.Find(digAlgOid);
	}

	public AlgorithmIdentifier Find(string digAlgName)
	{
		return DefaultDigestAlgorithmFinder.Instance.Find(digAlgName);
	}
}
