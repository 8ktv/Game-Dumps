using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Unity.Properties;
using UnityEngine.Bindings;
using UnityEngine.Pool;
using UnityEngine.UIElements.StyleSheets;

namespace UnityEngine.UIElements;

[VisibleToOtherModules(new string[] { "UnityEditor.UIBuilderModule" })]
internal sealed class StyleDiff : INotifyBindablePropertyChanged, IDataSourceViewHashProvider, IDisposable
{
	internal readonly struct ResolutionContext
	{
		public readonly StyleDiff diff;

		public readonly StyleSheet styleSheet;

		public readonly Dictionary<string, UxmlData> uxmlData;

		public readonly HashSet<string> uxmlOverrides;

		public ResolutionContext(StyleDiff diff, StyleSheet inline, Dictionary<string, UxmlData> uxmlData, HashSet<string> uxmlOverrides)
		{
			this.diff = diff;
			styleSheet = inline;
			this.uxmlData = uxmlData;
			this.uxmlOverrides = uxmlOverrides;
		}

		public void MarkAsOverride(string name)
		{
			if (uxmlOverrides.Add(name))
			{
				diff.Notify(name);
			}
		}

		public void ClearOverride(string name)
		{
			if (uxmlOverrides.Remove(name))
			{
				diff.Notify(name);
			}
		}
	}

	private StylePropertyData<StyleEnum<Align>, Align> m_AlignContent;

	private StylePropertyData<StyleEnum<Align>, Align> m_AlignItems;

	private StylePropertyData<StyleEnum<Align>, Align> m_AlignSelf;

	private StylePropertyData<StyleColor, Color> m_BackgroundColor;

	private StylePropertyData<StyleBackground, Background> m_BackgroundImage;

	private StylePropertyData<StyleBackgroundPosition, BackgroundPosition> m_BackgroundPositionX;

	private StylePropertyData<StyleBackgroundPosition, BackgroundPosition> m_BackgroundPositionY;

	private StylePropertyData<StyleBackgroundRepeat, BackgroundRepeat> m_BackgroundRepeat;

	private StylePropertyData<StyleBackgroundSize, BackgroundSize> m_BackgroundSize;

	private StylePropertyData<StyleColor, Color> m_BorderBottomColor;

	private StylePropertyData<StyleLength, Length> m_BorderBottomLeftRadius;

	private StylePropertyData<StyleLength, Length> m_BorderBottomRightRadius;

	private StylePropertyData<StyleFloat, float> m_BorderBottomWidth;

	private StylePropertyData<StyleColor, Color> m_BorderLeftColor;

	private StylePropertyData<StyleFloat, float> m_BorderLeftWidth;

	private StylePropertyData<StyleColor, Color> m_BorderRightColor;

	private StylePropertyData<StyleFloat, float> m_BorderRightWidth;

	private StylePropertyData<StyleColor, Color> m_BorderTopColor;

	private StylePropertyData<StyleLength, Length> m_BorderTopLeftRadius;

	private StylePropertyData<StyleLength, Length> m_BorderTopRightRadius;

	private StylePropertyData<StyleFloat, float> m_BorderTopWidth;

	private StylePropertyData<StyleLength, Length> m_Bottom;

	private StylePropertyData<StyleColor, Color> m_Color;

	private StylePropertyData<StyleCursor, Cursor> m_Cursor;

	private StylePropertyData<StyleEnum<DisplayStyle>, DisplayStyle> m_Display;

	private StylePropertyData<StyleLength, Length> m_FlexBasis;

	private StylePropertyData<StyleEnum<FlexDirection>, FlexDirection> m_FlexDirection;

	private StylePropertyData<StyleFloat, float> m_FlexGrow;

	private StylePropertyData<StyleFloat, float> m_FlexShrink;

	private StylePropertyData<StyleEnum<Wrap>, Wrap> m_FlexWrap;

	private StylePropertyData<StyleLength, Length> m_FontSize;

	private StylePropertyData<StyleLength, Length> m_Height;

	private StylePropertyData<StyleEnum<Justify>, Justify> m_JustifyContent;

	private StylePropertyData<StyleLength, Length> m_Left;

	private StylePropertyData<StyleLength, Length> m_LetterSpacing;

	private StylePropertyData<StyleLength, Length> m_MarginBottom;

	private StylePropertyData<StyleLength, Length> m_MarginLeft;

	private StylePropertyData<StyleLength, Length> m_MarginRight;

	private StylePropertyData<StyleLength, Length> m_MarginTop;

	private StylePropertyData<StyleLength, Length> m_MaxHeight;

	private StylePropertyData<StyleLength, Length> m_MaxWidth;

	private StylePropertyData<StyleLength, Length> m_MinHeight;

	private StylePropertyData<StyleLength, Length> m_MinWidth;

	private StylePropertyData<StyleFloat, float> m_Opacity;

	private StylePropertyData<StyleEnum<Overflow>, OverflowInternal> m_Overflow;

	private StylePropertyData<StyleLength, Length> m_PaddingBottom;

	private StylePropertyData<StyleLength, Length> m_PaddingLeft;

	private StylePropertyData<StyleLength, Length> m_PaddingRight;

	private StylePropertyData<StyleLength, Length> m_PaddingTop;

	private StylePropertyData<StyleEnum<Position>, Position> m_Position;

	private StylePropertyData<StyleLength, Length> m_Right;

	private StylePropertyData<StyleRotate, Rotate> m_Rotate;

	private StylePropertyData<StyleScale, Scale> m_Scale;

	private StylePropertyData<StyleEnum<TextOverflow>, TextOverflow> m_TextOverflow;

	private StylePropertyData<StyleTextShadow, TextShadow> m_TextShadow;

	private StylePropertyData<StyleLength, Length> m_Top;

	private StylePropertyData<StyleTransformOrigin, TransformOrigin> m_TransformOrigin;

	private StylePropertyData<StyleList<TimeValue>, List<TimeValue>> m_TransitionDelay;

	private StylePropertyData<StyleList<TimeValue>, List<TimeValue>> m_TransitionDuration;

	private StylePropertyData<StyleList<StylePropertyName>, List<StylePropertyName>> m_TransitionProperty;

	private StylePropertyData<StyleList<EasingFunction>, List<EasingFunction>> m_TransitionTimingFunction;

	private StylePropertyData<StyleTranslate, Translate> m_Translate;

	private StylePropertyData<StyleColor, Color> m_UnityBackgroundImageTintColor;

	private StylePropertyData<StyleEnum<EditorTextRenderingMode>, EditorTextRenderingMode> m_UnityEditorTextRenderingMode;

	private StylePropertyData<StyleFont, Font> m_UnityFont;

	private StylePropertyData<StyleFontDefinition, FontDefinition> m_UnityFontDefinition;

	private StylePropertyData<StyleEnum<FontStyle>, FontStyle> m_UnityFontStyleAndWeight;

	private StylePropertyData<StyleEnum<OverflowClipBox>, OverflowClipBox> m_UnityOverflowClipBox;

	private StylePropertyData<StyleLength, Length> m_UnityParagraphSpacing;

	private StylePropertyData<StyleInt, int> m_UnitySliceBottom;

	private StylePropertyData<StyleInt, int> m_UnitySliceLeft;

	private StylePropertyData<StyleInt, int> m_UnitySliceRight;

	private StylePropertyData<StyleFloat, float> m_UnitySliceScale;

	private StylePropertyData<StyleInt, int> m_UnitySliceTop;

	private StylePropertyData<StyleEnum<SliceType>, SliceType> m_UnitySliceType;

	private StylePropertyData<StyleEnum<TextAnchor>, TextAnchor> m_UnityTextAlign;

	private StylePropertyData<StyleTextAutoSize, TextAutoSize> m_UnityTextAutoSize;

	private StylePropertyData<StyleEnum<TextGeneratorType>, TextGeneratorType> m_UnityTextGenerator;

	private StylePropertyData<StyleColor, Color> m_UnityTextOutlineColor;

	private StylePropertyData<StyleFloat, float> m_UnityTextOutlineWidth;

	private StylePropertyData<StyleEnum<TextOverflowPosition>, TextOverflowPosition> m_UnityTextOverflowPosition;

	private StylePropertyData<StyleEnum<Visibility>, Visibility> m_Visibility;

	private StylePropertyData<StyleEnum<WhiteSpace>, WhiteSpace> m_WhiteSpace;

	private StylePropertyData<StyleLength, Length> m_Width;

	private StylePropertyData<StyleLength, Length> m_WordSpacing;

	private long m_Version;

	[CreateProperty]
	private readonly HashSet<string> uxmlOverrides = new HashSet<string>();

	private MatchedRulesExtractor m_MatchedRules;

	[CreateProperty]
	public StylePropertyData<StyleEnum<Align>, Align> alignContent
	{
		get
		{
			return m_AlignContent;
		}
		private set
		{
			m_AlignContent.target = value.target;
			if (m_AlignContent == value)
			{
				value.Dispose();
				return;
			}
			m_AlignContent.Dispose();
			m_AlignContent = value;
			Notify("alignContent");
		}
	}

	[CreateProperty]
	public StylePropertyData<StyleEnum<Align>, Align> alignItems
	{
		get
		{
			return m_AlignItems;
		}
		private set
		{
			m_AlignItems.target = value.target;
			if (m_AlignItems == value)
			{
				value.Dispose();
				return;
			}
			m_AlignItems.Dispose();
			m_AlignItems = value;
			Notify("alignItems");
		}
	}

	[CreateProperty]
	public StylePropertyData<StyleEnum<Align>, Align> alignSelf
	{
		get
		{
			return m_AlignSelf;
		}
		private set
		{
			m_AlignSelf.target = value.target;
			if (m_AlignSelf == value)
			{
				value.Dispose();
				return;
			}
			m_AlignSelf.Dispose();
			m_AlignSelf = value;
			Notify("alignSelf");
		}
	}

	[CreateProperty]
	public StylePropertyData<StyleColor, Color> backgroundColor
	{
		get
		{
			return m_BackgroundColor;
		}
		private set
		{
			m_BackgroundColor.target = value.target;
			if (m_BackgroundColor == value)
			{
				value.Dispose();
				return;
			}
			m_BackgroundColor.Dispose();
			m_BackgroundColor = value;
			Notify("backgroundColor");
		}
	}

	[CreateProperty]
	public StylePropertyData<StyleBackground, Background> backgroundImage
	{
		get
		{
			return m_BackgroundImage;
		}
		private set
		{
			m_BackgroundImage.target = value.target;
			if (m_BackgroundImage == value)
			{
				value.Dispose();
				return;
			}
			m_BackgroundImage.Dispose();
			m_BackgroundImage = value;
			Notify("backgroundImage");
		}
	}

	[CreateProperty]
	public StylePropertyData<StyleBackgroundPosition, BackgroundPosition> backgroundPositionX
	{
		get
		{
			return m_BackgroundPositionX;
		}
		private set
		{
			m_BackgroundPositionX.target = value.target;
			if (m_BackgroundPositionX == value)
			{
				value.Dispose();
				return;
			}
			m_BackgroundPositionX.Dispose();
			m_BackgroundPositionX = value;
			Notify("backgroundPositionX");
		}
	}

	[CreateProperty]
	public StylePropertyData<StyleBackgroundPosition, BackgroundPosition> backgroundPositionY
	{
		get
		{
			return m_BackgroundPositionY;
		}
		private set
		{
			m_BackgroundPositionY.target = value.target;
			if (m_BackgroundPositionY == value)
			{
				value.Dispose();
				return;
			}
			m_BackgroundPositionY.Dispose();
			m_BackgroundPositionY = value;
			Notify("backgroundPositionY");
		}
	}

	[CreateProperty]
	public StylePropertyData<StyleBackgroundRepeat, BackgroundRepeat> backgroundRepeat
	{
		get
		{
			return m_BackgroundRepeat;
		}
		private set
		{
			m_BackgroundRepeat.target = value.target;
			if (m_BackgroundRepeat == value)
			{
				value.Dispose();
				return;
			}
			m_BackgroundRepeat.Dispose();
			m_BackgroundRepeat = value;
			Notify("backgroundRepeat");
		}
	}

	[CreateProperty]
	public StylePropertyData<StyleBackgroundSize, BackgroundSize> backgroundSize
	{
		get
		{
			return m_BackgroundSize;
		}
		private set
		{
			m_BackgroundSize.target = value.target;
			if (m_BackgroundSize == value)
			{
				value.Dispose();
				return;
			}
			m_BackgroundSize.Dispose();
			m_BackgroundSize = value;
			Notify("backgroundSize");
		}
	}

	[CreateProperty]
	public StylePropertyData<StyleColor, Color> borderBottomColor
	{
		get
		{
			return m_BorderBottomColor;
		}
		private set
		{
			m_BorderBottomColor.target = value.target;
			if (m_BorderBottomColor == value)
			{
				value.Dispose();
				return;
			}
			m_BorderBottomColor.Dispose();
			m_BorderBottomColor = value;
			Notify("borderBottomColor");
		}
	}

	[CreateProperty]
	public StylePropertyData<StyleLength, Length> borderBottomLeftRadius
	{
		get
		{
			return m_BorderBottomLeftRadius;
		}
		private set
		{
			m_BorderBottomLeftRadius.target = value.target;
			if (m_BorderBottomLeftRadius == value)
			{
				value.Dispose();
				return;
			}
			m_BorderBottomLeftRadius.Dispose();
			m_BorderBottomLeftRadius = value;
			Notify("borderBottomLeftRadius");
		}
	}

	[CreateProperty]
	public StylePropertyData<StyleLength, Length> borderBottomRightRadius
	{
		get
		{
			return m_BorderBottomRightRadius;
		}
		private set
		{
			m_BorderBottomRightRadius.target = value.target;
			if (m_BorderBottomRightRadius == value)
			{
				value.Dispose();
				return;
			}
			m_BorderBottomRightRadius.Dispose();
			m_BorderBottomRightRadius = value;
			Notify("borderBottomRightRadius");
		}
	}

	[CreateProperty]
	public StylePropertyData<StyleFloat, float> borderBottomWidth
	{
		get
		{
			return m_BorderBottomWidth;
		}
		private set
		{
			m_BorderBottomWidth.target = value.target;
			if (m_BorderBottomWidth == value)
			{
				value.Dispose();
				return;
			}
			m_BorderBottomWidth.Dispose();
			m_BorderBottomWidth = value;
			Notify("borderBottomWidth");
		}
	}

	[CreateProperty]
	public StylePropertyData<StyleColor, Color> borderLeftColor
	{
		get
		{
			return m_BorderLeftColor;
		}
		private set
		{
			m_BorderLeftColor.target = value.target;
			if (m_BorderLeftColor == value)
			{
				value.Dispose();
				return;
			}
			m_BorderLeftColor.Dispose();
			m_BorderLeftColor = value;
			Notify("borderLeftColor");
		}
	}

	[CreateProperty]
	public StylePropertyData<StyleFloat, float> borderLeftWidth
	{
		get
		{
			return m_BorderLeftWidth;
		}
		private set
		{
			m_BorderLeftWidth.target = value.target;
			if (m_BorderLeftWidth == value)
			{
				value.Dispose();
				return;
			}
			m_BorderLeftWidth.Dispose();
			m_BorderLeftWidth = value;
			Notify("borderLeftWidth");
		}
	}

	[CreateProperty]
	public StylePropertyData<StyleColor, Color> borderRightColor
	{
		get
		{
			return m_BorderRightColor;
		}
		private set
		{
			m_BorderRightColor.target = value.target;
			if (m_BorderRightColor == value)
			{
				value.Dispose();
				return;
			}
			m_BorderRightColor.Dispose();
			m_BorderRightColor = value;
			Notify("borderRightColor");
		}
	}

	[CreateProperty]
	public StylePropertyData<StyleFloat, float> borderRightWidth
	{
		get
		{
			return m_BorderRightWidth;
		}
		private set
		{
			m_BorderRightWidth.target = value.target;
			if (m_BorderRightWidth == value)
			{
				value.Dispose();
				return;
			}
			m_BorderRightWidth.Dispose();
			m_BorderRightWidth = value;
			Notify("borderRightWidth");
		}
	}

	[CreateProperty]
	public StylePropertyData<StyleColor, Color> borderTopColor
	{
		get
		{
			return m_BorderTopColor;
		}
		private set
		{
			m_BorderTopColor.target = value.target;
			if (m_BorderTopColor == value)
			{
				value.Dispose();
				return;
			}
			m_BorderTopColor.Dispose();
			m_BorderTopColor = value;
			Notify("borderTopColor");
		}
	}

	[CreateProperty]
	public StylePropertyData<StyleLength, Length> borderTopLeftRadius
	{
		get
		{
			return m_BorderTopLeftRadius;
		}
		private set
		{
			m_BorderTopLeftRadius.target = value.target;
			if (m_BorderTopLeftRadius == value)
			{
				value.Dispose();
				return;
			}
			m_BorderTopLeftRadius.Dispose();
			m_BorderTopLeftRadius = value;
			Notify("borderTopLeftRadius");
		}
	}

	[CreateProperty]
	public StylePropertyData<StyleLength, Length> borderTopRightRadius
	{
		get
		{
			return m_BorderTopRightRadius;
		}
		private set
		{
			m_BorderTopRightRadius.target = value.target;
			if (m_BorderTopRightRadius == value)
			{
				value.Dispose();
				return;
			}
			m_BorderTopRightRadius.Dispose();
			m_BorderTopRightRadius = value;
			Notify("borderTopRightRadius");
		}
	}

	[CreateProperty]
	public StylePropertyData<StyleFloat, float> borderTopWidth
	{
		get
		{
			return m_BorderTopWidth;
		}
		private set
		{
			m_BorderTopWidth.target = value.target;
			if (m_BorderTopWidth == value)
			{
				value.Dispose();
				return;
			}
			m_BorderTopWidth.Dispose();
			m_BorderTopWidth = value;
			Notify("borderTopWidth");
		}
	}

	[CreateProperty]
	public StylePropertyData<StyleLength, Length> bottom
	{
		get
		{
			return m_Bottom;
		}
		private set
		{
			m_Bottom.target = value.target;
			if (m_Bottom == value)
			{
				value.Dispose();
				return;
			}
			m_Bottom.Dispose();
			m_Bottom = value;
			Notify("bottom");
		}
	}

	[CreateProperty]
	public StylePropertyData<StyleColor, Color> color
	{
		get
		{
			return m_Color;
		}
		private set
		{
			m_Color.target = value.target;
			if (m_Color == value)
			{
				value.Dispose();
				return;
			}
			m_Color.Dispose();
			m_Color = value;
			Notify("color");
		}
	}

	[CreateProperty]
	public StylePropertyData<StyleCursor, Cursor> cursor
	{
		get
		{
			return m_Cursor;
		}
		private set
		{
			m_Cursor.target = value.target;
			if (m_Cursor == value)
			{
				value.Dispose();
				return;
			}
			m_Cursor.Dispose();
			m_Cursor = value;
			Notify("cursor");
		}
	}

	[CreateProperty]
	public StylePropertyData<StyleEnum<DisplayStyle>, DisplayStyle> display
	{
		get
		{
			return m_Display;
		}
		private set
		{
			m_Display.target = value.target;
			if (m_Display == value)
			{
				value.Dispose();
				return;
			}
			m_Display.Dispose();
			m_Display = value;
			Notify("display");
		}
	}

	[CreateProperty]
	public StylePropertyData<StyleLength, Length> flexBasis
	{
		get
		{
			return m_FlexBasis;
		}
		private set
		{
			m_FlexBasis.target = value.target;
			if (m_FlexBasis == value)
			{
				value.Dispose();
				return;
			}
			m_FlexBasis.Dispose();
			m_FlexBasis = value;
			Notify("flexBasis");
		}
	}

	[CreateProperty]
	public StylePropertyData<StyleEnum<FlexDirection>, FlexDirection> flexDirection
	{
		get
		{
			return m_FlexDirection;
		}
		private set
		{
			m_FlexDirection.target = value.target;
			if (m_FlexDirection == value)
			{
				value.Dispose();
				return;
			}
			m_FlexDirection.Dispose();
			m_FlexDirection = value;
			Notify("flexDirection");
		}
	}

	[CreateProperty]
	public StylePropertyData<StyleFloat, float> flexGrow
	{
		get
		{
			return m_FlexGrow;
		}
		private set
		{
			m_FlexGrow.target = value.target;
			if (m_FlexGrow == value)
			{
				value.Dispose();
				return;
			}
			m_FlexGrow.Dispose();
			m_FlexGrow = value;
			Notify("flexGrow");
		}
	}

	[CreateProperty]
	public StylePropertyData<StyleFloat, float> flexShrink
	{
		get
		{
			return m_FlexShrink;
		}
		private set
		{
			m_FlexShrink.target = value.target;
			if (m_FlexShrink == value)
			{
				value.Dispose();
				return;
			}
			m_FlexShrink.Dispose();
			m_FlexShrink = value;
			Notify("flexShrink");
		}
	}

	[CreateProperty]
	public StylePropertyData<StyleEnum<Wrap>, Wrap> flexWrap
	{
		get
		{
			return m_FlexWrap;
		}
		private set
		{
			m_FlexWrap.target = value.target;
			if (m_FlexWrap == value)
			{
				value.Dispose();
				return;
			}
			m_FlexWrap.Dispose();
			m_FlexWrap = value;
			Notify("flexWrap");
		}
	}

	[CreateProperty]
	public StylePropertyData<StyleLength, Length> fontSize
	{
		get
		{
			return m_FontSize;
		}
		private set
		{
			m_FontSize.target = value.target;
			if (m_FontSize == value)
			{
				value.Dispose();
				return;
			}
			m_FontSize.Dispose();
			m_FontSize = value;
			Notify("fontSize");
		}
	}

	[CreateProperty]
	public StylePropertyData<StyleLength, Length> height
	{
		get
		{
			return m_Height;
		}
		private set
		{
			m_Height.target = value.target;
			if (m_Height == value)
			{
				value.Dispose();
				return;
			}
			m_Height.Dispose();
			m_Height = value;
			Notify("height");
		}
	}

	[CreateProperty]
	public StylePropertyData<StyleEnum<Justify>, Justify> justifyContent
	{
		get
		{
			return m_JustifyContent;
		}
		private set
		{
			m_JustifyContent.target = value.target;
			if (m_JustifyContent == value)
			{
				value.Dispose();
				return;
			}
			m_JustifyContent.Dispose();
			m_JustifyContent = value;
			Notify("justifyContent");
		}
	}

	[CreateProperty]
	public StylePropertyData<StyleLength, Length> left
	{
		get
		{
			return m_Left;
		}
		private set
		{
			m_Left.target = value.target;
			if (m_Left == value)
			{
				value.Dispose();
				return;
			}
			m_Left.Dispose();
			m_Left = value;
			Notify("left");
		}
	}

	[CreateProperty]
	public StylePropertyData<StyleLength, Length> letterSpacing
	{
		get
		{
			return m_LetterSpacing;
		}
		private set
		{
			m_LetterSpacing.target = value.target;
			if (m_LetterSpacing == value)
			{
				value.Dispose();
				return;
			}
			m_LetterSpacing.Dispose();
			m_LetterSpacing = value;
			Notify("letterSpacing");
		}
	}

	[CreateProperty]
	public StylePropertyData<StyleLength, Length> marginBottom
	{
		get
		{
			return m_MarginBottom;
		}
		private set
		{
			m_MarginBottom.target = value.target;
			if (m_MarginBottom == value)
			{
				value.Dispose();
				return;
			}
			m_MarginBottom.Dispose();
			m_MarginBottom = value;
			Notify("marginBottom");
		}
	}

	[CreateProperty]
	public StylePropertyData<StyleLength, Length> marginLeft
	{
		get
		{
			return m_MarginLeft;
		}
		private set
		{
			m_MarginLeft.target = value.target;
			if (m_MarginLeft == value)
			{
				value.Dispose();
				return;
			}
			m_MarginLeft.Dispose();
			m_MarginLeft = value;
			Notify("marginLeft");
		}
	}

	[CreateProperty]
	public StylePropertyData<StyleLength, Length> marginRight
	{
		get
		{
			return m_MarginRight;
		}
		private set
		{
			m_MarginRight.target = value.target;
			if (m_MarginRight == value)
			{
				value.Dispose();
				return;
			}
			m_MarginRight.Dispose();
			m_MarginRight = value;
			Notify("marginRight");
		}
	}

	[CreateProperty]
	public StylePropertyData<StyleLength, Length> marginTop
	{
		get
		{
			return m_MarginTop;
		}
		private set
		{
			m_MarginTop.target = value.target;
			if (m_MarginTop == value)
			{
				value.Dispose();
				return;
			}
			m_MarginTop.Dispose();
			m_MarginTop = value;
			Notify("marginTop");
		}
	}

	[CreateProperty]
	public StylePropertyData<StyleLength, Length> maxHeight
	{
		get
		{
			return m_MaxHeight;
		}
		private set
		{
			m_MaxHeight.target = value.target;
			if (m_MaxHeight == value)
			{
				value.Dispose();
				return;
			}
			m_MaxHeight.Dispose();
			m_MaxHeight = value;
			Notify("maxHeight");
		}
	}

	[CreateProperty]
	public StylePropertyData<StyleLength, Length> maxWidth
	{
		get
		{
			return m_MaxWidth;
		}
		private set
		{
			m_MaxWidth.target = value.target;
			if (m_MaxWidth == value)
			{
				value.Dispose();
				return;
			}
			m_MaxWidth.Dispose();
			m_MaxWidth = value;
			Notify("maxWidth");
		}
	}

	[CreateProperty]
	public StylePropertyData<StyleLength, Length> minHeight
	{
		get
		{
			return m_MinHeight;
		}
		private set
		{
			m_MinHeight.target = value.target;
			if (m_MinHeight == value)
			{
				value.Dispose();
				return;
			}
			m_MinHeight.Dispose();
			m_MinHeight = value;
			Notify("minHeight");
		}
	}

	[CreateProperty]
	public StylePropertyData<StyleLength, Length> minWidth
	{
		get
		{
			return m_MinWidth;
		}
		private set
		{
			m_MinWidth.target = value.target;
			if (m_MinWidth == value)
			{
				value.Dispose();
				return;
			}
			m_MinWidth.Dispose();
			m_MinWidth = value;
			Notify("minWidth");
		}
	}

	[CreateProperty]
	public StylePropertyData<StyleFloat, float> opacity
	{
		get
		{
			return m_Opacity;
		}
		private set
		{
			m_Opacity.target = value.target;
			if (m_Opacity == value)
			{
				value.Dispose();
				return;
			}
			m_Opacity.Dispose();
			m_Opacity = value;
			Notify("opacity");
		}
	}

	[CreateProperty]
	public StylePropertyData<StyleEnum<Overflow>, OverflowInternal> overflow
	{
		get
		{
			return m_Overflow;
		}
		private set
		{
			m_Overflow.target = value.target;
			if (m_Overflow == value)
			{
				value.Dispose();
				return;
			}
			m_Overflow.Dispose();
			m_Overflow = value;
			Notify("overflow");
		}
	}

	[CreateProperty]
	public StylePropertyData<StyleLength, Length> paddingBottom
	{
		get
		{
			return m_PaddingBottom;
		}
		private set
		{
			m_PaddingBottom.target = value.target;
			if (m_PaddingBottom == value)
			{
				value.Dispose();
				return;
			}
			m_PaddingBottom.Dispose();
			m_PaddingBottom = value;
			Notify("paddingBottom");
		}
	}

	[CreateProperty]
	public StylePropertyData<StyleLength, Length> paddingLeft
	{
		get
		{
			return m_PaddingLeft;
		}
		private set
		{
			m_PaddingLeft.target = value.target;
			if (m_PaddingLeft == value)
			{
				value.Dispose();
				return;
			}
			m_PaddingLeft.Dispose();
			m_PaddingLeft = value;
			Notify("paddingLeft");
		}
	}

	[CreateProperty]
	public StylePropertyData<StyleLength, Length> paddingRight
	{
		get
		{
			return m_PaddingRight;
		}
		private set
		{
			m_PaddingRight.target = value.target;
			if (m_PaddingRight == value)
			{
				value.Dispose();
				return;
			}
			m_PaddingRight.Dispose();
			m_PaddingRight = value;
			Notify("paddingRight");
		}
	}

	[CreateProperty]
	public StylePropertyData<StyleLength, Length> paddingTop
	{
		get
		{
			return m_PaddingTop;
		}
		private set
		{
			m_PaddingTop.target = value.target;
			if (m_PaddingTop == value)
			{
				value.Dispose();
				return;
			}
			m_PaddingTop.Dispose();
			m_PaddingTop = value;
			Notify("paddingTop");
		}
	}

	[CreateProperty]
	public StylePropertyData<StyleEnum<Position>, Position> position
	{
		get
		{
			return m_Position;
		}
		private set
		{
			m_Position.target = value.target;
			if (m_Position == value)
			{
				value.Dispose();
				return;
			}
			m_Position.Dispose();
			m_Position = value;
			Notify("position");
		}
	}

	[CreateProperty]
	public StylePropertyData<StyleLength, Length> right
	{
		get
		{
			return m_Right;
		}
		private set
		{
			m_Right.target = value.target;
			if (m_Right == value)
			{
				value.Dispose();
				return;
			}
			m_Right.Dispose();
			m_Right = value;
			Notify("right");
		}
	}

	[CreateProperty]
	public StylePropertyData<StyleRotate, Rotate> rotate
	{
		get
		{
			return m_Rotate;
		}
		private set
		{
			m_Rotate.target = value.target;
			if (m_Rotate == value)
			{
				value.Dispose();
				return;
			}
			m_Rotate.Dispose();
			m_Rotate = value;
			Notify("rotate");
		}
	}

	[CreateProperty]
	public StylePropertyData<StyleScale, Scale> scale
	{
		get
		{
			return m_Scale;
		}
		private set
		{
			m_Scale.target = value.target;
			if (m_Scale == value)
			{
				value.Dispose();
				return;
			}
			m_Scale.Dispose();
			m_Scale = value;
			Notify("scale");
		}
	}

	[CreateProperty]
	public StylePropertyData<StyleEnum<TextOverflow>, TextOverflow> textOverflow
	{
		get
		{
			return m_TextOverflow;
		}
		private set
		{
			m_TextOverflow.target = value.target;
			if (m_TextOverflow == value)
			{
				value.Dispose();
				return;
			}
			m_TextOverflow.Dispose();
			m_TextOverflow = value;
			Notify("textOverflow");
		}
	}

	[CreateProperty]
	public StylePropertyData<StyleTextShadow, TextShadow> textShadow
	{
		get
		{
			return m_TextShadow;
		}
		private set
		{
			m_TextShadow.target = value.target;
			if (m_TextShadow == value)
			{
				value.Dispose();
				return;
			}
			m_TextShadow.Dispose();
			m_TextShadow = value;
			Notify("textShadow");
		}
	}

	[CreateProperty]
	public StylePropertyData<StyleLength, Length> top
	{
		get
		{
			return m_Top;
		}
		private set
		{
			m_Top.target = value.target;
			if (m_Top == value)
			{
				value.Dispose();
				return;
			}
			m_Top.Dispose();
			m_Top = value;
			Notify("top");
		}
	}

	[CreateProperty]
	public StylePropertyData<StyleTransformOrigin, TransformOrigin> transformOrigin
	{
		get
		{
			return m_TransformOrigin;
		}
		private set
		{
			m_TransformOrigin.target = value.target;
			if (m_TransformOrigin == value)
			{
				value.Dispose();
				return;
			}
			m_TransformOrigin.Dispose();
			m_TransformOrigin = value;
			Notify("transformOrigin");
		}
	}

	[CreateProperty]
	public StylePropertyData<StyleList<TimeValue>, List<TimeValue>> transitionDelay
	{
		get
		{
			return m_TransitionDelay;
		}
		private set
		{
			m_TransitionDelay.target = value.target;
			if (m_TransitionDelay == value)
			{
				value.Dispose();
				return;
			}
			m_TransitionDelay.Dispose();
			m_TransitionDelay = value;
			Notify("transitionDelay");
		}
	}

	[CreateProperty]
	public StylePropertyData<StyleList<TimeValue>, List<TimeValue>> transitionDuration
	{
		get
		{
			return m_TransitionDuration;
		}
		private set
		{
			m_TransitionDuration.target = value.target;
			if (m_TransitionDuration == value)
			{
				value.Dispose();
				return;
			}
			m_TransitionDuration.Dispose();
			m_TransitionDuration = value;
			Notify("transitionDuration");
		}
	}

	[CreateProperty]
	public StylePropertyData<StyleList<StylePropertyName>, List<StylePropertyName>> transitionProperty
	{
		get
		{
			return m_TransitionProperty;
		}
		private set
		{
			m_TransitionProperty.target = value.target;
			if (m_TransitionProperty == value)
			{
				value.Dispose();
				return;
			}
			m_TransitionProperty.Dispose();
			m_TransitionProperty = value;
			Notify("transitionProperty");
		}
	}

	[CreateProperty]
	public StylePropertyData<StyleList<EasingFunction>, List<EasingFunction>> transitionTimingFunction
	{
		get
		{
			return m_TransitionTimingFunction;
		}
		private set
		{
			m_TransitionTimingFunction.target = value.target;
			if (m_TransitionTimingFunction == value)
			{
				value.Dispose();
				return;
			}
			m_TransitionTimingFunction.Dispose();
			m_TransitionTimingFunction = value;
			Notify("transitionTimingFunction");
		}
	}

	[CreateProperty]
	public StylePropertyData<StyleTranslate, Translate> translate
	{
		get
		{
			return m_Translate;
		}
		private set
		{
			m_Translate.target = value.target;
			if (m_Translate == value)
			{
				value.Dispose();
				return;
			}
			m_Translate.Dispose();
			m_Translate = value;
			Notify("translate");
		}
	}

	[CreateProperty]
	public StylePropertyData<StyleColor, Color> unityBackgroundImageTintColor
	{
		get
		{
			return m_UnityBackgroundImageTintColor;
		}
		private set
		{
			m_UnityBackgroundImageTintColor.target = value.target;
			if (m_UnityBackgroundImageTintColor == value)
			{
				value.Dispose();
				return;
			}
			m_UnityBackgroundImageTintColor.Dispose();
			m_UnityBackgroundImageTintColor = value;
			Notify("unityBackgroundImageTintColor");
		}
	}

	[CreateProperty]
	public StylePropertyData<StyleEnum<EditorTextRenderingMode>, EditorTextRenderingMode> unityEditorTextRenderingMode
	{
		get
		{
			return m_UnityEditorTextRenderingMode;
		}
		private set
		{
			m_UnityEditorTextRenderingMode.target = value.target;
			if (m_UnityEditorTextRenderingMode == value)
			{
				value.Dispose();
				return;
			}
			m_UnityEditorTextRenderingMode.Dispose();
			m_UnityEditorTextRenderingMode = value;
			Notify("unityEditorTextRenderingMode");
		}
	}

	[CreateProperty]
	public StylePropertyData<StyleFont, Font> unityFont
	{
		get
		{
			return m_UnityFont;
		}
		private set
		{
			m_UnityFont.target = value.target;
			if (m_UnityFont == value)
			{
				value.Dispose();
				return;
			}
			m_UnityFont.Dispose();
			m_UnityFont = value;
			Notify("unityFont");
		}
	}

	[CreateProperty]
	public StylePropertyData<StyleFontDefinition, FontDefinition> unityFontDefinition
	{
		get
		{
			return m_UnityFontDefinition;
		}
		private set
		{
			m_UnityFontDefinition.target = value.target;
			if (m_UnityFontDefinition == value)
			{
				value.Dispose();
				return;
			}
			m_UnityFontDefinition.Dispose();
			m_UnityFontDefinition = value;
			Notify("unityFontDefinition");
		}
	}

	[CreateProperty]
	public StylePropertyData<StyleEnum<FontStyle>, FontStyle> unityFontStyleAndWeight
	{
		get
		{
			return m_UnityFontStyleAndWeight;
		}
		private set
		{
			m_UnityFontStyleAndWeight.target = value.target;
			if (m_UnityFontStyleAndWeight == value)
			{
				value.Dispose();
				return;
			}
			m_UnityFontStyleAndWeight.Dispose();
			m_UnityFontStyleAndWeight = value;
			Notify("unityFontStyleAndWeight");
		}
	}

	[CreateProperty]
	public StylePropertyData<StyleEnum<OverflowClipBox>, OverflowClipBox> unityOverflowClipBox
	{
		get
		{
			return m_UnityOverflowClipBox;
		}
		private set
		{
			m_UnityOverflowClipBox.target = value.target;
			if (m_UnityOverflowClipBox == value)
			{
				value.Dispose();
				return;
			}
			m_UnityOverflowClipBox.Dispose();
			m_UnityOverflowClipBox = value;
			Notify("unityOverflowClipBox");
		}
	}

	[CreateProperty]
	public StylePropertyData<StyleLength, Length> unityParagraphSpacing
	{
		get
		{
			return m_UnityParagraphSpacing;
		}
		private set
		{
			m_UnityParagraphSpacing.target = value.target;
			if (m_UnityParagraphSpacing == value)
			{
				value.Dispose();
				return;
			}
			m_UnityParagraphSpacing.Dispose();
			m_UnityParagraphSpacing = value;
			Notify("unityParagraphSpacing");
		}
	}

	[CreateProperty]
	public StylePropertyData<StyleInt, int> unitySliceBottom
	{
		get
		{
			return m_UnitySliceBottom;
		}
		private set
		{
			m_UnitySliceBottom.target = value.target;
			if (m_UnitySliceBottom == value)
			{
				value.Dispose();
				return;
			}
			m_UnitySliceBottom.Dispose();
			m_UnitySliceBottom = value;
			Notify("unitySliceBottom");
		}
	}

	[CreateProperty]
	public StylePropertyData<StyleInt, int> unitySliceLeft
	{
		get
		{
			return m_UnitySliceLeft;
		}
		private set
		{
			m_UnitySliceLeft.target = value.target;
			if (m_UnitySliceLeft == value)
			{
				value.Dispose();
				return;
			}
			m_UnitySliceLeft.Dispose();
			m_UnitySliceLeft = value;
			Notify("unitySliceLeft");
		}
	}

	[CreateProperty]
	public StylePropertyData<StyleInt, int> unitySliceRight
	{
		get
		{
			return m_UnitySliceRight;
		}
		private set
		{
			m_UnitySliceRight.target = value.target;
			if (m_UnitySliceRight == value)
			{
				value.Dispose();
				return;
			}
			m_UnitySliceRight.Dispose();
			m_UnitySliceRight = value;
			Notify("unitySliceRight");
		}
	}

	[CreateProperty]
	public StylePropertyData<StyleFloat, float> unitySliceScale
	{
		get
		{
			return m_UnitySliceScale;
		}
		private set
		{
			m_UnitySliceScale.target = value.target;
			if (m_UnitySliceScale == value)
			{
				value.Dispose();
				return;
			}
			m_UnitySliceScale.Dispose();
			m_UnitySliceScale = value;
			Notify("unitySliceScale");
		}
	}

	[CreateProperty]
	public StylePropertyData<StyleInt, int> unitySliceTop
	{
		get
		{
			return m_UnitySliceTop;
		}
		private set
		{
			m_UnitySliceTop.target = value.target;
			if (m_UnitySliceTop == value)
			{
				value.Dispose();
				return;
			}
			m_UnitySliceTop.Dispose();
			m_UnitySliceTop = value;
			Notify("unitySliceTop");
		}
	}

	[CreateProperty]
	public StylePropertyData<StyleEnum<SliceType>, SliceType> unitySliceType
	{
		get
		{
			return m_UnitySliceType;
		}
		private set
		{
			m_UnitySliceType.target = value.target;
			if (m_UnitySliceType == value)
			{
				value.Dispose();
				return;
			}
			m_UnitySliceType.Dispose();
			m_UnitySliceType = value;
			Notify("unitySliceType");
		}
	}

	[CreateProperty]
	public StylePropertyData<StyleEnum<TextAnchor>, TextAnchor> unityTextAlign
	{
		get
		{
			return m_UnityTextAlign;
		}
		private set
		{
			m_UnityTextAlign.target = value.target;
			if (m_UnityTextAlign == value)
			{
				value.Dispose();
				return;
			}
			m_UnityTextAlign.Dispose();
			m_UnityTextAlign = value;
			Notify("unityTextAlign");
		}
	}

	[CreateProperty]
	public StylePropertyData<StyleTextAutoSize, TextAutoSize> unityTextAutoSize
	{
		get
		{
			return m_UnityTextAutoSize;
		}
		private set
		{
			m_UnityTextAutoSize.target = value.target;
			if (m_UnityTextAutoSize == value)
			{
				value.Dispose();
				return;
			}
			m_UnityTextAutoSize.Dispose();
			m_UnityTextAutoSize = value;
			Notify("unityTextAutoSize");
		}
	}

	[CreateProperty]
	public StylePropertyData<StyleEnum<TextGeneratorType>, TextGeneratorType> unityTextGenerator
	{
		get
		{
			return m_UnityTextGenerator;
		}
		private set
		{
			m_UnityTextGenerator.target = value.target;
			if (m_UnityTextGenerator == value)
			{
				value.Dispose();
				return;
			}
			m_UnityTextGenerator.Dispose();
			m_UnityTextGenerator = value;
			Notify("unityTextGenerator");
		}
	}

	[CreateProperty]
	public StylePropertyData<StyleColor, Color> unityTextOutlineColor
	{
		get
		{
			return m_UnityTextOutlineColor;
		}
		private set
		{
			m_UnityTextOutlineColor.target = value.target;
			if (m_UnityTextOutlineColor == value)
			{
				value.Dispose();
				return;
			}
			m_UnityTextOutlineColor.Dispose();
			m_UnityTextOutlineColor = value;
			Notify("unityTextOutlineColor");
		}
	}

	[CreateProperty]
	public StylePropertyData<StyleFloat, float> unityTextOutlineWidth
	{
		get
		{
			return m_UnityTextOutlineWidth;
		}
		private set
		{
			m_UnityTextOutlineWidth.target = value.target;
			if (m_UnityTextOutlineWidth == value)
			{
				value.Dispose();
				return;
			}
			m_UnityTextOutlineWidth.Dispose();
			m_UnityTextOutlineWidth = value;
			Notify("unityTextOutlineWidth");
		}
	}

	[CreateProperty]
	public StylePropertyData<StyleEnum<TextOverflowPosition>, TextOverflowPosition> unityTextOverflowPosition
	{
		get
		{
			return m_UnityTextOverflowPosition;
		}
		private set
		{
			m_UnityTextOverflowPosition.target = value.target;
			if (m_UnityTextOverflowPosition == value)
			{
				value.Dispose();
				return;
			}
			m_UnityTextOverflowPosition.Dispose();
			m_UnityTextOverflowPosition = value;
			Notify("unityTextOverflowPosition");
		}
	}

	[CreateProperty]
	public StylePropertyData<StyleEnum<Visibility>, Visibility> visibility
	{
		get
		{
			return m_Visibility;
		}
		private set
		{
			m_Visibility.target = value.target;
			if (m_Visibility == value)
			{
				value.Dispose();
				return;
			}
			m_Visibility.Dispose();
			m_Visibility = value;
			Notify("visibility");
		}
	}

	[CreateProperty]
	public StylePropertyData<StyleEnum<WhiteSpace>, WhiteSpace> whiteSpace
	{
		get
		{
			return m_WhiteSpace;
		}
		private set
		{
			m_WhiteSpace.target = value.target;
			if (m_WhiteSpace == value)
			{
				value.Dispose();
				return;
			}
			m_WhiteSpace.Dispose();
			m_WhiteSpace = value;
			Notify("whiteSpace");
		}
	}

	[CreateProperty]
	public StylePropertyData<StyleLength, Length> width
	{
		get
		{
			return m_Width;
		}
		private set
		{
			m_Width.target = value.target;
			if (m_Width == value)
			{
				value.Dispose();
				return;
			}
			m_Width.Dispose();
			m_Width = value;
			Notify("width");
		}
	}

	[CreateProperty]
	public StylePropertyData<StyleLength, Length> wordSpacing
	{
		get
		{
			return m_WordSpacing;
		}
		private set
		{
			m_WordSpacing.target = value.target;
			if (m_WordSpacing == value)
			{
				value.Dispose();
				return;
			}
			m_WordSpacing.Dispose();
			m_WordSpacing = value;
			Notify("wordSpacing");
		}
	}

	public event EventHandler<BindablePropertyChangedEventArgs> propertyChanged;

	private void Refresh(VisualElement element, in ResolutionContext context)
	{
		alignContent = ComputeStyleProperty<StyleEnum<Align>, Align>(element, "alignContent", element.style.alignContent, element.computedStyle.alignContent, in context);
		alignItems = ComputeStyleProperty<StyleEnum<Align>, Align>(element, "alignItems", element.style.alignItems, element.computedStyle.alignItems, in context);
		alignSelf = ComputeStyleProperty<StyleEnum<Align>, Align>(element, "alignSelf", element.style.alignSelf, element.computedStyle.alignSelf, in context);
		backgroundColor = ComputeStyleProperty<StyleColor, Color>(element, "backgroundColor", element.style.backgroundColor, element.computedStyle.backgroundColor, in context);
		backgroundImage = ComputeStyleProperty<StyleBackground, Background>(element, "backgroundImage", element.style.backgroundImage, element.computedStyle.backgroundImage, in context);
		backgroundPositionX = ComputeStyleProperty<StyleBackgroundPosition, BackgroundPosition>(element, "backgroundPositionX", element.style.backgroundPositionX, element.computedStyle.backgroundPositionX, in context);
		backgroundPositionY = ComputeStyleProperty<StyleBackgroundPosition, BackgroundPosition>(element, "backgroundPositionY", element.style.backgroundPositionY, element.computedStyle.backgroundPositionY, in context);
		backgroundRepeat = ComputeStyleProperty<StyleBackgroundRepeat, BackgroundRepeat>(element, "backgroundRepeat", element.style.backgroundRepeat, element.computedStyle.backgroundRepeat, in context);
		backgroundSize = ComputeStyleProperty<StyleBackgroundSize, BackgroundSize>(element, "backgroundSize", element.style.backgroundSize, element.computedStyle.backgroundSize, in context);
		borderBottomColor = ComputeStyleProperty<StyleColor, Color>(element, "borderBottomColor", element.style.borderBottomColor, element.computedStyle.borderBottomColor, in context);
		borderBottomLeftRadius = ComputeStyleProperty<StyleLength, Length>(element, "borderBottomLeftRadius", element.style.borderBottomLeftRadius, element.computedStyle.borderBottomLeftRadius, in context);
		borderBottomRightRadius = ComputeStyleProperty<StyleLength, Length>(element, "borderBottomRightRadius", element.style.borderBottomRightRadius, element.computedStyle.borderBottomRightRadius, in context);
		borderBottomWidth = ComputeStyleProperty<StyleFloat, float>(element, "borderBottomWidth", element.style.borderBottomWidth, element.computedStyle.borderBottomWidth, in context);
		borderLeftColor = ComputeStyleProperty<StyleColor, Color>(element, "borderLeftColor", element.style.borderLeftColor, element.computedStyle.borderLeftColor, in context);
		borderLeftWidth = ComputeStyleProperty<StyleFloat, float>(element, "borderLeftWidth", element.style.borderLeftWidth, element.computedStyle.borderLeftWidth, in context);
		borderRightColor = ComputeStyleProperty<StyleColor, Color>(element, "borderRightColor", element.style.borderRightColor, element.computedStyle.borderRightColor, in context);
		borderRightWidth = ComputeStyleProperty<StyleFloat, float>(element, "borderRightWidth", element.style.borderRightWidth, element.computedStyle.borderRightWidth, in context);
		borderTopColor = ComputeStyleProperty<StyleColor, Color>(element, "borderTopColor", element.style.borderTopColor, element.computedStyle.borderTopColor, in context);
		borderTopLeftRadius = ComputeStyleProperty<StyleLength, Length>(element, "borderTopLeftRadius", element.style.borderTopLeftRadius, element.computedStyle.borderTopLeftRadius, in context);
		borderTopRightRadius = ComputeStyleProperty<StyleLength, Length>(element, "borderTopRightRadius", element.style.borderTopRightRadius, element.computedStyle.borderTopRightRadius, in context);
		borderTopWidth = ComputeStyleProperty<StyleFloat, float>(element, "borderTopWidth", element.style.borderTopWidth, element.computedStyle.borderTopWidth, in context);
		bottom = ComputeStyleProperty<StyleLength, Length>(element, "bottom", element.style.bottom, element.computedStyle.bottom, in context);
		color = ComputeStyleProperty<StyleColor, Color>(element, "color", element.style.color, element.computedStyle.color, in context);
		cursor = ComputeStyleProperty<StyleCursor, Cursor>(element, "cursor", element.style.cursor, element.computedStyle.cursor, in context);
		display = ComputeStyleProperty<StyleEnum<DisplayStyle>, DisplayStyle>(element, "display", element.style.display, element.computedStyle.display, in context);
		flexBasis = ComputeStyleProperty<StyleLength, Length>(element, "flexBasis", element.style.flexBasis, element.computedStyle.flexBasis, in context);
		flexDirection = ComputeStyleProperty<StyleEnum<FlexDirection>, FlexDirection>(element, "flexDirection", element.style.flexDirection, element.computedStyle.flexDirection, in context);
		flexGrow = ComputeStyleProperty<StyleFloat, float>(element, "flexGrow", element.style.flexGrow, element.computedStyle.flexGrow, in context);
		flexShrink = ComputeStyleProperty<StyleFloat, float>(element, "flexShrink", element.style.flexShrink, element.computedStyle.flexShrink, in context);
		flexWrap = ComputeStyleProperty<StyleEnum<Wrap>, Wrap>(element, "flexWrap", element.style.flexWrap, element.computedStyle.flexWrap, in context);
		fontSize = ComputeStyleProperty<StyleLength, Length>(element, "fontSize", element.style.fontSize, element.computedStyle.fontSize, in context);
		height = ComputeStyleProperty<StyleLength, Length>(element, "height", element.style.height, element.computedStyle.height, in context);
		justifyContent = ComputeStyleProperty<StyleEnum<Justify>, Justify>(element, "justifyContent", element.style.justifyContent, element.computedStyle.justifyContent, in context);
		left = ComputeStyleProperty<StyleLength, Length>(element, "left", element.style.left, element.computedStyle.left, in context);
		letterSpacing = ComputeStyleProperty<StyleLength, Length>(element, "letterSpacing", element.style.letterSpacing, element.computedStyle.letterSpacing, in context);
		marginBottom = ComputeStyleProperty<StyleLength, Length>(element, "marginBottom", element.style.marginBottom, element.computedStyle.marginBottom, in context);
		marginLeft = ComputeStyleProperty<StyleLength, Length>(element, "marginLeft", element.style.marginLeft, element.computedStyle.marginLeft, in context);
		marginRight = ComputeStyleProperty<StyleLength, Length>(element, "marginRight", element.style.marginRight, element.computedStyle.marginRight, in context);
		marginTop = ComputeStyleProperty<StyleLength, Length>(element, "marginTop", element.style.marginTop, element.computedStyle.marginTop, in context);
		maxHeight = ComputeStyleProperty<StyleLength, Length>(element, "maxHeight", element.style.maxHeight, element.computedStyle.maxHeight, in context);
		maxWidth = ComputeStyleProperty<StyleLength, Length>(element, "maxWidth", element.style.maxWidth, element.computedStyle.maxWidth, in context);
		minHeight = ComputeStyleProperty<StyleLength, Length>(element, "minHeight", element.style.minHeight, element.computedStyle.minHeight, in context);
		minWidth = ComputeStyleProperty<StyleLength, Length>(element, "minWidth", element.style.minWidth, element.computedStyle.minWidth, in context);
		opacity = ComputeStyleProperty<StyleFloat, float>(element, "opacity", element.style.opacity, element.computedStyle.opacity, in context);
		overflow = ComputeStyleProperty<StyleEnum<Overflow>, OverflowInternal>(element, "overflow", element.style.overflow, element.computedStyle.overflow, in context);
		paddingBottom = ComputeStyleProperty<StyleLength, Length>(element, "paddingBottom", element.style.paddingBottom, element.computedStyle.paddingBottom, in context);
		paddingLeft = ComputeStyleProperty<StyleLength, Length>(element, "paddingLeft", element.style.paddingLeft, element.computedStyle.paddingLeft, in context);
		paddingRight = ComputeStyleProperty<StyleLength, Length>(element, "paddingRight", element.style.paddingRight, element.computedStyle.paddingRight, in context);
		paddingTop = ComputeStyleProperty<StyleLength, Length>(element, "paddingTop", element.style.paddingTop, element.computedStyle.paddingTop, in context);
		position = ComputeStyleProperty<StyleEnum<Position>, Position>(element, "position", element.style.position, element.computedStyle.position, in context);
		right = ComputeStyleProperty<StyleLength, Length>(element, "right", element.style.right, element.computedStyle.right, in context);
		rotate = ComputeStyleProperty<StyleRotate, Rotate>(element, "rotate", element.style.rotate, element.computedStyle.rotate, in context);
		scale = ComputeStyleProperty<StyleScale, Scale>(element, "scale", element.style.scale, element.computedStyle.scale, in context);
		textOverflow = ComputeStyleProperty<StyleEnum<TextOverflow>, TextOverflow>(element, "textOverflow", element.style.textOverflow, element.computedStyle.textOverflow, in context);
		textShadow = ComputeStyleProperty<StyleTextShadow, TextShadow>(element, "textShadow", element.style.textShadow, element.computedStyle.textShadow, in context);
		top = ComputeStyleProperty<StyleLength, Length>(element, "top", element.style.top, element.computedStyle.top, in context);
		transformOrigin = ComputeStyleProperty<StyleTransformOrigin, TransformOrigin>(element, "transformOrigin", element.style.transformOrigin, element.computedStyle.transformOrigin, in context);
		transitionDelay = ComputeStyleProperty<StyleList<TimeValue>, List<TimeValue>>(element, "transitionDelay", element.style.transitionDelay, element.computedStyle.transitionDelay, in context);
		transitionDuration = ComputeStyleProperty<StyleList<TimeValue>, List<TimeValue>>(element, "transitionDuration", element.style.transitionDuration, element.computedStyle.transitionDuration, in context);
		transitionProperty = ComputeStyleProperty<StyleList<StylePropertyName>, List<StylePropertyName>>(element, "transitionProperty", element.style.transitionProperty, element.computedStyle.transitionProperty, in context);
		transitionTimingFunction = ComputeStyleProperty<StyleList<EasingFunction>, List<EasingFunction>>(element, "transitionTimingFunction", element.style.transitionTimingFunction, element.computedStyle.transitionTimingFunction, in context);
		translate = ComputeStyleProperty<StyleTranslate, Translate>(element, "translate", element.style.translate, element.computedStyle.translate, in context);
		unityBackgroundImageTintColor = ComputeStyleProperty<StyleColor, Color>(element, "unityBackgroundImageTintColor", element.style.unityBackgroundImageTintColor, element.computedStyle.unityBackgroundImageTintColor, in context);
		unityEditorTextRenderingMode = ComputeStyleProperty<StyleEnum<EditorTextRenderingMode>, EditorTextRenderingMode>(element, "unityEditorTextRenderingMode", element.style.unityEditorTextRenderingMode, element.computedStyle.unityEditorTextRenderingMode, in context);
		unityFont = ComputeStyleProperty<StyleFont, Font>(element, "unityFont", element.style.unityFont, element.computedStyle.unityFont, in context);
		unityFontDefinition = ComputeStyleProperty<StyleFontDefinition, FontDefinition>(element, "unityFontDefinition", element.style.unityFontDefinition, element.computedStyle.unityFontDefinition, in context);
		unityFontStyleAndWeight = ComputeStyleProperty<StyleEnum<FontStyle>, FontStyle>(element, "unityFontStyleAndWeight", element.style.unityFontStyleAndWeight, element.computedStyle.unityFontStyleAndWeight, in context);
		unityOverflowClipBox = ComputeStyleProperty<StyleEnum<OverflowClipBox>, OverflowClipBox>(element, "unityOverflowClipBox", element.style.unityOverflowClipBox, element.computedStyle.unityOverflowClipBox, in context);
		unityParagraphSpacing = ComputeStyleProperty<StyleLength, Length>(element, "unityParagraphSpacing", element.style.unityParagraphSpacing, element.computedStyle.unityParagraphSpacing, in context);
		unitySliceBottom = ComputeStyleProperty<StyleInt, int>(element, "unitySliceBottom", element.style.unitySliceBottom, element.computedStyle.unitySliceBottom, in context);
		unitySliceLeft = ComputeStyleProperty<StyleInt, int>(element, "unitySliceLeft", element.style.unitySliceLeft, element.computedStyle.unitySliceLeft, in context);
		unitySliceRight = ComputeStyleProperty<StyleInt, int>(element, "unitySliceRight", element.style.unitySliceRight, element.computedStyle.unitySliceRight, in context);
		unitySliceScale = ComputeStyleProperty<StyleFloat, float>(element, "unitySliceScale", element.style.unitySliceScale, element.computedStyle.unitySliceScale, in context);
		unitySliceTop = ComputeStyleProperty<StyleInt, int>(element, "unitySliceTop", element.style.unitySliceTop, element.computedStyle.unitySliceTop, in context);
		unitySliceType = ComputeStyleProperty<StyleEnum<SliceType>, SliceType>(element, "unitySliceType", element.style.unitySliceType, element.computedStyle.unitySliceType, in context);
		unityTextAlign = ComputeStyleProperty<StyleEnum<TextAnchor>, TextAnchor>(element, "unityTextAlign", element.style.unityTextAlign, element.computedStyle.unityTextAlign, in context);
		unityTextAutoSize = ComputeStyleProperty<StyleTextAutoSize, TextAutoSize>(element, "unityTextAutoSize", element.style.unityTextAutoSize, element.computedStyle.unityTextAutoSize, in context);
		unityTextGenerator = ComputeStyleProperty<StyleEnum<TextGeneratorType>, TextGeneratorType>(element, "unityTextGenerator", element.style.unityTextGenerator, element.computedStyle.unityTextGenerator, in context);
		unityTextOutlineColor = ComputeStyleProperty<StyleColor, Color>(element, "unityTextOutlineColor", element.style.unityTextOutlineColor, element.computedStyle.unityTextOutlineColor, in context);
		unityTextOutlineWidth = ComputeStyleProperty<StyleFloat, float>(element, "unityTextOutlineWidth", element.style.unityTextOutlineWidth, element.computedStyle.unityTextOutlineWidth, in context);
		unityTextOverflowPosition = ComputeStyleProperty<StyleEnum<TextOverflowPosition>, TextOverflowPosition>(element, "unityTextOverflowPosition", element.style.unityTextOverflowPosition, element.computedStyle.unityTextOverflowPosition, in context);
		visibility = ComputeStyleProperty<StyleEnum<Visibility>, Visibility>(element, "visibility", element.style.visibility, element.computedStyle.visibility, in context);
		whiteSpace = ComputeStyleProperty<StyleEnum<WhiteSpace>, WhiteSpace>(element, "whiteSpace", element.style.whiteSpace, element.computedStyle.whiteSpace, in context);
		width = ComputeStyleProperty<StyleLength, Length>(element, "width", element.style.width, element.computedStyle.width, in context);
		wordSpacing = ComputeStyleProperty<StyleLength, Length>(element, "wordSpacing", element.style.wordSpacing, element.computedStyle.wordSpacing, in context);
	}

	public StyleDiff()
	{
		m_MatchedRules = new MatchedRulesExtractor(null);
	}

	public void Refresh(VisualElement element, StyleDiffAdditionalDataFlags flags = StyleDiffAdditionalDataFlags.All)
	{
		if (element != null)
		{
			VisualTreeAsset visualTreeAssetSource = element.visualTreeAssetSource;
			StyleSheet styleSheet = (visualTreeAssetSource ? visualTreeAssetSource.inlineSheet : null);
			StyleRule styleRule = element.inlineStyleAccess?.inlineRule.rule;
			Refresh(element, styleSheet, styleRule, flags);
		}
	}

	internal void Refresh(VisualElement element, StyleSheet styleSheet, StyleRule styleRule, StyleDiffAdditionalDataFlags flags = StyleDiffAdditionalDataFlags.All)
	{
		m_MatchedRules.Clear();
		uxmlOverrides.Clear();
		Dictionary<string, UxmlData> value;
		using (CollectionPool<Dictionary<string, UxmlData>, KeyValuePair<string, UxmlData>>.Get(out value))
		{
			if ((flags & StyleDiffAdditionalDataFlags.UxmlInlineProperties) == StyleDiffAdditionalDataFlags.UxmlInlineProperties && styleRule != null)
			{
				StyleProperty[] properties = styleRule.properties;
				foreach (StyleProperty styleProperty in properties)
				{
					if (StylePropertyUtil.ussNameToCSharpName.TryGetValue(styleProperty.name, out var value2) && value2 != styleProperty.name)
					{
						uxmlOverrides.Add(value2);
						UxmlData data = value.GetValueOrDefault(value2);
						value[value2] = UxmlData.WithProperty(in data, styleProperty);
					}
					uxmlOverrides.Add(styleProperty.name);
					UxmlData data2 = value.GetValueOrDefault(value2);
					value[styleProperty.name] = UxmlData.WithProperty(in data2, styleProperty);
				}
			}
			if ((flags & StyleDiffAdditionalDataFlags.Bindings) == StyleDiffAdditionalDataFlags.Bindings)
			{
				List<BindingInfo> value3;
				using (CollectionPool<List<BindingInfo>, BindingInfo>.Get(out value3))
				{
					element.GetBindingInfos(value3);
					foreach (BindingInfo item in value3)
					{
						PropertyPath propertyPath = item.bindingId;
						if (propertyPath.Length == 2 && propertyPath[0].IsName && string.CompareOrdinal(propertyPath[0].Name, "style") == 0 && propertyPath[1].IsName)
						{
							string name = propertyPath[1].Name;
							uxmlOverrides.Add(name);
							UxmlData data3 = value.GetValueOrDefault(name);
							value[name] = UxmlData.WithBindingInfo(in data3, item);
						}
					}
				}
			}
			if ((flags & StyleDiffAdditionalDataFlags.Selectors) == StyleDiffAdditionalDataFlags.Selectors)
			{
				Dictionary<string, SelectorMatchRecord> value4;
				using (CollectionPool<Dictionary<string, SelectorMatchRecord>, KeyValuePair<string, SelectorMatchRecord>>.Get(out value4))
				{
					FindMatchingRules(element, value4);
					foreach (KeyValuePair<string, SelectorMatchRecord> item2 in value4)
					{
						UxmlData data4 = value.GetValueOrDefault(item2.Key);
						value[item2.Key] = UxmlData.WithSelector(in data4, item2.Value);
					}
				}
			}
			Refresh(element, new ResolutionContext(this, styleSheet, value, uxmlOverrides));
		}
	}

	private void FindMatchingRules(VisualElement element, Dictionary<string, SelectorMatchRecord> propertyToMatchRecord)
	{
		m_MatchedRules.FindMatchingRules(element);
		for (int i = 0; i < m_MatchedRules.matchRecords.Count; i++)
		{
			SelectorMatchRecord value = m_MatchedRules.matchRecords[i];
			StyleProperty[] properties = value.complexSelector.rule.properties;
			foreach (StyleProperty styleProperty in properties)
			{
				if (StylePropertyUtil.ussNameToCSharpName.TryGetValue(styleProperty.name, out var value2) && value2 != styleProperty.name)
				{
					propertyToMatchRecord[value2] = value;
				}
				propertyToMatchRecord[styleProperty.name] = value;
			}
		}
	}

	private static StylePropertyData<TInline, TComputed> ComputeStyleProperty<TInline, TComputed>(VisualElement element, string propertyName, in TInline inlineStyle, in TComputed computedStyle, in ResolutionContext context)
	{
		StylePropertyData<TInline, TComputed> result = new StylePropertyData<TInline, TComputed>
		{
			target = element,
			inlineValue = inlineStyle,
			computedValue = computedStyle
		};
		if (!context.uxmlData.TryGetValue(propertyName, out var value))
		{
			context.ClearOverride(propertyName);
			return result;
		}
		bool flag = value.inlineProperty != null;
		result.uxmlValue = (flag ? new UxmlStyleProperty(value.inlineProperty.values, value.inlineProperty.ContainsVariable()) : new UxmlStyleProperty(Array.Empty<StyleValueHandle>(), requireVariableResolve: false));
		result.binding = value.bindingInfo.binding;
		if (flag || value.bindingInfo.binding != null)
		{
			context.MarkAsOverride(propertyName);
		}
		else
		{
			context.ClearOverride(propertyName);
		}
		result.selector = value.selector;
		return result;
	}

	public bool HasUxmlOverrides(string stylePropertyName)
	{
		return !string.IsNullOrEmpty(stylePropertyName) && uxmlOverrides.Contains(stylePropertyName);
	}

	private void Notify([CallerMemberName] string name = null)
	{
		m_Version++;
		this.propertyChanged?.Invoke(this, new BindablePropertyChangedEventArgs((BindingId)name));
	}

	public long GetViewHashCode()
	{
		return m_Version;
	}

	public void Dispose()
	{
		m_MatchedRules.Clear();
		uxmlOverrides.Clear();
		DisposeProperties();
	}

	private void DisposeProperties()
	{
		m_AlignContent.Dispose();
		m_AlignItems.Dispose();
		m_AlignSelf.Dispose();
		m_BackgroundColor.Dispose();
		m_BackgroundImage.Dispose();
		m_BackgroundPositionX.Dispose();
		m_BackgroundPositionY.Dispose();
		m_BackgroundRepeat.Dispose();
		m_BackgroundSize.Dispose();
		m_BorderBottomColor.Dispose();
		m_BorderBottomLeftRadius.Dispose();
		m_BorderBottomRightRadius.Dispose();
		m_BorderBottomWidth.Dispose();
		m_BorderLeftColor.Dispose();
		m_BorderLeftWidth.Dispose();
		m_BorderRightColor.Dispose();
		m_BorderRightWidth.Dispose();
		m_BorderTopColor.Dispose();
		m_BorderTopLeftRadius.Dispose();
		m_BorderTopRightRadius.Dispose();
		m_BorderTopWidth.Dispose();
		m_Bottom.Dispose();
		m_Color.Dispose();
		m_Cursor.Dispose();
		m_Display.Dispose();
		m_FlexBasis.Dispose();
		m_FlexDirection.Dispose();
		m_FlexGrow.Dispose();
		m_FlexShrink.Dispose();
		m_FlexWrap.Dispose();
		m_FontSize.Dispose();
		m_Height.Dispose();
		m_JustifyContent.Dispose();
		m_Left.Dispose();
		m_LetterSpacing.Dispose();
		m_MarginBottom.Dispose();
		m_MarginLeft.Dispose();
		m_MarginRight.Dispose();
		m_MarginTop.Dispose();
		m_MaxHeight.Dispose();
		m_MaxWidth.Dispose();
		m_MinHeight.Dispose();
		m_MinWidth.Dispose();
		m_Opacity.Dispose();
		m_Overflow.Dispose();
		m_PaddingBottom.Dispose();
		m_PaddingLeft.Dispose();
		m_PaddingRight.Dispose();
		m_PaddingTop.Dispose();
		m_Position.Dispose();
		m_Right.Dispose();
		m_Rotate.Dispose();
		m_Scale.Dispose();
		m_TextOverflow.Dispose();
		m_TextShadow.Dispose();
		m_Top.Dispose();
		m_TransformOrigin.Dispose();
		m_TransitionDelay.Dispose();
		m_TransitionDuration.Dispose();
		m_TransitionProperty.Dispose();
		m_TransitionTimingFunction.Dispose();
		m_Translate.Dispose();
		m_UnityBackgroundImageTintColor.Dispose();
		m_UnityEditorTextRenderingMode.Dispose();
		m_UnityFont.Dispose();
		m_UnityFontDefinition.Dispose();
		m_UnityFontStyleAndWeight.Dispose();
		m_UnityOverflowClipBox.Dispose();
		m_UnityParagraphSpacing.Dispose();
		m_UnitySliceBottom.Dispose();
		m_UnitySliceLeft.Dispose();
		m_UnitySliceRight.Dispose();
		m_UnitySliceScale.Dispose();
		m_UnitySliceTop.Dispose();
		m_UnitySliceType.Dispose();
		m_UnityTextAlign.Dispose();
		m_UnityTextAutoSize.Dispose();
		m_UnityTextGenerator.Dispose();
		m_UnityTextOutlineColor.Dispose();
		m_UnityTextOutlineWidth.Dispose();
		m_UnityTextOverflowPosition.Dispose();
		m_Visibility.Dispose();
		m_WhiteSpace.Dispose();
		m_Width.Dispose();
		m_WordSpacing.Dispose();
	}
}
