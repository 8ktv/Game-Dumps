#define UNITY_ASSERTIONS
using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using UnityEngine.Bindings;

namespace UnityEngine.TextCore.Text;

[StructLayout(LayoutKind.Sequential)]
[VisibleToOtherModules(new string[] { "UnityEngine.UIElementsModule", "Unity.UIElements.PlayModeTests" })]
[NativeHeader("Modules/TextCoreTextEngine/Native/TextLib.h")]
internal class TextLib
{
	internal static class BindingsMarshaller
	{
		public static IntPtr ConvertToNative(TextLib textLib)
		{
			return textLib.m_Ptr;
		}
	}

	public const int k_unconstrainedScreenSize = -1;

	private readonly IntPtr m_Ptr;

	public static Func<UnityEngine.TextAsset> GetICUAssetEditorDelegate;

	public TextLib(byte[] icuData)
	{
		m_Ptr = GetInstance(icuData);
	}

	private unsafe static IntPtr GetInstance(byte[] icuData)
	{
		Span<byte> span = new Span<byte>(icuData);
		IntPtr instance_Injected;
		fixed (byte* begin = span)
		{
			ManagedSpanWrapper icuData2 = new ManagedSpanWrapper(begin, span.Length);
			instance_Injected = GetInstance_Injected(ref icuData2);
		}
		return instance_Injected;
	}

	public NativeTextInfo GenerateText(NativeTextGenerationSettings settings, IntPtr textGenerationInfo)
	{
		Debug.Assert((settings.fontStyle & FontStyles.Bold) == 0);
		return GenerateTextInternal(settings, textGenerationInfo);
	}

	public void ProcessMeshInfos(NativeTextInfo textInfo, NativeTextGenerationSettings settings)
	{
		Span<ATGMeshInfo> span = MemoryExtensions.AsSpan(textInfo.meshInfos);
		for (int i = 0; i < span.Length; i++)
		{
			ref ATGMeshInfo reference = ref span[i];
			FontAsset fontAsset = (reference.fontAsset = FontAsset.GetFontAssetByID(reference.fontAssetId));
			reference.textElementInfoIndicesByAtlas = new List<List<int>>(fontAsset.atlasTextures.Length);
			for (int j = 0; j < fontAsset.atlasTextures.Length; j++)
			{
				reference.textElementInfoIndicesByAtlas.Add(new List<int>());
			}
			float num = (float)settings.vertexPadding / 64f;
			float num2 = 1f / (float)fontAsset.atlasWidth;
			float num3 = 1f / (float)fontAsset.atlasHeight;
			bool hasMultipleColors = false;
			Color? color = null;
			for (int k = 0; k < reference.textElementInfos.Length; k++)
			{
				ref NativeTextElementInfo reference2 = ref reference.textElementInfos[k];
				int glyphID = reference2.glyphID;
				if (fontAsset.TryAddGlyphInternal((uint)glyphID, out var glyph))
				{
					Color32 color2 = reference2.topLeft.color;
					if (color.HasValue && color.Value != color2)
					{
						hasMultipleColors = true;
					}
					color = color2;
					GlyphRect glyphRect = glyph.glyphRect;
					while (reference.textElementInfoIndicesByAtlas.Count < fontAsset.atlasTextures.Length)
					{
						reference.textElementInfoIndicesByAtlas.Add(new List<int>());
					}
					reference.textElementInfoIndicesByAtlas[glyph.atlasIndex].Add(k);
					if ((reference2.bottomLeft.uv0.x == 0f || reference2.bottomLeft.uv0.x == 1f) && (reference2.bottomLeft.uv0.y == 0f || reference2.bottomLeft.uv0.y == 1f) && (reference2.topLeft.uv0.x == 0f || reference2.topLeft.uv0.x == 1f) && (reference2.topLeft.uv0.y == 0f || reference2.topLeft.uv0.y == 1f) && (reference2.topRight.uv0.x == 0f || reference2.topRight.uv0.x == 1f) && (reference2.topRight.uv0.y == 0f || reference2.topRight.uv0.y == 1f) && (reference2.bottomRight.uv0.x == 0f || reference2.bottomRight.uv0.x == 1f) && (reference2.bottomRight.uv0.y == 0f || reference2.bottomRight.uv0.y == 1f))
					{
						float x = ((float)glyphRect.x - num) * num2;
						float y = ((float)glyphRect.y - num) * num3;
						float x2 = ((float)(glyphRect.x + glyphRect.width) + num) * num2;
						float y2 = ((float)(glyphRect.y + glyphRect.height) + num) * num3;
						reference2.bottomLeft.uv0 = new Vector2(x, y);
						reference2.topLeft.uv0 = new Vector2(x, y2);
						reference2.topRight.uv0 = new Vector2(x2, y2);
						reference2.bottomRight.uv0 = new Vector2(x2, y);
					}
					else
					{
						Vector2 vector = new Vector2(((float)glyphRect.x - num) * num2, ((float)glyphRect.y - num) * num3);
						Vector2 vector2 = new Vector2(vector.x, ((float)(glyphRect.y + glyphRect.height) + num) * num3);
						Vector2 vector3 = new Vector2(((float)(glyphRect.x + glyphRect.width) + num) * num2, vector2.y);
						reference2.bottomLeft.uv0 = vector3 * reference2.bottomLeft.uv0 + vector * (Vector2.one - reference2.bottomLeft.uv0);
						reference2.topLeft.uv0 = vector3 * reference2.topLeft.uv0 + vector * (Vector2.one - reference2.topLeft.uv0);
						reference2.topRight.uv0 = vector3 * reference2.topRight.uv0 + vector * (Vector2.one - reference2.topRight.uv0);
						reference2.bottomRight.uv0 = vector3 * reference2.bottomRight.uv0 + vector * (Vector2.one - reference2.bottomRight.uv0);
					}
				}
			}
			reference.hasMultipleColors = hasMultipleColors;
		}
	}

	[NativeMethod(Name = "TextLib::GenerateTextMesh", IsThreadSafe = true)]
	private NativeTextInfo GenerateTextInternal(NativeTextGenerationSettings settings, IntPtr textGenerationInfo)
	{
		IntPtr intPtr = BindingsMarshaller.ConvertToNative(this);
		if (intPtr == (IntPtr)0)
		{
			ThrowHelper.ThrowNullReferenceException(this);
		}
		GenerateTextInternal_Injected(intPtr, ref settings, textGenerationInfo, out var ret);
		return ret;
	}

	[NativeMethod(Name = "TextLib::MeasureText")]
	public Vector2 MeasureText(NativeTextGenerationSettings settings, IntPtr textGenerationInfo)
	{
		IntPtr intPtr = BindingsMarshaller.ConvertToNative(this);
		if (intPtr == (IntPtr)0)
		{
			ThrowHelper.ThrowNullReferenceException(this);
		}
		MeasureText_Injected(intPtr, ref settings, textGenerationInfo, out var ret);
		return ret;
	}

	[NativeMethod(Name = "TextLib::FindIntersectingLink")]
	public static int FindIntersectingLink(Vector2 point, IntPtr textGenerationInfo)
	{
		return FindIntersectingLink_Injected(ref point, textGenerationInfo);
	}

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern IntPtr GetInstance_Injected(ref ManagedSpanWrapper icuData);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern void GenerateTextInternal_Injected(IntPtr _unity_self, [In] ref NativeTextGenerationSettings settings, IntPtr textGenerationInfo, out NativeTextInfo ret);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern void MeasureText_Injected(IntPtr _unity_self, [In] ref NativeTextGenerationSettings settings, IntPtr textGenerationInfo, out Vector2 ret);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern int FindIntersectingLink_Injected([In] ref Vector2 point, IntPtr textGenerationInfo);
}
