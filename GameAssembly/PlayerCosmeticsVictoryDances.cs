using UnityEngine;

[CreateAssetMenu(fileName = "PlayerCosmeticsVictoryDances", menuName = "Scriptable Objects/PlayerCosmeticsVictoryDances")]
public class PlayerCosmeticsVictoryDances : ScriptableObject
{
	[SerializeField]
	private PlayerCosmeticsVictoryDanceMetadata[] victoryDances;

	public int Length => victoryDances.Length;

	public PlayerCosmeticsVictoryDanceMetadata this[int i] => victoryDances[i];

	public PlayerCosmeticsVictoryDanceMetadata GetDance(VictoryDance dance)
	{
		if (dance <= VictoryDance.None)
		{
			return null;
		}
		for (int i = 0; i < Length; i++)
		{
			if (victoryDances[i].dance == dance)
			{
				return victoryDances[i];
			}
		}
		return null;
	}
}
