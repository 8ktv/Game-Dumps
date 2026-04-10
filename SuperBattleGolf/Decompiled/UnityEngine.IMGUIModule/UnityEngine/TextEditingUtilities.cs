using System;
using System.Collections.Generic;
using System.Globalization;
using UnityEngine.Bindings;
using UnityEngine.TextCore.Text;

namespace UnityEngine;

[VisibleToOtherModules(new string[] { "UnityEngine.UIElementsModule" })]
internal class TextEditingUtilities
{
	internal record struct KeyEvent(KeyCode key, EventModifiers modifiers);

	private TextSelectingUtilities m_TextSelectingUtility;

	internal TextHandle textHandle;

	private int m_CursorIndexSavedState = -1;

	[VisibleToOtherModules(new string[] { "UnityEngine.UIElementsModule" })]
	internal bool isCompositionActive;

	private bool m_UpdateImeWindowPosition;

	internal Action OnTextChanged;

	public bool multiline = false;

	private string m_Text;

	internal static readonly List<(KeyEvent keyEvent, TextEditOp operation)> s_GlobalKeyMappings = new List<(KeyEvent, TextEditOp)>
	{
		(new KeyEvent(KeyCode.LeftArrow, EventModifiers.FunctionKey), TextEditOp.MoveLeft),
		(new KeyEvent(KeyCode.RightArrow, EventModifiers.FunctionKey), TextEditOp.MoveRight),
		(new KeyEvent(KeyCode.UpArrow, EventModifiers.FunctionKey), TextEditOp.MoveUp),
		(new KeyEvent(KeyCode.DownArrow, EventModifiers.FunctionKey), TextEditOp.MoveDown),
		(new KeyEvent(KeyCode.Delete, EventModifiers.FunctionKey), TextEditOp.Delete),
		(new KeyEvent(KeyCode.Backspace, EventModifiers.FunctionKey), TextEditOp.Backspace),
		(new KeyEvent(KeyCode.Backspace, EventModifiers.Shift | EventModifiers.FunctionKey), TextEditOp.Backspace)
	};

	internal static readonly List<(KeyEvent keyEvent, TextEditOp operation)> s_MacKeyMappings = new List<(KeyEvent, TextEditOp)>
	{
		(new KeyEvent(KeyCode.LeftArrow, EventModifiers.Control | EventModifiers.FunctionKey), TextEditOp.MoveGraphicalLineStart),
		(new KeyEvent(KeyCode.RightArrow, EventModifiers.Control | EventModifiers.FunctionKey), TextEditOp.MoveGraphicalLineEnd),
		(new KeyEvent(KeyCode.LeftArrow, EventModifiers.Alt | EventModifiers.FunctionKey), TextEditOp.MoveWordLeft),
		(new KeyEvent(KeyCode.RightArrow, EventModifiers.Alt | EventModifiers.FunctionKey), TextEditOp.MoveWordRight),
		(new KeyEvent(KeyCode.UpArrow, EventModifiers.Alt | EventModifiers.FunctionKey), TextEditOp.MoveParagraphBackward),
		(new KeyEvent(KeyCode.DownArrow, EventModifiers.Alt | EventModifiers.FunctionKey), TextEditOp.MoveParagraphForward),
		(new KeyEvent(KeyCode.LeftArrow, EventModifiers.Command | EventModifiers.FunctionKey), TextEditOp.MoveGraphicalLineStart),
		(new KeyEvent(KeyCode.RightArrow, EventModifiers.Command | EventModifiers.FunctionKey), TextEditOp.MoveGraphicalLineEnd),
		(new KeyEvent(KeyCode.UpArrow, EventModifiers.Command | EventModifiers.FunctionKey), TextEditOp.MoveTextStart),
		(new KeyEvent(KeyCode.DownArrow, EventModifiers.Command | EventModifiers.FunctionKey), TextEditOp.MoveTextEnd),
		(new KeyEvent(KeyCode.X, EventModifiers.Command), TextEditOp.Cut),
		(new KeyEvent(KeyCode.V, EventModifiers.Command), TextEditOp.Paste),
		(new KeyEvent(KeyCode.D, EventModifiers.Control), TextEditOp.Delete),
		(new KeyEvent(KeyCode.H, EventModifiers.Control), TextEditOp.Backspace),
		(new KeyEvent(KeyCode.B, EventModifiers.Control), TextEditOp.MoveLeft),
		(new KeyEvent(KeyCode.F, EventModifiers.Control), TextEditOp.MoveRight),
		(new KeyEvent(KeyCode.A, EventModifiers.Control), TextEditOp.MoveLineStart),
		(new KeyEvent(KeyCode.E, EventModifiers.Control), TextEditOp.MoveLineEnd),
		(new KeyEvent(KeyCode.Delete, EventModifiers.Alt | EventModifiers.FunctionKey), TextEditOp.DeleteWordForward),
		(new KeyEvent(KeyCode.Backspace, EventModifiers.Alt | EventModifiers.FunctionKey), TextEditOp.DeleteWordBack),
		(new KeyEvent(KeyCode.Backspace, EventModifiers.Command | EventModifiers.FunctionKey), TextEditOp.DeleteLineBack)
	};

	internal static readonly List<(KeyEvent keyEvent, TextEditOp operation)> s_WindowsLinuxKeyMappings = new List<(KeyEvent, TextEditOp)>
	{
		(new KeyEvent(KeyCode.Home, EventModifiers.FunctionKey), TextEditOp.MoveGraphicalLineStart),
		(new KeyEvent(KeyCode.End, EventModifiers.FunctionKey), TextEditOp.MoveGraphicalLineEnd),
		(new KeyEvent(KeyCode.LeftArrow, EventModifiers.Command | EventModifiers.FunctionKey), TextEditOp.MoveWordLeft),
		(new KeyEvent(KeyCode.RightArrow, EventModifiers.Command | EventModifiers.FunctionKey), TextEditOp.MoveWordRight),
		(new KeyEvent(KeyCode.UpArrow, EventModifiers.Command | EventModifiers.FunctionKey), TextEditOp.MoveParagraphBackward),
		(new KeyEvent(KeyCode.DownArrow, EventModifiers.Command | EventModifiers.FunctionKey), TextEditOp.MoveParagraphForward),
		(new KeyEvent(KeyCode.LeftArrow, EventModifiers.Control | EventModifiers.FunctionKey), TextEditOp.MoveToEndOfPreviousWord),
		(new KeyEvent(KeyCode.RightArrow, EventModifiers.Control | EventModifiers.FunctionKey), TextEditOp.MoveToStartOfNextWord),
		(new KeyEvent(KeyCode.UpArrow, EventModifiers.Control | EventModifiers.FunctionKey), TextEditOp.MoveParagraphBackward),
		(new KeyEvent(KeyCode.DownArrow, EventModifiers.Control | EventModifiers.FunctionKey), TextEditOp.MoveParagraphForward),
		(new KeyEvent(KeyCode.Delete, EventModifiers.Control | EventModifiers.FunctionKey), TextEditOp.DeleteWordForward),
		(new KeyEvent(KeyCode.Backspace, EventModifiers.Control | EventModifiers.FunctionKey), TextEditOp.DeleteWordBack),
		(new KeyEvent(KeyCode.Backspace, EventModifiers.Command | EventModifiers.FunctionKey), TextEditOp.DeleteLineBack),
		(new KeyEvent(KeyCode.X, EventModifiers.Control), TextEditOp.Cut),
		(new KeyEvent(KeyCode.V, EventModifiers.Control), TextEditOp.Paste),
		(new KeyEvent(KeyCode.Delete, EventModifiers.Shift | EventModifiers.FunctionKey), TextEditOp.Cut),
		(new KeyEvent(KeyCode.Insert, EventModifiers.Shift | EventModifiers.FunctionKey), TextEditOp.Paste)
	};

	private char m_HighSurrogate;

	private bool hasSelection => m_TextSelectingUtility.hasSelection;

	private string SelectedText => m_TextSelectingUtility.selectedText;

	private int m_iAltCursorPos => m_TextSelectingUtility.iAltCursorPos;

	internal bool revealCursor
	{
		get
		{
			return m_TextSelectingUtility.revealCursor;
		}
		set
		{
			m_TextSelectingUtility.revealCursor = value;
		}
	}

	internal int stringCursorIndex
	{
		get
		{
			return textHandle.GetCorrespondingStringIndex(cursorIndex);
		}
		set
		{
			cursorIndex = textHandle.GetCorrespondingCodePointIndex(value);
		}
	}

	private int cursorIndex
	{
		get
		{
			return m_TextSelectingUtility.cursorIndex;
		}
		set
		{
			m_TextSelectingUtility.cursorIndex = value;
		}
	}

	private int cursorIndexNoValidation
	{
		get
		{
			return m_TextSelectingUtility.cursorIndexNoValidation;
		}
		set
		{
			m_TextSelectingUtility.cursorIndexNoValidation = value;
		}
	}

	private int selectIndexNoValidation
	{
		get
		{
			return m_TextSelectingUtility.selectIndexNoValidation;
		}
		set
		{
			m_TextSelectingUtility.selectIndexNoValidation = value;
		}
	}

	private int stringCursorIndexNoValidation => textHandle.GetCorrespondingStringIndex(m_TextSelectingUtility.cursorIndexNoValidation);

	internal int stringSelectIndex
	{
		get
		{
			return textHandle.GetCorrespondingStringIndex(selectIndex);
		}
		set
		{
			selectIndex = textHandle.GetCorrespondingCodePointIndex(value);
		}
	}

	private int selectIndex
	{
		get
		{
			return m_TextSelectingUtility.selectIndex;
		}
		set
		{
			m_TextSelectingUtility.selectIndex = value;
		}
	}

	public string text
	{
		get
		{
			return m_Text;
		}
		set
		{
			if (!(value == m_Text))
			{
				m_Text = value ?? string.Empty;
				OnTextChanged?.Invoke();
			}
		}
	}

	internal void SetTextWithoutNotify(string value)
	{
		m_Text = value;
	}

	public TextEditingUtilities(TextSelectingUtilities selectingUtilities, TextHandle textHandle, string text)
	{
		m_TextSelectingUtility = selectingUtilities;
		this.textHandle = textHandle;
		m_Text = text;
	}

	public bool UpdateImeState()
	{
		if (GUIUtility.compositionString.Length > 0)
		{
			if (!isCompositionActive)
			{
				m_UpdateImeWindowPosition = true;
				ReplaceSelection(string.Empty);
			}
			isCompositionActive = true;
		}
		else
		{
			isCompositionActive = false;
		}
		return isCompositionActive;
	}

	public bool ShouldUpdateImeWindowPosition()
	{
		return m_UpdateImeWindowPosition;
	}

	public void SetImeWindowPosition(Vector2 worldPosition)
	{
		Vector2 cursorPositionFromStringIndexUsingCharacterHeight = textHandle.GetCursorPositionFromStringIndexUsingCharacterHeight(cursorIndex);
		GUIUtility.compositionCursorPos = worldPosition + cursorPositionFromStringIndexUsingCharacterHeight;
	}

	public string GeneratePreviewString(bool richText)
	{
		RestoreCursorState();
		string compositionString = GUIUtility.compositionString;
		if (isCompositionActive)
		{
			return richText ? text.Insert(stringCursorIndex, "<u>" + compositionString + "</u>") : text.Insert(stringCursorIndex, compositionString);
		}
		return text;
	}

	public void EnableCursorPreviewState()
	{
		if (m_CursorIndexSavedState == -1)
		{
			m_CursorIndexSavedState = m_TextSelectingUtility.cursorIndexNoValidation;
			int num = (selectIndexNoValidation = m_CursorIndexSavedState + GUIUtility.compositionString.Length);
			cursorIndexNoValidation = num;
		}
	}

	public void RestoreCursorState()
	{
		if (m_CursorIndexSavedState != -1)
		{
			int num = (selectIndex = m_CursorIndexSavedState);
			cursorIndex = num;
			m_CursorIndexSavedState = -1;
		}
	}

	internal bool HandleKeyEvent(Event e)
	{
		return HandleKeyEvent(e.keyCode, e.modifiers);
	}

	[VisibleToOtherModules(new string[] { "UnityEngine.UIElementsModule" })]
	internal bool HandleKeyEvent(KeyCode key, EventModifiers modifiers)
	{
		TextEditOp? textEditOp = TextEditOpFromEnum(key, modifiers, SystemInfo.operatingSystemFamily == OperatingSystemFamily.MacOSX);
		if (textEditOp.HasValue)
		{
			PerformOperation(textEditOp.Value);
			return true;
		}
		return false;
	}

	internal static TextEditOp? TextEditOpFromEnum(KeyCode key, EventModifiers modifiers, bool IsMacOsFamily)
	{
		modifiers &= ~EventModifiers.CapsLock;
		KeyEvent keyEvent = new KeyEvent(key, modifiers);
		foreach (var s_GlobalKeyMapping in s_GlobalKeyMappings)
		{
			if (s_GlobalKeyMapping.keyEvent == keyEvent)
			{
				return s_GlobalKeyMapping.operation;
			}
		}
		foreach (var item in IsMacOsFamily ? s_MacKeyMappings : s_WindowsLinuxKeyMappings)
		{
			if (item.keyEvent == keyEvent)
			{
				return item.operation;
			}
		}
		return null;
	}

	private void PerformOperation(TextEditOp operation)
	{
		revealCursor = true;
		switch (operation)
		{
		case TextEditOp.MoveLeft:
			m_TextSelectingUtility.MoveLeft();
			break;
		case TextEditOp.MoveRight:
			m_TextSelectingUtility.MoveRight();
			break;
		case TextEditOp.MoveUp:
			m_TextSelectingUtility.MoveUp();
			break;
		case TextEditOp.MoveDown:
			m_TextSelectingUtility.MoveDown();
			break;
		case TextEditOp.MoveLineStart:
			m_TextSelectingUtility.MoveLineStart();
			break;
		case TextEditOp.MoveLineEnd:
			m_TextSelectingUtility.MoveLineEnd();
			break;
		case TextEditOp.MoveWordRight:
			m_TextSelectingUtility.MoveWordRight();
			break;
		case TextEditOp.MoveToStartOfNextWord:
			m_TextSelectingUtility.MoveToStartOfNextWord();
			break;
		case TextEditOp.MoveToEndOfPreviousWord:
			m_TextSelectingUtility.MoveToEndOfPreviousWord();
			break;
		case TextEditOp.MoveWordLeft:
			m_TextSelectingUtility.MoveWordLeft();
			break;
		case TextEditOp.MoveTextStart:
			m_TextSelectingUtility.MoveTextStart();
			break;
		case TextEditOp.MoveTextEnd:
			m_TextSelectingUtility.MoveTextEnd();
			break;
		case TextEditOp.MoveParagraphForward:
			m_TextSelectingUtility.MoveParagraphForward();
			break;
		case TextEditOp.MoveParagraphBackward:
			m_TextSelectingUtility.MoveParagraphBackward();
			break;
		case TextEditOp.MoveGraphicalLineStart:
			m_TextSelectingUtility.MoveGraphicalLineStart();
			break;
		case TextEditOp.MoveGraphicalLineEnd:
			m_TextSelectingUtility.MoveGraphicalLineEnd();
			break;
		case TextEditOp.Delete:
			Delete();
			break;
		case TextEditOp.Backspace:
			Backspace();
			break;
		case TextEditOp.Cut:
			Cut();
			break;
		case TextEditOp.Paste:
			Paste();
			break;
		case TextEditOp.DeleteWordBack:
			DeleteWordBack();
			break;
		case TextEditOp.DeleteLineBack:
			DeleteLineBack();
			break;
		case TextEditOp.DeleteWordForward:
			DeleteWordForward();
			break;
		default:
			Debug.Log("Unimplemented: " + operation);
			break;
		}
	}

	public bool DeleteLineBack()
	{
		RestoreCursorState();
		if (hasSelection)
		{
			DeleteSelection();
			return true;
		}
		if (textHandle.useAdvancedText)
		{
			int firstCharacterIndexOnLine = textHandle.GetFirstCharacterIndexOnLine(cursorIndex);
			if (firstCharacterIndexOnLine != cursorIndex)
			{
				text = text.Remove(firstCharacterIndexOnLine, stringCursorIndex - firstCharacterIndexOnLine);
				int num = (selectIndex = firstCharacterIndexOnLine);
				cursorIndex = num;
				return true;
			}
			return false;
		}
		int firstCharacterIndex = textHandle.GetLineInfoFromCharacterIndex(cursorIndex).firstCharacterIndex;
		int correspondingStringIndex = textHandle.GetCorrespondingStringIndex(firstCharacterIndex);
		if (firstCharacterIndex != cursorIndex)
		{
			text = text.Remove(correspondingStringIndex, stringCursorIndex - correspondingStringIndex);
			int num = (selectIndex = firstCharacterIndex);
			cursorIndex = num;
			return true;
		}
		return false;
	}

	public bool DeleteWordBack()
	{
		RestoreCursorState();
		if (hasSelection)
		{
			DeleteSelection();
			return true;
		}
		int num = m_TextSelectingUtility.FindEndOfPreviousWord(cursorIndex);
		if (cursorIndex != num)
		{
			int correspondingStringIndex = textHandle.GetCorrespondingStringIndex(num);
			text = text.Remove(correspondingStringIndex, stringCursorIndex - correspondingStringIndex);
			int num2 = (cursorIndex = num);
			selectIndex = num2;
			return true;
		}
		return false;
	}

	public bool DeleteWordForward()
	{
		RestoreCursorState();
		if (hasSelection)
		{
			DeleteSelection();
			return true;
		}
		int index = m_TextSelectingUtility.FindStartOfNextWord(cursorIndex);
		if (cursorIndex < text.Length)
		{
			int correspondingStringIndex = textHandle.GetCorrespondingStringIndex(index);
			text = text.Remove(stringCursorIndex, correspondingStringIndex - stringCursorIndex);
			return true;
		}
		return false;
	}

	public bool Delete()
	{
		RestoreCursorState();
		if (hasSelection)
		{
			DeleteSelection();
			return true;
		}
		if (stringCursorIndex < text.Length)
		{
			int count = ((!textHandle.useAdvancedText) ? textHandle.textInfo.textElementInfo[cursorIndex].stringLength : Mathf.Abs(textHandle.NextCodePointIndex(cursorIndex) - cursorIndex));
			text = text.Remove(stringCursorIndex, count);
			return true;
		}
		return false;
	}

	public bool Backspace()
	{
		RestoreCursorState();
		if (hasSelection)
		{
			DeleteSelection();
			return true;
		}
		if (cursorIndex > 0)
		{
			int num = m_TextSelectingUtility.PreviousCodePointIndex(cursorIndex);
			int num2 = ((!textHandle.useAdvancedText) ? textHandle.textInfo.textElementInfo[cursorIndex - 1].stringLength : Mathf.Abs(cursorIndex - num));
			text = text.Remove(stringCursorIndex - num2, num2);
			cursorIndex = (textHandle.useAdvancedText ? Math.Max(0, cursorIndex - num2) : num);
			selectIndex = (textHandle.useAdvancedText ? Math.Max(0, selectIndex - num2) : num);
			m_TextSelectingUtility.ClearCursorPos();
			return true;
		}
		return false;
	}

	public bool DeleteSelection()
	{
		if (cursorIndex == selectIndex)
		{
			return false;
		}
		if (cursorIndex < selectIndex)
		{
			text = text.Substring(0, stringCursorIndex) + text.Substring(stringSelectIndex, text.Length - stringSelectIndex);
			selectIndex = cursorIndex;
		}
		else
		{
			text = text.Substring(0, stringSelectIndex) + text.Substring(stringCursorIndex, text.Length - stringCursorIndex);
			cursorIndex = selectIndex;
		}
		m_TextSelectingUtility.ClearCursorPos();
		return true;
	}

	public void ReplaceSelection(string replace)
	{
		RestoreCursorState();
		DeleteSelection();
		text = text.Insert(stringCursorIndex, replace);
		int num = (textHandle.useAdvancedText ? replace.Length : new StringInfo(replace).LengthInTextElements);
		selectIndexNoValidation = (cursorIndexNoValidation += num);
		m_TextSelectingUtility.ClearCursorPos();
	}

	public bool Insert(char c)
	{
		if (char.IsHighSurrogate(c))
		{
			m_HighSurrogate = c;
			return false;
		}
		if (char.IsLowSurrogate(c))
		{
			char c2 = c;
			string text = new string(new char[2] { m_HighSurrogate, c2 });
			ReplaceSelection(text.ToString());
			return true;
		}
		ReplaceSelection(c.ToString());
		return true;
	}

	public void MoveSelectionToAltCursor()
	{
		RestoreCursorState();
		if (m_iAltCursorPos != -1)
		{
			int iAltCursorPos = m_iAltCursorPos;
			string selectedText = SelectedText;
			text = text.Insert(iAltCursorPos, selectedText);
			if (iAltCursorPos < cursorIndex)
			{
				cursorIndex += selectedText.Length;
				selectIndex += selectedText.Length;
			}
			DeleteSelection();
			int num = (cursorIndex = iAltCursorPos);
			selectIndex = num;
			m_TextSelectingUtility.ClearCursorPos();
		}
	}

	public bool CanPaste()
	{
		return GUIUtility.systemCopyBuffer.Length != 0;
	}

	public bool Cut()
	{
		m_TextSelectingUtility.Copy();
		return DeleteSelection();
	}

	public bool Paste()
	{
		RestoreCursorState();
		string text = GUIUtility.systemCopyBuffer;
		if (text != "")
		{
			if (!multiline)
			{
				text = ReplaceNewlinesWithSpaces(text);
			}
			ReplaceSelection(text);
			return true;
		}
		return false;
	}

	private static string ReplaceNewlinesWithSpaces(string value)
	{
		value = value.Replace("\r\n", " ");
		value = value.Replace('\n', ' ');
		value = value.Replace('\r', ' ');
		return value;
	}

	[VisibleToOtherModules(new string[] { "UnityEngine.UIElementsModule" })]
	internal void OnBlur()
	{
		revealCursor = false;
		isCompositionActive = false;
		RestoreCursorState();
		m_TextSelectingUtility.SelectNone();
	}

	[VisibleToOtherModules(new string[] { "UnityEngine.UIElementsModule" })]
	internal bool TouchScreenKeyboardShouldBeUsed()
	{
		RuntimePlatform platform = Application.platform;
		RuntimePlatform runtimePlatform = platform;
		RuntimePlatform runtimePlatform2 = runtimePlatform;
		if (runtimePlatform2 == RuntimePlatform.Android || (uint)(runtimePlatform2 - 17) <= 3u)
		{
			return !TouchScreenKeyboard.isInPlaceEditingAllowed;
		}
		return TouchScreenKeyboard.isSupported;
	}
}
