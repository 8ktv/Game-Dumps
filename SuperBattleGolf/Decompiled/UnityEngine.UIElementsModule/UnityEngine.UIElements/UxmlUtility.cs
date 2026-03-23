using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using UnityEngine.Bindings;

namespace UnityEngine.UIElements;

[VisibleToOtherModules(new string[] { "UnityEditor.UIBuilderModule" })]
internal static class UxmlUtility
{
	private const string s_CommaEncoded = "%2C";

	public static List<string> ParseStringListAttribute(string itemList)
	{
		if (string.IsNullOrEmpty(itemList?.Trim()))
		{
			return null;
		}
		string[] array = itemList.Split(',');
		if (array.Length != 0)
		{
			List<string> list = new List<string>();
			string[] array2 = array;
			foreach (string text in array2)
			{
				list.Add(text.Trim());
			}
			return list;
		}
		return null;
	}

	public static string EncodeListItem(string item)
	{
		return item.Replace(",", "%2C");
	}

	public static string DecodeListItem(string item)
	{
		return item.Replace("%2C", ",");
	}

	public static void MoveListItem(IList list, int src, int dst)
	{
		object value = list[src];
		list.RemoveAt(src);
		list.Insert(dst, value);
	}

	public static float ParseFloat(string value, float defaultValue = 0f)
	{
		float result;
		return float.TryParse(value, NumberStyles.Float, CultureInfo.InvariantCulture, out result) ? result : defaultValue;
	}

	public static byte ParseByte(string value, byte defaultValue = 0)
	{
		byte result;
		return byte.TryParse(value, NumberStyles.Integer, CultureInfo.InvariantCulture, out result) ? result : defaultValue;
	}

	public static sbyte ParseSByte(string value, sbyte defaultValue = 0)
	{
		sbyte result;
		return sbyte.TryParse(value, NumberStyles.Integer, CultureInfo.InvariantCulture, out result) ? result : defaultValue;
	}

	public static short ParseShort(string value, short defaultValue = 0)
	{
		short result;
		return short.TryParse(value, NumberStyles.Integer, CultureInfo.InvariantCulture, out result) ? result : defaultValue;
	}

	public static ushort ParseUShort(string value, ushort defaultValue = 0)
	{
		ushort result;
		return ushort.TryParse(value, NumberStyles.Integer, CultureInfo.InvariantCulture, out result) ? result : defaultValue;
	}

	public static int ParseInt(string value, int defaultValue = 0)
	{
		int result;
		return int.TryParse(value, NumberStyles.Integer, CultureInfo.InvariantCulture, out result) ? result : defaultValue;
	}

	public static uint ParseUint(string value, uint defaultValue = 0u)
	{
		uint result;
		return uint.TryParse(value, NumberStyles.Integer, CultureInfo.InvariantCulture, out result) ? result : defaultValue;
	}

	public static float TryParseFloatAttribute(string attributeName, IUxmlAttributes bag, ref int foundAttributeCounter)
	{
		if (bag.TryGetAttributeValue(attributeName, out var value))
		{
			foundAttributeCounter++;
			return ParseFloat(value);
		}
		return 0f;
	}

	public static int TryParseIntAttribute(string attributeName, IUxmlAttributes bag, ref int foundAttributeCounter)
	{
		if (bag.TryGetAttributeValue(attributeName, out var value))
		{
			foundAttributeCounter++;
			return ParseInt(value);
		}
		return 0;
	}

	public static Type ParseType(string value, Type defaultType = null)
	{
		try
		{
			if (!string.IsNullOrEmpty(value))
			{
				return Type.GetType(value, throwOnError: true);
			}
		}
		catch (Exception exception)
		{
			Debug.LogException(exception);
		}
		return defaultType;
	}

	public static string ValidateUxmlName(string name)
	{
		if (!char.IsLetter(name[0]) && name[0] != '_')
		{
			return "Element names must start with a letter or underscore";
		}
		if (name.StartsWith("xml", StringComparison.OrdinalIgnoreCase))
		{
			return "Element names cannot start with the letters xml (or XML, or Xml, etc)";
		}
		for (int i = 1; i < name.Length; i++)
		{
			char c = name[i];
			if (char.IsWhiteSpace(c) || (!char.IsLetterOrDigit(c) && c != '-' && c != '_' && c != '.'))
			{
				return $"The character '{c}' is invalid. Element names can contain letters, digits, hyphens, underscores, and periods.";
			}
		}
		return null;
	}

	public static string TypeToString(Type value)
	{
		if (value == null)
		{
			return null;
		}
		return value.FullName + ", " + value.Assembly.GetName().Name;
	}

	public static string ValueToString(Bounds value)
	{
		return FormattableString.Invariant($"{value.center.x},{value.center.y},{value.center.z},{value.size.x},{value.size.y},{value.size.z}");
	}

	public static string ValueToString(BoundsInt value)
	{
		return FormattableString.Invariant($"{value.position.x},{value.position.y},{value.position.z},{value.size.x},{value.size.y},{value.size.z}");
	}

	public static string ValueToString(Rect value)
	{
		return FormattableString.Invariant($"{value.x},{value.y},{value.width},{value.height}");
	}

	public static string ValueToString(RectInt value)
	{
		return FormattableString.Invariant($"{value.x},{value.y},{value.width},{value.height}");
	}

	public static string ValueToString(Vector2 value)
	{
		return FormattableString.Invariant($"{value.x},{value.y}");
	}

	public static string ValueToString(Vector2Int value)
	{
		return FormattableString.Invariant($"{value.x},{value.y}");
	}

	public static string ValueToString(Vector3 value)
	{
		return FormattableString.Invariant($"{value.x},{value.y},{value.z}");
	}

	public static string ValueToString(Vector3Int value)
	{
		return FormattableString.Invariant($"{value.x},{value.y},{value.z}");
	}

	public static string ValueToString(Vector4 value)
	{
		return FormattableString.Invariant($"{value.x},{value.y},{value.z},{value.w}");
	}

	public static object CloneObject(object value)
	{
		if (value != null && !(value is string) && !(value is Type) && value.GetType().IsClass)
		{
			return UxmlSerializedDataUtility.CopySerialized(value);
		}
		return value;
	}
}
