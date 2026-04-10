namespace UnityEngine.UIElements;

public readonly struct HandleDragAndDropArgs
{
	private readonly DragAndDropArgs m_DragAndDropArgs;

	public Vector2 position { get; }

	public object target => m_DragAndDropArgs.target;

	public int insertAtIndex => m_DragAndDropArgs.insertAtIndex;

	public int parentId => m_DragAndDropArgs.parentId;

	public int childIndex => m_DragAndDropArgs.childIndex;

	public DragAndDropPosition dropPosition => m_DragAndDropArgs.dragAndDropPosition;

	public DragAndDropData dragAndDropData => m_DragAndDropArgs.dragAndDropData;

	internal EventModifiers modifiers => m_DragAndDropArgs.modifiers;

	internal HandleDragAndDropArgs(Vector2 position, DragAndDropArgs dragAndDropArgs)
	{
		this.position = position;
		m_DragAndDropArgs = dragAndDropArgs;
	}
}
