using UnityEngine;

public class PlayerStateVfx : MonoBehaviour
{
	[Header("Speed boost")]
	[SerializeField]
	private ParticleSystem speedUpIconVfx;

	[SerializeField]
	private ParticleSystem speedUpDropletsVfx;

	[SerializeField]
	private ParticleSystem speedUpBodyVfx;

	[SerializeField]
	private ParticleSystemRenderer[] speedUpParticleRenderers;

	private bool isPlayingSpeedBoostEffect;

	public void PlaySpeedBoostEffects()
	{
		bool num = isPlayingSpeedBoostEffect;
		isPlayingSpeedBoostEffect = true;
		speedUpIconVfx.Play(withChildren: true);
		if (!num)
		{
			speedUpDropletsVfx.Play(withChildren: true);
			speedUpBodyVfx.Play(withChildren: true);
		}
	}

	public void StopSpeedBoostEffects()
	{
		if (isPlayingSpeedBoostEffect)
		{
			isPlayingSpeedBoostEffect = false;
			speedUpIconVfx.Stop(withChildren: true, ParticleSystemStopBehavior.StopEmittingAndClear);
			speedUpDropletsVfx.Stop(withChildren: true, ParticleSystemStopBehavior.StopEmitting);
			speedUpBodyVfx.Stop(withChildren: true, ParticleSystemStopBehavior.StopEmitting);
		}
	}

	public void SetSpeedUpVfxHidden(bool hidden)
	{
		for (int i = 0; i < speedUpParticleRenderers.Length; i++)
		{
			speedUpParticleRenderers[i].enabled = !hidden;
		}
	}
}
