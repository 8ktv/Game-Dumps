using System.Collections.Generic;

public class BallOutOfBoundsTriggerManager : SingletonBehaviour<BallOutOfBoundsTriggerManager>
{
	private readonly Dictionary<GolfBall, HashSet<BallOutOfBoundsTrigger>> overlappingTriggersPerBall = new Dictionary<GolfBall, HashSet<BallOutOfBoundsTrigger>>();

	protected override void OnDestroy()
	{
		base.OnDestroy();
		foreach (GolfBall key in overlappingTriggersPerBall.Keys)
		{
			key.IsHiddenChangedReferenced -= OnRegisteredBallIsHiddenChanged;
		}
	}

	public static void RegisterOverlap(GolfBall ball, BallOutOfBoundsTrigger trigger)
	{
		if (SingletonBehaviour<BallOutOfBoundsTriggerManager>.HasInstance)
		{
			SingletonBehaviour<BallOutOfBoundsTriggerManager>.Instance.RegisterOverlapInternal(ball, trigger);
		}
	}

	public static void DeregisterOverlap(GolfBall ball, BallOutOfBoundsTrigger trigger)
	{
		if (SingletonBehaviour<BallOutOfBoundsTriggerManager>.HasInstance)
		{
			SingletonBehaviour<BallOutOfBoundsTriggerManager>.Instance.DeregisterOverlapInternal(ball, trigger);
		}
	}

	public static bool IsBallInOutOfBoundsTrigger(GolfBall ball)
	{
		if (!SingletonBehaviour<BallOutOfBoundsTriggerManager>.HasInstance)
		{
			return false;
		}
		return SingletonBehaviour<BallOutOfBoundsTriggerManager>.Instance.IsBallInOutOfBoundsTriggerInternal(ball);
	}

	private void RegisterOverlapInternal(GolfBall ball, BallOutOfBoundsTrigger trigger)
	{
		if (!overlappingTriggersPerBall.TryGetValue(ball, out var value))
		{
			value = new HashSet<BallOutOfBoundsTrigger>();
			overlappingTriggersPerBall.Add(ball, value);
			ball.IsHiddenChangedReferenced += OnRegisteredBallIsHiddenChanged;
		}
		value.Add(trigger);
	}

	private void DeregisterOverlapInternal(GolfBall ball, BallOutOfBoundsTrigger trigger)
	{
		if (overlappingTriggersPerBall.TryGetValue(ball, out var value))
		{
			value.Remove(trigger);
			if (value.Count <= 0)
			{
				overlappingTriggersPerBall.Remove(ball);
				ball.IsHiddenChangedReferenced -= OnRegisteredBallIsHiddenChanged;
			}
		}
	}

	private bool IsBallInOutOfBoundsTriggerInternal(GolfBall ball)
	{
		return overlappingTriggersPerBall.ContainsKey(ball);
	}

	private void OnRegisteredBallIsHiddenChanged(GolfBall ball)
	{
		if (ball.IsHidden)
		{
			overlappingTriggersPerBall.Remove(ball);
		}
	}
}
