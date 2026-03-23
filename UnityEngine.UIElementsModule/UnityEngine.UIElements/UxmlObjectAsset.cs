using System;
using UnityEngine.Bindings;

namespace UnityEngine.UIElements;

[Serializable]
[VisibleToOtherModules(new string[] { "UnityEditor.UIBuilderModule" })]
internal class UxmlObjectAsset : UxmlAsset
{
	[SerializeField]
	private bool m_IsField;

	public bool isField => m_IsField;

	public UxmlObjectAsset(string fullTypeNameOrFieldName, bool isField, UxmlNamespaceDefinition xmlNamespace = default(UxmlNamespaceDefinition))
		: base(fullTypeNameOrFieldName, xmlNamespace)
	{
		m_IsField = isField;
	}

	public override string ToString()
	{
		return isField ? $"Reference: {base.fullTypeName} (id:{base.id} parent:{base.parentId})" : base.ToString();
	}
}
