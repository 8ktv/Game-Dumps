using System;
using UnityEngine.Bindings;

namespace Unity.Hierarchy;

[Flags]
[NativeHeader("Modules/HierarchyCore/Public/HierarchyTraversalDirection.h")]
public enum HierarchyTraversalDirection : uint
{
	Parents = 0u,
	Children = 1u
}
