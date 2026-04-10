using System;

namespace UnityEngine.UIElements;

internal static class FilterFunctionDefinitionUtils
{
	private static FilterFunctionDefinition s_BlurDef;

	private static FilterFunctionDefinition s_TintDef;

	private static FilterFunctionDefinition s_OpacityDef;

	private static FilterFunctionDefinition s_InvertDef;

	private static FilterFunctionDefinition s_GrayscaleDef;

	private static FilterFunctionDefinition s_SepiaDef;

	private static FilterFunctionDefinition s_ContrastDef;

	private static FilterFunctionDefinition s_HueRotateDef;

	public static string GetBuiltinFilterName(FilterFunctionType type)
	{
		return type switch
		{
			FilterFunctionType.Blur => "blur", 
			FilterFunctionType.Tint => "tint", 
			FilterFunctionType.Opacity => "opacity", 
			FilterFunctionType.Invert => "invert", 
			FilterFunctionType.Grayscale => "grayscale", 
			FilterFunctionType.Sepia => "sepia", 
			FilterFunctionType.Contrast => "contrast", 
			FilterFunctionType.HueRotate => "hue-rotate", 
			_ => null, 
		};
	}

	public static FilterFunctionDefinition GetBuiltinDefinition(FilterFunctionType type)
	{
		switch (type)
		{
		case FilterFunctionType.Blur:
			if (s_BlurDef == null)
			{
				s_BlurDef = CreateBlurFilterFunctionDefinition();
			}
			return s_BlurDef;
		case FilterFunctionType.Tint:
			if (s_TintDef == null)
			{
				s_TintDef = CreateColorEffectFilterFunctionDefinition(FilterFunctionType.Tint);
			}
			return s_TintDef;
		case FilterFunctionType.Opacity:
			if (s_OpacityDef == null)
			{
				s_OpacityDef = CreateColorEffectFilterFunctionDefinition(FilterFunctionType.Opacity);
			}
			return s_OpacityDef;
		case FilterFunctionType.Invert:
			if (s_InvertDef == null)
			{
				s_InvertDef = CreateColorEffectFilterFunctionDefinition(FilterFunctionType.Invert);
			}
			return s_InvertDef;
		case FilterFunctionType.Grayscale:
			if (s_GrayscaleDef == null)
			{
				s_GrayscaleDef = CreateColorEffectFilterFunctionDefinition(FilterFunctionType.Grayscale);
			}
			return s_GrayscaleDef;
		case FilterFunctionType.Sepia:
			if (s_SepiaDef == null)
			{
				s_SepiaDef = CreateColorEffectFilterFunctionDefinition(FilterFunctionType.Sepia);
			}
			return s_SepiaDef;
		case FilterFunctionType.Contrast:
			if (s_ContrastDef == null)
			{
				s_ContrastDef = CreateColorEffectFilterFunctionDefinition(FilterFunctionType.Contrast);
			}
			return s_ContrastDef;
		case FilterFunctionType.HueRotate:
			if (s_HueRotateDef == null)
			{
				s_HueRotateDef = CreateColorEffectFilterFunctionDefinition(FilterFunctionType.HueRotate);
			}
			return s_HueRotateDef;
		default:
			return null;
		}
	}

	private static FilterFunctionDefinition CreateBlurFilterFunctionDefinition()
	{
		Material material = new Material(Shader.Find("Hidden/UIR/GaussianBlur"));
		material.hideFlags = HideFlags.HideAndDontSave;
		FilterFunctionDefinition filterFunctionDefinition = ScriptableObject.CreateInstance<FilterFunctionDefinition>();
		filterFunctionDefinition.hideFlags = HideFlags.HideAndDontSave;
		filterFunctionDefinition.filterName = GetBuiltinFilterName(FilterFunctionType.Blur);
		filterFunctionDefinition.parameters = new FilterParameterDeclaration[1]
		{
			new FilterParameterDeclaration
			{
				interpolationDefaultValue = new FilterParameter
				{
					type = FilterParameterType.Float,
					floatValue = 0f
				},
				defaultValue = new FilterParameter
				{
					type = FilterParameterType.Float,
					floatValue = 0f
				}
			}
		};
		filterFunctionDefinition.passes = new PostProcessingPass[2]
		{
			new PostProcessingPass
			{
				material = material,
				passIndex = 0,
				parameterBindings = new ParameterBinding[1]
				{
					new ParameterBinding
					{
						index = 0,
						name = "_Sigma"
					}
				},
				readMargins = default(PostProcessingMargins),
				writeMargins = default(PostProcessingMargins)
			},
			new PostProcessingPass
			{
				material = material,
				passIndex = 1,
				parameterBindings = new ParameterBinding[1]
				{
					new ParameterBinding
					{
						index = 0,
						name = "_Sigma"
					}
				},
				readMargins = default(PostProcessingMargins),
				writeMargins = default(PostProcessingMargins)
			}
		};
		filterFunctionDefinition.passes[0].computeRequiredReadMarginsCallback = ComputeHorizontalBlurMargins;
		filterFunctionDefinition.passes[0].computeRequiredWriteMarginsCallback = ComputeHorizontalBlurMargins;
		filterFunctionDefinition.passes[0].applySettingsCallback = ApplyBlurSettings;
		filterFunctionDefinition.passes[1].computeRequiredReadMarginsCallback = ComputeVerticalBlurMargins;
		filterFunctionDefinition.passes[1].computeRequiredWriteMarginsCallback = ComputeVerticalBlurMargins;
		filterFunctionDefinition.passes[1].applySettingsCallback = ApplyBlurSettings;
		return filterFunctionDefinition;
	}

	private static FilterFunctionDefinition CreateColorEffectFilterFunctionDefinition(FilterFunctionType filterType)
	{
		Material material = new Material(Shader.Find("Hidden/UIR/ColorEffect"));
		material.hideFlags = HideFlags.HideAndDontSave;
		FilterFunctionDefinition filterFunctionDefinition = ScriptableObject.CreateInstance<FilterFunctionDefinition>();
		filterFunctionDefinition.hideFlags = HideFlags.HideAndDontSave;
		filterFunctionDefinition.filterName = GetBuiltinFilterName(filterType);
		FilterParameter interpolationDefaultValue = new FilterParameter
		{
			type = FilterParameterType.Float,
			floatValue = 0f
		};
		FilterParameter defaultValue = new FilterParameter
		{
			type = FilterParameterType.Float,
			floatValue = 0f
		};
		switch (filterType)
		{
		case FilterFunctionType.Tint:
			interpolationDefaultValue = new FilterParameter
			{
				type = FilterParameterType.Color,
				colorValue = Color.white
			};
			defaultValue = new FilterParameter
			{
				type = FilterParameterType.Color,
				colorValue = Color.white
			};
			break;
		case FilterFunctionType.Opacity:
			interpolationDefaultValue = new FilterParameter
			{
				type = FilterParameterType.Float,
				floatValue = 1f
			};
			defaultValue = new FilterParameter
			{
				type = FilterParameterType.Float,
				floatValue = 1f
			};
			break;
		case FilterFunctionType.Invert:
		case FilterFunctionType.Grayscale:
		case FilterFunctionType.Sepia:
		case FilterFunctionType.Contrast:
			defaultValue = new FilterParameter
			{
				type = FilterParameterType.Float,
				floatValue = 1f
			};
			break;
		}
		filterFunctionDefinition.parameters = new FilterParameterDeclaration[1]
		{
			new FilterParameterDeclaration
			{
				interpolationDefaultValue = interpolationDefaultValue,
				defaultValue = defaultValue
			}
		};
		filterFunctionDefinition.passes = new PostProcessingPass[1]
		{
			new PostProcessingPass
			{
				material = material,
				passIndex = 0,
				parameterBindings = new ParameterBinding[1]
				{
					new ParameterBinding
					{
						index = 0,
						name = ""
					}
				},
				readMargins = new PostProcessingMargins
				{
					left = 0f,
					top = 0f,
					right = 0f,
					bottom = 0f
				},
				writeMargins = new PostProcessingMargins
				{
					left = 0f,
					top = 0f,
					right = 0f,
					bottom = 0f
				}
			}
		};
		filterFunctionDefinition.passes[0].applySettingsCallback = ApplySettings;
		return filterFunctionDefinition;
	}

	private static PostProcessingMargins ComputeHorizontalBlurMargins(FilterFunction func)
	{
		float num = Math.Max(0f, func.parameters[0].floatValue);
		int num2 = Mathf.CeilToInt(num * 3f + 1f);
		return new PostProcessingMargins
		{
			left = num2,
			top = 0f,
			right = num2,
			bottom = 0f
		};
	}

	private static PostProcessingMargins ComputeVerticalBlurMargins(FilterFunction func)
	{
		float num = Math.Max(1f, func.parameters[0].floatValue);
		int num2 = Mathf.CeilToInt(num * 3f + 1f);
		return new PostProcessingMargins
		{
			left = 0f,
			top = num2,
			right = 0f,
			bottom = num2
		};
	}

	private static void ApplyBlurSettings(MaterialPropertyBlock mpb, FilterPassContext context)
	{
		float num = Math.Max(0f, context.filterFunction.parameters[0].floatValue);
		float value = num * context.scaledPixelsPerPoint;
		mpb.SetFloat("_Sigma", value);
	}

	private static void ApplySettings(MaterialPropertyBlock mpb, FilterPassContext context)
	{
		Matrix4x4 value = Matrix4x4.identity;
		float value2 = 0f;
		float value3 = 0f;
		FilterFunction filterFunction = context.filterFunction;
		switch (filterFunction.type)
		{
		case FilterFunctionType.Tint:
		{
			Color color = filterFunction.parameters[0].colorValue;
			if (!context.readsGamma)
			{
				color = color.linear;
			}
			color.a = Mathf.Clamp01(color.a);
			color.r = Mathf.Clamp01(color.r * color.a);
			color.g = Mathf.Clamp01(color.g * color.a);
			color.b = Mathf.Clamp01(color.b * color.a);
			value = new Matrix4x4(new Vector4(color.r, 0f, 0f, 0f), new Vector4(0f, color.g, 0f, 0f), new Vector4(0f, 0f, color.b, 0f), new Vector4(0f, 0f, 0f, color.a));
			break;
		}
		case FilterFunctionType.Opacity:
		{
			float num9 = Mathf.Clamp01(filterFunction.parameters[0].floatValue);
			value = new Matrix4x4(new Vector4(num9, 0f, 0f, 0f), new Vector4(0f, num9, 0f, 0f), new Vector4(0f, 0f, num9, 0f), new Vector4(0f, 0f, 0f, num9));
			break;
		}
		case FilterFunctionType.Invert:
			value3 = Mathf.Clamp01(filterFunction.parameters[0].floatValue);
			break;
		case FilterFunctionType.Grayscale:
		{
			float num8 = Mathf.Clamp01(filterFunction.parameters[0].floatValue);
			value = new Matrix4x4(new Vector4(0.2126f + 0.7874f * (1f - num8), 0.2126f - 0.2126f * (1f - num8), 0.2126f - 0.2126f * (1f - num8), 0f), new Vector4(0.7152f - 0.7152f * (1f - num8), 0.7152f + 0.2848f * (1f - num8), 0.7152f - 0.7152f * (1f - num8), 0f), new Vector4(0.0722f - 0.0722f * (1f - num8), 0.0722f - 0.0722f * (1f - num8), 0.0722f + 0.9278f * (1f - num8), 0f), new Vector4(0f, 0f, 0f, 1f));
			break;
		}
		case FilterFunctionType.Sepia:
		{
			float num7 = Mathf.Clamp01(filterFunction.parameters[0].floatValue);
			value = new Matrix4x4(new Vector4(0.393f + 0.607f * (1f - num7), 0.349f - 0.349f * (1f - num7), 0.272f - 0.272f * (1f - num7), 0f), new Vector4(0.769f - 0.769f * (1f - num7), 0.686f + 0.314f * (1f - num7), 0.534f - 0.534f * (1f - num7), 0f), new Vector4(0.189f - 0.189f * (1f - num7), 0.168f - 0.168f * (1f - num7), 0.131f + 0.869f * (1f - num7), 0f), new Vector4(0f, 0f, 0f, 1f));
			break;
		}
		case FilterFunctionType.Contrast:
		{
			float num6 = Mathf.Max(0f, filterFunction.parameters[0].floatValue);
			value2 = (1f - num6) * 0.5f;
			value = new Matrix4x4(new Vector4(num6, 0f, 0f, 0f), new Vector4(0f, num6, 0f, 0f), new Vector4(0f, 0f, num6, 0f), new Vector4(0f, 0f, 0f, 1f));
			break;
		}
		case FilterFunctionType.HueRotate:
		{
			float floatValue = filterFunction.parameters[0].floatValue;
			float num = Mathf.Cos(floatValue);
			float num2 = Mathf.Sin(floatValue);
			float num3 = 0.213f;
			float num4 = 0.715f;
			float num5 = 0.072f;
			value = new Matrix4x4(new Vector4(num3 + num * (1f - num3) + num2 * (0f - num3), num3 + num * (0f - num3) + num2 * 0.143f, num3 + num * (0f - num3) + num2 * (0f - (1f - num3)), 0f), new Vector4(num4 + num * (0f - num4) + num2 * (0f - num4), num4 + num * (1f - num4) + num2 * 0.14f, num4 + num * (0f - num4) + num2 * num4, 0f), new Vector4(num5 + num * (0f - num5) + num2 * (1f - num5), num5 + num * (0f - num5) + num2 * -0.283f, num5 + num * (1f - num5) + num2 * num5, 0f), new Vector4(0f, 0f, 0f, 1f));
			break;
		}
		}
		mpb.SetMatrix("_ColorMatrix", value);
		mpb.SetFloat("_ColorOffset", value2);
		mpb.SetFloat("_ColorInvert", value3);
	}
}
