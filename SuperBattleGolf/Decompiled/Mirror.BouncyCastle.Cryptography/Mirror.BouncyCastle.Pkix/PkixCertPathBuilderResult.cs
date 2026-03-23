using System;
using System.Text;
using Mirror.BouncyCastle.Crypto;

namespace Mirror.BouncyCastle.Pkix;

public class PkixCertPathBuilderResult : PkixCertPathValidatorResult
{
	private PkixCertPath certPath;

	public PkixCertPath CertPath => certPath;

	public PkixCertPathBuilderResult(PkixCertPath certPath, TrustAnchor trustAnchor, PkixPolicyNode policyTree, AsymmetricKeyParameter subjectPublicKey)
		: base(trustAnchor, policyTree, subjectPublicKey)
	{
		this.certPath = certPath ?? throw new ArgumentNullException("certPath");
	}

	public override string ToString()
	{
		StringBuilder stringBuilder = new StringBuilder();
		stringBuilder.AppendLine("SimplePKIXCertPathBuilderResult: [");
		stringBuilder.Append("  Certification Path: ").Append(CertPath).AppendLine();
		stringBuilder.Append("  Trust Anchor: ").Append(base.TrustAnchor.TrustedCert.IssuerDN).AppendLine();
		stringBuilder.Append("  Subject Public Key: ").Append(base.SubjectPublicKey).AppendLine();
		return stringBuilder.ToString();
	}
}
