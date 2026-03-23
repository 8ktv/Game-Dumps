using System;
using System.Collections.Generic;
using System.Diagnostics;
using Unity.Properties;
using UnityEngine.Internal;

namespace UnityEngine.UIElements;

public class MultiColumnListView : BaseListView
{
	[Serializable]
	[ExcludeFromDocs]
	public new class UxmlSerializedData : BaseListView.UxmlSerializedData
	{
		[UxmlObjectReference]
		[SerializeReference]
		private Columns.UxmlSerializedData columns;

		[UxmlObjectReference]
		[SerializeReference]
		private SortColumnDescriptions.UxmlSerializedData sortColumnDescriptions;

		[SerializeField]
		private ColumnSortingMode sortingMode;

		[SerializeField]
		[HideInInspector]
		private bool sortingEnabled;

		[UxmlIgnore]
		[SerializeField]
		[HideInInspector]
		private UxmlAttributeFlags sortingEnabled_UxmlAttributeFlags;

		[HideInInspector]
		[UxmlIgnore]
		[SerializeField]
		private UxmlAttributeFlags sortingMode_UxmlAttributeFlags;

		[SerializeField]
		[HideInInspector]
		[UxmlIgnore]
		private UxmlAttributeFlags columns_UxmlAttributeFlags;

		[SerializeField]
		[UxmlIgnore]
		[HideInInspector]
		private UxmlAttributeFlags sortColumnDescriptions_UxmlAttributeFlags;

		[Conditional("UNITY_EDITOR")]
		public new static void Register()
		{
			UxmlDescriptionCache.RegisterType(typeof(UxmlSerializedData), new UxmlAttributeNames[4]
			{
				new UxmlAttributeNames("sortingEnabled", "sorting-enabled", null),
				new UxmlAttributeNames("sortingMode", "sorting-mode", null),
				new UxmlAttributeNames("columns", "columns", null),
				new UxmlAttributeNames("sortColumnDescriptions", "sort-column-descriptions", null)
			});
		}

		public override object CreateInstance()
		{
			return new MultiColumnListView();
		}

		public override void Deserialize(object obj)
		{
			base.Deserialize(obj);
			MultiColumnListView multiColumnListView = (MultiColumnListView)obj;
			if (UnityEngine.UIElements.UxmlSerializedData.ShouldWriteAttributeValue(sortingMode_UxmlAttributeFlags))
			{
				multiColumnListView.sortingMode = sortingMode;
			}
			else if (UnityEngine.UIElements.UxmlSerializedData.ShouldWriteAttributeValue(sortingEnabled_UxmlAttributeFlags))
			{
				multiColumnListView.sortingEnabled = sortingEnabled;
			}
			if (UnityEngine.UIElements.UxmlSerializedData.ShouldWriteAttributeValue(sortColumnDescriptions_UxmlAttributeFlags) && sortColumnDescriptions != null)
			{
				SortColumnDescriptions obj2 = new SortColumnDescriptions();
				sortColumnDescriptions.Deserialize(obj2);
				multiColumnListView.sortColumnDescriptions = obj2;
			}
			if (UnityEngine.UIElements.UxmlSerializedData.ShouldWriteAttributeValue(columns_UxmlAttributeFlags) && columns != null)
			{
				Columns obj3 = new Columns();
				columns.Deserialize(obj3);
				multiColumnListView.columns = obj3;
			}
		}
	}

	[Obsolete("UxmlFactory is deprecated and will be removed. Use UxmlElementAttribute instead.", false)]
	public new class UxmlFactory : UxmlFactory<MultiColumnListView, UxmlTraits>
	{
	}

	[Obsolete("UxmlTraits is deprecated and will be removed. Use UxmlElementAttribute instead.", false)]
	public new class UxmlTraits : BaseListView.UxmlTraits
	{
		private readonly UxmlEnumAttributeDescription<ColumnSortingMode> m_SortingMode = new UxmlEnumAttributeDescription<ColumnSortingMode>
		{
			name = "sorting-mode",
			obsoleteNames = new string[1] { "sorting-enabled" }
		};

		private readonly UxmlObjectAttributeDescription<Columns> m_Columns = new UxmlObjectAttributeDescription<Columns>();

		private readonly UxmlObjectAttributeDescription<SortColumnDescriptions> m_SortColumnDescriptions = new UxmlObjectAttributeDescription<SortColumnDescriptions>();

		public override void Init(VisualElement ve, IUxmlAttributes bag, CreationContext cc)
		{
			base.Init(ve, bag, cc);
			MultiColumnListView multiColumnListView = (MultiColumnListView)ve;
			if (m_SortingMode.TryGetValueFromBagAsString(bag, cc, out var value))
			{
				if (bool.TryParse(value, out var result))
				{
					multiColumnListView.sortingMode = (result ? ColumnSortingMode.Custom : ColumnSortingMode.None);
				}
				else
				{
					multiColumnListView.sortingMode = m_SortingMode.GetValueFromBag(bag, cc);
				}
			}
			multiColumnListView.sortColumnDescriptions = m_SortColumnDescriptions.GetValueFromBag(bag, cc);
			multiColumnListView.columns = m_Columns.GetValueFromBag(bag, cc);
		}
	}

	private static readonly BindingId columnsProperty = "columns";

	private static readonly BindingId sortColumnDescriptionsProperty = "sortColumnDescriptions";

	private static readonly BindingId sortingModeProperty = "sortingMode";

	private Columns m_Columns;

	private ColumnSortingMode m_SortingMode;

	private SortColumnDescriptions m_SortColumnDescriptions = new SortColumnDescriptions();

	private List<SortColumnDescription> m_SortedColumns = new List<SortColumnDescription>();

	public new MultiColumnListViewController viewController => base.viewController as MultiColumnListViewController;

	public IEnumerable<SortColumnDescription> sortedColumns => m_SortedColumns;

	[CreateProperty]
	public Columns columns
	{
		get
		{
			return m_Columns;
		}
		private set
		{
			if (m_Columns != null)
			{
				m_Columns.propertyChanged -= ColumnsChanged;
			}
			if (value == null)
			{
				m_Columns.Clear();
				return;
			}
			m_Columns = value;
			m_Columns.propertyChanged += ColumnsChanged;
			if (m_Columns.Count > 0)
			{
				GetOrCreateViewController();
			}
			NotifyPropertyChanged(in columnsProperty);
		}
	}

	[CreateProperty]
	public SortColumnDescriptions sortColumnDescriptions
	{
		get
		{
			return m_SortColumnDescriptions;
		}
		private set
		{
			if (value == null)
			{
				m_SortColumnDescriptions.Clear();
				return;
			}
			m_SortColumnDescriptions = value;
			if (viewController != null)
			{
				viewController.columnController.header.sortDescriptions = value;
				RaiseColumnSortingChanged();
			}
			NotifyPropertyChanged(in sortColumnDescriptionsProperty);
		}
	}

	[Obsolete("sortingEnabled has been deprecated. Use sortingMode instead.", false)]
	public bool sortingEnabled
	{
		get
		{
			return sortingMode == ColumnSortingMode.Custom;
		}
		set
		{
			sortingMode = (value ? ColumnSortingMode.Custom : ColumnSortingMode.None);
		}
	}

	[CreateProperty]
	public ColumnSortingMode sortingMode
	{
		get
		{
			return m_SortingMode;
		}
		set
		{
			if (sortingMode != value)
			{
				m_SortingMode = value;
				if (viewController != null)
				{
					viewController.columnController.sortingMode = value;
				}
				NotifyPropertyChanged(in sortingModeProperty);
			}
		}
	}

	public event Action columnSortingChanged;

	public event Action<ContextualMenuPopulateEvent, Column> headerContextMenuPopulateEvent;

	public MultiColumnListView()
		: this(new Columns())
	{
	}

	public MultiColumnListView(Columns columns)
	{
		base.scrollView.viewDataKey = "unity-multi-column-scroll-view";
		this.columns = columns ?? new Columns();
	}

	protected override CollectionViewController CreateViewController()
	{
		return new MultiColumnListViewController(columns, sortColumnDescriptions, m_SortedColumns);
	}

	public override void SetViewController(CollectionViewController controller)
	{
		if (viewController != null)
		{
			viewController.columnController.columnSortingChanged -= RaiseColumnSortingChanged;
			viewController.columnController.headerContextMenuPopulateEvent -= RaiseHeaderContextMenuPopulate;
		}
		base.SetViewController(controller);
		if (viewController != null)
		{
			viewController.columnController.sortingMode = m_SortingMode;
			viewController.columnController.columnSortingChanged += RaiseColumnSortingChanged;
			viewController.columnController.headerContextMenuPopulateEvent += RaiseHeaderContextMenuPopulate;
		}
	}

	private protected override void CreateVirtualizationController()
	{
		CreateVirtualizationController<ReusableMultiColumnListViewItem>();
	}

	private void RaiseColumnSortingChanged()
	{
		this.columnSortingChanged?.Invoke();
	}

	private void ColumnsChanged(object sender, BindablePropertyChangedEventArgs args)
	{
		NotifyPropertyChanged(args.propertyName);
	}

	private void RaiseHeaderContextMenuPopulate(ContextualMenuPopulateEvent evt, Column column)
	{
		this.headerContextMenuPopulateEvent?.Invoke(evt, column);
	}
}
