namespace UnityEngine.UIElements;

internal class KeyboardTextEditorEventHandler : TextEditorEventHandler
{
	internal bool m_Changed;

	internal bool m_ShouldInvokeUpdateValue;

	private const int k_LineFeed = 10;

	private const int k_Space = 32;

	public KeyboardTextEditorEventHandler(TextElement textElement, TextEditingUtilities editingUtilities)
		: base(textElement, editingUtilities)
	{
		editingUtilities.multiline = textElement.edition.multiline;
	}

	public override void HandleEventBubbleUp(EventBase evt)
	{
		base.HandleEventBubbleUp(evt);
		if (!(evt is KeyDownEvent evt2))
		{
			if (!(evt is ValidateCommandEvent evt3))
			{
				if (!(evt is ExecuteCommandEvent evt4))
				{
					if (!(evt is FocusEvent _))
					{
						if (!(evt is BlurEvent _2))
						{
							if (!(evt is NavigationMoveEvent evt5))
							{
								if (!(evt is NavigationSubmitEvent evt6))
								{
									if (!(evt is NavigationCancelEvent evt7))
									{
										if (evt is IMEEvent _3)
										{
											OnIMEInput(_3);
										}
									}
									else
									{
										OnNavigationEvent(evt7);
									}
								}
								else
								{
									OnNavigationEvent(evt6);
								}
							}
							else
							{
								OnNavigationEvent(evt5);
							}
						}
						else
						{
							OnBlur(_2);
						}
					}
					else
					{
						OnFocus(_);
					}
				}
				else
				{
					OnExecuteCommandEvent(evt4);
				}
			}
			else
			{
				OnValidateCommandEvent(evt3);
			}
		}
		else
		{
			OnKeyDown(evt2);
		}
	}

	private void OnFocus(FocusEvent _)
	{
		GUIUtility.imeCompositionMode = IMECompositionMode.On;
		textElement.edition.SaveValueAndText();
	}

	private void OnBlur(BlurEvent _)
	{
		GUIUtility.imeCompositionMode = IMECompositionMode.Auto;
	}

	private void OnIMEInput(IMEEvent _)
	{
		bool isCompositionActive = editingUtilities.isCompositionActive;
		if (editingUtilities.UpdateImeState() || isCompositionActive != editingUtilities.isCompositionActive)
		{
			UpdateLabel(generatePreview: true);
		}
	}

	private void OnKeyDown(KeyDownEvent evt)
	{
		if (!textElement.hasFocus)
		{
			return;
		}
		m_Changed = false;
		bool generatePreview = false;
		if (editingUtilities.HandleKeyEvent(evt.keyCode, evt.modifiers))
		{
			if (textElement.text != editingUtilities.text)
			{
				m_Changed = true;
			}
			evt.StopPropagation();
			goto IL_03cd;
		}
		char c = evt.character;
		if ((evt.actionKey && (!evt.altKey || c == '\0')) || (evt.keyCode >= KeyCode.F1 && evt.keyCode <= KeyCode.F15) || (evt.keyCode >= KeyCode.F16 && evt.keyCode <= KeyCode.F24) || (evt.altKey && c == '\0') || (c == '\t' && evt.keyCode == KeyCode.None && evt.modifiers == EventModifiers.None))
		{
			return;
		}
		if (evt.keyCode == KeyCode.Tab || (evt.keyCode == KeyCode.Tab && evt.character == '\t' && evt.modifiers == EventModifiers.Shift))
		{
			if (!textElement.edition.multiline || evt.shiftKey)
			{
				if (evt.ShouldSendNavigationMoveEvent())
				{
					textElement.focusController.FocusNextInDirection(textElement, evt.shiftKey ? VisualElementFocusChangeDirection.left : VisualElementFocusChangeDirection.right);
					evt.StopPropagation();
				}
				return;
			}
			if (!evt.ShouldSendNavigationMoveEvent())
			{
				return;
			}
		}
		if (!textElement.edition.multiline && (evt.keyCode == KeyCode.KeypadEnter || evt.keyCode == KeyCode.Return))
		{
			m_ShouldInvokeUpdateValue = true;
		}
		evt.StopPropagation();
		bool num;
		if (!textElement.edition.multiline)
		{
			if (c == '\n' || c == '\r' || c == '\n')
			{
				num = !evt.altKey;
				goto IL_025f;
			}
		}
		else if (c == '\n')
		{
			num = evt.shiftKey;
			goto IL_025f;
		}
		goto IL_028e;
		IL_025f:
		if (num)
		{
			ApplyTextIfNeeded();
			textElement.edition.MoveFocusToCompositeRoot?.Invoke();
			return;
		}
		goto IL_028e;
		IL_03cd:
		if (m_Changed || m_ShouldInvokeUpdateValue)
		{
			UpdateLabel(generatePreview);
		}
		textElement.edition.UpdateScrollOffset?.Invoke(evt.keyCode == KeyCode.Backspace);
		return;
		IL_028e:
		if (evt.keyCode == KeyCode.Escape)
		{
			textElement.edition.RestoreValueAndText();
			textElement.edition.UpdateValueFromText?.Invoke();
			textElement.edition.MoveFocusToCompositeRoot?.Invoke();
		}
		if (evt.keyCode == KeyCode.Tab)
		{
			c = '\t';
		}
		if (!textElement.edition.AcceptCharacter(c))
		{
			ApplyTextIfNeeded();
			return;
		}
		if (c >= ' ' || evt.keyCode == KeyCode.Tab || (textElement.edition.multiline && !evt.altKey && (c == '\n' || c == '\r' || c == '\n')))
		{
			m_Changed = editingUtilities.Insert(c);
		}
		else
		{
			bool isCompositionActive = editingUtilities.isCompositionActive;
			generatePreview = true;
			if (editingUtilities.UpdateImeState() || isCompositionActive != editingUtilities.isCompositionActive)
			{
				m_Changed = true;
			}
		}
		goto IL_03cd;
	}

	private void ApplyTextIfNeeded()
	{
		if (m_ShouldInvokeUpdateValue)
		{
			textElement.edition.UpdateValueFromText?.Invoke();
			m_ShouldInvokeUpdateValue = false;
		}
	}

	private void UpdateLabel(bool generatePreview)
	{
		string text = editingUtilities.text;
		bool flag = editingUtilities.UpdateImeState();
		if (flag && editingUtilities.ShouldUpdateImeWindowPosition())
		{
			editingUtilities.SetImeWindowPosition(new Vector2(textElement.worldBound.x, textElement.worldBound.y));
		}
		string value = (generatePreview ? editingUtilities.GeneratePreviewString(textElement.enableRichText) : editingUtilities.text);
		textElement.edition.UpdateText(value);
		if (!textElement.edition.isDelayed || m_ShouldInvokeUpdateValue)
		{
			textElement.edition.UpdateValueFromText?.Invoke();
			m_ShouldInvokeUpdateValue = false;
		}
		if (flag)
		{
			editingUtilities.text = text;
			editingUtilities.EnableCursorPreviewState();
		}
		textElement.uitkTextHandle.ComputeSettingsAndUpdate();
	}

	private void OnValidateCommandEvent(ValidateCommandEvent evt)
	{
		if (!textElement.hasFocus)
		{
			return;
		}
		switch (evt.commandName)
		{
		case "SelectAll":
			return;
		case "Cut":
			if (!textElement.selection.HasSelection())
			{
				return;
			}
			break;
		case "Paste":
			if (!editingUtilities.CanPaste())
			{
				return;
			}
			break;
		}
		evt.StopPropagation();
	}

	private void OnExecuteCommandEvent(ExecuteCommandEvent evt)
	{
		if (!textElement.hasFocus)
		{
			return;
		}
		m_Changed = false;
		bool flag = false;
		string text = editingUtilities.text;
		switch (evt.commandName)
		{
		case "OnLostFocus":
			evt.StopPropagation();
			return;
		case "Cut":
			editingUtilities.Cut();
			flag = true;
			evt.StopPropagation();
			break;
		case "Paste":
			editingUtilities.Paste();
			flag = true;
			evt.StopPropagation();
			break;
		case "Delete":
			editingUtilities.Cut();
			flag = true;
			evt.StopPropagation();
			break;
		}
		if (flag)
		{
			if (text != editingUtilities.text)
			{
				m_Changed = true;
			}
			evt.StopPropagation();
		}
		if (m_Changed)
		{
			UpdateLabel(generatePreview: true);
		}
		textElement.edition.UpdateScrollOffset?.Invoke(obj: false);
	}

	private void OnNavigationEvent<TEvent>(NavigationEventBase<TEvent> evt) where TEvent : NavigationEventBase<TEvent>, new()
	{
		if (evt.deviceType == NavigationDeviceType.Keyboard || evt.deviceType == NavigationDeviceType.Unknown)
		{
			evt.StopPropagation();
			textElement.focusController.IgnoreEvent(evt);
		}
	}
}
