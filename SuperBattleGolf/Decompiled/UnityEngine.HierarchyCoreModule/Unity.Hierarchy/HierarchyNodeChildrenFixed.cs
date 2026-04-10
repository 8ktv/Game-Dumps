using System.Runtime.InteropServices;
using Unity.Collections.LowLevel.Unsafe;

namespace Unity.Hierarchy;

[StructLayout(LayoutKind.Explicit, Size = 32)]
internal struct HierarchyNodeChildrenFixed
{
	public const int Capacity = 4;

	[FieldOffset(0)]
	private HierarchyNode m_Node1;

	[FieldOffset(8)]
	private HierarchyNode m_Node2;

	[FieldOffset(16)]
	private HierarchyNode m_Node3;

	[FieldOffset(24)]
	private HierarchyNode m_Node4;

	public unsafe HierarchyNode* Ptr => (HierarchyNode*)UnsafeUtility.AddressOf(ref m_Node1);
}
