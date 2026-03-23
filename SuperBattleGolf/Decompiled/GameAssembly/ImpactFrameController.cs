using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.Rendering.PostProcessing;

public class ImpactFrameController : MonoBehaviour
{
	[SerializeField]
	private PostProcessVolume volume;

	[SerializeField]
	private float impactFrameDuration = 0.12f;

	private float currentTimer;

	private Camera currentCamera;

	private void Start()
	{
		if (SingletonBehaviour<GameManager>.HasInstance)
		{
			currentCamera = GameManager.Camera;
		}
		else
		{
			currentCamera = Camera.main;
		}
		SetImpactFrameEnabled(impactFrameEnabled: false);
	}

	private void SetImpactFrameEnabled(bool impactFrameEnabled)
	{
		if (volume.profile.TryGetSettings<ImpactFrame>(out var outSetting))
		{
			outSetting.enabled.value = impactFrameEnabled;
		}
	}

	public void PlayImpactFrame(Vector3 impactWorldPos)
	{
		if (GameSettings.All.General.FlashingEffects)
		{
			Vector2 value = currentCamera.WorldToViewportPoint(impactWorldPos);
			if (volume.profile.TryGetSettings<ImpactFrame>(out var outSetting))
			{
				outSetting.distortionCenter.value = value;
			}
			if (currentTimer > 0f)
			{
				currentTimer = impactFrameDuration;
			}
			else
			{
				PlayingImpactFrame();
			}
		}
	}

	private async void PlayingImpactFrame()
	{
		currentTimer = impactFrameDuration;
		SetImpactFrameEnabled(impactFrameEnabled: true);
		while (currentTimer > 0f)
		{
			currentTimer -= Time.deltaTime;
			await UniTask.WaitForEndOfFrame();
			if (this == null)
			{
				return;
			}
		}
		SetImpactFrameEnabled(impactFrameEnabled: false);
	}
}
