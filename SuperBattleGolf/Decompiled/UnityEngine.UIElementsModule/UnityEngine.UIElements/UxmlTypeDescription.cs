using System;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine.Pool;
using UnityEngine.Serialization;

namespace UnityEngine.UIElements;

internal readonly struct UxmlTypeDescription
{
	private static readonly Type s_UxmlSerializedDataType = typeof(UxmlSerializedData);

	public readonly Type type;

	public readonly List<UxmlDescription> attributeDescriptions;

	public readonly Dictionary<string, int> uxmlNameToIndex;

	public readonly Dictionary<string, int> cSharpNameToIndex;

	public readonly bool isEditorOnly;

	public UxmlTypeDescription(Type type)
	{
		if (!typeof(UxmlSerializedData).IsAssignableFrom(type))
		{
			throw new ArgumentException();
		}
		this.type = type;
		attributeDescriptions = new List<UxmlDescription>();
		uxmlNameToIndex = new Dictionary<string, int>();
		cSharpNameToIndex = new Dictionary<string, int>();
		if (UxmlDescriptionCache.TryGetCachedDescription(type, out var description))
		{
			isEditorOnly = description.editorOnly;
		}
		else
		{
			isEditorOnly = false;
		}
		GenerateAttributeDescription(type, description.attributeNames);
	}

	private void GenerateAttributeDescription(Type t, UxmlAttributeNames[] attributes)
	{
		if (t.BaseType != null && t.BaseType != s_UxmlSerializedDataType)
		{
			UxmlTypeDescription description = UxmlDescriptionRegistry.GetDescription(t.BaseType);
			attributeDescriptions.AddRange(description.attributeDescriptions);
			foreach (KeyValuePair<string, int> item2 in description.uxmlNameToIndex)
			{
				uxmlNameToIndex[item2.Key] = item2.Value;
			}
			foreach (KeyValuePair<string, int> item3 in description.cSharpNameToIndex)
			{
				cSharpNameToIndex[item3.Key] = item3.Value;
			}
		}
		if (attributes != null)
		{
			for (int i = 0; i < attributes.Length; i++)
			{
				UxmlAttributeNames names = attributes[i];
				FieldInfo field = t.GetField(names.fieldName, BindingFlags.Instance | BindingFlags.NonPublic);
				if (null == field)
				{
					Debug.Log(t.DeclaringType.Name + ": " + names.fieldName + " not found.");
				}
				string text = UxmlUtility.ValidateUxmlName(names.uxmlName);
				if (text != null)
				{
					Debug.LogError($"Invalid UXML name '{names.uxmlName}' for attribute '{field.Name}' in type '{field.DeclaringType.DeclaringType}'. {text}");
					continue;
				}
				int value;
				bool flag = uxmlNameToIndex.TryGetValue(names.uxmlName, out value);
				string overriddenCSharpName = null;
				if (flag)
				{
					overriddenCSharpName = attributeDescriptions[value].overriddenCSharpName ?? attributeDescriptions[value].cSharpName;
				}
				UxmlDescription uxmlDescription = new UxmlDescription(field, names, overriddenCSharpName);
				if (flag)
				{
					attributeDescriptions[value] = uxmlDescription;
				}
				else
				{
					attributeDescriptions.Add(uxmlDescription);
					value = attributeDescriptions.Count - 1;
					uxmlNameToIndex[names.uxmlName] = value;
				}
				cSharpNameToIndex[field.Name] = value;
			}
			return;
		}
		FieldInfo[] fields = t.GetFields(BindingFlags.DeclaredOnly | BindingFlags.Instance | BindingFlags.NonPublic);
		if (fields.Length == 0)
		{
			return;
		}
		FieldInfo[] array = fields;
		foreach (FieldInfo fieldInfo in array)
		{
			if (fieldInfo.GetCustomAttribute<UxmlIgnoreAttribute>() != null)
			{
				continue;
			}
			string name = fieldInfo.Name;
			(bool, string, string[]) uxmlNames = GetUxmlNames(fieldInfo);
			if (uxmlNames.Item1)
			{
				string item = uxmlNames.Item2;
				int value2;
				bool flag2 = uxmlNameToIndex.TryGetValue(item, out value2);
				string overriddenCSharpName2 = null;
				if (flag2)
				{
					overriddenCSharpName2 = attributeDescriptions[value2].overriddenCSharpName ?? attributeDescriptions[value2].cSharpName;
				}
				UxmlDescription uxmlDescription2 = new UxmlDescription(uxmlNames.Item2, name, overriddenCSharpName2, fieldInfo, uxmlNames.Item3);
				if (flag2)
				{
					attributeDescriptions[value2] = uxmlDescription2;
				}
				else
				{
					attributeDescriptions.Add(uxmlDescription2);
					value2 = attributeDescriptions.Count - 1;
					uxmlNameToIndex[item] = value2;
				}
				cSharpNameToIndex[fieldInfo.Name] = value2;
			}
		}
	}

	internal static (bool valid, string uxmlName, string[] obsoleteNames) GetUxmlNames(FieldInfo fieldInfo)
	{
		List<string> value;
		using (CollectionPool<List<string>, string>.Get(out value))
		{
			HashSet<string> value2;
			using (CollectionPool<HashSet<string>, string>.Get(out value2))
			{
				IEnumerable<FormerlySerializedAsAttribute> customAttributes = fieldInfo.GetCustomAttributes<FormerlySerializedAsAttribute>();
				foreach (FormerlySerializedAsAttribute item3 in customAttributes)
				{
					if (value2.Add(item3.oldName))
					{
						value.Add(item3.oldName);
					}
				}
				UxmlAttributeAttribute customAttribute = fieldInfo.GetCustomAttribute<UxmlAttributeAttribute>();
				if (customAttribute != null)
				{
					if (customAttribute.obsoleteNames != null)
					{
						string[] array = customAttribute?.obsoleteNames;
						foreach (string item in array)
						{
							if (value2.Add(item))
							{
								value.Add(item);
							}
						}
					}
					if (!string.IsNullOrWhiteSpace(customAttribute.name))
					{
						string text = UxmlUtility.ValidateUxmlName(customAttribute.name);
						if (text != null)
						{
							Debug.LogError($"Invalid UXML name '{customAttribute.name}' for attribute '{fieldInfo.Name}' in type '{fieldInfo.DeclaringType.DeclaringType}'. {text}");
							return (valid: false, uxmlName: null, obsoleteNames: null);
						}
						return (valid: true, uxmlName: customAttribute.name, obsoleteNames: GetArray(value));
					}
				}
				UxmlObjectReferenceAttribute customAttribute2 = fieldInfo.GetCustomAttribute<UxmlObjectReferenceAttribute>();
				if (customAttribute2 != null && !string.IsNullOrWhiteSpace(customAttribute2.name))
				{
					string text2 = UxmlUtility.ValidateUxmlName(customAttribute2.name);
					if (text2 != null)
					{
						Debug.LogError($"Invalid UXML Object name '{customAttribute2.name}' for attribute '{fieldInfo.Name}' in type '{fieldInfo.DeclaringType.DeclaringType}'. {text2}");
						return (valid: false, uxmlName: null, obsoleteNames: null);
					}
					return (valid: true, uxmlName: customAttribute2.name, obsoleteNames: GetArray(value));
				}
				string item2 = fieldInfo.Name.ToKebabCase();
				return (valid: true, uxmlName: item2, obsoleteNames: GetArray(value));
			}
		}
		static string[] GetArray(List<string> list)
		{
			if (list.Count == 0)
			{
				return Array.Empty<string>();
			}
			return list.ToArray();
		}
	}
}
