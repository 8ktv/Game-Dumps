using System;
using System.Collections.Generic;
using Unity.Properties;

namespace UnityEngine.UIElements.Internal;

internal class TypePathVisitor : ITypeVisitor, IPropertyBagVisitor, IPropertyVisitor
{
	private Type m_LastType;

	private int m_PathIndex;

	public PropertyPath Path { get; set; }

	public Type resolvedType { get; private set; }

	public VisitReturnCode ReturnCode { get; internal set; }

	public int PathIndex => m_PathIndex;

	public void Reset()
	{
		resolvedType = null;
		m_LastType = null;
		Path = default(PropertyPath);
		ReturnCode = VisitReturnCode.Ok;
		m_PathIndex = 0;
	}

	void IPropertyBagVisitor.Visit<TContainer>(IPropertyBag<TContainer> properties, ref TContainer container)
	{
		PropertyPathPart propertyPathPart = Path[m_PathIndex++];
		IProperty<TContainer> property;
		switch (propertyPathPart.Kind)
		{
		case PropertyPathPartKind.Name:
			if (properties is INamedProperties<TContainer> namedProperties && namedProperties.TryGetProperty(ref container, propertyPathPart.Name, out property))
			{
				property.Accept(this, ref container);
				break;
			}
			foreach (IProperty<TContainer> property2 in properties.GetProperties())
			{
				if (property2.Name == propertyPathPart.Name)
				{
					PropertyBag.GetPropertyBag(m_LastType = property2.DeclaredValueType())?.Accept(this);
					return;
				}
			}
			ReturnCode = VisitReturnCode.InvalidPath;
			break;
		case PropertyPathPartKind.Index:
		{
			if (properties is IIndexedProperties<TContainer> indexedProperties && indexedProperties.TryGetProperty(ref container, propertyPathPart.Index, out property))
			{
				property.Accept(this, ref container);
				break;
			}
			Type elementType = GetElementType(typeof(TContainer));
			if (elementType != null)
			{
				PropertyBag.GetPropertyBag(elementType)?.Accept(this);
			}
			else
			{
				ReturnCode = VisitReturnCode.InvalidPath;
			}
			break;
		}
		default:
			ReturnCode = VisitReturnCode.InvalidPath;
			break;
		}
	}

	void IPropertyVisitor.Visit<TContainer, TValue>(Property<TContainer, TValue> property, ref TContainer container)
	{
		TValue value = property.GetValue(ref container);
		IPropertyBag propertyBag;
		if (m_PathIndex >= Path.Length)
		{
			resolvedType = property.DeclaredValueType();
		}
		else if (PropertyBag.TryGetPropertyBagForValue(ref value, out propertyBag))
		{
			if (TypeTraits<TValue>.CanBeNull && EqualityComparer<TValue>.Default.Equals(value, default(TValue)))
			{
				PropertyBag.GetPropertyBag(property.DeclaredValueType())?.Accept(this);
			}
			else
			{
				PropertyContainer.Accept(this, ref value);
			}
		}
		else
		{
			ReturnCode = VisitReturnCode.InvalidPath;
		}
	}

	void ITypeVisitor.Visit<TContainer>()
	{
		if (IsLastPartReached())
		{
			return;
		}
		PropertyPathPart propertyPathPart = Path[m_PathIndex++];
		m_LastType = null;
		switch (propertyPathPart.Kind)
		{
		case PropertyPathPartKind.Name:
		{
			IPropertyBag<TContainer> propertyBag2 = PropertyBag.GetPropertyBag<TContainer>();
			if (propertyBag2 == null)
			{
				return;
			}
			foreach (IProperty<TContainer> property in propertyBag2.GetProperties())
			{
				if (property.Name != propertyPathPart.Name)
				{
					continue;
				}
				Type type = (m_LastType = property.DeclaredValueType());
				IPropertyBag propertyBag3 = PropertyBag.GetPropertyBag(type);
				if (propertyBag3 != null)
				{
					propertyBag3.Accept(this);
					return;
				}
				Type elementType2 = GetElementType(type);
				if (!(elementType2 != null))
				{
					break;
				}
				if (!IsLastPartReached())
				{
					if (!Path[m_PathIndex++].IsIndex)
					{
						break;
					}
					m_LastType = elementType2;
					PropertyBag.GetPropertyBag(elementType2)?.Accept(this);
				}
				return;
			}
			break;
		}
		case PropertyPathPartKind.Index:
		{
			Type typeFromHandle = typeof(TContainer);
			Type elementType = GetElementType(typeFromHandle);
			if (elementType != null)
			{
				m_LastType = elementType;
				IPropertyBag propertyBag = PropertyBag.GetPropertyBag(elementType);
				if (propertyBag != null)
				{
					propertyBag.Accept(this);
					return;
				}
			}
			break;
		}
		}
		if (!IsLastPartReached() && ReturnCode == VisitReturnCode.Ok)
		{
			ReturnCode = VisitReturnCode.InvalidPath;
		}
	}

	private bool IsLastPartReached()
	{
		if (m_PathIndex < Path.Length)
		{
			return false;
		}
		if (m_LastType == null)
		{
			ReturnCode = VisitReturnCode.InvalidPath;
		}
		resolvedType = m_LastType;
		return true;
	}

	private static Type GetElementType(Type type)
	{
		Type result = null;
		if (type.IsArray && type.GetArrayRank() == 1)
		{
			result = type.GetElementType();
		}
		else if (type.IsGenericType && (type.GetGenericTypeDefinition().IsAssignableFrom(typeof(List<>)) || type.GetGenericTypeDefinition().IsAssignableFrom(typeof(IList<>))))
		{
			result = type.GenericTypeArguments[0];
		}
		return result;
	}
}
