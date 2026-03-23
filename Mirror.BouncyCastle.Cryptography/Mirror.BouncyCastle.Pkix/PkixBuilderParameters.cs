using System.Collections.Generic;
using System.Text;
using Mirror.BouncyCastle.Security;
using Mirror.BouncyCastle.Utilities.Collections;
using Mirror.BouncyCastle.X509;

namespace Mirror.BouncyCastle.Pkix;

public class PkixBuilderParameters : PkixParameters
{
	private int maxPathLength = 5;

	private HashSet<X509Certificate> excludedCerts = new HashSet<X509Certificate>();

	public virtual int MaxPathLength
	{
		get
		{
			return maxPathLength;
		}
		set
		{
			if (value < -1)
			{
				throw new InvalidParameterException("The maximum path length parameter can not be less than -1.");
			}
			maxPathLength = value;
		}
	}

	public static PkixBuilderParameters GetInstance(PkixParameters pkixParams)
	{
		PkixBuilderParameters pkixBuilderParameters = new PkixBuilderParameters(pkixParams.GetTrustAnchors(), pkixParams.GetTargetConstraintsCert(), pkixParams.GetTargetConstraintsAttrCert());
		pkixBuilderParameters.SetParams(pkixParams);
		return pkixBuilderParameters;
	}

	public PkixBuilderParameters(ISet<TrustAnchor> trustAnchors, ISelector<X509Certificate> targetConstraintsCert)
		: this(trustAnchors, targetConstraintsCert, null)
	{
	}

	public PkixBuilderParameters(ISet<TrustAnchor> trustAnchors, ISelector<X509Certificate> targetConstraintsCert, ISelector<X509V2AttributeCertificate> targetConstraintsAttrCert)
		: base(trustAnchors)
	{
		SetTargetConstraintsCert(targetConstraintsCert);
		SetTargetConstraintsAttrCert(targetConstraintsAttrCert);
	}

	public virtual ISet<X509Certificate> GetExcludedCerts()
	{
		return new HashSet<X509Certificate>(excludedCerts);
	}

	public virtual void SetExcludedCerts(ISet<X509Certificate> excludedCerts)
	{
		if (excludedCerts == null)
		{
			this.excludedCerts = new HashSet<X509Certificate>();
		}
		else
		{
			this.excludedCerts = new HashSet<X509Certificate>(excludedCerts);
		}
	}

	protected override void SetParams(PkixParameters parameters)
	{
		base.SetParams(parameters);
		if (parameters is PkixBuilderParameters pkixBuilderParameters)
		{
			maxPathLength = pkixBuilderParameters.maxPathLength;
			excludedCerts = new HashSet<X509Certificate>(pkixBuilderParameters.excludedCerts);
		}
	}

	public override object Clone()
	{
		PkixBuilderParameters pkixBuilderParameters = new PkixBuilderParameters(GetTrustAnchors(), GetTargetConstraintsCert(), GetTargetConstraintsAttrCert());
		pkixBuilderParameters.SetParams(this);
		return pkixBuilderParameters;
	}

	public override string ToString()
	{
		StringBuilder stringBuilder = new StringBuilder();
		stringBuilder.AppendLine("PkixBuilderParameters [");
		stringBuilder.Append(base.ToString());
		stringBuilder.Append("  Maximum Path Length: ");
		stringBuilder.Append(MaxPathLength);
		stringBuilder.AppendLine();
		stringBuilder.AppendLine("]");
		return stringBuilder.ToString();
	}
}
