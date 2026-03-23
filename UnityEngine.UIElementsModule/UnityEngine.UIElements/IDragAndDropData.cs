using System.Collections.Generic;

namespace UnityEngine.UIElements;

internal interface IDragAndDropData
{
	object userData { get; }

	IEnumerable<Object> unityObjectReferences { get; }

	string[] paths { get; set; }

	object GetGenericData(string key);
}
