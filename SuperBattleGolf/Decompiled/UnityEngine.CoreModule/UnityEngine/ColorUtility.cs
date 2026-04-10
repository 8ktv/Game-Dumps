using System;
using System.Runtime.CompilerServices;
using UnityEngine.Bindings;

namespace UnityEngine;

[NativeHeader("Runtime/Math/ColorUtility.h")]
public class ColorUtility
{
	private static ReadOnlySpan<Color32> HtmlColorValues => new Color32[23]
	{
		new Color32(byte.MaxValue, 0, 0, byte.MaxValue),
		new Color32(0, byte.MaxValue, byte.MaxValue, byte.MaxValue),
		new Color32(0, 0, byte.MaxValue, byte.MaxValue),
		new Color32(0, 0, 139, byte.MaxValue),
		new Color32(173, 216, 230, byte.MaxValue),
		new Color32(128, 0, 128, byte.MaxValue),
		new Color32(byte.MaxValue, byte.MaxValue, 0, byte.MaxValue),
		new Color32(0, byte.MaxValue, 0, byte.MaxValue),
		new Color32(byte.MaxValue, 0, byte.MaxValue, byte.MaxValue),
		new Color32(byte.MaxValue, byte.MaxValue, byte.MaxValue, byte.MaxValue),
		new Color32(192, 192, 192, byte.MaxValue),
		new Color32(128, 128, 128, byte.MaxValue),
		new Color32(0, 0, 0, byte.MaxValue),
		new Color32(byte.MaxValue, 165, 0, byte.MaxValue),
		new Color32(165, 42, 42, byte.MaxValue),
		new Color32(128, 0, 0, byte.MaxValue),
		new Color32(0, 128, 0, byte.MaxValue),
		new Color32(128, 128, 0, byte.MaxValue),
		new Color32(0, 0, 128, byte.MaxValue),
		new Color32(0, 128, 128, byte.MaxValue),
		new Color32(0, byte.MaxValue, byte.MaxValue, byte.MaxValue),
		new Color32(byte.MaxValue, 0, byte.MaxValue, byte.MaxValue),
		new Color32(0, 0, 0, 0)
	};

	private static ReadOnlySpan<string> HtmlColorNames => new string[23]
	{
		"red", "cyan", "blue", "darkblue", "lightblue", "purple", "yellow", "lime", "fuchsia", "white",
		"silver", "grey", "black", "orange", "brown", "maroon", "green", "olive", "navy", "teal",
		"aqua", "magenta", "transparent"
	};

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

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static string ToHtmlStringRGB(Color color)
	{
		return ToHtmlStringRGB(in color);
	}

	public static string ToHtmlStringRGB(in Color color)
	{
		Color32 color2 = new Color32((byte)Mathf.Clamp(Mathf.RoundToInt(color.r * 255f), 0, 255), (byte)Mathf.Clamp(Mathf.RoundToInt(color.g * 255f), 0, 255), (byte)Mathf.Clamp(Mathf.RoundToInt(color.b * 255f), 0, 255), 1);
		return $"{color2.r:X2}{color2.g:X2}{color2.b:X2}";
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static string ToHtmlStringRGBA(Color color)
	{
		return ToHtmlStringRGBA(in color);
	}

	public static string ToHtmlStringRGBA(in Color color)
	{
		Color32 color2 = new Color32((byte)Mathf.Clamp(Mathf.RoundToInt(color.r * 255f), 0, 255), (byte)Mathf.Clamp(Mathf.RoundToInt(color.g * 255f), 0, 255), (byte)Mathf.Clamp(Mathf.RoundToInt(color.b * 255f), 0, 255), (byte)Mathf.Clamp(Mathf.RoundToInt(color.a * 255f), 0, 255));
		return $"{color2.r:X2}{color2.g:X2}{color2.b:X2}{color2.a:X2}";
	}

	[VisibleToOtherModules(new string[] { "UnityEngine.TextCoreTextEngineModule" })]
	internal static bool TryParseHtmlString(ReadOnlySpan<char> input, out Color color)
	{
		color = Color.white;
		input = input.Trim();
		if (input.Length == 0)
		{
			return false;
		}
		if (input[0] == '#')
		{
			if (input.Length > 9)
			{
				return false;
			}
			if (!IsHexString(input.Slice(1)))
			{
				return false;
			}
			if (input.Length == 4 || input.Length == 5)
			{
				Span<char> span = stackalloc char[(input.Length - 1) * 2];
				int i = 1;
				int num = 0;
				for (; i < input.Length; i++)
				{
					span[num++] = input[i];
					span[num++] = input[i];
				}
				return TryParseHexColor(span, out color);
			}
			if (input.Length == 7 || input.Length == 9)
			{
				return TryParseHexColor(input.Slice(1), out color);
			}
		}
		else
		{
			for (int j = 0; j < HtmlColorNames.Length; j++)
			{
				if (MemoryExtensions.Equals(input, HtmlColorNames[j], StringComparison.OrdinalIgnoreCase))
				{
					color = HtmlColorValues[j];
					return true;
				}
			}
		}
		return false;
	}

	private static bool IsHexString(ReadOnlySpan<char> span)
	{
		ReadOnlySpan<char> readOnlySpan = span;
		for (int i = 0; i < readOnlySpan.Length; i++)
		{
			char character = readOnlySpan[i];
			if (!Uri.IsHexDigit(character))
			{
				return false;
			}
		}
		return true;
	}

	private static bool TryParseHexColor(ReadOnlySpan<char> hex, out Color color)
	{
		color = Color.white;
		if (hex.Length != 6 && hex.Length != 8)
		{
			return false;
		}
		if (!TryHexToByte(hex.Slice(0, 2), out var result))
		{
			return false;
		}
		if (!TryHexToByte(hex.Slice(2, 2), out var result2))
		{
			return false;
		}
		if (!TryHexToByte(hex.Slice(4, 2), out var result3))
		{
			return false;
		}
		byte result4 = byte.MaxValue;
		if (hex.Length == 8 && !TryHexToByte(hex.Slice(6, 2), out result4))
		{
			return false;
		}
		color = new Color32(result, result2, result3, result4);
		return true;
	}

	private static bool TryHexToByte(ReadOnlySpan<char> span, out byte result)
	{
		result = 0;
		if (span.Length != 2)
		{
			return false;
		}
		int num = HexDigitValue(span[0]);
		int num2 = HexDigitValue(span[1]);
		if (num == -1 || num2 == -1)
		{
			return false;
		}
		result = (byte)((num << 4) | num2);
		return true;
	}

	private static int HexDigitValue(char c)
	{
		if (c >= '0' && c <= '9')
		{
			return c - 48;
		}
		if (c >= 'a' && c <= 'f')
		{
			return c - 97 + 10;
		}
		if (c >= 'A' && c <= 'F')
		{
			return c - 65 + 10;
		}
		return -1;
	}

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern bool DoTryParseHtmlColor_Injected(ref ManagedSpanWrapper htmlString, out Color32 color);
}
