using System;
using System.Collections.Generic;

namespace UnityEngine.UIElements;

public static class UxmlDescriptionCache
{
	internal struct CachedDescription
	{
		public UxmlAttributeNames[] attributeNames;

		public bool editorOnly;
	}

	private static readonly Dictionary<Type, CachedDescription> s_NamesPerType = new Dictionary<Type, CachedDescription>();

	public static void RegisterType(Type type, UxmlAttributeNames[] attributeNames, bool isEditorOnly = false)
	{
		s_NamesPerType[type] = new CachedDescription
		{
			attributeNames = attributeNames,
			editorOnly = isEditorOnly
		};
	}

	internal static bool TryGetCachedDescription(Type type, out CachedDescription description)
	{
		return s_NamesPerType.TryGetValue(type, out description);
	}
}
