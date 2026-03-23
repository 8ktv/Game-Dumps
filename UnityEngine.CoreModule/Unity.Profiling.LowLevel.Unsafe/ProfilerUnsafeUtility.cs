using System;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Text;
using UnityEngine;
using UnityEngine.Bindings;
using UnityEngine.Scripting;

namespace Unity.Profiling.LowLevel.Unsafe;

[UsedByNativeCode]
[NativeHeader("Runtime/Profiler/ScriptBindings/ProfilerUnsafeUtility.bindings.h")]
[IgnoredByDeepProfiler]
public static class ProfilerUnsafeUtility
{
	public struct TimestampConversionRatio
	{
		public long Numerator;

		public long Denominator;
	}

	public const ushort CategoryRender = 0;

	public const ushort CategoryScripts = 1;

	public const ushort CategoryGUI = 4;

	public const ushort CategoryPhysics = 5;

	public const ushort CategoryAnimation = 6;

	public const ushort CategoryAi = 7;

	public const ushort CategoryAudio = 8;

	public const ushort CategoryVideo = 11;

	public const ushort CategoryParticles = 12;

	public const ushort CategoryLighting = 13;

	[Obsolete("CategoryLightning has been renamed. Use CategoryLighting instead (UnityUpgradable) -> CategoryLighting", false)]
	public const ushort CategoryLightning = 13;

	public const ushort CategoryNetwork = 14;

	public const ushort CategoryLoading = 15;

	public const ushort CategoryOther = 16;

	public const ushort CategoryVr = 22;

	public const ushort CategoryAllocation = 23;

	public const ushort CategoryInternal = 24;

	public const ushort CategoryFileIO = 25;

	public const ushort CategoryInput = 30;

	public const ushort CategoryVirtualTexturing = 31;

	internal const ushort CategoryGPU = 32;

	public const ushort CategoryPhysics2D = 33;

	internal const ushort CategoryAny = ushort.MaxValue;

	public static extern long Timestamp
	{
		[MethodImpl(MethodImplOptions.InternalCall)]
		[ThreadSafe]
		get;
	}

	public static TimestampConversionRatio TimestampToNanosecondsConversionRatio
	{
		[ThreadSafe]
		get
		{
			get_TimestampToNanosecondsConversionRatio_Injected(out var ret);
			return ret;
		}
	}

	[ThreadSafe]
	internal unsafe static ushort CreateCategory(string name, ProfilerCategoryColor colorIndex)
	{
		//The blocks IL_0029 are reachable both inside and outside the pinned region starting at IL_0018. ILSpy has duplicated these blocks in order to place them both within and outside the `fixed` statement.
		try
		{
			ManagedSpanWrapper managedSpanWrapper = default(ManagedSpanWrapper);
			if (!StringMarshaller.TryMarshalEmptyOrNullString(name, ref managedSpanWrapper))
			{
				ReadOnlySpan<char> readOnlySpan = MemoryExtensions.AsSpan(name);
				fixed (char* begin = readOnlySpan)
				{
					managedSpanWrapper = new ManagedSpanWrapper(begin, readOnlySpan.Length);
					return CreateCategory_Injected(ref managedSpanWrapper, colorIndex);
				}
			}
			return CreateCategory_Injected(ref managedSpanWrapper, colorIndex);
		}
		finally
		{
		}
	}

	[MethodImpl(MethodImplOptions.InternalCall)]
	[UnityEngine.Scripting.RequiredMember]
	[ThreadSafe]
	internal unsafe static extern ushort CreateCategory__Unmanaged(byte* name, int nameLen, ProfilerCategoryColor colorIndex);

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public unsafe static ushort CreateCategory(char* name, int nameLen, ProfilerCategoryColor colorIndex)
	{
		return CreateCategory_Unsafe(name, nameLen, colorIndex);
	}

	[MethodImpl(MethodImplOptions.InternalCall)]
	[ThreadSafe]
	private unsafe static extern ushort CreateCategory_Unsafe(char* name, int nameLen, ProfilerCategoryColor colorIndex);

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public unsafe static ushort GetCategoryByName(char* name, int nameLen)
	{
		return GetCategoryByName_Unsafe(name, nameLen);
	}

	[MethodImpl(MethodImplOptions.InternalCall)]
	[ThreadSafe]
	private unsafe static extern ushort GetCategoryByName_Unsafe(char* name, int nameLen);

	[ThreadSafe]
	public static ProfilerCategoryDescription GetCategoryDescription(ushort categoryId)
	{
		GetCategoryDescription_Injected(categoryId, out var ret);
		return ret;
	}

	[ThreadSafe]
	internal static Color32 GetCategoryColor(ProfilerCategoryColor colorIndex)
	{
		GetCategoryColor_Injected(colorIndex, out var ret);
		return ret;
	}

	[ThreadSafe]
	public unsafe static IntPtr CreateMarker(string name, ushort categoryId, MarkerFlags flags, int metadataCount)
	{
		//The blocks IL_0029 are reachable both inside and outside the pinned region starting at IL_0018. ILSpy has duplicated these blocks in order to place them both within and outside the `fixed` statement.
		try
		{
			ManagedSpanWrapper managedSpanWrapper = default(ManagedSpanWrapper);
			if (!StringMarshaller.TryMarshalEmptyOrNullString(name, ref managedSpanWrapper))
			{
				ReadOnlySpan<char> readOnlySpan = MemoryExtensions.AsSpan(name);
				fixed (char* begin = readOnlySpan)
				{
					managedSpanWrapper = new ManagedSpanWrapper(begin, readOnlySpan.Length);
					return CreateMarker_Injected(ref managedSpanWrapper, categoryId, flags, metadataCount);
				}
			}
			return CreateMarker_Injected(ref managedSpanWrapper, categoryId, flags, metadataCount);
		}
		finally
		{
		}
	}

	[MethodImpl(MethodImplOptions.InternalCall)]
	[ThreadSafe]
	[UnityEngine.Scripting.RequiredMember]
	internal unsafe static extern IntPtr CreateMarker__Unmanaged(byte* name, int nameLen, ushort categoryId, MarkerFlags flags, int metadataCount);

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public unsafe static IntPtr CreateMarker(char* name, int nameLen, ushort categoryId, MarkerFlags flags, int metadataCount)
	{
		return CreateMarker_Unsafe(name, nameLen, categoryId, flags, metadataCount);
	}

	[MethodImpl(MethodImplOptions.InternalCall)]
	[ThreadSafe]
	private unsafe static extern IntPtr CreateMarker_Unsafe(char* name, int nameLen, ushort categoryId, MarkerFlags flags, int metadataCount);

	[ThreadSafe]
	internal unsafe static IntPtr GetMarker(string name)
	{
		//The blocks IL_0029 are reachable both inside and outside the pinned region starting at IL_0018. ILSpy has duplicated these blocks in order to place them both within and outside the `fixed` statement.
		try
		{
			ManagedSpanWrapper managedSpanWrapper = default(ManagedSpanWrapper);
			if (!StringMarshaller.TryMarshalEmptyOrNullString(name, ref managedSpanWrapper))
			{
				ReadOnlySpan<char> readOnlySpan = MemoryExtensions.AsSpan(name);
				fixed (char* begin = readOnlySpan)
				{
					managedSpanWrapper = new ManagedSpanWrapper(begin, readOnlySpan.Length);
					return GetMarker_Injected(ref managedSpanWrapper);
				}
			}
			return GetMarker_Injected(ref managedSpanWrapper);
		}
		finally
		{
		}
	}

	[ThreadSafe]
	public unsafe static void SetMarkerMetadata(IntPtr markerPtr, int index, string name, byte type, byte unit)
	{
		//The blocks IL_002b are reachable both inside and outside the pinned region starting at IL_001a. ILSpy has duplicated these blocks in order to place them both within and outside the `fixed` statement.
		try
		{
			ManagedSpanWrapper managedSpanWrapper = default(ManagedSpanWrapper);
			if (!StringMarshaller.TryMarshalEmptyOrNullString(name, ref managedSpanWrapper))
			{
				ReadOnlySpan<char> readOnlySpan = MemoryExtensions.AsSpan(name);
				fixed (char* begin = readOnlySpan)
				{
					managedSpanWrapper = new ManagedSpanWrapper(begin, readOnlySpan.Length);
					SetMarkerMetadata_Injected(markerPtr, index, ref managedSpanWrapper, type, unit);
					return;
				}
			}
			SetMarkerMetadata_Injected(markerPtr, index, ref managedSpanWrapper, type, unit);
		}
		finally
		{
		}
	}

	[MethodImpl(MethodImplOptions.InternalCall)]
	[UnityEngine.Scripting.RequiredMember]
	[ThreadSafe]
	internal unsafe static extern void SetMarkerMetadata__Unmanaged(IntPtr markerPtr, int index, byte* name, int nameLen, byte type, byte unit);

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public unsafe static void SetMarkerMetadata(IntPtr markerPtr, int index, char* name, int nameLen, byte type, byte unit)
	{
		SetMarkerMetadata_Unsafe(markerPtr, index, name, nameLen, type, unit);
	}

	[MethodImpl(MethodImplOptions.InternalCall)]
	[ThreadSafe]
	private unsafe static extern void SetMarkerMetadata_Unsafe(IntPtr markerPtr, int index, char* name, int nameLen, byte type, byte unit);

	[MethodImpl(MethodImplOptions.InternalCall)]
	[ThreadSafe]
	public static extern void BeginSample(IntPtr markerPtr);

	[MethodImpl(MethodImplOptions.InternalCall)]
	[ThreadSafe]
	public unsafe static extern void BeginSampleWithMetadata(IntPtr markerPtr, int metadataCount, void* metadata);

	[MethodImpl(MethodImplOptions.InternalCall)]
	[ThreadSafe]
	public static extern void EndSample(IntPtr markerPtr);

	[MethodImpl(MethodImplOptions.InternalCall)]
	[ThreadSafe]
	public unsafe static extern void SingleSampleWithMetadata(IntPtr markerPtr, int metadataCount, void* metadata);

	[ThreadSafe]
	public unsafe static void* CreateCounterValue(out IntPtr counterPtr, string name, ushort categoryId, MarkerFlags flags, byte dataType, byte dataUnit, int dataSize, ProfilerCounterOptions counterOptions)
	{
		//The blocks IL_002a are reachable both inside and outside the pinned region starting at IL_0019. ILSpy has duplicated these blocks in order to place them both within and outside the `fixed` statement.
		try
		{
			ManagedSpanWrapper managedSpanWrapper = default(ManagedSpanWrapper);
			if (!StringMarshaller.TryMarshalEmptyOrNullString(name, ref managedSpanWrapper))
			{
				ReadOnlySpan<char> readOnlySpan = MemoryExtensions.AsSpan(name);
				fixed (char* begin = readOnlySpan)
				{
					managedSpanWrapper = new ManagedSpanWrapper(begin, readOnlySpan.Length);
					return CreateCounterValue_Injected(out counterPtr, ref managedSpanWrapper, categoryId, flags, dataType, dataUnit, dataSize, counterOptions);
				}
			}
			return CreateCounterValue_Injected(out counterPtr, ref managedSpanWrapper, categoryId, flags, dataType, dataUnit, dataSize, counterOptions);
		}
		finally
		{
		}
	}

	[MethodImpl(MethodImplOptions.InternalCall)]
	[UnityEngine.Scripting.RequiredMember]
	[ThreadSafe]
	internal unsafe static extern void* CreateCounterValue__Unmanaged(out IntPtr counterPtr, byte* name, int nameLen, ushort categoryId, MarkerFlags flags, byte dataType, byte dataUnit, int dataSize, ProfilerCounterOptions counterOptions);

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public unsafe static void* CreateCounterValue(out IntPtr counterPtr, char* name, int nameLen, ushort categoryId, MarkerFlags flags, byte dataType, byte dataUnit, int dataSize, ProfilerCounterOptions counterOptions)
	{
		return CreateCounterValue_Unsafe(out counterPtr, name, nameLen, categoryId, flags, dataType, dataUnit, dataSize, counterOptions);
	}

	[MethodImpl(MethodImplOptions.InternalCall)]
	[ThreadSafe]
	private unsafe static extern void* CreateCounterValue_Unsafe(out IntPtr counterPtr, char* name, int nameLen, ushort categoryId, MarkerFlags flags, byte dataType, byte dataUnit, int dataSize, ProfilerCounterOptions counterOptions);

	[MethodImpl(MethodImplOptions.InternalCall)]
	[ThreadSafe]
	public unsafe static extern void FlushCounterValue(void* counterValuePtr);

	internal unsafe static string Utf8ToString(byte* chars, int charsLen)
	{
		if (chars == null)
		{
			return null;
		}
		byte[] array = new byte[charsLen];
		Marshal.Copy((IntPtr)chars, array, 0, charsLen);
		return Encoding.UTF8.GetString(array, 0, charsLen);
	}

	[MethodImpl(MethodImplOptions.InternalCall)]
	[ThreadSafe]
	public static extern uint CreateFlow(ushort categoryId);

	[MethodImpl(MethodImplOptions.InternalCall)]
	[ThreadSafe]
	public static extern void FlowEvent(uint flowId, ProfilerFlowEventType flowEventType);

	[ThreadSafe]
	internal static void Internal_BeginWithObject(IntPtr markerPtr, UnityEngine.Object contextUnityObject)
	{
		Internal_BeginWithObject_Injected(markerPtr, UnityEngine.Object.MarshalledUnityObject.Marshal(contextUnityObject));
	}

	[NativeConditional("ENABLE_PROFILER")]
	internal static string Internal_GetName(IntPtr markerPtr)
	{
		ManagedSpanWrapper ret = default(ManagedSpanWrapper);
		string stringAndDispose;
		try
		{
			Internal_GetName_Injected(markerPtr, out ret);
		}
		finally
		{
			stringAndDispose = OutStringMarshaller.GetStringAndDispose(ret);
		}
		return stringAndDispose;
	}

	[ThreadSafe(ThrowsException = false)]
	[NativeConditional("ENABLE_MEM_PROFILER")]
	internal unsafe static IntPtr GetOrCreateMemLabel(string areaName, string objectName)
	{
		//The blocks IL_0029, IL_0036, IL_0044, IL_0052, IL_0057 are reachable both inside and outside the pinned region starting at IL_0018. ILSpy has duplicated these blocks in order to place them both within and outside the `fixed` statement.
		//The blocks IL_0057 are reachable both inside and outside the pinned region starting at IL_0044. ILSpy has duplicated these blocks in order to place them both within and outside the `fixed` statement.
		//The blocks IL_0057 are reachable both inside and outside the pinned region starting at IL_0044. ILSpy has duplicated these blocks in order to place them both within and outside the `fixed` statement.
		try
		{
			ManagedSpanWrapper managedSpanWrapper = default(ManagedSpanWrapper);
			ref ManagedSpanWrapper areaName2;
			ManagedSpanWrapper managedSpanWrapper2 = default(ManagedSpanWrapper);
			ReadOnlySpan<char> readOnlySpan2;
			if (!StringMarshaller.TryMarshalEmptyOrNullString(areaName, ref managedSpanWrapper))
			{
				ReadOnlySpan<char> readOnlySpan = MemoryExtensions.AsSpan(areaName);
				fixed (char* begin = readOnlySpan)
				{
					managedSpanWrapper = new ManagedSpanWrapper(begin, readOnlySpan.Length);
					areaName2 = ref managedSpanWrapper;
					if (!StringMarshaller.TryMarshalEmptyOrNullString(objectName, ref managedSpanWrapper2))
					{
						readOnlySpan2 = MemoryExtensions.AsSpan(objectName);
						fixed (char* begin2 = readOnlySpan2)
						{
							managedSpanWrapper2 = new ManagedSpanWrapper(begin2, readOnlySpan2.Length);
							return GetOrCreateMemLabel_Injected(ref areaName2, ref managedSpanWrapper2);
						}
					}
					return GetOrCreateMemLabel_Injected(ref areaName2, ref managedSpanWrapper2);
				}
			}
			areaName2 = ref managedSpanWrapper;
			if (!StringMarshaller.TryMarshalEmptyOrNullString(objectName, ref managedSpanWrapper2))
			{
				readOnlySpan2 = MemoryExtensions.AsSpan(objectName);
				fixed (char* begin2 = readOnlySpan2)
				{
					managedSpanWrapper2 = new ManagedSpanWrapper(begin2, readOnlySpan2.Length);
					return GetOrCreateMemLabel_Injected(ref areaName2, ref managedSpanWrapper2);
				}
			}
			return GetOrCreateMemLabel_Injected(ref areaName2, ref managedSpanWrapper2);
		}
		finally
		{
		}
	}

	[MethodImpl(MethodImplOptions.InternalCall)]
	[ThreadSafe(ThrowsException = true)]
	[NativeConditional("ENABLE_MEM_PROFILER")]
	internal static extern long GetMemLabelRelatedMemorySize(IntPtr label);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern ushort CreateCategory_Injected(ref ManagedSpanWrapper name, ProfilerCategoryColor colorIndex);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern void GetCategoryDescription_Injected(ushort categoryId, out ProfilerCategoryDescription ret);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern void GetCategoryColor_Injected(ProfilerCategoryColor colorIndex, out Color32 ret);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern IntPtr CreateMarker_Injected(ref ManagedSpanWrapper name, ushort categoryId, MarkerFlags flags, int metadataCount);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern IntPtr GetMarker_Injected(ref ManagedSpanWrapper name);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern void SetMarkerMetadata_Injected(IntPtr markerPtr, int index, ref ManagedSpanWrapper name, byte type, byte unit);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private unsafe static extern void* CreateCounterValue_Injected(out IntPtr counterPtr, ref ManagedSpanWrapper name, ushort categoryId, MarkerFlags flags, byte dataType, byte dataUnit, int dataSize, ProfilerCounterOptions counterOptions);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern void Internal_BeginWithObject_Injected(IntPtr markerPtr, IntPtr contextUnityObject);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern void Internal_GetName_Injected(IntPtr markerPtr, out ManagedSpanWrapper ret);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern void get_TimestampToNanosecondsConversionRatio_Injected(out TimestampConversionRatio ret);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern IntPtr GetOrCreateMemLabel_Injected(ref ManagedSpanWrapper areaName, ref ManagedSpanWrapper objectName);
}
