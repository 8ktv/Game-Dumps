using System;
using Unity.Collections.LowLevel.Unsafe;
using UnityEngine.Internal;

namespace Unity.Hierarchy;

public readonly struct HierarchyPropertyUnmanaged<T> : IEquatable<HierarchyPropertyUnmanaged<T>>, IHierarchyProperty<T> where T : unmanaged
{
	private readonly Hierarchy m_Hierarchy;

	internal readonly HierarchyPropertyId m_Property;

	public bool IsCreated => m_Property != HierarchyPropertyId.Null && (m_Hierarchy?.IsCreated ?? false);

	internal HierarchyPropertyUnmanaged(Hierarchy hierarchy, in HierarchyPropertyId property)
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

	public unsafe void SetValue(in HierarchyNode node, T value)
	{
		if (m_Hierarchy == null)
		{
			throw new NullReferenceException("Hierarchy reference has not been set.");
		}
		if (!m_Hierarchy.IsCreated)
		{
			throw new InvalidOperationException("Hierarchy has been disposed.");
		}
		m_Hierarchy.SetPropertyRaw(in m_Property, in node, &value, sizeof(T));
	}

	public unsafe T GetValue(in HierarchyNode node)
	{
		if (m_Hierarchy == null)
		{
			throw new NullReferenceException("Hierarchy reference has not been set.");
		}
		if (!m_Hierarchy.IsCreated)
		{
			throw new InvalidOperationException("Hierarchy has been disposed.");
		}
		int size;
		void* propertyRaw = m_Hierarchy.GetPropertyRaw(in m_Property, in node, out size);
		if (propertyRaw == null || size != sizeof(T))
		{
			return default(T);
		}
		return UnsafeUtility.AsRef<T>(propertyRaw);
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
	public static bool operator ==(in HierarchyPropertyUnmanaged<T> lhs, in HierarchyPropertyUnmanaged<T> rhs)
	{
		return lhs.m_Hierarchy == rhs.m_Hierarchy && lhs.m_Property == rhs.m_Property;
	}

	[ExcludeFromDocs]
	public static bool operator !=(in HierarchyPropertyUnmanaged<T> lhs, in HierarchyPropertyUnmanaged<T> rhs)
	{
		return !(lhs == rhs);
	}

	[ExcludeFromDocs]
	public bool Equals(HierarchyPropertyUnmanaged<T> other)
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
		return obj is HierarchyPropertyUnmanaged<T> other && Equals(other);
	}

	[ExcludeFromDocs]
	public override int GetHashCode()
	{
		return m_Property.GetHashCode();
	}

	T IHierarchyProperty<T>.GetValue(in HierarchyNode node)
	{
		return GetValue(in node);
	}

	void IHierarchyProperty<T>.SetValue(in HierarchyNode node, T value)
	{
		SetValue(in node, value);
	}

	void IHierarchyProperty<T>.ClearValue(in HierarchyNode node)
	{
		ClearValue(in node);
	}
}
