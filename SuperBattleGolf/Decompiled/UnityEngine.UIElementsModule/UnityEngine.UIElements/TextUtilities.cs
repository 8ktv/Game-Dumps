using System;
using UnityEngine.TextCore;
using UnityEngine.TextCore.Text;
using UnityEngine.UIElements.UIR;

namespace UnityEngine.UIElements;

internal static class TextUtilities
{
	public static Func<TextSettings> getEditorTextSettings;

	internal static Func<bool> IsAdvancedTextEnabled;

	private static TextSettings s_TextSettings;

	public static TextSettings textSettings
	{
		get
		{
			if (s_TextSettings == null)
			{
				s_TextSettings = getEditorTextSettings();
			}
			return s_TextSettings;
		}
	}

	private static Vector2 PostProcessMeasuredSize(TextElement te, Vector2 measuredSize, float width, VisualElement.MeasureMode widthMode, float height, VisualElement.MeasureMode heightMode, float pixelsPerPoint)
	{
		float num = measuredSize.x;
		float num2 = measuredSize.y;
		switch (widthMode)
		{
		case VisualElement.MeasureMode.Exactly:
			num = width;
			break;
		case VisualElement.MeasureMode.AtMost:
			num = Mathf.Min(num, width);
			break;
		}
		switch (heightMode)
		{
		case VisualElement.MeasureMode.Exactly:
			num2 = height;
			break;
		case VisualElement.MeasureMode.AtMost:
			num2 = Mathf.Min(num2, height);
			break;
		}
		float num3 = AlignmentUtils.CeilToPixelGrid(num, pixelsPerPoint, 0f);
		float y = AlignmentUtils.CeilToPixelGrid(num2, pixelsPerPoint, 0f);
		Vector2 result = new Vector2(num3, y);
		if (IsAdvancedTextEnabledForElement(te))
		{
			te.uitkTextHandle.ATGMeasuredWidth = num;
			te.uitkTextHandle.ATGRoundedWidth = num3;
			te.uitkTextHandle.LastPixelPerPoint = pixelsPerPoint;
		}
		else
		{
			te.uitkTextHandle.MeasuredWidth = num;
			te.uitkTextHandle.RoundedWidth = num3;
			te.uitkTextHandle.LastPixelPerPoint = pixelsPerPoint;
		}
		return result;
	}

	internal static Vector2 MeasureVisualElementTextSize(TextElement te, string textToMeasure, float width, VisualElement.MeasureMode widthMode, float height, VisualElement.MeasureMode heightMode, float? fontsize = null)
	{
		if (!IsFontAssigned(te))
		{
			return new Vector2(float.NaN, float.NaN);
		}
		float num = te.panel?.scaledPixelsPerPoint ?? 1f;
		if (num <= 0f)
		{
			return Vector2.zero;
		}
		Vector2 measuredSize = Vector2.zero;
		if (widthMode != VisualElement.MeasureMode.Exactly || heightMode != VisualElement.MeasureMode.Exactly)
		{
			measuredSize = te.uitkTextHandle.ComputeTextSize(textToMeasure, width, widthMode, height, heightMode, fontsize);
		}
		return PostProcessMeasuredSize(te, measuredSize, width, widthMode, height, heightMode, num);
	}

	internal static Vector2 MeasureVisualElementTextSize(TextElement te, in RenderedText textToMeasure, float width, VisualElement.MeasureMode widthMode, float height, VisualElement.MeasureMode heightMode, float? fontsize = null)
	{
		if (!IsFontAssigned(te))
		{
			return new Vector2(float.NaN, float.NaN);
		}
		float num = te.panel?.scaledPixelsPerPoint ?? 1f;
		if (num <= 0f)
		{
			return Vector2.zero;
		}
		Vector2 measuredSize = Vector2.zero;
		if (widthMode != VisualElement.MeasureMode.Exactly || heightMode != VisualElement.MeasureMode.Exactly)
		{
			measuredSize = te.uitkTextHandle.ComputeTextSize(in textToMeasure, width, height, fontsize);
		}
		return PostProcessMeasuredSize(te, measuredSize, width, widthMode, height, heightMode, num);
	}

	internal static FontAsset GetFontAsset(VisualElement ve)
	{
		if (!object.Equals(ve.computedStyle.unityFontDefinition.fontAsset, null))
		{
			return ve.computedStyle.unityFontDefinition.fontAsset;
		}
		TextSettings textSettingsFrom = GetTextSettingsFrom(ve);
		if (!object.Equals(ve.computedStyle.unityFontDefinition.font, null))
		{
			return textSettingsFrom.GetCachedFontAsset(ve.computedStyle.unityFontDefinition.font);
		}
		if (!object.Equals(ve.computedStyle.unityFont, null))
		{
			return textSettingsFrom.GetCachedFontAsset(ve.computedStyle.unityFont);
		}
		if (!object.Equals(textSettingsFrom, null))
		{
			return textSettingsFrom.defaultFontAsset;
		}
		return null;
	}

	internal static bool IsFontAssigned(VisualElement ve)
	{
		return ve.computedStyle.unityFont != null || !ve.computedStyle.unityFontDefinition.IsEmpty();
	}

	internal static TextSettings GetTextSettingsFrom(VisualElement ve)
	{
		if (ve.panel is RuntimePanel runtimePanel)
		{
			return runtimePanel.panelSettings.textSettings ?? PanelTextSettings.defaultPanelTextSettings;
		}
		return PanelTextSettings.defaultPanelTextSettings;
	}

	internal static bool IsAdvancedTextEnabledForPanel(IPanel panel)
	{
		bool result = false;
		if (panel is RuntimePanel runtimePanel)
		{
			result = runtimePanel.panelSettings?.m_ICUDataAsset != null;
		}
		return result;
	}

	internal static bool IsAdvancedTextEnabledForElement(VisualElement ve)
	{
		if (ve == null)
		{
			return false;
		}
		bool flag = ve.computedStyle.unityTextGenerator == TextGeneratorType.Advanced;
		bool flag2 = flag && IsAdvancedTextEnabledForPanel(ve.panel);
		return flag && flag2;
	}

	internal static TextCoreSettings GetTextCoreSettingsForElement(VisualElement ve, bool ignoreColors)
	{
		FontAsset fontAsset = GetFontAsset(ve);
		if (fontAsset == null)
		{
			return default(TextCoreSettings);
		}
		IResolvedStyle resolvedStyle = ve.resolvedStyle;
		ComputedStyle computedStyle = ve.computedStyle;
		TextShadow textShadow = computedStyle.textShadow;
		float num = TextHandle.ConvertPixelUnitsToTextCoreRelativeUnits(computedStyle.fontSize.value, fontAsset);
		float num2 = Mathf.Clamp(resolvedStyle.unityTextOutlineWidth * num, 0f, 1f);
		float underlaySoftness = Mathf.Clamp(textShadow.blurRadius * num, 0f, 1f);
		float x = ((textShadow.offset.x < 0f) ? Mathf.Max(textShadow.offset.x * num, -1f) : Mathf.Min(textShadow.offset.x * num, 1f));
		float y = ((textShadow.offset.y < 0f) ? Mathf.Max(textShadow.offset.y * num, -1f) : Mathf.Min(textShadow.offset.y * num, 1f));
		Vector2 underlayOffset = new Vector2(x, y);
		Color faceColor;
		Color outlineColor;
		if (ignoreColors)
		{
			faceColor = Color.white;
			Color white = Color.white;
			outlineColor = Color.white;
		}
		else
		{
			bool flag = ((Texture2D)fontAsset.material.mainTexture).format != TextureFormat.Alpha8;
			faceColor = resolvedStyle.color;
			outlineColor = resolvedStyle.unityTextOutlineColor;
			if (num2 < 1E-30f)
			{
				outlineColor.a = 0f;
			}
			Color white = textShadow.color;
			if (flag)
			{
				faceColor = new Color(1f, 1f, 1f, faceColor.a);
			}
			else
			{
				white.r *= faceColor.a;
				white.g *= faceColor.a;
				white.b *= faceColor.a;
				outlineColor.r *= outlineColor.a;
				outlineColor.g *= outlineColor.a;
				outlineColor.b *= outlineColor.a;
			}
		}
		return new TextCoreSettings
		{
			faceColor = faceColor,
			outlineColor = outlineColor,
			outlineWidth = num2,
			underlayColor = textShadow.color,
			underlayOffset = underlayOffset,
			underlaySoftness = underlaySoftness
		};
	}

	public static TextWrappingMode toTextWrappingMode(this WhiteSpace whiteSpace, bool isSingleLineInputField)
	{
		TextWrappingMode result;
		if (isSingleLineInputField)
		{
			if (1 == 0)
			{
			}
			switch (whiteSpace)
			{
			case WhiteSpace.Normal:
			case WhiteSpace.NoWrap:
				result = TextWrappingMode.NoWrap;
				break;
			case WhiteSpace.Pre:
			case WhiteSpace.PreWrap:
				result = TextWrappingMode.PreserveWhitespaceNoWrap;
				break;
			default:
				result = TextWrappingMode.NoWrap;
				break;
			}
			if (1 == 0)
			{
			}
			return result;
		}
		if (1 == 0)
		{
		}
		result = whiteSpace switch
		{
			WhiteSpace.Normal => TextWrappingMode.Normal, 
			WhiteSpace.NoWrap => TextWrappingMode.NoWrap, 
			WhiteSpace.PreWrap => TextWrappingMode.PreserveWhitespace, 
			WhiteSpace.Pre => TextWrappingMode.PreserveWhitespaceNoWrap, 
			_ => TextWrappingMode.Normal, 
		};
		if (1 == 0)
		{
		}
		return result;
	}

	public static UnityEngine.TextCore.TextOverflow toTextCore(this TextOverflow textOverflow, OverflowInternal overflow, TextOverflowPosition position)
	{
		if (position != TextOverflowPosition.End)
		{
			return UnityEngine.TextCore.TextOverflow.Clip;
		}
		if (1 == 0)
		{
		}
		UnityEngine.TextCore.TextOverflow result = ((textOverflow == TextOverflow.Ellipsis && overflow == OverflowInternal.Hidden) ? UnityEngine.TextCore.TextOverflow.Ellipsis : UnityEngine.TextCore.TextOverflow.Clip);
		if (1 == 0)
		{
		}
		return result;
	}
}
