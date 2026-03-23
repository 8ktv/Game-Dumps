using System;
using System.Diagnostics;
using Unity.Properties;
using UnityEngine.Internal;

namespace UnityEngine.UIElements;

public class HelpBox : VisualElement
{
	[Serializable]
	[ExcludeFromDocs]
	public new class UxmlSerializedData : VisualElement.UxmlSerializedData
	{
		[SerializeField]
		[MultilineTextField]
		private string text;

		[SerializeField]
		[UxmlIgnore]
		[HideInInspector]
		private UxmlAttributeFlags text_UxmlAttributeFlags;

		[SerializeField]
		private HelpBoxMessageType messageType;

		[HideInInspector]
		[UxmlIgnore]
		[SerializeField]
		private UxmlAttributeFlags messageType_UxmlAttributeFlags;

		[Conditional("UNITY_EDITOR")]
		public new static void Register()
		{
			UxmlDescriptionCache.RegisterType(typeof(UxmlSerializedData), new UxmlAttributeNames[2]
			{
				new UxmlAttributeNames("text", "text", null),
				new UxmlAttributeNames("messageType", "message-type", null)
			});
		}

		public override object CreateInstance()
		{
			return new HelpBox();
		}

		public override void Deserialize(object obj)
		{
			base.Deserialize(obj);
			HelpBox helpBox = (HelpBox)obj;
			if (UnityEngine.UIElements.UxmlSerializedData.ShouldWriteAttributeValue(text_UxmlAttributeFlags))
			{
				helpBox.text = text;
			}
			if (UnityEngine.UIElements.UxmlSerializedData.ShouldWriteAttributeValue(messageType_UxmlAttributeFlags))
			{
				helpBox.messageType = messageType;
			}
		}
	}

	[Obsolete("UxmlFactory is deprecated and will be removed. Use UxmlElementAttribute instead.", false)]
	public new class UxmlFactory : UxmlFactory<HelpBox, UxmlTraits>
	{
	}

	[Obsolete("UxmlTraits is deprecated and will be removed. Use UxmlElementAttribute instead.", false)]
	public new class UxmlTraits : VisualElement.UxmlTraits
	{
		private UxmlStringAttributeDescription m_Text = new UxmlStringAttributeDescription
		{
			name = "text"
		};

		private UxmlEnumAttributeDescription<HelpBoxMessageType> m_MessageType = new UxmlEnumAttributeDescription<HelpBoxMessageType>
		{
			name = "message-type",
			defaultValue = HelpBoxMessageType.None
		};

		public override void Init(VisualElement ve, IUxmlAttributes bag, CreationContext cc)
		{
			base.Init(ve, bag, cc);
			HelpBox helpBox = ve as HelpBox;
			helpBox.text = m_Text.GetValueFromBag(bag, cc);
			helpBox.messageType = m_MessageType.GetValueFromBag(bag, cc);
		}
	}

	internal static readonly BindingId textProperty = "text";

	internal static readonly BindingId messageTypeProperty = "messageType";

	public static readonly string ussClassName = "unity-help-box";

	public static readonly string labelUssClassName = ussClassName + "__label";

	public static readonly string iconUssClassName = ussClassName + "__icon";

	public static readonly string iconInfoUssClassName = iconUssClassName + "--info";

	public static readonly string iconwarningUssClassName = iconUssClassName + "--warning";

	public static readonly string iconErrorUssClassName = iconUssClassName + "--error";

	private HelpBoxMessageType m_HelpBoxMessageType;

	private VisualElement m_Icon;

	private string m_IconClass;

	private Label m_Label;

	[CreateProperty]
	public string text
	{
		get
		{
			return m_Label.text;
		}
		set
		{
			string strA = text;
			m_Label.text = value;
			if (string.CompareOrdinal(strA, text) != 0)
			{
				NotifyPropertyChanged(in textProperty);
			}
		}
	}

	[CreateProperty]
	public HelpBoxMessageType messageType
	{
		get
		{
			return m_HelpBoxMessageType;
		}
		set
		{
			if (value != m_HelpBoxMessageType)
			{
				m_HelpBoxMessageType = value;
				UpdateIcon(value);
				NotifyPropertyChanged(in messageTypeProperty);
			}
		}
	}

	public HelpBox()
		: this(string.Empty, HelpBoxMessageType.None)
	{
	}

	public HelpBox(string text, HelpBoxMessageType messageType)
	{
		AddToClassList(ussClassName);
		m_HelpBoxMessageType = messageType;
		m_Label = new Label(text);
		m_Label.AddToClassList(labelUssClassName);
		Add(m_Label);
		m_Icon = new VisualElement();
		m_Icon.AddToClassList(iconUssClassName);
		UpdateIcon(messageType);
	}

	private string GetIconClass(HelpBoxMessageType messageType)
	{
		return messageType switch
		{
			HelpBoxMessageType.Info => iconInfoUssClassName, 
			HelpBoxMessageType.Warning => iconwarningUssClassName, 
			HelpBoxMessageType.Error => iconErrorUssClassName, 
			_ => null, 
		};
	}

	private void UpdateIcon(HelpBoxMessageType messageType)
	{
		if (!string.IsNullOrEmpty(m_IconClass))
		{
			m_Icon.RemoveFromClassList(m_IconClass);
		}
		m_IconClass = GetIconClass(messageType);
		if (m_IconClass == null)
		{
			m_Icon.RemoveFromHierarchy();
			return;
		}
		m_Icon.AddToClassList(m_IconClass);
		if (m_Icon.parent == null)
		{
			Insert(0, m_Icon);
		}
	}
}
