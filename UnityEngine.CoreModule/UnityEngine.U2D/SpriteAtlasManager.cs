using System;
using System.Runtime.CompilerServices;
using UnityEngine.Bindings;
using UnityEngine.Scripting;

namespace UnityEngine.U2D;

[NativeHeader("Runtime/2D/SpriteAtlas/SpriteAtlasManager.h")]
[NativeHeader("Runtime/2D/SpriteAtlas/SpriteAtlas.h")]
[StaticAccessor("GetSpriteAtlasManager()", StaticAccessorType.Dot)]
public class SpriteAtlasManager
{
	public static event Action<string, Action<SpriteAtlas>> atlasRequested;

	public static event Action<SpriteAtlas> atlasRegistered;

	[RequiredByNativeCode]
	private static bool RequestAtlas(string tag)
	{
		if (SpriteAtlasManager.atlasRequested != null)
		{
			SpriteAtlasManager.atlasRequested(tag, Register);
			return true;
		}
		return false;
	}

	[RequiredByNativeCode]
	private static void PostRegisteredAtlas(SpriteAtlas spriteAtlas)
	{
		SpriteAtlasManager.atlasRegistered?.Invoke(spriteAtlas);
	}

	internal static void Register(SpriteAtlas spriteAtlas)
	{
		Register_Injected(Object.MarshalledUnityObject.Marshal(spriteAtlas));
	}

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern void Register_Injected(IntPtr spriteAtlas);
}
