using System;
using System.Diagnostics;
using Unity.Properties;
using UnityEngine.Bindings;
using UnityEngine.Internal;

namespace UnityEngine.UIElements;

public class GroupBox : BindableElement, IGroupBox
{
	[Serializable]
	[ExcludeFromDocs]
	public new class UxmlSerializedData : BindableElement.UxmlSerializedData
	{
		[MultilineTextField]
		[SerializeField]
		private string text;

		[UxmlIgnore]
		[SerializeField]
		[HideInInspector]
		private UxmlAttributeFlags text_UxmlAttributeFlags;

		[Conditional("UNITY_EDITOR")]
		public new static void Register()
		{
			UxmlDescriptionCache.RegisterType(typeof(UxmlSerializedData), new UxmlAttributeNames[1]
			{
				new UxmlAttributeNames("text", "text", null)
			});
		}

		public override object CreateInstance()
		{
			return new GroupBox();
		}

		public override void Deserialize(object obj)
		{
			base.Deserialize(obj);
			if (UnityEngine.UIElements.UxmlSerializedData.ShouldWriteAttributeValue(text_UxmlAttributeFlags))
			{
				GroupBox groupBox = (GroupBox)obj;
				groupBox.text = text;
			}
		}
	}

	[Obsolete("UxmlFactory is deprecated and will be removed. Use UxmlElementAttribute instead.", false)]
	public new class UxmlFactory : UxmlFactory<GroupBox, UxmlTraits>
	{
	}

	[Obsolete("UxmlTraits is deprecated and will be removed. Use UxmlElementAttribute instead.", false)]
	public new class UxmlTraits : BindableElement.UxmlTraits
	{
		private UxmlStringAttributeDescription m_Text = new UxmlStringAttributeDescription
		{
			name = "text"
		};

		public override void Init(VisualElement ve, IUxmlAttributes bag, CreationContext cc)
		{
			base.Init(ve, bag, cc);
			((GroupBox)ve).text = m_Text.GetValueFromBag(bag, cc);
		}
	}

	internal static readonly BindingId textProperty = "text";

	public static readonly string ussClassName = "unity-group-box";

	public static readonly string labelUssClassName = ussClassName + "__label";

	private Label m_TitleLabel;

	internal Label titleLabel
	{
		[VisibleToOtherModules(new string[] { "UnityEditor.UIBuilderModule" })]
		get
		{
			return m_TitleLabel;
		}
	}

	[CreateProperty]
	public string text
	{
		get
		{
			return m_TitleLabel?.text;
		}
		set
		{
			string strA = text;
			if (!string.IsNullOrEmpty(value))
			{
				if (m_TitleLabel == null)
				{
					m_TitleLabel = new Label(value);
					m_TitleLabel.AddToClassList(labelUssClassName);
					Insert(0, m_TitleLabel);
				}
				m_TitleLabel.text = value;
			}
			else if (m_TitleLabel != null)
			{
				m_TitleLabel.RemoveFromHierarchy();
				m_TitleLabel = null;
			}
			if (string.CompareOrdinal(strA, text) != 0)
			{
				NotifyPropertyChanged(in textProperty);
			}
		}
	}

	public GroupBox()
		: this(null)
	{
	}

	public GroupBox(string text)
	{
		AddToClassList(ussClassName);
		this.text = text;
	}

	void IGroupBox.OnOptionAdded(IGroupBoxOption option)
	{
	}

	void IGroupBox.OnOptionRemoved(IGroupBoxOption option)
	{
	}
}
