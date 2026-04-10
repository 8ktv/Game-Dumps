using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using Unity.Scripting.LifecycleManagement;
using UnityEngine.Bindings;
using UnityEngine.Scripting;

namespace Unity.Hierarchy;

[Serializable]
[StructLayout(LayoutKind.Sequential)]
[NativeHeader("Modules/HierarchyCore/Public/HierarchySearch.h")]
[NativeAsStruct]
[RequiredByNativeCode]
public sealed class HierarchySearchQueryDescriptor
{
	[NoAutoStaticsCleanup]
	private static readonly HashSet<string> s_SystemFilters = new HashSet<string>(new string[2] { "nodetype", "strict" });

	[NoAutoStaticsCleanup]
	private static readonly HierarchySearchQueryDescriptor s_Empty = new HierarchySearchQueryDescriptor();

	[NoAutoStaticsCleanup]
	private static readonly HierarchySearchQueryDescriptor s_InvalidQuery = new HierarchySearchQueryDescriptor
	{
		Invalid = true
	};

	private string m_Query;

	public static HierarchySearchQueryDescriptor Empty => s_Empty;

	public static HierarchySearchQueryDescriptor InvalidQuery => s_InvalidQuery;

	public HierarchySearchFilter[] SystemFilters { get; private set; }

	public HierarchySearchFilter[] Filters { get; private set; }

	public string[] TextValues { get; private set; }

	public bool Strict { get; set; }

	public bool Invalid { get; set; }

	public bool IsValid => !Invalid && !IsEmpty;

	public bool IsEmpty => Filters.Length == 0 && TextValues.Length == 0 && SystemFilters.Length == 0;

	public bool IsSystemOnlyQuery => SystemFilters.Length != 0 && Filters.Length == 0 && TextValues.Length == 0;

	public string Query
	{
		get
		{
			if (m_Query == null || (m_Query == "" && (SystemFilters.Length != 0 || TextValues.Length != 0 || Filters.Length != 0)))
			{
				m_Query = BuildQuery();
			}
			return m_Query;
		}
	}

	public HierarchySearchQueryDescriptor(HierarchySearchFilter[] filters = null, string[] textValues = null)
	{
		filters = filters ?? new HierarchySearchFilter[0];
		textValues = textValues ?? new string[0];
		Filters = Where(filters, (HierarchySearchFilter f) => !s_SystemFilters.Contains(f.Name));
		SystemFilters = Where(filters, (HierarchySearchFilter f) => s_SystemFilters.Contains(f.Name));
		TextValues = textValues;
		HierarchySearchFilter hierarchySearchFilter = HierarchySearchFilter.Invalid;
		HierarchySearchFilter[] systemFilters = SystemFilters;
		for (int num = 0; num < systemFilters.Length; num++)
		{
			HierarchySearchFilter hierarchySearchFilter2 = systemFilters[num];
			if (hierarchySearchFilter2.Name == "strict")
			{
				hierarchySearchFilter = hierarchySearchFilter2;
				break;
			}
		}
		Invalid = false;
		Strict = !hierarchySearchFilter.IsValid || hierarchySearchFilter.Value == "true";
	}

	public HierarchySearchQueryDescriptor(HierarchySearchQueryDescriptor desc)
	{
		SystemFilters = new HierarchySearchFilter[desc.SystemFilters.Length];
		Array.Copy(desc.SystemFilters, SystemFilters, desc.SystemFilters.Length);
		Filters = new HierarchySearchFilter[desc.Filters.Length];
		Array.Copy(desc.Filters, Filters, desc.Filters.Length);
		TextValues = new string[desc.TextValues.Length];
		Array.Copy(desc.TextValues, TextValues, desc.TextValues.Length);
		Strict = desc.Strict;
		Invalid = desc.Invalid;
	}

	public override string ToString()
	{
		return Query;
	}

	[VisibleToOtherModules(new string[] { "UnityEditor.HierarchyModule" })]
	internal string BuildFilterQuery()
	{
		return string.Join(" ", Filters);
	}

	internal string BuildSystemFilterQuery()
	{
		return string.Join(" ", SystemFilters);
	}

	internal string BuildTextQuery()
	{
		string[] array = new string[TextValues.Length];
		for (int i = 0; i < array.Length; i++)
		{
			array[i] = HierarchySearchFilter.QuoteStringIfNeeded(TextValues[i]);
		}
		return string.Join(" ", array);
	}

	internal string BuildQuery()
	{
		string text = "";
		if (SystemFilters.Length != 0)
		{
			text += BuildSystemFilterQuery();
		}
		if (Filters.Length != 0)
		{
			if (text.Length > 0)
			{
				text += " ";
			}
			text += BuildFilterQuery();
		}
		if (TextValues.Length != 0)
		{
			if (text.Length > 0)
			{
				text += " ";
			}
			text += BuildTextQuery();
		}
		return text;
	}

	private static T[] Where<T>(IEnumerable<T> src, Func<T, bool> pred)
	{
		int num = 0;
		foreach (T item in src)
		{
			if (pred(item))
			{
				num++;
			}
		}
		T[] array = new T[num];
		int num2 = 0;
		foreach (T item2 in src)
		{
			if (pred(item2))
			{
				array[num2++] = item2;
			}
		}
		return array;
	}
}
