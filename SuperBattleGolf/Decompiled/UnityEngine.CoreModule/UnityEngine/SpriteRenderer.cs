using System;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using UnityEngine.Bindings;
using UnityEngine.Events;
using UnityEngine.Scripting;

namespace UnityEngine;

[RequireComponent(typeof(Transform))]
[NativeType("Runtime/Graphics/Mesh/SpriteRenderer.h")]
public sealed class SpriteRenderer : Renderer
{
	private UnityEvent<SpriteRenderer> m_SpriteChangeEvent;

	internal bool shouldSupportTiling
	{
		[NativeMethod("ShouldSupportTiling")]
		get
		{
			IntPtr intPtr = MarshalledUnityObject.MarshalNotNull(this);
			if (intPtr == (IntPtr)0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			return get_shouldSupportTiling_Injected(intPtr);
		}
	}

	internal bool hasSpriteChangeEvents
	{
		get
		{
			IntPtr intPtr = MarshalledUnityObject.MarshalNotNull(this);
			if (intPtr == (IntPtr)0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			return get_hasSpriteChangeEvents_Injected(intPtr);
		}
		set
		{
			IntPtr intPtr = MarshalledUnityObject.MarshalNotNull(this);
			if (intPtr == (IntPtr)0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			set_hasSpriteChangeEvents_Injected(intPtr, value);
		}
	}

	public Sprite sprite
	{
		get
		{
			IntPtr intPtr = MarshalledUnityObject.MarshalNotNull(this);
			if (intPtr == (IntPtr)0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			return Unmarshal.UnmarshalUnityObject<Sprite>(get_sprite_Injected(intPtr));
		}
		set
		{
			IntPtr intPtr = MarshalledUnityObject.MarshalNotNull(this);
			if (intPtr == (IntPtr)0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			set_sprite_Injected(intPtr, MarshalledUnityObject.Marshal(value));
		}
	}

	public SpriteDrawMode drawMode
	{
		get
		{
			IntPtr intPtr = MarshalledUnityObject.MarshalNotNull(this);
			if (intPtr == (IntPtr)0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			return get_drawMode_Injected(intPtr);
		}
		set
		{
			IntPtr intPtr = MarshalledUnityObject.MarshalNotNull(this);
			if (intPtr == (IntPtr)0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			set_drawMode_Injected(intPtr, value);
		}
	}

	public Vector2 size
	{
		get
		{
			IntPtr intPtr = MarshalledUnityObject.MarshalNotNull(this);
			if (intPtr == (IntPtr)0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			get_size_Injected(intPtr, out var ret);
			return ret;
		}
		set
		{
			IntPtr intPtr = MarshalledUnityObject.MarshalNotNull(this);
			if (intPtr == (IntPtr)0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			set_size_Injected(intPtr, ref value);
		}
	}

	public float adaptiveModeThreshold
	{
		get
		{
			IntPtr intPtr = MarshalledUnityObject.MarshalNotNull(this);
			if (intPtr == (IntPtr)0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			return get_adaptiveModeThreshold_Injected(intPtr);
		}
		set
		{
			IntPtr intPtr = MarshalledUnityObject.MarshalNotNull(this);
			if (intPtr == (IntPtr)0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			set_adaptiveModeThreshold_Injected(intPtr, value);
		}
	}

	public SpriteTileMode tileMode
	{
		get
		{
			IntPtr intPtr = MarshalledUnityObject.MarshalNotNull(this);
			if (intPtr == (IntPtr)0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			return get_tileMode_Injected(intPtr);
		}
		set
		{
			IntPtr intPtr = MarshalledUnityObject.MarshalNotNull(this);
			if (intPtr == (IntPtr)0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			set_tileMode_Injected(intPtr, value);
		}
	}

	public Color color
	{
		get
		{
			IntPtr intPtr = MarshalledUnityObject.MarshalNotNull(this);
			if (intPtr == (IntPtr)0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			get_color_Injected(intPtr, out var ret);
			return ret;
		}
		set
		{
			IntPtr intPtr = MarshalledUnityObject.MarshalNotNull(this);
			if (intPtr == (IntPtr)0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			set_color_Injected(intPtr, ref value);
		}
	}

	public SpriteMaskInteraction maskInteraction
	{
		get
		{
			IntPtr intPtr = MarshalledUnityObject.MarshalNotNull(this);
			if (intPtr == (IntPtr)0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			return get_maskInteraction_Injected(intPtr);
		}
		set
		{
			IntPtr intPtr = MarshalledUnityObject.MarshalNotNull(this);
			if (intPtr == (IntPtr)0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			set_maskInteraction_Injected(intPtr, value);
		}
	}

	public bool flipX
	{
		get
		{
			IntPtr intPtr = MarshalledUnityObject.MarshalNotNull(this);
			if (intPtr == (IntPtr)0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			return get_flipX_Injected(intPtr);
		}
		set
		{
			IntPtr intPtr = MarshalledUnityObject.MarshalNotNull(this);
			if (intPtr == (IntPtr)0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			set_flipX_Injected(intPtr, value);
		}
	}

	public bool flipY
	{
		get
		{
			IntPtr intPtr = MarshalledUnityObject.MarshalNotNull(this);
			if (intPtr == (IntPtr)0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			return get_flipY_Injected(intPtr);
		}
		set
		{
			IntPtr intPtr = MarshalledUnityObject.MarshalNotNull(this);
			if (intPtr == (IntPtr)0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			set_flipY_Injected(intPtr, value);
		}
	}

	public SpriteSortPoint spriteSortPoint
	{
		get
		{
			IntPtr intPtr = MarshalledUnityObject.MarshalNotNull(this);
			if (intPtr == (IntPtr)0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			return get_spriteSortPoint_Injected(intPtr);
		}
		set
		{
			IntPtr intPtr = MarshalledUnityObject.MarshalNotNull(this);
			if (intPtr == (IntPtr)0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			set_spriteSortPoint_Injected(intPtr, value);
		}
	}

	public void RegisterSpriteChangeCallback(UnityAction<SpriteRenderer> callback)
	{
		if (m_SpriteChangeEvent == null)
		{
			m_SpriteChangeEvent = new UnityEvent<SpriteRenderer>();
		}
		m_SpriteChangeEvent.AddListener(callback);
		hasSpriteChangeEvents = true;
	}

	public void UnregisterSpriteChangeCallback(UnityAction<SpriteRenderer> callback)
	{
		if (m_SpriteChangeEvent != null)
		{
			m_SpriteChangeEvent.RemoveListener(callback);
			if (m_SpriteChangeEvent.GetCallsCount() == 0)
			{
				hasSpriteChangeEvents = false;
			}
		}
	}

	[RequiredByNativeCode]
	private void InvokeSpriteChanged()
	{
		try
		{
			m_SpriteChangeEvent?.Invoke(this);
		}
		catch (Exception exception)
		{
			Debug.LogException(exception, this);
		}
	}

	private IntPtr GetCurrentMeshDataPtr()
	{
		IntPtr intPtr = MarshalledUnityObject.MarshalNotNull(this);
		if (intPtr == (IntPtr)0)
		{
			ThrowHelper.ThrowNullReferenceException(this);
		}
		return GetCurrentMeshDataPtr_Injected(intPtr);
	}

	internal unsafe Mesh.MeshDataArray GetCurrentMeshData()
	{
		IntPtr currentMeshDataPtr = GetCurrentMeshDataPtr();
		if (currentMeshDataPtr == IntPtr.Zero)
		{
			return new Mesh.MeshDataArray(0);
		}
		Mesh.MeshDataArray result = new Mesh.MeshDataArray(1);
		*result.m_Ptrs = currentMeshDataPtr;
		return result;
	}

	[NativeMethod(Name = "GetSpriteBounds")]
	internal Bounds Internal_GetSpriteBounds(SpriteDrawMode mode)
	{
		IntPtr intPtr = MarshalledUnityObject.MarshalNotNull(this);
		if (intPtr == (IntPtr)0)
		{
			ThrowHelper.ThrowNullReferenceException(this);
		}
		Internal_GetSpriteBounds_Injected(intPtr, mode, out var ret);
		return ret;
	}

	internal void GetSecondaryTextureProperties([NotNull] MaterialPropertyBlock mbp)
	{
		if (mbp == null)
		{
			ThrowHelper.ThrowArgumentNullException(mbp, "mbp");
		}
		IntPtr intPtr = MarshalledUnityObject.MarshalNotNull(this);
		if (intPtr == (IntPtr)0)
		{
			ThrowHelper.ThrowNullReferenceException(this);
		}
		IntPtr intPtr2 = MaterialPropertyBlock.BindingsMarshaller.ConvertToNative(mbp);
		if (intPtr2 == (IntPtr)0)
		{
			ThrowHelper.ThrowArgumentNullException(mbp, "mbp");
		}
		GetSecondaryTextureProperties_Injected(intPtr, intPtr2);
	}

	internal Bounds GetSpriteBounds()
	{
		return Internal_GetSpriteBounds(drawMode);
	}

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern bool get_shouldSupportTiling_Injected(IntPtr _unity_self);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern bool get_hasSpriteChangeEvents_Injected(IntPtr _unity_self);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern void set_hasSpriteChangeEvents_Injected(IntPtr _unity_self, bool value);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern IntPtr get_sprite_Injected(IntPtr _unity_self);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern void set_sprite_Injected(IntPtr _unity_self, IntPtr value);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern SpriteDrawMode get_drawMode_Injected(IntPtr _unity_self);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern void set_drawMode_Injected(IntPtr _unity_self, SpriteDrawMode value);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern void get_size_Injected(IntPtr _unity_self, out Vector2 ret);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern void set_size_Injected(IntPtr _unity_self, [In] ref Vector2 value);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern float get_adaptiveModeThreshold_Injected(IntPtr _unity_self);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern void set_adaptiveModeThreshold_Injected(IntPtr _unity_self, float value);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern SpriteTileMode get_tileMode_Injected(IntPtr _unity_self);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern void set_tileMode_Injected(IntPtr _unity_self, SpriteTileMode value);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern void get_color_Injected(IntPtr _unity_self, out Color ret);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern void set_color_Injected(IntPtr _unity_self, [In] ref Color value);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern SpriteMaskInteraction get_maskInteraction_Injected(IntPtr _unity_self);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern void set_maskInteraction_Injected(IntPtr _unity_self, SpriteMaskInteraction value);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern bool get_flipX_Injected(IntPtr _unity_self);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern void set_flipX_Injected(IntPtr _unity_self, bool value);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern bool get_flipY_Injected(IntPtr _unity_self);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern void set_flipY_Injected(IntPtr _unity_self, bool value);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern SpriteSortPoint get_spriteSortPoint_Injected(IntPtr _unity_self);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern void set_spriteSortPoint_Injected(IntPtr _unity_self, SpriteSortPoint value);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern IntPtr GetCurrentMeshDataPtr_Injected(IntPtr _unity_self);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern void Internal_GetSpriteBounds_Injected(IntPtr _unity_self, SpriteDrawMode mode, out Bounds ret);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern void GetSecondaryTextureProperties_Injected(IntPtr _unity_self, IntPtr mbp);
}
