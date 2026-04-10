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
	private RectTransform terrainLayerSectionParent;

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

	private RectTransform rectTransform;

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
		rectTransform = base.transform as RectTransform;
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

	public static void SetNormalizedPower(float normalizedPower, float labelNormalizedPower)
	{
		if (SingletonBehaviour<SwingPowerBarUi>.HasInstance)
		{
			SingletonBehaviour<SwingPowerBarUi>.Instance.SetNormalizedPowerInternal(normalizedPower, labelNormalizedPower);
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

	private void SetNormalizedPowerInternal(float normalizedPower, float labelNormalizedPower)
	{
		float num = normalizedPower - 1f;
		bool flag = isDisplayingOvercharge;
		isDisplayingOvercharge = num > 0f;
		float f = labelNormalizedPower * 100f - 0.0001f;
		powerLabel.text = BMath.CeilToInt(f).ToString();
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
		terrainLayerSectionParent.gameObject.SetActive(value: true);
		int i = 0;
		foreach (PlayerGolfer.TerrainLayerNormalizedSwingPower item in terrainLayerNormalizedSwingPowers)
		{
			if (item.IsInvalid())
			{
				break;
			}
			bool flag = item.layer >= TerrainLayer.Fairway;
			bool flag2 = item.levelHazard >= LevelHazardType.BreakableIce;
			bool flag3 = item.outOfBoundsHazard >= OutOfBoundsHazard.Water;
			LevelHazardSettings value = default(LevelHazardSettings);
			if (flag2)
			{
				flag2 = GameManager.HazardSettings.levelHazardsByType.TryGetValue(item.levelHazard, out value);
				if (!flag2)
				{
					Debug.LogError($"No settings found for level hazard of type {item.levelHazard}");
				}
			}
			if (!flag && !flag2 && !flag3)
			{
				continue;
			}
			Color b = default(Color);
			Color color = default(Color);
			bool flag4 = false;
			if (flag)
			{
				if (!TerrainManager.Settings.LayerSettings.TryGetValue(item.layer, out var value2))
				{
					Debug.LogError($"Could not get settings for terrain layer {item.layer}", base.gameObject);
					continue;
				}
				b = value2.SwingPowerBarColor;
				flag4 = value2.IsOutOfBounds;
				if (flag4)
				{
					color = value2.SwingPowerBarOutOfBoundsOverlayColor;
				}
			}
			else if (flag2)
			{
				if (!TerrainManager.Settings.LayerSettings.TryGetValue(value.effectiveTerrainLayer, out var value3))
				{
					Debug.LogError($"Could not get settings for terrain layer {value.effectiveTerrainLayer}", base.gameObject);
					continue;
				}
				b = value3.SwingPowerBarColor;
				color = value.swingPowerBarOverlayColor;
				flag4 = false;
			}
			else if (flag3)
			{
				if (!TerrainManager.Settings.OutOfBoundsHazardSettings.TryGetValue(item.outOfBoundsHazard, out var value4))
				{
					Debug.LogError($"Could not get settings for out of bounds hazard {item.outOfBoundsHazard}", base.gameObject);
					continue;
				}
				b = value4.SwingPowerBarColor;
				color = value4.SwingPowerBarOutOfBoundsOverlayColor;
				flag4 = true;
			}
			Image image = GetOrCreateTerrainLayerSection(i++);
			image.gameObject.SetActive(value: true);
			RectTransform obj = image.rectTransform;
			obj.anchorMin = new Vector2(obj.anchorMin.x, GetNormalizedSwingPowerWithOffset(item.startNormalizedPower, 4f));
			obj.anchorMax = new Vector2(obj.anchorMax.x, GetNormalizedSwingPowerWithOffset(item.endNormalizedPower, -4f));
			b = Color.Lerp(Color.black, b, terrainLayerBrightness);
			image.color = b;
			RawImage component = image.transform.GetChild(0).GetComponent<RawImage>();
			if (flag2)
			{
				component.gameObject.SetActive(value: true);
				component.texture = value.swingPowerBarOverlayTexture;
				component.color = color;
				component.rectTransform.position = terrainLayerSectionParent.position;
				Rect uvRect = component.uvRect;
				uvRect.position = new Vector2(0.15f, 0.25f);
				uvRect.size = new Vector2(1.5f, 6f);
				component.uvRect = uvRect;
			}
			else if (flag4)
			{
				component.gameObject.SetActive(value: true);
				component.texture = GameManager.HazardSettings.SwingPowerBarOutOfBoundsHazardOVerlayTexture;
				component.color = color;
				component.rectTransform.position = terrainLayerSectionParent.position;
				Rect uvRect2 = component.uvRect;
				uvRect2.position = new Vector2((0f - Time.time * 2f) % 1f, 0f);
				uvRect2.size = new Vector2(8f, 32f);
				component.uvRect = uvRect2;
			}
			else
			{
				component.gameObject.SetActive(value: false);
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
		float height = terrainLayerSectionParent.rect.height;
		float num = offset / height;
		return BMath.Clamp(normalizedSwingPower - num, 0f, 1f);
	}

	private void HideTerrainLayersInternal()
	{
		terrainLayerSectionParent.gameObject.SetActive(value: false);
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
