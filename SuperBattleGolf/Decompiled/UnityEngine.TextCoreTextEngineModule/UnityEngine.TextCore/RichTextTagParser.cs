#define UNITY_ASSERTIONS
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Globalization;
using System.Runtime.CompilerServices;
using System.Text;
using UnityEngine.Bindings;
using UnityEngine.TextCore.Text;

namespace UnityEngine.TextCore;

[VisibleToOtherModules(new string[] { "UnityEngine.UIElementsModule" })]
internal static class RichTextTagParser
{
	public enum TagType
	{
		Hyperlink,
		Align,
		AllCaps,
		Alpha,
		Bold,
		Br,
		Color,
		CSpace,
		Font,
		FontWeight,
		Gradient,
		Italic,
		Indent,
		LineHeight,
		LineIndent,
		Link,
		Lowercase,
		Margin,
		MarginLeft,
		MarginRight,
		Mark,
		Mspace,
		NoBr,
		NoParse,
		Strikethrough,
		Size,
		SmallCaps,
		Space,
		Sprite,
		Style,
		Subscript,
		Superscript,
		Underline,
		Uppercase,
		VOffset,
		Width,
		Rotate,
		Pos,
		Material,
		Page,
		Action,
		Cr,
		Zwsp,
		Zwj,
		Nbsp,
		Shy,
		Unknown
	}

	public enum ValueID
	{
		Color,
		Padding,
		AssetID,
		GlyphMetrics,
		Scale,
		Tint,
		SpriteColor,
		Gradient
	}

	internal record TagTypeInfo
	{
		public TagType TagType;

		public string name;

		public TagValueType valueType;

		public TagUnitType unitType;

		internal TagTypeInfo(TagType tagType, string name, TagValueType valueType = TagValueType.None, TagUnitType unitType = TagUnitType.Unknown)
		{
			TagType = tagType;
			this.name = name;
			this.valueType = valueType;
			this.unitType = unitType;
		}
	}

	internal enum TagValueType
	{
		None,
		NumericalValue,
		StringValue,
		ColorValue,
		Vector4Value,
		GlyphMetricsValue,
		BoolValue
	}

	internal enum TagUnitType
	{
		Unknown,
		Pixels,
		FontUnits,
		Percentage
	}

	internal record TagValue
	{
		internal string? StringValue
		{
			get
			{
				if (type != TagValueType.StringValue)
				{
					throw new InvalidOperationException("Not a string value");
				}
				return m_stringValue;
			}
		}

		internal float NumericalValue
		{
			get
			{
				if (type != TagValueType.NumericalValue)
				{
					throw new InvalidOperationException("Not a numerical value");
				}
				return m_numericalValue;
			}
		}

		internal Color ColorValue
		{
			get
			{
				if (type != TagValueType.ColorValue)
				{
					throw new InvalidOperationException("Not a color value");
				}
				return m_colorValue;
			}
		}

		internal Vector4 Vector4Value
		{
			get
			{
				if (type != TagValueType.Vector4Value)
				{
					throw new InvalidOperationException("Not a vector4 value");
				}
				return m_vector4Value;
			}
		}

		internal GlyphMetrics GlyphMetricsValue
		{
			get
			{
				if (type != TagValueType.GlyphMetricsValue)
				{
					throw new InvalidOperationException("Not a GlyphMetrics value");
				}
				return m_glyphMetricsValue;
			}
		}

		internal bool BoolValue
		{
			get
			{
				if (type != TagValueType.BoolValue)
				{
					throw new InvalidOperationException("Not a Bool value");
				}
				return m_boolValue;
			}
		}

		internal ValueID? ID => m_ID;

		internal TagValueType type;

		internal TagUnitType unit;

		private string? m_stringValue;

		private float m_numericalValue;

		private Color m_colorValue;

		private Vector4 m_vector4Value;

		private GlyphMetrics m_glyphMetricsValue;

		private bool m_boolValue;

		private ValueID? m_ID;

		internal TagValue(float value, TagUnitType tagUnitType = TagUnitType.Unknown, ValueID? id = null)
		{
			type = TagValueType.NumericalValue;
			unit = tagUnitType;
			m_numericalValue = value;
			m_ID = id;
		}

		internal TagValue(Color value, ValueID? id = null)
		{
			type = TagValueType.ColorValue;
			m_colorValue = value;
			m_ID = id;
		}

		internal TagValue(string value, ValueID? id = null)
		{
			type = TagValueType.StringValue;
			m_stringValue = value;
			m_ID = id;
		}

		internal TagValue(Vector4 value, ValueID? id = null)
		{
			type = TagValueType.Vector4Value;
			m_vector4Value = value;
			m_ID = id;
		}

		internal TagValue(GlyphMetrics value, ValueID? id = null)
		{
			type = TagValueType.GlyphMetricsValue;
			m_glyphMetricsValue = value;
			m_ID = id;
		}

		internal TagValue(bool value, ValueID? id = null)
		{
			type = TagValueType.BoolValue;
			m_boolValue = value;
			m_ID = id;
		}

		[CompilerGenerated]
		protected virtual bool PrintMembers(StringBuilder builder)
		{
			return false;
		}
	}

	internal struct Tag
	{
		public TagType tagType;

		public bool isClosing;

		public int start;

		public int end;

		public TagValue? value;

		public TagValue? value2;

		public TagValue? value3;

		public TagValue? value4;

		public TagValue? value5;

		public sbyte nestingLevel;
	}

	public struct Segment
	{
		public List<Tag>? tags;

		public int start;

		public int end;
	}

	internal record ParseError
	{
		public readonly int position;

		public readonly string message;

		internal ParseError(string message, int position)
		{
			this.message = message;
			this.position = position;
		}
	}

	internal static readonly Color32 k_HighlightColor = new Color32(byte.MaxValue, byte.MaxValue, 0, 64);

	internal static readonly char k_PrivateArea = '\ue000';

	internal static Color s_AtgHyperlinkColor = new Color(0.29803923f, 42f / 85f, 1f, 1f);

	[VisibleToOtherModules(new string[] { "UnityEngine.UIElementsModule" })]
	internal static readonly Dictionary<string, IntPtr> s_FontAssetCache = new Dictionary<string, IntPtr>();

	internal static readonly Dictionary<string, WeakReference<SpriteAsset>> s_SpriteAssetCache = new Dictionary<string, WeakReference<SpriteAsset>>();

	internal static readonly Dictionary<string, IntPtr> s_GradientAssetCache = new Dictionary<string, IntPtr>();

	private static readonly ConcurrentDictionary<TagType, byte> s_LoggedUnsupportedTagWarnings = new ConcurrentDictionary<TagType, byte>();

	internal static readonly TagTypeInfo[] TagsInfo = new TagTypeInfo[46]
	{
		new TagTypeInfo(TagType.Hyperlink, "a"),
		new TagTypeInfo(TagType.Align, "align"),
		new TagTypeInfo(TagType.AllCaps, "allcaps"),
		new TagTypeInfo(TagType.Alpha, "alpha"),
		new TagTypeInfo(TagType.Bold, "b"),
		new TagTypeInfo(TagType.Br, "br"),
		new TagTypeInfo(TagType.Color, "color", TagValueType.ColorValue),
		new TagTypeInfo(TagType.CSpace, "cspace"),
		new TagTypeInfo(TagType.Font, "font"),
		new TagTypeInfo(TagType.FontWeight, "font-weight"),
		new TagTypeInfo(TagType.Gradient, "gradient"),
		new TagTypeInfo(TagType.Italic, "i"),
		new TagTypeInfo(TagType.Indent, "indent"),
		new TagTypeInfo(TagType.LineHeight, "line-height"),
		new TagTypeInfo(TagType.LineIndent, "line-indent"),
		new TagTypeInfo(TagType.Link, "link"),
		new TagTypeInfo(TagType.Lowercase, "lowercase"),
		new TagTypeInfo(TagType.Margin, "margin"),
		new TagTypeInfo(TagType.MarginLeft, "margin-left"),
		new TagTypeInfo(TagType.MarginRight, "margin-right"),
		new TagTypeInfo(TagType.Mark, "mark"),
		new TagTypeInfo(TagType.Mspace, "mspace"),
		new TagTypeInfo(TagType.NoBr, "nobr"),
		new TagTypeInfo(TagType.NoParse, "noparse"),
		new TagTypeInfo(TagType.Strikethrough, "s"),
		new TagTypeInfo(TagType.Size, "size"),
		new TagTypeInfo(TagType.SmallCaps, "smallcaps"),
		new TagTypeInfo(TagType.Space, "space"),
		new TagTypeInfo(TagType.Sprite, "sprite"),
		new TagTypeInfo(TagType.Style, "style"),
		new TagTypeInfo(TagType.Subscript, "sub"),
		new TagTypeInfo(TagType.Superscript, "sup"),
		new TagTypeInfo(TagType.Underline, "u"),
		new TagTypeInfo(TagType.Uppercase, "uppercase"),
		new TagTypeInfo(TagType.VOffset, "voffset"),
		new TagTypeInfo(TagType.Width, "width"),
		new TagTypeInfo(TagType.Rotate, "rotate"),
		new TagTypeInfo(TagType.Pos, "pos"),
		new TagTypeInfo(TagType.Material, "material"),
		new TagTypeInfo(TagType.Page, "page"),
		new TagTypeInfo(TagType.Action, "action"),
		new TagTypeInfo(TagType.Cr, "cr"),
		new TagTypeInfo(TagType.Zwsp, "zwsp"),
		new TagTypeInfo(TagType.Zwj, "zwj"),
		new TagTypeInfo(TagType.Nbsp, "nbsp"),
		new TagTypeInfo(TagType.Shy, "shy")
	};

	private const string k_FontTag = "<font=";

	private const string k_SpriteTag = "<sprite";

	private const string k_StyleTag = "<style=\"";

	private const string k_GradientTag = "<gradient";

	private static bool tagMatch(ReadOnlySpan<char> tagCandidate, string tagName)
	{
		return tagCandidate.StartsWith(MemoryExtensions.AsSpan(tagName)) && (tagCandidate.Length == tagName.Length || (!char.IsLetter(tagCandidate[tagName.Length]) && tagCandidate[tagName.Length] != '-'));
	}

	private static bool SpanToEnum(ReadOnlySpan<char> tagCandidate, out TagType tagType, out string? error, out ReadOnlySpan<char> attribute)
	{
		for (int i = 0; i < TagsInfo.Length; i++)
		{
			string name = TagsInfo[i].name;
			if (tagMatch(tagCandidate, name))
			{
				tagType = TagsInfo[i].TagType;
				error = null;
				attribute = tagCandidate.Slice(name.Length);
				return true;
			}
		}
		if (tagCandidate.Length > 4 && tagCandidate[0] == '#')
		{
			tagType = TagType.Color;
			error = null;
			attribute = tagCandidate;
			return true;
		}
		error = "Unknown tag: " + tagCandidate;
		tagType = TagType.Unknown;
		attribute = null;
		return false;
	}

	private static TagValue? ParseColorAttribute(ReadOnlySpan<char> attributeSection)
	{
		attributeSection = GetAttributeSpan(attributeSection);
		if (ColorUtility.TryParseHtmlString(attributeSection, out var color))
		{
			return new TagValue(color, ValueID.Color);
		}
		return null;
	}

	private static TagValue? ParseAlphaAttribute(ReadOnlySpan<char> attributeSection)
	{
		attributeSection = GetAttributeSpan(attributeSection);
		if (attributeSection.Length != 3 || attributeSection[0] != '#')
		{
			return null;
		}
		int num = HexCharToInt(attributeSection[1]);
		int num2 = HexCharToInt(attributeSection[2]);
		if (num < 0 || num2 < 0)
		{
			return null;
		}
		byte b = (byte)(num * 16 + num2);
		return new TagValue((int)b);
	}

	private static int HexCharToInt(char hex)
	{
		if (1 == 0)
		{
		}
		int result;
		switch (hex)
		{
		case '0':
		case '1':
		case '2':
		case '3':
		case '4':
		case '5':
		case '6':
		case '7':
		case '8':
		case '9':
			result = hex - 48;
			break;
		case 'A':
		case 'B':
		case 'C':
		case 'D':
		case 'E':
		case 'F':
			result = hex - 65 + 10;
			break;
		case 'a':
		case 'b':
		case 'c':
		case 'd':
		case 'e':
		case 'f':
			result = hex - 97 + 10;
			break;
		default:
			result = -1;
			break;
		}
		if (1 == 0)
		{
		}
		return result;
	}

	private static TagValue? ParsePaddingAttribute(ReadOnlySpan<char> value)
	{
		Span<int> span = stackalloc int[4];
		int num = 0;
		while (!value.IsEmpty && num < 4)
		{
			int num2 = value.IndexOf(',');
			ReadOnlySpan<char> s;
			if (num2 >= 0)
			{
				s = value.Slice(0, num2);
				value = value.Slice(num2 + 1);
			}
			else
			{
				s = value;
				value = ReadOnlySpan<char>.Empty;
			}
			if (!int.TryParse(s, NumberStyles.Integer, CultureInfo.InvariantCulture, out span[num]))
			{
				return null;
			}
			num++;
		}
		if (num != 4)
		{
			return null;
		}
		return new TagValue(new Vector4(span[0], span[1], span[2], span[3]), ValueID.Padding);
	}

	private static TagValue? ParseHref(ReadOnlySpan<char> attributeSection)
	{
		if (TryGetSimpleHref(attributeSection, out string hrefValue))
		{
			return new TagValue(hrefValue);
		}
		return new TagValue(attributeSection.TrimStart().ToString());
	}

	private static bool TryGetSimpleHref(ReadOnlySpan<char> attributeSection, out string hrefValue)
	{
		hrefValue = "";
		attributeSection = attributeSection.Trim();
		if (!attributeSection.StartsWith(MemoryExtensions.AsSpan("href="), StringComparison.OrdinalIgnoreCase))
		{
			return false;
		}
		ReadOnlySpan<char> span = attributeSection.Slice("href=".Length);
		char c = ((span.Length > 0) ? span[0] : '\0');
		if (c == '"' || c == '\'')
		{
			ReadOnlySpan<char> span2 = span.Slice(1);
			int num = span2.IndexOf(c);
			if (num == -1)
			{
				return false;
			}
			if (span2.Slice(num + 1).Trim().Length > 0)
			{
				return false;
			}
			hrefValue = span2.Slice(0, num).ToString();
		}
		else
		{
			if (span.Contains(new ReadOnlySpan<char>(new char[1] { ' ' }), StringComparison.OrdinalIgnoreCase))
			{
				return false;
			}
			hrefValue = span.ToString();
		}
		return true;
	}

	private static bool ParseSpriteAttributes(ReadOnlySpan<char> attributeSection, TextSettings textSettings, out char unicode, out TagValue? spriteAssetValue, out TagValue? glyphMetricsValue, out TagValue? tintValue, out TagValue? scaleValue, out TagValue? colorValue, out string? spriteAssetNameOut)
	{
		int num = -1;
		unicode = '\0';
		spriteAssetValue = null;
		glyphMetricsValue = null;
		tintValue = null;
		scaleValue = null;
		colorValue = null;
		spriteAssetNameOut = null;
		ReadOnlySpan<char> readOnlySpan = ReadOnlySpan<char>.Empty;
		ReadOnlySpan<char> readOnlySpan2 = ReadOnlySpan<char>.Empty;
		SpriteAsset target = null;
		while (!attributeSection.IsEmpty)
		{
			attributeSection = attributeSection.TrimStart();
			if (attributeSection.IsEmpty)
			{
				break;
			}
			int num2 = attributeSection.IndexOf('=');
			if (num2 == -1)
			{
				break;
			}
			ReadOnlySpan<char> span = attributeSection.Slice(0, num2).Trim();
			ReadOnlySpan<char> readOnlySpan3 = attributeSection.Slice(num2 + 1).TrimStart();
			char c = ((readOnlySpan3.Length > 0) ? readOnlySpan3[0] : '\0');
			ReadOnlySpan<char> readOnlySpan4;
			if (c == '"' || c == '\'')
			{
				ReadOnlySpan<char> span2 = readOnlySpan3.Slice(1);
				int num3 = span2.IndexOf(c);
				if (num3 == -1)
				{
					break;
				}
				readOnlySpan4 = span2.Slice(0, num3);
				attributeSection = span2.Slice(num3 + 1);
			}
			else
			{
				int num4 = readOnlySpan3.IndexOf(' ');
				if (num4 == -1)
				{
					readOnlySpan4 = readOnlySpan3;
					attributeSection = ReadOnlySpan<char>.Empty;
				}
				else
				{
					readOnlySpan4 = readOnlySpan3.Slice(0, num4);
					attributeSection = readOnlySpan3.Slice(num4);
				}
			}
			if (span.IsEmpty)
			{
				if (int.TryParse(readOnlySpan4, out var result))
				{
					num = result;
				}
				else
				{
					readOnlySpan = readOnlySpan4;
				}
			}
			else if (span.SequenceEqual("name"))
			{
				readOnlySpan2 = readOnlySpan4;
			}
			else if (span.SequenceEqual("index"))
			{
				if (int.TryParse(readOnlySpan4, out var result2))
				{
					num = result2;
				}
			}
			else if (span.SequenceEqual("tint"))
			{
				if (int.TryParse(readOnlySpan4, out var result3) && result3 == 1)
				{
					tintValue = new TagValue(value: true, ValueID.Tint);
				}
			}
			else if (span.SequenceEqual("color"))
			{
				readOnlySpan4 = GetAttributeSpan(readOnlySpan4);
				if (ColorUtility.TryParseHtmlString(readOnlySpan4, out var color))
				{
					colorValue = new TagValue(color, ValueID.SpriteColor);
				}
			}
		}
		if (!readOnlySpan.IsEmpty)
		{
			spriteAssetNameOut = readOnlySpan.ToString();
			if (!s_SpriteAssetCache.TryGetValue(spriteAssetNameOut, out WeakReference<SpriteAsset> value) || !value.TryGetTarget(out target))
			{
				return false;
			}
		}
		else
		{
			if (textSettings.defaultSpriteAsset != null)
			{
				target = textSettings.defaultSpriteAsset;
			}
			else if (TextSettings.s_GlobalSpriteAsset != null)
			{
				target = TextSettings.s_GlobalSpriteAsset;
			}
			if (target == null)
			{
				return false;
			}
		}
		if (!readOnlySpan2.IsEmpty)
		{
			num = target.GetSpriteIndexFromName(readOnlySpan2.ToString());
		}
		if (num == -1)
		{
			return false;
		}
		if (target.spriteCharacterTable.Count <= num)
		{
			return false;
		}
		SpriteCharacter spriteCharacter = target.spriteCharacterTable[num];
		spriteAssetValue = new TagValue(target.instanceID, TagUnitType.Unknown, ValueID.AssetID);
		glyphMetricsValue = new TagValue(spriteCharacter.glyph.metrics, ValueID.GlyphMetrics);
		scaleValue = new TagValue(spriteCharacter.scale, TagUnitType.Unknown, ValueID.Scale);
		unicode = (char)(k_PrivateArea + num);
		return true;
	}

	public static int GetHashCode(ReadOnlySpan<char> span)
	{
		HashCode hashCode = default(HashCode);
		ReadOnlySpan<char> readOnlySpan = span;
		for (int i = 0; i < readOnlySpan.Length; i++)
		{
			char value = readOnlySpan[i];
			hashCode.Add(value);
		}
		return hashCode.ToHashCode();
	}

	[VisibleToOtherModules(new string[] { "UnityEngine.UIElementsModule" })]
	internal static void PreloadFontAssetsFromTags(string text, TextSettings textSettings)
	{
		if (!HasFontTags(text, textSettings, out List<string> fontAssetNames))
		{
			return;
		}
		foreach (string item in fontAssetNames)
		{
			if (!s_FontAssetCache.ContainsKey(item))
			{
				FontAsset fontAsset = Resources.Load<FontAsset>(textSettings.defaultFontAssetPath + item);
				if (!(fontAsset == null))
				{
					fontAsset.EnsureNativeFontAssetIsCreated();
					s_FontAssetCache[item] = fontAsset.nativeFontAsset;
				}
			}
		}
	}

	[VisibleToOtherModules(new string[] { "UnityEngine.UIElementsModule" })]
	internal static void PreloadSpriteAssetsFromTags(string text, TextSettings textSettings)
	{
		if (!HasSpriteTags(text, textSettings, out List<string> spriteAssetNames))
		{
			return;
		}
		foreach (string item in spriteAssetNames)
		{
			if (!s_SpriteAssetCache.ContainsKey(item))
			{
				SpriteAsset spriteAsset = Resources.Load<SpriteAsset>(textSettings.defaultSpriteAssetPath + item);
				if (!(spriteAsset == null))
				{
					spriteAsset.UpdateLookupTables();
					s_SpriteAssetCache[item] = new WeakReference<SpriteAsset>(spriteAsset);
				}
			}
		}
	}

	[VisibleToOtherModules(new string[] { "UnityEngine.UIElementsModule" })]
	internal static void PreloadGradientAssetsFromTags(string text, TextSettings textSettings)
	{
		if (!HasGradientTags(text, textSettings, out List<string> gradientAssetNames))
		{
			return;
		}
		foreach (string item in gradientAssetNames)
		{
			if (!s_GradientAssetCache.ContainsKey(item))
			{
				TextColorGradient textColorGradient = Resources.Load<TextColorGradient>(textSettings.defaultColorGradientPresetsPath + item);
				if (!(textColorGradient == null))
				{
					textColorGradient.MarkNativeDirty();
					s_GradientAssetCache[item] = textColorGradient.nativeInstance;
				}
			}
		}
	}

	internal static List<Tag> FindTags(ref string inputStr, TextSettings textSettings, bool preprocessingOnly = false, List<ParseError>? errors = null)
	{
		char[] array = inputStr.ToCharArray();
		List<Tag> list = new List<Tag>();
		int num = 0;
		int num2 = 0;
		int num3 = 0;
		while (true)
		{
			int num4 = Array.IndexOf(array, '<', num);
			if (num4 == -1)
			{
				break;
			}
			int num5 = Array.IndexOf(array, '>', num4);
			if (num5 == -1)
			{
				break;
			}
			bool flag = array.Length > num4 + 1 && array[num4 + 1] == '/';
			if (num5 == num4 + 1)
			{
				errors?.Add(new ParseError("Empty tag", num4));
				num = num5 + 1;
				continue;
			}
			num = num5 + 1;
			TagType tagType2;
			string error2;
			ReadOnlySpan<char> attribute2;
			if (!flag)
			{
				Span<char> span = MemoryExtensions.AsSpan(array, num4 + 1, num5 - num4 - 1);
				if (SpanToEnum(span, out TagType tagType, out string error, out ReadOnlySpan<char> attribute))
				{
					TagValue spriteAssetValue = null;
					TagValue glyphMetricsValue = null;
					if (tagType == TagType.Color)
					{
						spriteAssetValue = ParseColorAttribute(attribute);
						if ((object)spriteAssetValue == null)
						{
							errors?.Add(new ParseError("Invalid color value", num4));
							num = num4 + 1;
							continue;
						}
					}
					if (tagType == TagType.Alpha)
					{
						spriteAssetValue = ParseAlphaAttribute(attribute);
						if ((object)spriteAssetValue == null)
						{
							errors?.Add(new ParseError("Invalid alpha value", num4));
							num = num4 + 1;
							continue;
						}
					}
					if (tagType == TagType.Mark)
					{
						spriteAssetValue = ParseColorAttribute(attribute);
						if (spriteAssetValue == null)
						{
							while (!attribute.IsEmpty)
							{
								int num6 = attribute.IndexOf(' ');
								ReadOnlySpan<char> span2;
								if (num6 >= 0)
								{
									span2 = attribute.Slice(0, num6);
									attribute = ((num6 + 1 >= attribute.Length) ? ReadOnlySpan<char>.Empty : attribute.Slice(num6 + 1));
								}
								else
								{
									span2 = attribute;
									attribute = ReadOnlySpan<char>.Empty;
								}
								int num7 = span2.IndexOf('=');
								if (num7 > 0 && num7 < span2.Length - 1)
								{
									ReadOnlySpan<char> span3 = span2.Slice(0, num7);
									ReadOnlySpan<char> readOnlySpan = span2.Slice(num7 + 1);
									if (span3.SequenceEqual("color"))
									{
										spriteAssetValue = ParseColorAttribute(readOnlySpan);
									}
									else if (span3.SequenceEqual("padding"))
									{
										glyphMetricsValue = ParsePaddingAttribute(readOnlySpan);
									}
								}
							}
						}
					}
					if (tagType == TagType.Hyperlink)
					{
						spriteAssetValue = ParseHref(attribute);
					}
					if (tagType == TagType.Link)
					{
						attribute = GetAttributeSpan(attribute);
						string value = attribute.ToString();
						spriteAssetValue = new TagValue(value);
					}
					switch (tagType)
					{
					case TagType.Sprite:
					{
						if (!ParseSpriteAttributes(attribute, textSettings, out char unicode, out spriteAssetValue, out glyphMetricsValue, out TagValue tintValue, out TagValue scaleValue, out TagValue colorValue, out string spriteAssetNameOut))
						{
							if (preprocessingOnly && spriteAssetNameOut != null)
							{
								list.Add(new Tag
								{
									tagType = tagType,
									start = num4,
									end = num5,
									isClosing = false,
									value = new TagValue(spriteAssetNameOut)
								});
							}
							continue;
						}
						list.Add(new Tag
						{
							tagType = tagType,
							start = num4,
							end = num5,
							isClosing = false,
							value = spriteAssetValue,
							value2 = glyphMetricsValue,
							value3 = tintValue,
							value4 = scaleValue,
							value5 = colorValue
						});
						inputStr = inputStr.Insert(num5 + 1, unicode + "/");
						array = inputStr.ToCharArray();
						list.Add(new Tag
						{
							tagType = tagType,
							start = num5 + 2,
							end = num5 + 2,
							isClosing = true,
							value = spriteAssetValue,
							value2 = glyphMetricsValue,
							value3 = tintValue,
							value4 = scaleValue,
							value5 = colorValue
						});
						num = num5 + 2;
						continue;
					}
					case TagType.Br:
						if (attribute.IsEmpty)
						{
							list.Add(new Tag
							{
								tagType = tagType,
								start = num4,
								end = num5,
								isClosing = false,
								value = null
							});
							inputStr = inputStr.Insert(num5 + 1, "\n/");
							array = inputStr.ToCharArray();
							list.Add(new Tag
							{
								tagType = tagType,
								start = num5 + 2,
								end = num5 + 2,
								isClosing = true,
								value = null
							});
							num = num5 + 2;
						}
						continue;
					case TagType.Align:
					{
						attribute = GetAttributeSpan(attribute);
						string value2 = attribute.ToString();
						if (Enum.TryParse<HorizontalAlignment>(value2, ignoreCase: true, out var _))
						{
							spriteAssetValue = new TagValue(value2);
						}
						if ((object)spriteAssetValue == null)
						{
							errors?.Add(new ParseError($"Invalid {tagType} value", num4));
							num = num4 + 1;
							continue;
						}
						break;
					}
					}
					if (tagType == TagType.Mspace || tagType == TagType.CSpace)
					{
						TagUnitType tagUnitType = ParseTagUnitType(ref attribute);
						switch (tagUnitType)
						{
						case TagUnitType.Percentage:
							errors?.Add(new ParseError($"Invalid {tagUnitType} value", num4));
							num = num4 + 1;
							continue;
						case TagUnitType.Unknown:
							tagUnitType = TagUnitType.Pixels;
							break;
						}
						attribute = GetAttributeSpan(attribute);
						if (!float.TryParse(attribute, NumberStyles.Float, CultureInfo.InvariantCulture, out var result2))
						{
							errors?.Add(new ParseError("Invalid numerical value", num4));
							num = num4 + 1;
							continue;
						}
						spriteAssetValue = new TagValue(result2, tagUnitType);
					}
					if (tagType == TagType.Margin || tagType == TagType.MarginLeft || tagType == TagType.MarginRight)
					{
						TagUnitType tagUnitType2 = ParseTagUnitType(ref attribute);
						if (tagUnitType2 == TagUnitType.Unknown)
						{
							tagUnitType2 = TagUnitType.Pixels;
						}
						attribute = GetAttributeSpan(attribute);
						if (!float.TryParse(attribute, NumberStyles.Float, CultureInfo.InvariantCulture, out var result3))
						{
							errors?.Add(new ParseError("Invalid numerical value", num4));
							num = num4 + 1;
							continue;
						}
						spriteAssetValue = new TagValue(result3, tagUnitType2);
					}
					if (tagType == TagType.LineHeight)
					{
						TagUnitType tagUnitType3 = ParseTagUnitType(ref attribute);
						if (tagUnitType3 == TagUnitType.Unknown)
						{
							tagUnitType3 = TagUnitType.Pixels;
						}
						attribute = GetAttributeSpan(attribute);
						if (!float.TryParse(attribute, NumberStyles.Float, CultureInfo.InvariantCulture, out var result4))
						{
							errors?.Add(new ParseError("Invalid line-height value", num4));
							num = num4 + 1;
							continue;
						}
						spriteAssetValue = new TagValue(result4, tagUnitType3);
					}
					if (tagType == TagType.Indent)
					{
						TagUnitType tagUnitType4 = ParseTagUnitType(ref attribute);
						if (tagUnitType4 == TagUnitType.Unknown)
						{
							tagUnitType4 = TagUnitType.Pixels;
						}
						attribute = GetAttributeSpan(attribute);
						if (!float.TryParse(attribute, NumberStyles.Float, CultureInfo.InvariantCulture, out var result5))
						{
							errors?.Add(new ParseError("Invalid numerical value", num4));
							num = num4 + 1;
							continue;
						}
						spriteAssetValue = new TagValue(result5, tagUnitType4);
					}
					if (tagType == TagType.VOffset)
					{
						TagUnitType tagUnitType5 = ParseTagUnitType(ref attribute);
						switch (tagUnitType5)
						{
						case TagUnitType.Percentage:
							errors?.Add(new ParseError($"Invalid {tagUnitType5} value", num4));
							num = num4 + 1;
							continue;
						case TagUnitType.Unknown:
							tagUnitType5 = TagUnitType.Pixels;
							break;
						}
						attribute = GetAttributeSpan(attribute);
						if (!float.TryParse(attribute, NumberStyles.Float, CultureInfo.InvariantCulture, out var result6))
						{
							errors?.Add(new ParseError("Invalid numerical value", num4));
							num = num4 + 1;
							continue;
						}
						spriteAssetValue = new TagValue(result6, tagUnitType5);
					}
					if (tagType == TagType.Font)
					{
						attribute = GetAttributeSpan(attribute);
						string text = attribute.ToString();
						if (string.IsNullOrEmpty(text))
						{
							errors?.Add(new ParseError("Font name cannot be empty", num4));
							num = num4 + 1;
							continue;
						}
						if (!s_FontAssetCache.ContainsKey(text))
						{
							if (preprocessingOnly)
							{
								list.Add(new Tag
								{
									tagType = tagType,
									start = num4,
									end = num5,
									isClosing = false,
									value = new TagValue(text)
								});
							}
							num = num4 + 1;
							continue;
						}
						spriteAssetValue = new TagValue(text);
					}
					if (tagType == TagType.Size)
					{
						TagUnitType tagUnitType6 = ParseTagUnitType(ref attribute);
						if (tagUnitType6 == TagUnitType.Unknown)
						{
							tagUnitType6 = TagUnitType.Pixels;
						}
						attribute = GetAttributeSpan(attribute);
						bool value3 = false;
						if (attribute.Length > 0 && (attribute[0] == '+' || attribute[0] == '-'))
						{
							value3 = true;
						}
						if (!float.TryParse(attribute, NumberStyles.Float, CultureInfo.InvariantCulture, out var result7))
						{
							errors?.Add(new ParseError("Invalid size value", num4));
							num = num4 + 1;
							continue;
						}
						spriteAssetValue = new TagValue(result7, tagUnitType6);
						glyphMetricsValue = new TagValue(value3);
					}
					if (tagType == TagType.FontWeight)
					{
						attribute = GetAttributeSpan(attribute);
						if (!int.TryParse(attribute, NumberStyles.Integer, CultureInfo.InvariantCulture, out var result8))
						{
							errors?.Add(new ParseError("Invalid font-weight value", num4));
							num = num4 + 1;
							continue;
						}
						if (!Enum.IsDefined(typeof(TextFontWeight), result8))
						{
							errors?.Add(new ParseError($"Invalid font-weight value: {result8}", num4));
							num = num4 + 1;
							continue;
						}
						spriteAssetValue = new TagValue(result8);
					}
					if (tagType == TagType.Gradient)
					{
						attribute = GetAttributeSpan(attribute);
						string text2 = attribute.ToString();
						if (string.IsNullOrEmpty(text2))
						{
							errors?.Add(new ParseError("Gradient name cannot be empty", num4));
							num = num4 + 1;
							continue;
						}
						if (!s_GradientAssetCache.ContainsKey(text2))
						{
							if (preprocessingOnly)
							{
								list.Add(new Tag
								{
									tagType = tagType,
									start = num4,
									end = num5,
									isClosing = false,
									value = new TagValue(text2)
								});
							}
							num = num4 + 1;
							continue;
						}
						spriteAssetValue = new TagValue(text2, ValueID.Gradient);
					}
					sbyte nestingLevel = 0;
					if (tagType == TagType.Subscript || tagType == TagType.Superscript)
					{
						nestingLevel = ((tagType != TagType.Superscript) ? ((sbyte)(++num2)) : ((sbyte)(++num3)));
					}
					list.Add(new Tag
					{
						tagType = tagType,
						start = num4,
						end = num5,
						isClosing = flag,
						value = spriteAssetValue,
						value2 = glyphMetricsValue,
						nestingLevel = nestingLevel
					});
					if (tagType == TagType.NoParse)
					{
						if ((num4 = MemoryExtensions.AsSpan(array, num).IndexOf("</noparse>")) == -1)
						{
							break;
						}
						num4 += num;
						num5 = num4 + "</noparse>".Length - 1;
						list.Add(new Tag
						{
							tagType = TagType.NoParse,
							start = num4,
							end = num5,
							isClosing = true
						});
						num = num5 + 1;
					}
				}
				else
				{
					if (error != null)
					{
						errors?.Add(new ParseError(error, num4));
					}
					num = num4 + 1;
				}
			}
			else if (SpanToEnum(MemoryExtensions.AsSpan(array, num4 + 2, num5 - num4 - 2), out tagType2, out error2, out attribute2))
			{
				list.Add(new Tag
				{
					tagType = tagType2,
					start = num4,
					end = num5,
					isClosing = flag
				});
				if (tagType2 == TagType.Subscript || tagType2 == TagType.Superscript)
				{
					if (tagType2 == TagType.Superscript)
					{
						num3 = Math.Max(0, num3 - 1);
					}
					else
					{
						num2 = Math.Max(0, num2 - 1);
					}
				}
			}
			else
			{
				if (error2 != null)
				{
					errors?.Add(new ParseError(error2, num4));
				}
				num = num4 + 1;
			}
		}
		return list;
	}

	private static ReadOnlySpan<char> GetAttributeSpan(ReadOnlySpan<char> attributeSection)
	{
		if (attributeSection.Length >= 1 && attributeSection[0] == '=')
		{
			attributeSection = attributeSection.Slice(1);
		}
		if (attributeSection.Length >= 2)
		{
			if (attributeSection[0] == '"')
			{
				if (attributeSection[attributeSection.Length - 1] == '"')
				{
					goto IL_0082;
				}
			}
			if (attributeSection[0] == '\'')
			{
				if (attributeSection[attributeSection.Length - 1] == '\'')
				{
					goto IL_0082;
				}
			}
		}
		return attributeSection;
		IL_0082:
		return attributeSection.Slice(1, attributeSection.Length - 2);
	}

	private static TagUnitType ParseTagUnitType(ref ReadOnlySpan<char> attributeSection)
	{
		if (attributeSection.EndsWith(MemoryExtensions.AsSpan("em"), StringComparison.OrdinalIgnoreCase))
		{
			attributeSection = attributeSection.Slice(0, attributeSection.Length - 2);
			return TagUnitType.FontUnits;
		}
		if (attributeSection.EndsWith(MemoryExtensions.AsSpan("px"), StringComparison.OrdinalIgnoreCase))
		{
			attributeSection = attributeSection.Slice(0, attributeSection.Length - 2);
			return TagUnitType.Pixels;
		}
		if (attributeSection.EndsWith(MemoryExtensions.AsSpan("%"), StringComparison.OrdinalIgnoreCase))
		{
			attributeSection = attributeSection.Slice(0, attributeSection.Length - 1);
			return TagUnitType.Percentage;
		}
		return TagUnitType.Unknown;
	}

	internal static List<Tag> PickResultingTags(List<Tag> allTags, string input, int atPosition, List<Tag>? applicableTags = null)
	{
		if (applicableTags == null)
		{
			applicableTags = new List<Tag>();
		}
		else
		{
			applicableTags.Clear();
		}
		int num = 0;
		Debug.Assert(string.IsNullOrEmpty(input) || (atPosition < input.Length && atPosition >= 0), "Invalid position");
		Debug.Assert(num <= atPosition && num >= 0, "Invalid starting position");
		int num2 = 0;
		foreach (Tag allTag in allTags)
		{
			Debug.Assert(allTag.start >= num2, "Tags are not sorted");
			num2 = allTag.end + 1;
		}
		foreach (Tag applicableTag in applicableTags)
		{
			Debug.Assert(applicableTag.end <= num, "Tag end pass the point where we should start parsing");
			Debug.Assert(allTags.Contains(applicableTag));
		}
		Span<int?> span = stackalloc int?[allTags.Count];
		Span<int?> span2 = stackalloc int?[TagsInfo.Length];
		int num3 = -1;
		foreach (Tag allTag2 in allTags)
		{
			num3++;
			if (allTag2.end < num || allTag2.tagType == TagType.NoParse)
			{
				continue;
			}
			if (allTag2.start > atPosition)
			{
				break;
			}
			if (allTag2.isClosing)
			{
				if (span2[(int)allTag2.tagType].HasValue)
				{
					if (span[num3].HasValue)
					{
						span2[(int)allTag2.tagType] = span[num3];
					}
					else
					{
						span2[(int)allTag2.tagType] = null;
					}
				}
			}
			else
			{
				int? num4 = span2[(int)allTag2.tagType];
				if (num4.HasValue)
				{
					span[num3] = num4;
				}
				span2[(int)allTag2.tagType] = num3;
			}
		}
		Span<bool> span3 = stackalloc bool[allTags.Count];
		for (int i = 0; i < span2.Length; i++)
		{
			int? num5 = span2[i];
			if (num5.HasValue)
			{
				span3[num5.Value] = true;
			}
		}
		int num6 = 0;
		foreach (Tag allTag3 in allTags)
		{
			if (span3[num6])
			{
				applicableTags.Add(allTag3);
			}
			num6++;
		}
		return applicableTags;
	}

	internal static Segment[] GenerateSegments(string input, List<Tag> tags)
	{
		List<Segment> list = new List<Segment>();
		int num = 0;
		for (int i = 0; i < tags.Count; i++)
		{
			Debug.Assert(tags[i].start >= num);
			if (tags[i].start > num)
			{
				list.Add(new Segment
				{
					start = num,
					end = tags[i].start - 1
				});
			}
			num = tags[i].end + 1;
		}
		if (num < input.Length)
		{
			list.Add(new Segment
			{
				start = num,
				end = input.Length - 1
			});
		}
		return list.ToArray();
	}

	internal static void ApplyStateToSegment(string input, List<Tag> tags, Segment[] segments)
	{
		for (int i = 0; i < segments.Length; i++)
		{
			segments[i].tags = PickResultingTags(tags, input, segments[i].start);
		}
	}

	private static int AddLink(TagType type, string value, List<(int, TagType, string)> links)
	{
		foreach (var (result, tagType, text) in links)
		{
			if (type == tagType && value == text)
			{
				return result;
			}
		}
		int count = links.Count;
		links.Add((count, type, value));
		return count;
	}

	private static TextSpan CreateTextSpan(Segment segment, ref NativeTextGenerationSettings tgs, List<(int, TagType, string)> links, Color hyperlinkColor, float pixelsPerPoint)
	{
		TextSpan result = tgs.CreateTextSpan();
		if (segment.tags == null)
		{
			return result;
		}
		for (int i = 0; i < segment.tags.Count; i++)
		{
			switch (segment.tags[i].tagType)
			{
			case TagType.Bold:
				result.fontWeight = TextFontWeight.Bold;
				break;
			case TagType.Italic:
				result.fontStyle |= FontStyles.Italic;
				break;
			case TagType.Underline:
				result.fontStyle |= FontStyles.Underline;
				break;
			case TagType.Strikethrough:
				result.fontStyle |= FontStyles.Strikethrough;
				break;
			case TagType.Subscript:
				result.fontStyle |= FontStyles.Subscript;
				result.subscriptNestingLevel = segment.tags[i].nestingLevel;
				break;
			case TagType.Superscript:
				result.fontStyle |= FontStyles.Superscript;
				result.superscriptNestingLevel = segment.tags[i].nestingLevel;
				break;
			case TagType.AllCaps:
			case TagType.Uppercase:
				result.fontStyle |= FontStyles.UpperCase;
				break;
			case TagType.SmallCaps:
				result.fontStyle |= FontStyles.SmallCaps;
				break;
			case TagType.Lowercase:
				result.fontStyle |= FontStyles.LowerCase;
				break;
			case TagType.Color:
				result.color = segment.tags[i].value.ColorValue;
				break;
			case TagType.Alpha:
				result.color.a = (byte)segment.tags[i].value.NumericalValue;
				break;
			case TagType.Mark:
			{
				result.fontStyle |= FontStyles.Highlight;
				TagValue? value2 = segment.tags[i].value;
				if ((object)value2 != null && value2.ID == ValueID.Color)
				{
					result.highlightColor = segment.tags[i].value.ColorValue;
				}
				else
				{
					result.highlightColor = k_HighlightColor;
				}
				TagValue? value3 = segment.tags[i].value2;
				if ((object)value3 != null && value3.ID == ValueID.Padding)
				{
					result.highlightPadding = segment.tags[i].value2.Vector4Value;
				}
				break;
			}
			case TagType.Style:
				Debug.Assert(condition: false, "Style tags should be handled by the preprocessor.");
				break;
			case TagType.Font:
			{
				string text2 = segment.tags[i].value?.StringValue ?? "";
				if (!string.IsNullOrEmpty(text2) && s_FontAssetCache.TryGetValue(text2, out var value11))
				{
					result.fontAsset = value11;
				}
				break;
			}
			case TagType.FontWeight:
			{
				TagValue? value5 = segment.tags[i].value;
				if ((object)value5 != null && value5.type == TagValueType.NumericalValue)
				{
					result.fontWeight = (TextFontWeight)segment.tags[i].value.NumericalValue;
				}
				break;
			}
			case TagType.Hyperlink:
				result.linkID = AddLink(TagType.Hyperlink, segment.tags[i].value?.StringValue ?? "", links);
				result.color = hyperlinkColor;
				result.fontStyle |= FontStyles.Underline;
				break;
			case TagType.Link:
				result.linkID = AddLink(TagType.Link, segment.tags[i].value?.StringValue ?? "", links);
				break;
			case TagType.Gradient:
			{
				string text = segment.tags[i].value?.StringValue ?? "";
				if (!string.IsNullOrEmpty(text) && s_GradientAssetCache.TryGetValue(text, out var value4))
				{
					result.gradientAsset = value4;
				}
				break;
			}
			case TagType.Sprite:
			{
				TagValue? value6 = segment.tags[i].value;
				if ((object)value6 != null && value6.ID == ValueID.AssetID)
				{
					result.spriteID = EntityId.From((int)segment.tags[i].value.NumericalValue);
				}
				TagValue? value7 = segment.tags[i].value2;
				if ((object)value7 != null && value7.ID == ValueID.GlyphMetrics)
				{
					result.spriteMetrics = segment.tags[i].value2.GlyphMetricsValue;
				}
				TagValue? value8 = segment.tags[i].value3;
				if ((object)value8 != null && value8.ID == ValueID.Tint)
				{
					result.spriteTint = segment.tags[i].value3.BoolValue;
				}
				TagValue? value9 = segment.tags[i].value4;
				if ((object)value9 != null && value9.ID == ValueID.Scale)
				{
					result.spriteScale = (int)segment.tags[i].value4.NumericalValue;
				}
				TagValue? value10 = segment.tags[i].value5;
				if ((object)value10 != null && value10.ID == ValueID.SpriteColor)
				{
					result.spriteColor = segment.tags[i].value5.ColorValue;
				}
				else
				{
					result.spriteColor = Color.white;
				}
				break;
			}
			case TagType.Size:
			{
				float numericalValue = segment.tags[i].value.NumericalValue;
				TagUnitType unit = segment.tags[i].value.unit;
				TagValue? value = segment.tags[i].value2;
				if ((object)value != null && value.BoolValue)
				{
					float num2 = (float)tgs.fontSize / 64f;
					float num3 = numericalValue * pixelsPerPoint;
					float num4 = num2 + num3;
					result.fontSize = (int)Math.Round(num4 * 64f, MidpointRounding.AwayFromZero);
					break;
				}
				if (numericalValue <= 0f)
				{
					result.fontSize = 0;
					break;
				}
				switch (unit)
				{
				case TagUnitType.FontUnits:
				{
					float num7 = (float)tgs.fontSize / 64f;
					float num8 = numericalValue * num7;
					result.fontSize = (int)Math.Round(num8 * 64f, MidpointRounding.AwayFromZero);
					break;
				}
				case TagUnitType.Percentage:
				{
					float num5 = (float)tgs.fontSize / 64f;
					float num6 = numericalValue / 100f * num5;
					result.fontSize = (int)Math.Round(num6 * 64f, MidpointRounding.AwayFromZero);
					break;
				}
				default:
					result.fontSize = (int)Math.Round(numericalValue * pixelsPerPoint * 64f, MidpointRounding.AwayFromZero);
					break;
				}
				break;
			}
			case TagType.CSpace:
			{
				float num13 = ((segment.tags[i].value.unit == TagUnitType.Pixels) ? (pixelsPerPoint * 64f) : 64f);
				result.cspace = (int)(segment.tags[i].value.NumericalValue * num13);
				result.cspaceUnitType = segment.tags[i].value.unit;
				break;
			}
			case TagType.Mspace:
			{
				float num12 = ((segment.tags[i].value.unit == TagUnitType.Pixels) ? (pixelsPerPoint * 64f) : 64f);
				result.mspace = (int)(segment.tags[i].value.NumericalValue * num12);
				result.mspaceUnitType = segment.tags[i].value.unit;
				break;
			}
			case TagType.LineIndent:
			case TagType.Space:
			case TagType.Width:
			case TagType.Rotate:
			case TagType.Pos:
			case TagType.Page:
			case TagType.Action:
			case TagType.Cr:
			case TagType.Zwsp:
			case TagType.Zwj:
			case TagType.Nbsp:
			case TagType.Shy:
				if (s_LoggedUnsupportedTagWarnings.TryAdd(segment.tags[i].tagType, 0))
				{
					Debug.LogWarning("The <noparse><" + TagsInfo[(int)segment.tags[i].tagType].name + "></noparse> rich text tag is not supported in the Advanced Text Generator (ATG). The tag will be stripped from the text.");
				}
				break;
			case TagType.Align:
				Enum.TryParse<HorizontalAlignment>(segment.tags[i].value.StringValue, ignoreCase: true, out result.alignment);
				break;
			case TagType.LineHeight:
			{
				float num11 = ((segment.tags[i].value.unit == TagUnitType.Pixels) ? (pixelsPerPoint * 64f) : 64f);
				result.lineHeight = (int)(segment.tags[i].value.NumericalValue * num11);
				result.lineHeightUnitType = segment.tags[i].value.unit;
				break;
			}
			case TagType.Margin:
			case TagType.MarginLeft:
			case TagType.MarginRight:
			{
				float num10 = ((segment.tags[i].value.unit == TagUnitType.Pixels) ? (pixelsPerPoint * 64f) : 64f);
				result.margin = (int)(segment.tags[i].value.NumericalValue * num10);
				result.marginUnitType = segment.tags[i].value.unit;
				TagType tagType = segment.tags[i].tagType;
				if (1 == 0)
				{
				}
				MarginDirection marginDirection = tagType switch
				{
					TagType.Margin => MarginDirection.Both, 
					TagType.MarginLeft => MarginDirection.Left, 
					TagType.MarginRight => MarginDirection.Right, 
					_ => MarginDirection.Both, 
				};
				if (1 == 0)
				{
				}
				result.marginDirection = marginDirection;
				break;
			}
			case TagType.Indent:
			{
				float num9 = ((segment.tags[i].value.unit == TagUnitType.Pixels) ? (pixelsPerPoint * 64f) : 64f);
				result.indent = (int)(segment.tags[i].value.NumericalValue * num9);
				result.indentUnitType = segment.tags[i].value.unit;
				break;
			}
			case TagType.VOffset:
			{
				float num = ((segment.tags[i].value.unit == TagUnitType.Pixels) ? (pixelsPerPoint * 64f) : 64f);
				result.vOffset = (int)(segment.tags[i].value.NumericalValue * num);
				result.vOffsetUnitType = segment.tags[i].value.unit;
				break;
			}
			case TagType.NoParse:
			case TagType.Unknown:
				throw new InvalidOperationException("Invalid tag type" + segment.tags[i].tagType);
			}
		}
		return result;
	}

	[VisibleToOtherModules(new string[] { "UnityEngine.UIElementsModule" })]
	internal static void CreateTextGenerationSettingsArray(ref NativeTextGenerationSettings tgs, List<(int, TagType, string)> links, Color hyperlinkColor, float pixelsPerPoint, TextSettings textSettings)
	{
		links.Clear();
		List<Tag> tags = FindTags(ref tgs.text, textSettings);
		Segment[] array = GenerateSegments(tgs.text, tags);
		ApplyStateToSegment(tgs.text, tags, array);
		StringBuilder stringBuilder = new StringBuilder(tgs.text.Length);
		tgs.textSpans = new TextSpan[array.Length];
		int num = 0;
		for (int i = 0; i < array.Length; i++)
		{
			Segment segment = array[i];
			string text = tgs.text.Substring(segment.start, segment.end + 1 - segment.start);
			TextSpan textSpan = CreateTextSpan(segment, ref tgs, links, hyperlinkColor, pixelsPerPoint);
			textSpan.startIndex = num;
			textSpan.length = text.Length;
			tgs.textSpans[i] = textSpan;
			stringBuilder.Append(text);
			num += text.Length;
		}
		tgs.text = stringBuilder.ToString();
	}

	[VisibleToOtherModules(new string[] { "UnityEngine.UIElementsModule" })]
	internal static bool MayNeedParsing(string text)
	{
		if (string.IsNullOrEmpty(text))
		{
			return false;
		}
		ReadOnlySpan<char> span = MemoryExtensions.AsSpan(text);
		int num = span.IndexOf('<');
		if (num < 0 || num >= span.Length - 1)
		{
			return false;
		}
		return span.Slice(num + 1).IndexOf('>') >= 0;
	}

	private static bool ContainsFontTag(string text)
	{
		if (string.IsNullOrEmpty(text))
		{
			return false;
		}
		ReadOnlySpan<char> span = MemoryExtensions.AsSpan(text);
		ReadOnlySpan<char> value = MemoryExtensions.AsSpan("<font=");
		int num = span.IndexOf(value, StringComparison.Ordinal);
		if (num < 0)
		{
			return false;
		}
		int num2 = num + value.Length;
		for (int i = num2; i < span.Length; i++)
		{
			if (span[i] == '>')
			{
				return true;
			}
		}
		return false;
	}

	[VisibleToOtherModules(new string[] { "UnityEngine.UIElementsModule" })]
	internal static bool ContainsSpriteTag(string text)
	{
		if (string.IsNullOrEmpty(text))
		{
			return false;
		}
		ReadOnlySpan<char> span = MemoryExtensions.AsSpan(text);
		ReadOnlySpan<char> value = MemoryExtensions.AsSpan("<sprite");
		int num = span.IndexOf(value, StringComparison.Ordinal);
		if (num < 0)
		{
			return false;
		}
		int num2 = num + value.Length;
		for (int i = num2; i < span.Length; i++)
		{
			if (span[i] == '>')
			{
				return true;
			}
		}
		return false;
	}

	internal static bool ContainsStyleTags(string text)
	{
		if (string.IsNullOrEmpty(text))
		{
			return false;
		}
		ReadOnlySpan<char> span = MemoryExtensions.AsSpan(text);
		ReadOnlySpan<char> value = MemoryExtensions.AsSpan("<style=\"");
		int num = span.IndexOf(value, StringComparison.Ordinal);
		if (num < 0)
		{
			return false;
		}
		int num2 = num + value.Length;
		for (int i = num2; i < span.Length; i++)
		{
			if (span[i] == '>')
			{
				return true;
			}
		}
		return false;
	}

	internal static bool ContainsGradientTag(string text)
	{
		if (string.IsNullOrEmpty(text))
		{
			return false;
		}
		ReadOnlySpan<char> span = MemoryExtensions.AsSpan(text);
		ReadOnlySpan<char> value = MemoryExtensions.AsSpan("<gradient");
		int num = span.IndexOf(value, StringComparison.Ordinal);
		if (num < 0)
		{
			return false;
		}
		int num2 = num + value.Length;
		for (int i = num2; i < span.Length; i++)
		{
			if (span[i] == '>')
			{
				return true;
			}
		}
		return false;
	}

	[VisibleToOtherModules(new string[] { "UnityEngine.UIElementsModule" })]
	internal static bool ContainsNobrTags(string text)
	{
		if (string.IsNullOrEmpty(text))
		{
			return false;
		}
		return MemoryExtensions.AsSpan(text).IndexOf(MemoryExtensions.AsSpan("<nobr>"), StringComparison.OrdinalIgnoreCase) >= 0;
	}

	[VisibleToOtherModules(new string[] { "UnityEngine.UIElementsModule", "UnityEngine.IMGUIModule" })]
	internal static bool HasFontTags(string text, TextSettings textSettings, out List<string> fontAssetNames)
	{
		fontAssetNames = new List<string>();
		if (!ContainsFontTag(text))
		{
			return false;
		}
		List<Tag> list = FindTags(ref text, textSettings, preprocessingOnly: true);
		foreach (Tag item in list)
		{
			if (item.tagType == TagType.Font && !item.isClosing && item.value?.StringValue != null)
			{
				string stringValue = item.value.StringValue;
				if (!fontAssetNames.Contains(stringValue))
				{
					fontAssetNames.Add(stringValue);
				}
			}
		}
		return fontAssetNames.Count > 0;
	}

	[VisibleToOtherModules(new string[] { "UnityEngine.UIElementsModule", "UnityEngine.IMGUIModule" })]
	internal static bool HasSpriteTags(string text, TextSettings textSettings, out List<string> spriteAssetNames)
	{
		spriteAssetNames = new List<string>();
		if (!ContainsSpriteTag(text))
		{
			return false;
		}
		List<Tag> list = FindTags(ref text, textSettings, preprocessingOnly: true);
		foreach (Tag item in list)
		{
			if (item.tagType != TagType.Sprite || item.isClosing)
			{
				continue;
			}
			TagValue? value = item.value;
			if ((object)value != null && value.type == TagValueType.StringValue)
			{
				string stringValue = item.value.StringValue;
				if (!string.IsNullOrEmpty(stringValue) && !spriteAssetNames.Contains(stringValue))
				{
					spriteAssetNames.Add(stringValue);
				}
			}
		}
		return spriteAssetNames.Count > 0;
	}

	internal static bool HasGradientTags(string text, TextSettings textSettings, out List<string> gradientAssetNames)
	{
		gradientAssetNames = new List<string>();
		if (!ContainsGradientTag(text))
		{
			return false;
		}
		List<Tag> list = FindTags(ref text, textSettings, preprocessingOnly: true);
		foreach (Tag item in list)
		{
			if (item.tagType != TagType.Gradient || item.isClosing)
			{
				continue;
			}
			TagValue? value = item.value;
			if ((object)value != null && value.type == TagValueType.StringValue)
			{
				string stringValue = item.value.StringValue;
				if (!string.IsNullOrEmpty(stringValue) && !gradientAssetNames.Contains(stringValue))
				{
					gradientAssetNames.Add(stringValue);
				}
			}
		}
		return gradientAssetNames.Count > 0;
	}
}
