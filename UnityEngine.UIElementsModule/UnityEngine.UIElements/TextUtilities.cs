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

	internal static Vector2 MeasureVisualElementTextSize(TextElement te, in RenderedText textToMeasure, float width, VisualElement.MeasureMode widthMode, float height, VisualElement.MeasureMode heightMode, float? fontsize = null)
	{
		float num = float.NaN;
		float num2 = float.NaN;
		if (!IsFontAssigned(te))
		{
			return new Vector2(num, num2);
		}
		float num3 = 1f;
		if (te.panel != null)
		{
			num3 = te.scaledPixelsPerPoint;
		}
		if (num3 <= 0f)
		{
			return Vector2.zero;
		}
		if (widthMode != VisualElement.MeasureMode.Exactly || heightMode != VisualElement.MeasureMode.Exactly)
		{
			Vector2 vector = te.uitkTextHandle.ComputeTextSize(in textToMeasure, width, height, fontsize);
			num = vector.x;
			num2 = vector.y;
		}
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
		float num4 = AlignmentUtils.CeilToPixelGrid(num, num3, 0f);
		float y = AlignmentUtils.CeilToPixelGrid(num2, num3, 0f);
		Vector2 vector2 = new Vector2(num4, y);
		if (IsAdvancedTextEnabledForElement(te))
		{
			te.uitkTextHandle.ATGMeasuredSizes = new Vector2(num, num2);
			te.uitkTextHandle.ATGRoundedSizes = vector2;
			te.uitkTextHandle.LastPixelPerPoint = num3;
		}
		else
		{
			te.uitkTextHandle.MeasuredWidth = num;
			te.uitkTextHandle.RoundedWidth = num4;
			te.uitkTextHandle.LastPixelPerPoint = num3;
		}
		return vector2;
	}

	internal static FontAsset GetFontAsset(VisualElement ve)
	{
		if (ve.computedStyle.unityFontDefinition.fontAsset != null)
		{
			return ve.computedStyle.unityFontDefinition.fontAsset;
		}
		TextSettings textSettingsFrom = GetTextSettingsFrom(ve);
		if (ve.computedStyle.unityFontDefinition.font != null)
		{
			return textSettingsFrom.GetCachedFontAsset(ve.computedStyle.unityFontDefinition.font);
		}
		if (ve.computedStyle.unityFont != null)
		{
			return textSettingsFrom.GetCachedFontAsset(ve.computedStyle.unityFont);
		}
		if (textSettingsFrom != null)
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

	internal static bool IsAdvancedTextEnabledForElement(VisualElement ve)
	{
		if (ve == null)
		{
			return false;
		}
		bool flag = ve.computedStyle.unityTextGenerator == TextGeneratorType.Advanced;
		bool flag2 = false;
		if (ve.panel == null)
		{
			return false;
		}
		if (ve.panel is RuntimePanel runtimePanel)
		{
			flag2 = runtimePanel.panelSettings?.m_ICUDataAsset != null;
		}
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

	public static UnityEngine.TextCore.WhiteSpace toTextCore(this WhiteSpace whiteSpace, bool isInputField)
	{
		UnityEngine.TextCore.WhiteSpace result;
		if (isInputField)
		{
			if (1 == 0)
			{
			}
			switch (whiteSpace)
			{
			case WhiteSpace.Normal:
			case WhiteSpace.PreWrap:
				result = UnityEngine.TextCore.WhiteSpace.PreWrap;
				break;
			case WhiteSpace.NoWrap:
			case WhiteSpace.Pre:
				result = UnityEngine.TextCore.WhiteSpace.Pre;
				break;
			default:
				result = UnityEngine.TextCore.WhiteSpace.Pre;
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
			WhiteSpace.Normal => UnityEngine.TextCore.WhiteSpace.Normal, 
			WhiteSpace.NoWrap => UnityEngine.TextCore.WhiteSpace.NoWrap, 
			WhiteSpace.PreWrap => UnityEngine.TextCore.WhiteSpace.PreWrap, 
			WhiteSpace.Pre => UnityEngine.TextCore.WhiteSpace.Pre, 
			_ => UnityEngine.TextCore.WhiteSpace.Normal, 
		};
		if (1 == 0)
		{
		}
		return result;
	}

	public static UnityEngine.TextCore.TextOverflow toTextCore(this TextOverflow textOverflow, OverflowInternal overflow)
	{
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
