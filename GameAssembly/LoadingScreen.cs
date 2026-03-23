using System.Collections;
using UnityEngine;

public class LoadingScreen : SingletonBehaviour<LoadingScreen>, IBUpdateCallback, IAnyBUpdateCallback
{
	[SerializeField]
	private Animator stingerAnimator;

	[SerializeField]
	private Animator contentAnimator;

	[SerializeField]
	private RectTransform spinner;

	[SerializeField]
	private CanvasGroup contentGroup;

	[SerializeField]
	private float screenFadeInDuration;

	[SerializeField]
	private float screenFadeOutDuration;

	[SerializeField]
	private float contentFadeInDuration;

	[SerializeField]
	private float contentFadeOutDuration;

	[SerializeField]
	private float spinnerRotationSpeed;

	private bool isVisible;

	private bool isFadingScreenIn;

	private bool isFadingScreenOut;

	private bool isUpdateLoopRegistered;

	private Coroutine visibilityRoutine;

	private Coroutine fadeScreenRoutine;

	private Coroutine fadeContentRoutine;

	private static readonly int shownHash = Animator.StringToHash("shown");

	public static bool IsVisible
	{
		get
		{
			if (SingletonBehaviour<LoadingScreen>.HasInstance)
			{
				return SingletonBehaviour<LoadingScreen>.Instance.isVisible;
			}
			return false;
		}
	}

	public static bool IsFadingScreenIn
	{
		get
		{
			if (SingletonBehaviour<LoadingScreen>.HasInstance)
			{
				return SingletonBehaviour<LoadingScreen>.Instance.isFadingScreenIn;
			}
			return false;
		}
	}

	public static bool IsFadingScreenOut
	{
		get
		{
			if (SingletonBehaviour<LoadingScreen>.HasInstance)
			{
				return SingletonBehaviour<LoadingScreen>.Instance.isFadingScreenOut;
			}
			return false;
		}
	}

	protected override void Awake()
	{
		base.Awake();
	}

	protected override void OnDestroy()
	{
		base.OnDestroy();
		if (!BNetworkManager.IsShuttingDown && isUpdateLoopRegistered)
		{
			BUpdate.DeregisterCallback(this);
		}
	}

	public void OnBUpdate()
	{
		spinner.Rotate(0f, 0f, (0f - spinnerRotationSpeed) * Time.deltaTime);
	}

	public static void Show(bool useUnscaledTime = false)
	{
		if (SingletonBehaviour<LoadingScreen>.HasInstance)
		{
			SingletonBehaviour<LoadingScreen>.Instance.ShowInternal(useUnscaledTime);
		}
	}

	public static void Hide(bool useUnscaledTime = false)
	{
		if (SingletonBehaviour<LoadingScreen>.HasInstance)
		{
			SingletonBehaviour<LoadingScreen>.Instance.HideInternal(useUnscaledTime);
		}
	}

	private void ShowInternal(bool useUnscaledTime)
	{
		if (!isVisible)
		{
			CancelAnimation(skipContentAnimation: false);
			contentGroup.alpha = 0f;
			isVisible = true;
			if (!isUpdateLoopRegistered)
			{
				BUpdate.RegisterCallback(this);
				isUpdateLoopRegistered = true;
			}
			visibilityRoutine = StartCoroutine(ShowRoutine());
		}
		IEnumerator ShowRoutine()
		{
			isFadingScreenIn = true;
			stingerAnimator.SetBool(shownHash, value: true);
			yield return new WaitForSeconds(screenFadeOutDuration * 0.5f);
			contentAnimator.SetBool(shownHash, value: true);
			yield return new WaitForSeconds(screenFadeOutDuration * 0.5f);
			isFadingScreenIn = false;
		}
	}

	private void HideInternal(bool useUnscaledTime)
	{
		if (isVisible)
		{
			CancelAnimation(skipContentAnimation: true);
			isVisible = false;
			BUpdate.DeregisterCallback(this);
			visibilityRoutine = StartCoroutine(HideRoutine());
		}
		IEnumerator HideRoutine()
		{
			isFadingScreenOut = true;
			contentAnimator.SetBool(shownHash, value: false);
			stingerAnimator.SetBool(shownHash, value: false);
			yield return new WaitForSeconds(screenFadeOutDuration);
			isFadingScreenOut = false;
			if (isUpdateLoopRegistered)
			{
				BUpdate.DeregisterCallback(this);
				isUpdateLoopRegistered = false;
			}
		}
	}

	private void CancelAnimation(bool skipContentAnimation)
	{
		isFadingScreenIn = false;
		isFadingScreenOut = false;
		if (visibilityRoutine != null)
		{
			StopCoroutine(visibilityRoutine);
		}
		if (fadeScreenRoutine != null)
		{
			StopCoroutine(fadeScreenRoutine);
		}
		if (!skipContentAnimation && fadeContentRoutine != null)
		{
			StopCoroutine(fadeContentRoutine);
		}
	}
}
