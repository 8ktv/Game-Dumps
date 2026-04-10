using System;
using System.Collections.Generic;

namespace UnityEngine.UIElements;

public abstract class DragAndDropData : IDragAndDropData
{
	internal const string dragSourceKey = "__unity-drag-and-drop__source-view";

	object IDragAndDropData.userData => GetGenericData("__unity-drag-and-drop__source-view");

	public abstract object source { get; }

	public abstract DragVisualMode visualMode { get; }

	[Obsolete("Use entityIDs instead, and call Object.FindObjectFromInstanceID(entityId) if you need to get a Unity object from an EntityId.")]
	public abstract IEnumerable<Object> unityObjectReferences { get; }

	public abstract IReadOnlyList<EntityId> entityIds { get; }

	public virtual string[] paths { get; set; }

	public abstract object GetGenericData(string key);

	public abstract void SetGenericData(string key, object data);
}
