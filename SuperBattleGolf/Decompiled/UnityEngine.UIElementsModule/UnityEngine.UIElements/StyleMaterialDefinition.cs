using System;

namespace UnityEngine.UIElements;

[Serializable]
public struct StyleMaterialDefinition : IStyleValue<MaterialDefinition>, IEquatable<StyleMaterialDefinition>
{
	[SerializeField]
	private MaterialDefinition m_Value;

	[SerializeField]
	private StyleKeyword m_Keyword;

	public MaterialDefinition value
	{
		get
		{
			return m_Value;
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

	public StyleMaterialDefinition(MaterialDefinition m)
		: this(m, StyleKeyword.Undefined)
	{
	}

	internal StyleMaterialDefinition(object obj, StyleKeyword keyword)
		: this(MaterialDefinition.FromObject(obj), keyword)
	{
	}

	public StyleMaterialDefinition(Material m)
		: this(m, StyleKeyword.Undefined)
	{
	}

	public StyleMaterialDefinition(StyleKeyword keyword)
		: this(null, keyword)
	{
	}

	internal StyleMaterialDefinition(MaterialDefinition m, StyleKeyword keyword)
	{
		m_Keyword = keyword;
		m_Value = m;
	}

	public static bool operator ==(StyleMaterialDefinition lhs, StyleMaterialDefinition rhs)
	{
		return lhs.m_Keyword == rhs.m_Keyword && lhs.m_Value == rhs.m_Value;
	}

	public static bool operator !=(StyleMaterialDefinition lhs, StyleMaterialDefinition rhs)
	{
		return !(lhs == rhs);
	}

	public static implicit operator StyleMaterialDefinition(StyleKeyword keyword)
	{
		return new StyleMaterialDefinition(keyword);
	}

	public static implicit operator StyleMaterialDefinition(MaterialDefinition m)
	{
		return new StyleMaterialDefinition(m);
	}

	public static implicit operator StyleMaterialDefinition(Material m)
	{
		return new StyleMaterialDefinition(m);
	}

	public bool Equals(StyleMaterialDefinition other)
	{
		return other == this;
	}

	public override bool Equals(object obj)
	{
		return obj is StyleMaterialDefinition other && Equals(other);
	}

	public override int GetHashCode()
	{
		return (((m_Value != null) ? m_Value.GetHashCode() : 0) * 397) ^ (int)m_Keyword;
	}

	public override string ToString()
	{
		return this.DebugString();
	}
}
