using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;
using UnityEngine.Bindings;
using UnityEngine.TextCore.Text;

namespace UnityEngine.TextCore;

[VisibleToOtherModules(new string[] { "UnityEngine.UIElementsModule" })]
internal static class TextPreprocessor
{
	private const char k_DoubleQuotes = '"';

	private const char k_GreaterThan = '>';

	private const char k_LessThan = '<';

	private const string k_StyleOpenTag = "<style=\"";

	private const string k_StyleCloseTag = "</style>";

	private const char k_Space = ' ';

	private const char k_NonBreakingSpace = '\u00a0';

	internal const string k_NobrOpenTag = "<nobr>";

	private const string k_NobrCloseTag = "</nobr>";

	[VisibleToOtherModules(new string[] { "UnityEngine.UIElementsModule" })]
	internal static void PreProcessString(ref string text, PreProcessFlags flags, TextSettings? textSettings)
	{
		if (string.IsNullOrEmpty(text))
		{
			return;
		}
		if (textSettings?.defaultStyleSheet != null && RichTextTagParser.ContainsStyleTags(text))
		{
			text = ReplaceStyleTags(text, textSettings);
		}
		if (RichTextTagParser.ContainsNobrTags(text))
		{
			text = ReplaceNobrTags(text);
		}
		if (flags == PreProcessFlags.None)
		{
			return;
		}
		bool flag = (flags & PreProcessFlags.CollapseWhiteSpaces) != 0;
		bool flag2 = (flags & PreProcessFlags.ParseEscapeSequences) != 0;
		PreProcessFlags preProcessFlags = PreProcessFlags.None;
		if (text.IndexOfAny(new char[5] { ' ', '\t', '\r', '\n', '\v' }) != -1)
		{
			preProcessFlags |= PreProcessFlags.CollapseWhiteSpaces;
		}
		if (text.IndexOf('\\') != -1)
		{
			preProcessFlags |= PreProcessFlags.ParseEscapeSequences;
		}
		if ((flags & preProcessFlags) == 0)
		{
			return;
		}
		StringBuilder stringBuilder = new StringBuilder(text.Length);
		int i = 0;
		bool flag3 = true;
		for (; i < text.Length; i++)
		{
			string text2 = "";
			char c = text[i];
			if (flag2 && c == '\\' && i < text.Length - 1)
			{
				i++;
				char c2 = text[i];
				bool flag4 = true;
				switch (c2)
				{
				case '\\':
					text2 = "\\";
					break;
				case 'n':
					text2 = "\n";
					break;
				case 'r':
					text2 = "\r";
					break;
				case 't':
					text2 = "\t";
					break;
				case 'v':
					text2 = "\v";
					break;
				case 'u':
					if (i + 4 < text.Length)
					{
						string s2 = text.Substring(i + 1, 4);
						if (uint.TryParse(s2, NumberStyles.HexNumber, CultureInfo.InvariantCulture, out var result2))
						{
							text2 = Convert.ToChar(result2).ToString();
							i += 4;
						}
						else
						{
							flag4 = false;
						}
					}
					else
					{
						flag4 = false;
					}
					break;
				case 'U':
					if (i + 8 < text.Length)
					{
						string s = text.Substring(i + 1, 8);
						if (uint.TryParse(s, NumberStyles.HexNumber, CultureInfo.InvariantCulture, out var result))
						{
							text2 = char.ConvertFromUtf32((int)result);
							i += 8;
						}
						else
						{
							flag4 = false;
						}
					}
					else
					{
						flag4 = false;
					}
					break;
				default:
					flag4 = false;
					break;
				}
				if (!flag4)
				{
					stringBuilder.Append('\\');
					text2 = c2.ToString();
				}
			}
			else
			{
				text2 = c.ToString();
			}
			bool flag5 = text2.Length == 1 && char.IsWhiteSpace(text2[0]) && text2[0] != '\u00a0';
			if (flag && flag5)
			{
				if (text2 == "\n")
				{
					if (stringBuilder.Length > 0 && stringBuilder[stringBuilder.Length - 1] == ' ')
					{
						stringBuilder.Length--;
					}
					stringBuilder.Append('\n');
					flag3 = true;
				}
				else if (!flag3)
				{
					stringBuilder.Append(' ');
					flag3 = true;
				}
			}
			else
			{
				stringBuilder.Append(text2);
				flag3 = flag5;
			}
		}
		if (flag && stringBuilder.Length > 0)
		{
			int num = stringBuilder.Length - 1;
			while (num >= 0 && char.IsWhiteSpace(stringBuilder[num]) && stringBuilder[num] != '\n' && stringBuilder[num] != '\u00a0')
			{
				num--;
			}
			stringBuilder.Length = num + 1;
		}
		text = stringBuilder.ToString();
	}

	private static int GetStyleHashCode(ReadOnlySpan<char> text)
	{
		int num = 0;
		for (int i = 0; i < text.Length; i++)
		{
			num = ((num << 5) + num) ^ TextUtilities.ToUpperFast(text[i]);
		}
		return num;
	}

	private static TextStyle? GetStyle(TextSettings textSettings, int hashCode)
	{
		TextStyleSheet defaultStyleSheet = textSettings.defaultStyleSheet;
		if (defaultStyleSheet == null)
		{
			return null;
		}
		return defaultStyleSheet.GetStyle(hashCode);
	}

	internal static string ReplaceStyleTags(string text, TextSettings textSettings)
	{
		if (string.IsNullOrEmpty(text))
		{
			return text;
		}
		ReadOnlySpan<char> readOnlySpan = MemoryExtensions.AsSpan(text);
		ReadOnlySpan<char> value = MemoryExtensions.AsSpan("<style=\"");
		ReadOnlySpan<char> value2 = MemoryExtensions.AsSpan("</style>");
		StringBuilder stringBuilder = new StringBuilder(text.Length);
		List<TextStyle> list = new List<TextStyle>(4);
		int num = 0;
		ReadOnlySpan<char> readOnlySpan2;
		while (true)
		{
			readOnlySpan2 = readOnlySpan.Slice(num);
			int num2 = readOnlySpan2.IndexOf('<');
			if (num2 == -1)
			{
				break;
			}
			if (num2 > 0)
			{
				stringBuilder.Append(readOnlySpan2.Slice(0, num2));
			}
			num += num2;
			readOnlySpan2 = readOnlySpan.Slice(num);
			if (readOnlySpan2.StartsWith(value))
			{
				int length = value.Length;
				int num3 = readOnlySpan2.Slice(length).IndexOf('"');
				if (num3 != -1)
				{
					int styleHashCode = GetStyleHashCode(readOnlySpan2.Slice(length, num3));
					TextStyle style = GetStyle(textSettings, styleHashCode);
					if (style != null)
					{
						int num4 = length + num3 + 1;
						int num5 = readOnlySpan2.Slice(num4).IndexOf('>');
						if (num5 != -1)
						{
							stringBuilder.Append(style.styleOpeningDefinition);
							list.Add(style);
							num += num4 + num5 + 1;
							continue;
						}
					}
				}
			}
			else if (readOnlySpan2.StartsWith(value2) && list.Count > 0)
			{
				int index = list.Count - 1;
				TextStyle textStyle = list[index];
				list.RemoveAt(index);
				stringBuilder.Append(textStyle.styleClosingDefinition);
				num += value2.Length;
				continue;
			}
			stringBuilder.Append('<');
			num++;
		}
		stringBuilder.Append(readOnlySpan2);
		return stringBuilder.ToString();
	}

	internal static string ReplaceNobrTags(string text)
	{
		if (string.IsNullOrEmpty(text))
		{
			return text;
		}
		ReadOnlySpan<char> readOnlySpan = MemoryExtensions.AsSpan(text);
		ReadOnlySpan<char> value = MemoryExtensions.AsSpan("<nobr>");
		ReadOnlySpan<char> value2 = MemoryExtensions.AsSpan("</nobr>");
		StringBuilder stringBuilder = new StringBuilder(text.Length);
		int num = 0;
		int num2 = 0;
		while (num2 < readOnlySpan.Length)
		{
			ReadOnlySpan<char> span = readOnlySpan.Slice(num2);
			if (span.StartsWith(value, StringComparison.OrdinalIgnoreCase))
			{
				num++;
				num2 += value.Length;
				continue;
			}
			if (span.StartsWith(value2, StringComparison.OrdinalIgnoreCase))
			{
				if (num > 0)
				{
					num--;
				}
				num2 += value2.Length;
				continue;
			}
			char c = readOnlySpan[num2];
			if (num > 0 && c == ' ')
			{
				stringBuilder.Append('\u00a0');
			}
			else
			{
				stringBuilder.Append(c);
			}
			num2++;
		}
		return stringBuilder.ToString();
	}
}
