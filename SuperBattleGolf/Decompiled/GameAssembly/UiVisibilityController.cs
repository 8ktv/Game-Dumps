using System;
using System.Threading;
using Cysharp.Threading.Tasks;
using UnityEngine;

public class UiVisibilityController : MonoBehaviour
{
	private static UiHidingGroup hidingGroupOverride;

	[SerializeField]
	private SharedUiHidingGroups sharedHidingGroups;

	[SerializeField]
	private bool interactableBlocksRaycastWhileTransparent = true;

	private bool hasCanvas;

	private Canvas canvas;

	private bool hasCanvasGroup;

	private CanvasGroup canvasGroup;

	private bool isHidden;

	private CancellationTokenSource cancellationTokenSource;

	public float DesiredAlpha { get; private set; } = 1f;

	public bool IsHidden
	{
		get
		{
			if (!isHidden)
			{
				if (hasCanvasGroup)
				{
					return canvasGroup.alpha <= 0f;
				}
				return false;
			}
			return true;
		}
	}

	[CCommand("setUiHidingGroupOverride", "", false, false)]
	private static void SetHidingGroupOverride(UiHidingGroup flags)
	{
		hidingGroupOverride = flags;
		RefreshHidingGroups();
	}

	public static void RefreshHidingGroups()
	{
		UiVisibilityController[] array = UnityEngine.Object.FindObjectsByType<UiVisibilityController>(FindObjectsSortMode.None);
		for (int i = 0; i < array.Length; i++)
		{
			array[i].OnUiVisibilityModeChanged();
		}
	}

	[CCommand("hudToggle", "", false, false)]
	private static void ConsoleHudToggle()
	{
		SetHidingGroupOverride((hidingGroupOverride == UiHidingGroup.None) ? UiHidingGroup.Paused : UiHidingGroup.None);
	}

	[CCommand("hudToggleExceptPowerBar", "", false, false)]
	private static void ConsoleToggleExceptPowerBar()
	{
		SetHidingGroupOverride((hidingGroupOverride == UiHidingGroup.None) ? UiHidingGroup.ExceptPowerBar : UiHidingGroup.None);
	}

	[CCommand("hudTogglePlayerTags", "", false, false)]
	private static void ConsoleToggleNameTags()
	{
		SetHidingGroupOverride(hidingGroupOverride ^ UiHidingGroup.NameTags);
	}

	private void Awake()
	{
		hasCanvas = TryGetComponent<Canvas>(out canvas);
		if (!hasCanvas)
		{
			FindOrCreateCanvasGroup();
		}
	}

	private void OnEnable()
	{
		OnUiVisibilityModeChanged();
		GameManager.UiHidingModeChanged += OnUiVisibilityModeChanged;
	}

	private void OnDisable()
	{
		GameManager.UiHidingModeChanged -= OnUiVisibilityModeChanged;
	}

	public void SetDesiredAlpha(float alpha)
	{
		SetDesiredAlphaInternal(alpha, cancelAnimation: true);
	}

	public UniTask AnimatedDesiredAlpha(float targetAlpha, float duration, Func<float, float> Easing)
	{
		CancelAlphaAnimation();
		cancellationTokenSource = new CancellationTokenSource();
		return AnimateAlpha(targetAlpha, duration, Easing, cancellationTokenSource.Token);
	}

	private void FindOrCreateCanvasGroup()
	{
		if (TryGetComponent<CanvasGroup>(out canvasGroup))
		{
			DesiredAlpha = canvasGroup.alpha;
		}
		else
		{
			canvasGroup = base.gameObject.AddComponent<CanvasGroup>();
		}
		hasCanvasGroup = true;
	}

	private void CancelAlphaAnimation()
	{
		if (cancellationTokenSource != null)
		{
			cancellationTokenSource.Cancel();
			cancellationTokenSource = null;
		}
	}

	private async UniTask AnimateAlpha(float targetAlpha, float duration, Func<float, float> Easing, CancellationToken cancellationToken)
	{
		float initialAlpha = DesiredAlpha;
		for (float time = 0f; time < duration; time += Time.deltaTime)
		{
			float t = time / duration;
			SetDesiredAlphaInternal(BMath.Lerp(initialAlpha, targetAlpha, BMath.EaseInOut(t)), cancelAnimation: false);
			await UniTask.Yield();
			if (this == null || cancellationToken.IsCancellationRequested)
			{
				return;
			}
		}
		SetDesiredAlphaInternal(targetAlpha, cancelAnimation: false);
		UpdateInteractable();
	}

	private void SetDesiredAlphaInternal(float alpha, bool cancelAnimation)
	{
		if (!hasCanvasGroup)
		{
			FindOrCreateCanvasGroup();
		}
		if (cancelAnimation)
		{
			CancelAlphaAnimation();
		}
		DesiredAlpha = alpha;
		if (!isHidden)
		{
			canvasGroup.alpha = alpha;
		}
		UpdateInteractable();
	}

	private void UpdateInteractable()
	{
		if (!interactableBlocksRaycastWhileTransparent)
		{
			CanvasGroup obj = canvasGroup;
			bool blocksRaycasts = (canvasGroup.interactable = canvasGroup.alpha > float.Epsilon);
			obj.blocksRaycasts = blocksRaycasts;
		}
	}

	private void OnUiVisibilityModeChanged()
	{
		UiHidingGroup uiHidingGroup = GameManager.HiddenUiGroups | hidingGroupOverride;
		if (!GameSettings.All.General.ShowNameTags)
		{
			uiHidingGroup |= UiHidingGroup.NameTags;
		}
		if (GameSettings.All.General.ButtonPrompts == GameSettings.GeneralSettings.ButtonPromptVisibility.Partial)
		{
			uiHidingGroup |= UiHidingGroup.HudButtonPrompts;
		}
		else if (GameSettings.All.General.ButtonPrompts == GameSettings.GeneralSettings.ButtonPromptVisibility.Off)
		{
			uiHidingGroup |= UiHidingGroup.AllButtonPrompts;
		}
		isHidden = (sharedHidingGroups.HidingGroups & uiHidingGroup) == sharedHidingGroups.HidingGroups;
		if (hasCanvas)
		{
			canvas.enabled = !isHidden;
		}
		if (hasCanvasGroup)
		{
			canvasGroup.alpha = (isHidden ? 0f : DesiredAlpha);
		}
		UpdateInteractable();
	}
}
