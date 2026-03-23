using UnityEngine;

[CreateAssetMenu(fileName = "PlayerCosmeticsVictoryDanceMetadata", menuName = "Scriptable Objects/PlayerCosmeticsVictoryDanceMetadata")]
public class PlayerCosmeticsVictoryDanceMetadata : ScriptableObject
{
	public VictoryDance dance;

	public Sprite icon;

	public int cost = 750;

	[DisableField]
	public string persistentGuid;
}
