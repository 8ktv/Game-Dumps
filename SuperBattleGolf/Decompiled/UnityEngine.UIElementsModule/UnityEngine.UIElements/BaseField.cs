using System;
using System.Collections.Generic;
using Unity.Properties;
using UnityEngine.Bindings;
using UnityEngine.Internal;

namespace UnityEngine.UIElements;

public abstract class BaseField<TValueType> : BindableElement, INotifyValueChanged<TValueType>, IMixedValueSupport, IPrefixLabel, IEditableElement
{
	[Serializable]
	[ExcludeFromDocs]
	public new abstract class UxmlSerializedData : BindableElement.UxmlSerializedData
	{
		[MultilineTextField]
		[SerializeField]
		private string label;

		[UxmlIgnore]
		[SerializeField]
		[HideInInspector]
		private UxmlAttributeFlags label_UxmlAttributeFlags;

		[SerializeField]
		private TValueType value;

		[UxmlIgnore]
		[HideInInspector]
		[SerializeField]
		private UxmlAttributeFlags value_UxmlAttributeFlags;

		internal TValueType Value
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

		public new static void Register()
		{
			UxmlDescriptionCache.RegisterType(typeof(UxmlSerializedData), new UxmlAttributeNames[2]
			{
				new UxmlAttributeNames("label", "label", null),
				new UxmlAttributeNames("value", "value", null)
			});
		}

		public override void Deserialize(object obj)
		{
			base.Deserialize(obj);
			BaseField<TValueType> baseField = (BaseField<TValueType>)obj;
			if (UnityEngine.UIElements.UxmlSerializedData.ShouldWriteAttributeValue(label_UxmlAttributeFlags))
			{
				baseField.label = label;
			}
			if (UnityEngine.UIElements.UxmlSerializedData.ShouldWriteAttributeValue(value_UxmlAttributeFlags))
			{
				baseField.SetValueWithoutNotify(value);
			}
		}
	}

	[Obsolete("UxmlTraits is deprecated and will be removed. Use UxmlElementAttribute instead.", false)]
	public new class UxmlTraits : BindableElement.UxmlTraits
	{
		private UxmlStringAttributeDescription m_Label = new UxmlStringAttributeDescription
		{
			name = "label"
		};

		public UxmlTraits()
		{
			base.focusIndex.defaultValue = 0;
			base.focusable.defaultValue = true;
		}

		public override void Init(VisualElement ve, IUxmlAttributes bag, CreationContext cc)
		{
			base.Init(ve, bag, cc);
			((BaseField<TValueType>)ve).label = m_Label.GetValueFromBag(bag, cc);
		}
	}

	[VisibleToOtherModules(new string[] { "UnityEditor.UIToolkitAuthoringModule" })]
	internal static readonly BindingId valueProperty = "value";

	internal static readonly BindingId labelProperty = "label";

	internal static readonly BindingId showMixedValueProperty = "showMixedValue";

	public static readonly string ussClassName = "unity-base-field";

	public static readonly string labelUssClassName = ussClassName + "__label";

	public static readonly string inputUssClassName = ussClassName + "__input";

	public static readonly string noLabelVariantUssClassName = ussClassName + "--no-label";

	public static readonly string labelDraggerVariantUssClassName = labelUssClassName + "--with-dragger";

	public static readonly string mixedValueLabelUssClassName = labelUssClassName + "--mixed-value";

	public static readonly string alignedFieldUssClassName = ussClassName + "__aligned";

	private static readonly string inspectorFieldUssClassName = ussClassName + "__inspector-field";

	protected internal static readonly string mixedValueString = "—";

	protected internal static readonly PropertyName serializedPropertyCopyName = "SerializedPropertyCopyName";

	private static CustomStyleProperty<float> s_LabelWidthRatioProperty = new CustomStyleProperty<float>("--unity-property-field-label-width-ratio");

	private static CustomStyleProperty<float> s_LabelExtraPaddingProperty = new CustomStyleProperty<float>("--unity-property-field-label-extra-padding");

	private static CustomStyleProperty<float> s_LabelBaseMinWidthProperty = new CustomStyleProperty<float>("--unity-property-field-label-base-min-width");

	private float m_LabelWidthRatio;

	private float m_LabelExtraPadding;

	private float m_LabelBaseMinWidth;

	private VisualElement m_VisualInput;

	internal Action<ExpressionEvaluator.Expression> expressionEvaluated;

	[SerializeField]
	[DontCreateProperty]
	private TValueType m_Value;

	private bool m_ShowMixedValue;

	private Label m_MixedValueLabel;

	private bool m_SkipValidation;

	private VisualElement m_CachedContextWidthElement;

	private VisualElement m_CachedInspectorElement;

	[VisibleToOtherModules(new string[] { "UnityEditor.UIBuilderModule" })]
	internal VisualElement visualInput
	{
		get
		{
			return m_VisualInput;
		}
		set
		{
			if (m_VisualInput != null)
			{
				if (m_VisualInput.parent == this)
				{
					m_VisualInput.RemoveFromHierarchy();
				}
				m_VisualInput = null;
			}
			if (value != null)
			{
				m_VisualInput = value;
			}
			else
			{
				m_VisualInput = new VisualElement
				{
					pickingMode = PickingMode.Ignore
				};
			}
			m_VisualInput.focusable = true;
			m_VisualInput.AddToClassList(inputUssClassName);
			Add(m_VisualInput);
		}
	}

	protected TValueType rawValue
	{
		get
		{
			return m_Value;
		}
		set
		{
			m_Value = value;
		}
	}

	[VisibleToOtherModules(new string[] { "UnityEditor.UIBuilderModule" })]
	internal DispatchMode dispatchMode { get; set; } = DispatchMode.Default;

	[CreateProperty]
	public virtual TValueType value
	{
		get
		{
			return m_Value;
		}
		set
		{
			if (EqualsCurrentValue(value) && !showMixedValue)
			{
				return;
			}
			TValueType previousValue = m_Value;
			SetValueWithoutNotify(value);
			showMixedValue = false;
			if (base.panel != null)
			{
				using (ChangeEvent<TValueType> changeEvent = ChangeEvent<TValueType>.GetPooled(previousValue, m_Value))
				{
					changeEvent.elementTarget = this;
					SendEvent(changeEvent, dispatchMode);
				}
				NotifyPropertyChanged(in valueProperty);
			}
		}
	}

	public Label labelElement { get; private set; }

	[CreateProperty]
	public string label
	{
		get
		{
			return labelElement.text;
		}
		set
		{
			if (labelElement.text != value)
			{
				labelElement.text = value;
				if (string.IsNullOrEmpty(labelElement.text))
				{
					AddToClassList(noLabelVariantUssClassName);
					labelElement.RemoveFromHierarchy();
				}
				else if (!Contains(labelElement))
				{
					base.hierarchy.Insert(0, labelElement);
					RemoveFromClassList(noLabelVariantUssClassName);
				}
				NotifyPropertyChanged(in labelProperty);
			}
		}
	}

	[CreateProperty]
	public bool showMixedValue
	{
		get
		{
			return m_ShowMixedValue;
		}
		set
		{
			if (value != m_ShowMixedValue && (!value || canSwitchToMixedValue))
			{
				m_ShowMixedValue = value;
				UpdateMixedValueContent();
				NotifyPropertyChanged(in showMixedValueProperty);
			}
		}
	}

	private protected virtual bool canSwitchToMixedValue => true;

	protected Label mixedValueLabel
	{
		get
		{
			if (m_MixedValueLabel == null)
			{
				m_MixedValueLabel = new Label(mixedValueString)
				{
					focusable = true,
					tabIndex = -1
				};
				m_MixedValueLabel.AddToClassList(labelUssClassName);
				m_MixedValueLabel.AddToClassList(mixedValueLabelUssClassName);
			}
			return m_MixedValueLabel;
		}
	}

	Action IEditableElement.editingStarted { get; set; }

	Action IEditableElement.editingEnded { get; set; }

	internal event Action viewDataRestored;

	internal event Func<TValueType, TValueType> onValidateValue;

	[VisibleToOtherModules(new string[] { "UnityEditor.UIBuilderModule" })]
	internal BaseField(string label)
	{
		base.isCompositeRoot = true;
		focusable = true;
		base.tabIndex = 0;
		base.excludeFromFocusRing = true;
		base.delegatesFocus = true;
		AddToClassList(ussClassName);
		labelElement = new Label
		{
			focusable = true,
			tabIndex = -1
		};
		labelElement.AddToClassList(labelUssClassName);
		if (label != null)
		{
			this.label = label;
		}
		else
		{
			AddToClassList(noLabelVariantUssClassName);
		}
		RegisterCallback<AttachToPanelEvent>(OnAttachToPanel);
		RegisterCallback<DetachFromPanelEvent>(OnDetachFromPanel);
		m_VisualInput = null;
	}

	protected BaseField(string label, VisualElement visualInput)
		: this(label)
	{
		this.visualInput = visualInput;
	}

	internal virtual bool EqualsCurrentValue(TValueType value)
	{
		return EqualityComparer<TValueType>.Default.Equals(m_Value, value);
	}

	private void OnAttachToPanel(AttachToPanelEvent e)
	{
		RegisterEditingCallbacks();
		if (e.destinationPanel == null || e.destinationPanel.contextType == ContextType.Player)
		{
			return;
		}
		m_CachedInspectorElement = null;
		m_CachedContextWidthElement = null;
		for (VisualElement visualElement = base.parent; visualElement != null; visualElement = visualElement.parent)
		{
			if (visualElement.ClassListContains("unity-inspector-element"))
			{
				m_CachedInspectorElement = visualElement;
			}
			if (visualElement.ClassListContains("unity-inspector-main-container"))
			{
				m_CachedContextWidthElement = visualElement;
				break;
			}
		}
		if (m_CachedInspectorElement == null)
		{
			RemoveFromClassList(inspectorFieldUssClassName);
			return;
		}
		m_LabelWidthRatio = 0.45f;
		m_LabelExtraPadding = 37f;
		m_LabelBaseMinWidth = 123f;
		RegisterCallback<CustomStyleResolvedEvent>(OnCustomStyleResolved);
		AddToClassList(inspectorFieldUssClassName);
		RegisterCallback<GeometryChangedEvent>(OnInspectorFieldGeometryChanged);
	}

	private void OnDetachFromPanel(DetachFromPanelEvent e)
	{
		UnregisterEditingCallbacks();
		this.onValidateValue = null;
	}

	internal virtual void RegisterEditingCallbacks()
	{
		RegisterCallback<FocusInEvent>(StartEditing);
		RegisterCallback<FocusOutEvent>(EndEditing);
	}

	internal virtual void UnregisterEditingCallbacks()
	{
		UnregisterCallback<FocusInEvent>(StartEditing);
		UnregisterCallback<FocusOutEvent>(EndEditing);
	}

	internal void StartEditing(EventBase e)
	{
		((IEditableElement)this).editingStarted?.Invoke();
	}

	internal void EndEditing(EventBase e)
	{
		((IEditableElement)this).editingEnded?.Invoke();
	}

	private void OnCustomStyleResolved(CustomStyleResolvedEvent evt)
	{
		if (evt.customStyle.TryGetValue(s_LabelWidthRatioProperty, out var labelWidthRatio))
		{
			m_LabelWidthRatio = labelWidthRatio;
		}
		if (evt.customStyle.TryGetValue(s_LabelExtraPaddingProperty, out var labelExtraPadding))
		{
			m_LabelExtraPadding = labelExtraPadding;
		}
		if (evt.customStyle.TryGetValue(s_LabelBaseMinWidthProperty, out var labelBaseMinWidth))
		{
			m_LabelBaseMinWidth = labelBaseMinWidth;
		}
		AlignLabel();
	}

	private void OnInspectorFieldGeometryChanged(GeometryChangedEvent e)
	{
		AlignLabel();
	}

	private void AlignLabel()
	{
		if (ClassListContains(alignedFieldUssClassName) && m_CachedInspectorElement != null)
		{
			float labelExtraPadding = m_LabelExtraPadding;
			float num = base.worldBound.x - m_CachedInspectorElement.worldBound.x - m_CachedInspectorElement.resolvedStyle.paddingLeft;
			labelExtraPadding += num;
			labelExtraPadding += base.resolvedStyle.paddingLeft;
			float a = m_LabelBaseMinWidth - num - base.resolvedStyle.paddingLeft;
			VisualElement visualElement = m_CachedContextWidthElement ?? m_CachedInspectorElement;
			labelElement.style.minWidth = Mathf.Max(a, 0f);
			float num2 = Mathf.Ceil(visualElement.resolvedStyle.width * m_LabelWidthRatio) - labelExtraPadding;
			if (Mathf.Abs(labelElement.resolvedStyle.width - num2) > 1E-30f)
			{
				labelElement.style.width = Mathf.Max(0f, num2);
			}
		}
	}

	private Rect ComputeTooltipRect()
	{
		if (!string.IsNullOrEmpty(label))
		{
			return string.IsNullOrEmpty(labelElement.tooltip) ? labelElement.worldBound : base.worldBound;
		}
		return base.worldBound;
	}

	internal TValueType ValidatedValue(TValueType value)
	{
		if (this.onValidateValue != null)
		{
			return this.onValidateValue(value);
		}
		return value;
	}

	[EventInterest(new Type[] { typeof(TooltipEvent) })]
	protected override void HandleEventBubbleUp(EventBase evt)
	{
		if (!(evt is TooltipEvent tooltipEvent))
		{
			base.HandleEventBubbleUp(evt);
		}
		else if (tooltipEvent.elementTarget == labelElement || !string.IsNullOrEmpty(labelElement?.tooltip) || string.IsNullOrEmpty(label))
		{
			tooltipEvent.rect = ComputeTooltipRect();
		}
		else
		{
			tooltipEvent.StopImmediatePropagation();
		}
	}

	protected virtual void UpdateMixedValueContent()
	{
		throw new NotImplementedException();
	}

	public virtual void SetValueWithoutNotify(TValueType newValue)
	{
		if (m_SkipValidation)
		{
			m_Value = newValue;
		}
		else
		{
			m_Value = ValidatedValue(newValue);
		}
		if (!string.IsNullOrEmpty(base.viewDataKey))
		{
			SaveViewData();
		}
		MarkDirtyRepaint();
		if (showMixedValue)
		{
			UpdateMixedValueContent();
		}
	}

	internal void SetValueWithoutValidation(TValueType newValue)
	{
		m_SkipValidation = true;
		value = newValue;
		m_SkipValidation = false;
	}

	internal override void OnViewDataReady()
	{
		base.OnViewDataReady();
		if (m_VisualInput == null)
		{
			return;
		}
		string fullHierarchicalViewDataKey = GetFullHierarchicalViewDataKey();
		TValueType val = m_Value;
		OverwriteFromViewData(this, fullHierarchicalViewDataKey);
		this.viewDataRestored?.Invoke();
		if (!EqualityComparer<TValueType>.Default.Equals(val, m_Value))
		{
			using (ChangeEvent<TValueType> changeEvent = ChangeEvent<TValueType>.GetPooled(val, m_Value))
			{
				changeEvent.elementTarget = this;
				SetValueWithoutNotify(m_Value);
				SendEvent(changeEvent);
			}
		}
	}
}
