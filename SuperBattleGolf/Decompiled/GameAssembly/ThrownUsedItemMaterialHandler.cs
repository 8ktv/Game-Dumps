using Cysharp.Threading.Tasks;
using UnityEngine;

public class ThrownUsedItemMaterialHandler : MonoBehaviour
{
	[SerializeField]
	private MeshRenderer[] meshRenderers;

	private MaterialPropertyBlock props;

	private const float grayscaleTransitionDuration = 0.5f;

	private static readonly int grayscaleEnabledHash = Shader.PropertyToID("_GrayscaleEnable");

	private static readonly int grayscaleValueHash = Shader.PropertyToID("_GrayscaleIntensity");

	private void OnEnable()
	{
		if (props == null)
		{
			props = new MaterialPropertyBlock();
		}
		SetGrayscaleEnabled(grayscaleEnabled: false);
		SetGrayscaleValue(0f);
		UpdateRenderers();
		AnimatingGrayscale();
	}

	private void UpdateRenderers()
	{
		for (int i = 0; i < meshRenderers.Length; i++)
		{
			meshRenderers[i].SetPropertyBlock(props);
		}
	}

	private void SetGrayscaleEnabled(bool grayscaleEnabled)
	{
		props.SetInteger(grayscaleEnabledHash, grayscaleEnabled ? 1 : 0);
	}

	private void SetGrayscaleValue(float grayscaleValue)
	{
		props.SetFloat(grayscaleValueHash, grayscaleValue);
	}

	private async void AnimatingGrayscale()
	{
		SetGrayscaleEnabled(grayscaleEnabled: true);
		UpdateRenderers();
		float timer = 0f;
		while (timer < 0.5f)
		{
			float grayscaleValue = timer / 0.5f;
			SetGrayscaleValue(grayscaleValue);
			UpdateRenderers();
			timer += Time.deltaTime;
			await UniTask.WaitForEndOfFrame();
			if (this == null)
			{
				return;
			}
		}
		SetGrayscaleValue(1f);
		UpdateRenderers();
	}
}
