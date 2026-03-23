using System;
using UnityEngine;

[CreateAssetMenu(fileName = "PlayerCosmeticsSettings", menuName = "Scriptable Objects/PlayerCosmeticsSettings")]
public class PlayerCosmeticsSettings : ScriptableObject
{
	[Serializable]
	public struct SkinColor
	{
		[ColorUsage(false)]
		public Color iconColor;

		[ColorUsage(false)]
		public Color baseColor;

		[ColorUsage(false)]
		public Color mouthColor;
	}

	public SkinColor[] skinColors;
}
