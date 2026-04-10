using UnityEngine.Bindings;

namespace Unity.Hierarchy;

[NativeHeader("Modules/HierarchyCore/Public/HierarchyPropertyStorageType.h")]
public enum HierarchyPropertyStorageType
{
	Sparse = 0,
	Dense = 1,
	Blob = 2,
	Default = 1
}
