using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.InputSystem;

public class DuelingPistolVfxTester : MonoBehaviour
{
	[SerializeField]
	private Animator pistolAnimator;

	[SerializeField]
	private ParticleSystem muzzleParticles;

	[SerializeField]
	private ParticleSystem impactParticles;

	[SerializeField]
	private LineRenderer tracerLine;

	[SerializeField]
	private float tracerInitialWidth = 0.05f;

	[SerializeField]
	private float tracerDuration = 1f;

	private void Start()
	{
		tracerLine.enabled = false;
	}

	private void Update()
	{
		if (Keyboard.current[Key.Q].wasPressedThisFrame)
		{
			Shoot();
		}
	}

	private void Shoot()
	{
		pistolAnimator.SetTrigger("shoot");
		if ((bool)muzzleParticles)
		{
			muzzleParticles?.Play(withChildren: true);
		}
		if ((bool)impactParticles)
		{
			impactParticles.Play(withChildren: true);
		}
		PlayingTracer();
	}

	private async void PlayingTracer()
	{
		tracerLine.enabled = true;
		SetTracerWidth(1f);
		float timer = 0f;
		while (timer < tracerDuration)
		{
			float num = timer / tracerDuration;
			SetTracerWidth(1f - num);
			timer += Time.deltaTime;
			await UniTask.WaitForEndOfFrame(this);
			if (this == null)
			{
				return;
			}
		}
		SetTracerWidth(0f);
		tracerLine.enabled = false;
	}

	private void SetTracerWidth(float multiplier)
	{
		tracerLine.widthMultiplier = multiplier;
	}
}
