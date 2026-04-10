using System;
using UnityEngine.Bindings;

namespace UnityEngine.UIElements;

[Serializable]
[VisibleToOtherModules(new string[] { "UnityEditor.UIBuilderModule" })]
internal struct StyleValueHandle : IEquatable<StyleValueHandle>
{
	[SerializeField]
	private StyleValueType m_ValueType;

	[VisibleToOtherModules(new string[] { "UnityEditor.UIBuilderModule" })]
	[SerializeField]
	internal int valueIndex;

	public StyleValueType valueType
	{
		get
		{
			return m_ValueType;
		}
		[VisibleToOtherModules(new string[] { "UnityEditor.UIBuilderModule" })]
		internal set
		{
			m_ValueType = value;
		}
	}

	[VisibleToOtherModules(new string[] { "UnityEditor.UIBuilderModule" })]
	internal StyleValueHandle(int valueIndex, StyleValueType valueType)
	{
		this.valueIndex = valueIndex;
		m_ValueType = valueType;
	}

	public bool IsVarFunction()
	{
		return valueType == StyleValueType.Function && valueIndex == 1;
	}

	public bool Equals(StyleValueHandle other)
	{
		return m_ValueType == other.m_ValueType && valueIndex == other.valueIndex;
	}

	public static bool operator ==(StyleValueHandle lhs, StyleValueHandle rhs)
	{
		return lhs.Equals(rhs);
	}

	public static bool operator !=(StyleValueHandle lhs, StyleValueHandle rhs)
	{
		return !(lhs == rhs);
	}

	public override bool Equals(object obj)
	{
		return obj is StyleValueHandle other && Equals(other);
	}

	public override int GetHashCode()
	{
		return HashCode.Combine((int)m_ValueType, valueIndex);
	}
}
