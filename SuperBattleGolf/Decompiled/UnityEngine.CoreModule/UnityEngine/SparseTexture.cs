using System;
using System.Runtime.CompilerServices;
using UnityEngine.Bindings;
using UnityEngine.Experimental.Rendering;
using UnityEngine.Internal;

namespace UnityEngine;

[NativeHeader("Runtime/Graphics/SparseTexture.h")]
public sealed class SparseTexture : Texture
{
	public int tileWidth
	{
		get
		{
			IntPtr intPtr = MarshalledUnityObject.MarshalNotNull(this);
			if (intPtr == (IntPtr)0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			return get_tileWidth_Injected(intPtr);
		}
	}

	public int tileHeight
	{
		get
		{
			IntPtr intPtr = MarshalledUnityObject.MarshalNotNull(this);
			if (intPtr == (IntPtr)0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			return get_tileHeight_Injected(intPtr);
		}
	}

	public bool isCreated
	{
		[NativeName("IsInitialized")]
		get
		{
			IntPtr intPtr = MarshalledUnityObject.MarshalNotNull(this);
			if (intPtr == (IntPtr)0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			return get_isCreated_Injected(intPtr);
		}
	}

	[MethodImpl(MethodImplOptions.InternalCall)]
	[FreeFunction(Name = "SparseTextureScripting::Create", ThrowsException = true)]
	private static extern void Internal_Create([Writable] SparseTexture mono, int width, int height, GraphicsFormat format, TextureColorSpace colorSpace, int mipCount);

	[FreeFunction(Name = "SparseTextureScripting::UpdateTile", HasExplicitThis = true)]
	public unsafe void UpdateTile(int tileX, int tileY, int miplevel, Color32[] data)
	{
		IntPtr intPtr = MarshalledUnityObject.MarshalNotNull(this);
		if (intPtr == (IntPtr)0)
		{
			ThrowHelper.ThrowNullReferenceException(this);
		}
		Span<Color32> span = new Span<Color32>(data);
		fixed (Color32* begin = span)
		{
			ManagedSpanWrapper data2 = new ManagedSpanWrapper(begin, span.Length);
			UpdateTile_Injected(intPtr, tileX, tileY, miplevel, ref data2);
		}
	}

	[FreeFunction(Name = "SparseTextureScripting::UpdateTileRaw", HasExplicitThis = true)]
	public unsafe void UpdateTileRaw(int tileX, int tileY, int miplevel, byte[] data)
	{
		IntPtr intPtr = MarshalledUnityObject.MarshalNotNull(this);
		if (intPtr == (IntPtr)0)
		{
			ThrowHelper.ThrowNullReferenceException(this);
		}
		Span<byte> span = new Span<byte>(data);
		fixed (byte* begin = span)
		{
			ManagedSpanWrapper data2 = new ManagedSpanWrapper(begin, span.Length);
			UpdateTileRaw_Injected(intPtr, tileX, tileY, miplevel, ref data2);
		}
	}

	public void UnloadTile(int tileX, int tileY, int miplevel)
	{
		UpdateTileRaw(tileX, tileY, miplevel, null);
	}

	internal bool ValidateFormat(TextureFormat format, int width, int height)
	{
		bool flag = ValidateFormat(format);
		if (flag && TextureFormat.PVRTC_RGB2 <= format && format <= TextureFormat.PVRTC_RGBA4 && (width != height || !Mathf.IsPowerOfTwo(width)))
		{
			throw new UnityException($"'{format.ToString()}' demands texture to be square and have power-of-two dimensions");
		}
		return flag;
	}

	internal bool ValidateFormat(GraphicsFormat format, int width, int height)
	{
		bool flag = ValidateFormat(format, GraphicsFormatUsage.Sparse);
		if (flag && GraphicsFormatUtility.IsPVRTCFormat(format) && (width != height || !Mathf.IsPowerOfTwo(width)))
		{
			throw new UnityException($"'{format.ToString()}' demands texture to be square and have power-of-two dimensions");
		}
		return flag;
	}

	internal bool ValidateSize(int width, int height, GraphicsFormat format)
	{
		if (GraphicsFormatUtility.GetBlockSize(format) * (width / GraphicsFormatUtility.GetBlockWidth(format)) * (height / GraphicsFormatUtility.GetBlockHeight(format)) < 65536)
		{
			Debug.LogError("SparseTexture creation failed. The minimum size in bytes of a SparseTexture is 64KB.", this);
			return false;
		}
		return true;
	}

	private static void ValidateIsNotCrunched(TextureFormat textureFormat)
	{
		if (GraphicsFormatUtility.IsCrunchFormat(textureFormat))
		{
			throw new ArgumentException("Crunched SparseTexture is not supported.");
		}
	}

	[ExcludeFromDocs]
	public SparseTexture(int width, int height, DefaultFormat format, int mipCount)
		: this(width, height, SystemInfo.GetGraphicsFormat(format), mipCount)
	{
	}

	[ExcludeFromDocs]
	public SparseTexture(int width, int height, GraphicsFormat format, int mipCount)
	{
		if (ValidateFormat(format, width, height) && ValidateSize(width, height, format))
		{
			Internal_Create(this, width, height, format, GetTextureColorSpace(format), mipCount);
		}
	}

	[ExcludeFromDocs]
	public SparseTexture(int width, int height, TextureFormat textureFormat, int mipCount)
		: this(width, height, textureFormat, mipCount, linear: false)
	{
	}

	public SparseTexture(int width, int height, TextureFormat textureFormat, int mipCount, [DefaultValue("false")] bool linear)
	{
		if (ValidateFormat(textureFormat, width, height))
		{
			ValidateIsNotCrunched(textureFormat);
			GraphicsFormat format = GraphicsFormatUtility.GetGraphicsFormat(textureFormat, !linear);
			if (!SystemInfo.IsFormatSupported(format, GraphicsFormatUsage.Sparse))
			{
				Debug.LogError($"Creation of a SparseTexture with '{textureFormat}' is not supported on this platform.");
			}
			else if (ValidateSize(width, height, format))
			{
				Internal_Create(this, width, height, format, GetTextureColorSpace(linear), mipCount);
			}
		}
	}

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern int get_tileWidth_Injected(IntPtr _unity_self);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern int get_tileHeight_Injected(IntPtr _unity_self);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern bool get_isCreated_Injected(IntPtr _unity_self);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern void UpdateTile_Injected(IntPtr _unity_self, int tileX, int tileY, int miplevel, ref ManagedSpanWrapper data);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern void UpdateTileRaw_Injected(IntPtr _unity_self, int tileX, int tileY, int miplevel, ref ManagedSpanWrapper data);
}
