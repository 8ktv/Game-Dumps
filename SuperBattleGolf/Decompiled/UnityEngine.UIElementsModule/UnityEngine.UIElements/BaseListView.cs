using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using Unity.Properties;
using UnityEngine.Bindings;
using UnityEngine.Internal;

namespace UnityEngine.UIElements;

public abstract class BaseListView : BaseVerticalCollectionView
{
	[Serializable]
	[ExcludeFromDocs]
	public new abstract class UxmlSerializedData : BaseVerticalCollectionView.UxmlSerializedData
	{
		[SerializeField]
		private string headerTitle;

		[SerializeField]
		private ListViewReorderMode reorderMode;

		[SerializeField]
		private BindingSourceSelectionMode bindingSourceSelectionMode;

		[SerializeField]
		private bool showFoldoutHeader;

		[SerializeField]
		private bool showAddRemoveFooter;

		[SerializeField]
		private bool allowAdd;

		[SerializeField]
		private bool allowRemove;

		[SerializeField]
		private bool showBoundCollectionSize;

		[HideInInspector]
		[SerializeField]
		[UxmlIgnore]
		private UxmlAttributeFlags showFoldoutHeader_UxmlAttributeFlags;

		[SerializeField]
		[UxmlIgnore]
		[HideInInspector]
		private UxmlAttributeFlags headerTitle_UxmlAttributeFlags;

		[HideInInspector]
		[UxmlIgnore]
		[SerializeField]
		private UxmlAttributeFlags showAddRemoveFooter_UxmlAttributeFlags;

		[SerializeField]
		[HideInInspector]
		[UxmlIgnore]
		private UxmlAttributeFlags allowAdd_UxmlAttributeFlags;

		[HideInInspector]
		[SerializeField]
		[UxmlIgnore]
		private UxmlAttributeFlags allowRemove_UxmlAttributeFlags;

		[HideInInspector]
		[UxmlIgnore]
		[SerializeField]
		private UxmlAttributeFlags reorderMode_UxmlAttributeFlags;

		[HideInInspector]
		[UxmlIgnore]
		[SerializeField]
		private UxmlAttributeFlags showBoundCollectionSize_UxmlAttributeFlags;

		[UxmlIgnore]
		[HideInInspector]
		[SerializeField]
		private UxmlAttributeFlags bindingSourceSelectionMode_UxmlAttributeFlags;

		[Conditional("UNITY_EDITOR")]
		public new static void Register()
		{
			UxmlDescriptionCache.RegisterType(typeof(UxmlSerializedData), new UxmlAttributeNames[8]
			{
				new UxmlAttributeNames("showFoldoutHeader", "show-foldout-header", null),
				new UxmlAttributeNames("headerTitle", "header-title", null),
				new UxmlAttributeNames("showAddRemoveFooter", "show-add-remove-footer", null),
				new UxmlAttributeNames("allowAdd", "allow-add", null),
				new UxmlAttributeNames("allowRemove", "allow-remove", null),
				new UxmlAttributeNames("reorderMode", "reorder-mode", null),
				new UxmlAttributeNames("showBoundCollectionSize", "show-bound-collection-size", null),
				new UxmlAttributeNames("bindingSourceSelectionMode", "binding-source-selection-mode", null)
			});
		}

		public override void Deserialize(object obj)
		{
			base.Deserialize(obj);
			BaseListView baseListView = (BaseListView)obj;
			if (UnityEngine.UIElements.UxmlSerializedData.ShouldWriteAttributeValue(showFoldoutHeader_UxmlAttributeFlags))
			{
				baseListView.showFoldoutHeader = showFoldoutHeader;
			}
			if (UnityEngine.UIElements.UxmlSerializedData.ShouldWriteAttributeValue(headerTitle_UxmlAttributeFlags))
			{
				baseListView.headerTitle = headerTitle;
			}
			if (UnityEngine.UIElements.UxmlSerializedData.ShouldWriteAttributeValue(showAddRemoveFooter_UxmlAttributeFlags))
			{
				baseListView.showAddRemoveFooter = showAddRemoveFooter;
			}
			if (UnityEngine.UIElements.UxmlSerializedData.ShouldWriteAttributeValue(allowAdd_UxmlAttributeFlags))
			{
				baseListView.allowAdd = allowAdd;
			}
			if (UnityEngine.UIElements.UxmlSerializedData.ShouldWriteAttributeValue(allowRemove_UxmlAttributeFlags))
			{
				baseListView.allowRemove = allowRemove;
			}
			if (UnityEngine.UIElements.UxmlSerializedData.ShouldWriteAttributeValue(reorderMode_UxmlAttributeFlags))
			{
				baseListView.reorderMode = reorderMode;
			}
			if (UnityEngine.UIElements.UxmlSerializedData.ShouldWriteAttributeValue(showBoundCollectionSize_UxmlAttributeFlags))
			{
				baseListView.showBoundCollectionSize = showBoundCollectionSize;
			}
			if (UnityEngine.UIElements.UxmlSerializedData.ShouldWriteAttributeValue(bindingSourceSelectionMode_UxmlAttributeFlags))
			{
				baseListView.bindingSourceSelectionMode = bindingSourceSelectionMode;
			}
		}
	}

	[Obsolete("UxmlTraits is deprecated and will be removed. Use UxmlElementAttribute instead.", false)]
	public new class UxmlTraits : BaseVerticalCollectionView.UxmlTraits
	{
		private readonly UxmlBoolAttributeDescription m_ShowFoldoutHeader = new UxmlBoolAttributeDescription
		{
			name = "show-foldout-header",
			defaultValue = false
		};

		private readonly UxmlStringAttributeDescription m_HeaderTitle = new UxmlStringAttributeDescription
		{
			name = "header-title",
			defaultValue = string.Empty
		};

		private readonly UxmlBoolAttributeDescription m_ShowAddRemoveFooter = new UxmlBoolAttributeDescription
		{
			name = "show-add-remove-footer",
			defaultValue = false
		};

		private readonly UxmlBoolAttributeDescription m_AllowAdd = new UxmlBoolAttributeDescription
		{
			name = "allow-add",
			defaultValue = true
		};

		private readonly UxmlBoolAttributeDescription m_AllowRemove = new UxmlBoolAttributeDescription
		{
			name = "allow-remove",
			defaultValue = true
		};

		private readonly UxmlEnumAttributeDescription<ListViewReorderMode> m_ReorderMode = new UxmlEnumAttributeDescription<ListViewReorderMode>
		{
			name = "reorder-mode",
			defaultValue = ListViewReorderMode.Simple
		};

		private readonly UxmlBoolAttributeDescription m_ShowBoundCollectionSize = new UxmlBoolAttributeDescription
		{
			name = "show-bound-collection-size",
			defaultValue = true
		};

		private readonly UxmlEnumAttributeDescription<BindingSourceSelectionMode> m_BindingSourceSelectionMode = new UxmlEnumAttributeDescription<BindingSourceSelectionMode>
		{
			name = "binding-source-selection-mode",
			defaultValue = BindingSourceSelectionMode.Manual
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
			BaseListView baseListView = (BaseListView)ve;
			baseListView.reorderMode = m_ReorderMode.GetValueFromBag(bag, cc);
			baseListView.showFoldoutHeader = m_ShowFoldoutHeader.GetValueFromBag(bag, cc);
			baseListView.headerTitle = m_HeaderTitle.GetValueFromBag(bag, cc);
			baseListView.showAddRemoveFooter = m_ShowAddRemoveFooter.GetValueFromBag(bag, cc);
			baseListView.allowAdd = m_AllowAdd.GetValueFromBag(bag, cc);
			baseListView.allowRemove = m_AllowRemove.GetValueFromBag(bag, cc);
			baseListView.showBoundCollectionSize = m_ShowBoundCollectionSize.GetValueFromBag(bag, cc);
			baseListView.bindingSourceSelectionMode = m_BindingSourceSelectionMode.GetValueFromBag(bag, cc);
		}

		protected UxmlTraits()
		{
			m_PickingMode.defaultValue = PickingMode.Ignore;
		}
	}

	private static readonly string k_SizeFieldLabel = "Size";

	internal static readonly BindingId showBoundCollectionSizeProperty = "showBoundCollectionSize";

	internal static readonly BindingId showFoldoutHeaderProperty = "showFoldoutHeader";

	internal static readonly BindingId headerTitleProperty = "headerTitle";

	internal static readonly BindingId makeHeaderProperty = "makeHeader";

	internal static readonly BindingId makeFooterProperty = "makeFooter";

	internal static readonly BindingId showAddRemoveFooterProperty = "showAddRemoveFooter";

	internal static readonly BindingId bindingSourceSelectionModeProperty = "bindingSourceSelectionMode";

	internal static readonly BindingId reorderModeProperty = "reorderMode";

	internal static readonly BindingId makeNoneElementProperty = "makeNoneElement";

	internal static readonly BindingId allowAddProperty = "allowAdd";

	internal static readonly BindingId overridingAddButtonBehaviorProperty = "overridingAddButtonBehavior";

	internal static readonly BindingId onAddProperty = "onAdd";

	internal static readonly BindingId allowRemoveProperty = "allowRemove";

	internal static readonly BindingId onRemoveProperty = "onRemove";

	private const int k_FoldoutTabIndex = 10;

	private const int k_ArraySizeFieldTabIndex = 20;

	private bool m_ShowBoundCollectionSize = true;

	private bool m_ShowFoldoutHeader;

	private string m_HeaderTitle;

	private VisualElement drawnHeader;

	private Func<VisualElement> m_MakeHeader;

	private VisualElement drawnFooter;

	private Func<VisualElement> m_MakeFooter;

	private bool m_ShowAddRemoveFooter;

	private IVisualElementScheduledItem m_TrackedItem;

	private Action m_TrackCount;

	private Func<bool> m_WhileAutoAssign;

	private BindingSourceSelectionMode m_BindingSourceSelectionMode = BindingSourceSelectionMode.Manual;

	private Label m_ListViewLabel;

	private Foldout m_Foldout;

	private TextField m_ArraySizeField;

	private bool m_IsOverMultiEditLimit;

	private int m_MaxMultiEditCount;

	private VisualElement m_Footer;

	private Button m_AddButton;

	private Button m_RemoveButton;

	private Action<IEnumerable<int>> m_ItemAddedCallback;

	private Action<IEnumerable<int>> m_ItemRemovedCallback;

	private Action m_ItemsSourceSizeChangedCallback;

	private ListViewReorderMode m_ReorderMode;

	private VisualElement m_NoneElement;

	private Func<VisualElement> m_MakeNoneElement;

	private bool m_AllowAdd = true;

	private Action<BaseListView, Button> m_OverridingAddButtonBehavior;

	private Action<BaseListView> m_OnAdd;

	private bool m_AllowRemove = true;

	private Action<BaseListView> m_OnRemove;

	public new static readonly string ussClassName = "unity-list-view";

	public new static readonly string itemUssClassName = ussClassName + "__item";

	public static readonly string emptyLabelUssClassName = ussClassName + "__empty-label";

	public static readonly string overMaxMultiEditLimitClassName = ussClassName + "__over-max-multi-edit-limit-label";

	public static readonly string reorderableUssClassName = ussClassName + "__reorderable";

	public static readonly string reorderableItemUssClassName = reorderableUssClassName + "-item";

	public static readonly string reorderableItemContainerUssClassName = reorderableItemUssClassName + "__container";

	public static readonly string reorderableItemHandleUssClassName = reorderableUssClassName + "-handle";

	public static readonly string reorderableItemHandleBarUssClassName = reorderableItemHandleUssClassName + "-bar";

	public static readonly string footerUssClassName = ussClassName + "__footer";

	public static readonly string foldoutHeaderUssClassName = ussClassName + "__foldout-header";

	public static readonly string arraySizeFieldUssClassName = ussClassName + "__size-field";

	public static readonly string arraySizeFieldWithHeaderUssClassName = arraySizeFieldUssClassName + "--with-header";

	public static readonly string arraySizeFieldWithFooterUssClassName = arraySizeFieldUssClassName + "--with-footer";

	public static readonly string listViewWithHeaderUssClassName = ussClassName + "--with-header";

	public static readonly string listViewWithFooterUssClassName = ussClassName + "--with-footer";

	public static readonly string scrollViewWithFooterUssClassName = ussClassName + "__scroll-view--with-footer";

	public static readonly string footerAddButtonName = ussClassName + "__add-button";

	public static readonly string footerRemoveButtonName = ussClassName + "__remove-button";

	private string m_MaxMultiEditStr;

	private static readonly string k_EmptyListStr = "List is empty";

	[CreateProperty]
	public bool showBoundCollectionSize
	{
		get
		{
			return m_ShowBoundCollectionSize;
		}
		set
		{
			if (m_ShowBoundCollectionSize != value)
			{
				m_ShowBoundCollectionSize = value;
				SetupArraySizeField();
				NotifyPropertyChanged(in showBoundCollectionSizeProperty);
			}
		}
	}

	[CreateProperty]
	public bool showFoldoutHeader
	{
		get
		{
			return m_ShowFoldoutHeader;
		}
		set
		{
			bool flag = m_ShowFoldoutHeader;
			m_ShowFoldoutHeader = value;
			try
			{
				if (makeHeader != null)
				{
					return;
				}
				EnableInClassList(listViewWithHeaderUssClassName, value);
				if (m_ShowFoldoutHeader)
				{
					AddFoldout();
				}
				else if (m_Foldout != null)
				{
					drawnFooter?.RemoveFromHierarchy();
					RemoveFoldout();
				}
				SetupArraySizeField();
				UpdateListViewLabel();
				if (makeFooter == null)
				{
					if (showAddRemoveFooter)
					{
						EnableFooter(enabled: true);
					}
				}
				else if (m_ShowFoldoutHeader)
				{
					drawnFooter?.RemoveFromHierarchy();
					m_Foldout?.contentContainer.Add(drawnFooter);
				}
				else
				{
					base.hierarchy.Add(drawnFooter);
					base.hierarchy.BringToFront(drawnFooter);
				}
			}
			finally
			{
				if (flag != m_ShowFoldoutHeader)
				{
					NotifyPropertyChanged(in showFoldoutHeaderProperty);
				}
			}
		}
	}

	[CreateProperty]
	public string headerTitle
	{
		get
		{
			return m_HeaderTitle;
		}
		set
		{
			string strA = m_HeaderTitle;
			m_HeaderTitle = value;
			if (m_Foldout != null)
			{
				m_Foldout.text = m_HeaderTitle;
			}
			if (string.CompareOrdinal(strA, m_HeaderTitle) != 0)
			{
				NotifyPropertyChanged(in headerTitleProperty);
			}
		}
	}

	[CreateProperty]
	public Func<VisualElement> makeHeader
	{
		get
		{
			return m_MakeHeader;
		}
		set
		{
			if (value == m_MakeHeader)
			{
				return;
			}
			RemoveFoldout();
			m_MakeHeader = value;
			if (m_MakeHeader != null)
			{
				SetupArraySizeField();
				drawnHeader = m_MakeHeader();
				drawnHeader.tabIndex = 1;
				base.hierarchy.Add(drawnHeader);
				base.hierarchy.SendToBack(drawnHeader);
			}
			else
			{
				drawnHeader?.RemoveFromHierarchy();
				drawnHeader = null;
				if (showFoldoutHeader)
				{
					AddFoldout();
					SetupArraySizeField();
					UpdateListViewLabel();
				}
			}
			if (drawnFooter != null)
			{
				if (m_Foldout != null)
				{
					drawnFooter.RemoveFromHierarchy();
					m_Foldout.contentContainer.hierarchy.Add(drawnFooter);
				}
				else
				{
					base.hierarchy.Add(drawnFooter);
					drawnFooter?.BringToFront();
				}
			}
			else
			{
				EnableFooter(showAddRemoveFooter);
			}
			NotifyPropertyChanged(in makeHeaderProperty);
		}
	}

	[CreateProperty]
	public Func<VisualElement> makeFooter
	{
		get
		{
			return m_MakeFooter;
		}
		set
		{
			if (value == m_MakeFooter)
			{
				return;
			}
			m_MakeFooter = value;
			if (m_MakeFooter != null)
			{
				m_Footer?.RemoveFromHierarchy();
				m_Footer = null;
				drawnFooter = m_MakeFooter();
				if (m_Foldout != null)
				{
					m_Foldout.contentContainer.Add(drawnFooter);
				}
				else
				{
					base.hierarchy.Add(drawnFooter);
					base.hierarchy.BringToFront(drawnFooter);
				}
				EnableInClassList(listViewWithFooterUssClassName, enable: true);
				base.scrollView.EnableInClassList(scrollViewWithFooterUssClassName, enable: true);
			}
			else
			{
				drawnFooter?.RemoveFromHierarchy();
				drawnFooter = null;
				EnableFooter(m_ShowAddRemoveFooter);
			}
			NotifyPropertyChanged(in makeFooterProperty);
		}
	}

	[CreateProperty]
	public bool showAddRemoveFooter
	{
		get
		{
			return m_Footer != null;
		}
		set
		{
			bool flag = showAddRemoveFooter;
			m_ShowAddRemoveFooter = value;
			if (makeFooter == null)
			{
				EnableFooter(value);
			}
			if (value && m_ArraySizeField != null)
			{
				m_ArraySizeField.AddToClassList(arraySizeFieldWithFooterUssClassName);
			}
			if (flag != showFoldoutHeader)
			{
				NotifyPropertyChanged(in showAddRemoveFooterProperty);
			}
		}
	}

	internal Foldout headerFoldout
	{
		[VisibleToOtherModules(new string[] { "UnityEditor.UIBuilderModule" })]
		get
		{
			return m_Foldout;
		}
	}

	private IVisualElementScheduledItem trackItemCount
	{
		get
		{
			if (m_TrackedItem != null)
			{
				return m_TrackedItem;
			}
			m_TrackedItem = base.schedule.Execute(trackCount).Until(untilManualBindingSourceSelectionMode);
			return m_TrackedItem;
		}
	}

	private Action trackCount => delegate
	{
		if (base.itemsSource?.Count != m_PreviousRefreshedCount)
		{
			RefreshItems();
		}
	};

	private Func<bool> untilManualBindingSourceSelectionMode => () => !autoAssignSource;

	[CreateProperty]
	public BindingSourceSelectionMode bindingSourceSelectionMode
	{
		get
		{
			return m_BindingSourceSelectionMode;
		}
		set
		{
			if (m_BindingSourceSelectionMode != value)
			{
				m_BindingSourceSelectionMode = value;
				Rebuild();
				NotifyPropertyChanged(in bindingSourceSelectionModeProperty);
				if (autoAssignSource)
				{
					trackItemCount.Resume();
				}
			}
		}
	}

	internal bool autoAssignSource => bindingSourceSelectionMode == BindingSourceSelectionMode.AutoAssign;

	internal TextField arraySizeField => m_ArraySizeField;

	internal VisualElement footer => m_Footer;

	public new BaseListViewController viewController => base.viewController as BaseListViewController;

	[CreateProperty]
	public ListViewReorderMode reorderMode
	{
		get
		{
			return m_ReorderMode;
		}
		set
		{
			if (value != m_ReorderMode)
			{
				m_ReorderMode = value;
				InitializeDragAndDropController(base.reorderable);
				this.reorderModeChanged?.Invoke();
				Rebuild();
				NotifyPropertyChanged(in reorderModeProperty);
			}
		}
	}

	[CreateProperty]
	public Func<VisualElement> makeNoneElement
	{
		get
		{
			return m_MakeNoneElement;
		}
		set
		{
			if (value != m_MakeNoneElement)
			{
				m_MakeNoneElement = value;
				m_NoneElement?.RemoveFromHierarchy();
				m_NoneElement = null;
				RefreshItems();
				NotifyPropertyChanged(in makeNoneElementProperty);
			}
		}
	}

	[CreateProperty]
	public bool allowAdd
	{
		get
		{
			return m_AllowAdd;
		}
		set
		{
			if (value != m_AllowAdd)
			{
				m_AllowAdd = value;
				m_AddButton?.SetEnabled(m_AllowAdd);
				NotifyPropertyChanged(in allowAddProperty);
			}
		}
	}

	[CreateProperty]
	public Action<BaseListView, Button> overridingAddButtonBehavior
	{
		get
		{
			return m_OverridingAddButtonBehavior;
		}
		set
		{
			if (value != m_OverridingAddButtonBehavior)
			{
				m_OverridingAddButtonBehavior = value;
				RefreshItems();
				NotifyPropertyChanged(in overridingAddButtonBehaviorProperty);
			}
		}
	}

	[CreateProperty]
	public Action<BaseListView> onAdd
	{
		get
		{
			return m_OnAdd;
		}
		set
		{
			if (value != m_OnAdd)
			{
				m_OnAdd = value;
				RefreshItems();
				NotifyPropertyChanged(in onAddProperty);
			}
		}
	}

	[CreateProperty]
	public bool allowRemove
	{
		get
		{
			return m_AllowRemove;
		}
		set
		{
			if (value != m_AllowRemove)
			{
				m_AllowRemove = value;
				UpdateRemoveButton();
				NotifyPropertyChanged(in allowRemoveProperty);
			}
		}
	}

	[CreateProperty]
	public Action<BaseListView> onRemove
	{
		get
		{
			return m_OnRemove;
		}
		set
		{
			if (value != m_OnRemove)
			{
				m_OnRemove = value;
				RefreshItems();
				NotifyPropertyChanged(in onRemoveProperty);
			}
		}
	}

	public event Action<IEnumerable<int>> itemsAdded;

	public event Action<IEnumerable<int>> itemsRemoved;

	internal event Action itemsSourceSizeChanged;

	internal event Action reorderModeChanged;

	private void AddFoldout()
	{
		if (m_Foldout == null)
		{
			m_Foldout = new Foldout
			{
				name = foldoutHeaderUssClassName,
				text = m_HeaderTitle
			};
			m_Foldout.toggle.tabIndex = 10;
			m_Foldout.toggle.acceptClicksIfDisabled = true;
			m_Foldout.AddToClassList(foldoutHeaderUssClassName);
			base.hierarchy.Add(m_Foldout);
			m_Foldout.Add(base.scrollView);
		}
	}

	private void RemoveFoldout()
	{
		m_Foldout?.RemoveFromHierarchy();
		m_Foldout = null;
		base.hierarchy.Add(base.scrollView);
	}

	internal void SetupArraySizeField()
	{
		if (!showBoundCollectionSize || (!showFoldoutHeader && GetProperty("__unity-collection-view-internal-binding") == null) || drawnHeader != null)
		{
			m_ArraySizeField?.RemoveFromHierarchy();
			return;
		}
		if (m_ArraySizeField == null)
		{
			m_ArraySizeField = new TextField
			{
				name = arraySizeFieldUssClassName,
				tabIndex = 20
			};
			m_ArraySizeField.AddToClassList(arraySizeFieldUssClassName);
			m_ArraySizeField.RegisterValueChangedCallback(OnArraySizeFieldChanged);
			m_ArraySizeField.isDelayed = true;
			m_ArraySizeField.focusable = true;
		}
		m_ArraySizeField.EnableInClassList(arraySizeFieldWithFooterUssClassName, showAddRemoveFooter);
		m_ArraySizeField.EnableInClassList(arraySizeFieldWithHeaderUssClassName, showFoldoutHeader);
		if (showFoldoutHeader)
		{
			m_ArraySizeField.label = string.Empty;
			base.hierarchy.Add(m_ArraySizeField);
		}
		else
		{
			m_ArraySizeField.label = k_SizeFieldLabel;
			base.hierarchy.Insert(0, m_ArraySizeField);
		}
		UpdateArraySizeField();
	}

	private void EnableFooter(bool enabled)
	{
		EnableInClassList(listViewWithFooterUssClassName, enabled);
		base.scrollView.EnableInClassList(scrollViewWithFooterUssClassName, enabled);
		if (enabled)
		{
			if (m_Footer == null)
			{
				m_Footer = new VisualElement
				{
					name = footerUssClassName
				};
				m_Footer.AddToClassList(footerUssClassName);
				m_AddButton = new Button(OnAddClicked)
				{
					name = footerAddButtonName,
					text = "+"
				};
				m_AddButton.SetEnabled(allowAdd);
				m_Footer.Add(m_AddButton);
				m_RemoveButton = new Button(OnRemoveClicked)
				{
					name = footerRemoveButtonName,
					text = "-"
				};
				m_Footer.Add(m_RemoveButton);
				UpdateRemoveButton();
			}
			if (m_Foldout != null)
			{
				m_Foldout.contentContainer.Add(m_Footer);
			}
			else
			{
				base.hierarchy.Add(m_Footer);
			}
		}
		else
		{
			m_RemoveButton?.RemoveFromHierarchy();
			m_AddButton?.RemoveFromHierarchy();
			m_Footer?.RemoveFromHierarchy();
			m_RemoveButton = null;
			m_AddButton = null;
			m_Footer = null;
		}
	}

	private void AddItems(int itemCount)
	{
		if (GetOrCreateViewController() is BaseListViewController baseListViewController)
		{
			baseListViewController.AddItems(itemCount);
		}
	}

	private void RemoveItems(List<int> indices)
	{
		viewController.RemoveItems(indices);
	}

	private void OnArraySizeFieldChanged(ChangeEvent<string> evt)
	{
		if (m_ArraySizeField.showMixedValue && BaseField<string>.mixedValueString == evt.newValue)
		{
			return;
		}
		if (!int.TryParse(evt.newValue, out var result) || result < 0)
		{
			m_ArraySizeField.SetValueWithoutNotify(evt.previousValue);
			return;
		}
		int itemsCount = viewController.GetItemsCount();
		if (itemsCount != 0 || result != viewController.GetItemsMinCount())
		{
			if (result > itemsCount)
			{
				viewController.AddItems(result - itemsCount);
			}
			else if (result < itemsCount)
			{
				viewController.RemoveItems(itemsCount - result);
			}
			else if (result == 0)
			{
				viewController.ClearItems();
				m_IsOverMultiEditLimit = false;
			}
			UpdateListViewLabel();
		}
	}

	private void UpdateRemoveButton()
	{
		Button removeButton = m_RemoveButton;
		if (removeButton != null)
		{
			int enabled;
			if (allowRemove)
			{
				BaseListViewController baseListViewController = viewController;
				enabled = ((baseListViewController != null && baseListViewController.GetItemsCount() > 0) ? 1 : 0);
			}
			else
			{
				enabled = 0;
			}
			removeButton.SetEnabled((byte)enabled != 0);
		}
	}

	internal void UpdateArraySizeField()
	{
		if (HasValidDataAndBindings() && m_ArraySizeField != null)
		{
			if (!m_ArraySizeField.showMixedValue)
			{
				m_ArraySizeField.SetValueWithoutNotify(viewController.GetItemsMinCount().ToString());
			}
			footer?.SetEnabled(!m_IsOverMultiEditLimit);
		}
	}

	internal void UpdateListViewLabel()
	{
		if (!HasValidDataAndBindings())
		{
			return;
		}
		bool flag = base.itemsSource.Count == 0;
		if (m_IsOverMultiEditLimit)
		{
			if (m_ListViewLabel == null)
			{
				m_ListViewLabel = new Label();
			}
			m_ListViewLabel.text = m_MaxMultiEditStr;
			base.scrollView.contentViewport.Add(m_ListViewLabel);
		}
		else if (flag)
		{
			if (m_MakeNoneElement != null)
			{
				if (m_NoneElement == null)
				{
					m_NoneElement = m_MakeNoneElement();
				}
				base.scrollView.contentViewport.Add(m_NoneElement);
				m_ListViewLabel?.RemoveFromHierarchy();
				m_ListViewLabel = null;
			}
			else
			{
				if (m_ListViewLabel == null)
				{
					m_ListViewLabel = new Label();
				}
				m_ListViewLabel.text = k_EmptyListStr;
				base.scrollView.contentViewport.Add(m_ListViewLabel);
				m_NoneElement?.RemoveFromHierarchy();
				m_NoneElement = null;
			}
		}
		else
		{
			m_NoneElement?.RemoveFromHierarchy();
			m_NoneElement = null;
			m_ListViewLabel?.RemoveFromHierarchy();
			m_ListViewLabel = null;
		}
		m_ListViewLabel?.EnableInClassList(emptyLabelUssClassName, flag);
		m_ListViewLabel?.EnableInClassList(overMaxMultiEditLimitClassName, m_IsOverMultiEditLimit);
	}

	private void OnAddClicked()
	{
		int itemsCountPreCallback = base.itemsSource?.Count ?? 0;
		if (overridingAddButtonBehavior != null)
		{
			overridingAddButtonBehavior(this, m_AddButton);
		}
		else if (onAdd != null)
		{
			onAdd(this);
		}
		else
		{
			AddItems(1);
		}
		if (GetProperty("__unity-collection-view-internal-binding") == null)
		{
			OnAfterAddClicked(itemsCountPreCallback);
		}
		else
		{
			base.schedule.Execute((Action)delegate
			{
				OnAfterAddClicked(itemsCountPreCallback);
			}).ExecuteLater(100L);
		}
		if (HasValidDataAndBindings() && m_ArraySizeField != null)
		{
			m_ArraySizeField.showMixedValue = false;
		}
	}

	private void OnAfterAddClicked(int itemsCountPreCallback)
	{
		if (base.itemsSource != null && itemsCountPreCallback != base.itemsSource.Count)
		{
			OnItemsSourceSizeChanged();
			SetSelection(base.itemsSource.Count - 1);
			ScrollToItem(-1);
		}
	}

	private void OnRemoveClicked()
	{
		if (onRemove != null)
		{
			onRemove(this);
		}
		else if (base.selectedIndices.Any())
		{
			viewController.RemoveItems(base.selectedIndices.ToList());
			ClearSelection();
		}
		else
		{
			IList list = base.itemsSource;
			if (list != null && list.Count > 0)
			{
				int index = base.itemsSource.Count - 1;
				viewController.RemoveItem(index);
			}
		}
		if (HasValidDataAndBindings() && m_ArraySizeField != null)
		{
			m_ArraySizeField.showMixedValue = false;
		}
	}

	internal void SetOverMaxMultiEditLimit(bool isOverLimit, int maxMultiEditCount)
	{
		m_IsOverMultiEditLimit = isOverLimit;
		m_MaxMultiEditCount = maxMultiEditCount;
		m_MaxMultiEditStr = $"This field cannot display arrays with more than {m_MaxMultiEditCount} elements when multiple objects are selected.";
	}

	private protected override void CreateVirtualizationController()
	{
		CreateVirtualizationController<ReusableListViewItem>();
	}

	public override void SetViewController(CollectionViewController controller)
	{
		if (m_ItemAddedCallback == null)
		{
			m_ItemAddedCallback = OnItemAdded;
		}
		if (m_ItemRemovedCallback == null)
		{
			m_ItemRemovedCallback = OnItemsRemoved;
		}
		if (m_ItemsSourceSizeChangedCallback == null)
		{
			m_ItemsSourceSizeChangedCallback = OnItemsSourceSizeChanged;
		}
		if (viewController != null)
		{
			viewController.itemsAdded -= m_ItemAddedCallback;
			viewController.itemsRemoved -= m_ItemRemovedCallback;
			viewController.itemsSourceSizeChanged -= m_ItemsSourceSizeChangedCallback;
		}
		base.SetViewController(controller);
		if (viewController != null)
		{
			viewController.itemsAdded += m_ItemAddedCallback;
			viewController.itemsRemoved += m_ItemRemovedCallback;
			viewController.itemsSourceSizeChanged += m_ItemsSourceSizeChangedCallback;
		}
	}

	private void OnItemAdded(IEnumerable<int> indices)
	{
		this.itemsAdded?.Invoke(indices);
	}

	private void OnItemsRemoved(IEnumerable<int> indices)
	{
		this.itemsRemoved?.Invoke(indices);
	}

	private void OnItemsSourceSizeChanged()
	{
		if (GetProperty("__unity-collection-view-internal-binding") == null)
		{
			RefreshItems();
		}
		this.itemsSourceSizeChanged?.Invoke();
	}

	internal override ListViewDragger CreateDragger()
	{
		if (m_ReorderMode == ListViewReorderMode.Simple)
		{
			return new ListViewDragger(this);
		}
		return new ListViewDraggerAnimated(this);
	}

	internal override ICollectionDragAndDropController CreateDragAndDropController()
	{
		return new ListViewReorderableDragAndDropController(this);
	}

	public BaseListView()
		: this(null)
	{
	}

	public BaseListView(IList itemsSource, float itemHeight = -1f)
		: base(itemsSource, itemHeight)
	{
		AddToClassList(ussClassName);
		base.pickingMode = PickingMode.Ignore;
		allowAdd = true;
		allowRemove = true;
	}

	private protected override void PostRefresh()
	{
		UpdateArraySizeField();
		UpdateRemoveButton();
		UpdateListViewLabel();
		base.PostRefresh();
	}

	private protected override bool HandleItemNavigation(bool moveIn, bool altPressed)
	{
		bool result = false;
		foreach (int selectedIndex in base.selectedIndices)
		{
			foreach (ReusableCollectionItem activeItem in base.activeItems)
			{
				if (activeItem.index == selectedIndex && GetProperty("__unity-collection-view-internal-binding") != null)
				{
					Foldout foldout = activeItem.bindableElement.Q<Foldout>();
					if (foldout != null)
					{
						foldout.value = moveIn;
						result = true;
					}
				}
			}
		}
		return result;
	}
}
