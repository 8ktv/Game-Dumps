using UnityEngine.Bindings;

namespace UnityEngine.TextCore.Text;

[VisibleToOtherModules(new string[] { "UnityEngine.IMGUIModule", "UnityEngine.UIElementsModule" })]
internal enum TextOverflowMode
{
	Overflow = 0,
	Ellipsis = 1,
	Masking = 2,
	Truncate = 3,
	ScrollRect = 4,
	Linked = 6
}
