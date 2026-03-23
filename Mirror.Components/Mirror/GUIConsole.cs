using System.Collections.Generic;
using UnityEngine;

namespace Mirror;

public class GUIConsole : MonoBehaviour
{
	public int height = 80;

	public int offsetY = 40;

	public int maxLogCount = 50;

	public bool showInEditor;

	private readonly Queue<LogEntry> log = new Queue<LogEntry>();

	[Tooltip("Hotkey to show/hide the console at runtime\nBack Quote is usually on the left above Tab\nChange with caution - F keys are generally already taken in Browsers")]
	public KeyCode hotKey = KeyCode.BackQuote;

	private bool visible;

	private Vector2 scroll = Vector2.zero;

	private bool show
	{
		get
		{
			if (Application.isEditor)
			{
				return showInEditor;
			}
			return true;
		}
	}

	private void Awake()
	{
		if (show)
		{
			Application.logMessageReceived += OnLog;
		}
	}

	private void OnLog(string message, string stackTrace, LogType type)
	{
		int num;
		if (type != LogType.Error && type != LogType.Exception)
		{
			num = ((type == LogType.Warning) ? 1 : 0);
			if (num == 0)
			{
				goto IL_0027;
			}
		}
		else
		{
			num = 1;
		}
		if (!string.IsNullOrWhiteSpace(stackTrace))
		{
			message = message + "\n" + stackTrace;
		}
		goto IL_0027;
		IL_0027:
		log.Enqueue(new LogEntry(message, type));
		if (log.Count > maxLogCount)
		{
			log.Dequeue();
		}
		if (num != 0)
		{
			visible = true;
		}
		scroll.y = float.MaxValue;
	}

	private void Update()
	{
		if (show && Input.GetKeyDown(hotKey))
		{
			visible = !visible;
		}
	}

	private void OnGUI()
	{
		if (!visible)
		{
			return;
		}
		int num = 320;
		GUILayout.BeginArea(new Rect(num, offsetY, Screen.width - num - 10, height));
		scroll = GUILayout.BeginScrollView(scroll, "Box", GUILayout.Width(Screen.width - num - 10), GUILayout.Height(height));
		foreach (LogEntry item in log)
		{
			if (item.type == LogType.Error || item.type == LogType.Exception)
			{
				GUI.color = Color.red;
			}
			else if (item.type == LogType.Warning)
			{
				GUI.color = Color.yellow;
			}
			GUILayout.Label(item.message);
			GUI.color = Color.white;
		}
		GUILayout.EndScrollView();
		GUILayout.EndArea();
	}
}
