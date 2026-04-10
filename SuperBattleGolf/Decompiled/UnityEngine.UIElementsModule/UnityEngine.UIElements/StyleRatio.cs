using System;
using System.Globalization;

namespace UnityEngine.UIElements;

public struct StyleRatio : IStyleValue<Ratio>, IEquatable<StyleRatio>
{
	private Ratio m_Value;

	private StyleKeyword m_Keyword;

	public Ratio value
	{
		get
		{
			return (m_Keyword == StyleKeyword.Undefined) ? m_Value : ((Ratio)float.NaN);
		}
		set
		{
			m_Value = value;
			m_Keyword = StyleKeyword.Undefined;
		}
	}

	public StyleKeyword keyword
	{
		get
		{
			return m_Keyword;
		}
		set
		{
			m_Keyword = value;
			m_Value = float.NaN;
		}
	}

	public StyleRatio(Ratio value)
		: this(value, StyleKeyword.Undefined)
	{
	}

	public StyleRatio(StyleKeyword keyword)
		: this(float.NaN, keyword)
	{
	}

	internal StyleRatio(Ratio value, StyleKeyword keyword)
	{
		m_Keyword = keyword;
		m_Value = value;
	}

	public static StyleRatio Auto()
	{
		return new StyleRatio(float.NaN, StyleKeyword.Auto);
	}

	internal bool IsAuto()
	{
		return m_Keyword == StyleKeyword.Auto;
	}

	public static implicit operator StyleRatio(float value)
	{
		return new StyleRatio(value);
	}

	public static implicit operator float(StyleRatio value)
	{
		return value.value;
	}

	public static implicit operator StyleRatio(Ratio value)
	{
		return new StyleRatio(value);
	}

	public static implicit operator Ratio(StyleRatio value)
	{
		return value.value;
	}

	public static implicit operator StyleKeyword(StyleRatio value)
	{
		return value.keyword;
	}

	public static implicit operator StyleRatio(StyleKeyword value)
	{
		return new StyleRatio(value);
	}

	public static bool operator ==(StyleRatio lhs, StyleRatio rhs)
	{
		return lhs.m_Keyword == rhs.m_Keyword && lhs.m_Value == rhs.m_Value;
	}

	public static bool operator !=(StyleRatio lhs, StyleRatio rhs)
	{
		return !(lhs == rhs);
	}

	public bool Equals(StyleRatio other)
	{
		return other == this;
	}

	public override bool Equals(object obj)
	{
		return obj is StyleRatio other && Equals(other);
	}

	public override int GetHashCode()
	{
		return m_Value.GetHashCode() * 793;
	}

	public override string ToString()
	{
		return IsAuto() ? StyleValueKeyword.Auto.ToUssString() : m_Value.value.ToString(CultureInfo.InvariantCulture.NumberFormat);
	}
}
