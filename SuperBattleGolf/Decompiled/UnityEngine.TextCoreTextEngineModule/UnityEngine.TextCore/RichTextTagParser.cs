#define UNITY_ASSERTIONS
using System;
using System.Collections.Generic;
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
		Italic,
		Indent,
		LineHeight,
		LineIndent,
		Link,
		Lowercase,
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
		Unknown
	}

	internal record TagTypeInfo
	{
		public TagType TagType;

		public string name;

		public TagValueType valueType;

		public TagUnitType unitType;

		internal TagTypeInfo(TagType tagType, string name, TagValueType valueType = TagValueType.None, TagUnitType unitType = TagUnitType.Pixels)
		{
			TagType = tagType;
			this.name = name;
			this.valueType = valueType;
			this.unitType = unitType;
		}
	}

	internal enum TagValueType
	{
		None = 0,
		NumericalValue = 1,
		StringValue = 2,
		ColorValue = 4
	}

	internal enum TagUnitType
	{
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

		internal TagValueType type;

		private string? m_stringValue;

		private float m_numericalValue;

		private Color m_colorValue;

		internal TagValue(float value)
		{
			type = TagValueType.NumericalValue;
			m_numericalValue = value;
		}

		internal TagValue(Color value)
		{
			type = TagValueType.ColorValue;
			m_colorValue = value;
		}

		internal TagValue(string value)
		{
			type = TagValueType.StringValue;
			m_stringValue = value;
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

	internal static readonly TagTypeInfo[] TagsInfo = new TagTypeInfo[30]
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
		new TagTypeInfo(TagType.Italic, "i"),
		new TagTypeInfo(TagType.Indent, "indent"),
		new TagTypeInfo(TagType.LineHeight, "line-height"),
		new TagTypeInfo(TagType.LineIndent, "line-indent"),
		new TagTypeInfo(TagType.Link, "link"),
		new TagTypeInfo(TagType.Lowercase, "lowercase"),
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
		new TagTypeInfo(TagType.Uppercase, "uppercase")
	};

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

	internal static List<Tag> FindTags(string inputStr, List<ParseError>? errors = null)
	{
		char[] array = inputStr.ToCharArray();
		List<Tag> list = new List<Tag>();
		int num = 0;
		while (true)
		{
			int num2 = Array.IndexOf(array, '<', num);
			if (num2 == -1)
			{
				break;
			}
			int num3 = Array.IndexOf(array, '>', num2);
			if (num3 == -1)
			{
				break;
			}
			bool flag = array.Length > num2 + 1 && array[num2 + 1] == '/';
			if (num3 == num2 + 1)
			{
				errors?.Add(new ParseError("Empty tag", num2));
				num = num3 + 1;
				continue;
			}
			num = num3 + 1;
			TagType tagType2;
			string error2;
			ReadOnlySpan<char> attribute2;
			if (!flag)
			{
				Span<char> span = MemoryExtensions.AsSpan(array, num2 + 1, num3 - num2 - 1);
				if (SpanToEnum(span, out TagType tagType, out string error, out ReadOnlySpan<char> attribute))
				{
					TagValue tagValue = null;
					if (tagType == TagType.Color)
					{
						attribute = GetAttributeSpan(attribute);
						ColorUtility.TryParseHtmlString(attribute.ToString(), out var color);
						tagValue = new TagValue(color);
						if ((object)tagValue == null)
						{
							errors?.Add(new ParseError("Invalid color value", num2));
							num = num2 + 1;
							continue;
						}
					}
					if (tagType == TagType.Link || tagType == TagType.Hyperlink)
					{
						if (tagType == TagType.Hyperlink && attribute.StartsWith(" href="))
						{
							attribute = attribute.Slice(" href=".Length);
						}
						attribute = GetAttributeSpan(attribute);
						string value = attribute.ToString();
						tagValue = new TagValue(value);
					}
					if (tagType == TagType.Align)
					{
						attribute = GetAttributeSpan(attribute);
						string value2 = attribute.ToString();
						if (Enum.TryParse<HorizontalAlignment>(value2, ignoreCase: true, out var _))
						{
							tagValue = new TagValue(value2);
						}
						if ((object)tagValue == null)
						{
							errors?.Add(new ParseError($"Invalid {tagType} value", num2));
							num = num2 + 1;
							continue;
						}
					}
					list.Add(new Tag
					{
						tagType = tagType,
						start = num2,
						end = num3,
						isClosing = flag,
						value = tagValue
					});
					if (tagType == TagType.NoParse)
					{
						if ((num2 = MemoryExtensions.AsSpan(array, num).IndexOf("</noparse>")) == -1)
						{
							break;
						}
						num2 += num;
						num3 = num2 + "</noparse>".Length;
						list.Add(new Tag
						{
							tagType = TagType.NoParse,
							start = num2,
							end = num3,
							isClosing = true
						});
						num = num3 + 1;
					}
				}
				else
				{
					if (error != null)
					{
						errors?.Add(new ParseError(error, num2));
					}
					num = num2 + 1;
				}
			}
			else if (SpanToEnum(MemoryExtensions.AsSpan(array, num2 + 2, num3 - num2 - 2), out tagType2, out error2, out attribute2))
			{
				list.Add(new Tag
				{
					tagType = tagType2,
					start = num2,
					end = num3,
					isClosing = flag
				});
			}
			else
			{
				if (error2 != null)
				{
					errors?.Add(new ParseError(error2, num2));
				}
				num = num2 + 1;
			}
		}
		return list;
	}

	private static ReadOnlySpan<char> GetAttributeSpan(ReadOnlySpan<char> atributeSection)
	{
		if (atributeSection.Length >= 2 && atributeSection[0] == '=')
		{
			atributeSection = atributeSection.Slice(1);
		}
		if (atributeSection.Length >= 2)
		{
			if (atributeSection[0] == '"')
			{
				if (atributeSection[atributeSection.Length - 1] == '"')
				{
					goto IL_0082;
				}
			}
			if (atributeSection[0] == '\'')
			{
				if (atributeSection[atributeSection.Length - 1] == '\'')
				{
					goto IL_0082;
				}
			}
		}
		return atributeSection;
		IL_0082:
		return atributeSection.Slice(1, atributeSection.Length - 2);
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
		int num5 = 0;
		foreach (Tag allTag3 in allTags)
		{
			int? num6 = span2[(int)allTag3.tagType];
			if (num6.HasValue && num5 == num6.Value)
			{
				applicableTags.Add(allTag3);
			}
			num5++;
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

	private static TextSpan CreateTextSpan(Segment segment, ref NativeTextGenerationSettings tgs, List<(int, TagType, string)> links, Color hyperlinkColor)
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
				break;
			case TagType.Superscript:
				result.fontStyle |= FontStyles.Superscript;
				break;
			case TagType.AllCaps:
			case TagType.Uppercase:
				result.fontStyle |= FontStyles.UpperCase;
				break;
			case TagType.Lowercase:
			case TagType.SmallCaps:
				result.fontStyle |= FontStyles.LowerCase;
				break;
			case TagType.Color:
				result.color = segment.tags[i].value.ColorValue;
				break;
			case TagType.Mark:
				result.fontStyle |= FontStyles.Highlight;
				break;
			case TagType.Hyperlink:
				result.linkID = AddLink(TagType.Hyperlink, segment.tags[i].value?.StringValue ?? "", links);
				result.color = hyperlinkColor;
				result.fontStyle |= FontStyles.Underline;
				break;
			case TagType.Link:
				result.linkID = AddLink(TagType.Link, segment.tags[i].value?.StringValue ?? "", links);
				break;
			case TagType.Align:
				Enum.TryParse<HorizontalAlignment>(segment.tags[i].value.StringValue, ignoreCase: true, out result.alignment);
				break;
			case TagType.NoParse:
			case TagType.Unknown:
				throw new InvalidOperationException("Invalid tag type" + segment.tags[i].tagType);
			}
		}
		return result;
	}

	[VisibleToOtherModules(new string[] { "UnityEngine.UIElementsModule" })]
	internal static void CreateTextGenerationSettingsArray(ref NativeTextGenerationSettings tgs, List<(int, TagType, string)> links, Color hyperlinkColor)
	{
		links.Clear();
		List<Tag> tags = FindTags(tgs.text);
		Segment[] array = GenerateSegments(tgs.text, tags);
		ApplyStateToSegment(tgs.text, tags, array);
		StringBuilder stringBuilder = new StringBuilder(tgs.text.Length);
		tgs.textSpans = new TextSpan[array.Length];
		int num = 0;
		for (int i = 0; i < array.Length; i++)
		{
			Segment segment = array[i];
			string text = tgs.text.Substring(segment.start, segment.end + 1 - segment.start);
			TextSpan textSpan = CreateTextSpan(segment, ref tgs, links, hyperlinkColor);
			textSpan.startIndex = num;
			textSpan.length = text.Length;
			tgs.textSpans[i] = textSpan;
			stringBuilder.Append(text);
			num += text.Length;
		}
		tgs.text = stringBuilder.ToString();
	}
}
