using System;

namespace UnityEngine.UIElements;

internal abstract class ScheduledItem
{
	public Func<bool> timerUpdateStopCondition;

	public static readonly Func<bool> OnceCondition = () => true;

	public static readonly Func<bool> ForeverCondition = () => false;

	public long startMs { get; set; }

	public long delayMs { get; set; }

	public long intervalMs { get; set; }

	public long endTimeMs { get; private set; }

	public ScheduledItem(long startMs)
	{
		ResetStartTime(startMs);
		timerUpdateStopCondition = OnceCondition;
	}

	protected void ResetStartTime(long startMs)
	{
		this.startMs = startMs;
	}

	public void SetDuration(long durationMs)
	{
		endTimeMs = startMs + durationMs;
	}

	public void OffsetBy(long deltaMs)
	{
		if (endTimeMs > 0)
		{
			endTimeMs += deltaMs;
		}
		startMs += deltaMs;
	}

	public abstract void PerformTimerUpdate(TimerState state);

	internal virtual void OnItemUnscheduled()
	{
	}

	public virtual bool ShouldUnschedule()
	{
		if (timerUpdateStopCondition != null)
		{
			return timerUpdateStopCondition();
		}
		return false;
	}
}
