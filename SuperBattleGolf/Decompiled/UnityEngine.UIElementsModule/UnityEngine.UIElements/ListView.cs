using System;
using System.Collections;
using System.Diagnostics;
using Unity.Properties;
using UnityEngine.Internal;

namespace UnityEngine.UIElements;

public class ListView : BaseListView
{
	[Serializable]
	[ExcludeFromDocs]
	public new class UxmlSerializedData : BaseListView.UxmlSerializedData
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
			return new ListView();
		}

		public override void Deserialize(object obj)
		{
			base.Deserialize(obj);
			if (UnityEngine.UIElements.UxmlSerializedData.ShouldWriteAttributeValue(itemTemplate_UxmlAttributeFlags))
			{
				ListView listView = (ListView)obj;
				listView.itemTemplate = itemTemplate;
			}
		}
	}

	[Obsolete("UxmlFactory is deprecated and will be removed. Use UxmlElementAttribute instead.", false)]
	public new class UxmlFactory : UxmlFactory<ListView, UxmlTraits>
	{
	}

	[Obsolete("UxmlTraits is deprecated and will be removed. Use UxmlElementAttribute instead.", false)]
	public new class UxmlTraits : BaseListView.UxmlTraits
	{
		private UxmlAssetAttributeDescription<VisualTreeAsset> m_ItemTemplate = new UxmlAssetAttributeDescription<VisualTreeAsset>
		{
			name = "item-template"
		};

		public override void Init(VisualElement ve, IUxmlAttributes bag, CreationContext cc)
		{
			base.Init(ve, bag, cc);
			ListView listView = ve as ListView;
			if (m_ItemTemplate.TryGetValueFromBag(bag, cc, out var value))
			{
				listView.itemTemplate = value;
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
				if (m_TemplateMakeItem != makeItem)
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

	internal void SetMakeItemWithoutNotify(Func<VisualElement> func)
	{
		m_MakeItem = func;
	}

	private VisualElement TemplateMakeItem()
	{
		if (m_ItemTemplate != null)
		{
			return m_ItemTemplate.Instantiate();
		}
		return new Label(BaseVerticalCollectionView.k_InvalidTemplateError);
	}

	internal void SetBindItemWithoutNotify(Action<VisualElement, int> callback)
	{
		m_BindItem = callback;
	}

	internal override bool HasValidDataAndBindings()
	{
		return base.HasValidDataAndBindings() && ((base.autoAssignSource && makeItem != null) || makeItem != null == (bindItem != null));
	}

	protected override CollectionViewController CreateViewController()
	{
		return new ListViewController();
	}

	public ListView()
	{
		AddToClassList(BaseListView.ussClassName);
		m_TemplateMakeItem = TemplateMakeItem;
	}

	public ListView(IList itemsSource, float itemHeight = -1f, Func<VisualElement> makeItem = null, Action<VisualElement, int> bindItem = null)
		: base(itemsSource, itemHeight)
	{
		AddToClassList(BaseListView.ussClassName);
		m_TemplateMakeItem = TemplateMakeItem;
		this.makeItem = makeItem;
		this.bindItem = bindItem;
	}
}
