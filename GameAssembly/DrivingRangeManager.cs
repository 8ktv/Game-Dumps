using System.Collections.Generic;
using UnityEngine;

public class DrivingRangeManager : SingletonBehaviour<DrivingRangeManager>, ILateBUpdateCallback, IAnyBUpdateCallback
{
	[SerializeField]
	private DrivingRangeSpawnArea spawnArea;

	private readonly List<BallDispenser> ballDispensers = new List<BallDispenser>();

	private bool isBallDispenserIconShown;

	private BallDispenser nearestBallDispenser;

	public static DrivingRangeSpawnArea SpawnArea
	{
		get
		{
			if (!SingletonBehaviour<DrivingRangeManager>.HasInstance)
			{
				return null;
			}
			return SingletonBehaviour<DrivingRangeManager>.Instance.spawnArea;
		}
	}

	protected override void Awake()
	{
		base.Awake();
		BUpdate.RegisterCallback(this);
	}

	protected override void OnDestroy()
	{
		base.OnDestroy();
		BUpdate.DeregisterCallback(this);
	}

	public static void RegisterBallDispenser(BallDispenser dispenser)
	{
		if (SingletonBehaviour<DrivingRangeManager>.HasInstance)
		{
			SingletonBehaviour<DrivingRangeManager>.Instance.RegisterBallDispenserInternal(dispenser);
		}
	}

	public static void DeregisterBallDispenser(BallDispenser dispenser)
	{
		if (SingletonBehaviour<DrivingRangeManager>.HasInstance)
		{
			SingletonBehaviour<DrivingRangeManager>.Instance.DeregisterBallDispenserInternal(dispenser);
		}
	}

	private void RegisterBallDispenserInternal(BallDispenser dispenser)
	{
		if (!ballDispensers.Contains(dispenser))
		{
			ballDispensers.Add(dispenser);
		}
	}

	private void DeregisterBallDispenserInternal(BallDispenser dispenser)
	{
		if (!BNetworkManager.IsChangingSceneOrShuttingDown)
		{
			ballDispensers.Remove(dispenser);
		}
	}

	public void OnLateBUpdate()
	{
		UpdateIsBallDispenserIconShown(out var changed);
		UpdateNearestBallDispenser(changed);
		static bool ShouldDisplayBallDispenserIcon()
		{
			if (GameManager.LocalPlayerAsGolfer == null)
			{
				return false;
			}
			if (GameManager.LocalPlayerAsGolfer.OwnBall != null && !GameManager.LocalPlayerAsGolfer.OwnBall.IsHidden)
			{
				return false;
			}
			return true;
		}
		void UpdateIsBallDispenserIconShown(out bool reference)
		{
			bool flag = isBallDispenserIconShown;
			isBallDispenserIconShown = ShouldDisplayBallDispenserIcon();
			reference = isBallDispenserIconShown != flag;
			if (reference && !isBallDispenserIconShown && nearestBallDispenser != null)
			{
				nearestBallDispenser.HideWorldspaceIcon();
			}
		}
		void UpdateNearestBallDispenser(bool forced)
		{
			if (isBallDispenserIconShown)
			{
				BallDispenser ballDispenser = null;
				float num = float.MaxValue;
				foreach (BallDispenser ballDispenser2 in ballDispensers)
				{
					float sqrMagnitude = (ballDispenser2.transform.position - GameManager.LocalPlayerInfo.transform.position).sqrMagnitude;
					if (!(sqrMagnitude > num))
					{
						num = sqrMagnitude;
						ballDispenser = ballDispenser2;
					}
				}
				if (forced || !(ballDispenser == nearestBallDispenser))
				{
					if (nearestBallDispenser != null)
					{
						nearestBallDispenser.HideWorldspaceIcon();
					}
					nearestBallDispenser = ballDispenser;
					if (nearestBallDispenser != null)
					{
						nearestBallDispenser.ShowWorldspaceIcon();
					}
				}
			}
		}
	}
}
