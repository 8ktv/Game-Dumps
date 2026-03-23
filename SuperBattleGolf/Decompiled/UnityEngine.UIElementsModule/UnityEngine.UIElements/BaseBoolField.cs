using System;
using System.Diagnostics;
using Unity.Properties;
using UnityEngine.Bindings;
using UnityEngine.Internal;

namespace UnityEngine.UIElements;

public abstract class BaseBoolField : BaseField<bool>
{
	[Serializable]
	[ExcludeFromDocs]
	public new abstract class UxmlSerializedData : BaseField<bool>.UxmlSerializedData
	{
		[SerializeField]
		private bool toggleOnLabelClick;

		[HideInInspector]
		[UxmlIgnore]
		[SerializeField]
		private UxmlAttributeFlags toggleOnLabelClick_UxmlAttributeFlags;

		[Conditional("UNITY_EDITOR")]
		public new static void Register()
		{
			BaseField<bool>.UxmlSerializedData.Register();
			UxmlDescriptionCache.RegisterType(typeof(UxmlSerializedData), new UxmlAttributeNames[1]
			{
				new UxmlAttributeNames("toggleOnLabelClick", "toggle-on-label-click", null)
			});
		}

		public override void Deserialize(object obj)
		{
			base.Deserialize(obj);
			BaseBoolField baseBoolField = (BaseBoolField)obj;
			if (UnityEngine.UIElements.UxmlSerializedData.ShouldWriteAttributeValue(toggleOnLabelClick_UxmlAttributeFlags))
			{
				baseBoolField.toggleOnLabelClick = toggleOnLabelClick;
			}
		}
	}

	internal static readonly BindingId textProperty = "text";

	internal static readonly BindingId toggleOnLabelClickProperty = "toggleOnLabelClick";

	protected Label m_Label;

	protected internal readonly VisualElement m_CheckMark;

	internal readonly Clickable m_Clickable;

	private string m_OriginalText;

	internal Label boolFieldLabelElement
	{
		[VisibleToOtherModules(new string[] { "UnityEditor.UIBuilderModule" })]
		get
		{
			return m_Label;
		}
	}

	internal bool acceptClicksIfDisabled
	{
		get
		{
			return m_Clickable.acceptClicksIfDisabled;
		}
		set
		{
			m_Clickable.acceptClicksIfDisabled = value;
		}
	}

	[CreateProperty]
	public bool toggleOnLabelClick { get; set; } = true;

	[VisibleToOtherModules(new string[] { "UnityEditor.UIBuilderModule" })]
	internal bool toggleOnTextClick { get; set; } = true;

	[CreateProperty]
	public string text
	{
		get
		{
			return m_Label?.text;
		}
		set
		{
			if (string.CompareOrdinal(m_Label?.text, value) != 0)
			{
				if (!string.IsNullOrEmpty(value))
				{
					InitLabel();
					m_Label.text = value;
				}
				else if (m_Label != null)
				{
					m_Label.RemoveFromHierarchy();
					m_Label.text = value;
				}
				NotifyPropertyChanged(in textProperty);
			}
		}
	}

	public BaseBoolField(string label)
		: base(label, (VisualElement)null)
	{
		m_CheckMark = new VisualElement
		{
			name = "unity-checkmark",
			pickingMode = PickingMode.Ignore
		};
		base.visualInput.Add(m_CheckMark);
		base.visualInput.pickingMode = PickingMode.Position;
		base.labelElement.focusable = false;
		text = null;
		this.AddManipulator(m_Clickable = new Clickable(OnClickEvent));
		RegisterCallback<NavigationSubmitEvent>(OnNavigationSubmit);
	}

	private void OnNavigationSubmit(NavigationSubmitEvent evt)
	{
		ToggleValue();
		evt.StopPropagation();
	}

	protected virtual void InitLabel()
	{
		if (m_Label == null)
		{
			m_Label = new Label();
		}
		else if (m_Label.parent != null)
		{
			return;
		}
		if (m_CheckMark.hierarchy.parent != base.visualInput)
		{
			base.visualInput.Add(m_Label);
			return;
		}
		int num = base.visualInput.IndexOf(m_CheckMark);
		base.visualInput.Insert(num + 1, m_Label);
	}

	public override void SetValueWithoutNotify(bool newValue)
	{
		if (newValue)
		{
			base.visualInput.pseudoStates |= PseudoStates.Checked;
			base.pseudoStates |= PseudoStates.Checked;
		}
		else
		{
			base.visualInput.pseudoStates &= ~PseudoStates.Checked;
			base.pseudoStates &= ~PseudoStates.Checked;
		}
		base.SetValueWithoutNotify(newValue);
	}

	private void OnClickEvent(EventBase evt)
	{
		if (evt.eventTypeId == EventBase<MouseUpEvent>.TypeId())
		{
			IMouseEvent mouseEvent = (IMouseEvent)evt;
			if (!ShouldIgnoreClick(mouseEvent.mousePosition) && mouseEvent.button == 0)
			{
				ToggleValue();
			}
		}
		else if (evt.eventTypeId == EventBase<PointerUpEvent>.TypeId() || evt.eventTypeId == EventBase<ClickEvent>.TypeId())
		{
			IPointerEvent pointerEvent = (IPointerEvent)evt;
			if (!ShouldIgnoreClick(pointerEvent.position) && pointerEvent.button == 0)
			{
				ToggleValue();
			}
		}
	}

	private bool ShouldIgnoreClick(Vector3 position)
	{
		if (!toggleOnLabelClick && base.labelElement.worldBound.Contains(position))
		{
			return true;
		}
		if (!toggleOnTextClick)
		{
			Label obj = m_Label;
			if (obj != null && obj.worldBound.Contains(position))
			{
				return true;
			}
		}
		return false;
	}

	protected virtual void ToggleValue()
	{
		value = !value;
	}

	protected override void UpdateMixedValueContent()
	{
		if (base.showMixedValue)
		{
			base.visualInput.pseudoStates &= ~PseudoStates.Checked;
			base.pseudoStates &= ~PseudoStates.Checked;
			m_CheckMark.RemoveFromHierarchy();
			base.visualInput.Add(base.mixedValueLabel);
			m_OriginalText = text;
			text = "";
		}
		else
		{
			base.mixedValueLabel.RemoveFromHierarchy();
			base.visualInput.Add(m_CheckMark);
			if (m_OriginalText != null)
			{
				text = m_OriginalText;
			}
		}
	}

	internal override void RegisterEditingCallbacks()
	{
		RegisterCallback<PointerUpEvent>(base.StartEditing);
		RegisterCallback<FocusOutEvent>(base.EndEditing);
	}

	internal override void UnregisterEditingCallbacks()
	{
		UnregisterCallback<PointerUpEvent>(base.StartEditing);
		UnregisterCallback<FocusOutEvent>(base.EndEditing);
	}
}
