using System;

namespace UnityEngine.UIElements;

public struct StyleTextAutoSize : IStyleValue<TextAutoSize>, IEquatable<StyleTextAutoSize>
{
	private StyleKeyword m_Keyword;

	private TextAutoSize m_Value;

	public TextAutoSize value
	{
		get
		{
			return (m_Keyword == StyleKeyword.Undefined) ? m_Value : default(TextAutoSize);
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
		}
	}

	public StyleTextAutoSize(TextAutoSize v)
		: this(v, StyleKeyword.Undefined)
	{
	}

	public StyleTextAutoSize(StyleKeyword keyword)
		: this(default(TextAutoSize), keyword)
	{
	}

	internal StyleTextAutoSize(TextAutoSize v, StyleKeyword keyword)
	{
		m_Value = v;
		m_Keyword = keyword;
	}

	public static bool operator ==(StyleTextAutoSize lhs, StyleTextAutoSize rhs)
	{
		return lhs.m_Keyword == rhs.m_Keyword && lhs.m_Value.Equals(rhs.m_Value);
	}

	public static bool operator !=(StyleTextAutoSize lhs, StyleTextAutoSize rhs)
	{
		return !(lhs == rhs);
	}

	public static implicit operator StyleTextAutoSize(StyleKeyword keyword)
	{
		return new StyleTextAutoSize(keyword);
	}

	public static implicit operator StyleTextAutoSize(TextAutoSize v)
	{
		return new StyleTextAutoSize(v);
	}

	public bool Equals(StyleTextAutoSize other)
	{
		return this == other;
	}

	public override bool Equals(object obj)
	{
		return obj is StyleTextAutoSize other && Equals(other);
	}

	public override int GetHashCode()
	{
		int num = 917506989;
		num = num * -1521134295 + m_Keyword.GetHashCode();
		return num * -1521134295 + m_Value.GetHashCode();
	}

	public override string ToString()
	{
		return this.DebugString();
	}
}
