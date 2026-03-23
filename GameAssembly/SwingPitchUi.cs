using System;
using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class SwingPitchUi : SingletonBehaviour<SwingPitchUi>
{
	[SerializeField]
	private GameObject pivot;

	[SerializeField]
	private Image pitchLine;

	[SerializeField]
	private Image pitchArc;

	[SerializeField]
	private TextMeshProUGUI pitchText;

	[SerializeField]
	private UiVisibilityController visibilityController;

	[SerializeField]
	private Vector2 centeredPosition;

	[SerializeField]
	private float lingerDuration;

	private RectTransform rectTransform;

	private Vector2 defaultPosition;

	private bool isCentered;

	private bool isMovingToCenter;

	private bool isMovingToDefaultPosition;

	private bool shouldBeCentered;

	private float displayedPitch = float.MinValue;

	private double lastChangeTimestamp = double.MinValue;

	private Coroutine animationCoroutine;

	protected override void Awake()
	{
		base.Awake();
		rectTransform = base.transform as RectTransform;
		defaultPosition = rectTransform.anchoredPosition;
		visibilityController.SetDesiredAlpha(1f);
		GameManager.LocalPlayerRegistered += OnLocalPlayerRegistered;
		PlayerInfo.LocalPlayerEnteredGolfCart += OnLocalPlayerEnteredGolfCart;
		PlayerInfo.LocalPlayerExitedGolfCart += OnLocalPlayerExitedGolfCart;
		PlayerSpectator.LocalPlayerIsSpectatingChanged += OnLocalPlayerIsSpectatingChanged;
	}

	protected override void OnDestroy()
	{
		base.OnDestroy();
		GameManager.LocalPlayerRegistered -= OnLocalPlayerRegistered;
		PlayerInfo.LocalPlayerEnteredGolfCart -= OnLocalPlayerEnteredGolfCart;
		PlayerInfo.LocalPlayerExitedGolfCart -= OnLocalPlayerExitedGolfCart;
		PlayerSpectator.LocalPlayerIsSpectatingChanged -= OnLocalPlayerIsSpectatingChanged;
	}

	public static void SetPitch(float pitch)
	{
		if (SingletonBehaviour<SwingPitchUi>.HasInstance)
		{
			SingletonBehaviour<SwingPitchUi>.Instance.SetPitchInternal(pitch);
		}
	}

	private void SetPitchInternal(float pitch)
	{
		float num = displayedPitch;
		displayedPitch = pitch;
		if (displayedPitch != num)
		{
			lastChangeTimestamp = Time.timeAsDouble;
			pitchLine.rectTransform.localRotation = Quaternion.Euler(0f, 0f, pitch);
			pitchArc.fillAmount = pitch / 360f;
			pitchText.text = $"{pitch:0}°";
		}
	}

	private void MoveToCenter()
	{
		if (!isMovingToCenter)
		{
			StopAnimation();
			isMovingToCenter = true;
			visibilityController.SetDesiredAlpha(1f);
			rectTransform.anchoredPosition = centeredPosition;
			isCentered = true;
		}
	}

	private void MoveToDefaultPosition()
	{
		if (isMovingToDefaultPosition)
		{
			return;
		}
		if (!isCentered)
		{
			if (visibilityController.DesiredAlpha < 1f)
			{
				StopAnimation();
				animationCoroutine = StartCoroutine(FadeToRoutine(1f, 0.2f, BMath.EaseIn));
			}
		}
		else
		{
			StopAnimation();
			animationCoroutine = StartCoroutine(MoveToDefaultPositionRoutine());
		}
	}

	private IEnumerator MoveToDefaultPositionRoutine()
	{
		isMovingToDefaultPosition = true;
		yield return FadeToRoutine(0f, 0.2f, BMath.EaseOut);
		yield return new WaitForSeconds(0.2f);
		rectTransform.anchoredPosition = defaultPosition;
		isCentered = false;
		yield return FadeToRoutine(1f, 0.2f, BMath.EaseIn);
		isMovingToDefaultPosition = false;
	}

	private IEnumerator FadeToRoutine(float targetAlpha, float duration, Func<float, float> Easing)
	{
		float time = 0f;
		float initialAlpha = visibilityController.DesiredAlpha;
		if (initialAlpha != targetAlpha)
		{
			for (; time < duration; time += Time.deltaTime)
			{
				float arg = time / duration;
				visibilityController.SetDesiredAlpha(BMath.LerpClamped(initialAlpha, targetAlpha, Easing(arg)));
				yield return null;
			}
			visibilityController.SetDesiredAlpha(targetAlpha);
		}
	}

	private void StopAnimation()
	{
		if (animationCoroutine != null)
		{
			StopCoroutine(animationCoroutine);
		}
		isMovingToCenter = false;
		isMovingToDefaultPosition = false;
	}

	private void UpdateIsVisible()
	{
		pivot.SetActive(IsVisible());
		static bool IsVisible()
		{
			if (GameManager.LocalPlayerInfo == null)
			{
				return false;
			}
			if (GameManager.LocalPlayerInfo.ActiveGolfCartSeat.IsValid())
			{
				return false;
			}
			if (GameManager.LocalPlayerAsSpectator.IsSpectating)
			{
				return false;
			}
			return true;
		}
	}

	private void OnLocalPlayerRegistered()
	{
		UpdateIsVisible();
	}

	private void OnLocalPlayerEnteredGolfCart(bool fromDriverSeatReservation)
	{
		UpdateIsVisible();
	}

	private void OnLocalPlayerExitedGolfCart()
	{
		UpdateIsVisible();
	}

	private void OnLocalPlayerIsSpectatingChanged()
	{
		UpdateIsVisible();
	}
}
