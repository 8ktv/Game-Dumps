using System;
using System.Diagnostics;
using Unity.Properties;
using UnityEngine.Internal;

namespace UnityEngine.UIElements;

public class Button : TextElement
{
	[Serializable]
	[ExcludeFromDocs]
	public new class UxmlSerializedData : TextElement.UxmlSerializedData
	{
		[UxmlAttributeBindingPath("iconImage")]
		[UxmlAttribute("icon-image")]
		[SerializeField]
		[ImageFieldValueDecorator("Icon Image")]
		private Object iconImageReference;

		[HideInInspector]
		[UxmlIgnore]
		[SerializeField]
		private UxmlAttributeFlags iconImageReference_UxmlAttributeFlags;

		[Conditional("UNITY_EDITOR")]
		public new static void Register()
		{
			UxmlDescriptionCache.RegisterType(typeof(UxmlSerializedData), new UxmlAttributeNames[1]
			{
				new UxmlAttributeNames("iconImageReference", "icon-image", null)
			});
		}

		public override object CreateInstance()
		{
			return new Button();
		}

		public override void Deserialize(object obj)
		{
			base.Deserialize(obj);
			if (UnityEngine.UIElements.UxmlSerializedData.ShouldWriteAttributeValue(iconImageReference_UxmlAttributeFlags))
			{
				Button button = (Button)obj;
				button.iconImageReference = iconImageReference;
			}
		}
	}

	[Obsolete("UxmlFactory is deprecated and will be removed. Use UxmlElementAttribute instead.", false)]
	public new class UxmlFactory : UxmlFactory<Button, UxmlTraits>
	{
	}

	[Obsolete("UxmlTraits is deprecated and will be removed. Use UxmlElementAttribute instead.", false)]
	public new class UxmlTraits : TextElement.UxmlTraits
	{
		private readonly UxmlImageAttributeDescription m_IconImage = new UxmlImageAttributeDescription
		{
			name = "icon-image"
		};

		public UxmlTraits()
		{
			base.focusable.defaultValue = true;
		}

		public override void Init(VisualElement ve, IUxmlAttributes bag, CreationContext cc)
		{
			base.Init(ve, bag, cc);
			Button button = (Button)ve;
			button.iconImage = m_IconImage.GetValueFromBag(bag, cc);
		}
	}

	internal static readonly BindingId iconImageProperty = "iconImage";

	public new static readonly string ussClassName = "unity-button";

	public static readonly string iconUssClassName = ussClassName + "--with-icon";

	public static readonly string iconOnlyUssClassName = ussClassName + "--with-icon-only";

	public static readonly string imageUSSClassName = ussClassName + "__image";

	private Clickable m_Clickable;

	private TextElement m_TextElement;

	private Image m_ImageElement;

	private Background m_IconImage;

	private string m_Text = string.Empty;

	private static readonly string NonEmptyString = " ";

	public Clickable clickable
	{
		get
		{
			return m_Clickable;
		}
		set
		{
			if (m_Clickable != null && m_Clickable.target == this)
			{
				this.RemoveManipulator(m_Clickable);
			}
			m_Clickable = value;
			if (m_Clickable != null)
			{
				this.AddManipulator(m_Clickable);
			}
		}
	}

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

	[CreateProperty]
	public Background iconImage
	{
		get
		{
			return m_IconImage;
		}
		set
		{
			if ((value.IsEmpty() && m_ImageElement == null) || value == m_IconImage)
			{
				return;
			}
			if (value.IsEmpty())
			{
				m_IconImage = value;
				ResetButtonHierarchy();
				NotifyPropertyChanged(in iconImageProperty);
				return;
			}
			if (m_ImageElement == null)
			{
				UpdateButtonHierarchy();
			}
			if ((bool)value.texture)
			{
				m_ImageElement.image = value.texture;
			}
			else if ((bool)value.sprite)
			{
				m_ImageElement.sprite = value.sprite;
			}
			else if ((bool)value.renderTexture)
			{
				m_ImageElement.image = value.renderTexture;
			}
			else
			{
				m_ImageElement.vectorImage = value.vectorImage;
			}
			m_IconImage = value;
			EnableInClassList(iconOnlyUssClassName, string.IsNullOrEmpty(text));
			NotifyPropertyChanged(in iconImageProperty);
		}
	}

	public override string text
	{
		get
		{
			return m_Text ?? string.Empty;
		}
		set
		{
			m_Text = value;
			EnableInClassList(iconOnlyUssClassName, !m_IconImage.IsEmpty() && string.IsNullOrEmpty(text));
			if (m_TextElement != null)
			{
				base.text = string.Empty;
				if (!(m_TextElement.text == m_Text))
				{
					m_TextElement.text = m_Text;
				}
			}
			else if (!(base.text == m_Text))
			{
				base.text = m_Text;
			}
		}
	}

	[Obsolete("onClick is obsolete. Use clicked instead (UnityUpgradable) -> clicked", true)]
	public event Action onClick
	{
		add
		{
			clicked += value;
		}
		remove
		{
			clicked -= value;
		}
	}

	public event Action clicked
	{
		add
		{
			if (m_Clickable == null)
			{
				clickable = new Clickable(value);
			}
			else
			{
				m_Clickable.clicked += value;
			}
		}
		remove
		{
			if (m_Clickable != null)
			{
				m_Clickable.clicked -= value;
			}
		}
	}

	public Button()
		: this(default(Background))
	{
	}

	public Button(Background iconImage, Action clickEvent = null)
		: this(clickEvent)
	{
		this.iconImage = iconImage;
	}

	public Button(Action clickEvent)
	{
		AddToClassList(ussClassName);
		clickable = new Clickable(clickEvent);
		focusable = true;
		base.tabIndex = 0;
		RegisterCallback<NavigationSubmitEvent>(OnNavigationSubmit);
	}

	private void OnNavigationSubmit(NavigationSubmitEvent evt)
	{
		clickable?.SimulateSingleClick(evt);
		evt.StopPropagation();
	}

	protected internal override Vector2 DoMeasure(float desiredWidth, MeasureMode widthMode, float desiredHeight, MeasureMode heightMode)
	{
		string nonEmptyString = text;
		if (string.IsNullOrEmpty(nonEmptyString))
		{
			nonEmptyString = NonEmptyString;
		}
		return MeasureTextSize(nonEmptyString, desiredWidth, widthMode, desiredHeight, heightMode);
	}

	private void UpdateButtonHierarchy()
	{
		if (m_ImageElement == null)
		{
			m_ImageElement = new Image
			{
				classList = { imageUSSClassName }
			};
			Add(m_ImageElement);
			AddToClassList(iconUssClassName);
		}
		if (m_TextElement == null)
		{
			m_TextElement = new TextElement
			{
				text = text
			};
			m_Text = text;
			base.text = string.Empty;
			Add(m_TextElement);
		}
	}

	private void ResetButtonHierarchy()
	{
		if (m_ImageElement != null)
		{
			m_ImageElement.RemoveFromHierarchy();
			m_ImageElement = null;
			RemoveFromClassList(iconUssClassName);
			RemoveFromClassList(iconOnlyUssClassName);
		}
		if (m_TextElement != null)
		{
			string text = m_TextElement.text;
			m_TextElement.RemoveFromHierarchy();
			m_TextElement = null;
			this.text = text;
		}
	}
}
