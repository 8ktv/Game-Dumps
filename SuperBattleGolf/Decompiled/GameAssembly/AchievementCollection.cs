using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Achievements", menuName = "Settings/Achievements")]
public class AchievementCollection : ScriptableObject
{
	public Dictionary<AchievementId, AchievementData> achievementsById = new Dictionary<AchievementId, AchievementData>();

	public Dictionary<string, AchievementData> achievementsBySteamApiName = new Dictionary<string, AchievementData>();

	[field: SerializeField]
	[field: DynamicElementName("Id")]
	public AchievementData[] Achievements { get; private set; }

	[field: SerializeField]
	public int BullyDominationRequirement { get; private set; }

	[field: SerializeField]
	public int CoolheadedMinHoleCount { get; private set; }

	[field: SerializeField]
	public int CoolheadedMaxStrokesRelativeToPar { get; private set; }

	[field: SerializeField]
	public float DangerCloseDistanceFromExplosion { get; private set; }

	[field: SerializeField]
	public float FrogLegsDistance { get; private set; }

	[field: SerializeField]
	public float GunslingerDistance { get; private set; }

	[field: SerializeField]
	public float HomeRunDistance { get; private set; }

	[field: SerializeField]
	public int LivingOnTheEdgeMinPar { get; private set; }

	[field: SerializeField]
	public int NeverGiveUpMinMatchKnockouts { get; private set; }

	[field: SerializeField]
	public float SweetMovesMinTime { get; private set; }

	[field: SerializeField]
	[field: HideInInspector]
	public float FrogLegsDistanceSquared { get; private set; }

	private void OnValidate()
	{
		FrogLegsDistanceSquared = FrogLegsDistance * FrogLegsDistance;
	}

	private void OnEnable()
	{
		achievementsById.Clear();
		achievementsBySteamApiName.Clear();
		AchievementData[] achievements = Achievements;
		foreach (AchievementData achievementData in achievements)
		{
			achievementsById.Add(achievementData.Id, achievementData);
			achievementsBySteamApiName.Add(achievementData.SteamApiName, achievementData);
		}
	}
}
