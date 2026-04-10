using System;

namespace UnityEngine.UIElements;

internal class TextEditingManipulator
{
	private readonly TextElement m_TextElement;

	private TextEditorEventHandler m_EditingEventHandler;

	internal TextEditingUtilities editingUtilities;

	private bool m_TouchScreenTextFieldInitialized;

	private IVisualElementScheduledItem m_HardwareKeyboardPoller = null;

	internal TextEditorEventHandler editingEventHandler
	{
		get
		{
			return m_EditingEventHandler;
		}
		set
		{
			if (m_EditingEventHandler != value)
			{
				m_EditingEventHandler?.UnregisterCallbacksFromTarget(m_TextElement);
				m_EditingEventHandler = value;
				m_EditingEventHandler?.RegisterCallbacksOnTarget(m_TextElement);
			}
		}
	}

	private bool touchScreenTextFieldChanged => m_TouchScreenTextFieldInitialized != editingUtilities?.TouchScreenKeyboardShouldBeUsed();

	public TextEditingManipulator(TextElement textElement)
	{
		m_TextElement = textElement;
		editingUtilities = new TextEditingUtilities(textElement.selectingManipulator.m_SelectingUtilities, textElement.uitkTextHandle, textElement.text);
		InitTextEditorEventHandler();
	}

	public void Reset()
	{
		editingEventHandler = null;
	}

	private void InitTextEditorEventHandler()
	{
		m_TouchScreenTextFieldInitialized = editingUtilities?.TouchScreenKeyboardShouldBeUsed() ?? false;
		if (m_TouchScreenTextFieldInitialized)
		{
			editingEventHandler = new TouchScreenTextEditorEventHandler(m_TextElement, editingUtilities);
		}
		else
		{
			editingEventHandler = new KeyboardTextEditorEventHandler(m_TextElement, editingUtilities);
		}
	}

	internal void HandleEventBubbleUp(EventBase evt)
	{
		if (m_TextElement.edition.isReadOnly)
		{
			return;
		}
		if (evt is BlurEvent)
		{
			m_TextElement.uitkTextHandle.RemoveFromPermanentCache();
		}
		else if ((!(evt is PointerMoveEvent) && !(evt is MouseMoveEvent)) || m_TextElement.selectingManipulator.isClicking)
		{
			m_TextElement.uitkTextHandle.AddToPermanentCacheAndGenerateMesh();
		}
		if (!(evt is FocusInEvent))
		{
			if (evt is FocusOutEvent)
			{
				OnFocusOutEvent();
			}
		}
		else
		{
			OnFocusInEvent();
		}
		editingEventHandler?.HandleEventBubbleUp(evt);
	}

	private void OnFocusInEvent()
	{
		m_TextElement.edition.SaveValueAndText();
		m_TextElement.focusController.selectedTextElement = m_TextElement;
		if (touchScreenTextFieldChanged)
		{
			InitTextEditorEventHandler();
		}
		if (m_HardwareKeyboardPoller == null)
		{
			m_HardwareKeyboardPoller = m_TextElement.schedule.Execute((Action)delegate
			{
				if (touchScreenTextFieldChanged)
				{
					InitTextEditorEventHandler();
					m_TextElement.Blur();
				}
			}).Every(250L);
		}
		else
		{
			m_HardwareKeyboardPoller.Resume();
		}
	}

	private void OnFocusOutEvent()
	{
		m_HardwareKeyboardPoller?.Pause();
		editingUtilities.OnBlur();
	}
}
