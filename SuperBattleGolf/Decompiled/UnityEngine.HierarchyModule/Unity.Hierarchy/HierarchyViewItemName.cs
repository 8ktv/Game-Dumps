using System;
using UnityEngine;
using UnityEngine.Bindings;
using UnityEngine.UIElements;

namespace Unity.Hierarchy;

[UxmlElement]
[VisibleToOtherModules(new string[] { "UnityEditor.HierarchyModule" })]
internal class HierarchyViewItemName : VisualElement
{
	internal const string k_StyleName = "hierarchy-item__name";

	private bool m_PrewarmControl;

	public string Text
	{
		get
		{
			return Label.text;
		}
		set
		{
			Label.text = value;
		}
	}

	internal bool IsRenaming { get; set; }

	public Label Label { get; } = new Label();

	private TextField TextField { get; } = new TextField();

	public event Action OnBeginRename;

	public event Action<string, bool> OnEndRename;

	public HierarchyViewItemName()
	{
		AddToClassList("hierarchy-item__name");
		focusable = true;
		base.delegatesFocus = false;
		m_PrewarmControl = false;
		Add(Label);
		Add(TextField);
		TextField.selectAllOnFocus = true;
		TextField.selectAllOnMouseUp = false;
		TextField.style.display = DisplayStyle.None;
		TextField.RegisterCallback<MouseUpEvent>(OnMouseUpEvent);
		TextField.RegisterCallback<KeyDownEvent>(OnInterceptKeyDownEvent, TrickleDown.TrickleDown);
		TextField.RegisterCallback<KeyDownEvent>(OnKeyDownEvent);
		TextField.RegisterCallback<BlurEvent>(OnBlurEvent);
	}

	public void BeginRename()
	{
		if (!IsRenaming)
		{
			IsRenaming = true;
			base.delegatesFocus = true;
			m_PrewarmControl = true;
			Label.style.display = DisplayStyle.None;
			TextField.style.display = DisplayStyle.Flex;
			TextField.value = Text;
			TextField.Q<TextElement>().Focus();
			this.OnBeginRename?.Invoke();
		}
	}

	public void CancelRename()
	{
		if (IsRenaming)
		{
			EndRename(canceled: true);
		}
	}

	private void EndRename(bool canceled = false)
	{
		IsRenaming = false;
		base.delegatesFocus = false;
		m_PrewarmControl = false;
		TextField.style.display = DisplayStyle.None;
		Label.style.display = DisplayStyle.Flex;
		if (!canceled && !string.IsNullOrEmpty(TextField.value))
		{
			Label.text = TextField.value;
		}
		this.OnEndRename?.Invoke(Text, canceled);
	}

	private void OnMouseUpEvent(MouseUpEvent evt)
	{
		if (IsRenaming)
		{
			TextField.Q<TextElement>().Focus();
			evt.StopPropagation();
		}
	}

	private void OnInterceptKeyDownEvent(KeyDownEvent evt)
	{
		if (m_PrewarmControl)
		{
			if (evt.keyCode == KeyCode.None)
			{
				evt.StopPropagation();
			}
			else
			{
				m_PrewarmControl = false;
			}
		}
	}

	private void OnKeyDownEvent(KeyDownEvent evt)
	{
		if (IsRenaming && evt.keyCode == KeyCode.Escape)
		{
			EndRename(canceled: true);
		}
		evt.StopPropagation();
	}

	private void OnBlurEvent(BlurEvent evt)
	{
		if (IsRenaming)
		{
			EndRename();
		}
	}
}
