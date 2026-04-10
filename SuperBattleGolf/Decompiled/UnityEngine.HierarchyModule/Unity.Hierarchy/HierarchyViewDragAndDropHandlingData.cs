using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Bindings;
using UnityEngine.UIElements;

namespace Unity.Hierarchy;

[VisibleToOtherModules(new string[] { "UnityEditor.UIToolkitAuthoringModule" })]
internal readonly ref struct HierarchyViewDragAndDropHandlingData
{
	private readonly DragAndDropData m_DragAndDropData;

	public HierarchyNode Parent { get; }

	public HierarchyNode Target { get; }

	public int InsertAtIndex { get; }

	public DragAndDropPosition DropPosition { get; }

	public HierarchyView View { get; }

	public IReadOnlyList<EntityId> EntityIds => m_DragAndDropData.entityIds;

	public string[] Paths => m_DragAndDropData.paths;

	public object Source => m_DragAndDropData.source;

	[VisibleToOtherModules(new string[] { "UnityEditor.HierarchyModule" })]
	internal EventModifiers EventModifiers { get; }

	public object GetGenericData(string key)
	{
		return m_DragAndDropData.GetGenericData(key);
	}

	internal HierarchyViewDragAndDropHandlingData(in HierarchyNode parent, in HierarchyNode target, int insertAtIndex, DragAndDropPosition dropPosition, DragAndDropData dragAndDropData, HierarchyView view, EventModifiers eventModifiers)
	{
		Parent = parent;
		Target = target;
		InsertAtIndex = insertAtIndex;
		DropPosition = dropPosition;
		m_DragAndDropData = dragAndDropData;
		View = view;
		EventModifiers = eventModifiers;
	}
}
