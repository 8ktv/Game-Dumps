using System.Collections.Generic;
using System.Text.RegularExpressions;
using Unity.Scripting.LifecycleManagement;

namespace Unity.Hierarchy;

internal class DefaultHierarchySearchQueryParser : IHierarchySearchQueryParser
{
	[NoAutoStaticsCleanup]
	private static readonly Regex s_Filter = new Regex("([#$\\w\\[\\]]+)(<=|<|>=|>|<|=|:)(.*)", RegexOptions.Compiled);

	private static List<string> Tokenize(string s)
	{
		s = s.Trim();
		List<string> list = new List<string>();
		int num = 0;
		int i = 0;
		while (i < s.Length)
		{
			if (char.IsWhiteSpace(s[i]))
			{
				string item = s.Substring(num, i - num);
				list.Add(item);
				for (i++; i < s.Length && char.IsWhiteSpace(s[i]); i++)
				{
				}
				if (i < s.Length)
				{
					num = i;
				}
			}
			else if (s[i] == '"')
			{
				for (i++; i < s.Length && s[i] != '"'; i++)
				{
				}
				if (i >= s.Length)
				{
					return null;
				}
				i++;
			}
			else
			{
				i++;
			}
		}
		if (i != num)
		{
			string item2 = s.Substring(num, i - num);
			list.Add(item2);
		}
		return list;
	}

	public HierarchySearchQueryDescriptor ParseQuery(string query)
	{
		if (string.IsNullOrWhiteSpace(query))
		{
			return HierarchySearchQueryDescriptor.Empty;
		}
		List<string> list = Tokenize(query);
		if (list == null)
		{
			return HierarchySearchQueryDescriptor.InvalidQuery;
		}
		List<string> list2 = new List<string>();
		List<HierarchySearchFilter> list3 = new List<HierarchySearchFilter>();
		bool flag = true;
		foreach (string item in list)
		{
			Match match = s_Filter.Match(item);
			if (match.Success)
			{
				if (match.Groups.Count < 4 || string.IsNullOrEmpty(match.Groups[1].Value) || string.IsNullOrEmpty(match.Groups[2].Value) || string.IsNullOrEmpty(match.Groups[3].Value))
				{
					flag = false;
					break;
				}
				list3.Add(HierarchySearchFilter.CreateFilter(match.Groups[1].Value, match.Groups[2].Value, match.Groups[3].Value));
			}
			else
			{
				list2.Add(item);
			}
		}
		if (!flag)
		{
			return HierarchySearchQueryDescriptor.InvalidQuery;
		}
		return new HierarchySearchQueryDescriptor(list3.ToArray(), list2.ToArray());
	}
}
