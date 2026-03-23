using System;
using System.Diagnostics;
using Unity.Properties;
using UnityEngine.Internal;

namespace UnityEngine.UIElements;

[Serializable]
[UxmlObject]
public class SortColumnDescription : INotifyBindablePropertyChanged
{
	[Serializable]
	[ExcludeFromDocs]
	public class UxmlSerializedData : UnityEngine.UIElements.UxmlSerializedData
	{
		[SerializeField]
		private string columnName;

		[SerializeField]
		private int columnIndex;

		[SerializeField]
		private SortDirection direction;

		[HideInInspector]
		[SerializeField]
		[UxmlIgnore]
		private UxmlAttributeFlags columnName_UxmlAttributeFlags;

		[HideInInspector]
		[UxmlIgnore]
		[SerializeField]
		private UxmlAttributeFlags columnIndex_UxmlAttributeFlags;

		[HideInInspector]
		[UxmlIgnore]
		[SerializeField]
		private UxmlAttributeFlags direction_UxmlAttributeFlags;

		[Conditional("UNITY_EDITOR")]
		public new static void Register()
		{
			UxmlDescriptionCache.RegisterType(typeof(UxmlSerializedData), new UxmlAttributeNames[3]
			{
				new UxmlAttributeNames("columnName", "column-name", null),
				new UxmlAttributeNames("columnIndex", "column-index", null),
				new UxmlAttributeNames("direction", "direction", null)
			});
		}

		public override object CreateInstance()
		{
			return new SortColumnDescription();
		}

		public override void Deserialize(object obj)
		{
			SortColumnDescription sortColumnDescription = (SortColumnDescription)obj;
			if (UnityEngine.UIElements.UxmlSerializedData.ShouldWriteAttributeValue(columnName_UxmlAttributeFlags))
			{
				sortColumnDescription.columnName = columnName;
			}
			if (UnityEngine.UIElements.UxmlSerializedData.ShouldWriteAttributeValue(columnIndex_UxmlAttributeFlags))
			{
				sortColumnDescription.columnIndex = columnIndex;
			}
			if (UnityEngine.UIElements.UxmlSerializedData.ShouldWriteAttributeValue(direction_UxmlAttributeFlags))
			{
				sortColumnDescription.direction = direction;
			}
		}
	}

	[Obsolete("UxmlObjectFactory<T> is deprecated and will be removed. Use UxmlElementAttribute instead.", false)]
	internal class UxmlObjectFactory<T> : UxmlObjectFactory<T, UxmlObjectTraits<T>> where T : SortColumnDescription, new()
	{
	}

	[Obsolete("UxmlObjectFactory<T> is deprecated and will be removed. Use UxmlElementAttribute instead.", false)]
	internal class UxmlObjectFactory : UxmlObjectFactory<SortColumnDescription>
	{
	}

	[Obsolete("UxmlObjectTraits<T> is deprecated and will be removed. Use UxmlElementAttribute instead.", false)]
	internal class UxmlObjectTraits<T> : UnityEngine.UIElements.UxmlObjectTraits<T> where T : SortColumnDescription
	{
		private readonly UxmlStringAttributeDescription m_ColumnName = new UxmlStringAttributeDescription
		{
			name = "column-name"
		};

		private readonly UxmlIntAttributeDescription m_ColumnIndex = new UxmlIntAttributeDescription
		{
			name = "column-index",
			defaultValue = -1
		};

		private readonly UxmlEnumAttributeDescription<SortDirection> m_SortDescription = new UxmlEnumAttributeDescription<SortDirection>
		{
			name = "direction",
			defaultValue = SortDirection.Ascending
		};

		public override void Init(ref T obj, IUxmlAttributes bag, CreationContext cc)
		{
			base.Init(ref obj, bag, cc);
			string valueFromBag = m_ColumnName.GetValueFromBag(bag, cc);
			obj.columnName = valueFromBag;
			int valueFromBag2 = m_ColumnIndex.GetValueFromBag(bag, cc);
			obj.columnIndex = valueFromBag2;
			SortDirection valueFromBag3 = m_SortDescription.GetValueFromBag(bag, cc);
			obj.direction = valueFromBag3;
		}
	}

	private static readonly BindingId columnNameProperty = "columnName";

	private static readonly BindingId columnIndexProperty = "columnIndex";

	private static readonly BindingId directionProperty = "direction";

	[SerializeField]
	private int m_ColumnIndex = -1;

	[SerializeField]
	private string m_ColumnName;

	[SerializeField]
	private SortDirection m_SortDirection;

	[CreateProperty]
	public string columnName
	{
		get
		{
			return m_ColumnName;
		}
		set
		{
			if (!(m_ColumnName == value))
			{
				m_ColumnName = value;
				this.changed?.Invoke(this);
				NotifyPropertyChanged(in columnNameProperty);
			}
		}
	}

	[CreateProperty]
	public int columnIndex
	{
		get
		{
			return m_ColumnIndex;
		}
		set
		{
			if (m_ColumnIndex != value)
			{
				m_ColumnIndex = value;
				this.changed?.Invoke(this);
				NotifyPropertyChanged(in columnIndexProperty);
			}
		}
	}

	public Column column { get; internal set; }

	[CreateProperty]
	public SortDirection direction
	{
		get
		{
			return m_SortDirection;
		}
		set
		{
			if (m_SortDirection != value)
			{
				m_SortDirection = value;
				this.changed?.Invoke(this);
				NotifyPropertyChanged(in directionProperty);
			}
		}
	}

	public event EventHandler<BindablePropertyChangedEventArgs> propertyChanged;

	internal event Action<SortColumnDescription> changed;

	public SortColumnDescription()
	{
	}

	public SortColumnDescription(int columnIndex, SortDirection direction)
	{
		this.columnIndex = columnIndex;
		this.direction = direction;
	}

	public SortColumnDescription(string columnName, SortDirection direction)
	{
		this.columnName = columnName;
		this.direction = direction;
	}

	private void NotifyPropertyChanged(in BindingId property)
	{
		this.propertyChanged?.Invoke(this, new BindablePropertyChangedEventArgs(in property));
	}
}
