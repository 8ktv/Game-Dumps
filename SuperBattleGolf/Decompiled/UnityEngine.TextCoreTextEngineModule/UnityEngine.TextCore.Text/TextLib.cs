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

	public NativeTextInfo GenerateText(NativeTextGenerationSettings settings, IntPtr textGenerationInfo, ref bool wasCached)
	{
		Debug.Assert((settings.fontStyle & FontStyles.Bold) == 0);
		return GenerateTextInternal(settings, textGenerationInfo, ref wasCached);
	}

	public bool HasMissingGlyphs(NativeTextInfo textInfo, ref Dictionary<int, HashSet<uint>> missingGlyphsPerFontAsset)
	{
		Span<ATGMeshInfo> meshInfos = textInfo.meshInfos;
		bool result = false;
		Span<ATGMeshInfo> span = meshInfos;
		for (int i = 0; i < span.Length; i++)
		{
			ref ATGMeshInfo reference = ref span[i];
			TextAsset textAssetByID = TextAsset.GetTextAssetByID(reference.textAssetId);
			HashSet<uint> value = null;
			SpriteAsset spriteAsset = textAssetByID as SpriteAsset;
			if ((object)spriteAsset != null || textAssetByID == null)
			{
				continue;
			}
			Span<NativeTextElementInfo> textElementInfos = reference.textElementInfos;
			for (int j = 0; j < textElementInfos.Length; j++)
			{
				int glyphID = textElementInfos[j].glyphID;
				Glyph glyph = null;
				glyph = ((FontAsset)textAssetByID).GetGlyphInCache((uint)glyphID);
				if (glyph == null)
				{
					result = true;
					if (value == null && !missingGlyphsPerFontAsset.TryGetValue(reference.textAssetId, out value))
					{
						value = new HashSet<uint>();
						missingGlyphsPerFontAsset.Add(reference.textAssetId, value);
					}
					value.Add((uint)glyphID);
				}
			}
		}
		return result;
	}

	public void ProcessMeshInfos(NativeTextInfo textInfo, NativeTextGenerationSettings settings, ref List<List<List<int>>> textElementIndicesByMesh, ref List<bool> hasMultipleColorsByMesh, bool uvsAreGenerated)
	{
		Span<ATGMeshInfo> meshInfos = textInfo.meshInfos;
		int num = 0;
		Span<ATGMeshInfo> span = meshInfos;
		for (int i = 0; i < span.Length; i++)
		{
			ref ATGMeshInfo reference = ref span[i];
			TextAsset textAssetByID = TextAsset.GetTextAssetByID(reference.textAssetId);
			if (textAssetByID == null)
			{
				continue;
			}
			float num2 = 0f;
			float num3 = 0f;
			bool flag = false;
			int num4 = 1;
			List<List<int>> list;
			if (num < textElementIndicesByMesh.Count)
			{
				list = textElementIndicesByMesh[num];
			}
			else
			{
				list = new List<List<int>>();
				textElementIndicesByMesh.Add(list);
			}
			if (textAssetByID is FontAsset fontAsset)
			{
				flag = false;
				num4 = fontAsset.atlasTextures.Length;
				num2 = 1f / (float)fontAsset.atlasWidth;
				num3 = 1f / (float)fontAsset.atlasHeight;
			}
			else if (textAssetByID is SpriteAsset spriteAsset)
			{
				flag = true;
				num4 = 1;
				num2 = 1f / (float)spriteAsset.m_SpriteAtlasTexture.width;
				num3 = 1f / (float)spriteAsset.m_SpriteAtlasTexture.height;
			}
			while (list.Count < num4)
			{
				list.Add(new List<int>());
			}
			float num5 = (float)settings.vertexPadding / 64f;
			bool item = false;
			Color? color = null;
			Span<NativeTextElementInfo> textElementInfos = reference.textElementInfos;
			for (int j = 0; j < textElementInfos.Length; j++)
			{
				ref NativeTextElementInfo reference2 = ref textElementInfos[j];
				int glyphID = reference2.glyphID;
				Glyph glyph = null;
				int num6 = 0;
				if (flag)
				{
					num6 = glyphID - 57344;
					num5 = 0f;
					glyph = ((SpriteAsset)textAssetByID).spriteCharacterTable[num6].glyph;
					if (num6 == -1)
					{
						continue;
					}
				}
				else
				{
					glyph = ((FontAsset)textAssetByID).GetGlyphInCache((uint)glyphID);
					if (glyph == null)
					{
						continue;
					}
				}
				Color32 color2 = reference2.topLeft.color;
				if (color.HasValue && color.Value != color2)
				{
					item = true;
				}
				color = color2;
				GlyphRect glyphRect = glyph.glyphRect;
				textElementIndicesByMesh[num][glyph.atlasIndex].Add(j);
				if (!uvsAreGenerated)
				{
					if ((reference2.bottomLeft.uv0.x == 0f || reference2.bottomLeft.uv0.x == 1f) && (reference2.bottomLeft.uv0.y == 0f || reference2.bottomLeft.uv0.y == 1f) && (reference2.topLeft.uv0.x == 0f || reference2.topLeft.uv0.x == 1f) && (reference2.topLeft.uv0.y == 0f || reference2.topLeft.uv0.y == 1f) && (reference2.topRight.uv0.x == 0f || reference2.topRight.uv0.x == 1f) && (reference2.topRight.uv0.y == 0f || reference2.topRight.uv0.y == 1f) && (reference2.bottomRight.uv0.x == 0f || reference2.bottomRight.uv0.x == 1f) && (reference2.bottomRight.uv0.y == 0f || reference2.bottomRight.uv0.y == 1f))
					{
						float x = ((float)glyphRect.x - num5) * num2;
						float y = ((float)glyphRect.y - num5) * num3;
						float x2 = ((float)(glyphRect.x + glyphRect.width) + num5) * num2;
						float y2 = ((float)(glyphRect.y + glyphRect.height) + num5) * num3;
						reference2.bottomLeft.uv0 = new Vector2(x, y);
						reference2.topLeft.uv0 = new Vector2(x, y2);
						reference2.topRight.uv0 = new Vector2(x2, y2);
						reference2.bottomRight.uv0 = new Vector2(x2, y);
					}
					else
					{
						Vector2 vector = new Vector2(((float)glyphRect.x - num5) * num2, ((float)glyphRect.y - num5) * num3);
						Vector2 vector2 = new Vector2(vector.x, ((float)(glyphRect.y + glyphRect.height) + num5) * num3);
						Vector2 vector3 = new Vector2(((float)(glyphRect.x + glyphRect.width) + num5) * num2, vector2.y);
						reference2.bottomLeft.uv0 = vector3 * reference2.bottomLeft.uv0 + vector * (Vector2.one - reference2.bottomLeft.uv0);
						reference2.topLeft.uv0 = vector3 * reference2.topLeft.uv0 + vector * (Vector2.one - reference2.topLeft.uv0);
						reference2.topRight.uv0 = vector3 * reference2.topRight.uv0 + vector * (Vector2.one - reference2.topRight.uv0);
						reference2.bottomRight.uv0 = vector3 * reference2.bottomRight.uv0 + vector * (Vector2.one - reference2.bottomRight.uv0);
					}
				}
			}
			hasMultipleColorsByMesh.Add(item);
			num++;
		}
	}

	[NativeMethod(IsThreadSafe = true)]
	public void ShapeText(NativeTextGenerationSettings settings, IntPtr textGenerationInfo)
	{
		IntPtr intPtr = BindingsMarshaller.ConvertToNative(this);
		if (intPtr == (IntPtr)0)
		{
			ThrowHelper.ThrowNullReferenceException(this);
		}
		ShapeText_Injected(intPtr, ref settings, textGenerationInfo);
	}

	[NativeMethod(Name = "TextLib::GenerateTextMesh", IsThreadSafe = true)]
	private NativeTextInfo GenerateTextInternal(NativeTextGenerationSettings settings, IntPtr textGenerationInfo, ref bool uvsAreGenerated)
	{
		IntPtr intPtr = BindingsMarshaller.ConvertToNative(this);
		if (intPtr == (IntPtr)0)
		{
			ThrowHelper.ThrowNullReferenceException(this);
		}
		GenerateTextInternal_Injected(intPtr, ref settings, textGenerationInfo, ref uvsAreGenerated, out var ret);
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
	[NativeMethod(Name = "TextLib::GetCharacterCount")]
	public static extern int GetCharacterCount(IntPtr textGenerationInfo);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern IntPtr GetInstance_Injected(ref ManagedSpanWrapper icuData);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern void ShapeText_Injected(IntPtr _unity_self, [In] ref NativeTextGenerationSettings settings, IntPtr textGenerationInfo);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern void GenerateTextInternal_Injected(IntPtr _unity_self, [In] ref NativeTextGenerationSettings settings, IntPtr textGenerationInfo, ref bool uvsAreGenerated, out NativeTextInfo ret);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern void MeasureText_Injected(IntPtr _unity_self, [In] ref NativeTextGenerationSettings settings, IntPtr textGenerationInfo, out Vector2 ret);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern int FindIntersectingLink_Injected([In] ref Vector2 point, IntPtr textGenerationInfo);
}
