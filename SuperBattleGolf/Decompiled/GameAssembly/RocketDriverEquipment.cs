using FMOD.Studio;
using FMODUnity;
using UnityEngine;

public class RocketDriverEquipment : MonoBehaviour
{
	private EventInstance idleSoundInstance;

	[field: SerializeField]
	public RocketDriverEquipmentVfx Vfx { get; private set; }

	private void OnEnable()
	{
		idleSoundInstance = RuntimeManager.CreateInstance(GameManager.AudioSettings.RocketDriverIdleEvent);
		RuntimeManager.AttachInstanceToGameObject(idleSoundInstance, base.gameObject);
		idleSoundInstance.setParameterByID(AudioSettings.ChargeUpId, 0f);
		idleSoundInstance.start();
		idleSoundInstance.release();
	}

	private void OnDisable()
	{
		if (idleSoundInstance.isValid())
		{
			idleSoundInstance.stop(FMOD.Studio.STOP_MODE.ALLOWFADEOUT);
		}
	}

	private void OnDestroy()
	{
		if (idleSoundInstance.isValid())
		{
			idleSoundInstance.stop(FMOD.Studio.STOP_MODE.IMMEDIATE);
		}
	}

	public void SetNormalizedCharge(float normalizedCharge)
	{
		if (idleSoundInstance.isValid())
		{
			idleSoundInstance.setParameterByID(AudioSettings.ChargeUpId, BMath.Clamp01(normalizedCharge));
		}
	}
}
