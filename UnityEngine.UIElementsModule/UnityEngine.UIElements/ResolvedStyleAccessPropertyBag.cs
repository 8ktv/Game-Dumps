using System;
using System.Collections.Generic;
using Unity.Properties;

namespace UnityEngine.UIElements;

internal class ResolvedStyleAccessPropertyBag : PropertyBag<ResolvedStyleAccess>, INamedProperties<ResolvedStyleAccess>
{
	private class AlignContentProperty : ResolvedEnumProperty<Align>
	{
		public override string Name => "alignContent";

		public override string ussName => "align-content";

		public override bool IsReadOnly => true;

		public override Align GetValue(ref ResolvedStyleAccess container)
		{
			return ((IResolvedStyle)container).alignContent;
		}

		public override void SetValue(ref ResolvedStyleAccess container, Align value)
		{
			throw new InvalidOperationException();
		}
	}

	private class AlignItemsProperty : ResolvedEnumProperty<Align>
	{
		public override string Name => "alignItems";

		public override string ussName => "align-items";

		public override bool IsReadOnly => true;

		public override Align GetValue(ref ResolvedStyleAccess container)
		{
			return ((IResolvedStyle)container).alignItems;
		}

		public override void SetValue(ref ResolvedStyleAccess container, Align value)
		{
			throw new InvalidOperationException();
		}
	}

	private class AlignSelfProperty : ResolvedEnumProperty<Align>
	{
		public override string Name => "alignSelf";

		public override string ussName => "align-self";

		public override bool IsReadOnly => true;

		public override Align GetValue(ref ResolvedStyleAccess container)
		{
			return ((IResolvedStyle)container).alignSelf;
		}

		public override void SetValue(ref ResolvedStyleAccess container, Align value)
		{
			throw new InvalidOperationException();
		}
	}

	private class BackgroundColorProperty : ResolvedColorProperty
	{
		public override string Name => "backgroundColor";

		public override string ussName => "background-color";

		public override bool IsReadOnly => true;

		public override Color GetValue(ref ResolvedStyleAccess container)
		{
			return ((IResolvedStyle)container).backgroundColor;
		}

		public override void SetValue(ref ResolvedStyleAccess container, Color value)
		{
			throw new InvalidOperationException();
		}
	}

	private class BackgroundImageProperty : ResolvedBackgroundProperty
	{
		public override string Name => "backgroundImage";

		public override string ussName => "background-image";

		public override bool IsReadOnly => true;

		public override Background GetValue(ref ResolvedStyleAccess container)
		{
			return ((IResolvedStyle)container).backgroundImage;
		}

		public override void SetValue(ref ResolvedStyleAccess container, Background value)
		{
			throw new InvalidOperationException();
		}
	}

	private class BackgroundPositionXProperty : ResolvedBackgroundPositionProperty
	{
		public override string Name => "backgroundPositionX";

		public override string ussName => "background-position-x";

		public override bool IsReadOnly => true;

		public override BackgroundPosition GetValue(ref ResolvedStyleAccess container)
		{
			return ((IResolvedStyle)container).backgroundPositionX;
		}

		public override void SetValue(ref ResolvedStyleAccess container, BackgroundPosition value)
		{
			throw new InvalidOperationException();
		}
	}

	private class BackgroundPositionYProperty : ResolvedBackgroundPositionProperty
	{
		public override string Name => "backgroundPositionY";

		public override string ussName => "background-position-y";

		public override bool IsReadOnly => true;

		public override BackgroundPosition GetValue(ref ResolvedStyleAccess container)
		{
			return ((IResolvedStyle)container).backgroundPositionY;
		}

		public override void SetValue(ref ResolvedStyleAccess container, BackgroundPosition value)
		{
			throw new InvalidOperationException();
		}
	}

	private class BackgroundRepeatProperty : ResolvedBackgroundRepeatProperty
	{
		public override string Name => "backgroundRepeat";

		public override string ussName => "background-repeat";

		public override bool IsReadOnly => true;

		public override BackgroundRepeat GetValue(ref ResolvedStyleAccess container)
		{
			return ((IResolvedStyle)container).backgroundRepeat;
		}

		public override void SetValue(ref ResolvedStyleAccess container, BackgroundRepeat value)
		{
			throw new InvalidOperationException();
		}
	}

	private class BackgroundSizeProperty : ResolvedBackgroundSizeProperty
	{
		public override string Name => "backgroundSize";

		public override string ussName => "background-size";

		public override bool IsReadOnly => true;

		public override BackgroundSize GetValue(ref ResolvedStyleAccess container)
		{
			return ((IResolvedStyle)container).backgroundSize;
		}

		public override void SetValue(ref ResolvedStyleAccess container, BackgroundSize value)
		{
			throw new InvalidOperationException();
		}
	}

	private class BorderBottomColorProperty : ResolvedColorProperty
	{
		public override string Name => "borderBottomColor";

		public override string ussName => "border-bottom-color";

		public override bool IsReadOnly => true;

		public override Color GetValue(ref ResolvedStyleAccess container)
		{
			return ((IResolvedStyle)container).borderBottomColor;
		}

		public override void SetValue(ref ResolvedStyleAccess container, Color value)
		{
			throw new InvalidOperationException();
		}
	}

	private class BorderBottomLeftRadiusProperty : ResolvedFloatProperty
	{
		public override string Name => "borderBottomLeftRadius";

		public override string ussName => "border-bottom-left-radius";

		public override bool IsReadOnly => true;

		public override float GetValue(ref ResolvedStyleAccess container)
		{
			return ((IResolvedStyle)container).borderBottomLeftRadius;
		}

		public override void SetValue(ref ResolvedStyleAccess container, float value)
		{
			throw new InvalidOperationException();
		}
	}

	private class BorderBottomRightRadiusProperty : ResolvedFloatProperty
	{
		public override string Name => "borderBottomRightRadius";

		public override string ussName => "border-bottom-right-radius";

		public override bool IsReadOnly => true;

		public override float GetValue(ref ResolvedStyleAccess container)
		{
			return ((IResolvedStyle)container).borderBottomRightRadius;
		}

		public override void SetValue(ref ResolvedStyleAccess container, float value)
		{
			throw new InvalidOperationException();
		}
	}

	private class BorderBottomWidthProperty : ResolvedFloatProperty
	{
		public override string Name => "borderBottomWidth";

		public override string ussName => "border-bottom-width";

		public override bool IsReadOnly => true;

		public override float GetValue(ref ResolvedStyleAccess container)
		{
			return ((IResolvedStyle)container).borderBottomWidth;
		}

		public override void SetValue(ref ResolvedStyleAccess container, float value)
		{
			throw new InvalidOperationException();
		}
	}

	private class BorderLeftColorProperty : ResolvedColorProperty
	{
		public override string Name => "borderLeftColor";

		public override string ussName => "border-left-color";

		public override bool IsReadOnly => true;

		public override Color GetValue(ref ResolvedStyleAccess container)
		{
			return ((IResolvedStyle)container).borderLeftColor;
		}

		public override void SetValue(ref ResolvedStyleAccess container, Color value)
		{
			throw new InvalidOperationException();
		}
	}

	private class BorderLeftWidthProperty : ResolvedFloatProperty
	{
		public override string Name => "borderLeftWidth";

		public override string ussName => "border-left-width";

		public override bool IsReadOnly => true;

		public override float GetValue(ref ResolvedStyleAccess container)
		{
			return ((IResolvedStyle)container).borderLeftWidth;
		}

		public override void SetValue(ref ResolvedStyleAccess container, float value)
		{
			throw new InvalidOperationException();
		}
	}

	private class BorderRightColorProperty : ResolvedColorProperty
	{
		public override string Name => "borderRightColor";

		public override string ussName => "border-right-color";

		public override bool IsReadOnly => true;

		public override Color GetValue(ref ResolvedStyleAccess container)
		{
			return ((IResolvedStyle)container).borderRightColor;
		}

		public override void SetValue(ref ResolvedStyleAccess container, Color value)
		{
			throw new InvalidOperationException();
		}
	}

	private class BorderRightWidthProperty : ResolvedFloatProperty
	{
		public override string Name => "borderRightWidth";

		public override string ussName => "border-right-width";

		public override bool IsReadOnly => true;

		public override float GetValue(ref ResolvedStyleAccess container)
		{
			return ((IResolvedStyle)container).borderRightWidth;
		}

		public override void SetValue(ref ResolvedStyleAccess container, float value)
		{
			throw new InvalidOperationException();
		}
	}

	private class BorderTopColorProperty : ResolvedColorProperty
	{
		public override string Name => "borderTopColor";

		public override string ussName => "border-top-color";

		public override bool IsReadOnly => true;

		public override Color GetValue(ref ResolvedStyleAccess container)
		{
			return ((IResolvedStyle)container).borderTopColor;
		}

		public override void SetValue(ref ResolvedStyleAccess container, Color value)
		{
			throw new InvalidOperationException();
		}
	}

	private class BorderTopLeftRadiusProperty : ResolvedFloatProperty
	{
		public override string Name => "borderTopLeftRadius";

		public override string ussName => "border-top-left-radius";

		public override bool IsReadOnly => true;

		public override float GetValue(ref ResolvedStyleAccess container)
		{
			return ((IResolvedStyle)container).borderTopLeftRadius;
		}

		public override void SetValue(ref ResolvedStyleAccess container, float value)
		{
			throw new InvalidOperationException();
		}
	}

	private class BorderTopRightRadiusProperty : ResolvedFloatProperty
	{
		public override string Name => "borderTopRightRadius";

		public override string ussName => "border-top-right-radius";

		public override bool IsReadOnly => true;

		public override float GetValue(ref ResolvedStyleAccess container)
		{
			return ((IResolvedStyle)container).borderTopRightRadius;
		}

		public override void SetValue(ref ResolvedStyleAccess container, float value)
		{
			throw new InvalidOperationException();
		}
	}

	private class BorderTopWidthProperty : ResolvedFloatProperty
	{
		public override string Name => "borderTopWidth";

		public override string ussName => "border-top-width";

		public override bool IsReadOnly => true;

		public override float GetValue(ref ResolvedStyleAccess container)
		{
			return ((IResolvedStyle)container).borderTopWidth;
		}

		public override void SetValue(ref ResolvedStyleAccess container, float value)
		{
			throw new InvalidOperationException();
		}
	}

	private class BottomProperty : ResolvedFloatProperty
	{
		public override string Name => "bottom";

		public override string ussName => "bottom";

		public override bool IsReadOnly => true;

		public override float GetValue(ref ResolvedStyleAccess container)
		{
			return ((IResolvedStyle)container).bottom;
		}

		public override void SetValue(ref ResolvedStyleAccess container, float value)
		{
			throw new InvalidOperationException();
		}
	}

	private class ColorProperty : ResolvedColorProperty
	{
		public override string Name => "color";

		public override string ussName => "color";

		public override bool IsReadOnly => true;

		public override Color GetValue(ref ResolvedStyleAccess container)
		{
			return ((IResolvedStyle)container).color;
		}

		public override void SetValue(ref ResolvedStyleAccess container, Color value)
		{
			throw new InvalidOperationException();
		}
	}

	private class DisplayProperty : ResolvedEnumProperty<DisplayStyle>
	{
		public override string Name => "display";

		public override string ussName => "display";

		public override bool IsReadOnly => true;

		public override DisplayStyle GetValue(ref ResolvedStyleAccess container)
		{
			return ((IResolvedStyle)container).display;
		}

		public override void SetValue(ref ResolvedStyleAccess container, DisplayStyle value)
		{
			throw new InvalidOperationException();
		}
	}

	private class FlexBasisProperty : ResolvedStyleFloatProperty
	{
		public override string Name => "flexBasis";

		public override string ussName => "flex-basis";

		public override bool IsReadOnly => true;

		public override StyleFloat GetValue(ref ResolvedStyleAccess container)
		{
			return ((IResolvedStyle)container).flexBasis;
		}

		public override void SetValue(ref ResolvedStyleAccess container, StyleFloat value)
		{
			throw new InvalidOperationException();
		}
	}

	private class FlexDirectionProperty : ResolvedEnumProperty<FlexDirection>
	{
		public override string Name => "flexDirection";

		public override string ussName => "flex-direction";

		public override bool IsReadOnly => true;

		public override FlexDirection GetValue(ref ResolvedStyleAccess container)
		{
			return ((IResolvedStyle)container).flexDirection;
		}

		public override void SetValue(ref ResolvedStyleAccess container, FlexDirection value)
		{
			throw new InvalidOperationException();
		}
	}

	private class FlexGrowProperty : ResolvedFloatProperty
	{
		public override string Name => "flexGrow";

		public override string ussName => "flex-grow";

		public override bool IsReadOnly => true;

		public override float GetValue(ref ResolvedStyleAccess container)
		{
			return ((IResolvedStyle)container).flexGrow;
		}

		public override void SetValue(ref ResolvedStyleAccess container, float value)
		{
			throw new InvalidOperationException();
		}
	}

	private class FlexShrinkProperty : ResolvedFloatProperty
	{
		public override string Name => "flexShrink";

		public override string ussName => "flex-shrink";

		public override bool IsReadOnly => true;

		public override float GetValue(ref ResolvedStyleAccess container)
		{
			return ((IResolvedStyle)container).flexShrink;
		}

		public override void SetValue(ref ResolvedStyleAccess container, float value)
		{
			throw new InvalidOperationException();
		}
	}

	private class FlexWrapProperty : ResolvedEnumProperty<Wrap>
	{
		public override string Name => "flexWrap";

		public override string ussName => "flex-wrap";

		public override bool IsReadOnly => true;

		public override Wrap GetValue(ref ResolvedStyleAccess container)
		{
			return ((IResolvedStyle)container).flexWrap;
		}

		public override void SetValue(ref ResolvedStyleAccess container, Wrap value)
		{
			throw new InvalidOperationException();
		}
	}

	private class FontSizeProperty : ResolvedFloatProperty
	{
		public override string Name => "fontSize";

		public override string ussName => "font-size";

		public override bool IsReadOnly => true;

		public override float GetValue(ref ResolvedStyleAccess container)
		{
			return ((IResolvedStyle)container).fontSize;
		}

		public override void SetValue(ref ResolvedStyleAccess container, float value)
		{
			throw new InvalidOperationException();
		}
	}

	private class HeightProperty : ResolvedFloatProperty
	{
		public override string Name => "height";

		public override string ussName => "height";

		public override bool IsReadOnly => true;

		public override float GetValue(ref ResolvedStyleAccess container)
		{
			return ((IResolvedStyle)container).height;
		}

		public override void SetValue(ref ResolvedStyleAccess container, float value)
		{
			throw new InvalidOperationException();
		}
	}

	private class JustifyContentProperty : ResolvedEnumProperty<Justify>
	{
		public override string Name => "justifyContent";

		public override string ussName => "justify-content";

		public override bool IsReadOnly => true;

		public override Justify GetValue(ref ResolvedStyleAccess container)
		{
			return ((IResolvedStyle)container).justifyContent;
		}

		public override void SetValue(ref ResolvedStyleAccess container, Justify value)
		{
			throw new InvalidOperationException();
		}
	}

	private class LeftProperty : ResolvedFloatProperty
	{
		public override string Name => "left";

		public override string ussName => "left";

		public override bool IsReadOnly => true;

		public override float GetValue(ref ResolvedStyleAccess container)
		{
			return ((IResolvedStyle)container).left;
		}

		public override void SetValue(ref ResolvedStyleAccess container, float value)
		{
			throw new InvalidOperationException();
		}
	}

	private class LetterSpacingProperty : ResolvedFloatProperty
	{
		public override string Name => "letterSpacing";

		public override string ussName => "letter-spacing";

		public override bool IsReadOnly => true;

		public override float GetValue(ref ResolvedStyleAccess container)
		{
			return ((IResolvedStyle)container).letterSpacing;
		}

		public override void SetValue(ref ResolvedStyleAccess container, float value)
		{
			throw new InvalidOperationException();
		}
	}

	private class MarginBottomProperty : ResolvedFloatProperty
	{
		public override string Name => "marginBottom";

		public override string ussName => "margin-bottom";

		public override bool IsReadOnly => true;

		public override float GetValue(ref ResolvedStyleAccess container)
		{
			return ((IResolvedStyle)container).marginBottom;
		}

		public override void SetValue(ref ResolvedStyleAccess container, float value)
		{
			throw new InvalidOperationException();
		}
	}

	private class MarginLeftProperty : ResolvedFloatProperty
	{
		public override string Name => "marginLeft";

		public override string ussName => "margin-left";

		public override bool IsReadOnly => true;

		public override float GetValue(ref ResolvedStyleAccess container)
		{
			return ((IResolvedStyle)container).marginLeft;
		}

		public override void SetValue(ref ResolvedStyleAccess container, float value)
		{
			throw new InvalidOperationException();
		}
	}

	private class MarginRightProperty : ResolvedFloatProperty
	{
		public override string Name => "marginRight";

		public override string ussName => "margin-right";

		public override bool IsReadOnly => true;

		public override float GetValue(ref ResolvedStyleAccess container)
		{
			return ((IResolvedStyle)container).marginRight;
		}

		public override void SetValue(ref ResolvedStyleAccess container, float value)
		{
			throw new InvalidOperationException();
		}
	}

	private class MarginTopProperty : ResolvedFloatProperty
	{
		public override string Name => "marginTop";

		public override string ussName => "margin-top";

		public override bool IsReadOnly => true;

		public override float GetValue(ref ResolvedStyleAccess container)
		{
			return ((IResolvedStyle)container).marginTop;
		}

		public override void SetValue(ref ResolvedStyleAccess container, float value)
		{
			throw new InvalidOperationException();
		}
	}

	private class MaxHeightProperty : ResolvedStyleFloatProperty
	{
		public override string Name => "maxHeight";

		public override string ussName => "max-height";

		public override bool IsReadOnly => true;

		public override StyleFloat GetValue(ref ResolvedStyleAccess container)
		{
			return ((IResolvedStyle)container).maxHeight;
		}

		public override void SetValue(ref ResolvedStyleAccess container, StyleFloat value)
		{
			throw new InvalidOperationException();
		}
	}

	private class MaxWidthProperty : ResolvedStyleFloatProperty
	{
		public override string Name => "maxWidth";

		public override string ussName => "max-width";

		public override bool IsReadOnly => true;

		public override StyleFloat GetValue(ref ResolvedStyleAccess container)
		{
			return ((IResolvedStyle)container).maxWidth;
		}

		public override void SetValue(ref ResolvedStyleAccess container, StyleFloat value)
		{
			throw new InvalidOperationException();
		}
	}

	private class MinHeightProperty : ResolvedStyleFloatProperty
	{
		public override string Name => "minHeight";

		public override string ussName => "min-height";

		public override bool IsReadOnly => true;

		public override StyleFloat GetValue(ref ResolvedStyleAccess container)
		{
			return ((IResolvedStyle)container).minHeight;
		}

		public override void SetValue(ref ResolvedStyleAccess container, StyleFloat value)
		{
			throw new InvalidOperationException();
		}
	}

	private class MinWidthProperty : ResolvedStyleFloatProperty
	{
		public override string Name => "minWidth";

		public override string ussName => "min-width";

		public override bool IsReadOnly => true;

		public override StyleFloat GetValue(ref ResolvedStyleAccess container)
		{
			return ((IResolvedStyle)container).minWidth;
		}

		public override void SetValue(ref ResolvedStyleAccess container, StyleFloat value)
		{
			throw new InvalidOperationException();
		}
	}

	private class OpacityProperty : ResolvedFloatProperty
	{
		public override string Name => "opacity";

		public override string ussName => "opacity";

		public override bool IsReadOnly => true;

		public override float GetValue(ref ResolvedStyleAccess container)
		{
			return ((IResolvedStyle)container).opacity;
		}

		public override void SetValue(ref ResolvedStyleAccess container, float value)
		{
			throw new InvalidOperationException();
		}
	}

	private class PaddingBottomProperty : ResolvedFloatProperty
	{
		public override string Name => "paddingBottom";

		public override string ussName => "padding-bottom";

		public override bool IsReadOnly => true;

		public override float GetValue(ref ResolvedStyleAccess container)
		{
			return ((IResolvedStyle)container).paddingBottom;
		}

		public override void SetValue(ref ResolvedStyleAccess container, float value)
		{
			throw new InvalidOperationException();
		}
	}

	private class PaddingLeftProperty : ResolvedFloatProperty
	{
		public override string Name => "paddingLeft";

		public override string ussName => "padding-left";

		public override bool IsReadOnly => true;

		public override float GetValue(ref ResolvedStyleAccess container)
		{
			return ((IResolvedStyle)container).paddingLeft;
		}

		public override void SetValue(ref ResolvedStyleAccess container, float value)
		{
			throw new InvalidOperationException();
		}
	}

	private class PaddingRightProperty : ResolvedFloatProperty
	{
		public override string Name => "paddingRight";

		public override string ussName => "padding-right";

		public override bool IsReadOnly => true;

		public override float GetValue(ref ResolvedStyleAccess container)
		{
			return ((IResolvedStyle)container).paddingRight;
		}

		public override void SetValue(ref ResolvedStyleAccess container, float value)
		{
			throw new InvalidOperationException();
		}
	}

	private class PaddingTopProperty : ResolvedFloatProperty
	{
		public override string Name => "paddingTop";

		public override string ussName => "padding-top";

		public override bool IsReadOnly => true;

		public override float GetValue(ref ResolvedStyleAccess container)
		{
			return ((IResolvedStyle)container).paddingTop;
		}

		public override void SetValue(ref ResolvedStyleAccess container, float value)
		{
			throw new InvalidOperationException();
		}
	}

	private class PositionProperty : ResolvedEnumProperty<Position>
	{
		public override string Name => "position";

		public override string ussName => "position";

		public override bool IsReadOnly => true;

		public override Position GetValue(ref ResolvedStyleAccess container)
		{
			return ((IResolvedStyle)container).position;
		}

		public override void SetValue(ref ResolvedStyleAccess container, Position value)
		{
			throw new InvalidOperationException();
		}
	}

	private class RightProperty : ResolvedFloatProperty
	{
		public override string Name => "right";

		public override string ussName => "right";

		public override bool IsReadOnly => true;

		public override float GetValue(ref ResolvedStyleAccess container)
		{
			return ((IResolvedStyle)container).right;
		}

		public override void SetValue(ref ResolvedStyleAccess container, float value)
		{
			throw new InvalidOperationException();
		}
	}

	private class RotateProperty : ResolvedRotateProperty
	{
		public override string Name => "rotate";

		public override string ussName => "rotate";

		public override bool IsReadOnly => true;

		public override Rotate GetValue(ref ResolvedStyleAccess container)
		{
			return ((IResolvedStyle)container).rotate;
		}

		public override void SetValue(ref ResolvedStyleAccess container, Rotate value)
		{
			throw new InvalidOperationException();
		}
	}

	private class ScaleProperty : ResolvedScaleProperty
	{
		public override string Name => "scale";

		public override string ussName => "scale";

		public override bool IsReadOnly => true;

		public override Scale GetValue(ref ResolvedStyleAccess container)
		{
			return ((IResolvedStyle)container).scale;
		}

		public override void SetValue(ref ResolvedStyleAccess container, Scale value)
		{
			throw new InvalidOperationException();
		}
	}

	private class TextOverflowProperty : ResolvedEnumProperty<TextOverflow>
	{
		public override string Name => "textOverflow";

		public override string ussName => "text-overflow";

		public override bool IsReadOnly => true;

		public override TextOverflow GetValue(ref ResolvedStyleAccess container)
		{
			return ((IResolvedStyle)container).textOverflow;
		}

		public override void SetValue(ref ResolvedStyleAccess container, TextOverflow value)
		{
			throw new InvalidOperationException();
		}
	}

	private class TopProperty : ResolvedFloatProperty
	{
		public override string Name => "top";

		public override string ussName => "top";

		public override bool IsReadOnly => true;

		public override float GetValue(ref ResolvedStyleAccess container)
		{
			return ((IResolvedStyle)container).top;
		}

		public override void SetValue(ref ResolvedStyleAccess container, float value)
		{
			throw new InvalidOperationException();
		}
	}

	private class TransformOriginProperty : ResolvedVector3Property
	{
		public override string Name => "transformOrigin";

		public override string ussName => "transform-origin";

		public override bool IsReadOnly => true;

		public override Vector3 GetValue(ref ResolvedStyleAccess container)
		{
			return ((IResolvedStyle)container).transformOrigin;
		}

		public override void SetValue(ref ResolvedStyleAccess container, Vector3 value)
		{
			throw new InvalidOperationException();
		}
	}

	private class TransitionDelayProperty : ResolvedListProperty<TimeValue>
	{
		public override string Name => "transitionDelay";

		public override string ussName => "transition-delay";

		public override bool IsReadOnly => true;

		public override IEnumerable<TimeValue> GetValue(ref ResolvedStyleAccess container)
		{
			return ((IResolvedStyle)container).transitionDelay;
		}

		public override void SetValue(ref ResolvedStyleAccess container, IEnumerable<TimeValue> value)
		{
			throw new InvalidOperationException();
		}
	}

	private class TransitionDurationProperty : ResolvedListProperty<TimeValue>
	{
		public override string Name => "transitionDuration";

		public override string ussName => "transition-duration";

		public override bool IsReadOnly => true;

		public override IEnumerable<TimeValue> GetValue(ref ResolvedStyleAccess container)
		{
			return ((IResolvedStyle)container).transitionDuration;
		}

		public override void SetValue(ref ResolvedStyleAccess container, IEnumerable<TimeValue> value)
		{
			throw new InvalidOperationException();
		}
	}

	private class TransitionPropertyProperty : ResolvedListProperty<StylePropertyName>
	{
		public override string Name => "transitionProperty";

		public override string ussName => "transition-property";

		public override bool IsReadOnly => true;

		public override IEnumerable<StylePropertyName> GetValue(ref ResolvedStyleAccess container)
		{
			return ((IResolvedStyle)container).transitionProperty;
		}

		public override void SetValue(ref ResolvedStyleAccess container, IEnumerable<StylePropertyName> value)
		{
			throw new InvalidOperationException();
		}
	}

	private class TransitionTimingFunctionProperty : ResolvedListProperty<EasingFunction>
	{
		public override string Name => "transitionTimingFunction";

		public override string ussName => "transition-timing-function";

		public override bool IsReadOnly => true;

		public override IEnumerable<EasingFunction> GetValue(ref ResolvedStyleAccess container)
		{
			return ((IResolvedStyle)container).transitionTimingFunction;
		}

		public override void SetValue(ref ResolvedStyleAccess container, IEnumerable<EasingFunction> value)
		{
			throw new InvalidOperationException();
		}
	}

	private class TranslateProperty : ResolvedVector3Property
	{
		public override string Name => "translate";

		public override string ussName => "translate";

		public override bool IsReadOnly => true;

		public override Vector3 GetValue(ref ResolvedStyleAccess container)
		{
			return ((IResolvedStyle)container).translate;
		}

		public override void SetValue(ref ResolvedStyleAccess container, Vector3 value)
		{
			throw new InvalidOperationException();
		}
	}

	private class UnityBackgroundImageTintColorProperty : ResolvedColorProperty
	{
		public override string Name => "unityBackgroundImageTintColor";

		public override string ussName => "-unity-background-image-tint-color";

		public override bool IsReadOnly => true;

		public override Color GetValue(ref ResolvedStyleAccess container)
		{
			return ((IResolvedStyle)container).unityBackgroundImageTintColor;
		}

		public override void SetValue(ref ResolvedStyleAccess container, Color value)
		{
			throw new InvalidOperationException();
		}
	}

	private class UnityEditorTextRenderingModeProperty : ResolvedEnumProperty<EditorTextRenderingMode>
	{
		public override string Name => "unityEditorTextRenderingMode";

		public override string ussName => "-unity-editor-text-rendering-mode";

		public override bool IsReadOnly => true;

		public override EditorTextRenderingMode GetValue(ref ResolvedStyleAccess container)
		{
			return ((IResolvedStyle)container).unityEditorTextRenderingMode;
		}

		public override void SetValue(ref ResolvedStyleAccess container, EditorTextRenderingMode value)
		{
			throw new InvalidOperationException();
		}
	}

	private class UnityFontProperty : ResolvedFontProperty
	{
		public override string Name => "unityFont";

		public override string ussName => "-unity-font";

		public override bool IsReadOnly => true;

		public override Font GetValue(ref ResolvedStyleAccess container)
		{
			return ((IResolvedStyle)container).unityFont;
		}

		public override void SetValue(ref ResolvedStyleAccess container, Font value)
		{
			throw new InvalidOperationException();
		}
	}

	private class UnityFontDefinitionProperty : ResolvedFontDefinitionProperty
	{
		public override string Name => "unityFontDefinition";

		public override string ussName => "-unity-font-definition";

		public override bool IsReadOnly => true;

		public override FontDefinition GetValue(ref ResolvedStyleAccess container)
		{
			return ((IResolvedStyle)container).unityFontDefinition;
		}

		public override void SetValue(ref ResolvedStyleAccess container, FontDefinition value)
		{
			throw new InvalidOperationException();
		}
	}

	private class UnityFontStyleAndWeightProperty : ResolvedEnumProperty<FontStyle>
	{
		public override string Name => "unityFontStyleAndWeight";

		public override string ussName => "-unity-font-style";

		public override bool IsReadOnly => true;

		public override FontStyle GetValue(ref ResolvedStyleAccess container)
		{
			return ((IResolvedStyle)container).unityFontStyleAndWeight;
		}

		public override void SetValue(ref ResolvedStyleAccess container, FontStyle value)
		{
			throw new InvalidOperationException();
		}
	}

	private class UnityParagraphSpacingProperty : ResolvedFloatProperty
	{
		public override string Name => "unityParagraphSpacing";

		public override string ussName => "-unity-paragraph-spacing";

		public override bool IsReadOnly => true;

		public override float GetValue(ref ResolvedStyleAccess container)
		{
			return ((IResolvedStyle)container).unityParagraphSpacing;
		}

		public override void SetValue(ref ResolvedStyleAccess container, float value)
		{
			throw new InvalidOperationException();
		}
	}

	private class UnitySliceBottomProperty : ResolvedIntProperty
	{
		public override string Name => "unitySliceBottom";

		public override string ussName => "-unity-slice-bottom";

		public override bool IsReadOnly => true;

		public override int GetValue(ref ResolvedStyleAccess container)
		{
			return ((IResolvedStyle)container).unitySliceBottom;
		}

		public override void SetValue(ref ResolvedStyleAccess container, int value)
		{
			throw new InvalidOperationException();
		}
	}

	private class UnitySliceLeftProperty : ResolvedIntProperty
	{
		public override string Name => "unitySliceLeft";

		public override string ussName => "-unity-slice-left";

		public override bool IsReadOnly => true;

		public override int GetValue(ref ResolvedStyleAccess container)
		{
			return ((IResolvedStyle)container).unitySliceLeft;
		}

		public override void SetValue(ref ResolvedStyleAccess container, int value)
		{
			throw new InvalidOperationException();
		}
	}

	private class UnitySliceRightProperty : ResolvedIntProperty
	{
		public override string Name => "unitySliceRight";

		public override string ussName => "-unity-slice-right";

		public override bool IsReadOnly => true;

		public override int GetValue(ref ResolvedStyleAccess container)
		{
			return ((IResolvedStyle)container).unitySliceRight;
		}

		public override void SetValue(ref ResolvedStyleAccess container, int value)
		{
			throw new InvalidOperationException();
		}
	}

	private class UnitySliceScaleProperty : ResolvedFloatProperty
	{
		public override string Name => "unitySliceScale";

		public override string ussName => "-unity-slice-scale";

		public override bool IsReadOnly => true;

		public override float GetValue(ref ResolvedStyleAccess container)
		{
			return ((IResolvedStyle)container).unitySliceScale;
		}

		public override void SetValue(ref ResolvedStyleAccess container, float value)
		{
			throw new InvalidOperationException();
		}
	}

	private class UnitySliceTopProperty : ResolvedIntProperty
	{
		public override string Name => "unitySliceTop";

		public override string ussName => "-unity-slice-top";

		public override bool IsReadOnly => true;

		public override int GetValue(ref ResolvedStyleAccess container)
		{
			return ((IResolvedStyle)container).unitySliceTop;
		}

		public override void SetValue(ref ResolvedStyleAccess container, int value)
		{
			throw new InvalidOperationException();
		}
	}

	private class UnitySliceTypeProperty : ResolvedEnumProperty<SliceType>
	{
		public override string Name => "unitySliceType";

		public override string ussName => "-unity-slice-type";

		public override bool IsReadOnly => true;

		public override SliceType GetValue(ref ResolvedStyleAccess container)
		{
			return ((IResolvedStyle)container).unitySliceType;
		}

		public override void SetValue(ref ResolvedStyleAccess container, SliceType value)
		{
			throw new InvalidOperationException();
		}
	}

	private class UnityTextAlignProperty : ResolvedEnumProperty<TextAnchor>
	{
		public override string Name => "unityTextAlign";

		public override string ussName => "-unity-text-align";

		public override bool IsReadOnly => true;

		public override TextAnchor GetValue(ref ResolvedStyleAccess container)
		{
			return ((IResolvedStyle)container).unityTextAlign;
		}

		public override void SetValue(ref ResolvedStyleAccess container, TextAnchor value)
		{
			throw new InvalidOperationException();
		}
	}

	private class UnityTextGeneratorProperty : ResolvedEnumProperty<TextGeneratorType>
	{
		public override string Name => "unityTextGenerator";

		public override string ussName => "-unity-text-generator";

		public override bool IsReadOnly => true;

		public override TextGeneratorType GetValue(ref ResolvedStyleAccess container)
		{
			return ((IResolvedStyle)container).unityTextGenerator;
		}

		public override void SetValue(ref ResolvedStyleAccess container, TextGeneratorType value)
		{
			throw new InvalidOperationException();
		}
	}

	private class UnityTextOutlineColorProperty : ResolvedColorProperty
	{
		public override string Name => "unityTextOutlineColor";

		public override string ussName => "-unity-text-outline-color";

		public override bool IsReadOnly => true;

		public override Color GetValue(ref ResolvedStyleAccess container)
		{
			return ((IResolvedStyle)container).unityTextOutlineColor;
		}

		public override void SetValue(ref ResolvedStyleAccess container, Color value)
		{
			throw new InvalidOperationException();
		}
	}

	private class UnityTextOutlineWidthProperty : ResolvedFloatProperty
	{
		public override string Name => "unityTextOutlineWidth";

		public override string ussName => "-unity-text-outline-width";

		public override bool IsReadOnly => true;

		public override float GetValue(ref ResolvedStyleAccess container)
		{
			return ((IResolvedStyle)container).unityTextOutlineWidth;
		}

		public override void SetValue(ref ResolvedStyleAccess container, float value)
		{
			throw new InvalidOperationException();
		}
	}

	private class UnityTextOverflowPositionProperty : ResolvedEnumProperty<TextOverflowPosition>
	{
		public override string Name => "unityTextOverflowPosition";

		public override string ussName => "-unity-text-overflow-position";

		public override bool IsReadOnly => true;

		public override TextOverflowPosition GetValue(ref ResolvedStyleAccess container)
		{
			return ((IResolvedStyle)container).unityTextOverflowPosition;
		}

		public override void SetValue(ref ResolvedStyleAccess container, TextOverflowPosition value)
		{
			throw new InvalidOperationException();
		}
	}

	private class VisibilityProperty : ResolvedEnumProperty<Visibility>
	{
		public override string Name => "visibility";

		public override string ussName => "visibility";

		public override bool IsReadOnly => true;

		public override Visibility GetValue(ref ResolvedStyleAccess container)
		{
			return ((IResolvedStyle)container).visibility;
		}

		public override void SetValue(ref ResolvedStyleAccess container, Visibility value)
		{
			throw new InvalidOperationException();
		}
	}

	private class WhiteSpaceProperty : ResolvedEnumProperty<WhiteSpace>
	{
		public override string Name => "whiteSpace";

		public override string ussName => "white-space";

		public override bool IsReadOnly => true;

		public override WhiteSpace GetValue(ref ResolvedStyleAccess container)
		{
			return ((IResolvedStyle)container).whiteSpace;
		}

		public override void SetValue(ref ResolvedStyleAccess container, WhiteSpace value)
		{
			throw new InvalidOperationException();
		}
	}

	private class WidthProperty : ResolvedFloatProperty
	{
		public override string Name => "width";

		public override string ussName => "width";

		public override bool IsReadOnly => true;

		public override float GetValue(ref ResolvedStyleAccess container)
		{
			return ((IResolvedStyle)container).width;
		}

		public override void SetValue(ref ResolvedStyleAccess container, float value)
		{
			throw new InvalidOperationException();
		}
	}

	private class WordSpacingProperty : ResolvedFloatProperty
	{
		public override string Name => "wordSpacing";

		public override string ussName => "word-spacing";

		public override bool IsReadOnly => true;

		public override float GetValue(ref ResolvedStyleAccess container)
		{
			return ((IResolvedStyle)container).wordSpacing;
		}

		public override void SetValue(ref ResolvedStyleAccess container, float value)
		{
			throw new InvalidOperationException();
		}
	}

	private abstract class ResolvedStyleProperty<TValue> : Property<ResolvedStyleAccess, TValue>
	{
		public abstract string ussName { get; }
	}

	private abstract class ResolvedEnumProperty<TValue> : ResolvedStyleProperty<TValue> where TValue : struct, IConvertible
	{
	}

	private abstract class ResolvedColorProperty : ResolvedStyleProperty<Color>
	{
	}

	private abstract class ResolvedBackgroundProperty : ResolvedStyleProperty<Background>
	{
	}

	private abstract class ResolvedFloatProperty : ResolvedStyleProperty<float>
	{
	}

	private abstract class ResolvedStyleFloatProperty : ResolvedStyleProperty<StyleFloat>
	{
	}

	private abstract class ResolvedListProperty<T> : ResolvedStyleProperty<IEnumerable<T>>
	{
	}

	private abstract class ResolvedFixedList4Property<T> : ResolvedStyleProperty<IEnumerable<T>>
	{
	}

	private abstract class ResolvedFontProperty : ResolvedStyleProperty<Font>
	{
	}

	private abstract class ResolvedFontDefinitionProperty : ResolvedStyleProperty<FontDefinition>
	{
	}

	private abstract class ResolvedIntProperty : ResolvedStyleProperty<int>
	{
	}

	private abstract class ResolvedRotateProperty : ResolvedStyleProperty<Rotate>
	{
	}

	private abstract class ResolvedScaleProperty : ResolvedStyleProperty<Scale>
	{
	}

	private abstract class ResolvedVector3Property : ResolvedStyleProperty<Vector3>
	{
	}

	private abstract class ResolvedBackgroundPositionProperty : ResolvedStyleProperty<BackgroundPosition>
	{
	}

	private abstract class ResolvedBackgroundRepeatProperty : ResolvedStyleProperty<BackgroundRepeat>
	{
	}

	private abstract class ResolvedBackgroundSizeProperty : ResolvedStyleProperty<BackgroundSize>
	{
	}

	private abstract class ResolvedMaterialProperty : ResolvedStyleProperty<Material>
	{
	}

	private readonly List<IProperty<ResolvedStyleAccess>> m_PropertiesList;

	private readonly Dictionary<string, IProperty<ResolvedStyleAccess>> m_PropertiesHash;

	public ResolvedStyleAccessPropertyBag()
	{
		m_PropertiesList = new List<IProperty<ResolvedStyleAccess>>(80);
		m_PropertiesHash = new Dictionary<string, IProperty<ResolvedStyleAccess>>(240);
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
		AddProperty(new PaddingBottomProperty());
		AddProperty(new PaddingLeftProperty());
		AddProperty(new PaddingRightProperty());
		AddProperty(new PaddingTopProperty());
		AddProperty(new PositionProperty());
		AddProperty(new RightProperty());
		AddProperty(new RotateProperty());
		AddProperty(new ScaleProperty());
		AddProperty(new TextOverflowProperty());
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
		AddProperty(new UnityParagraphSpacingProperty());
		AddProperty(new UnitySliceBottomProperty());
		AddProperty(new UnitySliceLeftProperty());
		AddProperty(new UnitySliceRightProperty());
		AddProperty(new UnitySliceScaleProperty());
		AddProperty(new UnitySliceTopProperty());
		AddProperty(new UnitySliceTypeProperty());
		AddProperty(new UnityTextAlignProperty());
		AddProperty(new UnityTextGeneratorProperty());
		AddProperty(new UnityTextOutlineColorProperty());
		AddProperty(new UnityTextOutlineWidthProperty());
		AddProperty(new UnityTextOverflowPositionProperty());
		AddProperty(new VisibilityProperty());
		AddProperty(new WhiteSpaceProperty());
		AddProperty(new WidthProperty());
		AddProperty(new WordSpacingProperty());
	}

	private void AddProperty<TValue>(ResolvedStyleProperty<TValue> property)
	{
		m_PropertiesList.Add(property);
		m_PropertiesHash.Add(property.Name, property);
		if (string.CompareOrdinal(property.Name, property.ussName) != 0)
		{
			m_PropertiesHash.Add(property.ussName, property);
		}
	}

	public override PropertyCollection<ResolvedStyleAccess> GetProperties()
	{
		return new PropertyCollection<ResolvedStyleAccess>(m_PropertiesList);
	}

	public override PropertyCollection<ResolvedStyleAccess> GetProperties(ref ResolvedStyleAccess container)
	{
		return new PropertyCollection<ResolvedStyleAccess>(m_PropertiesList);
	}

	public bool TryGetProperty(ref ResolvedStyleAccess container, string name, out IProperty<ResolvedStyleAccess> property)
	{
		return m_PropertiesHash.TryGetValue(name, out property);
	}
}
