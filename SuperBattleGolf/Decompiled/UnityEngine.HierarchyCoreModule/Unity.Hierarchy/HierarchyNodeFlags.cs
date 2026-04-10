using System;
using UnityEngine.Bindings;

namespace Unity.Hierarchy;

[Flags]
[NativeHeader("Modules/HierarchyCore/Public/HierarchyNodeFlags.h")]
public enum HierarchyNodeFlags : uint
{
	None = 0u,
	Expanded = 1u,
	Selected = 2u,
	Cut = 4u,
	Hidden = 8u
}
