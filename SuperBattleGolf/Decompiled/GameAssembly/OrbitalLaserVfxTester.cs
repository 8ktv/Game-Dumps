using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Rendering.PostProcessing;

public class OrbitalLaserVfxTester : MonoBehaviour
{
	[SerializeField]
	private Shake cameraShaker;

	[SerializeField]
	private float cameraShakeDuration;

	[SerializeField]
	private AnimationCurve cameraShakeCurve;

	[SerializeField]
	private GameObject targetIndicator;

	[SerializeField]
	private PostProcessVolume impactFrameVolume;

	[SerializeField]
	private float impactFrameDuration = 0.12f;

	[SerializeField]
	private GameObject player;

	[SerializeField]
	private ParticleSystem orbitalLaserAnticipation;

	[SerializeField]
	private ParticleSystem orbitalLaserExplosion;

	[SerializeField]
	private ParticleSystem orbitalLaserEnd;

	[SerializeField]
	private float targetingDuration;

	[SerializeField]
	private float anticipationDuration;

	[SerializeField]
	private float explosionDuration;

	private bool playingSequence;

	private void Start()
	{
		targetIndicator.SetActive(value: false);
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
		player.SetActive(value: true);
		targetIndicator.SetActive(value: true);
		await UniTask.WaitForSeconds(targetingDuration);
		targetIndicator.SetActive(value: false);
		player.SetActive(value: true);
		orbitalLaserAnticipation.Play(withChildren: true);
		await UniTask.WaitForSeconds(anticipationDuration);
		player.SetActive(value: false);
		PlayingImpactFrame();
		PlayingCameraShake();
		orbitalLaserExplosion.Play(withChildren: true);
		await UniTask.WaitForSeconds(explosionDuration);
		PlayingImpactFrame();
		orbitalLaserEnd.Play(withChildren: true);
		playingSequence = false;
	}

	private async void PlayingImpactFrame()
	{
		Vector2 value = Camera.main.WorldToViewportPoint(orbitalLaserExplosion.transform.position);
		if (impactFrameVolume.profile.TryGetSettings<ImpactFrame>(out var outSetting))
		{
			outSetting.distortionCenter.value = value;
		}
		impactFrameVolume.gameObject.SetActive(value: true);
		await UniTask.WaitForSeconds(impactFrameDuration);
		impactFrameVolume.gameObject.SetActive(value: false);
	}

	private async void PlayingCameraShake()
	{
		float timer = 0f;
		while (timer < cameraShakeDuration)
		{
			float time = timer / cameraShakeDuration;
			float shakeFactor = cameraShakeCurve.Evaluate(time);
			cameraShaker.ShakeFactor = shakeFactor;
			timer += Time.deltaTime;
			await UniTask.WaitForEndOfFrame(this);
		}
		cameraShaker.ShakeFactor = 0f;
	}
}
