#define UNITY_ASSERTIONS
using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using UnityEngine.TextCore;
using UnityEngine.UIElements.Experimental;

namespace UnityEngine.UIElements;

internal class ATGTextEventHandler
{
	private static readonly Regex s_ATagRegex = new Regex("(?<=\\b=\")[^\"]*");

	private static readonly Regex s_LinkTagRegex = new Regex("(?<=\\b=')[^']*");

	private TextElement m_TextElement;

	private EventCallback<PointerDownEvent> m_LinkTagOnPointerDown;

	private EventCallback<PointerUpEvent> m_LinkTagOnPointerUp;

	private EventCallback<PointerMoveEvent> m_LinkTagOnPointerMove;

	private EventCallback<PointerOutEvent> m_LinkTagOnPointerOut;

	private EventCallback<PointerUpEvent> m_HyperlinkOnPointerUp;

	private EventCallback<PointerMoveEvent> m_HyperlinkOnPointerMove;

	private EventCallback<PointerOverEvent> m_HyperlinkOnPointerOver;

	private EventCallback<PointerOutEvent> m_HyperlinkOnPointerOut;

	internal bool isOverridingCursor;

	internal int currentLinkIDHash = -1;

	internal static event Action<Dictionary<string, string>> onComplexHyperlinkClicked;

	public ATGTextEventHandler(TextElement textElement)
	{
		Debug.Assert(textElement.uitkTextHandle.useAdvancedText);
		m_TextElement = textElement;
	}

	public void OnDestroy()
	{
		UnRegisterLinkTagCallbacks();
		UnRegisterHyperlinkCallbacks();
	}

	private bool HasAllocatedLinkCallbacks()
	{
		return m_LinkTagOnPointerDown != null;
	}

	private void AllocateLinkCallbacks()
	{
		if (!HasAllocatedLinkCallbacks())
		{
			m_LinkTagOnPointerDown = LinkTagOnPointerDown;
			m_LinkTagOnPointerUp = LinkTagOnPointerUp;
			m_LinkTagOnPointerMove = LinkTagOnPointerMove;
			m_LinkTagOnPointerOut = LinkTagOnPointerOut;
		}
	}

	private bool HasAllocatedHyperlinkCallbacks()
	{
		return m_HyperlinkOnPointerUp != null;
	}

	private void AllocateHyperlinkCallbacks()
	{
		if (!HasAllocatedHyperlinkCallbacks())
		{
			m_HyperlinkOnPointerUp = HyperlinkOnPointerUp;
			m_HyperlinkOnPointerMove = HyperlinkOnPointerMove;
			m_HyperlinkOnPointerOver = HyperlinkOnPointerOver;
			m_HyperlinkOnPointerOut = HyperlinkOnPointerOut;
		}
	}

	private void HyperlinkOnPointerUp(PointerUpEvent pue)
	{
		Vector3 vector = pue.localPosition - new Vector3(m_TextElement.contentRect.min.x, m_TextElement.contentRect.min.y);
		var (tagType, text) = m_TextElement.uitkTextHandle.ATGFindIntersectingLink(vector);
		if (text != null && tagType == RichTextTagParser.TagType.Hyperlink)
		{
			Dictionary<string, string> hyperLinkData;
			if (Uri.IsWellFormedUriString(text, UriKind.Absolute))
			{
				Application.OpenURL(text);
			}
			else if (IsComplexHyperLink(text, out hyperLinkData))
			{
				ATGTextEventHandler.onComplexHyperlinkClicked?.Invoke(hyperLinkData);
			}
		}
	}

	private static bool IsComplexHyperLink(string link, out Dictionary<string, string> hyperLinkData)
	{
		hyperLinkData = new Dictionary<string, string>();
		MatchCollection matchCollection = s_ATagRegex.Matches(link);
		if (matchCollection.Count == 0)
		{
			matchCollection = s_LinkTagRegex.Matches(link);
		}
		int num = 0;
		foreach (Match item in matchCollection)
		{
			string text = link.Substring(num, item.Index - 2 - num);
			int startIndex = text.LastIndexOf(' ') + 1;
			string key = text.Substring(startIndex);
			hyperLinkData.Add(key, item.Value);
			num = item.Index + item.Value.Length + 1;
		}
		return true;
	}

	private void HyperlinkOnPointerOver(PointerOverEvent _)
	{
		isOverridingCursor = false;
	}

	private void HyperlinkOnPointerMove(PointerMoveEvent pme)
	{
		Vector3 vector = pme.localPosition - new Vector3(m_TextElement.contentRect.min.x, m_TextElement.contentRect.min.y);
		(RichTextTagParser.TagType, string) tuple = m_TextElement.uitkTextHandle.ATGFindIntersectingLink(vector);
		RichTextTagParser.TagType item = tuple.Item1;
		string item2 = tuple.Item2;
		ICursorManager cursorManager = (m_TextElement.panel as BaseVisualElementPanel)?.cursorManager;
		if (item2 != null && item == RichTextTagParser.TagType.Hyperlink)
		{
			if (!isOverridingCursor)
			{
				isOverridingCursor = true;
				cursorManager?.SetCursor(new Cursor
				{
					defaultCursorId = 4
				});
			}
		}
		else if (isOverridingCursor)
		{
			cursorManager?.SetCursor(m_TextElement.computedStyle.cursor);
			isOverridingCursor = false;
		}
	}

	private void HyperlinkOnPointerOut(PointerOutEvent evt)
	{
		isOverridingCursor = false;
	}

	private void LinkTagOnPointerDown(PointerDownEvent pde)
	{
		Vector3 vector = pde.localPosition - new Vector3(m_TextElement.contentRect.min.x, m_TextElement.contentRect.min.y);
		var (tagType, text) = m_TextElement.uitkTextHandle.ATGFindIntersectingLink(vector);
		if (text == null || tagType != RichTextTagParser.TagType.Link)
		{
			return;
		}
		using PointerDownLinkTagEvent pointerDownLinkTagEvent = PointerDownLinkTagEvent.GetPooled(pde, text, "test");
		pointerDownLinkTagEvent.elementTarget = m_TextElement;
		m_TextElement.SendEvent(pointerDownLinkTagEvent);
	}

	private void LinkTagOnPointerUp(PointerUpEvent pue)
	{
		Vector3 vector = pue.localPosition - new Vector3(m_TextElement.contentRect.min.x, m_TextElement.contentRect.min.y);
		var (tagType, text) = m_TextElement.uitkTextHandle.ATGFindIntersectingLink(vector);
		if (text == null || tagType != RichTextTagParser.TagType.Link)
		{
			return;
		}
		using PointerUpLinkTagEvent pointerUpLinkTagEvent = PointerUpLinkTagEvent.GetPooled(pue, text, "test");
		pointerUpLinkTagEvent.elementTarget = m_TextElement;
		m_TextElement.SendEvent(pointerUpLinkTagEvent);
	}

	private void LinkTagOnPointerMove(PointerMoveEvent pme)
	{
		Vector3 vector = pme.localPosition - new Vector3(m_TextElement.contentRect.min.x, m_TextElement.contentRect.min.y);
		var (tagType, text) = m_TextElement.uitkTextHandle.ATGFindIntersectingLink(vector);
		if (text != null && tagType == RichTextTagParser.TagType.Link)
		{
			if (currentLinkIDHash == -1)
			{
				currentLinkIDHash = 0;
				using PointerOverLinkTagEvent pointerOverLinkTagEvent = PointerOverLinkTagEvent.GetPooled(pme, text, "test");
				pointerOverLinkTagEvent.elementTarget = m_TextElement;
				m_TextElement.SendEvent(pointerOverLinkTagEvent);
				return;
			}
			if (currentLinkIDHash == 0)
			{
				using (PointerMoveLinkTagEvent pointerMoveLinkTagEvent = PointerMoveLinkTagEvent.GetPooled(pme, text, "test"))
				{
					pointerMoveLinkTagEvent.elementTarget = m_TextElement;
					m_TextElement.SendEvent(pointerMoveLinkTagEvent);
					return;
				}
			}
		}
		if (currentLinkIDHash != -1)
		{
			currentLinkIDHash = -1;
			using PointerOutLinkTagEvent pointerOutLinkTagEvent = PointerOutLinkTagEvent.GetPooled(pme, string.Empty);
			pointerOutLinkTagEvent.elementTarget = m_TextElement;
			m_TextElement.SendEvent(pointerOutLinkTagEvent);
		}
	}

	private void LinkTagOnPointerOut(PointerOutEvent poe)
	{
		if (currentLinkIDHash != -1)
		{
			using (PointerOutLinkTagEvent pointerOutLinkTagEvent = PointerOutLinkTagEvent.GetPooled(poe, string.Empty))
			{
				pointerOutLinkTagEvent.elementTarget = m_TextElement;
				m_TextElement.SendEvent(pointerOutLinkTagEvent);
			}
			currentLinkIDHash = -1;
		}
	}

	internal void RegisterLinkTagCallbacks()
	{
		if (m_TextElement?.panel != null)
		{
			AllocateLinkCallbacks();
			m_TextElement.RegisterCallback(m_LinkTagOnPointerDown, TrickleDown.TrickleDown);
			m_TextElement.RegisterCallback(m_LinkTagOnPointerUp, TrickleDown.TrickleDown);
			m_TextElement.RegisterCallback(m_LinkTagOnPointerMove, TrickleDown.TrickleDown);
			m_TextElement.RegisterCallback(m_LinkTagOnPointerOut, TrickleDown.TrickleDown);
		}
	}

	internal void UnRegisterLinkTagCallbacks()
	{
		if (HasAllocatedLinkCallbacks())
		{
			m_TextElement.UnregisterCallback(m_LinkTagOnPointerDown, TrickleDown.TrickleDown);
			m_TextElement.UnregisterCallback(m_LinkTagOnPointerUp, TrickleDown.TrickleDown);
			m_TextElement.UnregisterCallback(m_LinkTagOnPointerMove, TrickleDown.TrickleDown);
			m_TextElement.UnregisterCallback(m_LinkTagOnPointerOut, TrickleDown.TrickleDown);
		}
	}

	internal void RegisterHyperlinkCallbacks()
	{
		if (m_TextElement?.panel != null)
		{
			AllocateHyperlinkCallbacks();
			m_TextElement.RegisterCallback(m_HyperlinkOnPointerUp, TrickleDown.TrickleDown);
			if (m_TextElement.panel.contextType == ContextType.Editor)
			{
				m_TextElement.RegisterCallback(m_HyperlinkOnPointerMove, TrickleDown.TrickleDown);
				m_TextElement.RegisterCallback(m_HyperlinkOnPointerOver, TrickleDown.TrickleDown);
				m_TextElement.RegisterCallback(m_HyperlinkOnPointerOut, TrickleDown.TrickleDown);
			}
		}
	}

	internal void UnRegisterHyperlinkCallbacks()
	{
		if (m_TextElement?.panel != null && HasAllocatedHyperlinkCallbacks())
		{
			m_TextElement.UnregisterCallback(m_HyperlinkOnPointerUp, TrickleDown.TrickleDown);
			if (m_TextElement.panel.contextType == ContextType.Editor)
			{
				m_TextElement.UnregisterCallback(m_HyperlinkOnPointerMove, TrickleDown.TrickleDown);
				m_TextElement.UnregisterCallback(m_HyperlinkOnPointerOver, TrickleDown.TrickleDown);
				m_TextElement.UnregisterCallback(m_HyperlinkOnPointerOut, TrickleDown.TrickleDown);
			}
		}
	}
}
