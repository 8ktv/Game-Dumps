namespace UnityEngine.UIElements.HierarchyV2;

internal class ScrollContainer : VisualElement
{
	private const float k_MouseScrollFactor = 18f;

	private VisualElement m_Container;

	private VisualElement m_Viewport;

	private CollectionViewScroller m_VerticalScroller;

	private CollectionViewScroller m_HorizontalScroller;

	private Vector2 m_ContainerOffset;

	public static readonly string ussClassName = "unity-collection-view-scroll-view";

	public static readonly string containerUssClassName = ussClassName + "__content-container";

	public static readonly string verticalScrollerUssClassName = ussClassName + "__vertical-scroller";

	public static readonly string horizontalScrollerUssClassName = ussClassName + "__horizontal-scroller";

	public static readonly string contentAndHorizontalScrollUssClassName = ussClassName + "__content-and-horizontal-scroll-container";

	public static readonly string contentViewportUssClassName = ussClassName + "__content-viewport";

	public override VisualElement contentContainer => m_Container;

	public VisualElement viewport => m_Viewport;

	public CollectionViewScroller verticalScroller
	{
		get
		{
			return m_VerticalScroller;
		}
		private set
		{
			m_VerticalScroller = value;
		}
	}

	public CollectionViewScroller horizontalScroller
	{
		get
		{
			return m_HorizontalScroller;
		}
		private set
		{
			m_HorizontalScroller = value;
		}
	}

	public Vector2 containerOffset
	{
		get
		{
			return m_ContainerOffset;
		}
		set
		{
			if (!Mathf.Approximately(m_ContainerOffset.x, value.x) || !Mathf.Approximately(m_ContainerOffset.y, value.y))
			{
				m_ContainerOffset = value;
				m_Container.style.translate = new Vector3(0f - m_ContainerOffset.x, 0f - m_ContainerOffset.y, 0f);
			}
		}
	}

	public ScrollContainer()
	{
		AddToClassList(ussClassName);
		m_Viewport = new VisualElement();
		m_Viewport.AddToClassList(contentViewportUssClassName);
		m_Container = new VisualElement();
		m_Container.AddToClassList(containerUssClassName);
		m_Container.RegisterCallback<WheelEvent>(OnScrollWheel);
		verticalScroller = new CollectionViewScroller();
		verticalScroller.AddToClassList(verticalScrollerUssClassName);
		horizontalScroller = new CollectionViewScroller
		{
			direction = SliderDirection.Horizontal
		};
		horizontalScroller.AddToClassList(horizontalScrollerUssClassName);
		horizontalScroller.RegisterValueChangedCallback(delegate(ChangeEvent<double> evt)
		{
			Vector2 vector = containerOffset;
			vector.x = (float)evt.newValue;
			containerOffset = vector;
		});
		m_Viewport.Add(m_Container);
		VisualElement visualElement = new VisualElement();
		visualElement.AddToClassList(contentAndHorizontalScrollUssClassName);
		visualElement.Add(m_Viewport);
		visualElement.Add(horizontalScroller);
		base.hierarchy.Add(visualElement);
		base.hierarchy.Add(verticalScroller);
	}

	private void OnScrollWheel(WheelEvent evt)
	{
		verticalScroller.value += evt.delta.y * ((verticalScroller.lowValue < verticalScroller.highValue) ? 1f : (-1f)) * 18f;
	}
}
