using System;
using Unity.Properties;

namespace UnityEngine.UIElements.HierarchyV2;

internal class CollectionViewScroller : VisualElement, INotifyValueChanged<double>
{
	internal static readonly BindingId valueProperty = "value";

	internal static readonly BindingId lowValueProperty = "lowValue";

	internal static readonly BindingId highValueProperty = "highValue";

	internal static readonly BindingId directionProperty = "direction";

	private const float k_DefaultPageSize = 20f;

	private const double k_closeEnoughEpsilon = 8E-323;

	private ScrollerSlider slider { get; }

	private RepeatButton lowButton { get; }

	private RepeatButton highButton { get; }

	double INotifyValueChanged<double>.value
	{
		get
		{
			return value;
		}
		set
		{
			this.value = value;
		}
	}

	[CreateProperty]
	public double value
	{
		get
		{
			return slider.value;
		}
		set
		{
			if (base.enabledSelf)
			{
				double a = slider.value;
				slider.value = value;
				if (!Approximately(a, slider.value))
				{
					NotifyPropertyChanged(in valueProperty);
				}
			}
		}
	}

	[CreateProperty]
	public double lowValue
	{
		get
		{
			return slider.lowValue;
		}
		set
		{
			double a = slider.lowValue;
			slider.lowValue = value;
			if (!Approximately(a, slider.lowValue))
			{
				NotifyPropertyChanged(in lowValueProperty);
			}
		}
	}

	[CreateProperty]
	public double highValue
	{
		get
		{
			return slider.highValue;
		}
		set
		{
			double a = slider.highValue;
			slider.highValue = value;
			if (!Approximately(a, slider.highValue))
			{
				NotifyPropertyChanged(in highValueProperty);
			}
		}
	}

	[CreateProperty]
	public SliderDirection direction
	{
		get
		{
			return (base.resolvedStyle.flexDirection != FlexDirection.Row) ? SliderDirection.Vertical : SliderDirection.Horizontal;
		}
		set
		{
			SliderDirection sliderDirection = slider.direction;
			slider.direction = value;
			slider.inverted = value == SliderDirection.Vertical;
			if (value == SliderDirection.Horizontal)
			{
				base.style.flexDirection = FlexDirection.Row;
				AddToClassList(Scroller.horizontalVariantUssClassName);
				RemoveFromClassList(Scroller.verticalVariantUssClassName);
			}
			else
			{
				base.style.flexDirection = FlexDirection.Column;
				AddToClassList(Scroller.verticalVariantUssClassName);
				RemoveFromClassList(Scroller.horizontalVariantUssClassName);
			}
			if (sliderDirection != slider.direction)
			{
				NotifyPropertyChanged(in directionProperty);
			}
		}
	}

	public double scrollSize { get; set; }

	public void SetValueWithoutNotify(double newValue)
	{
		slider.SetValueWithoutNotify(newValue);
	}

	public CollectionViewScroller()
		: this(0.0, 0.0)
	{
	}

	public CollectionViewScroller(double lowValue, double highValue, SliderDirection direction = SliderDirection.Vertical)
	{
		scrollSize = 20.0;
		AddToClassList(Scroller.ussClassName);
		slider = new ScrollerSlider(lowValue, highValue, direction, 20f)
		{
			name = "unity-slider",
			viewDataKey = "Slider"
		};
		slider.AddToClassList(Scroller.sliderUssClassName);
		lowButton = new RepeatButton(ScrollPageUp, 250L, 30L)
		{
			name = "unity-low-button"
		};
		lowButton.AddToClassList(Scroller.lowButtonUssClassName);
		Add(lowButton);
		highButton = new RepeatButton(ScrollPageDown, 250L, 30L)
		{
			name = "unity-high-button"
		};
		highButton.AddToClassList(Scroller.highButtonUssClassName);
		Add(highButton);
		Add(slider);
		this.direction = direction;
	}

	public void Adjust(float factor)
	{
		SetEnabled(factor < 1f);
		slider.AdjustDragElement(factor);
	}

	public void ScrollPageUp()
	{
		ScrollPage(-1.0);
	}

	public void ScrollPageDown()
	{
		ScrollPage(1.0);
	}

	public void ScrollPage(double factor)
	{
		value += factor * (scrollSize * (double)((slider.lowValue < slider.highValue) ? 1f : (-1f)));
	}

	public bool Approximately(double a, double b)
	{
		double num = Math.Abs(a - b);
		return num < 8E-323;
	}
}
