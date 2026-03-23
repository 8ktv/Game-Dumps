using System;
using System.Diagnostics;
using UnityEngine.Internal;

namespace UnityEngine.UIElements;

public class Toggle : BaseBoolField
{
	[Serializable]
	[ExcludeFromDocs]
	public new class UxmlSerializedData : BaseBoolField.UxmlSerializedData
	{
		[SerializeField]
		[MultilineTextField]
		private string text;

		[SerializeField]
		[UxmlIgnore]
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
			return new Toggle();
		}

		public override void Deserialize(object obj)
		{
			base.Deserialize(obj);
			if (UnityEngine.UIElements.UxmlSerializedData.ShouldWriteAttributeValue(text_UxmlAttributeFlags))
			{
				Toggle toggle = (Toggle)obj;
				toggle.text = text;
			}
		}
	}

	[Obsolete("UxmlFactory is deprecated and will be removed. Use UxmlElementAttribute instead.", false)]
	public new class UxmlFactory : UxmlFactory<Toggle, UxmlTraits>
	{
	}

	[Obsolete("UxmlTraits is deprecated and will be removed. Use UxmlElementAttribute instead.", false)]
	public new class UxmlTraits : BaseFieldTraits<bool, UxmlBoolAttributeDescription>
	{
		private UxmlStringAttributeDescription m_Text = new UxmlStringAttributeDescription
		{
			name = "text"
		};

		public override void Init(VisualElement ve, IUxmlAttributes bag, CreationContext cc)
		{
			base.Init(ve, bag, cc);
			((Toggle)ve).text = m_Text.GetValueFromBag(bag, cc);
		}
	}

	public new static readonly string ussClassName = "unity-toggle";

	public new static readonly string labelUssClassName = ussClassName + "__label";

	public new static readonly string inputUssClassName = ussClassName + "__input";

	[Obsolete]
	public static readonly string noTextVariantUssClassName = ussClassName + "--no-text";

	public static readonly string checkmarkUssClassName = ussClassName + "__checkmark";

	public static readonly string textUssClassName = ussClassName + "__text";

	public static readonly string mixedValuesUssClassName = ussClassName + "__mixed-values";

	public Toggle()
		: this(null)
	{
	}

	public Toggle(string label)
		: base(label)
	{
		AddToClassList(ussClassName);
		base.visualInput.AddToClassList(inputUssClassName);
		base.labelElement.AddToClassList(labelUssClassName);
		m_CheckMark.AddToClassList(checkmarkUssClassName);
	}

	protected override void InitLabel()
	{
		base.InitLabel();
		m_Label.AddToClassList(textUssClassName);
	}

	protected override void UpdateMixedValueContent()
	{
		if (base.showMixedValue)
		{
			base.visualInput.pseudoStates &= ~PseudoStates.Checked;
			base.pseudoStates &= ~PseudoStates.Checked;
			m_CheckMark.AddToClassList(mixedValuesUssClassName);
			return;
		}
		m_CheckMark.RemoveFromClassList(mixedValuesUssClassName);
		if (value)
		{
			base.visualInput.pseudoStates |= PseudoStates.Checked;
			base.pseudoStates |= PseudoStates.Checked;
		}
		else
		{
			base.visualInput.pseudoStates &= ~PseudoStates.Checked;
			base.pseudoStates &= ~PseudoStates.Checked;
		}
	}
}
