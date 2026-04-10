using System;
using System.Collections.Generic;
using UnityEngine.Bindings;
using UnityEngine.Pool;

namespace UnityEngine.UIElements;

[VisibleToOtherModules(new string[] { "UnityEditor.UIBuilderModule" })]
internal static class VisualTreeAssetUtilities
{
	public static IEnumerable<string> EnumerateEnclosingNamespaces(string fullTypeName)
	{
		int startIndex = fullTypeName.Length - 1;
		while (true)
		{
			int lastDot = fullTypeName.LastIndexOf(".", startIndex, StringComparison.Ordinal);
			if (lastDot >= 0)
			{
				yield return fullTypeName.Substring(0, lastDot);
				startIndex = lastDot - 1;
				continue;
			}
			break;
		}
	}

	public static UxmlNamespaceDefinition FindUxmlNamespaceDefinitionFromPrefix(this VisualTreeAsset vta, UxmlAsset asset, string prefix)
	{
		for (UxmlAsset uxmlAsset = asset; uxmlAsset != null; uxmlAsset = uxmlAsset.parentAsset)
		{
			for (int i = 0; i < uxmlAsset.namespaceDefinitions.Count; i++)
			{
				UxmlNamespaceDefinition result = uxmlAsset.namespaceDefinitions[i];
				if (string.Compare(result.prefix, prefix, StringComparison.Ordinal) == 0)
				{
					return result;
				}
			}
		}
		return UxmlNamespaceDefinition.Empty;
	}

	public static UxmlNamespaceDefinition FindUxmlNamespaceDefinitionForTypeName(this VisualTreeAsset vta, UxmlAsset asset, string fullTypeName)
	{
		List<UxmlNamespaceDefinition> value;
		using (CollectionPool<List<UxmlNamespaceDefinition>, UxmlNamespaceDefinition>.Get(out value))
		{
			for (UxmlAsset uxmlAsset = asset; uxmlAsset != null; uxmlAsset = uxmlAsset.parentAsset)
			{
				value.AddRange(uxmlAsset.namespaceDefinitions);
			}
			if (value.Count == 0)
			{
				return UxmlNamespaceDefinition.Empty;
			}
			foreach (string item in EnumerateEnclosingNamespaces(fullTypeName))
			{
				for (int i = 0; i < value.Count; i++)
				{
					if (value[i].resolvedNamespace.Equals(item, StringComparison.Ordinal))
					{
						return value[i];
					}
				}
			}
			return UxmlNamespaceDefinition.Empty;
		}
	}

	public static void GatherUxmlNamespaceDefinitions(this VisualTreeAsset vta, UxmlAsset asset, List<UxmlNamespaceDefinition> definitions)
	{
		for (UxmlAsset uxmlAsset = asset; uxmlAsset != null; uxmlAsset = uxmlAsset.parentAsset)
		{
			definitions.InsertRange(0, uxmlAsset.namespaceDefinitions);
		}
	}
}
