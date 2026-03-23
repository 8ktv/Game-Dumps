using System;
using System.Diagnostics;
using System.Threading;
using UnityEngine;
using UnityEngine.Profiling;

namespace Mirror;

public class WorkerThread
{
	private readonly Thread thread;

	protected volatile bool active;

	private readonly Stopwatch watch = new Stopwatch();

	public Action Init;

	public Func<bool> Tick;

	public Action Cleanup;

	public bool IsAlive => thread.IsAlive;

	public WorkerThread(string identifier)
	{
		WorkerThread workerThread = this;
		thread = new Thread((ThreadStart)delegate
		{
			workerThread.Guard(identifier);
		});
		thread.IsBackground = true;
	}

	public void Start()
	{
		if (thread.IsAlive)
		{
			UnityEngine.Debug.LogWarning("WorkerThread is still active, can't start it again.");
			return;
		}
		active = true;
		thread.Start();
	}

	public virtual void SignalStop()
	{
		active = false;
	}

	public bool StopBlocking(float timeout)
	{
		if (!thread.IsAlive)
		{
			return true;
		}
		watch.Restart();
		SignalStop();
		while (IsAlive)
		{
			Thread.Sleep(0);
			if (watch.Elapsed.TotalSeconds >= (double)timeout)
			{
				Interrupt();
				return false;
			}
		}
		return true;
	}

	public void Interrupt()
	{
		thread.Interrupt();
	}

	private void OnInit()
	{
		Init?.Invoke();
	}

	private bool OnTick()
	{
		return Tick?.Invoke() ?? false;
	}

	private void OnCleanup()
	{
		Cleanup?.Invoke();
	}

	public void Guard(string identifier)
	{
		try
		{
			UnityEngine.Debug.Log(identifier + ": started.");
			OnInit();
			while (active && OnTick())
			{
			}
		}
		catch (ThreadInterruptedException)
		{
			UnityEngine.Debug.Log(identifier + ": interrupted. That's okay.");
		}
		catch (ThreadAbortException)
		{
			UnityEngine.Debug.LogWarning(identifier + ": aborted. This may happen after domain reload. That's okay.");
		}
		catch (Exception exception)
		{
			UnityEngine.Debug.LogException(exception);
		}
		finally
		{
			active = false;
			OnCleanup();
			Profiler.EndThreadProfiling();
			UnityEngine.Debug.Log(identifier + ": ended.");
		}
	}
}
