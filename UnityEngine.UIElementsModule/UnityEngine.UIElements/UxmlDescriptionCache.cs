using System;
using System.Collections.Generic;

namespace UnityEngine.UIElements;

public static class UxmlDescriptionCache
{
	private static readonly Dictionary<Type, UxmlAttributeNames[]> s_NamesPerType = new Dictionary<Type, UxmlAttributeNames[]>();

	public static void RegisterType(Type type, UxmlAttributeNames[] attributeNames)
	{
		s_NamesPerType[type] = attributeNames;
	}

	internal static bool TryGetCachedDescription(Type type, out UxmlAttributeNames[] attributes)
	{
		return s_NamesPerType.TryGetValue(type, out attributes);
	}
}
