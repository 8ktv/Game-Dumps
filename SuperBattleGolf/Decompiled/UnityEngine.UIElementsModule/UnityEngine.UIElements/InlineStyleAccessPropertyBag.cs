using System;
using System.Collections.Generic;
using Unity.Properties;
using UnityEngine.TextCore.Text;

namespace UnityEngine.UIElements;

internal class InlineStyleAccessPropertyBag : PropertyBag<InlineStyleAccess>, INamedProperties<InlineStyleAccess>
{
	private class AlignContentProperty : InlineStyleEnumProperty<Align>
	{
		public override string Name => "alignContent";

		public override string ussName => "align-content";

		public override bool IsReadOnly => false;

		public override StyleEnum<Align> GetValue(ref InlineStyleAccess container)
		{
			return ((IStyle)container).alignContent;
		}

		public override void SetValue(ref InlineStyleAccess container, StyleEnum<Align> value)
		{
			((IStyle)container).alignContent = value;
		}
	}

	private class AlignItemsProperty : InlineStyleEnumProperty<Align>
	{
		public override string Name => "alignItems";

		public override string ussName => "align-items";

		public override bool IsReadOnly => false;

		public override StyleEnum<Align> GetValue(ref InlineStyleAccess container)
		{
			return ((IStyle)container).alignItems;
		}

		public override void SetValue(ref InlineStyleAccess container, StyleEnum<Align> value)
		{
			((IStyle)container).alignItems = value;
		}
	}

	private class AlignSelfProperty : InlineStyleEnumProperty<Align>
	{
		public override string Name => "alignSelf";

		public override string ussName => "align-self";

		public override bool IsReadOnly => false;

		public override StyleEnum<Align> GetValue(ref InlineStyleAccess container)
		{
			return ((IStyle)container).alignSelf;
		}

		public override void SetValue(ref InlineStyleAccess container, StyleEnum<Align> value)
		{
			((IStyle)container).alignSelf = value;
		}
	}

	private class BackgroundColorProperty : InlineStyleColorProperty
	{
		public override string Name => "backgroundColor";

		public override string ussName => "background-color";

		public override bool IsReadOnly => false;

		public override StyleColor GetValue(ref InlineStyleAccess container)
		{
			return ((IStyle)container).backgroundColor;
		}

		public override void SetValue(ref InlineStyleAccess container, StyleColor value)
		{
			((IStyle)container).backgroundColor = value;
		}
	}

	private class BackgroundImageProperty : InlineStyleBackgroundProperty
	{
		public override string Name => "backgroundImage";

		public override string ussName => "background-image";

		public override bool IsReadOnly => false;

		public override StyleBackground GetValue(ref InlineStyleAccess container)
		{
			return ((IStyle)container).backgroundImage;
		}

		public override void SetValue(ref InlineStyleAccess container, StyleBackground value)
		{
			((IStyle)container).backgroundImage = value;
		}
	}

	private class BackgroundPositionXProperty : InlineStyleBackgroundPositionProperty
	{
		public override string Name => "backgroundPositionX";

		public override string ussName => "background-position-x";

		public override bool IsReadOnly => false;

		public override StyleBackgroundPosition GetValue(ref InlineStyleAccess container)
		{
			return ((IStyle)container).backgroundPositionX;
		}

		public override void SetValue(ref InlineStyleAccess container, StyleBackgroundPosition value)
		{
			((IStyle)container).backgroundPositionX = value;
		}
	}

	private class BackgroundPositionYProperty : InlineStyleBackgroundPositionProperty
	{
		public override string Name => "backgroundPositionY";

		public override string ussName => "background-position-y";

		public override bool IsReadOnly => false;

		public override StyleBackgroundPosition GetValue(ref InlineStyleAccess container)
		{
			return ((IStyle)container).backgroundPositionY;
		}

		public override void SetValue(ref InlineStyleAccess container, StyleBackgroundPosition value)
		{
			((IStyle)container).backgroundPositionY = value;
		}
	}

	private class BackgroundRepeatProperty : InlineStyleBackgroundRepeatProperty
	{
		public override string Name => "backgroundRepeat";

		public override string ussName => "background-repeat";

		public override bool IsReadOnly => false;

		public override StyleBackgroundRepeat GetValue(ref InlineStyleAccess container)
		{
			return ((IStyle)container).backgroundRepeat;
		}

		public override void SetValue(ref InlineStyleAccess container, StyleBackgroundRepeat value)
		{
			((IStyle)container).backgroundRepeat = value;
		}
	}

	private class BackgroundSizeProperty : InlineStyleBackgroundSizeProperty
	{
		public override string Name => "backgroundSize";

		public override string ussName => "background-size";

		public override bool IsReadOnly => false;

		public override StyleBackgroundSize GetValue(ref InlineStyleAccess container)
		{
			return ((IStyle)container).backgroundSize;
		}

		public override void SetValue(ref InlineStyleAccess container, StyleBackgroundSize value)
		{
			((IStyle)container).backgroundSize = value;
		}
	}

	private class BorderBottomColorProperty : InlineStyleColorProperty
	{
		public override string Name => "borderBottomColor";

		public override string ussName => "border-bottom-color";

		public override bool IsReadOnly => false;

		public override StyleColor GetValue(ref InlineStyleAccess container)
		{
			return ((IStyle)container).borderBottomColor;
		}

		public override void SetValue(ref InlineStyleAccess container, StyleColor value)
		{
			((IStyle)container).borderBottomColor = value;
		}
	}

	private class BorderBottomLeftRadiusProperty : InlineStyleLengthProperty
	{
		public override string Name => "borderBottomLeftRadius";

		public override string ussName => "border-bottom-left-radius";

		public override bool IsReadOnly => false;

		public override StyleLength GetValue(ref InlineStyleAccess container)
		{
			return ((IStyle)container).borderBottomLeftRadius;
		}

		public override void SetValue(ref InlineStyleAccess container, StyleLength value)
		{
			((IStyle)container).borderBottomLeftRadius = value;
		}
	}

	private class BorderBottomRightRadiusProperty : InlineStyleLengthProperty
	{
		public override string Name => "borderBottomRightRadius";

		public override string ussName => "border-bottom-right-radius";

		public override bool IsReadOnly => false;

		public override StyleLength GetValue(ref InlineStyleAccess container)
		{
			return ((IStyle)container).borderBottomRightRadius;
		}

		public override void SetValue(ref InlineStyleAccess container, StyleLength value)
		{
			((IStyle)container).borderBottomRightRadius = value;
		}
	}

	private class BorderBottomWidthProperty : InlineStyleFloatProperty
	{
		public override string Name => "borderBottomWidth";

		public override string ussName => "border-bottom-width";

		public override bool IsReadOnly => false;

		public override StyleFloat GetValue(ref InlineStyleAccess container)
		{
			return ((IStyle)container).borderBottomWidth;
		}

		public override void SetValue(ref InlineStyleAccess container, StyleFloat value)
		{
			((IStyle)container).borderBottomWidth = value;
		}
	}

	private class BorderLeftColorProperty : InlineStyleColorProperty
	{
		public override string Name => "borderLeftColor";

		public override string ussName => "border-left-color";

		public override bool IsReadOnly => false;

		public override StyleColor GetValue(ref InlineStyleAccess container)
		{
			return ((IStyle)container).borderLeftColor;
		}

		public override void SetValue(ref InlineStyleAccess container, StyleColor value)
		{
			((IStyle)container).borderLeftColor = value;
		}
	}

	private class BorderLeftWidthProperty : InlineStyleFloatProperty
	{
		public override string Name => "borderLeftWidth";

		public override string ussName => "border-left-width";

		public override bool IsReadOnly => false;

		public override StyleFloat GetValue(ref InlineStyleAccess container)
		{
			return ((IStyle)container).borderLeftWidth;
		}

		public override void SetValue(ref InlineStyleAccess container, StyleFloat value)
		{
			((IStyle)container).borderLeftWidth = value;
		}
	}

	private class BorderRightColorProperty : InlineStyleColorProperty
	{
		public override string Name => "borderRightColor";

		public override string ussName => "border-right-color";

		public override bool IsReadOnly => false;

		public override StyleColor GetValue(ref InlineStyleAccess container)
		{
			return ((IStyle)container).borderRightColor;
		}

		public override void SetValue(ref InlineStyleAccess container, StyleColor value)
		{
			((IStyle)container).borderRightColor = value;
		}
	}

	private class BorderRightWidthProperty : InlineStyleFloatProperty
	{
		public override string Name => "borderRightWidth";

		public override string ussName => "border-right-width";

		public override bool IsReadOnly => false;

		public override StyleFloat GetValue(ref InlineStyleAccess container)
		{
			return ((IStyle)container).borderRightWidth;
		}

		public override void SetValue(ref InlineStyleAccess container, StyleFloat value)
		{
			((IStyle)container).borderRightWidth = value;
		}
	}

	private class BorderTopColorProperty : InlineStyleColorProperty
	{
		public override string Name => "borderTopColor";

		public override string ussName => "border-top-color";

		public override bool IsReadOnly => false;

		public override StyleColor GetValue(ref InlineStyleAccess container)
		{
			return ((IStyle)container).borderTopColor;
		}

		public override void SetValue(ref InlineStyleAccess container, StyleColor value)
		{
			((IStyle)container).borderTopColor = value;
		}
	}

	private class BorderTopLeftRadiusProperty : InlineStyleLengthProperty
	{
		public override string Name => "borderTopLeftRadius";

		public override string ussName => "border-top-left-radius";

		public override bool IsReadOnly => false;

		public override StyleLength GetValue(ref InlineStyleAccess container)
		{
			return ((IStyle)container).borderTopLeftRadius;
		}

		public override void SetValue(ref InlineStyleAccess container, StyleLength value)
		{
			((IStyle)container).borderTopLeftRadius = value;
		}
	}

	private class BorderTopRightRadiusProperty : InlineStyleLengthProperty
	{
		public override string Name => "borderTopRightRadius";

		public override string ussName => "border-top-right-radius";

		public override bool IsReadOnly => false;

		public override StyleLength GetValue(ref InlineStyleAccess container)
		{
			return ((IStyle)container).borderTopRightRadius;
		}

		public override void SetValue(ref InlineStyleAccess container, StyleLength value)
		{
			((IStyle)container).borderTopRightRadius = value;
		}
	}

	private class BorderTopWidthProperty : InlineStyleFloatProperty
	{
		public override string Name => "borderTopWidth";

		public override string ussName => "border-top-width";

		public override bool IsReadOnly => false;

		public override StyleFloat GetValue(ref InlineStyleAccess container)
		{
			return ((IStyle)container).borderTopWidth;
		}

		public override void SetValue(ref InlineStyleAccess container, StyleFloat value)
		{
			((IStyle)container).borderTopWidth = value;
		}
	}

	private class BottomProperty : InlineStyleLengthProperty
	{
		public override string Name => "bottom";

		public override string ussName => "bottom";

		public override bool IsReadOnly => false;

		public override StyleLength GetValue(ref InlineStyleAccess container)
		{
			return ((IStyle)container).bottom;
		}

		public override void SetValue(ref InlineStyleAccess container, StyleLength value)
		{
			((IStyle)container).bottom = value;
		}
	}

	private class ColorProperty : InlineStyleColorProperty
	{
		public override string Name => "color";

		public override string ussName => "color";

		public override bool IsReadOnly => false;

		public override StyleColor GetValue(ref InlineStyleAccess container)
		{
			return ((IStyle)container).color;
		}

		public override void SetValue(ref InlineStyleAccess container, StyleColor value)
		{
			((IStyle)container).color = value;
		}
	}

	private class CursorProperty : InlineStyleCursorProperty
	{
		public override string Name => "cursor";

		public override string ussName => "cursor";

		public override bool IsReadOnly => false;

		public override StyleCursor GetValue(ref InlineStyleAccess container)
		{
			return ((IStyle)container).cursor;
		}

		public override void SetValue(ref InlineStyleAccess container, StyleCursor value)
		{
			((IStyle)container).cursor = value;
		}
	}

	private class DisplayProperty : InlineStyleEnumProperty<DisplayStyle>
	{
		public override string Name => "display";

		public override string ussName => "display";

		public override bool IsReadOnly => false;

		public override StyleEnum<DisplayStyle> GetValue(ref InlineStyleAccess container)
		{
			return ((IStyle)container).display;
		}

		public override void SetValue(ref InlineStyleAccess container, StyleEnum<DisplayStyle> value)
		{
			((IStyle)container).display = value;
		}
	}

	private class FlexBasisProperty : InlineStyleLengthProperty
	{
		public override string Name => "flexBasis";

		public override string ussName => "flex-basis";

		public override bool IsReadOnly => false;

		public override StyleLength GetValue(ref InlineStyleAccess container)
		{
			return ((IStyle)container).flexBasis;
		}

		public override void SetValue(ref InlineStyleAccess container, StyleLength value)
		{
			((IStyle)container).flexBasis = value;
		}
	}

	private class FlexDirectionProperty : InlineStyleEnumProperty<FlexDirection>
	{
		public override string Name => "flexDirection";

		public override string ussName => "flex-direction";

		public override bool IsReadOnly => false;

		public override StyleEnum<FlexDirection> GetValue(ref InlineStyleAccess container)
		{
			return ((IStyle)container).flexDirection;
		}

		public override void SetValue(ref InlineStyleAccess container, StyleEnum<FlexDirection> value)
		{
			((IStyle)container).flexDirection = value;
		}
	}

	private class FlexGrowProperty : InlineStyleFloatProperty
	{
		public override string Name => "flexGrow";

		public override string ussName => "flex-grow";

		public override bool IsReadOnly => false;

		public override StyleFloat GetValue(ref InlineStyleAccess container)
		{
			return ((IStyle)container).flexGrow;
		}

		public override void SetValue(ref InlineStyleAccess container, StyleFloat value)
		{
			((IStyle)container).flexGrow = value;
		}
	}

	private class FlexShrinkProperty : InlineStyleFloatProperty
	{
		public override string Name => "flexShrink";

		public override string ussName => "flex-shrink";

		public override bool IsReadOnly => false;

		public override StyleFloat GetValue(ref InlineStyleAccess container)
		{
			return ((IStyle)container).flexShrink;
		}

		public override void SetValue(ref InlineStyleAccess container, StyleFloat value)
		{
			((IStyle)container).flexShrink = value;
		}
	}

	private class FlexWrapProperty : InlineStyleEnumProperty<Wrap>
	{
		public override string Name => "flexWrap";

		public override string ussName => "flex-wrap";

		public override bool IsReadOnly => false;

		public override StyleEnum<Wrap> GetValue(ref InlineStyleAccess container)
		{
			return ((IStyle)container).flexWrap;
		}

		public override void SetValue(ref InlineStyleAccess container, StyleEnum<Wrap> value)
		{
			((IStyle)container).flexWrap = value;
		}
	}

	private class FontSizeProperty : InlineStyleLengthProperty
	{
		public override string Name => "fontSize";

		public override string ussName => "font-size";

		public override bool IsReadOnly => false;

		public override StyleLength GetValue(ref InlineStyleAccess container)
		{
			return ((IStyle)container).fontSize;
		}

		public override void SetValue(ref InlineStyleAccess container, StyleLength value)
		{
			((IStyle)container).fontSize = value;
		}
	}

	private class HeightProperty : InlineStyleLengthProperty
	{
		public override string Name => "height";

		public override string ussName => "height";

		public override bool IsReadOnly => false;

		public override StyleLength GetValue(ref InlineStyleAccess container)
		{
			return ((IStyle)container).height;
		}

		public override void SetValue(ref InlineStyleAccess container, StyleLength value)
		{
			((IStyle)container).height = value;
		}
	}

	private class JustifyContentProperty : InlineStyleEnumProperty<Justify>
	{
		public override string Name => "justifyContent";

		public override string ussName => "justify-content";

		public override bool IsReadOnly => false;

		public override StyleEnum<Justify> GetValue(ref InlineStyleAccess container)
		{
			return ((IStyle)container).justifyContent;
		}

		public override void SetValue(ref InlineStyleAccess container, StyleEnum<Justify> value)
		{
			((IStyle)container).justifyContent = value;
		}
	}

	private class LeftProperty : InlineStyleLengthProperty
	{
		public override string Name => "left";

		public override string ussName => "left";

		public override bool IsReadOnly => false;

		public override StyleLength GetValue(ref InlineStyleAccess container)
		{
			return ((IStyle)container).left;
		}

		public override void SetValue(ref InlineStyleAccess container, StyleLength value)
		{
			((IStyle)container).left = value;
		}
	}

	private class LetterSpacingProperty : InlineStyleLengthProperty
	{
		public override string Name => "letterSpacing";

		public override string ussName => "letter-spacing";

		public override bool IsReadOnly => false;

		public override StyleLength GetValue(ref InlineStyleAccess container)
		{
			return ((IStyle)container).letterSpacing;
		}

		public override void SetValue(ref InlineStyleAccess container, StyleLength value)
		{
			((IStyle)container).letterSpacing = value;
		}
	}

	private class MarginBottomProperty : InlineStyleLengthProperty
	{
		public override string Name => "marginBottom";

		public override string ussName => "margin-bottom";

		public override bool IsReadOnly => false;

		public override StyleLength GetValue(ref InlineStyleAccess container)
		{
			return ((IStyle)container).marginBottom;
		}

		public override void SetValue(ref InlineStyleAccess container, StyleLength value)
		{
			((IStyle)container).marginBottom = value;
		}
	}

	private class MarginLeftProperty : InlineStyleLengthProperty
	{
		public override string Name => "marginLeft";

		public override string ussName => "margin-left";

		public override bool IsReadOnly => false;

		public override StyleLength GetValue(ref InlineStyleAccess container)
		{
			return ((IStyle)container).marginLeft;
		}

		public override void SetValue(ref InlineStyleAccess container, StyleLength value)
		{
			((IStyle)container).marginLeft = value;
		}
	}

	private class MarginRightProperty : InlineStyleLengthProperty
	{
		public override string Name => "marginRight";

		public override string ussName => "margin-right";

		public override bool IsReadOnly => false;

		public override StyleLength GetValue(ref InlineStyleAccess container)
		{
			return ((IStyle)container).marginRight;
		}

		public override void SetValue(ref InlineStyleAccess container, StyleLength value)
		{
			((IStyle)container).marginRight = value;
		}
	}

	private class MarginTopProperty : InlineStyleLengthProperty
	{
		public override string Name => "marginTop";

		public override string ussName => "margin-top";

		public override bool IsReadOnly => false;

		public override StyleLength GetValue(ref InlineStyleAccess container)
		{
			return ((IStyle)container).marginTop;
		}

		public override void SetValue(ref InlineStyleAccess container, StyleLength value)
		{
			((IStyle)container).marginTop = value;
		}
	}

	private class MaxHeightProperty : InlineStyleLengthProperty
	{
		public override string Name => "maxHeight";

		public override string ussName => "max-height";

		public override bool IsReadOnly => false;

		public override StyleLength GetValue(ref InlineStyleAccess container)
		{
			return ((IStyle)container).maxHeight;
		}

		public override void SetValue(ref InlineStyleAccess container, StyleLength value)
		{
			((IStyle)container).maxHeight = value;
		}
	}

	private class MaxWidthProperty : InlineStyleLengthProperty
	{
		public override string Name => "maxWidth";

		public override string ussName => "max-width";

		public override bool IsReadOnly => false;

		public override StyleLength GetValue(ref InlineStyleAccess container)
		{
			return ((IStyle)container).maxWidth;
		}

		public override void SetValue(ref InlineStyleAccess container, StyleLength value)
		{
			((IStyle)container).maxWidth = value;
		}
	}

	private class MinHeightProperty : InlineStyleLengthProperty
	{
		public override string Name => "minHeight";

		public override string ussName => "min-height";

		public override bool IsReadOnly => false;

		public override StyleLength GetValue(ref InlineStyleAccess container)
		{
			return ((IStyle)container).minHeight;
		}

		public override void SetValue(ref InlineStyleAccess container, StyleLength value)
		{
			((IStyle)container).minHeight = value;
		}
	}

	private class MinWidthProperty : InlineStyleLengthProperty
	{
		public override string Name => "minWidth";

		public override string ussName => "min-width";

		public override bool IsReadOnly => false;

		public override StyleLength GetValue(ref InlineStyleAccess container)
		{
			return ((IStyle)container).minWidth;
		}

		public override void SetValue(ref InlineStyleAccess container, StyleLength value)
		{
			((IStyle)container).minWidth = value;
		}
	}

	private class OpacityProperty : InlineStyleFloatProperty
	{
		public override string Name => "opacity";

		public override string ussName => "opacity";

		public override bool IsReadOnly => false;

		public override StyleFloat GetValue(ref InlineStyleAccess container)
		{
			return ((IStyle)container).opacity;
		}

		public override void SetValue(ref InlineStyleAccess container, StyleFloat value)
		{
			((IStyle)container).opacity = value;
		}
	}

	private class OverflowProperty : InlineStyleEnumProperty<Overflow>
	{
		public override string Name => "overflow";

		public override string ussName => "overflow";

		public override bool IsReadOnly => false;

		public override StyleEnum<Overflow> GetValue(ref InlineStyleAccess container)
		{
			return ((IStyle)container).overflow;
		}

		public override void SetValue(ref InlineStyleAccess container, StyleEnum<Overflow> value)
		{
			((IStyle)container).overflow = value;
		}
	}

	private class PaddingBottomProperty : InlineStyleLengthProperty
	{
		public override string Name => "paddingBottom";

		public override string ussName => "padding-bottom";

		public override bool IsReadOnly => false;

		public override StyleLength GetValue(ref InlineStyleAccess container)
		{
			return ((IStyle)container).paddingBottom;
		}

		public override void SetValue(ref InlineStyleAccess container, StyleLength value)
		{
			((IStyle)container).paddingBottom = value;
		}
	}

	private class PaddingLeftProperty : InlineStyleLengthProperty
	{
		public override string Name => "paddingLeft";

		public override string ussName => "padding-left";

		public override bool IsReadOnly => false;

		public override StyleLength GetValue(ref InlineStyleAccess container)
		{
			return ((IStyle)container).paddingLeft;
		}

		public override void SetValue(ref InlineStyleAccess container, StyleLength value)
		{
			((IStyle)container).paddingLeft = value;
		}
	}

	private class PaddingRightProperty : InlineStyleLengthProperty
	{
		public override string Name => "paddingRight";

		public override string ussName => "padding-right";

		public override bool IsReadOnly => false;

		public override StyleLength GetValue(ref InlineStyleAccess container)
		{
			return ((IStyle)container).paddingRight;
		}

		public override void SetValue(ref InlineStyleAccess container, StyleLength value)
		{
			((IStyle)container).paddingRight = value;
		}
	}

	private class PaddingTopProperty : InlineStyleLengthProperty
	{
		public override string Name => "paddingTop";

		public override string ussName => "padding-top";

		public override bool IsReadOnly => false;

		public override StyleLength GetValue(ref InlineStyleAccess container)
		{
			return ((IStyle)container).paddingTop;
		}

		public override void SetValue(ref InlineStyleAccess container, StyleLength value)
		{
			((IStyle)container).paddingTop = value;
		}
	}

	private class PositionProperty : InlineStyleEnumProperty<Position>
	{
		public override string Name => "position";

		public override string ussName => "position";

		public override bool IsReadOnly => false;

		public override StyleEnum<Position> GetValue(ref InlineStyleAccess container)
		{
			return ((IStyle)container).position;
		}

		public override void SetValue(ref InlineStyleAccess container, StyleEnum<Position> value)
		{
			((IStyle)container).position = value;
		}
	}

	private class RightProperty : InlineStyleLengthProperty
	{
		public override string Name => "right";

		public override string ussName => "right";

		public override bool IsReadOnly => false;

		public override StyleLength GetValue(ref InlineStyleAccess container)
		{
			return ((IStyle)container).right;
		}

		public override void SetValue(ref InlineStyleAccess container, StyleLength value)
		{
			((IStyle)container).right = value;
		}
	}

	private class RotateProperty : InlineStyleRotateProperty
	{
		public override string Name => "rotate";

		public override string ussName => "rotate";

		public override bool IsReadOnly => false;

		public override StyleRotate GetValue(ref InlineStyleAccess container)
		{
			return ((IStyle)container).rotate;
		}

		public override void SetValue(ref InlineStyleAccess container, StyleRotate value)
		{
			((IStyle)container).rotate = value;
		}
	}

	private class ScaleProperty : InlineStyleScaleProperty
	{
		public override string Name => "scale";

		public override string ussName => "scale";

		public override bool IsReadOnly => false;

		public override StyleScale GetValue(ref InlineStyleAccess container)
		{
			return ((IStyle)container).scale;
		}

		public override void SetValue(ref InlineStyleAccess container, StyleScale value)
		{
			((IStyle)container).scale = value;
		}
	}

	private class TextOverflowProperty : InlineStyleEnumProperty<TextOverflow>
	{
		public override string Name => "textOverflow";

		public override string ussName => "text-overflow";

		public override bool IsReadOnly => false;

		public override StyleEnum<TextOverflow> GetValue(ref InlineStyleAccess container)
		{
			return ((IStyle)container).textOverflow;
		}

		public override void SetValue(ref InlineStyleAccess container, StyleEnum<TextOverflow> value)
		{
			((IStyle)container).textOverflow = value;
		}
	}

	private class TextShadowProperty : InlineStyleTextShadowProperty
	{
		public override string Name => "textShadow";

		public override string ussName => "text-shadow";

		public override bool IsReadOnly => false;

		public override StyleTextShadow GetValue(ref InlineStyleAccess container)
		{
			return ((IStyle)container).textShadow;
		}

		public override void SetValue(ref InlineStyleAccess container, StyleTextShadow value)
		{
			((IStyle)container).textShadow = value;
		}
	}

	private class TopProperty : InlineStyleLengthProperty
	{
		public override string Name => "top";

		public override string ussName => "top";

		public override bool IsReadOnly => false;

		public override StyleLength GetValue(ref InlineStyleAccess container)
		{
			return ((IStyle)container).top;
		}

		public override void SetValue(ref InlineStyleAccess container, StyleLength value)
		{
			((IStyle)container).top = value;
		}
	}

	private class TransformOriginProperty : InlineStyleTransformOriginProperty
	{
		public override string Name => "transformOrigin";

		public override string ussName => "transform-origin";

		public override bool IsReadOnly => false;

		public override StyleTransformOrigin GetValue(ref InlineStyleAccess container)
		{
			return ((IStyle)container).transformOrigin;
		}

		public override void SetValue(ref InlineStyleAccess container, StyleTransformOrigin value)
		{
			((IStyle)container).transformOrigin = value;
		}
	}

	private class TransitionDelayProperty : InlineStyleListProperty<TimeValue>
	{
		public override string Name => "transitionDelay";

		public override string ussName => "transition-delay";

		public override bool IsReadOnly => false;

		public override StyleList<TimeValue> GetValue(ref InlineStyleAccess container)
		{
			return ((IStyle)container).transitionDelay;
		}

		public override void SetValue(ref InlineStyleAccess container, StyleList<TimeValue> value)
		{
			((IStyle)container).transitionDelay = value;
		}
	}

	private class TransitionDurationProperty : InlineStyleListProperty<TimeValue>
	{
		public override string Name => "transitionDuration";

		public override string ussName => "transition-duration";

		public override bool IsReadOnly => false;

		public override StyleList<TimeValue> GetValue(ref InlineStyleAccess container)
		{
			return ((IStyle)container).transitionDuration;
		}

		public override void SetValue(ref InlineStyleAccess container, StyleList<TimeValue> value)
		{
			((IStyle)container).transitionDuration = value;
		}
	}

	private class TransitionPropertyProperty : InlineStyleListProperty<StylePropertyName>
	{
		public override string Name => "transitionProperty";

		public override string ussName => "transition-property";

		public override bool IsReadOnly => false;

		public override StyleList<StylePropertyName> GetValue(ref InlineStyleAccess container)
		{
			return ((IStyle)container).transitionProperty;
		}

		public override void SetValue(ref InlineStyleAccess container, StyleList<StylePropertyName> value)
		{
			((IStyle)container).transitionProperty = value;
		}
	}

	private class TransitionTimingFunctionProperty : InlineStyleListProperty<EasingFunction>
	{
		public override string Name => "transitionTimingFunction";

		public override string ussName => "transition-timing-function";

		public override bool IsReadOnly => false;

		public override StyleList<EasingFunction> GetValue(ref InlineStyleAccess container)
		{
			return ((IStyle)container).transitionTimingFunction;
		}

		public override void SetValue(ref InlineStyleAccess container, StyleList<EasingFunction> value)
		{
			((IStyle)container).transitionTimingFunction = value;
		}
	}

	private class TranslateProperty : InlineStyleTranslateProperty
	{
		public override string Name => "translate";

		public override string ussName => "translate";

		public override bool IsReadOnly => false;

		public override StyleTranslate GetValue(ref InlineStyleAccess container)
		{
			return ((IStyle)container).translate;
		}

		public override void SetValue(ref InlineStyleAccess container, StyleTranslate value)
		{
			((IStyle)container).translate = value;
		}
	}

	private class UnityBackgroundImageTintColorProperty : InlineStyleColorProperty
	{
		public override string Name => "unityBackgroundImageTintColor";

		public override string ussName => "-unity-background-image-tint-color";

		public override bool IsReadOnly => false;

		public override StyleColor GetValue(ref InlineStyleAccess container)
		{
			return ((IStyle)container).unityBackgroundImageTintColor;
		}

		public override void SetValue(ref InlineStyleAccess container, StyleColor value)
		{
			((IStyle)container).unityBackgroundImageTintColor = value;
		}
	}

	private class UnityEditorTextRenderingModeProperty : InlineStyleEnumProperty<EditorTextRenderingMode>
	{
		public override string Name => "unityEditorTextRenderingMode";

		public override string ussName => "-unity-editor-text-rendering-mode";

		public override bool IsReadOnly => false;

		public override StyleEnum<EditorTextRenderingMode> GetValue(ref InlineStyleAccess container)
		{
			return ((IStyle)container).unityEditorTextRenderingMode;
		}

		public override void SetValue(ref InlineStyleAccess container, StyleEnum<EditorTextRenderingMode> value)
		{
			((IStyle)container).unityEditorTextRenderingMode = value;
		}
	}

	private class UnityFontProperty : InlineStyleFontProperty
	{
		public override string Name => "unityFont";

		public override string ussName => "-unity-font";

		public override bool IsReadOnly => false;

		public override StyleFont GetValue(ref InlineStyleAccess container)
		{
			return ((IStyle)container).unityFont;
		}

		public override void SetValue(ref InlineStyleAccess container, StyleFont value)
		{
			((IStyle)container).unityFont = value;
		}
	}

	private class UnityFontDefinitionProperty : InlineStyleFontDefinitionProperty
	{
		public override string Name => "unityFontDefinition";

		public override string ussName => "-unity-font-definition";

		public override bool IsReadOnly => false;

		public override StyleFontDefinition GetValue(ref InlineStyleAccess container)
		{
			return ((IStyle)container).unityFontDefinition;
		}

		public override void SetValue(ref InlineStyleAccess container, StyleFontDefinition value)
		{
			((IStyle)container).unityFontDefinition = value;
		}
	}

	private class UnityFontStyleAndWeightProperty : InlineStyleEnumProperty<FontStyle>
	{
		public override string Name => "unityFontStyleAndWeight";

		public override string ussName => "-unity-font-style";

		public override bool IsReadOnly => false;

		public override StyleEnum<FontStyle> GetValue(ref InlineStyleAccess container)
		{
			return ((IStyle)container).unityFontStyleAndWeight;
		}

		public override void SetValue(ref InlineStyleAccess container, StyleEnum<FontStyle> value)
		{
			((IStyle)container).unityFontStyleAndWeight = value;
		}
	}

	private class UnityOverflowClipBoxProperty : InlineStyleEnumProperty<OverflowClipBox>
	{
		public override string Name => "unityOverflowClipBox";

		public override string ussName => "-unity-overflow-clip-box";

		public override bool IsReadOnly => false;

		public override StyleEnum<OverflowClipBox> GetValue(ref InlineStyleAccess container)
		{
			return ((IStyle)container).unityOverflowClipBox;
		}

		public override void SetValue(ref InlineStyleAccess container, StyleEnum<OverflowClipBox> value)
		{
			((IStyle)container).unityOverflowClipBox = value;
		}
	}

	private class UnityParagraphSpacingProperty : InlineStyleLengthProperty
	{
		public override string Name => "unityParagraphSpacing";

		public override string ussName => "-unity-paragraph-spacing";

		public override bool IsReadOnly => false;

		public override StyleLength GetValue(ref InlineStyleAccess container)
		{
			return ((IStyle)container).unityParagraphSpacing;
		}

		public override void SetValue(ref InlineStyleAccess container, StyleLength value)
		{
			((IStyle)container).unityParagraphSpacing = value;
		}
	}

	private class UnitySliceBottomProperty : InlineStyleIntProperty
	{
		public override string Name => "unitySliceBottom";

		public override string ussName => "-unity-slice-bottom";

		public override bool IsReadOnly => false;

		public override StyleInt GetValue(ref InlineStyleAccess container)
		{
			return ((IStyle)container).unitySliceBottom;
		}

		public override void SetValue(ref InlineStyleAccess container, StyleInt value)
		{
			((IStyle)container).unitySliceBottom = value;
		}
	}

	private class UnitySliceLeftProperty : InlineStyleIntProperty
	{
		public override string Name => "unitySliceLeft";

		public override string ussName => "-unity-slice-left";

		public override bool IsReadOnly => false;

		public override StyleInt GetValue(ref InlineStyleAccess container)
		{
			return ((IStyle)container).unitySliceLeft;
		}

		public override void SetValue(ref InlineStyleAccess container, StyleInt value)
		{
			((IStyle)container).unitySliceLeft = value;
		}
	}

	private class UnitySliceRightProperty : InlineStyleIntProperty
	{
		public override string Name => "unitySliceRight";

		public override string ussName => "-unity-slice-right";

		public override bool IsReadOnly => false;

		public override StyleInt GetValue(ref InlineStyleAccess container)
		{
			return ((IStyle)container).unitySliceRight;
		}

		public override void SetValue(ref InlineStyleAccess container, StyleInt value)
		{
			((IStyle)container).unitySliceRight = value;
		}
	}

	private class UnitySliceScaleProperty : InlineStyleFloatProperty
	{
		public override string Name => "unitySliceScale";

		public override string ussName => "-unity-slice-scale";

		public override bool IsReadOnly => false;

		public override StyleFloat GetValue(ref InlineStyleAccess container)
		{
			return ((IStyle)container).unitySliceScale;
		}

		public override void SetValue(ref InlineStyleAccess container, StyleFloat value)
		{
			((IStyle)container).unitySliceScale = value;
		}
	}

	private class UnitySliceTopProperty : InlineStyleIntProperty
	{
		public override string Name => "unitySliceTop";

		public override string ussName => "-unity-slice-top";

		public override bool IsReadOnly => false;

		public override StyleInt GetValue(ref InlineStyleAccess container)
		{
			return ((IStyle)container).unitySliceTop;
		}

		public override void SetValue(ref InlineStyleAccess container, StyleInt value)
		{
			((IStyle)container).unitySliceTop = value;
		}
	}

	private class UnitySliceTypeProperty : InlineStyleEnumProperty<SliceType>
	{
		public override string Name => "unitySliceType";

		public override string ussName => "-unity-slice-type";

		public override bool IsReadOnly => false;

		public override StyleEnum<SliceType> GetValue(ref InlineStyleAccess container)
		{
			return ((IStyle)container).unitySliceType;
		}

		public override void SetValue(ref InlineStyleAccess container, StyleEnum<SliceType> value)
		{
			((IStyle)container).unitySliceType = value;
		}
	}

	private class UnityTextAlignProperty : InlineStyleEnumProperty<TextAnchor>
	{
		public override string Name => "unityTextAlign";

		public override string ussName => "-unity-text-align";

		public override bool IsReadOnly => false;

		public override StyleEnum<TextAnchor> GetValue(ref InlineStyleAccess container)
		{
			return ((IStyle)container).unityTextAlign;
		}

		public override void SetValue(ref InlineStyleAccess container, StyleEnum<TextAnchor> value)
		{
			((IStyle)container).unityTextAlign = value;
		}
	}

	private class UnityTextAutoSizeProperty : InlineStyleTextAutoSizeProperty
	{
		public override string Name => "unityTextAutoSize";

		public override string ussName => "-unity-text-auto-size";

		public override bool IsReadOnly => false;

		public override StyleTextAutoSize GetValue(ref InlineStyleAccess container)
		{
			return ((IStyle)container).unityTextAutoSize;
		}

		public override void SetValue(ref InlineStyleAccess container, StyleTextAutoSize value)
		{
			((IStyle)container).unityTextAutoSize = value;
		}
	}

	private class UnityTextGeneratorProperty : InlineStyleEnumProperty<TextGeneratorType>
	{
		public override string Name => "unityTextGenerator";

		public override string ussName => "-unity-text-generator";

		public override bool IsReadOnly => false;

		public override StyleEnum<TextGeneratorType> GetValue(ref InlineStyleAccess container)
		{
			return ((IStyle)container).unityTextGenerator;
		}

		public override void SetValue(ref InlineStyleAccess container, StyleEnum<TextGeneratorType> value)
		{
			((IStyle)container).unityTextGenerator = value;
		}
	}

	private class UnityTextOutlineColorProperty : InlineStyleColorProperty
	{
		public override string Name => "unityTextOutlineColor";

		public override string ussName => "-unity-text-outline-color";

		public override bool IsReadOnly => false;

		public override StyleColor GetValue(ref InlineStyleAccess container)
		{
			return ((IStyle)container).unityTextOutlineColor;
		}

		public override void SetValue(ref InlineStyleAccess container, StyleColor value)
		{
			((IStyle)container).unityTextOutlineColor = value;
		}
	}

	private class UnityTextOutlineWidthProperty : InlineStyleFloatProperty
	{
		public override string Name => "unityTextOutlineWidth";

		public override string ussName => "-unity-text-outline-width";

		public override bool IsReadOnly => false;

		public override StyleFloat GetValue(ref InlineStyleAccess container)
		{
			return ((IStyle)container).unityTextOutlineWidth;
		}

		public override void SetValue(ref InlineStyleAccess container, StyleFloat value)
		{
			((IStyle)container).unityTextOutlineWidth = value;
		}
	}

	private class UnityTextOverflowPositionProperty : InlineStyleEnumProperty<TextOverflowPosition>
	{
		public override string Name => "unityTextOverflowPosition";

		public override string ussName => "-unity-text-overflow-position";

		public override bool IsReadOnly => false;

		public override StyleEnum<TextOverflowPosition> GetValue(ref InlineStyleAccess container)
		{
			return ((IStyle)container).unityTextOverflowPosition;
		}

		public override void SetValue(ref InlineStyleAccess container, StyleEnum<TextOverflowPosition> value)
		{
			((IStyle)container).unityTextOverflowPosition = value;
		}
	}

	private class VisibilityProperty : InlineStyleEnumProperty<Visibility>
	{
		public override string Name => "visibility";

		public override string ussName => "visibility";

		public override bool IsReadOnly => false;

		public override StyleEnum<Visibility> GetValue(ref InlineStyleAccess container)
		{
			return ((IStyle)container).visibility;
		}

		public override void SetValue(ref InlineStyleAccess container, StyleEnum<Visibility> value)
		{
			((IStyle)container).visibility = value;
		}
	}

	private class WhiteSpaceProperty : InlineStyleEnumProperty<WhiteSpace>
	{
		public override string Name => "whiteSpace";

		public override string ussName => "white-space";

		public override bool IsReadOnly => false;

		public override StyleEnum<WhiteSpace> GetValue(ref InlineStyleAccess container)
		{
			return ((IStyle)container).whiteSpace;
		}

		public override void SetValue(ref InlineStyleAccess container, StyleEnum<WhiteSpace> value)
		{
			((IStyle)container).whiteSpace = value;
		}
	}

	private class WidthProperty : InlineStyleLengthProperty
	{
		public override string Name => "width";

		public override string ussName => "width";

		public override bool IsReadOnly => false;

		public override StyleLength GetValue(ref InlineStyleAccess container)
		{
			return ((IStyle)container).width;
		}

		public override void SetValue(ref InlineStyleAccess container, StyleLength value)
		{
			((IStyle)container).width = value;
		}
	}

	private class WordSpacingProperty : InlineStyleLengthProperty
	{
		public override string Name => "wordSpacing";

		public override string ussName => "word-spacing";

		public override bool IsReadOnly => false;

		public override StyleLength GetValue(ref InlineStyleAccess container)
		{
			return ((IStyle)container).wordSpacing;
		}

		public override void SetValue(ref InlineStyleAccess container, StyleLength value)
		{
			((IStyle)container).wordSpacing = value;
		}
	}

	private abstract class InlineStyleProperty<TStyleValue, TValue> : Property<InlineStyleAccess, TStyleValue> where TStyleValue : IStyleValue<TValue>, new()
	{
		public abstract string ussName { get; }

		protected InlineStyleProperty()
		{
			ConverterGroups.RegisterGlobal(delegate(ref TStyleValue sv)
			{
				return sv.value;
			});
			ConverterGroups.RegisterGlobal(delegate(ref TValue v)
			{
				return new TStyleValue
				{
					value = v
				};
			});
			ConverterGroups.RegisterGlobal(delegate(ref TStyleValue sv)
			{
				return sv.keyword;
			});
			ConverterGroups.RegisterGlobal(delegate(ref StyleKeyword kw)
			{
				return new TStyleValue
				{
					keyword = kw
				};
			});
		}
	}

	private abstract class InlineStyleEnumProperty<TValue> : InlineStyleProperty<StyleEnum<TValue>, TValue> where TValue : struct, IConvertible
	{
	}

	private abstract class InlineStyleColorProperty : InlineStyleProperty<StyleColor, Color>
	{
		protected InlineStyleColorProperty()
		{
			ConverterGroups.RegisterGlobal(delegate(ref Color32 v)
			{
				return new StyleColor(v);
			});
			ConverterGroups.RegisterGlobal((TypeConverter<StyleColor, Color32>)delegate(ref StyleColor sv)
			{
				return sv.value;
			});
		}
	}

	private abstract class InlineStyleBackgroundProperty : InlineStyleProperty<StyleBackground, Background>
	{
		protected InlineStyleBackgroundProperty()
		{
			ConverterGroups.RegisterGlobal(delegate(ref Texture2D v)
			{
				return new StyleBackground(v);
			});
			ConverterGroups.RegisterGlobal(delegate(ref Sprite v)
			{
				return new StyleBackground(v);
			});
			ConverterGroups.RegisterGlobal(delegate(ref VectorImage v)
			{
				return new StyleBackground(v);
			});
			ConverterGroups.RegisterGlobal(delegate(ref StyleBackground sv)
			{
				return sv.value.texture;
			});
			ConverterGroups.RegisterGlobal(delegate(ref StyleBackground sv)
			{
				return sv.value.sprite;
			});
			ConverterGroups.RegisterGlobal(delegate(ref StyleBackground sv)
			{
				return sv.value.renderTexture;
			});
			ConverterGroups.RegisterGlobal(delegate(ref StyleBackground sv)
			{
				return sv.value.vectorImage;
			});
		}
	}

	private abstract class InlineStyleLengthProperty : InlineStyleProperty<StyleLength, Length>
	{
		protected InlineStyleLengthProperty()
		{
			ConverterGroups.RegisterGlobal(delegate(ref float v)
			{
				return new StyleLength(v);
			});
			ConverterGroups.RegisterGlobal(delegate(ref int v)
			{
				return new StyleLength(v);
			});
			ConverterGroups.RegisterGlobal(delegate(ref StyleLength sv)
			{
				return sv.value.value;
			});
			ConverterGroups.RegisterGlobal(delegate(ref StyleLength sv)
			{
				return (int)sv.value.value;
			});
		}
	}

	private abstract class InlineStyleFloatProperty : InlineStyleProperty<StyleFloat, float>
	{
		protected InlineStyleFloatProperty()
		{
			ConverterGroups.RegisterGlobal(delegate(ref int v)
			{
				return new StyleFloat(v);
			});
			ConverterGroups.RegisterGlobal(delegate(ref StyleFloat sv)
			{
				return (int)sv.value;
			});
		}
	}

	private abstract class InlineStyleListProperty<T> : InlineStyleProperty<StyleList<T>, List<T>>
	{
	}

	private abstract class InlineStyleFontProperty : InlineStyleProperty<StyleFont, Font>
	{
	}

	private abstract class InlineStyleFontDefinitionProperty : InlineStyleProperty<StyleFontDefinition, FontDefinition>
	{
		protected InlineStyleFontDefinitionProperty()
		{
			ConverterGroups.RegisterGlobal(delegate(ref Font v)
			{
				return new StyleFontDefinition(v);
			});
			ConverterGroups.RegisterGlobal(delegate(ref FontAsset v)
			{
				return new StyleFontDefinition(v);
			});
			ConverterGroups.RegisterGlobal(delegate(ref StyleFontDefinition sv)
			{
				return sv.value.font;
			});
			ConverterGroups.RegisterGlobal(delegate(ref StyleFontDefinition sv)
			{
				return sv.value.fontAsset;
			});
		}
	}

	private abstract class InlineStyleIntProperty : InlineStyleProperty<StyleInt, int>
	{
	}

	private abstract class InlineStyleRotateProperty : InlineStyleProperty<StyleRotate, Rotate>
	{
	}

	private abstract class InlineStyleScaleProperty : InlineStyleProperty<StyleScale, Scale>
	{
	}

	private abstract class InlineStyleCursorProperty : InlineStyleProperty<StyleCursor, Cursor>
	{
	}

	private abstract class InlineStyleTextShadowProperty : InlineStyleProperty<StyleTextShadow, TextShadow>
	{
	}

	private abstract class InlineStyleTextAutoSizeProperty : InlineStyleProperty<StyleTextAutoSize, TextAutoSize>
	{
	}

	private abstract class InlineStyleTransformOriginProperty : InlineStyleProperty<StyleTransformOrigin, TransformOrigin>
	{
	}

	private abstract class InlineStyleTranslateProperty : InlineStyleProperty<StyleTranslate, Translate>
	{
	}

	private abstract class InlineStyleBackgroundPositionProperty : InlineStyleProperty<StyleBackgroundPosition, BackgroundPosition>
	{
	}

	private abstract class InlineStyleBackgroundRepeatProperty : InlineStyleProperty<StyleBackgroundRepeat, BackgroundRepeat>
	{
	}

	private abstract class InlineStyleBackgroundSizeProperty : InlineStyleProperty<StyleBackgroundSize, BackgroundSize>
	{
	}

	private readonly List<IProperty<InlineStyleAccess>> m_PropertiesList;

	private readonly Dictionary<string, IProperty<InlineStyleAccess>> m_PropertiesHash;

	public InlineStyleAccessPropertyBag()
	{
		m_PropertiesList = new List<IProperty<InlineStyleAccess>>(85);
		m_PropertiesHash = new Dictionary<string, IProperty<InlineStyleAccess>>(255);
		AddProperty(new AlignContentProperty());
		AddProperty(new AlignItemsProperty());
		AddProperty(new AlignSelfProperty());
		AddProperty(new BackgroundColorProperty());
		AddProperty(new BackgroundImageProperty());
		AddProperty(new BackgroundPositionXProperty());
		AddProperty(new BackgroundPositionYProperty());
		AddProperty(new BackgroundRepeatProperty());
		AddProperty(new BackgroundSizeProperty());
		AddProperty(new BorderBottomColorProperty());
		AddProperty(new BorderBottomLeftRadiusProperty());
		AddProperty(new BorderBottomRightRadiusProperty());
		AddProperty(new BorderBottomWidthProperty());
		AddProperty(new BorderLeftColorProperty());
		AddProperty(new BorderLeftWidthProperty());
		AddProperty(new BorderRightColorProperty());
		AddProperty(new BorderRightWidthProperty());
		AddProperty(new BorderTopColorProperty());
		AddProperty(new BorderTopLeftRadiusProperty());
		AddProperty(new BorderTopRightRadiusProperty());
		AddProperty(new BorderTopWidthProperty());
		AddProperty(new BottomProperty());
		AddProperty(new ColorProperty());
		AddProperty(new CursorProperty());
		AddProperty(new DisplayProperty());
		AddProperty(new FlexBasisProperty());
		AddProperty(new FlexDirectionProperty());
		AddProperty(new FlexGrowProperty());
		AddProperty(new FlexShrinkProperty());
		AddProperty(new FlexWrapProperty());
		AddProperty(new FontSizeProperty());
		AddProperty(new HeightProperty());
		AddProperty(new JustifyContentProperty());
		AddProperty(new LeftProperty());
		AddProperty(new LetterSpacingProperty());
		AddProperty(new MarginBottomProperty());
		AddProperty(new MarginLeftProperty());
		AddProperty(new MarginRightProperty());
		AddProperty(new MarginTopProperty());
		AddProperty(new MaxHeightProperty());
		AddProperty(new MaxWidthProperty());
		AddProperty(new MinHeightProperty());
		AddProperty(new MinWidthProperty());
		AddProperty(new OpacityProperty());
		AddProperty(new OverflowProperty());
		AddProperty(new PaddingBottomProperty());
		AddProperty(new PaddingLeftProperty());
		AddProperty(new PaddingRightProperty());
		AddProperty(new PaddingTopProperty());
		AddProperty(new PositionProperty());
		AddProperty(new RightProperty());
		AddProperty(new RotateProperty());
		AddProperty(new ScaleProperty());
		AddProperty(new TextOverflowProperty());
		AddProperty(new TextShadowProperty());
		AddProperty(new TopProperty());
		AddProperty(new TransformOriginProperty());
		AddProperty(new TransitionDelayProperty());
		AddProperty(new TransitionDurationProperty());
		AddProperty(new TransitionPropertyProperty());
		AddProperty(new TransitionTimingFunctionProperty());
		AddProperty(new TranslateProperty());
		AddProperty(new UnityBackgroundImageTintColorProperty());
		AddProperty(new UnityEditorTextRenderingModeProperty());
		AddProperty(new UnityFontProperty());
		AddProperty(new UnityFontDefinitionProperty());
		AddProperty(new UnityFontStyleAndWeightProperty());
		AddProperty(new UnityOverflowClipBoxProperty());
		AddProperty(new UnityParagraphSpacingProperty());
		AddProperty(new UnitySliceBottomProperty());
		AddProperty(new UnitySliceLeftProperty());
		AddProperty(new UnitySliceRightProperty());
		AddProperty(new UnitySliceScaleProperty());
		AddProperty(new UnitySliceTopProperty());
		AddProperty(new UnitySliceTypeProperty());
		AddProperty(new UnityTextAlignProperty());
		AddProperty(new UnityTextAutoSizeProperty());
		AddProperty(new UnityTextGeneratorProperty());
		AddProperty(new UnityTextOutlineColorProperty());
		AddProperty(new UnityTextOutlineWidthProperty());
		AddProperty(new UnityTextOverflowPositionProperty());
		AddProperty(new VisibilityProperty());
		AddProperty(new WhiteSpaceProperty());
		AddProperty(new WidthProperty());
		AddProperty(new WordSpacingProperty());
	}

	private void AddProperty<TStyleValue, TValue>(InlineStyleProperty<TStyleValue, TValue> property) where TStyleValue : IStyleValue<TValue>, new()
	{
		m_PropertiesList.Add(property);
		m_PropertiesHash.Add(property.Name, property);
		if (string.CompareOrdinal(property.Name, property.ussName) != 0)
		{
			m_PropertiesHash.Add(property.ussName, property);
		}
	}

	public override PropertyCollection<InlineStyleAccess> GetProperties()
	{
		return new PropertyCollection<InlineStyleAccess>(m_PropertiesList);
	}

	public override PropertyCollection<InlineStyleAccess> GetProperties(ref InlineStyleAccess container)
	{
		return new PropertyCollection<InlineStyleAccess>(m_PropertiesList);
	}

	public bool TryGetProperty(ref InlineStyleAccess container, string name, out IProperty<InlineStyleAccess> property)
	{
		return m_PropertiesHash.TryGetValue(name, out property);
	}
}
