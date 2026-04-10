using System;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine.Bindings;
using UnityEngine.Internal;

namespace UnityEngine.UIElements;

[VisibleToOtherModules(new string[] { "UnityEditor.UIBuilderModule" })]
internal class EnumToggleField<T> : BaseField<T> where T : struct, Enum, IConvertible
{
	[Serializable]
	[ExcludeFromDocs]
	public new class UxmlSerializedData : BaseField<T>.UxmlSerializedData
	{
		[RegisterUxmlCache]
		[Conditional("UNITY_EDITOR")]
		public new static void Register()
		{
			BaseField<T>.UxmlSerializedData.Register();
		}
	}

	private static readonly Dictionary<string, string> k_SpecialEnumNamesCases = new Dictionary<string, string>
	{
		{ "nowrap", "no-wrap" },
		{ "tabindex", "tab-index" }
	};

	private ToggleButtonGroup m_ToggleButtonGroup;

	public ToggleButtonGroup toggleButtonGroup => m_ToggleButtonGroup;

	public EnumToggleField()
		: this((string)null, useIcon: false)
	{
	}

	public EnumToggleField(bool useIcon = false)
		: this((string)null, useIcon)
	{
	}

	public EnumToggleField(string label, bool useIcon = false)
		: base(label, (VisualElement)null)
	{
		m_ToggleButtonGroup = new ToggleButtonGroup();
		m_ToggleButtonGroup.AddToClassList(BaseField<T>.alignedFieldUssClassName);
		Type enumType = typeof(T);
		string valueOrDefault = k_SpecialEnumNamesCases.GetValueOrDefault(enumType.Name, enumType.Name.ToKebabCase());
		m_ToggleButtonGroup.AddToClassList(ToggleButtonGroup.ussClassName + "_" + valueOrDefault + "-field");
		foreach (Enum value in Enum.GetValues(enumType))
		{
			string enumName = value.ToString();
			string text = StyleValueKeyword.Auto.ToString();
			Button button = new Button();
			if (enumName == text)
			{
				button.name = "auto";
				button.text = text.ToUpperInvariant();
			}
			else
			{
				valueOrDefault = k_SpecialEnumNamesCases.GetValueOrDefault(enumName, enumName.ToKebabCase());
				button.name = valueOrDefault;
				if (useIcon)
				{
					button.iconImage = Background.FromTexture2D(new Texture2D(0, 0));
				}
				else
				{
					button.text = enumName;
				}
			}
			button.clicked += delegate
			{
				this.value = (T)Enum.Parse(enumType, enumName, ignoreCase: true);
			};
			m_ToggleButtonGroup.Add(button);
		}
		m_ToggleButtonGroup.userData = enumType;
		base.visualInput.Add(m_ToggleButtonGroup);
	}

	public void SetIconForEnumValue(T enumValue, Texture2D icon)
	{
		int num = Array.IndexOf(Enum.GetValues(typeof(T)), enumValue);
		if (num >= 0)
		{
			m_ToggleButtonGroup.GetButton(num).iconImage = Background.FromTexture2D(icon);
		}
	}

	public void SetTextForEnumValue(T enumValue, string text)
	{
		int num = Array.IndexOf(Enum.GetValues(typeof(T)), enumValue);
		if (num >= 0)
		{
			m_ToggleButtonGroup.GetButton(num).text = text;
		}
	}

	public override void SetValueWithoutNotify(T newValue)
	{
		base.SetValueWithoutNotify(newValue);
		ToggleButtonGroupState toggleButtonGroupState = m_ToggleButtonGroup.value;
		toggleButtonGroupState.ResetAllOptions();
		if (m_ToggleButtonGroup.userData is Type enumType)
		{
			int index = Array.IndexOf(Enum.GetValues(enumType), newValue);
			toggleButtonGroupState[index] = true;
		}
		m_ToggleButtonGroup.value = toggleButtonGroupState;
	}
}
