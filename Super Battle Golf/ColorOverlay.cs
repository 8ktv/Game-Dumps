using System;
using System.Threading;
using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;

public class ColorOverlay : SingletonBehaviour<ColorOverlay>
{
	[SerializeField]
	private Image overlay;

	private bool isFading;

	private float alpha;

	private CancellationTokenSource fadeCancellationTokenSource;

	public static bool IsActive
	{
		get
		{
			if (SingletonBehaviour<ColorOverlay>.HasInstance)
			{
				return SingletonBehaviour<ColorOverlay>.Instance.gameObject.activeSelf;
			}
			return false;
		}
	}

	public static bool IsFading
	{
		get
		{
			if (SingletonBehaviour<ColorOverlay>.HasInstance)
			{
				return SingletonBehaviour<ColorOverlay>.Instance.isFading;
			}
			return false;
		}
	}

	protected override void Awake()
	{
		base.Awake();
		SetAlpha(0f);
	}

	public static void SetColor(Color color)
	{
		if (SingletonBehaviour<ColorOverlay>.HasInstance)
		{
			SingletonBehaviour<ColorOverlay>.Instance.SetColorInternal(color);
		}
	}

	public static void ShowInstantly()
	{
		if (SingletonBehaviour<ColorOverlay>.HasInstance)
		{
			SingletonBehaviour<ColorOverlay>.Instance.ShowInstantlyInternal();
		}
	}

	public static void HideInstantly()
	{
		if (SingletonBehaviour<ColorOverlay>.HasInstance)
		{
			SingletonBehaviour<ColorOverlay>.Instance.HideInstantlyInternal();
		}
	}

	public static UniTask FadeIn(float duration, Func<float, float> Easing = null, bool useUnscaledTime = false)
	{
		if (!SingletonBehaviour<ColorOverlay>.HasInstance)
		{
			return UniTask.CompletedTask;
		}
		return SingletonBehaviour<ColorOverlay>.Instance.FadeInInternal(duration, Easing, useUnscaledTime);
	}

	public static UniTask FadeOut(float duration, Func<float, float> Easing = null, bool useUnscaledTime = false)
	{
		if (!SingletonBehaviour<ColorOverlay>.HasInstance)
		{
			return UniTask.CompletedTask;
		}
		return SingletonBehaviour<ColorOverlay>.Instance.FadeOutInternal(duration, Easing, useUnscaledTime);
	}

	private void SetColorInternal(Color color)
	{
		color.a = alpha;
		overlay.color = color;
	}

	private void ShowInstantlyInternal()
	{
		CancelFade();
		SetAlpha(1f);
	}

	private void HideInstantlyInternal()
	{
		CancelFade();
		SetAlpha(0f);
	}

	private UniTask FadeInInternal(float duration, Func<float, float> Easing, bool useUnscaledTime)
	{
		return FadeTo(1f, duration, Easing, useUnscaledTime);
	}

	private UniTask FadeOutInternal(float duration, Func<float, float> Easing, bool useUnscaledTime)
	{
		return FadeTo(0f, duration, Easing, useUnscaledTime);
	}

	private UniTask FadeTo(float targetAlpha, float fullDuration, Func<float, float> Easing, bool useUnscaledTime)
	{
		CancelFade();
		fadeCancellationTokenSource = new CancellationTokenSource();
		return FadeToInternal(targetAlpha, fullDuration, Easing, useUnscaledTime, fadeCancellationTokenSource.Token);
	}

	private async UniTask FadeToInternal(float targetAlpha, float fullDuration, Func<float, float> Easing, bool useUnscaledTime, CancellationToken cancellationToken)
	{
		isFading = true;
		float time = 0f;
		float initialAlpha = alpha;
		float duration = BMath.Abs(targetAlpha - initialAlpha) * fullDuration;
		while (time < duration)
		{
			await UniTask.Yield(PlayerLoopTiming.LastPostLateUpdate);
			if (cancellationToken.IsCancellationRequested)
			{
				return;
			}
			time += (useUnscaledTime ? Time.unscaledDeltaTime : Time.deltaTime);
			float num = time / duration;
			float t = Easing?.Invoke(num) ?? num;
			SetAlpha(BMath.LerpClamped(initialAlpha, targetAlpha, t));
		}
		SetAlpha(targetAlpha);
		isFading = false;
	}

	private void CancelFade()
	{
		if (isFading)
		{
			if (fadeCancellationTokenSource != null)
			{
				fadeCancellationTokenSource.Cancel();
				fadeCancellationTokenSource = null;
			}
			isFading = false;
		}
	}

	private void SetAlpha(float alpha)
	{
		this.alpha = alpha;
		Color color = overlay.color;
		color.a = alpha;
		overlay.color = color;
		base.gameObject.SetActive(alpha != 0f);
	}
}
