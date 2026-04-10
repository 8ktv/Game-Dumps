using System;
using System.Runtime.CompilerServices;
using UnityEngine.Bindings;

namespace UnityEngine.U2D;

[NativeHeader("Runtime/Graphics/SpriteFrame.h")]
[NativeType(Header = "Runtime/2D/SpriteAtlas/SpriteAtlas.h")]
public class SpriteAtlas : Object
{
	public bool isVariant
	{
		[NativeMethod("IsVariant")]
		get
		{
			IntPtr intPtr = MarshalledUnityObject.MarshalNotNull(this);
			if (intPtr == (IntPtr)0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			return get_isVariant_Injected(intPtr);
		}
	}

	public string tag
	{
		get
		{
			ManagedSpanWrapper ret = default(ManagedSpanWrapper);
			string stringAndDispose;
			try
			{
				IntPtr intPtr = MarshalledUnityObject.MarshalNotNull(this);
				if (intPtr == (IntPtr)0)
				{
					ThrowHelper.ThrowNullReferenceException(this);
				}
				get_tag_Injected(intPtr, out ret);
			}
			finally
			{
				stringAndDispose = OutStringMarshaller.GetStringAndDispose(ret);
			}
			return stringAndDispose;
		}
	}

	public int spriteCount
	{
		get
		{
			IntPtr intPtr = MarshalledUnityObject.MarshalNotNull(this);
			if (intPtr == (IntPtr)0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			return get_spriteCount_Injected(intPtr);
		}
	}

	public bool CanBindTo([NotNull] Sprite sprite)
	{
		if ((object)sprite == null)
		{
			ThrowHelper.ThrowArgumentNullException(sprite, "sprite");
		}
		IntPtr intPtr = MarshalledUnityObject.MarshalNotNull(this);
		if (intPtr == (IntPtr)0)
		{
			ThrowHelper.ThrowNullReferenceException(this);
		}
		IntPtr intPtr2 = MarshalledUnityObject.MarshalNotNull(sprite);
		if (intPtr2 == (IntPtr)0)
		{
			ThrowHelper.ThrowArgumentNullException(sprite, "sprite");
		}
		return CanBindTo_Injected(intPtr, intPtr2);
	}

	public unsafe Sprite GetSprite(string name)
	{
		//The blocks IL_0039 are reachable both inside and outside the pinned region starting at IL_0028. ILSpy has duplicated these blocks in order to place them both within and outside the `fixed` statement.
		IntPtr sprite_Injected = default(IntPtr);
		Sprite result;
		try
		{
			IntPtr intPtr = MarshalledUnityObject.MarshalNotNull(this);
			if (intPtr == (IntPtr)0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			ManagedSpanWrapper managedSpanWrapper = default(ManagedSpanWrapper);
			if (!StringMarshaller.TryMarshalEmptyOrNullString(name, ref managedSpanWrapper))
			{
				ReadOnlySpan<char> readOnlySpan = MemoryExtensions.AsSpan(name);
				fixed (char* begin = readOnlySpan)
				{
					managedSpanWrapper = new ManagedSpanWrapper(begin, readOnlySpan.Length);
					sprite_Injected = GetSprite_Injected(intPtr, ref managedSpanWrapper);
				}
			}
			else
			{
				sprite_Injected = GetSprite_Injected(intPtr, ref managedSpanWrapper);
			}
		}
		finally
		{
			result = Unmarshal.UnmarshalUnityObject<Sprite>(sprite_Injected);
		}
		return result;
	}

	public int GetSprites(Sprite[] sprites)
	{
		return GetSpritesScripting(sprites);
	}

	public int GetSprites(Sprite[] sprites, string name)
	{
		return GetSpritesWithNameScripting(sprites, name);
	}

	private int GetSpritesScripting([UnityMarshalAs(NativeType.ScriptingObjectPtr)] Sprite[] sprites)
	{
		IntPtr intPtr = MarshalledUnityObject.MarshalNotNull(this);
		if (intPtr == (IntPtr)0)
		{
			ThrowHelper.ThrowNullReferenceException(this);
		}
		return GetSpritesScripting_Injected(intPtr, sprites);
	}

	private unsafe int GetSpritesWithNameScripting([UnityMarshalAs(NativeType.ScriptingObjectPtr)] Sprite[] sprites, string name)
	{
		//The blocks IL_003a are reachable both inside and outside the pinned region starting at IL_0029. ILSpy has duplicated these blocks in order to place them both within and outside the `fixed` statement.
		try
		{
			IntPtr intPtr = MarshalledUnityObject.MarshalNotNull(this);
			if (intPtr == (IntPtr)0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			ManagedSpanWrapper managedSpanWrapper = default(ManagedSpanWrapper);
			if (!StringMarshaller.TryMarshalEmptyOrNullString(name, ref managedSpanWrapper))
			{
				ReadOnlySpan<char> readOnlySpan = MemoryExtensions.AsSpan(name);
				fixed (char* begin = readOnlySpan)
				{
					managedSpanWrapper = new ManagedSpanWrapper(begin, readOnlySpan.Length);
					return GetSpritesWithNameScripting_Injected(intPtr, sprites, ref managedSpanWrapper);
				}
			}
			return GetSpritesWithNameScripting_Injected(intPtr, sprites, ref managedSpanWrapper);
		}
		finally
		{
		}
	}

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern bool get_isVariant_Injected(IntPtr _unity_self);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern void get_tag_Injected(IntPtr _unity_self, out ManagedSpanWrapper ret);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern int get_spriteCount_Injected(IntPtr _unity_self);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern bool CanBindTo_Injected(IntPtr _unity_self, IntPtr sprite);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern IntPtr GetSprite_Injected(IntPtr _unity_self, ref ManagedSpanWrapper name);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern int GetSpritesScripting_Injected(IntPtr _unity_self, Sprite[] sprites);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern int GetSpritesWithNameScripting_Injected(IntPtr _unity_self, Sprite[] sprites, ref ManagedSpanWrapper name);
}
