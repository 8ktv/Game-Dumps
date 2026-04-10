using System;
using UnityEngine.Bindings;

namespace UnityEngine.UIElements;

public interface ITextSelection
{
	bool isSelectable { get; set; }

	[Obsolete("cursorColor is deprecated. Please use the corresponding USS property (--unity-cursor-color) instead.")]
	Color cursorColor { get; set; }

	[Obsolete("selectionColor is deprecated. Please use the corresponding USS property (--unity-selection-color) instead.")]
	Color selectionColor { get; set; }

	int cursorIndex { get; set; }

	bool doubleClickSelectsWord { get; set; }

	int selectIndex { get; set; }

	bool tripleClickSelectsLine { get; set; }

	bool selectAllOnFocus { get; set; }

	bool selectAllOnMouseUp { get; set; }

	Vector2 cursorPosition { get; }

	internal float lineHeightAtCursorPosition { get; }

	internal float cursorWidth { get; set; }

	event Action OnCursorIndexChange;

	event Action OnSelectIndexChange;

	bool HasSelection();

	void SelectAll();

	void SelectNone();

	void SelectRange(int cursorIndex, int selectionIndex);

	[VisibleToOtherModules(new string[] { "UnityEditor.QuickSearchModule" })]
	internal void MoveTextEnd();

	Vector2 GetCursorPositionFromStringIndex(int stringIndex);

	void MoveForward();

	void MoveBackward();

	void MoveToParagraphEnd();

	void MoveToParagraphStart();

	void MoveToEndOfPreviousWord();

	void MoveToStartOfNextWord();

	void MoveWordBackward();

	void MoveWordForward();
}
