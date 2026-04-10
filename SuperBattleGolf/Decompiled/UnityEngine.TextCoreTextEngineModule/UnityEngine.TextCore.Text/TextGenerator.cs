#define UNITY_ASSERTIONS
using System;
using System.Collections.Generic;
using Unity.Jobs.LowLevel.Unsafe;
using UnityEngine.Bindings;
using UnityEngine.TextCore.LowLevel;

namespace UnityEngine.TextCore.Text;

[VisibleToOtherModules(new string[] { "UnityEngine.UIElementsModule" })]
internal class TextGenerator
{
	public delegate void MissingCharacterEventCallback(uint unicode, int stringIndex, TextInfo text, FontAsset fontAsset);

	protected struct SpecialCharacter
	{
		public Character character;

		public FontAsset fontAsset;

		public Material material;

		public int materialIndex;

		public SpecialCharacter(Character character, int materialIndex)
		{
			this.character = character;
			fontAsset = character.textAsset as FontAsset;
			material = ((fontAsset != null) ? fontAsset.material : null);
			this.materialIndex = materialIndex;
		}
	}

	private const int k_Tab = 9;

	private const int k_LineFeed = 10;

	private const int k_VerticalTab = 11;

	private const int k_CarriageReturn = 13;

	private const int k_Space = 32;

	private const int k_DoubleQuotes = 34;

	private const int k_NumberSign = 35;

	private const int k_PercentSign = 37;

	private const int k_SingleQuote = 39;

	private const int k_Plus = 43;

	private const int k_Period = 46;

	private const int k_LesserThan = 60;

	private const int k_Equal = 61;

	private const int k_GreaterThan = 62;

	private const int k_Underline = 95;

	private const int k_NoBreakSpace = 160;

	private const int k_SoftHyphen = 173;

	private const int k_HyphenMinus = 45;

	private const int k_FigureSpace = 8199;

	private const int k_Hyphen = 8208;

	private const int k_NonBreakingHyphen = 8209;

	private const int k_ZeroWidthSpace = 8203;

	private const int k_NarrowNoBreakSpace = 8239;

	private const int k_WordJoiner = 8288;

	private const int k_HorizontalEllipsis = 8230;

	private const int k_LineSeparator = 8232;

	private const int k_ParagraphSeparator = 8233;

	private const int k_RightSingleQuote = 8217;

	private const int k_Square = 9633;

	private const int k_HangulJamoStart = 4352;

	private const int k_HangulJamoEnd = 4607;

	private const int k_CjkStart = 11904;

	private const int k_CjkEnd = 40959;

	private const int k_HangulJameExtendedStart = 43360;

	private const int k_HangulJameExtendedEnd = 43391;

	private const int k_HangulSyllablesStart = 44032;

	private const int k_HangulSyllablesEnd = 55295;

	private const int k_CjkIdeographsStart = 63744;

	private const int k_CjkIdeographsEnd = 64255;

	private const int k_CjkFormsStart = 65072;

	private const int k_CjkFormsEnd = 65103;

	private const int k_CjkHalfwidthStart = 65280;

	private const int k_CjkHalfwidthEnd = 65519;

	private const int k_EndOfText = 3;

	private const float k_FloatUnset = -32767f;

	private const int k_MaxCharacters = 8;

	private static TextGenerator s_TextGenerator;

	private TextBackingContainer m_TextBackingArray = new TextBackingContainer(4);

	internal TextProcessingElement[] m_TextProcessingArray = new TextProcessingElement[8];

	internal int m_InternalTextProcessingArraySize;

	[SerializeField]
	protected bool m_VertexBufferAutoSizeReduction = false;

	private char[] m_HtmlTag = new char[256];

	internal HighlightState m_HighlightState = new HighlightState(Color.white, Offset.zero);

	protected bool m_IsIgnoringAlignment;

	protected bool m_IsTextTruncated;

	private Vector3[] m_RectTransformCorners = new Vector3[4];

	private float m_MarginWidth;

	private float m_MarginHeight;

	private float m_PreferredWidth;

	private float m_PreferredHeight;

	private FontAsset m_CurrentFontAsset;

	private Material m_CurrentMaterial;

	private int m_CurrentMaterialIndex;

	private TextProcessingStack<MaterialReference> m_MaterialReferenceStack = new TextProcessingStack<MaterialReference>(new MaterialReference[16]);

	private float m_Padding;

	private SpriteAsset m_CurrentSpriteAsset;

	private int m_TotalCharacterCount;

	private float m_FontSize;

	private float m_FontScaleMultiplier;

	private bool m_ShouldRenderBitmap;

	private float m_CurrentFontSize;

	private TextProcessingStack<float> m_SizeStack = new TextProcessingStack<float>(16);

	protected TextProcessingStack<int>[] m_TextStyleStacks = new TextProcessingStack<int>[8];

	protected int m_TextStyleStackDepth = 0;

	private FontStyles m_FontStyleInternal = FontStyles.Normal;

	private FontStyleStack m_FontStyleStack;

	private TextFontWeight m_FontWeightInternal = TextFontWeight.Regular;

	private TextProcessingStack<TextFontWeight> m_FontWeightStack = new TextProcessingStack<TextFontWeight>(8);

	private TextAlignment m_LineJustification;

	private TextProcessingStack<TextAlignment> m_LineJustificationStack = new TextProcessingStack<TextAlignment>(16);

	private float _m_BaselineOffset;

	private TextProcessingStack<float> m_BaselineOffsetStack = new TextProcessingStack<float>(new float[16]);

	private Color32 m_FontColor32;

	private Color32 m_HtmlColor;

	private Color32 m_UnderlineColor;

	private Color32 m_StrikethroughColor;

	private TextProcessingStack<Color32> m_ColorStack = new TextProcessingStack<Color32>(new Color32[16]);

	private TextProcessingStack<Color32> m_UnderlineColorStack = new TextProcessingStack<Color32>(new Color32[16]);

	private TextProcessingStack<Color32> m_StrikethroughColorStack = new TextProcessingStack<Color32>(new Color32[16]);

	private TextProcessingStack<Color32> m_HighlightColorStack = new TextProcessingStack<Color32>(new Color32[16]);

	private TextProcessingStack<HighlightState> m_HighlightStateStack = new TextProcessingStack<HighlightState>(new HighlightState[16]);

	private TextProcessingStack<int> m_ItalicAngleStack = new TextProcessingStack<int>(new int[16]);

	private TextColorGradient m_ColorGradientPreset;

	private TextProcessingStack<TextColorGradient> m_ColorGradientStack = new TextProcessingStack<TextColorGradient>(new TextColorGradient[16]);

	private bool m_ColorGradientPresetIsTinted;

	private TextProcessingStack<int> m_ActionStack = new TextProcessingStack<int>(new int[16]);

	private float _m_LineOffset;

	private float _m_LineHeight;

	private bool m_IsDrivenLineSpacing;

	private float m_CSpacing;

	private float m_MonoSpacing;

	private bool m_DuoSpace;

	private float _m_XAdvance;

	private float m_TagLineIndent;

	private float m_TagIndent;

	private TextProcessingStack<float> m_IndentStack = new TextProcessingStack<float>(new float[16]);

	private bool m_TagNoParsing;

	private int m_CharacterCount;

	private int m_FirstCharacterOfLine;

	private int m_LastCharacterOfLine;

	private int m_FirstVisibleCharacterOfLine;

	private int m_LastVisibleCharacterOfLine;

	private float m_MaxLineAscender;

	private float m_MaxLineDescender;

	private int m_LineNumber;

	private int m_LineVisibleCharacterCount;

	private int m_LineVisibleSpaceCount;

	private int m_FirstOverflowCharacterIndex;

	private float m_MarginLeft;

	private float m_MarginRight;

	private float m_Width;

	private Extents m_MeshExtents;

	private float m_MaxCapHeight;

	private float m_MaxAscender;

	private float m_MaxDescender;

	private bool m_IsNonBreakingSpace;

	private WordWrapState m_SavedWordWrapState;

	private WordWrapState m_SavedLineState;

	private WordWrapState m_SavedEllipsisState = default(WordWrapState);

	private WordWrapState m_SavedLastValidState = default(WordWrapState);

	private WordWrapState m_SavedSoftLineBreakState = default(WordWrapState);

	private TextElementType m_TextElementType;

	private bool m_isTextLayoutPhase;

	private int m_SpriteIndex;

	private Color32 m_SpriteColor;

	private TextElement m_CachedTextElement;

	private Color32 m_HighlightColor;

	private float m_CharWidthAdjDelta;

	private float m_MaxFontSize;

	private float m_MinFontSize;

	private int m_AutoSizeIterationCount;

	private int m_AutoSizeMaxIterationCount = 100;

	private float m_StartOfLineAscender;

	private float m_LineSpacingDelta;

	internal MaterialReference[] m_MaterialReferences = new MaterialReference[8];

	private int m_SpriteCount = 0;

	private TextProcessingStack<int> m_StyleStack = new TextProcessingStack<int>(new int[16]);

	private TextProcessingStack<WordWrapState> m_EllipsisInsertionCandidateStack = new TextProcessingStack<WordWrapState>(8, 8);

	private int m_SpriteAnimationId;

	private int m_ItalicAngle;

	private Vector3 m_FXScale;

	private Quaternion m_FXRotation;

	private int m_LastBaseGlyphIndex;

	private float m_PageAscender;

	private RichTextTagAttribute[] m_XmlAttribute = new RichTextTagAttribute[8];

	private float[] m_AttributeParameterValues = new float[16];

	private Dictionary<int, int> m_MaterialReferenceIndexLookup = new Dictionary<int, int>();

	private bool m_IsCalculatingPreferredValues;

	private bool m_TintSprite;

	protected SpecialCharacter m_Ellipsis;

	protected SpecialCharacter m_Underline;

	private TextElementInfo[] m_InternalTextElementInfo;

	internal static readonly bool EnableTextAlignmentAssertions;

	internal static readonly bool EnableCheckerboardPattern;

	[VisibleToOtherModules(new string[] { "UnityEngine.UIElementsModule" })]
	internal static bool IsExecutingJob { get; set; }

	private bool vertexBufferAutoSizeReduction
	{
		get
		{
			return m_VertexBufferAutoSizeReduction;
		}
		set
		{
			m_VertexBufferAutoSizeReduction = value;
		}
	}

	public bool isTextTruncated => m_IsTextTruncated;

	private float m_BaselineOffset
	{
		get
		{
			return _m_BaselineOffset;
		}
		set
		{
			_m_BaselineOffset = Round(value);
		}
	}

	private float m_LineOffset
	{
		get
		{
			return _m_LineOffset;
		}
		set
		{
			_m_LineOffset = Round(value);
		}
	}

	private float m_LineHeight
	{
		get
		{
			return _m_LineHeight;
		}
		set
		{
			_m_LineHeight = Round(value);
		}
	}

	private float m_XAdvance
	{
		get
		{
			return _m_XAdvance;
		}
		set
		{
			float num = Round(value);
			_m_XAdvance = num;
		}
	}

	private bool NeedToRound => m_ShouldRenderBitmap;

	public static event MissingCharacterEventCallback OnMissingCharacter;

	[VisibleToOtherModules(new string[] { "UnityEngine.UIElementsModule" })]
	internal static TextGenerator GetTextGenerator()
	{
		if (s_TextGenerator == null)
		{
			s_TextGenerator = new TextGenerator();
		}
		return s_TextGenerator;
	}

	public void GenerateText(TextGenerationSettings settings, TextInfo textInfo)
	{
		bool flag = !IsExecutingJob;
		if (settings.fontAsset == null)
		{
			Debug.LogWarning("Can't Generate Mesh, No Font Asset has been assigned.");
			return;
		}
		if (textInfo == null)
		{
			Debug.LogError("Null TextInfo provided to TextGenerator. Cannot update its content.");
			return;
		}
		Prepare(settings, textInfo);
		if (flag)
		{
			FontAsset.UpdateFontAssetsInUpdateQueue();
		}
		GenerateTextMesh(settings, textInfo);
	}

	[VisibleToOtherModules(new string[] { "UnityEngine.UIElementsModule" })]
	internal void GenerateTextMesh(TextGenerationSettings generationSettings, TextInfo textInfo)
	{
		if (generationSettings.fontAsset == null)
		{
			Debug.LogWarning("Can't Generate Mesh! No Font Asset has been assigned.");
			return;
		}
		textInfo?.Clear();
		if (generationSettings.fontSize <= 0 || m_TextProcessingArray == null || m_TextProcessingArray.Length == 0 || m_TextProcessingArray[0].unicode == 0)
		{
			ClearMesh(updateMesh: true, textInfo);
			m_PreferredWidth = 0f;
			m_PreferredHeight = 0f;
			return;
		}
		float num = 0f;
		ParsingPhase(textInfo, generationSettings, out var charCode, out var maxVisibleDescender);
		num = m_MaxFontSize - m_MinFontSize;
		bool flag = false;
		if (m_AutoSizeIterationCount >= m_AutoSizeMaxIterationCount)
		{
			Debug.Log("Auto Size Iteration Count: " + m_AutoSizeIterationCount + ". Final Point Size: " + m_FontSize);
		}
		if (m_CharacterCount == 0 || (m_CharacterCount == 1 && charCode == 3))
		{
			ClearMesh(updateMesh: true, textInfo);
			return;
		}
		if (NeedToRound && EnableTextAlignmentAssertions)
		{
			Debug.AssertFormat((double)Mathf.Abs(generationSettings.screenRect.x - Round(generationSettings.screenRect.x)) < 0.01, "Bitmap Rendering specified and screenRect.x is not rounded:{0}", generationSettings.screenRect.x);
			Debug.AssertFormat((double)Mathf.Abs(generationSettings.screenRect.y - Round(generationSettings.screenRect.y)) < 0.01, "Bitmap Rendering specified and screenRect.y is not rounded:{0}", generationSettings.screenRect.y);
			Debug.AssertFormat((double)Mathf.Abs(generationSettings.screenRect.width - Round(generationSettings.screenRect.width)) < 0.01, "Bitmap Rendering specified and screenRect.width is not rounded:{0}", generationSettings.screenRect.width);
			Debug.AssertFormat((double)Mathf.Abs(generationSettings.screenRect.height - Round(generationSettings.screenRect.height)) < 0.01, "Bitmap Rendering specified and screenRect.height is not rounded:{0}", generationSettings.screenRect.height);
		}
		LayoutPhase(textInfo, generationSettings, maxVisibleDescender);
		for (int i = 1; i < textInfo.materialCount; i++)
		{
			textInfo.meshInfo[i].ClearUnusedVertices();
		}
	}

	private bool ValidateHtmlTag(TextProcessingElement[] chars, int startIndex, out int endIndex, TextGenerationSettings generationSettings, TextInfo textInfo, out bool isThreadSuccess)
	{
		bool flag = !IsExecutingJob;
		isThreadSuccess = true;
		TextSettings textSettings = generationSettings.textSettings;
		int num = 0;
		byte b = 0;
		int num2 = 0;
		ClearMarkupTagAttributes();
		TagValueType tagValueType = TagValueType.None;
		TagUnitType tagUnitType = TagUnitType.Pixels;
		endIndex = startIndex;
		bool flag2 = false;
		bool flag3 = false;
		bool flag4 = false;
		bool flag5 = false;
		for (int i = startIndex; i < chars.Length && chars[i].unicode != 0; i++)
		{
			if (num >= m_HtmlTag.Length)
			{
				break;
			}
			if (chars[i].unicode == 60)
			{
				break;
			}
			uint unicode = chars[i].unicode;
			if (unicode == 62)
			{
				flag3 = true;
				endIndex = i;
				m_HtmlTag[num] = '\0';
				break;
			}
			m_HtmlTag[num] = (char)unicode;
			num++;
			if (b == 1)
			{
				switch (tagValueType)
				{
				case TagValueType.None:
					if (unicode == 43 || unicode == 45 || unicode == 46 || (unicode >= 48 && unicode <= 57))
					{
						tagUnitType = TagUnitType.Pixels;
						tagValueType = (m_XmlAttribute[num2].valueType = TagValueType.NumericalValue);
						m_XmlAttribute[num2].valueStartIndex = num - 1;
						m_XmlAttribute[num2].valueLength++;
						break;
					}
					switch (unicode)
					{
					case 35u:
						tagUnitType = TagUnitType.Pixels;
						tagValueType = (m_XmlAttribute[num2].valueType = TagValueType.ColorValue);
						m_XmlAttribute[num2].valueStartIndex = num - 1;
						m_XmlAttribute[num2].valueLength++;
						break;
					case 39u:
						tagUnitType = TagUnitType.Pixels;
						tagValueType = (m_XmlAttribute[num2].valueType = TagValueType.StringValue);
						m_XmlAttribute[num2].valueStartIndex = num;
						flag4 = true;
						break;
					case 34u:
						tagUnitType = TagUnitType.Pixels;
						tagValueType = (m_XmlAttribute[num2].valueType = TagValueType.StringValue);
						m_XmlAttribute[num2].valueStartIndex = num;
						flag5 = true;
						break;
					default:
						tagUnitType = TagUnitType.Pixels;
						tagValueType = (m_XmlAttribute[num2].valueType = TagValueType.StringValue);
						m_XmlAttribute[num2].valueStartIndex = num - 1;
						m_XmlAttribute[num2].valueHashCode = ((m_XmlAttribute[num2].valueHashCode << 5) + m_XmlAttribute[num2].valueHashCode) ^ TextGeneratorUtilities.ToUpperFast((char)unicode);
						m_XmlAttribute[num2].valueLength++;
						break;
					}
					break;
				case TagValueType.NumericalValue:
					if (unicode == 112 || unicode == 101 || unicode == 37 || unicode == 32)
					{
						b = 2;
						tagValueType = TagValueType.None;
						tagUnitType = unicode switch
						{
							101u => m_XmlAttribute[num2].unitType = TagUnitType.FontUnits, 
							37u => m_XmlAttribute[num2].unitType = TagUnitType.Percentage, 
							_ => m_XmlAttribute[num2].unitType = TagUnitType.Pixels, 
						};
						num2++;
						m_XmlAttribute[num2].nameHashCode = 0;
						m_XmlAttribute[num2].valueHashCode = 0;
						m_XmlAttribute[num2].valueType = TagValueType.None;
						m_XmlAttribute[num2].unitType = TagUnitType.Pixels;
						m_XmlAttribute[num2].valueStartIndex = 0;
						m_XmlAttribute[num2].valueLength = 0;
					}
					else
					{
						m_XmlAttribute[num2].valueLength++;
					}
					break;
				case TagValueType.ColorValue:
					if (unicode != 32)
					{
						m_XmlAttribute[num2].valueLength++;
						break;
					}
					b = 2;
					tagValueType = TagValueType.None;
					tagUnitType = TagUnitType.Pixels;
					num2++;
					m_XmlAttribute[num2].nameHashCode = 0;
					m_XmlAttribute[num2].valueType = TagValueType.None;
					m_XmlAttribute[num2].unitType = TagUnitType.Pixels;
					m_XmlAttribute[num2].valueHashCode = 0;
					m_XmlAttribute[num2].valueStartIndex = 0;
					m_XmlAttribute[num2].valueLength = 0;
					break;
				case TagValueType.StringValue:
					if ((!flag5 || unicode != 34) && (!flag4 || unicode != 39))
					{
						m_XmlAttribute[num2].valueHashCode = ((m_XmlAttribute[num2].valueHashCode << 5) + m_XmlAttribute[num2].valueHashCode) ^ TextGeneratorUtilities.ToUpperFast((char)unicode);
						m_XmlAttribute[num2].valueLength++;
						break;
					}
					b = 2;
					tagValueType = TagValueType.None;
					tagUnitType = TagUnitType.Pixels;
					num2++;
					if (m_XmlAttribute.Length <= num2)
					{
						int newSize = Mathf.NextPowerOfTwo(num2 + 1);
						Array.Resize(ref m_XmlAttribute, newSize);
					}
					m_XmlAttribute[num2].nameHashCode = 0;
					m_XmlAttribute[num2].valueType = TagValueType.None;
					m_XmlAttribute[num2].unitType = TagUnitType.Pixels;
					m_XmlAttribute[num2].valueHashCode = 0;
					m_XmlAttribute[num2].valueStartIndex = 0;
					m_XmlAttribute[num2].valueLength = 0;
					break;
				}
			}
			if (unicode == 61)
			{
				b = 1;
			}
			if (b == 0 && unicode == 32)
			{
				if (flag2)
				{
					return false;
				}
				flag2 = true;
				b = 2;
				tagValueType = TagValueType.None;
				tagUnitType = TagUnitType.Pixels;
				num2++;
				m_XmlAttribute[num2].nameHashCode = 0;
				m_XmlAttribute[num2].valueType = TagValueType.None;
				m_XmlAttribute[num2].unitType = TagUnitType.Pixels;
				m_XmlAttribute[num2].valueHashCode = 0;
				m_XmlAttribute[num2].valueStartIndex = 0;
				m_XmlAttribute[num2].valueLength = 0;
			}
			if (b == 0)
			{
				m_XmlAttribute[num2].nameHashCode = ((m_XmlAttribute[num2].nameHashCode << 5) + m_XmlAttribute[num2].nameHashCode) ^ TextGeneratorUtilities.ToUpperFast((char)unicode);
			}
			if (b == 2 && unicode == 32)
			{
				b = 0;
			}
		}
		if (!flag3)
		{
			return false;
		}
		if (m_TagNoParsing && m_XmlAttribute[0].nameHashCode != -294095813)
		{
			return false;
		}
		if (m_XmlAttribute[0].nameHashCode == -294095813)
		{
			m_TagNoParsing = false;
			return true;
		}
		if (m_HtmlTag[0] == '#' && num == 4)
		{
			m_HtmlColor = TextGeneratorUtilities.HexCharsToColor(m_HtmlTag, 0, num);
			m_ColorStack.Add(m_HtmlColor);
			return true;
		}
		if (m_HtmlTag[0] == '#' && num == 5)
		{
			m_HtmlColor = TextGeneratorUtilities.HexCharsToColor(m_HtmlTag, 0, num);
			m_ColorStack.Add(m_HtmlColor);
			return true;
		}
		if (m_HtmlTag[0] == '#' && num == 7)
		{
			m_HtmlColor = TextGeneratorUtilities.HexCharsToColor(m_HtmlTag, 0, num);
			m_ColorStack.Add(m_HtmlColor);
			return true;
		}
		if (m_HtmlTag[0] == '#' && num == 9)
		{
			m_HtmlColor = TextGeneratorUtilities.HexCharsToColor(m_HtmlTag, 0, num);
			m_ColorStack.Add(m_HtmlColor);
			return true;
		}
		float num3 = 0f;
		Material material;
		switch ((MarkupTag)m_XmlAttribute[0].nameHashCode)
		{
		case MarkupTag.BOLD:
			m_FontStyleInternal |= FontStyles.Bold;
			m_FontStyleStack.Add(FontStyles.Bold);
			m_FontWeightInternal = TextFontWeight.Bold;
			return true;
		case MarkupTag.SLASH_BOLD:
			if ((generationSettings.fontStyle & FontStyles.Bold) != FontStyles.Bold && m_FontStyleStack.Remove(FontStyles.Bold) == 0)
			{
				m_FontStyleInternal &= ~FontStyles.Bold;
				m_FontWeightInternal = m_FontWeightStack.Peek();
			}
			return true;
		case MarkupTag.ITALIC:
			m_FontStyleInternal |= FontStyles.Italic;
			m_FontStyleStack.Add(FontStyles.Italic);
			if (m_XmlAttribute[1].nameHashCode == 75347905)
			{
				m_ItalicAngle = (int)TextGeneratorUtilities.ConvertToFloat(m_HtmlTag, m_XmlAttribute[1].valueStartIndex, m_XmlAttribute[1].valueLength);
				if (m_ItalicAngle < -180 || m_ItalicAngle > 180)
				{
					return false;
				}
			}
			else
			{
				m_ItalicAngle = m_CurrentFontAsset.italicStyleSlant;
			}
			m_ItalicAngleStack.Add(m_ItalicAngle);
			return true;
		case MarkupTag.SLASH_ITALIC:
			if ((generationSettings.fontStyle & FontStyles.Italic) != FontStyles.Italic)
			{
				m_ItalicAngle = m_ItalicAngleStack.Remove();
				if (m_FontStyleStack.Remove(FontStyles.Italic) == 0)
				{
					m_FontStyleInternal &= ~FontStyles.Italic;
				}
			}
			return true;
		case MarkupTag.STRIKETHROUGH:
			m_FontStyleInternal |= FontStyles.Strikethrough;
			m_FontStyleStack.Add(FontStyles.Strikethrough);
			if (m_XmlAttribute[1].nameHashCode == 81999901)
			{
				m_StrikethroughColor = TextGeneratorUtilities.HexCharsToColor(m_HtmlTag, m_XmlAttribute[1].valueStartIndex, m_XmlAttribute[1].valueLength);
				m_StrikethroughColor.a = ((m_HtmlColor.a < m_StrikethroughColor.a) ? m_HtmlColor.a : m_StrikethroughColor.a);
				if (textInfo != null)
				{
					textInfo.hasMultipleColors = true;
				}
			}
			else
			{
				m_StrikethroughColor = m_HtmlColor;
			}
			m_StrikethroughColorStack.Add(m_StrikethroughColor);
			return true;
		case MarkupTag.SLASH_STRIKETHROUGH:
			if ((generationSettings.fontStyle & FontStyles.Strikethrough) != FontStyles.Strikethrough && m_FontStyleStack.Remove(FontStyles.Strikethrough) == 0)
			{
				m_FontStyleInternal &= ~FontStyles.Strikethrough;
			}
			m_StrikethroughColor = m_StrikethroughColorStack.Remove();
			return true;
		case MarkupTag.UNDERLINE:
			m_FontStyleInternal |= FontStyles.Underline;
			m_FontStyleStack.Add(FontStyles.Underline);
			if (m_XmlAttribute[1].nameHashCode == 81999901)
			{
				m_UnderlineColor = TextGeneratorUtilities.HexCharsToColor(m_HtmlTag, m_XmlAttribute[1].valueStartIndex, m_XmlAttribute[1].valueLength);
				m_UnderlineColor.a = ((m_HtmlColor.a < m_UnderlineColor.a) ? m_HtmlColor.a : m_UnderlineColor.a);
				if (textInfo != null)
				{
					textInfo.hasMultipleColors = true;
				}
			}
			else
			{
				m_UnderlineColor = m_HtmlColor;
			}
			m_UnderlineColorStack.Add(m_UnderlineColor);
			return true;
		case MarkupTag.SLASH_UNDERLINE:
			if ((generationSettings.fontStyle & FontStyles.Underline) != FontStyles.Underline && m_FontStyleStack.Remove(FontStyles.Underline) == 0)
			{
				m_FontStyleInternal &= ~FontStyles.Underline;
			}
			m_UnderlineColor = m_UnderlineColorStack.Remove();
			return true;
		case MarkupTag.MARK:
		{
			m_FontStyleInternal |= FontStyles.Highlight;
			m_FontStyleStack.Add(FontStyles.Highlight);
			Color32 color = new Color32(byte.MaxValue, byte.MaxValue, 0, 64);
			Offset padding = Offset.zero;
			for (int m = 0; m < m_XmlAttribute.Length && m_XmlAttribute[m].nameHashCode != 0; m++)
			{
				switch ((MarkupTag)m_XmlAttribute[m].nameHashCode)
				{
				case MarkupTag.MARK:
					if (m_XmlAttribute[m].valueType == TagValueType.ColorValue)
					{
						color = TextGeneratorUtilities.HexCharsToColor(m_HtmlTag, m_XmlAttribute[0].valueStartIndex, m_XmlAttribute[0].valueLength);
					}
					break;
				case MarkupTag.COLOR:
					color = TextGeneratorUtilities.HexCharsToColor(m_HtmlTag, m_XmlAttribute[m].valueStartIndex, m_XmlAttribute[m].valueLength);
					break;
				case MarkupTag.PADDING:
				{
					int attributeParameters2 = TextGeneratorUtilities.GetAttributeParameters(m_HtmlTag, m_XmlAttribute[m].valueStartIndex, m_XmlAttribute[m].valueLength, ref m_AttributeParameterValues);
					if (attributeParameters2 != 4)
					{
						return false;
					}
					padding = new Offset(m_AttributeParameterValues[0], m_AttributeParameterValues[1], m_AttributeParameterValues[2], m_AttributeParameterValues[3]);
					padding *= m_FontSize * 0.01f;
					break;
				}
				}
			}
			color.a = ((m_HtmlColor.a < color.a) ? m_HtmlColor.a : color.a);
			m_HighlightState = new HighlightState(color, padding);
			m_HighlightStateStack.Push(m_HighlightState);
			if (textInfo != null)
			{
				textInfo.hasMultipleColors = true;
			}
			return true;
		}
		case MarkupTag.SLASH_MARK:
			if ((generationSettings.fontStyle & FontStyles.Highlight) != FontStyles.Highlight)
			{
				m_HighlightStateStack.Remove();
				m_HighlightState = m_HighlightStateStack.current;
				if (m_FontStyleStack.Remove(FontStyles.Highlight) == 0)
				{
					m_FontStyleInternal &= ~FontStyles.Highlight;
				}
			}
			return true;
		case MarkupTag.SUBSCRIPT:
		{
			m_FontScaleMultiplier *= ((m_CurrentFontAsset.faceInfo.subscriptSize > 0f) ? m_CurrentFontAsset.faceInfo.subscriptSize : 1f);
			m_BaselineOffsetStack.Push(m_BaselineOffset);
			m_MaterialReferenceStack.Push(m_MaterialReferences[m_CurrentMaterialIndex]);
			float num7 = m_CurrentFontSize / m_CurrentFontAsset.faceInfo.pointSize * m_CurrentFontAsset.faceInfo.scale;
			m_BaselineOffset += m_CurrentFontAsset.faceInfo.subscriptOffset * num7 * m_FontScaleMultiplier;
			m_FontStyleStack.Add(FontStyles.Subscript);
			m_FontStyleInternal |= FontStyles.Subscript;
			return true;
		}
		case MarkupTag.SLASH_SUBSCRIPT:
			if ((m_FontStyleInternal & FontStyles.Subscript) == FontStyles.Subscript)
			{
				FontAsset fontAsset3 = m_MaterialReferenceStack.Pop().fontAsset;
				if (m_FontScaleMultiplier < 1f)
				{
					m_BaselineOffset = m_BaselineOffsetStack.Pop();
					m_FontScaleMultiplier /= ((fontAsset3.faceInfo.subscriptSize > 0f) ? fontAsset3.faceInfo.subscriptSize : 1f);
				}
				if (m_FontStyleStack.Remove(FontStyles.Subscript) == 0)
				{
					m_FontStyleInternal &= ~FontStyles.Subscript;
				}
			}
			return true;
		case MarkupTag.SUPERSCRIPT:
		{
			m_FontScaleMultiplier *= ((m_CurrentFontAsset.faceInfo.superscriptSize > 0f) ? m_CurrentFontAsset.faceInfo.superscriptSize : 1f);
			m_BaselineOffsetStack.Push(m_BaselineOffset);
			m_MaterialReferenceStack.Push(m_MaterialReferences[m_CurrentMaterialIndex]);
			float num7 = m_CurrentFontSize / m_CurrentFontAsset.faceInfo.pointSize * m_CurrentFontAsset.faceInfo.scale;
			m_BaselineOffset += m_CurrentFontAsset.faceInfo.superscriptOffset * num7 * m_FontScaleMultiplier;
			m_FontStyleStack.Add(FontStyles.Superscript);
			m_FontStyleInternal |= FontStyles.Superscript;
			return true;
		}
		case MarkupTag.SLASH_SUPERSCRIPT:
			if ((m_FontStyleInternal & FontStyles.Superscript) == FontStyles.Superscript)
			{
				FontAsset fontAsset = m_MaterialReferenceStack.Pop().fontAsset;
				if (m_FontScaleMultiplier < 1f)
				{
					m_BaselineOffset = m_BaselineOffsetStack.Pop();
					m_FontScaleMultiplier /= ((fontAsset.faceInfo.superscriptSize > 0f) ? fontAsset.faceInfo.superscriptSize : 1f);
				}
				if (m_FontStyleStack.Remove(FontStyles.Superscript) == 0)
				{
					m_FontStyleInternal &= ~FontStyles.Superscript;
				}
			}
			return true;
		case MarkupTag.FONT_WEIGHT:
			num3 = TextGeneratorUtilities.ConvertToFloat(m_HtmlTag, m_XmlAttribute[0].valueStartIndex, m_XmlAttribute[0].valueLength);
			if (num3 == -32767f)
			{
				return false;
			}
			switch ((int)num3)
			{
			case 100:
				m_FontWeightInternal = TextFontWeight.Thin;
				break;
			case 200:
				m_FontWeightInternal = TextFontWeight.ExtraLight;
				break;
			case 300:
				m_FontWeightInternal = TextFontWeight.Light;
				break;
			case 400:
				m_FontWeightInternal = TextFontWeight.Regular;
				break;
			case 500:
				m_FontWeightInternal = TextFontWeight.Medium;
				break;
			case 600:
				m_FontWeightInternal = TextFontWeight.SemiBold;
				break;
			case 700:
				m_FontWeightInternal = TextFontWeight.Bold;
				break;
			case 800:
				m_FontWeightInternal = TextFontWeight.Heavy;
				break;
			case 900:
				m_FontWeightInternal = TextFontWeight.Black;
				break;
			}
			m_FontWeightStack.Add(m_FontWeightInternal);
			return true;
		case MarkupTag.SLASH_FONT_WEIGHT:
			m_FontWeightStack.Remove();
			if (m_FontStyleInternal == FontStyles.Bold)
			{
				m_FontWeightInternal = TextFontWeight.Bold;
			}
			else
			{
				m_FontWeightInternal = m_FontWeightStack.Peek();
			}
			return true;
		case MarkupTag.POSITION:
			num3 = TextGeneratorUtilities.ConvertToFloat(m_HtmlTag, m_XmlAttribute[0].valueStartIndex, m_XmlAttribute[0].valueLength);
			if (num3 == -32767f)
			{
				return false;
			}
			switch (tagUnitType)
			{
			case TagUnitType.Pixels:
				m_XAdvance = num3;
				return true;
			case TagUnitType.FontUnits:
				m_XAdvance = num3 * m_CurrentFontSize;
				return true;
			case TagUnitType.Percentage:
				m_XAdvance = m_MarginWidth * num3 / 100f;
				return true;
			default:
				return false;
			}
		case MarkupTag.SLASH_POSITION:
			m_IsIgnoringAlignment = false;
			return true;
		case MarkupTag.VERTICAL_OFFSET:
			num3 = TextGeneratorUtilities.ConvertToFloat(m_HtmlTag, m_XmlAttribute[0].valueStartIndex, m_XmlAttribute[0].valueLength);
			if (num3 == -32767f)
			{
				return false;
			}
			switch (tagUnitType)
			{
			case TagUnitType.Pixels:
				m_BaselineOffset = num3 * generationSettings.pixelsPerPoint;
				return true;
			case TagUnitType.FontUnits:
				m_BaselineOffset = num3 * m_CurrentFontSize;
				return true;
			case TagUnitType.Percentage:
				return false;
			default:
				return false;
			}
		case MarkupTag.SLASH_VERTICAL_OFFSET:
			m_BaselineOffset = 0f;
			return true;
		case MarkupTag.PAGE:
			return true;
		case MarkupTag.NO_BREAK:
			m_IsNonBreakingSpace = true;
			return true;
		case MarkupTag.SLASH_NO_BREAK:
			m_IsNonBreakingSpace = false;
			return true;
		case MarkupTag.SIZE:
			num3 = TextGeneratorUtilities.ConvertToFloat(m_HtmlTag, m_XmlAttribute[0].valueStartIndex, m_XmlAttribute[0].valueLength);
			if (num3 == -32767f)
			{
				return false;
			}
			switch (tagUnitType)
			{
			case TagUnitType.Pixels:
				if (m_HtmlTag[5] == '+')
				{
					m_CurrentFontSize = m_FontSize + num3 * generationSettings.pixelsPerPoint;
					m_SizeStack.Add(m_CurrentFontSize);
					return true;
				}
				if (m_HtmlTag[5] == '-')
				{
					m_CurrentFontSize = m_FontSize + num3 * generationSettings.pixelsPerPoint;
					m_SizeStack.Add(m_CurrentFontSize);
					return true;
				}
				m_CurrentFontSize = num3 * generationSettings.pixelsPerPoint;
				m_SizeStack.Add(m_CurrentFontSize);
				return true;
			case TagUnitType.FontUnits:
				m_CurrentFontSize = m_FontSize * num3;
				m_SizeStack.Add(m_CurrentFontSize);
				return true;
			case TagUnitType.Percentage:
				m_CurrentFontSize = m_FontSize * num3 / 100f;
				m_SizeStack.Add(m_CurrentFontSize);
				return true;
			default:
				return false;
			}
		case MarkupTag.SLASH_SIZE:
			m_CurrentFontSize = m_SizeStack.Remove();
			return true;
		case MarkupTag.FONT:
		{
			int valueHashCode3 = m_XmlAttribute[0].valueHashCode;
			int nameHashCode = m_XmlAttribute[1].nameHashCode;
			int valueHashCode = m_XmlAttribute[1].valueHashCode;
			if (valueHashCode3 == -620974005)
			{
				m_CurrentFontAsset = m_MaterialReferences[0].fontAsset;
				m_CurrentMaterial = m_MaterialReferences[0].material;
				m_CurrentMaterialIndex = 0;
				m_MaterialReferenceStack.Add(m_MaterialReferences[0]);
				return true;
			}
			MaterialReferenceManager.TryGetFontAsset(valueHashCode3, out var fontAsset2);
			if (fontAsset2 == null)
			{
				if (fontAsset2 == null)
				{
					if (!flag)
					{
						isThreadSuccess = false;
						return false;
					}
					fontAsset2 = Resources.Load<FontAsset>(textSettings.defaultFontAssetPath + new string(m_HtmlTag, m_XmlAttribute[0].valueStartIndex, m_XmlAttribute[0].valueLength));
				}
				if (fontAsset2 == null)
				{
					return false;
				}
				MaterialReferenceManager.AddFontAsset(fontAsset2);
			}
			if (nameHashCode == 0 && valueHashCode == 0)
			{
				m_CurrentMaterial = fontAsset2.material;
				m_CurrentMaterialIndex = MaterialReference.AddMaterialReference(m_CurrentMaterial, fontAsset2, ref m_MaterialReferences, m_MaterialReferenceIndexLookup);
				m_MaterialReferenceStack.Add(m_MaterialReferences[m_CurrentMaterialIndex]);
			}
			else
			{
				if (nameHashCode != 825491659)
				{
					return false;
				}
				if (MaterialReferenceManager.TryGetMaterial(valueHashCode, out material))
				{
					m_CurrentMaterial = material;
					m_CurrentMaterialIndex = MaterialReference.AddMaterialReference(m_CurrentMaterial, fontAsset2, ref m_MaterialReferences, m_MaterialReferenceIndexLookup);
					m_MaterialReferenceStack.Add(m_MaterialReferences[m_CurrentMaterialIndex]);
				}
				else
				{
					if (!flag)
					{
						isThreadSuccess = false;
						return false;
					}
					material = Resources.Load<Material>(textSettings.defaultFontAssetPath + new string(m_HtmlTag, m_XmlAttribute[1].valueStartIndex, m_XmlAttribute[1].valueLength));
					if (material == null)
					{
						return false;
					}
					MaterialReferenceManager.AddFontMaterial(valueHashCode, material);
					m_CurrentMaterial = material;
					m_CurrentMaterialIndex = MaterialReference.AddMaterialReference(m_CurrentMaterial, fontAsset2, ref m_MaterialReferences, m_MaterialReferenceIndexLookup);
					m_MaterialReferenceStack.Add(m_MaterialReferences[m_CurrentMaterialIndex]);
				}
			}
			m_CurrentFontAsset = fontAsset2;
			return true;
		}
		case MarkupTag.SLASH_FONT:
		{
			MaterialReference materialReference2 = m_MaterialReferenceStack.Remove();
			m_CurrentFontAsset = materialReference2.fontAsset;
			m_CurrentMaterial = materialReference2.material;
			m_CurrentMaterialIndex = materialReference2.index;
			return true;
		}
		case MarkupTag.MATERIAL:
		{
			int valueHashCode = m_XmlAttribute[0].valueHashCode;
			if (valueHashCode == -620974005)
			{
				m_CurrentMaterial = m_MaterialReferences[0].material;
				m_CurrentMaterialIndex = 0;
				m_MaterialReferenceStack.Add(m_MaterialReferences[0]);
				return true;
			}
			if (MaterialReferenceManager.TryGetMaterial(valueHashCode, out material))
			{
				m_CurrentMaterial = material;
				m_CurrentMaterialIndex = MaterialReference.AddMaterialReference(m_CurrentMaterial, m_CurrentFontAsset, ref m_MaterialReferences, m_MaterialReferenceIndexLookup);
				m_MaterialReferenceStack.Add(m_MaterialReferences[m_CurrentMaterialIndex]);
			}
			else
			{
				if (!flag)
				{
					isThreadSuccess = false;
					return false;
				}
				material = Resources.Load<Material>(textSettings.defaultFontAssetPath + new string(m_HtmlTag, m_XmlAttribute[0].valueStartIndex, m_XmlAttribute[0].valueLength));
				if (material == null)
				{
					return false;
				}
				MaterialReferenceManager.AddFontMaterial(valueHashCode, material);
				m_CurrentMaterial = material;
				m_CurrentMaterialIndex = MaterialReference.AddMaterialReference(m_CurrentMaterial, m_CurrentFontAsset, ref m_MaterialReferences, m_MaterialReferenceIndexLookup);
				m_MaterialReferenceStack.Add(m_MaterialReferences[m_CurrentMaterialIndex]);
			}
			return true;
		}
		case MarkupTag.SLASH_MATERIAL:
		{
			MaterialReference materialReference = m_MaterialReferenceStack.Remove();
			m_CurrentMaterial = materialReference.material;
			m_CurrentMaterialIndex = materialReference.index;
			return true;
		}
		case MarkupTag.SPACE:
			num3 = TextGeneratorUtilities.ConvertToFloat(m_HtmlTag, m_XmlAttribute[0].valueStartIndex, m_XmlAttribute[0].valueLength);
			if (num3 == -32767f)
			{
				return false;
			}
			switch (tagUnitType)
			{
			case TagUnitType.Pixels:
				m_XAdvance += num3 * generationSettings.pixelsPerPoint;
				return true;
			case TagUnitType.FontUnits:
				m_XAdvance += num3 * m_CurrentFontSize;
				return true;
			case TagUnitType.Percentage:
				return false;
			default:
				return false;
			}
		case MarkupTag.ALPHA:
			if (m_XmlAttribute[0].valueLength != 3)
			{
				return false;
			}
			m_HtmlColor.a = (byte)(TextGeneratorUtilities.HexToInt(m_HtmlTag[7]) * 16 + TextGeneratorUtilities.HexToInt(m_HtmlTag[8]));
			return true;
		case MarkupTag.A:
			if (m_isTextLayoutPhase && !m_IsCalculatingPreferredValues)
			{
				if (generationSettings.isIMGUI && textInfo != null)
				{
					CloseLastLinkTag(textInfo);
					int linkCount = textInfo.linkCount;
					if (linkCount + 1 > textInfo.linkInfo.Length)
					{
						TextInfo.Resize(ref textInfo.linkInfo, linkCount + 1);
					}
					textInfo.linkInfo[linkCount].hashCode = 2535353;
					textInfo.linkInfo[linkCount].linkTextfirstCharacterIndex = m_CharacterCount;
					textInfo.linkInfo[linkCount].linkIdFirstCharacterIndex = 3;
					int num4 = m_XmlAttribute[1].valueLength;
					for (int num5 = num2; num5 >= 1; num5--)
					{
						if (m_XmlAttribute[num5].valueLength > 0)
						{
							num4 = m_XmlAttribute[num5].valueLength + m_XmlAttribute[num5].valueStartIndex;
							break;
						}
					}
					if (m_XmlAttribute[1].valueLength > 0)
					{
						textInfo.linkInfo[linkCount].SetLinkId(m_HtmlTag, 2, num4 - 1);
					}
					textInfo.linkCount++;
				}
				else if (m_XmlAttribute[1].nameHashCode == 2535353 && textInfo != null)
				{
					CloseLastLinkTag(textInfo);
					int linkCount2 = textInfo.linkCount;
					if (linkCount2 + 1 > textInfo.linkInfo.Length)
					{
						TextInfo.Resize(ref textInfo.linkInfo, linkCount2 + 1);
					}
					textInfo.linkInfo[linkCount2].hashCode = 2535353;
					textInfo.linkInfo[linkCount2].linkTextfirstCharacterIndex = m_CharacterCount;
					textInfo.linkInfo[linkCount2].linkIdFirstCharacterIndex = startIndex + m_XmlAttribute[1].valueStartIndex;
					textInfo.linkInfo[linkCount2].SetLinkId(m_HtmlTag, m_XmlAttribute[1].valueStartIndex, m_XmlAttribute[1].valueLength);
					textInfo.linkInfo[linkCount2].linkTextLength = -1;
					textInfo.linkCount++;
				}
			}
			return true;
		case MarkupTag.SLASH_A:
			if (m_isTextLayoutPhase && !m_IsCalculatingPreferredValues && textInfo != null)
			{
				if (textInfo.linkInfo.Length == 0 || textInfo.linkCount <= 0)
				{
					if (generationSettings.textSettings.displayWarnings)
					{
						Debug.LogWarning("There seems to be an issue with the formatting of the <a> tag. Possible issues include: missing or misplaced closing '>', missing or incorrect attribute, or unclosed quotes for attribute values. Please review the tag syntax.");
					}
				}
				else
				{
					int num9 = textInfo.linkCount - 1;
					textInfo.linkInfo[num9].linkTextLength = m_CharacterCount - textInfo.linkInfo[num9].linkTextfirstCharacterIndex;
				}
			}
			return true;
		case MarkupTag.LINK:
			if (m_isTextLayoutPhase && !m_IsCalculatingPreferredValues && textInfo != null)
			{
				CloseLastLinkTag(textInfo);
				int linkCount3 = textInfo.linkCount;
				if (linkCount3 + 1 > textInfo.linkInfo.Length)
				{
					TextInfo.Resize(ref textInfo.linkInfo, linkCount3 + 1);
				}
				textInfo.linkInfo[linkCount3].hashCode = m_XmlAttribute[0].valueHashCode;
				textInfo.linkInfo[linkCount3].linkTextfirstCharacterIndex = m_CharacterCount;
				textInfo.linkInfo[linkCount3].linkIdFirstCharacterIndex = startIndex + m_XmlAttribute[0].valueStartIndex;
				textInfo.linkInfo[linkCount3].SetLinkId(m_HtmlTag, m_XmlAttribute[0].valueStartIndex, m_XmlAttribute[0].valueLength);
				textInfo.linkInfo[linkCount3].linkTextLength = -1;
				textInfo.linkCount++;
			}
			return true;
		case MarkupTag.SLASH_LINK:
			if (m_isTextLayoutPhase && !m_IsCalculatingPreferredValues && textInfo != null)
			{
				if (textInfo.linkInfo.Length == 0 || textInfo.linkCount <= 0)
				{
					if (generationSettings.textSettings.displayWarnings)
					{
						Debug.LogWarning("There seems to be an issue with the formatting of the <link> tag. Possible issues include: missing or misplaced closing '>', missing or incorrect attribute, or unclosed quotes for attribute values. Please review the tag syntax.");
					}
				}
				else
				{
					textInfo.linkInfo[textInfo.linkCount - 1].linkTextLength = m_CharacterCount - textInfo.linkInfo[textInfo.linkCount - 1].linkTextfirstCharacterIndex;
				}
			}
			return true;
		case MarkupTag.ALIGN:
			switch ((MarkupTag)m_XmlAttribute[0].valueHashCode)
			{
			case MarkupTag.LEFT:
				m_LineJustification = TextAlignment.MiddleLeft;
				m_LineJustificationStack.Add(m_LineJustification);
				return true;
			case MarkupTag.RIGHT:
				m_LineJustification = TextAlignment.MiddleRight;
				m_LineJustificationStack.Add(m_LineJustification);
				return true;
			case MarkupTag.CENTER:
				m_LineJustification = TextAlignment.MiddleCenter;
				m_LineJustificationStack.Add(m_LineJustification);
				return true;
			case MarkupTag.JUSTIFIED:
				m_LineJustification = TextAlignment.MiddleJustified;
				m_LineJustificationStack.Add(m_LineJustification);
				return true;
			case MarkupTag.FLUSH:
				m_LineJustification = TextAlignment.MiddleFlush;
				m_LineJustificationStack.Add(m_LineJustification);
				return true;
			default:
				return false;
			}
		case MarkupTag.SLASH_ALIGN:
			m_LineJustification = m_LineJustificationStack.Remove();
			return true;
		case MarkupTag.WIDTH:
			num3 = TextGeneratorUtilities.ConvertToFloat(m_HtmlTag, m_XmlAttribute[0].valueStartIndex, m_XmlAttribute[0].valueLength);
			if (num3 == -32767f)
			{
				return false;
			}
			switch (tagUnitType)
			{
			case TagUnitType.Pixels:
				m_Width = num3 * generationSettings.pixelsPerPoint;
				break;
			case TagUnitType.FontUnits:
				return false;
			case TagUnitType.Percentage:
				m_Width = m_MarginWidth * num3 / 100f;
				break;
			}
			return true;
		case MarkupTag.SLASH_WIDTH:
			m_Width = -1f;
			return true;
		case MarkupTag.COLOR:
			if (textInfo != null)
			{
				textInfo.hasMultipleColors = true;
			}
			if (m_HtmlTag[6] == '#' || m_HtmlTag[7] == '#')
			{
				int num6 = num;
				if (m_HtmlTag[6] == '#')
				{
					startIndex = 6;
				}
				else
				{
					startIndex = 7;
					num6--;
				}
				m_HtmlColor = TextGeneratorUtilities.HexCharsToColor(m_HtmlTag, startIndex, num6 - startIndex);
				m_ColorStack.Add(m_HtmlColor);
				return true;
			}
			switch (m_XmlAttribute[0].valueHashCode)
			{
			case 91635:
				m_HtmlColor = Color.red;
				m_ColorStack.Add(m_HtmlColor);
				return true;
			case 341063360:
				m_HtmlColor = new Color32(173, 216, 230, byte.MaxValue);
				m_ColorStack.Add(m_HtmlColor);
				return true;
			case 2457214:
				m_HtmlColor = Color.blue;
				m_ColorStack.Add(m_HtmlColor);
				return true;
			case 2638345:
				m_HtmlColor = new Color32(128, 128, 128, byte.MaxValue);
				m_ColorStack.Add(m_HtmlColor);
				return true;
			case 81074727:
				m_HtmlColor = Color.black;
				m_ColorStack.Add(m_HtmlColor);
				return true;
			case 87065851:
				m_HtmlColor = Color.green;
				m_ColorStack.Add(m_HtmlColor);
				return true;
			case 105680263:
				m_HtmlColor = Color.white;
				m_ColorStack.Add(m_HtmlColor);
				return true;
			case -1108587920:
				m_HtmlColor = new Color32(byte.MaxValue, 128, 0, byte.MaxValue);
				m_ColorStack.Add(m_HtmlColor);
				return true;
			case -1250222130:
				m_HtmlColor = new Color32(160, 32, 240, byte.MaxValue);
				m_ColorStack.Add(m_HtmlColor);
				return true;
			case -882444668:
				m_HtmlColor = Color.yellow;
				m_ColorStack.Add(m_HtmlColor);
				return true;
			case 2947772:
				m_HtmlColor = new Color32(0, 128, 128, byte.MaxValue);
				m_ColorStack.Add(m_HtmlColor);
				return true;
			case 2504597:
				m_HtmlColor = new Color32(0, byte.MaxValue, byte.MaxValue, byte.MaxValue);
				m_ColorStack.Add(m_HtmlColor);
				return true;
			case -1960309918:
				m_HtmlColor = new Color32(0, 0, 160, byte.MaxValue);
				m_ColorStack.Add(m_HtmlColor);
				return true;
			case -1002715645:
				m_HtmlColor = new Color32(byte.MaxValue, 0, byte.MaxValue, byte.MaxValue);
				m_ColorStack.Add(m_HtmlColor);
				return true;
			case -960329321:
				m_HtmlColor = new Color32(192, 192, 192, byte.MaxValue);
				m_ColorStack.Add(m_HtmlColor);
				return true;
			case 81017702:
				m_HtmlColor = new Color32(165, 42, 42, byte.MaxValue);
				m_ColorStack.Add(m_HtmlColor);
				return true;
			case -1355621936:
				m_HtmlColor = new Color32(128, 0, 0, byte.MaxValue);
				m_ColorStack.Add(m_HtmlColor);
				return true;
			case 95492953:
				m_HtmlColor = new Color32(128, 128, 0, byte.MaxValue);
				m_ColorStack.Add(m_HtmlColor);
				return true;
			case 2876352:
				m_HtmlColor = new Color32(0, 0, 128, byte.MaxValue);
				m_ColorStack.Add(m_HtmlColor);
				return true;
			case 2284356:
				m_HtmlColor = new Color32(0, byte.MaxValue, byte.MaxValue, byte.MaxValue);
				m_ColorStack.Add(m_HtmlColor);
				return true;
			case -1812576107:
				m_HtmlColor = new Color32(byte.MaxValue, 0, byte.MaxValue, byte.MaxValue);
				m_ColorStack.Add(m_HtmlColor);
				return true;
			case 2656045:
				m_HtmlColor = new Color32(0, byte.MaxValue, 0, byte.MaxValue);
				m_ColorStack.Add(m_HtmlColor);
				return true;
			case -1014785338:
				m_HtmlColor = new Color32(0, 0, 0, 0);
				m_ColorStack.Add(m_HtmlColor);
				return true;
			default:
				return false;
			}
		case MarkupTag.GRADIENT:
		{
			int valueHashCode5 = m_XmlAttribute[0].valueHashCode;
			if (MaterialReferenceManager.TryGetColorGradientPreset(valueHashCode5, out var gradientPreset))
			{
				m_ColorGradientPreset = gradientPreset;
			}
			else
			{
				if (gradientPreset == null)
				{
					if (!flag)
					{
						isThreadSuccess = false;
						return false;
					}
					gradientPreset = Resources.Load<TextColorGradient>(textSettings.defaultColorGradientPresetsPath + new string(m_HtmlTag, m_XmlAttribute[0].valueStartIndex, m_XmlAttribute[0].valueLength));
				}
				if (gradientPreset == null)
				{
					return false;
				}
				MaterialReferenceManager.AddColorGradientPreset(valueHashCode5, gradientPreset);
				m_ColorGradientPreset = gradientPreset;
			}
			m_ColorGradientPresetIsTinted = false;
			for (int l = 1; l < m_XmlAttribute.Length && m_XmlAttribute[l].nameHashCode != 0; l++)
			{
				int nameHashCode3 = m_XmlAttribute[l].nameHashCode;
				MarkupTag markupTag = (MarkupTag)nameHashCode3;
				MarkupTag markupTag2 = markupTag;
				if (markupTag2 == MarkupTag.TINT)
				{
					m_ColorGradientPresetIsTinted = TextGeneratorUtilities.ConvertToFloat(m_HtmlTag, m_XmlAttribute[l].valueStartIndex, m_XmlAttribute[l].valueLength) != 0f;
				}
			}
			m_ColorGradientStack.Add(m_ColorGradientPreset);
			return true;
		}
		case MarkupTag.SLASH_GRADIENT:
			m_ColorGradientPreset = m_ColorGradientStack.Remove();
			return true;
		case MarkupTag.CHARACTER_SPACE:
			num3 = TextGeneratorUtilities.ConvertToFloat(m_HtmlTag, m_XmlAttribute[0].valueStartIndex, m_XmlAttribute[0].valueLength);
			if (num3 == -32767f)
			{
				return false;
			}
			switch (tagUnitType)
			{
			case TagUnitType.Pixels:
				m_CSpacing = num3 * generationSettings.pixelsPerPoint;
				break;
			case TagUnitType.FontUnits:
				m_CSpacing = num3 * m_CurrentFontSize;
				break;
			case TagUnitType.Percentage:
				return false;
			}
			return true;
		case MarkupTag.SLASH_CHARACTER_SPACE:
			if (!m_isTextLayoutPhase || textInfo == null)
			{
				return true;
			}
			if (m_CharacterCount > 0)
			{
				m_XAdvance -= m_CSpacing;
				textInfo.textElementInfo[m_CharacterCount - 1].xAdvance = m_XAdvance;
			}
			m_CSpacing = 0f;
			return true;
		case MarkupTag.MONOSPACE:
			num3 = TextGeneratorUtilities.ConvertToFloat(m_HtmlTag, m_XmlAttribute[0].valueStartIndex, m_XmlAttribute[0].valueLength);
			if (num3 == -32767f)
			{
				return false;
			}
			switch (m_XmlAttribute[0].unitType)
			{
			case TagUnitType.Pixels:
				m_MonoSpacing = num3 * generationSettings.pixelsPerPoint;
				break;
			case TagUnitType.FontUnits:
				m_MonoSpacing = num3 * m_CurrentFontSize;
				break;
			case TagUnitType.Percentage:
				return false;
			}
			if (m_XmlAttribute[1].nameHashCode == 582810522)
			{
				m_DuoSpace = TextGeneratorUtilities.ConvertToFloat(m_HtmlTag, m_XmlAttribute[1].valueStartIndex, m_XmlAttribute[1].valueLength) != 0f;
			}
			return true;
		case MarkupTag.SLASH_MONOSPACE:
			m_MonoSpacing = 0f;
			m_DuoSpace = false;
			return true;
		case MarkupTag.CLASS:
			return false;
		case MarkupTag.SLASH_COLOR:
			m_HtmlColor = m_ColorStack.Remove();
			return true;
		case MarkupTag.INDENT:
			num3 = TextGeneratorUtilities.ConvertToFloat(m_HtmlTag, m_XmlAttribute[0].valueStartIndex, m_XmlAttribute[0].valueLength);
			if (num3 == -32767f)
			{
				return false;
			}
			switch (tagUnitType)
			{
			case TagUnitType.Pixels:
				m_TagIndent = num3 * generationSettings.pixelsPerPoint;
				break;
			case TagUnitType.FontUnits:
				m_TagIndent = num3 * m_CurrentFontSize;
				break;
			case TagUnitType.Percentage:
				m_TagIndent = m_MarginWidth * num3 / 100f;
				break;
			}
			m_IndentStack.Add(m_TagIndent);
			m_XAdvance = m_TagIndent;
			return true;
		case MarkupTag.SLASH_INDENT:
			m_TagIndent = m_IndentStack.Remove();
			return true;
		case MarkupTag.LINE_INDENT:
			num3 = TextGeneratorUtilities.ConvertToFloat(m_HtmlTag, m_XmlAttribute[0].valueStartIndex, m_XmlAttribute[0].valueLength);
			if (num3 == -32767f)
			{
				return false;
			}
			switch (tagUnitType)
			{
			case TagUnitType.Pixels:
				m_TagLineIndent = num3 * generationSettings.pixelsPerPoint;
				break;
			case TagUnitType.FontUnits:
				m_TagLineIndent = num3 * m_CurrentFontSize;
				break;
			case TagUnitType.Percentage:
				m_TagLineIndent = m_MarginWidth * num3 / 100f;
				break;
			}
			m_XAdvance += m_TagLineIndent;
			return true;
		case MarkupTag.SLASH_LINE_INDENT:
			m_TagLineIndent = 0f;
			return true;
		case MarkupTag.SPRITE:
		{
			int valueHashCode4 = m_XmlAttribute[0].valueHashCode;
			m_SpriteIndex = -1;
			SpriteAsset spriteAsset;
			if (m_XmlAttribute[0].valueType == TagValueType.None || m_XmlAttribute[0].valueType == TagValueType.NumericalValue)
			{
				if (textSettings.defaultSpriteAsset != null)
				{
					m_CurrentSpriteAsset = textSettings.defaultSpriteAsset;
				}
				else if (TextSettings.s_GlobalSpriteAsset != null)
				{
					m_CurrentSpriteAsset = TextSettings.s_GlobalSpriteAsset;
				}
				if (m_CurrentSpriteAsset == null)
				{
					return false;
				}
			}
			else if (MaterialReferenceManager.TryGetSpriteAsset(valueHashCode4, out spriteAsset))
			{
				m_CurrentSpriteAsset = spriteAsset;
			}
			else
			{
				if (spriteAsset == null && spriteAsset == null)
				{
					if (!flag)
					{
						isThreadSuccess = false;
						return false;
					}
					spriteAsset = Resources.Load<SpriteAsset>(textSettings.defaultSpriteAssetPath + new string(m_HtmlTag, m_XmlAttribute[0].valueStartIndex, m_XmlAttribute[0].valueLength));
				}
				if (spriteAsset == null)
				{
					return false;
				}
				MaterialReferenceManager.AddSpriteAsset(valueHashCode4, spriteAsset);
				m_CurrentSpriteAsset = spriteAsset;
			}
			if (!flag && m_CurrentSpriteAsset.m_GlyphIndexLookup == null)
			{
				isThreadSuccess = false;
				return false;
			}
			if (m_XmlAttribute[0].valueType == TagValueType.NumericalValue)
			{
				int num8 = (int)TextGeneratorUtilities.ConvertToFloat(m_HtmlTag, m_XmlAttribute[0].valueStartIndex, m_XmlAttribute[0].valueLength);
				if ((float)num8 == -32767f)
				{
					return false;
				}
				if (num8 > m_CurrentSpriteAsset.spriteCharacterTable.Count - 1)
				{
					return false;
				}
				m_SpriteIndex = num8;
			}
			m_SpriteColor = Color.white;
			m_TintSprite = false;
			for (int k = 0; k < m_XmlAttribute.Length && m_XmlAttribute[k].nameHashCode != 0; k++)
			{
				int nameHashCode2 = m_XmlAttribute[k].nameHashCode;
				int spriteIndex = 0;
				switch ((MarkupTag)nameHashCode2)
				{
				case MarkupTag.NAME:
					m_CurrentSpriteAsset = SpriteAsset.SearchForSpriteByHashCode(m_CurrentSpriteAsset, m_XmlAttribute[k].valueHashCode, includeFallbacks: true, out spriteIndex);
					if (spriteIndex == -1)
					{
						return false;
					}
					m_SpriteIndex = spriteIndex;
					break;
				case MarkupTag.INDEX:
					spriteIndex = (int)TextGeneratorUtilities.ConvertToFloat(m_HtmlTag, m_XmlAttribute[1].valueStartIndex, m_XmlAttribute[1].valueLength);
					if ((float)spriteIndex == -32767f)
					{
						return false;
					}
					if (spriteIndex > m_CurrentSpriteAsset.spriteCharacterTable.Count - 1)
					{
						return false;
					}
					m_SpriteIndex = spriteIndex;
					break;
				case MarkupTag.TINT:
					m_TintSprite = TextGeneratorUtilities.ConvertToFloat(m_HtmlTag, m_XmlAttribute[k].valueStartIndex, m_XmlAttribute[k].valueLength) != 0f;
					break;
				case MarkupTag.COLOR:
					m_SpriteColor = TextGeneratorUtilities.HexCharsToColor(m_HtmlTag, m_XmlAttribute[k].valueStartIndex, m_XmlAttribute[k].valueLength);
					break;
				case MarkupTag.ANIM:
				{
					int attributeParameters = TextGeneratorUtilities.GetAttributeParameters(m_HtmlTag, m_XmlAttribute[k].valueStartIndex, m_XmlAttribute[k].valueLength, ref m_AttributeParameterValues);
					if (attributeParameters != 3)
					{
						return false;
					}
					m_SpriteIndex = (int)m_AttributeParameterValues[0];
					if (!m_isTextLayoutPhase)
					{
					}
					break;
				}
				default:
					if (nameHashCode2 != -991527447)
					{
						return false;
					}
					break;
				}
			}
			if (m_SpriteIndex == -1)
			{
				return false;
			}
			m_CurrentMaterialIndex = MaterialReference.AddMaterialReference(m_CurrentSpriteAsset.material, m_CurrentSpriteAsset, ref m_MaterialReferences, m_MaterialReferenceIndexLookup);
			m_TextElementType = TextElementType.Sprite;
			return true;
		}
		case MarkupTag.LOWERCASE:
			m_FontStyleInternal |= FontStyles.LowerCase;
			m_FontStyleStack.Add(FontStyles.LowerCase);
			return true;
		case MarkupTag.SLASH_LOWERCASE:
			if ((generationSettings.fontStyle & FontStyles.LowerCase) != FontStyles.LowerCase && m_FontStyleStack.Remove(FontStyles.LowerCase) == 0)
			{
				m_FontStyleInternal &= ~FontStyles.LowerCase;
			}
			return true;
		case MarkupTag.UPPERCASE:
		case MarkupTag.ALLCAPS:
			m_FontStyleInternal |= FontStyles.UpperCase;
			m_FontStyleStack.Add(FontStyles.UpperCase);
			return true;
		case MarkupTag.SLASH_ALLCAPS:
		case MarkupTag.SLASH_UPPERCASE:
			if ((generationSettings.fontStyle & FontStyles.UpperCase) != FontStyles.UpperCase && m_FontStyleStack.Remove(FontStyles.UpperCase) == 0)
			{
				m_FontStyleInternal &= ~FontStyles.UpperCase;
			}
			return true;
		case MarkupTag.SMALLCAPS:
			m_FontStyleInternal |= FontStyles.SmallCaps;
			m_FontStyleStack.Add(FontStyles.SmallCaps);
			return true;
		case MarkupTag.SLASH_SMALLCAPS:
			if ((generationSettings.fontStyle & FontStyles.SmallCaps) != FontStyles.SmallCaps && m_FontStyleStack.Remove(FontStyles.SmallCaps) == 0)
			{
				m_FontStyleInternal &= ~FontStyles.SmallCaps;
			}
			return true;
		case MarkupTag.MARGIN:
			switch (m_XmlAttribute[0].valueType)
			{
			case TagValueType.NumericalValue:
				num3 = TextGeneratorUtilities.ConvertToFloat(m_HtmlTag, m_XmlAttribute[0].valueStartIndex, m_XmlAttribute[0].valueLength);
				if (num3 == -32767f)
				{
					return false;
				}
				switch (tagUnitType)
				{
				case TagUnitType.Pixels:
					m_MarginLeft = num3 * generationSettings.pixelsPerPoint;
					break;
				case TagUnitType.FontUnits:
					m_MarginLeft = num3 * m_CurrentFontSize;
					break;
				case TagUnitType.Percentage:
					m_MarginLeft = (m_MarginWidth - ((m_Width != -1f) ? m_Width : 0f)) * num3 / 100f;
					break;
				}
				m_MarginLeft = ((m_MarginLeft >= 0f) ? m_MarginLeft : 0f);
				m_MarginRight = m_MarginLeft;
				return true;
			case TagValueType.None:
			{
				for (int j = 1; j < m_XmlAttribute.Length && m_XmlAttribute[j].nameHashCode != 0; j++)
				{
					switch ((MarkupTag)m_XmlAttribute[j].nameHashCode)
					{
					case MarkupTag.LEFT:
						num3 = TextGeneratorUtilities.ConvertToFloat(m_HtmlTag, m_XmlAttribute[j].valueStartIndex, m_XmlAttribute[j].valueLength);
						if (num3 == -32767f)
						{
							return false;
						}
						switch (m_XmlAttribute[j].unitType)
						{
						case TagUnitType.Pixels:
							m_MarginLeft = num3 * generationSettings.pixelsPerPoint;
							break;
						case TagUnitType.FontUnits:
							m_MarginLeft = num3 * m_CurrentFontSize;
							break;
						case TagUnitType.Percentage:
							m_MarginLeft = (m_MarginWidth - ((m_Width != -1f) ? m_Width : 0f)) * num3 / 100f;
							break;
						}
						m_MarginLeft = ((m_MarginLeft >= 0f) ? m_MarginLeft : 0f);
						break;
					case MarkupTag.RIGHT:
						num3 = TextGeneratorUtilities.ConvertToFloat(m_HtmlTag, m_XmlAttribute[j].valueStartIndex, m_XmlAttribute[j].valueLength);
						if (num3 == -32767f)
						{
							return false;
						}
						switch (m_XmlAttribute[j].unitType)
						{
						case TagUnitType.Pixels:
							m_MarginRight = num3 * generationSettings.pixelsPerPoint;
							break;
						case TagUnitType.FontUnits:
							m_MarginRight = num3 * m_CurrentFontSize;
							break;
						case TagUnitType.Percentage:
							m_MarginRight = (m_MarginWidth - ((m_Width != -1f) ? m_Width : 0f)) * num3 / 100f;
							break;
						}
						m_MarginRight = ((m_MarginRight >= 0f) ? m_MarginRight : 0f);
						break;
					}
				}
				return true;
			}
			default:
				return false;
			}
		case MarkupTag.SLASH_MARGIN:
			m_MarginLeft = 0f;
			m_MarginRight = 0f;
			return true;
		case MarkupTag.MARGIN_LEFT:
			num3 = TextGeneratorUtilities.ConvertToFloat(m_HtmlTag, m_XmlAttribute[0].valueStartIndex, m_XmlAttribute[0].valueLength);
			if (num3 == -32767f)
			{
				return false;
			}
			switch (tagUnitType)
			{
			case TagUnitType.Pixels:
				m_MarginLeft = num3 * generationSettings.pixelsPerPoint;
				break;
			case TagUnitType.FontUnits:
				m_MarginLeft = num3 * m_CurrentFontSize;
				break;
			case TagUnitType.Percentage:
				m_MarginLeft = (m_MarginWidth - ((m_Width != -1f) ? m_Width : 0f)) * num3 / 100f;
				break;
			}
			m_MarginLeft = ((m_MarginLeft >= 0f) ? m_MarginLeft : 0f);
			return true;
		case MarkupTag.MARGIN_RIGHT:
			num3 = TextGeneratorUtilities.ConvertToFloat(m_HtmlTag, m_XmlAttribute[0].valueStartIndex, m_XmlAttribute[0].valueLength);
			if (num3 == -32767f)
			{
				return false;
			}
			switch (tagUnitType)
			{
			case TagUnitType.Pixels:
				m_MarginRight = num3 * generationSettings.pixelsPerPoint;
				break;
			case TagUnitType.FontUnits:
				m_MarginRight = num3 * m_CurrentFontSize;
				break;
			case TagUnitType.Percentage:
				m_MarginRight = (m_MarginWidth - ((m_Width != -1f) ? m_Width : 0f)) * num3 / 100f;
				break;
			}
			m_MarginRight = ((m_MarginRight >= 0f) ? m_MarginRight : 0f);
			return true;
		case MarkupTag.LINE_HEIGHT:
			num3 = TextGeneratorUtilities.ConvertToFloat(m_HtmlTag, m_XmlAttribute[0].valueStartIndex, m_XmlAttribute[0].valueLength);
			if (num3 == -32767f)
			{
				return false;
			}
			switch (tagUnitType)
			{
			case TagUnitType.Pixels:
				m_LineHeight = num3 * generationSettings.pixelsPerPoint;
				break;
			case TagUnitType.FontUnits:
				m_LineHeight = num3 * m_CurrentFontSize;
				break;
			case TagUnitType.Percentage:
			{
				float num7 = m_CurrentFontSize / m_CurrentFontAsset.faceInfo.pointSize * m_CurrentFontAsset.faceInfo.scale;
				m_LineHeight = generationSettings.fontAsset.faceInfo.lineHeight * num3 / 100f * num7;
				break;
			}
			}
			return true;
		case MarkupTag.SLASH_LINE_HEIGHT:
			m_LineHeight = -32767f;
			return true;
		case MarkupTag.NO_PARSE:
			m_TagNoParsing = true;
			return true;
		case MarkupTag.ACTION:
		{
			int valueHashCode2 = m_XmlAttribute[0].valueHashCode;
			if (m_isTextLayoutPhase)
			{
				m_ActionStack.Add(valueHashCode2);
				Debug.Log("Action ID: [" + valueHashCode2 + "] First character index: " + m_CharacterCount);
			}
			return true;
		}
		case MarkupTag.SLASH_ACTION:
			if (m_isTextLayoutPhase)
			{
				Debug.Log("Action ID: [" + m_ActionStack.CurrentItem() + "] Last character index: " + (m_CharacterCount - 1));
			}
			m_ActionStack.Remove();
			return true;
		case MarkupTag.SCALE:
			num3 = TextGeneratorUtilities.ConvertToFloat(m_HtmlTag, m_XmlAttribute[0].valueStartIndex, m_XmlAttribute[0].valueLength);
			if (num3 == -32767f)
			{
				return false;
			}
			m_FXScale = new Vector3(num3, 1f, 1f);
			return true;
		case MarkupTag.SLASH_SCALE:
			m_FXScale = Vector3.one;
			return true;
		case MarkupTag.ROTATE:
			num3 = TextGeneratorUtilities.ConvertToFloat(m_HtmlTag, m_XmlAttribute[0].valueStartIndex, m_XmlAttribute[0].valueLength);
			if (num3 == -32767f)
			{
				return false;
			}
			m_FXRotation = Quaternion.Euler(0f, 0f, num3);
			return true;
		case MarkupTag.SLASH_ROTATE:
			m_FXRotation = Quaternion.identity;
			return true;
		case MarkupTag.TABLE:
			return false;
		case MarkupTag.SLASH_TABLE:
			return false;
		case MarkupTag.TR:
			return false;
		case MarkupTag.SLASH_TR:
			return false;
		case MarkupTag.TH:
			return false;
		case MarkupTag.SLASH_TH:
			return false;
		case MarkupTag.TD:
			return false;
		case MarkupTag.SLASH_TD:
			return false;
		default:
			return false;
		}
	}

	internal void CloseLastLinkTag(TextInfo textInfo)
	{
		if (textInfo.linkInfo.Length != 0 && textInfo.linkCount > 0)
		{
			CloseLinkTag(textInfo, textInfo.linkCount - 1);
		}
	}

	internal void CloseAllLinkTags(TextInfo textInfo)
	{
		for (int num = textInfo.linkCount - 1; num >= 0; num--)
		{
			CloseLinkTag(textInfo, num);
		}
	}

	private void CloseLinkTag(TextInfo textInfo, int index)
	{
		if (textInfo.linkInfo[index].linkTextLength == -1)
		{
			textInfo.linkInfo[index].linkTextLength = m_CharacterCount - textInfo.linkInfo[index].linkTextfirstCharacterIndex;
		}
	}

	private void ClearMarkupTagAttributes()
	{
		int num = m_XmlAttribute.Length;
		for (int i = 0; i < num; i++)
		{
			m_XmlAttribute[i] = default(RichTextTagAttribute);
		}
	}

	private void SaveWordWrappingState(ref WordWrapState state, int index, int count, TextInfo textInfo)
	{
		state.currentFontAsset = m_CurrentFontAsset;
		state.currentSpriteAsset = m_CurrentSpriteAsset;
		state.currentMaterial = m_CurrentMaterial;
		state.currentMaterialIndex = m_CurrentMaterialIndex;
		state.previousWordBreak = index;
		state.totalCharacterCount = count;
		state.visibleCharacterCount = m_LineVisibleCharacterCount;
		state.visibleSpaceCount = m_LineVisibleSpaceCount;
		state.visibleLinkCount = textInfo.linkCount;
		state.firstCharacterIndex = m_FirstCharacterOfLine;
		state.firstVisibleCharacterIndex = m_FirstVisibleCharacterOfLine;
		state.lastVisibleCharIndex = m_LastVisibleCharacterOfLine;
		state.fontStyle = m_FontStyleInternal;
		state.italicAngle = m_ItalicAngle;
		state.fontScaleMultiplier = m_FontScaleMultiplier;
		state.currentFontSize = m_CurrentFontSize;
		state.xAdvance = m_XAdvance;
		state.maxCapHeight = m_MaxCapHeight;
		state.maxAscender = m_MaxAscender;
		state.maxDescender = m_MaxDescender;
		state.maxLineAscender = m_MaxLineAscender;
		state.maxLineDescender = m_MaxLineDescender;
		state.startOfLineAscender = m_StartOfLineAscender;
		state.preferredWidth = m_PreferredWidth;
		state.preferredHeight = m_PreferredHeight;
		state.meshExtents = m_MeshExtents;
		state.pageAscender = m_PageAscender;
		state.lineNumber = m_LineNumber;
		state.lineOffset = m_LineOffset;
		state.baselineOffset = m_BaselineOffset;
		state.isDrivenLineSpacing = m_IsDrivenLineSpacing;
		state.vertexColor = m_HtmlColor;
		state.underlineColor = m_UnderlineColor;
		state.strikethroughColor = m_StrikethroughColor;
		state.highlightColor = m_HighlightColor;
		state.highlightState = m_HighlightState;
		state.isNonBreakingSpace = m_IsNonBreakingSpace;
		state.tagNoParsing = m_TagNoParsing;
		state.fxScale = m_FXScale;
		state.fxRotation = m_FXRotation;
		state.basicStyleStack = m_FontStyleStack;
		state.italicAngleStack = m_ItalicAngleStack;
		state.colorStack = m_ColorStack;
		state.underlineColorStack = m_UnderlineColorStack;
		state.strikethroughColorStack = m_StrikethroughColorStack;
		state.highlightColorStack = m_HighlightColorStack;
		state.colorGradientStack = m_ColorGradientStack;
		state.highlightStateStack = m_HighlightStateStack;
		state.sizeStack = m_SizeStack;
		state.indentStack = m_IndentStack;
		state.fontWeightStack = m_FontWeightStack;
		state.styleStack = m_StyleStack;
		state.baselineStack = m_BaselineOffsetStack;
		state.actionStack = m_ActionStack;
		state.materialReferenceStack = m_MaterialReferenceStack;
		state.lineJustificationStack = m_LineJustificationStack;
		state.lastBaseGlyphIndex = m_LastBaseGlyphIndex;
		state.spriteAnimationId = m_SpriteAnimationId;
		if (m_LineNumber < textInfo.lineInfo.Length)
		{
			state.lineInfo = textInfo.lineInfo[m_LineNumber];
		}
	}

	private int RestoreWordWrappingState(ref WordWrapState state, TextInfo textInfo)
	{
		int previousWordBreak = state.previousWordBreak;
		m_CurrentFontAsset = state.currentFontAsset;
		m_CurrentSpriteAsset = state.currentSpriteAsset;
		m_CurrentMaterial = state.currentMaterial;
		m_CurrentMaterialIndex = state.currentMaterialIndex;
		m_CharacterCount = state.totalCharacterCount + 1;
		m_LineVisibleCharacterCount = state.visibleCharacterCount;
		m_LineVisibleSpaceCount = state.visibleSpaceCount;
		textInfo.linkCount = state.visibleLinkCount;
		m_FirstCharacterOfLine = state.firstCharacterIndex;
		m_FirstVisibleCharacterOfLine = state.firstVisibleCharacterIndex;
		m_LastVisibleCharacterOfLine = state.lastVisibleCharIndex;
		m_FontStyleInternal = state.fontStyle;
		m_ItalicAngle = state.italicAngle;
		m_FontScaleMultiplier = state.fontScaleMultiplier;
		m_CurrentFontSize = state.currentFontSize;
		m_XAdvance = state.xAdvance;
		m_MaxCapHeight = state.maxCapHeight;
		m_MaxAscender = state.maxAscender;
		m_MaxDescender = state.maxDescender;
		m_MaxLineAscender = state.maxLineAscender;
		m_MaxLineDescender = state.maxLineDescender;
		m_StartOfLineAscender = state.startOfLineAscender;
		m_PreferredWidth = state.preferredWidth;
		m_PreferredHeight = state.preferredHeight;
		m_MeshExtents = state.meshExtents;
		m_PageAscender = state.pageAscender;
		m_LineNumber = state.lineNumber;
		m_LineOffset = state.lineOffset;
		m_BaselineOffset = state.baselineOffset;
		m_IsDrivenLineSpacing = state.isDrivenLineSpacing;
		m_HtmlColor = state.vertexColor;
		m_UnderlineColor = state.underlineColor;
		m_StrikethroughColor = state.strikethroughColor;
		m_HighlightColor = state.highlightColor;
		m_HighlightState = state.highlightState;
		m_IsNonBreakingSpace = state.isNonBreakingSpace;
		m_TagNoParsing = state.tagNoParsing;
		m_FXScale = state.fxScale;
		m_FXRotation = state.fxRotation;
		m_FontStyleStack = state.basicStyleStack;
		m_ItalicAngleStack = state.italicAngleStack;
		m_ColorStack = state.colorStack;
		m_UnderlineColorStack = state.underlineColorStack;
		m_StrikethroughColorStack = state.strikethroughColorStack;
		m_HighlightColorStack = state.highlightColorStack;
		m_ColorGradientStack = state.colorGradientStack;
		m_HighlightStateStack = state.highlightStateStack;
		m_SizeStack = state.sizeStack;
		m_IndentStack = state.indentStack;
		m_FontWeightStack = state.fontWeightStack;
		m_StyleStack = state.styleStack;
		m_BaselineOffsetStack = state.baselineStack;
		m_ActionStack = state.actionStack;
		m_MaterialReferenceStack = state.materialReferenceStack;
		m_LineJustificationStack = state.lineJustificationStack;
		m_LastBaseGlyphIndex = state.lastBaseGlyphIndex;
		m_SpriteAnimationId = state.spriteAnimationId;
		if (m_LineNumber < textInfo.lineInfo.Length)
		{
			textInfo.lineInfo[m_LineNumber] = state.lineInfo;
		}
		return previousWordBreak;
	}

	private void SaveGlyphVertexInfo(float padding, float stylePadding, Color32 vertexColor, TextGenerationSettings generationSettings, TextInfo textInfo)
	{
		textInfo.textElementInfo[m_CharacterCount].vertexBottomLeft.position = textInfo.textElementInfo[m_CharacterCount].bottomLeft;
		textInfo.textElementInfo[m_CharacterCount].vertexTopLeft.position = textInfo.textElementInfo[m_CharacterCount].topLeft;
		textInfo.textElementInfo[m_CharacterCount].vertexTopRight.position = textInfo.textElementInfo[m_CharacterCount].topRight;
		textInfo.textElementInfo[m_CharacterCount].vertexBottomRight.position = textInfo.textElementInfo[m_CharacterCount].bottomRight;
		vertexColor.a = ((m_FontColor32.a < vertexColor.a) ? m_FontColor32.a : vertexColor.a);
		bool flag = (m_CurrentFontAsset.m_AtlasRenderMode & (GlyphRenderMode)65536) == (GlyphRenderMode)65536;
		vertexColor = (flag ? new Color32(byte.MaxValue, byte.MaxValue, byte.MaxValue, vertexColor.a) : vertexColor);
		textInfo.textElementInfo[m_CharacterCount].vertexBottomLeft.color = vertexColor;
		textInfo.textElementInfo[m_CharacterCount].vertexTopLeft.color = vertexColor;
		textInfo.textElementInfo[m_CharacterCount].vertexTopRight.color = vertexColor;
		textInfo.textElementInfo[m_CharacterCount].vertexBottomRight.color = vertexColor;
		if (m_ColorGradientPreset != null && !flag)
		{
			if (m_ColorGradientPresetIsTinted)
			{
				ref Color32 color = ref textInfo.textElementInfo[m_CharacterCount].vertexBottomLeft.color;
				color *= m_ColorGradientPreset.bottomLeft;
				ref Color32 color2 = ref textInfo.textElementInfo[m_CharacterCount].vertexTopLeft.color;
				color2 *= m_ColorGradientPreset.topLeft;
				ref Color32 color3 = ref textInfo.textElementInfo[m_CharacterCount].vertexTopRight.color;
				color3 *= m_ColorGradientPreset.topRight;
				ref Color32 color4 = ref textInfo.textElementInfo[m_CharacterCount].vertexBottomRight.color;
				color4 *= m_ColorGradientPreset.bottomRight;
			}
			else
			{
				textInfo.textElementInfo[m_CharacterCount].vertexBottomLeft.color = m_ColorGradientPreset.bottomLeft.MinAlpha(vertexColor);
				textInfo.textElementInfo[m_CharacterCount].vertexTopLeft.color = m_ColorGradientPreset.topLeft.MinAlpha(vertexColor);
				textInfo.textElementInfo[m_CharacterCount].vertexTopRight.color = m_ColorGradientPreset.topRight.MinAlpha(vertexColor);
				textInfo.textElementInfo[m_CharacterCount].vertexBottomRight.color = m_ColorGradientPreset.bottomRight.MinAlpha(vertexColor);
			}
		}
		stylePadding = 0f;
		GlyphRect glyphRect = textInfo.textElementInfo[m_CharacterCount].alternativeGlyph?.glyphRect ?? m_CachedTextElement.m_Glyph.glyphRect;
		Vector2 vector = default(Vector2);
		vector.x = ((float)glyphRect.x - padding - stylePadding) / (float)m_CurrentFontAsset.atlasWidth;
		vector.y = ((float)glyphRect.y - padding - stylePadding) / (float)m_CurrentFontAsset.atlasHeight;
		Vector2 vector2 = default(Vector2);
		vector2.x = vector.x;
		vector2.y = ((float)glyphRect.y + padding + stylePadding + (float)glyphRect.height) / (float)m_CurrentFontAsset.atlasHeight;
		Vector2 vector3 = default(Vector2);
		vector3.x = ((float)glyphRect.x + padding + stylePadding + (float)glyphRect.width) / (float)m_CurrentFontAsset.atlasWidth;
		vector3.y = vector2.y;
		Vector2 vector4 = default(Vector2);
		vector4.x = vector3.x;
		vector4.y = vector.y;
		textInfo.textElementInfo[m_CharacterCount].vertexBottomLeft.uv = vector;
		textInfo.textElementInfo[m_CharacterCount].vertexTopLeft.uv = vector2;
		textInfo.textElementInfo[m_CharacterCount].vertexTopRight.uv = vector3;
		textInfo.textElementInfo[m_CharacterCount].vertexBottomRight.uv = vector4;
	}

	private void SaveSpriteVertexInfo(Color32 vertexColor, TextGenerationSettings generationSettings, TextInfo textInfo)
	{
		textInfo.textElementInfo[m_CharacterCount].vertexBottomLeft.position = textInfo.textElementInfo[m_CharacterCount].bottomLeft;
		textInfo.textElementInfo[m_CharacterCount].vertexTopLeft.position = textInfo.textElementInfo[m_CharacterCount].topLeft;
		textInfo.textElementInfo[m_CharacterCount].vertexTopRight.position = textInfo.textElementInfo[m_CharacterCount].topRight;
		textInfo.textElementInfo[m_CharacterCount].vertexBottomRight.position = textInfo.textElementInfo[m_CharacterCount].bottomRight;
		Color32 color = (m_TintSprite ? ColorUtilities.MultiplyColors(m_SpriteColor, vertexColor) : m_SpriteColor);
		color.a = ((color.a >= m_FontColor32.a) ? m_FontColor32.a : ((color.a < vertexColor.a) ? color.a : vertexColor.a));
		Color32 color2 = color;
		Color32 color3 = color;
		Color32 color4 = color;
		Color32 color5 = color;
		if (m_ColorGradientPreset != null)
		{
			color2 = (m_TintSprite ? ColorUtilities.MultiplyColors(color2, m_ColorGradientPreset.bottomLeft) : color2);
			color3 = (m_TintSprite ? ColorUtilities.MultiplyColors(color3, m_ColorGradientPreset.topLeft) : color3);
			color4 = (m_TintSprite ? ColorUtilities.MultiplyColors(color4, m_ColorGradientPreset.topRight) : color4);
			color5 = (m_TintSprite ? ColorUtilities.MultiplyColors(color5, m_ColorGradientPreset.bottomRight) : color5);
		}
		m_TintSprite = false;
		textInfo.textElementInfo[m_CharacterCount].vertexBottomLeft.color = color2;
		textInfo.textElementInfo[m_CharacterCount].vertexTopLeft.color = color3;
		textInfo.textElementInfo[m_CharacterCount].vertexTopRight.color = color4;
		textInfo.textElementInfo[m_CharacterCount].vertexBottomRight.color = color5;
		Vector2 vector = new Vector2((float)m_CachedTextElement.glyph.glyphRect.x / m_CurrentSpriteAsset.width, (float)m_CachedTextElement.glyph.glyphRect.y / m_CurrentSpriteAsset.height);
		Vector2 vector2 = new Vector2(vector.x, (float)(m_CachedTextElement.glyph.glyphRect.y + m_CachedTextElement.glyph.glyphRect.height) / m_CurrentSpriteAsset.height);
		Vector2 vector3 = new Vector2((float)(m_CachedTextElement.glyph.glyphRect.x + m_CachedTextElement.glyph.glyphRect.width) / m_CurrentSpriteAsset.width, vector2.y);
		Vector2 vector4 = new Vector2(vector3.x, vector.y);
		textInfo.textElementInfo[m_CharacterCount].vertexBottomLeft.uv = vector;
		textInfo.textElementInfo[m_CharacterCount].vertexTopLeft.uv = vector2;
		textInfo.textElementInfo[m_CharacterCount].vertexTopRight.uv = vector3;
		textInfo.textElementInfo[m_CharacterCount].vertexBottomRight.uv = vector4;
	}

	private void DrawUnderlineMesh(Vector3 start, Vector3 end, float startScale, float endScale, float maxScale, float sdfScale, Color32 underlineColor, TextGenerationSettings generationSettings, TextInfo textInfo)
	{
		GetUnderlineSpecialCharacter(generationSettings);
		if (m_Underline.character == null)
		{
			if (generationSettings.textSettings.displayWarnings)
			{
				Debug.LogWarning("Unable to add underline or strikethrough since the character [0x5F] used by these features is not present in the Font Asset assigned to this text object.");
			}
			return;
		}
		int materialIndex = m_Underline.materialIndex;
		int vertexCount = textInfo.meshInfo[materialIndex].vertexCount;
		int num = vertexCount + 12;
		if (num > textInfo.meshInfo[materialIndex].vertexBufferSize)
		{
			textInfo.meshInfo[materialIndex].ResizeMeshInfo(num / 4, generationSettings.isIMGUI);
		}
		start.y = Mathf.Min(start.y, end.y);
		end.y = Mathf.Min(start.y, end.y);
		GlyphMetrics metrics = m_Underline.character.glyph.metrics;
		GlyphRect glyphRect = m_Underline.character.glyph.glyphRect;
		float underlineThickness = m_Underline.fontAsset.faceInfo.underlineThickness;
		start.x += (startScale - maxScale) * m_Padding;
		end.x += (maxScale - endScale) * m_Padding;
		float num2 = (metrics.width * 0.5f + m_Padding) * maxScale;
		float num3 = 1f;
		float num4 = 2f * num2;
		float num5 = end.x - start.x;
		if (num5 < num4)
		{
			num3 = num5 / num4;
			num2 *= num3;
		}
		TextCoreVertex[] vertexData = textInfo.meshInfo[materialIndex].vertexData;
		float x = start.x;
		float x2 = start.x + num2;
		float x3 = end.x - num2;
		float x4 = end.x;
		float y = start.y - (underlineThickness + m_Padding) * maxScale;
		float y2 = start.y + m_Padding * maxScale;
		vertexData[vertexCount].position = new Vector3(x, y);
		vertexData[vertexCount + 1].position = new Vector3(x, y2);
		vertexData[vertexCount + 2].position = new Vector3(x2, y2);
		vertexData[vertexCount + 3].position = new Vector3(x2, y);
		vertexData[vertexCount + 4].position = new Vector3(x2, y);
		vertexData[vertexCount + 5].position = new Vector3(x2, y2);
		vertexData[vertexCount + 6].position = new Vector3(x3, y2);
		vertexData[vertexCount + 7].position = new Vector3(x3, y);
		vertexData[vertexCount + 8].position = new Vector3(x3, y);
		vertexData[vertexCount + 9].position = new Vector3(x3, y2);
		vertexData[vertexCount + 10].position = new Vector3(x4, y2);
		vertexData[vertexCount + 11].position = new Vector3(x4, y);
		Vector3 vector = default(Vector3);
		vector.x = 0f;
		vector.y = generationSettings.screenRect.height;
		vector.z = 0f;
		for (int i = 0; i < 12; i++)
		{
			textInfo.meshInfo[materialIndex].vertexData[vertexCount + i].position.y = textInfo.meshInfo[materialIndex].vertexData[vertexCount + i].position.y * -1f + vector.y;
		}
		float num6 = 1f / (float)m_Underline.fontAsset.atlasWidth;
		float num7 = 1f / (float)m_Underline.fontAsset.atlasHeight;
		float num8 = ((float)glyphRect.width * 0.5f + m_Padding) * num3 * num6;
		float num9 = ((float)glyphRect.x - m_Padding) * num6;
		float x5 = num9 + num8;
		float x6 = ((float)glyphRect.x + (float)glyphRect.width * 0.5f) * num6;
		float num10 = ((float)(glyphRect.x + glyphRect.width) + m_Padding) * num6;
		float x7 = num10 - num8;
		float y3 = ((float)glyphRect.y - m_Padding) * num7;
		float y4 = ((float)(glyphRect.y + glyphRect.height) + m_Padding) * num7;
		vertexData[vertexCount].uv0 = new Vector4(num9, y3);
		vertexData[1 + vertexCount].uv0 = new Vector4(num9, y4);
		vertexData[2 + vertexCount].uv0 = new Vector4(x5, y4);
		vertexData[3 + vertexCount].uv0 = new Vector4(x5, y3);
		vertexData[4 + vertexCount].uv0 = new Vector4(x6, y3);
		vertexData[5 + vertexCount].uv0 = new Vector4(x6, y4);
		vertexData[6 + vertexCount].uv0 = new Vector4(x6, y4);
		vertexData[7 + vertexCount].uv0 = new Vector4(x6, y3);
		vertexData[8 + vertexCount].uv0 = new Vector4(x7, y3);
		vertexData[9 + vertexCount].uv0 = new Vector4(x7, y4);
		vertexData[10 + vertexCount].uv0 = new Vector4(num10, y4);
		vertexData[11 + vertexCount].uv0 = new Vector4(num10, y3);
		float num11 = 0f;
		float num12 = 1f / num5;
		float x8 = (vertexData[vertexCount + 2].position.x - start.x) * num12;
		vertexData[vertexCount].uv2 = new Vector2(0f, 0f);
		vertexData[1 + vertexCount].uv2 = new Vector2(0f, 1f);
		vertexData[2 + vertexCount].uv2 = new Vector2(x8, 1f);
		vertexData[3 + vertexCount].uv2 = new Vector2(x8, 0f);
		num11 = (vertexData[vertexCount + 4].position.x - start.x) * num12;
		x8 = (vertexData[vertexCount + 6].position.x - start.x) * num12;
		vertexData[4 + vertexCount].uv2 = new Vector2(num11, 0f);
		vertexData[5 + vertexCount].uv2 = new Vector2(num11, 1f);
		vertexData[6 + vertexCount].uv2 = new Vector2(x8, 1f);
		vertexData[7 + vertexCount].uv2 = new Vector2(x8, 0f);
		num11 = (vertexData[vertexCount + 8].position.x - start.x) * num12;
		vertexData[8 + vertexCount].uv2 = new Vector2(num11, 0f);
		vertexData[9 + vertexCount].uv2 = new Vector2(num11, 1f);
		vertexData[10 + vertexCount].uv2 = new Vector2(1f, 1f);
		vertexData[11 + vertexCount].uv2 = new Vector2(1f, 0f);
		underlineColor.a = ((m_FontColor32.a < underlineColor.a) ? m_FontColor32.a : underlineColor.a);
		for (int j = 0; j < 12; j++)
		{
			vertexData[j + vertexCount].color = underlineColor;
		}
		textInfo.meshInfo[materialIndex].vertexCount += 12;
	}

	private void DrawTextHighlight(Vector3 start, Vector3 end, Color32 highlightColor, TextGenerationSettings generationSettings, TextInfo textInfo)
	{
		GetUnderlineSpecialCharacter(generationSettings);
		if (m_Underline.character == null)
		{
			if (generationSettings.textSettings.displayWarnings)
			{
				Debug.LogWarning("Unable to add highlight since the primary Font Asset doesn't contain the underline character.");
			}
			return;
		}
		int vertexCount = textInfo.meshInfo[m_CurrentMaterialIndex].vertexCount;
		int num = vertexCount + 4;
		if (num > textInfo.meshInfo[m_CurrentMaterialIndex].vertexBufferSize)
		{
			textInfo.meshInfo[m_CurrentMaterialIndex].ResizeMeshInfo(num / 4, generationSettings.isIMGUI);
		}
		TextCoreVertex[] vertexData = textInfo.meshInfo[m_CurrentMaterialIndex].vertexData;
		vertexData[vertexCount].position = start;
		vertexData[vertexCount + 1].position = new Vector3(start.x, end.y, 0f);
		vertexData[vertexCount + 2].position = end;
		vertexData[vertexCount + 3].position = new Vector3(end.x, start.y, 0f);
		Vector3 vector = default(Vector3);
		vector.x = 0f;
		vector.y = generationSettings.screenRect.height;
		vector.z = 0f;
		for (int i = 0; i < 4; i++)
		{
			vertexData[vertexCount + i].position.y = vertexData[vertexCount + i].position.y * -1f + vector.y;
		}
		int atlasWidth = m_Underline.fontAsset.atlasWidth;
		int atlasHeight = m_Underline.fontAsset.atlasHeight;
		GlyphRect glyphRect = m_Underline.character.glyph.glyphRect;
		Vector2 vector2 = new Vector2(((float)glyphRect.x + (float)glyphRect.width / 2f) / (float)atlasWidth, ((float)glyphRect.y + (float)glyphRect.height / 2f) / (float)atlasHeight);
		Vector2 vector3 = new Vector2(1f / (float)atlasWidth, 1f / (float)atlasHeight);
		vertexData[vertexCount].uv0 = vector2 - vector3;
		vertexData[1 + vertexCount].uv0 = vector2 + new Vector2(0f - vector3.x, vector3.y);
		vertexData[2 + vertexCount].uv0 = vector2 + vector3;
		vertexData[3 + vertexCount].uv0 = vector2 + new Vector2(vector3.x, 0f - vector3.y);
		Vector2 uv = new Vector2(0f, 1f);
		vertexData[vertexCount].uv2 = uv;
		vertexData[1 + vertexCount].uv2 = uv;
		vertexData[2 + vertexCount].uv2 = uv;
		vertexData[3 + vertexCount].uv2 = uv;
		highlightColor.a = ((m_FontColor32.a < highlightColor.a) ? m_FontColor32.a : highlightColor.a);
		vertexData[vertexCount].color = highlightColor;
		vertexData[1 + vertexCount].color = highlightColor;
		vertexData[2 + vertexCount].color = highlightColor;
		vertexData[3 + vertexCount].color = highlightColor;
		textInfo.meshInfo[m_CurrentMaterialIndex].vertexCount += 4;
	}

	private static void ClearMesh(bool updateMesh, TextInfo textInfo)
	{
		textInfo.ClearMeshInfo(updateMesh);
	}

	public void LayoutPhase(TextInfo textInfo, TextGenerationSettings generationSettings, float maxVisibleDescender)
	{
		int underlineVertexIndex = m_MaterialReferences[m_Underline.materialIndex].referenceCount * 4;
		textInfo.meshInfo[m_CurrentMaterialIndex].Clear(uploadChanges: false);
		Vector3 vector = Vector3.zero;
		Vector3[] rectTransformCorners = m_RectTransformCorners;
		switch (generationSettings.textAlignment)
		{
		case TextAlignment.TopLeft:
		case TextAlignment.TopCenter:
		case TextAlignment.TopRight:
		case TextAlignment.TopJustified:
		case TextAlignment.TopFlush:
		case TextAlignment.TopGeoAligned:
			vector = rectTransformCorners[1] + new Vector3(0f, 0f - m_MaxAscender, 0f);
			break;
		case TextAlignment.MiddleLeft:
		case TextAlignment.MiddleCenter:
		case TextAlignment.MiddleRight:
		case TextAlignment.MiddleJustified:
		case TextAlignment.MiddleFlush:
		case TextAlignment.MiddleGeoAligned:
			vector = (rectTransformCorners[0] + rectTransformCorners[1]) / 2f + new Vector3(0f, 0f - (m_MaxAscender + maxVisibleDescender) / 2f, 0f);
			break;
		case TextAlignment.BottomLeft:
		case TextAlignment.BottomCenter:
		case TextAlignment.BottomRight:
		case TextAlignment.BottomJustified:
		case TextAlignment.BottomFlush:
		case TextAlignment.BottomGeoAligned:
			vector = rectTransformCorners[0] + new Vector3(0f, 0f - maxVisibleDescender, 0f);
			break;
		case TextAlignment.BaselineLeft:
		case TextAlignment.BaselineCenter:
		case TextAlignment.BaselineRight:
		case TextAlignment.BaselineJustified:
		case TextAlignment.BaselineFlush:
		case TextAlignment.BaselineGeoAligned:
			vector = (rectTransformCorners[0] + rectTransformCorners[1]) / 2f + new Vector3(0f, 0f, 0f);
			break;
		case TextAlignment.MidlineLeft:
		case TextAlignment.MidlineCenter:
		case TextAlignment.MidlineRight:
		case TextAlignment.MidlineJustified:
		case TextAlignment.MidlineFlush:
		case TextAlignment.MidlineGeoAligned:
			vector = (rectTransformCorners[0] + rectTransformCorners[1]) / 2f + new Vector3(0f, 0f - (m_MeshExtents.max.y + m_MeshExtents.min.y) / 2f, 0f);
			break;
		case TextAlignment.CaplineLeft:
		case TextAlignment.CaplineCenter:
		case TextAlignment.CaplineRight:
		case TextAlignment.CaplineJustified:
		case TextAlignment.CaplineFlush:
		case TextAlignment.CaplineGeoAligned:
			vector = (rectTransformCorners[0] + rectTransformCorners[1]) / 2f + new Vector3(0f, 0f - m_MaxCapHeight / 2f, 0f);
			break;
		}
		Vector3 vector2 = Vector3.zero;
		Vector3 zero = Vector3.zero;
		int num = 0;
		int lineCount = 0;
		int num2 = 0;
		bool flag = false;
		bool flag2 = false;
		int num3 = 0;
		int num4 = 0;
		Color32 color = Color.white;
		Color32 underlineColor = Color.white;
		HighlightState highlightState = new HighlightState(new Color32(byte.MaxValue, byte.MaxValue, 0, 64), Offset.zero);
		float num5 = 0f;
		float num6 = 0f;
		float num7 = 0f;
		float num8 = 0f;
		float num9 = 0f;
		float num10 = 32767f;
		float num11 = 0f;
		float num12 = 0f;
		float b = 0f;
		bool flag3 = false;
		Vector3 start = Vector3.zero;
		Vector3 zero2 = Vector3.zero;
		bool flag4 = false;
		Vector3 start2 = Vector3.zero;
		Vector3 zero3 = Vector3.zero;
		bool flag5 = false;
		Vector3 start3 = Vector3.zero;
		Vector3 end = Vector3.zero;
		TextElementInfo[] textElementInfo = textInfo.textElementInfo;
		for (int i = 0; i < m_CharacterCount; i++)
		{
			FontAsset fontAsset = textElementInfo[i].fontAsset;
			char c = (char)textElementInfo[i].character;
			bool flag6 = char.IsWhiteSpace(c);
			int lineNumber = textElementInfo[i].lineNumber;
			LineInfo lineInfo = textInfo.lineInfo[lineNumber];
			lineCount = lineNumber + 1;
			TextAlignment alignment = lineInfo.alignment;
			switch (alignment)
			{
			case TextAlignment.TopLeft:
			case TextAlignment.MiddleLeft:
			case TextAlignment.BottomLeft:
			case TextAlignment.BaselineLeft:
			case TextAlignment.MidlineLeft:
			case TextAlignment.CaplineLeft:
				vector2 = (generationSettings.isRightToLeft ? new Vector3(0f - lineInfo.maxAdvance, 0f, 0f) : new Vector3(0f + lineInfo.marginLeft, 0f, 0f));
				break;
			case TextAlignment.TopCenter:
			case TextAlignment.MiddleCenter:
			case TextAlignment.BottomCenter:
			case TextAlignment.BaselineCenter:
			case TextAlignment.MidlineCenter:
			case TextAlignment.CaplineCenter:
				vector2 = new Vector3(lineInfo.marginLeft + lineInfo.width / 2f - lineInfo.maxAdvance / 2f, 0f, 0f);
				break;
			case TextAlignment.TopGeoAligned:
			case TextAlignment.MiddleGeoAligned:
			case TextAlignment.BottomGeoAligned:
			case TextAlignment.BaselineGeoAligned:
			case TextAlignment.MidlineGeoAligned:
			case TextAlignment.CaplineGeoAligned:
				vector2 = new Vector3(lineInfo.marginLeft + lineInfo.width / 2f - (lineInfo.lineExtents.min.x + lineInfo.lineExtents.max.x) / 2f, 0f, 0f);
				break;
			case TextAlignment.TopRight:
			case TextAlignment.MiddleRight:
			case TextAlignment.BottomRight:
			case TextAlignment.BaselineRight:
			case TextAlignment.MidlineRight:
			case TextAlignment.CaplineRight:
				vector2 = (generationSettings.isRightToLeft ? new Vector3(lineInfo.marginLeft + lineInfo.width, 0f, 0f) : new Vector3(lineInfo.marginLeft + lineInfo.width - lineInfo.maxAdvance, 0f, 0f));
				break;
			case TextAlignment.TopJustified:
			case TextAlignment.TopFlush:
			case TextAlignment.MiddleJustified:
			case TextAlignment.MiddleFlush:
			case TextAlignment.BottomJustified:
			case TextAlignment.BottomFlush:
			case TextAlignment.BaselineJustified:
			case TextAlignment.BaselineFlush:
			case TextAlignment.MidlineJustified:
			case TextAlignment.MidlineFlush:
			case TextAlignment.CaplineJustified:
			case TextAlignment.CaplineFlush:
			{
				if (i > lineInfo.lastVisibleCharacterIndex || c == '\n' || c == '\u00ad' || c == '\u200b' || c == '\u2060' || c == '\u0003')
				{
					break;
				}
				char c2 = (char)textElementInfo[lineInfo.lastCharacterIndex].character;
				bool flag7 = (alignment & (TextAlignment)16) == (TextAlignment)16;
				if ((!char.IsControl(c2) && lineNumber < m_LineNumber) || flag7 || lineInfo.maxAdvance > lineInfo.width)
				{
					if (lineNumber != num2 || i == 0 || i == 0)
					{
						vector2 = (generationSettings.isRightToLeft ? new Vector3(lineInfo.marginLeft + lineInfo.width, 0f, 0f) : new Vector3(lineInfo.marginLeft, 0f, 0f));
						flag = (char.IsSeparator(c) ? true : false);
						break;
					}
					float num13 = (generationSettings.isRightToLeft ? (lineInfo.width + lineInfo.maxAdvance) : (lineInfo.width - lineInfo.maxAdvance));
					int num14 = lineInfo.visibleCharacterCount - 1 + lineInfo.controlCharacterCount;
					int num15 = lineInfo.visibleSpaceCount - lineInfo.controlCharacterCount;
					if (flag)
					{
						num15--;
						num14++;
					}
					float num16 = ((num15 > 0) ? 0.4f : 1f);
					if (num15 < 1)
					{
						num15 = 1;
					}
					if (c != '\u00a0' && (c == '\t' || char.IsSeparator(c)))
					{
						if (!generationSettings.isRightToLeft)
						{
							vector2 += new Vector3(num13 * (1f - num16) / (float)num15, 0f, 0f);
						}
						else
						{
							vector2 -= new Vector3(num13 * (1f - num16) / (float)num15, 0f, 0f);
						}
					}
					else if (!generationSettings.isRightToLeft)
					{
						vector2 += new Vector3(num13 * num16 / (float)num14, 0f, 0f);
					}
					else
					{
						vector2 -= new Vector3(num13 * num16 / (float)num14, 0f, 0f);
					}
				}
				else
				{
					vector2 = (generationSettings.isRightToLeft ? new Vector3(lineInfo.marginLeft + lineInfo.width, 0f, 0f) : new Vector3(lineInfo.marginLeft, 0f, 0f));
				}
				break;
			}
			}
			zero = vector + vector2;
			zero = new Vector3(Round(zero.x), Round(zero.y));
			bool isVisible = textElementInfo[i].isVisible;
			if (isVisible)
			{
				TextElementType elementType = textElementInfo[i].elementType;
				switch (elementType)
				{
				case TextElementType.Character:
				{
					Extents lineExtents = lineInfo.lineExtents;
					textElementInfo[i].vertexBottomLeft.uv2.x = 0f;
					textElementInfo[i].vertexTopLeft.uv2.x = 0f;
					textElementInfo[i].vertexTopRight.uv2.x = 1f;
					textElementInfo[i].vertexBottomRight.uv2.x = 1f;
					textElementInfo[i].vertexBottomLeft.uv2.y = 0f;
					textElementInfo[i].vertexTopLeft.uv2.y = 1f;
					textElementInfo[i].vertexTopRight.uv2.y = 1f;
					textElementInfo[i].vertexBottomRight.uv2.y = 0f;
					num5 = textElementInfo[i].scale * (1f - m_CharWidthAdjDelta) * 1f;
					if (!textElementInfo[i].isUsingAlternateTypeface && (textElementInfo[i].style & FontStyles.Bold) == FontStyles.Bold)
					{
						num5 *= -1f;
					}
					textElementInfo[i].vertexBottomLeft.uv.w = num5;
					textElementInfo[i].vertexTopLeft.uv.w = num5;
					textElementInfo[i].vertexTopRight.uv.w = num5;
					textElementInfo[i].vertexBottomRight.uv.w = num5;
					textElementInfo[i].vertexBottomLeft.uv2.x = 1f;
					textElementInfo[i].vertexBottomLeft.uv2.y = num5;
					textElementInfo[i].vertexTopLeft.uv2.x = 1f;
					textElementInfo[i].vertexTopLeft.uv2.y = num5;
					textElementInfo[i].vertexTopRight.uv2.x = 1f;
					textElementInfo[i].vertexTopRight.uv2.y = num5;
					textElementInfo[i].vertexBottomRight.uv2.x = 1f;
					textElementInfo[i].vertexBottomRight.uv2.y = num5;
					break;
				}
				}
				if (i < 99999 && num < 99999 && lineNumber < 99999)
				{
					textElementInfo[i].vertexBottomLeft.position += zero;
					textElementInfo[i].vertexTopLeft.position += zero;
					textElementInfo[i].vertexTopRight.position += zero;
					textElementInfo[i].vertexBottomRight.position += zero;
				}
				else
				{
					textElementInfo[i].vertexBottomLeft.position = Vector3.zero;
					textElementInfo[i].vertexTopLeft.position = Vector3.zero;
					textElementInfo[i].vertexTopRight.position = Vector3.zero;
					textElementInfo[i].vertexBottomRight.position = Vector3.zero;
					textElementInfo[i].isVisible = false;
				}
				switch (elementType)
				{
				case TextElementType.Character:
					TextGeneratorUtilities.FillCharacterVertexBuffers(i, generationSettings.shouldConvertToLinearSpace, generationSettings, textInfo, NeedToRound);
					break;
				case TextElementType.Sprite:
					TextGeneratorUtilities.FillSpriteVertexBuffers(i, generationSettings.shouldConvertToLinearSpace, generationSettings, textInfo);
					break;
				}
			}
			textInfo.textElementInfo[i].bottomLeft += zero;
			textInfo.textElementInfo[i].topLeft += zero;
			textInfo.textElementInfo[i].topRight += zero;
			textInfo.textElementInfo[i].bottomRight += zero;
			textInfo.textElementInfo[i].origin += zero.x;
			textInfo.textElementInfo[i].xAdvance += zero.x;
			textInfo.textElementInfo[i].ascender += zero.y;
			textInfo.textElementInfo[i].descender += zero.y;
			textInfo.textElementInfo[i].baseLine += zero.y;
			if (isVisible)
			{
			}
			if (lineNumber != num2 || i == m_CharacterCount - 1)
			{
				if (lineNumber != num2)
				{
					int num17 = ((generationSettings.textWrappingMode == TextWrappingMode.PreserveWhitespace || generationSettings.textWrappingMode == TextWrappingMode.PreserveWhitespaceNoWrap) ? textInfo.lineInfo[num2].lastCharacterIndex : textInfo.lineInfo[num2].lastVisibleCharacterIndex);
					textInfo.lineInfo[num2].baseline += zero.y;
					textInfo.lineInfo[num2].ascender += zero.y;
					textInfo.lineInfo[num2].descender += zero.y;
					textInfo.lineInfo[num2].maxAdvance += zero.x;
					textInfo.lineInfo[num2].lineExtents.min = new Vector2(textInfo.textElementInfo[textInfo.lineInfo[num2].firstCharacterIndex].bottomLeft.x, textInfo.lineInfo[num2].descender);
					textInfo.lineInfo[num2].lineExtents.max = new Vector2(textInfo.textElementInfo[num17].topRight.x, textInfo.lineInfo[num2].ascender);
				}
				if (i == m_CharacterCount - 1)
				{
					int num18 = ((generationSettings.textWrappingMode == TextWrappingMode.PreserveWhitespace || generationSettings.textWrappingMode == TextWrappingMode.PreserveWhitespaceNoWrap) ? textInfo.lineInfo[lineNumber].lastCharacterIndex : textInfo.lineInfo[lineNumber].lastVisibleCharacterIndex);
					textInfo.lineInfo[lineNumber].baseline += zero.y;
					textInfo.lineInfo[lineNumber].ascender += zero.y;
					textInfo.lineInfo[lineNumber].descender += zero.y;
					textInfo.lineInfo[lineNumber].maxAdvance += zero.x;
					textInfo.lineInfo[lineNumber].lineExtents.min = new Vector2(textInfo.textElementInfo[textInfo.lineInfo[lineNumber].firstCharacterIndex].bottomLeft.x, textInfo.lineInfo[lineNumber].descender);
					textInfo.lineInfo[lineNumber].lineExtents.max = new Vector2(textInfo.textElementInfo[num18].topRight.x, textInfo.lineInfo[lineNumber].ascender);
				}
			}
			if (char.IsLetterOrDigit(c) || c == '-' || c == '\u00ad' || c == '‐' || c == '‑')
			{
				if (!flag2)
				{
					flag2 = true;
					num3 = i;
				}
				if (flag2 && i == m_CharacterCount - 1)
				{
					int num19 = textInfo.wordInfo.Length;
					int wordCount = textInfo.wordCount;
					if (textInfo.wordCount + 1 > num19)
					{
						TextInfo.Resize(ref textInfo.wordInfo, num19 + 1);
					}
					num4 = i;
					textInfo.wordInfo[wordCount].firstCharacterIndex = num3;
					textInfo.wordInfo[wordCount].lastCharacterIndex = num4;
					textInfo.wordInfo[wordCount].characterCount = num4 - num3 + 1;
					num++;
					textInfo.wordCount++;
					textInfo.lineInfo[lineNumber].wordCount++;
				}
			}
			else if ((flag2 || (i == 0 && (!char.IsPunctuation(c) || flag6 || c == '\u200b' || i == m_CharacterCount - 1))) && (i <= 0 || i >= textElementInfo.Length - 1 || i >= m_CharacterCount || (c != '\'' && c != '’') || !char.IsLetterOrDigit((char)textElementInfo[i - 1].character) || !char.IsLetterOrDigit((char)textElementInfo[i + 1].character)))
			{
				num4 = ((i == m_CharacterCount - 1 && char.IsLetterOrDigit(c)) ? i : (i - 1));
				flag2 = false;
				int num20 = textInfo.wordInfo.Length;
				int wordCount2 = textInfo.wordCount;
				if (textInfo.wordCount + 1 > num20)
				{
					TextInfo.Resize(ref textInfo.wordInfo, num20 + 1);
				}
				textInfo.wordInfo[wordCount2].firstCharacterIndex = num3;
				textInfo.wordInfo[wordCount2].lastCharacterIndex = num4;
				textInfo.wordInfo[wordCount2].characterCount = num4 - num3 + 1;
				num++;
				textInfo.wordCount++;
				textInfo.lineInfo[lineNumber].wordCount++;
			}
			if ((textInfo.textElementInfo[i].style & FontStyles.Underline) == FontStyles.Underline)
			{
				bool flag8 = true;
				textInfo.textElementInfo[i].underlineVertexIndex = underlineVertexIndex;
				if (i > 99999 || lineNumber > 99999)
				{
					flag8 = false;
				}
				if (!flag6 && c != '\u200b')
				{
					num9 = Mathf.Max(num9, textInfo.textElementInfo[i].scale);
					num6 = Mathf.Max(num6, Mathf.Abs(num5));
					num10 = Mathf.Min(num10, textInfo.textElementInfo[i].baseLine + fontAsset.faceInfo.underlineOffset * num9);
				}
				if (!flag3 && flag8 && i <= lineInfo.lastVisibleCharacterIndex && c != '\n' && c != '\v' && c != '\r' && (i != lineInfo.lastVisibleCharacterIndex || !char.IsSeparator(c)))
				{
					flag3 = true;
					num7 = textInfo.textElementInfo[i].scale;
					if (num9 == 0f)
					{
						num9 = num7;
						num6 = num5;
					}
					start = new Vector3(textInfo.textElementInfo[i].bottomLeft.x, num10, 0f);
					color = textInfo.textElementInfo[i].underlineColor;
				}
				if (flag3 && m_CharacterCount == 1)
				{
					flag3 = false;
					zero2 = new Vector3(textInfo.textElementInfo[i].topRight.x, num10, 0f);
					num8 = textInfo.textElementInfo[i].scale;
					DrawUnderlineMesh(start, zero2, num7, num8, num9, num6, color, generationSettings, textInfo);
					num9 = 0f;
					num6 = 0f;
					num10 = 32767f;
				}
				else if (flag3 && (i == lineInfo.lastCharacterIndex || i >= lineInfo.lastVisibleCharacterIndex))
				{
					if (flag6 || c == '\u200b')
					{
						int lastVisibleCharacterIndex = lineInfo.lastVisibleCharacterIndex;
						zero2 = new Vector3(textInfo.textElementInfo[lastVisibleCharacterIndex].topRight.x, num10, 0f);
						num8 = textInfo.textElementInfo[lastVisibleCharacterIndex].scale;
					}
					else
					{
						zero2 = new Vector3(textInfo.textElementInfo[i].topRight.x, num10, 0f);
						num8 = textInfo.textElementInfo[i].scale;
					}
					flag3 = false;
					DrawUnderlineMesh(start, zero2, num7, num8, num9, num6, color, generationSettings, textInfo);
					num9 = 0f;
					num6 = 0f;
					num10 = 32767f;
				}
				else if (flag3 && !flag8)
				{
					flag3 = false;
					zero2 = new Vector3(textInfo.textElementInfo[i - 1].topRight.x, num10, 0f);
					num8 = textInfo.textElementInfo[i - 1].scale;
					DrawUnderlineMesh(start, zero2, num7, num8, num9, num6, color, generationSettings, textInfo);
					num9 = 0f;
					num6 = 0f;
					num10 = 32767f;
				}
				else if (flag3 && i < m_CharacterCount - 1 && !ColorUtilities.CompareColors(color, textInfo.textElementInfo[i + 1].underlineColor))
				{
					flag3 = false;
					zero2 = new Vector3(textInfo.textElementInfo[i].topRight.x, num10, 0f);
					num8 = textInfo.textElementInfo[i].scale;
					DrawUnderlineMesh(start, zero2, num7, num8, num9, num6, color, generationSettings, textInfo);
					num9 = 0f;
					num6 = 0f;
					num10 = 32767f;
				}
			}
			else if (flag3)
			{
				flag3 = false;
				zero2 = new Vector3(textInfo.textElementInfo[i - 1].topRight.x, num10, 0f);
				num8 = textInfo.textElementInfo[i - 1].scale;
				DrawUnderlineMesh(start, zero2, num7, num8, num9, num6, color, generationSettings, textInfo);
				num9 = 0f;
				num6 = 0f;
				num10 = 32767f;
			}
			bool flag9 = (textInfo.textElementInfo[i].style & FontStyles.Strikethrough) == FontStyles.Strikethrough;
			float strikethroughOffset = fontAsset.faceInfo.strikethroughOffset;
			if (flag9)
			{
				bool flag10 = true;
				textInfo.textElementInfo[i].strikethroughVertexIndex = m_MaterialReferences[m_Underline.materialIndex].referenceCount * 4;
				if (i > 99999 || lineNumber > 99999)
				{
					flag10 = false;
				}
				if (!flag4 && flag10 && i <= lineInfo.lastVisibleCharacterIndex && c != '\n' && c != '\v' && c != '\r' && (i != lineInfo.lastVisibleCharacterIndex || !char.IsSeparator(c)))
				{
					flag4 = true;
					num11 = textInfo.textElementInfo[i].pointSize;
					num12 = textInfo.textElementInfo[i].scale;
					start2 = new Vector3(textInfo.textElementInfo[i].bottomLeft.x, textInfo.textElementInfo[i].baseLine + strikethroughOffset * num12, 0f);
					underlineColor = textInfo.textElementInfo[i].strikethroughColor;
					b = textInfo.textElementInfo[i].baseLine;
				}
				if (flag4 && m_CharacterCount == 1)
				{
					flag4 = false;
					zero3 = new Vector3(textInfo.textElementInfo[i].topRight.x, textInfo.textElementInfo[i].baseLine + strikethroughOffset * num12, 0f);
					DrawUnderlineMesh(start2, zero3, num12, num12, num12, num5, underlineColor, generationSettings, textInfo);
				}
				else if (flag4 && i == lineInfo.lastCharacterIndex)
				{
					if (flag6 || c == '\u200b')
					{
						int lastVisibleCharacterIndex2 = lineInfo.lastVisibleCharacterIndex;
						zero3 = new Vector3(textInfo.textElementInfo[lastVisibleCharacterIndex2].topRight.x, textInfo.textElementInfo[lastVisibleCharacterIndex2].baseLine + strikethroughOffset * num12, 0f);
					}
					else
					{
						zero3 = new Vector3(textInfo.textElementInfo[i].topRight.x, textInfo.textElementInfo[i].baseLine + strikethroughOffset * num12, 0f);
					}
					flag4 = false;
					DrawUnderlineMesh(start2, zero3, num12, num12, num12, num5, underlineColor, generationSettings, textInfo);
				}
				else if (flag4 && i < m_CharacterCount && (textInfo.textElementInfo[i + 1].pointSize != num11 || !TextGeneratorUtilities.Approximately(textInfo.textElementInfo[i + 1].baseLine + zero.y, b)))
				{
					flag4 = false;
					int lastVisibleCharacterIndex3 = lineInfo.lastVisibleCharacterIndex;
					zero3 = ((i <= lastVisibleCharacterIndex3) ? new Vector3(textInfo.textElementInfo[i].topRight.x, textInfo.textElementInfo[i].baseLine + strikethroughOffset * num12, 0f) : new Vector3(textInfo.textElementInfo[lastVisibleCharacterIndex3].topRight.x, textInfo.textElementInfo[lastVisibleCharacterIndex3].baseLine + strikethroughOffset * num12, 0f));
					DrawUnderlineMesh(start2, zero3, num12, num12, num12, num5, underlineColor, generationSettings, textInfo);
				}
				else if (flag4 && i < m_CharacterCount && fontAsset.GetHashCode() != textElementInfo[i + 1].fontAsset.GetHashCode())
				{
					flag4 = false;
					zero3 = new Vector3(textInfo.textElementInfo[i].topRight.x, textInfo.textElementInfo[i].baseLine + strikethroughOffset * num12, 0f);
					DrawUnderlineMesh(start2, zero3, num12, num12, num12, num5, underlineColor, generationSettings, textInfo);
				}
				else if (flag4 && !flag10)
				{
					flag4 = false;
					zero3 = new Vector3(textInfo.textElementInfo[i - 1].topRight.x, textInfo.textElementInfo[i - 1].baseLine + strikethroughOffset * num12, 0f);
					DrawUnderlineMesh(start2, zero3, num12, num12, num12, num5, underlineColor, generationSettings, textInfo);
				}
			}
			else if (flag4)
			{
				flag4 = false;
				zero3 = new Vector3(textInfo.textElementInfo[i - 1].topRight.x, textInfo.textElementInfo[i - 1].baseLine + strikethroughOffset * num12, 0f);
				DrawUnderlineMesh(start2, zero3, num12, num12, num12, num5, underlineColor, generationSettings, textInfo);
			}
			if ((textInfo.textElementInfo[i].style & FontStyles.Highlight) == FontStyles.Highlight)
			{
				bool flag11 = true;
				if (i > 99999 || lineNumber > 99999)
				{
					flag11 = false;
				}
				if (!flag5 && flag11 && i <= lineInfo.lastVisibleCharacterIndex && c != '\n' && c != '\v' && c != '\r' && (i != lineInfo.lastVisibleCharacterIndex || !char.IsSeparator(c)))
				{
					flag5 = true;
					start3 = TextGeneratorUtilities.largePositiveVector2;
					end = TextGeneratorUtilities.largeNegativeVector2;
					highlightState = textInfo.textElementInfo[i].highlightState;
				}
				if (flag5)
				{
					TextElementInfo textElementInfo2 = textInfo.textElementInfo[i];
					HighlightState highlightState2 = textElementInfo2.highlightState;
					bool flag12 = false;
					if (highlightState != highlightState2)
					{
						if (flag6)
						{
							end.x = (end.x - highlightState.padding.right + textElementInfo2.origin) / 2f;
						}
						else
						{
							end.x = (end.x - highlightState.padding.right + textElementInfo2.bottomLeft.x) / 2f;
						}
						start3.y = Mathf.Min(start3.y, textElementInfo2.descender);
						end.y = Mathf.Max(end.y, textElementInfo2.ascender);
						DrawTextHighlight(start3, end, highlightState.color, generationSettings, textInfo);
						flag5 = true;
						start3 = new Vector2(end.x, textElementInfo2.descender - highlightState2.padding.bottom);
						end = ((!flag6) ? ((Vector3)new Vector2(textElementInfo2.topRight.x + highlightState2.padding.right, textElementInfo2.ascender + highlightState2.padding.top)) : ((Vector3)new Vector2(textElementInfo2.xAdvance + highlightState2.padding.right, textElementInfo2.ascender + highlightState2.padding.top)));
						highlightState = highlightState2;
						flag12 = true;
					}
					if (!flag12)
					{
						if (flag6)
						{
							start3.x = Mathf.Min(start3.x, textElementInfo2.origin - highlightState.padding.left);
							end.x = Mathf.Max(end.x, textElementInfo2.xAdvance + highlightState.padding.right);
						}
						else
						{
							start3.x = Mathf.Min(start3.x, textElementInfo2.bottomLeft.x - highlightState.padding.left);
							end.x = Mathf.Max(end.x, textElementInfo2.topRight.x + highlightState.padding.right);
						}
						start3.y = Mathf.Min(start3.y, textElementInfo2.descender - highlightState.padding.bottom);
						end.y = Mathf.Max(end.y, textElementInfo2.ascender + highlightState.padding.top);
					}
				}
				if (flag5 && m_CharacterCount == 1)
				{
					flag5 = false;
					DrawTextHighlight(start3, end, highlightState.color, generationSettings, textInfo);
				}
				else if (flag5 && (i == lineInfo.lastCharacterIndex || i >= lineInfo.lastVisibleCharacterIndex))
				{
					flag5 = false;
					DrawTextHighlight(start3, end, highlightState.color, generationSettings, textInfo);
				}
				else if (flag5 && !flag11)
				{
					flag5 = false;
					DrawTextHighlight(start3, end, highlightState.color, generationSettings, textInfo);
				}
			}
			else if (flag5)
			{
				flag5 = false;
				DrawTextHighlight(start3, end, highlightState.color, generationSettings, textInfo);
			}
			num2 = lineNumber;
		}
		textInfo.characterCount = m_CharacterCount;
		textInfo.spriteCount = m_SpriteCount;
		textInfo.lineCount = lineCount;
		textInfo.wordCount = ((num == 0 || m_CharacterCount <= 0) ? 1 : num);
	}

	private float Round(float v)
	{
		if (!NeedToRound)
		{
			return v;
		}
		return Mathf.Floor(v + 0.48f);
	}

	public void ParsingPhase(TextInfo textInfo, TextGenerationSettings generationSettings, out uint charCode, out float maxVisibleDescender)
	{
		TextSettings textSettings = generationSettings.textSettings;
		m_CurrentMaterial = generationSettings.fontAsset.material;
		m_CurrentMaterialIndex = 0;
		m_MaterialReferenceStack.SetDefault(new MaterialReference(m_CurrentMaterialIndex, m_CurrentFontAsset, null, m_CurrentMaterial, m_Padding));
		m_CurrentSpriteAsset = null;
		int totalCharacterCount = m_TotalCharacterCount;
		float num = m_FontSize / generationSettings.fontAsset.m_FaceInfo.pointSize * generationSettings.fontAsset.m_FaceInfo.scale;
		float num2 = num;
		float num3 = m_FontSize * 0.01f;
		m_FontScaleMultiplier = 1f;
		m_ShouldRenderBitmap = generationSettings.fontAsset.IsBitmap();
		m_CurrentFontSize = m_FontSize;
		m_SizeStack.SetDefault(m_CurrentFontSize);
		charCode = 0u;
		m_FontStyleInternal = generationSettings.fontStyle;
		m_FontWeightInternal = (((m_FontStyleInternal & FontStyles.Bold) == FontStyles.Bold) ? TextFontWeight.Bold : generationSettings.fontWeight);
		m_FontWeightStack.SetDefault(m_FontWeightInternal);
		m_FontStyleStack.Clear();
		m_LineJustification = generationSettings.textAlignment;
		m_LineJustificationStack.SetDefault(m_LineJustification);
		float num4 = 0f;
		m_BaselineOffset = 0f;
		m_BaselineOffsetStack.Clear();
		m_FontColor32 = generationSettings.color;
		m_HtmlColor = m_FontColor32;
		m_UnderlineColor = m_HtmlColor;
		m_StrikethroughColor = m_HtmlColor;
		m_ColorStack.SetDefault(m_HtmlColor);
		m_UnderlineColorStack.SetDefault(m_HtmlColor);
		m_StrikethroughColorStack.SetDefault(m_HtmlColor);
		m_HighlightStateStack.SetDefault(new HighlightState(m_HtmlColor, Offset.zero));
		m_ColorGradientPreset = null;
		m_ColorGradientStack.SetDefault(null);
		m_ItalicAngle = m_CurrentFontAsset.italicStyleSlant;
		m_ItalicAngleStack.SetDefault(m_ItalicAngle);
		m_ActionStack.Clear();
		m_FXScale = Vector3.one;
		m_FXRotation = Quaternion.identity;
		m_LineOffset = 0f;
		m_LineHeight = -32767f;
		float num5 = Round(m_CurrentFontAsset.faceInfo.lineHeight - (m_CurrentFontAsset.m_FaceInfo.ascentLine - m_CurrentFontAsset.m_FaceInfo.descentLine));
		m_CSpacing = 0f;
		m_MonoSpacing = 0f;
		m_XAdvance = 0f;
		m_TagLineIndent = 0f;
		m_TagIndent = 0f;
		m_IndentStack.SetDefault(0f);
		m_TagNoParsing = false;
		m_CharacterCount = 0;
		m_FirstCharacterOfLine = 0;
		m_LastCharacterOfLine = 0;
		m_FirstVisibleCharacterOfLine = 0;
		m_LastVisibleCharacterOfLine = 0;
		m_MaxLineAscender = -32767f;
		m_MaxLineDescender = 32767f;
		m_LineNumber = 0;
		m_StartOfLineAscender = 0f;
		m_LineVisibleCharacterCount = 0;
		m_LineVisibleSpaceCount = 0;
		bool flag = true;
		m_IsDrivenLineSpacing = false;
		m_FirstOverflowCharacterIndex = -1;
		m_LastBaseGlyphIndex = int.MinValue;
		bool flag2 = TextGenerationSettings.fontFeatures.Contains(OTL_FeatureTag.kern);
		bool flag3 = TextGenerationSettings.fontFeatures.Contains(OTL_FeatureTag.mark);
		bool flag4 = TextGenerationSettings.fontFeatures.Contains(OTL_FeatureTag.mkmk);
		float num6 = ((m_MarginWidth > 0f) ? m_MarginWidth : 0f);
		float num7 = ((m_MarginHeight > 0f) ? m_MarginHeight : 0f);
		m_MarginLeft = 0f;
		m_MarginRight = 0f;
		m_Width = -1f;
		float num8 = num6 + 0.0001f - m_MarginLeft - m_MarginRight;
		m_MeshExtents.min = TextGeneratorUtilities.largePositiveVector2;
		m_MeshExtents.max = TextGeneratorUtilities.largeNegativeVector2;
		textInfo.ClearLineInfo();
		m_MaxCapHeight = 0f;
		m_MaxAscender = 0f;
		m_MaxDescender = 0f;
		m_PageAscender = 0f;
		maxVisibleDescender = 0f;
		bool isMaxVisibleDescenderSet = false;
		bool flag5 = true;
		m_IsNonBreakingSpace = false;
		bool flag6 = false;
		int num9 = 0;
		CharacterSubstitution characterSubstitution = new CharacterSubstitution(-1, 0u);
		bool flag7 = false;
		TextWrappingMode textWrappingMode = generationSettings.textWrappingMode;
		SaveWordWrappingState(ref m_SavedWordWrapState, -1, -1, textInfo);
		SaveWordWrappingState(ref m_SavedLineState, -1, -1, textInfo);
		SaveWordWrappingState(ref m_SavedEllipsisState, -1, -1, textInfo);
		SaveWordWrappingState(ref m_SavedLastValidState, -1, -1, textInfo);
		SaveWordWrappingState(ref m_SavedSoftLineBreakState, -1, -1, textInfo);
		m_EllipsisInsertionCandidateStack.Clear();
		m_IsTextTruncated = false;
		int num10 = 0;
		Vector3 vector = default(Vector3);
		Vector3 vector2 = default(Vector3);
		Vector3 vector3 = default(Vector3);
		Vector3 vector4 = default(Vector3);
		for (int i = 0; i < m_TextProcessingArray.Length && m_TextProcessingArray[i].unicode != 0; i++)
		{
			charCode = m_TextProcessingArray[i].unicode;
			if (num10 > 5)
			{
				Debug.LogError("Line breaking recursion max threshold hit... Character [" + charCode + "] index: " + i);
				characterSubstitution.index = m_CharacterCount;
				characterSubstitution.unicode = 3u;
			}
			if (charCode == 26)
			{
				continue;
			}
			if (generationSettings.richText && charCode == 60)
			{
				m_isTextLayoutPhase = true;
				m_TextElementType = TextElementType.Character;
				if (ValidateHtmlTag(m_TextProcessingArray, i + 1, out var endIndex, generationSettings, textInfo, out var _))
				{
					i = endIndex;
					if (m_TextElementType == TextElementType.Character)
					{
						continue;
					}
				}
			}
			else
			{
				m_TextElementType = textInfo.textElementInfo[m_CharacterCount].elementType;
				m_CurrentMaterialIndex = textInfo.textElementInfo[m_CharacterCount].materialReferenceIndex;
				m_CurrentFontAsset = textInfo.textElementInfo[m_CharacterCount].fontAsset;
			}
			int currentMaterialIndex = m_CurrentMaterialIndex;
			bool isUsingAlternateTypeface = textInfo.textElementInfo[m_CharacterCount].isUsingAlternateTypeface;
			m_isTextLayoutPhase = false;
			bool flag8 = false;
			if (characterSubstitution.index == m_CharacterCount)
			{
				charCode = characterSubstitution.unicode;
				m_TextElementType = TextElementType.Character;
				flag8 = true;
				switch (charCode)
				{
				case 3u:
					textInfo.textElementInfo[m_CharacterCount].textElement = m_CurrentFontAsset.characterLookupTable[3u];
					m_IsTextTruncated = true;
					break;
				case 8230u:
					textInfo.textElementInfo[m_CharacterCount].textElement = m_Ellipsis.character;
					textInfo.textElementInfo[m_CharacterCount].elementType = TextElementType.Character;
					textInfo.textElementInfo[m_CharacterCount].fontAsset = m_Ellipsis.fontAsset;
					textInfo.textElementInfo[m_CharacterCount].material = m_Ellipsis.material;
					textInfo.textElementInfo[m_CharacterCount].materialReferenceIndex = m_Ellipsis.materialIndex;
					m_MaterialReferences[m_Underline.materialIndex].referenceCount++;
					m_IsTextTruncated = true;
					characterSubstitution.index = m_CharacterCount + 1;
					characterSubstitution.unicode = 3u;
					break;
				}
			}
			if (m_CharacterCount < 0 && charCode != 3)
			{
				textInfo.textElementInfo[m_CharacterCount].isVisible = false;
				textInfo.textElementInfo[m_CharacterCount].character = 8203u;
				textInfo.textElementInfo[m_CharacterCount].lineNumber = 0;
				m_CharacterCount++;
				continue;
			}
			float num11 = 1f;
			if (m_TextElementType == TextElementType.Character)
			{
				if ((m_FontStyleInternal & FontStyles.UpperCase) == FontStyles.UpperCase)
				{
					if (char.IsLower((char)charCode))
					{
						charCode = char.ToUpper((char)charCode);
					}
				}
				else if ((m_FontStyleInternal & FontStyles.LowerCase) == FontStyles.LowerCase)
				{
					if (char.IsUpper((char)charCode))
					{
						charCode = char.ToLower((char)charCode);
					}
				}
				else if ((m_FontStyleInternal & FontStyles.SmallCaps) == FontStyles.SmallCaps && char.IsLower((char)charCode))
				{
					num11 = 0.8f;
					charCode = char.ToUpper((char)charCode);
				}
			}
			float num12 = 0f;
			float num13 = 0f;
			float num14 = 0f;
			if (m_TextElementType == TextElementType.Sprite)
			{
				SpriteCharacter spriteCharacter = (SpriteCharacter)textInfo.textElementInfo[m_CharacterCount].textElement;
				m_CurrentSpriteAsset = spriteCharacter.textAsset as SpriteAsset;
				m_SpriteIndex = (int)spriteCharacter.glyphIndex;
				if (charCode == 60)
				{
					charCode = (uint)(57344 + m_SpriteIndex);
				}
				else
				{
					m_SpriteColor = Color.white;
				}
				float num15 = m_CurrentFontSize / m_CurrentFontAsset.faceInfo.pointSize * m_CurrentFontAsset.faceInfo.scale;
				if (m_CurrentSpriteAsset.m_FaceInfo.pointSize > 0f)
				{
					float num16 = m_CurrentFontSize / m_CurrentSpriteAsset.m_FaceInfo.pointSize * m_CurrentSpriteAsset.m_FaceInfo.scale;
					num2 = spriteCharacter.m_Scale * spriteCharacter.m_Glyph.scale * num16;
					num13 = m_CurrentSpriteAsset.m_FaceInfo.ascentLine;
					num12 = m_CurrentSpriteAsset.m_FaceInfo.baseline * num15 * m_FontScaleMultiplier * m_CurrentSpriteAsset.m_FaceInfo.scale;
					num14 = m_CurrentSpriteAsset.m_FaceInfo.descentLine;
				}
				else
				{
					float num17 = m_CurrentFontSize / m_CurrentFontAsset.m_FaceInfo.pointSize * m_CurrentFontAsset.m_FaceInfo.scale;
					num2 = m_CurrentFontAsset.m_FaceInfo.ascentLine / spriteCharacter.m_Glyph.metrics.height * spriteCharacter.m_Scale * spriteCharacter.m_Glyph.scale * num17;
					float num18 = num17 / num2;
					num13 = m_CurrentFontAsset.m_FaceInfo.ascentLine * num18;
					num12 = m_CurrentFontAsset.m_FaceInfo.baseline * num15 * m_FontScaleMultiplier * m_CurrentFontAsset.m_FaceInfo.scale;
					num14 = m_CurrentFontAsset.m_FaceInfo.descentLine * num18;
				}
				m_CachedTextElement = spriteCharacter;
				textInfo.textElementInfo[m_CharacterCount].elementType = TextElementType.Sprite;
				textInfo.textElementInfo[m_CharacterCount].scale = num2;
				textInfo.textElementInfo[m_CharacterCount].spriteAsset = m_CurrentSpriteAsset;
				textInfo.textElementInfo[m_CharacterCount].fontAsset = m_CurrentFontAsset;
				textInfo.textElementInfo[m_CharacterCount].materialReferenceIndex = m_CurrentMaterialIndex;
				m_CurrentMaterialIndex = currentMaterialIndex;
				num4 = 0f;
			}
			else if (m_TextElementType == TextElementType.Character)
			{
				m_CachedTextElement = textInfo.textElementInfo[m_CharacterCount].textElement;
				if (m_CachedTextElement == null)
				{
					continue;
				}
				m_CurrentFontAsset = textInfo.textElementInfo[m_CharacterCount].fontAsset;
				m_CurrentMaterial = textInfo.textElementInfo[m_CharacterCount].material;
				m_CurrentMaterialIndex = textInfo.textElementInfo[m_CharacterCount].materialReferenceIndex;
				float num19 = ((!flag8 || m_TextProcessingArray[i].unicode != 10 || m_CharacterCount == m_FirstCharacterOfLine) ? (m_CurrentFontSize * num11 / m_CurrentFontAsset.m_FaceInfo.pointSize * m_CurrentFontAsset.m_FaceInfo.scale) : (textInfo.textElementInfo[m_CharacterCount - 1].pointSize * num11 / m_CurrentFontAsset.m_FaceInfo.pointSize * m_CurrentFontAsset.m_FaceInfo.scale));
				if (flag8 && charCode == 8230)
				{
					num13 = 0f;
					num14 = 0f;
				}
				else
				{
					num13 = m_CurrentFontAsset.m_FaceInfo.ascentLine;
					num14 = m_CurrentFontAsset.m_FaceInfo.descentLine;
				}
				num2 = num19 * m_FontScaleMultiplier * m_CachedTextElement.m_Scale * m_CachedTextElement.m_Glyph.scale;
				num12 = Round(m_CurrentFontAsset.m_FaceInfo.baseline * num19 * m_FontScaleMultiplier * m_CurrentFontAsset.m_FaceInfo.scale);
				textInfo.textElementInfo[m_CharacterCount].elementType = TextElementType.Character;
				textInfo.textElementInfo[m_CharacterCount].scale = num2;
				num4 = m_Padding;
			}
			float num20 = num2;
			if (charCode == 173 || charCode == 3)
			{
				num2 = 0f;
			}
			textInfo.textElementInfo[m_CharacterCount].character = charCode;
			textInfo.textElementInfo[m_CharacterCount].pointSize = m_CurrentFontSize;
			textInfo.textElementInfo[m_CharacterCount].color = m_HtmlColor;
			textInfo.textElementInfo[m_CharacterCount].underlineColor = m_UnderlineColor;
			textInfo.textElementInfo[m_CharacterCount].strikethroughColor = m_StrikethroughColor;
			textInfo.textElementInfo[m_CharacterCount].highlightState = m_HighlightState;
			textInfo.textElementInfo[m_CharacterCount].style = m_FontStyleInternal;
			if (m_FontWeightInternal == TextFontWeight.Bold)
			{
				textInfo.textElementInfo[m_CharacterCount].style |= FontStyles.Bold;
			}
			GlyphMetrics glyphMetrics = textInfo.textElementInfo[m_CharacterCount].alternativeGlyph?.metrics ?? m_CachedTextElement.m_Glyph.metrics;
			bool flag9 = charCode <= 65535 && char.IsWhiteSpace((char)charCode);
			GlyphValueRecord glyphValueRecord = default(GlyphValueRecord);
			float num21 = generationSettings.characterSpacing;
			if (flag2 && m_TextElementType == TextElementType.Character)
			{
				uint glyphIndex = m_CachedTextElement.m_GlyphIndex;
				GlyphPairAdjustmentRecord value;
				if (m_CharacterCount < totalCharacterCount - 1 && textInfo.textElementInfo[m_CharacterCount + 1].elementType == TextElementType.Character)
				{
					uint glyphIndex2 = textInfo.textElementInfo[m_CharacterCount + 1].textElement.m_GlyphIndex;
					uint key = (glyphIndex2 << 16) | glyphIndex;
					if (m_CurrentFontAsset.m_FontFeatureTable.m_GlyphPairAdjustmentRecordLookup.TryGetValue(key, out value))
					{
						glyphValueRecord = value.firstAdjustmentRecord.glyphValueRecord;
						num21 = (((value.featureLookupFlags & FontFeatureLookupFlags.IgnoreSpacingAdjustments) == FontFeatureLookupFlags.IgnoreSpacingAdjustments) ? 0f : num21);
					}
				}
				if (m_CharacterCount >= 1)
				{
					uint glyphIndex3 = textInfo.textElementInfo[m_CharacterCount - 1].textElement.m_GlyphIndex;
					uint key2 = (glyphIndex << 16) | glyphIndex3;
					if (textInfo.textElementInfo[m_CharacterCount - 1].elementType == TextElementType.Character && m_CurrentFontAsset.m_FontFeatureTable.m_GlyphPairAdjustmentRecordLookup.TryGetValue(key2, out value))
					{
						glyphValueRecord += value.secondAdjustmentRecord.glyphValueRecord;
						num21 = (((value.featureLookupFlags & FontFeatureLookupFlags.IgnoreSpacingAdjustments) == FontFeatureLookupFlags.IgnoreSpacingAdjustments) ? 0f : num21);
					}
				}
				textInfo.textElementInfo[m_CharacterCount].adjustedHorizontalAdvance = glyphValueRecord.xAdvance;
			}
			bool flag10 = TextGeneratorUtilities.IsBaseGlyph(charCode);
			if (flag10)
			{
				m_LastBaseGlyphIndex = m_CharacterCount;
			}
			if (m_CharacterCount > 0 && !flag10)
			{
				if (flag3 && m_LastBaseGlyphIndex != int.MinValue && m_LastBaseGlyphIndex == m_CharacterCount - 1)
				{
					Glyph glyph = textInfo.textElementInfo[m_LastBaseGlyphIndex].textElement.glyph;
					uint index = glyph.index;
					uint glyphIndex4 = m_CachedTextElement.glyphIndex;
					uint key3 = (glyphIndex4 << 16) | index;
					if (m_CurrentFontAsset.fontFeatureTable.m_MarkToBaseAdjustmentRecordLookup.TryGetValue(key3, out var value2))
					{
						float num22 = (textInfo.textElementInfo[m_LastBaseGlyphIndex].origin - m_XAdvance) / num2;
						glyphValueRecord.xPlacement = num22 + value2.baseGlyphAnchorPoint.xCoordinate - value2.markPositionAdjustment.xPositionAdjustment;
						glyphValueRecord.yPlacement = value2.baseGlyphAnchorPoint.yCoordinate - value2.markPositionAdjustment.yPositionAdjustment;
						num21 = 0f;
					}
				}
				else
				{
					bool flag11 = false;
					if (flag4)
					{
						int num23 = m_CharacterCount - 1;
						while (num23 >= 0 && num23 != m_LastBaseGlyphIndex)
						{
							Glyph glyph2 = textInfo.textElementInfo[num23].textElement.glyph;
							uint index2 = glyph2.index;
							uint glyphIndex5 = m_CachedTextElement.glyphIndex;
							uint key4 = (glyphIndex5 << 16) | index2;
							if (m_CurrentFontAsset.fontFeatureTable.m_MarkToMarkAdjustmentRecordLookup.TryGetValue(key4, out var value3))
							{
								float num24 = (textInfo.textElementInfo[num23].origin - m_XAdvance) / num2;
								float num25 = num12 - m_LineOffset + m_BaselineOffset;
								float num26 = (textInfo.textElementInfo[num23].baseLine - num25) / num2;
								glyphValueRecord.xPlacement = num24 + value3.baseMarkGlyphAnchorPoint.xCoordinate - value3.combiningMarkPositionAdjustment.xPositionAdjustment;
								glyphValueRecord.yPlacement = num26 + value3.baseMarkGlyphAnchorPoint.yCoordinate - value3.combiningMarkPositionAdjustment.yPositionAdjustment;
								num21 = 0f;
								flag11 = true;
								break;
							}
							num23--;
						}
					}
					if (flag3 && m_LastBaseGlyphIndex != int.MinValue && !flag11)
					{
						Glyph glyph3 = textInfo.textElementInfo[m_LastBaseGlyphIndex].textElement.glyph;
						uint index3 = glyph3.index;
						uint glyphIndex6 = m_CachedTextElement.glyphIndex;
						uint key5 = (glyphIndex6 << 16) | index3;
						if (m_CurrentFontAsset.fontFeatureTable.m_MarkToBaseAdjustmentRecordLookup.TryGetValue(key5, out var value4))
						{
							float num27 = (textInfo.textElementInfo[m_LastBaseGlyphIndex].origin - m_XAdvance) / num2;
							glyphValueRecord.xPlacement = num27 + value4.baseGlyphAnchorPoint.xCoordinate - value4.markPositionAdjustment.xPositionAdjustment;
							glyphValueRecord.yPlacement = value4.baseGlyphAnchorPoint.yCoordinate - value4.markPositionAdjustment.yPositionAdjustment;
							num21 = 0f;
						}
					}
				}
			}
			num13 += glyphValueRecord.yPlacement;
			num14 += glyphValueRecord.yPlacement;
			if (generationSettings.isRightToLeft)
			{
				m_XAdvance -= glyphMetrics.horizontalAdvance * (1f - m_CharWidthAdjDelta) * num2;
				if (flag9 || charCode == 8203)
				{
					m_XAdvance -= generationSettings.wordSpacing * num3;
				}
			}
			float num28 = 0f;
			if (m_MonoSpacing != 0f && charCode != 8203)
			{
				num28 = ((!m_DuoSpace || (charCode != 46 && charCode != 58 && charCode != 44)) ? ((m_MonoSpacing / 2f - (glyphMetrics.width / 2f + glyphMetrics.horizontalBearingX) * num2) * (1f - m_CharWidthAdjDelta)) : ((m_MonoSpacing / 4f - (glyphMetrics.width / 2f + glyphMetrics.horizontalBearingX) * num2) * (1f - m_CharWidthAdjDelta)));
				m_XAdvance += num28;
			}
			bool flag12 = m_CurrentFontAsset.atlasRenderMode != GlyphRenderMode.SMOOTH && m_CurrentFontAsset.atlasRenderMode != GlyphRenderMode.COLOR;
			float num30;
			float num31;
			if (m_TextElementType == TextElementType.Character && !isUsingAlternateTypeface && (textInfo.textElementInfo[m_CharacterCount].style & FontStyles.Bold) == FontStyles.Bold)
			{
				if (flag12)
				{
					float num29 = ((generationSettings.isIMGUI && m_CurrentMaterial.HasFloat(TextShaderUtilities.ID_GradientScale)) ? m_CurrentMaterial.GetFloat(TextShaderUtilities.ID_GradientScale) : ((float)(m_CurrentFontAsset.atlasPadding + 1)));
					num30 = m_CurrentFontAsset.boldStyleWeight / 4f * num29;
					if (num30 + num4 > num29)
					{
						num4 = num29 - num30;
					}
				}
				else
				{
					num30 = 0f;
				}
				num31 = m_CurrentFontAsset.boldStyleSpacing;
			}
			else
			{
				if (flag12)
				{
					float num32 = ((generationSettings.isIMGUI && m_CurrentMaterial.HasFloat(TextShaderUtilities.ID_GradientScale)) ? m_CurrentMaterial.GetFloat(TextShaderUtilities.ID_GradientScale) : ((float)(m_CurrentFontAsset.atlasPadding + 1)));
					num30 = m_CurrentFontAsset.m_RegularStyleWeight / 4f * num32;
					if (num30 + num4 > num32)
					{
						num4 = num32 - num30;
					}
				}
				else
				{
					num30 = 0f;
				}
				num31 = 0f;
			}
			vector.x = m_XAdvance + (glyphMetrics.horizontalBearingX * m_FXScale.x - num4 - num30 + glyphValueRecord.xPlacement) * num2 * (1f - m_CharWidthAdjDelta);
			vector.y = num12 + Round((glyphMetrics.horizontalBearingY + num4 + glyphValueRecord.yPlacement) * num2) - m_LineOffset + m_BaselineOffset;
			vector.z = 0f;
			vector2.x = vector.x;
			vector2.y = vector.y - (glyphMetrics.height + num4 * 2f) * num2;
			vector2.z = 0f;
			vector3.x = vector2.x + (glyphMetrics.width * m_FXScale.x + num4 * 2f + num30 * 2f) * num2 * (1f - m_CharWidthAdjDelta);
			vector3.y = vector.y;
			vector3.z = 0f;
			vector4.x = vector3.x;
			vector4.y = vector2.y;
			vector4.z = 0f;
			if (charCode == 8203)
			{
				vector = Vector3.zero;
				vector2 = Vector3.zero;
				vector3 = Vector3.zero;
				vector4 = Vector3.zero;
			}
			if (m_TextElementType == TextElementType.Character && !isUsingAlternateTypeface && (m_FontStyleInternal & FontStyles.Italic) == FontStyles.Italic)
			{
				float num33 = (float)m_ItalicAngle * 0.01f;
				float num34 = (m_CurrentFontAsset.m_FaceInfo.capLine - (m_CurrentFontAsset.m_FaceInfo.baseline + m_BaselineOffset)) / 2f * m_FontScaleMultiplier * m_CurrentFontAsset.m_FaceInfo.scale;
				Vector3 vector5 = new Vector3(num33 * ((glyphMetrics.horizontalBearingY + num4 + num30 - num34) * num2), 0f, 0f);
				Vector3 vector6 = new Vector3(num33 * ((glyphMetrics.horizontalBearingY - glyphMetrics.height - num4 - num30 - num34) * num2), 0f, 0f);
				vector += vector5;
				vector2 += vector6;
				vector3 += vector5;
				vector4 += vector6;
			}
			if (m_FXRotation != Quaternion.identity)
			{
				Matrix4x4 matrix4x = Matrix4x4.Rotate(m_FXRotation);
				Vector3 vector7 = (vector3 + vector2) / 2f;
				vector = matrix4x.MultiplyPoint3x4(vector - vector7) + vector7;
				vector2 = matrix4x.MultiplyPoint3x4(vector2 - vector7) + vector7;
				vector3 = matrix4x.MultiplyPoint3x4(vector3 - vector7) + vector7;
				vector4 = matrix4x.MultiplyPoint3x4(vector4 - vector7) + vector7;
			}
			textInfo.textElementInfo[m_CharacterCount].bottomLeft = vector2;
			textInfo.textElementInfo[m_CharacterCount].topLeft = vector;
			textInfo.textElementInfo[m_CharacterCount].topRight = vector3;
			textInfo.textElementInfo[m_CharacterCount].bottomRight = vector4;
			textInfo.textElementInfo[m_CharacterCount].origin = Round(m_XAdvance + glyphValueRecord.xPlacement * num2);
			textInfo.textElementInfo[m_CharacterCount].baseLine = Round(num12 - m_LineOffset + m_BaselineOffset + glyphValueRecord.yPlacement * num2);
			textInfo.textElementInfo[m_CharacterCount].aspectRatio = (vector3.x - vector2.x) / (vector.y - vector2.y);
			float num35 = ((m_TextElementType == TextElementType.Character) ? (num13 * num2 / num11 + m_BaselineOffset) : (num13 * num2 + m_BaselineOffset));
			float num36 = ((m_TextElementType == TextElementType.Character) ? (num14 * num2 / num11 + m_BaselineOffset) : (num14 * num2 + m_BaselineOffset));
			float num37 = num35;
			float num38 = num36;
			bool flag13 = m_CharacterCount == m_FirstCharacterOfLine;
			if (flag13 || !flag9)
			{
				if (m_BaselineOffset != 0f)
				{
					num37 = Mathf.Max((num35 - m_BaselineOffset) / m_FontScaleMultiplier, num37);
					num38 = Mathf.Min((num36 - m_BaselineOffset) / m_FontScaleMultiplier, num38);
				}
				m_MaxLineAscender = Mathf.Max(num37, m_MaxLineAscender);
				m_MaxLineDescender = Mathf.Min(num38, m_MaxLineDescender);
			}
			if (flag13 || !flag9)
			{
				textInfo.textElementInfo[m_CharacterCount].adjustedAscender = num37;
				textInfo.textElementInfo[m_CharacterCount].adjustedDescender = num38;
				textInfo.textElementInfo[m_CharacterCount].ascender = num35 - m_LineOffset;
				m_MaxDescender = (textInfo.textElementInfo[m_CharacterCount].descender = num36 - m_LineOffset);
			}
			else
			{
				textInfo.textElementInfo[m_CharacterCount].adjustedAscender = m_MaxLineAscender;
				textInfo.textElementInfo[m_CharacterCount].adjustedDescender = m_MaxLineDescender;
				textInfo.textElementInfo[m_CharacterCount].ascender = m_MaxLineAscender - m_LineOffset;
				m_MaxDescender = (textInfo.textElementInfo[m_CharacterCount].descender = m_MaxLineDescender - m_LineOffset);
			}
			if (m_LineNumber == 0 && (flag13 || !flag9))
			{
				m_MaxAscender = m_MaxLineAscender;
				m_MaxCapHeight = Mathf.Max(m_MaxCapHeight, m_CurrentFontAsset.m_FaceInfo.capLine * num2 / num11);
			}
			if (m_LineOffset == 0f && (flag13 || !flag9))
			{
				m_PageAscender = ((m_PageAscender > num35) ? m_PageAscender : num35);
			}
			textInfo.textElementInfo[m_CharacterCount].isVisible = false;
			if (charCode == 9 || ((textWrappingMode == TextWrappingMode.PreserveWhitespace || textWrappingMode == TextWrappingMode.PreserveWhitespaceNoWrap) && (flag9 || charCode == 8203)) || (!flag9 && charCode != 8203 && charCode != 173 && charCode != 3) || (charCode == 173 && !flag7) || m_TextElementType == TextElementType.Sprite)
			{
				textInfo.textElementInfo[m_CharacterCount].isVisible = true;
				float marginLeft = m_MarginLeft;
				float marginRight = m_MarginRight;
				if (flag8)
				{
					marginLeft = textInfo.lineInfo[m_LineNumber].marginLeft;
					marginRight = textInfo.lineInfo[m_LineNumber].marginRight;
				}
				num8 = ((m_Width != -1f) ? Mathf.Min(num6 + 0.0001f - marginLeft - marginRight, m_Width) : (num6 + 0.0001f - marginLeft - marginRight));
				float num39 = Mathf.Abs(m_XAdvance) + ((!generationSettings.isRightToLeft) ? glyphMetrics.horizontalAdvance : 0f) * (1f - m_CharWidthAdjDelta) * ((charCode == 173) ? num20 : num2);
				float num40 = m_MaxAscender - (m_MaxLineDescender - m_LineOffset) + ((m_LineOffset > 0f && !m_IsDrivenLineSpacing) ? (m_MaxLineAscender - m_StartOfLineAscender) : 0f);
				int characterCount = m_CharacterCount;
				if (num40 > num7 + 0.0001f)
				{
					if (m_FirstOverflowCharacterIndex == -1)
					{
						m_FirstOverflowCharacterIndex = m_CharacterCount;
					}
					bool flag14 = false;
					switch (generationSettings.overflowMode)
					{
					case TextOverflowMode.Truncate:
						i = RestoreWordWrappingState(ref m_SavedLastValidState, textInfo);
						characterSubstitution.index = characterCount;
						characterSubstitution.unicode = 3u;
						continue;
					case TextOverflowMode.Ellipsis:
						if (m_LineNumber > 0)
						{
							if (m_EllipsisInsertionCandidateStack.Count == 0)
							{
								i = -1;
								m_CharacterCount = 0;
								characterSubstitution.index = 0;
								characterSubstitution.unicode = 3u;
								m_FirstCharacterOfLine = 0;
							}
							else
							{
								WordWrapState state = m_EllipsisInsertionCandidateStack.Pop();
								i = RestoreWordWrappingState(ref state, textInfo);
								i--;
								m_CharacterCount--;
								characterSubstitution.index = m_CharacterCount;
								characterSubstitution.unicode = 8230u;
								num10++;
							}
							continue;
						}
						break;
					case TextOverflowMode.Linked:
						i = RestoreWordWrappingState(ref m_SavedLastValidState, textInfo);
						characterSubstitution.index = characterCount;
						characterSubstitution.unicode = 3u;
						continue;
					}
				}
				if (flag10 && num39 > num8)
				{
					if (textWrappingMode != TextWrappingMode.NoWrap && textWrappingMode != TextWrappingMode.PreserveWhitespaceNoWrap && m_CharacterCount != m_FirstCharacterOfLine)
					{
						i = RestoreWordWrappingState(ref m_SavedWordWrapState, textInfo);
						float num41 = 0f;
						if (m_LineHeight == -32767f)
						{
							float adjustedAscender = textInfo.textElementInfo[m_CharacterCount].adjustedAscender;
							num41 = ((m_LineOffset > 0f && !m_IsDrivenLineSpacing) ? (m_MaxLineAscender - m_StartOfLineAscender) : 0f) - m_MaxLineDescender + adjustedAscender + (num5 + m_LineSpacingDelta) * num + 0f * num3;
						}
						else
						{
							num41 = m_LineHeight + 0f * num3;
							m_IsDrivenLineSpacing = true;
						}
						float num42 = m_MaxAscender + num41 + m_LineOffset - textInfo.textElementInfo[m_CharacterCount].adjustedDescender;
						if (textInfo.textElementInfo[m_CharacterCount - 1].character == 173 && !flag7 && (generationSettings.overflowMode == TextOverflowMode.Overflow || num42 < num7 + 0.0001f))
						{
							characterSubstitution.index = m_CharacterCount - 1;
							characterSubstitution.unicode = 45u;
							i--;
							m_CharacterCount--;
							continue;
						}
						flag7 = false;
						if (textInfo.textElementInfo[m_CharacterCount].character == 173)
						{
							flag7 = true;
							continue;
						}
						bool flag15 = false;
						int previousWordBreak = m_SavedSoftLineBreakState.previousWordBreak;
						if (flag5 && previousWordBreak != -1 && previousWordBreak != num9)
						{
							i = RestoreWordWrappingState(ref m_SavedSoftLineBreakState, textInfo);
							num9 = previousWordBreak;
							if (textInfo.textElementInfo[m_CharacterCount - 1].character == 173)
							{
								characterSubstitution.index = m_CharacterCount - 1;
								characterSubstitution.unicode = 45u;
								i--;
								m_CharacterCount--;
								continue;
							}
						}
						if (!(num42 > num7 + 0.0001f))
						{
							InsertNewLine(i, num, num2, num3, num31, num21, num8, num5, ref isMaxVisibleDescenderSet, ref maxVisibleDescender, generationSettings, textInfo);
							flag = true;
							flag5 = true;
							continue;
						}
						if (m_FirstOverflowCharacterIndex == -1)
						{
							m_FirstOverflowCharacterIndex = m_CharacterCount;
						}
						bool flag16 = false;
						switch (generationSettings.overflowMode)
						{
						case TextOverflowMode.Overflow:
						case TextOverflowMode.Masking:
						case TextOverflowMode.ScrollRect:
							InsertNewLine(i, num, num2, num3, num31, num21, num8, num5, ref isMaxVisibleDescenderSet, ref maxVisibleDescender, generationSettings, textInfo);
							flag = true;
							flag5 = true;
							continue;
						case TextOverflowMode.Truncate:
							i = RestoreWordWrappingState(ref m_SavedLastValidState, textInfo);
							characterSubstitution.index = characterCount;
							characterSubstitution.unicode = 3u;
							continue;
						case TextOverflowMode.Ellipsis:
							if (m_EllipsisInsertionCandidateStack.Count == 0)
							{
								i = -1;
								m_CharacterCount = 0;
								characterSubstitution.index = 0;
								characterSubstitution.unicode = 3u;
								m_FirstCharacterOfLine = 0;
							}
							else
							{
								WordWrapState state2 = m_EllipsisInsertionCandidateStack.Pop();
								i = RestoreWordWrappingState(ref state2, textInfo);
								i--;
								m_CharacterCount--;
								characterSubstitution.index = m_CharacterCount;
								characterSubstitution.unicode = 8230u;
								num10++;
							}
							continue;
						case TextOverflowMode.Linked:
							characterSubstitution.index = m_CharacterCount;
							characterSubstitution.unicode = 3u;
							continue;
						}
					}
					else
					{
						bool flag17 = false;
						switch (generationSettings.overflowMode)
						{
						case TextOverflowMode.Truncate:
							i = RestoreWordWrappingState(ref m_SavedWordWrapState, textInfo);
							characterSubstitution.index = characterCount;
							characterSubstitution.unicode = 3u;
							continue;
						case TextOverflowMode.Ellipsis:
							if (m_EllipsisInsertionCandidateStack.Count == 0)
							{
								i = -1;
								m_CharacterCount = 0;
								characterSubstitution.index = 0;
								characterSubstitution.unicode = 3u;
								m_FirstCharacterOfLine = 0;
							}
							else
							{
								WordWrapState state3 = m_EllipsisInsertionCandidateStack.Pop();
								i = RestoreWordWrappingState(ref state3, textInfo);
								i--;
								m_CharacterCount--;
								characterSubstitution.index = m_CharacterCount;
								characterSubstitution.unicode = 8230u;
								num10++;
							}
							continue;
						case TextOverflowMode.Linked:
							i = RestoreWordWrappingState(ref m_SavedWordWrapState, textInfo);
							characterSubstitution.index = m_CharacterCount;
							characterSubstitution.unicode = 3u;
							continue;
						}
					}
				}
				if (flag9)
				{
					textInfo.textElementInfo[m_CharacterCount].isVisible = false;
					m_LineVisibleSpaceCount = ++textInfo.lineInfo[m_LineNumber].spaceCount;
					textInfo.lineInfo[m_LineNumber].marginLeft = marginLeft;
					textInfo.lineInfo[m_LineNumber].marginRight = marginRight;
					textInfo.spaceCount++;
					if (charCode == 160)
					{
						textInfo.lineInfo[m_LineNumber].controlCharacterCount++;
					}
				}
				else if (charCode == 173)
				{
					textInfo.textElementInfo[m_CharacterCount].isVisible = false;
				}
				else
				{
					Color32 htmlColor = m_HtmlColor;
					if (m_TextElementType == TextElementType.Character)
					{
						SaveGlyphVertexInfo(num4, num30, htmlColor, generationSettings, textInfo);
					}
					else if (m_TextElementType == TextElementType.Sprite)
					{
						SaveSpriteVertexInfo(htmlColor, generationSettings, textInfo);
					}
					if (flag)
					{
						flag = false;
						m_FirstVisibleCharacterOfLine = m_CharacterCount;
					}
					m_LineVisibleCharacterCount++;
					m_LastVisibleCharacterOfLine = m_CharacterCount;
					textInfo.lineInfo[m_LineNumber].marginLeft = marginLeft;
					textInfo.lineInfo[m_LineNumber].marginRight = marginRight;
				}
			}
			else
			{
				if (generationSettings.overflowMode == TextOverflowMode.Linked && (charCode == 10 || charCode == 11))
				{
					float num43 = m_MaxAscender - (m_MaxLineDescender - m_LineOffset) + ((m_LineOffset > 0f && !m_IsDrivenLineSpacing) ? (m_MaxLineAscender - m_StartOfLineAscender) : 0f);
					int characterCount2 = m_CharacterCount;
					if (num43 > num7 + 0.0001f)
					{
						if (m_FirstOverflowCharacterIndex == -1)
						{
							m_FirstOverflowCharacterIndex = m_CharacterCount;
						}
						i = RestoreWordWrappingState(ref m_SavedLastValidState, textInfo);
						characterSubstitution.index = characterCount2;
						characterSubstitution.unicode = 3u;
						continue;
					}
				}
				if ((charCode == 10 || charCode == 11 || charCode == 160 || charCode == 8199 || charCode == 8232 || charCode == 8233 || char.IsSeparator((char)charCode)) && charCode != 173 && charCode != 8203 && charCode != 8288)
				{
					textInfo.lineInfo[m_LineNumber].spaceCount++;
					textInfo.spaceCount++;
				}
				if (charCode == 160)
				{
					textInfo.lineInfo[m_LineNumber].controlCharacterCount++;
				}
			}
			if (generationSettings.overflowMode == TextOverflowMode.Ellipsis && (!flag8 || charCode == 45))
			{
				float num44 = m_CurrentFontSize / m_Ellipsis.fontAsset.m_FaceInfo.pointSize * m_Ellipsis.fontAsset.m_FaceInfo.scale;
				float num45 = num44 * m_FontScaleMultiplier * m_Ellipsis.character.m_Scale * m_Ellipsis.character.m_Glyph.scale;
				float marginLeft2 = m_MarginLeft;
				float marginRight2 = m_MarginRight;
				if (charCode == 10 && m_CharacterCount != m_FirstCharacterOfLine)
				{
					num44 = textInfo.textElementInfo[m_CharacterCount - 1].pointSize / m_Ellipsis.fontAsset.m_FaceInfo.pointSize * m_Ellipsis.fontAsset.m_FaceInfo.scale;
					num45 = num44 * m_FontScaleMultiplier * m_Ellipsis.character.m_Scale * m_Ellipsis.character.m_Glyph.scale;
					marginLeft2 = textInfo.lineInfo[m_LineNumber].marginLeft;
					marginRight2 = textInfo.lineInfo[m_LineNumber].marginRight;
				}
				float num46 = Mathf.Abs(m_XAdvance) + ((!generationSettings.isRightToLeft) ? m_Ellipsis.character.m_Glyph.metrics.horizontalAdvance : 0f) * (1f - m_CharWidthAdjDelta) * num45;
				float num47 = ((m_Width != -1f) ? Mathf.Min(num6 + 0.0001f - marginLeft2 - marginRight2, m_Width) : (num6 + 0.0001f - marginLeft2 - marginRight2));
				if (num46 < num47)
				{
					SaveWordWrappingState(ref m_SavedEllipsisState, i, m_CharacterCount, textInfo);
					m_EllipsisInsertionCandidateStack.Push(m_SavedEllipsisState);
				}
			}
			textInfo.textElementInfo[m_CharacterCount].lineNumber = m_LineNumber;
			if ((charCode != 10 && charCode != 11 && charCode != 13 && !flag8) || textInfo.lineInfo[m_LineNumber].characterCount == 1)
			{
				textInfo.lineInfo[m_LineNumber].alignment = m_LineJustification;
			}
			if (charCode != 8203)
			{
				if (charCode == 9)
				{
					float num48 = m_CurrentFontAsset.m_FaceInfo.tabWidth * (float)(int)m_CurrentFontAsset.tabMultiple * num2;
					float num49 = Mathf.Ceil(m_XAdvance / num48) * num48;
					m_XAdvance = ((num49 > m_XAdvance) ? num49 : (m_XAdvance + num48));
				}
				else if (m_MonoSpacing != 0f)
				{
					float num50 = ((!m_DuoSpace || (charCode != 46 && charCode != 58 && charCode != 44)) ? (m_MonoSpacing - num28) : (m_MonoSpacing / 2f - num28));
					m_XAdvance += (num50 + (m_CurrentFontAsset.regularStyleSpacing + num21) * num3 + m_CSpacing) * (1f - m_CharWidthAdjDelta);
					if (flag9 || charCode == 8203)
					{
						m_XAdvance += generationSettings.wordSpacing * num3;
					}
				}
				else if (generationSettings.isRightToLeft)
				{
					m_XAdvance -= (glyphValueRecord.xAdvance * num2 + (m_CurrentFontAsset.regularStyleSpacing + num21 + num31) * num3 + m_CSpacing) * (1f - m_CharWidthAdjDelta);
					if (flag9 || charCode == 8203)
					{
						m_XAdvance -= generationSettings.wordSpacing * num3;
					}
				}
				else
				{
					m_XAdvance += ((glyphMetrics.horizontalAdvance * m_FXScale.x + glyphValueRecord.xAdvance) * num2 + (m_CurrentFontAsset.regularStyleSpacing + num21 + num31) * num3 + m_CSpacing) * (1f - m_CharWidthAdjDelta);
					if (flag9 || charCode == 8203)
					{
						m_XAdvance += generationSettings.wordSpacing * num3;
					}
				}
			}
			textInfo.textElementInfo[m_CharacterCount].xAdvance = m_XAdvance;
			if (charCode == 13)
			{
				m_XAdvance = 0f + m_TagIndent;
			}
			if (charCode == 10 || charCode == 11 || charCode == 3 || charCode == 8232 || charCode == 8232 || (charCode == 45 && flag8) || m_CharacterCount == totalCharacterCount - 1)
			{
				float num51 = m_MaxLineAscender - m_StartOfLineAscender;
				if (m_LineOffset > 0f && Math.Abs(num51) > 0.01f && !m_IsDrivenLineSpacing)
				{
					TextGeneratorUtilities.AdjustLineOffset(m_FirstCharacterOfLine, m_CharacterCount, Round(num51), textInfo);
					m_MaxDescender -= num51;
					m_LineOffset += num51;
					if (m_SavedEllipsisState.lineNumber == m_LineNumber)
					{
						m_SavedEllipsisState = m_EllipsisInsertionCandidateStack.Pop();
						m_SavedEllipsisState.startOfLineAscender += num51;
						m_SavedEllipsisState.lineOffset += num51;
						m_EllipsisInsertionCandidateStack.Push(m_SavedEllipsisState);
					}
				}
				float num52 = m_MaxLineAscender - m_LineOffset;
				float num53 = m_MaxLineDescender - m_LineOffset;
				m_MaxDescender = ((m_MaxDescender < num53) ? m_MaxDescender : num53);
				if (!isMaxVisibleDescenderSet)
				{
					maxVisibleDescender = m_MaxDescender;
				}
				bool flag18 = false;
				textInfo.lineInfo[m_LineNumber].firstCharacterIndex = m_FirstCharacterOfLine;
				textInfo.lineInfo[m_LineNumber].firstVisibleCharacterIndex = (m_FirstVisibleCharacterOfLine = ((m_FirstCharacterOfLine > m_FirstVisibleCharacterOfLine) ? m_FirstCharacterOfLine : m_FirstVisibleCharacterOfLine));
				textInfo.lineInfo[m_LineNumber].lastCharacterIndex = (m_LastCharacterOfLine = m_CharacterCount);
				textInfo.lineInfo[m_LineNumber].lastVisibleCharacterIndex = (m_LastVisibleCharacterOfLine = ((m_LastVisibleCharacterOfLine < m_FirstVisibleCharacterOfLine) ? m_FirstVisibleCharacterOfLine : m_LastVisibleCharacterOfLine));
				int num54 = m_FirstVisibleCharacterOfLine;
				int num55 = m_LastVisibleCharacterOfLine;
				if ((generationSettings.textWrappingMode == TextWrappingMode.PreserveWhitespace || generationSettings.textWrappingMode == TextWrappingMode.PreserveWhitespaceNoWrap) && textInfo.textElementInfo[m_LastCharacterOfLine].xAdvance != 0f)
				{
					num54 = m_FirstCharacterOfLine;
					num55 = m_LastCharacterOfLine;
				}
				textInfo.lineInfo[m_LineNumber].characterCount = textInfo.lineInfo[m_LineNumber].lastCharacterIndex - textInfo.lineInfo[m_LineNumber].firstCharacterIndex + 1;
				textInfo.lineInfo[m_LineNumber].visibleCharacterCount = m_LineVisibleCharacterCount;
				textInfo.lineInfo[m_LineNumber].visibleSpaceCount = textInfo.lineInfo[m_LineNumber].lastVisibleCharacterIndex + 1 - textInfo.lineInfo[m_LineNumber].firstCharacterIndex - m_LineVisibleCharacterCount;
				textInfo.lineInfo[m_LineNumber].lineExtents.min = new Vector2(textInfo.textElementInfo[num54].bottomLeft.x, num53);
				textInfo.lineInfo[m_LineNumber].lineExtents.max = new Vector2(textInfo.textElementInfo[num55].topRight.x, num52);
				textInfo.lineInfo[m_LineNumber].length = (generationSettings.isIMGUI ? textInfo.textElementInfo[num55].xAdvance : (textInfo.lineInfo[m_LineNumber].lineExtents.max.x - num4 * num2));
				textInfo.lineInfo[m_LineNumber].width = num8;
				if (textInfo.lineInfo[m_LineNumber].characterCount == 1)
				{
					textInfo.lineInfo[m_LineNumber].alignment = m_LineJustification;
				}
				float num56 = ((m_CurrentFontAsset.regularStyleSpacing + num21 + num31) * num3 + m_CSpacing) * (1f - m_CharWidthAdjDelta);
				if (textInfo.textElementInfo[m_LastVisibleCharacterOfLine].isVisible)
				{
					textInfo.lineInfo[m_LineNumber].maxAdvance = textInfo.textElementInfo[m_LastVisibleCharacterOfLine].xAdvance + (generationSettings.isRightToLeft ? num56 : (0f - num56));
				}
				else
				{
					textInfo.lineInfo[m_LineNumber].maxAdvance = textInfo.textElementInfo[m_LastCharacterOfLine].xAdvance + (generationSettings.isRightToLeft ? num56 : (0f - num56));
				}
				textInfo.lineInfo[m_LineNumber].baseline = 0f - m_LineOffset;
				textInfo.lineInfo[m_LineNumber].ascender = num52;
				textInfo.lineInfo[m_LineNumber].descender = num53;
				textInfo.lineInfo[m_LineNumber].lineHeight = num52 - num53 + num5 * num;
				if (charCode == 10 || charCode == 11 || charCode == 45 || charCode == 8232 || charCode == 8233)
				{
					SaveWordWrappingState(ref m_SavedLineState, i, m_CharacterCount, textInfo);
					m_LineNumber++;
					flag = true;
					flag6 = false;
					flag5 = true;
					m_FirstCharacterOfLine = m_CharacterCount + 1;
					m_LineVisibleCharacterCount = 0;
					m_LineVisibleSpaceCount = 0;
					if (m_LineNumber >= textInfo.lineInfo.Length)
					{
						TextGeneratorUtilities.ResizeLineExtents(m_LineNumber, textInfo);
					}
					float adjustedAscender2 = textInfo.textElementInfo[m_CharacterCount].adjustedAscender;
					if (m_LineHeight == -32767f)
					{
						float num57 = 0f - m_MaxLineDescender + adjustedAscender2 + (num5 + m_LineSpacingDelta) * num + (0f + ((charCode == 10 || charCode == 8233) ? generationSettings.paragraphSpacing : 0f)) * num3;
						m_LineOffset += num57;
						m_IsDrivenLineSpacing = false;
					}
					else
					{
						m_LineOffset += m_LineHeight + (0f + ((charCode == 10 || charCode == 8233) ? generationSettings.paragraphSpacing : 0f)) * num3;
						m_IsDrivenLineSpacing = true;
					}
					m_MaxLineAscender = -32767f;
					m_MaxLineDescender = 32767f;
					m_StartOfLineAscender = adjustedAscender2;
					m_XAdvance = 0f + m_TagLineIndent + m_TagIndent;
					SaveWordWrappingState(ref m_SavedWordWrapState, i, m_CharacterCount, textInfo);
					SaveWordWrappingState(ref m_SavedLastValidState, i, m_CharacterCount, textInfo);
					m_CharacterCount++;
					continue;
				}
				if (charCode == 3)
				{
					i = m_TextProcessingArray.Length;
				}
			}
			if (textInfo.textElementInfo[m_CharacterCount].isVisible)
			{
				m_MeshExtents.min.x = Mathf.Min(m_MeshExtents.min.x, textInfo.textElementInfo[m_CharacterCount].bottomLeft.x);
				m_MeshExtents.min.y = Mathf.Min(m_MeshExtents.min.y, textInfo.textElementInfo[m_CharacterCount].bottomLeft.y);
				m_MeshExtents.max.x = Mathf.Max(m_MeshExtents.max.x, textInfo.textElementInfo[m_CharacterCount].topRight.x);
				m_MeshExtents.max.y = Mathf.Max(m_MeshExtents.max.y, textInfo.textElementInfo[m_CharacterCount].topRight.y);
			}
			if ((textWrappingMode != TextWrappingMode.NoWrap && textWrappingMode != TextWrappingMode.PreserveWhitespaceNoWrap) || generationSettings.overflowMode == TextOverflowMode.Truncate || generationSettings.overflowMode == TextOverflowMode.Ellipsis || generationSettings.overflowMode == TextOverflowMode.Linked)
			{
				bool flag19 = false;
				bool flag20 = false;
				if ((flag9 || charCode == 8203 || (charCode == 45 && (m_CharacterCount <= 0 || !char.IsWhiteSpace((char)textInfo.textElementInfo[m_CharacterCount - 1].character))) || charCode == 173) && (!m_IsNonBreakingSpace || flag6) && charCode != 160 && charCode != 8199 && charCode != 8209 && charCode != 8239 && charCode != 8288)
				{
					if (charCode != 45 || m_CharacterCount <= 0 || !char.IsWhiteSpace((char)textInfo.textElementInfo[m_CharacterCount - 1].character))
					{
						flag5 = false;
						flag19 = true;
						m_SavedSoftLineBreakState.previousWordBreak = -1;
					}
				}
				else if (!m_IsNonBreakingSpace && ((TextGeneratorUtilities.IsHangul(charCode) && !textSettings.lineBreakingRules.useModernHangulLineBreakingRules) || TextGeneratorUtilities.IsCJK(charCode)))
				{
					bool flag21 = textSettings.lineBreakingRules.leadingCharactersLookup.Contains(charCode);
					bool flag22 = m_CharacterCount < totalCharacterCount - 1 && textSettings.lineBreakingRules.followingCharactersLookup.Contains(textInfo.textElementInfo[m_CharacterCount + 1].character);
					if (!flag21)
					{
						if (!flag22)
						{
							flag5 = false;
							flag19 = true;
						}
						if (flag5)
						{
							if (flag9)
							{
								flag20 = true;
							}
							flag19 = true;
						}
					}
					else if (flag5 && flag13)
					{
						if (flag9)
						{
							flag20 = true;
						}
						flag19 = true;
					}
				}
				else if (!m_IsNonBreakingSpace && m_CharacterCount + 1 < totalCharacterCount && TextGeneratorUtilities.IsCJK(textInfo.textElementInfo[m_CharacterCount + 1].character))
				{
					uint character = textInfo.textElementInfo[m_CharacterCount + 1].character;
					bool flag23 = textSettings.lineBreakingRules.leadingCharactersLookup.Contains(charCode);
					bool flag24 = textSettings.lineBreakingRules.leadingCharactersLookup.Contains(character);
					if (!flag23 && !flag24)
					{
						flag19 = true;
					}
				}
				else if (flag5)
				{
					if ((flag9 && charCode != 160) || (charCode == 173 && !flag7))
					{
						flag20 = true;
					}
					flag19 = true;
				}
				if (flag19)
				{
					SaveWordWrappingState(ref m_SavedWordWrapState, i, m_CharacterCount, textInfo);
				}
				if (flag20)
				{
					SaveWordWrappingState(ref m_SavedSoftLineBreakState, i, m_CharacterCount, textInfo);
				}
			}
			SaveWordWrappingState(ref m_SavedLastValidState, i, m_CharacterCount, textInfo);
			m_CharacterCount++;
		}
		CloseAllLinkTags(textInfo);
	}

	private void InsertNewLine(int i, float baseScale, float currentElementScale, float currentEmScale, float boldSpacingAdjustment, float characterSpacingAdjustment, float width, float lineGap, ref bool isMaxVisibleDescenderSet, ref float maxVisibleDescender, TextGenerationSettings generationSettings, TextInfo textInfo)
	{
		float num = m_MaxLineAscender - m_StartOfLineAscender;
		if (m_LineOffset > 0f && Math.Abs(num) > 0.01f && !m_IsDrivenLineSpacing)
		{
			TextGeneratorUtilities.AdjustLineOffset(m_FirstCharacterOfLine, m_CharacterCount, Round(num), textInfo);
			m_MaxDescender -= num;
			m_LineOffset += num;
		}
		float num2 = m_MaxLineAscender - m_LineOffset;
		float num3 = m_MaxLineDescender - m_LineOffset;
		m_MaxDescender = ((m_MaxDescender < num3) ? m_MaxDescender : num3);
		if (!isMaxVisibleDescenderSet)
		{
			maxVisibleDescender = m_MaxDescender;
		}
		bool flag = false;
		textInfo.lineInfo[m_LineNumber].firstCharacterIndex = m_FirstCharacterOfLine;
		textInfo.lineInfo[m_LineNumber].firstVisibleCharacterIndex = (m_FirstVisibleCharacterOfLine = ((m_FirstCharacterOfLine > m_FirstVisibleCharacterOfLine) ? m_FirstCharacterOfLine : m_FirstVisibleCharacterOfLine));
		textInfo.lineInfo[m_LineNumber].lastCharacterIndex = (m_LastCharacterOfLine = ((m_CharacterCount - 1 > 0) ? (m_CharacterCount - 1) : 0));
		textInfo.lineInfo[m_LineNumber].lastVisibleCharacterIndex = (m_LastVisibleCharacterOfLine = ((m_LastVisibleCharacterOfLine < m_FirstVisibleCharacterOfLine) ? m_FirstVisibleCharacterOfLine : m_LastVisibleCharacterOfLine));
		textInfo.lineInfo[m_LineNumber].characterCount = textInfo.lineInfo[m_LineNumber].lastCharacterIndex - textInfo.lineInfo[m_LineNumber].firstCharacterIndex + 1;
		textInfo.lineInfo[m_LineNumber].visibleCharacterCount = m_LineVisibleCharacterCount;
		textInfo.lineInfo[m_LineNumber].visibleSpaceCount = textInfo.lineInfo[m_LineNumber].lastVisibleCharacterIndex + 1 - textInfo.lineInfo[m_LineNumber].firstCharacterIndex - m_LineVisibleCharacterCount;
		textInfo.lineInfo[m_LineNumber].lineExtents.min = new Vector2(textInfo.textElementInfo[m_FirstVisibleCharacterOfLine].bottomLeft.x, num3);
		textInfo.lineInfo[m_LineNumber].lineExtents.max = new Vector2(textInfo.textElementInfo[m_LastVisibleCharacterOfLine].topRight.x, num2);
		textInfo.lineInfo[m_LineNumber].length = (generationSettings.isIMGUI ? textInfo.textElementInfo[m_LastVisibleCharacterOfLine].xAdvance : textInfo.lineInfo[m_LineNumber].lineExtents.max.x);
		textInfo.lineInfo[m_LineNumber].width = width;
		float adjustedHorizontalAdvance = textInfo.textElementInfo[m_LastVisibleCharacterOfLine].adjustedHorizontalAdvance;
		float num4 = (adjustedHorizontalAdvance * currentElementScale + (m_CurrentFontAsset.regularStyleSpacing + characterSpacingAdjustment + boldSpacingAdjustment) * currentEmScale + m_CSpacing) * 1f;
		float v = (textInfo.lineInfo[m_LineNumber].maxAdvance = textInfo.textElementInfo[m_LastVisibleCharacterOfLine].xAdvance + (generationSettings.isRightToLeft ? num4 : (0f - num4)));
		textInfo.textElementInfo[m_LastVisibleCharacterOfLine].xAdvance = Round(v);
		textInfo.lineInfo[m_LineNumber].baseline = 0f - m_LineOffset;
		textInfo.lineInfo[m_LineNumber].ascender = num2;
		textInfo.lineInfo[m_LineNumber].descender = num3;
		textInfo.lineInfo[m_LineNumber].lineHeight = num2 - num3 + lineGap * baseScale;
		m_FirstCharacterOfLine = m_CharacterCount;
		m_LineVisibleCharacterCount = 0;
		m_LineVisibleSpaceCount = 0;
		SaveWordWrappingState(ref m_SavedLineState, i, m_CharacterCount - 1, textInfo);
		m_LineNumber++;
		if (m_LineNumber >= textInfo.lineInfo.Length)
		{
			TextGeneratorUtilities.ResizeLineExtents(m_LineNumber, textInfo);
		}
		if (m_LineHeight == -32767f)
		{
			float adjustedAscender = textInfo.textElementInfo[m_CharacterCount].adjustedAscender;
			float num5 = 0f - m_MaxLineDescender + adjustedAscender + (lineGap + m_LineSpacingDelta) * baseScale + 0f * currentEmScale;
			m_LineOffset += num5;
			m_StartOfLineAscender = adjustedAscender;
		}
		else
		{
			m_LineOffset += m_LineHeight + 0f * currentEmScale;
		}
		m_MaxLineAscender = -32767f;
		m_MaxLineDescender = 32767f;
		m_XAdvance = 0f + m_TagIndent;
	}

	public Vector2 GetPreferredValues(TextGenerationSettings settings, TextInfo textInfo)
	{
		if (settings.fontAsset == null || settings.fontAsset.characterLookupTable == null)
		{
			Debug.LogWarning("Can't Generate Mesh, No Font Asset has been assigned.");
			return Vector2.zero;
		}
		if (settings.fontSize <= 0)
		{
			return Vector2.zero;
		}
		Prepare(settings, textInfo);
		return GetPreferredValuesInternal(settings, textInfo);
	}

	private Vector2 GetPreferredValuesInternal(TextGenerationSettings generationSettings, TextInfo textInfo)
	{
		if (generationSettings.textSettings == null)
		{
			return Vector2.zero;
		}
		float fontSize = m_FontSize;
		m_MinFontSize = 0f;
		m_MaxFontSize = 0f;
		m_CharWidthAdjDelta = 0f;
		Vector2 marginSize = new Vector2((m_MarginWidth != 0f) ? m_MarginWidth : 32767f, (m_MarginHeight != 0f) ? m_MarginHeight : 32767f);
		m_AutoSizeIterationCount = 0;
		return CalculatePreferredValues(ref fontSize, marginSize, isTextAutoSizingEnabled: false, generationSettings, textInfo);
	}

	protected virtual Vector2 CalculatePreferredValues(ref float fontSize, Vector2 marginSize, bool isTextAutoSizingEnabled, TextGenerationSettings generationSettings, TextInfo textInfo)
	{
		if (generationSettings.fontAsset == null || generationSettings.fontAsset.characterLookupTable == null)
		{
			Debug.LogWarning("Can't Generate Mesh! No Font Asset has been assigned.");
			return Vector2.zero;
		}
		if (m_TextProcessingArray == null || m_TextProcessingArray.Length == 0 || m_TextProcessingArray[0].unicode == 0)
		{
			return Vector2.zero;
		}
		m_CurrentFontAsset = generationSettings.fontAsset;
		m_CurrentMaterial = generationSettings.fontAsset.material;
		m_CurrentMaterialIndex = 0;
		m_MaterialReferenceStack.SetDefault(new MaterialReference(0, m_CurrentFontAsset, null, m_CurrentMaterial, m_Padding));
		int totalCharacterCount = m_TotalCharacterCount;
		if (m_InternalTextElementInfo == null || totalCharacterCount > m_InternalTextElementInfo.Length)
		{
			m_InternalTextElementInfo = new TextElementInfo[(totalCharacterCount > 1024) ? (totalCharacterCount + 256) : Mathf.NextPowerOfTwo(totalCharacterCount)];
		}
		float num = fontSize / generationSettings.fontAsset.faceInfo.pointSize * generationSettings.fontAsset.faceInfo.scale;
		float num2 = num;
		float num3 = fontSize * 0.01f;
		m_FontScaleMultiplier = 1f;
		m_ShouldRenderBitmap = generationSettings.fontAsset.IsBitmap();
		m_CurrentFontSize = fontSize;
		m_SizeStack.SetDefault(m_CurrentFontSize);
		float num4 = 0f;
		m_FontStyleInternal = generationSettings.fontStyle;
		m_FontWeightInternal = (((m_FontStyleInternal & FontStyles.Bold) == FontStyles.Bold) ? TextFontWeight.Bold : generationSettings.fontWeight);
		m_FontWeightStack.SetDefault(m_FontWeightInternal);
		m_FontStyleStack.Clear();
		m_LineJustification = generationSettings.textAlignment;
		m_LineJustificationStack.SetDefault(m_LineJustification);
		m_BaselineOffset = 0f;
		m_BaselineOffsetStack.Clear();
		m_FXScale = Vector3.one;
		m_LineOffset = 0f;
		m_LineHeight = -32767f;
		float num5 = Round(m_CurrentFontAsset.faceInfo.lineHeight - (m_CurrentFontAsset.faceInfo.ascentLine - m_CurrentFontAsset.faceInfo.descentLine));
		m_CSpacing = 0f;
		m_MonoSpacing = 0f;
		m_XAdvance = 0f;
		m_TagLineIndent = 0f;
		m_TagIndent = 0f;
		m_IndentStack.SetDefault(0f);
		m_TagNoParsing = false;
		m_CharacterCount = 0;
		m_FirstCharacterOfLine = 0;
		m_MaxLineAscender = -32767f;
		m_MaxLineDescender = 32767f;
		m_LineNumber = 0;
		m_StartOfLineAscender = 0f;
		m_IsDrivenLineSpacing = false;
		m_LastBaseGlyphIndex = int.MinValue;
		bool flag = TextGenerationSettings.fontFeatures.Contains(OTL_FeatureTag.kern);
		bool flag2 = TextGenerationSettings.fontFeatures.Contains(OTL_FeatureTag.mark);
		bool flag3 = TextGenerationSettings.fontFeatures.Contains(OTL_FeatureTag.mkmk);
		TextSettings textSettings = generationSettings.textSettings;
		float x = marginSize.x;
		float y = marginSize.y;
		m_MarginLeft = 0f;
		m_MarginRight = 0f;
		m_Width = -1f;
		float num6 = x + 0.0001f - m_MarginLeft - m_MarginRight;
		TextWrappingMode textWrappingMode = generationSettings.textWrappingMode;
		float num7 = 0f;
		float num8 = 0f;
		float num9 = 0f;
		m_IsCalculatingPreferredValues = true;
		m_MaxCapHeight = 0f;
		m_MaxAscender = 0f;
		m_MaxDescender = 0f;
		float num10 = 0f;
		bool flag4 = false;
		bool flag5 = true;
		m_IsNonBreakingSpace = false;
		bool flag6 = false;
		CharacterSubstitution characterSubstitution = new CharacterSubstitution(-1, 0u);
		bool flag7 = false;
		WordWrapState state = default(WordWrapState);
		WordWrapState state2 = default(WordWrapState);
		WordWrapState state3 = default(WordWrapState);
		m_IsTextTruncated = false;
		m_AutoSizeIterationCount++;
		for (int i = 0; i < m_TextProcessingArray.Length && m_TextProcessingArray[i].unicode != 0; i++)
		{
			uint num11 = m_TextProcessingArray[i].unicode;
			if (num11 == 26)
			{
				continue;
			}
			if (generationSettings.richText && num11 == 60)
			{
				m_isTextLayoutPhase = true;
				m_TextElementType = TextElementType.Character;
				if (ValidateHtmlTag(m_TextProcessingArray, i + 1, out var endIndex, generationSettings, textInfo, out var _))
				{
					i = endIndex;
					if (m_TextElementType == TextElementType.Character)
					{
						continue;
					}
				}
			}
			else
			{
				m_TextElementType = textInfo.textElementInfo[m_CharacterCount].elementType;
				m_CurrentMaterialIndex = textInfo.textElementInfo[m_CharacterCount].materialReferenceIndex;
				m_CurrentFontAsset = textInfo.textElementInfo[m_CharacterCount].fontAsset;
			}
			int currentMaterialIndex = m_CurrentMaterialIndex;
			bool isUsingAlternateTypeface = textInfo.textElementInfo[m_CharacterCount].isUsingAlternateTypeface;
			m_isTextLayoutPhase = false;
			bool flag8 = false;
			if (characterSubstitution.index == m_CharacterCount)
			{
				num11 = characterSubstitution.unicode;
				m_TextElementType = TextElementType.Character;
				flag8 = true;
				switch (num11)
				{
				case 3u:
					m_InternalTextElementInfo[m_CharacterCount].textElement = m_CurrentFontAsset.characterLookupTable[3u];
					m_IsTextTruncated = true;
					break;
				case 8230u:
					m_InternalTextElementInfo[m_CharacterCount].textElement = m_Ellipsis.character;
					m_InternalTextElementInfo[m_CharacterCount].elementType = TextElementType.Character;
					m_InternalTextElementInfo[m_CharacterCount].fontAsset = m_Ellipsis.fontAsset;
					m_InternalTextElementInfo[m_CharacterCount].material = m_Ellipsis.material;
					m_InternalTextElementInfo[m_CharacterCount].materialReferenceIndex = m_Ellipsis.materialIndex;
					m_IsTextTruncated = true;
					characterSubstitution.index = m_CharacterCount + 1;
					characterSubstitution.unicode = 3u;
					break;
				}
			}
			if (m_CharacterCount < 0 && num11 != 3)
			{
				m_InternalTextElementInfo[m_CharacterCount].isVisible = false;
				m_InternalTextElementInfo[m_CharacterCount].character = 8203u;
				m_InternalTextElementInfo[m_CharacterCount].lineNumber = 0;
				m_CharacterCount++;
				continue;
			}
			float num12 = 1f;
			if (m_TextElementType == TextElementType.Character)
			{
				if ((m_FontStyleInternal & FontStyles.UpperCase) == FontStyles.UpperCase)
				{
					if (char.IsLower((char)num11))
					{
						num11 = char.ToUpper((char)num11);
					}
				}
				else if ((m_FontStyleInternal & FontStyles.LowerCase) == FontStyles.LowerCase)
				{
					if (char.IsUpper((char)num11))
					{
						num11 = char.ToLower((char)num11);
					}
				}
				else if ((m_FontStyleInternal & FontStyles.SmallCaps) == FontStyles.SmallCaps && char.IsLower((char)num11))
				{
					num12 = 0.8f;
					num11 = char.ToUpper((char)num11);
				}
			}
			float num13 = 0f;
			float num14 = 0f;
			float num15 = 0f;
			if (m_TextElementType == TextElementType.Sprite)
			{
				SpriteCharacter spriteCharacter = (SpriteCharacter)textInfo.textElementInfo[m_CharacterCount].textElement;
				m_CurrentSpriteAsset = spriteCharacter.textAsset as SpriteAsset;
				m_SpriteIndex = (int)spriteCharacter.glyphIndex;
				if (spriteCharacter == null)
				{
					continue;
				}
				if (num11 == 60)
				{
					num11 = (uint)(57344 + m_SpriteIndex);
				}
				if (m_CurrentSpriteAsset.faceInfo.pointSize > 0f)
				{
					float num16 = m_CurrentFontSize / m_CurrentSpriteAsset.faceInfo.pointSize * m_CurrentSpriteAsset.faceInfo.scale;
					num2 = spriteCharacter.scale * spriteCharacter.glyph.scale * num16;
					num14 = m_CurrentSpriteAsset.faceInfo.ascentLine;
					num15 = m_CurrentSpriteAsset.faceInfo.descentLine;
				}
				else
				{
					float num17 = m_CurrentFontSize / m_CurrentFontAsset.faceInfo.pointSize * m_CurrentFontAsset.faceInfo.scale;
					num2 = m_CurrentFontAsset.faceInfo.ascentLine / spriteCharacter.glyph.metrics.height * spriteCharacter.scale * spriteCharacter.glyph.scale * num17;
					float num18 = num17 / num2;
					num14 = m_CurrentFontAsset.faceInfo.ascentLine * num18;
					num15 = m_CurrentFontAsset.faceInfo.descentLine * num18;
				}
				m_CachedTextElement = spriteCharacter;
				m_InternalTextElementInfo[m_CharacterCount].elementType = TextElementType.Sprite;
				m_InternalTextElementInfo[m_CharacterCount].scale = num2;
				m_CurrentMaterialIndex = currentMaterialIndex;
			}
			else if (m_TextElementType == TextElementType.Character)
			{
				m_CachedTextElement = textInfo.textElementInfo[m_CharacterCount].textElement;
				if (m_CachedTextElement == null)
				{
					continue;
				}
				m_CurrentFontAsset = textInfo.textElementInfo[m_CharacterCount].fontAsset;
				m_CurrentMaterial = textInfo.textElementInfo[m_CharacterCount].material;
				m_CurrentMaterialIndex = textInfo.textElementInfo[m_CharacterCount].materialReferenceIndex;
				float num19 = ((!flag8 || m_TextProcessingArray[i].unicode != 10 || m_CharacterCount == m_FirstCharacterOfLine) ? (m_CurrentFontSize * num12 / m_CurrentFontAsset.m_FaceInfo.pointSize * m_CurrentFontAsset.m_FaceInfo.scale) : (textInfo.textElementInfo[m_CharacterCount - 1].pointSize * num12 / m_CurrentFontAsset.m_FaceInfo.pointSize * m_CurrentFontAsset.m_FaceInfo.scale));
				if (flag8 && num11 == 8230)
				{
					num14 = 0f;
					num15 = 0f;
				}
				else
				{
					num14 = m_CurrentFontAsset.m_FaceInfo.ascentLine;
					num15 = m_CurrentFontAsset.m_FaceInfo.descentLine;
				}
				num2 = num19 * m_FontScaleMultiplier * m_CachedTextElement.scale;
				m_InternalTextElementInfo[m_CharacterCount].elementType = TextElementType.Character;
			}
			float num20 = num2;
			if (num11 == 173 || num11 == 3)
			{
				num2 = 0f;
			}
			m_InternalTextElementInfo[m_CharacterCount].character = (ushort)num11;
			m_InternalTextElementInfo[m_CharacterCount].style = m_FontStyleInternal;
			if (m_FontWeightInternal == TextFontWeight.Bold)
			{
				m_InternalTextElementInfo[m_CharacterCount].style |= FontStyles.Bold;
			}
			GlyphMetrics glyphMetrics = textInfo.textElementInfo[m_CharacterCount].alternativeGlyph?.metrics ?? m_CachedTextElement.m_Glyph.metrics;
			bool flag9 = num11 <= 65535 && char.IsWhiteSpace((char)num11);
			GlyphValueRecord glyphValueRecord = default(GlyphValueRecord);
			float num21 = generationSettings.characterSpacing;
			if (flag && m_TextElementType == TextElementType.Character)
			{
				uint glyphIndex = m_CachedTextElement.m_GlyphIndex;
				GlyphPairAdjustmentRecord value;
				if (m_CharacterCount < totalCharacterCount - 1 && textInfo.textElementInfo[m_CharacterCount + 1].elementType == TextElementType.Character)
				{
					uint glyphIndex2 = textInfo.textElementInfo[m_CharacterCount + 1].textElement.m_GlyphIndex;
					uint key = (glyphIndex2 << 16) | glyphIndex;
					if (m_CurrentFontAsset.m_FontFeatureTable.m_GlyphPairAdjustmentRecordLookup.TryGetValue(key, out value))
					{
						glyphValueRecord = value.firstAdjustmentRecord.glyphValueRecord;
						num21 = (((value.featureLookupFlags & FontFeatureLookupFlags.IgnoreSpacingAdjustments) == FontFeatureLookupFlags.IgnoreSpacingAdjustments) ? 0f : num21);
					}
				}
				if (m_CharacterCount >= 1)
				{
					uint glyphIndex3 = textInfo.textElementInfo[m_CharacterCount - 1].textElement.m_GlyphIndex;
					uint key2 = (glyphIndex << 16) | glyphIndex3;
					if (textInfo.textElementInfo[m_CharacterCount - 1].elementType == TextElementType.Character && m_CurrentFontAsset.m_FontFeatureTable.m_GlyphPairAdjustmentRecordLookup.TryGetValue(key2, out value))
					{
						glyphValueRecord += value.secondAdjustmentRecord.glyphValueRecord;
						num21 = (((value.featureLookupFlags & FontFeatureLookupFlags.IgnoreSpacingAdjustments) == FontFeatureLookupFlags.IgnoreSpacingAdjustments) ? 0f : num21);
					}
				}
				m_InternalTextElementInfo[m_CharacterCount].adjustedHorizontalAdvance = glyphValueRecord.xAdvance;
			}
			bool flag10 = TextGeneratorUtilities.IsBaseGlyph(num11);
			if (flag10)
			{
				m_LastBaseGlyphIndex = m_CharacterCount;
			}
			if (m_CharacterCount > 0 && !flag10)
			{
				if (m_LastBaseGlyphIndex != int.MinValue && m_LastBaseGlyphIndex == m_CharacterCount - 1)
				{
					Glyph glyph = textInfo.textElementInfo[m_LastBaseGlyphIndex].textElement.glyph;
					uint index = glyph.index;
					uint glyphIndex4 = m_CachedTextElement.glyphIndex;
					uint key3 = (glyphIndex4 << 16) | index;
					if (m_CurrentFontAsset.fontFeatureTable.m_MarkToBaseAdjustmentRecordLookup.TryGetValue(key3, out var value2))
					{
						float num22 = (m_InternalTextElementInfo[m_LastBaseGlyphIndex].origin - m_XAdvance) / num2;
						glyphValueRecord.xPlacement = num22 + value2.baseGlyphAnchorPoint.xCoordinate - value2.markPositionAdjustment.xPositionAdjustment;
						glyphValueRecord.yPlacement = value2.baseGlyphAnchorPoint.yCoordinate - value2.markPositionAdjustment.yPositionAdjustment;
						num21 = 0f;
					}
				}
				else
				{
					bool flag11 = false;
					int num23 = m_CharacterCount - 1;
					while (num23 >= 0 && num23 != m_LastBaseGlyphIndex)
					{
						Glyph glyph2 = textInfo.textElementInfo[num23].textElement.glyph;
						uint index2 = glyph2.index;
						uint glyphIndex5 = m_CachedTextElement.glyphIndex;
						uint key4 = (glyphIndex5 << 16) | index2;
						if (m_CurrentFontAsset.fontFeatureTable.m_MarkToMarkAdjustmentRecordLookup.TryGetValue(key4, out var value3))
						{
							float num24 = (textInfo.textElementInfo[num23].origin - m_XAdvance) / num2;
							float num25 = num13 - m_LineOffset + m_BaselineOffset;
							float num26 = (m_InternalTextElementInfo[num23].baseLine - num25) / num2;
							glyphValueRecord.xPlacement = num24 + value3.baseMarkGlyphAnchorPoint.xCoordinate - value3.combiningMarkPositionAdjustment.xPositionAdjustment;
							glyphValueRecord.yPlacement = num26 + value3.baseMarkGlyphAnchorPoint.yCoordinate - value3.combiningMarkPositionAdjustment.yPositionAdjustment;
							num21 = 0f;
							flag11 = true;
							break;
						}
						num23--;
					}
					if (m_LastBaseGlyphIndex != int.MinValue && !flag11)
					{
						Glyph glyph3 = textInfo.textElementInfo[m_LastBaseGlyphIndex].textElement.glyph;
						uint index3 = glyph3.index;
						uint glyphIndex6 = m_CachedTextElement.glyphIndex;
						uint key5 = (glyphIndex6 << 16) | index3;
						if (m_CurrentFontAsset.fontFeatureTable.m_MarkToBaseAdjustmentRecordLookup.TryGetValue(key5, out var value4))
						{
							float num27 = (m_InternalTextElementInfo[m_LastBaseGlyphIndex].origin - m_XAdvance) / num2;
							glyphValueRecord.xPlacement = num27 + value4.baseGlyphAnchorPoint.xCoordinate - value4.markPositionAdjustment.xPositionAdjustment;
							glyphValueRecord.yPlacement = value4.baseGlyphAnchorPoint.yCoordinate - value4.markPositionAdjustment.yPositionAdjustment;
							num21 = 0f;
						}
					}
				}
			}
			num14 += glyphValueRecord.yPlacement;
			num15 += glyphValueRecord.yPlacement;
			float num28 = 0f;
			if (m_MonoSpacing != 0f && num11 != 8203)
			{
				num28 = (m_MonoSpacing / 2f - (m_CachedTextElement.glyph.metrics.width / 2f + m_CachedTextElement.glyph.metrics.horizontalBearingX) * num2) * (1f - m_CharWidthAdjDelta);
				m_XAdvance += num28;
			}
			float num29 = 0f;
			if (m_TextElementType == TextElementType.Character && !isUsingAlternateTypeface && (m_InternalTextElementInfo[m_CharacterCount].style & FontStyles.Bold) == FontStyles.Bold)
			{
				num29 = m_CurrentFontAsset.boldStyleSpacing;
			}
			m_InternalTextElementInfo[m_CharacterCount].origin = Round(m_XAdvance + glyphValueRecord.xPlacement * num2);
			m_InternalTextElementInfo[m_CharacterCount].baseLine = Round(num13 - m_LineOffset + m_BaselineOffset + glyphValueRecord.yPlacement * num2);
			float num30 = ((m_TextElementType == TextElementType.Character) ? (num14 * num2 / num12 + m_BaselineOffset) : (num14 * num2 + m_BaselineOffset));
			float num31 = ((m_TextElementType == TextElementType.Character) ? (num15 * num2 / num12 + m_BaselineOffset) : (num15 * num2 + m_BaselineOffset));
			float num32 = num30;
			float num33 = num31;
			bool flag12 = m_CharacterCount == m_FirstCharacterOfLine;
			if (flag12 || !flag9)
			{
				if (m_BaselineOffset != 0f)
				{
					num32 = Mathf.Max((num30 - m_BaselineOffset) / m_FontScaleMultiplier, num32);
					num33 = Mathf.Min((num31 - m_BaselineOffset) / m_FontScaleMultiplier, num33);
				}
				m_MaxLineAscender = Mathf.Max(num32, m_MaxLineAscender);
				m_MaxLineDescender = Mathf.Min(num33, m_MaxLineDescender);
			}
			if (flag12 || !flag9)
			{
				m_InternalTextElementInfo[m_CharacterCount].adjustedAscender = num32;
				m_InternalTextElementInfo[m_CharacterCount].adjustedDescender = num33;
				m_InternalTextElementInfo[m_CharacterCount].ascender = num30 - m_LineOffset;
				m_MaxDescender = (m_InternalTextElementInfo[m_CharacterCount].descender = num31 - m_LineOffset);
			}
			else
			{
				m_InternalTextElementInfo[m_CharacterCount].adjustedAscender = m_MaxLineAscender;
				m_InternalTextElementInfo[m_CharacterCount].adjustedDescender = m_MaxLineDescender;
				m_InternalTextElementInfo[m_CharacterCount].ascender = m_MaxLineAscender - m_LineOffset;
				m_MaxDescender = (m_InternalTextElementInfo[m_CharacterCount].descender = m_MaxLineDescender - m_LineOffset);
			}
			if (m_LineNumber == 0 && (flag12 || !flag9))
			{
				m_MaxAscender = m_MaxLineAscender;
				m_MaxCapHeight = Mathf.Max(m_MaxCapHeight, m_CurrentFontAsset.m_FaceInfo.capLine * num2 / num12);
			}
			if (m_LineOffset == 0f && (!flag9 || m_CharacterCount == m_FirstCharacterOfLine))
			{
				m_PageAscender = ((m_PageAscender > num30) ? m_PageAscender : num30);
			}
			if (num11 == 9 || num11 == 8203 || ((textWrappingMode == TextWrappingMode.PreserveWhitespace || textWrappingMode == TextWrappingMode.PreserveWhitespaceNoWrap) && (flag9 || num11 == 8203)) || (!flag9 && num11 != 8203 && num11 != 173 && num11 != 3) || (num11 == 173 && !flag7) || m_TextElementType == TextElementType.Sprite)
			{
				num6 = ((m_Width != -1f) ? Mathf.Min(x + 0.0001f - m_MarginLeft - m_MarginRight, m_Width) : (x + 0.0001f - m_MarginLeft - m_MarginRight));
				num9 = Round(Mathf.Abs(m_XAdvance) + glyphMetrics.horizontalAdvance * (1f - m_CharWidthAdjDelta) * ((num11 == 173) ? num20 : num2));
				if (flag10 && num9 > num6 && textWrappingMode != TextWrappingMode.NoWrap && textWrappingMode != TextWrappingMode.PreserveWhitespaceNoWrap && m_CharacterCount != m_FirstCharacterOfLine)
				{
					i = RestoreWordWrappingState(ref state, textInfo);
					if (m_InternalTextElementInfo[m_CharacterCount - 1].character == 173 && !flag7 && generationSettings.overflowMode == TextOverflowMode.Overflow)
					{
						characterSubstitution.index = m_CharacterCount - 1;
						characterSubstitution.unicode = 45u;
						i--;
						m_CharacterCount--;
						continue;
					}
					flag7 = false;
					if (m_InternalTextElementInfo[m_CharacterCount].character == 173)
					{
						flag7 = true;
						continue;
					}
					if (isTextAutoSizingEnabled && flag5)
					{
						if (m_CharWidthAdjDelta < 0f && m_AutoSizeIterationCount < m_AutoSizeMaxIterationCount)
						{
							float num34 = num9;
							if (m_CharWidthAdjDelta > 0f)
							{
								num34 /= 1f - m_CharWidthAdjDelta;
							}
							float num35 = num9 - (num6 - 0.0001f);
							m_CharWidthAdjDelta += num35 / num34;
							m_CharWidthAdjDelta = Mathf.Min(m_CharWidthAdjDelta, 0f);
							return Vector2.zero;
						}
						if (fontSize > 0f && m_AutoSizeIterationCount < m_AutoSizeMaxIterationCount)
						{
							m_MaxFontSize = fontSize;
							float num36 = Mathf.Max((fontSize - m_MinFontSize) / 2f, 0.05f);
							fontSize -= num36;
							fontSize = Mathf.Max((float)(int)(fontSize * 20f + 0.5f) / 20f, 0f);
						}
					}
					float num37 = m_MaxLineAscender - m_StartOfLineAscender;
					if (m_LineOffset > 0f && Math.Abs(num37) > 0.01f && !m_IsDrivenLineSpacing)
					{
						m_MaxDescender -= num37;
						m_LineOffset += num37;
					}
					float num38 = m_MaxLineAscender - m_LineOffset;
					float num39 = m_MaxLineDescender - m_LineOffset;
					m_MaxDescender = ((m_MaxDescender < num39) ? m_MaxDescender : num39);
					if (!flag4)
					{
						num10 = m_MaxDescender;
					}
					bool flag13 = false;
					m_FirstCharacterOfLine = m_CharacterCount;
					m_LineVisibleCharacterCount = 0;
					SaveWordWrappingState(ref state2, i, m_CharacterCount - 1, textInfo);
					m_LineNumber++;
					float adjustedAscender = m_InternalTextElementInfo[m_CharacterCount].adjustedAscender;
					if (m_LineHeight == -32767f)
					{
						m_LineOffset += 0f - m_MaxLineDescender + adjustedAscender + (num5 + m_LineSpacingDelta) * num + 0f * num3;
						m_IsDrivenLineSpacing = false;
					}
					else
					{
						m_LineOffset += m_LineHeight + 0f * num3;
						m_IsDrivenLineSpacing = true;
					}
					m_MaxLineAscender = -32767f;
					m_MaxLineDescender = 32767f;
					m_StartOfLineAscender = adjustedAscender;
					m_XAdvance = 0f + m_TagIndent;
					flag5 = true;
					continue;
				}
				num7 = Mathf.Max(num7, num9 + m_MarginLeft + m_MarginRight);
				num8 = Mathf.Max(num8, m_MaxAscender - m_MaxDescender);
			}
			if (m_LineOffset > 0f && !TextGeneratorUtilities.Approximately(m_MaxLineAscender, m_StartOfLineAscender) && !m_IsDrivenLineSpacing)
			{
				float num40 = m_MaxLineAscender - m_StartOfLineAscender;
				m_MaxDescender -= num40;
				m_LineOffset += num40;
				m_StartOfLineAscender += num40;
				state.lineOffset = m_LineOffset;
				state.startOfLineAscender = m_StartOfLineAscender;
			}
			switch (num11)
			{
			case 9u:
			{
				float num42 = m_CurrentFontAsset.faceInfo.tabWidth * (float)(int)m_CurrentFontAsset.tabMultiple * num2;
				float num43 = Mathf.Ceil(m_XAdvance / num42) * num42;
				m_XAdvance = ((num43 > m_XAdvance) ? num43 : (m_XAdvance + num42));
				break;
			}
			default:
				if (m_MonoSpacing != 0f)
				{
					float num41 = ((!m_DuoSpace || (num11 != 46 && num11 != 58 && num11 != 44)) ? (m_MonoSpacing - num28) : (m_MonoSpacing / 2f - num28));
					m_XAdvance += (num41 + (m_CurrentFontAsset.regularStyleSpacing + num21) * num3 + m_CSpacing) * (1f - m_CharWidthAdjDelta);
					if (flag9 || num11 == 8203)
					{
						m_XAdvance += generationSettings.wordSpacing * num3;
					}
				}
				else
				{
					m_XAdvance += ((glyphMetrics.horizontalAdvance * m_FXScale.x + glyphValueRecord.xAdvance) * num2 + (m_CurrentFontAsset.regularStyleSpacing + num21 + num29) * num3 + m_CSpacing) * (1f - m_CharWidthAdjDelta);
					if (flag9 || num11 == 8203)
					{
						m_XAdvance += generationSettings.wordSpacing * num3;
					}
				}
				break;
			case 8203u:
				break;
			}
			if (num11 == 13)
			{
				m_XAdvance = 0f + m_TagIndent;
			}
			if (num11 == 10 || num11 == 11 || num11 == 3 || num11 == 8232 || num11 == 8233 || m_CharacterCount == totalCharacterCount - 1)
			{
				float num44 = m_MaxLineAscender - m_StartOfLineAscender;
				if (m_LineOffset > 0f && Math.Abs(num44) > 0.01f && !m_IsDrivenLineSpacing)
				{
					m_MaxDescender -= num44;
					m_LineOffset += num44;
				}
				float num45 = m_MaxLineDescender - m_LineOffset;
				m_MaxDescender = ((m_MaxDescender < num45) ? m_MaxDescender : num45);
				if (num11 == 10 || num11 == 11 || num11 == 45 || num11 == 8232 || num11 == 8233)
				{
					SaveWordWrappingState(ref state2, i, m_CharacterCount, textInfo);
					SaveWordWrappingState(ref state, i, m_CharacterCount, textInfo);
					m_LineNumber++;
					m_FirstCharacterOfLine = m_CharacterCount + 1;
					float adjustedAscender2 = m_InternalTextElementInfo[m_CharacterCount].adjustedAscender;
					if (m_LineHeight == -32767f)
					{
						float num46 = 0f - m_MaxLineDescender + adjustedAscender2 + (num5 + m_LineSpacingDelta) * num + (0f + ((num11 == 10 || num11 == 8233) ? generationSettings.paragraphSpacing : 0f)) * num3;
						m_LineOffset += num46;
						m_IsDrivenLineSpacing = false;
					}
					else
					{
						m_LineOffset += m_LineHeight + (0f + ((num11 == 10 || num11 == 8233) ? generationSettings.paragraphSpacing : 0f)) * num3;
						m_IsDrivenLineSpacing = true;
					}
					m_MaxLineAscender = -32767f;
					m_MaxLineDescender = 32767f;
					m_StartOfLineAscender = adjustedAscender2;
					m_XAdvance = 0f + m_TagLineIndent + m_TagIndent;
					m_CharacterCount++;
					continue;
				}
				if (num11 == 3)
				{
					i = m_TextProcessingArray.Length;
				}
			}
			if ((textWrappingMode != TextWrappingMode.NoWrap && textWrappingMode != TextWrappingMode.PreserveWhitespaceNoWrap) || generationSettings.overflowMode == TextOverflowMode.Truncate || generationSettings.overflowMode == TextOverflowMode.Ellipsis)
			{
				bool flag14 = false;
				bool flag15 = false;
				if ((flag9 || num11 == 8203 || num11 == 45 || num11 == 173) && (!m_IsNonBreakingSpace || flag6) && num11 != 160 && num11 != 8199 && num11 != 8209 && num11 != 8239 && num11 != 8288)
				{
					if (num11 != 45 || m_CharacterCount <= 0 || !char.IsWhiteSpace((char)textInfo.textElementInfo[m_CharacterCount - 1].character))
					{
						flag5 = false;
						flag14 = true;
						state3.previousWordBreak = -1;
					}
				}
				else if (!m_IsNonBreakingSpace && ((TextGeneratorUtilities.IsHangul(num11) && !textSettings.lineBreakingRules.useModernHangulLineBreakingRules) || TextGeneratorUtilities.IsCJK(num11)))
				{
					bool flag16 = textSettings.lineBreakingRules.leadingCharactersLookup.Contains(num11);
					bool flag17 = m_CharacterCount < totalCharacterCount - 1 && textSettings.lineBreakingRules.leadingCharactersLookup.Contains(m_InternalTextElementInfo[m_CharacterCount + 1].character);
					if (!flag16)
					{
						if (!flag17)
						{
							flag5 = false;
							flag14 = true;
						}
						if (flag5)
						{
							if (flag9)
							{
								flag15 = true;
							}
							flag14 = true;
						}
					}
					else if (flag5 && flag12)
					{
						if (flag9)
						{
							flag15 = true;
						}
						flag14 = true;
					}
				}
				else if (!m_IsNonBreakingSpace && m_CharacterCount + 1 < totalCharacterCount && TextGeneratorUtilities.IsCJK(textInfo.textElementInfo[m_CharacterCount + 1].character))
				{
					uint character = textInfo.textElementInfo[m_CharacterCount + 1].character;
					bool flag18 = textSettings.lineBreakingRules.leadingCharactersLookup.Contains(num11);
					bool flag19 = textSettings.lineBreakingRules.leadingCharactersLookup.Contains(character);
					if (!flag18 && !flag19)
					{
						flag14 = true;
					}
				}
				else if (flag5)
				{
					if ((flag9 && num11 != 160) || (num11 == 173 && !flag7))
					{
						flag15 = true;
					}
					flag14 = true;
				}
				if (flag14)
				{
					SaveWordWrappingState(ref state, i, m_CharacterCount, textInfo);
				}
				if (flag15)
				{
					SaveWordWrappingState(ref state3, i, m_CharacterCount, textInfo);
				}
			}
			m_CharacterCount++;
		}
		num4 = m_MaxFontSize - m_MinFontSize;
		if (isTextAutoSizingEnabled && num4 > 0.051f && fontSize < 0f && m_AutoSizeIterationCount < m_AutoSizeMaxIterationCount)
		{
			if (m_CharWidthAdjDelta < 0f)
			{
				m_CharWidthAdjDelta = 0f;
			}
			m_MinFontSize = fontSize;
			float num47 = Mathf.Max((m_MaxFontSize - fontSize) / 2f, 0.05f);
			fontSize += num47;
			fontSize = Mathf.Min((float)(int)(fontSize * 20f + 0.5f) / 20f, 0f);
			return Vector2.zero;
		}
		m_IsCalculatingPreferredValues = false;
		if (NeedToRound)
		{
			Debug.AssertFormat(num7 == Mathf.Round(num7), "renderedWidth was not rounded: {0}", num7);
		}
		else
		{
			if (num7 != 0f)
			{
				num7 = (float)(int)(num7 * 100f + 1f) / 100f;
			}
			if (num8 != 0f)
			{
				num8 = (float)(int)(num8 * 100f + 1f) / 100f;
			}
		}
		return new Vector2(num7, num8);
	}

	[VisibleToOtherModules(new string[] { "UnityEngine.UIElementsModule" })]
	internal void Prepare(TextGenerationSettings generationSettings, TextInfo textInfo)
	{
		m_Padding = generationSettings.extraPadding;
		m_CurrentFontAsset = generationSettings.fontAsset;
		m_ShouldRenderBitmap = generationSettings.fontAsset.IsBitmap();
		m_FontStyleInternal = generationSettings.fontStyle;
		m_FontWeightInternal = (((m_FontStyleInternal & FontStyles.Bold) == FontStyles.Bold) ? TextFontWeight.Bold : generationSettings.fontWeight);
		GetSpecialCharacters(generationSettings);
		ComputeMarginSize(generationSettings.screenRect, Vector4.zero);
		PopulateTextBackingArray(generationSettings.renderedText);
		PopulateTextProcessingArray(generationSettings);
		SetArraySizes(m_TextProcessingArray, generationSettings, textInfo);
		bool flag = false;
		m_FontSize = generationSettings.fontSize;
		m_MaxFontSize = 0f;
		m_MinFontSize = 0f;
		m_LineSpacingDelta = 0f;
		m_CharWidthAdjDelta = 0f;
	}

	internal bool PrepareFontAsset(TextGenerationSettings generationSettings)
	{
		m_CurrentFontAsset = generationSettings.fontAsset;
		m_FontStyleInternal = generationSettings.fontStyle;
		m_FontWeightInternal = (((m_FontStyleInternal & FontStyles.Bold) == FontStyles.Bold) ? TextFontWeight.Bold : generationSettings.fontWeight);
		if (!GetSpecialCharacters(generationSettings))
		{
			return false;
		}
		PopulateTextBackingArray(generationSettings.renderedText);
		PopulateTextProcessingArray(generationSettings);
		return PopulateFontAsset(generationSettings, m_TextProcessingArray);
	}

	private int SetArraySizes(TextProcessingElement[] textProcessingArray, TextGenerationSettings generationSettings, TextInfo textInfo)
	{
		TextSettings textSettings = generationSettings.textSettings;
		int num = 0;
		m_TotalCharacterCount = 0;
		m_isTextLayoutPhase = false;
		m_TagNoParsing = false;
		m_FontStyleInternal = generationSettings.fontStyle;
		m_FontStyleStack.Clear();
		m_FontWeightInternal = (((m_FontStyleInternal & FontStyles.Bold) == FontStyles.Bold) ? TextFontWeight.Bold : generationSettings.fontWeight);
		m_FontWeightStack.SetDefault(m_FontWeightInternal);
		m_CurrentFontAsset = generationSettings.fontAsset;
		m_CurrentMaterial = generationSettings.fontAsset.material;
		m_CurrentMaterialIndex = 0;
		m_MaterialReferenceStack.SetDefault(new MaterialReference(m_CurrentMaterialIndex, m_CurrentFontAsset, null, m_CurrentMaterial, m_Padding));
		m_MaterialReferenceIndexLookup.Clear();
		MaterialReference.AddMaterialReference(m_CurrentMaterial, m_CurrentFontAsset, ref m_MaterialReferences, m_MaterialReferenceIndexLookup);
		m_CurrentSpriteAsset = null;
		if (textInfo == null)
		{
			textInfo = new TextInfo();
		}
		else if (textInfo.textElementInfo.Length < m_InternalTextProcessingArraySize)
		{
			TextInfo.Resize(ref textInfo.textElementInfo, m_InternalTextProcessingArraySize, isBlockAllocated: false);
		}
		m_TextElementType = TextElementType.Character;
		if (generationSettings.overflowMode == TextOverflowMode.Ellipsis)
		{
			GetEllipsisSpecialCharacter(generationSettings);
			if (m_Ellipsis.character != null)
			{
				if (m_Ellipsis.fontAsset.GetHashCode() != m_CurrentFontAsset.GetHashCode())
				{
					if (textSettings.matchMaterialPreset && m_CurrentMaterial.GetHashCode() != m_Ellipsis.fontAsset.material.GetHashCode())
					{
						m_Ellipsis.material = MaterialManager.GetFallbackMaterial(m_CurrentMaterial, m_Ellipsis.fontAsset.material);
					}
					else
					{
						m_Ellipsis.material = m_Ellipsis.fontAsset.material;
					}
					m_Ellipsis.materialIndex = MaterialReference.AddMaterialReference(m_Ellipsis.material, m_Ellipsis.fontAsset, ref m_MaterialReferences, m_MaterialReferenceIndexLookup);
					m_MaterialReferences[m_Ellipsis.materialIndex].referenceCount = 0;
				}
			}
			else
			{
				generationSettings.overflowMode = TextOverflowMode.Truncate;
				if (textSettings.displayWarnings)
				{
					Debug.LogWarning("The character used for Ellipsis is not available in font asset [" + m_CurrentFontAsset.name + "] or any potential fallbacks. Switching Text Overflow mode to Truncate.");
				}
			}
		}
		bool flag = TextGenerationSettings.fontFeatures.Contains(OTL_FeatureTag.liga);
		for (int i = 0; i < textProcessingArray.Length && textProcessingArray[i].unicode != 0; i++)
		{
			if (textInfo.textElementInfo == null || m_TotalCharacterCount >= textInfo.textElementInfo.Length)
			{
				TextInfo.Resize(ref textInfo.textElementInfo, m_TotalCharacterCount + 1, isBlockAllocated: true);
			}
			uint num2 = textProcessingArray[i].unicode;
			int currentMaterialIndex = m_CurrentMaterialIndex;
			if (generationSettings.richText && num2 == 60)
			{
				currentMaterialIndex = m_CurrentMaterialIndex;
				if (ValidateHtmlTag(textProcessingArray, i + 1, out var endIndex, generationSettings, textInfo, out var _))
				{
					int stringIndex = textProcessingArray[i].stringIndex;
					i = endIndex;
					if (m_TextElementType == TextElementType.Sprite)
					{
						m_MaterialReferences[m_CurrentMaterialIndex].referenceCount++;
						textInfo.textElementInfo[m_TotalCharacterCount].character = (ushort)(57344 + m_SpriteIndex);
						textInfo.textElementInfo[m_TotalCharacterCount].fontAsset = m_CurrentFontAsset;
						textInfo.textElementInfo[m_TotalCharacterCount].materialReferenceIndex = m_CurrentMaterialIndex;
						textInfo.textElementInfo[m_TotalCharacterCount].textElement = m_CurrentSpriteAsset.spriteCharacterTable[m_SpriteIndex];
						textInfo.textElementInfo[m_TotalCharacterCount].elementType = m_TextElementType;
						textInfo.textElementInfo[m_TotalCharacterCount].index = stringIndex;
						textInfo.textElementInfo[m_TotalCharacterCount].stringLength = textProcessingArray[i].stringIndex - stringIndex + 1;
						m_TextElementType = TextElementType.Character;
						m_CurrentMaterialIndex = currentMaterialIndex;
						num++;
						m_TotalCharacterCount++;
					}
					continue;
				}
			}
			bool isAlternativeTypeface = false;
			bool flag2 = false;
			FontAsset currentFontAsset = m_CurrentFontAsset;
			Material currentMaterial = m_CurrentMaterial;
			currentMaterialIndex = m_CurrentMaterialIndex;
			if (m_TextElementType == TextElementType.Character)
			{
				if ((m_FontStyleInternal & FontStyles.UpperCase) == FontStyles.UpperCase)
				{
					if (char.IsLower((char)num2))
					{
						num2 = char.ToUpper((char)num2);
					}
				}
				else if ((m_FontStyleInternal & FontStyles.LowerCase) == FontStyles.LowerCase)
				{
					if (char.IsUpper((char)num2))
					{
						num2 = char.ToLower((char)num2);
					}
				}
				else if ((m_FontStyleInternal & FontStyles.SmallCaps) == FontStyles.SmallCaps && char.IsLower((char)num2))
				{
					num2 = char.ToUpper((char)num2);
				}
			}
			TextElement textElement = null;
			uint num3 = ((i + 1 < textProcessingArray.Length) ? textProcessingArray[i + 1].unicode : 0u);
			if (generationSettings.emojiFallbackSupport && ((TextGeneratorUtilities.IsEmojiPresentationForm(num2) && num3 != 65038) || (TextGeneratorUtilities.IsEmoji(num2) && num3 == 65039)) && textSettings.emojiFallbackTextAssets != null && textSettings.emojiFallbackTextAssets.Count > 0)
			{
				textElement = FontAssetUtilities.GetTextElementFromTextAssets(num2, m_CurrentFontAsset, textSettings.emojiFallbackTextAssets, includeFallbacks: true, m_FontStyleInternal, m_FontWeightInternal, out isAlternativeTypeface, flag);
				if (textElement == null)
				{
				}
			}
			if (textElement == null)
			{
				textElement = GetTextElement(generationSettings, num2, m_CurrentFontAsset, m_FontStyleInternal, m_FontWeightInternal, out isAlternativeTypeface, flag);
			}
			if (textElement == null)
			{
				DoMissingGlyphCallback(num2, textProcessingArray[i].stringIndex, m_CurrentFontAsset, textInfo);
				uint num4 = num2;
				num2 = (textProcessingArray[i].unicode = ((textSettings.missingCharacterUnicode == 0) ? 9633u : ((uint)textSettings.missingCharacterUnicode)));
				textElement = FontAssetUtilities.GetCharacterFromFontAsset(num2, m_CurrentFontAsset, includeFallbacks: true, m_FontStyleInternal, m_FontWeightInternal, out isAlternativeTypeface, flag);
				if (textElement == null)
				{
					textElement = FontAssetUtilities.GetCharacterFromFontAssetsInternal(num2, m_CurrentFontAsset, textSettings.GetFallbackFontAssets(m_CurrentFontAsset.IsRaster(), m_ShouldRenderBitmap ? generationSettings.fontSize : (-1)), textSettings.fallbackOSFontAssets, includeFallbacks: true, m_FontStyleInternal, m_FontWeightInternal, out isAlternativeTypeface, flag);
				}
				if (textElement == null && textSettings.defaultFontAsset != null)
				{
					textElement = FontAssetUtilities.GetCharacterFromFontAsset(num2, textSettings.defaultFontAsset, includeFallbacks: true, m_FontStyleInternal, m_FontWeightInternal, out isAlternativeTypeface, flag);
				}
				if (textElement == null)
				{
					num2 = (textProcessingArray[i].unicode = 32u);
					textElement = FontAssetUtilities.GetCharacterFromFontAsset(num2, m_CurrentFontAsset, includeFallbacks: true, m_FontStyleInternal, m_FontWeightInternal, out isAlternativeTypeface, flag);
				}
				if (textElement == null)
				{
					num2 = (textProcessingArray[i].unicode = 3u);
					textElement = FontAssetUtilities.GetCharacterFromFontAsset(num2, m_CurrentFontAsset, includeFallbacks: true, m_FontStyleInternal, m_FontWeightInternal, out isAlternativeTypeface, flag);
				}
				if (textSettings.displayWarnings)
				{
					bool flag3 = !JobsUtility.IsExecutingJob;
					string message = ((num4 > 65535) ? $"The character with Unicode value \\U{num4:X8} was not found in the [{(flag3 ? generationSettings.fontAsset.name : ((object)generationSettings.fontAsset.GetHashCode()))}] font asset or any potential fallbacks. It was replaced by Unicode character \\u{textElement.unicode:X4}." : $"The character with Unicode value \\u{num4:X4} was not found in the [{(flag3 ? generationSettings.fontAsset.name : ((object)generationSettings.fontAsset.GetHashCode()))}] font asset or any potential fallbacks. It was replaced by Unicode character \\u{textElement.unicode:X4}.");
					Debug.LogWarning(message);
				}
			}
			textInfo.textElementInfo[m_TotalCharacterCount].alternativeGlyph = null;
			if (textElement.elementType == TextElementType.Character)
			{
				if (textElement.textAsset.instanceID != m_CurrentFontAsset.instanceID)
				{
					flag2 = true;
					m_CurrentFontAsset = textElement.textAsset as FontAsset;
				}
				if ((num3 >= 65024 && num3 <= 65039) || (num3 >= 917760 && num3 <= 917999))
				{
					if (!m_CurrentFontAsset.TryGetGlyphVariantIndexInternal(num2, num3, out var variantGlyphIndex))
					{
						variantGlyphIndex = m_CurrentFontAsset.GetGlyphVariantIndex(num2, num3);
						m_CurrentFontAsset.TryAddGlyphVariantIndexInternal(num2, num3, variantGlyphIndex);
					}
					if (variantGlyphIndex != 0 && m_CurrentFontAsset.TryAddGlyphInternal(variantGlyphIndex, out var glyph))
					{
						textInfo.textElementInfo[m_TotalCharacterCount].alternativeGlyph = glyph;
					}
					textProcessingArray[i + 1].unicode = 26u;
					i++;
				}
				if (flag && m_CurrentFontAsset.fontFeatureTable.m_LigatureSubstitutionRecordLookup.TryGetValue(textElement.glyphIndex, out var value))
				{
					if (value == null)
					{
						break;
					}
					for (int j = 0; j < value.Count; j++)
					{
						LigatureSubstitutionRecord ligatureSubstitutionRecord = value[j];
						int num5 = ligatureSubstitutionRecord.componentGlyphIDs.Length;
						uint num6 = ligatureSubstitutionRecord.ligatureGlyphID;
						for (int k = 1; k < num5; k++)
						{
							uint unicode = textProcessingArray[i + k].unicode;
							bool success;
							uint glyphIndex = m_CurrentFontAsset.GetGlyphIndex(unicode, out success);
							if (glyphIndex != ligatureSubstitutionRecord.componentGlyphIDs[k])
							{
								num6 = 0u;
								break;
							}
						}
						if (num6 == 0 || !m_CurrentFontAsset.TryAddGlyphInternal(num6, out var glyph2))
						{
							continue;
						}
						textInfo.textElementInfo[m_TotalCharacterCount].alternativeGlyph = glyph2;
						for (int l = 0; l < num5; l++)
						{
							if (l == 0)
							{
								textProcessingArray[i + l].length = num5;
							}
							else
							{
								textProcessingArray[i + l].unicode = 26u;
							}
						}
						i += num5 - 1;
						break;
					}
				}
			}
			textInfo.textElementInfo[m_TotalCharacterCount].elementType = TextElementType.Character;
			textInfo.textElementInfo[m_TotalCharacterCount].textElement = textElement;
			textInfo.textElementInfo[m_TotalCharacterCount].isUsingAlternateTypeface = isAlternativeTypeface;
			textInfo.textElementInfo[m_TotalCharacterCount].character = (ushort)num2;
			textInfo.textElementInfo[m_TotalCharacterCount].index = textProcessingArray[i].stringIndex;
			textInfo.textElementInfo[m_TotalCharacterCount].stringLength = textProcessingArray[i].length;
			textInfo.textElementInfo[m_TotalCharacterCount].fontAsset = m_CurrentFontAsset;
			if (textElement.elementType == TextElementType.Sprite)
			{
				SpriteAsset spriteAsset = textElement.textAsset as SpriteAsset;
				m_CurrentMaterialIndex = MaterialReference.AddMaterialReference(spriteAsset.material, spriteAsset, ref m_MaterialReferences, m_MaterialReferenceIndexLookup);
				m_MaterialReferences[m_CurrentMaterialIndex].referenceCount++;
				textInfo.textElementInfo[m_TotalCharacterCount].elementType = TextElementType.Sprite;
				textInfo.textElementInfo[m_TotalCharacterCount].materialReferenceIndex = m_CurrentMaterialIndex;
				m_TextElementType = TextElementType.Character;
				m_CurrentMaterialIndex = currentMaterialIndex;
				num++;
				m_TotalCharacterCount++;
				continue;
			}
			if (flag2 && m_CurrentFontAsset.instanceID != generationSettings.fontAsset.instanceID)
			{
				if (textSettings.matchMaterialPreset)
				{
					m_CurrentMaterial = MaterialManager.GetFallbackMaterial(m_CurrentMaterial, m_CurrentFontAsset.material);
				}
				else
				{
					m_CurrentMaterial = m_CurrentFontAsset.material;
				}
				m_CurrentMaterialIndex = MaterialReference.AddMaterialReference(m_CurrentMaterial, m_CurrentFontAsset, ref m_MaterialReferences, m_MaterialReferenceIndexLookup);
			}
			if (textElement != null && textElement.glyph.atlasIndex > 0)
			{
				m_CurrentMaterial = MaterialManager.GetFallbackMaterial(m_CurrentFontAsset, m_CurrentMaterial, textElement.glyph.atlasIndex);
				m_CurrentMaterialIndex = MaterialReference.AddMaterialReference(m_CurrentMaterial, m_CurrentFontAsset, ref m_MaterialReferences, m_MaterialReferenceIndexLookup);
				flag2 = true;
			}
			if (!char.IsWhiteSpace((char)num2) && num2 != 8203)
			{
				if (generationSettings.isIMGUI && m_MaterialReferences[m_CurrentMaterialIndex].referenceCount >= 16383)
				{
					m_CurrentMaterialIndex = MaterialReference.AddMaterialReference(new Material(m_CurrentMaterial), m_CurrentFontAsset, ref m_MaterialReferences, m_MaterialReferenceIndexLookup);
				}
				m_MaterialReferences[m_CurrentMaterialIndex].referenceCount++;
			}
			textInfo.textElementInfo[m_TotalCharacterCount].material = m_CurrentMaterial;
			textInfo.textElementInfo[m_TotalCharacterCount].materialReferenceIndex = m_CurrentMaterialIndex;
			m_MaterialReferences[m_CurrentMaterialIndex].isFallbackMaterial = flag2;
			if (flag2)
			{
				m_MaterialReferences[m_CurrentMaterialIndex].fallbackMaterial = currentMaterial;
				m_CurrentFontAsset = currentFontAsset;
				m_CurrentMaterial = currentMaterial;
				m_CurrentMaterialIndex = currentMaterialIndex;
			}
			m_TotalCharacterCount++;
		}
		if (m_IsCalculatingPreferredValues)
		{
			m_IsCalculatingPreferredValues = false;
			return m_TotalCharacterCount;
		}
		textInfo.spriteCount = num;
		int num7 = (textInfo.materialCount = m_MaterialReferenceIndexLookup.Count);
		if (num7 > textInfo.meshInfo.Length)
		{
			TextInfo.Resize(ref textInfo.meshInfo, num7, isBlockAllocated: false);
		}
		if (m_VertexBufferAutoSizeReduction && textInfo.textElementInfo.Length - m_TotalCharacterCount > 256)
		{
			TextInfo.Resize(ref textInfo.textElementInfo, Mathf.Max(m_TotalCharacterCount + 1, 256), isBlockAllocated: true);
		}
		for (int m = 0; m < num7; m++)
		{
			int referenceCount = m_MaterialReferences[m].referenceCount;
			if (textInfo.meshInfo[m].vertexData == null || textInfo.meshInfo[m].vertexBufferSize < referenceCount * 4)
			{
				if (textInfo.meshInfo[m].vertexData == null)
				{
					textInfo.meshInfo[m] = new MeshInfo(referenceCount + 1, generationSettings.isIMGUI);
				}
				else
				{
					textInfo.meshInfo[m].ResizeMeshInfo((referenceCount > 1024) ? (referenceCount + 256) : Mathf.NextPowerOfTwo(referenceCount), generationSettings.isIMGUI);
				}
			}
			else if (textInfo.meshInfo[m].vertexBufferSize - referenceCount * 4 > 1024)
			{
				textInfo.meshInfo[m].ResizeMeshInfo((referenceCount > 1024) ? (referenceCount + 256) : Mathf.Max(Mathf.NextPowerOfTwo(referenceCount), 256), generationSettings.isIMGUI);
			}
			textInfo.meshInfo[m].material = m_MaterialReferences[m].material;
			textInfo.meshInfo[m].glyphRenderMode = m_MaterialReferences[m].fontAsset.atlasRenderMode;
		}
		return m_TotalCharacterCount;
	}

	private TextElement GetTextElement(TextGenerationSettings generationSettings, uint unicode, FontAsset fontAsset, FontStyles fontStyle, TextFontWeight fontWeight, out bool isUsingAlternativeTypeface, bool populateLigatures)
	{
		bool flag = !IsExecutingJob;
		TextSettings textSettings = generationSettings.textSettings;
		Character character = FontAssetUtilities.GetCharacterFromFontAsset(unicode, fontAsset, includeFallbacks: false, fontStyle, fontWeight, out isUsingAlternativeTypeface, populateLigatures);
		if (character != null)
		{
			return character;
		}
		if (!flag && (fontAsset.atlasPopulationMode == AtlasPopulationMode.Dynamic || fontAsset.atlasPopulationMode == AtlasPopulationMode.DynamicOS))
		{
			return null;
		}
		if (fontAsset.m_FallbackFontAssetTable != null && fontAsset.m_FallbackFontAssetTable.Count > 0)
		{
			character = FontAssetUtilities.GetCharacterFromFontAssetsInternal(unicode, fontAsset, fontAsset.m_FallbackFontAssetTable, null, includeFallbacks: true, fontStyle, fontWeight, out isUsingAlternativeTypeface, populateLigatures);
		}
		if (character != null)
		{
			if (isUsingAlternativeTypeface)
			{
				fontAsset.AddCharacterToLookupCache(unicode, character, fontStyle, fontWeight);
			}
			else
			{
				fontAsset.AddCharacterToLookupCache(unicode, character, FontStyles.Normal, TextFontWeight.Regular);
			}
			return character;
		}
		if (!(flag ? (fontAsset.instanceID == generationSettings.fontAsset.instanceID) : (fontAsset == generationSettings.fontAsset)))
		{
			character = FontAssetUtilities.GetCharacterFromFontAsset(unicode, generationSettings.fontAsset, includeFallbacks: false, fontStyle, fontWeight, out isUsingAlternativeTypeface, populateLigatures);
			if (character != null)
			{
				m_CurrentMaterialIndex = 0;
				m_CurrentMaterial = m_MaterialReferences[0].material;
				fontAsset.AddCharacterToLookupCache(unicode, character, fontStyle, fontWeight);
				return character;
			}
			if (generationSettings.fontAsset.m_FallbackFontAssetTable != null && generationSettings.fontAsset.m_FallbackFontAssetTable.Count > 0)
			{
				character = FontAssetUtilities.GetCharacterFromFontAssetsInternal(unicode, fontAsset, generationSettings.fontAsset.m_FallbackFontAssetTable, null, includeFallbacks: true, fontStyle, fontWeight, out isUsingAlternativeTypeface, populateLigatures);
			}
			if (character != null)
			{
				if (isUsingAlternativeTypeface)
				{
					fontAsset.AddCharacterToLookupCache(unicode, character, fontStyle, fontWeight);
				}
				else
				{
					fontAsset.AddCharacterToLookupCache(unicode, character, FontStyles.Normal, TextFontWeight.Regular);
				}
				return character;
			}
		}
		if (textSettings.GetStaticFallbackOSFontAsset() == null && !flag)
		{
			return null;
		}
		character = FontAssetUtilities.GetCharacterFromFontAssetsInternal(unicode, fontAsset, textSettings.GetFallbackFontAssets(fontAsset.IsRaster(), m_ShouldRenderBitmap ? generationSettings.fontSize : (-1)), textSettings.fallbackOSFontAssets, includeFallbacks: true, fontStyle, fontWeight, out isUsingAlternativeTypeface, populateLigatures);
		if (character != null)
		{
			if (isUsingAlternativeTypeface)
			{
				fontAsset.AddCharacterToLookupCache(unicode, character, fontStyle, fontWeight);
			}
			else
			{
				fontAsset.AddCharacterToLookupCache(unicode, character, FontStyles.Normal, TextFontWeight.Regular);
			}
			return character;
		}
		if (textSettings.defaultFontAsset != null)
		{
			character = FontAssetUtilities.GetCharacterFromFontAsset(unicode, textSettings.defaultFontAsset, includeFallbacks: true, fontStyle, fontWeight, out isUsingAlternativeTypeface, populateLigatures);
		}
		if (character != null)
		{
			if (isUsingAlternativeTypeface)
			{
				fontAsset.AddCharacterToLookupCache(unicode, character, fontStyle, fontWeight);
			}
			else
			{
				fontAsset.AddCharacterToLookupCache(unicode, character, FontStyles.Normal, TextFontWeight.Regular);
			}
			return character;
		}
		if (textSettings.defaultSpriteAsset != null)
		{
			if (!flag && textSettings.defaultSpriteAsset.m_SpriteCharacterLookup == null)
			{
				return null;
			}
			SpriteCharacter spriteCharacterFromSpriteAsset = FontAssetUtilities.GetSpriteCharacterFromSpriteAsset(unicode, textSettings.defaultSpriteAsset, includeFallbacks: true);
			if (spriteCharacterFromSpriteAsset != null)
			{
				return spriteCharacterFromSpriteAsset;
			}
		}
		return null;
	}

	private void PopulateTextBackingArray(in RenderedText sourceText)
	{
		int num = 0;
		int characterCount = sourceText.CharacterCount;
		if (characterCount >= m_TextBackingArray.Capacity)
		{
			m_TextBackingArray.Resize(characterCount);
		}
		RenderedText.Enumerator enumerator = sourceText.GetEnumerator();
		while (enumerator.MoveNext())
		{
			char current = enumerator.Current;
			m_TextBackingArray[num] = current;
			num++;
		}
		m_TextBackingArray[num] = 0u;
		m_TextBackingArray.Count = num;
	}

	private void PopulateTextProcessingArray(TextGenerationSettings generationSettings)
	{
		int count = m_TextBackingArray.Count;
		if (m_TextProcessingArray.Length < count)
		{
			TextGeneratorUtilities.ResizeInternalArray(ref m_TextProcessingArray, count);
		}
		TextProcessingStack<int>.SetDefault(m_TextStyleStacks, 0);
		m_TextStyleStackDepth = 0;
		int writeIndex = 0;
		int hashCode = m_TextStyleStacks[0].Pop();
		TextStyle style = TextGeneratorUtilities.GetStyle(generationSettings, hashCode);
		if (style != null && style.hashCode != -1183493901)
		{
			TextGeneratorUtilities.InsertOpeningStyleTag(style, ref m_TextProcessingArray, ref writeIndex, ref m_TextStyleStackDepth, ref m_TextStyleStacks, ref generationSettings);
		}
		bool flag = false;
		for (int i = 0; i < count; i++)
		{
			uint num = m_TextBackingArray[i];
			if (num == 0)
			{
				break;
			}
			if (num == 92 && i < count - 1)
			{
				switch (m_TextBackingArray[i + 1])
				{
				case 92u:
					if (generationSettings.parseControlCharacters)
					{
						i++;
					}
					break;
				case 110u:
					if (!generationSettings.parseControlCharacters)
					{
						break;
					}
					m_TextProcessingArray[writeIndex] = new TextProcessingElement
					{
						elementType = TextProcessingElementType.TextCharacterElement,
						stringIndex = i,
						length = 1,
						unicode = 10u
					};
					i++;
					writeIndex++;
					continue;
				case 114u:
					if (!generationSettings.parseControlCharacters)
					{
						break;
					}
					m_TextProcessingArray[writeIndex] = new TextProcessingElement
					{
						elementType = TextProcessingElementType.TextCharacterElement,
						stringIndex = i,
						length = 1,
						unicode = 13u
					};
					i++;
					writeIndex++;
					continue;
				case 116u:
					if (!generationSettings.parseControlCharacters)
					{
						break;
					}
					m_TextProcessingArray[writeIndex] = new TextProcessingElement
					{
						elementType = TextProcessingElementType.TextCharacterElement,
						stringIndex = i,
						length = 1,
						unicode = 9u
					};
					i++;
					writeIndex++;
					continue;
				case 118u:
					if (!generationSettings.parseControlCharacters)
					{
						break;
					}
					m_TextProcessingArray[writeIndex] = new TextProcessingElement
					{
						elementType = TextProcessingElementType.TextCharacterElement,
						stringIndex = i,
						length = 1,
						unicode = 11u
					};
					i++;
					writeIndex++;
					continue;
				case 117u:
					if (!generationSettings.parseControlCharacters || count <= i + 5 || !TextGeneratorUtilities.IsValidUTF16(m_TextBackingArray, i + 2))
					{
						break;
					}
					m_TextProcessingArray[writeIndex] = new TextProcessingElement
					{
						elementType = TextProcessingElementType.TextCharacterElement,
						stringIndex = i,
						length = 6,
						unicode = TextGeneratorUtilities.GetUTF16(m_TextBackingArray, i + 2)
					};
					i += 5;
					writeIndex++;
					continue;
				case 85u:
					if (!generationSettings.parseControlCharacters || count <= i + 9 || !TextGeneratorUtilities.IsValidUTF32(m_TextBackingArray, i + 2))
					{
						break;
					}
					m_TextProcessingArray[writeIndex] = new TextProcessingElement
					{
						elementType = TextProcessingElementType.TextCharacterElement,
						stringIndex = i,
						length = 10,
						unicode = TextGeneratorUtilities.GetUTF32(m_TextBackingArray, i + 2)
					};
					i += 9;
					writeIndex++;
					continue;
				}
			}
			if (num >= 55296 && num <= 56319 && count > i + 1 && m_TextBackingArray[i + 1] >= 56320 && m_TextBackingArray[i + 1] <= 57343)
			{
				m_TextProcessingArray[writeIndex] = new TextProcessingElement
				{
					elementType = TextProcessingElementType.TextCharacterElement,
					stringIndex = i,
					length = 2,
					unicode = TextGeneratorUtilities.ConvertToUTF32(num, m_TextBackingArray[i + 1])
				};
				i++;
				writeIndex++;
				continue;
			}
			if (num == 13 && i + 1 < count && m_TextBackingArray[i + 1] == 10)
			{
				m_TextProcessingArray[writeIndex] = new TextProcessingElement
				{
					elementType = TextProcessingElementType.TextCharacterElement,
					stringIndex = i,
					length = 2,
					unicode = 10u
				};
				i++;
				writeIndex++;
				continue;
			}
			if (num == 60 && generationSettings.richText)
			{
				switch ((MarkupTag)TextGeneratorUtilities.GetMarkupTagHashCode(m_TextBackingArray, i + 1))
				{
				case MarkupTag.NO_PARSE:
					flag = true;
					break;
				case MarkupTag.SLASH_NO_PARSE:
					flag = false;
					break;
				case MarkupTag.BR:
					if (flag)
					{
						break;
					}
					if (writeIndex == m_TextProcessingArray.Length)
					{
						TextGeneratorUtilities.ResizeInternalArray(ref m_TextProcessingArray);
					}
					m_TextProcessingArray[writeIndex] = new TextProcessingElement
					{
						elementType = TextProcessingElementType.TextCharacterElement,
						stringIndex = i,
						length = 4,
						unicode = 10u
					};
					writeIndex++;
					i += 3;
					continue;
				case MarkupTag.CR:
					if (flag)
					{
						break;
					}
					if (writeIndex == m_TextProcessingArray.Length)
					{
						TextGeneratorUtilities.ResizeInternalArray(ref m_TextProcessingArray);
					}
					m_TextProcessingArray[writeIndex] = new TextProcessingElement
					{
						elementType = TextProcessingElementType.TextCharacterElement,
						stringIndex = i,
						length = 4,
						unicode = 13u
					};
					writeIndex++;
					i += 3;
					continue;
				case MarkupTag.NBSP:
					if (flag)
					{
						break;
					}
					if (writeIndex == m_TextProcessingArray.Length)
					{
						TextGeneratorUtilities.ResizeInternalArray(ref m_TextProcessingArray);
					}
					m_TextProcessingArray[writeIndex] = new TextProcessingElement
					{
						elementType = TextProcessingElementType.TextCharacterElement,
						stringIndex = i,
						length = 6,
						unicode = 160u
					};
					writeIndex++;
					i += 5;
					continue;
				case MarkupTag.ZWSP:
					if (flag)
					{
						break;
					}
					if (writeIndex == m_TextProcessingArray.Length)
					{
						TextGeneratorUtilities.ResizeInternalArray(ref m_TextProcessingArray);
					}
					m_TextProcessingArray[writeIndex] = new TextProcessingElement
					{
						elementType = TextProcessingElementType.TextCharacterElement,
						stringIndex = i,
						length = 6,
						unicode = 8203u
					};
					writeIndex++;
					i += 5;
					continue;
				case MarkupTag.ZWJ:
					if (flag)
					{
						break;
					}
					if (writeIndex == m_TextProcessingArray.Length)
					{
						TextGeneratorUtilities.ResizeInternalArray(ref m_TextProcessingArray);
					}
					m_TextProcessingArray[writeIndex] = new TextProcessingElement
					{
						elementType = TextProcessingElementType.TextCharacterElement,
						stringIndex = i,
						length = 5,
						unicode = 8205u
					};
					writeIndex++;
					i += 4;
					continue;
				case MarkupTag.SHY:
					if (flag)
					{
						break;
					}
					if (writeIndex == m_TextProcessingArray.Length)
					{
						TextGeneratorUtilities.ResizeInternalArray(ref m_TextProcessingArray);
					}
					m_TextProcessingArray[writeIndex] = new TextProcessingElement
					{
						elementType = TextProcessingElementType.TextCharacterElement,
						stringIndex = i,
						length = 5,
						unicode = 173u
					};
					writeIndex++;
					i += 4;
					continue;
				case MarkupTag.A:
					if (m_TextBackingArray.Count > i + 4 && m_TextBackingArray[i + 3] == 104 && m_TextBackingArray[i + 4] == 114)
					{
						TextGeneratorUtilities.InsertOpeningTextStyle(TextGeneratorUtilities.GetStyle(generationSettings, 65), ref m_TextProcessingArray, ref writeIndex, ref m_TextStyleStackDepth, ref m_TextStyleStacks, ref generationSettings);
					}
					break;
				case MarkupTag.STYLE:
				{
					if (flag)
					{
						break;
					}
					int k = writeIndex;
					if (!TextGeneratorUtilities.ReplaceOpeningStyleTag(ref m_TextBackingArray, i, out var srcOffset, ref m_TextProcessingArray, ref writeIndex, ref m_TextStyleStackDepth, ref m_TextStyleStacks, ref generationSettings))
					{
						break;
					}
					for (; k < writeIndex; k++)
					{
						m_TextProcessingArray[k].stringIndex = i;
						m_TextProcessingArray[k].length = srcOffset - i + 1;
					}
					i = srcOffset;
					continue;
				}
				case MarkupTag.SLASH_A:
					TextGeneratorUtilities.InsertClosingTextStyle(TextGeneratorUtilities.GetStyle(generationSettings, 65), ref m_TextProcessingArray, ref writeIndex, ref m_TextStyleStackDepth, ref m_TextStyleStacks, ref generationSettings);
					break;
				case MarkupTag.SLASH_STYLE:
				{
					if (flag)
					{
						break;
					}
					int j = writeIndex;
					TextGeneratorUtilities.ReplaceClosingStyleTag(ref m_TextProcessingArray, ref writeIndex, ref m_TextStyleStackDepth, ref m_TextStyleStacks, ref generationSettings);
					for (; j < writeIndex; j++)
					{
						m_TextProcessingArray[j].stringIndex = i;
						m_TextProcessingArray[j].length = 8;
					}
					i += 7;
					continue;
				}
				}
			}
			if (writeIndex == m_TextProcessingArray.Length)
			{
				TextGeneratorUtilities.ResizeInternalArray(ref m_TextProcessingArray);
			}
			m_TextProcessingArray[writeIndex] = new TextProcessingElement
			{
				elementType = TextProcessingElementType.TextCharacterElement,
				stringIndex = i,
				length = 1,
				unicode = num
			};
			writeIndex++;
		}
		m_TextStyleStackDepth = 0;
		if (style != null && style.hashCode != -1183493901)
		{
			TextGeneratorUtilities.InsertClosingStyleTag(ref m_TextProcessingArray, ref writeIndex, ref m_TextStyleStackDepth, ref m_TextStyleStacks, ref generationSettings);
		}
		if (writeIndex == m_TextProcessingArray.Length)
		{
			TextGeneratorUtilities.ResizeInternalArray(ref m_TextProcessingArray);
		}
		m_TextProcessingArray[writeIndex].unicode = 0u;
		m_InternalTextProcessingArraySize = writeIndex;
	}

	private bool PopulateFontAsset(TextGenerationSettings generationSettings, TextProcessingElement[] textProcessingArray)
	{
		bool flag = !IsExecutingJob;
		TextSettings textSettings = generationSettings.textSettings;
		int num = 0;
		m_TotalCharacterCount = 0;
		m_isTextLayoutPhase = false;
		m_TagNoParsing = false;
		m_FontStyleInternal = generationSettings.fontStyle;
		m_FontStyleStack.Clear();
		m_FontWeightInternal = (((m_FontStyleInternal & FontStyles.Bold) == FontStyles.Bold) ? TextFontWeight.Bold : generationSettings.fontWeight);
		m_FontWeightStack.SetDefault(m_FontWeightInternal);
		m_CurrentFontAsset = generationSettings.fontAsset;
		m_CurrentMaterial = generationSettings.fontAsset.material;
		m_CurrentMaterialIndex = 0;
		m_MaterialReferenceStack.SetDefault(new MaterialReference(m_CurrentMaterialIndex, m_CurrentFontAsset, null, m_CurrentMaterial, m_Padding));
		m_MaterialReferenceIndexLookup.Clear();
		MaterialReference.AddMaterialReference(m_CurrentMaterial, m_CurrentFontAsset, ref m_MaterialReferences, m_MaterialReferenceIndexLookup);
		m_TextElementType = TextElementType.Character;
		if (generationSettings.overflowMode == TextOverflowMode.Ellipsis)
		{
			GetEllipsisSpecialCharacter(generationSettings);
			if (m_Ellipsis.character != null && m_Ellipsis.fontAsset.GetHashCode() != m_CurrentFontAsset.GetHashCode())
			{
				if (textSettings.matchMaterialPreset && m_CurrentMaterial.GetHashCode() != m_Ellipsis.fontAsset.material.GetHashCode())
				{
					if (!flag)
					{
						return false;
					}
					m_Ellipsis.material = MaterialManager.GetFallbackMaterial(m_CurrentMaterial, m_Ellipsis.fontAsset.material);
				}
				else
				{
					m_Ellipsis.material = m_Ellipsis.fontAsset.material;
				}
				m_Ellipsis.materialIndex = MaterialReference.AddMaterialReference(m_Ellipsis.material, m_Ellipsis.fontAsset, ref m_MaterialReferences, m_MaterialReferenceIndexLookup);
				m_MaterialReferences[m_Ellipsis.materialIndex].referenceCount = 0;
			}
		}
		bool flag2 = TextGenerationSettings.fontFeatures.Contains(OTL_FeatureTag.liga);
		for (int i = 0; i < textProcessingArray.Length && textProcessingArray[i].unicode != 0; i++)
		{
			uint num2 = textProcessingArray[i].unicode;
			int currentMaterialIndex = m_CurrentMaterialIndex;
			if (generationSettings.richText && num2 == 60)
			{
				currentMaterialIndex = m_CurrentMaterialIndex;
				if (ValidateHtmlTag(textProcessingArray, i + 1, out var endIndex, generationSettings, null, out var isThreadSuccess))
				{
					int stringIndex = textProcessingArray[i].stringIndex;
					i = endIndex;
					if (m_TextElementType == TextElementType.Sprite)
					{
						m_TextElementType = TextElementType.Character;
						m_CurrentMaterialIndex = currentMaterialIndex;
						num++;
						m_TotalCharacterCount++;
					}
					continue;
				}
				if (!isThreadSuccess)
				{
					return false;
				}
			}
			bool flag3 = false;
			FontAsset currentFontAsset = m_CurrentFontAsset;
			Material currentMaterial = m_CurrentMaterial;
			currentMaterialIndex = m_CurrentMaterialIndex;
			if (m_TextElementType == TextElementType.Character)
			{
				if ((m_FontStyleInternal & FontStyles.UpperCase) == FontStyles.UpperCase)
				{
					if (char.IsLower((char)num2))
					{
						num2 = char.ToUpper((char)num2);
					}
				}
				else if ((m_FontStyleInternal & FontStyles.LowerCase) == FontStyles.LowerCase)
				{
					if (char.IsUpper((char)num2))
					{
						num2 = char.ToLower((char)num2);
					}
				}
				else if ((m_FontStyleInternal & FontStyles.SmallCaps) == FontStyles.SmallCaps && char.IsLower((char)num2))
				{
					num2 = char.ToUpper((char)num2);
				}
			}
			if (!flag && m_CurrentFontAsset.m_CharacterLookupDictionary == null)
			{
				return false;
			}
			TextElement textElement = null;
			uint num3 = ((i + 1 < textProcessingArray.Length) ? textProcessingArray[i + 1].unicode : 0u);
			bool isAlternativeTypeface;
			if (generationSettings.emojiFallbackSupport && ((TextGeneratorUtilities.IsEmojiPresentationForm(num2) && num3 != 65038) || (TextGeneratorUtilities.IsEmoji(num2) && num3 == 65039)) && textSettings.emojiFallbackTextAssets != null && textSettings.emojiFallbackTextAssets.Count > 0)
			{
				textElement = FontAssetUtilities.GetTextElementFromTextAssets(num2, m_CurrentFontAsset, textSettings.emojiFallbackTextAssets, includeFallbacks: true, m_FontStyleInternal, m_FontWeightInternal, out isAlternativeTypeface, flag2);
				if (textElement == null)
				{
				}
			}
			if (textElement == null)
			{
				textElement = GetTextElement(generationSettings, num2, m_CurrentFontAsset, m_FontStyleInternal, m_FontWeightInternal, out isAlternativeTypeface, flag2);
			}
			if (textElement == null)
			{
				if (!flag)
				{
					return false;
				}
				uint num4 = num2;
				num2 = (textProcessingArray[i].unicode = ((textSettings.missingCharacterUnicode == 0) ? 9633u : ((uint)textSettings.missingCharacterUnicode)));
				textElement = FontAssetUtilities.GetCharacterFromFontAsset(num2, m_CurrentFontAsset, includeFallbacks: true, m_FontStyleInternal, m_FontWeightInternal, out isAlternativeTypeface, flag2);
				if (textElement == null)
				{
					if (textSettings.GetFallbackFontAssets(m_CurrentFontAsset.IsRaster(), m_ShouldRenderBitmap ? generationSettings.fontSize : (-1)) == null && !flag)
					{
						return false;
					}
					textElement = FontAssetUtilities.GetCharacterFromFontAssetsInternal(num2, m_CurrentFontAsset, textSettings.GetFallbackFontAssets(m_CurrentFontAsset.IsRaster(), m_ShouldRenderBitmap ? generationSettings.fontSize : (-1)), textSettings.fallbackOSFontAssets, includeFallbacks: true, m_FontStyleInternal, m_FontWeightInternal, out isAlternativeTypeface, flag2);
				}
				if (textElement == null && textSettings.defaultFontAsset != null)
				{
					textElement = FontAssetUtilities.GetCharacterFromFontAsset(num2, textSettings.defaultFontAsset, includeFallbacks: true, m_FontStyleInternal, m_FontWeightInternal, out isAlternativeTypeface, flag2);
				}
				if (textElement == null)
				{
					num2 = (textProcessingArray[i].unicode = 32u);
					textElement = FontAssetUtilities.GetCharacterFromFontAsset(num2, m_CurrentFontAsset, includeFallbacks: true, m_FontStyleInternal, m_FontWeightInternal, out isAlternativeTypeface, flag2);
				}
				if (textElement == null)
				{
					num2 = (textProcessingArray[i].unicode = 3u);
					textElement = FontAssetUtilities.GetCharacterFromFontAsset(num2, m_CurrentFontAsset, includeFallbacks: true, m_FontStyleInternal, m_FontWeightInternal, out isAlternativeTypeface, flag2);
				}
				if (textSettings.displayWarnings)
				{
					string message = ((num4 > 65535) ? $"The character with Unicode value \\U{num4:X8} was not found in the [{generationSettings.fontAsset.name}] font asset or any potential fallbacks. It was replaced by Unicode character \\u{textElement.unicode:X4}." : $"The character with Unicode value \\u{num4:X4} was not found in the [{generationSettings.fontAsset.name}] font asset or any potential fallbacks. It was replaced by Unicode character \\u{textElement.unicode:X4}.");
					Debug.LogWarning(message);
				}
			}
			if (textElement.elementType == TextElementType.Character)
			{
				if (!(flag ? (textElement.textAsset.instanceID == m_CurrentFontAsset.instanceID) : (textElement.textAsset == m_CurrentFontAsset)))
				{
					flag3 = true;
					m_CurrentFontAsset = textElement.textAsset as FontAsset;
				}
				if ((num3 >= 65024 && num3 <= 65039) || (num3 >= 917760 && num3 <= 917999))
				{
					if (!m_CurrentFontAsset.TryGetGlyphVariantIndexInternal(num2, num3, out var variantGlyphIndex))
					{
						if (!flag)
						{
							return false;
						}
						variantGlyphIndex = m_CurrentFontAsset.GetGlyphVariantIndex(num2, num3);
						m_CurrentFontAsset.TryAddGlyphVariantIndexInternal(num2, num3, variantGlyphIndex);
					}
					if (variantGlyphIndex != 0)
					{
						m_CurrentFontAsset.TryAddGlyphInternal(variantGlyphIndex, out var _);
					}
					textProcessingArray[i + 1].unicode = 26u;
					i++;
				}
				if (flag2 && m_CurrentFontAsset.fontFeatureTable.m_LigatureSubstitutionRecordLookup.TryGetValue(textElement.glyphIndex, out var value))
				{
					if (value == null)
					{
						break;
					}
					for (int j = 0; j < value.Count; j++)
					{
						LigatureSubstitutionRecord ligatureSubstitutionRecord = value[j];
						int num5 = ligatureSubstitutionRecord.componentGlyphIDs.Length;
						uint num6 = ligatureSubstitutionRecord.ligatureGlyphID;
						for (int k = 1; k < num5; k++)
						{
							uint unicode = textProcessingArray[i + k].unicode;
							bool success;
							uint glyphIndex = m_CurrentFontAsset.GetGlyphIndex(unicode, out success);
							if (!success)
							{
								return false;
							}
							if (glyphIndex != ligatureSubstitutionRecord.componentGlyphIDs[k])
							{
								num6 = 0u;
								break;
							}
						}
						if (num6 == 0)
						{
							continue;
						}
						if (!flag)
						{
							return false;
						}
						if (!m_CurrentFontAsset.TryAddGlyphInternal(num6, out var _))
						{
							continue;
						}
						for (int l = 0; l < num5; l++)
						{
							if (l == 0)
							{
								textProcessingArray[i + l].length = num5;
							}
							else
							{
								textProcessingArray[i + l].unicode = 26u;
							}
						}
						i += num5 - 1;
						break;
					}
				}
			}
			if (textElement.elementType == TextElementType.Sprite)
			{
				SpriteAsset spriteAsset = textElement.textAsset as SpriteAsset;
				m_CurrentMaterialIndex = MaterialReference.AddMaterialReference(spriteAsset.material, spriteAsset, ref m_MaterialReferences, m_MaterialReferenceIndexLookup);
				m_TextElementType = TextElementType.Character;
				m_CurrentMaterialIndex = currentMaterialIndex;
				num++;
				m_TotalCharacterCount++;
				continue;
			}
			if (flag3 && m_CurrentFontAsset.instanceID != generationSettings.fontAsset.instanceID)
			{
				if (flag)
				{
					if (textSettings.matchMaterialPreset)
					{
						m_CurrentMaterial = MaterialManager.GetFallbackMaterial(m_CurrentMaterial, m_CurrentFontAsset.material);
					}
					else
					{
						m_CurrentMaterial = m_CurrentFontAsset.material;
					}
				}
				else
				{
					if (textSettings.matchMaterialPreset)
					{
						return false;
					}
					m_CurrentMaterial = m_CurrentFontAsset.material;
				}
				m_CurrentMaterialIndex = MaterialReference.AddMaterialReference(m_CurrentMaterial, m_CurrentFontAsset, ref m_MaterialReferences, m_MaterialReferenceIndexLookup);
			}
			if (textElement != null && textElement.glyph.atlasIndex > 0)
			{
				if (!flag)
				{
					return false;
				}
				m_CurrentMaterial = MaterialManager.GetFallbackMaterial(m_CurrentFontAsset, m_CurrentMaterial, textElement.glyph.atlasIndex);
				m_CurrentMaterialIndex = MaterialReference.AddMaterialReference(m_CurrentMaterial, m_CurrentFontAsset, ref m_MaterialReferences, m_MaterialReferenceIndexLookup);
				flag3 = true;
			}
			if (!char.IsWhiteSpace((char)num2) && num2 != 8203)
			{
				if (generationSettings.isIMGUI && m_MaterialReferences[m_CurrentMaterialIndex].referenceCount >= 16383)
				{
					m_CurrentMaterialIndex = MaterialReference.AddMaterialReference(new Material(m_CurrentMaterial), m_CurrentFontAsset, ref m_MaterialReferences, m_MaterialReferenceIndexLookup);
				}
				m_MaterialReferences[m_CurrentMaterialIndex].referenceCount++;
			}
			m_MaterialReferences[m_CurrentMaterialIndex].isFallbackMaterial = flag3;
			if (flag3)
			{
				m_MaterialReferences[m_CurrentMaterialIndex].fallbackMaterial = currentMaterial;
				m_CurrentFontAsset = currentFontAsset;
				m_CurrentMaterial = currentMaterial;
				m_CurrentMaterialIndex = currentMaterialIndex;
			}
			m_TotalCharacterCount++;
		}
		return true;
	}

	private void ComputeMarginSize(Rect rect, Vector4 margins)
	{
		m_MarginWidth = rect.width - margins.x - margins.z;
		m_MarginHeight = rect.height - margins.y - margins.w;
		m_RectTransformCorners[0].x = 0f;
		m_RectTransformCorners[0].y = 0f;
		m_RectTransformCorners[1].x = 0f;
		m_RectTransformCorners[1].y = rect.height;
		m_RectTransformCorners[2].x = rect.width;
		m_RectTransformCorners[2].y = rect.height;
		m_RectTransformCorners[3].x = rect.width;
		m_RectTransformCorners[3].y = 0f;
	}

	protected bool GetSpecialCharacters(TextGenerationSettings generationSettings)
	{
		if (!GetEllipsisSpecialCharacter(generationSettings))
		{
			return false;
		}
		if (!GetUnderlineSpecialCharacter(generationSettings) || m_Underline.character == null)
		{
			return false;
		}
		return true;
	}

	protected bool GetEllipsisSpecialCharacter(TextGenerationSettings generationSettings)
	{
		bool flag = !IsExecutingJob;
		FontAsset fontAsset = m_CurrentFontAsset ?? generationSettings.fontAsset;
		TextSettings textSettings = generationSettings.textSettings;
		bool populateLigatures = TextGenerationSettings.fontFeatures.Contains(OTL_FeatureTag.liga);
		bool isAlternativeTypeface;
		Character character = FontAssetUtilities.GetCharacterFromFontAsset(8230u, fontAsset, includeFallbacks: false, m_FontStyleInternal, m_FontWeightInternal, out isAlternativeTypeface, populateLigatures);
		if (character == null && fontAsset.m_FallbackFontAssetTable != null && fontAsset.m_FallbackFontAssetTable.Count > 0)
		{
			character = FontAssetUtilities.GetCharacterFromFontAssetsInternal(8230u, fontAsset, fontAsset.m_FallbackFontAssetTable, null, includeFallbacks: true, m_FontStyleInternal, m_FontWeightInternal, out isAlternativeTypeface, populateLigatures);
		}
		if (character == null)
		{
			if (textSettings.GetStaticFallbackOSFontAsset() == null && !flag)
			{
				return false;
			}
			character = FontAssetUtilities.GetCharacterFromFontAssetsInternal(8230u, fontAsset, textSettings.GetFallbackFontAssets(fontAsset.IsRaster(), m_ShouldRenderBitmap ? generationSettings.fontSize : (-1)), textSettings.fallbackOSFontAssets, includeFallbacks: true, m_FontStyleInternal, m_FontWeightInternal, out isAlternativeTypeface, populateLigatures);
		}
		if (character == null && textSettings.defaultFontAsset != null)
		{
			character = FontAssetUtilities.GetCharacterFromFontAsset(8230u, textSettings.defaultFontAsset, includeFallbacks: true, m_FontStyleInternal, m_FontWeightInternal, out isAlternativeTypeface, populateLigatures);
		}
		if (character != null)
		{
			m_Ellipsis = new SpecialCharacter(character, 0);
		}
		return true;
	}

	protected bool GetUnderlineSpecialCharacter(TextGenerationSettings generationSettings)
	{
		bool flag = !IsExecutingJob;
		FontAsset fontAsset = m_CurrentFontAsset ?? generationSettings.fontAsset;
		TextSettings textSettings = generationSettings.textSettings;
		bool populateLigatures = TextGenerationSettings.fontFeatures.Contains(OTL_FeatureTag.liga);
		bool isAlternativeTypeface;
		Character character = FontAssetUtilities.GetCharacterFromFontAsset(95u, fontAsset, includeFallbacks: false, m_FontStyleInternal, m_FontWeightInternal, out isAlternativeTypeface, populateLigatures);
		if (character == null && fontAsset.m_FallbackFontAssetTable != null && fontAsset.m_FallbackFontAssetTable.Count > 0)
		{
			character = FontAssetUtilities.GetCharacterFromFontAssetsInternal(95u, fontAsset, fontAsset.m_FallbackFontAssetTable, null, includeFallbacks: true, m_FontStyleInternal, m_FontWeightInternal, out isAlternativeTypeface, populateLigatures);
		}
		if (character == null)
		{
			if (textSettings.GetStaticFallbackOSFontAsset() == null && !flag)
			{
				return false;
			}
			character = FontAssetUtilities.GetCharacterFromFontAssetsInternal(95u, fontAsset, textSettings.GetFallbackFontAssets(fontAsset.IsRaster(), m_ShouldRenderBitmap ? generationSettings.fontSize : (-1)), textSettings.fallbackOSFontAssets, includeFallbacks: true, m_FontStyleInternal, m_FontWeightInternal, out isAlternativeTypeface, populateLigatures);
		}
		if (character == null && textSettings.defaultFontAsset != null)
		{
			character = FontAssetUtilities.GetCharacterFromFontAsset(95u, textSettings.defaultFontAsset, includeFallbacks: true, m_FontStyleInternal, m_FontWeightInternal, out isAlternativeTypeface, populateLigatures);
		}
		if (character != null)
		{
			m_Underline = new SpecialCharacter(character, m_CurrentMaterialIndex);
			if (m_Underline.fontAsset.GetHashCode() != m_CurrentFontAsset.GetHashCode())
			{
				if (generationSettings.textSettings.matchMaterialPreset && m_CurrentMaterial != null && m_CurrentMaterial.GetHashCode() != m_Underline.fontAsset.material.GetHashCode())
				{
					m_Underline.material = MaterialManager.GetFallbackMaterial(m_CurrentMaterial, m_Underline.fontAsset.material);
					if (m_Underline.material == null)
					{
						return false;
					}
				}
				else
				{
					m_Underline.material = m_Underline.fontAsset.material;
				}
				m_Underline.materialIndex = MaterialReference.AddMaterialReference(m_Underline.material, m_Underline.fontAsset, ref m_MaterialReferences, m_MaterialReferenceIndexLookup);
				m_MaterialReferences[m_Underline.materialIndex].referenceCount = 0;
			}
		}
		return true;
	}

	protected void DoMissingGlyphCallback(uint unicode, int stringIndex, FontAsset fontAsset, TextInfo textInfo)
	{
		TextGenerator.OnMissingCharacter?.Invoke(unicode, stringIndex, textInfo, fontAsset);
	}
}
