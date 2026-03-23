using FMOD.Studio;
using FMODUnity;
using UnityEngine;

public class ElectromagnetEquipment : MonoBehaviour
{
	[SerializeField]
	private ParticleSystem idleVfx;

	[SerializeField]
	private ParticleSystem activationVfx;

	private EventInstance idleSoundInstance;

	private void OnEnable()
	{
		idleVfx.Play(withChildren: true);
		idleSoundInstance = RuntimeManager.CreateInstance(GameManager.AudioSettings.ElectromagnetIdleEvent);
		RuntimeManager.AttachInstanceToGameObject(idleSoundInstance, base.gameObject);
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

	public void Activate()
	{
		idleVfx.Stop(withChildren: true);
		activationVfx.Play(withChildren: true);
		if (idleSoundInstance.isValid())
		{
			idleSoundInstance.stop(FMOD.Studio.STOP_MODE.ALLOWFADEOUT);
		}
		RuntimeManager.PlayOneShotAttached(GameManager.AudioSettings.ElectromagnetActivationEvent, base.gameObject);
	}
}
