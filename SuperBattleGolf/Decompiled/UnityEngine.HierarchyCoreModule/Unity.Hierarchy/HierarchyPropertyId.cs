using System;
using UnityEngine.Bindings;

namespace Unity.Hierarchy;

[NativeHeader("Modules/HierarchyCore/Public/HierarchyPropertyId.h")]
internal readonly struct HierarchyPropertyId : IEquatable<HierarchyPropertyId>
{
	private const int k_HierarchyPropertyIdNull = 0;

	private static readonly HierarchyPropertyId s_Null;

	private readonly int m_Id;

	public static ref readonly HierarchyPropertyId Null => ref s_Null;

	public int Id => m_Id;

	public HierarchyPropertyId()
	{
		m_Id = 0;
	}

	internal HierarchyPropertyId(int id)
	{
		m_Id = id;
	}

	public static bool operator ==(in HierarchyPropertyId lhs, in HierarchyPropertyId rhs)
	{
		return lhs.Id == rhs.Id;
	}

	public static bool operator !=(in HierarchyPropertyId lhs, in HierarchyPropertyId rhs)
	{
		return !(lhs == rhs);
	}

	public bool Equals(HierarchyPropertyId other)
	{
		return other.Id == Id;
	}

	public override string ToString()
	{
		return string.Format("{0}({1})", "HierarchyPropertyId", (this == Null) ? "Null" : ((object)Id));
	}

	public override bool Equals(object obj)
	{
		return obj is HierarchyPropertyId other && Equals(other);
	}

	public override int GetHashCode()
	{
		return Id.GetHashCode();
	}
}
