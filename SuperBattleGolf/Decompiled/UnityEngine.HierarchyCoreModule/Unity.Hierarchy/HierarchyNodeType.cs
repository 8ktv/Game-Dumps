using System;
using UnityEngine.Bindings;
using UnityEngine.Internal;

namespace Unity.Hierarchy;

[NativeHeader("Modules/HierarchyCore/Public/HierarchyNodeType.h")]
public readonly struct HierarchyNodeType : IEquatable<HierarchyNodeType>
{
	internal const int k_HierarchyNodeTypeNull = 0;

	private static readonly HierarchyNodeType s_Null;

	private readonly int m_Id;

	public static ref readonly HierarchyNodeType Null => ref s_Null;

	public int Id => m_Id;

	public HierarchyNodeType()
	{
		m_Id = 0;
	}

	internal HierarchyNodeType(int id)
	{
		m_Id = id;
	}

	[ExcludeFromDocs]
	public static bool operator ==(in HierarchyNodeType lhs, in HierarchyNodeType rhs)
	{
		return lhs.Id == rhs.Id;
	}

	[ExcludeFromDocs]
	public static bool operator !=(in HierarchyNodeType lhs, in HierarchyNodeType rhs)
	{
		return !(lhs == rhs);
	}

	[ExcludeFromDocs]
	public bool Equals(HierarchyNodeType other)
	{
		return other.Id == Id;
	}

	[ExcludeFromDocs]
	public override string ToString()
	{
		return string.Format("{0}({1})", "HierarchyNodeType", (this == Null) ? "Null" : ((object)Id));
	}

	[ExcludeFromDocs]
	public override bool Equals(object obj)
	{
		return obj is HierarchyNodeType other && Equals(other);
	}

	[ExcludeFromDocs]
	public override int GetHashCode()
	{
		return Id.GetHashCode();
	}
}
