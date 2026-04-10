using System;
using System.Collections.Generic;
using System.Diagnostics;
using Unity.Properties;
using UnityEngine.Internal;

namespace UnityEngine.UIElements;

public class TextField : TextInputBaseField<string>
{
	[Serializable]
	[ExcludeFromDocs]
	public new class UxmlSerializedData : TextInputBaseField<string>.UxmlSerializedData, IUxmlSerializedDataCustomAttributeHandler
	{
		[SerializeField]
		[MultilineDecorator]
		private bool multiline;

		[HideInInspector]
		[UxmlIgnore]
		[SerializeField]
		private UxmlAttributeFlags multiline_UxmlAttributeFlags;

		[Conditional("UNITY_EDITOR")]
		public new static void Register()
		{
			TextInputBaseField<string>.UxmlSerializedData.Register();
			UxmlDescriptionCache.RegisterType(typeof(UxmlSerializedData), new UxmlAttributeNames[1]
			{
				new UxmlAttributeNames("multiline", "multiline", null)
			});
		}

		public override object CreateInstance()
		{
			return new TextField();
		}

		public override void Deserialize(object obj)
		{
			base.Deserialize(obj);
			TextField textField = (TextField)obj;
			if (UnityEngine.UIElements.UxmlSerializedData.ShouldWriteAttributeValue(multiline_UxmlAttributeFlags))
			{
				textField.multiline = multiline;
			}
			if (UnityEngine.UIElements.UxmlSerializedData.ShouldWriteAttributeValue(verticalScrollerVisibility_UxmlAttributeFlags))
			{
				textField.verticalScrollerVisibility = verticalScrollerVisibility;
			}
		}

		void IUxmlSerializedDataCustomAttributeHandler.SerializeCustomAttributes(IUxmlAttributes bag, HashSet<string> handledAttributes)
		{
			if (bag.TryGetAttributeValue("text", out var text))
			{
				base.Value = text;
				handledAttributes.Add("value");
				if (bag is UxmlAsset uxmlAsset)
				{
					uxmlAsset.RemoveAttribute("text");
					uxmlAsset.SetAttribute("value", base.Value);
				}
			}
		}
	}

	[Obsolete("UxmlFactory is deprecated and will be removed. Use UxmlElementAttribute instead.", false)]
	public new class UxmlFactory : UxmlFactory<TextField, UxmlTraits>
	{
	}

	[Obsolete("UxmlTraits is deprecated and will be removed. Use UxmlElementAttribute instead.", false)]
	public new class UxmlTraits : TextInputBaseField<string>.UxmlTraits
	{
		private static readonly UxmlStringAttributeDescription k_Value = new UxmlStringAttributeDescription
		{
			name = "value",
			obsoleteNames = new string[1] { "text" }
		};

		private UxmlBoolAttributeDescription m_Multiline = new UxmlBoolAttributeDescription
		{
			name = "multiline"
		};

		public override void Init(VisualElement ve, IUxmlAttributes bag, CreationContext cc)
		{
			TextField textField = (TextField)ve;
			base.Init(ve, bag, cc);
			string value = string.Empty;
			if (k_Value.TryGetValueFromBag(bag, cc, ref value))
			{
				textField.SetValueWithoutNotify(value);
			}
			textField.multiline = m_Multiline.GetValueFromBag(bag, cc);
		}
	}

	private class TextInput : TextInputBase
	{
		private TextField parentTextField => (TextField)base.parent;

		public bool multiline
		{
			get
			{
				return base.textEdition.multiline;
			}
			set
			{
				if ((!value && !string.IsNullOrEmpty(base.text) && base.text.Contains("\n")) || base.textEdition.multiline != value)
				{
					base.textEdition.multiline = value;
					if (value)
					{
						base.text = parentTextField.rawValue;
						SetMultiline();
					}
					else
					{
						base.text = base.text.Replace("\n", "");
						SetSingleLine();
					}
				}
			}
		}

		[Obsolete("isPasswordField is deprecated. Use textEdition.isPassword instead.")]
		public override bool isPasswordField
		{
			set
			{
				base.textEdition.isPassword = value;
				if (value)
				{
					multiline = false;
				}
			}
		}

		protected override string StringToValue(string str)
		{
			return str;
		}
	}

	internal static readonly BindingId multilineProperty = "multiline";

	public new static readonly string ussClassName = "unity-text-field";

	public new static readonly string labelUssClassName = ussClassName + "__label";

	public new static readonly string inputUssClassName = ussClassName + "__input";

	private TextInput textInput => (TextInput)base.textInputBase;

	[CreateProperty]
	public bool multiline
	{
		get
		{
			return textInput.multiline;
		}
		set
		{
			bool flag = multiline;
			textInput.multiline = value;
			if (flag != multiline)
			{
				NotifyPropertyChanged(in multilineProperty);
			}
		}
	}

	public override string value
	{
		get
		{
			return base.value;
		}
		set
		{
			base.value = value;
			base.textEdition.UpdateText(base.rawValue);
		}
	}

	public TextField()
		: this(null)
	{
	}

	public TextField(int maxLength, bool multiline, bool isPasswordField, char maskChar)
		: this(null, maxLength, multiline, isPasswordField, maskChar)
	{
	}

	public TextField(string label)
		: this(label, -1, multiline: false, isPasswordField: false, '*')
	{
	}

	public TextField(string label, int maxLength, bool multiline, bool isPasswordField, char maskChar)
		: base(label, maxLength, maskChar, (TextInputBase)new TextInput())
	{
		AddToClassList(ussClassName);
		base.labelElement.AddToClassList(labelUssClassName);
		base.visualInput.AddToClassList(inputUssClassName);
		base.pickingMode = PickingMode.Ignore;
		SetValueWithoutNotify("");
		this.multiline = multiline;
		base.textEdition.isPassword = isPasswordField;
	}

	public override void SetValueWithoutNotify(string newValue)
	{
		base.SetValueWithoutNotify(newValue);
		string valueWithoutNotify = base.rawValue;
		if (!multiline && base.rawValue != null)
		{
			valueWithoutNotify = base.rawValue.Replace("\n", "");
		}
		((INotifyValueChanged<string>)textInput.textElement).SetValueWithoutNotify(valueWithoutNotify);
	}

	internal override void UpdateTextFromValue()
	{
		SetValueWithoutNotify(base.rawValue);
	}

	[EventInterest(new Type[] { typeof(FocusOutEvent) })]
	protected override void HandleEventBubbleUp(EventBase evt)
	{
		base.HandleEventBubbleUp(evt);
		if (base.isDelayed && evt?.eventTypeId == EventBase<FocusOutEvent>.TypeId())
		{
			DispatchMode dispatchMode = base.dispatchMode;
			try
			{
				base.dispatchMode = DispatchMode.Immediate;
				value = base.text;
			}
			finally
			{
				base.dispatchMode = dispatchMode;
			}
		}
	}

	internal override void OnViewDataReady()
	{
		base.OnViewDataReady();
		string fullHierarchicalViewDataKey = GetFullHierarchicalViewDataKey();
		OverwriteFromViewData(this, fullHierarchicalViewDataKey);
		base.text = base.rawValue;
	}

	protected override string ValueToString(string value)
	{
		return value;
	}

	protected override string StringToValue(string str)
	{
		return str;
	}
}
