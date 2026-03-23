using System.Collections.Generic;
using Mirror.BouncyCastle.Math;
using Mirror.BouncyCastle.Tls.Crypto;

namespace Mirror.BouncyCastle.Tls;

public class DefaultTlsSrpConfigVerifier : TlsSrpConfigVerifier
{
	private static readonly List<Srp6Group> DefaultGroups;

	protected readonly IList<Srp6Group> m_groups;

	static DefaultTlsSrpConfigVerifier()
	{
		DefaultGroups = new List<Srp6Group>();
		DefaultGroups.Add(Srp6StandardGroups.rfc5054_1024);
		DefaultGroups.Add(Srp6StandardGroups.rfc5054_1536);
		DefaultGroups.Add(Srp6StandardGroups.rfc5054_2048);
		DefaultGroups.Add(Srp6StandardGroups.rfc5054_3072);
		DefaultGroups.Add(Srp6StandardGroups.rfc5054_4096);
		DefaultGroups.Add(Srp6StandardGroups.rfc5054_6144);
		DefaultGroups.Add(Srp6StandardGroups.rfc5054_8192);
	}

	public DefaultTlsSrpConfigVerifier()
		: this(DefaultGroups)
	{
	}

	public DefaultTlsSrpConfigVerifier(IList<Srp6Group> groups)
	{
		m_groups = new List<Srp6Group>(groups);
	}

	public virtual bool Accept(TlsSrpConfig srpConfig)
	{
		foreach (Srp6Group group in m_groups)
		{
			if (AreGroupsEqual(srpConfig, group))
			{
				return true;
			}
		}
		return false;
	}

	protected virtual bool AreGroupsEqual(TlsSrpConfig a, Srp6Group b)
	{
		BigInteger[] explicitNG = a.GetExplicitNG();
		if (AreParametersEqual(explicitNG[0], b.N))
		{
			return AreParametersEqual(explicitNG[1], b.G);
		}
		return false;
	}

	protected virtual bool AreParametersEqual(BigInteger a, BigInteger b)
	{
		if (a != b)
		{
			return a.Equals(b);
		}
		return true;
	}
}
