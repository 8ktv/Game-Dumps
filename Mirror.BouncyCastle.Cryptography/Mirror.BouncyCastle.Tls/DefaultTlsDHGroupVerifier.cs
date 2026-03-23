using System.Collections.Generic;
using Mirror.BouncyCastle.Math;
using Mirror.BouncyCastle.Tls.Crypto;

namespace Mirror.BouncyCastle.Tls;

public class DefaultTlsDHGroupVerifier : TlsDHGroupVerifier
{
	public static readonly int DefaultMinimumPrimeBits;

	private static readonly List<DHGroup> DefaultGroups;

	protected readonly IList<DHGroup> m_groups;

	protected readonly int m_minimumPrimeBits;

	public virtual int MinimumPrimeBits => m_minimumPrimeBits;

	private static void AddDefaultGroup(DHGroup dhGroup)
	{
		DefaultGroups.Add(dhGroup);
	}

	static DefaultTlsDHGroupVerifier()
	{
		DefaultMinimumPrimeBits = 2048;
		DefaultGroups = new List<DHGroup>();
		AddDefaultGroup(DHStandardGroups.rfc3526_2048);
		AddDefaultGroup(DHStandardGroups.rfc3526_3072);
		AddDefaultGroup(DHStandardGroups.rfc3526_4096);
		AddDefaultGroup(DHStandardGroups.rfc3526_6144);
		AddDefaultGroup(DHStandardGroups.rfc3526_8192);
		AddDefaultGroup(DHStandardGroups.rfc7919_ffdhe2048);
		AddDefaultGroup(DHStandardGroups.rfc7919_ffdhe3072);
		AddDefaultGroup(DHStandardGroups.rfc7919_ffdhe4096);
		AddDefaultGroup(DHStandardGroups.rfc7919_ffdhe6144);
		AddDefaultGroup(DHStandardGroups.rfc7919_ffdhe8192);
	}

	public DefaultTlsDHGroupVerifier()
		: this(DefaultMinimumPrimeBits)
	{
	}

	public DefaultTlsDHGroupVerifier(int minimumPrimeBits)
		: this(DefaultGroups, minimumPrimeBits)
	{
	}

	public DefaultTlsDHGroupVerifier(IList<DHGroup> groups, int minimumPrimeBits)
	{
		m_groups = new List<DHGroup>(groups);
		m_minimumPrimeBits = minimumPrimeBits;
	}

	public virtual bool Accept(DHGroup dhGroup)
	{
		if (CheckMinimumPrimeBits(dhGroup))
		{
			return CheckGroup(dhGroup);
		}
		return false;
	}

	protected virtual bool AreGroupsEqual(DHGroup a, DHGroup b)
	{
		if (a != b)
		{
			if (AreParametersEqual(a.P, b.P))
			{
				return AreParametersEqual(a.G, b.G);
			}
			return false;
		}
		return true;
	}

	protected virtual bool AreParametersEqual(BigInteger a, BigInteger b)
	{
		if (a != b)
		{
			return a.Equals(b);
		}
		return true;
	}

	protected virtual bool CheckGroup(DHGroup dhGroup)
	{
		foreach (DHGroup group in m_groups)
		{
			if (AreGroupsEqual(dhGroup, group))
			{
				return true;
			}
		}
		return false;
	}

	protected virtual bool CheckMinimumPrimeBits(DHGroup dhGroup)
	{
		return dhGroup.P.BitLength >= MinimumPrimeBits;
	}
}
