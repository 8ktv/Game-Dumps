using System;
using UnityEngine.Bindings;

namespace UnityEngine.UIElements;

[Serializable]
[VisibleToOtherModules(new string[] { "UnityEditor.UIBuilderModule" })]
internal struct UxmlProperty
{
	public string name;

	public string value;
}
