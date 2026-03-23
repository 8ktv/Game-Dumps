using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.InputSystem;

public class RocketLauncherVfxTester : MonoBehaviour
{
	[SerializeField]
	private ParticleSystem backBlastVfx;

	[SerializeField]
	private ParticleSystem muzzleVfx;

	[SerializeField]
	private ParticleSystem explosionVfx;

	[SerializeField]
	private ParticleSystem rocketTrailVfx;

	[SerializeField]
	private GameObject rocketModel;

	[SerializeField]
	private Transform rocketContainer;

	[SerializeField]
	private Transform rocketPointA;

	[SerializeField]
	private Transform rocketPointB;

	[SerializeField]
	private Animator rocketLauncherAnimator;

	[SerializeField]
	private Animator cameraAnimator;

	[SerializeField]
	private ImpactFrameController impactFrameController;

	[SerializeField]
	private float rocketFlightDuration;

	private bool playingSequence;

	private void Start()
	{
		rocketModel.SetActive(value: true);
		rocketContainer.position = rocketPointA.position;
	}

	private void Update()
	{
		if (Keyboard.current[Key.Q].wasPressedThisFrame && !playingSequence)
		{
			PlayingSequence();
		}
	}

	private async void PlayingSequence()
	{
		playingSequence = true;
		rocketLauncherAnimator.SetTrigger("shoot");
		rocketTrailVfx.Clear(withChildren: true);
		backBlastVfx.Play(withChildren: true);
		muzzleVfx.Play(withChildren: true);
		rocketModel.SetActive(value: true);
		rocketTrailVfx.Play(withChildren: true);
		rocketContainer.position = rocketPointA.position;
		float timer = 0f;
		while (timer < rocketFlightDuration)
		{
			float t = timer / rocketFlightDuration;
			rocketContainer.position = Vector3.Lerp(rocketPointA.position, rocketPointB.position, t);
			timer += Time.deltaTime;
			await UniTask.WaitForEndOfFrame(this);
		}
		rocketModel.SetActive(value: false);
		rocketTrailVfx.Stop(withChildren: true);
		rocketContainer.position = rocketPointB.position;
		explosionVfx.Play(withChildren: true);
		playingSequence = false;
		impactFrameController.PlayImpactFrame(explosionVfx.transform.position);
		cameraAnimator.SetTrigger("shake");
		await UniTask.WaitForSeconds(1.5f);
		rocketModel.SetActive(value: true);
		rocketContainer.position = rocketPointA.position;
	}
}
