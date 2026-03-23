using System;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine.Internal;

namespace UnityEngine.UIElements;

public class DropdownField : PopupField<string>
{
	[Serializable]
	[ExcludeFromDocs]
	public new class UxmlSerializedData : BaseField<string>.UxmlSerializedData
	{
		[Delayed]
		[SerializeField]
		private int index;

		[SerializeField]
		[HideInInspector]
		[UxmlIgnore]
		private UxmlAttributeFlags index_UxmlAttributeFlags;

		[SerializeField]
		private List<string> choices;

		[SerializeField]
		[UxmlIgnore]
		[HideInInspector]
		private UxmlAttributeFlags choices_UxmlAttributeFlags;

		[SerializeField]
		[HideInInspector]
		[UxmlAttribute("value")]
		private int valueOverride;

		[SerializeField]
		[HideInInspector]
		[UxmlIgnore]
		private UxmlAttributeFlags valueOverride_UxmlAttributeFlags;

		[Conditional("UNITY_EDITOR")]
		public new static void Register()
		{
			BaseField<string>.UxmlSerializedData.Register();
			UxmlDescriptionCache.RegisterType(typeof(UxmlSerializedData), new UxmlAttributeNames[3]
			{
				new UxmlAttributeNames("index", "index", null),
				new UxmlAttributeNames("choices", "choices", null),
				new UxmlAttributeNames("valueOverride", "value", null)
			});
		}

		public override object CreateInstance()
		{
			return new DropdownField();
		}

		public override void Deserialize(object obj)
		{
			base.Deserialize(obj);
			DropdownField dropdownField = (DropdownField)obj;
			if (UnityEngine.UIElements.UxmlSerializedData.ShouldWriteAttributeValue(choices_UxmlAttributeFlags) && choices != null)
			{
				dropdownField.choices = new List<string>(choices);
			}
			if (UnityEngine.UIElements.UxmlSerializedData.ShouldWriteAttributeValue(index_UxmlAttributeFlags) && index != -1)
			{
				dropdownField.index = index;
			}
			if (UnityEngine.UIElements.UxmlSerializedData.ShouldWriteAttributeValue(valueOverride_UxmlAttributeFlags))
			{
				dropdownField.valueOverride = valueOverride;
			}
		}
	}

	[Obsolete("UxmlFactory is deprecated and will be removed. Use UxmlElementAttribute instead.", false)]
	public new class UxmlFactory : UxmlFactory<DropdownField, UxmlTraits>
	{
	}

	[Obsolete("UxmlTraits is deprecated and will be removed. Use UxmlElementAttribute instead.", false)]
	public new class UxmlTraits : BaseField<string>.UxmlTraits
	{
		private UxmlIntAttributeDescription m_Index = new UxmlIntAttributeDescription
		{
			name = "index"
		};

		private UxmlStringAttributeDescription m_Choices = new UxmlStringAttributeDescription
		{
			name = "choices"
		};

		public override void Init(VisualElement ve, IUxmlAttributes bag, CreationContext cc)
		{
			base.Init(ve, bag, cc);
			DropdownField dropdownField = (DropdownField)ve;
			List<string> list = UxmlUtility.ParseStringListAttribute(m_Choices.GetValueFromBag(bag, cc));
			if (list != null)
			{
				dropdownField.choices = list;
			}
			dropdownField.index = m_Index.GetValueFromBag(bag, cc);
		}
	}

	internal int valueOverride { get; set; }

	public DropdownField()
		: this(null)
	{
	}

	public DropdownField(string label)
		: base(label)
	{
	}

	public DropdownField(List<string> choices, string defaultValue, Func<string, string> formatSelectedValueCallback = null, Func<string, string> formatListItemCallback = null)
		: this(null, choices, defaultValue, formatSelectedValueCallback, formatListItemCallback)
	{
	}

	public DropdownField(string label, List<string> choices, string defaultValue, Func<string, string> formatSelectedValueCallback = null, Func<string, string> formatListItemCallback = null)
		: base(label, choices, defaultValue, formatSelectedValueCallback, formatListItemCallback)
	{
	}

	public DropdownField(List<string> choices, int defaultIndex, Func<string, string> formatSelectedValueCallback = null, Func<string, string> formatListItemCallback = null)
		: this(null, choices, defaultIndex, formatSelectedValueCallback, formatListItemCallback)
	{
	}

	public DropdownField(string label, List<string> choices, int defaultIndex, Func<string, string> formatSelectedValueCallback = null, Func<string, string> formatListItemCallback = null)
		: base(label, choices, defaultIndex, formatSelectedValueCallback, formatListItemCallback)
	{
	}
}
