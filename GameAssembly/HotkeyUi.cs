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

	private float defaultNameYOffset;

	private readonly List<Image> uses = new List<Image>();

	public RectTransform RectTransform => rectTransform;

	private void Awake()
	{
		defaultSize = rectTransform.sizeDelta;
		if (name != null)
		{
			defaultNameYOffset = name.anchoredPosition.y;
		}
		selected.alpha = 0f;
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

	public void SetIsGreyedOut(bool greyedOut)
	{
		Color color = icon.color;
		color.a = (greyedOut ? 0.5f : 1f);
		icon.color = color;
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
		if (sizeAnimationCoroutine != null)
		{
			StopCoroutine(sizeAnimationCoroutine);
		}
		Vector2 vector = defaultSize * selectScale;
		if (animate && base.gameObject.activeInHierarchy)
		{
			sizeAnimationCoroutine = StartCoroutine(AnimateSizeRoutine(vector, 1f, 0.1f, BMath.EaseOut));
			return;
		}
		rectTransform.sizeDelta = vector;
		selected.alpha = 1f;
	}

	public void ResetSize(bool animate)
	{
		if (sizeAnimationCoroutine != null)
		{
			StopCoroutine(sizeAnimationCoroutine);
		}
		if (animate && base.gameObject.activeInHierarchy)
		{
			sizeAnimationCoroutine = StartCoroutine(AnimateSizeRoutine(defaultSize, 0f, 0.1f, BMath.EaseOut));
			return;
		}
		rectTransform.sizeDelta = defaultSize;
		selected.alpha = 0f;
	}

	private IEnumerator AnimateSizeRoutine(Vector2 targetSize, float selectAlpha, float duration, Func<float, float> Easing)
	{
		Vector2 initialSize = rectTransform.sizeDelta;
		float initialAlpha = selected.alpha;
		for (float time = 0f; time < duration; time += Time.deltaTime)
		{
			float arg = time / duration;
			rectTransform.sizeDelta = Vector2.LerpUnclamped(initialSize, targetSize, Easing(arg));
			selected.alpha = BMath.Lerp(initialAlpha, selectAlpha, Easing(arg));
			yield return null;
		}
		rectTransform.sizeDelta = targetSize;
	}
}
