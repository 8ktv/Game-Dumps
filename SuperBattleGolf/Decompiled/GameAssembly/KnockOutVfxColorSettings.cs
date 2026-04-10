using UnityEngine;

[CreateAssetMenu(fileName = "Knock Out VFX Color Settings", menuName = "Settings/VFX/Knock Out VFX Color Settings")]
public class KnockOutVfxColorSettings : ScriptableObject
{
	[SerializeField]
	private KnockOutVfxColorData[] colorData;

	public KnockOutVfxColorData GetData(KnockOutVfxColor color)
	{
		for (int i = 0; i < colorData.Length; i++)
		{
			if (colorData[i].knockoutColor == color)
			{
				return colorData[i];
			}
		}
		return null;
	}
}
