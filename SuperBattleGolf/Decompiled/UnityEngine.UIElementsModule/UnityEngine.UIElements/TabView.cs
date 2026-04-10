using System;
using System.Collections.Generic;
using System.Diagnostics;
using Unity.Properties;
using UnityEngine.Internal;

namespace UnityEngine.UIElements;

[UxmlElement(null, new Type[] { typeof(Tab) })]
public class TabView : VisualElement
{
	[Serializable]
	[ExcludeFromDocs]
	public new class UxmlSerializedData : VisualElement.UxmlSerializedData
	{
		[SerializeField]
		private bool reorderable;

		[SerializeField]
		[UxmlIgnore]
		[HideInInspector]
		private UxmlAttributeFlags reorderable_UxmlAttributeFlags;

		[Conditional("UNITY_EDITOR")]
		public new static void Register()
		{
			UxmlDescriptionCache.RegisterType(typeof(UxmlSerializedData), new UxmlAttributeNames[1]
			{
				new UxmlAttributeNames("reorderable", "reorderable", null)
			});
		}

		public override object CreateInstance()
		{
			return new TabView();
		}

		public override void Deserialize(object obj)
		{
			base.Deserialize(obj);
			if (UnityEngine.UIElements.UxmlSerializedData.ShouldWriteAttributeValue(reorderable_UxmlAttributeFlags))
			{
				TabView tabView = (TabView)obj;
				tabView.reorderable = reorderable;
			}
		}
	}

	[Obsolete("UxmlFactory is deprecated and will be removed. Use UxmlElementAttribute instead.", false)]
	public new class UxmlFactory : UxmlFactory<TabView, UxmlTraits>
	{
	}

	[Obsolete("UxmlTraits is deprecated and will be removed. Use UxmlElementAttribute instead.", false)]
	public new class UxmlTraits : VisualElement.UxmlTraits
	{
		private readonly UxmlBoolAttributeDescription m_Reorderable = new UxmlBoolAttributeDescription
		{
			name = "reorderable",
			defaultValue = false
		};

		public override void Init(VisualElement ve, IUxmlAttributes bag, CreationContext cc)
		{
			base.Init(ve, bag, cc);
			TabView tabView = (TabView)ve;
			tabView.reorderable = m_Reorderable.GetValueFromBag(bag, cc);
		}
	}

	[Serializable]
	private class ViewState : ISerializationCallbackReceiver
	{
		private bool m_HasPersistedData;

		[SerializeField]
		private List<string> m_TabOrder = new List<string>();

		[SerializeField]
		private string m_ActiveTabKey;

		internal void Save(TabView tabView)
		{
			m_HasPersistedData = true;
			if (tabView.m_ActiveTab != null)
			{
				m_ActiveTabKey = tabView.m_ActiveTab.viewDataKey;
			}
			m_TabOrder.Clear();
			if (!tabView.reorderable)
			{
				return;
			}
			foreach (Tab tab in tabView.tabs)
			{
				m_TabOrder.Add(tab.viewDataKey);
			}
		}

		internal void Apply(TabView tabView)
		{
			if (!m_HasPersistedData)
			{
				return;
			}
			int num = Math.Min(m_TabOrder.Count, tabView.tabs.Count);
			int num2 = 0;
			Tab tab = tabView.FindTabByKey(m_ActiveTabKey);
			if (tab != null)
			{
				tabView.activeTab = tab;
			}
			if (!tabView.reorderable)
			{
				return;
			}
			for (int i = 0; i < m_TabOrder.Count; i++)
			{
				if (num2 >= num)
				{
					break;
				}
				string key = m_TabOrder[i];
				Tab tab2 = tabView.FindTabByKey(key);
				if (tab2 != null)
				{
					int num3 = tabView.tabs.IndexOf(tab2);
					tabView.ReorderTab(num3, num2++);
				}
			}
		}

		public void OnBeforeSerialize()
		{
			m_HasPersistedData = true;
		}

		public void OnAfterDeserialize()
		{
			m_HasPersistedData = true;
		}
	}

	private class TabViewContentContainer : VisualElement
	{
		internal override void OnChildAdded(VisualElement ve)
		{
			((TabView)base.parent).OnElementAdded(ve);
		}

		internal override void OnChildRemoved(VisualElement ve)
		{
			((TabView)base.parent).OnElementRemoved(ve);
		}
	}

	internal static readonly BindingId reorderableProperty = "reorderable";

	public static readonly string ussClassName = "unity-tab-view";

	public static readonly string headerContainerClassName = ussClassName + "__header-container";

	public static readonly string contentContainerUssClassName = ussClassName + "__content-container";

	public static readonly string reorderableUssClassName = ussClassName + "__reorderable";

	public static readonly string verticalUssClassName = ussClassName + "__vertical";

	public static readonly string viewportUssClassName = ussClassName + "__content-viewport";

	public static readonly string nextButtonUssClassName = ussClassName + "__next-button";

	public static readonly string previousButtonUssClassName = ussClassName + "__previous-button";

	private VisualElement m_HeaderContainer;

	private VisualElement m_ContentContainer;

	private List<Tab> m_Tabs = new List<Tab>();

	private List<VisualElement> m_TabHeaders = new List<VisualElement>();

	private Tab m_ActiveTab;

	private ViewState m_ViewState;

	private bool m_ApplyingViewState;

	private bool m_Reordering;

	private const float k_SizeThreshold = 0.001f;

	private const float k_PixelThreshold = 50f;

	private bool m_Reorderable;

	public override VisualElement contentContainer => m_ContentContainer;

	public VisualElement contentViewport { get; }

	internal VisualElement header => m_HeaderContainer;

	internal List<Tab> tabs => m_Tabs;

	internal List<VisualElement> tabHeaders => m_TabHeaders;

	internal RepeatButton nextButton { get; private set; }

	internal RepeatButton previousButton { get; private set; }

	internal float scrollableWidth => Mathf.Max(0f, m_HeaderContainer.boundingBox.width - contentViewport.layout.width);

	internal bool needsButtons => scrollableWidth > 0.001f;

	public Tab activeTab
	{
		get
		{
			return m_ActiveTab;
		}
		set
		{
			if (value == null && m_Tabs.Count > 0)
			{
				throw new NullReferenceException("Active tab cannot be null when there are available tabs.");
			}
			if (m_Tabs.IndexOf(value) == -1)
			{
				throw new Exception("The tab to be set as active does not exist in this TabView.");
			}
			if (value != m_ActiveTab)
			{
				Tab arg = m_ActiveTab;
				m_ActiveTab?.SetInactive();
				m_ActiveTab = value;
				m_ActiveTab?.SetActive();
				if (!m_ApplyingViewState)
				{
					SaveViewState();
				}
				this.activeTabChanged?.Invoke(arg, value);
			}
		}
	}

	public int selectedTabIndex
	{
		get
		{
			if (activeTab == null || m_Tabs.Count == 0)
			{
				return -1;
			}
			return m_Tabs.IndexOf(activeTab);
		}
		set
		{
			if (value >= 0 && m_Tabs.Count > value)
			{
				activeTab = m_Tabs[value];
			}
		}
	}

	[CreateProperty]
	public bool reorderable
	{
		get
		{
			return m_Reorderable;
		}
		set
		{
			if (m_Reorderable == value)
			{
				return;
			}
			m_Reorderable = value;
			EnableInClassList(reorderableUssClassName, value);
			foreach (Tab tab in m_Tabs)
			{
				tab.EnableTabDragHandles(value);
			}
			NotifyPropertyChanged(in reorderableProperty);
		}
	}

	public event Action<Tab, Tab> activeTabChanged;

	public event Action<int, int> tabReordered;

	public event Action<Tab, int> tabClosed;

	public TabView()
	{
		AddToClassList(ussClassName);
		contentViewport = new VisualElement();
		contentViewport.AddToClassList(viewportUssClassName);
		contentViewport.RegisterCallback<GeometryChangedEvent>(OnGeometryChanged);
		contentViewport.pickingMode = PickingMode.Ignore;
		base.hierarchy.Add(contentViewport);
		m_HeaderContainer = new VisualElement
		{
			name = headerContainerClassName,
			classList = { headerContainerClassName }
		};
		header.RegisterCallback<GeometryChangedEvent>(OnGeometryChanged);
		contentViewport.Add(m_HeaderContainer);
		m_ContentContainer = new TabViewContentContainer
		{
			name = contentContainerUssClassName,
			classList = { contentContainerUssClassName }
		};
		base.hierarchy.Add(m_ContentContainer);
		nextButton = new RepeatButton(OnNextClicked, 250L, 30L)
		{
			classList = { nextButtonUssClassName }
		};
		previousButton = new RepeatButton(OnPreviousClicked, 250L, 30L)
		{
			classList = { previousButtonUssClassName }
		};
		contentViewport.Add(nextButton);
		contentViewport.Add(previousButton);
		RegisterCallback<DetachFromPanelEvent>(OnDetachFromPanel);
	}

	internal override void OnViewDataReady()
	{
		try
		{
			m_ApplyingViewState = true;
			base.OnViewDataReady();
			string fullHierarchicalViewDataKey = GetFullHierarchicalViewDataKey();
			m_ViewState = GetOrCreateViewData<ViewState>(m_ViewState, fullHierarchicalViewDataKey);
			m_ViewState.Apply(this);
		}
		finally
		{
			m_ApplyingViewState = false;
		}
	}

	private void OnDetachFromPanel(DetachFromPanelEvent evt)
	{
		if (evt.originPanel != null)
		{
			header.UnregisterCallback<GeometryChangedEvent>(OnGeometryChanged);
			contentViewport.UnregisterCallback<GeometryChangedEvent>(OnGeometryChanged);
		}
	}

	private void OnGeometryChanged(GeometryChangedEvent evt)
	{
		if (!(evt.oldRect.size == evt.newRect.size))
		{
			Vector3 translate = m_HeaderContainer.resolvedStyle.translate;
			Vector3 vector = translate;
			if ((!needsButtons || nextButton.resolvedStyle.display == DisplayStyle.None) && translate.x < 0f)
			{
				vector.x = Math.Min(0f, translate.x + (contentViewport.worldBound.xMax - m_HeaderContainer.worldBound.xMax));
			}
			if (vector != translate)
			{
				m_HeaderContainer.style.translate = vector;
			}
			UpdateButtons(vector);
		}
	}

	private void OnNextClicked()
	{
		Vector3 translate = m_HeaderContainer.resolvedStyle.translate;
		VisualElement visualElement = m_TabHeaders.Find((VisualElement tab) => (!(tab.worldBound.xMax - contentViewport.worldBound.xMax < 50f) || m_TabHeaders.IndexOf(tab) == m_TabHeaders.Count - 1) && tab.worldBound.xMax >= contentViewport.worldBound.xMax);
		if (visualElement != null)
		{
			translate.x = 0f - (visualElement.layout.xMax + nextButton.layout.width - contentViewport.layout.xMax);
			translate.x = Mathf.Max(translate.x, 0f - scrollableWidth);
			m_HeaderContainer.style.translate = translate;
		}
		UpdateButtons(translate);
	}

	private void OnPreviousClicked()
	{
		Vector3 translate = m_HeaderContainer.resolvedStyle.translate;
		VisualElement visualElement = m_TabHeaders.FindLast((VisualElement tab) => (!(contentViewport.worldBound.xMin - tab.worldBound.xMin < 50f) || m_TabHeaders.IndexOf(tab) == 0) && tab.worldBound.xMin <= contentViewport.worldBound.xMin);
		if (visualElement != null)
		{
			translate.x = contentViewport.layout.xMin + previousButton.layout.width - visualElement.layout.xMin;
			translate.x = Mathf.Min(translate.x, 0f);
			m_HeaderContainer.style.translate = translate;
		}
		UpdateButtons(translate);
	}

	internal void UpdateButtons(Vector3 contentTransform)
	{
		nextButton.style.display = ((!(contentTransform.x > 0f - scrollableWidth)) ? DisplayStyle.None : DisplayStyle.Flex);
		previousButton.style.display = ((!(contentTransform.x < 0f)) ? DisplayStyle.None : DisplayStyle.Flex);
	}

	private void SaveViewState()
	{
		if (!m_ApplyingViewState)
		{
			m_ViewState?.Save(this);
			SaveViewData();
		}
	}

	private void UpdateIndexes()
	{
		for (int i = 0; i < m_Tabs.Count; i++)
		{
			m_Tabs[i].index = i;
		}
	}

	private void OnElementAdded(VisualElement ve)
	{
		if (ve is Tab tab && !m_Reordering)
		{
			VisualElement tabHeader = tab.tabHeader;
			if (tabHeader != null)
			{
				int index = m_ContentContainer.IndexOf(tab);
				m_HeaderContainer.Insert(index, tabHeader);
				m_TabHeaders.Insert(index, tabHeader);
				m_Tabs.Insert(index, tab);
				tab.EnableTabDragHandles(m_Reorderable);
				tab.closed += OnTabClosed;
			}
			tab.selected += OnTabSelected;
			UpdateIndexes();
			if (activeTab == null)
			{
				activeTab = tab;
			}
		}
	}

	private void OnElementRemoved(VisualElement ve)
	{
		if (ve is Tab tab && !m_Reordering)
		{
			VisualElement tabHeader = tab.tabHeader;
			m_HeaderContainer.Remove(tabHeader);
			m_TabHeaders.Remove(tabHeader);
			m_Tabs.Remove(tab);
			tab.EnableTabDragHandles(enable: false);
			tab.hierarchy.Insert(0, tabHeader);
			tab.SetInactive();
			UpdateIndexes();
			if (activeTab == tab && m_Tabs.Count > 0)
			{
				activeTab = m_Tabs[0];
			}
			else if (m_Tabs.Count == 0)
			{
				m_ActiveTab = null;
			}
		}
	}

	private void OnTabSelected(Tab tab)
	{
		activeTab = tab;
	}

	private void OnTabClosed(Tab tab)
	{
		this.tabClosed?.Invoke(tab, tab.index);
	}

	public void ReorderTab(int from, int to)
	{
		VisualElement visualElement = m_TabHeaders[from];
		Tab tab = m_Tabs[from];
		if (visualElement.visible && reorderable && from != to)
		{
			m_Reordering = true;
			m_TabHeaders.RemoveAt(from);
			m_TabHeaders.Insert(to, visualElement);
			m_Tabs.RemoveAt(from);
			m_Tabs.Insert(to, tab);
			m_HeaderContainer.Insert(to, visualElement);
			Insert(to, tab);
			m_Reordering = false;
			UpdateIndexes();
			this.tabReordered?.Invoke(from, to);
			if (!m_ApplyingViewState)
			{
				SaveViewState();
			}
		}
	}

	public Tab GetTab(int index)
	{
		if (index < 0 || index >= m_Tabs.Count)
		{
			return null;
		}
		return m_Tabs[index];
	}

	public VisualElement GetTabHeader(int index)
	{
		if (index < 0 || index >= m_Tabs.Count)
		{
			return null;
		}
		return m_TabHeaders[index];
	}

	internal Tab FindTabByKey(string key)
	{
		return m_Tabs.Find((Tab tab) => tab.viewDataKey == key);
	}
}
