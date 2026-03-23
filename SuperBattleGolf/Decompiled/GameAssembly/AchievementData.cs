using System;
using UnityEngine;

[Serializable]
public class AchievementData
{
	[field: SerializeField]
	public AchievementId Id { get; private set; }

	[field: SerializeField]
	public string SteamApiName { get; private set; }

	[field: SerializeField]
	public bool HasProgressRequirement { get; private set; }

	[field: SerializeField]
	[field: DisplayIf("HasProgressRequirement", true)]
	public int RequiredProgress { get; private set; }

	[field: SerializeField]
	[field: DisplayIf("HasProgressRequirement", true)]
	public string SteamProgressStatApiName { get; private set; }
}
