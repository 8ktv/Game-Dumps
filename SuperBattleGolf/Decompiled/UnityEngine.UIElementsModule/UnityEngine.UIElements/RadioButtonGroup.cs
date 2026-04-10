using System;
using System.Collections.Generic;
using System.Diagnostics;
using Unity.Properties;
using UnityEngine.Internal;
using UnityEngine.Pool;

namespace UnityEngine.UIElements;

public class RadioButtonGroup : BaseField<int>, IGroupBox
{
	[Serializable]
	[ExcludeFromDocs]
	public new class UxmlSerializedData : BaseField<int>.UxmlSerializedData
	{
		[UxmlAttributeBindingPath("choices")]
		[SerializeField]
		[UxmlAttribute("choices")]
		private List<string> choicesList;

		[HideInInspector]
		[UxmlIgnore]
		[SerializeField]
		private UxmlAttributeFlags choicesList_UxmlAttributeFlags;

		[Conditional("UNITY_EDITOR")]
		public new static void Register()
		{
			BaseField<int>.UxmlSerializedData.Register();
			UxmlDescriptionCache.RegisterType(typeof(UxmlSerializedData), new UxmlAttributeNames[1]
			{
				new UxmlAttributeNames("choicesList", "choices", null)
			});
		}

		public override object CreateInstance()
		{
			return new RadioButtonGroup();
		}

		public override void Deserialize(object obj)
		{
			base.Deserialize(obj);
			if (UnityEngine.UIElements.UxmlSerializedData.ShouldWriteAttributeValue(choicesList_UxmlAttributeFlags))
			{
				RadioButtonGroup radioButtonGroup = (RadioButtonGroup)obj;
				radioButtonGroup.choicesList = choicesList;
			}
		}
	}

	[Obsolete("UxmlFactory is deprecated and will be removed. Use UxmlElementAttribute instead.", false)]
	public new class UxmlFactory : UxmlFactory<RadioButtonGroup, UxmlTraits>
	{
	}

	[Obsolete("UxmlTraits is deprecated and will be removed. Use UxmlElementAttribute instead.", false)]
	public new class UxmlTraits : BaseFieldTraits<int, UxmlIntAttributeDescription>
	{
		private UxmlStringAttributeDescription m_Choices = new UxmlStringAttributeDescription
		{
			name = "choices"
		};

		public override void Init(VisualElement ve, IUxmlAttributes bag, CreationContext cc)
		{
			base.Init(ve, bag, cc);
			RadioButtonGroup radioButtonGroup = (RadioButtonGroup)ve;
			radioButtonGroup.choicesList = UxmlUtility.ParseStringListAttribute(m_Choices.GetValueFromBag(bag, cc));
		}
	}

	internal static readonly BindingId choicesProperty = "choices";

	public new static readonly string ussClassName = "unity-radio-button-group";

	public static readonly string containerUssClassName = ussClassName + "__container";

	internal static readonly string containerName = "contentContainer";

	internal static readonly string choicesContainerName = "choicesContentContainer";

	private VisualElement m_ChoiceRadioButtonContainer;

	private VisualElement m_ContentContainer;

	private UQueryBuilder<RadioButton> m_GetAllRadioButtonsQuery;

	private readonly List<RadioButton> m_RegisteredRadioButtons = new List<RadioButton>();

	private RadioButton m_SelectedRadioButton;

	private EventCallback<ChangeEvent<bool>> m_RadioButtonValueChangedCallback;

	private bool m_UpdatingButtons;

	private List<string> m_Choices = new List<string>();

	[CreateProperty]
	public IEnumerable<string> choices
	{
		get
		{
			List<RadioButton> radioButtons;
			using (CollectionPool<List<RadioButton>, RadioButton>.Get(out radioButtons))
			{
				GetAllRadioButtons(radioButtons);
				foreach (RadioButton button in radioButtons)
				{
					yield return button.text;
				}
			}
		}
		set
		{
			if ((value == null || !AreListEqual(m_Choices, value)) && (value != null || m_Choices.Count != 0))
			{
				m_Choices.Clear();
				if (value != null)
				{
					m_Choices.AddRange(value);
				}
				RebuildRadioButtonsFromChoices();
				NotifyPropertyChanged(in choicesProperty);
			}
			static bool AreListEqual(List<string> list1, IEnumerable<string> list2)
			{
				int num = 0;
				using (IEnumerator<string> enumerator = list2.GetEnumerator())
				{
					while (enumerator.MoveNext())
					{
						num++;
					}
				}
				if (list1.Count != num)
				{
					return false;
				}
				int num2 = 0;
				using (IEnumerator<string> enumerator2 = list2.GetEnumerator())
				{
					while (enumerator2.MoveNext())
					{
						if (!string.Equals(list1[num2], enumerator2.Current))
						{
							return false;
						}
						num2++;
					}
				}
				return true;
			}
		}
	}

	internal List<string> choicesList
	{
		get
		{
			return m_Choices;
		}
		set
		{
			choices = value;
		}
	}

	public override VisualElement contentContainer => m_ContentContainer ?? this;

	private void RebuildRadioButtonsFromChoices()
	{
		if (m_Choices.Count == 0)
		{
			m_ChoiceRadioButtonContainer.Clear();
			return;
		}
		int num = 0;
		foreach (string choice in m_Choices)
		{
			if (num < m_ChoiceRadioButtonContainer.childCount)
			{
				(m_ChoiceRadioButtonContainer[num] as RadioButton).text = choice;
				ScheduleRadioButtons();
			}
			else
			{
				RadioButton child = new RadioButton
				{
					text = choice
				};
				m_ChoiceRadioButtonContainer.Add(child);
			}
			num++;
		}
		int num2 = m_ChoiceRadioButtonContainer.childCount - 1;
		for (int num3 = num2; num3 >= num; num3--)
		{
			m_ChoiceRadioButtonContainer[num3].RemoveFromHierarchy();
		}
	}

	public RadioButtonGroup()
		: this(null)
	{
	}

	public RadioButtonGroup(string label, List<string> radioButtonChoices = null)
		: base(label, (VisualElement)null)
	{
		AddToClassList(ussClassName);
		VisualElement visualElement = base.visualInput;
		VisualElement obj = new VisualElement
		{
			name = choicesContainerName
		};
		VisualElement child = obj;
		m_ChoiceRadioButtonContainer = obj;
		visualElement.Add(child);
		m_ChoiceRadioButtonContainer.AddToClassList(containerUssClassName);
		VisualElement visualElement2 = base.visualInput;
		VisualElement obj2 = new VisualElement
		{
			name = containerName
		};
		child = obj2;
		m_ContentContainer = obj2;
		visualElement2.Add(child);
		m_ContentContainer.AddToClassList(containerUssClassName);
		m_GetAllRadioButtonsQuery = this.Query<RadioButton>();
		m_RadioButtonValueChangedCallback = RadioButtonValueChangedCallback;
		choices = radioButtonChoices;
		value = -1;
		base.visualInput.focusable = false;
		base.delegatesFocus = true;
	}

	private void RadioButtonValueChangedCallback(ChangeEvent<bool> evt)
	{
		if (evt.newValue)
		{
			RadioButton item = evt.target as RadioButton;
			List<RadioButton> list;
			using (CollectionPool<List<RadioButton>, RadioButton>.Get(out list))
			{
				GetAllRadioButtons(list);
				value = list.IndexOf(item);
				evt.StopPropagation();
			}
		}
	}

	public override void SetValueWithoutNotify(int newValue)
	{
		base.SetValueWithoutNotify(newValue);
		UpdateRadioButtons(notify: true);
	}

	private void GetAllRadioButtons(List<RadioButton> radioButtons)
	{
		radioButtons.Clear();
		m_GetAllRadioButtonsQuery.ForEach(radioButtons.Add);
	}

	private void UpdateRadioButtons(bool notify)
	{
		if (base.panel == null)
		{
			return;
		}
		List<RadioButton> list;
		using (CollectionPool<List<RadioButton>, RadioButton>.Get(out list))
		{
			GetAllRadioButtons(list);
			if (value >= 0 && value < list.Count)
			{
				m_SelectedRadioButton = list[value];
				if (notify)
				{
					m_SelectedRadioButton.value = true;
				}
				else
				{
					m_SelectedRadioButton.SetValueWithoutNotify(newValue: true);
				}
				foreach (RadioButton item in list)
				{
					if (item != m_SelectedRadioButton)
					{
						if (notify)
						{
							item.value = false;
						}
						else
						{
							item.SetValueWithoutNotify(newValue: false);
						}
					}
				}
			}
			else
			{
				foreach (RadioButton registeredRadioButton in m_RegisteredRadioButtons)
				{
					if (notify)
					{
						registeredRadioButton.value = false;
					}
					else
					{
						registeredRadioButton.SetValueWithoutNotify(newValue: false);
					}
				}
			}
			m_UpdatingButtons = false;
		}
	}

	private void ScheduleRadioButtons()
	{
		if (!m_UpdatingButtons)
		{
			base.schedule.Execute((Action)delegate
			{
				UpdateRadioButtons(notify: false);
			});
			m_UpdatingButtons = true;
		}
	}

	private void RegisterRadioButton(RadioButton radioButton)
	{
		if (m_RegisteredRadioButtons.Contains(radioButton))
		{
			return;
		}
		m_RegisteredRadioButtons.Add(radioButton);
		radioButton.RegisterValueChangedCallback(m_RadioButtonValueChangedCallback);
		if (value == -1 && radioButton.value)
		{
			List<RadioButton> list;
			using (CollectionPool<List<RadioButton>, RadioButton>.Get(out list))
			{
				GetAllRadioButtons(list);
				SetValueWithoutNotify(list.IndexOf(radioButton));
			}
		}
		ScheduleRadioButtons();
	}

	private void UnregisterRadioButton(RadioButton radioButton)
	{
		if (m_RegisteredRadioButtons.Contains(radioButton))
		{
			m_RegisteredRadioButtons.Remove(radioButton);
			radioButton.UnregisterValueChangedCallback(m_RadioButtonValueChangedCallback);
			UpdateRadioButtons(notify: false);
		}
	}

	void IGroupBox.OnOptionAdded(IGroupBoxOption option)
	{
		if (!(option is RadioButton radioButton))
		{
			throw new ArgumentException("[UI Toolkit] Internal group box error. Expected a radio button element. Please report this using Help -> Report a bug...");
		}
		RegisterRadioButton(radioButton);
	}

	void IGroupBox.OnOptionRemoved(IGroupBoxOption option)
	{
		if (!(option is RadioButton radioButton))
		{
			throw new ArgumentException("[UI Toolkit] Internal group box error. Expected a radio button element. Please report this using Help -> Report a bug...");
		}
		UnregisterRadioButton(radioButton);
		if (m_SelectedRadioButton == radioButton)
		{
			m_SelectedRadioButton = null;
			value = -1;
		}
	}
}
