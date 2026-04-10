using System;
using UnityEngine.Bindings;
using UnityEngine.UIElements;

namespace Unity.Hierarchy;

[VisibleToOtherModules(new string[] { "UnityEditor.UIToolkitAuthoringModule" })]
internal sealed class HierarchyViewCellValueEditor<TModel, TEditor, TValue> where TEditor : VisualElement, INotifyValueChanged<TValue>, new()
{
	private readonly Func<HierarchyViewCellValueEditor<TModel, TEditor, TValue>, TValue> m_GetModelValue;

	private readonly Action<HierarchyViewCellValueEditor<TModel, TEditor, TValue>, TValue> m_SetModelValue;

	private readonly Func<HierarchyViewCellValueEditor<TModel, TEditor, TValue>, TValue, bool> m_IsDefaultValue;

	private readonly Action<HierarchyViewCellValueEditor<TModel, TEditor, TValue>, TValue> m_OnSetEditorValue;

	public TEditor Element;

	public TModel Model { get; private set; }

	public HierarchyViewCell Cell { get; private set; }

	public HierarchyViewCellValueEditor(Func<HierarchyViewCellValueEditor<TModel, TEditor, TValue>, TValue> getModelValue, Action<HierarchyViewCellValueEditor<TModel, TEditor, TValue>, TValue> setModelValue, Func<HierarchyViewCellValueEditor<TModel, TEditor, TValue>, TValue, bool> isDefaultValue, Action<HierarchyViewCellValueEditor<TModel, TEditor, TValue>, TValue> onSetEditorValue = null)
	{
		m_GetModelValue = getModelValue;
		m_SetModelValue = setModelValue;
		m_IsDefaultValue = isDefaultValue;
		m_OnSetEditorValue = onSetEditorValue;
	}

	public void Bind(TModel model, HierarchyViewCell cell, TEditor editor)
	{
		Model = model;
		Cell = cell;
		Cell.userData = this;
		Element = editor;
		Element.visible = true;
		Element.RegisterCallback<ChangeEvent<TValue>>(SetModelValue);
		SyncEditorValueWithoutNotify();
	}

	public void Unbind()
	{
		Cell.userData = null;
		Cell = null;
		Element.visible = false;
		Element.UnregisterCallback<ChangeEvent<TValue>>(SetModelValue);
		Element = null;
	}

	public TValue GetModelValue()
	{
		return m_GetModelValue(this);
	}

	public void SetModelValue(TValue value)
	{
		if (Cell != null)
		{
			if (!GetModelValue().Equals(value))
			{
				m_SetModelValue(this, value);
			}
			Cell.IsDefaultValue = IsModelDefaultValue();
		}
	}

	public TValue GetEditorValue()
	{
		return Element.value;
	}

	public void SetModelValue(ChangeEvent<TValue> evt)
	{
		SetModelValue(evt.newValue);
	}

	public void SetEditorValueWithoutNotify(TValue value)
	{
		if (!value.Equals(Element.value))
		{
			Element.SetValueWithoutNotify(value);
		}
		m_OnSetEditorValue?.Invoke(this, value);
		Cell.IsDefaultValue = IsModelDefaultValue();
	}

	public void SyncEditorValueWithoutNotify()
	{
		SetEditorValueWithoutNotify(GetModelValue());
	}

	public bool IsModelDefaultValue()
	{
		return m_IsDefaultValue(this, GetModelValue());
	}
}
