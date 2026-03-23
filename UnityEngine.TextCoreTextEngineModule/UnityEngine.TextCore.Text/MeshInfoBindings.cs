using UnityEngine.Bindings;
using UnityEngine.Scripting;

namespace UnityEngine.TextCore.Text;

[VisibleToOtherModules(new string[] { "UnityEngine.IMGUIModule" })]
[UsedByNativeCode("MeshInfo")]
[NativeHeader("Modules/TextCoreTextEngine/Native/IMGUI/MeshInfo.h")]
internal struct MeshInfoBindings
{
	public TextCoreVertex[] vertexData;

	public Material material;

	public int vertexCount;
}
