using System;
using System.Collections.Generic;
using UnityEngine.Bindings;
using UnityEngine.UIElements;

namespace Unity.Hierarchy;

[VisibleToOtherModules(new string[] { "UnityEditor.HierarchyModule", "UnityEditor.UIToolkitAuthoringModule" })]
internal class HierarchyViewColumnUtility
{
	public const string k_ToggleIcon = "toggle-icon";

	public const string k_CellPropField = "cell-prop-field";

	public static HierarchyViewCell GetCellFromTarget(VisualElement target)
	{
		return (HierarchyViewCell)target.parent;
	}

	public static HierarchyViewCellValueEditor<TModel, TEditor, TValue> BindCellToValueEditor<TModel, TEditor, TValue>(TModel model, HierarchyViewCell cell, HierarchyViewColumnContextPool<HierarchyViewCellValueEditor<TModel, TEditor, TValue>> pool, params string[] classes) where TEditor : VisualElement, INotifyValueChanged<TValue>, new()
	{
		TEditor orCreateEditor = GetOrCreateEditor<TEditor>(cell, classes);
		HierarchyViewCellValueEditor<TModel, TEditor, TValue> hierarchyViewCellValueEditor = pool.Get(cell.View.GetHashCode());
		hierarchyViewCellValueEditor.Bind(model, cell, orCreateEditor);
		return hierarchyViewCellValueEditor;
	}

	public static void UnbindCellFromValueEditor<TModel, TEditor, TValue>(HierarchyViewCell cell, HierarchyViewColumnContextPool<HierarchyViewCellValueEditor<TModel, TEditor, TValue>> pool) where TEditor : VisualElement, INotifyValueChanged<TValue>, new()
	{
		if (cell.userData is HierarchyViewCellValueEditor<TModel, TEditor, TValue> hierarchyViewCellValueEditor)
		{
			pool.Release(cell.View.GetHashCode(), hierarchyViewCellValueEditor);
			hierarchyViewCellValueEditor.Unbind();
		}
	}

	public static TEditor GetOrCreateEditor<TEditor>(HierarchyViewCell cell, params string[] classes) where TEditor : VisualElement, new()
	{
		TEditor val = cell.Q<TEditor>();
		if (val == null)
		{
			val = new TEditor();
			AddToClassList(val, classes);
			cell.Add(val);
		}
		return val;
	}

	internal static VisualElement AddToClassList(VisualElement element, params string[] classes)
	{
		foreach (string className in classes)
		{
			element.AddToClassList(className);
		}
		return element;
	}

	[VisibleToOtherModules(new string[] { "UnityEditor.UIToolkitAuthoringModule" })]
	internal static HierarchyViewCellValueEditor<TModel, TEditor, TValue> CreateCellValueEditor<TModel, TEditor, TValue>(TModel model, HierarchyViewCell cell, Func<HierarchyViewCellValueEditor<TModel, TEditor, TValue>, TValue> getModelValue, Action<HierarchyViewCellValueEditor<TModel, TEditor, TValue>, TValue> setModelValue, Func<HierarchyViewCellValueEditor<TModel, TEditor, TValue>, TValue, bool> isDefaultValue, params string[] classes) where TEditor : VisualElement, INotifyValueChanged<TValue>, new()
	{
		TEditor orCreateEditor = GetOrCreateEditor<TEditor>(cell, classes);
		HierarchyViewCellValueEditor<TModel, TEditor, TValue> hierarchyViewCellValueEditor = new HierarchyViewCellValueEditor<TModel, TEditor, TValue>(getModelValue, setModelValue, isDefaultValue);
		hierarchyViewCellValueEditor.Bind(model, cell, orCreateEditor);
		return hierarchyViewCellValueEditor;
	}

	internal static int GetVisibleIndex(HierarchyViewState viewState, Column c)
	{
		string columnId = GetColumnId(c);
		HierarchyViewColumnState[] columns = viewState.Columns;
		foreach (HierarchyViewColumnState hierarchyViewColumnState in columns)
		{
			if (hierarchyViewColumnState.ColumnId == columnId)
			{
				return hierarchyViewColumnState.Index;
			}
		}
		return GetColumnDefaultPriority(c);
	}

	internal static string GetColumnId(Column col)
	{
		if (col is HierarchyViewColumn hierarchyViewColumn)
		{
			return hierarchyViewColumn.Descriptor.Id;
		}
		if (col is HierarchyViewItemColumn)
		{
			return "HierarchyViewColumn Name";
		}
		return null;
	}

	internal static int GetColumnDefaultPriority(Column col)
	{
		if (col is HierarchyViewColumn hierarchyViewColumn)
		{
			return hierarchyViewColumn.Descriptor.DefaultPriority;
		}
		if (col is HierarchyViewItemColumn)
		{
			return 0;
		}
		return 1000;
	}

	internal static Column GetColumnWithId(IEnumerable<Column> columns, string id)
	{
		foreach (Column column in columns)
		{
			if (GetColumnId(column) == id)
			{
				return column;
			}
		}
		return null;
	}
}
