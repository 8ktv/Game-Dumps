using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Rendering.PostProcessing;

public class MineVfxTester : MonoBehaviour
{
	[SerializeField]
	private MineVfx mineVfx;

	[SerializeField]
	private Animator testAnimator;

	[SerializeField]
	private ParticleSystem armedParticles;

	[SerializeField]
	private ParticleSystem burrowParticles;

	[SerializeField]
	private ParticleSystem explosionParticles;

	[SerializeField]
	private PostProcessVolume impactFrameVolume;

	[SerializeField]
	private float impactFrameDuration = 0.1f;

	private bool armed;

	private void Start()
	{
		impactFrameVolume.gameObject.SetActive(value: false);
	}

	private void Update()
	{
		if (Keyboard.current[Key.Q].wasPressedThisFrame)
		{
			Burrow();
		}
		if (Keyboard.current[Key.W].wasPressedThisFrame)
		{
			ToggleArmedState();
		}
		if (Keyboard.current[Key.E].wasPressedThisFrame)
		{
			Explode();
		}
	}

	private void Burrow()
	{
		burrowParticles.Play();
		testAnimator.SetTrigger("burrow");
	}

	private void ToggleArmedState()
	{
		SetArmedState(!armed);
	}

	private void SetArmedState(bool armed)
	{
		this.armed = armed;
		if (armed)
		{
			mineVfx.Arm(skipArmingEffects: false);
			armedParticles.Play();
		}
		else
		{
			mineVfx.Unarm();
		}
	}

	private async void Explode()
	{
		explosionParticles.Play();
		testAnimator.SetTrigger("explode");
		SetArmedState(armed: false);
		impactFrameVolume.gameObject.SetActive(value: true);
		await UniTask.WaitForSeconds(impactFrameDuration);
		impactFrameVolume.gameObject.SetActive(value: false);
	}
}
