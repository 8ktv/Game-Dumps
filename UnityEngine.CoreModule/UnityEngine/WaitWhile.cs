using System;
using System.Runtime.CompilerServices;

namespace UnityEngine;

public sealed class WaitWhile : CustomYieldInstruction
{
	private readonly Func<bool> m_Predicate;

	private readonly Action m_TimeoutCallback;

	private readonly WaitTimeoutMode m_TimeoutMode;

	private readonly double m_MaxExecutionTime = -1.0;

	public override bool keepWaiting
	{
		get
		{
			if (m_MaxExecutionTime == -1.0)
			{
				return m_Predicate();
			}
			if (GetTime() > m_MaxExecutionTime)
			{
				m_TimeoutCallback();
				return false;
			}
			return m_Predicate();
		}
	}

	public WaitWhile(Func<bool> predicate)
	{
		m_Predicate = predicate;
	}

	public WaitWhile(Func<bool> predicate, TimeSpan timeout, Action onTimeout, WaitTimeoutMode timeoutMode = WaitTimeoutMode.Realtime)
		: this(predicate)
	{
		if (timeout <= TimeSpan.Zero)
		{
			throw new ArgumentException("Timeout must be greater than zero", "timeout");
		}
		m_TimeoutCallback = onTimeout ?? throw new ArgumentNullException("onTimeout", "Timeout callback must be specified");
		m_TimeoutMode = timeoutMode;
		m_MaxExecutionTime = GetTime() + timeout.TotalSeconds;
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	private double GetTime()
	{
		return (m_TimeoutMode == WaitTimeoutMode.InGameTime) ? Time.timeAsDouble : Time.realtimeSinceStartupAsDouble;
	}
}
