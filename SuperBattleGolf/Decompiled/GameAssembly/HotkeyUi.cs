using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Localization;
using UnityEngine.Localization.Components;
using UnityEngine.UI;

public class HotkeyUi : MonoBehaviour
{
	[SerializeField]
	private Image buttonPrompt;

	[SerializeField]
	private Image icon;

	[SerializeField]
	private Image disarmed;

	[SerializeField]
	private RectTransform usesParent;

	[SerializeField]
	private Image useTemplate;

	[SerializeField]
	private float useNormalizedSpacing;

	[SerializeField]
	private new RectTransform name;

	[SerializeField]
	private LocalizeStringEvent nameLocalizeStringEvent;

	[SerializeField]
	private CanvasGroup selected;

	[SerializeField]
	private RectTransform rectTransform;

	[SerializeField]
	private float selectScale = 1.2f;

	private Vector2 defaultSize;

	private Coroutine sizeAnimationCoroutine;

	private readonly List<Image> uses = new List<Image>();

	private bool isInitialized;

	public RectTransform RectTransform => rectTransform;

	private void Awake()
	{
		Initialize();
	}

	private void Initialize()
	{
		if (!isInitialized)
		{
			isInitialized = true;
			defaultSize = rectTransform.sizeDelta;
			selected.alpha = 0f;
			disarmed.enabled = false;
		}
	}

	public void ShowButtonPrompt()
	{
		if (buttonPrompt != null)
		{
			buttonPrompt.gameObject.SetActive(value: true);
		}
	}

	public void HideButtonPrompt()
	{
		if (buttonPrompt != null)
		{
			buttonPrompt.gameObject.SetActive(value: false);
		}
	}

	public void ShowName()
	{
		if (nameLocalizeStringEvent != null)
		{
			nameLocalizeStringEvent.gameObject.SetActive(value: true);
		}
	}

	public void HideName()
	{
		if (nameLocalizeStringEvent != null)
		{
			nameLocalizeStringEvent.gameObject.SetActive(value: false);
		}
	}

	public void SetState(bool greyedOut, bool isDisarmed)
	{
		Color color = icon.color;
		color.a = (greyedOut ? 0.5f : 1f);
		icon.color = color;
		disarmed.enabled = isDisarmed;
	}

	public void SetIcon(Sprite icon)
	{
		if (icon == null)
		{
			this.icon.gameObject.SetActive(value: false);
			return;
		}
		this.icon.gameObject.SetActive(value: true);
		this.icon.sprite = icon;
	}

	public void SetName(LocalizedString localizedName)
	{
		if (!(nameLocalizeStringEvent == null))
		{
			nameLocalizeStringEvent.StringReference = localizedName;
			localizedName.GetLocalizedString();
		}
	}

	public void SetUses(int remainingUses, int maxUses)
	{
		if (maxUses <= 0)
		{
			usesParent.gameObject.SetActive(value: false);
			return;
		}
		usesParent.gameObject.SetActive(value: true);
		float num = (1f - useNormalizedSpacing * (float)(maxUses - 1)) / (float)maxUses;
		int i;
		for (i = 0; i < remainingUses; i++)
		{
			Image image = GetOrCreateUse(i);
			image.gameObject.SetActive(value: true);
			image.rectTransform.anchorMin = new Vector2((float)i * (num + useNormalizedSpacing), image.rectTransform.anchorMin.y);
			image.rectTransform.anchorMax = new Vector2(image.rectTransform.anchorMin.x + num, image.rectTransform.anchorMax.y);
		}
		for (int j = i; j < uses.Count; j++)
		{
			uses[j].gameObject.SetActive(value: false);
		}
		Image GetOrCreateUse(int index)
		{
			if (index < uses.Count)
			{
				return uses[index];
			}
			Image image2 = UnityEngine.Object.Instantiate(useTemplate, usesParent);
			uses.Add(image2);
			return image2;
		}
	}

	public void Expand(bool animate)
	{
		Initialize();
		if (sizeAnimationCoroutine != null)
		{
			StopCoroutine(sizeAnimationCoroutine);
		}
		Vector2 vector = defaultSize * selectScale;
		if (animate && base.gameObject.activeInHierarchy)
		{
			sizeAnimationCoroutine = StartCoroutine(AnimateSelectionRoutine(vector, 1f, 0.1f, BMath.EaseOut));
			return;
		}
		rectTransform.sizeDelta = vector;
		selected.alpha = 1f;
	}

	public void ResetSize(bool animate)
	{
		Initialize();
		if (sizeAnimationCoroutine != null)
		{
			StopCoroutine(sizeAnimationCoroutine);
		}
		if (animate && base.gameObject.activeInHierarchy)
		{
			sizeAnimationCoroutine = StartCoroutine(AnimateSelectionRoutine(defaultSize, 0f, 0.1f, BMath.EaseOut));
			return;
		}
		rectTransform.sizeDelta = defaultSize;
		selected.alpha = 0f;
	}

	private IEnumerator AnimateSelectionRoutine(Vector2 targetSize, float targetSelectionAlpha, float duration, Func<float, float> Easing)
	{
		Vector2 initialSize = rectTransform.sizeDelta;
		float initialAlpha = selected.alpha;
		for (float time = 0f; time < duration; time += Time.deltaTime)
		{
			float arg = time / duration;
			float t = Easing(arg);
			rectTransform.sizeDelta = Vector2.LerpUnclamped(initialSize, targetSize, t);
			selected.alpha = BMath.Lerp(initialAlpha, targetSelectionAlpha, t);
			yield return null;
		}
		rectTransform.sizeDelta = targetSize;
		selected.alpha = targetSelectionAlpha;
	}
}
