using System;
using UnityEngine.TextCore.Text;
using UnityEngine.UIElements.Experimental;

namespace UnityEngine.UIElements;

internal class TextEventHandler
{
	private TextElement m_TextElement;

	private EventCallback<PointerDownEvent> m_LinkTagOnPointerDown;

	private EventCallback<PointerUpEvent> m_LinkTagOnPointerUp;

	private EventCallback<PointerMoveEvent> m_LinkTagOnPointerMove;

	private EventCallback<PointerOutEvent> m_LinkTagOnPointerOut;

	private EventCallback<PointerUpEvent> m_ATagOnPointerUp;

	private EventCallback<PointerMoveEvent> m_ATagOnPointerMove;

	private EventCallback<PointerOverEvent> m_ATagOnPointerOver;

	private EventCallback<PointerOutEvent> m_ATagOnPointerOut;

	internal bool isOverridingCursor;

	internal int currentLinkIDHash = -1;

	internal bool hasLinkTag;

	internal bool hasATag;

	private TextInfo textInfo => m_TextElement.uitkTextHandle.textInfo;

	public TextEventHandler(TextElement textElement)
	{
		m_TextElement = textElement;
	}

	public void OnDestroy()
	{
		if (HasAllocatedLinkCallbacks())
		{
			m_TextElement.UnregisterCallback(m_LinkTagOnPointerDown, TrickleDown.TrickleDown);
			m_TextElement.UnregisterCallback(m_LinkTagOnPointerUp, TrickleDown.TrickleDown);
			m_TextElement.UnregisterCallback(m_LinkTagOnPointerMove, TrickleDown.TrickleDown);
			m_TextElement.UnregisterCallback(m_LinkTagOnPointerOut, TrickleDown.TrickleDown);
		}
		if (HasAllocatedATagCallbacks())
		{
			m_TextElement.UnregisterCallback(m_ATagOnPointerOver, TrickleDown.TrickleDown);
			m_TextElement.UnregisterCallback(m_ATagOnPointerMove, TrickleDown.TrickleDown);
			m_TextElement.UnregisterCallback(m_ATagOnPointerUp, TrickleDown.TrickleDown);
			m_TextElement.UnregisterCallback(m_ATagOnPointerOut, TrickleDown.TrickleDown);
		}
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

	private bool HasAllocatedATagCallbacks()
	{
		return m_ATagOnPointerUp != null;
	}

	private void AllocateATagCallbacks()
	{
		if (!HasAllocatedATagCallbacks())
		{
			m_ATagOnPointerUp = ATagOnPointerUp;
			m_ATagOnPointerMove = ATagOnPointerMove;
			m_ATagOnPointerOver = ATagOnPointerOver;
			m_ATagOnPointerOut = ATagOnPointerOut;
		}
	}

	private void ATagOnPointerUp(PointerUpEvent pue)
	{
		Vector3 position = pue.localPosition - new Vector3(m_TextElement.contentRect.min.x, m_TextElement.contentRect.min.y);
		int num = m_TextElement.uitkTextHandle.FindIntersectingLink(position);
		if (num < 0)
		{
			return;
		}
		LinkInfo linkInfo = textInfo.linkInfo[num];
		if (linkInfo.hashCode == 2535353 && linkInfo.linkId != null && linkInfo.linkIdLength > 0)
		{
			string linkId = linkInfo.GetLinkId();
			if (Uri.IsWellFormedUriString(linkId, UriKind.Absolute))
			{
				Application.OpenURL(linkId);
			}
		}
	}

	private void ATagOnPointerOver(PointerOverEvent _)
	{
		isOverridingCursor = false;
	}

	private void ATagOnPointerMove(PointerMoveEvent pme)
	{
		Vector3 position = pme.localPosition - new Vector3(m_TextElement.contentRect.min.x, m_TextElement.contentRect.min.y);
		int num = m_TextElement.uitkTextHandle.FindIntersectingLink(position);
		ICursorManager cursorManager = (m_TextElement.panel as BaseVisualElementPanel)?.cursorManager;
		if (num >= 0)
		{
			LinkInfo linkInfo = textInfo.linkInfo[num];
			if (linkInfo.hashCode == 2535353)
			{
				if (!isOverridingCursor)
				{
					isOverridingCursor = true;
					cursorManager?.SetCursor(new Cursor
					{
						defaultCursorId = 4
					});
				}
				return;
			}
		}
		if (isOverridingCursor)
		{
			cursorManager?.SetCursor(m_TextElement.computedStyle.cursor);
			isOverridingCursor = false;
		}
	}

	private void ATagOnPointerOut(PointerOutEvent evt)
	{
		isOverridingCursor = false;
	}

	private void LinkTagOnPointerDown(PointerDownEvent pde)
	{
		Vector3 position = pde.localPosition - new Vector3(m_TextElement.contentRect.min.x, m_TextElement.contentRect.min.y);
		int num = m_TextElement.uitkTextHandle.FindIntersectingLink(position);
		if (num < 0)
		{
			return;
		}
		LinkInfo linkInfo = textInfo.linkInfo[num];
		if (linkInfo.hashCode == 2535353 || linkInfo.linkId == null || linkInfo.linkIdLength <= 0)
		{
			return;
		}
		using PointerDownLinkTagEvent pointerDownLinkTagEvent = PointerDownLinkTagEvent.GetPooled(pde, linkInfo.GetLinkId(), linkInfo.GetLinkText(textInfo));
		pointerDownLinkTagEvent.elementTarget = m_TextElement;
		m_TextElement.SendEvent(pointerDownLinkTagEvent);
	}

	private void LinkTagOnPointerUp(PointerUpEvent pue)
	{
		Vector3 position = pue.localPosition - new Vector3(m_TextElement.contentRect.min.x, m_TextElement.contentRect.min.y);
		int num = m_TextElement.uitkTextHandle.FindIntersectingLink(position);
		if (num < 0)
		{
			return;
		}
		LinkInfo linkInfo = textInfo.linkInfo[num];
		if (linkInfo.hashCode == 2535353 || linkInfo.linkId == null || linkInfo.linkIdLength <= 0)
		{
			return;
		}
		using PointerUpLinkTagEvent pointerUpLinkTagEvent = PointerUpLinkTagEvent.GetPooled(pue, linkInfo.GetLinkId(), linkInfo.GetLinkText(textInfo));
		pointerUpLinkTagEvent.elementTarget = m_TextElement;
		m_TextElement.SendEvent(pointerUpLinkTagEvent);
	}

	private void LinkTagOnPointerMove(PointerMoveEvent pme)
	{
		Vector3 position = pme.localPosition - new Vector3(m_TextElement.contentRect.min.x, m_TextElement.contentRect.min.y);
		int num = m_TextElement.uitkTextHandle.FindIntersectingLink(position);
		if (num >= 0)
		{
			LinkInfo linkInfo = textInfo.linkInfo[num];
			if (linkInfo.hashCode != 2535353)
			{
				if (currentLinkIDHash == -1)
				{
					currentLinkIDHash = linkInfo.hashCode;
					using PointerOverLinkTagEvent pointerOverLinkTagEvent = PointerOverLinkTagEvent.GetPooled(pme, linkInfo.GetLinkId(), linkInfo.GetLinkText(textInfo));
					pointerOverLinkTagEvent.elementTarget = m_TextElement;
					m_TextElement.SendEvent(pointerOverLinkTagEvent);
					return;
				}
				if (currentLinkIDHash == linkInfo.hashCode)
				{
					using (PointerMoveLinkTagEvent pointerMoveLinkTagEvent = PointerMoveLinkTagEvent.GetPooled(pme, linkInfo.GetLinkId(), linkInfo.GetLinkText(textInfo)))
					{
						pointerMoveLinkTagEvent.elementTarget = m_TextElement;
						m_TextElement.SendEvent(pointerMoveLinkTagEvent);
						return;
					}
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

	internal void HandleLinkAndATagCallbacks()
	{
		if (m_TextElement?.panel == null)
		{
			return;
		}
		if (hasLinkTag)
		{
			AllocateLinkCallbacks();
			m_TextElement.RegisterCallback(m_LinkTagOnPointerDown, TrickleDown.TrickleDown);
			m_TextElement.RegisterCallback(m_LinkTagOnPointerUp, TrickleDown.TrickleDown);
			m_TextElement.RegisterCallback(m_LinkTagOnPointerMove, TrickleDown.TrickleDown);
			m_TextElement.RegisterCallback(m_LinkTagOnPointerOut, TrickleDown.TrickleDown);
		}
		else if (HasAllocatedLinkCallbacks())
		{
			m_TextElement.UnregisterCallback(m_LinkTagOnPointerDown, TrickleDown.TrickleDown);
			m_TextElement.UnregisterCallback(m_LinkTagOnPointerUp, TrickleDown.TrickleDown);
			m_TextElement.UnregisterCallback(m_LinkTagOnPointerMove, TrickleDown.TrickleDown);
			m_TextElement.UnregisterCallback(m_LinkTagOnPointerOut, TrickleDown.TrickleDown);
		}
		if (hasATag)
		{
			AllocateATagCallbacks();
			m_TextElement.RegisterCallback(m_ATagOnPointerUp, TrickleDown.TrickleDown);
			if (m_TextElement.panel.contextType == ContextType.Editor)
			{
				m_TextElement.RegisterCallback(m_ATagOnPointerMove, TrickleDown.TrickleDown);
				m_TextElement.RegisterCallback(m_ATagOnPointerOver, TrickleDown.TrickleDown);
				m_TextElement.RegisterCallback(m_ATagOnPointerOut, TrickleDown.TrickleDown);
			}
		}
		else if (HasAllocatedATagCallbacks())
		{
			m_TextElement.UnregisterCallback(m_ATagOnPointerUp, TrickleDown.TrickleDown);
			if (m_TextElement.panel.contextType == ContextType.Editor)
			{
				m_TextElement.UnregisterCallback(m_ATagOnPointerMove, TrickleDown.TrickleDown);
				m_TextElement.UnregisterCallback(m_ATagOnPointerOver, TrickleDown.TrickleDown);
				m_TextElement.UnregisterCallback(m_ATagOnPointerOut, TrickleDown.TrickleDown);
			}
		}
	}

	internal void HandleLinkTag()
	{
		for (int i = 0; i < textInfo.linkCount; i++)
		{
			LinkInfo linkInfo = textInfo.linkInfo[i];
			if (linkInfo.hashCode != 2535353)
			{
				hasLinkTag = true;
				m_TextElement.uitkTextHandle.AddToPermanentCacheAndGenerateMesh();
				return;
			}
		}
		if (hasLinkTag)
		{
			hasLinkTag = false;
			m_TextElement.uitkTextHandle.RemoveFromPermanentCache();
		}
	}

	internal void HandleATag()
	{
		for (int i = 0; i < textInfo.linkCount; i++)
		{
			LinkInfo linkInfo = textInfo.linkInfo[i];
			if (linkInfo.hashCode == 2535353)
			{
				hasATag = true;
				m_TextElement.uitkTextHandle.AddToPermanentCacheAndGenerateMesh();
				return;
			}
		}
		if (hasATag)
		{
			hasATag = false;
			m_TextElement.uitkTextHandle.RemoveFromPermanentCache();
		}
	}
}
