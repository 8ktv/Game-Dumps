using System;
using System.Diagnostics;
using Unity.Properties;
using UnityEngine.Bindings;
using UnityEngine.Internal;

namespace UnityEngine.UIElements;

public class Tab : VisualElement
{
	[Serializable]
	[ExcludeFromDocs]
	public new class UxmlSerializedData : VisualElement.UxmlSerializedData
	{
		[SerializeField]
		[MultilineTextField]
		private string label;

		[UxmlAttribute("icon-image")]
		[UxmlAttributeBindingPath("iconImage")]
		[ImageFieldValueDecorator]
		[SerializeField]
		private Object iconImageReference;

		[SerializeField]
		private bool closeable;

		[SerializeField]
		[UxmlIgnore]
		[HideInInspector]
		private UxmlAttributeFlags label_UxmlAttributeFlags;

		[HideInInspector]
		[SerializeField]
		[UxmlIgnore]
		private UxmlAttributeFlags iconImageReference_UxmlAttributeFlags;

		[SerializeField]
		[UxmlIgnore]
		[HideInInspector]
		private UxmlAttributeFlags closeable_UxmlAttributeFlags;

		[Conditional("UNITY_EDITOR")]
		public new static void Register()
		{
			UxmlDescriptionCache.RegisterType(typeof(UxmlSerializedData), new UxmlAttributeNames[3]
			{
				new UxmlAttributeNames("label", "label", null),
				new UxmlAttributeNames("iconImageReference", "icon-image", null),
				new UxmlAttributeNames("closeable", "closeable", null)
			});
		}

		public override object CreateInstance()
		{
			return new Tab();
		}

		public override void Deserialize(object obj)
		{
			base.Deserialize(obj);
			Tab tab = (Tab)obj;
			if (UnityEngine.UIElements.UxmlSerializedData.ShouldWriteAttributeValue(label_UxmlAttributeFlags))
			{
				tab.label = label;
			}
			if (UnityEngine.UIElements.UxmlSerializedData.ShouldWriteAttributeValue(iconImageReference_UxmlAttributeFlags))
			{
				tab.iconImageReference = iconImageReference;
			}
			if (UnityEngine.UIElements.UxmlSerializedData.ShouldWriteAttributeValue(closeable_UxmlAttributeFlags))
			{
				tab.closeable = closeable;
			}
		}
	}

	[Obsolete("UxmlFactory is deprecated and will be removed. Use UxmlElementAttribute instead.", false)]
	public new class UxmlFactory : UxmlFactory<Tab, UxmlTraits>
	{
	}

	[Obsolete("UxmlTraits is deprecated and will be removed. Use UxmlElementAttribute instead.", false)]
	public new class UxmlTraits : VisualElement.UxmlTraits
	{
		private readonly UxmlStringAttributeDescription m_Label = new UxmlStringAttributeDescription
		{
			name = "label"
		};

		private readonly UxmlImageAttributeDescription m_IconImage = new UxmlImageAttributeDescription
		{
			name = "icon-image"
		};

		private readonly UxmlBoolAttributeDescription m_Closeable = new UxmlBoolAttributeDescription
		{
			name = "closeable",
			defaultValue = false
		};

		public override void Init(VisualElement ve, IUxmlAttributes bag, CreationContext cc)
		{
			base.Init(ve, bag, cc);
			Tab tab = (Tab)ve;
			tab.label = m_Label.GetValueFromBag(bag, cc);
			tab.iconImage = m_IconImage.GetValueFromBag(bag, cc);
			tab.closeable = m_Closeable.GetValueFromBag(bag, cc);
		}
	}

	internal static readonly BindingId labelProperty = "label";

	internal static readonly BindingId iconImageProperty = "iconImage";

	internal static readonly BindingId closeableProperty = "closeable";

	public static readonly string ussClassName = "unity-tab";

	public static readonly string tabHeaderUssClassName = ussClassName + "__header";

	public static readonly string tabHeaderImageUssClassName = tabHeaderUssClassName + "-image";

	public static readonly string tabHeaderEmptyImageUssClassName = tabHeaderImageUssClassName + "--empty";

	public static readonly string tabHeaderStandaloneImageUssClassName = tabHeaderImageUssClassName + "--standalone";

	public static readonly string tabHeaderLabelUssClassName = tabHeaderUssClassName + "-label";

	public static readonly string tabHeaderEmptyLabeUssClassName = tabHeaderLabelUssClassName + "--empty";

	public static readonly string tabHeaderUnderlineUssClassName = tabHeaderUssClassName + "-underline";

	public static readonly string contentUssClassName = ussClassName + "__content-container";

	public static readonly string draggingUssClassName = ussClassName + "--dragging";

	public static readonly string reorderableUssClassName = ussClassName + "__reorderable";

	public static readonly string reorderableItemHandleUssClassName = reorderableUssClassName + "-handle";

	public static readonly string reorderableItemHandleBarUssClassName = reorderableItemHandleUssClassName + "-bar";

	public static readonly string closeableUssClassName = tabHeaderUssClassName + "__closeable";

	public static readonly string closeButtonUssClassName = ussClassName + "__close-button";

	private string m_Label;

	private Background m_IconImage;

	private bool m_Closeable;

	private VisualElement m_ContentContainer;

	private VisualElement m_DragHandle;

	private VisualElement m_CloseButton;

	private VisualElement m_TabHeader;

	private Image m_TabHeaderImage;

	private Label m_TabHeaderLabel;

	private Object iconImageReference
	{
		get
		{
			return iconImage.GetSelectedImage();
		}
		set
		{
			iconImage = Background.FromObject(value);
		}
	}

	internal Label headerLabel
	{
		[VisibleToOtherModules(new string[] { "UnityEditor.UIBuilderModule" })]
		get
		{
			return m_TabHeaderLabel;
		}
	}

	public VisualElement tabHeader => m_TabHeader;

	internal TabDragger dragger { get; }

	internal int index { get; set; }

	[CreateProperty]
	public string label
	{
		get
		{
			return m_Label;
		}
		set
		{
			if (string.CompareOrdinal(value, m_Label) != 0)
			{
				m_TabHeaderLabel.text = value;
				m_TabHeaderLabel.EnableInClassList(tabHeaderEmptyLabeUssClassName, string.IsNullOrEmpty(value));
				m_TabHeaderImage.EnableInClassList(tabHeaderStandaloneImageUssClassName, string.IsNullOrEmpty(value));
				m_Label = value;
				NotifyPropertyChanged(in labelProperty);
			}
		}
	}

	[CreateProperty]
	public Background iconImage
	{
		get
		{
			return m_IconImage;
		}
		set
		{
			if (value == m_IconImage)
			{
				return;
			}
			if (value.IsEmpty())
			{
				m_TabHeaderImage.image = null;
				m_TabHeaderImage.sprite = null;
				m_TabHeaderImage.vectorImage = null;
				m_TabHeaderImage.AddToClassList(tabHeaderEmptyImageUssClassName);
				m_TabHeaderImage.RemoveFromClassList(tabHeaderStandaloneImageUssClassName);
				m_IconImage = value;
				NotifyPropertyChanged(in iconImageProperty);
				return;
			}
			if ((bool)value.texture)
			{
				m_TabHeaderImage.image = value.texture;
			}
			else if ((bool)value.sprite)
			{
				m_TabHeaderImage.sprite = value.sprite;
			}
			else if ((bool)value.renderTexture)
			{
				m_TabHeaderImage.image = value.renderTexture;
			}
			else
			{
				m_TabHeaderImage.vectorImage = value.vectorImage;
			}
			m_TabHeaderImage.RemoveFromClassList(tabHeaderEmptyImageUssClassName);
			m_TabHeaderImage.EnableInClassList(tabHeaderStandaloneImageUssClassName, string.IsNullOrEmpty(m_Label));
			m_IconImage = value;
			NotifyPropertyChanged(in iconImageProperty);
		}
	}

	[CreateProperty]
	public bool closeable
	{
		get
		{
			return m_Closeable;
		}
		set
		{
			if (m_Closeable != value)
			{
				m_Closeable = value;
				m_TabHeader.EnableInClassList(closeableUssClassName, value);
				EnableTabCloseButton(value);
				NotifyPropertyChanged(in closeableProperty);
			}
		}
	}

	public override VisualElement contentContainer => m_ContentContainer;

	public event Action<Tab> selected;

	public event Func<bool> closing;

	public event Action<Tab> closed;

	public Tab()
		: this(null, null)
	{
	}

	public Tab(string label)
		: this(label, null)
	{
	}

	public Tab(Background iconImage)
		: this(null, iconImage)
	{
	}

	public Tab(string label, Background iconImage)
	{
		AddToClassList(ussClassName);
		m_TabHeader = new VisualElement
		{
			classList = { tabHeaderUssClassName },
			name = tabHeaderUssClassName
		};
		m_DragHandle = new VisualElement
		{
			name = reorderableItemHandleUssClassName,
			classList = { reorderableItemHandleUssClassName }
		};
		m_DragHandle.AddToClassList(reorderableItemHandleUssClassName);
		m_DragHandle.Add(new VisualElement
		{
			name = reorderableItemHandleBarUssClassName,
			classList = 
			{
				reorderableItemHandleBarUssClassName,
				reorderableItemHandleBarUssClassName + "--left"
			}
		});
		m_DragHandle.Add(new VisualElement
		{
			name = reorderableItemHandleBarUssClassName,
			classList = { reorderableItemHandleBarUssClassName }
		});
		m_TabHeaderImage = new Image
		{
			name = tabHeaderImageUssClassName,
			classList = { tabHeaderImageUssClassName, tabHeaderEmptyImageUssClassName }
		};
		m_TabHeader.Add(m_TabHeaderImage);
		m_TabHeaderLabel = new Label
		{
			name = tabHeaderLabelUssClassName,
			classList = { tabHeaderLabelUssClassName }
		};
		m_TabHeader.Add(m_TabHeaderLabel);
		m_TabHeader.RegisterCallback<PointerDownEvent>(OnTabClicked);
		m_TabHeader.Add(new VisualElement
		{
			name = tabHeaderUnderlineUssClassName,
			classList = { tabHeaderUnderlineUssClassName }
		});
		m_CloseButton = new VisualElement
		{
			name = closeButtonUssClassName,
			classList = { closeButtonUssClassName }
		};
		m_CloseButton.RegisterCallback<PointerDownEvent>(OnCloseButtonClicked);
		base.hierarchy.Add(m_TabHeader);
		m_ContentContainer = new VisualElement
		{
			name = contentUssClassName,
			classList = { contentUssClassName },
			userData = m_TabHeader
		};
		base.hierarchy.Add(m_ContentContainer);
		this.label = label;
		this.iconImage = iconImage;
		m_DragHandle.AddManipulator(dragger = new TabDragger());
		m_TabHeader.RegisterCallback<TooltipEvent>(UpdateTooltip);
		RegisterCallback(delegate(TooltipEvent evt)
		{
			evt.StopImmediatePropagation();
		});
	}

	private void UpdateTooltip(TooltipEvent evt)
	{
		if (evt.currentTarget is VisualElement visualElement && !string.IsNullOrEmpty(base.tooltip))
		{
			evt.rect = visualElement.GetTooltipRect();
			evt.tooltip = base.tooltip;
			evt.StopImmediatePropagation();
		}
	}

	private void AddDragHandles()
	{
		m_TabHeader.Insert(0, m_DragHandle);
	}

	private void RemoveDragHandles()
	{
		if (m_TabHeader.Contains(m_DragHandle))
		{
			m_TabHeader.Remove(m_DragHandle);
		}
	}

	internal void EnableTabDragHandles(bool enable)
	{
		if (enable)
		{
			AddDragHandles();
		}
		else
		{
			RemoveDragHandles();
		}
	}

	private void AddCloseButton()
	{
		m_TabHeader.Add(m_CloseButton);
	}

	private void RemoveCloseButton()
	{
		if (m_TabHeader.Contains(m_CloseButton))
		{
			m_TabHeader.Remove(m_CloseButton);
		}
	}

	internal void EnableTabCloseButton(bool enable)
	{
		if (enable)
		{
			AddCloseButton();
		}
		else
		{
			RemoveCloseButton();
		}
	}

	internal void SetActive()
	{
		m_TabHeader.pseudoStates |= PseudoStates.Checked;
		base.pseudoStates |= PseudoStates.Checked;
	}

	internal void SetInactive()
	{
		m_TabHeader.pseudoStates &= ~PseudoStates.Checked;
		base.pseudoStates &= ~PseudoStates.Checked;
	}

	private void OnTabClicked(PointerDownEvent _)
	{
		this.selected?.Invoke(this);
	}

	private void OnCloseButtonClicked(PointerDownEvent evt)
	{
		Func<bool> func = this.closing;
		if (func == null || func())
		{
			RemoveFromHierarchy();
			this.closed?.Invoke(this);
		}
		evt.StopPropagation();
	}
}
