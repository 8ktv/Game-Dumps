using System;
using System.Collections.Generic;
using Unity.Properties;

namespace UnityEngine.UIElements.Internal;

internal class AutoCompletePathVisitor : ITypeVisitor, IPropertyVisitor, IPropertyBagVisitor, IListPropertyVisitor
{
	private class VisitContext
	{
		public List<PropertyPathInfo> propertyPathInfos { get; set; }

		public HashSet<Type> types { get; } = new HashSet<Type>();

		public PropertyPath current { get; set; }

		public int currentDepth { get; set; }
	}

	private struct InspectedTypeScope<TContainer> : IDisposable
	{
		private VisitContext m_VisitContext;

		public InspectedTypeScope(VisitContext context)
		{
			m_VisitContext = context;
			m_VisitContext.types.Add(typeof(TContainer));
		}

		public void Dispose()
		{
			m_VisitContext.types.Remove(typeof(TContainer));
		}
	}

	private struct VisitedPropertyScope : IDisposable
	{
		private VisitContext m_VisitContext;

		public VisitedPropertyScope(VisitContext context, IProperty property)
		{
			m_VisitContext = context;
			m_VisitContext.current = PropertyPath.AppendProperty(m_VisitContext.current, property);
			PropertyPathInfo item = new PropertyPathInfo(m_VisitContext.current, property.DeclaredValueType());
			m_VisitContext.propertyPathInfos?.Add(item);
			m_VisitContext.currentDepth++;
		}

		public VisitedPropertyScope(VisitContext context, int index, Type type)
		{
			m_VisitContext = context;
			m_VisitContext.current = PropertyPath.AppendIndex(m_VisitContext.current, index);
			PropertyPathInfo item = new PropertyPathInfo(m_VisitContext.current, type);
			m_VisitContext.propertyPathInfos?.Add(item);
			m_VisitContext.currentDepth++;
		}

		public void Dispose()
		{
			m_VisitContext.current = PropertyPath.Pop(m_VisitContext.current);
			m_VisitContext.currentDepth--;
		}
	}

	private VisitContext m_VisitContext = new VisitContext();

	public List<PropertyPathInfo> propertyPathList
	{
		set
		{
			m_VisitContext.propertyPathInfos = value;
		}
	}

	public int maxDepth { get; set; }

	private bool HasReachedEnd(Type containerType)
	{
		return m_VisitContext.currentDepth >= maxDepth || m_VisitContext.types.Contains(containerType);
	}

	public void Reset()
	{
		m_VisitContext.current = default(PropertyPath);
		m_VisitContext.propertyPathInfos = null;
		m_VisitContext.types.Clear();
		m_VisitContext.currentDepth = 0;
	}

	void ITypeVisitor.Visit<TContainer>()
	{
		if (HasReachedEnd(typeof(TContainer)))
		{
			return;
		}
		using (new InspectedTypeScope<TContainer>(m_VisitContext))
		{
			IPropertyBag<TContainer> propertyBag = PropertyBag.GetPropertyBag<TContainer>();
			if (propertyBag == null)
			{
				return;
			}
			foreach (IProperty<TContainer> property in propertyBag.GetProperties())
			{
				using (new VisitedPropertyScope(m_VisitContext, property))
				{
					VisitPropertyType(property.DeclaredValueType());
				}
			}
		}
	}

	void IPropertyBagVisitor.Visit<TContainer>(IPropertyBag<TContainer> properties, ref TContainer container)
	{
		if (HasReachedEnd(typeof(TContainer)))
		{
			return;
		}
		using (new InspectedTypeScope<TContainer>(m_VisitContext))
		{
			if (!(properties is IIndexedProperties<TContainer> indexedProperties))
			{
				if (properties is IKeyedProperties<TContainer, object>)
				{
					return;
				}
				{
					foreach (IProperty<TContainer> property2 in properties.GetProperties(ref container))
					{
						using (new VisitedPropertyScope(m_VisitContext, property2))
						{
							property2.Accept(this, ref container);
						}
					}
					return;
				}
			}
			if (indexedProperties.TryGetProperty(ref container, 0, out var property))
			{
				using (new VisitedPropertyScope(m_VisitContext, 0, property.DeclaredValueType()))
				{
					property.Accept(this, ref container);
					return;
				}
			}
			VisitPropertyType(typeof(TContainer));
		}
	}

	void IPropertyVisitor.Visit<TContainer, TValue>(Property<TContainer, TValue> property, ref TContainer container)
	{
		if (!TypeTraits.IsContainer(typeof(TValue)))
		{
			return;
		}
		TValue value = property.GetValue(ref container);
		if ((!TypeTraits<TValue>.CanBeNull || !EqualityComparer<TValue>.Default.Equals(value, default(TValue))) && PropertyBag.TryGetPropertyBagForValue(ref value, out var propertyBag))
		{
			IPropertyBag propertyBag2 = propertyBag;
			IPropertyBag propertyBag3 = propertyBag2;
			if (propertyBag3 is IListPropertyAccept<TValue> listPropertyAccept)
			{
				listPropertyAccept.Accept(this, property, ref container, ref value);
			}
			else
			{
				PropertyContainer.TryAccept(this, ref value);
			}
		}
		else
		{
			VisitPropertyType(property.DeclaredValueType());
		}
	}

	void IListPropertyVisitor.Visit<TContainer, TList, TElement>(Property<TContainer, TList> property, ref TContainer container, ref TList list)
	{
		PropertyContainer.TryAccept(this, ref list);
	}

	private void VisitPropertyType(Type type)
	{
		if (HasReachedEnd(type))
		{
			return;
		}
		if (type.IsArray)
		{
			if (type.GetArrayRank() == 1)
			{
				Type elementType = type.GetElementType();
				IPropertyBag propertyBag = PropertyBag.GetPropertyBag(elementType);
				using (new VisitedPropertyScope(m_VisitContext, 0, elementType))
				{
					propertyBag?.Accept(this);
				}
			}
		}
		else if (type.IsGenericType)
		{
			if (type.GetGenericTypeDefinition().IsAssignableFrom(typeof(List<>)) || type.GetGenericTypeDefinition().IsAssignableFrom(typeof(IList<>)))
			{
				Type type2 = type.GenericTypeArguments[0];
				IPropertyBag propertyBag2 = PropertyBag.GetPropertyBag(type2);
				using (new VisitedPropertyScope(m_VisitContext, 0, type2))
				{
					propertyBag2?.Accept(this);
				}
			}
		}
		else
		{
			PropertyBag.GetPropertyBag(type)?.Accept(this);
		}
	}
}
