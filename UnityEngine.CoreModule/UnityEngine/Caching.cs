using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using UnityEngine.Bindings;

namespace UnityEngine;

[NativeHeader("Runtime/Misc/CachingManager.h")]
[StaticAccessor("GetCachingManager()", StaticAccessorType.Dot)]
public sealed class Caching
{
	public static extern bool compressionEnabled
	{
		[MethodImpl(MethodImplOptions.InternalCall)]
		get;
		[MethodImpl(MethodImplOptions.InternalCall)]
		set;
	}

	public static extern bool ready
	{
		[MethodImpl(MethodImplOptions.InternalCall)]
		[NativeName("GetIsReady")]
		get;
	}

	[Obsolete("Please use use Cache.spaceOccupied to get used bytes per cache.")]
	public static int spaceUsed => (int)spaceOccupied;

	[Obsolete("This property is only used for the current cache, use Cache.spaceOccupied to get used bytes per cache.")]
	public static extern long spaceOccupied
	{
		[MethodImpl(MethodImplOptions.InternalCall)]
		[NativeName("GetCachingDiskSpaceUsed")]
		[StaticAccessor("GetCachingManager().GetCurrentCache()", StaticAccessorType.Dot)]
		get;
	}

	[Obsolete("Please use use Cache.spaceOccupied to get used bytes per cache.")]
	public static int spaceAvailable => (int)spaceFree;

	[Obsolete("This property is only used for the current cache, use Cache.spaceFree to get unused bytes per cache.")]
	public static extern long spaceFree
	{
		[MethodImpl(MethodImplOptions.InternalCall)]
		[StaticAccessor("GetCachingManager().GetCurrentCache()", StaticAccessorType.Dot)]
		[NativeName("GetCachingDiskSpaceFree")]
		get;
	}

	[Obsolete("This property is only used for the current cache, use Cache.maximumAvailableStorageSpace to access the maximum available storage space per cache.")]
	[StaticAccessor("GetCachingManager().GetCurrentCache()", StaticAccessorType.Dot)]
	public static extern long maximumAvailableDiskSpace
	{
		[MethodImpl(MethodImplOptions.InternalCall)]
		[NativeName("GetMaximumDiskSpaceAvailable")]
		get;
		[MethodImpl(MethodImplOptions.InternalCall)]
		[NativeName("SetMaximumDiskSpaceAvailable")]
		set;
	}

	[StaticAccessor("GetCachingManager().GetCurrentCache()", StaticAccessorType.Dot)]
	[Obsolete("This property is only used for the current cache, use Cache.expirationDelay to access the expiration delay per cache.")]
	public static extern int expirationDelay
	{
		[MethodImpl(MethodImplOptions.InternalCall)]
		get;
		[MethodImpl(MethodImplOptions.InternalCall)]
		set;
	}

	public static extern int cacheCount
	{
		[MethodImpl(MethodImplOptions.InternalCall)]
		get;
	}

	[StaticAccessor("CachingManagerWrapper", StaticAccessorType.DoubleColon)]
	public static Cache defaultCache
	{
		[NativeName("Caching_GetDefaultCacheHandle")]
		get
		{
			get_defaultCache_Injected(out var ret);
			return ret;
		}
	}

	[StaticAccessor("CachingManagerWrapper", StaticAccessorType.DoubleColon)]
	public static Cache currentCacheForWriting
	{
		[NativeName("Caching_GetCurrentCacheHandle")]
		get
		{
			get_currentCacheForWriting_Injected(out var ret);
			return ret;
		}
		[NativeThrows]
		[NativeName("Caching_SetCurrentCacheByHandle")]
		set
		{
			set_currentCacheForWriting_Injected(ref value);
		}
	}

	[MethodImpl(MethodImplOptions.InternalCall)]
	public static extern bool ClearCache();

	public static bool ClearCache(int expiration)
	{
		return ClearCache_Int(expiration);
	}

	[MethodImpl(MethodImplOptions.InternalCall)]
	[NativeName("ClearCache")]
	internal static extern bool ClearCache_Int(int expiration);

	public static bool ClearCachedVersion(string assetBundleName, Hash128 hash)
	{
		if (string.IsNullOrEmpty(assetBundleName))
		{
			throw new ArgumentException("Input AssetBundle name cannot be null or empty.");
		}
		return ClearCachedVersionInternal(assetBundleName, hash);
	}

	[NativeName("ClearCachedVersion")]
	internal unsafe static bool ClearCachedVersionInternal(string assetBundleName, Hash128 hash)
	{
		//The blocks IL_0029 are reachable both inside and outside the pinned region starting at IL_0018. ILSpy has duplicated these blocks in order to place them both within and outside the `fixed` statement.
		try
		{
			ManagedSpanWrapper managedSpanWrapper = default(ManagedSpanWrapper);
			if (!StringMarshaller.TryMarshalEmptyOrNullString(assetBundleName, ref managedSpanWrapper))
			{
				ReadOnlySpan<char> readOnlySpan = MemoryExtensions.AsSpan(assetBundleName);
				fixed (char* begin = readOnlySpan)
				{
					managedSpanWrapper = new ManagedSpanWrapper(begin, readOnlySpan.Length);
					return ClearCachedVersionInternal_Injected(ref managedSpanWrapper, ref hash);
				}
			}
			return ClearCachedVersionInternal_Injected(ref managedSpanWrapper, ref hash);
		}
		finally
		{
		}
	}

	public static bool ClearOtherCachedVersions(string assetBundleName, Hash128 hash)
	{
		if (string.IsNullOrEmpty(assetBundleName))
		{
			throw new ArgumentException("Input AssetBundle name cannot be null or empty.");
		}
		return ClearCachedVersions(assetBundleName, hash, keepInputVersion: true);
	}

	public static bool ClearAllCachedVersions(string assetBundleName)
	{
		if (string.IsNullOrEmpty(assetBundleName))
		{
			throw new ArgumentException("Input AssetBundle name cannot be null or empty.");
		}
		return ClearCachedVersions(assetBundleName, default(Hash128), keepInputVersion: false);
	}

	internal unsafe static bool ClearCachedVersions(string assetBundleName, Hash128 hash, bool keepInputVersion)
	{
		//The blocks IL_0029 are reachable both inside and outside the pinned region starting at IL_0018. ILSpy has duplicated these blocks in order to place them both within and outside the `fixed` statement.
		try
		{
			ManagedSpanWrapper managedSpanWrapper = default(ManagedSpanWrapper);
			if (!StringMarshaller.TryMarshalEmptyOrNullString(assetBundleName, ref managedSpanWrapper))
			{
				ReadOnlySpan<char> readOnlySpan = MemoryExtensions.AsSpan(assetBundleName);
				fixed (char* begin = readOnlySpan)
				{
					managedSpanWrapper = new ManagedSpanWrapper(begin, readOnlySpan.Length);
					return ClearCachedVersions_Injected(ref managedSpanWrapper, ref hash, keepInputVersion);
				}
			}
			return ClearCachedVersions_Injected(ref managedSpanWrapper, ref hash, keepInputVersion);
		}
		finally
		{
		}
	}

	internal unsafe static Hash128[] GetCachedVersions(string assetBundleName)
	{
		//The blocks IL_0029 are reachable both inside and outside the pinned region starting at IL_0018. ILSpy has duplicated these blocks in order to place them both within and outside the `fixed` statement.
		BlittableArrayWrapper ret = default(BlittableArrayWrapper);
		Hash128[] result;
		try
		{
			ManagedSpanWrapper managedSpanWrapper = default(ManagedSpanWrapper);
			if (!StringMarshaller.TryMarshalEmptyOrNullString(assetBundleName, ref managedSpanWrapper))
			{
				ReadOnlySpan<char> readOnlySpan = MemoryExtensions.AsSpan(assetBundleName);
				fixed (char* begin = readOnlySpan)
				{
					managedSpanWrapper = new ManagedSpanWrapper(begin, readOnlySpan.Length);
					GetCachedVersions_Injected(ref managedSpanWrapper, out ret);
				}
			}
			else
			{
				GetCachedVersions_Injected(ref managedSpanWrapper, out ret);
			}
		}
		finally
		{
			Hash128[] array = default(Hash128[]);
			ret.Unmarshal(ref array);
			result = array;
		}
		return result;
	}

	public static void GetCachedVersions(string assetBundleName, List<Hash128> outCachedVersions)
	{
		if (string.IsNullOrEmpty(assetBundleName))
		{
			throw new ArgumentException("Input AssetBundle name cannot be null or empty.");
		}
		if (outCachedVersions == null)
		{
			throw new ArgumentNullException("Input outCachedVersions cannot be null.");
		}
		outCachedVersions.AddRange(GetCachedVersions(assetBundleName));
	}

	[Obsolete("Please use IsVersionCached with Hash128 instead.")]
	public static bool IsVersionCached(string url, int version)
	{
		return IsVersionCached(url, new Hash128(0u, 0u, 0u, (uint)version));
	}

	public static bool IsVersionCached(string url, Hash128 hash)
	{
		if (string.IsNullOrEmpty(url))
		{
			throw new ArgumentException("Input AssetBundle url cannot be null or empty.");
		}
		return IsVersionCached(url, "", hash);
	}

	public static bool IsVersionCached(CachedAssetBundle cachedBundle)
	{
		if (string.IsNullOrEmpty(cachedBundle.name))
		{
			throw new ArgumentException("Input AssetBundle name cannot be null or empty.");
		}
		return IsVersionCached("", cachedBundle.name, cachedBundle.hash);
	}

	[NativeName("IsCached")]
	internal unsafe static bool IsVersionCached(string url, string assetBundleName, Hash128 hash)
	{
		//The blocks IL_0029, IL_0036, IL_0044, IL_0052, IL_0057 are reachable both inside and outside the pinned region starting at IL_0018. ILSpy has duplicated these blocks in order to place them both within and outside the `fixed` statement.
		//The blocks IL_0057 are reachable both inside and outside the pinned region starting at IL_0044. ILSpy has duplicated these blocks in order to place them both within and outside the `fixed` statement.
		//The blocks IL_0057 are reachable both inside and outside the pinned region starting at IL_0044. ILSpy has duplicated these blocks in order to place them both within and outside the `fixed` statement.
		try
		{
			ManagedSpanWrapper managedSpanWrapper = default(ManagedSpanWrapper);
			ref ManagedSpanWrapper url2;
			ManagedSpanWrapper managedSpanWrapper2 = default(ManagedSpanWrapper);
			ReadOnlySpan<char> readOnlySpan2;
			if (!StringMarshaller.TryMarshalEmptyOrNullString(url, ref managedSpanWrapper))
			{
				ReadOnlySpan<char> readOnlySpan = MemoryExtensions.AsSpan(url);
				fixed (char* begin = readOnlySpan)
				{
					managedSpanWrapper = new ManagedSpanWrapper(begin, readOnlySpan.Length);
					url2 = ref managedSpanWrapper;
					if (!StringMarshaller.TryMarshalEmptyOrNullString(assetBundleName, ref managedSpanWrapper2))
					{
						readOnlySpan2 = MemoryExtensions.AsSpan(assetBundleName);
						fixed (char* begin2 = readOnlySpan2)
						{
							managedSpanWrapper2 = new ManagedSpanWrapper(begin2, readOnlySpan2.Length);
							return IsVersionCached_Injected(ref url2, ref managedSpanWrapper2, ref hash);
						}
					}
					return IsVersionCached_Injected(ref url2, ref managedSpanWrapper2, ref hash);
				}
			}
			url2 = ref managedSpanWrapper;
			if (!StringMarshaller.TryMarshalEmptyOrNullString(assetBundleName, ref managedSpanWrapper2))
			{
				readOnlySpan2 = MemoryExtensions.AsSpan(assetBundleName);
				fixed (char* begin2 = readOnlySpan2)
				{
					managedSpanWrapper2 = new ManagedSpanWrapper(begin2, readOnlySpan2.Length);
					return IsVersionCached_Injected(ref url2, ref managedSpanWrapper2, ref hash);
				}
			}
			return IsVersionCached_Injected(ref url2, ref managedSpanWrapper2, ref hash);
		}
		finally
		{
		}
	}

	[Obsolete("Please use MarkAsUsed with Hash128 instead.")]
	public static bool MarkAsUsed(string url, int version)
	{
		return MarkAsUsed(url, new Hash128(0u, 0u, 0u, (uint)version));
	}

	public static bool MarkAsUsed(string url, Hash128 hash)
	{
		if (string.IsNullOrEmpty(url))
		{
			throw new ArgumentException("Input AssetBundle url cannot be null or empty.");
		}
		return MarkAsUsed(url, "", hash);
	}

	public static bool MarkAsUsed(CachedAssetBundle cachedBundle)
	{
		if (string.IsNullOrEmpty(cachedBundle.name))
		{
			throw new ArgumentException("Input AssetBundle name cannot be null or empty.");
		}
		return MarkAsUsed("", cachedBundle.name, cachedBundle.hash);
	}

	internal unsafe static bool MarkAsUsed(string url, string assetBundleName, Hash128 hash)
	{
		//The blocks IL_0029, IL_0036, IL_0044, IL_0052, IL_0057 are reachable both inside and outside the pinned region starting at IL_0018. ILSpy has duplicated these blocks in order to place them both within and outside the `fixed` statement.
		//The blocks IL_0057 are reachable both inside and outside the pinned region starting at IL_0044. ILSpy has duplicated these blocks in order to place them both within and outside the `fixed` statement.
		//The blocks IL_0057 are reachable both inside and outside the pinned region starting at IL_0044. ILSpy has duplicated these blocks in order to place them both within and outside the `fixed` statement.
		try
		{
			ManagedSpanWrapper managedSpanWrapper = default(ManagedSpanWrapper);
			ref ManagedSpanWrapper url2;
			ManagedSpanWrapper managedSpanWrapper2 = default(ManagedSpanWrapper);
			ReadOnlySpan<char> readOnlySpan2;
			if (!StringMarshaller.TryMarshalEmptyOrNullString(url, ref managedSpanWrapper))
			{
				ReadOnlySpan<char> readOnlySpan = MemoryExtensions.AsSpan(url);
				fixed (char* begin = readOnlySpan)
				{
					managedSpanWrapper = new ManagedSpanWrapper(begin, readOnlySpan.Length);
					url2 = ref managedSpanWrapper;
					if (!StringMarshaller.TryMarshalEmptyOrNullString(assetBundleName, ref managedSpanWrapper2))
					{
						readOnlySpan2 = MemoryExtensions.AsSpan(assetBundleName);
						fixed (char* begin2 = readOnlySpan2)
						{
							managedSpanWrapper2 = new ManagedSpanWrapper(begin2, readOnlySpan2.Length);
							return MarkAsUsed_Injected(ref url2, ref managedSpanWrapper2, ref hash);
						}
					}
					return MarkAsUsed_Injected(ref url2, ref managedSpanWrapper2, ref hash);
				}
			}
			url2 = ref managedSpanWrapper;
			if (!StringMarshaller.TryMarshalEmptyOrNullString(assetBundleName, ref managedSpanWrapper2))
			{
				readOnlySpan2 = MemoryExtensions.AsSpan(assetBundleName);
				fixed (char* begin2 = readOnlySpan2)
				{
					managedSpanWrapper2 = new ManagedSpanWrapper(begin2, readOnlySpan2.Length);
					return MarkAsUsed_Injected(ref url2, ref managedSpanWrapper2, ref hash);
				}
			}
			return MarkAsUsed_Injected(ref url2, ref managedSpanWrapper2, ref hash);
		}
		finally
		{
		}
	}

	[Obsolete("This function is obsolete and will always return -1. Use IsVersionCached instead.")]
	public static int GetVersionFromCache(string url)
	{
		return -1;
	}

	public static Cache AddCache(string cachePath)
	{
		if (string.IsNullOrEmpty(cachePath))
		{
			throw new ArgumentNullException("Cache path cannot be null or empty.");
		}
		bool isReadonly = false;
		if (cachePath.Replace('\\', '/').StartsWith(Application.streamingAssetsPath))
		{
			isReadonly = true;
		}
		else
		{
			if (!Directory.Exists(cachePath))
			{
				throw new ArgumentException("Cache path '" + cachePath + "' doesn't exist.");
			}
			if ((File.GetAttributes(cachePath) & FileAttributes.ReadOnly) == FileAttributes.ReadOnly)
			{
				isReadonly = true;
			}
		}
		if (GetCacheByPath(cachePath).valid)
		{
			throw new InvalidOperationException("Cache with path '" + cachePath + "' has already been added.");
		}
		return AddCache(cachePath, isReadonly);
	}

	[NativeName("AddCachePath")]
	internal unsafe static Cache AddCache(string cachePath, bool isReadonly)
	{
		//The blocks IL_0029 are reachable both inside and outside the pinned region starting at IL_0018. ILSpy has duplicated these blocks in order to place them both within and outside the `fixed` statement.
		Cache ret = default(Cache);
		Cache result;
		try
		{
			ManagedSpanWrapper managedSpanWrapper = default(ManagedSpanWrapper);
			if (!StringMarshaller.TryMarshalEmptyOrNullString(cachePath, ref managedSpanWrapper))
			{
				ReadOnlySpan<char> readOnlySpan = MemoryExtensions.AsSpan(cachePath);
				fixed (char* begin = readOnlySpan)
				{
					managedSpanWrapper = new ManagedSpanWrapper(begin, readOnlySpan.Length);
					AddCache_Injected(ref managedSpanWrapper, isReadonly, out ret);
				}
			}
			else
			{
				AddCache_Injected(ref managedSpanWrapper, isReadonly, out ret);
			}
		}
		finally
		{
			result = ret;
		}
		return result;
	}

	[NativeName("Caching_GetCacheHandleAt")]
	[NativeThrows]
	[StaticAccessor("CachingManagerWrapper", StaticAccessorType.DoubleColon)]
	public static Cache GetCacheAt(int cacheIndex)
	{
		GetCacheAt_Injected(cacheIndex, out var ret);
		return ret;
	}

	[NativeThrows]
	[NativeName("Caching_GetCacheHandleByPath")]
	[StaticAccessor("CachingManagerWrapper", StaticAccessorType.DoubleColon)]
	public unsafe static Cache GetCacheByPath(string cachePath)
	{
		//The blocks IL_0029 are reachable both inside and outside the pinned region starting at IL_0018. ILSpy has duplicated these blocks in order to place them both within and outside the `fixed` statement.
		Cache ret = default(Cache);
		Cache result;
		try
		{
			ManagedSpanWrapper managedSpanWrapper = default(ManagedSpanWrapper);
			if (!StringMarshaller.TryMarshalEmptyOrNullString(cachePath, ref managedSpanWrapper))
			{
				ReadOnlySpan<char> readOnlySpan = MemoryExtensions.AsSpan(cachePath);
				fixed (char* begin = readOnlySpan)
				{
					managedSpanWrapper = new ManagedSpanWrapper(begin, readOnlySpan.Length);
					GetCacheByPath_Injected(ref managedSpanWrapper, out ret);
				}
			}
			else
			{
				GetCacheByPath_Injected(ref managedSpanWrapper, out ret);
			}
		}
		finally
		{
			result = ret;
		}
		return result;
	}

	public static void GetAllCachePaths(List<string> cachePaths)
	{
		cachePaths.Clear();
		for (int i = 0; i < cacheCount; i++)
		{
			cachePaths.Add(GetCacheAt(i).path);
		}
	}

	[NativeName("Caching_RemoveCacheByHandle")]
	[NativeThrows]
	[StaticAccessor("CachingManagerWrapper", StaticAccessorType.DoubleColon)]
	public static bool RemoveCache(Cache cache)
	{
		return RemoveCache_Injected(ref cache);
	}

	[NativeName("Caching_MoveCacheBeforeByHandle")]
	[StaticAccessor("CachingManagerWrapper", StaticAccessorType.DoubleColon)]
	[NativeThrows]
	public static void MoveCacheBefore(Cache src, Cache dst)
	{
		MoveCacheBefore_Injected(ref src, ref dst);
	}

	[NativeName("Caching_MoveCacheAfterByHandle")]
	[NativeThrows]
	[StaticAccessor("CachingManagerWrapper", StaticAccessorType.DoubleColon)]
	public static void MoveCacheAfter(Cache src, Cache dst)
	{
		MoveCacheAfter_Injected(ref src, ref dst);
	}

	[Obsolete("This function is obsolete. Please use ClearCache.  (UnityUpgradable) -> ClearCache()")]
	public static bool CleanCache()
	{
		return ClearCache();
	}

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern bool ClearCachedVersionInternal_Injected(ref ManagedSpanWrapper assetBundleName, [In] ref Hash128 hash);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern bool ClearCachedVersions_Injected(ref ManagedSpanWrapper assetBundleName, [In] ref Hash128 hash, bool keepInputVersion);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern void GetCachedVersions_Injected(ref ManagedSpanWrapper assetBundleName, out BlittableArrayWrapper ret);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern bool IsVersionCached_Injected(ref ManagedSpanWrapper url, ref ManagedSpanWrapper assetBundleName, [In] ref Hash128 hash);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern bool MarkAsUsed_Injected(ref ManagedSpanWrapper url, ref ManagedSpanWrapper assetBundleName, [In] ref Hash128 hash);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern void AddCache_Injected(ref ManagedSpanWrapper cachePath, bool isReadonly, out Cache ret);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern void GetCacheAt_Injected(int cacheIndex, out Cache ret);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern void GetCacheByPath_Injected(ref ManagedSpanWrapper cachePath, out Cache ret);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern bool RemoveCache_Injected([In] ref Cache cache);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern void MoveCacheBefore_Injected([In] ref Cache src, [In] ref Cache dst);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern void MoveCacheAfter_Injected([In] ref Cache src, [In] ref Cache dst);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern void get_defaultCache_Injected(out Cache ret);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern void get_currentCacheForWriting_Injected(out Cache ret);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern void set_currentCacheForWriting_Injected([In] ref Cache value);
}
