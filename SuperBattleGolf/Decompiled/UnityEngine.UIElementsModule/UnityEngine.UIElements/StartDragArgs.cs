using System.Collections;
using System.Collections.Generic;

namespace UnityEngine.UIElements;

public struct StartDragArgs
{
	public string title { get; }

	public DragVisualMode visualMode { get; }

	internal Hashtable genericData { get; private set; }

	internal IEnumerable<Object> unityObjectReferences { get; private set; }

	internal string[] assetPaths { get; private set; }

	public StartDragArgs(string title, DragVisualMode visualMode)
	{
		this.title = title;
		this.visualMode = visualMode;
		genericData = null;
		assetPaths = null;
		unityObjectReferences = null;
	}

	internal StartDragArgs(string title, object target)
	{
		this.title = title;
		visualMode = DragVisualMode.Move;
		genericData = null;
		assetPaths = null;
		unityObjectReferences = null;
		SetGenericData("__unity-drag-and-drop__source-view", target);
	}

	public void SetGenericData(string key, object data)
	{
		if (genericData == null)
		{
			Hashtable hashtable = (genericData = new Hashtable());
		}
		genericData[key] = data;
	}

	public void SetUnityObjectReferences(IEnumerable<Object> references)
	{
		unityObjectReferences = references;
	}

	public void SetPaths(string[] paths)
	{
		assetPaths = paths;
	}
}
