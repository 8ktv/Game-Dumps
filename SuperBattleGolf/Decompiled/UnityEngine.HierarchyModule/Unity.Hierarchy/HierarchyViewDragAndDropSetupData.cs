using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Bindings;

namespace Unity.Hierarchy;

[VisibleToOtherModules]
internal readonly ref struct HierarchyViewDragAndDropSetupData
{
	private readonly Dictionary<string, object> m_GenericData;

	public ReadOnlySpan<HierarchyNode> Nodes { get; }

	public List<EntityId> EntityIds { get; }

	public List<string> Paths { get; }

	public HierarchyView View { get; }

	public void SetGenericData(string key, object value)
	{
		m_GenericData[key] = value;
	}

	internal HierarchyViewDragAndDropSetupData(ReadOnlySpan<HierarchyNode> nodes, List<EntityId> entityIds, List<string> paths, HierarchyView view, Dictionary<string, object> genericData)
	{
		Nodes = nodes;
		EntityIds = entityIds;
		Paths = paths;
		View = view;
		m_GenericData = genericData;
	}
}
