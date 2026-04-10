using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using UnityEngine.Bindings;
using UnityEngine.Scripting;

namespace UnityEngine;

[NativeHeader("Runtime/2D/Common/SpriteDataAccess.h")]
[NativeType("Runtime/Graphics/SpriteFrame.h")]
[NativeHeader("Runtime/2D/Common/ScriptBindings/SpritesMarshalling.h")]
[ExcludeFromPreset]
[NativeHeader("Runtime/Graphics/SpriteUtility.h")]
public sealed class Sprite : Object
{
	public Bounds bounds
	{
		get
		{
			IntPtr intPtr = MarshalledUnityObject.MarshalNotNull(this);
			if (intPtr == (IntPtr)0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			get_bounds_Injected(intPtr, out var ret);
			return ret;
		}
	}

	public Rect rect
	{
		get
		{
			IntPtr intPtr = MarshalledUnityObject.MarshalNotNull(this);
			if (intPtr == (IntPtr)0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			get_rect_Injected(intPtr, out var ret);
			return ret;
		}
	}

	public Vector4 border
	{
		get
		{
			IntPtr intPtr = MarshalledUnityObject.MarshalNotNull(this);
			if (intPtr == (IntPtr)0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			get_border_Injected(intPtr, out var ret);
			return ret;
		}
	}

	public Texture2D texture
	{
		get
		{
			IntPtr intPtr = MarshalledUnityObject.MarshalNotNull(this);
			if (intPtr == (IntPtr)0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			return Unmarshal.UnmarshalUnityObject<Texture2D>(get_texture_Injected(intPtr));
		}
	}

	internal uint extrude
	{
		get
		{
			IntPtr intPtr = MarshalledUnityObject.MarshalNotNull(this);
			if (intPtr == (IntPtr)0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			return get_extrude_Injected(intPtr);
		}
	}

	public float pixelsPerUnit
	{
		[NativeMethod("GetPixelsToUnits")]
		get
		{
			IntPtr intPtr = MarshalledUnityObject.MarshalNotNull(this);
			if (intPtr == (IntPtr)0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			return get_pixelsPerUnit_Injected(intPtr);
		}
	}

	public float spriteAtlasTextureScale
	{
		[NativeMethod("GetSpriteAtlasTextureScale")]
		get
		{
			IntPtr intPtr = MarshalledUnityObject.MarshalNotNull(this);
			if (intPtr == (IntPtr)0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			return get_spriteAtlasTextureScale_Injected(intPtr);
		}
	}

	public Texture2D associatedAlphaSplitTexture
	{
		[NativeMethod("GetAlphaTexture")]
		get
		{
			IntPtr intPtr = MarshalledUnityObject.MarshalNotNull(this);
			if (intPtr == (IntPtr)0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			return Unmarshal.UnmarshalUnityObject<Texture2D>(get_associatedAlphaSplitTexture_Injected(intPtr));
		}
	}

	public Vector2 pivot
	{
		[NativeMethod("GetPivotInPixels")]
		get
		{
			IntPtr intPtr = MarshalledUnityObject.MarshalNotNull(this);
			if (intPtr == (IntPtr)0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			get_pivot_Injected(intPtr, out var ret);
			return ret;
		}
	}

	public bool packed => GetPacked() == 1;

	public SpritePackingMode packingMode => (SpritePackingMode)GetPackingMode();

	public SpritePackingRotation packingRotation => (SpritePackingRotation)GetPackingRotation();

	public Rect textureRect => GetTextureRect();

	public Vector2 textureRectOffset => GetTextureRectOffset();

	public Vector2[] vertices
	{
		[FreeFunction("SpriteAccessLegacy::GetSpriteVertices", HasExplicitThis = true)]
		[return: UnityMarshalAs(NativeType.ScriptingObjectPtr)]
		get
		{
			IntPtr intPtr = MarshalledUnityObject.MarshalNotNull(this);
			if (intPtr == (IntPtr)0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			return get_vertices_Injected(intPtr);
		}
	}

	public ushort[] triangles
	{
		[FreeFunction("SpriteAccessLegacy::GetSpriteIndices", HasExplicitThis = true)]
		[return: UnityMarshalAs(NativeType.ScriptingObjectPtr)]
		get
		{
			IntPtr intPtr = MarshalledUnityObject.MarshalNotNull(this);
			if (intPtr == (IntPtr)0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			return get_triangles_Injected(intPtr);
		}
	}

	public Vector2[] uv
	{
		[FreeFunction("SpriteAccessLegacy::GetSpriteUVs", HasExplicitThis = true)]
		[return: UnityMarshalAs(NativeType.ScriptingObjectPtr)]
		get
		{
			IntPtr intPtr = MarshalledUnityObject.MarshalNotNull(this);
			if (intPtr == (IntPtr)0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			return get_uv_Injected(intPtr);
		}
	}

	[RequiredByNativeCode]
	private Sprite()
	{
	}

	internal int GetPackingMode()
	{
		IntPtr intPtr = MarshalledUnityObject.MarshalNotNull(this);
		if (intPtr == (IntPtr)0)
		{
			ThrowHelper.ThrowNullReferenceException(this);
		}
		return GetPackingMode_Injected(intPtr);
	}

	internal int GetPackingRotation()
	{
		IntPtr intPtr = MarshalledUnityObject.MarshalNotNull(this);
		if (intPtr == (IntPtr)0)
		{
			ThrowHelper.ThrowNullReferenceException(this);
		}
		return GetPackingRotation_Injected(intPtr);
	}

	internal int GetPacked()
	{
		IntPtr intPtr = MarshalledUnityObject.MarshalNotNull(this);
		if (intPtr == (IntPtr)0)
		{
			ThrowHelper.ThrowNullReferenceException(this);
		}
		return GetPacked_Injected(intPtr);
	}

	internal Rect GetTextureRect()
	{
		IntPtr intPtr = MarshalledUnityObject.MarshalNotNull(this);
		if (intPtr == (IntPtr)0)
		{
			ThrowHelper.ThrowNullReferenceException(this);
		}
		GetTextureRect_Injected(intPtr, out var ret);
		return ret;
	}

	internal Vector2 GetTextureRectOffset()
	{
		IntPtr intPtr = MarshalledUnityObject.MarshalNotNull(this);
		if (intPtr == (IntPtr)0)
		{
			ThrowHelper.ThrowNullReferenceException(this);
		}
		GetTextureRectOffset_Injected(intPtr, out var ret);
		return ret;
	}

	internal Vector4 GetInnerUVs()
	{
		IntPtr intPtr = MarshalledUnityObject.MarshalNotNull(this);
		if (intPtr == (IntPtr)0)
		{
			ThrowHelper.ThrowNullReferenceException(this);
		}
		GetInnerUVs_Injected(intPtr, out var ret);
		return ret;
	}

	internal Vector4 GetOuterUVs()
	{
		IntPtr intPtr = MarshalledUnityObject.MarshalNotNull(this);
		if (intPtr == (IntPtr)0)
		{
			ThrowHelper.ThrowNullReferenceException(this);
		}
		GetOuterUVs_Injected(intPtr, out var ret);
		return ret;
	}

	internal Vector4 GetPadding()
	{
		IntPtr intPtr = MarshalledUnityObject.MarshalNotNull(this);
		if (intPtr == (IntPtr)0)
		{
			ThrowHelper.ThrowNullReferenceException(this);
		}
		GetPadding_Injected(intPtr, out var ret);
		return ret;
	}

	[FreeFunction("SpritesBindings::CreateSpriteWithoutTextureScripting")]
	internal static Sprite CreateSpriteWithoutTextureScripting(Rect rect, Vector2 pivot, float pixelsToUnits, Texture2D texture)
	{
		return Unmarshal.UnmarshalUnityObject<Sprite>(CreateSpriteWithoutTextureScripting_Injected(ref rect, ref pivot, pixelsToUnits, MarshalledUnityObject.Marshal(texture)));
	}

	[FreeFunction("SpritesBindings::CreateSprite", ThrowsException = true)]
	internal static Sprite CreateSprite(Texture2D texture, Rect rect, Vector2 pivot, float pixelsPerUnit, uint extrude, SpriteMeshType meshType, Vector4 border, bool generateFallbackPhysicsShape, [UnityMarshalAs(NativeType.ScriptingObjectPtr)] SecondarySpriteTexture[] secondaryTexture)
	{
		return Unmarshal.UnmarshalUnityObject<Sprite>(CreateSprite_Injected(MarshalledUnityObject.Marshal(texture), ref rect, ref pivot, pixelsPerUnit, extrude, meshType, ref border, generateFallbackPhysicsShape, secondaryTexture));
	}

	internal Texture2D GetSecondaryTexture(int index)
	{
		IntPtr intPtr = MarshalledUnityObject.MarshalNotNull(this);
		if (intPtr == (IntPtr)0)
		{
			ThrowHelper.ThrowNullReferenceException(this);
		}
		return Unmarshal.UnmarshalUnityObject<Texture2D>(GetSecondaryTexture_Injected(intPtr, index));
	}

	public int GetSecondaryTextureCount()
	{
		IntPtr intPtr = MarshalledUnityObject.MarshalNotNull(this);
		if (intPtr == (IntPtr)0)
		{
			ThrowHelper.ThrowNullReferenceException(this);
		}
		return GetSecondaryTextureCount_Injected(intPtr);
	}

	[FreeFunction("SpritesBindings::GetSecondaryTextures", ThrowsException = true, HasExplicitThis = true)]
	public int GetSecondaryTextures([NotNull][UnityMarshalAs(NativeType.ScriptingObjectPtr)] SecondarySpriteTexture[] secondaryTexture)
	{
		if (secondaryTexture == null)
		{
			ThrowHelper.ThrowArgumentNullException(secondaryTexture, "secondaryTexture");
		}
		IntPtr intPtr = MarshalledUnityObject.MarshalNotNull(this);
		if (intPtr == (IntPtr)0)
		{
			ThrowHelper.ThrowNullReferenceException(this);
		}
		return GetSecondaryTextures_Injected(intPtr, secondaryTexture);
	}

	public int GetPhysicsShapeCount()
	{
		IntPtr intPtr = MarshalledUnityObject.MarshalNotNull(this);
		if (intPtr == (IntPtr)0)
		{
			ThrowHelper.ThrowNullReferenceException(this);
		}
		return GetPhysicsShapeCount_Injected(intPtr);
	}

	public uint GetScriptableObjectsCount()
	{
		IntPtr intPtr = MarshalledUnityObject.MarshalNotNull(this);
		if (intPtr == (IntPtr)0)
		{
			ThrowHelper.ThrowNullReferenceException(this);
		}
		return GetScriptableObjectsCount_Injected(intPtr);
	}

	[FreeFunction("SpritesBindings::GetScriptableObjects", ThrowsException = true, HasExplicitThis = true)]
	public uint GetScriptableObjects([UnityMarshalAs(NativeType.ScriptingObjectPtr)][NotNull] ScriptableObject[] scriptableObjects)
	{
		if (scriptableObjects == null)
		{
			ThrowHelper.ThrowArgumentNullException(scriptableObjects, "scriptableObjects");
		}
		IntPtr intPtr = MarshalledUnityObject.MarshalNotNull(this);
		if (intPtr == (IntPtr)0)
		{
			ThrowHelper.ThrowNullReferenceException(this);
		}
		return GetScriptableObjects_Injected(intPtr, scriptableObjects);
	}

	public bool AddScriptableObject([NotNull] ScriptableObject obj)
	{
		if ((object)obj == null)
		{
			ThrowHelper.ThrowArgumentNullException(obj, "obj");
		}
		IntPtr intPtr = MarshalledUnityObject.MarshalNotNull(this);
		if (intPtr == (IntPtr)0)
		{
			ThrowHelper.ThrowNullReferenceException(this);
		}
		IntPtr intPtr2 = MarshalledUnityObject.MarshalNotNull(obj);
		if (intPtr2 == (IntPtr)0)
		{
			ThrowHelper.ThrowArgumentNullException(obj, "obj");
		}
		return AddScriptableObject_Injected(intPtr, intPtr2);
	}

	public bool RemoveScriptableObjectAt(uint i)
	{
		IntPtr intPtr = MarshalledUnityObject.MarshalNotNull(this);
		if (intPtr == (IntPtr)0)
		{
			ThrowHelper.ThrowNullReferenceException(this);
		}
		return RemoveScriptableObjectAt_Injected(intPtr, i);
	}

	public bool SetScriptableObjectAt([NotNull] ScriptableObject obj, uint i)
	{
		if ((object)obj == null)
		{
			ThrowHelper.ThrowArgumentNullException(obj, "obj");
		}
		IntPtr intPtr = MarshalledUnityObject.MarshalNotNull(this);
		if (intPtr == (IntPtr)0)
		{
			ThrowHelper.ThrowNullReferenceException(this);
		}
		IntPtr intPtr2 = MarshalledUnityObject.MarshalNotNull(obj);
		if (intPtr2 == (IntPtr)0)
		{
			ThrowHelper.ThrowArgumentNullException(obj, "obj");
		}
		return SetScriptableObjectAt_Injected(intPtr, intPtr2, i);
	}

	public int GetPhysicsShapePointCount(int shapeIdx)
	{
		int physicsShapeCount = GetPhysicsShapeCount();
		if (shapeIdx < 0 || shapeIdx >= physicsShapeCount)
		{
			throw new IndexOutOfRangeException($"Index({shapeIdx}) is out of bounds(0 - {physicsShapeCount - 1})");
		}
		return Internal_GetPhysicsShapePointCount(shapeIdx);
	}

	[NativeMethod("GetPhysicsShapePointCount")]
	private int Internal_GetPhysicsShapePointCount(int shapeIdx)
	{
		IntPtr intPtr = MarshalledUnityObject.MarshalNotNull(this);
		if (intPtr == (IntPtr)0)
		{
			ThrowHelper.ThrowNullReferenceException(this);
		}
		return Internal_GetPhysicsShapePointCount_Injected(intPtr, shapeIdx);
	}

	public int GetPhysicsShape(int shapeIdx, List<Vector2> physicsShape)
	{
		int physicsShapeCount = GetPhysicsShapeCount();
		if (shapeIdx < 0 || shapeIdx >= physicsShapeCount)
		{
			throw new IndexOutOfRangeException($"Index({shapeIdx}) is out of bounds(0 - {physicsShapeCount - 1})");
		}
		GetPhysicsShapeImpl(this, shapeIdx, physicsShape);
		return physicsShape.Count;
	}

	public ReadOnlySpan<Vector2> GetPhysicsShape(int shapeIdx)
	{
		int physicsShapeCount = GetPhysicsShapeCount();
		if (shapeIdx < 0 || shapeIdx >= physicsShapeCount)
		{
			throw new IndexOutOfRangeException($"Index({shapeIdx}) is out of bounds(0 - {physicsShapeCount - 1})");
		}
		return GetPhysicsShapeSpanImpl(this, shapeIdx);
	}

	[FreeFunction("SpritesBindings::GetPhysicsShape", ThrowsException = true)]
	private unsafe static void GetPhysicsShapeImpl(Sprite sprite, int shapeIdx, [NotNull] List<Vector2> physicsShape)
	{
		if (physicsShape == null)
		{
			ThrowHelper.ThrowArgumentNullException(physicsShape, "physicsShape");
		}
		List<Vector2> list = default(List<Vector2>);
		BlittableListWrapper physicsShape2 = default(BlittableListWrapper);
		try
		{
			IntPtr sprite2 = MarshalledUnityObject.Marshal(sprite);
			list = physicsShape;
			fixed (Vector2[] array = NoAllocHelpers.ExtractArrayFromList(list))
			{
				BlittableArrayWrapper arrayWrapper = default(BlittableArrayWrapper);
				if (array.Length != 0)
				{
					arrayWrapper = new BlittableArrayWrapper(Unsafe.AsPointer(ref array[0]), array.Length);
				}
				physicsShape2 = new BlittableListWrapper(arrayWrapper, list.Count);
				GetPhysicsShapeImpl_Injected(sprite2, shapeIdx, ref physicsShape2);
			}
		}
		finally
		{
			physicsShape2.Unmarshal(list);
		}
	}

	[FreeFunction("SpritesBindings::GetPhysicsShape", ThrowsException = true)]
	private static ReadOnlySpan<Vector2> GetPhysicsShapeSpanImpl(Sprite sprite, int shapeIdx)
	{
		GetPhysicsShapeSpanImpl_Injected(MarshalledUnityObject.Marshal(sprite), shapeIdx, out var ret);
		return ManagedSpanWrapper.ToReadOnlySpan<Vector2>(ret);
	}

	public void OverridePhysicsShape(IList<Vector2[]> physicsShapes)
	{
		if (physicsShapes == null)
		{
			throw new ArgumentNullException("physicsShapes");
		}
		for (int i = 0; i < physicsShapes.Count; i++)
		{
			Vector2[] array = physicsShapes[i];
			if (array == null)
			{
				throw new ArgumentNullException("physicsShape", $"Physics Shape at {i} is null.");
			}
			if (array.Length < 3)
			{
				throw new ArgumentException($"Physics Shape at {i} has less than 3 vertices ({array.Length}).");
			}
		}
		OverridePhysicsShapeCount(this, physicsShapes.Count);
		for (int j = 0; j < physicsShapes.Count; j++)
		{
			OverridePhysicsShape(this, physicsShapes[j], j);
		}
	}

	[FreeFunction("SpritesBindings::OverridePhysicsShapeCount")]
	private static void OverridePhysicsShapeCount(Sprite sprite, int physicsShapeCount)
	{
		OverridePhysicsShapeCount_Injected(MarshalledUnityObject.Marshal(sprite), physicsShapeCount);
	}

	[FreeFunction("SpritesBindings::OverridePhysicsShape", ThrowsException = true)]
	private unsafe static void OverridePhysicsShape(Sprite sprite, [NotNull] Vector2[] physicsShape, int idx)
	{
		if (physicsShape == null)
		{
			ThrowHelper.ThrowArgumentNullException(physicsShape, "physicsShape");
		}
		IntPtr sprite2 = MarshalledUnityObject.Marshal(sprite);
		Span<Vector2> span = new Span<Vector2>(physicsShape);
		fixed (Vector2* begin = span)
		{
			ManagedSpanWrapper physicsShape2 = new ManagedSpanWrapper(begin, span.Length);
			OverridePhysicsShape_Injected(sprite2, ref physicsShape2, idx);
		}
	}

	[FreeFunction("SpritesBindings::OverrideGeometry", HasExplicitThis = true)]
	public unsafe void OverrideGeometry([NotNull] Vector2[] vertices, [NotNull] ushort[] triangles)
	{
		if (vertices == null)
		{
			ThrowHelper.ThrowArgumentNullException(vertices, "vertices");
		}
		if (triangles == null)
		{
			ThrowHelper.ThrowArgumentNullException(triangles, "triangles");
		}
		IntPtr intPtr = MarshalledUnityObject.MarshalNotNull(this);
		if (intPtr == (IntPtr)0)
		{
			ThrowHelper.ThrowNullReferenceException(this);
		}
		Span<Vector2> span = new Span<Vector2>(vertices);
		fixed (Vector2* begin = span)
		{
			ManagedSpanWrapper managedSpanWrapper = new ManagedSpanWrapper(begin, span.Length);
			Span<ushort> span2 = new Span<ushort>(triangles);
			fixed (ushort* begin2 = span2)
			{
				ManagedSpanWrapper managedSpanWrapper2 = new ManagedSpanWrapper(begin2, span2.Length);
				OverrideGeometry_Injected(intPtr, ref managedSpanWrapper, ref managedSpanWrapper2);
			}
		}
	}

	[VisibleToOtherModules]
	internal static Sprite Create(Rect rect, Vector2 pivot, float pixelsToUnits, Texture2D texture)
	{
		return CreateSpriteWithoutTextureScripting(rect, pivot, pixelsToUnits, texture);
	}

	internal static Sprite Create(Rect rect, Vector2 pivot, float pixelsToUnits)
	{
		return CreateSpriteWithoutTextureScripting(rect, pivot, pixelsToUnits, null);
	}

	public static Sprite Create(Texture2D texture, Rect rect, Vector2 pivot, float pixelsPerUnit, uint extrude, SpriteMeshType meshType, Vector4 border, bool generateFallbackPhysicsShape)
	{
		return Create(texture, rect, pivot, pixelsPerUnit, extrude, meshType, border, generateFallbackPhysicsShape, null);
	}

	public static Sprite Create(Texture2D texture, Rect rect, Vector2 pivot, float pixelsPerUnit, uint extrude, SpriteMeshType meshType, Vector4 border, bool generateFallbackPhysicsShape, SecondarySpriteTexture[] secondaryTextures)
	{
		if (texture == null)
		{
			return null;
		}
		if (rect.xMax > (float)texture.width || rect.yMax > (float)texture.height)
		{
			throw new ArgumentException($"Could not create sprite ({rect.x}, {rect.y}, {rect.width}, {rect.height}) from a {texture.width}x{texture.height} texture.");
		}
		if (pixelsPerUnit <= 0f)
		{
			throw new ArgumentException("pixelsPerUnit must be set to a positive non-zero value.");
		}
		if (secondaryTextures != null)
		{
			for (int i = 0; i < secondaryTextures.Length; i++)
			{
				SecondarySpriteTexture secondarySpriteTexture = secondaryTextures[i];
				if (secondarySpriteTexture.texture == texture)
				{
					throw new ArgumentException($"{secondarySpriteTexture.name} is using source Texture as Secondary Texture.");
				}
			}
		}
		return CreateSprite(texture, rect, pivot, pixelsPerUnit, extrude, meshType, border, generateFallbackPhysicsShape, secondaryTextures);
	}

	public static Sprite Create(Texture2D texture, Rect rect, Vector2 pivot, float pixelsPerUnit, uint extrude, SpriteMeshType meshType, Vector4 border)
	{
		return Create(texture, rect, pivot, pixelsPerUnit, extrude, meshType, border, generateFallbackPhysicsShape: false);
	}

	public static Sprite Create(Texture2D texture, Rect rect, Vector2 pivot, float pixelsPerUnit, uint extrude, SpriteMeshType meshType)
	{
		return Create(texture, rect, pivot, pixelsPerUnit, extrude, meshType, Vector4.zero);
	}

	public static Sprite Create(Texture2D texture, Rect rect, Vector2 pivot, float pixelsPerUnit, uint extrude)
	{
		return Create(texture, rect, pivot, pixelsPerUnit, extrude, SpriteMeshType.Tight);
	}

	public static Sprite Create(Texture2D texture, Rect rect, Vector2 pivot, float pixelsPerUnit)
	{
		return Create(texture, rect, pivot, pixelsPerUnit, 0u);
	}

	public static Sprite Create(Texture2D texture, Rect rect, Vector2 pivot)
	{
		return Create(texture, rect, pivot, 100f);
	}

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern int GetPackingMode_Injected(IntPtr _unity_self);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern int GetPackingRotation_Injected(IntPtr _unity_self);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern int GetPacked_Injected(IntPtr _unity_self);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern void GetTextureRect_Injected(IntPtr _unity_self, out Rect ret);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern void GetTextureRectOffset_Injected(IntPtr _unity_self, out Vector2 ret);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern void GetInnerUVs_Injected(IntPtr _unity_self, out Vector4 ret);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern void GetOuterUVs_Injected(IntPtr _unity_self, out Vector4 ret);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern void GetPadding_Injected(IntPtr _unity_self, out Vector4 ret);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern IntPtr CreateSpriteWithoutTextureScripting_Injected([In] ref Rect rect, [In] ref Vector2 pivot, float pixelsToUnits, IntPtr texture);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern IntPtr CreateSprite_Injected(IntPtr texture, [In] ref Rect rect, [In] ref Vector2 pivot, float pixelsPerUnit, uint extrude, SpriteMeshType meshType, [In] ref Vector4 border, bool generateFallbackPhysicsShape, SecondarySpriteTexture[] secondaryTexture);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern void get_bounds_Injected(IntPtr _unity_self, out Bounds ret);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern void get_rect_Injected(IntPtr _unity_self, out Rect ret);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern void get_border_Injected(IntPtr _unity_self, out Vector4 ret);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern IntPtr get_texture_Injected(IntPtr _unity_self);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern uint get_extrude_Injected(IntPtr _unity_self);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern IntPtr GetSecondaryTexture_Injected(IntPtr _unity_self, int index);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern int GetSecondaryTextureCount_Injected(IntPtr _unity_self);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern int GetSecondaryTextures_Injected(IntPtr _unity_self, SecondarySpriteTexture[] secondaryTexture);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern float get_pixelsPerUnit_Injected(IntPtr _unity_self);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern float get_spriteAtlasTextureScale_Injected(IntPtr _unity_self);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern IntPtr get_associatedAlphaSplitTexture_Injected(IntPtr _unity_self);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern void get_pivot_Injected(IntPtr _unity_self, out Vector2 ret);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern Vector2[] get_vertices_Injected(IntPtr _unity_self);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern ushort[] get_triangles_Injected(IntPtr _unity_self);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern Vector2[] get_uv_Injected(IntPtr _unity_self);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern int GetPhysicsShapeCount_Injected(IntPtr _unity_self);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern uint GetScriptableObjectsCount_Injected(IntPtr _unity_self);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern uint GetScriptableObjects_Injected(IntPtr _unity_self, ScriptableObject[] scriptableObjects);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern bool AddScriptableObject_Injected(IntPtr _unity_self, IntPtr obj);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern bool RemoveScriptableObjectAt_Injected(IntPtr _unity_self, uint i);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern bool SetScriptableObjectAt_Injected(IntPtr _unity_self, IntPtr obj, uint i);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern int Internal_GetPhysicsShapePointCount_Injected(IntPtr _unity_self, int shapeIdx);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern void GetPhysicsShapeImpl_Injected(IntPtr sprite, int shapeIdx, ref BlittableListWrapper physicsShape);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern void GetPhysicsShapeSpanImpl_Injected(IntPtr sprite, int shapeIdx, out ManagedSpanWrapper ret);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern void OverridePhysicsShapeCount_Injected(IntPtr sprite, int physicsShapeCount);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern void OverridePhysicsShape_Injected(IntPtr sprite, ref ManagedSpanWrapper physicsShape, int idx);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern void OverrideGeometry_Injected(IntPtr _unity_self, ref ManagedSpanWrapper vertices, ref ManagedSpanWrapper triangles);
}
