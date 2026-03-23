using System;
using UnityEngine.Bindings;

namespace UnityEngine.Animations;

[NativeHeader("Modules/Animation/BoundProperty.h")]
public readonly struct BoundProperty : IEquatable<BoundProperty>, IComparable<BoundProperty>
{
	private readonly int m_Index;

	private readonly int m_Version;

	public int index => m_Index;

	public int version => m_Version;

	public static BoundProperty Null => default(BoundProperty);

	public static bool operator ==(BoundProperty lhs, BoundProperty rhs)
	{
		return lhs.m_Index == rhs.m_Index && lhs.m_Version == rhs.m_Version;
	}

	public static bool operator !=(BoundProperty lhs, BoundProperty rhs)
	{
		return !(lhs == rhs);
	}

	public override bool Equals(object compare)
	{
		return compare is BoundProperty boundProperty && Equals(boundProperty);
	}

	public bool Equals(BoundProperty boundProperty)
	{
		return boundProperty.m_Index == m_Index && boundProperty.m_Version == m_Version;
	}

	public int CompareTo(BoundProperty other)
	{
		return m_Index - other.m_Index;
	}

	public override int GetHashCode()
	{
		return (m_Version * 397) ^ m_Index;
	}
}
