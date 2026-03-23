using System.Runtime.CompilerServices;
using UnityEngine.Bindings;
using UnityEngine.Experimental.Rendering;

namespace UnityEngine.Rendering;

[NativeHeader("Runtime/Graphics/ShadingRateInfo.h")]
public static class ShadingRateInfo
{
	public static bool supportsPerImageTile => SupportsPerImageTile();

	public static bool supportsPerDrawCall => SupportsPerDrawCall();

	public static Vector2Int imageTileSize => GetImageTileSize();

	public static ShadingRateFragmentSize[] availableFragmentSizes => GetAvailableFragmentSizes();

	public static GraphicsFormat graphicsFormat => GetGraphicsFormat();

	[MethodImpl(MethodImplOptions.InternalCall)]
	[FreeFunction("ShadingRateInfo::QueryNativeValue")]
	public static extern byte QueryNativeValue(ShadingRateFragmentSize fragmentSize);

	[MethodImpl(MethodImplOptions.InternalCall)]
	[FreeFunction("ShadingRateInfo::SupportsPerImageTile")]
	private static extern bool SupportsPerImageTile();

	[MethodImpl(MethodImplOptions.InternalCall)]
	[FreeFunction("ShadingRateInfo::SupportsPerDrawCall")]
	private static extern bool SupportsPerDrawCall();

	[FreeFunction("ShadingRateInfo::GetImageTileSize")]
	private static Vector2Int GetImageTileSize()
	{
		GetImageTileSize_Injected(out var ret);
		return ret;
	}

	[FreeFunction("ShadingRateInfo::GetAvailableFragmentSizes")]
	private static ShadingRateFragmentSize[] GetAvailableFragmentSizes()
	{
		BlittableArrayWrapper ret = default(BlittableArrayWrapper);
		ShadingRateFragmentSize[] result;
		try
		{
			GetAvailableFragmentSizes_Injected(out ret);
		}
		finally
		{
			ShadingRateFragmentSize[] array = default(ShadingRateFragmentSize[]);
			ret.Unmarshal(ref array);
			result = array;
		}
		return result;
	}

	[MethodImpl(MethodImplOptions.InternalCall)]
	[FreeFunction("ShadingRateInfo::GetGraphicsFormat")]
	private static extern GraphicsFormat GetGraphicsFormat();

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern void GetImageTileSize_Injected(out Vector2Int ret);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern void GetAvailableFragmentSizes_Injected(out BlittableArrayWrapper ret);
}
