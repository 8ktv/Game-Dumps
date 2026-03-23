using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class RadialMenuOptionUi : MonoBehaviour
{
	[SerializeField]
	private Image background;

	[SerializeField]
	private Image icon;

	private Material sliceMaterialInstance;

	private float startAngle;

	private float sweepAngle;

	private Color currentColor;

	private float currentThickness;

	private float currentIconRadius;

	private Coroutine highlightAnimationRoutine;

	private static readonly int colorId = Shader.PropertyToID("_Color");

	private static readonly int innerRadiusId = Shader.PropertyToID("_InnerRadius");

	private static readonly int thicknessId = Shader.PropertyToID("_Thickness");

	private static readonly int startAngleId = Shader.PropertyToID("_StartAngle");

	private static readonly int sweepAngleId = Shader.PropertyToID("_SweepAngle");

	private static readonly int radialCutsWitdhId = Shader.PropertyToID("_RadialCutWidth");

	public RectTransform RectTransform { get; private set; }

	public int SelectionIndex { get; private set; } = -1;

	private void Awake()
	{
		RectTransform = base.transform as RectTransform;
		sliceMaterialInstance = UnityEngine.Object.Instantiate(RadialMenu.OptionSettings.Material);
		sliceMaterialInstance.SetFloat(innerRadiusId, RadialMenu.OptionSettings.DefaultInnerRadius);
		SetColor(RadialMenu.OptionSettings.DefaultColor);
		SetThickness(RadialMenu.OptionSettings.DefaultThickness);
		SetIconRadius(RadialMenu.OptionSettings.DefaultIconRadius * RadialMenu.Size);
		sliceMaterialInstance.SetFloat(radialCutsWitdhId, RadialMenu.OptionSettings.SpacingWidth);
		background.material = sliceMaterialInstance;
	}

	private void OnDestroy()
	{
		UnityEngine.Object.Destroy(sliceMaterialInstance);
	}

	public void Initialize(Sprite sprite, int selectionIndex)
	{
		icon.sprite = sprite;
		if (sprite == null)
		{
			icon.gameObject.SetActive(sprite != null);
		}
		SelectionIndex = selectionIndex;
		SetIsHighlighted(isHighlighted: false, instant: true);
	}

	public void SetIsHighlighted(bool isHighlighted, bool instant)
	{
		Color color;
		float num;
		float num2;
		if (isHighlighted)
		{
			color = RadialMenu.OptionSettings.HighlightedColor;
			num = RadialMenu.OptionSettings.HighlightedThickness;
			num2 = RadialMenu.OptionSettings.HighlightedIconRadius * RadialMenu.Size;
		}
		else
		{
			color = RadialMenu.OptionSettings.DefaultColor;
			num = RadialMenu.OptionSettings.DefaultThickness;
			num2 = RadialMenu.OptionSettings.DefaultIconRadius * RadialMenu.Size;
		}
		if (highlightAnimationRoutine != null)
		{
			StopCoroutine(highlightAnimationRoutine);
		}
		if (instant)
		{
			SetColor(color);
			SetThickness(num);
			SetIconRadius(num2);
		}
		else
		{
			highlightAnimationRoutine = StartCoroutine(AnimateHighlightRoutine(color, num, num2, RadialMenu.OptionSettings.HighlightAnimationDuration));
		}
		IEnumerator AnimateHighlightRoutine(Color targetColor, float targetThickness, float targetIconRadius, float duration)
		{
			Color initialColor = currentColor;
			float initialThickness = currentThickness;
			float initialIconRadius = currentIconRadius;
			for (float time = 0f; time < duration; time += Time.deltaTime)
			{
				float t = time / duration;
				float t2 = BMath.EaseOut(t);
				SetColor(Color.LerpUnclamped(initialColor, targetColor, t));
				SetThickness(BMath.Lerp(initialThickness, targetThickness, t2));
				SetIconRadius(BMath.Lerp(initialIconRadius, targetIconRadius, t2));
				yield return null;
			}
			SetColor(targetColor);
			SetThickness(targetThickness);
			SetIconRadius(targetIconRadius);
		}
	}

	private void SetColor(Color color)
	{
		currentColor = color;
		sliceMaterialInstance.SetColor(colorId, currentColor);
	}

	private void SetThickness(float thickness)
	{
		currentThickness = thickness;
		sliceMaterialInstance.SetFloat(thicknessId, currentThickness);
	}

	private void SetIconRadius(float radius)
	{
		currentIconRadius = radius;
		UpdateIconPosition(updateSize: false);
	}

	public void SetSlice(int sliceIndex, int totalSlices)
	{
		sweepAngle = 360f / (float)totalSlices;
		startAngle = (float)sliceIndex * sweepAngle;
		sliceMaterialInstance.SetFloat(startAngleId, startAngle);
		sliceMaterialInstance.SetFloat(sweepAngleId, sweepAngle);
		UpdateIconPosition(updateSize: true);
	}

	private void UpdateIconPosition(bool updateSize)
	{
		float angle = (90f - (startAngle + sweepAngle / 2f)) * (MathF.PI / 180f);
		float angle2 = (90f - startAngle) * (MathF.PI / 180f);
		Vector2 vector = new Vector2(BMath.Cos(angle), BMath.Sin(angle)) * currentIconRadius;
		icon.rectTransform.anchoredPosition = vector;
		if (updateSize)
		{
			Vector2 vector2 = new Vector2(BMath.Cos(angle2), BMath.Sin(angle2)) * currentIconRadius;
			float num = (vector - vector2).magnitude * 1.4142135f;
			icon.rectTransform.sizeDelta = Vector2.one * num;
		}
	}
}
