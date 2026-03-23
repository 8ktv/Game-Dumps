using System;
using System.Collections.Concurrent;
using System.Threading;
using UnityEngine;

namespace Mirror;

public static class ThreadLog
{
	private struct LogEntry
	{
		public int threadId;

		public LogType type;

		public string message;

		public string stackTrace;

		public LogEntry(int threadId, LogType type, string message, string stackTrace)
		{
			this.threadId = threadId;
			this.type = type;
			this.message = message;
			this.stackTrace = stackTrace;
		}
	}

	private static readonly ConcurrentQueue<LogEntry> logs = new ConcurrentQueue<LogEntry>();

	private static int mainThreadId;

	[RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
	private static void Initialize()
	{
		mainThreadId = Thread.CurrentThread.ManagedThreadId;
		Application.logMessageReceivedThreaded -= OnLog;
		Application.logMessageReceivedThreaded += OnLog;
		NetworkLoop.OnLateUpdate = (Action)Delegate.Remove(NetworkLoop.OnLateUpdate, new Action(OnLateUpdate));
		NetworkLoop.OnLateUpdate = (Action)Delegate.Combine(NetworkLoop.OnLateUpdate, new Action(OnLateUpdate));
		Debug.Log("ThreadLog initialized.");
	}

	private static bool IsMainThread()
	{
		return Thread.CurrentThread.ManagedThreadId == mainThreadId;
	}

	private static void OnLog(string message, string stackTrace, LogType type)
	{
		if (!IsMainThread())
		{
			logs.Enqueue(new LogEntry(Thread.CurrentThread.ManagedThreadId, type, message, stackTrace));
		}
	}

	private static void OnLateUpdate()
	{
		LogEntry result;
		while (logs.TryDequeue(out result))
		{
			switch (result.type)
			{
			case LogType.Log:
				Debug.Log($"[Thread{result.threadId}] {result.message}\n{result.stackTrace}");
				break;
			case LogType.Warning:
				Debug.LogWarning($"[Thread{result.threadId}] {result.message}\n{result.stackTrace}");
				break;
			case LogType.Error:
				Debug.LogError($"[Thread{result.threadId}] {result.message}\n{result.stackTrace}");
				break;
			case LogType.Exception:
				Debug.LogError($"[Thread{result.threadId}] {result.message}\n{result.stackTrace}");
				break;
			}
		}
	}
}
