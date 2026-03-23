using System;
using UnityEngine.Bindings;

namespace UnityEngine.UIElements;

[VisibleToOtherModules(new string[] { "UnityEditor.UIBuilderModule" })]
internal static class StyleValueKeywordExtension
{
	public static string ToUssString(this StyleValueKeyword svk)
	{
		return svk switch
		{
			StyleValueKeyword.Inherit => "inherit", 
			StyleValueKeyword.Initial => "initial", 
			StyleValueKeyword.Auto => "auto", 
			StyleValueKeyword.Unset => "unset", 
			StyleValueKeyword.True => "true", 
			StyleValueKeyword.False => "false", 
			StyleValueKeyword.None => "none", 
			StyleValueKeyword.Cover => "cover", 
			StyleValueKeyword.Contain => "contain", 
			_ => throw new ArgumentOutOfRangeException("svk", svk, "Unknown StyleValueKeyword"), 
		};
	}
}
