using System.Collections.Generic;
using DG.Tweening;
using TMPro;
using Unity.Collections;
using UnityEngine;
using UnityEngine.UI;

public class SwingPowerBarUi : SingletonBehaviour<SwingPowerBarUi>
{
	[SerializeField]
	private Image background;

	[SerializeField]
	private Image powerFill;

	[SerializeField]
	private Image overchargeFill;

	[SerializeField]
	private CanvasGroup fillGroup;

	[SerializeField]
	private RectTransform powerLevel;

	[SerializeField]
	private CanvasGroup powerLevelGroup;

	[SerializeField]
	private TextMeshProUGUI powerLabel;

	[SerializeField]
	private Image flagIcon;

	[SerializeField]
	private GameObject terrainLayerSectionParent;

	[SerializeField]
	private Image terrainLayerSectionTemplate;

	[SerializeField]
	private Color overchargeFillColor;

	[SerializeField]
	[Min(0f)]
	private float defaultLingerTime;

	[SerializeField]
	[Min(0f)]
	private float overchargedPopScaleAddition;

	[SerializeField]
	[Min(0f)]
	private float releasePopScaleAddition;

	[SerializeField]
	[Min(0f)]
	private float overchargedPopDuration;

	[SerializeField]
	[Min(0f)]
	private float releasePopDuration;

	[SerializeField]
	[Min(0f)]
	private float powerLevelFadeDuration;

	[SerializeField]
	[Min(0f)]
	private float flagMaxYawXPosition;

	[SerializeField]
	private UiVisibilityController visibilityController;

	[SerializeField]
	[Range(0f, 1f)]
	private float terrainLayerBrightness = 1f;

	private readonly List<Image> terrainLayerSections = new List<Image>();

	private Color defaultFillColor;

	private bool isDisplayingOvercharge;

	private bool isShowing;

	private double showChangeTimestamp;

	private float initialVisibilityFadeAlpha;

	private Tween popTween;

	private Tween shakeTween;

	private Tween powerLevelVisibilityTween;

	protected override void Awake()
	{
		base.Awake();
		defaultFillColor = powerFill.color;
		powerLevelGroup.alpha = 0f;
		powerLabel.color = new Color(1f, 1f, 1f, 0f);
		fillGroup.alpha = 0f;
		visibilityController.SetDesiredAlpha(0f);
	}

	protected override void OnDestroy()
	{
		popTween?.Kill();
		shakeTween?.Kill();
		powerLevelVisibilityTween?.Kill();
		base.OnDestroy();
	}

	public static void SetNormalizedPower(float power)
	{
		if (SingletonBehaviour<SwingPowerBarUi>.HasInstance)
		{
			SingletonBehaviour<SwingPowerBarUi>.Instance.SetNormalizedPowerInternal(power);
		}
	}

	public static void SetFlagIcon(float flagYaw, float flagNormalizedSwingSpeed)
	{
		if (SingletonBehaviour<SwingPowerBarUi>.HasInstance)
		{
			SingletonBehaviour<SwingPowerBarUi>.Instance.SetFlagIconInternal(flagYaw, flagNormalizedSwingSpeed);
		}
	}

	public static void HideFlagIcon()
	{
		if (SingletonBehaviour<SwingPowerBarUi>.HasInstance)
		{
			SingletonBehaviour<SwingPowerBarUi>.Instance.HideFlagIconInternal();
		}
	}

	public static void SetTerrainLayers(NativeList<PlayerGolfer.TerrainLayerNormalizedSwingPower> terrainLayerNormalizedSwingPowers)
	{
		if (SingletonBehaviour<SwingPowerBarUi>.HasInstance)
		{
			SingletonBehaviour<SwingPowerBarUi>.Instance.SetTerrainLayersInternal(terrainLayerNormalizedSwingPowers);
		}
	}

	public static void HideTerrainLayers()
	{
		if (SingletonBehaviour<SwingPowerBarUi>.HasInstance)
		{
			SingletonBehaviour<SwingPowerBarUi>.Instance.HideTerrainLayersInternal();
		}
	}

	public static void ReleaseSwingCharge()
	{
		if (SingletonBehaviour<SwingPowerBarUi>.HasInstance)
		{
			SingletonBehaviour<SwingPowerBarUi>.Instance.ReleaseSwingChargeInternal();
		}
	}

	public static void CancelSwingCharge()
	{
		if (SingletonBehaviour<SwingPowerBarUi>.HasInstance)
		{
			SingletonBehaviour<SwingPowerBarUi>.Instance.CancelSwingChargeInternal();
		}
	}

	private void Update()
	{
		UpdateVisibility();
		static bool ShouldShow()
		{
			if (GameManager.LocalPlayerAsGolfer == null)
			{
				return false;
			}
			if (GameManager.LocalPlayerAsGolfer.IsAimingSwing)
			{
				return true;
			}
			if (GameManager.LocalPlayerAsGolfer.IsSwinging)
			{
				return true;
			}
			return false;
		}
		void UpdateVisibility()
		{
			bool flag = isShowing;
			isShowing = ShouldShow();
			if (isShowing != flag)
			{
				showChangeTimestamp = Time.timeAsDouble;
				initialVisibilityFadeAlpha = visibilityController.DesiredAlpha;
			}
			if (isShowing)
			{
				if (visibilityController.DesiredAlpha < 1f)
				{
					float desiredAlpha = BMath.LerpClamped(initialVisibilityFadeAlpha, 1f, BMath.EaseOutClamped(BMath.GetTimeSince(showChangeTimestamp) / 0.2f));
					visibilityController.SetDesiredAlpha(desiredAlpha);
				}
			}
			else if (visibilityController.DesiredAlpha > 0f)
			{
				float desiredAlpha2 = BMath.LerpClamped(initialVisibilityFadeAlpha, 0f, BMath.EaseInClamped(BMath.GetTimeSince(showChangeTimestamp) / 0.2f));
				visibilityController.SetDesiredAlpha(desiredAlpha2);
			}
		}
	}

	private void SetNormalizedPowerInternal(float normalizedPower)
	{
		float num = normalizedPower - 1f;
		bool flag = isDisplayingOvercharge;
		isDisplayingOvercharge = num > 0f;
		powerLabel.text = BMath.CeilToInt(normalizedPower * 100f).ToString();
		powerLevelVisibilityTween?.Kill();
		powerLabel.color = Color.white;
		powerLevelGroup.alpha = 1f;
		fillGroup.alpha = 1f;
		bool flag2 = normalizedPower > 1f;
		overchargeFill.gameObject.SetActive(flag2);
		float y = BMath.Clamp01(normalizedPower);
		powerLevel.anchorMin = new Vector2(powerLevel.anchorMin.x, y);
		powerLevel.anchorMax = new Vector2(powerLevel.anchorMax.x, y);
		powerFill.rectTransform.anchorMax = new Vector2(powerFill.rectTransform.anchorMax.x, y);
		if (flag2)
		{
			overchargeFill.rectTransform.anchorMin = new Vector2(overchargeFill.rectTransform.anchorMin.x, y);
			overchargeFill.rectTransform.anchorMax = new Vector2(overchargeFill.rectTransform.anchorMax.x, normalizedPower);
		}
		if (flag && !isDisplayingOvercharge)
		{
			StopShake(0.1f);
			powerFill.color = defaultFillColor;
		}
		else if (!flag && isDisplayingOvercharge)
		{
			Pop(overchargedPopScaleAddition, overchargedPopDuration, Ease.OutQuad);
			shakeTween?.Kill();
			base.transform.localRotation = Quaternion.identity;
			shakeTween = base.transform.DOShakeRotation(1f, new Vector3(0f, 0f, 3f), 30, 90f, fadeOut: false, ShakeRandomnessMode.Harmonic).SetLoops(-1);
			powerFill.color = overchargeFillColor;
		}
	}

	private void SetFlagIconInternal(float flagYaw, float flagNormalizedSwingSpeed)
	{
		float value = BMath.Abs(flagYaw);
		float swingPowerBarFlagPreviewMaxYaw = GameManager.UiSettings.SwingPowerBarFlagPreviewMaxYaw;
		float x = (float)BMath.SignNonZero(flagYaw) * BMath.RemapClamped(0f, swingPowerBarFlagPreviewMaxYaw, 0f, flagMaxYawXPosition, value);
		float a = BMath.InverseLerpClamped(swingPowerBarFlagPreviewMaxYaw, background.rectTransform.sizeDelta.x / 2f, value);
		Color color = flagIcon.color;
		color.a = a;
		flagIcon.color = color;
		Vector3 localPosition = flagIcon.rectTransform.localPosition;
		localPosition.x = x;
		flagIcon.rectTransform.localPosition = localPosition;
		flagIcon.rectTransform.anchorMin = new Vector2(flagIcon.rectTransform.anchorMin.x, flagNormalizedSwingSpeed);
		flagIcon.rectTransform.anchorMax = new Vector2(flagIcon.rectTransform.anchorMax.x, flagNormalizedSwingSpeed);
	}

	private void HideFlagIconInternal()
	{
		Color color = flagIcon.color;
		color.a = 0f;
		flagIcon.color = color;
	}

	private void SetTerrainLayersInternal(NativeList<PlayerGolfer.TerrainLayerNormalizedSwingPower> terrainLayerNormalizedSwingPowers)
	{
		terrainLayerNormalizedSwingPowers.Sort(default(PlayerGolfer.TerrainLayerNormalizedSwingPowerComparer));
		_ = (bool)terrainLayerSectionParent;
		terrainLayerSectionParent.SetActive(value: true);
		int i = 0;
		foreach (PlayerGolfer.TerrainLayerNormalizedSwingPower item in terrainLayerNormalizedSwingPowers)
		{
			if (item.IsInvalid())
			{
				break;
			}
			bool flag = item.layer >= TerrainLayer.Fairway;
			bool flag2 = item.outOfBoundsHazard >= OutOfBoundsHazard.Water;
			if (!flag && !flag2)
			{
				continue;
			}
			Color b = default(Color);
			bool flag3 = false;
			if (flag)
			{
				if (!TerrainManager.Settings.LayerSettings.TryGetValue(item.layer, out var value))
				{
					Debug.LogError($"Could not get settings for terrain layer {item.layer}", base.gameObject);
					continue;
				}
				b = value.SwingPowerBarColor;
				flag3 = value.IsOutOfBounds;
			}
			else if (flag2)
			{
				if (!TerrainManager.Settings.OutOfBoundsHazardSettings.TryGetValue(item.outOfBoundsHazard, out var value2))
				{
					Debug.LogError($"Could not get settings for out of bounds hazard {item.outOfBoundsHazard}", base.gameObject);
					continue;
				}
				b = value2.SwingPowerBarColor;
				flag3 = true;
			}
			Image image = GetOrCreateTerrainLayerSection(i++);
			image.gameObject.SetActive(value: true);
			RawImage component = image.transform.GetChild(0).GetComponent<RawImage>();
			component.gameObject.SetActive(flag3);
			RectTransform rectTransform = image.rectTransform;
			rectTransform.anchorMin = new Vector2(rectTransform.anchorMin.x, GetNormalizedSwingPowerWithOffset(item.startNormalizedPower, 4f));
			rectTransform.anchorMax = new Vector2(rectTransform.anchorMax.x, GetNormalizedSwingPowerWithOffset(item.endNormalizedPower, -4f));
			b = (image.color = Color.Lerp(Color.black, b, terrainLayerBrightness));
			if (flag3)
			{
				component.color = b + new Color(0.1f, 0.1f, 0.1f, 0f);
				Rect uvRect = component.uvRect;
				uvRect.position = new Vector2((0f - Time.time * 2f) % 1f, 0f);
				component.uvRect = uvRect;
			}
		}
		for (; i < terrainLayerSections.Count; i++)
		{
			terrainLayerSections[i].gameObject.SetActive(value: false);
		}
		Image GetOrCreateTerrainLayerSection(int index)
		{
			if (index < terrainLayerSections.Count)
			{
				return terrainLayerSections[index];
			}
			Image image2 = Object.Instantiate(terrainLayerSectionTemplate, terrainLayerSectionTemplate.transform.parent);
			terrainLayerSections.Add(image2);
			return image2;
		}
	}

	private float GetNormalizedSwingPowerWithOffset(float normalizedSwingPower, float offset)
	{
		float height = terrainLayerSectionParent.GetComponent<RectTransform>().rect.height;
		float num = offset / height;
		return BMath.Clamp(normalizedSwingPower - num, 0f, 1f);
	}

	private void HideTerrainLayersInternal()
	{
		terrainLayerSectionParent.SetActive(value: false);
	}

	private void ReleaseSwingChargeInternal()
	{
		Pop(releasePopScaleAddition, releasePopDuration, Ease.InQuad);
		StopShake(0.1f);
		FadeOutPowerLevel();
	}

	private void CancelSwingChargeInternal()
	{
		StopShake(0.1f);
		FadeOutPowerLevel();
	}

	private void StopShake(float duration)
	{
		shakeTween?.Kill();
		shakeTween = base.transform.DORotate(Vector3.zero, duration);
	}

	private void Pop(float scaleAddition, float duration, Ease ease)
	{
		popTween?.Kill();
		base.transform.localScale = Vector3.one * (1f + scaleAddition);
		popTween = base.transform.DOScale(1f, duration).SetEase(ease);
	}

	private void FadeOutPowerLevel()
	{
		powerLevelVisibilityTween?.Kill();
		powerLevelVisibilityTween = DOTween.To(GetAlpha, SetAlpha, 0f, powerLevelFadeDuration).SetEase(Ease.InQuad);
		float GetAlpha()
		{
			return powerLevelGroup.alpha;
		}
		void SetAlpha(float alpha)
		{
			powerLevelGroup.alpha = alpha;
			fillGroup.alpha = alpha;
		}
	}
}
