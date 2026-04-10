using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace UnityEngine.UIElements;

public struct StartDragArgs
{
	public string title { get; }

	public DragVisualMode visualMode { get; }

	internal EventModifiers modifiers { get; set; }

	internal Hashtable genericData { get; private set; }

	internal IReadOnlyList<EntityId> entityIds { get; private set; }

	internal string[] assetPaths { get; private set; }

	public StartDragArgs(string title, DragVisualMode visualMode)
	{
		this.title = title;
		this.visualMode = visualMode;
		genericData = null;
		assetPaths = null;
		entityIds = null;
		modifiers = EventModifiers.None;
	}

	internal StartDragArgs(string title, object target)
	{
		this = default(StartDragArgs);
		this.title = title;
		visualMode = DragVisualMode.Move;
		genericData = null;
		assetPaths = null;
		entityIds = null;
		SetGenericData("__unity-drag-and-drop__source-view", target);
	}

	internal StartDragArgs(string title, DragVisualMode visualMode, EventModifiers modifiers)
		: this(title, visualMode)
	{
		this.modifiers = modifiers;
	}

	public void SetGenericData(string key, object data)
	{
		if (genericData == null)
		{
			Hashtable hashtable = (genericData = new Hashtable());
		}
		genericData[key] = data;
	}

	[Obsolete("Use SetEntityIds instead, and call Object.GetEntityId() if you really need to convert from a Unity object to an EntityId.")]
	public void SetUnityObjectReferences(IEnumerable<Object> references)
	{
		SetEntityIds(references.Select((Object x) => x.GetEntityId()).ToList());
	}

	public void SetEntityIds(IReadOnlyList<EntityId> ids)
	{
		entityIds = ids;
	}

	public void SetPaths(string[] paths)
	{
		assetPaths = paths;
	}
}
