using System;
using System.Text;
using Unity.Properties;
using UnityEngine.UIElements.Layout;

namespace UnityEngine.UIElements;

[Serializable]
internal struct FilterFunction : IEquatable<FilterFunction>
{
	internal class PropertyBag : ContainerPropertyBag<FilterFunction>
	{
		private class ParametersProperty : Property<FilterFunction, FixedBuffer4<FilterParameter>>
		{
			public override string Name { get; } = "parameters";

			public override bool IsReadOnly { get; } = false;

			public override FixedBuffer4<FilterParameter> GetValue(ref FilterFunction container)
			{
				return container.parameters;
			}

			public override void SetValue(ref FilterFunction container, FixedBuffer4<FilterParameter> value)
			{
				container.parameters = value;
			}
		}

		private class FilterFunctionDefinitionProperty : Property<FilterFunction, FilterFunctionDefinition>
		{
			public override string Name { get; } = "customDefinition";

			public override bool IsReadOnly { get; } = false;

			public override FilterFunctionDefinition GetValue(ref FilterFunction container)
			{
				return container.customDefinition;
			}

			public override void SetValue(ref FilterFunction container, FilterFunctionDefinition value)
			{
				container.customDefinition = value;
			}
		}

		public PropertyBag()
		{
			AddProperty(new ParametersProperty());
			AddProperty(new FilterFunctionDefinitionProperty());
		}
	}

	[SerializeField]
	private FilterFunctionType m_Type;

	[SerializeField]
	private FixedBuffer4<FilterParameter> m_Parameters;

	[SerializeField]
	private int m_ParameterCount;

	[SerializeField]
	private FilterFunctionDefinition m_CustomDefinition;

	public FilterFunctionType type
	{
		get
		{
			return m_Type;
		}
		set
		{
			m_Type = value;
		}
	}

	internal FixedBuffer4<FilterParameter> parameters
	{
		get
		{
			return m_Parameters;
		}
		set
		{
			m_Parameters = value;
		}
	}

	public int parameterCount => m_ParameterCount;

	public FilterFunctionDefinition customDefinition
	{
		get
		{
			return m_CustomDefinition;
		}
		set
		{
			m_CustomDefinition = value;
		}
	}

	public void AddParameter(FilterParameter p)
	{
		int num = m_ParameterCount;
		if (num >= 4)
		{
			throw new ArgumentOutOfRangeException($"FilterFunction.AddParameter only support up to {4} parameters");
		}
		m_Parameters[num] = p;
		m_ParameterCount++;
	}

	public void SetParameter(int index, FilterParameter p)
	{
		if (index < 0 || index >= m_ParameterCount)
		{
			throw new ArgumentOutOfRangeException("FilterFunction.SetParameter index out of range");
		}
		m_Parameters[index] = p;
	}

	public FilterParameter GetParameter(int index)
	{
		if (index < 0 || index >= parameterCount)
		{
			throw new ArgumentOutOfRangeException("index");
		}
		return m_Parameters[index];
	}

	public void ClearParameters()
	{
		m_ParameterCount = 0;
	}

	public FilterFunction(FilterFunctionType type)
	{
		m_ParameterCount = 0;
		m_Type = type;
		m_CustomDefinition = null;
		m_Parameters = default(FixedBuffer4<FilterParameter>);
	}

	public FilterFunction(FilterFunctionDefinition filterDef)
	{
		m_ParameterCount = 0;
		if (filterDef == null)
		{
			throw new ArgumentNullException("filterDef");
		}
		m_Type = FilterFunctionType.Custom;
		m_CustomDefinition = filterDef;
		m_Parameters = default(FixedBuffer4<FilterParameter>);
	}

	internal FilterFunction(FilterFunctionType type, FixedBuffer4<FilterParameter> parameters, int paramCount)
	{
		m_ParameterCount = 0;
		m_Type = type;
		m_CustomDefinition = null;
		m_Parameters = parameters;
		m_ParameterCount = paramCount;
		FilterFunctionDefinition definition = GetDefinition();
		if (definition != null)
		{
			int num = GetDefinition().parameters.Length;
			if (num != paramCount)
			{
				Debug.LogError($"Invalid parameter count provided with filter of type {type}: provided {paramCount} but expected {num}");
			}
		}
	}

	internal FilterFunction(FilterFunctionDefinition customDefinition, FixedBuffer4<FilterParameter> parameters, int paramCount)
	{
		m_ParameterCount = 0;
		m_Type = FilterFunctionType.Custom;
		m_CustomDefinition = customDefinition;
		m_Parameters = parameters;
		m_ParameterCount = paramCount;
		if (customDefinition != null)
		{
			FilterParameterDeclaration[] array = customDefinition.parameters;
			int num = ((array != null) ? array.Length : 0);
			if (num != paramCount)
			{
				Debug.LogError($"Invalid parameter count provided with custom filter function definition {customDefinition}: provided {paramCount} but expected {num}");
			}
		}
	}

	internal FilterFunctionDefinition GetDefinition()
	{
		if (m_Type == FilterFunctionType.Custom)
		{
			return m_CustomDefinition;
		}
		return FilterFunctionDefinitionUtils.GetBuiltinDefinition(m_Type);
	}

	public static bool operator ==(FilterFunction lhs, FilterFunction rhs)
	{
		if (lhs.m_CustomDefinition != rhs.m_CustomDefinition)
		{
			return false;
		}
		for (int i = 0; i < 4; i++)
		{
			if (lhs.m_Parameters[i] != rhs.m_Parameters[i])
			{
				return false;
			}
		}
		return true;
	}

	public static bool operator !=(FilterFunction lhs, FilterFunction rhs)
	{
		return !(lhs == rhs);
	}

	public bool Equals(FilterFunction other)
	{
		return other == this;
	}

	public override bool Equals(object obj)
	{
		return obj is FilterFunction other && Equals(other);
	}

	public override int GetHashCode()
	{
		return (m_Parameters.GetHashCode() * 397) ^ ((m_CustomDefinition != null) ? m_CustomDefinition.GetHashCode() : 0);
	}

	public override string ToString()
	{
		StringBuilder stringBuilder = new StringBuilder();
		FilterFunctionDefinition definition = GetDefinition();
		if (!string.IsNullOrEmpty(definition?.filterName))
		{
			stringBuilder.Append(definition.filterName);
		}
		else if (type == FilterFunctionType.Custom)
		{
			stringBuilder.Append("custom");
		}
		else if (type == FilterFunctionType.None)
		{
			stringBuilder.Append("none");
		}
		stringBuilder.Append("(");
		for (int i = 0; i < parameterCount; i++)
		{
			if (i > 0)
			{
				stringBuilder.Append(" ");
			}
			stringBuilder.Append(parameters[i].ToString());
		}
		stringBuilder.Append(")");
		return stringBuilder.ToString();
	}
}
