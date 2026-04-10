using System;
using UnityEngine.Bindings;

namespace UnityEngine.UIElements;

[Serializable]
[VisibleToOtherModules(new string[] { "UnityEditor.UIBuilderModule" })]
internal class UxmlObjectAsset : UxmlAsset
{
	[SerializeField]
	private int m_ParentId;

	[SerializeField]
	private int m_OrderInDocument;

	[SerializeField]
	private bool m_IsField;

	public int parentId
	{
		get
		{
			return m_ParentId;
		}
		set
		{
			m_ParentId = value;
		}
	}

	public int orderInDocument
	{
		get
		{
			return m_OrderInDocument;
		}
		set
		{
			m_OrderInDocument = value;
		}
	}

	public bool isField => m_IsField;

	public override bool HasParent()
	{
		return m_ParentId != 0;
	}

	public UxmlObjectAsset(string fullTypeNameOrFieldName, bool isField, UxmlNamespaceDefinition xmlNamespace = default(UxmlNamespaceDefinition))
		: base(fullTypeNameOrFieldName, xmlNamespace)
	{
		m_IsField = isField;
	}

	public override void GetExportTypename(out string typename, out UxmlNamespaceDefinition uxmlNamespaceDefinition)
	{
		if (isField)
		{
			typename = base.fullTypeName;
			uxmlNamespaceDefinition = UxmlNamespaceDefinition.Empty;
		}
		else
		{
			base.GetExportTypename(out typename, out uxmlNamespaceDefinition);
		}
	}

	internal override bool Accepts(UxmlAsset asset, out string errorMessage)
	{
		bool flag = asset is UxmlObjectAsset;
		errorMessage = ((!flag) ? ("[UI Toolkit] Cannot add a UXML asset of type '" + asset.fullTypeName + "' to a UXML asset of type '" + base.fullTypeName + "': UXML objects can only contain other UXML objects.") : null);
		return flag;
	}

	public override string ToString()
	{
		return isField ? $"Reference: {base.fullTypeName} (id:{base.id} parent:{parentId})" : base.ToString();
	}
}
