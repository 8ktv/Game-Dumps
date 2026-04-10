using System;
using System.Collections.Generic;
using System.Diagnostics;
using Unity.Properties;
using UnityEngine.Internal;

namespace UnityEngine.UIElements;

public class Scroller : VisualElement
{
	private class ScrollerSlider : Slider
	{
		public ScrollerSlider(float start, float end, SliderDirection direction, float pageSize)
			: base(start, end, direction, pageSize)
		{
		}

		internal override float SliderNormalizeValue(float currentValue, float lowerValue, float higherValue)
		{
			return Mathf.Clamp(base.SliderNormalizeValue(currentValue, lowerValue, higherValue), 0f, 1f);
		}
	}

	[Serializable]
	[ExcludeFromDocs]
	public new class UxmlSerializedData : VisualElement.UxmlSerializedData
	{
		[SerializeField]
		[UxmlAttribute("low-value", new string[] { "lowValue" })]
		private float lowValue;

		[HideInInspector]
		[UxmlIgnore]
		[SerializeField]
		private UxmlAttributeFlags lowValue_UxmlAttributeFlags;

		[SerializeField]
		[UxmlAttribute("high-value", new string[] { "highValue" })]
		private float highValue;

		[SerializeField]
		[UxmlIgnore]
		[HideInInspector]
		private UxmlAttributeFlags highValue_UxmlAttributeFlags;

		[SerializeField]
		private SliderDirection direction;

		[SerializeField]
		[UxmlIgnore]
		[HideInInspector]
		private UxmlAttributeFlags direction_UxmlAttributeFlags;

		[SerializeField]
		private float value;

		[UxmlIgnore]
		[HideInInspector]
		[SerializeField]
		private UxmlAttributeFlags value_UxmlAttributeFlags;

		[Conditional("UNITY_EDITOR")]
		public new static void Register()
		{
			UxmlDescriptionCache.RegisterType(typeof(UxmlSerializedData), new UxmlAttributeNames[4]
			{
				new UxmlAttributeNames("lowValue", "low-value", null, "lowValue"),
				new UxmlAttributeNames("highValue", "high-value", null, "highValue"),
				new UxmlAttributeNames("direction", "direction", null),
				new UxmlAttributeNames("value", "value", null)
			});
		}

		public override object CreateInstance()
		{
			return new Scroller();
		}

		public override void Deserialize(object obj)
		{
			base.Deserialize(obj);
			Scroller scroller = (Scroller)obj;
			if (UnityEngine.UIElements.UxmlSerializedData.ShouldWriteAttributeValue(lowValue_UxmlAttributeFlags))
			{
				scroller.slider.lowValue = lowValue;
			}
			if (UnityEngine.UIElements.UxmlSerializedData.ShouldWriteAttributeValue(highValue_UxmlAttributeFlags))
			{
				scroller.slider.highValue = highValue;
			}
			if (UnityEngine.UIElements.UxmlSerializedData.ShouldWriteAttributeValue(direction_UxmlAttributeFlags))
			{
				scroller.direction = direction;
			}
			if (UnityEngine.UIElements.UxmlSerializedData.ShouldWriteAttributeValue(value_UxmlAttributeFlags))
			{
				scroller.value = value;
			}
		}
	}

	[Obsolete("UxmlFactory is deprecated and will be removed. Use UxmlElementAttribute instead.", false)]
	public new class UxmlFactory : UxmlFactory<Scroller, UxmlTraits>
	{
	}

	[Obsolete("UxmlTraits is deprecated and will be removed. Use UxmlElementAttribute instead.", false)]
	public new class UxmlTraits : VisualElement.UxmlTraits
	{
		private UxmlFloatAttributeDescription m_LowValue = new UxmlFloatAttributeDescription
		{
			name = "low-value",
			obsoleteNames = new string[1] { "lowValue" }
		};

		private UxmlFloatAttributeDescription m_HighValue = new UxmlFloatAttributeDescription
		{
			name = "high-value",
			obsoleteNames = new string[1] { "highValue" }
		};

		private UxmlEnumAttributeDescription<SliderDirection> m_Direction = new UxmlEnumAttributeDescription<SliderDirection>
		{
			name = "direction",
			defaultValue = SliderDirection.Vertical
		};

		private UxmlFloatAttributeDescription m_Value = new UxmlFloatAttributeDescription
		{
			name = "value"
		};

		public override IEnumerable<UxmlChildElementDescription> uxmlChildElementsDescription
		{
			get
			{
				yield break;
			}
		}

		public override void Init(VisualElement ve, IUxmlAttributes bag, CreationContext cc)
		{
			base.Init(ve, bag, cc);
			Scroller scroller = (Scroller)ve;
			scroller.slider.lowValue = m_LowValue.GetValueFromBag(bag, cc);
			scroller.slider.highValue = m_HighValue.GetValueFromBag(bag, cc);
			scroller.direction = m_Direction.GetValueFromBag(bag, cc);
			scroller.value = m_Value.GetValueFromBag(bag, cc);
		}
	}

	internal static readonly BindingId valueProperty = "value";

	internal static readonly BindingId lowValueProperty = "lowValue";

	internal static readonly BindingId highValueProperty = "highValue";

	internal static readonly BindingId directionProperty = "direction";

	internal const float kDefaultPageSize = 20f;

	public static readonly string ussClassName = "unity-scroller";

	public static readonly string horizontalVariantUssClassName = ussClassName + "--horizontal";

	public static readonly string verticalVariantUssClassName = ussClassName + "--vertical";

	public static readonly string sliderUssClassName = ussClassName + "__slider";

	public static readonly string lowButtonUssClassName = ussClassName + "__low-button";

	public static readonly string highButtonUssClassName = ussClassName + "__high-button";

	public Slider slider { get; }

	public RepeatButton lowButton { get; }

	public RepeatButton highButton { get; }

	[CreateProperty]
	public float value
	{
		get
		{
			return slider.value;
		}
		set
		{
			float a = slider.value;
			slider.value = value;
			if (!Mathf.Approximately(a, slider.value))
			{
				NotifyPropertyChanged(in valueProperty);
			}
		}
	}

	[CreateProperty]
	public float lowValue
	{
		get
		{
			return slider.lowValue;
		}
		set
		{
			float a = slider.lowValue;
			slider.lowValue = value;
			if (!Mathf.Approximately(a, slider.lowValue))
			{
				NotifyPropertyChanged(in lowValueProperty);
			}
		}
	}

	[CreateProperty]
	public float highValue
	{
		get
		{
			return slider.highValue;
		}
		set
		{
			float a = slider.highValue;
			slider.highValue = value;
			if (!Mathf.Approximately(a, slider.highValue))
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
				AddToClassList(horizontalVariantUssClassName);
				RemoveFromClassList(verticalVariantUssClassName);
			}
			else
			{
				base.style.flexDirection = FlexDirection.Column;
				AddToClassList(verticalVariantUssClassName);
				RemoveFromClassList(horizontalVariantUssClassName);
			}
			if (sliderDirection != slider.direction)
			{
				NotifyPropertyChanged(in directionProperty);
			}
		}
	}

	public event Action<float> valueChanged;

	public Scroller()
		: this(0f, 0f, null)
	{
	}

	public Scroller(float lowValue, float highValue, Action<float> valueChanged, SliderDirection direction = SliderDirection.Vertical)
	{
		AddToClassList(ussClassName);
		slider = new ScrollerSlider(lowValue, highValue, direction, 20f)
		{
			name = "unity-slider",
			viewDataKey = "Slider"
		};
		slider.AddToClassList(sliderUssClassName);
		slider.RegisterValueChangedCallback(OnSliderValueChange);
		lowButton = new RepeatButton(ScrollPageUp, 250L, 30L)
		{
			name = "unity-low-button"
		};
		lowButton.AddToClassList(lowButtonUssClassName);
		Add(lowButton);
		highButton = new RepeatButton(ScrollPageDown, 250L, 30L)
		{
			name = "unity-high-button"
		};
		highButton.AddToClassList(highButtonUssClassName);
		Add(highButton);
		Add(slider);
		this.direction = direction;
		this.valueChanged = valueChanged;
	}

	public void Adjust(float factor)
	{
		SetEnabled(factor < 1f);
		slider.AdjustDragElement(factor);
	}

	private void OnSliderValueChange(ChangeEvent<float> evt)
	{
		value = evt.newValue;
		this.valueChanged?.Invoke(slider.value);
		IncrementVersion(VersionChangeType.Repaint);
	}

	public void ScrollPageUp()
	{
		ScrollPageUp(1f);
	}

	public void ScrollPageDown()
	{
		ScrollPageDown(1f);
	}

	public void ScrollPageUp(float factor)
	{
		value -= factor * (slider.pageSize * ((slider.lowValue < slider.highValue) ? 1f : (-1f)));
	}

	public void ScrollPageDown(float factor)
	{
		value += factor * (slider.pageSize * ((slider.lowValue < slider.highValue) ? 1f : (-1f)));
	}
}
