using UnityEngine.Bindings;
using UnityEngine.Scripting;

namespace UnityEngine.Rendering;

[UsedByNativeCode]
[NativeType("Runtime/Export/Graphics/GraphicsTexture.bindings.h")]
public enum GraphicsTextureState
{
	Constructed,
	Initializing,
	InitializedOnRenderThread,
	DestroyQueued,
	Destroyed
}
