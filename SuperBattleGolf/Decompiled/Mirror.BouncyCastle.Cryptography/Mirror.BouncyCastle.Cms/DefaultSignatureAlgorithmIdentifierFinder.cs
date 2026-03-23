using System;
using Mirror.BouncyCastle.Asn1.X509;
using Mirror.BouncyCastle.Operators.Utilities;

namespace Mirror.BouncyCastle.Cms;

[Obsolete("Use 'Mirror.BouncyCastle.Operators.Utilities.DefaultSignatureAlgorithmFinder' instead")]
public class DefaultSignatureAlgorithmIdentifierFinder : ISignatureAlgorithmFinder
{
	public AlgorithmIdentifier Find(string sigAlgName)
	{
		return DefaultSignatureAlgorithmFinder.Instance.Find(sigAlgName);
	}
}
