using System;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Localization;

public class MainMenuButton : MonoBehaviour, IPointerEnterHandler, IEventSystemHandler, IPointerExitHandler
{
	[Serializable]
	private struct LocAdjustment
	{
		public LocaleIdentifier locale;

		public float fontSize;

		public float widthOffset;
	}

	[SerializeField]
	private Animator animator;

	[SerializeField]
	private ControllerSelectable asSelectable;

	[SerializeField]
	private TextMeshProUGUI label;

	[SerializeField]
	private LocAdjustment[] locAdjustments;

	private bool pointerHover;

	private float defaultWidth;

	private float defaultFontSize;

	private RectTransform rectTransform;

	private static readonly int selectedHash = Animator.StringToHash("selected");

	private void Awake()
	{
		rectTransform = GetComponent<RectTransform>();
		defaultWidth = rectTransform.sizeDelta.x;
		defaultFontSize = label.fontSize;
		OnLocaleChanged();
		LocalizationManager.LanguageChanged += OnLocaleChanged;
	}

	private void OnDestroy()
	{
		LocalizationManager.LanguageChanged -= OnLocaleChanged;
	}

	public void OnPointerEnter(PointerEventData eventData)
	{
		pointerHover = true;
	}

	public void OnPointerExit(PointerEventData eventData)
	{
		pointerHover = false;
	}

	private void OnDisable()
	{
		pointerHover = false;
	}

	private void Update()
	{
		if (InputManager.UsingGamepad)
		{
			pointerHover = false;
			animator.SetBool(selectedHash, asSelectable.IsSelected);
		}
		else
		{
			animator.SetBool(selectedHash, pointerHover);
		}
	}

	private void OnLocaleChanged()
	{
		float num = 0f;
		float fontSize = defaultFontSize;
		LocAdjustment[] array = locAdjustments;
		for (int i = 0; i < array.Length; i++)
		{
			LocAdjustment locAdjustment = array[i];
			LocaleIdentifier locale = locAdjustment.locale;
			if (locale.Code == LocalizationManager.CurrentLanguage)
			{
				num = locAdjustment.widthOffset;
				fontSize = locAdjustment.fontSize;
				break;
			}
		}
		rectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, defaultWidth + num);
		label.fontSize = fontSize;
	}
}
