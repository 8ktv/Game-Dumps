using System;
using System.Collections.Generic;

namespace UnityEngine.UIElements;

internal static class UxmlDescriptionRegistry
{
	private static readonly Dictionary<Type, UxmlTypeDescription> s_UxmlDescriptions = new Dictionary<Type, UxmlTypeDescription>();

	public static UxmlTypeDescription GetDescription(Type type)
	{
		if (!s_UxmlDescriptions.TryGetValue(type, out var value))
		{
			Dictionary<Type, UxmlTypeDescription> dictionary = s_UxmlDescriptions;
			value = new UxmlTypeDescription(type);
			dictionary.Add(type, value);
		}
		return value;
	}

	public static void Clear()
	{
		s_UxmlDescriptions.Clear();
	}
}
