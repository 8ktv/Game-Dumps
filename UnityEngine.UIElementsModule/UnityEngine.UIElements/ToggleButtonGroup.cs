using System;
using System.Collections.Generic;
using System.Diagnostics;
using Unity.Properties;
using UnityEngine.Internal;

namespace UnityEngine.UIElements;

public class ToggleButtonGroup : BaseField<ToggleButtonGroupState>
{
	[Serializable]
	[ExcludeFromDocs]
	public new class UxmlSerializedData : BaseField<ToggleButtonGroupState>.UxmlSerializedData
	{
		[SerializeField]
		private bool isMultipleSelection;

		[UxmlIgnore]
		[SerializeField]
		[HideInInspector]
		private UxmlAttributeFlags isMultipleSelection_UxmlAttributeFlags;

		[SerializeField]
		private bool allowEmptySelection;

		[HideInInspector]
		[SerializeField]
		[UxmlIgnore]
		private UxmlAttributeFlags allowEmptySelection_UxmlAttributeFlags;

		[Conditional("UNITY_EDITOR")]
		public new static void Register()
		{
			BaseField<ToggleButtonGroupState>.UxmlSerializedData.Register();
			UxmlDescriptionCache.RegisterType(typeof(UxmlSerializedData), new UxmlAttributeNames[2]
			{
				new UxmlAttributeNames("isMultipleSelection", "is-multiple-selection", null),
				new UxmlAttributeNames("allowEmptySelection", "allow-empty-selection", null)
			});
		}

		public override object CreateInstance()
		{
			return new ToggleButtonGroup();
		}

		public override void Deserialize(object obj)
		{
			base.Deserialize(obj);
			ToggleButtonGroup toggleButtonGroup = (ToggleButtonGroup)obj;
			if (UnityEngine.UIElements.UxmlSerializedData.ShouldWriteAttributeValue(isMultipleSelection_UxmlAttributeFlags))
			{
				toggleButtonGroup.isMultipleSelection = isMultipleSelection;
			}
			if (UnityEngine.UIElements.UxmlSerializedData.ShouldWriteAttributeValue(allowEmptySelection_UxmlAttributeFlags))
			{
				toggleButtonGroup.allowEmptySelection = allowEmptySelection;
			}
		}
	}

	[Obsolete("UxmlFactory is deprecated and will be removed. Use UxmlElementAttribute instead.", false)]
	public new class UxmlFactory : UxmlFactory<ToggleButtonGroup, UxmlTraits>
	{
	}

	[Obsolete("UxmlTraits is deprecated and will be removed. Use UxmlElementAttribute instead.", false)]
	public new class UxmlTraits : BaseField<ToggleButtonGroupState>.UxmlTraits
	{
		private UxmlBoolAttributeDescription m_IsMultipleSelection = new UxmlBoolAttributeDescription
		{
			name = "is-multiple-selection"
		};

		private UxmlBoolAttributeDescription m_AllowEmptySelection = new UxmlBoolAttributeDescription
		{
			name = "allow-empty-selection"
		};

		public override void Init(VisualElement ve, IUxmlAttributes bag, CreationContext cc)
		{
			base.Init(ve, bag, cc);
			ToggleButtonGroup toggleButtonGroup = (ToggleButtonGroup)ve;
			toggleButtonGroup.isMultipleSelection = m_IsMultipleSelection.GetValueFromBag(bag, cc);
			toggleButtonGroup.allowEmptySelection = m_AllowEmptySelection.GetValueFromBag(bag, cc);
		}
	}

	private static readonly string k_MaxToggleButtonGroupMessage = $"The number of buttons added to ToggleButtonGroup exceeds the maximum allowed ({64}). The newly added button will not be treated as part of this control.";

	internal static readonly BindingId isMultipleSelectionProperty = "isMultipleSelection";

	internal static readonly BindingId allowEmptySelectionProperty = "allowEmptySelection";

	public new static readonly string ussClassName = "unity-toggle-button-group";

	public static readonly string containerUssClassName = ussClassName + "__container";

	public static readonly string buttonGroupClassName = "unity-button-group";

	public static readonly string buttonClassName = buttonGroupClassName + "__button";

	public static readonly string buttonLeftClassName = buttonClassName + "--left";

	public static readonly string buttonMidClassName = buttonClassName + "--mid";

	public static readonly string buttonRightClassName = buttonClassName + "--right";

	public static readonly string buttonStandaloneClassName = buttonClassName + "--standalone";

	public static readonly string emptyStateLabelClassName = buttonGroupClassName + "__empty-label";

	private VisualElement m_ButtonGroupContainer;

	private List<Button> m_Buttons = new List<Button>();

	private VisualElement m_EmptyLabel;

	private const string k_EmptyStateLabel = "Group has no buttons.";

	private bool m_IsMultipleSelection;

	private bool m_AllowEmptySelection;

	[CreateProperty]
	public bool isMultipleSelection
	{
		get
		{
			return m_IsMultipleSelection;
		}
		set
		{
			if (m_IsMultipleSelection != value)
			{
				ToggleButtonGroupState valueWithoutNotify = this.value;
				Span<int> activeOptionsIndices = stackalloc int[valueWithoutNotify.length];
				Span<int> activeOptions = valueWithoutNotify.GetActiveOptions(activeOptionsIndices);
				if (activeOptions.Length > 1 && m_Buttons.Count > 0)
				{
					valueWithoutNotify.ResetAllOptions();
					valueWithoutNotify[activeOptions[0]] = true;
					SetValueWithoutNotify(valueWithoutNotify);
				}
				m_IsMultipleSelection = value;
				NotifyPropertyChanged(in isMultipleSelectionProperty);
			}
		}
	}

	[CreateProperty]
	public bool allowEmptySelection
	{
		get
		{
			return m_AllowEmptySelection;
		}
		set
		{
			if (m_AllowEmptySelection == value)
			{
				return;
			}
			if (!value)
			{
				ToggleButtonGroupState valueWithoutNotify = this.value;
				Span<int> activeOptionsIndices = stackalloc int[valueWithoutNotify.length];
				if (valueWithoutNotify.GetActiveOptions(activeOptionsIndices).Length == 0 && m_Buttons.Count > 0)
				{
					valueWithoutNotify[0] = true;
					SetValueWithoutNotify(valueWithoutNotify);
				}
			}
			m_AllowEmptySelection = value;
			NotifyPropertyChanged(in allowEmptySelectionProperty);
		}
	}

	public override VisualElement contentContainer => m_ButtonGroupContainer ?? this;

	public ToggleButtonGroup()
		: this(null)
	{
	}

	public ToggleButtonGroup(string label)
		: this(label, new ToggleButtonGroupState(0uL, 64))
	{
	}

	public ToggleButtonGroup(ToggleButtonGroupState toggleButtonGroupState)
		: this(null, toggleButtonGroupState)
	{
	}

	public ToggleButtonGroup(string label, ToggleButtonGroupState toggleButtonGroupState)
		: base(label)
	{
		AddToClassList(ussClassName);
		base.visualInput = new VisualElement
		{
			name = containerUssClassName,
			classList = { buttonGroupClassName },
			focusable = false
		};
		m_ButtonGroupContainer = base.visualInput;
		m_ButtonGroupContainer.elementAdded += OnButtonGroupContainerElementAdded;
		m_ButtonGroupContainer.elementRemoved += OnButtonGroupContainerElementRemoved;
		SetValueWithoutNotify(toggleButtonGroupState);
	}

	internal override void OnViewDataReady()
	{
		base.OnViewDataReady();
		UpdateButtonStates(value);
	}

	protected override void UpdateMixedValueContent()
	{
		if (base.showMixedValue)
		{
			foreach (Button button in m_Buttons)
			{
				button.pseudoStates &= ~PseudoStates.Checked;
				button.IncrementVersion(VersionChangeType.Styles);
			}
			return;
		}
		SetValueWithoutNotify(value);
	}

	public override void SetValueWithoutNotify(ToggleButtonGroupState newValue)
	{
		if (newValue.length == 0)
		{
			newValue = new ToggleButtonGroupState(0uL, 0);
			if (m_EmptyLabel == null)
			{
				m_EmptyLabel = new Label("Group has no buttons.")
				{
					name = emptyStateLabelClassName,
					classList = { emptyStateLabelClassName }
				};
			}
			base.visualInput.Insert(0, m_EmptyLabel);
		}
		else
		{
			m_EmptyLabel?.RemoveFromHierarchy();
		}
		base.SetValueWithoutNotify(newValue);
		UpdateButtonStates(newValue);
	}

	private void OnButtonGroupContainerElementAdded(VisualElement ve, int index)
	{
		if (!(ve is Button button))
		{
			if (ve != m_EmptyLabel)
			{
				base.hierarchy.Add(ve);
			}
			return;
		}
		if (m_Buttons.Count + 1 > 64)
		{
			Debug.LogWarning(k_MaxToggleButtonGroupMessage);
			return;
		}
		button.AddToClassList(buttonClassName);
		button.clickable.clickedWithEventInfo += OnOptionChange;
		m_Buttons = m_ButtonGroupContainer.Query<Button>().ToList();
		UpdateButtonsStyling();
		bool flag = false;
		ToggleButtonGroupState toggleButtonGroupState = value;
		if (m_Buttons.Count >= value.length && m_Buttons.Count <= 64)
		{
			toggleButtonGroupState.length = m_Buttons.Count;
			flag = true;
		}
		if (value.data == 0L && !allowEmptySelection)
		{
			toggleButtonGroupState[0] = true;
			flag = true;
		}
		if (flag)
		{
			value = toggleButtonGroupState;
		}
	}

	private void OnButtonGroupContainerElementRemoved(VisualElement ve)
	{
		if (!(ve is Button button))
		{
			return;
		}
		ToggleButtonGroupState valueWithoutNotify = value;
		int index = m_Buttons.IndexOf(button);
		Span<int> activeOptionsIndices = stackalloc int[valueWithoutNotify.length];
		Span<int> activeOptions = valueWithoutNotify.GetActiveOptions(activeOptionsIndices);
		bool flag = activeOptions.IndexOf(index) != -1;
		button.clickable.clickedWithEventInfo -= OnOptionChange;
		if (flag)
		{
			m_Buttons[index].pseudoStates &= ~PseudoStates.Checked;
		}
		m_Buttons.Remove(button);
		UpdateButtonsStyling();
		valueWithoutNotify.length = m_Buttons.Count;
		if (m_Buttons.Count == 0)
		{
			valueWithoutNotify.ResetAllOptions();
			SetValueWithoutNotify(valueWithoutNotify);
		}
		else if (flag)
		{
			valueWithoutNotify[index] = false;
			if (!allowEmptySelection && activeOptions.Length == 1)
			{
				valueWithoutNotify[0] = true;
			}
			value = valueWithoutNotify;
		}
	}

	private void UpdateButtonStates(ToggleButtonGroupState options)
	{
		Span<int> activeOptionsIndices = stackalloc int[value.length];
		Span<int> activeOptions = options.GetActiveOptions(activeOptionsIndices);
		for (int i = 0; i < m_Buttons.Count; i++)
		{
			if (activeOptions.IndexOf(i) == -1)
			{
				m_Buttons[i].pseudoStates &= ~PseudoStates.Checked;
				m_Buttons[i].IncrementVersion(VersionChangeType.Styles);
			}
			else
			{
				m_Buttons[i].pseudoStates |= PseudoStates.Checked;
				m_Buttons[i].IncrementVersion(VersionChangeType.Styles);
			}
		}
	}

	private void OnOptionChange(EventBase evt)
	{
		Button item = evt.target as Button;
		int num = m_Buttons.IndexOf(item);
		ToggleButtonGroupState toggleButtonGroupState = value;
		Span<int> activeOptionsIndices = stackalloc int[toggleButtonGroupState.length];
		Span<int> activeOptions = toggleButtonGroupState.GetActiveOptions(activeOptionsIndices);
		if (base.showMixedValue)
		{
			ToggleButtonGroupState toggleButtonGroupState2 = value;
			toggleButtonGroupState2.ResetAllOptions();
			if (value != toggleButtonGroupState2)
			{
				SetValueWithoutNotify(toggleButtonGroupState2);
			}
		}
		if (isMultipleSelection)
		{
			if (!allowEmptySelection && activeOptions.Length == 1 && toggleButtonGroupState[num])
			{
				return;
			}
			if (toggleButtonGroupState[num])
			{
				toggleButtonGroupState[num] = false;
			}
			else
			{
				toggleButtonGroupState[num] = true;
			}
		}
		else if (allowEmptySelection && activeOptions.Length == 1 && toggleButtonGroupState[activeOptions[0]])
		{
			toggleButtonGroupState[activeOptions[0]] = false;
			if (num != activeOptions[0])
			{
				toggleButtonGroupState[num] = true;
			}
		}
		else
		{
			toggleButtonGroupState.ResetAllOptions();
			toggleButtonGroupState[num] = true;
		}
		value = toggleButtonGroupState;
	}

	private void UpdateButtonsStyling()
	{
		int count = m_Buttons.Count;
		for (int i = 0; i < count; i++)
		{
			Button button = m_Buttons[i];
			bool flag = count == 1;
			bool flag2 = i == 0 && !flag;
			bool flag3 = i == count - 1 && !flag;
			bool enable = !flag2 && !flag3 && !flag;
			button.EnableInClassList(buttonStandaloneClassName, flag);
			button.EnableInClassList(buttonLeftClassName, flag2);
			button.EnableInClassList(buttonRightClassName, flag3);
			button.EnableInClassList(buttonMidClassName, enable);
		}
	}
}
