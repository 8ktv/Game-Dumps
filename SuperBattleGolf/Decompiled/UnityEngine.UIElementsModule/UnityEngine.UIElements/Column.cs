using System;
using System.Collections.Generic;
using System.Diagnostics;
using Unity.Properties;
using UnityEngine.Internal;

namespace UnityEngine.UIElements;

[UxmlObject]
public class Column : INotifyBindablePropertyChanged
{
	[Serializable]
	[ExcludeFromDocs]
	public class UxmlSerializedData : UnityEngine.UIElements.UxmlSerializedData
	{
		[SerializeField]
		private VisualTreeAsset headerTemplate;

		[SerializeField]
		private VisualTreeAsset cellTemplate;

		[SerializeField]
		private string name;

		[SerializeField]
		private string title;

		[SerializeField]
		private string bindingPath;

		[SerializeField]
		private Length width;

		[SerializeField]
		private Length minWidth;

		[SerializeField]
		private Length maxWidth;

		[SerializeField]
		private bool visible;

		[SerializeField]
		private bool stretchable;

		[SerializeField]
		private bool sortable;

		[SerializeField]
		private bool optional;

		[SerializeField]
		private bool resizable;

		[HideInInspector]
		[SerializeField]
		[UxmlIgnore]
		private UxmlAttributeFlags name_UxmlAttributeFlags;

		[SerializeField]
		[UxmlIgnore]
		[HideInInspector]
		private UxmlAttributeFlags title_UxmlAttributeFlags;

		[HideInInspector]
		[SerializeField]
		[UxmlIgnore]
		private UxmlAttributeFlags visible_UxmlAttributeFlags;

		[HideInInspector]
		[UxmlIgnore]
		[SerializeField]
		private UxmlAttributeFlags width_UxmlAttributeFlags;

		[UxmlIgnore]
		[SerializeField]
		[HideInInspector]
		private UxmlAttributeFlags minWidth_UxmlAttributeFlags;

		[SerializeField]
		[UxmlIgnore]
		[HideInInspector]
		private UxmlAttributeFlags maxWidth_UxmlAttributeFlags;

		[UxmlIgnore]
		[SerializeField]
		[HideInInspector]
		private UxmlAttributeFlags stretchable_UxmlAttributeFlags;

		[SerializeField]
		[UxmlIgnore]
		[HideInInspector]
		private UxmlAttributeFlags sortable_UxmlAttributeFlags;

		[SerializeField]
		[UxmlIgnore]
		[HideInInspector]
		private UxmlAttributeFlags optional_UxmlAttributeFlags;

		[HideInInspector]
		[UxmlIgnore]
		[SerializeField]
		private UxmlAttributeFlags resizable_UxmlAttributeFlags;

		[HideInInspector]
		[UxmlIgnore]
		[SerializeField]
		private UxmlAttributeFlags headerTemplate_UxmlAttributeFlags;

		[UxmlIgnore]
		[SerializeField]
		[HideInInspector]
		private UxmlAttributeFlags cellTemplate_UxmlAttributeFlags;

		[UxmlIgnore]
		[SerializeField]
		[HideInInspector]
		private UxmlAttributeFlags bindingPath_UxmlAttributeFlags;

		[Conditional("UNITY_EDITOR")]
		public new static void Register()
		{
			UxmlDescriptionCache.RegisterType(typeof(UxmlSerializedData), new UxmlAttributeNames[13]
			{
				new UxmlAttributeNames("name", "name", null),
				new UxmlAttributeNames("title", "title", null),
				new UxmlAttributeNames("visible", "visible", null),
				new UxmlAttributeNames("width", "width", null),
				new UxmlAttributeNames("minWidth", "min-width", null),
				new UxmlAttributeNames("maxWidth", "max-width", null),
				new UxmlAttributeNames("stretchable", "stretchable", null),
				new UxmlAttributeNames("sortable", "sortable", null),
				new UxmlAttributeNames("optional", "optional", null),
				new UxmlAttributeNames("resizable", "resizable", null),
				new UxmlAttributeNames("headerTemplate", "header-template", null),
				new UxmlAttributeNames("cellTemplate", "cell-template", null),
				new UxmlAttributeNames("bindingPath", "binding-path", null)
			});
		}

		public override object CreateInstance()
		{
			return new Column();
		}

		public override void Deserialize(object obj)
		{
			Column column = (Column)obj;
			if (UnityEngine.UIElements.UxmlSerializedData.ShouldWriteAttributeValue(name_UxmlAttributeFlags))
			{
				column.name = name;
			}
			if (UnityEngine.UIElements.UxmlSerializedData.ShouldWriteAttributeValue(title_UxmlAttributeFlags))
			{
				column.title = title;
			}
			if (UnityEngine.UIElements.UxmlSerializedData.ShouldWriteAttributeValue(visible_UxmlAttributeFlags))
			{
				column.visible = visible;
			}
			if (UnityEngine.UIElements.UxmlSerializedData.ShouldWriteAttributeValue(width_UxmlAttributeFlags))
			{
				column.width = width;
			}
			if (UnityEngine.UIElements.UxmlSerializedData.ShouldWriteAttributeValue(minWidth_UxmlAttributeFlags))
			{
				column.minWidth = minWidth;
			}
			if (UnityEngine.UIElements.UxmlSerializedData.ShouldWriteAttributeValue(maxWidth_UxmlAttributeFlags))
			{
				column.maxWidth = maxWidth;
			}
			if (UnityEngine.UIElements.UxmlSerializedData.ShouldWriteAttributeValue(sortable_UxmlAttributeFlags))
			{
				column.sortable = sortable;
			}
			if (UnityEngine.UIElements.UxmlSerializedData.ShouldWriteAttributeValue(stretchable_UxmlAttributeFlags))
			{
				column.stretchable = stretchable;
			}
			if (UnityEngine.UIElements.UxmlSerializedData.ShouldWriteAttributeValue(optional_UxmlAttributeFlags))
			{
				column.optional = optional;
			}
			if (UnityEngine.UIElements.UxmlSerializedData.ShouldWriteAttributeValue(resizable_UxmlAttributeFlags))
			{
				column.resizable = resizable;
			}
			if (UnityEngine.UIElements.UxmlSerializedData.ShouldWriteAttributeValue(bindingPath_UxmlAttributeFlags))
			{
				column.bindingPath = bindingPath;
			}
			if (UnityEngine.UIElements.UxmlSerializedData.ShouldWriteAttributeValue(headerTemplate_UxmlAttributeFlags) && headerTemplate != null)
			{
				column.headerTemplate = headerTemplate;
				column.makeHeader = () => headerTemplate.Instantiate();
			}
			if (UnityEngine.UIElements.UxmlSerializedData.ShouldWriteAttributeValue(cellTemplate_UxmlAttributeFlags) && cellTemplate != null)
			{
				column.cellTemplate = cellTemplate;
				column.makeCell = () => cellTemplate.Instantiate();
			}
		}
	}

	[Obsolete("UxmlObjectFactory<T> is deprecated and will be removed. Use UxmlElementAttribute instead.", false)]
	internal class UxmlObjectFactory<T> : UxmlObjectFactory<T, UxmlObjectTraits<T>> where T : Column, new()
	{
	}

	[Obsolete("UxmlObjectFactory<T> is deprecated and will be removed. Use UxmlElementAttribute instead.", false)]
	internal class UxmlObjectFactory : UxmlObjectFactory<Column>
	{
	}

	[Obsolete("UxmlObjectTraits<T> is deprecated and will be removed. Use UxmlElementAttribute instead.", false)]
	internal class UxmlObjectTraits<T> : UnityEngine.UIElements.UxmlObjectTraits<T> where T : Column
	{
		private UxmlStringAttributeDescription m_Name = new UxmlStringAttributeDescription
		{
			name = "name"
		};

		private UxmlStringAttributeDescription m_Text = new UxmlStringAttributeDescription
		{
			name = "title"
		};

		private UxmlBoolAttributeDescription m_Visible = new UxmlBoolAttributeDescription
		{
			name = "visible",
			defaultValue = true
		};

		private UxmlStringAttributeDescription m_Width = new UxmlStringAttributeDescription
		{
			name = "width"
		};

		private UxmlStringAttributeDescription m_MinWidth = new UxmlStringAttributeDescription
		{
			name = "min-width"
		};

		private UxmlStringAttributeDescription m_MaxWidth = new UxmlStringAttributeDescription
		{
			name = "max-width"
		};

		private UxmlBoolAttributeDescription m_Stretch = new UxmlBoolAttributeDescription
		{
			name = "stretchable"
		};

		private UxmlBoolAttributeDescription m_Sortable = new UxmlBoolAttributeDescription
		{
			name = "sortable",
			defaultValue = true
		};

		private UxmlBoolAttributeDescription m_Optional = new UxmlBoolAttributeDescription
		{
			name = "optional",
			defaultValue = true
		};

		private UxmlBoolAttributeDescription m_Resizable = new UxmlBoolAttributeDescription
		{
			name = "resizable",
			defaultValue = true
		};

		private UxmlStringAttributeDescription m_HeaderTemplateId = new UxmlStringAttributeDescription
		{
			name = "header-template"
		};

		private UxmlStringAttributeDescription m_CellTemplateId = new UxmlStringAttributeDescription
		{
			name = "cell-template"
		};

		private UxmlStringAttributeDescription m_BindingPath = new UxmlStringAttributeDescription
		{
			name = "binding-path"
		};

		private static Length ParseLength(string str, Length defaultValue)
		{
			float value = defaultValue.value;
			LengthUnit unit = defaultValue.unit;
			int num = 0;
			int num2 = -1;
			for (int i = 0; i < str.Length; i++)
			{
				char c = str[i];
				if (char.IsLetter(c) || c == '%')
				{
					num2 = i;
					break;
				}
				num++;
			}
			string s = str.Substring(0, num);
			string text = string.Empty;
			if (num2 > 0)
			{
				text = str.Substring(num2, str.Length - num2).ToLowerInvariant();
			}
			if (float.TryParse(s, out var result))
			{
				value = result;
			}
			string text2 = text;
			string text3 = text2;
			if (!(text3 == "px"))
			{
				if (text3 == "%")
				{
					unit = LengthUnit.Percent;
				}
			}
			else
			{
				unit = LengthUnit.Pixel;
			}
			return new Length(value, unit);
		}

		public override void Init(ref T obj, IUxmlAttributes bag, CreationContext cc)
		{
			base.Init(ref obj, bag, cc);
			string valueFromBag = m_Name.GetValueFromBag(bag, cc);
			obj.name = valueFromBag;
			string valueFromBag2 = m_Text.GetValueFromBag(bag, cc);
			obj.title = valueFromBag2;
			bool valueFromBag3 = m_Visible.GetValueFromBag(bag, cc);
			obj.visible = valueFromBag3;
			Length width = ParseLength(m_Width.GetValueFromBag(bag, cc), default(Length));
			obj.width = width;
			Length maxWidth = ParseLength(m_MaxWidth.GetValueFromBag(bag, cc), new Length(8388608f));
			obj.maxWidth = maxWidth;
			Length minWidth = ParseLength(m_MinWidth.GetValueFromBag(bag, cc), new Length(35f));
			obj.minWidth = minWidth;
			bool valueFromBag4 = m_Sortable.GetValueFromBag(bag, cc);
			obj.sortable = valueFromBag4;
			bool valueFromBag5 = m_Stretch.GetValueFromBag(bag, cc);
			obj.stretchable = valueFromBag5;
			bool valueFromBag6 = m_Optional.GetValueFromBag(bag, cc);
			obj.optional = valueFromBag6;
			bool valueFromBag7 = m_Resizable.GetValueFromBag(bag, cc);
			obj.resizable = valueFromBag7;
			string valueFromBag8 = m_BindingPath.GetValueFromBag(bag, cc);
			obj.bindingPath = valueFromBag8;
			string valueFromBag9 = m_HeaderTemplateId.GetValueFromBag(bag, cc);
			if (!string.IsNullOrEmpty(valueFromBag9))
			{
				VisualTreeAsset asset = cc.visualTreeAsset?.ResolveTemplate(valueFromBag9);
				Func<VisualElement> makeHeader = () => (asset != null) ? ((BindableElement)asset.Instantiate()) : ((BindableElement)new Label(BaseVerticalCollectionView.k_InvalidTemplateError));
				obj.makeHeader = makeHeader;
			}
			string valueFromBag10 = m_CellTemplateId.GetValueFromBag(bag, cc);
			if (!string.IsNullOrEmpty(valueFromBag10))
			{
				VisualTreeAsset asset2 = cc.visualTreeAsset?.ResolveTemplate(valueFromBag10);
				Func<VisualElement> makeCell = () => (asset2 != null) ? ((BindableElement)asset2.Instantiate()) : ((BindableElement)new Label(BaseVerticalCollectionView.k_InvalidTemplateError));
				obj.makeCell = makeCell;
			}
		}
	}

	private static readonly BindingId nameProperty = "name";

	private static readonly BindingId titleProperty = "title";

	private static readonly BindingId iconProperty = "icon";

	private static readonly BindingId visibleProperty = "visible";

	private static readonly BindingId widthProperty = "width";

	private static readonly BindingId minWidthProperty = "minWidth";

	private static readonly BindingId maxWidthProperty = "maxWidth";

	private static readonly BindingId sortableProperty = "sortable";

	private static readonly BindingId stretchableProperty = "stretchable";

	private static readonly BindingId optionalProperty = "optional";

	private static readonly BindingId resizableProperty = "resizable";

	private static readonly BindingId headerTemplateProperty = "headerTemplate";

	private static readonly BindingId cellTemplateProperty = "cellTemplate";

	internal const string k_HeaderTemplateAttributeName = "header-template";

	internal const string k_CellTemplateAttributeName = "cell-template";

	internal const float kDefaultMinWidth = 35f;

	private string m_Name;

	private string m_Title;

	private Background m_Icon;

	private bool m_Visible = true;

	private Length m_Width = 0f;

	private Length m_MinWidth = 35f;

	private Length m_MaxWidth = 8388608f;

	private float m_DesiredWidth = float.NaN;

	private bool m_Stretchable;

	private bool m_Sortable = true;

	private bool m_Optional = true;

	private bool m_Resizable = true;

	private VisualTreeAsset m_HeaderTemplate;

	private VisualTreeAsset m_CellTemplate;

	private Func<VisualElement> m_MakeHeader;

	private Action<VisualElement> m_BindHeader;

	private Action<VisualElement> m_UnbindHeader;

	private Action<VisualElement> m_DestroyHeader;

	private Func<VisualElement> m_MakeCell;

	private Action<VisualElement, int> m_BindCell;

	private Action<VisualElement, int> m_UnbindCellItem;

	[CreateProperty]
	public string name
	{
		get
		{
			return m_Name;
		}
		set
		{
			if (!(m_Name == value))
			{
				m_Name = value;
				NotifyChange(ColumnDataType.Name);
				NotifyPropertyChanged(in nameProperty);
			}
		}
	}

	[CreateProperty]
	public string title
	{
		get
		{
			return m_Title;
		}
		set
		{
			if (!(m_Title == value))
			{
				m_Title = value;
				NotifyChange(ColumnDataType.Title);
				NotifyPropertyChanged(in titleProperty);
			}
		}
	}

	[CreateProperty]
	public Background icon
	{
		get
		{
			return m_Icon;
		}
		set
		{
			if (!(m_Icon == value))
			{
				m_Icon = value;
				NotifyChange(ColumnDataType.Icon);
				NotifyPropertyChanged(in iconProperty);
			}
		}
	}

	public Comparison<int> comparison { get; set; }

	internal int index => collection?.IndexOf(this) ?? (-1);

	internal int displayIndex => (collection?.displayList as List<Column>)?.IndexOf(this) ?? (-1);

	internal int visibleIndex => (collection?.visibleList as List<Column>)?.IndexOf(this) ?? (-1);

	[CreateProperty]
	public bool visible
	{
		get
		{
			return m_Visible;
		}
		set
		{
			if (m_Visible != value)
			{
				m_Visible = value;
				NotifyChange(ColumnDataType.Visibility);
				NotifyPropertyChanged(in visibleProperty);
			}
		}
	}

	[CreateProperty]
	public Length width
	{
		get
		{
			return m_Width;
		}
		set
		{
			if (!(m_Width == value))
			{
				m_Width = value;
				desiredWidth = float.NaN;
				NotifyChange(ColumnDataType.Width);
				NotifyPropertyChanged(in widthProperty);
			}
		}
	}

	[CreateProperty]
	public Length minWidth
	{
		get
		{
			return m_MinWidth;
		}
		set
		{
			if (!(m_MinWidth == value))
			{
				m_MinWidth = value;
				NotifyChange(ColumnDataType.MinWidth);
				NotifyPropertyChanged(in minWidthProperty);
			}
		}
	}

	[CreateProperty]
	public Length maxWidth
	{
		get
		{
			return m_MaxWidth;
		}
		set
		{
			if (!(m_MaxWidth == value))
			{
				m_MaxWidth = value;
				NotifyChange(ColumnDataType.MaxWidth);
				NotifyPropertyChanged(in maxWidthProperty);
			}
		}
	}

	internal float desiredWidth
	{
		get
		{
			return m_DesiredWidth;
		}
		set
		{
			if (m_DesiredWidth != value)
			{
				m_DesiredWidth = value;
				this.resized?.Invoke(this);
			}
		}
	}

	[CreateProperty]
	public bool sortable
	{
		get
		{
			return m_Sortable;
		}
		set
		{
			if (m_Sortable != value)
			{
				m_Sortable = value;
				NotifyChange(ColumnDataType.Sortable);
				NotifyPropertyChanged(in sortableProperty);
			}
		}
	}

	[CreateProperty]
	public bool stretchable
	{
		get
		{
			return m_Stretchable;
		}
		set
		{
			if (m_Stretchable != value)
			{
				m_Stretchable = value;
				NotifyChange(ColumnDataType.Stretchable);
				NotifyPropertyChanged(in stretchableProperty);
			}
		}
	}

	[CreateProperty]
	public bool optional
	{
		get
		{
			return m_Optional;
		}
		set
		{
			if (m_Optional != value)
			{
				m_Optional = value;
				NotifyChange(ColumnDataType.Optional);
				NotifyPropertyChanged(in optionalProperty);
			}
		}
	}

	[CreateProperty]
	public bool resizable
	{
		get
		{
			return m_Resizable;
		}
		set
		{
			if (m_Resizable != value)
			{
				m_Resizable = value;
				NotifyChange(ColumnDataType.Resizable);
				NotifyPropertyChanged(in resizableProperty);
			}
		}
	}

	public string bindingPath { get; set; }

	[CreateProperty]
	public VisualTreeAsset headerTemplate
	{
		get
		{
			return m_HeaderTemplate;
		}
		set
		{
			if (!(m_HeaderTemplate == value))
			{
				m_HeaderTemplate = value;
				NotifyChange(ColumnDataType.HeaderTemplate);
				NotifyPropertyChanged(in headerTemplateProperty);
			}
		}
	}

	[CreateProperty]
	public VisualTreeAsset cellTemplate
	{
		get
		{
			return m_CellTemplate;
		}
		set
		{
			if (!(m_CellTemplate == value))
			{
				m_CellTemplate = value;
				NotifyChange(ColumnDataType.CellTemplate);
				NotifyPropertyChanged(in cellTemplateProperty);
			}
		}
	}

	public Func<VisualElement> makeHeader
	{
		get
		{
			return m_MakeHeader;
		}
		set
		{
			if (m_MakeHeader != value)
			{
				m_MakeHeader = value;
				NotifyChange(ColumnDataType.HeaderTemplate);
			}
		}
	}

	public Action<VisualElement> bindHeader
	{
		get
		{
			return m_BindHeader;
		}
		set
		{
			if (m_BindHeader != value)
			{
				m_BindHeader = value;
				NotifyChange(ColumnDataType.HeaderTemplate);
			}
		}
	}

	public Action<VisualElement> unbindHeader
	{
		get
		{
			return m_UnbindHeader;
		}
		set
		{
			if (m_UnbindHeader != value)
			{
				m_UnbindHeader = value;
				NotifyChange(ColumnDataType.HeaderTemplate);
			}
		}
	}

	public Action<VisualElement> destroyHeader
	{
		get
		{
			return m_DestroyHeader;
		}
		set
		{
			if (m_DestroyHeader != value)
			{
				m_DestroyHeader = value;
				NotifyChange(ColumnDataType.HeaderTemplate);
			}
		}
	}

	public Func<VisualElement> makeCell
	{
		get
		{
			return m_MakeCell;
		}
		set
		{
			if (m_MakeCell != value)
			{
				m_MakeCell = value;
				NotifyChange(ColumnDataType.CellTemplate);
			}
		}
	}

	public Action<VisualElement, int> bindCell
	{
		get
		{
			return m_BindCell;
		}
		set
		{
			if (m_BindCell != value)
			{
				m_BindCell = value;
				NotifyChange(ColumnDataType.CellTemplate);
			}
		}
	}

	public Action<VisualElement, int> unbindCell
	{
		get
		{
			return m_UnbindCellItem;
		}
		set
		{
			if (m_UnbindCellItem != value)
			{
				m_UnbindCellItem = value;
				NotifyChange(ColumnDataType.CellTemplate);
			}
		}
	}

	public Action<VisualElement> destroyCell { get; set; }

	public Columns collection { get; internal set; }

	public event EventHandler<BindablePropertyChangedEventArgs> propertyChanged;

	internal event Action<Column, ColumnDataType> changed;

	internal event Action<Column> resized;

	private void NotifyChange(ColumnDataType type)
	{
		this.changed?.Invoke(this, type);
	}

	private void NotifyPropertyChanged(in BindingId property)
	{
		this.propertyChanged?.Invoke(this, new BindablePropertyChangedEventArgs(in property));
	}

	internal float GetWidth(float layoutWidth)
	{
		return (width.unit == LengthUnit.Pixel) ? width.value : (width.value * layoutWidth / 100f);
	}

	internal float GetMaxWidth(float layoutWidth)
	{
		return (maxWidth.unit == LengthUnit.Pixel) ? maxWidth.value : (maxWidth.value * layoutWidth / 100f);
	}

	internal float GetMinWidth(float layoutWidth)
	{
		return (minWidth.unit == LengthUnit.Pixel) ? minWidth.value : (minWidth.value * layoutWidth / 100f);
	}
}
