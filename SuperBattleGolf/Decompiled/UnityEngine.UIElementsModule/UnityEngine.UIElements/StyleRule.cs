using System;
using UnityEngine.Bindings;

namespace UnityEngine.UIElements;

[Serializable]
[VisibleToOtherModules(new string[] { "UnityEditor.UIBuilderModule" })]
internal class StyleRule
{
	[SerializeField]
	private StyleProperty[] m_Properties = Array.Empty<StyleProperty>();

	[VisibleToOtherModules(new string[] { "UnityEditor.UIBuilderModule" })]
	[SerializeField]
	internal int line;

	[NonSerialized]
	[VisibleToOtherModules(new string[] { "UnityEditor.UIBuilderModule" })]
	internal int customPropertiesCount;

	public StyleProperty[] properties
	{
		get
		{
			return m_Properties;
		}
		[VisibleToOtherModules(new string[] { "UnityEditor.UIBuilderModule" })]
		internal set
		{
			m_Properties = value;
		}
	}
}
