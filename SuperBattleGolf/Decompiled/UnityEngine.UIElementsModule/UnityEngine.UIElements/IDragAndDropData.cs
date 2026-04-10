using System.Collections.Generic;

namespace UnityEngine.UIElements;

internal interface IDragAndDropData
{
	object userData { get; }

	IEnumerable<Object> unityObjectReferences { get; }

	IReadOnlyList<EntityId> entityIds { get; }

	string[] paths { get; set; }

	object GetGenericData(string key);
}
