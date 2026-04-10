using System;

namespace UnityEngine.UIElements;

internal static class StyleValueFunctionExtension
{
	public const string k_Var = "var";

	public const string k_Env = "env";

	public const string k_LinearGradient = "linear-gradient";

	public const string k_NoneFilter = "none";

	public const string k_CustomFilter = "filter";

	public const string k_FilterTint = "tint";

	public const string k_FilterOpacity = "opacity";

	public const string k_FilterInvert = "invert";

	public const string k_FilterGrayscale = "grayscale";

	public const string k_FilterSepia = "sepia";

	public const string k_FilterBlur = "blur";

	public const string k_FilterContrast = "contrast";

	public const string k_FilterHueRotate = "hue-rotate";

	public const string k_MaterialProperty = "prop";

	public static StyleValueFunction FromUssString(string ussValue)
	{
		ussValue = ussValue.ToLowerInvariant();
		return ussValue switch
		{
			"var" => StyleValueFunction.Var, 
			"env" => StyleValueFunction.Env, 
			"linear-gradient" => StyleValueFunction.LinearGradient, 
			"none" => StyleValueFunction.NoneFilter, 
			"tint" => StyleValueFunction.FilterTint, 
			"opacity" => StyleValueFunction.FilterOpacity, 
			"invert" => StyleValueFunction.FilterInvert, 
			"grayscale" => StyleValueFunction.FilterGrayscale, 
			"sepia" => StyleValueFunction.FilterSepia, 
			"blur" => StyleValueFunction.FilterBlur, 
			"contrast" => StyleValueFunction.FilterContrast, 
			"hue-rotate" => StyleValueFunction.FilterHueRotate, 
			"prop" => StyleValueFunction.MaterialProperty, 
			_ => throw new ArgumentOutOfRangeException("ussValue", ussValue, "Unknown function name"), 
		};
	}

	public static string ToUssString(this StyleValueFunction svf)
	{
		return svf switch
		{
			StyleValueFunction.Var => "var", 
			StyleValueFunction.Env => "env", 
			StyleValueFunction.LinearGradient => "linear-gradient", 
			StyleValueFunction.NoneFilter => "none", 
			StyleValueFunction.CustomFilter => "filter", 
			StyleValueFunction.FilterTint => "tint", 
			StyleValueFunction.FilterOpacity => "opacity", 
			StyleValueFunction.FilterInvert => "invert", 
			StyleValueFunction.FilterGrayscale => "grayscale", 
			StyleValueFunction.FilterSepia => "sepia", 
			StyleValueFunction.FilterBlur => "blur", 
			StyleValueFunction.FilterContrast => "contrast", 
			StyleValueFunction.FilterHueRotate => "hue-rotate", 
			StyleValueFunction.MaterialProperty => "prop", 
			_ => throw new ArgumentOutOfRangeException("svf", svf, "Unknown StyleValueFunction"), 
		};
	}
}
