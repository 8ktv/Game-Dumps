using UnityEngine.Bindings;
using UnityEngine.Scripting;

namespace UnityEngine.TextCore.Text;

[NativeHeader("Modules/TextCoreTextEngine/Native/IMGUI/MeshInfo.h")]
[UsedByNativeCode("MeshInfo")]
[VisibleToOtherModules(new string[] { "UnityEngine.IMGUIModule" })]
internal struct MeshInfoBindings
{
	public TextCoreVertex[] vertexData;

	public Material material;

	public int vertexCount;
}
