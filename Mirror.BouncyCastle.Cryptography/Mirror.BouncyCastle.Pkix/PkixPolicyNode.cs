using System.Collections.Generic;
using System.Text;
using Mirror.BouncyCastle.Asn1.X509;
using Mirror.BouncyCastle.Utilities.Collections;

namespace Mirror.BouncyCastle.Pkix;

public class PkixPolicyNode
{
	protected IList<PkixPolicyNode> mChildren;

	protected int mDepth;

	protected ISet<string> mExpectedPolicies;

	protected PkixPolicyNode mParent;

	protected ISet<PolicyQualifierInfo> mPolicyQualifiers;

	protected string mValidPolicy;

	protected bool mCritical;

	public virtual int Depth => mDepth;

	public virtual IEnumerable<PkixPolicyNode> Children => CollectionUtilities.Proxy(mChildren);

	public virtual bool IsCritical
	{
		get
		{
			return mCritical;
		}
		set
		{
			mCritical = value;
		}
	}

	public virtual ISet<PolicyQualifierInfo> PolicyQualifiers => new HashSet<PolicyQualifierInfo>(mPolicyQualifiers);

	public virtual string ValidPolicy => mValidPolicy;

	public virtual bool HasChildren => mChildren.Count != 0;

	public virtual ISet<string> ExpectedPolicies
	{
		get
		{
			return new HashSet<string>(mExpectedPolicies);
		}
		set
		{
			mExpectedPolicies = new HashSet<string>(value);
		}
	}

	public virtual PkixPolicyNode Parent
	{
		get
		{
			return mParent;
		}
		set
		{
			mParent = value;
		}
	}

	public PkixPolicyNode(IEnumerable<PkixPolicyNode> children, int depth, ISet<string> expectedPolicies, PkixPolicyNode parent, ISet<PolicyQualifierInfo> policyQualifiers, string validPolicy, bool critical)
	{
		if (children == null)
		{
			mChildren = new List<PkixPolicyNode>();
		}
		else
		{
			mChildren = new List<PkixPolicyNode>(children);
		}
		mDepth = depth;
		mExpectedPolicies = expectedPolicies;
		mParent = parent;
		mPolicyQualifiers = policyQualifiers;
		mValidPolicy = validPolicy;
		mCritical = critical;
	}

	public virtual void AddChild(PkixPolicyNode child)
	{
		child.Parent = this;
		mChildren.Add(child);
	}

	public virtual void RemoveChild(PkixPolicyNode child)
	{
		mChildren.Remove(child);
	}

	public override string ToString()
	{
		return ToString("");
	}

	public virtual string ToString(string indent)
	{
		StringBuilder stringBuilder = new StringBuilder();
		stringBuilder.Append(indent);
		stringBuilder.Append(mValidPolicy);
		stringBuilder.AppendLine(" {");
		foreach (PkixPolicyNode mChild in mChildren)
		{
			stringBuilder.Append(mChild.ToString(indent + "    "));
		}
		stringBuilder.Append(indent);
		stringBuilder.AppendLine("}");
		return stringBuilder.ToString();
	}

	public virtual object Clone()
	{
		return Copy();
	}

	public virtual PkixPolicyNode Copy()
	{
		PkixPolicyNode pkixPolicyNode = new PkixPolicyNode(new List<PkixPolicyNode>(), mDepth, new HashSet<string>(mExpectedPolicies), null, new HashSet<PolicyQualifierInfo>(mPolicyQualifiers), mValidPolicy, mCritical);
		foreach (PkixPolicyNode mChild in mChildren)
		{
			PkixPolicyNode pkixPolicyNode2 = mChild.Copy();
			pkixPolicyNode2.Parent = pkixPolicyNode;
			pkixPolicyNode.AddChild(pkixPolicyNode2);
		}
		return pkixPolicyNode;
	}
}
