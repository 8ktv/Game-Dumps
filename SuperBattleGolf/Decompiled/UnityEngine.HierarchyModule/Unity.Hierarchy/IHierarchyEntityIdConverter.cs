using System;
using UnityEngine;
using UnityEngine.Bindings;

namespace Unity.Hierarchy;

[VisibleToOtherModules(new string[] { "UnityEditor.UIToolkitAuthoringModule" })]
internal interface IHierarchyEntityIdConverter
{
	protected internal HierarchyNode GetNode(EntityId entityId);

	protected internal void GetNodes(ReadOnlySpan<EntityId> entityIds, Span<HierarchyNode> outNodes);

	protected internal EntityId GetEntityId(in HierarchyNode node);

	protected internal void GetEntityIds(ReadOnlySpan<HierarchyNode> nodes, Span<EntityId> outEntityIds);
}
