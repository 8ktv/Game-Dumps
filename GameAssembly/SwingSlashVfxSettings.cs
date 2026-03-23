using UnityEngine;

[CreateAssetMenu(fileName = "Swing Slash VFX Settings", menuName = "Scriptable Objects/VFX/Swing Slash Settings")]
public class SwingSlashVfxSettings : ScriptableObject
{
	[SerializeField]
	private SwingSlashVfxData[] settings;

	public SwingSlashVfxData GetSwingSlashData(float power)
	{
		for (int i = 0; i < settings.Length; i++)
		{
			SwingSlashVfxData swingSlashVfxData = settings[i];
			if (power <= swingSlashVfxData.maximumPower)
			{
				return swingSlashVfxData;
			}
		}
		return settings[settings.Length - 1];
	}
}
