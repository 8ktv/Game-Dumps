using UnityEngine.Bindings;

namespace UnityEngine.TextCore.Text;

[VisibleToOtherModules(new string[] { "UnityEngine.UIElementsModule", "UnityEngine.IMGUIModule" })]
[NativeHeader("Modules/TextCoreTextEngine/Native/TextElementInfo.h")]
internal struct NativeTextElementInfo
{
	public int glyphID;

	public TextCoreVertex bottomLeft;

	public TextCoreVertex topLeft;

	public TextCoreVertex topRight;

	public TextCoreVertex bottomRight;
}
