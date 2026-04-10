using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using Unity.Collections.LowLevel.Unsafe;
using UnityEngine.TextCore.Text;
using UnityEngine.UIElements.Layout;
using UnityEngine.UIElements.StyleSheets;

namespace UnityEngine.UIElements;

internal class InlineStyleAccess : StyleValueCollection, IStyle
{
	internal struct InlineRule
	{
		public StyleSheet sheet;

		public StyleRule rule;

		public StyleProperty[] properties => rule?.properties;
	}

	private static StylePropertyReader s_StylePropertyReader = new StylePropertyReader();

	private List<StyleValueManaged> m_ValuesManaged;

	private bool m_HasInlineCursor;

	private StyleCursor m_InlineCursor;

	private bool m_HasInlineTextShadow;

	private StyleTextShadow m_InlineTextShadow;

	private bool m_HasInlineTextAutoSize;

	private StyleTextAutoSize m_InlineTextAutoSize;

	private bool m_HasInlineTransformOrigin;

	private StyleTransformOrigin m_InlineTransformOrigin;

	private bool m_HasInlineTranslate;

	private StyleTranslate m_InlineTranslateOperation;

	private bool m_HasInlineRotate;

	private StyleRotate m_InlineRotateOperation;

	private bool m_HasInlineScale;

	private StyleScale m_InlineScale;

	private bool m_HasInlineBackgroundSize;

	public StyleBackgroundSize m_InlineBackgroundSize;

	private bool m_HasInlineFilter;

	public StyleList<FilterFunction> m_InlineFilter;

	private InlineRule m_InlineRule;

	StyleEnum<Align> IStyle.alignContent
	{
		get
		{
			StyleInt styleInt = GetStyleInt(StylePropertyId.AlignContent);
			return new StyleEnum<Align>((Align)styleInt.value, styleInt.keyword);
		}
		set
		{
			if (SetStyleValue(StylePropertyId.AlignContent, value))
			{
				ve.IncrementVersion(VersionChangeType.Layout | VersionChangeType.Styles);
				ve.layoutNode.AlignContent = (LayoutAlign)ve.computedStyle.alignContent;
			}
		}
	}

	StyleEnum<Align> IStyle.alignItems
	{
		get
		{
			StyleInt styleInt = GetStyleInt(StylePropertyId.AlignItems);
			return new StyleEnum<Align>((Align)styleInt.value, styleInt.keyword);
		}
		set
		{
			if (SetStyleValue(StylePropertyId.AlignItems, value))
			{
				ve.IncrementVersion(VersionChangeType.Layout | VersionChangeType.Styles);
				ve.layoutNode.AlignItems = (LayoutAlign)ve.computedStyle.alignItems;
			}
		}
	}

	StyleEnum<Align> IStyle.alignSelf
	{
		get
		{
			StyleInt styleInt = GetStyleInt(StylePropertyId.AlignSelf);
			return new StyleEnum<Align>((Align)styleInt.value, styleInt.keyword);
		}
		set
		{
			if (SetStyleValue(StylePropertyId.AlignSelf, value))
			{
				ve.IncrementVersion(VersionChangeType.Layout | VersionChangeType.Styles);
				ve.layoutNode.AlignSelf = (LayoutAlign)ve.computedStyle.alignSelf;
			}
		}
	}

	StyleRatio IStyle.aspectRatio
	{
		get
		{
			return GetStyleRatio(StylePropertyId.AspectRatio);
		}
		set
		{
			if (SetStyleValue(StylePropertyId.AspectRatio, value))
			{
				ve.IncrementVersion(VersionChangeType.Layout | VersionChangeType.Styles);
				ve.layoutNode.AspectRatio = ve.computedStyle.aspectRatio;
			}
		}
	}

	StyleColor IStyle.backgroundColor
	{
		get
		{
			return GetStyleColor(StylePropertyId.BackgroundColor);
		}
		set
		{
			if (SetStyleValue(StylePropertyId.BackgroundColor, value))
			{
				ve.IncrementVersion(VersionChangeType.Styles | VersionChangeType.Color);
			}
		}
	}

	StyleBackground IStyle.backgroundImage
	{
		get
		{
			return GetStyleBackground(StylePropertyId.BackgroundImage);
		}
		set
		{
			if (SetStyleValue(StylePropertyId.BackgroundImage, value))
			{
				ve.IncrementVersion(VersionChangeType.Styles | VersionChangeType.Repaint);
			}
		}
	}

	StyleBackgroundPosition IStyle.backgroundPositionX
	{
		get
		{
			return GetStyleBackgroundPosition(StylePropertyId.BackgroundPositionX);
		}
		set
		{
			if (SetStyleValue(StylePropertyId.BackgroundPositionX, value))
			{
				ve.IncrementVersion(VersionChangeType.Styles | VersionChangeType.Repaint);
			}
		}
	}

	StyleBackgroundPosition IStyle.backgroundPositionY
	{
		get
		{
			return GetStyleBackgroundPosition(StylePropertyId.BackgroundPositionY);
		}
		set
		{
			if (SetStyleValue(StylePropertyId.BackgroundPositionY, value))
			{
				ve.IncrementVersion(VersionChangeType.Styles | VersionChangeType.Repaint);
			}
		}
	}

	StyleBackgroundRepeat IStyle.backgroundRepeat
	{
		get
		{
			return GetStyleBackgroundRepeat(StylePropertyId.BackgroundRepeat);
		}
		set
		{
			if (SetStyleValue(StylePropertyId.BackgroundRepeat, value))
			{
				ve.IncrementVersion(VersionChangeType.Styles | VersionChangeType.Repaint);
			}
		}
	}

	StyleColor IStyle.borderBottomColor
	{
		get
		{
			return GetStyleColor(StylePropertyId.BorderBottomColor);
		}
		set
		{
			if (SetStyleValue(StylePropertyId.BorderBottomColor, value))
			{
				ve.IncrementVersion(VersionChangeType.Styles | VersionChangeType.Color);
			}
		}
	}

	StyleLength IStyle.borderBottomLeftRadius
	{
		get
		{
			return GetStyleLength(StylePropertyId.BorderBottomLeftRadius);
		}
		set
		{
			if (SetStyleValue(StylePropertyId.BorderBottomLeftRadius, value))
			{
				ve.IncrementVersion(VersionChangeType.Styles | VersionChangeType.BorderRadius | VersionChangeType.Repaint);
			}
		}
	}

	StyleLength IStyle.borderBottomRightRadius
	{
		get
		{
			return GetStyleLength(StylePropertyId.BorderBottomRightRadius);
		}
		set
		{
			if (SetStyleValue(StylePropertyId.BorderBottomRightRadius, value))
			{
				ve.IncrementVersion(VersionChangeType.Styles | VersionChangeType.BorderRadius | VersionChangeType.Repaint);
			}
		}
	}

	StyleFloat IStyle.borderBottomWidth
	{
		get
		{
			return GetStyleFloat(StylePropertyId.BorderBottomWidth);
		}
		set
		{
			if (SetStyleValue(StylePropertyId.BorderBottomWidth, value))
			{
				ve.IncrementVersion(VersionChangeType.Layout | VersionChangeType.Styles | VersionChangeType.BorderWidth | VersionChangeType.Repaint);
				ve.layoutNode.BorderBottomWidth = ve.computedStyle.borderBottomWidth;
			}
		}
	}

	StyleColor IStyle.borderLeftColor
	{
		get
		{
			return GetStyleColor(StylePropertyId.BorderLeftColor);
		}
		set
		{
			if (SetStyleValue(StylePropertyId.BorderLeftColor, value))
			{
				ve.IncrementVersion(VersionChangeType.Styles | VersionChangeType.Color);
			}
		}
	}

	StyleFloat IStyle.borderLeftWidth
	{
		get
		{
			return GetStyleFloat(StylePropertyId.BorderLeftWidth);
		}
		set
		{
			if (SetStyleValue(StylePropertyId.BorderLeftWidth, value))
			{
				ve.IncrementVersion(VersionChangeType.Layout | VersionChangeType.Styles | VersionChangeType.BorderWidth | VersionChangeType.Repaint);
				ve.layoutNode.BorderLeftWidth = ve.computedStyle.borderLeftWidth;
			}
		}
	}

	StyleColor IStyle.borderRightColor
	{
		get
		{
			return GetStyleColor(StylePropertyId.BorderRightColor);
		}
		set
		{
			if (SetStyleValue(StylePropertyId.BorderRightColor, value))
			{
				ve.IncrementVersion(VersionChangeType.Styles | VersionChangeType.Color);
			}
		}
	}

	StyleFloat IStyle.borderRightWidth
	{
		get
		{
			return GetStyleFloat(StylePropertyId.BorderRightWidth);
		}
		set
		{
			if (SetStyleValue(StylePropertyId.BorderRightWidth, value))
			{
				ve.IncrementVersion(VersionChangeType.Layout | VersionChangeType.Styles | VersionChangeType.BorderWidth | VersionChangeType.Repaint);
				ve.layoutNode.BorderRightWidth = ve.computedStyle.borderRightWidth;
			}
		}
	}

	StyleColor IStyle.borderTopColor
	{
		get
		{
			return GetStyleColor(StylePropertyId.BorderTopColor);
		}
		set
		{
			if (SetStyleValue(StylePropertyId.BorderTopColor, value))
			{
				ve.IncrementVersion(VersionChangeType.Styles | VersionChangeType.Color);
			}
		}
	}

	StyleLength IStyle.borderTopLeftRadius
	{
		get
		{
			return GetStyleLength(StylePropertyId.BorderTopLeftRadius);
		}
		set
		{
			if (SetStyleValue(StylePropertyId.BorderTopLeftRadius, value))
			{
				ve.IncrementVersion(VersionChangeType.Styles | VersionChangeType.BorderRadius | VersionChangeType.Repaint);
			}
		}
	}

	StyleLength IStyle.borderTopRightRadius
	{
		get
		{
			return GetStyleLength(StylePropertyId.BorderTopRightRadius);
		}
		set
		{
			if (SetStyleValue(StylePropertyId.BorderTopRightRadius, value))
			{
				ve.IncrementVersion(VersionChangeType.Styles | VersionChangeType.BorderRadius | VersionChangeType.Repaint);
			}
		}
	}

	StyleFloat IStyle.borderTopWidth
	{
		get
		{
			return GetStyleFloat(StylePropertyId.BorderTopWidth);
		}
		set
		{
			if (SetStyleValue(StylePropertyId.BorderTopWidth, value))
			{
				ve.IncrementVersion(VersionChangeType.Layout | VersionChangeType.Styles | VersionChangeType.BorderWidth | VersionChangeType.Repaint);
				ve.layoutNode.BorderTopWidth = ve.computedStyle.borderTopWidth;
			}
		}
	}

	StyleLength IStyle.bottom
	{
		get
		{
			return GetStyleLength(StylePropertyId.Bottom);
		}
		set
		{
			if (SetStyleValue(StylePropertyId.Bottom, value))
			{
				ve.IncrementVersion(VersionChangeType.Layout | VersionChangeType.Styles);
				ve.layoutNode.Bottom = ve.computedStyle.bottom.ToLayoutValue();
			}
		}
	}

	StyleColor IStyle.color
	{
		get
		{
			return GetStyleColor(StylePropertyId.Color);
		}
		set
		{
			if (SetStyleValue(StylePropertyId.Color, value))
			{
				ve.IncrementVersion(VersionChangeType.StyleSheet | VersionChangeType.Styles | VersionChangeType.Color);
			}
		}
	}

	StyleEnum<DisplayStyle> IStyle.display
	{
		get
		{
			StyleInt styleInt = GetStyleInt(StylePropertyId.Display);
			return new StyleEnum<DisplayStyle>((DisplayStyle)styleInt.value, styleInt.keyword);
		}
		set
		{
			if (SetStyleValue(StylePropertyId.Display, value))
			{
				ve.IncrementVersion(VersionChangeType.Layout | VersionChangeType.Styles);
				ve.layoutNode.Display = (LayoutDisplay)ve.computedStyle.display;
			}
		}
	}

	StyleLength IStyle.flexBasis
	{
		get
		{
			return GetStyleLength(StylePropertyId.FlexBasis);
		}
		set
		{
			if (SetStyleValue(StylePropertyId.FlexBasis, value))
			{
				ve.IncrementVersion(VersionChangeType.Layout | VersionChangeType.Styles);
				ve.layoutNode.FlexBasis = ve.computedStyle.flexBasis.ToLayoutValue();
			}
		}
	}

	StyleEnum<FlexDirection> IStyle.flexDirection
	{
		get
		{
			StyleInt styleInt = GetStyleInt(StylePropertyId.FlexDirection);
			return new StyleEnum<FlexDirection>((FlexDirection)styleInt.value, styleInt.keyword);
		}
		set
		{
			if (SetStyleValue(StylePropertyId.FlexDirection, value))
			{
				ve.IncrementVersion(VersionChangeType.Layout | VersionChangeType.Styles);
				ve.layoutNode.FlexDirection = (LayoutFlexDirection)ve.computedStyle.flexDirection;
			}
		}
	}

	StyleFloat IStyle.flexGrow
	{
		get
		{
			return GetStyleFloat(StylePropertyId.FlexGrow);
		}
		set
		{
			if (SetStyleValue(StylePropertyId.FlexGrow, value))
			{
				ve.IncrementVersion(VersionChangeType.Layout | VersionChangeType.Styles);
				ve.layoutNode.FlexGrow = ve.computedStyle.flexGrow;
			}
		}
	}

	StyleFloat IStyle.flexShrink
	{
		get
		{
			return GetStyleFloat(StylePropertyId.FlexShrink);
		}
		set
		{
			if (SetStyleValue(StylePropertyId.FlexShrink, value))
			{
				ve.IncrementVersion(VersionChangeType.Layout | VersionChangeType.Styles);
				ve.layoutNode.FlexShrink = ve.computedStyle.flexShrink;
			}
		}
	}

	StyleEnum<Wrap> IStyle.flexWrap
	{
		get
		{
			StyleInt styleInt = GetStyleInt(StylePropertyId.FlexWrap);
			return new StyleEnum<Wrap>((Wrap)styleInt.value, styleInt.keyword);
		}
		set
		{
			if (SetStyleValue(StylePropertyId.FlexWrap, value))
			{
				ve.IncrementVersion(VersionChangeType.Layout | VersionChangeType.Styles);
				ve.layoutNode.Wrap = (LayoutWrap)ve.computedStyle.flexWrap;
			}
		}
	}

	StyleLength IStyle.fontSize
	{
		get
		{
			return GetStyleLength(StylePropertyId.FontSize);
		}
		set
		{
			if (SetStyleValue(StylePropertyId.FontSize, value))
			{
				ve.IncrementVersion(VersionChangeType.Layout | VersionChangeType.StyleSheet | VersionChangeType.Styles | VersionChangeType.Repaint);
			}
		}
	}

	StyleLength IStyle.height
	{
		get
		{
			return GetStyleLength(StylePropertyId.Height);
		}
		set
		{
			if (SetStyleValue(StylePropertyId.Height, value))
			{
				ve.IncrementVersion(VersionChangeType.Layout | VersionChangeType.Styles);
				ve.layoutNode.Height = ve.computedStyle.height.ToLayoutValue();
			}
		}
	}

	StyleEnum<Justify> IStyle.justifyContent
	{
		get
		{
			StyleInt styleInt = GetStyleInt(StylePropertyId.JustifyContent);
			return new StyleEnum<Justify>((Justify)styleInt.value, styleInt.keyword);
		}
		set
		{
			if (SetStyleValue(StylePropertyId.JustifyContent, value))
			{
				ve.IncrementVersion(VersionChangeType.Layout | VersionChangeType.Styles);
				ve.layoutNode.JustifyContent = (LayoutJustify)ve.computedStyle.justifyContent;
			}
		}
	}

	StyleLength IStyle.left
	{
		get
		{
			return GetStyleLength(StylePropertyId.Left);
		}
		set
		{
			if (SetStyleValue(StylePropertyId.Left, value))
			{
				ve.IncrementVersion(VersionChangeType.Layout | VersionChangeType.Styles);
				ve.layoutNode.Left = ve.computedStyle.left.ToLayoutValue();
			}
		}
	}

	StyleLength IStyle.letterSpacing
	{
		get
		{
			return GetStyleLength(StylePropertyId.LetterSpacing);
		}
		set
		{
			if (SetStyleValue(StylePropertyId.LetterSpacing, value))
			{
				ve.IncrementVersion(VersionChangeType.Layout | VersionChangeType.StyleSheet | VersionChangeType.Styles | VersionChangeType.Repaint);
			}
		}
	}

	StyleLength IStyle.marginBottom
	{
		get
		{
			return GetStyleLength(StylePropertyId.MarginBottom);
		}
		set
		{
			if (SetStyleValue(StylePropertyId.MarginBottom, value))
			{
				ve.IncrementVersion(VersionChangeType.Layout | VersionChangeType.Styles);
				ve.layoutNode.MarginBottom = ve.computedStyle.marginBottom.ToLayoutValue();
			}
		}
	}

	StyleLength IStyle.marginLeft
	{
		get
		{
			return GetStyleLength(StylePropertyId.MarginLeft);
		}
		set
		{
			if (SetStyleValue(StylePropertyId.MarginLeft, value))
			{
				ve.IncrementVersion(VersionChangeType.Layout | VersionChangeType.Styles);
				ve.layoutNode.MarginLeft = ve.computedStyle.marginLeft.ToLayoutValue();
			}
		}
	}

	StyleLength IStyle.marginRight
	{
		get
		{
			return GetStyleLength(StylePropertyId.MarginRight);
		}
		set
		{
			if (SetStyleValue(StylePropertyId.MarginRight, value))
			{
				ve.IncrementVersion(VersionChangeType.Layout | VersionChangeType.Styles);
				ve.layoutNode.MarginRight = ve.computedStyle.marginRight.ToLayoutValue();
			}
		}
	}

	StyleLength IStyle.marginTop
	{
		get
		{
			return GetStyleLength(StylePropertyId.MarginTop);
		}
		set
		{
			if (SetStyleValue(StylePropertyId.MarginTop, value))
			{
				ve.IncrementVersion(VersionChangeType.Layout | VersionChangeType.Styles);
				ve.layoutNode.MarginTop = ve.computedStyle.marginTop.ToLayoutValue();
			}
		}
	}

	StyleLength IStyle.maxHeight
	{
		get
		{
			return GetStyleLength(StylePropertyId.MaxHeight);
		}
		set
		{
			if (SetStyleValue(StylePropertyId.MaxHeight, value))
			{
				ve.IncrementVersion(VersionChangeType.Layout | VersionChangeType.Styles);
				ve.layoutNode.MaxHeight = ve.computedStyle.maxHeight.ToLayoutValue();
			}
		}
	}

	StyleLength IStyle.maxWidth
	{
		get
		{
			return GetStyleLength(StylePropertyId.MaxWidth);
		}
		set
		{
			if (SetStyleValue(StylePropertyId.MaxWidth, value))
			{
				ve.IncrementVersion(VersionChangeType.Layout | VersionChangeType.Styles);
				ve.layoutNode.MaxWidth = ve.computedStyle.maxWidth.ToLayoutValue();
			}
		}
	}

	StyleLength IStyle.minHeight
	{
		get
		{
			return GetStyleLength(StylePropertyId.MinHeight);
		}
		set
		{
			if (SetStyleValue(StylePropertyId.MinHeight, value))
			{
				ve.IncrementVersion(VersionChangeType.Layout | VersionChangeType.Styles);
				ve.layoutNode.MinHeight = ve.computedStyle.minHeight.ToLayoutValue();
			}
		}
	}

	StyleLength IStyle.minWidth
	{
		get
		{
			return GetStyleLength(StylePropertyId.MinWidth);
		}
		set
		{
			if (SetStyleValue(StylePropertyId.MinWidth, value))
			{
				ve.IncrementVersion(VersionChangeType.Layout | VersionChangeType.Styles);
				ve.layoutNode.MinWidth = ve.computedStyle.minWidth.ToLayoutValue();
			}
		}
	}

	StyleFloat IStyle.opacity
	{
		get
		{
			return GetStyleFloat(StylePropertyId.Opacity);
		}
		set
		{
			if (SetStyleValue(StylePropertyId.Opacity, value))
			{
				ve.IncrementVersion(VersionChangeType.Styles | VersionChangeType.Opacity);
			}
		}
	}

	StyleEnum<Overflow> IStyle.overflow
	{
		get
		{
			StyleInt styleInt = GetStyleInt(StylePropertyId.Overflow);
			return new StyleEnum<Overflow>((Overflow)styleInt.value, styleInt.keyword);
		}
		set
		{
			StyleEnum<OverflowInternal> inlineValue = new StyleEnum<OverflowInternal>((OverflowInternal)value.value, value.keyword);
			if (SetStyleValue(StylePropertyId.Overflow, inlineValue))
			{
				ve.IncrementVersion(VersionChangeType.Layout | VersionChangeType.Styles | VersionChangeType.Overflow);
				ve.layoutNode.Overflow = (LayoutOverflow)ve.computedStyle.overflow;
			}
		}
	}

	StyleLength IStyle.paddingBottom
	{
		get
		{
			return GetStyleLength(StylePropertyId.PaddingBottom);
		}
		set
		{
			if (SetStyleValue(StylePropertyId.PaddingBottom, value))
			{
				ve.IncrementVersion(VersionChangeType.Layout | VersionChangeType.Styles);
				ve.layoutNode.PaddingBottom = ve.computedStyle.paddingBottom.ToLayoutValue();
			}
		}
	}

	StyleLength IStyle.paddingLeft
	{
		get
		{
			return GetStyleLength(StylePropertyId.PaddingLeft);
		}
		set
		{
			if (SetStyleValue(StylePropertyId.PaddingLeft, value))
			{
				ve.IncrementVersion(VersionChangeType.Layout | VersionChangeType.Styles);
				ve.layoutNode.PaddingLeft = ve.computedStyle.paddingLeft.ToLayoutValue();
			}
		}
	}

	StyleLength IStyle.paddingRight
	{
		get
		{
			return GetStyleLength(StylePropertyId.PaddingRight);
		}
		set
		{
			if (SetStyleValue(StylePropertyId.PaddingRight, value))
			{
				ve.IncrementVersion(VersionChangeType.Layout | VersionChangeType.Styles);
				ve.layoutNode.PaddingRight = ve.computedStyle.paddingRight.ToLayoutValue();
			}
		}
	}

	StyleLength IStyle.paddingTop
	{
		get
		{
			return GetStyleLength(StylePropertyId.PaddingTop);
		}
		set
		{
			if (SetStyleValue(StylePropertyId.PaddingTop, value))
			{
				ve.IncrementVersion(VersionChangeType.Layout | VersionChangeType.Styles);
				ve.layoutNode.PaddingTop = ve.computedStyle.paddingTop.ToLayoutValue();
			}
		}
	}

	StyleEnum<Position> IStyle.position
	{
		get
		{
			StyleInt styleInt = GetStyleInt(StylePropertyId.Position);
			return new StyleEnum<Position>((Position)styleInt.value, styleInt.keyword);
		}
		set
		{
			if (SetStyleValue(StylePropertyId.Position, value))
			{
				ve.IncrementVersion(VersionChangeType.Layout | VersionChangeType.Styles);
				ve.layoutNode.PositionType = (LayoutPositionType)ve.computedStyle.position;
			}
		}
	}

	StyleLength IStyle.right
	{
		get
		{
			return GetStyleLength(StylePropertyId.Right);
		}
		set
		{
			if (SetStyleValue(StylePropertyId.Right, value))
			{
				ve.IncrementVersion(VersionChangeType.Layout | VersionChangeType.Styles);
				ve.layoutNode.Right = ve.computedStyle.right.ToLayoutValue();
			}
		}
	}

	StyleEnum<TextOverflow> IStyle.textOverflow
	{
		get
		{
			StyleInt styleInt = GetStyleInt(StylePropertyId.TextOverflow);
			return new StyleEnum<TextOverflow>((TextOverflow)styleInt.value, styleInt.keyword);
		}
		set
		{
			if (SetStyleValue(StylePropertyId.TextOverflow, value))
			{
				ve.IncrementVersion(VersionChangeType.Layout | VersionChangeType.Styles | VersionChangeType.Repaint);
			}
		}
	}

	StyleLength IStyle.top
	{
		get
		{
			return GetStyleLength(StylePropertyId.Top);
		}
		set
		{
			if (SetStyleValue(StylePropertyId.Top, value))
			{
				ve.IncrementVersion(VersionChangeType.Layout | VersionChangeType.Styles);
				ve.layoutNode.Top = ve.computedStyle.top.ToLayoutValue();
			}
		}
	}

	StyleList<TimeValue> IStyle.transitionDelay
	{
		get
		{
			return GetStyleList<TimeValue>(StylePropertyId.TransitionDelay);
		}
		set
		{
			if (SetStyleValue(StylePropertyId.TransitionDelay, value))
			{
				ve.IncrementVersion(VersionChangeType.Styles | VersionChangeType.TransitionProperty);
			}
		}
	}

	StyleList<TimeValue> IStyle.transitionDuration
	{
		get
		{
			return GetStyleList<TimeValue>(StylePropertyId.TransitionDuration);
		}
		set
		{
			if (SetStyleValue(StylePropertyId.TransitionDuration, value))
			{
				ve.IncrementVersion(VersionChangeType.Styles | VersionChangeType.TransitionProperty);
			}
		}
	}

	StyleList<StylePropertyName> IStyle.transitionProperty
	{
		get
		{
			return GetStyleList<StylePropertyName>(StylePropertyId.TransitionProperty);
		}
		set
		{
			if (SetStyleValue(StylePropertyId.TransitionProperty, value))
			{
				ve.IncrementVersion(VersionChangeType.Styles | VersionChangeType.TransitionProperty);
			}
		}
	}

	StyleList<EasingFunction> IStyle.transitionTimingFunction
	{
		get
		{
			return GetStyleList<EasingFunction>(StylePropertyId.TransitionTimingFunction);
		}
		set
		{
			if (SetStyleValue(StylePropertyId.TransitionTimingFunction, value))
			{
				ve.IncrementVersion(VersionChangeType.Styles);
			}
		}
	}

	StyleColor IStyle.unityBackgroundImageTintColor
	{
		get
		{
			return GetStyleColor(StylePropertyId.UnityBackgroundImageTintColor);
		}
		set
		{
			if (SetStyleValue(StylePropertyId.UnityBackgroundImageTintColor, value))
			{
				ve.IncrementVersion(VersionChangeType.Styles | VersionChangeType.Color);
			}
		}
	}

	StyleEnum<EditorTextRenderingMode> IStyle.unityEditorTextRenderingMode
	{
		get
		{
			StyleInt styleInt = GetStyleInt(StylePropertyId.UnityEditorTextRenderingMode);
			return new StyleEnum<EditorTextRenderingMode>((EditorTextRenderingMode)styleInt.value, styleInt.keyword);
		}
		set
		{
			if (SetStyleValue(StylePropertyId.UnityEditorTextRenderingMode, value))
			{
				ve.IncrementVersion(VersionChangeType.Layout | VersionChangeType.StyleSheet | VersionChangeType.Styles | VersionChangeType.Repaint);
			}
		}
	}

	StyleFont IStyle.unityFont
	{
		get
		{
			return GetStyleFont(StylePropertyId.UnityFont);
		}
		set
		{
			if (SetStyleValue(StylePropertyId.UnityFont, value))
			{
				ve.IncrementVersion(VersionChangeType.Layout | VersionChangeType.StyleSheet | VersionChangeType.Styles | VersionChangeType.Repaint);
			}
		}
	}

	StyleFontDefinition IStyle.unityFontDefinition
	{
		get
		{
			return GetStyleFontDefinition(StylePropertyId.UnityFontDefinition);
		}
		set
		{
			if (SetStyleValue(StylePropertyId.UnityFontDefinition, value))
			{
				ve.IncrementVersion(VersionChangeType.Layout | VersionChangeType.StyleSheet | VersionChangeType.Styles | VersionChangeType.Repaint);
			}
		}
	}

	StyleEnum<FontStyle> IStyle.unityFontStyleAndWeight
	{
		get
		{
			StyleInt styleInt = GetStyleInt(StylePropertyId.UnityFontStyleAndWeight);
			return new StyleEnum<FontStyle>((FontStyle)styleInt.value, styleInt.keyword);
		}
		set
		{
			if (SetStyleValue(StylePropertyId.UnityFontStyleAndWeight, value))
			{
				ve.IncrementVersion(VersionChangeType.Layout | VersionChangeType.StyleSheet | VersionChangeType.Styles | VersionChangeType.Repaint);
			}
		}
	}

	StyleMaterialDefinition IStyle.unityMaterial
	{
		get
		{
			return GetStyleMaterialDefinition(StylePropertyId.UnityMaterial);
		}
		set
		{
			if (SetStyleValue(StylePropertyId.UnityMaterial, value))
			{
				ve.IncrementVersion(VersionChangeType.StyleSheet | VersionChangeType.Styles | VersionChangeType.Repaint);
			}
		}
	}

	StyleEnum<OverflowClipBox> IStyle.unityOverflowClipBox
	{
		get
		{
			StyleInt styleInt = GetStyleInt(StylePropertyId.UnityOverflowClipBox);
			return new StyleEnum<OverflowClipBox>((OverflowClipBox)styleInt.value, styleInt.keyword);
		}
		set
		{
			if (SetStyleValue(StylePropertyId.UnityOverflowClipBox, value))
			{
				ve.IncrementVersion(VersionChangeType.Styles | VersionChangeType.Repaint);
			}
		}
	}

	StyleLength IStyle.unityParagraphSpacing
	{
		get
		{
			return GetStyleLength(StylePropertyId.UnityParagraphSpacing);
		}
		set
		{
			if (SetStyleValue(StylePropertyId.UnityParagraphSpacing, value))
			{
				ve.IncrementVersion(VersionChangeType.Layout | VersionChangeType.StyleSheet | VersionChangeType.Styles | VersionChangeType.Repaint);
			}
		}
	}

	StyleInt IStyle.unitySliceBottom
	{
		get
		{
			return GetStyleInt(StylePropertyId.UnitySliceBottom);
		}
		set
		{
			if (SetStyleValue(StylePropertyId.UnitySliceBottom, value))
			{
				ve.IncrementVersion(VersionChangeType.Styles | VersionChangeType.Repaint);
			}
		}
	}

	StyleInt IStyle.unitySliceLeft
	{
		get
		{
			return GetStyleInt(StylePropertyId.UnitySliceLeft);
		}
		set
		{
			if (SetStyleValue(StylePropertyId.UnitySliceLeft, value))
			{
				ve.IncrementVersion(VersionChangeType.Styles | VersionChangeType.Repaint);
			}
		}
	}

	StyleInt IStyle.unitySliceRight
	{
		get
		{
			return GetStyleInt(StylePropertyId.UnitySliceRight);
		}
		set
		{
			if (SetStyleValue(StylePropertyId.UnitySliceRight, value))
			{
				ve.IncrementVersion(VersionChangeType.Styles | VersionChangeType.Repaint);
			}
		}
	}

	StyleFloat IStyle.unitySliceScale
	{
		get
		{
			return GetStyleFloat(StylePropertyId.UnitySliceScale);
		}
		set
		{
			if (SetStyleValue(StylePropertyId.UnitySliceScale, value))
			{
				ve.IncrementVersion(VersionChangeType.Layout | VersionChangeType.Styles | VersionChangeType.Repaint);
			}
		}
	}

	StyleInt IStyle.unitySliceTop
	{
		get
		{
			return GetStyleInt(StylePropertyId.UnitySliceTop);
		}
		set
		{
			if (SetStyleValue(StylePropertyId.UnitySliceTop, value))
			{
				ve.IncrementVersion(VersionChangeType.Styles | VersionChangeType.Repaint);
			}
		}
	}

	StyleEnum<SliceType> IStyle.unitySliceType
	{
		get
		{
			StyleInt styleInt = GetStyleInt(StylePropertyId.UnitySliceType);
			return new StyleEnum<SliceType>((SliceType)styleInt.value, styleInt.keyword);
		}
		set
		{
			if (SetStyleValue(StylePropertyId.UnitySliceType, value))
			{
				ve.IncrementVersion(VersionChangeType.Layout | VersionChangeType.Styles | VersionChangeType.Repaint);
			}
		}
	}

	StyleEnum<TextAnchor> IStyle.unityTextAlign
	{
		get
		{
			StyleInt styleInt = GetStyleInt(StylePropertyId.UnityTextAlign);
			return new StyleEnum<TextAnchor>((TextAnchor)styleInt.value, styleInt.keyword);
		}
		set
		{
			if (SetStyleValue(StylePropertyId.UnityTextAlign, value))
			{
				ve.IncrementVersion(VersionChangeType.StyleSheet | VersionChangeType.Styles | VersionChangeType.Repaint);
			}
		}
	}

	StyleEnum<TextGeneratorType> IStyle.unityTextGenerator
	{
		get
		{
			StyleInt styleInt = GetStyleInt(StylePropertyId.UnityTextGenerator);
			return new StyleEnum<TextGeneratorType>((TextGeneratorType)styleInt.value, styleInt.keyword);
		}
		set
		{
			if (SetStyleValue(StylePropertyId.UnityTextGenerator, value))
			{
				ve.IncrementVersion(VersionChangeType.Layout | VersionChangeType.StyleSheet | VersionChangeType.Styles | VersionChangeType.Repaint);
			}
		}
	}

	StyleColor IStyle.unityTextOutlineColor
	{
		get
		{
			return GetStyleColor(StylePropertyId.UnityTextOutlineColor);
		}
		set
		{
			if (SetStyleValue(StylePropertyId.UnityTextOutlineColor, value))
			{
				ve.IncrementVersion(VersionChangeType.StyleSheet | VersionChangeType.Styles | VersionChangeType.Repaint);
			}
		}
	}

	StyleFloat IStyle.unityTextOutlineWidth
	{
		get
		{
			return GetStyleFloat(StylePropertyId.UnityTextOutlineWidth);
		}
		set
		{
			if (SetStyleValue(StylePropertyId.UnityTextOutlineWidth, value))
			{
				ve.IncrementVersion(VersionChangeType.Layout | VersionChangeType.StyleSheet | VersionChangeType.Styles | VersionChangeType.Repaint);
			}
		}
	}

	StyleEnum<TextOverflowPosition> IStyle.unityTextOverflowPosition
	{
		get
		{
			StyleInt styleInt = GetStyleInt(StylePropertyId.UnityTextOverflowPosition);
			return new StyleEnum<TextOverflowPosition>((TextOverflowPosition)styleInt.value, styleInt.keyword);
		}
		set
		{
			if (SetStyleValue(StylePropertyId.UnityTextOverflowPosition, value))
			{
				ve.IncrementVersion(VersionChangeType.Styles | VersionChangeType.Repaint);
			}
		}
	}

	StyleEnum<Visibility> IStyle.visibility
	{
		get
		{
			StyleInt styleInt = GetStyleInt(StylePropertyId.Visibility);
			return new StyleEnum<Visibility>((Visibility)styleInt.value, styleInt.keyword);
		}
		set
		{
			if (SetStyleValue(StylePropertyId.Visibility, value))
			{
				ve.IncrementVersion(VersionChangeType.StyleSheet | VersionChangeType.Styles | VersionChangeType.Repaint | VersionChangeType.Picking);
			}
		}
	}

	StyleEnum<WhiteSpace> IStyle.whiteSpace
	{
		get
		{
			StyleInt styleInt = GetStyleInt(StylePropertyId.WhiteSpace);
			return new StyleEnum<WhiteSpace>((WhiteSpace)styleInt.value, styleInt.keyword);
		}
		set
		{
			if (SetStyleValue(StylePropertyId.WhiteSpace, value))
			{
				ve.IncrementVersion(VersionChangeType.Layout | VersionChangeType.StyleSheet | VersionChangeType.Styles | VersionChangeType.Repaint);
			}
		}
	}

	StyleLength IStyle.width
	{
		get
		{
			return GetStyleLength(StylePropertyId.Width);
		}
		set
		{
			if (SetStyleValue(StylePropertyId.Width, value))
			{
				ve.IncrementVersion(VersionChangeType.Layout | VersionChangeType.Styles);
				ve.layoutNode.Width = ve.computedStyle.width.ToLayoutValue();
			}
		}
	}

	StyleLength IStyle.wordSpacing
	{
		get
		{
			return GetStyleLength(StylePropertyId.WordSpacing);
		}
		set
		{
			if (SetStyleValue(StylePropertyId.WordSpacing, value))
			{
				ve.IncrementVersion(VersionChangeType.Layout | VersionChangeType.StyleSheet | VersionChangeType.Styles | VersionChangeType.Repaint);
			}
		}
	}

	private VisualElement ve { get; set; }

	public InlineRule inlineRule => m_InlineRule;

	StyleCursor IStyle.cursor
	{
		get
		{
			StyleCursor value = default(StyleCursor);
			if (TryGetInlineCursor(ref value))
			{
				return value;
			}
			return StyleKeyword.Null;
		}
		set
		{
			if (SetInlineCursor(value))
			{
				ve.IncrementVersion(VersionChangeType.Styles);
			}
		}
	}

	StyleTextShadow IStyle.textShadow
	{
		get
		{
			StyleTextShadow value = default(StyleTextShadow);
			if (TryGetInlineTextShadow(ref value))
			{
				return value;
			}
			return StyleKeyword.Null;
		}
		set
		{
			if (SetInlineTextShadow(value))
			{
				ve.IncrementVersion(VersionChangeType.Layout | VersionChangeType.Styles | VersionChangeType.Repaint);
			}
		}
	}

	StyleTextAutoSize IStyle.unityTextAutoSize
	{
		get
		{
			StyleTextAutoSize value = default(StyleTextAutoSize);
			if (TryGetInlineTextAutoSize(ref value))
			{
				return value;
			}
			return StyleKeyword.Null;
		}
		set
		{
			if (SetInlineTextAutoSize(value))
			{
				ve.IncrementVersion(VersionChangeType.Layout | VersionChangeType.Styles | VersionChangeType.Repaint);
			}
		}
	}

	StyleBackgroundSize IStyle.backgroundSize
	{
		get
		{
			StyleBackgroundSize value = default(StyleBackgroundSize);
			if (TryGetInlineBackgroundSize(ref value))
			{
				return value;
			}
			return StyleKeyword.Null;
		}
		set
		{
			if (SetInlineBackgroundSize(value))
			{
				ve.IncrementVersion(VersionChangeType.Styles | VersionChangeType.Repaint);
			}
		}
	}

	StyleList<FilterFunction> IStyle.filter
	{
		get
		{
			StyleList<FilterFunction> value = default(StyleList<FilterFunction>);
			if (TryGetInlineFilter(ref value))
			{
				return value;
			}
			return StyleKeyword.Null;
		}
		set
		{
			if (SetInlineFilter(value))
			{
				ve.IncrementVersion(VersionChangeType.Styles | VersionChangeType.Repaint);
			}
		}
	}

	StyleTransformOrigin IStyle.transformOrigin
	{
		get
		{
			StyleTransformOrigin value = default(StyleTransformOrigin);
			if (TryGetInlineTransformOrigin(ref value))
			{
				return value;
			}
			return StyleKeyword.Null;
		}
		set
		{
			if (SetInlineTransformOrigin(value))
			{
				ve.IncrementVersion(VersionChangeType.Styles | VersionChangeType.Transform);
			}
		}
	}

	StyleTranslate IStyle.translate
	{
		get
		{
			StyleTranslate value = default(StyleTranslate);
			if (TryGetInlineTranslate(ref value))
			{
				return value;
			}
			return StyleKeyword.Null;
		}
		set
		{
			if (SetInlineTranslate(value))
			{
				ve.IncrementVersion(VersionChangeType.Styles | VersionChangeType.Transform);
			}
		}
	}

	StyleRotate IStyle.rotate
	{
		get
		{
			StyleRotate value = default(StyleRotate);
			if (TryGetInlineRotate(ref value))
			{
				return value;
			}
			return StyleKeyword.Null;
		}
		set
		{
			if (SetInlineRotate(value))
			{
				ve.IncrementVersion(VersionChangeType.Styles | VersionChangeType.Transform);
			}
		}
	}

	StyleScale IStyle.scale
	{
		get
		{
			StyleScale value = default(StyleScale);
			if (TryGetInlineScale(ref value))
			{
				return value;
			}
			return StyleKeyword.Null;
		}
		set
		{
			if (SetInlineScale(value))
			{
				ve.IncrementVersion(VersionChangeType.Styles | VersionChangeType.Transform);
			}
		}
	}

	StyleEnum<ScaleMode> IStyle.unityBackgroundScaleMode
	{
		get
		{
			bool valid;
			return new StyleEnum<ScaleMode>(BackgroundPropertyHelper.ResolveUnityBackgroundScaleMode(ve.style.backgroundPositionX.value, ve.style.backgroundPositionY.value, ve.style.backgroundRepeat.value, ve.style.backgroundSize.value, out valid));
		}
		set
		{
			ve.style.backgroundPositionX = BackgroundPropertyHelper.ConvertScaleModeToBackgroundPosition(value.value);
			ve.style.backgroundPositionY = BackgroundPropertyHelper.ConvertScaleModeToBackgroundPosition(value.value);
			ve.style.backgroundRepeat = BackgroundPropertyHelper.ConvertScaleModeToBackgroundRepeat(value.value);
			ve.style.backgroundSize = BackgroundPropertyHelper.ConvertScaleModeToBackgroundSize(value.value);
		}
	}

	public InlineStyleAccess(VisualElement ve)
	{
		this.ve = ve;
	}

	~InlineStyleAccess()
	{
		StyleValue value = default(StyleValue);
		if (TryGetStyleValue(StylePropertyId.BackgroundImage, ref value) && value.resource.IsAllocated)
		{
			value.resource.Free();
		}
		if (TryGetStyleValue(StylePropertyId.UnityFont, ref value) && value.resource.IsAllocated)
		{
			value.resource.Free();
		}
	}

	public void SetInlineRule(StyleSheet sheet, StyleRule rule)
	{
		m_InlineRule.sheet = sheet;
		m_InlineRule.rule = rule;
		ApplyInlineStyles(ref ve.computedStyle);
	}

	public bool IsValueSet(StylePropertyId id)
	{
		foreach (StyleValue value in m_Values)
		{
			if (value.id == id)
			{
				return true;
			}
		}
		if (m_ValuesManaged != null)
		{
			foreach (StyleValueManaged item in m_ValuesManaged)
			{
				if (item.id == id)
				{
					return true;
				}
			}
		}
		return id switch
		{
			StylePropertyId.Cursor => m_HasInlineCursor, 
			StylePropertyId.TextShadow => m_HasInlineTextShadow, 
			StylePropertyId.UnityTextAutoSize => m_HasInlineTextAutoSize, 
			StylePropertyId.TransformOrigin => m_HasInlineTransformOrigin, 
			StylePropertyId.Translate => m_HasInlineTranslate, 
			StylePropertyId.Rotate => m_HasInlineRotate, 
			StylePropertyId.Scale => m_HasInlineScale, 
			StylePropertyId.BackgroundSize => m_HasInlineBackgroundSize, 
			StylePropertyId.Filter => m_HasInlineFilter, 
			_ => false, 
		};
	}

	public void ApplyInlineStyles(ref ComputedStyle computedStyle)
	{
		VisualElement parent = ve.hierarchy.parent;
		ref ComputedStyle reference;
		if (parent != null)
		{
			_ = ref parent.computedStyle;
			reference = ref parent.computedStyle;
		}
		else
		{
			reference = ref InitialStyle.Get();
		}
		ref ComputedStyle parentStyle = ref reference;
		if (m_InlineRule.sheet != null)
		{
			s_StylePropertyReader.SetInlineContext(m_InlineRule.sheet, m_InlineRule.rule.properties);
			computedStyle.ApplyProperties(s_StylePropertyReader, ref parentStyle);
		}
		foreach (StyleValue value in m_Values)
		{
			computedStyle.ApplyStyleValue(value, ref parentStyle);
		}
		if (m_ValuesManaged != null)
		{
			foreach (StyleValueManaged item in m_ValuesManaged)
			{
				computedStyle.ApplyStyleValueManaged(item, ref parentStyle);
			}
		}
		if (ve.style.cursor.keyword != StyleKeyword.Null)
		{
			computedStyle.ApplyStyleCursor(ve.style.cursor.value);
		}
		if (ve.style.textShadow.keyword != StyleKeyword.Null)
		{
			computedStyle.ApplyStyleTextShadow(ve.style.textShadow.value);
		}
		if (ve.style.unityTextAutoSize.keyword != StyleKeyword.Null)
		{
			computedStyle.ApplyStyleTextAutoSize(ve.style.unityTextAutoSize.value);
		}
		if (m_HasInlineTransformOrigin)
		{
			computedStyle.ApplyStyleTransformOrigin(ve.style.transformOrigin.value);
		}
		if (m_HasInlineTranslate)
		{
			computedStyle.ApplyStyleTranslate(ve.style.translate.value);
		}
		if (m_HasInlineScale)
		{
			computedStyle.ApplyStyleScale(ve.style.scale.value);
		}
		if (m_HasInlineRotate)
		{
			computedStyle.ApplyStyleRotate(ve.style.rotate.value);
		}
		if (m_HasInlineBackgroundSize)
		{
			computedStyle.ApplyStyleBackgroundSize(ve.style.backgroundSize.value);
		}
		if (m_HasInlineFilter)
		{
			computedStyle.ApplyStyleFilter(ve.style.filter.value);
		}
	}

	private StyleList<T> GetStyleList<T>(StylePropertyId id)
	{
		StyleValueManaged value = default(StyleValueManaged);
		if (TryGetStyleValueManaged(id, ref value))
		{
			return new StyleList<T>(value.value as List<T>, value.keyword);
		}
		return StyleKeyword.Null;
	}

	private void SetStyleValueManaged(StyleValueManaged value)
	{
		if (m_ValuesManaged == null)
		{
			m_ValuesManaged = new List<StyleValueManaged>();
		}
		for (int i = 0; i < m_ValuesManaged.Count; i++)
		{
			if (m_ValuesManaged[i].id == value.id)
			{
				if (value.keyword == StyleKeyword.Null)
				{
					m_ValuesManaged.RemoveAt(i);
				}
				else
				{
					m_ValuesManaged[i] = value;
				}
				return;
			}
		}
		m_ValuesManaged.Add(value);
	}

	private bool TryGetStyleValueManaged(StylePropertyId id, ref StyleValueManaged value)
	{
		value.id = StylePropertyId.Unknown;
		if (m_ValuesManaged == null)
		{
			return false;
		}
		foreach (StyleValueManaged item in m_ValuesManaged)
		{
			if (item.id == id)
			{
				value = item;
				return true;
			}
		}
		return false;
	}

	private bool SetStyleValue(StylePropertyId id, StyleBackgroundPosition inlineValue)
	{
		StyleValue value = default(StyleValue);
		if (TryGetStyleValue(id, ref value))
		{
			if (value.position == inlineValue.value && value.keyword == inlineValue.keyword)
			{
				return false;
			}
		}
		else if (inlineValue.keyword == StyleKeyword.Null)
		{
			return false;
		}
		value.id = id;
		value.keyword = inlineValue.keyword;
		value.position = inlineValue.value;
		SetStyleValue(value);
		if (inlineValue.keyword == StyleKeyword.Null)
		{
			return RemoveInlineStyle(id);
		}
		ApplyStyleValue(value);
		return true;
	}

	private bool SetStyleValue(StylePropertyId id, StyleBackgroundRepeat inlineValue)
	{
		StyleValue value = default(StyleValue);
		if (TryGetStyleValue(id, ref value))
		{
			if (value.repeat == inlineValue.value && value.keyword == inlineValue.keyword)
			{
				return false;
			}
		}
		else if (inlineValue.keyword == StyleKeyword.Null)
		{
			return false;
		}
		value.id = id;
		value.keyword = inlineValue.keyword;
		value.repeat = inlineValue.value;
		SetStyleValue(value);
		if (inlineValue.keyword == StyleKeyword.Null)
		{
			return RemoveInlineStyle(id);
		}
		ApplyStyleValue(value);
		return true;
	}

	private bool SetStyleValue(StylePropertyId id, StyleLength inlineValue)
	{
		StyleValue value = default(StyleValue);
		if (TryGetStyleValue(id, ref value))
		{
			if (value.length == inlineValue.ToLength() && value.keyword == inlineValue.keyword)
			{
				return false;
			}
		}
		else if (inlineValue.keyword == StyleKeyword.Null)
		{
			return false;
		}
		value.id = id;
		value.keyword = inlineValue.keyword;
		value.length = inlineValue.ToLength();
		SetStyleValue(value);
		if (inlineValue.keyword == StyleKeyword.Null)
		{
			return RemoveInlineStyle(id);
		}
		ApplyStyleValue(value);
		return true;
	}

	private bool SetStyleValue(StylePropertyId id, StyleFloat inlineValue)
	{
		StyleValue value = default(StyleValue);
		if (TryGetStyleValue(id, ref value))
		{
			if (value.number == inlineValue.value && value.keyword == inlineValue.keyword)
			{
				return false;
			}
		}
		else if (inlineValue.keyword == StyleKeyword.Null)
		{
			return false;
		}
		value.id = id;
		value.keyword = inlineValue.keyword;
		value.number = inlineValue.value;
		SetStyleValue(value);
		if (inlineValue.keyword == StyleKeyword.Null)
		{
			return RemoveInlineStyle(id);
		}
		ApplyStyleValue(value);
		return true;
	}

	private bool SetStyleValue(StylePropertyId id, StyleInt inlineValue)
	{
		StyleValue value = default(StyleValue);
		if (TryGetStyleValue(id, ref value))
		{
			if (value.number == (float)inlineValue.value && value.keyword == inlineValue.keyword)
			{
				return false;
			}
		}
		else if (inlineValue.keyword == StyleKeyword.Null)
		{
			return false;
		}
		value.id = id;
		value.keyword = inlineValue.keyword;
		value.number = inlineValue.value;
		SetStyleValue(value);
		if (inlineValue.keyword == StyleKeyword.Null)
		{
			return RemoveInlineStyle(id);
		}
		ApplyStyleValue(value);
		return true;
	}

	private bool SetStyleValue(StylePropertyId id, StyleColor inlineValue)
	{
		StyleValue value = default(StyleValue);
		if (TryGetStyleValue(id, ref value))
		{
			if (value.color == inlineValue.value && value.keyword == inlineValue.keyword)
			{
				return false;
			}
		}
		else if (inlineValue.keyword == StyleKeyword.Null)
		{
			return false;
		}
		value.id = id;
		value.keyword = inlineValue.keyword;
		value.color = inlineValue.value;
		SetStyleValue(value);
		if (inlineValue.keyword == StyleKeyword.Null)
		{
			return RemoveInlineStyle(id);
		}
		ApplyStyleValue(value);
		return true;
	}

	private bool SetStyleValue<T>(StylePropertyId id, StyleEnum<T> inlineValue) where T : struct, IConvertible
	{
		StyleValue value = default(StyleValue);
		int num = UnsafeUtility.EnumToInt(inlineValue.value);
		if (TryGetStyleValue(id, ref value))
		{
			if (value.number == (float)num && value.keyword == inlineValue.keyword)
			{
				return false;
			}
		}
		else if (inlineValue.keyword == StyleKeyword.Null)
		{
			return false;
		}
		value.id = id;
		value.keyword = inlineValue.keyword;
		value.number = num;
		SetStyleValue(value);
		if (inlineValue.keyword == StyleKeyword.Null)
		{
			return RemoveInlineStyle(id);
		}
		ApplyStyleValue(value);
		return true;
	}

	private bool SetStyleValue(StylePropertyId id, StyleBackground inlineValue)
	{
		StyleValue value = default(StyleValue);
		if (TryGetStyleValue(id, ref value))
		{
			VectorImage vectorImage = (value.resource.IsAllocated ? (value.resource.Target as VectorImage) : null);
			Sprite sprite = (value.resource.IsAllocated ? (value.resource.Target as Sprite) : null);
			Texture2D texture2D = (value.resource.IsAllocated ? (value.resource.Target as Texture2D) : null);
			RenderTexture renderTexture = (value.resource.IsAllocated ? (value.resource.Target as RenderTexture) : null);
			if (vectorImage == inlineValue.value.vectorImage && texture2D == inlineValue.value.texture && sprite == inlineValue.value.sprite && renderTexture == inlineValue.value.renderTexture && value.keyword == inlineValue.keyword)
			{
				return false;
			}
			if (value.resource.IsAllocated)
			{
				value.resource.Free();
			}
		}
		else if (inlineValue.keyword == StyleKeyword.Null)
		{
			return false;
		}
		value.id = id;
		value.keyword = inlineValue.keyword;
		if (inlineValue.value.vectorImage != null)
		{
			value.resource = GCHandle.Alloc(inlineValue.value.vectorImage);
		}
		else if (inlineValue.value.sprite != null)
		{
			value.resource = GCHandle.Alloc(inlineValue.value.sprite);
		}
		else if (inlineValue.value.texture != null)
		{
			value.resource = GCHandle.Alloc(inlineValue.value.texture);
		}
		else if (inlineValue.value.renderTexture != null)
		{
			value.resource = GCHandle.Alloc(inlineValue.value.renderTexture);
		}
		else
		{
			value.resource = default(GCHandle);
		}
		SetStyleValue(value);
		if (inlineValue.keyword == StyleKeyword.Null)
		{
			return RemoveInlineStyle(id);
		}
		ApplyStyleValue(value);
		return true;
	}

	private bool SetStyleValue(StylePropertyId id, StyleFontDefinition inlineValue)
	{
		StyleValue value = default(StyleValue);
		if (TryGetStyleValue(id, ref value))
		{
			Font font = (value.resource.IsAllocated ? (value.resource.Target as Font) : null);
			FontAsset fontAsset = (value.resource.IsAllocated ? (value.resource.Target as FontAsset) : null);
			if (font == inlineValue.value.font && fontAsset == inlineValue.value.fontAsset && value.keyword == inlineValue.keyword)
			{
				return false;
			}
			if (value.resource.IsAllocated)
			{
				value.resource.Free();
			}
		}
		else if (inlineValue.keyword == StyleKeyword.Null)
		{
			return false;
		}
		value.id = id;
		value.keyword = inlineValue.keyword;
		if (inlineValue.value.font != null)
		{
			value.resource = GCHandle.Alloc(inlineValue.value.font);
		}
		else if (inlineValue.value.fontAsset != null)
		{
			value.resource = GCHandle.Alloc(inlineValue.value.fontAsset);
		}
		else
		{
			value.resource = default(GCHandle);
		}
		SetStyleValue(value);
		if (inlineValue.keyword == StyleKeyword.Null)
		{
			return RemoveInlineStyle(id);
		}
		ApplyStyleValue(value);
		return true;
	}

	private bool SetStyleValue(StylePropertyId id, StyleFont inlineValue)
	{
		StyleValue value = default(StyleValue);
		if (TryGetStyleValue(id, ref value))
		{
			Font font = (value.resource.IsAllocated ? (value.resource.Target as Font) : null);
			if (font == inlineValue.value && value.keyword == inlineValue.keyword)
			{
				return false;
			}
			if (value.resource.IsAllocated)
			{
				value.resource.Free();
			}
		}
		else if (inlineValue.keyword == StyleKeyword.Null)
		{
			return false;
		}
		value.id = id;
		value.keyword = inlineValue.keyword;
		value.resource = ((inlineValue.value != null) ? GCHandle.Alloc(inlineValue.value) : default(GCHandle));
		SetStyleValue(value);
		if (inlineValue.keyword == StyleKeyword.Null)
		{
			return RemoveInlineStyle(id);
		}
		ApplyStyleValue(value);
		return true;
	}

	private bool SetStyleValue(StylePropertyId id, StyleMaterialDefinition inlineValue)
	{
		StyleValue value = default(StyleValue);
		if (TryGetStyleValue(id, ref value))
		{
			Material material = (value.resource.IsAllocated ? (value.resource.Target as Material) : null);
			if (material == inlineValue.value && value.keyword == inlineValue.keyword)
			{
				return false;
			}
			if (value.resource.IsAllocated)
			{
				value.resource.Free();
			}
		}
		else if (inlineValue.keyword == StyleKeyword.Null)
		{
			return false;
		}
		value.id = id;
		value.keyword = inlineValue.keyword;
		value.resource = ((inlineValue.value != null) ? GCHandle.Alloc(inlineValue.value) : default(GCHandle));
		SetStyleValue(value);
		if (inlineValue.keyword == StyleKeyword.Null)
		{
			return RemoveInlineStyle(id);
		}
		ApplyStyleValue(value);
		return true;
	}

	private bool SetStyleValue<T>(StylePropertyId id, StyleList<T> inlineValue)
	{
		StyleValueManaged value = default(StyleValueManaged);
		if (TryGetStyleValueManaged(id, ref value))
		{
			if (value.keyword == inlineValue.keyword)
			{
				if (value.value == null && inlineValue.value == null)
				{
					return false;
				}
				if (value.value is List<T> first && inlineValue.value != null && first.SequenceEqual(inlineValue.value))
				{
					return false;
				}
			}
		}
		else if (inlineValue.keyword == StyleKeyword.Null)
		{
			return false;
		}
		value.id = id;
		value.keyword = inlineValue.keyword;
		if (inlineValue.value != null)
		{
			if (value.value == null)
			{
				value.value = new List<T>(inlineValue.value);
			}
			else
			{
				List<T> list = (List<T>)value.value;
				list.Clear();
				list.AddRange(inlineValue.value);
			}
		}
		else
		{
			value.value = null;
		}
		SetStyleValueManaged(value);
		if (inlineValue.keyword == StyleKeyword.Null)
		{
			return RemoveInlineStyle(id);
		}
		ApplyStyleValue(value);
		return true;
	}

	private bool SetStyleValue(StylePropertyId id, StyleRatio inlineValue)
	{
		StyleValue value = default(StyleValue);
		if (TryGetStyleValue(id, ref value))
		{
			if (value.number == inlineValue.value && value.keyword == inlineValue.keyword)
			{
				return false;
			}
		}
		else if (inlineValue.keyword == StyleKeyword.Null)
		{
			return false;
		}
		value.id = id;
		value.keyword = inlineValue.keyword;
		value.number = inlineValue.value;
		SetStyleValue(value);
		if (inlineValue.keyword == StyleKeyword.Null)
		{
			return RemoveInlineStyle(id);
		}
		ApplyStyleValue(value);
		return true;
	}

	private bool SetInlineCursor(StyleCursor inlineValue)
	{
		StyleCursor value = default(StyleCursor);
		if (TryGetInlineCursor(ref value))
		{
			if (value.value == inlineValue.value && value.keyword == inlineValue.keyword)
			{
				return false;
			}
		}
		else if (inlineValue.keyword == StyleKeyword.Null)
		{
			return false;
		}
		value.value = inlineValue.value;
		value.keyword = inlineValue.keyword;
		if (inlineValue.keyword == StyleKeyword.Null)
		{
			m_HasInlineCursor = false;
			return RemoveInlineStyle(StylePropertyId.Cursor);
		}
		m_InlineCursor = value;
		m_HasInlineCursor = true;
		ApplyStyleCursor(value);
		return true;
	}

	private void ApplyStyleCursor(StyleCursor cursor)
	{
		ve.computedStyle.ApplyStyleCursor(cursor.value);
		if (ve.elementPanel?.GetTopElementUnderPointer(PointerId.mousePointerId) == ve)
		{
			ve.elementPanel.cursorManager.SetCursor(cursor.value);
		}
	}

	private bool SetInlineTextShadow(StyleTextShadow inlineValue)
	{
		StyleTextShadow value = default(StyleTextShadow);
		if (TryGetInlineTextShadow(ref value))
		{
			if (value.value == inlineValue.value && value.keyword == inlineValue.keyword)
			{
				return false;
			}
		}
		else if (inlineValue.keyword == StyleKeyword.Null)
		{
			return false;
		}
		value.value = inlineValue.value;
		value.keyword = inlineValue.keyword;
		if (inlineValue.keyword == StyleKeyword.Null)
		{
			m_HasInlineTextShadow = false;
			return RemoveInlineStyle(StylePropertyId.TextShadow);
		}
		m_InlineTextShadow = value;
		m_HasInlineTextShadow = true;
		ApplyStyleTextShadow(value);
		return true;
	}

	private void ApplyStyleTextShadow(StyleTextShadow textShadow)
	{
		ComputedTransitionUtils.UpdateComputedTransitions(ref ve.computedStyle);
		bool flag = false;
		if (ve.computedStyle.hasTransition && ve.styleInitialized && ve.computedStyle.GetTransitionProperty(StylePropertyId.TextShadow, out var result))
		{
			flag = ComputedStyle.StartAnimationInlineTextShadow(ve, ref ve.computedStyle, textShadow, result.durationMs, result.delayMs, result.easingCurve);
		}
		else
		{
			ve.styleAnimation.CancelAnimation(StylePropertyId.TextShadow);
		}
		if (!flag)
		{
			ve.computedStyle.ApplyStyleTextShadow(textShadow.value);
		}
	}

	private bool SetInlineTextAutoSize(StyleTextAutoSize inlineValue)
	{
		StyleTextAutoSize value = default(StyleTextAutoSize);
		if (TryGetInlineTextAutoSize(ref value))
		{
			if (value.value == inlineValue.value && value.keyword == inlineValue.keyword)
			{
				return false;
			}
		}
		else if (inlineValue.keyword == StyleKeyword.Null)
		{
			return false;
		}
		value.value = inlineValue.value;
		value.keyword = inlineValue.keyword;
		if (inlineValue.keyword == StyleKeyword.Null)
		{
			m_HasInlineTextAutoSize = false;
			return RemoveInlineStyle(StylePropertyId.UnityTextAutoSize);
		}
		m_InlineTextAutoSize = value;
		m_HasInlineTextAutoSize = true;
		ApplyStyleTextAutoSize(value);
		return true;
	}

	private void ApplyStyleTextAutoSize(StyleTextAutoSize textAutoSize)
	{
		ve.computedStyle.ApplyStyleTextAutoSize(textAutoSize.value);
	}

	private bool SetInlineTransformOrigin(StyleTransformOrigin inlineValue)
	{
		StyleTransformOrigin value = default(StyleTransformOrigin);
		if (TryGetInlineTransformOrigin(ref value))
		{
			if (value.value == inlineValue.value && value.keyword == inlineValue.keyword)
			{
				return false;
			}
		}
		else if (inlineValue.keyword == StyleKeyword.Null)
		{
			return false;
		}
		if (inlineValue.keyword == StyleKeyword.Null)
		{
			m_HasInlineTransformOrigin = false;
			return RemoveInlineStyle(StylePropertyId.TransformOrigin);
		}
		m_InlineTransformOrigin = inlineValue;
		m_HasInlineTransformOrigin = true;
		ApplyStyleTransformOrigin(inlineValue);
		return true;
	}

	private void ApplyStyleTransformOrigin(StyleTransformOrigin transformOrigin)
	{
		ComputedTransitionUtils.UpdateComputedTransitions(ref ve.computedStyle);
		bool flag = false;
		if (ve.computedStyle.hasTransition && ve.styleInitialized && ve.computedStyle.GetTransitionProperty(StylePropertyId.TransformOrigin, out var result))
		{
			flag = ComputedStyle.StartAnimationInlineTransformOrigin(ve, ref ve.computedStyle, transformOrigin, result.durationMs, result.delayMs, result.easingCurve);
		}
		else
		{
			ve.styleAnimation.CancelAnimation(StylePropertyId.TransformOrigin);
		}
		if (!flag)
		{
			ve.computedStyle.ApplyStyleTransformOrigin(transformOrigin.value);
		}
	}

	private bool SetInlineTranslate(StyleTranslate inlineValue)
	{
		StyleTranslate value = default(StyleTranslate);
		if (TryGetInlineTranslate(ref value))
		{
			if (value.value == inlineValue.value && value.keyword == inlineValue.keyword)
			{
				return false;
			}
		}
		else if (inlineValue.keyword == StyleKeyword.Null)
		{
			return false;
		}
		if (inlineValue.keyword == StyleKeyword.Null)
		{
			m_HasInlineTranslate = false;
			return RemoveInlineStyle(StylePropertyId.Translate);
		}
		m_InlineTranslateOperation = inlineValue;
		m_HasInlineTranslate = true;
		ApplyStyleTranslate(inlineValue);
		return true;
	}

	private void ApplyStyleTranslate(StyleTranslate translate)
	{
		ComputedTransitionUtils.UpdateComputedTransitions(ref ve.computedStyle);
		bool flag = false;
		if (ve.computedStyle.hasTransition && ve.styleInitialized && ve.computedStyle.GetTransitionProperty(StylePropertyId.Translate, out var result))
		{
			flag = ComputedStyle.StartAnimationInlineTranslate(ve, ref ve.computedStyle, translate, result.durationMs, result.delayMs, result.easingCurve);
		}
		else
		{
			ve.styleAnimation.CancelAnimation(StylePropertyId.Translate);
		}
		if (!flag)
		{
			ve.computedStyle.ApplyStyleTranslate(translate.value);
		}
	}

	private bool SetInlineScale(StyleScale inlineValue)
	{
		StyleScale value = default(StyleScale);
		if (TryGetInlineScale(ref value))
		{
			if (value.value == inlineValue.value && value.keyword == inlineValue.keyword)
			{
				return false;
			}
		}
		else if (inlineValue.keyword == StyleKeyword.Null)
		{
			return false;
		}
		if (inlineValue.keyword == StyleKeyword.Null)
		{
			m_HasInlineScale = false;
			return RemoveInlineStyle(StylePropertyId.Scale);
		}
		m_InlineScale = inlineValue;
		m_HasInlineScale = true;
		ApplyStyleScale(inlineValue);
		return true;
	}

	private void ApplyStyleScale(StyleScale scale)
	{
		ComputedTransitionUtils.UpdateComputedTransitions(ref ve.computedStyle);
		bool flag = false;
		if (ve.computedStyle.hasTransition && ve.styleInitialized && ve.computedStyle.GetTransitionProperty(StylePropertyId.Scale, out var result))
		{
			flag = ComputedStyle.StartAnimationInlineScale(ve, ref ve.computedStyle, scale, result.durationMs, result.delayMs, result.easingCurve);
		}
		else
		{
			ve.styleAnimation.CancelAnimation(StylePropertyId.Scale);
		}
		if (!flag)
		{
			ve.computedStyle.ApplyStyleScale(scale.value);
		}
	}

	private bool SetInlineRotate(StyleRotate inlineValue)
	{
		StyleRotate value = default(StyleRotate);
		if (TryGetInlineRotate(ref value))
		{
			if (value.value == inlineValue.value && value.keyword == inlineValue.keyword)
			{
				return false;
			}
		}
		else if (inlineValue.keyword == StyleKeyword.Null)
		{
			return false;
		}
		if (inlineValue.keyword == StyleKeyword.Null)
		{
			m_HasInlineRotate = false;
			return RemoveInlineStyle(StylePropertyId.Rotate);
		}
		m_InlineRotateOperation = inlineValue;
		m_HasInlineRotate = true;
		ApplyStyleRotate(inlineValue);
		return true;
	}

	private void ApplyStyleRotate(StyleRotate rotate)
	{
		VisualElement parent = ve.hierarchy.parent;
		ref ComputedStyle reference;
		if (parent != null)
		{
			_ = ref parent.computedStyle;
			reference = ref parent.computedStyle;
		}
		else
		{
			reference = ref InitialStyle.Get();
		}
		ref ComputedStyle reference2 = ref reference;
		ComputedTransitionUtils.UpdateComputedTransitions(ref ve.computedStyle);
		bool flag = false;
		if (ve.computedStyle.hasTransition && ve.styleInitialized && ve.computedStyle.GetTransitionProperty(StylePropertyId.Rotate, out var result))
		{
			flag = ComputedStyle.StartAnimationInlineRotate(ve, ref ve.computedStyle, rotate, result.durationMs, result.delayMs, result.easingCurve);
		}
		else
		{
			ve.styleAnimation.CancelAnimation(StylePropertyId.Rotate);
		}
		if (!flag)
		{
			ve.computedStyle.ApplyStyleRotate(rotate.value);
		}
	}

	private bool SetInlineBackgroundSize(StyleBackgroundSize inlineValue)
	{
		StyleBackgroundSize value = default(StyleBackgroundSize);
		if (TryGetInlineBackgroundSize(ref value))
		{
			if (value.value == inlineValue.value && value.keyword == inlineValue.keyword)
			{
				return false;
			}
		}
		else if (inlineValue.keyword == StyleKeyword.Null)
		{
			return false;
		}
		if (inlineValue.keyword == StyleKeyword.Null)
		{
			m_HasInlineBackgroundSize = false;
			return RemoveInlineStyle(StylePropertyId.BackgroundSize);
		}
		m_InlineBackgroundSize = inlineValue;
		m_HasInlineBackgroundSize = true;
		ApplyStyleBackgroundSize(inlineValue);
		return true;
	}

	private void ApplyStyleBackgroundSize(StyleBackgroundSize backgroundSize)
	{
		ComputedTransitionUtils.UpdateComputedTransitions(ref ve.computedStyle);
		bool flag = false;
		if (ve.computedStyle.hasTransition && ve.styleInitialized && ve.computedStyle.GetTransitionProperty(StylePropertyId.BackgroundSize, out var result))
		{
			flag = ComputedStyle.StartAnimationInlineBackgroundSize(ve, ref ve.computedStyle, backgroundSize, result.durationMs, result.delayMs, result.easingCurve);
		}
		else
		{
			ve.styleAnimation.CancelAnimation(StylePropertyId.TransformOrigin);
		}
		if (!flag)
		{
			ve.computedStyle.ApplyStyleBackgroundSize(backgroundSize.value);
		}
	}

	private bool SetInlineFilter(StyleList<FilterFunction> inlineValue)
	{
		StyleList<FilterFunction> value = default(StyleList<FilterFunction>);
		if (TryGetInlineFilter(ref value))
		{
			if (value.value == inlineValue.value && value.keyword == inlineValue.keyword)
			{
				return false;
			}
		}
		else if (inlineValue.keyword == StyleKeyword.Null)
		{
			return false;
		}
		if (inlineValue.keyword == StyleKeyword.Null)
		{
			m_HasInlineBackgroundSize = false;
			return RemoveInlineStyle(StylePropertyId.Filter);
		}
		m_InlineFilter = inlineValue;
		m_HasInlineFilter = true;
		ApplyStyleFilter(inlineValue);
		return true;
	}

	private void ApplyStyleFilter(StyleList<FilterFunction> filter)
	{
		ComputedTransitionUtils.UpdateComputedTransitions(ref ve.computedStyle);
		bool flag = false;
		if (ve.computedStyle.hasTransition && ve.styleInitialized && ve.computedStyle.GetTransitionProperty(StylePropertyId.BackgroundSize, out var result))
		{
			flag = ComputedStyle.StartAnimationInlineFilter(ve, ref ve.computedStyle, filter, result.durationMs, result.delayMs, result.easingCurve);
		}
		else
		{
			ve.styleAnimation.CancelAnimation(StylePropertyId.TransformOrigin);
		}
		if (!flag)
		{
			ve.computedStyle.ApplyStyleFilter(filter.value);
		}
	}

	private void ApplyStyleValue(StyleValue value)
	{
		VisualElement parent = ve.hierarchy.parent;
		ref ComputedStyle reference;
		if (parent != null)
		{
			_ = ref parent.computedStyle;
			reference = ref parent.computedStyle;
		}
		else
		{
			reference = ref InitialStyle.Get();
		}
		ref ComputedStyle parentStyle = ref reference;
		bool flag = false;
		if (StylePropertyUtil.IsAnimatable(value.id))
		{
			ComputedTransitionUtils.UpdateComputedTransitions(ref ve.computedStyle);
			if (ve.computedStyle.hasTransition && ve.styleInitialized && ve.computedStyle.GetTransitionProperty(value.id, out var result))
			{
				flag = ComputedStyle.StartAnimationInline(ve, value.id, ref ve.computedStyle, value, result.durationMs, result.delayMs, result.easingCurve);
			}
			else
			{
				ve.styleAnimation.CancelAnimation(value.id);
			}
		}
		if (!flag)
		{
			ve.computedStyle.ApplyStyleValue(value, ref parentStyle);
		}
	}

	private void ApplyStyleValue(StyleValueManaged value)
	{
		VisualElement parent = ve.hierarchy.parent;
		ref ComputedStyle parentStyle;
		if (parent != null)
		{
			_ = ref parent.computedStyle;
			parentStyle = ref parent.computedStyle;
		}
		else
		{
			parentStyle = ref InitialStyle.Get();
		}
		ve.computedStyle.ApplyStyleValueManaged(value, ref parentStyle);
	}

	private bool RemoveInlineStyle(StylePropertyId id)
	{
		long matchingRulesHash = ve.computedStyle.matchingRulesHash;
		if (matchingRulesHash == 0)
		{
			ApplyFromComputedStyle(id, ref InitialStyle.Get());
			return true;
		}
		if (StyleCache.TryGetValue(matchingRulesHash, out var data))
		{
			ApplyFromComputedStyle(id, ref data);
			return true;
		}
		return false;
	}

	private void ApplyFromComputedStyle(StylePropertyId id, ref ComputedStyle newStyle)
	{
		bool flag = false;
		if (StylePropertyUtil.IsAnimatable(id))
		{
			ComputedTransitionUtils.UpdateComputedTransitions(ref ve.computedStyle);
			if (ve.computedStyle.hasTransition && ve.styleInitialized && ve.computedStyle.GetTransitionProperty(id, out var result))
			{
				flag = ComputedStyle.StartAnimation(ve, id, ref ve.computedStyle, ref newStyle, result.durationMs, result.delayMs, result.easingCurve);
			}
			else
			{
				ve.styleAnimation.CancelAnimation(id);
			}
		}
		if (!flag)
		{
			ve.computedStyle.ApplyFromComputedStyle(id, ref newStyle);
		}
	}

	public bool TryGetInlineCursor(ref StyleCursor value)
	{
		if (m_HasInlineCursor)
		{
			value = m_InlineCursor;
			return true;
		}
		return false;
	}

	public bool TryGetInlineTextShadow(ref StyleTextShadow value)
	{
		if (m_HasInlineTextShadow)
		{
			value = m_InlineTextShadow;
			return true;
		}
		return false;
	}

	public bool TryGetInlineTextAutoSize(ref StyleTextAutoSize value)
	{
		if (m_HasInlineTextAutoSize)
		{
			value = m_InlineTextAutoSize;
			return true;
		}
		return false;
	}

	public bool TryGetInlineTransformOrigin(ref StyleTransformOrigin value)
	{
		if (m_HasInlineTransformOrigin)
		{
			value = m_InlineTransformOrigin;
			return true;
		}
		return false;
	}

	public bool TryGetInlineTranslate(ref StyleTranslate value)
	{
		if (m_HasInlineTranslate)
		{
			value = m_InlineTranslateOperation;
			return true;
		}
		return false;
	}

	public bool TryGetInlineRotate(ref StyleRotate value)
	{
		if (m_HasInlineRotate)
		{
			value = m_InlineRotateOperation;
			return true;
		}
		return false;
	}

	public bool TryGetInlineScale(ref StyleScale value)
	{
		if (m_HasInlineScale)
		{
			value = m_InlineScale;
			return true;
		}
		return false;
	}

	public bool TryGetInlineBackgroundSize(ref StyleBackgroundSize value)
	{
		if (m_HasInlineBackgroundSize)
		{
			value = m_InlineBackgroundSize;
			return true;
		}
		return false;
	}

	public bool TryGetInlineFilter(ref StyleList<FilterFunction> value)
	{
		if (m_HasInlineFilter)
		{
			value = m_InlineFilter;
			return true;
		}
		return false;
	}
}
