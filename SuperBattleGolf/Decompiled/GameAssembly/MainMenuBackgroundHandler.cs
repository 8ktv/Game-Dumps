using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using UnityEngine;

public class MainMenuBackgroundHandler : MonoBehaviour
{
	[SerializeField]
	private List<MainMenuScenario> scenarios;

	[SerializeField]
	private ParticleSystem courseAmbientVfxContainer;

	[SerializeField]
	private float interval = 6f;

	[SerializeField]
	private float overlayTransitionDuration = 1f;

	[SerializeField]
	private CanvasGroup overlayCanvasGroup;

	private int currentIndex = -1;

	private void Start()
	{
		ShuffleScenarios();
		SetOverlayCanvasAlpha(1f);
		Playing();
	}

	private void SetScenariosActive(int index)
	{
		for (int i = 0; i < scenarios.Count; i++)
		{
			scenarios[i].SetActive(i == index);
			if (i == index && courseAmbientVfxContainer != null)
			{
				courseAmbientVfxContainer.transform.parent = scenarios[i].CameraTransform;
				courseAmbientVfxContainer.transform.localPosition = Vector3.zero;
				courseAmbientVfxContainer.transform.rotation = Quaternion.identity;
				courseAmbientVfxContainer.Play();
			}
		}
	}

	private void SetOverlayCanvasAlpha(float alpha)
	{
		overlayCanvasGroup.alpha = alpha;
	}

	private void ShuffleScenarios()
	{
		for (int num = scenarios.Count - 1; num > 0; num--)
		{
			int index = Random.Range(0, num);
			MainMenuScenario value = scenarios[num];
			scenarios[num] = scenarios[index];
			scenarios[index] = value;
		}
	}

	private async void Playing()
	{
		MainMenuScenario currentScenario = null;
		while (this != null)
		{
			await UniTask.WaitForEndOfFrame();
			if (this == null)
			{
				break;
			}
			SettingOverlayShown(shown: false);
			DelayingOverlayFadeIn();
			if (currentIndex >= scenarios.Count || currentIndex < 0)
			{
				ShuffleScenarios();
				if (scenarios[0] == currentScenario)
				{
					int index = Random.Range(1, scenarios.Count);
					MainMenuScenario value = scenarios[0];
					scenarios[0] = scenarios[index];
					scenarios[index] = value;
				}
				currentIndex = 0;
			}
			currentScenario = scenarios[currentIndex];
			SetScenariosActive(currentIndex);
			await currentScenario.Playing(interval);
			if (this == null)
			{
				break;
			}
			currentIndex++;
		}
	}

	private async void DelayingOverlayFadeIn()
	{
		await UniTask.WaitForSeconds(interval - overlayTransitionDuration);
		if (!(this == null))
		{
			SettingOverlayShown(shown: true);
		}
	}

	private async void SettingOverlayShown(bool shown)
	{
		float timer = 0f;
		float startAlpha = overlayCanvasGroup.alpha;
		float endAlpha = (shown ? 1f : 0f);
		while (timer < overlayTransitionDuration)
		{
			float t = timer / overlayTransitionDuration;
			SetOverlayCanvasAlpha(BMath.Lerp(startAlpha, endAlpha, t));
			timer += Time.deltaTime;
			await UniTask.WaitForEndOfFrame();
			if (this == null)
			{
				return;
			}
		}
		SetOverlayCanvasAlpha(endAlpha);
	}
}
