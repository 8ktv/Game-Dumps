using System;
using System.Collections.Generic;
using System.Globalization;
using Unity.Properties;
using UnityEngine.Internal;

namespace UnityEngine.UIElements;

public abstract class BaseSlider<TValueType> : BaseField<TValueType>, IValueField<TValueType> where TValueType : IComparable<TValueType>
{
	[Serializable]
	[ExcludeFromDocs]
	public new abstract class UxmlSerializedData : BaseField<TValueType>.UxmlSerializedData
	{
		[SerializeField]
		private bool fill;

		[HideInInspector]
		[UxmlIgnore]
		[SerializeField]
		private UxmlAttributeFlags fill_UxmlAttributeFlags;

		public new static void Register()
		{
			BaseField<TValueType>.UxmlSerializedData.Register();
			UxmlDescriptionCache.RegisterType(typeof(UxmlSerializedData), new UxmlAttributeNames[1]
			{
				new UxmlAttributeNames("fill", "fill", null)
			});
		}

		public override void Deserialize(object obj)
		{
			base.Deserialize(obj);
			BaseSlider<TValueType> baseSlider = (BaseSlider<TValueType>)obj;
			if (UnityEngine.UIElements.UxmlSerializedData.ShouldWriteAttributeValue(fill_UxmlAttributeFlags))
			{
				baseSlider.fill = fill;
			}
		}
	}

	[Obsolete("UxmlTraits is deprecated and will be removed. Use UxmlElementAttribute instead.", false)]
	public new class UxmlTraits : BaseField<TValueType>.UxmlTraits
	{
	}

	[Obsolete("UxmlTraits<TValueUxmlAttributeType> is deprecated and will be removed. Use UxmlElementAttribute instead.", false)]
	public new class UxmlTraits<TValueUxmlAttributeType> : BaseFieldTraits<TValueType, TValueUxmlAttributeType> where TValueUxmlAttributeType : TypedUxmlAttributeDescription<TValueType>, new()
	{
		public UxmlTraits()
		{
			m_PickingMode.defaultValue = PickingMode.Ignore;
		}
	}

	internal enum SliderKey
	{
		None,
		Lowest,
		LowerPage,
		Lower,
		Higher,
		HigherPage,
		Highest
	}

	internal static readonly BindingId lowValueProperty = "lowValue";

	internal static readonly BindingId highValueProperty = "highValue";

	internal static readonly BindingId rangeProperty = "range";

	internal static readonly BindingId pageSizeProperty = "pageSize";

	internal static readonly BindingId showInputFieldProperty = "showInputField";

	internal static readonly BindingId directionProperty = "direction";

	internal static readonly BindingId invertedProperty = "inverted";

	internal static readonly BindingId fillProperty = "fill";

	private float m_AdjustedPageSizeFromClick = 0f;

	private bool m_IsEditingTextField;

	private bool m_Fill;

	[SerializeField]
	[DontCreateProperty]
	private TValueType m_LowValue;

	[DontCreateProperty]
	[SerializeField]
	private TValueType m_HighValue;

	private float m_PageSize;

	private bool m_ShowInputField = false;

	private Rect m_DragElementStartPos;

	private SliderDirection m_Direction;

	private bool m_Inverted = false;

	internal const float kDefaultPageSize = 0f;

	internal const bool kDefaultShowInputField = false;

	internal const bool kDefaultInverted = false;

	public new static readonly string ussClassName = "unity-base-slider";

	public new static readonly string labelUssClassName = ussClassName + "__label";

	public new static readonly string inputUssClassName = ussClassName + "__input";

	public static readonly string horizontalVariantUssClassName = ussClassName + "--horizontal";

	public static readonly string verticalVariantUssClassName = ussClassName + "--vertical";

	public static readonly string dragContainerUssClassName = ussClassName + "__drag-container";

	public static readonly string trackerUssClassName = ussClassName + "__tracker";

	public static readonly string draggerUssClassName = ussClassName + "__dragger";

	public static readonly string draggerBorderUssClassName = ussClassName + "__dragger-border";

	public static readonly string textFieldClassName = ussClassName + "__text-field";

	public static readonly string fillUssClassName = ussClassName + "__fill";

	public static readonly string movableUssClassName = ussClassName + "--movable";

	internal const string k_FillElementName = "unity-fill";

	internal VisualElement dragContainer { get; private set; }

	internal VisualElement dragElement { get; private set; }

	internal VisualElement trackElement { get; private set; }

	internal VisualElement dragBorderElement { get; private set; }

	internal TextField inputTextField { get; private set; }

	internal VisualElement fillElement { get; private set; }

	private protected override bool canSwitchToMixedValue
	{
		get
		{
			if (inputTextField == null)
			{
				return true;
			}
			return !inputTextField.textInputBase.textElement.hasFocus;
		}
	}

	[CreateProperty]
	public TValueType lowValue
	{
		get
		{
			return m_LowValue;
		}
		set
		{
			if (!EqualityComparer<TValueType>.Default.Equals(m_LowValue, value))
			{
				m_LowValue = value;
				ClampValue();
				UpdateDragElementPosition();
				SaveViewData();
				NotifyPropertyChanged(in lowValueProperty);
			}
		}
	}

	[CreateProperty]
	public TValueType highValue
	{
		get
		{
			return m_HighValue;
		}
		set
		{
			if (!EqualityComparer<TValueType>.Default.Equals(m_HighValue, value))
			{
				m_HighValue = value;
				ClampValue();
				UpdateDragElementPosition();
				SaveViewData();
				NotifyPropertyChanged(in highValueProperty);
			}
		}
	}

	[CreateProperty(ReadOnly = true)]
	public TValueType range => SliderRange();

	[CreateProperty]
	public virtual float pageSize
	{
		get
		{
			return m_PageSize;
		}
		set
		{
			if (m_PageSize != value)
			{
				m_PageSize = value;
				NotifyPropertyChanged(in pageSizeProperty);
			}
		}
	}

	[CreateProperty]
	public virtual bool showInputField
	{
		get
		{
			return m_ShowInputField;
		}
		set
		{
			if (m_ShowInputField != value)
			{
				m_ShowInputField = value;
				UpdateTextFieldVisibility();
				NotifyPropertyChanged(in showInputFieldProperty);
			}
		}
	}

	[CreateProperty]
	public bool fill
	{
		get
		{
			return m_Fill;
		}
		set
		{
			if (m_Fill != value)
			{
				m_Fill = value;
				if (value)
				{
					UpdateDragElementPosition();
				}
				else if (fillElement != null)
				{
					fillElement.RemoveFromHierarchy();
					fillElement = null;
				}
				NotifyPropertyChanged(in fillProperty);
			}
		}
	}

	internal bool clamped { get; set; } = true;

	internal ClampedDragger<TValueType> clampedDragger { get; private set; }

	public override TValueType value
	{
		get
		{
			return base.value;
		}
		set
		{
			TValueType val = (clamped ? GetClampedValue(value) : value);
			base.value = val;
		}
	}

	[CreateProperty]
	public SliderDirection direction
	{
		get
		{
			return m_Direction;
		}
		set
		{
			SliderDirection sliderDirection = m_Direction;
			m_Direction = value;
			if (m_Direction == SliderDirection.Horizontal)
			{
				RemoveFromClassList(verticalVariantUssClassName);
				AddToClassList(horizontalVariantUssClassName);
			}
			else
			{
				RemoveFromClassList(horizontalVariantUssClassName);
				AddToClassList(verticalVariantUssClassName);
			}
			if (sliderDirection != m_Direction)
			{
				NotifyPropertyChanged(in directionProperty);
			}
		}
	}

	[CreateProperty]
	public bool inverted
	{
		get
		{
			return m_Inverted;
		}
		set
		{
			if (m_Inverted != value)
			{
				m_Inverted = value;
				UpdateDragElementPosition();
				NotifyPropertyChanged(in invertedProperty);
			}
		}
	}

	internal event Action<TValueType> onSetValueWithoutNotify;

	internal void SetHighValueWithoutNotify(TValueType newHighValue)
	{
		m_HighValue = newHighValue;
		TValueType valueWithoutNotify = (clamped ? GetClampedValue(value) : value);
		SetValueWithoutNotify(valueWithoutNotify);
		UpdateDragElementPosition();
		SaveViewData();
	}

	private TValueType Clamp(TValueType value, TValueType lowBound, TValueType highBound)
	{
		TValueType result = value;
		if (lowBound.CompareTo(value) > 0)
		{
			result = lowBound;
		}
		else if (highBound.CompareTo(value) < 0)
		{
			result = highBound;
		}
		return result;
	}

	private TValueType GetClampedValue(TValueType newValue)
	{
		TValueType val = lowValue;
		TValueType val2 = highValue;
		if (val.CompareTo(val2) > 0)
		{
			TValueType val3 = val;
			val = val2;
			val2 = val3;
		}
		return Clamp(newValue, val, val2);
	}

	public virtual void ApplyInputDeviceDelta(Vector3 delta, DeltaSpeed speed, TValueType startValue)
	{
	}

	void IValueField<TValueType>.StartDragging()
	{
	}

	void IValueField<TValueType>.StopDragging()
	{
	}

	public override void SetValueWithoutNotify(TValueType newValue)
	{
		TValueType val = (clamped ? GetClampedValue(newValue) : newValue);
		base.SetValueWithoutNotify(val);
		this.onSetValueWithoutNotify?.Invoke(val);
		UpdateDragElementPosition();
		UpdateTextFieldValue();
	}

	internal BaseSlider(string label, TValueType start, TValueType end, SliderDirection direction = SliderDirection.Horizontal, float pageSize = 0f)
		: base(label, (VisualElement)null)
	{
		AddToClassList(ussClassName);
		base.labelElement.AddToClassList(labelUssClassName);
		base.visualInput.AddToClassList(inputUssClassName);
		this.direction = direction;
		this.pageSize = pageSize;
		lowValue = start;
		highValue = end;
		base.pickingMode = PickingMode.Ignore;
		dragContainer = new VisualElement
		{
			name = "unity-drag-container"
		};
		dragContainer.AddToClassList(dragContainerUssClassName);
		dragContainer.RegisterCallback<GeometryChangedEvent>(UpdateDragElementPosition);
		base.visualInput.Add(dragContainer);
		trackElement = new VisualElement
		{
			name = "unity-tracker",
			usageHints = UsageHints.DynamicColor
		};
		trackElement.AddToClassList(trackerUssClassName);
		dragContainer.Add(trackElement);
		dragBorderElement = new VisualElement
		{
			name = "unity-dragger-border"
		};
		dragBorderElement.AddToClassList(draggerBorderUssClassName);
		dragContainer.Add(dragBorderElement);
		dragElement = new VisualElement
		{
			name = "unity-dragger",
			usageHints = UsageHints.DynamicTransform
		};
		dragElement.RegisterCallback<GeometryChangedEvent>(UpdateDragElementPosition);
		dragElement.AddToClassList(draggerUssClassName);
		dragContainer.Add(dragElement);
		clampedDragger = new ClampedDragger<TValueType>(this, SetSliderValueFromClick, SetSliderValueFromDrag);
		dragContainer.pickingMode = PickingMode.Position;
		dragContainer.AddManipulator(clampedDragger);
		RegisterCallback<KeyDownEvent>(OnKeyDown);
		RegisterCallback<FocusInEvent>(OnFocusIn);
		RegisterCallback<FocusOutEvent>(OnFocusOut);
		RegisterCallback<NavigationSubmitEvent>(OnNavigationSubmit);
		RegisterCallback<NavigationMoveEvent>(OnNavigationMove);
		UpdateTextFieldVisibility();
		FieldMouseDragger<TValueType> fieldMouseDragger = new FieldMouseDragger<TValueType>(this);
		fieldMouseDragger.SetDragZone(base.labelElement);
		base.labelElement.AddToClassList(BaseField<TValueType>.labelDraggerVariantUssClassName);
	}

	protected internal static float GetClosestPowerOfTen(float positiveNumber)
	{
		if (positiveNumber <= 0f)
		{
			return 1f;
		}
		return Mathf.Pow(10f, Mathf.RoundToInt(Mathf.Log10(positiveNumber)));
	}

	protected internal static float RoundToMultipleOf(float value, float roundingValue)
	{
		if (roundingValue == 0f)
		{
			return value;
		}
		return Mathf.Round(value / roundingValue) * roundingValue;
	}

	private void ClampValue()
	{
		value = base.rawValue;
	}

	internal abstract TValueType SliderLerpUnclamped(TValueType a, TValueType b, float interpolant);

	internal abstract float SliderNormalizeValue(TValueType currentValue, TValueType lowerValue, TValueType higherValue);

	internal abstract TValueType SliderRange();

	internal abstract TValueType ParseStringToValue(string previousValue, string newValue);

	internal abstract void ComputeValueFromKey(SliderKey sliderKey, bool isShift);

	private TValueType SliderLerpDirectionalUnclamped(TValueType a, TValueType b, float positionInterpolant)
	{
		float interpolant = ((direction == SliderDirection.Vertical) ? (1f - positionInterpolant) : positionInterpolant);
		if (inverted)
		{
			return SliderLerpUnclamped(b, a, interpolant);
		}
		return SliderLerpUnclamped(a, b, interpolant);
	}

	private void SetSliderValueFromDrag()
	{
		if (clampedDragger.dragDirection == ClampedDragger<TValueType>.DragDirection.Free)
		{
			Vector2 delta = clampedDragger.delta;
			if (direction == SliderDirection.Horizontal)
			{
				ComputeValueAndDirectionFromDrag(dragContainer.resolvedStyle.width, dragElement.resolvedStyle.width, m_DragElementStartPos.x + delta.x);
			}
			else
			{
				ComputeValueAndDirectionFromDrag(dragContainer.resolvedStyle.height, dragElement.resolvedStyle.height, m_DragElementStartPos.y + delta.y);
			}
		}
	}

	private void ComputeValueAndDirectionFromDrag(float sliderLength, float dragElementLength, float dragElementPos)
	{
		float num = sliderLength - dragElementLength;
		if (!(Mathf.Abs(num) < 1E-30f))
		{
			float positionInterpolant = ((!clamped) ? (dragElementPos / num) : (Mathf.Max(0f, Mathf.Min(dragElementPos, num)) / num));
			TValueType y = value;
			value = SliderLerpDirectionalUnclamped(lowValue, highValue, positionInterpolant);
			if (EqualityComparer<TValueType>.Default.Equals(value, y))
			{
				UpdateDragElementPosition();
			}
		}
	}

	private void SetSliderValueFromClick()
	{
		if (clampedDragger.dragDirection == ClampedDragger<TValueType>.DragDirection.Free)
		{
			return;
		}
		if (clampedDragger.dragDirection == ClampedDragger<TValueType>.DragDirection.None)
		{
			if (Mathf.Approximately(pageSize, 0f))
			{
				float num;
				float num2;
				float num3;
				float num4;
				float dragElementPos;
				if (direction == SliderDirection.Horizontal)
				{
					num = dragContainer.resolvedStyle.width;
					num2 = dragElement.resolvedStyle.width;
					float b = num - num2;
					float a = clampedDragger.startMousePosition.x - num2 / 2f;
					num3 = Mathf.Max(0f, Mathf.Min(a, b));
					num4 = dragElement.resolvedStyle.translate.y;
					dragElementPos = num3;
				}
				else
				{
					num = dragContainer.resolvedStyle.height;
					num2 = dragElement.resolvedStyle.height;
					float b2 = num - num2;
					float a2 = clampedDragger.startMousePosition.y - num2 / 2f;
					num3 = dragElement.resolvedStyle.translate.x;
					num4 = Mathf.Max(0f, Mathf.Min(a2, b2));
					dragElementPos = num4;
				}
				Vector3 vector = new Vector3(num3, num4, 0f);
				dragElement.style.translate = vector;
				dragBorderElement.style.translate = vector;
				m_DragElementStartPos = new Rect(num3, num4, dragElement.resolvedStyle.width, dragElement.resolvedStyle.height);
				clampedDragger.dragDirection = ClampedDragger<TValueType>.DragDirection.Free;
				ComputeValueAndDirectionFromDrag(num, num2, dragElementPos);
				return;
			}
			m_DragElementStartPos = new Rect(dragElement.resolvedStyle.translate.x, dragElement.resolvedStyle.translate.y, dragElement.resolvedStyle.width, dragElement.resolvedStyle.height);
		}
		if (direction == SliderDirection.Horizontal)
		{
			ComputeValueAndDirectionFromClick(dragContainer.resolvedStyle.width, dragElement.resolvedStyle.width, dragElement.resolvedStyle.translate.x, clampedDragger.lastMousePosition.x);
		}
		else
		{
			ComputeValueAndDirectionFromClick(dragContainer.resolvedStyle.height, dragElement.resolvedStyle.height, dragElement.resolvedStyle.translate.y, clampedDragger.lastMousePosition.y);
		}
	}

	private void OnKeyDown(KeyDownEvent evt)
	{
		SliderKey sliderKey = SliderKey.None;
		bool flag = direction == SliderDirection.Horizontal;
		if ((flag && evt.keyCode == KeyCode.Home) || (!flag && evt.keyCode == KeyCode.End))
		{
			sliderKey = ((!inverted) ? SliderKey.Lowest : SliderKey.Highest);
		}
		else if ((flag && evt.keyCode == KeyCode.End) || (!flag && evt.keyCode == KeyCode.Home))
		{
			sliderKey = (inverted ? SliderKey.Lowest : SliderKey.Highest);
		}
		else if ((flag && evt.keyCode == KeyCode.PageUp) || (!flag && evt.keyCode == KeyCode.PageDown))
		{
			sliderKey = (inverted ? SliderKey.HigherPage : SliderKey.LowerPage);
		}
		else if ((flag && evt.keyCode == KeyCode.PageDown) || (!flag && evt.keyCode == KeyCode.PageUp))
		{
			sliderKey = (inverted ? SliderKey.LowerPage : SliderKey.HigherPage);
		}
		if (sliderKey != SliderKey.None)
		{
			ComputeValueFromKey(sliderKey, evt.shiftKey);
			evt.StopPropagation();
		}
	}

	private void OnNavigationMove(NavigationMoveEvent evt)
	{
		if (dragElement.ClassListContains(movableUssClassName))
		{
			SliderKey sliderKey = SliderKey.None;
			bool flag = direction == SliderDirection.Horizontal;
			if (evt.direction == (NavigationMoveEvent.Direction)(flag ? 1 : 4))
			{
				sliderKey = (inverted ? SliderKey.Higher : SliderKey.Lower);
			}
			else if (evt.direction == (NavigationMoveEvent.Direction)(flag ? 3 : 2))
			{
				sliderKey = (inverted ? SliderKey.Lower : SliderKey.Higher);
			}
			if (sliderKey != SliderKey.None)
			{
				ComputeValueFromKey(sliderKey, evt.shiftKey);
				evt.StopPropagation();
				focusController?.IgnoreEvent(evt);
			}
		}
	}

	private void OnNavigationSubmit(NavigationSubmitEvent evt)
	{
		if (!m_IsEditingTextField)
		{
			dragElement.EnableInClassList(movableUssClassName, !dragElement.ClassListContains(movableUssClassName));
		}
	}

	internal virtual void ComputeValueAndDirectionFromClick(float sliderLength, float dragElementLength, float dragElementPos, float dragElementLastPos)
	{
		float num = sliderLength - dragElementLength;
		if (!(Mathf.Abs(num) < 1E-30f))
		{
			bool flag = dragElementLastPos < dragElementPos;
			bool flag2 = dragElementLastPos > dragElementPos + dragElementLength;
			bool flag3 = (inverted ? flag2 : flag);
			bool flag4 = (inverted ? flag : flag2);
			m_AdjustedPageSizeFromClick = (inverted ? (m_AdjustedPageSizeFromClick - pageSize) : (m_AdjustedPageSizeFromClick + pageSize));
			if (flag3 && clampedDragger.dragDirection != ClampedDragger<TValueType>.DragDirection.LowToHigh)
			{
				clampedDragger.dragDirection = ClampedDragger<TValueType>.DragDirection.HighToLow;
				float positionInterpolant = Mathf.Max(0f, Mathf.Min(dragElementPos - m_AdjustedPageSizeFromClick, num)) / num;
				value = SliderLerpDirectionalUnclamped(lowValue, highValue, positionInterpolant);
			}
			else if (flag4 && clampedDragger.dragDirection != ClampedDragger<TValueType>.DragDirection.HighToLow)
			{
				clampedDragger.dragDirection = ClampedDragger<TValueType>.DragDirection.LowToHigh;
				float positionInterpolant2 = Mathf.Max(0f, Mathf.Min(dragElementPos + m_AdjustedPageSizeFromClick, num)) / num;
				value = SliderLerpDirectionalUnclamped(lowValue, highValue, positionInterpolant2);
			}
		}
	}

	public void AdjustDragElement(float factor)
	{
		if (factor < 1f)
		{
			dragElement.style.visibility = new StyleEnum<Visibility>(Visibility.Visible, StyleKeyword.Null);
			IStyle style = dragElement.style;
			if (direction == SliderDirection.Horizontal)
			{
				float b = ((base.resolvedStyle.minWidth == StyleKeyword.Auto) ? 0f : base.resolvedStyle.minWidth.value);
				style.width = Mathf.Round(Mathf.Max(dragContainer.layout.width * factor, b));
			}
			else
			{
				float b2 = ((base.resolvedStyle.minHeight == StyleKeyword.Auto) ? 0f : base.resolvedStyle.minHeight.value);
				style.height = Mathf.Round(Mathf.Max(dragContainer.layout.height * factor, b2));
			}
		}
		else
		{
			dragElement.style.visibility = new StyleEnum<Visibility>(Visibility.Hidden, StyleKeyword.Undefined);
		}
		dragBorderElement.visible = dragElement.visible;
	}

	private void UpdateDragElementPosition(GeometryChangedEvent evt)
	{
		if (!(evt.oldRect.size == evt.newRect.size))
		{
			UpdateDragElementPosition();
		}
	}

	internal override void OnViewDataReady()
	{
		base.OnViewDataReady();
		string fullHierarchicalViewDataKey = GetFullHierarchicalViewDataKey();
		OverwriteFromViewData(this, fullHierarchicalViewDataKey);
		UpdateDragElementPosition();
	}

	private bool SameValues(float a, float b, float epsilon)
	{
		return Mathf.Abs(b - a) < epsilon;
	}

	private void UpdateDragElementPosition()
	{
		if (base.panel == null)
		{
			return;
		}
		float num = SliderNormalizeValue(value, lowValue, highValue);
		float num2 = (inverted ? (1f - num) : num);
		float epsilon = base.scaledPixelsPerPoint * 0.5f;
		if (direction == SliderDirection.Horizontal)
		{
			float width = dragElement.resolvedStyle.width;
			float num3 = 0f - dragElement.resolvedStyle.marginLeft - dragElement.resolvedStyle.marginRight;
			float num4 = dragContainer.layout.width - width + num3;
			float num5 = num2 * num4;
			if (float.IsNaN(num5))
			{
				return;
			}
			float x = dragElement.resolvedStyle.translate.x;
			if (!SameValues(x, num5, epsilon))
			{
				Vector3 vector = new Vector3(num5, 0f, 0f);
				dragElement.style.translate = vector;
				dragBorderElement.style.translate = vector;
				m_AdjustedPageSizeFromClick = 0f;
			}
		}
		else
		{
			float height = dragElement.resolvedStyle.height;
			float num6 = dragContainer.resolvedStyle.height - height;
			float num7 = (1f - num2) * num6;
			if (float.IsNaN(num7))
			{
				return;
			}
			float y = dragElement.resolvedStyle.translate.y;
			if (!SameValues(y, num7, epsilon))
			{
				Vector3 vector2 = new Vector3(0f, num7, 0f);
				dragElement.style.translate = vector2;
				dragBorderElement.style.translate = vector2;
				m_AdjustedPageSizeFromClick = 0f;
			}
		}
		UpdateFill(num);
	}

	private void UpdateFill(float normalizedValue)
	{
		if (fill)
		{
			if (fillElement == null)
			{
				fillElement = new VisualElement
				{
					name = "unity-fill",
					usageHints = UsageHints.DynamicColor
				};
				fillElement.AddToClassList(fillUssClassName);
				trackElement.Add(fillElement);
			}
			float num = 1f - normalizedValue;
			Length length = Length.Percent(num * 100f);
			if (direction == SliderDirection.Vertical)
			{
				fillElement.style.right = 0f;
				fillElement.style.left = 0f;
				fillElement.style.bottom = (inverted ? length : ((Length)0f));
				fillElement.style.top = (inverted ? ((Length)0f) : length);
			}
			else
			{
				fillElement.style.top = 0f;
				fillElement.style.bottom = 0f;
				fillElement.style.left = (inverted ? length : ((Length)0f));
				fillElement.style.right = (inverted ? ((Length)0f) : length);
			}
		}
	}

	[EventInterest(new Type[] { typeof(GeometryChangedEvent) })]
	protected override void HandleEventBubbleUp(EventBase evt)
	{
		base.HandleEventBubbleUp(evt);
		if (evt != null && evt.eventTypeId == EventBase<GeometryChangedEvent>.TypeId())
		{
			UpdateDragElementPosition((GeometryChangedEvent)evt);
		}
	}

	[EventInterest(EventInterestOptions.Inherit)]
	[Obsolete("ExecuteDefaultAction override has been removed because default event handling was migrated to HandleEventBubbleUp. Please use HandleEventBubbleUp.", false)]
	protected override void ExecuteDefaultAction(EventBase evt)
	{
	}

	private void UpdateTextFieldVisibility()
	{
		if (showInputField)
		{
			if (inputTextField == null)
			{
				inputTextField = new TextField
				{
					name = "unity-text-field"
				};
				inputTextField.AddToClassList(textFieldClassName);
				inputTextField.RegisterValueChangedCallback(OnTextFieldValueChange);
				inputTextField.RegisterCallback<FocusInEvent>(OnTextFieldFocusIn);
				inputTextField.RegisterCallback<FocusOutEvent>(OnTextFieldFocusOut);
				base.visualInput.Add(inputTextField);
				UpdateTextFieldValue();
			}
		}
		else if (inputTextField != null && inputTextField.panel != null)
		{
			if (inputTextField.panel != null)
			{
				inputTextField.RemoveFromHierarchy();
			}
			inputTextField.UnregisterValueChangedCallback(OnTextFieldValueChange);
			inputTextField.UnregisterCallback<FocusInEvent>(OnTextFieldFocusIn);
			inputTextField.UnregisterCallback<FocusOutEvent>(OnTextFieldFocusOut);
			inputTextField = null;
		}
	}

	private void UpdateTextFieldValue()
	{
		if (inputTextField != null && !m_IsEditingTextField)
		{
			inputTextField.SetValueWithoutNotify(string.Format(CultureInfo.InvariantCulture, "{0:g7}", value));
		}
	}

	private void OnFocusIn(FocusInEvent evt)
	{
		dragElement.AddToClassList(movableUssClassName);
	}

	private void OnFocusOut(FocusOutEvent evt)
	{
		dragElement.RemoveFromClassList(movableUssClassName);
	}

	private void OnTextFieldFocusIn(FocusInEvent evt)
	{
		m_IsEditingTextField = true;
	}

	private void OnTextFieldFocusOut(FocusOutEvent evt)
	{
		m_IsEditingTextField = false;
		UpdateTextFieldValue();
	}

	private void OnTextFieldValueChange(ChangeEvent<string> evt)
	{
		TValueType clampedValue = GetClampedValue(ParseStringToValue(evt.previousValue, evt.newValue));
		if (!EqualityComparer<TValueType>.Default.Equals(clampedValue, value))
		{
			value = clampedValue;
			evt.StopPropagation();
			if (base.elementPanel != null)
			{
				OnViewDataReady();
			}
		}
	}

	protected override void UpdateMixedValueContent()
	{
		if (base.showMixedValue)
		{
			dragElement?.RemoveFromHierarchy();
			if (inputTextField != null)
			{
				inputTextField.showMixedValue = true;
			}
		}
		else
		{
			dragContainer.Add(dragElement);
			if (inputTextField != null)
			{
				inputTextField.showMixedValue = false;
			}
		}
	}

	internal override void RegisterEditingCallbacks()
	{
		base.labelElement.RegisterCallback<PointerDownEvent>(base.StartEditing, TrickleDown.TrickleDown);
		dragContainer.RegisterCallback<PointerDownEvent>(base.StartEditing, TrickleDown.TrickleDown);
		dragContainer.RegisterCallback<PointerUpEvent>(base.EndEditing);
	}

	internal override void UnregisterEditingCallbacks()
	{
		base.labelElement.UnregisterCallback<PointerDownEvent>(base.StartEditing, TrickleDown.TrickleDown);
		dragContainer.UnregisterCallback<PointerDownEvent>(base.StartEditing, TrickleDown.TrickleDown);
		dragContainer.UnregisterCallback<PointerUpEvent>(base.EndEditing);
	}
}
