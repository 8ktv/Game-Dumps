using Cysharp.Threading.Tasks;
using UnityEngine;

public class RespawnVfx : MonoBehaviour
{
	[SerializeField]
	private PoolableParticleSystem asPoolable;

	[SerializeField]
	private MeshRenderer meshRenderer;

	[SerializeField]
	private float meshGenerationDuration;

	[SerializeField]
	private AnimationCurve meshGenerationCurve;

	[SerializeField]
	private float stompDuration;

	[SerializeField]
	private AnimationCurve stompPropsCurve;

	[SerializeField]
	private float rayDuration;

	[SerializeField]
	private ParticleSystem poofParticles;

	[SerializeField]
	private ParticleSystem spawnParticles;

	[SerializeField]
	private ParticleSystem spawnDownwardParticles;

	[SerializeField]
	private ParticleSystem rayParticles;

	[SerializeField]
	private ParticleSystem releaseParticles;

	[SerializeField]
	private Vector2 spawnParticlesHeightRange;

	[SerializeField]
	private AnimationCurve spawnParticlesHeightCurve;

	private MaterialPropertyBlock props;

	private int heightErosionId = Shader.PropertyToID("_Height_Erosion");

	private Vector2 heightErosion = Vector2.one;

	public PoolableParticleSystem AsPoolable => asPoolable;

	private void OnEnable()
	{
		if (props == null)
		{
			props = new MaterialPropertyBlock();
		}
		SetHeightErosion(-1f);
		meshRenderer.gameObject.SetActive(value: true);
	}

	public void PlayAnimation()
	{
		PlayingAnimation();
	}

	private void SetHeightErosion(float height)
	{
		heightErosion.Set(height, 1f);
		props.SetVector(heightErosionId, heightErosion);
		meshRenderer.SetPropertyBlock(props);
	}

	private async void PlayingAnimation()
	{
		meshRenderer.gameObject.SetActive(value: true);
		spawnDownwardParticles.Play();
		PlayingMeshGeneration();
		PlayingSpawnParticles();
		await UniTask.WaitForSeconds(meshGenerationDuration);
		if (this == null)
		{
			return;
		}
		rayParticles.Play();
		await UniTask.WaitForSeconds(rayDuration);
		if (!(this == null))
		{
			meshRenderer.gameObject.SetActive(value: false);
			releaseParticles.Play();
			await UniTask.WaitForSeconds(2f);
			if (!(this == null))
			{
				asPoolable.ReturnToPool();
			}
		}
	}

	private async void PlayingMeshGeneration()
	{
		float timer = 0f;
		SetHeightErosion(stompPropsCurve.Evaluate(0f));
		while (timer < stompDuration)
		{
			float time = timer / stompDuration;
			float num = stompPropsCurve.Evaluate(time);
			SetHeightErosion(num);
			timer += Time.deltaTime;
			await UniTask.WaitForEndOfFrame(this);
			if (this == null)
			{
				return;
			}
		}
		poofParticles.Play();
		timer = 0f;
		float finalGenerationDuration = meshGenerationDuration - stompDuration;
		while (timer < finalGenerationDuration)
		{
			float time2 = timer / finalGenerationDuration;
			float num2 = meshGenerationCurve.Evaluate(time2);
			SetHeightErosion(num2);
			timer += Time.deltaTime;
			await UniTask.WaitForEndOfFrame(this);
			if (this == null)
			{
				return;
			}
		}
		SetHeightErosion(meshGenerationCurve.Evaluate(1f));
	}

	private async void PlayingSpawnParticles()
	{
		float timer = 0f;
		Vector3 spawnParticlesPosition = Vector3.zero;
		spawnParticlesPosition.y = spawnParticlesHeightRange.x;
		spawnParticles.Play();
		spawnParticles.transform.localPosition = spawnParticlesPosition;
		while (timer < meshGenerationDuration)
		{
			float time = timer / meshGenerationDuration;
			float t = spawnParticlesHeightCurve.Evaluate(time);
			spawnParticlesPosition.y = Mathf.Lerp(spawnParticlesHeightRange.x, spawnParticlesHeightRange.y, t);
			spawnParticles.transform.localPosition = spawnParticlesPosition;
			timer += Time.deltaTime;
			await UniTask.WaitForEndOfFrame(this);
			if (this == null)
			{
				return;
			}
		}
		spawnParticles.Stop();
	}
}
