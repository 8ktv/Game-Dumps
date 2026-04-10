using UnityEngine.Bindings;

namespace Unity.Hierarchy;

[NativeHeader("Modules/HierarchyCore/Public/HierarchyPropertyDescriptor.h")]
public struct HierarchyPropertyDescriptor
{
	private int m_Size;

	private HierarchyPropertyStorageType m_Type;

	public int Size
	{
		get
		{
			return m_Size;
		}
		set
		{
			m_Size = value;
		}
	}

	public HierarchyPropertyStorageType Type
	{
		get
		{
			return m_Type;
		}
		set
		{
			m_Type = value;
		}
	}
}
