using System;
using System.Collections.Generic;

namespace UnityEngine.UIElements;

internal class ResolvedStyleAccess : IResolvedStyle
{
	public Align alignContent => ve.computedStyle.alignContent;

	public Align alignItems => ve.computedStyle.alignItems;

	public Align alignSelf => ve.computedStyle.alignSelf;

	public Color backgroundColor => ve.computedStyle.backgroundColor;

	public Background backgroundImage => ve.computedStyle.backgroundImage;

	public BackgroundPosition backgroundPositionX => ve.computedStyle.backgroundPositionX;

	public BackgroundPosition backgroundPositionY => ve.computedStyle.backgroundPositionY;

	public BackgroundRepeat backgroundRepeat => ve.computedStyle.backgroundRepeat;

	public BackgroundSize backgroundSize => ve.computedStyle.backgroundSize;

	public Color borderBottomColor => ve.computedStyle.borderBottomColor;

	public float borderBottomLeftRadius => ve.computedStyle.borderBottomLeftRadius.value;

	public float borderBottomRightRadius => ve.computedStyle.borderBottomRightRadius.value;

	public float borderBottomWidth => ve.layoutNode.LayoutBorderBottom;

	public Color borderLeftColor => ve.computedStyle.borderLeftColor;

	public float borderLeftWidth => ve.layoutNode.LayoutBorderLeft;

	public Color borderRightColor => ve.computedStyle.borderRightColor;

	public float borderRightWidth => ve.layoutNode.LayoutBorderRight;

	public Color borderTopColor => ve.computedStyle.borderTopColor;

	public float borderTopLeftRadius => ve.computedStyle.borderTopLeftRadius.value;

	public float borderTopRightRadius => ve.computedStyle.borderTopRightRadius.value;

	public float borderTopWidth => ve.layoutNode.LayoutBorderTop;

	public float bottom => ve.layoutNode.LayoutBottom;

	public Color color => ve.computedStyle.color;

	public DisplayStyle display => ve.computedStyle.display;

	public StyleFloat flexBasis => new StyleFloat(ve.layoutNode.ComputedFlexBasis);

	public FlexDirection flexDirection => ve.computedStyle.flexDirection;

	public float flexGrow => ve.computedStyle.flexGrow;

	public float flexShrink => ve.computedStyle.flexShrink;

	public Wrap flexWrap => ve.computedStyle.flexWrap;

	public float fontSize => ve.computedStyle.fontSize.value;

	public float height => ve.layoutNode.LayoutHeight;

	public Justify justifyContent => ve.computedStyle.justifyContent;

	public float left => ve.layoutNode.LayoutX;

	public float letterSpacing => ve.computedStyle.letterSpacing.value;

	public float marginBottom => ve.layoutNode.LayoutMarginBottom;

	public float marginLeft => ve.layoutNode.LayoutMarginLeft;

	public float marginRight => ve.layoutNode.LayoutMarginRight;

	public float marginTop => ve.layoutNode.LayoutMarginTop;

	public StyleFloat maxHeight => ve.ResolveLengthValue(ve.computedStyle.maxHeight, isRow: false);

	public StyleFloat maxWidth => ve.ResolveLengthValue(ve.computedStyle.maxWidth, isRow: true);

	public StyleFloat minHeight => ve.ResolveLengthValue(ve.computedStyle.minHeight, isRow: false);

	public StyleFloat minWidth => ve.ResolveLengthValue(ve.computedStyle.minWidth, isRow: true);

	public float opacity => ve.computedStyle.opacity;

	public float paddingBottom => ve.layoutNode.LayoutPaddingBottom;

	public float paddingLeft => ve.layoutNode.LayoutPaddingLeft;

	public float paddingRight => ve.layoutNode.LayoutPaddingRight;

	public float paddingTop => ve.layoutNode.LayoutPaddingTop;

	public Position position => ve.computedStyle.position;

	public float right => ve.layoutNode.LayoutRight;

	public Rotate rotate => ve.computedStyle.rotate;

	public Scale scale => ve.computedStyle.scale;

	public TextOverflow textOverflow => ve.computedStyle.textOverflow;

	public float top => ve.layoutNode.LayoutY;

	public Vector3 transformOrigin => ve.ResolveTransformOrigin();

	public IEnumerable<TimeValue> transitionDelay => ve.computedStyle.transitionDelay;

	public IEnumerable<TimeValue> transitionDuration => ve.computedStyle.transitionDuration;

	public IEnumerable<StylePropertyName> transitionProperty => ve.computedStyle.transitionProperty;

	public IEnumerable<EasingFunction> transitionTimingFunction => ve.computedStyle.transitionTimingFunction;

	public Vector3 translate => ve.ResolveTranslate();

	public Color unityBackgroundImageTintColor => ve.computedStyle.unityBackgroundImageTintColor;

	public EditorTextRenderingMode unityEditorTextRenderingMode => ve.computedStyle.unityEditorTextRenderingMode;

	public Font unityFont => ve.computedStyle.unityFont;

	public FontDefinition unityFontDefinition => ve.computedStyle.unityFontDefinition;

	public FontStyle unityFontStyleAndWeight => ve.computedStyle.unityFontStyleAndWeight;

	public float unityParagraphSpacing => ve.computedStyle.unityParagraphSpacing.value;

	public int unitySliceBottom => ve.computedStyle.unitySliceBottom;

	public int unitySliceLeft => ve.computedStyle.unitySliceLeft;

	public int unitySliceRight => ve.computedStyle.unitySliceRight;

	public float unitySliceScale => ve.computedStyle.unitySliceScale;

	public int unitySliceTop => ve.computedStyle.unitySliceTop;

	public SliceType unitySliceType => ve.computedStyle.unitySliceType;

	public TextAnchor unityTextAlign => ve.computedStyle.unityTextAlign;

	public TextGeneratorType unityTextGenerator => ve.computedStyle.unityTextGenerator;

	public Color unityTextOutlineColor => ve.computedStyle.unityTextOutlineColor;

	public float unityTextOutlineWidth => ve.computedStyle.unityTextOutlineWidth;

	public TextOverflowPosition unityTextOverflowPosition => ve.computedStyle.unityTextOverflowPosition;

	public Visibility visibility => ve.computedStyle.visibility;

	public WhiteSpace whiteSpace => ve.computedStyle.whiteSpace;

	public float width => ve.layoutNode.LayoutWidth;

	public float wordSpacing => ve.computedStyle.wordSpacing.value;

	private VisualElement ve { get; }

	[Obsolete("unityBackgroundScaleMode is deprecated. Use background-* properties instead.")]
	public StyleEnum<ScaleMode> unityBackgroundScaleMode
	{
		get
		{
			bool valid;
			return BackgroundPropertyHelper.ResolveUnityBackgroundScaleMode(ve.computedStyle.backgroundPositionX, ve.computedStyle.backgroundPositionY, ve.computedStyle.backgroundRepeat, ve.computedStyle.backgroundSize, out valid);
		}
	}

	public ResolvedStyleAccess(VisualElement ve)
	{
		this.ve = ve;
	}
}
