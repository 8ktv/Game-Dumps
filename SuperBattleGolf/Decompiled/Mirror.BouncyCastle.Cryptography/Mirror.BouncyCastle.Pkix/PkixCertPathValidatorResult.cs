using System;
using System.Text;
using Mirror.BouncyCastle.Crypto;

namespace Mirror.BouncyCastle.Pkix;

public class PkixCertPathValidatorResult
{
	private TrustAnchor trustAnchor;

	private PkixPolicyNode policyTree;

	private AsymmetricKeyParameter subjectPublicKey;

	public PkixPolicyNode PolicyTree => policyTree;

	public TrustAnchor TrustAnchor => trustAnchor;

	public AsymmetricKeyParameter SubjectPublicKey => subjectPublicKey;

	public PkixCertPathValidatorResult(TrustAnchor trustAnchor, PkixPolicyNode policyTree, AsymmetricKeyParameter subjectPublicKey)
	{
		this.trustAnchor = trustAnchor ?? throw new ArgumentNullException("trustAnchor");
		this.policyTree = policyTree;
		this.subjectPublicKey = subjectPublicKey ?? throw new ArgumentNullException("subjectPublicKey");
	}

	public object Clone()
	{
		return new PkixCertPathValidatorResult(TrustAnchor, PolicyTree, SubjectPublicKey);
	}

	public override string ToString()
	{
		StringBuilder stringBuilder = new StringBuilder();
		stringBuilder.AppendLine("PKIXCertPathValidatorResult: [");
		stringBuilder.Append("  Trust Anchor: ").Append(TrustAnchor).AppendLine();
		stringBuilder.Append("  Policy Tree: ").Append(PolicyTree).AppendLine();
		stringBuilder.Append("  Subject Public Key: ").Append(SubjectPublicKey).AppendLine();
		return stringBuilder.ToString();
	}
}
