using UnityEngine;

public class AmbientVfx : MonoBehaviour
{
	[SerializeField]
	private ParticleSystem vfx;

	[SerializeField]
	private AmbientVfxType ambientVfxType;

	private bool initialized;

	public AmbientVfxType VfxType => ambientVfxType;

	public void Initialize()
	{
		if (!initialized)
		{
			if (vfx == null)
			{
				Debug.LogError($"AmbientVfx of type {ambientVfxType} is missing a reference to its ParticleSystem component.", this);
			}
			else if (ambientVfxType == AmbientVfxType.None)
			{
				Debug.LogError("AmbientVfx cannot be initialized with AmbientVfxType set to None.", this);
			}
			else
			{
				initialized = true;
			}
		}
	}

	public void DisableVfx()
	{
		if (vfx != null)
		{
			vfx.Stop();
		}
		base.enabled = false;
	}

	private void Start()
	{
		if (ambientVfxType != AmbientVfxType.None)
		{
			AmbientVfxManager.RegisterAmbientVfx(this);
		}
	}

	private void OnDestroy()
	{
		AmbientVfxManager.DeregisterAmbientVfx(this);
	}

	public void PlayLocal()
	{
		if (initialized && !vfx.isPlaying)
		{
			vfx.Play();
		}
	}

	public void StopLocal()
	{
		if (initialized && vfx.isPlaying)
		{
			vfx.Stop();
		}
	}
}
