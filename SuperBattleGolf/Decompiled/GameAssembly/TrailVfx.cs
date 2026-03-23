using Ara;
using UnityEngine;

public class TrailVfx : MonoBehaviour
{
	[SerializeField]
	private ParticleSystem[] particles;

	[SerializeField]
	private ParticleSystem[] ribbonParticles;

	[SerializeField]
	private AraTrail[] trails;

	public bool IsPlaying { get; private set; }

	public void SetPlaying(bool playing, bool forced = false)
	{
		if (!forced && playing == IsPlaying)
		{
			return;
		}
		IsPlaying = playing;
		if (IsPlaying)
		{
			for (int i = 0; i < particles.Length; i++)
			{
				particles[i].Play();
			}
			for (int j = 0; j < ribbonParticles.Length; j++)
			{
				ParticleSystem.TrailModule trailModule = ribbonParticles[j].trails;
				trailModule.attachRibbonsToTransform = true;
			}
			for (int k = 0; k < trails.Length; k++)
			{
				trails[k].emit = true;
			}
		}
		else
		{
			for (int l = 0; l < particles.Length; l++)
			{
				particles[l].Stop();
			}
			for (int m = 0; m < ribbonParticles.Length; m++)
			{
				ParticleSystem.TrailModule trailModule2 = ribbonParticles[m].trails;
				trailModule2.attachRibbonsToTransform = false;
			}
			for (int n = 0; n < trails.Length; n++)
			{
				trails[n].emit = false;
			}
		}
	}

	public void Clear()
	{
		for (int i = 0; i < particles.Length; i++)
		{
			particles[i].Clear();
		}
		for (int j = 0; j < ribbonParticles.Length; j++)
		{
			ribbonParticles[j].Clear();
		}
		for (int k = 0; k < trails.Length; k++)
		{
			trails[k].Clear();
		}
	}
}
