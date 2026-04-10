using System;
using UnityEngine.Bindings;

namespace UnityEngine.UIElements;

[Serializable]
public sealed class VectorImage : ScriptableObject
{
	[SerializeField]
	[VisibleToOtherModules(new string[] { "UnityEngine.VectorGraphicsModule" })]
	internal int version = 0;

	[SerializeField]
	[VisibleToOtherModules(new string[] { "UnityEngine.VectorGraphicsModule", "UnityEditor.VectorGraphicsModule" })]
	internal Texture2D atlas = null;

	[VisibleToOtherModules(new string[] { "UnityEngine.VectorGraphicsModule", "UnityEditor.VectorGraphicsModule" })]
	[SerializeField]
	internal VectorImageVertex[] vertices = null;

	[VisibleToOtherModules(new string[] { "UnityEngine.VectorGraphicsModule" })]
	[SerializeField]
	internal ushort[] indices = null;

	[SerializeField]
	[VisibleToOtherModules(new string[] { "UnityEngine.VectorGraphicsModule" })]
	internal GradientSettings[] settings = null;

	[SerializeField]
	[VisibleToOtherModules(new string[] { "UnityEngine.VectorGraphicsModule" })]
	internal Vector2 size = Vector2.zero;

	public float width => size.x;

	public float height => size.y;

	private void OnDestroy()
	{
		if (atlas != null)
		{
			UIRUtility.Destroy(atlas);
		}
	}
}
