using System;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Match settings", menuName = "Settings/Gameplay/Match")]
public class MatchSettings : ScriptableObject
{
	[Serializable]
	private struct ParSpeedrunTime : IComparable<ParSpeedrunTime>
	{
		[Min(1f)]
		[Delayed]
		public int par;

		[Min(0f)]
		public float speedrunTime;

		public readonly int CompareTo(ParSpeedrunTime other)
		{
			return par.CompareTo(other.par);
		}
	}

	[Serializable]
	public struct ComebackBonus
	{
		public int MinPointsGap;

		public float BonusMultiplier;
	}

	[SerializeField]
	[ElementName("Time")]
	private List<ParSpeedrunTime> speedrunTimes;

	[field: SerializeField]
	public float HoleOverviewInitialBlankDuration { get; private set; }

	[field: SerializeField]
	public float HoleOverviewHoleNameDuration { get; private set; }

	[field: SerializeField]
	public float HoleOverviewFinalBlankDuration { get; private set; }

	[field: SerializeField]
	public float HoleOverviewCameraSlowdownDuration { get; private set; }

	[field: SerializeField]
	public AnimationCurve HoleOverviewFlyOverToTeeCurve { get; private set; }

	[field: SerializeField]
	public float TeeOffCountdownDuration { get; private set; }

	[field: SerializeField]
	public float MatchEndCountdownDuration { get; private set; }

	[field: SerializeField]
	public float OvertimeDuration { get; private set; }

	[field: SerializeField]
	public float StartNextMatchDelay { get; private set; }

	[field: SerializeField]
	public float FinishCourseDelay { get; private set; }

	[field: SerializeField]
	public float MatchEndScoreboardDisplayDelay { get; private set; }

	[field: SerializeField]
	public float EliminationInWaterDrowningDuration { get; private set; }

	[field: SerializeField]
	public float RespawnPostEliminationDelay { get; private set; }

	[field: SerializeField]
	public float RespawnAnimationDuration { get; private set; }

	[field: SerializeField]
	public float MatchResolvedSpectateStartDelay { get; private set; }

	[field: SerializeField]
	public float SpectatedPlayerMatchResolvedAutoCycleDelay { get; private set; }

	[field: SerializeField]
	public int DominationKnockoutStreak { get; private set; }

	[field: SerializeField]
	[field: ElementName("Position")]
	public int[] Scores { get; private set; }

	[field: SerializeField]
	public int ParScore { get; private set; }

	[field: SerializeField]
	public int BirdieScore { get; private set; }

	[field: SerializeField]
	public int EagleScore { get; private set; }

	[field: SerializeField]
	public int AlbatrossScore { get; private set; }

	[field: SerializeField]
	public int CondorScore { get; private set; }

	[field: SerializeField]
	public int HoleInOneScore { get; private set; }

	[field: SerializeField]
	public int ChipInMinDistance { get; private set; }

	[field: SerializeField]
	public int ChipInScore { get; private set; }

	[field: SerializeField]
	public int SpeedrunScore { get; private set; }

	[field: SerializeField]
	public int KnockoutScore { get; private set; }

	[field: SerializeField]
	public ComebackBonus LowerComebackBonus { get; private set; }

	[field: SerializeField]
	public ComebackBonus UpperComebackBonus { get; private set; }

	public float HoleOverviewFlyOverToTeeDuration { get; private set; }

	private void OnValidate()
	{
		Initialize();
	}

	private void OnEnable()
	{
		Initialize();
	}

	private void Initialize()
	{
		speedrunTimes.Sort();
		HoleOverviewFlyOverToTeeDuration = HoleOverviewFlyOverToTeeCurve.keys[^1].time;
	}

	public float GetSpeedrunTimeForPar(int par)
	{
		if (speedrunTimes == null && speedrunTimes.Count <= 0)
		{
			return 0f;
		}
		foreach (ParSpeedrunTime speedrunTime in speedrunTimes)
		{
			if (par <= speedrunTime.par)
			{
				return speedrunTime.speedrunTime;
			}
		}
		List<ParSpeedrunTime> list = speedrunTimes;
		return list[list.Count - 1].speedrunTime;
	}
}
