using System;
using UnityEngine;
using UnityEngine.Localization;

public class LocalizedWidth : MonoBehaviour
{
	[Serializable]
	private struct LocAdjustment
	{
		public LocaleIdentifier locale;

		public float widthOffset;
	}

	[SerializeField]
	private LocAdjustment[] locAdjustments;

	private RectTransform rectTransform;

	private float defaultWidth;

	private void Awake()
	{
		rectTransform = GetComponent<RectTransform>();
		defaultWidth = rectTransform.sizeDelta.x;
		OnLocaleChanged();
		LocalizationManager.LanguageChanged += OnLocaleChanged;
	}

	private void OnDestroy()
	{
		LocalizationManager.LanguageChanged -= OnLocaleChanged;
	}

	private void OnLocaleChanged()
	{
		float num = 0f;
		LocAdjustment[] array = locAdjustments;
		for (int i = 0; i < array.Length; i++)
		{
			LocAdjustment locAdjustment = array[i];
			LocaleIdentifier locale = locAdjustment.locale;
			if (locale.Code == LocalizationManager.CurrentLanguage)
			{
				num = locAdjustment.widthOffset;
				break;
			}
		}
		rectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, defaultWidth + num);
	}
}
