using UnityEngine;

public class GolfCartVfx : MonoBehaviour
{
	[SerializeField]
	private ParticleSystem pipeVfx;

	[SerializeField]
	private ParticleSystem speedUpVfx;

	[SerializeField]
	private GameObject headlightsContainer;

	[SerializeField]
	private Animator headlightsAnimator;

	[SerializeField]
	private GolfCartFlagPoleVfx flagPole;

	[SerializeField]
	private GolfCartTireTrailVfx[] tireTrails;

	[SerializeField]
	private ParticleSystem waterWadingRings;

	private bool isPlayingWaterWadingRings;

	public bool ArePipeVfxPlaying { get; private set; }

	public bool HeadlightsAreOn => headlightsContainer.activeSelf;

	private void Awake()
	{
		SetTrailsEmitting(emitting: true);
		SetPipeVfxPlaying(playing: false);
		SetHeadlightsOn(active: false);
	}

	public void SetLocalVelocity(Vector3 localVelocity)
	{
		int forwardLean = ((localVelocity.z < -2f) ? 1 : ((localVelocity.z > 2f) ? (-1) : 0));
		flagPole.SetForwardLean(forwardLean);
	}

	public void ResetLocalAcceleration()
	{
		flagPole.SetForwardLean(0);
	}

	public void SetTrailsEmitting(bool emitting)
	{
		for (int i = 0; i < tireTrails.Length; i++)
		{
			tireTrails[i].SetEmitting(emitting);
		}
	}

	public void SetPipeVfxPlaying(bool playing)
	{
		if (playing != ArePipeVfxPlaying)
		{
			ArePipeVfxPlaying = playing;
			if (playing)
			{
				pipeVfx.Play();
			}
			else
			{
				pipeVfx.Stop();
			}
		}
	}

	public void SetSpeedUpVfxPlaying(bool playing)
	{
		if (playing && !speedUpVfx.isPlaying)
		{
			speedUpVfx.Play(withChildren: true);
		}
		else if (!playing && speedUpVfx.isPlaying)
		{
			speedUpVfx.Stop(withChildren: true, ParticleSystemStopBehavior.StopEmitting);
		}
	}

	public void OnJumpStart()
	{
		VfxManager.PlayPooledVfxLocalOnly(VfxType.GolfCartJumpStart, base.transform.position, base.transform.rotation);
	}

	public void OnJumpEnd()
	{
		VfxManager.PlayPooledVfxLocalOnly(VfxType.GolfCartJumpEnd, base.transform.position, base.transform.rotation);
	}

	public void SetHeadlightsOn(bool active)
	{
		headlightsContainer.SetActive(active);
	}

	public void SetIsWadingInWater(bool isWading)
	{
		if (isWading != isPlayingWaterWadingRings)
		{
			isPlayingWaterWadingRings = isWading;
			if (isPlayingWaterWadingRings)
			{
				waterWadingRings.Play();
			}
			else
			{
				waterWadingRings.Stop(withChildren: true, ParticleSystemStopBehavior.StopEmitting);
			}
		}
	}

	public void SetWadingWaterWorldHeight(float worldHeight)
	{
		Vector3 position = waterWadingRings.transform.position;
		position.y = worldHeight;
		waterWadingRings.transform.position = position;
	}

	public void OnCollision()
	{
		headlightsAnimator.SetTrigger("collision");
	}
}
