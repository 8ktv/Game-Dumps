using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Pool;

public class DevConsoleGui : SingletonBehaviour<DevConsoleGui>
{
	public class LogMessage
	{
		public string message;

		public Color color;
	}

	private const string commandFieldControlName = "CommandField";

	private const float scrollWidth = 16f;

	[CVar("devConsoleHeight", "Height of the dev console as a fraction of the screen height.", "", false, true, resetOnSceneChangeOrCheatsDisabled = false)]
	public static float devConsoleHeight = 0.5f;

	[CVar("devConsoleMaxLogCount", "", "", false, true, resetOnSceneChangeOrCheatsDisabled = false)]
	public static int maxLogCount = 2048;

	public Action<string> CommandEntered;

	[Header("Attributes")]
	[SerializeField]
	private float singleElementHeight = 18f;

	[SerializeField]
	private float elementPadding = 2f;

	[SerializeField]
	private Color bgColor;

	[SerializeField]
	private Color logEvenColor;

	[SerializeField]
	private Color logOddColor;

	[SerializeField]
	private Color autoCompleteNormalColor;

	[SerializeField]
	private Color autoCompleteSelectedColor;

	[SerializeField]
	private Color autoCompleteHoverColor;

	[SerializeField]
	private Font font;

	[SerializeField]
	private int debugStartCount;

	[SerializeField]
	private int previousCommandListCapacity = 50;

	private string currentInput;

	private string lastInput;

	private bool inputChangedThisFrame;

	private bool isInputEmpty;

	private static readonly List<LogMessage> log = new List<LogMessage>();

	private readonly List<string> autoComplete = new List<string>();

	private List<string> previousCommands;

	private GUIStyle boxStyle = new GUIStyle();

	private Vector2 logScrollPosition;

	private Vector2 autoCompleteScrollPosition;

	private bool scrollAutoCompleteToSelected;

	private bool acquireFocus;

	private bool moveCursorToEnd;

	private int autoCompleteSelectedIndex = -1;

	private int previousCommandSelected = -1;

	private bool shownWelcomeMessage;

	private bool isEnabled = true;

	public static bool Active { get; private set; }

	public static event Action Activated;

	public static event Action Deactivated;

	protected override void Awake()
	{
		base.Awake();
		boxStyle.normal.textColor = Color.white;
		boxStyle.font = font;
		boxStyle.fontSize = 12;
		boxStyle.padding = new RectOffset(2, 2, 2, 2);
		boxStyle.normal.background = Texture2D.whiteTexture;
		previousCommands = new List<string>(previousCommandListCapacity);
		for (int i = 0; i < debugStartCount; i++)
		{
			AddLog("TEST", Color.cyan);
		}
	}

	public static void SetEnabled(bool isEnabled)
	{
		SingletonBehaviour<DevConsoleGui>.Instance.SetEnabledInternal(isEnabled);
	}

	private void SetEnabledInternal(bool isEnabled)
	{
		if (this.isEnabled != isEnabled)
		{
			this.isEnabled = isEnabled;
			SingletonBehaviour<DevConsoleGui>.Instance.gameObject.SetActive(isEnabled);
		}
	}

	private void OnEnable()
	{
		Application.logMessageReceived += HandleLog;
		if (shownWelcomeMessage)
		{
			Debug.Log(Resources.Load<TextAsset>("buildstring").text);
			shownWelcomeMessage = true;
		}
	}

	private void OnDisable()
	{
		Application.logMessageReceived -= HandleLog;
		if (Active)
		{
			Deactivate();
		}
	}

	private void Update()
	{
		inputChangedThisFrame = currentInput != lastInput;
		isInputEmpty = currentInput == string.Empty;
		if (!inputChangedThisFrame)
		{
			if (isInputEmpty)
			{
				autoComplete.Clear();
			}
		}
		else
		{
			HandleChangedInput();
		}
	}

	private void HandleChangedInput()
	{
		lastInput = currentInput;
		autoComplete.Clear();
		previousCommandSelected = -1;
		if (!isInputEmpty)
		{
			UpdateAutoComplete();
		}
	}

	private void UpdateAutoComplete()
	{
		List<string> value;
		using (CollectionPool<List<string>, string>.Get(out value))
		{
			DevConsole.GetCommands(value);
			foreach (string item in value)
			{
				if (item.Contains(currentInput, StringComparison.CurrentCultureIgnoreCase))
				{
					autoComplete.Add(item);
				}
			}
			autoComplete.Sort(CompareAutocompleteCommands);
			autoCompleteSelectedIndex = -1;
		}
	}

	private int CompareAutocompleteCommands(string a, string b)
	{
		bool flag = a.StartsWith(currentInput);
		bool flag2 = b.StartsWith(currentInput);
		if (flag != flag2)
		{
			if (!flag)
			{
				return 1;
			}
			return -1;
		}
		return string.Compare(a, b);
	}

	private void HandleLog(string condition, string stackTrace, LogType type)
	{
		Color col;
		switch (type)
		{
		case LogType.Assert:
		case LogType.Log:
			col = Color.white;
			break;
		case LogType.Warning:
			col = Color.yellow;
			break;
		case LogType.Error:
		case LogType.Exception:
			col = Color.red;
			break;
		default:
			col = Color.gray;
			break;
		}
		string[] array = condition.Split('\n');
		string text = DateTime.Now.ToString("[HH:mm:ss]");
		if (array.Length == 1)
		{
			AddLog(text + " " + condition, col);
			return;
		}
		AddLog(text, col);
		string[] array2 = array;
		foreach (string msg in array2)
		{
			AddLog(msg, col);
		}
	}

	[CCommand("clear", "Clears the console", false, false)]
	public static void ClearLog()
	{
		log.Clear();
	}

	public void AddLog(string msg, Color col)
	{
		log.Add(new LogMessage
		{
			message = msg,
			color = col
		});
		logScrollPosition.y = float.MaxValue;
		for (int i = 0; i < log.Count - maxLogCount; i++)
		{
			log.RemoveAt(0);
		}
	}

	public static void TryActivate()
	{
		if (!Active)
		{
			SingletonBehaviour<DevConsoleGui>.Instance.Activate();
		}
	}

	public static void TryDeactivate()
	{
		if (Active)
		{
			SingletonBehaviour<DevConsoleGui>.Instance.Deactivate();
		}
	}

	private void Activate()
	{
		if (isEnabled)
		{
			Active = true;
			currentInput = string.Empty;
			acquireFocus = true;
			autoCompleteSelectedIndex = -1;
			previousCommandSelected = -1;
			GUI.FocusControl(string.Empty);
			if (EventSystem.current != null)
			{
				EventSystem.current.SetSelectedGameObject(null);
			}
			DevConsoleGui.Activated?.Invoke();
		}
	}

	private void Deactivate()
	{
		Active = false;
		GUI.FocusControl(string.Empty);
		DevConsoleGui.Deactivated?.Invoke();
	}

	private void ToggleActivation()
	{
		if (Active)
		{
			Deactivate();
		}
		else
		{
			Activate();
		}
	}

	private void OnGUI()
	{
		Event current = Event.current;
		if (current.isKey && current.type == EventType.KeyDown)
		{
			HandleInputEvent(current);
		}
		if (current.type == EventType.Repaint)
		{
			HandleRepaintEvent();
		}
		if (Active)
		{
			DrawConsole(current);
		}
	}

	private void HandleInputEvent(Event inputEvent)
	{
		switch (inputEvent.keyCode)
		{
		case KeyCode.Return:
			ReturnPress();
			break;
		case KeyCode.Backslash:
		case KeyCode.BackQuote:
			ToggleActivation();
			break;
		case KeyCode.Escape:
			EscapePress();
			break;
		case KeyCode.Tab:
			TabPress(inputEvent);
			break;
		case KeyCode.DownArrow:
			DownPress(inputEvent);
			break;
		case KeyCode.UpArrow:
			UpPress(inputEvent);
			break;
		}
	}

	private void ReturnPress()
	{
		if (Active)
		{
			AddLog("> " + currentInput, Color.green);
			if (currentInput != string.Empty)
			{
				LogPreviousCommand(currentInput);
				previousCommandSelected = -1;
			}
			CommandEntered?.Invoke(currentInput);
			currentInput = string.Empty;
		}
	}

	private void EscapePress()
	{
		if (!isInputEmpty)
		{
			currentInput = (lastInput = string.Empty);
			previousCommandSelected = -1;
		}
	}

	private void TabPress(Event inputEvent)
	{
		if (Active && autoComplete.Count != 0)
		{
			NavigateAutoCompleteDown();
			GUI.FocusControl(string.Empty);
			if (EventSystem.current != null)
			{
				EventSystem.current.SetSelectedGameObject(null);
			}
			inputEvent.Use();
		}
	}

	private void DownPress(Event inputEvent)
	{
		if (Active)
		{
			if (autoComplete.Count == 0)
			{
				NavigatePreviousCommandsDown();
			}
			else
			{
				NavigateAutoCompleteDown();
			}
			inputEvent.Use();
		}
	}

	private void UpPress(Event inputEvent)
	{
		if (Active)
		{
			if (autoComplete.Count == 0)
			{
				NavigatePreviousCommandsUp();
			}
			else
			{
				NavigateAutoCompleteUp();
			}
			inputEvent.Use();
		}
	}

	private void LogPreviousCommand(string command)
	{
		if (previousCommands.Count == previousCommandListCapacity)
		{
			previousCommands.RemoveAt(0);
		}
		previousCommands.Add(command);
	}

	private void NavigateAutoCompleteDown()
	{
		autoCompleteSelectedIndex++;
		if (autoCompleteSelectedIndex >= autoComplete.Count)
		{
			autoCompleteSelectedIndex = 0;
		}
		lastInput = (currentInput = autoComplete[autoCompleteSelectedIndex] + " ");
		moveCursorToEnd = true;
		scrollAutoCompleteToSelected = true;
	}

	private void NavigateAutoCompleteUp()
	{
		autoCompleteSelectedIndex--;
		if (autoCompleteSelectedIndex < 0)
		{
			autoCompleteSelectedIndex = autoComplete.Count - 1;
		}
		lastInput = (currentInput = autoComplete[autoCompleteSelectedIndex] + " ");
		moveCursorToEnd = true;
		scrollAutoCompleteToSelected = true;
	}

	private void NavigatePreviousCommandsDown()
	{
		if (previousCommandSelected >= 0)
		{
			moveCursorToEnd = true;
			if (previousCommands.Count == 0)
			{
				previousCommandSelected = -1;
			}
			else if (previousCommandSelected == previousCommands.Count - 1)
			{
				previousCommandSelected = -1;
				lastInput = (currentInput = string.Empty);
			}
			else if (previousCommandSelected < previousCommands.Count)
			{
				previousCommandSelected++;
				lastInput = (currentInput = previousCommands[previousCommandSelected]);
			}
		}
	}

	private void NavigatePreviousCommandsUp()
	{
		if (previousCommands.Count == 0)
		{
			previousCommandSelected = -1;
			return;
		}
		if (previousCommandSelected < 0)
		{
			previousCommandSelected = previousCommands.Count - 1;
		}
		else if (previousCommandSelected > 0)
		{
			previousCommandSelected--;
		}
		lastInput = (currentInput = previousCommands[previousCommandSelected]);
		moveCursorToEnd = true;
	}

	private void HandleRepaintEvent()
	{
		if (acquireFocus)
		{
			AcquireFocus();
		}
		else if (moveCursorToEnd)
		{
			MoveCursorToEnd();
		}
	}

	private void AcquireFocus()
	{
		acquireFocus = false;
		GUI.FocusControl("CommandField");
	}

	private void MoveCursorToEnd()
	{
		TextEditor obj = (TextEditor)GUIUtility.GetStateObject(typeof(TextEditor), GUIUtility.keyboardControl);
		obj.SelectNone();
		obj.MoveTextEnd();
		moveCursorToEnd = false;
	}

	private void DrawConsole(Event guiEvent)
	{
		Rect position = new Rect
		{
			width = Screen.width,
			height = BMath.Round((float)Screen.height * devConsoleHeight / (singleElementHeight + elementPadding)) * (singleElementHeight + elementPadding),
			y = 0f
		};
		GUI.backgroundColor = bgColor;
		GUI.Box(position, string.Empty, boxStyle);
		position.height -= singleElementHeight + elementPadding;
		position.min = Vector2.zero;
		Rect viewRect = new Rect
		{
			width = position.width - 16f,
			height = BMath.Max(position.height, (singleElementHeight + elementPadding) * (float)log.Count)
		};
		GUI.backgroundColor = Color.white;
		logScrollPosition = GUI.BeginScrollView(position, logScrollPosition, viewRect, alwaysShowHorizontal: false, alwaysShowVertical: true);
		int num = BMath.CeilToInt(position.height / (singleElementHeight + elementPadding));
		int num2 = BMath.Clamp(BMath.FloorToInt(logScrollPosition.y / (singleElementHeight + elementPadding)), 0, log.Count);
		Rect position2 = new Rect
		{
			width = position.width,
			height = singleElementHeight,
			y = viewRect.yMin + elementPadding + (float)num2 * (singleElementHeight + elementPadding)
		};
		if (log.Count < num)
		{
			position2.y += (float)BMath.Max(0, num - log.Count) * (singleElementHeight + elementPadding) + elementPadding;
		}
		for (int i = num2; i < BMath.Clamp(num2 + num + 1, 0, log.Count); i++)
		{
			GUI.backgroundColor = ((i % 2 == 0) ? logEvenColor : logOddColor);
			GUI.contentColor = log[i].color;
			GUI.Label(position2, log[i].message, boxStyle);
			position2.y += singleElementHeight + elementPadding;
		}
		GUI.contentColor = Color.white;
		GUI.EndScrollView();
		Rect position3 = new Rect
		{
			width = position.width,
			height = singleElementHeight,
			y = position.yMax + elementPadding
		};
		GUI.SetNextControlName("CommandField");
		GUI.backgroundColor = bgColor;
		currentInput = GUI.TextField(position3, currentInput, boxStyle);
		float num3 = singleElementHeight * (float)autoComplete.Count;
		if (num3 == 0f)
		{
			return;
		}
		float b = (float)Screen.height - position3.yMax;
		Rect viewRect2 = new Rect
		{
			width = position.width - 16f,
			height = num3,
			y = position3.yMax
		};
		Rect position4 = new Rect
		{
			width = position.width,
			height = BMath.Min(num3, b),
			y = viewRect2.y
		};
		autoCompleteScrollPosition = GUI.BeginScrollView(position4, autoCompleteScrollPosition, viewRect2);
		if (scrollAutoCompleteToSelected)
		{
			float y = autoCompleteScrollPosition.y;
			float num4 = y + position4.height;
			float num5 = (float)autoCompleteSelectedIndex * singleElementHeight;
			float num6 = num5 + singleElementHeight;
			if (num4 < num6)
			{
				autoCompleteScrollPosition += (num6 - num4) * Vector2.up;
			}
			else if (y > num5)
			{
				autoCompleteScrollPosition += (num5 - y) * Vector2.up;
			}
			scrollAutoCompleteToSelected = false;
		}
		Rect rect = new Rect
		{
			width = viewRect2.width,
			height = singleElementHeight,
			y = viewRect2.y
		};
		for (int j = 0; j < autoComplete.Count; j++)
		{
			if (rect.Contains(guiEvent.mousePosition))
			{
				GUI.backgroundColor = autoCompleteHoverColor;
				if (guiEvent.isMouse && guiEvent.button == 0)
				{
					currentInput = autoComplete[j];
					acquireFocus = true;
					moveCursorToEnd = true;
					break;
				}
			}
			else if (j == autoCompleteSelectedIndex)
			{
				GUI.backgroundColor = autoCompleteSelectedColor;
			}
			else
			{
				GUI.backgroundColor = autoCompleteNormalColor;
			}
			Rect position5 = rect;
			position5.width = 200f;
			Rect position6 = rect;
			position6.width -= 200f;
			position6.x += 200f;
			string text = Regex.Replace(autoComplete[j], currentInput, "<color=#5cc71c>$&</color>", RegexOptions.IgnoreCase);
			string text2 = "<i>" + DevConsole.GetDescription(autoComplete[j]) + "</i>";
			GUI.Box(position5, text, boxStyle);
			GUI.Box(position6, text2, boxStyle);
			rect.y += singleElementHeight;
		}
		GUI.EndScrollView();
	}
}
