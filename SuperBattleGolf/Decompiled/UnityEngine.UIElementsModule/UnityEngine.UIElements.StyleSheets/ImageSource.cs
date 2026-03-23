using UnityEngine.Bindings;

namespace UnityEngine.UIElements.StyleSheets;

[VisibleToOtherModules(new string[] { "UnityEditor.UIBuilderModule" })]
internal struct ImageSource
{
	public Texture2D texture;

	public Sprite sprite;

	public VectorImage vectorImage;

	public RenderTexture renderTexture;

	public bool IsNull()
	{
		return texture == null && sprite == null && vectorImage == null && renderTexture == null;
	}
}
