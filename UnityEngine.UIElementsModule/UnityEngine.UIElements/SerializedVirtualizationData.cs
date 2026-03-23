using System;
using UnityEngine.Bindings;

namespace UnityEngine.UIElements;

[Serializable]
[VisibleToOtherModules(new string[] { "UnityEditor.UIBuilderModule" })]
internal class SerializedVirtualizationData
{
	public Vector2 scrollOffset;

	public int firstVisibleIndex;

	public float contentPadding;

	public float contentHeight;

	public int anchoredItemIndex;

	public float anchorOffset;
}
