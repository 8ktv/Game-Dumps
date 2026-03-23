using System;
using UnityEngine;
using UnityEngine.AddressableAssets;

[CreateAssetMenu(fileName = "PlayerCosmeticsMetadata", menuName = "Scriptable Objects/PlayerCosmeticsMetadata")]
public class PlayerCosmeticsMetadata : ScriptableObject
{
	[Serializable]
	public class Variation
	{
		public int materialIndex = -1;

		[ColorUsage(false)]
		public Color menuColor = Color.white;

		[ColorUsage(false)]
		public Color tintColor = Color.white;

		[ColorUsage(false, true)]
		public Color emissiveColor = Color.black;

		public AssetReferenceTexture2D textureOverride;
	}

	public enum Category
	{
		Head,
		Hat,
		Face,
		FaceLower,
		Clothing,
		Club,
		Golfball,
		Mouth,
		Eyes,
		Brows,
		Cheeks,
		Hair,
		VictoryDance,
		Count
	}

	public static readonly Variation NoVariation = new Variation();

	public AssetReference model;

	public Category category;

	public Sprite icon;

	public Variation[] variations;

	public int cost;

	public string externalIpCredit = string.Empty;

	public bool unlockedByAchievment;

	[DisplayIf("unlockedByAchievment", true)]
	public AchievementId requiredAchievement;

	[SerializeField]
	[DisableField]
	private string persistentGuid;

	[DisableField]
	public PlayerCosmeticObject.ModelSlot allowedCosmetics;

	public string PersistentGuid => persistentGuid;

	public bool IsHidden()
	{
		if (CosmeticsUnlocksManager.everythingUnlocked)
		{
			return false;
		}
		if (unlockedByAchievment && !GameManager.AchievementsManager.IsUnlocked(requiredAchievement))
		{
			return true;
		}
		return false;
	}
}
