using System;
using System.Collections.Generic;
using System.Diagnostics;
using Unity.Properties;
using UnityEngine.Internal;

namespace UnityEngine.UIElements;

public class MinMaxSlider : BaseField<Vector2>
{
	[Serializable]
	[ExcludeFromDocs]
	public new class UxmlSerializedData : BaseField<Vector2>.UxmlSerializedData, IUxmlSerializedDataCustomAttributeHandler
	{
		[SerializeField]
		private float lowLimit;

		[SerializeField]
		private float highLimit;

		[SerializeField]
		[UxmlIgnore]
		[HideInInspector]
		private UxmlAttributeFlags lowLimit_UxmlAttributeFlags;

		[HideInInspector]
		[UxmlIgnore]
		[SerializeField]
		private UxmlAttributeFlags highLimit_UxmlAttributeFlags;

		[Conditional("UNITY_EDITOR")]
		public new static void Register()
		{
			BaseField<Vector2>.UxmlSerializedData.Register();
			UxmlDescriptionCache.RegisterType(typeof(UxmlSerializedData), new UxmlAttributeNames[2]
			{
				new UxmlAttributeNames("lowLimit", "low-limit", null),
				new UxmlAttributeNames("highLimit", "high-limit", null)
			});
		}

		void IUxmlSerializedDataCustomAttributeHandler.SerializeCustomAttributes(IUxmlAttributes bag, HashSet<string> handledAttributes)
		{
			int foundAttributeCounter = 0;
			float x = UxmlUtility.TryParseFloatAttribute("min-value", bag, ref foundAttributeCounter);
			float y = UxmlUtility.TryParseFloatAttribute("max-value", bag, ref foundAttributeCounter);
			if (foundAttributeCounter > 0)
			{
				base.Value = new Vector2(x, y);
				handledAttributes.Add("value");
				if (bag is UxmlAsset uxmlAsset)
				{
					uxmlAsset.RemoveAttribute("min-value");
					uxmlAsset.RemoveAttribute("max-value");
					uxmlAsset.SetAttribute("value", UxmlUtility.ValueToString(base.Value));
				}
			}
		}

		public override object CreateInstance()
		{
			return new MinMaxSlider();
		}

		public override void Deserialize(object obj)
		{
			base.Deserialize(obj);
			MinMaxSlider minMaxSlider = (MinMaxSlider)obj;
			if (UnityEngine.UIElements.UxmlSerializedData.ShouldWriteAttributeValue(lowLimit_UxmlAttributeFlags))
			{
				minMaxSlider.lowLimit = lowLimit;
			}
			if (UnityEngine.UIElements.UxmlSerializedData.ShouldWriteAttributeValue(highLimit_UxmlAttributeFlags))
			{
				minMaxSlider.highLimit = highLimit;
			}
		}
	}

	[Obsolete("UxmlFactory is deprecated and will be removed. Use UxmlElementAttribute instead.", false)]
	public new class UxmlFactory : UxmlFactory<MinMaxSlider, UxmlTraits>
	{
	}

	[Obsolete("UxmlTraits is deprecated and will be removed. Use UxmlElementAttribute instead.", false)]
	public new class UxmlTraits : BaseField<Vector2>.UxmlTraits
	{
		private UxmlFloatAttributeDescription m_MinValue = new UxmlFloatAttributeDescription
		{
			name = "min-value",
			defaultValue = 0f
		};

		private UxmlFloatAttributeDescription m_MaxValue = new UxmlFloatAttributeDescription
		{
			name = "max-value",
			defaultValue = 10f
		};

		private UxmlFloatAttributeDescription m_LowLimit = new UxmlFloatAttributeDescription
		{
			name = "low-limit",
			defaultValue = float.MinValue
		};

		private UxmlFloatAttributeDescription m_HighLimit = new UxmlFloatAttributeDescription
		{
			name = "high-limit",
			defaultValue = float.MaxValue
		};

		public UxmlTraits()
		{
			m_PickingMode.defaultValue = PickingMode.Ignore;
		}

		public override void Init(VisualElement ve, IUxmlAttributes bag, CreationContext cc)
		{
			base.Init(ve, bag, cc);
			MinMaxSlider minMaxSlider = (MinMaxSlider)ve;
			minMaxSlider.lowLimit = m_LowLimit.GetValueFromBag(bag, cc);
			minMaxSlider.highLimit = m_HighLimit.GetValueFromBag(bag, cc);
			Vector2 value = new Vector2(m_MinValue.GetValueFromBag(bag, cc), m_MaxValue.GetValueFromBag(bag, cc));
			minMaxSlider.value = value;
		}
	}

	private enum DragState
	{
		MinThumb,
		MaxThumb,
		MiddleThumb,
		NoThumb
	}

	internal static readonly BindingId minValueProperty = "minValue";

	internal static readonly BindingId maxValueProperty = "maxValue";

	internal static readonly BindingId rangeProperty = "range";

	internal static readonly BindingId lowLimitProperty = "lowLimit";

	internal static readonly BindingId highLimitProperty = "highLimit";

	private Vector2 m_DragElementStartPos;

	private Vector2 m_ValueStartPos;

	private DragState m_DragState;

	private float m_MinLimit;

	private float m_MaxLimit;

	internal const float kDefaultHighValue = 10f;

	public new static readonly string ussClassName = "unity-min-max-slider";

	public new static readonly string labelUssClassName = ussClassName + "__label";

	public new static readonly string inputUssClassName = ussClassName + "__input";

	public static readonly string trackerUssClassName = ussClassName + "__tracker";

	public static readonly string draggerUssClassName = ussClassName + "__dragger";

	public static readonly string minThumbUssClassName = ussClassName + "__min-thumb";

	public static readonly string maxThumbUssClassName = ussClassName + "__max-thumb";

	public static readonly string movableUssClassName = ussClassName + "--movable";

	internal VisualElement dragElement { get; private set; }

	internal VisualElement dragMinThumb { get; private set; }

	internal VisualElement dragMaxThumb { get; private set; }

	internal ClampedDragger<float> clampedDragger { get; private set; }

	[CreateProperty]
	public float minValue
	{
		get
		{
			return value.x;
		}
		set
		{
			float a = minValue;
			base.value = ClampValues(new Vector2(value, base.rawValue.y));
			if (!Mathf.Approximately(a, minValue))
			{
				NotifyPropertyChanged(in minValueProperty);
			}
		}
	}

	[CreateProperty]
	public float maxValue
	{
		get
		{
			return value.y;
		}
		set
		{
			float a = maxValue;
			base.value = ClampValues(new Vector2(base.rawValue.x, value));
			if (!Mathf.Approximately(a, maxValue))
			{
				NotifyPropertyChanged(in maxValueProperty);
			}
		}
	}

	public override Vector2 value
	{
		get
		{
			return base.value;
		}
		set
		{
			base.value = ClampValues(value);
		}
	}

	[CreateProperty(ReadOnly = true)]
	public float range => Math.Abs(highLimit - lowLimit);

	[CreateProperty]
	public float lowLimit
	{
		get
		{
			return m_MinLimit;
		}
		set
		{
			if (!Mathf.Approximately(m_MinLimit, value))
			{
				if (value > m_MaxLimit)
				{
					throw new ArgumentException("lowLimit is greater than highLimit");
				}
				m_MinLimit = value;
				this.value = base.rawValue;
				UpdateDragElementPosition();
				if (!string.IsNullOrEmpty(base.viewDataKey))
				{
					SaveViewData();
				}
				NotifyPropertyChanged(in lowLimitProperty);
			}
		}
	}

	[CreateProperty]
	public float highLimit
	{
		get
		{
			return m_MaxLimit;
		}
		set
		{
			if (!Mathf.Approximately(m_MaxLimit, value))
			{
				if (value < m_MinLimit)
				{
					throw new ArgumentException("highLimit is smaller than lowLimit");
				}
				m_MaxLimit = value;
				this.value = base.rawValue;
				UpdateDragElementPosition();
				if (!string.IsNullOrEmpty(base.viewDataKey))
				{
					SaveViewData();
				}
				NotifyPropertyChanged(in highLimitProperty);
			}
		}
	}

	public override void SetValueWithoutNotify(Vector2 newValue)
	{
		base.SetValueWithoutNotify(ClampValues(newValue));
		UpdateDragElementPosition();
	}

	public MinMaxSlider()
		: this(null)
	{
	}

	public MinMaxSlider(float minValue, float maxValue, float minLimit, float maxLimit)
		: this(null, minValue, maxValue, minLimit, maxLimit)
	{
	}

	public MinMaxSlider(string label, float minValue = 0f, float maxValue = 10f, float minLimit = float.MinValue, float maxLimit = float.MaxValue)
		: base(label, (VisualElement)null)
	{
		m_MinLimit = float.MinValue;
		m_MaxLimit = float.MaxValue;
		lowLimit = minLimit;
		highLimit = maxLimit;
		Vector2 vector = ClampValues(new Vector2(minValue, maxValue));
		this.minValue = vector.x;
		this.maxValue = vector.y;
		AddToClassList(ussClassName);
		base.labelElement.AddToClassList(labelUssClassName);
		base.visualInput.AddToClassList(inputUssClassName);
		base.pickingMode = PickingMode.Ignore;
		m_DragState = DragState.NoThumb;
		base.visualInput.pickingMode = PickingMode.Position;
		VisualElement visualElement = new VisualElement
		{
			name = "unity-tracker"
		};
		visualElement.AddToClassList(trackerUssClassName);
		base.visualInput.Add(visualElement);
		dragElement = new VisualElement
		{
			name = "unity-dragger"
		};
		dragElement.AddToClassList(draggerUssClassName);
		dragElement.RegisterCallback<GeometryChangedEvent>(UpdateDragElementPosition);
		base.visualInput.Add(dragElement);
		dragMinThumb = new VisualElement
		{
			name = "unity-thumb-min"
		};
		dragMaxThumb = new VisualElement
		{
			name = "unity-thumb-max"
		};
		dragMinThumb.AddToClassList(minThumbUssClassName);
		dragMaxThumb.AddToClassList(maxThumbUssClassName);
		dragElement.Add(dragMinThumb);
		dragElement.Add(dragMaxThumb);
		clampedDragger = new ClampedDragger<float>(null, SetSliderValueFromClick, SetSliderValueFromDrag);
		base.visualInput.AddManipulator(clampedDragger);
		m_MinLimit = minLimit;
		m_MaxLimit = maxLimit;
		base.rawValue = ClampValues(new Vector2(minValue, maxValue));
		UpdateDragElementPosition();
		RegisterCallback<FocusInEvent>(OnFocusIn);
		RegisterCallback<BlurEvent>(OnBlur);
		RegisterCallback<NavigationSubmitEvent>(OnNavigationSubmit);
		RegisterCallback<NavigationMoveEvent>(OnNavigationMove);
	}

	private Vector2 ClampValues(Vector2 valueToClamp)
	{
		if (m_MinLimit > m_MaxLimit)
		{
			m_MinLimit = m_MaxLimit;
		}
		Vector2 result = default(Vector2);
		if (valueToClamp.y > m_MaxLimit)
		{
			valueToClamp.y = m_MaxLimit;
		}
		result.x = Mathf.Clamp(valueToClamp.x, m_MinLimit, valueToClamp.y);
		result.y = Mathf.Clamp(valueToClamp.y, valueToClamp.x, m_MaxLimit);
		return result;
	}

	private void UpdateDragElementPosition(GeometryChangedEvent evt)
	{
		if (!(evt.oldRect.size == evt.newRect.size))
		{
			UpdateDragElementPosition();
		}
	}

	private void UpdateDragElementPosition()
	{
		if (base.panel != null)
		{
			float num = dragElement.resolvedStyle.borderLeftWidth + dragElement.resolvedStyle.marginLeft;
			float num2 = dragElement.resolvedStyle.borderRightWidth + dragElement.resolvedStyle.marginRight;
			float num3 = num2 + num;
			float num4 = dragMinThumb.resolvedStyle.width + dragMaxThumb.resolvedStyle.width + num3;
			float num5 = this.RoundToPanelPixelSize(SliderLerpUnclamped(dragMinThumb.resolvedStyle.width, base.visualInput.layout.width - dragMaxThumb.resolvedStyle.width - num3, SliderNormalizeValue(minValue, lowLimit, highLimit)));
			float num6 = this.RoundToPanelPixelSize(SliderLerpUnclamped(dragMinThumb.resolvedStyle.width + num3, base.visualInput.layout.width - dragMaxThumb.resolvedStyle.width, SliderNormalizeValue(maxValue, lowLimit, highLimit)));
			dragElement.style.width = num6 - num5;
			dragElement.style.left = num5;
			dragMinThumb.style.left = 0f - dragMinThumb.resolvedStyle.width - num;
			dragMaxThumb.style.right = 0f - dragMaxThumb.resolvedStyle.width - num2;
		}
	}

	internal float SliderLerpUnclamped(float a, float b, float interpolant)
	{
		return Mathf.LerpUnclamped(a, b, interpolant);
	}

	internal float SliderNormalizeValue(float currentValue, float lowerValue, float higherValue)
	{
		return (currentValue - lowerValue) / (higherValue - lowerValue);
	}

	private float ComputeValueFromPosition(float positionToConvert)
	{
		float interpolant = SliderNormalizeValue(positionToConvert, 0f, base.visualInput.layout.width);
		return SliderLerpUnclamped(lowLimit, highLimit, interpolant);
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

	private DragState GetNavigationState()
	{
		bool flag = dragMinThumb.ClassListContains(movableUssClassName);
		bool flag2 = dragMaxThumb.ClassListContains(movableUssClassName);
		if (flag)
		{
			return flag2 ? DragState.MiddleThumb : DragState.MinThumb;
		}
		if (flag2)
		{
			return DragState.MaxThumb;
		}
		return DragState.NoThumb;
	}

	private void SetNavigationState(DragState newState)
	{
		dragMinThumb.EnableInClassList(movableUssClassName, newState == DragState.MinThumb || newState == DragState.MiddleThumb);
		dragMaxThumb.EnableInClassList(movableUssClassName, newState == DragState.MaxThumb || newState == DragState.MiddleThumb);
		dragElement.EnableInClassList(movableUssClassName, newState == DragState.MiddleThumb);
	}

	private void OnFocusIn(FocusInEvent evt)
	{
		if (GetNavigationState() == DragState.NoThumb)
		{
			SetNavigationState(DragState.MinThumb);
		}
	}

	private void OnBlur(BlurEvent evt)
	{
		SetNavigationState(DragState.NoThumb);
	}

	private void OnNavigationSubmit(NavigationSubmitEvent evt)
	{
		DragState dragState = GetNavigationState() + 1;
		if (dragState > DragState.NoThumb)
		{
			dragState = DragState.MinThumb;
		}
		SetNavigationState(dragState);
	}

	private void OnNavigationMove(NavigationMoveEvent evt)
	{
		DragState navigationState = GetNavigationState();
		if (navigationState != DragState.NoThumb && (evt.direction == NavigationMoveEvent.Direction.Left || evt.direction == NavigationMoveEvent.Direction.Right))
		{
			ComputeValueFromKey(evt.direction == NavigationMoveEvent.Direction.Left, evt.shiftKey, navigationState);
			evt.StopPropagation();
			focusController?.IgnoreEvent(evt);
		}
	}

	private void ComputeValueFromKey(bool leftDirection, bool isShift, DragState moveState)
	{
		float num = BaseSlider<float>.GetClosestPowerOfTen(Mathf.Abs((highLimit - lowLimit) * 0.01f));
		if (isShift)
		{
			num *= 10f;
		}
		if (leftDirection)
		{
			num = 0f - num;
		}
		switch (moveState)
		{
		case DragState.MinThumb:
		{
			float num6 = BaseSlider<float>.RoundToMultipleOf(value.x + num * 0.5001f, Mathf.Abs(num));
			num6 = Math.Clamp(num6, lowLimit, value.y);
			value = new Vector2(num6, value.y);
			break;
		}
		case DragState.MaxThumb:
		{
			float num5 = BaseSlider<float>.RoundToMultipleOf(value.y + num * 0.5001f, Mathf.Abs(num));
			num5 = Math.Clamp(num5, value.x, highLimit);
			value = new Vector2(value.x, num5);
			break;
		}
		case DragState.MiddleThumb:
		{
			float num2 = value.y - value.x;
			if (num > 0f)
			{
				float num3 = BaseSlider<float>.RoundToMultipleOf(value.y + num * 0.5001f, Mathf.Abs(num));
				num3 = Math.Clamp(num3, value.x, highLimit);
				value = new Vector2(num3 - num2, num3);
			}
			else
			{
				float num4 = BaseSlider<float>.RoundToMultipleOf(value.x + num * 0.5001f, Mathf.Abs(num));
				num4 = Math.Clamp(num4, lowLimit, value.y);
				value = new Vector2(num4, num4 + num2);
			}
			break;
		}
		}
	}

	private void SetSliderValueFromDrag()
	{
		if (clampedDragger.dragDirection == ClampedDragger<float>.DragDirection.Free)
		{
			float x = m_DragElementStartPos.x;
			float dragElementEndPos = x + clampedDragger.delta.x;
			ComputeValueFromDraggingThumb(x, dragElementEndPos);
		}
	}

	private void SetSliderValueFromClick()
	{
		if (clampedDragger.dragDirection == ClampedDragger<float>.DragDirection.Free)
		{
			return;
		}
		Vector2 point = base.visualInput.LocalToWorld(clampedDragger.startMousePosition);
		if (dragMinThumb.worldBound.Contains(point))
		{
			m_DragState = DragState.MinThumb;
		}
		else if (dragMaxThumb.worldBound.Contains(point))
		{
			m_DragState = DragState.MaxThumb;
		}
		else if (clampedDragger.startMousePosition.x > dragElement.layout.xMin && clampedDragger.startMousePosition.x < dragElement.layout.xMax)
		{
			m_DragState = DragState.MiddleThumb;
		}
		else
		{
			m_DragState = DragState.NoThumb;
		}
		if (m_DragState == DragState.NoThumb)
		{
			float num = ComputeValueFromPosition(clampedDragger.startMousePosition.x);
			if (clampedDragger.startMousePosition.x < dragElement.layout.x)
			{
				m_DragState = DragState.MinThumb;
				value = new Vector2(num, value.y);
			}
			else
			{
				m_DragState = DragState.MaxThumb;
				value = new Vector2(value.x, num);
			}
		}
		SetNavigationState(m_DragState);
		m_ValueStartPos = value;
		clampedDragger.dragDirection = ClampedDragger<float>.DragDirection.Free;
		m_DragElementStartPos = clampedDragger.startMousePosition;
	}

	private void ComputeValueFromDraggingThumb(float dragElementStartPos, float dragElementEndPos)
	{
		float num = ComputeValueFromPosition(dragElementStartPos);
		float num2 = ComputeValueFromPosition(dragElementEndPos);
		float num3 = num2 - num;
		SetNavigationState(m_DragState);
		switch (m_DragState)
		{
		case DragState.MiddleThumb:
		{
			Vector2 vector = value;
			vector.x = m_ValueStartPos.x + num3;
			vector.y = m_ValueStartPos.y + num3;
			float num5 = m_ValueStartPos.y - m_ValueStartPos.x;
			if (vector.x < lowLimit)
			{
				vector.x = lowLimit;
				vector.y = lowLimit + num5;
			}
			else if (vector.y > highLimit)
			{
				vector.y = highLimit;
				vector.x = highLimit - num5;
			}
			value = vector;
			break;
		}
		case DragState.MinThumb:
		{
			float num6 = m_ValueStartPos.x + num3;
			if (num6 > maxValue)
			{
				num6 = maxValue;
			}
			else if (num6 < lowLimit)
			{
				num6 = lowLimit;
			}
			value = new Vector2(num6, maxValue);
			break;
		}
		case DragState.MaxThumb:
		{
			float num4 = m_ValueStartPos.y + num3;
			if (num4 < minValue)
			{
				num4 = minValue;
			}
			else if (num4 > highLimit)
			{
				num4 = highLimit;
			}
			value = new Vector2(minValue, num4);
			break;
		}
		}
	}

	protected override void UpdateMixedValueContent()
	{
	}

	internal override void RegisterEditingCallbacks()
	{
		base.visualInput.RegisterCallback<PointerDownEvent>(base.StartEditing, TrickleDown.TrickleDown);
		base.visualInput.RegisterCallback<PointerUpEvent>(base.EndEditing);
	}

	internal override void UnregisterEditingCallbacks()
	{
		base.visualInput.UnregisterCallback<PointerDownEvent>(base.StartEditing, TrickleDown.TrickleDown);
		base.visualInput.UnregisterCallback<PointerUpEvent>(base.EndEditing);
	}
}
