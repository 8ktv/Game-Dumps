using System;
using System.Collections.Generic;
using Unity.Properties;
using UnityEngine.Bindings;
using UnityEngine.Internal;
using UnityEngine.Scripting.APIUpdating;

namespace UnityEngine.UIElements;

[MovedFrom(true, "UnityEditor.UIElements", "UnityEditor.UIElementsModule", null)]
public abstract class TextValueField<TValueType> : TextInputBaseField<TValueType>, IValueField<TValueType>
{
	[Serializable]
	[ExcludeFromDocs]
	public new abstract class UxmlSerializedData : TextInputBaseField<TValueType>.UxmlSerializedData
	{
		[SerializeField]
		[Tooltip("Indicates whether the field supports expressions that can be evaluated into a value.")]
		private bool supportExpressions;

		[UxmlIgnore]
		[HideInInspector]
		[SerializeField]
		private UxmlAttributeFlags supportExpressions_UxmlAttributeFlags;

		public new static void Register()
		{
			TextInputBaseField<TValueType>.UxmlSerializedData.Register();
			UxmlDescriptionCache.RegisterType(typeof(UxmlSerializedData), new UxmlAttributeNames[1]
			{
				new UxmlAttributeNames("supportExpressions", "support-expressions", null)
			});
		}

		public override void Deserialize(object obj)
		{
			base.Deserialize(obj);
			TextValueField<TValueType> textValueField = (TextValueField<TValueType>)obj;
			if (UnityEngine.UIElements.UxmlSerializedData.ShouldWriteAttributeValue(supportExpressions_UxmlAttributeFlags))
			{
				textValueField.supportExpressions = supportExpressions;
			}
		}
	}

	protected abstract class TextValueInput : TextInputBase
	{
		private TextValueField<TValueType> textValueFieldParent => (TextValueField<TValueType>)base.parent;

		protected abstract string allowedCharacters { get; }

		public string formatString { get; set; }

		protected TextValueInput()
		{
			base.textEdition.AcceptCharacter = AcceptCharacter;
		}

		internal override bool AcceptCharacter(char c)
		{
			return base.AcceptCharacter(c) && c != 0 && allowedCharacters.IndexOf(c) != -1;
		}

		public abstract void ApplyInputDeviceDelta(Vector3 delta, DeltaSpeed speed, TValueType startValue);

		public void StartDragging()
		{
			base.isDragging = true;
			base.textSelection.SelectNone();
			MarkDirtyRepaint();
		}

		public void StopDragging()
		{
			if (textValueFieldParent.isDelayed)
			{
				UpdateValueFromText();
			}
			base.isDragging = false;
			base.textSelection.SelectAll();
			MarkDirtyRepaint();
		}

		protected abstract string ValueToString(TValueType value);

		protected override TValueType StringToValue(string str)
		{
			return base.StringToValue(str);
		}
	}

	internal static readonly BindingId formatStringProperty = "formatString";

	internal static readonly BindingId supportExpressionsProperty = "supportExpressions";

	private BaseFieldMouseDragger m_Dragger;

	private bool m_ForceUpdateDisplay;

	private bool m_SupportExpressions = true;

	internal const int kMaxValueFieldLength = 1000;

	private TextValueInput textValueInput => (TextValueInput)base.textInputBase;

	internal bool forceUpdateDisplay
	{
		set
		{
			m_ForceUpdateDisplay = value;
		}
	}

	[CreateProperty]
	public bool supportExpressions
	{
		get
		{
			return m_SupportExpressions;
		}
		set
		{
			if (m_SupportExpressions != value)
			{
				m_SupportExpressions = value;
				NotifyPropertyChanged(in supportExpressionsProperty);
			}
		}
	}

	[CreateProperty]
	public string formatString
	{
		get
		{
			return textValueInput.formatString;
		}
		set
		{
			if (textValueInput.formatString != value)
			{
				textValueInput.formatString = value;
				base.textEdition.UpdateText(ValueToString(base.rawValue));
				NotifyPropertyChanged(in formatStringProperty);
			}
		}
	}

	protected TextValueField(int maxLength, TextValueInput textValueInput)
		: this((string)null, maxLength, textValueInput)
	{
	}

	protected TextValueField(string label, int maxLength, TextValueInput textValueInput)
		: base(label, maxLength, '\0', (TextInputBase)textValueInput)
	{
		base.textEdition.UpdateText(ValueToString(base.rawValue));
		base.onIsReadOnlyChanged = (Action<bool>)Delegate.Combine(base.onIsReadOnlyChanged, new Action<bool>(OnIsReadOnlyChanged));
	}

	public abstract void ApplyInputDeviceDelta(Vector3 delta, DeltaSpeed speed, TValueType startValue);

	public void StartDragging()
	{
		if (base.showMixedValue)
		{
			value = default(TValueType);
		}
		textValueInput.StartDragging();
	}

	public void StopDragging()
	{
		textValueInput.StopDragging();
	}

	internal override void UpdateValueFromText()
	{
		UpdatePlaceholderClassList();
		m_UpdateTextFromValue = false;
		try
		{
			value = StringToValue(base.text);
		}
		finally
		{
			m_UpdateTextFromValue = true;
		}
	}

	[VisibleToOtherModules(new string[] { "UnityEditor.UIBuilderModule" })]
	internal override void UpdateTextFromValue()
	{
		if (m_UpdateTextFromValue)
		{
			base.text = ValueToString(base.rawValue);
		}
	}

	private void OnIsReadOnlyChanged(bool newValue)
	{
		EnableLabelDragger(!newValue);
	}

	internal virtual bool CanTryParse(string textString)
	{
		return false;
	}

	protected void AddLabelDragger<TDraggerType>()
	{
		m_Dragger = new FieldMouseDragger<TDraggerType>((IValueField<TDraggerType>)this);
		EnableLabelDragger(!base.isReadOnly);
	}

	private void EnableLabelDragger(bool enable)
	{
		if (m_Dragger != null)
		{
			m_Dragger.SetDragZone(enable ? base.labelElement : null);
			base.labelElement.EnableInClassList(BaseField<TValueType>.labelDraggerVariantUssClassName, enable);
		}
	}

	public override void SetValueWithoutNotify(TValueType newValue)
	{
		bool flag = m_ForceUpdateDisplay || (m_UpdateTextFromValue && !EqualityComparer<TValueType>.Default.Equals(base.rawValue, newValue));
		base.SetValueWithoutNotify(newValue);
		if (flag)
		{
			base.textEdition.UpdateText(ValueToString(base.rawValue));
		}
		m_ForceUpdateDisplay = false;
	}

	public void ClearValue()
	{
		base.text = string.Empty;
		UpdateValueFromText();
	}

	[EventInterest(new Type[]
	{
		typeof(BlurEvent),
		typeof(FocusEvent)
	})]
	protected override void HandleEventBubbleUp(EventBase evt)
	{
		base.HandleEventBubbleUp(evt);
		if (string.IsNullOrEmpty(base.text) && !string.IsNullOrEmpty(base.textEdition.placeholder))
		{
			return;
		}
		if (evt.eventTypeId == EventBase<BlurEvent>.TypeId())
		{
			if (base.showMixedValue)
			{
				UpdateMixedValueContent();
				return;
			}
			if (string.IsNullOrEmpty(base.text))
			{
				base.textInputBase.UpdateTextFromValue();
				return;
			}
			base.textInputBase.UpdateValueFromText();
			base.textInputBase.UpdateTextFromValue();
		}
		else if (evt.eventTypeId == EventBase<FocusEvent>.TypeId() && base.showMixedValue && base.textInputBase.textElement.hasFocus)
		{
			base.textInputBase.text = "";
		}
	}

	internal override void OnViewDataReady()
	{
		m_ForceUpdateDisplay = true;
		base.OnViewDataReady();
	}

	internal override void RegisterEditingCallbacks()
	{
		base.RegisterEditingCallbacks();
		base.labelElement.RegisterCallback<PointerDownEvent>(base.StartEditing, TrickleDown.TrickleDown);
		base.labelElement.RegisterCallback<PointerUpEvent>(base.EndEditing);
	}

	internal override void UnregisterEditingCallbacks()
	{
		base.UnregisterEditingCallbacks();
		base.labelElement.UnregisterCallback<PointerDownEvent>(base.StartEditing, TrickleDown.TrickleDown);
		base.labelElement.UnregisterCallback<PointerUpEvent>(base.EndEditing);
	}
}
