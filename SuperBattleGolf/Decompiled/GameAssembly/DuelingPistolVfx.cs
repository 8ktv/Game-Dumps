using Cysharp.Threading.Tasks;
using UnityEngine;

public class DuelingPistolVfx : MonoBehaviour
{
	[SerializeField]
	private GameObject muzzleEffects;

	[SerializeField]
	private LineRenderer tracerLine;

	[SerializeField]
	private float tracerDuration = 0.5f;

	public void SetMuzzleEffectsEnabled(bool enabled)
	{
		muzzleEffects.SetActive(enabled);
	}

	public void SetPoints(Vector3 startPoint, Vector3 endPoint)
	{
		tracerLine.SetPosition(0, startPoint);
		tracerLine.SetPosition(1, endPoint);
		PlayingTracer();
	}

	private async void PlayingTracer()
	{
		tracerLine.enabled = true;
		SetTracerWidthMultiplier(1f);
		float timer = 0f;
		while (timer < tracerDuration)
		{
			float num = timer / tracerDuration;
			SetTracerWidthMultiplier(1f - num);
			timer += Time.deltaTime;
			await UniTask.Yield();
			if (this == null)
			{
				return;
			}
		}
		SetTracerWidthMultiplier(0f);
		tracerLine.enabled = false;
	}

	private void SetTracerWidthMultiplier(float multiplier)
	{
		tracerLine.widthMultiplier = multiplier;
	}
}
