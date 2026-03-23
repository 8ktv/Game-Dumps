using FMODUnity;
using UnityEngine;

public class OrbitalLaserEquipment : MonoBehaviour
{
	public void Activate()
	{
		if (!VfxPersistentData.TryGetPooledVfx(VfxType.OrbitalLaserRemoteActivation, out var particleSystem))
		{
			Debug.LogError("Failed to get orbital laser remote activation VFX");
			return;
		}
		particleSystem.transform.SetParent(base.transform);
		particleSystem.transform.SetLocalPositionAndRotation(Vector3.zero, Quaternion.identity);
		particleSystem.Play();
		RuntimeManager.PlayOneShotAttached(GameManager.AudioSettings.OrbitalLaserActivationEvent, base.gameObject);
	}
}
