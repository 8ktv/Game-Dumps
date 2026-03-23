using System;
using System.Collections.Generic;

namespace UnityEngine.UIElements;

internal class RuntimePanel : BaseRuntimePanel, IRuntimePanel, IPanel, IDisposable
{
	internal static readonly EventDispatcher s_EventDispatcher = RuntimeEventDispatcher.Create();

	private readonly PanelSettings m_PanelSettings;

	private static readonly List<UIDocument> s_EmptyDocumentList = new List<UIDocument>();

	public PanelSettings panelSettings => m_PanelSettings;

	internal List<UIDocument> documents => m_PanelSettings.m_AttachedUIDocumentsList?.m_AttachedUIDocuments ?? s_EmptyDocumentList;

	public static RuntimePanel Create(ScriptableObject ownerObject)
	{
		return new RuntimePanel(ownerObject);
	}

	private RuntimePanel(ScriptableObject ownerObject)
		: base(ownerObject, s_EventDispatcher)
	{
		focusController = new FocusController(new NavigateFocusRing(visualTree));
		m_PanelSettings = ownerObject as PanelSettings;
		base.name = ((m_PanelSettings != null) ? m_PanelSettings.name : "RuntimePanel");
		visualTree.RegisterCallback(delegate(FocusEvent e, RuntimePanel p)
		{
			p.OnElementFocus(e);
		}, this, TrickleDown.TrickleDown);
	}

	internal override void Update()
	{
		if (m_PanelSettings != null)
		{
			m_PanelSettings.ApplyPanelSettings();
		}
		base.Update();
	}

	private void OnElementFocus(FocusEvent evt)
	{
		UIElementsRuntimeUtility.defaultEventSystem.OnFocusEvent(this, evt);
	}
}
