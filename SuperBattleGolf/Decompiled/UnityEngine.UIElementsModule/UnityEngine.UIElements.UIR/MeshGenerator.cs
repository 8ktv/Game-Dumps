#define UNITY_ASSERTIONS
using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using Unity.Collections;
using Unity.Collections.LowLevel.Unsafe;
using Unity.Jobs;
using Unity.Profiling;
using UnityEngine.TextCore.LowLevel;
using UnityEngine.TextCore.Text;

namespace UnityEngine.UIElements.UIR;

internal class MeshGenerator : IMeshGenerator, IDisposable
{
	private struct RepeatRectUV
	{
		public Rect rect;

		public Rect uv;
	}

	public struct BackgroundRepeatInstance
	{
		public Rect rect;

		public Rect backgroundRepeatRect;

		public Rect uv;
	}

	public struct BorderParams
	{
		public Rect rect;

		public Color playmodeTintColor;

		public Color leftColor;

		public Color topColor;

		public Color rightColor;

		public Color bottomColor;

		public float leftWidth;

		public float topWidth;

		public float rightWidth;

		public float bottomWidth;

		public Vector2 topLeftRadius;

		public Vector2 topRightRadius;

		public Vector2 bottomRightRadius;

		public Vector2 bottomLeftRadius;

		internal ColorPage leftColorPage;

		internal ColorPage topColorPage;

		internal ColorPage rightColorPage;

		internal ColorPage bottomColorPage;

		internal void ToNativeParams(out MeshBuilderNative.NativeBorderParams nativeBorderParams)
		{
			nativeBorderParams = new MeshBuilderNative.NativeBorderParams
			{
				rect = rect,
				leftColor = leftColor,
				topColor = topColor,
				rightColor = rightColor,
				bottomColor = bottomColor,
				leftWidth = leftWidth,
				topWidth = topWidth,
				rightWidth = rightWidth,
				bottomWidth = bottomWidth,
				topLeftRadius = topLeftRadius,
				topRightRadius = topRightRadius,
				bottomRightRadius = bottomRightRadius,
				bottomLeftRadius = bottomLeftRadius,
				leftColorPage = leftColorPage.ToNativeColorPage(),
				topColorPage = topColorPage.ToNativeColorPage(),
				rightColorPage = rightColorPage.ToNativeColorPage(),
				bottomColorPage = bottomColorPage.ToNativeColorPage()
			};
		}
	}

	public struct RectangleParams
	{
		public Rect rect;

		public Rect uv;

		public Color color;

		public Rect subRect;

		public Rect backgroundRepeatRect;

		public NativePagedList<BackgroundRepeatInstance> backgroundRepeatInstanceList;

		public int backgroundRepeatInstanceListStartIndex;

		public int backgroundRepeatInstanceListEndIndex;

		public BackgroundPosition backgroundPositionX;

		public BackgroundPosition backgroundPositionY;

		public BackgroundRepeat backgroundRepeat;

		public BackgroundSize backgroundSize;

		public Texture texture;

		public Sprite sprite;

		public VectorImage vectorImage;

		public ScaleMode scaleMode;

		public Color playmodeTintColor;

		public Vector2 topLeftRadius;

		public Vector2 topRightRadius;

		public Vector2 bottomRightRadius;

		public Vector2 bottomLeftRadius;

		public Vector2 contentSize;

		public Vector2 textureSize;

		public int leftSlice;

		public int topSlice;

		public int rightSlice;

		public int bottomSlice;

		public float sliceScale;

		internal Rect spriteGeomRect;

		public Vector4 rectInset;

		internal ColorPage colorPage;

		internal MeshGenerationContext.MeshFlags meshFlags;

		public static RectangleParams MakeSolid(Rect rect, Color color, Color playModeTintColor)
		{
			return new RectangleParams
			{
				rect = rect,
				color = color,
				uv = new Rect(0f, 0f, 1f, 1f),
				playmodeTintColor = playModeTintColor
			};
		}

		private static void AdjustUVsForScaleMode(Rect rect, Rect uv, Texture texture, ScaleMode scaleMode, out Rect rectOut, out Rect uvOut)
		{
			float num = Mathf.Abs((float)texture.width * uv.width / ((float)texture.height * uv.height));
			float num2 = rect.width / rect.height;
			switch (scaleMode)
			{
			case ScaleMode.ScaleAndCrop:
				if (num2 > num)
				{
					float num5 = uv.height * (num / num2);
					float num6 = (uv.height - num5) * 0.5f;
					uv = new Rect(uv.x, uv.y + num6, uv.width, num5);
				}
				else
				{
					float num7 = uv.width * (num2 / num);
					float num8 = (uv.width - num7) * 0.5f;
					uv = new Rect(uv.x + num8, uv.y, num7, uv.height);
				}
				break;
			case ScaleMode.ScaleToFit:
				if (num2 > num)
				{
					float num3 = num / num2;
					rect = new Rect(rect.xMin + rect.width * (1f - num3) * 0.5f, rect.yMin, num3 * rect.width, rect.height);
				}
				else
				{
					float num4 = num2 / num;
					rect = new Rect(rect.xMin, rect.yMin + rect.height * (1f - num4) * 0.5f, rect.width, num4 * rect.height);
				}
				break;
			default:
				throw new NotImplementedException();
			case ScaleMode.StretchToFill:
				break;
			}
			rectOut = rect;
			uvOut = uv;
		}

		private static void AdjustSpriteUVsForScaleMode(Rect containerRect, Rect srcRect, Rect spriteGeomRect, Sprite sprite, ScaleMode scaleMode, out Rect rectOut, out Rect uvOut)
		{
			float num = sprite.rect.width / sprite.rect.height;
			float num2 = containerRect.width / containerRect.height;
			Rect rect = spriteGeomRect;
			rect.position -= (Vector2)sprite.bounds.min;
			rect.position /= (Vector2)sprite.bounds.size;
			rect.size /= (Vector2)sprite.bounds.size;
			Vector2 position = rect.position;
			position.y = 1f - rect.size.y - position.y;
			rect.position = position;
			switch (scaleMode)
			{
			case ScaleMode.StretchToFill:
			{
				Vector2 size2 = containerRect.size;
				containerRect.position = rect.position * size2;
				containerRect.size = rect.size * size2;
				break;
			}
			case ScaleMode.ScaleAndCrop:
			{
				Rect b = containerRect;
				if (num2 > num)
				{
					b.height = b.width / num;
					b.position = new Vector2(b.position.x, (0f - (b.height - containerRect.height)) / 2f);
				}
				else
				{
					b.width = b.height * num;
					b.position = new Vector2((0f - (b.width - containerRect.width)) / 2f, b.position.y);
				}
				Vector2 size = b.size;
				b.position += rect.position * size;
				b.size = rect.size * size;
				Rect rect2 = RectIntersection(containerRect, b);
				if (rect2.width < 1E-30f || rect2.height < 1E-30f)
				{
					rect2 = Rect.zero;
				}
				else
				{
					Rect rect3 = rect2;
					rect3.position -= b.position;
					rect3.position /= b.size;
					rect3.size /= b.size;
					Vector2 position2 = rect3.position;
					position2.y = 1f - rect3.size.y - position2.y;
					rect3.position = position2;
					srcRect.position += rect3.position * srcRect.size;
					srcRect.size *= rect3.size;
				}
				containerRect = rect2;
				break;
			}
			case ScaleMode.ScaleToFit:
				if (num2 > num)
				{
					float num3 = num / num2;
					containerRect = new Rect(containerRect.xMin + containerRect.width * (1f - num3) * 0.5f, containerRect.yMin, num3 * containerRect.width, containerRect.height);
				}
				else
				{
					float num4 = num2 / num;
					containerRect = new Rect(containerRect.xMin, containerRect.yMin + containerRect.height * (1f - num4) * 0.5f, containerRect.width, num4 * containerRect.height);
				}
				containerRect.position += rect.position * containerRect.size;
				containerRect.size *= rect.size;
				break;
			default:
				throw new NotImplementedException();
			}
			rectOut = containerRect;
			uvOut = srcRect;
		}

		internal static Rect RectIntersection(Rect a, Rect b)
		{
			Rect zero = Rect.zero;
			zero.min = Vector2.Max(a.min, b.min);
			zero.max = Vector2.Min(a.max, b.max);
			zero.size = Vector2.Max(zero.size, Vector2.zero);
			return zero;
		}

		private static Rect ComputeGeomRect(Sprite sprite)
		{
			Vector2 vector = new Vector2(float.MaxValue, float.MaxValue);
			Vector2 vector2 = new Vector2(float.MinValue, float.MinValue);
			Vector2[] vertices = sprite.vertices;
			foreach (Vector2 rhs in vertices)
			{
				vector = Vector2.Min(vector, rhs);
				vector2 = Vector2.Max(vector2, rhs);
			}
			return new Rect(vector, vector2 - vector);
		}

		private static Rect ComputeUVRect(Sprite sprite)
		{
			Vector2 vector = new Vector2(float.MaxValue, float.MaxValue);
			Vector2 vector2 = new Vector2(float.MinValue, float.MinValue);
			Vector2[] array = sprite.uv;
			foreach (Vector2 rhs in array)
			{
				vector = Vector2.Min(vector, rhs);
				vector2 = Vector2.Max(vector2, rhs);
			}
			return new Rect(vector, vector2 - vector);
		}

		private static Rect ApplyPackingRotation(Rect uv, SpritePackingRotation rotation)
		{
			switch (rotation)
			{
			case SpritePackingRotation.FlipHorizontal:
			{
				uv.position += new Vector2(uv.size.x, 0f);
				Vector2 size2 = uv.size;
				size2.x = 0f - size2.x;
				uv.size = size2;
				break;
			}
			case SpritePackingRotation.FlipVertical:
			{
				uv.position += new Vector2(0f, uv.size.y);
				Vector2 size = uv.size;
				size.y = 0f - size.y;
				uv.size = size;
				break;
			}
			case SpritePackingRotation.Rotate180:
				uv.position += uv.size;
				uv.size = -uv.size;
				break;
			}
			return uv;
		}

		public static RectangleParams MakeTextured(Rect rect, Rect uv, Texture texture, ScaleMode scaleMode, Color playModeTintColor)
		{
			AdjustUVsForScaleMode(rect, uv, texture, scaleMode, out rect, out uv);
			Vector2 vector = new Vector2(texture.width, texture.height);
			return new RectangleParams
			{
				rect = rect,
				subRect = new Rect(0f, 0f, 1f, 1f),
				uv = uv,
				color = Color.white,
				texture = texture,
				contentSize = vector,
				textureSize = vector,
				scaleMode = scaleMode,
				playmodeTintColor = playModeTintColor
			};
		}

		public static RectangleParams MakeSprite(Rect containerRect, Rect subRect, Sprite sprite, ScaleMode scaleMode, Color playModeTintColor, bool hasRadius, ref Vector4 slices, bool useForRepeat = false)
		{
			if (sprite == null || sprite.bounds.size.x < 1E-30f || sprite.bounds.size.y < 1E-30f)
			{
				return default(RectangleParams);
			}
			if (sprite.texture == null)
			{
				Debug.LogWarning("Ignoring textureless sprite named \"" + sprite.name + "\", please import as a VectorImage instead");
				return default(RectangleParams);
			}
			Rect rect = ComputeGeomRect(sprite);
			Rect rect2 = ComputeUVRect(sprite);
			Vector4 border = sprite.border;
			bool flag = border != Vector4.zero || slices != Vector4.zero;
			bool flag2 = subRect != new Rect(0f, 0f, 1f, 1f);
			bool flag3 = scaleMode == ScaleMode.ScaleAndCrop || flag || hasRadius || useForRepeat || flag2;
			if (flag3 && sprite.packed && sprite.packingRotation != SpritePackingRotation.None)
			{
				rect2 = ApplyPackingRotation(rect2, sprite.packingRotation);
			}
			Rect srcRect;
			if (flag2)
			{
				srcRect = subRect;
				srcRect.position *= rect2.size;
				srcRect.position += rect2.position;
				srcRect.size *= rect2.size;
			}
			else
			{
				srcRect = rect2;
			}
			AdjustSpriteUVsForScaleMode(containerRect, srcRect, rect, sprite, scaleMode, out var rectOut, out var uvOut);
			Rect rect3 = rect;
			rect3.size /= (Vector2)sprite.bounds.size;
			rect3.position -= (Vector2)sprite.bounds.min;
			rect3.position /= (Vector2)sprite.bounds.size;
			rect3.position = new Vector2(rect3.position.x, 1f - (rect3.position.y + rect3.height));
			RectangleParams result = new RectangleParams
			{
				rect = rectOut,
				uv = uvOut,
				subRect = rect3,
				color = Color.white,
				texture = (flag3 ? sprite.texture : null),
				sprite = (flag3 ? null : sprite),
				contentSize = sprite.rect.size,
				textureSize = new Vector2(sprite.texture.width, sprite.texture.height),
				spriteGeomRect = rect,
				scaleMode = scaleMode,
				playmodeTintColor = playModeTintColor,
				meshFlags = (sprite.packed ? MeshGenerationContext.MeshFlags.SkipDynamicAtlas : MeshGenerationContext.MeshFlags.None)
			};
			Vector4 vector = new Vector4(border.x, border.w, border.z, border.y);
			if (slices != Vector4.zero && vector != Vector4.zero && vector != slices)
			{
				Debug.LogWarning($"Sprite \"{sprite.name}\" borders {vector} are overridden by style slices {slices}");
			}
			else if (slices == Vector4.zero)
			{
				slices = vector;
			}
			return result;
		}

		public static RectangleParams MakeVectorTextured(Rect rect, Rect uv, VectorImage vectorImage, ScaleMode scaleMode, Color playModeTintColor)
		{
			return new RectangleParams
			{
				rect = rect,
				subRect = new Rect(0f, 0f, 1f, 1f),
				uv = uv,
				color = Color.white,
				vectorImage = vectorImage,
				contentSize = new Vector2(vectorImage.width, vectorImage.height),
				scaleMode = scaleMode,
				playmodeTintColor = playModeTintColor
			};
		}

		internal bool HasRadius(float epsilon)
		{
			return (topLeftRadius.x > epsilon && topLeftRadius.y > epsilon) || (topRightRadius.x > epsilon && topRightRadius.y > epsilon) || (bottomRightRadius.x > epsilon && bottomRightRadius.y > epsilon) || (bottomLeftRadius.x > epsilon && bottomLeftRadius.y > epsilon);
		}

		internal bool HasSlices(float epsilon)
		{
			return (float)leftSlice > epsilon || (float)topSlice > epsilon || (float)rightSlice > epsilon || (float)bottomSlice > epsilon;
		}

		internal void ToNativeParams(out MeshBuilderNative.NativeRectParams nativeRectParams)
		{
			nativeRectParams = new MeshBuilderNative.NativeRectParams
			{
				rect = rect,
				subRect = subRect,
				backgroundRepeatRect = backgroundRepeatRect,
				uv = uv,
				color = color,
				scaleMode = scaleMode,
				topLeftRadius = topLeftRadius,
				topRightRadius = topRightRadius,
				bottomRightRadius = bottomRightRadius,
				bottomLeftRadius = bottomLeftRadius,
				spriteGeomRect = spriteGeomRect,
				contentSize = contentSize,
				textureSize = textureSize,
				texturePixelsPerPoint = 1f,
				leftSlice = leftSlice,
				topSlice = topSlice,
				rightSlice = rightSlice,
				bottomSlice = bottomSlice,
				sliceScale = sliceScale,
				rectInset = rectInset,
				colorPage = colorPage.ToNativeColorPage(),
				meshFlags = (int)meshFlags
			};
		}
	}

	private struct TessellationJobParameters
	{
		public bool isBorderJob;

		public MeshBuilderNative.NativeRectParams rectParams;

		public BorderParams borderParams;

		public UnsafeMeshGenerationNode node;
	}

	private struct TessellationJob : IJobParallelFor
	{
		[ReadOnly]
		public TempMeshAllocator allocator;

		[ReadOnly]
		public NativeSlice<TessellationJobParameters> jobParameters;

		public void Execute(int i)
		{
			TessellationJobParameters tessellationJobParameters = jobParameters[i];
			if (tessellationJobParameters.isBorderJob)
			{
				DrawBorder(tessellationJobParameters.node, ref tessellationJobParameters.borderParams);
				return;
			}
			ref MeshBuilderNative.NativeRectParams rectParams = ref tessellationJobParameters.rectParams;
			if (rectParams.vectorImage != IntPtr.Zero)
			{
				DrawVectorImage(tessellationJobParameters.node, ref rectParams, ExtractHandle<VectorImage>(rectParams.vectorImage));
			}
			else if (rectParams.sprite != IntPtr.Zero)
			{
				DrawSprite(tessellationJobParameters.node, ref rectParams, ExtractHandle<Sprite>(rectParams.sprite));
			}
			else
			{
				DrawRectangle(tessellationJobParameters.node, ref rectParams, ExtractHandle<Texture>(rectParams.texture));
			}
		}

		private T ExtractHandle<T>(IntPtr handlePtr) where T : class
		{
			GCHandle gCHandle = ((handlePtr != IntPtr.Zero) ? GCHandle.FromIntPtr(handlePtr) : default(GCHandle));
			return gCHandle.IsAllocated ? (gCHandle.Target as T) : null;
		}

		private unsafe void DrawBorder(UnsafeMeshGenerationNode node, ref BorderParams borderParams)
		{
			borderParams.ToNativeParams(out var nativeBorderParams);
			MeshWriteDataInterface meshWriteDataInterface = MeshBuilderNative.MakeBorder(ref nativeBorderParams);
			if (meshWriteDataInterface.vertexCount != 0 && meshWriteDataInterface.indexCount != 0)
			{
				NativeSlice<Vertex> slice = UIRenderDevice.PtrToSlice<Vertex>((void*)meshWriteDataInterface.vertices, meshWriteDataInterface.vertexCount);
				NativeSlice<ushort> slice2 = UIRenderDevice.PtrToSlice<ushort>((void*)meshWriteDataInterface.indices, meshWriteDataInterface.indexCount);
				if (slice.Length != 0 && slice2.Length != 0)
				{
					allocator.AllocateTempMesh(slice.Length, slice2.Length, out var vertices, out var indices);
					Debug.Assert(vertices.Length == slice.Length);
					Debug.Assert(indices.Length == slice2.Length);
					vertices.CopyFrom(slice);
					indices.CopyFrom(slice2);
					node.DrawMesh(vertices, indices);
				}
			}
		}

		private unsafe void DrawRectangle(UnsafeMeshGenerationNode node, ref MeshBuilderNative.NativeRectParams rectParams, Texture tex)
		{
			if (rectParams.backgroundRepeatInstanceList != IntPtr.Zero)
			{
				NativePagedList<BackgroundRepeatInstance> nativePagedList = ((GCHandle)rectParams.backgroundRepeatInstanceList).Target as NativePagedList<BackgroundRepeatInstance>;
				int num = rectParams.backgroundRepeatInstanceListEndIndex - rectParams.backgroundRepeatInstanceListStartIndex;
				int num2 = Math.Min(4 * num, (int)UIRenderDevice.maxVerticesPerPage);
				int num3 = Math.Min(6 * num, (int)(UIRenderDevice.maxVerticesPerPage * 3));
				int num4 = num2;
				int num5 = num3;
				allocator.AllocateTempMesh(num4, num5, out var vertices, out var indices);
				NativePagedList<BackgroundRepeatInstance>.Enumerator enumerator = new NativePagedList<BackgroundRepeatInstance>.Enumerator(nativePagedList, rectParams.backgroundRepeatInstanceListStartIndex);
				for (int i = 0; i < num; i++)
				{
					Debug.Assert(enumerator.HasNext());
					BackgroundRepeatInstance next = enumerator.GetNext();
					rectParams.rect = next.rect;
					rectParams.backgroundRepeatRect = next.backgroundRepeatRect;
					rectParams.uv = next.uv;
					MeshWriteDataInterface meshWriteDataInterface = ((!(rectParams.texture != IntPtr.Zero)) ? MeshBuilderNative.MakeSolidRect(ref rectParams) : MeshBuilderNative.MakeTexturedRect(ref rectParams));
					if (meshWriteDataInterface.vertexCount == 0 || meshWriteDataInterface.indexCount == 0)
					{
						continue;
					}
					NativeSlice<Vertex> nativeSlice = UIRenderDevice.PtrToSlice<Vertex>((void*)meshWriteDataInterface.vertices, meshWriteDataInterface.vertexCount);
					NativeSlice<ushort> nativeSlice2 = UIRenderDevice.PtrToSlice<ushort>((void*)meshWriteDataInterface.indices, meshWriteDataInterface.indexCount);
					if (num2 < meshWriteDataInterface.vertexCount || num3 < meshWriteDataInterface.indexCount)
					{
						if (vertices.Length - num2 > 0 && indices.Length - num3 > 0)
						{
							node.DrawMesh(vertices.Slice(0, vertices.Length - num2), indices.Slice(0, indices.Length - num3), tex);
						}
						num4 = Math.Min(Math.Max(meshWriteDataInterface.vertexCount, num4) * 2, (int)UIRenderDevice.maxVerticesPerPage);
						num5 = Math.Min(Math.Max(meshWriteDataInterface.indexCount, num5) * 2, (int)(UIRenderDevice.maxVerticesPerPage * 3));
						allocator.AllocateTempMesh(num4, num5, out vertices, out indices);
						num2 = num4;
						num3 = num5;
					}
					int num6 = vertices.Length - num2;
					void* destination = (byte*)vertices.GetUnsafePtr() + (nint)num6 * (nint)sizeof(Vertex);
					int num7 = meshWriteDataInterface.vertexCount * sizeof(Vertex);
					UnsafeUtility.MemCpy(destination, nativeSlice.GetUnsafePtr(), num7);
					ushort num8 = (ushort)num6;
					num6 = indices.Length - num3;
					for (int j = 0; j < meshWriteDataInterface.indexCount; j++)
					{
						indices[num6 + j] = (ushort)(nativeSlice2[j] + num8);
					}
					num2 -= meshWriteDataInterface.vertexCount;
					num3 -= meshWriteDataInterface.indexCount;
				}
				if (vertices.Length - num2 > 0 && indices.Length - num3 > 0)
				{
					node.DrawMesh(vertices.Slice(0, vertices.Length - num2), indices.Slice(0, indices.Length - num3), tex);
				}
				return;
			}
			MeshWriteDataInterface meshWriteDataInterface2 = ((!(rectParams.texture != IntPtr.Zero)) ? MeshBuilderNative.MakeSolidRect(ref rectParams) : MeshBuilderNative.MakeTexturedRect(ref rectParams));
			if (meshWriteDataInterface2.vertexCount != 0 && meshWriteDataInterface2.indexCount != 0)
			{
				NativeSlice<Vertex> slice = UIRenderDevice.PtrToSlice<Vertex>((void*)meshWriteDataInterface2.vertices, meshWriteDataInterface2.vertexCount);
				NativeSlice<ushort> slice2 = UIRenderDevice.PtrToSlice<ushort>((void*)meshWriteDataInterface2.indices, meshWriteDataInterface2.indexCount);
				if (slice.Length != 0 && slice2.Length != 0)
				{
					allocator.AllocateTempMesh(slice.Length, slice2.Length, out var vertices2, out var indices2);
					Debug.Assert(vertices2.Length == slice.Length);
					Debug.Assert(indices2.Length == slice2.Length);
					vertices2.CopyFrom(slice);
					indices2.CopyFrom(slice2);
					node.DrawMesh(vertices2, indices2, tex);
				}
			}
		}

		private void DrawSprite(UnsafeMeshGenerationNode node, ref MeshBuilderNative.NativeRectParams rectParams, Sprite sprite)
		{
			if (rectParams.spriteTexture == IntPtr.Zero)
			{
				return;
			}
			Texture2D texture = ExtractHandle<Texture2D>(rectParams.spriteTexture);
			Vector2[] array = ExtractHandle<Vector2[]>(rectParams.spriteVertices);
			Vector2[] array2 = ExtractHandle<Vector2[]>(rectParams.spriteUVs);
			ushort[] array3 = ExtractHandle<ushort[]>(rectParams.spriteTriangles);
			if (array3 == null || array3.Length != 0)
			{
				int num = array.Length;
				allocator.AllocateTempMesh(num, array3.Length, out var vertices, out var indices);
				AdjustSpriteWinding(array, array3, indices);
				MeshBuilderNative.NativeColorPage colorPage = rectParams.colorPage;
				Color32 pageAndID = colorPage.pageAndID;
				Color32 flags = new Color32(0, 0, 0, (byte)((colorPage.isValid != 0) ? 1 : 0));
				Color32 opacityColorPages = new Color32(0, 0, colorPage.pageAndID.r, colorPage.pageAndID.g);
				Color32 ids = new Color32(0, 0, 0, colorPage.pageAndID.b);
				for (int i = 0; i < num; i++)
				{
					Vector2 vector = array[i];
					vector -= rectParams.spriteGeomRect.position;
					vector /= rectParams.spriteGeomRect.size;
					vector.y = 1f - vector.y;
					vector *= rectParams.rect.size;
					vector += rectParams.rect.position;
					vertices[i] = new Vertex
					{
						position = new Vector3(vector.x, vector.y, Vertex.nearZ),
						tint = rectParams.color,
						uv = array2[i],
						flags = flags,
						opacityColorPages = opacityColorPages,
						ids = ids
					};
				}
				MeshGenerationContext.MeshFlags meshFlags = (MeshGenerationContext.MeshFlags)rectParams.meshFlags;
				bool flag = meshFlags == MeshGenerationContext.MeshFlags.SkipDynamicAtlas;
				node.DrawMeshInternal(vertices, indices, texture, flag ? TextureOptions.SkipDynamicAtlas : TextureOptions.None);
			}
		}

		private unsafe void DrawVectorImage(UnsafeMeshGenerationNode node, ref MeshBuilderNative.NativeRectParams rectParams, VectorImage vi)
		{
			bool flag = (rectParams.meshFlags & 4) != 0;
			int num = vi.vertices.Length;
			Vertex[] array = new Vertex[num];
			for (int i = 0; i < num; i++)
			{
				VectorImageVertex vectorImageVertex = vi.vertices[i];
				array[i] = new Vertex
				{
					position = vectorImageVertex.position,
					tint = vectorImageVertex.tint,
					uv = vectorImageVertex.uv,
					settingIndex = new Color32((byte)(vectorImageVertex.settingIndex >> 8), (byte)vectorImageVertex.settingIndex, 0, 0),
					flags = vectorImageVertex.flags,
					circle = vectorImageVertex.circle
				};
			}
			MeshWriteDataInterface meshWriteDataInterface = ((!((float)rectParams.leftSlice <= 1E-30f) || !((float)rectParams.topSlice <= 1E-30f) || !((float)rectParams.rightSlice <= 1E-30f) || !((float)rectParams.bottomSlice <= 1E-30f)) ? MeshBuilderNative.MakeVectorGraphics9SliceBackground(sliceLTRB: new Vector4(rectParams.leftSlice, rectParams.topSlice, rectParams.rightSlice, rectParams.bottomSlice), svgVertices: array, svgIndices: vi.indices, svgWidth: vi.size.x, svgHeight: vi.size.y, targetRect: rectParams.rect, tint: rectParams.color, colorPage: rectParams.colorPage) : MeshBuilderNative.MakeVectorGraphicsStretchBackground(array, vi.indices, vi.size.x, vi.size.y, rectParams.rect, rectParams.uv, rectParams.scaleMode, rectParams.color, rectParams.colorPage));
			NativeSlice<Vertex> slice = UIRenderDevice.PtrToSlice<Vertex>((void*)meshWriteDataInterface.vertices, meshWriteDataInterface.vertexCount);
			NativeSlice<ushort> slice2 = UIRenderDevice.PtrToSlice<ushort>((void*)meshWriteDataInterface.indices, meshWriteDataInterface.indexCount);
			if (slice.Length != 0 && slice2.Length != 0)
			{
				allocator.AllocateTempMesh(slice.Length, slice2.Length, out var vertices, out var indices);
				Debug.Assert(vertices.Length == slice.Length);
				Debug.Assert(indices.Length == slice2.Length);
				vertices.CopyFrom(slice);
				indices.CopyFrom(slice2);
				if (flag)
				{
					node.DrawGradientsInternal(vertices, indices, vi);
				}
				else
				{
					node.DrawMesh(vertices, indices);
				}
			}
		}
	}

	private const string k_MemoryLabelName = "Renderer.MeshGenerator";

	private static readonly MemoryLabel k_MemoryLabel = new MemoryLabel("UIElements", "Renderer.MeshGenerator");

	private static readonly ProfilerMarker k_MarkerDrawRectangle = new ProfilerMarker("MeshGenerator.DrawRectangle");

	private static readonly ProfilerMarker k_MarkerDrawBorder = new ProfilerMarker("MeshGenerator.DrawBorder");

	private static readonly ProfilerMarker k_MarkerDrawVectorImage = new ProfilerMarker("MeshGenerator.DrawVectorImage");

	private static readonly ProfilerMarker k_MarkerDrawRectangleRepeat = new ProfilerMarker("MeshGenerator.DrawRectangleRepeat");

	private MeshGenerationContext m_MeshGenerationContext;

	private List<RepeatRectUV>[] m_RepeatRectUVList = null;

	private NativePagedList<BackgroundRepeatInstance> m_BackgroundRepeatInstanceList = null;

	private GCHandlePool m_GCHandlePool = new GCHandlePool();

	private NativeArray<TessellationJobParameters> m_JobParameters;

	private TextInfo m_TextInfo = new TextInfo();

	private UnityEngine.TextCore.Text.TextGenerationSettings m_Settings = new UnityEngine.TextCore.Text.TextGenerationSettings
	{
		screenRect = Rect.zero,
		richText = true
	};

	private List<NativeSlice<Vertex>> m_VerticesArray = new List<NativeSlice<Vertex>>();

	private List<NativeSlice<ushort>> m_IndicesArray = new List<NativeSlice<ushort>>();

	private List<Texture2D> m_Atlases = new List<Texture2D>();

	private List<float> m_SdfScales = new List<float>();

	private List<GlyphRenderMode> m_RenderModes = new List<GlyphRenderMode>();

	private MeshGenerationCallback m_OnMeshGenerationDelegate;

	private List<TessellationJobParameters> m_TesselationJobParameters = new List<TessellationJobParameters>(256);

	public VisualElement currentElement { get; set; }

	public TextJobSystem textJobSystem { get; set; }

	internal bool disposed { get; private set; }

	public MeshGenerator(MeshGenerationContext mgc)
	{
		m_MeshGenerationContext = mgc;
		m_OnMeshGenerationDelegate = OnMeshGeneration;
		textJobSystem = new TextJobSystem();
	}

	private static Vector2 ConvertBorderRadiusPercentToPoints(Vector2 borderRectSize, Length length)
	{
		float a = length.value;
		float a2 = length.value;
		if (length.unit == LengthUnit.Percent)
		{
			a = borderRectSize.x * length.value / 100f;
			a2 = borderRectSize.y * length.value / 100f;
		}
		a = Mathf.Max(a, 0f);
		a2 = Mathf.Max(a2, 0f);
		return new Vector2(a, a2);
	}

	public static void GetVisualElementRadii(VisualElement ve, out Vector2 topLeft, out Vector2 bottomLeft, out Vector2 topRight, out Vector2 bottomRight)
	{
		IResolvedStyle resolvedStyle = ve.resolvedStyle;
		Vector2 borderRectSize = new Vector2(resolvedStyle.width, resolvedStyle.height);
		ComputedStyle computedStyle = ve.computedStyle;
		topLeft = ConvertBorderRadiusPercentToPoints(borderRectSize, computedStyle.borderTopLeftRadius);
		bottomLeft = ConvertBorderRadiusPercentToPoints(borderRectSize, computedStyle.borderBottomLeftRadius);
		topRight = ConvertBorderRadiusPercentToPoints(borderRectSize, computedStyle.borderTopRightRadius);
		bottomRight = ConvertBorderRadiusPercentToPoints(borderRectSize, computedStyle.borderBottomRightRadius);
	}

	public static void AdjustBackgroundSizeForBorders(VisualElement visualElement, ref RectangleParams rectParams)
	{
		IResolvedStyle resolvedStyle = visualElement.resolvedStyle;
		Vector4 zero = Vector4.zero;
		if (resolvedStyle.borderLeftWidth >= 1f && resolvedStyle.borderLeftColor.a >= 1f)
		{
			zero.x = 0.5f;
		}
		if (resolvedStyle.borderTopWidth >= 1f && resolvedStyle.borderTopColor.a >= 1f)
		{
			zero.y = 0.5f;
		}
		if (resolvedStyle.borderRightWidth >= 1f && resolvedStyle.borderRightColor.a >= 1f)
		{
			zero.z = 0.5f;
		}
		if (resolvedStyle.borderBottomWidth >= 1f && resolvedStyle.borderBottomColor.a >= 1f)
		{
			zero.w = 0.5f;
		}
		rectParams.rectInset = zero;
	}

	public void DrawText(string text, Vector2 pos, float fontSize, Color color, FontAsset font)
	{
		TextSettings textSettingsFrom = TextUtilities.GetTextSettingsFrom(currentElement);
		m_TextInfo.Clear();
		m_Settings.text = text;
		m_Settings.fontAsset = font;
		m_Settings.textSettings = textSettingsFrom;
		m_Settings.fontSize = (int)Mathf.Round(fontSize);
		m_Settings.color = color;
		m_Settings.textWrappingMode = TextWrappingMode.NoWrap;
		UnityEngine.TextCore.Text.TextGenerator.GetTextGenerator().GenerateText(m_Settings, m_TextInfo);
		DrawTextBase(m_TextInfo, default(NativeTextInfo), pos, isNative: false);
	}

	private void DrawTextBase(TextInfo textInfo, NativeTextInfo nativeTextInfo, Vector2 pos, bool isNative)
	{
		int i = 0;
		for (int num = (isNative ? nativeTextInfo.meshInfoCount : textInfo.meshInfo.Length); i < num; i++)
		{
			MeshInfo meshInfo = default(MeshInfo);
			FontAsset fontAsset = null;
			SpriteAsset spriteAsset = null;
			bool flag = false;
			Span<ATGMeshInfo> span = default(Span<ATGMeshInfo>);
			ATGMeshInfo aTGMeshInfo = default(ATGMeshInfo);
			int num2;
			if (!isNative)
			{
				meshInfo = textInfo.meshInfo[i];
				Debug.Assert((meshInfo.vertexCount & 3) == 0);
				num2 = meshInfo.vertexCount;
			}
			else
			{
				aTGMeshInfo = nativeTextInfo.meshInfos[i];
				int length = aTGMeshInfo.textElementInfos.Length;
				num2 = length * 4;
				UnityEngine.TextCore.Text.TextAsset textAssetByID = UnityEngine.TextCore.Text.TextAsset.GetTextAssetByID(aTGMeshInfo.textAssetId);
				if (textAssetByID == null)
				{
					continue;
				}
				if (textAssetByID is FontAsset)
				{
					fontAsset = textAssetByID as FontAsset;
				}
				else
				{
					flag = true;
					spriteAsset = textAssetByID as SpriteAsset;
				}
			}
			int b = (int)(UIRenderDevice.maxVerticesPerPage & -4);
			float inverseScale = 1f / currentElement.scaledPixelsPerPoint;
			while (num2 > 0)
			{
				int num3 = Mathf.Min(num2, b);
				int num4 = num3 >> 2;
				int indexCount = num4 * 6;
				Texture2D item;
				GlyphRenderMode item2;
				if (isNative)
				{
					if (flag)
					{
						item = (Texture2D)spriteAsset.material.mainTexture;
						item2 = GlyphRenderMode.COLOR;
					}
					else
					{
						item = (Texture2D)fontAsset.material.mainTexture;
						item2 = fontAsset.atlasRenderMode;
					}
				}
				else
				{
					item = (Texture2D)meshInfo.material.mainTexture;
					item2 = meshInfo.glyphRenderMode;
				}
				m_Atlases.Add(item);
				m_RenderModes.Add(item2);
				float item3 = 0f;
				List<GlyphRenderMode> renderModes = m_RenderModes;
				if (!TextGeneratorUtilities.IsBitmapRendering(renderModes[renderModes.Count - 1]))
				{
					List<Texture2D> atlases = m_Atlases;
					if (atlases[atlases.Count - 1].format == TextureFormat.Alpha8)
					{
						item3 = ((!isNative) ? meshInfo.material.GetFloat(TextShaderUtilities.ID_GradientScale) : ((float)((!flag) ? (fontAsset.atlasPadding + 1) : 0)));
					}
				}
				m_SdfScales.Add(item3);
				m_MeshGenerationContext.AllocateTempMesh(num3, indexCount, out var vertices, out var indices);
				int num5 = 0;
				int num6 = 0;
				int num7 = 0;
				while (num5 < num3)
				{
					if (isNative)
					{
						Span<NativeTextElementInfo> textElementInfos = aTGMeshInfo.textElementInfos;
						bool isColorGlyph = !flag && (fontAsset.atlasRenderMode == GlyphRenderMode.COLOR || fontAsset.atlasRenderMode == GlyphRenderMode.COLOR_HINTED);
						vertices[num5] = ConvertTextVertexToUIRVertex(ref textElementInfos[num6].bottomLeft, pos, inverseScale, isDynamicColor: false, isColorGlyph);
						vertices[num5 + 1] = ConvertTextVertexToUIRVertex(ref textElementInfos[num6].topLeft, pos, inverseScale, isDynamicColor: false, isColorGlyph);
						vertices[num5 + 2] = ConvertTextVertexToUIRVertex(ref textElementInfos[num6].topRight, pos, inverseScale, isDynamicColor: false, isColorGlyph);
						vertices[num5 + 3] = ConvertTextVertexToUIRVertex(ref textElementInfos[num6].bottomRight, pos, inverseScale, isDynamicColor: false, isColorGlyph);
					}
					else
					{
						vertices[num5] = ConvertTextVertexToUIRVertex(ref meshInfo.vertexData[num5], pos, inverseScale);
						vertices[num5 + 1] = ConvertTextVertexToUIRVertex(ref meshInfo.vertexData[num5 + 1], pos, inverseScale);
						vertices[num5 + 2] = ConvertTextVertexToUIRVertex(ref meshInfo.vertexData[num5 + 2], pos, inverseScale);
						vertices[num5 + 3] = ConvertTextVertexToUIRVertex(ref meshInfo.vertexData[num5 + 3], pos, inverseScale);
					}
					indices[num7] = (ushort)num5;
					indices[num7 + 1] = (ushort)(num5 + 1);
					indices[num7 + 2] = (ushort)(num5 + 2);
					indices[num7 + 3] = (ushort)(num5 + 2);
					indices[num7 + 4] = (ushort)(num5 + 3);
					indices[num7 + 5] = (ushort)num5;
					num5 += 4;
					num6++;
					num7 += 6;
				}
				m_VerticesArray.Add(vertices);
				m_IndicesArray.Add(indices);
				num2 -= num3;
			}
			Debug.Assert(num2 == 0);
		}
		DrawText(m_VerticesArray, m_IndicesArray, m_Atlases, m_RenderModes, m_SdfScales);
		m_VerticesArray.Clear();
		m_IndicesArray.Clear();
		m_Atlases.Clear();
		m_SdfScales.Clear();
		m_RenderModes.Clear();
	}

	public void DrawText(List<NativeSlice<Vertex>> vertices, List<NativeSlice<ushort>> indices, List<Material> materials, List<GlyphRenderMode> renderModes)
	{
		for (int i = 0; i < materials.Count; i++)
		{
			Material material = materials[i];
			m_Atlases.Add(material.mainTexture as Texture2D);
			float item = 0f;
			if (!TextGeneratorUtilities.IsBitmapRendering(renderModes[i]))
			{
				List<Texture2D> atlases = m_Atlases;
				if (atlases[atlases.Count - 1].format == TextureFormat.Alpha8)
				{
					item = material.GetFloat(TextShaderUtilities.ID_GradientScale);
				}
			}
			m_SdfScales.Add(item);
		}
		DrawText(vertices, indices, m_Atlases, renderModes, m_SdfScales);
		m_Atlases.Clear();
		m_SdfScales.Clear();
	}

	public void DrawText(List<NativeSlice<Vertex>> vertices, List<NativeSlice<ushort>> indices, List<Texture2D> atlases, List<GlyphRenderMode> renderModes, List<float> sdfScales)
	{
		if (vertices == null)
		{
			return;
		}
		int i = 0;
		for (int count = vertices.Count; i < count; i++)
		{
			if (vertices[i].Length != 0)
			{
				if (atlases[i].format != TextureFormat.Alpha8)
				{
					MakeText(atlases[i], vertices[i], indices[i], isSdf: false, 0f, 0f, multiChannel: true);
					continue;
				}
				float sharpness = 0f;
				MakeText(atlases[i], vertices[i], indices[i], isSdf: true, sdfScales[i], sharpness, multiChannel: false);
			}
		}
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	internal static Vertex ConvertTextVertexToUIRVertex(ref TextCoreVertex vertex, Vector2 posOffset, float inverseScale, bool isDynamicColor = false, bool isColorGlyph = false)
	{
		float num = 0f;
		if (vertex.uv2.y < 0f)
		{
			num = 1f;
		}
		return new Vertex
		{
			position = new Vector3(vertex.position.x * inverseScale + posOffset.x, vertex.position.y * inverseScale + posOffset.y),
			uv = new Vector2(vertex.uv0.x, vertex.uv0.y),
			tint = (isColorGlyph ? new Color32(byte.MaxValue, byte.MaxValue, byte.MaxValue, vertex.color.a) : vertex.color),
			flags = new Color32(0, (byte)(num * 255f), 0, (byte)(isDynamicColor ? 2 : 0))
		};
	}

	private void MakeText(Texture texture, NativeSlice<Vertex> vertices, NativeSlice<ushort> indices, bool isSdf, float sdfScale, float sharpness, bool multiChannel)
	{
		if (isSdf)
		{
			m_MeshGenerationContext.entryRecorder.DrawSdfText(m_MeshGenerationContext.parentEntry, vertices, indices, texture, sdfScale, sharpness);
		}
		else
		{
			m_MeshGenerationContext.entryRecorder.DrawRasterText(m_MeshGenerationContext.parentEntry, vertices, indices, texture, multiChannel);
		}
	}

	public void DrawRectangle(RectangleParams rectParams)
	{
		if (!(rectParams.rect.width < 1E-30f) && !(rectParams.rect.height < 1E-30f))
		{
			TessellationJobParameters item = new TessellationJobParameters
			{
				isBorderJob = false
			};
			rectParams.ToNativeParams(out item.rectParams);
			item.rectParams.texture = m_GCHandlePool.GetIntPtr(rectParams.texture);
			item.rectParams.sprite = m_GCHandlePool.GetIntPtr(rectParams.sprite);
			if (rectParams.sprite != null && rectParams.sprite.texture != null)
			{
				item.rectParams.spriteTexture = m_GCHandlePool.GetIntPtr(rectParams.sprite.texture);
				item.rectParams.spriteVertices = m_GCHandlePool.GetIntPtr(rectParams.sprite.vertices);
				item.rectParams.spriteUVs = m_GCHandlePool.GetIntPtr(rectParams.sprite.uv);
				item.rectParams.spriteTriangles = m_GCHandlePool.GetIntPtr(rectParams.sprite.triangles);
			}
			if (rectParams.backgroundRepeatInstanceList != null)
			{
				item.rectParams.backgroundRepeatInstanceListStartIndex = rectParams.backgroundRepeatInstanceListStartIndex;
				item.rectParams.backgroundRepeatInstanceListEndIndex = rectParams.backgroundRepeatInstanceListEndIndex;
				item.rectParams.backgroundRepeatInstanceList = m_GCHandlePool.GetIntPtr(rectParams.backgroundRepeatInstanceList);
			}
			item.rectParams.vectorImage = m_GCHandlePool.GetIntPtr(rectParams.vectorImage);
			bool flag = rectParams.vectorImage?.atlas != null;
			item.rectParams.meshFlags |= (flag ? 4 : 0);
			m_MeshGenerationContext.InsertUnsafeMeshGenerationNode(out var node);
			item.node = node;
			m_TesselationJobParameters.Add(item);
		}
	}

	public void DrawBorder(BorderParams borderParams)
	{
		TessellationJobParameters item = new TessellationJobParameters
		{
			isBorderJob = true,
			borderParams = borderParams
		};
		m_MeshGenerationContext.InsertUnsafeMeshGenerationNode(out var node);
		item.node = node;
		m_TesselationJobParameters.Add(item);
	}

	public void DrawVectorImage(VectorImage vectorImage, Vector2 offset, Angle rotationAngle, Vector2 scale)
	{
		if (vectorImage == null || vectorImage.vertices.Length == 0 || vectorImage.indices.Length == 0)
		{
			return;
		}
		m_MeshGenerationContext.AllocateTempMesh(vectorImage.vertices.Length, vectorImage.indices.Length, out var vertices, out var indices);
		if (vectorImage.atlas != null)
		{
			m_MeshGenerationContext.entryRecorder.DrawGradients(m_MeshGenerationContext.parentEntry, vertices, indices, vectorImage);
		}
		else
		{
			m_MeshGenerationContext.entryRecorder.DrawMesh(m_MeshGenerationContext.parentEntry, vertices, indices);
		}
		Matrix4x4 matrix4x = Matrix4x4.TRS((Vector3)offset, Quaternion.AngleAxis(rotationAngle.ToDegrees(), Vector3.forward), new Vector3(scale.x, scale.y, 1f));
		bool flag = (scale.x < 0f) ^ (scale.y < 0f);
		int num = vectorImage.vertices.Length;
		for (int i = 0; i < num; i++)
		{
			VectorImageVertex vectorImageVertex = vectorImage.vertices[i];
			Vector3 position = matrix4x.MultiplyPoint3x4(vectorImageVertex.position);
			position.z = Vertex.nearZ;
			Color32 settingIndex = new Color32((byte)(vectorImageVertex.settingIndex >> 8), (byte)vectorImageVertex.settingIndex, 0, 0);
			vertices[i] = new Vertex
			{
				position = position,
				tint = vectorImageVertex.tint,
				uv = vectorImageVertex.uv,
				settingIndex = settingIndex,
				flags = vectorImageVertex.flags,
				circle = vectorImageVertex.circle
			};
		}
		if (!flag)
		{
			indices.CopyFrom(vectorImage.indices);
			return;
		}
		ushort[] indices2 = vectorImage.indices;
		for (int j = 0; j < indices2.Length; j += 3)
		{
			indices[j] = indices2[j];
			indices[j + 1] = indices2[j + 2];
			indices[j + 2] = indices2[j + 1];
		}
	}

	public void DrawRectangleRepeat(RectangleParams rectParams, Rect totalRect, float scaledPixelsPerPoint)
	{
		DoDrawRectangleRepeat(ref rectParams, totalRect, scaledPixelsPerPoint);
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	private void DoDrawRectangleRepeat(ref RectangleParams rectParams, Rect totalRect, float scaledPixelsPerPoint)
	{
		Rect rect = new Rect(0f, 0f, 1f, 1f);
		if (m_RepeatRectUVList == null)
		{
			m_RepeatRectUVList = new List<RepeatRectUV>[2];
			m_RepeatRectUVList[0] = new List<RepeatRectUV>();
			m_RepeatRectUVList[1] = new List<RepeatRectUV>();
		}
		else
		{
			m_RepeatRectUVList[0].Clear();
			m_RepeatRectUVList[1].Clear();
		}
		Rect rect2 = rectParams.rect;
		if (rectParams.backgroundSize.sizeType != BackgroundSizeType.Length)
		{
			if (rectParams.backgroundSize.sizeType == BackgroundSizeType.Contain)
			{
				float num = totalRect.width / rect2.width;
				float num2 = totalRect.height / rect2.height;
				Rect rect3 = rect2;
				if (num < num2)
				{
					rect3.width = totalRect.width;
					rect3.height = rect2.height * totalRect.width / rect2.width;
				}
				else
				{
					rect3.width = rect2.width * totalRect.height / rect2.height;
					rect3.height = totalRect.height;
				}
				rect2 = rect3;
			}
			else if (rectParams.backgroundSize.sizeType == BackgroundSizeType.Cover)
			{
				float num3 = totalRect.width / rect2.width;
				float num4 = totalRect.height / rect2.height;
				Rect rect4 = rect2;
				if (num3 > num4)
				{
					rect4.width = totalRect.width;
					rect4.height = rect2.height * totalRect.width / rect2.width;
				}
				else
				{
					rect4.width = rect2.width * totalRect.height / rect2.height;
					rect4.height = totalRect.height;
				}
				rect2 = rect4;
			}
		}
		else if (!rectParams.backgroundSize.x.IsNone() || !rectParams.backgroundSize.y.IsNone())
		{
			if (!rectParams.backgroundSize.x.IsNone() && rectParams.backgroundSize.y.IsAuto())
			{
				Rect rect5 = rect2;
				if (rectParams.backgroundSize.x.unit == LengthUnit.Percent)
				{
					rect5.width = totalRect.width * rectParams.backgroundSize.x.value / 100f;
					rect5.height = rect5.width * rect2.height / rect2.width;
				}
				else if (rectParams.backgroundSize.x.unit == LengthUnit.Pixel)
				{
					rect5.width = rectParams.backgroundSize.x.value;
					rect5.height = rect5.width * rect2.height / rect2.width;
				}
				rect2 = rect5;
			}
			else if (!rectParams.backgroundSize.x.IsNone() && !rectParams.backgroundSize.y.IsNone())
			{
				Rect rect6 = rect2;
				if (!rectParams.backgroundSize.x.IsAuto())
				{
					if (rectParams.backgroundSize.x.unit == LengthUnit.Percent)
					{
						rect6.width = totalRect.width * rectParams.backgroundSize.x.value / 100f;
					}
					else if (rectParams.backgroundSize.x.unit == LengthUnit.Pixel)
					{
						rect6.width = rectParams.backgroundSize.x.value;
					}
				}
				if (!rectParams.backgroundSize.y.IsAuto())
				{
					if (rectParams.backgroundSize.y.unit == LengthUnit.Percent)
					{
						rect6.height = totalRect.height * rectParams.backgroundSize.y.value / 100f;
					}
					else if (rectParams.backgroundSize.y.unit == LengthUnit.Pixel)
					{
						rect6.height = rectParams.backgroundSize.y.value;
					}
					if (rectParams.backgroundSize.x.IsAuto())
					{
						rect6.width = rect6.height * rect2.width / rect2.height;
					}
				}
				rect2 = rect6;
			}
		}
		if (rect2.size.x <= 1E-30f || rect2.size.y <= 1E-30f || totalRect.size.x <= 1E-30f || totalRect.size.y <= 1E-30f)
		{
			return;
		}
		if (rectParams.backgroundSize.x.IsAuto() && rectParams.backgroundRepeat.y == Repeat.Round)
		{
			float num5 = 1f / rect2.height;
			int val = (int)(totalRect.height * num5 + 0.5f);
			val = Math.Max(val, 1);
			Rect rect7 = default(Rect);
			rect7.height = totalRect.height / (float)val;
			rect7.width = rect7.height * rect2.width * num5;
			rect2 = rect7;
		}
		else if (rectParams.backgroundSize.y.IsAuto() && rectParams.backgroundRepeat.x == Repeat.Round)
		{
			float num6 = 1f / rect2.width;
			int val2 = (int)(totalRect.width * num6 + 0.5f);
			val2 = Math.Max(val2, 1);
			Rect rect8 = default(Rect);
			rect8.width = totalRect.width / (float)val2;
			rect8.height = rect8.width * rect2.height * num6;
			rect2 = rect8;
		}
		RepeatRectUV item2 = default(RepeatRectUV);
		RepeatRectUV item6 = default(RepeatRectUV);
		RepeatRectUV item3 = default(RepeatRectUV);
		RepeatRectUV item4 = default(RepeatRectUV);
		RepeatRectUV item5 = default(RepeatRectUV);
		RepeatRectUV item = default(RepeatRectUV);
		for (int i = 0; i < 2; i++)
		{
			Repeat repeat = ((i == 0) ? rectParams.backgroundRepeat.x : rectParams.backgroundRepeat.y);
			BackgroundPosition backgroundPosition = ((i == 0) ? rectParams.backgroundPositionX : rectParams.backgroundPositionY);
			float num7 = 0f;
			switch (repeat)
			{
			case Repeat.NoRepeat:
			{
				Rect rect10 = rect2;
				item2.uv = rect;
				item2.rect = rect10;
				num7 = rect10.size[i];
				m_RepeatRectUVList[i].Add(item2);
				break;
			}
			case Repeat.Repeat:
			{
				Rect rect12 = rect2;
				int num11 = (int)((totalRect.size[i] + 1f / scaledPixelsPerPoint) / rect2.size[i]);
				num11 = ((backgroundPosition.keyword != BackgroundPositionKeyword.Center) ? (num11 + 2) : (((num11 & 1) != 1) ? (num11 + 1) : (num11 + 2)));
				for (int l = 0; l < num11; l++)
				{
					Vector2 position4 = rect12.position;
					position4[i] = (float)l * rect2.size[i];
					rect12.position = position4;
					item6.rect = rect12;
					item6.uv = rect;
					num7 += item6.rect.size[i];
					m_RepeatRectUVList[i].Add(item6);
				}
				break;
			}
			case Repeat.Space:
			{
				Rect rect11 = rect2;
				int num9 = (int)(totalRect.size[i] / rect2.size[i]);
				if (num9 >= 0)
				{
					item3.rect = rect11;
					item3.uv = rect;
					m_RepeatRectUVList[i].Add(item3);
					num7 = rect2.size[i];
				}
				if (num9 >= 2)
				{
					Vector2 position2 = rect11.position;
					position2[i] = totalRect.size[i] - rect2.size[i];
					rect11.position = position2;
					item4.rect = rect11;
					item4.uv = rect;
					m_RepeatRectUVList[i].Add(item4);
					num7 = totalRect.size[i];
				}
				if (num9 > 2)
				{
					float num10 = (totalRect.size[i] - rect2.size[i] * (float)num9) / (float)(num9 - 1);
					for (int k = 0; k < num9 - 2; k++)
					{
						Vector2 position3 = rect11.position;
						position3[i] = (rect2.size[i] + num10) * (float)(1 + k);
						rect11.position = position3;
						item5.rect = rect11;
						item5.uv = rect;
						m_RepeatRectUVList[i].Add(item5);
					}
				}
				break;
			}
			case Repeat.Round:
			{
				int val3 = (int)((totalRect.size[i] + rect2.size[i] * 0.5f) / rect2.size[i]);
				val3 = Math.Max(val3, 1);
				float num8 = totalRect.size[i] / (float)val3;
				val3 = ((backgroundPosition.keyword != BackgroundPositionKeyword.Center) ? (val3 + 1) : (((val3 & 1) != 1) ? (val3 + 1) : (val3 + 2)));
				Rect rect9 = rect2;
				Vector2 size = rect9.size;
				size[i] = num8;
				rect9.size = size;
				rect2 = rect9;
				for (int j = 0; j < val3; j++)
				{
					Vector2 position = rect9.position;
					position[i] = num8 * (float)j;
					rect9.position = position;
					item.rect = rect9;
					item.uv = rect;
					m_RepeatRectUVList[i].Add(item);
					num7 += item.rect.size[i];
				}
				break;
			}
			}
			float num12 = 0f;
			bool flag = false;
			if (backgroundPosition.keyword == BackgroundPositionKeyword.Center)
			{
				num12 = (totalRect.size[i] - num7) * 0.5f;
				flag = true;
			}
			else if (repeat != Repeat.Space)
			{
				if (backgroundPosition.offset.unit == LengthUnit.Percent)
				{
					num12 = (totalRect.size[i] - rect2.size[i]) * backgroundPosition.offset.value / 100f;
					flag = true;
				}
				else if (backgroundPosition.offset.unit == LengthUnit.Pixel)
				{
					num12 = backgroundPosition.offset.value;
				}
				if (backgroundPosition.keyword == BackgroundPositionKeyword.Right || backgroundPosition.keyword == BackgroundPositionKeyword.Bottom)
				{
					num12 = totalRect.size[i] - num7 - num12;
				}
			}
			if (flag && rectParams.sprite == null && rectParams.vectorImage == null)
			{
				float num13 = rect2.size[i] * scaledPixelsPerPoint;
				if (Mathf.Abs(Mathf.Round(num13) - num13) < 0.001f)
				{
					num12 = AlignmentUtils.CeilToPixelGrid(num12, scaledPixelsPerPoint);
				}
			}
			if (repeat == Repeat.Repeat || repeat == Repeat.Round)
			{
				float num14 = rect2.size[i];
				if (num14 > 1E-30f)
				{
					if (num12 < 0f - num14)
					{
						int num15 = (int)((0f - num12) / num14);
						num12 += (float)num15 * num14;
					}
					if (num12 > 0f)
					{
						int num16 = (int)(num12 / num14);
						num12 -= (float)(1 + num16) * num14;
					}
				}
			}
			for (int m = 0; m < m_RepeatRectUVList[i].Count; m++)
			{
				RepeatRectUV value = m_RepeatRectUVList[i][m];
				Vector2 position5 = value.rect.position;
				position5[i] += num12;
				value.rect.position = position5;
				m_RepeatRectUVList[i][m] = value;
			}
		}
		Rect rect13 = new Rect(rect);
		int num17 = m_RepeatRectUVList[1].Count * m_RepeatRectUVList[0].Count;
		if (num17 > 1 && rectParams.vectorImage == null)
		{
			if (m_BackgroundRepeatInstanceList == null)
			{
				m_BackgroundRepeatInstanceList = new NativePagedList<BackgroundRepeatInstance>(8, "Renderer.MeshGenerator", Allocator.Persistent, Allocator.TempJob);
			}
			rectParams.backgroundRepeatInstanceList = m_BackgroundRepeatInstanceList;
			rectParams.backgroundRepeatInstanceListStartIndex = m_BackgroundRepeatInstanceList.GetCount();
		}
		int num18 = 0;
		foreach (RepeatRectUV item7 in m_RepeatRectUVList[1])
		{
			rect2.y = item7.rect.y;
			rect2.height = item7.rect.height;
			rect.y = item7.uv.y;
			rect.height = item7.uv.height;
			if (rect2.y < totalRect.y)
			{
				float num19 = totalRect.y - rect2.y;
				float num20 = rect2.height - num19;
				float num21 = num19 + num20;
				float height = rect13.height * num20 / num21;
				float num22 = rect13.height * num19 / num21;
				rect.y = num22 + rect13.y;
				rect.height = height;
				rect2.y = totalRect.y;
				rect2.height = num20;
			}
			if (rect2.yMax > totalRect.yMax)
			{
				float num23 = rect2.yMax - totalRect.yMax;
				float num24 = rect2.height - num23;
				float num25 = num24 + num23;
				float num26 = (rect.height = rect.height * num24 / num25);
				rect.y = rect.yMax - num26;
				rect2.height = num24;
			}
			if (rectParams.vectorImage == null)
			{
				float num28 = rect.y - rect13.y;
				float num29 = rect13.yMax - rect.yMax;
				rect.y += num29 - num28;
			}
			foreach (RepeatRectUV item8 in m_RepeatRectUVList[0])
			{
				rect2.x = item8.rect.x;
				rect2.width = item8.rect.width;
				rect.x = item8.uv.x;
				rect.width = item8.uv.width;
				if (rect2.x < totalRect.x)
				{
					float num30 = totalRect.x - rect2.x;
					float num31 = rect2.width - num30;
					float num32 = num30 + num31;
					float width = rect.width * num31 / num32;
					float x = rect13.x + rect13.width * num30 / num32;
					rect.x = x;
					rect.width = width;
					rect2.x = totalRect.x;
					rect2.width = num31;
				}
				if (rect2.xMax > totalRect.xMax)
				{
					float num33 = rect2.xMax - totalRect.xMax;
					float num34 = rect2.width - num33;
					float num35 = num34 + num33;
					float width2 = rect.width * num34 / num35;
					rect.width = width2;
					rect2.width = num34;
				}
				StampRectangleWithSubRect(rectParams, rect2, totalRect, rect, ref rectParams.backgroundRepeatInstanceList);
				num18++;
				if (rectParams.backgroundRepeatInstanceList != null && num18 > 60)
				{
					num18 = 0;
					rectParams.backgroundRepeatInstanceListEndIndex = m_BackgroundRepeatInstanceList.GetCount();
					DrawRectangle(rectParams);
					rectParams.backgroundRepeatInstanceListStartIndex = rectParams.backgroundRepeatInstanceListEndIndex;
				}
			}
		}
		if (rectParams.backgroundRepeatInstanceList != null && num18 > 0)
		{
			rectParams.backgroundRepeatInstanceListEndIndex = m_BackgroundRepeatInstanceList.GetCount();
			DrawRectangle(rectParams);
		}
	}

	private void StampRectangleWithSubRect(RectangleParams rectParams, Rect targetRect, Rect totalRect, Rect targetUV, ref NativePagedList<BackgroundRepeatInstance> backgroundRepeatInstanceList)
	{
		if (targetRect.width < 0.001f || targetRect.height < 0.001f)
		{
			return;
		}
		Rect rect = targetRect;
		rect.size /= targetUV.size;
		rect.position -= new Vector2(targetUV.position.x, 1f - targetUV.position.y - targetUV.size.y) * rect.size;
		Rect subRect = rectParams.subRect;
		subRect.position *= rect.size;
		subRect.position += rect.position;
		subRect.size *= rect.size;
		if (rectParams.HasSlices(0.001f))
		{
			rectParams.backgroundRepeatRect = Rect.zero;
			rectParams.rect = targetRect;
		}
		else
		{
			Rect rect2 = RectangleParams.RectIntersection(subRect, targetRect);
			if (rect2.size.x < 0.001f || rect2.size.y < 0.001f)
			{
				return;
			}
			if (rect2.size != subRect.size)
			{
				Vector2 vector = rect2.size / subRect.size;
				Vector2 vector2 = rectParams.uv.size * vector;
				Vector2 vector3 = rectParams.uv.size - vector2;
				if (rect2.x > subRect.x)
				{
					float num = (subRect.xMax - rect2.xMax) / subRect.width * rectParams.uv.size.x;
					rectParams.uv.x += vector3.x - num;
				}
				if (rect2.yMax < subRect.yMax)
				{
					float num2 = (rect2.y - subRect.y) / subRect.height * rectParams.uv.size.y;
					rectParams.uv.y += vector3.y - num2;
				}
				rectParams.uv.size = vector2;
			}
			if (rectParams.vectorImage != null)
			{
				rectParams.backgroundRepeatRect = Rect.zero;
				rectParams.rect = rect2;
			}
			else
			{
				if (totalRect == rect2)
				{
					rectParams.backgroundRepeatRect = Rect.zero;
				}
				else
				{
					rectParams.backgroundRepeatRect = rect2;
				}
				rectParams.rect = totalRect;
			}
		}
		if (rectParams.vectorImage == null && backgroundRepeatInstanceList != null)
		{
			BackgroundRepeatInstance data = default(BackgroundRepeatInstance);
			data.rect = rectParams.rect;
			data.backgroundRepeatRect = rectParams.backgroundRepeatRect;
			data.uv = rectParams.uv;
			backgroundRepeatInstanceList.Add(data);
		}
		else
		{
			DrawRectangle(rectParams);
		}
	}

	private static void AdjustSpriteWinding(Vector2[] vertices, ushort[] indices, NativeSlice<ushort> newIndices)
	{
		for (int i = 0; i < indices.Length; i += 3)
		{
			Vector3 vector = vertices[indices[i]];
			Vector3 vector2 = vertices[indices[i + 1]];
			Vector3 vector3 = vertices[indices[i + 2]];
			Vector3 normalized = (vector2 - vector).normalized;
			Vector3 normalized2 = (vector3 - vector).normalized;
			if (Vector3.Cross(normalized, normalized2).z >= 0f)
			{
				newIndices[i] = indices[i + 1];
				newIndices[i + 1] = indices[i];
				newIndices[i + 2] = indices[i + 2];
			}
			else
			{
				newIndices[i] = indices[i];
				newIndices[i + 1] = indices[i + 1];
				newIndices[i + 2] = indices[i + 2];
			}
		}
	}

	public void ScheduleJobs(MeshGenerationContext mgc)
	{
		int count = m_TesselationJobParameters.Count;
		if (count != 0)
		{
			if (m_JobParameters.Length < count)
			{
				m_JobParameters.Dispose();
				m_JobParameters = new NativeArray<TessellationJobParameters>(count, k_MemoryLabel, NativeArrayOptions.UninitializedMemory);
			}
			for (int i = 0; i < count; i++)
			{
				m_JobParameters[i] = m_TesselationJobParameters[i];
			}
			m_TesselationJobParameters.Clear();
			TessellationJob jobData = new TessellationJob
			{
				jobParameters = m_JobParameters.Slice(0, count)
			};
			mgc.GetTempMeshAllocator(out jobData.allocator);
			JobHandle jobHandle = jobData.Schedule(count, 1);
			mgc.AddMeshGenerationJob(jobHandle);
			mgc.AddMeshGenerationCallback(m_OnMeshGenerationDelegate, null, MeshGenerationCallbackType.Work, isJobDependent: true);
		}
	}

	private void OnMeshGeneration(MeshGenerationContext ctx, object data)
	{
		if (m_BackgroundRepeatInstanceList != null)
		{
			m_BackgroundRepeatInstanceList.Reset();
		}
		m_GCHandlePool.ReturnAll();
	}

	public void Dispose()
	{
		Dispose(disposing: true);
		GC.SuppressFinalize(this);
	}

	private void Dispose(bool disposing)
	{
		if (disposed)
		{
			return;
		}
		if (disposing)
		{
			if (m_BackgroundRepeatInstanceList != null)
			{
				m_BackgroundRepeatInstanceList.Dispose();
			}
			m_GCHandlePool.Dispose();
			m_JobParameters.Dispose();
		}
		disposed = true;
	}
}
