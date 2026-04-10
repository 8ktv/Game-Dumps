using System;
using System.Collections.Generic;
using UnityEngine.Bindings;
using UnityEngine.Pool;

namespace UnityEngine.UIElements;

[Serializable]
[VisibleToOtherModules(new string[] { "UnityEditor.UIBuilderModule" })]
internal abstract class UxmlAsset : IUxmlAttributes
{
	public const string NullNodeType = "null";

	[SerializeField]
	private string m_FullTypeName;

	[SerializeField]
	private UxmlNamespaceDefinition m_XmlNamespace;

	[SerializeField]
	private int m_Id;

	[SerializeReference]
	[HideInInspector]
	private UxmlAsset m_Parent;

	[SerializeReference]
	private List<UxmlAsset> m_Children;

	[SerializeField]
	private VisualTreeAsset m_VisualTreeAsset;

	[SerializeField]
	private List<UxmlNamespaceDefinition> m_NamespaceDefinitions;

	[SerializeField]
	protected List<UxmlProperty> m_Properties;

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

	public bool isRoot => fullTypeName.Equals("UXML", StringComparison.Ordinal) || fullTypeName.Equals("UnityEngine.UIElements.UXML", StringComparison.Ordinal) || fullTypeName.EndsWith(".UXML", StringComparison.Ordinal);

	public UxmlAsset parentAsset => m_Parent;

	internal VisualTreeAsset visualTreeAsset => m_VisualTreeAsset;

	public int childCount => m_Children?.Count ?? 0;

	public UxmlAsset this[int index] => m_Children[index];

	public List<UxmlNamespaceDefinition> namespaceDefinitions => m_NamespaceDefinitions ?? (m_NamespaceDefinitions = new List<UxmlNamespaceDefinition>());

	public List<UxmlProperty> properties => m_Properties;

	public UxmlAsset(string fullTypeName, UxmlNamespaceDefinition xmlNamespace = default(UxmlNamespaceDefinition))
	{
		m_FullTypeName = fullTypeName;
		m_XmlNamespace = xmlNamespace;
	}

	public void GetChildren(List<UxmlAsset> children)
	{
		children.Clear();
		for (int i = 0; i < childCount; i++)
		{
			children.Add(this[i]);
		}
	}

	public void GetChildrenUxmlObjectAssets(List<UxmlObjectAsset> children)
	{
		children.Clear();
		for (int i = 0; i < childCount; i++)
		{
			if (this[i] is UxmlObjectAsset item)
			{
				children.Add(item);
			}
		}
	}

	public bool HasAnyUxmlObjectAsset()
	{
		for (int i = 0; i < childCount; i++)
		{
			if (this[i] is UxmlObjectAsset)
			{
				return true;
			}
		}
		return false;
	}

	public UxmlObjectAsset GetField(string fieldName)
	{
		for (int i = 0; i < childCount; i++)
		{
			if (this[i] is UxmlObjectAsset { isField: not false } uxmlObjectAsset && uxmlObjectAsset.fullTypeName == fieldName)
			{
				return uxmlObjectAsset;
			}
		}
		return null;
	}

	private void RemoveNonFields()
	{
		for (int num = childCount - 1; num >= 0; num--)
		{
			if (this[num] is UxmlObjectAsset { isField: false } uxmlObjectAsset)
			{
				uxmlObjectAsset.RemoveFromHierarchy();
			}
		}
	}

	public void RemoveUxmlObjectAssetChildren()
	{
		for (int num = childCount - 1; num >= 0; num--)
		{
			if (this[num] is UxmlObjectAsset uxmlObjectAsset)
			{
				uxmlObjectAsset.RemoveFromHierarchy();
			}
		}
	}

	[VisibleToOtherModules(new string[] { "UnityEditor.UIBuilderModule" })]
	internal void SetUxmlObjectAssets(string fieldName, List<UxmlObjectAsset> entries)
	{
		if (!string.IsNullOrEmpty(fieldName))
		{
			GetField(fieldName)?.SetUxmlObjectAssets(null, entries);
			return;
		}
		RemoveNonFields();
		foreach (UxmlObjectAsset entry in entries)
		{
			Add(entry);
		}
	}

	[VisibleToOtherModules(new string[] { "UnityEditor.UIBuilderModule" })]
	internal void CollectUxmlObjectAssets(string fieldName, List<UxmlObjectAsset> foundEntries)
	{
		for (int i = 0; i < childCount; i++)
		{
			if (this[i] is UxmlObjectAsset uxmlObjectAsset)
			{
				if (!string.IsNullOrEmpty(fieldName) && uxmlObjectAsset.isField && uxmlObjectAsset.fullTypeName == fieldName)
				{
					uxmlObjectAsset.CollectUxmlObjectAssets(null, foundEntries);
					break;
				}
				if (!uxmlObjectAsset.isField)
				{
					foundEntries.Add(uxmlObjectAsset);
				}
			}
		}
	}

	public virtual void GetExportTypename(out string typename, out UxmlNamespaceDefinition resolvedNamespace)
	{
		if (xmlNamespace != UxmlNamespaceDefinition.Empty)
		{
			UxmlNamespaceDefinition uxmlNamespaceDefinition = m_VisualTreeAsset.FindUxmlNamespaceDefinitionFromPrefix(this, xmlNamespace.prefix);
			if (uxmlNamespaceDefinition != xmlNamespace)
			{
				xmlNamespace = m_VisualTreeAsset.FindUxmlNamespaceDefinitionForTypeName(this, fullTypeName);
			}
		}
		if (string.IsNullOrEmpty(xmlNamespace.prefix))
		{
			if (string.IsNullOrEmpty(xmlNamespace.resolvedNamespace))
			{
				typename = fullTypeName;
				resolvedNamespace = xmlNamespace;
			}
			else
			{
				string text = fullTypeName.Substring(xmlNamespace.resolvedNamespace.Length + 1);
				typename = text;
				resolvedNamespace = xmlNamespace;
			}
		}
		else
		{
			string text2 = fullTypeName.Substring(xmlNamespace.resolvedNamespace.Length + 1);
			typename = text2;
			resolvedNamespace = xmlNamespace;
		}
	}

	internal void SetVisualTreeAssetWithOutNotify(VisualTreeAsset vta)
	{
		m_VisualTreeAsset = vta;
	}

	internal void SetVisualTreeAsset(VisualTreeAsset vta)
	{
		VisualTreeAsset visualTreeAsset = this.visualTreeAsset;
		SetVisualTreeAssetWithOutNotify(vta);
		if (visualTreeAsset != this.visualTreeAsset)
		{
			OnVisualTreeAssetChanged(visualTreeAsset, this.visualTreeAsset);
		}
		if (m_Children == null)
		{
			return;
		}
		foreach (UxmlAsset child in m_Children)
		{
			child.SetVisualTreeAsset(vta);
		}
	}

	public void Add(UxmlAsset asset)
	{
		if (asset == null)
		{
			throw new ArgumentNullException("asset");
		}
		Insert(childCount, asset);
	}

	public void Insert(int index, UxmlAsset asset)
	{
		if (asset == null)
		{
			throw new ArgumentNullException("asset");
		}
		if (index < 0 || index > childCount)
		{
			throw new ArgumentOutOfRangeException("Index out of range: " + index);
		}
		if (asset == this)
		{
			throw new ArgumentException("Cannot insert element as its own child.");
		}
		if (asset.IsAncestorOf(this))
		{
			throw new ArgumentException("Cannot insert element as a child because it is an ancestor.");
		}
		if (!Accepts(asset, out var errorMessage))
		{
			throw new InvalidOperationException(errorMessage);
		}
		if (asset.parentAsset == this)
		{
			int num = m_Children.IndexOf(asset);
			if (num != index)
			{
				bool flag = index == childCount;
				m_Children.RemoveAt(num);
				m_Children.Insert(flag ? childCount : index, asset);
			}
		}
		else
		{
			InsertInChildren(index, asset);
			asset.SetParent(this);
		}
	}

	public bool Remove(UxmlAsset asset)
	{
		if (asset == null)
		{
			throw new ArgumentNullException("asset");
		}
		if (asset == this)
		{
			throw new ArgumentException("Cannot remove element from itself.");
		}
		if (asset.m_Parent != this)
		{
			return false;
		}
		RemoveAt(m_Children.IndexOf(asset));
		return true;
	}

	public void RemoveAt(int index)
	{
		if (index < 0 || index > childCount)
		{
			throw new ArgumentOutOfRangeException("Index out of range: " + index);
		}
		UxmlAsset uxmlAsset = m_Children[index];
		uxmlAsset.SetParent(null);
	}

	private void InsertInChildren(int index, UxmlAsset asset)
	{
		if (m_Children == null)
		{
			m_Children = new List<UxmlAsset>();
		}
		m_Children.Insert(index, asset);
	}

	private void RemoveFromChildren(UxmlAsset child)
	{
		RemoveFromChildren(IndexOf(child));
	}

	private void RemoveFromChildren(int index)
	{
		m_Children.RemoveAt(index);
	}

	private void SetParent(UxmlAsset parent)
	{
		m_Parent?.RemoveFromChildren(this);
		m_Parent = parent;
		SetVisualTreeAsset(parent?.visualTreeAsset);
	}

	private protected virtual void OnVisualTreeAssetChanged(VisualTreeAsset previousVta, VisualTreeAsset newVta)
	{
	}

	public int IndexOf(UxmlAsset asset)
	{
		return m_Children.IndexOf(asset);
	}

	public int SiblingIndex()
	{
		return parentAsset?.IndexOf(this) ?? (-1);
	}

	public void RemoveFromHierarchy()
	{
		parentAsset?.Remove(this);
	}

	public bool IsAncestorOf(UxmlAsset other)
	{
		HashSet<UxmlAsset> value;
		using (CollectionPool<HashSet<UxmlAsset>, UxmlAsset>.Get(out value))
		{
			for (UxmlAsset uxmlAsset = other; uxmlAsset != null; uxmlAsset = uxmlAsset.parentAsset)
			{
				if (!value.Add(uxmlAsset))
				{
					throw new InvalidOperationException("Recursion Detected");
				}
				if (this == uxmlAsset.parentAsset)
				{
					return true;
				}
			}
			return false;
		}
	}

	public virtual bool HasParent()
	{
		return m_Parent != null;
	}

	public bool HasAttribute(string attributeName)
	{
		List<UxmlProperty> list = m_Properties;
		if (list == null || list.Count <= 0)
		{
			return false;
		}
		for (int i = 0; i < m_Properties.Count; i++)
		{
			if (string.CompareOrdinal(m_Properties[i].name, attributeName) == 0)
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
		for (int i = 0; i < m_Properties.Count; i++)
		{
			UxmlProperty uxmlProperty = m_Properties[i];
			if (string.CompareOrdinal(uxmlProperty.name, propertyName) == 0)
			{
				value = uxmlProperty.value;
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
		for (int i = 0; i < m_Properties.Count; i++)
		{
			if (string.CompareOrdinal(m_Properties[i].name, attributeName) == 0)
			{
				m_Properties.RemoveAt(i);
				break;
			}
		}
	}

	private void SetOrAddProperty(string propertyName, string propertyValue)
	{
		if (m_Properties == null)
		{
			m_Properties = new List<UxmlProperty>();
		}
		for (int i = 0; i < m_Properties.Count; i++)
		{
			UxmlProperty value = m_Properties[i];
			if (string.CompareOrdinal(value.name, propertyName) == 0)
			{
				value.value = propertyValue;
				m_Properties[i] = value;
				return;
			}
		}
		m_Properties.Add(new UxmlProperty
		{
			name = propertyName,
			value = propertyValue
		});
	}

	internal abstract bool Accepts(UxmlAsset asset, out string errorMessage);

	public override string ToString()
	{
		return $"{fullTypeName}(id:{id})";
	}
}
