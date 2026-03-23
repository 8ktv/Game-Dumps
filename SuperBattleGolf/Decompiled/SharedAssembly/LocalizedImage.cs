using System;
using UnityEngine;
using UnityEngine.Localization.Settings;
using UnityEngine.UI;

public class LocalizedImage : MonoBehaviour
{
	[Serializable]
	public class LocalizedImageSprite
	{
		[SerializeField]
		internal string localeCode;

		[SerializeField]
		internal Sprite sprite;
	}

	[SerializeField]
	private Image image;

	[SerializeField]
	private Sprite defaultSprite;

	[SerializeField]
	private LocalizedImageSprite[] localizedSprites;

	private void Awake()
	{
		RefreshImage();
		LocalizationManager.LanguageChanged += RefreshImage;
	}

	private void OnDestroy()
	{
		LocalizationManager.LanguageChanged -= RefreshImage;
	}

	private void RefreshImage()
	{
		image.sprite = GetSprite();
	}

	private Sprite GetSprite()
	{
		string code = LocalizationSettings.SelectedLocale.Identifier.Code;
		LocalizedImageSprite[] array = localizedSprites;
		foreach (LocalizedImageSprite localizedImageSprite in array)
		{
			if (localizedImageSprite.localeCode == code && localizedImageSprite.sprite != null)
			{
				return localizedImageSprite.sprite;
			}
		}
		return defaultSprite;
	}
}
