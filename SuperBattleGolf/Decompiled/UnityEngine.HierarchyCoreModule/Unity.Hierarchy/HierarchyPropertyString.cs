using System;
using UnityEngine.Internal;

namespace Unity.Hierarchy;

public readonly struct HierarchyPropertyString : IEquatable<HierarchyPropertyString>, IHierarchyProperty<string>
{
	private readonly Hierarchy m_Hierarchy;

	internal readonly HierarchyPropertyId m_Property;

	public bool IsCreated => m_Property != HierarchyPropertyId.Null && (m_Hierarchy?.IsCreated ?? false);

	internal HierarchyPropertyString(Hierarchy hierarchy, in HierarchyPropertyId property)
	{
		if (hierarchy == null)
		{
			throw new ArgumentNullException("hierarchy");
		}
		if (property == HierarchyPropertyId.Null)
		{
			throw new ArgumentException("property");
		}
		m_Hierarchy = hierarchy;
		m_Property = property;
	}

	public string GetValue(in HierarchyNode node)
	{
		if (m_Hierarchy == null)
		{
			throw new NullReferenceException("Hierarchy reference has not been set.");
		}
		if (!m_Hierarchy.IsCreated)
		{
			throw new InvalidOperationException("Hierarchy has been disposed.");
		}
		return m_Hierarchy.GetPropertyString(in m_Property, in node);
	}

	public void SetValue(in HierarchyNode node, string value)
	{
		if (m_Hierarchy == null)
		{
			throw new NullReferenceException("Hierarchy reference has not been set.");
		}
		if (!m_Hierarchy.IsCreated)
		{
			throw new InvalidOperationException("Hierarchy has been disposed.");
		}
		m_Hierarchy.SetPropertyString(in m_Property, in node, value);
	}

	public void ClearValue(in HierarchyNode node)
	{
		if (m_Hierarchy == null)
		{
			throw new NullReferenceException("Hierarchy reference has not been set.");
		}
		if (!m_Hierarchy.IsCreated)
		{
			throw new InvalidOperationException("Hierarchy has been disposed.");
		}
		m_Hierarchy.ClearProperty(in m_Property, in node);
	}

	[ExcludeFromDocs]
	public static bool operator ==(in HierarchyPropertyString lhs, in HierarchyPropertyString rhs)
	{
		return lhs.m_Hierarchy == rhs.m_Hierarchy && lhs.m_Property == rhs.m_Property;
	}

	[ExcludeFromDocs]
	public static bool operator !=(in HierarchyPropertyString lhs, in HierarchyPropertyString rhs)
	{
		return !(lhs == rhs);
	}

	[ExcludeFromDocs]
	public bool Equals(HierarchyPropertyString other)
	{
		return m_Hierarchy == other.m_Hierarchy && m_Property == other.m_Property;
	}

	[ExcludeFromDocs]
	public override string ToString()
	{
		return m_Property.ToString();
	}

	[ExcludeFromDocs]
	public override bool Equals(object obj)
	{
		return obj is HierarchyPropertyString other && Equals(other);
	}

	[ExcludeFromDocs]
	public override int GetHashCode()
	{
		return m_Property.GetHashCode();
	}

	string IHierarchyProperty<string>.GetValue(in HierarchyNode node)
	{
		return GetValue(in node);
	}

	void IHierarchyProperty<string>.SetValue(in HierarchyNode node, string value)
	{
		SetValue(in node, value);
	}

	void IHierarchyProperty<string>.ClearValue(in HierarchyNode node)
	{
		ClearValue(in node);
	}
}
