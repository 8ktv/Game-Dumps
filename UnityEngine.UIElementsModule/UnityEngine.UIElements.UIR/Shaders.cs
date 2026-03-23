#define UNITY_ASSERTIONS
namespace UnityEngine.UIElements.UIR;

internal static class Shaders
{
	public static readonly string k_AtlasBlit = "Hidden/Internal-UIRAtlasBlitCopy";

	public static readonly string k_Editor = "Hidden/UIElements/EditorUIE";

	public static readonly string k_Runtime = "Hidden/Internal-UIRDefault";

	public static readonly string k_RuntimeWorld = "Hidden/Internal-UIRDefaultWorld";

	public static readonly string k_RuntimeGaussianBlur = "Hidden/UIR/GaussianBlur";

	public static readonly string k_RuntimeColorEffect = "Hidden/UIR/ColorEffect";

	public static readonly string k_ColorConversionBlit = "Hidden/Internal-UIE-ColorConversionBlit";

	public static readonly string k_ForceGammaKeyword = "UIE_FORCE_GAMMA";

	private static Material s_RuntimeMaterial;

	private static Material s_RuntimeWorldMaterial;

	private static Material s_EditorMaterial;

	private static int s_RefCount;

	public static Material runtimeMaterial => GetOrCreateMaterial(ref s_RuntimeMaterial, k_Runtime);

	public static Material runtimeWorldMaterial => GetOrCreateMaterial(ref s_RuntimeWorldMaterial, k_RuntimeWorld);

	public static Material editorMaterial => GetOrCreateMaterial(ref s_EditorMaterial, k_Editor);

	private static Material GetOrCreateMaterial(ref Material material, string shaderName)
	{
		if (material == null)
		{
			Shader shader = Shader.Find(shaderName);
			if (shader == null)
			{
				Debug.LogError("Could not find shader '" + shaderName + "'");
				return null;
			}
			material = new Material(shader);
			material.hideFlags = HideFlags.DontSave;
		}
		return material;
	}

	public static void Acquire()
	{
		s_RefCount++;
	}

	public static void Release()
	{
		s_RefCount--;
		Debug.Assert(s_RefCount >= 0, "UIR materials acquire/release don't match.");
		if (s_RefCount < 1)
		{
			s_RefCount = 0;
			UIRUtility.Destroy(s_RuntimeMaterial);
			UIRUtility.Destroy(s_RuntimeWorldMaterial);
			UIRUtility.Destroy(s_EditorMaterial);
			s_RuntimeMaterial = null;
			s_RuntimeWorldMaterial = null;
			s_EditorMaterial = null;
		}
	}
}
