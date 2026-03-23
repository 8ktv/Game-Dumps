using System;
using UnityEngine.Bindings;
using UnityEngine.Scripting;
using UnityEngine.TextCore.Text;

namespace UnityEngine;

public class TextEditor
{
	public enum DblClickSnapping : byte
	{
		WORDS,
		PARAGRAPHS
	}

	private readonly GUIContent m_Content = new GUIContent();

	private TextSelectingUtilities m_TextSelecting;

	internal TextEditingUtilities m_TextEditing;

	internal IMGUITextHandle m_TextHandle;

	public TouchScreenKeyboard keyboardOnScreen = null;

	public int controlID = 0;

	public GUIStyle style;

	[Obsolete("'multiline' has been deprecated. Changes to this member will not be observed. Use 'isMultiline' instead.", true)]
	public bool multiline;

	[Obsolete("'hasHorizontalCursorPos' has been deprecated. Changes to this member will not be observed. Use 'hasHorizontalCursor' instead.", true)]
	public bool hasHorizontalCursorPos = false;

	public bool isPasswordField = false;

	public Vector2 scrollOffset;

	[Obsolete("'revealCursor' has been deprecated. Changes to this member will not be observed. Use 'showCursor' instead.", true)]
	public bool revealCursor;

	private bool focus;

	private string m_TextWithWhitespace;

	public Vector2 graphicalCursorPos;

	public Vector2 graphicalSelectCursorPos;

	private Vector2 lastCursorPos = Vector2.zero;

	private Vector2 previousContentSize = Vector2.zero;

	private string oldText;

	private int oldPos;

	private int oldSelectPos;

	public bool isMultiline
	{
		get
		{
			return m_TextEditing.multiline;
		}
		set
		{
			m_TextEditing.multiline = value;
		}
	}

	public bool hasHorizontalCursor
	{
		get
		{
			return m_TextSelecting.hasHorizontalCursorPos;
		}
		set
		{
			m_TextSelecting.hasHorizontalCursorPos = value;
		}
	}

	public bool showCursor
	{
		get
		{
			return m_TextSelecting.revealCursor;
		}
		set
		{
			m_TextSelecting.revealCursor = value;
		}
	}

	internal bool m_HasFocus
	{
		get
		{
			return focus;
		}
		set
		{
			focus = value;
		}
	}

	[Obsolete("Please use 'text' instead of 'content'", true)]
	public GUIContent content
	{
		get
		{
			throw new NotImplementedException("Please use 'text' instead of 'content'");
		}
		set
		{
			throw new NotImplementedException("Please use 'text' instead of 'content'");
		}
	}

	public string text
	{
		get
		{
			return m_TextEditing.text;
		}
		set
		{
			string text = value ?? "";
			if (!(m_TextEditing.text == text))
			{
				m_TextEditing.SetTextWithoutNotify(text);
				m_Content.SetTextWithoutNotify(text);
				textWithWhitespace = text;
				UpdateTextHandle();
			}
		}
	}

	internal string textWithWhitespace
	{
		get
		{
			return string.IsNullOrEmpty(m_TextWithWhitespace) ? GUIContent.k_ZeroWidthSpace : m_TextWithWhitespace;
		}
		set
		{
			m_TextWithWhitespace = value + GUIContent.k_ZeroWidthSpace;
		}
	}

	public Rect position { get; set; }

	internal virtual Rect localPosition => style.padding.Remove(position);

	public int cursorIndex
	{
		get
		{
			return m_TextSelecting.cursorIndex;
		}
		set
		{
			m_TextSelecting.cursorIndex = value;
		}
	}

	internal int stringCursorIndex
	{
		get
		{
			return m_TextEditing.stringCursorIndex;
		}
		set
		{
			m_TextEditing.stringCursorIndex = value;
		}
	}

	public int selectIndex
	{
		get
		{
			return m_TextSelecting.selectIndex;
		}
		set
		{
			m_TextSelecting.selectIndex = value;
		}
	}

	internal int stringSelectIndex
	{
		get
		{
			return m_TextEditing.stringSelectIndex;
		}
		set
		{
			m_TextEditing.stringSelectIndex = value;
		}
	}

	public DblClickSnapping doubleClickSnapping
	{
		get
		{
			return m_TextSelecting.dblClickSnap;
		}
		set
		{
			m_TextSelecting.dblClickSnap = value;
		}
	}

	public int altCursorPosition
	{
		get
		{
			return m_TextSelecting.iAltCursorPos;
		}
		set
		{
			m_TextSelecting.iAltCursorPos = value;
		}
	}

	public bool hasSelection => m_TextSelecting.hasSelection;

	public string SelectedText => m_TextSelecting.selectedText;

	[RequiredByNativeCode]
	public TextEditor()
	{
		GUIStyle none = GUIStyle.none;
		m_TextHandle = IMGUITextHandle.GetTextHandle(none, position, textWithWhitespace, Color.white);
		m_TextHandle.AddToPermanentCacheAndGenerateMesh();
		m_TextSelecting = new TextSelectingUtilities(m_TextHandle);
		m_TextEditing = new TextEditingUtilities(m_TextSelecting, m_TextHandle, m_Content.text);
		m_Content.OnTextChanged += OnContentTextChangedHandle;
		TextEditingUtilities textEditing = m_TextEditing;
		textEditing.OnTextChanged = (Action)Delegate.Combine(textEditing.OnTextChanged, new Action(OnTextChangedHandle));
		style = none;
		TextSelectingUtilities textSelecting = m_TextSelecting;
		textSelecting.OnCursorIndexChange = (Action)Delegate.Combine(textSelecting.OnCursorIndexChange, new Action(OnCursorIndexChange));
		TextSelectingUtilities textSelecting2 = m_TextSelecting;
		textSelecting2.OnSelectIndexChange = (Action)Delegate.Combine(textSelecting2.OnSelectIndexChange, new Action(OnSelectIndexChange));
	}

	private void OnTextChangedHandle()
	{
		m_Content.SetTextWithoutNotify(text);
		textWithWhitespace = text;
		UpdateTextHandle();
	}

	private void OnContentTextChangedHandle()
	{
		text = m_Content.text;
		textWithWhitespace = text;
	}

	public void OnFocus()
	{
		m_HasFocus = true;
		m_TextSelecting.OnFocus();
	}

	public void OnLostFocus()
	{
		m_HasFocus = false;
	}

	public bool HasClickedOnLink(Vector2 mousePosition, out string linkData)
	{
		Vector2 vector = mousePosition + scrollOffset;
		linkData = "";
		int num = m_TextHandle.FindIntersectingLink(vector - new Vector2(position.x, position.y));
		if (num < 0)
		{
			return false;
		}
		LinkInfo linkInfo = m_TextHandle.textInfo.linkInfo[num];
		if (linkInfo.linkId != null && linkInfo.linkIdLength > 0)
		{
			linkData = new string(linkInfo.linkId);
			return true;
		}
		return false;
	}

	public bool HasClickedOnHREF(Vector2 mousePosition, out string href)
	{
		Vector2 vector = mousePosition + scrollOffset;
		href = "";
		int num = m_TextHandle.FindIntersectingLink(vector - new Vector2(position.x, position.y));
		if (num < 0)
		{
			return false;
		}
		LinkInfo linkInfo = m_TextHandle.textInfo.linkInfo[num];
		if (linkInfo.hashCode == 2535353 && linkInfo.linkId != null && linkInfo.linkIdLength > 0)
		{
			href = new string(linkInfo.linkId);
			if (!href.StartsWith("href"))
			{
				return false;
			}
			if (href.StartsWith("href=\"") || href.StartsWith("href='"))
			{
				href = href.Substring(6, href.Length - 7);
			}
			else
			{
				href = href.Substring(5, href.Length - 6);
			}
			if (Uri.IsWellFormedUriString(href, UriKind.Absolute))
			{
				return true;
			}
		}
		return false;
	}

	public bool HandleKeyEvent(Event e)
	{
		return m_TextEditing.HandleKeyEvent(e) || m_TextSelecting.HandleKeyEvent(e);
	}

	public bool DeleteLineBack()
	{
		return m_TextEditing.DeleteLineBack();
	}

	public bool DeleteWordBack()
	{
		return m_TextEditing.DeleteWordBack();
	}

	public bool DeleteWordForward()
	{
		return m_TextEditing.DeleteWordForward();
	}

	public bool Delete()
	{
		return m_TextEditing.Delete();
	}

	public bool CanPaste()
	{
		return m_TextEditing.CanPaste();
	}

	public bool Backspace()
	{
		return m_TextEditing.Backspace();
	}

	public void SelectAll()
	{
		m_TextSelecting.SelectAll();
	}

	public void SelectNone()
	{
		m_TextSelecting.SelectNone();
	}

	public bool DeleteSelection()
	{
		return m_TextEditing.DeleteSelection();
	}

	public void ReplaceSelection(string replace)
	{
		m_TextEditing.ReplaceSelection(replace);
	}

	public void Insert(char c)
	{
		m_TextEditing.Insert(c);
	}

	public void MoveSelectionToAltCursor()
	{
		m_TextEditing.MoveSelectionToAltCursor();
	}

	public void MoveRight()
	{
		m_TextSelecting.MoveRight();
	}

	public void MoveLeft()
	{
		m_TextSelecting.MoveLeft();
	}

	public void MoveUp()
	{
		m_TextSelecting.MoveUp();
	}

	public void MoveDown()
	{
		m_TextSelecting.MoveDown();
	}

	public void MoveLineStart()
	{
		m_TextSelecting.MoveLineStart();
	}

	public void MoveLineEnd()
	{
		m_TextSelecting.MoveLineEnd();
	}

	public void MoveGraphicalLineStart()
	{
		m_TextSelecting.MoveGraphicalLineStart();
	}

	public void MoveGraphicalLineEnd()
	{
		m_TextSelecting.MoveGraphicalLineEnd();
	}

	public void MoveTextStart()
	{
		m_TextSelecting.MoveTextStart();
	}

	public void MoveTextEnd()
	{
		m_TextSelecting.MoveTextEnd();
	}

	public void MoveParagraphForward()
	{
		m_TextSelecting.MoveParagraphForward();
	}

	public void MoveParagraphBackward()
	{
		m_TextSelecting.MoveParagraphBackward();
	}

	public void MoveCursorToPosition(Vector2 cursorPosition)
	{
		MoveCursorToPosition_Internal(cursorPosition, Event.current.shift);
	}

	protected internal void MoveCursorToPosition_Internal(Vector2 cursorPosition, bool shift)
	{
		m_TextSelecting.MoveCursorToPosition_Internal(GetLocalCursorPosition(cursorPosition), shift);
	}

	public void MoveAltCursorToPosition(Vector2 cursorPosition)
	{
		m_TextSelecting.MoveAltCursorToPosition(GetLocalCursorPosition(cursorPosition));
	}

	public bool IsOverSelection(Vector2 cursorPosition)
	{
		return m_TextSelecting.IsOverSelection(GetLocalCursorPosition(cursorPosition));
	}

	public void SelectToPosition(Vector2 cursorPosition)
	{
		m_TextSelecting.SelectToPosition(GetLocalCursorPosition(cursorPosition));
	}

	private Vector2 GetLocalCursorPosition(Vector2 cursorPosition)
	{
		return cursorPosition - style.Internal_GetTextRectOffset(position, m_Content, new Vector2(m_TextHandle.preferredSize.x, (m_TextHandle.preferredSize.y > 0f) ? m_TextHandle.preferredSize.y : style.lineHeight)) + scrollOffset;
	}

	public void SelectLeft()
	{
		m_TextSelecting.SelectLeft();
	}

	public void SelectRight()
	{
		m_TextSelecting.SelectRight();
	}

	public void SelectUp()
	{
		m_TextSelecting.SelectUp();
	}

	public void SelectDown()
	{
		m_TextSelecting.SelectDown();
	}

	public void SelectTextEnd()
	{
		m_TextSelecting.SelectTextEnd();
	}

	public void SelectTextStart()
	{
		m_TextSelecting.SelectTextStart();
	}

	public void MouseDragSelectsWholeWords(bool on)
	{
		m_TextSelecting.MouseDragSelectsWholeWords(on);
	}

	public void DblClickSnap(DblClickSnapping snapping)
	{
		m_TextSelecting.DblClickSnap(snapping);
	}

	public void MoveWordRight()
	{
		m_TextSelecting.MoveWordRight();
	}

	public void MoveToStartOfNextWord()
	{
		m_TextSelecting.MoveToStartOfNextWord();
	}

	public void MoveToEndOfPreviousWord()
	{
		m_TextSelecting.MoveToEndOfPreviousWord();
	}

	public void SelectToStartOfNextWord()
	{
		m_TextSelecting.SelectToStartOfNextWord();
	}

	public void SelectToEndOfPreviousWord()
	{
		m_TextSelecting.SelectToEndOfPreviousWord();
	}

	public int FindStartOfNextWord(int p)
	{
		return m_TextSelecting.FindStartOfNextWord(p);
	}

	public void MoveWordLeft()
	{
		m_TextSelecting.MoveWordLeft();
	}

	public void SelectWordRight()
	{
		m_TextSelecting.SelectWordRight();
	}

	public void SelectWordLeft()
	{
		m_TextSelecting.SelectWordLeft();
	}

	public void ExpandSelectGraphicalLineStart()
	{
		m_TextSelecting.ExpandSelectGraphicalLineStart();
	}

	public void ExpandSelectGraphicalLineEnd()
	{
		m_TextSelecting.ExpandSelectGraphicalLineEnd();
	}

	public void SelectGraphicalLineStart()
	{
		m_TextSelecting.SelectGraphicalLineStart();
	}

	public void SelectGraphicalLineEnd()
	{
		m_TextSelecting.SelectGraphicalLineEnd();
	}

	public void SelectParagraphForward()
	{
		m_TextSelecting.SelectParagraphForward();
	}

	public void SelectParagraphBackward()
	{
		m_TextSelecting.SelectParagraphBackward();
	}

	public void SelectCurrentWord()
	{
		m_TextSelecting.SelectCurrentWord();
	}

	public void SelectCurrentParagraph()
	{
		m_TextSelecting.SelectCurrentParagraph();
	}

	public void UpdateScrollOffsetIfNeeded(Event evt)
	{
		if (evt.type != EventType.Repaint && evt.type != EventType.Layout)
		{
			UpdateScrollOffset();
		}
	}

	internal void UpdateTextHandle()
	{
		m_TextHandle = IMGUITextHandle.GetTextHandle(style, style.padding.Remove(position), textWithWhitespace, Color.white);
		m_TextHandle.AddToPermanentCacheAndGenerateMesh();
		m_TextEditing.textHandle = m_TextHandle;
		m_TextSelecting.textHandle = m_TextHandle;
	}

	[VisibleToOtherModules]
	internal void UpdateScrollOffset()
	{
		float num = scrollOffset.x;
		float num2 = scrollOffset.y;
		graphicalCursorPos = style.GetCursorPixelPosition(new Rect(0f, 0f, position.width, position.height), m_Content, m_TextSelecting.cursorIndexNoValidation);
		Rect rect = style.padding.Remove(position);
		Vector2 vector = graphicalCursorPos;
		vector.x -= style.padding.left;
		vector.y -= style.padding.top;
		Vector2 vector2 = (previousContentSize = style.GetPreferredSize(m_Content.textWithWhitespace, position));
		if (vector2.x < rect.width)
		{
			num = 0f;
		}
		else if (showCursor)
		{
			if (vector.x > scrollOffset.x + rect.width - 1f)
			{
				num = vector.x - rect.width + 1f;
			}
			else if (vector.x < scrollOffset.x)
			{
				num = Mathf.Max(vector.x, 0f);
			}
			else if (previousContentSize.x != vector2.x && vector.x < rect.x + Math.Abs(vector2.x + 1f - rect.width))
			{
				num = Mathf.Max(rect.width - vector.x, 0f);
			}
		}
		if (Mathf.Round(vector2.y) <= Mathf.Round(rect.height) || rect.height == 0f)
		{
			num2 = 0f;
		}
		else if (showCursor && Math.Abs(lastCursorPos.y - vector.y) > 0.05f)
		{
			if (vector.y + style.lineHeight > scrollOffset.y + rect.height)
			{
				num2 = vector.y - rect.height + style.lineHeight;
			}
			else if (vector.y < style.lineHeight + scrollOffset.y)
			{
				num2 = vector.y - style.lineHeight;
			}
		}
		if (scrollOffset.x != num || scrollOffset.y != num2)
		{
			scrollOffset = new Vector2(num, (num2 < 0f) ? 0f : num2);
		}
		lastCursorPos = vector;
	}

	public void DrawCursor(string newText)
	{
		string text = this.text;
		int cursorStringIndex = cursorIndex;
		if (GUIUtility.compositionString.Length > 0)
		{
			m_Content.text = newText.Substring(0, cursorIndex) + GUIUtility.compositionString + newText.Substring(selectIndex);
		}
		else
		{
			m_Content.text = newText;
		}
		graphicalCursorPos = style.GetCursorPixelPosition(position, m_Content, cursorStringIndex) + new Vector2(0f, style.lineHeight);
		Vector2 contentOffset = style.contentOffset;
		style.contentOffset -= scrollOffset;
		style.Internal_clipOffset = scrollOffset;
		GUIUtility.compositionCursorPos = GUIClip.UnclipToWindow(graphicalCursorPos - scrollOffset);
		if (GUIUtility.compositionString.Length > 0)
		{
			style.DrawWithTextSelection(position, m_Content, controlID, cursorIndex, cursorIndex + GUIUtility.compositionString.Length, drawSelectionAsComposition: true);
		}
		else
		{
			style.DrawWithTextSelection(position, m_Content, controlID, cursorIndex, selectIndex);
		}
		if (m_TextSelecting.iAltCursorPos != -1)
		{
			style.DrawCursor(position, m_Content, controlID, m_TextSelecting.iAltCursorPos);
		}
		style.contentOffset = contentOffset;
		style.Internal_clipOffset = Vector2.zero;
		m_Content.text = text;
	}

	public void SaveBackup()
	{
		oldText = text;
		oldPos = cursorIndex;
		oldSelectPos = selectIndex;
	}

	public void Undo()
	{
		m_Content.text = oldText;
		cursorIndex = oldPos;
		selectIndex = oldSelectPos;
	}

	public bool Cut()
	{
		if (isPasswordField)
		{
			return false;
		}
		bool result = m_TextEditing.Cut();
		UpdateTextHandle();
		return result;
	}

	public void Copy()
	{
		if (!isPasswordField)
		{
			m_TextSelecting.Copy();
		}
	}

	internal Rect[] GetHyperlinksRect()
	{
		Rect[] hyperlinkRects = style.GetHyperlinkRects(m_TextHandle, localPosition);
		for (int i = 0; i < hyperlinkRects.Length; i++)
		{
			hyperlinkRects[i].position -= scrollOffset;
		}
		return hyperlinkRects;
	}

	public bool Paste()
	{
		return m_TextEditing.Paste();
	}

	public void DetectFocusChange()
	{
		OnDetectFocusChange();
	}

	internal virtual void OnDetectFocusChange()
	{
		if (m_HasFocus && controlID != GUIUtility.keyboardControl)
		{
			OnLostFocus();
		}
		if (!m_HasFocus && controlID == GUIUtility.keyboardControl)
		{
			OnFocus();
		}
	}

	internal virtual void OnCursorIndexChange()
	{
		UpdateScrollOffset();
	}

	internal virtual void OnSelectIndexChange()
	{
		UpdateScrollOffset();
	}
}
