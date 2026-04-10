using System;
using System.Diagnostics;
using Unity.Properties;
using UnityEngine.Bindings;
using UnityEngine.Internal;

namespace UnityEngine.UIElements;

public class ScrollView : VisualElement
{
	[Serializable]
	[ExcludeFromDocs]
	public new class UxmlSerializedData : VisualElement.UxmlSerializedData
	{
		[SerializeField]
		private long elasticAnimationIntervalMs;

		[SerializeField]
		private ScrollViewMode mode;

		[SerializeField]
		private NestedInteractionKind nestedInteractionKind;

		[SerializeField]
		private ScrollerVisibility horizontalScrollerVisibility;

		[SerializeField]
		private ScrollerVisibility verticalScrollerVisibility;

		[SerializeField]
		private float horizontalPageSize;

		[SerializeField]
		private float verticalPageSize;

		[SerializeField]
		private float mouseWheelScrollSize;

		[UxmlAttribute("touch-scroll-type")]
		[SerializeField]
		private TouchScrollBehavior touchScrollBehavior;

		[SerializeField]
		private float scrollDecelerationRate;

		[SerializeField]
		private float elasticity;

		[UxmlAttribute("show-horizontal-scroller")]
		[HideInInspector]
		[SerializeField]
		private bool showHorizontal;

		[SerializeField]
		[HideInInspector]
		[UxmlAttribute("show-vertical-scroller")]
		private bool showVertical;

		[UxmlIgnore]
		[HideInInspector]
		[SerializeField]
		private UxmlAttributeFlags mode_UxmlAttributeFlags;

		[SerializeField]
		[UxmlIgnore]
		[HideInInspector]
		private UxmlAttributeFlags nestedInteractionKind_UxmlAttributeFlags;

		[SerializeField]
		[UxmlIgnore]
		[HideInInspector]
		private UxmlAttributeFlags showHorizontal_UxmlAttributeFlags;

		[HideInInspector]
		[SerializeField]
		[UxmlIgnore]
		private UxmlAttributeFlags showVertical_UxmlAttributeFlags;

		[UxmlIgnore]
		[HideInInspector]
		[SerializeField]
		private UxmlAttributeFlags horizontalScrollerVisibility_UxmlAttributeFlags;

		[SerializeField]
		[UxmlIgnore]
		[HideInInspector]
		private UxmlAttributeFlags verticalScrollerVisibility_UxmlAttributeFlags;

		[HideInInspector]
		[UxmlIgnore]
		[SerializeField]
		private UxmlAttributeFlags horizontalPageSize_UxmlAttributeFlags;

		[UxmlIgnore]
		[HideInInspector]
		[SerializeField]
		private UxmlAttributeFlags verticalPageSize_UxmlAttributeFlags;

		[SerializeField]
		[UxmlIgnore]
		[HideInInspector]
		private UxmlAttributeFlags mouseWheelScrollSize_UxmlAttributeFlags;

		[SerializeField]
		[HideInInspector]
		[UxmlIgnore]
		private UxmlAttributeFlags touchScrollBehavior_UxmlAttributeFlags;

		[SerializeField]
		[HideInInspector]
		[UxmlIgnore]
		private UxmlAttributeFlags scrollDecelerationRate_UxmlAttributeFlags;

		[HideInInspector]
		[SerializeField]
		[UxmlIgnore]
		private UxmlAttributeFlags elasticity_UxmlAttributeFlags;

		[SerializeField]
		[UxmlIgnore]
		[HideInInspector]
		private UxmlAttributeFlags elasticAnimationIntervalMs_UxmlAttributeFlags;

		[Conditional("UNITY_EDITOR")]
		public new static void Register()
		{
			UxmlDescriptionCache.RegisterType(typeof(UxmlSerializedData), new UxmlAttributeNames[13]
			{
				new UxmlAttributeNames("mode", "mode", null),
				new UxmlAttributeNames("nestedInteractionKind", "nested-interaction-kind", null),
				new UxmlAttributeNames("showHorizontal", "show-horizontal-scroller", null),
				new UxmlAttributeNames("showVertical", "show-vertical-scroller", null),
				new UxmlAttributeNames("horizontalScrollerVisibility", "horizontal-scroller-visibility", null),
				new UxmlAttributeNames("verticalScrollerVisibility", "vertical-scroller-visibility", null),
				new UxmlAttributeNames("horizontalPageSize", "horizontal-page-size", null),
				new UxmlAttributeNames("verticalPageSize", "vertical-page-size", null),
				new UxmlAttributeNames("mouseWheelScrollSize", "mouse-wheel-scroll-size", null),
				new UxmlAttributeNames("touchScrollBehavior", "touch-scroll-type", null),
				new UxmlAttributeNames("scrollDecelerationRate", "scroll-deceleration-rate", null),
				new UxmlAttributeNames("elasticity", "elasticity", null),
				new UxmlAttributeNames("elasticAnimationIntervalMs", "elastic-animation-interval-ms", null)
			});
		}

		public override object CreateInstance()
		{
			return new ScrollView();
		}

		public override void Deserialize(object obj)
		{
			base.Deserialize(obj);
			ScrollView scrollView = (ScrollView)obj;
			if (UnityEngine.UIElements.UxmlSerializedData.ShouldWriteAttributeValue(mode_UxmlAttributeFlags))
			{
				scrollView.mode = mode;
			}
			if (UnityEngine.UIElements.UxmlSerializedData.ShouldWriteAttributeValue(horizontalScrollerVisibility_UxmlAttributeFlags))
			{
				scrollView.horizontalScrollerVisibility = horizontalScrollerVisibility;
			}
			else if (UnityEngine.UIElements.UxmlSerializedData.ShouldWriteAttributeValue(showHorizontal_UxmlAttributeFlags))
			{
				scrollView.showHorizontal = showHorizontal;
			}
			if (UnityEngine.UIElements.UxmlSerializedData.ShouldWriteAttributeValue(verticalScrollerVisibility_UxmlAttributeFlags))
			{
				scrollView.verticalScrollerVisibility = verticalScrollerVisibility;
			}
			else if (UnityEngine.UIElements.UxmlSerializedData.ShouldWriteAttributeValue(showVertical_UxmlAttributeFlags))
			{
				scrollView.showVertical = showVertical;
			}
			if (UnityEngine.UIElements.UxmlSerializedData.ShouldWriteAttributeValue(nestedInteractionKind_UxmlAttributeFlags))
			{
				scrollView.nestedInteractionKind = nestedInteractionKind;
			}
			if (UnityEngine.UIElements.UxmlSerializedData.ShouldWriteAttributeValue(horizontalPageSize_UxmlAttributeFlags))
			{
				scrollView.horizontalPageSize = horizontalPageSize;
			}
			if (UnityEngine.UIElements.UxmlSerializedData.ShouldWriteAttributeValue(verticalPageSize_UxmlAttributeFlags))
			{
				scrollView.verticalPageSize = verticalPageSize;
			}
			if (UnityEngine.UIElements.UxmlSerializedData.ShouldWriteAttributeValue(mouseWheelScrollSize_UxmlAttributeFlags))
			{
				scrollView.mouseWheelScrollSize = mouseWheelScrollSize;
			}
			if (UnityEngine.UIElements.UxmlSerializedData.ShouldWriteAttributeValue(scrollDecelerationRate_UxmlAttributeFlags))
			{
				scrollView.scrollDecelerationRate = scrollDecelerationRate;
			}
			if (UnityEngine.UIElements.UxmlSerializedData.ShouldWriteAttributeValue(touchScrollBehavior_UxmlAttributeFlags))
			{
				scrollView.touchScrollBehavior = touchScrollBehavior;
			}
			if (UnityEngine.UIElements.UxmlSerializedData.ShouldWriteAttributeValue(elasticity_UxmlAttributeFlags))
			{
				scrollView.elasticity = elasticity;
			}
			if (UnityEngine.UIElements.UxmlSerializedData.ShouldWriteAttributeValue(elasticAnimationIntervalMs_UxmlAttributeFlags))
			{
				scrollView.elasticAnimationIntervalMs = elasticAnimationIntervalMs;
			}
		}
	}

	[Obsolete("UxmlFactory is deprecated and will be removed. Use UxmlElementAttribute instead.", false)]
	public new class UxmlFactory : UxmlFactory<ScrollView, UxmlTraits>
	{
	}

	[Obsolete("UxmlTraits is deprecated and will be removed. Use UxmlElementAttribute instead.", false)]
	public new class UxmlTraits : VisualElement.UxmlTraits
	{
		private UxmlEnumAttributeDescription<ScrollViewMode> m_ScrollViewMode = new UxmlEnumAttributeDescription<ScrollViewMode>
		{
			name = "mode",
			defaultValue = ScrollViewMode.Vertical
		};

		private UxmlEnumAttributeDescription<NestedInteractionKind> m_NestedInteractionKind = new UxmlEnumAttributeDescription<NestedInteractionKind>
		{
			name = "nested-interaction-kind",
			defaultValue = NestedInteractionKind.Default
		};

		private UxmlBoolAttributeDescription m_ShowHorizontal = new UxmlBoolAttributeDescription
		{
			name = "show-horizontal-scroller"
		};

		private UxmlBoolAttributeDescription m_ShowVertical = new UxmlBoolAttributeDescription
		{
			name = "show-vertical-scroller"
		};

		private UxmlEnumAttributeDescription<ScrollerVisibility> m_HorizontalScrollerVisibility = new UxmlEnumAttributeDescription<ScrollerVisibility>
		{
			name = "horizontal-scroller-visibility"
		};

		private UxmlEnumAttributeDescription<ScrollerVisibility> m_VerticalScrollerVisibility = new UxmlEnumAttributeDescription<ScrollerVisibility>
		{
			name = "vertical-scroller-visibility"
		};

		private UxmlFloatAttributeDescription m_HorizontalPageSize = new UxmlFloatAttributeDescription
		{
			name = "horizontal-page-size",
			defaultValue = -1f
		};

		private UxmlFloatAttributeDescription m_VerticalPageSize = new UxmlFloatAttributeDescription
		{
			name = "vertical-page-size",
			defaultValue = -1f
		};

		private UxmlFloatAttributeDescription m_MouseWheelScrollSize = new UxmlFloatAttributeDescription
		{
			name = "mouse-wheel-scroll-size",
			defaultValue = 18f
		};

		private UxmlEnumAttributeDescription<TouchScrollBehavior> m_TouchScrollBehavior = new UxmlEnumAttributeDescription<TouchScrollBehavior>
		{
			name = "touch-scroll-type",
			defaultValue = TouchScrollBehavior.Clamped
		};

		private UxmlFloatAttributeDescription m_ScrollDecelerationRate = new UxmlFloatAttributeDescription
		{
			name = "scroll-deceleration-rate",
			defaultValue = k_DefaultScrollDecelerationRate
		};

		private UxmlFloatAttributeDescription m_Elasticity = new UxmlFloatAttributeDescription
		{
			name = "elasticity",
			defaultValue = k_DefaultElasticity
		};

		public override void Init(VisualElement ve, IUxmlAttributes bag, CreationContext cc)
		{
			base.Init(ve, bag, cc);
			ScrollView scrollView = (ScrollView)ve;
			scrollView.mode = m_ScrollViewMode.GetValueFromBag(bag, cc);
			ScrollerVisibility value = ScrollerVisibility.Auto;
			if (m_HorizontalScrollerVisibility.TryGetValueFromBag(bag, cc, ref value))
			{
				scrollView.horizontalScrollerVisibility = value;
			}
			else
			{
				scrollView.showHorizontal = m_ShowHorizontal.GetValueFromBag(bag, cc);
			}
			ScrollerVisibility value2 = ScrollerVisibility.Auto;
			if (m_VerticalScrollerVisibility.TryGetValueFromBag(bag, cc, ref value2))
			{
				scrollView.verticalScrollerVisibility = value2;
			}
			else
			{
				scrollView.showVertical = m_ShowVertical.GetValueFromBag(bag, cc);
			}
			scrollView.nestedInteractionKind = m_NestedInteractionKind.GetValueFromBag(bag, cc);
			scrollView.horizontalPageSize = m_HorizontalPageSize.GetValueFromBag(bag, cc);
			scrollView.verticalPageSize = m_VerticalPageSize.GetValueFromBag(bag, cc);
			scrollView.mouseWheelScrollSize = m_MouseWheelScrollSize.GetValueFromBag(bag, cc);
			scrollView.scrollDecelerationRate = m_ScrollDecelerationRate.GetValueFromBag(bag, cc);
			scrollView.touchScrollBehavior = m_TouchScrollBehavior.GetValueFromBag(bag, cc);
			scrollView.elasticity = m_Elasticity.GetValueFromBag(bag, cc);
		}
	}

	public enum TouchScrollBehavior
	{
		Unrestricted,
		Elastic,
		Clamped
	}

	public enum NestedInteractionKind
	{
		Default,
		StopScrolling,
		ForwardScrolling
	}

	internal enum TouchScrollingResult
	{
		Apply,
		Forward,
		Block
	}

	internal static readonly BindingId horizontalScrollerVisibilityProperty = "horizontalScrollerVisibility";

	internal static readonly BindingId verticalScrollerVisibilityProperty = "verticalScrollerVisibility";

	internal static readonly BindingId scrollOffsetProperty = "scrollOffset";

	internal static readonly BindingId horizontalPageSizeProperty = "horizontalPageSize";

	internal static readonly BindingId verticalPageSizeProperty = "verticalPageSize";

	internal static readonly BindingId mouseWheelScrollSizeProperty = "mouseWheelScrollSize";

	internal static readonly BindingId scrollDecelerationRateProperty = "scrollDecelerationRate";

	internal static readonly BindingId elasticityProperty = "elasticity";

	internal static readonly BindingId touchScrollBehaviorProperty = "touchScrollBehavior";

	internal static readonly BindingId nestedInteractionKindProperty = "nestedInteractionKind";

	internal static readonly BindingId modeProperty = "mode";

	internal static readonly BindingId elasticAnimationIntervalMsProperty = "elasticAnimationIntervalMs";

	private VisualElement m_DeferredScrollToElement;

	private IVisualElementScheduledItem m_DeferredScrollTo;

	private const int k_MaxLocalLayoutPassCount = 5;

	private int m_FirstLayoutPass = -1;

	private ScrollerVisibility m_HorizontalScrollerVisibility;

	private ScrollerVisibility m_VerticalScrollerVisibility;

	private long m_ElasticAnimationIntervalMs = 16L;

	private const float k_SizeThreshold = 0.001f;

	private VisualElement m_AttachedRootVisualContainer;

	private float m_SingleLineHeight = UIElementsUtility.singleLineHeight;

	private bool m_SingleLineHeightDirtyFlag;

	private const string k_SingleLineHeightPropertyName = "--unity-metrics-single_line-height";

	private const float k_ScrollPageOverlapFactor = 0.1f;

	internal const float k_UnsetPageSizeValue = -1f;

	internal const float k_MouseWheelScrollSizeDefaultValue = 18f;

	internal const float k_MouseWheelScrollSizeUnset = -1f;

	internal bool m_MouseWheelScrollSizeIsInline;

	[SerializeField]
	[DontCreateProperty]
	private Vector2 m_ScrollOffset;

	private float m_HorizontalPageSize;

	private float m_VerticalPageSize;

	private float m_MouseWheelScrollSize = 18f;

	private static readonly float k_DefaultScrollDecelerationRate = 0.135f;

	private float m_ScrollDecelerationRate = k_DefaultScrollDecelerationRate;

	private float k_ScaledPixelsPerPointMultiplier = 10f;

	private float k_TouchScrollInertiaBaseTimeInterval = 0.004167f;

	private static readonly float k_DefaultElasticity = 0.1f;

	private float m_Elasticity = k_DefaultElasticity;

	private TouchScrollBehavior m_TouchScrollBehavior;

	private NestedInteractionKind m_NestedInteractionKind;

	private VisualElement m_ContentContainer;

	private VisualElement m_ContentAndVerticalScrollContainer;

	private float previousVerticalTouchScrollTimeStamp = 0f;

	private float previousHorizontalTouchScrollTimeStamp = 0f;

	private float elapsedTimeSinceLastVerticalTouchScroll = 0f;

	private float elapsedTimeSinceLastHorizontalTouchScroll = 0f;

	public static readonly string ussClassName = "unity-scroll-view";

	public static readonly string viewportUssClassName = ussClassName + "__content-viewport";

	public static readonly string horizontalVariantViewportUssClassName = viewportUssClassName + "--horizontal";

	public static readonly string verticalVariantViewportUssClassName = viewportUssClassName + "--vertical";

	public static readonly string verticalHorizontalVariantViewportUssClassName = viewportUssClassName + "--vertical-horizontal";

	public static readonly string contentAndVerticalScrollUssClassName = ussClassName + "__content-and-vertical-scroll-container";

	public static readonly string contentUssClassName = ussClassName + "__content-container";

	public static readonly string horizontalVariantContentUssClassName = contentUssClassName + "--horizontal";

	public static readonly string verticalVariantContentUssClassName = contentUssClassName + "--vertical";

	public static readonly string verticalHorizontalVariantContentUssClassName = contentUssClassName + "--vertical-horizontal";

	public static readonly string hScrollerUssClassName = ussClassName + "__horizontal-scroller";

	public static readonly string vScrollerUssClassName = ussClassName + "__vertical-scroller";

	public static readonly string horizontalVariantUssClassName = ussClassName + "--horizontal";

	public static readonly string verticalVariantUssClassName = ussClassName + "--vertical";

	public static readonly string verticalHorizontalVariantUssClassName = ussClassName + "--vertical-horizontal";

	public static readonly string scrollVariantUssClassName = ussClassName + "--scroll";

	private ScrollViewMode m_Mode;

	private IVisualElementScheduledItem m_ScheduledLayoutPassResetItem;

	private const float k_VelocityLerpTimeFactor = 10f;

	internal const float ScrollThresholdSquared = 100f;

	private Vector2 m_StartPosition;

	private Vector2 m_PointerStartPosition;

	private Vector2 m_Velocity;

	private Vector2 m_SpringBackVelocity;

	private Vector2 m_LowBounds;

	private Vector2 m_HighBounds;

	private float m_LastVelocityLerpTime;

	private bool m_StartedMoving;

	internal bool m_TouchDraggingAllowed = true;

	private bool m_TouchPointerMoveAllowed;

	private bool m_TouchStoppedVelocity;

	private VisualElement m_CapturedTarget;

	private EventCallback<PointerMoveEvent> m_CapturedTargetPointerMoveCallback;

	private EventCallback<PointerUpEvent> m_CapturedTargetPointerUpCallback;

	internal IVisualElementScheduledItem m_PostPointerUpAnimation;

	[CreateProperty]
	public ScrollerVisibility horizontalScrollerVisibility
	{
		get
		{
			return m_HorizontalScrollerVisibility;
		}
		set
		{
			ScrollerVisibility scrollerVisibility = m_HorizontalScrollerVisibility;
			m_HorizontalScrollerVisibility = value;
			UpdateScrollers(needsHorizontal, needsVertical);
			if (scrollerVisibility != m_HorizontalScrollerVisibility)
			{
				NotifyPropertyChanged(in horizontalScrollerVisibilityProperty);
			}
		}
	}

	[CreateProperty]
	public ScrollerVisibility verticalScrollerVisibility
	{
		get
		{
			return m_VerticalScrollerVisibility;
		}
		set
		{
			ScrollerVisibility scrollerVisibility = m_VerticalScrollerVisibility;
			m_VerticalScrollerVisibility = value;
			UpdateScrollers(needsHorizontal, needsVertical);
			if (scrollerVisibility != m_VerticalScrollerVisibility)
			{
				NotifyPropertyChanged(in verticalScrollerVisibilityProperty);
			}
		}
	}

	[CreateProperty]
	public long elasticAnimationIntervalMs
	{
		get
		{
			return m_ElasticAnimationIntervalMs;
		}
		set
		{
			long num = m_ElasticAnimationIntervalMs;
			m_ElasticAnimationIntervalMs = value;
			if (num != m_ElasticAnimationIntervalMs)
			{
				NotifyPropertyChanged(in elasticAnimationIntervalMsProperty);
				m_PostPointerUpAnimation = base.schedule.Execute(PostPointerUpAnimation).Every(m_ElasticAnimationIntervalMs);
			}
		}
	}

	[Obsolete("showHorizontal is obsolete. Use horizontalScrollerVisibility instead")]
	public bool showHorizontal
	{
		get
		{
			return horizontalScrollerVisibility == ScrollerVisibility.AlwaysVisible;
		}
		set
		{
			m_HorizontalScrollerVisibility = (value ? ScrollerVisibility.AlwaysVisible : ScrollerVisibility.Auto);
		}
	}

	[Obsolete("showVertical is obsolete. Use verticalScrollerVisibility instead")]
	public bool showVertical
	{
		get
		{
			return verticalScrollerVisibility == ScrollerVisibility.AlwaysVisible;
		}
		set
		{
			m_VerticalScrollerVisibility = (value ? ScrollerVisibility.AlwaysVisible : ScrollerVisibility.Auto);
		}
	}

	internal bool needsHorizontal => (mode != ScrollViewMode.Vertical && horizontalScrollerVisibility == ScrollerVisibility.AlwaysVisible) || (horizontalScrollerVisibility == ScrollerVisibility.Auto && scrollableWidth > 0.001f);

	internal bool needsVertical
	{
		[VisibleToOtherModules(new string[] { "UnityEditor.UIBuilderModule" })]
		get
		{
			return (mode != ScrollViewMode.Horizontal && verticalScrollerVisibility == ScrollerVisibility.AlwaysVisible) || (verticalScrollerVisibility == ScrollerVisibility.Auto && scrollableHeight > 0.001f);
		}
	}

	internal bool isVerticalScrollDisplayed => verticalScroller.resolvedStyle.display == DisplayStyle.Flex;

	internal bool isHorizontalScrollDisplayed => horizontalScroller.resolvedStyle.display == DisplayStyle.Flex;

	[CreateProperty]
	public Vector2 scrollOffset
	{
		get
		{
			return m_ScrollOffset;
		}
		set
		{
			if (!(value != m_ScrollOffset))
			{
				return;
			}
			horizontalScroller.value = value.x;
			verticalScroller.value = value.y;
			m_ScrollOffset = new Vector2(horizontalScroller.value, verticalScroller.value);
			SaveViewData();
			if (base.panel != null)
			{
				if (!Mathf.Approximately(m_ContentAndVerticalScrollContainer.layout.height - contentViewport.layout.height, 0f))
				{
					UpdateScrollers(needsHorizontal, needsVertical);
				}
				UpdateContentViewTransform();
			}
			NotifyPropertyChanged(in scrollOffsetProperty);
		}
	}

	[CreateProperty]
	public float horizontalPageSize
	{
		get
		{
			return m_HorizontalPageSize;
		}
		set
		{
			float a = m_HorizontalPageSize;
			m_HorizontalPageSize = value;
			UpdateHorizontalSliderPageSize();
			if (!Mathf.Approximately(a, m_HorizontalPageSize))
			{
				NotifyPropertyChanged(in horizontalPageSizeProperty);
			}
		}
	}

	[CreateProperty]
	public float verticalPageSize
	{
		get
		{
			return m_VerticalPageSize;
		}
		set
		{
			float a = m_VerticalPageSize;
			m_VerticalPageSize = value;
			UpdateVerticalSliderPageSize();
			if (!Mathf.Approximately(a, m_VerticalPageSize))
			{
				NotifyPropertyChanged(in verticalPageSizeProperty);
			}
		}
	}

	[CreateProperty]
	public float mouseWheelScrollSize
	{
		get
		{
			return m_MouseWheelScrollSize;
		}
		set
		{
			float num = m_MouseWheelScrollSize;
			if (Math.Abs(m_MouseWheelScrollSize - value) > float.Epsilon)
			{
				m_MouseWheelScrollSizeIsInline = true;
				m_MouseWheelScrollSize = value;
				NotifyPropertyChanged(in mouseWheelScrollSizeProperty);
			}
		}
	}

	internal float scrollableWidth => contentContainer.boundingBox.width - contentViewport.layout.width;

	internal float scrollableHeight => contentContainer.boundingBox.height - contentViewport.layout.height;

	private bool hasInertia => scrollDecelerationRate > 0f;

	[CreateProperty]
	public float scrollDecelerationRate
	{
		get
		{
			return m_ScrollDecelerationRate;
		}
		set
		{
			float a = m_ScrollDecelerationRate;
			m_ScrollDecelerationRate = Mathf.Max(0f, value);
			if (!Mathf.Approximately(a, m_ScrollDecelerationRate))
			{
				NotifyPropertyChanged(in scrollDecelerationRateProperty);
			}
		}
	}

	[CreateProperty]
	public float elasticity
	{
		get
		{
			return m_Elasticity;
		}
		set
		{
			float a = m_Elasticity;
			m_Elasticity = Mathf.Max(0f, value);
			if (!Mathf.Approximately(a, m_Elasticity))
			{
				NotifyPropertyChanged(in elasticityProperty);
			}
		}
	}

	[CreateProperty]
	public TouchScrollBehavior touchScrollBehavior
	{
		get
		{
			return m_TouchScrollBehavior;
		}
		set
		{
			TouchScrollBehavior touchScrollBehavior = m_TouchScrollBehavior;
			m_TouchScrollBehavior = value;
			if (m_TouchScrollBehavior == TouchScrollBehavior.Clamped)
			{
				horizontalScroller.slider.clamped = true;
				verticalScroller.slider.clamped = true;
			}
			else
			{
				horizontalScroller.slider.clamped = false;
				verticalScroller.slider.clamped = false;
			}
			if (touchScrollBehavior != m_TouchScrollBehavior)
			{
				NotifyPropertyChanged(in touchScrollBehaviorProperty);
			}
		}
	}

	[CreateProperty]
	public NestedInteractionKind nestedInteractionKind
	{
		get
		{
			return m_NestedInteractionKind;
		}
		set
		{
			NestedInteractionKind nestedInteractionKind = m_NestedInteractionKind;
			m_NestedInteractionKind = value;
			if (nestedInteractionKind != m_NestedInteractionKind)
			{
				NotifyPropertyChanged(in nestedInteractionKindProperty);
			}
		}
	}

	public VisualElement contentViewport { get; }

	public Scroller horizontalScroller { get; }

	public Scroller verticalScroller { get; }

	public override VisualElement contentContainer => m_ContentContainer;

	[CreateProperty]
	public ScrollViewMode mode
	{
		get
		{
			return m_Mode;
		}
		set
		{
			ScrollViewMode scrollViewMode = m_Mode;
			SetScrollViewMode(value);
			if (scrollViewMode != m_Mode)
			{
				NotifyPropertyChanged(in modeProperty);
			}
		}
	}

	private void OnHorizontalScrollDragElementChanged(GeometryChangedEvent evt)
	{
		if (!(evt.oldRect.size == evt.newRect.size))
		{
			UpdateHorizontalSliderPageSize();
		}
	}

	private void OnVerticalScrollDragElementChanged(GeometryChangedEvent evt)
	{
		if (!(evt.oldRect.size == evt.newRect.size))
		{
			UpdateVerticalSliderPageSize();
		}
	}

	private void UpdateHorizontalSliderPageSize()
	{
		float width = horizontalScroller.resolvedStyle.width;
		float num = m_HorizontalPageSize;
		if (width > 0f && Mathf.Approximately(m_HorizontalPageSize, -1f))
		{
			float width2 = horizontalScroller.slider.dragElement.resolvedStyle.width;
			num = width2 * 0.9f;
		}
		if (num >= 0f)
		{
			horizontalScroller.slider.pageSize = num;
		}
	}

	private void UpdateVerticalSliderPageSize()
	{
		float height = verticalScroller.resolvedStyle.height;
		float num = m_VerticalPageSize;
		if (height > 0f && Mathf.Approximately(m_VerticalPageSize, -1f))
		{
			float height2 = verticalScroller.slider.dragElement.resolvedStyle.height;
			num = height2 * 0.9f;
		}
		if (num >= 0f)
		{
			verticalScroller.slider.pageSize = num;
		}
	}

	internal void UpdateContentViewTransform()
	{
		Vector3 translate = contentContainer.resolvedStyle.translate;
		Vector2 vector = scrollOffset;
		if (needsVertical)
		{
			vector.y += contentContainer.resolvedStyle.top;
		}
		translate.x = this.RoundToPanelPixelSize(0f - vector.x);
		translate.y = this.RoundToPanelPixelSize(0f - vector.y);
		contentContainer.style.translate = translate;
		IncrementVersion(VersionChangeType.Repaint);
	}

	public void ScrollTo(VisualElement child)
	{
		if (child == null)
		{
			throw new ArgumentNullException("child");
		}
		if (!contentContainer.Contains(child))
		{
			throw new ArgumentException("Cannot scroll to a VisualElement that's not a child of the ScrollView content-container.");
		}
		if (ShouldDeferScrollTo())
		{
			StartDeferredScrollTo(child);
		}
		else
		{
			StopDeferredScrollTo();
		}
		m_Velocity = Vector2.zero;
		float num = 0f;
		float num2 = 0f;
		if (scrollableHeight > 0f)
		{
			num = GetYDeltaOffset(child);
			verticalScroller.value = scrollOffset.y + num;
		}
		if (scrollableWidth > 0f)
		{
			num2 = GetXDeltaOffset(child);
			horizontalScroller.value = scrollOffset.x + num2;
		}
		if (num != 0f || num2 != 0f)
		{
			UpdateContentViewTransform();
		}
	}

	private bool ShouldDeferScrollTo()
	{
		return contentContainer.panel.isDirty;
	}

	private bool ShouldStopDeferredScrollTo()
	{
		return !ShouldDeferScrollTo();
	}

	private void StartDeferredScrollTo(VisualElement target)
	{
		m_DeferredScrollToElement = target;
		if (m_DeferredScrollTo == null)
		{
			m_DeferredScrollTo = base.schedule.Execute(PerformDeferredScrollTo).Until(ShouldStopDeferredScrollTo);
		}
		else if (!m_DeferredScrollTo.isActive)
		{
			m_DeferredScrollTo.Resume();
		}
	}

	private void StopDeferredScrollTo()
	{
		m_DeferredScrollToElement = null;
		if (m_DeferredScrollTo != null && m_DeferredScrollTo.isActive)
		{
			m_DeferredScrollTo.Pause();
		}
	}

	private void PerformDeferredScrollTo()
	{
		if (m_DeferredScrollToElement != null)
		{
			if (!contentContainer.Contains(m_DeferredScrollToElement))
			{
				StopDeferredScrollTo();
			}
			else
			{
				ScrollTo(m_DeferredScrollToElement);
			}
		}
		else
		{
			StopDeferredScrollTo();
		}
	}

	private float GetXDeltaOffset(VisualElement child)
	{
		float num = contentContainer.resolvedStyle.translate.x * -1f;
		Rect rect = contentViewport.worldBound;
		float num2 = rect.xMin + num;
		float num3 = rect.xMax + num;
		Rect rect2 = child.worldBound;
		float num4 = rect2.xMin + num;
		float num5 = rect2.xMax + num;
		if ((num4 >= num2 && num5 <= num3) || float.IsNaN(num4) || float.IsNaN(num5))
		{
			return 0f;
		}
		float deltaDistance = GetDeltaDistance(num2, num3, num4, num5);
		return deltaDistance * horizontalScroller.highValue / scrollableWidth;
	}

	private float GetYDeltaOffset(VisualElement child)
	{
		float num = contentContainer.resolvedStyle.translate.y * -1f;
		Rect rect = contentViewport.worldBound;
		float num2 = rect.yMin + num;
		float num3 = rect.yMax + num;
		Rect rect2 = child.worldBound;
		float num4 = rect2.yMin + num;
		float num5 = rect2.yMax + num;
		if ((num4 >= num2 && num5 <= num3) || float.IsNaN(num4) || float.IsNaN(num5))
		{
			return 0f;
		}
		float deltaDistance = GetDeltaDistance(num2, num3, num4, num5);
		return deltaDistance * verticalScroller.highValue / scrollableHeight;
	}

	private float GetDeltaDistance(float viewMin, float viewMax, float childBoundaryMin, float childBoundaryMax)
	{
		float num = viewMax - viewMin;
		float num2 = childBoundaryMax - childBoundaryMin;
		if (num2 > num)
		{
			if (viewMin > childBoundaryMin && childBoundaryMax > viewMax)
			{
				return 0f;
			}
			return (childBoundaryMin > viewMin) ? (childBoundaryMin - viewMin) : (childBoundaryMax - viewMax);
		}
		float num3 = childBoundaryMax - viewMax;
		if (num3 < -1f)
		{
			num3 = childBoundaryMin - viewMin;
		}
		return num3;
	}

	public ScrollView()
		: this(ScrollViewMode.Vertical)
	{
	}

	public ScrollView(ScrollViewMode scrollViewMode)
	{
		AddToClassList(ussClassName);
		m_ContentAndVerticalScrollContainer = new VisualElement
		{
			name = "unity-content-and-vertical-scroll-container"
		};
		m_ContentAndVerticalScrollContainer.AddToClassList(contentAndVerticalScrollUssClassName);
		base.hierarchy.Add(m_ContentAndVerticalScrollContainer);
		contentViewport = new VisualElement
		{
			name = "unity-content-viewport"
		};
		contentViewport.AddToClassList(viewportUssClassName);
		contentViewport.RegisterCallback<GeometryChangedEvent>(OnGeometryChanged);
		contentViewport.pickingMode = PickingMode.Ignore;
		m_ContentAndVerticalScrollContainer.RegisterCallback<AttachToPanelEvent>(OnAttachToPanel);
		m_ContentAndVerticalScrollContainer.RegisterCallback<DetachFromPanelEvent>(OnDetachFromPanel);
		m_ContentAndVerticalScrollContainer.Add(contentViewport);
		m_ContentContainer = new VisualElement
		{
			name = "unity-content-container"
		};
		m_ContentContainer.disableClipping = true;
		m_ContentContainer.RegisterCallback<GeometryChangedEvent>(OnGeometryChanged);
		m_ContentContainer.AddToClassList(contentUssClassName);
		m_ContentContainer.usageHints = UsageHints.GroupTransform;
		contentViewport.Add(m_ContentContainer);
		SetScrollViewMode(scrollViewMode);
		horizontalScroller = new Scroller(0f, 2.1474836E+09f, delegate(float value)
		{
			scrollOffset = new Vector2(value, scrollOffset.y);
			UpdateContentViewTransform();
		}, SliderDirection.Horizontal)
		{
			viewDataKey = "HorizontalScroller"
		};
		horizontalScroller.AddToClassList(hScrollerUssClassName);
		horizontalScroller.style.display = DisplayStyle.None;
		base.hierarchy.Add(horizontalScroller);
		verticalScroller = new Scroller(0f, 2.1474836E+09f, delegate(float value)
		{
			scrollOffset = new Vector2(scrollOffset.x, value);
			UpdateContentViewTransform();
		})
		{
			viewDataKey = "VerticalScroller"
		};
		verticalScroller.slider.viewDataRestored += OnVerticalSliderViewDataRestored;
		horizontalScroller.slider.viewDataRestored += OnHorizontalSliderViewDataRestored;
		horizontalScroller.slider.onSetValueWithoutNotify += OnHorizontalScrollerSetValueWithoutNotify;
		verticalScroller.slider.onSetValueWithoutNotify += OnVerticalScrollerSetValueWithoutNotify;
		horizontalScroller.slider.clampedDragger.draggingEnded += UpdateElasticBehaviour;
		verticalScroller.slider.clampedDragger.draggingEnded += UpdateElasticBehaviour;
		horizontalScroller.lowButton.AddAction(UpdateElasticBehaviour);
		horizontalScroller.highButton.AddAction(UpdateElasticBehaviour);
		verticalScroller.lowButton.AddAction(UpdateElasticBehaviour);
		verticalScroller.highButton.AddAction(UpdateElasticBehaviour);
		verticalScroller.AddToClassList(vScrollerUssClassName);
		verticalScroller.style.display = DisplayStyle.None;
		m_ContentAndVerticalScrollContainer.Add(verticalScroller);
		touchScrollBehavior = TouchScrollBehavior.Clamped;
		RegisterCallback<WheelEvent>(OnScrollWheel, InvokePolicy.IncludeDisabled);
		verticalScroller.RegisterCallback<GeometryChangedEvent>(OnScrollersGeometryChanged);
		horizontalScroller.RegisterCallback<GeometryChangedEvent>(OnScrollersGeometryChanged);
		horizontalPageSize = -1f;
		verticalPageSize = -1f;
		horizontalScroller.slider.dragElement.RegisterCallback<GeometryChangedEvent>(OnHorizontalScrollDragElementChanged);
		verticalScroller.slider.dragElement.RegisterCallback<GeometryChangedEvent>(OnVerticalScrollDragElementChanged);
		m_CapturedTargetPointerMoveCallback = OnPointerMove;
		m_CapturedTargetPointerUpCallback = OnPointerUp;
		scrollOffset = Vector2.zero;
		m_ContentContainer.receivesHierarchyGeometryChangedEvents = true;
	}

	private void SetScrollViewMode(ScrollViewMode mode)
	{
		m_Mode = mode;
		RemoveFromClassList(verticalVariantUssClassName);
		RemoveFromClassList(horizontalVariantUssClassName);
		RemoveFromClassList(verticalHorizontalVariantUssClassName);
		RemoveFromClassList(scrollVariantUssClassName);
		contentContainer.RemoveFromClassList(verticalVariantContentUssClassName);
		contentContainer.RemoveFromClassList(horizontalVariantContentUssClassName);
		contentContainer.RemoveFromClassList(verticalHorizontalVariantContentUssClassName);
		contentViewport.RemoveFromClassList(verticalVariantViewportUssClassName);
		contentViewport.RemoveFromClassList(horizontalVariantViewportUssClassName);
		contentViewport.RemoveFromClassList(verticalHorizontalVariantViewportUssClassName);
		switch (mode)
		{
		case ScrollViewMode.Vertical:
			AddToClassList(scrollVariantUssClassName);
			AddToClassList(verticalVariantUssClassName);
			contentViewport.AddToClassList(verticalVariantViewportUssClassName);
			contentContainer.AddToClassList(verticalVariantContentUssClassName);
			break;
		case ScrollViewMode.Horizontal:
			AddToClassList(scrollVariantUssClassName);
			AddToClassList(horizontalVariantUssClassName);
			contentViewport.AddToClassList(horizontalVariantViewportUssClassName);
			contentContainer.AddToClassList(horizontalVariantContentUssClassName);
			break;
		case ScrollViewMode.VerticalAndHorizontal:
			AddToClassList(scrollVariantUssClassName);
			AddToClassList(verticalHorizontalVariantUssClassName);
			contentViewport.AddToClassList(verticalHorizontalVariantViewportUssClassName);
			contentContainer.AddToClassList(verticalHorizontalVariantContentUssClassName);
			break;
		}
	}

	private void OnAttachToPanel(AttachToPanelEvent evt)
	{
		if (evt.destinationPanel != null)
		{
			m_AttachedRootVisualContainer = GetRootVisualContainer() ?? evt.destinationPanel.visualTree;
			m_AttachedRootVisualContainer?.RegisterCallback<CustomStyleResolvedEvent>(OnRootCustomStyleResolved);
			RegisterCallback<CustomStyleResolvedEvent>(OnRootCustomStyleResolved);
			MarkSingleLineHeightDirty();
			m_ContentAndVerticalScrollContainer.RegisterCallback<PointerMoveEvent>(OnPointerMove);
			contentContainer.RegisterCallback<PointerDownEvent>(OnPointerDown, TrickleDown.TrickleDown);
			contentContainer.RegisterCallback<PointerCancelEvent>(OnPointerCancel);
			contentContainer.RegisterCallback<PointerUpEvent>(OnPointerUp, TrickleDown.TrickleDown);
			contentContainer.RegisterCallback<PointerCaptureEvent>(OnPointerCapture);
			contentContainer.RegisterCallback<PointerCaptureOutEvent>(OnPointerCaptureOut);
			evt.destinationPanel.visualTree.RegisterCallback<PointerUpEvent>(OnRootPointerUp, TrickleDown.TrickleDown);
		}
	}

	private void OnDetachFromPanel(DetachFromPanelEvent evt)
	{
		m_ScheduledLayoutPassResetItem?.Pause();
		ResetLayoutPass();
		if (evt.originPanel != null)
		{
			m_AttachedRootVisualContainer?.UnregisterCallback<CustomStyleResolvedEvent>(OnRootCustomStyleResolved);
			m_AttachedRootVisualContainer = null;
			UnregisterCallback<CustomStyleResolvedEvent>(OnRootCustomStyleResolved);
			m_ContentAndVerticalScrollContainer.UnregisterCallback<PointerMoveEvent>(OnPointerMove);
			contentContainer.UnregisterCallback<PointerDownEvent>(OnPointerDown, TrickleDown.TrickleDown);
			contentContainer.UnregisterCallback<PointerCancelEvent>(OnPointerCancel);
			contentContainer.UnregisterCallback<PointerUpEvent>(OnPointerUp, TrickleDown.TrickleDown);
			contentContainer.UnregisterCallback<PointerCaptureEvent>(OnPointerCapture);
			contentContainer.UnregisterCallback<PointerCaptureOutEvent>(OnPointerCaptureOut);
			evt.originPanel.visualTree.UnregisterCallback<PointerUpEvent>(OnRootPointerUp, TrickleDown.TrickleDown);
		}
	}

	private void OnPointerCapture(PointerCaptureEvent evt)
	{
		m_CapturedTarget = evt.elementTarget;
		if (m_CapturedTarget != null)
		{
			m_TouchPointerMoveAllowed = true;
			m_CapturedTarget.RegisterCallback(m_CapturedTargetPointerMoveCallback);
			m_CapturedTarget.RegisterCallback(m_CapturedTargetPointerUpCallback);
		}
	}

	private void OnPointerCaptureOut(PointerCaptureOutEvent evt)
	{
		ReleaseScrolling(evt.pointerId, evt.target);
		if (m_CapturedTarget != null)
		{
			m_CapturedTarget.UnregisterCallback(m_CapturedTargetPointerMoveCallback);
			m_CapturedTarget.UnregisterCallback(m_CapturedTargetPointerUpCallback);
			m_CapturedTarget = null;
		}
	}

	private void OnGeometryChanged(GeometryChangedEvent evt)
	{
		if (!(evt.oldRect.size == evt.newRect.size))
		{
			bool flag = needsVertical;
			bool flag2 = needsHorizontal;
			if (m_FirstLayoutPass == -1)
			{
				m_FirstLayoutPass = evt.layoutPass;
			}
			else if (evt.layoutPass - m_FirstLayoutPass > 5)
			{
				flag = flag || isVerticalScrollDisplayed;
				flag2 = flag2 || isHorizontalScrollDisplayed;
			}
			UpdateScrollers(flag2, flag);
			UpdateContentViewTransform();
			ScheduleResetLayoutPass();
		}
	}

	private void OnVerticalSliderViewDataRestored()
	{
		verticalScroller.highValue = (float.IsNaN(scrollableHeight) ? verticalScroller.highValue : scrollableHeight);
		UpdateContentViewTransform();
	}

	private void OnHorizontalSliderViewDataRestored()
	{
		horizontalScroller.highValue = (float.IsNaN(scrollableWidth) ? horizontalScroller.highValue : scrollableWidth);
		UpdateContentViewTransform();
	}

	private void OnVerticalScrollerSetValueWithoutNotify(float value)
	{
		m_ScrollOffset = new Vector2(scrollOffset.x, value);
		SaveViewData();
	}

	private void OnHorizontalScrollerSetValueWithoutNotify(float value)
	{
		m_ScrollOffset = new Vector2(value, scrollOffset.y);
		SaveViewData();
	}

	private void ScheduleResetLayoutPass()
	{
		if (m_ScheduledLayoutPassResetItem == null)
		{
			m_ScheduledLayoutPassResetItem = base.schedule.Execute(ResetLayoutPass);
			return;
		}
		m_ScheduledLayoutPassResetItem.Pause();
		m_ScheduledLayoutPassResetItem.Resume();
	}

	private void ResetLayoutPass()
	{
		m_FirstLayoutPass = -1;
	}

	private static float ComputeElasticOffset(float deltaPointer, float initialScrollOffset, float lowLimit, float hardLowLimit, float highLimit, float hardHighLimit)
	{
		initialScrollOffset = Mathf.Max(initialScrollOffset, hardLowLimit * 0.95f);
		initialScrollOffset = Mathf.Min(initialScrollOffset, hardHighLimit * 0.95f);
		float num3;
		float num;
		if (initialScrollOffset < lowLimit && hardLowLimit < lowLimit)
		{
			num = lowLimit - hardLowLimit;
			float num2 = (lowLimit - initialScrollOffset) / num;
			num3 = num2 * num / (1f - num2);
			num3 += deltaPointer;
			initialScrollOffset = lowLimit;
		}
		else if (initialScrollOffset > highLimit && hardHighLimit > highLimit)
		{
			num = hardHighLimit - highLimit;
			float num4 = (initialScrollOffset - highLimit) / num;
			num3 = -1f * num4 * num / (1f - num4);
			num3 += deltaPointer;
			initialScrollOffset = highLimit;
		}
		else
		{
			num3 = deltaPointer;
		}
		float num5 = initialScrollOffset - num3;
		float num6;
		if (num5 < lowLimit)
		{
			num3 = lowLimit - num5;
			initialScrollOffset = lowLimit;
			num = lowLimit - hardLowLimit;
			num6 = 1f;
		}
		else
		{
			if (num5 <= highLimit)
			{
				return num5;
			}
			num3 = num5 - highLimit;
			initialScrollOffset = highLimit;
			num = hardHighLimit - highLimit;
			num6 = -1f;
		}
		if (Mathf.Abs(num3) < 1E-30f)
		{
			return initialScrollOffset;
		}
		float num7 = num3 / (num3 + num);
		num7 *= num;
		num7 *= num6;
		return initialScrollOffset - num7;
	}

	private void ComputeInitialSpringBackVelocity()
	{
		if (touchScrollBehavior != TouchScrollBehavior.Elastic)
		{
			m_SpringBackVelocity = Vector2.zero;
			return;
		}
		if (scrollOffset.x < m_LowBounds.x)
		{
			m_SpringBackVelocity.x = m_LowBounds.x - scrollOffset.x;
		}
		else if (scrollOffset.x > m_HighBounds.x)
		{
			m_SpringBackVelocity.x = m_HighBounds.x - scrollOffset.x;
		}
		else
		{
			m_SpringBackVelocity.x = 0f;
		}
		if (scrollOffset.y < m_LowBounds.y)
		{
			m_SpringBackVelocity.y = m_LowBounds.y - scrollOffset.y;
		}
		else if (scrollOffset.y > m_HighBounds.y)
		{
			m_SpringBackVelocity.y = m_HighBounds.y - scrollOffset.y;
		}
		else
		{
			m_SpringBackVelocity.y = 0f;
		}
	}

	private void SpringBack()
	{
		if (touchScrollBehavior != TouchScrollBehavior.Elastic)
		{
			m_SpringBackVelocity = Vector2.zero;
			return;
		}
		Vector2 vector = scrollOffset;
		if (vector.x < m_LowBounds.x)
		{
			vector.x = Mathf.SmoothDamp(vector.x, m_LowBounds.x, ref m_SpringBackVelocity.x, elasticity, float.PositiveInfinity, elapsedTimeSinceLastHorizontalTouchScroll);
			if (Mathf.Abs(m_SpringBackVelocity.x) < base.scaledPixelsPerPoint)
			{
				m_SpringBackVelocity.x = 0f;
			}
		}
		else if (vector.x > m_HighBounds.x)
		{
			vector.x = Mathf.SmoothDamp(vector.x, m_HighBounds.x, ref m_SpringBackVelocity.x, elasticity, float.PositiveInfinity, elapsedTimeSinceLastHorizontalTouchScroll);
			if (Mathf.Abs(m_SpringBackVelocity.x) < base.scaledPixelsPerPoint)
			{
				m_SpringBackVelocity.x = 0f;
			}
		}
		else
		{
			m_SpringBackVelocity.x = 0f;
		}
		if (vector.y < m_LowBounds.y)
		{
			vector.y = Mathf.SmoothDamp(vector.y, m_LowBounds.y, ref m_SpringBackVelocity.y, elasticity, float.PositiveInfinity, elapsedTimeSinceLastVerticalTouchScroll);
			if (Mathf.Abs(m_SpringBackVelocity.y) < base.scaledPixelsPerPoint)
			{
				m_SpringBackVelocity.y = 0f;
			}
		}
		else if (vector.y > m_HighBounds.y)
		{
			vector.y = Mathf.SmoothDamp(vector.y, m_HighBounds.y, ref m_SpringBackVelocity.y, elasticity, float.PositiveInfinity, elapsedTimeSinceLastVerticalTouchScroll);
			if (Mathf.Abs(m_SpringBackVelocity.y) < base.scaledPixelsPerPoint)
			{
				m_SpringBackVelocity.y = 0f;
			}
		}
		else
		{
			m_SpringBackVelocity.y = 0f;
		}
		scrollOffset = vector;
	}

	internal void ApplyScrollInertia()
	{
		if (hasInertia && m_Velocity != Vector2.zero)
		{
			Vector2 zero = Vector2.zero;
			float num = 0f;
			while (num < elapsedTimeSinceLastVerticalTouchScroll)
			{
				m_Velocity *= Mathf.Pow(scrollDecelerationRate, k_TouchScrollInertiaBaseTimeInterval);
				num += k_TouchScrollInertiaBaseTimeInterval;
				zero += m_Velocity * k_TouchScrollInertiaBaseTimeInterval;
			}
			float num2 = elapsedTimeSinceLastVerticalTouchScroll - num;
			if (num2 > 0f && num2 < k_TouchScrollInertiaBaseTimeInterval)
			{
				m_Velocity *= Mathf.Pow(scrollDecelerationRate, num2);
				zero += m_Velocity * num2;
			}
			float num3 = base.scaledPixelsPerPoint * k_ScaledPixelsPerPointMultiplier;
			if (Mathf.Abs(m_Velocity.x) <= num3 || (touchScrollBehavior == TouchScrollBehavior.Elastic && (scrollOffset.x < m_LowBounds.x || scrollOffset.x > m_HighBounds.x)))
			{
				m_Velocity.x = 0f;
			}
			if (Mathf.Abs(m_Velocity.y) <= num3 || (touchScrollBehavior == TouchScrollBehavior.Elastic && (scrollOffset.y < m_LowBounds.y || scrollOffset.y > m_HighBounds.y)))
			{
				m_Velocity.y = 0f;
			}
			scrollOffset += zero;
		}
		else
		{
			m_Velocity = Vector2.zero;
		}
	}

	private void PostPointerUpAnimation()
	{
		elapsedTimeSinceLastVerticalTouchScroll = Time.unscaledTime - previousVerticalTouchScrollTimeStamp;
		previousVerticalTouchScrollTimeStamp = Time.unscaledTime;
		elapsedTimeSinceLastHorizontalTouchScroll = Time.unscaledTime - previousHorizontalTouchScrollTimeStamp;
		previousHorizontalTouchScrollTimeStamp = Time.unscaledTime;
		ApplyScrollInertia();
		SpringBack();
		if (m_SpringBackVelocity == Vector2.zero && m_Velocity == Vector2.zero)
		{
			m_PostPointerUpAnimation.Pause();
			elapsedTimeSinceLastVerticalTouchScroll = 0f;
			elapsedTimeSinceLastHorizontalTouchScroll = 0f;
			previousVerticalTouchScrollTimeStamp = 0f;
			previousHorizontalTouchScrollTimeStamp = 0f;
		}
	}

	private void OnPointerDown(PointerDownEvent evt)
	{
		if (evt.pointerType == PointerType.mouse || !m_TouchDraggingAllowed)
		{
			return;
		}
		bool flag = evt.pointerType == PointerType.tracked;
		if (evt.isPrimary || flag)
		{
			if (evt.pointerId != PointerId.invalidPointerId)
			{
				ReleaseScrolling(evt.pointerId, evt.target);
			}
			m_PostPointerUpAnimation?.Pause();
			bool flag2 = Mathf.Abs(m_Velocity.x) > 10f || Mathf.Abs(m_Velocity.y) > 10f;
			m_TouchPointerMoveAllowed = true;
			m_StartedMoving = false;
			InitTouchScrolling(evt.position);
			if (flag2)
			{
				contentContainer.CapturePointer(evt.pointerId);
				contentContainer.panel.PreventCompatibilityMouseEvents(evt.pointerId);
				evt.StopPropagation();
				m_TouchStoppedVelocity = true;
			}
		}
	}

	private void OnPointerMove(PointerMoveEvent evt)
	{
		if (evt.pointerType == PointerType.mouse || !m_TouchDraggingAllowed || !m_TouchPointerMoveAllowed)
		{
			return;
		}
		bool flag = evt.pointerType == PointerType.tracked;
		if (!evt.isPrimary && !flag)
		{
			return;
		}
		if (evt.isHandledByDraggable)
		{
			m_PointerStartPosition = evt.position;
			m_StartPosition = scrollOffset;
			return;
		}
		Vector2 vector = evt.position;
		Vector2 vector2 = vector - m_PointerStartPosition;
		if (flag)
		{
			if (vector2.sqrMagnitude > 100f)
			{
				m_PointerStartPosition = vector;
				m_StartPosition = scrollOffset;
				vector2 = Vector2.zero;
			}
			vector2 *= 100f;
			vector2.y = 0f - vector2.y;
		}
		if (mode == ScrollViewMode.Horizontal)
		{
			vector2.y = 0f;
		}
		else if (mode == ScrollViewMode.Vertical)
		{
			vector2.x = 0f;
		}
		if (!m_TouchStoppedVelocity && !m_StartedMoving && vector2.sqrMagnitude < 100f)
		{
			return;
		}
		Vector2 position = m_PointerStartPosition + vector2;
		TouchScrollingResult touchScrollingResult = ComputeTouchScrolling(position);
		if (touchScrollingResult != TouchScrollingResult.Forward)
		{
			evt.isHandledByDraggable = true;
			evt.StopPropagation();
			if (!contentContainer.HasPointerCapture(evt.pointerId))
			{
				contentContainer.CapturePointer(evt.pointerId);
			}
		}
		else
		{
			m_Velocity = Vector2.zero;
		}
	}

	private void OnPointerCancel(PointerCancelEvent evt)
	{
		ReleaseScrolling(evt.pointerId, evt.target);
	}

	private void OnPointerUp(PointerUpEvent evt)
	{
		if (ReleaseScrolling(evt.pointerId, evt.target))
		{
			contentContainer.panel.PreventCompatibilityMouseEvents(evt.pointerId);
			evt.StopPropagation();
		}
	}

	internal void InitTouchScrolling(Vector2 position)
	{
		m_PointerStartPosition = position;
		m_StartPosition = scrollOffset;
		m_Velocity = Vector2.zero;
		m_SpringBackVelocity = Vector2.zero;
		m_LowBounds = new Vector2(Mathf.Min(horizontalScroller.lowValue, horizontalScroller.highValue), Mathf.Min(verticalScroller.lowValue, verticalScroller.highValue));
		m_HighBounds = new Vector2(Mathf.Max(horizontalScroller.lowValue, horizontalScroller.highValue), Mathf.Max(verticalScroller.lowValue, verticalScroller.highValue));
	}

	internal TouchScrollingResult ComputeTouchScrolling(Vector2 position)
	{
		Vector2 lhs = default(Vector2);
		if (touchScrollBehavior == TouchScrollBehavior.Clamped)
		{
			lhs = m_StartPosition - (position - m_PointerStartPosition);
			lhs = Vector2.Max(lhs, m_LowBounds);
			lhs = Vector2.Min(lhs, m_HighBounds);
		}
		else if (touchScrollBehavior == TouchScrollBehavior.Elastic)
		{
			Vector2 vector = position - m_PointerStartPosition;
			lhs.x = ComputeElasticOffset(vector.x, m_StartPosition.x, m_LowBounds.x, m_LowBounds.x - contentViewport.resolvedStyle.width, m_HighBounds.x, m_HighBounds.x + contentViewport.resolvedStyle.width);
			lhs.y = ComputeElasticOffset(vector.y, m_StartPosition.y, m_LowBounds.y, m_LowBounds.y - contentViewport.resolvedStyle.height, m_HighBounds.y, m_HighBounds.y + contentViewport.resolvedStyle.height);
			previousVerticalTouchScrollTimeStamp = Time.unscaledTime;
			previousHorizontalTouchScrollTimeStamp = Time.unscaledTime;
		}
		else
		{
			lhs = m_StartPosition - (position - m_PointerStartPosition);
		}
		if (mode == ScrollViewMode.Vertical)
		{
			lhs.x = m_LowBounds.x;
		}
		else if (mode == ScrollViewMode.Horizontal)
		{
			lhs.y = m_LowBounds.y;
		}
		if (scrollOffset != lhs)
		{
			return (!ApplyTouchScrolling(lhs)) ? TouchScrollingResult.Forward : TouchScrollingResult.Apply;
		}
		return (!m_StartedMoving || nestedInteractionKind == NestedInteractionKind.ForwardScrolling) ? TouchScrollingResult.Forward : TouchScrollingResult.Block;
	}

	private bool ApplyTouchScrolling(Vector2 newScrollOffset)
	{
		m_StartedMoving = true;
		if (hasInertia)
		{
			if (newScrollOffset == m_LowBounds || newScrollOffset == m_HighBounds)
			{
				m_Velocity = Vector2.zero;
				scrollOffset = newScrollOffset;
				return false;
			}
			if (m_LastVelocityLerpTime > 0f)
			{
				float num = Time.unscaledTime - m_LastVelocityLerpTime;
				m_Velocity = Vector2.Lerp(m_Velocity, Vector2.zero, num * 10f);
			}
			m_LastVelocityLerpTime = Time.unscaledTime;
			float num2 = k_TouchScrollInertiaBaseTimeInterval;
			Vector2 b = (newScrollOffset - scrollOffset) / num2;
			m_Velocity = Vector2.Lerp(m_Velocity, b, num2 * 10f);
		}
		bool result = scrollOffset != newScrollOffset;
		scrollOffset = newScrollOffset;
		return result;
	}

	private bool ReleaseScrolling(int pointerId, IEventHandler target)
	{
		m_TouchStoppedVelocity = false;
		m_StartedMoving = false;
		m_TouchPointerMoveAllowed = false;
		if (target != contentContainer || !contentContainer.HasPointerCapture(pointerId))
		{
			return false;
		}
		previousVerticalTouchScrollTimeStamp = Time.unscaledTime;
		previousHorizontalTouchScrollTimeStamp = Time.unscaledTime;
		if (touchScrollBehavior == TouchScrollBehavior.Elastic || hasInertia)
		{
			ExecuteElasticSpringAnimation();
		}
		contentContainer.ReleasePointer(pointerId);
		return true;
	}

	private void ExecuteElasticSpringAnimation()
	{
		ComputeInitialSpringBackVelocity();
		if (m_PostPointerUpAnimation == null)
		{
			m_PostPointerUpAnimation = base.schedule.Execute(PostPointerUpAnimation).Every(m_ElasticAnimationIntervalMs);
		}
		else
		{
			m_PostPointerUpAnimation.Resume();
		}
	}

	private void AdjustScrollers()
	{
		float factor = ((contentContainer.boundingBox.width > 1E-30f) ? (contentViewport.layout.width / contentContainer.boundingBox.width) : 1f);
		float factor2 = ((contentContainer.boundingBox.height > 1E-30f) ? (contentViewport.layout.height / contentContainer.boundingBox.height) : 1f);
		horizontalScroller.Adjust(factor);
		verticalScroller.Adjust(factor2);
	}

	internal void UpdateScrollers(bool displayHorizontal, bool displayVertical)
	{
		AdjustScrollers();
		bool flag = scrollableHeight > 0f;
		bool flag2 = scrollableWidth > 0f;
		horizontalScroller.SetEnabled(flag2);
		verticalScroller.SetEnabled(flag);
		verticalScroller.slider.clampedDragger.acceptClicksIfDisabled = flag;
		verticalScroller.lowButton.acceptClicksIfDisabled = flag;
		verticalScroller.highButton.acceptClicksIfDisabled = flag;
		horizontalScroller.slider.clampedDragger.acceptClicksIfDisabled = flag2;
		horizontalScroller.lowButton.acceptClicksIfDisabled = flag2;
		horizontalScroller.highButton.acceptClicksIfDisabled = flag2;
		bool flag3 = displayHorizontal && m_HorizontalScrollerVisibility != ScrollerVisibility.Hidden;
		bool flag4 = displayVertical && m_VerticalScrollerVisibility != ScrollerVisibility.Hidden;
		DisplayStyle displayStyle = ((!flag3) ? DisplayStyle.None : DisplayStyle.Flex);
		DisplayStyle displayStyle2 = ((!flag4) ? DisplayStyle.None : DisplayStyle.Flex);
		if (displayStyle != horizontalScroller.style.display)
		{
			horizontalScroller.style.display = displayStyle;
		}
		if (displayStyle2 != verticalScroller.style.display)
		{
			verticalScroller.style.display = displayStyle2;
		}
		verticalScroller.lowValue = 0f;
		verticalScroller.highValue = (float.IsNaN(scrollableHeight) ? verticalScroller.highValue : scrollableHeight);
		horizontalScroller.lowValue = 0f;
		horizontalScroller.highValue = (float.IsNaN(scrollableWidth) ? horizontalScroller.highValue : scrollableWidth);
	}

	private void OnScrollersGeometryChanged(GeometryChangedEvent evt)
	{
		if (!(evt.oldRect.size == evt.newRect.size))
		{
			if (needsHorizontal && m_HorizontalScrollerVisibility != ScrollerVisibility.Hidden)
			{
				horizontalScroller.style.marginRight = verticalScroller.layout.width;
			}
			AdjustScrollers();
		}
	}

	private void OnScrollWheel(WheelEvent evt)
	{
		if (Application.platform == RuntimePlatform.OSXEditor || Application.platform == RuntimePlatform.OSXPlayer)
		{
			if (evt.commandKey)
			{
				return;
			}
		}
		else if (evt.ctrlKey)
		{
			return;
		}
		bool flag = false;
		bool flag2 = mode != ScrollViewMode.Horizontal && scrollableHeight > 0f;
		bool flag3 = mode != ScrollViewMode.Vertical && scrollableWidth > 0f;
		float num = ((flag3 && !flag2) ? evt.delta.y : evt.delta.x);
		if ((flag3 || flag2) && !m_MouseWheelScrollSizeIsInline)
		{
			if (!m_SingleLineHeightDirtyFlag)
			{
				VisualElement visualElement = base.parent;
				if (visualElement == null || !visualElement.isRootVisualContainer || base.parent == m_AttachedRootVisualContainer)
				{
					goto IL_00f0;
				}
			}
			ReadSingleLineHeight();
		}
		goto IL_00f0;
		IL_00f0:
		float num2 = (m_MouseWheelScrollSizeIsInline ? mouseWheelScrollSize : m_SingleLineHeight);
		if (flag2)
		{
			float value = verticalScroller.value;
			verticalScroller.value += evt.delta.y * ((verticalScroller.lowValue < verticalScroller.highValue) ? 1f : (-1f)) * num2;
			if (nestedInteractionKind == NestedInteractionKind.StopScrolling || !Mathf.Approximately(verticalScroller.value, value))
			{
				evt.StopPropagation();
				flag = true;
			}
		}
		if (flag3)
		{
			float value2 = horizontalScroller.value;
			horizontalScroller.value += num * ((horizontalScroller.lowValue < horizontalScroller.highValue) ? 1f : (-1f)) * num2;
			if (nestedInteractionKind == NestedInteractionKind.StopScrolling || !Mathf.Approximately(horizontalScroller.value, value2))
			{
				evt.StopPropagation();
				flag = true;
			}
		}
		if (flag)
		{
			UpdateElasticBehaviour();
			UpdateContentViewTransform();
		}
	}

	private void OnRootCustomStyleResolved(CustomStyleResolvedEvent evt)
	{
		MarkSingleLineHeightDirty();
	}

	private void MarkSingleLineHeightDirty()
	{
		m_SingleLineHeightDirtyFlag = true;
	}

	private void OnRootPointerUp(PointerUpEvent evt)
	{
		m_TouchPointerMoveAllowed = false;
	}

	private void ReadSingleLineHeight()
	{
		if (base.computedStyle.customProperties != null && base.computedStyle.customProperties.TryGetValue("--unity-metrics-single_line-height", out var value))
		{
			m_SingleLineHeightDirtyFlag = false;
			if (value.sheet.TryReadDimension(value.handle, out var value2))
			{
				m_SingleLineHeight = value2.value;
				return;
			}
		}
		VisualElement firstAncestorWhere = GetFirstAncestorWhere((VisualElement x) => x.isRootVisualContainer);
		if (firstAncestorWhere != m_AttachedRootVisualContainer)
		{
			m_AttachedRootVisualContainer.UnregisterCallback<CustomStyleResolvedEvent>(OnRootCustomStyleResolved);
			m_AttachedRootVisualContainer = firstAncestorWhere;
			m_AttachedRootVisualContainer.RegisterCallback<CustomStyleResolvedEvent>(OnRootCustomStyleResolved);
		}
		VisualElement attachedRootVisualContainer = m_AttachedRootVisualContainer;
		if (attachedRootVisualContainer != null && attachedRootVisualContainer.computedStyle.customProperties != null && m_AttachedRootVisualContainer.computedStyle.customProperties.TryGetValue("--unity-metrics-single_line-height", out var value3))
		{
			m_SingleLineHeightDirtyFlag = false;
			if (value3.sheet.TryReadDimension(value3.handle, out var value4))
			{
				m_SingleLineHeight = value4.value;
				return;
			}
		}
		m_SingleLineHeight = UIElementsUtility.singleLineHeight;
		m_SingleLineHeightDirtyFlag = false;
	}

	private void UpdateElasticBehaviour()
	{
		if (touchScrollBehavior == TouchScrollBehavior.Elastic)
		{
			m_LowBounds = new Vector2(Mathf.Min(horizontalScroller.lowValue, horizontalScroller.highValue), Mathf.Min(verticalScroller.lowValue, verticalScroller.highValue));
			m_HighBounds = new Vector2(Mathf.Max(horizontalScroller.lowValue, horizontalScroller.highValue), Mathf.Max(verticalScroller.lowValue, verticalScroller.highValue));
			ExecuteElasticSpringAnimation();
		}
	}

	internal void SetScrollOffsetWithoutNotify(Vector2 value)
	{
		horizontalScroller.slider.SetValueWithoutNotify(value.x);
		verticalScroller.slider.SetValueWithoutNotify(value.y);
		m_ScrollOffset = new Vector2(horizontalScroller.value, verticalScroller.value);
		SaveViewData();
	}

	internal override void OnViewDataReady()
	{
		base.OnViewDataReady();
		if (!string.IsNullOrEmpty(verticalScroller.viewDataKey) || !string.IsNullOrEmpty(verticalScroller.slider.viewDataKey) || !string.IsNullOrEmpty(horizontalScroller.viewDataKey) || !string.IsNullOrEmpty(horizontalScroller.slider.viewDataKey))
		{
			string fullHierarchicalViewDataKey = GetFullHierarchicalViewDataKey();
			OverwriteFromViewData(this, fullHierarchicalViewDataKey);
			UpdateContentViewTransform();
		}
	}
}
