using System;

namespace Mirror.BouncyCastle.Asn1;

internal abstract class Asn1Type
{
	internal readonly Type m_platformType;

	internal Type PlatformType => m_platformType;

	internal Asn1Type(Type platformType)
	{
		m_platformType = platformType;
	}

	public sealed override bool Equals(object that)
	{
		return this == that;
	}

	public sealed override int GetHashCode()
	{
		return base.GetHashCode();
	}
}
