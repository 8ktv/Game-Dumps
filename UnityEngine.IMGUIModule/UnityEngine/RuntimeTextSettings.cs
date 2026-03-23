using System.Collections.Generic;
using UnityEngine.TextCore.Text;

namespace UnityEngine;

internal class RuntimeTextSettings : TextSettings
{
	private static RuntimeTextSettings s_DefaultTextSettings;

	private static List<FontAsset> s_FallbackOSFontAssetIMGUIInternal;

	internal static RuntimeTextSettings defaultTextSettings
	{
		get
		{
			if (s_DefaultTextSettings == null)
			{
				s_DefaultTextSettings = ScriptableObject.CreateInstance<RuntimeTextSettings>();
			}
			return s_DefaultTextSettings;
		}
	}

	internal override List<FontAsset> GetStaticFallbackOSFontAsset()
	{
		return s_FallbackOSFontAssetIMGUIInternal;
	}

	internal override void SetStaticFallbackOSFontAsset(List<FontAsset> fontAssets)
	{
		s_FallbackOSFontAssetIMGUIInternal = fontAssets;
	}
}
