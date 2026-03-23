using System;
using System.Collections.Generic;
using UnityEngine.Bindings;

namespace UnityEngine.UIElements;

[Serializable]
[VisibleToOtherModules(new string[] { "UnityEditor.UIBuilderModule" })]
internal class UxmlAsset : IUxmlAttributes
{
	public const string NullNodeType = "null";

	[SerializeField]
	private string m_FullTypeName;

	[SerializeField]
	private UxmlNamespaceDefinition m_XmlNamespace;

	[SerializeField]
	private int m_Id;

	[SerializeField]
	private int m_OrderInDocument;

	[SerializeField]
	private int m_ParentId;

	[SerializeField]
	private List<UxmlNamespaceDefinition> m_NamespaceDefinitions;

	[SerializeField]
	protected List<string> m_Properties;

	public string fullTypeName
	{
		get
		{
			return m_FullTypeName;
		}
		set
		{
			m_FullTypeName = value;
		}
	}

	public UxmlNamespaceDefinition xmlNamespace
	{
		get
		{
			return m_XmlNamespace;
		}
		set
		{
			m_XmlNamespace = value;
		}
	}

	public int id
	{
		get
		{
			return m_Id;
		}
		set
		{
			m_Id = value;
		}
	}

	public bool isNull => fullTypeName == "null";

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

	public List<UxmlNamespaceDefinition> namespaceDefinitions => m_NamespaceDefinitions ?? (m_NamespaceDefinitions = new List<UxmlNamespaceDefinition>());

	public UxmlAsset(string fullTypeName, UxmlNamespaceDefinition xmlNamespace = default(UxmlNamespaceDefinition))
	{
		m_FullTypeName = fullTypeName;
		m_XmlNamespace = xmlNamespace;
	}

	public List<string> GetProperties()
	{
		return m_Properties;
	}

	public bool HasParent()
	{
		return m_ParentId != 0;
	}

	public bool HasAttribute(string attributeName)
	{
		if (m_Properties == null || m_Properties.Count <= 0)
		{
			return false;
		}
		for (int i = 0; i < m_Properties.Count; i += 2)
		{
			string text = m_Properties[i];
			if (text == attributeName)
			{
				return true;
			}
		}
		return false;
	}

	public string GetAttributeValue(string attributeName)
	{
		TryGetAttributeValue(attributeName, out var value);
		return value;
	}

	public bool TryGetAttributeValue(string propertyName, out string value)
	{
		if (m_Properties == null)
		{
			value = null;
			return false;
		}
		for (int i = 0; i < m_Properties.Count - 1; i += 2)
		{
			if (m_Properties[i] == propertyName)
			{
				value = m_Properties[i + 1];
				return true;
			}
		}
		value = null;
		return false;
	}

	public void AddUxmlNamespace(string prefix, string resolvedNamespace)
	{
		namespaceDefinitions.Add(new UxmlNamespaceDefinition
		{
			prefix = prefix,
			resolvedNamespace = resolvedNamespace
		});
	}

	public void SetAttribute(string name, string value)
	{
		SetOrAddProperty(name, value);
	}

	public void RemoveAttribute(string attributeName)
	{
		if (m_Properties == null || m_Properties.Count <= 0)
		{
			return;
		}
		for (int i = 0; i < m_Properties.Count; i += 2)
		{
			string text = m_Properties[i];
			if (!(text != attributeName))
			{
				m_Properties.RemoveAt(i);
				m_Properties.RemoveAt(i);
				break;
			}
		}
	}

	private void SetOrAddProperty(string propertyName, string propertyValue)
	{
		if (m_Properties == null)
		{
			m_Properties = new List<string>();
		}
		for (int i = 0; i < m_Properties.Count - 1; i += 2)
		{
			if (m_Properties[i] == propertyName)
			{
				m_Properties[i + 1] = propertyValue;
				return;
			}
		}
		m_Properties.Add(propertyName);
		m_Properties.Add(propertyValue);
	}

	public override string ToString()
	{
		return $"{fullTypeName}(id:{id})";
	}
}
