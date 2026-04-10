using System;
using System.Globalization;

namespace UnityEngine.UIElements;

[Serializable]
public struct FilterParameter : IEquatable<FilterParameter>
{
	[SerializeField]
	private FilterParameterType m_Type;

	[SerializeField]
	private float m_FloatValue;

	[SerializeField]
	private Color m_ColorValue;

	public FilterParameterType type
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

	public float floatValue
	{
		get
		{
			return m_FloatValue;
		}
		set
		{
			m_FloatValue = value;
		}
	}

	public Color colorValue
	{
		get
		{
			return m_ColorValue;
		}
		set
		{
			m_ColorValue = value;
		}
	}

	public FilterParameter(float value)
	{
		m_Type = FilterParameterType.Float;
		m_FloatValue = value;
		m_ColorValue = Color.clear;
	}

	public FilterParameter(Color value)
	{
		m_Type = FilterParameterType.Color;
		m_ColorValue = value;
		m_FloatValue = 0f;
	}

	public static bool operator ==(FilterParameter a, FilterParameter b)
	{
		if (a.type != b.type)
		{
			return false;
		}
		if (a.type == FilterParameterType.Float)
		{
			return a.floatValue == b.floatValue;
		}
		return a.colorValue == b.colorValue;
	}

	public static bool operator !=(FilterParameter a, FilterParameter b)
	{
		return !(a == b);
	}

	public override bool Equals(object obj)
	{
		return obj is FilterParameter filterParameter && this == filterParameter;
	}

	public bool Equals(FilterParameter other)
	{
		return this == other;
	}

	public override int GetHashCode()
	{
		return (type == FilterParameterType.Float) ? floatValue.GetHashCode() : colorValue.GetHashCode();
	}

	public override string ToString()
	{
		return (type == FilterParameterType.Float) ? floatValue.ToString(CultureInfo.InvariantCulture) : colorValue.ToString();
	}
}
