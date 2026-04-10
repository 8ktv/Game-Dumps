using System;
using UnityEngine.Bindings;

namespace UnityEngine.UIElements;

[Serializable]
[VisibleToOtherModules(new string[] { "UnityEngine.VectorGraphicsModule", "UnityEditor.VectorGraphicsModule" })]
internal struct GradientSettings
{
	public GradientType gradientType;

	public AddressMode addressMode;

	public Vector2 radialFocus;

	public RectInt location;
}
