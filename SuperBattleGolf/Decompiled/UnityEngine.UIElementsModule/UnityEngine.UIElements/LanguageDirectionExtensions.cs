using System;
using UnityEngine.TextCore;

namespace UnityEngine.UIElements;

internal static class LanguageDirectionExtensions
{
	internal static UnityEngine.TextCore.LanguageDirection toTextCore(this LanguageDirection dir)
	{
		switch (dir)
		{
		case LanguageDirection.Inherit:
		case LanguageDirection.LTR:
			return UnityEngine.TextCore.LanguageDirection.LTR;
		case LanguageDirection.RTL:
			return UnityEngine.TextCore.LanguageDirection.RTL;
		default:
			throw new ArgumentOutOfRangeException("dir", dir, "impossible to convert value");
		}
	}
}
