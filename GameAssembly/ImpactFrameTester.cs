using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Rendering.PostProcessing;

public class ImpactFrameTester : MonoBehaviour
{
	[SerializeField]
	private Animator testAnimator;

	[SerializeField]
	private PostProcessVolume volume;

	[SerializeField]
	private ParticleSystem explosion;

	[SerializeField]
	private Vector2 xRange = new Vector2(-4f, 4f);

	[SerializeField]
	private Vector2 zRange = new Vector2(-3f, 7f);

	[SerializeField]
	private float impactFrameDuration = 0.12f;

	private void Start()
	{
		volume.gameObject.SetActive(value: false);
		explosion.Stop(withChildren: true, ParticleSystemStopBehavior.StopEmittingAndClear);
	}

	private void Update()
	{
		if (Keyboard.current[Key.Q].wasPressedThisFrame)
		{
			Explode();
		}
	}

	private Vector3 GetRandomLocation()
	{
		return new Vector3(Random.Range(xRange.x, xRange.y), 0f, Random.Range(zRange.x, zRange.y));
	}

	private async void Explode()
	{
		Vector3 randomLocation = GetRandomLocation();
		Vector2 value = Camera.main.WorldToViewportPoint(randomLocation);
		if (volume.profile.TryGetSettings<ImpactFrame>(out var outSetting))
		{
			outSetting.distortionCenter.value = value;
		}
		explosion.transform.position = randomLocation;
		explosion.Play(withChildren: true);
		testAnimator.SetTrigger("shake");
		volume.gameObject.SetActive(value: true);
		await UniTask.WaitForSeconds(impactFrameDuration);
		volume.gameObject.SetActive(value: false);
	}
}
