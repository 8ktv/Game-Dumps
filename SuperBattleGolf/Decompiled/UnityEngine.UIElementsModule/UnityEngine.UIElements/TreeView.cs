using System;
using System.Collections.Generic;
using System.Diagnostics;
using Unity.Properties;
using UnityEngine.Internal;

namespace UnityEngine.UIElements;

public class TreeView : BaseTreeView
{
	[Serializable]
	[ExcludeFromDocs]
	public new class UxmlSerializedData : BaseTreeView.UxmlSerializedData
	{
		[SerializeField]
		private VisualTreeAsset itemTemplate;

		[HideInInspector]
		[UxmlIgnore]
		[SerializeField]
		private UxmlAttributeFlags itemTemplate_UxmlAttributeFlags;

		[Conditional("UNITY_EDITOR")]
		public new static void Register()
		{
			UxmlDescriptionCache.RegisterType(typeof(UxmlSerializedData), new UxmlAttributeNames[1]
			{
				new UxmlAttributeNames("itemTemplate", "item-template", null)
			});
		}

		public override object CreateInstance()
		{
			return new TreeView();
		}

		public override void Deserialize(object obj)
		{
			base.Deserialize(obj);
			if (UnityEngine.UIElements.UxmlSerializedData.ShouldWriteAttributeValue(itemTemplate_UxmlAttributeFlags))
			{
				TreeView treeView = (TreeView)obj;
				treeView.itemTemplate = itemTemplate;
			}
		}
	}

	[Obsolete("UxmlFactory is deprecated and will be removed. Use UxmlElementAttribute instead.", false)]
	public new class UxmlFactory : UxmlFactory<TreeView, UxmlTraits>
	{
	}

	[Obsolete("UxmlTraits is deprecated and will be removed. Use UxmlElementAttribute instead.", false)]
	public new class UxmlTraits : BaseTreeView.UxmlTraits
	{
		private UxmlAssetAttributeDescription<VisualTreeAsset> m_ItemTemplate = new UxmlAssetAttributeDescription<VisualTreeAsset>
		{
			name = "item-template"
		};

		public override void Init(VisualElement ve, IUxmlAttributes bag, CreationContext cc)
		{
			base.Init(ve, bag, cc);
			TreeView treeView = ve as TreeView;
			if (m_ItemTemplate.TryGetValueFromBag(bag, cc, out var value))
			{
				treeView.itemTemplate = value;
			}
		}
	}

	internal static readonly BindingId itemTemplateProperty = "itemTemplate";

	internal static readonly BindingId makeItemProperty = "makeItem";

	internal static readonly BindingId bindItemProperty = "bindItem";

	internal static readonly BindingId unbindItemProperty = "unbindItem";

	internal static readonly BindingId destroyItemProperty = "destroyItem";

	private Func<VisualElement> m_MakeItem;

	private Func<VisualElement> m_TemplateMakeItem;

	private VisualTreeAsset m_ItemTemplate;

	private Action<VisualElement, int> m_BindItem;

	private Action<VisualElement, int> m_UnbindItem;

	private Action<VisualElement> m_DestroyItem;

	[CreateProperty]
	public new Func<VisualElement> makeItem
	{
		get
		{
			return m_MakeItem;
		}
		set
		{
			if (value != m_MakeItem)
			{
				m_MakeItem = value;
				Rebuild();
				NotifyPropertyChanged(in makeItemProperty);
			}
		}
	}

	[CreateProperty]
	public VisualTreeAsset itemTemplate
	{
		get
		{
			return m_ItemTemplate;
		}
		set
		{
			if (!(m_ItemTemplate == value))
			{
				m_ItemTemplate = value;
				if (makeItem != m_TemplateMakeItem)
				{
					makeItem = m_TemplateMakeItem;
				}
				else
				{
					Rebuild();
				}
				NotifyPropertyChanged(in itemTemplateProperty);
			}
		}
	}

	[CreateProperty]
	public new Action<VisualElement, int> bindItem
	{
		get
		{
			return m_BindItem;
		}
		set
		{
			if (value != m_BindItem)
			{
				m_BindItem = value;
				RefreshItems();
				NotifyPropertyChanged(in bindItemProperty);
			}
		}
	}

	[CreateProperty]
	public new Action<VisualElement, int> unbindItem
	{
		get
		{
			return m_UnbindItem;
		}
		set
		{
			if (value != m_UnbindItem)
			{
				m_UnbindItem = value;
				NotifyPropertyChanged(in unbindItemProperty);
			}
		}
	}

	[CreateProperty]
	public new Action<VisualElement> destroyItem
	{
		get
		{
			return m_DestroyItem;
		}
		set
		{
			if (value != m_DestroyItem)
			{
				m_DestroyItem = value;
				NotifyPropertyChanged(in destroyItemProperty);
			}
		}
	}

	public new TreeViewController viewController => base.viewController as TreeViewController;

	private VisualElement TemplateMakeItem()
	{
		if (m_ItemTemplate != null)
		{
			return m_ItemTemplate.Instantiate();
		}
		return new Label(BaseVerticalCollectionView.k_InvalidTemplateError);
	}

	internal override void SetRootItemsInternal<T>(IList<TreeViewItemData<T>> rootItems)
	{
		TreeViewHelpers<T, DefaultTreeViewController<T>>.SetRootItems(this, rootItems, () => new DefaultTreeViewController<T>());
	}

	internal override bool HasValidDataAndBindings()
	{
		return base.HasValidDataAndBindings() && makeItem != null == (bindItem != null);
	}

	protected override CollectionViewController CreateViewController()
	{
		return new DefaultTreeViewController<object>();
	}

	public TreeView()
		: this(null, null)
	{
	}

	public TreeView(Func<VisualElement> makeItem, Action<VisualElement, int> bindItem)
		: base(-1)
	{
		this.makeItem = makeItem;
		this.bindItem = bindItem;
		m_TemplateMakeItem = TemplateMakeItem;
	}

	public TreeView(int itemHeight, Func<VisualElement> makeItem, Action<VisualElement, int> bindItem)
		: this(makeItem, bindItem)
	{
		base.fixedItemHeight = itemHeight;
	}

	private protected override IEnumerable<TreeViewItemData<T>> GetSelectedItemsInternal<T>()
	{
		return TreeViewHelpers<T, DefaultTreeViewController<T>>.GetSelectedItems(this);
	}

	private protected override T GetItemDataForIndexInternal<T>(int index)
	{
		return TreeViewHelpers<T, DefaultTreeViewController<T>>.GetItemDataForIndex(this, index);
	}

	private protected override T GetItemDataForIdInternal<T>(int id)
	{
		return TreeViewHelpers<T, DefaultTreeViewController<T>>.GetItemDataForId(this, id);
	}

	private protected override void AddItemInternal<T>(TreeViewItemData<T> item, int parentId, int childIndex, bool rebuildTree)
	{
		TreeViewHelpers<T, DefaultTreeViewController<T>>.AddItem(this, item, parentId, childIndex, rebuildTree);
	}
}
