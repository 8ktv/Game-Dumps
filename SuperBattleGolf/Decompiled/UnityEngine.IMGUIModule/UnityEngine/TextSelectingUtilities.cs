using System;
using System.Collections.Generic;
using UnityEngine.Bindings;
using UnityEngine.TextCore.Text;

namespace UnityEngine;

[VisibleToOtherModules(new string[] { "UnityEngine.UIElementsModule", "UnityEditor.UIBuilderModule" })]
internal class TextSelectingUtilities
{
	private enum CharacterType
	{
		LetterLike,
		Symbol,
		Symbol2,
		WhiteSpace,
		NewLine
	}

	private enum Direction
	{
		Forward,
		Backward
	}

	public TextEditor.DblClickSnapping dblClickSnap = TextEditor.DblClickSnapping.WORDS;

	public int iAltCursorPos = -1;

	public bool hasHorizontalCursorPos = false;

	private bool m_bJustSelected = false;

	private bool m_MouseDragSelectsWholeWords = false;

	private int m_DblClickInitPosStart = 0;

	private int m_DblClickInitPosEnd = 0;

	public TextHandle textHandle;

	private const int kMoveDownHeight = 5;

	private const char kNewLineChar = '\n';

	private bool m_RevealCursor;

	private int m_CursorIndex = 0;

	internal int m_SelectIndex = 0;

	private static Dictionary<Event, TextSelectOp> s_KeySelectOps;

	[VisibleToOtherModules(new string[] { "UnityEngine.UIElementsModule" })]
	internal Action OnCursorIndexChange;

	[VisibleToOtherModules(new string[] { "UnityEngine.UIElementsModule" })]
	internal Action OnSelectIndexChange;

	[VisibleToOtherModules(new string[] { "UnityEngine.UIElementsModule" })]
	internal Action OnRevealCursorChange;

	public bool hasSelection => cursorIndex != selectIndex;

	public bool revealCursor
	{
		get
		{
			return m_RevealCursor;
		}
		set
		{
			if (m_RevealCursor != value)
			{
				m_RevealCursor = value;
				OnRevealCursorChange?.Invoke();
			}
		}
	}

	private int m_CharacterCount => textHandle.characterCount;

	private int characterCount => (!textHandle.useAdvancedText && m_CharacterCount > 0 && textHandle.textInfo.textElementInfo[m_CharacterCount - 1].character == 8203) ? (m_CharacterCount - 1) : m_CharacterCount;

	private TextElementInfo[] m_TextElementInfos => textHandle.textInfo.textElementInfo;

	public int cursorIndex
	{
		get
		{
			return (!textHandle.IsPlaceholder) ? ClampTextIndex(m_CursorIndex) : 0;
		}
		set
		{
			if (m_CursorIndex != value)
			{
				m_CursorIndex = value;
				OnCursorIndexChange?.Invoke();
			}
		}
	}

	internal int cursorIndexNoValidation
	{
		get
		{
			return m_CursorIndex;
		}
		set
		{
			if (m_CursorIndex != value)
			{
				SetCursorIndexWithoutNotify(value);
				OnCursorIndexChange?.Invoke();
			}
		}
	}

	public int selectIndex
	{
		get
		{
			return (!textHandle.IsPlaceholder) ? ClampTextIndex(m_SelectIndex) : 0;
		}
		set
		{
			if (m_SelectIndex != value)
			{
				SetSelectIndexWithoutNotify(value);
				OnSelectIndexChange?.Invoke();
			}
		}
	}

	internal int selectIndexNoValidation
	{
		get
		{
			return m_SelectIndex;
		}
		set
		{
			if (m_SelectIndex != value)
			{
				SetSelectIndexWithoutNotify(value);
				OnSelectIndexChange?.Invoke();
			}
		}
	}

	public string selectedText
	{
		get
		{
			if (cursorIndex == selectIndex)
			{
				return "";
			}
			if (cursorIndex < selectIndex)
			{
				return textHandle.Substring(cursorIndex, selectIndex - cursorIndex);
			}
			return textHandle.Substring(selectIndex, cursorIndex - selectIndex);
		}
	}

	internal void SetCursorIndexWithoutNotify(int index)
	{
		m_CursorIndex = index;
	}

	internal void SetSelectIndexWithoutNotify(int index)
	{
		m_SelectIndex = index;
	}

	public TextSelectingUtilities(TextHandle textHandle)
	{
		this.textHandle = textHandle;
	}

	[VisibleToOtherModules(new string[] { "UnityEngine.UIElementsModule" })]
	internal bool HandleKeyEvent(Event e)
	{
		InitKeyActions();
		EventModifiers modifiers = e.modifiers;
		e.modifiers &= ~EventModifiers.CapsLock;
		if (s_KeySelectOps.ContainsKey(e))
		{
			TextSelectOp operation = s_KeySelectOps[e];
			PerformOperation(operation);
			e.modifiers = modifiers;
			return true;
		}
		e.modifiers = modifiers;
		return false;
	}

	private bool PerformOperation(TextSelectOp operation)
	{
		switch (operation)
		{
		case TextSelectOp.SelectLeft:
			SelectLeft();
			break;
		case TextSelectOp.SelectRight:
			SelectRight();
			break;
		case TextSelectOp.SelectUp:
			SelectUp();
			break;
		case TextSelectOp.SelectDown:
			SelectDown();
			break;
		case TextSelectOp.SelectWordRight:
			SelectWordRight();
			break;
		case TextSelectOp.SelectWordLeft:
			SelectWordLeft();
			break;
		case TextSelectOp.SelectToEndOfPreviousWord:
			SelectToEndOfPreviousWord();
			break;
		case TextSelectOp.SelectToStartOfNextWord:
			SelectToStartOfNextWord();
			break;
		case TextSelectOp.SelectTextStart:
			SelectTextStart();
			break;
		case TextSelectOp.SelectTextEnd:
			SelectTextEnd();
			break;
		case TextSelectOp.ExpandSelectGraphicalLineStart:
			ExpandSelectGraphicalLineStart();
			break;
		case TextSelectOp.ExpandSelectGraphicalLineEnd:
			ExpandSelectGraphicalLineEnd();
			break;
		case TextSelectOp.SelectParagraphForward:
			SelectParagraphForward();
			break;
		case TextSelectOp.SelectParagraphBackward:
			SelectParagraphBackward();
			break;
		case TextSelectOp.SelectGraphicalLineStart:
			SelectGraphicalLineStart();
			break;
		case TextSelectOp.SelectGraphicalLineEnd:
			SelectGraphicalLineEnd();
			break;
		case TextSelectOp.Copy:
			Copy();
			break;
		case TextSelectOp.SelectAll:
			SelectAll();
			break;
		case TextSelectOp.SelectNone:
			SelectNone();
			break;
		default:
			Debug.Log("Unimplemented: " + operation);
			break;
		}
		return false;
	}

	private static void MapKey(string key, TextSelectOp action)
	{
		s_KeySelectOps[Event.KeyboardEvent(key)] = action;
	}

	private void InitKeyActions()
	{
		if (s_KeySelectOps == null)
		{
			s_KeySelectOps = new Dictionary<Event, TextSelectOp>();
			MapKey("#left", TextSelectOp.SelectLeft);
			MapKey("#right", TextSelectOp.SelectRight);
			MapKey("#up", TextSelectOp.SelectUp);
			MapKey("#down", TextSelectOp.SelectDown);
			if (SystemInfo.operatingSystemFamily == OperatingSystemFamily.MacOSX)
			{
				MapKey("#home", TextSelectOp.SelectTextStart);
				MapKey("#end", TextSelectOp.SelectTextEnd);
				MapKey("#^left", TextSelectOp.ExpandSelectGraphicalLineStart);
				MapKey("#^right", TextSelectOp.ExpandSelectGraphicalLineEnd);
				MapKey("#^up", TextSelectOp.SelectParagraphBackward);
				MapKey("#^down", TextSelectOp.SelectParagraphForward);
				MapKey("#&left", TextSelectOp.SelectWordLeft);
				MapKey("#&right", TextSelectOp.SelectWordRight);
				MapKey("#&up", TextSelectOp.SelectParagraphBackward);
				MapKey("#&down", TextSelectOp.SelectParagraphForward);
				MapKey("#%left", TextSelectOp.ExpandSelectGraphicalLineStart);
				MapKey("#%right", TextSelectOp.ExpandSelectGraphicalLineEnd);
				MapKey("#%up", TextSelectOp.SelectTextStart);
				MapKey("#%down", TextSelectOp.SelectTextEnd);
				MapKey("%a", TextSelectOp.SelectAll);
				MapKey("%c", TextSelectOp.Copy);
			}
			else
			{
				MapKey("#^left", TextSelectOp.SelectToEndOfPreviousWord);
				MapKey("#^right", TextSelectOp.SelectToStartOfNextWord);
				MapKey("#^up", TextSelectOp.SelectParagraphBackward);
				MapKey("#^down", TextSelectOp.SelectParagraphForward);
				MapKey("#home", TextSelectOp.SelectGraphicalLineStart);
				MapKey("#end", TextSelectOp.SelectGraphicalLineEnd);
				MapKey("^a", TextSelectOp.SelectAll);
				MapKey("^c", TextSelectOp.Copy);
				MapKey("^insert", TextSelectOp.Copy);
			}
		}
	}

	public void ClearCursorPos()
	{
		hasHorizontalCursorPos = false;
		iAltCursorPos = -1;
	}

	public void OnFocus(bool selectAll = true)
	{
		if (selectAll)
		{
			SelectAll();
		}
		revealCursor = true;
	}

	public void SelectAll()
	{
		cursorIndex = 0;
		selectIndex = int.MaxValue;
		ClearCursorPos();
	}

	public void SelectNone()
	{
		selectIndex = cursorIndex;
		ClearCursorPos();
	}

	public void SelectLeft()
	{
		if (m_bJustSelected && cursorIndex > selectIndex)
		{
			int num = cursorIndex;
			cursorIndex = selectIndex;
			selectIndex = num;
		}
		m_bJustSelected = false;
		cursorIndex = PreviousCodePointIndex(cursorIndex);
	}

	public void SelectRight()
	{
		if (m_bJustSelected && cursorIndex < selectIndex)
		{
			int num = cursorIndex;
			cursorIndex = selectIndex;
			selectIndex = num;
		}
		m_bJustSelected = false;
		cursorIndex = NextCodePointIndex(cursorIndex);
	}

	public void SelectUp()
	{
		cursorIndex = textHandle.LineUpCharacterPosition(cursorIndex);
	}

	public void SelectDown()
	{
		cursorIndex = textHandle.LineDownCharacterPosition(cursorIndex);
	}

	public void SelectTextEnd()
	{
		cursorIndex = characterCount;
	}

	public void SelectTextStart()
	{
		cursorIndex = 0;
	}

	public void SelectToStartOfNextWord()
	{
		ClearCursorPos();
		cursorIndex = FindStartOfNextWord(cursorIndex);
	}

	public void SelectToEndOfPreviousWord()
	{
		ClearCursorPos();
		cursorIndex = FindEndOfPreviousWord(cursorIndex);
	}

	public void SelectWordRight()
	{
		ClearCursorPos();
		int num = selectIndex;
		if (cursorIndex < selectIndex)
		{
			selectIndex = cursorIndex;
			MoveWordRight();
			selectIndex = num;
			cursorIndex = ((cursorIndex < selectIndex) ? cursorIndex : selectIndex);
		}
		else
		{
			selectIndex = cursorIndex;
			MoveWordRight();
			selectIndex = num;
		}
	}

	public void SelectWordLeft()
	{
		ClearCursorPos();
		int num = selectIndex;
		if (cursorIndex > selectIndex)
		{
			selectIndex = cursorIndex;
			MoveWordLeft();
			selectIndex = num;
			cursorIndex = ((cursorIndex > selectIndex) ? cursorIndex : selectIndex);
		}
		else
		{
			selectIndex = cursorIndex;
			MoveWordLeft();
			selectIndex = num;
		}
	}

	public void SelectGraphicalLineStart()
	{
		ClearCursorPos();
		cursorIndex = GetGraphicalLineStart(cursorIndex);
	}

	public void SelectGraphicalLineEnd()
	{
		ClearCursorPos();
		cursorIndex = GetGraphicalLineEnd(cursorIndex);
	}

	public void SelectParagraphForward()
	{
		ClearCursorPos();
		bool flag = cursorIndex < selectIndex;
		if (textHandle.useAdvancedText)
		{
			int num = cursorIndex;
			textHandle.SelectToNextParagraph(ref num);
			cursorIndex = num;
		}
		else if (cursorIndex < characterCount)
		{
			cursorIndex = IndexOfEndOfLine(cursorIndex + 1);
			if (flag && cursorIndex > selectIndex)
			{
				cursorIndex = selectIndex;
			}
		}
	}

	public void SelectParagraphBackward()
	{
		ClearCursorPos();
		bool flag = cursorIndex > selectIndex;
		if (textHandle.useAdvancedText)
		{
			int num = cursorIndex;
			textHandle.SelectToPreviousParagraph(ref num);
			cursorIndex = num;
		}
		else if (cursorIndex > 1)
		{
			cursorIndex = textHandle.LastIndexOf('\n', cursorIndex - 2) + 1;
			if (flag && cursorIndex < selectIndex)
			{
				cursorIndex = selectIndex;
			}
		}
		else
		{
			int num2 = (cursorIndex = 0);
			selectIndex = num2;
		}
	}

	public void SelectCurrentWord()
	{
		int num = cursorIndex;
		if (textHandle.useAdvancedText)
		{
			int num2 = 0;
			int num3 = 0;
			textHandle.SelectCurrentWord(num, ref num2, ref num3);
			if (cursorIndex < selectIndex)
			{
				cursorIndex = num2;
				selectIndex = num3;
			}
			else
			{
				cursorIndex = num3;
				selectIndex = num2;
			}
		}
		else if (cursorIndex < selectIndex)
		{
			cursorIndex = FindEndOfClassification(num, Direction.Backward);
			selectIndex = FindEndOfClassification(num, Direction.Forward);
		}
		else
		{
			cursorIndex = FindEndOfClassification(num, Direction.Forward);
			selectIndex = FindEndOfClassification(num, Direction.Backward);
		}
		ClearCursorPos();
		m_bJustSelected = true;
	}

	public void SelectCurrentParagraph()
	{
		ClearCursorPos();
		int num = characterCount;
		if (textHandle.useAdvancedText)
		{
			int num2 = cursorIndex;
			int num3 = selectIndex;
			textHandle.SelectCurrentParagraph(ref num2, ref num3);
			cursorIndex = num2;
			selectIndex = num3;
			return;
		}
		if (cursorIndex < num)
		{
			cursorIndex = IndexOfEndOfLine(cursorIndex);
		}
		if (selectIndex != 0)
		{
			selectIndex = textHandle.LastIndexOf('\n', selectIndex - 1) + 1;
		}
	}

	public void MoveRight()
	{
		ClearCursorPos();
		if (selectIndex == cursorIndex)
		{
			cursorIndex = NextCodePointIndex(cursorIndex);
			selectIndex = cursorIndex;
		}
		else if (selectIndex > cursorIndex)
		{
			cursorIndex = selectIndex;
		}
		else
		{
			selectIndex = cursorIndex;
		}
	}

	public void MoveLeft()
	{
		if (selectIndex == cursorIndex)
		{
			cursorIndex = PreviousCodePointIndex(cursorIndex);
			selectIndex = cursorIndex;
		}
		else if (selectIndex > cursorIndex)
		{
			selectIndex = cursorIndex;
		}
		else
		{
			cursorIndex = selectIndex;
		}
		ClearCursorPos();
	}

	public void MoveUp()
	{
		if (selectIndex < cursorIndex)
		{
			selectIndex = cursorIndex;
		}
		else
		{
			cursorIndex = selectIndex;
		}
		int num = (selectIndex = textHandle.LineUpCharacterPosition(cursorIndex));
		cursorIndex = num;
		if (cursorIndex <= 0)
		{
			ClearCursorPos();
		}
	}

	public void MoveDown()
	{
		if (selectIndex > cursorIndex)
		{
			selectIndex = cursorIndex;
		}
		else
		{
			cursorIndex = selectIndex;
		}
		int num = (selectIndex = textHandle.LineDownCharacterPosition(cursorIndex));
		cursorIndex = num;
		if (cursorIndex == characterCount)
		{
			ClearCursorPos();
		}
	}

	public void MoveLineStart()
	{
		int num = ((selectIndex < cursorIndex) ? selectIndex : cursorIndex);
		int num2 = num;
		int num3;
		while (num2-- != 0)
		{
			if (m_TextElementInfos[num2].character == 10)
			{
				num3 = (cursorIndex = num2 + 1);
				selectIndex = num3;
				return;
			}
		}
		num3 = (cursorIndex = 0);
		selectIndex = num3;
	}

	public void MoveLineEnd()
	{
		int num = ((selectIndex > cursorIndex) ? selectIndex : cursorIndex);
		int i = num;
		int num2;
		int num3;
		for (num2 = characterCount; i < num2; i++)
		{
			if (m_TextElementInfos[i].character == 10)
			{
				num3 = (cursorIndex = i);
				selectIndex = num3;
				return;
			}
		}
		num3 = (cursorIndex = num2);
		selectIndex = num3;
	}

	public void MoveGraphicalLineStart()
	{
		int num = (selectIndex = GetGraphicalLineStart((cursorIndex < selectIndex) ? cursorIndex : selectIndex));
		cursorIndex = num;
	}

	public void MoveGraphicalLineEnd()
	{
		int num = (selectIndex = GetGraphicalLineEnd((cursorIndex > selectIndex) ? cursorIndex : selectIndex));
		cursorIndex = num;
	}

	public void MoveTextStart()
	{
		int num = (cursorIndex = 0);
		selectIndex = num;
	}

	public void MoveTextEnd()
	{
		int num = (cursorIndex = characterCount);
		selectIndex = num;
	}

	public void MoveParagraphForward()
	{
		if (textHandle.useAdvancedText)
		{
			int num = cursorIndex;
			textHandle.SelectToNextParagraph(ref num);
			int num2 = (selectIndex = num);
			cursorIndex = num2;
			return;
		}
		cursorIndex = ((cursorIndex > selectIndex) ? cursorIndex : selectIndex);
		if (cursorIndex < characterCount)
		{
			int num2 = (cursorIndex = IndexOfEndOfLine(cursorIndex + 1));
			selectIndex = num2;
		}
	}

	public void MoveParagraphBackward()
	{
		if (textHandle.useAdvancedText)
		{
			int num = cursorIndex;
			textHandle.SelectToPreviousParagraph(ref num);
			int num2 = (selectIndex = num);
			cursorIndex = num2;
			return;
		}
		cursorIndex = ((cursorIndex < selectIndex) ? cursorIndex : selectIndex);
		if (cursorIndex > 1)
		{
			int num2 = (cursorIndex = textHandle.LastIndexOf('\n', cursorIndex - 2) + 1);
			selectIndex = num2;
		}
		else
		{
			int num2 = (cursorIndex = 0);
			selectIndex = num2;
		}
	}

	public void MoveWordRight()
	{
		cursorIndex = ((cursorIndex > selectIndex) ? cursorIndex : selectIndex);
		if (textHandle.useAdvancedText)
		{
			int num = (selectIndex = FindStartOfNextWord(cursorIndex));
			cursorIndex = num;
		}
		else
		{
			int num = (selectIndex = FindNextSeperator(cursorIndex));
			cursorIndex = num;
		}
		ClearCursorPos();
	}

	public void MoveToStartOfNextWord()
	{
		ClearCursorPos();
		if (cursorIndex != selectIndex)
		{
			MoveRight();
			return;
		}
		int num = (selectIndex = FindStartOfNextWord(cursorIndex));
		cursorIndex = num;
	}

	public void MoveToEndOfPreviousWord()
	{
		ClearCursorPos();
		if (cursorIndex != selectIndex)
		{
			MoveLeft();
			return;
		}
		int num = (selectIndex = FindEndOfPreviousWord(cursorIndex));
		cursorIndex = num;
	}

	public void MoveWordLeft()
	{
		cursorIndex = ((cursorIndex < selectIndex) ? cursorIndex : selectIndex);
		if (textHandle.useAdvancedText)
		{
			cursorIndex = FindEndOfPreviousWord(cursorIndex);
		}
		else
		{
			cursorIndex = FindPrevSeperator(cursorIndex);
		}
		selectIndex = cursorIndex;
	}

	public void MouseDragSelectsWholeWords(bool on)
	{
		m_MouseDragSelectsWholeWords = on;
		m_DblClickInitPosStart = ((cursorIndex < selectIndex) ? cursorIndex : selectIndex);
		m_DblClickInitPosEnd = ((cursorIndex < selectIndex) ? selectIndex : cursorIndex);
	}

	public void ExpandSelectGraphicalLineStart()
	{
		ClearCursorPos();
		if (cursorIndex < selectIndex)
		{
			cursorIndex = GetGraphicalLineStart(cursorIndex);
			return;
		}
		int num = cursorIndex;
		cursorIndex = GetGraphicalLineStart(selectIndex);
		selectIndex = num;
	}

	public void ExpandSelectGraphicalLineEnd()
	{
		ClearCursorPos();
		if (cursorIndex > selectIndex)
		{
			cursorIndex = GetGraphicalLineEnd(cursorIndex);
			return;
		}
		int num = cursorIndex;
		cursorIndex = GetGraphicalLineEnd(selectIndex);
		selectIndex = num;
	}

	public void DblClickSnap(TextEditor.DblClickSnapping snapping)
	{
		dblClickSnap = snapping;
	}

	protected internal void MoveCursorToPosition_Internal(Vector2 cursorPosition, bool shift)
	{
		selectIndex = textHandle.GetCursorIndexFromPosition(cursorPosition);
		if (!shift)
		{
			cursorIndex = selectIndex;
		}
	}

	protected internal void MoveAltCursorToPosition(Vector2 cursorPosition)
	{
		if (cursorIndex == 0 && selectIndex == characterCount)
		{
			iAltCursorPos = -1;
			return;
		}
		int cursorIndexFromPosition = textHandle.GetCursorIndexFromPosition(cursorPosition);
		iAltCursorPos = Mathf.Min(characterCount, cursorIndexFromPosition);
	}

	protected internal bool IsOverSelection(Vector2 cursorPosition)
	{
		int cursorIndexFromPosition = textHandle.GetCursorIndexFromPosition(cursorPosition);
		return cursorIndexFromPosition < Mathf.Max(cursorIndex, selectIndex) && cursorIndexFromPosition > Mathf.Min(cursorIndex, selectIndex);
	}

	public void SelectToPosition(Vector2 cursorPosition)
	{
		if (characterCount == 0)
		{
			return;
		}
		if (!m_MouseDragSelectsWholeWords)
		{
			cursorIndex = textHandle.GetCursorIndexFromPosition(cursorPosition);
			return;
		}
		int cursorIndexFromPosition = textHandle.GetCursorIndexFromPosition(cursorPosition);
		if (dblClickSnap == TextEditor.DblClickSnapping.WORDS)
		{
			if (cursorIndexFromPosition <= m_DblClickInitPosStart)
			{
				if (textHandle.useAdvancedText)
				{
					selectIndex = Mathf.Max(selectIndex, cursorIndex);
					cursorIndex = textHandle.GetEndOfPreviousWord(cursorIndexFromPosition);
				}
				else
				{
					cursorIndex = FindEndOfClassification(cursorIndexFromPosition, Direction.Backward);
					selectIndex = FindEndOfClassification(m_DblClickInitPosEnd - 1, Direction.Forward);
				}
			}
			else if (cursorIndexFromPosition >= m_DblClickInitPosEnd)
			{
				if (textHandle.useAdvancedText)
				{
					selectIndex = Mathf.Min(selectIndex, cursorIndex);
					cursorIndex = textHandle.GetStartOfNextWord(cursorIndexFromPosition - 1);
				}
				else
				{
					cursorIndex = FindEndOfClassification(cursorIndexFromPosition - 1, Direction.Forward);
					selectIndex = FindEndOfClassification(m_DblClickInitPosStart + 1, Direction.Backward);
				}
			}
			else
			{
				cursorIndex = m_DblClickInitPosStart;
				selectIndex = m_DblClickInitPosEnd;
			}
		}
		else if ((!textHandle.useAdvancedText && cursorIndexFromPosition <= m_DblClickInitPosStart) || (textHandle.useAdvancedText && cursorIndexFromPosition < m_DblClickInitPosStart))
		{
			if (textHandle.useAdvancedText)
			{
				int num = cursorIndexFromPosition;
				textHandle.SelectToStartOfParagraph(ref num);
				selectIndex = num;
				return;
			}
			if (cursorIndexFromPosition > 0)
			{
				cursorIndex = textHandle.LastIndexOf('\n', Mathf.Max(0, cursorIndexFromPosition - 1)) + 1;
			}
			else
			{
				cursorIndex = 0;
			}
			selectIndex = textHandle.LastIndexOf('\n', Mathf.Min(characterCount - 1, m_DblClickInitPosEnd + 1));
		}
		else if (cursorIndexFromPosition >= m_DblClickInitPosEnd)
		{
			if (textHandle.useAdvancedText)
			{
				int num2 = cursorIndexFromPosition;
				textHandle.SelectToEndOfParagraph(ref num2);
				cursorIndex = num2;
				return;
			}
			if (cursorIndexFromPosition < characterCount)
			{
				cursorIndex = IndexOfEndOfLine(cursorIndexFromPosition);
			}
			else
			{
				cursorIndex = characterCount;
			}
			selectIndex = textHandle.LastIndexOf('\n', Mathf.Max(0, m_DblClickInitPosEnd - 2)) + 1;
		}
		else if (textHandle.useAdvancedText)
		{
			cursorIndex = m_DblClickInitPosEnd;
			selectIndex = m_DblClickInitPosStart;
		}
		else
		{
			cursorIndex = m_DblClickInitPosStart;
			selectIndex = m_DblClickInitPosEnd;
		}
	}

	private int FindNextSeperator(int startPos)
	{
		int num = characterCount;
		while (startPos < num && ClassifyChar(startPos) != CharacterType.LetterLike)
		{
			startPos = NextCodePointIndex(startPos);
		}
		while (startPos < num && ClassifyChar(startPos) == CharacterType.LetterLike)
		{
			startPos = NextCodePointIndex(startPos);
		}
		return startPos;
	}

	private int FindPrevSeperator(int startPos)
	{
		startPos = PreviousCodePointIndex(startPos);
		while (startPos > 0 && ClassifyChar(startPos) != CharacterType.LetterLike)
		{
			startPos = PreviousCodePointIndex(startPos);
		}
		if (startPos == 0)
		{
			return 0;
		}
		while (startPos > 0 && ClassifyChar(startPos) == CharacterType.LetterLike)
		{
			startPos = PreviousCodePointIndex(startPos);
		}
		if (ClassifyChar(startPos) == CharacterType.LetterLike)
		{
			return startPos;
		}
		return NextCodePointIndex(startPos);
	}

	public int FindStartOfNextWord(int p)
	{
		if (textHandle.useAdvancedText)
		{
			return textHandle.GetStartOfNextWord(p);
		}
		int num = characterCount;
		if (p == num)
		{
			return p;
		}
		CharacterType characterType = ClassifyChar(p);
		if (characterType != CharacterType.WhiteSpace)
		{
			p = NextCodePointIndex(p);
			while (p < num && ClassifyChar(p) == characterType)
			{
				p = NextCodePointIndex(p);
			}
		}
		else if (m_TextElementInfos[p].character == 9 || m_TextElementInfos[p].character == 10)
		{
			return NextCodePointIndex(p);
		}
		if (p == num)
		{
			return p;
		}
		if (m_TextElementInfos[p].character == 32)
		{
			while (p < num && ClassifyChar(p) == CharacterType.WhiteSpace)
			{
				p = NextCodePointIndex(p);
			}
		}
		else if (m_TextElementInfos[p].character == 9 || m_TextElementInfos[p].character == 10)
		{
			return p;
		}
		return p;
	}

	public int FindEndOfPreviousWord(int p)
	{
		if (textHandle.useAdvancedText)
		{
			return textHandle.GetEndOfPreviousWord(p);
		}
		if (p == 0)
		{
			return p;
		}
		p = PreviousCodePointIndex(p);
		while (p > 0 && m_TextElementInfos[p].character == 32)
		{
			p = PreviousCodePointIndex(p);
		}
		CharacterType characterType = ClassifyChar(p);
		if (characterType != CharacterType.WhiteSpace)
		{
			while (p > 0 && ClassifyChar(PreviousCodePointIndex(p)) == characterType)
			{
				p = PreviousCodePointIndex(p);
			}
		}
		return p;
	}

	private int FindEndOfClassification(int p, Direction dir)
	{
		if (characterCount == 0)
		{
			return 0;
		}
		if (p >= characterCount)
		{
			p = characterCount - 1;
		}
		CharacterType characterType = ClassifyChar(p);
		if (characterType == CharacterType.NewLine)
		{
			return p;
		}
		do
		{
			switch (dir)
			{
			case Direction.Backward:
				p = PreviousCodePointIndex(p);
				if (p == 0)
				{
					return (ClassifyChar(0) != characterType) ? NextCodePointIndex(0) : 0;
				}
				break;
			case Direction.Forward:
				p = NextCodePointIndex(p);
				if (p >= characterCount)
				{
					return characterCount;
				}
				break;
			}
		}
		while (ClassifyChar(p) == characterType);
		if (dir == Direction.Forward)
		{
			return p;
		}
		return NextCodePointIndex(p);
	}

	private int ClampTextIndex(int index)
	{
		return Mathf.Clamp(index, 0, characterCount);
	}

	private int IndexOfEndOfLine(int startIndex)
	{
		int num = textHandle.IndexOf('\n', startIndex);
		return (num != -1) ? num : characterCount;
	}

	public int PreviousCodePointIndex(int index)
	{
		if (textHandle.useAdvancedText)
		{
			return textHandle.PreviousCodePointIndex(index);
		}
		if (index > 0)
		{
			index--;
		}
		return index;
	}

	public int NextCodePointIndex(int index)
	{
		if (textHandle.useAdvancedText)
		{
			return textHandle.NextCodePointIndex(index);
		}
		if (index < characterCount)
		{
			index++;
		}
		return index;
	}

	private int GetGraphicalLineStart(int p)
	{
		return textHandle.GetFirstCharacterIndexOnLine(p);
	}

	private int GetGraphicalLineEnd(int p)
	{
		return textHandle.GetLastCharacterIndexOnLine(p);
	}

	public void Copy()
	{
		if (selectIndex != cursorIndex)
		{
			GUIUtility.systemCopyBuffer = selectedText;
		}
	}

	private CharacterType ClassifyChar(int index)
	{
		char c = (char)m_TextElementInfos[index].character;
		if (c == '\n')
		{
			return CharacterType.NewLine;
		}
		if (char.IsWhiteSpace(c))
		{
			return CharacterType.WhiteSpace;
		}
		if (char.IsLetterOrDigit(c) || m_TextElementInfos[index].character == 39)
		{
			return CharacterType.LetterLike;
		}
		return CharacterType.Symbol;
	}
}
