using System;
using UnityEngine.Bindings;
using UnityEngine.Internal;

namespace Unity.Hierarchy;

[NativeHeader("Modules/HierarchyCore/Public/HierarchyNode.h")]
public readonly struct HierarchyNode : IEquatable<HierarchyNode>
{
	private const int k_HierarchyNodeIdNull = 0;

	private const int k_HierarchyNodeVersionNull = 0;

	private static readonly HierarchyNode s_Null;

	private readonly int m_Id;

	private readonly int m_Version;

	public static ref readonly HierarchyNode Null => ref s_Null;

	public int Id => m_Id;

	public int Version => m_Version;

	public HierarchyNode()
	{
		m_Id = 0;
		m_Version = 0;
	}

	[ExcludeFromDocs]
	public static bool operator ==(in HierarchyNode lhs, in HierarchyNode rhs)
	{
		return lhs.Id == rhs.Id && lhs.Version == rhs.Version;
	}

	[ExcludeFromDocs]
	public static bool operator !=(in HierarchyNode lhs, in HierarchyNode rhs)
	{
		return !(lhs == rhs);
	}

	[ExcludeFromDocs]
	public bool Equals(HierarchyNode other)
	{
		return other.Id == Id && other.Version == Version;
	}

	[ExcludeFromDocs]
	public override string ToString()
	{
		return "HierarchyNode(" + ((this == Null) ? "Null" : $"{Id}:{Version}") + ")";
	}

	[ExcludeFromDocs]
	public override bool Equals(object obj)
	{
		return obj is HierarchyNode other && Equals(other);
	}

	[ExcludeFromDocs]
	public override int GetHashCode()
	{
		return HashCode.Combine(Id, Version);
	}
}
