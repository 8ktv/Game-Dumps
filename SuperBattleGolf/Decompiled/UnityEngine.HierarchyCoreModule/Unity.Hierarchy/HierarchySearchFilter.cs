using System;
using Unity.Scripting.LifecycleManagement;
using UnityEngine.Bindings;
using UnityEngine.Scripting;

namespace Unity.Hierarchy;

[Serializable]
[NativeHeader("Modules/HierarchyCore/Public/HierarchySearch.h")]
[RequiredByNativeCode]
public struct HierarchySearchFilter
{
	private static readonly char[] s_WhiteSpaces = new char[3] { ' ', '\t', '\n' };

	[NoAutoStaticsCleanup]
	private static readonly HierarchySearchFilter s_Invalid;

	public static ref readonly HierarchySearchFilter Invalid => ref s_Invalid;

	public bool IsValid => !string.IsNullOrEmpty(Name);

	public string Name { get; set; }

	public string Value { get; set; }

	public float NumValue { get; set; }

	public HierarchySearchFilterOperator Op { get; set; }

	public static string ToString(HierarchySearchFilterOperator op)
	{
		return op switch
		{
			HierarchySearchFilterOperator.Equal => "=", 
			HierarchySearchFilterOperator.Contains => ":", 
			HierarchySearchFilterOperator.Greater => ">", 
			HierarchySearchFilterOperator.GreaterOrEqual => ">=", 
			HierarchySearchFilterOperator.Lesser => "<", 
			HierarchySearchFilterOperator.LesserOrEqual => "<=", 
			HierarchySearchFilterOperator.NotEqual => "!=", 
			HierarchySearchFilterOperator.Not => "-", 
			_ => throw new NotImplementedException($"Cannot convert {op} to string"), 
		};
	}

	public static HierarchySearchFilterOperator ToOp(string op)
	{
		return op switch
		{
			"<" => HierarchySearchFilterOperator.Lesser, 
			"<=" => HierarchySearchFilterOperator.LesserOrEqual, 
			">" => HierarchySearchFilterOperator.Greater, 
			">=" => HierarchySearchFilterOperator.GreaterOrEqual, 
			"=" => HierarchySearchFilterOperator.Equal, 
			":" => HierarchySearchFilterOperator.Contains, 
			"!=" => HierarchySearchFilterOperator.NotEqual, 
			"-" => HierarchySearchFilterOperator.Not, 
			_ => throw new NotImplementedException("Cannot convert " + op + " to SearchFilterOperator"), 
		};
	}

	public override string ToString()
	{
		string s = (float.IsNaN(NumValue) ? Value : NumValue.ToString());
		return Name + ToString(Op) + QuoteStringIfNeeded(s);
	}

	[VisibleToOtherModules(new string[] { "UnityEditor.HierarchyModule" })]
	internal static HierarchySearchFilter CreateFilter(string name, string op, string value)
	{
		return CreateFilter(name, ToOp(op), value);
	}

	internal static HierarchySearchFilter CreateFilter(string name, HierarchySearchFilterOperator op, string str)
	{
		string value = str;
		float numValue = float.NaN;
		try
		{
			numValue = Convert.ToSingle(str);
			value = null;
		}
		catch (Exception)
		{
		}
		return new HierarchySearchFilter
		{
			Name = name,
			Op = op,
			Value = value,
			NumValue = numValue
		};
	}

	internal static string QuoteStringIfNeeded(string s)
	{
		if (s.Length > 0 && s.IndexOfAny(s_WhiteSpaces) != -1 && s[0] != '"')
		{
			return "\"" + s + "\"";
		}
		return s;
	}
}
