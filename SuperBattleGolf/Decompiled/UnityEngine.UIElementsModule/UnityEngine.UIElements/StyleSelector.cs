using System;
using System.Linq;
using UnityEngine.Bindings;

namespace UnityEngine.UIElements;

[Serializable]
[VisibleToOtherModules(new string[] { "UnityEditor.UIBuilderModule" })]
internal class StyleSelector
{
	[SerializeField]
	private StyleSelectorPart[] m_Parts;

	[SerializeField]
	private StyleSelectorRelationship m_PreviousRelationship;

	public const int InvalidPseudoStateMask = -1;

	internal int pseudoStateMask = -1;

	internal int negatedPseudoStateMask = -1;

	public StyleSelectorPart[] parts
	{
		get
		{
			return m_Parts;
		}
		[VisibleToOtherModules(new string[] { "UnityEditor.UIBuilderModule" })]
		internal set
		{
			m_Parts = value;
		}
	}

	public StyleSelectorRelationship previousRelationship
	{
		get
		{
			return m_PreviousRelationship;
		}
		[VisibleToOtherModules(new string[] { "UnityEditor.UIBuilderModule" })]
		internal set
		{
			m_PreviousRelationship = value;
		}
	}

	public override string ToString()
	{
		return string.Join(", ", parts.Select((StyleSelectorPart p) => p.ToString()).ToArray());
	}
}
