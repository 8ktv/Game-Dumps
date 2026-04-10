using System;
using UnityEngine.Bindings;

namespace UnityEngine.UIElements;

[Serializable]
[VisibleToOtherModules(new string[] { "UnityEditor.UIBuilderModule" })]
internal struct StyleSelectorPart
{
	[SerializeField]
	private string m_Value;

	[SerializeField]
	private StyleSelectorType m_Type;

	internal object tempData;

	public string value
	{
		get
		{
			return m_Value;
		}
		[VisibleToOtherModules(new string[] { "UnityEditor.UIBuilderModule" })]
		internal set
		{
			m_Value = value;
		}
	}

	public StyleSelectorType type
	{
		get
		{
			return m_Type;
		}
		[VisibleToOtherModules(new string[] { "UnityEditor.UIBuilderModule" })]
		internal set
		{
			m_Type = value;
		}
	}

	public override string ToString()
	{
		return $"[StyleSelectorPart: value={value}, type={type}]";
	}

	public static StyleSelectorPart CreateClass(string className)
	{
		return new StyleSelectorPart
		{
			m_Type = StyleSelectorType.Class,
			m_Value = className
		};
	}

	public static StyleSelectorPart CreatePseudoClass(string className)
	{
		return new StyleSelectorPart
		{
			m_Type = StyleSelectorType.PseudoClass,
			m_Value = className
		};
	}

	public static StyleSelectorPart CreateId(string Id)
	{
		return new StyleSelectorPart
		{
			m_Type = StyleSelectorType.ID,
			m_Value = Id
		};
	}

	public static StyleSelectorPart CreateType(Type t)
	{
		return new StyleSelectorPart
		{
			m_Type = StyleSelectorType.Type,
			m_Value = t.Name
		};
	}

	public static StyleSelectorPart CreateType(string typeName)
	{
		return new StyleSelectorPart
		{
			m_Type = StyleSelectorType.Type,
			m_Value = typeName
		};
	}

	public static StyleSelectorPart CreatePredicate(object predicate)
	{
		return new StyleSelectorPart
		{
			m_Type = StyleSelectorType.Predicate,
			tempData = predicate
		};
	}

	public static StyleSelectorPart CreateWildCard()
	{
		return new StyleSelectorPart
		{
			m_Value = "*",
			m_Type = StyleSelectorType.Wildcard
		};
	}
}
