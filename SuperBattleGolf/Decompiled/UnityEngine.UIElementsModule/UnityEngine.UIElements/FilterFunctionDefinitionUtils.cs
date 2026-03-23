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
		filterFunctionDefinition.passes[1].computeRequiredReadMarginsCallback = ComputeVerticalBlurMargins;
		filterFunctionDefinition.passes[1].computeRequiredWriteMarginsCallback = ComputeVerticalBlurMargins;
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
		filterFunctionDefinition.passes[0].prepareMaterialPropertyBlockCallback = PrepareBuiltinColorEffectMaterialPropertyBlock;
		return filterFunctionDefinition;
	}

	private static PostProcessingMargins ComputeHorizontalBlurMargins(FilterFunction func)
	{
		float num = Math.Max(0f, func.parameters[0].floatValue);
		float num2 = num * 3f;
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
		float num = Math.Max(0f, func.parameters[0].floatValue);
		float num2 = num * 3f;
		return new PostProcessingMargins
		{
			left = 0f,
			top = num2,
			right = 0f,
			bottom = num2
		};
	}

	private static void PrepareBuiltinColorEffectMaterialPropertyBlock(MaterialPropertyBlock mpb, FilterFunction func)
	{
		Matrix4x4 value = Matrix4x4.identity;
		Color value2 = Color.white;
		float value3 = 0f;
		switch (func.type)
		{
		case FilterFunctionType.Tint:
			value2 = func.parameters[0].colorValue;
			break;
		case FilterFunctionType.Opacity:
			value2.a = Mathf.Clamp01(func.parameters[0].floatValue);
			break;
		case FilterFunctionType.Invert:
			value3 = Mathf.Clamp01(func.parameters[0].floatValue);
			break;
		case FilterFunctionType.Grayscale:
		{
			float num2 = Mathf.Clamp01(func.parameters[0].floatValue);
			value = new Matrix4x4(new Vector4(0.2126f + 0.7874f * (1f - num2), 0.2126f - 0.2126f * (1f - num2), 0.2126f - 0.2126f * (1f - num2), 0f), new Vector4(0.7152f - 0.7152f * (1f - num2), 0.7152f + 0.2848f * (1f - num2), 0.7152f - 0.7152f * (1f - num2), 0f), new Vector4(0.0722f - 0.0722f * (1f - num2), 0.0722f - 0.0722f * (1f - num2), 0.0722f + 0.9278f * (1f - num2), 0f), new Vector4(0f, 0f, 0f, 1f));
			break;
		}
		case FilterFunctionType.Sepia:
		{
			float num = Mathf.Clamp01(func.parameters[0].floatValue);
			value = new Matrix4x4(new Vector4(0.393f + 0.607f * (1f - num), 0.349f - 0.349f * (1f - num), 0.272f - 0.272f * (1f - num), 0f), new Vector4(0.769f - 0.769f * (1f - num), 0.686f + 0.314f * (1f - num), 0.534f - 0.534f * (1f - num), 0f), new Vector4(0.189f - 0.189f * (1f - num), 0.168f - 0.168f * (1f - num), 0.131f + 0.869f * (1f - num), 0f), new Vector4(0f, 0f, 0f, 1f));
			break;
		}
		}
		mpb.SetMatrix("_ColorMatrix", value);
		mpb.SetColor("_ColorTint", value2);
		mpb.SetFloat("_ColorInvert", value3);
	}
}
