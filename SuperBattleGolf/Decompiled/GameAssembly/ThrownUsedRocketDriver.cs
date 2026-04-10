using FMOD.Studio;
using FMODUnity;
using UnityEngine;

public class ThrownUsedRocketDriver : MonoBehaviour, IFixedBUpdateCallback, IAnyBUpdateCallback
{
	[SerializeField]
	private Rigidbody rigidbody;

	[SerializeField]
	private ParticleSystem vfx;

	[SerializeField]
	private Transform rocketPosition;

	[SerializeField]
	private float rocketAcceleration;

	[SerializeField]
	private Vector3 rocketActiveLocalCenterOfMass;

	private EventInstance engineLoop;

	public bool IsRocketActive { get; private set; }

	private void OnDisable()
	{
		SetIsRocketActive(isRocketActive: false);
	}

	private void OnDestroy()
	{
		if (engineLoop.isValid())
		{
			engineLoop.stop(FMOD.Studio.STOP_MODE.ALLOWFADEOUT);
		}
		if (IsRocketActive)
		{
			BUpdate.DeregisterCallback(this);
		}
	}

	public void OnFixedBUpdate()
	{
		rigidbody.AddForceAtPosition(-rocketPosition.forward * rocketAcceleration, rocketPosition.position, ForceMode.Acceleration);
	}

	public void SetIsRocketActive(bool isRocketActive)
	{
		if (isRocketActive != IsRocketActive)
		{
			IsRocketActive = isRocketActive;
			if (engineLoop.isValid())
			{
				engineLoop.stop(FMOD.Studio.STOP_MODE.ALLOWFADEOUT);
			}
			if (isRocketActive)
			{
				BUpdate.RegisterCallback(this);
				vfx.Play(withChildren: true);
				engineLoop = RuntimeManager.CreateInstance(GameManager.AudioSettings.RocketDriverThrownUsedLoopEvent);
				RuntimeManager.AttachInstanceToGameObject(engineLoop, rocketPosition.gameObject);
				engineLoop.start();
				engineLoop.release();
			}
			else
			{
				BUpdate.DeregisterCallback(this);
				vfx.Stop(withChildren: true, ParticleSystemStopBehavior.StopEmitting);
			}
		}
	}
}
