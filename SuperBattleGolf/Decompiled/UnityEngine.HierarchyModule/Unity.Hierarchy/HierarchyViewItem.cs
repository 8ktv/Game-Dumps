using System;
using System.Text;
using UnityEngine.Bindings;
using UnityEngine.UIElements;

namespace Unity.Hierarchy;

[VisibleToOtherModules(new string[] { "UnityEditor.HierarchyModule", "UnityEditor.UIToolkitAuthoringModule" })]
internal sealed class HierarchyViewItem : VisualElement
{
	internal delegate void ExpandedStateChangedEventHandler(in HierarchyNode node, bool isExpanded, bool recursive);

	private const string k_UnityListViewItem = "unity-list-view__item";

	private const string k_UnityTreeViewItem = "unity-tree-view__item";

	private const string k_UnityTreeViewItemToggle = "unity-tree-view__item-toggle";

	private const string k_UnityToggleCheckmark = "unity-toggle__checkmark";

	private const string k_HierarchyItemContainer = "hierarchy-item__container";

	private const string k_HierarchyItemOverrideBarContainer = "hierarchy-item__override-bar-container";

	private const string k_HierarchyItemIcon = "hierarchy-item__icon";

	private const string k_HierarchyItemIconCut = "hierarchy-item__icon--cut";

	private const string k_HierarchyItemOverlayIcon = "hierarchy-item__overlay-icon";

	private const string k_HierarchyItemName = "hierarchy-item__name";

	private const string k_HierarchyItemLeftContainer = "hierarchy-item__left-container";

	private const string k_HierarchyItemLeftCustomSection = "hierarchy-item__left-custom-section";

	private const string k_HierarchyItemRightContainer = "hierarchy-item__right-container";

	private const string k_HierarchyItemRightArrowButton = "hierarchy-item__right-arrow-button";

	private const string k_HierarchyItemToggleHidden = "hierarchy-item__toggle--hidden";

	internal const int k_IndentWidth = 14;

	private HierarchyNode m_Node;

	private HierarchyNodeTypeHandler m_Handler;

	private HierarchyView m_View;

	private readonly Toggle m_Toggle;

	private readonly VisualElement m_OverrideBarContainer;

	private readonly VisualElement m_Icon;

	private readonly VisualElement m_OverlayIcon;

	private readonly HierarchyViewItemName m_Name;

	private readonly Lazy<Button> m_NavigateIntoButton;

	private readonly VisualElement m_LeftCustomContainer;

	private readonly VisualElement m_RightCustomContainer;

	private readonly VisualElement m_LeftContainer;

	internal VisualElement LeftContainer => m_LeftContainer;

	public HierarchyNodeType NodeType => m_Handler?.GetNodeType() ?? HierarchyNodeType.Null;

	public ref readonly HierarchyNode Node => ref m_Node;

	public Label Name => m_Name.Label;

	public VisualElement Icon => m_Icon;

	public VisualElement OverlayIcon => m_OverlayIcon;

	public VisualElement LeftCustomContainer => m_LeftCustomContainer;

	public VisualElement RightCustomContainer => m_RightCustomContainer;

	public Button NavigateIntoButton => m_NavigateIntoButton.Value;

	public VisualElement OverrideBarContainer => m_OverrideBarContainer;

	public Toggle Toggle => m_Toggle;

	public VisualElement RowContainer
	{
		get
		{
			VisualElement visualElement = base.parent;
			while (visualElement != null && !visualElement.ClassListContains("unity-multi-column-view__row-container"))
			{
				visualElement = visualElement.parent;
			}
			return visualElement;
		}
	}

	public HierarchyNodeTypeHandler Handler => m_Handler;

	public HierarchyView View => m_View;

	internal bool Bound => m_Node != HierarchyNode.Null || m_View != null;

	internal event ExpandedStateChangedEventHandler ExpandedStateChanged;

	internal HierarchyViewItem()
	{
		base.name = "unity-tree-view__item";
		base.style.flexDirection = FlexDirection.Row;
		VisualElement visualElement = new VisualElement();
		visualElement.AddToClassList("hierarchy-item__container");
		base.hierarchy.Add(visualElement);
		m_LeftContainer = new VisualElement();
		m_LeftContainer.AddToClassList("hierarchy-item__left-container");
		m_OverrideBarContainer = new VisualElement();
		m_OverrideBarContainer.AddToClassList("hierarchy-item__override-bar-container");
		m_Toggle = new Toggle();
		m_Toggle.AddToClassList("unity-tree-view__item-toggle");
		m_Toggle.AddToClassList(Foldout.toggleUssClassName);
		m_Toggle.Q(null, "unity-toggle__checkmark").style.marginTop = 0f;
		m_Toggle.focusable = false;
		m_Icon = new VisualElement();
		m_Icon.AddToClassList("hierarchy-item__icon");
		m_OverlayIcon = new VisualElement();
		m_OverlayIcon.AddToClassList("hierarchy-item__overlay-icon");
		m_Name = new HierarchyViewItemName();
		m_Name.AddToClassList("hierarchy-item__name");
		m_LeftCustomContainer = new VisualElement();
		m_LeftCustomContainer.AddToClassList("hierarchy-item__left-custom-section");
		m_LeftContainer.Add(m_OverrideBarContainer);
		m_LeftContainer.Add(m_Toggle);
		m_LeftContainer.Add(m_Icon);
		m_LeftContainer.Add(m_OverlayIcon);
		m_LeftContainer.Add(m_Name);
		m_LeftContainer.Add(m_LeftCustomContainer);
		m_RightCustomContainer = new VisualElement();
		m_RightCustomContainer.AddToClassList("hierarchy-item__right-container");
		m_NavigateIntoButton = new Lazy<Button>(delegate
		{
			Button button = new Button();
			button.AddToClassList("hierarchy-item__right-arrow-button");
			button.RemoveFromClassList(Button.ussClassName);
			button.style.display = DisplayStyle.None;
			m_RightCustomContainer.Add(button);
			return button;
		});
		visualElement.Add(m_OverrideBarContainer);
		visualElement.Add(m_LeftContainer);
		visualElement.Add(m_RightCustomContainer);
		AddToClassList("unity-tree-view__item");
		AddToClassList("unity-list-view__item");
	}

	internal void Bind(in HierarchyNode node, HierarchyView view)
	{
		if (Bound)
		{
			throw new InvalidOperationException("Cannot bind a hierarchy view item that is already bound.");
		}
		m_Node = node;
		m_Handler = view.Source.GetNodeTypeHandler(in node);
		m_View = view;
		HierarchyViewModel viewModel = m_View.ViewModel;
		HierarchyNode lhs = viewModel.GetRoot();
		int depth = viewModel.GetDepth(in m_Node);
		int num = ((lhs == m_View.Source.Root) ? depth : (depth - viewModel.GetDepth(in lhs) - 1));
		bool flag = !m_View.Filtering;
		int num2 = (flag ? (num * 14) : 0);
		Translate value = m_LeftContainer.style.translate.value;
		m_LeftContainer.style.translate = new Translate(m_LeftContainer.CeilToPanelPixelSize(num2), value.y, value.z);
		bool flag2 = flag && viewModel.GetChildrenCount(in m_Node) > 0;
		m_Toggle.EnableInClassList("hierarchy-item__toggle--hidden", !flag2);
		bool flag3 = viewModel.HasAllFlags(in m_Node, HierarchyNodeFlags.Expanded);
		m_Toggle.SetValueWithoutNotify(flag2 && flag3);
		Icon.EnableInClassList("hierarchy-item__icon--cut", viewModel.HasAllFlags(in m_Node, HierarchyNodeFlags.Cut));
		if (m_Handler is IHierarchyEditorNodeTypeHandler hierarchyEditorNodeTypeHandler)
		{
			m_Name.Text = hierarchyEditorNodeTypeHandler.GetDisplayName(m_View, in m_Node);
		}
		else
		{
			m_Name.Text = m_View.Source.GetName(in m_Node);
		}
		m_View.InvokeBindViewItem(this);
		m_Name.OnBeginRename += OnBeginRename;
		m_Name.OnEndRename += OnEndRename;
	}

	internal void Unbind()
	{
		if (Bound)
		{
			if (RowContainer != null && RowContainer.ClassListContains("hierarchy - item__ping-base"))
			{
				RowContainer.SendEvent(new TransitionEndEvent
				{
					target = RowContainer
				}, DispatchMode.Immediate);
				RowContainer.SendEvent(new TransitionEndEvent
				{
					target = RowContainer
				}, DispatchMode.Immediate);
			}
			m_Node = HierarchyNode.Null;
			m_View.InvokeUnbindViewItem(this);
			m_Name.OnBeginRename -= OnBeginRename;
			m_Name.OnEndRename -= OnEndRename;
			m_Handler = null;
			m_View = null;
		}
	}

	[EventInterest(new Type[] { typeof(TooltipEvent) })]
	[EventInterest(new Type[] { typeof(ClickEvent) })]
	protected override void HandleEventBubbleUp(EventBase evt)
	{
		if (evt is TooltipEvent tooltipEvent)
		{
			bool filtering = m_View.Filtering;
			StringBuilder stringBuilder = new StringBuilder();
			m_View.InvokeGetTooltip(this, filtering, stringBuilder);
			if (stringBuilder.Length != 0)
			{
				tooltipEvent.rect = m_Name.worldBound;
				tooltipEvent.tooltip = stringBuilder.ToString();
			}
		}
		else if (evt is ClickEvent clickEvent && m_Toggle.visible && m_Toggle.worldBound.Contains(clickEvent.position))
		{
			bool isExpanded = !m_View.ViewModel.HasAllFlags(in m_Node, HierarchyNodeFlags.Expanded);
			this.ExpandedStateChanged?.Invoke(in m_Node, isExpanded, clickEvent.altKey);
			evt.StopPropagation();
		}
	}

	[EventInterest(new Type[] { typeof(PointerDownEvent) })]
	protected override void HandleEventTrickleDown(EventBase evt)
	{
		if (evt is PointerDownEvent pointerDownEvent)
		{
			HierarchyView view = m_View;
			if (view != null && view.m_IsRenamingItem && m_Toggle.worldBound.Contains(pointerDownEvent.position))
			{
				pointerDownEvent.StopImmediatePropagation();
			}
		}
	}

	public void BeginRename()
	{
		if (!(m_Node == HierarchyNode.Null) && (!(m_Handler is IHierarchyEditorNodeTypeHandler hierarchyEditorNodeTypeHandler) || hierarchyEditorNodeTypeHandler.CanSetName(m_View, in m_Node)))
		{
			m_Name.BeginRename();
		}
	}

	private void OnBeginRename()
	{
		m_View.SetRenamingItem(this);
	}

	private void OnEndRename(string text, bool canceled)
	{
		m_View.SetRenamingItem(null);
		if (!canceled && !(m_Node == HierarchyNode.Null) && !string.IsNullOrEmpty(text))
		{
			if (m_Handler is IHierarchyEditorNodeTypeHandler hierarchyEditorNodeTypeHandler)
			{
				hierarchyEditorNodeTypeHandler.OnSetName(m_View, in m_Node, text);
			}
			else
			{
				m_View.Source.SetName(in m_Node, text);
			}
		}
	}
}
