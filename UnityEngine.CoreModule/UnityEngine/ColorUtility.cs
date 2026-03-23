using System;
using System.Runtime.CompilerServices;
using UnityEngine.Bindings;

namespace UnityEngine;

[NativeHeader("Runtime/Math/ColorUtility.h")]
public class ColorUtility
{
	[FreeFunction("TryParseHtmlColor", true)]
	internal unsafe static bool DoTryParseHtmlColor(string htmlString, out Color32 color)
	{
		//The blocks IL_0029 are reachable both inside and outside the pinned region starting at IL_0018. ILSpy has duplicated these blocks in order to place them both within and outside the `fixed` statement.
		try
		{
			ManagedSpanWrapper managedSpanWrapper = default(ManagedSpanWrapper);
			if (!StringMarshaller.TryMarshalEmptyOrNullString(htmlString, ref managedSpanWrapper))
			{
				ReadOnlySpan<char> readOnlySpan = MemoryExtensions.AsSpan(htmlString);
				fixed (char* begin = readOnlySpan)
				{
					managedSpanWrapper = new ManagedSpanWrapper(begin, readOnlySpan.Length);
					return DoTryParseHtmlColor_Injected(ref managedSpanWrapper, out color);
				}
			}
			return DoTryParseHtmlColor_Injected(ref managedSpanWrapper, out color);
		}
		finally
		{
		}
	}

	public static bool TryParseHtmlString(string htmlString, out Color color)
	{
		Color32 color2;
		bool result = DoTryParseHtmlColor(htmlString, out color2);
		color = color2;
		return result;
	}

	public static string ToHtmlStringRGB(Color color)
	{
		Color32 color2 = new Color32((byte)Mathf.Clamp(Mathf.RoundToInt(color.r * 255f), 0, 255), (byte)Mathf.Clamp(Mathf.RoundToInt(color.g * 255f), 0, 255), (byte)Mathf.Clamp(Mathf.RoundToInt(color.b * 255f), 0, 255), 1);
		return $"{color2.r:X2}{color2.g:X2}{color2.b:X2}";
	}

	public static string ToHtmlStringRGBA(Color color)
	{
		Color32 color2 = new Color32((byte)Mathf.Clamp(Mathf.RoundToInt(color.r * 255f), 0, 255), (byte)Mathf.Clamp(Mathf.RoundToInt(color.g * 255f), 0, 255), (byte)Mathf.Clamp(Mathf.RoundToInt(color.b * 255f), 0, 255), (byte)Mathf.Clamp(Mathf.RoundToInt(color.a * 255f), 0, 255));
		return $"{color2.r:X2}{color2.g:X2}{color2.b:X2}{color2.a:X2}";
	}

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern bool DoTryParseHtmlColor_Injected(ref ManagedSpanWrapper htmlString, out Color32 color);
}
