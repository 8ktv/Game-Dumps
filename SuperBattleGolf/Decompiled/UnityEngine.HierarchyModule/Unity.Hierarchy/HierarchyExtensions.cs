using System;
using UnityEngine;
using UnityEngine.Bindings;

namespace Unity.Hierarchy;

[VisibleToOtherModules(new string[] { "UnityEditor.UIToolkitAuthoringModule" })]
internal static class HierarchyExtensions
{
	public static T GetNodeTypeHandler<T>(this Hierarchy hierarchy) where T : HierarchyNodeTypeHandler
	{
		return hierarchy.GetNodeTypeHandlerBase<T>();
	}

	public static HierarchyNodeTypeHandler GetNodeTypeHandler(this Hierarchy hierarchy, in HierarchyNode node)
	{
		HierarchyNodeTypeHandlerBase nodeTypeHandlerBase = hierarchy.GetNodeTypeHandlerBase(in node);
		return (nodeTypeHandlerBase is HierarchyNodeTypeHandler hierarchyNodeTypeHandler) ? hierarchyNodeTypeHandler : null;
	}

	public static HierarchyNodeTypeHandler GetNodeTypeHandler(this Hierarchy hierarchy, string nodeTypeName)
	{
		HierarchyNodeTypeHandlerBase nodeTypeHandlerBase = hierarchy.GetNodeTypeHandlerBase(nodeTypeName);
		return (nodeTypeHandlerBase is HierarchyNodeTypeHandler hierarchyNodeTypeHandler) ? hierarchyNodeTypeHandler : null;
	}

	public static HierarchyNodeTypeHandlerEnumerable EnumerateNodeTypeHandlers(this Hierarchy hierarchy)
	{
		return new HierarchyNodeTypeHandlerEnumerable(hierarchy);
	}

	public static HierarchyNode GetNode(this Hierarchy hierarchy, EntityId entityId)
	{
		if (entityId == EntityId.None)
		{
			return HierarchyNode.Null;
		}
		foreach (HierarchyNodeTypeHandler item in hierarchy.EnumerateNodeTypeHandlers())
		{
			if (item is IHierarchyEntityIdConverter hierarchyEntityIdConverter)
			{
				HierarchyNode lhs = hierarchyEntityIdConverter.GetNode(entityId);
				if (lhs != HierarchyNode.Null)
				{
					return lhs;
				}
			}
		}
		return HierarchyNode.Null;
	}

	public static void GetNodes(this Hierarchy hierarchy, ReadOnlySpan<EntityId> entityIds, Span<HierarchyNode> outNodes)
	{
		if (outNodes.Length != entityIds.Length)
		{
			throw new ArgumentException("entityIds and outNodes must have the same length.");
		}
		outNodes.Clear();
		foreach (HierarchyNodeTypeHandler item in hierarchy.EnumerateNodeTypeHandlers())
		{
			if (item is IHierarchyEntityIdConverter hierarchyEntityIdConverter)
			{
				hierarchyEntityIdConverter.GetNodes(entityIds, outNodes);
			}
		}
	}

	public static EntityId GetEntityId(this Hierarchy hierarchy, in HierarchyNode node)
	{
		if (node == HierarchyNode.Null)
		{
			return EntityId.None;
		}
		foreach (HierarchyNodeTypeHandler item in hierarchy.EnumerateNodeTypeHandlers())
		{
			if (item is IHierarchyEntityIdConverter hierarchyEntityIdConverter)
			{
				EntityId entityId = hierarchyEntityIdConverter.GetEntityId(in node);
				if (entityId != EntityId.None)
				{
					return entityId;
				}
			}
		}
		return EntityId.None;
	}

	public static void GetEntityIds(this Hierarchy hierarchy, ReadOnlySpan<HierarchyNode> nodes, Span<EntityId> outEntityIds)
	{
		if (outEntityIds.Length != nodes.Length)
		{
			throw new ArgumentException("nodes and outEntityIds must have the same length.");
		}
		outEntityIds.Clear();
		foreach (HierarchyNodeTypeHandler item in hierarchy.EnumerateNodeTypeHandlers())
		{
			if (item is IHierarchyEntityIdConverter hierarchyEntityIdConverter)
			{
				hierarchyEntityIdConverter.GetEntityIds(nodes, outEntityIds);
			}
		}
	}
}
