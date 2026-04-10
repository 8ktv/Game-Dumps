using System;
using System.Diagnostics;
using Unity.Properties;
using UnityEngine.Bindings;
using UnityEngine.Internal;
using UnityEngine.Scripting.APIUpdating;

namespace UnityEngine.UIElements;

[MovedFrom(true, "UnityEditor.UIElements", "UnityEditor.UIElementsModule", null)]
public class EnumField : BaseField<Enum>
{
	[Serializable]
	[ExcludeFromDocs]
	public new class UxmlSerializedData : BaseField<Enum>.UxmlSerializedData
	{
		[UxmlAttribute("type")]
		[SerializeField]
		[UxmlTypeReference(typeof(Enum))]
		private string typeAsString;

		[UxmlAttribute("value")]
		[SerializeField]
		[EnumFieldValueDecorator]
		private string valueAsString;

		[SerializeField]
		private bool includeObsoleteValues;

		[UxmlIgnore]
		[SerializeField]
		[HideInInspector]
		private UxmlAttributeFlags typeAsString_UxmlAttributeFlags;

		[HideInInspector]
		[SerializeField]
		[UxmlIgnore]
		private UxmlAttributeFlags valueAsString_UxmlAttributeFlags;

		[HideInInspector]
		[UxmlIgnore]
		[SerializeField]
		private UxmlAttributeFlags includeObsoleteValues_UxmlAttributeFlags;

		[Conditional("UNITY_EDITOR")]
		public new static void Register()
		{
			BaseField<Enum>.UxmlSerializedData.Register();
			UxmlDescriptionCache.RegisterType(typeof(UxmlSerializedData), new UxmlAttributeNames[3]
			{
				new UxmlAttributeNames("typeAsString", "type", typeof(Enum)),
				new UxmlAttributeNames("valueAsString", "value", null),
				new UxmlAttributeNames("includeObsoleteValues", "include-obsolete-values", null)
			});
		}

		public override object CreateInstance()
		{
			return new EnumField();
		}

		public override void Deserialize(object obj)
		{
			base.Deserialize(obj);
			EnumField enumField = (EnumField)obj;
			if (UnityEngine.UIElements.UxmlSerializedData.ShouldWriteAttributeValue(includeObsoleteValues_UxmlAttributeFlags))
			{
				enumField.includeObsoleteValues = includeObsoleteValues;
			}
			if (UnityEngine.UIElements.UxmlSerializedData.ShouldWriteAttributeValue(typeAsString_UxmlAttributeFlags))
			{
				enumField.typeAsString = typeAsString;
			}
			if (UnityEngine.UIElements.UxmlSerializedData.ShouldWriteAttributeValue(valueAsString_UxmlAttributeFlags))
			{
				enumField.valueAsString = valueAsString;
			}
			else
			{
				enumField.valueAsString = null;
			}
		}
	}

	[Obsolete("UxmlFactory is deprecated and will be removed. Use UxmlElementAttribute instead.", false)]
	public new class UxmlFactory : UxmlFactory<EnumField, UxmlTraits>
	{
	}

	[Obsolete("UxmlTraits is deprecated and will be removed. Use UxmlElementAttribute instead.", false)]
	public new class UxmlTraits : BaseField<Enum>.UxmlTraits
	{
		private UxmlTypeAttributeDescription<Enum> m_Type = EnumFieldHelpers.type;

		private UxmlStringAttributeDescription m_Value = EnumFieldHelpers.value;

		private UxmlBoolAttributeDescription m_IncludeObsoleteValues = EnumFieldHelpers.includeObsoleteValues;

		public override void Init(VisualElement ve, IUxmlAttributes bag, CreationContext cc)
		{
			base.Init(ve, bag, cc);
			if (EnumFieldHelpers.ExtractValue(bag, cc, out var resEnumType, out var resEnumValue, out var resIncludeObsoleteValues))
			{
				EnumField enumField = (EnumField)ve;
				enumField.Init(resEnumValue, resIncludeObsoleteValues);
			}
			else if (null != resEnumType)
			{
				EnumField enumField2 = (EnumField)ve;
				enumField2.m_EnumType = resEnumType;
				if (enumField2.m_EnumType != null)
				{
					enumField2.PopulateDataFromType(enumField2.m_EnumType);
				}
				enumField2.value = null;
			}
			else
			{
				EnumField enumField3 = (EnumField)ve;
				enumField3.m_EnumType = null;
				enumField3.value = null;
			}
		}
	}

	internal static readonly BindingId textProperty = "text";

	private Type m_EnumType;

	private bool m_IncludeObsoleteValues;

	private TextElement m_TextElement;

	private VisualElement m_ArrowElement;

	private EnumData m_EnumData;

	internal Func<AbstractGenericMenu> createMenuCallback;

	public new static readonly string ussClassName = "unity-enum-field";

	public static readonly string textUssClassName = ussClassName + "__text";

	public static readonly string arrowUssClassName = ussClassName + "__arrow";

	public new static readonly string labelUssClassName = ussClassName + "__label";

	public new static readonly string inputUssClassName = ussClassName + "__input";

	internal Type type
	{
		[VisibleToOtherModules(new string[] { "UnityEditor.UIBuilderModule" })]
		get
		{
			return m_EnumType;
		}
	}

	[VisibleToOtherModules(new string[] { "UnityEditor.UIBuilderModule" })]
	internal bool includeObsoleteValues
	{
		get
		{
			return m_IncludeObsoleteValues;
		}
		set
		{
			m_IncludeObsoleteValues = value;
		}
	}

	internal string typeAsString
	{
		get
		{
			return UxmlUtility.TypeToString(m_EnumType);
		}
		[VisibleToOtherModules(new string[] { "UnityEditor.UIBuilderModule" })]
		set
		{
			m_EnumType = UxmlUtility.ParseType(value);
			if (m_EnumType == null)
			{
				this.value = null;
				m_TextElement.text = string.Empty;
			}
		}
	}

	[VisibleToOtherModules(new string[] { "UnityEditor.UIBuilderModule" })]
	internal string valueAsString
	{
		get
		{
			return value?.ToString();
		}
		set
		{
			if (type != null)
			{
				if (!string.IsNullOrEmpty(value))
				{
					if (Enum.TryParse(type, value, ignoreCase: false, out var result) && result is Enum defaultValue)
					{
						Init(defaultValue, includeObsoleteValues);
						return;
					}
					PopulateDataFromType(type);
					this.value = null;
				}
				else
				{
					Enum defaultValue2 = (Enum)Enum.ToObject(type, 0);
					Init(defaultValue2, includeObsoleteValues);
				}
			}
			else
			{
				this.value = null;
			}
		}
	}

	[CreateProperty(ReadOnly = true)]
	public string text => m_TextElement.text;

	private void Initialize(Enum defaultValue)
	{
		m_TextElement = new TextElement();
		m_TextElement.AddToClassList(textUssClassName);
		m_TextElement.pickingMode = PickingMode.Ignore;
		base.visualInput.Add(m_TextElement);
		m_ArrowElement = new VisualElement();
		m_ArrowElement.AddToClassList(arrowUssClassName);
		m_ArrowElement.pickingMode = PickingMode.Ignore;
		base.visualInput.Add(m_ArrowElement);
		if (defaultValue != null)
		{
			Init(defaultValue);
		}
	}

	public EnumField()
		: this(null, null)
	{
	}

	public EnumField(Enum defaultValue)
		: this(null, defaultValue)
	{
	}

	public EnumField(string label, Enum defaultValue = null)
		: base(label, (VisualElement)null)
	{
		AddToClassList(ussClassName);
		base.labelElement.AddToClassList(labelUssClassName);
		base.visualInput.AddToClassList(inputUssClassName);
		Initialize(defaultValue);
		RegisterCallback<PointerDownEvent>(OnPointerDownEvent);
		RegisterCallback<PointerMoveEvent>(OnPointerMoveEvent);
		RegisterCallback(delegate(MouseDownEvent e)
		{
			if (e.button == 0)
			{
				e.StopPropagation();
			}
		});
		RegisterCallback<NavigationSubmitEvent>(OnNavigationSubmit);
	}

	public void Init(Enum defaultValue)
	{
		Init(defaultValue, includeObsoleteValues: false);
	}

	public void Init(Enum defaultValue, bool includeObsoleteValues)
	{
		if (defaultValue == null)
		{
			throw new ArgumentNullException("defaultValue");
		}
		m_IncludeObsoleteValues = includeObsoleteValues;
		PopulateDataFromType(defaultValue.GetType());
		if (!object.Equals(base.rawValue, defaultValue))
		{
			SetValueWithoutNotify(defaultValue);
		}
		else
		{
			UpdateValueLabel(defaultValue);
		}
	}

	[VisibleToOtherModules(new string[] { "UnityEditor.UIBuilderModule" })]
	internal void PopulateDataFromType(Type enumType)
	{
		m_EnumType = enumType;
		m_EnumData = EnumDataUtility.GetCachedEnumData(m_EnumType, includeObsoleteValues ? EnumDataUtility.CachedType.IncludeObsoleteExceptErrors : EnumDataUtility.CachedType.ExcludeObsolete, NameFormatter.FormatVariableName);
	}

	public override void SetValueWithoutNotify(Enum newValue)
	{
		if (!object.Equals(base.rawValue, newValue))
		{
			base.SetValueWithoutNotify(newValue);
			if (!(m_EnumType == null))
			{
				UpdateValueLabel(newValue);
			}
		}
	}

	private void UpdateValueLabel(Enum value)
	{
		int num = Array.IndexOf(m_EnumData.values, value);
		if ((num >= 0) & (num < m_EnumData.values.Length))
		{
			m_TextElement.text = m_EnumData.displayNames[num];
		}
		else
		{
			m_TextElement.text = string.Empty;
		}
	}

	private void OnPointerDownEvent(PointerDownEvent evt)
	{
		ProcessPointerDown(evt);
	}

	private void OnPointerMoveEvent(PointerMoveEvent evt)
	{
		if (evt.button == 0 && (evt.pressedButtons & 1) != 0)
		{
			ProcessPointerDown(evt);
		}
	}

	private bool ContainsPointer(int pointerId)
	{
		VisualElement topElementUnderPointer = base.elementPanel.GetTopElementUnderPointer(pointerId);
		return this == topElementUnderPointer || base.visualInput == topElementUnderPointer;
	}

	private void ProcessPointerDown<T>(PointerEventBase<T> evt) where T : PointerEventBase<T>, new()
	{
		if (evt.button == 0 && ContainsPointer(evt.pointerId))
		{
			base.schedule.Execute(ShowMenu);
			evt.StopPropagation();
		}
	}

	private void OnNavigationSubmit(NavigationSubmitEvent evt)
	{
		ShowMenu();
		evt.StopPropagation();
	}

	internal void ShowMenu()
	{
		if (m_EnumType == null)
		{
			return;
		}
		AbstractGenericMenu abstractGenericMenu = ((createMenuCallback != null) ? createMenuCallback() : base.elementPanel.CreateMenu());
		int num = Array.IndexOf(m_EnumData.values, value);
		for (int i = 0; i < m_EnumData.values.Length; i++)
		{
			bool isChecked = num == i;
			abstractGenericMenu.AddItem(m_EnumData.displayNames[i], isChecked, delegate(object contentView)
			{
				ChangeValueFromMenu(contentView);
			}, m_EnumData.values[i]);
		}
		abstractGenericMenu.DropDown(base.visualInput.worldBound, this, DropdownMenuSizeMode.Fixed);
	}

	private void ChangeValueFromMenu(object menuItem)
	{
		value = menuItem as Enum;
	}

	protected override void UpdateMixedValueContent()
	{
		if (base.showMixedValue)
		{
			m_TextElement.text = BaseField<Enum>.mixedValueString;
		}
		else
		{
			UpdateValueLabel(value);
		}
		m_TextElement.EnableInClassList(labelUssClassName, base.showMixedValue);
		m_TextElement.EnableInClassList(BaseField<Enum>.mixedValueLabelUssClassName, base.showMixedValue);
	}
}
