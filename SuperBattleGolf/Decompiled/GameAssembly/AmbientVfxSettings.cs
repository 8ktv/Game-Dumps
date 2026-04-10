using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Ambient VFX Settings", menuName = "Settings/VFX/Ambient VFX Settings")]
public class AmbientVfxSettings : ScriptableObject
{
	[SerializeField]
	private float activationRadius = 50f;

	[SerializeField]
	[DynamicElementName("vfxType")]
	private AmbientVfxData[] ambientVfxTypeSettings;

	public Dictionary<AmbientVfxType, AmbientVfxData> SettingsByType = new Dictionary<AmbientVfxType, AmbientVfxData>();

	public float ActivationRadiusSqr => activationRadius * activationRadius;

	private void OnEnable()
	{
		SettingsByType.Clear();
		if (ambientVfxTypeSettings == null)
		{
			return;
		}
		AmbientVfxData[] array = ambientVfxTypeSettings;
		foreach (AmbientVfxData ambientVfxData in array)
		{
			if (ambientVfxData != null)
			{
				if (SettingsByType.ContainsKey(ambientVfxData.vfxType))
				{
					Debug.LogWarning($"Duplicate AmbientVfxType entry found in {base.name}: {ambientVfxData.vfxType}", this);
				}
				else
				{
					SettingsByType.Add(ambientVfxData.vfxType, ambientVfxData);
				}
			}
		}
	}
}
